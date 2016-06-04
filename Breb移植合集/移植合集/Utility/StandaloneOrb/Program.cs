
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
        static SCommon.Orbwalking.Orbwalker Orbwalker;
        public static LeagueSharp.Common.Spell Q;
        public static LeagueSharp.Common.Spell W;
        public static LeagueSharp.Common.Spell E;
        public static LeagueSharp.Common.Spell R;

        public static void Game_OnGameLoad()
        {
            Orbwalker = new SCommon.Orbwalking.Orbwalker();
            SCommon.TS.TargetSelector.Initialize();

            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 1);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 1);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 1);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 1);

            Chat.Print("In the EB Orbwalker make sure all the keys are the same as the standalone orbwalker.");
            Chat.Print("- Disable everything in drawings.");
            Chat.Print("- In Advanced Orbwalker settings check Disable Attacking & Movement");
            Chat.Print("Some things do NOT work such as resetting auto attack.");

            Game.OnUpdate += Game_OnGameUpdate;
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case SCommon.Orbwalking.Orbwalker.Mode.Combo:
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