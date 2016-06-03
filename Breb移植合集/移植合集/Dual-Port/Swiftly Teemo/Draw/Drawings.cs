#region

using System;
using LeagueSharp.SDK.Core.Utils;
using Swiftly_Teemo.Main;
using LeagueSharp.SDK;

#endregion

namespace Swiftly_Teemo.Draw
{
    internal class Drawings : Core
    {
        public static HpBarDraw DrawHpBar = new HpBarDraw();
        public static void OnDraw(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }
            
            if (MenuConfig.EngageDraw)
            {
                Render.Circle.DrawCircle(Player.Position, Spells.Q.Range,
                   Spells.Q.IsReady() ? System.Drawing.Color.FromArgb(120, 0, 170, 255) : System.Drawing.Color.IndianRed);
            }
        }
    }
}
