using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SebbyLib;
using SharpDX;
using Spell = LeagueSharp.Common.Spell;

namespace OneKeyToWin_AIO_Sebby.Core
{
    internal class HiddenObj
    {
        public int type;
        //0 - missile
        //1 - normal
        //2 - pink
        //3 - teemo trap
        public float endTime { get; set; }
        public Vector3 pos { get; set; }
    }

    internal class OKTWward
    {
        private static Menu Sub;
        private static Spell Q, W, E, R;

        public static List<HiddenObj> HiddenObjList = new List<HiddenObj>();
        private readonly Menu Config = Program.Config;
        private bool rengar;
        private AIHeroClient Vayne;

        private readonly Items.Item
            VisionWard = new Items.Item(2043, 550f);

        private readonly Items.Item
            OracleLens = new Items.Item(3364, 550f);

        private readonly Items.Item
            WardN = new Items.Item(2044, 600f);

        private readonly Items.Item
            TrinketN = new Items.Item(3340, 600f);

        private readonly Items.Item
            SightStone = new Items.Item(2049, 600f);

        private readonly Items.Item
            EOTOasis = new Items.Item(2302, 600f);

        private readonly Items.Item
            EOTEquinox = new Items.Item(2303, 600f);

        private readonly Items.Item
            EOTWatchers = new Items.Item(2301, 600f);

        private readonly Items.Item
            FarsightOrb = new Items.Item(3342, 4000f);

        private readonly Items.Item
            ScryingOrb = new Items.Item(3363, 3500f);

        public AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public static bool getCheckBoxItem(string item)
        {
            return Sub[item].Cast<CheckBox>().CurrentValue;
        }

        public void LoadOKTW()
        {
            Q = new Spell(SpellSlot.Q);
            E = new Spell(SpellSlot.E);
            W = new Spell(SpellSlot.W);
            R = new Spell(SpellSlot.R);

            Sub = Config.AddSubMenu("AutoWard OKTW©");
            Sub.Add("AutoWard", new CheckBox("Auto Ward"));
            Sub.Add("autoBuy", new CheckBox("Auto buy blue trinket after lvl 9", false));
            Sub.Add("AutoWardBlue", new CheckBox("Auto Blue Trinket"));
            Sub.Add("AutoWardCombo", new CheckBox("Only combo mode"));
            Sub.Add("AutoWardPink", new CheckBox("Auto VisionWard, OracleLens"));

            foreach (var hero in ObjectManager.Get<AIHeroClient>())
            {
                if (hero.IsEnemy)
                {
                    if (hero.ChampionName == "Rengar")
                        rengar = true;
                    if (hero.ChampionName == "Vayne")
                        Vayne = hero;
                }
            }

            Game.OnUpdate += Game_OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if (!Program.LagFree(0) || Player.IsRecalling() || Player.IsDead)
                return;

            foreach (var obj in HiddenObjList)
            {
                if (obj.endTime < Game.Time)
                {
                    HiddenObjList.Remove(obj);
                    return;
                }
            }

            if (getCheckBoxItem("autoBuy") && Player.InFountain() && !ScryingOrb.IsOwned() && Player.Level >= 9)
            {
                var itm = new Item(ItemId.Farsight_Alteration);
                itm.Buy();
            }

            if (rengar && Player.HasBuff("rengarralertsound"))
                CastVisionWards(Player.ServerPosition);

            if (Vayne != null && Vayne.IsValidTarget(1000) && Vayne.HasBuff("vaynetumblefade"))
                CastVisionWards(Vayne.ServerPosition);

            AutoWardLogic();
        }

        private void AutoWardLogic()
        {
            foreach (var enemy in Program.Enemies.Where(enemy => enemy.IsValid && !enemy.IsVisible && !enemy.IsDead))
            {
                var need = OKTWtracker.ChampionInfoList.Find(x => x.NetworkId == enemy.NetworkId);

                if (need == null || need.PredictedPos == null)
                    continue;

                var PPDistance = need.PredictedPos.Distance(Player.Position);

                if (PPDistance > 1400)
                    continue;

                var timer = Game.Time - need.LastVisableTime;

                if (timer > 1 && timer < 3)
                {
                    //if (Program.Combo && PPDistance < 1500 && Player.ChampionName == "Quinn" && W.IsReady() && Config.Item("autoW", true).GetValue<bool>())
                    //{
                    //W.Cast();
                    //}

                    if (Program.Combo && PPDistance < 900 && Player.ChampionName == "Karhus" && Q.IsReady() &&
                        Player.CountEnemiesInRange(900) == 0)
                    {
                        Q.Cast(need.PredictedPos);
                    }

                    if (Program.Combo && PPDistance < 1400 && Player.ChampionName == "Ashe" && E.IsReady() &&
                        Player.CountEnemiesInRange(800) == 0 && PortAIO.Champion.Ashe.Program.getAutoE())
                    {
                        E.Cast(Player.Position.Extend(need.PredictedPos, 5000));
                    }

                    if (PPDistance < 800 && Player.ChampionName == "MissFortune" && E.IsReady() && Program.Combo &&
                        Player.Mana > 200)
                    {
                        E.Cast(Player.Position.Extend(need.PredictedPos, 800));
                    }

                    if (!Orbwalker.IsAutoAttacking && PPDistance < 800 && Player.ChampionName == "Caitlyn" && W.IsReady() && Player.Mana > 200f && PortAIO.Champion.Caitlyn.Program.getBushW() && Utils.TickCount - W.LastCastAttemptT > 2000)
                    {
                        W.Cast(need.PredictedPos);
                    }
                    //if (PPDistance < 760 && Player.ChampionName == "Jhin" && E.IsReady() && Player.Mana > 200f && Config.Item("bushE", true).GetValue<bool>() && Utils.TickCount - E.LastCastAttemptT > 2000)
                    //{
                    //E.Cast(need.PredictedPos);
                    //}
                }

                if (timer < 4)
                {
                    if (getCheckBoxItem("AutoWardCombo") && Program.AIOmode != 2 && !Program.Combo)
                        return;

                    if (NavMesh.IsWallOfGrass(need.PredictedPos, 0))
                    {
                        if (PPDistance < 600 && getCheckBoxItem("AutoWard"))
                        {
                            if (TrinketN.IsReady())
                            {
                                TrinketN.Cast(need.PredictedPos);
                                need.LastVisableTime = Game.Time - 5;
                            }
                            else if (SightStone.IsReady())
                            {
                                SightStone.Cast(need.PredictedPos);
                                need.LastVisableTime = Game.Time - 5;
                            }
                            else if (WardN.IsReady())
                            {
                                WardN.Cast(need.PredictedPos);
                                need.LastVisableTime = Game.Time - 5;
                            }
                            else if (EOTOasis.IsReady())
                            {
                                EOTOasis.Cast(need.PredictedPos);
                                need.LastVisableTime = Game.Time - 5;
                            }
                            else if (EOTEquinox.IsReady())
                            {
                                EOTEquinox.Cast(need.PredictedPos);
                                need.LastVisableTime = Game.Time - 5;
                            }
                            else if (EOTWatchers.IsReady())
                            {
                                EOTWatchers.Cast(need.PredictedPos);
                                need.LastVisableTime = Game.Time - 5;
                            }
                        }

                        if (getCheckBoxItem("AutoWardBlue"))
                        {
                            if (FarsightOrb.IsReady())
                            {
                                FarsightOrb.Cast(need.PredictedPos);
                                need.LastVisableTime = Game.Time - 5;
                            }
                            else if (ScryingOrb.IsReady())
                            {
                                ScryingOrb.Cast(need.PredictedPos);
                                need.LastVisableTime = Game.Time - 5;
                            }
                        }
                    }
                }
            }
        }

        private void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (!sender.IsEnemy || sender.IsAlly)
                return;
            var missile = sender as MissileClient;
            if (missile != null)
            {
                if (!missile.SpellCaster.IsVisible)
                {
                    if ((missile.SData.Name == "BantamTrapShort" || missile.SData.Name == "BantamTrapBounceSpell") &&
                        !HiddenObjList.Exists(x => missile.EndPosition == x.pos))
                        AddWard("teemorcast", missile.EndPosition);
                }
            }

            var minion = sender as Obj_AI_Minion;
            if (minion != null)
            {
                if ((sender.Name.ToLower() == "visionward" || sender.Name.ToLower() == "sightward") &&
                    !HiddenObjList.Exists(x => x.pos.Distance(sender.Position) < 100))
                {
                    foreach (var obj in HiddenObjList)
                    {
                        if (obj.pos.Distance(sender.Position) < 400)
                        {
                            if (obj.type == 0)
                            {
                                HiddenObjList.Remove(obj);
                                return;
                            }
                        }
                    }

                    var dupa = (Obj_AI_Minion)sender;
                    if (dupa.Mana == 0)
                        HiddenObjList.Add(new HiddenObj { type = 2, pos = sender.Position, endTime = float.MaxValue });
                    else
                        HiddenObjList.Add(new HiddenObj
                        {
                            type = 1,
                            pos = sender.Position,
                            endTime = Game.Time + dupa.Mana
                        });
                }
            }
            else if (rengar && sender.Position.Distance(Player.Position) < 800)
            {
                switch (sender.Name)
                {
                    case "Rengar_LeapSound.troy":
                        CastVisionWards(sender.Position);
                        break;
                    case "Rengar_Base_R_Alert":
                        CastVisionWards(sender.Position);
                        break;
                }
            }
        }

        private void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            var minion = sender as Obj_AI_Minion;
            if (minion != null && minion.Health < 100)
            {
                foreach (var obj in HiddenObjList)
                {
                    if (obj.pos == sender.Position)
                    {
                        HiddenObjList.Remove(obj);
                        return;
                    }
                    if (obj.type == 3 && obj.pos.Distance(sender.Position) < 100)
                    {
                        HiddenObjList.Remove(obj);
                        return;
                    }
                    if (obj.pos.Distance(sender.Position) < 400)
                    {
                        if (obj.type == 2 && sender.Name.ToLower() == "visionward")
                        {
                            HiddenObjList.Remove(obj);
                            return;
                        }
                        if ((obj.type == 0 || obj.type == 1) && sender.Name.ToLower() == "sightward")
                        {
                            HiddenObjList.Remove(obj);
                            return;
                        }
                    }
                }
            }
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender is AIHeroClient && sender.IsEnemy)
            {
                if (args.Target == null)
                    AddWard(args.SData.Name.ToLower(), args.End);

                if ((OracleLens.IsReady() || VisionWard.IsReady()) && sender.Distance(Player.Position) < 1200)
                {
                    switch (args.SData.Name.ToLower())
                    {
                        case "akalismokebomb":
                            CastVisionWards(sender.ServerPosition);
                            break;
                        case "deceive":
                            CastVisionWards(sender.ServerPosition);
                            break;
                        case "khazixr":
                            CastVisionWards(sender.ServerPosition);
                            break;
                        case "khazixrlong":
                            CastVisionWards(sender.ServerPosition);
                            break;
                        case "talonshadowassault":
                            CastVisionWards(sender.ServerPosition);
                            break;
                        case "monkeykingdecoy":
                            CastVisionWards(sender.ServerPosition);
                            break;
                        case "rengarr":
                            CastVisionWards(sender.ServerPosition);
                            break;
                        case "twitchhideinshadows":
                            CastVisionWards(sender.ServerPosition);
                            break;
                    }
                }
            }
        }

        private void AddWard(string name, Vector3 posCast)
        {
            switch (name)
            {
                //PINKS
                case "visionward":
                    HiddenObjList.Add(new HiddenObj { type = 2, pos = posCast, endTime = float.MaxValue });
                    break;
                case "trinkettotemlvl3B":
                    HiddenObjList.Add(new HiddenObj { type = 1, pos = posCast, endTime = Game.Time + 180 });
                    break;
                //SIGH WARD
                case "itemghostward":
                    HiddenObjList.Add(new HiddenObj { type = 1, pos = posCast, endTime = Game.Time + 180 });
                    break;
                case "wrigglelantern":
                    HiddenObjList.Add(new HiddenObj { type = 1, pos = posCast, endTime = Game.Time + 180 });
                    break;
                case "sightward":
                    HiddenObjList.Add(new HiddenObj { type = 1, pos = posCast, endTime = Game.Time + 180 });
                    break;
                case "itemferalflare":
                    HiddenObjList.Add(new HiddenObj { type = 1, pos = posCast, endTime = Game.Time + 180 });
                    break;
                //TRINKET
                case "trinkettotemlvl1":
                    HiddenObjList.Add(new HiddenObj { type = 1, pos = posCast, endTime = Game.Time + 60 });
                    break;
                case "trinkettotemlvl2":
                    HiddenObjList.Add(new HiddenObj { type = 1, pos = posCast, endTime = Game.Time + 120 });
                    break;
                case "trinkettotemlvl3":
                    HiddenObjList.Add(new HiddenObj { type = 1, pos = posCast, endTime = Game.Time + 180 });
                    break;
                //others
                case "teemorcast":
                    HiddenObjList.Add(new HiddenObj { type = 3, pos = posCast, endTime = Game.Time + 300 });
                    break;
                case "noxious trap":
                    HiddenObjList.Add(new HiddenObj { type = 3, pos = posCast, endTime = Game.Time + 300 });
                    break;
                case "JackInTheBox":
                    HiddenObjList.Add(new HiddenObj { type = 3, pos = posCast, endTime = Game.Time + 100 });
                    break;
                case "Jack In The Box":
                    HiddenObjList.Add(new HiddenObj { type = 3, pos = posCast, endTime = Game.Time + 100 });
                    break;
            }
        }

        private void CastVisionWards(Vector3 position)
        {
            if (getCheckBoxItem("AutoWardPink"))
            {
                if (OracleLens.IsReady())
                    OracleLens.Cast(Player.Position.Extend(position, OracleLens.Range));
                else if (VisionWard.IsReady())
                    VisionWard.Cast(Player.Position.Extend(position, VisionWard.Range));
            }
        }
    }
}