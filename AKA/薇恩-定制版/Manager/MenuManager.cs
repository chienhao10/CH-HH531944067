using System;
using EloBuddy.SDK;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto_Carry_Vayne.Manager
{
    class MenuManager
    {
        private static Menu VMenu,
            Hotkeymenu,
    ComboMenu,
    CondemnMenu,
    HarassMenu,
    LaneClearMenu,
    JungleClearMenu,
    FleeMenu,
    MiscMenu,
    ItemMenu,
    DrawingMenu;

        public static void Load()
        {
            Mainmenu();
            Hotkeys();
            PackageLoader();
            if (Variables.Combo == true) Combomenu();
            if (Variables.Condemn == true) Condemnmenu();
            if (Variables.Harass == true) Harassmenu();
            if (Variables.LC == true) LaneClearmenu();
            if (Variables.JC == true) JungleClearmenu();
            if (Variables.Flee == true) Fleemenu();
            if (Variables.Misc == true) Miscmenu();
            if (Variables.Activator == true) Activator();
            if (Variables.Draw == true) Drawingmenu();
        }

        private static void Mainmenu()
        {
            VMenu = MainMenu.AddMenu("AKA薇恩-自由版", "akavayne");
            VMenu.AddGroupLabel("Made by Aka.");
            VMenu.AddGroupLabel("CH汉化.");
            VMenu.AddSeparator();
        }

        private static void PackageLoader()
        {
            VMenu.AddGroupLabel("功能加载器");
            VMenu.AddLabel("'想自己加载哪一些功能就在以下选择！比如开了活化剂就不要再开这边的了'");
            VMenu.AddLabel("-选择好后按下F5重新加载脚本!-");
            VMenu.Add("Combo", new CheckBox("加载连招选项", false));
            VMenu.Add("Condemn", new CheckBox("加载定墙选项", false));
            VMenu.Add("Harass", new CheckBox("加载骚扰选项", false));
            VMenu.Add("Flee", new CheckBox("加载逃跑选项", false));
            VMenu.Add("LC", new CheckBox("加载清线选项", false));
            VMenu.Add("JC", new CheckBox("加载清野选项", false));
            VMenu.Add("Misc", new CheckBox("加载杂项设置", false));
            VMenu.Add("Activator", new CheckBox("加载活化剂选项", false));
            VMenu.Add("Drawing", new CheckBox("加载线圈选项", false));
            #region CheckMenu

            //Combo
            if (VMenu["Combo"].Cast<CheckBox>().CurrentValue)
            {
                Variables.Combo = true;
            }
            //Condemn
            if (VMenu["Condemn"].Cast<CheckBox>().CurrentValue)
            {
                Variables.Condemn = true;
            }
            //Harass
            if (VMenu["Harass"].Cast<CheckBox>().CurrentValue)
            {
                Variables.Harass = true;
            }
            //Flee
            if (VMenu["Flee"].Cast<CheckBox>().CurrentValue)
            {
                Variables.Flee = true;
            }
            //LC
            if (VMenu["LC"].Cast<CheckBox>().CurrentValue)
            {
                Variables.LC = true;
            }
            //JC
            if (VMenu["JC"].Cast<CheckBox>().CurrentValue)
            {
                Variables.JC = true;
            }
            //Misc
            if (VMenu["Misc"].Cast<CheckBox>().CurrentValue)
            {
                Variables.Misc = true;
            }
            //Activator
            if (VMenu["Activator"].Cast<CheckBox>().CurrentValue)
            {
                Variables.Activator = true;
            }
            //Drawing
            if (VMenu["Drawing"].Cast<CheckBox>().CurrentValue)
            {
                Variables.Draw = true;
            }

            #endregion CheckMenu
        }

        private static void Hotkeys()
        {
            Hotkeymenu = VMenu.AddSubMenu("快捷键", "Hotkeys");
            Hotkeymenu.Add("flashe", new KeyBind("闪现定墙!", false, KeyBind.BindTypes.HoldActive, 'Y'));
            Hotkeymenu.Add("insece", new KeyBind("闪现 推人!", false, KeyBind.BindTypes.HoldActive, 'Z'));
            Hotkeymenu.Add("rote", new KeyBind("传送门定墙!", false, KeyBind.BindTypes.HoldActive, 'N'));
            Hotkeymenu.Add("insecmodes", new ComboBox("推人模式", 0, "至友军", "至塔下", "至鼠标"));
            Hotkeymenu.Add("RnoAA", new KeyBind("隐身时不普攻", false, KeyBind.BindTypes.PressToggle, 'T'));
            Hotkeymenu.Add("RnoAAif", new Slider("当附近 X 敌人隐身时不普攻 >= enemy in range", 2, 0, 5));
        }

        private static void Combomenu()
        {
            ComboMenu = VMenu.AddSubMenu("连招", "Combo");
            ComboMenu.Add("UseQ", new CheckBox("使用 Q"));
            ComboMenu.Add("UseQE", new CheckBox("尝试QE"));
            ComboMenu.Add("UseQWall", new CheckBox("防止Q墙"));
            ComboMenu.Add("UseQEnemies", new Slider("防止Q进 X 名敌人", 3, 5, 0));
            ComboMenu.Add("UseQStacks", new CheckBox("对2层 W 目标使用Q", false));
            ComboMenu.Add("UseQMode", new ComboBox("Q 模式", 1, "靠边", "安全距离"));
            ComboMenu.Add("UseW", new CheckBox("集火 W目标", false));
            ComboMenu.Add("UseE", new CheckBox("使用 E"));
            ComboMenu.Add("UseEKill", new CheckBox("使用E杀死敌方?"));
            ComboMenu.Add("UseR", new CheckBox("使用 R", false));
            ComboMenu.Add("UseRif", new Slider("使用 R 如果敌人数量为 X", 2, 1, 5));
            ComboMenu.Add("RnoAA", new CheckBox("隐身时不普攻", false));
            ComboMenu.Add("RnoAAif", new Slider("当附近敌人数量为 X 时，隐身不普攻", 2, 0, 5));

        }

        private static void Condemnmenu()
        {
            CondemnMenu = VMenu.AddSubMenu("定墙", "Condemn");
            CondemnMenu.Add("UseEAuto", new CheckBox("自动 E"));
            CondemnMenu.Add("UseETarget", new CheckBox("只定墙当前目标?", false));
            CondemnMenu.Add("UseEHitchance", new Slider("定墙命中率", 2, 1, 4));
            CondemnMenu.Add("UseEPush", new Slider("E 推行距离", 420, 350, 470));
            CondemnMenu.Add("UseEAA", new Slider("不使用E 当目标能被 X 普攻杀死", 0, 0, 4));
            CondemnMenu.Add("AutoTrinket", new CheckBox("自动草丛插眼?"));
            CondemnMenu.Add("J4Flag", new CheckBox("E 皇子旗?"));
        }

        private static void Harassmenu()
        {
            HarassMenu = VMenu.AddSubMenu("骚扰", "Harass");
            HarassMenu.Add("HarassCombo", new CheckBox("骚扰连招"));
            HarassMenu.Add("HarassMana", new Slider("骚扰连招蓝量使用", 40));
        }

        private static void LaneClearmenu()
        {
            LaneClearMenu = VMenu.AddSubMenu("清线", "LaneClear");
            LaneClearMenu.Add("UseQ", new CheckBox("使用 Q"));
            LaneClearMenu.Add("UseQMana", new Slider("最大蓝量使用百分比({0}%)", 40));
        }

        private static void JungleClearmenu()
        {
            JungleClearMenu = VMenu.AddSubMenu("清野", "JungleClear");
            JungleClearMenu.Add("UseQ", new CheckBox("使用 Q"));
            JungleClearMenu.Add("UseE", new CheckBox("使用 E"));
        }

        private static void Fleemenu()
        {
            FleeMenu = VMenu.AddSubMenu("逃跑", "Flee");
            FleeMenu.Add("UseQ", new CheckBox("使用 Q"));
            FleeMenu.Add("UseE", new CheckBox("使用 E"));
        }

        private static void Miscmenu()
        {
            MiscMenu = VMenu.AddSubMenu("杂项", "Misc");
            MiscMenu.AddGroupLabel("杂项");
            MiscMenu.Add("GapcloseQ", new CheckBox("Q防突进"));
            MiscMenu.Add("GapcloseE", new CheckBox("E防突进"));
            MiscMenu.Add("InterruptE", new CheckBox("E技能打断"));
            MiscMenu.Add("LowLifeE", new CheckBox("低生命 E", false));
            MiscMenu.Add("LowLifeES", new Slider("生命低于 X，使用E", 20));
            MiscMenu.AddGroupLabel("通用");
            MiscMenu.Add("Skinhack", new CheckBox("开启换肤", false));
            MiscMenu.Add("SkinId", new ComboBox("换肤", 0, "初始", "摩登骇客", "猎天使魔女", "巨龙追猎者", "觅心猎手", "SKT T1", "苍穹之光", "绿色包", "红色包", "灰色包"));
            MiscMenu.Add("Autolvl", new CheckBox("自动加点"));
            MiscMenu.Add("AutolvlS", new ComboBox("加点模式", 0, "主 W", "主 Q(我喜欢)"));
            MiscMenu.Add("Autobuy", new CheckBox("开局自动买装备"));
            MiscMenu.Add("Autobuyt", new CheckBox("自动买饰品", false));
        }

        private static void Activator()
        {
            ItemMenu = VMenu.AddSubMenu("活化剂", "Activator");
            ItemMenu.AddGroupLabel("物品");
            ItemMenu.AddLabel("私聊我如果需要添加物品.");
            ItemMenu.Add("Botrk", new CheckBox("使用破败/弯刀"));
            ItemMenu.Add("You", new CheckBox("使用 幽梦"));
            ItemMenu.Add("YouS", new Slider("使用 幽梦如果范围 =>", 500, 0, 1000));
            ItemMenu.Add("AutoPotion", new CheckBox("自动吃药"));
            ItemMenu.Add("AutoPotionHp", new Slider("生命值低于 X", 60));
            ItemMenu.Add("AutoBiscuit", new CheckBox("自动饼干"));
            ItemMenu.Add("AutoBiscuitHp", new Slider("生命值低于 X", 60));
            ItemMenu.AddGroupLabel("召唤师技能");
            ItemMenu.AddLabel("私聊我如果需要添加技能.");
            ItemMenu.Add("Heal", new CheckBox("治疗"));
            ItemMenu.Add("HealHp", new Slider("生命值低于", 20, 0, 100));
            ItemMenu.Add("HealAlly", new CheckBox("治疗友军"));
            ItemMenu.Add("HealAllyHp", new Slider("友军生命值低于", 20, 0, 100));
            ItemMenu.Add("Barrier", new CheckBox("使用护盾"));
            ItemMenu.Add("BarrierHp", new Slider("使用护盾如果血量 <=", 20, 0, 100));
            ItemMenu.AddGroupLabel("解控");
            ItemMenu.Add("Qss", new CheckBox("使用水银"));
            ItemMenu.Add("QssDelay", new Slider("延迟", 100, 0, 2000));
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

        private static void Drawingmenu()
        {
            DrawingMenu = VMenu.AddSubMenu("线圈", "Drawings");
            DrawingMenu.Add("DrawQ", new CheckBox("显示 Q", false));
            DrawingMenu.Add("DrawE", new CheckBox("显示 E", false));
            DrawingMenu.Add("DrawCondemn", new CheckBox("显示定墙"));
            DrawingMenu.Add("DrawOnlyReady", new CheckBox("只显示无冷却技能"));
        }

        #region checkvalues
        #region checkvalues:hotkeys
        public static bool FlashE
        {
            get { return (Hotkeymenu["flashe"].Cast<KeyBind>().CurrentValue); }
        }
        public static bool InsecE
        {
            get { return (Hotkeymenu["insece"].Cast<KeyBind>().CurrentValue); }
        }
        public static bool RotE
        {
            get { return (Hotkeymenu["rote"].Cast<KeyBind>().CurrentValue); }
        }
        public static int InsecPositions
        {
            get { return (Hotkeymenu["insecmodes"].Cast<ComboBox>().CurrentValue); }
        }
        public static bool RNoAA
        {
            get { return (Hotkeymenu["RnoAA"].Cast<KeyBind>().CurrentValue); }
        }
        public static int RNoAASlider
        {
            get { return (Hotkeymenu["RnoAAif"].Cast<Slider>().CurrentValue); }
        }
        #endregion checkvalues:hotkeys
        #region checkvalues:Combo
        public static bool UseQ
        {
            get { return (VMenu["Combo"].Cast<CheckBox>().CurrentValue ? ComboMenu["UseQ"].Cast<CheckBox>().CurrentValue : true); }
        }

        public static int UseQEnemies
        {
            get { return (VMenu["Combo"].Cast<CheckBox>().CurrentValue ? ComboMenu["UseQEnemies"].Cast<Slider>().CurrentValue : 3); }
        }

        public static bool UseQWall
        {
            get { return (VMenu["Combo"].Cast<CheckBox>().CurrentValue ? ComboMenu["UseQWall"].Cast<CheckBox>().CurrentValue : true); }
        }

        public static int UseQMode
        {
            get { return (VMenu["Combo"].Cast<CheckBox>().CurrentValue ? ComboMenu["UseQMode"].Cast<ComboBox>().CurrentValue : 1); }
        }

        public static bool UseQStacks
        {
            get { return (VMenu["Combo"].Cast<CheckBox>().CurrentValue ? ComboMenu["UseQStacks"].Cast<CheckBox>().CurrentValue : false); }
        }

        public static bool FocusW
        {
            get { return (VMenu["Combo"].Cast<CheckBox>().CurrentValue ? ComboMenu["UseW"].Cast<CheckBox>().CurrentValue : false); }
        }

        public static bool UseE
        {
            get { return (VMenu["Combo"].Cast<CheckBox>().CurrentValue ? ComboMenu["UseE"].Cast<CheckBox>().CurrentValue : true); }
        }

        public static bool UseEKill
        {
            get { return (VMenu["Combo"].Cast<CheckBox>().CurrentValue ? ComboMenu["UseEKill"].Cast<CheckBox>().CurrentValue : true); }
        }

        public static bool UseR
        {
            get { return (VMenu["Combo"].Cast<CheckBox>().CurrentValue ? ComboMenu["UseR"].Cast<CheckBox>().CurrentValue : false); }
        }

        public static int UseRSlider
        {
            get { return (VMenu["Combo"].Cast<CheckBox>().CurrentValue ? ComboMenu["UseRif"].Cast<Slider>().CurrentValue : 2); }
        }
        //Condemn
        #endregion checkvalues:Combo
        #region checkvalues:Condemn

        public static bool AutoE
        {
            get { return (VMenu["Condemn"].Cast<CheckBox>().CurrentValue ? CondemnMenu["UseEAuto"].Cast<CheckBox>().CurrentValue : true); }
        }

        public static bool OnlyStunCurrentTarget
        {
            get { return (VMenu["Condemn"].Cast<CheckBox>().CurrentValue ? CondemnMenu["UseETarget"].Cast<CheckBox>().CurrentValue : false); }
        }

        public static int CondemnHitchance
        {
            get { return (VMenu["Condemn"].Cast<CheckBox>().CurrentValue ? CondemnMenu["UseEHitchance"].Cast<Slider>().CurrentValue : 2); }
        }

        public static int CondemnPushDistance
        {
            get { return (VMenu["Condemn"].Cast<CheckBox>().CurrentValue ? CondemnMenu["UseEPush"].Cast<Slider>().CurrentValue : 420); }
        }

        public static int CondemnBlock
        {
            get { return (VMenu["Condemn"].Cast<CheckBox>().CurrentValue ? CondemnMenu["UseEAA"].Cast<Slider>().CurrentValue : 1); }
        }

        public static bool AutoTrinket
        {
            get { return (VMenu["Condemn"].Cast<CheckBox>().CurrentValue ? CondemnMenu["AutoTrinket"].Cast<CheckBox>().CurrentValue : true); }
        }

        public static bool J4Flag
        {
            get { return (VMenu["Condemn"].Cast<CheckBox>().CurrentValue ? CondemnMenu["J4Flag"].Cast<CheckBox>().CurrentValue : true); }
        }

        #endregion checkvalues:Condemn
        #region checkvalues:Harass

        public static bool HarassCombo
        {
            get { return (VMenu["Harass"].Cast<CheckBox>().CurrentValue ? HarassMenu["HarassCombo"].Cast<CheckBox>().CurrentValue : true); }
        }

        public static int HarassMana
        {
            get { return (VMenu["Harass"].Cast<CheckBox>().CurrentValue ? HarassMenu["HarassMana"].Cast<Slider>().CurrentValue : 0); }
        }


        #endregion checkvalues:Harass
        #region checkvalues:LC

        public static bool UseQLC
        {
            get { return (VMenu["LC"].Cast<CheckBox>().CurrentValue ? LaneClearMenu["UseQ"].Cast<CheckBox>().CurrentValue : true); }
        }

        public static int UseQLCMana
        {
            get { return (VMenu["LC"].Cast<CheckBox>().CurrentValue ? LaneClearMenu["UseQMana"].Cast<Slider>().CurrentValue : 40); }
        }


        #endregion checkvalues:LC
        #region checkvalues:JC

        public static bool UseQJC
        {
            get { return (VMenu["JC"].Cast<CheckBox>().CurrentValue ? JungleClearMenu["UseQ"].Cast<CheckBox>().CurrentValue : true); }
        }

        public static bool UseEJC
        {
            get { return (VMenu["JC"].Cast<CheckBox>().CurrentValue ? JungleClearMenu["UseE"].Cast<CheckBox>().CurrentValue : true); }
        }

        #endregion checkvalues:JC
        #region checkvalues:Flee
        public static bool UseQFlee
        {
            get { return (VMenu["Flee"].Cast<CheckBox>().CurrentValue ? FleeMenu["UseQ"].Cast<CheckBox>().CurrentValue : true); }
        }

        public static bool UseEFlee
        {
            get { return (VMenu["Flee"].Cast<CheckBox>().CurrentValue ? FleeMenu["UseE"].Cast<CheckBox>().CurrentValue : true); }
        }

        #endregion checkvalues:Flee
        #region checkvalues:Misc

        public static bool GapcloseQ
        {
            get { return (VMenu["Misc"].Cast<CheckBox>().CurrentValue ? MiscMenu["GapcloseQ"].Cast<CheckBox>().CurrentValue : true); }
        }

        public static bool GapcloseE
        {
            get { return (VMenu["Misc"].Cast<CheckBox>().CurrentValue ? MiscMenu["GapcloseE"].Cast<CheckBox>().CurrentValue : true); }
        }

        public static bool InterruptE
        {
            get { return (VMenu["Misc"].Cast<CheckBox>().CurrentValue ? MiscMenu["InterruptE"].Cast<CheckBox>().CurrentValue : true); }
        }

        public static bool LowLifeE
        {
            get { return (VMenu["Misc"].Cast<CheckBox>().CurrentValue ? MiscMenu["LowLifeE"].Cast<CheckBox>().CurrentValue : false); }
        }

        public static int LowLifeESlider
        {
            get { return (VMenu["Misc"].Cast<CheckBox>().CurrentValue ? MiscMenu["LowLifeES"].Cast<Slider>().CurrentValue : 20); }
        }

        public static bool Skinhack
        {
            get { return (VMenu["Misc"].Cast<CheckBox>().CurrentValue ? MiscMenu["Skinhack"].Cast<CheckBox>().CurrentValue : false); }
        }

        public static int SkinId
        {
            get { return (VMenu["Misc"].Cast<CheckBox>().CurrentValue ? MiscMenu["SkinId"].Cast<ComboBox>().CurrentValue : 0); }
        }

        public static bool Autolvl
        {
            get { return (VMenu["Misc"].Cast<CheckBox>().CurrentValue ? MiscMenu["Autolvl"].Cast<CheckBox>().CurrentValue : true); }
        }

        public static int AutolvlSlider
        {
            get { return (VMenu["Misc"].Cast<CheckBox>().CurrentValue ? MiscMenu["AutolvlS"].Cast<ComboBox>().CurrentValue : 0); }
        }

        public static bool AutobuyStarters
        {
            get { return (VMenu["Misc"].Cast<CheckBox>().CurrentValue ? MiscMenu["Autobuy"].Cast<CheckBox>().CurrentValue : true); }
        }

        public static bool AutobuyTrinkets
        {
            get { return (VMenu["Misc"].Cast<CheckBox>().CurrentValue ? MiscMenu["Autobuyt"].Cast<CheckBox>().CurrentValue : true); }
        }


        #endregion checkvalues:Misc
        #region checkvalues:Activator

        public static bool Botrk
        {
            get { return (VMenu["Activator"].Cast<CheckBox>().CurrentValue ? ItemMenu["Botrk"].Cast<CheckBox>().CurrentValue : true); }
        }

        public static bool Youmus
        {
            get { return (VMenu["Activator"].Cast<CheckBox>().CurrentValue ? ItemMenu["You"].Cast<CheckBox>().CurrentValue : true); }
        }

        public static int YoumusSlider
        {
            get { return (VMenu["Activator"].Cast<CheckBox>().CurrentValue ? ItemMenu["YouS"].Cast<Slider>().CurrentValue : 500); }
        }

        public static bool AutoPotion
        {
            get { return (VMenu["Activator"].Cast<CheckBox>().CurrentValue ? ItemMenu["AutoPotion"].Cast<CheckBox>().CurrentValue : true); }
        }

        public static bool AutoBiscuit
        {
            get { return (VMenu["Activator"].Cast<CheckBox>().CurrentValue ? ItemMenu["AutoBiscuit"].Cast<CheckBox>().CurrentValue : true); }
        }

        public static int AutoBiscuitHp
        {
            get { return (VMenu["Activator"].Cast<CheckBox>().CurrentValue ? ItemMenu["AutoBiscuitHp"].Cast<Slider>().CurrentValue : 60); }
        }

        public static int AutoPotionHp
        {
            get { return (VMenu["Activator"].Cast<CheckBox>().CurrentValue ? ItemMenu["AutoPotionHp"].Cast<Slider>().CurrentValue : 60); }
        }

        public static bool Heal
        {
            get { return (VMenu["Activator"].Cast<CheckBox>().CurrentValue ? ItemMenu["Heal"].Cast<CheckBox>().CurrentValue : true); }
        }

        public static int HealHp
        {
            get { return (VMenu["Activator"].Cast<CheckBox>().CurrentValue ? ItemMenu["HealHp"].Cast<Slider>().CurrentValue : 20); }
        }

        public static bool Barrier
        {
            get { return (VMenu["Activator"].Cast<CheckBox>().CurrentValue ? ItemMenu["Barrier"].Cast<CheckBox>().CurrentValue : true); }
        }

        public static int BarrierHp
        {
            get { return (VMenu["Activator"].Cast<CheckBox>().CurrentValue ? ItemMenu["BarrierHp"].Cast<Slider>().CurrentValue : 20); }
        }

        public static bool HealAlly
        {
            get { return (VMenu["Activator"].Cast<CheckBox>().CurrentValue ? ItemMenu["HealAlly"].Cast<CheckBox>().CurrentValue : true); }
        }

        public static int HealAllyHp
        {
            get { return (VMenu["Activator"].Cast<CheckBox>().CurrentValue ? ItemMenu["HealAllyHp"].Cast<Slider>().CurrentValue : 20); }
        }

        public static bool Qss
        {
            get { return (VMenu["Activator"].Cast<CheckBox>().CurrentValue ? ItemMenu["Qss"].Cast<CheckBox>().CurrentValue : true); }
        }

        public static int QssDelay
        {
            get { return (VMenu["Activator"].Cast<CheckBox>().CurrentValue ? ItemMenu["QssDelay"].Cast<Slider>().CurrentValue : 0); }
        }

        public static bool QssBlind
        {
            get { return (VMenu["Activator"].Cast<CheckBox>().CurrentValue ? ItemMenu["Blind"].Cast<CheckBox>().CurrentValue : false); }
        }

        public static bool QssCharm
        {
            get { return (VMenu["Activator"].Cast<CheckBox>().CurrentValue ? ItemMenu["Charm"].Cast<CheckBox>().CurrentValue : true); }
        }

        public static bool QssFear
        {
            get { return (VMenu["Activator"].Cast<CheckBox>().CurrentValue ? ItemMenu["Fear"].Cast<CheckBox>().CurrentValue : true); }
        }

        public static bool QssPolymorph
        {
            get { return (VMenu["Activator"].Cast<CheckBox>().CurrentValue ? ItemMenu["Polymorph"].Cast<CheckBox>().CurrentValue : true); }
        }

        public static bool QssStun
        {
            get { return (VMenu["Activator"].Cast<CheckBox>().CurrentValue ? ItemMenu["Stun"].Cast<CheckBox>().CurrentValue : true); }
        }

        public static bool QssSnare
        {
            get { return (VMenu["Activator"].Cast<CheckBox>().CurrentValue ? ItemMenu["Snare"].Cast<CheckBox>().CurrentValue : true); }
        }

        public static bool QssSilence
        {
            get { return (VMenu["Activator"].Cast<CheckBox>().CurrentValue ? ItemMenu["Silence"].Cast<CheckBox>().CurrentValue : false); }
        }

        public static bool QssTaunt
        {
            get { return (VMenu["Activator"].Cast<CheckBox>().CurrentValue ? ItemMenu["Taunt"].Cast<CheckBox>().CurrentValue : true); }
        }

        public static bool QssSupression
        {
            get { return (VMenu["Activator"].Cast<CheckBox>().CurrentValue ? ItemMenu["Suppression"].Cast<CheckBox>().CurrentValue : true); }
        }

        #endregion checkvalues:Activator
        #region checkvalues:Drawing
        public static bool DrawQ
        {
            get { return (VMenu["Drawing"].Cast<CheckBox>().CurrentValue ? DrawingMenu["DrawQ"].Cast<CheckBox>().CurrentValue : false); }
        }

        public static bool DrawE
        {
            get { return (VMenu["Drawing"].Cast<CheckBox>().CurrentValue ? DrawingMenu["DrawE"].Cast<CheckBox>().CurrentValue : false); }
        }

        public static bool DrawCondemn
        {
            get { return (VMenu["Drawing"].Cast<CheckBox>().CurrentValue ? DrawingMenu["DrawCondemn"].Cast<CheckBox>().CurrentValue : true); }
        }

        public static bool DrawOnlyRdy
        {
            get { return (VMenu["Drawing"].Cast<CheckBox>().CurrentValue ? DrawingMenu["DrawOnlyReady"].Cast<CheckBox>().CurrentValue : false); }
        }
        #endregion checkvalues:Drawing
        #endregion checkvalues
    }
}
