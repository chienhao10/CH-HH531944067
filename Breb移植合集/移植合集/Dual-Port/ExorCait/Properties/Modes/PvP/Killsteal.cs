using System;
using System.Linq;
using ExorSDK.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy;

namespace ExorSDK.Champions.Caitlyn
{
    /// <summary>
    ///     The logics class.
    /// </summary>
    internal partial class Logics
    {
        /// <summary>
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void Killsteal(EventArgs args)
        {
            if (GameObjects.EnemyHeroes.Any(t => t.LSIsValidTarget(Vars.AARange)))
            {
                return;
            }

            /// <summary>
            ///     The KillSteal Q Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                Vars.getCheckBoxItem(Vars.QMenu, "killsteal"))
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(
                    t =>
                        !Invulnerable.Check(t) &&
                        t.LSIsValidTarget(Vars.Q.Range - 200f)))
                {
                    if (Vars.GetRealHealth(target) <
                            (float)GameObjects.Player.LSGetSpellDamage(target, SpellSlot.Q) *
                                (!Vars.Q.GetPrediction(target).CollisionObjects.Any()
                                    ? 1
                                    : 0.67))
                    {
                        Vars.Q.Cast(Vars.Q.GetPrediction(target).UnitPosition);
                        return;
                    }
                }
            }

            if (GameObjects.EnemyHeroes.Any(t => t.LSIsValidTarget(Vars.W.Range)))
            {
                return;
            }

            /// <summary>
            ///     The KillSteal R Logic.
            /// </summary>
            if (Vars.R.IsReady() &&
                Vars.getCheckBoxItem(Vars.RMenu, "killsteal"))
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(
                    t =>
                        !Invulnerable.Check(t) &&
                        t.LSIsValidTarget(Vars.R.Range) &&
                        Vars.GetRealHealth(t) <
                            (float)GameObjects.Player.LSGetSpellDamage(t, SpellSlot.R)))
                {
                    Vars.R.CastOnUnit(target);
                }
            }
        }
    }
}