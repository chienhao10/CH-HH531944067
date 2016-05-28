using System; using EloBuddy; using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using System.Linq;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Plugins
{
	public class Katarina : PluginBase
	{
		public Katarina()
		{
			Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 675);
			W = new LeagueSharp.Common.Spell(SpellSlot.W, 375);
			E = new LeagueSharp.Common.Spell(SpellSlot.E, 700);
			R = new LeagueSharp.Common.Spell(SpellSlot.R, 550);

			Q.SetTargetted(400, 1400);
			R.SetCharged("KatarinaR", "KatarinaR", 550, 550, 1.0f);
		}

	    public override void OnUpdate(EventArgs args)
	    {
	        KS();
	        var target = TargetSelector.GetTarget(900, DamageType.Magical);

	        if (ComboMode)
	        {
	            if ((target.HealthPercent < 60))
	            {
	                E.Cast(target);

	            }
	            Q.Cast(target);
	            if ((target.HealthPercent < 40))
	            {
	                E.Cast(target);

	            }

	            if (W.IsReady() && Target.LSIsValidTarget(W.Range))
	            {
	                W.Cast();
	            }
	            if (Player.LSDistance(target.ServerPosition) <= E.Range && (target.Health < 1000))
	            {
	                if (E.IsReady() && Heroes.Player.LSDistance(Target) < E.Range)
	                {
	                    E.Cast(target);

	                }
	            }
	            if (R.IsReady() && Target.LSIsValidTarget(R.Range))
	            {
	                if (R.IsKillable(Target))
	                {
	                    R.Cast();
	                }
	                R.CastIfWillHit(Target, 3);
	            }

	        }
	    }



	    // ReSharper disable once InconsistentNaming
	    public void KS()
	    {

	        foreach (
	            var target in
	                ObjectManager.Get<AIHeroClient>()
	                    .Where(
	                        x =>
	                            Player.LSDistance(x) < 1000 && x.LSIsValidTarget() && x.IsEnemy &&
	                            !x.IsDead))
	        {
	            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
	            if (target != null)
	            {

	                if (Player.LSDistance(target.ServerPosition) <= E.Range &&
	                    (Player.LSGetSpellDamage(target, SpellSlot.E)) > target.Health)
	                {
	                    if (E.IsReady() && Heroes.Player.LSDistance(Target) < E.Range)
	                    {
	                        E.Cast(target);

	                    }
	                }
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
