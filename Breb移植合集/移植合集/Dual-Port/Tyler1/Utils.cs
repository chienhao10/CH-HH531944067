using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.SDK;
using SharpDX;

namespace Tyler1
{
    public static class Utils
    {
        private static Random _rand = new Random();
        public static Vector3 Randomize(this Vector3 v)
        {
            return new Vector2(v.X + _rand.Next(-10, 10), v.Y + _rand.Next(-10, 10)).ToVector3();
        }
    }
}
