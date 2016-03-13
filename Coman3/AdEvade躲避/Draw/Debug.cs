using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AdEvade.Config;
using EloBuddy;
using Menu = EloBuddy.SDK.Menu.Menu;

namespace AdEvade.Draw
{
    public static class Debug
    {
        private static List<string> textToWrite = new List<string>();

        static Debug()
        {
            Drawing.OnEndScene += Drawing_OnEndScene;
        }

        public static int DrawCount = 10;
        private static void Drawing_OnEndScene(EventArgs args)
        {
            if (ConfigValue.ShowDebugInfo.GetBool())
            {
                if (textToWrite.Count > DrawCount)
                {
                    textToWrite.RemoveRange(0, 1);
                }
                for (int i = 0; i < textToWrite.Count; i++)
                {
                    Drawing.DrawText(10f, 10f + (textToWrite[i].Count(x => x == '\n')*12f) + (12f*i),
                        textToWrite[i].StartsWith("*") ? Color.Red : Color.Yellow, textToWrite[i]);
                }
            }

        }

        public static void PrintChat(object data)
        {
            if (!ConfigValue.ShowDebugInfo.GetBool()) return;
            Chat.Print(data);
            ConsoleDebug.WriteLine(data);
        }
        public static void DrawTopLeft(object data)
        {
            ConsoleDebug.WriteLine(data);
            if (!ConfigValue.ShowDebugInfo.GetBool()) return;
            textToWrite.Add(data.ToString());
            ConsoleDebug.WriteLine(data);
        }

        public static bool DebugBool(this bool value, string key = "")
        {
            if(key == "")
                DrawTopLeft(value);
            else
                DrawTopLeft(key + " : " + value);
            return value;
        }
    }
}