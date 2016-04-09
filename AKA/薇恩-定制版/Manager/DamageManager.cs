using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;

namespace Auto_Carry_Vayne.Manager
{
    class DamageManager
    {
        public static float Wdmg(Obj_AI_Base target)
        {
            var dmg = (SpellManager.W.Level * 10 + 10) + ((0.03 + (SpellManager.W.Level * 0.01)) * target.MaxHealth);
            return (float)dmg;

        }
    }
}
