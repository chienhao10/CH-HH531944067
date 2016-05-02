namespace JustWukong
{
    using System;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;

    using SharpDX;

    internal class Program
    {
        public static readonly Item Cutlass = new Item((int)ItemId.Bilgewater_Cutlass, 550);

        public static readonly Item Botrk = new Item((int)ItemId.Blade_of_the_Ruined_King, 550);

        public static readonly Item Youmuu = new Item((int)ItemId.Youmuus_Ghostblade);

        public const string ChampName = "MonkeyKing";

        public const string Menuname = "Wukong";

        public static HpBarIndicator Hpi = new HpBarIndicator();

        public static Menu Config;

        public static Spell.Active Q { get; private set; }

        public static Spell.Active W { get; private set; }

        public static Spell.Targeted E { get; private set; }

        public static Spell.Active R { get; private set; }

        public static Menu UltMenu { get; private set; }

        public static Menu ComboMenu { get; private set; }

        public static Menu HarassMenu { get; private set; }

        public static Menu LaneMenu { get; private set; }

        public static Menu KillStealMenu { get; private set; }

        public static Menu MiscMenu { get; private set; }

        public static Menu ItemsMenu { get; private set; }

        public static Menu DrawMenu { get; private set; }

        public static Menu test { get; private set; }

        private static Menu menuIni;

        public static readonly AIHeroClient player = ObjectManager.Player;

        public static void Execute()
        {
            if (player.ChampionName != ChampName)
            {
                return;
            }

            //Ability Information - Range - Variables.

            Q = new Spell.Active(SpellSlot.Q, 375);
            W = new Spell.Active(SpellSlot.W, 0);
            E = new Spell.Targeted(SpellSlot.E, 640);
            R = new Spell.Active(SpellSlot.R, 375);

            menuIni = MainMenu.AddMenu("Wukong ", "Wukong");
            menuIni.AddGroupLabel("Welcome to the Worst Wukong addon!");
            menuIni.AddGroupLabel("Global Settings");
            menuIni.Add("Items", new CheckBox("Use Items?"));
            menuIni.Add("Ultimate", new CheckBox("Use Ultimate?"));
            menuIni.Add("Combo", new CheckBox("Use Combo?"));
            menuIni.Add("Harass", new CheckBox("Use Harass?"));
            menuIni.Add("LaneClear", new CheckBox("Use LaneClear?"));
            menuIni.Add("KillSteal", new CheckBox("Use Kill Steal?"));
            menuIni.Add("Misc", new CheckBox("Use Misc?"));
            menuIni.Add("Drawings", new CheckBox("Use Drawings?"));

            ItemsMenu = menuIni.AddSubMenu("Items");
            ItemsMenu.AddGroupLabel("Items Settings");
            ItemsMenu.Add("useGhostblade", new CheckBox("Use Youmuu's Ghostblade"));
            ItemsMenu.Add("UseBOTRK", new CheckBox("Use Blade of the Ruined King"));
            ItemsMenu.Add("UseBilge", new CheckBox("Use Bilgewater Cutlass"));
            ItemsMenu.Add("eL", new Slider("Use On Enemy health", 65, 0, 100));
            ItemsMenu.Add("oL", new Slider("Use On My health", 65, 0, 100));

            UltMenu = menuIni.AddSubMenu("Ultimate");
            UltMenu.AddGroupLabel("Ultimate Settings");
            UltMenu.Add("stickR", new CheckBox("Stick to Target While R Is active", false));
            UltMenu.Add("interrupt", new CheckBox("Interrupt Spells (R)"));
            UltMenu.Add("tower", new CheckBox("Auto R Under Tower"));
            UltMenu.Add("saveR", new CheckBox("Disable AA While R Active"));

            ComboMenu = menuIni.AddSubMenu("Combo");
            ComboMenu.AddGroupLabel("Combo Settings");
            ComboMenu.Add("UseQ", new CheckBox("Use Q"));
            ComboMenu.Add("UseW", new CheckBox("Use W", false));
            ComboMenu.Add("UseE", new CheckBox("Use E"));
            ComboMenu.Add("UseR", new CheckBox("Use R"));
            ComboMenu.Add("Rene", new Slider("Min Enemies for R", 1, 1, 5));

            HarassMenu = menuIni.AddSubMenu("Harass");
            HarassMenu.AddGroupLabel("Harass Settings");
            HarassMenu.Add("hQ", new CheckBox("Use Q"));
            HarassMenu.Add("hW", new CheckBox("Use W", false));
            HarassMenu.Add("hE", new CheckBox("Use E"));
            HarassMenu.Add("harassmana", new Slider("Harass Mana Manager", 60, 0, 100));

            LaneMenu = menuIni.AddSubMenu("Farm");
            LaneMenu.AddGroupLabel("Farm Settings");
            LaneMenu.Add("laneQ", new CheckBox("Use Q"));
            LaneMenu.Add("laneE", new CheckBox("Use E"));
            LaneMenu.Add("lanemana", new Slider("Farm Mana Manager", 80, 0, 100));

            KillStealMenu = menuIni.AddSubMenu("Kill Steal");
            KillStealMenu.AddGroupLabel("Kill Steal Settings");
            KillStealMenu.Add("ksQ", new CheckBox("Kill Steal Q"));
            KillStealMenu.Add("ksE", new CheckBox("Kill Steal E"));

            MiscMenu = menuIni.AddSubMenu("Misc");
            MiscMenu.AddGroupLabel("Misc Settings");
            MiscMenu.Add("gapcloser", new CheckBox("Use W On GapCloser"));
            MiscMenu.Add("gapclosermana", new Slider("Anti-GapCloser Mana", 25, 0, 100));

            DrawMenu = menuIni.AddSubMenu("Drawings");
            DrawMenu.AddGroupLabel("Drawing Settings");
            DrawMenu.Add("Qdraw", new CheckBox("Draw Q"));
            DrawMenu.Add("Wdraw", new CheckBox("Draw W"));
            DrawMenu.Add("Edraw", new CheckBox("Draw E"));
            DrawMenu.Add("Rdraw", new CheckBox("Draw R"));
            DrawMenu.Add("DrawD", new CheckBox("Draw Damage"));

            Drawing.OnDraw += OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Spellbook.OnCastSpell += OnCastSpell;
            Drawing.OnEndScene += OnEndScene;
            Interrupter.OnInterruptableSpell += Interrupter2_OnInterruptableTarget;
            Gapcloser.OnGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        private static void Interrupter2_OnInterruptableTarget(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {
            if (sender != null && R.IsReady() && sender.IsEnemy && sender.IsValidTarget(R.Range) && UltMenu.Get<CheckBox>("interrupt").CurrentValue)
            {
                R.Cast();
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(AIHeroClient Sender, Gapcloser.GapcloserEventArgs args)
        {
            if (IsCastingR())
            {
                return;
            }

            if (menuIni["Misc"].Cast<CheckBox>().CurrentValue && player.ManaPercent > MiscMenu.Get<Slider>("gapclosermana").CurrentValue)
            {
                if (Sender != null && W.IsReady() && Sender.IsEnemy && Sender.IsValidTarget(Q.Range)
                    && MiscMenu.Get<CheckBox>("gapcloser").CurrentValue)
                {
                    W.Cast();
                }
            }
        }

        private static void OnEndScene(EventArgs args)
        {
            if (menuIni["Drawings"].Cast<CheckBox>().CurrentValue && DrawMenu["DrawD"].Cast<CheckBox>().CurrentValue)
            {
                foreach (var enemy in
                    ObjectManager.Get<AIHeroClient>().Where(ene => ene != null && !ene.IsDead && ene.IsEnemy && ene.IsVisible && ene.IsValid))
                {
                    Hpi.unit = enemy;
                    Hpi.drawDmg(CalcDamage(enemy), System.Drawing.Color.Goldenrod);
                }
            }
        }

        private static void OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender.Owner.IsMe && (args.Slot == SpellSlot.Q || args.Slot == SpellSlot.W || args.Slot == SpellSlot.E))
            {
                if (player.HasBuff("MonkeyKingSpinToWin"))
                {
                    args.Process = false;
                }
            }
        }

        private static bool IsCastingR()
        {
            if (ObjectManager.Player.HasBuff("MonkeyKingSpinToWin"))
            {
                return true;
            }

            return false;
        }

        private static void combo()
        {
            var target = TargetSelector.GetTarget(1000, DamageType.Physical);
            if (target == null || !target.IsValidTarget())
            {
                return;
            }

            if (IsCastingR())
            {
                return;
            }

            var enemys = ComboMenu["Rene"].Cast<Slider>().CurrentValue;
            if (R.IsReady() && ComboMenu["UseR"].Cast<CheckBox>().CurrentValue && target.IsValidTarget(R.Range))
            {
                if (enemys >= ObjectManager.Player.CountEnemiesInRange(375))
                {
                    R.Cast();
                }
            }

            if (E.IsReady() && target.IsValidTarget(E.Range) && ComboMenu["UseE"].Cast<CheckBox>().CurrentValue)
            {
                E.Cast(target);
            }

            if (Q.IsReady() && ComboMenu["UseQ"].Cast<CheckBox>().CurrentValue && target.IsValidTarget(Q.Range))
            {
                Q.Cast();
            }

            if (menuIni["Items"].Cast<CheckBox>().CurrentValue)
            {
                items();
            }

            if (W.IsReady() && ComboMenu["UseW"].Cast<CheckBox>().CurrentValue && target.IsValidTarget(Q.Range))
            {
                W.Cast();
            }
        }

        private static int CalcDamage(Obj_AI_Base target)
        {
            var aa = ObjectManager.Player.GetAutoAttackDamage(target, true) * ObjectManager.Player.Crit / 5;
            var damage = aa;

            if (ObjectManager.Player.HasItem(3153) && Item.CanUseItem(3153))
            {
                damage += ObjectManager.Player.GetItemDamage(target, (ItemId)3153); //ITEM BOTRK
            }

            if (ObjectManager.Player.HasItem(3144) && Item.CanUseItem(3144))
            {
                damage += ObjectManager.Player.GetItemDamage(target, (ItemId)3144); //ITEM BOTRK
            }

            if (R.IsReady() && ComboMenu["UseR"].Cast<CheckBox>().CurrentValue) // rdamage
            {
                if (R.IsReady())
                {
                    damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.R);
                }
            }

            if (Q.IsReady() && ComboMenu["UseQ"].Cast<CheckBox>().CurrentValue) // qdamage
            {
                damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q);
            }

            if (E.IsReady() && ComboMenu["UseE"].Cast<CheckBox>().CurrentValue) // edamage
            {
                damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.E);
            }

            return (int)damage;
        }

        private static void Killsteal()
        {
            foreach (AIHeroClient target in
                ObjectManager.Get<AIHeroClient>()
                    .Where(hero => hero.IsValidTarget(Q.Range) && !hero.HasBuffOfType(BuffType.Invulnerability) && hero.IsEnemy && hero != null))
            {
                if (target == null || IsCastingR())
                {
                    return;
                }

                var qDmg = player.GetSpellDamage(target, SpellSlot.Q);
                if (KillStealMenu["ksQ"].Cast<CheckBox>().CurrentValue && Q.IsReady() && target.IsValidTarget(Q.Range) && target.Health <= qDmg)
                {
                    Q.Cast();
                    Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                }

                var eDmg = player.GetSpellDamage(target, SpellSlot.E);
                if (KillStealMenu["ksE"].Cast<CheckBox>().CurrentValue && E.IsReady() && target.IsValidTarget(E.Range) && target.Health <= eDmg)
                {
                    E.Cast(target);
                }
            }
        }

        private static void items()
        {
            if (IsCastingR())
            {
                return;
            }
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (target == null || !target.IsValidTarget())
            {
                return;
            }

            if (Botrk.IsReady() && Botrk.IsOwned(player) && Botrk.IsInRange(target)
                && target.HealthPercent <= ItemsMenu["eL"].Cast<Slider>().CurrentValue && ItemsMenu["UseBOTRK"].Cast<CheckBox>().CurrentValue)
            {
                Botrk.Cast(target);
            }

            if (Botrk.IsReady() && Botrk.IsOwned(player) && Botrk.IsInRange(target)
                && target.HealthPercent <= ItemsMenu["oL"].Cast<Slider>().CurrentValue && ItemsMenu["UseBOTRK"].Cast<CheckBox>().CurrentValue)

            {
                Botrk.Cast(target);
            }

            if (Cutlass.IsReady() && Cutlass.IsOwned(player) && Cutlass.IsInRange(target)
                && target.HealthPercent <= ItemsMenu["eL"].Cast<Slider>().CurrentValue && ItemsMenu["UseBilge"].Cast<CheckBox>().CurrentValue)
            {
                Cutlass.Cast(target);
            }

            if (Youmuu.IsReady() && Youmuu.IsOwned(player) && target.IsValidTarget(Q.Range)
                && ItemsMenu["useGhostblade"].Cast<CheckBox>().CurrentValue)
            {
                Youmuu.Cast();
            }
        }

        private static void UnderTower()
        {
            var Target = TargetSelector.GetTarget(R.Range, DamageType.Physical);

            if (Target != null && R.IsReady() && Target.IsUnderTurret() && !Target.IsUnderEnemyturret() && R.IsReady())
            {
                R.Cast();
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (player.IsDead || MenuGUI.IsChatOpen || player.IsRecalling())
            {
                return;
            }

            var flags = Orbwalker.ActiveModesFlags;
            if (flags.HasFlag(Orbwalker.ActiveModes.Combo) && menuIni.Get<CheckBox>("Combo").CurrentValue)
            {
                combo();
            }

            if (flags.HasFlag(Orbwalker.ActiveModes.LaneClear) && menuIni.Get<CheckBox>("LaneClear").CurrentValue)
            {
                Clear();
            }
            if (flags.HasFlag(Orbwalker.ActiveModes.Harass) && menuIni.Get<CheckBox>("Harass").CurrentValue)
            {
                harass();
            }

            if (menuIni["Killsteal"].Cast<CheckBox>().CurrentValue)
            {
                Killsteal();
            }

            //Flee();

            if (menuIni["Ultimate"].Cast<CheckBox>().CurrentValue)
            {
                if (UltMenu["saveR"].Cast<CheckBox>().CurrentValue)
                {
                    Orbwalker.DisableAttacking = IsCastingR();
                }

                if (IsCastingR())
                {
                    if (UltMenu["stickR"].Cast<CheckBox>().CurrentValue)
                    {
                        var target = TargetSelector.GetTarget(750, DamageType.Physical);
                        if (target == null || !target.IsValidTarget())
                        {
                            return;
                        }

                        Player.IssueOrder(GameObjectOrder.MoveTo, target.Position);
                    }
                }

                if (UltMenu["tower"].Cast<CheckBox>().CurrentValue)
                {
                    UnderTower();
                }
            }
        }

        private static void harass()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            var harassmana = HarassMenu.Get<Slider>("harassmana").CurrentValue;
            if (target == null || !target.IsValidTarget() || IsCastingR())
            {
                return;
            }

            if (E.IsReady() && HarassMenu.Get<CheckBox>("hE").CurrentValue && target.IsValidTarget(E.Range) && player.ManaPercent >= harassmana)
            {
                E.Cast(target);
            }

            if (Q.IsReady() && HarassMenu.Get<CheckBox>("hQ").CurrentValue && target.IsValidTarget(Q.Range) && player.ManaPercent >= harassmana)
            {
                Q.Cast();
            }
        }

        private static void Clear()
        {
            if (IsCastingR())
            {
                return;
            }
            var lanemana = LaneMenu["lanemana"].Cast<Slider>().CurrentValue;
            var Qlane = LaneMenu["laneQ"].Cast<CheckBox>().CurrentValue && Q.IsReady();
            var Elane = LaneMenu["laneE"].Cast<CheckBox>().CurrentValue && E.IsReady();

            var minions = ObjectManager.Get<Obj_AI_Minion>().OrderBy(m => m.Health).Where(m => m.IsMinion && m.IsEnemy && !m.IsDead);

            if (lanemana <= Player.Instance.ManaPercent)
            {
                foreach (var minion in minions)
                {
                    {
                        if (Qlane && !minion.IsValidTarget(player.AttackRange) && minion.IsValidTarget(Q.Range)
                            && minion.Health <= player.GetSpellDamage(minion, SpellSlot.Q) && minions.Count() > 1)
                        {
                            Q.Cast();
                        }
                    }

                    if (Elane && E.IsReady() && minion.Health <= player.GetSpellDamage(minion, SpellSlot.E)
                        && !minion.IsValidTarget(player.AttackRange) && !minion.IsUnderEnemyturret())
                    {
                        E.Cast(minion);
                    }
                }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (!menuIni["Drawings"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            if (DrawMenu["Qdraw"].Cast<CheckBox>().CurrentValue && Q.IsReady())
            {
                Circle.Draw(Color.White, Q.Range, Player.Instance.Position);
            }

            if (DrawMenu["Edraw"].Cast<CheckBox>().CurrentValue && E.IsReady())
            {
                Circle.Draw(Color.White, E.Range, Player.Instance.Position);
            }

            if (DrawMenu["Rdraw"].Cast<CheckBox>().CurrentValue && R.IsReady())
            {
                Circle.Draw(Color.DarkOrange, R.Range, Player.Instance.Position);
            }
        }
    }
}