using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SebbyLib;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Kindred
    {
        private static Menu Config = Program.Config;
        private static LeagueSharp.Common.Spell E, Q, R, W;
        private static float QMANA = 0, WMANA = 0, EMANA = 0, RMANA = 0;

        public static AIHeroClient Player { get { return ObjectManager.Player; } }
        public static Core.OKTWdash Dash;

        public static Menu drawMenu, qMenu, wMenu, eMenu, rMenu, harassMenu, farmMenu;

        public static void LoadOKTW()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 340);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 800);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 600);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 550);

            drawMenu = Config.AddSubMenu("Drawings");
            drawMenu.Add("qRange", new CheckBox("Q range", false));
            drawMenu.Add("wRange", new CheckBox("W range", false));
            drawMenu.Add("eRange", new CheckBox("E range", false));
            drawMenu.Add("rRange", new CheckBox("R range", false));
            drawMenu.Add("onlyRdy", new CheckBox("Draw only ready spells", true));

            qMenu = Config.AddSubMenu("Q Config");
            qMenu.Add("autoQ", new CheckBox("Auto Q", true));
            Dash = new Core.OKTWdash(Q);

            wMenu = Config.AddSubMenu("W Config");
            wMenu.Add("autoW", new CheckBox("Auto W", true));
            wMenu.Add("harrasW", new CheckBox("Harass W", true));

            eMenu = Config.AddSubMenu("E Config");
            eMenu.Add("autoE", new CheckBox("Auto E", true));
            eMenu.Add("harrasE", new CheckBox("Harass E", true));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                eMenu.Add("Euse" + enemy.ChampionName, new CheckBox("Use On : " + enemy.ChampionName, true));

            rMenu = Config.AddSubMenu("R Config");
            rMenu.Add("autoR", new CheckBox("Auto R", true));
            rMenu.Add("Renemy", new Slider("Don't R if x enemies", 4, 0, 5));

            harassMenu = Config.AddSubMenu("Harass");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != Player.Team))
                harassMenu.Add("haras" + enemy.ChampionName, new CheckBox(enemy.ChampionName));

            farmMenu = Config.AddSubMenu("Farm");
            farmMenu.Add("farmQ", new CheckBox("Lane clear Q", true));
            farmMenu.Add("farmW", new CheckBox("Lane clear W", true));
            farmMenu.Add("farmE", new CheckBox("Lane clear E", true));
            farmMenu.Add("Mana", new Slider("LaneClear Mana", 50, 0, 100));
            farmMenu.Add("LCminions", new Slider("LaneClear minimum minions", 3, 0, 10));
            farmMenu.Add("jungleQ", new CheckBox("Jungle clear Q", true));
            farmMenu.Add("jungleW", new CheckBox("Jungle clear W", true));
            farmMenu.Add("jungleE", new CheckBox("Jungle clear E", true));

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Orbwalker.OnPostAttack += Orbwalker_AfterAttack;
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


        public static void Orbwalker_AfterAttack(AttackableUnit target, EventArgs args)
        {
            if (target.Type != GameObjectType.AIHeroClient)
                return;

            if (Program.Combo && Player.Mana > RMANA + QMANA && Q.IsReady() && getCheckBoxItem(qMenu, "autoQ"))
            {
                var t = target as AIHeroClient;
                if (t.IsValidTarget())
                {
                    var dashPos = Dash.CastDash();
                    if (!dashPos.IsZero && dashPos.CountEnemiesInRange(500) > 0)
                    {
                        Q.Cast(dashPos);
                    }
                }
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Program.LagFree(0))
            {
                SetMana();
                Jungle();
            }

            if (Program.LagFree(1) && E.IsReady() && getCheckBoxItem(eMenu, "autoE"))
                LogicE();

            if (Program.LagFree(2) && W.IsReady() && getCheckBoxItem(wMenu, "autoW"))
                LogicW();

            if (Program.LagFree(3) && Q.IsReady() && getCheckBoxItem(qMenu, "autoQ"))
                LogicQ();

            if (R.IsReady() && getCheckBoxItem(rMenu, "autoR"))
                LogicR();
        }

        private static void LogicQ()
        {
            if (Program.Combo && Player.Mana > RMANA + QMANA)
            {
                if (Orbwalker.LastTarget != null)
                    return;
                var dashPos = Dash.CastDash();
                if (!dashPos.IsZero && dashPos.CountEnemiesInRange(500) > 0)
                {
                    Q.Cast(dashPos);
                }
            }
            if (Program.LaneClear && Player.ManaPercent > getSliderItem(farmMenu, "Mana") && getCheckBoxItem(farmMenu, "farmQ") && Player.Mana > RMANA + QMANA)
            {
                var allMinionsQ = Cache.GetMinions(Player.ServerPosition, 400);
                if (allMinionsQ.Count >= getSliderItem(farmMenu, "LCminions"))
                    Q.Cast(Game.CursorPos);
            }
        }

        private static void LogicW()
        {
            var t = TargetSelector.GetTarget(650, DamageType.Physical);
            if (t.IsValidTarget() && !Q.IsReady())
            {
                if (Program.Combo && Player.Mana > RMANA + WMANA)
                    W.Cast();
                else if (Program.Farm && getCheckBoxItem(wMenu, "harrasW") && Player.Mana > RMANA + EMANA + WMANA + EMANA && getCheckBoxItem(harassMenu, "haras" + t.ChampionName))
                    W.Cast();
            }
            var tks = TargetSelector.GetTarget(1600, DamageType.Physical);
            if (tks.IsValidTarget())
            {
                if (W.GetDamage(tks) * 3 > tks.Health - OktwCommon.GetIncomingDamage(tks))
                    W.Cast();
            }

            if (Program.LaneClear && Player.ManaPercent > getSliderItem(farmMenu, "Mana") && getCheckBoxItem(farmMenu, "farmW") && Player.Mana > RMANA + WMANA)
            {
                var allMinionsQ = Cache.GetMinions(Player.ServerPosition, 600);
                if (allMinionsQ.Count >= getSliderItem(farmMenu, "LCminions"))
                    W.Cast();
            }
        }

        private static void LogicE()
        {
            var torb = Orbwalker.LastTarget;
            if (torb == null || torb.Type != GameObjectType.AIHeroClient)
                return;
            else
            {
                var t = torb as AIHeroClient;

                if (t.IsValidTarget(E.Range))
                {
                    if (!getCheckBoxItem(eMenu, "Euse" + t.ChampionName))
                        return;
                    if (Program.Combo && Player.Mana > RMANA + EMANA)
                        E.CastOnUnit(t);
                    else if (Program.Farm && getCheckBoxItem(eMenu, "harrasE") && Player.Mana > RMANA + EMANA + WMANA + EMANA && getCheckBoxItem(harassMenu, "haras" + t.ChampionName))
                        E.CastOnUnit(t);
                }
            }
        }

        private static void LogicR()
        {
            var rEnemy = getSliderItem(rMenu, "Renemy");

            double dmg = OktwCommon.GetIncomingDamage(Player, 1);
            var enemys = Player.CountEnemiesInRange(900);

            if (dmg == 0 && enemys == 0)
                return;

            if (Player.CountEnemiesInRange(500) < rEnemy)
            {
                if (Player.Health - dmg < enemys * Player.Level * 15)
                    R.Cast(Player);
                else if (Player.Health - dmg < Player.Level * 15)
                    R.Cast(Player);
            }
        }

        private static void Jungle()
        {
            if (Program.LaneClear)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, 600, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];

                    if (E.IsReady() && getCheckBoxItem(farmMenu, "jungleE"))
                    {
                        E.Cast(mob);
                        return;
                    }
                    if (Q.IsReady() && getCheckBoxItem(farmMenu, "jungleQ"))
                    {
                        Q.Cast(Game.CursorPos);
                        return;
                    }
                    if (W.IsReady() && getCheckBoxItem(farmMenu, "jungleW"))
                    {
                        W.Cast();
                        return;
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
                RMANA = QMANA - Player.PARRegenRate * Q.Instance.Cooldown;
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