using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace MagicianRyze
{
    internal class MenuManager
    {
        // Create Main Segments
        public static Menu MagicianRyzeMenu, ComboMenu, HarassMenu, JungleMenu, LaneClearMenu, LastHitMenu, KillStealMenu, DrawingMenu, SettingMenu;

        public static void Initialize()
        {
            // Addon Menu
            MagicianRyzeMenu = MainMenu.AddMenu("CH汉化魔术师-瑞兹", "魔术师-瑞兹");
            MagicianRyzeMenu.AddGroupLabel("CH汉化魔术师瑞兹");

            // Combo Menu
            ComboMenu = MagicianRyzeMenu.AddSubMenu("连招设置", "ComboFeatures");
            ComboMenu.AddGroupLabel("连招设置");
            ComboMenu.Add("Ucombo", new Slider("作者连招 - 作者喜欢的连招", 1, 1, 2));
            ComboMenu.AddSeparator(1);
            ComboMenu.AddLabel("技能使用:");
            ComboMenu.Add("Qcombo", new CheckBox("使用 Q"));
            ComboMenu.Add("Wcombo", new CheckBox("使用 W"));
            ComboMenu.Add("Ecombo", new CheckBox("使用 E"));
            ComboMenu.Add("Rcombo", new CheckBox("使用 R"));
            ComboMenu.Add("Dcombo", new CheckBox("当敌人被禁锢才使用R"));
            ComboMenu.AddSeparator(1);
            ComboMenu.Add("Scombo", new Slider("被动叠加层数使用大招", 4, 1, 4));

            // Harass Menu
            HarassMenu = MagicianRyzeMenu.AddSubMenu("骚扰设置", "HarassFeatures");
            HarassMenu.AddGroupLabel("骚扰设置");
            HarassMenu.AddLabel("技能使用:");
            HarassMenu.Add("Qharass", new CheckBox("使用 Q"));
            HarassMenu.AddSeparator(1);
            HarassMenu.Add("Mharass", new Slider("蓝量限制百分比 %", 25));

            // Jungle Menu
            JungleMenu = MagicianRyzeMenu.AddSubMenu("清野", "JungleFeatures");
            JungleMenu.AddGroupLabel("清野");
            JungleMenu.AddLabel("技能使用:");
            JungleMenu.Add("Qjungle", new CheckBox("使用 Q"));
            JungleMenu.Add("Wjungle", new CheckBox("使用 W"));
            JungleMenu.Add("Ejungle", new CheckBox("使用 E"));
            JungleMenu.AddSeparator(1);
            JungleMenu.Add("Mjungle", new Slider("蓝量限制百分比 %", 25));

            // LaneClear Menu
            LaneClearMenu = MagicianRyzeMenu.AddSubMenu("清线", "LaneClearFeatures");
            LaneClearMenu.AddGroupLabel("清线");
            LaneClearMenu.AddLabel("技能使用:");
            LaneClearMenu.Add("Qlanec", new CheckBox("使用 Q"));
            LaneClearMenu.Add("Wlanec", new CheckBox("使用 W", false));
            LaneClearMenu.Add("Elanec", new CheckBox("使用 E", false));
            LaneClearMenu.AddSeparator(1);
            LaneClearMenu.Add("Mlanec", new Slider("蓝量限制百分比 %", 25));
            LaneClearMenu.AddSeparator(1);
            LaneClearMenu.Add("Planec", new CheckBox("清线是进行被动叠加"));
            LaneClearMenu.AddSeparator(1);
            LaneClearMenu.AddLabel("后期清线模式 - QWE小兵蓝量使用百分比 %");
            LaneClearMenu.Add("Ulategame", new CheckBox("后期模式", false));
            LaneClearMenu.AddSeparator(1);
            LaneClearMenu.AddLabel("后期模式激活器");
            LaneClearMenu.Add("Llategame", new Slider("开启后期模式当等级为", 14, 1, 18));
            LaneClearMenu.Add("Mlategame", new Slider("蓝量限制百分比 %", 15));

            // LastHit Menu
            LastHitMenu = MagicianRyzeMenu.AddSubMenu("尾兵", "LastHitFeatures");
            LastHitMenu.AddGroupLabel("尾兵");
            LastHitMenu.AddLabel("技能使用:");
            LastHitMenu.Add("Qlasthit", new CheckBox("使用 Q"));
            LastHitMenu.Add("Wlasthit", new CheckBox("使用 W", false));
            LastHitMenu.Add("Elasthit", new CheckBox("使用 E", false));
            LastHitMenu.AddSeparator(1);
            LastHitMenu.Add("Mlasthit", new Slider("蓝量限制百分比 %", 25));

            // Kill Steal Menu
            KillStealMenu = MagicianRyzeMenu.AddSubMenu("抢头", "KSFeatures");
            KillStealMenu.AddGroupLabel("抢头设置");
            KillStealMenu.Add("Uks", new CheckBox("抢头模式"));
            KillStealMenu.AddSeparator(1);
            KillStealMenu.AddLabel("技能使用:");
            KillStealMenu.Add("Qks", new CheckBox("使用 Q 抢头"));
            KillStealMenu.Add("Wks", new CheckBox("使用 W 抢头"));
            KillStealMenu.Add("Eks", new CheckBox("使用 E 抢头"));

            // Drawing Menu
            DrawingMenu = MagicianRyzeMenu.AddSubMenu("线圈", "DrawingFeatures");
            DrawingMenu.AddGroupLabel("线圈设置");
            DrawingMenu.Add("Udraw", new CheckBox("显示线圈"));
            DrawingMenu.AddSeparator(1);
            DrawingMenu.AddLabel("技能显示:");
            DrawingMenu.Add("Qdraw", new CheckBox("显示 Q"));
            DrawingMenu.Add("WEdraw", new CheckBox("显示 W 和 E"));
            DrawingMenu.AddSeparator(1);
            DrawingMenu.AddLabel("换肤");
            DrawingMenu.Add("Udesign", new CheckBox("显示换肤"));
            DrawingMenu.Add("Sdesign", new Slider("Skin Designer: ", 9, 0, 9));

            // Setting Menu
            SettingMenu = MagicianRyzeMenu.AddSubMenu("杂项", "Settings");
            SettingMenu.AddGroupLabel("杂项");
            SettingMenu.AddLabel("自动加点");
            SettingMenu.Add("Ulevel", new CheckBox("自动加点"));
            SettingMenu.AddSeparator(1);
            SettingMenu.AddLabel("自动女神叠加");
            SettingMenu.Add("Ustack", new CheckBox("叠加模式"));
            SettingMenu.AddSeparator(1);
            SettingMenu.AddLabel("大招模式 - QWE滚键盘");
            SettingMenu.Add("Uultimate", new CheckBox("大招模式", false));
            SettingMenu.AddSeparator(1);
            SettingMenu.AddLabel("技能打断");
            SettingMenu.Add("Uinterrupt", new CheckBox("打断模式"));
            SettingMenu.Add("Winterrupt", new CheckBox("使用W打断技能"));
            SettingMenu.AddLabel("防止突击");
            SettingMenu.Add("Ugapc", new CheckBox("防止突击模式"));
            SettingMenu.Add("Wgapc", new CheckBox("使用W造成间距"));
        }

        // Assign Global Checks+
        public static int ComboMode { get { return ComboMenu["Ucombo"].Cast<Slider>().CurrentValue; } }
        public static bool ComboUseQ { get { return ComboMenu["Qcombo"].Cast<CheckBox>().CurrentValue; } }
        public static bool ComboUseW { get { return ComboMenu["Wcombo"].Cast<CheckBox>().CurrentValue; } }
        public static bool ComboUseE { get { return ComboMenu["Ecombo"].Cast<CheckBox>().CurrentValue; } }
        public static bool ComboUseR { get { return ComboMenu["Rcombo"].Cast<CheckBox>().CurrentValue; } }
        public static bool ComboStun { get { return ComboMenu["Dcombo"].Cast<CheckBox>().CurrentValue; } }
        public static int ComboStacks { get { return ComboMenu["Scombo"].Cast<Slider>().CurrentValue; } }

        public static bool HarassUseQ { get { return HarassMenu["Qharass"].Cast<CheckBox>().CurrentValue; } }
        public static int HarassMana { get { return HarassMenu["Mharass"].Cast<Slider>().CurrentValue; } }

        public static bool JungleUseQ { get { return JungleMenu["Qjungle"].Cast<CheckBox>().CurrentValue; } }
        public static bool JungleUseW { get { return JungleMenu["Wjungle"].Cast<CheckBox>().CurrentValue; } }
        public static bool JungleUseE { get { return JungleMenu["Ejungle"].Cast<CheckBox>().CurrentValue; } }
        public static int JungleMana { get { return JungleMenu["Mjungle"].Cast<Slider>().CurrentValue; } }

        public static bool LaneClearUseQ { get { return LaneClearMenu["Qlanec"].Cast<CheckBox>().CurrentValue; } }
        public static bool LaneClearUseW { get { return LaneClearMenu["Wlanec"].Cast<CheckBox>().CurrentValue; } }
        public static bool LaneClearUseE { get { return LaneClearMenu["Elanec"].Cast<CheckBox>().CurrentValue; } }
        public static bool LaneClearPassive { get { return LaneClearMenu["Planec"].Cast<CheckBox>().CurrentValue; } }
        public static int LaneClearMana { get { return LaneClearMenu["Mlanec"].Cast<Slider>().CurrentValue; } }

        public static bool LateGameMode { get { return LaneClearMenu["Ulategame"].Cast<CheckBox>().CurrentValue; } }
        public static int LateGameLevel { get { return LaneClearMenu["Llategame"].Cast<Slider>().CurrentValue; } }
        public static int LateGameMana { get { return LaneClearMenu["Mlategame"].Cast<Slider>().CurrentValue; } }
        
        public static bool LastHitUseQ { get { return LastHitMenu["Qlasthit"].Cast<CheckBox>().CurrentValue; } }
        public static bool LastHitUseW { get { return LastHitMenu["Wlasthit"].Cast<CheckBox>().CurrentValue; } }
        public static bool LastHitUseE { get { return LastHitMenu["Elasthit"].Cast<CheckBox>().CurrentValue; } }
        public static int LastHitMana { get { return LastHitMenu["Mlasthit"].Cast<Slider>().CurrentValue; } }

        public static bool KsMode { get { return KillStealMenu["Uks"].Cast<CheckBox>().CurrentValue; } }
        public static bool KsUseQ { get { return KillStealMenu["Qks"].Cast<CheckBox>().CurrentValue; } }
        public static bool KsUseW { get { return KillStealMenu["Wks"].Cast<CheckBox>().CurrentValue; } }
        public static bool KsUseE { get { return KillStealMenu["Eks"].Cast<CheckBox>().CurrentValue; } }

        public static bool DrawMode { get { return DrawingMenu["Udraw"].Cast<CheckBox>().CurrentValue; } }
        public static bool DrawQ { get { return DrawingMenu["Qdraw"].Cast<CheckBox>().CurrentValue; } }
        public static bool DrawWe { get { return DrawingMenu["WEdraw"].Cast<CheckBox>().CurrentValue; } }
        public static bool DesignerMode { get { return DrawingMenu["Udesign"].Cast<CheckBox>().CurrentValue; } }
        public static int DesignerSkin { get { return DrawingMenu["Sdesign"].Cast<Slider>().CurrentValue; } }

        public static bool LevelerMode { get { return SettingMenu["Ulevel"].Cast<CheckBox>().CurrentValue; } }
        public static bool StackMode { get { return SettingMenu["Ustack"].Cast<CheckBox>().CurrentValue; } }
        public static bool UltimateMode { get { return SettingMenu["Uultimate"].Cast<CheckBox>().CurrentValue; } }

        public static bool InterrupterMode { get { return SettingMenu["Uinterrupt"].Cast<CheckBox>().CurrentValue; } }
        public static bool InterrupterUseW { get { return SettingMenu["Winterrupt"].Cast<CheckBox>().CurrentValue; } }
        public static bool GapCloserMode { get { return SettingMenu["Ugapc"].Cast<CheckBox>().CurrentValue; } }
        public static bool GapCloserUseW { get { return SettingMenu["Wgapc"].Cast<CheckBox>().CurrentValue; } }
    }
}
