using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using Ex1L_Riven.Base;

namespace Ex1L_Riven.Modes
{
    internal class Jungle
    {
        public static bool ShouldBeExecuted()
        {
            return ModeController.Activemode(Orbwalker.ActiveModes.JungleClear);
        }

        public static void Execute()
        {
            var monster = Logic.Monsters(Spells.E.Range, Player.Instance.ServerPosition).FirstOrDefault();

            if (monster == null || !monster.IsValidTarget()) return;

            if (Variables.UseJungleE && Spells.E.IsReady())
            {
                Spells.E.Cast(monster.ServerPosition);
            }

            if (Items.Tiamat.IsReady() && monster.IsValidTarget(Items.Tiamat.Range))
            {
                Items.Tiamat.Cast();
            }
            else if (Items.Hydra.IsReady() && monster.IsValidTarget(Items.Hydra.Range))
            {
                Items.Hydra.Cast();
            }

            if (Variables.UseJungleW && Spells.W.IsReady() && monster.IsValidTarget(Spells.W.Range))
            {
                Spells.W.Cast();
            }
        }
    }
}