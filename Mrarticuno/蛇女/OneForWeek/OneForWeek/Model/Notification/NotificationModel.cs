using System.Text.RegularExpressions;
using SharpDX;

namespace OneForWeek.Model.Notification
{
    public class NotificationModel
    {
        private float time;
        private float v1;
        private float v2;
        private string v3;
        private Color deepSkyBlue;

        public float StartTimer { get; set; }
        public float ShowTimer { get; set; }
        public float AnimationTimer { get; set; }
        public string ShowText { get; set; }
        public System.Drawing.Color Color { get; set; }

        public NotificationModel(float startTimer, float showTimer, float animationTimer, string showText, System.Drawing.Color color)
        {
            StartTimer = startTimer;
            ShowTimer = showTimer;
            AnimationTimer = animationTimer;
            var value = Regex.Replace(showText, ".{23}", "$0\n");
            ShowText = value;
            Color = color;
        }

        public NotificationModel(float time, float v1, float v2, string v3, Color deepSkyBlue)
        {
            this.time = time;
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
            this.deepSkyBlue = deepSkyBlue;
        }
    }
}
