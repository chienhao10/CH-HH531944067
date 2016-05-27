using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Reflection;
using AutoBuddy.Humanizers;
using AutoBuddy.MainLogics;
using AutoBuddy.MyChampLogic;
using AutoBuddy.Utilities;
using AutoBuddy.Utilities.AutoLvl;
using AutoBuddy.Utilities.AutoShop;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using Version = System.Version;

namespace AutoBuddy
{
    internal static class Program
    {
        private static Menu menu;
        private static IChampLogic myChamp;
        private static LogicSelector Logic { get; set; }
        static List<string> _allowed = new List<string> { "/noff", "/ff", "/mute all", "/msg", "/r", "/w", "/surrender", "/nosurrender", "/help", "/dance", "/d", "/taunt", "/t", "/joke", "/j", "/laugh", "/l" };

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            createFS();
            Chat.Print("Welcome to Auto Buddy Plus.");
            Core.DelayAction(Start, 3000);
            menu = MainMenu.AddMenu("CH自动挂机", "AB");
            menu.AddGroupLabel("Default");
            CheckBox c =
                new CheckBox("喊中路（有其他玩家则选其他路）", true);

            PropertyInfo property2 = typeof(CheckBox).GetProperty("Size");
            property2.GetSetMethod(true).Invoke(c, new object[] { new Vector2(500, 20) });
            menu.Add("mid", c);

            Slider sliderLanes = menu.Add("lane", new Slider(" ", 1, 1, 4));
            string[] lanes =
            {
                "", "选择 线路: 自动", "选择 线路: 上单", "选择 线路: 中单",
                "选择 线路: 下路"
            };
            sliderLanes.DisplayName = lanes[sliderLanes.CurrentValue];
            sliderLanes.OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
            {
                sender.DisplayName = lanes[changeArgs.NewValue];
            };

            menu.Add("disablepings", new CheckBox("屏蔽信号", false));
            menu.Add("disablechat", new CheckBox("屏蔽聊天", false));

            menu.AddSeparator(5);
            menu.AddGroupLabel("投降");
            Slider sliderFF = menu.Add("ff", new Slider(" ", 2, 1, 2));
            string[] ffStrings =
            {
                "", "投降选择: Yes", "投降选择: No"
            };
            sliderFF.DisplayName = ffStrings[sliderFF.CurrentValue];
            sliderFF.OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
            {
                sender.DisplayName = ffStrings[changeArgs.NewValue];
            };

            menu.AddSeparator(5);
            menu.AddGroupLabel("其他");
            menu.Add("lockchat", new CheckBox("锁定聊天", true));
            CheckBox newpf = new CheckBox("智能选择路线", true);
            menu.Add("newPF", newpf);
            newpf.OnValueChange += newpf_OnValueChange;
            menu.Add("reselectlane", new CheckBox("重新选择路线", false));

            menu.AddLabel("----------------------------");
            menu.Add("autoclose", new CheckBox("自动关闭游戏 (需F5)", true));
            menu.Add("oldWalk", new CheckBox("使用老版走砍 (需F5)", false));
            menu.Add("debuginfo", new CheckBox("显示调试信息", false));
            menu.Add("l1", new Label("贩卖者退散！支持自定义装扮，详情请看我主页"));

            Chat.OnInput += Chat_OnInput;
        }


        private static void Chat_OnInput(ChatInputEventArgs args)
        {
            if (MainMenu.GetMenu("AB").Get<CheckBox>("lockchat").CurrentValue)
            {
                args.Process = false;
                if (_allowed.Any(str => args.Input.StartsWith(str)))
                    args.Process = true;
            }
            else 
            {
                args.Process = true;
            }
        }

        static void newpf_OnValueChange(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
        {
            AutoWalker.newPF = args.NewValue;
        }

        private static void Start()
        {
            RandGen.Start();
            bool generic = false;
            switch (ObjectManager.Player.Hero)
            {
                case Champion.Sivir:
                    myChamp = new Sivir();
                    break;
                case Champion.Ashe:
                    myChamp = new Ashe();
                    break;
                case Champion.Caitlyn:
                    myChamp = new Caitlyn();
                    break;
                case Champion.Ezreal:
                    myChamp = new Ezreal();
                    break;
                case Champion.Jinx:
                    myChamp = new Jinx();
                    break;
                case Champion.Cassiopeia:
                    myChamp = new Cassiopeia();
                    break;
                case Champion.Vayne:
                    myChamp = new Vayne();
                    break;
                case Champion.Tristana:
                    myChamp = new Tristana();
                    break;
                default:
                    generic = true;
                    myChamp = new Generic();
                    break;
            }
            CustomLvlSeq cl = new CustomLvlSeq(menu, AutoWalker.myHero, Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.ApplicationData), "EloBuddy\\AutoBuddyPlus\\Skills"));
            if (!generic)
            {
                BuildCreator bc = new BuildCreator(menu, Path.Combine(Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData), "EloBuddy\\AutoBuddyPlus\\Builds"), myChamp.ShopSequence);
            }


            else
            {
                myChamp = new Generic();
                if (MainMenu.GetMenu("AB_" + ObjectManager.Player.ChampionName) != null &&
                    MainMenu.GetMenu("AB_" + ObjectManager.Player.ChampionName).Get<Label>("shopSequence") != null)
                {
                    Chat.Print("Auto Buddy Plus: Loaded shop plugin for " + ObjectManager.Player.ChampionName);
                    BuildCreator bc = new BuildCreator(menu, Path.Combine(Environment.GetFolderPath(
                        Environment.SpecialFolder.ApplicationData), "EloBuddy\\AutoBuddyPlus\\Builds"),
                        MainMenu.GetMenu("AB_" + ObjectManager.Player.ChampionName)
                            .Get<Label>("shopSequence")
                            .DisplayName);
                }
                else
                {
                    BuildCreator bc = new BuildCreator(menu, Path.Combine(Environment.GetFolderPath(
                        Environment.SpecialFolder.ApplicationData), "EloBuddy\\AutoBuddyPlus\\Builds"), myChamp.ShopSequence);
                }
            }

            Logic = new LogicSelector(myChamp, menu);
        }

        private static void createFS()
        {
            Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData), "EloBuddy\\AutoBuddyPlus"));
            Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData), "EloBuddy\\AutoBuddyPlus\\Builds"));
            Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData), "EloBuddy\\AutoBuddyPlus\\Skills"));
        }
    }
}