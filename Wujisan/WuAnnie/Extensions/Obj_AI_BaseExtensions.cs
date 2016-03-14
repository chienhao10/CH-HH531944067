using System.Linq;
using EloBuddy.SDK;
using EloBuddy;

namespace WuAIO.Extensions
{
    public static class Obj_AI_BaseExtensions
    {
        public static int CountAlliesInRange(this Obj_AI_Base unit, int range)
        {
            return EntityManager.Heroes.Allies.Where(it => !it.IsDead && it.IsValidTarget() && it.Distance(unit) <= range).Count();
        }

        public static bool CanMove(this Obj_AI_Base target)
        {
            if (target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Knockup) || target.HasBuffOfType(BuffType.Sleep) ||
                target.HasBuffOfType(BuffType.Snare) || target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Suppression) || target.HasBuffOfType(BuffType.Taunt)) return false;

            return true;
        }
    }
}
