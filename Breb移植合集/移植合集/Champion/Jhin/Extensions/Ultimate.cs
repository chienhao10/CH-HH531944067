using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;

namespace Jhin___The_Virtuoso.Extensions
{
    internal static class Ultimate
    {
        public static void ComboUltimate()
        {
            if (ObjectManager.Player.IsActive(Spells.R))
            {
                if (Menus.getCheckBoxItem(Menus.miscMenu, "auto.shoot.bullets"))
                {
                    var tstarget = TargetSelector.GetTarget(Spells.R.Range, DamageType.Physical);
                    if (tstarget != null)
                    {
                        var pred = Spells.R.GetPrediction(tstarget);
                        if (pred.Hitchance >= Menus.miscMenu.HikiChance("r.hit.chance"))
                        {
                            Spells.R.Cast(tstarget);
                        }
                    }
                }
            }
            else
            {
                if (Spells.R.IsReady() && Menus.getKeyBindItem(Menus.miscMenu, "semi.manual.ult"))
                {
                    var tstarget = TargetSelector.GetTarget(Spells.R.Range, DamageType.Physical);
                    if (tstarget != null)
                    {
                        var pred = Spells.R.GetPrediction(tstarget);
                        if (pred.Hitchance >= Menus.miscMenu.HikiChance("r.hit.chance"))
                        {
                            Spells.R.Cast(tstarget);
                        }
                    }
                }
            }
        }
    }
}