using EloBuddy;
using EloBuddy.SDK;

// ReSharper disable InconsistentNaming

namespace AutoSharp.Auto.HowlingAbyss
{
    public static class HAManager
    {
        public static Orbwalker.OrbwalkPositionDelegate GetOrbPosDelegate()
        {
            //var vec = DecisionMaker.GetOrbPos;
            return () => DecisionMaker.GetOrbPos;
        }
        public static void Load()
        {
            Game.OnUpdate += DecisionMaker.OnUpdate;
            Orbwalker.OverrideOrbwalkPosition = GetOrbPosDelegate();
            ARAMShopAI.Main.Init(); 
        }

        public static void Unload()
        {
            Game.OnUpdate -= DecisionMaker.OnUpdate;
        }

        public static void FastHalt()
        {
            Orbwalker.ActiveModesFlags = Orbwalker.ActiveModes.None;
        }
    }
}
