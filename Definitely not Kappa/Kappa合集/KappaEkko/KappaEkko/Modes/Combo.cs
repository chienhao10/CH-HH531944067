namespace KappaEkko.Modes
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Menu.Values;

    using Logics;

    internal class Combo
    {
        public static void Start()
        {
            var Rk = Menu.ComboMenu["Rk"].Cast<CheckBox>().CurrentValue;
            var useQ = Menu.ComboMenu["Q"].Cast<CheckBox>().CurrentValue;
            var useW = Menu.ComboMenu["W"].Cast<CheckBox>().CurrentValue;
            var useWpred = Menu.ComboMenu["Wpred"].Cast<CheckBox>().CurrentValue;
            var Whit = Menu.ComboMenu["Whit"].Cast<Slider>().CurrentValue;
            var useE = Menu.ComboMenu["E"].Cast<CheckBox>().CurrentValue;
            var Emode = Menu.ComboMenu["Emode"].Cast<ComboBox>().CurrentValue;
            var useR = Menu.ComboMenu["R"].Cast<CheckBox>().CurrentValue;
            var Qtarget = TargetSelector.GetTarget(Spells.Q.Range, DamageType.Magical);
            var Wtarget = TargetSelector.GetTarget(Spells.W.Range, DamageType.Magical);

            if (Spells.R.IsReady() && Spells.EkkoREmitter != null)
            {
                if (useR)
                {
                    Rlogic.Combo();
                }

                if (Rk)
                {
                    Rlogic.Rk();
                }
            }

            if (Wtarget != null && Wtarget.IsValidTarget(Spells.W.Range) && Spells.W.IsReady())
            {
                if (useWpred)
                {
                    var pred = Spells.W.GetPrediction(Wtarget);
                    if (pred.HitChance >= HitChance.High && pred.CastPosition.CountEnemiesInRange(500) >= Whit)
                    {
                        Spells.W.Cast(pred.CastPosition);
                    }
                }

                if (useW && Wtarget.Position.CountEnemiesInRange(500) >= Whit)
                {
                    if (useWpred)
                    {
                        return;
                    }

                    Spells.W.Cast(Wtarget.Position);
                }
            }

            if (Qtarget != null && Qtarget.IsValidTarget(Spells.Q.Range))
            {
                if (useE && Spells.E.IsReady())
                {
                    if (Emode == 0)
                    {
                        if (Spells.E.Cast(Qtarget.Position))
                        {
                            Orbwalker.ResetAutoAttack();
                            Player.IssueOrder(GameObjectOrder.AttackUnit, Qtarget);
                            return;
                        }
                    }
                    else
                    {
                        if (Spells.E.Cast(Game.CursorPos))
                        {
                            Orbwalker.ResetAutoAttack();
                            Player.IssueOrder(GameObjectOrder.AttackUnit, Qtarget);
                            return;
                        }
                    }
                }

                if (useQ && Spells.Q.IsReady())
                {
                    var pred = Spells.Q.GetPrediction(Qtarget).CastPosition;
                    Spells.Q.Cast(pred);
                }
            }
        }
    }
}