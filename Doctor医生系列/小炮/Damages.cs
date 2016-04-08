using EloBuddy;
using EloBuddy.SDK;

namespace Tristana
{
    class Damages
    {
        private static AIHeroClient _Player
        {
            get { return Player.Instance; }
        }

        private static float PlayerAP
        {
            get { return Player.Instance.TotalMagicalDamage; }
        }

        private static float PlayerAD
        {
            get { return Player.Instance.TotalAttackDamage; }
        }

        private static float PlayerBonusAD
        {
            get { return Player.Instance.TotalAttackDamage - Player.Instance.BaseAttackDamage; }
        }

        public static float WRawDamage()
        {
            return (new[] { 60.0f, 110.0f, 160.0f, 210.0f, 260.0f }[SpellManager.W.Level - 1]) + 0.5f * PlayerAP;
        }

        public static float WDamage(Obj_AI_Base target)
        {
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, WRawDamage()) *
                   (Player.Instance.HasBuff("SummonerExhaustSlow") ? 0.6f : 1);
        }
        
        public static float ERawDamage(Obj_AI_Base target)
        {
            var baseDmg = (new[] {60.0f, 70.0f, 80.0f, 90.0f, 100.0f}[SpellManager.E.Level - 1]) +
                      (new[] {0.5f, 0.65f, 0.8f, 0.95f, 1.1f}[SpellManager.E.Level - 1])*PlayerBonusAD + 0.5f*PlayerAP;
            var stackDmg = EStacks(target) *0.3f*baseDmg;
            return baseDmg + stackDmg;
        }

        public static float EDamage(Obj_AI_Base target)
        {
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, ERawDamage(target)) *
                   (Player.Instance.HasBuff("SummonerExhaustSlow") ? 0.6f : 1);
        }

        public static float RRawDamage()
        {
            return (new[] { 300.0f, 400.0f, 500.0f }[SpellManager.R.Level - 1]) + PlayerAP;
        }

        public static float RDamage(Obj_AI_Base target)
        {
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, RRawDamage()) *
                   (Player.Instance.HasBuff("SummonerExhaustSlow") ? 0.6f : 1);
        }

        public static float IgniteDmg(Obj_AI_Base target)
        {
            return Player.Instance.GetSummonerSpellDamage(target, DamageLibrary.SummonerSpells.Ignite);
        }

        public static int EStacks(Obj_AI_Base target)
        {
            var buff = target.GetBuff("TristanaECharge");
            if (buff == null)
            {
                return 0;
            }
            else
            {
                return buff.Count;
            }
        }
    }
}