using System;
using System.Linq;
using ExorSDK.Utilities;
using LeagueSharp.SDK;
using SharpDX;
using Geometry = ExorSDK.Utilities.Geometry;
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
        public static void Harass(EventArgs args)
        {
            /// <summary>
            ///     The Harass Q Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.Q.Slot, Vars.getSliderItem(Vars.QMenu, "clear")) &&
                Vars.getSliderItem(Vars.QMenu, "clear") != 101)
            {
                if (GameObjects.EnemyHeroes.Any(
                    t =>
                        !Invulnerable.Check(t) &&
                        t.LSIsValidTarget(Vars.Q.Range)))
                {
                    if (Vars.Q.GetLineFarmLocation(Targets.Minions, Vars.Q.Width).MinionsHit >= 3 &&
                        !new Geometry.Rectangle(
                            GameObjects.Player.ServerPosition,
                            GameObjects.Player.ServerPosition.LSExtend(Targets.Minions[0].ServerPosition, Vars.Q.Range),
                            Vars.Q.Width).IsOutside(
                                (Vector2)
                                    Vars.Q.GetPrediction(GameObjects.EnemyHeroes.FirstOrDefault(
                                        t =>
                                            !Invulnerable.Check(t) &&
                                            t.LSIsValidTarget(Vars.Q.Range))).UnitPosition))
                    {
                        Vars.Q.Cast(Vars.Q.GetLineFarmLocation(Targets.Minions, Vars.Q.Width).Position);
                    }
                }
            }
        }
    }
}