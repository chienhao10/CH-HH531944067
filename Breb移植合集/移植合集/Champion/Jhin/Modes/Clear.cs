using EloBuddy;
using Jhin___The_Virtuoso.Extensions;
using LeagueSharp.Common;

namespace Jhin___The_Virtuoso.Modes
{
    internal static class Clear
    {
        /// <summary>
        ///     Execute Q Clear
        /// </summary>
        private static void ExecuteQ()
        {
            var min =
                MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Spells.Q.Range)
                    .MinOrDefault(x => x.Health);
            Spells.Q.CastOnUnit(min);
        }

        /// <summary>
        ///     Execute W Clear
        /// </summary>
        private static void ExecuteW()
        {
            var min = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Spells.W.Range);
            if (Spells.W.GetLineFarmLocation(min).MinionsHit >= Menus.getSliderItem(Menus.clearMenu, "w.hit.x.minion"))
            {
                Spells.W.Cast(Spells.W.GetLineFarmLocation(min).Position);
            }
        }

        /// <summary>
        ///     Execute Clear
        /// </summary>
        public static void ExecuteClear()
        {
            if (ObjectManager.Player.ManaPercent < Menus.getSliderItem(Menus.clearMenu, "clear.mana"))
            {
                return;
            }

            if (Spells.Q.IsReady() && Menus.getCheckBoxItem(Menus.clearMenu, "q.clear")) // done working
            {
                ExecuteQ();
            }

            if (Spells.W.IsReady() && Menus.getCheckBoxItem(Menus.clearMenu, "w.clear")) // done working
            {
                ExecuteW();
            }
        }
    }
}