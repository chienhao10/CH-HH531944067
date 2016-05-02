namespace KappaKindred.Modes
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;

    internal class Combo
    {
        public static void Start()
        {
            var useQ = Menu.ComboMenu["Q"].Cast<CheckBox>().CurrentValue && Spells.Q.IsReady();
            var QW = Menu.ComboMenu["QW"].Cast<CheckBox>().CurrentValue;
            var QAA = Menu.ComboMenu["QAA"].Cast<CheckBox>().CurrentValue;
            var qmode = Menu.ComboMenu["Qmode"].Cast<ComboBox>().CurrentValue;
            var useW = Menu.ComboMenu["W"].Cast<CheckBox>().CurrentValue && Spells.W.IsReady();
            var useE = Menu.ComboMenu["E"].Cast<CheckBox>().CurrentValue && Spells.E.IsReady();
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

            if (qtarget != null && useQ && qtarget.IsValidTarget(Spells.Q.Range))
            {
                if (Spells.W.Handle.ToggleState != 1 && QW)
                {
                    if (QAA && !qtarget.IsValidTarget(Player.Instance.GetAutoAttackRange()))
                    {
                        Spells.Q.Cast(qmode == 0 ? qtarget.Position : Game.CursorPos);
                    }

                    if (!QAA)
                    {
                        Spells.Q.Cast(qmode == 0 ? qtarget.Position : Game.CursorPos);
                    }
                }

                if (QAA && !qtarget.IsValidTarget(Player.Instance.GetAutoAttackRange()))
                {
                    Spells.Q.Cast(qmode == 0 ? qtarget.Position : Game.CursorPos);
                }

                if (!QAA && !QW)
                {
                    Spells.Q.Cast(qmode == 0 ? qtarget.Position : Game.CursorPos);
                }
            }
        }
    }
}