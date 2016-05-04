// This file is part of LeagueSharp.Common.
// 
// LeagueSharp.Common is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// LeagueSharp.Common is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with LeagueSharp.Common.  If not, see <http://www.gnu.org/licenses/>.

using EloBuddy;
using LeagueSharp.Common;

namespace iDZed.Utils
{
    internal static class ZedDamage
    {
        private static float GetPassiveDamage(AIHeroClient target)
        {
            double totalDamage = 0;

            if (ObjectManager.Player.Level > 16)
            {
                var targetHealth = target.MaxHealth*.1;
                totalDamage += ObjectManager.Player.CalcDamage(target, DamageType.Magical, targetHealth);
            }
            else if (ObjectManager.Player.Level > 6)
            {
                var targetHealth = target.MaxHealth*0.8;
                totalDamage += ObjectManager.Player.CalcDamage(target, DamageType.Magical, targetHealth);
            }
            else
            {
                var targetHealth = target.MaxHealth*0.6;
                totalDamage += ObjectManager.Player.CalcDamage(target, DamageType.Magical, targetHealth);
            }

            return (float) totalDamage;
        }

        private static float GetDeathmarkDamage(AIHeroClient target)
        {
            double totalDamage = 0;

            if (Zed._spells[SpellSlot.R].IsReady() || target.HasBuff("zedulttargetmark"))
            {
                totalDamage += Zed._spells[SpellSlot.R].GetDamage(target);

                switch (Zed._spells[SpellSlot.R].Level)
                {
                    case 1:
                        totalDamage += totalDamage*1.2;
                        break;
                    case 2:
                        totalDamage += totalDamage*1.35;
                        break;
                    case 3:
                        totalDamage += totalDamage*1.5;
                        break;
                }
            }

            return (float) totalDamage;
        }

        public static float GetTotalDamage(AIHeroClient target)
        {
            double totalDamage = 0;

            if (Zed._spells[SpellSlot.Q].IsReady()) // TODO calculate 2 or 3 q's depending on shadows kappa
            {
                totalDamage += Zed._spells[SpellSlot.Q].GetDamage(target)*2; // shadow logic pls
            }

            if (Zed._spells[SpellSlot.E].IsReady())
            {
                totalDamage += Zed._spells[SpellSlot.E].GetDamage(target)*2; // Same shadow situation
            }

            if (target.HealthPercent <= 50)
            {
                totalDamage += GetPassiveDamage(target);
            }

            if (Zed._spells[SpellSlot.R].IsReady())
            {
                totalDamage += GetDeathmarkDamage(target);
            }

            return (float) totalDamage;
        }
    }
}