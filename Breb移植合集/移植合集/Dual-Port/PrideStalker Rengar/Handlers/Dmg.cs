using EloBuddy;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.UI;
using LeagueSharp.SDK.Core.Utils;
using SharpDX;
using System;
using System.Linq;
using PrideStalker_Rengar.Main;

namespace PrideStalker_Rengar.Handlers
{
    class Dmg : Core
    {
        public static int IgniteDmg = 50 + 20 * GameObjects.Player.Level;

        public static float ComboDmg(Obj_AI_Base enemy)
        {
            if (enemy != null)
            {
                float damage = 0;
                if(MenuConfig.KillStealSummoner)
                {
                    if(Spells.Ignite.IsReady())
                    {
                        damage = damage + IgniteDmg;
                    }
                }
                if (Player.CanAttack) damage = damage + (float)Player.GetAutoAttackDamage(enemy);

                if (Spells.W.IsReady()) damage = damage + Spells.W.GetDamage(enemy);

                if (Spells.Q.IsReady()) damage = damage + Spells.Q.GetDamage(enemy) + (float)Player.GetAutoAttackDamage(enemy);

                if (Spells.Q.IsReady() && Player.Mana == 5) damage = damage + Spells.Q.GetDamage(enemy) + (float)Player.GetAutoAttackDamage(enemy);

                if (Player.Mana == 5) damage = damage + (float)Player.GetAutoAttackDamage(enemy) * 2;

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
