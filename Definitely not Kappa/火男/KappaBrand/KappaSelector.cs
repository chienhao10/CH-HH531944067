namespace KappaBrand
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;

    internal static class KappaSelector
    {
        public static AIHeroClient SelectedTarget = TargetSelector.SelectedTarget;

        public static AIHeroClient SelectTarget(float range)
        {
            if (SelectedTarget == null)
            {
                if (Brand.tsmode.CurrentValue == 0)
                {
                    switch (Brand.tselect.CurrentValue)
                    {
                        case 0:
                            {
                                return
                                    EntityManager.Heroes.Enemies.OrderByDescending(e => e.GetBuffCount("BrandAblaze"))
                                        .ThenByDescending(TargetSelector.GetPriority)
                                        .FirstOrDefault(e => e.IsValidTarget(range) && e.IsKillable() && e.IsVisible);
                            }

                        case 1:
                            {
                                return
                                    EntityManager.Heroes.Enemies.OrderBy(
                                        e => e.TotalShieldHealth() / Player.Instance.CalculateDamageOnUnit(e, DamageType.Magical, 100))
                                        .ThenByDescending(TargetSelector.GetPriority)
                                        .FirstOrDefault(e => e.IsValidTarget(range) && e.IsKillable() && e.IsVisible);
                            }

                        case 2:
                            {
                                return
                                    EntityManager.Heroes.Enemies.OrderBy(e => e.Distance(Game.CursorPos))
                                        .FirstOrDefault(e => e.IsValidTarget(range) && e.IsVisible);
                            }
                    }
                }
                return TargetSelector.GetTarget(range, DamageType.Magical);
            }
            return SelectedTarget;
        }
    }
}