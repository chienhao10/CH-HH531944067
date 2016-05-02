namespace KappaUtility.Trackers
{
    using System;
    using System.Globalization;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;

    using SharpDX;

    using Color = System.Drawing.Color;

    internal class Traps : Tracker
    {
        internal static void Draw()
        {
            if (!TrackMenu["Tracktraps"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            var traps = ObjectManager.Get<Obj_AI_Minion>();
            {
                foreach (var trap in traps.Where(trap => trap != null && trap.IsEnemy))
                {
                    switch (trap.Name)
                    {
                        case "Cupcake Trap":
                            Drawing.DrawText(Drawing.WorldToScreen(trap.Position) - new Vector2(30, -30), Color.White, "陷阱", 2);
                            Circle.Draw(SharpDX.Color.White, trap.BoundingRadius + 10, trap.Position);
                            break;

                        case "Noxious Trap":
                            if (trap.BaseSkinName == "NidaleeSpear")
                            {
                                var endTime = Math.Max(0, -Game.Time + 120);
                                Drawing.DrawText(Drawing.WorldToScreen(trap.Position) - new Vector2(30, -30), Color.White, "奶大力W", 2);
                                Circle.Draw(SharpDX.Color.White, trap.BoundingRadius + 25, trap.Position);
                            }

                            if (trap.BaseSkinName == "TeemoMushroom")
                            {
                                if (trap.GetBuff("BantamTrap") != null)
                                {
                                    var endTime = Math.Max(0, trap.GetBuff("BantamTrap").EndTime - Game.Time);
                                    Drawing.DrawText(
                                        Drawing.WorldToScreen(trap.Position) - new Vector2(30, -30),
                                        Color.White,
                                        "提莫R: " + Convert.ToString(endTime, CultureInfo.InstalledUICulture),
                                        2);
                                }
                                else if (trap.GetBuff("BantamTrap") == null)
                                {
                                    Drawing.DrawText(Drawing.WorldToScreen(trap.Position) - new Vector2(30, -30), Color.White, "提莫R", 2);
                                }

                                Circle.Draw(SharpDX.Color.White, trap.BoundingRadius * 3, trap.Position);
                            }
                            if (trap.BaseSkinName == "JhinTrap")
                            {
                                if (trap.GetBuff("JhinETrap") != null)
                                {
                                    var endTime = Math.Max(0, trap.GetBuff("JhinETrap").EndTime - Game.Time);
                                    Drawing.DrawText(
                                        Drawing.WorldToScreen(trap.Position) - new Vector2(30, -30),
                                        Color.White,
                                        "烬陷阱: " + Convert.ToString(endTime, CultureInfo.InstalledUICulture),
                                        2);
                                }
                                else if (trap.GetBuff("JhinETrap") == null)
                                {
                                    Drawing.DrawText(Drawing.WorldToScreen(trap.Position) - new Vector2(30, -30), Color.White, "烬陷阱", 2);
                                }

                                Circle.Draw(SharpDX.Color.White, trap.BoundingRadius * 3, trap.Position);
                            }
                            break;

                        case "Jack In The Box":
                            if (trap.GetBuff("JackInTheBox") != null)
                            {
                                var endTime = Math.Max(0, trap.GetBuff("JackInTheBox").EndTime - Game.Time);
                                Drawing.DrawText(
                                    Drawing.WorldToScreen(trap.Position) - new Vector2(30, -30),
                                    Color.White,
                                    "小丑W: " + Convert.ToString(endTime, CultureInfo.InvariantCulture),
                                    2);
                            }
                            else if (trap.GetBuff("JackInTheBox") == null)
                            {
                                Drawing.DrawText(Drawing.WorldToScreen(trap.Position) - new Vector2(30, -30), Color.White, "小丑W", 2);
                            }

                            Circle.Draw(SharpDX.Color.White, trap.BoundingRadius * 15, trap.Position);
                            break;
                    }
                }
            }
        }
    }
}