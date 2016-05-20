using System;
using LeagueSharp;
using LeagueSharp.Common;
using SebbyLib;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Evelynn
    {
        private static Menu Config = Program.Config;
        private static LeagueSharp.Common.Spell E, Q, R, W;
        private static float QMANA = 0, EMANA = 0, RMANA = 0;
        public static AIHeroClient Player { get { return ObjectManager.Player; } }

        public static Menu drawMenu, qMenu, wMenu, eMenu, rMenu, farmMenu;

        public static void LoadOKTW()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 500f);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 700f);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 250f);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 650f);

            R.SetSkillshot(0.25f, 300f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            drawMenu = Config.AddSubMenu("Drawings");
            drawMenu.Add("qRange", new CheckBox("Q range", false));
            drawMenu.Add("wRange", new CheckBox("W range", false));
            drawMenu.Add("eRange", new CheckBox("E range", false));
            drawMenu.Add("rRange", new CheckBox("R range", false));
            drawMenu.Add("onlyRdy", new CheckBox("Draw only ready spells", true));

            qMenu = Config.AddSubMenu("Q Config");
            qMenu.Add("autoQ", new CheckBox("Auto Q", true));

            wMenu = Config.AddSubMenu("W Config");
            wMenu.Add("autoW", new CheckBox("Auto W", true));
            wMenu.Add("slowW", new CheckBox("Auto W slow", true));

            eMenu = Config.AddSubMenu("E Config");
            eMenu.Add("autoE", new CheckBox("Auto E", true));

            rMenu = Config.AddSubMenu("R Config");
            rMenu.Add("rCount", new Slider("Auto R x enemies", 3, 0, 5));
            rMenu.Add("useR", new KeyBind("Semi-manual cast R key", false, KeyBind.BindTypes.HoldActive, 'T'));

            farmMenu = Config.AddSubMenu("Farm");
            farmMenu.Add("Mana", new Slider("Clear Mana", 30, 0, 100));
            farmMenu.Add("jungleQ", new CheckBox("Jungle Q", true));
            farmMenu.Add("jungleE", new CheckBox("Jungle E", true));
            farmMenu.Add("laneQ", new CheckBox("Lane clear Q", true));

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += GameOnOnUpdate;
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

        private static void GameOnOnUpdate(EventArgs args)
        {
            if (getKeyBindItem(rMenu, "useR"))
            {
                var t = TargetSelector.GetTarget(R.Range, DamageType.Physical);

                if (t.IsValidTarget())
                {
                    R.CastIfWillHit(t, 2, true);
                    R.Cast(t, true, true);
                }
            }
            if (Program.Combo)
            {
                if (Program.LagFree(1) && Q.IsReady() && getCheckBoxItem(qMenu, "autoQ"))
                    LogicQ();
                if (Program.LagFree(2) && E.IsReady() && getCheckBoxItem(eMenu, "autoE"))
                    LogicE();
                if (Program.LagFree(3) && W.IsReady())
                    LogicW();
                if (Program.LagFree(4) && R.IsReady())
                    LogicR();
            }
            else if (Program.LaneClear)
            {
                Jungle();
            }
        }

        private static void LogicQ()
        {
            if (Player.CountEnemiesInRange(Q.Range) > 0)
                Q.Cast();
        }

        private static void LogicE()
        {
            var t = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            if (t.IsValidTarget())
            {
                E.CastOnUnit(t);
            }
        }

        private static void LogicW()
        {
            if (getCheckBoxItem(wMenu, "autoW") && Player.Mana > RMANA + EMANA + QMANA && Player.CountEnemiesInRange(W.Range) > 0)
                W.Cast();
            else if (getCheckBoxItem(wMenu, "slowW") && Player.Mana > RMANA + EMANA + QMANA && Player.HasBuffOfType(BuffType.Slow))
                W.Cast();
        }

        private static void LogicR()
        {
            var t = TargetSelector.GetTarget(R.Range, DamageType.Physical);
            if (t.IsValidTarget())
            {
                var poutput = R.GetPrediction(t, true);

                var aoeCount = poutput.AoeTargetsHitCount;

                aoeCount = (aoeCount == 0) ? 1 : aoeCount;

                if (getSliderItem(rMenu, "rCount") > 0 && getSliderItem(rMenu, "rCount") <= aoeCount)
                    R.Cast(poutput.CastPosition);

                if (Player.HealthPercent < 60)
                {
                    double dmg = OktwCommon.GetIncomingDamage(Player, 1);
                    var enemys = Player.CountEnemiesInRange(700);
                    if (Player.Health - dmg < enemys * Player.Level * 20)
                        R.Cast(poutput.CastPosition);
                    else if (Player.Health - dmg < Player.Level * 10)
                        R.Cast(poutput.CastPosition);
                }
            }
        }

        private static void Jungle()
        {
            if (Player.ManaPercent < getSliderItem(farmMenu, "Mana"))
                return;
            var mobs = Cache.GetMinions(Player.ServerPosition, Q.Range, MinionTeam.Neutral);
            if (mobs.Count > 0)
            {
                var mob = mobs[0];
                if (getCheckBoxItem(farmMenu, "jungleE") && E.IsReady())
                    E.CastOnUnit(mob);
                if (getCheckBoxItem(farmMenu, "jungleQ") && Q.IsReady())
                    Q.Cast();
            }

            if (getCheckBoxItem(farmMenu, "laneQ") && Q.IsReady())
                Q.Cast();
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(drawMenu, "qRange"))
            {
                if (getCheckBoxItem(drawMenu,"onlyRdy"))
                {
                    if (Q.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
            }
            if (getCheckBoxItem(drawMenu,"wRange"))
            {
                if (getCheckBoxItem(drawMenu,"onlyRdy"))
                {
                    if (W.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Orange, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Orange, 1, 1);
            }
            if (getCheckBoxItem(drawMenu,"eRange"))
            {
                if (getCheckBoxItem(drawMenu,"onlyRdy"))
                {
                    if (E.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Yellow, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Yellow, 1, 1);
            }
            if (getCheckBoxItem(drawMenu,"rRange"))
            {
                if (getCheckBoxItem(drawMenu,"onlyRdy"))
                {
                    if (R.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.Gray, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.Gray, 1, 1);
            }
        }
    }

}