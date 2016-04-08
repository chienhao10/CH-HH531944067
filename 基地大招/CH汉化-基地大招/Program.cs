using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Notifications;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;
using Color = System.Drawing.Color;

namespace Baseult
{
    class Program
    {
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            switch (Player.Instance.ChampionName)
            {
                case "Jinx":

                    BaseultMenu.Load();
                    Baseult.Load();
                    Drawing.OnEndScene += Drawing_OnEndScene;

                    break;

                case "Draven":

                    BaseultMenu.Load();
                    Baseult.Load();
                    Drawing.OnEndScene += Drawing_OnEndScene;

                    break;

                case "Ashe":

                    BaseultMenu.Load();
                    Baseult.Load();
                    Drawing.OnEndScene += Drawing_OnEndScene;

                    break;

                case "Ezreal":

                    BaseultMenu.Load();
                    Baseult.Load();
                    Drawing.OnEndScene += Drawing_OnEndScene;

                    break;
            }
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            int Count = 0;

            foreach (var x in Baseult.Recalls)
            {
                Count++;

                Color ColorRecall = Color.Red;
                Color ColorHealth = Color.Red;

                if (x.Unit.HealthPercent >= 70)
                {
                    ColorHealth = Color.LimeGreen;
                }
                else if (x.Unit.HealthPercent >= 30)
                {
                    ColorHealth = Color.Orange;
                }
                else if (x.Unit.HealthPercent >= 15)
                {
                    ColorHealth = Color.Red;
                }

                if (x.Status == RecallStatus.Active)
                {
                    ColorRecall = Color.LimeGreen;
                }
                else if (x.Status == RecallStatus.Abort)
                {
                    ColorRecall = Color.Red;
                }
                else if (x.Status == RecallStatus.Finished)
                {
                    ColorRecall = Color.Orange;
                }

                if (x.Status != RecallStatus.Inactive)
                {
                    if (Count == 1)
                    {
                        Drawing.DrawText(Player.Instance.HPBarXOffset + 50, Player.Instance.HPBarYOffset + 50, Color.White, x.Unit.ChampionName);
                        Drawing.DrawText(Player.Instance.HPBarXOffset + 50, Player.Instance.HPBarYOffset + 70, ColorHealth, "Health: " + x.Unit.HealthPercent + "%");
                        Drawing.DrawText(Player.Instance.HPBarXOffset + 50, Player.Instance.HPBarYOffset + 90, ColorRecall, "Recall Status: " + x.Status);
                    }
                    else if (Count == 2)
                    {
                        Drawing.DrawText(Player.Instance.HPBarXOffset + 50, Player.Instance.HPBarYOffset + 110, Color.White, x.Unit.ChampionName);
                        Drawing.DrawText(Player.Instance.HPBarXOffset + 50, Player.Instance.HPBarYOffset + 130, ColorHealth, "Health: " + x.Unit.HealthPercent + "%");
                        Drawing.DrawText(Player.Instance.HPBarXOffset + 50, Player.Instance.HPBarYOffset + 150, ColorRecall, "Recall Status: " + x.Status);
                    }
                    else if (Count == 3)
                    {
                        Drawing.DrawText(Player.Instance.HPBarXOffset + 50, Player.Instance.HPBarYOffset + 170, Color.White, x.Unit.ChampionName);
                        Drawing.DrawText(Player.Instance.HPBarXOffset + 50, Player.Instance.HPBarYOffset + 190, ColorHealth, "Health: " + x.Unit.HealthPercent + "%");
                        Drawing.DrawText(Player.Instance.HPBarXOffset + 50, Player.Instance.HPBarYOffset + 210, ColorRecall, "Recall Status: " + x.Status);
                    }
                    else if (Count == 4)
                    {
                        Drawing.DrawText(Player.Instance.HPBarXOffset + 50, Player.Instance.HPBarYOffset + 230, Color.White, x.Unit.ChampionName);
                        Drawing.DrawText(Player.Instance.HPBarXOffset + 50, Player.Instance.HPBarYOffset + 250, ColorHealth, "Health: " + x.Unit.HealthPercent + "%");
                        Drawing.DrawText(Player.Instance.HPBarXOffset + 50, Player.Instance.HPBarYOffset + 270, ColorRecall, "Recall Status: " + x.Status);
                    }
                }
            }

            if(Baseult.BaseUltUnits.Any())
            {
                Drawing.DrawText(Player.Instance.HPBarPosition.X, Player.Instance.HPBarPosition.Y - 15, Color.Red, "Baseult is Coming !");
            }
        }
    }
}
