using EloBuddy;
using EloBuddy.SDK;
using System;
using System.Linq;

using Settings = JokerFioraBuddy.Config.Modes.LastHit;
namespace JokerFioraBuddy.Modes
{
    public sealed class LastHit : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit);
        }

        public override void Execute()
        {
            if (Settings.UseQ)
            {
                var minions = ObjectManager.Get<Obj_AI_Base>().OrderBy(m => m.Health).Where(m => m.IsMinion && m.IsEnemy && !m.IsDead);

                foreach (var minion in minions)
                {
                    if (Player.Instance.ManaPercent >= Settings.Mana && Q.IsReady() && minion.Health <= Player.Instance.GetSpellDamage(minion, SpellSlot.Q) && minion.Distance(Player.Instance) > Player.Instance.AttackRange)
                        Q.Cast(minion);
                }
            }
        }
    }
}
