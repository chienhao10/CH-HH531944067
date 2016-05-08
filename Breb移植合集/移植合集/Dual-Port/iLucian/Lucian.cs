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
            DZAntigapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
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

        private static void OnEnemyGapcloser(DZLib.Core.ActiveGapcloser gapcloser)
        {
            if (!getCheckBoxItem(MenuGenerator.miscOptions, "com.ilucian.misc.gapcloser"))
            {
                return;
            }

            if (!gapcloser.Sender.IsEnemy || !(gapcloser.End.Distance(ObjectManager.Player.ServerPosition) < 350))
                return;

            var extendedPosition = ObjectManager.Player.ServerPosition.LSExtend(Game.CursorPos,
                Variables.Spell[Variables.Spells.E].Range);
            if (extendedPosition.IsSafe(Variables.Spell[Variables.Spells.E].Range) &&
                extendedPosition.CountAlliesInRange(650f) > 0)
            {
                Variables.Spell[Variables.Spells.E].Cast(extendedPosition);
            }
        }

        private void OnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (args.SData.Name != "LucianPassiveShot" && !args.SData.Name.Contains("LucianBasicAttack"))
                return;
            if (Variables.HasPassive)
                return;

            var target = TargetSelector.GetTarget(Variables.Spell[Variables.Spells.Q].Range,
                DamageType.Physical);
            if (target == null)
                return;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (target.IsValidTarget(Variables.Spell[Variables.Spells.Q].Range) &&
                    getCheckBoxItem(MenuGenerator.comboOptions, "com.ilucian.combo.q"))
                {
                    if (Variables.Spell[Variables.Spells.Q].IsReady() &&
                        Variables.Spell[Variables.Spells.Q].IsInRange(target))
                    {
                        Variables.Spell[Variables.Spells.Q].Cast(target);
                    }
                }
                if (!ObjectManager.Player.LSIsDashing() &&
                    getCheckBoxItem(MenuGenerator.comboOptions, "com.ilucian.combo.w"))
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
                    }
                    else
                    {
                        if (target.Distance(ObjectManager.Player) < 600)
                        {
                            Variables.Spell[Variables.Spells.W].Cast(target.Position);
                        }
                    }
                }
                CastE(target);
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                if (target.IsValidTarget(Variables.Spell[Variables.Spells.Q].Range) &&
                    getCheckBoxItem(MenuGenerator.harassOptions, "com.ilucian.harass.q"))
                {
                    if (Variables.Spell[Variables.Spells.Q].IsReady() &&
                        Variables.Spell[Variables.Spells.Q].IsInRange(target))
                    {
                        Variables.Spell[Variables.Spells.Q].Cast(target);
                    }
                }
                if (!ObjectManager.Player.LSIsDashing() &&
                    getCheckBoxItem(MenuGenerator.harassOptions, "com.ilucian.harass.w"))
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
                    }
                    else
                    {
                        if (target.Distance(ObjectManager.Player) < 600)
                        {
                            Variables.Spell[Variables.Spells.W].Cast(target.Position);
                        }
                    }
                }
            }
        }

        private void LoadSpells()
        {
            Variables.Spell[Variables.Spells.Q].SetTargetted(0.25f, 1400f);
            Variables.Spell[Variables.Spells.Q2].SetSkillshot(0.5f, 50, float.MaxValue, false,
                SkillshotType.SkillshotLine);
            Variables.Spell[Variables.Spells.W].SetSkillshot(0.30f, 70f, 1600f, true, SkillshotType.SkillshotLine);
            Variables.Spell[Variables.Spells.R].SetSkillshot(0.2f, 110f, 2500, true, SkillshotType.SkillshotLine);
        }

        public void Killsteal()
        {
            var target = TargetSelector.GetTarget(Variables.Spell[Variables.Spells.E].Range + Variables.Spell[Variables.Spells.Q2].Range, DamageType.Physical);

            if (!getCheckBoxItem(MenuGenerator.miscOptions, "com.ilucian.misc.eqKs") || !Variables.Spell[Variables.Spells.Q].IsReady() || !target.IsValidTarget(Variables.Spell[Variables.Spells.E].Range + Variables.Spell[Variables.Spells.Q2].Range))
            {
                return;
            }

            if (Variables.Spell[Variables.Spells.Q].GetDamage(target) - 20 >= target.Health)
            {
                if (target.IsValidTarget(Variables.Spell[Variables.Spells.Q].Range))
                {
                    Variables.Spell[Variables.Spells.Q].Cast(target);
                }

                if (target.IsValidTarget(Variables.Spell[Variables.Spells.Q2].Range) && !target.IsValidTarget(Variables.Spell[Variables.Spells.Q].Range))
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

            if (!target.IsValidTarget(Variables.Spell[Variables.Spells.E].Range + Variables.Spell[Variables.Spells.Q2].Range))
                return;

            var dashSpeed = (int)(Variables.Spell[Variables.Spells.E].Range / (700 + ObjectManager.Player.MoveSpeed));
            var extendedPrediction = GetExtendedPrediction(target, dashSpeed);

            var minions = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsEnemy && x.IsValid && x.Distance(extendedPrediction, true) < 900 * 900).OrderByDescending(x => x.Distance(extendedPrediction));

            foreach (var minion in
                minions.Select(x => LeagueSharp.Common.Prediction.GetPrediction(x, dashSpeed)).Select(pred => MathHelper.GetCicleLineInteraction(pred.UnitPosition.To2D(), extendedPrediction.To2D(), ObjectManager.Player.ServerPosition.To2D(), Variables.Spell[Variables.Spells.E].Range)).Select(inter => inter.GetBestInter(target)))
            {
                if (Math.Abs(minion.X) < 1)
                    return;

                if (!NavMesh.GetCollisionFlags(minion.To3D()).HasFlag(CollisionFlags.Wall) && !NavMesh.GetCollisionFlags(minion.To3D()).HasFlag(CollisionFlags.Building) && minion.To3D().IsSafe(Variables.Spell[Variables.Spells.E].Range))
                {
                    Console.WriteLine("EQ KILLSTEAL THO");
                    Variables.Spell[Variables.Spells.E].Cast((Vector3)minion);
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

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                OnCombo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                OnHarass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                OnLaneclear();
            }
        }

        private void OnCombo()
        {
            var target = TargetSelector.GetTarget(Variables.Spell[Variables.Spells.Q2].Range, DamageType.Physical);

            if (target == null || Variables.HasPassive)
                return;
            if (getCheckBoxItem(MenuGenerator.comboOptions, "com.ilucian.combo.qExtended"))
            {
                CastExtendedQ();
            }
            if (target.IsValidTarget(Variables.Spell[Variables.Spells.Q].Range) && getCheckBoxItem(MenuGenerator.comboOptions, "com.ilucian.combo.q"))
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
                if (target.Distance(ObjectManager.Player) < 600)
                {
                    Variables.Spell[Variables.Spells.W].Cast(target.Position);
                }
            }
        }

        private void OnHarass()
        {
            var target = TargetSelector.GetTarget(Variables.Spell[Variables.Spells.Q].Range, DamageType.Physical);

            if (target == null || Variables.HasPassive)
                return;
            if (getCheckBoxItem(MenuGenerator.harassOptions, "com.ilucian.harass.qExtended"))
            {
                CastExtendedQ();
            }
            if (target.IsValidTarget(Variables.Spell[Variables.Spells.Q].Range) && getCheckBoxItem(MenuGenerator.harassOptions, "com.ilucian.harass.q"))
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
                        if (target.Distance(ObjectManager.Player) < 600)
                        {
                            Variables.Spell[Variables.Spells.W].Cast(target.Position);
                        }
                    }
                }
            }
        }

        private void OnLaneclear()
        {
            if (getCheckBoxItem(MenuGenerator.laneclearOptions, "com.ilucian.laneclear.q"))
            {
                var minions = MinionManager.GetMinions(Variables.Spell[Variables.Spells.Q].Range);
                var bestLocation = Variables.Spell[Variables.Spells.Q].GetCircularFarmLocation(minions, 60);

                if (bestLocation.MinionsHit < getSliderItem(MenuGenerator.laneclearOptions, "com.ilucian.laneclear.qMinions"))
                    return;
                var adjacentMinions = minions.Where(m => m.Distance(bestLocation.Position) <= 45).ToList();
                if (!adjacentMinions.Any())
                {
                    return;
                }

                var firstMinion = adjacentMinions.OrderBy(m => m.Distance(bestLocation.Position)).First();

                if (!firstMinion.IsValidTarget(Variables.Spell[Variables.Spells.Q].Range))
                    return;
                if (!Variables.HasPassive && Orbwalking.InAutoAttackRange(firstMinion))
                {
                    Variables.Spell[Variables.Spells.Q].Cast(firstMinion);
                }
            }
        }

        private void CastE(Obj_AI_Base target)
        {
            if (!Variables.Spell[Variables.Spells.E].IsReady() || !getCheckBoxItem(MenuGenerator.comboOptions, "com.ilucian.combo.e") || target == null)
            {
                return;
            }

            if (HeroManager.Player.HasBuff("AwesomeBuff"))
            {
                var extendedPosition = ObjectManager.Player.ServerPosition.LSExtend(Game.CursorPos, Variables.Spell[Variables.Spells.E].Range);
                if (extendedPosition.IsSafe(Variables.Spell[Variables.Spells.E].Range))
                {
                    Variables.Spell[Variables.Spells.E].Cast(Game.CursorPos);
                }
                return;
            }

            switch (getBoxItem(MenuGenerator.comboOptions, "com.ilucian.combo.eMode"))
            {
                case 0: // kite
                    var hypotheticalPosition = ObjectManager.Player.ServerPosition.LSExtend(Game.CursorPos,
                        Variables.Spell[Variables.Spells.E].Range);
                    if (ObjectManager.Player.HealthPercent <= 70 &&
                        target.HealthPercent >= ObjectManager.Player.HealthPercent)
                    {
                        if (ObjectManager.Player.Position.Distance(ObjectManager.Player.ServerPosition) >= 35 &&
                            target.Distance(ObjectManager.Player.ServerPosition) <
                            target.Distance(ObjectManager.Player.Position) &&
                            hypotheticalPosition.IsSafe(Variables.Spell[Variables.Spells.E].Range))
                        {
                            Variables.Spell[Variables.Spells.E].Cast(hypotheticalPosition);
                        }
                    }

                    if (hypotheticalPosition.IsSafe(Variables.Spell[Variables.Spells.E].Range) &&
                        hypotheticalPosition.Distance(target.ServerPosition) <= Orbwalking.GetRealAutoAttackRange(null) &&
                        (hypotheticalPosition.Distance(target.ServerPosition) > 400) && !Variables.HasPassive)
                    {
                        Variables.Spell[Variables.Spells.E].Cast(hypotheticalPosition);
                    }
                    break;

                case 1: // side
                    Variables.Spell[Variables.Spells.E].Cast(
                        Deviation(ObjectManager.Player.Position.To2D(), target.Position.To2D(), 65).To3D());
                    break;

                case 2: //Cursor
                    if (Game.CursorPos.IsSafe(475))
                    {
                        Variables.Spell[Variables.Spells.E].Cast(ObjectManager.Player.Position.Extend(Game.CursorPos,
                            65f));
                    }
                    break;

                case 3: // Enemy
                    Variables.Spell[Variables.Spells.E].Cast(ObjectManager.Player.Position.Extend(target.Position, 400));
                    break;
            }
        }

        private void CastExtendedQ()
        {
            if (!Variables.Spell[Variables.Spells.Q].IsReady())
            {
                return;
            }
            var target = TargetSelector.SelectedTarget != null && TargetSelector.SelectedTarget.Distance(ObjectManager.Player) < 1800 ? TargetSelector.SelectedTarget : TargetSelector.GetTarget(Variables.Spell[Variables.Spells.Q2].Range, DamageType.Physical);
            var predictionPosition = Variables.Spell[Variables.Spells.Q2].GetPrediction(target);
            var minions = MinionManager.GetMinions(ObjectManager.Player.Position, Variables.Spell[Variables.Spells.Q].Range);

            foreach (var minion in from minion in minions let polygon = new LeagueSharp.Common.Geometry.Polygon.Rectangle(ObjectManager.Player.ServerPosition, ObjectManager.Player.ServerPosition.LSExtend(minion.ServerPosition, Variables.Spell[Variables.Spells.Q2].Range), 65f) where polygon.IsInside(predictionPosition.CastPosition) select minion)
            {
                Variables.Spell[Variables.Spells.Q].Cast(minion);
            }
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