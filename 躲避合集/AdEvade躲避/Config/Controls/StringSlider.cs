using EloBuddy.SDK.Menu.Values;

namespace AdEvade.Config.Controls
{
    public class StringSlider
    {
        public DynamicSlider Slider;
        public string[] Options;

        public StringSlider(ConfigDataType type, string key, string displayName, int defaultValueIndex = 0, SpellConfigProperty property = SpellConfigProperty.None, params string[] options)
        {
            Options = options;
            Slider = new DynamicSlider(type, key, displayName, defaultValueIndex, 0, options.Length - 1, property != SpellConfigProperty.None, property);
            Slider.Slider.OnValueChange += _slider_OnValueChange;
            Slider.Slider.DisplayName = options[Slider.Slider.CurrentValue];
        }

        private void _slider_OnValueChange(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
        {
            sender.DisplayName = Options[args.NewValue];
        }
    }
}