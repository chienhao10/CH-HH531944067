using System.Drawing;
using LeagueSharp;
using LeagueSharp.SDK;
using EloBuddy;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy.SDK.Menu.Values;

namespace ExorSDK.Utilities
{
    /// <summary>
    ///     The drawings class.
    /// </summary>
    internal class Drawings
    {
        /// <summary>
        ///     Loads the drawings.
        /// </summary>
        public static void Initialize()
        {
            Drawing.OnDraw += delegate
            {
                /// <summary>
                ///     Loads the Q drawing,
                ///     Loads the Extended Q drawing.
                /// </summary>
                if (Vars.Q != null &&
                    Vars.Q.IsReady())
                {
                    if (Vars.DrawingsMenu["q"] != null && Vars.DrawingsMenu["q"].Cast<CheckBox>().CurrentValue)
                    {
                        Render.Circle.DrawCircle(GameObjects.Player.Position, Vars.Q.Range, Color.Green, 1);
                    }

                    if (Vars.DrawingsMenu["qe"] != null && Vars.DrawingsMenu["qe"].Cast<CheckBox>().CurrentValue)
                    {
                        Render.Circle.DrawCircle(GameObjects.Player.Position, Vars.Q2.Range, Color.LightGreen, 1);
                    }
                }

                /// <summary>
                ///     Loads the W drawing.
                /// </summary>
                if (Vars.W != null &&
                    Vars.W.IsReady() &&
                    Vars.DrawingsMenu["w"] != null &&
                    Vars.DrawingsMenu["w"].Cast<CheckBox>().CurrentValue)
                {
                    Render.Circle.DrawCircle(GameObjects.Player.Position, Vars.W.Range, Color.Purple, 1);
                }

                /// <summary>
                ///     Loads the E drawing.
                /// </summary>
                if (Vars.E != null &&
                    Vars.E.IsReady() &&
                    Vars.DrawingsMenu["e"] != null &&
                    Vars.DrawingsMenu["e"].Cast<CheckBox>().CurrentValue)
                {
                    Render.Circle.DrawCircle(GameObjects.Player.Position, Vars.E.Range, Color.Cyan, 1);
                }

                /// <summary>
                ///     Loads the R drawing.
                /// </summary>
                if (Vars.R != null &&
                    Vars.R.IsReady() &&
                    Vars.DrawingsMenu["r"] != null &&
                    Vars.DrawingsMenu["r"].Cast<CheckBox>().CurrentValue)
                {
                    Render.Circle.DrawCircle(GameObjects.Player.Position, Vars.R.Range, Color.Red, 1);
                }
            };
        }
    }
}