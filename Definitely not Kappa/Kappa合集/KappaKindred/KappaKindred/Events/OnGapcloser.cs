namespace KappaKindred.Events
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu.Values;

    internal class OnGapcloser
    {
        internal static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (e.End.Distance(Player.Instance.Position) <= Spells.Q.Range - 200 && Menu.FleeMenu["Qgap"].Cast<CheckBox>().CurrentValue
                && sender.IsEnemy)
            {
                Spells.Q.Cast(e.End.Extend(Player.Instance.Position, Player.Instance.Distance(e.End) + Spells.Q.Range).To3D());
            }
        }
    }
}