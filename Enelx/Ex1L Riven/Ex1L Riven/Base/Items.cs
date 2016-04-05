using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;

namespace Ex1L_Riven.Base
{
    internal class Items
    {
        public static Item Bilge = new Item(ItemId.Bilgewater_Cutlass, 550);
        public static Item Botrk = new Item(ItemId.Blade_of_the_Ruined_King, 550);
        public static Item Hextech = new Item(ItemId.Hextech_Gunblade, 700);
        public static Item Tiamat = new Item(ItemId.Tiamat_Melee_Only, 400);
        public static Item Hydra = new Item(ItemId.Ravenous_Hydra_Melee_Only, 400);
        public static Item Titanic = new Item(ItemId.Titanic_Hydra, Player.Instance.AttackRange);

        public static Item Youmuus = new Item(ItemId.Youmuus_Ghostblade,
            Player.Instance.AttackRange + Player.Instance.BoundingRadius + 300);

        public static List<Item> ItemList = new List<Item>
        {
            Bilge,
            Botrk,
            Hextech,
            Titanic,
            Youmuus
        };

        public static void CastItems(AIHeroClient target)
        {
            foreach (var item in ItemList.Where(i => i.IsReady() && target.IsValidTarget(i.Range)))
            {
                item.Cast(target);
            }
        }
    }
}