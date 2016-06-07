using System.Linq;
using EloBuddy;
using LeagueSharp.SDK.Core.Utils;

namespace NabbActivator
{
    /// <summary>
    ///     The resetters class.
    /// </summary>
    internal class Resetters
    {
        /// <summary>
        ///     Sets the resetter slots.
        /// </summary>
        public static void Initialize()
        {
            if (ObjectManager.Player.Spellbook.Spells.Any(s => AutoAttack.IsAutoAttackReset(s.Name.ToLower())))
            {
                Vars.HasAnyReset = true;
            }
        }
    }
}