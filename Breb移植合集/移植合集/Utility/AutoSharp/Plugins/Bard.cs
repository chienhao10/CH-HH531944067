using System; using EloBuddy; using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Plugins
{
    class Bard : PluginBase
    {
        public Bard()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 950);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 3400);

            Q.SetSkillshot(0.25f, 60, 1600, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.5f, 325, 2100, false, SkillshotType.SkillshotCircle);
        }

        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {
                if (Q.IsReady() && Heroes.Player.LSDistance(Target) < Q.Range)
                {
                    Q.Cast(Target);
                }
            }

            if (HarassMode)
            {
                if (W.CastCheck(Target, "HarassQ"))
                {
                    Q.Cast(Target);
                }
            }
        }

        public override void OnPossibleToInterrupt(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs spell)
        {
            if (spell.DangerLevel < Interrupter2.DangerLevel.High || unit.IsAlly)
            {
                return;
            }

            if (R.CastCheck(unit, "Interrupt.R"))
            {
                R.Cast(unit);
            }
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
        }

        public override void HarassMenu(Menu config)
        {
            config.AddBool("HarassQ", "Use Q", true);
        }

        public override void InterruptMenu(Menu config)
        {
            config.AddBool("Interrupt.R", "Use R to Interrupt Spells", true);
        }
    }
}
