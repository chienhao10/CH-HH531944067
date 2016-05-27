namespace ElUtilitySuite
{
    using LeagueSharp.Common;

    /// <summary>
    ///     An interface for a utility plugin.
    /// </summary>
    internal interface IPlugin
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        void Load();

        #endregion

        void CreateMenu(EloBuddy.SDK.Menu.Menu menu);
    }
}