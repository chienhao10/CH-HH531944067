namespace WreckingBall
{
    internal static class ChampInfo
    {
        public static SpellInfo Q { get; private set; }

        public static SpellInfo Q2 { get; private set; }

        public static SpellInfo W { get; private set; }

        public static SpellInfo W2 { get; private set; }

        public static SpellInfo E { get; private set; }

        public static SpellInfo E2 { get; private set; }

        public static SpellInfo R { get; private set; }

        internal class SpellInfo
        {
            public float Range { get; }

            public float Delay { get; }

            public float Speed { get; }

            public float Width { get; }

            public SpellInfo(float range, float delay, float speed, float width)
            {
                this.Range = range;
                this.Delay = delay;
                this.Speed = speed;
                this.Width = width;
            }
        }

        public static void InitSpells()
        {
            Q = new SpellInfo(1100, .25f, 1800, 65);
            Q2 = new SpellInfo(1300, .25f, 1400, 0);
            W = new SpellInfo(700, .25f, 1000, 0);
            W2 = new SpellInfo(float.MaxValue, 0, 0, 0);
            E = new SpellInfo(125, .25f, 1400, 0);
            E2 = new SpellInfo(float.MaxValue, .25f, 1000, 0);
            R = new SpellInfo(375, .25f, 1000, 0);
        }
    }
}
