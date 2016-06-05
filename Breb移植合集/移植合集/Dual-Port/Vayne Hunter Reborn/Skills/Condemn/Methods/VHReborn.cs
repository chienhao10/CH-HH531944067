using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SPrediction;
using VayneHunter_Reborn.Utility;
using VayneHunter_Reborn.Utility.Helpers;
using VayneHunter_Reborn.Utility.MenuUtility;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;

namespace VayneHunter_Reborn.Skills.Condemn.Methods
{
    class VHReborn
    {

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        public static AIHeroClient GetTarget(Vector3 fromPosition)
        {
            if (ObjectManager.Player.ServerPosition.UnderTurret(true))
            {
                return null;
            }

            var pushDistance = MenuGenerator.miscMenu["dz191.vhr.misc.condemn.pushdistance"].Cast<Slider>().CurrentValue;

            foreach (var target in HeroManager.Enemies.Where(h => h.IsValidTarget(Variables.spells[SpellSlot.E].Range) && !h.HasBuffOfType(BuffType.SpellShield) && !h.HasBuffOfType(BuffType.SpellImmunity)))
            {
                var targetPosition = Vector3.Zero;

                var pred = Variables.spells[SpellSlot.E].GetPrediction(target);
                if (pred.Hitchance >= HitChance.VeryHigh)
                {
                    targetPosition = pred.UnitPosition;
                }

                if (targetPosition == Vector3.Zero)
                {
                    return null;
                }

                var finalPosition = targetPosition.Extend(fromPosition, -pushDistance);
                var numberOfChecks = (float)Math.Ceiling(pushDistance / 30f);


                if (getCheckBoxItem(MenuGenerator.miscMenu, "dz191.vhr.misc.condemn.onlystuncurrent") && Orbwalker.LastTarget != null &&
                            !target.NetworkId.Equals(Orbwalker.LastTarget.NetworkId))
                {
                    continue;
                }

                for (var i = 1; i <= 30; i++)
                {
                    var v3 = (targetPosition - fromPosition).Normalized();
                    var extendedPosition = targetPosition + v3 * (numberOfChecks * i);
                    var j4Flag = getCheckBoxItem(MenuGenerator.miscMenu, "dz191.vhr.misc.condemn.condemnflag") && (extendedPosition.IsJ4Flag(target));
                    if ((extendedPosition.IsWall() || j4Flag) && (target.Path.Count() < 2) && !target.LSIsDashing())
                    {

                        if (target.Health + 10 <=
                            ObjectManager.Player.GetAutoAttackDamage(target) *
                            getSliderItem(MenuGenerator.miscMenu, "dz191.vhr.misc.condemn.noeaa"))
                        {
                            return null;
                        }
                        
                        return target;
                    }
                }
            }
            return null;
        }
    }
}
