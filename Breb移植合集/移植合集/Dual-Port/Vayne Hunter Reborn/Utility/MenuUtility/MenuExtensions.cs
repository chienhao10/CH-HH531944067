using System;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy;
using EloBuddy.SDK;

namespace VayneHunter_Reborn.Utility.MenuUtility
{
    static class MenuExtensions
    {
        public static void AddSkill(this Menu menu, Enumerations.Skills skill, string mode, bool defValue = true)
        {
            var name = string.Format("dz191.vhr.{0}.use{1}", mode.ToLower(), skill.ToString().ToLower());
            var displayName = string.Format("Use {0}", skill);
            menu.Add(name, new CheckBox(displayName, defValue));
        }

        public static void AddManaLimiter(this Menu menu, Enumerations.Skills skill, string mode, int defMana = 0, bool displayMode = false)
        {
            var name = string.Format("dz191.vhr.{0}.mm.{1}.mana", mode.ToLower(), skill.ToString().ToLower());
            var displayName = displayMode ? string.Format("{0} Mana {1}", skill, mode) : string.Format("{0} Mana", skill);
            menu.Add(name, new Slider(displayName, defMana));
        }

        public static bool IsEnabledAndReady(this LeagueSharp.Common.Spell spell, string mode, bool checkMana = true)
        {
            var m = Variables.Menu;

            if (mode.Contains("combo"))
            {
                m = MenuGenerator.comboMenu;
                mode = "combo";
            }

            if (mode.Contains("harass"))
            {
                m = MenuGenerator.harassMenu;
            }

            if (mode.Contains("lasthit"))
            {
                m = MenuGenerator.farmMenu;
                mode = "lasthit";
            }

            if (mode.Contains("clear"))
            {
                m = MenuGenerator.farmMenu;
                mode = "laneclear";
            }

            var name = string.Format("dz191.vhr.{0}.use{1}", mode.ToLower(), spell.Slot.ToString().ToLower());
            var mana = string.Format("dz191.vhr.{0}.mm.{1}.mana", mode.ToLower(), spell.Slot.ToString().ToLower());

            if (m[name] != null && m[mana] != null)
            {
                return spell.IsReady() && m[name].Cast<CheckBox>().CurrentValue && (!checkMana || (ObjectManager.Player.Mana >= m[mana].Cast<Slider>().CurrentValue));
            }
            else
            {
                return false;
            }
        }
    }
}
