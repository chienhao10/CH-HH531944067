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
    class Shine
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

        public static Obj_AI_Base GetTarget(Vector3 fromPosition)
        {
            foreach (var target in HeroManager.Enemies.Where(h => h.IsValidTarget(Variables.spells[SpellSlot.E].Range)))
            {
                var pushDistance = MenuGenerator.miscMenu["dz191.vhr.misc.condemn.pushdistance"].Cast<Slider>().CurrentValue;
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

                var pushDirection = (targetPosition - ObjectManager.Player.ServerPosition).Normalized();
                float checkDistance = pushDistance / 40f;
                for (int i = 0; i < 40; i++)
                {
                    Vector3 finalPosition = targetPosition + (pushDirection * checkDistance * i);
                    var collFlags = NavMesh.GetCollisionFlags(finalPosition);
                    var j4Flag = getCheckBoxItem(MenuGenerator.miscMenu, "dz191.vhr.misc.condemn.condemnflag") && (finalPosition.IsJ4Flag(target));
                    if (collFlags.HasFlag(CollisionFlags.Wall) || collFlags.HasFlag(CollisionFlags.Building) || j4Flag) //not sure about building, I think its turrets, nexus etc
                    {
                        if (getCheckBoxItem(MenuGenerator.miscMenu, "dz191.vhr.misc.condemn.onlystuncurrent") && Orbwalker.LastTarget != null &&
                                        !target.NetworkId.Equals(Orbwalker.LastTarget.NetworkId))
                        {
                            return null;
                        }

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
