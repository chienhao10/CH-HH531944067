using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;

namespace ElUtilitySuite.Items.OffensiveItems
{
    internal class Hextech : Item
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
                return ItemId.Hextech_Gunblade;
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
                return "Hextech Gunblade";
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
            Menu.AddGroupLabel("Hextech Gunblade");
            Menu.Add("UseHextechCombo", new CheckBox("Use on Combo"));
            Menu.Add("HextechEnemyHp", new Slider("Use on Enemy Hp %", 70, 1));
        }

        /// <summary>
        ///     Shoulds the use item.
        /// </summary>
        /// <returns></returns>
        public override bool ShouldUseItem()
        {
            return getCheckBoxItem("UseHextechCombo") && ComboModeActive
                   && HeroManager.Enemies.Any(
                       x =>
                       x.HealthPercent < getSliderItem("HextechEnemyHp")
                       && x.LSDistance(ObjectManager.Player) < 700 && !x.IsDead && !x.IsZombie);
        }

        /// <summary>
        ///     Uses the item.
        /// </summary>
        public override void UseItem()
        {
            LeagueSharp.Common.Items.UseItem((int)Id, HeroManager.Enemies.FirstOrDefault(x => x.HealthPercent < getSliderItem("HextechEnemyHp") && x.LSDistance(ObjectManager.Player) < 700 && !x.IsDead && !x.IsZombie));
        }

        #endregion
    }
}