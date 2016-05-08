namespace ElXerath
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Color = System.Drawing.Color;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Menu;
    using EloBuddy;
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

        public static void Drawing_OnDraw(EventArgs args)
        {
            var drawOff = getCheckBoxItem(ElXerathMenu.miscMenu, "ElXerath.Draw.off");
            var drawQ = getCheckBoxItem(ElXerathMenu.miscMenu,"ElXerath.Draw.Q");
            var drawW = getCheckBoxItem(ElXerathMenu.miscMenu,"ElXerath.Draw.W");
            var drawE = getCheckBoxItem(ElXerathMenu.miscMenu,"ElXerath.Draw.E");
            var drawR = getCheckBoxItem(ElXerathMenu.miscMenu,"ElXerath.Draw.R");
            var drawText = getCheckBoxItem(ElXerathMenu.miscMenu,"ElXerath.Draw.Text");

            var rBool = getKeyBindItem(ElXerathMenu.hMenu,"ElXerath.AutoHarass");

            if (drawOff)
            {
                return;
            }

            var playerPos = Drawing.WorldToScreen(ObjectManager.Player.Position);

            if (drawQ)
            {
                if (Xerath.spells[Spells.Q].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Xerath.spells[Spells.Q].Range, Color.White);
                }
            }

            if (drawW)
            {
                if (Xerath.spells[Spells.W].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Xerath.spells[Spells.W].Range, Color.White);
                }
            }

            if (drawE)
            {
                if (Xerath.spells[Spells.E].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Xerath.spells[Spells.E].Range, Color.White);
                }
            }

            if (drawR)
            {
                if (Xerath.spells[Spells.R].Level > 0)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Xerath.spells[Spells.R].Range, Color.White);
                }
            }

            if (drawText)
            {
                Drawing.DrawText(
                    playerPos.X - 70,
                    playerPos.Y + 40,
                    (rBool ? Color.Green : Color.Red),
                    (rBool ? "自动骚扰 开启" : "自动骚扰 关闭"));
            }
        }

        #endregion
    }
}