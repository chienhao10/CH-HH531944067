using EloBuddy;
using Jhin___The_Virtuoso.Extensions;
using LeagueSharp.Common;

namespace Jhin___The_Virtuoso.Modes
{
    internal static class Jungle
    {
        /// <summary>
        ///     Execute Jungle
        /// </summary>
        public static void ExecuteJungle()
        {
            if (ObjectManager.Player.ManaPercent < Menus.getSliderItem(Menus.clearMenu, "jungle.mana"))
            {
                return;
            }

            var mobs = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Spells.Q.Range, MinionTypes.All,
                MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (mobs == null || (mobs.Count == 0))
            {
                return;
            }

            if (Spells.Q.IsReady() && Menus.getCheckBoxItem(Menus.clearMenu, "q.jungle"))
            {
                Spells.Q.Cast(mobs[0]);
            }

            if (Spells.W.IsReady() && Menus.getCheckBoxItem(Menus.clearMenu, "w.jungle"))
            {
                Spells.W.Cast(mobs[0]);
            }
        }
    }
}