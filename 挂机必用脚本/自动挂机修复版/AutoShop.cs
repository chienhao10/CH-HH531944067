using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;

namespace AutoBuddy
{
    internal class AutoShop
    {
        private float maxTime = 1.2f;
        private float minTime = .4f;
        private readonly List<int> itemsQueue;

        public float AdditionalMaxRandomTime
        {
            get { return maxTime; }
            set
            {
                if (value >= 0 && value <= 4 && value > minTime)
                    maxTime = value;
                else
                    Console.WriteLine("Shop MaxTime not set, invalid value.");
            }
        }

        public float AdditionalMinRandomTime
        {
            get { return minTime; }
            set
            {
                if (value >= 0 && value <= 4 && value < maxTime)
                    minTime = value;
                else
                    Console.WriteLine("Shop MinTime not set, invalid value.");
            }
        }
        private float lastShopActionTime;

        public AutoShop()
        {
            itemsQueue = new List<int>();
        }

        public void Buy(int id)
        {
            if (ObjectManager.Player.InventoryItems.Length == 7) return;
            itemsQueue.Add(id);
            if (itemsQueue.Count == 1)
                CheckQueue();
        }

        public void Buy(ItemId id)
        {
            Buy((int)id);
        }

        public void BuyIfNotOwned(ItemId id)
        {
            if (ObjectManager.Player.InventoryItems.All(it => it.Id != id))
                Buy(id);
        }
        public void BuyIfNotOwned(int id)
        {
            BuyIfNotOwned((ItemId)id);
        }

        private void CheckQueue()
        {
            if (lastShopActionTime > Game.Time)
            {
                Core.DelayAction(CheckQueue, (int)((lastShopActionTime - Game.Time) * 1000));
                return;
            }

            int item = itemsQueue.Last();
            itemsQueue.Remove(item);
            if (Shop.BuyItem(item))
            {
                lastShopActionTime = Game.Time + RandGen.r.NextFloat(minTime, maxTime);
                if (itemsQueue.Any())
                    Core.DelayAction(CheckQueue, (int)((lastShopActionTime - Game.Time) * 1000));
                return;
            }

            if (itemsQueue.Any())
                CheckQueue();

        }

    }
}
