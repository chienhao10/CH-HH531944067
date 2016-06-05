namespace Valvrave_Sharp.Evade
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp.SDK;
    using EloBuddy;
    #endregion

    public static class SpellDatabase
    {
        #region Static Fields

        public static List<SpellData> Spells = new List<SpellData>();

        #endregion

        #region Constructors and Destructors

        static SpellDatabase()
        {
            #region Aatrox

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Aatrox",
                    SpellName = "AatroxQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 600,
                    Range = 650,
                    Radius = 250,
                    MissileSpeed = 2000,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Aatrox",
                    SpellName = "AatroxE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1075,
                    Radius = 35,
                    MissileSpeed = 1250,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    MissileSpellName = "AatroxEConeMissile",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            #endregion Aatrox

            #region Ahri

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ahri",
                    SpellName = "AhriOrbofDeception",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1000,
                    Radius = 100,
                    MissileSpeed = 2500,
                    MissileAccel = -3200,
                    MissileMaxSpeed = 2500,
                    MissileMinSpeed = 400,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "AhriOrbMissile",
                    CanBeRemoved = true,
                    ForceRemove = true,
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ahri",
                    SpellName = "AhriOrbReturn",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1000,
                    Radius = 100,
                    MissileSpeed = 60,
                    MissileAccel = 1900,
                    MissileMinSpeed = 60,
                    MissileMaxSpeed = 2600,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileFollowsUnit = true,
                    CanBeRemoved = true,
                    ForceRemove = true,
                    MissileSpellName = "AhriOrbReturn",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ahri",
                    SpellName = "AhriSeduce",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1000,
                    Radius = 60,
                    MissileSpeed = 1550,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "AhriSeduceMissile",
                    CanBeRemoved = true,
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            #endregion Ahri

            #region Alistar

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Alistar",
                    SpellName = "Pulverize",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Radius = 365,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    DangerValue = 3,
                    IsDangerous = true
                });

            #endregion Alistar

            #region Amumu

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Amumu",
                    SpellName = "BandageToss",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1100,
                    Radius = 90,
                    MissileSpeed = 2000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "SadMummyBandageToss",
                    CanBeRemoved = true,
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Amumu",
                    SpellName = "CurseoftheSadMummy",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Radius = 550,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    DangerValue = 5,
                    IsDangerous = true
                });

            #endregion Amumu

            #region Anivia

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Anivia",
                    SpellName = "FlashFrost",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1100,
                    Radius = 110,
                    MissileSpeed = 850,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "FlashFrostSpell",
                    CanBeRemoved = true,
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            #endregion Anivia

            #region Annie

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Annie",
                    SpellName = "Incinerate",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCone,
                    Delay = 250,
                    Range = 825,
                    Radius = 80,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    DangerValue = 2
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Annie",
                    SpellName = "InfernalGuardian",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 600,
                    Radius = 251,
                    MissileSpeed = int.MaxValue,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true
                });

            #endregion Annie

            #region Ashe

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ashe",
                    SpellName = "Volley",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1250,
                    Radius = 60,
                    MissileSpeed = 1500,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "VolleyAttack",
                    MultipleNumber = 9,
                    MultipleAngle = 4.62f * (float)Math.PI / 180,
                    CanBeRemoved = true,
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ashe",
                    SpellName = "EnchantedCrystalArrow",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 20000,
                    Radius = 130,
                    MissileSpeed = 1600,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "EnchantedCrystalArrow",
                    CanBeRemoved = true,
                    CollisionObjects = CollisionableObjects.Heroes | CollisionableObjects.YasuoWall
                });

            #endregion Ashe

            #region Aurelion Sol

            Spells.Add(
                new SpellData
                {
                    ChampionName = "AurelionSol",
                    SpellName = "AurelionSolQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1500,
                    Radius = 180,
                    MissileSpeed = 850,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "AurelionSolQMissile",
                    CanBeRemoved = true,
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "AurelionSol",
                    SpellName = "AurelionSolR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 300,
                    Range = 1420,
                    Radius = 120,
                    MissileSpeed = 4500,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "AurelionSolRBeamMissile",
                    CanBeRemoved = true
                });

            #endregion Aurelion Sol

            #region Bard

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Bard",
                    SpellName = "BardQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 950,
                    Radius = 60,
                    MissileSpeed = 1600,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "BardQMissile",
                    CanBeRemoved = true,
                    CollisionObjects = CollisionableObjects.Heroes | CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Bard",
                    SpellName = "BardR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 500,
                    Range = 3400,
                    Radius = 350,
                    MissileSpeed = 2100,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "BardR"
                });

            #endregion

            #region Blatzcrink

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Blitzcrank",
                    SpellName = "RocketGrab",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1050,
                    Radius = 70,
                    MissileSpeed = 1800,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 4,
                    IsDangerous = true,
                    MissileSpellName = "RocketGrabMissile",
                    CanBeRemoved = true,
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Blitzcrank",
                    SpellName = "StaticField",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Radius = 600,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    DangerValue = 2
                });

            #endregion Blatzcrink

            #region Brand

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Brand",
                    SpellName = "BrandBlaze",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1100,
                    Radius = 60,
                    MissileSpeed = 1600,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "BrandBlazeMissile",
                    CanBeRemoved = true,
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Brand",
                    SpellName = "BrandFissure",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 850,
                    Range = 900,
                    Radius = 240,
                    MissileSpeed = int.MaxValue,
                    AddHitbox = true,
                    DangerValue = 2
                });

            #endregion Brand

            #region Braum

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Braum",
                    SpellName = "BraumQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1050,
                    Radius = 60,
                    MissileSpeed = 1700,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "BraumQMissile",
                    CanBeRemoved = true,
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Braum",
                    SpellName = "BraumRWrapper",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 500,
                    Range = 1200,
                    Radius = 115,
                    MissileSpeed = 1400,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 4,
                    IsDangerous = true,
                    MissileSpellName = "braumrmissile",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            #endregion Braum

            #region Caitlyn

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Caitlyn",
                    SpellName = "CaitlynPiltoverPeacemaker",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 625,
                    Range = 1300,
                    Radius = 90,
                    MissileSpeed = 2200,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "CaitlynPiltoverPeacemaker",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Caitlyn",
                    SpellName = "CaitlynEntrapment",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 125,
                    Range = 1000,
                    Radius = 70,
                    MissileSpeed = 1600,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 1,
                    MissileSpellName = "CaitlynEntrapmentMissile",
                    CanBeRemoved = true,
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            #endregion Caitlyn

            #region Cassiopeia

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Cassiopeia",
                    SpellName = "CassiopeiaNoxiousBlast",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 750,
                    Range = 850,
                    Radius = 150,
                    MissileSpeed = int.MaxValue,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "CassiopeiaNoxiousBlast"
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Cassiopeia",
                    SpellName = "CassiopeiaMiasma",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 850,
                    Radius = 220,
                    MissileSpeed = 2500,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "CassiopeiaMiasma",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Cassiopeia",
                    SpellName = "CassiopeiaPetrifyingGaze",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCone,
                    Delay = 600,
                    Range = 825,
                    Radius = 80,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "CassiopeiaPetrifyingGaze"
                });

            #endregion Cassiopeia

            #region Chogath

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Chogath",
                    SpellName = "Rupture",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 1200,
                    Range = 950,
                    Radius = 250,
                    MissileSpeed = int.MaxValue,
                    AddHitbox = true,
                    DangerValue = 3,
                    MissileSpellName = "Rupture"
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Chogath",
                    SpellName = "FeralScream",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCone,
                    Delay = 250,
                    Range = 650,
                    Radius = 60,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    DangerValue = 2,
                    IsDangerous = true,
                    MissileSpellName = "FeralScream"
                });

            #endregion Chogath

            #region Corki

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Corki",
                    SpellName = "PhosphorusBomb",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 300,
                    Range = 825,
                    Radius = 250,
                    MissileSpeed = 1000,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "PhosphorusBombMissile",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Corki",
                    SpellName = "MissileBarrage",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 200,
                    Range = 1300,
                    Radius = 40,
                    MissileSpeed = 2000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "MissileBarrageMissile",
                    CanBeRemoved = true,
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Corki",
                    SpellName = "MissileBarrage2",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 200,
                    Range = 1500,
                    Radius = 40,
                    MissileSpeed = 2000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "MissileBarrageMissile2",
                    CanBeRemoved = true,
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            #endregion Corki

            #region Darius

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Darius",
                    SpellName = "DariusCleave",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 750,
                    Radius = 375,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    MissileSpellName = "DariusCleave",
                    FollowCaster = true,
                    DisabledByDefault = true
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Darius",
                    SpellName = "DariusAxeGrabCone",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCone,
                    Delay = 250,
                    Range = 550,
                    Radius = 80,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "DariusAxeGrabCone"
                });

            #endregion Darius

            #region Diana

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Diana",
                    SpellName = "DianaArc",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 895,
                    Radius = 195,
                    MissileSpeed = 1400,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "DianaArcArc",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Diana",
                    SpellName = "DianaArcArc",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotArc,
                    Delay = 250,
                    Range = 895,
                    Radius = 195,
                    DontCross = true,
                    MissileSpeed = 1400,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "DianaArcArc",
                    TakeClosestPath = true,
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            #endregion Diana

            #region DrMundo

            Spells.Add(
                new SpellData
                {
                    ChampionName = "DrMundo",
                    SpellName = "InfectedCleaverMissileCast",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 275,
                    Range = 1050,
                    Radius = 60,
                    MissileSpeed = 2100,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    MissileSpellName = "InfectedCleaverMissile",
                    CanBeRemoved = true,
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            #endregion DrMundo

            #region Draven

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Draven",
                    SpellName = "DravenDoubleShot",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1100,
                    Radius = 130,
                    MissileSpeed = 1400,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "DravenDoubleShotMissile",
                    CanBeRemoved = true,
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Draven",
                    SpellName = "DravenRCast",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 400,
                    Range = 20000,
                    Radius = 160,
                    MissileSpeed = 2000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "DravenR",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            #endregion Draven

            #region Ekko

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ekko",
                    SpellName = "EkkoQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 950,
                    Radius = 60,
                    MissileSpeed = 1650,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 4,
                    IsDangerous = true,
                    MissileSpellName = "ekkoqmis",
                    CanBeRemoved = true,
                    CollisionObjects = CollisionableObjects.Heroes | CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ekko",
                    SpellName = "EkkoW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 3750,
                    Range = 1600,
                    Radius = 375,
                    MissileSpeed = 1650,
                    DisabledByDefault = true,
                    DangerValue = 3,
                    MissileSpellName = "EkkoW",
                    CanBeRemoved = true
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ekko",
                    SpellName = "EkkoR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 1600,
                    Radius = 375,
                    MissileSpeed = 1650,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    MissileSpellName = "EkkoR",
                    CanBeRemoved = true,
                    FromObject = "TestCubeRender"
                });

            #endregion Ekko

            #region Elise

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Elise",
                    SpellName = "EliseHumanE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1100,
                    Radius = 55,
                    MissileSpeed = 1600,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 4,
                    IsDangerous = true,
                    MissileSpellName = "EliseHumanE",
                    CanBeRemoved = true,
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            #endregion Elise

            #region Evelynn

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Evelynn",
                    SpellName = "EvelynnR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 650,
                    Radius = 350,
                    MissileSpeed = int.MaxValue,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "EvelynnR"
                });

            #endregion Evelynn

            #region Ezreal

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ezreal",
                    SpellName = "EzrealMysticShot",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1200,
                    Radius = 60,
                    MissileSpeed = 2000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "EzrealMysticShotMissile",
                    ExtraMissileNames = new[] { "EzrealMysticShotPulseMissile" },
                    CanBeRemoved = true,
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ezreal",
                    SpellName = "EzrealEssenceFlux",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1050,
                    Radius = 80,
                    MissileSpeed = 1600,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "EzrealEssenceFluxMissile",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ezreal",
                    SpellName = "EzrealTrueshotBarrage",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 1000,
                    Range = 20000,
                    Radius = 160,
                    MissileSpeed = 2000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "EzrealTrueshotBarrage",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            #endregion Ezreal

            #region Fiora

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Fiora",
                    SpellName = "FioraW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 500,
                    Range = 800,
                    Radius = 70,
                    MissileSpeed = 3200,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "FioraWMissile",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            #endregion Fiora

            #region Fizz

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Fizz",
                    SpellName = "FizzMarinerDoom",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1300,
                    Radius = 120,
                    MissileSpeed = 1350,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "FizzMarinerDoomMissile",
                    CollisionObjects = CollisionableObjects.Heroes | CollisionableObjects.YasuoWall,
                    CanBeRemoved = true
                });

            #endregion Fizz

            #region Galio

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Galio",
                    SpellName = "GalioResoluteSmite",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 900,
                    Radius = 200,
                    MissileSpeed = 1300,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "GalioResoluteSmite",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Galio",
                    SpellName = "GalioRighteousGust",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1200,
                    Radius = 120,
                    MissileSpeed = 1200,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "GalioRighteousGust",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Galio",
                    SpellName = "GalioIdolOfDurand",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Radius = 550,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    DangerValue = 5,
                    IsDangerous = true
                });

            #endregion Galio

            #region Gnar

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Gnar",
                    SpellName = "GnarQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1125,
                    Radius = 60,
                    MissileSpeed = 2500,
                    MissileAccel = -3000,
                    MissileMaxSpeed = 2500,
                    MissileMinSpeed = 1400,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    CanBeRemoved = true,
                    ForceRemove = true,
                    MissileSpellName = "gnarqmissile",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Gnar",
                    SpellName = "GnarQReturn",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Range = 2500,
                    Radius = 75,
                    MissileSpeed = 60,
                    MissileAccel = 800,
                    MissileMaxSpeed = 2600,
                    MissileMinSpeed = 60,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    CanBeRemoved = true,
                    ForceRemove = true,
                    MissileSpellName = "GnarQMissileReturn",
                    DisabledByDefault = true,
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Gnar",
                    SpellName = "GnarBigQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 500,
                    Range = 1150,
                    Radius = 90,
                    MissileSpeed = 2100,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "GnarBigQMissile",
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Gnar",
                    SpellName = "GnarBigW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 600,
                    Range = 600,
                    Radius = 80,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "GnarBigW"
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Gnar",
                    SpellName = "GnarE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCircle,
                    Range = 473,
                    Radius = 150,
                    MissileSpeed = 903,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "GnarE"
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Gnar",
                    SpellName = "GnarBigE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 475,
                    Radius = 200,
                    MissileSpeed = 1000,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "GnarBigE"
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Gnar",
                    SpellName = "GnarR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Radius = 500,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    DangerValue = 5,
                    IsDangerous = true
                });

            #endregion

            #region Gragas

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Gragas",
                    SpellName = "GragasQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 1100,
                    Radius = 275,
                    MissileSpeed = 1300,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "GragasQMissile",
                    ExtraDuration = 4500,
                    ToggleParticleName = "Gragas_.+_Q_(Enemy|Ally)",
                    DontCross = true,
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Gragas",
                    SpellName = "GragasE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Range = 950,
                    Radius = 200,
                    MissileSpeed = 1200,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "GragasE",
                    CanBeRemoved = true,
                    ExtraRange = 300,
                    CollisionObjects = CollisionableObjects.Heroes | CollisionableObjects.Minions
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Gragas",
                    SpellName = "GragasR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 1050,
                    Radius = 375,
                    MissileSpeed = 1800,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "GragasRBoom",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            #endregion Gragas

            #region Graves

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Graves",
                    SpellName = "GravesQLineSpell",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 808,
                    Radius = 40,
                    MissileSpeed = 3000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "GravesQLineMis",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Graves",
                    SpellName = "GravesChargeShot",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1100,
                    Radius = 100,
                    MissileSpeed = 2100,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "GravesChargeShotShot",
                    CollisionObjects = CollisionableObjects.Heroes | CollisionableObjects.YasuoWall
                });

            #endregion Graves

            #region Heimerdinger

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Heimerdinger",
                    SpellName = "Heimerdingerwm",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1500,
                    Radius = 70,
                    MissileSpeed = 1800,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "HeimerdingerWAttack2",
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Heimerdinger",
                    SpellName = "HeimerdingerE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 925,
                    Radius = 100,
                    MissileSpeed = 1200,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "heimerdingerespell",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            #endregion Heimerdinger

            #region Illaoi

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Illaoi",
                    SpellName = "IllaoiQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 750,
                    Range = 850,
                    Radius = 100,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "illaoiemis",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Illaoi",
                    SpellName = "IllaoiE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 950,
                    Radius = 50,
                    MissileSpeed = 1900,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "illaoiemis",
                    CollisionObjects = CollisionableObjects.Heroes | CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Illaoi",
                    SpellName = "IllaoiR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 500,
                    Range = 0,
                    Radius = 450,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    DangerValue = 3,
                    IsDangerous = true
                });

            #endregion Illaoi

            #region Irelia

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Irelia",
                    SpellName = "IreliaTranscendentBlades",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Range = 1200,
                    Radius = 65,
                    MissileSpeed = 1600,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "ireliatranscendentbladesspell",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            #endregion Irelia

            #region Janna

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Janna",
                    SpellName = "JannaQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1700,
                    Radius = 120,
                    MissileSpeed = 900,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "HowlingGaleSpell",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            #endregion Janna

            #region JarvanIV

            Spells.Add(
                new SpellData
                {
                    ChampionName = "JarvanIV",
                    SpellName = "JarvanIVDragonStrike",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 600,
                    Range = 770,
                    Radius = 70,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "JarvanIV",
                    SpellName = "JarvanIVEQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 880,
                    Radius = 70,
                    MissileSpeed = 1450,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "JarvanIV",
                    SpellName = "JarvanIVDemacianStandard",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 500,
                    Range = 860,
                    Radius = 175,
                    MissileSpeed = int.MaxValue,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "JarvanIVDemacianStandard"
                });

            #endregion JarvanIV

            #region Jayce

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Jayce",
                    SpellName = "jayceshockblast",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1300,
                    Radius = 70,
                    MissileSpeed = 1450,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "JayceShockBlastMis",
                    CanBeRemoved = true,
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Jayce",
                    SpellName = "JayceQAccel",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1300,
                    Radius = 70,
                    MissileSpeed = 2350,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "JayceShockBlastWallMis",
                    CanBeRemoved = true,
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            #endregion Jayce

            #region Jhin

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Jhin",
                    SpellName = "JhinW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 750,
                    Range = 2550,
                    Radius = 40,
                    MissileSpeed = 5000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "JhinWMissile",
                    CanBeRemoved = true,
                    CollisionObjects = CollisionableObjects.Heroes
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Jhin",
                    SpellName = "JhinRShot",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 3500,
                    Radius = 80,
                    MissileSpeed = 5000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "JhinRShotMis",
                    CanBeRemoved = true,
                    CollisionObjects = CollisionableObjects.Heroes | CollisionableObjects.YasuoWall
                });

            #endregion Jhin

            #region Jinx

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Jinx",
                    SpellName = "JinxW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 600,
                    Range = 1500,
                    Radius = 60,
                    MissileSpeed = 3300,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "JinxWMissile",
                    CanBeRemoved = true,
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Jinx",
                    SpellName = "JinxR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 600,
                    Range = 20000,
                    Radius = 140,
                    MissileSpeed = 1700,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "JinxR",
                    CanBeRemoved = true,
                    CollisionObjects = CollisionableObjects.Heroes | CollisionableObjects.YasuoWall
                });

            #endregion Jinx

            #region Kalista

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Kalista",
                    SpellName = "KalistaMysticShot",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1200,
                    Radius = 40,
                    MissileSpeed = 1700,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "kalistamysticshotmis",
                    ExtraMissileNames = new[] { "kalistamysticshotmistrue" },
                    CanBeRemoved = true,
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            #endregion Kalista

            #region Karma

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Karma",
                    SpellName = "KarmaQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1050,
                    Radius = 60,
                    MissileSpeed = 1700,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "KarmaQMissile",
                    CanBeRemoved = true,
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Karma",
                    SpellName = "KarmaQMantra",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 950,
                    Radius = 80,
                    MissileSpeed = 1700,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "KarmaQMissileMantra",
                    CanBeRemoved = true,
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            #endregion Karma

            #region Karthus

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Karthus",
                    SpellName = "KarthusLayWasteA2",
                    ExtraSpellNames =
                            new[]
                                {
                                    "karthuslaywastea3", "karthuslaywastea1", "karthuslaywastedeada1",
                                    "karthuslaywastedeada2", "karthuslaywastedeada3"
                                },
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 625,
                    Range = 875,
                    Radius = 160,
                    MissileSpeed = int.MaxValue,
                    AddHitbox = true,
                    DangerValue = 2
                });

            #endregion Karthus

            #region Kassadin

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Kassadin",
                    SpellName = "RiftWalk",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 450,
                    Radius = 270,
                    MissileSpeed = int.MaxValue,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "RiftWalk"
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Kassadin",
                    SpellName = "ForcePulse",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCone,
                    Delay = 250,
                    Range = 700,
                    Radius = 80,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    DangerValue = 2,
                    MissileSpellName = "ForcePulse"
                });

            #endregion Kassadin

            #region Kennen

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Kennen",
                    SpellName = "KennenShurikenHurlMissile1",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 200,
                    Range = 1050,
                    Radius = 50,
                    MissileSpeed = 1700,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "KennenShurikenHurlMissile1",
                    CanBeRemoved = true,
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            #endregion Kennen

            #region Khazix

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Khazix",
                    SpellName = "KhazixW",
                    ExtraSpellNames = new[] { "khazixwlong" },
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1025,
                    Radius = 73,
                    MissileSpeed = 1700,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "KhazixWMissile",
                    CanBeRemoved = true,
                    MultipleNumber = 3,
                    MultipleAngle = 22f * (float)Math.PI / 180,
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Khazix",
                    SpellName = "KhazixE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 600,
                    Radius = 300,
                    MissileSpeed = 1500,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "KhazixE"
                });

            #endregion Khazix

            #region KogMaw

            Spells.Add(
                new SpellData
                {
                    ChampionName = "KogMaw",
                    SpellName = "KogMawQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1200,
                    Radius = 70,
                    MissileSpeed = 1650,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "KogMawQ",
                    CanBeRemoved = true,
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "KogMaw",
                    SpellName = "KogMawVoidOoze",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1360,
                    Radius = 120,
                    MissileSpeed = 1400,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "KogMawVoidOozeMissile",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "KogMaw",
                    SpellName = "KogMawLivingArtillery",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 1200,
                    Range = 1800,
                    Radius = 225,
                    MissileSpeed = int.MaxValue,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "KogMawLivingArtillery"
                });

            #endregion KogMaw

            #region Leblanc

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Leblanc",
                    SpellName = "LeblancSlide",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Range = 600,
                    Radius = 220,
                    MissileSpeed = 1450,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "LeblancSlide"
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Leblanc",
                    SpellName = "LeblancSlideM",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Range = 600,
                    Radius = 220,
                    MissileSpeed = 1450,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "LeblancSlideM"
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Leblanc",
                    SpellName = "LeblancSoulShackle",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 950,
                    Radius = 70,
                    MissileSpeed = 1750,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "LeblancSoulShackle",
                    CanBeRemoved = true,
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Leblanc",
                    SpellName = "LeblancSoulShackleM",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 950,
                    Radius = 70,
                    MissileSpeed = 1750,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "LeblancSoulShackleM",
                    CanBeRemoved = true,
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            #endregion Leblanc

            #region LeeSin

            Spells.Add(
                new SpellData
                {
                    ChampionName = "LeeSin",
                    SpellName = "BlindMonkQOne",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 275,
                    Range = 1100,
                    Radius = 60,
                    MissileSpeed = 1850,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "BlindMonkQOne",
                    CanBeRemoved = true,
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            #endregion LeeSin

            #region Leona

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Leona",
                    SpellName = "LeonaZenithBlade",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 905,
                    Radius = 70,
                    MissileSpeed = 2000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    TakeClosestPath = true,
                    MissileSpellName = "LeonaZenithBladeMissile",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Leona",
                    SpellName = "LeonaSolarFlare",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 1000,
                    Range = 1200,
                    Radius = 300,
                    MissileSpeed = int.MaxValue,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "LeonaSolarFlare"
                });

            #endregion Leona

            #region Lissandra

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Lissandra",
                    SpellName = "LissandraQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 700,
                    Radius = 75,
                    MissileSpeed = 2200,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "LissandraQMissile",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Lissandra",
                    SpellName = "LissandraQShards",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 700,
                    Radius = 90,
                    MissileSpeed = 2200,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "lissandraqshards",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Lissandra",
                    SpellName = "LissandraW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Radius = 450,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Lissandra",
                    SpellName = "LissandraE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1025,
                    Radius = 125,
                    MissileSpeed = 850,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "LissandraEMissile",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            #endregion Lulu

            #region Lucian

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Lucian",
                    SpellName = "LucianQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 350,
                    Range = 1150,
                    Radius = 60,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "LucianQ"
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Lucian",
                    SpellName = "LucianW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 350,
                    Range = 1000,
                    Radius = 55,
                    MissileSpeed = 1650,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "lucianwmissile",
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Lucian",
                    SpellName = "LucianRMis",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 100,
                    Range = 1200,
                    Radius = 110,
                    MissileSpeed = 2700,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "lucianrmissileoffhand",
                    ExtraMissileNames = new[] { "lucianrmissile" },
                    DontCheckForDuplicates = true,
                    DisabledByDefault = true,
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            #endregion Lucian

            #region Lulu

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Lulu",
                    SpellName = "LuluQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 950,
                    Radius = 60,
                    MissileSpeed = 1450,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "LuluQMissile",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Lulu",
                    SpellName = "LuluQPix",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 950,
                    Radius = 60,
                    MissileSpeed = 1450,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "LuluQMissileTwo",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            #endregion Lulu

            #region Lux

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Lux",
                    SpellName = "LuxLightBinding",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1300,
                    Radius = 70,
                    MissileSpeed = 1200,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "LuxLightBindingMis",
                    //CanBeRemoved = true,
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Lux",
                    SpellName = "LuxLightStrikeKugel",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 1100,
                    Radius = 275,
                    MissileSpeed = 1300,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "LuxLightStrikeKugel",
                    ExtraDuration = 5500,
                    ToggleParticleName = "Lux_.+_E_tar_aoe_",
                    DontCross = true,
                    CanBeRemoved = true,
                    DisabledByDefault = true,
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Lux",
                    SpellName = "LuxMaliceCannon",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 1000,
                    Range = 3500,
                    Radius = 190,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "LuxMaliceCannon"
                });

            #endregion Lux

            #region Malphite

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Malphite",
                    SpellName = "UFSlash",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Range = 1000,
                    Radius = 270,
                    MissileSpeed = 1500,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "UFSlash"
                });

            #endregion Malphite

            #region Malzahar

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Malzahar",
                    SpellName = "AlZaharCalloftheVoid",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 1000,
                    Range = 900,
                    Radius = 85,
                    MissileSpeed = int.MaxValue,
                    AddHitbox = true,
                    DangerValue = 2,
                    DontCross = true,
                    MissileSpellName = "AlZaharCalloftheVoid"
                });

            #endregion Malzahar

            #region Morgana

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Morgana",
                    SpellName = "DarkBindingMissile",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1300,
                    Radius = 80,
                    MissileSpeed = 1200,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "DarkBindingMissile",
                    CanBeRemoved = true,
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            #endregion Morgana

            #region Nami

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Nami",
                    SpellName = "NamiQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 950,
                    Range = 1625,
                    Radius = 150,
                    MissileSpeed = int.MaxValue,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "namiqmissile",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Nami",
                    SpellName = "NamiR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 500,
                    Range = 2750,
                    Radius = 260,
                    MissileSpeed = 850,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "NamiRMissile",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            #endregion Nami

            #region Nautilus

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Nautilus",
                    SpellName = "NautilusAnchorDrag",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1250,
                    Radius = 90,
                    MissileSpeed = 2000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "NautilusAnchorDragMissile",
                    CanBeRemoved = true,
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            #endregion Nautilus

            #region Nocturne

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Nocturne",
                    SpellName = "NocturneDuskbringer",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1125,
                    Radius = 60,
                    MissileSpeed = 1400,
                    DangerValue = 2,
                    MissileSpellName = "NocturneDuskbringer",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            #endregion Nocturne

            #region Nidalee

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Nidalee",
                    SpellName = "JavelinToss",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1500,
                    Radius = 40,
                    MissileSpeed = 1300,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "JavelinToss",
                    CanBeRemoved = true,
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            #endregion Nidalee

            #region Olaf

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Olaf",
                    SpellName = "OlafAxeThrowCast",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1000,
                    ExtraRange = 150,
                    Radius = 105,
                    MissileSpeed = 1600,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "olafaxethrow",
                    CanBeRemoved = true,
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            #endregion Olaf

            #region Orianna

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Orianna",
                    SpellName = "OriannasQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Range = 1500,
                    Radius = 80,
                    MissileSpeed = 1200,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "orianaizuna",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Orianna",
                    SpellName = "OriannaQend",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Range = 1500,
                    Radius = 90,
                    MissileSpeed = 1200,
                    AddHitbox = true,
                    DangerValue = 2,
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Orianna",
                    SpellName = "OriannaDissonanceW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Radius = 255,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "OrianaDissonanceCommand-" /*, FromObject = "TheDoomBall"*/,
                    SourceObjectName = "w_dissonance_ball"
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Orianna",
                    SpellName = "OriannasE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Range = 1500,
                    Radius = 85,
                    MissileSpeed = 1850,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "orianaredact",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Orianna",
                    SpellName = "OriannaDetonateR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 700,
                    Radius = 410,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "OrianaDetonateCommand-" /*, FromObject = "TheDoomBall"*/,
                    SourceObjectName = "r_vacuumindicator"
                });

            #endregion Orianna

            #region Poppy

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Poppy",
                    SpellName = "PoppyQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 500,
                    Range = 430,
                    Radius = 100,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "PoppyQ"
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Poppy",
                    SpellName = "PoppyRSpell",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 300,
                    Range = 1200,
                    Radius = 100,
                    MissileSpeed = 1600,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "PoppyRMissile",
                    CanBeRemoved = true,
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            #endregion Poppy

            #region Quinn

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Quinn",
                    SpellName = "QuinnQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 313,
                    Range = 1050,
                    Radius = 60,
                    MissileSpeed = 1550,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "QuinnQ",
                    CanBeRemoved = true,
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            #endregion Quinn

            #region RekSai

            Spells.Add(
                new SpellData
                {
                    ChampionName = "RekSai",
                    SpellName = "reksaiqburrowed",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 500,
                    Range = 1625,
                    Radius = 60,
                    MissileSpeed = 1950,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    MissileSpellName = "RekSaiQBurrowedMis",
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            #endregion RekSai

            #region Rengar

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Rengar",
                    SpellName = "RengarE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1000,
                    Radius = 70,
                    MissileSpeed = 1500,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "RengarEFinal",
                    CanBeRemoved = true,
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            #endregion Rengar

            #region Riven

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Riven",
                    SpellName = "RivenMartyr",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Radius = 280,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = true
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Riven",
                    SpellName = "rivenizunablade",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1100,
                    Radius = 125,
                    MissileSpeed = 1600,
                    FixedRange = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    MultipleNumber = 3,
                    MultipleAngle = 15 * (float)Math.PI / 180,
                    MissileSpellName = "RivenLightsaberMissile",
                    ExtraMissileNames = new[] { "RivenLightsaberMissileSide" },
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            #endregion Riven

            #region Rumble

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Rumble",
                    SpellName = "RumbleGrenade",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 950,
                    Radius = 60,
                    MissileSpeed = 2000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "RumbleGrenade",
                    CanBeRemoved = true,
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Rumble",
                    SpellName = "RumbleCarpetBombM",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 400,
                    MissileDelayed = true,
                    Range = 1200,
                    Radius = 200,
                    MissileSpeed = 1600,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 4,
                    MissileSpellName = "RumbleCarpetBombMissile"
                });

            #endregion Rumble

            #region Ryze

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ryze",
                    SpellName = "RyzeQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 900,
                    Radius = 50,
                    MissileSpeed = 1700,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "RyzeQ",
                    CanBeRemoved = true,
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            #endregion

            #region Sejuani

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Sejuani",
                    SpellName = "SejuaniArcticAssault",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Range = 900,
                    Radius = 70,
                    MissileSpeed = 1600,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    ExtraRange = 200,
                    CollisionObjects = CollisionableObjects.Heroes | CollisionableObjects.Minions
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Sejuani",
                    SpellName = "SejuaniGlacialPrisonStart",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1100,
                    Radius = 110,
                    MissileSpeed = 1600,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "sejuaniglacialprison",
                    CanBeRemoved = true,
                    CollisionObjects = CollisionableObjects.Heroes | CollisionableObjects.YasuoWall
                });

            #endregion Sejuani

            #region Shen

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Shen",
                    SpellName = "ShenE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Range = 650,
                    Radius = 50,
                    MissileSpeed = 1600,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "ShenE",
                    ExtraRange = 200
                });

            #endregion Shen

            #region Shyvana

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Shyvana",
                    SpellName = "ShyvanaFireball",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 950,
                    Radius = 60,
                    MissileSpeed = 1700,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "ShyvanaFireballMissile",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Shyvana",
                    SpellName = "ShyvanaTransformCast",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1000,
                    Radius = 150,
                    MissileSpeed = 1500,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "ShyvanaTransformCast",
                    ExtraRange = 200
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Shyvana",
                    SpellName = "shyvanafireballdragon2",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 850,
                    Radius = 70,
                    MissileSpeed = 2000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    MissileSpellName = "ShyvanaFireballDragonFxMissile",
                    ExtraRange = 200,
                    MultipleNumber = 5,
                    MultipleAngle = 10 * (float)Math.PI / 180
                });

            #endregion Shyvana

            #region Sion

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Sion",
                    SpellName = "SionE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 800,
                    Radius = 80,
                    MissileSpeed = 1800,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "SionEMissile",
                    CollisionObjects = CollisionableObjects.Heroes | CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Sion",
                    SpellName = "SionR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 500,
                    Range = 800,
                    Radius = 120,
                    MissileSpeed = 1000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    CollisionObjects = CollisionableObjects.Heroes
                });

            #endregion Sion

            #region Sivir

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Sivir",
                    SpellName = "SivirQReturn",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Range = 1250,
                    Radius = 100,
                    MissileSpeed = 1350,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "SivirQMissileReturn",
                    MissileFollowsUnit = true,
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Sivir",
                    SpellName = "SivirQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1250,
                    Radius = 90,
                    MissileSpeed = 1350,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "SivirQMissile",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            #endregion Sivir

            #region Skarner

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Skarner",
                    SpellName = "SkarnerFracture",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1000,
                    Radius = 70,
                    MissileSpeed = 1500,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "SkarnerFractureMissile",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            #endregion Skarner

            #region Sona

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Sona",
                    SpellName = "SonaR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1000,
                    Radius = 140,
                    MissileSpeed = 2400,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "SonaR",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            #endregion Sona

            #region Soraka

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Soraka",
                    SpellName = "SorakaQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 500,
                    Range = 800,
                    Radius = 235,
                    MissileSpeed = 1750,
                    AddHitbox = true,
                    DangerValue = 2,
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            #endregion Soraka

            #region Swain

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Swain",
                    SpellName = "SwainShadowGrasp",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 1100,
                    Range = 900,
                    Radius = 180,
                    MissileSpeed = int.MaxValue,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "SwainShadowGrasp"
                });

            #endregion Swain

            #region Syndra

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Syndra",
                    SpellName = "SyndraQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 600,
                    Range = 800,
                    Radius = 150,
                    MissileSpeed = int.MaxValue,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "SyndraQ"
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Syndra",
                    SpellName = "syndrawcast",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 950,
                    Radius = 210,
                    MissileSpeed = 1450,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "syndrawcast"
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Syndra",
                    SpellName = "syndrae5",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 0,
                    Range = 950,
                    Radius = 100,
                    MissileSpeed = 2000,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "syndrae5",
                    DisableFowDetection = true,
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Syndra",
                    SpellName = "SyndraE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 0,
                    Range = 950,
                    Radius = 100,
                    MissileSpeed = 2000,
                    AddHitbox = true,
                    DangerValue = 2,
                    DisableFowDetection = true,
                    MissileSpellName = "SyndraE",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            #endregion Syndra

            #region Tahm Kench

            Spells.Add(
                new SpellData
                {
                    ChampionName = "TahmKench",
                    SpellName = "TahmKenchQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 951,
                    Radius = 70,
                    MissileSpeed = 2800,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "tahmkenchqmissile",
                    CanBeRemoved = true,
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            #endregion Tahm Kench

            #region Talon

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Talon",
                    SpellName = "TalonRake",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 800,
                    Radius = 80,
                    MissileSpeed = 2300,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = true,
                    MultipleNumber = 3,
                    MultipleAngle = 20 * (float)Math.PI / 180,
                    MissileSpellName = "talonrakemissileone",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Talon",
                    SpellName = "TalonRakeReturn",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 800,
                    Radius = 80,
                    MissileSpeed = 1850,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = true,
                    MultipleNumber = 3,
                    MultipleAngle = 20 * (float)Math.PI / 180,
                    MissileSpellName = "talonrakemissiletwo",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            #endregion Talon

            #region Taric

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Taric",
                    SpellName = "TaricE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 1000,
                    Range = 750,
                    Radius = 100,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "TaricE",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            #endregion Taric

            #region Thresh

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Thresh",
                    SpellName = "ThreshQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 500,
                    Range = 1100,
                    Radius = 70,
                    MissileSpeed = 1900,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "ThreshQMissile",
                    CanBeRemoved = true,
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Thresh",
                    SpellName = "ThreshEFlay",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 125,
                    Range = 1075,
                    Radius = 110,
                    MissileSpeed = 2000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    Centered = true,
                    MissileSpellName = "ThreshEMissile1",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            #endregion Thresh

            #region Tristana

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Tristana",
                    SpellName = "RocketJump",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 500,
                    Range = 900,
                    Radius = 270,
                    MissileSpeed = 1500,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "RocketJump"
                });

            #endregion Tristana

            #region Tryndamere

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Tryndamere",
                    SpellName = "slashCast",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Range = 660,
                    Radius = 93,
                    MissileSpeed = 1300,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "slashCast"
                });

            #endregion Tryndamere

            #region TwistedFate

            Spells.Add(
                new SpellData
                {
                    ChampionName = "TwistedFate",
                    SpellName = "WildCards",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1450,
                    Radius = 40,
                    MissileSpeed = 1000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "SealFateMissile",
                    MultipleNumber = 3,
                    MultipleAngle = 28 * (float)Math.PI / 180,
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            #endregion TwistedFate

            #region Twitch

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Twitch",
                    SpellName = "TwitchVenomCask",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 900,
                    Radius = 275,
                    MissileSpeed = 1400,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "TwitchVenomCaskMissile",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            #endregion Twitch

            #region Urgot

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Urgot",
                    SpellName = "UrgotHeatseekingLineMissile",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 125,
                    Range = 1000,
                    Radius = 60,
                    MissileSpeed = 1600,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "UrgotHeatseekingLineMissile",
                    CanBeRemoved = true,
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Urgot",
                    SpellName = "UrgotPlasmaGrenade",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 1100,
                    Radius = 210,
                    MissileSpeed = 1500,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "UrgotPlasmaGrenadeBoom",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            #endregion Urgot

            #region Varus

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Varus",
                    SpellName = "VarusQMissilee",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1800,
                    Radius = 70,
                    MissileSpeed = 1900,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "VarusQMissile",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Varus",
                    SpellName = "VarusE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 1000,
                    Range = 925,
                    Radius = 235,
                    MissileSpeed = 1500,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "VarusE",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Varus",
                    SpellName = "VarusR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1200,
                    Radius = 120,
                    MissileSpeed = 1950,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "VarusRMissile",
                    CanBeRemoved = true,
                    CollisionObjects = CollisionableObjects.Heroes | CollisionableObjects.YasuoWall
                });

            #endregion Varus

            #region Veigar

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Veigar",
                    SpellName = "VeigarBalefulStrike",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 950,
                    Radius = 70,
                    MissileSpeed = 2000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "VeigarBalefulStrikeMis",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Veigar",
                    SpellName = "VeigarDarkMatter",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 1350,
                    Range = 900,
                    Radius = 225,
                    MissileSpeed = int.MaxValue,
                    AddHitbox = true,
                    DangerValue = 2
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Veigar",
                    SpellName = "VeigarEventHorizon",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotRing,
                    Delay = 500,
                    Range = 700,
                    Radius = 80,
                    MissileSpeed = int.MaxValue,
                    DangerValue = 3,
                    IsDangerous = true,
                    DontAddExtraDuration = true,
                    RingRadius = 350,
                    ExtraDuration = 3300,
                    DontCross = true
                });

            #endregion Veigar

            #region Velkoz

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Velkoz",
                    SpellName = "VelkozQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1100,
                    Radius = 50,
                    MissileSpeed = 1300,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "VelkozQMissile",
                    CanBeRemoved = true,
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Velkoz",
                    SpellName = "VelkozQSplit",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1100,
                    Radius = 55,
                    MissileSpeed = 2100,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "VelkozQMissileSplit",
                    CanBeRemoved = true,
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Velkoz",
                    SpellName = "VelkozW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1200,
                    Radius = 88,
                    MissileSpeed = 1700,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "VelkozWMissile",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Velkoz",
                    SpellName = "VelkozE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 500,
                    Range = 800,
                    Radius = 225,
                    MissileSpeed = 1500,
                    DangerValue = 2,
                    MissileSpellName = "VelkozEMissile"
                });

            #endregion Velkoz

            #region Vi

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Vi",
                    SpellName = "Vi-Q",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1000,
                    Radius = 90,
                    MissileSpeed = 1500,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "ViQMissile"
                });

            #endregion Vi

            #region Viktor

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Viktor",
                    SpellName = "Laser",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1500,
                    Radius = 80,
                    MissileSpeed = 780,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "ViktorDeathRayMissile",
                    ExtraMissileNames = new[] { "viktoreaugmissile" },
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            #endregion Viktor

            #region Vladimir

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Vladimir",
                    SpellName = "VladimirHemoplague",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 389,
                    Range = 700,
                    Radius = 375,
                    MissileSpeed = int.MaxValue,
                    DangerValue = 3,
                    MissileSpellName = "VladimirHemoplague"
                });

            #endregion Vladimir

            #region Xerath

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Xerath",
                    SpellName = "xeratharcanopulse2",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 600,
                    Range = 1600,
                    Radius = 100,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "xeratharcanopulse2"
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Xerath",
                    SpellName = "XerathArcaneBarrage2",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 700,
                    Range = 1000,
                    Radius = 200,
                    MissileSpeed = int.MaxValue,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "XerathArcaneBarrage2"
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Xerath",
                    SpellName = "XerathMageSpear",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 200,
                    Range = 1150,
                    Radius = 60,
                    MissileSpeed = 1400,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = true,
                    MissileSpellName = "XerathMageSpearMissile",
                    CanBeRemoved = true,
                    CollisionObjects =
                            CollisionableObjects.Heroes | CollisionableObjects.Minions | CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Xerath",
                    SpellName = "xerathrmissilewrapper",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 700,
                    Range = 5600,
                    Radius = 120,
                    MissileSpeed = int.MaxValue,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "xerathrmissilewrapper"
                });

            #endregion Xerath

            #region Yasuo 

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Yasuo",
                    SpellName = "yasuoq2w",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 400,
                    Range = 550,
                    Radius = 20,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = true,
                    MissileSpellName = "yasuoq2"
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Yasuo",
                    SpellName = "yasuoq3w",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 400,
                    Range = 1100,
                    Radius = 90,
                    MissileSpeed = 1200,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "yasuoq3mis",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Yasuo",
                    SpellName = "yasuoqw",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 400,
                    Range = 550,
                    Radius = 20,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    IsDangerous = true,
                    MissileSpellName = "yasuoq"
                });

            #endregion Yasuo

            #region Zac

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Zac",
                    SpellName = "ZacQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotLine,
                    Delay = 500,
                    Range = 550,
                    Radius = 120,
                    MissileSpeed = int.MaxValue,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "ZacQ"
                });

            #endregion Zac

            #region Zed

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Zed",
                    SpellName = "ZedQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 275,
                    Range = 925,
                    Radius = 50,
                    MissileSpeed = 1700,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "ZedQMissile",
                    FromObjects = new[] { "zedshadow" },
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            #endregion Zed

            #region Ziggs

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ziggs",
                    SpellName = "ZiggsQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 850,
                    Radius = 140,
                    MissileSpeed = 1700,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "ZiggsQSpell",
                    DisableFowDetection = true,
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ziggs",
                    SpellName = "ZiggsQBounce1",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 850,
                    Radius = 140,
                    MissileSpeed = 1700,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "ZiggsQSpell2",
                    ExtraMissileNames = new[] { "ZiggsQSpell2" },
                    DisableFowDetection = true,
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ziggs",
                    SpellName = "ZiggsQBounce2",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 850,
                    Radius = 160,
                    MissileSpeed = 1700,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "ZiggsQSpell3",
                    ExtraMissileNames = new[] { "ZiggsQSpell3" },
                    DisableFowDetection = true,
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ziggs",
                    SpellName = "ZiggsW",
                    Slot = SpellSlot.W,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 250,
                    Range = 1000,
                    Radius = 275,
                    MissileSpeed = 1750,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "ZiggsW",
                    DisableFowDetection = true,
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ziggs",
                    SpellName = "ZiggsE",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 500,
                    Range = 900,
                    Radius = 235,
                    MissileSpeed = 1750,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "ZiggsE",
                    DisableFowDetection = true,
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ziggs",
                    SpellName = "ZiggsR",
                    Slot = SpellSlot.R,
                    Type = SkillShotType.SkillshotCircle,
                    Range = 5300,
                    Radius = 500,
                    MissileSpeed = int.MaxValue,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "ZiggsR",
                    DisableFowDetection = true
                });

            #endregion Ziggs

            #region Zilean

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Zilean",
                    SpellName = "ZileanQ",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 300,
                    Range = 900,
                    Radius = 210,
                    MissileSpeed = 2000,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "ZileanQMissile"
                });

            #endregion Zilean

            #region Zyra

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Zyra",
                    SpellName = "ZyraQFissure",
                    Slot = SpellSlot.Q,
                    Type = SkillShotType.SkillshotCircle,
                    Delay = 850,
                    Range = 800,
                    Radius = 220,
                    MissileSpeed = int.MaxValue,
                    AddHitbox = true,
                    DangerValue = 2,
                    MissileSpellName = "ZyraQFissure"
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Zyra",
                    SpellName = "ZyraGraspingRoots",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1150,
                    Radius = 70,
                    MissileSpeed = 1150,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "ZyraGraspingRoots",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Zyra",
                    SpellName = "zyrapassivedeathmanager",
                    Slot = SpellSlot.E,
                    Type = SkillShotType.SkillshotMissileLine,
                    Delay = 500,
                    Range = 1474,
                    Radius = 70,
                    MissileSpeed = 2000,
                    FixedRange = true,
                    AddHitbox = true,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "zyrapassivedeathmanager",
                    CollisionObjects = CollisionableObjects.YasuoWall
                });

            #endregion Zyra
        }

        #endregion

        #region Public Methods and Operators

        public static SpellData GetByMissileName(string missileSpellName)
        {
            missileSpellName = missileSpellName.ToLower();
            return
                Spells.FirstOrDefault(
                    i =>
                    i.MissileSpellName.ToLower() == missileSpellName || i.ExtraMissileNames.Contains(missileSpellName));
        }

        public static SpellData GetByName(string spellName)
        {
            spellName = spellName.ToLower();
            return
                Spells.FirstOrDefault(i => i.SpellName.ToLower() == spellName || i.ExtraSpellNames.Contains(spellName));
        }

        public static SpellData GetBySourceObjectName(string objectName)
        {
            objectName = objectName.ToLowerInvariant();
            return
                Spells.Where(i => i.SourceObjectName.Length != 0)
                    .FirstOrDefault(i => objectName.Contains(i.SourceObjectName));
        }

        #endregion
    }
}