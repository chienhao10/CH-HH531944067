using System.Collections.Generic;
using EloBuddy;
using EvadePlus.SkillshotTypes;
using EvadePlus.SkillshotTypes.SpecialTypes;

namespace EvadePlus
{
    internal static class SkillshotDatabase
    {
        public static readonly List<EvadeSkillshot> Database;

        static SkillshotDatabase()
        {
            Database = new List<EvadeSkillshot>
            {
                new SummonerMark
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Mark",
                        ChampionName = "AllChampions",
                        SpellName = "summonersnowball",
                        Slot = SpellSlot.Summoner1,
                        Delay = 0,
                        Range = 1600,
                        Radius = 60,
                        MissileSpeed = 1300,
                        DangerValue = 1,
                        IsDangerous = true,
                        MissileSpellName = "disabled/TestCubeRender",
                        ToggleParticleName = "Summoner_Snowball_Explosion_Sound.troy"
                    }
                },
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Dark Flight",
                        ChampionName = "Aatrox",
                        SpellName = "AatroxQ",
                        Slot = SpellSlot.Q,
                        Delay = 250,
                        Range = 650,
                        Radius = 285,
                        MissileSpeed = 450,
                        DangerValue = 3,
                        IsDangerous = true,
                        MissileSpellName = "AatroxQ"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Blades of Torment",
                        ChampionName = "Aatrox",
                        SpellName = "AatroxE",
                        Slot = SpellSlot.E,
                        Delay = 250,
                        Range = 1075,
                        Radius = 100,
                        MissileSpeed = 1200,
                        DangerValue = 3,
                        IsDangerous = false,
                        MissileSpellName = "AatroxE"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Orb of Deception",
                        ChampionName = "Ahri",
                        SpellName = "AhriOrbofDeception",
                        Slot = SpellSlot.Q,
                        Delay = 250,
                        Range = 925,
                        Radius = 100,
                        MissileSpeed = 1750,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "AhriOrbMissile"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Charm",
                        ChampionName = "Ahri",
                        SpellName = "AhriSeduce",
                        Slot = SpellSlot.E,
                        Delay = 250,
                        Range = 1000,
                        Radius = 60,
                        MissileSpeed = 1550,
                        DangerValue = 3,
                        IsDangerous = true,
                        MissileSpellName = "AhriSeduceMissile",
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Orb of Deception - Back",
                        ChampionName = "Ahri",
                        SpellName = "AhriOrbofDeception2",
                        Slot = SpellSlot.Q,
                        Delay = 250,
                        Range = 925,
                        Radius = 100,
                        MissileSpeed = 915,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "AhriOrbofDeception2"
                    }
                },
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Pulverize",
                        ChampionName = "Alistar",
                        SpellName = "Pulverize",
                        Slot = SpellSlot.Q,
                        Delay = 0,
                        Range = 365,
                        Radius = 365,
                        MissileSpeed = 0,
                        DangerValue = 2,
                        IsDangerous = true,
                        MissileSpellName = "Pulverize"
                    }
                },
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Curse of the Sad Mummy",
                        ChampionName = "Amumu",
                        SpellName = "CurseoftheSadMummy",
                        Slot = SpellSlot.R,
                        Delay = 250,
                        Range = 560,
                        Radius = 560,
                        MissileSpeed = 0,
                        DangerValue = 5,
                        MissileSpellName = "CurseoftheSadMummy",
                        IsDangerous = true
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Bandage Toss",
                        ChampionName = "Amumu",
                        SpellName = "BandageToss",
                        Slot = SpellSlot.Q,
                        Delay = 250,
                        Range = 1100,
                        Radius = 80,
                        MissileSpeed = 2000,
                        DangerValue = 3,
                        IsDangerous = true,
                        MissileSpellName = "SadMummyBandageToss"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Flash Frost",
                        ChampionName = "Anivia",
                        SpellName = "FlashFrostSpell",
                        Slot = SpellSlot.Q,
                        Delay = 250,
                        Range = 1250,
                        Radius = 110,
                        MissileSpeed = 850,
                        DangerValue = 3,
                        IsDangerous = true,
                        MissileSpellName = "FlashFrostSpell"
                    }
                },
                //new CircularMissileSkillshot
                //{
                //    SpellData = new SpellData
                //    {
                //        ChampionName = "Annie",
                //        SpellName = "Incinerate",
                //        Slot = SpellSlot.W,
                //        Delay = 250,
                //        Range = 625,
                //        Radius = 80,
                //        MissileSpeed = 0,
                //        DangerValue = 2,
                //        IsDangerous = false,
                //        MissileSpellName = "Incinerate"
                //    }
                //},
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Summon: Tibbers",
                        ChampionName = "Annie",
                        SpellName = "InfernalGuardian",
                        Slot = SpellSlot.R,
                        Delay = 250,
                        Range = 600,
                        Radius = 290,
                        MissileSpeed = 0,
                        DangerValue = 5,
                        IsDangerous = true,
                        MissileSpellName = "InfernalGuardian"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Enchanted Crystal Arrow",
                        ChampionName = "Ashe",
                        SpellName = "EnchantedCrystalArrow",
                        Slot = SpellSlot.R,
                        Delay = 250,
                        Range = 12500,
                        Radius = 130,
                        MissileSpeed = 1600,
                        DangerValue = 5,
                        IsDangerous = true,
                        MissileSpellName = "EnchantedCrystalArrow"
                    }
                },
                //new LinearMissileSkillshot
                //{
                //    SpellData = new SpellData
                //    {
                //        ChampionName = "Ashe",
                //        SpellName = "Volley",
                //        Slot = SpellSlot.W,
                //        Delay = 250,
                //        Range = 1150,
                //        Radius = 20,
                //        MissileSpeed = 1500,
                //        DangerValue = 3,
                //        MissileSpellName = "VolleyAttack",
                //        ExtraMissiles = 8
                //    }
                //},
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Conquering Sands",
                        ChampionName = "Azir",
                        SpellName = "disabled/AzirQ",
                        Slot = SpellSlot.Q,
                        Delay = 250,
                        Range = 850,
                        Radius = 80,
                        MissileSpeed = 1000,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "azirsoldiermissile"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Cosmic Binding",
                        ChampionName = "Bard",
                        SpellName = "BardQ",
                        Slot = SpellSlot.Q,
                        Delay = 250,
                        Range = 950,
                        Radius = 60,
                        MissileSpeed = 1600,
                        DangerValue = 3,
                        IsDangerous = true,
                        MissileSpellName = "BardQMissile"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Rocket Grab",
                        ChampionName = "Blitzcrank",
                        SpellName = "RocketGrab",
                        Slot = SpellSlot.Q,
                        Delay = 250,
                        Range = 1050,
                        Radius = 70,
                        MissileSpeed = 1800,
                        DangerValue = 4,
                        IsDangerous = true,
                        MissileSpellName = "RocketGrabMissile"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Sear",
                        ChampionName = "Brand",
                        SpellName = "BrandQ",
                        Slot = SpellSlot.Q,
                        Delay = 250,
                        Range = 1100,
                        Radius = 60,
                        MissileSpeed = 1600,
                        DangerValue = 3,
                        IsDangerous = true,
                        MissileSpellName = "BrandQMissile"
                    }
                },
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Pillar of Flame",
                        ChampionName = "Brand",
                        SpellName = "BrandW",
                        Slot = SpellSlot.W,
                        Delay = 850,
                        Range = 1100,
                        Radius = 250,
                        MissileSpeed = 0,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "BrandFissure"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Glacial Fissure",
                        ChampionName = "Braum",
                        SpellName = "BraumRWrapper",
                        Slot = SpellSlot.R,
                        Delay = 500,
                        Range = 1250,
                        Radius = 100,
                        MissileSpeed = 1125,
                        DangerValue = 4,
                        IsDangerous = true,
                        MissileSpellName = "braumrmissile"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Winter's Bite",
                        ChampionName = "Braum",
                        SpellName = "BraumQ",
                        Slot = SpellSlot.Q,
                        Delay = 250,
                        Range = 1000,
                        Radius = 100,
                        MissileSpeed = 1200,
                        DangerValue = 3,
                        IsDangerous = true,
                        MissileSpellName = "BraumQMissile"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Piltover Peacemaker",
                        ChampionName = "Caitlyn",
                        SpellName = "CaitlynPiltoverPeacemaker",
                        Slot = SpellSlot.Q,
                        Delay = 625,
                        Range = 1300,
                        Radius = 90,
                        MissileSpeed = 2200,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "CaitlynPiltoverPeacemaker"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "90 Caliber Net",
                        ChampionName = "Caitlyn",
                        SpellName = "CaitlynEntrapment",
                        Slot = SpellSlot.E,
                        Delay = 125,
                        Range = 950,
                        Radius = 80,
                        MissileSpeed = 2000,
                        DangerValue = 1,
                        IsDangerous = false,
                        MissileSpellName = "CaitlynEntrapmentMissile"
                    }
                },
                //new CircularMissileSkillshot
                //{
                //    SpellData = new SpellData
                //    {
                //        ChampionName = "Cassiopeia",
                //        SpellName = "CassiopeiaPetrifyingGaze",
                //        Slot = SpellSlot.R,
                //        Delay = 500,
                //        Range = 825,
                //        Radius = 20,
                //        MissileSpeed = 0,
                //        DangerValue = 3,
                //        MissileSpellName = "CassiopeiaPetrifyingGaze"
                //    }
                //},
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Noxious Blast",
                        ChampionName = "Cassiopeia",
                        SpellName = "CassiopeiaQ",
                        Slot = SpellSlot.Q,
                        Delay = 825,
                        Range = 600,
                        Radius = 200,
                        MissileSpeed = 0,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "CassiopeiaQ"
                    }
                },
                //new CircularMissileSkillshot
                //{
                //    SpellData = new SpellData
                //    {
                //        ChampionName = "Cassiopeia",
                //        SpellName = "CassiopeiaMiasma",
                //        Slot = SpellSlot.W,
                //        Delay = 250,
                //        Range = 850,
                //        Radius = 220,
                //        MissileSpeed = 2500,
                //        DangerValue = 3,
                //        MissileSpellName = "CassiopeiaMiasma"
                //    }
                //},
                //new CircularMissileSkillshot
                //{
                //    SpellData = new SpellData
                //    {
                //        ChampionName = "Chogath",
                //        SpellName = "FeralScream",
                //        Slot = SpellSlot.W,
                //        Delay = 250,
                //        Range = 650,
                //        Radius = 20,
                //        MissileSpeed = 0,
                //        DangerValue = 3,
                //        MissileSpellName = "FeralScream"
                //    }
                //},
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Rupture",
                        ChampionName = "Chogath",
                        SpellName = "Rupture",
                        Slot = SpellSlot.Q,
                        Delay = 1200,
                        Range = 950,
                        Radius = 250,
                        MissileSpeed = 0,
                        DangerValue = 3,
                        IsDangerous = false,
                        MissileSpellName = "Rupture"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Missile Barrage Big",
                        ChampionName = "Corki",
                        SpellName = "MissileBarrage2",
                        Slot = SpellSlot.R,
                        Delay = 175,
                        Range = 1500,
                        Radius = 40,
                        MissileSpeed = 2000,
                        DangerValue = 3,
                        IsDangerous = false,
                        MissileSpellName = "MissileBarrageMissile2"
                    }
                },
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Phosphorus Bomb",
                        ChampionName = "Corki",
                        SpellName = "PhosphorusBomb",
                        Slot = SpellSlot.Q,
                        Delay = 500,
                        Range = 825,
                        Radius = 270,
                        MissileSpeed = 1125,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "PhosphorusBombMissile"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        ChampionName = "Corki",
                        SpellName = "MissileBarrage",
                        Slot = SpellSlot.R,
                        Delay = 175,
                        Range = 1300,
                        Radius = 40,
                        MissileSpeed = 2000,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "MissileBarrageMissile"
                    }
                },
                //new CircularMissileSkillshot
                //{
                //    SpellData = new SpellData
                //    {
                //        ChampionName = "Darius",
                //        SpellName = "DariusAxeGrabCone",
                //        Slot = SpellSlot.E,
                //        Delay = 320,
                //        Range = 570,
                //        Radius = 20,
                //        MissileSpeed = 0,
                //        DangerValue = 3,
                //        MissileSpellName = "DariusAxeGrabCone"
                //    }
                //},
                //new CircularMissileSkillshot //Unknown:SpellType.Arc
                //{
                //    SpellData = new SpellData
                //    {
                //        ChampionName = "Diana",
                //        SpellName = "DianaArc",
                //        Slot = SpellSlot.Q,
                //        Delay = 250,
                //        Range = 850,
                //        Radius = 50,
                //        MissileSpeed = 1400,
                //        DangerValue = 3,
                //        MissileSpellName = "DianaArc"
                //    }
                //},
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Infected Cleaver",
                        ChampionName = "DrMundo",
                        SpellName = "InfectedCleaverMissileCast",
                        Slot = SpellSlot.Q,
                        Delay = 250,
                        Range = 1050,
                        Radius = 60,
                        MissileSpeed = 2000,
                        DangerValue = 3,
                        IsDangerous = false,
                        MissileSpellName = "InfectedCleaverMissile"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Whirling Death",
                        ChampionName = "Draven",
                        SpellName = "DravenRCast",
                        Slot = SpellSlot.R,
                        Delay = 500,
                        Range = 12500,
                        Radius = 160,
                        MissileSpeed = 2000,
                        DangerValue = 5,
                        IsDangerous = true,
                        MissileSpellName = "DravenR"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Stand Aside",
                        ChampionName = "Draven",
                        SpellName = "DravenDoubleShot",
                        Slot = SpellSlot.E,
                        Delay = 250,
                        Range = 1100,
                        Radius = 130,
                        MissileSpeed = 1400,
                        DangerValue = 3,
                        IsDangerous = true,
                        MissileSpellName = "DravenDoubleShotMissile"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Timewinder",
                        ChampionName = "Ekko",
                        SpellName = "EkkoQ",
                        Slot = SpellSlot.Q,
                        Delay = 250,
                        Range = 950,
                        Radius = 60,
                        MissileSpeed = 1650,
                        DangerValue = 4,
                        IsDangerous = true,
                        MissileSpellName = "ekkoqmis"
                    }
                },
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Parallel Convergence",
                        ChampionName = "Ekko",
                        SpellName = "EkkoW",
                        Slot = SpellSlot.W,
                        Delay = 3750,
                        Range = 1600,
                        Radius = 375,
                        MissileSpeed = 1650,
                        DangerValue = 3,
                        IsDangerous = false,
                        AddHitbox = false,
                        MissileSpellName = "EkkoW"
                    }
                },
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Chronobreak",
                        ChampionName = "Ekko",
                        SpellName = "EkkoR",
                        Slot = SpellSlot.R,
                        Delay = 250,
                        Range = 1600,
                        Radius = 375,
                        MissileSpeed = 1650,
                        DangerValue = 3,
                        IsDangerous = false,
                        MissileSpellName = "EkkoR"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Cocoon",
                        ChampionName = "Elise",
                        SpellName = "EliseHumanE",
                        Slot = SpellSlot.E,
                        Delay = 250,
                        Range = 1100,
                        Radius = 70,
                        MissileSpeed = 1600,
                        DangerValue = 4,
                        IsDangerous = true,
                        MissileSpellName = "EliseHumanE"
                    }
                },
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Agony's Embrace",
                        ChampionName = "Evelynn",
                        SpellName = "EvelynnR",
                        Slot = SpellSlot.R,
                        Delay = 250,
                        Range = 650,
                        Radius = 350,
                        MissileSpeed = 0,
                        DangerValue = 5,
                        IsDangerous = true,
                        MissileSpellName = "EvelynnR"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Mystic Shot",
                        ChampionName = "Ezreal",
                        SpellName = "EzrealMysticShot",
                        Slot = SpellSlot.Q,
                        Delay = 250,
                        Range = 1200,
                        Radius = 60,
                        MissileSpeed = 2000,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "EzrealMysticShotMissile"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Trueshot Barrage",
                        ChampionName = "Ezreal",
                        SpellName = "EzrealTrueshotBarrage",
                        Slot = SpellSlot.R,
                        Delay = 1000,
                        Range = 20000,
                        Radius = 160,
                        MissileSpeed = 2000,
                        DangerValue = 3,
                        IsDangerous = true,
                        MissileSpellName = "EzrealTrueshotBarrage"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Essence Flux",
                        ChampionName = "Ezreal",
                        SpellName = "EzrealEssenceFlux",
                        Slot = SpellSlot.W,
                        Delay = 250,
                        Range = 1050,
                        Radius = 80,
                        MissileSpeed = 1600,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "EzrealEssenceFluxMissile"
                    }
                },
                /*
                 * TODO: Fiora W
                */
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Chum the Waters",
                        ChampionName = "Fizz",
                        SpellName = "FizzMarinerDoom",
                        Slot = SpellSlot.R,
                        Delay = 250,
                        Range = 1275,
                        Radius = 120,
                        MissileSpeed = 1350,
                        DangerValue = 5,
                        IsDangerous = true,
                        MissileSpellName = "FizzMarinerDoomMissile"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Righteous Gust",
                        ChampionName = "Galio",
                        SpellName = "GalioRighteousGust",
                        Slot = SpellSlot.E,
                        Delay = 250,
                        Range = 1280,
                        Radius = 120,
                        MissileSpeed = 1300,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "GalioRighteousGust"
                    }
                },
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Resolute Smite",
                        ChampionName = "Galio",
                        SpellName = "GalioResoluteSmite",
                        Slot = SpellSlot.Q,
                        Delay = 250,
                        Range = 1040,
                        Radius = 235,
                        MissileSpeed = 1200,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "GalioResoluteSmite"
                    }
                },
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Idol of Durand",
                        ChampionName = "Galio",
                        SpellName = "GalioIdolOfDurand",
                        Slot = SpellSlot.R,
                        Delay = 250,
                        Range = 600,
                        Radius = 600,
                        MissileSpeed = 0,
                        DangerValue = 3,
                        AddHitbox = false,
                        MissileSpellName = ""
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Boulder Toss",
                        ChampionName = "Gnar",
                        SpellName = "gnarbigq",
                        Slot = SpellSlot.Q,
                        Delay = 500,
                        Range = 1150,
                        Radius = 90,
                        MissileSpeed = 2100,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "gnarbigq"
                    }
                },
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "GNAR!",
                        ChampionName = "Gnar",
                        SpellName = "GnarR",
                        Slot = SpellSlot.R,
                        Delay = 250,
                        Range = 500,
                        Radius = 500,
                        MissileSpeed = 0,
                        DangerValue = 5,
                        IsDangerous = true,
                        AddHitbox = false,
                        MissileSpellName = "GnarR"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Wallop",
                        ChampionName = "Gnar",
                        SpellName = "gnarbigw",
                        Slot = SpellSlot.W,
                        Delay = 600,
                        Range = 600,
                        Radius = 100,
                        MissileSpeed = 0,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "gnarbigw"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Boomerang Throw",
                        ChampionName = "Gnar",
                        SpellName = "GnarQ",
                        Slot = SpellSlot.Q,
                        Delay = 250,
                        Range = 1185,
                        Radius = 60,
                        MissileSpeed = 2400,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "GnarQ"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Boomerang Throw Return",
                        ChampionName = "Gnar",
                        SpellName = "GnarQReturn",
                        Slot = SpellSlot.Q,
                        Delay = 250,
                        Range = 1185,
                        Radius = 60,
                        MissileSpeed = 2400,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "GnarQMissileReturn"
                    }
                },
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Hop",
                        ChampionName = "Gnar",
                        SpellName = "GnarE",
                        Slot = SpellSlot.E,
                        Delay = 0,
                        Range = 475,
                        Radius = 150,
                        MissileSpeed = 900,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "GnarE"
                    }
                },
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Crunch",
                        ChampionName = "Gnar",
                        SpellName = "gnarbige",
                        Slot = SpellSlot.E,
                        Delay = 0,
                        Range = 475,
                        Radius = 100,
                        MissileSpeed = 800,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "gnarbige"
                    }
                },
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Barrel Roll",
                        ChampionName = "Gragas",
                        SpellName = "GragasQ",
                        Slot = SpellSlot.Q,
                        Delay = 500,
                        Range = 975,
                        Radius = 250,
                        MissileSpeed = 1000,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "GragasQ",
                        ToggleParticleName = "Gragas_.+_Q_(Enemy|Ally)"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Body Slam",
                        ChampionName = "Gragas",
                        SpellName = "GragasE",
                        Slot = SpellSlot.E,
                        Delay = 0,
                        Range = 950,
                        Radius = 200,
                        MissileSpeed = 1200,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "GragasE"
                    }
                },
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Explosive Cask",
                        ChampionName = "Gragas",
                        SpellName = "GragasR",
                        Slot = SpellSlot.R,
                        Delay = 250,
                        Range = 1050,
                        Radius = 350,
                        MissileSpeed = 1750,
                        DangerValue = 5,
                        IsDangerous = true,
                        MissileSpellName = "GragasR"
                    }
                },
                //new LinearMissileSkillshot
                //{
                //    SpellData = new SpellData
                //    {
                //        ChampionName = "Graves",
                //        SpellName = "GravesClusterShot",
                //        Slot = SpellSlot.Q,
                //        Delay = 250,
                //        Range = 1025,
                //        Radius = 60,
                //        MissileSpeed = 2000,
                //        DangerValue = 3,
                //        MissileSpellName = "GravesClusterShotAttack",
                //        ExtraMissiles = 2
                //    }
                //},
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Collateral Damage",
                        ChampionName = "Graves",
                        SpellName = "GravesChargeShot",
                        Slot = SpellSlot.R,
                        Delay = 250,
                        Range = 1000,
                        Radius = 100,
                        MissileSpeed = 2100,
                        DangerValue = 5,
                        IsDangerous = true,
                        MissileSpellName = "GravesChargeShotShot"
                    }
                },
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Onslaught of Shadows",
                        ChampionName = "Hecarim",
                        SpellName = "HecarimUlt",
                        Slot = SpellSlot.R,
                        Delay = 10,
                        Range = 1500,
                        Radius = 300,
                        MissileSpeed = 1100,
                        DangerValue = 5,
                        IsDangerous = true,
                        MissileSpellName = "HecarimUlt"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Hextech Micro-Rockets",
                        ChampionName = "Heimerdinger",
                        SpellName = "disabled/HeimerdingerW",
                        Slot = SpellSlot.W,
                        Delay = 250,
                        Range = 1500,
                        Radius = 70,
                        MissileSpeed = 1800,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "HeimerdingerWAttack2"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Hextech Micro-Rockets Ult",
                        ChampionName = "Heimerdinger",
                        SpellName = "disabled/HeimerdingerW",
                        Slot = SpellSlot.W,
                        Delay = 250,
                        Range = 1500,
                        Radius = 70,
                        MissileSpeed = 1800,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "HeimerdingerWAttack2Ult"
                    }
                },
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "CH-2 Electron Storm Grenade",
                        ChampionName = "Heimerdinger",
                        SpellName = "HeimerdingerE",
                        Slot = SpellSlot.E,
                        Delay = 325,
                        Range = 925,
                        Radius = 135,
                        MissileSpeed = 1750,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "HeimerdingerESpell"
                    }
                },
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "CH-2 Electron Storm Grenade Ult",
                        ChampionName = "Heimerdinger",
                        SpellName = "disabled/HeimerdingerE",
                        Slot = SpellSlot.E,
                        Delay = 325,
                        Range = 925,
                        Radius = 135,
                        MissileSpeed = 1750,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "heimerdingerespell_ult"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "ranscendent Blades",
                        ChampionName = "Irelia",
                        SpellName = "IreliaTranscendentBlades",
                        Slot = SpellSlot.R,
                        Delay = 0,
                        Range = 1200,
                        Radius = 65,
                        MissileSpeed = 1600,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "ireliatranscendentbladesspell"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Howling Gale",
                        ChampionName = "Janna",
                        SpellName = "//HowlingGale",
                        Slot = SpellSlot.Q,
                        Delay = 0,
                        Range = 1700,
                        Radius = 120,
                        MissileSpeed = 900,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "HowlingGaleSpell"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Dragon Strike",
                        ChampionName = "JarvanIV",
                        SpellName = "JarvanIVDragonStrike",
                        Slot = SpellSlot.Q,
                        Delay = 250,
                        Range = 845,
                        Radius = 80,
                        MissileSpeed = 2000,
                        DangerValue = 3,
                        IsDangerous = true,
                        MissileSpellName = "JarvanIVDragonStrike"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Dragon Strike EQ",
                        ChampionName = "JarvanIVEQ",
                        SpellName = "JarvanIVDragonStrike2",
                        Slot = SpellSlot.Q,
                        Delay = 250,
                        Range = 845,
                        Radius = 120,
                        MissileSpeed = 1800,
                        DangerValue = 3,
                        IsDangerous = false,
                        MissileSpellName = "JarvanIVDragonStrike2"
                    }
                },
                //new CircularMissileSkillshot
                //{
                //    SpellData = new SpellData
                //    {
                //        ChampionName = "JarvanIV",
                //        SpellName = "JarvanIVCataclysm",
                //        Slot = SpellSlot.R,
                //        Delay = 0,
                //        Range = 825,
                //        Radius = 350,
                //        MissileSpeed = 1900,
                //        DangerValue = 3,
                //        MissileSpellName = "JarvanIVCataclysm"
                //    }
                //},
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Shock Blast Fast",
                        ChampionName = "Jayce",
                        SpellName = "JayceQAccel",
                        Slot = SpellSlot.Q,
                        Delay = 250,
                        Range = 1170,
                        Radius = 70,
                        MissileSpeed = 2350,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "JayceShockBlastWallMis"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Shock Blast",
                        ChampionName = "Jayce",
                        SpellName = "jayceshockblast",
                        Slot = SpellSlot.Q,
                        Delay = 250,
                        Range = 1050,
                        Radius = 70,
                        MissileSpeed = 1450,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "JayceShockBlastMis"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Super Mega Death Rocket!",
                        ChampionName = "Jinx",
                        SpellName = "JinxR",
                        Slot = SpellSlot.R,
                        Delay = 600,
                        Range = 25000,
                        Radius = 120,
                        MissileSpeed = 1700,
                        DangerValue = 5,
                        IsDangerous = true,
                        MissileSpellName = "JinxR"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Zap!",
                        ChampionName = "Jinx",
                        SpellName = "JinxWMissile",
                        Slot = SpellSlot.W,
                        Delay = 600,
                        Range = 1500,
                        Radius = 60,
                        MissileSpeed = 3300,
                        DangerValue = 3,
                        IsDangerous = true,
                        MissileSpellName = "JinxWMissile"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Pierce",
                        ChampionName = "Kalista",
                        SpellName = "KalistaMysticShot",
                        Slot = SpellSlot.Q,
                        Delay = 350,
                        Range = 1200,
                        Radius = 70,
                        MissileSpeed = 2000,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "kalistamysticshotmistrue"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Inner Flame",
                        ChampionName = "Karma",
                        SpellName = "KarmaQ",
                        Slot = SpellSlot.Q,
                        Delay = 250,
                        Range = 1050,
                        Radius = 90,
                        MissileSpeed = 1700,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "KarmaQMissile"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Soulflare (Mantra)",
                        ChampionName = "Karma",
                        SpellName = "KarmaQMissileMantra",
                        Slot = SpellSlot.Q,
                        Delay = 250,
                        Range = 1050,
                        Radius = 90,
                        MissileSpeed = 1700,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "KarmaQMissileMantra"
                    }
                },
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Lay Waste",
                        ChampionName = "Karthus",
                        SpellName = "KarthusLayWasteA1",
                        Slot = SpellSlot.Q,
                        Delay = 625,
                        Range = 875,
                        Radius = 190,
                        MissileSpeed = 0,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "KarthusLayWasteA1"
                    }
                },
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Riftwalk",
                        ChampionName = "Kassadin",
                        SpellName = "RiftWalk",
                        Slot = SpellSlot.R,
                        Delay = 250,
                        Range = 700,
                        Radius = 270,
                        MissileSpeed = 0,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "RiftWalk"
                    }
                },
                //new CircularMissileSkillshot
                //{
                //    SpellData = new SpellData
                //    {
                //        ChampionName = "Kassadin",
                //        SpellName = "ForcePulse",
                //        Slot = SpellSlot.E,
                //        Delay = 250,
                //        Range = 700,
                //        Radius = 20,
                //        MissileSpeed = 0,
                //        DangerValue = 3,
                //        MissileSpellName = "ForcePulse"
                //    }
                //},
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Thundering Shuriken",
                        ChampionName = "Kennen",
                        SpellName = "KennenShurikenHurlMissile1",
                        Slot = SpellSlot.Q,
                        Delay = 125,
                        Range = 1175,
                        Radius = 50,
                        MissileSpeed = 1700,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "KennenShurikenHurlMissile1"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Void Spike",
                        ChampionName = "Khazix",
                        SpellName = "KhazixW",
                        Slot = SpellSlot.W,
                        Delay = 250,
                        Range = 1100,
                        Radius = 70,
                        MissileSpeed = 1700,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "KhazixWMissile"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Void Spike Evolved",
                        ChampionName = "Khazix",
                        SpellName = "khazixwlong",
                        Slot = SpellSlot.W,
                        Delay = 250,
                        Range = 1025,
                        Radius = 70,
                        MissileSpeed = 1700,
                        DangerValue = 2,
                        IsDangerous = false,
                        ExtraMissiles = 2,
                        MissileSpellName = "khazixwlong"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Caustic Spittle",
                        ChampionName = "KogMaw",
                        SpellName = "KogMawQ",
                        Slot = SpellSlot.Q,
                        Delay = 250,
                        Range = 1125,
                        Radius = 70,
                        MissileSpeed = 1650,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "KogMawQMis"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Void Ooze",
                        ChampionName = "KogMaw",
                        SpellName = "KogMawVoidOoze",
                        Slot = SpellSlot.E,
                        Delay = 250,
                        Range = 1360,
                        Radius = 120,
                        MissileSpeed = 1400,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "KogMawVoidOozeMissile"
                    }
                },
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Living Artillery",
                        ChampionName = "KogMaw",
                        SpellName = "KogMawLivingArtillery",
                        Slot = SpellSlot.R,
                        Delay = 1100,
                        Range = 2200,
                        Radius = 235,
                        MissileSpeed = 0,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "KogMawLivingArtillery"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Ethereal Chains (Mimic)",
                        ChampionName = "Leblanc",
                        SpellName = "LeblancSoulShackleM",
                        Slot = SpellSlot.R,
                        Delay = 250,
                        Range = 960,
                        Radius = 70,
                        MissileSpeed = 1600,
                        DangerValue = 3,
                        IsDangerous = true,
                        MissileSpellName = "LeblancSoulShackleM"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Ethereal Chains",
                        ChampionName = "Leblanc",
                        SpellName = "LeblancSoulShackle",
                        Slot = SpellSlot.E,
                        Delay = 250,
                        Range = 960,
                        Radius = 70,
                        MissileSpeed = 1600,
                        DangerValue = 3,
                        IsDangerous = true,
                        MissileSpellName = "LeblancSoulShackle"
                    }
                },
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Distortion (Mimic)",
                        ChampionName = "Leblanc",
                        SpellName = "LeblancSlideM",
                        Slot = SpellSlot.R,
                        Delay = 250,
                        Range = 725,
                        Radius = 250,
                        MissileSpeed = 1600,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "LeblancSlideM"
                    }
                },
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Distortion",
                        ChampionName = "Leblanc",
                        SpellName = "LeblancSlide",
                        Slot = SpellSlot.W,
                        Delay = 250,
                        Range = 725,
                        Radius = 250,
                        MissileSpeed = 1600,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "LeblancSlide"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Sonic Wave",
                        ChampionName = "LeeSin",
                        SpellName = "BlindMonkQOne",
                        Slot = SpellSlot.Q,
                        Delay = 250,
                        Range = 1100,
                        Radius = 60,
                        MissileSpeed = 1800,
                        DangerValue = 3,
                        IsDangerous = true,
                        MissileSpellName = "BlindMonkQOne"
                    }
                },
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Solar Flare",
                        ChampionName = "Leona",
                        SpellName = "LeonaSolarFlare",
                        Slot = SpellSlot.R,
                        Delay = 625,
                        Range = 1200,
                        Radius = 250,
                        MissileSpeed = 0,
                        DangerValue = 5,
                        IsDangerous = true,
                        MissileSpellName = "LeonaSolarFlare"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Zenith Blade",
                        ChampionName = "Leona",
                        SpellName = "LeonaZenithBlade",
                        Slot = SpellSlot.E,
                        Delay = 350,
                        Range = 975,
                        Radius = 70,
                        MissileSpeed = 2000,
                        DangerValue = 3,
                        IsDangerous = true,
                        MissileSpellName = "FlashFrostSpell"
                    }
                },
                //new CircularMissileSkillshot
                //{
                //    SpellData = new SpellData
                //    {
                //        ChampionName = "Lissandra",
                //        SpellName = "LissandraW",
                //        Slot = SpellSlot.W,
                //        Delay = 250,
                //        Range = 725,
                //        Radius = 450,
                //        MissileSpeed = 0,
                //        DangerValue = 3,
                //        MissileSpellName = "LissandraW"
                //    }
                //},
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Ice Shard",
                        ChampionName = "Lissandra",
                        SpellName = "LissandraQ",
                        Slot = SpellSlot.Q,
                        Delay = 250,
                        Range = 825,
                        Radius = 75,
                        MissileSpeed = 2200,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "LissandraQ"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Ardent Blaze",
                        ChampionName = "Lucian",
                        SpellName = "LucianW",
                        Slot = SpellSlot.W,
                        Delay = 250,
                        Range = 1000,
                        Radius = 80,
                        MissileSpeed = 1600,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "LucianW"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Piercing Light",
                        ChampionName = "Lucian",
                        SpellName = "LucianQ",
                        Slot = SpellSlot.Q,
                        Delay = 450,
                        Range = 1140,
                        Radius = 65,
                        MissileSpeed = 0,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "LucianQ",
                        AddHitbox = false
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Glitterlance",
                        ChampionName = "Lulu",
                        SpellName = "LuluQ",
                        Slot = SpellSlot.Q,
                        Delay = 250,
                        Range = 925,
                        Radius = 80,
                        MissileSpeed = 1450,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "LuluQMissile"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Glitterlance Pix",
                        ChampionName = "Lulu",
                        SpellName = "LuluQPix",
                        Slot = SpellSlot.Q,
                        Delay = 250,
                        Range = 925,
                        Radius = 80,
                        MissileSpeed = 1450,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "LuluQMissileTwo"
                    }
                },
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Lucent Singularity",
                        ChampionName = "Lux",
                        SpellName = "LuxLightStrikeKugel",
                        Slot = SpellSlot.E,
                        Delay = 250,
                        Range = 1100,
                        Radius = 340,
                        MissileSpeed = 1400,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "LuxLightStrikeKugel",
                        ToggleParticleName = "Lux_.+_E_tar_aoe_",
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Final Spark",
                        ChampionName = "Lux",
                        SpellName = "LuxMaliceCannon",
                        Slot = SpellSlot.R,
                        Delay = 1000,
                        Range = 3500,
                        Radius = 110,
                        MissileSpeed = 0,
                        DangerValue = 5,
                        IsDangerous = true,
                        MissileSpellName = "LuxMaliceCannon"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Light Binding",
                        ChampionName = "Lux",
                        SpellName = "LuxLightBinding",
                        Slot = SpellSlot.Q,
                        Delay = 250,
                        Range = 1300,
                        Radius = 70,
                        MissileSpeed = 1200,
                        DangerValue = 3,
                        IsDangerous = true,
                        MissileSpellName = "LuxLightBindingMis"
                    }
                },
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Unstoppable Force",
                        ChampionName = "Malphite",
                        SpellName = "UFSlash",
                        Slot = SpellSlot.R,
                        Delay = 0,
                        Range = 1000,
                        Radius = 270,
                        MissileSpeed = 1500,
                        DangerValue = 5,
                        IsDangerous = true,
                        MissileSpellName = "UFSlash"
                    }
                },
                //new LinearMissileSkillshot
                //{
                //    SpellData = new SpellData
                //    {
                //        ChampionName = "Malzahar",
                //        SpellName = "AlZaharCalloftheVoid",
                //        Slot = SpellSlot.Q,
                //        Delay = 1000,
                //        Range = 900,
                //        Radius = 85,
                //        MissileSpeed = 1600,
                //        DangerValue = 3,
                //        MissileSpellName = "AlZaharCalloftheVoidMissile"
                //    }
                //},
                //new CircularMissileSkillshot
                //{
                //    SpellData = new SpellData
                //    {
                //        ChampionName = "MonkeyKing",
                //        SpellName = "MonkeyKingSpinToWin",
                //        Slot = SpellSlot.R,
                //        Delay = 250,
                //        Range = 300,
                //        Radius = 225,
                //        MissileSpeed = 0,
                //        DangerValue = 3,
                //        MissileSpellName = "MonkeyKingSpinToWin"
                //    }
                //},
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Dark Binding",
                        ChampionName = "Morgana",
                        SpellName = "DarkBindingMissile",
                        Slot = SpellSlot.Q,
                        Delay = 250,
                        Range = 1300,
                        Radius = 80,
                        MissileSpeed = 1200,
                        DangerValue = 3,
                        IsDangerous = true,
                        MissileSpellName = "DarkBindingMissile"
                    }
                },
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Aqua Prison",
                        ChampionName = "Nami",
                        SpellName = "NamiQ",
                        Slot = SpellSlot.Q,
                        Delay = 950,
                        Range = 875,
                        Radius = 200,
                        MissileSpeed = 0,
                        DangerValue = 3,
                        IsDangerous = true,
                        MissileSpellName = "NamiQ"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Tidal Wave",
                        ChampionName = "Nami",
                        SpellName = "NamiR",
                        Slot = SpellSlot.R,
                        Delay = 500,
                        Range = 2750,
                        Radius = 250,
                        MissileSpeed = 850,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "NamiRMissile"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Dredge Line",
                        ChampionName = "Nautilus",
                        SpellName = "NautilusAnchorDrag",
                        Slot = SpellSlot.Q,
                        Delay = 250,
                        Range = 1080,
                        Radius = 90,
                        MissileSpeed = 2000,
                        DangerValue = 3,
                        IsDangerous = true,
                        MissileSpellName = "NautilusAnchorDragMissile"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Javelin Toss",
                        ChampionName = "Nidalee",
                        SpellName = "JavelinToss",
                        Slot = SpellSlot.Q,
                        Delay = 125,
                        Range = 1500,
                        Radius = 40,
                        MissileSpeed = 1300,
                        DangerValue = 3,
                        IsDangerous = true,
                        MissileSpellName = "JavelinToss"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Duskbringer",
                        ChampionName = "Nocturne",
                        SpellName = "NocturneDuskbringer",
                        Slot = SpellSlot.Q,
                        Delay = 250,
                        Range = 1125,
                        Radius = 60,
                        MissileSpeed = 1400,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "NocturneDuskbringer"
                    }
                },
                //new LinearMissileSkillshot
                //{
                //    SpellData = new SpellData
                //    {
                //        ChampionName = "Olaf",
                //        SpellName = "OlafAxeThrowCast",
                //        Slot = SpellSlot.Q,
                //        Delay = 250,
                //        Range = 1000,
                //        Radius = 90,
                //        MissileSpeed = 1600,
                //        DangerValue = 3,
                //        MissileSpellName = "OlafAxeThrowCast"
                //    }
                //},
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Commnad: Attack",
                        ChampionName = "Orianna",
                        SpellName = "OrianaIzunaCommand",
                        Slot = SpellSlot.Q,
                        Delay = 0,
                        Range = 2000,
                        Radius = 80,
                        MissileSpeed = 1200,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "OrianaIzunaCommand"
                    }
                },
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Command: Shockwave",
                        ChampionName = "Orianna",
                        SpellName = "OrianaDetonateCommand",
                        Slot = SpellSlot.R,
                        Delay = 500,
                        Range = 410,
                        Radius = 410,
                        MissileSpeed = 0,
                        DangerValue = 5,
                        IsDangerous = true,
                        MissileSpellName = "OrianaDetonateCommand"
                    }
                },
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Command: Dissonance",
                        ChampionName = "Orianna",
                        SpellName = "OrianaDissonanceCommand",
                        Slot = SpellSlot.W,
                        Delay = 250,
                        Range = 1825,
                        Radius = 250,
                        MissileSpeed = 0,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "OrianaDissonanceCommand"
                    }
                },
                //new CircularMissileSkillshot
                //{
                //    SpellData = new SpellData
                //    {
                //        ChampionName = "Pantheon",
                //        SpellName = "PantheonE",
                //        Slot = SpellSlot.E,
                //        Delay = 1000,
                //        Range = 650,
                //        Radius = 100,
                //        MissileSpeed = 0,
                //        DangerValue = 3,
                //        MissileSpellName = "PantheonE"
                //    }
                //},
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Blinding Assault",
                        ChampionName = "Quinn",
                        SpellName = "QuinnQ",
                        Slot = SpellSlot.Q,
                        Delay = 250,
                        Range = 1050,
                        Radius = 80,
                        MissileSpeed = 1550,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "QuinnQMissile"
                    }
                },
                //new LinearMissileSkillshot
                //{
                //    SpellData = new SpellData
                //    {
                //        ChampionName = "RekSai",
                //        SpellName = "reksaiqburrowed",
                //        Slot = SpellSlot.E,
                //        Delay = 125,
                //        Range = 1500,
                //        Radius = 65,
                //        MissileSpeed = 1950,
                //        DangerValue = 3,
                //        MissileSpellName = "RekSaiQBurrowedMis"
                //    }
                //},
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Bola Strike",
                        ChampionName = "Rengar",
                        SpellName = "RengarE",
                        Slot = SpellSlot.E,
                        Delay = 250,
                        Range = 1000,
                        Radius = 70,
                        MissileSpeed = 1500,
                        DangerValue = 3,
                        IsDangerous = true,
                        MissileSpellName = "RengarEFinal"
                    }
                },
                //new LinearMissileSkillshot
                //{
                //    SpellData = new SpellData
                //    {
                //        ChampionName = "Riven",
                //        SpellName = "rivenizunablade",
                //        Slot = SpellSlot.R,
                //        Delay = 250,
                //        Range = 1100,
                //        Radius = 100,
                //        MissileSpeed = 1600,
                //        DangerValue = 3,
                //        MissileSpellName = "rivenizunablade"
                //    }
                //},
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Ki Burst",
                        ChampionName = "Riven",
                        SpellName = "RivenMartyr",
                        Slot = SpellSlot.W,
                        Delay = 267,
                        Range = 650,
                        Radius = 280,
                        MissileSpeed = 1500,
                        DangerValue = 3,
                        IsDangerous = true,
                        MissileSpellName = "RivenMartyr"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Electro-Harpoon",
                        ChampionName = "Rumble",
                        SpellName = "RumbleGrenade",
                        Slot = SpellSlot.E,
                        Delay = 250,
                        Range = 950,
                        Radius = 90,
                        MissileSpeed = 2000,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "RumbleGrenade"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Overload",
                        ChampionName = "Ryze",
                        SpellName = "RyzeQ",
                        Slot = SpellSlot.Q,
                        Delay = 250,
                        Range = 900,
                        Radius = 60,
                        MissileSpeed = 1700,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "RyzeQ"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Arctic Assault",
                        ChampionName = "Sejuani",
                        SpellName = "SejuaniArcticAssault",
                        Slot = SpellSlot.Q,
                        Delay = 0,
                        Range = 900,
                        Radius = 70,
                        MissileSpeed = 1600,
                        DangerValue = 3,
                        IsDangerous = true,
                        MissileSpellName = ""
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Glacial Prison",
                        ChampionName = "Sejuani",
                        SpellName = "SejuaniGlacialPrisonCast",
                        Slot = SpellSlot.R,
                        Delay = 250,
                        Range = 1200,
                        Radius = 110,
                        MissileSpeed = 1600,
                        DangerValue = 3,
                        IsDangerous = true,
                        MissileSpellName = "SejuaniGlacialPrison"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Shadow Dash",
                        ChampionName = "Shen",
                        SpellName = "ShenShadowDash",
                        Slot = SpellSlot.E,
                        Delay = 0,
                        Range = 1600,
                        Radius = 75,
                        MissileSpeed = 1250,
                        DangerValue = 3,
                        IsDangerous = true,
                        MissileSpellName = "ShenShadowDash"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Flame Breath",
                        ChampionName = "Shyvana",
                        SpellName = "ShyvanaFireball",
                        Slot = SpellSlot.E,
                        Delay = 0,
                        Range = 950,
                        Radius = 60,
                        MissileSpeed = 1700,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "ShyvanaFireball"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Dragon's Descent",
                        ChampionName = "Shyvana",
                        SpellName = "ShyvanaTransformCast",
                        Slot = SpellSlot.R,
                        Delay = 250,
                        Range = 1000,
                        Radius = 160,
                        MissileSpeed = 1500,
                        DangerValue = 3,
                        IsDangerous = true,
                        MissileSpellName = "ShyvanaTransformCast"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Roar of the Slayer",
                        ChampionName = "Sion",
                        SpellName = "SionE",
                        Slot = SpellSlot.E,
                        Delay = 250,
                        Range = 800,
                        Radius = 80,
                        MissileSpeed = 1800,
                        DangerValue = 3,
                        IsDangerous = true,
                        MissileSpellName = "SionEMissile"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Boomerang Blade",
                        ChampionName = "Sivir",
                        SpellName = "SivirQ",
                        Slot = SpellSlot.Q,
                        Delay = 250,
                        Range = 1275,
                        Radius = 100,
                        MissileSpeed = 1350,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "SivirQMissile"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Boomerang Blade (return)",
                        ChampionName = "Sivir",
                        SpellName = "SivirQReturn",
                        Slot = SpellSlot.Q,
                        Delay = 250,
                        Range = 1275,
                        Radius = 100,
                        MissileSpeed = 1350,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "SivirQMissileReturn"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Fracture",
                        ChampionName = "Skarner",
                        SpellName = "SkarnerFracture",
                        Slot = SpellSlot.E,
                        Delay = 250,
                        Range = 1000,
                        Radius = 60,
                        MissileSpeed = 1400,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "SkarnerFractureMissile"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Crescendo",
                        ChampionName = "Sona",
                        SpellName = "SonaR",
                        Slot = SpellSlot.R,
                        Delay = 250,
                        Range = 1000,
                        Radius = 150,
                        MissileSpeed = 2400,
                        DangerValue = 5,
                        IsDangerous = true,
                        MissileSpellName = "SonaR"
                    }
                },
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Starcall",
                        ChampionName = "Soraka",
                        SpellName = "SorakaQ",
                        Slot = SpellSlot.Q,
                        Delay = 500,
                        Range = 970,
                        Radius = 260,
                        MissileSpeed = 1100,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "SorakaQ"
                    }
                },
                //new CircularMissileSkillshot
                //{
                //    SpellData = new SpellData
                //    {
                //        ChampionName = "Soraka",
                //        SpellName = "SorakaE",
                //        Slot = SpellSlot.E,
                //        Delay = 1750,
                //        Range = 925,
                //        Radius = 275,
                //        MissileSpeed = 0,
                //        DangerValue = 3,
                //        MissileSpellName = "SorakaE"
                //    }
                //},
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Nevermove",
                        ChampionName = "Swain",
                        SpellName = "SwainShadowGrasp",
                        Slot = SpellSlot.W,
                        Delay = 1100,
                        Range = 900,
                        Radius = 200,
                        MissileSpeed = 0,
                        DangerValue = 3,
                        IsDangerous = true,
                        MissileSpellName = "SwainShadowGrasp"
                    }
                },
                //new LinearMissileSkillshot
                //{
                //    SpellData = new SpellData
                //    {
                //        ChampionName = "Syndra",
                //        SpellName = "SyndraE",
                //        Slot = SpellSlot.E,
                //        Delay = 250,
                //        Range = 800,
                //        Radius = 140,
                //        MissileSpeed = 1500,
                //        DangerValue = 3,
                //        MissileSpellName = "SyndraE"
                //    }
                //},
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Force of Will",
                        ChampionName = "Syndra",
                        SpellName = "syndrawcast",
                        Slot = SpellSlot.W,
                        Delay = 0,
                        Range = 925,
                        Radius = 220,
                        MissileSpeed = 1450,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "syndrawcast"
                    }
                },
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Dark Sphere",
                        ChampionName = "Syndra",
                        SpellName = "SyndraQ",
                        Slot = SpellSlot.Q,
                        Delay = 600,
                        Range = 800,
                        Radius = 210,
                        MissileSpeed = 0,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "SyndraQ"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Tongue Lash",
                        ChampionName = "TahmKench",
                        SpellName = "TahmKenchQ",
                        Slot = SpellSlot.Q,
                        Delay = 250,
                        Range = 951,
                        Radius = 90,
                        MissileSpeed = 2000,
                        DangerValue = 3,
                        IsDangerous = true,
                        MissileSpellName = "tahmkenchqmissile"
                    }
                },
                //new LinearMissileSkillshot
                //{
                //    SpellData = new SpellData
                //    {
                //        ChampionName = "Talon",
                //        SpellName = "TalonRake",
                //        Slot = SpellSlot.W,
                //        Delay = 0,
                //        Range = 780,
                //        Radius = 75,
                //        MissileSpeed = 2300,
                //        DangerValue = 3,
                //        MissileSpellName = "TalonRake"
                //    }
                //},
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Death Sentence",
                        ChampionName = "Thresh",
                        SpellName = "ThreshQ",
                        Slot = SpellSlot.Q,
                        Delay = 500,
                        Range = 1200,
                        Radius = 70,
                        MissileSpeed = 1900,
                        DangerValue = 3,
                        IsDangerous = true,
                        MissileSpellName = "ThreshQMissile"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Flay",
                        ChampionName = "Thresh",
                        SpellName = "ThreshE",
                        Slot = SpellSlot.E,
                        Delay = 125,
                        Range = 1075,
                        Radius = 110,
                        MissileSpeed = 2000,
                        DangerValue = 3,
                        IsDangerous = true,
                        MissileSpellName = "ThreshEMissile1"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Wild Cards",
                        ChampionName = "TwistedFate",
                        SpellName = "disabled/WildCards",
                        Slot = SpellSlot.Q,
                        Delay = 250,
                        Range = 1450,
                        Radius = 40,
                        MissileSpeed = 1000,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "SealFateMissile"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Acid Hunter",
                        ChampionName = "Urgot",
                        SpellName = "UrgotHeatseekingLineMissile",
                        Slot = SpellSlot.Q,
                        Delay = 125,
                        Range = 1000,
                        Radius = 60,
                        MissileSpeed = 1600,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "UrgotHeatseekingLineMissile"
                    }
                },
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Noxian Corrosive Charge",
                        ChampionName = "Urgot",
                        SpellName = "UrgotPlasmaGrenade",
                        Slot = SpellSlot.E,
                        Delay = 250,
                        Range = 900,
                        Radius = 250,
                        MissileSpeed = 1500,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "UrgotPlasmaGrenadeBoom"
                    }
                },
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Hail of Arrows",
                        ChampionName = "Varus",
                        SpellName = "VarusE",
                        Slot = SpellSlot.E,
                        Delay = 1000,
                        Range = 925,
                        Radius = 235,
                        MissileSpeed = 1500,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "VarusE"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Piercing Arrow",
                        ChampionName = "Varus",
                        SpellName = "disabled/varusq",
                        Slot = SpellSlot.Q,
                        Delay = 0,
                        Range = 1600,
                        Radius = 75,
                        MissileSpeed = 1900,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "VarusQMissile"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Chain of Corruption",
                        ChampionName = "Varus",
                        SpellName = "VarusR",
                        Slot = SpellSlot.R,
                        Delay = 250,
                        Range = 1200,
                        Radius = 100,
                        MissileSpeed = 1950,
                        DangerValue = 3,
                        IsDangerous = true,
                        MissileSpellName = "VarusRMissile"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Baleful Strike",
                        ChampionName = "Veigar",
                        SpellName = "VeigarBalefulStrike",
                        Slot = SpellSlot.Q,
                        Delay = 250,
                        Range = 950,
                        Radius = 70,
                        MissileSpeed = 2000,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "VeigarBalefulStrikeMis"
                    }
                },
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Dark Matter",
                        ChampionName = "Veigar",
                        SpellName = "VeigarDarkMatter",
                        Slot = SpellSlot.W,
                        Delay = 1350,
                        Range = 900,
                        Radius = 225,
                        MissileSpeed = 0,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "VeigarDarkMatter"
                    }
                },
                //new CircularMissileSkillshot
                //{
                //    SpellData = new SpellData
                //    {
                //        ChampionName = "Veigar",
                //        SpellName = "VeigarEventHorizon",
                //        Slot = SpellSlot.E,
                //        Delay = 500,
                //        Range = 700,
                //        Radius = 425,
                //        MissileSpeed = 0,
                //        DangerValue = 3,
                //        MissileSpellName = "VeigarEventHorizon"
                //    }
                //},
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Tectonic Disruption",
                        ChampionName = "Velkoz",
                        SpellName = "VelkozE",
                        Slot = SpellSlot.E,
                        Delay = 500,
                        Range = 950,
                        Radius = 225,
                        MissileSpeed = 1500,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "VelkozEMissile"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Void Rift",
                        ChampionName = "Velkoz",
                        SpellName = "VelkozW",
                        Slot = SpellSlot.W,
                        Delay = 250,
                        Range = 1100,
                        Radius = 90,
                        MissileSpeed = 1700,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "VelkozW"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Plasma Fission (split)",
                        ChampionName = "Velkoz",
                        SpellName = "VelkozQMissileSplit",
                        Slot = SpellSlot.Q,
                        Delay = 0,
                        Range = 900,
                        Radius = 90,
                        MissileSpeed = 2100,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "VelkozQMissileSplit"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Plasma Fission",
                        ChampionName = "Velkoz",
                        SpellName = "VelkozQ",
                        Slot = SpellSlot.Q,
                        Delay = 0,
                        Range = 1200,
                        Radius = 90,
                        MissileSpeed = 1300,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "VelkozQMissile"
                    }
                },
                //new LinearMissileSkillshot
                //{
                //    SpellData = new SpellData
                //    {
                //        ChampionName = "Vi",
                //        SpellName = "ViQMissile",
                //        Slot = SpellSlot.Q,
                //        Delay = 250,
                //        Range = 725,
                //        Radius = 90,
                //        MissileSpeed = 1500,
                //        DangerValue = 3,
                //        MissileSpellName = "ViQMissile"
                //    }
                //},
                //new LinearMissileSkillshot //Unknown:
                //{
                //    SpellData = new SpellData
                //    {
                //        ChampionName = "Viktor",
                //        SpellName = "ViktorDeathRay",
                //        Slot = SpellSlot.E,
                //        Delay = 0,
                //        Range = 800,
                //        Radius = 80,
                //        MissileSpeed = 780,
                //        DangerValue = 3,
                //        MissileSpellName = "ViktorDeathRayMissile"
                //    }
                //},
                //new LinearMissileSkillshot
                //{
                //    SpellData = new SpellData
                //    {
                //        ChampionName = "Viktor",
                //        SpellName = "ViktorDeathRay3",
                //        Slot = SpellSlot.E,
                //        Delay = 500,
                //        Range = 800,
                //        Radius = 80,
                //        MissileSpeed = 0,
                //        DangerValue = 3,
                //        MissileSpellName = "ViktorDeathRay3"
                //    }
                //},
                //new LinearMissileSkillshot
                //{
                //    SpellData = new SpellData
                //    {
                //        ChampionName = "Viktor",
                //        SpellName = "ViktorDeathRay2",
                //        Slot = SpellSlot.E,
                //        Delay = 0,
                //        Range = 800,
                //        Radius = 80,
                //        MissileSpeed = 1500,
                //        DangerValue = 3,
                //        MissileSpellName = "ViktorDeathRayMissile2"
                //    }
                //},
                //new CircularMissileSkillshot
                //{
                //    SpellData = new SpellData
                //    {
                //        ChampionName = "Viktor",
                //        SpellName = "ViktorGravitonField",
                //        Slot = SpellSlot.W,
                //        Delay = 1500,
                //        Range = 625,
                //        Radius = 300,
                //        MissileSpeed = 0,
                //        DangerValue = 3,
                //        MissileSpellName = "ViktorGravitonField"
                //    }
                //},
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Hemoplague",
                        ChampionName = "Vladimir",
                        SpellName = "VladimirHemoplague",
                        Slot = SpellSlot.R,
                        Delay = 389,
                        Range = 700,
                        Radius = 375,
                        MissileSpeed = 0,
                        DangerValue = 3,
                        IsDangerous = false,
                        MissileSpellName = "VladimirHemoplague"
                    }
                },
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Eye of Destruction",
                        ChampionName = "Xerath",
                        SpellName = "XerathArcaneBarrage2",
                        Slot = SpellSlot.W,
                        Delay = 700,
                        Range = 1100,
                        Radius = 270,
                        MissileSpeed = 0,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "XerathArcaneBarrage2"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Arcanopulse",
                        ChampionName = "Xerath",
                        SpellName = "xeratharcanopulse2",
                        Slot = SpellSlot.Q,
                        Delay = 500,
                        Range = 1525,
                        Radius = 80,
                        MissileSpeed = 0,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "xeratharcanopulse2"
                    }
                },
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Rite of the Arcane",
                        ChampionName = "Xerath",
                        SpellName = "xerathrmissilewrapper",
                        Slot = SpellSlot.R,
                        Delay = 700,
                        Range = 5600,
                        Radius = 200,
                        MissileSpeed = 0,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "xerathrmissilewrapper"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Shocking Orb",
                        ChampionName = "Xerath",
                        SpellName = "XerathMageSpear",
                        Slot = SpellSlot.E,
                        Delay = 200,
                        Range = 1125,
                        Radius = 60,
                        MissileSpeed = 1400,
                        DangerValue = 2,
                        IsDangerous = true,
                        MissileSpellName = "XerathMageSpearMissile"
                    }
                },
                new LinearMissileSkillshot()
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Steel Tempest (tornado)",
                        ChampionName = "Yasuo",
                        SpellName = "YasuoQ3/disabled",
                        Slot = SpellSlot.Q,
                        Delay = 400,
                        Range = 1150,
                        Radius = 90,
                        MissileSpeed = 1500,
                        DangerValue = 3,
                        MissileSpellName = "YasuoQ3Mis"
                    }
                },             
                new YasuoQ
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Steel Tempest (tornado)",
                        ChampionName = "Yasuo",
                        SpellName = "YasuoQ3",
                        Slot = SpellSlot.Q,
                        Delay = 100,
                        Range = 1150,
                        Radius = 90,
                        MissileSpeed = 1500,
                        DangerValue = 3,
                        MissileSpellName = "YasuoQ3Mis/disabled"
                    }
                },
                new YasuoQ
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Steel Tempest 1",
                        ChampionName = "Yasuo",
                        SpellName = "YasuoQ",
                        Slot = SpellSlot.Q,
                        Delay = 400,
                        Range = 550,
                        Radius = 40,
                        MissileSpeed = 0,
                        DangerValue = 2,
                        IsDangerous = true,
                        MissileSpellName = "yasuoq"
                    }
                },
                new YasuoQ
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Steel Tempest 2",
                        ChampionName = "Yasuo",
                        SpellName = "YasuoQ2",
                        Slot = SpellSlot.Q,
                        Delay = 400,
                        Range = 550,
                        Radius = 40,
                        MissileSpeed = 0,
                        DangerValue = 2,
                        IsDangerous = true,
                        MissileSpellName = "yasuoq2"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Razor Shuriken",
                        ChampionName = "Zed",
                        SpellName = "ZedQ",
                        Slot = SpellSlot.Q,
                        Delay = 300,
                        Range = 925,
                        Radius = 50,
                        MissileSpeed = 1700,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "ZedQMissile"
                    }
                },
                //new CircularMissileSkillshot
                //{
                //    SpellData = new SpellData
                //    {
                //        ChampionName = "Zed",
                //        SpellName = "ZedPBAOEDummy",
                //        Slot = SpellSlot.E,
                //        Delay = 0,
                //        Range = 290,
                //        Radius = 290,
                //        MissileSpeed = 0,
                //        DangerValue = 3,
                //        MissileSpellName = "ZedPBAOEDummy"
                //    }
                //},
                //new CircularMissileSkillshot
                //{
                //    SpellData = new SpellData
                //    {
                //        ChampionName = "Ziggs",
                //        SpellName = "ZiggsE",
                //        Slot = SpellSlot.E,
                //        Delay = 250,
                //        Range = 2000,
                //        Radius = 235,
                //        MissileSpeed = 3000,
                //        DangerValue = 3,
                //        MissileSpellName = "ZiggsE"
                //    }
                //},
                //new CircularMissileSkillshot
                //{
                //    SpellData = new SpellData
                //    {
                //        ChampionName = "Ziggs",
                //        SpellName = "ZiggsW",
                //        Slot = SpellSlot.W,
                //        Delay = 250,
                //        Range = 2000,
                //        Radius = 275,
                //        MissileSpeed = 3000,
                //        DangerValue = 3,
                //        MissileSpellName = "ZiggsW"
                //    }
                //},
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Bouncing Bomb",
                        ChampionName = "Ziggs",
                        SpellName = "ZiggsQ",
                        Slot = SpellSlot.Q,
                        Delay = 250,
                        Range = 850,
                        Radius = 150,
                        MissileSpeed = 1700,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "ZiggsQSpell"
                    }
                },
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Mega Inferno Bomb",
                        ChampionName = "Ziggs",
                        SpellName = "ZiggsR",
                        Slot = SpellSlot.R,
                        Delay = 1500,
                        Range = 5300,
                        Radius = 550,
                        MissileSpeed = 1500,
                        DangerValue = 3,
                        MissileSpellName = "ZiggsR"
                    }
                },
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Time Bomb",
                        ChampionName = "Zilean",
                        SpellName = "ZileanQ",
                        Slot = SpellSlot.Q,
                        Delay = 300,
                        Range = 900,
                        Radius = 250,
                        MissileSpeed = 2000,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "ZileanQ"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Grasping Roots",
                        ChampionName = "Zyra",
                        SpellName = "ZyraE",
                        Slot = SpellSlot.E,
                        Delay = 250,
                        Range = 1150,
                        Radius = 70,
                        MissileSpeed = 1150,
                        DangerValue = 3,
                        IsDangerous = true,
                        MissileSpellName = "ZyraEMissile"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Deadly Bloom (passive)",
                        ChampionName = "Zyra",
                        SpellName = "zyrapassivedeathmanager",
                        Slot = SpellSlot.Internal,
                        Delay = 500,
                        Range = 1474,
                        Radius = 80,
                        MissileSpeed = 2000,
                        DangerValue = 3,
                        IsDangerous = true,
                        MissileSpellName = "zyrapassivedeathmanager"
                    }
                },
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Rampant Growth",
                        ChampionName = "Zyra",
                        SpellName = "ZyraQFissure",
                        Slot = SpellSlot.Q,
                        Delay = 800,
                        Range = 825,
                        Radius = 260,
                        MissileSpeed = 0,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "ZyraQFissure"
                    }
                },
                new CircularMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Stranglethorns",
                        ChampionName = "Zyra",
                        SpellName = "ZyraBrambleZone",
                        Slot = SpellSlot.R,
                        Delay = 500,
                        Range = 700,
                        Radius = 525,
                        MissileSpeed = 0,
                        DangerValue = 5,
                        IsDangerous = true,
                        MissileSpellName = "ZyraBrambleZone"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Tentacle Smash",
                        ChampionName = "Illaoi",
                        SpellName = "IllaoiQ",
                        Slot = SpellSlot.Q,
                        Delay = 750,
                        Range = 850,
                        Radius = 100,
                        MissileSpeed = 0,
                        DangerValue = 3,
                        IsDangerous = true,
                        MissileSpellName = "illaoiemis"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Test of Spirit",
                        ChampionName = "Illaoi",
                        SpellName = "IllaoiE",
                        Slot = SpellSlot.E,
                        Delay = 250,
                        Range = 950,
                        Radius = 50,
                        MissileSpeed = 1900,
                        DangerValue = 3,
                        IsDangerous = true,
                        MissileSpellName = "illaoiemis"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "End of the Line",
                        ChampionName = "Graves",
                        SpellName = "GravesQLineSpell",
                        Slot = SpellSlot.Q,
                        Delay = 250,
                        Range = 808,
                        Radius = 40,
                        MissileSpeed = 3000,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "GravesQLineMis"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Hammer Shock",
                        ChampionName = "Poppy",
                        SpellName = "PoppyQ",
                        Slot = SpellSlot.Q,
                        Delay = 500,
                        Range = 430,
                        Radius = 100,
                        MissileSpeed = 0,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "PoppyQ"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Deadly Flourish",
                        ChampionName = "Jhin",
                        SpellName = "JhinW",
                        Slot = SpellSlot.W,
                        Delay = 250,
                        Range = 3000,
                        Radius = 40,
                        MissileSpeed = 5000,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "JhinWMissile"
                    }
                },
                new LinearMissileSkillshot
                {
                    SpellData = new SpellData
                    {
                        DisplayName = "Curtain Call",
                        ChampionName = "Jhin",
                        SpellName = "JhinRShot",
                        Slot = SpellSlot.R,
                        Delay = 250,
                        Range = 3500,
                        Radius = 80,
                        MissileSpeed = 5000,
                        DangerValue = 2,
                        IsDangerous = false,
                        MissileSpellName = "JhinRShotMis"
                    }
                },
            };
        }
    }
}
