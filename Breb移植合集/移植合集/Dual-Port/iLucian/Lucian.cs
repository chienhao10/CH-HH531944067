using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DZLib.Core;
using DZLib.Positioning;
using iLucian.MenuHelper;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using iLucian.Utils;

namespace iLucian
{
    class Lucian
    {
        public void OnLoad()
        {
            Console.WriteLine("Loaded Lucian");
            MenuGenerator.Generate();
            LoadSpells();
            LoadEvents();
        }

        private void LoadEvents()
        {
            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnSpellCast += OnDoCast;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Obj_AI_Base.OnProcessSpellCast += OnSpellCast;
            Spellbook.OnCastSpell += (sender, args) =>
            {
                if (sender.Owner.IsMe && args.Slot == SpellSlot.E)
                {
                    Variables.LastECast = Environment.TickCount;
                }
            };
            Drawing.OnDraw += args =>
            {
                if (getCheckBoxItem(MenuGenerator.miscOptions, "com.ilucian.misc.drawQ"))
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Variables.Spell[Variables.Spells.Q2].Range, System.Drawing.Color.BlueViolet);
                }
            };
        }

        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (getCheckBoxItem(MenuGenerator.miscOptions, "com.ilucian.misc.antiVayne") && sender.IsEnemy && sender.IsChampion("Vayne")
                && args.Slot == SpellSlot.E)
            {
                var predictedPosition = ObjectManager.Player.ServerPosition.LSExtend(sender.Position, -425);
                var dashPosition = ObjectManager.Player.Position.LSExtend(Game.CursorPos, 450);
                var dashCondemnCheck = dashPosition.LSExtend(sender.Position, -425);

                if (Variables.Spell[Variables.Spells.E].IsReady() && predictedPosition.LSIsWall() && !dashCondemnCheck.LSIsWall())
                {
                    Variables.Spell[Variables.Spells.E].Cast(dashPosition);
                }
            }
        }

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

        private static void OnEnemyGapcloser(LeagueSharp.Common.ActiveGapcloser gapcloser)
        {
            if (!getCheckBoxItem(MenuGenerator.miscOptions, "com.ilucian.misc.gapcloser"))
            {
                return;
            }

            if (!gapcloser.Sender.IsEnemy || !(gapcloser.End.LSDistance(ObjectManager.Player.ServerPosition) < 300)) return;

            if (Variables.Spell[Variables.Spells.E].IsReady())
            {
                Variables.Spell[Variables.Spells.E].Cast(
                    ObjectManager.Player.ServerPosition.LSExtend(gapcloser.End, -475));
            }
        }

        private void OnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (args.SData.Name != "LucianPassiveShot" && !args.SData.Name.Contains("LucianBasicAttack"))
                return;

            var target = TargetSelector.GetTarget(Variables.Spell[Variables.Spells.Q].Range,
                DamageType.Physical);

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (target == null || Environment.TickCount - Variables.LastECast < 250) return;

                if (getCheckBoxItem(MenuGenerator.comboOptions, "com.ilucian.combo.startE") && Variables.Spell[Variables.Spells.E].IsReady())
                {
                    if (!sender.IsDead && !Variables.HasPassive())
                    {
                        CastE(target);
                    }

                    if (!Variables.Spell[Variables.Spells.E].IsReady() && target.LSIsValidTarget(Variables.Spell[Variables.Spells.Q].Range) && getCheckBoxItem(MenuGenerator.comboOptions, "com.ilucian.combo.q") && !Variables.HasPassive())
                    {
                        if (Variables.Spell[Variables.Spells.Q].IsReady()
                            && Variables.Spell[Variables.Spells.Q].IsInRange(target)
                            && !ObjectManager.Player.LSIsDashing())
                        {
                            Variables.Spell[Variables.Spells.Q].Cast(target);
                        }
                    }
                    if (!Variables.Spell[Variables.Spells.E].IsReady() && !ObjectManager.Player.LSIsDashing()
                       && getCheckBoxItem(MenuGenerator.comboOptions, "com.ilucian.combo.w"))
                    {
                        if (Variables.Spell[Variables.Spells.W].IsReady() && !Variables.HasPassive())
                        {
                            if (getCheckBoxItem(MenuGenerator.miscOptions, "com.ilucian.misc.usePrediction"))
                            {
                                var prediction = Variables.Spell[Variables.Spells.W].GetPrediction(target);
                                if (prediction.Hitchance >= HitChance.High)
                                {
                                    Variables.Spell[Variables.Spells.W].Cast(prediction.CastPosition);
                                }
                            }
                            else
                            {
                                if (target.LSDistance(ObjectManager.Player) < 600)
                                {
                                    Variables.Spell[Variables.Spells.W].Cast(target.Position);
                                }
                            }

                        }
                    }
                }
                else
                {
                    if (target.LSIsValidTarget(Variables.Spell[Variables.Spells.Q].Range)
                        && getCheckBoxItem(MenuGenerator.comboOptions, "com.ilucian.combo.q")
                        && !Variables.HasPassive())
                    {
                        if (Variables.Spell[Variables.Spells.Q].IsReady()
                            && Variables.Spell[Variables.Spells.Q].IsInRange(target)
                            && !ObjectManager.Player.LSIsDashing())
                        {
                            Variables.Spell[Variables.Spells.Q].Cast(target);
                        }
                    }

                    if (!ObjectManager.Player.LSIsDashing()
                        && getCheckBoxItem(MenuGenerator.comboOptions, "com.ilucian.combo.w"))
                    {
                        if (Variables.Spell[Variables.Spells.W].IsReady() && !Variables.HasPassive())
                        {
                            if (getCheckBoxItem(MenuGenerator.miscOptions, "com.ilucian.misc.usePrediction"))
                            {
                                var prediction = Variables.Spell[Variables.Spells.W].GetPrediction(target);
                                if (prediction.Hitchance >= HitChance.High)
                                {
                                    Variables.Spell[Variables.Spells.W].Cast(prediction.CastPosition);
                                }
                            }
                            else
                            {
                                if (target.LSDistance(ObjectManager.Player) < 600)
                                {
                                    Variables.Spell[Variables.Spells.W].Cast(target.Position);
                                }
                            }

                        }
                    }
                }

                if (!sender.IsDead && !Variables.HasPassive() && !Variables.Spell[Variables.Spells.Q].IsReady())
                {
                    CastE(target);
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                if (target == null || Environment.TickCount - Variables.LastECast < 250) return;

                if (target.LSIsValidTarget(Variables.Spell[Variables.Spells.Q].Range) &&
                    getCheckBoxItem(MenuGenerator.harassOptions, "com.ilucian.harass.q"))
                {
                    if (Variables.Spell[Variables.Spells.Q].IsReady() &&
                        Variables.Spell[Variables.Spells.Q].IsInRange(target))
                    {
                        Variables.Spell[Variables.Spells.Q].Cast(target);
                    }
                }
                if (!ObjectManager.Player.LSIsDashing()
                    && getCheckBoxItem(MenuGenerator.harassOptions, "com.ilucian.harass.w"))
                {
                    if (Variables.Spell[Variables.Spells.W].IsReady())
                    {
                        if (getCheckBoxItem(MenuGenerator.miscOptions, "com.ilucian.misc.usePrediction"))
                        {
                            var prediction = Variables.Spell[Variables.Spells.W].GetPrediction(target);
                            if (prediction.Hitchance >= HitChance.High)
                            {
                                Variables.Spell[Variables.Spells.W].Cast(prediction.CastPosition);
                            }
                        }
                        else
                        {
                            if (target.LSDistance(ObjectManager.Player) < 600)
                            {
                                Variables.Spell[Variables.Spells.W].Cast(target.Position);
                            }
                        }
                    }

                }
            }
        
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                if (args.Target is Obj_AI_Minion && args.Target.IsValid
                && ((Obj_AI_Minion)args.Target).Team == GameObjectTeam.Neutral)
                {
                    if (ObjectManager.Player.ManaPercent
                        < getSliderItem(MenuGenerator.jungleclearOptions, "com.ilucian.jungleclear.mana") || Variables.HasPassive())
                        return;

                    if (Variables.Spell[Variables.Spells.Q].IsReady()
                        && getCheckBoxItem(MenuGenerator.jungleclearOptions, "com.ilucian.jungleclear.q"))
                    {
                        Variables.Spell[Variables.Spells.Q].Cast((Obj_AI_Minion)args.Target);
                    }

                    if (Variables.Spell[Variables.Spells.W].IsReady()
                        && getCheckBoxItem(MenuGenerator.jungleclearOptions, "com.ilucian.jungleclear.w"))
                    {
                        Variables.Spell[Variables.Spells.W].Cast(((Obj_AI_Minion)args.Target).Position);
                    }

                    if (Variables.Spell[Variables.Spells.E].IsReady()
                        && getCheckBoxItem(MenuGenerator.jungleclearOptions, "com.ilucian.jungleclear.e"))
                    {
                        Variables.Spell[Variables.Spells.E].Cast(
                            ObjectManager.Player.Position.LSExtend(Game.CursorPos, 475));
                    }
                }
            }
        }

        private void LoadSpells()
        {
            Variables.Spell[Variables.Spells.Q].SetTargetted(0.25f, 1400f);
            Variables.Spell[Variables.Spells.Q2].SetSkillshot(0.5f, 50, float.MaxValue, false,
                SkillshotType.SkillshotLine);
            Variables.Spell[Variables.Spells.W].SetSkillshot(0.30f, 70f, 1600f, false, SkillshotType.SkillshotLine);
            Variables.Spell[Variables.Spells.R].SetSkillshot(0.2f, 110f, 2500, true, SkillshotType.SkillshotLine);
            Variables.Spell[Variables.Spells.Q3].SetSkillshot(0.25f, 70, 3000, false, SkillshotType.SkillshotLine);
        }

        public void Killsteal()
        {
            var ch = HeroManager.Enemies.Where(hero => hero.LSIsValidTarget(1150)).Where(hero => hero.LSDistance(ObjectManager.Player) > 675).FirstOrDefault();

            var target = TargetSelector.GetTarget(Variables.Spell[Variables.Spells.E].Range + Variables.Spell[Variables.Spells.Q2].Range, DamageType.Physical);

            if (!getCheckBoxItem(MenuGenerator.miscOptions, "com.ilucian.misc.eqKs") || !Variables.Spell[Variables.Spells.Q].IsReady() || !target.LSIsValidTarget(Variables.Spell[Variables.Spells.E].Range + Variables.Spell[Variables.Spells.Q2].Range))
            {
                return;
            }

            if (Variables.Spell[Variables.Spells.Q].GetDamage(target) - 20 >= target.Health)
            {
                if (target.LSIsValidTarget(Variables.Spell[Variables.Spells.Q].Range))
                {
                    Variables.Spell[Variables.Spells.Q].Cast(target);
                }

                if (target.LSIsValidTarget(Variables.Spell[Variables.Spells.Q2].Range) && !target.LSIsValidTarget(Variables.Spell[Variables.Spells.Q].Range))
                {
                    CastExtendedQ();
                }
                else if (Variables.Spell[Variables.Spells.E].IsReady() && Variables.Spell[Variables.Spells.Q].IsReady())
                {
                    CastEqKillsteal();
                }
            }
        }

        private void CastEqKillsteal()
        {
            var target = TargetSelector.GetTarget(Variables.Spell[Variables.Spells.E].Range + Variables.Spell[Variables.Spells.Q2].Range, DamageType.Physical);

            if (!target.LSIsValidTarget(Variables.Spell[Variables.Spells.E].Range + Variables.Spell[Variables.Spells.Q2].Range))
                return;

            var dashSpeed = (int)(Variables.Spell[Variables.Spells.E].Range / (700 + ObjectManager.Player.MoveSpeed));
            var extendedPrediction = GetExtendedPrediction(target, dashSpeed);

            var minions = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsEnemy && x.IsValid && x.LSDistance(extendedPrediction, true) < 900 * 900).OrderByDescending(x => x.LSDistance(extendedPrediction));

            foreach (var minion in minions.Select(x => LeagueSharp.Common.Prediction.GetPrediction(x, dashSpeed)).Select(pred => MathHelper.GetCicleLineInteraction(pred.UnitPosition.LSTo2D(), extendedPrediction.LSTo2D(), ObjectManager.Player.ServerPosition.LSTo2D(), Variables.Spell[Variables.Spells.E].Range)).Select(inter => inter.GetBestInter(target)))
            {
                if (Math.Abs(minion.X) < 1)
                    return;

                if (!NavMesh.GetCollisionFlags(minion.To3D()).HasFlag(CollisionFlags.Wall) && !NavMesh.GetCollisionFlags(minion.To3D()).HasFlag(CollisionFlags.Building) && minion.To3D().IsSafe(Variables.Spell[Variables.Spells.E].Range))
                {
                    Variables.Spell[Variables.Spells.E].Cast((Vector3)minion);
                }
            }

            var champions = ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy && x.IsValid && x.LSDistance(extendedPrediction, true) < 900 * 900).OrderByDescending(x => x.LSDistance(extendedPrediction));

            if (getCheckBoxItem(MenuGenerator.miscOptions, "com.ilucian.misc.useChampions"))
            {
                foreach (var position in
                    champions.Select(x => LeagueSharp.Common.Prediction.GetPrediction(x, dashSpeed)).Select(pred => MathHelper.GetCicleLineInteraction(pred.UnitPosition.LSTo2D(), extendedPrediction.LSTo2D(), ObjectManager.Player.ServerPosition.LSTo2D(), Variables.Spell[Variables.Spells.E].Range)).Select(inter => inter.GetBestInter(target)))
                {
                    if (Math.Abs(position.X) < 1)
                        return;

                    if (!NavMesh.GetCollisionFlags(position.To3D()).HasFlag(CollisionFlags.Wall) &&
                        !NavMesh.GetCollisionFlags(position.To3D()).HasFlag(CollisionFlags.Building) &&
                        position.To3D().IsSafe(Variables.Spell[Variables.Spells.E].Range))
                    {
                        Variables.Spell[Variables.Spells.E].Cast((Vector3)position);
                    }
                }
            }
        }

        //Detuks ofc
        private Vector3 GetExtendedPrediction(AIHeroClient target, int delay)
        {
            var res = Variables.Spell[Variables.Spells.Q2].GetPrediction(target);
            var del = LeagueSharp.Common.Prediction.GetPrediction(target, delay);

            var dif = del.UnitPosition - target.ServerPosition;
            return res.CastPosition + dif;
        }

        private void OnUpdate(EventArgs args)
        {
            Variables.Spell[Variables.Spells.W].Collision = getCheckBoxItem(MenuGenerator.miscOptions, "com.ilucian.misc.usePrediction");

            Killsteal();

            if (getCheckBoxItem(MenuGenerator.miscOptions, "com.ilucian.misc.antiMelee")
                && Variables.Spell[Variables.Spells.E].IsReady())
            {
                foreach (var meleeTarget in
                    HeroManager.Enemies.Where(
                        x =>
                        x.IsMelee && x.LSDistance(ObjectManager.Player) <= x.AttackRange * 2f
                        && ObjectManager.Player.HealthPercent <= 30 && x.HealthPercent > 50))
                {
                    Variables.Spell[Variables.Spells.E].Cast(
                        ObjectManager.Player.ServerPosition.LSExtend(meleeTarget.Position, -475));
                }
            }

            if (getKeyBindItem(MenuGenerator.comboOptions, "com.ilucian.combo.forceR")
                && ObjectManager.Player.HasBuff("LucianR"))
            {
                Orbwalker.DisableAttacking = true;
                SemiUlt();
                Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }

            if (!getKeyBindItem(MenuGenerator.comboOptions, "com.ilucian.combo.forceR")
                && !ObjectManager.Player.HasBuff("LucianR"))
            {
                Orbwalker.DisableAttacking = false;
            }

            if (getCheckBoxItem(MenuGenerator.miscOptions, "com.ilucian.misc.forcePassive") && Variables.HasPassive())
            {
                var target = TargetSelector.GetTarget(ObjectManager.Player.AttackRange, DamageType.Physical);
                if (target != null && target.IsValid)
                {
                    Orbwalker.ForcedTarget = target;
                }
            }
            else
            {
                Orbwalker.ForcedTarget = null;
            }

            AutoHarass();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                OnCombo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                OnHarass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                OnLaneclear();
            }
        }

        public void AutoHarass()
        {
            if (!getKeyBindItem(MenuGenerator.harassOptions, "com.ilucian.harass.auto.autoharass")
               || ObjectManager.Player.ManaPercent
               < getSliderItem(MenuGenerator.harassOptions, "com.ilucian.harass.auto.autoharass.mana")) return;

            var ch = HeroManager.Enemies.Where(hero => hero.LSIsValidTarget(1150)).Where(hero => hero.LSDistance(ObjectManager.Player) > 675).FirstOrDefault();

            var target = TargetSelector.GetTarget(Variables.Spell[Variables.Spells.Q2].Range, DamageType.Physical);

            if (getCheckBoxItem(MenuGenerator.harassOptions, "com.ilucian.harass.auto.q") && Variables.Spell[Variables.Spells.Q].IsReady())
            {
                if (Variables.Spell[Variables.Spells.Q].IsReady() &&
                    Variables.Spell[Variables.Spells.Q].IsInRange(target) && target.LSIsValidTarget())
                {
                    Variables.Spell[Variables.Spells.Q].Cast(target);
                }
            }

            if (getCheckBoxItem(MenuGenerator.harassOptions, "com.ilucian.harass.auto.qExtended") &&
                Variables.Spell[Variables.Spells.Q].IsReady())
            {
                CastExtendedQ();
            }
        }

        public void UltimateLock()
        {
            var currentTarget = TargetSelector.SelectedTarget;
            if (currentTarget.LSIsValidTarget())
            {
                var predictedPosition = Variables.Spell[Variables.Spells.R].GetPrediction(currentTarget).UnitPosition;
                var directionVector = (currentTarget.ServerPosition - ObjectManager.Player.ServerPosition).LSNormalized();
                const float rRangeCoefficient = 0.95f;
                var rRangeAdjusted = Variables.Spell[Variables.Spells.R].Range * rRangeCoefficient;
                var rEndPointXCoordinate = predictedPosition.X + directionVector.X * rRangeAdjusted;
                var rEndPointYCoordinate = predictedPosition.Y + directionVector.Y * rRangeAdjusted;
                var rEndPoint = new Vector2(rEndPointXCoordinate, rEndPointYCoordinate).To3D();

                if (rEndPoint.LSDistance(ObjectManager.Player.ServerPosition) < Variables.Spell[Variables.Spells.R].Range)
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, rEndPoint);
                }
            }
        }

        private void OnCombo()
        {
            var ch = HeroManager.Enemies.Where(hero => hero.LSIsValidTarget(1150)).Where(hero => hero.LSDistance(ObjectManager.Player) > 675).FirstOrDefault();

            var target = TargetSelector.GetTarget(Variables.Spell[Variables.Spells.Q2].Range, DamageType.Physical);


            if (target == null || Variables.HasPassive()
                || ObjectManager.Player.ManaPercent
                 < getSliderItem(MenuGenerator.harassOptions, "com.ilucian.harass.auto.autoharass.mana")) return;
            if (getCheckBoxItem(MenuGenerator.comboOptions, "com.ilucian.combo.qExtended"))
            {
                CastExtendedQ();
            }
            if (target.LSIsValidTarget(Variables.Spell[Variables.Spells.Q].Range) && getCheckBoxItem(MenuGenerator.comboOptions, "com.ilucian.combo.q"))
            {
                if (Variables.Spell[Variables.Spells.Q].IsReady() && Variables.Spell[Variables.Spells.Q].IsInRange(target))
                {
                    Variables.Spell[Variables.Spells.Q].Cast(target);
                }
            }
            if (ObjectManager.Player.LSIsDashing() || !getCheckBoxItem(MenuGenerator.comboOptions, "com.ilucian.combo.w")) return;
            if (!Variables.Spell[Variables.Spells.W].IsReady()) return;

            if (getCheckBoxItem(MenuGenerator.miscOptions, "com.ilucian.misc.usePrediction"))
            {
                var prediction = Variables.Spell[Variables.Spells.W].GetPrediction(target);
                if (prediction.Hitchance >= HitChance.High)
                {
                    Variables.Spell[Variables.Spells.W].Cast(prediction.CastPosition);
                }
            }
            else
            {
                if (target.LSDistance(ObjectManager.Player) < 600)
                {
                    Variables.Spell[Variables.Spells.W].Cast(target.Position);
                }
            }
        }

        private void OnHarass()
        {
            var target = TargetSelector.GetTarget(Variables.Spell[Variables.Spells.Q].Range, DamageType.Physical);

            if (target == null || Variables.HasPassive()
                || ObjectManager.Player.ManaPercent
                < getSliderItem(MenuGenerator.harassOptions, "com.ilucian.harass.mana"))
                return;
            if (getCheckBoxItem(MenuGenerator.harassOptions, "com.ilucian.harass.qExtended"))
            {
                CastExtendedQ();
            }

            if (target.LSIsValidTarget(Variables.Spell[Variables.Spells.Q].Range) && getCheckBoxItem(MenuGenerator.harassOptions, "com.ilucian.harass.q"))
            {
                if (Variables.Spell[Variables.Spells.Q].IsReady() && Variables.Spell[Variables.Spells.Q].IsInRange(target))
                {
                    Variables.Spell[Variables.Spells.Q].Cast(target);
                }
            }
            if (!ObjectManager.Player.LSIsDashing() && getCheckBoxItem(MenuGenerator.harassOptions, "com.ilucian.harass.w"))
            {
                if (Variables.Spell[Variables.Spells.W].IsReady())
                {
                    if (getCheckBoxItem(MenuGenerator.miscOptions, "com.ilucian.misc.usePrediction"))
                    {
                        var prediction = Variables.Spell[Variables.Spells.W].GetPrediction(target);
                        if (prediction.Hitchance >= HitChance.High)
                        {
                            Variables.Spell[Variables.Spells.W].Cast(prediction.CastPosition);
                        }
                    }
                    else
                    {
                        if (target.LSDistance(ObjectManager.Player) < 600)
                        {
                            Variables.Spell[Variables.Spells.W].Cast(target.Position);
                        }
                    }

                }
            }
        }

        public void SemiUlt()
        {
            var target = TargetSelector.SelectedTarget != null ? TargetSelector.SelectedTarget : TargetSelector.GetTarget(Variables.Spell[Variables.Spells.R].Range, DamageType.Physical);
            if (target == null)
            {
                return;
            }
            if (target.IsValid && Variables.Spell[Variables.Spells.R].IsReady() && !ObjectManager.Player.HasBuff("LucianR") && !target.IsDead && !target.IsZombie)
            {
                Variables.Spell[Variables.Spells.R].Cast(target.Position);
            }
        }

        private void OnLaneclear()
        {
            if (!getCheckBoxItem(MenuGenerator.laneclearOptions, "com.ilucian.laneclear.q")
                || ObjectManager.Player.ManaPercent
                < getSliderItem(MenuGenerator.laneclearOptions, "com.ilucian.laneclear.mana"))
                return;

            foreach (var minion in
                MinionManager.GetMinions(
                    ObjectManager.Player.ServerPosition,
                    Variables.Spell[Variables.Spells.Q].Range,
                    MinionTypes.All,
                    MinionTeam.NotAlly))
            {
                var prediction = LeagueSharp.Common.Prediction.GetPrediction(
                    minion,
                    Variables.Spell[Variables.Spells.Q].Delay,
                    ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).SData.CastRadius);

                var collision = Variables.Spell[Variables.Spells.Q].GetCollision(
                    ObjectManager.Player.Position.LSTo2D(),
                    new List<Vector2> { prediction.UnitPosition.LSTo2D() });

                foreach (var cs in collision)
                {
                    if (collision.Count < getSliderItem(MenuGenerator.laneclearOptions, "com.ilucian.laneclear.qMinions")) continue;
                    if (collision.Last().LSDistance(ObjectManager.Player) - collision[0].LSDistance(ObjectManager.Player)
                        <= 600 && collision[0].LSDistance(ObjectManager.Player) <= 500)
                    {
                        Variables.Spell[Variables.Spells.Q].Cast(cs);
                    }
                }
            }
        }

        private void CastE(Obj_AI_Base target)
        {
            if (!Variables.Spell[Variables.Spells.E].IsReady() || !getCheckBoxItem(MenuGenerator.comboOptions, "com.ilucian.combo.e") || ObjectManager.Player.HasBuff("LucianR") || Variables.HasPassive())
            {
                return;
            }

            var dashRange = getSliderItem(MenuGenerator.comboOptions, "com.ilucian.combo.eRange");

            switch (getBoxItem(MenuGenerator.comboOptions, "com.ilucian.combo.eMode"))
            {
                case 0: // kite
                    var hypotheticalPosition = ObjectManager.Player.ServerPosition.LSExtend(Game.CursorPos,
                        Variables.Spell[Variables.Spells.E].Range);
                    if (ObjectManager.Player.HealthPercent <= 70 &&
                        target.HealthPercent >= ObjectManager.Player.HealthPercent)
                    {
                        if (ObjectManager.Player.Position.LSDistance(ObjectManager.Player.ServerPosition) >= 35 &&
                            target.LSDistance(ObjectManager.Player.ServerPosition) <
                            target.LSDistance(ObjectManager.Player.Position) &&
                            hypotheticalPosition.IsSafe(Variables.Spell[Variables.Spells.E].Range))
                        {
                            Variables.Spell[Variables.Spells.E].Cast(hypotheticalPosition);
                        }
                    }

                    if (hypotheticalPosition.IsSafe(Variables.Spell[Variables.Spells.E].Range) &&
                        hypotheticalPosition.LSDistance(target.ServerPosition) <= Orbwalking.GetRealAutoAttackRange(null) &&
                        (hypotheticalPosition.LSDistance(target.ServerPosition) > 400) && !Variables.HasPassive())
                    {
                        Variables.Spell[Variables.Spells.E].Cast(hypotheticalPosition);
                    }
                    break;

                case 1: // side
                    Variables.Spell[Variables.Spells.E].Cast(
                        Deviation(ObjectManager.Player.Position.LSTo2D(), target.Position.LSTo2D(), dashRange).To3D());
                    break;

                case 2: //Cursor
                    if (Game.CursorPos.IsSafe(475))
                    {
                        Variables.Spell[Variables.Spells.E].Cast(ObjectManager.Player.Position.LSExtend(Game.CursorPos,
                            dashRange));
                    }
                    break;

                case 3: // Enemy
                    Variables.Spell[Variables.Spells.E].Cast(ObjectManager.Player.Position.LSExtend(target.Position, dashRange));
                    break;
                case 4:
                    Variables.Spell[Variables.Spells.E].Cast(
                        Deviation(ObjectManager.Player.Position.LSTo2D(), target.Position.LSTo2D(), 65f).To3D());
                    break;
                case 5: // Smart E Credits to ASUNOOO
                    var ePosition = new EPosition();
                    var bestPosition = ePosition.GetEPosition();
                    if (bestPosition != Vector3.Zero
                        && bestPosition.LSDistance(target.ServerPosition) < Orbwalking.GetRealAutoAttackRange(target))
                    {
                        Variables.Spell[Variables.Spells.E].Cast(bestPosition);
                    }
                    break;
                case 6: // URF
                    Variables.Spell[Variables.Spells.E].Cast(ObjectManager.Player.Position.LSExtend(Game.CursorPos, 475));
                    break;
            }
        }

        public static float GetComboDamage(Obj_AI_Base target)
        {
            float damage = 0;
            if (Variables.Spell[Variables.Spells.Q].IsReady())
                damage = damage + Variables.Spell[Variables.Spells.Q].GetDamage(target) +
                         (float)ObjectManager.Player.GetAutoAttackDamage(target);
            if (Variables.Spell[Variables.Spells.W].IsReady())
                damage = damage + Variables.Spell[Variables.Spells.W].GetDamage(target) +
                         (float)ObjectManager.Player.GetAutoAttackDamage(target);
            if (Variables.Spell[Variables.Spells.E].IsReady())
                damage = damage + (float)ObjectManager.Player.GetAutoAttackDamage(target) * 2;

            damage = (float)(damage + ObjectManager.Player.GetAutoAttackDamage(target));

            return damage;
        }

        private void CastExtendedQ()
        {
            if (!Variables.Spell[Variables.Spells.Q].IsReady())
            {
                return;
            }

            var target = TargetSelector.SelectedTarget != null
                         && TargetSelector.SelectedTarget.LSDistance(ObjectManager.Player) < 1800
                             ? TargetSelector.SelectedTarget
                             : TargetSelector.GetTarget(
                                 Variables.Spell[Variables.Spells.Q2].Range,
                                 DamageType.Physical);

            var predictionPosition = Variables.Spell[Variables.Spells.Q2].GetPrediction(target);

            foreach (var unit in from unit in GetHittableTargets()
                                 let polygon =
                                     new LeagueSharp.Common.Geometry.Polygon.Rectangle(
                                     ObjectManager.Player.ServerPosition,
                                     ObjectManager.Player.ServerPosition.LSExtend(
                                         unit.ServerPosition,
                                         Variables.Spell[Variables.Spells.Q2].Range),
                                     65f)
                                 where polygon.IsInside(predictionPosition.CastPosition)
                                 select unit)
            {
                Variables.Spell[Variables.Spells.Q].Cast(unit);
            }
        }

        public List<Obj_AI_Base> GetHittableTargets()
        {
            var unitList = new List<Obj_AI_Base>();
            var minions = MinionManager.GetMinions(
                ObjectManager.Player.Position,
                Variables.Spell[Variables.Spells.Q].Range);
            var champions =
                HeroManager.Enemies.Where(
                    x =>
                    ObjectManager.Player.LSDistance(x) <= Variables.Spell[Variables.Spells.Q].Range
                    && !x.HasBuffOfType(BuffType.SpellShield)
                    && !getCheckBoxItem(MenuGenerator.harassOptions, "com.ilucian.harass.whitelist." + x.ChampionName.ToLower()));

            unitList.AddRange(minions);

            /*if (Variables.Menu.IsEnabled("com.ilucian.misc.LSExtendChamps"))
            {
                unitList.AddRange(champions);
            }*/

            return unitList;
        }

        /// <summary>
        ///     Credits to Myo, stolen from him, ily :^)
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public Vector2 Deviation(Vector2 point1, Vector2 point2, double angle)
        {
            angle *= Math.PI / 180.0;
            var temp = Vector2.Subtract(point2, point1);
            var result = new Vector2(0)
            {
                X = (float)(temp.X * Math.Cos(angle) - temp.Y * Math.Sin(angle)) / 4,
                Y = (float)(temp.X * Math.Sin(angle) + temp.Y * Math.Cos(angle)) / 4
            };
            result = Vector2.Add(result, point1);
            return result;
        }
    }
}