using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using Ex1L_Riven.Base;

namespace Ex1L_Riven.Modes
{
    internal class Lane
    {
        public static bool ShouldBeExecuted()
        {
            return ModeController.Activemode(Orbwalker.ActiveModes.LaneClear);
        }

        public static void Execute()
        {
            if (Variables.UseLaneW && Spells.W.IsReady())
            {
                var minions =
                    EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, radius: Spells.W.Range)
                        .ToArray();
                if (minions.Length >= Variables.MinMinionsW)
                {
                    var killable =
                        minions.Count(
                            minion => Spells.WDamage(minion) > minion.Health);

                    if (killable >= Variables.MinMinionsW)
                    {
                        Spells.W.Cast();
                    }
                }
            }

            if (Variables.UseLaneE && Spells.E.IsReady())
            {
                var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy,
                    Player.Instance.ServerPosition, Spells.E.Range)
                    .Where(m => m.Distance(Player.Instance) > Player.Instance.GetAutoAttackRange(m)).ToList();

                if (minions.Count() > 1)
                {
                    var position = EntityManager.MinionsAndMonsters.GetLineFarmLocation(minions, Spells.E.Width,
                        (int) Spells.E.Range);

                    Spells.E.Cast(position.CastPosition);
                }
            }
        }
    }
}