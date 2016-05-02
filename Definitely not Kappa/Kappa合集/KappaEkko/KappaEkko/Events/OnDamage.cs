namespace KappaEkko.Events
{
    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;

    internal class OnDamage
    {
        public static void Damage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            if (sender == null || sender.IsAlly || sender.IsMe)
            {
                return;
            }

            var Rsave = Menu.UltMenu["Rsave"].Cast<CheckBox>().CurrentValue;
            var Rsaveh = Menu.UltMenu["Rsaveh"].Cast<Slider>().CurrentValue;
            var Health = ObjectManager.Player.HealthPercent;

            if (Rsave)
            {
                if (sender.IsEnemy || sender is Obj_AI_Turret)
                {
                    if (Rsaveh >= Health)
                    {
                        Spells.R.Cast();
                    }
                }
            }
        }
    }
}