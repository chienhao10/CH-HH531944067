using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using TreeLib.Extensions;
using TreeLib.Objects;
using Color = SharpDX.Color;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

namespace TreeLib.Managers
{
    public static class ManaManager
    {
        public enum ManaMode
        {
            Combo,
            Harass,
            Farm,
            None
        }

        private static Menu _menu;

        private static readonly Dictionary<ManaMode, Dictionary<SpellSlot, int>> ManaDictionary =
            new Dictionary<ManaMode, Dictionary<SpellSlot, int>>();

        private static ManaMode CurrentMode
        {
            get
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    return ManaMode.Combo;
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                {
                    return ManaMode.Harass;
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                {
                    return ManaMode.Farm;
                }

                return ManaMode.None;
            }
        }

        public static void Initialize(Menu menu)
        {
            _menu = menu.AddSubMenu("ManaManager", "Mana Manager");
            _menu.Add("Enabled", new CheckBox("Enabled", false));
        }

        public static void SetManaCondition(this LeagueSharp.Common.Spell spell, ManaMode mode, int value)
        {
            if (!ManaDictionary.ContainsKey(mode))
            {
                ManaDictionary.Add(mode, new Dictionary<SpellSlot, int>());
            }

            ManaDictionary[mode].Add(spell.Slot, value);

            var m = mode.ToString();

            if (_menu[m + "Enabled" + spell.Slot] == null)
            {
                _menu.Add(m + "Enabled" + spell.Slot, new CheckBox("Enabled in " + m));
            }

            var item = _menu.Add(ObjectManager.Player.ChampionName + spell.Slot + "Mana" + m, new Slider(spell.Slot + " Mana Percent", value));

            _menu.AddSeparator();

            _menu[ObjectManager.Player.ChampionName + spell.Slot + "Mana" + m].Cast<Slider>().OnValueChange += delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args) { ManaDictionary[mode][spell.Slot] = args.NewValue; };
        }

        public static bool HasManaCondition(this LeagueSharp.Common.Spell spell)
        {
            if (!_menu["Enabled"].Cast<CheckBox>().CurrentValue)
            {
                return false;
            }

            var mode = CurrentMode;

            if (mode == ManaMode.None || !ManaDictionary.ContainsKey(mode) || !_menu[mode + "Enabled" + spell.Slot].Cast<CheckBox>().CurrentValue)
            {
                return false;
            }

            var currentMode = ManaDictionary[mode];

            if (!currentMode.ContainsKey(spell.Slot))
            {
                return false;
            }

            return ObjectManager.Player.ManaPercent < currentMode[spell.Slot];
        }
    }
}