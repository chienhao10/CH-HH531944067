namespace KappaKindred.Modes
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;

    internal class Harass
    {
        public static void Start()
        {
            var useQ = Menu.HarassMenu["Q"].Cast<CheckBox>().CurrentValue && Spells.Q.IsReady();
            var useW = Menu.HarassMenu["W"].Cast<CheckBox>().CurrentValue && Spells.W.IsReady();
            var useE = Menu.HarassMenu["E"].Cast<CheckBox>().CurrentValue && Spells.E.IsReady();
            var target = TargetSelector.GetTarget(Spells.E.Range, DamageType.Physical);
            var qtarget = TargetSelector.GetTarget(Spells.Q.Range, DamageType.Physical);

            if (target != null)
            {
                if (useE)
                {
                    Spells.E.Cast(target);
                }

                if (useW)
                {
                    Spells.W.Cast();
                }
            }

            if (qtarget != null)
            {
                if (useQ)
                {
                    Spells.Q.Cast(qtarget.Position);
                }
            }
        }
    }
}