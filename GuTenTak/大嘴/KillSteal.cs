using System.Linq;
using EloBuddy;
using EloBuddy.SDK;

namespace GuTenTak_KogMaw
{
    public static class KillSteal
    {
        private static AIHeroClient MyHero
        {
            get { return ObjectManager.Player; }
        }

        public static bool IsKillable(this AIHeroClient target, SpellSlot spell)
        {
            var totalHealth = target.TotalShieldHealth();
            if (target.HasUndyingBuff() || target.HasSpellShield() || target.IsInvulnerable)
                return false;
            if (target.ChampionName == "Blitzcrank" && !target.HasBuff("BlitzcrankManaBarrierCD") && !target.HasBuff("ManaBarrier"))
                totalHealth += target.Mana / 2;
            return (MyHero.GetSpellDamage(target, spell) >= totalHealth);
        }

        public static float TotalShieldHealth(this Obj_AI_Base target)
        {
            return target.Health + target.AllShield + target.AttackShield + target.MagicShield;
        }
        public static bool HasUndyingBuff(this AIHeroClient target)
        {
            if (target.Buffs.Any(
                b => b.IsValid() &&
                     (b.Name == "ChronoShift" /* Zilean R */||
                      b.Name == "FioraW" || /* Fiora Riposte */
                      b.Name == "BardRStasis" || /* Bard ult */
                      b.Name == "JudicatorIntervention" /* Kayle R */||
                      b.Name == "UndyingRage" /* Tryndamere R */)))
            {
                return true;
            }

            if (target.ChampionName == "Poppy")
            {
                if (EntityManager.Heroes.Allies.Any(o => !o.IsMe && o.Buffs.Any(b => b.Caster.NetworkId == target.NetworkId && b.IsValid() && b.DisplayName == "PoppyDITarget")))
                    return true;
            }

            return target.IsInvulnerable;
        }

        public static bool HasSpellShield(this AIHeroClient target)
        {
            return target.HasBuffOfType(BuffType.SpellShield) || target.HasBuffOfType(BuffType.SpellImmunity);
        }
    }
}