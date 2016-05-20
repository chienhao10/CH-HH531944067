#region

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
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

#endregion

namespace Ziggs
{
    internal class Program
    {
        public static string ChampionName = "Ziggs";
        public static List<Spell> SpellList = new List<Spell>();
        public static Spell Q1;
        public static Spell Q2;
        public static Spell Q3;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Menu Config;

        public static int LastWToMouseT;
        public static int UseSecondWT;

        public static Menu comboMenu, harassMenu, farmMenu, jungleMenu, miscMenu, drawMenu;

        public static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName != ChampionName)
            {
                return;
            }

            Q1 = new Spell(SpellSlot.Q, 850f);
            Q2 = new Spell(SpellSlot.Q, 1125f);
            Q3 = new Spell(SpellSlot.Q, 1400f);

            W = new Spell(SpellSlot.W, 1000f);
            E = new Spell(SpellSlot.E, 900f);
            R = new Spell(SpellSlot.R, 5300f);

            Q1.SetSkillshot(0.3f, 130f, 1700f, false, SkillshotType.SkillshotCircle);
            Q2.SetSkillshot(0.25f + Q1.Delay, 130f, 1700f, false, SkillshotType.SkillshotCircle);
            Q3.SetSkillshot(0.3f + Q2.Delay, 130f, 1700f, false, SkillshotType.SkillshotCircle);

            W.SetSkillshot(0.25f, 275f, 1750f, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.5f, 100f, 1750f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(1f, 500f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            SpellList.Add(Q1);
            SpellList.Add(Q2);
            SpellList.Add(Q3);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            Config = MainMenu.AddMenu(ChampionName, ChampionName);

            comboMenu = Config.AddSubMenu("Combo", "Combo");
            comboMenu.Add("UseQCombo", new CheckBox("Use Q"));
            comboMenu.Add("UseWCombo", new CheckBox("Use W"));
            comboMenu.Add("UseECombo", new CheckBox("Use E"));
            comboMenu.Add("UseRCombo", new CheckBox("Use R"));

            harassMenu = Config.AddSubMenu("Harass", "Harass");
            harassMenu.Add("UseQHarass", new CheckBox("Use Q"));
            harassMenu.Add("UseWHarass", new CheckBox("Use W", false));
            harassMenu.Add("UseEHarass", new CheckBox("Use E", false));
            harassMenu.Add("ManaSliderHarass", new Slider("Mana To Harass", 50));

            farmMenu = Config.AddSubMenu("Farm", "Farm");
            farmMenu.Add("UseQFarm", new CheckBox("Use Q"));
            farmMenu.Add("UseWFarm", new CheckBox("Use W"));
            farmMenu.Add("UseEFarm", new CheckBox("Use E"));
            farmMenu.Add("ManaSliderFarm", new Slider("Mana To Farm", 25));

            jungleMenu = Config.AddSubMenu("JungleFarm", "JungleFarm");
            jungleMenu.Add("UseQJFarm", new CheckBox("Use Q"));
            jungleMenu.Add("UseEJFarm", new CheckBox("Use E"));

            miscMenu = Config.AddSubMenu("Misc", "Misc");
            miscMenu.Add("WToMouse", new KeyBind("W to mouse", false, KeyBind.BindTypes.HoldActive, 'T'));
            miscMenu.Add("Peel", new CheckBox("Use W defensively"));

            drawMenu = Config.AddSubMenu("Drawings", "Drawings");
            drawMenu.Add("DrawQRange", new CheckBox("Draw Q Range"));
                //.SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
            drawMenu.Add("DrawWRange", new CheckBox("Draw W Range"));
                //.SetValue(new Circle(true, Color.FromArgb(100, 255, 255, 255))));
            drawMenu.Add("DrawERange", new CheckBox("Draw E Range"));
                //.SetValue(new Circle(false, Color.FromArgb(100, 255, 255, 255))));
            drawMenu.Add("DrawRRange", new CheckBox("Draw R Range"));
                //.SetValue(new Circle(false, Color.FromArgb(100, 255, 255, 255))));
            drawMenu.Add("DrawRRangeM", new CheckBox("Draw R Range (Minimap)"));
                //.SetValue(new Circle(false, Color.FromArgb(100, 255, 255, 255))));

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;

            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
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

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsAlly)
            {
                return;
            }

            W.Cast(sender);
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            W.Cast(gapcloser.Sender);
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            var qValue = getCheckBoxItem(drawMenu, "DrawQRange");
            if (qValue)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q3.Range, Color.FromArgb(100, 255, 0, 255));
            }

            var wValue = getCheckBoxItem(drawMenu, "DrawWRange");
            if (wValue)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, Color.FromArgb(100, 255, 255, 255));
            }

            var eValue = getCheckBoxItem(drawMenu, "DrawERange");
            if (eValue)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, Color.FromArgb(100, 255, 255, 255));
            }

            var rValue = getCheckBoxItem(drawMenu, "DrawRRange");
            if (rValue)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, Color.FromArgb(100, 255, 255, 255));
            }

            var rValueM = getCheckBoxItem(drawMenu, "DrawRRangeM");
            if (rValueM)
            {
                Utility.DrawCircle(ObjectManager.Player.Position, R.Range, Color.FromArgb(100, 255, 255, 255), 2, 30,
                    true);
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            //Combo & Harass
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) ||
                (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) &&
                 ObjectManager.Player.Mana/ObjectManager.Player.MaxMana*100 >
                 getSliderItem(harassMenu, "ManaSliderHarass")))
            {
                var target = TargetSelector.GetTarget(1200f, DamageType.Magical);
                if (target != null)
                {
                    var comboActive = Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
                    var harassActive = Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass);

                    if (((comboActive && getCheckBoxItem(comboMenu, "UseQCombo")) ||
                         (harassActive && getCheckBoxItem(harassMenu, "UseQHarass"))) && Q1.IsReady())
                    {
                        CastQ(target);
                    }

                    if (((comboActive && getCheckBoxItem(comboMenu, "UseWCombo")) ||
                         (harassActive && getCheckBoxItem(harassMenu, "UseWHarass"))) && W.IsReady())
                    {
                        var prediction = W.GetPrediction(target);
                        if (prediction.Hitchance >= HitChance.High)
                        {
                            if (ObjectManager.Player.ServerPosition.LSDistance(prediction.UnitPosition) < W.Range &&
                                ObjectManager.Player.ServerPosition.LSDistance(prediction.UnitPosition) > W.Range - 250 &&
                                prediction.UnitPosition.LSDistance(ObjectManager.Player.ServerPosition) >
                                target.LSDistance(ObjectManager.Player))
                            {
                                var cp =
                                    ObjectManager.Player.ServerPosition.To2D()
                                        .Extend(prediction.UnitPosition.To2D(), W.Range)
                                        .To3D();
                                W.Cast(cp);
                                UseSecondWT = Utils.TickCount;
                            }
                        }
                    }

                    if (((comboActive && getCheckBoxItem(comboMenu, "UseECombo")) ||
                         (harassActive && getCheckBoxItem(harassMenu, "UseEHarass"))) && E.IsReady())
                    {
                        E.Cast(target, false, true);
                    }

                    var useR = getCheckBoxItem(comboMenu, "UseRCombo");

                    //R at close range
                    if (comboActive && useR && R.IsReady() &&
                        (ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q) +
                         ObjectManager.Player.GetSpellDamage(target, SpellSlot.W) +
                         ObjectManager.Player.GetSpellDamage(target, SpellSlot.E) +
                         ObjectManager.Player.GetSpellDamage(target, SpellSlot.R) > target.Health) &&
                        ObjectManager.Player.LSDistance(target) <= Q2.Range)
                    {
                        R.Delay = 2000 + 1500*target.LSDistance(ObjectManager.Player)/5300;
                        R.Cast(target, true, true);
                    }

                    //R aoe in teamfights
                    if (comboActive && useR && R.IsReady())
                    {
                        var alliesarround = 0;
                        var n = 0;
                        foreach (var ally in ObjectManager.Get<AIHeroClient>())
                        {
                            if (ally.IsAlly && !ally.IsMe && ally.IsValidTarget(float.MaxValue) &&
                                ally.LSDistance(target) < 700)
                            {
                                alliesarround++;
                                if (Utils.TickCount - ally.LastCastedSpellT() < 1500)
                                {
                                    n++;
                                }
                            }
                        }

                        if (n < Math.Max(alliesarround/2 - 1, 1))
                        {
                            return;
                        }

                        switch (alliesarround)
                        {
                            case 2:
                                R.CastIfWillHit(target, 2);
                                break;
                            case 3:
                                R.CastIfWillHit(target, 3);
                                break;
                            case 4:
                                R.CastIfWillHit(target, 4);
                                break;
                        }
                    }

                    //R if killable
                    if (comboActive && useR && R.IsReady() &&
                        ObjectManager.Player.GetSpellDamage(target, SpellSlot.R) > target.Health)
                    {
                        R.Delay = 2000 + 1500*target.LSDistance(ObjectManager.Player)/5300;
                        R.Cast(target, true, true);
                    }
                }
            }

            if (Utils.TickCount - UseSecondWT < 500 &&
                ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "ziggswtoggle")
            {
                W.Cast(ObjectManager.Player.ServerPosition, true);
            }

            //Farm
            var lc = Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                     Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear);
            if (lc)
            {
                Farm();
                JungleFarm();
            }

            //W to mouse
            var castToMouse = getKeyBindItem(miscMenu, "WToMouse");
            if (castToMouse || Utils.TickCount - LastWToMouseT < 400)
            {
                var pos = ObjectManager.Player.ServerPosition.To2D().Extend(Game.CursorPos.To2D(), -150).To3D();
                W.Cast(pos, true);
                if (castToMouse)
                {
                    LastWToMouseT = Utils.TickCount;
                }
            }

            //Peel from melees
            if (getCheckBoxItem(miscMenu, "Peel"))
            {
                foreach (var pos in from enemy in ObjectManager.Get<AIHeroClient>()
                    where
                        enemy.IsValidTarget() &&
                        enemy.LSDistance(ObjectManager.Player) <=
                        enemy.BoundingRadius + enemy.AttackRange + ObjectManager.Player.BoundingRadius &&
                        enemy.IsMelee()
                    let direction =
                        (enemy.ServerPosition.To2D() - ObjectManager.Player.ServerPosition.To2D()).Normalized()
                    let pos = ObjectManager.Player.ServerPosition.To2D()
                    select pos + Math.Min(200, Math.Max(50, enemy.LSDistance(ObjectManager.Player)/2))*direction)
                {
                    W.Cast(pos.To3D(), true);
                    UseSecondWT = Utils.TickCount;
                }
            }
        }

        private static void CastQ(Obj_AI_Base target)
        {
            PredictionOutput prediction;

            if (ObjectManager.Player.LSDistance(target) < Q1.Range)
            {
                var oldrange = Q1.Range;
                Q1.Range = Q2.Range;
                prediction = Q1.GetPrediction(target, true);
                Q1.Range = oldrange;
            }
            else if (ObjectManager.Player.LSDistance(target) < Q2.Range)
            {
                var oldrange = Q2.Range;
                Q2.Range = Q3.Range;
                prediction = Q2.GetPrediction(target, true);
                Q2.Range = oldrange;
            }
            else if (ObjectManager.Player.LSDistance(target) < Q3.Range)
            {
                prediction = Q3.GetPrediction(target, true);
            }
            else
            {
                return;
            }

            if (prediction.Hitchance >= HitChance.High)
            {
                if (ObjectManager.Player.ServerPosition.LSDistance(prediction.CastPosition) <= Q1.Range + Q1.Width)
                {
                    Vector3 p;
                    if (ObjectManager.Player.ServerPosition.LSDistance(prediction.CastPosition) > 300)
                    {
                        p = prediction.CastPosition -
                            100*
                            (prediction.CastPosition.To2D() - ObjectManager.Player.ServerPosition.To2D()).Normalized()
                                .To3D();
                    }
                    else
                    {
                        p = prediction.CastPosition;
                    }

                    Q1.Cast(p);
                }
                else if (ObjectManager.Player.ServerPosition.LSDistance(prediction.CastPosition) <=
                         (Q1.Range + Q2.Range)/2)
                {
                    var p = ObjectManager.Player.ServerPosition.To2D()
                        .Extend(prediction.CastPosition.To2D(), Q1.Range - 100);

                    if (!CheckQCollision(target, prediction.UnitPosition, p.To3D()))
                    {
                        Q1.Cast(p.To3D());
                    }
                }
                else
                {
                    var p = ObjectManager.Player.ServerPosition.To2D() +
                            Q1.Range*
                            (prediction.CastPosition.To2D() - ObjectManager.Player.ServerPosition.To2D()).Normalized
                                ();

                    if (!CheckQCollision(target, prediction.UnitPosition, p.To3D()))
                    {
                        Q1.Cast(p.To3D());
                    }
                }
            }
        }

        private static bool CheckQCollision(Obj_AI_Base target, Vector3 targetPosition, Vector3 castPosition)
        {
            var direction = (castPosition.To2D() - ObjectManager.Player.ServerPosition.To2D()).Normalized();
            var firstBouncePosition = castPosition.To2D();
            var secondBouncePosition = firstBouncePosition +
                                       direction*0.4f*
                                       ObjectManager.Player.ServerPosition.To2D().LSDistance(firstBouncePosition);
            var thirdBouncePosition = secondBouncePosition +
                                      direction*0.6f*firstBouncePosition.LSDistance(secondBouncePosition);

            //TODO: Check for wall collision.

            if (thirdBouncePosition.LSDistance(targetPosition.To2D()) < Q1.Width + target.BoundingRadius)
            {
                //Check the second one.
                if ((from minion in ObjectManager.Get<Obj_AI_Minion>() where minion.IsValidTarget(3000) let predictedPos = Q2.GetPrediction(minion) where predictedPos.UnitPosition.To2D().LSDistance(secondBouncePosition) <
                                                                                                                                                          Q2.Width + minion.BoundingRadius select minion).Any())
                {
                    return true;
                }
            }

            if (secondBouncePosition.LSDistance(targetPosition.To2D()) < Q1.Width + target.BoundingRadius ||
                thirdBouncePosition.LSDistance(targetPosition.To2D()) < Q1.Width + target.BoundingRadius)
            {
                //Check the first one
                return (from minion in ObjectManager.Get<Obj_AI_Minion>() where minion.IsValidTarget(3000) let predictedPos = Q1.GetPrediction(minion) where predictedPos.UnitPosition.To2D().LSDistance(firstBouncePosition) < Q1.Width + minion.BoundingRadius select minion).Any();
            }

            return true;
        }

        private static void Farm()
        {
            if (getSliderItem(farmMenu, "ManaSliderFarm") >
                ObjectManager.Player.Mana/ObjectManager.Player.MaxMana*100)
            {
                return;
            }

            var rangedMinions = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition, Q2.Range, MinionTypes.Ranged);
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q2.Range);

            var useQi = getCheckBoxItem(farmMenu, "UseQFarm");
            var useWi = getCheckBoxItem(farmMenu, "UseWFarm");
            var useEi = getCheckBoxItem(farmMenu, "UseEFarm");
            var useQ = useQi;
            var useW = useWi;
            var useE = useEi;

            if (Q1.IsReady() && useQ)
            {
                var rangedLocation = Q2.GetCircularFarmLocation(rangedMinions);
                var location = Q2.GetCircularFarmLocation(allMinions);

                var bLocation = location.MinionsHit > rangedLocation.MinionsHit + 1 ? location : rangedLocation;

                if (bLocation.MinionsHit > 0)
                {
                    Q2.Cast(bLocation.Position.To3D());
                }
            }

            if (W.IsReady() && useW)
            {
                var dmgpct = new[] { 25, 27.5, 30, 32.5, 35 }[W.Level - 1];

                var killableTurret = ObjectManager.Get<Obj_AI_Turret>().Find(x => x.IsEnemy && ObjectManager.Player.LSDistance(x.Position) <= W.Range && x.HealthPercent < dmgpct);
                if (killableTurret != null)
                {
                    W.Cast(killableTurret.Position);
                }
            }

            if (E.IsReady() && useE)
            {
                var rangedLocation = E.GetCircularFarmLocation(rangedMinions, E.Width*2);
                var location = E.GetCircularFarmLocation(allMinions, E.Width*2);

                var bLocation = location.MinionsHit > rangedLocation.MinionsHit + 1 ? location : rangedLocation;

                if (bLocation.MinionsHit > 2)
                {
                    E.Cast(bLocation.Position.To3D());
                }
            }
            /*
            {
                if (useQ && Q1.IsReady())
                {
                    foreach (var minion in allMinions)
                    {
                        if (!Orbwalking.InAutoAttackRange(minion))
                        {
                            var Qdamage = ObjectManager.Player.GetSpellDamage(minion, SpellSlot.Q) * 0.75;

                            if (Qdamage > Q1.GetHealthPrediction(minion))
                            {
                                Q2.Cast(minion);
                            }
                        }
                    }
                }

                if (E.IsReady() && useE)
                {
                    var rangedLocation = E.GetCircularFarmLocation(rangedMinions, E.Width * 2);
                    var location = E.GetCircularFarmLocation(allMinions, E.Width * 2);

                    var bLocation = (location.MinionsHit > rangedLocation.MinionsHit + 1) ? location : rangedLocation;

                    if (bLocation.MinionsHit > 2)
                    {
                        E.Cast(bLocation.Position.To3D());
                    }
                }
            }
             */
        }

        private static void JungleFarm()
        {
            var useQ = getCheckBoxItem(jungleMenu, "UseQJFarm");
            var useE = getCheckBoxItem(jungleMenu, "UseEJFarm");

            var mobs = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition, Q1.Range, MinionTypes.All, MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);

            if (mobs.Count > 0)
            {
                var mob = mobs[0];

                if (useQ && Q1.IsReady())
                {
                    Q1.Cast(mob);
                }


                if (useE)
                {
                    E.Cast(mob);
                }
            }
        }
    }
}