using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using KoreanAIO.Managers;
using KoreanAIO.Model;
using SharpDX;
using Color = System.Drawing.Color;

namespace KoreanAIO.Champions
{
    public class Syndra : ChampionBase
    {
        private class Sphere
        {
            private const string BaseSkinName = "SyndraSphere";
            private const string MissileBuff = "SyndraESphereMissile";
            public static bool IsMySphere(Obj_AI_Base minion)
            {
                return minion != null && minion.Team == Player.Instance.Team && minion.Type == GameObjectType.obj_AI_Minion && minion.BaseSkinName == BaseSkinName;
            }
            public readonly Obj_AI_Base GameObject;

            public Sphere(Obj_AI_Base gameObject)
            {
                GameObject = gameObject;
            }
            public bool IsValid
            {
                get { return GameObject != null && GameObject.IsValid && GameObject.IsVisible && !GameObject.IsDead; }
            }
            public bool IsGrabbable
            {
                get { return GameObject.IsTargetable && !GameObject.HasBuff(MissileBuff); }
            }
        }

        private static class Spells
        {
            public static class Q
            {
                public const int Width = 180;
                public const int ReducedWidth = 120;
            }

            public static class W
            {
                public const int Width = 210;
                public const int ReducedWidth = 150;
                public const int TotalTravelTime = 910;
            }

            public static class E
            {
                public const int Level1Angle = 20;
                public const int Level5Angle = 30;
                public const int TotalTravelTime = 530;
                public const int ExtraRange = 40;
            }

            // ReSharper disable once InconsistentNaming
            public static class QE
            {
                public const int Speed = 2000;
                public const int CastDelay = 0;
                public const int Range = 1300;
            }

            public static class R
            {
                public const int Level1Range = 675;
                public const int Level5Range = 750;
            }
        }
        public bool IsW2
        {
            get
            {
                return W.Instance.Name.ToLower() == "syndrawcast";
            }
        }
        // ReSharper disable once InconsistentNaming
        private readonly SpellBase QE;
        private static readonly List<Sphere> Spheres = new List<Sphere>();
        private static readonly Dictionary<int, Dictionary<int, Vector3>> WillHitSpheres = new Dictionary<int, Dictionary<int, Vector3>>();
        public readonly Dictionary<int, Text> IsKillableOnEnemyPosition = new Dictionary<int, Text>();
        public readonly Dictionary<int, Text> IsKillableOnScreen = new Dictionary<int, Text>();
        private static readonly Vector2 BaseScreenPoint = new Vector2(100, 50);
        private const float TextScreenSize = 28F;
        private const float TextEnemyPositionSize = 22F;
        public Syndra()
        {
            Q = new SpellBase(SpellSlot.Q, SpellType.Circular, 825)
            {
                CastDelay = 600,
                Width = Spells.Q.Width,
            };
            W = new SpellBase(SpellSlot.W, SpellType.Circular, 950)
            {
                CastDelay = 250,
                Width = Spells.W.Width,
                Speed = 1450,
            };
            E = new SpellBase(SpellSlot.E, SpellType.Cone, 675 + Spells.E.ExtraRange)
            {
                CastDelay = 250,
                Width = Spells.E.Level1Angle,
                Speed = 2500,
            };
            QE = new SpellBase(SpellSlot.E, SpellType.Linear, Spells.QE.Range)
            {
                CastDelay = Spells.QE.CastDelay,
                Width = 60,
                Speed = Spells.QE.Speed,
            };
            R = new SpellBase(SpellSlot.R, SpellType.Targeted, Spells.R.Level1Range);
            foreach (var enemy in EntityManager.Heroes.Enemies)
            {
                IsKillableOnEnemyPosition.Add(enemy.NetworkId, new Text("R KILLABLE", new Font("Arial", TextEnemyPositionSize, FontStyle.Bold))
                {
                    Color = Color.Red,
                });
                IsKillableOnScreen.Add(enemy.NetworkId, new Text(enemy.BaseSkinName + " is killable", new Font("Arial", TextScreenSize, FontStyle.Bold))
                {
                    Color = Color.Red,
                });
            }
            Obj_AI_Base.OnProcessSpellCast += delegate (Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
            {
                if (sender.IsMe)
                {
                    switch (args.Slot)
                    {
                        case SpellSlot.Q:
                            Q.LastEndPosition = args.End.IsInRange(sender, Q.Range) ? args.End : (MyHero.Position + (args.End - MyHero.Position).Normalized() * Q.Range);
                            Q.LastCastTime = Core.GameTickCount;
                            break;
                        case SpellSlot.W:
                            if (args.SData.Name.ToLower() == "syndrawcast")
                            {
                                W.LastEndPosition = args.End.IsInRange(sender, W.Range) ? args.End : (MyHero.Position + (args.End - MyHero.Position).Normalized() * W.Range);
                                W.LastCastTime = Core.GameTickCount;
                            }
                            break;
                        case SpellSlot.E:
                            E.LastCastTime = Core.GameTickCount;
                            break;
                    }
                }
            };
            GameObject.OnCreate += delegate (GameObject sender, EventArgs args)
            {
                var objBase = sender as Obj_AI_Base;
                if (objBase != null && Sphere.IsMySphere(objBase))
                {
                    Spheres.Add(new Sphere(objBase));
                }
            };
            Gapcloser.OnGapcloser += delegate (AIHeroClient sender, Gapcloser.GapcloserEventArgs args)
            {
                if (sender.IsEnemy && AutomaticMenu.CheckBox("Gapcloser") && args.End.Distance(MyHero, true) <= sender.Distance(MyHero, true))
                {
                    CastE(sender);
                    CastQE(sender);
                }
            };
            Dash.OnDash += delegate (Obj_AI_Base sender, Dash.DashEventArgs args)
            {
                if (sender.IsEnemy && AutomaticMenu.CheckBox("Gapcloser") && args.EndPos.Distance(MyHero, true) <= sender.Distance(MyHero, true))
                {
                    CastE(sender);
                    CastQE(sender);
                }
            };
            Interrupter.OnInterruptableSpell +=
                delegate (Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
                {
                    if (sender.IsEnemy && AutomaticMenu.CheckBox("Interrupter"))
                    {
                        CastE(sender);
                        CastQE(sender);
                    }
                };
            MenuManager.AddSubMenu("Keys");
            {
                KeysMenu.AddValue("QE",
                    new KeyBind("Use QE/WE on enemy near mouse", false, KeyBind.BindTypes.HoldActive, 'E'));
                ToggleManager.RegisterToggle(
                    KeysMenu.AddValue("HarassToggle",
                        new KeyBind("Harass Toggle", false, KeyBind.BindTypes.PressToggle, 'K')),
                    delegate
                    {
                        if (!ModeManager.Combo)
                        {
                            Harass(HarassMenu);
                        }
                    });
            }
            Q.AddConfigurableHitChancePercent(50);
            W.AddConfigurableHitChancePercent(60);
            QE.AddConfigurableHitChancePercent(75);

            MenuManager.AddSubMenu("Combo");
            {
                ComboMenu.AddValue("Q", new CheckBox("Use Q"));
                ComboMenu.AddValue("W", new CheckBox("Use W"));
                ComboMenu.AddValue("E", new CheckBox("Use E"));
                ComboMenu.AddValue("QE", new CheckBox("Use QE"));
                ComboMenu.AddValue("WE", new CheckBox("Use WE"));
            }

            MenuManager.AddSubMenu("Ultimate");
            {
                UltimateMenu.AddValue("BlackList", new GroupLabel("BlackList"));
                var hashSet = new HashSet<string>();
                foreach (var enemy in EntityManager.Heroes.Enemies.Where(enemy => hashSet.Add(enemy.ChampionName)))
                {
                    UltimateMenu.AddValue("BlackList." + enemy.ChampionName, new CheckBox(enemy.ChampionName));
                }
            }
            MenuManager.AddSubMenu("Harass");
            {
                HarassMenu.AddValue("Turret", new CheckBox("Don't harass under enemy turret"));
                HarassMenu.AddValue("Q", new CheckBox("Use Q"));
                HarassMenu.AddValue("W", new CheckBox("Use W", false));
                HarassMenu.AddValue("E", new CheckBox("Use E", false));
                HarassMenu.AddValue("QE", new CheckBox("Use QE", false));
                HarassMenu.AddValue("WE", new CheckBox("Use WE", false));
                HarassMenu.AddValue("ManaPercent", new Slider("Minimum Mana Percent", 25));
            }
            MenuManager.AddSubMenu("Clear");
            {
                ClearMenu.AddValue("LaneClear", new GroupLabel("LaneClear"));
                {
                    ClearMenu.AddValue("LaneClear.Q", new Slider("Use Q if hit >= {0}", 3, 0, 10));
                    ClearMenu.AddValue("LaneClear.W", new Slider("Use W if hit >= {0}", 3, 0, 10));
                    ClearMenu.AddValue("LaneClear.ManaPercent", new Slider("Minimum Mana Percent", 50));
                }
                ClearMenu.AddValue("LastHit", new GroupLabel("LastHit"));
                {
                    ClearMenu.AddStringList("LastHit.Q", "Use Q", new[] { "Never", "Smartly", "Always" }, 1);
                    ClearMenu.AddValue("LastHit.ManaPercent", new Slider("Minimum Mana Percent", 50));
                }
                ClearMenu.AddValue("JungleClear", new GroupLabel("JungleClear"));
                {
                    ClearMenu.AddValue("JungleClear.Q", new CheckBox("Use Q"));
                    ClearMenu.AddValue("JungleClear.W", new CheckBox("Use W"));
                    ClearMenu.AddValue("JungleClear.E", new CheckBox("Use E"));
                    ClearMenu.AddValue("JungleClear.ManaPercent", new Slider("Minimum Mana Percent", 20));
                }
            }
            MenuManager.AddKillStealMenu();
            {
                KillStealMenu.AddValue("Q", new CheckBox("Use Q"));
                KillStealMenu.AddValue("W", new CheckBox("Use W"));
                KillStealMenu.AddValue("E", new CheckBox("Use E"));
                KillStealMenu.AddValue("R", new CheckBox("Use R", false));
            }
            MenuManager.AddSubMenu("Automatic");
            {
                AutomaticMenu.AddValue("Interrupter", new CheckBox("Use E on channeling spells"));
                AutomaticMenu.AddValue("Gapcloser", new CheckBox("Use E on hero gapclosing / dashing"));
            }
            MenuManager.AddSubMenu("Misc");
            {
                MiscMenu.AddValue("QE.ReducedRange", new Slider("Less QE Range", 0, 0, 650));
            }
            MenuManager.AddDrawingsMenu();
            {
                Q.AddDrawings();
                W.AddDrawings();
                QE.AddDrawings();
                R.AddDrawings();
                DrawingsMenu.AddValue("R.Killable", new CheckBox("Draw text if target is r killable"));
                DrawingsMenu.AddValue("Toggles", new CheckBox("Draw toggles status"));
                DrawingsMenu.AddValue("E.Lines", new CheckBox("Draw lines for E"));
            }
            foreach (var sphere in ObjectManager.Get<Obj_AI_Minion>().Where(o => o.IsValid && o.IsVisible && !o.IsDead && Sphere.IsMySphere(o)))
            {
                Spheres.Add(new Sphere(sphere));
            }
        }

        public override void OnDraw(Menu menu)
        {
            if (menu.CheckBox("E.Lines") && E.IsReady && MyHero.Mana >= E.Mana)
            {
                foreach (var sphere in Spheres.Where(i => i.GameObject.VisibleOnScreen && i.GameObject.IsInRange(MyHero, E.Range)))
                {
                    Drawing.DrawLine(sphere.GameObject.Position.WorldToScreen(), (MyHero.Position + (sphere.GameObject.Position - MyHero.Position).Normalized() * QE.Range).WorldToScreen(), QE.Width + MyHero.BoundingRadius, WillHitSpheres.ContainsKey(sphere.GameObject.NetworkId) ? Color.FromArgb(100, 255, 0, 0) : Color.FromArgb(100, 255, 255, 255));
                }
            }
            base.OnDraw(menu);
        }

        public override void OnEndScene(Menu menu)
        {
            if (menu.CheckBox("R.Killable") && R.IsReady && MyHero.Mana >= R.Mana)
            {
                var count = 0;
                foreach (var enemy in R.EnemyHeroes.Where(h => R.GetDamage(h) >= h.TotalShieldHealth()))
                {
                    if (enemy.VisibleOnScreen && IsKillableOnEnemyPosition.ContainsKey(enemy.NetworkId))
                    {
                        IsKillableOnEnemyPosition[enemy.NetworkId].Position = enemy.Position.WorldToScreen();
                        IsKillableOnEnemyPosition[enemy.NetworkId].Draw();
                    }
                    if (IsKillableOnScreen.ContainsKey(enemy.NetworkId))
                    {
                        IsKillableOnScreen[enemy.NetworkId].Position = BaseScreenPoint + new Vector2(0, 50 * count);
                        IsKillableOnScreen[enemy.NetworkId].Draw();
                        count++;
                    }
                }
            }
            base.OnEndScene(menu);
        }
        protected override void KillSteal(Menu menu)
        {
            foreach (var enemy in UnitManager.ValidEnemyHeroesInRange.Where(h => h.HealthPercent <= 40f))
            {
                var result = GetBestCombo(enemy);
                if (menu.CheckBox("Q") && (result.Q || Q.IsKillable(enemy)))
                {
                    CastQ(enemy);
                }
                if (menu.CheckBox("W") && (result.W || W.IsKillable(enemy)))
                {
                    CastW(enemy);
                }
                if (menu.CheckBox("E") && (result.E || E.IsKillable(enemy)))
                {
                    CastE(enemy);
                    CastQE(enemy);
                }
                if (menu.CheckBox("R") && R.GetDamage(enemy) >= enemy.TotalShieldHealth())
                {
                    CastR(enemy);
                }
            }
            base.KillSteal(menu);
        }


        protected override void OnUpdate()
        {
            Spheres.RemoveAll(o => !o.IsValid);
            base.OnUpdate();
        }

        protected override void PermaActive()
        {
            Q.Width = Spells.Q.Width;
            W.Width = Spells.W.Width;
            QE.Speed = Spells.QE.Speed;
            QE.CastDelay = Spells.QE.CastDelay;
            QE.Range = Spells.QE.Range - MiscMenu.Slider("QE.ReducedRange");
            E.Width = E.Level == 5 ? Spells.E.Level5Angle : Spells.E.Level1Angle;
            R.Range = R.Level == 5 ? Spells.R.Level5Range : Spells.R.Level1Range;
            Range = 1200;
            Target = TargetSelector.GetTarget(UnitManager.ValidEnemyHeroesInRange, DamageType.Magical);
            WillHitSpheres.Clear();
            foreach (var sphere in Spheres)
            {
                foreach (var enemy in UnitManager.ValidEnemyHeroesInRange)
                {
                    CheckBallStatus(enemy, sphere.GameObject);
                }
            }
            if (KeysMenu.KeyBind("QE"))
            {
                var bestEnemy = UnitManager.ValidEnemyHeroesInRange.OrderBy(i => i.Distance(MousePos, true)).FirstOrDefault();
                if (bestEnemy != null)
                {
                    CastE(bestEnemy);
                    CastQE(bestEnemy);
                }
            }
            base.PermaActive();
        }

        protected override void Combo(Menu menu)
        {
            if (Target != null)
            {
                if (Core.GameTickCount - W.LastSentTime < 100)
                {
                    return;
                }
                if (Core.GameTickCount - W.LastCastTime < Spells.W.TotalTravelTime)
                {
                    return;
                }
                if (menu.CheckBox("E"))
                {
                    CastE(Target);
                }
                if (menu.CheckBox("W"))
                {
                    CastW(Target);
                }
                if (menu.CheckBox("Q"))
                {
                    CastQ(Target);
                }
                if (menu.CheckBox("QE"))
                {
                    CastQE(Target);
                }
            }
            base.Combo(menu);
        }
        protected override void Harass(Menu menu)
        {
            if (MyHero.ManaPercent >= menu.Slider("ManaPercent"))
            {
                if (Target != null)
                {
                    if (Core.GameTickCount - W.LastSentTime < 100)
                    {
                        return;
                    }
                    if (Core.GameTickCount - W.LastCastTime < Spells.W.TotalTravelTime)
                    {
                        return;
                    }
                    if (menu.CheckBox("Turret") && EntityManager.Turrets.Enemies.Any(i => i.IsInAutoAttackRange(MyHero)))
                    {
                        return;
                    }
                    if (menu.CheckBox("E"))
                    {
                        CastE(Target);
                    }
                    if (menu.CheckBox("W"))
                    {
                        CastW(Target);
                    }
                    if (menu.CheckBox("Q"))
                    {
                        CastQ(Target);
                    }
                    if (menu.CheckBox("QE"))
                    {
                        CastQE(Target);
                    }
                }
            }
            base.Harass(menu);
        }
        protected override void LaneClear(Menu menu)
        {
            if (MyHero.ManaPercent >= menu.Slider("LaneClear.ManaPercent"))
            {
                Q.LaneClear(true, menu.Slider("LaneClear.Q"));
                CastW(W.LaneClear(false, menu.Slider("LaneClear.W")));
            }
            base.LaneClear(menu);
        }
        protected override void LastHit(Menu menu)
        {
            if (MyHero.ManaPercent >= menu.Slider("LastHit.ManaPercent"))
            {
                Q.LastHit((LastHitType)menu.Slider("LastHit.Q"));
            }
            base.LastHit(menu);
        }

        protected override void JungleClear(Menu menu)
        {
            if (MyHero.ManaPercent >= menu.Slider("JungleClear.ManaPercent"))
            {
                if (menu.CheckBox("JungleClear.Q"))
                {
                    Q.JungleClear();
                }
                if (menu.CheckBox("JungleClear.W"))
                {
                    CastW(W.JungleClear(false));
                }
                if (menu.CheckBox("JungleClear.E"))
                {
                    CastE(E.JungleClear(false));
                }
            }
            base.JungleClear(menu);
        }


        public void CastQ(Obj_AI_Base target)
        {
            if (target != null && Q.IsReady)
            {
                Q.Width = target.Type == GameObjectType.AIHeroClient ? Spells.Q.ReducedWidth : Spells.Q.Width;
                Q.Cast(target);
            }
        }
        public void CastW(Obj_AI_Base target)
        {
            if (target != null && W.IsReady)
            {
                if (target.Type == GameObjectType.AIHeroClient)
                {
                    W.Width = Spells.W.ReducedWidth;
                }
                var pred = W.GetPrediction(target);
                if (pred.HitChancePercent >= W.HitChancePercent / 2f)
                {
                    if (IsW2)
                    {
                        W.Cast(target);
                    }
                    else if (Core.GameTickCount - W.LastSentTime >= 200)
                    {
                        var canGrabBall = Core.GameTickCount - E.LastSentTime > 100;
                        if (Core.GameTickCount - E.LastCastTime < Spells.E.TotalTravelTime)
                        {
                            canGrabBall = false;
                        }
                        var validSpheres = Spheres.Where(o => !o.GameObject.IdEquals(target) && MyHero.IsInRange(o.GameObject, W.Range) && o.IsGrabbable && canGrabBall).OrderByDescending(o => o.GameObject.Distance(target, true)).ToList();
                        var spheres = validSpheres.FirstOrDefault();
                        if (validSpheres.Count > 1 && spheres != null)
                        {
                            W.Cast(spheres.GameObject.Position);
                            return;
                        }
                        var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Concat(EntityManager.MinionsAndMonsters.Monsters);
                        var minion = minions.Where(o => !o.IdEquals(target) && MyHero.IsInRange(o, W.Range)).OrderBy(o => MyHero.Distance(o, true)).FirstOrDefault();
                        if (minion != null)
                        {
                            W.Cast(minion.Position);
                            return;
                        }
                        if (spheres != null)
                        {
                            W.Cast(spheres.GameObject.Position);
                        }
                    }
                }
            }
        }

        public void CastE(Obj_AI_Base target)
        {
            if (target != null && E.IsReady)
            {
                foreach (var ball in Spheres.Where(ball => WillHitSpheres.ContainsKey(ball.GameObject.NetworkId) && WillHitSpheres[ball.GameObject.NetworkId].ContainsKey(target.NetworkId)))
                {
                    QE.Cast(WillHitSpheres[ball.GameObject.NetworkId][target.NetworkId]);
                }
            }
        }
        public void CastE(Obj_AI_Base target, Vector3 position)
        {
            if (target != null && E.IsReady)
            {
                var distance = MyHero.Distance(position);
                if (distance <= E.Range)
                {
                    var startPosition = position.To2D();
                    var endPosition = (MyHero.Position + (position - MyHero.Position).Normalized() * QE.Range).To2D();
                    var travelTimeToBall = E.CastDelay + (int)(1000 * distance / E.Speed);
                    QE.CastDelay = travelTimeToBall;
                    var pred = QE.GetPrediction(target, new CustomSettings { SourcePosition = position });
                    QE.CachedPredictions.Clear();
                    if (pred.HitChancePercent >= QE.HitChancePercent)
                    {
                        if (pred.CastPosition.To2D().Distance(startPosition, endPosition, true, true) <= Math.Pow(QE.Width + target.BoundingRadius, 2))
                        {
                            var castPositionInRange = MyHero.Position + (pred.CastPosition - MyHero.Position).Normalized() * 100;
                            QE.Cast(castPositionInRange);
                        }
                    }
                }
            }
        }

        public void CheckBallStatus(Obj_AI_Base target, Obj_AI_Base sphere)
        {
            if (target != null && E.IsReady)
            {
                var distance = MyHero.Distance(sphere.Position);
                if (distance <= E.Range)
                {
                    var startPosition = sphere.Position.To2D();
                    var endPosition = (MyHero.Position + (sphere.Position - MyHero.Position).Normalized() * QE.Range).To2D();
                    var travelTimeToBall = E.CastDelay + (int)(1000 * distance / E.Speed);
                    QE.CastDelay = travelTimeToBall;
                    var pred = QE.GetPrediction(target, new CustomSettings { SourcePosition = sphere.Position });
                    QE.CachedPredictions.Clear();
                    if (pred.HitChancePercent >= QE.HitChancePercent)
                    {
                        if (pred.CastPosition.To2D().Distance(startPosition, endPosition, true, true) <= Math.Pow(QE.Width + target.BoundingRadius, 2))
                        {
                            var castPositionInRange = MyHero.Position + (pred.CastPosition - MyHero.Position).Normalized() * 100;
                            if (!WillHitSpheres.ContainsKey(sphere.NetworkId))
                            {
                                WillHitSpheres.Add(sphere.NetworkId, new Dictionary<int, Vector3>());
                            }
                            WillHitSpheres[sphere.NetworkId].Add(target.NetworkId, castPositionInRange);
                        }
                    }
                }
            }
        }

        // ReSharper disable once InconsistentNaming
        public void CastQE(Obj_AI_Base target)
        {
            if (target != null && MyHero.IsInRange(target, QE.Range))
            {
                if (Q.IsReady)
                {
                    if (E.IsReady && MyHero.Mana >= E.Mana + Q.Mana && Core.GameTickCount - E.LastSentTime > 100)
                    {
                        if (!Q.IsInRange(target))
                        {
                            QE.CastDelay = Q.CastDelay + E.CastDelay;
                            QE.Speed = int.MaxValue;
                            var qPred = QE.GetPrediction(target);
                            QE.CachedPredictions.Clear();
                            if (qPred.HitChancePercent >= QE.HitChancePercent / 2f)
                            {
                                QE.Speed = Spells.QE.Speed;
                                var startPosition3D = MyHero.Position + (qPred.CastPosition - MyHero.Position).Normalized() * Math.Min(E.Range - Spells.E.ExtraRange, MyHero.Distance(qPred.CastPosition));
                                var startPosition = startPosition3D.To2D();
                                var endPosition = (MyHero.Position + (qPred.CastPosition - MyHero.Position).Normalized() * QE.Range).To2D();
                                QE.CastDelay = Q.CastDelay + E.CastDelay + (int)(1000 * startPosition.Distance(MyHero) / E.Speed);
                                var pred = QE.GetPrediction(target, new CustomSettings {SourcePosition = startPosition3D});
                                QE.CachedPredictions.Clear();
                                if (pred.HitChancePercent >= QE.HitChancePercent && pred.CastPosition.To2D().Distance(startPosition, endPosition, true, true) <= Math.Pow(QE.Width + target.BoundingRadius, 2))
                                {
                                    var castPositionInRange = startPosition3D;
                                    Q.Cast(castPositionInRange);
                                }
                            }
                        }
                        else
                        {
                            Q.Width = target.Type == GameObjectType.AIHeroClient ? Spells.Q.ReducedWidth : Spells.Q.Width;
                            var pred = Q.GetPrediction(target);
                            if (pred.HitChancePercent >= Q.HitChancePercent / 2f && MyHero.IsInRange(pred.CastPosition, Range))
                            {
                                Q.Cast(pred.CastPosition);
                            }
                        }
                    }
                }
                else if (E.IsReady && MyHero.Mana >= E.Mana)
                {
                    var timeToArrive = Q.CastDelay - (Core.GameTickCount - Q.LastCastTime);
                    if (0 <= timeToArrive + 50)
                    {
                        var travelTimeToBall = E.CastDelay + (int)(1000 * Q.LastEndPosition.Distance(MyHero) / E.Speed);
                        if (travelTimeToBall >= timeToArrive)
                        {
                            CastE(target, Q.LastEndPosition);
                        }
                    }
                }
            }
        }

        public void CastR(AIHeroClient target)
        {
            if (target != null && R.IsReady && !UltimateMenu.CheckBox("BlackList." + target.ChampionName))
            {
                R.Cast(target);
            }
        }
        public override float GetSpellDamage(SpellSlot slot, Obj_AI_Base target)
        {
            if (target != null)
            {
                var level = slot.GetSpellDataInst().Level;
                switch (slot)
                {
                    case SpellSlot.Q:
                        return (target.Type == GameObjectType.AIHeroClient && level == 5 ? 1.15f : 1) * MyHero.CalculateDamageOnUnit(target, DamageType.Magical,
                            45f * level + 5f + 0.75f * MyHero.FlatMagicDamageMod);
                    case SpellSlot.W:
                        return MyHero.CalculateDamageOnUnit(target, DamageType.Magical,
                            40f * level + 40f + 0.7f * MyHero.FlatMagicDamageMod);
                    case SpellSlot.E:
                        return MyHero.CalculateDamageOnUnit(target, DamageType.Magical,
                            45f * level + 25f + 0.4f * MyHero.FlatMagicDamageMod);
                    case SpellSlot.R:
                        return (Spheres.Count + 3) * MyHero.CalculateDamageOnUnit(target, DamageType.Magical,
                            45f * level + 45f + 0.2f * MyHero.FlatMagicDamageMod);
                }
            }
            return base.GetSpellDamage(slot, target);
        }
    }
}
