using System;
using ExorSDK.Utilities;
using LeagueSharp.SDK;
using SharpDX;
using LeagueSharp.SDK.Core.Utils;

namespace ExorSDK.Champions.Sivir
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
            if (!Targets.Target.LSIsValidTarget() ||
                Invulnerable.Check(Targets.Target))
            {
                return;
            }

            /// <summary>
            ///     The Q Harass Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                Targets.Target.LSIsValidTarget(Vars.Q.Range) &&
                GameObjects.Player.ManaPercent >
                    ManaManager.GetNeededMana(Vars.Q.Slot, Menus.getSliderItem(Vars.QMenu, "harass")) &&
                Menus.getSliderItem(Vars.QMenu, "harass") != 101)
            {
                if (GameObjects.Player.Distance(Targets.Target) > 650 &&
                    Vars.Q.GetPrediction(Targets.Target).Hitchance >= HitChance.Medium)
                {
                    Vars.Q.Cast(Vars.Q.GetPrediction(Targets.Target).UnitPosition.Extend((Vector2)GameObjects.Player.ServerPosition, -140));
                }
            }
        }
    }
}