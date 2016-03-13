using System;
using System.Collections.Generic;
using System.Linq;
using AdEvade.Config.Controls;
using AdEvade.Data;
using AdEvade.Data.Spells;
using AdEvade.Draw;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SpellData = AdEvade.Data.Spells.SpellData;

namespace AdEvade.Config
{
    public enum ConfigDataType
    {
        Data, Spells, KeyBind, EvadeSpell
    }

    public class ConfigValueChangedArgs : EventArgs
    {
        public ConfigValue Key { get; set; }
        public string SpellKey { get; set; }
        public readonly bool IsSpell;
        public object Value { get; set; }

        public ConfigValueChangedArgs(ConfigValue key, object value)
        {
            Key = key;
            Value = value;
        }
        public ConfigValueChangedArgs(string key, object value)
        {
            SpellKey = key;
            IsSpell = true;
            Value = value;
        }
    }
    public static class Properties
    {
        public delegate void ConfigValueChangedHandler(ConfigValueChangedArgs args);
        public static event ConfigValueChangedHandler OnConfigValueChanged;

        public static Dictionary<ConfigValue, object> Values = new Dictionary<ConfigValue, object>();
        public static readonly Dictionary<string, SpellConfig> Spells = new Dictionary<string, SpellConfig>();
        private static Randomizer.Randomizer _randomizer = new Randomizer.Randomizer();

        public static readonly Dictionary<string, EvadeSpellConfig> EvadeSpells = new Dictionary<string, EvadeSpellConfig>();
        
        public static SpellConfig GetSpellConfig(this SpellData spell, SpellConfigControl control)
        {
            return new SpellConfig
            {
                SData = spell,
                DangerLevel = spell.Dangerlevel,
                Dodge = true,
                Draw = true,
                Radius = spell.Radius,
                PlayerName = spell.CharName
            };
        }
        public static void OnValueChanged(ConfigValue key, object value)
        {
            if(OnConfigValueChanged != null) OnConfigValueChanged.Invoke(new ConfigValueChangedArgs(key, value));
        }
        public static SpellConfig GetSpell(string key)
        {
            if (Spells.Any(i => i.Key == key))
            {
                return Randomize(Spells[key]);
            }
            return new SpellConfig { DangerLevel = SpellDangerLevel.Normal, Dodge = false, Draw = true, EvadeSpellMode = SpellModes.Undodgeable, Radius = 20 };

        }

        private static SpellConfig Randomize(SpellConfig spellConfig)
        {
            if (spellConfig == null) return null;
            if(_randomizer == null) _randomizer = new Randomizer.Randomizer();
            if (!KeysExist(ConfigValue.EnableRandomizer, ConfigValue.RandomizerMaxDangerLevel, ConfigValue.RandomizerPercentage, ConfigValue.DrawBlockedRandomizerSpells))
            {
                ConsoleDebug.WriteLine("Config Keys Not Found!", true);
                return spellConfig;
            }

            if (GetBool(ConfigValue.EnableRandomizer) && GetInt(ConfigValue.RandomizerMaxDangerLevel) > (int) spellConfig.DangerLevel)
            {
                spellConfig.Dodge = _randomizer.IsAbovePercentage(1f - GetInt(ConfigValue.RandomizerPercentage) / 100f);
                spellConfig.Draw = spellConfig.Draw && GetBool(ConfigValue.DrawBlockedRandomizerSpells);
            }
            return spellConfig;
        }

        public static EvadeSpellConfig GetEvadeSpell(string key)
        {
            if (EvadeSpells.Any(i => i.Key == key))
            {
                return EvadeSpells[key];
            }
            ConsoleDebug.WriteLineColor("Evade Spell: " + key + " Not Found, Returning: DO NOT USE", ConsoleColor.Red);
            return new EvadeSpellConfig { DangerLevel = SpellDangerLevel.Low, SpellMode = SpellModes.Undodgeable, Use = false};
        }

        public static bool KeysExist(params ConfigValue[] values)
        {
            if (Values == null) return false;
            return values.All(configValue => Values.ContainsKey(configValue));
        }

        public static int GetInt(ConfigValue key)
        {
            return (int)Values[key];
        }
        public static bool GetBool(ConfigValue key)
        {
            return (bool)Values[key];
        }
        public static void SetValue(ConfigValue key, object value, bool raiseEvent = true)
        {
            Values[key] = value;
            if(OnConfigValueChanged != null && raiseEvent) OnConfigValueChanged.Invoke(new ConfigValueChangedArgs(key, value));
        }
        public static void SetSpell(string id, SpellConfig value, bool raiseEvent = true)
        {
            if (Spells.Any(i => i.Key == id))
            {
                Spells[id] = value;
                return;
            }
            Spells.Add(id, value);
            if (raiseEvent && OnConfigValueChanged != null)
                OnConfigValueChanged.Invoke(new ConfigValueChangedArgs(id, value));
        }
        public static void SetEvadeSpell(string key, EvadeSpellConfig value, bool raiseEvent = true)
        {
            if (EvadeSpells.Any(i => i.Key == key))
            {
                EvadeSpells[key] = value;
                return;
            }
            EvadeSpells.Add(key, value);
            if (raiseEvent && OnConfigValueChanged != null)
                OnConfigValueChanged.Invoke(new ConfigValueChangedArgs(key, value));
        }

        public static void SetValues(Dictionary<ConfigValue, object> dictionary)
        {
            foreach (var o in dictionary)
            {
                Values[o.Key] = o.Value; //So we don't remove keys that didn't seem to be in the dictionary 
            }
        }
    }

    public class EvadeSpellConfig
    {
        public bool Use { get; set; }
        public SpellDangerLevel DangerLevel { get; set; }
        public SpellModes SpellMode { get; set; }

        public override string ToString()
        {
            return string.Format("Use: {0} DangerLevel: {1} SpellMode: {2}", Use, DangerLevel, SpellMode);
        }
    }

    public class SpellConfig
    {
        public SpellData SData { get; set; }
        public bool Dodge { get; set; }
        public bool Draw { get; set; }
        public float Radius { get; set; }
        public SpellDangerLevel DangerLevel { get; set; }
        public SpellModes EvadeSpellMode { get; set; }
        public string PlayerName { get; set; }

        public override string ToString()
        {
            return string.Format("{6} (Dodge:{0} Draw:{1} Radius:{2} DangerLevel:{3} EvadeSpellMode:{4} PlayerName:{5})", Dodge, Draw, Radius, DangerLevel, EvadeSpellMode, PlayerName,SData != null ? SData.SpellName : "");
        }
    }
}
