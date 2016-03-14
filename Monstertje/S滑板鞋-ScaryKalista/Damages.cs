using EloBuddy;
using EloBuddy.SDK;

namespace ScaryKalista
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

        public static float GetQDamage(Obj_AI_Base target)
        {
            if (!Spells.Q.IsReady()) return 0f;

            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical,
                    new float[] { 10, 70, 130, 190, 250 }[Spells.Q.Level - 1]
                    + 1f * Player.Instance.TotalAttackDamage);
        }

        public static float GetRendDamage(Obj_AI_Base target)
        {
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, GetRawRendDamage(target)) *
                   (Player.Instance.HasBuff("summonerexhaust") ? 0.6f : 1);
        }

        public static float GetRawRendDamage(Obj_AI_Base target)
        {
            var stacks = (target.HasRendBuff() ?  target.GetRendBuff().Count : 0) - 1;
            if (stacks > -1)
            {
                var index = Spells.E.Level - 1;
                return RawRendDamage[index] + stacks * RawRendDamagePerSpear[index] +
                       Player.Instance.TotalAttackDamage * (RawRendDamageMultiplier[index] + stacks * RawRendDamagePerSpearMultiplier[index]);
            }

            return 0;
        }

        public static float GetActualDamage(Obj_AI_Base target)
        {
            if (!Spells.E.IsReady() || !target.HasRendBuff()) return 0f;

            var damage = GetRendDamage(target);

            if (target.Name.Contains("Baron"))
            {
                // Buff Name: barontarget or barondebuff
                // Baron's Gaze: Baron Nashor takes 50% reduced damage from champions he's damaged in the last 15 seconds. 
                damage = Player.Instance.HasBuff("barontarget")
                    ? damage * 0.5f
                    : damage;
            }

            else if (target.Name.Contains("Dragon"))
            {
                // DragonSlayer: Reduces damage dealt by 7% per a stack
                damage = Player.Instance.HasBuff("s5test_dragonslayerbuff")
                    ? damage * (1 - (.07f * Player.Instance.GetBuffCount("s5test_dragonslayerbuff")))
                    : damage;
            }

            if (Player.Instance.HasBuff("summonerexhaust"))
            {
                damage = damage * 0.6f;
            }

            if (target.HasBuff("FerociousHowl"))
            {
                damage = damage * 0.7f;
            }

            return damage;
        }
    }
}
