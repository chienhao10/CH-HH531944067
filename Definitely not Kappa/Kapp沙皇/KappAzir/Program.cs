namespace KappAzir
{
    using System;

    using EloBuddy;
    using EloBuddy.SDK.Events;

    using Modes;

    internal class Program
    {
        /// <summary>
        /// This event is triggered when the game loads
        /// </summary>
        /// <param name="args"></param>
        public static void Execute()
        {
            try
            {
                //Put the name of the champion here
                if (Player.Instance.Hero != Champion.Azir)
                {
                    return;
                }

                SpellsManager.InitializeSpells();
                Menus.CreateMenu();
                ModeManager.InitializeModes();
                DrawingsManager.InitializeDrawings();
                Jumper.OnLoad();
            }
            catch (Exception e)
            {
                if (e.ToString().Contains("Mario"))
                {
                    Chat.Print("[KappAzir ERROR] Failed to Load addon Please Make sure you have Mario's Lib installed");
                    Console.Write("[KappAzir ERROR] Failed to Load addon Please Make sure you have Mario's Lib installed");
                }
            }
        }
    }
}