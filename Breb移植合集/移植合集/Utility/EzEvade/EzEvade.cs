using System;
using System.Linq;
using ezEvade;
using EloBuddy;
using EloBuddy.SDK.Menu;

namespace EzEvade
{
    public static class EzEvade
    {
        public static SpellSlot GetSpellSlot(this AIHeroClient unit, string name)
        {
            foreach (var spell in
                unit.Spellbook.Spells.Where(
                    spell => String.Equals(spell.Name, name, StringComparison.CurrentCultureIgnoreCase)))
            {
                return spell.Slot;
            }

            return SpellSlot.Unknown;
        }

        public static Menu AddSubMenuEx(this Menu menu, string display, string unique)
        {
            var submenu = menu.AddSubMenu(display, unique);
            ObjectCache.menuCache.AddMenuToCache(submenu);
            return submenu;
        }
    }
}
