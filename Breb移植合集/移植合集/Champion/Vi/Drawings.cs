using System;
using System.Drawing;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;

namespace ElVi
{
    internal static class Drawings
    {
        public static Menu
            _menu = ElViMenu._menu,
            cMenu = ElViMenu.cMenu,
            hMenu = ElViMenu.hMenu,
            rMenu = ElViMenu.rMenu,
            clearMenu = ElViMenu.clearMenu,
            miscMenu = ElViMenu.miscMenu;

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
            var drawOff = getCheckBoxItem(miscMenu, "ElVi.Draw.off");
            var drawQ = getCheckBoxItem(miscMenu, "ElVi.Draw.Q");
            var drawE = getCheckBoxItem(miscMenu, "ElVi.Draw.E");
            var drawR = getCheckBoxItem(miscMenu, "ElVi.Draw.R");

            if (drawOff)
                return;

            if (drawQ)
                if (Vi.Spells[Spells.Q].Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Vi.Spells[Spells.Q].Range, Color.White);

            if (drawE)
                if (Vi.Spells[Spells.E].Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Vi.Spells[Spells.E].Range, Color.White);

            if (drawR)
                if (Vi.Spells[Spells.R].Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Vi.Spells[Spells.R].Range, Color.White);

            var target = TargetSelector.GetTarget(Vi.Spells[Spells.Q].Range, DamageType.Physical);
            if (target != null)
            {
                Render.Circle.DrawCircle(target.Position, 50, Color.Yellow);
            }
        }
    }
}