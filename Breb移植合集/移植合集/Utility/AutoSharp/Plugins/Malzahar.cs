using System; using EloBuddy; using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Plugins
{
    public class Malzahar : PluginBase
    {
        public Malzahar()
        {
            //spelldata from Mechanics-StackOverflow Galio
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 850);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 800);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 650);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 700);

            Q.SetSkillshot(.5f, 30, 1600, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(0.50f, 50, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }

        public override void OnUpdate(EventArgs args)
        {
            if (ComboMode)
            {

                if (Q.IsReady() && Heroes.Player.LSDistance(Target) < Q.Range && Q.GetPrediction(Target).Hitchance >= HitChance.High)
                {
                    Q.Cast(Target);
                }
                if (E.IsReady() && Heroes.Player.LSDistance(Target) < E.Range)
                {
                    E.Cast(Target);
                }

                if (W.IsReady() && Target.LSIsValidTarget(W.Range))
                {
                    W.Cast(Target);
                }
                if (R.IsReady() && Target.LSIsValidTarget(R.Range))
                {
                    R.Cast(Target);
                }

            }
            if (HarassMode)
            {
                if (Q.IsReady() && Heroes.Player.LSDistance(Target) < Q.Range && Q.GetPrediction(Target).Hitchance >= HitChance.High)
                {
                    Q.Cast(Target);
                }
                if (E.IsReady() && Heroes.Player.LSDistance(Target) < E.Range)
                {
                    E.Cast(Target);
                }

                if (W.IsReady() && Target.LSIsValidTarget(W.Range))
                {
                    W.Cast(Target);
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