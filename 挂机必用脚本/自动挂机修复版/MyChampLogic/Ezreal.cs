using System.Linq;
using AutoBuddy.MainLogics;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace AutoBuddy.MyChampLogic
{
    internal class Ezreal : IChampLogic
    {
        public float MaxDistanceForAA { get { return int.MaxValue; } }
        public float OptimalMaxComboDistance { get { return AutoWalker.p.AttackRange; } }
        public float HarassDistance { get { return AutoWalker.p.AttackRange; } }


        private readonly Spell.Skillshot E;
        private readonly Spell.Skillshot Q;
        private readonly Spell.Skillshot R;
        private readonly Spell.Skillshot W;

        public Ezreal()
        {
            //                     1  2  3  4  5  6  7  8  9  10 11 12 13 14 15 16 17 18
            skillSequence = new[] {1, 3, 1, 2, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2};
            ShopSequence =
                "3340:Buy,1036:Buy,2003:StartHpPot,1053:Buy,1042:Buy,1001:Buy,3006:Buy,1036:Buy,1038:Buy,3072:Buy,2003:StopHpPot,1042:Buy,1051:Buy,3086:Buy,1042:Buy,1042:Buy,1043:Buy,3085:Buy,2015:Buy,3086:Buy,3094:Buy,1018:Buy,1038:Buy,3031:Buy,1037:Buy,3035:Buy,3033:Buy";

            Q = new Spell.Skillshot(SpellSlot.Q, 1160, SkillShotType.Linear, 350, 2000, 65)
            {
                MinimumHitChance = HitChance.High
            };
            W = new Spell.Skillshot(SpellSlot.W, 970, SkillShotType.Linear, 350, 1550, 80)
            {
                AllowedCollisionCount = int.MaxValue
            };
            E = new Spell.Skillshot(SpellSlot.E, 470, SkillShotType.Circular, 450, int.MaxValue, 10);
            R = new Spell.Skillshot(SpellSlot.R, 2000, SkillShotType.Linear, 1, 2000, 160)
            {
                MinimumHitChance = HitChance.High,
                AllowedCollisionCount = int.MaxValue
            };

            Game.OnTick += Game_OnTick;
        }

        /* Made By: MarioGK */
        public int[] skillSequence { get; private set; }
        public LogicSelector Logic { get; set; }
        public string ShopSequence { get; private set; }

        public void Harass(AIHeroClient target)
        {
            if (Q.IsReady() && target.IsValidTarget(Q.Range) && 20 <= Player.Instance.ManaPercent)
            {
                Q.Cast(target);
            }

            if (W.IsReady() && target.IsValidTarget(W.Range) && 30 <= Player.Instance.ManaPercent)
            {
                W.Cast(target);
            }
        }

        public void Survi()
        {
            if (E.IsReady() && Player.Instance.CountEnemiesInRange(800) <= 1)
            {
                E.Cast(Player.Instance.Position.Extend(AutoWalker.Target, E.Range).To3D());
            }
        }

        public void Combo(AIHeroClient target)
        {
            if (E.IsReady() && Player.Instance.CountEnemiesInRange(800) == 1 &&
                target.HealthPercent < Player.Instance.HealthPercent - 10)
            {
                E.Cast(Player.Instance.Position.Extend(target.Position, E.Range).To3D());
            }

            if (Q.IsReady() && target.IsValidTarget(Q.Range))
            {
                Q.Cast(target);
            }

            if (W.IsReady() && target.IsValidTarget(W.Range) && Player.Instance.ManaPercent > 40)
            {
                W.Cast(target);
            }

            if (R.IsReady() && Player.Instance.CountEnemiesInRange(W.Range) <= 1)
            {
                var hero =
                    EntityManager.Heroes.Enemies.OrderByDescending(e => e.Health)
                        .FirstOrDefault(
                            e => e.Health < Player.Instance.GetSpellDamage(e, SpellSlot.R) && e.IsValidTarget(2500));
                if (hero == null) return;

                R.Cast(hero);
            }
        }

        private void Game_OnTick(System.EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                var laneMinion =
                    EntityManager.MinionsAndMonsters.GetLaneMinions()
                        .OrderByDescending(m => m.Health)
                        .FirstOrDefault(
                            m => m.IsValidTarget(Q.Range) && m.Health <= Player.Instance.GetSpellDamage(m, SpellSlot.Q));
                if (laneMinion == null) return;

                if (Q.IsReady() && 40 <= Player.Instance.ManaPercent)
                {
                    Q.Cast(laneMinion);
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                {
                    var lastMinion =
                        EntityManager.MinionsAndMonsters.GetLaneMinions()
                            .OrderByDescending(m => m.Health)
                            .FirstOrDefault(
                                m =>
                                    m.IsValidTarget(Q.Range) &&
                                    !m.IsInRange(Player.Instance, Player.Instance.GetAutoAttackRange()) &&
                                    m.Health <= Player.Instance.GetSpellDamage(m, SpellSlot.Q));
                    if (lastMinion == null) return;

                    if (Q.IsReady() && 20 <= Player.Instance.ManaPercent)
                    {
                        Q.Cast(lastMinion);
                    }
                }
            }
        }
    }
}