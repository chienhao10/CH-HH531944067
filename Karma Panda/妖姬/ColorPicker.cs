using System;
using System.Drawing;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;
using Color = System.Drawing.Color;
using Sprite = EloBuddy.SDK.Rendering.Sprite;

namespace LelBlanc
{
    /// <summary>
    /// Credits to MarioGK. From KickAss AIO.
    /// </summary>
    internal class ColorPicker
    {
        public Slider RedSlider { get; set; }
        public Slider BlueSlider { get; set; }
        public Slider GreenSlider { get; set; }
        public Slider AlphaSlider { get; set; }
        private ColorPickerControl Picker { get; set; }

        public string Id { get; private set; }
        private static Menu _menu;

        public ColorPicker(Menu menu, string id, Color color, string groupLabelName)
        {
            Id = id;
            _menu = menu;
            Init(color, groupLabelName);
        }

        public void Init(Color color, string name)
        {
            AlphaSlider = new Slider("Alpha", color.A, 0, 255);
            RedSlider = new Slider("Red", color.R, 0, 255);
            GreenSlider = new Slider("Green", color.B, 0, 255);
            BlueSlider = new Slider("Blue", color.G, 0, 255);
            Picker = new ColorPickerControl(Id + "ColorDisplay", color);

            _menu.AddLabel(name);

            _menu.Add(Id + "ColorDisplay", Picker);
            _menu.Add(Id + "Alpha", AlphaSlider);
            _menu.Add(Id + "Red", RedSlider);
            _menu.Add(Id + "Green", GreenSlider);
            _menu.Add(Id + "Blue", BlueSlider);

            AlphaSlider.OnValueChange += OnValueChange;
            RedSlider.OnValueChange += OnValueChange;
            GreenSlider.OnValueChange += OnValueChange;
            BlueSlider.OnValueChange += OnValueChange;

            Picker.SetColor(Color.FromArgb(GetValue(ColorBytes.Alpha), GetValue(ColorBytes.Red),
                GetValue(ColorBytes.Green), GetValue(ColorBytes.Blue)));
        }

        private void OnValueChange(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
        {
            if (sender.DisplayName == RedSlider.DisplayName)
            {
                Picker.SetColor(Color.FromArgb(Picker.CurrentValue.A, sender.CurrentValue, Picker.CurrentValue.G,
                    Picker.CurrentValue.B));
            }
            if (sender.DisplayName == GreenSlider.DisplayName)
            {
                Picker.SetColor(Color.FromArgb(Picker.CurrentValue.A, Picker.CurrentValue.R, sender.CurrentValue,
                    Picker.CurrentValue.B));
            }
            if (sender.DisplayName == BlueSlider.DisplayName)
            {
                Picker.SetColor(Color.FromArgb(Picker.CurrentValue.A, Picker.CurrentValue.R, Picker.CurrentValue.G,
                    sender.CurrentValue));
            }
            if (sender.DisplayName == AlphaSlider.DisplayName)
            {
                Picker.SetColor(Color.FromArgb(sender.CurrentValue, Picker.CurrentValue.R, Picker.CurrentValue.G,
                    Picker.CurrentValue.B));
            }
        }

        public ColorBGRA GetSharpColor()
        {
            return new ColorBGRA(GetValue(ColorBytes.Red), GetValue(ColorBytes.Green), GetValue(ColorBytes.Blue),
                GetValue(ColorBytes.Alpha));
        }

        public Color GetSystemColor()
        {
            return Color.FromArgb(GetValue(ColorBytes.Alpha), GetValue(ColorBytes.Red), GetValue(ColorBytes.Green),
                GetValue(ColorBytes.Blue));
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

            private Sprite _colorPickerSprite;
            private Sprite _colorOverlaySprite;
            private TextureLoader _textureLoader;

            public override string VisibleName
            {
                get { return _name; }
            }

            public override Vector2 Offset
            {
                get { return _offset; }
            }

            public ColorPickerControl(string uId, Color defaultValue) : base(uId, "", 52)
            {
                _name = "";
                Init(defaultValue);
            }

            private static Bitmap ColorPickerSprite()
            {
                var bitmap = new Bitmap(32, 32);
                return bitmap;
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
                _colorPickerSprite = new Sprite(_textureLoader.Load("ColorPickerSprite", ColorPickerSprite()));
                _colorOverlaySprite = new Sprite(_textureLoader.Load("ColorOverlaySprite", ContructColorOverlaySprite()));
                SelectedColor = color;
            }

            public override Color CurrentValue
            {
                get { return SelectedColor; }
            }

            public override bool Draw()
            {
                var rect = new SharpDX.Rectangle((int) MainMenu.Position.X + 160, (int) MainMenu.Position.Y + 95 + 50,
                    750, 380);
                if (MainMenu.IsVisible && IsVisible && rect.IsInside(Position))
                {
                    _colorPickerSprite.Draw(new Vector2(Position.X + 522, Position.Y - 34));
                    _colorOverlaySprite.Color = SelectedColor;
                    _colorOverlaySprite.Draw(new Vector2(Position.X + 522 + 1, Position.Y - 34 + 1));
                    return true;
                }
                return false;
            }
        }

        public enum ColorBytes
        {
            Alpha,
            Red,
            Green,
            Blue
        }
    }
}