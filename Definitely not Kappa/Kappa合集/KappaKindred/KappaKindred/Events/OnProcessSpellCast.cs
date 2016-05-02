namespace KappaKindred.Events
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;

    internal class OnProcessSpellCast
    {
        public static void OnSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(args.Target is AIHeroClient))
            {
                return;
            }

            var caster = sender;
            var target = (AIHeroClient)args.Target;

            if ((!(caster is AIHeroClient) && !(caster is Obj_AI_Turret)) || caster == null || target == null)
            {
                return;
            }

            var Rally = Menu.UltMenu.Get<CheckBox>("Rally").CurrentValue && Spells.R.IsReady();
            var Rallyh = Menu.UltMenu.Get<Slider>("Rallyh").CurrentValue;

            if (!target.IsAlly || !target.IsMe || !caster.IsEnemy || caster.IsMinion || target.IsMinion
                || Menu.UltMenu["DontUlt" + target.BaseSkinName].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            if (!Player.Instance.IsInRange(target, Spells.R.Range))
            {
                return;
            }

            if (!target.IsValidTarget(Spells.R.Range) && Rally)
            {
                if (target.HealthPercent <= Rallyh)
                {
                    Spells.R.Cast();
                }

                if (caster.BaseAttackDamage >= target.TotalShieldHealth() || caster.BaseAbilityDamage >= target.TotalShieldHealth())
                {
                    Spells.R.Cast();
                }
            }
        }
    }
}