using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using RengarPro_Revamped.Helper;

namespace RengarPro_Revamped.Modes
{
    internal class Combo : Standarts
    {
        public static void Do()
        {
            try
            {
                var enemyTarget = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                if (RengarHasUltimate || enemyTarget == null)
                {
                    return;
                }
                switch (MenuChecker.ComboModeSelected)
                {
                    case 1:
                    {
                        //One Shot Mode Logic
                        if (!RengarHasPassive && Ferocity <= 4)
                        {
                            if (enemyTarget.IsValidTarget(W.Range) && W.IsReady())
                            {
                                W.Cast();
                            }
                            if (enemyTarget.IsValidTarget(400))
                            {
                                CastItems();
                            }
                            CastSmite(Misc.Smite, enemyTarget);
                            if (enemyTarget.IsValidTarget(E.Range) && E.IsReady())
                            {
                                CastEPrediction(enemyTarget);
                            }
                            if (enemyTarget.IsValidTarget(Q.Range) && Q.IsReady())
                            {
                                Q.Cast();
                            }
                        }
                        if (!RengarHasPassive && Ferocity == 5)
                        {
                            if (enemyTarget.IsValidTarget(Q.Range) && Q.IsReady())
                            {
                                Q.Cast();
                            }
                        }
                        if (RengarHasPassive && Ferocity <= 4)
                        {
                            if (enemyTarget.IsValidTarget(Rengar.GetAutoAttackRange()) && Q.IsReady())
                            {
                                Q.Cast();
                            }
                            if (enemyTarget.IsValidTarget(W.Range) && W.IsReady())
                            {
                                W.Cast();
                            }
                            if (enemyTarget.IsValidTarget(400))
                            {
                                CastItems();
                            }
                            CastSmite(Misc.Smite, enemyTarget);
                            if (enemyTarget.IsValidTarget(E.Range) && E.IsReady())
                            {
                                CastEPrediction(enemyTarget);
                            }
                        }
                        if (RengarHasPassive && Ferocity == 5)
                        {
                            if (enemyTarget.IsValidTarget(Rengar.GetAutoAttackRange()) && Q.IsReady())
                            {
                                Q.Cast();
                            }
                        }
                        if (enemyTarget.IsValidTarget(E.Range) && E.IsReady() && MenuChecker.UseEoutofQRange && !RengarQ &&
                            !RengarHasPassive)
                        {
                            CastEPrediction(enemyTarget);
                        }
                        break;
                    }
                    case 2:
                    {
                        //Snare Logic
                        if (!RengarHasPassive && Ferocity <= 4)
                        {
                            if (enemyTarget.IsValidTarget(W.Range) && W.IsReady())
                            {
                                W.Cast();
                            }
                            if (enemyTarget.IsValidTarget(400))
                            {
                                CastItems();
                            }
                            CastSmite(Misc.Smite, enemyTarget);
                            if (enemyTarget.IsValidTarget(E.Range) && E.IsReady())
                            {
                                CastEPrediction(enemyTarget);
                            }
                            if (enemyTarget.IsValidTarget(Q.Range) && Q.IsReady())
                            {
                                Q.Cast();
                            }
                        }
                        if (!RengarHasPassive && Ferocity == 5)
                        {
                            if (E.IsReady() && enemyTarget.IsValidTarget(E.Range))
                            {
                                CastEPrediction(enemyTarget);
                            }
                        }
                        if (RengarHasPassive && Ferocity <= 4)
                        {
                            if (enemyTarget.IsValidTarget(Rengar.GetAutoAttackRange()) && Q.IsReady())
                            {
                                Q.Cast();
                            }
                            if (enemyTarget.IsValidTarget(W.Range) && W.IsReady())
                            {
                                W.Cast();
                            }
                            if (enemyTarget.IsValidTarget(400))
                            {
                                CastItems();
                            }
                            CastSmite(Misc.Smite, enemyTarget);
                            if (enemyTarget.IsValidTarget(E.Range) && E.IsReady())
                            {
                                CastEPrediction(enemyTarget);
                            }
                        }
                        if (RengarHasPassive && Ferocity == 5)
                        {
                            if (enemyTarget.IsValidTarget(E.Range) && E.IsReady())
                            {
                                CastEPrediction(enemyTarget);
                            }
                        }
                        break;
                    }
                    case 3:
                    {
                        //Ap Rengoo
                        if (!RengarHasPassive && Ferocity == 4)
                        {
                            if (enemyTarget.IsValidTarget(W.Range) && W.IsReady())
                            {
                                W.Cast();
                            }
                        }
                        if (RengarHasPassive && Ferocity == 4)
                        {
                            if (enemyTarget.IsValidTarget(W.Range) && W.IsReady())
                            {
                                W.Cast();
                            }
                        }
                        if (!RengarHasPassive && Ferocity <= 4)
                        {
                            if (W.IsReady() && enemyTarget.IsValidTarget(W.Range))
                            {
                                W.Cast();
                            }
                            if (enemyTarget.IsValidTarget(400))
                            {
                                CastItems();
                            }
                            CastSmite(Misc.Smite, enemyTarget);
                            if (E.IsReady() && enemyTarget.IsValidTarget(E.Range))
                            {
                                CastEPrediction(enemyTarget);
                            }

                            if (Q.IsReady() && enemyTarget.IsValidTarget(Q.Range))
                            {
                                Q.Cast();
                            }
                        }
                        if (!RengarHasPassive && Ferocity == 5)
                        {
                            if (enemyTarget.IsValidTarget(W.Range) && W.IsReady())
                            {
                                W.Cast();
                            }
                        }
                        if (RengarHasPassive && Ferocity <= 4)
                        {
                            if (Q.IsReady() && enemyTarget.IsValidTarget(Rengar.GetAutoAttackRange()))
                            {
                                Q.Cast();
                            }
                            if (W.IsReady() && enemyTarget.IsValidTarget(W.Range))
                            {
                                W.Cast();
                            }
                            if (enemyTarget.IsValidTarget(400))
                            {
                                CastItems();
                            }
                            CastSmite(Misc.Smite, enemyTarget);
                            if (E.IsReady() && enemyTarget.IsValidTarget(E.Range))
                            {
                                CastEPrediction(enemyTarget);
                            }
                        }
                        if (RengarHasPassive && Ferocity == 5)
                        {
                            if (W.IsReady() && enemyTarget.IsValidTarget(W.Range))
                            {
                                W.Cast();
                            }
                        }
                        if (enemyTarget.IsValidTarget(E.Range) && E.IsReady() && MenuChecker.UseEoutofQRange && !RengarQ &&
                            !RengarHasPassive)
                        {
                            CastEPrediction(enemyTarget);
                        }
                    }
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        internal static void Dash_OnDash(Obj_AI_Base sender, Dash.DashEventArgs e)
        {
            try
            {
                if (!sender.IsMe)
                {
                    return;
                }

                var target = TargetSelector.GetTarget(1500, DamageType.Physical);
                if (!target.IsValidTarget())
                {
                    return;
                }

                if (Ferocity == 5 && MenuChecker.ComboModeSelected == 1)
                {
                    if (Q.IsReady())
                    {
                        Q.Cast();
                    }
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    if (Ferocity == 5)
                    {
                        switch (MenuChecker.ComboModeSelected)
                        {
                            case 2:
                                if (E.IsReady() && target.IsValidTarget(E.Range))
                                {
                                    CastEPrediction(target);
                                }
                                break;
                            case 1:
                                if (Q.IsReady() && target.IsValidTarget(Q.Range))
                                {
                                    Q.Cast();
                                }

                                if (target.IsValidTarget(Q.Range))
                                {
                                    Core.DelayAction((() =>
                                    {
                                        if (target.IsValidTarget(W.Range))
                                        {
                                            W.Cast();
                                        }

                                        CastEPrediction(target);
                                        CastItems();
                                    }), 50);
                                }
                                break;
                        }
                    }

                    switch (MenuChecker.ComboModeSelected)
                    {
                        case 2:
                            if (E.IsReady() && target.IsValidTarget(E.Range))
                            {
                                CastEPrediction(target);
                            }
                            break;

                        case 1:
                            if (Misc.BetaQVariables.BetaQActive &&
                                TargetSelector.SelectedTarget.IsValidTarget(Misc.BetaQVariables.BetaQRange) &&
                                MenuChecker.ComboModeSelected == 1 && RengarHasUltimate && Q.IsReady())
                            {
                                Q.Cast();
                            }
                            break;
                    }

                    if (e.Duration - 100 - Game.Ping/2 > 0)
                    {
                        Core.DelayAction((() => CastItems()), e.Duration - 100 - Game.Ping/2);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static void CastItems()
        {
            try
            {
                if (!(Item.CanUseItem(ItemId.Ravenous_Hydra_Melee_Only) || Item.CanUseItem(ItemId.Tiamat_Melee_Only)))
                {
                    return;
                }

                Item.UseItem(ItemId.Ravenous_Hydra_Melee_Only);
                Item.UseItem(ItemId.Tiamat_Melee_Only);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void CastSmite(SpellSlot smiteSlotx, AIHeroClient target)
        {
            try
            {
                if (MenuChecker.ComboSmiteActive && !RengarHasUltimate && Misc.Smite != SpellSlot.Unknown
                    && Rengar.Spellbook.CanUseSpell(Misc.Smite) == SpellState.Ready && target.IsValidTarget(500))
                {
                    Rengar.Spellbook.CastSpell(smiteSlotx, target);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void CastEPrediction(AIHeroClient eCastTarget)
        {
            try
            {
                var predictionE = E.GetPrediction(eCastTarget);
                if (predictionE.HitChancePercent >= 90 && predictionE.CollisionObjects.Count() == 0 &&
                    !eCastTarget.IsMoving)
                {
                    E.Cast(predictionE.CastPosition);
                }
                else if (predictionE.HitChancePercent >= 90 && predictionE.CollisionObjects.Count() == 0 &&
                         eCastTarget.IsMoving)
                {
                    E.Cast(predictionE.UnitPosition);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}