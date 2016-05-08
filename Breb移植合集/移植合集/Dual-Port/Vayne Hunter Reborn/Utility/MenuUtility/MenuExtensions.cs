using System;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy;

namespace VayneHunter_Reborn.Utility.MenuUtility
{
    static class MenuExtensions
    {
        public static void AddSkill(this Menu menu, Enumerations.Skills skill, string mode, bool defValue = true)
        {
            var name = string.Format("dz191.vhr.{0}.use{1}", mode.ToLower(), skill.ToString().ToLower());
            var displayName = string.Format("使用 {0}", skill);
            menu.Add(name, new CheckBox(displayName, defValue));
        }

        public static void AddManaLimiter(this Menu menu, Enumerations.Skills skill, string mode, int defMana = 0, bool displayMode = false)
        {
            var name = string.Format("dz191.vhr.{0}.mm.{1}.mana", mode.ToLower(), skill.ToString().ToLower());
            var displayName = displayMode ? string.Format("{0} 蓝量 {1}", skill, mode) : string.Format("{0} 蓝量", skill);
            menu.Add(name, new Slider(displayName, defMana));
        }

        public static bool IsEnabledAndReady(this Spell spell, string mode, bool checkMana = true)
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
            if (mode.Contains("laneclear") || mode.Contains("jungle") || mode.Contains("clear"))
            {
                m = MenuGenerator.farmMenu;
                mode = "laneclear";
            }

            var name = string.Format("dz191.vhr.{0}.use{1}", mode.ToLower(), spell.Slot.ToString().ToLower());
            var mana = string.Format("dz191.vhr.{0}.mm.{1}.mana", mode.ToLower(), spell.Slot.ToString().ToLower());

            //Console.WriteLine(mana);
            //Console.WriteLine(name);

            //dz191.vhr.farm.mm.q.mana
            //dz191.vhr.farm.useq
            //dz191.vhr.laneclear.useq

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
