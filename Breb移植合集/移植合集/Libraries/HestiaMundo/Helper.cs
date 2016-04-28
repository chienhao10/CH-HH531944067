using EloBuddy;
using LeagueSharp.Common;
using ItemData = LeagueSharp.Common.Data.ItemData;

namespace Mundo
{
    internal class CommonUtilities
    {
        public static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public static HitChance GetHitChance(string name)
        {
            var hitChance = Mundo.getSliderItem(Mundo.miscMenu, "hitchanceQ");

            switch (hitChance)
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
                case 3:
                    return HitChance.VeryHigh;
            }
            return HitChance.VeryHigh;
        }

        public static bool CheckItem()
        {
            return ItemData.Tiamat_Melee_Only.GetItem().IsReady() ||
                   ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady() ||
                   ItemData.Titanic_Hydra_Melee_Only.GetItem().IsReady();
        }

        public static void UseItem()
        {
            if (!CheckItem())
                return;

            if (ItemData.Tiamat_Melee_Only.GetItem().IsOwned(Player))
            {
                ItemData.Tiamat_Melee_Only.GetItem().Cast();
            }
            if (ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsOwned(Player))
            {
                ItemData.Ravenous_Hydra_Melee_Only.GetItem().Cast();
            }
            if (ItemData.Titanic_Hydra_Melee_Only.GetItem().IsOwned(Player))
            {
                ItemData.Titanic_Hydra_Melee_Only.GetItem().Cast();
            }
        }
    }
}