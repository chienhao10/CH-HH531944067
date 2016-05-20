using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SebbyLib;
using SharpDX;
using Color = System.Drawing.Color;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    internal class Swain
    {
        private static readonly Menu Config = Program.Config;
        private static Spell E, Q, R, W;
        private static float QMANA, WMANA, EMANA, RMANA;
        private static bool Ractive;

        private static readonly string[] Spells =
        {
            "katarinar", "drain", "consume", "absolutezero", "staticfield", "reapthewhirlwind", "jinxw", "jinxr",
            "shenstandunited", "threshe", "threshrpenta", "threshq", "meditate", "caitlynpiltoverpeacemaker",
            "volibearqattack",
            "cassiopeiapetrifyinggaze", "ezrealtrueshotbarrage", "galioidolofdurand", "luxmalicecannon",
            "missfortunebullettime", "infiniteduress", "alzaharnethergrasp", "lucianq", "velkozr", "rocketgrabmissile"
        };

        public static Menu draw, q, w, e, r, farm;

        public static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        public static void LoadOKTW()
        {
            Q = new Spell(SpellSlot.Q, 700);
            W = new Spell(SpellSlot.W, 900);
            E = new Spell(SpellSlot.E, 625);
            R = new Spell(SpellSlot.R, 675);

            Q.SetSkillshot(0.5f, 200f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(1.5f, 240f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            draw = Config.AddSubMenu("Draw");
            draw.Add("qRange", new CheckBox("Q range", false));
            draw.Add("wRange", new CheckBox("W range", false));
            draw.Add("eRange", new CheckBox("E range", false));
            draw.Add("rRange", new CheckBox("R range", false));
            draw.Add("onlyRdy", new CheckBox("Draw only ready spells"));

            q = Config.AddSubMenu("Q Config");
            q.Add("autoQ", new CheckBox("Auto Q"));
            q.Add("harrasQ", new CheckBox("Harass Q"));
            q.AddSeparator();
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                q.Add("Quse" + enemy.NetworkId, new CheckBox("Q : " + enemy.ChampionName));

            w = Config.AddSubMenu("W Config");
            w.Add("autoW", new CheckBox("Auto W on hard CC"));
            w.Add("Wspell", new CheckBox("W on special spell detection"));
            w.Add("Int", new CheckBox("W On Interruptable Target"));
            w.Add("WmodeCombo", new ComboBox("W combo mode", 1, "always", "run - cheese"));
            w.Add("Waoe", new Slider("Auto W x enemies", 3, 0, 5));
            w.Add("WmodeGC", new ComboBox("Gap Closer position mode", 0, "Dash end position", "My hero position"));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                w.Add("WGCchampion" + enemy.NetworkId, new CheckBox("W : " + enemy.ChampionName));

            e = Config.AddSubMenu("E Config");
            e.Add("autoE", new CheckBox("Auto E"));
            e.Add("harrasE", new CheckBox("Harass E"));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                e.Add("Euse" + enemy.NetworkId, new CheckBox(enemy.ChampionName));

            r = Config.AddSubMenu("R Config");
            r.Add("autoR", new CheckBox("Auto R"));
            r.Add("harrasR", new CheckBox("Harass R"));
            r.Add("Raoe", new Slider("Auto R if x enemies in range", 2, 1, 5));

            farm = Config.AddSubMenu("Farm");
            farm.Add("farmW", new CheckBox("Lane clear W"));
            farm.Add("farmR", new CheckBox("Lane clear R"));
            farm.Add("Mana", new Slider("LaneClear Mana", 80));
            farm.Add("LCminions", new Slider("LaneClear minimum minions", 3, 0, 10));
            farm.Add("jungleQ", new CheckBox("Jungle clear Q"));
            farm.Add("jungleW", new CheckBox("Jungle clear W"));
            farm.Add("jungleE", new CheckBox("Jungle clear E"));
            farm.Add("jungleR", new CheckBox("Jungle clear R"));

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (W.IsReady() && getCheckBoxItem(w, "Int") && sender.IsValidTarget(W.Range))
                W.Cast(sender.Position);
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!W.IsReady() || sender.IsMinion || !sender.IsEnemy || !getCheckBoxItem(w, "Wspell") ||
                !sender.IsValid<AIHeroClient>() || !sender.IsValidTarget(W.Range))
                return;

            var foundSpell = Spells.Find(x => args.SData.Name.ToLower() == x);
            if (foundSpell != null)
            {
                W.Cast(sender.Position);
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (W.IsReady() && Player.Mana > RMANA + WMANA)
            {
                var t = gapcloser.Sender;
                if (t.IsValidTarget(W.Range) && getCheckBoxItem(w, "WGCchampion" + t.NetworkId))
                {
                    W.Cast(getBoxItem(w, "WmodeGC") == 0 ? gapcloser.End : Player.ServerPosition);
                }
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Program.LagFree(0))
            {
                SetMana();
                Ractive = Player.HasBuff("SwainMetamorphism");
                Jungle();
            }

            if (Program.LagFree(1) && E.IsReady() && getCheckBoxItem(e, "autoE"))
                LogicE();

            if (Program.LagFree(2) && Q.IsReady() && getCheckBoxItem(q, "autoQ"))
                LogicQ();

            if (Program.LagFree(3) && W.IsReady())
                LogicW();

            if (Program.LagFree(4) && R.IsReady() && getCheckBoxItem(r, "autoR"))
                LogicR();
        }

        private static void LogicR()
        {
            if (Ractive)
            {
                if (Program.LaneClear && getCheckBoxItem(farm, "farmR"))
                {
                    var allMinions = Cache.GetMinions(Player.Position, R.Range);
                    var mobs = Cache.GetMinions(Player.Position, R.Range, MinionTeam.Neutral);
                    if (mobs.Count > 0)
                    {
                        if (!getCheckBoxItem(farm, "jungleR"))
                        {
                            R.Cast();
                        }
                    }
                    else if (allMinions.Count > 0)
                    {
                        if (allMinions.Count < 2 || Player.ManaPercent < getSliderItem(farm, "Mana"))
                            R.Cast();
                        else if (Player.ManaPercent < getSliderItem(farm, "Mana"))
                            R.Cast();
                    }
                    else
                        R.Cast();
                }
                else if ((Player.Position.CountEnemiesInRange(R.Range + 400) == 0 || Player.Mana < EMANA) &&
                         ((Program.Farm && getCheckBoxItem(farm, "farmR")) || Program.None))
                {
                    R.Cast();
                }
            }
            else
            {
                var countAOE = Player.CountEnemiesInRange(R.Range);
                if (countAOE > 0)
                {
                    if (Program.Combo && getCheckBoxItem(r, "autoR"))
                        R.Cast();
                    else if (Program.Farm && getCheckBoxItem(r, "harrasR"))
                        R.Cast();
                    else if (countAOE >= getSliderItem(r, "Raoe"))
                        R.Cast();
                }
                if (Program.LaneClear && Player.ManaPercent > getSliderItem(farm, "Mana") &&
                    getCheckBoxItem(farm, "farmR"))
                {
                    var allMinions = Cache.GetMinions(Player.ServerPosition, R.Range);

                    if (allMinions.Count >= getSliderItem(farm, "LCminions"))
                        R.Cast();
                }
            }
        }

        private static void LogicW()
        {
            var t = TargetSelector.GetTarget(W.Range, DamageType.Magical);
            if (t.IsValidTarget())
            {
                if (Program.Combo)
                {
                    if (getBoxItem(w, "WmodeCombo") == 1)
                    {
                        if (W.GetPrediction(t).CastPosition.LSDistance(t.Position) > 100)
                        {
                            if (Player.Position.LSDistance(t.ServerPosition) > Player.Position.LSDistance(t.Position))
                            {
                                if (t.Position.LSDistance(Player.ServerPosition) < t.Position.LSDistance(Player.Position))
                                    Program.CastSpell(W, t);
                            }
                            else
                            {
                                if (t.Position.LSDistance(Player.ServerPosition) > t.Position.LSDistance(Player.Position))
                                    Program.CastSpell(W, t);
                            }
                        }
                    }
                    else
                    {
                        Program.CastSpell(W, t);
                    }
                }

                W.CastIfWillHit(t, getSliderItem(w, "Waoe"));
            }
            else if (Program.LaneClear && Player.ManaPercent > getSliderItem(farm, "Mana") &&
                     getCheckBoxItem(farm, "farmW"))
            {
                var minionList = Cache.GetMinions(Player.ServerPosition, W.Range);
                var farmPosition = W.GetCircularFarmLocation(minionList, W.Width);

                if (farmPosition.MinionsHit > getSliderItem(farm, "LCminions"))
                    W.Cast(farmPosition.Position);
            }

            if (getCheckBoxItem(w, "autoW"))
                foreach (
                    var enemy in
                        Program.Enemies.Where(enemy => enemy.IsValidTarget(W.Range) && !OktwCommon.CanMove(enemy)))
                    W.Cast(enemy, true);
        }

        private static void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (t.IsValidTarget())
            {
                if (t.Health < OktwCommon.GetKsDamage(t, Q) + E.GetDamage(t))
                    Q.Cast(t);
                if (!getCheckBoxItem(q, "Quse" + t.NetworkId))
                    return;
                if (Program.Combo && Player.Mana > RMANA + EMANA)
                    Q.Cast(t);
                else if (Program.Farm && getCheckBoxItem(q, "harrasQ") && Player.Mana > RMANA + EMANA + WMANA + EMANA)
                    Q.Cast(t);
                else if ((Program.Combo || Program.Farm))
                {
                    foreach (var enemy in Program.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                        Q.Cast(enemy);
                }
            }
        }

        private static void LogicE()
        {
            var t = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            if (t.IsValidTarget())
            {
                if (t.Health < E.GetDamage(t) + OktwCommon.GetKsDamage(t, Q))
                    E.CastOnUnit(t);
                if (!getCheckBoxItem(e, "Euse" + t.NetworkId))
                    return;
                if (Program.Combo && Player.Mana > RMANA + EMANA)
                    E.CastOnUnit(t);
                else if (Program.Farm && getCheckBoxItem(e, "harrasE") && Player.Mana > RMANA + EMANA + WMANA + EMANA)
                    E.CastOnUnit(t);
            }
        }

        private static void Jungle()
        {
            if (Program.LaneClear)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, Q.Range, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];

                    if (W.IsReady() && getCheckBoxItem(farm, "jungleW"))
                    {
                        W.Cast(mob.ServerPosition);
                        return;
                    }
                    if (E.IsReady() && getCheckBoxItem(farm, "jungleE"))
                    {
                        E.CastOnUnit(mob);
                        return;
                    }
                    if (Q.IsReady() && getCheckBoxItem(farm, "jungleQ"))
                    {
                        Q.CastOnUnit(mob);
                        return;
                    }
                    if (R.IsReady() && getCheckBoxItem(farm, "jungleR") && !Ractive)
                    {
                        R.Cast();
                    }
                }
            }
        }

        private static void SetMana()
        {
            if ((Program.getCheckBoxItem("manaDisable") && Program.Combo) || Player.HealthPercent < 20)
            {
                QMANA = 0;
                WMANA = 0;
                EMANA = 0;
                RMANA = 0;
                return;
            }

            QMANA = Q.Instance.SData.Mana;
            WMANA = W.Instance.SData.Mana;
            EMANA = E.Instance.SData.Mana;

            if (!R.IsReady())
                RMANA = WMANA - Player.PARRegenRate*W.Instance.Cooldown;
            else
                RMANA = R.Instance.SData.Mana;
        }

        public static void drawLine(Vector3 pos1, Vector3 pos2, int bold, Color color)
        {
            var wts1 = Drawing.WorldToScreen(pos1);
            var wts2 = Drawing.WorldToScreen(pos2);

            Drawing.DrawLine(wts1[0], wts1[1], wts2[0], wts2[1], bold, color);
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(draw, "qRange"))
            {
                if (getCheckBoxItem(draw, "onlyRdy"))
                {
                    if (Q.IsReady())
                        Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Cyan, 1, 1);
                }
                else
                    Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Cyan, 1, 1);
            }
            if (getCheckBoxItem(draw, "wRange"))
            {
                if (getCheckBoxItem(draw, "onlyRdy"))
                {
                    if (W.IsReady())
                        Utility.DrawCircle(ObjectManager.Player.Position, W.Range, Color.Orange, 1, 1);
                }
                else
                    Utility.DrawCircle(ObjectManager.Player.Position, W.Range, Color.Orange, 1, 1);
            }
            if (getCheckBoxItem(draw, "eRange"))
            {
                if (getCheckBoxItem(draw, "onlyRdy"))
                {
                    if (E.IsReady())
                        Utility.DrawCircle(ObjectManager.Player.Position, E.Range, Color.Yellow, 1, 1);
                }
                else
                    Utility.DrawCircle(ObjectManager.Player.Position, E.Range, Color.Yellow, 1, 1);
            }
            if (getCheckBoxItem(draw, "rRange"))
            {
                if (getCheckBoxItem(draw, "onlyRdy"))
                {
                    if (R.IsReady())
                        Utility.DrawCircle(ObjectManager.Player.Position, R.Range, Color.Gray, 1, 1);
                }
                else
                    Utility.DrawCircle(ObjectManager.Player.Position, R.Range, Color.Gray, 1, 1);
            }
        }
    }
}