using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace BallistaKogMaw
{
    internal class MenuManager
    {
        // Create Main Segments
        public static Menu BallistaKogMawMenu, ComboMenu, HarassMenu, JungleMenu, LaneClearMenu, LastHitMenu, KillStealMenu, DrawingMenu, SettingMenu;

        public static void Initialize()
        {
            // Addon Menu
            BallistaKogMawMenu = MainMenu.AddMenu("CH汉化-弩弓大嘴", "BallistaKogMaw");
            BallistaKogMawMenu.AddGroupLabel("CH汉化-弩弓大嘴");

            // Combo Menu
            ComboMenu = BallistaKogMawMenu.AddSubMenu("连招", "ComboFeatures");
            ComboMenu.AddGroupLabel("连招设置");
            ComboMenu.AddLabel("技能使用:");
            ComboMenu.Add("Qcombo", new CheckBox("使用 Q"));
            ComboMenu.Add("Wcombo", new CheckBox("使用 W"));
            ComboMenu.Add("Ecombo", new CheckBox("使用 E"));
            ComboMenu.Add("Rcombo", new CheckBox("使用 R"));
            ComboMenu.Add("Scombo", new Slider("最大R叠加", 2, 1, 10));

            // Harass Menu
            HarassMenu = BallistaKogMawMenu.AddSubMenu("骚扰", "HarassFeatures");
            HarassMenu.AddGroupLabel("骚扰设置");
            HarassMenu.AddLabel("技能使用:");
            HarassMenu.Add("Qharass", new CheckBox("使用 Q"));
            HarassMenu.Add("Wharass", new CheckBox("使用 W", false));
            HarassMenu.Add("Eharass", new CheckBox("使用 E", false));
            HarassMenu.Add("Rharass", new CheckBox("使用 R", false));
            HarassMenu.Add("Sharass", new Slider("最大R叠加", 1, 1, 10));
            HarassMenu.AddSeparator(1);
            HarassMenu.Add("Mharass", new Slider("蓝量限制百分比 %", 25));

            // Jungle Menu
            JungleMenu = BallistaKogMawMenu.AddSubMenu("清野", "JungleFeatures");
            JungleMenu.AddGroupLabel("清野设置");
            JungleMenu.AddLabel("技能使用:");
            JungleMenu.Add("Qjungle", new CheckBox("使用 Q"));
            JungleMenu.Add("Wjungle", new CheckBox("使用 W"));
            JungleMenu.Add("Rjungle", new CheckBox("使用 R", false));
            JungleMenu.Add("Sjungle", new Slider("最大R叠加", 1, 1, 10));
            JungleMenu.AddSeparator(1);
            JungleMenu.Add("Mjungle", new Slider("蓝量限制百分比 %", 25));

            // LaneClear Menu
            LaneClearMenu = BallistaKogMawMenu.AddSubMenu("清线", "LaneClearFeatures");
            LaneClearMenu.AddGroupLabel("清线设置");
            LaneClearMenu.AddLabel("技能使用:");
            LaneClearMenu.Add("Qlanec", new CheckBox("使用 Q", false));
            LaneClearMenu.Add("Wlanec", new CheckBox("使用 W", false));
            LaneClearMenu.Add("Rlanec", new CheckBox("使用 R", false));
            LaneClearMenu.Add("Slanec", new Slider("最大R叠加", 1, 1, 10));
            LaneClearMenu.AddSeparator(1);
            LaneClearMenu.Add("Mlanec", new Slider("蓝量限制百分比 %", 25));

            // LastHit Menu
            LastHitMenu = BallistaKogMawMenu.AddSubMenu("尾兵", "LastHitFeatures");
            LastHitMenu.AddGroupLabel("尾兵设置");
            LastHitMenu.AddLabel("技能使用:");
            LastHitMenu.Add("Qlasthit", new CheckBox("使用 Q"));
            LastHitMenu.Add("Rlasthit", new CheckBox("使用 R", false));
            LastHitMenu.Add("Slasthit", new Slider("最大R叠加", 1, 1, 10));
            LastHitMenu.AddSeparator(1);
            LastHitMenu.Add("Mlasthit", new Slider("蓝量限制百分比 %", 25));

            // Kill Steal Menu
            KillStealMenu = BallistaKogMawMenu.AddSubMenu("抢头", "KSFeatures");
            KillStealMenu.AddGroupLabel("抢头设置");
            KillStealMenu.Add("Uks", new CheckBox("抢头模式"));
            KillStealMenu.AddSeparator(1);
            KillStealMenu.AddLabel("技能使用:");
            KillStealMenu.Add("Qks", new CheckBox("使用 Q 抢头"));
            KillStealMenu.Add("Rks", new CheckBox("使用 R 抢头"));

            // Drawing Menu
            DrawingMenu = BallistaKogMawMenu.AddSubMenu("线圈", "DrawingFeatures");
            DrawingMenu.AddGroupLabel("线圈设置");
            DrawingMenu.Add("Udrawer", new CheckBox("显示线圈"));
            DrawingMenu.AddSeparator(1);
            DrawingMenu.AddLabel("技能线圈:");
            DrawingMenu.Add("Qdraw", new CheckBox("显示 Q"));
            DrawingMenu.Add("Wdraw", new CheckBox("显示 W"));
            DrawingMenu.Add("Edraw", new CheckBox("显示 E"));
            DrawingMenu.Add("Rdraw", new CheckBox("显示 R"));
            DrawingMenu.AddSeparator(1);
            DrawingMenu.AddLabel("换肤");
            DrawingMenu.Add("Udesigner", new CheckBox("使用换肤"));
            DrawingMenu.Add("Sdesign", new Slider("Skin Designer: ", 7, 0, 8));

            // Setting Menu
            SettingMenu = BallistaKogMawMenu.AddSubMenu("杂项", "Settings");
            SettingMenu.AddGroupLabel("杂项");
            SettingMenu.AddLabel("自动加点");
            SettingMenu.Add("Uleveler", new CheckBox("使用自动加点"));
            SettingMenu.AddSeparator(1);
            SettingMenu.AddLabel("自动叠加女神");
            SettingMenu.Add("Ustacker", new CheckBox("使用自动叠加女神"));
            SettingMenu.AddSeparator(1);
            SettingMenu.AddLabel("自动被动 - 死亡跟随");
            SettingMenu.Add("Ufollower", new CheckBox("使用死亡跟随"));
            SettingMenu.AddSeparator(1);
            SettingMenu.AddLabel("防突击");
            SettingMenu.Add("Ugapc", new CheckBox("使用防突进"));
            SettingMenu.Add("Egapc", new CheckBox("使用造成间距"));
        }

        // Assign Global Checks+
        public static bool ComboUseQ { get { return ComboMenu["Qcombo"].Cast<CheckBox>().CurrentValue; } }
        public static bool ComboUseW { get { return ComboMenu["Wcombo"].Cast<CheckBox>().CurrentValue; } }
        public static bool ComboUseE { get { return ComboMenu["Ecombo"].Cast<CheckBox>().CurrentValue; } }
        public static bool ComboUseR { get { return ComboMenu["Rcombo"].Cast<CheckBox>().CurrentValue; } }
        public static int ComboStacks { get { return ComboMenu["Scombo"].Cast<Slider>().CurrentValue; } }

        public static bool HarassUseQ { get { return HarassMenu["Qharass"].Cast<CheckBox>().CurrentValue; } }
        public static bool HarassUseW { get { return HarassMenu["Wharass"].Cast<CheckBox>().CurrentValue; } }
        public static bool HarassUseE { get { return HarassMenu["Eharass"].Cast<CheckBox>().CurrentValue; } }
        public static bool HarassUseR { get { return HarassMenu["Rharass"].Cast<CheckBox>().CurrentValue; } }
        public static int HarassStacks { get { return HarassMenu["Sharass"].Cast<Slider>().CurrentValue; } }
        public static int HarassMana { get { return HarassMenu["Mharass"].Cast<Slider>().CurrentValue; } }

        public static bool JungleUseQ { get { return JungleMenu["Qjungle"].Cast<CheckBox>().CurrentValue; } }
        public static bool JungleUseW { get { return JungleMenu["Wjungle"].Cast<CheckBox>().CurrentValue; } }
        public static bool JungleUseE { get { return JungleMenu["Ejungle"].Cast<CheckBox>().CurrentValue; } }
        public static bool JungleUseR { get { return JungleMenu["Rjungle"].Cast<CheckBox>().CurrentValue; } }
        public static int JungleStacks { get { return JungleMenu["Sjungle"].Cast<Slider>().CurrentValue; } }
        public static int JungleMana { get { return JungleMenu["Mjungle"].Cast<Slider>().CurrentValue; } }

        public static bool LaneClearUseQ { get { return LaneClearMenu["Qlanec"].Cast<CheckBox>().CurrentValue; } }
        public static bool LaneClearUseW { get { return LaneClearMenu["Wlanec"].Cast<CheckBox>().CurrentValue; } }
        public static bool LaneClearUseE { get { return LaneClearMenu["Elanec"].Cast<CheckBox>().CurrentValue; } }
        public static bool LaneClearUseR { get { return LaneClearMenu["Rlanec"].Cast<CheckBox>().CurrentValue; } }
        public static int LaneClearStacks { get { return LaneClearMenu["Slanec"].Cast<Slider>().CurrentValue; } }
        public static int LaneClearMana { get { return LaneClearMenu["Mlanec"].Cast<Slider>().CurrentValue; } }

        public static bool LastHitUseQ { get { return LastHitMenu["Qlasthit"].Cast<CheckBox>().CurrentValue; } }
        public static bool LastHitUseR { get { return LastHitMenu["Rlasthit"].Cast<CheckBox>().CurrentValue; } }
        public static int LastHitStacks { get { return LastHitMenu["Slasthit"].Cast<Slider>().CurrentValue; } }
        public static int LastHitMana { get { return LastHitMenu["Mlasthit"].Cast<Slider>().CurrentValue; } }

        public static bool KsMode { get { return KillStealMenu["Uks"].Cast<CheckBox>().CurrentValue; } }
        public static bool KsUseQ { get { return KillStealMenu["Qks"].Cast<CheckBox>().CurrentValue; } }
        public static bool KsUseR { get { return KillStealMenu["Rks"].Cast<CheckBox>().CurrentValue; } }

        public static bool DrawMode { get { return DrawingMenu["Udrawer"].Cast<CheckBox>().CurrentValue; } }
        public static bool DrawQ { get { return DrawingMenu["Qdraw"].Cast<CheckBox>().CurrentValue; } }
        public static bool DrawW { get { return DrawingMenu["Wdraw"].Cast<CheckBox>().CurrentValue; } }
        public static bool DrawE { get { return DrawingMenu["Edraw"].Cast<CheckBox>().CurrentValue; } }
        public static bool DrawR { get { return DrawingMenu["Rdraw"].Cast<CheckBox>().CurrentValue; } }
        public static bool DesignerMode { get { return DrawingMenu["Udesigner"].Cast<CheckBox>().CurrentValue; } }
        public static int DesignerSkin { get { return DrawingMenu["Sdesign"].Cast<Slider>().CurrentValue; } }

        public static bool LevelerMode { get { return SettingMenu["Uleveler"].Cast<CheckBox>().CurrentValue; } }
        public static bool StackerMode { get { return SettingMenu["Ustacker"].Cast<CheckBox>().CurrentValue; } }
        public static bool FollowerMode { get { return SettingMenu["Ufollower"].Cast<CheckBox>().CurrentValue; } }

        public static bool GapCloserMode { get { return SettingMenu["Ugapc"].Cast<CheckBox>().CurrentValue; } }
        public static bool GapCloserUseE { get { return SettingMenu["Egapc"].Cast<CheckBox>().CurrentValue; } }
    }
}
