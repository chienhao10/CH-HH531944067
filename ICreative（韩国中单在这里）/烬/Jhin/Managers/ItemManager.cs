using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using Jhin;

namespace Jhin.Managers
{
    public static class ItemManager
    {
        public static List<Item> OffensiveItems = new List<Item>();
        public static Item Tiamat;
        public static Item Hydra;
        public static Item BladeOfTheRuinedKing;
        public static Item BilgewaterCutlass;
        public static Item HextechGunblade;
        public static Item YoumuusGhostblade;
        public static Item RanduinsOmen;
        public static Item ZhonyasHourglass;

        public static void Initialize()
        {
            Tiamat = new Item(ItemId.Tiamat_Melee_Only, 300);
            Hydra = new Item(ItemId.Ravenous_Hydra_Melee_Only, 300);
            BladeOfTheRuinedKing = new Item(ItemId.Blade_of_the_Ruined_King, 450);
            BilgewaterCutlass = new Item(ItemId.Bilgewater_Cutlass, 400);
            HextechGunblade = new Item(ItemId.Hextech_Gunblade, 400);
            YoumuusGhostblade = new Item(ItemId.Youmuus_Ghostblade,
                (int) (AIO.MyHero.AttackRange + AIO.MyHero.BoundingRadius + 300));
            RanduinsOmen = new Item(ItemId.Randuins_Omen, 500);
            ZhonyasHourglass = new Item(ItemId.Zhonyas_Hourglass);
            OffensiveItems = new List<Item>
            {
                Tiamat,
                Hydra,
                BladeOfTheRuinedKing,
                BilgewaterCutlass,
                HextechGunblade,
                YoumuusGhostblade,
                RanduinsOmen
            };
        }

        public static void CastOffensiveItems(AIHeroClient target)
        {
            foreach (var item in OffensiveItems.Where(i => i.IsReady() && target.IsValidTarget(i.Range)))
            {
                item.Cast(target);
            }
        }
    }
}