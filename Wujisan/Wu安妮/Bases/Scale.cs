using ScalingTypes = WuAIO.Managers.DamageManager.ScalingTypes;


namespace WuAIO.Bases
{
    class Scale
    {
        public readonly float[] scaling;
        public readonly ScalingTypes scalingType;

        public Scale(float[] scaling, ScalingTypes scalingType)
        {
            this.scaling = scaling;
            this.scalingType = scalingType;
        }
    }
}
