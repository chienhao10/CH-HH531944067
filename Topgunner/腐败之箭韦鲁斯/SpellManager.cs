using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;
using Settings = CorruptedVarus.Config.Misc;

namespace CorruptedVarus
{
    public static class SpellManager
    {
        // You will need to edit the types of spells you have for each champ as they
        // don't have the same type for each champ, for example Xerath Q is chargeable,
        // right now it's  set to Active.
        public static Spell.Skillshot Q2 { get; private set; }
        public static Spell.Chargeable Q { get; private set; }
        public static Spell.Skillshot E { get; private set; }
        public static Spell.Skillshot R { get; private set; }
        public static Spell.Active heal { get; private set; }
        public static Spell.Targeted ignite { get; private set; }
        public static bool isCharging = false;

        static SpellManager()
        {
            // Initialize spells
            Q2 = new Spell.Skillshot(SpellSlot.Q, 925, EloBuddy.SDK.Enumerations.SkillShotType.Linear, 0, 1900, 100);
            Q2.AllowedCollisionCount = int.MaxValue;
            Q = new Spell.Chargeable(SpellSlot.Q, 925, 1625, 2000, 0, 1900, 100);
            Q.OnSpellCasted += Q_OnSpellCasted;
            Q.AllowedCollisionCount = int.MaxValue;
            E = new Spell.Skillshot(SpellSlot.E, 925, SkillShotType.Circular, 500, int.MaxValue, 750);
            R = new Spell.Skillshot(SpellSlot.R, 1075, SkillShotType.Linear, 0, 1200, 120);
            var slot = ObjectManager.Player.GetSpellSlotFromName("summonerheal");
            if (slot != SpellSlot.Unknown)
            {
                heal = new Spell.Active(slot, 850);
            }
            var slot2 = ObjectManager.Player.GetSpellSlotFromName("summonerdot");
            if (slot2 != SpellSlot.Unknown)
            {
                ignite = new Spell.Targeted(slot2, 600);
            }

        }

        private static void Q_OnSpellCasted(Spell.SpellBase spell, GameObjectProcessSpellCastEventArgs args)
        {
            isCharging = false;
        }
        public static void Initialize()
        {
        }


    }
}
