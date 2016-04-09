using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK;

namespace Auto_Carry_Vayne.Features.Utility
{
    class Summoners
    {
        #region Healme
        public static void Healme()
        {
            if (!Manager.MenuManager.Heal || Variables._Player.HealthPercent > Manager.MenuManager.HealHp) return;

            if (Variables._Player.CountEnemiesInRange(800) >= 1)
            {
                Manager.SpellManager.Heal.Cast();
            }
            if (Variables._Player.HasBuff("summonerdot"))
            {
                Manager.SpellManager.Heal.Cast();
            }
        }
        #endregion Heal
        #region Healally
        public static void Healally()
        {
            foreach (var ally in EntityManager.Heroes.Allies.Where(a => !a.IsDead))
            {
                if (ally.HealthPercent > Manager.MenuManager.HealAllyHp || !Manager.MenuManager.HealAlly) return;
                if (ally.CountEnemiesInRange(800) >= 1 && Variables._Player.Position.Distance(ally) < 600)
                {
                    Manager.SpellManager.Heal.Cast();
                }
            }
            foreach (
    var ally in EntityManager.Heroes.Allies.Where(a => !a.IsDead))
            {
                if (ally.HealthPercent > Manager.MenuManager.HealAllyHp || !Manager.MenuManager.HealAlly) return;
                if (Variables._Player.Position.Distance(ally) < 600 && ally.HasBuff("summonerdot"))
                {
                    Manager.SpellManager.Heal.Cast();
                }
            }
        }
        #endregion
    }
}
