using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK.Menu;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace Aka_s_Vayne_reworked
{
    class MenuManager
    {
        public static Menu VMenu,
            Qsettings,
            ComboMenu,
            CondemnMenu,
            HarassMenu,
            FleeMenu,
            LaneClearMenu,
            JungleClearMenu,
            MiscMenu,
            ItemMenu,
            DrawingMenu;

        public static void Load()
        {
            Mainmenu();
            Combomenu();
            QSettings();
            Condemnmenu();
            Harassmenu();
            Fleemenu();
            LaneClearmenu();
            JungleClearmenu();
            Miscmenu();
            Itemmenu();
            Drawingmenu();
        }

        public static void Mainmenu()
        {
            VMenu = MainMenu.AddMenu("Aka´s Vayne", "akavayne");
            VMenu.AddGroupLabel("Welcome to my Vayne Addon have fun! :)");
            VMenu.AddGroupLabel("Made by Aka *-*");
        }

        public static void Combomenu()
        {
            ComboMenu = VMenu.AddSubMenu("连招", "Combo");
            ComboMenu.AddGroupLabel("连招");
            ComboMenu.AddGroupLabel("Q 模式");
            ComboMenu.AddLabel("智能模式无效. 请使用“新版”，“旧版”.");
            var qmode = ComboMenu.Add("Qmode", new ComboBox("Q 模式", 4, "鼠标", "智能(无效)", "风筝", "旧版", "新版"));
            qmode.OnValueChange += delegate
            {
                if (qmode.CurrentValue == 1)
                {
                    Qsettings["UseSafeQ"].IsVisible = true;
                    Qsettings["UseQE"].IsVisible = true;
                    Qsettings["QE"].IsVisible = true;
                    Qsettings["UseQspam"].IsVisible = true;
                    Qsettings["QNmode"].IsVisible = false;
                    Qsettings["QNenemies"].IsVisible = false;
                    Qsettings["QNWall"].IsVisible = false;
                    Qsettings["QNTurret"].IsVisible = false;
                }
                if (qmode.CurrentValue == 3)
                {
                    Qsettings["UseSafeQ"].IsVisible = false;
                    Qsettings["UseQE"].IsVisible = false;
                    Qsettings["QE"].IsVisible = false;
                    Qsettings["UseQspam"].IsVisible = false;
                    Qsettings["QNmode"].IsVisible = false;
                    Qsettings["QNenemies"].IsVisible = false;
                    Qsettings["QNWall"].IsVisible = false;
                    Qsettings["QNTurret"].IsVisible = false;
                }
                if (qmode.CurrentValue == 2)
                {
                    Qsettings["UseSafeQ"].IsVisible = false;
                    Qsettings["UseQE"].IsVisible = false;
                    Qsettings["QE"].IsVisible = false;
                    Qsettings["UseQspam"].IsVisible = false;
                    Qsettings["QNmode"].IsVisible = false;
                    Qsettings["QNenemies"].IsVisible = false;
                    Qsettings["QNWall"].IsVisible = false;
                    Qsettings["QNTurret"].IsVisible = false;
                }
                if (qmode.CurrentValue == 0)
                {
                    Qsettings["UseSafeQ"].IsVisible = false;
                    Qsettings["UseQE"].IsVisible = false;
                    Qsettings["QE"].IsVisible = false;
                    Qsettings["UseQspam"].IsVisible = false;
                    Qsettings["QNmode"].IsVisible = false;
                    Qsettings["QNenemies"].IsVisible = false;
                    Qsettings["QNWall"].IsVisible = false;
                    Qsettings["QNTurret"].IsVisible = false;
                }
                if (qmode.CurrentValue == 4)
                {
                    Qsettings["UseSafeQ"].IsVisible = false;
                    Qsettings["UseQE"].IsVisible = false;
                    Qsettings["QE"].IsVisible = false;
                    Qsettings["UseQspam"].IsVisible = false;
                    Qsettings["QNmode"].IsVisible = true;
                    Qsettings["QNenemies"].IsVisible = true;
                    Qsettings["QNWall"].IsVisible = true;
                    Qsettings["QNTurret"].IsVisible = true;
                }
            };
            ComboMenu.Add("Qmode2", new ComboBox("Smart Mode", 0, "Aggressive", "Defensive"));
            ComboMenu.Add("UseQwhen", new ComboBox("Use Q", 0, "After Attack", "Before Attack", "Never"));
            ComboMenu.AddGroupLabel("普攻重置");
            ComboMenu.AddLabel("更改选项需要重新加载脚本 F5");
            ComboMenu.Add("AAReset", new CheckBox("使用独立走砍可以更好的进行风筝"));
            ComboMenu.AddLabel("如果普攻卡刀，使用这个或者停用独立走砍");
            ComboMenu.Add("AACancel", new Slider("普攻重置", 0, 0, 20));
            ComboMenu.AddGroupLabel("W 设置");
            ComboMenu.Add("focusw", new CheckBox("集火 W目标", false));
            ComboMenu.AddGroupLabel("E 设置");
            ComboMenu.Add("Ekill", new CheckBox("可击杀使用 E?"));
            ComboMenu.Add("comboUseE", new CheckBox("使用 E"));
            ComboMenu.AddGroupLabel("R 设置");
            ComboMenu.Add("comboUseR", new CheckBox("使用 R", false));
            ComboMenu.Add("comboRSlider", new Slider("使用R如果附近敌人数量为", 2, 1, 5));
            ComboMenu.Add("RnoAA", new CheckBox("隐身时不普攻", false));
            ComboMenu.Add("RnoAAs", new Slider("附近敌人数量为X，隐身不普攻", 2, 0, 5));
        }

        public static void QSettings()
        {
            Qsettings = VMenu.AddSubMenu("Q 设置", "Q Settings");
            Qsettings.AddGroupLabel("Q 设置");
            Qsettings.AddLabel("爆发模式将会Q进墙（更快普攻重置）.");
            Qsettings.Add("Mirin", new CheckBox("爆发模式"));
            //smart
            Qsettings.Add("UseSafeQ", new CheckBox("Q向安全位置?（动态计算）", false)).IsVisible = true;
            Qsettings.Add("UseQE", new CheckBox("不Q进敌人", false)).IsVisible = true;
            Qsettings.Add("QE", new CheckBox("尝试QE", false)).IsVisible = true;
            Qsettings.Add("UseQspam", new CheckBox("忽略检查", false)).IsVisible = true;
            //new
            Qsettings.Add("QNmode", new ComboBox("新版模式", 1, "边上", "安全位置")).IsVisible = false;
            Qsettings.Add("QNenemies", new Slider("附近X名敌人阻挡Q", 3, 5, 0)).IsVisible = false;
            Qsettings.Add("QNWall", new CheckBox("防止Q墙", true)).IsVisible = false;
            Qsettings.Add("QNTurret", new CheckBox("防止Q塔下", true)).IsVisible = false;

        }

        public static void Condemnmenu()
        {
            CondemnMenu = VMenu.AddSubMenu("定墙", "Condemn");
            CondemnMenu.AddGroupLabel("定墙");
            CondemnMenu.AddLabel("Shine和AKA停止使用.");
            CondemnMenu.Add("Condemnmode", new ComboBox("定墙模式", 2, "最强", "新版", "神射手", "Shine(disabled)", "Aka(disabled)"));
            CondemnMenu.Add("UseEauto", new CheckBox("自动定墙？"));
            CondemnMenu.Add("UseEc", new CheckBox("只定墙当前目标", false));
            CondemnMenu.Add("condemnPercent", new Slider("定墙命中率 %", 33, 1));
            CondemnMenu.Add("pushDistance", new Slider("E推行距离", 420, 350, 470));
            CondemnMenu.Add("noeaa", new Slider("不使用E 当目标能被 X 普攻杀死", 0, 0, 4));
            CondemnMenu.Add("trinket", new CheckBox("自动草丛插眼"));
            CondemnMenu.Add("j4flag", new CheckBox("E 皇子旗?"));
            CondemnMenu.AddGroupLabel("神技");
            CondemnMenu.Add("flashe", new KeyBind("闪现定墙!", false, KeyBind.BindTypes.HoldActive, 'Y'));
            CondemnMenu.Add("insece", new KeyBind("闪现 推人!", false, KeyBind.BindTypes.HoldActive, 'Z'));
            CondemnMenu.Add("insecmodes", new ComboBox("推人模式", 0, "至友军", "至塔下", "至鼠标"));
        }

        public static void Harassmenu()
        {
            HarassMenu = VMenu.AddSubMenu("骚扰", "Harass");
            HarassMenu.AddGroupLabel("骚扰");
            HarassMenu.AddLabel("建议只选择一个，我推荐Q.");
            HarassMenu.Add("UseQHarass", new CheckBox("使用 Q(如果2层W)"));
            HarassMenu.Add("UseEHarass", new CheckBox("使用 E(如果2层W)", false));
            HarassMenu.Add("UseCHarass", new CheckBox("使用连招: 普攻 -> Q+普攻 -> E(无冷却时)", false));
            HarassMenu.Add("ManaHarass", new Slider("骚扰连招蓝量使用 ({0}%)", 40));
        }

        public static void Fleemenu()
        {
            FleeMenu = VMenu.AddSubMenu("逃跑", "Flee");
            FleeMenu.AddGroupLabel("逃跑");
            FleeMenu.Add("FleeUseQ", new CheckBox("使用 Q"));
            FleeMenu.Add("FleeUseE", new CheckBox("使用 E"));
        }

        public static void LaneClearmenu()
        {
            LaneClearMenu = VMenu.AddSubMenu("清线", "LaneClear");
            LaneClearMenu.AddGroupLabel("清线");
            LaneClearMenu.Add("LCQ", new CheckBox("使用 Q"));
            LaneClearMenu.Add("LCQMana", new Slider("最大蓝量使用百分比 ({0}%)", 40));

        }

        public static void JungleClearmenu()
        {
            JungleClearMenu = VMenu.AddSubMenu("清野", "JungleClear");
            JungleClearMenu.AddGroupLabel("清野");
            JungleClearMenu.Add("JCQ", new CheckBox("使用 Q"));
            JungleClearMenu.Add("JCE", new CheckBox("使用 E"));
        }

        public static void Miscmenu()
        {
            MiscMenu = VMenu.AddSubMenu("杂项", "Misc");
            MiscMenu.AddGroupLabel("杂项");
            MiscMenu.AddLabel("感谢Fluxy:");
            MiscMenu.Add("GapcloseE", new CheckBox("E防突进"));
            MiscMenu.Add("AntiRengar", new CheckBox("防狮子狗"));
            MiscMenu.Add("AntiPanth", new CheckBox("防潘森"));
            MiscMenu.Add("fpsdrop", new CheckBox("防掉FPS", false));
            MiscMenu.Add("InterruptE", new CheckBox("E技能打断"));
            MiscMenu.Add("LowLifeE", new CheckBox("低生命 E", false));
            MiscMenu.Add("dangerLevel", new ComboBox("E技能打断危险等级 ", 2, "低", "中", "高"));
            MiscMenu.AddGroupLabel("通用");
            MiscMenu.Add("skinhack", new CheckBox("开启换肤"));
            MiscMenu.Add("skinId", new ComboBox("换肤", 0, "初始", "摩登骇客", "猎天使魔女", "巨龙追猎者", "觅心猎手", "SKT T1", "苍穹之光", "绿色包", "红色包", "灰色包"));
            MiscMenu.Add("autolvl", new CheckBox("自动加点"));
            MiscMenu.Add("autolvls", new ComboBox("加点模式", 0, "主 W", "主 Q(我喜欢)"));
            MiscMenu.Add("autobuy", new CheckBox("开局自动买装备"));
            MiscMenu.Add("autobuyt", new CheckBox("自动买饰品", false));
            switch (MiscMenu["autolvls"].Cast<ComboBox>().CurrentValue)
            {
                case 0:
                    Variables.AbilitySequence = new[] { 1, 3, 2, 2, 2, 4, 2, 1, 2, 1, 4, 1, 1, 3, 3, 4, 3, 3 };
                    break;
                case 1:
                    Variables.AbilitySequence = new[] { 1, 3, 2, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
                    break;
            }
        }

        public static void Itemmenu()
        {
            ItemMenu = VMenu.AddSubMenu("活化剂", "Activator");
            ItemMenu.AddGroupLabel("物品");
            ItemMenu.AddLabel("私聊我如果需要添加物品");
            ItemMenu.Add("botrk", new CheckBox("使用破败/弯刀"));
            ItemMenu.Add("autopotion", new CheckBox("自动吃药"));
            ItemMenu.Add("autopotionhp", new Slider("生命值低于 X", 60));
            ItemMenu.Add("autobiscuit", new CheckBox("Auto Biscuit"));
            ItemMenu.Add("autobiscuithp", new Slider("生命值低于 X", 60));
            ItemMenu.AddGroupLabel("召唤师技能");
            ItemMenu.AddLabel("私聊我如果需要添加技能");
            ItemMenu.Add("heal", new CheckBox("治疗"));
            ItemMenu.Add("hp", new Slider("生命值低于", 20, 0, 100));
            ItemMenu.Add("healally", new CheckBox("治疗友军"));
            ItemMenu.Add("hpally", new Slider("友军生命值低于", 20, 0, 100));
            ItemMenu.AddGroupLabel("解控");
            ItemMenu.Add("qss", new CheckBox("使用水银"));
            ItemMenu.Add("delay", new Slider("延迟", 100, 0, 2000));
            ItemMenu.Add("Blind",
                new CheckBox("致盲", false));
            ItemMenu.Add("Charm",
                new CheckBox("魅惑"));
            ItemMenu.Add("Fear",
                new CheckBox("恐惧"));
            ItemMenu.Add("Polymorph",
                new CheckBox("变形"));
            ItemMenu.Add("Stun",
                new CheckBox("晕眩"));
            ItemMenu.Add("Snare",
                new CheckBox("禁锢"));
            ItemMenu.Add("Silence",
                new CheckBox("沉默", false));
            ItemMenu.Add("Taunt",
                new CheckBox("嘲讽"));
            ItemMenu.Add("Suppression",
                new CheckBox("压制"));

        }
      
        public static void Drawingmenu()
        {
            DrawingMenu = VMenu.AddSubMenu("线圈", "Drawings");
            DrawingMenu.AddGroupLabel("线圈");
            DrawingMenu.Add("DrawQ", new CheckBox("显示 Q", false));
            DrawingMenu.Add("DrawE", new CheckBox("显示 E", false));
            DrawingMenu.Add("DrawOnlyReady", new CheckBox("只显示无冷却技能"));
        }
    }
}
