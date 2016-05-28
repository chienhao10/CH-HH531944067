using System; using EloBuddy; using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Plugins
{
    public class Azir : PluginBase
    {


        public Azir()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 850);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 450);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 2000);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 450);

            Q.SetSkillshot(0.1f, 100, 1700, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 100, 1200, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.5f, 700, 1400, false, SkillshotType.SkillshotLine);
        }

        public override void OnUpdate(EventArgs args)
        {

            if (ComboMode)
            {

                if (ShouldR(Target) && R.IsReady() && Heroes.Player.LSDistance(Target) < R.Range)
                {
                    R.Cast(Target);
                }
                if (W.IsReady() && Heroes.Player.LSDistance(Target) < W.Range)
                {
                    W.Cast(Target);
                }
                if (Q.IsReady() && Heroes.Player.LSDistance(Target) < Q.Range)
                {
                    Q.Cast(Target);
                }
                if (E.IsReady() && Heroes.Player.LSDistance(Target) < E.Range)
                {
                    E.Cast(Target);
                }

            }


        }



        private bool ShouldR(AIHeroClient target)
        {
            if (Player.LSGetSpellDamage(target, SpellSlot.R) > target.Health - 150)
                return true;

            var hpPercent = Player.Health / Player.MaxHealth * 100;
            if (hpPercent < 20)
                return true;

            var pred = R.GetPrediction(target);
            if (pred.AoeTargetsHitCount >= 2)
                return true;

            return false;
        }



        public override void OnPossibleToInterrupt(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs spell)
        {
            if (spell.DangerLevel < Interrupter2.DangerLevel.High || unit.IsAlly)
            {
                return;
            }

            if (E.CastCheck(unit, "Interrupt.R"))
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



        public override void InterruptMenu(Menu config)
        {
            config.AddBool("Interrupt.R", "Use R to Interrupt Spells", true);
        }
    }
}
