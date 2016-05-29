using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Slutty_ryze
{
    class DrawManager
    {
        #region Variable Declaration
        private const int XOffset = 10;
        private const int YOffset = 20;
        private const int Width = 103;
        private const int Height = 8;
        private static readonly Render.Text Text = new Render.Text(0, 0, "", 14, Color.Red, "monospace");
        private static readonly System.Drawing.Color _color = System.Drawing.Color.Lime;
        private static readonly System.Drawing.Color _fillColor = System.Drawing.Color.Goldenrod;
        private static readonly System.Drawing.Color _colorblind = System.Drawing.Color.LightBlue;
        private static readonly System.Drawing.Color _fillColorblind = System.Drawing.Color.Teal;
        #endregion
        #region Private Fuctions
        private static System.Drawing.Color GetColor(bool b)
        {
            return b ? System.Drawing.Color.DarkGreen : System.Drawing.Color.Red;
        }

        private static string BoolToString(bool b)
        {
            return b ? "ON" : "OFF";
        }

        private static System.Drawing.Color GetColorblind(bool c)
        {
            return c ? System.Drawing.Color.Teal : System.Drawing.Color.Magenta;
        }

        private static string BoolToStringblind(bool c)
        {
            return c ? "ON" : "OFF";
        }

        private static void DrawKeys(Vector2 pos)
        {
            switch (getBoxItem(MenuManager.drawMenu, "drawoptions"))
            {
                case 0:
                    Drawing.DrawLine(new Vector2(pos.X - 25, pos.Y + 20), new Vector2(pos.X + 150, pos.Y + 20), 2,
                        System.Drawing.Color.SteelBlue);
                    var col = 0;
                    Drawing.DrawText(pos.X, pos.Y, System.Drawing.Color.SteelBlue, "Key Table");

                    Drawing.DrawText(pos.X, ++col * 25 + pos.Y, System.Drawing.Color.SteelBlue, "Stack Tear Key: " + MenuManager.itemMenu["tearS"].Cast<KeyBind>().KeyStrings.Item1, 13);
                    Drawing.DrawText(pos.X, ++col * 25 + pos.Y, System.Drawing.Color.SteelBlue, "Auto Passive Key: " + MenuManager.passiveMenu["autoPassive"].Cast<KeyBind>().KeyStrings.Item1, 13);
                    Drawing.DrawText(pos.X, ++col * 25 + pos.Y, System.Drawing.Color.SteelBlue, "Disable Lane Clear Key: " + MenuManager.laneMenu["disablelane"].Cast<KeyBind>().KeyStrings.Item1, 13);

                    Drawing.DrawLine(new Vector2(pos.X - 25, ++col * 25 + pos.Y), new Vector2(pos.X + 150, col * 25 + pos.Y),
                        2, System.Drawing.Color.SteelBlue);
                    break;
                case 1:
                    Drawing.DrawLine(new Vector2(pos.X - 25, pos.Y + 20), new Vector2(pos.X + 150, pos.Y + 20), 2,
                        System.Drawing.Color.LightBlue);

                    var col1 = 0;
                    Drawing.DrawText(pos.X, pos.Y, System.Drawing.Color.LightBlue, "Key Table");

                    Drawing.DrawText(pos.X, ++col1 * 25 + pos.Y, System.Drawing.Color.LightBlue, "Stack Tear Key: " + MenuManager.itemMenu["tearS"].Cast<KeyBind>().KeyStrings.Item1, 13);
                    Drawing.DrawText(pos.X, ++col1 * 25 + pos.Y, System.Drawing.Color.LightBlue, "Auto Passive Key: " + MenuManager.passiveMenu["autoPassive"].Cast<KeyBind>().KeyStrings.Item1, 13);
                    Drawing.DrawText(pos.X, ++col1 * 25 + pos.Y, System.Drawing.Color.LightBlue, "Disable Lane Clear Key: " + MenuManager.laneMenu["disablelane"].Cast<KeyBind>().KeyStrings.Item1, 13);

                    Drawing.DrawLine(new Vector2(pos.X - 25, ++col1 * 25 + pos.Y), new Vector2(pos.X + 150, col1 * 25 + pos.Y),
                        2, System.Drawing.Color.LightBlue);
                    break;
            }
        }

        /*
         static Point[] getPoints(Vector2 c, int R)
         {
             int X = (int)c.X;
             int Y = (int)c.Y;

             int R2 = (int)(R / Math.PI);

             Point[] pt = new Point[5];

             pt[0].X = X;
             pt[0].Y = Y;

             pt[1].X = X;
             pt[1].Y = Y + R * 2;

             pt[2].X = X + R * 2;
             pt[2].Y = Y;

             pt[3].X = X - R * 2;
             pt[3].Y = Y;

             pt[4].X = X;
             pt[4].Y = Y - R * 2;

             return pt;
         }

         private static void drawCircleThing(int radius, SharpDX.Vector2 center, Color c)
         {
             var lineWalls = getPoints(center,radius);
             Drawing.DrawLine(lineWalls[0], lineWalls[1], 2, c);
             Drawing.DrawLine(lineWalls[0], lineWalls[2], 2, c);
             Drawing.DrawLine(lineWalls[0], lineWalls[3], 2, c);
             Drawing.DrawLine(lineWalls[0], lineWalls[4], 2, c);
         }
         */
        #endregion
        #region Public Functions
        public static void Drawing_OnDraw(EventArgs args)
        {
            if (GlobalManager.GetHero.IsDead)
                return;
            if (!getCheckBoxItem(MenuManager.drawMenu, "Draw"))
                return;

            if (getCheckBoxItem(MenuManager.drawMenu, "keyBindDisplay"))
                DrawKeys(new Vector2(Drawing.Width - 250, (float)Drawing.Height / 2));

            if (!GlobalManager.GetHero.Position.LSIsOnScreen())
                return;
            if (getCheckBoxItem(MenuManager.drawMenu, "stackDraw"))
            {
                Drawing.DrawText(Drawing.WorldToScreen(GlobalManager.GetHero.Position).X - 55,
                    Drawing.WorldToScreen(GlobalManager.GetHero.Position).Y, System.Drawing.Color.Cyan,
                    "Current Stacks " + GlobalManager.GetPassiveBuff);
            }
            // drawCircleThing((int)Champion.Q.Range/2, Drawing.WorldToScreen(GlobalManager.GetHero.Position), Color.Pink);
            var tears = getKeyBindItem(MenuManager.itemMenu, "tearS");
            var passive = getKeyBindItem(MenuManager.passiveMenu, "autoPassive");
            var laneclear = getKeyBindItem(MenuManager.laneMenu, "disablelane");

            var heroPosition = Drawing.WorldToScreen(GlobalManager.GetHero.Position);
            var textDimension = 50;

            switch (getBoxItem(MenuManager.drawMenu, "drawoptions"))
            {
                case 0:
                    if (getCheckBoxItem(MenuManager.drawMenu, "qDraw") && Champion.Q.Level > 0)
                        Render.Circle.DrawCircle(GlobalManager.GetHero.Position, Champion.Q.Range, System.Drawing.Color.Green, 3);
                    if (getCheckBoxItem(MenuManager.drawMenu, "eDraw") && Champion.E.Level > 0)
                        Render.Circle.DrawCircle(GlobalManager.GetHero.Position, Champion.E.Range, System.Drawing.Color.Gold, 3);
                    if (getCheckBoxItem(MenuManager.drawMenu, "wDraw") && Champion.W.Level > 0)
                        Render.Circle.DrawCircle(GlobalManager.GetHero.Position, Champion.W.Range, System.Drawing.Color.Blue, 3);

                    if (!getCheckBoxItem(MenuManager.drawMenu, "notdraw")) return;

                    Drawing.DrawText(heroPosition.X - textDimension, heroPosition.Y - textDimension,
                        GetColor(tears),
                        "Tear Stack: " + BoolToString(tears));

                    Drawing.DrawText(heroPosition.X - 150, heroPosition.Y - 30, GetColor(passive),
                        "Passive Stack: " + BoolToString(passive));

                    Drawing.DrawText(heroPosition.X + 20, heroPosition.Y - 30, GetColor(laneclear),
                        "Lane Clear: " + BoolToString(laneclear));
                    break;
                case 1:
                    if (getCheckBoxItem(MenuManager.drawMenu, "qDraw") && Champion.Q.Level > 0)
                        Render.Circle.DrawCircle(GlobalManager.GetHero.Position, Champion.Q.Range, System.Drawing.Color.Teal, 3);
                    if (getCheckBoxItem(MenuManager.drawMenu, "eDraw") && Champion.E.Level > 0)
                        Render.Circle.DrawCircle(GlobalManager.GetHero.Position, Champion.E.Range, System.Drawing.Color.Magenta, 3);
                    if (getCheckBoxItem(MenuManager.drawMenu, "wDraw") && Champion.W.Level > 0)
                        Render.Circle.DrawCircle(GlobalManager.GetHero.Position, Champion.W.Range, System.Drawing.Color.Black, 3);

                    if (!getCheckBoxItem(MenuManager.drawMenu, "notdraw")) return;

                    Drawing.DrawText(heroPosition.X - textDimension, heroPosition.Y - textDimension,
                        GetColorblind(tears),
                        "Tear Stack: " + BoolToStringblind(tears));

                    Drawing.DrawText(heroPosition.X - 150, heroPosition.Y - 30, GetColorblind(passive),
                        "Passive Stack: " + BoolToStringblind(passive));

                    Drawing.DrawText(heroPosition.X + 20, heroPosition.Y - 30, GetColorblind(laneclear),
                        "Lane Clear: " + BoolToStringblind(laneclear));
                    break;
            }
        }

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

        public static void Drawing_OnDrawChamp(EventArgs args)
        {
            if (!GlobalManager.EnableDrawingDamage || GlobalManager.DamageToUnit == null)
                return;


            foreach (var unit in HeroManager.Enemies.Where(h => h.IsValid && h.IsHPBarRendered))
            {
                var barPos = unit.HPBarPosition;
                var damage = GlobalManager.DamageToUnit(unit);
                var percentHealthAfterDamage = Math.Max(0, unit.Health - damage) / unit.MaxHealth;
                var yPos = barPos.Y + YOffset;
                var xPosDamage = barPos.X + XOffset + Width * percentHealthAfterDamage;
                var xPosCurrentHp = barPos.X + XOffset + Width * unit.Health / unit.MaxHealth;

                if (damage > unit.Health)
                {
                    Text.X = (int)barPos.X + XOffset;
                    Text.Y = (int)barPos.Y + YOffset - 13;
                    Text.text = "Killable With Combo Rotation " + (unit.Health - damage);
                    Text.OnEndScene();
                }
                switch (getBoxItem(MenuManager.drawMenu, "drawoptions"))
                {
                    case 0:
                        Drawing.DrawLine(xPosDamage, yPos, xPosDamage, yPos + Height, 1, _color);

                        if (!GlobalManager.EnableFillDamage) continue;
                        var differenceInHp = xPosCurrentHp - xPosDamage;
                        var pos1 = barPos.X + 9 + (107 * percentHealthAfterDamage);
                        for (var i = 0; i < differenceInHp; i++)
                        {
                            Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, _fillColor);
                        }
                        break;
                    case 1:
                        Drawing.DrawLine(xPosDamage, yPos, xPosDamage, yPos + Height, 1, _colorblind);

                        if (!GlobalManager.EnableFillDamage) continue;
                        var differenceInHp1 = xPosCurrentHp - xPosDamage;
                        var pos11 = barPos.X + 9 + (107 * percentHealthAfterDamage);
                        for (var i = 0; i < differenceInHp1; i++)
                        {
                            Drawing.DrawLine(pos11 + i, yPos, pos11 + i, yPos + Height, 1, _fillColorblind);
                        }
                        break;
                }
            }
        }
        #endregion
    }
}
