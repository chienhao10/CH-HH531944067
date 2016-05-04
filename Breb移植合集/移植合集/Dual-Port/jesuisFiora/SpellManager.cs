using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp;
using LeagueSharp.Common;
using TreeLib.Extensions;

namespace jesuisFiora
{
    internal static class SpellManager
    {
        public static LeagueSharp.Common.Spell Q;
        public static LeagueSharp.Common.Spell W;
        public static LeagueSharp.Common.Spell E;
        public static LeagueSharp.Common.Spell R;
        public static LeagueSharp.Common.Spell Ignite;
        public static float QSkillshotRange = 400;
        public static float QCircleRadius = 350;

        static SpellManager()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, QSkillshotRange + QCircleRadius);
            Q.SetSkillshot(.25f, 0, 500, false, SkillshotType.SkillshotLine);

            W = new LeagueSharp.Common.Spell(SpellSlot.W, 750);
            W.SetSkillshot(0.5f, 70, 3200, false, SkillshotType.SkillshotLine);

            E = new LeagueSharp.Common.Spell(SpellSlot.E);
            E.SetTargetted(0f, 0f);

            R = new LeagueSharp.Common.Spell(SpellSlot.R, 500);
            R.SetTargetted(.066f, 500);

            var igniteSlot = ObjectManager.Player.GetSpellSlot("summonerdot");

            if (!igniteSlot.Equals(SpellSlot.Unknown))
            {
                Ignite = new LeagueSharp.Common.Spell(igniteSlot, 600);
                Ignite.SetTargetted(.172f, 20);
            }
        }

        public static bool IsActive(this LeagueSharp.Common.Spell spell)
        {
            string s = "";
            Menu m = null;
            if (spell.Slot == SpellSlot.Q)
            {
                m = Program.qMenu;
            }

            if (spell.Slot == SpellSlot.E)
            {
                m = Program.eMenu;
            }

            if (spell.Slot == SpellSlot.R)
            {
                m = Program.rMenu;
            }


            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                s = "LaneClear";
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                s = "Combo";
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                s = "Harass";
            }

            var name = spell.Slot + s;
            var item = getCheckBoxItem(m, name);
            return m[name] != null && item;
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