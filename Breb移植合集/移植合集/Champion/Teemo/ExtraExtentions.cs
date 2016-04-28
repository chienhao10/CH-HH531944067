using EloBuddy;
using EloBuddy.SDK;

namespace PortAIO.Champion.Teemo
{
    internal static class ExtraExtensions
    {
        internal static bool IsKillableAndValidTarget(this AIHeroClient target, double calculatedDamage,
            DamageType damageType, float distance = float.MaxValue)
        {
            if (target == null || !target.IsValidTarget(distance) || target.CharData.BaseSkinName == "gangplankbarrel")
                return false;

            if (target.HasBuff("kindredrnodeathbuff"))
            {
                return false;
            }

            // Tryndamere's Undying Rage (R)
            if (target.HasBuff("Undying Rage"))
            {
                return false;
            }

            // Kayle's Intervention (R)
            if (target.HasBuff("JudicatorIntervention"))
            {
                return false;
            }

            // Poppy's Diplomatic Immunity (R)
            if (target.HasBuff("DiplomaticImmunity") && !ObjectManager.Player.HasBuff("poppyulttargetmark"))
            {
                //TODO: Get the actual target mark buff name
                return false;
            }

            // Banshee's Veil (PASSIVE)
            if (target.HasBuff("BansheesVeil"))
            {
                // TODO: Get exact Banshee's Veil buff name.
                return false;
            }

            // Sivir's Spell Shield (E)
            if (target.HasBuff("SivirShield"))
            {
                // TODO: Get exact Sivir's Spell Shield buff name
                return false;
            }

            // Nocturne's Shroud of Darkness (W)
            if (target.HasBuff("ShroudofDarkness"))
            {
                // TODO: Get exact Nocturne's Shourd of Darkness buff name
                return false;
            }

            if (ObjectManager.Player.HasBuff("summonerexhaust"))
                calculatedDamage *= 0.6;

            if (target.ChampionName == "Blitzcrank")
                if (!target.HasBuff("manabarriercooldown"))
                    if (target.Health + target.HPRegenRate +
                        (damageType == DamageType.Physical ? target.AttackShield : target.MagicShield) +
                        target.Mana*0.6 + target.PARRegenRate < calculatedDamage)
                        return true;

            if (target.ChampionName == "Garen")
                if (target.HasBuff("GarenW"))
                    calculatedDamage *= 0.7;

            if (target.HasBuff("FerociousHowl"))
                calculatedDamage *= 0.3;

            return target.Health + target.HPRegenRate +
                   (damageType == DamageType.Physical ? target.AttackShield : target.MagicShield) <
                   calculatedDamage - 2;
        }

        internal static bool IsKillableAndValidTarget(this Obj_AI_Minion target, double calculatedDamage,
            DamageType damageType, float distance = float.MaxValue)
        {
            if (target == null || !target.IsValidTarget(distance) || target.Health <= 0 ||
                target.HasBuffOfType(BuffType.SpellImmunity) || target.HasBuffOfType(BuffType.SpellShield) ||
                target.CharData.BaseSkinName == "gangplankbarrel")
                return false;

            if (ObjectManager.Player.HasBuff("summonerexhaust"))
                calculatedDamage *= 0.6;

            var dragonSlayerBuff = ObjectManager.Player.GetBuff("s5test_dragonslayerbuff");
            if (dragonSlayerBuff != null)
            {
                if (dragonSlayerBuff.Count >= 4)
                    calculatedDamage += dragonSlayerBuff.Count == 5 ? calculatedDamage*0.30 : calculatedDamage*0.15;

                if (target.CharData.BaseSkinName.ToLowerInvariant().Contains("dragon"))
                    calculatedDamage *= 1 - dragonSlayerBuff.Count*0.07;
            }

            if (target.CharData.BaseSkinName.ToLowerInvariant().Contains("baron") &&
                ObjectManager.Player.HasBuff("barontarget"))
                calculatedDamage *= 0.5;

            return target.Health + target.HPRegenRate +
                   (damageType == DamageType.Physical ? target.AttackShield : target.MagicShield) <
                   calculatedDamage - 2;
        }

        internal static bool IsKillableAndValidTarget(this Obj_AI_Base target, double calculatedDamage,
            DamageType damageType, float distance = float.MaxValue)
        {
            if (target == null || !target.IsValidTarget(distance) || target.CharData.BaseSkinName == "gangplankbarrel")
                return false;

            if (target.HasBuff("kindredrnodeathbuff"))
            {
                return false;
            }

            // Tryndamere's Undying Rage (R)
            if (target.HasBuff("Undying Rage"))
            {
                return false;
            }

            // Kayle's Intervention (R)
            if (target.HasBuff("JudicatorIntervention"))
            {
                return false;
            }

            // Poppy's Diplomatic Immunity (R)
            if (target.HasBuff("DiplomaticImmunity") && !ObjectManager.Player.HasBuff("poppyulttargetmark"))
            {
                //TODO: Get the actual target mark buff name
                return false;
            }

            // Banshee's Veil (PASSIVE)
            if (target.HasBuff("BansheesVeil"))
            {
                // TODO: Get exact Banshee's Veil buff name.
                return false;
            }

            // Sivir's Spell Shield (E)
            if (target.HasBuff("SivirShield"))
            {
                // TODO: Get exact Sivir's Spell Shield buff name
                return false;
            }

            // Nocturne's Shroud of Darkness (W)
            if (target.HasBuff("ShroudofDarkness"))
            {
                // TODO: Get exact Nocturne's Shourd of Darkness buff name
                return false;
            }

            if (ObjectManager.Player.HasBuff("summonerexhaust"))
                calculatedDamage *= 0.6;

            if (target.CharData.BaseSkinName == "Blitzcrank")
                if (!target.HasBuff("manabarriercooldown"))
                    if (target.Health + target.HPRegenRate +
                        (damageType == DamageType.Physical ? target.AttackShield : target.MagicShield) +
                        target.Mana*0.6 + target.PARRegenRate < calculatedDamage)
                        return true;

            if (target.CharData.BaseSkinName == "Garen")
                if (target.HasBuff("GarenW"))
                    calculatedDamage *= 0.7;


            if (target.HasBuff("FerociousHowl"))
                calculatedDamage *= 0.3;

            var dragonSlayerBuff = ObjectManager.Player.GetBuff("s5test_dragonslayerbuff");
            if (dragonSlayerBuff != null)
                if (target.IsMinion)
                {
                    if (dragonSlayerBuff.Count >= 4)
                        calculatedDamage += dragonSlayerBuff.Count == 5 ? calculatedDamage*0.30 : calculatedDamage*0.15;

                    if (target.CharData.BaseSkinName.ToLowerInvariant().Contains("dragon"))
                        calculatedDamage *= 1 - dragonSlayerBuff.Count*0.07;
                }

            if (target.CharData.BaseSkinName.ToLowerInvariant().Contains("baron") &&
                ObjectManager.Player.HasBuff("barontarget"))
                calculatedDamage *= 0.5;

            return target.Health + target.HPRegenRate +
                   (damageType == DamageType.Physical ? target.AttackShield : target.MagicShield) <
                   calculatedDamage - 2;
        }
    }
}