using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;

namespace Evade
{
    static class Helpers
    {
        public static bool IsSpellShielded(this AIHeroClient unit)
        {
            if (ObjectManager.Player.HasBuffOfType(BuffType.SpellShield))
            {
                return true;
            }

            if (ObjectManager.Player.HasBuffOfType(BuffType.SpellImmunity))
            {
                return true;
            }

            //Sivir E
            if (unit.LastCastedSpellName() == "SivirE" && (Environment.TickCount - unit.LastCastedSpellT()) < 300)
            {
                return true;
            }

            //Morganas E
            if (unit.LastCastedSpellName() == "BlackShield" && (Environment.TickCount - unit.LastCastedSpellT()) < 300)
            {
                return true;
            }

            //Nocturnes E
            if (unit.LastCastedSpellName() == "NocturneShit" && (Environment.TickCount - unit.LastCastedSpellT()) < 300)
            {
                return true;
            }

            return false;
        }
    }
}
