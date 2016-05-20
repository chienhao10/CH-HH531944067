using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SPrediction;
using SharpDX;
using System.Collections.Generic;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;

namespace Nechrito_Diana
{
    class Program
    {
        /// <summary>
        /// To Do
        /// Auto Zhonyas
        /// Suggestions?
        /// </summary>
        public static readonly AIHeroClient Player = ObjectManager.Player;
        private static readonly HpBarIndicator Indicator = new HpBarIndicator();

        public static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Diana") return;
            MenuConfig.LoadMenu();
            Spells.Initialise();
            Spells.Ignite = Player.GetSpellSlot("summonerdot");
            Game.OnUpdate += OnTick;
            Interrupter2.OnInterruptableTarget += interrupt;
            AntiGapcloser.OnEnemyGapcloser += gapcloser;
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += Drawing_OnEndScene;
        }
        private static void OnTick(EventArgs args)
        {
            Modes.Flee();
            Logic.SmiteCombo();
            Logic.SmiteJungle();
            Killsteal();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Modes.ComboLogic();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Modes.LaneLogic();
                Modes.JungleLogic();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Modes.HarassLogic();
            }
        }
        public static void Killsteal()
        {
            if (Spells._q.IsReady() && MenuConfig.ksQ)
            {
                var targets = HeroManager.Enemies.Where(x => x.IsValidTarget(Spells._q.Range) && !x.IsZombie);
                foreach (var target in targets)
                {
                    if (target.Health < Spells._r.GetDamage(target) && !target.IsInvulnerable && (Player.LSDistance(target.Position) <= Spells._q.Range))
                    {
                        Spells._q.Cast(target);
                    }
                }
            }
            if (Spells._r.IsReady() && MenuConfig.ksR)
            {
                var targets = HeroManager.Enemies.Where(x => x.IsValidTarget(Spells._r.Range) && !x.IsZombie);
                foreach (var target in targets)
                {
                    if (target.Health < Spells._r.GetDamage(target) && !target.IsInvulnerable && (Player.LSDistance(target.Position) <= Spells._q.Range))
                    {
                        Spells._r.Cast(target);
                    }
                }
            }
            if (Spells._r.IsReady() && Spells._q.IsReady() && MenuConfig.ksR && MenuConfig.ksQ)
            {
                var targets = HeroManager.Enemies.Where(x => x.IsValidTarget(Spells._r.Range) && !x.IsZombie);
                foreach (var target in targets)
                {
                    if (target.Health < Spells._r.GetDamage(target) + Spells._q.GetDamage(target) && !target.IsInvulnerable && (Player.LSDistance(target.Position) <= Spells._q.Range))
                    {
                        Spells._q.Cast(target);
                        Spells._r.Cast(target);
                    }
                }
            }
            if (Spells.Ignite.IsReady() && MenuConfig.ignite)
            {
                var target = TargetSelector.GetTarget(600f, DamageType.True);
                if (target.IsValidTarget(600f) && Dmg.IgniteDamage(target) >= target.Health)
                {
                    Player.Spellbook.CastSpell(Spells.Ignite, target);
                }
            }
            if (Logic.Smite.IsReady() && MenuConfig.ksSmite)
            {
                var target = TargetSelector.GetTarget(600f, DamageType.True);
                if (target.IsValidTarget(600f) && Dmg.SmiteDamage(target) >= target.Health)
                {
                    Player.Spellbook.CastSpell(Logic.Smite, target);
                }
            }
        }
        public static readonly List<Vector3> JunglePos = new List<Vector3>()
        {
          new Vector3(6271.479f, 12181.25f, 56.47668f),
           new Vector3(6971.269f, 10839.12f, 55.2f),
           new Vector3(8006.336f, 9517.511f, 52.31763f),
           new Vector3(10995.34f, 8408.401f, 61.61731f),
          new Vector3(10895.08f, 7045.215f, 51.72278f),
           new Vector3(12665.45f, 6466.962f, 51.70544f),
           //pos of baron
           new Vector3(5048f, 10460f, -71.2406f),
           new Vector3(39000.529f, 7901.832f, 51.84973f),
          new Vector3(2106.111f, 8388.643f, 51.77686f),
           new Vector3(3753.737f, 6454.71f, 52.46301f),
           new Vector3(6776.247f, 5542.872f, 55.27625f),
           new Vector3(7811.688f, 4152.602f, 53.79456f),
          new Vector3(8528.921f, 2822.875f, 50.92188f),
          //pos of dragon
           new Vector3(9802f, 4366f, -71.2406f),
           new Vector3(3926f, 7918f, 51.74162f)
        };

        // Draws position where maxrange jumps are possible (drag, baron, wolves)
        public static readonly Dictionary<String, Vector3> JumpPos = new Dictionary<String, Vector3>()
        {
            { "mid_Dragon" , new Vector3 (9122f, 4058f, 53.95995f) },
            { "left_dragon" , new Vector3 (9088f, 4544f, 52.24316f) },
            { "baron" , new Vector3 (5774f, 10706f, 55.77578F) }, //not pre 20
            { "red_wolves" , new Vector3 (11772f, 8856f, 50.30728f) },
            { "blue_wolves" , new Vector3 (3046f, 6132f, 57.04655f) },
        };

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead)
                return;

            if (MenuConfig.EscapeSpot)
            {
                foreach (var pos in JunglePos)
                    if (pos.LSDistance(Player.Position) < 1200)
                    {
                        Render.Circle.DrawCircle(pos, 85, Spells._r.IsReady() ? System.Drawing.Color.GreenYellow : System.Drawing.Color.Gray);
                    }
                foreach (var pos in JumpPos)
                {
                    if (pos.Value.LSDistance(Player.Position) < 12000)
                    {
                        Render.Circle.DrawCircle(pos.Value, 40, System.Drawing.Color.White);
                    }
                }
                if (MenuConfig.EngageDraw)
                {
                    Render.Circle.DrawCircle(Player.Position, 800,
                        Spells._q.IsReady() ? System.Drawing.Color.FromArgb(120, 0, 170, 255) : System.Drawing.Color.IndianRed);
                }
            }
        }

        private static void interrupt(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsEnemy && Spells._e.IsReady() && sender.IsValidTarget() && !sender.IsZombie && MenuConfig.Interrupt)
            {
                if (sender.IsValidTarget(Spells._e.Range + sender.BoundingRadius)) Spells._e.Cast();
            }
        }
        private static void gapcloser(ActiveGapcloser gapcloser)
        {
            var target = gapcloser.Sender;
            if (target.IsEnemy && Spells._e.IsReady() && target.IsValidTarget() && !target.IsZombie && MenuConfig.Gapcloser)
            {
                if (target.IsValidTarget(Spells._e.Range + Player.BoundingRadius + target.BoundingRadius)) Spells._e.Cast();
            }
        }
        private static void Drawing_OnEndScene(EventArgs args)
        {
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(ene => ene.IsValidTarget() && !ene.IsZombie))
            {
                var EasyKill = Spells._r.IsReady() && Spells._r.IsReady() && Dmg.IsLethal(enemy)
                       ? new ColorBGRA(0, 255, 0, 120)
                       : new ColorBGRA(255, 255, 0, 120);
                Indicator.unit = enemy;
                Indicator.drawDmg(Dmg.ComboDmg(enemy), EasyKill);
            }
        }
    }
}