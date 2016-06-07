using System;
using System.Linq;
using EloBuddy;
using LeagueSharp.SDK;

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
        public static void Consumables(EventArgs args)
        {
            if (ObjectManager.Player.InFountain() ||
                ObjectManager.Player.LSIsRecalling())
            {
                return;
            }

            if (!Vars.getCheckBoxItem(Vars.TypesMenu, "potions"))
            {
                return;
            }

            if (!Bools.IsHealthPotRunning())
            {
                /// <summary>
                ///     The Refillable Potion Logic.
                /// </summary>
                if (Items.CanUseItem(2031) &&
                    ObjectManager.Player.HealthPercent < Managers.MinHealthPercent)
                {
                    Items.UseItem(2031);
                    return;
                }

                /// <summary>
                ///     The Total Biscuit of Rejuvenation Logic.
                /// </summary>
                if (Items.CanUseItem(2010) &&
                    ObjectManager.Player.HealthPercent < Managers.MinHealthPercent)
                {
                    Items.UseItem(2010);
                    return;
                }

                /// <summary>
                ///     The Health Potion Logic.
                /// </summary>
                if (Items.CanUseItem(2003) &&
                    ObjectManager.Player.HealthPercent < Managers.MinHealthPercent)
                {
                    Items.UseItem(2003);
                }
            }

            if (ObjectManager.Player.MaxMana < 200)
            {
                return;
            }

            /// <summary>
            ///     The Hunter's Potion Logic.
            /// </summary>
            if (Items.CanUseItem(2032))
            {
                if (!Bools.IsHealthPotRunning() &&
                    ObjectManager.Player.HealthPercent < Managers.MinHealthPercent)
                {
                    Items.UseItem(2032);
                }
                else if (!Bools.IsManaPotRunning() &&
                    ObjectManager.Player.ManaPercent < Managers.MinManaPercent)
                {
                    Items.UseItem(2032);
                }
            }

            /// <summary>
            ///     The Corrupting Potion Logic.
            /// </summary>
            if (Items.CanUseItem(2033))
            {
                if (!Bools.IsHealthPotRunning() &&
                    ObjectManager.Player.HealthPercent < Managers.MinHealthPercent)
                {
                    Items.UseItem(2033);
                }
                else if (!Bools.IsManaPotRunning() &&
                    ObjectManager.Player.ManaPercent < Managers.MinManaPercent)
                {
                    Items.UseItem(2033);
                }
            }
        }
    }
}