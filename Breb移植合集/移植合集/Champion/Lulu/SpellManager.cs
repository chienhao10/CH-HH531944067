using ClipperLib;
using Color = System.Drawing.Color;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;
using EloBuddy;
using Font = SharpDX.Direct3D9.Font;
using LeagueSharp.Common.Data;
using LeagueSharp.Common;
using SharpDX.Direct3D9;
using SharpDX;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Security.AccessControl;
using System;
using System.Speech.Synthesis;
using TreeLib.Extensions;

namespace LuluLicious
{
    internal static class SpellManager
    {
        public static LeagueSharp.Common.Spell Q;
        public static LeagueSharp.Common.Spell PixQ;
        public static LeagueSharp.Common.Spell W;
        public static LeagueSharp.Common.Spell E;
        public static LeagueSharp.Common.Spell R;

        static SpellManager()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 925);
            PixQ = new LeagueSharp.Common.Spell(SpellSlot.Q, 925);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 650);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 650);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 900);

            Q.SetSkillshot(0.25f, 60, 1450, false, SkillshotType.SkillshotLine);
            PixQ.SetSkillshot(0.25f, 60, 1450, false, SkillshotType.SkillshotLine);
        }

        public static void Initialize()
        {
        }

        public static bool IsActive(this LeagueSharp.Common.Spell spell, bool force = false)
        {
            if (force)
            {
                return true;
            }

            var m = Lulu.qMenu;

            if (spell.Slot == SpellSlot.Q)
            {
                m = Lulu.qMenu;
            }

            if (spell.Slot == SpellSlot.W)
            {
                m = Lulu.wMenu;
            }

            if (spell.Slot == SpellSlot.E)
            {
                m = Lulu.eMenu;
            }

            if (spell.Slot == SpellSlot.R)
            {
                m = Lulu.rMenu;
            }

            string s = "";

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                s = "Combo";
            }
            else
            {
                s = "Harass";
            }

            var name = spell.Slot + s;
            var item = m[name];
            return item != null && item.Cast<CheckBox>().CurrentValue;
        }
    }
}