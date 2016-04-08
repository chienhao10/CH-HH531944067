using System;

namespace AdEvade.Config
{
    public static class ConsoleDebug
    {
        public static bool Enabled = false;
        public static bool DrawAddonTag = true;
        public const string AddonTag = "[AdEvade] ";

        public static void WriteLineColor(object value, ConsoleColor color, bool force = false)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            WriteLine(value, force);
            Console.ForegroundColor = oldColor;
        }
        public static void WriteLineColor(string format, ConsoleColor color, bool force = false, params object[] values)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            WriteLine(format, force, values);
            Console.ForegroundColor = oldColor;
        }
        public static void WriteLine(object value, bool force = false)
        {
            if(Enabled || force)
                Console.WriteLine((DrawAddonTag ? AddonTag : "") + value);
        }
        public static void WriteLine(string format, params object[] values)
        {
            WriteLine(format, false, values);
        }
        public static void WriteLine(string format, bool force = false, params object[] values)
        {
            if(Enabled || force)
                Console.WriteLine((DrawAddonTag ? AddonTag : "") + format, values);
        }
    }
}