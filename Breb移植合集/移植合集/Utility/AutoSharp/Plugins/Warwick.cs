using System; using EloBuddy; using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Plugins
{
	public class Warwick : PluginBase
	{
		public Warwick()
		{
			Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 400);
			W = new LeagueSharp.Common.Spell(SpellSlot.W, 1000);
			E = new LeagueSharp.Common.Spell(SpellSlot.E, 1500);
			R = new LeagueSharp.Common.Spell(SpellSlot.R, 700);
		}

		public override void OnUpdate(EventArgs args)
		{
		//	if (ComboMode)
		//	{
					 if (Target.HasBuffOfType(BuffType.Invulnerability))
            {
                return;
            }

                if (Q.IsReady() && Heroes.Player.LSDistance(Target) < Q.Range)
                {
                    Q.Cast(Target);
                }
                if (R.IsReady() && Heroes.Player.LSDistance(Target) < R.Range && R.IsKillable(Target))
                {
                    R.Cast(Target);
                }
                if (Player.HealthPercent > 20 && Player.LSDistance(Target) < 1000)
                {
                    if (W.IsReady())
                    {
                        W.Cast();
                    }
                }
				var allminions = MinionManager.GetMinions(ObjectManager.Player.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
				foreach(var minion in allminions)
                {
                    if (minion.Health < Player.LSGetSpellDamage(minion, SpellSlot.Q)) Q.Cast(minion);
                    return;
                }
          //  }
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