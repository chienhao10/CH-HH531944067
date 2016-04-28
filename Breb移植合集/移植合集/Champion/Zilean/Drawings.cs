using System;
using System.Drawing;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;

namespace ElZilean
{
    public class Drawings
    {
        #region Public Methods and Operators

        public static Menu
            comboMenu = ZileanMenu.comboMenu,
            harassMenu = ZileanMenu.harassMenu,
            clearMenu = ZileanMenu.clearMenu,
            castUltMenu = ZileanMenu.castUltMenu,
            miscMenu = ZileanMenu.miscMenu;

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

        public static void Drawing_OnDraw(EventArgs args)
        {
            var drawOff = getCheckBoxItem(miscMenu, "ElZilean.Draw.off");
            var drawQ = getCheckBoxItem(miscMenu, "ElZilean.Draw.Q");
            var drawW = getCheckBoxItem(miscMenu, "ElZilean.Draw.W");
            var drawE = getCheckBoxItem(miscMenu, "ElZilean.Draw.E");
            var drawR = getCheckBoxItem(miscMenu, "ElZilean.Draw.R");

            if (drawOff)
            {
                return;
            }

            if (drawQ)
            {
                if (Zilean.spells[Spells.Q].Level > 0)
                {
                    Render.Circle.DrawCircle(
                        ObjectManager.Player.Position,
                        Zilean.spells[Spells.Q].Range,
                        Zilean.spells[Spells.Q].IsReady() ? Color.Green : Color.Red);
                }
            }

            if (drawW)
            {
                if (Zilean.spells[Spells.W].Level > 0)
                {
                    Render.Circle.DrawCircle(
                        ObjectManager.Player.Position,
                        Zilean.spells[Spells.W].Range,
                        Zilean.spells[Spells.W].IsReady() ? Color.Green : Color.Red);
                }
            }

            if (drawE)
            {
                if (Zilean.spells[Spells.E].Level > 0)
                {
                    Render.Circle.DrawCircle(
                        ObjectManager.Player.Position,
                        Zilean.spells[Spells.E].Range,
                        Zilean.spells[Spells.E].IsReady() ? Color.Green : Color.Red);
                }
            }

            if (drawR)
            {
                if (Zilean.spells[Spells.R].Level > 0)
                {
                    Render.Circle.DrawCircle(
                        ObjectManager.Player.Position,
                        Zilean.spells[Spells.R].Range,
                        Zilean.spells[Spells.R].IsReady() ? Color.Green : Color.Red);
                }
            }
        }

        #endregion
    }
}