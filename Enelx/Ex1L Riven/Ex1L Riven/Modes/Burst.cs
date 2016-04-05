using EloBuddy;
using EloBuddy.SDK;
using Ex1L_Riven.Base;

namespace Ex1L_Riven.Modes
{
    internal class Burst
    {
        public static bool ShouldBeExecuted()
        {
            return ModeController.Activemode(Orbwalker.ActiveModes.Combo);
        }

        public static void Execute()
        {
            var target = TargetSelector.SelectedTarget;

            if (target == null || target.IsZombie || target.IsInvulnerable) return;

            if (target.IsValidTarget(800) && Variables.UseFlashBurst)
            {
                if (Variables.UseItems)
                {
                    Items.CastItems(target);
                }

                if (Spells.E.IsReady())
                {
                    Player.CastSpell(SpellSlot.E, target.ServerPosition);
                }

                if (Spells.R1.IsReady() && Riven.R1Activated == false)
                {
                    Spells.R1.Cast();
                }

                if (Spells.Flash.IsReady())
                {
                    Spells.Flash.Cast(target.ServerPosition);
                }

                Logic.CastTiaHydra(target);
                Logic.CastW(target);

                if (Spells.R2.IsReady() && Riven.R1Activated)
                {
                    Spells.R2.Cast(target.ServerPosition);
                }

                Logic.CastQ(target);
            }

            if ((Player.Instance.Distance(target) <= Spells.E.Range) && Spells.E.IsReady() && !Spells.Flash.IsReady())
            {
                if (Variables.UseItems)
                {
                    Items.CastItems(target);
                }

                Logic.CastE(target);
                Logic.CastR(target);
                Logic.CastTiaHydra(target);
                Logic.CastW(target);
                if (Spells.R2.IsReady() && Riven.R1Activated)
                {
                    Spells.R2.Cast(target.ServerPosition);
                }

                Logic.CastQ(target);
            }
        }
    }
}