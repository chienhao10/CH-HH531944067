using EloBuddy;
using LeagueSharp;
using LeagueSharp.SDK;

namespace ExorSDK.Utilities
{
    /// <summary>
    ///     The Mana manager class.
    /// </summary>
    internal class ManaManager
    {
        /// <summary>
        ///     The minimum mana needed to cast the given Spell.
        /// </summary>
        public static int GetNeededHealth(SpellSlot slot, int value) => value + (int)(GameObjects.Player.Spellbook.GetSpell(slot).SData.Mana / GameObjects.Player.MaxHealth * 100);

        /// <summary>
        ///     The minimum mana needed to cast the given Spell.
        /// </summary>
        public static int GetNeededMana(SpellSlot slot, int value) => value + (int)(GameObjects.Player.Spellbook.GetSpell(slot).SData.Mana / GameObjects.Player.MaxMana * 100);
    }
}