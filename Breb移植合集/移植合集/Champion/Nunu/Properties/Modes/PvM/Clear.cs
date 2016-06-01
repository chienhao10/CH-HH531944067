using System;
using System.Linq;
using ExorSDK.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using EloBuddy;

namespace ExorSDK.Champions.Nunu
{
    /// <summary>
    ///     The logics class.
    /// </summary>
    internal partial class Logics
    {
        /// <summary>
        ///     Fired when the game is updated.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void Clear(EventArgs args)
        {
            if (Bools.HasSheenBuff())
            {
                return;
            }

            /// <summary>
            ///     The Q LaneClear Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                Targets.Minions.Any() &&
                Vars.getSliderItem(Vars.QMenu, "laneclear") != 101)
            {
                if (GameObjects.Player.ManaPercent <
                        ManaManager.GetNeededMana(Vars.Q.Slot, Vars.getSliderItem(Vars.QMenu, "laneclear")) &&
                    !GameObjects.Player.Buffs.Any(b => b.Name.Equals("visionary")))
                {
                    return;
                }

                Vars.Q.CastOnUnit(Targets.Minions.FirstOrDefault(
                    m =>
                        Vars.GetRealHealth(m) <
                            (float)GameObjects.Player.LSGetSpellDamage(m, SpellSlot.Q)));
            }

            /// <summary>
            ///     The E Clear Logics.
            /// </summary>
            if (Vars.E.IsReady() &&
                Vars.getSliderItem(Vars.EMenu, "clear") != 101)
            {
                if (GameObjects.Player.ManaPercent <
                        ManaManager.GetNeededMana(Vars.E.Slot, Vars.getSliderItem(Vars.EMenu, "clear")) &&
                    !GameObjects.Player.Buffs.Any(b => b.Name.Equals("visionary")))
                {
                    return;
                }

                /// <summary>
                ///     The E LaneClear Logic.
                /// </summary>
                foreach (var minion in Targets.Minions.Where(
                    m =>
                        m.LSIsValidTarget(Vars.E.Range) &&
                        Vars.GetRealHealth(m) <
                            (float)GameObjects.Player.LSGetSpellDamage(m, SpellSlot.E)))
                {
                    Vars.E.CastOnUnit(minion);
                }

                /// <summary>
                ///     The E JungleClear Logic.
                /// </summary>
                if (Targets.JungleMinions.Any())
                {
                    Vars.E.CastOnUnit(Targets.JungleMinions[0]);
                }
            }
        }
    }
}