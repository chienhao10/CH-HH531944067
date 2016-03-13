using AdEvade.Utils;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using Color = System.Drawing.Color;

namespace AdEvade.Draw
{
    class RenderLine : RenderObject
    {
        public Vector2 Start = new Vector2(0, 0);
        public Vector2 End = new Vector2(0, 0);

        public int Width = 3;
        public Color color = Color.White;

        public RenderLine(Vector2 start, Vector2 end, float renderTime,
            int radius = 65, int width = 3)
        {
            StartTime = EvadeUtils.TickCount;
            EndTime = StartTime + renderTime;
            Start = start;
            End = end;

            Width = width;
        }

        public RenderLine(Vector2 start, Vector2 end, float renderTime,
            Color color, int radius = 65, int width = 3)
        {
            StartTime = EvadeUtils.TickCount;
            EndTime = StartTime + renderTime;
            Start = start;
            End = end;

            this.color = color;

            Width = width;
        }

        override public void Draw()
        {
            if (Start.IsOnScreen() || End.IsOnScreen())
            {
                var realStart = Drawing.WorldToScreen(Start.To3D());
                var realEnd = Drawing.WorldToScreen(End.To3D());

                Drawing.DrawLine(realStart, realEnd, Width, color);
            }
        }
    }
}
