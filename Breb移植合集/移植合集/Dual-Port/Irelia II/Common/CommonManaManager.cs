using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Irelia.Modes;
using SharpDX;
using Color = SharpDX.Color;
using EloBuddy;
using EloBuddy.SDK.Menu;

namespace Irelia.Common
{
    internal static class CommonManaManager
    {
        public static Menu MenuLocal { get; private set; }
        public static Spell Q => Champion.PlayerSpells.Q;

        public enum FromMobClass
        {
            ByName,
            ByType
        }

        public enum MobTypes
        {
            None,
            Small,
            Red,
            Blue,
            Baron,
            Dragon,
            Big
        }

        public enum GameObjectTeam
        {
            Unknown = 0,
            Order = 100,
            Chaos = 200,
            Neutral = 300,
        }

        private static Dictionary<Vector2, GameObjectTeam> mobTeams;

        public static GameObjectTeam GetMobTeam(this Obj_AI_Base mob, float range)
        {
            mobTeams = new Dictionary<Vector2, GameObjectTeam>();
            if (Game.MapId == (GameMapId) 11)
            {
                mobTeams.Add(new Vector2(7756f, 4118f), GameObjectTeam.Order); // blue team :red;
                mobTeams.Add(new Vector2(3824f, 7906f), GameObjectTeam.Order); // blue team :blue
                mobTeams.Add(new Vector2(8356f, 2660f), GameObjectTeam.Order); // blue team :golems
                mobTeams.Add(new Vector2(3860f, 6440f), GameObjectTeam.Order); // blue team :wolfs
                mobTeams.Add(new Vector2(6982f, 5468f), GameObjectTeam.Order); // blue team :wariaths
                mobTeams.Add(new Vector2(2166f, 8348f), GameObjectTeam.Order); // blue team :Frog jQuery

                mobTeams.Add(new Vector2(4768, 10252), GameObjectTeam.Neutral); // Baron
                mobTeams.Add(new Vector2(10060, 4530), GameObjectTeam.Neutral); // Dragon

                mobTeams.Add(new Vector2(7274f, 11018f), GameObjectTeam.Chaos); // Red team :red;
                mobTeams.Add(new Vector2(11182f, 6844f), GameObjectTeam.Chaos); // Red team :Blue
                mobTeams.Add(new Vector2(6450f, 12302f), GameObjectTeam.Chaos); // Red team :golems
                mobTeams.Add(new Vector2(11152f, 8440f), GameObjectTeam.Chaos); // Red team :wolfs
                mobTeams.Add(new Vector2(7830f, 9526f), GameObjectTeam.Chaos); // Red team :wariaths
                mobTeams.Add(new Vector2(12568, 6274), GameObjectTeam.Chaos); // Red team : Frog jQuery

                return mobTeams.Where(hp => mob.LSDistance(hp.Key) <= (range)).Select(hp => hp.Value).FirstOrDefault();
            }

            return GameObjectTeam.Unknown;
        }

        public static MobTypes GetMobType(Obj_AI_Base mob, FromMobClass fromMobClass = FromMobClass.ByName)
        {
            if (mob == null)
            {
                return MobTypes.None;
            }
            if (fromMobClass == FromMobClass.ByName)
            {
                if (mob.BaseSkinName.Contains("SRU_Baron") || mob.BaseSkinName.Contains("SRU_RiftHerald"))
                {
                    return MobTypes.Baron;
                }

                if (mob.BaseSkinName.Contains("SRU_Dragon"))
                {
                    return MobTypes.Dragon;
                }

                if (mob.BaseSkinName.Contains("SRU_Blue"))
                {
                    return MobTypes.Blue;
                }

                if (mob.BaseSkinName.Contains("SRU_Red"))
                {
                    return MobTypes.Red;
                }

                if (mob.BaseSkinName.Contains("SRU_Red"))
                {
                    return MobTypes.Red;
                }
            }

            if (fromMobClass == FromMobClass.ByType)
            {
                Obj_AI_Base oMob =
                    (from fBigBoys in
                        new[]
                        {
                            "SRU_Baron", "SRU_Dragon", "SRU_RiftHerald", "SRU_Blue", "SRU_Gromp", "SRU_Murkwolf",
                            "SRU_Razorbeak", "SRU_Red", "SRU_Krug", "Sru_Crab"
                        }
                        where
                            fBigBoys == mob.BaseSkinName
                     select mob)
                        .FirstOrDefault();

                if (oMob != null)
                {
                    return MobTypes.Big;
                }
            }

            return MobTypes.Small;
        }
    }
}