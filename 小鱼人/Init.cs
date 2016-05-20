#region

using System;
using EloBuddy;
using EloBuddy.SDK.Events;
using SimplisticTemplate.Champion.Fizz;

#endregion

namespace SimplisticTemplate
{
    internal static class Init
    {
        private static void Main()
        {
            // Once loading is complete we will now call Initialize()
            Loading.OnLoadingComplete += Initialize;
        }

        private static void Initialize(EventArgs args)
        {
            // AiO function but looks Sleek, so who cares.
            //Switches between your current champion
            switch (ObjectManager.Player.ChampionName.ToLower())
            {
                //Once it reached "Fizz" it will execute.
                case "fizz":
                    Fizz.Initialize();
                    break;
                default:
                    return;
            }
        }
    }
}