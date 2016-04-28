using System;
using System.Drawing;
using EloBuddy;
using LeagueSharp.Common;

namespace LCS_Lucian
{
    internal class LucianDrawing
    {
        public static void Init()
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }
            if (Program.getCheckBoxItem(Program.drawMenu, "lucian.q.draw") && LucianSpells.Q.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, LucianSpells.Q.Range, Color.Gold);
            }
            if (Program.getCheckBoxItem(Program.drawMenu, "lucian.q2.draw") && LucianSpells.Q2.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, LucianSpells.Q2.Range, Color.Gold);
            }
            if (Program.getCheckBoxItem(Program.drawMenu, "lucian.w.draw") && LucianSpells.W.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, LucianSpells.W.Range, Color.Gold);
            }
            if (Program.getCheckBoxItem(Program.drawMenu, "lucian.e.draw") && LucianSpells.E.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, LucianSpells.E.Range, Color.Gold);
            }
            if (Program.getCheckBoxItem(Program.drawMenu, "lucian.r.draw") && LucianSpells.R.IsReady())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, LucianSpells.R.Range, Color.Gold);
            }
        }
    }
}