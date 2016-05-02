using System;
using System.Collections.Generic;
using System.Drawing;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;
using Color = System.Drawing.Color;

namespace KA_Lux
{
    public class ColorConfig
    {
        public Slider RedSlider { get; set; }
        public Slider BlueSlider { get; set; }
        public Slider GreenSlider { get; set; }
        public Slider AlphaSlider { get; set; }
        private ColorPickerControl ColorPicker { get; set; }

        public string Id { get; private set; }
        private static Menu _menu;

        public ColorConfig(Menu menu, string id, Color color, string GropuLabelName)
        {
            Id = id;
            _menu = menu;
            Init(color, GropuLabelName);
        }

        public void Init(Color color, string name)
        {
            RedSlider = new Slider("Red", color.R, 0, 255);
            GreenSlider = new Slider("Green", color.B, 0, 255);
            BlueSlider = new Slider("Blue", color.G, 0, 255);
            AlphaSlider = new Slider("Alpha", color.A, 0, 255);
            ColorPicker = new ColorPickerControl(Id + "ColorDisplay", color);

            _menu.AddGroupLabel(name);

            _menu.Add(Id + "ColorDisplay", ColorPicker);
            _menu.Add(Id + "Red", RedSlider);
            _menu.Add(Id + "Green", GreenSlider);
            _menu.Add(Id + "Blue", BlueSlider);
            _menu.Add(Id + "Alpha", AlphaSlider);

            RedSlider.OnValueChange += OnValueChange;
            GreenSlider.OnValueChange += OnValueChange;
            BlueSlider.OnValueChange += OnValueChange;
            AlphaSlider.OnValueChange += OnValueChange;

            ColorPicker.SetColor(Color.FromArgb(GetValue(ColorBytes.Alpha), GetValue(ColorBytes.Red), GetValue(ColorBytes.Green), GetValue(ColorBytes.Blue)));
        }

        private void OnValueChange(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
        {
            if (sender.DisplayName == RedSlider.DisplayName)
            {
                ColorPicker.SetColor(Color.FromArgb(ColorPicker.CurrentValue.A, sender.CurrentValue, ColorPicker.CurrentValue.G, ColorPicker.CurrentValue.B));
            }
            if (sender.DisplayName == GreenSlider.DisplayName)
            {
                ColorPicker.SetColor(Color.FromArgb(ColorPicker.CurrentValue.A, ColorPicker.CurrentValue.R, sender.CurrentValue, ColorPicker.CurrentValue.B));
            }
            if (sender.DisplayName == BlueSlider.DisplayName)
            {
                ColorPicker.SetColor(Color.FromArgb(ColorPicker.CurrentValue.A ,ColorPicker.CurrentValue.R, ColorPicker.CurrentValue.G, sender.CurrentValue));
            }
            if (sender.DisplayName == AlphaSlider.DisplayName)
            {
                ColorPicker.SetColor(Color.FromArgb(sender.CurrentValue, ColorPicker.CurrentValue.R, ColorPicker.CurrentValue.G, ColorPicker.CurrentValue.B));
            }
            
        }

        public ColorBGRA GetSharpColor()
        {                  //RED,GREEN,BLUE,AA
            return new ColorBGRA(GetValue(ColorBytes.Red), GetValue(ColorBytes.Green), GetValue(ColorBytes.Blue), GetValue(ColorBytes.Alpha));
        }

        public Color GetSystemColor()
        {
            return Color.FromArgb(GetValue(ColorBytes.Alpha), GetValue(ColorBytes.Red), GetValue(ColorBytes.Green), GetValue(ColorBytes.Blue));
        }

        public byte GetValue(ColorBytes color)
        {
            switch (color)
            {
                case ColorBytes.Red:
                    return Convert.ToByte(RedSlider.CurrentValue);
                case ColorBytes.Blue:
                    return Convert.ToByte(BlueSlider.CurrentValue);
                case ColorBytes.Green:
                    return Convert.ToByte(GreenSlider.CurrentValue);
                case ColorBytes.Alpha:
                    return Convert.ToByte(AlphaSlider.CurrentValue);
            }
            return 255;
        }
        private class ColorPickerControl : ValueBase<Color>
        {
            private readonly string _name;
            private Vector2 _offset;
            private Color SelectedColor { get; set; }

            private Sprite _colorOverlaySprite;
            private TextureLoader _textureLoader;

            public override string VisibleName { get { return _name; } }
            public override Vector2 Offset { get { return _offset; } }

            public ColorPickerControl(string uId, Color defaultValue) : base(uId, "", 52)
            {
                _name = "";
                Init(defaultValue);
            }

            private static Bitmap ContructColorOverlaySprite()
            {
                var bitmap = new Bitmap(30, 30);
                for (int x = 0; x < 30; x++)
                {
                    for (int y = 0; y < 30; y++)
                    {
                        bitmap.SetPixel(x, y, Color.White);
                    }
                }
                return bitmap;
            }

            public void SetColor(Color color)
            {
                SelectedColor = color;
            }
            private void Init(Color color)
            {
                _offset = new Vector2(0, 10);
                _textureLoader = new TextureLoader();
                _colorOverlaySprite = new Sprite(_textureLoader.Load("ColorOverlaySprite", ContructColorOverlaySprite()));
                SelectedColor = color;
            }

            public override Color CurrentValue { get { return SelectedColor; } }

            public override bool Draw()
            {
                var rect = new SharpDX.Rectangle((int)MainMenu.Position.X + 160, (int)MainMenu.Position.Y + 95 + 50, 750, 380);
                if (MainMenu.IsVisible && IsVisible && rect.IsInside(Position))
                {
                    _colorOverlaySprite.Color = SelectedColor;
                    _colorOverlaySprite.Draw(new Vector2(Position.X + 522 + 1, Position.Y - 34 + 1));
                    return true;
                }
                return false;
            }

            public override Dictionary<string, object> Serialize()
            {
                return base.Serialize();
            }
        }

        public enum ColorBytes
        {
            Red, Green, Blue, Alpha
        }
    }
}