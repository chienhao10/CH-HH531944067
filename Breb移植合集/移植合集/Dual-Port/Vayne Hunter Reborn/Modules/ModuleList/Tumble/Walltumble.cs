using EloBuddy;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using VayneHunter_Reborn.Modules.ModuleHelpers;
using VayneHunter_Reborn.Utility;
using VayneHunter_Reborn.Utility.Helpers;
using VayneHunter_Reborn.Utility.MenuUtility;

namespace VayneHunter_Reborn.Modules.ModuleList.Tumble
{
    class WallTumble : IModule
    {
        public void OnLoad()
        {

        }

        public bool ShouldGetExecuted()
        {
            return false;
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public void OnExecute()
        {
            return;
            var Player = ObjectManager.Player;

            Vector2 drakeWallQPos = new Vector2(11514, 4462);
            Vector2 midWallQPos = new Vector2(6667, 8794);

            if (Player.LSDistance(midWallQPos) >= Player.LSDistance(drakeWallQPos))
            {

                if (Player.Position.X < 12000 || Player.Position.X > 12070 || Player.Position.Y < 4800 ||
                    Player.Position.Y > 4872)
                {
                    PlayerHelper.MoveToLimited(new Vector2(12050, 4827).To3D());
                }
                else
                {
                    PlayerHelper.MoveToLimited(new Vector2(12050, 4827).To3D());
                    LeagueSharp.Common.Utility.DelayAction.Add((int)(106 + Game.Ping / 2f), () =>
                    {
                        Variables.spells[SpellSlot.Q].Cast(drakeWallQPos);
                    });
                }
            }
            else
            {
                if (Player.Position.X < 6908 || Player.Position.X > 6978 || Player.Position.Y < 8917 ||
                    Player.Position.Y > 8989)
                {
                    PlayerHelper.MoveToLimited(new Vector2(6962, 8952).To3D());
                }
                else
                {
                    PlayerHelper.MoveToLimited(new Vector2(6962, 8952).To3D());
                    LeagueSharp.Common.Utility.DelayAction.Add((int)(106 + Game.Ping / 2f), () =>
                    {
                        Variables.spells[SpellSlot.Q].Cast(midWallQPos);

                    });
                }
            }
        }
    }
}
