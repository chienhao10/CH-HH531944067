using System;
using System.Collections.Generic;
using EloBuddy;
using LeagueSharp.Common;
using SharpDX;

namespace LCS_Janna.Plugins
{
    [Flags]
    public enum EvadeMethods
    {
        Default = 0,
        Blink = 1,
        SpellShield = 2,
        Dash = 4,
        Invulnerability = 8,
        AllyTargetted = 16, //soon tm
        EnemyTargetted = 32, //soon tm
        None = 64,
    }

    [Flags]
    public enum Collisions
    {
        None = 0,
        Minions = 1,
        Champions = 2,
        YasuoWall = 4,
    }

    public struct ArcData
    {
        public float Width;
        public float Height;
        public float Radius;
        public Vector2 Pos;
        public float Angle;
    }

    public class SpellData
    {
        public string ChampionName;
        public string SpellName;
        public SpellSlot Slot;
        public bool IsArc;
        public ArcData ArcData;
        public bool IsSkillshot;
        public SkillshotType Type;
        public int Delay;
        public int Range;
        public int Radius;
        public int MissileSpeed;
        public bool IsDangerous;
        public string MissileSpellName;
        public EvadeMethods EvadeMethods;
        public Collisions Collisionable;
        public bool IsTargeted;
        public bool IsAAEmpower;
    }

    public class TrundleSpellData : SpellData
    {
        public float[] Percent;
        public float[] Extra;
        public bool IsDecaying;
        public float DecaysTo;
        public float DecayTime;
    }

    public class EscapeSpellData : SpellData
    {
        public float[] Percent;
        public float[] Extra;
        public bool IsDecaying;
        public float DecaysTo;
        public float DecayTime;
    }

    public class SpellDatabase
    {
        public static List<SpellData> EvadeableSpells;
        public static List<SpellData> TargetedSpells;
        public static List<SpellData> EscapeSpells;
        public static List<TrundleSpellData> TrundleSpells;

        private static bool blInitialized = false;

        public static void InitalizeSpellDatabase()
        {
            if (blInitialized)
                return;

            blInitialized = true;

            EvadeableSpells = new List<SpellData>();
            TargetedSpells = new List<SpellData>();
            EscapeSpells = new List<SpellData>();
            #region Dangreous Spell Database
            //diana q x axis eliptic radius 315
            EvadeableSpells.Add(
                new SpellData
                {
                    ChampionName = "Ahri",
                    SpellName = "AhriSeduce",
                    Slot = SpellSlot.E,
                    IsSkillshot = true,
                    Type = SkillshotType.SkillshotLine,
                    Delay = 250,
                    Range = 1000,
                    Radius = 60,
                    MissileSpeed = 1550,
                    IsDangerous = true,
                    MissileSpellName = "AhriSeduceMissile",
                    EvadeMethods = EvadeMethods.Blink | EvadeMethods.SpellShield | EvadeMethods.Dash,
                    Collisionable = Collisions.Champions | Collisions.Minions | Collisions.YasuoWall
                });

            EvadeableSpells.Add(
                new SpellData
                {
                    ChampionName = "Amumu",
                    SpellName = "BandageToss",
                    Slot = SpellSlot.Q,
                    IsSkillshot = true,
                    Type = SkillshotType.SkillshotLine,
                    Delay = 250,
                    Range = 1100,
                    Radius = 90,
                    MissileSpeed = 2000,
                    IsDangerous = true,
                    MissileSpellName = "SadMummyBandageToss",
                    EvadeMethods = EvadeMethods.Blink | EvadeMethods.SpellShield | EvadeMethods.Dash,
                    Collisionable = Collisions.Champions | Collisions.Minions | Collisions.YasuoWall

                });

            EvadeableSpells.Add(
                new SpellData
                {
                    ChampionName = "Amumu",
                    SpellName = "CurseoftheSadMummy",
                    Slot = SpellSlot.R,
                    IsSkillshot = true,
                    Type = SkillshotType.SkillshotCircle,
                    Delay = 250,
                    Range = 0,
                    Radius = 550,
                    MissileSpeed = int.MaxValue,
                    IsDangerous = true,
                    EvadeMethods = EvadeMethods.Blink | EvadeMethods.SpellShield | EvadeMethods.Dash
                });

            EvadeableSpells.Add(
                new SpellData
                {
                    ChampionName = "Annie",
                    SpellName = "InfernalGuardian",
                    Slot = SpellSlot.R,
                    IsSkillshot = true,
                    Type = SkillshotType.SkillshotCircle,
                    Delay = 250,
                    Range = 600,
                    Radius = 251,
                    MissileSpeed = int.MaxValue,
                    IsDangerous = true,
                    MissileSpellName = "",
                    EvadeMethods = EvadeMethods.Blink | EvadeMethods.SpellShield | EvadeMethods.Dash
                });

            EvadeableSpells.Add(
                new SpellData
                {
                    ChampionName = "Ashe",
                    SpellName = "EnchantedCrystalArrow",
                    Slot = SpellSlot.R,
                    IsSkillshot = true,
                    Type = SkillshotType.SkillshotLine,
                    Delay = 250,
                    Range = 20000,
                    Radius = 130,
                    MissileSpeed = 1600,
                    IsDangerous = true,
                    MissileSpellName = "EnchantedCrystalArrow",
                    EvadeMethods = EvadeMethods.Blink | EvadeMethods.SpellShield | EvadeMethods.Dash,
                    Collisionable = Collisions.Champions | Collisions.YasuoWall
                });

            EvadeableSpells.Add(
                new SpellData
                {
                    ChampionName = "Bard",
                    SpellName = "BardR",
                    Slot = SpellSlot.R,
                    IsSkillshot = true,
                    Type = SkillshotType.SkillshotCircle,
                    Delay = 500,
                    Range = 3400,
                    Radius = 350,
                    MissileSpeed = 2100,
                    IsDangerous = false,
                    MissileSpellName = "BardR",
                    EvadeMethods = EvadeMethods.Blink | EvadeMethods.SpellShield | EvadeMethods.Dash,
                });

            EvadeableSpells.Add(
                new SpellData
                {
                    ChampionName = "Blitzcrank",
                    SpellName = "RocketGrab",
                    Slot = SpellSlot.Q,
                    IsSkillshot = true,
                    Type = SkillshotType.SkillshotLine,
                    Delay = 250,
                    Range = 1050,
                    Radius = 70,
                    MissileSpeed = 1800,
                    IsDangerous = true,
                    MissileSpellName = "RocketGrabMissile",
                    EvadeMethods = EvadeMethods.Blink | EvadeMethods.SpellShield | EvadeMethods.Dash,
                    Collisionable = Collisions.Champions | Collisions.YasuoWall
                });

            EvadeableSpells.Add(
                new SpellData
                {
                    ChampionName = "Braum",
                    SpellName = "BraumRWrapper",
                    Slot = SpellSlot.R,
                    IsSkillshot = true,
                    Type = SkillshotType.SkillshotLine,
                    Delay = 500,
                    Range = 1200,
                    Radius = 115,
                    MissileSpeed = 1400,
                    IsDangerous = true,
                    MissileSpellName = "braumrmissile",
                    EvadeMethods = EvadeMethods.Blink | EvadeMethods.SpellShield | EvadeMethods.Dash,
                    Collisionable = Collisions.YasuoWall
                });

            EvadeableSpells.Add(
                new SpellData
                {
                    ChampionName = "Diana",
                    SpellName = "DianaArc",
                    Slot = SpellSlot.Q,
                    IsSkillshot = true,
                    Type = SkillshotType.SkillshotCircle,
                    Delay = 250,
                    Range = 895,
                    Radius = 195,
                    IsArc = true,
                    ArcData = new ArcData
                    {
                        Pos = new Vector2(875 / 2f, 20),
                        Angle = (float)Math.PI,
                        Width = 410,
                        Height = 200,
                        Radius = 120
                    },
                    MissileSpeed = 1600,
                    IsDangerous = true,
                    MissileSpellName = "DianaArc",
                    EvadeMethods = EvadeMethods.Blink | EvadeMethods.SpellShield | EvadeMethods.Dash,
                    Collisionable = Collisions.YasuoWall
                });

            EvadeableSpells.Add(
                new SpellData
                {
                    ChampionName = "Draven",
                    SpellName = "DravenDoubleShot",
                    Slot = SpellSlot.E,
                    IsSkillshot = true,
                    Type = SkillshotType.SkillshotLine,
                    Delay = 250,
                    Range = 1100,
                    Radius = 130,
                    MissileSpeed = 1400,
                    IsDangerous = true,
                    MissileSpellName = "DravenDoubleShotMissile",
                    EvadeMethods = EvadeMethods.Blink | EvadeMethods.SpellShield,
                    Collisionable = Collisions.YasuoWall
                });

            EvadeableSpells.Add(
                new SpellData
                {
                    ChampionName = "Elise",
                    SpellName = "EliseHumanE",
                    IsSkillshot = true,
                    Slot = SpellSlot.E,
                    Type = SkillshotType.SkillshotLine,
                    Delay = 250,
                    Range = 1100,
                    Radius = 55,
                    MissileSpeed = 1600,
                    IsDangerous = true,
                    MissileSpellName = "EliseHumanE",
                    EvadeMethods = EvadeMethods.Blink | EvadeMethods.SpellShield | EvadeMethods.Dash,
                    Collisionable = Collisions.YasuoWall
                });

            EvadeableSpells.Add(
                new SpellData
                {
                    ChampionName = "Evelynn",
                    SpellName = "EvelynnR",
                    IsSkillshot = true,
                    Slot = SpellSlot.R,
                    Type = SkillshotType.SkillshotCircle,
                    Delay = 250,
                    Range = 650,
                    Radius = 350,
                    MissileSpeed = int.MaxValue,
                    IsDangerous = true,
                    MissileSpellName = "EvelynnR",
                    EvadeMethods = EvadeMethods.Blink | EvadeMethods.SpellShield | EvadeMethods.Dash
                });

            EvadeableSpells.Add(
                new SpellData
                {
                    ChampionName = "Cassiopeia",
                    SpellName = "CassiopeiaPetrifyingGaze",
                    IsSkillshot = true,
                    Slot = SpellSlot.R,
                    Type = SkillshotType.SkillshotCone,
                    Delay = 600,
                    Range = 825,
                    Radius = 80,
                    MissileSpeed = int.MaxValue,
                    IsDangerous = true,
                    MissileSpellName = "CassiopeiaPetrifyingGaze",
                    EvadeMethods = EvadeMethods.Blink | EvadeMethods.SpellShield | EvadeMethods.Dash
                });

            EvadeableSpells.Add(
                new SpellData
                {
                    ChampionName = "Fizz",
                    SpellName = "FizzMarinerDoom",
                    Slot = SpellSlot.R,
                    IsSkillshot = true,
                    Type = SkillshotType.SkillshotLine,
                    Delay = 250,
                    Range = 1300,
                    Radius = 120,
                    MissileSpeed = 1350,
                    IsDangerous = true,
                    MissileSpellName = "FizzMarinerDoomMissile",
                    Collisionable = Collisions.Champions | Collisions.YasuoWall,
                    EvadeMethods = EvadeMethods.Blink | EvadeMethods.SpellShield | EvadeMethods.Dash
                });

            EvadeableSpells.Add(
                new SpellData
                {
                    ChampionName = "Galio",
                    SpellName = "GalioIdolOfDurand",
                    Slot = SpellSlot.R,
                    IsSkillshot = true,
                    Type = SkillshotType.SkillshotCircle,
                    Delay = 250,
                    Range = 0,
                    Radius = 550,
                    MissileSpeed = int.MaxValue,
                    IsDangerous = true,
                    MissileSpellName = "",
                    EvadeMethods = EvadeMethods.Blink | EvadeMethods.SpellShield | EvadeMethods.Dash
                });

            EvadeableSpells.Add(
                new SpellData
                {
                    ChampionName = "Gnar",
                    SpellName = "GnarR",
                    Slot = SpellSlot.R,
                    IsSkillshot = true,
                    Type = SkillshotType.SkillshotCircle,
                    Delay = 250,
                    Range = 0,
                    Radius = 500,
                    MissileSpeed = int.MaxValue,
                    IsDangerous = true,
                    MissileSpellName = "",
                    EvadeMethods = EvadeMethods.Blink | EvadeMethods.SpellShield | EvadeMethods.Dash
                });

            EvadeableSpells.Add(
                new SpellData
                {
                    ChampionName = "Gragas",
                    SpellName = "GragasE",
                    Slot = SpellSlot.E,
                    IsSkillshot = true,
                    Type = SkillshotType.SkillshotLine,
                    Delay = 0,
                    Range = 950,
                    Radius = 200,
                    MissileSpeed = 1200,
                    IsDangerous = false,
                    MissileSpellName = "GragasE",
                    Collisionable = Collisions.Champions | Collisions.Minions,
                    EvadeMethods = EvadeMethods.Blink | EvadeMethods.SpellShield | EvadeMethods.Dash
                });



            EvadeableSpells.Add(
                new SpellData
                {
                    ChampionName = "Gragas",
                    SpellName = "GragasR",
                    Slot = SpellSlot.R,
                    IsSkillshot = true,
                    Type = SkillshotType.SkillshotCircle,
                    Delay = 250,
                    Range = 1050,
                    Radius = 375,
                    MissileSpeed = 1800,
                    IsDangerous = true,
                    MissileSpellName = "GragasRBoom",
                    Collisionable = Collisions.YasuoWall,
                    EvadeMethods = EvadeMethods.Blink | EvadeMethods.SpellShield | EvadeMethods.Dash
                });

            EvadeableSpells.Add(
                new SpellData
                {
                    ChampionName = "Graves",
                    SpellName = "GravesChargeShot",
                    Slot = SpellSlot.R,
                    IsSkillshot = true,
                    Type = SkillshotType.SkillshotLine,
                    Delay = 250,
                    Range = 1100,
                    Radius = 100,
                    MissileSpeed = 2100,
                    IsDangerous = true,
                    MissileSpellName = "GravesChargeShotShot",
                    Collisionable = Collisions.YasuoWall,
                    EvadeMethods = EvadeMethods.Blink | EvadeMethods.SpellShield | EvadeMethods.Dash
                });

            EvadeableSpells.Add(
                new SpellData
                {
                    ChampionName = "Leona",
                    SpellName = "LeonaSolarFlare",
                    Slot = SpellSlot.R,
                    IsSkillshot = true,
                    Type = SkillshotType.SkillshotCircle,
                    Delay = 1000,
                    Range = 1200,
                    Radius = 300,
                    MissileSpeed = int.MaxValue,
                    IsDangerous = true,
                    MissileSpellName = "LeonaSolarFlare",
                    EvadeMethods = EvadeMethods.Blink | EvadeMethods.SpellShield | EvadeMethods.Dash
                });

            EvadeableSpells.Add(
                new SpellData
                {
                    ChampionName = "Leona",
                    SpellName = "LeonaZenithBlade",
                    Slot = SpellSlot.E,
                    IsSkillshot = true,
                    Type = SkillshotType.SkillshotLine,
                    Delay = 250,
                    Range = 905,
                    Radius = 70,
                    MissileSpeed = 2000,
                    IsDangerous = true,
                    MissileSpellName = "LeonaZenithBladeMissile",
                    Collisionable = Collisions.YasuoWall,
                    EvadeMethods = EvadeMethods.Blink | EvadeMethods.SpellShield | EvadeMethods.Dash
                });

            EvadeableSpells.Add(
                new SpellData
                {
                    ChampionName = "Malphite",
                    SpellName = "UFSlash",
                    Slot = SpellSlot.R,
                    IsSkillshot = true,
                    Type = SkillshotType.SkillshotCircle,
                    Delay = 0,
                    Range = 1000,
                    Radius = 270,
                    MissileSpeed = 1500,
                    IsDangerous = true,
                    MissileSpellName = "UFSlash",
                    EvadeMethods = EvadeMethods.Blink | EvadeMethods.SpellShield | EvadeMethods.Dash
                });

            EvadeableSpells.Add(
                new SpellData
                {
                    ChampionName = "Morgana",
                    SpellName = "DarkBindingMissile",
                    Slot = SpellSlot.Q,
                    IsSkillshot = true,
                    Type = SkillshotType.SkillshotLine,
                    Delay = 250,
                    Range = 1300,
                    Radius = 80,
                    MissileSpeed = 1200,
                    IsDangerous = true,
                    MissileSpellName = "DarkBindingMissile",
                    EvadeMethods = EvadeMethods.Blink | EvadeMethods.SpellShield | EvadeMethods.Dash,
                    Collisionable = Collisions.Champions | Collisions.Minions | Collisions.YasuoWall
                });


            EvadeableSpells.Add(
                new SpellData
                {
                    ChampionName = "Nami",
                    SpellName = "NamiQ",
                    Slot = SpellSlot.Q,
                    IsSkillshot = true,
                    Type = SkillshotType.SkillshotCircle,
                    Delay = 950,
                    Range = 1625,
                    Radius = 150,
                    MissileSpeed = int.MaxValue,
                    IsDangerous = true,
                    MissileSpellName = "namiqmissile",
                    EvadeMethods = EvadeMethods.Blink | EvadeMethods.SpellShield | EvadeMethods.Dash
                });


            EvadeableSpells.Add(
                new SpellData
                {
                    ChampionName = "Nautilus",
                    SpellName = "NautilusAnchorDrag",
                    IsSkillshot = true,
                    Slot = SpellSlot.Q,
                    Type = SkillshotType.SkillshotLine,
                    Delay = 250,
                    Range = 1100,
                    Radius = 90,
                    MissileSpeed = 2000,
                    IsDangerous = true,
                    MissileSpellName = "NautilusAnchorDragMissile",
                    EvadeMethods = EvadeMethods.Blink | EvadeMethods.SpellShield | EvadeMethods.Dash,
                    Collisionable = Collisions.Champions | Collisions.Minions | Collisions.YasuoWall
                });


            EvadeableSpells.Add(
                new SpellData
                {
                    ChampionName = "Rengar",
                    SpellName = "RengarE",
                    Slot = SpellSlot.E,
                    IsSkillshot = true,
                    Type = SkillshotType.SkillshotLine,
                    Delay = 250,
                    Range = 1000,
                    Radius = 70,
                    MissileSpeed = 1500,
                    IsDangerous = true,
                    MissileSpellName = "RengarEFinal",
                    EvadeMethods = EvadeMethods.Blink | EvadeMethods.SpellShield | EvadeMethods.Dash,
                    Collisionable = Collisions.Champions | Collisions.Minions | Collisions.YasuoWall
                });


            EvadeableSpells.Add(
                new SpellData
                {
                    ChampionName = "Sona",
                    SpellName = "SonaR",
                    Slot = SpellSlot.R,
                    IsSkillshot = true,
                    Type = SkillshotType.SkillshotLine,
                    Delay = 250,
                    Range = 1000,
                    Radius = 140,
                    MissileSpeed = 2400,
                    IsDangerous = true,
                    MissileSpellName = "SonaR",
                    EvadeMethods = EvadeMethods.Blink | EvadeMethods.SpellShield | EvadeMethods.Dash,
                    Collisionable = Collisions.YasuoWall
                });



            EvadeableSpells.Add(
                new SpellData
                {
                    ChampionName = "Swain",
                    SpellName = "SwainShadowGrasp",
                    Slot = SpellSlot.W,
                    IsSkillshot = true,
                    Type = SkillshotType.SkillshotCircle,
                    Delay = 1100,
                    Range = 900,
                    Radius = 180,
                    MissileSpeed = int.MaxValue,
                    IsDangerous = true,
                    MissileSpellName = "SwainShadowGrasp",
                    EvadeMethods = EvadeMethods.Blink | EvadeMethods.SpellShield | EvadeMethods.Dash,
                });

            EvadeableSpells.Add(
                new SpellData
                {
                    ChampionName = "Syndra",
                    SpellName = "syndrae5",
                    Slot = SpellSlot.E,
                    IsSkillshot = true,
                    Type = SkillshotType.SkillshotLine,
                    Delay = 300,
                    Range = 950,
                    Radius = 90,
                    MissileSpeed = 1601,
                    IsDangerous = false,
                    MissileSpellName = "syndrae5",
                    EvadeMethods = EvadeMethods.Blink | EvadeMethods.SpellShield | EvadeMethods.Dash
                });

            EvadeableSpells.Add(
                new SpellData
                {
                    ChampionName = "Syndra",
                    SpellName = "SyndraE",
                    Slot = SpellSlot.E,
                    IsSkillshot = true,
                    Type = SkillshotType.SkillshotLine,
                    Delay = 300,
                    Range = 950,
                    Radius = 90,
                    MissileSpeed = 1601,
                    IsDangerous = false,
                    MissileSpellName = "SyndraE",
                    EvadeMethods = EvadeMethods.Blink | EvadeMethods.SpellShield | EvadeMethods.Dash,
                });

            EvadeableSpells.Add(
                new SpellData
                {
                    ChampionName = "Thresh",
                    SpellName = "ThreshQ",
                    Slot = SpellSlot.Q,
                    IsSkillshot = true,
                    Type = SkillshotType.SkillshotLine,
                    Delay = 500,
                    Range = 1100,
                    Radius = 70,
                    MissileSpeed = 1900,
                    IsDangerous = true,
                    MissileSpellName = "ThreshQMissile",
                    EvadeMethods = EvadeMethods.Blink | EvadeMethods.SpellShield | EvadeMethods.Dash,
                    Collisionable = Collisions.Champions | Collisions.Minions | Collisions.YasuoWall
                });


            EvadeableSpells.Add(
                new SpellData
                {
                    ChampionName = "Varus",
                    SpellName = "VarusQMissilee",
                    Slot = SpellSlot.Q,
                    IsSkillshot = true,
                    Type = SkillshotType.SkillshotLine,
                    Delay = 250,
                    Range = 1800,
                    Radius = 70,
                    MissileSpeed = 1900,
                    IsDangerous = false,
                    MissileSpellName = "VarusQMissile",
                    EvadeMethods = EvadeMethods.Blink | EvadeMethods.SpellShield | EvadeMethods.Dash,
                    Collisionable = Collisions.YasuoWall
                });

            EvadeableSpells.Add(
                new SpellData
                {
                    ChampionName = "Varus",
                    SpellName = "VarusR",
                    Slot = SpellSlot.R,
                    IsSkillshot = true,
                    Type = SkillshotType.SkillshotLine,
                    Delay = 250,
                    Range = 1200,
                    Radius = 120,
                    MissileSpeed = 1950,
                    IsDangerous = true,
                    MissileSpellName = "VarusRMissile",
                    EvadeMethods = EvadeMethods.Blink | EvadeMethods.SpellShield | EvadeMethods.Dash,
                    Collisionable = Collisions.Champions | Collisions.YasuoWall
                });

            EvadeableSpells.Add(
                new SpellData
                {
                    ChampionName = "Velkoz",
                    SpellName = "VelkozE",
                    Slot = SpellSlot.E,
                    IsSkillshot = true,
                    Type = SkillshotType.SkillshotCircle,
                    Delay = 500,
                    Range = 800,
                    Radius = 225,
                    MissileSpeed = 1500,
                    IsDangerous = false,
                    MissileSpellName = "VelkozEMissile",
                    EvadeMethods = EvadeMethods.Blink | EvadeMethods.SpellShield | EvadeMethods.Dash,
                });

            EvadeableSpells.Add(
                new SpellData
                {
                    ChampionName = "Yasuo",
                    SpellName = "yasuoq3w",
                    Slot = SpellSlot.Q,
                    IsSkillshot = true,
                    Type = SkillshotType.SkillshotLine,
                    Delay = 500,
                    Range = 1150,
                    Radius = 90,
                    MissileSpeed = 1500,
                    IsDangerous = true,
                    MissileSpellName = "yasuoq3w",
                    Collisionable = Collisions.YasuoWall,
                    EvadeMethods = EvadeMethods.Blink | EvadeMethods.SpellShield | EvadeMethods.Dash,
                });

            EvadeableSpells.Add(
                new SpellData
                {
                    ChampionName = "Zyra",
                    SpellName = "ZyraGraspingRoots",
                    Slot = SpellSlot.E,
                    IsSkillshot = true,
                    Type = SkillshotType.SkillshotLine,
                    Delay = 250,
                    Range = 1150,
                    Radius = 70,
                    MissileSpeed = 1150,
                    IsDangerous = true,
                    MissileSpellName = "ZyraGraspingRoots",
                    Collisionable = Collisions.YasuoWall,
                    EvadeMethods = EvadeMethods.Blink | EvadeMethods.SpellShield | EvadeMethods.Dash,
                });

            EvadeableSpells.Add(
                new SpellData
                {
                    ChampionName = "Zyra",
                    SpellName = "zyrapassivedeathmanager",
                    Slot = SpellSlot.E,
                    IsSkillshot = true,
                    Type = SkillshotType.SkillshotLine,
                    Delay = 500,
                    Range = 1474,
                    Radius = 70,
                    MissileSpeed = 2000,
                    IsDangerous = true,
                    MissileSpellName = "zyrapassivedeathmanager",
                    Collisionable = Collisions.YasuoWall,
                    EvadeMethods = EvadeMethods.Blink | EvadeMethods.SpellShield | EvadeMethods.Dash,
                });
            #endregion
            #region Escape Spell Data
            EscapeSpells.Add(
                new SpellData
                {
                    ChampionName = "Lucian",
                    SpellName = "LucianE",
                    Slot = SpellSlot.E,
                });
            EscapeSpells.Add(
               new SpellData
               {
                   ChampionName = "Graves",
                   SpellName = "GravesMove",
                   Slot = SpellSlot.E,
               });
            EscapeSpells.Add(
               new SpellData
               {
                   ChampionName = "Vayne",
                   SpellName = "VayneTumble",
                   Slot = SpellSlot.Q,
               });
            EscapeSpells.Add(
               new SpellData
               {
                   ChampionName = "Corki",
                   SpellName = "CarpetBomb",
                   Slot = SpellSlot.W,
               });
            EscapeSpells.Add(
               new SpellData
               {
                   ChampionName = "Ekko",
                   SpellName = "EkkoE",
                   Slot = SpellSlot.E,
               });
            EscapeSpells.Add(
               new SpellData
               {
                   ChampionName = "Caitlyn",
                   SpellName = "CaitlynEntrapment",
                   Slot = SpellSlot.E,
               });
            EscapeSpells.Add(
               new SpellData
               {
                   ChampionName = "Gnar",
                   SpellName = "GnarE",
                   Slot = SpellSlot.E,
               });
            EscapeSpells.Add(
               new SpellData
               {
                   ChampionName = "Fiora",
                   SpellName = "FioraQ",
                   Slot = SpellSlot.Q,
               });
            EscapeSpells.Add(
               new SpellData
               {
                   ChampionName = "Kassadin",
                   SpellName = "RiftWalk",
                   Slot = SpellSlot.R,
               });
            EscapeSpells.Add(
               new SpellData
               {
                   ChampionName = "Fizz",
                   SpellName = "FizzJump",
                   Slot = SpellSlot.E,
               });
            EscapeSpells.Add(
               new SpellData
               {
                   ChampionName = "Riven",
                   SpellName = "RivenFeint",
                   Slot = SpellSlot.E,
               });

            EscapeSpells.Add(
               new SpellData
               {
                   ChampionName = "Kindred",
                   SpellName = "KindredQ",
                   Slot = SpellSlot.Q,
               });

            #endregion
            #region Trundle Tricks Data

            TrundleSpells = new List<TrundleSpellData>();
            TrundleSpells.Add(new TrundleSpellData
            {
                ChampionName = "Caitlyn",
                SpellName = "CaitlynEntrapment",
                Slot = SpellSlot.E,
            });

            TrundleSpells.Add(new TrundleSpellData
            {
                ChampionName = "KhaZix",
                SpellName = "soontm",
                Slot = SpellSlot.E
            });

            TrundleSpells.Add(new TrundleSpellData
            {
                ChampionName = "LeeSin",
                SpellName = "BlindMonkQOne",
                Slot = SpellSlot.Q,
            });

            TrundleSpells.Add(new TrundleSpellData
            {
                ChampionName = "Tristana",
                SpellName = "RocketJump",
                Slot = SpellSlot.W,
            });

            TrundleSpells.Add(new TrundleSpellData
            {
                ChampionName = "Vi",
                SpellName = "ViQMissile",
                Slot = SpellSlot.Q,
            });

            TrundleSpells.Add(new TrundleSpellData
            {
                ChampionName = "Blitzcrank",
                SpellName = "RocketGrab",
                Slot = SpellSlot.Q,
            });


            #endregion
            #region Targeted Spell Database
            TargetedSpells.Add(
                new SpellData
                {
                    ChampionName = "Malzahar",
                    Slot = SpellSlot.R,
                    SpellName = "AlZaharNetherGrasp",
                    IsTargeted = true,
                    IsDangerous = true
                });

            TargetedSpells.Add(
                new SpellData
                {
                    ChampionName = "Mordekaiser",
                    Slot = SpellSlot.R,
                    SpellName = "MordekaiserChildrenOfTheGrave",
                    IsTargeted = true
                });

            TargetedSpells.Add(
                new SpellData
                {
                    ChampionName = "Nocturne",
                    Slot = SpellSlot.R,
                    SpellName = "NocturneParanoia",
                    IsTargeted = true,
                    IsDangerous = true
                });

            TargetedSpells.Add(
                new SpellData
                {
                    ChampionName = "Rammus",
                    Slot = SpellSlot.E,
                    SpellName = "PuncturingTaunt",
                    IsDangerous = true,
                    IsTargeted = true
                });

            TargetedSpells.Add(
                new SpellData
                {
                    ChampionName = "Skarner",
                    Slot = SpellSlot.R,
                    SpellName = "SkarnerImpale",
                    IsDangerous = true,
                    IsTargeted = true
                });

            TargetedSpells.Add(
                new SpellData
                {
                    ChampionName = "Vladimir",
                    Slot = SpellSlot.Q,
                    SpellName = "VladimirTransfusion",
                    IsTargeted = true
                });

            TargetedSpells.Add(
                new SpellData
                {
                    ChampionName = "Vladimir",
                    Slot = SpellSlot.R,
                    SpellName = "VladimirHemoplague",
                    IsDangerous = true
                });

            TargetedSpells.Add(
                new SpellData
                {
                    ChampionName = "Warwick",
                    Slot = SpellSlot.R,
                    SpellName = "InfiniteDuress",
                    IsTargeted = true,
                    IsDangerous = true
                });

            TargetedSpells.Add(
                new SpellData
                {
                    ChampionName = "Zed",
                    Slot = SpellSlot.R,
                    SpellName = "zedulttargetmark",
                    IsTargeted = true,
                    IsDangerous = true
                });

            TargetedSpells.Add(
                new SpellData
                {
                    ChampionName = "Amumu",
                    Slot = SpellSlot.R,
                    SpellName = "CurseoftheSadMummy",
                    Radius = 550,
                    IsDangerous = true,
                });

            TargetedSpells.Add(
                new SpellData
                {
                    ChampionName = "Annie",
                    Slot = SpellSlot.Q,
                    SpellName = "Disintegrate",
                    IsTargeted = true,
                });

            TargetedSpells.Add(
                new SpellData
                {
                    ChampionName = "Diana",
                    Slot = SpellSlot.R,
                    SpellName = "DianaTeleport",
                    IsTargeted = true,
                    IsDangerous = true
                });

            TargetedSpells.Add(
                new SpellData
                {
                    ChampionName = "Fizz",
                    Slot = SpellSlot.Q,
                    SpellName = "FizzPiercingStrike",
                    IsTargeted = true
                });

            TargetedSpells.Add(
                new SpellData
                {
                    ChampionName = "KhaZix",
                    Slot = SpellSlot.Q,
                    SpellName = "KhazixQ",
                    IsTargeted = true,
                });

            TargetedSpells.Add(
               new SpellData
               {
                   ChampionName = "KhaZix",
                   Slot = SpellSlot.Q, //evolved
                   SpellName = "khazixqlong",
                   IsTargeted = true
               });

            TargetedSpells.Add(
                new SpellData
                {
                    ChampionName = "LeeSin",
                    Slot = SpellSlot.R,
                    SpellName = "BlindMonkRKick",
                    IsTargeted = true,
                    IsDangerous = true
                });

            TargetedSpells.Add(
                new SpellData
                {
                    ChampionName = "Malphite",
                    Slot = SpellSlot.Q,
                    SpellName = "SismicShard",
                    IsTargeted = true
                });

            TargetedSpells.Add(
                new SpellData
                {
                    ChampionName = "Maokai",
                    Slot = SpellSlot.W,
                    SpellName = "MaokaiUnstableGrowth",
                    IsTargeted = true
                });

            TargetedSpells.Add(
                new SpellData
                {
                    ChampionName = "Fiddlesticks",
                    Slot = SpellSlot.Q,
                    SpellName = "Terrify",
                    IsTargeted = true,
                    IsDangerous = true
                });

            TargetedSpells.Add(
                new SpellData
                {
                    ChampionName = "Renekton",
                    Slot = SpellSlot.W,
                    SpellName = "RenektonPreExecute",
                    IsTargeted = true
                });

            TargetedSpells.Add(
                new SpellData
                {
                    ChampionName = "Riven",
                    Slot = SpellSlot.W,
                    SpellName = "RivenMartyr",
                    Radius = 270,
                    IsDangerous = true,
                });

            TargetedSpells.Add(
                new SpellData
                {
                    ChampionName = "Ryze",
                    Slot = SpellSlot.W,
                    SpellName = "RunePrison",
                    IsTargeted = true,
                    IsDangerous = true
                });

            TargetedSpells.Add(
                new SpellData
                {
                    ChampionName = "Nocturne",
                    Slot = SpellSlot.E,
                    SpellName = "NocturneUnspeakableHorror",
                    IsTargeted = true,
                    IsDangerous = true
                });

            TargetedSpells.Add(
                new SpellData
                {
                    ChampionName = "Pantheon",
                    Slot = SpellSlot.W,
                    SpellName = "PantheonW",
                    IsTargeted = true,
                    IsDangerous = true
                });

            TargetedSpells.Add(
                new SpellData
                {
                    ChampionName = "Pantheon",
                    Slot = SpellSlot.Q,
                    SpellName = "PantheonQ",
                    IsTargeted = true
                });

            TargetedSpells.Add(
                new SpellData
                {
                    ChampionName = "Trundle",
                    Slot = SpellSlot.R,
                    SpellName = "TrundlePain",
                    IsTargeted = true,
                    IsDangerous = true
                });

            TargetedSpells.Add(
                new SpellData
                {
                    ChampionName = "Yasuo",
                    Slot = SpellSlot.E,
                    SpellName = "YasuoDashWrapper",
                    IsTargeted = true,
                    IsDangerous = true
                });

            TargetedSpells.Add(
                new SpellData
                {
                    ChampionName = "Taric",
                    Slot = SpellSlot.E,
                    SpellName = "Dazzle",
                    IsTargeted = true,
                    IsDangerous = true,
                });

            TargetedSpells.Add(
                new SpellData
                {
                    ChampionName = "Garen",
                    Slot = SpellSlot.Q,
                    SpellName = "GarenQAttack",
                    IsTargeted = true,
                    IsDangerous = true,
                });

            TargetedSpells.Add(
                new SpellData
                {
                    ChampionName = "Garen",
                    Slot = SpellSlot.R,
                    SpellName = "GarenRPreCast",
                    IsTargeted = true,
                    IsDangerous = true
                });

            TargetedSpells.Add(
                new SpellData
                {
                    ChampionName = "Ekko",
                    Slot = SpellSlot.E,
                    SpellName = "EkkoEAttack",
                    IsTargeted = true
                });

            TargetedSpells.Add(
                new SpellData
                {
                    ChampionName = "Illaoi",
                    Slot = SpellSlot.W,
                    SpellName = "IllaoiWAttack",
                    IsTargeted = true
                });

            TargetedSpells.Add(
                new SpellData
                {
                    ChampionName = "Leona",
                    Slot = SpellSlot.Q,
                    SpellName = "LeonaShieldOfDaybreakAttack",
                    IsTargeted = true,
                    IsDangerous = true
                });

            TargetedSpells.Add(
                new SpellData
                {
                    ChampionName = "Darius",
                    Slot = SpellSlot.R,
                    SpellName = "DariusExecute",
                    IsTargeted = true,
                    IsDangerous = true
                });

            TargetedSpells.Add(
                new SpellData
                {
                    ChampionName = "Gangplank",
                    Slot = SpellSlot.Q,
                    SpellName = "GangplankQWrapper",
                    IsTargeted = true,
                });

            TargetedSpells.Add(
                new SpellData
                {
                    ChampionName = "Syndra",
                    Slot = SpellSlot.R,
                    SpellName = "SyndraR",
                    IsTargeted = true
                });

            TargetedSpells.Add(
                new SpellData
                {
                    ChampionName = "Veigar",
                    Slot = SpellSlot.R,
                    SpellName = "VeigarPrimordialBurst",
                    IsTargeted = true
                });

            TargetedSpells.Add(
                new SpellData
                {
                    ChampionName = "TwistedFate",
                    Slot = SpellSlot.W,
                    SpellName = "goldcardpreattack",
                    IsTargeted = true,
                    IsDangerous = true
                });

            TargetedSpells.Add(
                new SpellData
                {
                    ChampionName = "XinZhao",
                    Slot = SpellSlot.Q,
                    SpellName = "XenZhaoThrust3",
                    IsTargeted = true,
                    IsDangerous = true
                });
            #endregion
        }
    }

    public class DetectedSpellData
    {
        public SpellData Spell;
        public Vector2 StartPosition;
        public Vector2 EndPosition;
        public Obj_AI_Base Sender;
        public GameObjectProcessSpellCastEventArgs Args;

        public void Set(SpellData s, Vector2 sp, Vector2 ep, Obj_AI_Base snd, GameObjectProcessSpellCastEventArgs ar)
        {
            Spell = s;
            StartPosition = sp;
            EndPosition = ep;
            Sender = snd;
            Args = ar;
        }
    }
}