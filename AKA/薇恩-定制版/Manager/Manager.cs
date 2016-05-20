using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Auto_Carry_Vayne.Features.Utility;

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

            //Less Fps drops i hope
            drawing.Load();

        }
    }
}
