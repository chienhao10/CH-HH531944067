namespace KappaKindred.Events
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;

    internal class OnDamage
    {
        public static void Damage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
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

            if (!target.IsAlly || !target.IsMe || !caster.IsEnemy || target.IsEnemy
                || Menu.UltMenu["DontUlt" + target.BaseSkinName].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            if (!Player.Instance.IsInRange(target, Spells.R.Range))
            {
                return;
            }

            if (Rally && target.HealthPercent <= Rallyh)
            {
                Spells.R.Cast();
            }
        }
    }
}