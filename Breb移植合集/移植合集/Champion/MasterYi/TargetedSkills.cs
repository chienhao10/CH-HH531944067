using System.Collections.Generic;
using EloBuddy;

namespace MasterSharp
{
    internal class TargetedSkills
    {
        public static List<TargSkill> targetedSkillsAll = new List<TargSkill>();

        public static List<TargSkill> dagerousBuffs = new List<TargSkill>();
        /*{
            "timebombenemybuff",
            "",
            "NocturneUnspeakableHorror"
        };*/


        public static void setUpSkills()
        {
            //Bufs
            dagerousBuffs.Add(new TargSkill("timebombenemybuff", 1, 1, 1, 300));
            dagerousBuffs.Add(new TargSkill("karthusfallenonetarget", 1, 1, 1, 300));
            dagerousBuffs.Add(new TargSkill("NocturneUnspeakableHorror", 1, 0, 1, 500));
            dagerousBuffs.Add(new TargSkill("virknockup", 1, 0, 1, 300));
            dagerousBuffs.Add(new TargSkill("tristanaechargesound", 1, 1, 1, 300));
            dagerousBuffs.Add(new TargSkill("zedulttargetmark", 1, 1, 1, 300));
            dagerousBuffs.Add(new TargSkill("fizzmarinerdoombomb", 1, 1, 1, 300));
            dagerousBuffs.Add(new TargSkill("soulshackles", 1, 1, 1, 300));
            dagerousBuffs.Add(new TargSkill("vladimirhemoplague", 1, 1, 1, 300));

            // name of spellName, Q use, W use --- 2-prioritize more , 1- prioritize less 0 dont use
            targetedSkillsAll.Add(new TargSkill("SyndraR", 0, 1, 1));
            targetedSkillsAll.Add(new TargSkill("VayneCondemn", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("Dazzle", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("Overload", 2, 1, 0));
            targetedSkillsAll.Add(new TargSkill("IceBlast", 2, 1, 0));
            targetedSkillsAll.Add(new TargSkill("LeblancChaosOrb", 2, 1, 0));
            targetedSkillsAll.Add(new TargSkill("JudicatorReckoning", 2, 1, 0));
            targetedSkillsAll.Add(new TargSkill("KatarinaQ", 2, 1, 0));
            targetedSkillsAll.Add(new TargSkill("NullLance", 2, 1, 0));
            targetedSkillsAll.Add(new TargSkill("FiddlesticksDarkWind", 2, 1, 0));
            targetedSkillsAll.Add(new TargSkill("CaitlynHeadshotMissile", 2, 1, 1));
            targetedSkillsAll.Add(new TargSkill("BrandWildfire", 2, 1, 1, 150));
            targetedSkillsAll.Add(new TargSkill("Disintegrate", 2, 1, 0));
            targetedSkillsAll.Add(new TargSkill("Frostbite", 2, 1, 0));
            targetedSkillsAll.Add(new TargSkill("AkaliMota", 2, 1, 0));
            //infiniteduresschannel  InfiniteDuress
            targetedSkillsAll.Add(new TargSkill("InfiniteDuress", 2, 0, 1, 0));
            targetedSkillsAll.Add(new TargSkill("PantheonW", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("blindingdart", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("JayceToTheSkies", 2, 1, 0));
            targetedSkillsAll.Add(new TargSkill("dariusexecute", 2, 1, 1));
            targetedSkillsAll.Add(new TargSkill("ireliaequilibriumstrike", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("maokaiunstablegrowth", 2, 1, 1));
            targetedSkillsAll.Add(new TargSkill("missfortunericochetshot", 2, 1, 0));
            targetedSkillsAll.Add(new TargSkill("nautilusgandline", 2, 1, 1));
            targetedSkillsAll.Add(new TargSkill("runeprison", 2, 1, 1));
            targetedSkillsAll.Add(new TargSkill("goldcardpreattack", 2, 0, 1, 0));
            targetedSkillsAll.Add(new TargSkill("vir", 2, 1, 1));
            targetedSkillsAll.Add(new TargSkill("zedult", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("AkaliMota", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("AkaliShadowDance", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("Headbutt", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("Frostbite", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("Disintegrate", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("PowerFist", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("BrandConflagration", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("BrandWildfire", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("CaitlynAceintheHole", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("CassiopeiaTwinFang", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("Feast", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("DariusNoxianTacticsONH", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("DariusExecute", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("DianaTeleport", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("dravenspinning", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("EliseHumanQ", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("EvelynnE", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("EzrealArcaneShift", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("Terrify", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("FiddlesticksDarkWind", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("FioraQ", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("FioraDance", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("FizzPiercingStrike", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("Parley", 2, 0, 1, 0));
            targetedSkillsAll.Add(new TargSkill("GarenQ", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("GarenR", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("IreliaGatotsu", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("IreliaEquilibriumStrike", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("SowTheWind", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("JarvanIVCataclysm", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("JaxLeapStrike", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("JaxEmpowerTwo", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("JayceToTheSkies", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("JayceThunderingBlow", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("KarmaSpiritBind", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("NullLance", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("NetherBlade", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("KatarinaQ", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("KatarinaE", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("JudicatorReckoning", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("JudicatorRighteousFury", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("KennenBringTheLight", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("khazixqlong", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("KhazixQ", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("LeblancChaosOrb", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("LeblancChaosOrbM", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("BlindMonkRKick", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("LeonaShieldOfDaybreak", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("LissandraR", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("LucianQ", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("LuluW", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("LuluE", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("SeismicShard", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("AlZaharMaleficVisions", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("AlZaharNetherGrasp", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("MaokaiUnstableGrowth", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("AlphaStrike", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("MissFortuneRicochetShot", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("MordekaiserMaceOfSpades", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("MordekaiserChildrenOfTheGrave", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("SoulShackles", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("NamiW", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("NasusQ", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("NasusW", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("NautilusGandLine", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("Takedown", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("NocturneUnspeakableHorror", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("NocturneParanoia", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("IceBlast", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("OlafRecklessStrike", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("PantheonQ", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("PantheonW", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("PoppyDevastatingBlow", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("PoppyHeroicCharge", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("QuinnE", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("RengarQ", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("PuncturingTaunt", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("RenektonPreExecute", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("Overload", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("SpellFlux", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("SejuaniWintersClaw", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("TwoShivPoisen", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("ShenVorpalStar", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("ShyvanaDoubleAttack", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("shyvanadoubleattackdragon", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("Fling", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("SkarnerImpale", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("SonaHymnofValor", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("SwainTorment", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("SwainDecrepify", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("SyndraR", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("TalonNoxianDiplomacy", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("TalonCutthroat", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("Dazzle", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("BlindingDart", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("DetonatingShot", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("BusterShot", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("TrundleTrollSmash", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("TrundlePain", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("MockingShout", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("goldcardpreattack", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("redcardpreattack", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("bluecardpreattack", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("Expunge", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("UdyrBearStance", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("UrgotHeatseekingLineMissile", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("UrgotHeatseekingLineqqMissile", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("UrgotSwap2", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("VayneCondemm", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("VeigarBalefulStrike", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("VeigarPrimordialBurst", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("ViR", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("ViktorPowerTransfer", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("VladimirTransfusion", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("VolibearQ", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("HungeringStrike", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("InfiniteDuress", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("XenZhaoComboTarget", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("XenZhaoSweep", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("YasuoDashWrapper", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("YasuoRKnockUpComboW", 2, 0, 1));
            targetedSkillsAll.Add(new TargSkill("zedult", 2, 0, 1));
            // targetedSkillsAll.Add(new TargSkill("NocturneUnspeakableHorror", 2, 0, 1,0));
        }

        internal class TargSkill
        {
            public int danger;
            public int delay = 250;
            public string sName;
            public int useQ;
            public int useW;

            public TargSkill(string name, int useq, int usew, int dangerlevel, int delayIn = 250)
            {
                sName = name;
                useQ = useq;
                useW = usew;
                danger = dangerlevel;
                delay = delayIn;
            }
        }
    }
}