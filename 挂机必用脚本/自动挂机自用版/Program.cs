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
            /*
                  foreach (Obj_AI_Base so in ObjectManager.Get<Obj_AI_Turret>().Where(so => so.Team == ObjectManager.Player.Team))
                  {
                      Chat.Print(so.Name);
                  }


            //LANES
            //Shrine_A

            //C_01_A = torre do nexus
            //L_02_A = T2 do top
            //L_03_A = t1 do top
            //C_06_A = t3 do top

            //R_02_A = t2 do bot
            //R_03_A = t1 do bot
            //C_07_A = t3 do bot

            foreach (Obj_AI_Base so in ObjectManager.Get<Obj_AI_Turret>().Where(so => so.Name.EndsWith("R_03_A") && so.Team == ObjectManager.Player.Team))
            {
                Player.IssueOrder(GameObjectOrder.MoveTo, so.Position);
            }
            */
            createFS();
            Chat.Print("Welcome to Auto Buddy Treeline.");
            Core.DelayAction(Start, 3000);
            menu = MainMenu.AddMenu("扭曲丛林", "AB");
            menu.AddGroupLabel("预设");
            CheckBox c =
                new CheckBox("喊下路，如果下路有人则离开去别路 (自动选路必开)", true);

            PropertyInfo property2 = typeof(CheckBox).GetProperty("Size");
            property2.GetSetMethod(true).Invoke(c, new object[] { new Vector2(500, 20) });
            menu.Add("bot", c);

            Slider sliderLanes = menu.Add("路线", new Slider(" ", 1, 1, 2));
            string[] lanes =
            {
                "", "Selected lane: 上", "Selected lane: 下"
            };
            sliderLanes.DisplayName = lanes[sliderLanes.CurrentValue];
            sliderLanes.OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
            {
                sender.DisplayName = lanes[changeArgs.NewValue];
            };

            menu.Add("disablepings", new CheckBox("屏蔽信号", false));
            menu.Add("disablechat", new CheckBox("屏蔽信号聊天", false));

            menu.AddSeparator(5);
            menu.AddGroupLabel("投降");
            Slider sliderFF = menu.Add("ff", new Slider(" ", 2, 1, 2));
            string[] ffStrings =
            {
                "", "Surrender Vote: 投降", "Surrender Vote: 不投降"
            };
            sliderFF.DisplayName = ffStrings[sliderFF.CurrentValue];
            sliderFF.OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
            {
                sender.DisplayName = ffStrings[changeArgs.NewValue];
            };

            menu.AddSeparator(5);
            menu.AddGroupLabel("杂项");
            menu.Add("lockchat", new CheckBox("锁定聊天", true));
            CheckBox newpf = new CheckBox("智能寻路", true);
            menu.Add("newPF", newpf);
            newpf.OnValueChange += newpf_OnValueChange;
            menu.Add("reselectlane", new CheckBox("重新选择路线", false));

            menu.AddLabel("----------------------------");
            menu.Add("autoclose", new CheckBox("自动关闭游戏 (按F5生效)", true));
            menu.Add("oldWalk", new CheckBox("使用老版走砍 (按F5生效)", false));
            menu.Add("debuginfo", new CheckBox("显示调试信息", false));
            menu.Add("l1", new Label("推荐打开智能寻路以及重新选择路线！其他可以无视"));

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
            Environment.SpecialFolder.ApplicationData), "EloBuddy\\AutoBuddyTreeline\\Skills"));
            if (!generic)
            {
                BuildCreator bc = new BuildCreator(menu, Path.Combine(Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData), "EloBuddy\\AutoBuddyTreeline\\Builds"), myChamp.ShopSequence);
            }


            else
            {
                myChamp = new Generic();
                if (MainMenu.GetMenu("AB_" + ObjectManager.Player.ChampionName) != null &&
                    MainMenu.GetMenu("AB_" + ObjectManager.Player.ChampionName).Get<Label>("shopSequence") != null)
                {
                    Chat.Print("Auto Buddy Plus: Loaded shop plugin for " + ObjectManager.Player.ChampionName);
                    BuildCreator bc = new BuildCreator(menu, Path.Combine(Environment.GetFolderPath(
                        Environment.SpecialFolder.ApplicationData), "EloBuddy\\AutoBuddyTreeline\\Builds"),
                        MainMenu.GetMenu("AB_" + ObjectManager.Player.ChampionName)
                            .Get<Label>("shopSequence")
                            .DisplayName);
                }
                else
                {
                    BuildCreator bc = new BuildCreator(menu, Path.Combine(Environment.GetFolderPath(
                        Environment.SpecialFolder.ApplicationData), "EloBuddy\\AutoBuddyTreeline\\Builds"), myChamp.ShopSequence);
                }
            }

            Logic = new LogicSelector(myChamp, menu);
        }

        private static void createFS()
        {
            Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData), "EloBuddy\\AutoBuddyTreeline"));
            Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData), "EloBuddy\\AutoBuddyTreeline\\Builds"));
            Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData), "EloBuddy\\AutoBuddyTreeline\\Skills"));
        }
    }
}