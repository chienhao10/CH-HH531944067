using System;
using System.Collections.Generic;
using System.Linq;
using AutoSharp.Utils;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;

namespace AutoSharp.Auto.SummonersRift.SRShopAI
{
    internal static class BuyItemEvent
    {
        public delegate void OnBoughtItem(InventorySlot item);

        //public static short BuyItemAns = 0x97;
        public static int LastUpdate;
        private static List<ItemId> Inventory = new List<ItemId>();

        static BuyItemEvent()
        {
            Loading.OnLoadingComplete += Game_OnGameLoad;
        }

        public static event OnBoughtItem OnBuyItem;

        private static void Game_OnGameLoad(EventArgs args)
        {
            UpdateInventory();
            Game.OnUpdate += OnUpdate;
            //Game.OnProcessPacket += Game_OnProcessPacket;
        }

        private static void OnUpdate(EventArgs args)
        {
            if (!ObjectManager.Player.InFountain() || Environment.TickCount - LastUpdate <= 500) return;
            LastUpdate = Environment.TickCount;
            Core.DelayAction(UpdateInventory, 300);
        }

        /*private static void Game_OnProcessPacket(GamePacketEventArgs args)
        {
            if (args.PacketData.GetPacketId().Equals(BuyItemAns))
            {
                Utility.DelayAction.Add(300, UpdateInventory);
            }
        }*/

        private static void UpdateInventory()
        {
            if (Inventory.Count == 0)
            {
                Inventory = GetInventoryItems();
                return;
            }

            if (Inventory.Count == ObjectManager.Player.InventoryItems.Length)
            {
                return;
            }

            var items = new List<ItemId>().Concat(Inventory).ToList();

            foreach (var item in ObjectManager.Player.InventoryItems)
            {
                if (items.Contains(item.Id))
                {
                    items.Remove(item.Id);
                    continue;
                }

                if (OnBuyItem != null)
                {
                    OnBuyItem(item);
                }
            }

            Inventory.Clear();
            Inventory = GetInventoryItems();
        }

        private static List<ItemId> GetInventoryItems()
        {
            return ObjectManager.Player.InventoryItems.Select(item => item.Id).ToList();
        }

        private static short GetPacketId(this IReadOnlyList<byte> data)
        {
            return (short)((data[1] << 8) | (data[0] << 0));
        }
    }
}
