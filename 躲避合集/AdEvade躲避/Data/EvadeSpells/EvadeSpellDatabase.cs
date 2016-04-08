using System.Collections.Generic;
using AdEvade.Data.Spells;
using EloBuddy;

namespace AdEvade.Data.EvadeSpells
{
    class EvadeSpellDatabase
    {
        public static List<EvadeSpellData> Spells = new List<EvadeSpellData>();

        static EvadeSpellDatabase()
        {
            #region Ahri

            Spells.Add(
            new EvadeSpellData
            {
                CharName = "Ahri",
                Dangerlevel = SpellDangerLevel.Extreme,
                Name = "AhriTumble",
                SpellName = "AhriTumble",
                Range = 500,
                SpellDelay = 50,
                Speed = 1575,
                SpellKey = SpellSlot.R,
                EvadeType = EvadeType.Dash,
                CastType = CastType.Position,
            });

            #endregion

            #region Caitlyn

            Spells.Add(
            new EvadeSpellData
            {
                CharName = "Caitlyn",
                Dangerlevel = SpellDangerLevel.High,
                Name = "CaitlynEntrapment",
                SpellName = "CaitlynEntrapment",
                Range = 490,
                SpellDelay = 50,
                Speed = 1000,
                IsReversed = true,
                FixedRange = true,
                SpellKey = SpellSlot.E,
                EvadeType = EvadeType.Dash,
                CastType = CastType.Position,
            });

            #endregion
            
            #region Corki

            Spells.Add(
            new EvadeSpellData
            {
                CharName = "Corki",
                Dangerlevel = SpellDangerLevel.High,
                Name = "CarpetBomb",
                SpellName = "CarpetBomb",
                Range = 790,
                SpellDelay = 50,
                Speed = 975,
                SpellKey = SpellSlot.W,
                EvadeType = EvadeType.Dash,
                CastType = CastType.Position,
            });

            #endregion

            #region Ekko

            Spells.Add(
            new EvadeSpellData
            {
                CharName = "Ekko",
                Dangerlevel = SpellDangerLevel.High,
                Name = "PhaseDive",
                SpellName = "EkkoE",
                Range = 350,
                FixedRange = true,
                SpellDelay = 50,
                Speed = 1150,
                SpellKey = SpellSlot.E,
                EvadeType = EvadeType.Dash,
                CastType = CastType.Position,
            });

            Spells.Add(
            new EvadeSpellData
            {
                CharName = "Ekko",
                Dangerlevel = SpellDangerLevel.High,
                Name = "PhaseDive2",
                SpellName = "EkkoEAttack",
                Range = 490,
                SpellDelay = 250,
                InfrontTarget = true,
                SpellKey = SpellSlot.Recall,
                EvadeType = EvadeType.Blink,                
                CastType = CastType.Target,                
                SpellTargets = new[] { SpellTargets.EnemyChampions, SpellTargets.EnemyMinions },
                IsSpecial = true,
            });

            Spells.Add(
            new EvadeSpellData
            {
                CharName = "Ekko",
                Dangerlevel = SpellDangerLevel.Extreme,
                Name = "Chronobreak",
                SpellName = "EkkoR",
                Range = 20000,
                SpellDelay = 50,
                SpellKey = SpellSlot.R,
                EvadeType = EvadeType.Blink,
                CastType = CastType.Self,
                IsSpecial = true,
            });

            #endregion

            #region Ezreal

            Spells.Add(
            new EvadeSpellData
            {
                CharName = "Ezreal",
                Dangerlevel = SpellDangerLevel.Normal,
                Name = "ArcaneShift",
                SpellName = "EzrealArcaneShift",
                Range = 450,
                SpellDelay = 250,
                SpellKey = SpellSlot.E,
                EvadeType = EvadeType.Blink,
                CastType = CastType.Position,
            });

            #endregion

            #region Gragas

            Spells.Add(
            new EvadeSpellData
            {
                CharName = "Gragas",
                Dangerlevel = SpellDangerLevel.Normal,
                Name = "BodySlam",
                SpellName = "GragasBodySlam",
                Range = 600,
                SpellDelay = 50,
                Speed = 900,
                SpellKey = SpellSlot.E,
                EvadeType = EvadeType.Dash,
                CastType = CastType.Position,
            });

            #endregion

            #region Gnar

            Spells.Add(
            new EvadeSpellData
            {
                CharName = "Gnar",
                Dangerlevel = SpellDangerLevel.High,
                Name = "GnarE",
                SpellName = "GnarE",
                Range = 475,
                SpellDelay = 50,
                Speed = 900,
                CheckSpellName = true,
                SpellKey = SpellSlot.E,
                EvadeType = EvadeType.Dash,
                CastType = CastType.Position,
            });

            Spells.Add(
            new EvadeSpellData
            {
                CharName = "Gnar",
                Dangerlevel = SpellDangerLevel.Extreme,
                Name = "GnarE",
                SpellName = "gnarbige",
                Range = 475,
                SpellDelay = 50,
                Speed = 800,
                CheckSpellName = true,
                SpellKey = SpellSlot.E,
                EvadeType = EvadeType.Dash,
                CastType = CastType.Position,
            });

            #endregion

            #region Graves

            Spells.Add(
            new EvadeSpellData
            {
                CharName = "Graves",
                Dangerlevel = SpellDangerLevel.Normal,
                Name = "QuickDraw",
                SpellName = "GravesMove",
                Range = 425,
                SpellDelay = 50,
                Speed = 1250,
                SpellKey = SpellSlot.E,
                EvadeType = EvadeType.Dash,
                CastType = CastType.Position,
            });

            #endregion

            #region Kassadin

            Spells.Add(
            new EvadeSpellData
            {
                CharName = "Kassadin",
                Dangerlevel = SpellDangerLevel.Low,
                Name = "RiftWalk",
                Range = 450,
                SpellDelay = 250,
                SpellKey = SpellSlot.R,
                EvadeType = EvadeType.Blink,
                CastType = CastType.Position,
            });

            #endregion

            #region Kayle

            Spells.Add(
            new EvadeSpellData
            {
                CharName = "Kayle",
                Dangerlevel = SpellDangerLevel.Extreme,
                Name = "Intervention",
                SpellName = "JudicatorIntervention",
                SpellDelay = 250,
                SpellKey = SpellSlot.R,
                EvadeType = EvadeType.SpellShield, //Invulnerability
                CastType = CastType.Target,
                SpellTargets = new[] { SpellTargets.AllyChampions },
            });

            #endregion

            #region Leblanc

            Spells.Add(
            new EvadeSpellData
            {
                CharName = "Leblanc",
                Dangerlevel = SpellDangerLevel.Normal,
                Name = "Distortion",
                SpellName = "LeblancSlide",
                Range = 600,
                SpellDelay = 50,
                Speed = 1600,
                SpellKey = SpellSlot.W,
                EvadeType = EvadeType.Dash,
                CastType = CastType.Position,
            });

            Spells.Add(
            new EvadeSpellData
            {
                CharName = "Leblanc",
                Dangerlevel = SpellDangerLevel.Normal,
                Name = "DistortionR",
                SpellName = "LeblancSlideM",
                CheckSpellName = true,
                Range = 600,
                SpellDelay = 50,
                Speed = 1600,
                SpellKey = SpellSlot.R,
                EvadeType = EvadeType.Dash,
                CastType = CastType.Position,
            });
                        
            #endregion

            #region LeeSin

            Spells.Add(
            new EvadeSpellData
            {
                CharName = "LeeSin",
                Dangerlevel = SpellDangerLevel.High,
                Name = "LeeSinW",
                SpellName = "BlindMonkWOne",
                Range = 700,
                Speed = 1400,
                SpellDelay = 50,
                SpellKey = SpellSlot.W,
                EvadeType = EvadeType.Dash,
                CastType = CastType.Target,
                SpellTargets = new[] { SpellTargets.AllyChampions, SpellTargets.AllyMinions },
            });

            #endregion

            #region Lucian

            Spells.Add(
            new EvadeSpellData
            {
                CharName = "Lucian",
                Dangerlevel = SpellDangerLevel.Low,
                Name = "RelentlessPursuit",
                SpellName = "LucianE",
                Range = 425,
                SpellDelay = 50,
                Speed = 1350,
                SpellKey = SpellSlot.E,
                EvadeType = EvadeType.Dash,
                CastType = CastType.Position,
            });

            #endregion

            #region Morgana

            Spells.Add(
            new EvadeSpellData
            {
                CharName = "Morgana",
                Dangerlevel = SpellDangerLevel.High,
                Name = "BlackShield",
                SpellName = "BlackShield",
                SpellDelay = 50,
                SpellKey = SpellSlot.E,
                EvadeType = EvadeType.SpellShield,
                CastType = CastType.Target,
                SpellTargets = new[] { SpellTargets.AllyChampions },
            });

            #endregion

            #region Nocturne

            Spells.Add(
            new EvadeSpellData
            {
                CharName = "Nocturne",
                Dangerlevel = SpellDangerLevel.High,
                Name = "ShroudofDarkness",
                SpellName = "NocturneShroudofDarkness",
                SpellDelay = 50,
                SpellKey = SpellSlot.W,
                EvadeType = EvadeType.SpellShield,
                CastType = CastType.Self,
            });

            #endregion

            #region Fiora

            Spells.Add(
            new EvadeSpellData
            {
                CharName = "Fiora",
                Dangerlevel = SpellDangerLevel.High,
                Name = "FioraW",
                SpellName = "FioraW",
                Range = 750,
                SpellDelay = 100,
                SpellKey = SpellSlot.W,
                EvadeType = EvadeType.WindWall,
                CastType = CastType.Position,
            });

            #endregion

            #region Fizz

            Spells.Add(
            new EvadeSpellData
            {
                CharName = "Fizz",
                Dangerlevel = SpellDangerLevel.High,
                Name = "FizzPiercingStrike",
                SpellName = "FizzPiercingStrike",
                Range = 550,
                Speed = 1400,
                FixedRange = true,
                SpellDelay = 50,
                SpellKey = SpellSlot.Q,
                EvadeType = EvadeType.Dash,
                CastType = CastType.Target,
                SpellTargets = new[] { SpellTargets.EnemyMinions, SpellTargets.EnemyChampions },
            });

            Spells.Add(
            new EvadeSpellData
            {
                CharName = "Fizz",
                Dangerlevel = SpellDangerLevel.High,
                Name = "FizzJump",
                SpellName = "FizzJump",
                Range = 400,
                Speed = 1400,
                FixedRange = true,
                SpellDelay = 50,
                SpellKey = SpellSlot.E,
                EvadeType = EvadeType.Dash,
                CastType = CastType.Position,
            });

            #endregion

            #region Riven

            Spells.Add(
            new EvadeSpellData
            {
                CharName = "Riven",
                Dangerlevel = SpellDangerLevel.Low,
                Name = "BrokenWings",
                SpellName = "RivenTriCleave",
                Range = 260,
                FixedRange = true,
                SpellDelay = 50,
                Speed = 560,
                SpellKey = SpellSlot.Q,
                EvadeType = EvadeType.Dash,
                CastType = CastType.Position,
            });

            Spells.Add(
            new EvadeSpellData
            {
                CharName = "Riven",
                Dangerlevel = SpellDangerLevel.Low,
                Name = "Valor",
                SpellName = "RivenFeint",
                Range = 325,
                FixedRange = true,
                SpellDelay = 50,
                Speed = 1200,
                SpellKey = SpellSlot.E,
                EvadeType = EvadeType.Dash,
                CastType = CastType.Position,
            });

            #endregion

            #region Sivir

            Spells.Add(
            new EvadeSpellData
            {
                CharName = "Sivir",
                Dangerlevel = SpellDangerLevel.Normal,
                Name = "SivirE",
                SpellName = "SivirE",
                SpellDelay = 50,
                SpellKey = SpellSlot.E,
                EvadeType = EvadeType.SpellShield,
                CastType = CastType.Self,
            });

            #endregion

            #region Shaco

            Spells.Add(
            new EvadeSpellData
            {
                CharName = "Shaco",
                Dangerlevel = SpellDangerLevel.High,
                Name = "Deceive",
                SpellName = "Deceive",
                Range = 400,
                SpellDelay = 250,
                SpellKey = SpellSlot.Q,
                EvadeType = EvadeType.Blink,
                CastType = CastType.Position,
            });

            /*Spells.Add(
            new EvadeSpellData
            {
                charName = "Shaco",
                Dangerlevel = SpellDangerLevel.High,
                name = "JackInTheBox",
                spellName = "JackInTheBox",
                range = 425,
                spellDelay = 250,
                SpellKey = SpellSlot.W,
                evadeType = EvadeType.WindWall,
                castType = CastType.Position,
            });*/

            #endregion

            #region Tristana

            Spells.Add(
            new EvadeSpellData
            {
                CharName = "Tristana",
                Dangerlevel = SpellDangerLevel.High,
                Name = "RocketJump",
                SpellName = "RocketJump",
                Range = 900,
                SpellDelay = 250,
                Speed = 1100,
                SpellKey = SpellSlot.W,
                EvadeType = EvadeType.Dash,
                CastType = CastType.Position,
            });         

            #endregion

            #region Tryndamare

            Spells.Add(
            new EvadeSpellData
            {
                CharName = "Tryndamare",
                Dangerlevel = SpellDangerLevel.High,
                Name = "SpinningSlash",
                SpellName = "Slash",
                Range = 660,
                SpellDelay = 50,
                Speed = 900,
                SpellKey = SpellSlot.E,
                EvadeType = EvadeType.Dash,
                CastType = CastType.Position,
            });    

            #endregion

            #region Vayne

            Spells.Add(
            new EvadeSpellData
            {
                CharName = "Vayne",
                Dangerlevel = SpellDangerLevel.Low,
                Name = "Tumble",
                SpellName = "VayneTumble",
                Range = 300,
                FixedRange = true,
                Speed = 900,
                SpellDelay = 50,
                SpellKey = SpellSlot.Q,
                EvadeType = EvadeType.Dash,
                CastType = CastType.Position,
            });

            #endregion

            #region Yasuo

            Spells.Add(
            new EvadeSpellData
            {
                CharName = "Yasuo",
                Dangerlevel = SpellDangerLevel.Normal,
                Name = "SweepingBlade",
                SpellName = "YasuoDashWrapper",
                Range = 475,
                FixedRange = true,
                Speed = 1000,
                SpellDelay = 50,
                SpellKey = SpellSlot.E,
                EvadeType = EvadeType.Dash,
                CastType = CastType.Target,
                SpellTargets = new[] { SpellTargets.EnemyChampions, SpellTargets.EnemyMinions },
            });

            Spells.Add(
            new EvadeSpellData
            {
                CharName = "Yasuo",
                Dangerlevel = SpellDangerLevel.High,
                Name = "WindWall",
                SpellName = "YasuoWMovingWall",
                Range = 400,
                SpellDelay = 250,
                SpellKey = SpellSlot.W,
                EvadeType = EvadeType.WindWall,
                CastType = CastType.Position,
            });

            #endregion

            #region MasterYi

            Spells.Add(
            new EvadeSpellData
            {
                CharName = "MasterYi",
                Dangerlevel = SpellDangerLevel.High,
                Name = "AlphaStrike",
                SpellName = "AlphaStrike",
                Range = 600,
                Speed = float.MaxValue,
                SpellDelay = 100,
                SpellKey = SpellSlot.Q,
                EvadeType = EvadeType.Blink,
                CastType = CastType.Target,
                SpellTargets = new[] { SpellTargets.EnemyChampions, SpellTargets.EnemyMinions },
            });

            #endregion

            #region Katarina

            Spells.Add(
            new EvadeSpellData
            {
                CharName = "Katarina",
                Dangerlevel = SpellDangerLevel.High,
                Name = "KatarinaE",
                SpellName = "KatarinaE",
                Range = 700,
                Speed = float.MaxValue,
                SpellDelay = 50,
                SpellKey = SpellSlot.E,
                EvadeType = EvadeType.Blink, //behind target
                CastType = CastType.Target,
                SpellTargets = new[] { SpellTargets.Targetables },
            });

            #endregion

            #region Talon

            Spells.Add(
            new EvadeSpellData
            {
                CharName = "Talon",
                Dangerlevel = SpellDangerLevel.High,
                Name = "Cutthroat",
                SpellName = "TalonCutthroat",
                Range = 700,
                Speed = float.MaxValue,
                SpellDelay = 50,
                SpellKey = SpellSlot.E,
                EvadeType = EvadeType.Blink, //behind target
                CastType = CastType.Target,
                SpellTargets = new[] { SpellTargets.EnemyChampions, SpellTargets.EnemyMinions },
            });

            #endregion

            #region Kindred
            Spells.Add(
            new EvadeSpellData
            {
                CharName = "Kindred",
                Dangerlevel = SpellDangerLevel.Normal,
                Name = "KindredQ",
                SpellName = "KindredQ",
                Range = 500,
                FixedRange = true,
                Speed = 900,
                SpellDelay = 50,
                SpellKey = SpellSlot.Q,
                EvadeType = EvadeType.Dash,
                CastType = CastType.Position,
            });
            #endregion

            #region Properties.Constants.AllChampions

            Spells.Add(
            new EvadeSpellData
            {
                CharName = Config.Constants.AllChampions,
                Dangerlevel = SpellDangerLevel.Extreme,
                Name = "Flash",
                SpellName = "SummonerFlash",
                Range = 400,
                FixedRange = true, //test
                SpellDelay = 50,
                IsSummonerSpell = true,
                SpellKey = SpellSlot.R,
                EvadeType = EvadeType.Blink,
                CastType = CastType.Position,
            });

            Spells.Add(
            new EvadeSpellData
            {
                CharName = Config.Constants.AllChampions,
                Dangerlevel = SpellDangerLevel.Extreme,
                Name = "Hourglass",
                SpellName = "ZhonyasHourglass",
                SpellDelay = 50,
                SpellKey = SpellSlot.Q,
                EvadeType = EvadeType.SpellShield, //Invulnerability
                CastType = CastType.Self,
                IsItem = true,
                ItemId = ItemId.Zhonyas_Hourglass,
            });

            Spells.Add(
            new EvadeSpellData
            {
                CharName = Config.Constants.AllChampions,
                Dangerlevel = SpellDangerLevel.Extreme,
                Name = "Witchcap",
                SpellName = "Witchcap",
                SpellDelay = 50,
                SpellKey = SpellSlot.Q,
                EvadeType = EvadeType.SpellShield, //Invulnerability
                CastType = CastType.Self,
                IsItem = true,
                ItemId = ItemId.Wooglets_Witchcap,
            });

            #endregion Properties.Constants.AllChampions
        }
    }
}
