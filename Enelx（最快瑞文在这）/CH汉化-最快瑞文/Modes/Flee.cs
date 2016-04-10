using EloBuddy;
using EloBuddy.SDK;
using Ex1L_Riven.Base;

namespace Ex1L_Riven.Modes
{
    internal class Flee
    {
        public static bool ShouldBeExecuted()
        {
            return ModeController.Activemode(Orbwalker.ActiveModes.Flee);
        }

        public static void Execute()
        {
            var target = TargetSelector.GetTarget(Spells.W.Range, DamageType.Physical);

            if (Items.Youmuus.IsOwned() && Items.Youmuus.IsReady() && Player.Instance.HealthPercent < 50 &&
                Player.Instance.CountEnemiesInRange(Spells.R2.Range) >= 2)
            {
                Items.Youmuus.Cast();
            }

            if (Spells.W.IsReady() && target.IsValidTarget())
            {
                Spells.W.Cast();
            }

            if (Spells.E.IsReady())
            {
                Player.CastSpell(SpellSlot.E, Game.CursorPos + 300);
            }

            if (Spells.Q.IsReady())
            {
                Player.CastSpell(SpellSlot.Q, Game.CursorPos);
            }
        }
    }
}