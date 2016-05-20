using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using Jhin___The_Virtuoso.Extensions;
using LeagueSharp.Common;

namespace Jhin___The_Virtuoso.Modes
{
    internal static class Mixed
    {
        /// <summary>
        ///     Execute Harass W
        /// </summary>
        private static void ExecuteW()
        {
            foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.W.Range)))
            {
                var pred = Spells.W.GetPrediction(enemy);
                if (pred.Hitchance >= Menus.wMenu.HikiChance("w.hit.chance"))
                {
                    Spells.W.Cast(enemy);
                }
            }
        }

        /// <summary>
        ///     Execute Harass
        /// </summary>
        public static void ExecuteHarass()
        {
            if (ObjectManager.Player.ManaPercent < Menus.getSliderItem(Menus.harassMenu, "harass.mana"))
            {
                return;
            }

            if (Spells.W.IsReady() && Menus.getCheckBoxItem(Menus.harassMenu, "w.harass"))
            {
                ExecuteW();
            }
        }
    }
}