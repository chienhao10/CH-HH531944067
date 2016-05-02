namespace KappaEkko
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    internal class Spells
    {
        public static MissileClient EkkoQMissile;

        public static Spell.Skillshot E { get; private set; }

        public static Obj_GeneralParticleEmitter EkkoREmitter { get; set; }

        public static Spell.Skillshot Q { get; private set; }

        public static Spell.Active R { get; private set; }

        public static Spell.Skillshot W { get; private set; }

        public static void Load()
        {
            EkkoREmitter = ObjectManager.Get<Obj_GeneralParticleEmitter>().FirstOrDefault(x => x.Name.Equals("Ekko_Base_R_TrailEnd.troy"));
            Q = new Spell.Skillshot(SpellSlot.Q, 750, SkillShotType.Linear, 250, 2200, 60);
            W = new Spell.Skillshot(SpellSlot.W, 1600, SkillShotType.Circular, 1500, 500, 650);
            E = new Spell.Skillshot(SpellSlot.E, 450, SkillShotType.Linear, 250, int.MaxValue, 1);
            R = new Spell.Active(SpellSlot.R, 375);
        }
    }
}