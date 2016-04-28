using System.Drawing;
using EloBuddy;
using LeagueSharp.Common;

namespace ExorAIO.Utilities
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
                ///     Loads the Q spots drawing,
                ///     Loads the Extended Q drawing.
                /// </summary>
                if (Variables.Q != null && Variables.Q.IsReady())
                {
                    if (Variables.DrawingsMenu["drawings.q"] != null)
                    //Variables.DrawingsMenu["drawings.q"]
                    {
                        if (Variables.getCheckBoxItem(Variables.DrawingsMenu, "drawings.q"))
                        {
                            Render.Circle.DrawCircle(ObjectManager.Player.Position, Variables.Q.Range, Color.Green, 1);
                        }
                    }
                }

                /// <summary>
                ///     Loads the W drawing.
                /// </summary>
                /// 
                if (Variables.DrawingsMenu["drawings.w"] != null)
                {
                    if (Variables.W != null && Variables.W.IsReady() &&
                        Variables.getCheckBoxItem(Variables.DrawingsMenu, "drawings.w"))
                    {
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, Variables.W.Range, Color.Purple, 1);
                    }
                }

                /// <summary>
                ///     Loads the E drawing.
                /// </summary>
                /// 
                if (Variables.DrawingsMenu["drawings.e"] != null)
                {
                    if (Variables.E != null && Variables.E.IsReady() &&
                        Variables.getCheckBoxItem(Variables.DrawingsMenu, "drawings.e"))
                    {
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, Variables.E.Range, Color.Cyan, 1);
                    }
                }

                /// <summary>
                ///     Loads the R drawing.
                /// </summary>
                /// i
                if (Variables.DrawingsMenu["drawings.r"] != null)
                {
                    if (Variables.R != null && Variables.R.IsReady() &&
                        Variables.getCheckBoxItem(Variables.DrawingsMenu, "drawings.r"))
                    {
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, Variables.R.Range, Color.Red, 1);
                    }
                }
            };
        }
    }
}