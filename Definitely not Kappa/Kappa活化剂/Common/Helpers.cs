namespace KappaUtility.Common
{
    using EloBuddy;

    public static class Helpers
    {
        public static bool IsKillable(this Obj_AI_Base target)
        {
            if (target != null)
            {
                if (target.HasBuff("kindredrnodeathbuff") || target.HasBuff("JudicatorIntervention") || target.HasBuff("ChronoShift")
                    || target.HasBuff("UndyingRage") || target.IsInvulnerable || target.IsZombie)
                {
                    return false;
                }
                return true;
            }
            return true;
        }
    }
}