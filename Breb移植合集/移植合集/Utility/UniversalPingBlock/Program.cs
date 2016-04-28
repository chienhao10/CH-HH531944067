using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace UniversalPings
{
    internal class Program
    {
        private static Program _instance;
        private readonly IList<Ping> _pings = new List<Ping>();
        private Menu _menu;

        private static void Main(string[] args)
        {
            _instance = new Program();
        }

        public Program()
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        ~Program()
        {
            Game.OnUpdate -= GameOnOnGameUpdate;
            Drawing.OnEndScene -= Drawing_OnEndScene;
            Drawing.OnPreReset -= Drawing_OnPreReset;
            Drawing.OnPostReset -= Drawing_OnPostReset;
        }

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        private void Game_OnGameLoad(EventArgs args)
        {
            _menu = MainMenu.AddMenu("Universal Pings", "UniversalPings");
            _menu.Add("print", new ComboBox("Show", 0, "Champion", "Player", "Both"));
            _menu.AddGroupLabel("[Block Settings]");
            foreach (AIHeroClient hero in ObjectManager.Get<AIHeroClient>())
            {
                if (hero.Team == ObjectManager.Player.Team && hero.NetworkId != ObjectManager.Player.NetworkId)
                {
                    _menu.Add(hero.Name, new CheckBox(hero.Name + " (" + hero.ChampionName + ")", false));
                }
            }

            EloBuddy.TacticalMap.OnPing += Game_OnPing;
            Game.OnUpdate += GameOnOnGameUpdate;
            Drawing.OnEndScene += Drawing_OnEndScene;
            Drawing.OnPreReset += Drawing_OnPreReset;
            Drawing.OnPostReset += Drawing_OnPostReset;
            Print("Loaded!");
        }

        private void Game_OnPing(TacticalMapPingEventArgs args)
        {
            try
            {
                    GameObject srcObject = args.Source;
                    AIHeroClient src = srcObject as AIHeroClient;
                    if (src == null)
                    {
                        return;
                    }

                    if (_menu[src.Name] != null && getCheckBoxItem(_menu, src.Name))
                    {
                        args.Process = false;
                        return;
                    }

                    GameObject target = args.Target;

                    if (args.PingType == PingCategory.OnMyWay || !src.IsValid) return;
                    Color c = Color.White;
                    switch (args.PingType)
                    {
                        case PingCategory.Fallback:
                        case PingCategory.EnemyMissing:
                            c = Color.LightYellow;
                            break;
                        case PingCategory.AssistMe:
                            c = Color.LightBlue;
                            break;
                        case PingCategory.Danger:
                            c = new Color(255, 204, 203);
                            break;
                    }

                    int selectedIndex = getBoxItem(_menu, "print");
                    String name;
                    switch (selectedIndex)
                    {
                        case 0:
                            name = src.ChampionName;
                            break;
                        case 1:
                            name = src.Name;
                            break;
                        default:
                            name = src.Name + " (" + src.ChampionName + ")";
                            break;
                    }
                    _pings.Add(new Ping(name, Game.Time + 2f, args.Position.X, args.Position.Y, target, c));
                }
          
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void Drawing_OnPostReset(EventArgs args)
        {
            foreach (Ping p in _pings)
            {
                p.OnPostReset();
            }
        }

        private void Drawing_OnPreReset(EventArgs args)
        {
            foreach (Ping p in _pings)
            {
                p.OnPreReset();
            }
        }

        private void Drawing_OnEndScene(EventArgs args)
        {
            foreach (Ping p in _pings)
            {
                p.OnEndScene();
            }
        }

        private void GameOnOnGameUpdate(EventArgs args)
        {
            for (int i = _pings.Count - 1; i >= 0; i--)
            {
                Ping p = _pings[i];
                if (Game.Time > p.End)
                {
                    p.Dispose();
                    _pings.RemoveAt(i);
                }
            }
        }

        private void Print(string msg)
        {
            Chat.Print("<font color='#ff3232'>Universal</font><font color='#d4d4d4'>Pings:</font> <font color='#FFFFFF'>" + msg + "</font>");
        }
    }

    internal class Ping : Render.Text
    {
        public Ping(String name, float end, float x, float y, GameObject target, Color c)
            : base(name, 0, 0, 20, c)
        {
            End = end;
            Target = target;
            Centered = true;
            OutLined = true;
            PositionUpdate = delegate
            {
                if (Target != null && Target.IsValid)
                {
                    return Drawing.WorldToScreen(Target.Position);
                }
                return
                    Drawing.WorldToScreen(new Vector3(x, y,
                        NavMesh.GetHeightForPosition(x, y)));
            };
        }

        public float End { get; set; }
        public GameObject Target { get; set; }
    }
}