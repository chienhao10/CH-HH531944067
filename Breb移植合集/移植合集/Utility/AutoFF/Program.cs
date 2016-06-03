namespace AutoFF
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy.SDK.Menu;
    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;/// <summary>
                                   /// The program.
                                   /// </summary>
    internal class Program
    {
        /// <summary>
        /// The config.
        /// </summary>
        private static Menu config;

        /// <summary>
        /// The last surrender time.
        /// </summary>
        /// <returns>
        /// The surrender time.
        /// </returns>
        private static float lastSurrenderTime;

        /// <summary>
        /// Should I say Surrender?
        /// </summary>
        /// <param name="gameTime">
        /// Current Game Time.
        /// </param>
        /// <returns>
        /// Returns if I should surrender.
        /// </returns>
        private static bool Surrender(float gameTime)
        {
            return (gameTime + 30) >= lastSurrenderTime;
        }

        /// <summary>
        /// Called when Game Loads
        /// </summary>
        /// <param name="args">
        /// The Args
        /// </param>
        public static void Game_OnGameLoad()
        {
            config = MainMenu.AddMenu("自动投降", "menu");

            config.Add("toggle", new CheckBox("自动投降时间设置"));
            config.Add("time", new Slider("投降时间", 20, 15, 120));

            Game.OnUpdate += Game_OnUpdate;
            Game.OnNotify += Game_OnNotify;
        }

        /// <summary>
        /// Called when the Game has a notification
        /// </summary>
        /// <param name="args">
        /// The Args
        /// </param>
        private static void Game_OnNotify(GameNotifyEventArgs args)
        {
            if (string.Equals(args.EventId.ToString(), "OnSurrenderVote") || args.EventId == GameEventId.OnSurrenderVote)
            {
                Chat.Say("/ff");
            }
        }

        /// <summary>
        /// Called each time the Game Updates
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void Game_OnUpdate(EventArgs args)
        {
            var time = config["time"].Cast<Slider>().CurrentValue;

            if (Game.Time >= time * 60 && config["toggle"].Cast<CheckBox>().CurrentValue && Surrender(Game.Time))
            {
                Chat.Say("/ff");
                lastSurrenderTime = Game.Time;
            }
        }
    }
}
