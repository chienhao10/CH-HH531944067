using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Twitch
{
    public static class Util
    {

        public static Slider CreateHCSlider(string identifier, string displayName, HitChance defaultValue, Menu menu)
        {
            var slider = menu.Add(identifier, new Slider(displayName, (int)defaultValue, 0, 8));
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

        public static HitChance GetHCSliderHitChance(Slider slider)
        {
            if (slider == null)
            {
                return HitChance.Impossible;
            }
            var currVal = slider.CurrentValue;
            return (HitChance)currVal;
        }
    }
}
