using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;

namespace Jhin___The_Virtuoso.Plugins
{
    internal class Compare
    {
        /// <summary>
        ///     Marksman Array for Compare
        /// </summary>
        public static string[] MarksmanStrings = {"Graves"};

        public static void Compares()
        {
            var screenpos = Game.CursorPos.To2D().To3D2();
            Drawing.DrawText(screenpos.X, screenpos.Y, Color.Gold, "Deneme");
        }
    }
}