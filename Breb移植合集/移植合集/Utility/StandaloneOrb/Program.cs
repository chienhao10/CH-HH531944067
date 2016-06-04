
using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;

namespace PuppyStandaloneOrbwalker
{
    internal class Program
    {
        public const string ChampionName = "balblabll";
        static Orbwalking.Orbwalker Orbwalker;
        static Menu Menu;
        public static LeagueSharp.Common.Spell Q;
        public static LeagueSharp.Common.Spell W;
        public static LeagueSharp.Common.Spell E;
        public static LeagueSharp.Common.Spell R;

        public static void Game_OnGameLoad()
        {
            Orbwalker = new Orbwalking.Orbwalker();

            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 1);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 1);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 1);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 1);

            Game.OnUpdate += Game_OnGameUpdate;

        }
        private static void Game_OnGameUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:

                    var Target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

                    if (Q.IsReady()) Q.Cast(Target);
                    if (W.IsReady()) W.Cast(Target);
                    if (E.IsReady()) E.Cast(Target);
                    if (R.IsReady()) R.Cast(Target);
                    break;
            }
        }
    }
}