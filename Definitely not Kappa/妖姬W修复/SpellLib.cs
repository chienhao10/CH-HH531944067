using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using SharpDX;
namespace KappaLeBlanc
{
    class Lib
    {
        public static Spell.Targeted Q;
        public static Spell.Skillshot W;
        public static Spell.Skillshot E;
        public static Spell.Targeted R;
        public static float QlasTick;

        static Lib()
        {
            Q = new Spell.Targeted(SpellSlot.Q, 700);
            W = new Spell.Skillshot(SpellSlot.W, 800, SkillShotType.Circular, 250, 1450, 250);
            E = new Spell.Skillshot(SpellSlot.E, 950, SkillShotType.Linear, 250, 1550, 55); { E.AllowedCollisionCount = 0; }
            R = new Spell.Targeted(SpellSlot.R, 950);
        }

        public static void CastW(Obj_AI_Base target)
        {
            if (Program.RavenForm) return;

            var wpred = W.GetPrediction(target);
            if (wpred.HitChance >= HitChance.Medium)
            {
                W.Cast(wpred.CastPosition);
            }
        }
        public static void CastW(Vector3 target)
        {
            if (!Program.RavenForm)
            {
                W.Cast(target);
            }
        }
        public static void CastR(Obj_AI_Base target)
        {
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancChaosOrbM") // Q
            {
                if (target.IsValidTarget(Q.Range))
                {
                    R.Cast(target);
                }
            }
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancSlideM") // W
            {
                if (target.IsValidTarget(W.Range))
                {
                    var wpred = W.GetPrediction(target).CastPosition;
                    R.Cast(wpred);
                }
            }
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancSoulShackleM") // E
            {
                if (target.IsValidTarget(E.Range))
                {
                    var epred = E.GetPrediction(target);
                    if (epred.HitChance >= HitChance.Medium)
                    {
                        R.Cast(epred.CastPosition);
                    }
                }
            }
        }
        public static void CastR(Vector3 target)
        {
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancSlideM") // W
            {
                R.Cast(target);
            }
        }
    }    
}