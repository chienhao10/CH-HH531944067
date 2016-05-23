using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;
using EloBuddy;
namespace LeagueSharp.SDK.Core.Wrappers.Spells.Tracker.Skillshots
{
    public class _ZiggsR : Skillshot
    {
        #region Constructors and Destructors

        public _ZiggsR()
            : base("ZiggsR")
        {
        }

        #endregion

        #region Public Properties

        public int Delay => (int)(1500 + 1500 * this.EndPosition.Distance(this.StartPosition) / this.SData.Range);

        #endregion
    }
}