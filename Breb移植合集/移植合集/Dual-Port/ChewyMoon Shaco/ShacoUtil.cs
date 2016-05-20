#region

using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;

#endregion

namespace ChewyMoonsShaco
{
    internal class ShacoUtil
    {
        public static Vector3 GetQPos(AIHeroClient target, bool serverPos, int distance = 150)
        {
            var enemyPos = serverPos ? target.ServerPosition : target.Position;
            var myPos = serverPos ? ObjectManager.Player.ServerPosition : ObjectManager.Player.Position;

            return enemyPos + Vector3.Normalize(enemyPos - myPos) * distance;
        }

        public static Vector2 GetShortestWayPoint(List<Vector2> waypoints)
        {
            return waypoints.MinOrDefault(x => x.LSDistance(ObjectManager.Player));
        }
    }
}