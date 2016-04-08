using System;
using System.Collections.Generic;
using System.Linq;
using AdEvade.Config;
using AdEvade.Data;
using AdEvade.Data.Spells;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using AdEvade.Draw;
using AdEvade.Utils;
using SharpDX;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;
using Line = EloBuddy.SDK.Rendering.Line;
using SpellData = AdEvade.Data.Spells.SpellData;

namespace AdEvade.Testing
{

    public class SpellPoint
    {
        public Vector3 StartPosition { get; set; }
        public Vector3 EndPosition { get; set; }
        public int Angle { get; set; }
        public string ChampionName { get; set; }
        public SpellSlot SpellSlot { get; set; }
    }
    public class SpellTester
    {
        public List<SpellPoint> SelectedPoints = new List<SpellPoint>();
        public SpellPoint SelectedPoint;
        public List<string> ChampionNames = new List<string>(); 
        private AIHeroClient MyHero = ObjectManager.Player;
        private Slider _spellIndexSlider;
        private Slider spellChampionIndexSlider;
        public SpellTester(Menu menu)
        {
            if (ConfigValue.EnableSpellTester.GetBool())
            {
                Init();
                CreateMenu(menu.AddSubMenu("Spell Tester"));
            }
        }

        private void CreateMenu(Menu menu)
        {
            menu.AddGroupLabel("Spell Tester");
            menu.Add("SpellTester_CreateSelectedPoint", new KeyBind("Create A Selected Point", false, KeyBind.BindTypes.PressToggle)).OnValueChange += SpellTester_CreateSelectedPoint_OnValueChange; ;
            menu.Add("SpellTester_CreateSpell", new KeyBind("Create Spell At Selected Points", false, KeyBind.BindTypes.PressToggle)).OnValueChange += SpellTester_CreateSpell_OnValueChange; ;
            menu.AddSeparator();
            menu.Add("SpellTester_ClearSelectedPoints", new CheckBox("Clear Selected Points", false)).OnValueChange += SpellTester_ClearSelectedPoints_OnValueChange;
            menu.AddSeparator();
            menu.AddGroupLabel("Spell Selector");
            _spellIndexSlider = new Slider("Selected Spell", 0, 0, 0);
            menu.Add("SpellTester_SelectedSpellIndex", _spellIndexSlider).OnValueChange += SpellTester_SelectedSpellIndex_OnValueChange;
            menu.Add("SpellTester_SelectedSpellChampionIndex", new StringSlider(ConfigDataType.Data, "SpellTester_SelectedSpellChampionIndex", "Select Spells Champion",
                    0, SpellConfigProperty.None, ChampionNames.ToArray()).Slider.Slider);
            menu.Add("SpellTester_SelectedSpellChampionSlot",
                new StringSlider(ConfigDataType.Data, "SpellTester_SelectedSpellChampionSlot", "Select Spells Champion",
                    0, SpellConfigProperty.None, Enum.GetNames(typeof(SpellSlot))).Slider.Slider);
            menu.Add("SpellTester_SelectedSpellAngle",
                new DynamicSlider(ConfigDataType.Data, "SpellTester_SelectedSpellAngle", "Angle Of Spell", 90, 0, 360)
                    .Slider).OnValueChange += SpellTester_SelectedSpellAngle_OnValueChange;
        }

        private void SpellTester_SelectedSpellIndex_OnValueChange(ValueBase<int> sender,
            ValueBase<int>.ValueChangeArgs args)
        {
            if (SelectedPoints.Count > 0)
            {
                SelectedPoint = SelectedPoints[Config.Properties.GetData<int>("SpellTester_SelectedSpellIndex") - 1];
            }
        }

        private void SpellTester_SelectedSpellAngle_OnValueChange(ValueBase<int> sender,
            ValueBase<int>.ValueChangeArgs args)
        {
            if (SelectedPoint != null)
            {
                SelectedPoint.Angle = sender.CurrentValue;
            }

        }

        private SpellData GetSpellData(string championName, SpellSlot slot)
        {
            return SpellDatabase.Spells.FirstOrDefault(x => x.CharName == championName && x.SpellKey == slot);
        }
        private void SpellTester_CreateSpell_OnValueChange(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
        {
            foreach (var selectedPoint in SelectedPoints)
            {
                SpellDetector.CreateTestSpell(selectedPoint,
                    GetSpellData(selectedPoint.ChampionName, selectedPoint.SpellSlot));
            }
        }

        private void SpellTester_ClearSelectedPoints_OnValueChange(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
        {
            if (!args.OldValue && args.NewValue)
            {
                SelectedPoints.Clear();
                _spellIndexSlider.Slider.MinValue = 0;
                _spellIndexSlider.Slider.CurrentValue = 0;
                _spellIndexSlider.Slider.MaxValue = 0;
                sender.CurrentValue = false;
                SelectedPoint = null;
            }
        }

        private SpellSlot GetSelectedSpellSlot()
        {
            return (SpellSlot) Config.Properties.GetData<int>("SpellTester_SelectedSpellChampionSlot");
        }
        private string GetSelectedChampionName()
        {
            return ChampionNames[Config.Properties.GetData<int>("SpellTester_SelectedSpellChampionIndex")];
        }
        private void SpellTester_CreateSelectedPoint_OnValueChange(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
        {
            if (!args.OldValue && args.NewValue)
            {
                if (SelectedPoint == null)
                {
                    SelectedPoint = new SpellPoint
                    {
                        StartPosition = MyHero.Position,
                        ChampionName = GetSelectedChampionName(),
                        SpellSlot = GetSelectedSpellSlot(),
                        Angle = Config.Properties.GetData<int>("SpellTester_SelectedSpellAngle")
                    };
                    SelectedPoints.Add(SelectedPoint);
                    _spellIndexSlider.Slider.MaxValue = SelectedPoints.Count;
                    _spellIndexSlider.Slider.MinValue = 1;
                }
                sender.CurrentValue = false;              
            }
        }

        private void Init()
        {
            UpdateChampionNameList();
            Drawing.OnBeginScene += Drawing_OnDraw;

        }
        private void UpdateChampionNameList()
        {
            foreach (var spell in SpellDatabase.Spells)
            {
                // ReSharper disable once SimplifyLinqExpression
                if (!ChampionNames.Any(x => x == spell.CharName))
                    ChampionNames.Add(spell.CharName);
            }
            ChampionNames.Sort();
        }

        private void DrawSelectedPoints()
        {
            var drawPoints = SelectedPoints.Select(selectedPoint => selectedPoint.StartPosition).ToArray();
            if (SelectedPoints.Count > 0)
            {
                drawPoints = SelectedPoints.Where(p => SelectedPoints.IndexOf(p) != _spellIndexSlider.Slider.CurrentValue - 1).Select(selectedPoint => selectedPoint.StartPosition).ToArray();
                Circle.Draw(new ColorBGRA(255, 0, 255, 255), 10, 10f, SelectedPoints[_spellIndexSlider.Slider.CurrentValue - 1].StartPosition);
            }
            Circle.Draw(new ColorBGRA(0, 0, 255, 255), 10, 10f, drawPoints);
        }

        private void Drawing_OnDraw(System.EventArgs args)
        {
            DrawSelectedPoints();
            DrawPendingSpell();
        }

        private void DrawPendingSpell()
        {
            if (SelectedPoint != null)
            {
                var spellData = GetSpellData(SelectedPoint.ChampionName, SelectedPoint.SpellSlot);

                if (spellData != null)
                {
                    Circle.Draw(new ColorBGRA(255, 255, 255, 255), spellData.Range, 1, SelectedPoint.StartPosition);
                    var angle = SelectedPoint.Angle/(180/(Math.PI));
                    var newX = SelectedPoint.StartPosition.X + spellData.Range* Math.Cos(angle);
                    var newY = SelectedPoint.StartPosition.Y + spellData.Range* Math.Sin(angle);
                    SelectedPoint.EndPosition = new Vector3((float) newX, (float) newY, SelectedPoint.StartPosition.Z);
                    Line.DrawLine(Color.White, SelectedPoint.StartPosition, SelectedPoint.EndPosition);

                }
            }
        }
    }
}