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
        public static void Automatic(EventArgs args)
        {
            if (GameObjects.Player.LSIsRecalling())
            {
                return;
            }

            /// <summary>
            ///     The Automatic Q Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                GameObjects.Player.CountEnemyHeroesInRange(Vars.Q.Range) == 1 &&
                Vars.getCheckBoxItem(Vars.QMenu, "logical"))
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(
                    t =>
                        !Invulnerable.Check(t) &&
                        t.LSIsValidTarget(Vars.Q.Range)))
                {
                    if (target.HasBuff("caitlynyordletrapdebuff") ||
                        target.HasBuff("caitlynyordletrapinternal"))
                    {
                        Vars.Q.Cast(Vars.Q.GetPrediction(target).UnitPosition);
                    }
                }
            }

            /// <summary>
            ///     The Automatic W Logic. 
            /// </summary>
            if (Vars.W.IsReady() &&
                Vars.getCheckBoxItem(Vars.WMenu, "logical"))
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(
                    t =>
                        Bools.IsImmobile(t) &&
                        !Invulnerable.Check(t) &&
                        t.LSIsValidTarget(Vars.W.Range)))
                {
                    if (!GameObjects.Minions.Any(
                        m =>
                            m.Distance(target.ServerPosition) < 100f &&
                            m.CharData.BaseSkinName.Contains("Cupcake")))
                    {
                        Vars.W.Cast(target.ServerPosition);
                    }
                }
            }

            /// <summary>
            ///     The Semi-Automatic R Management.
            /// </summary>
            if (Vars.R.IsReady() &&
                Vars.getCheckBoxItem(Vars.RMenu, "bool"))
            {
                if (GameObjects.EnemyHeroes.Any(t => t.LSIsValidTarget(Vars.R.Range)) &&
                    Vars.getKeyBindItem(Vars.RMenu, "key"))
                {
                    Vars.R.CastOnUnit(
                        GameObjects.EnemyHeroes
                            .Where(t => t.LSIsValidTarget(Vars.R.Range))
                            .OrderBy(o => o.Health)
                            .LastOrDefault());
                }
            }
        }
    }
}