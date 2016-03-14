using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using SharpDX;

namespace ScaryKalista
{
    class Modes
    {
        public static void Combo()
        {
            if (Config.ComboMenu.IsChecked("combo.useQ")
                && Spells.Q.IsReady()
                && !Player.Instance.IsDashing()
                && Player.Instance.ManaPercent > Config.ComboMenu.GetValue("combo.minManaQ"))
            {
                var target = TargetSelector.GetTarget(Spells.Q.Range, DamageType.Physical);
                if (target != null && target.IsValidTarget())
                {
                    Spells.Q.Cast(target);
                }
            }

            if (Config.ComboMenu.IsChecked("combo.useE") 
                && !Config.MiscMenu.IsChecked("misc.killstealE"))
            {
                EnemyKill();
            }

            if (Config.ComboMenu.IsChecked("combo.gapClose"))
            {
                var gapCloseTarget = 
                    EntityManager.MinionsAndMonsters.CombinedAttackable
                        .FirstOrDefault(x => x.IsValidTarget(Player.Instance.GetAutoAttackRange()));

                Orbwalker.ForcedTarget = EntityManager.Heroes.Enemies.Any(x => Player.Instance.IsInAutoAttackRange(x)) ? null : gapCloseTarget;
            }

            if (Config.ComboMenu.IsChecked("combo.harassEnemyE"))
            {
                MinionHarass();
            }
        }

        public static void Harass()
        {
            if (Config.HarassMenu.IsChecked("harass.useQ")
                && Spells.Q.IsReady() 
                && !Player.Instance.IsDashing()
                && Player.Instance.ManaPercent > Config.HarassMenu.GetValue("harass.minManaQ"))
            {
                var target = TargetSelector.GetTarget(Spells.Q.Range, DamageType.Physical);
                if (target != null && target.IsValidTarget())
                {
                    Spells.Q.Cast(target);
                }
            }

            if (Config.HarassMenu.IsChecked("harass.harassEnemyE"))
            {
                MinionHarass();
            }
        }

        public static void LaneClear()
        {
            if (Player.HasBuff("summonerexhaust")) return;

            var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(
                EntityManager.UnitTeam.Enemy, Player.Instance.Position, Spells.Q.Range).ToArray();

            if (!minions.Any()) return;

            if (Config.LaneMenu.IsChecked("laneclear.useQ")
                && Player.Instance.ManaPercent > Config.LaneMenu.GetValue("laneclear.minManaQ"))
            {
                var qKillableMinions = minions.Where(x => x.GetTotalHealth() < Damages.GetQDamage(x)).ToArray();
                if (!qKillableMinions.Any()) return;

                var predictionResult =
                    (from minion in qKillableMinions
                     let pred = Spells.Q.GetPrediction(minion)
                     let count = pred.GetCollisionObjects<Obj_AI_Minion>().Count(x =>
                                    x.GetTotalHealth() < Damages.GetQDamage(x) 
                                    && x.IsValidTarget() && x.IsEnemy)
                     where count >= Config.LaneMenu.GetValue("laneclear.minQ")
                     where !Player.Instance.IsDashing()
                     select pred).FirstOrDefault();

                if (Spells.Q.IsReady() && predictionResult != null)
                {
                    Spells.Q.Cast(predictionResult.CastPosition);
                }
            }

            if (Config.LaneMenu.IsChecked("laneclear.useE")
                && Player.Instance.ManaPercent > Config.LaneMenu.GetValue("laneclear.minManaE") 
                && Spells.E.IsReady())
            {
                var count = minions.Count(x => x.IsRendKillable() && x.Health > 10);
                if (count >= Config.LaneMenu.GetValue("laneclear.minE"))
                {
                    Spells.E.Cast();
                }
            }

            if (Config.LaneMenu.IsChecked("laneclear.harassEnemyE"))
            {
                MinionHarass();
            }
        }

        public static void JungleClear()
        {
            if (Config.JungleMenu.IsChecked("jungleclear.useE")
                && !Config.MiscMenu.IsChecked("misc.junglestealE"))
            {
                JungleKill();
            }
        }

        public static void Flee()
        {
            if (Config.FleeMenu.IsChecked("flee.useJump") && Game.MapId == GameMapId.SummonersRift)
            {
                var spot = WallJump.GetJumpSpot();
                if (spot != null && Spells.Q.IsReady())
                {
                    Orbwalker.DisableAttacking = true;
                    Orbwalker.DisableMovement = true;

                    WallJump.JumpWall();
                    return;
                }
            }

            if(Config.FleeMenu.IsChecked("flee.attack"))
            {
                Orbwalker.DisableAttacking = false;
                Orbwalker.DisableMovement = false;

                var target =
                    ObjectManager.Get<Obj_AI_Base>()
                    .FirstOrDefault(
                        x => 
                            x.IsValidTarget(Player.Instance.GetAutoAttackRange())
                            && !x.IsMe 
                            && !x.IsAlly);

                Orbwalker.ForcedTarget = target;
            }
        }

        public static void PermaActive()
        {
            if (Config.MiscMenu.IsChecked("misc.killstealE"))
            {
                EnemyKill();
            }

            if (Config.MiscMenu.IsChecked("misc.junglestealE"))
            {
                JungleKill();
            }

            if (Config.MiscMenu.IsChecked("misc.useR") && Spells.R.IsReady())
            {
                if (Kalista.Soulbound == null) return;

                if (Kalista.Soulbound.HealthPercent <= Config.MiscMenu.GetValue("misc.healthR")
                    && Kalista.Soulbound.IsValidTarget(Spells.R.Range)
                    && Kalista.Soulbound.CountEnemiesInRange(500) > 0)
                {
                    Spells.R.Cast();
                }
            }

            if (Config.MiscMenu.IsChecked("misc.autoE") && Spells.E.IsReady())
            {
                if (Player.Instance.HealthPercent < Config.MiscMenu.GetValue("misc.autoEHealth") 
                    && EntityManager.Heroes.Enemies.Any(x => x.IsValidTarget(Spells.E.Range) && x.HasRendBuff()))
                {
                    Spells.E.Cast();
                }
            }
        }

        public static void OnUnkillableMinion(Obj_AI_Base unit, Orbwalker.UnkillableMinionArgs args)
        {
            if (!Spells.E.IsReady() 
                || Player.HasBuff("summonerexhaust") 
                || (Player.Instance.Mana - 40) < 40)
            {
                return;
            }

            if (Config.MiscMenu.IsChecked("misc.unkillableE") && unit.IsRendKillable())
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)
                    || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                {
                    Spells.E.Cast();
                }
            }
        }

        public static void CastW(Vector2 location)
        {
            if (!Spells.W.IsReady()) return;

            if (Player.Instance.Distance(location) <= Spells.W.Range)
            {
                Spells.W.Cast(location.To3DWorld());
            }
        }

        private static void MinionHarass()
        {
            if (!Spells.E.IsReady()
                || Player.HasBuff("summonerexhaust")
                || (Player.Instance.Mana - 40) < 40)
            {
                return;
            }

            var minion = EntityManager.MinionsAndMonsters.GetLaneMinions(
                            EntityManager.UnitTeam.Enemy, Player.Instance.Position, Spells.E.Range)
                            .Any(x => x.IsRendKillable() && x.Health > 10);

            if (!minion) return;

            var hero = EntityManager.Heroes.Enemies.Any(
                x =>
                    x.HasRendBuff()
                    && x.IsValidTarget(Spells.E.Range)
                    && !x.HasSpellShield()
                    && !x.HasUndyingBuff());

            if (hero) Spells.E.Cast();
        }

        private static void JungleKill()
        {
            if (!Spells.E.IsReady()) return;

            var minions = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, Spells.E.Range).ToArray();
            if (!minions.Any()) return;

            if (!Config.JungleMenu.IsChecked("jungleclear.miniE"))
            {
                if (minions.Any(x => x.IsRendKillable() && !x.Name.Contains("Mini")))
                {
                    Spells.E.Cast();
                }
            }

            else if (minions.Any(x => x.IsRendKillable()))
            {
                Spells.E.Cast();
            }
        }

        private static void EnemyKill()
        {
            if (!Spells.E.IsReady()) return;

            if (EntityManager.Heroes.Enemies.Any(x => x.IsRendKillable()))
            {
                Spells.E.Cast();
            }
        }
    }
}
