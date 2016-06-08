using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using forms = System.Windows.Forms;
using EloBuddy.SDK.Menu;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;

namespace PastingSharp
{
    public class Program
    {
        public static string contents = "";
        public static string[] linestoprint;
        public static Menu menu;
        public static int tick;

        public static void Game_OnGameLoad()
        {
            menu = MainMenu.AddMenu("PastingSharp", "pasting");
            menu.Add("sleep", new Slider("Pause between pastes (milliseconds)", 0, 0, 1000));
            menu.Add("paste", new KeyBind("Paste", false, KeyBind.BindTypes.HoldActive, 'N'));
            menu.Add("allChat", new CheckBox("All Chat?"));
            Chat.Print("PastingSharp loaded. Press N to paste.");
            Game.OnUpdate += Game_OnGameUpdate;
        }
        public static void Game_OnGameUpdate(EventArgs args)
        {
            var sleep = (menu["sleep"].Cast<Slider>().CurrentValue);
            if (Environment.TickCount - tick < sleep)
            {
                return;
            }
            if (forms.Clipboard.ContainsText())
            {
                contents = forms.Clipboard.GetText();
                if (contents.Contains("\n"))
                {
                    var separator = new string[] { "\n" };
                    linestoprint = contents.Split(separator, StringSplitOptions.None);
                }
            }

            if (menu["paste"].Cast<KeyBind>().CurrentValue)
            {
                if (linestoprint == null)
                {
                    if (menu["allChat"].Cast<CheckBox>().CurrentValue)
                    {
                        Chat.Say("/all" + " " + contents);
                    }
                    else
                    {
                        Chat.Say(contents);
                    }
                }
                else
                {
                    foreach (string s in linestoprint)
                    {
                        if (menu["allChat"].Cast<CheckBox>().CurrentValue)
                        {
                            Chat.Say("/all" + " " + s);
                        }
                        else
                        {
                            Chat.Say(s);
                        }
                    }
                    var linestoprintsize = contents.Count();
                    Array.Clear(linestoprint, 0, linestoprintsize);
                }
                tick = Environment.TickCount;
            }
        }
    }
}