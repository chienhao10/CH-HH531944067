using System.Collections.Generic;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using TheBrand.Commons.ComboSystem;

namespace TheBrand
{
    internal class BrandCombo : ComboProvider
    {
        // ReSharper disable once InconsistentNaming

        public bool ForceAutoAttacks = false;

        public BrandCombo(IEnumerable<Skill> skills, float range) : base(range, skills)
        {
        }

        public BrandCombo(float range, params Skill[] skills) : base(range, skills)
        {
        }

        public override void Update()
        {
            if (!(ForceAutoAttacks && Orbwalker.IsAutoAttacking))
                base.Update();

            var target = TargetSelector.GetTarget(600, DamageType.True);
            if (target.IsValidTarget())
            {
                var passiveBuff = target.GetBuff("brandablaze");
                if (passiveBuff != null)
                {
                }
            }
        }

        public override bool ShouldBeDead(AIHeroClient target, float additionalSpellDamage = 0f)
        {
            var passive = target.GetBuff("brandablaze");
            return base.ShouldBeDead(target, passive != null ? GetRemainingPassiveDamage(target, passive) : 0f);
        }


        private float GetRemainingPassiveDamage(Obj_AI_Base target, BuffInstance passive)
        {
            return
                (float)
                    ObjectManager.Player.CalcDamage(target, DamageType.Magical,
                        ((int) (passive.EndTime - Game.Time) + 1)*target.MaxHealth*0.02f);
        }

        public static float GetPassiveDamage(AIHeroClient target)
        {
            return (float) ObjectManager.Player.CalcDamage(target, DamageType.Magical, target.MaxHealth*0.08);
        }
    }
}