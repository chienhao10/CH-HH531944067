using System;
using System.Linq;
using EloBuddy;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;

namespace NabbActivator
{
    /// <summary>
    ///     The activator class.
    /// </summary>
    internal partial class Activator
    {
        /// <summary>
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void Cleansers(EventArgs args)
        {
            if (!Vars.getCheckBoxItem(Vars.TypesMenu, "cleansers"))
            {
                return;
            }

            /// <summary>
            ///     The Mikaels Crucible Logic.
            /// </summary>
            if (Items.CanUseItem(3222))
            {
                foreach (var ally in GameObjects.AllyHeroes.Where(
                    a =>
                        Bools.ShouldCleanse(a) &&
                        a.LSIsValidTarget(750f, false)))
                {
                    DelayAction.Add(Vars.Delay, () =>
                    {
                        Items.UseItem(3222, ally);
                    });
                }
            }

            if (Bools.ShouldUseCleanser() ||
                (!SpellSlots.Cleanse.IsReady() &&
                    Bools.ShouldCleanse(GameObjects.Player)))
            {
                /// <summary>
                ///     The Quicksilver Sash Logic.
                /// </summary>
                if (Items.CanUseItem(3140))
                {
                    DelayAction.Add(Vars.Delay, () =>
                    {
                        Items.UseItem(3140);
                        return;
                    });
                }

                /// <summary>
                ///     The Dervish Blade Logic.
                /// </summary>
                if (Items.CanUseItem(3137))
                {
                    DelayAction.Add(Vars.Delay, () =>
                    {
                        Items.UseItem(3137);
                        return;
                    });
                }

                /// <summary>
                ///     The Mercurial Scimitar Logic.
                /// </summary>
                if (Items.CanUseItem(3139))
                {
                    DelayAction.Add(Vars.Delay, () =>
                    {
                        Items.UseItem(3139);
                    });
                }
            }

            if (GameObjects.Player.HealthPercent < 10)
            {
                /// <summary>
                ///     The Dervish Blade Logic.
                /// </summary>
                if (Items.CanUseItem(3137))
                {
                    DelayAction.Add(Vars.Delay, () =>
                    {
                        Items.UseItem(3137);
                        return;
                    });
                }

                /// <summary>
                ///     The Mercurial Scimitar Logic.
                /// </summary>
                if (Items.CanUseItem(3139))
                {
                    DelayAction.Add(Vars.Delay, () =>
                    {
                        Items.UseItem(3139);
                    });
                }
            }
        }
    }
}