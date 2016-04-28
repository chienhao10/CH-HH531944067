using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using SCommon.Database;

namespace SCommon.Damage
{
    public static class AutoAttack
    {
        private static readonly List<ItemPassive> ItemPassives;
        private static readonly List<AAEnpower>[] AAEnpowers;

        /// <summary>
        ///     Initializes AutoAttack class
        /// </summary>
        static AutoAttack()
        {
            #region Item Passives

            ItemPassives = new List<ItemPassive>();

            //Recursive bow
            ItemPassives.Add(
                new ItemPassive
                {
                    ItemID = 1043,
                    GetDamage = (s, t) => (float) s.CalcDamage(t, DamageType.Physical, 15)
                });

            //Runaan
            ItemPassives.Add(
                new ItemPassive
                {
                    ItemID = 3085,
                    GetDamage = (s, t) => (float) s.CalcDamage(t, DamageType.Physical, 15)
                });

            //Botrk
            ItemPassives.Add(
                new ItemPassive
                {
                    ItemID = 3153,
                    GetDamage = (s, t) =>
                    {
                        var dmg = (float) s.CalcDamage(t, DamageType.Physical, t.Health*0.06f);
                        if (dmg < 10)
                            dmg = 10;

                        if (!(t is AIHeroClient) && dmg > 60)
                            dmg = 60;

                        return dmg;
                    }
                });

            //Wit's end
            ItemPassives.Add(
                new ItemPassive
                {
                    ItemID = 3091,
                    GetDamage = (s, t) => (float) s.CalcDamage(t, DamageType.Magical, 42)
                });

            //Nashor's
            ItemPassives.Add(
                new ItemPassive
                {
                    ItemID = 3115,
                    GetDamage =
                        (s, t) =>
                            (float) s.CalcDamage(t, DamageType.Magical, 15 + ObjectManager.Player.TotalMagicalDamage *15/100)
                });

            ////Devourer
            //ItemPassives.Add(
            //    new ItemPassive
            //    {
            //        ItemID = 3710,
            //        GetDamage = (s, t) => (float)s.CalcDamage(t, DamageType.Magical, 30 * 1) //devourer stacks are not calculated yet
            //    });

            //ItemPassives.Add(
            //    new ItemPassive
            //    {
            //        ItemID = 3718,
            //        GetDamage = (s, t) => (float)s.CalcDamage(t, DamageType.Magical, 30 * 1) //devourer stacks are not calculated yet
            //    });

            //ItemPassives.Add(
            //    new ItemPassive
            //    {
            //        ItemID = 3722,
            //        GetDamage = (s, t) => (float)s.CalcDamage(t, DamageType.Magical, 30 * 1) //devourer stacks are not calculated yet
            //    });

            //ItemPassives.Add(
            //    new ItemPassive
            //    {
            //        ItemID = 3722,
            //        GetDamage = (s, t) => (float)s.CalcDamage(t, DamageType.Magical, 30 * 1) //devourer stacks are not calculated yet
            //    });

            ////Sated Devourer
            //ItemPassives.Add(
            //    new ItemPassive
            //    {
            //        ItemID = 3930,
            //        GetDamage = (s, t) => (float)s.CalcDamage(t, DamageType.Magical, 50) 
            //    });

            //ItemPassives.Add(
            //    new ItemPassive
            //    {
            //        ItemID = 3931,
            //        GetDamage = (s, t) => (float)s.CalcDamage(t, DamageType.Magical, 50) 
            //    });

            //ItemPassives.Add(
            //    new ItemPassive
            //    {
            //        ItemID = 3932,
            //        GetDamage = (s, t) => (float)s.CalcDamage(t, DamageType.Magical, 50)
            //    });

            //ItemPassives.Add(
            //    new ItemPassive
            //    {
            //        ItemID = 3933,
            //        GetDamage = (s, t) => (float)s.CalcDamage(t, DamageType.Magical, 50)
            //    });

            #endregion

            //to do: kalista w passive

            #region Hero Passives

            AAEnpowers = new List<AAEnpower>[Data.GetMaxHeroID() + 1];

            //aatrox w
            AAEnpowers[Data.GetID("AAtrox")] = new List<AAEnpower>();
            AAEnpowers[Data.GetID("AAtrox")].Add(
                new AAEnpower
                {
                    ChampionName = "Aatrox",
                    IsActive =
                        (source, target) => source.HasBuff("AatroxWPower") && source.HasBuff("AatroxWONHPowerBuff"),
                    GetDamage = (source, target) => source.GetSpellDamage(target, SpellSlot.W)
                });

            //caitlyn passive
            AAEnpowers[Data.GetID("Caitlyn")] = new List<AAEnpower>();
            AAEnpowers[Data.GetID("Caitlyn")].Add(
                new AAEnpower
                {
                    ChampionName = "Caitlyn",
                    IsActive = (source, target) => source.HasBuff("caitlynheadshot"),
                    GetDamage =
                        (source, target) =>
                            (float)
                                source.CalcDamage(
                                    target,
                                    DamageType.Physical,
                                    1.5d*(source.BaseAttackDamage + source.FlatPhysicalDamageMod))
                });

            //draven q
            AAEnpowers[Data.GetID("Draven")] = new List<AAEnpower>();
            AAEnpowers[Data.GetID("Draven")].Add(
                new AAEnpower
                {
                    ChampionName = "Draven",
                    IsActive = (source, target) => source.HasBuff("DravenSpinning"),
                    GetDamage =
                        (source, target) =>
                            (float)
                                source.CalcDamage(
                                    target,
                                    DamageType.Physical,
                                    0.45d*(source.BaseAttackDamage + source.FlatPhysicalDamageMod))
                });

            //corki passive
            AAEnpowers[Data.GetID("Corki")] = new List<AAEnpower>();
            AAEnpowers[Data.GetID("Corki")].Add(
                new AAEnpower
                {
                    ChampionName = "Corki",
                    IsActive = (source, target) => source.HasBuff("rapidreload") && !(target is Obj_AI_Turret),
                    GetDamage =
                        (source, target) => (float) 0.1d*(source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                });

            //ekko passive
            AAEnpowers[Data.GetID("Ekko")] = new List<AAEnpower>();
            AAEnpowers[Data.GetID("Ekko")].Add(
                new AAEnpower
                {
                    ChampionName = "Ekko",
                    IsActive = (source, target) => target.GetBuffCount("EkkoStacks") == 2,
                    GetDamage = (source, target) =>
                        (float)
                            source.CalcDamage(target, DamageType.Magical,
                                10 + source.Level*10 + source.TotalMagicalDamage*0.8)
                });

            //ekko w
            AAEnpowers[Data.GetID("Ekko")].Add(
                new AAEnpower
                {
                    ChampionName = "Ekko",
                    IsActive = (source, target) => target.HealthPercent < 30,
                    GetDamage = (source, target) =>
                    {
                        var dmg =
                            (float)
                                source.CalcDamage(target, DamageType.Magical,
                                    (target.MaxHealth - target.Health)*(5 + Math.Floor(source.TotalMagicalDamage/100)*2.2f)/
                                    100);
                        if (!(target is AIHeroClient) && dmg > 150f)
                            dmg = 150f;

                        return dmg;
                    }
                });

            //gnar w
            AAEnpowers[Data.GetID("Gnar")] = new List<AAEnpower>();
            AAEnpowers[Data.GetID("Gnar")].Add(
                new AAEnpower
                {
                    ChampionName = "Gnar",
                    IsActive = (source, target) => target.GetBuffCount("gnarwproc") == 2,
                    GetDamage = (source, target) => source.GetSpellDamage(target, SpellSlot.W)
                });

            //jinx q
            AAEnpowers[Data.GetID("Jinx")] = new List<AAEnpower>();
            AAEnpowers[Data.GetID("Jinx")].Add(
                new AAEnpower
                {
                    ChampionName = "Jinx",
                    IsActive = (source, target) => source.HasBuff("JinxQ"),
                    GetDamage =
                        (source, target) =>
                            (float)
                                source.CalcDamage(
                                    target,
                                    DamageType.Physical,
                                    0.1d*(source.BaseAttackDamage + source.FlatPhysicalDamageMod))
                });

            //jhin passive
            AAEnpowers[Data.GetID("Jhin")] = new List<AAEnpower>();
            AAEnpowers[Data.GetID("Jhin")].Add(
                new AAEnpower
                {
                    ChampionName = "Jhin",
                    IsActive = (source, target) => source.HasBuff("jhinpassiveattackbuff"),
                    GetDamage =
                        (source, target) =>
                            (float)
                                source.CalcDamage(
                                    target,
                                    DamageType.Physical,
                                    source.TotalAttackDamage*0.5f +
                                    (target.MaxHealth - target.Health)*
                                    new[] {0.15f, 0.20f, 0.25f}[Math.Min(2, (source.Level - 1)/5)])
                });

            //katarina q
            AAEnpowers[Data.GetID("Katarina")] = new List<AAEnpower>();
            AAEnpowers[Data.GetID("Katarina")].Add(
                new AAEnpower
                {
                    ChampionName = "Katarina",
                    IsActive = (source, target) => target.HasBuff("katarinaqmark"),
                    GetDamage = (source, target) => (float) source.LSGetSpellDamage(target, SpellSlot.Q, 1)
                });

            //kowmaw w
            AAEnpowers[Data.GetID("KogMaw")] = new List<AAEnpower>();
            AAEnpowers[Data.GetID("KogMaw")].Add(
                new AAEnpower
                {
                    ChampionName = "KogMaw",
                    IsActive = (source, target) => source.HasBuff("KogMawBioArcaneBarrage"),
                    GetDamage = (source, target) => source.GetSpellDamage(target, SpellSlot.W)
                });

            //miss fortune passive
            AAEnpowers[Data.GetID("MissFortune")] = new List<AAEnpower>();
            AAEnpowers[Data.GetID("MissFortune")].Add(
                new AAEnpower
                {
                    ChampionName = "MissFortune",
                    IsActive = (source, target) => source.HasBuff("missfortunepassive"),
                    GetDamage =
                        (source, target) =>
                            (float)
                                source.CalcDamage(
                                    target,
                                    DamageType.Magical,
                                    (float) 0.06*(source.BaseAttackDamage + source.FlatPhysicalDamageMod))
                });

            //nasus q
            AAEnpowers[Data.GetID("Nasus")] = new List<AAEnpower>();
            AAEnpowers[Data.GetID("Nasus")].Add(
                new AAEnpower
                {
                    ChampionName = "Nasus",
                    IsActive = (source, target) => source.HasBuff("NasusQ"),
                    GetDamage = (source, target) => source.GetSpellDamage(target, SpellSlot.Q)
                });


            //orianna passive
            AAEnpowers[Data.GetID("Orianna")] = new List<AAEnpower>();
            AAEnpowers[Data.GetID("Orianna")].Add(
                new AAEnpower
                {
                    ChampionName = "Orianna",
                    IsActive = (source, target) => source.HasBuff("orianaspellsword"),
                    GetDamage =
                        (source, target) =>
                            (float)
                                source.CalcDamage(
                                    target,
                                    DamageType.Magical,
                                    (float) 0.15*source.TotalMagicalDamage
                                    +
                                    new float[] {10, 10, 10, 18, 18, 18, 26, 26, 26, 34, 34, 34, 42, 42, 42, 50, 50, 50}
                                        [
                                            source.Level - 1])
                });

            //rengar q
            AAEnpowers[Data.GetID("Rengar")] = new List<AAEnpower>();
            AAEnpowers[Data.GetID("Rengar")].Add(
                new AAEnpower
                {
                    ChampionName = "Rengar",
                    IsActive = (source, target) => source.HasBuff("rengarqbase"),
                    GetDamage =
                        (source, target) =>
                            (float)
                                source.CalcDamage(target, DamageType.Physical,
                                    new[] {30, 60, 90, 120, 150}[source.GetSpell(SpellSlot.Q).Level - 1] +
                                    (source.BaseAttackDamage + source.FlatPhysicalDamageMod)*
                                    new[] {0, 5, 10, 15, 20}[source.GetSpell(SpellSlot.Q).Level - 1]/100f)
                });

            //rengar q emp
            AAEnpowers[Data.GetID("Rengar")].Add(
                new AAEnpower
                {
                    ChampionName = "Rengar",
                    IsActive = (source, target) => source.HasBuff("rengarqemp"),
                    GetDamage =
                        (source, target) =>
                            (float)
                                source.CalcDamage(target, DamageType.Physical,
                                    new[]
                                    {
                                        30, 45, 60, 75, 90, 105, 120, 135, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240
                                    }[source.Level - 1] + (source.BaseAttackDamage + source.FlatPhysicalDamageMod)*0.3f)
                });


            //riven passive
            AAEnpowers[Data.GetID("Riven")] = new List<AAEnpower>();
            AAEnpowers[Data.GetID("Riven")].Add(
                new AAEnpower
                {
                    ChampionName = "Riven",
                    IsActive = (source, target) => source.GetBuffCount("rivenpassiveaaboost") > 0,
                    GetDamage =
                        (source, target) =>
                            (float)
                                source.CalcDamage(
                                    target,
                                    DamageType.Physical,
                                    new[]
                                    {
                                        0.2, 0.2, 0.25, 0.25, 0.25, 0.3, 0.3, 0.3, 0.35, 0.35, 0.35, 0.4, 0.4, 0.4, 0.45,
                                        0.45, 0.45, 0.5
                                    }[source.Level - 1]
                                    *(source.BaseAttackDamage + source.FlatPhysicalDamageMod))
                });


            //teemo e
            AAEnpowers[Data.GetID("Teemo")] = new List<AAEnpower>();
            AAEnpowers[Data.GetID("Teemo")].Add(
                new AAEnpower
                {
                    ChampionName = "Teemo",
                    IsActive = (source, target) => source.HasBuff("ToxicShot"),
                    GetDamage =
                        (source, target) =>
                            (float)
                                source.CalcDamage(
                                    target,
                                    DamageType.Magical,
                                    source.Spellbook.GetSpell(SpellSlot.E).Level*10 + source.TotalMagicalDamage*0.3)
                });

            //twisted fate e
            AAEnpowers[Data.GetID("TwistedFate")] = new List<AAEnpower>();
            AAEnpowers[Data.GetID("TwistedFate")].Add(
                new AAEnpower
                {
                    ChampionName = "TwistedFate",
                    IsActive = (source, target) => source.HasBuff("cardmasterstackparticle"),
                    GetDamage =
                        (source, target) =>
                            (float)
                                Math.Floor(source.CalcDamage(target, DamageType.Magical,
                                    new[] {55, 80, 105, 130, 155}[source.Spellbook.GetSpell(SpellSlot.E).Level - 1] +
                                    source.TotalMagicalDamage*0.5f))
                });

            //varus w
            AAEnpowers[Data.GetID("Varus")] = new List<AAEnpower>();
            AAEnpowers[Data.GetID("Varus")].Add(
                new AAEnpower
                {
                    ChampionName = "Varus",
                    IsActive = (source, target) => source.HasBuff("VarusW"),
                    GetDamage = (source, target) => source.GetSpellDamage(target, SpellSlot.W)
                });

            //vayne q
            AAEnpowers[Data.GetID("Vayne")] = new List<AAEnpower>();
            AAEnpowers[Data.GetID("Vayne")].Add(
                new AAEnpower
                {
                    ChampionName = "Vayne",
                    IsActive = (source, target) => source.HasBuff("vaynetumblebonus"),
                    GetDamage = (source, target) => source.GetSpellDamage(target, SpellSlot.Q)
                });

            //vayne w
            AAEnpowers[Data.GetID("Vayne")].Add(
                new AAEnpower
                {
                    ChampionName = "Vayne",
                    IsActive =
                        (source, target) =>
                            (from buff in target.Buffs where buff.Name == "vaynesilvereddebuff" select buff.Count)
                                .FirstOrDefault() == 2,
                    GetDamage = (source, target) =>
                    {
                        var dmg = target.MaxHealth*(0.06f + 0.015f*(source.GetSpell(SpellSlot.W).Level - 1));
                        if (!(target is AIHeroClient) && dmg > 200)
                            dmg = 200;

                        if (dmg < 40 + 20*(source.GetSpell(SpellSlot.W).Level - 1))
                            dmg = 40 + 20*(source.GetSpell(SpellSlot.W).Level - 1);

                        return dmg;
                    }
                });

            //viktor q
            AAEnpowers[Data.GetID("Viktor")] = new List<AAEnpower>();
            AAEnpowers[Data.GetID("Viktor")].Add(
                new AAEnpower
                {
                    ChampionName = "Viktor",
                    IsActive = (source, target) => source.HasBuff("viktorpowertransferreturn"),
                    GetDamage =
                        (source, target) =>
                            (float)
                                source.CalcDamage(
                                    target,
                                    DamageType.Magical,
                                    (float) 0.5d*source.TotalMagicalDamage
                                    +
                                    new float[]
                                    {20, 25, 30, 35, 40, 45, 50, 55, 60, 70, 80, 90, 110, 130, 150, 170, 190, 210}[
                                        source.Level - 1])
                });

            //ziggs passive
            AAEnpowers[Data.GetID("Ziggs")] = new List<AAEnpower>();
            AAEnpowers[Data.GetID("Ziggs")].Add(
                new AAEnpower
                {
                    ChampionName = "Ziggs",
                    IsActive = (source, target) => source.HasBuff("ziggsshortfuse"),
                    GetDamage =
                        (source, target) =>
                            (float)
                                source.CalcDamage(
                                    target,
                                    DamageType.Magical,
                                    (float) 0.25d*source.TotalMagicalDamage
                                    +
                                    new float[]
                                    {20, 24, 28, 32, 36, 40, 48, 56, 64, 72, 80, 88, 100, 112, 124, 136, 148, 160}[
                                        source.Level - 1])
                });

            #endregion
        }

        /// <summary>
        ///     Gets aa damage to given target
        /// </summary>
        /// <param name="target">Target</param>
        /// <returns></returns>
        public static float GetDamage(Obj_AI_Base target, bool passiveCheck = false)
        {
            var hero = ObjectManager.Player;

            var result = hero.TotalAttackDamage;
            var k = hero.ChampionName == "Kalista" ? 0.9f : 1f;
            var reduction = 0f;

            #region relic shield

            var minionTarget = target as Obj_AI_Minion;
            if (hero.IsMelee() && minionTarget != null && minionTarget.IsEnemy
                && minionTarget.Team != GameObjectTeam.Neutral
                && hero.Buffs.Any(buff => buff.Name == "talentreaperdisplay" && buff.Count > 0))
            {
                if (
                    HeroManager.AllHeroes.Any(
                        h =>
                            h.NetworkId != hero.NetworkId && h.Team == hero.Team
                            && h.Distance(minionTarget.Position) < 1100))
                {
                    var value = 0;

                    if (Items.HasItem(3302, hero))
                        value = 200; // Relic Shield
                    else if (Items.HasItem(3097, hero))
                        value = 240; // Targon's Brace
                    else if (Items.HasItem(3401, hero))
                        value = 400; // Face of the Mountain

                    return value + hero.TotalAttackDamage;
                }
            }

            #endregion

            var targetHero = target as AIHeroClient;
            if (targetHero != null)
            {
                // Ninja tabi
                if (Items.HasItem(3047, targetHero))
                    k *= 0.9f;

                // Nimble Fighter
                if (targetHero.ChampionName == "Fizz")
                    reduction +=
                        new[] {4, 4, 4, 6, 6, 6, 8, 8, 8, 10, 10, 10, 12, 12, 12, 14, 14, 14}[targetHero.Level - 1];
            }

            if (passiveCheck)
            {
                var mastery = ObjectManager.Player.Masteries.FirstOrDefault(p => p.Page == MasteryPage.Cunning);
                if (mastery != null)
                    result += Math.Min(5, (int) mastery.Points);
            }
            //float dmg = (float)LeagueSharp.Common.Damage.CalcDamage(hero, target, DamageType.Physical, (result - reduction) * k + (passiveCheck ? PassiveFlatMod(ObjectManager.Player, target) : 0));
            var dmg = 0f;
            if (target.Type == GameObjectType.obj_AI_Minion)
                dmg = CalcPhysicalDamage(hero, target, (result - reduction)*k);
            else
                dmg =
                    (float)
                        ObjectManager.Player.CalcDamage(target, DamageType.Physical,
                            k + (passiveCheck ? PassiveFlatMod(ObjectManager.Player, target) : 0));
            if (hero.ChampionName == "Azir")
            {
                var cnt =
                    ObjectManager.Get<GameObject>()
                        .Count(p => p.IsAlly && p.Name == "AzirSoldier" && p.Position.Distance(target.Position) < 315);
                if (cnt > 0)
                {
                    dmg =
                        (float)
                            hero.CalcDamage(target, DamageType.Magical,
                                new[] {50, 55, 60, 65, 70, 75, 80, 85, 90, 100, 110, 120, 130, 140, 150, 160, 170, 180}[
                                    ObjectManager.Player.Level - 1] + 0.6f*hero.TotalMagicalDamage);
                    return dmg + dmg*0.25f*(cnt - 1) - 1;
                }
            }
            else if (hero.ChampionName == "Pantheon")
            {
                if (target.HealthPercent <= 15 && hero.Spellbook.GetSpell(SpellSlot.E).Level > 0)
                    dmg *= 2;
            }
            else if (hero.ChampionName == "Lucian")
            {
                if (ObjectManager.Player.HasBuff("lucianpassivebuff"))
                    dmg *= 2;
            }
            else if (hero.ChampionName == "MasterYi")
            {
                if (ObjectManager.Player.HasBuff("doublestrike"))
                    dmg += dmg/2f;
            }

            if (passiveCheck)
            {
                if (AAEnpowers[ObjectManager.Player.GetID()] != null)
                    dmg +=
                        AAEnpowers[ObjectManager.Player.GetID()].Where(p => p.IsActive(hero, target))
                            .Sum(passive => passive.GetDamage(hero, target));

                dmg += ItemPassives.Where(p => Items.HasItem(p.ItemID)).Sum(passive => passive.GetDamage(hero, target));
            }

            return (float) Math.Floor(dmg) - 1f;
        }

        /// <summary>
        ///     Calculates mastery passives
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        private static float PassiveFlatMod(AIHeroClient source, Obj_AI_Base target)
        {
            var value = 0f;

            //Offensive masteries:

            //Fervor of Battle: STACKTIVATE Your basic attacks and spells give you stacks of Fervor for 5 seconds, stacking 10 times. Each stack of Fervor adds 1-8 bonus physical damage to your basic attacks against champions, based on your level.
            if (target.Type == GameObjectType.AIHeroClient)
            {
                var Fervor =
                    ObjectManager.Player.Masteries.FirstOrDefault(
                        p => p.Page == MasteryPage.Ferocity && p.Id == (int) Ferocity.FervorofBattle);
                if (Fervor != null && Fervor.Points >= 1)
                    value += (0.9f + source.Level*0.42f)*source.GetBuffCount("MasteryOnHitDamageStacker");
            }

            // Defensive masteries:

            //Tough Skin DIRT OFF YOUR SHOULDERS You take 2 less damage from champion and monster basic attacks
            if (target.Type == GameObjectType.AIHeroClient)
            {
                var Toughskin =
                    ObjectManager.Player.Masteries.FirstOrDefault(
                        p => p.Page == MasteryPage.Resolve && p.Id == (int) Resolve.ToughSkin);
                if (Toughskin != null && Toughskin.Points >= 1)
                    value -= 2;
            }

            return value;
        }

        /// <summary>
        ///     Calculates physical damage
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="amount">The amount</param>
        /// <returns></returns>
        private static float CalcPhysicalDamage(AIHeroClient source, Obj_AI_Base target, float amount)
        {
            var t = Environment.TickCount;
            var armorPenetrationPercent = source.PercentArmorPenetrationMod;
            var armorPenetrationFlat = source.FlatArmorPenetrationMod;
            var bonusArmorPenetrationMod = source.PercentBonusArmorPenetrationMod;
            // Penetration can't reduce armor below 0.

            float armor = 0;
            float bonusArmor = 0;


            t = Environment.TickCount;
            float value;
            if (armor < float.Epsilon)
            {
                value = 2f - 100f/(100f - armor);
            }
            else if (armor*armorPenetrationPercent - bonusArmor*(1f - bonusArmorPenetrationMod) - armorPenetrationFlat <
                     float.Epsilon)
            {
                value = 2f;
            }
            else
            {
                value = 100f/
                        (100f + armor*armorPenetrationPercent - bonusArmor*(1f - bonusArmorPenetrationMod) -
                         armorPenetrationFlat);
            }

            return amount;
        }

        private class Damage
        {
            public delegate float dGetDamage(AIHeroClient source, Obj_AI_Base target);

            public delegate bool dIsActive(AIHeroClient source, Obj_AI_Base target);

            public dGetDamage GetDamage;
            public dIsActive IsActive;
        }

        private class ItemPassive : Damage
        {
            public int ItemID;
        }

        private class AAEnpower : Damage
        {
            public string ChampionName = "";
        }

        private enum Ferocity
        {
            Fury = 65,
            Sorcery = 68,
            DoubleEdgedSword = 81,
            Vampirism = 97,
            NaturalTalent = 100,
            Feast = 82,
            BountyHunter = 113,
            Oppresor = 114,
            BatteringBlows = 129,
            PiercingThoughts = 132,
            WarlordsBloodlust = 145,
            FervorofBattle = 146,
            DeathFireTouch = 137
        }

        private enum Cunning
        {
            Wanderer = 65,
            Savagery = 66,
            RunicAffinity = 81,
            SecretStash = 82,
            Meditation = 98,
            Merciless = 97,
            Bandit = 114,
            DangerousGame = 115,
            Precision = 129,
            Intelligence = 130,
            StormraidersSurge = 145,
            ThunderlordsDecree = 146,
            WindspeakerBlessing = 147
        }

        private enum Resolve
        {
            Recovery = 65,
            Unyielding = 66,
            Explorer = 81,
            ToughSkin = 82,
            RunicArmor = 97,
            VeteransScar = 98,
            Insight = 113,
            Swiftness = 129,
            LegendaryGuardian = 130,
            GraspoftheUndying = 145,
            StrengthoftheAges = 146,
            BondofStones = 147
        }
    }
}