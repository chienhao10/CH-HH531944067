using AdEvade.Data.Spells;
using AdEvade.Draw;
using EloBuddy.SDK.Menu.Values;

namespace AdEvade.Config.Controls
{
    public enum SpellConfigProperty 
    {
        Dodge, Draw, Radius, DangerLevel, SpellMode, UseEvadeSpell, None, 
    }
    public class DynamicSlider : IDynamicControl<int>
    {
        public Slider Slider;
        private readonly ConfigDataType _type;
        private readonly ConfigValue _configKey;
        private readonly string _spellKey;
        private bool _isBasedOnSpell;
        private SpellConfigProperty _spellProperty;

        public DynamicSlider(ConfigDataType configDataType, ConfigValue key, string displayName, int defaultValue, int minValue, int maxValue)
        {
            _type = configDataType;
            _configKey = key;
            DynamicSliderInit(displayName, defaultValue, minValue, maxValue, false, SpellConfigProperty.None);
        }

        public DynamicSlider(ConfigDataType configDataType, string spelKey, string displayName, int defaultValue,
            int minValue, int maxValue, bool isBasedOnSpell, SpellConfigProperty property)
        {
            _type = configDataType;
            _spellKey = spelKey;
            DynamicSliderInit(displayName, defaultValue, minValue, maxValue, isBasedOnSpell, property);
        }

        public void DynamicSliderInit(string displayName, int defaultValue,
            int minValue, int maxValue, bool isBasedOnSpell, SpellConfigProperty property)
        {
            _spellProperty = property;
            _isBasedOnSpell = isBasedOnSpell;
            Slider = new Slider(displayName, defaultValue, minValue, maxValue);
            if (_type == ConfigDataType.Data)
            {
                Properties.SetValue(_configKey, defaultValue, false);
            }
            Slider.OnValueChange += Slider_OnValueChange;
            Properties.OnConfigValueChanged += Config_OnConfigValueChanged;
        }

        private void Config_OnConfigValueChanged(ConfigValueChangedArgs args)
        {
            if(!(args.Value is int)) return;
            if (args.Key == _configKey && !_isBasedOnSpell) Slider.CurrentValue = (int) args.Value;
        }

        private void Slider_OnValueChange(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
        {
            switch (_type)
            {
                case ConfigDataType.Data:
                    Properties.SetValue(_configKey, sender.CurrentValue, false);
                    break;
                case ConfigDataType.Spells:
                    if (_isBasedOnSpell)
                    {
                        var spell = Properties.GetSpell(_spellKey);
                        switch (_spellProperty)
                        {
                            case SpellConfigProperty.Radius:
                                spell.Radius = sender.CurrentValue;
                                break;
                            case SpellConfigProperty.DangerLevel:
                                spell.DangerLevel = (SpellDangerLevel) sender.CurrentValue;
                                break;
                            case SpellConfigProperty.SpellMode:
                                spell.EvadeSpellMode = (SpellModes) sender.CurrentValue;
                                break;
                            default:
                                return;
                        }
                        Properties.SetSpell(_spellKey, spell);
                    }
                    break;
                case ConfigDataType.EvadeSpell:
                    if (_isBasedOnSpell)
                    {
                        var spell = Properties.GetEvadeSpell(_spellKey);
                        switch (_spellProperty)
                        {
                            case SpellConfigProperty.DangerLevel:
                                spell.DangerLevel = (SpellDangerLevel) sender.CurrentValue;
                                break;
                            case SpellConfigProperty.SpellMode:
                                spell.SpellMode = (SpellModes) sender.CurrentValue;
                                break;
                            default:
                                return;
                        }
                        Properties.SetEvadeSpell(_spellKey, spell);
                    }
                    break;
            }
        }

        public ConfigValue GetConfigValue()
        {
            return _configKey;
        }

        public object GetValue()
        {
            return Slider.CurrentValue;
        }

        public ValueBase<int> GetControl()
        {
            return Slider;
        }
    }
}