namespace CowAwareness.Drawings
{
    using System;
    using System.Drawing;

    using CowAwareness.Features;

    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;

    public class Clock : Feature, IToggleFeature
    {
        #region Public Properties

        public override string Name
        {
            get
            {
                return "Clock";
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Disable()
        {
            Drawing.OnDraw -= this.Drawing_OnDraw;
        }

        public void Enable()
        {
            Drawing.OnDraw += this.Drawing_OnDraw;
        }

        #endregion

        #region Methods

        protected override void Initialize()
        {
            this.Menu.AddLabel("繪畫系統時間在遊戲畫面中");
            this.Menu.Add("topOffset", new Slider("Y", 75, 0, 500));
            this.Menu.Add("rightOffset", new Slider("X", 100, 0, 500));
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            Drawing.DrawText(
                Drawing.Width - this["rightOffset"].Cast<Slider>().CurrentValue,
                this["topOffset"].Cast<Slider>().CurrentValue,
                Color.Gold,
                DateTime.Now.ToShortTimeString());
        }

        #endregion
    }
}