namespace Lulu
{
    using System;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;

    using SharpDX;

    internal class Program
    {
        private const string ChampionName = "Lulu";

        private static AIHeroClient Player;

        public static Menu Config;

        public static Spell.Skillshot Q { get; private set; }

        public static Spell.Skillshot Q2 { get; private set; }

        public static Spell.Targeted W { get; private set; }

        public static Spell.Targeted E { get; private set; }

        public static Spell.Targeted R { get; private set; }

        public static Menu UltMenu { get; private set; }

        public static Menu ComboMenu { get; private set; }

        public static Menu HarassMenu { get; private set; }

        public static Menu LaneMenu { get; private set; }

        public static Menu FleeMenu { get; private set; }

        public static Menu KillStealMenu { get; private set; }

        public static Menu MiscMenu { get; private set; }

        public static Menu ItemsMenu { get; private set; }

        public static Menu DrawMenu { get; private set; }

        public static Menu Saver { get; private set; }

        public static Menu menuIni;

        public static SpellSlot IgniteSlot;

        public static void Execute()
        {
            Player = ObjectManager.Player;
            if (Player.ChampionName != ChampionName)
            {
                return;
            }

            Q = new Spell.Skillshot(SpellSlot.Q, 925, SkillShotType.Linear, 250, 1450, 60);
            Q2 = new Spell.Skillshot(SpellSlot.Q, 1800, SkillShotType.Linear, 250, 1450, 60);
            Q.AllowedCollisionCount = int.MaxValue;
            W = new Spell.Targeted(SpellSlot.W, 650);
            E = new Spell.Targeted(SpellSlot.E, 650);
            R = new Spell.Targeted(SpellSlot.R, 900);

            IgniteSlot = Player.GetSpellSlotFromName("SummonerDot");

            PixManager.DrawPix = true;

            menuIni = MainMenu.AddMenu(ChampionName, ChampionName);
            menuIni.AddGroupLabel("Welcome to the Worst Lulu addon!");
            menuIni.AddGroupLabel("Global Settings");
            menuIni.Add("Combo", new CheckBox("Use Combo?"));
            menuIni.Add("Harass", new CheckBox("Use Harass?"));
            menuIni.Add("LaneClear", new CheckBox("Use LaneClear?"));
            menuIni.Add("Flee", new CheckBox("Use Flee?"));
            menuIni.Add("Saver", new CheckBox("Use Saver?"));
            menuIni.Add("Drawings", new CheckBox("Use Drawings?"));

            ComboMenu = menuIni.AddSubMenu("Combo");
            ComboMenu.AddGroupLabel("Combo Settings");
            ComboMenu.Add("Q", new CheckBox("Use Q"));
            ComboMenu.Add("W", new CheckBox("Use W"));
            ComboMenu.Add("E", new CheckBox("Use E"));
            ComboMenu.Add("Wkite", new CheckBox("Use W to Kite"));
            ComboMenu.Add("WkiteD", new Slider("W Kite distance", 300, 0, 500));

            HarassMenu = menuIni.AddSubMenu("Harass");
            HarassMenu.AddGroupLabel("Harass Settings");
            HarassMenu.Add("Q", new CheckBox("Use Q"));
            HarassMenu.Add("E", new CheckBox("Use E"));
            HarassMenu.Add("harassmana", new Slider("Harass Mana Manager", 60, 0, 100));

            LaneMenu = menuIni.AddSubMenu("Farm");
            LaneMenu.AddGroupLabel("LaneCelar Settings");
            LaneMenu.Add("Q", new CheckBox("Use Q"));
            LaneMenu.Add("E", new CheckBox("Use E"));
            LaneMenu.Add("lanemana", new Slider("Farm Mana Manager", 80, 0, 100));
            LaneMenu.AddGroupLabel("JungleClear Settings");
            LaneMenu.Add("QJ", new CheckBox("Use Q"));
            LaneMenu.Add("EJ", new CheckBox("Use E"));

            FleeMenu = menuIni.AddSubMenu("Flee");
            FleeMenu.AddGroupLabel("Flee Settings");
            FleeMenu.Add("Q", new CheckBox("Use Q"));
            FleeMenu.Add("exQ", new CheckBox("Use Extended Q", false));
            FleeMenu.Add("Wkite", new CheckBox("Use W to Kite"));
            FleeMenu.Add("WkiteD", new Slider("W Kite distance", 300, 0, 500));
            FleeMenu.Add("fleemana", new Slider("Flee Mana Manager", 60, 0, 100));

            MiscMenu = menuIni.AddSubMenu("Misc");
            MiscMenu.AddGroupLabel("Misc Settings");
            MiscMenu.Add("AutoE", new CheckBox("KS Enemy with E"));
            MiscMenu.Add("Support", new CheckBox("Support Mode", false));

            Saver = menuIni.AddSubMenu("Saver");
            Saver.AddGroupLabel("Saver Settings");
            Saver.AddGroupLabel("Anti GapCloser");
            Saver.Add("allywgapclose", new CheckBox("Use W On GapClosing Ally"));
            Saver.Add("enemywgapclose", new CheckBox("Use W On GapClosing Enemy"));
            Saver.Add("gapcloserR", new CheckBox("Use R On GapClosing Enemy"));
            Saver.AddGroupLabel("Interrupter");
            Saver.Add("InterruptSpellsW", new CheckBox("Use W Interrupt Spells"));
            Saver.Add("InterruptSpellsR", new CheckBox("Use R Interrupt Spells"));
            Saver.AddGroupLabel("Auto Shields");
            Saver.Add("AutoES", new CheckBox("Auto E Shields Allies"));
            Saver.Add("AutoR", new CheckBox("Auto R AoE || Saver"));
            Saver.AddSeparator();
            Saver.AddGroupLabel("Don't Use Saver On:");
            foreach (var ally in ObjectManager.Get<AIHeroClient>())
            {
                CheckBox cb = new CheckBox(ally.BaseSkinName);
                cb.CurrentValue = false;
                if (ObjectManager.Player.Team == ally.Team)
                {
                    Saver.Add("DontUlt" + ally.BaseSkinName, cb);
                }
            }

            DrawMenu = menuIni.AddSubMenu("Drawings");
            DrawMenu.AddGroupLabel("Drawing Settings");
            DrawMenu.Add("Q", new CheckBox("Draw Q"));
            DrawMenu.Add("PixQ", new CheckBox("Draw Pix Q Range"));
            DrawMenu.Add("W", new CheckBox("Draw W"));
            DrawMenu.Add("E", new CheckBox("Draw E"));
            DrawMenu.Add("R", new CheckBox("Draw R"));
            DrawMenu.Add("PixP", new CheckBox("Draw Pix Position"));

            Drawing.OnDraw += OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Interrupter.OnInterruptableSpell += Interrupter2_OnInterruptableTarget;
            Gapcloser.OnGapcloser += OnGapClose;
            Orbwalker.OnPreAttack += OnBeforeAttack;
            Obj_AI_Base.OnBasicAttack += OnBasicAttack;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
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

            if (target.IsAlly || target.IsMe)
            {
                if (menuIni.Get<CheckBox>("Saver").CurrentValue)
                {
                    if (Saver.Get<CheckBox>("AutoES").CurrentValue)
                    {
                        if (target.IsValidTarget(E.Range) && !target.IsDead && !Saver["DontUlt" + target.BaseSkinName].Cast<CheckBox>().CurrentValue)
                        {
                            var c = target.CountEnemiesInRange(750);
                            if (target.HealthPercent <= 25 && (c >= 1 || target.IsUnderEnemyturret()))
                            {
                                E.Cast(target);
                            }

                            if (target.HealthPercent <= 15 || caster.GetAutoAttackDamage(target) > Game.Time / 9
                                || caster.BaseAbilityDamage > Game.Time / 10 || caster.BaseAttackDamage > Game.Time / 9)
                            {
                                E.Cast(target);
                            }
                        }
                    }

                    if (Saver.Get<CheckBox>("AutoR").CurrentValue)
                    {
                        if (target.IsValidTarget(R.Range) && target != null && !Saver["DontUlt" + target.BaseSkinName].Cast<CheckBox>().CurrentValue)
                        {
                            var c = target.CountEnemiesInRange(300);
                            if (c >= 1 + 1 + 1 || target.HealthPercent <= 20 && c >= 1)
                            {
                                R.Cast(target);
                            }

                            if (target.HealthPercent <= 15 || caster.GetAutoAttackDamage(target) > target.Health
                                || caster.BaseAbilityDamage > target.Health || caster.BaseAttackDamage > target.Health)
                            {
                                R.Cast(target);
                            }
                        }
                    }
                }
            }
        }

        private static void OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
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

            if (target.IsAlly || target.IsMe)
            {
                if (menuIni.Get<CheckBox>("Saver").CurrentValue)
                {
                    if (Saver.Get<CheckBox>("AutoES").CurrentValue)
                    {
                        if (target.IsValidTarget(E.Range) && !target.IsDead && !Saver["DontUlt" + target.BaseSkinName].Cast<CheckBox>().CurrentValue)
                        {
                            var c = target.CountEnemiesInRange(600);
                            if (target.HealthPercent <= 25 && (c >= 1 || target.IsUnderEnemyturret()))
                            {
                                E.Cast(target);
                            }

                            if (target.HealthPercent <= 15 || caster.GetAutoAttackDamage(target) > Game.Time / 9
                                || caster.BaseAbilityDamage > Game.Time / 10 || caster.BaseAttackDamage > Game.Time / 9)
                            {
                                E.Cast(target);
                            }
                        }
                    }

                    if (Saver.Get<CheckBox>("AutoR").CurrentValue)
                    {
                        if (target.IsValidTarget(R.Range) && target != null && !Saver["DontUlt" + target.BaseSkinName].Cast<CheckBox>().CurrentValue)
                        {
                            var c = target.CountEnemiesInRange(300);
                            if (c >= 1 + 1 + 1 || target.HealthPercent <= 20 && c >= 1)
                            {
                                R.Cast(target);
                            }
                            if (target.HealthPercent <= 15 || caster.GetAutoAttackDamage(target) > target.Health
                                || caster.BaseAbilityDamage > target.Health || caster.BaseAttackDamage > target.Health)
                            {
                                R.Cast(target);
                            }
                        }
                    }
                }
            }
        }

        private static void OnBeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (MiscMenu["Support"].Cast<CheckBox>().CurrentValue)
            {
                if (args.Target.Type == GameObjectType.obj_AI_Minion)
                {
                    var alliesinrange = EntityManager.Heroes.Allies.Count(x => !x.IsMe && x.Distance(Player) <= 999);
                    if (alliesinrange > 0)
                    {
                        args.Process = false;
                    }
                }
            }
        }

        private static void OnGapClose(AIHeroClient Sender, Gapcloser.GapcloserEventArgs args)
        {
            if (!menuIni.Get<CheckBox>("Saver").CurrentValue || Sender == null)
            {
                return;
            }

            if (Saver["allywgapclose"].Cast<CheckBox>().CurrentValue && Sender.IsAlly && Sender.IsValidTarget(W.Range))
            {
                W.Cast(Sender);
            }

            if (Saver["Enemywgapclose"].Cast<CheckBox>().CurrentValue && Sender.IsEnemy && Sender.IsValidTarget(W.Range))
            {
                W.Cast(Sender);
            }

            if (Saver.Get<CheckBox>("gapcloserR").CurrentValue)
            {
                if (R.IsReady())
                {
                    foreach (var ally in EntityManager.Heroes.Allies)
                    {
                        if (ally.IsValidTarget(R.Range) && !Saver["DontUlt" + ally.BaseSkinName].Cast<CheckBox>().CurrentValue && Sender.IsEnemy)
                        {
                            if (ally.Distance(Sender, true) < 300 * 300 && ally.HealthPercent <= 25)
                            {
                                R.Cast(ally);
                            }
                        }
                    }

                    if (Player.Distance(Sender, true) < 300 * 300 && Sender.IsEnemy && Player.HealthPercent <= 20)
                    {
                        R.Cast(Player);
                    }
                }
            }
        }

        private static void Interrupter2_OnInterruptableTarget(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {
            if (!menuIni.Get<CheckBox>("Saver").CurrentValue)
            {
                return;
            }

            if (!sender.IsValidTarget() || !sender.IsEnemy || sender.IsAlly || sender.IsMe || sender == null)
            {
                return;
            }

            if (Saver.Get<CheckBox>("InterruptSpellsW").CurrentValue)
            {
                if (W.IsReady() && sender.IsValidTarget(W.Range))
                {
                    W.Cast(sender);
                    return;
                }
            }

            if (Saver.Get<CheckBox>("InterruptSpellsR").CurrentValue)
            {
                if (R.IsReady())
                {
                    foreach (var ally in EntityManager.Heroes.AllHeroes)
                    {
                        if (ally.IsValidTarget(R.Range) && !Saver["DontUlt" + ally.BaseSkinName].Cast<CheckBox>().CurrentValue)
                        {
                            if (ally.Distance(sender, true) < 300 * 300 && ally.HealthPercent < 25)
                            {
                                R.Cast(ally);
                            }
                        }
                    }

                    if (Player.Distance(sender, true) < 300 * 300 && Player.HealthPercent < 20)
                    {
                        R.Cast(Player);
                    }
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            var flags = Orbwalker.ActiveModesFlags;
            if (flags.HasFlag(Orbwalker.ActiveModes.Combo) && menuIni.Get<CheckBox>("Combo").CurrentValue)
            {
                Combo();
            }

            if (ObjectManager.Player.ManaPercent > HarassMenu["harassmana"].Cast<Slider>().CurrentValue)
            {
                if (flags.HasFlag(Orbwalker.ActiveModes.Harass) && menuIni.Get<CheckBox>("Harass").CurrentValue)
                {
                    Harass();
                }
            }

            if (ObjectManager.Player.ManaPercent > LaneMenu["lanemana"].Cast<Slider>().CurrentValue)
            {
                if (flags.HasFlag(Orbwalker.ActiveModes.LaneClear) && menuIni.Get<CheckBox>("LaneClear").CurrentValue)
                {
                    Farm();
                }
            }

            if (ObjectManager.Player.ManaPercent > LaneMenu["lanemana"].Cast<Slider>().CurrentValue)
            {
                if (flags.HasFlag(Orbwalker.ActiveModes.JungleClear) && menuIni.Get<CheckBox>("LaneClear").CurrentValue)
                {
                    JungleFarm();
                }
            }

            if (ObjectManager.Player.ManaPercent > FleeMenu["fleemana"].Cast<Slider>().CurrentValue)
            {
                if (flags.HasFlag(Orbwalker.ActiveModes.Flee) && menuIni.Get<CheckBox>("Flee").CurrentValue)
                {
                    Flee();
                }
            }

            if (flags.HasFlag(Orbwalker.ActiveModes.None))
            {
                Orbwalker.DisableAttacking = false;
            }

            if (MiscMenu.Get<CheckBox>("AutoE").CurrentValue)
            {
                ImABitch();
            }

            if (Saver.Get<CheckBox>("AutoR").CurrentValue)
            {
                foreach (var ally in EntityManager.Heroes.Allies)
                {
                    if (ally.IsValidTarget(R.Range) && ally != null && !Saver["DontUlt" + ally.BaseSkinName].Cast<CheckBox>().CurrentValue)
                    {
                        var c = ally.CountEnemiesInRange(300);
                        if (c >= 1 + 1 + 1 || ally.HealthPercent <= 20 && c >= 1)
                        {
                            R.Cast(ally);
                        }
                    }
                }

                var ec = Player.CountEnemiesInRange(300);
                if ((ec >= 1 + 1 + 1 || Player.HealthPercent <= 25 && ec >= 1) && Player != null)
                {
                    R.Cast(Player);
                }
            }
        }

        private static void ShootQ(bool useE = true)
        {
            if (!Q.IsReady())
            {
                return;
            }

            Obj_AI_Base pixTarget = null;
            if (PixManager.Pix != null)
            {
                pixTarget = TargetSelector.GetTarget(Q.Range + E.Range, DamageType.Magical);
            }

            Obj_AI_Base luluTarget = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            var pixTargetEffectiveHealth = pixTarget != null ? pixTarget.Health * (1 + pixTarget.SpellBlock / 100f) : float.MaxValue;
            var luluTargetEffectiveHealth = luluTarget != null ? luluTarget.Health * (1 + luluTarget.SpellBlock / 100f) : float.MaxValue;

            var target = pixTargetEffectiveHealth * 1.2f > luluTargetEffectiveHealth ? luluTarget : pixTarget;
            var flag = false;
            bool qCastState = !Q.IsInRange(target);
            if (target != null)
            {
                var distanceToTargetFromPlayer = Player.Distance(target, true);
                var distanceToTargetFromPix = PixManager.Pix != null ? PixManager.Pix.Distance(target, true) : float.MaxValue;

                var source = PixManager.Pix == null ? Player : (distanceToTargetFromPix < distanceToTargetFromPlayer ? PixManager.Pix : Player);
                Q.SourcePosition = source.ServerPosition;
                Q.RangeCheckSource = source.ServerPosition;
                if (!useE || !E.IsReady() || source.ServerPosition.Distance(target.ServerPosition) < Q.Range - 100)
                {
                    qCastState = Q.Cast(target);
                }

                flag = true;
            }

            if (target == null)
            {
                return;
            }

            if (qCastState == !Q.IsInRange(target) && Q.Handle.SData.Mana + E.Handle.SData.Mana < Player.Mana)
            {
                // or outofrange
                if (useE && E.IsReady())
                {
                    var eqTarget = TargetSelector.GetTarget(Q.Range + E.Range, DamageType.Magical);
                    if (eqTarget != null)
                    {
                        var eTarget =
                            ObjectManager.Get<Obj_AI_Base>()
                                .Where(
                                    t =>
                                    t.IsValidTarget(E.Range) && t != null && !t.IsMe && t.Distance(eqTarget, true) < Q.RangeSquared
                                    && Player.GetSpellDamage(eqTarget, SpellSlot.E) < eqTarget.TotalShieldHealth())
                                .FirstOrDefault(t => t.Distance(eqTarget) < 1750);
                        if (eTarget != null)
                        {
                            E.Cast(eTarget);
                            return;
                        }
                    }
                }

                if (flag)
                {
                    var predeq = Q2.GetPrediction(target);
                    qCastState = Q.Cast(predeq.CastPosition);
                }
            }
        }

        private static void Flee()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (FleeMenu["Q"].Cast<CheckBox>().CurrentValue && Q.IsReady())
            {
                if (target != null)
                {
                    var pred = Q.GetPrediction(target);
                    Q.Cast(pred.CastPosition);
                }
            }

            if (FleeMenu["exQ"].Cast<CheckBox>().CurrentValue)
            {
                ShootQ();
            }

            if (FleeMenu.Get<CheckBox>("WKite").CurrentValue && W.IsReady())
            {
                var d = FleeMenu.Get<Slider>("WKiteD").CurrentValue;
                if (Player != null && Player.CountEnemiesInRange(d) >= 1)
                {
                    W.Cast(Player);
                }
            }
        }

        private static void Harass()
        {
            if (HarassMenu["Q"].Cast<CheckBox>().CurrentValue)
            {
                ShootQ();
            }

            if (HarassMenu["E"].Cast<CheckBox>().CurrentValue)
            {
                var eTarget = TargetSelector.GetTarget(E.Range, DamageType.Magical);
                if (eTarget != null)
                {
                    E.Cast(eTarget);
                }

                var comboDamage = GetComboDamage(eTarget);
            }
        }

        private static void Combo()
        {
            if (ComboMenu["Q"].Cast<CheckBox>().CurrentValue)
            {
                ShootQ();
            }

            var eTarget = TargetSelector.GetTarget(E.Range, DamageType.Magical);

            if (ComboMenu["E"].Cast<CheckBox>().CurrentValue)
            {
                if (eTarget != null)
                {
                    E.Cast(eTarget);
                }
            }

            var comboDamage = GetComboDamage(eTarget);

            if (eTarget != null && Player.Distance(eTarget) < 600 && IgniteSlot != SpellSlot.Unknown
                && Player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
            {
                if (comboDamage > eTarget.Health)
                {
                    Player.Spellbook.CastSpell(IgniteSlot, eTarget);
                }
            }

            if (ComboMenu.Get<CheckBox>("WKite").CurrentValue && W.IsReady())
            {
                var d = ComboMenu.Get<Slider>("WKiteD").CurrentValue;
                if (Player != null && Player.CountEnemiesInRange(d) >= 1)
                {
                    W.Cast(Player);
                }
            }
        }

        private static void Farm()
        {
            var useQ = LaneMenu["Q"].Cast<CheckBox>().CurrentValue;
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
                foreach (var minion in allMinions)
                {
                    allMinions.Any();
                    {
                        var fl = EntityManager.MinionsAndMonsters.GetLineFarmLocation(allMinions, Q.Width, (int)Q.Range);
                        if (fl.HitNumber >= 1)
                        {
                            Q.Cast(fl.CastPosition);
                        }
                    }
                }

                Q.SourcePosition = Player.ServerPosition;
                Q.RangeCheckSource = Player.ServerPosition;
            }

            if (useE)
            {
                foreach (var minion in
                    allMinions.Where(m => m.BaseSkinName.EndsWith("MinionSiege") && Player.GetSpellDamage(m, SpellSlot.E) > m.TotalShieldHealth()))
                {
                    E.Cast(minion);
                }
            }
        }

        private static void JungleFarm()
        {
            var useQ = LaneMenu["QJ"].Cast<CheckBox>().CurrentValue;
            var useE = LaneMenu["EJ"].Cast<CheckBox>().CurrentValue;

            var mobs = ObjectManager.Get<Obj_AI_Minion>().OrderBy(m => m.Health).Where(m => m != null && m.IsMonster && !m.IsDead);
            foreach (var mob in mobs)
            {
                if (useQ && Q.IsReady())
                {
                    Q.Cast(mob.Position);
                }
                else if (useE && E.IsReady())
                {
                    E.Cast(mob);
                }
            }
        }

        private static void ImABitch()
        {
            foreach (var enemy in
                EntityManager.Heroes.Enemies.Where(
                    e => e != null && e.IsValidTarget(E.Range) && e.IsEnemy && Player.GetSpellDamage(e, SpellSlot.E) > e.TotalShieldHealth()))
            {
                E.Cast(enemy);
            }
        }

        public static float GetComboDamage(AIHeroClient target)
        {
            var result = 0f;

            if (target == null)
            {
                return 0f;
            }

            if (Q.IsReady())
            {
                result += 2 * Player.GetSpellDamage(target, SpellSlot.Q);
            }

            if (E.IsReady())
            {
                result += Player.GetSpellDamage(target, SpellSlot.E);
            }

            result += 3 * (float)Player.GetAutoAttackDamage(target);

            return result;
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
                    Circle.Draw(Color.MediumPurple, Q.Range, ObjectManager.Player.Position);
                }

                if (!Q.IsReady())
                {
                    Circle.Draw(Color.OrangeRed, Q.Range, ObjectManager.Player.Position);
                }
            }

            if (DrawMenu.Get<CheckBox>("W").CurrentValue && W.IsLearned)
            {
                if (W.IsReady())
                {
                    Circle.Draw(Color.MediumPurple, W.Range, ObjectManager.Player.Position);
                }

                if (!W.IsReady())
                {
                    Circle.Draw(Color.OrangeRed, W.Range, ObjectManager.Player.Position);
                }
            }

            if (DrawMenu.Get<CheckBox>("E").CurrentValue && E.IsLearned)
            {
                if (E.IsReady())
                {
                    Circle.Draw(Color.MediumPurple, E.Range, ObjectManager.Player.Position);
                }

                if (!E.IsReady())
                {
                    Circle.Draw(Color.OrangeRed, E.Range, ObjectManager.Player.Position);
                }
            }

            if (DrawMenu.Get<CheckBox>("R").CurrentValue && R.IsLearned)
            {
                if (R.IsReady())
                {
                    Circle.Draw(Color.MediumPurple, R.Range, ObjectManager.Player.Position);
                }

                if (!R.IsReady())
                {
                    Circle.Draw(Color.OrangeRed, R.Range, ObjectManager.Player.Position);
                }
            }
        }
    }
}