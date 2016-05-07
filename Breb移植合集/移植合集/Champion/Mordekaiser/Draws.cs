using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using LeagueSharp.Common;

namespace Mordekaiser
{
    internal class Draws
    {
        public Draws()
        {
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Utils.Player.Self.IsDead)
                return;

            DrawW();
            DrawE();
            DrawR();
            DrawGhost();
        }

        private static void DrawW()
        {
            if (!Menu.getCheckBoxItem(Menu.MenuW, "Allies.Active")) return;

            var drawSearch = Menu.getCheckBoxItem(Menu.MenuW, "DrawW.Search");
            if (drawSearch)
            {
                Render.Circle.DrawCircle(Utils.Player.Self.Position, Spells.W.Range, Color.Aqua, 1);
            }

            var dmgRadiusDraw = Menu.getCheckBoxItem(Menu.MenuW, "DrawW.DamageRadius");
            if (dmgRadiusDraw)
            {
                Render.Circle.DrawCircle(
                    Utils.Player.Self.Position,
                    Menu.getSliderItem(Menu.MenuW, "UseW.DamageRadius"),
                    Color.Coral);
            }
        }

        private static void DrawE()
        {
            var drawSearch = Menu.getCheckBoxItem(Menu.MenuE, "DrawE.Search");
            if (drawSearch)
            {
                Render.Circle.DrawCircle(Utils.Player.Self.Position, Spells.E.Range, Color.Aqua, 1);
            }
        }

        private static void DrawR()
        {
            if (!Menu.getCheckBoxItem(Menu.MenuR, "UseR.Active")) return;

            if (!Menu.getCheckBoxItem(Menu.MenuR, "DrawR.Search")) return;

            if (Menu.getBoxItem(Menu.MenuR, "DrawR.Status.Show") == 1)
            {
                foreach (var a in HeroManager.Enemies.Where(e => e.IsVisible && !e.IsDead && !e.IsZombie))
                {
                    var vSelected = Menu.getBoxItem(Menu.MenuR, "Selected" + a.NetworkId);

                    if (Menu.getBoxItem(Menu.MenuR, "DrawR.Status.Show") == 2 && vSelected != 3) continue;

                    if (vSelected != 0)
                        Utils.DrawText(
                            vSelected == 3 ? Utils.TextBold : Utils.Text,
                            "Use Ultimate: "
                            + Menu.getBoxItem(Menu.MenuR, "DrawR.Status.Show"),
                            a.HPBarPosition.X + a.BoundingRadius/2 - 20,
                            a.HPBarPosition.Y - 20,
                            vSelected == 3
                                ? SharpDX.Color.Red
                                : (vSelected == 2 ? SharpDX.Color.Yellow : SharpDX.Color.Gray));
                }
            }

            var drawSearch = Menu.getCheckBoxItem(Menu.MenuR, "DrawR.Search");
            if (drawSearch)
            {
                Render.Circle.DrawCircle(Utils.Player.Self.Position, Spells.R.Range, Color.GreenYellow, 1);
            }
        }

        private static void DrawGhost()
        {
            var ghost = Utils.HowToTrainYourDragon;
            if (ghost == null)
                return;

            if (Menu.getCheckBoxItem(Menu.MenuGhost, "Ghost.Draw.Position"))
            {
                Render.Circle.DrawCircle(ghost.Position, 105f, Color.DarkRed);
            }

            if (Menu.getCheckBoxItem(Menu.MenuGhost, "Ghost.Draw.AARange"))
            {
                Render.Circle.DrawCircle(ghost.Position, ghost.AttackRange, Color.DarkRed);
            }

            if (Menu.getCheckBoxItem(Menu.MenuGhost, "Ghost.Draw.ControlRange"))
            {
                Render.Circle.DrawCircle(ghost.Position, 1500f, Color.WhiteSmoke);
            }
        }
    }
}