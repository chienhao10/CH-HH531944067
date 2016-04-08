using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using Color = System.Drawing.Color;

namespace EzEvade
{
    public static class Render
    {
        public static class Circle
        {
            public static void DrawCircle(Vector3 pos, float radius, Color color, float width)
            {
                EloBuddy.SDK.Rendering.Circle.Draw(new ColorBGRA(color.R, color.G, color.B, 255), radius, width, pos);
            }
        }
    }
}
