using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using IKalista;
using LeagueSharp.Common;

namespace IKalista
{

    public static class Extensions
    {

        private static readonly float[] RawRendDamage = { 20, 30, 40, 50, 60 };
        private static readonly float[] RawRendDamageMultiplier = { 0.6f, 0.6f, 0.6f, 0.6f, 0.6f };
        private static readonly float[] RawRendDamagePerSpear = { 10, 14, 19, 25, 32 };
        private static readonly float[] RawRendDamagePerSpearMultiplier = { 0.5f, 0.5f, 0.5f, 0.5f, 0.5f };

        public static BuffInstance GetRendBuff(this Obj_AI_Base target)
        {
            return target.Buffs.Find(b => b.Caster.IsMe && b.IsValid() && b.DisplayName == "KalistaExpungeMarker");
        }

        public static bool HasUndyingBuff(this Obj_AI_Base target1)
        {
            var target = target1 as AIHeroClient;

            if (target == null) return false;
            // Tryndamere R
            if (target.ChampionName == "Tryndamere"
                && target.Buffs.Any(
                    b => b.Caster.NetworkId == target.NetworkId && b.IsValid && b.DisplayName == "Undying Rage"))
            {
                return true;
            }

            // Zilean R
            if (target.Buffs.Any(b => b.IsValid && b.DisplayName == "Chrono Shift"))
            {
                return true;
            }

            if (target.HasBuff("kindredrnodeathbuff"))
            {
                return true;
            }

            // Kayle R
            if (target.Buffs.Any(b => b.IsValid && b.DisplayName == "JudicatorIntervention"))
            {
                return true;
            }

            //TODO poppy

            return false;
        }

        public static bool IsRendKillable(this Obj_AI_Base target)
        {
            if (target == null)
                return false;

            var baseDamage = Kalista.spells[SpellSlot.E].GetDamage(target);

            if (target is AIHeroClient)
            {
                if (target.HasUndyingBuff() || target.Health < 1 || target.HasBuffOfType(BuffType.SpellShield))
                    return false;

                if (target.HasBuff("meditate"))
                {
                    baseDamage *= (0.5f - 0.05f * target.Spellbook.GetSpell(SpellSlot.W).Level);
                }
            }

            if (target is Obj_AI_Minion)
            {
                if (target.Name.Contains("Baron") && ObjectManager.Player.HasBuff("barontarget"))
                {
                    baseDamage *= 0.5f;
                }
                //if (target.Name.Contains("Dragon") && ObjectManager.Player.HasBuff("s5test_dragonslayerbuff"))
                //{
                //    baseDamage *= (1f - (0.07f*ObjectManager.Player.GetBuffCount("s5test_dragonslayerbuff")));
                //} HM???
            }

            if (ObjectManager.Player.HasBuff("SummonerExhaustSlow"))
            {
                baseDamage *= 0.55f;
            }


            return (baseDamage - Kalista.getSliderItem(Kalista.miscMenu, "reduceE")) > target.GetHealthWithShield();
        }

        public static float GetHealthWithShield(this Obj_AI_Base target)
            => target.AttackShield > 0 ? target.Health + target.AttackShield : target.Health + 10;

        public static float GetRendDamage(AIHeroClient target)
        {
            return GetRendDamage(target, -1);
        }

        public static float GetRendDamage(Obj_AI_Base target, int customStacks = -1)
        {
            // Calculate the damage and return
            return (float)ObjectManager.Player.CalcDamage(target, DamageType.Physical, GetRawRendDamage(target, customStacks));
        }

        public static float GetRawRendDamage(Obj_AI_Base target, int customStacks = -1)
        {
            // Get buff
            var buff = target?.GetRendBuff();

            if (buff == null && customStacks <= -1) return 0;

            if (buff != null)
                return RawRendDamage[Player.Instance.Spellbook.GetSpell(SpellSlot.E).Level - 1]
                       + RawRendDamageMultiplier[Player.Instance.Spellbook.GetSpell(SpellSlot.E).Level - 1]
                       * ObjectManager.Player.TotalAttackDamage + // Base damage
                       ((customStacks < 0 ? buff.Count : customStacks) - 1) * // Spear count
                       (RawRendDamagePerSpear[Player.Instance.Spellbook.GetSpell(SpellSlot.E).Level - 1]
                        + RawRendDamagePerSpearMultiplier[Player.Instance.Spellbook.GetSpell(SpellSlot.E).Level - 1]
                        * ObjectManager.Player.TotalAttackDamage); // Damage per spear

            return 0;
        }

        public static float GetTotalHealth(this Obj_AI_Base target)
        {
            return target.Health + target.AllShield + target.AttackShield + target.MagicShield + (target.HPRegenRate * 2);
        }
    }
}