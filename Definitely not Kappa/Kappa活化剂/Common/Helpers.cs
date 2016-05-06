namespace KappaUtility.Common
{
    using EloBuddy;
    using EloBuddy.SDK;

    public static class Helpers
    {
        public static bool IsKillable(this Obj_AI_Base target)
        {
            return !target.HasBuff("kindredrnodeathbuff") && !target.HasBuff("JudicatorIntervention") && !target.HasBuff("ChronoShift")
                   && !target.HasBuff("UndyingRage") && !target.IsInvulnerable && !target.IsZombie && !target.HasBuff("bansheesveil")
                   && !target.IsDead && !target.IsPhysicalImmune && target.Health > 0 && !target.HasBuffOfType(BuffType.Invulnerability)
                   && !target.HasBuffOfType(BuffType.PhysicalImmunity) && target.IsValidTarget();
        }
    }
}