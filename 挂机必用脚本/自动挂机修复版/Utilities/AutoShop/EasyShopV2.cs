using System.Collections.Generic;
using System.Linq;
using AutoBuddy.Humanizers;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace AutoBuddy.Utilities.AutoShop
{
    internal enum ShopActionType
    {
        Buy = 1,
        Sell = 2,
        StartHpPot = 3,
        StopHpPot = 4
    }

    public static class ShopGlobals
    {
        public static int GoldForNextItem=999999;
        public static string Next;
    }

    internal class EasyShopV2
    {
        private readonly List<BuildElement> buildElements;
        private readonly CheckBox enabled;
        private bool first = false;

        public EasyShopV2(List<BuildElement> elements, CheckBox en)
        {
            enabled = en;
            buildElements = elements;
            Shopping();
        }


        private void Shopping()
        {
            List<LoLItem> myit = BrutalItemInfo.MyItems();
            if (!first&&(!enabled.CurrentValue || !ObjectManager.Player.IsInShopRange() || !buildElements.Any()))
            {
                first = true;
                Core.DelayAction(Shopping, 300);
                return;
            }

            ShopGlobals.GoldForNextItem = 9999999;
            int currentPos = BrutalItemInfo.GetNum(buildElements);
            if (currentPos == -1)
                ShopGlobals.Next = "Inventories mismatch, won't buy any items";
            if (currentPos == 0)
            {
                if (!myit.Any())
                {
                    Shop.BuyItem(buildElements.First(el => el.position == 1).item.id);
                    Core.DelayAction(Shopping, 800);
                    return;
                }
            }
            if (currentPos + 2 > buildElements.Count)
            {
                Core.DelayAction(Shopping, RandGen.r.Next(400, 800));
                return;
            }

            if (buildElements.First(b => b.position == currentPos + 2).action != ShopActionType.Buy)
                foreach (
                    BuildElement buildElement in
                        buildElements.Where(b => b.position > currentPos + 1).OrderBy(b => b.position).ToList())
                {
                    if (buildElement.action == ShopActionType.Buy || buildElement.action == ShopActionType.Sell) break;

                    currentPos++;
                    if (currentPos + 2 > buildElements.Count)
                    {
                        Core.DelayAction(Shopping, RandGen.r.Next(400, 800));
                        return;
                    }
                }
            

            if (currentPos < buildElements.Count - 1)
            {
                BuildElement b = buildElements.First(el => el.position == currentPos + 2);
                if (b.action == ShopActionType.Sell)
                {
                    int slot = BrutalItemInfo.GetItemSlot(buildElements.First(el => el.position == currentPos + 2).item.id);
                    if (slot != -1)
                    {
                        Shop.SellItem(slot);
                        
                    }
                    else
                    {
                        b = buildElements.First(el => el.position == currentPos + 3);
                    }
                }

                if (b.action == ShopActionType.Buy)
                {
                    ShopGlobals.Next = b.item.name;
                    ShopGlobals.GoldForNextItem = BrutalItemInfo.BuyItemSim(myit, b.item);
                    Shop.BuyItem(b.item.id);
                }

            }


            Core.DelayAction(() =>
            {
                if (currentPos == -1) return;
                List<BuildElement> cur = buildElements.Where(b => b.position < currentPos+2).ToList();

                int hp = cur.Count(e => e.action == ShopActionType.StartHpPot) -
                         cur.Count(e => e.action == ShopActionType.StopHpPot);
                if (hp > 0 && !AutoWalker.p.InventoryItems.Any(it => it.Id.IsHealthlyConsumable()))
                    Shop.BuyItem(ItemId.Health_Potion);
                else if (hp <= 0)
                {
                    int slot = BrutalItemInfo.GetHealtlyConsumableSlot();
                    if (slot != -1)
                       Shop.SellItem(slot);
                }
            }
                , 150);

            Core.DelayAction(Shopping, RandGen.r.Next(600, 1000));
        }
    }
}