using EloBuddy.SDK.Menu.Values;

namespace AdEvade.Config.Controls
{
    public class DynamicComboBox : IDynamicControl<int>
    {
        public ComboBox ComboBox { get; set; }
        private readonly ConfigDataType _type;
        private readonly ConfigValue _configKey;
        public DynamicComboBox(ConfigDataType type, ConfigValue key, string displayName, int defaultValue, string[] values)
        {
            _configKey = key;
            _type = type;
            ComboBox = new ComboBox(displayName, defaultValue, values);
            ComboBox.OnValueChange += ComboBox_OnValueChange;
            Properties.OnConfigValueChanged += Properties_OnConfigValueChanged;
        }

        private void Properties_OnConfigValueChanged(ConfigValueChangedArgs args)
        {

            if (args.Key == _configKey)
            {
                if (ComboBox.CurrentValue != (int)args.Value)
                ComboBox.CurrentValue = (int)args.Value;
            }
                
        }

        private void ComboBox_OnValueChange(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
        {
            Properties.SetValue(_configKey, sender.CurrentValue, false);
        }

        public ConfigValue GetConfigValue()
        {
            return _configKey;
        }

        public object GetValue()
        {
            return ComboBox.CurrentValue;
        }

        public ValueBase<int> GetControl()
        {
            return ComboBox;
        }
    }
}