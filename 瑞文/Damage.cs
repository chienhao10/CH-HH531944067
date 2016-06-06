using System;
using EloBuddy;
using EloBuddy.SDK;

namespace Blessed_Riven
{
    class Damage
    {
        public static double ComboDamage(Obj_AI_Base target, bool noR = false)
        {
            double dmg = 0;
            var passiveStacks = 0;

            dmg += Program.Q.IsReady()
                ? QDamage(!noR) * (3 - Program.QCount)
                : 0;
            passiveStacks += Program.Q.IsReady()
                ? (3 - Program.QCount)
                : 0;

            dmg += Program.W.IsReady()
                ? WDamage()
                : 0;
            passiveStacks += Program.W.IsReady()
                ? 1
                : 0;
            passiveStacks += Program.E.IsReady()
                ? 1
                : 0;

            dmg += PassiveDamage() * passiveStacks;
            dmg += (Program.R.IsReady() && !noR && !Player.Instance.HasBuff("RivenFengShuiEngine")
                ? Player.Instance.TotalAttackDamage * 1.2
                : Player.Instance.TotalAttackDamage) * passiveStacks;

            if (dmg < 10)
            {
                return 3 * Player.Instance.TotalAttackDamage;
            }

            dmg += Program.R.IsReady() && !noR
                ? RDamage(target, Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, (float)dmg))
                : 0;
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, (float)dmg);
        }

        public static float QDamage(bool useR = false)
        {
            return (float)(new double[] { 10, 30, 50, 70, 90 }[Program.Q.Level - 1] +
                            ((Program.R.IsReady() && useR && !Player.Instance.HasBuff("RivenFengShuiEngine")
                                ? Player.Instance.TotalAttackDamage * 1.2
                                : Player.Instance.TotalAttackDamage) / 100) *
                            new double[] { 40, 45, 50, 55, 60 }[Program.Q.Level - 1]);
        }

        public static float WDamage()
        {
            return (float)(new double[] { 50, 80, 110, 140, 170 }[Program.W.Level - 1] +
                            1 * ObjectManager.Player.FlatPhysicalDamageMod);
        }

        public static double PassiveDamage()
        {
            return ((20 + ((Math.Floor((double)ObjectManager.Player.Level / 3)) * 5)) / 100) *
                   (ObjectManager.Player.BaseAttackDamage + ObjectManager.Player.FlatPhysicalDamageMod);
        }

        public static float RDamage(Obj_AI_Base target, double healthMod = 0)
        {
            if (!Program.R.IsLearned) return 0;
            var hpPercent = (target.Health - healthMod > 0 ? 1 : target.Health - healthMod) / target.MaxHealth;
            return (float)((new double[] { 80, 120, 160 }[Program.R.Level - 1]
                             + 0.6 * Player.Instance.FlatPhysicalDamageMod) *
                            (hpPercent < 25 ? 3 : (((100 - hpPercent) * 2.67) / 100) + 1));
        }

    }
}
