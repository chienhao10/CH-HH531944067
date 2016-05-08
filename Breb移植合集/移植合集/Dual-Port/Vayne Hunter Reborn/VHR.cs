using System;
using System.Linq;
using LeagueSharp;
using VayneHunter_Reborn.Modules.ModuleHelpers;
using VayneHunter_Reborn.Skills.Condemn;
using VayneHunter_Reborn.Skills.Tumble;
using VayneHunter_Reborn.Skills.Tumble.VHRQ;
using VayneHunter_Reborn.Utility;
using EloBuddy;

namespace VayneHunter_Reborn
{
    class VHR
    {
        public static void OnLoad()
        {
            TumbleLogic.OnLoad();
            CondemnLogic.OnLoad();

            foreach (var module in Variables.moduleList)
            {
                module.OnLoad();
            }

            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs args)
        {
            CondemnLogic.Execute(args);

            foreach (var module in Variables.moduleList.Where(module => module.GetModuleType() == ModuleType.OnUpdate && module.ShouldGetExecuted()))
            {
                module.OnExecute();
            }
        }
    }
}
