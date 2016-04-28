using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using Spell = LeagueSharp.Common.Spell;

namespace Illaoi___Tentacle_Kitty
{
    internal class Program
    {
        public static Spell Q, W, E, R;
        private static readonly AIHeroClient Illaoi = ObjectManager.Player;
        public static Menu Config, comboMenu, harassMenu, clearMenu, eMenu, ksMenu, drawMenu;

        public static string[] HighChamps =
        {
            "Ahri", "Anivia", "Annie", "Ashe", "Azir", "Brand", "Caitlyn", "Cassiopeia", "Corki", "Draven",
            "Ezreal", "Graves", "Jinx", "Kalista", "Karma", "Karthus", "Katarina", "Kennen", "KogMaw", "Leblanc",
            "Lucian", "Lux", "Malzahar", "MasterYi", "MissFortune", "Orianna", "Quinn", "Sivir", "Syndra", "Talon",
            "Teemo", "Tristana", "TwistedFate", "Twitch", "Varus", "Vayne", "Veigar", "VelKoz", "Viktor", "Xerath",
            "Zed", "Ziggs", "Kindred"
        };

        public static void Game_OnGameLoad()
        {
            if (Illaoi.ChampionName != "Illaoi")
            {
                return;
            }

            Q = new Spell(SpellSlot.Q, 850);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 900);
            R = new Spell(SpellSlot.R, 450);

            Q.SetSkillshot(.484f, 0, 500, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(.066f, 50, 1900, true, SkillshotType.SkillshotLine);

            Config = MainMenu.AddMenu("Illaoi - Tentacle Kitty", "Illaoi - Tentacle Kitty");

            comboMenu = Config.AddSubMenu("Combo Settings", "Combo Settings");
            comboMenu.Add("q.combo", new CheckBox("Use Q"));
            comboMenu.Add("q.ghost.combo", new CheckBox("Use Q (Ghost)"));
            comboMenu.Add("w.combo", new CheckBox("Use W"));
            comboMenu.Add("e.combo", new CheckBox("Use E"));
            comboMenu.Add("r.combo", new CheckBox("Use R"));
            comboMenu.Add("r.min.hit", new Slider("(R) Min. Hit", 3, 1, 5));

            harassMenu = Config.AddSubMenu("Harass Settings", "Harass Settings");
            harassMenu.Add("q.harass", new CheckBox("Use Q"));
            harassMenu.Add("q.ghost.harass", new CheckBox("Use Q (Ghost)"));
            harassMenu.Add("w.harass", new CheckBox("Use W"));
            harassMenu.Add("e.harass", new CheckBox("Use E"));
            harassMenu.Add("harass.mana", new Slider("Mana Manager", 20, 1, 99));

            clearMenu = Config.AddSubMenu("Clear Settings", "Clear Settings");
            clearMenu.Add("q.clear", new CheckBox("Use Q")); //
            clearMenu.Add("q.minion.hit", new Slider("(Q) Min. Hit", 3, 1, 6));
            clearMenu.Add("clear.mana", new Slider("Mana Manager", 20, 1, 99));

            eMenu = Config.AddSubMenu("E Settings", "E Settings");
            eMenu.AddGroupLabel("E Whitelist");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(o => o.IsEnemy))
                eMenu.Add("enemy." + enemy.CharData.BaseSkinName,
                    new CheckBox(string.Format("E: {0}", enemy.CharData.BaseSkinName),
                        HighChamps.Contains(enemy.CharData.BaseSkinName)));


            ksMenu = Config.AddSubMenu("KillSteal Settings", "KillSteal Settings");
            ksMenu.Add("q.ks", new CheckBox("Use Q"));

            drawMenu = Config.AddSubMenu("Draw Settings", "Draw Settings");
            drawMenu.Add("aa.indicator", new CheckBox("AA Indicator"));
            drawMenu.Add("q.draw", new CheckBox("Q Range"));
            drawMenu.Add("w.draw", new CheckBox("W Range"));
            drawMenu.Add("e.draw", new CheckBox("E Range"));
            drawMenu.Add("r.draw", new CheckBox("R Range"));
            drawMenu.Add("passive.draw", new CheckBox("Passive Draw"));


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

        private static void Drawing_OnDraw(EventArgs args)
        {
            var menuItem1 = getCheckBoxItem(drawMenu, "q.draw");
            var menuItem2 = getCheckBoxItem(drawMenu, "w.draw");
            var menuItem3 = getCheckBoxItem(drawMenu, "e.draw");
            var menuItem4 = getCheckBoxItem(drawMenu, "r.draw");
            var menuItem5 = getCheckBoxItem(drawMenu, "aa.indicator");
            if (menuItem1 && Q.IsReady())
            {
                Render.Circle.DrawCircle(new Vector3(Illaoi.Position.X, Illaoi.Position.Y, Illaoi.Position.Z), Q.Range,
                    Color.White);
            }
            if (menuItem2 && W.IsReady())
            {
                Render.Circle.DrawCircle(new Vector3(Illaoi.Position.X, Illaoi.Position.Y, Illaoi.Position.Z), W.Range,
                    Color.Gold);
            }
            if (menuItem3 && E.IsReady())
            {
                Render.Circle.DrawCircle(new Vector3(Illaoi.Position.X, Illaoi.Position.Y, Illaoi.Position.Z), E.Range,
                    Color.DodgerBlue);
            }
            if (menuItem4 && R.IsReady())
            {
                Render.Circle.DrawCircle(new Vector3(Illaoi.Position.X, Illaoi.Position.Y, Illaoi.Position.Z), R.Range,
                    Color.GreenYellow);
            }
            if (menuItem4)
            {
                foreach (
                    var enemy in
                        HeroManager.Enemies.Where(
                            x => x.IsValidTarget(1500) && x.IsValid && x.IsVisible && !x.IsDead && !x.IsZombie))
                {
                    Drawing.DrawText(enemy.HPBarPosition.X, enemy.HPBarPosition.Y, Color.Gold,
                        string.Format("{0} Basic Attack = Kill", AaIndicator(enemy)));
                }
            }
            if (menuItem5)
            {
                var enemy = HeroManager.Enemies.FirstOrDefault(x => x.IsValidTarget(2000));
                foreach (var passive in ObjectManager.Get<Obj_AI_Minion>().Where(x => x.Name == "God"))
                {
                    Render.Circle.DrawCircle(new Vector3(passive.Position.X, passive.Position.Y, passive.Position.Z),
                        850, Color.Gold, 2);
                    if (enemy != null)
                    {
                        var xx = Drawing.WorldToScreen(passive.Position.LSExtend(enemy.Position, 850));
                        var xy = Drawing.WorldToScreen(passive.Position);
                        Drawing.DrawLine(xy.X, xy.Y, xx.X, xx.Y, 5, Color.Gold);
                    }
                }
            }
        }

        private static int AaIndicator(AIHeroClient enemy)
        {
            var aCalculator = ObjectManager.Player.CalcDamage(enemy, DamageType.Physical,
                Illaoi.GetAutoAttackDamage(enemy));
            var killableAaCount = enemy.Health/aCalculator;
            var totalAa = (int) Math.Ceiling(killableAaCount);
            return totalAa;
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Clear();
            }
        }

        private static void Combo()
        {
            if (Q.IsReady() && getCheckBoxItem(comboMenu, "q.combo"))
            {
                var enemy = HeroManager.Enemies.FirstOrDefault(x => x.IsValidTarget(Q.Range));
                var enemyGhost = ObjectManager.Get<Obj_AI_Minion>().FirstOrDefault(x => x.Name == enemy.Name);
                if (enemy != null && enemyGhost == null)
                {
                    if (Q.CanCast(enemy) && Q.GetPrediction(enemy).Hitchance >= HitChance.High
                        && Q.GetPrediction(enemy).CollisionObjects.Count == 0)
                    {
                        Q.Cast(enemy);
                    }
                }
                if (enemy == null && enemyGhost != null && getCheckBoxItem(comboMenu, "q.ghost.combo"))
                {
                    if (Q.CanCast(enemyGhost) && Q.GetPrediction(enemyGhost).Hitchance >= HitChance.High
                        && Q.GetPrediction(enemyGhost).CollisionObjects.Count == 0)
                    {
                        Q.Cast(enemyGhost);
                    }
                }
            }

            if (W.IsReady() && getCheckBoxItem(comboMenu, "w.combo"))
            {
                var tentacle = ObjectManager.Get<Obj_AI_Minion>().First(x => x.Name == "God");
                if (tentacle != null)
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(850)))
                    {
                        W.Cast();
                    }
                }
            }
            if (E.IsReady() && getCheckBoxItem(comboMenu, "e.combo"))
            {
                foreach (
                    var enemy in HeroManager.Enemies.Where(o => o.IsValidTarget(E.Range) && !o.IsDead && !o.IsZombie))
                {
                    if (getCheckBoxItem(eMenu, "enemy." + enemy.CharData.BaseSkinName) &&
                        E.GetPrediction(enemy).Hitchance >= HitChance.High
                        && E.GetPrediction(enemy).CollisionObjects.Count == 0)
                    {
                        E.Cast(enemy);
                    }
                }
            }
            if (R.IsReady() && getCheckBoxItem(comboMenu, "r.combo"))
            {
                foreach (
                    var enemy in HeroManager.Enemies.Where(o => o.IsValidTarget(R.Range) && !o.IsDead && !o.IsZombie))
                {
                    if (Illaoi.CountEnemiesInRange(R.Range) >= getSliderItem(comboMenu, "r.min.hit"))
                    {
                        R.Cast();
                    }
                }
            }
        }

        private static void Harass()
        {
            if (Illaoi.ManaPercent < getSliderItem(harassMenu, "harass.mana"))
            {
                return;
            }
            if (Q.IsReady() && getCheckBoxItem(harassMenu, "q.harass"))
            {
                var enemy = HeroManager.Enemies.FirstOrDefault(x => x.IsValidTarget(Q.Range));
                var enemyGhost = ObjectManager.Get<Obj_AI_Minion>().FirstOrDefault(x => x.Name == enemy.Name);
                if (enemy != null && enemyGhost == null)
                {
                    if (Q.CanCast(enemy) && Q.GetPrediction(enemy).Hitchance >= HitChance.High
                        && Q.GetPrediction(enemy).CollisionObjects.Count == 0)
                    {
                        Q.Cast(enemy);
                    }
                }
                if (enemy == null && enemyGhost != null && getCheckBoxItem(harassMenu, "q.ghost.harass"))
                {
                    if (Q.CanCast(enemyGhost) && Q.GetPrediction(enemyGhost).Hitchance >= HitChance.High
                        && Q.GetPrediction(enemyGhost).CollisionObjects.Count == 0)
                    {
                        Q.Cast(enemyGhost);
                    }
                }
            }
            if (W.IsReady() && getCheckBoxItem(harassMenu, "w.harass"))
            {
                var tentacle = ObjectManager.Get<Obj_AI_Minion>().First(x => x.Name == "God");
                if (tentacle != null)
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(850)))
                    {
                        W.Cast();
                    }
                }
            }
            if (E.IsReady() && getCheckBoxItem(harassMenu, "e.harass"))
            {
                foreach (
                    var enemy in HeroManager.Enemies.Where(o => o.IsValidTarget(E.Range) && !o.IsDead && !o.IsZombie))
                {
                    if (getCheckBoxItem(eMenu, "enemy." + enemy.CharData.BaseSkinName) &&
                        E.GetPrediction(enemy).Hitchance >= HitChance.High
                        && E.GetPrediction(enemy).CollisionObjects.Count == 0)
                    {
                        E.Cast(enemy);
                    }
                }
            }
        }

        private static void Clear()
        {
            if (Illaoi.ManaPercent < getSliderItem(clearMenu, "clear.mana"))
            {
                return;
            }

            var minionCount = MinionManager.GetMinions(Illaoi.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
            if (Q.IsReady() && getCheckBoxItem(clearMenu, "q.clear"))
            {
                var mfarm = Q.GetLineFarmLocation(minionCount);
                if (minionCount.Count >= getSliderItem(clearMenu, "q.minion.hit"))
                {
                    Q.Cast(mfarm.Position);
                }
            }
        }
    }
}