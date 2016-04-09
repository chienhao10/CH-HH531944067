
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AkaYasuo;
using EloBuddy.SDK.Menu;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace AkaYasuo
{
    internal class MenuManager
    {
        public static Menu YMenu,
            ComboMenu,
            HarassMenu,
            LaneClearMenu,
            LastHitMenu,
            JungleClearMenu,
            MiscMenu,
            FleeMenu,
            KillStealMenu,
            DrawingMenu,
            DogeMenu,
            ItemMenu;

        public static string[] gapcloser;
        public static string[] interrupt;
        public static string[] notarget;
        public static Dictionary<string, Menu> SubMenu = new Dictionary<string, Menu>() { };

        public static void Load()
        {
            Mainmenu();
            Combomenu();
            Harassmenu();
            Fleemenu();
            LaneClearmenu();
            LastHitmenu();
            JungleClearmenu();
            Miscmenu();
            KillStealmenu();
            Drawingmenu();
            Dogemenu();
            Itemmenu();
        }

        public static void Mainmenu()
        {
            YMenu = MainMenu.AddMenu("Aka亚索", "akasyasuo");
            YMenu.AddGroupLabel("欢迎使用AKA亚索，由CH汉化:)");
        }

        public static void Combomenu()
        {
            ComboMenu = YMenu.AddSubMenu("连招", "Combo");
            ComboMenu.AddGroupLabel("连招");
            ComboMenu.Add("Q", new CheckBox("使用Q"));
            ComboMenu.Add("EC", new CheckBox("使用E"));
            ComboMenu.Add("EQ", new CheckBox("使用EQ"));
            ComboMenu.Add("EGap", new CheckBox("使用E 间距"));
            ComboMenu.Add("EGaps", new Slider("敌人范围不在则使用E 接近敌人", 300, 1, 475));
            ComboMenu.Add("EGapTower", new CheckBox("间距 塔范围?", false));
            ComboMenu.Add("StackQ", new CheckBox("进行间距时进行Q叠加"));
            ComboMenu.Add("R", new CheckBox("使用R"));
            ComboMenu.Add("Ignite", new CheckBox("使用点燃"));
            ComboMenu.AddGroupLabel("R 连招设定");
            foreach (var hero in EntityManager.Heroes.Enemies.Where(x => x.IsEnemy))
            {
                ComboMenu.Add(hero.ChampionName, new CheckBox("以下敌人使用R " + hero.ChampionName));
            }
            ComboMenu.AddSeparator();
            ComboMenu.Add("R4", new CheckBox("当附近友军 >= 1 立刻使用R"));
            ComboMenu.Add("R2", new Slider("使用R当敌人血量 <=", 50, 0, 101));
            ComboMenu.Add("R3", new Slider("X 数量敌人被击飞使用R", 2, 0, 5));
            ComboMenu.AddGroupLabel("自动R设定");
            ComboMenu.Add("AutoR", new CheckBox("自动使用 R"));
            ComboMenu.Add("AutoR2", new Slider("X 数量敌人被击飞时", 3, 0, 5));
            ComboMenu.Add("AutoR2HP", new Slider("自身生命 >=", 101, 0, 101));
            ComboMenu.Add("AutoR2Enemies", new Slider("范围内的敌人数量 <=", 2, 0, 5));
        }

        public static void Harassmenu()
        {
            HarassMenu = YMenu.AddSubMenu("骚扰", "Harass");
            HarassMenu.AddGroupLabel("自动骚扰");
            HarassMenu.Add("AutoQ", new KeyBind("自动Q切换开关", true, KeyBind.BindTypes.PressToggle, 'T'));
            HarassMenu.Add("AutoQ3", new CheckBox("自动 Q3"));
            HarassMenu.Add("QTower", new CheckBox("塔下自动Q"));
            HarassMenu.AddGroupLabel("骚扰");
            HarassMenu.Add("Q", new CheckBox("使用Q"));
            HarassMenu.Add("Q3", new CheckBox("使用Q3"));
            HarassMenu.Add("QLastHit", new CheckBox("Q 尾兵?"));
        }

        public static void Fleemenu()
        {
            FleeMenu = YMenu.AddSubMenu("逃跑", "Flee");
            FleeMenu.AddGroupLabel("逃跑");
            FleeMenu.Add("EscQ", new CheckBox("叠加 Q"));
            FleeMenu.Add("EscE", new CheckBox("使用 E"));
            FleeMenu.Add("WJ", new KeyBind("逃跑时进行E过墙", false, KeyBind.BindTypes.HoldActive, 'G'));
        }

        public static void LaneClearmenu()
        {
            LaneClearMenu = YMenu.AddSubMenu("清线", "LaneClear");
            LaneClearMenu.AddGroupLabel("清线");
            LaneClearMenu.Add("Q", new CheckBox("使用 Q"));
            LaneClearMenu.Add("Q3", new CheckBox("使用 Q3"));
            LaneClearMenu.Add("E", new CheckBox("使用 E"));
            LaneClearMenu.Add("Items", new CheckBox("使用物品"));
        }

        public static void JungleClearmenu()
        {
            JungleClearMenu = YMenu.AddSubMenu("清野", "JungleClear");
            JungleClearMenu.AddGroupLabel("清野");
            JungleClearMenu.Add("Q", new CheckBox("使用 Q"));
            JungleClearMenu.Add("E", new CheckBox("使用 E"));
            JungleClearMenu.Add("Items", new CheckBox("使用物品"));
        }

        public static void LastHitmenu()
        {
            LastHitMenu = YMenu.AddSubMenu("尾兵", "LastHit");
            LastHitMenu.AddGroupLabel("尾兵");
            LastHitMenu.Add("Q", new CheckBox("使用 Q"));
            LastHitMenu.Add("Q3", new CheckBox("使用 Q3"));
            LastHitMenu.Add("E", new CheckBox("使用 E"));
        }

        public static void KillStealmenu()
        {
            KillStealMenu = YMenu.AddSubMenu("抢头", "KillSteal");
            KillStealMenu.AddGroupLabel("抢头");
            KillStealMenu.Add("KsQ", new CheckBox("使用 Q"));
            KillStealMenu.Add("KsE", new CheckBox("使用 E"));
            KillStealMenu.Add("KsIgnite", new CheckBox("使用点燃"));
        }

        public static void Miscmenu()
        {
            MiscMenu = YMenu.AddSubMenu("杂项", "Misc");
            MiscMenu.AddGroupLabel("杂项");
            MiscMenu.Add("StackQ", new CheckBox("叠加 Q"));
            MiscMenu.Add("InterruptQ", new CheckBox("使用 Q3进行技能打断"));
            MiscMenu.Add("noEturret", new CheckBox("不E向敌方塔下"));
            MiscMenu.AddSeparator();
            MiscMenu.AddLabel("1: Q 2: E");
            MiscMenu.Add("autolvl", new CheckBox("自动加点"));
            MiscMenu.Add("autolvls", new Slider("加点模式", 1, 1, 2));
            switch (MiscMenu["autolvls"].Cast<Slider>().CurrentValue)
            {
                case 1:
                    Variables.abilitySequence = new[] { 1, 3, 2, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
                    break;
                case 2:
                    Variables.abilitySequence = new[] { 3, 1, 2, 3, 3, 4, 3, 1, 3, 1, 4, 1, 1, 2, 2, 4, 2, 2 };
                    break;
            }
            var skin = MiscMenu.Add("sID", new Slider("Skin", 0, 0, 2));
            var sID = new[] { "Classic", "High-Noon Yasuo", "Project Yasuo" };
            skin.DisplayName = sID[skin.CurrentValue];

            skin.OnValueChange +=
                delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
                {
                    sender.DisplayName = sID[changeArgs.NewValue];
                };
        }

        public static void Drawingmenu()
        {
            DrawingMenu = YMenu.AddSubMenu("线圈", "Drawing");
            DrawingMenu.AddGroupLabel("线圈");
            DrawingMenu.Add("DrawQ", new CheckBox("显示 Q"));
            DrawingMenu.Add("DrawQ3", new CheckBox("显示 Q3"));
            DrawingMenu.Add("DrawE", new CheckBox("显示 E"));
            DrawingMenu.Add("DrawR", new CheckBox("显示 R"));
            DrawingMenu.Add("DrawSpots", new CheckBox("显示可跳墙点"));
        }

        public static void Dogemenu()
        {
            if (EntityManager.Heroes.Enemies.Any())
            {
                EvadeManager.EvadeSkillshot.Init();
                EvadeManager.EvadeTarget.Init();
            }
        }

        public static void Itemmenu()
        {
            ItemMenu = YMenu.AddSubMenu("物品", "QSS");
            ItemMenu.AddGroupLabel("进攻类物品");
            ItemMenu.Add("Items", new CheckBox("使用物品"));
            ItemMenu.Add("myhp", new Slider("使用破败当我的生命 <=", 70, 0, 101));
            ItemMenu.AddGroupLabel("Qss");
            ItemMenu.Add("use", new KeyBind("使用净化/水银", true, KeyBind.BindTypes.PressToggle, 'K'));
            ItemMenu.Add("delay", new Slider("激活延迟", 1000, 0, 2000));
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
                new CheckBox("定身"));
            ItemMenu.Add("Silence",
                new CheckBox("沉默", false));
            ItemMenu.Add("Taunt",
                new CheckBox("嘲讽"));
            ItemMenu.Add("Suppression",
                new CheckBox("压制（螃蟹，蝎子，蚂蚱，狼人那类R）"));
        }
    }
}

