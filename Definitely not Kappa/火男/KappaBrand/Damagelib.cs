namespace KappaBrand
{
    using EloBuddy;
    using EloBuddy.SDK;

    internal static class Damagelib
    {
        private static readonly float[] Qdmg = { 80, 110, 140, 170, 200 };

        private static readonly float[] Wdmg = { 75, 120, 165, 210, 255 };

        private static readonly float[] WPdmg = { 93, 150, 205, 260, 320 };

        private static readonly float[] Edmg = { 80, 110, 140, 170, 200 };

        private static readonly float[] Rdmg = { 150, 300, 500 };

        public static float GetDamage(this Spell.SpellBase spell, Obj_AI_Base target)
        {
            float dmg = 0f;
            var AP = Player.Instance.TotalMagicalDamage;
            var slotLevel = Player.GetSpell(spell.Slot).Level - 1;

            switch (spell.Slot)
            {
                case SpellSlot.Q:
                    {
                        dmg += Qdmg[slotLevel] + 0.55f * AP;
                    }
                    break;
                case SpellSlot.W:
                    {
                        if (target.brandpassive())
                        {
                            dmg += WPdmg[slotLevel] + 0.75f * AP;
                        }
                        else
                        {
                            dmg += Wdmg[slotLevel] + 0.6f * AP;
                        }
                    }
                    break;
                case SpellSlot.E:
                    {
                        dmg += Edmg[slotLevel] + 0.35f * AP;
                    }
                    break;
                case SpellSlot.R:
                    {
                        dmg += Rdmg[slotLevel] + 0.5f * AP;
                    }
                    break;
            }
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical, dmg - 10);
        }

        public static float GetDamage(Obj_AI_Base target)
        {
            float dmg = 0f;
            var AP = Player.Instance.TotalMagicalDamage;

            if (Brand.Q.IsReady() && Common.Qmana)
            {
                dmg += Qdmg[Player.GetSpell(SpellSlot.Q).Level - 1] + 0.55f * AP;
            }

            if (Brand.W.IsReady() && Common.Wmana)
            {
                if (target.brandpassive())
                {
                    dmg += WPdmg[Player.GetSpell(SpellSlot.W).Level - 1] + 0.75f * AP;
                }
                else
                {
                    dmg += Wdmg[Player.GetSpell(SpellSlot.W).Level - 1] + 0.6f * AP;
                }
            }

            if (Brand.E.IsReady() && Common.Emana)
            {
                dmg += Edmg[Player.GetSpell(SpellSlot.E).Level - 1] + 0.35f * AP;
            }

            if (Brand.R.IsReady() && Common.Rmana)
            {
                dmg += Rdmg[Player.GetSpell(SpellSlot.R).Level - 1] + 0.5f * AP;
            }
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical, dmg - 25);
        }
    }
}