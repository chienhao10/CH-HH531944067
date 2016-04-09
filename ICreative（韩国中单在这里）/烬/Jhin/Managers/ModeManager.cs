using System.Linq;
using EloBuddy.SDK;

namespace Jhin.Managers
{
    public static class ModeManager
    {
        public static bool Combo
        {
            get { return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo); }
        }

        public static bool Harass
        {
            get { return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass); }
        }

        public static bool LaneClear
        {
            get { return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear); }
        }

        public static bool JungleClear
        {
            get { return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) && EntityManager.MinionsAndMonsters.Monsters.Any(o => AIO.MyHero.IsInAutoAttackRange(o)); }
        }

        public static bool LastHit
        {
            get { return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit); }
        }

        public static bool Flee
        {
            get { return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee); }
        }
    }
}