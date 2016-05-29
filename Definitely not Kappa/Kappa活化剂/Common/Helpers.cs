namespace KappaUtility.Common
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using EloBuddy;
    using EloBuddy.SDK;

    public static class Helpers
    {
        public static bool IsKillable(this Obj_AI_Base target)
        {
            try
            {
                return !target.HasBuff("kindredrnodeathbuff") && !target.HasBuff("JudicatorIntervention") && !target.HasBuff("ChronoShift")
                       && !target.HasBuff("UndyingRage") && !target.IsInvulnerable && !target.IsZombie && !target.HasBuff("bansheesveil")
                       && !target.IsDead && !target.IsPhysicalImmune && target.Health > 0 && !target.HasBuffOfType(BuffType.Invulnerability)
                       && !target.HasBuffOfType(BuffType.PhysicalImmunity) && target.IsValidTarget();
            }
            catch (Exception e)
            {
                Log(e.ToString());
            }
            return true;
        }

        public static float SmiteDamage(Obj_AI_Base target)
        {
            try
            {
                var Level = Player.Instance.Level;
                var dmg = 0f;

                if (target is AIHeroClient)
                {
                    dmg += new float[] { 28, 36, 44, 52, 60, 68, 76, 84, 92, 100, 108, 116, 124, 132, 140, 148, 156, 166 }[Level - 1];
                    return Player.Instance.CalculateDamageOnUnit(target, DamageType.True, dmg);
                }
                dmg += new float[] { 390, 410, 430, 450, 480, 510, 540, 570, 600, 640, 680, 720, 760, 800, 850, 900, 950, 1000 }[Level - 1];
                return Player.Instance.CalculateDamageOnUnit(target, DamageType.True, dmg);
            }
            catch (Exception e)
            {
                Log(e.ToString());
            }
            return 0f;
        }

        public static float CountEnemies(int range)
        {
            return EntityManager.Heroes.Enemies.Count(e => e.IsValidTarget(range) && !e.IsDead && e.IsVisible);
        }   

        public static void Log(string message, [CallerFilePath] string file = null, [CallerLineNumber] int line = 0)
        {
            Console.WriteLine("{0} ({1}): {2}", Path.GetFileName(file), line, message);
        }
    }
}