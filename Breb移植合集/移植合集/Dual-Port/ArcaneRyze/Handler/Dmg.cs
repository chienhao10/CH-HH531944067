#region

using EloBuddy;
using LeagueSharp.SDK;
using static Arcane_Ryze.Core;

#endregion

namespace Arcane_Ryze.Handler
{
    internal class Dmg
    {
        public static int IgniteDmg = 50 + 20 * GameObjects.Player.Level;
        public static float QDmg(Obj_AI_Base e)
        {
            if (e != null)
            {
                float damage = 0;
                if (Spells.Q.IsReady()) damage = damage + Spells.Q.GetDamage(e);
                return damage;
            }
            return 0;
        }
        public static float WDmg(Obj_AI_Base e)
        {
            if (e != null)
            {
                float damage = 0;
                if (Spells.W.IsReady()) damage = damage + Spells.W.GetDamage(e);
                return damage;
            }
            return 0;
        }
        public static float EDmg(Obj_AI_Base e)
        {
            if (e != null)
            {
                float damage = 0;
                if (Spells.E.IsReady()) damage = damage + Spells.E.GetDamage(e);
                return damage;
            }
            return 0;
        }
        public static bool EasyFuckingKillKappa(Obj_AI_Base unit)
        {
            return EDmg(unit) + QDmg(unit) + WDmg(unit) / 1.65 >= unit.Health;
        }
    }
}
