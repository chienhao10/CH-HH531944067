using System;
using System.Linq;
using ExorSDK.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;

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
        public static void Combo(EventArgs args)
        {
            if (Bools.HasSheenBuff() ||
                GameObjects.Player.Mana < Vars.E.Instance.SData.Mana + Vars.Q.Instance.SData.Mana)
            {
                return;
            }

            /// <summary>
            ///     The E Combo Logic.
            /// </summary>
            if (Vars.E.IsReady() &&
                Vars.getCheckBoxItem(Vars.EMenu, "combo"))
            {
                foreach (var target in
                    GameObjects.EnemyHeroes.Where(
                        t =>
                            t.LSIsValidTarget(550f) &&
                            !Invulnerable.Check(t) &&
                            !t.HasBuff("caitlynyordletrapinternal")))
                {
                    if (!Vars.E.GetPrediction(target).CollisionObjects.Any() &&
						Vars.E.GetPrediction(target).Hitchance >= HitChance.High)
                    {
                        Vars.E.Cast(Vars.E.GetPrediction(target).UnitPosition);

                        if (Vars.Q.IsReady() &&
                            Vars.getCheckBoxItem(Vars.QMenu, "logical"))
                        {
                            Vars.Q.Cast(target.ServerPosition);
                        }

                    }
                }
            }
        }
    }
}