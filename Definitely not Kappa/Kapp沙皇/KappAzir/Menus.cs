namespace KappAzir
{
    using Mario_s_Lib;

    using System.Collections.Generic;
    using System.Drawing;

    using EloBuddy;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    using System.Linq;

    internal class Menus
    {
        public const string SpellsMenuID = "Spellsmenuid";

        public const string ComboMenuID = "combomenuid";

        public const string FleeMenuID = "fleemenuid";

        public const string HarassMenuID = "harassmenuid";

        public const string AutoHarassMenuID = "autoharassmenuid";

        public const string LaneClearMenuID = "laneclearmenuid";

        public const string LastHitMenuID = "lasthitmenuid";

        public const string JungleClearMenuID = "jungleclearmenuid";

        public const string KillStealMenuID = "killstealmenuid";

        public const string DrawingsMenuID = "drawingsmenuid";

        public const string MiscMenuID = "miscmenuid";

        public static Menu FirstMenu;

        public static Menu SpellsMenu;

        public static Menu ComboMenu;

        public static Menu FleeMenu;

        public static Menu HarassMenu;

        public static Menu AutoHarassMenu;

        public static Menu LaneClearMenu;

        public static Menu LasthitMenu;

        public static Menu JungleClearMenu;

        public static Menu KillStealMenu;

        public static Menu DrawingsMenu;

        public static Menu MiscMenu;

        //These colorslider are from Mario`s Lib
        public static ColorSlide QColorSlide;

        public static ColorSlide WColorSlide;

        public static ColorSlide EColorSlide;

        public static ColorSlide RColorSlide;

        public static ColorSlide DamageIndicatorColorSlide;

        public static void CreateMenu()
        {
            FirstMenu = MainMenu.AddMenu("Kapp沙皇", "KappAzir");
            FleeMenu = FirstMenu.AddSubMenu("• 漂移", FleeMenuID);
            SpellsMenu = FirstMenu.AddSubMenu("• 技能管理", SpellsMenuID);
            ComboMenu = FirstMenu.AddSubMenu("• 连招", ComboMenuID);
            HarassMenu = FirstMenu.AddSubMenu("• 骚扰", HarassMenuID);
            AutoHarassMenu = FirstMenu.AddSubMenu("• 自动骚扰", AutoHarassMenuID);
            LaneClearMenu = FirstMenu.AddSubMenu("• 清线", LaneClearMenuID);
            LasthitMenu = FirstMenu.AddSubMenu("• 尾兵", LastHitMenuID);
            JungleClearMenu = FirstMenu.AddSubMenu("• 清野", JungleClearMenuID);
            KillStealMenu = FirstMenu.AddSubMenu("• 抢头", KillStealMenuID);
            MiscMenu = FirstMenu.AddSubMenu("• 杂项", MiscMenuID);
            DrawingsMenu = FirstMenu.AddSubMenu("• 线圈", DrawingsMenuID);

            FleeMenu.AddGroupLabel("漂移 - 逃跑");
            FleeMenu.Add("flee", new KeyBind("漂移按键", false, KeyBind.BindTypes.HoldActive, 'A'));
            FleeMenu.CreateSlider("使用 {0} 范围内的士兵", "range", 1000, 150, 1500);
            FleeMenu.CreateSlider("EQ 延迟 = {0}", "delay", 250, 150, 500);
            FleeMenu.AddSeparator(0);
            FleeMenu.AddLabel("上面的控制E和Q之间的延迟 E > Q");
            FleeMenu.AddLabel("漂移推和逃跑模式中使用的延迟");
            FleeMenu.AddSeparator(0);
            FleeMenu.AddGroupLabel("漂移推模式");
            FleeMenu.AddLabel("选择目标后按下神推按键");
            FleeMenu.Add("insect", new KeyBind("普通漂移推", false, KeyBind.BindTypes.HoldActive, 'S'));
            FleeMenu.Add("insected", new KeyBind("新漂移推", false, KeyBind.BindTypes.HoldActive, 'Z'));
            FleeMenu.AddGroupLabel("普通漂移推设置");
            FleeMenu.CreateCheckBox(" - 推至友军", "Ally");
            FleeMenu.CreateCheckBox(" - 推至友军塔", "Tower");
            FleeMenu.AddSeparator(0);
            FleeMenu.AddGroupLabel("新漂移推设置");
            FleeMenu.CreateComboBox("Q 位置", "qpos", new List<string> { "至鼠标", "至之前的位置", "至塔", "至友军" });
            FleeMenu.CreateComboBox("R 位置", "rpos", new List<string> { "至鼠标", "至之前的位置", "至塔", "至友军" });
            FleeMenu.CreateSlider("降低 Q 距离 [{0}]", "dis", 0, 0, 500);

            SpellsMenu.AddGroupLabel("技能管理");
            SpellsMenu.CreateCheckBox(" - R 防突进", "rUseGap");
            SpellsMenu.CreateCheckBox(" - R 技能打断", "rUseInt");
            SpellsMenu.CreateComboBox("技能危险等级打断", "Intdanger", new List<string> { "高", "中", "低" });
            SpellsMenu.AddGroupLabel("命中率");
            SpellsMenu.CreateComboBox("命中率", "chance", new List<string> { "高", "中", "低" });

            ComboMenu.AddGroupLabel("技能");
            ComboMenu.CreateCheckBox(" - 使用 Q", "qUse");
            ComboMenu.CreateCheckBox(" - 使用 W", "wUse");
            ComboMenu.CreateCheckBox(" - 使用 E", "eUse");
            ComboMenu.CreateCheckBox(" - 使用 R", "rUse");
            ComboMenu.CreateSlider("附近敌人多于 [{0}] 召唤防御塔", "TowerPls", 2, 1, 5);
            ComboMenu.AddSeparator();
            ComboMenu.AddGroupLabel("Q 额外设置");
            ComboMenu.CreateCheckBox(" - 使用 Q 如果敌人不在 Q 范围", "qUseAA", false);
            ComboMenu.CreateSlider("士兵数量使用 Q [{0}]", "Qcount", 1, 1, 3);
            ComboMenu.CreateSlider("Q 命中数量 [{0}]", "Qaoe", 2, 1, 5);
            ComboMenu.AddSeparator();
            ComboMenu.AddGroupLabel("W 额外设置");
            ComboMenu.CreateCheckBox(" - 使用 W 如果敌人不在范围", "wUseAA", false);
            ComboMenu.CreateCheckBox(" - 保留 1 个 W", "wSave", false);
            ComboMenu.AddSeparator();
            ComboMenu.AddGroupLabel("E 额外设置");
            ComboMenu.CreateCheckBox(" - 只使用 E 当目标可被击杀", "eUsekill");
            ComboMenu.CreateCheckBox(" - E 越塔", "eUseDive", false);
            ComboMenu.CreateSlider("不 E 如果目标血量高于我的血量 [{0}%]", "eHealth", 15);
            ComboMenu.CreateSlider("不 E 如果敌人附近敌方英雄多于[{0}] 个", "eSave", 2, 1, 5);
            ComboMenu.AddSeparator();
            ComboMenu.AddGroupLabel("R 额外设置");
            ComboMenu.CreateCheckBox(" - R Over Kill Check", "rOverKill");
            ComboMenu.CreateCheckBox(" - 使用 R Finisher", "rUsekill");
            ComboMenu.CreateCheckBox(" - 使用 R Saver", "rUseSave", false);
            ComboMenu.CreateCheckBox(" - 推向友军", "rUseAlly");
            ComboMenu.CreateCheckBox(" - 推行友军塔", "rUseTower");
            ComboMenu.Add("Rcast", new KeyBind("半自动 R", false, KeyBind.BindTypes.HoldActive, 'R'));
            ComboMenu.CreateSlider("R 命中数量 [{0}]", "Raoe", 2, 1, 5);

            HarassMenu.AddGroupLabel("技能");
            HarassMenu.CreateCheckBox(" - 使用 Q", "qUse");
            HarassMenu.CreateCheckBox(" - 使用 W", "wUse");
            HarassMenu.CreateCheckBox(" - 使用 E", "eUse");
            HarassMenu.AddGroupLabel("设置");
            HarassMenu.CreateCheckBox(" - 保留 1 个 W", "wSave");
            HarassMenu.CreateCheckBox(" - E 越塔", "eUseDive", false);
            HarassMenu.CreateSlider("不 E 如果敌人附近敌方英雄多于[{0}] 个", "eSave", 3, 1, 5);
            HarassMenu.CreateSlider("蓝量多于 [{0}%] 才使用技能骚扰", "manaSlider", 60);

            AutoHarassMenu.AddGroupLabel("技能");
            AutoHarassMenu.CreateCheckBox(" - 使用 Q", "qUse");
            AutoHarassMenu.CreateCheckBox(" - 使用 W", "wUse");
            AutoHarassMenu.CreateCheckBox(" - 使用 E", "eUse");
            AutoHarassMenu.AddGroupLabel("设置");
            AutoHarassMenu.CreateCheckBox(" - 保留 1 个 W", "wSave");
            AutoHarassMenu.CreateCheckBox(" - 总是使用士兵攻击", "attack", false);
            AutoHarassMenu.CreateCheckBox(" - E 越塔", "eDive", false);
            AutoHarassMenu.CreateSlider("不 E 如果敌人附近敌方英雄多于[{0}] 个", "eSave", 3, 1, 5);
            AutoHarassMenu.CreateKeyBind("自动骚扰开关按键", "autoHarassKey", 'Z', 'U');
            AutoHarassMenu.CreateSlider("蓝量多于 [{0}%] 才使用技能骚扰", "manaSlider", 60);

            LaneClearMenu.AddGroupLabel("技能");
            LaneClearMenu.CreateCheckBox(" - 使用 Q", "qUse");
            LaneClearMenu.CreateCheckBox(" - 使用 W", "wUse");
            LaneClearMenu.AddGroupLabel("设置");
            LaneClearMenu.CreateCheckBox(" - 保留 1 个 W", "wSave");
            LaneClearMenu.CreateCheckBox(" - 使用 W 攻击敌方塔", "wTurret");
            LaneClearMenu.CreateSlider("蓝量多于 [{0}%] 才使用技能清线", "manaSlider", 75);

            LasthitMenu.AddGroupLabel("技能");
            LasthitMenu.CreateCheckBox(" - 使用 Q", "qUse");
            LasthitMenu.AddGroupLabel("设置");
            LasthitMenu.CreateSlider("蓝量多于 [{0}%] 才使用技能尾兵", "manaSlider", 75);

            JungleClearMenu.AddGroupLabel("技能");
            JungleClearMenu.CreateCheckBox(" - 使用 Q", "qUse");
            JungleClearMenu.CreateCheckBox(" - 使用 W", "wUse");
            JungleClearMenu.AddGroupLabel("设置");
            JungleClearMenu.CreateCheckBox(" - 保留 1 个 W", "wSave");
            JungleClearMenu.CreateSlider("蓝量多于 [{0}%] 才使用技能清野", "manaSlider", 50);

            KillStealMenu.AddGroupLabel("技能");
            KillStealMenu.CreateCheckBox(" - 使用 Q", "qUse");
            KillStealMenu.CreateCheckBox(" - 使用 W", "wUse");
            KillStealMenu.CreateCheckBox(" - 使用 E", "eUse");
            KillStealMenu.CreateCheckBox(" - 使用 R", "rUse");

            MiscMenu.AddGroupLabel("换肤");

            var skinList = Mario_s_Lib.DataBases.Skins.SkinsDB.FirstOrDefault(list => list.Champ == Player.Instance.Hero);
            if (skinList != null)
            {
                MiscMenu.CreateComboBox("选择皮肤", "skinComboBox", skinList.Skins);
                MiscMenu.Get<ComboBox>("skinComboBox").OnValueChange +=
                    delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args) { Player.Instance.SetSkinId(sender.CurrentValue); };
            }

            MiscMenu.AddGroupLabel("自动加点");
            MiscMenu.CreateCheckBox("开启自动加点", "activateAutoLVL", false);
            MiscMenu.AddLabel("将会优先加R");
            MiscMenu.CreateComboBox("第一技能", "firstFocus", new List<string> { "Q", "W", "E" });
            MiscMenu.CreateComboBox("第二技能", "secondFocus", new List<string> { "Q", "W", "E" }, 1);
            MiscMenu.CreateComboBox("第三技能", "thirdFocus", new List<string> { "Q", "W", "E" }, 2);
            MiscMenu.CreateSlider("延迟", "delaySlider", 200, 150, 500);

            DrawingsMenu.AddGroupLabel("设置");
            DrawingsMenu.CreateCheckBox(" - 只显示无冷却技能", "readyDraw");
            DrawingsMenu.CreateCheckBox(" - 显示伤害指示器.", "damageDraw");
            DrawingsMenu.CreateCheckBox(" - 显示伤害百分比", "perDraw");
            DrawingsMenu.CreateCheckBox(" - 显示伤害指示器数据.", "statDraw", false);
            DrawingsMenu.AddGroupLabel("技能");
            DrawingsMenu.CreateCheckBox(" - 显示 Q.", "qDraw");
            DrawingsMenu.CreateCheckBox(" - 显示 W.", "wDraw");
            DrawingsMenu.CreateCheckBox(" - 显示 E.", "eDraw");
            DrawingsMenu.CreateCheckBox(" - 显示 R.", "rDraw");
            DrawingsMenu.AddGroupLabel("线圈颜色");
            QColorSlide = new ColorSlide(DrawingsMenu, "qColor", Color.Red, "Q 颜色:");
            WColorSlide = new ColorSlide(DrawingsMenu, "wColor", Color.Purple, "W 颜色:");
            EColorSlide = new ColorSlide(DrawingsMenu, "eColor", Color.Orange, "E 颜色:");
            RColorSlide = new ColorSlide(DrawingsMenu, "rColor", Color.DeepPink, "R 颜色:");
            DamageIndicatorColorSlide = new ColorSlide(DrawingsMenu, "healthColor", Color.YellowGreen, "DamageIndicator Color:");
        }
    }
}