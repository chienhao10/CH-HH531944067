using System; using EloBuddy; using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Plugins
{
    public class Ahri : PluginBase
    {
        public Ahri()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 900);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 800);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 875);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 850);

            Q.SetSkillshot(0.25f, 100, 1600, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 60, 1200, true, SkillshotType.SkillshotLine);
        }

        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode && Target != null)
            {
                if (E.IsReady() && Heroes.Player.LSDistance(Target) < E.Range)
                {
                    E.CastIfHitchanceEquals(Target, HitChance.High);
                }
                if (Q.IsReady() && Heroes.Player.LSDistance(Target) < Q.Range && Target.HasBuffOfType(BuffType.Charm))
                {
                    Q.Cast(Target);
                }
                if (Q.IsReady() && Heroes.Player.LSDistance(Target) < Q.Range)
                {
                    Q.Cast(Target, UsePackets);
                }
                if (W.IsReady() && Heroes.Player.LSDistance(Target) < W.Range)
                {
                    W.Cast();
                }
                if (R.IsReady() && ((!Q.IsReady() && !W.IsReady() && !E.IsReady()) || IsRActive()))
                {
                    R.Cast(Target);
                }
            }
            if (HarassMode && Target != null)
            {
                if (E.IsReady() && Heroes.Player.LSDistance(Target) < E.Range)
                {
                    E.CastIfHitchanceEquals(Target, HitChance.High);
                }
                if (Q.IsReady() && Heroes.Player.LSDistance(Target) < Q.Range && Target.HasBuffOfType(BuffType.Charm))
                {
                    Q.Cast(Target);
                }
            }
        }

        private bool IsRActive()
        {
            return ObjectManager.Player.HasBuff("AhriTumble");
        }

        public override void OnPossibleToInterrupt(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs spell)
        {
            if (spell.DangerLevel < Interrupter2.DangerLevel.High || unit.IsAlly)
            {
                return;
            }

            if (E.CastCheck(unit, "Interrupt.E"))
            {
                E.CastIfHitchanceEquals(unit, HitChance.Medium);
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
            config.AddBool("Interrupt.E", "Use E to Interrupt Spells", true);
        }
    }
}