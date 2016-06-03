#region

using System;
using EloBuddy;
using LeagueSharp.SDK.Core.Utils;
using Spirit_Karma.Core;
using Spirit_Karma.Menus;

#endregion

namespace Spirit_Karma.Draw 
{
    internal class DrawMantra : Core.Core
    {

        public static void SelectedMantra(EventArgs args)
        {
            if(Player.IsDead || !MenuConfig.getCheckBoxItem(MenuConfig.drawMenu, "MantraDraw") || !MenuConfig.getCheckBoxItem(MenuConfig.drawMenu, "UseDrawings")) return;
            var heropos = Drawing.WorldToScreen(ObjectManager.Player.Position);

            if (MenuConfig.getCheckBoxItem(MenuConfig.drawMenu, "QRange") && MenuConfig.getBoxItem(MenuConfig.comboMenu, "MantraMode") == 0)
            {
                Render.Circle.DrawCircle(Player.Position, Spells.Q.Range,
                   Spells.R.IsReady() ? System.Drawing.Color.FromArgb(120, 0, 170, 255) : System.Drawing.Color.IndianRed);
            }

            if (MenuConfig.getCheckBoxItem(MenuConfig.drawMenu, "QRange") && MenuConfig.getBoxItem(MenuConfig.comboMenu, "MantraMode") == 1)
            {
                Render.Circle.DrawCircle(Player.Position, Spells.W.Range,
                   Spells.R.IsReady() ? System.Drawing.Color.FromArgb(120, 0, 170, 255) : System.Drawing.Color.IndianRed);
            }

            if (MenuConfig.getCheckBoxItem(MenuConfig.drawMenu, "QRange") && MenuConfig.getBoxItem(MenuConfig.comboMenu, "MantraMode") == 2)
            {
                Render.Circle.DrawCircle(Player.Position, Spells.E.Range,
                   Spells.R.IsReady() ? System.Drawing.Color.FromArgb(120, 0, 170, 255) : System.Drawing.Color.IndianRed);
            }

            if (MenuConfig.getCheckBoxItem(MenuConfig.drawMenu, "QRange") && MenuConfig.getBoxItem(MenuConfig.comboMenu, "MantraMode") == 3)
            {
                Render.Circle.DrawCircle(Player.Position, Spells.Q.Range,
                   Spells.R.IsReady() ? System.Drawing.Color.FromArgb(120, 0, 170, 255) : System.Drawing.Color.IndianRed);
            }

            if (MenuConfig.getBoxItem(MenuConfig.comboMenu, "MantraMode") == 0)
            {
                Drawing.DrawText(heropos.X - 15, heropos.Y + 40, System.Drawing.Color.White, "Selected Prio: Q");
            }
            if (MenuConfig.getBoxItem(MenuConfig.comboMenu, "MantraMode") == 1)
            {
                Drawing.DrawText(heropos.X - 15, heropos.Y + 40, System.Drawing.Color.White, "Selected Prio: W");
            }
            if (MenuConfig.getBoxItem(MenuConfig.comboMenu, "MantraMode") == 2)
            {
                Drawing.DrawText(heropos.X - 15, heropos.Y + 40, System.Drawing.Color.White, "Selected Prio: E");
            }
            if (MenuConfig.getBoxItem(MenuConfig.comboMenu, "MantraMode") == 3)
            {
                Drawing.DrawText(heropos.X - 15, heropos.Y + 40, System.Drawing.Color.White, "Selected Prio: Auto");
            }
        }
    }
}
