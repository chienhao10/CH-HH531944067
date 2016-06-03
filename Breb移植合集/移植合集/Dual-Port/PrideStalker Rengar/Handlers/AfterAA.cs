using EloBuddy;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.UI;
using LeagueSharp.SDK.Core.Utils;
using SharpDX;
using System;
using System.Linq;
using PrideStalker_Rengar.Main;
using EloBuddy.SDK;

namespace PrideStalker_Rengar.Handlers
{
    class AfterAA : Core
    {
        public static void Orbwalker_OnPostAttack(AttackableUnit dgfg, EventArgs args)
        {
            if (dgfg is AIHeroClient)
            {
                var Target = dgfg as AIHeroClient;
      
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {

                if (Player.Mana == 5 && MenuConfig.Passive)
                {
                    return;
                }
                if (Mode.getBoxItem(MenuConfig.comboMenu, "ComboMode") != 2)
                {
                    if (Spells.Q.IsReady() && Player.HealthPercent >= 80 && Player.Mana == 5)
                    {
                        Spells.Q.Cast(Target);
                    }
                    if (Player.Mana < 5)
                    {
                        Spells.Q.Cast(Target);
                    }
                }
                if(Mode.getBoxItem(MenuConfig.comboMenu, "ComboMode") == 2)
                {
                    if(Player.Mana < 5)
                    {
                        Spells.Q.Cast(Target);
                    }
                }
            }
        }
      }
    }
}
