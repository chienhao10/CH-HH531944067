using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy.SDK;
using EloBuddy;

namespace Auto_Carry_Vayne.Features.Module.Misc
{
    class AutoLantern : IModule
    {
        private const String LanternName = "ThreshLantern";

        public void OnLoad()
        {

        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public bool ShouldGetExecuted()
        {
            return Manager.MenuManager.AutoLantern && Manager.MenuManager.AutoLanternS <= Variables._Player.HealthPercent && Variables.ThreshInGame();
        }

        public void OnExecute()
        {
            var lantern =
                ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(o => o.IsValid && o.IsAlly && o.Name.Equals(LanternName));

            if (lantern != null && Variables._Player.Distance(lantern) <= 500 && Variables._Player.Spellbook.GetSpell((SpellSlot)62).Name.Equals("LanternWAlly"))
            {
               Variables._Player.Spellbook.CastSpell((SpellSlot)62, lantern);
            }
        }
    }
}