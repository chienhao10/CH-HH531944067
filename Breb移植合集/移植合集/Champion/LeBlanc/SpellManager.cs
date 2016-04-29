using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using LeagueSharp.Common;
using System.Collections.Generic;
using Spell = LeagueSharp.Common.Spell;

namespace PopBlanc
{
    internal static class SpellManager
    {
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R, ER;
        public static Spell EPrediction;
        public static List<Spell> SpellList = new List<Spell>();

        private static Menu _menu;

        static SpellManager()
        {
            Q = new Spell(SpellSlot.Q, 700);
            Q.SetTargetted(.401f, 2000);

            W = new Spell(SpellSlot.W, 600);
            W.SetSkillshot(.5f, 100, 2000, false, SkillshotType.SkillshotCircle);

            E = new Spell(SpellSlot.E, 950);
            E.SetSkillshot(.25f, 70, 1750, true, SkillshotType.SkillshotLine);

            ER = new Spell(SpellSlot.R, 950);
            ER.SetSkillshot(.25f, 70, 1750, true, SkillshotType.SkillshotLine);

            EPrediction = new Spell(SpellSlot.E, 950);
            EPrediction.SetSkillshot(.25f, 70, 1750, true, SkillshotType.SkillshotLine);

            R = new Spell(SpellSlot.R, 720);
            {
                SpellList.Add(Q);
                SpellList.Add(W);
                SpellList.Add(E);
                SpellList.Add(R);
            }
        }

        public static void Initialize(Menu menu)
        {
            _menu = menu;
        }

        public static bool IsActive(this Spell spell, bool force = false)
        {
            if (force)
            {
                return true;
            }

            var s = "";
            var menu = Program.eMenu;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                s = "Combo";
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                s = "Harass";
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                s = "LastHit";
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                s = "LaneClear";
            }

            if (spell.Slot == SpellSlot.Q)
            {
                menu = Program.qMenu;
            }

            if (spell.Slot == SpellSlot.W)
            {
                menu = Program.wMenu;
            }

            if (spell.Slot == SpellSlot.E)
            {
                menu = Program.eMenu;
            }

            if (spell.Slot == SpellSlot.R)
            {
                menu = Program.rMenu;
            }

            var name = s + spell.Slot;
            var item = Program.getCheckBoxItem(menu, name);
            return item;
        }

        public static bool IsFirstW(this Spell spell)
        {
            return spell.Instance.ToggleState.Equals(1);
        }

        public static SpellSlot GetSpellSlot(this Spell spell)
        {
            switch (spell.Instance.Name)
            {
                case "LeblancChaosOrbM":
                    return SpellSlot.Q;
                case "LeblancSlideM":
                    return SpellSlot.W;
                case "LeblancSoulShackleM":
                    return SpellSlot.E;
                default:
                    return SpellSlot.R;
            }
        }

        public static void UpdateUltimate()
        {
            switch (R.GetSpellSlot())
            {
                case SpellSlot.Q:
                    R = new Spell(SpellSlot.R, 700);
                    R.SetTargetted(.401f, 2000);
                    return;
                case SpellSlot.W:
                    R = new Spell(SpellSlot.R, 600);
                    R.SetSkillshot(.5f, 100, 2000, false, SkillshotType.SkillshotCircle);
                    return;
                case SpellSlot.E:
                    R = new Spell(SpellSlot.R, 950);
                    R.SetSkillshot(.366f, 70, 1600, true, SkillshotType.SkillshotLine);
                    return;
            }
        }
    }
}