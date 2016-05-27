using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;

namespace ElUtilitySuite.Items.OffensiveItems
{
    internal class Botrk : Item
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
                return ItemId.Blade_of_the_Ruined_King;
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
                return "Blade of the Ruined King";
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
            Menu.AddGroupLabel("Botrk");
            Menu.Add("UseBotrkCombo", new CheckBox("Use on Combo"));
            Menu.Add("BotrkEnemyHp", new Slider("Use on Enemy Hp %", 100, 1));
        }

        /// <summary>
        ///     Shoulds the use item.
        /// </summary>
        /// <returns></returns>
        public override bool ShouldUseItem()
        {
            return getCheckBoxItem("UseBotrkCombo") && ComboModeActive && (HeroManager.Enemies.Any(x => x.HealthPercent < getSliderItem("BotrkEnemyHp") && x.LSDistance(Player) < 500));
        }

        /// <summary>
        ///     Uses the item.
        /// </summary>
        public override void UseItem()
        {
            LeagueSharp.Common.Items.UseItem((int)Id, HeroManager.Enemies.First(x => x.HealthPercent < getSliderItem("BotrkEnemyHp") && x.LSDistance(ObjectManager.Player) < 550));
        }

        #endregion
    }
}