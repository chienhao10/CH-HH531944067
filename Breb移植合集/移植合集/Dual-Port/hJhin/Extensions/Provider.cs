using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;

namespace hJhin.Extensions
{
    static class Provider
    {
        /// <summary>
        /// Thats gives if enemy stunnable with W or not
        /// </summary>
        /// <param name="unit">Target</param>
        /// <returns></returns>
        public static bool IsStunnable(this AIHeroClient unit)
        {
            return unit.HasBuff("jhinespotteddebuff");
        }
        /// <summary>
        /// Thats check if spell name equals to value
        /// </summary>
        /// <param name="unit">Player</param>
        /// <param name="spell">Spell</param>
        /// <returns></returns>
        public static bool IsActive(this AIHeroClient unit, Spell spell)
        {
            return spell.Instance.Name == "JhinRShot";
        }

        /// <summary>
        /// Thats gives me if enemy immobile or not for e stun root
        /// </summary>
        /// <param name="unit">Target</param>
        /// <returns></returns>
        public static bool IsEnemyImmobile(this AIHeroClient unit)
        {
            return unit.HasBuffOfType(BuffType.Stun) || unit.HasBuffOfType(BuffType.Snare) ||
                   unit.HasBuffOfType(BuffType.Knockup) ||
                   unit.HasBuffOfType(BuffType.Charm) || unit.HasBuffOfType(BuffType.Fear) ||
                   unit.HasBuffOfType(BuffType.Knockback) ||
                   unit.HasBuffOfType(BuffType.Taunt) || unit.HasBuffOfType(BuffType.Suppression) ||
                   unit.IsStunned;
        }

        /// <summary>
        /// Thats gives me if unit has teleport buff
        /// </summary>
        /// <param name="unit">Unit</param>
        /// <returns></returns>
        public static bool HasTeleportBuff(this Obj_AI_Base unit)
        {
            return unit.HasBuff("teleport_target") && unit.Team != ObjectManager.Player.Team;
        }

        /// <summary>
        /// Gives me Jhin is reloading or not 
        /// </summary>
        /// <param name="unit">Target</param>
        /// <returns></returns>
        public static bool IsReloading(this AIHeroClient unit)
        {
            return unit.HasBuff("JhinPassiveReload");
        }


        /// <summary>
        /// Basic Attack Indicator
        /// </summary>
        /// <param name="enemy">Target</param>
        /// <returns></returns>
        public static int BasicAttackIndicator(AIHeroClient enemy)
        {
            var aCalculator = ObjectManager.Player.CalculateDamage(enemy, DamageType.Physical, ObjectManager.Player.TotalAttackDamage);
            var killableAaCount = enemy.Health / aCalculator;
            var totalAa = (int)Math.Ceiling(killableAaCount);
            return totalAa;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        public static HitChance HikiChance()
        {
            if (getBoxItem(Config.hitchanceMenu, "hitchance") == 0)
            {
                return HitChance.Medium;
            }

            if (getBoxItem(Config.hitchanceMenu, "hitchance") == 1)
            {
                return HitChance.High;
            }

            if (getBoxItem(Config.hitchanceMenu, "hitchance") == 2)
            {
                return HitChance.VeryHigh;
            }
            return HitChance.Low;
        }
    }
}
