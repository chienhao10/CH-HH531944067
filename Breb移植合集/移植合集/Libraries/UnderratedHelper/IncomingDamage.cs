using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using UnderratedAIO.Helpers.SkillShot;
using Utility = LeagueSharp.Common.Utility;

namespace UnderratedAIO.Helpers
{
    internal class IncomingDamage
    {
        public bool enabled, skillShotChecked;
        public float Globalreset;
        public List<IncData> IncomingDamagesAlly = new List<IncData>();
        public List<IncData> IncomingDamagesEnemy = new List<IncData>();

        public IncomingDamage()
        {
            Obj_AI_Base.OnProcessSpellCast += Game_ProcessSpell;
            Game.OnUpdate += Game_OnGameUpdate;
            // from H3h3 SpellDetector Lib
            SkillshotDetector.Init();
            foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(h => h.IsAlly))
            {
                IncomingDamagesAlly.Add(new IncData(ally));
            }
            foreach (var Enemy in ObjectManager.Get<AIHeroClient>().Where(h => h.IsEnemy))
            {
                IncomingDamagesEnemy.Add(new IncData(Enemy));
            }
            enabled = true;
        }

        //public List<Skillshot> SkillShots = new List<Skillshot>();

        public IncData GetAllyData(int networkId)
        {
            return IncomingDamagesAlly.FirstOrDefault(i => i.Hero.NetworkId == networkId);
        }

        public IncData GetEnemyData(int networkId)
        {
            return IncomingDamagesEnemy.FirstOrDefault(i => i.Hero.NetworkId == networkId);
        }

        public void Debug()
        {
            var data = IncomingDamagesAlly.Concat(IncomingDamagesEnemy);
            foreach (var d in data)
            {
                Console.WriteLine(d.Hero.Name);
                Console.WriteLine("\t DamageCount: " + d.DamageCount);
                Console.WriteLine("\t AADamageCount: " + d.AADamageCount);
                Console.WriteLine("\t DamageTaken: " + d.DamageTaken);
                Console.WriteLine("\t AADamageTaken: " + d.AADamageTaken);
                Console.WriteLine("\t TargetedCC: " + d.AnyCC);
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            try
            {
                //Remove the detected skillshots that have expired.
                SkillshotDetector.ActiveSkillshots.RemoveAll(s => !s.IsActive);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            if (ObjectManager.Player.IsDead)
            {
                resetAllData();
            }
            else
            {
                resetData();
                CheckSkillShots();
            }
            if (System.Environment.TickCount - Globalreset > 10000)
            {
                Globalreset = System.Environment.TickCount;
                resetAllData();
            }
        }


        private void CheckSkillShots()
        {
            if (!skillShotChecked)
            {
                skillShotChecked = true;
                Utility.DelayAction.Add(70, () => skillShotChecked = false);
                for (var i = 0; i < SkillshotDetector.ActiveSkillshots.Count; i++)
                {
                    if (
                        CombatHelper.BuffsList.Any(
                            b =>
                                SkillshotDetector.ActiveSkillshots[i].SkillshotData.Slot == b.Slot &&
                                ((AIHeroClient) SkillshotDetector.ActiveSkillshots[i].Caster).ChampionName ==
                                b.ChampionName))
                    {
                        SkillshotDetector.ActiveSkillshots.RemoveAt(i);
                        break;
                    }
                    foreach (var Hero in
                        HeroManager.AllHeroes.Where(
                            h =>
                                h.LSDistance(SkillshotDetector.ActiveSkillshots[i].Caster) <
                                SkillshotDetector.ActiveSkillshots[i].SkillshotData.Range)
                            .OrderBy(h => h.LSDistance(SkillshotDetector.ActiveSkillshots[i].Caster)))
                    {
                        SkillshotDetector.ActiveSkillshots[i].Game_OnGameUpdate();
                        if (SkillshotDetector.ActiveSkillshots[i].Caster.NetworkId != Hero.NetworkId &&
                            SkillshotDetector.ActiveSkillshots[i].Caster.Team != Hero.Team &&
                            !SkillshotDetector.ActiveSkillshots[i].IsSafePath(
                                0, -1, SkillshotDetector.ActiveSkillshots[i].SkillshotData.Delay, Hero).IsSafe &&
                            Hero.IsValidTarget(1500, false, Hero.Position) &&
                            SkillshotDetector.ActiveSkillshots[i].IsAboutToHit(
                                510, Hero, SkillshotDetector.ActiveSkillshots[i].Caster))
                        {
                            var data =
                                IncomingDamagesAlly.Concat(IncomingDamagesEnemy)
                                    .FirstOrDefault(h => h.Hero.NetworkId == Hero.NetworkId);
                            var missileSpeed =
                                SkillshotDetector.ActiveSkillshots[i].GetMissilePosition(0).LSDistance(Hero)/
                                SkillshotDetector.ActiveSkillshots[i].SkillshotData.MissileSpeed;
                            missileSpeed = missileSpeed > 5f ? 5f : missileSpeed;
                            var newData = new Dmg(
                                Hero,
                                (float)
                                    ((AIHeroClient) SkillshotDetector.ActiveSkillshots[i].Caster).LSGetSpellDamage(Hero,
                                        SkillshotDetector.ActiveSkillshots[i].SkillshotData.Slot), missileSpeed, false,
                                false, SkillshotDetector.ActiveSkillshots[i]);
                            if (data == null ||
                                data.Damages.Any(
                                    d =>
                                        d.SkillShot != null &&
                                        d.SkillShot.Caster.NetworkId ==
                                        SkillshotDetector.ActiveSkillshots[i].Caster.NetworkId &&
                                        d.SkillShot.SkillshotData.SpellName ==
                                        SkillshotDetector.ActiveSkillshots[i].SkillshotData.SpellName &&
                                        SkillshotDetector.ActiveSkillshots[i].SkillshotData.PossibleTargets.Any(
                                            h => h == Hero.NetworkId)))
                            {
                                continue;
                            }
                            if (data != null && data.Hero != Hero)
                            {
                                if (
                                    SkillshotDetector.ActiveSkillshots[i].SkillshotData.CollisionObjects.Count(
                                        c => c != CollisionObjectTypes.YasuoWall) > 0)
                                {
                                    /* Console.WriteLine(
                                        SkillshotDetector.ActiveSkillshots[i].SkillshotData.SpellName + " -> " +
                                        Hero.Name + " - " +
                                        SkillshotDetector.ActiveSkillshots[i].SkillshotData.IsDangerous);*/
                                    data.Damages.Add(newData);
                                    SkillshotDetector.ActiveSkillshots[i].SkillshotData.PossibleTargets.Add(
                                        Hero.NetworkId);
                                    SkillshotDetector.ActiveSkillshots.RemoveAt(i);
                                    break;
                                }
                                /*Console.WriteLine(
                                        SkillshotDetector.ActiveSkillshots[i].SkillshotData.SpellName + " --> " +
                                        Hero.Name + " - " +
                                        SkillshotDetector.ActiveSkillshots[i].SkillshotData.IsDangerous);*/
                                data.Damages.Add(newData);
                                SkillshotDetector.ActiveSkillshots[i].SkillshotData.PossibleTargets.Add(
                                    Hero.NetworkId);
                            }
                        }
                    }
                }
            }
        }

        private void resetData()
        {
            foreach (var incDamage in
                IncomingDamagesAlly.Concat(IncomingDamagesEnemy))
            {
                for (var index = 0; index < incDamage.Damages.Count; index++)
                {
                    var d = incDamage.Damages[index];
                    if (Game.Time - d.Time > d.delete)
                    {
                        incDamage.Damages.RemoveAt(index);
                    }
                }
            }
        }

        private void resetAllData()
        {
            foreach (var incDamage in
                IncomingDamagesAlly.Concat(IncomingDamagesEnemy))
            {
                incDamage.Damages.Clear();
            }
        }

        private void Game_ProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (enabled)
            {
                var target = args.Target as AIHeroClient;
                if (target != null && target.Team != sender.Team)
                {
                    if (sender.IsValid && !sender.IsDead)
                    {
                        var data =
                            IncomingDamagesAlly.Concat(IncomingDamagesEnemy)
                                .FirstOrDefault(i => i.Hero.NetworkId == target.NetworkId);
                        if (data != null)
                        {
                            var missileSpeed = sender.LSDistance(target)/args.SData.MissileSpeed +
                                               args.SData.SpellCastTime;
                            missileSpeed = missileSpeed > 1f ? 0.8f : missileSpeed;
                            if (Orbwalking.IsAutoAttack(args.SData.Name))
                            {
                                var dmg = sender.GetAutoAttackDamage(target, true);
                                data.Damages.Add(new Dmg(target, dmg, missileSpeed,
                                    !sender.Name.ToLower().Contains("turret")));
                            }
                            else
                            {
                                var hero = sender as AIHeroClient;
                                if (hero != null &&
                                    !CombatHelper.BuffsList.Any(
                                        b => args.Slot == b.Slot && hero.ChampionName == b.ChampionName))
                                {
                                    data.Damages.Add(
                                        new Dmg(
                                            target,
                                            (float) hero.LSGetSpellDamage((Obj_AI_Base) args.Target, args.Slot),
                                            missileSpeed, false,
                                            SpellDatabase.CcList.Any(
                                                cc =>
                                                    cc.Slot == args.Slot &&
                                                    cc.Champion.ChampionName == hero.ChampionName)));
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    internal class IncData
    {
        public List<Dmg> Damages = new List<Dmg>();
        public AIHeroClient Hero;

        public IncData(AIHeroClient _hero)
        {
            Hero = _hero;
        }


        public float DamageTaken
        {
            get
            {
                var dmg = Damages.Sum(d => d.Damage) + CombatHelper.BuffRemainingDamage(Hero);
                return dmg > 0 ? dmg + 20 : 0;
            }
        }

        public float ProjectileDamageTaken
        {
            get { return Damages.Sum(d => d.Damage); }
        }

        public bool IncSkillShot
        {
            get { return Damages.Count(d => d.SkillShot != null && d.Damage > 50) > 0; }
        }

        public float SkillShotDamage
        {
            get { return Damages.Where(d => d.SkillShot != null && d.Damage > 50).Sum(d => d.Damage); }
        }

        public float HealthPrediction
        {
            get { return Hero.Health - DamageTaken; }
        }

        public bool AnyCC
        {
            get { return AnyTargetedCC || AnySkillShotCC; }
        }

        public bool AnyTargetedCC
        {
            get { return Damages.Any(d => d.TargetedCC); }
        }

        public bool AnySkillShotCC
        {
            get { return Damages.Any(d => d.SkillShot != null && d.SkillShot.SkillshotData.IsDangerous); }
        }

        public float AADamageTaken
        {
            get { return Damages.Where(d => d.isAA).Sum(d => d.Damage); }
        }

        public float AADamageCount
        {
            get { return Damages.Count(d => d.isAA); }
        }

        public float DamageCount
        {
            get { return Damages.Count(); }
        }

        public bool IsAboutToDie
        {
            get
            {
                return Hero.Health < DamageTaken ||
                       CombatHelper.CheckCriticalBuffsNextSec(Hero) && !CombatHelper.IsInvulnerable2(Hero);
            }
        }
    }

    internal class Dmg
    {
        public float Damage;
        public float delete;
        public bool isAA;
        public Skillshot SkillShot;
        public AIHeroClient Target;
        public bool TargetedCC;
        public float Time;

        public Dmg(AIHeroClient target,
            float dmg,
            float delete,
            bool isAA = false,
            bool TargetedCC = false,
            Skillshot spell = null)
        {
            Target = target;
            Damage = dmg;
            Time = Game.Time;
            this.delete = delete;
            this.isAA = isAA;
            this.TargetedCC = TargetedCC;
            SkillShot = spell;
        }
    }
}