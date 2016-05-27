using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;

namespace ElUtilitySuite.Items.OffensiveItems
{
    class FrostQueen : Item
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
                return ItemId.Frost_Queens_Claim;
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
                return "Frost Queen's Claim";
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
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
            Menu.AddGroupLabel("Frost Queen's Claim");
            Menu.Add("UseFrostQueenCombo", new CheckBox("Use on Combo"));
            Menu.Add("FrostQueenEnemyHp", new Slider("Use on Enemy Hp %", 70, 1));
            Menu.Add("FrostQueenMyHp", new Slider("Use on My Hp %", 100, 1));
        }

        /// <summary>
        ///     Shoulds the use item.
        /// </summary>
        /// <returns></returns>
        public override bool ShouldUseItem()
        {
            return getCheckBoxItem("UseFrostQueenCombo") && ComboModeActive
                   && (HeroManager.Enemies.Any(
                       x =>
                       x.HealthPercent < getSliderItem("FrostQueenEnemyHp")
                       && x.LSDistance(ObjectManager.Player) < 1500)
                       || ObjectManager.Player.HealthPercent < getSliderItem("FrostQueenMyHp"));
        }

        /// <summary>
        ///     Uses the item.
        /// </summary>
        public override void UseItem()
        {
            LeagueSharp.Common.Items.UseItem((int)Id);
        }

        #endregion
    }
}