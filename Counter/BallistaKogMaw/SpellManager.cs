using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace BallistaKogMaw
{
    internal class SpellManager
    {
        public static Spell.Active P { get; set; }
        public static Spell.Skillshot Q { get; set; }
        public static Spell.Active W { get; set; }
        public static Spell.Skillshot E { get; set; }
        public static Spell.Skillshot R { get; set; }

        // Clone Character Object
        public static AIHeroClient Champion = Program.Champion;

        // Tear Timestamp
        public static float StackerStamp = 0;

        public static void Initialize()
        {
            // Initialize spells
            P = new Spell.Active(SpellSlot.Unknown, 450);
            Q = new Spell.Skillshot(SpellSlot.Q, 1175, SkillShotType.Linear, 250, 1650, 70)
            {
                MinimumHitChance = HitChance.High,
                AllowedCollisionCount = 0
            };
            W = new Spell.Active(SpellSlot.W, (uint)Champion.GetAutoAttackRange());
            E = new Spell.Skillshot(SpellSlot.E, 1280, SkillShotType.Linear, 250, 1400, 120)
            {
                MinimumHitChance = HitChance.High,
                AllowedCollisionCount = int.MaxValue
            };
            R = new Spell.Skillshot(SpellSlot.R,
            (uint)(1200 + 300 * Champion.Spellbook.GetSpell(SpellSlot.R).Level),
            SkillShotType.Circular, 1200, int.MaxValue, 225)
            {
                MinimumHitChance = HitChance.High,
                AllowedCollisionCount = int.MaxValue
            };
        }

        public static void ConfigSpells(EventArgs args)
        {
            W = new Spell.Active(SpellSlot.W, (uint)Champion.GetAutoAttackRange());
            R = new Spell.Skillshot(SpellSlot.R,
            (uint)(900 + 300 * Champion.Spellbook.GetSpell(SpellSlot.R).Level),
            SkillShotType.Circular, 1200, int.MaxValue, 225)
            {
                MinimumHitChance = HitChance.High,
                AllowedCollisionCount = int.MaxValue
            };
        }

        // Champion Specified Abilities
        public static float PDamage()
        {
            return 100 + (25 * Champion.Level);
        }

        public static float QDamage()
        {
            return new float[] { 0, 80, 130, 180, 230, 280 }[Q.Level] + (0.5f * Champion.FlatMagicDamageMod);
        }

        public static float RDamage()
        {
            return new float[] { 0, 70, 110, 150 }[R.Level] 
                + (0.25f * Champion.FlatMagicDamageMod)
                + (0.65f * Champion.FlatPhysicalDamageMod);
        }
       
        public static float RMultiplier(Obj_AI_Base target)
        {
            float multiplier = 1;
            if (target.HealthPercent <= target.MaxHealth * 0.25f)
                multiplier = 3;
            else if (target.HealthPercent <= target.MaxHealth * 0.5f)
                multiplier = 2;

            return multiplier;
        }

        // Cast Methods
        public static void CastQ(Obj_AI_Base target)
        {
            if (target == null) return;
            if (Q.IsReady())
                Q.Cast(target);
        }

        public static void CastW(Obj_AI_Base target)
        {
            if (target == null) return;
            if (W.IsReady())
                W.Cast();
        }

        public static void CastE(Obj_AI_Base target)
        {
            if (target == null) return;
            if (E.IsReady())
                E.Cast(target);
        }

        public static void CastR(Obj_AI_Base target)
        {
            if (target == null) return;
            if (R.IsReady())
                R.Cast(target);
        }
    }
}
