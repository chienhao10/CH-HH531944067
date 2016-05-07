using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using IKalista;

namespace IKalista
{
    class Damages
    {
        public static readonly Damage.DamageSourceBoundle QDamage = new Damage.DamageSourceBoundle();

        private static readonly float[] RawRendDamage = { 20, 30, 40, 50, 60 };
        private static readonly float[] RawRendDamageMultiplier = { 0.6f, 0.6f, 0.6f, 0.6f, 0.6f };
        private static readonly float[] RawRendDamagePerSpear = { 10, 14, 19, 25, 32 };
        private static readonly float[] RawRendDamagePerSpearMultiplier = { 0.2f, 0.225f, 0.25f, 0.275f, 0.3f };

        static Damages()
        {
            QDamage.Add(new Damage.DamageSource(SpellSlot.Q, DamageType.Physical)
            {
                Damages = new float[] { 10, 70, 130, 190, 250 }
            });
            QDamage.Add(new Damage.BonusDamageSource(SpellSlot.Q, DamageType.Physical)
            {
                DamagePercentages = new float[] { 1, 1, 1, 1, 1 }
            });
        }
    }

    public static class Extensions
    {

        private static readonly float[] RawRendDamage = { 20, 30, 40, 50, 60 };
        private static readonly float[] RawRendDamageMultiplier = { 0.6f, 0.6f, 0.6f, 0.6f, 0.6f };
        private static readonly float[] RawRendDamagePerSpear = { 10, 14, 19, 25, 32 };
        private static readonly float[] RawRendDamagePerSpearMultiplier = { 0.2f, 0.225f, 0.25f, 0.275f, 0.3f };

        public static bool HasRendBuff(this Obj_AI_Base target)
        {
            return target.GetRendBuff() != null;
        }

        public static BuffInstance GetRendBuff(this Obj_AI_Base target)
        {
            return target.Buffs.Find(b => b.Caster.IsMe && b.IsValid() && b.DisplayName == "KalistaExpungeMarker");
        }

        public static bool IsRendKillable(Obj_AI_Base target, float? damage = null)
        {
            // Validate unit
            if (target == null || !target.IsValidTarget() || !target.HasRendBuff())
            {
                return false;
            }

            // Take into account all kinds of shields
            var totalHealth = target.TotalShieldHealth();

            var hero = target as AIHeroClient;
            if (hero != null)
            {
                // Validate that target has no undying buff or spellshield
                if (hero.HasUndyingBuff() || hero.HasSpellShield())
                {
                    return false;
                }

                // Take into account Blitzcranks passive
                if (hero.ChampionName == "Blitzcrank" && !target.HasBuff("BlitzcrankManaBarrierCD") && !target.HasBuff("ManaBarrier"))
                {
                    totalHealth += target.Mana / 2;
                }
            }

            return (damage ?? GetRendDamage(target)) > totalHealth;
        }

        public static float GetRendDamage(AIHeroClient target)
        {
            return GetRendDamage(target, -1);
        }

        public static float GetRendDamage(Obj_AI_Base target, int customStacks = -1, BuffInstance rendBuff = null)
        {
            // Calculate the damage and return
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, GetRawRendDamage(target, customStacks, rendBuff)) *
                   (Player.Instance.HasBuff("SummonerExhaustSlow") ? 0.6f : 1); // Take into account Exhaust, migh just add that to the SDK
        }

        public static float GetRawRendDamage(Obj_AI_Base target, int customStacks = -1, BuffInstance rendBuff = null)
        {
            rendBuff = rendBuff ?? target.GetRendBuff();
            var stacks = (customStacks > -1 ? customStacks : rendBuff != null ? rendBuff.Count : 0) - 1;
            if (stacks > -1)
            {
                var index = Player.Instance.Spellbook.GetSpell(SpellSlot.E).Level - 1;
                return RawRendDamage[index] + stacks * RawRendDamagePerSpear[index] + Player.Instance.TotalAttackDamage * (RawRendDamageMultiplier[index] + stacks * RawRendDamagePerSpearMultiplier[index]);
            }

            return 0;
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

    }
}