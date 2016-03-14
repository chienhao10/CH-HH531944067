using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace ScaryKalista
{
    public static class Extensions
    {
        public static bool HasRendBuff(this Obj_AI_Base target)
        {
            return target.GetRendBuff() != null;
        }

        public static BuffInstance GetRendBuff(this Obj_AI_Base target)
        {
            return target.Buffs.Find(b => b.Caster.IsMe && b.IsValid() && b.DisplayName == "KalistaExpungeMarker");
        }

        public static bool IsRendKillable(this Obj_AI_Base target)
        {
            if (target == null 
                || !target.IsValidTarget(Spells.E.Range + 200)
                || !target.HasRendBuff()
                || target.Health <= 0
                || !Spells.E.IsReady())
            {
                return false;
            }

            var hero = target as AIHeroClient;
            if (hero != null)
            {
                if (hero.HasUndyingBuff() || hero.HasSpellShield())
                {
                    return false;
                }

                if (hero.ChampionName == "Blitzcrank")
                {
                    if (!hero.HasBuff("BlitzcrankManaBarrierCD") && !hero.HasBuff("ManaBarrier"))
                    {
                        return Damages.GetActualDamage(target) > (target.GetTotalHealth() + (hero.Mana / 2));
                    }

                    if (hero.HasBuff("ManaBarrier") && !(hero.AllShield > 0))
                    {
                        return false;
                    }
                }
            }

            return Damages.GetActualDamage(target) > target.GetTotalHealth();
        }

        public static bool HasUndyingBuff(this AIHeroClient target)
        {
            // Tryndamere R
            if (target.Buffs.Any(b => b.IsValid() && b.DisplayName == "UndyingRage"))
            {
                return true;
            }

            // Zilean R
            if (target.Buffs.Any(b => b.IsValid() && b.DisplayName == "ChronoShift"))
            {
                return true;
            }

            // Kayle R
            if (target.Buffs.Any(b => b.IsValid() && b.DisplayName == "JudicatorIntervention"))
            {
                return true;
            }

            // Poppy R
            if (target.ChampionName == "Poppy")
            {
                if (
                    EntityManager.Heroes.Allies.Any(
                        o =>
                        !o.IsMe
                        && o.Buffs.Any(
                            b =>
                            b.Caster.NetworkId == target.NetworkId && b.IsValid()
                            && b.DisplayName == "PoppyDITarget")))
                {
                    return true;
                }
            }

            //Kindred R
            if (target.Buffs.Any(b => b.IsValid() && b.DisplayName == "kindredrnodeathbuff"))
            {
                return true;
            }

            if (target.HasBuffOfType(BuffType.Invulnerability))
            {
                return true;
            }

            return target.IsInvulnerable;
        }

        public static bool HasSpellShield(this AIHeroClient target)
        {
            //Banshee's Veil
            if (target.Buffs.Any(b => b.IsValid() && b.DisplayName == "bansheesveil"))
            {
                return true;
            }

            //Sivir E
            if (target.Buffs.Any(b => b.IsValid() && b.DisplayName == "SivirE"))
            {
                return true;
            }

            //Nocturne W
            if (target.Buffs.Any(b => b.IsValid() && b.DisplayName == "NocturneW"))
            {
                return true;
            }

            //Other spellshields
            return target.HasBuffOfType(BuffType.SpellShield) || target.HasBuffOfType(BuffType.SpellImmunity);
        }

        public static float GetTotalHealth(this Obj_AI_Base target)
        {
            return target.Health + target.AllShield + target.AttackShield + target.MagicShield + (target.HPRegenRate * 2);
        }

        public static bool IsChecked(this Menu menu, string id)
        {
            return menu.Get<CheckBox>(id).CurrentValue;
        }

        public static int GetValue(this Menu menu, string id)
        {
            return menu.Get<Slider>(id).CurrentValue;
        }

        public static bool IsActive(this Menu menu, string id)
        {
            return menu.Get<KeyBind>(id).CurrentValue;
        }
    }
}
