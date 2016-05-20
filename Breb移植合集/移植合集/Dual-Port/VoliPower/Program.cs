using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
using Spell = LeagueSharp.Common.Spell;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace VoliPower
{
    class Program
    {
        private const string ChampionName = "Volibear";
        public static AIHeroClient Player;
        public static Menu Menu { get; set; }
        public static Menu comboMenu, laneclearing, fleeMenu, misc, drawingMenu;
        private static Spell _q, _w, _e, _r;
        static Items.Item _botrk, _cutlass;

        public static HpBarIndicator Hpi = new HpBarIndicator();

        //credits Kurisu
        private static readonly int[] SmitePurple = { 3713, 3726, 3725, 3726, 3723 };
        private static readonly int[] SmiteGrey = { 3711, 3722, 3721, 3720, 3719 };
        private static readonly int[] SmiteRed = { 3715, 3718, 3717, 3716, 3714 };
        private static readonly int[] SmiteBlue = { 3706, 3710, 3709, 3708, 3707 };


       public static void Game_OnLoad()
        {
            #region main
            {
                Player = ObjectManager.Player;

                if (Player.ChampionName != ChampionName)
                {
                    return;
                }

                _q = new Spell(SpellSlot.Q, 600, DamageType.Physical);
                _w = new Spell(SpellSlot.W, 405, DamageType.Physical);
                _e = new Spell(SpellSlot.E, 400, DamageType.Magical);
                _r = new Spell(SpellSlot.R, 125, DamageType.Magical);

                _botrk = new Items.Item(3153, 450f);
                _cutlass = new Items.Item(3144, 450f);

            }
            #endregion

            #region content menu

            Menu = MainMenu.AddMenu("Teddy Bear - ThunderBuddy", "teddy.bear");

            comboMenu = Menu.AddSubMenu("Combo", "teddy.bear.combo");
            comboMenu.Add("teddy.bear.combo.useq", new CheckBox("Use Q", true));
            comboMenu.Add("teddy.bear.combo.usew", new CheckBox("Use W", true));
            comboMenu.Add("teddy.bear.combo.usee", new CheckBox("Use E", true));
            comboMenu.Add("teddy.bear.combo.user", new CheckBox("Use R", true));

            laneclearing = Menu.AddSubMenu("Lane clear", "teddy.bear.laneclearing");
            laneclearing.Add("teddy.bear.laneclearing.useQ", new CheckBox("Use Q", true));
            laneclearing.Add("teddy.bear.laneclearing.useW", new CheckBox("Use W", true));
            laneclearing.Add("teddy.bear.laneclearing.useE", new CheckBox("Use E", true));


            fleeMenu = Menu.AddSubMenu("Flee", "teddy.bear.flee");
            fleeMenu.Add("teddy.bear.flee.useQ", new CheckBox("Use Q", true));
            fleeMenu.Add("teddy.bear.flee.useE", new CheckBox("Use E", true));


            misc = Menu.AddSubMenu("Misc", "teddy.bear.misc");
            misc.Add("teddy.bear.misc.skW", new CheckBox("safe kill with W", true));

            drawingMenu = Menu.AddSubMenu("Drawing", "teddy.bear.drawing");
            drawingMenu.Add("DrawQ", new CheckBox("Draw Q range", true));
            drawingMenu.Add("DrawW", new CheckBox("Draw W range", true));
            drawingMenu.Add("DrawE", new CheckBox("Draw E range", true));
            drawingMenu.Add("DrawR", new CheckBox("Draw R range", true));
            drawingMenu.Add("DrawHP", new CheckBox("Draw HP Indicator", true));


            #endregion

            Interrupter2.OnInterruptableTarget += Interrupter_OnPossibleToInterrupt;
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += OnEndScene;
            Chat.Print("<font color='#881df2'>TeddyBear - Loaded.");

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

        static void Interrupter_OnPossibleToInterrupt(Obj_AI_Base unit, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (args.DangerLevel >= Interrupter2.DangerLevel.High && unit.Distance(Player.Position) <= _q.Range)
            {
                _q.Cast(unit);
            }
        }

        private static void OnEndScene(EventArgs args)
        {
            if (getCheckBoxItem(drawingMenu, "DrawHP"))
            {
                foreach (var enemy in
                    ObjectManager.Get<AIHeroClient>().Where(ene => !ene.IsDead && ene.IsEnemy && ene.IsVisible))
                {
                    Hpi.unit = enemy;
                    Hpi.DrawDmg(CalcDamage(enemy), Color.Green);
                }
            }
        }

        private static int CalcDamage(Obj_AI_Base target)
        {
            var aa = Player.GetAutoAttackDamage(target, true) * (1 + Player.Crit);
            var damage = aa;

            if (_r.IsReady()) // rdamage
            {
                damage += _r.GetDamage(target);
            }

            if (_q.IsReady()) // qdamage
            {

                damage += _q.GetDamage(target);
            }

            if (_e.IsReady()) // edamage
            {

                damage += _e.GetDamage(target);
            }

            return (int)damage;
        }


        private static void Drawing_OnDraw(EventArgs args)
        {
            var menuItem1 = getCheckBoxItem(drawingMenu, "DrawQ");
            var menuItem2 = getCheckBoxItem(drawingMenu, "DrawE");
            var menuItem3 = getCheckBoxItem(drawingMenu, "DrawW");
            var menuItem4 = getCheckBoxItem(drawingMenu, "DrawR");

            if (menuItem1 && _q.IsReady()) Render.Circle.DrawCircle(Player.Position, _q.Range, Color.SpringGreen);
            if (menuItem2 && _e.IsReady()) Render.Circle.DrawCircle(Player.Position, _e.Range, Color.Crimson);
            if (menuItem3 && _w.IsReady()) Render.Circle.DrawCircle(Player.Position, _w.Range, Color.Aqua);
            if (menuItem4 && _r.IsReady()) Render.Circle.DrawCircle(Player.Position, _r.Range, Color.Firebrick);
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                Flee();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Laneclear();
            }

        }

        private static void Flee()
        {
           EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos, false);
            _q.Cast();
        }

        private static void Laneclear() // jungle clear ^^
        {
            bool vQ = _q.IsReady() && getCheckBoxItem(laneclearing, "teddy.bear.laneclearing.useQ");
            bool vW = _w.IsReady() && getCheckBoxItem(laneclearing, "teddy.bear.laneclearing.useW");
            bool vE = _e.IsReady() && getCheckBoxItem(laneclearing, "teddy.bear.laneclearing.useE");

            var minionBase = MinionManager.GetMinions(_e.Range);
            var jungleBase = MinionManager.GetMinions(_w.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            #region Q-Cast Jungle
            if (vQ)
            {
                foreach (var junglemob in jungleBase.Where(x => x.HealthPercent >= 25))
                {
                    _q.Cast(junglemob);
                }
            }
            #endregion

            #region W-Cast Jungle
            if (vW)
            {
                foreach (Obj_AI_Minion junglemob in ObjectManager.Get<Obj_AI_Minion>().Where(
                    x =>
                        x.Name.Contains("SRU_Blue1.1.1") || x.Name.Contains("SRU_Blue7.1.1") ||
                        x.Name.Contains("SRU_Red4.1.1") || x.Name.Contains("SRU_Red10.1.1") ||
                        x.Name.Contains("SRU_Dragon6.1.1") || x.Name.Contains("SRU_Baron12.1.1")
                        && x.Health < _w.GetDamage(x)))
                {
                    if (junglemob.Health < _w.GetDamage(junglemob))
                        _w.CastOnUnit(junglemob);
                }
            }
            #endregion

            #region E-Cast Jungle
            if (vE)
            {
                if (jungleBase.Count >= 2)
                {
                    _e.Cast();
                }

                if (minionBase.Count >= 3)
                {
                    _e.Cast();
                }

            }
            #endregion
        }

        private static void Combo()
        {
            bool vQ = _q.IsReady() && getCheckBoxItem(comboMenu, "teddy.bear.combo.useq");
            bool vW = _w.IsReady() && getCheckBoxItem(comboMenu, "teddy.bear.combo.usew");
            bool vE = _e.IsReady() && getCheckBoxItem(comboMenu, "teddy.bear.combo.usee");
            bool vR = _r.IsReady() && getCheckBoxItem(comboMenu, "teddy.bear.combo.user");;
            bool useskW = getCheckBoxItem(misc, "teddy.bear.misc.skW");

            AIHeroClient tsQ = TargetSelector.GetTarget(_q.Range, DamageType.Magical);
            AIHeroClient tsR = TargetSelector.GetTarget(_r.Range, DamageType.Magical);

            #region Q-Cast
            if (vQ)
            {
                if (tsQ.Distance(Player.Position) >= 2500 && tsQ.Direction == Player.Direction && tsQ.MoveSpeed > Player.MoveSpeed &&
                    tsQ.MoveSpeed < Player.MoveSpeed * 1.3)
                {
                    _q.Cast(tsQ);
                }
                if (tsQ.GetEnemiesInRange(100).Any(enemies => enemies.IsEnemy && !enemies.IsDead) && tsQ.IsValidTarget())
                {
                    _q.Cast(tsQ);
                }
                else if (Player.CountAlliesInRange(500) >= 1 && tsQ.IsValidTarget())
                {
                    _q.Cast(tsQ);
                }
                else if (tsQ.IsValidTarget())
                {
                    _q.Cast(tsQ);
                }
            }
            #endregion

            #region W-Cast
            if (vW && useskW)
            {
                if (tsQ.IsValidTarget(_w.Range) && tsQ.Health < _w.GetDamage(tsQ))
                {
                    _w.CastOnUnit(tsQ);
                }
            }
            else if (vW)
            {
                if (tsQ.IsValidTarget(_w.Range))
                {
                    _w.CastOnUnit(tsQ);
                }
            }
            #endregion

            #region E-Cast
            if (vE)
            {
                if (tsQ.IsValidTarget(_e.Range) && tsQ.Distance(Player.Position) <= _w.Range)
                {
                    _e.Cast();
                }
            }
            #endregion

            #region R-Cast
            if (vR)
            {
                if (tsR.IsValidTarget(Player.AttackRange) && tsR.HealthPercent > 25)
                {
                    _r.Cast();
                }
            }
            #endregion

        }
    }
}