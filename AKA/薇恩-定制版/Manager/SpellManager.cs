using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace Auto_Carry_Vayne.Manager
{
    class SpellManager
    {
        public static Spell.Active Q;
        public static Spell.Skillshot Q2;
        public static Spell.Active W;
        public static Spell.Targeted E;
        public static Spell.Skillshot E2;
        public static Spell.Active R;
        public static Spell.Active Heal, Barrier;
        public static Item totem, Qss, Mercurial, HPPot, Biscuit, zzrot;

        private static void SpellsItems()
        {
            Q = new Spell.Active(SpellSlot.Q, 300);
            Q2 = new Spell.Skillshot(SpellSlot.Q, 300, SkillShotType.Linear);
            W = new Spell.Active(SpellSlot.W);
            E = new Spell.Targeted(SpellSlot.E, (uint)(650 + Variables._Player.BoundingRadius));
            E2 = new Spell.Skillshot(
                SpellSlot.E,
                (uint)(650 + Variables._Player.BoundingRadius),
                SkillShotType.Linear,
                250,
                1200);
            R = new Spell.Active(SpellSlot.R);
            var slot1 = Variables._Player.GetSpellSlotFromName("summonerheal");
            if (slot1 != SpellSlot.Unknown)
            {
                Heal = new Spell.Active(slot1, 600);
            }
            var slot2 = Variables._Player.GetSpellSlotFromName("summonerbarrier");
            if (slot2 != SpellSlot.Unknown)
            {
                Barrier = new Spell.Active(slot2);
            }
            totem = new Item((int)ItemId.Warding_Totem_Trinket);
            Qss = new Item((int)ItemId.Quicksilver_Sash);
            Mercurial = new Item((int)ItemId.Mercurial_Scimitar);
            HPPot = new Item(2003);
            Biscuit = new Item(2010);
            zzrot = new Item(ItemId.ZzRot_Portal, 400);
        }

        public static void Load()
        {
            SpellsItems();
        }
    }
}
