namespace KappaLissandra
{
    using System;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Rendering;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    using SharpDX;

    internal class Lissandra
    {
        private static MissileClient LissEMissile;

        public static Spell.Skillshot Q { get; set; }

        public static Spell.Skillshot Qtest { get; set; }

        public static Spell.Skillshot Q2 { get; set; }

        public static Spell.Active W { get; set; }

        public static Spell.Skillshot E { get; set; }

        public static Spell.Active E2 { get; set; }

        public static Spell.Targeted R { get; set; }

        public static AIHeroClient Player { get; set; }

        public static Menu ComboMenu { get; private set; }

        public static Menu UltMenu { get; private set; }

        public static Menu HarassMenu { get; private set; }

        public static Menu LaneMenu { get; private set; }

        public static Menu MiscMenu { get; private set; }

        public static Menu FleeMenu { get; private set; }

        public static Menu DrawMenu { get; private set; }

        private static Menu menuIni;

        public static void Execute()
        {
            if (ObjectManager.Player.BaseSkinName != "Lissandra")
            {
                return;
            }

            menuIni = MainMenu.AddMenu("KappaLissandra", "KappaLissandra");
            menuIni.AddGroupLabel("Welcome to the Worst Lissandra addon!");
            menuIni.AddGroupLabel("Global Settings");
            menuIni.Add("Combo", new CheckBox("Use Combo?"));
            menuIni.Add("Harass", new CheckBox("Use Harass?"));
            menuIni.Add("LaneClear", new CheckBox("Use Lane Clear?"));
            menuIni.Add("JungleClear", new CheckBox("Use Jungle Clear?"));
            menuIni.Add("Flee", new CheckBox("Use Flee?"));
            menuIni.Add("Misc", new CheckBox("Use Misc?"));
            menuIni.Add("Drawings", new CheckBox("Use Drawings?"));

            UltMenu = menuIni.AddSubMenu("Ultimate");
            UltMenu.AddGroupLabel("Ultimate Settings");
            UltMenu.Add("aoeR", new CheckBox("AoE R Logic"));
            UltMenu.Add("RF", new CheckBox("Use R Finisher"));
            UltMenu.Add("RS", new CheckBox("Use R On Self"));
            UltMenu.Add("RE", new CheckBox("Use R On Enemy"));
            UltMenu.Add("hitR", new Slider("R AoE Hit >=", 2, 1, 5));
            UltMenu.Add("shp", new Slider("On Self Health to use R", 15, 0, 100));
            UltMenu.AddGroupLabel("Don't Use Ult On: Enemy");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>())
            {
                CheckBox cb = new CheckBox(enemy.BaseSkinName) { CurrentValue = false };
                if (enemy.Team != ObjectManager.Player.Team)
                {
                    UltMenu.Add("DontUltenemy" + enemy.BaseSkinName, cb);
                }
            }

            ComboMenu = menuIni.AddSubMenu("Combo");
            ComboMenu.AddGroupLabel("Combo Settings");
            ComboMenu.Add("Q", new CheckBox("Use Q"));
            ComboMenu.Add("W", new CheckBox("Use W"));
            ComboMenu.Add("E", new CheckBox("Use E"));
            ComboMenu.Add("ET", new CheckBox("Use E2 If hit target"));
            ComboMenu.Add("E2", new CheckBox("Always E2 Max", false));
            ComboMenu.Add("ES", new CheckBox("Use E2 Safe", false));
            ComboMenu.Add("EHP", new Slider("Use E2 Safe if HP <= %", 30, 0, 100));
            ComboMenu.Add("ESE", new Slider("Use E2 Safe if Enemies are <=", 2, 1, 5));

            HarassMenu = menuIni.AddSubMenu("Harass");
            HarassMenu.AddGroupLabel("Harass Settings");
            HarassMenu.Add("Q", new CheckBox("Use Q"));
            HarassMenu.Add("W", new CheckBox("Use W"));
            HarassMenu.Add("E", new CheckBox("Use E", false));
            HarassMenu.Add("Mana", new Slider("Save Mana %", 30, 0, 100));

            LaneMenu = menuIni.AddSubMenu("Farm");
            LaneMenu.AddGroupLabel("LaneClear Settings");
            LaneMenu.Add("Q", new CheckBox("Use Q"));
            LaneMenu.Add("W", new CheckBox("Use W"));
            LaneMenu.Add("E", new CheckBox("Use E", false));
            LaneMenu.Add("Mana", new Slider("Save Mana %", 30, 0, 100));
            LaneMenu.AddGroupLabel("JungleClear Settings");
            LaneMenu.Add("jQ", new CheckBox("Use Q"));
            LaneMenu.Add("jW", new CheckBox("Use W"));
            LaneMenu.Add("jE", new CheckBox("Use E", false));

            MiscMenu = menuIni.AddSubMenu("Misc");
            MiscMenu.AddGroupLabel("Misc Settings");
            MiscMenu.Add("gapcloserW", new CheckBox("Anti-GapCloser W"));
            MiscMenu.Add("gapcloserR", new CheckBox("Anti-GapCloser R"));
            MiscMenu.Add("Interruptr", new CheckBox("Interrupt R"));
            MiscMenu.Add("WTower", new CheckBox("Auto W Under Tower"));
            MiscMenu.Add("AutoW", new Slider("Auto W On Hit >=", 2, 1, 5));

            FleeMenu = menuIni.AddSubMenu("Flee");
            FleeMenu.AddGroupLabel("Flee Settings");
            FleeMenu.Add("Q", new CheckBox("Use Q"));
            FleeMenu.Add("W", new CheckBox("Use W"));
            FleeMenu.Add("E", new CheckBox("Use E"));

            DrawMenu = menuIni.AddSubMenu("Drawings");
            DrawMenu.AddGroupLabel("Drawing Settings");
            DrawMenu.Add("Q", new CheckBox("Draw Q"));
            DrawMenu.Add("W", new CheckBox("Draw W"));
            DrawMenu.Add("E", new CheckBox("Draw E"));
            DrawMenu.Add("R", new CheckBox("Draw R"));
            DrawMenu.Add("debug", new CheckBox("debug", false));

            Q = new Spell.Skillshot(SpellSlot.Q, 715, SkillShotType.Linear, 250, 2200, 75);
            Q2 = new Spell.Skillshot(SpellSlot.Q, 825, SkillShotType.Linear, 250, 2200, 90);
            Qtest = new Spell.Skillshot(SpellSlot.Q, 715, SkillShotType.Linear, 250, 2200, 75) { AllowedCollisionCount = int.MaxValue };
            W = new Spell.Active(SpellSlot.W, 425);
            E = new Spell.Skillshot(SpellSlot.E, 1000, SkillShotType.Linear, 250, 850, 125);
            R = new Spell.Targeted(SpellSlot.R, 400);

            Game.OnUpdate += OnUpdate;
            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDelete;
            Drawing.OnDraw += OnDraw;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Obj_AI_Base.OnBasicAttack += OnBasicAttack;
            Interrupter.OnInterruptableSpell += OnInterruptableSpell;
            Gapcloser.OnGapcloser += OnGapcloser;
        }

        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(args.Target is AIHeroClient))
            {
                return;
            }

            var caster = sender;
            var target = (AIHeroClient)args.Target;

            if ((!(caster is AIHeroClient) && !(caster is Obj_AI_Turret)) || caster == null || target == null)
            {
                return;
            }
            if (target.IsMe)
            {
                var shp = UltMenu["shp"].Cast<Slider>().CurrentValue;
                var useRS = UltMenu["RS"].Cast<CheckBox>().CurrentValue && R.IsReady();
                if (sender == null || sender.IsAlly || sender.IsMe)
                {
                    return;
                }

                if (sender.IsEnemy || sender is Obj_AI_Turret)
                {
                    if (useRS && Player.HealthPercent <= shp && !Player.HasBuff("kindredrnodeathbuff") && !Player.HasBuff("JudicatorIntervention")
                        && !Player.HasBuff("ChronoShift") && !Player.HasBuff("UndyingRage"))
                    {
                        R.Cast(Player);
                    }
                }
            }
        }

        public static void OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(args.Target is AIHeroClient))
            {
                return;
            }

            var caster = sender;
            var target = (AIHeroClient)args.Target;

            if ((!(caster is AIHeroClient) && !(caster is Obj_AI_Turret)) || caster == null || target == null)
            {
                return;
            }

            if (target.IsMe)
            {
                var shp = UltMenu["shp"].Cast<Slider>().CurrentValue;
                var useRS = UltMenu["RS"].Cast<CheckBox>().CurrentValue && R.IsReady();
                if (sender == null || sender.IsAlly || sender.IsMe)
                {
                    return;
                }

                if (sender.IsEnemy || sender is Obj_AI_Turret)
                {
                    if (useRS && Player.HealthPercent <= shp && !Player.HasBuff("kindredrnodeathbuff") && !Player.HasBuff("JudicatorIntervention")
                        && !Player.HasBuff("ChronoShift") && !Player.HasBuff("UndyingRage"))
                    {
                        R.Cast(Player);
                    }
                }
            }
        }

        private static void OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (!menuIni.Get<CheckBox>("Misc").CurrentValue || sender == null || sender.IsAlly || sender.IsMe)
            {
                return;
            }

            if (W.IsReady() && sender.IsValidTarget(W.Range - 15) && MiscMenu.Get<CheckBox>("gapcloserW").CurrentValue)
            {
                W.Cast();
                return;
            }

            if (R.IsReady() && sender.IsValidTarget(R.Range) && MiscMenu.Get<CheckBox>("gapcloserR").CurrentValue
                && !UltMenu["DontUltenemy" + sender.BaseSkinName].Cast<CheckBox>().CurrentValue)
            {
                R.Cast(sender);
            }
        }

        private static void OnInterruptableSpell(Obj_AI_Base Sender, Interrupter.InterruptableSpellEventArgs args)
        {
            if (!menuIni.Get<CheckBox>("Misc").CurrentValue || Sender == null || Sender.IsAlly || Sender.IsMe || !Sender.IsEnemy)
            {
                return;
            }

            if (R.IsReady() && Sender.IsValidTarget(R.Range) && MiscMenu.Get<CheckBox>("Interruptr").CurrentValue
                && !UltMenu["DontUltenemy" + Sender.BaseSkinName].Cast<CheckBox>().CurrentValue)
            {
                R.Cast(Sender);
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            Player = ObjectManager.Player;

            var flags = Orbwalker.ActiveModesFlags;
            if (flags.HasFlag(Orbwalker.ActiveModes.Combo) && menuIni.Get<CheckBox>("Combo").CurrentValue)
            {
                Combo();
            }

            if (flags.HasFlag(Orbwalker.ActiveModes.Harass) && menuIni.Get<CheckBox>("Harass").CurrentValue)
            {
                Harass();
            }

            if (flags.HasFlag(Orbwalker.ActiveModes.LaneClear) && menuIni.Get<CheckBox>("LaneClear").CurrentValue)
            {
                if (Player.ManaPercent > LaneMenu["Mana"].Cast<Slider>().CurrentValue)
                {
                    Clear();
                }
            }

            if (flags.HasFlag(Orbwalker.ActiveModes.JungleClear) && menuIni.Get<CheckBox>("JungleClear").CurrentValue)
            {
                if (Player.ManaPercent > LaneMenu["Mana"].Cast<Slider>().CurrentValue)
                {
                    jClear();
                }
            }

            if (flags.HasFlag(Orbwalker.ActiveModes.Flee) && menuIni.Get<CheckBox>("Flee").CurrentValue)
            {
                Flee();
            }

            if (W.IsReady())
            {
                if (MiscMenu.Get<CheckBox>("WTower").CurrentValue && Player.CountEnemiesInRange(W.Range) >= 1 && Player.IsUnderHisturret()
                    && Player.IsUnderTurret() && !Player.IsUnderEnemyturret())
                {
                    W.Cast();
                }

                if (Player.CountEnemiesInRange(W.Range) >= MiscMenu.Get<Slider>("AutoW").CurrentValue)
                {
                    W.Cast();
                }
            }
        }

        private static void OnCreate(GameObject sender, EventArgs args)
        {
            var miss = sender as MissileClient;
            if (miss != null && miss.IsValid)
            {
                if (miss.SpellCaster.IsMe && miss.SpellCaster.IsValid && miss.SData.Name == "LissandraEMissile")
                {
                    LissEMissile = miss;
                }
            }
        }

        private static void OnDelete(GameObject sender, EventArgs args)
        {
            var miss = sender as MissileClient;
            if (miss == null || !miss.IsValid)
            {
                return;
            }
            if (miss.SpellCaster is AIHeroClient && miss.SpellCaster.IsValid && miss.SpellCaster.IsMe && miss.SData.Name == "LissandraEMissile")
            {
                LissEMissile = null;
            }
        }

        private static void Flee()
        {
            Player = ObjectManager.Player;
            var useQ = FleeMenu["Q"].Cast<CheckBox>().CurrentValue && Q.IsReady();
            var useW = FleeMenu["W"].Cast<CheckBox>().CurrentValue && W.IsReady();
            var useE = FleeMenu["E"].Cast<CheckBox>().CurrentValue && E.IsReady();

            if (useW)
            {
                CastW();
            }

            if (useQ)
            {
                CastQ();
            }

            if (LissEMissile == null && useE)
            {
                E.Cast(Game.CursorPos);
            }

            if (useE && LissEMissile != null && LissEMissile.Position.IsInRange(LissEMissile.EndPosition, 50))
            {
                E.Cast(Game.CursorPos);
            }
        }

        private static void Combo()
        {
            Player = ObjectManager.Player;
            var useQ = ComboMenu["Q"].Cast<CheckBox>().CurrentValue && Q.IsReady();
            var useW = ComboMenu["W"].Cast<CheckBox>().CurrentValue && W.IsReady();
            var useE = ComboMenu["E"].Cast<CheckBox>().CurrentValue && E.IsReady();
            var useRS = UltMenu["RS"].Cast<CheckBox>().CurrentValue && R.IsReady();
            var useRE = UltMenu["RE"].Cast<CheckBox>().CurrentValue && R.IsReady();

            if (useW)
            {
                CastW();
            }

            if (useQ)
            {
                CastQ();
            }

            if (useE)
            {
                CastE();
            }

            if (useRS || useRE)
            {
                CastR();
            }
        }

        private static void Harass()
        {
            Player = ObjectManager.Player;
            var useQ = HarassMenu["Q"].Cast<CheckBox>().CurrentValue;
            var useW = HarassMenu["W"].Cast<CheckBox>().CurrentValue;
            var useE = HarassMenu["E"].Cast<CheckBox>().CurrentValue;

            var Target = TargetSelector.GetTarget(E.Range * 0.94f, DamageType.Magical);

            if (Target == null || !Target.IsValidTarget())
            {
                Target =
                    EntityManager.Heroes.Enemies.FirstOrDefault(
                        h => h.IsValidTarget() && (Vector3.Distance(h.ServerPosition, Player.ServerPosition) < E.Range * 0.94) && !h.IsZombie);
            }

            if (Target != null && !Target.IsInvulnerable)
            {
                if (useQ)
                {
                    CastQ();
                }

                if (useW)
                {
                    CastW();
                }

                if (useE)
                {
                    CastE();
                }
            }
        }

        private static void Clear()
        {
            Player = ObjectManager.Player;
            var useQ = LaneMenu["Q"].Cast<CheckBox>().CurrentValue;
            var useW = LaneMenu["W"].Cast<CheckBox>().CurrentValue;
            var useE = LaneMenu["E"].Cast<CheckBox>().CurrentValue;

            var allMinions = EntityManager.MinionsAndMonsters.Get(
                EntityManager.MinionsAndMonsters.EntityType.Minion,
                EntityManager.UnitTeam.Enemy,
                ObjectManager.Player.Position,
                Q.Range,
                false);
            if (allMinions == null)
            {
                return;
            }

            if (useQ)
            {
                var fl = EntityManager.MinionsAndMonsters.GetLineFarmLocation(allMinions, Q.Width, (int)Q.Range);
                if (fl.HitNumber >= 1)
                {
                    Q.Cast(fl.CastPosition);
                }
            }

            if (useW)
            {
                var fl = EntityManager.MinionsAndMonsters.GetLineFarmLocation(allMinions, 100, (int)W.Range);
                if (fl.HitNumber >= 2)
                {
                    W.Cast();
                }
            }

            if (useE && LissEMissile == null && E.Handle.ToggleState == 1)
            {
                var fl = EntityManager.MinionsAndMonsters.GetLineFarmLocation(allMinions, E.Width, (int)E.Range);
                if (fl.HitNumber >= 1)
                {
                    E.Cast(fl.CastPosition);
                }
            }
        }

        private static void jClear()
        {
            Player = ObjectManager.Player;
            var useQ = LaneMenu["jQ"].Cast<CheckBox>().CurrentValue;
            var useW = LaneMenu["jW"].Cast<CheckBox>().CurrentValue;
            var useE = LaneMenu["jE"].Cast<CheckBox>().CurrentValue;

            var jmobs = ObjectManager.Get<Obj_AI_Minion>().OrderBy(m => m.CampNumber).Where(m => m.IsMonster && m.IsEnemy && !m.IsDead);
            foreach (var jmob in jmobs)
            {
                if (useQ && jmob.IsValidTarget(Q.Range) && jmobs.Any())
                {
                    Q.Cast(jmob.Position);
                }

                if (useW && jmob.IsValidTarget(W.Range))
                {
                    W.Cast();
                }

                if (useE && E.IsReady() && jmob.IsValidTarget(E.Range) && LissEMissile == null && E.Handle.ToggleState == 1)
                {
                    E.Cast(jmob.Position);
                }
            }
        }

        private static void CastQ()
        {
            if (!Q.IsReady())
            {
                return;
            }

            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (target == null)
            {
                return;
            }

            if (target.IsValidTarget(Q.Range))
            {
                var predq = Q.GetPrediction(target);
                Q.Cast(predq.CastPosition);
            }

            var target2 = TargetSelector.GetTarget(Q2.Range, DamageType.Physical);

            if (target2 == null)
            {
                return;
            }

            var pred = Q2.GetPrediction(target2);
            var collisions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(it => it.IsValidTarget(Q.Range)).ToList();

            if (!collisions.Any())
            {
                return;
            }

            foreach (var minion in collisions)
            {
                var poly = new Geometry.Polygon.Rectangle(
                    (Vector2)Player.ServerPosition,
                    Player.ServerPosition.Extend(minion.ServerPosition, Q2.Range),
                    Q2.Width);

                if (poly.IsInside(pred.UnitPosition))
                {
                    Q.Cast(minion.Position);
                }
            }
        }

        private static void CastW()
        {
            if (!W.IsReady())
            {
                return;
            }

            var target = TargetSelector.GetTarget(W.Range, DamageType.Magical);
            if (target != null && Vector3.Distance(target.ServerPosition, Player.ServerPosition) <= W.Range - 5)
            {
                W.Cast();
            }

            if (
                EntityManager.Heroes.Enemies.Any(
                    h => h.IsValidTarget() && h != null && (Vector3.Distance(h.ServerPosition, Player.ServerPosition) < W.Range) && !h.IsZombie))
            {
                W.Cast();
            }
        }

        private static void CastE()
        {
            var useE = ComboMenu["E"].Cast<CheckBox>().CurrentValue && E.IsReady();
            var EHP = ComboMenu["EHP"].Cast<Slider>().CurrentValue;
            var ESE = ComboMenu["ESE"].Cast<Slider>().CurrentValue;
            var useET = ComboMenu["ET"].Cast<CheckBox>().CurrentValue && E.IsReady();
            var useE2 = ComboMenu["E2"].Cast<CheckBox>().CurrentValue && E.IsReady();
            var useES = ComboMenu["ES"].Cast<CheckBox>().CurrentValue && E.IsReady();
            if (!E.IsReady())
            {
                return;
            }

            var target = TargetSelector.GetTarget(E.Range + 100, DamageType.Magical);
            if (LissEMissile == null && !Player.HasBuff("LissandraE") && target != null && useE)
            {
                var pred = E.GetPrediction(target);
                E.Cast(pred.CastPosition);
            }

            if (useES && LissEMissile != null && LissEMissile.Position.CountEnemiesInRange(W.Range - 50) <= ESE && Player.HealthPercent <= EHP)
            {
                E.Cast(Game.CursorPos);
            }

            if (useET && LissEMissile != null && LissEMissile.Position.IsInRange(target, 250))
            {
                E.Cast(Game.CursorPos);
            }

            if (useE2 && LissEMissile != null && LissEMissile.Position.IsInRange(LissEMissile.EndPosition, 50))
            {
                E.Cast(Game.CursorPos);
            }
        }

        private static void CastR()
        {
            var aoeR = UltMenu["aoeR"].Cast<CheckBox>().CurrentValue;
            var useRS = UltMenu["RS"].Cast<CheckBox>().CurrentValue && R.IsReady();
            var useRE = UltMenu["RE"].Cast<CheckBox>().CurrentValue && R.IsReady();
            var useRF = UltMenu["RF"].Cast<CheckBox>().CurrentValue && R.IsReady();
            var hitR = UltMenu["hitR"].Cast<Slider>().CurrentValue;
            var target =
                EntityManager.Heroes.Enemies.FirstOrDefault(
                    e =>
                    !e.IsZombie && !e.IsInvulnerable && !e.IsDead && !e.HasBuff("kindredrnodeathbuff") && !e.HasBuff("JudicatorIntervention")
                    && !e.HasBuff("ChronoShift") && !e.HasBuff("UndyingRage"));

            if (target != null && useRE)
            {
                if (aoeR && target.CountEnemiesInRange(R.Range) >= hitR && target.IsValidTarget(R.Range)
                    && !UltMenu["DontUltenemy" + target.BaseSkinName].Cast<CheckBox>().CurrentValue)
                {
                    R.Cast(target);
                }
            }

            if (useRS)
            {
                if (aoeR && Player.CountEnemiesInRange(R.Range) >= hitR && !UltMenu["DontUltally" + Player.BaseSkinName].Cast<CheckBox>().CurrentValue)
                {
                    R.Cast(Player);
                }
            }

            if (target != null && useRF)
            {
                if (target.TotalShieldHealth() < Player.GetSpellDamage(target, SpellSlot.R)
                    && !UltMenu["DontUltenemy" + target.BaseSkinName].Cast<CheckBox>().CurrentValue)
                {
                    if (target.IsValidTarget(R.Range))
                    {
                        R.Cast(target);
                    }

                    if (target.IsInRange(Player, R.Range) && !UltMenu["DontUltally" + Player.BaseSkinName].Cast<CheckBox>().CurrentValue)
                    {
                        R.Cast(Player);
                    }
                }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (!menuIni.Get<CheckBox>("Drawings").CurrentValue)
            {
                return;
            }

            if (DrawMenu.Get<CheckBox>("Q").CurrentValue && Q.IsLearned)
            {
                if (Q.IsReady())
                {
                    Circle.Draw(Color.Blue, Q.Range, ObjectManager.Player.Position);
                }

                if (!Q.IsReady())
                {
                    Circle.Draw(Color.DarkBlue, Q.Range, ObjectManager.Player.Position);
                }
            }

            if (DrawMenu.Get<CheckBox>("W").CurrentValue && W.IsLearned)
            {
                if (W.IsReady())
                {
                    Circle.Draw(Color.Blue, W.Range, ObjectManager.Player.Position);
                }

                if (!W.IsReady())
                {
                    Circle.Draw(Color.DarkBlue, W.Range, ObjectManager.Player.Position);
                }
            }

            if (DrawMenu.Get<CheckBox>("E").CurrentValue && E.IsLearned)
            {
                if (E.IsReady())
                {
                    Circle.Draw(Color.Blue, E.Range, ObjectManager.Player.Position);
                }

                if (!E.IsReady())
                {
                    Circle.Draw(Color.DarkBlue, E.Range, ObjectManager.Player.Position);
                }
            }

            if (DrawMenu.Get<CheckBox>("R").CurrentValue && R.IsLearned)
            {
                if (R.IsReady())
                {
                    Circle.Draw(Color.Blue, R.Range, ObjectManager.Player.Position);
                }

                if (!R.IsReady())
                {
                    Circle.Draw(Color.DarkBlue, R.Range, ObjectManager.Player.Position);
                }
            }

            if (DrawMenu["debug"].Cast<CheckBox>().CurrentValue)
            {
                var target =
                    EntityManager.Heroes.Enemies.FirstOrDefault(
                        e =>
                        !e.IsZombie && !e.IsDead && !e.HasBuff("kindredrnodeathbuff") && !e.HasBuff("JudicatorIntervention")
                        && !e.HasBuff("ChronoShift") && !e.HasBuff("UndyingRage") && e.IsHPBarRendered && e.IsEnemy);

                if (LissEMissile != null)
                {
                    Circle.Draw(Color.DarkBlue, W.Range, LissEMissile.Position);
                    Circle.Draw(Color.DarkBlue, W.Range, LissEMissile.EndPosition);
                }

                if (Player != null)
                {
                    var hpPosp = Player.HPBarPosition;
                    Circle.Draw(Color.DarkBlue, R.Range, Player.Position);
                    Drawing.DrawText(
                        hpPosp.X + 135f,
                        hpPosp.Y,
                        System.Drawing.Color.White,
                        "Enemies in Range " + Player.CountEnemiesInRange(R.Range),
                        10);
                }

                if (target != null)
                {
                    var hpPos = target.HPBarPosition;
                    Circle.Draw(Color.White, R.Range, target.Position);
                    Drawing.DrawText(
                        hpPos.X + 135f,
                        hpPos.Y,
                        System.Drawing.Color.White,
                        "Enemies in Range " + target.CountEnemiesInRange(R.Range).ToString(),
                        10);
                }
            }
        }
    }
}