namespace KappaEkko.Modes
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Menu.Values;

    internal class Harass
    {
        public static void Start()
        {
            var useQ = Menu.HarassMenu["Q"].Cast<CheckBox>().CurrentValue;
            var useW = Menu.HarassMenu["W"].Cast<CheckBox>().CurrentValue;
            var useE = Menu.HarassMenu["E"].Cast<CheckBox>().CurrentValue;
            var Qtarget = TargetSelector.GetTarget(Spells.Q.Range, DamageType.Magical);
            var Wtarget = TargetSelector.GetTarget(Spells.W.Range, DamageType.Magical);

            if (Wtarget != null && Wtarget.IsValidTarget(Spells.W.Range) && Spells.W.IsReady())
            {
                if (useW)
                {
                    Spells.W.Cast(Wtarget.Position);
                }
            }

            if (Qtarget != null && Qtarget.IsValidTarget(Spells.Q.Range))
            {
                if (useE && Spells.E.IsReady())
                {
                    Spells.E.Cast(Qtarget.Position);
                }

                if (useQ && Spells.Q.IsReady())
                {
                    var pred = Spells.Q.GetPrediction(Qtarget);
                    if (pred.HitChance >= HitChance.High)
                    {
                        Spells.Q.Cast(pred.CastPosition);
                    }
                }
            }
        }
    }
}