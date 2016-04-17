namespace KappaUtility.Common
{
    using EloBuddy;
    using EloBuddy.SDK;

    using Items;

    internal class DamageHandler
    {
        internal static void OnLoad()
        {
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Obj_AI_Base.OnBasicAttack += OnBasicAttack;
        }

        public static void OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(args.Target is AIHeroClient))
            {
                return;
            }

            var caster = sender;
            var target = (AIHeroClient)args.Target;
            if ((caster is AIHeroClient || caster is Obj_AI_Turret) && caster.IsEnemy && target != null && target.IsAlly)
            {
                if (target.IsValidTarget(Defensive.FOTM.Range) && Defensive.FaceOfTheMountainc)
                {
                    if (target.HealthPercent <= Defensive.FaceOfTheMountainh || caster.GetAutoAttackDamage(target) >= target.TotalShieldHealth()
                        || args.SData.SpellDamageRatio >= target.HealthPercent)
                    {
                        Defensive.FOTM.Cast(target);
                    }
                }

                if (target.IsValidTarget(Defensive.Solari.Range) && Defensive.Solaric)
                {
                    if (target.HealthPercent <= Defensive.Solarih || caster.GetAutoAttackDamage(target) >= target.TotalShieldHealth()
                        || args.SData.SpellDamageRatio >= target.HealthPercent)
                    {
                        Defensive.Solari.Cast();
                    }
                }

                if (target.IsMe)
                {
                    if (Defensive.Seraphc)
                    {
                        if (target.HealthPercent <= Defensive.Seraphh || caster.GetAutoAttackDamage(target) >= target.TotalShieldHealth()
                            || args.SData.SpellDamageRatio >= target.HealthPercent)
                        {
                            Defensive.Seraph.Cast();
                        }
                    }

                    if (Defensive.Zhonyasc)
                    {
                        if (target.HealthPercent <= Defensive.Zhonyash || caster.GetAutoAttackDamage(target) >= target.TotalShieldHealth()
                            || args.SData.SpellDamageRatio >= target.HealthPercent)
                        {
                            Defensive.Zhonyas.Cast();
                        }
                    }
                }
            }
        }

        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(args.Target is AIHeroClient))
            {
                return;
            }

            var caster = sender;
            var target = (AIHeroClient)args.Target;

            if ((caster is AIHeroClient || caster is Obj_AI_Turret) && caster.IsEnemy && target != null && target.IsAlly)
            {
                if (target.IsValidTarget(Defensive.FOTM.Range))
                {
                    if ((Defensive.FaceOfTheMountainc) && target.HealthPercent <= Defensive.FaceOfTheMountainh
                        || caster.BaseAttackDamage >= target.TotalShieldHealth() || caster.BaseAbilityDamage >= target.TotalShieldHealth()
                        || args.SData.SpellDamageRatio >= target.HealthPercent)
                    {
                        Defensive.FOTM.Cast(target);
                    }
                }

                if (Defensive.Solaric && target.IsValidTarget(Defensive.Solari.Range))
                {
                    if (target.HealthPercent <= Defensive.Solarih || caster.BaseAttackDamage >= target.TotalShieldHealth()
                        || caster.BaseAbilityDamage >= target.TotalShieldHealth() || args.SData.SpellDamageRatio >= target.HealthPercent)
                    {
                        Defensive.Solari.Cast();
                    }
                }

                if (target.IsMe)
                {
                    if (Defensive.Seraphc)
                    {
                        if (target.HealthPercent <= Defensive.Seraphh || caster.BaseAttackDamage >= target.TotalShieldHealth()
                            || caster.BaseAbilityDamage >= target.TotalShieldHealth() || args.SData.SpellDamageRatio >= target.HealthPercent)
                        {
                            Defensive.Seraph.Cast();
                        }
                    }

                    if (Defensive.Zhonyasc)
                    {
                        if (target.HealthPercent <= Defensive.Zhonyash || caster.BaseAttackDamage >= target.TotalShieldHealth()
                            || caster.BaseAbilityDamage >= target.TotalShieldHealth())
                        {
                            Defensive.Zhonyas.Cast();
                        }

                        if (args.SData.SpellDamageRatio >= target.HealthPercent)
                        {
                            Defensive.Zhonyas.Cast();
                        }
                    }
                }
            }
        }
    }
}