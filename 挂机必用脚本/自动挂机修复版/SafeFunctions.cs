using EloBuddy;
using EloBuddy.SDK;
using SharpDX;

namespace AutoBuddy
{
    internal static class SafeFunctions
    {
        private static float lastPing;
        private static float lastChat;
        private static readonly AutoShop autoShop;

        static SafeFunctions()
        {
            autoShop=new AutoShop();
        }

        public static void BuyItem(int itemId)
        {
            autoShop.Buy(itemId);
        }

        public static void BuyItem(ItemId itemId)
        {
            autoShop.Buy(itemId);
        }

        public static void BuyIfNotOwned(int itemId)
        {
            autoShop.BuyIfNotOwned(itemId);
        }

        public static void BuyIfNotOwned(ItemId itemId)
        {
            autoShop.BuyIfNotOwned(itemId);
        }

        public static void Ping(PingCategory cat, Vector3 pos)
        {
            if (lastPing > Game.Time) return;
            lastPing = Game.Time + .8f;
            Core.DelayAction(()=>TacticalMap.SendPing(cat, pos), RandGen.r.Next(150, 400));
        }

        public static void Ping(PingCategory cat, GameObject target)
        {
            if (lastPing > Game.Time) return;
            lastPing = Game.Time + .8f;
            Core.DelayAction(() => TacticalMap.SendPing(cat, target), RandGen.r.Next(150, 400));
        }

        public static void SayChat(string msg)
        {
            if (lastChat > Game.Time) return;
            lastChat = Game.Time + .8f;
            Core.DelayAction(() => Chat.Say(msg), RandGen.r.Next(150, 400));
        }
        
    }
}
