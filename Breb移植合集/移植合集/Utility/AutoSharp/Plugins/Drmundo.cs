using System;
using EloBuddy;
using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Plugins
{
	public class DrMundo : PluginBase
	{
		public DrMundo()
		{
			Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 930);
			W = new LeagueSharp.Common.Spell(SpellSlot.W, 320);
			E = new LeagueSharp.Common.Spell(SpellSlot.E, 225);
			R = new LeagueSharp.Common.Spell(SpellSlot.R, 0);

			Q.SetSkillshot(0.50f, 75f, 1500f, true, SkillshotType.SkillshotLine);
		}

		public override void OnUpdate(EventArgs args)
		{
							var target1 = TargetSelector.GetTarget(1000, DamageType.Magical);
			if (target1==null) return;
            if (ComboMode)
            {
                if (Player.HealthPercent < 50 && R.IsReady())
                {
                    R.Cast();
                }
                Combo(target1);
            }

                if (Q.IsReady() && Heroes.Player.LSDistance(Target) < Q.Range)
                {
                    Q.CastIfHitchanceEquals(target1, HitChance.High);
                }
            
        }

        //from mundo TheKushStyle
        private void Combo(AIHeroClient target)
        {

            if (Q.IsReady() && Heroes.Player.LSDistance(Target) < Q.Range)
            {
                Q.CastIfHitchanceEquals(Target, HitChance.High);
            }

            if (target.LSIsValidTarget() && W.IsReady() && Player.LSDistance(target) <= 500 && !Player.HasBuff("BurningAgony"))
            {
                W.Cast();
            }
            if (target.LSIsValidTarget() && Player.LSDistance(target) > 500 && Player.HasBuff("BurningAgony"))
            {
                W.Cast();
            }

            if (E.IsReady() && Player.LSDistance(target) <= 700)
            {
                E.Cast();
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