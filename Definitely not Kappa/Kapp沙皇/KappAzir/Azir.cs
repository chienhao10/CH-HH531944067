namespace KappAzir
{
    using System.Collections.Generic;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    public static class Azir
    {
        public static Spell.Skillshot Q;

        public static Spell.Skillshot W;

        public static Spell.Skillshot E;

        public static Spell.Skillshot R;

        public static Spell.Skillshot Flash;

        public static List<Spell.SpellBase> SpellList = new List<Spell.SpellBase>();

        public static void Execute()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 1000, SkillShotType.Linear, 250, 1000, 65) { AllowedCollisionCount = int.MaxValue };
            W = new Spell.Skillshot(SpellSlot.W, 525, SkillShotType.Circular);
            E = new Spell.Skillshot(SpellSlot.E, 1100, SkillShotType.Linear, 250, 1200, 80) { AllowedCollisionCount = int.MaxValue };
            R = new Spell.Skillshot(SpellSlot.R, 350, SkillShotType.Linear, 500, 1000, 220) { AllowedCollisionCount = int.MaxValue };

            if (Player.Spells.FirstOrDefault(o => o.SData.Name.Contains("SummonerFlash")) != null)
            {
                Flash = new Spell.Skillshot(Player.Instance.GetSpellSlotFromName("SummonerFlash"), 450, SkillShotType.Circular);
            }

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);
        }
    }
}