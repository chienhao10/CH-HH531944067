using LeagueSharp;
using LeagueSharp.Common;

namespace ExorAIO.Champions.Caitlyn
{
    using System;
    using System.Linq;
    using ExorAIO.Utilities;
    using EloBuddy;
    /// <summary>
    ///     The logics class.
    /// </summary>
    partial class Logics
    {
        /// <summary>
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs"/> instance containing the event data.</param>
        public static void Combo(EventArgs args)
        {
            if (Bools.HasSheenBuff() ||
                !Targets.Target.LSIsValidTarget() ||
                Bools.IsSpellShielded(Targets.Target))
            {
                return;
            }

            /// <summary>
            ///     The E Combo Logic.
            /// </summary>
            if (Variables.E.IsReady() && Variables.Q.IsReady() && ObjectManager.Player.ManaPercent >= 20 && Menus.getCheckBoxItem(Variables.EMenu, "espell.combo"))
            {
                foreach (AIHeroClient target in HeroManager.Enemies.Where(t => t.LSIsValidTarget(550f) && !Bools.IsSpellShielded(t) && !t.HasBuff("caitlynyordletrapinternal")))
                {
                    if (!(Variables.E.GetPrediction(target).CollisionObjects.Count > 1))
                    {
                        Variables.E.CastIfHitchanceEquals(target, HitChance.High);
                    }
                }
            }
        }
    }
}
