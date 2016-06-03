using EloBuddy;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace VCursor
{
    internal static class Cursor
    {
        public static Vector3 GamePosition => Game.CursorPos;
        public static Vector2 GameScreenPosition => GamePosition.ToScreenPoint();
        public static Vector2 ScreenPosition => Utils.GetCursorPos();
        public static Vector3 ScreenGamePosition => ScreenPosition.ToWorldPoint();
    }
}