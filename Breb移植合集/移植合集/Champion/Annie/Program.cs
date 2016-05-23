using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SebbyLib;
using HealthPrediction = SebbyLib.HealthPrediction;
using Spell = LeagueSharp.Common.Spell;

namespace PortAIO.Champion.Annie
{
    internal class Program
    {
        private static readonly Menu Config = SebbyLib.Program.Config;
        public static Spell Q, W, E, R, FR;
        private static Menu drawMenu, QMenu, WMenu, EMenu, RMenu, FarmMenu;
        public static float QMANA, WMANA, EMANA, RMANA;
        private static SpellSlot flash;
        public static Obj_AI_Base Tibbers;
        public static float TibbersTimer = 0;
        private static bool HaveStun;

        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        private static bool HaveTibers
        {
            get { return Player.HasBuff("infernalguardiantimer"); }
        }

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static void LoadOKTW()
        {
            Q = new Spell(SpellSlot.Q, 625f);
            W = new Spell(SpellSlot.W, 550f);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 625f);
            FR = new Spell(SpellSlot.R, 1000f);

            Q.SetTargetted(0.25f, 1400f);
            W.SetSkillshot(0.3f, 80f, float.MaxValue, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.25f, 180f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            FR.SetSkillshot(0.25f, 180f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            flash = Player.GetSpellSlot("summonerflash");

            drawMenu = Config.AddSubMenu("线圈");
            drawMenu.Add("qRange", new CheckBox("Q 范围"));
            drawMenu.Add("wRange", new CheckBox("W 范围"));
            drawMenu.Add("rRange", new CheckBox("R 范围"));
            drawMenu.Add("onlyRdy", new CheckBox("只显示无冷却技能"));

            QMenu = Config.AddSubMenu("Q 设置");
            QMenu.Add("autoQ", new CheckBox("自动 Q"));
            QMenu.Add("harrasQ", new CheckBox("骚扰 Q"));

            WMenu = Config.AddSubMenu("W 设置");
            WMenu.Add("autoW", new CheckBox("自动 W"));
            WMenu.Add("harrasW", new CheckBox("骚扰 W"));

            EMenu = Config.AddSubMenu("E 设置");
            EMenu.Add("autoE", new CheckBox("自动叠加E"));

            RMenu = Config.AddSubMenu("R 设置");
            RMenu.AddLabel("0 : 普通");
            RMenu.AddLabel("1 : 一直");
            RMenu.AddLabel("2 : 从不");
            RMenu.AddLabel("3 : 可晕眩时");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
            {
                RMenu.Add("UM" + enemy.NetworkId, new Slider(enemy.ChampionName, 0, 0, 3));
            }
            RMenu.AddSeparator();
            RMenu.Add("autoRks", new CheckBox("自动 R 抢头"));
            RMenu.Add("autoRcombo", new CheckBox("连招自动R，如果可晕眩"));
            RMenu.Add("rCount", new Slider("自动 R X 数量", 3, 2, 5));
            RMenu.Add("tibers", new CheckBox("自动移动熊"));

            if (flash != SpellSlot.Unknown)
            {
                RMenu.Add("rCountFlash", new Slider("自动闪现 + R 可击晕敌人数量", 4, 2, 5));
            }

            FarmMenu = Config.AddSubMenu("农兵");
            FarmMenu.Add("farmQ", new CheckBox("尾兵 Q"));
            FarmMenu.Add("farmW", new CheckBox("清线 W"));
            FarmMenu.Add("Mana", new Slider("清线蓝量", 60));

            Game.OnUpdate += Game_OnGameUpdate;
            GameObject.OnCreate += Obj_AI_Base_OnCreate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Obj_AI_Base_OnCreate(GameObject obj, EventArgs args)
        {
            if (obj.IsValid && obj.IsAlly && obj is Obj_AI_Minion && obj.Name.ToLower() == "tibbers")
            {
                Tibbers = (Obj_AI_Base) obj;
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.HasBuff("Recall"))
                return;

            HaveStun = Player.HasBuff("pyromania_particle");

            SetMana();

            if (R.IsReady() && (SebbyLib.Program.LagFree(1) || SebbyLib.Program.LagFree(3)) && !HaveTibers)
            {
                var realRange = R.Range;

                if (flash.IsReady())
                    realRange = FR.Range;

                foreach (
                    var enemy in
                        SebbyLib.Program.Enemies.Where(
                            enemy => enemy.IsValidTarget(realRange) && OktwCommon.ValidUlt(enemy)))
                {
                    if (enemy.IsValidTarget(R.Range))
                    {
                        var Rmode = getSliderItem(RMenu, "UM" + enemy.NetworkId);

                        if (Rmode == 2)
                            continue;

                        var poutput = R.GetPrediction(enemy, true);
                        var aoeCount = poutput.AoeTargetsHitCount;

                        if (Rmode == 1)
                            R.Cast(enemy);

                        if (Rmode == 3 && HaveStun)
                            R.Cast(enemy);

                        if (aoeCount >= getSliderItem(RMenu, "rCount") && getSliderItem(RMenu, "rCount") > 0)
                            R.Cast(enemy);
                        else if (SebbyLib.Program.Combo && HaveStun && getCheckBoxItem(RMenu, "autoRcombo"))
                            R.Cast(enemy);
                        else if (getCheckBoxItem(RMenu, "autoRks"))
                        {
                            var comboDmg = OktwCommon.GetKsDamage(enemy, R);

                            if (W.IsReady() && RMANA + WMANA < Player.Mana)
                                comboDmg += W.GetDamage(enemy);

                            if (Q.IsReady() && RMANA + WMANA + QMANA < Player.Mana)
                                comboDmg += Q.GetDamage(enemy);

                            if (enemy.Health < comboDmg)
                                R.Cast(poutput.CastPosition);
                        }
                    }
                    else if (HaveStun && flash.IsReady())
                    {
                        var poutputFlas = FR.GetPrediction(enemy, true);
                        var aoeCountFlash = poutputFlas.AoeTargetsHitCount;
                        if (HaveStun && aoeCountFlash >= getSliderItem(RMenu, "rCountFlash") &&
                            getSliderItem(RMenu, "rCountFlash") > 0)
                        {
                            Player.Spellbook.CastSpell(flash, poutputFlas.CastPosition);
                            R.Cast(poutputFlas.CastPosition);
                        }
                    }
                }
            }

            var t = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (t.IsValidTarget() && SebbyLib.Program.LagFree(2))
            {
                if (Q.IsReady() && getCheckBoxItem(QMenu, "autoQ"))
                {
                    if (SebbyLib.Program.Combo && RMANA + WMANA < Player.Mana)
                        Q.Cast(t);
                    else if (SebbyLib.Program.Farm && RMANA + WMANA + QMANA < Player.Mana &&
                             getCheckBoxItem(QMenu, "harrasQ"))
                        Q.Cast(t);
                    else
                    {
                        var qDmg = OktwCommon.GetKsDamage(t, Q);
                        var wDmg = W.GetDamage(t);
                        if (qDmg > t.Health)
                            Q.Cast(t);
                        else if (qDmg + wDmg > t.Health && Player.Mana > QMANA + WMANA)
                            Q.Cast(t);
                    }
                }
                if (W.IsReady() && SebbyLib.Program.LagFree(2) && getCheckBoxItem(WMenu, "autoW") &&
                    t.IsValidTarget(W.Range))
                {
                    var poutput = W.GetPrediction(t, true);

                    if (SebbyLib.Program.Combo && RMANA + WMANA < Player.Mana)
                        W.Cast(poutput.CastPosition);
                    else if (SebbyLib.Program.Farm && RMANA + WMANA + QMANA < Player.Mana &&
                             getCheckBoxItem(WMenu, "harrasW"))
                        W.Cast(poutput.CastPosition);
                    else
                    {
                        var wDmg = OktwCommon.GetKsDamage(t, W);
                        var qDmg = Q.GetDamage(t);
                        if (wDmg > t.Health)
                            W.Cast(poutput.CastPosition);
                        else if (qDmg + wDmg > t.Health && Player.Mana > QMANA + WMANA)
                            W.Cast(poutput.CastPosition);
                    }
                }
            }
            else if (Q.IsReady() || W.IsReady())
            {
                if (getCheckBoxItem(FarmMenu, "farmQ"))
                {
                    if (SebbyLib.Program.getCheckBoxItem("supportMode"))
                    {
                        if (SebbyLib.Program.LaneClear && Player.Mana > RMANA + QMANA)
                            farm();
                    }
                    else
                    {
                        if ((!HaveStun || SebbyLib.Program.LaneClear) && SebbyLib.Program.Farm)
                            farm();
                    }
                }
            }

            if (SebbyLib.Program.LagFree(3))
            {
                if (!HaveStun)
                {
                    if (E.IsReady() && !SebbyLib.Program.LaneClear && getCheckBoxItem(EMenu, "autoE") &&
                        Player.Mana > RMANA + EMANA + QMANA + WMANA)
                        E.Cast();
                    else if (W.IsReady() && Player.InFountain())
                        W.Cast(Player.Position);
                }
                if (R.IsReady())
                {
                    if (getCheckBoxItem(RMenu, "tibers") && HaveTibers && Tibbers != null && Tibbers.IsValid)
                    {
                        var enemy =
                            SebbyLib.Program.Enemies.Where(
                                x => x.IsValidTarget() && Tibbers.LSDistance(x.Position) < 1000 && !x.UnderTurret(true))
                                .OrderBy(x => x.LSDistance(Tibbers))
                                .FirstOrDefault();
                        if (enemy != null)
                        {
                            EloBuddy.Player.IssueOrder(Tibbers.LSDistance(enemy.Position) > 200 ? GameObjectOrder.MovePet : GameObjectOrder.AutoAttackPet, enemy);
                        }
                        else
                        {
                            var annieTarget = Orbwalker.LastTarget as Obj_AI_Base;
                            if (annieTarget != null)
                            {
                                EloBuddy.Player.IssueOrder(
                                    Tibbers.LSDistance(annieTarget.Position) > 200
                                        ? GameObjectOrder.MovePet
                                        : GameObjectOrder.AutoAttackPet, annieTarget);
                            }
                            else if (Tibbers.UnderTurret(true))
                            {
                                EloBuddy.Player.IssueOrder(GameObjectOrder.MovePet, Player);
                            }
                        }
                    }
                    else
                    {
                        Tibbers = null;
                    }
                }
            }
        }

        private static void farm()
        {
            if (SebbyLib.Program.LaneClear)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, Q.Range, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    if (W.IsReady())
                        W.Cast(mob);
                    else if (Q.IsReady())
                        Q.Cast(mob);
                }
            }

            var minionsList = Cache.GetMinions(Player.ServerPosition, Q.Range);
            if (Q.IsReady())
            {
                var minion =
                    minionsList.FirstOrDefault(x => HealthPrediction.LaneClearHealthPrediction(x, 250, 50) < Q.GetDamage(x) &&
                            x.Health > Player.GetAutoAttackDamage(x));
                Q.Cast(minion);
            }
            else if (SebbyLib.Program.LaneClear && W.IsReady() && Player.ManaPercent > getSliderItem(FarmMenu, "Mana") &&
                     getCheckBoxItem(FarmMenu, "farmW"))
            {
                var farmLocation = W.GetCircularFarmLocation(minionsList, W.Width);
                if (farmLocation.MinionsHit > 1)
                    W.Cast(farmLocation.Position);
            }
        }

        private static void SetMana()
        {
            if ((SebbyLib.Program.getCheckBoxItem("manaDisable") && SebbyLib.Program.Combo) || Player.HealthPercent < 20)
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

            if (!R.IsReady() || HaveTibers)
                RMANA = 0;
            else
                RMANA = R.Instance.SData.Mana;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(drawMenu, "qRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (Q.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Cyan, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Cyan, 1, 1);
            }
            if (getCheckBoxItem(drawMenu, "wRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (W.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, W.Range, Color.Orange, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, W.Range, Color.Orange, 1, 1);
            }

            if (getCheckBoxItem(drawMenu, "rRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (R.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, R.Range + R.Width/2,
                            Color.Gray, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, R.Range + R.Width/2, Color.Gray,
                        1, 1);
            }
        }
    }
}
