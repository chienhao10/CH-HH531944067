using EloBuddy.SDK.Menu;
using LeagueSharp;
using VayneHunter_Reborn.Utility.MenuUtility;

namespace VayneHunter_Reborn.Utility
{
    class VHRBootstrap
    {
        public static void OnLoad()
        {
            Variables.Menu = MainMenu.AddMenu("薇恩猎手 重生","dz191.vhr");

            SPrediction.Prediction.Initialize(Variables.Menu);
            MenuGenerator.OnLoad();
            VHR.OnLoad();
            DrawManager.OnLoad();
        }
    }
}
