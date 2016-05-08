using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using VayneHunter_Reborn.Utility.MenuUtility;

namespace VayneHunter_Reborn.Skills.Condemn
{
    static class CondemnExtensions
    {
        public static bool IsCondemnable(this Obj_AI_Base target, Vector3 fromPosition)
        {
            var pushDistance = MenuGenerator.miscMenu["dz191.vhr.misc.condemn.pushdistance"].Cast<Slider>().CurrentValue;
            var targetPosition = target.ServerPosition;
            float checkDistance = pushDistance / 40f;
            var pushDirection = (targetPosition - ObjectManager.Player.ServerPosition).LSNormalized();
            for (int i = 0; i < 40; i++)
            {
                Vector3 finalPosition = targetPosition + (pushDirection * checkDistance * i);
                if (finalPosition.LSIsWall())
                {
                    return true;
                }
            }
            return false;
        }
    }
}
