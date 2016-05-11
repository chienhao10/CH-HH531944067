namespace KappAzir.Modes
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;

    /// <summary>
    /// This mode will always run
    /// </summary>
    internal class Active : ModeManager
    {
        /// <summary>
        /// Put in here what you want to do when the mode is running
        /// </summary>
        public static void Execute()
        {
            if (SpellsManager.R.IsReady() && Game.Time - InSec.LastQTime > 0.1f && Game.Time - InSec.LastQTime < 1
                && InSec.target.IsValidTarget(SpellsManager.R.Range))
            {
                SpellsManager.R.Cast(Azir.Position.Extend(InSec.rpos, SpellsManager.R.Range).To3D());
            }

            SpellsManager.R.Width = 107 * SpellsManager.R.Level < 200 ? 220 : (107 * SpellsManager.R.Level) + 5;

            int count = Orbwalker.AzirSoldiers.Count(s => s.IsAlly);
            SpellsManager.Q.Width = count != 0 ? 65 * count : 65;
        }
    }
}