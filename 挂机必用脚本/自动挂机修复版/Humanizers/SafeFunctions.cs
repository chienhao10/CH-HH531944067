using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace AutoBuddy.Humanizers
{
    internal static class SafeFunctions
    {
        private static float lastPing;
        private static float lastChat;

        static SafeFunctions()
        {
            lastChat = 0;
        }


        public static void Ping(PingCategory cat, Vector3 pos)
        {
            if (MainMenu.GetMenu("AB").Get<CheckBox>("disablepings").CurrentValue)
                return;

            if (lastPing > Game.Time) return;
            lastPing = Game.Time + 1.8f;
            Core.DelayAction(() => TacticalMap.SendPing(cat, pos), RandGen.r.Next(450, 800));
        }

        public static void Ping(PingCategory cat, GameObject target)
        {
            if (MainMenu.GetMenu("AB").Get<CheckBox>("disablepings").CurrentValue)
                return;

            if (lastPing > Game.Time) return;
            lastPing = Game.Time + 1.8f;
            Core.DelayAction(() => TacticalMap.SendPing(cat, target), RandGen.r.Next(450, 800));
        }

        public static void SayChat(string msg)
        {
            if (MainMenu.GetMenu("AB").Get<CheckBox>("disablechat").CurrentValue)
                return;

            if (lastChat > Game.Time) return;
            lastChat = Game.Time + .8f;
            Core.DelayAction(() => Chat.Say(msg), RandGen.r.Next(150, 400));
        }
    }
}