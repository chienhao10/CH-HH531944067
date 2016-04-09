using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto_Carry_Vayne.Manager
{
    class Manager
    {
        public static void Load()
        {
            //SpellManager
            SpellManager.Load();
            //MenuManager
            MenuManager.Load();
            //EventManager
            EventManager.Load();
            //ObjectManager

        }
    }
}
