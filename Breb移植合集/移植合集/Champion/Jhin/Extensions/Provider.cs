using System;
using EloBuddy;
using EloBuddy.SDK.Menu;
using LeagueSharp.Common;

namespace Jhin___The_Virtuoso.Extensions
{
    internal static class Provider
    {
        /// <summary>
        ///     Hitchance Name Array
        /// </summary>
        public static readonly string[] HitchanceNameArray = {"Low", "Medium", "High", "Very High", "Only Immobile"};

        /// <summary>
        ///     Hitchance Array
        /// </summary>
        private static readonly HitChance[] HitchanceArray =
        {
            HitChance.Low, HitChance.Medium, HitChance.High,
            HitChance.VeryHigh, HitChance.Immobile
        };

        /// <summary>
        ///     Thats gives if enemy stunnable with W or not
        /// </summary>
        /// <param name="unit">Target</param>
        /// <returns></returns>
        public static bool IsStunnable(this AIHeroClient unit)
        {
            return unit.HasBuff("jhinespotteddebuff");
        }

        /// <summary>
        ///     Thats check if spell name equals to value
        /// </summary>
        /// <param name="unit">Player</param>
        /// <param name="spell">Spell</param>
        /// <returns></returns>
        public static bool IsActive(this AIHeroClient unit, Spell spell)
        {
            return spell.Instance.Name == "JhinRShot";
        }

        /// <summary>
        ///     Thats gives me if enemy immobile or not for e stun root
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
                   unit.IsStunned || unit.IsChannelingImportantSpell();
        }

        /// <summary>
        ///     Thats gives me if unit has teleport buff
        /// </summary>
        /// <param name="unit">Unit</param>
        /// <returns></returns>
        public static bool HasTeleportBuff(this Obj_AI_Base unit)
        {
            return unit.HasBuff("teleport_target") && unit.Team != ObjectManager.Player.Team;
        }

        /// <summary>
        ///     Gives me current hitchance value
        /// </summary>
        /// <param name="menu">Main menu</param>
        /// <param name="menuName">Menu name</param>
        /// <returns></returns>
        public static HitChance HikiChance(this Menu menu, string menuName)
        {
            return HitchanceArray[Menus.getBoxItem(menu, menuName)];
        }

        /// <summary>
        ///     Gives me Jhin is reloading or not
        /// </summary>
        /// <param name="unit">Target</param>
        /// <returns></returns>
        public static bool IsReloading(this AIHeroClient unit)
        {
            return unit.HasBuff("JhinPassiveReload");
        }


        /// <summary>
        ///     Basic Attack Indicator
        /// </summary>
        /// <param name="enemy">Target</param>
        /// <returns></returns>
        public static int BasicAttackIndicator(AIHeroClient enemy)
        {
            var aCalculator = ObjectManager.Player.CalcDamage(enemy, DamageType.Physical,
                ObjectManager.Player.TotalAttackDamage);
            var killableAaCount = enemy.Health/aCalculator;
            var totalAa = (int) Math.Ceiling(killableAaCount);
            return totalAa;
        }
    }
}