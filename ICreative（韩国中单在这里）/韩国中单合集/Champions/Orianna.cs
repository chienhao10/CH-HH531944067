using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using KoreanAIO.Managers;
using KoreanAIO.Model;
using KoreanAIO.Utilities;
using SharpDX;

namespace KoreanAIO.Champions
{
    public class Orianna : ChampionBase
    {
        private const int QAoeWidth = 145;
        public static string BallName = "Orianna_Base_Q_yomu_ring_green.troy";
        private bool _ballIsMissile;
        private GameObject _ballObject;
        private bool _canShield;
        private int _hitR;
        private int _hitW;

        public Orianna()
        {
            try
            {
                AIO.Initializers.Add(delegate
                {
                    _ballObject = (MyHero.HasBuff("orianaghostself") ? MyHero : null) ??
                EntityManager.Heroes.AllHeroes.FirstOrDefault(h => h.IsValidTarget() && MyHero.Team == h.Team && h.HasBuff("orianaghost") && h.GetBuff("orianaghost").Caster.IdEquals(MyHero)) ??
                ObjectManager.Get<Obj_AI_Minion>()
                    .FirstOrDefault(
                        o =>
                            o.IsValid && !o.IsDead && o.IsVisible && o.Team == MyHero.Team && o.BaseSkinName == "OriannaBall" && o.HasBuff("orianaghost") &&
                            o.GetBuff("orianaghost").Caster.IdEquals(MyHero)) as GameObject;
                });
                Q = new SpellBase(SpellSlot.Q, SpellType.Circular, 815)
                {
                    Speed = 1200,
                    Width = 80,
                    CollidesWithYasuoWall = false
                };
                Q.SetSourceFunction(() => Ball);
                W = new SpellBase(SpellSlot.W, SpellType.Self, 255);
                W.SetSourceFunction(() => Ball);
                W.SetRangeCheckSourceFunction(() => Ball);
                E = new SpellBase(SpellSlot.E, SpellType.Linear, 1095)
                {
                    Speed = 1800,
                    Width = 85,
                    MinHitChancePercent = 45,
                    CollidesWithYasuoWall = false
                };
                E.SetSourceFunction(() => Ball);
                R = new SpellBase(SpellSlot.R, SpellType.Self, 400)
                {
                    CastDelay = 500
                };
                R.SetSourceFunction(() => Ball);
                R.SetRangeCheckSourceFunction(() => Ball);
                Spellbook.OnCastSpell += delegate (Spellbook sender, SpellbookCastSpellEventArgs args)
                {
                    if (sender.Owner.IsMe)
                    {
                        if (args.Slot == SpellSlot.R && MiscMenu.CheckBox("R.Block"))
                        {
                            args.Process = _hitR != 0 && !_ballIsMissile;
                        }
                    }
                };
                GameObject eTarget = null;
                Obj_AI_Base.OnProcessSpellCast += delegate (Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
                {
                    if (sender.IsMe)
                    {
                        if (args.Slot == SpellSlot.E)
                        {
                            eTarget = args.Target;
                        }
                    }
                };
                GameObject.OnCreate += delegate (GameObject sender, EventArgs args)
                {
                    if (sender.Name.Equals(BallName))
                    {
                        _ballObject = sender;
                    }
                    else
                    {
                        var missile = sender as MissileClient;
                        if (missile != null && missile.SpellCaster != null && missile.SpellCaster.IsMe)
                        {
                            if (missile.SData.Name.ToLower().Equals("orianaizuna") || missile.SData.Name.ToLower().Equals("orianaredact"))
                            {
                                _ballObject = missile;
                            }
                        }
                    }
                };
                GameObject.OnDelete += delegate (GameObject sender, EventArgs args)
                {
                    var missile = sender as MissileClient;
                    if (missile != null && missile.SpellCaster != null && missile.SpellCaster.IsMe)
                    {
                        if (missile.SData.Name.ToLower().Equals("orianaredact"))
                        {
                            _ballObject = eTarget;
                        }
                    }
                };
                Obj_AI_Base.OnPlayAnimation += delegate (Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
                {
                    if (sender.IsMe && args.Animation.Equals("Prop"))
                    {
                        _ballObject = Player.Instance;
                    }
                };
                Gapcloser.OnGapcloser += delegate (AIHeroClient sender, Gapcloser.GapcloserEventArgs args)
                {
                    if (sender.IsAlly)
                    {
                        if (Target != null && AutomaticMenu.CheckBox("Gapcloser") &&
                            Ball.GetDistanceSqr(Target) > args.End.Distance(Target, true) &&
                            args.End.Distance(Target, true) < args.Sender.GetDistanceSqr(Target))
                        {
                            CastE(sender);
                        }
                    }
                };
                Dash.OnDash += delegate (Obj_AI_Base sender, Dash.DashEventArgs args)
                {
                    if (sender.IsAlly)
                    {
                        if (Target != null && AutomaticMenu.CheckBox("Gapcloser") &&
                            Ball.GetDistanceSqr(Target) > args.EndPos.Distance(Target, true) &&
                            args.EndPos.Distance(Target, true) < sender.GetDistanceSqr(Target))
                        {
                            CastE(sender);
                        }
                    }
                };
                Interrupter.OnInterruptableSpell +=
                    delegate (Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
                    {
                        if (sender.IsEnemy)
                        {
                            if (AutomaticMenu.CheckBox("Interrupter"))
                            {
                                if (!Ball.InRange(args.Sender, R.Range))
                                {
                                    ThrowBall(args.Sender);
                                }
                                else
                                {
                                    CastR(args.Sender);
                                }
                            }
                        }
                    };
                Obj_AI_Base.OnBasicAttack += delegate (Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
                {
                    if (sender.IsEnemy && _canShield && args.Target != null && args.Target.IsMe)
                    {
                        if (sender.Type == GameObjectType.AIHeroClient)
                        {
                            if (sender.IsMelee)
                            {
                                CastE(MyHero);
                            }
                        }
                        else if (sender.Type == GameObjectType.obj_AI_Turret)
                        {
                            CastE(MyHero);
                        }
                    }
                };
                Obj_AI_Base.OnProcessSpellCast += delegate (Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
                {
                    if (sender.IsEnemy && _canShield && args.Target != null && args.Target.IsMe)
                    {
                        if (sender.Type == GameObjectType.AIHeroClient)
                        {
                            CastE(MyHero);
                        }
                    }
                };

                Q.AddConfigurableHitChancePercent();
                W.AddConfigurableHitChancePercent();
                E.AddConfigurableHitChancePercent();
                R.AddConfigurableHitChancePercent();

                MenuManager.AddSubMenu("Combo");
                {
                    ComboMenu.AddValue("TeamFight",
                        new Slider("Use TeamFight logic if enemies near >= {0}", 3, 1, 5));
                    ComboMenu.AddValue("Common", new GroupLabel("Common logic"));
                    ComboMenu.AddValue("Q", new CheckBox("Use Q"));
                    ComboMenu.AddValue("W", new CheckBox("Use W"));
                    ComboMenu.AddValue("E.Shield", new CheckBox("Use E on enemy spells"));
                    ComboMenu.AddValue("E.HealthPercent", new Slider("Use E if my % of health is less than {0}", 40));
                    ComboMenu.AddValue("1vs1", new GroupLabel("1 vs 1 logic"));
                    ComboMenu.AddValue("R.Killable", new CheckBox("Use R if target is killable"));
                    ComboMenu.AddValue("TeamFightLogic", new GroupLabel("TeamFight logic"));
                    ComboMenu.AddValue("Q.Hit", new Slider("Use Q if hit >= {0}", 2, 1, 5));
                    ComboMenu.AddValue("W.Hit", new Slider("Use W if hit >= {0}", 2, 1, 5));
                    ComboMenu.AddValue("R.Hit", new Slider("Use R if hit >= {0}", 3, 1, 5));
                }

                MenuManager.AddSubMenu("Harass");
                {
                    HarassMenu.AddValue("Q", new CheckBox("Use Q"));
                    HarassMenu.AddValue("W", new CheckBox("Use W"));
                    HarassMenu.AddValue("E.Shield", new CheckBox("Use E on enemy spells"));
                    HarassMenu.AddValue("E.HealthPercent", new Slider("Use E if my % of health is less than {0}", 40));
                    HarassMenu.AddValue("ManaPercent", new Slider("Minimum Mana Percent", 25));
                }

                MenuManager.AddSubMenu("Clear");
                {
                    ClearMenu.AddValue("LaneClear", new GroupLabel("LaneClear"));
                    {
                        ClearMenu.AddValue("LaneClear.Q", new Slider("Use Q if hit >= {0}", 4, 0, 10));
                        ClearMenu.AddValue("LaneClear.W", new Slider("Use W if hit >= {0}", 3, 0, 10));
                        ClearMenu.AddValue("LaneClear.E", new Slider("Use E if hit >= {0}", 6, 0, 10));
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
                    AutomaticMenu.AddValue("Gapcloser", new CheckBox("Use E on hero gapclosing / dashing"));
                    AutomaticMenu.AddValue("Interrupter", new CheckBox("Use R on channeling spells"));
                    AutomaticMenu.AddValue("E.Shield", new CheckBox("Use E on enemy spells"));
                    AutomaticMenu.AddValue("W.Hit", new Slider("Use W if hit >= {0}", 2, 1, 5));
                    AutomaticMenu.AddValue("R.Hit", new Slider("Use R if hit >= {0}", 3, 1, 5));
                }

                MenuManager.AddDrawingsMenu();
                {
                    var c = DrawingsMenu.AddValue("Ball", new CheckBox("Draw ball position"));
                    CircleManager.Circles.Add(new Circle(c, new ColorBGRA(0, 0, 255, 100), () => 120, () => true,
                        () => Ball)
                    { Width = 3 });
                    Q.AddDrawings();
                    W.AddDrawings();
                    E.AddDrawings(false);
                    R.AddDrawings();
                }
                MenuManager.AddSubMenu("Misc");
                {
                    MiscMenu.AddValue("R.Block", new CheckBox("Block R if will not hit"));
                    if (EntityManager.Heroes.Enemies.Count > 0)
                    {
                        var enemiesAdded = new HashSet<string>();
                        MiscMenu.AddValue("BlackList.R", new GroupLabel("Don't use R on:"));
                        foreach (var enemy in EntityManager.Heroes.Enemies.Where(enemy => enemiesAdded.Add(enemy.ChampionName)))
                        {
                            MiscMenu.AddValue("BlackList." + enemy.ChampionName,
                                new CheckBox(enemy.ChampionName, false));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                AIO.WriteInConsole(e.ToString());
            }
        }

        private GameObject Ball
        {
            get
            {
                if (_ballObject != null && _ballObject.IsValid && !_ballObject.IsDead)
                {
                    return _ballObject;
                }
                return MyHero;
            }
        }

        protected override void PermaActive()
        {
            _hitR = R.IsReady ? R.EnemyHeroes.Count : 0;
            _hitW = W.IsReady ? W.ObjectsInRange(W.EnemyHeroes).Count : 0;
            Range = Q.Range + R.Width;
            _canShield = AutomaticMenu.CheckBox("E.Shield") || (ModeManager.Combo && ComboMenu.CheckBox("E.Shield")) ||
                         (ModeManager.Harass && HarassMenu.CheckBox("E.Shield"));
            _ballIsMissile = _ballObject != null && _ballObject.IsValid &&
                             _ballObject.Type == GameObjectType.MissileClient;
            Target = TargetSelector.GetTarget(UnitManager.ValidEnemyHeroesInRange, DamageType.Magical);
            if (_hitR >= AutomaticMenu.Slider("R.Hit"))
            {
                R.Cast();
            }
            if (_hitW >= AutomaticMenu.Slider("W.Hit"))
            {
                W.Cast();
            }
            if (E.IsReady && AutomaticMenu.CheckBox("E.Shield"))
            {
                if (MissileManager.MissileWillHitMyHero)
                {
                    CastE(MyHero);
                }
            }
            base.PermaActive();
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
                }
                if (menu.CheckBox("R") && (result.R || R.IsKillable(enemy)))
                {
                    CastR(enemy);
                }
                if ((menu.CheckBox("Q") || menu.CheckBox("E")) &&
                    (
                        result.Q || Q.IsKillable(enemy) || result.W || W.IsKillable(enemy) || result.R ||
                        R.IsKillable(enemy)
                        )
                    )
                {
                    ThrowBall(enemy);
                }
            }
            base.KillSteal(menu);
        }

        protected override void Combo(Menu menu)
        {
            if (Target != null)
            {
                var bestCombo = GetBestCombo(Target);
                if (MyHero.CountEnemiesInRange(E.Range) >= menu.Slider("TeamFight"))
                {
                    if (menu.Slider("Q.Hit") > 0)
                    {
                        var list = Q.EnemyHeroes;
                        if (list.Count >= menu.Slider("Q.Hit"))
                        {
                            var result = GetBestHitQ(list);
                            if (result.Hits >= menu.Slider("Q.Hit"))
                            {
                                Q.Cast(result.Position);
                            }
                        }
                    }
                    if (W.IsReady && menu.Slider("W.Hit") > 0)
                    {
                        var list = W.EnemyHeroes;
                        if (list.Count >= menu.Slider("W.Hit"))
                        {
                            if (W.ObjectsInRange(list).Count >= menu.Slider("W.Hit"))
                            {
                                W.Cast();
                            }
                        }
                    }
                    if (R.IsReady && menu.Slider("R.Hit") > 0)
                    {
                        var list = R.EnemyHeroes;
                        if (list.Count >= menu.Slider("R.Hit"))
                        {
                            if (R.ObjectsInRange(list).Count >= menu.Slider("R.Hit"))
                            {
                                R.Cast();
                            }
                        }
                    }
                    CastQr();
                    CastEr();
                }
                else
                {
                    if (R.IsReady && menu.CheckBox("R.Killable") && bestCombo.R && R.IsKillable(Target))
                    {
                        CastR(Target);
                    }
                }
                if (menu.CheckBox("Q"))
                {
                    CastQ(Target);
                }
                if (menu.CheckBox("W"))
                {
                    CastW(Target);
                }
                if (menu.Slider("E.HealthPercent") >= MyHero.HealthPercent)
                {
                    var enemy = UnitManager.ValidEnemyHeroesInRange.FirstOrDefault(h => h.InAutoAttackRange(MyHero));
                    if (enemy != null)
                    {
                        CastE(MyHero);
                    }
                }
            }
            base.Combo(menu);
        }

        protected override void Harass(Menu menu)
        {
            if (MyHero.ManaPercent >= menu.Slider("ManaPercent"))
            {
                if (E.IsReady && menu.CheckBox("E.Shield"))
                {
                    if (MissileManager.MissileWillHitMyHero)
                    {
                        CastE(MyHero);
                    }
                }
                if (Target != null)
                {
                    if (menu.CheckBox("Q"))
                    {
                        CastQ(Target);
                    }
                    if (menu.CheckBox("W"))
                    {
                        CastW(Target);
                    }
                    if (menu.Slider("E.HealthPercent") >= MyHero.HealthPercent)
                    {
                        var enemy =
                            UnitManager.ValidEnemyHeroesInRange.FirstOrDefault(h => h.InAutoAttackRange(MyHero));
                        if (enemy != null)
                        {
                            CastE(MyHero);
                        }
                    }
                }
            }
            base.Harass(menu);
        }

        protected override void LaneClear(Menu menu)
        {
            if (MyHero.ManaPercent >= menu.Slider("LaneClear.ManaPercent"))
            {
                if (Q.IsReady && menu.Slider("LaneClear.Q") > 0)
                {
                    var minions = Q.LaneClearMinions;
                    if (minions.Count >= menu.Slider("LaneClear.Q"))
                    {
                        var result = GetBestHitQ(minions);
                        if (result.Hits >= menu.Slider("LaneClear.Q"))
                        {
                            Q.Cast(result.Position);
                        }
                    }
                }
                if (W.IsReady && menu.Slider("LaneClear.W") > 0)
                {
                    var minions = W.LaneClearMinions;
                    if (minions.Count >= menu.Slider("LaneClear.W"))
                    {
                        W.Cast();
                    }
                }
                if (E.IsReady && menu.Slider("LaneClear.E") > 0)
                {
                    var minions = E.LaneClearMinions;
                    if (minions.Count >= menu.Slider("LaneClear.E"))
                    {
                        var result = GetBestHitE(minions);
                        if (result.Hits >= menu.Slider("LaneClear.E"))
                        {
                            CastE(result.Target);
                        }
                    }
                }
            }
            base.LaneClear(menu);
        }

        protected override void LastHit(Menu menu)
        {
            if (MyHero.ManaPercent >= menu.Slider("LastHit.ManaPercent"))
            {
                var tuples = Q.LastHit((LastHitType)menu.Slider("LastHit.Q"), false);
                var minion = tuples.FirstOrDefault();
                if (minion != null)
                {
                    CastQ(minion);
                }
            }
            base.LastHit(menu);
        }

        protected override void JungleClear(Menu menu)
        {
            if (MyHero.ManaPercent >= ClearMenu.Slider("JungleClear.ManaPercent"))
            {
                if (ClearMenu.CheckBox("JungleClear.Q"))
                {
                    CastQ(Q.JungleClear(false));
                }
                if (ClearMenu.CheckBox("JungleClear.W"))
                {
                    CastW(W.JungleClear(false));
                }
                if (ClearMenu.CheckBox("JungleClear.E"))
                {
                    var minion = E.JungleClear(false);
                    CastE(minion);
                }
            }
            base.JungleClear(menu);
        }

        protected override void Flee()
        {
            if (Ball.InRange(MyHero, W.Range))
            {
                if (W.IsReady)
                {
                    W.Cast();
                }
            }
            else
            {
                if (E.IsReady)
                {
                    CastE(MyHero);
                }
                else if (Q.IsReady && !(_ballObject is MissileClient))
                {
                    Q.Cast(MyHero);
                }
            }
            base.Flee();
        }

        private void CastQ(Obj_AI_Base target, int minHits = 1)
        {
            if (Q.IsReady && target != null)
            {
                var pred = Q.GetPrediction(target);
                if (E.IsReady && MyHero.Mana >= Q.Mana + E.Mana && target.Type == GameObjectType.AIHeroClient &&
                    pred.HitChancePercent <= 5 &&
                    !Q.Source.IsInRange(pred.CastPosition, Q.Range * 1.2f) && Player.Instance.InRange(target, Q.Range + Q.Width + target.BoundingRadius / 2f))
                {
                    var pred2 = Q.GetPrediction(target, new CustomSettings { Source = MyHero });
                    if (pred2.HitChancePercent >= Q.HitChancePercent)
                    {
                        CastE(MyHero);
                    }
                }
                var list = new List<Obj_AI_Base>();
                var tempList = Q.EnemyHeroes;
                if (tempList.Contains(target))
                {
                    list = tempList;
                }
                else
                {
                    tempList = Q.EnemyMinions;
                    if (tempList.Contains(target))
                    {
                        list = tempList;
                    }
                    else
                    {
                        tempList = Q.Monsters;
                        if (tempList.Contains(target))
                        {
                            list = tempList;
                        }
                    }
                }
                if (list.Count >= minHits)
                {
                    var res = GetBestHitQ(list, target);
                    if (res.Hits >= minHits)
                    {
                        Q.Cast(res.Position);
                    }
                }
            }
        }

        private void CastW(Obj_AI_Base target)
        {
            if (W.IsReady && target != null && !_ballIsMissile)
            {
                W.Cast(target);
            }
        }

        private void CastE(Obj_AI_Base target)
        {
            if (E.IsReady && target != null)
            {
                if (target.Team == MyHero.Team)
                {
                    MyHero.Spellbook.CastSpell(SpellSlot.E, target);
                }
                else
                {
                    var list = new List<Obj_AI_Base>();
                    var tempList = E.EnemyHeroes;
                    if (tempList.Contains(target))
                    {
                        list = tempList;
                    }
                    else
                    {
                        tempList = E.EnemyMinions;
                        if (tempList.Contains(target))
                        {
                            list = tempList;
                        }
                        else
                        {
                            tempList = E.Monsters;
                            if (tempList.Contains(target))
                            {
                                list = tempList;
                            }
                        }
                    }
                    if (list.Count > 0)
                    {
                        var result = GetBestHitE(list);
                        if (result.Hits > 0)
                        {
                            MyHero.Spellbook.CastSpell(SpellSlot.E, result.Target);
                        }
                    }
                }
            }
        }

        public void CastR(AIHeroClient target)
        {
            if (R.IsReady && target != null && !MiscMenu.CheckBox("BlackList." + target.ChampionName) && !_ballIsMissile)
            {
                R.Cast(target);
            }
        }

        private void ThrowBall(AIHeroClient target)
        {
            if (E.IsReady)
            {
                var bestAllyNear =
                    UnitManager.ValidAllyHeroesInRange.Where(
                        h => h.InRange(target, R.Range * 1.5f) && Ball.GetDistanceSqr(target) > h.GetDistanceSqr(target))
                        .OrderBy(h => h.GetDistanceSqr(target))
                        .FirstOrDefault();
                if (bestAllyNear != null)
                {
                    CastE(bestAllyNear);
                    return;
                }
            }
            CastQ(target);
        }

        private void CastQr()
        {
            if (Q.IsReady && R.IsReady)
            {
                Q.CachedPredictions.Clear();
                var qWidth = Q.Width;
                var qCastDelay = Q.CastDelay;
                Q.CastDelay = R.CastDelay + qCastDelay;
                Q.Width = R.Range;
                var list = (from enemy in UnitManager.ValidEnemyHeroes
                            select Q.GetPrediction(enemy)
                    into pred
                            where pred.HitChancePercent >= R.HitChancePercent / 2f
                            select pred.CastPosition.To2D()).ToList();
                if (list.Count >= ComboMenu.Slider("R.Hit"))
                {
                    var bestCount = -1;
                    var bestPoint = new Vector2(0, 0);
                    var result = Enumerable.Range(1, (1 << list.Count) - 1).Select(index => list.Where((item, idx) => ((1 << idx) & index) != 0).ToList()).ToList();
                    foreach (var points in result)
                    {
                        var polygon = new Geometry.Polygon();
                        polygon.Points.AddRange(points);
                        var center = polygon.CenterOfPolygon();
                        var count = list.Count(v => center.IsInRange(v, R.Range * 1.4f));
                        if (count > bestCount)
                        {
                            bestCount = count;
                            bestPoint = center;
                        }
                    }
                    if (bestCount >= ComboMenu.Slider("R.Hit"))
                    {
                        Q.Cast(bestPoint.To3DWorld());
                    }
                }
                Q.CachedPredictions.Clear();
                Q.CastDelay = qCastDelay;
                Q.Width = qWidth;
            }
        }

        private void CastEr()
        {
            if (E.IsReady && R.IsReady)
            {
                E.CachedPredictions.Clear();
                var eWidth = E.Width;
                var eCastDelay = E.CastDelay;
                E.CastDelay = R.CastDelay + eCastDelay;
                E.Width = R.Range;
                E.Type = SpellType.Circular;
                var enemyList = (from enemy in UnitManager.ValidEnemyHeroes
                                 select E.GetPrediction(enemy)
                    into pred
                                 where pred.HitChancePercent >= R.HitChancePercent / 2f
                                 select pred.CastPosition).ToList();
                if (enemyList.Count >= ComboMenu.Slider("R.Hit"))
                {
                    var allyList =
                        UnitManager.ValidAllyHeroesInRange.Select(ally => E.GetPrediction(ally))
                            .Select(pred => pred.CastPosition.To2D()).ToList();
                    var bestCount = -1;
                    var bestPoint = default(Vector2);
                    var result = Enumerable.Range(1, (1 << allyList.Count) - 1).Select(index => allyList.Where((item, idx) => ((1 << idx) & index) != 0).ToList()).ToList();
                    foreach (var points in result)
                    {
                        var polygon = new Geometry.Polygon();
                        polygon.Points.AddRange(points);
                        var center = polygon.CenterOfPolygon();
                        var count = enemyList.Count(v => center.IsInRange(v, R.Range * 1.4f));
                        if (count > bestCount)
                        {
                            bestCount = count;
                            bestPoint = center;
                        }
                    }
                    if (bestCount >= ComboMenu.Slider("R.Hit"))
                    {
                        var allyNear =
                            UnitManager.ValidAllyHeroesInRange.OrderBy(h => h.Distance(bestPoint, true))
                                .FirstOrDefault();
                        if (allyNear != null)
                        {
                            CastE(allyNear);
                        }
                    }
                }
                E.CachedPredictions.Clear();
                E.Type = SpellType.Self;
                E.CastDelay = eCastDelay;
                E.Width = eWidth;
            }
        }

        private BestPositionResult GetBestHitQ(IReadOnlyCollection<Obj_AI_Base> list, Obj_AI_Base target = null)
        {
            var bestResult = new BestPositionResult();
            if (Q.IsReady)
            {
                var checkTarget = target != null;
                foreach (var obj in list)
                {
                    var pred = Q.GetPrediction(obj, new CustomSettings { Width = QAoeWidth });
                    if (pred.HitChancePercent >= Q.HitChancePercent)
                    {
                        var res = Q.ObjectsInLine(list.Where(o => !o.IdEquals(obj)).ToList(), obj);
                        if (!checkTarget || res.Contains(target) || obj.IdEquals(target))
                        {
                            var count = res.Count + 1;
                            if (bestResult.Hits < count)
                            {
                                bestResult.Hits = count;
                                bestResult.Position = pred.CastPosition;
                                bestResult.Target = obj;
                                if (bestResult.Hits == list.Count)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return bestResult;
        }

        private BestPositionResult GetBestHitE(List<Obj_AI_Base> objAiBases)
        {
            var bestResult = new BestPositionResult();
            if (E.IsReady)
            {
                foreach (
                    var ally in
                        UnitManager.ValidAllyHeroes.Where(
                            h => E.IsInRange(h) && h.GetDistanceSqr(Ball) > 0))
                {
                    var pred = E.GetPrediction(ally);
                    if (pred.HitChancePercent >= E.HitChancePercent / 3)
                    {
                        var res = E.ObjectsInLine(objAiBases, ally);
                        var count = res.Count;
                        if (bestResult.Hits < count)
                        {
                            bestResult.Hits = count;
                            bestResult.Position = pred.CastPosition;
                            bestResult.Target = ally;
                            if (bestResult.Hits == objAiBases.Count)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            return bestResult;
        }

        public override float GetSpellDamage(SpellSlot slot, Obj_AI_Base target)
        {
            if (target != null)
            {
                var level = slot.GetSpellDataInst().Level;
                switch (slot)
                {
                    case SpellSlot.Q:
                        return MyHero.CalculateDamageOnUnit(target, DamageType.Magical,
                            30f * level + 30f + 0.5f * MyHero.FlatMagicDamageMod);
                    case SpellSlot.W:
                        return MyHero.CalculateDamageOnUnit(target, DamageType.Magical,
                            45f * level + 25f + 0.7f * MyHero.FlatMagicDamageMod);
                    case SpellSlot.E:
                        return MyHero.CalculateDamageOnUnit(target, DamageType.Magical,
                            30f * level + 30f + 0.3f * MyHero.FlatMagicDamageMod);
                    case SpellSlot.R:
                        return MyHero.CalculateDamageOnUnit(target, DamageType.Magical,
                            75f * level + 75f + 0.7f * MyHero.FlatMagicDamageMod);
                }
            }
            return base.GetSpellDamage(slot, target);
        }
    }
}