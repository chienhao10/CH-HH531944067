using System;
using AdEvade.Draw;
using EloBuddy.SDK.Menu.Values;

namespace AdEvade.Config.Controls
{
    public class DynamicCheckBox : IDynamicControl<bool>
    {
        public CheckBox CheckBox;
        private readonly ConfigDataType _type;
        private readonly ConfigValue _configKey;
        private readonly string _spellKey;
        private bool _isBasedOnSpell;
        private SpellConfigProperty _spellProperty;


        public DynamicCheckBox(ConfigDataType configDataType, ConfigValue key, string displayName, bool defaultValue)
        {
            _type = configDataType;
            _configKey = key;
            DynamicCheckBoxInit(displayName, defaultValue, false, SpellConfigProperty.None);
        }

        public DynamicCheckBox(ConfigDataType configDataType, string spellKey, string displayName, bool defaultValue,
            bool isBasedOnSpell, SpellConfigProperty property)
        {
            _type = configDataType;
            _spellKey = spellKey;
            DynamicCheckBoxInit(displayName, defaultValue, isBasedOnSpell, property);
        }

        public void DynamicCheckBoxInit(string displayName, bool defaultValue, bool isBasedOnSpell,
            SpellConfigProperty property)
        {
            CheckBox = new CheckBox(displayName, defaultValue);
            _spellProperty = property;
            _isBasedOnSpell = isBasedOnSpell;
            if (_type == ConfigDataType.Data)
            {
                Properties.SetValue(_configKey, defaultValue, false);
            }
            CheckBox.OnValueChange += CheckBox_OnValueChange;
            Properties.OnConfigValueChanged += Config_OnConfigValueChanged;
        }

        private void Config_OnConfigValueChanged(ConfigValueChangedArgs args)
        {
            if (args.Key == _configKey && !_isBasedOnSpell) CheckBox.CurrentValue = (bool) args.Value;
        }

        private void CheckBox_OnValueChange(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
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
                            case SpellConfigProperty.Dodge:
                                spell.Dodge = sender.CurrentValue;
                                break;
                            case SpellConfigProperty.Draw:
                                spell.Draw = sender.CurrentValue;
                                break;
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
                            case SpellConfigProperty.UseEvadeSpell:
                                spell.Use = sender.CurrentValue;
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
            return CheckBox.CurrentValue;
        }

        public ValueBase<bool> GetControl()
        {
            return CheckBox;
        }
    }
}