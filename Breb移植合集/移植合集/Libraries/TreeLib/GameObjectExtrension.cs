using EloBuddy;
using EloBuddy.SDK;
using SharpDX;

namespace TreeLib.Extensions
{
    public static class GameObjectExtensions
    {
        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public static float DistanceToPlayer(this GameObject obj)
        {
            var unit = obj as Obj_AI_Base;
            if (unit == null || !unit.IsValid)
            {
                return obj.Position.Distance(Player.ServerPosition);
            }

            return unit.ServerPosition.Distance(Player.ServerPosition);
        }

        public static float DistanceToPlayer(this Vector3 position)
        {
            return position.Distance(Player.ServerPosition);
        }

        public static float DistanceToPlayer(this Vector2 position)
        {
            return position.Distance(Player.ServerPosition);
        }
    }
}