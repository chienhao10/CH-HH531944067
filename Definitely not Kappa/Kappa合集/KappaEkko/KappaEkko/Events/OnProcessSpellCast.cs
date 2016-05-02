namespace KappaEkko.Events
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;

    internal class OnProcessSpellCast
    {
        public static void OnSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsEnemy || sender.IsMe || sender is Obj_AI_Minion || !args.Target.IsMe || sender == null || args == null)
            {
                return;
            }

            var Rsave = Menu.UltMenu["Rsave"].Cast<CheckBox>().CurrentValue;
            var Rsaveh = Menu.UltMenu["Rsaveh"].Cast<Slider>().CurrentValue;
            var Health = ObjectManager.Player.HealthPercent;
            var caster = sender;
            var target = (AIHeroClient)args.Target;

            if (caster != null && target != null && caster.IsValid && target.IsMe && Rsave && Spells.R.IsReady()
                && ObjectManager.Player.CountEnemiesInRange(1000) >= 1)
            {
                if (Rsaveh >= Health)
                {
                    Spells.R.Cast();
                }
            }
        }
    }
}