using EloBuddy;
using LeagueSharp;
using LeagueSharp.Common;

namespace Nechrito_Gragas
{
    class Dmg
    {
        public static float ComboDmg(Obj_AI_Base enemy)
        {
            if (enemy != null)
            {
                float damage = 0;
                // if (Logic.HasItem()) damage = damage + (float)Program.Player.GetAutoAttackDamage(enemy) * 0.7f;
                if (Spells._w.IsReady()) damage = damage + Spells._w.GetDamage(enemy) +
                              (float)Program.Player.LSGetAutoAttackDamage(enemy);
                if (Spells._q.IsReady()) damage = damage + Spells._q.GetDamage(enemy);
                if (Spells._r.IsReady()) damage = damage + Spells._r.GetDamage(enemy);
                return damage;
            }
            return 0;
        }
        public static bool IsLethal(Obj_AI_Base unit)
        {
            return ComboDmg(unit) / 1.75 >= unit.Health;
        }
    
}
}
