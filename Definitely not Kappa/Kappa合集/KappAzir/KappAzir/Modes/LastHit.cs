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
    internal class LastHit
    {
        /// <summary>
        /// Put in here what you want to do when the mode is running
        /// </summary>
        public static void Execute()
        {
            if (Q.GetLastHitMinion() == null)
            {
                return;
            }
            Q.RangeCheckSource = Player.Instance.ServerPosition;
            Q.SourcePosition = Orbwalker.AzirSoldiers.FirstOrDefault(s => s.IsAlly)?.ServerPosition;
            if (Orbwalker.AzirSoldiers.Any(x => x != null) && Q.IsReady())
            {
                Q.TryToCast(Q.GetLastHitMinion(), LasthitMenu);
            }
        }
    }
}