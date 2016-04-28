using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using LeagueSharp.Common;
using SharpDX.Direct3D9;
using PortAIO.Properties;

namespace UniversalRecallTracker
{
    public class Program
    {
        private readonly IDictionary<AIHeroClient, RecallInfo> _recallInfo = new Dictionary<AIHeroClient, RecallInfo>();

        private static Program _instance;

        public int X
        {
            get { return getSliderItem("x"); }
        }

        public int Y
        {
            get { return getSliderItem("y"); }
        }

        public int TextSize
        {
            get { return getSliderItem("textSize"); }
        }

        public float BarScale
        {
            get { return getSliderItem("barScale") / 100f; }
        }

        public bool ChatWarning
        {
            get { return getCheckBoxItem("chatWarning"); }
        }

        public static void Main()
        {
            new Program();
        }

        private Program()
        {
            _instance = this;
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        public static bool getCheckBoxItem(string item)
        {
            return menu[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(string item)
        {
            return menu[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(string item)
        {
            return menu[item].Cast<KeyBind>().CurrentValue;
        }

        public static Program Instance()
        {
            if (_instance == null)
            {
                return new Program();
            }
            return _instance;
        }

        public static Menu menu;

        public void Game_OnGameLoad(EventArgs args)
        {
            menu = MainMenu.AddMenu("Universal RecallTracker", "universalrecalltracker");
            menu.Add("x", new Slider("X", (int)((Drawing.Direct3DDevice.Viewport.Width - Resources.RecallBar.Width) / 2f), 0, Drawing.Direct3DDevice.Viewport.Width));
            menu.Add("y", new Slider("Y", (int)(Drawing.Direct3DDevice.Viewport.Height * 3f / 4f), 0, Drawing.Direct3DDevice.Viewport.Height));
            menu.Add("textSize", new Slider("Text Size (F5 Reload)", 15, 5, 50));
            menu.Add("chatWarning", new CheckBox("Chat Notification", false));
            menu.Add("barScale", new Slider("Bar Scale %", 100, 0, 200));

            int i = 0;

            foreach (AIHeroClient hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.Team != ObjectManager.Player.Team))
            {
                RecallInfo recallInfo = new RecallInfo(hero, i++);
                _recallInfo[hero] = recallInfo;
            }

            Print("Loaded!");
        }

        public void Print(string msg, bool timer = false)
        {
            string s = null;
            if (timer)
            {
                s = "<font color='#d8d8d8'>[" + Utils.FormatTime(Game.Time) + "]</font> ";
            }
            s += "<font color='#ff3232'>Universal</font><font color='#d4d4d4'>RecallTracker:</font> <font color='#FFFFFF'>" + msg + "</font>";
            Chat.Print(s);
        }

        public void Notify(string msg)
        {
            if (ChatWarning)
            {
                Print(msg, true);
            }
        }
    }

    public class RecallInfo
    {
        public const int GapTextBar = 10;

        private static readonly Font TextFont = new Font(
            Drawing.Direct3DDevice,
            new FontDescription
            {
                FaceName = "Calibri",
                Height = Program.Instance().TextSize,
                OutputPrecision = FontPrecision.Default,
                Quality = FontQuality.Default,
            });

        private readonly AIHeroClient _hero;
        private int _duration;
        private float _begin;
        private bool _active;
        private readonly int _index;

        private readonly Render.Sprite _sprite;
        private readonly Render.Text _countdownText;
        private readonly Render.Text _healthText;
        private int lastChange;

        public RecallInfo(AIHeroClient hero, int index)
        {
            _hero = hero;
            _index = index;
            _sprite = new Render.Sprite(Resources.RecallBar, new Vector2(0, 0))
            {
                Scale = new Vector2(Program.Instance().BarScale, Program.Instance().BarScale),
                VisibleCondition = sender => _active || Environment.TickCount - lastChange < 3000,
                PositionUpdate = () => new Vector2(Program.Instance().X, Program.Instance().Y - (_index * 20))
            };
            _sprite.Add(0);

            _healthText = new Render.Text(0, 0, "", 20, Color.Green)
            {
                OutLined = true,
                VisibleCondition = sender => _active || Environment.TickCount - lastChange < 3000,
                PositionUpdate = delegate
                {
                    Rectangle rect = TextFont.MeasureText("(" + (int)hero.HealthPercent + "%)");
                    return new Vector2(_sprite.X - rect.Width - GapTextBar, _sprite.Y - rect.Height / 2 + (_sprite.Height * Program.Instance().BarScale) / 2);
                },
                TextUpdate = () => "(" + (int)hero.HealthPercent + "%)"
            };

            _healthText.Add(1);
            Render.Text heroText = new Render.Text(0, 0, _hero.ChampionName, 20, Color.White)
            {
                OutLined = true,
                VisibleCondition = sender => _active || Environment.TickCount - lastChange < 3000,
                PositionUpdate = delegate
                {
                    Rectangle rect = TextFont.MeasureText(_hero.ChampionName + _healthText.text);
                    return new Vector2(_sprite.X - rect.Width - GapTextBar - 3, _sprite.Y - rect.Height / 2 + (_sprite.Height * Program.Instance().BarScale) / 2);
                }
            };

            heroText.Add(1);
            _countdownText = new Render.Text(0, 0, "", 20, Color.White)
            {
                OutLined = true,
                VisibleCondition = sender => _active
            };
            _countdownText.Add(1);
            Game.OnUpdate += Game_OnGameUpdate;
            Teleport.OnTeleport += Teleport_OnTeleport;
        }

        void Teleport_OnTeleport(Obj_AI_Base sender, Teleport.TeleportEventArgs args)
        {
            if (_hero == null || !_hero.IsValid || _hero.IsAlly)
            {
                return;
            }

            if (args.Type != TeleportType.Recall || !(sender is AIHeroClient))
            {
                return;
            }

            if (sender.NetworkId == _hero.NetworkId)
            {
                switch (args.Status)
                {
                    case TeleportStatus.Start:
                        _begin = Game.Time;
                        _duration = args.Duration;
                        _active = true;
                        break;
                    case TeleportStatus.Finish:
                        int colorIndex = (int)((_hero.HealthPercent / 100) * 255);
                        string color = (255 - colorIndex).ToString("X2") + colorIndex.ToString("X2") + "00";
                        Program.Instance().Notify(_hero.ChampionName + " has recalled with <font color='#" + color + "'>" + (int)_hero.HealthPercent + "&#37; HP</font>");
                        _active = false;
                        break;
                    case TeleportStatus.Abort:
                        _active = false;
                        break;
                    case TeleportStatus.Unknown:
                        Program.Instance().Notify(_hero.ChampionName + " is <font color='#ff3232'>unknown</font> (" + _hero.Spellbook.GetSpell(SpellSlot.Recall).Name + ")");
                        _active = false;
                        break;
                }
            }

        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            float colorPercentage = (_hero.HealthPercent / 100);

            _healthText.Color = new ColorBGRA(1 - colorPercentage, colorPercentage, 0, 1);

            if (_active && _duration > 0)
            {
                float percentage = (Game.Time - _begin) / (_duration / 1000f);
                int width = (int)(_sprite.Width - (percentage * _sprite.Width));
                _countdownText.X = (int)(_sprite.X + (width * _sprite.Scale.X) + GapTextBar);
                _countdownText.text = Math.Round((Decimal)((_duration / 1000f) - (Game.Time - _begin)), 1, MidpointRounding.AwayFromZero) + "s";
                Rectangle rect = TextFont.MeasureText(_countdownText.text);
                _countdownText.Y = (int)(_sprite.Y - rect.Height / 2 + (_sprite.Height * Program.Instance().BarScale) / 2);
                _sprite.Crop(0, 0, width, _sprite.Height);
            }
            else
            {
                _sprite.Crop(0, 0, _sprite.Width, _sprite.Height);
            }
        }

        public void Reset()
        {
            lastChange = Environment.TickCount;
        }

        public void Scale(float barScale)
        {
            _sprite.Scale = new Vector2(barScale, barScale);
            Reset();
        }
    }
}