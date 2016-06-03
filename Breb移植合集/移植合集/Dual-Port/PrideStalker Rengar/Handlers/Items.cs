using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PrideStalker_Rengar.Handlers
{
    class ITEM : Core
    {
      //  public static readonly int[] BlueSmite = { 3706, 1400, 1401, 1402, 1403 };
      //  public static readonly int[] RedSmite = { 3715, 1415, 1414, 1413, 1412 };
        public static void CastProtobelt()
        {
            var Target = TargetSelector.GetTarget(1000, DamageType.Physical);
            if (Items.CanUseItem(3152) && Target.IsValidTarget())
            {
                Items.UseItem(3152, Player.ServerPosition.Extend(Target.ServerPosition, Player.AttackRange));
            }
        }
        public static void CastHydra()
        {
            if(Items.CanUseItem(3074))
            {
                    Items.UseItem(3074);
            }
            if (Items.CanUseItem(3077))
            {
                    Items.UseItem(3077);
            }
        }
        public static void CastYomu()
        {
            if (Items.CanUseItem(3142))
            {
                if (ObjectManager.Player.Spellbook.IsAutoAttacking ||
                    GameObjects.Player.IsCastingInterruptableSpell())
                {
                    Items.UseItem(3142);
                }
            }
        }
    }
}
