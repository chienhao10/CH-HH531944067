using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;

namespace ElUtilitySuite.Items.OffensiveItems
{
    internal class Youmuu : Item
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        public override ItemId Id
        {
            get
            {
                return ItemId.Youmuus_Ghostblade;
            }
        }

        /// <summary>
        ///     Gets or sets the name of the item.
        /// </summary>
        /// <value>
        ///     The name of the item.
        /// </value>
        public override string Name
        {
            get
            {
                return "Youmuu";
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Shoulds the use item.
        /// </summary>
        /// <returns></returns>
        /// 

        public bool getCheckBoxItem(string item)
        {
            return Menu[item].Cast<CheckBox>().CurrentValue;
        }

        public int getSliderItem(string item)
        {
            return Menu[item].Cast<Slider>().CurrentValue;
        }

        public bool getKeyBindItem(string item)
        {
            return Menu[item].Cast<KeyBind>().CurrentValue;
        }

        public override void CreateMenu()
        {
            Menu.AddGroupLabel("Youmuu's Ghostblade");
            Menu.Add("Youmuucombo", new CheckBox("Use on Combo"));
        }

        public override bool ShouldUseItem()
        {
            return getCheckBoxItem("Youmuucombo") && ComboModeActive && HeroManager.Enemies.Any(x => x.LSDistance(ObjectManager.Player) < Orbwalking.GetRealAutoAttackRange(ObjectManager.Player));
        }

        #endregion
    }
}