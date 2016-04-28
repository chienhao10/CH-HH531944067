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

namespace Kindred___YinYang
{
    internal class Program
    {
        public static Spell Q;
        public static Spell E;
        public static Spell W;
        public static Spell R;
        private static readonly AIHeroClient Kindred = ObjectManager.Player;
        public static Menu Config;
        public static Vector3 OrderSpawnPosition = new Vector3(394, 461, 171);
        public static Vector3 ChaosSpawnPosition = new Vector3(14340, 14391, 179);

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
            Q = new Spell(SpellSlot.Q, 340);
            W = new Spell(SpellSlot.W, 800);
            E = new Spell(SpellSlot.E, 500);
            R = new Spell(SpellSlot.R, 550);

            Config = MainMenu.AddMenu("Kindred - Yin Yang", "Kindred - Yin Yang");
            Language.MenuInit();

            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Game.OnUpdate += Game_OnGameUpdate;
            GameObject.OnCreate += GameObject_OnCreate;
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

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs spell)
        {
            Helper.Protector(sender, spell);
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            Helper.AntiRengarOnCreate(sender, args);
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            Helper.AntiGapcloser(gapcloser);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Harass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                Clear();
                Jungle();
            }

            if (getCheckBoxItem(Language.miscMenu, "use.r") && R.IsReady())
            {
                Helper.ClassicUltimate();
            }

            if (getCheckBoxItem(Language.ksMenu, "q.ks") && Q.IsReady())
            {
                KillSteal(getSliderItem(Language.ksMenu, "q.ks.count"));
            }

            if (getCheckBoxItem(Language.miscMenu, "spell.broker") && R.IsReady())
            {
                Helper.SpellBreaker();
            }
        }

        private static void Combo()
        {
            var useQ = getCheckBoxItem(Language.comboMenu, "q.combo");
            var useW = getCheckBoxItem(Language.comboMenu, "w.combo");
            var useE = getCheckBoxItem(Language.comboMenu, "e.combo");
            if (useQ && Q.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(ObjectManager.Player.AttackRange)))
                {
                    Helper.AdvancedQ(Q, enemy, 3);
                }
            }
            if (useW && W.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(o => o.IsValidTarget(W.Range) && !o.IsDead && !o.IsZombie))
                {
                    W.Cast();
                }
            }
            if (useE && E.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(o => o.IsValidTarget(E.Range) && !o.IsDead && !o.IsZombie))
                {
                    if (getCheckBoxItem(Language.eMenu, "enemy." + enemy.CharData.BaseSkinName))
                    {
                        E.Cast(enemy);
                    }
                }
            }
        }

        private static void Harass()
        {
            var useQ = getCheckBoxItem(Language.harassMenu, "q.harass");
            var useW = getCheckBoxItem(Language.harassMenu, "w.harass");
            var useE = getCheckBoxItem(Language.harassMenu, "e.harass");
            var harassMana = getSliderItem(Language.harassMenu, "harass.mana");

            if (Kindred.ManaPercent > harassMana)
            {
                if (useQ && Q.IsReady())
                {
                    foreach (
                        var enemy in
                            HeroManager.Enemies.Where(
                                o => o.IsValidTarget(ObjectManager.Player.AttackRange) && !o.IsDead && !o.IsZombie))
                    {
                        Q.Cast(Game.CursorPos);
                    }
                }
                if (useW && W.IsReady())
                {
                    foreach (
                        var enemy in
                            HeroManager.Enemies.Where(o => o.IsValidTarget(W.Range) && !o.IsDead && !o.IsZombie))
                    {
                        W.Cast();
                    }
                }
                if (useE && E.IsReady())
                {
                    foreach (
                        var enemy in
                            HeroManager.Enemies.Where(o => o.IsValidTarget(E.Range) && !o.IsDead && !o.IsZombie))
                    {
                        if (getCheckBoxItem(Language.eMenu, "enemy." + enemy.CharData.BaseSkinName))
                        {
                            E.Cast(enemy);
                        }
                    }
                }
            }
        }

        private static void Clear()
        {
            var xMinion = MinionManager.GetMinions(Kindred.ServerPosition, Kindred.AttackRange);
            var useQ = getCheckBoxItem(Language.laneClearMenu, "q.clear");
            var manaClear = getSliderItem(Language.laneClearMenu, "clear.mana");
            var minCount = getSliderItem(Language.laneClearMenu, "q.minion.count");
            if (Kindred.ManaPercent >= manaClear)
            {
                if (useQ && Q.IsReady() && xMinion.Count >= minCount)
                {
                    Q.Cast(Game.CursorPos);
                }
            }
        }

        private static void KillSteal(int aacount)
        {
            foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range)))
            {
                if (enemy.Health <
                    ObjectManager.Player.CalcDamage(enemy, DamageType.Physical, Kindred.TotalAttackDamage)*aacount)
                {
                    Q.Cast(Game.CursorPos);
                }
            }
        }

        private static void Jungle()
        {
            var useQ = getCheckBoxItem(Language.jungleClearMenu, "q.jungle");
            var useW = getCheckBoxItem(Language.jungleClearMenu, "w.jungle");
            var useE = getCheckBoxItem(Language.jungleClearMenu, "e.jungle");
            var manaSlider = getSliderItem(Language.jungleClearMenu, "jungle.mana");
            var mob = MinionManager.GetMinions(Kindred.ServerPosition, Orbwalking.GetRealAutoAttackRange(Kindred) + 100,
                MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (mob == null || mob.Count == 0)
            {
                return;
            }

            if (Kindred.ManaPercent > manaSlider)
            {
                if (Q.IsReady() && useQ)
                {
                    Q.Cast(Game.CursorPos);
                }
                if (W.IsReady() && useW)
                {
                    W.Cast();
                }
                if (E.IsReady() && useE)
                {
                    E.Cast(mob[0]);
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            /*
            drawMenu.Add("aa.indicator", "AA Indicator").SetValue(new Circle(true, System.Drawing.Color.Gold)));
            
             */

            var menuItem1 = getCheckBoxItem(Language.drawMenu, "q.drawx");
            var menuItem2 = getCheckBoxItem(Language.drawMenu, "w.draw");
            var menuItem3 = getCheckBoxItem(Language.drawMenu, "e.draw");
            var menuItem4 = getCheckBoxItem(Language.drawMenu, "r.draw");

            if (menuItem1 && Q.IsReady())
            {
                Render.Circle.DrawCircle(new Vector3(Kindred.Position.X, Kindred.Position.Y, Kindred.Position.Z),
                    Q.Range, Color.White);
            }
            if (menuItem2 && W.IsReady())
            {
                Render.Circle.DrawCircle(new Vector3(Kindred.Position.X, Kindred.Position.Y, Kindred.Position.Z),
                    W.Range, Color.Gold);
            }
            if (menuItem3 && E.IsReady())
            {
                Render.Circle.DrawCircle(new Vector3(Kindred.Position.X, Kindred.Position.Y, Kindred.Position.Z),
                    E.Range, Color.DodgerBlue);
            }
            if (menuItem4 && R.IsReady())
            {
                Render.Circle.DrawCircle(new Vector3(Kindred.Position.X, Kindred.Position.Y, Kindred.Position.Z),
                    R.Range, Color.GreenYellow);
            }
            if (menuItem4)
            {
                foreach (
                    var enemy in
                        HeroManager.Enemies.Where(
                            x => x.IsValidTarget(1500) && x.IsValid && x.IsVisible && !x.IsDead && !x.IsZombie))
                {
                    Drawing.DrawText(enemy.HPBarPosition.X, enemy.HPBarPosition.Y, Color.Gold,
                        string.Format("{0} Basic Attack = Kill", Helper.AaIndicator(enemy)));
                }
            }
        }
    }
}