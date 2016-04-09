using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using EloBuddy.SDK.Events;


namespace AkaYasuo.Modes
{
    internal class Combo
    {
        public static void Load()
        {
            _combo();
            itemsandsums();
        }

        #region old

        public static void itemsandsums()
        {
            var TsTarget = TargetSelector.GetTarget(1300, DamageType.Physical);

            if (TsTarget == null)
            {
                return;
            }

            if (MenuManager.ItemMenu["Items"].Cast<CheckBox>().CurrentValue && TsTarget.IsValidTarget())
            {
                Items.UseItems(TsTarget);
            }
            if (Program.Ignite != null && Program.Ignite.IsReady() &&
                MenuManager.ComboMenu["Ignite"].Cast<CheckBox>().CurrentValue)
            {
                if (TsTarget.Distance(Variables._Player) <= (600) &&
                    Variables._Player.GetSummonerSpellDamage(TsTarget, DamageLibrary.SummonerSpells.Ignite) >=
                    TsTarget.Health)
                {
                    Program.Ignite.Cast(TsTarget);
                }
            }
        }
        
        #endregion old

        public static void _combo()
        {
            var TsTarget = TargetSelector.GetTarget(1300, DamageType.Physical);

            if (TsTarget == null)
            {
                return;
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (Program.R.IsReady() && MenuManager.ComboMenu["R"].Cast<CheckBox>().CurrentValue)
                {
                    List<AIHeroClient> enemies = EntityManager.Heroes.Enemies;
                    foreach (AIHeroClient enemy in enemies)
                    {
                        if (Variables._Player.Distance(enemy) <= 1200)
                        {
                            var enemiesKnockedUp =
                                ObjectManager.Get<AIHeroClient>()
                                    .Where(x => x.IsValidTarget(Program.R.Range))
                                    .Where(x => x.HasBuffOfType(BuffType.Knockup));

                            var enemiesKnocked = enemiesKnockedUp as IList<AIHeroClient> ??
                                                 enemiesKnockedUp.ToList();
                            if (enemy.IsValidTarget(Program.R.Range) &&
                                MenuManager.ComboMenu[TsTarget.ChampionName].Cast<CheckBox>().CurrentValue &&
                                Variables.CanCastDelayR(enemy) &&
                                enemiesKnocked.Count() >=
                                (MenuManager.ComboMenu["R3"].Cast<Slider>().CurrentValue))
                            {
                                Program.R.Cast();
                            }
                        }
                        if (enemy.IsValidTarget(Program.R.Range))
                        {
                            if (Variables.CanCastR(enemy) && Variables.CanCastDelayR(enemy) &&
                                (enemy.Health / enemy.MaxHealth * 100 <=
                                 (MenuManager.ComboMenu["R2"].Cast<Slider>().CurrentValue)))
                            {
                                Program.R.Cast();
                            }
                            else if (Variables.CanCastR(enemy) &&
                                     MenuManager.ComboMenu[TsTarget.ChampionName].Cast<CheckBox>().CurrentValue &&
                                     Variables.CanCastDelayR(enemy) &&
                                     enemy.HealthPercent >=
                                     (MenuManager.ComboMenu["R2"].Cast<Slider>().CurrentValue) &&
                                     (MenuManager.ComboMenu["R4"].Cast<CheckBox>().CurrentValue))
                            {
                                if (Variables.AlliesNearTarget(TsTarget, 600))
                                {
                                    Program.R.Cast();
                                }
                            }
                        }
                    }
                }

                if (MenuManager.ComboMenu["EC"].Cast<CheckBox>().CurrentValue && Program.E.IsReady())
                {
                    if (MenuManager.ComboMenu["EQ"].Cast<CheckBox>().CurrentValue && MenuManager.ComboMenu["Q"].Cast<CheckBox>().CurrentValue && Variables.HaveQ3 && Program.Q.IsReady(50))
                    {
                        var target = TargetSelector.GetTarget(Variables.QRange, DamageType.Physical);
                        if (target != null)
                        {
                            var obj = Variables.GetNearObj(target, true);
                            if (obj != null)
                            {
                                Program.E.Cast(obj);
                            }
                        }
                    }
                    if (MenuManager.ComboMenu["EGap"].Cast<CheckBox>().CurrentValue)
                    {
                        var target = TargetSelector.GetTarget(Variables.QRange, DamageType.Physical)
                                     ?? TargetSelector.GetTarget(Variables.Q2Range, DamageType.Physical);
                        if (target != null)
                        {
                            var obj = Variables.GetNearObj(target);
                            if (obj != null
                                && (obj.NetworkId != target.NetworkId
                                        ? Variables._Player.Distance(target) > MenuManager.ComboMenu["EGaps"].Cast<Slider>().CurrentValue
                                        : !Variables._Player.IsInAutoAttackRange(target))
                                && (!Variables.PosAfterE(obj).IsUnderTurret() || MenuManager.ComboMenu["EGapTower"].Cast<CheckBox>().CurrentValue))
                            {
                                Program.E.Cast(obj);
                            }
                        }
                    }
                }
            }

            if  (MenuManager.ComboMenu["Q"].Cast<CheckBox>().CurrentValue && Program.Q.IsReady())
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && ((!Variables.HaveQ3 || MenuManager.HarassMenu["Q3"].Cast<CheckBox>().CurrentValue))
                        && (!Variables._Player.IsUnderEnemyturret() || MenuManager.HarassMenu["QTower"].Cast<CheckBox>().CurrentValue))
                {
                    if (Variables._Player.IsDashing())
                    {
                        if (Variables.QCirTarget != null)
                        {
                             Variables.CastQCir(Variables.QCirTarget);
                        }
                        if (!Variables.HaveQ3 && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && MenuManager.ComboMenu["StackQ"].Cast<CheckBox>().CurrentValue && MenuManager.ComboMenu["EC"].Cast<CheckBox>().CurrentValue
                            && MenuManager.ComboMenu["EGap"].Cast<CheckBox>().CurrentValue)
                        {
                            var minionObj = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy,
                                Variables._Player.GetDashInfo().EndPos,
                                Variables.QCirWidth);
                            if (minionObj.Any() && Variables._Player.Distance(Variables._Player.GetDashInfo().EndPos) < 150)
                            {
                                Variables.CastQCir(minionObj.MinOrDefault(i => i.Distance(Variables._Player)));
                            }
                        }
                    }
                    else
                    {
                        var target = TargetSelector.GetTarget(
                            !Variables.HaveQ3 ? Variables.QRange : Variables.Q2Range,
                            DamageType.Physical);
                        if (target != null)
                        {
                            if (!Variables.HaveQ3)
                            {
                                Program.Q.Cast(target);
                            }
                            else if (Variables.HaveQ3)
                            {
                                var hit = -1;
                                var predPos = new Vector3();
                                foreach (var hero in EntityManager.Heroes.Enemies.Where(i => i.IsValidTarget(Variables.Q2Range)))
                                {
                                    var pred = Prediction.Position.PredictLinearMissile(hero, Variables.Q2Range, Program.Q2.Width, Program.Q2.CastDelay, Program.Q2.Speed, int.MaxValue, Variables._Player.ServerPosition, true);
                                    var pred2 = pred.GetCollisionObjects<AIHeroClient>();
                                    if (pred.HitChance >= EloBuddy.SDK.Enumerations.HitChance.High && pred2.Length > hit)
                                    {
                                        hit = pred2.Length;
                                        predPos = pred.CastPosition;
                                    }
                                }
                                if (predPos.IsValid())
                                {
                                    Program.Q2.Cast(predPos);
                                }
                                else
                                {
                                    Program.Q2.Cast(target.Position);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

