using System.Collections.Generic;
using EloBuddy;
using LeagueSharp.Common;

namespace LCS_Lucian
{
    public enum SpellType
    {
        Skillshot,
        Targeted
    }

    public class SpellData
    {
        public string ChampionName;
        public int DangerLevel;
        public GapcloserType SkillType;
        public SpellSlot Slot;
        public string SpellName;
    }

    public static class AntiGapcloseSpell
    {
        public static List<SpellData> GapcloseableSpells = new List<SpellData>();

        static AntiGapcloseSpell()
        {
            GapcloseableSpells.Add(
                new SpellData
                {
                    ChampionName = "Aatrox",
                    Slot = SpellSlot.Q,
                    SpellName = "aatroxq",
                    SkillType = GapcloserType.Skillshot,
                    DangerLevel = 1
                });

            GapcloseableSpells.Add(
                new SpellData
                {
                    ChampionName = "Akali",
                    Slot = SpellSlot.R,
                    SpellName = "akalishadowdance",
                    SkillType = GapcloserType.Targeted,
                    DangerLevel = 5
                });

            GapcloseableSpells.Add(
                new SpellData
                {
                    ChampionName = "Alistar",
                    Slot = SpellSlot.W,
                    SpellName = "headbutt",
                    SkillType = GapcloserType.Targeted,
                    DangerLevel = 5
                });

            GapcloseableSpells.Add(
                new SpellData
                {
                    ChampionName = "Corki",
                    Slot = SpellSlot.W,
                    SpellName = "carpetbomb",
                    SkillType = GapcloserType.Skillshot,
                    DangerLevel = 1
                });

            GapcloseableSpells.Add(
                new SpellData
                {
                    ChampionName = "Diana",
                    Slot = SpellSlot.R,
                    SpellName = "dianateleport",
                    SkillType = GapcloserType.Targeted,
                    DangerLevel = 4
                });

            GapcloseableSpells.Add(
                new SpellData
                {
                    ChampionName = "Ekko",
                    Slot = SpellSlot.E,
                    SpellName = "ekkoe",
                    SkillType = GapcloserType.Skillshot,
                    DangerLevel = 4
                });

            GapcloseableSpells.Add(
                new SpellData
                {
                    ChampionName = "Elise",
                    Slot = SpellSlot.Q,
                    SpellName = "elisespiderqcast",
                    SkillType = GapcloserType.Skillshot,
                    DangerLevel = 1
                });

            GapcloseableSpells.Add(
                new SpellData
                {
                    ChampionName = "Fiora",
                    Slot = SpellSlot.Q,
                    SpellName = "fioraq",
                    SkillType = GapcloserType.Skillshot,
                    DangerLevel = 1
                });

            GapcloseableSpells.Add(
                new SpellData
                {
                    ChampionName = "Fizz",
                    Slot = SpellSlot.Q,
                    SpellName = "fizzpiercingstrike",
                    SkillType = GapcloserType.Targeted,
                    DangerLevel = 3
                });

            GapcloseableSpells.Add(
                new SpellData
                {
                    ChampionName = "Gnar",
                    Slot = SpellSlot.E,
                    SpellName = "gnare",
                    SkillType = GapcloserType.Skillshot,
                    DangerLevel = 1
                });

            GapcloseableSpells.Add(
                new SpellData
                {
                    ChampionName = "Gragas",
                    Slot = SpellSlot.E,
                    SpellName = "gragase",
                    SkillType = GapcloserType.Skillshot,
                    DangerLevel = 2
                });

            GapcloseableSpells.Add(
                new SpellData
                {
                    ChampionName = "Graves",
                    Slot = SpellSlot.E,
                    SpellName = "gravesmove",
                    SkillType = GapcloserType.Skillshot,
                    DangerLevel = 4
                });

            GapcloseableSpells.Add(
                new SpellData
                {
                    ChampionName = "Hecarim",
                    Slot = SpellSlot.R,
                    SpellName = "hecarimult",
                    SkillType = GapcloserType.Skillshot,
                    DangerLevel = 4
                });

            GapcloseableSpells.Add(
                new SpellData
                {
                    ChampionName = "Illaoi",
                    Slot = SpellSlot.W,
                    SpellName = "illaoiwattack",
                    SkillType = GapcloserType.Targeted,
                    DangerLevel = 3
                });

            GapcloseableSpells.Add(
                new SpellData
                {
                    ChampionName = "Irelia",
                    Slot = SpellSlot.Q,
                    SpellName = "ireliagatotsu",
                    SkillType = GapcloserType.Targeted,
                    DangerLevel = 5
                });

            GapcloseableSpells.Add(
                new SpellData
                {
                    ChampionName = "JarvanIV",
                    Slot = SpellSlot.Q,
                    SpellName = "jarvanivdragonstrike",
                    SkillType = GapcloserType.Skillshot,
                    DangerLevel = 2
                });

            GapcloseableSpells.Add(
                new SpellData
                {
                    ChampionName = "Jax",
                    Slot = SpellSlot.Q,
                    SpellName = "jaxleapstrike",
                    SkillType = GapcloserType.Targeted,
                    DangerLevel = 5
                });

            GapcloseableSpells.Add(
                new SpellData
                {
                    ChampionName = "Jayce",
                    Slot = SpellSlot.Q,
                    SpellName = "jaycetotheskies",
                    SkillType = GapcloserType.Targeted,
                    DangerLevel = 3
                });

            GapcloseableSpells.Add(
                new SpellData
                {
                    ChampionName = "Khazix",
                    Slot = SpellSlot.E,
                    SpellName = "khazixe",
                    SkillType = GapcloserType.Skillshot,
                    DangerLevel = 3
                });

            GapcloseableSpells.Add(
                new SpellData
                {
                    ChampionName = "Khazix",
                    Slot = SpellSlot.E,
                    SpellName = "khazixelong",
                    SkillType = GapcloserType.Skillshot,
                    DangerLevel = 3
                });

            GapcloseableSpells.Add(
                new SpellData
                {
                    ChampionName = "LeBlanc",
                    Slot = SpellSlot.W,
                    SpellName = "leblancslide",
                    SkillType = GapcloserType.Skillshot,
                    DangerLevel = 5
                });

            GapcloseableSpells.Add(
                new SpellData
                {
                    ChampionName = "LeBlanc",
                    Slot = SpellSlot.R,
                    SpellName = "leblancslidem",
                    SkillType = GapcloserType.Skillshot,
                    DangerLevel = 5
                });

            GapcloseableSpells.Add(
                new SpellData
                {
                    ChampionName = "LeeSin",
                    Slot = SpellSlot.Q,
                    SpellName = "blindmonkqtwo",
                    SkillType = GapcloserType.Targeted,
                    DangerLevel = 3
                });

            GapcloseableSpells.Add(
                new SpellData
                {
                    ChampionName = "Lucian",
                    Slot = SpellSlot.E,
                    SpellName = "luciane",
                    SkillType = GapcloserType.Skillshot,
                    DangerLevel = 4
                });

            GapcloseableSpells.Add(
                new SpellData
                {
                    ChampionName = "Pantheon",
                    Slot = SpellSlot.W,
                    SpellName = "pantheon_leapbash",
                    SkillType = GapcloserType.Targeted,
                    DangerLevel = 4
                });

            GapcloseableSpells.Add(
                new SpellData
                {
                    ChampionName = "Poppy",
                    Slot = SpellSlot.E,
                    SpellName = "poppyheroiccharge",
                    SkillType = GapcloserType.Targeted,
                    DangerLevel = 5
                });

            GapcloseableSpells.Add(
                new SpellData
                {
                    ChampionName = "Renekton",
                    Slot = SpellSlot.E,
                    SpellName = "renektonsliceanddice",
                    SkillType = GapcloserType.Skillshot,
                    DangerLevel = 4
                });

            GapcloseableSpells.Add(
                new SpellData
                {
                    ChampionName = "Riven",
                    Slot = SpellSlot.Q,
                    SpellName = "riventricleave",
                    SkillType = GapcloserType.Skillshot,
                    DangerLevel = 4
                });

            GapcloseableSpells.Add(
                new SpellData
                {
                    ChampionName = "Riven",
                    Slot = SpellSlot.E,
                    SpellName = "rivenfeint",
                    SkillType = GapcloserType.Skillshot,
                    DangerLevel = 4
                });

            GapcloseableSpells.Add(
                new SpellData
                {
                    ChampionName = "Sejuani",
                    Slot = SpellSlot.Q,
                    SpellName = "sejuaniarcticassault",
                    SkillType = GapcloserType.Skillshot,
                    DangerLevel = 4
                });

            GapcloseableSpells.Add(
                new SpellData
                {
                    ChampionName = "Shen",
                    Slot = SpellSlot.E,
                    SpellName = "shenshadowdash",
                    SkillType = GapcloserType.Skillshot,
                    DangerLevel = 5
                });

            GapcloseableSpells.Add(
                new SpellData
                {
                    ChampionName = "Shyvana",
                    Slot = SpellSlot.R,
                    SpellName = "shyvanatransformcast",
                    SkillType = GapcloserType.Skillshot,
                    DangerLevel = 5
                });

            GapcloseableSpells.Add(
                new SpellData
                {
                    ChampionName = "Tristana",
                    Slot = SpellSlot.W,
                    SpellName = "rocketjump",
                    SkillType = GapcloserType.Skillshot,
                    DangerLevel = 4
                });

            GapcloseableSpells.Add(
                new SpellData
                {
                    ChampionName = "Tryndamere",
                    Slot = SpellSlot.E,
                    SpellName = "slashcast",
                    SkillType = GapcloserType.Skillshot,
                    DangerLevel = 2
                });

            GapcloseableSpells.Add(
                new SpellData
                {
                    ChampionName = "XinZhao",
                    Slot = SpellSlot.E,
                    SpellName = "xenzhaosweep",
                    SkillType = GapcloserType.Targeted,
                    DangerLevel = 4
                });

            GapcloseableSpells.Add(
                new SpellData
                {
                    ChampionName = "Yasuo",
                    Slot = SpellSlot.E,
                    SpellName = "yasuodashwrapper",
                    SkillType = GapcloserType.Targeted,
                    DangerLevel = 4
                });

            GapcloseableSpells.Add(
                new SpellData
                {
                    ChampionName = "Zac",
                    Slot = SpellSlot.E,
                    SpellName = "zace",
                    SkillType = GapcloserType.Skillshot,
                    DangerLevel = 3
                });

            GapcloseableSpells.Add(
                new SpellData
                {
                    ChampionName = "Ziggs",
                    Slot = SpellSlot.W,
                    SpellName = "ziggswtoggle",
                    SkillType = GapcloserType.Skillshot,
                    DangerLevel = 4
                });

            GapcloseableSpells.Add(
                new SpellData
                {
                    ChampionName = "Vi",
                    Slot = SpellSlot.Q,
                    SpellName = "ViQ",
                    SkillType = GapcloserType.Skillshot,
                    DangerLevel = 5
                });
        }
    }
}