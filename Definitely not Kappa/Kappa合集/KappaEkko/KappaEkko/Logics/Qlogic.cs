namespace KappaEkko.Logics
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;

    internal class Qlogic
    {
        public static void OnStacks()
        {
            var target =
                ObjectManager.Get<AIHeroClient>()
                    .FirstOrDefault(
                        enemy =>
                        enemy != null && enemy.IsEnemy && enemy.IsValidTarget(Spells.Q.Range) && !enemy.IsDead
                        && !enemy.HasBuff("kindredrnodeathbuff") && !enemy.HasBuff("JudicatorIntervention") && !enemy.HasBuff("ChronoShift")
                        && !enemy.HasBuff("UndyingRage") && enemy.GetBuffCount("EkkoStacks") > 1);

            if (target != null)
            {
                Spells.Q.Cast(target.Position);
            }
        }
    }
}