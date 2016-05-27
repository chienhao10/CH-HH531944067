using System;
using System.Linq;
using AutoBuddy.MainLogics;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using SharpDX;
using AutoBuddy.Utilities;

namespace AutoBuddy.MyChampLogic
{
    internal class Tristana : IChampLogic
    {
        public float MaxDistanceForAA { get { return int.MaxValue; } }
        public float OptimalMaxComboDistance { get { return AutoWalker.myHero.AttackRange; } }
        public float HarassDistance { get { return AutoWalker.myHero.AttackRange; } }

        private static Spell.Active Q;
        private static Spell.Skillshot W;
        private static Spell.Targeted E, R;

        public Tristana()
        {
            skillSequence = new[] { 3, 1, 2, 3, 3, 4, 3, 1, 3, 1, 4, 1, 1, 2, 2, 4, 2, 2 };
            ShopSequence =
                "3340:Buy,1036:Buy,2003:StartHpPot,1053:Buy,1042:Buy,1001:Buy,3006:Buy,1036:Buy,1038:Buy,3072:Buy,2003:StopHpPot,1042:Buy,1051:Buy,3086:Buy,1042:Buy,1042:Buy,1043:Buy,3085:Buy,2015:Buy,3086:Buy,3094:Buy,1018:Buy,1038:Buy,3031:Buy,1037:Buy,3035:Buy,3033:Buy";

            Q = new Spell.Active(SpellSlot.Q, 550);
            W = new Spell.Skillshot(SpellSlot.W, 900, SkillShotType.Circular, 450, int.MaxValue, 180);
            E = new Spell.Targeted(SpellSlot.E, 550);
            R = new Spell.Targeted(SpellSlot.R, 550);
        }

        public int[] skillSequence { get; private set; }
        public LogicSelector Logic { get; set; }

        public string ShopSequence { get; private set; }

        public void Harass(AIHeroClient target)
        {

        }

        public void Survi()
        {

        }

        public void Combo(AIHeroClient target)
        {
            if (target.HealthPercent <= 60)
            {

                if (target == null || target.IsZombie) return;

                if (E.IsReady() && target.IsValidTarget(E.Range))
                {
                    E.Cast(target);
                }

                if (Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    Q.Cast();
                }

                if (W.IsReady() && target.IsValidTarget(W.Range) &&
                                    !target.IsInvulnerable &&
                                    target.Position.CountEnemiesInRange(800) < 2)
                {
                    if (W.GetPrediction(target).HitChancePercent >= 70)
                    {
                        var optimizedCircleLocation = OKTRGeometry.GetOptimizedCircleLocation(W, target);
                        if (optimizedCircleLocation != null)
                            W.Cast(optimizedCircleLocation.Value.Position.To3D());
                    }
                }


                if (R.IsReady())
                {
                    if (R.IsReady() && target.IsValidTarget(R.Range) && !target.IsInvulnerable &&
                        target.Health < Player.Instance.GetSpellDamage(target, SpellSlot.R))
                    {
                        R.Cast(target);
                    }
                }
            }
        }
    }
}