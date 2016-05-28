using System;
using EloBuddy;
using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using System.Linq;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Plugins
{
	public class Teemo : PluginBase
	{

		public Teemo()
		{
			Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 680);
			W = new LeagueSharp.Common.Spell(SpellSlot.W);
			R = new LeagueSharp.Common.Spell(SpellSlot.R, 230);
			Q.SetTargetted(0f, 2000f);
			R.SetSkillshot(0.1f, 75f, float.MaxValue, false, SkillshotType.SkillshotCircle);
		}

	    public override void OnUpdate(EventArgs args)
	    {
	        var target = TargetSelector.GetTarget(900, DamageType.Magical);
	        if (target != null)
	        {
	            if (Q.IsReady() && Heroes.Player.LSDistance(Target) < Q.Range)
	            {
	                Q.Cast(target);
	            }
	        }
	        if (Player.Position.LSCountEnemiesInRange(325) != 0)
	        {
	            W.Cast();
	        }
	        if (R.IsReady() &&
	            (NavMesh.IsWallOfGrass(Player.ServerPosition, 100) ||
	             HealingBuffs.AllyBuffs.Any(h => h.Position.LSDistance(Player.ServerPosition) < 100) ||
	             HealingBuffs.EnemyBuffs.Any(h => h.Position.LSDistance(Player.ServerPosition) < 100)))
	        {
	            R.Cast(Player.Position);
	        }
	    }

	    public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboW", "Use W", true);
            config.AddBool("ComboR", "Use R", true);
        }
    }
}