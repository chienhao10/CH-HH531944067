using System; using EloBuddy; using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Plugins
{
    public class Diana : PluginBase
    {
        public Diana()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 830f);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 200f);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 420f);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 825f);

            Q.SetSkillshot(0.35f, 200f, 1800, false, SkillshotType.SkillshotCircle);
        }

        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {
                if (Target.LSIsValidTarget(Q.Range) && Q.IsReady() && Q.GetPrediction(Target).Hitchance >= HitChance.High)
                {
                    Q.CastIfHitchanceEquals(Target, HitChance.High);
                }

                if (Target.LSIsValidTarget(R.Range) && R.IsReady() && (Target.HasBuff("dianamoonlight")))
                {
                    R.Cast(Target);
                }

                if (Target.LSIsValidTarget(W.Range) && W.IsReady() && !Q.IsReady())
                {
                    W.Cast();
                }
                if (Target.LSIsValidTarget(E.Range) && Player.LSDistance(Target) >= W.Range && E.IsReady() && !W.IsReady())
                {
                    E.Cast();
                }
                if (Target.LSIsValidTarget(R.Range) && R.IsReady() && !W.IsReady() && !Q.IsReady())
                {
                    R.Cast(Target);
                }
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