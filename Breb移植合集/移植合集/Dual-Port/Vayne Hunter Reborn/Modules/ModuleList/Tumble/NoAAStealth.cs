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
            if (MenuGenerator.miscMenu["dz191.vhr.misc.tumble.ijava"].Cast<CheckBox>().CurrentValue)
            {
                Orbwalker.OnPreAttack += iJava;
            }
            else
            {
                Orbwalker.OnPreAttack += OW;
            }
        }

        private void iJava(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (!ShouldGetExecuted() || !args.Target.IsEnemy)
                return;

            if (Player.HasBuff("vaynetumblefade"))
            {
                var stealthtime = MenuGenerator.miscMenu["dz191.vhr.misc.tumble.noaastealth.duration"].Cast<Slider>().CurrentValue;
                var stealthbuff = Player.GetBuff("vaynetumblefade");
                if (stealthbuff.EndTime - Game.Time > stealthbuff.EndTime - stealthbuff.StartTime - stealthtime / 1000f)
                {
                    args.Process = false;
                }
            }
            else
            {
                args.Process = true;
            }
        }

        private void OW(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (ShouldGetExecuted() && ObjectManager.Player.Buffs.Any(m => m.Name.ToLower() == "vaynetumblefade"))
            {
                if (ObjectManager.Player.CountEnemiesInRange(1100f) <= 1 
                    || ObjectManager.Player.CountEnemiesInRange(1100f) < MenuGenerator.miscMenu["dz191.vhr.misc.tumble.noaa.enemies"].Cast<Slider>().CurrentValue || ObjectManager.Player.HealthPercent > MenuGenerator.miscMenu["dz191.vhr.misc.tumble.noaastealthex.hp"].Cast<Slider>().CurrentValue)
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
