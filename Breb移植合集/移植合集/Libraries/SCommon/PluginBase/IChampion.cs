using System;

namespace SCommon.PluginBase
{
    internal interface IChampion
    {
        /// <summary>
        ///     Sets champion spells
        /// </summary>
        void SetSpells();

        /// <summary>
        ///     OnUpdate event
        /// </summary>
        /// <param name="args">
        ///     <see cref="EventArgs" />
        /// </param>
        void Game_OnUpdate(EventArgs args);

        /// <summary>
        ///     OnDraw event
        /// </summary>
        /// <param name="args">
        ///     <see cref="EventArgs" />
        /// </param>
        void Drawing_OnDraw(EventArgs args);
    }
}