using System;
using DZLib;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace iDZEzreal.MenuHelper
{
    public static class MenuGenerator
    {
        public static void Generate()
        {
            Variables.Menu = MainMenu.AddMenu("iDZEzreal 3.0", "ezreal");

            Variables.comboMenu = Variables.Menu.AddSubMenu("[Ez] Combo", "ezreal.combo");
            Variables.comboMenu.AddBool("ezreal.combo.q", "Use Q", true);
            Variables.comboMenu.AddBool("ezreal.combo.w", "Use W", true);
            Variables.comboMenu.AddBool("ezreal.combo.r", "Use R", true);
            Variables.comboMenu.AddSlider("ezreal.combo.r.min", "Min Enemies", 2, 1, 5);

            Variables.mixedMenu = Variables.Menu.AddSubMenu("[Ez] Harass", "ezreal.mixed");
            Variables.mixedMenu.AddBool("ezreal.mixed.q", "Use Q", true);
            Variables.mixedMenu.AddBool("ezreal.mixed.w", "Use W", true);
            Variables.mixedMenu.AddSlider("ezreal.mixed.mana", "Min Mana", 45, 0, 100);

            Variables.farmMenu = Variables.Menu.AddSubMenu("[Ez] Farm", "ezreal.farm");
            Variables.farmMenu.AddBool("ezreal.farm.q", "Use Q", true);

            Variables.miscMenu = Variables.Menu.AddSubMenu("[Ez] Misc", "ezreal.misc");
            Variables.miscMenu.AddStringList("ezreal.misc.hitchance", "Hitchance", new[] { "Low", "Medium", "High", "Very High" }, 3);
            Variables.miscMenu.AddBool("ezreal.misc.gapcloser", "Anti Gap Closer", true);
            Variables.miscMenu.AddBool("ezreal.misc.selfWE", "Self W/E - Beta AF", true);
            Variables.miscMenu.AddKeybind("ezreal.misc.semimanualr", "Semimanual R", false, KeyBind.BindTypes.HoldActive, 'U');

            Variables.moduleMenu = Variables.Menu.AddSubMenu("[Ez] Modules", "ezreal.modules");
            foreach (var module in Variables.Modules)
            {
                Variables.moduleMenu.AddBool("ezreal.modules." + module.GetName().ToLowerInvariant(), "" + module.GetName());
            }

            Variables.drawingsMenu = Variables.Menu.AddSubMenu("[Ez] Drawings", "ezreal.drawings");
            Variables.drawingsMenu.AddBool("ezreal.drawings.q", "Q Draw", true);
            Variables.drawingsMenu.AddBool("ezreal.drawings.w", "W Draw", true);
        }

        public static void AddSlider(this Menu m, String uniq, String display, int def, int min, int max)
        {
            m.Add(uniq, new Slider(display, def, min, max));
        }

        public static void AddStringList(this Menu m, String uniq, String display, String[] s, int def = 0)
        {
            m.Add(uniq, new ComboBox(display, def, s));
        }

        public static void AddKeybind(this Menu m, String uniq, String display, bool def = false, KeyBind.BindTypes key = KeyBind.BindTypes.HoldActive, uint keyy = 32)
        {
            m.Add(uniq, new KeyBind(display, def, key, keyy));
        }

        public static void AddBool(this Menu m, String uniq, String display, bool def = true)
        {
            m.Add(uniq, new CheckBox(display, def));
        }


        public static HitChance GetHitchance()
        {
            switch (Variables.miscMenu["ezreal.misc.hitchance"].Cast<ComboBox>().CurrentValue)
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
                case 3:
                    return HitChance.VeryHigh;
            }
            return HitChance.High;
        }
    }
}