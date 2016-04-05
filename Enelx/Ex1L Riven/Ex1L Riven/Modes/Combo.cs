using EloBuddy;
using EloBuddy.SDK;
using Ex1L_Riven.Base;

namespace Ex1L_Riven.Modes
{
    internal class Combo
    {
        public static bool ShouldBeExecuted()
        {
            return ModeController.Activemode(Orbwalker.ActiveModes.Combo);
        }

        public static void Execute()
        {
            var target = TargetSelector.GetTarget(1000, DamageType.Physical);

            if (target == null || target.IsZombie || target.IsInvulnerable) return;

            if (target.IsValidTarget())
            {
                if (Variables.UseItems)
                {
                    Items.CastItems(target);
                }

                Logic.CastE(target);
                Logic.CastW(target);
                Logic.CastR(target);
                Logic.CastQaa2(target);
                Logic.CastTiaHydra(target);
            }
        }
    }
}