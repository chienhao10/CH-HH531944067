using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace Jhin.Managers
{
    public class Circle
    {
        public CheckBox CheckBox;
        public ColorBGRA Color;

        public Func<bool> Condition;
        public Func<float> Range;
        public Func<GameObject> SourceObject;
        public int Width = 1;

        public Circle(CheckBox checkBox, ColorBGRA color, Func<float> range, Func<bool> condition,
            Func<GameObject> source)
        {
            CheckBox = checkBox;
            Range = range;
            Color = color;
            Condition = condition;
            SourceObject = source;
        }

        public bool Enabled
        {
            get { return CheckBox.CurrentValue; }
        }
    }

    public static class CircleManager
    {
        public static readonly List<Circle> Circles = new List<Circle>();

        public static void Draw()
        {
            foreach (var circle in Circles.Where(c => c.Enabled && c.Condition()))
            {
                var obj = circle.SourceObject();
                if (obj.IsValid && !obj.IsDead)
                {
                    EloBuddy.SDK.Rendering.Circle.Draw(circle.Color, circle.Range(), circle.Width, obj);
                }
            }
        }
    }
}