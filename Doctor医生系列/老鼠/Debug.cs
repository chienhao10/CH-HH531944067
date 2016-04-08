using System;
using System.Drawing;
using EloBuddy;
using Settings = Twitch.Config.DebugMenu;

namespace Twitch
{
    class Debug
    {
        private static int _lastConsoleMsg = Environment.TickCount; // To prevent msg spam
        private static int _lastChatMsg = Environment.TickCount;
        private static string _lastChatMsgText = "Chat";
        private static string _lastConsoleMsgText = "Console";

        public static void Write(String text)
        {
            WriteChat(text, Color.LightBlue);
            WriteConsole(text, ConsoleColor.Cyan);
        }

        public static void Write(string format, params string[] args)
        {
            WriteChat(String.Format(format, args), Color.LightBlue);
            WriteConsole(String.Format(format, args), ConsoleColor.Cyan);
        }

        public static void WriteChat(string text)
        {
            WriteChat(text, Color.LightBlue);
        }

        public static void WriteChat(string format, params string[] args)
        {
            WriteChat(String.Format(format, args), Color.LightBlue);
        }


        public static void WriteChat(string format, Color color, params string[] args)
        {
            WriteChat(String.Format(format, args), color);
        }

        public static void WriteChat(string text, Color color)
        {
            if (!Settings.DebugChat || text == null || text.Length == 0 || (text.Substring(0, text.Length >= 20 ? 20 : text.Length - 1).Equals(_lastChatMsgText, StringComparison.CurrentCultureIgnoreCase) && Environment.TickCount - _lastChatMsg < 500))
            {
                return;
            }
            Chat.Print("[Doctor{0}] {1}", color, Program.ChampName, text);
            _lastChatMsg = Environment.TickCount;
            _lastChatMsgText = text.Substring(0, text.Length >= 20 ? 20 : text.Length - 1);
        }

        public static void WriteConsole(string text)
        {
            WriteConsole(text, ConsoleColor.Cyan);
        }

        public static void WriteConsole(string format, params string[] args)
        {
            WriteConsole(String.Format(format, args), ConsoleColor.Cyan);
        }

        public static void WriteConsole(string format, ConsoleColor color, params string[] args)
        {
            WriteConsole(String.Format(format, args), color);
        }

        public static void WriteConsole(string text, ConsoleColor color)
        {
            if (!Settings.DebugConsole || text == null || text.Length == 0 || (text.Substring(0, 20).Equals(_lastConsoleMsgText, StringComparison.CurrentCultureIgnoreCase) && Environment.TickCount - _lastConsoleMsg < 500))
            {
                return;
            }
            Console.ForegroundColor = color;
            Console.WriteLine("[Doctor{0}] {1}", Program.ChampName, text);
            Console.ResetColor();
            _lastConsoleMsg = Environment.TickCount;
            _lastConsoleMsgText = text.Substring(0, text.Length >= 20 ? 20 : text.Length - 1);
        }
    }
}
