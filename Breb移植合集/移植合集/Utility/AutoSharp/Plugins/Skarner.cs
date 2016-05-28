using System; using EloBuddy; using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Plugins
{
    public class Skarner : PluginBase
    {
        public Skarner()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 350);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 0);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 1000);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 350);


            E.SetSkillshot(0.50f, 60, 1200, false, SkillshotType.SkillshotLine);
        }

        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {
                if (Q.IsReady() && Player.LSCountEnemiesInRange(Q.Range) > 0)
                {
                    Q.Cast();
                    if (W.IsReady())
                    {
                        W.Cast();
                    }
                }
                if (E.IsReady() && Heroes.Player.LSDistance(Target) < E.Range)
                {
                    E.Cast(Target);
                }
                if (R.IsReady() && Target.LSIsValidTarget(R.Range))
                {
                    R.Cast(Target);
                }
            }
        }


        public override void OnPossibleToInterrupt(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs spell)
        {
            if (spell.DangerLevel < Interrupter2.DangerLevel.High || unit.IsAlly)
            {
                return;
            }

            if (R.IsReady() && unit.LSIsValidTarget(R.Range))
            {
                R.Cast(unit);
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