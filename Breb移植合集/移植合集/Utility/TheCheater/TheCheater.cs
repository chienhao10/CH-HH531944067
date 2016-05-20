using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace TheCheater
{
    class TheCheater
    {
        private readonly Dictionary<int, List<IDetector>> _detectors = new Dictionary<int, List<IDetector>>();
        private Menu _mainMenu;
        private Vector2 _screenPos;

        public void Load()
        {
            _mainMenu = MainMenu.AddMenu("辅助探测器", "thecheater");
            var detectionType = _mainMenu.Add("detection", new ComboBox("探测", 0, "一般", "安全", "反人类的"));
            detectionType.OnValueChange += (sender, args) =>
            {
                foreach (var detector in _detectors)
                {
                    detector.Value.ForEach(item => item.ApplySetting((DetectorSetting)args.NewValue));
                }
            };
            _mainMenu.Add("enabled", new CheckBox("开启"));
            _mainMenu.Add("drawing", new CheckBox("线圈"));

            var posX = _mainMenu.Add("positionx", new Slider("位置 X", Drawing.Width - 270, 0, Drawing.Width - 20));
            var posY = _mainMenu.Add("positiony", new Slider("位置 Y", Drawing.Height / 2, 0, Drawing.Height - 20));

            posX.OnValueChange += (sender, args) => _screenPos.X = args.NewValue;
            posY.OnValueChange += (sender, args) => _screenPos.Y = args.NewValue;

            _screenPos.X = posX.Cast<Slider>().CurrentValue;
            _screenPos.Y = posY.Cast<Slider>().CurrentValue;


            Obj_AI_Base.OnNewPath += OnNewPath;
            Drawing.OnDraw += Draw;
        }

        private void Draw(EventArgs args)
        {
            if (!_mainMenu["drawing"].Cast<CheckBox>().CurrentValue) return;

            Drawing.DrawLine(new Vector2(_screenPos.X, _screenPos.Y + 15), new Vector2(_screenPos.X + 180, _screenPos.Y + 15), 2, Color.Red);

            var column = 1;
            Drawing.DrawText(_screenPos.X, _screenPos.Y, Color.Red, "探测到辅助:");
            foreach (var detector in _detectors)
            {
                var maxValue = detector.Value.Max(item => item.GetScriptDetections());
                Drawing.DrawText(_screenPos.X, column * 20 + _screenPos.Y, Color.Red, HeroManager.AllHeroes.First(hero => hero.NetworkId == detector.Key).Name + ": " + maxValue + (maxValue > 0 ? " (" + detector.Value.First(itemId => itemId.GetScriptDetections() == maxValue).GetName() + ")" : string.Empty));
                column++;
            }
        }

        private void OnNewPath(Obj_AI_Base sender, GameObjectNewPathEventArgs args)
        {
            if (sender.Type != GameObjectType.AIHeroClient || !_mainMenu["enabled"].Cast<CheckBox>().CurrentValue) return;

            if (!_detectors.ContainsKey(sender.NetworkId))
            {
                var detectors = new List<IDetector> { new SacOrbwalkerDetector(), new LeaguesharpOrbwalkDetector() };
                detectors.ForEach(detector => detector.Initialize((AIHeroClient)sender));
                _detectors.Add(sender.NetworkId, detectors);
            }
            else
                _detectors[sender.NetworkId].ForEach(detector => detector.FeedData(args.Path.Last()));
        }
    }
}
