using System;

namespace LeagueSharp.Common
{
    /// <summary>
    ///     The menu string list.
    /// </summary>
    [Serializable]
    public struct StringList
    {
        #region Fields

        /// <summary>
        ///     The selected index.
        /// </summary>
        public int SelectedIndex;

        /// <summary>
        ///     The string list.
        /// </summary>
        public string[] SList;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="StringList" /> struct.
        /// </summary>
        /// <param name="stringList">
        ///     The string list.
        /// </param>
        /// <param name="defaultIndex">
        ///     The default index.
        /// </param>
        public StringList(string[] stringList, int defaultIndex = 0)
        {
            SList = stringList;
            SelectedIndex = defaultIndex;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the selected value.
        /// </summary>
        public string SelectedValue
        {
            get
            {
                return SelectedIndex >= 0 && SelectedIndex < SList.Length
                    ? SList[SelectedIndex]
                    : string.Empty;
            }
        }

        #endregion
    }
}