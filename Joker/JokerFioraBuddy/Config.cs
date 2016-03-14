using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;

namespace JokerFioraBuddy
{
    public static class Config
    {
        private const string MenuName = "Joker Fiora 2.0.0.2";

        private static readonly Menu Menu;

        static Config() 
        {
            Menu = MainMenu.AddMenu(MenuName, MenuName.ToLower());
            Menu.AddGroupLabel("Welcome to Joker Fiora Addon!");
            Menu.AddLabel("以下是广告无视即可~");
            Menu.AddLabel("Features:");
            Menu.AddLabel("- Epic Combo! 100-0 in 2 seconds.");
            Menu.AddLabel("- Auto Shield Block (W).");
            Menu.AddLabel("- Auto Dispell Channelling Spells (W).");
            Menu.AddLabel("- Harass Mode with all spells.");
            Menu.AddLabel("- Last Hit Mode with Q.");
            Menu.AddLabel("- Lane Clear Mode with Q/E.");
            Menu.AddLabel("- Flee Mode with Q.");
            Menu.AddLabel("- Smart Target Selector.");
            Menu.AddLabel("- Auto-Ignite!");
            Menu.AddLabel("- Champion 1 shot combo indicator!");
            Menu.AddLabel("All customizable! Featuring Youmuu's Ghostblade / Ravenous Hydra / Blade of the Ruined King");
            Menu.AddLabel("Credits to: Danny - Main Coder / Trees - Shield Block / Fluxy - Target Selector 2");

            Modes.Initialize();
            ShieldBlock.Initialize();
            Dispell.Initialize();
            Drawings.Initialize();
            Misc.Initialize();
        }

        public static void Initialize() 
        {
 
        }

        public static class Drawings
        {
            public static readonly Menu Menu;
            public static bool ShowKillable
            {
                get { return Menu["drawingKillable"].Cast<CheckBox>().CurrentValue; }
            }

            public static bool ShowChampionTarget
            {
                get { return Menu["drawingChampionTarget"].Cast<CheckBox>().CurrentValue; }
            }

            public static bool ShowNotification
            {
                get { return Menu["drawingNotification"].Cast<CheckBox>().CurrentValue; }
            }

            static Drawings()
            {
                Menu = Config.Menu.AddSubMenu("线圈");
                Menu.AddGroupLabel("线圈");
                Menu.Add("drawingKillable", new CheckBox("文本显示如果敌人可击杀"));
                Menu.Add("drawingChampionTarget", new CheckBox("在选择的目标下显示圆圈"));
                Menu.Add("drawingNotification", new CheckBox("游戏开始时显示提示"));      
            }

            public static void Initialize()
            {

            }
        }

        public static class ShieldBlock
        {
            public static readonly Menu Menu;

            public static bool BlockSpells
            {
                get { return Menu["blockSpellsW"].Cast<CheckBox>().CurrentValue; }
            }

            public static bool EvadeIntegration
            {
                get { return Menu["evade"].Cast<CheckBox>().CurrentValue; }
            }

            static ShieldBlock()
            {
                Menu = Config.Menu.AddSubMenu("技能阻挡");
                Menu.AddGroupLabel("核心选项");
                Menu.Add("blockSpellsW", new CheckBox("自动阻挡技能 (W)"));
                Menu.Add("evade", new CheckBox("躲避整合"));
                Menu.AddSeparator();

                Menu.AddGroupLabel("敌方可阻挡技能");
            }

            public static void Initialize()
            {

            }
        }

        public static class Dispell
        {
            public static readonly Menu Menu;

            public static bool DispellSpells
            {
                get { return Menu["dispellSpellsW"].Cast<CheckBox>().CurrentValue; }
            }

            static Dispell()
            {
                Menu = Config.Menu.AddSubMenu("驱逐器");
                Menu.AddGroupLabel("核心技能");
                Menu.Add("dispellSpellsW", new CheckBox("自动驱逐吟唱技能 (W)"));
               Menu.AddSeparator();

                Menu.AddGroupLabel("敌方可驱逐技能");
            }

            public static void Initialize()
            {

            }
        }

        public static class Misc
        {
            private static readonly Menu Menu;

            public static int SkinID
            {
                get { return Menu["skinid"].Cast<Slider>().CurrentValue; }
            }

            public static bool enableSkinHack
            {
                get { return Menu["skinhack"].Cast<CheckBox>().CurrentValue; }
            }

            public static bool enableLevelUP
            {
                get { return Menu["evolveskills"].Cast<CheckBox>().CurrentValue; }
            }
            public static bool drawQ
            {
                get { return Menu["drawq"].Cast<CheckBox>().CurrentValue; }
            }
            public static bool drawW
            {
                get { return Menu["draww"].Cast<CheckBox>().CurrentValue; }
            }
            public static bool drawE
            {
                get { return Menu["drawe"].Cast<CheckBox>().CurrentValue; }
            }
            public static bool drawR
            {
                get { return Menu["drawr"].Cast<CheckBox>().CurrentValue; }
            }
            public static Color currentColor
            {
                get { return colorlist[Menu["mastercolor"].Cast<Slider>().CurrentValue]; }
            }


            public static Slider SkinSlider = new Slider("SkinID : ({0})", 0, 0, 4);
            public static CheckBox SkinEnable = new CheckBox("开启");
            public static CheckBox EvolveEnable = new CheckBox("开启");
            public static CheckBox qdraw = new CheckBox("显示 Q",false);
            public static CheckBox wdraw = new CheckBox("显示 W", false);
            public static CheckBox edraw = new CheckBox("显示 E", false);
            public static CheckBox rdraw = new CheckBox("显示 R", false);
            static Color[] colorlist = {Color.Green,Color.Aqua,Color.Black,Color.Blue,Color.Firebrick,Color.Gold,Color.Pink,Color.Violet,Color.White,Color.Lime,Color.LimeGreen,Color.Yellow,Color.Magenta};
            static Slider masterColorSlider = new Slider("Color Slider",0,0,colorlist.Length-1);

            static Misc()
            {
                Menu = Config.Menu.AddSubMenu("杂项");
                Menu.AddGroupLabel("换肤");
                Menu.Add("skinhack", SkinEnable);
                Menu.Add("skinid", SkinSlider);
                Menu.AddGroupLabel("自动加点");
                Menu.Add("evolveskills", EvolveEnable);
                Menu.AddGroupLabel("线圈");
                Menu.Add("mastercolor", masterColorSlider);
                Menu.Add("drawq", qdraw);
                Menu.Add("draww", wdraw);
                Menu.Add("drawe", edraw);
                Menu.Add("drawr", rdraw);
                SkinSlider.OnValueChange += SkinSlider_OnValueChange;
                SkinEnable.OnValueChange += SkinEnable_OnValueChange;
            }

            private static void SkinEnable_OnValueChange(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
            {
                if (!Misc.enableSkinHack)
                {
                    Player.SetSkinId(0);
                    return;
                }

                Player.SetSkinId(Misc.SkinID);
            }

            private static void SkinSlider_OnValueChange(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
            {
                if (!Misc.enableSkinHack)
                {
                    Player.SetSkinId(0);
                    return;
                }

                Player.SetSkinId(Misc.SkinID);
            }

            public static void Initialize()
            {

            }

        }

        public static class Modes
        {
            private static readonly Menu Menu;

            static Modes()
            {
                Menu = Config.Menu.AddSubMenu("模式");

                Combo.Initialize();
                Menu.AddSeparator();

                Harass.Initialize();
                Menu.AddSeparator();

                LaneClear.Initialize();
                Menu.AddSeparator();

                LastHit.Initialize();
                Menu.AddSeparator();

                Flee.Initialize();
                Menu.AddSeparator();

                Perma.Initialize();
                
            }

            public static void Initialize()
            {

            }

            public static class Combo
            {
                static List<EloBuddy.AIHeroClient> enemies = EntityManager.Heroes.Enemies;
                public static bool UseQ
                {
                    get { return Menu["comboUseQ"].Cast<CheckBox>().CurrentValue; }
                }

                public static bool UseE
                {
                    get { return Menu["comboUseE"].Cast<CheckBox>().CurrentValue; }
                }

                public static bool UseR
                {
                    get { return Menu["comboUseR"].Cast<CheckBox>().CurrentValue; }
                }

                public static bool UseTiamatHydra
                {
                    get { return Menu["comboUseTiamatHydra"].Cast<CheckBox>().CurrentValue; }
                }

                public static bool UseCutlassBOTRK
                {
                    get { return Menu["comboUseCutlassBOTRK"].Cast<CheckBox>().CurrentValue; }
                }

                public static bool UseYomuus
                {
                    get { return Menu["comboUseYomuus"].Cast<CheckBox>().CurrentValue; }
                }

                public static bool useRonTarget(string x)
                {
                     return Menu[x].Cast<CheckBox>().CurrentValue; 
                }

                public static int rSliderValue()
                {
                    return Menu["useRSlider"].Cast<Slider>().CurrentValue;
                }
                static Combo()
                {
                    Menu.AddGroupLabel("连招");
                    Menu.Add("comboUseQ", new CheckBox("使用 Q"));
                    Menu.Add("comboUseE", new CheckBox("使用 E"));
                    Menu.Add("comboUseR", new CheckBox("使用 R"));
                    Menu.Add("useRSlider", new Slider("使用R当生命值低于 ({0}%)", 70));
                    Menu.Add("comboUseTiamatHydra", new CheckBox("使用 提亚马特/九头蛇"));
                    Menu.Add("comboUseCutlassBOTRK", new CheckBox("使用 弯刀/破败"));
                    Menu.Add("comboUseYomuus", new CheckBox("使用幽梦"));
                    Menu.AddSeparator();
                }

                public static void Initialize()
                {
                    Menu.AddLabel("对以下目标使用R");
                    foreach (var unit in enemies)
                    {
                        Menu.Add(unit.ChampionName, new CheckBox(unit.ChampionName));
                    }
                }
            }

            public static class Harass
            {
                public static bool UseQ
                {
                    get { return Menu["harassUseQ"].Cast<CheckBox>().CurrentValue; }
                }

                public static bool UseE
                {
                    get { return Menu["harassUseE"].Cast<CheckBox>().CurrentValue; }
                }

                public static bool UseR
                {
                    get { return Menu["harassUseR"].Cast<CheckBox>().CurrentValue; }
                }

                public static bool UseTiamatHydra
                {
                    get { return Menu["harassUseTiamatHydra"].Cast<CheckBox>().CurrentValue; }
                }

                public static int Mana
                {
                    get { return Menu["harassMana"].Cast<Slider>().CurrentValue; }
                }

                static Harass()
                {
                    Menu.AddGroupLabel("骚扰");
                    Menu.Add("harassUseQ", new CheckBox("使用 Q"));
                    Menu.Add("harassUseE", new CheckBox("使用 E"));
                    Menu.Add("harassUseR", new CheckBox("使用 R", false));
                    Menu.Add("harassUseTiamatHydra", new CheckBox("使用 提亚马特/九头蛇"));
                    Menu.Add("harassMana", new Slider("最大蓝量百分比使用 ({0}%)", 40));
                }

                public static void Initialize()
                {

                }
            }

            public static class LaneClear
            {
                public static bool UseQ
                {
                    get { return Menu["lcUseQ"].Cast<CheckBox>().CurrentValue; }
                }

                public static bool UseE
                {
                    get { return Menu["lcUseE"].Cast<CheckBox>().CurrentValue; }
                }

                public static bool UseTiamatHydra
                {
                    get { return Menu["lcUseTiamatHydra"].Cast<CheckBox>().CurrentValue; }
                }

                public static int Mana
                {
                    get { return Menu["lcMana"].Cast<Slider>().CurrentValue; }
                }

                static LaneClear()
                {
                    Menu.AddGroupLabel("清线");
                    Menu.Add("lcUseQ", new CheckBox("使用 Q"));
                    Menu.Add("lcUseE", new CheckBox("使用 E"));
                    Menu.Add("lcUseTiamatHydra", new CheckBox("使用 提亚马特/九头蛇"));
                    Menu.Add("lcMana", new Slider("最大蓝量百分比使用 ({0}%)", 40));
                }

                public static void Initialize()
                {

                }
            }

            public static class LastHit
            {
                public static bool UseQ
                {
                    get { return Menu["lhUseQ"].Cast<CheckBox>().CurrentValue; }
                }

                public static int Mana
                {
                    get { return Menu["lhMana"].Cast<Slider>().CurrentValue; }
                }

                static LastHit()
                {
                    Menu.AddGroupLabel("尾兵");
                    Menu.Add("lhUseQ", new CheckBox("使用 Q"));
                    Menu.Add("lhMana", new Slider("最大蓝量百分比使用 ({0}%)", 40));
                }

                public static void Initialize()
                {

                }
            }

            public static class Flee
            {
                public static bool UseQ
                {
                    get { return Menu["fleeUseQ"].Cast<CheckBox>().CurrentValue; }
                }

                static Flee()
                {
                    Menu.AddGroupLabel("逃跑");
                    Menu.Add("fleeUseQ", new CheckBox("使用 Q"));
                }

                public static void Initialize()
                {

                }
            }

            public static class Perma
            {
                static Slider igniteModeSlider = new Slider("点燃模式 : 智能", 1, 0, 1);

               

                public static bool UseIgnite
                {
                    get { return Menu["permaUseIG"].Cast<CheckBox>().CurrentValue; }
                }

                public static bool UseW
                {
                    get { return Menu["useWifKillable"].Cast<CheckBox>().CurrentValue; }
                }

                public static int igniteMode
                {
                    get { return Menu["igniteMode"].Cast<Slider>().CurrentValue; }
                }

                
                static Perma()
                {
                    Menu.AddGroupLabel("Perma Active");
                    Menu.Add("permaUseIG", new CheckBox("自动点燃敌方"));
                    Menu.Add("igniteMode", igniteModeSlider);
                    Menu.Add("useWifKillable", new CheckBox("自动W如果可击杀敌人"));

                }

                public static void Initialize()
                {
                    igniteModeSlider.OnValueChange += IgniteModeSlider_OnValueChange;
                }

                private static void IgniteModeSlider_OnValueChange(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                {
                    if (igniteModeSlider.CurrentValue == 0)
                        igniteModeSlider.DisplayName = "点燃模式 : 正常";
                    else
                        igniteModeSlider.DisplayName = "点燃模式 : 智能";
                }
            }
        }
    }
}
