namespace CowAwareness.Drawings
{
    using CowAwareness.Features;

    using EloBuddy;

    public class WatermarkDisabler : Feature, IToggleFeature
    {
        #region Public Properties

        public override string Name
        {
            get
            {
                return "Disable EB Watermark";
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Disable()
        {
            Hacks.RenderWatermark = true;
        }

        public void Enable()
        {
            Hacks.RenderWatermark = false;
        }

        #endregion

        #region Methods

        protected override void Initialize()
        {
        }

        #endregion
    }
}