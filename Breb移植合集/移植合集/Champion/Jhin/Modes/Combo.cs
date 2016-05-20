using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using Jhin___The_Virtuoso.Extensions;
using LeagueSharp.Common;
using Spell = LeagueSharp.Common.Spell;

namespace Jhin___The_Virtuoso.Modes
{
    internal static class Combo
    {
        /// <summary>
        ///     W Minimum Range
        /// </summary>
        private static readonly int MinRange = Menus.getSliderItem(Menus.wMenu, "w.combo.min.distance");

        /// <summary>
        ///     W Maximum Range
        /// </summary>
        private static readonly int MaxRange = Menus.getSliderItem(Menus.wMenu, "w.combo.max.distance");

        /// <summary>
        ///     Basit spell execute
        /// </summary>
        /// <param name="spell">Spell</param>
        public static void Execute(this Spell spell)
        {
            foreach (var enemy in HeroManager.Enemies.Where(o => o.IsValidTarget(spell.Range)))
            {
                spell.Cast(enemy);
            }
        }

        /// <summary>
        ///     Q Logic
        /// </summary>
        public static void ExecuteQ()
        {
            foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.Q.Range) && !x.IsReloading()))
            {
                Spells.Q.CastOnUnit(enemy);
            }
        }

        /// <summary>
        ///     W Logic
        /// </summary>
        public static void ExecuteW()
        {
            if (Menus.getCheckBoxItem(Menus.wMenu, "w.passive.combo"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.W.Range) &&
                                                                     (x.IsStunnable() || x.IsEnemyImmobile())))
                {
                    Spells.W.Cast(enemy);
                }
            }
            else
            {
                foreach (
                    var enemy in HeroManager.Enemies.Where(x => x.IsValid && x.LSDistance(ObjectManager.Player) < MaxRange
                                                                && x.LSDistance(ObjectManager.Player) > MinRange &&
                                                                Spells.W.GetPrediction(x).Hitchance >=
                                                                Menus.wMenu.HikiChance("w.hit.chance")
                                                                && !x.IsDead && !x.IsZombie))
                {
                    Spells.W.Cast(enemy);
                }
            }
        }

        /// <summary>
        ///     E Logic
        /// </summary>
        public static void ExecuteE()
        {
            foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.E.Range) && x.IsEnemyImmobile())
                )
            {
                var pred = Spells.E.GetPrediction(enemy);
                if (pred.Hitchance >= Menus.eMenu.HikiChance("e.hit.chance"))
                {
                    Spells.E.Cast(enemy);
                }
            }
        }

        /// <summary>
        ///     Execute all combo
        /// </summary>
        public static void ExecuteCombo()
        {
            if (Spells.Q.IsReady() && Menus.getCheckBoxItem(Menus.qMenu, "q.combo"))
            {
                ExecuteQ();
            }
            if (Spells.W.IsReady() && Menus.getCheckBoxItem(Menus.wMenu, "w.combo"))
            {
                ExecuteW();
            }
            if (Spells.E.IsReady() && Menus.getCheckBoxItem(Menus.eMenu, "e.combo"))
            {
                ExecuteE();
            }
        }
    }
}