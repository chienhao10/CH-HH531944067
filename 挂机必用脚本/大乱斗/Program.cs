using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AutoSharp.Auto;
using AutoSharp.Utils;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using Color = System.Drawing.Color;
using AutoSharp.Properties;
using PortAIO.Utility;
using LeagueSharp.Common;
// ReSharper disable ObjectCreationAsStatement

namespace AutoSharp
{
    class Program
    {
        public static GameMapId Map;
        public static Menu Config;
        
        private static bool _loaded = false;
		private static Render.Sprite Intro;
        private static float IntroTimer = Game.Time;
		private static System.Drawing.Bitmap LoadImg(string imgName)
        {
            var bitmap = Resources.ResourceManager.GetObject(imgName) as System.Drawing.Bitmap;
            if (bitmap == null)
            {
                Console.WriteLine(imgName + ".png not found.");
            }
            return bitmap;
        }

        
        public static void Init()
        {
			

            //if (Loader.intro)
            //{
                Intro = new Render.Sprite(LoadImg("ASLogo"), new Vector2((Drawing.Width / 2) - 283, (Drawing.Height / 2) - 87));
                Intro.Add(0);
                Intro.OnDraw();
                LeagueSharp.Common.Utility.DelayAction.Add(5000, () => Intro.Remove());
            //}
			
            Chat.Print("AutoSharp loaded", Color.CornflowerBlue);
            Map = Game.MapId;
            //Chat.Print(Map.ToString()); // Prints Summoners Rift on Howling Abbyss
            /*
            Config = new Menu("AutoSharp: " + ObjectManager.Player.ChampionName, "autosharp." + ObjectManager.Player.ChampionName, true);
            Config.AddItem(new MenuItem("autosharp.mode", "Mode").SetValue(new StringList(new[] {"AUTO", "SBTW"}))).ValueChanged +=
                (sender, args) =>
                {
                    if (Config.Item("autosharp.mode").GetValue<StringList>().SelectedValue == "SBTW")
                    {
                        Autoplay.Load();
                    }
                    else
                    {
                        Autoplay.Unload();
                        Orbwalker.SetOrbwalkingPoint(Game.CursorPos);
                    }
                };
            Config.AddItem(new MenuItem("autosharp.humanizer", "Humanize Movement by ").SetValue(new Slider(new Random().Next(125, 350), 125, 350)));
            Config.AddItem(new MenuItem("autosharp.quit", "Quit after Game End").SetValue(true));
            Config.AddItem(new MenuItem("autosharp.shop", "AutoShop?").SetValue(true));
            var options = Config.AddSubMenu(new Menu("Options: ", "autosharp.options"));
            options.AddItem(new MenuItem("autosharp.options.healup", "Take Heals?").SetValue(true));
            options.AddItem(new MenuItem("onlyfarm", "Only Farm").SetValue(false));
            if (Map == Utility.Map.MapType.SummonersRift)
            {
                options.AddItem(new MenuItem("recallhp", "Recall if Health% <").SetValue(new Slider(30, 0, 100)));
            }
            var randomizer = Config.AddSubMenu(new Menu("Randomizer", "autosharp.randomizer"));
            var orbwalker = Config.AddSubMenu(new Menu("Orbwalker", "autosharp.orbwalker"));
            randomizer.AddItem(new MenuItem("autosharp.randomizer.minrand", "Min Rand By").SetValue(new Slider(0, 0, 90)));
            randomizer.AddItem(new MenuItem("autosharp.randomizer.maxrand", "Max Rand By").SetValue(new Slider(100, 100, 300)));
            randomizer.AddItem(new MenuItem("autosharp.randomizer.playdefensive", "Play Defensive?").SetValue(true));
            randomizer.AddItem(new MenuItem("autosharp.randomizer.auto", "Auto-Adjust? (ALPHA)").SetValue(true));
            */
            // Moved it to another addon: ChampionPlugins
            //new PluginLoader();
            
                Cache.Load(); 
                Game.OnUpdate += Positioning.OnUpdate;
                Autoplay.Load();
                Game.OnEnd += OnEnd;
                //Obj_AI_Base.OnIssueOrder += AntiShrooms;
                Game.OnUpdate += AntiShrooms2;
                Spellbook.OnCastSpell += OnCastSpell;
                Obj_AI_Base.OnDamage += OnDamage;
            
            /*
            Orbwalker = new MyOrbwalker.Orbwalker(orbwalker);
            
            Utility.DelayAction.Add(
                    new Random().Next(1000, 10000), () =>
                    {
                        new AutoSharp.Utils (Utils.AutoLevel.GetSequence().Select(num => num - 1).ToArray());
                        AutoSharp.Utils.AutoLevel.Enable();
                        Console.WriteLine("AutoLevel Init Success!");
                    });
             */
        }

        //auto agree to surrender
	    internal class Surrender
    	{
        	public Surrender()
        	{
            Obj_AI_Base.OnSurrender += delegate(Obj_AI_Base sender, Obj_AI_BaseSurrenderVoteEventArgs args)
                {
                    if (args.Type == SurrenderVoteType.Yes && sender.IsAlly)
                    {
                        Chat.Print("/ff");
                    }
                };
        }

    	}
    	
   
        public static void OnDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            if (sender == null) return;
            if (args.Target.NetworkId == ObjectManager.Player.NetworkId && (sender is Obj_AI_Turret || sender is Obj_AI_Minion))
            {
                Orbwalker.OrbwalkTo(
                    Heroes.Player.Position.Extend(Wizard.GetFarthestMinion().Position, 500).RandomizePosition());
            }
        }

        private static void AntiShrooms2(EventArgs args)
        {
           
        }

        private static void OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            
        }

        private static void OnEnd(GameEndEventArgs args)
        {
            
            Thread.Sleep(30000);
            Game.QuitGame();
        }
        
        public static void Main(string[] args)
        {
            Game.OnUpdate += AdvancedLoading;
        }

        private static void AdvancedLoading(EventArgs args)
        {
            if (!_loaded)
            {
                if (ObjectManager.Player.Gold > 0)
                {
                    _loaded = true;
                    Core.DelayAction(Init, new Random().Next(3000, 25000));
                }
            }
        }
    }
}
