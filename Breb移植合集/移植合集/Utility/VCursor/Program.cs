using System;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy;

namespace VCursor
{
    internal class Program
    {
        public static Menu Menu;
        public static bool FollowMovement => Menu["Movement"].Cast<CheckBox>().CurrentValue;

        public static void Game_OnGameLoad()
        {
            Menu = MainMenu.AddMenu("VCursor", "VCursor");
            Menu.Add("Movement", new CheckBox("Follow Cursor Movement"));
            Menu.Add("Icon", new CheckBox("Change Icon [BROKEN IN L#]"));
            Menu.Add("Chat", new CheckBox("Clear Chat on Load"));

            if (Menu["Chat"].Cast<CheckBox>().CurrentValue)
            {
                for (var i = 0; i < 15; i++)
                {
                    Chat.Print("<font color =\"\">");
                }
            }

            FakeClicks.Initialize(Menu);
            VirtualCursor.Initialize();
            VirtualCursor.SetPosition(Cursor.ScreenPosition);
            VirtualCursor.Draw();
        }
    }
}