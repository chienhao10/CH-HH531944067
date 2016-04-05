using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace Ex1L_Riven.Base
{
    internal class Spells
    {
        public static Spell.Active W, R1;
        public static Spell.Skillshot Q, E, R2, Flash;

        public static float R2Damage(AIHeroClient target, float health)
        {
            if (Riven.R1Activated && target != null && target.IsValidTarget(R2.Range))
            {
                var healthMis = target.MaxHealth - health/target.MaxHealth > 0.75
                    ? 0.75
                    : (target.MaxHealth - health)/target.MaxHealth;
                var percent = healthMis*2.67;
                var dmg = new double[] {80, 120, 160}[R1.Level - 1] + 0.6*Player.Instance.FlatPhysicalDamageMod;
                return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, (float) (dmg*(1 + percent)));
            }

            return 0;
        }

        public static float WDamage(Obj_AI_Base target)
        {
            if (target != null)
            {
                var dmg = new double[] {50, 80, 110, 140, 170}[W.Level - 1] + 1*Player.Instance.FlatPhysicalDamageMod;
                return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, (float) dmg);
            }

            return 0;
        }

        public static float QDamage(AIHeroClient target, float dmg = 0)
        {
            if (!Player.Instance.HasBuff("RivenFengShuiEngine"))
            {
                dmg =
                    (float)
                        (new double[] {10, 30, 50, 70, 90}[Q.Level - 1] +
                         new[] {0.4f, 0.45f, 0.5f, 0.55f, 0.6f}[Q.Level - 1]*Player.Instance.TotalAttackDamage);
            }
            else if (Player.Instance.HasBuff("RivenFengShuiEngine"))
            {
                dmg =
                    (float)
                        (new double[] {30, 90, 150, 210, 270}[Q.Level - 1] +
                         new[] {1.2f, 1.35f, 1.5f, 1.65f, 1.8f}[Q.Level - 1]*Player.Instance.TotalAttackDamage);
            }

            return dmg;
        }

        public static void Initialize()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, (uint) Player.Instance.Spellbook.GetSpell(SpellSlot.Q).SData.CastRange,
                SkillShotType.Linear)
            {
                AllowedCollisionCount = int.MaxValue
            };

            W = new Spell.Active(SpellSlot.W,
                70 + (uint) Player.Instance.BoundingRadius +
                (uint) (Player.Instance.HasBuff("RivenFengShuiEngine") ? 195 : 120));

            E = new Spell.Skillshot(SpellSlot.E, 325 + (uint) Player.Instance.AttackRange,
                SkillShotType.Linear)
            {
                AllowedCollisionCount = int.MaxValue
            };

            R1 = new Spell.Active(SpellSlot.R);

            R2 = new Spell.Skillshot(SpellSlot.R, 900, SkillShotType.Cone, 250, 1600, 125)
            {
                AllowedCollisionCount = int.MaxValue
            };

            var slot = Player.Instance.GetSpellSlotFromName("summonerflash");

            if (slot != SpellSlot.Unknown)
            {
                Flash = new Spell.Skillshot(slot, 425, SkillShotType.Linear);
            }
        }
    }
}