//Plugins Brand Take Combo Part From Hellsing Brand Credit to Hellsing

using System; using EloBuddy; using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using System.Collections.Generic;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Plugins
{
    public class Brand : PluginBase
    {
        // ReSharper disable once InconsistentNaming
        public readonly List<LeagueSharp.Common.Spell> SpellList = new List<LeagueSharp.Common.Spell>();

        public Brand()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 1100);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 900);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 625);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 750);

            SpellList.AddRange(new[] { Q, W, E, R });
            Q.SetSkillshot(0.25f, 60, 1600, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(1, 240, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetTargetted(0.25f, float.MaxValue);
            R.SetTargetted(0.25f, 1000);
        }

        public override void OnUpdate(EventArgs args)
        {
            OnCombo(Target);
        }

        private void OnCombo(AIHeroClient target)
        {
            // Target validation
            if (target == null)
            {
                return;
            }

            //0KTW
            if (Q.IsReady())
            {
                Q.Cast(target);
            }

            if (W.IsReady())
            {
                W.Cast(target);
            }

            if (E.IsReady())
            {
                E.Cast(target);
            }

            if (R.IsReady())
            {
                R.Cast(target);
            }
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboW", "Use W", true);
            config.AddBool("ComboE", "Use E", true);
            config.AddBool("ComboR", "Use R", true);
        }
    }
}