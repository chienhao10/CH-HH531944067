using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using Damage = LeagueSharp.Common.Damage;
using Geometry = LeagueSharp.Common.Geometry;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

namespace MathFizz
{
    internal class Program
    {
        #region OnDraw

        private static void OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(drawingsMenu, "drawQ"))
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.DarkRed, 3);
            }
            if (getCheckBoxItem(drawingsMenu, "drawE"))
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.DarkRed, 3);
            }
            if (getCheckBoxItem(drawingsMenu, "drawEMax"))
            {
                Render.Circle.DrawCircle(Player.Position, E.Range + E.Range, Color.DarkRed, 3);
            }
            if (getCheckBoxItem(drawingsMenu, "drawRr"))
            {
                Render.Circle.DrawCircle(Player.Position, R.Range, Color.DarkRed, 3);
            }
            if (getCheckBoxItem(drawingsMenu, "drawMinionQCombo") && SelectedTarget.IsValidTarget())
            {
                if (Player.LSDistance(SelectedTarget) <= R.Range + Q.Range)
                {
                    RRectangle.Draw(Color.CornflowerBlue, 3);
                }
            }
            if (hitchanceR != "" && getCheckBoxItem(drawingsMenu, "drawRHitChance"))
            {
                Drawing.DrawText(getSliderItem(drawingsMenu, "drawRHitChanceX"),
                    getSliderItem(drawingsMenu, "drawRHitChanceY"), Color.DarkTurquoise, "Hitchance: " + hitchanceR);
            }
            if (debugText != "")
            {
                Drawing.DrawText(400, 600, Color.DarkTurquoise, "Debug: " + debugText);
            }
            if (debugText2 != "")
            {
                Drawing.DrawText(400, 800, Color.DarkTurquoise, "Debug: " + debugText2);
            }
            if (getCheckBoxItem(drawingsMenu, "drawR") && SelectedTarget.IsValidTarget())
            {
                Render.Circle.DrawCircle(
                    R.GetPrediction(SelectedTarget, false, Player.LSDistance(SelectedTarget.Position))
                        .CastPosition.LSExtend(Player.Position, -600), 250, Color.Blue);
            }
            if (getCheckBoxItem(drawingsMenu, "drawComboDamage"))
            {
                foreach (var unit in HeroManager.Enemies.Where(u => u.IsValidTarget() && u.IsHPBarRendered))
                {
                    var damage = TotalComboDamage(unit);
                    if (damage <= 0)
                    {
                        continue;
                    }
                    var damagePercentage = (unit.Health - damage > 0 ? unit.Health - damage : 0)/unit.MaxHealth;
                    var currentHealthPercentage = unit.Health/unit.MaxHealth;

                    var startPoint = new Vector2((int) (unit.HPBarPosition.X + BarOffset.X + damagePercentage*104),
                        (int) (unit.HPBarPosition.Y + BarOffset.Y) - 5);
                    var endPoint =
                        new Vector2((int) (unit.HPBarPosition.X + BarOffset.X + currentHealthPercentage*104) + 1,
                            (int) (unit.HPBarPosition.Y + BarOffset.Y) - 5);

                    Drawing.DrawLine(startPoint, endPoint, 9, Color.Goldenrod);
                }
            }
        }

        #endregion

        #region OnUpdate

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead || Player.IsRecalling())
            {
                return;
            }

            ping = Game.Ping;

            SelectedTarget = TargetSelector.SelectedTarget;

            #region working stuff

            if (SelectedTarget.IsValidTarget())
            {
                if (Player.LSDistance(SelectedTarget) <= R.Range + Q.Range + 100)
                {
                    CollisionableObjects[] collisionCheck = {CollisionableObjects.YasuoWall};
                    RRectangle.Start = Player.Position.LSShorten(SelectedTarget.Position, -250).To2D();
                    RRectangle.End =
                        R.GetPrediction(SelectedTarget, false, 1, collisionCheck)
                            .CastPosition.LSExtend(Player.Position, -330)
                            .To2D();
                    RRectangle.UpdatePolygon();
                }
            }

            if (getCheckBoxItem(harassMenu, "useEWQ"))
            {
                if (!getCheckBoxItem(harassMenu, "useharassQ"))
                {
                    harassMenu["useharassQ"].Cast<CheckBox>().CurrentValue = true;
                }

                if (!getCheckBoxItem(harassMenu, "useharassE"))
                {
                    harassMenu["useharassE"].Cast<CheckBox>().CurrentValue = true;
                }

                if (!getCheckBoxItem(harassMenu, "useharassW"))
                {
                    harassMenu["useharassW"].Cast<CheckBox>().CurrentValue = true;
                }

                if (getSliderItem(harassMenu, "harassmana") != 0)
                {
                    harassMenu["harassmana"].Cast<Slider>().CurrentValue = 0;
                }
            }

            if (!getCheckBoxItem(comboMenu, "useEcombo"))
            {
                if (getCheckBoxItem(comboMenu, "useZhonya"))
                {
                    comboMenu["useZhonya"].Cast<CheckBox>().CurrentValue = false;
                }
            }

            if (getBoxItem(comboMenu, "ComboMode") == 1)
            {
                //Menu.Item("HitChancewR").SetValue<StringList>(new StringList(new[] { "Medium", "High", "Very High" }));
            }

            if (getBoxItem(comboMenu, "ComboMode") == 3)
            {
                //Menu.Item("HitChancewR").SetValue<StringList>(new StringList(new[] { "Medium", "High", "Very High" }));
            }


            if (!getKeyBindItem(customComboMenu, "EFlashCombo") && isEProcessed)
            {
                isEProcessed = false;
            }


            if (getCheckBoxItem(drawingsMenu, "drawRHitChance") && SelectedTarget.IsValidTarget())
            {
                var collisionCheck = new CollisionableObjects[1];
                collisionCheck[0] = CollisionableObjects.YasuoWall;
                var test = R.GetPrediction(SelectedTarget, false, -1, collisionCheck).Hitchance;
                if (test == HitChance.Collision) hitchanceR = "Collision Detected";
                if (test == HitChance.Dashing) hitchanceR = "Is Dashing";
                if (test == HitChance.Immobile) hitchanceR = "Immobile";
                if (test == HitChance.Medium) hitchanceR = "Medium Chance";
                if (test == HitChance.VeryHigh) hitchanceR = "VeryHigh Chance";
                if (test == HitChance.Low) hitchanceR = "Low  Chance";
                if (test == HitChance.High) hitchanceR = "High Chance";
                if (test == HitChance.Impossible) hitchanceR = "Impossible";
            }


            if (R.IsReady())
            {
                doOnce = true;
            }


            if (Player.LastCastedSpellT() >= 1)
            {
                if (doOnce && Player.LastCastedspell().Name == "FizzMarinerDoom")
                {
                    RCooldownTimer = Game.Time;
                    doOnce = false;
                }
            }


            //R cast tick lastRCastTick
            canCastZhonyaOnDash = Game.Time - RCooldownTimer <= 5.0f;

            #endregion

            #region Orbwalker

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Lane();
                Jungle();
            }

            #endregion

            #region flee

            if (getKeyBindItem(customComboMenu, "Flee"))
            {
                Flee();
            }

            #endregion

            #region Auto cast R

            if (R.IsReady())
            {
                if (getKeyBindItem(customComboMenu, "manualR"))
                {
                    var t = SelectedTarget;
                    if (!t.IsValidTarget())
                    {
                        t = TargetSelector.GetTarget(R.Range, DamageType.Magical);
                        if (!t.IsValidTarget())
                        {
                            t = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                            if (!t.IsValidTarget())
                            {
                                t = TargetSelector.GetTarget(R.Range, DamageType.True);
                            }
                        }
                    }
                    if (t.IsValidTarget())
                    {
                        if (Player.LSDistance(t.Position) <= R.Range)
                        {
                            //if enemy is not facing us, check via movespeed
                            if (!t.LSIsFacing(Player))
                            {
                                if (Player.LSDistance(t.Position) < R.Range - t.MoveSpeed - 165)
                                {
                                    CastRSmart(t);
                                    lastRCastTick = Game.Time;
                                }
                            }
                            else
                            {
                                if (Player.LSDistance(t.Position) <= R.Range)
                                {
                                    CastRSmart(t);
                                    lastRCastTick = Game.Time;
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            #region Custom combo's

            if (getKeyBindItem(customComboMenu, "lateGameZhonyaCombo"))
            {
                lateGameZhonyaCombo();
            }
            if (getKeyBindItem(customComboMenu, "QminionREWCombo"))
            {
                QminionREWCombo();
            }
            if (getKeyBindItem(customComboMenu, "EFlashCombo"))
            {
                EFlashCombo();
            }

            #endregion
        }

        #endregion

        #region Flee

        private static void Flee()
        {
            Orbwalker.OrbwalkTo(Game.CursorPos);
            if (E.IsReady())
            {
                E.Cast(Game.CursorPos);
            }
        }

        #endregion

        #region R smartCast

        //R usage
        public static void CastRSmart(AIHeroClient target)
        {
            var veryhigh = getBoxItem(comboMenu, "HitChancewR") == 2;
            var medium = getBoxItem(comboMenu, "HitChancewR") == 0;
            var high = getBoxItem(comboMenu, "HitChancewR") == 1;
            var veryhighAuto = getBoxItem(customComboMenu, "manualRHitchance") == 2;
            var mediumAuto = getBoxItem(customComboMenu, "manualRHitchance") == 0;
            var highAuto = getBoxItem(customComboMenu, "manualRHitchance") == 1;
            if (R.IsReady())
            {
                //Check YasuoWall
                var collisionCheck = new CollisionableObjects[1];
                collisionCheck[0] = CollisionableObjects.YasuoWall;
                var hitChance = R.GetPrediction(target, false, -1, collisionCheck).Hitchance;
                var endPosition =
                    R.GetPrediction(target, false, Player.LSDistance(target.Position), collisionCheck)
                        .CastPosition.LSExtend(Player.Position, -600);
                //Tweak hitchance
                if (hitChance == HitChance.OutOfRange || hitChance == HitChance.Low || hitChance == HitChance.Immobile)
                {
                    hitChance = HitChance.Medium;
                }
                //Check for spellshields
                if (!target.HasBuff("summonerbarrier") || !target.HasBuff("BlackShield") ||
                    !target.HasBuff("SivirShield") || !target.HasBuff("BansheesVeil") ||
                    !target.HasBuff("ShroudofDarkness"))
                {
                    //in combo & custom combo casts hitchance
                    if (medium && hitChance >= HitChance.Medium && !getKeyBindItem(customComboMenu, "manualR"))
                    {
                        R.Cast(endPosition);
                    }
                    if (high && hitChance >= HitChance.High && !getKeyBindItem(customComboMenu, "manualR"))
                    {
                        R.Cast(endPosition);
                    }
                    if (veryhigh && hitChance >= HitChance.VeryHigh && !getKeyBindItem(customComboMenu, "manualR"))
                    {
                        R.Cast(endPosition);
                    }
                    //manual casts hitchance
                    if (mediumAuto && hitChance >= HitChance.Medium && getKeyBindItem(customComboMenu, "manualR"))
                    {
                        R.Cast(endPosition);
                    }
                    if (highAuto && hitChance >= HitChance.High && getKeyBindItem(customComboMenu, "manualR"))
                    {
                        R.Cast(endPosition);
                    }
                    if (veryhighAuto && hitChance >= HitChance.VeryHigh && getKeyBindItem(customComboMenu, "manualR"))
                    {
                        R.Cast(endPosition);
                    }
                }
            }
        }

        #endregion

        #region TotalComboDamage

        private static float TotalComboDamage(AIHeroClient target)
        {
            double damage = 0;
            double predamage;
            if (Q.IsReady())
            {
                predamage = Player.GetSpellDamage(target, SpellSlot.Q);
                damage += Player.CalcDamage(target, DamageType.Magical, predamage);
            }
            if (W.IsReady())
            {
                predamage = Player.GetSpellDamage(target, SpellSlot.W);
                damage += Player.CalcDamage(target, DamageType.Magical, predamage);
            }
            if (E.IsReady())
            {
                predamage = Player.GetSpellDamage(target, SpellSlot.E);
                damage += Player.CalcDamage(target, DamageType.Magical, predamage);
            }
            if (R.IsReady())
            {
                predamage = Player.GetSpellDamage(target, SpellSlot.R);
                damage += Player.CalcDamage(target, DamageType.Magical, predamage);
            }
            if (D.IsReady())
            {
                //ignite
                predamage = Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
                damage += Player.CalcDamage(target, DamageType.True, predamage);
            }
            if (I.IsReady())
            {
                //smite
                predamage = Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Smite);
                damage += Player.CalcDamage(target, DamageType.True, predamage);
            }
            predamage = Player.GetAutoAttackDamage(target);
            damage += Player.CalcDamage(target, DamageType.Physical, predamage);
            return (float) damage;
        }

        #endregion

        #region Lane

        private static void Lane()
        {
            if (ObjectManager.Player.ManaPercent <= getSliderItem(laneClearMenu, "lanemana"))
            {
                return;
            }
            if (getCheckBoxItem(laneClearMenu, "laneclearQ") && Q.IsReady())
            {
                MinionList = MinionManager.GetMinions(Player.ServerPosition, Q.Range);
                foreach (var minion in MinionList)
                {
                    Q.Cast(minion);
                }
            }
            if (getCheckBoxItem(laneClearMenu, "laneclearW") && W.IsReady())
            {
                var allMinionsW =
                    MinionManager.GetMinions(Player.Position, W.Range).ToList();
                foreach (var minion in allMinionsW)
                {
                    W.Cast(minion);
                }
            }
            if (getCheckBoxItem(laneClearMenu, "laneclearE") && E.Instance.Name == "FizzJump" && E.IsReady())
            {
                var allMinionsE =
                    MinionManager.GetMinions(Player.Position, E.Range).ToList();
                foreach (var minion in allMinionsE)
                {
                    E.Cast(minion);
                }
            }
        }

        #endregion

        #region Jungle

        private static void Jungle()
        {
            if (ObjectManager.Player.ManaPercent <= getSliderItem(jungleClearMenu, "junglemana"))
            {
                return;
            }
            var mobs = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);
            if (!mobs.Any())
                return;
            var mob = mobs.First();

            if (getCheckBoxItem(jungleClearMenu, "jungleclearQ") && Q.IsReady() && mob.IsValidTarget(Q.Range))
            {
                Q.Cast(mob);
            }
            if (getCheckBoxItem(jungleClearMenu, "jungleclearW") && W.IsReady() && mob.IsValidTarget(W.Range))
            {
                W.Cast(mob);
            }
            if (getCheckBoxItem(jungleClearMenu, "jungleclearE") && E.IsReady() && mob.IsValidTarget(E.Range))
            {
                E.Cast(mob.ServerPosition);
            }
        }

        #endregion

        #region Harass

        private static void Harass()
        {
            var useQ = getCheckBoxItem(harassMenu, "useharassQ") && Q.IsReady();
            var useW = getCheckBoxItem(harassMenu, "useharassW") && W.IsReady();
            var useE = getCheckBoxItem(harassMenu, "useharassE") && E.IsReady();
            var m = SelectedTarget;
            if (!m.IsValidTarget())
            {
                m = TargetSelector.GetTarget(530, DamageType.Magical);
                if (!m.IsValidTarget())
                {
                    m = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                    if (!m.IsValidTarget())
                    {
                        m = TargetSelector.GetTarget(R.Range, DamageType.True);
                    }
                }
            }
            if (m.IsValidTarget())
            {
                if (ObjectManager.Player.ManaPercent <= getSliderItem(harassMenu, "harassmana"))
                {
                    return;
                }

                #region EWQ Combo

                //EWQ Combo
                if (getCheckBoxItem(harassMenu, "useEWQ"))
                {
                    if (Q.IsReady())
                    {
                        //Do EWQ
                        if (Player.Mana >= Q.ManaCost + E.ManaCost + W.ManaCost || enoughManaEWQ)
                        {
                            if (useE && E.Instance.Name == "FizzJump" && Player.LSDistance(m.Position) <= 530)
                            {
                                enoughManaEWQ = true;
                                startPos = Player.Position;
                                var harassEcastPosition = E.GetPrediction(m, false, 1).CastPosition;
                                E.Cast(harassEcastPosition);
                                //Delay for fizzjumptwo
                                Utility.DelayAction.Add(365 - ping,
                                    () => E.Cast(E.GetPrediction(m, false, 1).CastPosition.Extend(startPos, -135)));
                            }
                            if (useW && (Player.LSDistance(m.Position) <= 175))
                            {
                                W.Cast();
                                enoughManaEWQ = false;
                            }
                        }
                        //Do EQ
                        if (Player.Mana >= Q.ManaCost + E.ManaCost || enoughManaEQ)
                        {
                            if (useE && E.Instance.Name == "FizzJump" && Player.LSDistance(m.Position) <= 530)
                            {
                                enoughManaEQ = true;
                                startPos = Player.Position;
                                var harassEcastPosition3 = E.GetPrediction(m, false, 1).CastPosition;
                                E.Cast(harassEcastPosition3);
                                //Delay for fizzjumptwo
                                Utility.DelayAction.Add(365 - ping, () =>
                                {
                                    E.Cast(E.GetPrediction(m, false, 1).CastPosition.Extend(startPos, -135));
                                    enoughManaEQ = false;
                                });
                            }
                        }
                    }
                }
                    #endregion

                //Basic Harass WQ AA E
                else
                {
                    if (useW && (Player.LSDistance(m.Position) <= Q.Range)) W.Cast();
                    if (useQ && (Player.LSDistance(m.Position) <= Q.Range))
                    {
                        harassQCastedPosition = Player.Position;
                        Q.Cast(m);
                    }
                }
            }
        }

        #endregion

        #region Combo

        private static void Combo()
        {
            var useQ = Q.IsReady() && getCheckBoxItem(comboMenu, "useQcombo");
            var useW = W.IsReady() && getCheckBoxItem(comboMenu, "useWcombo");
            var useE = E.IsReady() && getCheckBoxItem(comboMenu, "useEcombo");
            var useR = R.IsReady() && getCheckBoxItem(comboMenu, "useRcombo");
            var UseEOnlyAfterAA = getCheckBoxItem(comboMenu, "UseEOnlyAfterAA");
            var useZhonya = getCheckBoxItem(comboMenu, "useZhonya") && zhonya.IsReady() && zhonya.IsOwned();
            var gapclose = getBoxItem(comboMenu, "ComboMode") == 0;
            var ondash = getBoxItem(comboMenu, "ComboMode") == 1;
            var afterdash = getBoxItem(comboMenu, "ComboMode") == 2;
            var realondash = getBoxItem(comboMenu, "ComboMode") == 3;
            var m = SelectedTarget;
            if (!m.IsValidTarget())
            {
                m = TargetSelector.GetTarget(R.Range, DamageType.Magical);
                if (!m.IsValidTarget())
                {
                    m = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                    if (!m.IsValidTarget())
                    {
                        m = TargetSelector.GetTarget(R.Range, DamageType.True);
                    }
                }
            }
            if (m.IsValidTarget())
            {
                //Only use when R is Ready & Q is Ready and target is valid
                if (ondash && !m.IsZombie && useR && Player.LSDistance(m.Position) <= 550)
                {
                    if (useQ && Player.LSDistance(m.Position) <= Q.Range)
                    {
                        if (useR && m.HealthPercent >= getSliderItem(comboMenu, "targetMinHPforR"))
                        {
                            CastRSmart(m);
                            lastRCastTick = Game.Time;
                        }
                        Q.Cast(m);
                    }
                    if (useW && Player.LSDistance(m.Position) <= 540)
                    {
                        W.Cast();
                    }
                    if (hydra.IsOwned() && Player.LSDistance(m) < hydra.Range && hydra.IsReady() && !E.IsReady())
                        hydra.Cast();
                    if (tiamat.IsOwned() && Player.LSDistance(m) < tiamat.Range && tiamat.IsReady() && !E.IsReady())
                        tiamat.Cast();
                }
                //Only use when R is Ready & Q is Ready
                if (afterdash && !m.IsZombie && useR)
                {
                    if (useW && Player.LSDistance(m.Position) <= 540) W.Cast();
                    if (useQ && Player.LSDistance(m.Position) <= Q.Range)
                    {
                        Q.Cast(m);
                        Utility.DelayAction.Add(540 - ping, () =>
                        {
                            if (useR && m.HealthPercent >= getSliderItem(comboMenu, "targetMinHPforR"))
                            {
                                CastRSmart(m);
                                lastRCastTick = Game.Time;
                            }
                        });
                    }
                    if (hydra.IsOwned() && Player.LSDistance(m) < hydra.Range && hydra.IsReady() && !E.IsReady())
                        hydra.Cast();
                    if (tiamat.IsOwned() && Player.LSDistance(m) < tiamat.Range && tiamat.IsReady() && !E.IsReady())
                        tiamat.Cast();
                }
                if (gapclose && !m.IsZombie && useR)
                {
                    if (useR && m.HealthPercent >= getSliderItem(comboMenu, "targetMinHPforR"))
                    {
                        //if enemy is not facing us, check via movespeed
                        if (!m.LSIsFacing(Player))
                        {
                            if (Player.LSDistance(m.Position) < R.Range - m.MoveSpeed - 165)
                            {
                                CastRSmart(m);
                                lastRCastTick = Game.Time;
                            }
                        }
                        else
                        {
                            if (Player.LSDistance(m.Position) <= R.Range - 200)
                            {
                                CastRSmart(m);
                                lastRCastTick = Game.Time;
                            }
                        }
                    }
                    if (useQ) Q.Cast(m);
                    if (useW && Player.LSDistance(m.Position) <= 540) W.Cast();
                    if (hydra.IsOwned() && Player.LSDistance(m) < hydra.Range && hydra.IsReady() && !E.IsReady())
                        hydra.Cast();
                    if (tiamat.IsOwned() && Player.LSDistance(m) < tiamat.Range && tiamat.IsReady() && !E.IsReady())
                        tiamat.Cast();
                }
                if (realondash && !m.IsZombie && useR && Player.LSDistance(m.Position) <= 550)
                {
                    if (useQ) Q.Cast(m);
                    if (useR && m.HealthPercent >= getSliderItem(comboMenu, "targetMinHPforR"))
                    {
                        if (Player.LSDistance(m.Position) <= 380)
                        {
                            Utility.DelayAction.Add(500 - ping, () =>
                            {
                                CastRSmart(m);
                                lastRCastTick = Game.Time;
                            });
                        }
                        else
                        {
                            CastRSmart(m);
                            lastRCastTick = Game.Time;
                        }
                    }
                    if (useW && Player.LSDistance(m.Position) <= 540)
                    {
                        W.Cast();
                    }
                    if (hydra.IsOwned() && Player.LSDistance(m) < hydra.Range && hydra.IsReady() && !E.IsReady())
                        hydra.Cast();
                    if (tiamat.IsOwned() && Player.LSDistance(m) < tiamat.Range && tiamat.IsReady() && !E.IsReady())
                        tiamat.Cast();
                }
                if (useW && Player.LSDistance(m.Position) <= 540) W.Cast();
                if (useQ && Player.LSDistance(m.Position) <= Q.Range) Q.Cast(m);
                if (!UseEOnlyAfterAA && E.Instance.Name == "FizzJump" && useE && Player.LSDistance(m.Position) > 300 &&
                    Player.LSDistance(m.Position) <= E.Range + 270 && !W.IsReady() && !Q.IsReady() && !R.IsReady())
                {
                    castPosition = E.GetPrediction(m, false, 1).CastPosition;
                    E.Cast(castPosition);
                    Utility.DelayAction.Add(680 - ping, () =>
                    {
                        if (!W.IsReady() && !Q.IsReady() && Player.LSDistance(m.Position) > 330 &&
                            Player.LSDistance(m.Position) <= 400 + 270)
                        {
                            E.Cast(E.GetPrediction(m, false, 1).CastPosition);
                        }
                    });
                    if (ondash && useZhonya && canCastZhonyaOnDash)
                    {
                        Utility.DelayAction.Add(2150 - ping, () => { zhonya.Cast(); });
                    }
                    if (gapclose && useZhonya && canCastZhonyaOnDash)
                    {
                        Utility.DelayAction.Add(2150 - ping, () => { zhonya.Cast(); });
                    }
                    if (afterdash && useZhonya && canCastZhonyaOnDash)
                    {
                        Utility.DelayAction.Add(2150 - ping, () => { zhonya.Cast(); });
                    }
                }
            }
        }

        #endregion

        #region Variables

        public static AIHeroClient Player = ObjectManager.Player;
        private static AIHeroClient SelectedTarget;

        public static Random Random;

        public static List<Obj_AI_Base> MinionList;

        //Spells
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Spell F;
        public static Spell D;
        public static Spell I;

        private static Items.Item tiamat;
        private static Items.Item hydra;
        private static Items.Item cutlass;
        private static Items.Item botrk;
        private static Items.Item hextech;
        private static Items.Item zhonya;

        public const string ChampionName = "Fizz";
        public static string hitchanceR = string.Empty;
        public static string debugText = string.Empty;
        public static string debugText2 = string.Empty;

        private static float lastRCastTick;
        private static float RCooldownTimer;

        private static bool doOnce = true;
        private static bool enoughManaEWQ;
        private static bool enoughManaEQ;
        private static bool canCastZhonyaOnDash;
        private static bool isEProcessed;

        private static readonly Vector2 BarOffset = new Vector2(10, 25);

        private static Vector3 startPos;
        private static Vector3 harassQCastedPosition;
        private static Vector3 castPosition;

        private static Geometry.Polygon.Rectangle RRectangle;

        private static int ping = 50;

        //Menu
        public static Menu Menu, comboMenu, harassMenu, laneClearMenu, jungleClearMenu, customComboMenu, drawingsMenu;

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        #endregion

        #region OnGameLoad

        public static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Fizz") return;
            Q = new Spell(SpellSlot.Q, 550);
            W = new Spell(SpellSlot.W, Orbwalking.GetRealAutoAttackRange(Player));
            E = new Spell(SpellSlot.E, 400);
            R = new Spell(SpellSlot.R, 1300);
            F = new Spell(Player.GetSpellSlot("summonerflash"), 425);
            D = new Spell(Player.GetSpellSlot("summonerignite"), 600);
            I = new Spell(Player.GetSpellSlot("summonersmite"), 500);

            E.SetSkillshot(0.25f, 330, float.MaxValue, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.25f, 80, 1300, true, SkillshotType.SkillshotLine);

            RRectangle = new Geometry.Polygon.Rectangle(Player.Position, Player.Position, 300);

            Menu = MainMenu.AddMenu(Player.ChampionName, Player.ChampionName);

            //Combo Menu
            comboMenu = Menu.AddSubMenu("连招", "Combo");
            comboMenu.Add("ComboMode",
                new ComboBox("连招模式", 0, "R 接近", "R 冲刺范围", "R 冲刺后", "R 冲刺中"));
            comboMenu.Add("HitChancewR", new ComboBox("R 命中率", 2, "中", "高", "非常高"));
            comboMenu.Add("targetMinHPforR", new Slider("敌人最低血量 % 使用R", 35));
            comboMenu.Add("useZhonya", new CheckBox("连招使用金身 (后期推荐)"));
            comboMenu.Add("useQcombo", new CheckBox("使用 Q"));
            comboMenu.Add("useWcombo", new CheckBox("使用 W"));
            comboMenu.Add("useEcombo", new CheckBox("使用 E"));
            comboMenu.Add("UseEOnlyAfterAA", new CheckBox("普攻后再 E", false));
            comboMenu.Add("useRcombo", new CheckBox("使用R"));

            //Harass Menu
            harassMenu = Menu.AddSubMenu("骚扰", "Harass");
            harassMenu.Add("harassEMode",
                new ComboBox("E 模式", 0, "E 至鼠标位置", "E 至敌人", "E 回",
                    "E 回（2E,短距离）"));
            harassMenu.Add("useharassQ", new CheckBox("使用 Q"));
            harassMenu.Add("useharassW", new CheckBox("使用 W"));
            harassMenu.Add("useharassE", new CheckBox("使用 E"));
            harassMenu.Add("harassmana", new Slider("最低蓝量使用%"));
            harassMenu.Add("useEWQ", new CheckBox("使用 EE(W)Q 连击 进行骚扰", false));

            //LaneClear Menu
            laneClearMenu = Menu.AddSubMenu("清线", "Laneclear");
            laneClearMenu.Add("laneclearQ", new CheckBox("使用 Q", false));
            laneClearMenu.Add("laneclearW", new CheckBox("使用 W", false));
            laneClearMenu.Add("laneclearE", new CheckBox("使用 E", false));
            laneClearMenu.Add("lanemana", new Slider("最低蓝量使用%"));

            //JungleClear Menu
            jungleClearMenu = Menu.AddSubMenu("清野", "Jungleclear");
            jungleClearMenu.Add("jungleclearQ", new CheckBox("使用 Q", false));
            jungleClearMenu.Add("jungleclearW", new CheckBox("使用 W", false));
            jungleClearMenu.Add("jungleclearE", new CheckBox("使用 E", false));
            jungleClearMenu.Add("junglemana", new Slider("最低蓝量使用%"));

            //CustomCombo Menu
            customComboMenu = Menu.AddSubMenu("自定义连技 (需要选择目标!)", "CustomCombo");
            customComboMenu.AddGroupLabel("如何使用 :");
            customComboMenu.AddGroupLabel("1) 确定所有技能无冷却.");
            customComboMenu.AddGroupLabel("2) 选择目标.");
            customComboMenu.AddGroupLabel("3) 按下连招键，直到所有技能都用了.");
            customComboMenu.AddGroupLabel("4) 按下空格键，回到视角回到自己.");
            customComboMenu.Add("lateGameZhonyaCombo",
                new KeyBind("EE 接近 RWQ 中亚", false, KeyBind.BindTypes.HoldActive, 'G'));
            customComboMenu.Add("lateGameZhonyaComboZhonya", new CheckBox("EE 接近 RWQ连技 中使用中亚"));
            customComboMenu.Add("QminionREWCombo",
                new KeyBind("Q 小兵/野怪/英雄 进行接近 REW", false, KeyBind.BindTypes.HoldActive, 'H'));
            customComboMenu.Add("EFlashCombo",
                new KeyBind("E 闪现至目标 RWQ 中亚", false, KeyBind.BindTypes.HoldActive, 'J'));
            customComboMenu.Add("EFlashComboZhonya", new CheckBox("E 闪现至目标 RWQ连技 中使用中亚"));
            customComboMenu.Add("Flee",
                new KeyBind("逃跑按键 (不需要选择目标)", false, KeyBind.BindTypes.HoldActive, 'Q'));
            customComboMenu.Add("manualR", new KeyBind("自动使用 R 按键", false, KeyBind.BindTypes.HoldActive, 'K'));
            customComboMenu.Add("manualRHitchance",
                new ComboBox("自动使用 R 命中率", 2, "中", "高", "非常高"));

            //Drawings Menu
            drawingsMenu = Menu.AddSubMenu("线圈", "Drawings");
            drawingsMenu.Add("drawComboDamage", new CheckBox("显示预计连招伤害", false));
            drawingsMenu.Add("drawQ", new CheckBox("显示 Q 范围", false));
            drawingsMenu.Add("drawE", new CheckBox("显示 E 范围", false));
            drawingsMenu.Add("drawEMax", new CheckBox("显示 E 最大范围", false));
            drawingsMenu.Add("drawRr", new CheckBox("显示 R 范围", false));
            drawingsMenu.Add("drawMinionQCombo",
                new CheckBox("显示 Q小兵REW连招助手 (只在选择的目标)", false));
            drawingsMenu.Add("drawR", new CheckBox("显示 R 预判 (只在选择的目标)", false));
            drawingsMenu.Add("drawRHitChance",
                new CheckBox("显示  R命中率 状态文字 (只在选择的目标)", false));
            drawingsMenu.Add("drawRHitChanceX",
                new Slider("X 命中率 状态文字屏幕位置", 450, 0, 2000));
            drawingsMenu.Add("drawRHitChanceY",
                new Slider("Y 命中率 状态文字屏幕位置", 200, 0, 2000));

            hydra = new Items.Item(3074, 185);
            tiamat = new Items.Item(3077, 185);
            cutlass = new Items.Item(3144, 450);
            botrk = new Items.Item(3153, 450);
            hextech = new Items.Item(3146, 700);
            zhonya = new Items.Item(3157);
            Random = new Random();
            harassQCastedPosition = Player.Position;

            Obj_AI_Base.OnSpellCast += Obj_AI_Base_OnSpellCast;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
        }

        private static void Obj_AI_Base_OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.IsAutoAttack() && args.Target.IsValid<AIHeroClient>())
            {
                var useE = getCheckBoxItem(comboMenu, "useEcombo") && E.IsReady();
                var useZhonya = getCheckBoxItem(comboMenu, "useZhonya") && zhonya.IsReady();
                var ondash = getBoxItem(comboMenu, "ComboMode") == 1;
                var afterdash = getBoxItem(comboMenu, "ComboMode") == 2;
                var gapclose = getBoxItem(comboMenu, "ComboMode") == 0;
                var realondash = getBoxItem(comboMenu, "ComboMode") == 3;
                var target = (AIHeroClient) args.Target;

                if (target.IsMinion)
                {
                    return;
                }

                #region Orbwalking Combo

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    if (useE && E.Instance.Name == "FizzJump" && Player.LSDistance(target.Position) <= E.Range)
                    {
                        var castPosition1 = E.GetPrediction(target, false, 1)
                            .CastPosition.LSExtend(Player.Position, -165);
                        E.Cast(castPosition1);
                        if (useZhonya)
                        {
                            if (ondash && canCastZhonyaOnDash)
                            {
                                Utility.DelayAction.Add(1690 - ping, () => { zhonya.Cast(); });
                            }
                            if (afterdash && canCastZhonyaOnDash)
                            {
                                Utility.DelayAction.Add(1690 - ping, () => { zhonya.Cast(); });
                            }
                            if (gapclose && canCastZhonyaOnDash)
                            {
                                Utility.DelayAction.Add(1690 - ping, () => { zhonya.Cast(); });
                            }
                            if (realondash && canCastZhonyaOnDash)
                            {
                                Utility.DelayAction.Add(1690 - ping, () => { zhonya.Cast(); });
                            }
                        }
                        Utility.DelayAction.Add(660 - ping, () =>
                        {
                            if (!W.IsReady() && !Q.IsReady() && Player.LSDistance(target.Position) > 330 &&
                                Player.LSDistance(target.Position) <= 400 + 270)
                            {
                                E.Cast(E.GetPrediction(target, false, 1).CastPosition);
                            }
                        });
                    }
                }

                #endregion

                #region Orbwalking Harass

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                {
                    var useQ = getCheckBoxItem(harassMenu, "useharassQ") && Q.IsReady();
                    var useEHarass = getCheckBoxItem(harassMenu, "useharassE") && E.IsReady();
                    var EtoMousePos = getBoxItem(harassMenu, "harassEMode") == 0;
                    var EtoHitEnemy = getBoxItem(harassMenu, "harassEMode") == 1;
                    var EtoComeback = getBoxItem(harassMenu, "harassEMode") == 2;
                    var EEtoComeback = getBoxItem(harassMenu, "harassEMode") == 3;

                    if (getCheckBoxItem(harassMenu, "useEWQ"))
                    {
                        if (useQ && !E.IsReady() && Player.LSDistance(target.Position) <= Q.Range)
                        {
                            Q.Cast(target);
                        }
                    }
                    if (!getCheckBoxItem(harassMenu, "useEWQ"))
                    {
                        if (useEHarass && Player.LSDistance(target.Position) <= 550)
                        {
                            if (EtoComeback || EEtoComeback)
                            {
                                var haraspos = harassQCastedPosition.Extend(Player.Position, -(E.Range + E.Range));
                                //E to comeback
                                E.Cast(harassQCastedPosition.Extend(Player.Position, -(E.Range + E.Range)));
                                if (EEtoComeback)
                                {
                                    Utility.DelayAction.Add(365 - ping, () => E.Cast(haraspos));
                                }
                            }
                            if (EtoHitEnemy)
                            {
                                //E to enemy
                                var castPosition = E.GetPrediction(target, false, 1).CastPosition;
                                E.Cast(castPosition);
                                Utility.DelayAction.Add(660 - ping, () =>
                                {
                                    if (Player.LSDistance(target.Position) > 330 &&
                                        Player.LSDistance(target.Position) <= 400 + 270)
                                    {
                                        E.Cast(E.GetPrediction(target, false, 1).CastPosition);
                                    }
                                });
                            }
                            if (EtoMousePos)
                            {
                                //E to mouse
                                E.Cast(Game.CursorPos);
                                Utility.DelayAction.Add(660 - ping, () =>
                                {
                                    if (Player.LSDistance(Game.CursorPos) > 330)
                                    {
                                        E.Cast(Game.CursorPos);
                                    }
                                });
                            }
                        }
                    }
                }

                #endregion
            }
        }

        #endregion

        #region Custom Combos

        private static void lateGameZhonyaCombo()
        {
            Orbwalker.OrbwalkTo(Game.CursorPos);
            var m = SelectedTarget;
            if (m.IsValidTarget())
            {
                var distance = Player.LSDistance(m.Position);
                //Check distance
                if (distance <= E.Range + Q.Range + E.Range - 50)
                {
                    if (E.IsReady())
                    {
                        //Use E1
                        castPosition = E.GetPrediction(m, false, 1).CastPosition.LSExtend(Player.Position, -135);
                        E.Cast(castPosition);
                    }
                    if (R.IsReady() && !E.IsReady())
                    {
                        //Use R
                        //if enemy is not facing us, check via movespeed
                        if (!m.LSIsFacing(Player))
                        {
                            if (Player.LSDistance(m.Position) < R.Range - m.MoveSpeed - 165)
                            {
                                CastRSmart(m);
                                lastRCastTick = Game.Time;
                            }
                        }
                        else
                        {
                            if (Player.LSDistance(m.Position) <= R.Range)
                            {
                                CastRSmart(m);
                                lastRCastTick = Game.Time;
                            }
                        }
                    }
                    //Use W
                    if (W.IsReady() && !E.IsReady())
                    {
                        W.Cast();
                    }
                    //Use Q
                    if (Q.IsReady() && !E.IsReady())
                    {
                        Q.Cast(m);
                    }
                    if (Player.LastCastedSpellName() == "FizzPiercingStrike" &&
                        getCheckBoxItem(customComboMenu, "lateGameZhonyaComboZhonya"))
                    {
                        //Check if zhonya is active
                        if (zhonya.IsOwned() && zhonya.IsReady())
                        {
                            zhonya.Cast();
                        }
                    }
                }
            }
        }

        private static void QminionREWCombo()
        {
            Orbwalker.OrbwalkTo(Game.CursorPos);
            var m = SelectedTarget;
            if (m.IsValidTarget())
            {
                var distance = Player.LSDistance(m.Position);
                if (distance <= Q.Range + R.Range - 600)
                {
                    if (Q.IsReady())
                    {
                        //Check if HeroTarget is in Q.Range then Q 
                        if (Player.LSDistance(m.Position) <= Q.Range)
                        {
                            Q.Cast(m);
                        }
                        else
                        {
                            //Check if champions in rectange is in q range
                            var champions = HeroManager.Enemies;
                            foreach (AIHeroClient champion in champions.Where(champion => RRectangle.IsInside(champion.Position) && champion.LSDistance(m.Position) > 300 && Player.LSDistance(champion.Position) <= Q.Range))
                            {
                                Q.Cast(champion);
                            }
                            //Check if minions in rectangle is in Q.Range then Q
                            var mins = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.NotAlly);
                            foreach (var min in mins)
                            {
                                if (RRectangle.IsInside(min.Position) && min.LSDistance(m.Position) > 300)
                                {
                                    Q.Cast(min);
                                }
                            }
                        }
                    }
                    if (R.IsReady() && !Q.IsReady())
                    {
                        //Use R
                        Utility.DelayAction.Add(540 - ping, () =>
                        {
                            if (!m.LSIsFacing(Player))
                            {
                                if (Player.LSDistance(m.Position) < R.Range - m.MoveSpeed - 165)
                                {
                                    CastRSmart(m);
                                    lastRCastTick = Game.Time;
                                }
                            }
                            else
                            {
                                if (Player.LSDistance(m.Position) <= R.Range)
                                {
                                    CastRSmart(m);
                                    lastRCastTick = Game.Time;
                                }
                            }
                        });
                    }
                    if (E.IsReady() && Player.LastCastedSpellName() == "FizzMarinerDoom")
                    {
                        if (E.Instance.Name == "FizzJump")
                        {
                            //Use E1
                            castPosition = E.GetPrediction(m, false, 1).CastPosition.LSExtend(Player.Position, -165);
                            E.Cast(castPosition);
                        }
                        if (E.Instance.Name == "fizzjumptwo" && Player.LSDistance(m.Position) > 330)
                        {
                            //Use E2 if target not in range
                            castPosition = E.GetPrediction(m, false, 1).CastPosition.LSExtend(Player.Position, -135);
                            E.Cast(castPosition);
                        }
                    }
                    //Use W
                    if (W.IsReady() && Player.LastCastedSpellName() == "FizzJump")
                    {
                        W.Cast();
                    }
                }
            }
        }

        private static void EFlashCombo()
        {
            //E Flash RWQ Combo
            Orbwalker.OrbwalkTo(Game.CursorPos);
            var m = SelectedTarget;
            if (m.IsValidTarget())
            {
                var distance = Player.LSDistance(m.Position);
                if (distance <= E.Range + F.Range + 165)
                {
                    //E
                    if (E.IsReady() && E.Instance.Name == "FizzJump")
                    {
                        //Use E1
                        castPosition = E.GetPrediction(m, false, 1).CastPosition.LSExtend(Player.Position, -165);
                        E.Cast(castPosition);
                        Utility.DelayAction.Add(990 - ping, () => isEProcessed = true);
                    }
                    //Flash
                    if (F.IsReady() && !isEProcessed && Player.LastCastedSpellName() == "FizzJump" &&
                        Player.LSDistance(m.Position) <= F.Range + 530 && Player.LSDistance(m.Position) >= 330)
                    {
                        var endPosition = F.GetPrediction(m, false, 1).CastPosition.LSExtend(Player.Position, -135);
                        F.Cast(endPosition);
                    }
                    if (R.IsReady() && !F.IsReady())
                    {
                        CastRSmart(m);
                    }
                    if (W.IsReady() && !F.IsReady() && Player.LastCastedSpellName() == "FizzMarinerDoom")
                    {
                        W.Cast();
                    }
                    if (Q.IsReady() && !E.IsReady() && !F.IsReady() &&
                        Player.LastCastedSpellName() == "FizzSeastonePassive")
                    {
                        Q.Cast(m);
                    }
                    if (Player.LastCastedSpellName() == "FizzPiercingStrike" &&
                        getCheckBoxItem(customComboMenu, "EFlashComboZhonya"))
                    {
                        //Check if zhonya is active
                        if (zhonya.IsOwned() && zhonya.IsReady())
                        {
                            zhonya.Cast();
                        }
                    }
                }
            }
        }

        #endregion
    }
}
