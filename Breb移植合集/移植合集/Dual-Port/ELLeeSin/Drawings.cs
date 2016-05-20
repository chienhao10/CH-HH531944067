using System;
using System.Drawing;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;

namespace ElLeeSin
{
    public class Drawings
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
            var newTarget = getCheckBoxItem(InitMenu.insecMenu, "insecMode")
                ? TargetSelector.SelectedTarget
                : TargetSelector.GetTarget(Program.spells[Program.Spells.Q].Range + 200, DamageType.Physical);


            if (Program.ClicksecEnabled)
            {
                Render.Circle.DrawCircle(Program.InsecClickPos, 100, Color.DeepSkyBlue);
            }

            var playerPos = Drawing.WorldToScreen(ObjectManager.Player.Position);
            if (getCheckBoxItem(InitMenu.drawMenu, "ElLeeSin.Draw.Insec.Text"))
            {
                Drawing.DrawText(playerPos.X, playerPos.Y + 40, Color.White, "Flash Insec enabled");
            }

            if (getCheckBoxItem(InitMenu.drawMenu, "Draw.Insec.Lines")) //&& Program.spells[Program.Spells.R].IsReady()
            {
                if (newTarget != null && newTarget.IsVisible && newTarget.IsValidTarget() && !newTarget.IsDead &&
                    Program.Player.LSDistance(newTarget) < 3000)
                {
                    var targetPos = Drawing.WorldToScreen(newTarget.Position);
                    Drawing.DrawLine(
                        Program.InsecLinePos.X,
                        Program.InsecLinePos.Y,
                        targetPos.X,
                        targetPos.Y,
                        3,
                        Color.Gold);

                    Drawing.DrawText(
                        Drawing.WorldToScreen(newTarget.Position).X - 40,
                        Drawing.WorldToScreen(newTarget.Position).Y + 10,
                        Color.White,
                        "Selected Target");

                    Drawing.DrawCircle(Program.GetInsecPos(newTarget), 100, Color.DeepSkyBlue);
                }
            }

            if (!getCheckBoxItem(InitMenu.drawMenu, "DrawEnabled"))
            {
                return;
            }

            foreach (var t in ObjectManager.Get<AIHeroClient>())
            {
                if (t.HasBuff("BlindMonkQOne") || t.HasBuff("blindmonkqonechaos"))
                {
                    Drawing.DrawCircle(t.Position, 200, Color.Red);
                }
            }

            if (getKeyBindItem(InitMenu.wardjumpMenu, "ElLeeSin.Wardjump")
                && getCheckBoxItem(InitMenu.drawMenu, "ElLeeSin.Draw.WJDraw"))
            {
                Render.Circle.DrawCircle(Program.JumpPos.To3D(), 20, Color.Red);
                Render.Circle.DrawCircle(Program.Player.Position, 600, Color.Red);
            }
            if (getCheckBoxItem(InitMenu.drawMenu, "ElLeeSin.Draw.Q"))
            {
                Render.Circle.DrawCircle(
                    Program.Player.Position,
                    Program.spells[Program.Spells.Q].Range - 80,
                    Program.spells[Program.Spells.Q].IsReady() ? Color.LightSkyBlue : Color.Tomato);
            }
            if (getCheckBoxItem(InitMenu.drawMenu, "ElLeeSin.Draw.W"))
            {
                Render.Circle.DrawCircle(
                    Program.Player.Position,
                    Program.spells[Program.Spells.W].Range - 80,
                    Program.spells[Program.Spells.W].IsReady() ? Color.LightSkyBlue : Color.Tomato);
            }
            if (getCheckBoxItem(InitMenu.drawMenu, "ElLeeSin.Draw.E"))
            {
                Render.Circle.DrawCircle(
                    Program.Player.Position,
                    Program.spells[Program.Spells.E].Range - 80,
                    Program.spells[Program.Spells.E].IsReady() ? Color.LightSkyBlue : Color.Tomato);
            }
            if (getCheckBoxItem(InitMenu.drawMenu, "ElLeeSin.Draw.R"))
            {
                Render.Circle.DrawCircle(
                    Program.Player.Position,
                    Program.spells[Program.Spells.R].Range - 80,
                    Program.spells[Program.Spells.R].IsReady() ? Color.LightSkyBlue : Color.Tomato);
            }
        }

        #endregion
    }
}