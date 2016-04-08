using AdEvade.Utils;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using Color = System.Drawing.Color;

namespace AdEvade.Draw
{
    class RenderText : RenderObject
    {
        public Vector2 RenderPosition = new Vector2(0, 0);
        public string Text = "";

        public Color color = Color.White;

        public RenderText(string text, Vector2 renderPosition, float renderTime)
        {
            StartTime = EvadeUtils.TickCount;
            EndTime = StartTime + renderTime;
            RenderPosition = renderPosition;

            Text = text;
        }

        public RenderText(string text, Vector2 renderPosition, float renderTime,
            Color color)
        {
            StartTime = EvadeUtils.TickCount;
            EndTime = StartTime + renderTime;
            RenderPosition = renderPosition;

            this.color = color;

            Text = text;
        }

        override public void Draw()
        {
            if (RenderPosition.IsOnScreen())
            {
                var textDimension = Drawing.GetTextEntent(Text, 12);
                var wardScreenPos = Drawing.WorldToScreen(RenderPosition.To3D());

                Drawing.DrawText(wardScreenPos.X - textDimension.Width / 2, wardScreenPos.Y, color, Text);
            }
        }
    }
}
