using System;
using System.Drawing;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;

namespace ElSinged
{
    internal class Drawings
    {
        public static Menu
            Menu = ElSingedMenu.Menu,
            cMenu = ElSingedMenu.cMenu,
            hMenu = ElSingedMenu.hMenu,
            lcMenu = ElSingedMenu.lcMenu,
            miscMenu = ElSingedMenu.miscMenu;

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
            var drawOff = getCheckBoxItem(miscMenu, "ElSinged.Draw.off");
            var drawQ = getCheckBoxItem(miscMenu, "ElSinged.Draw.Q");
            var drawW = getCheckBoxItem(miscMenu, "ElSinged.Draw.W");
            var drawE = getCheckBoxItem(miscMenu, "ElSinged.Draw.E");

            if (drawOff)
                return;

            if (drawQ)
                if (Singed.spells[Spells.Q].Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Singed.spells[Spells.Q].Range,
                        Singed.spells[Spells.Q].IsReady() ? Color.Green : Color.Red);

            if (drawW)
                if (Singed.spells[Spells.W].Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Singed.spells[Spells.W].Range,
                        Singed.spells[Spells.W].IsReady() ? Color.Green : Color.Red);

            if (drawE)
                if (Singed.spells[Spells.E].Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Singed.spells[Spells.E].Range,
                        Singed.spells[Spells.E].IsReady() ? Color.Green : Color.Red);
        }
    }
}