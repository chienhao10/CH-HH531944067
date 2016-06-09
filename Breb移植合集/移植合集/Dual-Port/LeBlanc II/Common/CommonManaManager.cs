using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Leblanc.Modes;
using SharpDX;
using Color = SharpDX.Color;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;

namespace Leblanc.Common
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

        public static void Init(Menu mainMenu)
        {
            MenuLocal = mainMenu.AddSubMenu("Mana Settings", "MinMana");
            MenuLocal.Add("MinMana.Jungle.DontCheckEnemyBuff", new ComboBox("Don't check min. mana if I'm taking:", 2, "Off", "Ally Buff", "Enemy Buff", "Both"));
            MenuLocal.Add("MinMana.Jungle.DontCheckBlueBuff", new CheckBox("Don't check min. mana if I have Blue Buff"));
            MenuLocal.Add("MinMana.Jungle.Default", new CheckBox("Load Recommended Settings")).OnValueChange +=
                (sender, args) =>
                {
                    if (args.NewValue)
                    {
                        LoadDefaultSettings();
                    }
                };
            MenuLocal.AddSeparator();
            InitAdvancedMenu();
        }

        static void InitAdvancedMenu()
        {
            MenuLocal.AddGroupLabel("Harass Min. Mana Control");
            {
                MenuLocal.Add("MinMana.Advanced.Harass", new Slider("Harass %", 30, 0, 100));
            }

            MenuLocal.AddGroupLabel("Lane Clear Min. Mana Control");
            {
                MenuLocal.Add("MinMana.Lane.Alone", new Slider("I'm Alone %", 30, 0, 100));
                MenuLocal.Add("MinMana.Lane.Enemy", new Slider("I'm NOT Alone (Enemy Close) %", 60, 0, 100));
            }

            MenuLocal.AddGroupLabel("Jungle Clear Min. Mana Control");
            {
                MenuLocal.Add("MinMana.Jungle.AllyBig", new Slider("Ally: Big Mob %", 50, 0, 100));
                MenuLocal.Add("MinMana.Jungle.AllySmall", new Slider("Ally: Small Mob %", 50, 0, 100));
                MenuLocal.Add("MinMana.Jungle.EnemyBig", new Slider("Enemy: Big Mob %", 30, 0, 100));
                MenuLocal.Add("MinMana.Jungle.EnemySmall", new Slider("Enemy: Small Mob %", 30, 0, 100));
                MenuLocal.Add("MinMana.Jungle.BigBoys", new Slider("Baron/Dragon/RH %", 70, 0, 100));
            }
        }

        public static void LoadDefaultSettings()
        {
            MenuLocal["MinMana.Lane.Alone"].Cast<Slider>().CurrentValue = 30;
            MenuLocal["MinMana.Lane.Enemy"].Cast<Slider>().CurrentValue = 60;

            MenuLocal["MinMana.Jungle.AllyBig"].Cast<Slider>().CurrentValue = 50;
            MenuLocal["MinMana.Jungle.EnemyBig"].Cast<Slider>().CurrentValue = 30;
            MenuLocal["MinMana.Jungle.AllySmall"].Cast<Slider>().CurrentValue = 50;
            MenuLocal["MinMana.Jungle.EnemySmall"].Cast<Slider>().CurrentValue = 30;
            MenuLocal["MinMana.Jungle.BigBoys"].Cast<Slider>().CurrentValue = 70;

            MenuLocal["MinMana.Jungle.DontCheckEnemyBuff"].Cast<ComboBox>().CurrentValue = 3;
            MenuLocal["MinMana.Jungle.DontCheckBlueBuff"].Cast<CheckBox>().CurrentValue = true;
        }

        public static GameObjectTeam GetMobTeam(this Obj_AI_Base mob, float range)
        {
            mobTeams = new Dictionary<Vector2, GameObjectTeam>();
            if (Game.MapId == (GameMapId)11)
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

        public static float HarassMinManaPercent
            =>
                ModeConfig.MenuFarm["Farm.MinMana.Enable"].Cast<KeyBind>().CurrentValue
                    ? MenuLocal["MinMana.Advanced.Harass"].Cast<Slider>().CurrentValue
                    : 0f;

        public static float ToggleMinManaPercent
            =>
                ModeConfig.MenuFarm["Farm.MinMana.Enable"].Cast<KeyBind>().CurrentValue
                    ? MenuLocal["MinMana.Toggle"].Cast<Slider>().CurrentValue
                    : 0f;

        public static float LaneMinManaPercent
        {
            get
            {
                if (ModeConfig.MenuFarm["Farm.MinMana.Enable"].Cast<KeyBind>().CurrentValue)
                {
                    return HeroManager.Enemies.Find(e => e.LSIsValidTarget(2000) && !e.IsZombie) == null
                        ? MenuLocal["MinMana.Lane.Alone"].Cast<Slider>().CurrentValue
                        : MenuLocal["MinMana.Lane.Enemy"].Cast<Slider>().CurrentValue;
                }

                return 0f;
            }
        }

        public static float JungleMinManaPercent(Obj_AI_Base mob)
        {
            // Enable / Disable Min Mana
            if (!ModeConfig.MenuFarm["Farm.MinMana.Enable"].Cast<KeyBind>().CurrentValue)
            {
                return 0f;
            }

            // Don't Control Min Mana 
            if (MenuLocal["MinMana.Jungle.DontCheckBlueBuff"].Cast<CheckBox>().CurrentValue &&
                ObjectManager.Player.HasBuffInst("CrestoftheAncientGolem"))
            {
                return 0f;
            }

            var dontCheckMinMana =
                MenuLocal["MinMana.Jungle.DontCheckEnemyBuff"].Cast<ComboBox>().CurrentValue;

            if ((dontCheckMinMana == 1 || dontCheckMinMana == 3) &&
                mob.GetMobTeam(Q.Range) == (GameObjectTeam)ObjectManager.Player.Team &&
                (mob.BaseSkinName == "SRU_Blue" || mob.BaseSkinName == "SRU_Red"))
            {
                return 0f;
            }

            if ((dontCheckMinMana == 2 || dontCheckMinMana == 3) &&
                mob.GetMobTeam(Q.Range) != (GameObjectTeam)ObjectManager.Player.Team &&
                (mob.BaseSkinName == "SRU_Blue" || mob.BaseSkinName == "SRU_Red"))
            {
                return 0f;
            }

            // Return Min Mana Baron / Dragon
            if (GetMobType(mob) == MobTypes.Baron || GetMobType(mob) == MobTypes.Dragon)
            {
                return MenuLocal["MinMana.Jungle.BigBoys"].Cast<Slider>().CurrentValue;
            }

            // Return Min Mana Ally Big / Small
            if (mob.GetMobTeam(Q.Range) == (GameObjectTeam)ObjectManager.Player.Team)
            {
                return GetMobType(mob) == MobTypes.Big
                    ? MenuLocal["MinMana.Jungle.AllyBig"].Cast<Slider>().CurrentValue
                    : MenuLocal["MinMana.Jungle.AllySmall"].Cast<Slider>().CurrentValue;
            }

            // Return Min Mana Enemy Big / Small
            if (mob.GetMobTeam(Q.Range) != (GameObjectTeam)ObjectManager.Player.Team)
            {
                return GetMobType(mob) == MobTypes.Big
                    ? MenuLocal["MinMana.Jungle.EnemyBig"].Cast<Slider>().CurrentValue
                    : MenuLocal["MinMana.Jungle.EnemySmall"].Cast<Slider>().CurrentValue;
            }

            return 0f;
        }
    }
}