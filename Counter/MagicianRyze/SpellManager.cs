using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace MagicianRyze
{
    internal class SpellManager
    {
        public static Spell.Skillshot Q { get; set; }
        public static Spell.Targeted W { get; set; }
        public static Spell.Targeted E { get; set; }
        public static Spell.Active R { get; set; }

        // Clone Character Object
        public static AIHeroClient Champion = Program.Champion;

        // Tear Timestamp
        public static float StackerStamp = 0;

        public static void Initialize()
        {
            // Initialize spells
            Q = new Spell.Skillshot(SpellSlot.Q, 900, SkillShotType.Linear, 250, 1700, 50)
            {
                MinimumHitChance = HitChance.High,
                AllowedCollisionCount = 0
            };
            W = new Spell.Targeted(SpellSlot.W, 600);
            E = new Spell.Targeted(SpellSlot.E, 600);
            R = new Spell.Active(SpellSlot.R);
        }

        // Champion Specified Abilities
        public static float QDamage()
        {
            return new float[] { 0, 60, 85, 110, 135, 160 }[Q.Level] 
                + (0.55f * Champion.FlatMagicDamageMod )
                + (new[] { 0f, 0.02f, 0.025f, 0.03f, 0.035f, 0.04f }[Q.Level] * Champion.MaxMana);
        }

        public static float WDamage()
        {
            return new float[] { 0, 80, 100, 120, 140, 160 }[W.Level]
                + (0.4f * Champion.FlatMagicDamageMod)
                + (0.025f * Champion.MaxMana);
        }

        public static float EDamage()
        {
            return new float[] { 0, 36, 52, 68, 84, 100 }[E.Level]
                + (0.2f * Champion.FlatMagicDamageMod)
                + (0.02f * Champion.MaxMana);
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
                W.Cast(target);
        }

        public static void CastE(Obj_AI_Base target)
        {
            if (target == null) return;
            if (E.IsReady())
                E.Cast(target);
        }

        public static void CastR(Obj_AI_Base target)
        {
            if (target != null && R.IsReady() && target.Health > QDamage() + EDamage())
            {
                if (MenuManager.ComboStun
                    && target.HasBuff("RyzeW"))
                    R.Cast();
                if (!MenuManager.ComboStun)
                    R.Cast();
            }
        }
    }
}
