namespace KappaKindred.Events
{
    using System;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;

    internal class OnPostAttack
    {
        public static void PostAttack(AttackableUnit target, EventArgs args)
        {
            var qtarget = TargetSelector.GetTarget(Spells.Q.Range, DamageType.True);
            var hero = target as AIHeroClient;
            var useQ = Menu.ComboMenu["Q"].Cast<CheckBox>().CurrentValue && Spells.Q.IsReady();
            var qmode = Menu.ComboMenu["Qmode"].Cast<ComboBox>().CurrentValue;

            if (qtarget == null || hero == null || hero.Type != GameObjectType.AIHeroClient || qtarget.HasBuff("kindredrnodeathbuff"))
            {
                return;
            }

            var flags = Orbwalker.ActiveModesFlags;
            if (flags.HasFlag(Orbwalker.ActiveModes.Combo) && useQ)
            {
                if (qmode == 0)
                {
                    if (Spells.Q.Cast(qtarget.Position))
                    {
                        Orbwalker.ResetAutoAttack();
                        Player.IssueOrder(GameObjectOrder.AttackUnit, qtarget);
                    }
                }
                else
                {
                    if (Spells.Q.Cast(Game.CursorPos))
                    {
                        Orbwalker.ResetAutoAttack();
                        Player.IssueOrder(GameObjectOrder.AttackUnit, qtarget);
                    }
                }
            }
        }
    }
}