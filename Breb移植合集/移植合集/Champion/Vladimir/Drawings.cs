using System;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace ElVladimirReborn
{
    internal class Drawings
    {
        #region Public Methods and Operators

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        public static void OnDraw(EventArgs args)
        {
            var drawOff = getCheckBoxItem(ElVladimirMenu.miscMenu, "ElVladimir.Draw.off");
            var drawQ = getCheckBoxItem(ElVladimirMenu.miscMenu, "ElVladimir.Draw.Q");
            var drawW = getCheckBoxItem(ElVladimirMenu.miscMenu, "ElVladimir.Draw.W");
            var drawE = getCheckBoxItem(ElVladimirMenu.miscMenu, "ElVladimir.Draw.E");
            var drawR = getCheckBoxItem(ElVladimirMenu.miscMenu, "ElVladimir.Draw.R");

            if (drawOff)
            {
                return;
            }

            if (drawQ)
            {
                if (Vladimir.spells[Spells.Q].Level > 0)
                {
                    Render.Circle.DrawCircle(
                        ObjectManager.Player.Position,
                        Vladimir.spells[Spells.Q].Range,
                        Color.White);
                }
            }

            if (drawE)
            {
                if (Vladimir.spells[Spells.E].Level > 0)
                {
                    Render.Circle.DrawCircle(
                        ObjectManager.Player.Position,
                        Vladimir.spells[Spells.E].Range,
                        Color.White);
                }
            }

            if (drawW)
            {
                if (Vladimir.spells[Spells.W].Level > 0)
                {
                    Render.Circle.DrawCircle(
                        ObjectManager.Player.Position,
                        Vladimir.spells[Spells.W].Range,
                        Color.White);
                }
            }

            if (drawR)
            {
                if (Vladimir.spells[Spells.R].Level > 0)
                {
                    Render.Circle.DrawCircle(
                        ObjectManager.Player.Position,
                        Vladimir.spells[Spells.R].Range,
                        Color.White);
                }
            }

            //if (drawText)
            //Drawing.DrawText(playerPos.X - 70, playerPos.Y + 40, (rBool ? Color.Green : Color.Red), "{0}", (rBool ? "Auto harass Enabled" : "Auto harass Disabled"));
        }

        #endregion
    }
}