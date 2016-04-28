using System.Drawing;
using EloBuddy;
using LeagueSharp.Common;

namespace UnderratedAIO.Helpers
{
    public class DrawHelper
    {
        public static AIHeroClient player = ObjectManager.Player;

        public static void DrawCircle(bool circle, float spellRange, Color c)
        {
            if (circle)
            {
                Render.Circle.DrawCircle(player.Position, spellRange, c);
            }
        }
    }
}