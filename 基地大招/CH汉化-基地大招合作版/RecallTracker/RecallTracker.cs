using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using HumanziedBaseUlt.Properties;
using SharpDX;
using Color = System.Drawing.Color;

namespace HumanziedBaseUlt.RecallTracker
{
    static class RecallTracker
    {
        public static readonly List<Recall> Recalls = new List<Recall>();

        private static readonly TextureLoader TextureLoader = new TextureLoader();

        public static void Init()
        {
            Loading_OnLoadingComplete();
        }

        private static Sprite TopSprite { get; set; }
        private static Sprite BottomSprite { get; set; }
        private static Sprite BackSprite { get; set; }
        private static Text Text { get; set; }
        private static Text TextTwo { get; set; }

        private static void Loading_OnLoadingComplete()
        {
            Listing.recallTrackerMenu = Listing.config.AddSubMenu("Recall Tracker", "RecallTrackerAIOGlobalUlt");
            Listing.recallTrackerMenu.AddLabel("X/Y Settings");
            Listing.recallTrackerMenu.Add("recallX", new Slider("X Offset", 0, -500, 500));
            Listing.recallTrackerMenu.Add("recallY", new Slider("Y Offset", 0, -500, 500));
            Listing.recallTrackerMenu.AddLabel("Misc Settings");
            var a = Listing.recallTrackerMenu.Add("resetDefault", new CheckBox("Reset X/Y", false));
            Listing.recallTrackerMenu.Add("alwaysDrawFrame", new CheckBox("Always Draw Frame", false));
            Listing.recallTrackerMenu.Add("drawPlayerNames", new CheckBox("Draw Player Names (All for One)", false));

            a.OnValueChange += delegate
            {
                Listing.recallTrackerMenu["recallX"].Cast<Slider>().CurrentValue = 0;
                Listing.recallTrackerMenu["recallY"].Cast<Slider>().CurrentValue = 0;
            };

            TextureLoader.Load("top", Resources.TopHUD);
            TextureLoader.Load("bottom", Resources.BottomHUD);
            TextureLoader.Load("back", Resources.Back);

            TopSprite = new Sprite(() => TextureLoader["top"]);
            BottomSprite = new Sprite(() => TextureLoader["bottom"]);
            BackSprite = new Sprite(() => TextureLoader["back"]);

            Text = new Text("", new Font(FontFamily.GenericSansSerif, 8, FontStyle.Bold)) { Color = Color.AntiqueWhite };
            TextTwo = new Text("", new Font(FontFamily.GenericSansSerif, 8, FontStyle.Bold)) { Color = Color.AntiqueWhite };

            Teleport.OnTeleport += Teleport_OnTeleport;
            Drawing.OnEndScene += Drawing_OnEndScene;
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            if (!Recalls.Any(recalll => recalll.GetsBaseUlted) && !Listing.recallTrackerMenu["alwaysDrawFrame"].Cast<CheckBox>().CurrentValue) return;

            int x = (int)((Drawing.Width * 0.846875) + Listing.recallTrackerMenu["recallX"].Cast<Slider>().CurrentValue);
            int y = (int)(Drawing.Height * 0.5555555555555556) + Listing.recallTrackerMenu["recallY"].Cast<Slider>().CurrentValue;

            TopSprite.Draw(new Vector2(x + 1, y));

            int bonus = 0;
            foreach (var recall in Recalls.ToList().Where(recall => recall.GetsBaseUlted))
            {
                BackSprite.Draw(new Vector2(x, y + 18 + bonus));

                Text.Draw(Listing.recallTrackerMenu["drawPlayerNames"].Cast<CheckBox>().CurrentValue ?
                    recall.Unit.Name.Truncate(10) : recall.Unit.ChampionName.Truncate(10), Color.White, x + 15, y + bonus + 27);
                Text.Draw(recall.PercentComplete() + "%", Color.White, new Vector2(x + 258, y + bonus + 26));


                Line.DrawLine(Color.White, 10, new Vector2(x + 80, y + bonus + 33), new Vector2(x + 250, y + bonus + 33));

                Line.DrawLine(recall.IsAborted ? Color.Orange : BarColour(recall.PercentComplete()), 10,
                    new Vector2(x + 80, y + bonus + 33),
                    new Vector2(x + 80 + (170 * (recall.PercentComplete() / 100)), y + bonus + 33));

                DrawShootTime(x, y, bonus, recall);
                bonus += 31;



                if (recall.ExpireTime < Environment.TickCount && Recalls.Contains(recall))
                {
                    Recalls.Remove(recall);
                }
            }

            BottomSprite.Draw(new Vector2(x + 1, y + bonus + 18));
        }

        private static void DrawShootTime(int x, int y, int bonus, Recall recall)
        {
            if (!recall.GetsBaseUlted)
                return;

            var percentage = recall.BaseUltArriveTimePercentComplete;
            var startBarVector = new Vector2(x + 80 + (170*(percentage/100)), y + bonus + 33);
            var endBarVector = new Vector2(x + 80 + (170 * (percentage / 100 - 0.02f)), y + bonus + 33);

            Line.DrawLine(Color.Red, 10,
                                startBarVector,
                                endBarVector);
        }

        private static Color BarColour(float percent)
        {
            if (percent > 80)
            {
                return Color.LawnGreen;
            }
            if (percent > 60)
            {
                return Color.GreenYellow;
            }

            if (percent > 40)
            {
                return Color.MediumSpringGreen;
            }
            if (percent > 20)
            {
                return Color.Aquamarine;
            }
            if (percent > 0)
            {
                return Color.DeepSkyBlue;
            }
            return Color.DeepSkyBlue;
        }

        private static void Teleport_OnTeleport(Obj_AI_Base sender, Teleport.TeleportEventArgs args)
        {
            if (args.Type != TeleportType.Recall || !(sender is AIHeroClient) || sender.IsAlly && !sender.IsMe || sender.IsMe) return;

            switch (args.Status)
            {
                case TeleportStatus.Abort:
                    foreach (var source in Recalls.Where(a => a.Unit == sender))
                    {
                        source.Abort();
                    }
                    break;
                case TeleportStatus.Start:
                    var recall = Recalls.FirstOrDefault(a => a.Unit == sender);
                    if (recall != null)
                    {
                        Recalls.Remove(recall);
                    }
                    Recalls.Add(new Recall((AIHeroClient)sender, Environment.TickCount, 
                        Environment.TickCount + args.Duration, args.Duration) { GetsBaseUlted = false})
                    ;
                    break;
            }
        }

        private static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }
}
