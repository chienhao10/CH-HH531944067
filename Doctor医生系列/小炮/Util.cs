using System;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace Tristana
{
    public static class Util
    {

        public static Slider CreateHitChanceSlider(string identifier, string displayName, HitChance defaultValue, Menu menu)
        {
            var slider = menu.Add(identifier, new Slider(displayName, (int) defaultValue, 0, 8));
            var hcNames = new[]
            {"Unknown", "Impossible", "Collision", "Low", "AveragePoint", "Medium", "High", "Dashing", "Immobile"};
            slider.DisplayName = hcNames[slider.CurrentValue];
            slider.OnValueChange +=
                delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
                {
                    sender.DisplayName = hcNames[changeArgs.NewValue];
                };
            return slider;
        }

        public static HitChance GetHitChanceSliderValue(Slider slider)
        {
            if (slider == null)
            {
                return HitChance.Impossible;
            }
            var currVal = slider.CurrentValue;
            return (HitChance)currVal;
        }

        public static bool Between(this Vector3 checkPos, Vector3 source, Vector3 destination)
        {
            return Math.Abs(((source.X * checkPos.Y) + (source.Y * destination.X) + (checkPos.X * destination.Y)) - ((checkPos.Y * destination.X) + (source.X * destination.Y) + (source.Y * checkPos.X))) < 5;
        }
    }
}
