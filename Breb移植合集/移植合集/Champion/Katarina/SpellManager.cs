using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using Spell = LeagueSharp.Common.Spell;

namespace Staberina
{
    internal static class SpellManager
    {
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        private static Menu _menu;

        static SpellManager()
        {
            Q = new Spell(SpellSlot.Q, 675);
            W = new Spell(SpellSlot.W, 375);
            E = new Spell(SpellSlot.E, 700);
            R = new Spell(SpellSlot.R, 550);
        }

        public static void Initialize(Menu menu)
        {
            _menu = menu;
        }

        public static bool IsActive(this Spell spell, bool ks = false)
        {
            var mode = Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
            var name = string.Format("{0}{1}{2}", ks ? "KS" : string.Empty, spell.Slot.ToString().ToUpper(),
                ks ? string.Empty : (mode ? "Combo" : "Harass"));
            var item = false;
            if ((spell.Slot == SpellSlot.Q || spell.Slot == SpellSlot.Q || spell.Slot == SpellSlot.Q ||
                 spell.Slot == SpellSlot.Q) && ks)
            {
                item = Katarina.getCheckBoxItem(Katarina.ksMenu, name);
            }
            if (!ks)
            {
                if (spell.Slot == SpellSlot.Q)
                {
                    item = Katarina.getCheckBoxItem(Katarina.qMenu, name);
                }
                if (spell.Slot == SpellSlot.W)
                {
                    item = Katarina.getCheckBoxItem(Katarina.wMenu, name);
                }
                if (spell.Slot == SpellSlot.E)
                {
                    item = Katarina.getCheckBoxItem(Katarina.eMenu, name);
                }
                if (spell.Slot == SpellSlot.R)
                {
                    item = Katarina.getCheckBoxItem(Katarina.rMenu, name);
                }
            }
            return item;
        }

        public static bool IsCastable(this Spell spell, Obj_AI_Base target, bool ks = false, bool checkKillable = true)
        {
            return spell.CanCast(target) && spell.IsActive(ks) && (!checkKillable || spell.IsKillable(target));
        }
    }
}