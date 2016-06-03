using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;

namespace VCursor
{
    internal static class Utility
    {
        public static Vector3 ToWorldPoint(this Vector2 position)
        {
            if (!position.IsValid())
            {
                return new Vector3();
            }

            return Drawing.ScreenToWorld(position);
        }

        public static Vector2 ToScreenPoint(this Vector3 position)
        {
            return Drawing.WorldToScreen(position);
        }

        public static Vector2 ToScreenPoint(this Vector2 position)
        {
            return Drawing.WorldToScreen(position.To3D());
        }

        public static bool HoverShop(this Vector2 position)
        {
            var shop = ObjectManager.Get<Obj_Shop>().FirstOrDefault(o => o.Position.LSDistance(position.ToWorldPoint()) < 300);
            return ObjectManager.Player.InShop() && shop != null && !Shop.IsOpen;
        }

        public static bool HoverAllyTurret(this Vector2 position)
        {
            var allyTurret =
                ObjectManager.Get<Obj_AI_Turret>()
                    .FirstOrDefault(o => o.IsAlly && o.LSDistance(position.ToWorldPoint()) < 120 && o.Health < 9999);
            return allyTurret != null;
        }

        public static bool HoverEnemy(this Vector2 position)
        {
            var enemy =
                ObjectManager.Get<Obj_AI_Base>()
                    .FirstOrDefault(o => o.LSIsValidTarget(300, true, position.ToWorldPoint()));
            return enemy != null;
        }
    }
}