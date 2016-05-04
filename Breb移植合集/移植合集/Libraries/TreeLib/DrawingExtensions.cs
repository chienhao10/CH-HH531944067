using SharpDX;
using Color = System.Drawing.Color;

namespace TreeLib.Extensions
{
    public static class DrawingExtensions
    {
        public static string ToHexString(this Color c)
        {
            return string.Format("#{0:X2}{1:X2}{2:X2}", c.R, c.G, c.B);
        }

        public static string ToHexString(this ColorBGRA c)
        {
            return string.Format("#{0:X2}{1:X2}{2:X2}", c.R, c.G, c.B);
        }

        public static ColorBGRA ToSharpDXColor(this Color c)
        {
            return new ColorBGRA(c.R, c.G, c.B, c.A);
        }

        public static Color ToDrawingColor(this ColorBGRA c)
        {
            return Color.FromArgb(c.A, c.R, c.G, c.B);
        }
    }
}