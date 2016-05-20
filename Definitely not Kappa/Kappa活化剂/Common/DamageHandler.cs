namespace KappaUtility.Common
{
    using System;
    using System.Threading;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;

    using Items;

    using Summoners;

    using SharpDX;

    internal class DamageHandler
    {
        internal static void OnLoad()
        {
            try
            {
                Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
                Obj_AI_Base.OnBasicAttack += Obj_AI_Base_OnBasicAttack;
            }
            catch (Exception e)
            {
                Helpers.Log(e.ToString());
            }
        }

        private static void Obj_AI_Base_OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            try
            {
                if (!(args.Target is AIHeroClient))
                {
                    return;
                }

                var caster = sender;
                var target = (AIHeroClient)args.Target;

                if (!(caster is AIHeroClient || caster is Obj_AI_Turret) || !caster.IsEnemy || target == null || caster == null || !target.IsAlly
                    || !target.IsKillable())
                {
                    return;
                }

                var aaprecent = (caster.GetAutoAttackDamage(target, true) / target.TotalShieldHealth()) * 100;
                var death = caster.GetAutoAttackDamage(target, true) >= target.TotalShieldHealth() || aaprecent >= target.HealthPercent;

                if (target.IsAlly && !target.IsMe)
                {
                    if (Spells.Exhaust != null)
                    {
                        var exhaustc = (Spells.SummMenu["EnableactiveExhaust"].Cast<KeyBind>().CurrentValue
                                        || Spells.SummMenu["EnableExhaust"].Cast<KeyBind>().CurrentValue) && Spells.Exhaust.IsReady();
                        var Exhaustally = Spells.SummMenu["exhaustally"].Cast<Slider>().CurrentValue;
                        var Exhaustenemy = Spells.SummMenu["exhaustenemy"].Cast<Slider>().CurrentValue;

                        if (exhaustc
                            && (target.IsValidTarget(Spells.Exhaust.Range)
                                && !Spells.SummMenu["DontExhaust" + caster.BaseSkinName].Cast<CheckBox>().CurrentValue))
                        {
                            if (target.HealthPercent <= Exhaustenemy || target.HealthPercent <= Exhaustally || death)
                            {
                                Spells.Exhaust.Cast(caster);
                            }
                        }
                    }

                    if (Spells.Heal != null && !Spells.SummMenu["DontHeal" + target.BaseSkinName].Cast<CheckBox>().CurrentValue)
                    {
                        var healc = (Spells.SummMenu["EnableactiveHeal"].Cast<KeyBind>().CurrentValue
                                     || Spells.SummMenu["EnableHeal"].Cast<KeyBind>().CurrentValue) && Spells.Heal.IsReady();
                        var healally = Spells.SummMenu["Healally"].Cast<Slider>().CurrentValue;
                        if (healc)
                        {
                            if (target.IsInRange(Player.Instance, Spells.Heal.Range))
                            {
                                if (target.HealthPercent <= healally || death)
                                {
                                    Spells.Heal.Cast();
                                }
                            }
                        }
                    }
                }

                if (target.IsValidTarget(Defensive.FOTM.Range) && Defensive.FaceOfTheMountainc)
                {
                    if (Defensive.FaceOfTheMountainh >= target.HealthPercent || death || aaprecent >= Defensive.FaceOfTheMountainn)
                    {
                        Defensive.FOTM.Cast(target);
                    }
                }

                if (target.IsValidTarget(Defensive.Solari.Range) && Defensive.Solaric)
                {
                    if (Defensive.Solarih >= target.HealthPercent || death || aaprecent >= Defensive.Solarin)
                    {
                        Defensive.Solari.Cast();
                    }
                }

                if (target.IsMe)
                {
                    if (Defensive.Seraphc)
                    {
                        if (Defensive.Seraphh >= target.HealthPercent || death || aaprecent >= Defensive.Seraphn)
                        {
                            Defensive.Seraph.Cast();
                        }
                    }

                    if (Defensive.Zhonyasc)
                    {
                        if (Defensive.Zhonyash >= target.HealthPercent || death || aaprecent >= Defensive.Zhonyasn)
                        {
                            Defensive.Zhonyas.Cast();
                        }
                    }

                    if (Spells.Heal != null && !Spells.SummMenu["DontHeal" + target.BaseSkinName].Cast<CheckBox>().CurrentValue)
                    {
                        var healc = (Spells.SummMenu["EnableactiveHeal"].Cast<KeyBind>().CurrentValue
                                     || Spells.SummMenu["EnableHeal"].Cast<KeyBind>().CurrentValue) && Spells.Heal.IsReady();
                        var healme = Spells.SummMenu["Healme"].Cast<Slider>().CurrentValue;
                        if (healc)
                        {
                            if (target.HealthPercent <= healme || death)
                            {
                                Spells.Heal.Cast();
                            }
                        }
                    }

                    if (Spells.Exhaust != null)
                    {
                        var exhaustc = (Spells.SummMenu["EnableactiveExhaust"].Cast<KeyBind>().CurrentValue
                                        || Spells.SummMenu["EnableExhaust"].Cast<KeyBind>().CurrentValue) && Spells.Exhaust.IsReady();
                        var Exhaustally = Spells.SummMenu["exhaustally"].Cast<Slider>().CurrentValue;
                        var Exhaustenemy = Spells.SummMenu["exhaustenemy"].Cast<Slider>().CurrentValue;
                        if (exhaustc && !Spells.SummMenu["DontExhaust" + caster.BaseSkinName].Cast<CheckBox>().CurrentValue)
                        {
                            if (target.HealthPercent <= Exhaustenemy || target.HealthPercent <= Exhaustally || death)
                            {
                                Spells.Exhaust.Cast(caster);
                            }
                        }
                    }

                    if (Spells.Barrier != null)
                    {
                        var barrierc = (Spells.SummMenu["EnableactiveBarrier"].Cast<KeyBind>().CurrentValue
                                        || Spells.SummMenu["EnableBarrier"].Cast<KeyBind>().CurrentValue) && Spells.Barrier.IsReady();
                        var barrierme = Spells.SummMenu["barrierme"].Cast<Slider>().CurrentValue;
                        if (barrierc)
                        {
                            if (target.HealthPercent <= barrierme || death)
                            {
                                Spells.Barrier.Cast();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Helpers.Log(e.ToString());
            }
        }

        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            try
            {
                if (!(args.Target is AIHeroClient))
                {
                    return;
                }

                var caster = sender;
                var enemy = sender as AIHeroClient;
                var target = (AIHeroClient)args.Target;
                var hit = args.End != Vector3.Zero && args.End.Distance(target) < 100;

                if (!(caster is AIHeroClient || caster is Obj_AI_Turret) || !caster.IsEnemy || !hit || enemy == null || target == null
                    || caster == null || !target.IsAlly || !target.IsKillable())
                {
                    return;
                }

                var spelldamage = enemy.GetSpellDamage(target, args.Slot);
                var damagepercent = (spelldamage / target.TotalShieldHealth()) * 100;
                var death = damagepercent >= target.HealthPercent || spelldamage >= target.TotalShieldHealth()
                            || caster.GetAutoAttackDamage(target, true) >= target.TotalShieldHealth()
                            || enemy.GetAutoAttackDamage(target, true) >= target.TotalShieldHealth();
                ;

                if (target.IsAlly && !target.IsMe)
                {
                    if (Spells.Exhaust != null)
                    {
                        var exhaustc = (Spells.SummMenu["EnableactiveExhaust"].Cast<KeyBind>().CurrentValue
                                        || Spells.SummMenu["EnableExhaust"].Cast<KeyBind>().CurrentValue) && Spells.Exhaust.IsReady();
                        var Exhaustally = Spells.SummMenu["exhaustally"].Cast<Slider>().CurrentValue;
                        var Exhaustenemy = Spells.SummMenu["exhaustenemy"].Cast<Slider>().CurrentValue;

                        if (exhaustc
                            && (target.IsValidTarget(Spells.Exhaust.Range)
                                && !Spells.SummMenu["DontExhaust" + caster.BaseSkinName].Cast<CheckBox>().CurrentValue))
                        {
                            if (target.HealthPercent <= Exhaustenemy || target.HealthPercent <= Exhaustally || death)
                            {
                                Spells.Exhaust.Cast(caster);
                            }
                        }
                    }

                    if (Spells.Heal != null && !Spells.SummMenu["DontHeal" + target.BaseSkinName].Cast<CheckBox>().CurrentValue)
                    {
                        var healc = (Spells.SummMenu["EnableactiveHeal"].Cast<KeyBind>().CurrentValue
                                     || Spells.SummMenu["EnableHeal"].Cast<KeyBind>().CurrentValue) && Spells.Heal.IsReady();
                        var healally = Spells.SummMenu["Healally"].Cast<Slider>().CurrentValue;
                        if (healc)
                        {
                            if (target.IsInRange(Player.Instance, Spells.Heal.Range))
                            {
                                if (target.HealthPercent <= healally || death)
                                {
                                    Spells.Heal.Cast();
                                }
                            }
                        }
                    }
                }

                if (target.IsValidTarget(Defensive.FOTM.Range) && Defensive.FaceOfTheMountainc)
                {
                    if (Defensive.FaceOfTheMountainh >= target.HealthPercent || death || damagepercent >= Defensive.FaceOfTheMountainn)
                    {
                        Defensive.FOTM.Cast(target);
                    }
                }

                if (target.IsValidTarget(Defensive.Solari.Range) && Defensive.Solaric)
                {
                    if (Defensive.Solarih >= target.HealthPercent || death || damagepercent >= Defensive.Solarin)
                    {
                        Defensive.Solari.Cast(target);
                    }
                }

                if (target.IsMe)
                {
                    if (Defensive.Seraphc)
                    {
                        if (Defensive.Seraphh >= target.HealthPercent || death || damagepercent >= Defensive.Seraphn)
                        {
                            Defensive.Seraph.Cast();
                        }
                    }

                    if (Defensive.Zhonyasc)
                    {
                        if (Defensive.Zhonyash >= target.HealthPercent || death || damagepercent >= Defensive.Zhonyasn)
                        {
                            Defensive.Zhonyas.Cast();
                        }
                    }

                    if (Spells.Heal != null && !Spells.SummMenu["DontHeal" + target.BaseSkinName].Cast<CheckBox>().CurrentValue)
                    {
                        var healc = (Spells.SummMenu["EnableactiveHeal"].Cast<KeyBind>().CurrentValue
                                     || Spells.SummMenu["EnableHeal"].Cast<KeyBind>().CurrentValue) && Spells.Heal.IsReady();
                        var healme = Spells.SummMenu["Healme"].Cast<Slider>().CurrentValue;
                        if (healc)
                        {
                            if (target.HealthPercent <= healme || death)
                            {
                                Spells.Heal.Cast();
                            }
                        }
                    }

                    if (Spells.Exhaust != null)
                    {
                        var exhaustc = (Spells.SummMenu["EnableactiveExhaust"].Cast<KeyBind>().CurrentValue
                                        || Spells.SummMenu["EnableExhaust"].Cast<KeyBind>().CurrentValue) && Spells.Exhaust.IsReady();
                        var Exhaustally = Spells.SummMenu["exhaustally"].Cast<Slider>().CurrentValue;
                        var Exhaustenemy = Spells.SummMenu["exhaustenemy"].Cast<Slider>().CurrentValue;
                        if (exhaustc && !Spells.SummMenu["DontExhaust" + caster.BaseSkinName].Cast<CheckBox>().CurrentValue)
                        {
                            if (target.HealthPercent <= Exhaustenemy || target.HealthPercent <= Exhaustally || death)
                            {
                                Spells.Exhaust.Cast(caster);
                            }
                        }
                    }

                    if (Spells.Barrier != null)
                    {
                        var barrierc = (Spells.SummMenu["EnableactiveBarrier"].Cast<KeyBind>().CurrentValue
                                        || Spells.SummMenu["EnableBarrier"].Cast<KeyBind>().CurrentValue) && Spells.Barrier.IsReady();
                        var barrierme = Spells.SummMenu["barrierme"].Cast<Slider>().CurrentValue;
                        if (barrierc)
                        {
                            if (target.HealthPercent <= barrierme || death)
                            {
                                Spells.Barrier.Cast();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Helpers.Log(e.ToString());
            }
        }
    }
}