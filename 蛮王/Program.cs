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

namespace Bloodimir_Tryndamere
{
    internal static class Program
    {
        private static Spell.Active Q;
        public static Spell.Active W;
        public static Spell.Skillshot E;
        private static Spell.Active R;
        private static Menu _trynMenu;
        public static Menu ComboMenu;
        private static Menu _drawMenu;
        private static Menu _skinMenu;
        public static Menu MiscMenu, LaneJungleClear;
        public static Item Tiamat, Hydra, Bilgewater, Youmuu, Botrk;
        public static AIHeroClient Tryndamere = ObjectManager.Player;

        private static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoaded;
        }

        private static bool HasSpell(string s)
        {
            return Player.Spells.FirstOrDefault(o => o.SData.Name.Contains(s)) != null;
        }

        private static void OnLoaded(EventArgs args)
        {
            if (Player.Instance.ChampionName != "Tryndamere")
                return;
            Bootstrap.Init(null);
            Q = new Spell.Active(SpellSlot.Q);
            W = new Spell.Active(SpellSlot.W, 400);
            E = new Spell.Skillshot(SpellSlot.E, 660, SkillShotType.Linear, 250, 700, (int) 92.5);
            R = new Spell.Active(SpellSlot.R);
            Botrk = new Item(3153, 550f);
            Bilgewater = new Item(3144, 475f);
            Hydra = new Item(3074, 250f);
            Tiamat = new Item(3077, 250f);
            Youmuu = new Item(3142, 10);

            _trynMenu = MainMenu.AddMenu("Blood蛮王", "bloodimirtry");
            _trynMenu.AddGroupLabel("CH汉化-Bloodimir 蛮王");
            _trynMenu.AddSeparator();
            _trynMenu.AddLabel("Bloodimir Tryndamere V1.0.0.0");

            ComboMenu = _trynMenu.AddSubMenu("连招", "sbtw");
            ComboMenu.AddGroupLabel("连招设置");
            ComboMenu.AddSeparator();
            ComboMenu.Add("usecomboq", new CheckBox("使用 Q"));
            ComboMenu.Add("usecombow", new CheckBox("使用 W"));
            ComboMenu.Add("usecomboe", new CheckBox("使用 E"));
            ComboMenu.Add("usecombor", new CheckBox("使用 R"));
            ComboMenu.AddSeparator();
            ComboMenu.Add("rslider", new Slider("最低血量% 使用R", 20, 0, 95));
            ComboMenu.AddSeparator();
            ComboMenu.Add("qhp", new Slider("Q % 血量", 25, 0, 95));


            _drawMenu = _trynMenu.AddSubMenu("线圈", "drawings");
            _drawMenu.AddGroupLabel("线圈");
            _drawMenu.AddSeparator();
            _drawMenu.Add("drawe", new CheckBox("显示 E"));

            LaneJungleClear = _trynMenu.AddSubMenu("清线/清野", "lanejungleclear");
            LaneJungleClear.AddGroupLabel("清线/清野设置");
            LaneJungleClear.Add("LCE", new CheckBox("声音 E"));

            MiscMenu = _trynMenu.AddSubMenu("杂项", "miscmenu");
            MiscMenu.AddGroupLabel("杂项设置");
            MiscMenu.AddSeparator();
            MiscMenu.Add("kse", new CheckBox("E 抢头"));
            MiscMenu.Add("ksbotrk", new CheckBox("破败 抢头"));
            MiscMenu.Add("kshydra", new CheckBox("九头蛇 抢头"));
            MiscMenu.Add("usehydra", new CheckBox("使用 九头蛇"));
            MiscMenu.Add("usetiamat", new CheckBox("使用 提亚马特"));
            MiscMenu.Add("usebotrk", new CheckBox("使用 破败"));
            MiscMenu.Add("usebilge", new CheckBox("使用 弯刀"));
            MiscMenu.Add("useyoumuu", new CheckBox("使用 幽梦"));


            _skinMenu = _trynMenu.AddSubMenu("换肤", "skin");
            _skinMenu.AddGroupLabel("选择皮肤ID");

            var skinchange = _skinMenu.Add("skinid", new Slider("皮肤", 4, 0, 7));
            var skinid = new[]
            {"Default", "Highland", "King", "Viking", "Demon Blade", "Sultan", "Warring Kingdoms", "Nightmare"};
            skinchange.DisplayName = skinid[skinchange.CurrentValue];
            skinchange.OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
            {
                sender.DisplayName = skinid[changeArgs.NewValue];
                if (MiscMenu["debug"].Cast<CheckBox>().CurrentValue)
                {
                    Chat.Print("skin-changed");
                }
            };
            Game.OnUpdate += Tick;
            Drawing.OnDraw += OnDraw;
        }

        private static void AutoQ(bool useR)
        {
            var autoQ = ComboMenu["usecomboq"].Cast<CheckBox>().CurrentValue;
            var healthAutoR = ComboMenu["qhp"].Cast<Slider>().CurrentValue;
            if (autoQ && _Player.HealthPercent < healthAutoR)
            {
                Q.Cast();
            }
        }

        private static void AutoUlt(bool useR)
        {
            var autoR = ComboMenu["usecombor"].Cast<CheckBox>().CurrentValue;
            var healthAutoR = ComboMenu["rslider"].Cast<Slider>().CurrentValue;
            if (!autoR || !(_Player.HealthPercent < healthAutoR)) return;
            if (
                ObjectManager
                    .Get<AIHeroClient>().Any(x => x.IsEnemy && x.Distance(Tryndamere.Position) <= 1100))
            {
                R.Cast();
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (Tryndamere.IsDead) return;
            if (_drawMenu["drawe"].Cast<CheckBox>().CurrentValue && E.IsLearned)
            {
                Circle.Draw(Color.Green, Q.Range, Player.Instance.Position);
            }
        }

        private static void Flee()
        {
            Orbwalker.MoveTo(Game.CursorPos);
            E.Cast(Game.CursorPos);
        }

        private static void Tick(EventArgs args)
        {
            Killsteal();
            SkinChange();
            AutoQ(ComboMenu["usecomboq"].Cast<CheckBox>().CurrentValue);
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                Flee();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo.TrynCombo();
                Combo.Items();
            }
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                {
                    LaneJungleClearA.LaneClearA();
                }
                AutoUlt(ComboMenu["usecombor"].Cast<CheckBox>().CurrentValue);
            } }

        private static void Killsteal()
        {
            if (!MiscMenu["kse"].Cast<CheckBox>().CurrentValue || !E.IsReady()) return;
            try
            {
                foreach (var etarget in EntityManager.Heroes.Enemies.Where(
                    hero => hero.IsValidTarget(E.Range) && !hero.IsDead && !hero.IsZombie)
                    .Where(etarget => Tryndamere.GetSpellDamage(etarget, SpellSlot.E) >= etarget.Health))
                {
                    {
                        E.Cast(etarget.ServerPosition);
                    }
                    if ((!MiscMenu["ksbotrk"].Cast<CheckBox>().CurrentValue || !Botrk.IsReady()) &&
                        !Bilgewater.IsReady() && !Tiamat.IsReady()) continue;
                    {
                        try
                        {
                            foreach (var itarget in EntityManager.Heroes.Enemies.Where(
                                hero =>
                                    hero.IsValidTarget(Botrk.Range) && !hero.IsDead &&
                                    !hero.IsZombie)
                                .Where(itarget => Tryndamere.GetItemDamage(itarget, ItemId.Blade_of_the_Ruined_King) >=
                                                  itarget.Health))
                            {
                                {
                                    Botrk.Cast(itarget);
                                }
                                if ((!MiscMenu["kshydra"].Cast<CheckBox>().CurrentValue ||
                                     !Botrk.IsReady()) && !Bilgewater.IsReady() && !Tiamat.IsReady())
                                    continue;
                                {
                                    try
                                    {
                                        foreach (var htarget in EntityManager.Heroes.Enemies.Where(
                                            hero =>
                                                hero.IsValidTarget(Hydra.Range) &&
                                                !hero.IsDead && !hero.IsZombie)
                                            .Where(htarget => Tryndamere.GetItemDamage(htarget,
                                                ItemId.Ravenous_Hydra_Melee_Only) >=
                                                              htarget.Health))
                                        {
                                            Hydra.Cast();
                                        }
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private static
            void SkinChange
            ()
        {
            var style = _skinMenu["skinid"].DisplayName;
            switch (style)
            {
                case "Default":
                    Player.SetSkinId(0);
                    break;
                case "Highland":
                    Player.SetSkinId(1);
                    break;
                case "King":
                    Player.SetSkinId(2);
                    break;
                case "Viking":
                    Player.SetSkinId(3);
                    break;
                case "Demon Blade":
                    Player.SetSkinId(4);
                    break;
                case "Sultan":
                    Player.SetSkinId(5);
                    break;
                case "Warring Kingdoms":
                    Player.SetSkinId(5);
                    break;
                case "Nightmare":
                    Player.SetSkinId(5);
                    break;
            }
        }
    }
}
