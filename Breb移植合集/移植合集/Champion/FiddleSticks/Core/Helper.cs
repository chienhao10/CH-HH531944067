using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;

namespace Feedlesticks.Core
{
    internal class Helper
    {
        /// <summary>
        ///     w buff checker
        /// </summary>
        public static bool IsWActive
        {
            get { return ObjectManager.Player.HasBuff("fiddlebuff"); }
        }

        /// <summary>
        ///     Enemy Immobile
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool IsEnemyImmobile(AIHeroClient target)
        {
            if (target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare) ||
                target.HasBuffOfType(BuffType.Knockup) ||
                target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Fear) ||
                target.HasBuffOfType(BuffType.Knockback) ||
                target.HasBuffOfType(BuffType.Taunt) || target.HasBuffOfType(BuffType.Suppression) ||
                target.IsStunned || target.IsChannelingImportantSpell())
            {
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Process spell cast. thats need for last w game time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (Spells.Wable && IsWActive && args.Slot == SpellSlot.W)
            {
                Spells.LastW = Game.Time;
            }
        }

        /// <summary>
        ///     W Lock
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (IsWActive && sender.Owner.IsMe && Spells.Wable)
            {
                if (args.Slot == SpellSlot.W)
                {
                    args.Process = false;
                    Orbwalker.DisableAttacking = true;
                    Orbwalker.DisableMovement = true;
                }
                else
                {
                    args.Process = true;
                    Orbwalker.DisableAttacking = false;
                    Orbwalker.DisableMovement = false;
                }
            }
        }
    }
}