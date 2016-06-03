using LeagueSharp.Common;

namespace UnderratedAIO.Helpers
{
    internal class AutoLeveler
    {
        public static AutoLevel autoLevel;
        public int[] LevelingOrder = new int[] { 0, 1, 2, 0, 0, 3, 0, 1, 0, 1, 3, 1, 1, 2, 2, 3, 2, 2 };

        public AutoLeveler(int[] tree)
        {
            autoLevel = new AutoLevel(tree);
            AutoLevel.Enable();
        }
    }
}