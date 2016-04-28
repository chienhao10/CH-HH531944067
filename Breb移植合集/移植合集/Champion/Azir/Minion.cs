using ClipperLib;
using Color = System.Drawing.Color;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;
using EloBuddy;
using Font = SharpDX.Direct3D9.Font;
using LeagueSharp.Common.Data;
using LeagueSharp.Common;
using SharpDX.Direct3D9;
using SharpDX;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Security.AccessControl;
using System;
using System.Speech.Synthesis;
using System.Text.RegularExpressions;

namespace HeavenStrikeAzir
{
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
    [Flags]
    public enum MinionTypes
    {
        /// <summary>
        ///     The unknown type.
        /// </summary>
        Unknown = 0,

        /// <summary>
        ///     The normal type.
        /// </summary>
        Normal = 1 << 0,

        /// <summary>
        ///     The ranged type.
        /// </summary>
        Ranged = 1 << 1,

        /// <summary>
        ///     The melee type.
        /// </summary>
        Melee = 1 << 2,

        /// <summary>
        ///     The siege type.
        /// </summary>
        Siege = 1 << 3,

        /// <summary>
        ///     The super type.
        /// </summary>
        Super = 1 << 4,

        /// <summary>
        ///     The ward type.
        /// </summary>
        Ward = 1 << 5
    }
    public static class Minion
    {
        #region Static Fields

        private static readonly List<string> CloneList = new List<string> { "leblanc", "shaco", "monkeyking" };

        /// <summary>
        ///     The normal minion list.
        /// </summary>
        private static readonly List<string> NormalMinionList = new List<string>
                                                                    {
                                                                        "SRU_ChaosMinionMelee", "SRU_ChaosMinionRanged",
                                                                        "SRU_OrderMinionMelee", "SRU_OrderMinionRanged",
                                                                        "HA_ChaosMinionMelee", "HA_ChaosMinionRanged",
                                                                        "HA_OrderMinionMelee", "HA_OrderMinionRanged"
                                                                    };

        private static readonly List<string> PetList = new List<string>
                                                           {
                                                               "annietibbers", "elisespiderling", "heimertyellow",
                                                               "heimertblue", "malzaharvoidling", "shacobox",
                                                               "yorickspectralghoul", "yorickdecayedghoul",
                                                               "yorickravenousghoul", "zyrathornplant",
                                                               "zyragraspingplant"
                                                           };

        /// <summary>
        ///     The siege minion list.
        /// </summary>
        private static readonly List<string> SiegeMinionList = new List<string>
                                                                   {
                                                                       "SRU_ChaosMinionSiege", "SRU_OrderMinionSiege",
                                                                       "HA_ChaosMinionSiege", "HA_OrderMinionSiege"
                                                                   };

        /// <summary>
        ///     The super minion list.
        /// </summary>
        private static readonly List<string> SuperMinionList = new List<string>
                                                                   {
                                                                       "SRU_ChaosMinionSuper", "SRU_OrderMinionSuper",
                                                                       "HA_ChaosMinionSuper", "HA_OrderMinionSuper"
                                                                   };

        #endregion

        #region Public Methods and Operators


        /// <summary>
        ///     Gets the minion type.
        /// </summary>
        /// <param name="minion">
        ///     The minion.
        /// </param>
        /// <returns>
        ///     The <see cref="MinionTypes" />
        /// </returns>
        public static MinionTypes GetMinionType(this Obj_AI_Minion minion)
        {
            var baseSkinName = minion.CharData.BaseSkinName;

            if (NormalMinionList.Any(n => baseSkinName.Equals(n)))
            {
                return MinionTypes.Normal
                       | (minion.CharData.BaseSkinName.Contains("Melee") ? MinionTypes.Melee : MinionTypes.Ranged);
            }

            if (SiegeMinionList.Any(n => baseSkinName.Equals(n)))
            {
                return MinionTypes.Siege | MinionTypes.Ranged;
            }

            if (SuperMinionList.Any(n => baseSkinName.Equals(n)))
            {
                return MinionTypes.Super | MinionTypes.Melee;
            }

            if (baseSkinName.ToLower().Contains("ward") || baseSkinName.ToLower().Contains("trinket"))
            {
                return MinionTypes.Ward;
            }

            return MinionTypes.Unknown;
        }

        /// <summary>
        ///     Tells whether the <see cref="Obj_AI_Minion" /> is an actual minion.
        /// </summary>
        /// <param name="minion">The Minion</param>
        /// <returns>Whether the <see cref="Obj_AI_Minion" /> is an actual minion.</returns>
        public static bool IsMinion(this Obj_AI_Minion minion)
        {
            return minion.GetMinionType().HasFlag(MinionTypes.Melee)
                   || minion.GetMinionType().HasFlag(MinionTypes.Ranged);
        }

        /// <summary>
        ///     Tells whether the <see cref="Obj_AI_Minion" /> is an actual minion.
        /// </summary>
        /// <param name="minion">The Minion</param>
        /// <param name="includeClones">Whether to include clones.</param>
        /// <returns>Whether the <see cref="Obj_AI_Minion" /> is an actual pet.</returns>
        public static bool IsPet(this Obj_AI_Minion minion, bool includeClones = true)
        {
            var name = minion.CharData.BaseSkinName.ToLower();
            return PetList.Contains(name) || (includeClones && CloneList.Contains(name));
        }

        #endregion
    }

    /// <summary>
    ///     The farm location.
    /// </summary>
    public struct FarmLocation
    {
        #region Fields

        /// <summary>
        ///     The minions hit.
        /// </summary>
        public int MinionsHit;

        /// <summary>
        ///     The position.
        /// </summary>
        public Vector2 Position;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="FarmLocation" /> struct.
        /// </summary>
        /// <param name="position">
        ///     The position.
        /// </param>
        /// <param name="minionsHit">
        ///     The minions hit.
        /// </param>
        public FarmLocation(Vector2 position, int minionsHit)
        {
            this.Position = position;
            this.MinionsHit = minionsHit;
        }

        #endregion
    }
    public static class Jungle
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
        private static readonly string[] LegendaryNameRegex = { "SRU_Dragon", "SRU_Baron", "SRU_RiftHerald" };

        /// <summary>
        ///     The small name regex list.
        /// </summary>
        private static readonly string[] SmallNameRegex = { "SRU_[a-zA-Z](.*?)Mini", "Sru_Crab" };

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
        public static JungleType GetJungleType(this Obj_AI_Minion minion)
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
        public static bool IsJungleBuff(this Obj_AI_Minion minion)
        {
            var @base = minion.CharData.BaseSkinName;
            return @base.Equals("SRU_Blue") || @base.Equals("SRU_Red");
        }

        #endregion
    }
}

