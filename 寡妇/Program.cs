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

namespace Evelynn
{
    internal static class Program
    {
        public static Spell.Active Q, W;
        public static Spell.Targeted E;
        private static Spell.Skillshot _r;
        private static Menu _eveMenu;
        public static Menu ComboMenu;
        private static Menu _drawMenu;
        private static Menu _skinMenu;
        private static Menu _miscMenu;
        public static Menu LaneJungleClear, LastHitMenu;
        private static readonly AIHeroClient Eve = ObjectManager.Player;

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoaded;
        }

        private static void OnLoaded(EventArgs args)
        {
            if (Player.Instance.ChampionName != "Evelynn")
                return;
            Bootstrap.Init(null);
            Q = new Spell.Active(SpellSlot.Q, 475);
            W = new Spell.Active(SpellSlot.W);
            E = new Spell.Targeted(SpellSlot.E, 225);
            _r = new Spell.Skillshot(SpellSlot.R, 900, SkillShotType.Circular, 250, 1200, 150);

            _eveMenu = MainMenu.AddMenu("CH汉化-寡妇", "bloodimireve");
            _eveMenu.AddGroupLabel("Bloodimir.Evelynn");
            _eveMenu.AddSeparator();
            _eveMenu.AddLabel("CH汉化-寡妇 V1.0.1.0");

            ComboMenu = _eveMenu.AddSubMenu("连招", "sbtw");
            ComboMenu.AddGroupLabel("连招设置");
            ComboMenu.AddSeparator();
            ComboMenu.Add("usecomboq", new CheckBox("使用 Q"));
            ComboMenu.Add("usecombow", new CheckBox("使用 W"));
            ComboMenu.Add("usecomboe", new CheckBox("使用 E"));
            ComboMenu.Add("usecombor", new CheckBox("使用 R"));
            ComboMenu.Add("useignite", new CheckBox("使用 点燃"));
            ComboMenu.AddSeparator();
            ComboMenu.Add("rslider", new Slider("最低敌人数量使用 R", 1, 0, 5));

            _drawMenu = _eveMenu.AddSubMenu("线圈", "drawings");
            _drawMenu.AddGroupLabel("线圈设置");
            _drawMenu.AddSeparator();
            _drawMenu.Add("drawq", new CheckBox("显示 Q"));
            _drawMenu.Add("drawr", new CheckBox("显示 R"));
            _drawMenu.Add("drawe", new CheckBox("显示 R"));

            LaneJungleClear = _eveMenu.AddSubMenu("清野/清线", "lanejungleclear");
            LaneJungleClear.AddGroupLabel("清野/清线设置");
            LaneJungleClear.Add("LCE", new CheckBox("使用 E"));
            LaneJungleClear.Add("LCQ", new CheckBox("使用 Q"));

            LastHitMenu = _eveMenu.AddSubMenu("尾兵", "lasthit");
            LastHitMenu.AddGroupLabel("尾兵设置");
            LastHitMenu.Add("LHQ", new CheckBox("使用 Q"));

            _miscMenu = _eveMenu.AddSubMenu("杂项", "miscmenu");
            _miscMenu.AddGroupLabel("抢头");
            _miscMenu.AddSeparator();
            _miscMenu.Add("kse", new CheckBox("抢头 E"));
            _miscMenu.AddSeparator();
            _miscMenu.Add("ksq", new CheckBox("抢头 Q"));
            _miscMenu.Add("asw", new CheckBox("自动/智能 W"));

            _skinMenu = _eveMenu.AddSubMenu("换肤", "skin");
            _skinMenu.AddGroupLabel("选择想要使用的皮肤");

            var skinchange = _skinMenu.Add("sID", new Slider("皮肤", 2, 0, 4));
            var sid = new[] {"Default", "Shadow", "Masquerade", "Tango", "Safecracker"};
            skinchange.DisplayName = sid[skinchange.CurrentValue];
            skinchange.OnValueChange +=
                delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
                {
                    sender.DisplayName = sid[changeArgs.NewValue];
                };

            Game.OnUpdate += Tick;
            Drawing.OnDraw += OnDraw;
        }

        private static void OnDraw(EventArgs args)
        {
            if (Eve.IsDead) return;
            if (_drawMenu["drawq"].Cast<CheckBox>().CurrentValue && Q.IsLearned)
            {
                Circle.Draw(Color.DarkBlue, Q.Range, Player.Instance.Position);
            }
            if (_drawMenu["drawr"].Cast<CheckBox>().CurrentValue && _r.IsLearned)
            {
                Circle.Draw(Color.Red, _r.Range, Player.Instance.Position);
            }
            if (_drawMenu["drawe"].Cast<CheckBox>().CurrentValue && E.IsLearned)
            {
                Circle.Draw(Color.Green, E.Range, Player.Instance.Position);
            }
        }

        private static void Flee()
        {
            Orbwalker.MoveTo(Game.CursorPos);
            W.Cast();
        }

        private static void AutoW()
        {
            var useW = _miscMenu["asw"].Cast<CheckBox>().CurrentValue;

            if (Player.HasBuffOfType(BuffType.Slow) || Eve.CountEnemiesInRange(550) >= 3 && useW)
            {
                W.Cast();
            }
        }

        private static void Tick(EventArgs args)
        {
            Killsteal();
            SkinChange();
            AutoW();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                Flee();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo.EveCombo();
                Rincombo(ComboMenu["usecombor"].Cast<CheckBox>().CurrentValue);
            }
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                {
                    LaneJungleClearA.LaneClearB();
                }
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                {
                    LaneJungleClearA.JungleClearB();
                }
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
                {
                    LastHitA.LastHitB();
                }
            }
        }

        private static void Rincombo(bool useR)
        {
            var rtarget = TargetSelector.GetTarget(_r.Range, DamageType.Magical);
            if (!ComboMenu["usecombor"].Cast<CheckBox>().CurrentValue) return;
            if (!useR || !_r.IsReady() ||
                rtarget.CountEnemiesInRange(_r.Width) < ComboMenu["rslider"].Cast<Slider>().CurrentValue) return;
            _r.Cast(rtarget.ServerPosition);
        }

        private static void Killsteal()
        {
            if (!_miscMenu["ksq"].Cast<CheckBox>().CurrentValue || !Q.IsReady()) return;
            try
            {
                foreach (
                    var qtarget in
                        EntityManager.Heroes.Enemies.Where(
                            hero =>
                                hero.IsValidTarget(Q.Range) && !hero.IsDead && !hero.IsZombie))
                {
                    if (Eve.GetSpellDamage(qtarget, SpellSlot.Q) >= qtarget.Health)
                    {
                        Q.Cast();
                    }
                    if (!_miscMenu["kse"].Cast<CheckBox>().CurrentValue || !E.IsReady()) continue;
                    try
                    {
                        foreach (var etarget in EntityManager.Heroes.Enemies.Where(
                            hero =>
                                hero.IsValidTarget(E.Range) && !hero.IsDead && !hero.IsZombie)
                            .Where(etarget => Eve.GetSpellDamage(etarget, SpellSlot.E) >= etarget.Health))
                        {
                            E.Cast(etarget);
                        }
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }
        }

        private static void SkinChange()
        {
            var style = _skinMenu["sID"].DisplayName;
            switch (style)
            {
                case "Default":
                    Player.SetSkinId(0);
                    break;
                case "Shadow":
                    Player.SetSkinId(1);
                    break;
                case "Masquerade":
                    Player.SetSkinId(2);
                    break;
                case "Tango":
                    Player.SetSkinId(3);
                    break;
                case "Safecracker":
                    Player.SetSkinId(4);
                    break;
            }
        }
    }
}