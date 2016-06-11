using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using PortAIO.Champion.Teemo;
using Spell = LeagueSharp.Common.Spell;

namespace SharpShooter.Plugins
{
    public class Teemo
    {
        public static Menu comboMenu, harassMenu, laneClearMenu, jungleMenu, miscMenu, drawingMenu, config, gapCloser;
        private readonly Spell _q;
        private readonly Spell _r;
        private readonly Spell _w;
        private Spell _e;

        public Teemo()
        {
            _q = new Spell(SpellSlot.Q, 680f, DamageType.Magical);
            _w = new Spell(SpellSlot.W);
            _e = new Spell(SpellSlot.E);
            _r = new Spell(SpellSlot.R, 300f, DamageType.Magical) {MinHitChance = HitChance.High};

            _r.SetSkillshot(1.5f, 100f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            config = MainMenu.AddMenu("SS : Teemo", "SharpShooter");

            comboMenu = config.AddSubMenu("Combo");
            comboMenu.Add("qCombo", new CheckBox("Use Q"));
            comboMenu.Add("wCombo", new CheckBox("Use W"));
            comboMenu.Add("rCombo", new CheckBox("Use R"));

            harassMenu = config.AddSubMenu("Harass");
            harassMenu.Add("qHarass", new CheckBox("Use Q"));
            harassMenu.Add("harassMana", new Slider("Min Mana % : ", 60));

            laneClearMenu = config.AddSubMenu("Lane Clear");
            laneClearMenu.Add("qClear", new CheckBox("Use Q"));
            laneClearMenu.Add("clearMana", new Slider("Min Mana % : ", 60));

            jungleMenu = config.AddSubMenu("Jungle Clear");
            jungleMenu.Add("qJungle", new CheckBox("Use Q"));
            jungleMenu.Add("jungleMana", new Slider("Min Mana % : ", 20));

            gapCloser = config.AddSubMenu("Gap Closer");
            gapCloser.Add("antiGap", new CheckBox("Use Anti Gap"));

            drawingMenu = config.AddSubMenu("Drawings");
            drawingMenu.Add("drawQ", new CheckBox("Draw Q")); //Color.DeepSkyBlue
            drawingMenu.Add("drawR", new CheckBox("Draw R")); // Color.DeepSkyBlue

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Orbwalker.OnPostAttack += Orbwalking_AfterAttack;
            Console.WriteLine("Sharpshooter: Teemo Loaded.");
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

        private void Orbwalking_AfterAttack(AttackableUnit target, EventArgs args)
        {
            if (target.Type == GameObjectType.AIHeroClient)
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    if (getCheckBoxItem(comboMenu, "qCombo"))
                        if (target.IsValidTarget(_q.Range))
                            if (_q.IsReady())
                                _q.CastOnUnit(target as Obj_AI_Base);
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None))
                {
                    if (getCheckBoxItem(harassMenu, "qHarass"))
                        if (target.IsValidTarget(_q.Range))
                            if (_q.IsReady())
                                _q.CastOnUnit(target as Obj_AI_Base);
                }
            }
        }

        private void Game_OnUpdate(EventArgs args)
        {
            _r.Range = _r.Level*300;

            if (!ObjectManager.Player.IsDead)
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    if (getCheckBoxItem(comboMenu, "qCombo"))
                        if (_q.IsReady())
                            if (!HeroManager.Enemies.Any(Orbwalking.InAutoAttackRange))
                            {
                                var target = TargetSelector.GetTarget(_q.Range, _q.DamageType);
                                if (target != null)
                                    _q.CastOnUnit(target);
                            }

                    if (getCheckBoxItem(comboMenu, "wCombo"))
                        if (_w.IsReady())
                            if (ObjectManager.Player.CountEnemiesInRange(1000f) >= 1)
                                _w.Cast();

                    if (getCheckBoxItem(comboMenu, "rCombo"))
                        if (_r.IsReady())
                        {
                            var target =
                                HeroManager.Enemies.FirstOrDefault(
                                    x =>
                                        x.IsValidTarget(_r.Range) && !x.IsFacing(ObjectManager.Player) &&
                                        !x.HasBuff("bantamtraptarget") &&
                                        _r.GetPrediction(x).Hitchance >= _r.MinHitChance);
                            if (target != null && target.LSDistance(ObjectManager.Player) <= _r.Range - 50)
                                _r.Cast(target, false, true);
                        }
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                {
                    if (getCheckBoxItem(harassMenu, "qHarass"))
                        if (_q.IsReady())
                            if (!HeroManager.Enemies.Any(Orbwalking.InAutoAttackRange))
                            {
                                var target = TargetSelector.GetTarget(_q.Range, _q.DamageType);
                                if (target != null)
                                    _q.CastOnUnit(target);
                            }
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                {
                    //Laneclear
                    if (getCheckBoxItem(laneClearMenu, "qClear"))
                        if (ObjectManager.Player.ManaPercent >= getSliderItem(laneClearMenu, "clearMana"))
                            if (_q.IsReady())
                            {
                                var target =
                                    MinionManager.GetMinions(_q.Range)
                                        .FirstOrDefault(
                                            x =>
                                                x.IsKillableAndValidTarget(_q.GetDamage(x), DamageType.Magical, _q.Range) &&
                                                (x.CharData.BaseSkinName.Contains("siege") ||
                                                 x.CharData.BaseSkinName.Contains("super")));
                                if (target != null)
                                    _q.CastOnUnit(target);
                            }

                    //Jungleclear
                    if (getCheckBoxItem(jungleMenu, "qJungle"))
                        if (ObjectManager.Player.ManaPercent >= getSliderItem(jungleMenu, "jungleMana"))
                            if (_q.IsReady())
                            {
                                var target =
                                    MinionManager.GetMinions(600, MinionTypes.All, MinionTeam.Neutral,
                                        MinionOrderTypes.MaxHealth).FirstOrDefault(x => x.IsValidTarget(600));
                                if (target != null)
                                    _q.CastOnUnit(target);
                            }
                }
            }
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (getCheckBoxItem(gapCloser, "antiGap"))
            {
                if (gapcloser.Sender.IsValidTarget(_q.Range))
                    if (_q.IsReady())
                        _q.CastOnUnit(gapcloser.Sender);

                if (_w.IsReady())
                    _w.Cast();

                if (gapcloser.End.LSDistance(ObjectManager.Player.Position) <= 300)
                    if (_r.IsReady())
                        _r.Cast(ObjectManager.Player.Position);
            }
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (!ObjectManager.Player.IsDead)
            {
                if (getCheckBoxItem(drawingMenu, "drawQ") && _q.IsReady())
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, _q.Range, Color.DeepSkyBlue);

                if (getCheckBoxItem(drawingMenu, "drawR") && _r.IsReady())
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, _r.Range, Color.DeepSkyBlue);
            }
        }
    }
}