using EloBuddy;
using LeagueSharp.Common;

namespace Feedlesticks.Core
{
    internal class Spells
    {
        /// <summary>
        ///     Spells
        /// </summary>
        public static Spell Q, W, E, R;

        /// <summary>
        ///     Ignite Slot
        /// </summary>
        public static readonly SpellSlot Ignite = ObjectManager.Player.GetSpellSlot("summonerdot");

        /// <summary>
        ///     Fiddle
        /// </summary>
        public static readonly AIHeroClient FiddleStick = ObjectManager.Player;

        /// <summary>
        ///     Ignite Ready Check
        /// </summary>
        public static bool Igniteable
        {
            get
            {
                return Ignite.IsReady() && Ignite != SpellSlot.Unknown &&
                       FiddleStick.Spellbook.CanUseSpell(Ignite) == SpellState.Ready;
            }
        }

        /// <summary>
        ///     Spell data
        /// </summary>
        public static void Init()
        {
            Q = new Spell(SpellSlot.Q, 575);
            W = new Spell(SpellSlot.W, 575);
            E = new Spell(SpellSlot.E, 750);
            R = new Spell(SpellSlot.R, 800);
        }
    }
}