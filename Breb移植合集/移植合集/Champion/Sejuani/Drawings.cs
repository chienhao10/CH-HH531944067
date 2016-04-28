using System;
using System.Drawing;
using EloBuddy;
using LeagueSharp.Common;

namespace ElSejuani
{
    internal class Drawings
    {
        public static void OnDraw(EventArgs args)
        {
            var drawOff = ElSejuaniMenu.getCheckBoxItem(ElSejuaniMenu.miscMenu, "ElSejuani.Draw.off");
            var drawQ = ElSejuaniMenu.getCheckBoxItem(ElSejuaniMenu.miscMenu, "ElSejuani.Draw.Q");
            var drawW = ElSejuaniMenu.getCheckBoxItem(ElSejuaniMenu.miscMenu, "ElSejuani.Draw.W");
            var drawE = ElSejuaniMenu.getCheckBoxItem(ElSejuaniMenu.miscMenu, "ElSejuani.Draw.E");
            var drawR = ElSejuaniMenu.getCheckBoxItem(ElSejuaniMenu.miscMenu, "ElSejuani.Draw.R");


            if (drawOff)
                return;

            if (drawQ)
                if (Sejuani.spells[Spells.Q].Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Sejuani.spells[Spells.Q].Range, Color.White);

            if (drawW)
                if (Sejuani.spells[Spells.W].Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Sejuani.spells[Spells.W].Range, Color.White);

            if (drawE)
                if (Sejuani.spells[Spells.E].Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Sejuani.spells[Spells.E].Range, Color.White);

            if (drawR)
                if (Sejuani.spells[Spells.R].Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Sejuani.spells[Spells.R].Range, Color.White);
        }
    }
}