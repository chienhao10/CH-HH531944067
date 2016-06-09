using EloBuddy;
using LeagueSharp;
using SharpDX;
using SharpDX.Direct3D9;

namespace LeblancOLD
{
    internal class Slide
    {
        public GameObject Object { get; set; }
        public float NetworkId { get; set; }
        public Vector3 Position { get; set; }
        public double ExpireTime { get; set; }
        public Texture ExpireTimePicture { get; set; }
        public string Type { get; set; }
        public Texture Picture { get; set; }
    }
}