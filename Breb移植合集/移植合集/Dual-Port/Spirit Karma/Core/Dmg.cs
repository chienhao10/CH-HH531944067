using EloBuddy;
using LeagueSharp.SDK;

namespace Spirit_Karma.Core
{
    class Dmg : Core
    {
        public static float ComboDmg(Obj_AI_Base enemy)
        {
            if (enemy != null)
            {
                float damage = 0;
                
                if (Player.CanAttack) damage = damage + (float)Player.GetAutoAttackDamage(enemy);

                if (Spells.W.IsReady()) damage = damage + Spells.W.GetDamage(enemy);

                if (Spells.R.IsReady()) damage = damage + Spells.Q.GetDamage(enemy);

                if (Spells.Q.IsReady()) damage = damage + Spells.Q.GetDamage(enemy);

                return damage;
            }
            return 0;
        }
        public static bool IsLethal(Obj_AI_Base unit)
        {
            return ComboDmg(unit) / 1.65 >= unit.Health;
        }
    }
}
