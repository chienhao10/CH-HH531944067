namespace KappAzir.Modes
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;

    using Mario_s_Lib;
    using static Menus;
    using static SpellsManager;

    /// <summary>
    /// This mode will run when the key of the orbwalker is pressed
    /// </summary>
    internal class JungleClear
    {
        /// <summary>
        /// Put in here what you want to do when the mode is running
        /// </summary>
        public static void Execute()
        {
            if (Q.GetJungleMinion() == null)
            {
                return;
            }
            Q.RangeCheckSource = Player.Instance.ServerPosition;
            Q.SourcePosition = Orbwalker.AzirSoldiers.FirstOrDefault(s => s.IsAlly)?.ServerPosition;
            if (Orbwalker.AzirSoldiers.Count(s => s.IsAlly) >= 1)
            {
                Q.TryToCast(Q.GetJungleMinion().Position, JungleClearMenu);
            }
            W.TryToCast(W.GetJungleMinion().Position, JungleClearMenu);
        }
    }
}