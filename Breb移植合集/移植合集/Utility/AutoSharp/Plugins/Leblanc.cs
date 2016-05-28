using System; using EloBuddy; using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Plugins
{
	public class Leblanc : PluginBase
	{
	    // ReSharper disable once RedundantDefaultMemberInitializer
		private bool _firstW = false;
		public Leblanc()
		{
			Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 720);
			Q.SetTargetted(0.5f, 1500f);

			W = new LeagueSharp.Common.Spell(SpellSlot.W, 670);
			W.SetSkillshot(0.6f, 220f, 1900f, false, SkillshotType.SkillshotCircle);

			E = new LeagueSharp.Common.Spell(SpellSlot.E, 900);
			E.SetSkillshot(0.3f, 80f, 1650f, true, SkillshotType.SkillshotLine);

			R = new LeagueSharp.Common.Spell(SpellSlot.R, 720);
		}

		public override void OnUpdate(EventArgs args)
		{
				            var target = TargetSelector.GetTarget(1400, DamageType.Magical);


            if (ComboMode)
            {

                if (Q.IsReady() && R.IsReady() && target.LSIsValidTarget(Q.Range))
                {
                    Q.Cast(target);
                    LeagueSharp.Common.Utility.DelayAction.Add(100, () => R.Cast(target));
                }

                if (W.IsReady() && target.LSIsValidTarget(W.Range) && !_firstW && (Player.HealthPercent > 30 || W.IsKillable(target)))
                {
                    W.Cast(target);
                    _firstW = true;
                }
                if (Q.IsReady() && target.LSIsValidTarget(Q.Range))
                {
                    Q.Cast(target);
                }
                if (R.IsReady() && target.LSIsValidTarget(Q.Range))
                {
                    R.Cast(target);
                }
                if (E.IsReady() && target.LSIsValidTarget(700))
                {
                    E.CastIfHitchanceEquals(Target, HitChance.High);
                }
                if (W.IsReady() && _firstW && Player.HealthPercent < 60)
                {
                    W.Cast();
                    _firstW = false;
                }

                if (W.Instance.State == SpellState.Cooldown)
                {
                    _firstW = false;
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
