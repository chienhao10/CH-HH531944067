namespace AutoFarmer.Modes
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;

    using Genesis.Library;
    using Genesis.Library.Spells;

    internal class LastHit
    {
        protected static SpellBase Spells => SpellManager.CurrentSpells;

        internal static void Last()
        {
            var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(x => x != null);

            foreach (var target in minions)
            {
                if (AutoFarm.Lh.Get<ComboBox>(AutoFarm.Player.ChampionName + "Qmode").CurrentValue == 1
                    && AutoFarm.Lh[AutoFarm.Player.ChampionName + "Q"].Cast<CheckBox>().CurrentValue
                    && Player.Instance.ManaPercent
                    >= AutoFarm.ManaMenu[AutoFarm.Player.ChampionName + "Q"].Cast<Slider>().CurrentValue
                    && Spells.Q.IsReady())
                {
                    if (Spells.QisToggle || Spells.QisDash || Spells.QisCc || Spells.Q == null)
                    {
                        return;
                    }

                    if (ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q)
                        >= Prediction.Health.GetPrediction(target, (int)Spells.Q.CastDelay * 1000)
                        && Spells.Q.IsInRange(target))
                    {
                        if (Spells.Q.GetType() == typeof(Spell.Skillshot))
                        {
                            var qx = Spells.Q as Spell.Skillshot;
                            qx?.GetPrediction(target);
                            qx?.Cast(target);
                            return;
                        }

                        if (Spells.Q.GetType() == typeof(Spell.Active))
                        {
                            Spells.Q.Cast(target);
                            return;
                        }

                        if (Spells.Q.GetType() == typeof(Spell.Chargeable))
                        {
                            var qx = Spells.Q as Spell.Chargeable;
                            if (qx != null && !qx.IsCharging)
                            {
                                qx.StartCharging();
                            }

                            if (qx.Range == qx.MaximumRange)
                            {
                                qx.Cast(target.Position);
                            }

                            return;
                        }

                        if (Spells.Q.GetType() == typeof(Spell.Ranged))
                        {
                            Spells.Q.Cast(target.Position);
                            return;
                        }
                    }
                }

                if (AutoFarm.Lh.Get<ComboBox>(AutoFarm.Player.ChampionName + "Wmode").CurrentValue == 1
                    && AutoFarm.Lh[AutoFarm.Player.ChampionName + "W"].Cast<CheckBox>().CurrentValue
                    && Player.Instance.ManaPercent
                    >= AutoFarm.ManaMenu[AutoFarm.Player.ChampionName + "W"].Cast<Slider>().CurrentValue
                    && Spells.W.IsReady())
                {
                    if (Spells.WisToggle || Spells.WisDash || Spells.WisCc || Spells.W == null)
                    {
                        return;
                    }
                    if (ObjectManager.Player.GetSpellDamage(target, SpellSlot.W)
                        > Prediction.Health.GetPrediction(target, (int)(Spells.W.CastDelay))
                        && Spells.W.IsInRange(target))
                    {
                        if (Spells.W.GetType() == typeof(Spell.Skillshot))
                        {
                            var wx = Spells.W as Spell.Skillshot;
                            if (wx != null)
                            {
                                wx.GetPrediction(target);
                                wx.Cast(target);
                            }
                            return;
                        }

                        if (Spells.W.GetType() == typeof(Spell.Targeted))
                        {
                            Spells.W.Cast(target);
                            return;
                        }

                        if (Spells.W.GetType() == typeof(Spell.Active))
                        {
                            Spells.W.Cast();
                            return;
                        }

                        if (Spells.W.GetType() == typeof(Spell.Chargeable))
                        {
                            var wx = Spells.W as Spell.Chargeable;
                            if (wx != null && !wx.IsCharging)
                            {
                                wx.StartCharging();
                            }

                            if (wx != null && wx.Range == wx.MaximumRange)
                            {
                                wx.Cast(target.Position);
                            }
                            return;
                        }

                        if (Spells.W.GetType() == typeof(Spell.Ranged))
                        {
                            Spells.W.Cast(target.Position);
                            return;
                        }
                    }
                }

                if (AutoFarm.Lh.Get<ComboBox>(AutoFarm.Player.ChampionName + "Emode").CurrentValue == 1
                    && AutoFarm.Lh[AutoFarm.Player.ChampionName + "E"].Cast<CheckBox>().CurrentValue
                    && Player.Instance.ManaPercent
                    >= AutoFarm.ManaMenu[AutoFarm.Player.ChampionName + "E"].Cast<Slider>().CurrentValue
                    && Spells.E.IsReady())
                {
                    if (Spells.EisToggle || Spells.EisDash || Spells.EisCc || Spells.E == null)
                    {
                        return;
                    }
                    if (ObjectManager.Player.GetSpellDamage(target, SpellSlot.E)
                        > Prediction.Health.GetPrediction(target, (int)(Spells.E.CastDelay * 1000))
                        && Spells.E.IsInRange(target))
                    {
                        if (Spells.E.GetType() == typeof(Spell.Skillshot))
                        {
                            var ex = Spells.E as Spell.Skillshot;
                            if (ex != null)
                            {
                                ex.GetPrediction(target);
                                ex.Cast(target);
                            }
                            return;
                        }

                        if (Spells.E.GetType() == typeof(Spell.Targeted))
                        {
                            Spells.E.Cast(target);
                            return;
                        }

                        if (Spells.E.GetType() == typeof(Spell.Active))
                        {
                            Spells.E.Cast();
                            return;
                        }

                        if (Spells.E.GetType() == typeof(Spell.Chargeable))
                        {
                            Spell.Chargeable ex = Spells.E as Spell.Chargeable;
                            if (ex != null && !ex.IsCharging)
                            {
                                ex.StartCharging();
                            }

                            if (ex != null && ex.Range == ex.MaximumRange)
                            {
                                ex.Cast(target.Position);
                            }
                            return;
                        }

                        if (Spells.E.GetType() == typeof(Spell.Ranged))
                        {
                            Spells.E.Cast(target.Position);
                            return;
                        }
                    }
                }

                if (AutoFarm.Lh.Get<ComboBox>(AutoFarm.Player.ChampionName + "Rmode").CurrentValue == 1
                    && AutoFarm.Lh[AutoFarm.Player.ChampionName + "R"].Cast<CheckBox>().CurrentValue
                    && Player.Instance.ManaPercent
                    >= AutoFarm.ManaMenu[AutoFarm.Player.ChampionName + "R"].Cast<Slider>().CurrentValue)
                {
                    if (Spells.RisToggle || Spells.RisDash || Spells.RisCc || Spells.R == null)
                    {
                        return;
                    }
                    if (ObjectManager.Player.GetSpellDamage(target, SpellSlot.R)
                        > Prediction.Health.GetPrediction(target, (int)(Spells.R.CastDelay * 1000))
                        && Spells.R.IsInRange(target))
                    {
                        if (Spells.R.GetType() == typeof(Spell.Skillshot))
                        {
                            Spell.Skillshot rx = Spells.R as Spell.Skillshot;
                            if (rx != null)
                            {
                                rx.GetPrediction(target);
                                rx.Cast(target);
                            }
                            return;
                        }

                        if (Spells.R.GetType() == typeof(Spell.Targeted))
                        {
                            Spells.R.Cast(target);
                            return;
                        }

                        if (Spells.R.GetType() == typeof(Spell.Active))
                        {
                            Spells.R.Cast();
                            return;
                        }

                        if (Spells.R.GetType() == typeof(Spell.Chargeable))
                        {
                            Spell.Chargeable rx = Spells.R as Spell.Chargeable;
                            if (rx != null && !rx.IsCharging)
                            {
                                rx.StartCharging();
                            }

                            if (rx != null && rx.Range == rx.MaximumRange)
                            {
                                rx.Cast(target.Position);
                            }
                            return;
                        }

                        if (Spells.R.GetType() == typeof(Spell.Ranged))
                        {
                            Spells.R.Cast(target.Position);
                            return;
                        }
                    }
                }
            }
        }

        internal static void Orbwalker_OnUnkillableMinion(Obj_AI_Base target, Orbwalker.UnkillableMinionArgs args)
        {
            if (target == null || target.IsAlly)
            {
                return;
            }

            if (AutoFarm.Lh[AutoFarm.Player.ChampionName + "Enable"].Cast<KeyBind>().CurrentValue
                || AutoFarm.Lh[AutoFarm.Player.ChampionName + "Enableactive"].Cast<KeyBind>().CurrentValue)
            {
                if (AutoFarm.Lh.Get<ComboBox>(AutoFarm.Player.ChampionName + "Qmode").CurrentValue == 0
                    && AutoFarm.Lh[AutoFarm.Player.ChampionName + "Q"].Cast<CheckBox>().CurrentValue
                    && Player.Instance.ManaPercent
                    >= AutoFarm.ManaMenu[AutoFarm.Player.ChampionName + "Q"].Cast<Slider>().CurrentValue
                    && Spells.Q.IsReady())
                {
                    if (Spells.QisToggle || Spells.QisDash || Spells.QisCc || Spells.Q == null)
                    {
                        return;
                    }

                    if (ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q)
                        >= Prediction.Health.GetPrediction(target, (int)Spells.Q.CastDelay * 1000)
                        && Spells.Q.IsInRange(target))
                    {
                        if (Spells.Q.GetType() == typeof(Spell.Skillshot))
                        {
                            var qx = Spells.Q as Spell.Skillshot;
                            qx?.GetPrediction(target);
                            qx?.Cast(target);
                            return;
                        }

                        if (Spells.Q.GetType() == typeof(Spell.Active))
                        {
                            Spells.Q.Cast(target);
                            return;
                        }

                        if (Spells.Q.GetType() == typeof(Spell.Chargeable))
                        {
                            var qx = Spells.Q as Spell.Chargeable;
                            if (qx != null && !qx.IsCharging)
                            {
                                qx.StartCharging();
                            }

                            if (qx.Range == qx.MaximumRange)
                            {
                                qx.Cast(target.Position);
                            }

                            return;
                        }

                        if (Spells.Q.GetType() == typeof(Spell.Ranged))
                        {
                            Spells.Q.Cast(target.Position);
                            return;
                        }
                    }
                }

                if (AutoFarm.Lh.Get<ComboBox>(AutoFarm.Player.ChampionName + "Wmode").CurrentValue == 0
                    && AutoFarm.Lh[AutoFarm.Player.ChampionName + "W"].Cast<CheckBox>().CurrentValue
                    && Player.Instance.ManaPercent
                    >= AutoFarm.ManaMenu[AutoFarm.Player.ChampionName + "W"].Cast<Slider>().CurrentValue
                    && Spells.W.IsReady())
                {
                    if (Spells.WisToggle || Spells.WisDash || Spells.WisCc || Spells.W == null)
                    {
                        return;
                    }
                    if (ObjectManager.Player.GetSpellDamage(target, SpellSlot.W)
                        > Prediction.Health.GetPrediction(target, (int)(Spells.W.CastDelay))
                        && Spells.W.IsInRange(target))
                    {
                        if (Spells.W.GetType() == typeof(Spell.Skillshot))
                        {
                            var wx = Spells.W as Spell.Skillshot;
                            if (wx != null)
                            {
                                wx.GetPrediction(target);
                                wx.Cast(target);
                            }
                            return;
                        }

                        if (Spells.W.GetType() == typeof(Spell.Targeted))
                        {
                            Spells.W.Cast(target);
                            return;
                        }

                        if (Spells.W.GetType() == typeof(Spell.Active))
                        {
                            Spells.W.Cast();
                            return;
                        }

                        if (Spells.W.GetType() == typeof(Spell.Chargeable))
                        {
                            var wx = Spells.W as Spell.Chargeable;
                            if (wx != null && !wx.IsCharging)
                            {
                                wx.StartCharging();
                            }

                            if (wx != null && wx.Range == wx.MaximumRange)
                            {
                                wx.Cast(target.Position);
                            }
                            return;
                        }

                        if (Spells.W.GetType() == typeof(Spell.Ranged))
                        {
                            Spells.W.Cast(target.Position);
                            return;
                        }
                    }
                }

                if (AutoFarm.Lh.Get<ComboBox>(AutoFarm.Player.ChampionName + "Emode").CurrentValue == 0
                    && AutoFarm.Lh[AutoFarm.Player.ChampionName + "E"].Cast<CheckBox>().CurrentValue
                    && Player.Instance.ManaPercent
                    >= AutoFarm.ManaMenu[AutoFarm.Player.ChampionName + "E"].Cast<Slider>().CurrentValue
                    && Spells.E.IsReady())
                {
                    if (Spells.EisToggle || Spells.EisDash || Spells.EisCc || Spells.E == null)
                    {
                        return;
                    }
                    if (ObjectManager.Player.GetSpellDamage(target, SpellSlot.E)
                        > Prediction.Health.GetPrediction(target, (int)(Spells.E.CastDelay * 1000))
                        && Spells.E.IsInRange(target))
                    {
                        if (Spells.E.GetType() == typeof(Spell.Skillshot))
                        {
                            var ex = Spells.E as Spell.Skillshot;
                            if (ex != null)
                            {
                                ex.GetPrediction(target);
                                ex.Cast(target);
                            }
                            return;
                        }

                        if (Spells.E.GetType() == typeof(Spell.Targeted))
                        {
                            Spells.E.Cast(target);
                            return;
                        }

                        if (Spells.E.GetType() == typeof(Spell.Active))
                        {
                            Spells.E.Cast();
                            return;
                        }

                        if (Spells.E.GetType() == typeof(Spell.Chargeable))
                        {
                            Spell.Chargeable ex = Spells.E as Spell.Chargeable;
                            if (ex != null && !ex.IsCharging)
                            {
                                ex.StartCharging();
                            }

                            if (ex != null && ex.Range == ex.MaximumRange)
                            {
                                ex.Cast(target.Position);
                            }
                            return;
                        }

                        if (Spells.E.GetType() == typeof(Spell.Ranged))
                        {
                            Spells.E.Cast(target.Position);
                            return;
                        }
                    }
                }

                if (AutoFarm.Lh.Get<ComboBox>(AutoFarm.Player.ChampionName + "Rmode").CurrentValue == 0
                    && AutoFarm.Lh[AutoFarm.Player.ChampionName + "R"].Cast<CheckBox>().CurrentValue
                    && Player.Instance.ManaPercent
                    >= AutoFarm.ManaMenu[AutoFarm.Player.ChampionName + "R"].Cast<Slider>().CurrentValue)
                {
                    if (Spells.RisToggle || Spells.RisDash || Spells.RisCc || Spells.R == null)
                    {
                        return;
                    }
                    if (ObjectManager.Player.GetSpellDamage(target, SpellSlot.R)
                        > Prediction.Health.GetPrediction(target, (int)(Spells.R.CastDelay * 1000))
                        && Spells.R.IsInRange(target))
                    {
                        if (Spells.R.GetType() == typeof(Spell.Skillshot))
                        {
                            Spell.Skillshot rx = Spells.R as Spell.Skillshot;
                            if (rx != null)
                            {
                                rx.GetPrediction(target);
                                rx.Cast(target);
                            }
                            return;
                        }

                        if (Spells.R.GetType() == typeof(Spell.Targeted))
                        {
                            Spells.R.Cast(target);
                            return;
                        }

                        if (Spells.R.GetType() == typeof(Spell.Active))
                        {
                            Spells.R.Cast();
                            return;
                        }

                        if (Spells.R.GetType() == typeof(Spell.Chargeable))
                        {
                            Spell.Chargeable rx = Spells.R as Spell.Chargeable;
                            if (rx != null && !rx.IsCharging)
                            {
                                rx.StartCharging();
                            }

                            if (rx != null && rx.Range == rx.MaximumRange)
                            {
                                rx.Cast(target.Position);
                            }
                            return;
                        }

                        if (Spells.R.GetType() == typeof(Spell.Ranged))
                        {
                            Spells.R.Cast(target.Position);
                            return;
                        }
                    }
                }
            }
        }
    }
}