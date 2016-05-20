using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iLucian.Utils
{
    using System.Text.RegularExpressions;

    using LeagueSharp;
    using EloBuddy;
    static class Jungle
    {
        #region Static Fields

        /// <summary>
        ///     The large name regex list.
        /// </summary>
        private static readonly string[] LargeNameRegex =
            {
                "SRU_Murkwolf[0-9.]{1,}", "SRU_Gromp", "SRU_Blue[0-9.]{1,}",
                "SRU_Razorbeak[0-9.]{1,}", "SRU_Red[0-9.]{1,}",
                "SRU_Krug[0-9]{1,}"
            };

        /// <summary>
        ///     The legendary name regex list.
        /// </summary>
        private static readonly string[] LegendaryNameRegex = { "SRU_Dragon", "SRU_Baron" };

        /// <summary>
        ///     The small name regex list.
        /// </summary>
        private static readonly string[] SmallNameRegex = { "Mini", "Sru_Crab" };

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Get the minion jungle type.
        /// </summary>
        /// <param name="minion">
        ///     The minion
        /// </param>
        /// <returns>
        ///     The <see cref="JungleType" />
        /// </returns>
        public static JungleType GetJungleType(this Obj_AI_Base minion)
        {
            if (SmallNameRegex.Any(regex => Regex.IsMatch(minion.Name, regex)))
            {
                return JungleType.Small;
            }

            if (LargeNameRegex.Any(regex => Regex.IsMatch(minion.Name, regex)))
            {
                return JungleType.Large;
            }

            if (LegendaryNameRegex.Any(regex => Regex.IsMatch(minion.Name, regex)))
            {
                return JungleType.Legendary;
            }

            return JungleType.Unknown;
        }

        /// <summary>
        ///     Indicates whether the object is a jungle buff carrier.
        /// </summary>
        /// <param name="minion">
        ///     The minion.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool IsJungleBuff(this Obj_AI_Base minion)
        {
            var @base = minion.CharData.BaseSkinName;
            return @base.Equals("SRU_Blue") || @base.Equals("SRU_Red");
        }

        #endregion
    }

    public enum JungleType
    {
        /// <summary>
        ///     The unknown type.
        /// </summary>
        Unknown,

        /// <summary>
        ///     The small type.
        /// </summary>
        Small,

        /// <summary>
        ///     The large type.
        /// </summary>
        Large,

        /// <summary>
        ///     The legendary type.
        /// </summary>
        Legendary
    }
}