using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using LeagueSharp.Common.Data;
using ItemData = LeagueSharp.Common.Data.ItemData;
using FioraProject.Evade;

namespace FioraProject
{
    using static Program;
    using static FioraPassive;
    using static GetTargets;
    using EloBuddy;
    using EloBuddy.SDK;
    public static class Combos
    {
        #region Clear

        #endregion Clear

        #region Harass
        public static void Harass()
        {
            //Qcast
            if (Q.IsReady() && Qharass && Player.ManaPercent >= Manaharass)
            {
                if (CastQPassiveHarasss || CastQPrePassiveHarass || CastQGapCloseHarass)
                {
                    if (TargetingMode == TargetMode.Normal)
                    {
                        foreach (var hero in HeroManager.Enemies.Where(x => x.LSIsValidTarget() && !x.IsZombie).OrderBy(x => x.LSDistance(Player.Position)))
                        {
                            var status = hero.GetPassiveStatus(0);
                            if (status.HasPassive
                                && !(Orbwalking.InAutoAttackRange(hero)
                                && status.PassivePredictedPositions.Any(x => Player.Position.LSTo2D()
                                    .InTheCone(status.TargetPredictedPosition, x, 90))))
                            {
                                if (CastQPassiveHarasss && status.PassiveType == PassiveType.UltiPassive
                                    && castQtoUltPassive(hero, getQPassivedelay(hero)))
                                    goto Wcast;
                                if (CastQPassiveHarasss && status.PassiveType == PassiveType.NormalPassive
                                    && castQtoPassive(hero, getQPassivedelay(hero)))
                                    goto Wcast;
                                if (CastQPrePassiveHarass && status.PassiveType == PassiveType.PrePassive
                                    && castQtoPrePassive(hero, getQPassivedelay(hero)))
                                    goto Wcast;
                                if (CastQGapCloseHarass
                                    && castQtoGapClose(hero, getQGapClosedelay(hero)))
                                    goto Wcast;
                            }
                        }
                    }
                    else
                    {
                        var hero = GetTarget();
                        if (hero != null)
                        {
                            var status = hero.GetPassiveStatus(0);
                            if (status.HasPassive
                                && !(Orbwalking.InAutoAttackRange(hero)
                                && status.PassivePredictedPositions.Any(x => Player.Position.LSTo2D()
                                    .InTheCone(status.TargetPredictedPosition, x, 90))))
                            {
                                if (CastQPassiveHarasss && status.PassiveType == PassiveType.UltiPassive
                                    && castQtoUltPassive(hero, getQPassivedelay(hero)))
                                    goto Wcast;
                                if (CastQPassiveHarasss && status.PassiveType == PassiveType.NormalPassive
                                    && castQtoPassive(hero, getQPassivedelay(hero)))
                                    goto Wcast;
                                if (CastQPrePassiveHarass && status.PassiveType == PassiveType.PrePassive
                                    && castQtoPrePassive(hero, getQPassivedelay(hero)))
                                    goto Wcast;
                                if (CastQGapCloseHarass
                                    && castQtoGapClose(hero, getQGapClosedelay(hero)))
                                    goto Wcast;
                            }
                        }
                    }
                }
                if (CastQGapCloseHarass && !CastQPassiveHarasss && !CastQPrePassiveHarass)
                {
                    if (TargetingMode == TargetMode.Normal)
                    {
                        foreach (var hero in HeroManager.Enemies.Where(x => x.LSIsValidTarget() && !x.IsZombie)
                            .OrderBy(x => x.LSDistance(Player.Position)))
                        {
                            if (castQtoGapClose(hero, getQGapClosedelay(hero)))
                                goto Wcast;
                        }
                    }
                    else
                    {
                        var hero = GetTarget();
                        if (hero != null)
                        {
                            if (castQtoGapClose(hero, getQGapClosedelay(hero)))
                                goto Wcast;
                        }
                    }
                }
            }

        Wcast:

            if (W.IsReady())
            {

            }
        }
        #endregion Harass

        #region Combo

        public static void Combo()
        {
            //Qcast
            if (Q.IsReady() && Qcombo)
            {
                if (CastQPassiveCombo || CastQPrePassiveCombo || CastQGapCloseCombo)
                {
                    if (TargetingMode == TargetMode.Normal)
                    {
                        foreach (var hero in HeroManager.Enemies.Where(x => x.LSIsValidTarget() && !x.IsZombie)
                            .OrderBy(x => x.LSDistance(Player.Position)))
                        {
                            var status = hero.GetPassiveStatus(0);
                            if (status.HasPassive
                                && !(Orbwalking.InAutoAttackRange(hero)
                                && status.PassivePredictedPositions.Any(x => Player.Position.LSTo2D()
                                    .InTheCone(status.TargetPredictedPosition, x, 90))))
                            {
                                if (CastQPassiveCombo && status.PassiveType == PassiveType.UltiPassive
                                    && castQtoUltPassive(hero, getQPassivedelay(hero)))
                                    goto Wcast;
                                if (CastQPassiveCombo && status.PassiveType == PassiveType.NormalPassive
                                    && castQtoPassive(hero, getQPassivedelay(hero)))
                                    goto Wcast;
                                if (CastQPrePassiveCombo && status.PassiveType == PassiveType.PrePassive
                                    && castQtoPrePassive(hero, getQPassivedelay(hero)))
                                    goto Wcast;
                                if (CastQGapCloseCombo
                                    && castQtoGapClose(hero, getQGapClosedelay(hero)))
                                    goto Wcast;
                            }
                        }
                    }
                    else
                    {
                        var hero = GetTarget();
                        if (hero != null)
                        {
                            var status = hero.GetPassiveStatus(0);
                            if (status.HasPassive
                                && !(Orbwalking.InAutoAttackRange(hero)
                                && status.PassivePredictedPositions.Any(x => Player.Position.LSTo2D()
                                    .InTheCone(status.TargetPredictedPosition, x, 90))))
                            {
                                if (CastQPassiveCombo && status.PassiveType == PassiveType.UltiPassive
                                    && castQtoUltPassive(hero, getQPassivedelay(hero)))
                                    goto Wcast;
                                if (CastQPassiveCombo && status.PassiveType == PassiveType.NormalPassive
                                    && castQtoPassive(hero, getQPassivedelay(hero)))
                                    goto Wcast;
                                if (CastQPrePassiveCombo && status.PassiveType == PassiveType.PrePassive
                                    && castQtoPrePassive(hero, getQPassivedelay(hero)))
                                    goto Wcast;
                                if (CastQGapCloseCombo
                                    && castQtoGapClose(hero, getQGapClosedelay(hero)))
                                    goto Wcast;
                            }
                        }
                    }
                }
                if (CastQGapCloseCombo && !CastQPassiveCombo && !CastQPrePassiveCombo)
                {
                    if (TargetingMode == TargetMode.Normal)
                    {
                        foreach (var hero in HeroManager.Enemies.Where(x => x.LSIsValidTarget() && !x.IsZombie)
                            .OrderBy(x => x.LSDistance(Player.Position)))
                        {
                            if (castQtoGapClose(hero, getQGapClosedelay(hero)))
                                goto Wcast;
                        }
                    }
                    else
                    {
                        var hero = GetTarget();
                        if (hero != null)
                        {
                            if (castQtoGapClose(hero, getQGapClosedelay(hero)))
                                goto Wcast;
                        }
                    }
                }
                if (CastQMinionGapCloseCombo && Math.Abs(Player.PercentCooldownMod) * 100 >= ValueQMinionGapCloseCombo)
                {
                    var hero = GetTarget();
                    if (hero != null && Player.Position.LSDistance(hero.Position) >= 500)
                    {
                        if (Player.Position.LSExtend(hero.Position, 400).CountMinionsInRange(300, false) >= 1)
                            Q.Cast(Player.Position.LSExtend(hero.Position, 400));
                    }
                }
            }

            if (R.IsReady() && Rcombo)
            {
                var hero = GetTarget();
                if (hero.LSIsValidTarget(500) && !hero.IsZombie)
                {
                    var status = hero.GetPassiveStatus(0);
                    if (!status.HasPassive || (status.HasPassive && !(Orbwalking.InAutoAttackRange(hero)
                         && status.PassivePredictedPositions.Any(x => Player.Position.LSTo2D()
                         .InTheCone(status.TargetPredictedPosition, x, 90)))))
                    {
                        if (UseRComboLowHP && Player.HealthPercent <= ValueRComboLowHP)
                        {
                            R.Cast(hero);
                            return;
                        }

                        if (UseRComboKillable && GetFastDamage(hero) >= hero.Health && hero.Health >= GetFastDamage(hero) / 3)
                        {
                            R.Cast(hero);
                            return;
                        }

                        if (UseRComboAlways)
                        {
                            R.Cast(hero);
                            return;
                        }
                    }
                    if (UseRComboOnTap && RTapKeyActive)
                    {
                        R.Cast(hero);
                        return;
                    }
                }
            }

            Wcast:

            if (W.IsReady())
            {

            }

        }
        #endregion Combo

        #region Damage
        public static AIHeroClient Player { get { return ObjectManager.Player; } }

        public static float GetPassiveDamage(AIHeroClient target)
        {
            return
                (0.03f + (0.027f + 0.001f * Player.Level) * Player.FlatPhysicalDamageMod / 100) * target.MaxHealth;
        }
        public static float GetUltiPassiveDamage(AIHeroClient target)
        {
            return GetPassiveDamage(target) * 4;
        }
        public static float GetUltiDamage(AIHeroClient target)
        {
            return GetUltiPassiveDamage(target) + (float)Player.LSGetAutoAttackDamage(target) * 4;
        }
        public static float GetFastDamage(AIHeroClient target)
        {
            //var statuss = target.GetPassiveStatus(0);
            //if (statuss.HasPassive)
            //{
            //    return statuss.PassivePredictedPositions.Count() * (float)(GetPassiveDamage(target) + Player.GetAutoAttackDamage(target));
            //}
            //return 0;
            ////
            float damage = 0;
            damage += Q.GetDamage(target);
            if (Q.IsReady())
                damage += Q.GetDamage(target);
            if (R.IsReady())
            {
                damage += GetUltiDamage(target);
                return damage;
            }
            else
            {
                var status = target.GetPassiveStatus(0);
                if (status.HasPassive)
                {
                    damage += status.PassivePredictedPositions.Count() * (float)(GetPassiveDamage(target) + Player.LSGetAutoAttackDamage(target));
                    if (status.PassivePredictedPositions.Count() < 3)
                        damage += (3 - status.PassivePredictedPositions.Count()) * (float)Player.LSGetAutoAttackDamage(target);
                    return damage;
                }
                else
                {
                    damage += (float)Player.LSGetAutoAttackDamage(target) * 2;
                    return damage;
                }
            }
        }
        #endregion Damage

        #region CastRHelper
        #endregion CastRHelper

        #region CastWHelper

        #endregion CastWHelper

        #region CastQHelper
        private static float getQPassivedelay(AIHeroClient target)
        {
            if (target == null)
                return 0;
            PassiveStatus targetStatus;
            if (LeagueSharp.Common.Prediction.GetPrediction(target, 0.25f).UnitPosition.LSTo2D().LSDistance(Player.Position.LSTo2D())
                > Player.Position.LSTo2D().LSDistance(target.Position.LSTo2D()))
                targetStatus = target.GetPassiveStatus(Player.Position.LSTo2D().LSDistance(target.Position.LSTo2D()) / 1100);
            else
                targetStatus = target.GetPassiveStatus(0);
            if (!targetStatus.HasPassive)
                return 0;
            if (targetStatus.PassiveType == PassiveType.PrePassive || targetStatus.PassiveType == PassiveType.NormalPassive)
            {
                if (!targetStatus.PassivePredictedPositions.Any())
                    return 0;
                var pos = targetStatus.PassivePredictedPositions.First();
                return Player.Position.LSTo2D().LSDistance(pos) / 1100 + Game.Ping / 1000;
            }
            if (targetStatus.PassiveType == PassiveType.UltiPassive)
            {
                if (!targetStatus.PassivePredictedPositions.Any())
                    return 0;
                var poses = targetStatus.PassivePredictedPositions;
                var pos = poses.OrderBy(x => Player.Position.LSTo2D().LSDistance(x)).First();
                return Player.Position.LSTo2D().LSDistance(pos) / 1100 + Game.Ping / 1000;
            }
            return 0;
        }
        private static float getQGapClosedelay(AIHeroClient target)
        {
            var distance = Player.LSDistance(target.Position);
            return
                distance > 400 ?
                400 / 1100 + Game.Ping / 1000 :
                distance / 1100 + Game.Ping / 1000;
        }
        private static bool castQtoGapClose(AIHeroClient target, float delay)
        {
            if (target == null)
                return false;
            var targetpredictedpos = LeagueSharp.Common.Prediction.GetPrediction(target, delay).UnitPosition.LSTo2D();
            var pos = Player.Position.LSTo2D().LSDistance(targetpredictedpos) > 400 ?
                Player.Position.LSTo2D().LSExtend(targetpredictedpos, 400) : targetpredictedpos;
            if (targetpredictedpos.LSDistance(pos) <= 300 && !pos.LSIsWall())
            {
                Q.Cast(pos);
                return true;
            }
            return false;
        }
        private static bool castQtoPrePassive(AIHeroClient target, float delay)
        {
            if (target == null)
                return false;
            var targetStatus = target.GetPassiveStatus(delay);
            if (targetStatus.PassiveType != PassiveType.PrePassive)
                return false;
            if (!targetStatus.PassivePredictedPositions.Any())
                return false;
            var passivepos = targetStatus.PassivePredictedPositions.First();
            var poses = GetRadiusPoints(targetStatus.TargetPredictedPosition, passivepos);
            var pos = poses.Where(x => x.LSDistance(Player.Position.LSTo2D()) <= 400 && !x.LSIsWall())
                           .OrderBy(x => x.LSDistance(passivepos)).FirstOrDefault();
            if (pos == null || !pos.IsValid())
                return false;
            Q.Cast(pos);
            return true;
        }
        private static bool castQtoPassive(AIHeroClient target, float delay)
        {
            if (target == null)
                return false;
            var targetStatus = target.GetPassiveStatus(delay);
            if (!targetStatus.HasPassive || targetStatus.PassiveType != PassiveType.NormalPassive)
                return false;
            if (targetStatus.PassiveType != PassiveType.NormalPassive)
                return false;
            if (!targetStatus.PassivePredictedPositions.Any())
                return false;
            var passivepos = targetStatus.PassivePredictedPositions.First();
            var poses = GetRadiusPoints(targetStatus.TargetPredictedPosition, passivepos);
            var pos = poses.Where(x => x.LSDistance(Player.Position.LSTo2D()) <= 400 && !x.LSIsWall()) .OrderBy(x => x.LSDistance(passivepos)).FirstOrDefault();
            if (pos == null || !pos.IsValid())
                return false;
            Q.Cast(pos);
            return true;
        }
        private static bool castQtoUltPassive(AIHeroClient target, float delay)
        {
            if (target == null)
                return false;
            var targetStatus = target.GetPassiveStatus(delay);
            if (!targetStatus.HasPassive || targetStatus.PassiveType != PassiveType.UltiPassive)
                return false;
            if (targetStatus.PassiveType != PassiveType.UltiPassive)
                return false;
            if (!targetStatus.PassivePredictedPositions.Any())
                return false;
            var passiveposes = targetStatus.PassivePredictedPositions;
            var passivepos = passiveposes.OrderBy(x => Player.Position.LSTo2D().LSDistance(x)).First();
            var poses = GetRadiusPoints(targetStatus.TargetPredictedPosition, passivepos);
            var pos = poses.Where(x => x.LSDistance(Player.Position.LSTo2D()) <= 400 && !x.LSIsWall())
                           .OrderBy(x => x.LSDistance(passivepos)).FirstOrDefault();
            if (pos == null || !pos.IsValid())
                return false;
            Q.Cast(pos);
            return true;
        }
        #endregion CastQHelper

        #region Item
        public static bool HasItem()
        {
            var tiamat = new Item(ItemId.Tiamat_Melee_Only);
            var hydra = new Item(ItemId.Ravenous_Hydra_Melee_Only);

            if (tiamat.IsReady() || hydra.IsReady() || Items.CanUseItem(3748))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static void CastItem()
        {
            var tiamat = new Item(ItemId.Tiamat_Melee_Only);
            var hydra = new Item(ItemId.Ravenous_Hydra_Melee_Only);

            if (tiamat.IsReady())
                tiamat.Cast();

            if (hydra.IsReady())
                hydra.Cast();
            
            if (Items.CanUseItem(3748))
                Items.UseItem(3748);
        }
        #endregion Item
    }
}
