using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

namespace SCommon.Database
{
    public static class Data
    {
        private static Dictionary<string, float> s_HeroHitBoxes;
        private static Dictionary<string, int> s_JunglePrio;
        private static List<WardData> s_WardData;
        private static List<ChampionData> s_ChampionData;
        private static List<string> s_JungleMinionData;
        private static readonly string[] s_SiegeMinionData = {"Red_Minion_MechCannon", "Blue_Minion_MechCannon"};

        private static readonly string[] s_NormalMinionData =
        {
            "Red_Minion_Wizard", "Blue_Minion_Wizard",
            "Red_Minion_Basic", "Blue_Minion_Basic"
        };

        /// <summary>
        ///     Initializes Data class
        /// </summary>
        static Data()
        {
            GenerateHitBoxData();
            GenerateJungleData();
            GenerateWardData();
            GenerateChampionData();
        }

        /// <summary>
        ///     Gets orginal hitbox value of the hero
        /// </summary>
        /// <param name="hero">The hero</param>
        /// <returns>Orginal hitbox value</returns>
        public static float GetOrginalHitBox(this AIHeroClient hero)
        {
            return s_HeroHitBoxes[hero.ChampionName];
        }

        /// <summary>
        ///     Gets jungle priority of the mob
        /// </summary>
        /// <param name="mob">The mob</param>
        /// <returns>Jungle priority of the mob</returns>
        public static int GetJunglePriority(this GameObject mob)
        {
            if (!s_JunglePrio.ContainsKey(mob.Name))
                return 0;
            return s_JunglePrio[mob.Name];
        }

        /// <summary>
        ///     Checks if the unit is ward
        /// </summary>
        /// <param name="unit">The unit</param>
        /// <returns><c>true</c> if the unit is ward</returns>
        public static bool IsWard(this GameObject unit)
        {
            return s_WardData.Exists(p => p.Name == unit.Name);
        }

        /// <summary>
        ///     Gets priority of the hero
        /// </summary>
        /// <param name="hero">The hero</param>
        /// <returns>Priority of the hero</returns>
        public static int GetPriority(this AIHeroClient hero)
        {
            return (int) s_ChampionData.Find(p => p.Name == hero.ChampionName).Role;
        }

        /// <summary>
        ///     Gets role of the hero
        /// </summary>
        /// <param name="hero">The hero</param>
        /// <returns>Role of the hero</returns>
        public static ChampionRole GetRole(this AIHeroClient hero)
        {
            return s_ChampionData.Find(p => p.Name == hero.ChampionName).Role;
        }

        /// <summary>
        ///     Gets champion id of the hero
        /// </summary>
        /// <param name="hero">The hero</param>
        /// <returns>ID of the hero</returns>
        public static int GetID(this AIHeroClient hero)
        {
            return s_ChampionData.Find(p => p.Name == hero.ChampionName).ID;
        }

        /// <summary>
        ///     Gets champion id of the hero with given name
        /// </summary>
        /// <param name="name">The name of hero</param>
        /// <returns>ID of the hero</returns>
        public static int GetID(string name)
        {
            return s_ChampionData.Find(p => p.Name == name).ID;
        }

        /// <summary>
        ///     Gets Max. ID of heroes
        /// </summary>
        /// <returns>Max ID of heroes</returns>
        public static int GetMaxHeroID()
        {
            return s_ChampionData.Max(p => p.ID);
        }

        /// <summary>
        ///     check if the unit is siege minion
        /// </summary>
        /// <param name="unit">The unit</param>
        /// <returns><c>true</c> if the unit is siege minion</returns>
        public static bool IsSiegeMinion(this Obj_AI_Base unit)
        {
            return s_SiegeMinionData.Any(p => unit.Name.Contains(p));
        }

        /// <summary>
        ///     check if the unit is normal minion
        /// </summary>
        /// <param name="unit">The unit</param>
        /// <returns><c>true</c> if the unit is normal minion</returns>
        public static bool IsNormalMinion(this Obj_AI_Base unit)
        {
            return s_NormalMinionData.Any(p => unit.Name.Contains(p));
        }

        /// <summary>
        ///     check if the unit is jungle minion
        /// </summary>
        /// <param name="unit">The unit</param>
        /// <returns><c>true</c> if the unit is jungle minion</returns>
        public static bool IsJungleMinion(this GameObject unit)
        {
            return s_JunglePrio.ContainsKey(unit.Name);
        }

        /// <summary>
        ///     Checks if the type is immobilize buff type
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns><c>true</c> if the type is immobilize buff type</returns>
        public static bool IsImmobilizeBuff(BuffType type)
        {
            return type == BuffType.Snare || type == BuffType.Stun || type == BuffType.Charm || type == BuffType.Knockup ||
                   type == BuffType.Suppression;
        }

        /// <summary>
        ///     Checks if the unit is immobilized
        /// </summary>
        /// <param name="unit">The unit</param>
        /// <returns><c>true</c> if the unit is immobilized</returns>
        public static bool IsImmobilized(this AIHeroClient unit)
        {
            return unit.Buffs.Any(p => IsImmobilizeBuff(p.Type)) || unit.IsChannelingImportantSpell();
        }

        public static bool IsActive(this Spell s)
        {
            return ObjectManager.Player.Spellbook.GetSpell(s.Slot).ToggleState == 2;
        }

        /// <summary>
        ///     Generates hitbox data
        /// </summary>
        private static void GenerateHitBoxData()
        {
            if (s_HeroHitBoxes != null)
                return;
            s_HeroHitBoxes = new Dictionary<string, float>();
            foreach (var hero in HeroManager.AllHeroes)
                if (!s_HeroHitBoxes.ContainsKey(hero.ChampionName))
                    s_HeroHitBoxes.Add(hero.ChampionName, hero.BBox.Minimum.Distance(hero.BBox.Maximum));
        }

        /// <summary>
        ///     Generates jungle data
        /// </summary>
        private static void GenerateJungleData()
        {
            if (s_JunglePrio != null)
                return;
            s_JunglePrio = new Dictionary<string, int>
            {
                {"SRU_Razorbeak3.1.1", 1},
                {"SRU_RazorbeakMini3.1.4", 2},
                {"SRU_RazorbeakMini3.1.3", 2},
                {"SRU_RazorbeakMini3.1.2", 2},
                {"SRU_Red4.1.1", 1},
                {"SRU_RedMini4.1.2", 2},
                {"SRU_RedMini4.1.3", 2},
                {"SRU_Krug5.1.2", 1},
                {"SRU_KrugMini5.1.1", 2},
                {"SRU_Murkwolf2.1.1", 1},
                {"SRU_MurkwolfMini2.1.3", 2},
                {"SRU_MurkwolfMini2.1.2", 2},
                {"SRU_Blue1.1.1", 1},
                {"SRU_BlueMini21.1.3", 2},
                {"SRU_BlueMini1.1.2", 2},
                {"SRU_Gromp13.1.1", 1},
                {"SRU_Razorbeak9.1.1", 1},
                {"SRU_RazorbeakMini9.1.4", 2},
                {"SRU_RazorbeakMini9.1.3", 2},
                {"SRU_RazorbeakMini9.1.2", 2},
                {"SRU_Red10.1.1", 1},
                {"SRU_RedMini10.1.2", 2},
                {"SRU_RedMini10.1.3", 2},
                {"SRU_Krug11.1.2", 1},
                {"SRU_KrugMini11.1.1", 2},
                {"SRU_Murkwolf8.1.1", 1},
                {"SRU_MurkwolfMini8.1.3", 2},
                {"SRU_MurkwolfMini8.1.2", 2},
                {"SRU_Blue7.1.1", 1},
                {"SRU_BlueMini27.1.3", 2},
                {"SRU_BlueMini7.1.2", 2},
                {"SRU_Gromp14.1.1", 1},
                {"SRU_Dragon6.1.1", 1},
                {"SRU_Baron12.1.1", 1},
                {"Sru_Crab15.1.1", 2},
                {"Sru_Crab16.1.1", 2},
                {"TT_NGolem2.1.1", 1},
                {"TT_NGolem22.1.2", 2},
                {"TT_NWraith21.1.3", 2},
                {"TT_NWraith21.1.2", 2},
                {"TT_NWraith1.1.1", 1},
                {"TT_NWolf23.1.3", 2},
                {"TT_NWolf23.1.2", 2},
                {"TT_NWolf3.1.1", 2},
                {"TT_NWolf26.1.3", 2},
                {"TT_NWolf26.1.2", 2},
                {"TT_NWolf6.1.1", 1},
                {"TT_NWraith4.1.1", 1},
                {"TT_NWraith24.1.3", 1},
                {"TT_NWraith24.1.2", 1},
                {"TT_NGolem25.1.2", 2},
                {"TT_NGolem5.1.1", 1},
                {"AscXerath", 1}
            };

            if (Utility.Map.GetMap().Type.Equals(Utility.Map.MapType.TwistedTreeline))
                s_JungleMinionData = new List<string> {"TT_Spiderboss", "TT_NWraith", "TT_NGolem", "TT_NWolf"};
            else
                s_JungleMinionData = new List<string>
                {
                    "SRU_Blue",
                    "SRU_Gromp",
                    "SRU_Murkwolf",
                    "SRU_Razorbeak",
                    "SRU_Red",
                    "SRU_Krug",
                    "SRU_Dragon",
                    "SRU_Baron",
                    "Sru_Crab"
                };
        }

        /// <summary>
        ///     Generates ward data
        /// </summary>
        private static void GenerateWardData()
        {
            if (s_WardData != null)
                return;
            s_WardData = new List<WardData>
            {
                new WardData
                {
                    Name = "VisionWard",
                    DisplayName = "VisionWard",
                    ObjectName = "visionward",
                    Type = ObjectType.Ward,
                    CastRange = 1450,
                    Duration = 180000
                },
                new WardData
                {
                    Name = "SightWard",
                    DisplayName = "SightWard",
                    ObjectName = "sightward",
                    Type = ObjectType.Ward,
                    CastRange = 1450,
                    Duration = 180000
                },
                new WardData
                {
                    Name = "YellowTrinket",
                    DisplayName = "SightWard",
                    ObjectName = "sightward",
                    Type = ObjectType.Ward,
                    CastRange = 1450,
                    Duration = 180000
                },
                new WardData
                {
                    Name = "SightWard",
                    DisplayName = "SightWard",
                    ObjectName = "itemghostward",
                    Type = ObjectType.Ward,
                    CastRange = 1450,
                    Duration = 180000
                },
                new WardData
                {
                    Name = "SightWard",
                    DisplayName = "SightWard",
                    ObjectName = "wrigglelantern",
                    Type = ObjectType.Ward,
                    CastRange = 1450,
                    Duration = 60000
                },
                new WardData
                {
                    Name = "ShacoBox",
                    DisplayName = "Jack In The Box",
                    ObjectName = "jackinthebox",
                    Type = ObjectType.Ward,
                    CastRange = 1450,
                    Duration = 60000
                }
            };





        }

        /// <summary>
        ///     Generates champion data
        /// </summary>
        private static void GenerateChampionData()
        {
            if (s_ChampionData != null)
                return;

            s_ChampionData = new List<ChampionData>();
            var id = 0;
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Aatrox", Role = ChampionRole.Bruiser});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Ahri", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Akali", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Alistar", Role = ChampionRole.Support});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Amumu", Role = ChampionRole.Tank});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Anivia", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Annie", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Ashe", Role = ChampionRole.ADC});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Azir", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Bard", Role = ChampionRole.Support});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Blitzcrank", Role = ChampionRole.Support});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Brand", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Braum", Role = ChampionRole.Tank});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Caitlyn", Role = ChampionRole.ADC});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Cassiopeia", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Chogath", Role = ChampionRole.Tank});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Corki", Role = ChampionRole.ADC});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Darius", Role = ChampionRole.Bruiser});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Diana", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "DrMundo", Role = ChampionRole.Tank});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Draven", Role = ChampionRole.ADC});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Ekko", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Elise", Role = ChampionRole.Bruiser});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Evelynn", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Ezreal", Role = ChampionRole.ADC});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "FiddleSticks", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Fiora", Role = ChampionRole.Bruiser});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Fizz", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Galio", Role = ChampionRole.Tank});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Gangplank", Role = ChampionRole.Bruiser});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Garen", Role = ChampionRole.Bruiser});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Gnar", Role = ChampionRole.Bruiser});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Gragas", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Graves", Role = ChampionRole.ADC});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Hecarim", Role = ChampionRole.Tank});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Heimerdinger", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Illaoi", Role = ChampionRole.Bruiser});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Irelia", Role = ChampionRole.Bruiser});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Janna", Role = ChampionRole.Support});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "JarvanIV", Role = ChampionRole.Bruiser});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Jax", Role = ChampionRole.Bruiser});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Jayce", Role = ChampionRole.ADC});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Jinx", Role = ChampionRole.ADC});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Jhin", Role = ChampionRole.ADC});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Karma", Role = ChampionRole.Support});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Kalista", Role = ChampionRole.ADC});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Karthus", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Kassadin", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Katarina", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Kayle", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Kennen", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Khazix", Role = ChampionRole.Bruiser});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Kindred", Role = ChampionRole.ADC});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "KogMaw", Role = ChampionRole.ADC});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Leblanc", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "LeeSin", Role = ChampionRole.Bruiser});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Leona", Role = ChampionRole.Support});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Lissandra", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Lucian", Role = ChampionRole.ADC});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Lulu", Role = ChampionRole.Support});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Lux", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Malphite", Role = ChampionRole.Tank});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Malzahar", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Maokai", Role = ChampionRole.Tank});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "MasterYi", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "MissFortune", Role = ChampionRole.ADC});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "MonkeyKing", Role = ChampionRole.Bruiser});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Mordekaiser", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Morgana", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Nami", Role = ChampionRole.Support});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Nasus", Role = ChampionRole.Tank});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Nautilus", Role = ChampionRole.Bruiser});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Nidalee", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Nocturne", Role = ChampionRole.Bruiser});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Nunu", Role = ChampionRole.Support});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Olaf", Role = ChampionRole.Bruiser});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Orianna", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Pantheon", Role = ChampionRole.ADC});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Poppy", Role = ChampionRole.Bruiser});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Quinn", Role = ChampionRole.ADC});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Rammus", Role = ChampionRole.Tank});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "RekSai", Role = ChampionRole.Tank});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Renekton", Role = ChampionRole.Bruiser});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Rengar", Role = ChampionRole.Bruiser});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Riven", Role = ChampionRole.Bruiser});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Rumble", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Ryze", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Sejuani", Role = ChampionRole.Tank});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Shaco", Role = ChampionRole.ADC});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Shen", Role = ChampionRole.Tank});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Shyvana", Role = ChampionRole.Bruiser});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Singed", Role = ChampionRole.Tank});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Sion", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Sivir", Role = ChampionRole.ADC});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Skarner", Role = ChampionRole.Tank});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Sona", Role = ChampionRole.Support});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Soraka", Role = ChampionRole.Support});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Swain", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Syndra", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "TahmKench", Role = ChampionRole.Tank});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Talon", Role = ChampionRole.ADC});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Taric", Role = ChampionRole.Support});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Teemo", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Thresh", Role = ChampionRole.Support});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Tristana", Role = ChampionRole.ADC});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Trundle", Role = ChampionRole.Bruiser});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Tryndamere", Role = ChampionRole.Bruiser});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "TwistedFate", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Twitch", Role = ChampionRole.ADC});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Udyr", Role = ChampionRole.Bruiser});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Urgot", Role = ChampionRole.ADC});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Varus", Role = ChampionRole.ADC});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Vayne", Role = ChampionRole.ADC});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Veigar", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Velkoz", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Vi", Role = ChampionRole.Bruiser});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Viktor", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Vladimir", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Volibear", Role = ChampionRole.Tank});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Warwick", Role = ChampionRole.Tank});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Xerath", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "XinZhao", Role = ChampionRole.Bruiser});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Yasuo", Role = ChampionRole.Bruiser});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Yorick", Role = ChampionRole.Tank});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Zac", Role = ChampionRole.Tank});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Zed", Role = ChampionRole.ADC});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Ziggs", Role = ChampionRole.AP});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Zilean", Role = ChampionRole.Support});
            s_ChampionData.Add(new ChampionData {ID = id++, Name = "Zyra", Role = ChampionRole.AP});
        }
    }
}