namespace ElEasy
{
    using EloBuddy.SDK.Menu;
    using LeagueSharp.Common;

    /// <summary>
    ///     An interface for a utility plugin.
    /// </summary>
    internal interface IPlugin
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        void CreateMenu(Menu rootMenu);

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        void Load();

        #endregion
    }
}