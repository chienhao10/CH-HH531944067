namespace KappAzir.Utility
{
    using EloBuddy;
    using EloBuddy.SDK;

    public static class DamageLib
    {
        public static float GetDamage(this Spell.SpellBase spell, Obj_AI_Base target)
        {
            const DamageType damageType = DamageType.Magical;
            var AP = Player.Instance.TotalMagicalDamage;
            var sLevel = Player.GetSpell(spell.Slot).Level - 1;

            var dmg = 0f;

            switch (spell.Slot)
            {
                case SpellSlot.Q:
                    if (Azir.Q.IsReady())
                    {
                        dmg += new float[] { 65, 85, 105, 125, 145 }[sLevel] + 0.5f * AP;
                    }
                    break;
                case SpellSlot.W:
                    if (Azir.W.IsReady())
                    {
                        dmg += 50 + (10 * Player.Instance.Level) + 0.4f * AP;
                    }
                    break;
                case SpellSlot.E:
                    if (Azir.E.IsReady())
                    {
                        dmg += new float[] { 60, 90, 120, 150, 180 }[sLevel] + 0.4f * AP;
                    }
                    break;
                case SpellSlot.R:
                    if (Azir.R.IsReady())
                    {
                        dmg += new float[] { 150, 225, 300 }[sLevel] + 0.6f * AP;
                    }
                    break;
            }
            return Player.Instance.CalculateDamageOnUnit(target, damageType, dmg - 10);
        }

        public static float Damage(this Obj_AI_Base target)
        {
            const DamageType damageType = DamageType.Magical;
            var AP = Player.Instance.TotalMagicalDamage;

            var dmg = 0f;

            if (Azir.Q.IsReady())
            {
                dmg += new float[] { 65, 85, 105, 125, 145 }[Player.GetSpell(SpellSlot.Q).Level - 1] + 0.5f * AP;
            }
            if (Azir.W.IsReady())
            {
                dmg += 50 + (10 * Player.Instance.Level) + 0.4f * AP;
            }
            if (Azir.E.IsReady())
            {
                dmg += new float[] { 60, 90, 120, 150, 180 }[Player.GetSpell(SpellSlot.E).Level - 1] + 0.4f * AP;
            }
            if (Azir.R.IsReady())
            {
                dmg += new float[] { 150, 225, 300 }[Player.GetSpell(SpellSlot.R).Level - 1] + 0.6f * AP;
            }

            return Player.Instance.CalculateDamageOnUnit(target, damageType, dmg - 10);
        }
    }
}