using System.Collections.Generic;
using EloBuddy;
using EloBuddy.SDK;

namespace WuAIO.Bases
{
    class Damage
    {
        public readonly Spell.SpellBase spell;
        public readonly DamageType dealtDamageType;
        public readonly float[] baseDamage;
        public readonly List<Scale> scales;
        public readonly bool isAbility, isAAorTargeted, applyOnHitEffects;

        public Damage(Spell.SpellBase spell, DamageType dealtDamageType, float[] baseDamage, List<Scale> scales = null, bool isAbility = true, bool isAAorTargeted = false, bool applyOnHitEffects = false)
        {
            this.spell = spell;
            this.dealtDamageType = dealtDamageType;
            this.baseDamage = baseDamage;
            this.scales = scales;
            this.isAbility = isAbility;
            this.isAAorTargeted = isAAorTargeted;
            this.applyOnHitEffects = applyOnHitEffects;
        }
    }
}
