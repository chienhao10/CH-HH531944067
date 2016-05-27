namespace KappAzir.Modes
{
    using System.Linq;

    using EloBuddy.SDK;

    using KappAzir.Utility;

    internal class Killsteal
    {
        public static void Execute()
        {
            var menu = Menus.KillstealMenu;
            var enemies = EntityManager.Heroes.Enemies.Where(e => e.IsKillable());

            foreach (var enemy in enemies)
            {
                if (enemy != null)
                {
                    var Q = menu.checkbox("Q") && Azir.Q.IsReady() && Orbwalker.AzirSoldiers.Count(s => s.IsAlly) > 0
                            && enemy.IsValidTarget(Azir.Q.Range)
                            && Azir.Q.GetDamage(enemy) >= Prediction.Health.GetPrediction(enemy, Azir.Q.CastDelay);
                    var E = menu.checkbox("E") && Azir.E.IsReady() && enemy.Ehit() && enemy.IsValidTarget(Azir.E.Range)
                            && Azir.E.GetDamage(enemy) >= Prediction.Health.GetPrediction(enemy, Azir.E.CastDelay);
                    var R = menu.checkbox("R") && Azir.R.IsReady() && enemy.IsValidTarget(Azir.R.Range)
                            && Azir.R.GetDamage(enemy) >= Prediction.Health.GetPrediction(enemy, Azir.R.CastDelay);

                    if (Q)
                    {
                        Azir.Q.Cast(enemy);
                    }
                    if (E)
                    {
                        Azir.E.Cast(enemy);
                    }
                    if (R && !Q && !E)
                    {
                        Azir.R.Cast(enemy);
                    }
                }
            }
        }
    }
}