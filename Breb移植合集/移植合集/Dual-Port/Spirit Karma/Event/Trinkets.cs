#region

using System;
using EloBuddy;
using LeagueSharp.SDK;
using Spirit_Karma.Menus;

#endregion

namespace Spirit_Karma.Event
{
    internal class Trinkets : Core.Core
    {
        public static void Update(EventArgs args)
        {
            if(!MenuConfig.getCheckBoxItem(MenuConfig.trinketMenu, "Trinket") || Player.Level < 9 || !Player.InShop() || Items.HasItem(3363) || Items.HasItem(3364)) return;

            switch (MenuConfig.getBoxItem(MenuConfig.trinketMenu, "TrinketList"))
            {
                case 0:
                    Shop.BuyItem(ItemId.Oracle_Alteration);
                    break;
                case 1:
                    Shop.BuyItem(ItemId.Farsight_Alteration);
                    break;
            }
        }
    }
}
