using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace OneForWeek.Util
{
    static class Spells
    {
        public static readonly Spell.Active Q;
        public static readonly Spell.Skillshot W;
        public static readonly Spell.Skillshot E;
        public static readonly Spell.Active R;
        public static int[] qDamage = { 75, 115, 155, 195, 235 };
        public static int[] wDamage = { 25, 35, 45, 55, 65 };
        public static int[] eDamage = { 50, 85, 120, 155, 190 };
        public static int[] rDamage = { 200, 325, 450 };

        static Spells()
        {
            Q = new Spell.Active(SpellSlot.Q, 150);
            W = new Spell.Skillshot(SpellSlot.W, 500, SkillShotType.Circular, 250, 2000, 100)
            {
                AllowedCollisionCount = -1
            };
            E = new Spell.Skillshot(SpellSlot.E, 1000, SkillShotType.Linear, 250, 1500, 140);
            R = new Spell.Active(SpellSlot.R);
        }

        public static float GetComboDamage(Obj_AI_Base enemy)
        {
            var damage = 0f;

            if (Q.IsReady())
                damage += QDamage(enemy);

            if (W.IsReady())
                damage += WDamage(enemy);

            if (E.IsReady())
                damage += EDamage(enemy);

            if (R.IsReady())
                damage += RDamage(enemy);

            return damage;
        }

        public static float QDamage(Obj_AI_Base target)
        {
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical,
                   qDamage[Q.Level - 1] + 0.27f * Player.Instance.FlatMagicDamageMod);
        }

        public static float WDamage(Obj_AI_Base target)
        {
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical,
                wDamage[W.Level - 1] + 0.15f * Player.Instance.FlatMagicDamageMod);
        }

        public static float EDamage(Obj_AI_Base target)
        {
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical,
                eDamage[E.Level - 1] + 0.55f * Player.Instance.FlatMagicDamageMod);
        }

        public static float RDamage(Obj_AI_Base target)
        {
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical,
                rDamage[R.Level - 1] + 0.6f * Player.Instance.FlatMagicDamageMod);
        }

    }
}
