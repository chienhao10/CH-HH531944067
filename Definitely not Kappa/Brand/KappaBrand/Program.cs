namespace KappaBrand
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using SharpDX;
    using Color = System.Drawing.Color;

    internal class Program
    {
        public static Vector2[] GMinMaxCorners;
        public static RectangleF GMinMaxBox;
        public static Vector2[] GNonCulledPoints;
        private static AIHeroClient Player { get { return ObjectManager.Player; } }
        public static Spell.Skillshot _Q { get; private set; }
        public static Spell.Skillshot _W { get; private set; }
        public static Spell.Skillshot _W2 { get; private set; }
        public static Spell.Targeted _E { get; private set; }
        public static Spell.Targeted _R { get; private set; }
        
        public static Menu ComboMenu { get; private set; }
        public static Menu HarassMenu { get; private set; }
        public static Menu LaneMenu { get; private set; }
        public static Menu KillStealMenu { get; private set; }
        public static Menu MiscMenu { get; private set; }
        public static Menu DrawMenu { get; private set; }
        private static Menu menuIni;

        private static AIHeroClient comboTarget;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.BaseSkinName != "Brand")
            {
                return;
            }

            menuIni = MainMenu.AddMenu("CH汉化-火男", "Brand");
            menuIni.AddGroupLabel("欢迎使用最“渣”火男!");
            menuIni.AddGroupLabel("全局设定");
            menuIni.Add("Combo", new CheckBox("使用连招?"));
            menuIni.Add("Harass", new CheckBox("使用骚扰?"));
            menuIni.Add("Farm", new CheckBox("使用农兵?"));
            menuIni.Add("KillSteal", new CheckBox("使用抢头?"));
            menuIni.Add("Misc", new CheckBox("使用杂项?"));
            menuIni.Add("Drawings", new CheckBox("使用线圈?"));

            ComboMenu = menuIni.AddSubMenu("连招");
            ComboMenu.AddGroupLabel("连招设置");
            ComboMenu.Add("useQ", new CheckBox("使用 Q"));
            ComboMenu.Add("useW", new CheckBox("使用 W"));
            ComboMenu.Add("useE", new CheckBox("使用 E"));
            ComboMenu.Add("blaze", new CheckBox("使用 Q 前先E"));
            ComboMenu.Add("useR", new CheckBox("使用 R"));
            ComboMenu.Add("Rhit", new Slider("使用大范围 R", 2, 1, 5));
            ComboMenu.AddLabel("大招逻辑: 优先命中更多敌人而非击杀敌人.");

            HarassMenu = menuIni.AddSubMenu("骚扰");
            HarassMenu.AddGroupLabel("骚扰设置");
            HarassMenu.Add("useQ", new CheckBox("使用 Q"));
            HarassMenu.Add("useW", new CheckBox("使用 W"));
            HarassMenu.Add("useE", new CheckBox("使用 E"));
            HarassMenu.Add("mana", new Slider("骚扰蓝量使用", 60, 0, 100));

            LaneMenu = menuIni.AddSubMenu("农兵");
            LaneMenu.AddGroupLabel("农兵设置");
            LaneMenu.Add("useQ", new CheckBox("使用 Q", false));
            LaneMenu.Add("useW", new CheckBox("使用 W"));
            LaneMenu.Add("useE", new CheckBox("使用 E"));
            LaneMenu.Add("mana", new Slider("农兵蓝量使用", 80, 0, 100));

            KillStealMenu = menuIni.AddSubMenu("抢头");
            KillStealMenu.AddGroupLabel("抢头设置");
            KillStealMenu.Add("ksQ", new CheckBox("使用 Q"));
            KillStealMenu.Add("ksW", new CheckBox("使用 W"));
            KillStealMenu.Add("ksE", new CheckBox("使用 E"));
            KillStealMenu.Add("ksR", new CheckBox("使用 R", false));

            MiscMenu = menuIni.AddSubMenu("杂项");
            MiscMenu.AddGroupLabel("杂项设置");
            MiscMenu.Add("gapclose", new CheckBox("防突进"));
            MiscMenu.Add("interrupt", new CheckBox("自动技能打断"));


            DrawMenu = menuIni.AddSubMenu("线圈");
            DrawMenu.AddGroupLabel("线圈设置");
            DrawMenu.Add("drawQ", new CheckBox("显示 Q"));
            DrawMenu.Add("drawW", new CheckBox("显示 W"));
            DrawMenu.Add("drawE", new CheckBox("显示 E"));
            DrawMenu.Add("drawR", new CheckBox("显示 R"));
            DrawMenu.Add("drawDamage", new CheckBox("显示血量伤害"));
            DrawMenu.Add("drawKill", new CheckBox("显示可击杀目标"));

                _Q = new Spell.Skillshot(SpellSlot.Q, 1100, SkillShotType.Linear, 250, 1600, 120);
                _W = new Spell.Skillshot(SpellSlot.W, 900, SkillShotType.Circular, 850, -1, 250);
            _W2 = new Spell.Skillshot(SpellSlot.W, 900, SkillShotType.Circular, 850, -1, 125);
            _E = new Spell.Targeted(SpellSlot.E, 640);
                _R = new Spell.Targeted(SpellSlot.R, 750);
            

            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Interrupter.OnInterruptableSpell += OnInterruptableTarget;
            Gapcloser.OnGapcloser += OnEnemyGapcloser;
        }

        private static void OnEnemyGapcloser(AIHeroClient Sender, Gapcloser.GapcloserEventArgs args)
        {
            if (!menuIni.Get<CheckBox>("Misc").CurrentValue || !MiscMenu.Get<CheckBox>("gapcloser").CurrentValue || Sender == null)
            {
                return;
            }

            if (Sender.HasBuff("brandablaze") && _Q.IsReady() && !Sender.IsAlly && !Sender.IsMe)
            {
                _Q.Cast(Sender.ServerPosition);
            }
            else
            {
                if (_E.IsReady() && _Q.IsReady() && !Sender.IsAlly && !Sender.IsMe)
                {
                    _E.Cast(Sender);
                }
            }
        }

        private static void OnInterruptableTarget(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs arg)
        {
            if (!menuIni.Get<CheckBox>("Misc").CurrentValue || !MiscMenu.Get<CheckBox>("interrupt").CurrentValue || sender == null)
                return;
            var pred = _Q.GetPrediction(sender);
            if (sender.HasBuff("brandablaze") && _Q.IsReady() && pred.HitChance >= HitChance.High && !sender.IsAlly && !sender.IsMe)
            {
                _Q.Cast(pred.CastPosition);
            }
            else
            {
                if (_E.IsReady() && _Q.IsReady() && !sender.IsAlly && !sender.IsMe)
                {
                    _E.Cast(sender);
                }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (menuIni.Get<CheckBox>("Drawings").CurrentValue && DrawMenu.Get<CheckBox>("drawQ").CurrentValue && _Q.IsReady())
            {
                Drawing.DrawCircle(Player.Position, _Q.Range, Color.OrangeRed);
            }
            if (menuIni.Get<CheckBox>("Drawings").CurrentValue && DrawMenu.Get<CheckBox>("drawW").CurrentValue && _W.IsReady())
            {
                Drawing.DrawCircle(Player.Position, _W.Range, Color.OrangeRed);
            }
            if (menuIni.Get<CheckBox>("Drawings").CurrentValue && DrawMenu.Get<CheckBox>("drawE").CurrentValue && _E.IsReady())
            {
                Drawing.DrawCircle(Player.Position, _E.Range, Color.OrangeRed);
            }
            if (menuIni.Get<CheckBox>("Drawings").CurrentValue && DrawMenu.Get<CheckBox>("drawR").CurrentValue && _R.IsReady())
            {
                Drawing.DrawCircle(Player.Position, _R.Range, Color.OrangeRed);
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead) return;
            comboTarget = TargetSelector.GetTarget(_Q.Range, DamageType.Magical);

            var flags = Orbwalker.ActiveModesFlags;
            if (flags.HasFlag(Orbwalker.ActiveModes.Combo) && menuIni.Get<CheckBox>("Combo").CurrentValue)
            {
                    Combo(comboTarget);
            }

            if (flags.HasFlag(Orbwalker.ActiveModes.LaneClear) && menuIni.Get<CheckBox>("Farm").CurrentValue)
            {
                LaneClear();
            }

            if (flags.HasFlag(Orbwalker.ActiveModes.Harass) && menuIni.Get<CheckBox>("Harass").CurrentValue)
            {
                comboTarget = TargetSelector.GetTarget(_Q.Range, DamageType.Magical);
                if (comboTarget.IsValid && comboTarget.IsValidTarget())
                {
                    if (comboTarget == null)
                    {
                        return;
                    }

                    Harass(comboTarget);
                }
            }
            if (menuIni.Get<CheckBox>("KillSteal").CurrentValue)
            {
                KS();
            }
        }


        private static void LaneClear()
        {
            if (menuIni.Get<CheckBox>("Farm").CurrentValue && Player.ManaPercent >= LaneMenu.Get<Slider>("mana").CurrentValue)
            {

                if (LaneMenu.Get<CheckBox>("useW").CurrentValue
                    && _W.IsReady())
                {
                    var minions1 = EntityManager.MinionsAndMonsters.EnemyMinions;
                    if (minions1 == null || !minions1.Any())
                    {
                        return;
                    }

                    var location =
                        GetBestCircularFarmLocation(
                            EntityManager.MinionsAndMonsters.EnemyMinions.Where(
                                x => x.Distance(Player.Position) <= _W.Range)
                                .Select(xm => xm.ServerPosition.To2D())
                                .ToList(),
                            _W.Width,
                            _W.Range);
                    if (location.MinionsHit >= 2)
                    {
                        _W.Cast(location.Position.To3D());
                    }
                }
                if (LaneMenu.Get<CheckBox>("useE").CurrentValue
                    && _E.IsReady())
                {

                    var minions =
                        EntityManager.MinionsAndMonsters.GetLaneMinions(
                            EntityManager.UnitTeam.Enemy,
                            Player.Position,
                            _E.Range + 20,
                            false).Where(m => m.HasBuff("brandablaze"));
                    foreach (var minion in minions)
                    {
                        _E.Cast(minion);
                    }

                }
                if (LaneMenu.Get<CheckBox>("useQ").CurrentValue
                    && _Q.IsReady())
                {

                    var minions =
                        EntityManager.MinionsAndMonsters.GetLaneMinions(
                            EntityManager.UnitTeam.Enemy,
                            Player.Position,
                            _Q.Range + 20,
                            false);
                    foreach (var minion in minions)
                    {
                        _Q.Cast(minion.ServerPosition);
                    }
                }
            }
        }

        private static void Harass(AIHeroClient target)
        {
            if (target == null)
            {
                return;
            }
            if (menuIni.Get<CheckBox>("Harass").CurrentValue && HarassMenu.Get<CheckBox>("useE").CurrentValue
                && _E.IsReady() && CheckMana())
            {
                CastE(target);
            }
            if (menuIni.Get<CheckBox>("Harass").CurrentValue && HarassMenu.Get<CheckBox>("useQ").CurrentValue
                && _Q.IsReady() && CheckMana() && CheckMana())
            {
                CastQ(target);
            }
            if (menuIni.Get<CheckBox>("Harass").CurrentValue && HarassMenu.Get<CheckBox>("useW").CurrentValue
                && _W.IsReady() && CheckMana() && CheckMana())
            {
                CastW(target);
            }
        }

        private static bool CheckMana()
        {
            return Player.ManaPercent >= HarassMenu.Get<Slider>("mana").CurrentValue;
        }

        private static void Combo(AIHeroClient target)
        {
            if (target == null)
            {
                return;
            }
            if (menuIni.Get<CheckBox>("Combo").CurrentValue && ComboMenu.Get<CheckBox>("useE").CurrentValue
                && _E.IsReady())
            {
                CastE(target);
            }
            if (menuIni.Get<CheckBox>("Combo").CurrentValue && ComboMenu.Get<CheckBox>("useQ").CurrentValue
                && _Q.IsReady())
            {
                CastQ(target);
            }
            if (menuIni.Get<CheckBox>("Combo").CurrentValue && ComboMenu.Get<CheckBox>("useW").CurrentValue
                && _W.IsReady())
            {
                CastW(target);
            }
            if (menuIni.Get<CheckBox>("Combo").CurrentValue && ComboMenu.Get<CheckBox>("useR").CurrentValue
                && _R.IsReady())
            {
                CastR(target);
            }
        }

        private static void KS()
        {
            foreach (var enemy in EntityManager.Heroes.Enemies)
            {
                if (enemy == null)
                {
                    return;
                }
                if (KillStealMenu.Get<CheckBox>("ksQ").CurrentValue
                 && Player.GetSpellDamage(enemy, SpellSlot.Q) > enemy.Health && _Q.IsReady() && enemy.IsValidTarget(_Q.Range))
                {
                    _Q.Cast(enemy);
                }
                else if (KillStealMenu.Get<CheckBox>("ksW").CurrentValue && Player.GetSpellDamage(enemy, SpellSlot.W) > enemy.Health && _W.IsReady() && enemy.IsValidTarget(_W.Range))
                {
                    _W.Cast(enemy);
                }
                else if (KillStealMenu.Get<CheckBox>("ksE").CurrentValue && Player.GetSpellDamage(enemy, SpellSlot.E) > enemy.Health && _E.IsReady() && enemy.IsValidTarget(_E.Range))
                {
                    _E.Cast(enemy);
                }
                else if (KillStealMenu.Get<CheckBox>("ksR").CurrentValue && Player.GetSpellDamage(enemy, SpellSlot.R) > enemy.Health && _R.IsReady() && enemy.IsValidTarget(_R.Range))
                {
                    _R.Cast(enemy);
                }
            }
        }

        private static void CastQ(AIHeroClient target)
        {
            if (target == null)
            {
                return;
            }
            var predq = _Q.GetPrediction(target);
            if (target.HasBuff("brandablaze") && menuIni.Get<CheckBox>("Combo").CurrentValue && ComboMenu.Get<CheckBox>("blaze").CurrentValue && predq.HitChance >= HitChance.High)
            {
                if (_Q.IsReady() && target.IsValidTarget(_Q.Range))
                {
                    _Q.Cast(predq.CastPosition);
                }
            }
            else
            {
                if (!ComboMenu.Get<CheckBox>("blaze").CurrentValue && _Q.IsReady() && target.IsValidTarget(_Q.Range) && predq.HitChance >= HitChance.High)
                {
                    _Q.Cast(predq.CastPosition);
                }
            }
        }

        private static void CastW(AIHeroClient target)
        {
            if (target == null)
            {
                return;
            }
            const int range = 1100;
            const float aoeratio = 0.2f;

            var enemies = EntityManager.Heroes.Enemies.Where(n => n.IsValidTarget(range));
            var selectedTarget = TargetSelector.GetTarget(range, DamageType.Magical);

            if (selectedTarget == null && !enemies.Any())
            {
                return;
            }
            
            if (enemies.Count() > 1)
            {
                var aoePrediction =
                    Prediction.Position.PredictCircularMissileAoe(
                        enemies.Cast<Obj_AI_Base>().ToArray(),
                        _W.Range,
                        _W.Radius,
                        _W.CastDelay,
                        _W.Speed).OrderByDescending(r => r.GetCollisionObjects<Obj_AI_Minion>().Length)
                        .FirstOrDefault();

                if (aoePrediction != null)
                {
                    var predictedHeroes = aoePrediction.GetCollisionObjects<AIHeroClient>();

                    if (predictedHeroes.Length > 1 && (float)predictedHeroes.Length / enemies.Count() >= aoeratio)
                    {
                        _W.Cast(aoePrediction.CastPosition);
                        return;
                    }
                }
            }
            var predw = _W2.GetPrediction(target);
                if (_W2.IsReady() && target.IsValidTarget(_W2.Range) && predw.HitChance >= HitChance.High)
            {
                _W2.Cast(predw.CastPosition);
            }
                else
                {
                _W.Cast(target.Position);
            }
        }

        private static void CastE(AIHeroClient target)
        {

            if (target == null)
            {
                return;
            }

            if (_E.IsReady() && target.IsValidTarget(_E.Range))
            {
                _E.Cast(target);
            }
        }

        private static void CastR(AIHeroClient target)
        {

            if (target == null)
            {
                return;
            }
            if (_R.IsReady() && target.IsValidTarget(_R.Range))
            {
                if (target.CountEnemiesInRange(700) >= ComboMenu.Get<Slider>("Rhit").CurrentValue)
                {
                    _R.Cast(target);
                }

                if ((_Q.IsReady()
                     && (Player.GetSpellDamage(target, SpellSlot.Q) + Player.GetSpellDamage(target, SpellSlot.R)
                         >= target.Health))
                    || (_W.IsReady()
                        && (Player.GetSpellDamage(target, SpellSlot.W) + Player.GetSpellDamage(target, SpellSlot.R)
                            >= target.Health))
                    || (_E.IsReady()
                        && (Player.GetSpellDamage(target, SpellSlot.E) + Player.GetSpellDamage(target, SpellSlot.R)
                            >= target.Health)) || (_Q.IsReady() && (Player.GetSpellDamage(target, SpellSlot.Q) + Player.GetSpellDamage(target, SpellSlot.R) >= target.Health)))
                {
                    _R.Cast(target);
                }
            }
        }
        

        private static double GetDamage(AIHeroClient target)
        {
            var pDamage = Player.CalculateDamageOnUnit(target, DamageType.Magical, (float)((.08) * target.MaxHealth));
            double qDamage = Player.GetSpellDamage(target, SpellSlot.Q);
            double wDamage = Player.GetSpellDamage(target, SpellSlot.W);
            double eDamage = Player.GetSpellDamage(target, SpellSlot.E);
            double rDamage = Player.GetSpellDamage(target, SpellSlot.R);
            var totalDamage = 0.0;

            var myMana = Player.Mana;
            var qMana = _Q.Handle.SData.Mana;
            var wMana = _W.Handle.SData.Mana;
            var eMana = _E.Handle.SData.Mana;
            var rMana = _R.Handle.SData.Mana;
            var totalMana = 0.0;

            if (!_Q.IsReady())
            {
                qDamage = 0.0;
            }
            if (!_W.IsReady())
            {
                wDamage = 0.0;
            }
            if (!_E.IsReady())
            {
                eDamage = 0.0;
            }
            if (!_R.IsReady())
            {
                rDamage = 0.0;
            }

            if (myMana >= eMana && myMana >= totalMana)
            {
                totalMana += eMana;
                totalDamage += eDamage;
            }

            if (myMana >= qMana && myMana >= totalMana)
            {
                totalMana += qMana;
                totalDamage += qDamage;
            }

            if (myMana >= wMana && myMana >= totalMana)
            {
                totalMana += wMana;
                totalDamage += wDamage;
            }

            if (myMana >= rMana && myMana >= totalMana)
            {
                totalMana += rMana;
                totalDamage += rDamage;
            }

            totalDamage += pDamage;
            return totalDamage;
        }

        // For debugging.

        // Find the points nearest the upper left, upper right,
        // lower left, and lower right corners.
        private static void GetMinMaxCorners(
            List<Vector2> points,
            ref Vector2 ul,
            ref Vector2 ur,
            ref Vector2 ll,
            ref Vector2 lr)
        {
            // Start with the first point as the solution.
            ul = points[0];
            ur = ul;
            ll = ul;
            lr = ul;

            // Search the other points.
            foreach (var pt in points)
            {
                if (-pt.X - pt.Y > -ul.X - ul.Y)
                {
                    ul = pt;
                }
                if (pt.X - pt.Y > ur.X - ur.Y)
                {
                    ur = pt;
                }
                if (-pt.X + pt.Y > -ll.X + ll.Y)
                {
                    ll = pt;
                }
                if (pt.X + pt.Y > lr.X + lr.Y)
                {
                    lr = pt;
                }
            }

            GMinMaxCorners = new[] { ul, ur, lr, ll }; // For debugging.
        }

        // Find a box that fits inside the MinMax quadrilateral.
        private static RectangleF GetMinMaxBox(List<Vector2> points)
        {
            // Find the MinMax quadrilateral.
            Vector2 ul = new Vector2(0, 0), ur = ul, ll = ul, lr = ul;
            GetMinMaxCorners(points, ref ul, ref ur, ref ll, ref lr);

            // Get the coordinates of a box that lies inside this quadrilateral.
            var xmin = ul.X;
            var ymin = ul.Y;

            var xmax = ur.X;
            if (ymin < ur.Y)
            {
                ymin = ur.Y;
            }

            if (xmax > lr.X)
            {
                xmax = lr.X;
            }
            var ymax = lr.Y;

            if (xmin < ll.X)
            {
                xmin = ll.X;
            }
            if (ymax > ll.Y)
            {
                ymax = ll.Y;
            }

            var result = new RectangleF(xmin, ymin, xmax - xmin, ymax - ymin);
            GMinMaxBox = result; // For debugging.
            return result;
        }

        // Cull points out of the convex hull that lie inside the
        // trapezoid defined by the vertices with smallest and
        // largest X and Y coordinates.
        // Return the points that are not culled.
        private static List<Vector2> HullCull(List<Vector2> points)
        {
            // Find a culling box.
            var cullingBox = GetMinMaxBox(points);

            // Cull the points.
            var results =
                points.Where(
                    pt =>
                        pt.X <= cullingBox.Left || pt.X >= cullingBox.Right || pt.Y <= cullingBox.Top ||
                        pt.Y >= cullingBox.Bottom).ToList();

            GNonCulledPoints = new Vector2[results.Count]; // For debugging.
            results.CopyTo(GNonCulledPoints); // For debugging.
            return results;
        }

        // Return the points that make up a polygon's convex hull.
        // This method leaves the points list unchanged.
        public static List<Vector2> MakeConvexHull(List<Vector2> points)
        {
            // Cull.
            points = HullCull(points);

            // Find the remaining point with the smallest Y value.
            // if (there's a tie, take the one with the smaller X value.
            Vector2[] bestPt = { points[0] };
            foreach (
                var pt in points.Where(pt => (pt.Y < bestPt[0].Y) || ((pt.Y == bestPt[0].Y) && (pt.X < bestPt[0].X))))
            {
                bestPt[0] = pt;
            }

            // Move this point to the convex hull.
            var hull = new List<Vector2> { bestPt[0] };
            points.Remove(bestPt[0]);

            // Start wrapping up the other points.
            float sweepAngle = 0;
            for (;;)
            {
                // If all of the points are on the hull, we're done.
                if (points.Count == 0)
                {
                    break;
                }

                // Find the point with smallest AngleValue
                // from the last point.
                var x = hull[hull.Count - 1].X;
                var y = hull[hull.Count - 1].Y;
                bestPt[0] = points[0];
                float bestAngle = 3600;

                // Search the rest of the points.
                foreach (var pt in points)
                {
                    var testAngle = AngleValue(x, y, pt.X, pt.Y);
                    if ((testAngle >= sweepAngle) && (bestAngle > testAngle))
                    {
                        bestAngle = testAngle;
                        bestPt[0] = pt;
                    }
                }

                // See if the first point is better.
                // If so, we are done.
                var firstAngle = AngleValue(x, y, hull[0].X, hull[0].Y);
                if ((firstAngle >= sweepAngle) && (bestAngle >= firstAngle))
                {
                    // The first point is better. We're done.
                    break;
                }

                // Add the best point to the convex hull.
                hull.Add(bestPt[0]);
                points.Remove(bestPt[0]);

                sweepAngle = bestAngle;
            }

            return hull;
        }

        // Return a number that gives the ordering of angles
        // WRST horizontal from the point (x1, y1) to (x2, y2).
        // In other words, AngleValue(x1, y1, x2, y2) is not
        // the angle, but if:
        //   Angle(x1, y1, x2, y2) > Angle(x1, y1, x2, y2)
        // then
        //   AngleValue(x1, y1, x2, y2) > AngleValue(x1, y1, x2, y2)
        // this angle is greater than the angle for another set
        // of points,) this number for
        //
        // This function is dy / (dy + dx).
        private static float AngleValue(float x1, float y1, float x2, float y2)
        {
            float t;

            var dx = x2 - x1;
            var ax = Math.Abs(dx);
            var dy = y2 - y1;
            var ay = Math.Abs(dy);
            if (ax + ay == 0)
            {
                // if (the two points are the same, return 360.
                t = 360f / 9f;
            }
            else
            {
                t = dy / (ax + ay);
            }
            if (dx < 0)
            {
                t = 2 - t;
            }
            else if (dy < 0)
            {
                t = 4 + t;
            }
            return t * 90;
        }

        // Find a minimal bounding circle.
        public static void FindMinimalBoundingCircle(List<Vector2> points, out Vector2 center, out float radius)
        {
            // Find the convex hull.
            var hull = MakeConvexHull(points);

            // The best solution so far.
            var bestCenter = points[0];
            var bestRadius2 = float.MaxValue;

            // Look at pairs of hull points.
            for (var i = 0; i < hull.Count - 1; i++)
            {
                for (var j = i + 1; j < hull.Count; j++)
                {
                    // Find the circle through these two points.
                    var testCenter = new Vector2((hull[i].X + hull[j].X) / 2f, (hull[i].Y + hull[j].Y) / 2f);
                    var dx = testCenter.X - hull[i].X;
                    var dy = testCenter.Y - hull[i].Y;
                    var testRadius2 = dx * dx + dy * dy;

                    // See if this circle would be an improvement.
                    if (testRadius2 < bestRadius2)
                    {
                        // See if this circle encloses all of the points.
                        if (CircleEnclosesPoints(testCenter, testRadius2, points, i, j, -1))
                        {
                            // Save this solution.
                            bestCenter = testCenter;
                            bestRadius2 = testRadius2;
                        }
                    }
                } // for i
            } // for j

            // Look at triples of hull points.
            for (var i = 0; i < hull.Count - 2; i++)
            {
                for (var j = i + 1; j < hull.Count - 1; j++)
                {
                    for (var k = j + 1; k < hull.Count; k++)
                    {
                        // Find the circle through these three points.
                        Vector2 testCenter;
                        float testRadius2;
                        FindCircle(hull[i], hull[j], hull[k], out testCenter, out testRadius2);

                        // See if this circle would be an improvement.
                        if (testRadius2 < bestRadius2)
                        {
                            // See if this circle encloses all of the points.
                            if (CircleEnclosesPoints(testCenter, testRadius2, points, i, j, k))
                            {
                                // Save this solution.
                                bestCenter = testCenter;
                                bestRadius2 = testRadius2;
                            }
                        }
                    } // for k
                } // for i
            } // for j

            center = bestCenter;
            if (bestRadius2 == float.MaxValue)
            {
                radius = 0;
            }
            else
            {
                radius = (float)Math.Sqrt(bestRadius2);
            }
        }

        // Return true if the indicated circle encloses all of the points.
        private static bool CircleEnclosesPoints(Vector2 center,
            float radius2,
            List<Vector2> points,
            int skip1,
            int skip2,
            int skip3)
        {
            return (from point in points.Where((t, i) => (i != skip1) && (i != skip2) && (i != skip3))
                    let dx = center.X - point.X
                    let dy = center.Y - point.Y
                    select dx * dx + dy * dy).All(testRadius2 => !(testRadius2 > radius2));
        }

        // Find a circle through the three points.
        private static void FindCircle(Vector2 a, Vector2 b, Vector2 c, out Vector2 center, out float radius2)
        {
            // Get the perpendicular bisector of (x1, y1) and (x2, y2).
            var x1 = (b.X + a.X) / 2;
            var y1 = (b.Y + a.Y) / 2;
            var dy1 = b.X - a.X;
            var dx1 = -(b.Y - a.Y);

            // Get the perpendicular bisector of (x2, y2) and (x3, y3).
            var x2 = (c.X + b.X) / 2;
            var y2 = (c.Y + b.Y) / 2;
            var dy2 = c.X - b.X;
            var dx2 = -(c.Y - b.Y);

            // See where the lines intersect.
            var cx = (y1 * dx1 * dx2 + x2 * dx1 * dy2 - x1 * dy1 * dx2 - y2 * dx1 * dx2) / (dx1 * dy2 - dy1 * dx2);
            var cy = (cx - x1) * dy1 / dx1 + y1;
            center = new Vector2(cx, cy);

            var dx = cx - a.X;
            var dy = cy - a.Y;
            radius2 = dx * dx + dy * dy;
        }

        public struct MecCircle
        {
            public Vector2 Center;
            public float Radius;

            public MecCircle(Vector2 center, float radius)
            {
                Center = center;
                Radius = radius;
            }
        }

        public static MecCircle GetMec(List<Vector2> points)
        {
            var center = new Vector2();
            float radius;

            var convexHull = MakeConvexHull(points);
            FindMinimalBoundingCircle(convexHull, out center, out radius);
            return new MecCircle(center, radius);
        }
        public static FarmLocation GetBestCircularFarmLocation(List<Vector2> minionPositions,
            float width,
            float range,
            int useMecMax = 9)
        {
            var result = new Vector2();
            var minionCount = 0;
            var startPos = ObjectManager.Player.ServerPosition.To2D();

            range = range * range;

            if (minionPositions.Count == 0)
            {
                return new FarmLocation(result, minionCount);
            }

            /* Use MEC to get the best positions only when there are less than 9 positions because it causes lag with more. */
            if (minionPositions.Count <= useMecMax)
            {
                var subGroups = GetCombinations(minionPositions);
                foreach (var subGroup in subGroups)
                {
                    if (subGroup.Count > 0)
                    {
                        var circle = GetMec(subGroup);

                        if (circle.Radius <= width && circle.Center.Distance(startPos, true) <= range)
                        {
                            minionCount = subGroup.Count;
                            return new FarmLocation(circle.Center, minionCount);
                        }
                    }
                }
            }
            else
            {
                foreach (var pos in minionPositions)
                {
                    if (pos.Distance(startPos, true) <= range)
                    {
                        var count = minionPositions.Count(pos2 => pos.Distance(pos2, true) <= width * width);

                        if (count >= minionCount)
                        {
                            result = pos;
                            minionCount = count;
                        }
                    }
                }
            }

            return new FarmLocation(result, minionCount);
        }

        private static List<List<Vector2>> GetCombinations(List<Vector2> allValues)
        {
            var collection = new List<List<Vector2>>();
            for (var counter = 0; counter < (1 << allValues.Count); ++counter)
            {
                var combination = allValues.Where((t, i) => (counter & (1 << i)) == 0).ToList();

                collection.Add(combination);
            }
            return collection;
        }

        public struct FarmLocation
        {
            public int MinionsHit;
            public Vector2 Position;

            public FarmLocation(Vector2 position, int minionsHit)
            {
                Position = position;
                MinionsHit = minionsHit;
            }
        }
    }
}
