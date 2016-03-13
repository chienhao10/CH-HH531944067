
    using System;
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;
    using SharpDX;
    using SharpDX.Direct3D9;
    using Color = System.Drawing.Color;
    using Line = EloBuddy.SDK.Rendering.Line;
    
namespace AdEvade.Config.Controls
{
    /// <summary>
    /// A Color Picker for EloBuddy.net using 4 sliders adjusting the red, green, blue and alpha values respectively.
    /// </summary>
    /// <remarks>
    /// Use at free will, with reference to my GitHub: https://github.com/coman3/
    /// 
    /// Developed by: coman3
    /// </remarks>
    public sealed class ColorPicker : ValueBase<Color>
    {
        #region Values


        internal Text RedText { get; set; }
        internal Text GreenText { get; set; }
        internal Text BlueText { get; set; }
        internal Text AlphaText { get; set; }
        internal Text DisplayNameText { get; set; }
        internal FontDescription DisplayNameTextFont { get; set; }

        public override Color CurrentValue { get; set; }

        public override string VisibleName
        {
            get { return CurrentValue.IsNamedColor ? CurrentValue.Name : CurrentValue.ToString(); }
        }

        public bool IsNamedColor
        {
            get { return CurrentValue.IsNamedColor; }
        }

        public int Red
        {
            get { return CurrentValue.R; }
            set { CurrentValue = Color.FromArgb(Alpha, value, Green, Blue); }
        }
        public int Green
        {
            get { return CurrentValue.G; }
            set { CurrentValue = Color.FromArgb(Alpha, Red, value, Blue); }
        }
        public int Blue
        {
            get { return CurrentValue.B; }
            set { CurrentValue = Color.FromArgb(Alpha, Red, Green, value); }
        }
        public int Alpha
        {
            get { return CurrentValue.A; }
            set { CurrentValue = Color.FromArgb(value, Red, Green, Blue); }
        }
        /// <summary>
        /// Draw a line between the two pointer arrows
        /// </summary>
        public bool DrawPointerLine { get; set; }

        //Mouse Event Data
        internal bool IsMouseDown { get; set; }
        internal RectangleF MouseClickedBox { get; set; }

        public override string ToString()
        {
            return string.Format("Red: {0}, Green: {1}, Blue: {2}, Alpha: {3}", Red, Green, Blue, Alpha);
        }

        #endregion

        #region Rectangles

        /// <summary>
        /// Rectangle for the Red Color Slider. Relative location.
        /// </summary>
        internal RectangleF RedBar { get; set; }
        internal RectangleF RedBarNative
        {
            get { return new RectangleF(Position.X + RedBar.X, Position.Y + RedBar.Y, RedBar.Width, RedBar.Height); }
        }

        /// <summary>
        /// Rectangle for the Green Color Slider. Relative location.
        /// </summary>
        internal RectangleF GreenBar { get; set; }
        internal RectangleF GreenBarNative
        {
            get { return new RectangleF(Position.X + GreenBar.X, Position.Y + GreenBar.Y, GreenBar.Width, GreenBar.Height); }
        }

        /// <summary>
        /// Rectangle for the Blue Color Slider. Relative location.
        /// </summary>
        internal RectangleF BlueBar { get; set; }
        internal RectangleF BlueBarNative
        {
            get { return new RectangleF(Position.X + BlueBar.X, Position.Y + BlueBar.Y, BlueBar.Width, BlueBar.Height); }
        }

        /// <summary>
        /// Rectangle for the Alpha Chanel Slider. Relative location.
        /// </summary>
        internal RectangleF AlphaBar { get; set; }
        internal RectangleF AlphaBarNative
        {
            get { return new RectangleF(Position.X + AlphaBar.X, Position.Y + AlphaBar.Y, AlphaBar.Width, AlphaBar.Height); }
        }

        /// <summary>
        /// Rectangle for the Color Display Box. Relative location.
        /// </summary>
        internal RectangleF ColorBar { get; set; }
        internal RectangleF ColorBarNative
        {
            get { return new RectangleF(Position.X + ColorBar.X, Position.Y + ColorBar.Y, ColorBar.Width, ColorBar.Height); }
        }

        /// <summary>
        /// The box in which the control is contained within.
        /// </summary>
        public RectangleF ControlBoxNative
        {
            get { return new RectangleF(Position.X, Position.Y, Width, Height); }
        }

        #endregion

        #region Positioning 

        internal int BarWidth { get; set; }
        internal float LineWidth { get; set; }
        private readonly int[] _barPadding = { 45, 10, 10, 8 };
        public override Vector2 Offset
        {
            get { return new Vector2(0, 20); }
        }

        #endregion

        #region Constructors 

        public ColorPicker(string displayName, Color defaultColor) : this("", defaultColor, displayName, 100)
        {
        }

        internal ColorPicker(string serializationId, Color defaultColor, string displayName, int height) : base(serializationId, displayName, height)
        {
            MouseClickedBox = RectangleF.Empty;
            DisplayNameTextFont = new FontDescription()
            {
                Height = 20,
                Width = 8,
                CharacterSet = FontCharacterSet.Default,
                Weight = DefaultFont.Weight,
                OutputPrecision = FontPrecision.Default,
                Quality = FontQuality.Default,
                FaceName = DefaultFont.FaceName,
                Italic = false,
                MipLevels = DefaultFont.MipLevels,
                PitchAndFamily = FontPitchAndFamily.Default
            };

            OnThemeChange();
            CurrentValue = defaultColor;
            Messages.OnMessage += Messages_OnMessage;
            //OnLeftMouseDown += ColorPicker_OnLeftMouseDown;
            //OnLeftMouseUp += ColorPicker_OnLeftMouseUp;
            OnMouseMove += ColorPicker_OnMouseMove;
        }

        #endregion

        #region Events

        /// <summary>
        /// Use while events are not being called
        /// </summary>
        /// <param name="args"></param>
        private void Messages_OnMessage(Messages.WindowMessage args)
        {
            if (args.Message == WindowMessages.MouseMove)
                return;
            //Because events are not being called i have to call them my self

            if (args.Message == WindowMessages.LeftButtonDown)
                ColorPicker_OnLeftMouseDown(this, new EventArgs());
            if (args.Message == WindowMessages.LeftButtonUp)
                ColorPicker_OnLeftMouseUp(this, new EventArgs());

        }

        private void ColorPicker_OnMouseMove(EloBuddy.SDK.Menu.Control sender, EventArgs args)
        {
            if (!IsMouseDown || MouseClickedBox == RectangleF.Empty)
                return;
            if (MouseClickedBox == RedBarNative)
                Red = Math.Min(Math.Max(0, GetPosValue(MouseClickedBox, Game.CursorPos2D)), 255);
            else if (MouseClickedBox == GreenBarNative)
                Green = Math.Min(Math.Max(0, GetPosValue(MouseClickedBox, Game.CursorPos2D)), 255);
            else if (MouseClickedBox == BlueBarNative)
                Blue = Math.Min(Math.Max(0, GetPosValue(MouseClickedBox, Game.CursorPos2D)), 255);
            else if (MouseClickedBox == AlphaBarNative)
                Alpha = Math.Min(Math.Max(0, GetPosValue(MouseClickedBox, Game.CursorPos2D)), 255);



        }

        private void ColorPicker_OnLeftMouseDown(EloBuddy.SDK.Menu.Control sender, EventArgs args)
        {
            if (!IsVisible)
                return;
            if (IsInsideRectangle(RedBarNative, Game.CursorPos2D))
                MouseClickedBox = RedBarNative;
            else if (IsInsideRectangle(GreenBarNative, Game.CursorPos2D))
                MouseClickedBox = GreenBarNative;
            else if (IsInsideRectangle(BlueBarNative, Game.CursorPos2D))
                MouseClickedBox = BlueBarNative;
            else if (IsInsideRectangle(AlphaBarNative, Game.CursorPos2D))
                MouseClickedBox = AlphaBarNative;
            else
                return;
            IsMouseDown = true;

        }

        private bool IsInsideRectangle(RectangleF rectangle, Vector2 checkPos)
        {
            return rectangle.Contains(checkPos);
            //return checkPos.X >= rectangle.Left && checkPos.Y >= rectangle.Top &&
            //                checkPos.X <= rectangle.Right && checkPos.Y <= rectangle.Bottom;
        }

        private void ColorPicker_OnLeftMouseUp(EloBuddy.SDK.Menu.Control sender, EventArgs args)
        {
            MouseClickedBox = RectangleF.Empty;
            IsMouseDown = false;
        }


        protected override void OnThemeChange()
        {
            BarWidth = (DefaultWidth - (_barPadding[1] * 2 + _barPadding[2] * 3)) / 4;
            LineWidth = BarWidth / 255f;
            RedBar = new RectangleF(_barPadding[1], _barPadding[0], BarWidth, DefaultHeight - _barPadding[3]);
            GreenBar = new RectangleF(BarWidth + _barPadding[1] * 2, _barPadding[0], BarWidth, DefaultHeight - _barPadding[3]);
            BlueBar = new RectangleF(BarWidth * 2 + _barPadding[1] * 3, _barPadding[0], BarWidth, DefaultHeight - _barPadding[3]);
            AlphaBar = new RectangleF(BarWidth * 3 + _barPadding[1] * 4, _barPadding[0], BarWidth, DefaultHeight - _barPadding[3]);

            ColorBar = new RectangleF(_barPadding[1], 25, Width - _barPadding[1] - _barPadding[2], 10);

            RedText = new Text("Red", DefaultFont) { Color = DefaultColorGreen };
            GreenText = new Text("Green", DefaultFont) { Color = DefaultColorGreen };
            BlueText = new Text("Blue", DefaultFont) { Color = DefaultColorGreen };
            AlphaText = new Text("Alpha", DefaultFont) { Color = DefaultColorGreen };
            DisplayNameText = new Text(DisplayName, DisplayNameTextFont) { Color = DefaultColorGold };

            base.OnThemeChange();
        }

        #endregion

        #region Helpers

        private int GetPosValue(RectangleF box, Vector2 position)
        {
            //Make position within the box's bounds
            position = new Vector2(Math.Max(Math.Min(position.X, box.Right), box.Left) - box.X, 0);
            return (int) (position.X / LineWidth);
        }

        private Vector2 GetLineStartPos(RectangleF box, int value)
        {
            return new Vector2(box.Left + value * LineWidth, box.Top);
        }

        private RectangleF RectangleFromCenter(Vector2 center, float width, float height)
        {
            return new RectangleF(center.X - width / 2f, center.Y - height / 2f, width, height);
        }

        #endregion

        #region Drawing

        public override bool Draw()
        {
            base.Draw();



            DisplayNameText.Position = OffsetVector(ColorBarNative.TopLeft, 0, -22);
            DisplayNameText.Draw();

            //Color Display Bar
            DrawRectangle(ColorBarNative, DefaultColorGold, 2, false);
            Line.DrawLine(CurrentValue, ColorBarNative.Height - 2,
                SetVector(ColorBarNative.TopLeft, ColorBarNative.TopLeft.X + 1, ColorBarNative.Center.Y),
                SetVector(ColorBarNative.TopRight, ColorBarNative.Right, ColorBarNative.Center.Y));

            //Red
            DrawColorBar(RedBarNative, color => Color.FromArgb(color, Green, Blue));
            DrawRectangle(RedBarNative, DefaultColorGold, 2f);
            DrawColorPointer(RedBarNative, Red);
            RedText.TextValue = "Red: " + Red;
            RedText.Position = OffsetVector(RedBarNative.BottomRight, -50, _barPadding[3]);
            RedText.Draw();

            //Green
            DrawColorBar(GreenBarNative, color => Color.FromArgb(Red, color, Blue));
            DrawRectangle(GreenBarNative, DefaultColorGold, 2);
            DrawColorPointer(GreenBarNative, Green);
            GreenText.TextValue = "Green: " + Green;
            GreenText.Position = OffsetVector(GreenBarNative.BottomRight, -60, _barPadding[3]);
            GreenText.Draw();

            //Blue
            DrawColorBar(BlueBarNative, color => Color.FromArgb(Red, Green, color));
            DrawRectangle(BlueBarNative, DefaultColorGold, 2);
            DrawColorPointer(BlueBarNative, Blue);
            BlueText.TextValue = "Blue: " + Blue;
            BlueText.Position = OffsetVector(BlueBarNative.BottomRight, -50, _barPadding[3]);
            BlueText.Draw();

            //Alpha
            DrawColorBar(AlphaBarNative, color => Color.FromArgb(color, color, color));
            DrawRectangle(AlphaBarNative, DefaultColorGold, 2);
            DrawColorPointer(AlphaBarNative, Alpha);
            AlphaText.TextValue = "Alpha: " + Alpha;
            AlphaText.Position = OffsetVector(AlphaBarNative.BottomRight, -60, _barPadding[3]);
            AlphaText.Draw();

            return true;
        }

        private Vector2 SetVector(Vector2 value, float x = float.MaxValue, float y = float.MaxValue)
        {
            value.X = x == float.MaxValue ? value.X : x;
            value.Y = y == float.MaxValue ? value.Y : y;
            return value;
        }

        private Vector2 OffsetVector(Vector2 value, float x, float y)
        {
            value.X += x;
            value.Y += y;
            return value;
        }

        private void DrawRectangle(RectangleF rectangle, Color color, float width, bool inflate = true)
        {
            if (inflate)
                rectangle.Inflate(1, 0);
            Line.DrawLine(color, width, rectangle.TopLeft, rectangle.TopRight, rectangle.BottomRight, rectangle.BottomLeft, rectangle.TopLeft);
        }


        private void DrawColorPointer(RectangleF box, int color)
        {
            var linePos = GetLineStartPos(box, color);
            var arrowTop = RectangleFromCenter(linePos, 8, 10);
            var arrowBottom = RectangleFromCenter(SetVector(linePos, float.MaxValue, box.Bottom), 8, 10);
            Line.DrawLine(DefaultColorGold, 2, arrowTop.TopLeft, arrowTop.Center, arrowTop.TopRight, arrowTop.TopLeft);
            Line.DrawLine(DefaultColorGold, 2, arrowBottom.BottomLeft, arrowBottom.Center, arrowBottom.BottomRight, arrowBottom.BottomLeft);
            if (DrawPointerLine)
                Line.DrawLine(Color.Black, arrowTop.Center, arrowBottom.Center);
        }


        private void DrawColorBar(RectangleF box, Func<int, Color> colorModifier)
        {
            if (colorModifier == null)
                throw new ArgumentException("Color Modifier Argument Invalid!");
            for (int color = 0; color < 255; color++)
            {
                var linePos = GetLineStartPos(box, color);
                Line.DrawLine(colorModifier.Invoke(color), LineWidth + 1, linePos, SetVector(linePos, float.MaxValue, box.Bottom));
            }
        }

        #endregion

    }
}
