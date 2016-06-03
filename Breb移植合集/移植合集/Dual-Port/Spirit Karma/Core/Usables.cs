#region

using LeagueSharp.SDK;
using Spirit_Karma.Menus;

#endregion

namespace Spirit_Karma.Core
{
    internal class Usables : Core
    {
        public static void ProtoBelt()
        {
            if(!MenuConfig.getCheckBoxItem(MenuConfig.itemMenu, "UseItems") || !MenuConfig.getCheckBoxItem(MenuConfig.itemMenu, "ItemProtoBelt")) return;
            if (Items.CanUseItem(3152) && Target.LSIsValidTarget())
            {
                Items.UseItem(3152, Target.ServerPosition);
            }
        }
        public static void FrostQueen()
        {
            if (!MenuConfig.getCheckBoxItem(MenuConfig.itemMenu, "UseItems") || !MenuConfig.getCheckBoxItem(MenuConfig.itemMenu, "ItemFrostQueen")) return;
            if (Items.CanUseItem(3092) && Target.LSIsValidTarget())
            {
                Items.UseItem(3092, Target.ServerPosition);
            }
        }/*
        public static void Seraph()
        {
            if (!MenuConfig.UseItems || !MenuConfig.ItemSeraph) return;
            if (Health.GetPrediction(Player, 250) <= Player.MaxHealth/4)
            {
                if (Items.CanUseItem(3040))
                {
                    Items.UseItem(3040);
                }
            }
        }*/
        public static void Locket()
        {
            if (!MenuConfig.getCheckBoxItem(MenuConfig.itemMenu, "UseItems") || !MenuConfig.getCheckBoxItem(MenuConfig.itemMenu, "ItemLocket")) return;
            if (Health.GetPrediction(Player, 250) <= Player.MaxHealth / 4)
            {
                if (Items.CanUseItem(3190))
                {
                    Items.UseItem(3190);
                }
            }
        }
    }
}
