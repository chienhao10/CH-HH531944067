namespace KappAzir.Utility
{
    using System;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;

    using SharpDX;
    using SharpDX.Direct3D9;

    using Color = System.Drawing.Color;
    using Line = EloBuddy.SDK.Rendering.Line;

    /// <summary>
    /// A Color Picker for EloBuddy.net using 4 sliders adjusting the red, green, blue and alpha values respectively.
    /// </summary>
    /// <remarks>
    /// Use at free will, with reference to my GitHub: https://github.com/coman3/
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
            get
            {
                return this.CurrentValue.IsNamedColor ? this.CurrentValue.Name : this.CurrentValue.ToString();
            }
        }

        public bool IsNamedColor
        {
            get
            {
                return this.CurrentValue.IsNamedColor;
            }
        }

        public int Red
        {
            get
            {
                return this.CurrentValue.R;
            }
            set
            {
                this.CurrentValue = Color.FromArgb(this.Alpha, value, this.Green, this.Blue);
            }
        }

        public int Green
        {
            get
            {
                return this.CurrentValue.G;
            }
            set
            {
                this.CurrentValue = Color.FromArgb(this.Alpha, this.Red, value, this.Blue);
            }
        }

        public int Blue
        {
            get
            {
                return this.CurrentValue.B;
            }
            set
            {
                this.CurrentValue = Color.FromArgb(this.Alpha, this.Red, this.Green, value);
            }
        }

        public int Alpha
        {
            get
            {
                return this.CurrentValue.A;
            }
            set
            {
                this.CurrentValue = Color.FromArgb(value, this.Red, this.Green, this.Blue);
            }
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
            return string.Format("Red: {0}, Green: {1}, Blue: {2}, Alpha: {3}", this.Red, this.Green, this.Blue, this.Alpha);
        }

        #endregion

        #region Rectangles

        /// <summary>
        /// Rectangle for the Red Color Slider. Relative location.
        /// </summary>
        internal RectangleF RedBar { get; set; }

        internal RectangleF RedBarNative
        {
            get
            {
                return new RectangleF(this.Position.X + this.RedBar.X, this.Position.Y + this.RedBar.Y, this.RedBar.Width, this.RedBar.Height);
            }
        }

        /// <summary>
        /// Rectangle for the Green Color Slider. Relative location.
        /// </summary>
        internal RectangleF GreenBar { get; set; }

        internal RectangleF GreenBarNative
        {
            get
            {
                return new RectangleF(this.Position.X + this.GreenBar.X, this.Position.Y + this.GreenBar.Y, this.GreenBar.Width, this.GreenBar.Height);
            }
        }

        /// <summary>
        /// Rectangle for the Blue Color Slider. Relative location.
        /// </summary>
        internal RectangleF BlueBar { get; set; }

        internal RectangleF BlueBarNative
        {
            get
            {
                return new RectangleF(this.Position.X + this.BlueBar.X, this.Position.Y + this.BlueBar.Y, this.BlueBar.Width, this.BlueBar.Height);
            }
        }

        /// <summary>
        /// Rectangle for the Alpha Chanel Slider. Relative location.
        /// </summary>
        internal RectangleF AlphaBar { get; set; }

        internal RectangleF AlphaBarNative
        {
            get
            {
                return new RectangleF(this.Position.X + this.AlphaBar.X, this.Position.Y + this.AlphaBar.Y, this.AlphaBar.Width, this.AlphaBar.Height);
            }
        }

        /// <summary>
        /// Rectangle for the Color Display Box. Relative location.
        /// </summary>
        internal RectangleF ColorBar { get; set; }

        internal RectangleF ColorBarNative
        {
            get
            {
                return new RectangleF(this.Position.X + this.ColorBar.X, this.Position.Y + this.ColorBar.Y, this.ColorBar.Width, this.ColorBar.Height);
            }
        }

        /// <summary>
        /// The box in which the control is contained within.
        /// </summary>
        public RectangleF ControlBoxNative
        {
            get
            {
                return new RectangleF(this.Position.X, this.Position.Y, this.Width, this.Height);
            }
        }

        #endregion

        #region Positioning 

        internal int BarWidth { get; set; }

        internal float LineWidth { get; set; }

        private readonly int[] _barPadding = { 45, 10, 10, 8 };

        public override Vector2 Offset
        {
            get
            {
                return new Vector2(0, 20);
            }
        }

        #endregion

        #region Constructors 

        public ColorPicker(string displayName, Color defaultColor)
            : this("", defaultColor, displayName, 100)
        {
        }

        internal ColorPicker(string serializationId, Color defaultColor, string displayName, int height)
            : base(serializationId, displayName, height)
        {
            this.MouseClickedBox = RectangleF.Empty;
            this.DisplayNameTextFont = new FontDescription()
                                           {
                                               Height = 20, Width = 8, CharacterSet = FontCharacterSet.Default, Weight = DefaultFont.Weight,
                                               OutputPrecision = FontPrecision.Default, Quality = FontQuality.Default, FaceName = DefaultFont.FaceName,
                                               Italic = false, MipLevels = DefaultFont.MipLevels, PitchAndFamily = FontPitchAndFamily.Default
                                           };

            this.OnThemeChange();
            this.CurrentValue = defaultColor;
            Messages.OnMessage += this.Messages_OnMessage;
            //OnLeftMouseDown += ColorPicker_OnLeftMouseDown;
            //OnLeftMouseUp += ColorPicker_OnLeftMouseUp;
            this.OnMouseMove += this.ColorPicker_OnMouseMove;
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
            {
                return;
            }
            //Because events are not being called i have to call them my self

            if (args.Message == WindowMessages.LeftButtonDown)
            {
                this.ColorPicker_OnLeftMouseDown(this, new EventArgs());
            }
            if (args.Message == WindowMessages.LeftButtonUp)
            {
                this.ColorPicker_OnLeftMouseUp(this, new EventArgs());
            }
        }

        private void ColorPicker_OnMouseMove(EloBuddy.SDK.Menu.Control sender, EventArgs args)
        {
            if (!this.IsMouseDown || this.MouseClickedBox == RectangleF.Empty)
            {
                return;
            }
            if (this.MouseClickedBox == this.RedBarNative)
            {
                this.Red = Math.Min(Math.Max(0, this.GetPosValue(this.MouseClickedBox, Game.CursorPos2D)), 255);
            }
            else if (this.MouseClickedBox == this.GreenBarNative)
            {
                this.Green = Math.Min(Math.Max(0, this.GetPosValue(this.MouseClickedBox, Game.CursorPos2D)), 255);
            }
            else if (this.MouseClickedBox == this.BlueBarNative)
            {
                this.Blue = Math.Min(Math.Max(0, this.GetPosValue(this.MouseClickedBox, Game.CursorPos2D)), 255);
            }
            else if (this.MouseClickedBox == this.AlphaBarNative)
            {
                this.Alpha = Math.Min(Math.Max(0, this.GetPosValue(this.MouseClickedBox, Game.CursorPos2D)), 255);
            }
        }

        private void ColorPicker_OnLeftMouseDown(EloBuddy.SDK.Menu.Control sender, EventArgs args)
        {
            if (!this.IsVisible)
            {
                return;
            }
            if (this.IsInsideRectangle(this.RedBarNative, Game.CursorPos2D))
            {
                this.MouseClickedBox = this.RedBarNative;
            }
            else if (this.IsInsideRectangle(this.GreenBarNative, Game.CursorPos2D))
            {
                this.MouseClickedBox = this.GreenBarNative;
            }
            else if (this.IsInsideRectangle(this.BlueBarNative, Game.CursorPos2D))
            {
                this.MouseClickedBox = this.BlueBarNative;
            }
            else if (this.IsInsideRectangle(this.AlphaBarNative, Game.CursorPos2D))
            {
                this.MouseClickedBox = this.AlphaBarNative;
            }
            else
            {
                return;
            }
            this.IsMouseDown = true;
        }

        private bool IsInsideRectangle(RectangleF rectangle, Vector2 checkPos)
        {
            return rectangle.Contains(checkPos);
            //return checkPos.X >= rectangle.Left && checkPos.Y >= rectangle.Top &&
            //                checkPos.X <= rectangle.Right && checkPos.Y <= rectangle.Bottom;
        }

        private void ColorPicker_OnLeftMouseUp(EloBuddy.SDK.Menu.Control sender, EventArgs args)
        {
            this.MouseClickedBox = RectangleF.Empty;
            this.IsMouseDown = false;
        }

        protected override void OnThemeChange()
        {
            this.BarWidth = (DefaultWidth - (this._barPadding[1] * 2 + this._barPadding[2] * 3)) / 4;
            this.LineWidth = this.BarWidth / 255f;
            this.RedBar = new RectangleF(this._barPadding[1], this._barPadding[0], this.BarWidth, DefaultHeight - this._barPadding[3]);
            this.GreenBar = new RectangleF(
                this.BarWidth + this._barPadding[1] * 2,
                this._barPadding[0],
                this.BarWidth,
                DefaultHeight - this._barPadding[3]);
            this.BlueBar = new RectangleF(
                this.BarWidth * 2 + this._barPadding[1] * 3,
                this._barPadding[0],
                this.BarWidth,
                DefaultHeight - this._barPadding[3]);
            this.AlphaBar = new RectangleF(
                this.BarWidth * 3 + this._barPadding[1] * 4,
                this._barPadding[0],
                this.BarWidth,
                DefaultHeight - this._barPadding[3]);

            this.ColorBar = new RectangleF(this._barPadding[1], 25, this.Width - this._barPadding[1] - this._barPadding[2], 10);

            this.RedText = new Text("Red", DefaultFont) { Color = DefaultColorGreen };
            this.GreenText = new Text("Green", DefaultFont) { Color = DefaultColorGreen };
            this.BlueText = new Text("Blue", DefaultFont) { Color = DefaultColorGreen };
            this.AlphaText = new Text("Alpha", DefaultFont) { Color = DefaultColorGreen };
            this.DisplayNameText = new Text(this.DisplayName, this.DisplayNameTextFont) { Color = DefaultColorGold };

            base.OnThemeChange();
        }

        #endregion

        #region Helpers

        private int GetPosValue(RectangleF box, Vector2 position)
        {
            //Make position within the box's bounds
            position = new Vector2(Math.Max(Math.Min(position.X, box.Right), box.Left) - box.X, 0);
            return (int)(position.X / this.LineWidth);
        }

        private Vector2 GetLineStartPos(RectangleF box, int value)
        {
            return new Vector2(box.Left + value * this.LineWidth, box.Top);
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

            this.DisplayNameText.Position = this.OffsetVector(this.ColorBarNative.TopLeft, 0, -22);
            this.DisplayNameText.Draw();

            //Color Display Bar
            this.DrawRectangle(this.ColorBarNative, DefaultColorGold, 2, false);
            Line.DrawLine(
                this.CurrentValue,
                this.ColorBarNative.Height - 2,
                this.SetVector(this.ColorBarNative.TopLeft, this.ColorBarNative.TopLeft.X + 1, this.ColorBarNative.Center.Y),
                this.SetVector(this.ColorBarNative.TopRight, this.ColorBarNative.Right, this.ColorBarNative.Center.Y));

            //Red
            this.DrawColorBar(this.RedBarNative, color => Color.FromArgb(color, this.Green, this.Blue));
            this.DrawRectangle(this.RedBarNative, DefaultColorGold, 2f);
            this.DrawColorPointer(this.RedBarNative, this.Red);
            this.RedText.TextValue = "Red: " + this.Red;
            this.RedText.Position = this.OffsetVector(this.RedBarNative.BottomRight, -50, this._barPadding[3]);
            this.RedText.Draw();

            //Green
            this.DrawColorBar(this.GreenBarNative, color => Color.FromArgb(this.Red, color, this.Blue));
            this.DrawRectangle(this.GreenBarNative, DefaultColorGold, 2);
            this.DrawColorPointer(this.GreenBarNative, this.Green);
            this.GreenText.TextValue = "Green: " + this.Green;
            this.GreenText.Position = this.OffsetVector(this.GreenBarNative.BottomRight, -60, this._barPadding[3]);
            this.GreenText.Draw();

            //Blue
            this.DrawColorBar(this.BlueBarNative, color => Color.FromArgb(this.Red, this.Green, color));
            this.DrawRectangle(this.BlueBarNative, DefaultColorGold, 2);
            this.DrawColorPointer(this.BlueBarNative, this.Blue);
            this.BlueText.TextValue = "Blue: " + this.Blue;
            this.BlueText.Position = this.OffsetVector(this.BlueBarNative.BottomRight, -50, this._barPadding[3]);
            this.BlueText.Draw();

            //Alpha
            this.DrawColorBar(this.AlphaBarNative, color => Color.FromArgb(color, color, color));
            this.DrawRectangle(this.AlphaBarNative, DefaultColorGold, 2);
            this.DrawColorPointer(this.AlphaBarNative, this.Alpha);
            this.AlphaText.TextValue = "Alpha: " + this.Alpha;
            this.AlphaText.Position = this.OffsetVector(this.AlphaBarNative.BottomRight, -60, this._barPadding[3]);
            this.AlphaText.Draw();

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
            {
                rectangle.Inflate(1, 0);
            }
            Line.DrawLine(color, width, rectangle.TopLeft, rectangle.TopRight, rectangle.BottomRight, rectangle.BottomLeft, rectangle.TopLeft);
        }

        private void DrawColorPointer(RectangleF box, int color)
        {
            var linePos = this.GetLineStartPos(box, color);
            var arrowTop = this.RectangleFromCenter(linePos, 8, 10);
            var arrowBottom = this.RectangleFromCenter(this.SetVector(linePos, float.MaxValue, box.Bottom), 8, 10);
            Line.DrawLine(DefaultColorGold, 2, arrowTop.TopLeft, arrowTop.Center, arrowTop.TopRight, arrowTop.TopLeft);
            Line.DrawLine(DefaultColorGold, 2, arrowBottom.BottomLeft, arrowBottom.Center, arrowBottom.BottomRight, arrowBottom.BottomLeft);
            if (this.DrawPointerLine)
            {
                Line.DrawLine(Color.Black, arrowTop.Center, arrowBottom.Center);
            }
        }

        private void DrawColorBar(RectangleF box, Func<int, Color> colorModifier)
        {
            if (colorModifier == null)
            {
                throw new ArgumentException("Color Modifier Argument Invalid!");
            }
            for (int color = 0; color < 255; color++)
            {
                var linePos = this.GetLineStartPos(box, color);
                Line.DrawLine(colorModifier.Invoke(color), this.LineWidth + 1, linePos, this.SetVector(linePos, float.MaxValue, box.Bottom));
            }
        }

        #endregion
    }
}