using System;
using System.Linq;
using DZLib.Modules;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using iKalistaReborn.Utils;
using LeagueSharp.Common;

namespace iKalistaReborn.Modules
{
    internal class JungleStealModule : IModule
    {
        public static Menu
            comboMenu = Kalista.comboMenu,
            mixedMenu = Kalista.mixedMenu,
            laneclearMenu = Kalista.laneclearMenu,
            jungleStealMenu = Kalista.jungleStealMenu,
            miscMenu = Kalista.miscMenu,
            drawingMenu = Kalista.drawingMenu;

        public void OnLoad()
        {
            Console.WriteLine("Jungle Steal Module Loaded");
        }

        public string GetName()
        {
            return "JungleSteal";
        }

        public bool ShouldGetExecuted()
        {
            return SpellManager.Spell[SpellSlot.E].IsReady() &&
                   getCheckBoxItem(jungleStealMenu, "com.ikalista.jungleSteal.enabled");
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public void OnExecute()
        {
            var attackableMinion =
                MinionManager.GetMinions(ObjectManager.Player.ServerPosition, SpellManager.Spell[SpellSlot.E].Range,
                    MinionTypes.All, MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth).FirstOrDefault(x => !x.Name.Contains("Mini"));

            if (attackableMinion == null || !attackableMinion.HasRendBuff() || !attackableMinion.IsMobKillable() ||
                !getCheckBoxItem(jungleStealMenu, attackableMinion.CharData.BaseSkinName))
                return;

            Console.WriteLine("Minion Killable: " + attackableMinion.CharData.BaseSkinName);
            SpellManager.Spell[SpellSlot.E].Cast();
        }

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }
    }
}