using System.Linq;
using AutoBuddy.MainLogics;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace AutoBuddy.MyChampLogic
{
    internal class Jinx : IChampLogic
    {

        public float MaxDistanceForAA { get { return int.MaxValue; } }
        public float OptimalMaxComboDistance { get { return AutoWalker.p.AttackRange; } }
        public float HarassDistance { get { return AutoWalker.p.AttackRange; } }

        public static Spell.Active Q;
        public static Spell.Skillshot W, E, R;

        public Jinx()
        {
            Q = new Spell.Active(SpellSlot.Q);
            W = new Spell.Skillshot(SpellSlot.W, 1500, SkillShotType.Linear, 500, 3300, 60)
            {
                AllowedCollisionCount = 0
            };
            E = new Spell.Skillshot(SpellSlot.E, 900, SkillShotType.Circular, 250, 1750, 315);
            R = new Spell.Skillshot(SpellSlot.R, 3000, SkillShotType.Linear, 500, 1500, 140);

            skillSequence = new[] { 1, 3, 2, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
            ShopSequence =
                "3340:Buy,2003:StartHpPot,1055:Buy,3086:Buy,3087:Buy,1001:Buy,1053:Buy,3144:Buy,1043:Buy,3153:Buy,1038:Buy,2003:StopHpPot,1037:Buy,3031:Buy,3006:Buy,1038:Buy,1055:Sell,1053:Buy,3072:Buy,3140:Buy,3139:Buy";
            Game.OnTick += Game_OnTick;
        }

        public int[] skillSequence { get; private set; }
        public LogicSelector Logic { get; set; }


        public string ShopSequence { get; private set; }

        public static float FishBonesBonus
        {
            get { return 75f + 25f * Q.Level; }
        }

        private void Game_OnTick(System.EventArgs args)
        {
            if (AutoWalker.p.HasBuff("JinxQ") && AutoWalker.p.CountEnemiesInRange(800) == 0)
                Q.Cast();
        }

        public void Harass(AIHeroClient target)
        {
            if (AutoWalker.p.ManaPercent >= 65)
            {
                if (W.IsReady() && target != null && target.IsValidTarget())
                {
                    var wPred = W.GetPrediction(target);
                    if (wPred != null && !wPred.Collision && wPred.HitChancePercent >= 70)
                        W.Cast(wPred.CastPosition);
                }
            }
        }

        public void Survi()
        {
            AIHeroClient chaser = EntityManager.Heroes.Enemies.FirstOrDefault(chase => chase.Distance(AutoWalker.p) < 600 && chase.IsVisible());
            if (chaser != null)
            {
                if (chaser != null && chaser.IsValidTarget())
                {
                    if (chaser.Distance(AutoWalker.p) <= AutoWalker.p.AttackRange - FishBonesBonus)
                    {
                        Q.Cast();
                    }

                    if (chaser.Distance(AutoWalker.p) > AutoWalker.p.AttackRange)
                    {
                        Q.Cast();
                    }
                }

                if (W.IsReady() && chaser != null && chaser.IsValidTarget())
                {
                    var wPred = W.GetPrediction(chaser);
                    if (wPred != null && !wPred.Collision && wPred.HitChancePercent >= 70)
                        W.Cast(wPred.CastPosition);
                }

                if (E.IsReady() && chaser != null && chaser.IsValidTarget())
                {
                    var ePred = E.GetPrediction(chaser);
                    if (ePred != null && ePred.HitChancePercent >= 70)
                        E.Cast(ePred.CastPosition);
                }
            }
        }

        public static float RDamage(Obj_AI_Base target)
        {

            if (!R.IsLearned)
                return 0;
            var level = R.Level - 1;

            if (target.Distance(AutoWalker.p) < 1350)
            {
                return AutoWalker.p.CalculateDamageOnUnit(target, DamageType.Physical,
                    (float)
                        (new double[] { 25, 35, 45 }[level] +
                         new double[] { 25, 30, 35 }[level] / 100 * (target.MaxHealth - target.Health) +
                         0.1 * AutoWalker.p.TotalAttackDamage));
            }

            return AutoWalker.p.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)
                    (new double[] { 250, 350, 450 }[level] +
                     new double[] { 25, 30, 35 }[level] / 100 * (target.MaxHealth - target.Health) +
                     1 * AutoWalker.p.TotalAttackDamage));
        }

        public void Combo(AIHeroClient target)
        {
            if (target != null && target.IsValidTarget())
            {
                if (target.Distance(AutoWalker.p) <= AutoWalker.p.AttackRange - FishBonesBonus)
                {
                    Q.Cast();
                }

                if (target.Distance(AutoWalker.p) > AutoWalker.p.AttackRange)
                {
                    Q.Cast();
                }
            }

            if (W.IsReady() && target != null && target.IsValidTarget())
            {
                var wPred = W.GetPrediction(target);
                if (wPred != null && !wPred.Collision && wPred.HitChancePercent >= 70)
                    W.Cast(wPred.CastPosition);
            }

            if (E.IsReady() && target != null && target.IsValidTarget())
            {
                var ePred = E.GetPrediction(target);
                if (ePred != null && ePred.HitChancePercent >= 70)
                    E.Cast(ePred.CastPosition);
            }

            foreach (var enemy in EntityManager.Heroes.Enemies)
            {
                if (R.IsReady() && enemy.Distance(AutoWalker.p) <= 1000 && RDamage(enemy) >= enemy.Health && !enemy.IsZombie && !enemy.IsDead)
                {
                    R.Cast(enemy);
                }
            }
        }
    }
}
