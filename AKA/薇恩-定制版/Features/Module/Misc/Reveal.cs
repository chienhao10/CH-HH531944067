using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK;
using EloBuddy;

namespace Auto_Carry_Vayne.Features.Module.Misc
{
    class Reveal : IModule
    {
        private static readonly Item Trinket = new Item(3364, 600f);

        public void OnLoad()
        {
            Manager.StealthManager.OnStealth += StealthHelper_OnStealth;
        }

        void StealthHelper_OnStealth(Manager.StealthManager.OnStealthEventArgs obj)
        {
            //Using First the Trinket then the vision ward.

            if (Manager.MenuManager.AutoTrinket)
            {
                if (obj.IsStealthed
                    && obj.Sender.IsEnemy
                    && obj.Sender.ServerPosition.Distance(ObjectManager.Player.ServerPosition) <= 600f)
                {
                    var objectPosition = obj.Sender.ServerPosition;
                    if (Trinket.IsOwned() && Trinket.IsReady())
                    {
                        var extend = ObjectManager.Player.ServerPosition.Extend(objectPosition, 400f);
                        Trinket.Cast(extend);
                        return;
                    }

                    if (Manager.SpellManager.totem.IsOwned() && Manager.SpellManager.totem.IsReady())
                    {
                        var extend = ObjectManager.Player.ServerPosition.Extend(objectPosition, 400f);
                        Manager.SpellManager.totem.Cast(extend);
                    }
                }
            }
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public bool ShouldGetExecuted()
        {
            return false;
        }

        public void OnExecute() { }
    }
}
