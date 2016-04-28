using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace UniversalGankAlerter
{
    internal class Program
    {
        private static Program _instance;

        private readonly IDictionary<int, ChampionInfo> _championInfoById = new Dictionary<int, ChampionInfo>();
        private Menu _menu;
        private PreviewCircle _previewCircle;
        private Menu _enemies;
        private Menu _allies;

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

        public int Radius
        {
            get { return getSliderItem(_menu, "range"); }
        }

        public int Cooldown
        {
            get { return getSliderItem(_menu, "cooldown"); }
        }

        public bool DangerPing
        {
            get { return getCheckBoxItem(_menu, "dangerping"); }
        }

        public int LineDuration
        {
            get { return getSliderItem(_menu, "lineduration"); }
        }

        public bool EnemyJunglerOnly
        {
            get { return getCheckBoxItem(_enemies, "jungleronly"); }
        }

        public bool AllyJunglerOnly
        {
            get { return getCheckBoxItem(_allies, "allyjungleronly"); }
        }

        public bool ShowChampionNames
        {
            get { return getCheckBoxItem(_menu, "shownames"); }
        }

        public bool DrawMinimapLines
        {
            get { return getCheckBoxItem(_menu, "drawminimaplines"); }
        }

        public static void Main()
        {
            _instance = new Program();
        }

        public static Program Instance()
        {
            return _instance;
        }

        public Program()
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private void Game_OnGameLoad(EventArgs args)
        {
            _previewCircle = new PreviewCircle();

            _menu = MainMenu.AddMenu("Universal GankAlerter", "universalgankalerter");

            _menu.Add("range", new Slider("Trigger range", 3000, 500, 5000));
            _menu["range"].Cast<Slider>().OnValueChange += SliderRadiusValueChanged;

            _menu.Add("cooldown", new Slider("Trigger cooldown (sec)", 10, 0, 60));
            _menu.Add("lineduration", new Slider("Line duration (sec)", 10, 0, 20));
            _menu.Add("shownames", new CheckBox("Show champion name"));
            _menu.Add("drawminimaplines", new CheckBox("Draw minimap lines", false));
            _menu.Add("dangerping", new CheckBox("Danger Ping (local)", false));

            _enemies = _menu.AddSubMenu("Enemies", "enemies");
            _enemies.Add("jungleronly", new CheckBox("Warn jungler only (smite)", false));

            _allies = _menu.AddSubMenu("Allies", "allies");
            _allies.Add("allyjungleronly", new CheckBox("Warn jungler only (smite)"));

            foreach (AIHeroClient hero in ObjectManager.Get<AIHeroClient>())
            {
                if (hero.NetworkId != ObjectManager.Player.NetworkId)
                {
                    if (hero.IsEnemy)
                    {
                        _championInfoById[hero.NetworkId] = new ChampionInfo(hero, false);
                        _enemies.Add("enemy" + hero.ChampionName, new CheckBox(hero.ChampionName));
                    }
                    else
                    {
                        _championInfoById[hero.NetworkId] = new ChampionInfo(hero, true);
                        _allies.Add("ally" + hero.ChampionName, new CheckBox(hero.ChampionName, false));
                    }
                }
            }

            Print("Loaded!");
        }

        private void SliderRadiusValueChanged(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
        {
            _previewCircle.SetRadius(args.NewValue);
        }

        private static void Print(string msg)
        {
            Chat.Print("<font color='#ff3232'>Universal</font><font color='#d4d4d4'>GankAlerter:</font> <font color='#FFFFFF'>" + msg + "</font>");
        }

        public bool IsEnabled(AIHeroClient hero)
        {
            return hero.IsEnemy ? getCheckBoxItem(_enemies, "enemy" + hero.ChampionName) : getCheckBoxItem(_allies, "ally" + hero.ChampionName);
        }
    }

    internal class PreviewCircle
    {
        private const int Delay = 2;

        private float _lastChanged;
        private readonly Render.Circle _mapCircle;
        private int _radius;

        public PreviewCircle()
        {
            Drawing.OnEndScene += Drawing_OnEndScene;
            _mapCircle = new Render.Circle(ObjectManager.Player, 0, System.Drawing.Color.Red, 5);
            _mapCircle.Add(0);
            _mapCircle.VisibleCondition = sender => _lastChanged > 0 && Game.Time - _lastChanged < Delay;
        }

        private void Drawing_OnEndScene(EventArgs args)
        {
            if (_lastChanged > 0 && Game.Time - _lastChanged < Delay)
            {
                LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, _radius, System.Drawing.Color.Red, 2, 30, true);
            }
        }

        public void SetRadius(int radius)
        {
            _radius = radius;
            _mapCircle.Radius = radius;
            _lastChanged = Game.Time;
        }
    }

    internal class ChampionInfo
    {
        private static int index = 0;

        private readonly AIHeroClient _hero;

        private event EventHandler OnEnterRange;

        private bool _visible;
        private float _distance;
        private float _lastEnter;
        private float _lineStart;
        private readonly Render.Line _line;

        public ChampionInfo(AIHeroClient hero, bool ally)
        {
            index++;
            int textoffset = index * 50;
            _hero = hero;
            Render.Text text = new Render.Text(
                new Vector2(), _hero.ChampionName, 20,
                ally
                    ? new Color { R = 205, G = 255, B = 205, A = 255 }
                    : new Color { R = 255, G = 205, B = 205, A = 255 })
            {
                PositionUpdate =
                    () =>
                        Drawing.WorldToScreen(
                            ObjectManager.Player.Position.LSExtend(_hero.Position, 300 + textoffset)),
                VisibleCondition = delegate
                {
                    float dist = _hero.LSDistance(ObjectManager.Player.Position);
                    return Program.Instance().ShowChampionNames && !_hero.IsDead &&
                           Game.Time - _lineStart < Program.Instance().LineDuration &&
                           (!_hero.IsVisible || !Render.OnScreen(Drawing.WorldToScreen(_hero.Position))) &&
                           dist < Program.Instance().Radius && dist > 300 + textoffset;
                },
                Centered = true,
                OutLined = true,
            };
            text.Add(1);
            _line = new Render.Line(
                new Vector2(), new Vector2(), 5,
                ally ? new Color { R = 0, G = 255, B = 0, A = 125 } : new Color { R = 255, G = 0, B = 0, A = 125 })
            {
                StartPositionUpdate = () => Drawing.WorldToScreen(ObjectManager.Player.Position),
                EndPositionUpdate = () => Drawing.WorldToScreen(_hero.Position),
                VisibleCondition =
                    delegate
                    {
                        return !_hero.IsDead && Game.Time - _lineStart < Program.Instance().LineDuration &&
                               _hero.LSDistance(ObjectManager.Player.Position) < (Program.Instance().Radius + 1000);
                    }
            };
            _line.Add(0);
            Render.Line minimapLine = new Render.Line(
                new Vector2(), new Vector2(), 2,
                ally ? new Color { R = 0, G = 255, B = 0, A = 255 } : new Color { R = 255, G = 0, B = 0, A = 255 })
            {
                StartPositionUpdate = () => Drawing.WorldToMinimap(ObjectManager.Player.Position),
                EndPositionUpdate = () => Drawing.WorldToMinimap(_hero.Position),
                VisibleCondition =
                    delegate
                    {
                        return Program.Instance().DrawMinimapLines && !_hero.IsDead && Game.Time - _lineStart < Program.Instance().LineDuration;
                    }
            };
            minimapLine.Add(0);
            Game.OnUpdate += Game_OnGameUpdate;
            OnEnterRange += ChampionInfo_OnEnterRange;
        }

        private void ChampionInfo_OnEnterRange(object sender, EventArgs e)
        {
            bool enabled = false;
            if (Program.Instance().EnemyJunglerOnly && _hero.IsEnemy)
            {
                enabled = IsJungler(_hero);
            }
            else if (Program.Instance().AllyJunglerOnly && _hero.IsAlly)
            {
                enabled = IsJungler(_hero);
            }
            else
            {
                enabled = Program.Instance().IsEnabled(_hero);
            }

            if (Game.Time - _lastEnter > Program.Instance().Cooldown && enabled)
            {
                _lineStart = Game.Time;
                if (Program.Instance().DangerPing && _hero.IsEnemy && !_hero.IsDead)
                {
                    TacticalMap.ShowPing(PingCategory.Danger, _hero, true);
                }
            }
            _lastEnter = Game.Time;
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            float newDistance = _hero.LSDistance(ObjectManager.Player);

            if (Game.Time - _lineStart < Program.Instance().LineDuration)
            {
                float percentage = newDistance / Program.Instance().Radius;
                if (percentage <= 1)
                {
                    _line.Width = (int) (2 + (percentage * 8));
                }
            }

            if (newDistance < Program.Instance().Radius && _hero.IsVisible)
            {
                if (_distance >= Program.Instance().Radius || !_visible)
                {
                    if (OnEnterRange != null)
                    {
                        OnEnterRange(this, null);
                    }
                }
                else if (_distance < Program.Instance().Radius && _visible)
                {
                    _lastEnter = Game.Time;
                }
            }
            _distance = newDistance;
            _visible = _hero.IsVisible;
        }

        private bool IsJungler(AIHeroClient hero)
        {
            return hero.Spellbook.Spells.Any(spell => spell.Name.ToLower().Contains("smite"));
        }
    }
}