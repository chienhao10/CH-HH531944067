using EloBuddy;
using EloBuddy.SDK;

namespace KA_Ezreal
{
    public static class SpellDamage
    {
        public static float GetTotalDamage(AIHeroClient target)
        {
            // Auto attack
            var damage = Player.Instance.GetAutoAttackDamage(target);

            // Q
            if (SpellManager.Q.IsReady())
            {
                damage += SpellManager.Q.GetRealDamage(target);
            }

            // W
            if (SpellManager.W.IsReady())
            {
                damage += SpellManager.W.GetRealDamage(target);
            }

            // E
            if (SpellManager.E.IsReady())
            {
                damage += SpellManager.E.GetRealDamage(target);
            }

            // R
            if (SpellManager.R.IsReady())
            {
                damage += SpellManager.R.GetRealDamage(target);
            }

            return damage;
        }

        public static float GetRealDamage(this Spell.SpellBase spell, Obj_AI_Base target)
        {
            return spell.Slot.GetRealDamage(target);
        }

        public static float GetRealDamage(this SpellSlot slot, Obj_AI_Base target)
        {
            // Helpers
            var spellLevel = Player.Instance.Spellbook.GetSpell(slot).Level;
            const DamageType damageType = DamageType.Magical;
            float damage = 0;

            // Validate spell level
            if (spellLevel == 0)
            {
                return 0;
            }
            spellLevel--;

            switch (slot)
            {
                case SpellSlot.Q:

                    var sheen = new Item(ItemId.Sheen);
                    var trinity = new Item(ItemId.Trinity_Force);
                    var ice = new Item(ItemId.Iceborn_Gauntlet);
                    if (sheen.IsOwned() && sheen.IsReady())
                    {
                        damage += new float[] { 35, 55, 75, 95, 115 }[spellLevel] + 0.40f * Player.Instance.FlatMagicDamageMod + 1.1f * Player.Instance.FlatPhysicalDamageMod + 1f * Player.Instance.FlatPhysicalDamageMod;
                    }
                    else if (sheen.IsOwned() && sheen.IsReady())
                    {
                        damage += new float[] { 35, 55, 75, 95, 115 }[spellLevel] + 0.40f * Player.Instance.FlatMagicDamageMod + 1.1f * Player.Instance.FlatPhysicalDamageMod + 1f * Player.Instance.FlatPhysicalDamageMod;
                    }
                    else if (trinity.IsOwned() && trinity.IsReady())
                    {
                        damage += new float[] { 35, 55, 75, 95, 115 }[spellLevel] + 0.40f * Player.Instance.FlatMagicDamageMod + 1.1f * Player.Instance.FlatPhysicalDamageMod + 2f * Player.Instance.FlatPhysicalDamageMod;
                    }
                    else if (ice.IsOwned() && ice.IsReady())
                    {
                        damage += new float[] { 35, 55, 75, 95, 115 }[spellLevel] + 0.40f * Player.Instance.FlatMagicDamageMod + 1.1f * Player.Instance.FlatPhysicalDamageMod + 1.25f * Player.Instance.FlatPhysicalDamageMod;
                    }
                    else
                    {
                        damage = new float[] { 35, 55, 75, 95, 115 }[spellLevel] + 0.40f * Player.Instance.FlatMagicDamageMod + 1.1f * Player.Instance.FlatPhysicalDamageMod;
                    }
                    break;

                case SpellSlot.W:

                    damage = new float[] { 70, 115, 160, 205, 250 }[spellLevel] + 0.80f * Player.Instance.FlatMagicDamageMod;
                    break;

                case SpellSlot.E:

                    damage = new float[] { 75, 125, 175, 225, 275 }[spellLevel] + 0.75f * Player.Instance.FlatMagicDamageMod + 0.5f * Player.Instance.FlatPhysicalDamageMod;
                    break;

                case SpellSlot.R:

                    damage = new float[] { 350, 500, 650 }[spellLevel] + 0.9f * Player.Instance.FlatMagicDamageMod + 1f * Player.Instance.FlatPhysicalDamageMod;
                    break;
            }

            if (damage <= 0)
            {
                return 0;
            }

            return Player.Instance.CalculateDamageOnUnit(target, damageType, damage) - 20;
        }
    }
}