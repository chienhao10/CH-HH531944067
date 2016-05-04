using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using Spell = LeagueSharp.Common.Spell;

namespace SKT_Series
{
    internal class Program
    {
        public const string ChampName = "Lulu";
        public static GameObject pix;
        public static AIHeroClient Player;
        public static Spell _Q, _Q2, _W, _E, _R;
        public static Menu _MainMenu, comboMenu, wMenu, autoMenu;

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

        public static void InitMenu()
        {
            _MainMenu = MainMenu.AddMenu("SKT T1_Lulu", "WawooK");

            comboMenu = _MainMenu.AddSubMenu("Combo", "Combo");
            comboMenu.Add("Combo_Q", new CheckBox("Use Q"));
            comboMenu.Add("Combo_EQ", new CheckBox("Use E+Q"));
            comboMenu.Add("Combo_E", new CheckBox("Use E"));
            comboMenu.Add("Combo_R", new CheckBox("Use R"));

            wMenu = _MainMenu.AddSubMenu("W", "W");
            wMenu.Add("Combo_W", new CheckBox("W"));
            foreach (var hero in HeroManager.Enemies)
            {
                wMenu.Add("Combo_W" + hero.ChampionName, new CheckBox("W " + hero.ChampionName, false));
            }

            autoMenu = _MainMenu.AddSubMenu("Auto Config", "Auto");
            autoMenu.Add("W_Gap", new CheckBox("Auto_W Anti"));
            autoMenu.Add("W_InR", new CheckBox("*W_Interrupt*"));
            autoMenu.Add("R_InR", new CheckBox("*R_Interrupt*"));
            autoMenu.Add("AutoE", new CheckBox("Auto E"));
            autoMenu.AddSeparator();
            autoMenu.AddGroupLabel("Auto R");
            autoMenu.Add("AREnable", new CheckBox("Enable"));
            autoMenu.Add("ARHP", new Slider("Auto_R HP %", 10));
        }

        public static void Game_OnGameLoad()
        {
            Player = ObjectManager.Player;
            if (Player.ChampionName != ChampName)
            {
                return;
            }
            _Q = new Spell(SpellSlot.Q, 950);
            _Q.SetSkillshot(0.25f, 60, 1450, false, SkillshotType.SkillshotLine);
            _Q2 = new Spell(SpellSlot.Q, 950);
            _Q2.SetSkillshot(0.25f, 60, 1450, false, SkillshotType.SkillshotLine);
            _W = new Spell(SpellSlot.W, 650);
            _E = new Spell(SpellSlot.E, 650);
            _R = new Spell(SpellSlot.R, 900);

            InitMenu();
            Game.OnUpdate += OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;

            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
        }

        // 픽스가져오기 . (Credit by HeavenStrikeLulu)
        public static void Getpixed()
        {
            if (Player.IsDead)
                pix = Player;
            if (!Player.IsDead)
                pix = ObjectManager.Get<GameObject>().Find(x => x.IsAlly && x.Name == "RobotBuddy") == null
                    ? Player
                    : ObjectManager.Get<GameObject>().Find(x => x.IsAlly && x.Name == "RobotBuddy");
        }


        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            // W 안티갭클로져
            var target = gapcloser.Sender;
            if (_W.IsReady() && target.IsValidTarget(_W.Range) && getCheckBoxItem(autoMenu, "W_Gap"))
            {
                _W.Cast(target);
            }
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            // W 스킬끊기
            if (_W.IsReady() && sender.IsValidTarget(_W.Range) && !sender.IsZombie && getCheckBoxItem(autoMenu, "W_InR"))
            {
                _W.Cast(sender);
            }
            // R 스킬끊기
            if (_R.IsReady() && sender.IsValidTarget() && !sender.IsZombie && getCheckBoxItem(autoMenu, "R_InR"))
            {
                var target = HeroManager.Allies.Where(x => x.IsValidTarget(_R.Range))
                    .OrderByDescending(x => 1 - x.Distance(sender.Position))
                    .Find(x => x.Distance(sender.Position) <= 350);
                if (target != null)
                    _R.Cast(target);
            }
        }

        //자동 E (같은팀에게) Credit for Korfresh :D.
        private static void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            try
            {
                if (sender is AIHeroClient && sender.IsEnemy && args.Target.IsAlly && !args.Target.IsDead &&
                    _E.IsReady() && args.Target.Position.Distance(Player.ServerPosition) <= _E.Range
                    && args.Target.Type != GameObjectType.obj_AI_Minion
                    && getCheckBoxItem(autoMenu, "AutoE"))
                {
                    var target =
                        ObjectManager.Get<AIHeroClient>()
                            .OrderBy(x => x.ServerPosition.Distance(args.Target.Position))
                            .FirstOrDefault(x => x.ServerPosition.Distance(args.Target.Position) < 5);
                    _E.Cast(target, true);
                }
                if (sender is AIHeroClient && sender.IsAlly && args.Target.IsEnemy && !args.Target.IsDead &&
                    _E.IsReady()
                    && args.Target.Type == GameObjectType.AIHeroClient
                    && getCheckBoxItem(autoMenu, "AutoE") &&
                    sender.ServerPosition.Distance(Player.ServerPosition) <= _E.Range)
                {
                    var Ally =
                        ObjectManager.Get<AIHeroClient>()
                            .OrderBy(x => x.ServerPosition.Distance(Player.ServerPosition))
                            .FirstOrDefault(
                                x => x.IsAlly && !x.IsMe && x.ServerPosition.Distance(Player.ServerPosition) < _E.Range);
                    if (sender.IsMe && Ally != null)
                    {
                    }
                    else
                    {
                        _E.Cast(sender, true);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private static void OnGameUpdate(EventArgs args)
        {
            Getpixed();
            var QTarget = TargetSelector.GetTarget(_Q.Range, DamageType.Magical);
            var ETarget = TargetSelector.GetTarget(_E.Range, DamageType.Magical);

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && getCheckBoxItem(comboMenu, "Combo_Q"))
                // 콤보키 눌렀을떄
            {
                if (_E.IsReady() && ETarget != null) // E 사용 
                    _E.CastOnUnit(ETarget, true);
                if (QTarget != null && _Q.IsReady()) // Q 사용
                {
                    //_Q.Cast(QTarget, true);
                    _Q.CastIfHitchanceEquals(QTarget, HitChance.Medium, true);
                }
                if (_W.IsReady() && getCheckBoxItem(wMenu, "Combo_W"))
                {
                    foreach (
                        var hero in
                            HeroManager.Enemies.Where(
                                x => x.IsValidTarget(_W.Range) && getCheckBoxItem(wMenu, "Combo_W" + x.ChampionName)))
                    {
                        _W.Cast(hero);
                    }
                }
                //EQ
                if (_Q.IsReady() && _E.IsReady() && Player.Mana >= _Q.Instance.SData.Mana + _E.Instance.SData.Mana &&
                    getCheckBoxItem(comboMenu, "Combo_EQ") && getCheckBoxItem(comboMenu, "Combo_E"))
                {
                    var hero = TargetSelector.GetTarget(_Q.Range + _E.Range, DamageType.Magical);
                    if (hero != null && hero.IsValidTarget() && !hero.IsValidTarget(_Q.Range))
                    {
                        // E 타겟 = 챔피언
                        foreach (
                            var target in
                                HeroManager.AllHeroes.Where(
                                    x => x.IsValidTarget(_E.Range) && x.Distance(hero.Position) <= _Q.Range)
                                    .OrderByDescending(y => 1 - y.Distance(hero.Position)))
                        {
                            _E.Cast(target);
                            _Q.Cast(hero);
                            _Q2.SetSkillshot(0.25f, 70, 1450, false, SkillshotType.SkillshotLine, pix.Position,
                                pix.Position);
                            _Q2.Cast(hero);
                        }
                        // E 타겟 = 미니언
                        foreach (
                            var target in
                                MinionManager.GetMinions(_E.Range, MinionTypes.All, MinionTeam.All)
                                    .Where(x => x.IsValidTarget(_E.Range)
                                                && !x.Name.ToLower().Contains("ward") &&
                                                x.Distance(hero.Position) <= _Q.Range)
                                    .OrderByDescending(y => 1 - y.Distance(hero.Position)))
                        {
                        }
                    }
                }
                if (_W.IsReady() && getCheckBoxItem(wMenu, "Combo_W"))
                {
                    foreach (
                        var hero in
                            HeroManager.Enemies.Where(
                                x => x.IsValidTarget(_W.Range) && getCheckBoxItem(wMenu, "Combo_W" + x.ChampionName)))
                    {
                        _W.Cast(hero);
                    }
                }
            }


            //자동 R 활성화 & 비활성화 & HP계산기
            if (getCheckBoxItem(autoMenu, "AREnable"))
            {
                var AutoR =
                    ObjectManager.Get<AIHeroClient>()
                        .OrderByDescending(x => x.Health)
                        .FirstOrDefault(
                            x =>
                                x.HealthPercent < getSliderItem(autoMenu, "ARHP") &&
                                x.Distance(Player.ServerPosition) < _R.Range && !x.IsDead && x.IsAlly);
                if (AutoR != null && _R.IsReady() && !Player.IsRecalling())
                {
                    _R.Cast(AutoR);
                }
            }
        }
    }
}