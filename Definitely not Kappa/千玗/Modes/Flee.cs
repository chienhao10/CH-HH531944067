namespace KappaKindred.Modes
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;

    internal class Flee
    {
        public static void Start()
        {
            var useQ = Menu.FleeMenu["Q"].Cast<CheckBox>().CurrentValue && Spells.Q.IsReady();
            var qtarget = TargetSelector.GetTarget(Spells.Q.Range, DamageType.Physical);

            if (qtarget != null)
            {
                if (useQ)
                {
                    Spells.Q.Cast(Game.CursorPos);
                }
            }
        }
    }
}