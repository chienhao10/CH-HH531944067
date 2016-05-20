using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Kayle
    {
        private static Menu Config = Program.Config;
        private static LeagueSharp.Common.Spell E, Q, R, W;
        private static float QMANA = 0, WMANA = 0, EMANA = 0, RMANA = 0;
        public static AIHeroClient Player { get { return ObjectManager.Player; } }

        public static Menu drawMenu, qMenu, wMenu, eMenu, rMenu, farmMenu, harassMenu;

        public static void LoadOKTW()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 670);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 900);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 660);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 900);

            drawMenu = Config.AddSubMenu("Drawings");
            drawMenu.Add("noti", new CheckBox("Show notification & line", true));
            drawMenu.Add("onlyRdy", new CheckBox("Draw only ready spells", true));
            drawMenu.Add("qRange", new CheckBox("Q range", false));
            drawMenu.Add("wRange", new CheckBox("W range", false));
            drawMenu.Add("eRange", new CheckBox("E range", false));
            drawMenu.Add("rRange", new CheckBox("R range", false));

            qMenu = Config.AddSubMenu("Q Config");
            qMenu.Add("autoQ", new CheckBox("Auto Q", true));

            wMenu = Config.AddSubMenu("W Config");
            wMenu.Add("autoW", new CheckBox("Auto W", true));
            wMenu.Add("autoWspeed", new CheckBox("W speed-up", true));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsAlly))
                wMenu.Add("Wally" + enemy.ChampionName, new CheckBox(enemy.ChampionName));

            eMenu = Config.AddSubMenu("E Config");
            eMenu.Add("autoE", new CheckBox("Auto E", true));
            eMenu.Add("harrasE", new CheckBox("Harras E", true));

            rMenu = Config.AddSubMenu("R Config");
            rMenu.Add("autoR", new CheckBox("Auto R", true));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsAlly))
                rMenu.Add("Rally" + enemy.ChampionName, new CheckBox(enemy.ChampionName));

            harassMenu = Config.AddSubMenu("Harass");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                harassMenu.Add("harras" + enemy.ChampionName, new CheckBox(enemy.ChampionName));

            farmMenu = Config.AddSubMenu("Farm");
            farmMenu.Add("farmE", new CheckBox("Lane clear E", true));
            farmMenu.Add("jungleQ", new CheckBox("Jungle clear Q", true));
            farmMenu.Add("jungleE", new CheckBox("Jungle clear E", true));

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
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

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Program.LagFree(1))
            {
                SetMana();
                Jungle();
            }

            if (R.IsReady() && getCheckBoxItem(rMenu, "autoR"))
                LogicR();

            if (Program.LagFree(2) && W.IsReady() && !Player.Spellbook.IsAutoAttacking && getCheckBoxItem(wMenu, "autoW"))
                LogicW();

            if (Program.LagFree(3) && E.IsReady() && getCheckBoxItem(eMenu, "autoE"))
                LogicE();
            if (Program.LagFree(4) && Q.IsReady() && !Player.Spellbook.IsAutoAttacking && getCheckBoxItem(qMenu, "autoQ"))
                LogicQ();
        }

        private static void LogicR()
        {
            foreach (var ally in Program.Allies.Where(ally => ally.IsValid && !ally.IsDead && ally.HealthPercent < 70 && Player.ServerPosition.Distance(ally.ServerPosition) < R.Range && getCheckBoxItem(rMenu, "Rally" + ally.ChampionName)))
            {
                double dmg = OktwCommon.GetIncomingDamage(ally, 1);
                var enemys = ally.CountEnemiesInRange(800);

                if (dmg == 0 && enemys == 0)
                    continue;

                enemys = (enemys == 0) ? 1 : enemys;

                if (ally.Health - dmg < enemys * ally.Level * 20)
                    R.CastOnUnit(ally);
            }
        }

        private static void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (t.IsValidTarget())
            {
                if (Program.Combo)
                    Q.Cast(t);
                else if (Program.Farm && getCheckBoxItem(harassMenu, "harras" + t.ChampionName) && Player.Mana > RMANA + WMANA + QMANA + QMANA)
                    Q.Cast(t);
                else if (Player.Health < Player.Level * 40 && !W.IsReady() && !R.IsReady())
                    Q.Cast(t);
                else if (OktwCommon.GetKsDamage(t, Q) > t.Health)
                    Q.Cast(t);
            }
        }

        private static void LogicW()
        {
            if (!Player.InFountain() && !Player.HasBuff("Recall") && !Player.IsRecalling())
            {
                AIHeroClient lowest = Player;

                foreach (var ally in Program.Allies.Where(ally => ally.IsValid && !ally.IsDead && getCheckBoxItem(wMenu, "Wally" + ally.ChampionName) && Player.Distance(ally.Position) < W.Range))
                {
                    if (ally.Health < lowest.Health)
                        lowest = ally;
                }

                if (Player.Mana > WMANA + QMANA && lowest.Health < lowest.Level * 40)
                    W.CastOnUnit(lowest);
                else if (Player.Mana > WMANA + EMANA + QMANA && lowest.Health < lowest.MaxHealth * 0.4 && lowest.Health < 1500)
                    W.CastOnUnit(lowest);
                else if (Player.Mana > Player.MaxMana * 0.5 && lowest.Health < lowest.MaxHealth * 0.7 && lowest.Health < 2000)
                    W.CastOnUnit(lowest);
                else if (Player.Mana > Player.MaxMana * 0.9 && lowest.Health < lowest.MaxHealth * 0.9)
                    W.CastOnUnit(lowest);
                else if (Player.Mana == Player.MaxMana && lowest.Health < lowest.MaxHealth * 0.9)
                    W.CastOnUnit(lowest);
                if (getCheckBoxItem(wMenu, "autoWspeed"))
                {
                    var t = TargetSelector.GetTarget(1000, DamageType.Magical);
                    if (t.IsValidTarget())
                    {
                        if (Program.Combo && Player.Mana > WMANA + QMANA + EMANA && Player.Distance(t.Position) > Q.Range)
                            W.CastOnUnit(Player);
                    }
                }
            }
        }

        private static void LogicE()
        {
            if (Program.Combo && Player.Mana > WMANA + EMANA && Player.CountEnemiesInRange(700) > 0)
                E.Cast();
            else if (Program.Farm && getCheckBoxItem(eMenu, "harrasE") && Player.Mana > WMANA + EMANA + QMANA && Player.CountEnemiesInRange(500) > 0)
                E.Cast();
            else if (Program.LaneClear && getCheckBoxItem(eMenu, "farmE") && Player.Mana > WMANA + EMANA + QMANA && FarmE())
                E.Cast();
        }

        private static void Jungle()
        {
            if (Program.LaneClear && Player.Mana > RMANA + WMANA + RMANA + WMANA)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, 600, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    if (E.IsReady() && getCheckBoxItem(farmMenu, "jungleE"))
                    {
                        E.Cast();
                        return;
                    }
                    if (Q.IsReady() && getCheckBoxItem(farmMenu, "jungleQ"))
                    {
                        Q.Cast(mob);
                        return;
                    }
                }
            }
        }

        private static bool FarmE()
        {
            return (Cache.GetMinions(Player.ServerPosition, 600).Count > 0);
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
            RMANA = 0;

            if (!Q.IsReady())
                QMANA = QMANA - Player.PARRegenRate * Q.Instance.Cooldown;

        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(drawMenu, "qRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (Q.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
            }
            if (getCheckBoxItem(drawMenu, "wRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (W.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Orange, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Orange, 1, 1);
            }
            if (getCheckBoxItem(drawMenu, "eRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (E.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Yellow, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Yellow, 1, 1);
            }
            if (getCheckBoxItem(drawMenu, "rRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
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