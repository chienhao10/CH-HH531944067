namespace AurelionSol
{
    using EloBuddy;
    using EloBuddy.SDK;

    internal class Damage
    {
        internal static double R(Obj_AI_Base target)
        {
            return Player.Instance.CalculateDamageOnUnit(
                target,
                DamageType.Magical,
                (float)new double[] { 200, 400, 600 }[Program.R.Level - 1] + 0.70f * Player.Instance.TotalMagicalDamage);
        }

        internal static double Q(Obj_AI_Base target)
        {
            return Player.Instance.CalculateDamageOnUnit(
                target,
                DamageType.Magical,
                (float)new double[] { 70, 110, 150, 190, 230 }[Program.Q.Level - 1] + 0.65f * Player.Instance.TotalMagicalDamage);
        }
    }
}