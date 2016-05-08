using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using VayneHunter_Reborn.Modules.ModuleHelpers;
using VayneHunter_Reborn.Utility;
using VayneHunter_Reborn.Utility.MenuUtility;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace VayneHunter_Reborn.Modules.ModuleList.Tumble
{
    class NoAAStealth : IModule
    {
        public void OnLoad()
        {
            Orbwalker.OnPreAttack += OW;
        }

        private void OW(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (ShouldGetExecuted() && ObjectManager.Player.Buffs.Any(m => m.Name.ToLower() == "vaynetumblefade"))
            {
                if (ObjectManager.Player.CountEnemiesInRange(1100f) <= 1 
                    || ObjectManager.Player.CountEnemiesInRange(1100f) < MenuGenerator.miscMenu["dz191.vhr.misc.tumble.noaa.enemies"].Cast<Slider>().CurrentValue)
                {
                    return;
                }


                args.Process = false;
            }
        }

        public bool ShouldGetExecuted()
        {
            return MenuGenerator.miscMenu["dz191.vhr.misc.tumble.noaastealthex"] != null 
                && MenuGenerator.miscMenu["dz191.vhr.misc.tumble.noaastealthex"].Cast<KeyBind>().CurrentValue;
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.Other;
        }

        public void OnExecute()
        {
        }
    }
}
