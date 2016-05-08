using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nechrito_Nidalee.Handlers;
using Nechrito_Nidalee.Extras;
using EloBuddy;

namespace Nechrito_Nidalee.Drawings
{
    class DRAWING : Core
    {
       
        public static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead)
                return;
            var heropos = Drawing.WorldToScreen(ObjectManager.Player.Position);
            var IsWallDash = FleeLogic.IsWallDash(Player.ServerPosition, Champion.Pounce.Range);
            var end = Player.ServerPosition.LSExtend(Game.CursorPos, Champion.Pounce.Range);
            var WallPoint = FleeLogic.GetFirstWallPoint(Player.ServerPosition, end);

            if (MenuConfig.fleeDraw)
            {
               if(IsWallDash)
                {
                    if(WallPoint.LSDistance(Player.ServerPosition) <= 1200)
                    {
                        Render.Circle.DrawCircle(WallPoint, 60, System.Drawing.Color.White);
                    }
                }
            }
            if (MenuConfig.EngageDraw)
                {
                    Render.Circle.DrawCircle(Player.Position, 1500,
                       Champion.Javelin.IsReady() ? System.Drawing.Color.FromArgb(120, 0, 170, 255) : System.Drawing.Color.IndianRed);
                }
            }
        }
       
    }
