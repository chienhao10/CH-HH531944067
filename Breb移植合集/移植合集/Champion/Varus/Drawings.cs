using System;
using System.Drawing;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;

namespace Elvarus
{
    internal class Drawings
    {
        #region Public Methods and Operators

        public static Menu
            Menu = ElVarusMenu.Menu,
            cMenu = ElVarusMenu.cMenu,
            hMenu = ElVarusMenu.hMenu,
            itemMenu = ElVarusMenu.itemMenu,
            lMenu = ElVarusMenu.lMenu,
            miscMenu = ElVarusMenu.miscMenu;

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
            var drawOff = getCheckBoxItem(miscMenu, "ElVarus.Draw.off");
            var drawQ = getCheckBoxItem(miscMenu, "ElVarus.Draw.Q");
            var drawW = getCheckBoxItem(miscMenu, "ElVarus.Draw.W");
            var drawE = getCheckBoxItem(miscMenu, "ElVarus.Draw.E");
            var drawR = getCheckBoxItem(miscMenu, "ElVarus.Draw.E");

            if (drawOff)
            {
                return;
            }

            if (drawQ)
            {
                if (Varus.spells[Spells.Q].Level > 0)
                {
                    Render.Circle.DrawCircle(
                        ObjectManager.Player.Position,
                        Varus.spells[Spells.Q].Range,
                        Varus.spells[Spells.Q].IsReady() ? Color.Green : Color.Red);
                }
            }

            if (drawW)
            {
                if (Varus.spells[Spells.W].Level > 0)
                {
                    Render.Circle.DrawCircle(
                        ObjectManager.Player.Position,
                        Varus.spells[Spells.W].Range,
                        Varus.spells[Spells.W].IsReady() ? Color.Green : Color.Red);
                }
            }

            if (drawE)
            {
                if (Varus.spells[Spells.E].Level > 0)
                {
                    Render.Circle.DrawCircle(
                        ObjectManager.Player.Position,
                        Varus.spells[Spells.E].Range,
                        Varus.spells[Spells.E].IsReady() ? Color.Green : Color.Red);
                }
            }

            if (drawR)
            {
                if (Varus.spells[Spells.R].Level > 0)
                {
                    Render.Circle.DrawCircle(
                        ObjectManager.Player.Position,
                        Varus.spells[Spells.R].Range,
                        Varus.spells[Spells.R].IsReady() ? Color.Green : Color.Red);
                }
            }
        }

        #endregion
    }
}