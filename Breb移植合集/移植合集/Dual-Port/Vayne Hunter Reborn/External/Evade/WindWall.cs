using System;
using System.Linq;
using System.Text.RegularExpressions;
using EloBuddy;
using SharpDX;
using LeagueSharp.Common;

namespace VayneHunter_Reborn.External.Evade
{
    internal class WindWall
    {
        private static int _wallCastT;

        /// <summary>
        ///     The yasuo wind wall casted position.
        /// </summary>
        private static Vector2 _yasuoWallCastedPos;


        /// <summary>
        ///     Called when a spell cast is processed by the client.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">
        ///     The <see cref="LeagueSharp.GameObjectProcessSpellCastEventArgs" /> instance containing the event
        ///     data.
        /// </param>
        internal static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsValid && sender.Team != ObjectManager.Player.Team && args.SData.Name == "YasuoWMovingWall")
            {
                _wallCastT = Environment.TickCount;
                _yasuoWallCastedPos = sender.ServerPosition.LSTo2D();
            }
        }

        /// <summary>
        ///     Collideses the with wall.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns></returns>
        internal static bool CollidesWithWall(Vector3 start, Vector3 end)
        {
            if (Environment.TickCount - _wallCastT > 4000)
            {
                return false;
            }

            GameObject wall = null;
            foreach (var gameObject in
                ObjectManager.Get<GameObject>()
                    .Where(
                        gameObject =>
                            gameObject.IsValid &&
                            Regex.IsMatch(
                                gameObject.Name, "_w_windwall_enemy_0.\\.troy", RegexOptions.IgnoreCase))
                )
            {
                wall = gameObject;
            }
            if (wall == null)
            {
                return false;
            }
            var level = wall.Name.Substring(wall.Name.Length - 6, 1);
            var wallWidth = 300 + 50 * Convert.ToInt32(level);

            var wallDirection =
                (wall.Position.LSTo2D() - _yasuoWallCastedPos).LSNormalized().LSPerpendicular();
            var wallStart = wall.Position.LSTo2D() + wallWidth / 2f * wallDirection;
            var wallEnd = wallStart - wallWidth * wallDirection;

            for (var i = 0; i < start.LSDistance(end); i += 30)
            {
                var currentPosition = start.LSExtend(end, i);
                if (wallStart.LSIntersection(wallEnd, currentPosition.LSTo2D(), start.LSTo2D()).Intersects)
                {
                    return true;
                }
            }

            return false;
        }
    }
}