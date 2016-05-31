#region

using Arcane_Ryze.Main;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.SDK;

#endregion

namespace Arcane_Ryze.Handler
{
    internal class Usables
    {
        public static void CastProtobelt()
        {
            if(!MenuConfig.UseItems)
            {
                return;
            }
            var Target = TargetSelector.SelectedTarget;
            if (Items.CanUseItem(3152) && Target.IsValidTarget())
            {
                Items.UseItem(3152, Target.ServerPosition);
            }

            if (Items.CanUseItem(3040)) // Got this from Exory, credits to him, didn't know how to see if health was going down
            {
                if (Health.GetPrediction(ObjectManager.Player, (int)(250 + Game.Ping / 2f)) <= ObjectManager.Player.MaxHealth / 4)
                {
                    Items.UseItem(3040);
                }
            }
        }
    }
}
