using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace ElUtilitySuite.Items.OffensiveItems
{
    internal class Cutlass : Item
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
                return ItemId.Bilgewater_Cutlass;
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
                return "Bilgewater Cutlass";
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
            Menu.AddGroupLabel("Cutlass");
            Menu.Add("UseCutlassCombo", new CheckBox("Use on Combo"));
            Menu.Add("CutlassMyHp", new Slider("Use on My Hp %", 100, 1));
        }

        /// <summary>
        ///     Shoulds the use item.
        /// </summary>
        /// <returns></returns>
        public override bool ShouldUseItem()
        {
            return getCheckBoxItem("UseCutlassCombo") && ComboModeActive && ObjectManager.Player.HealthPercent <= getSliderItem("CutlassMyHp");
        }

        /// <summary>
        ///     Uses the item.
        /// </summary>
        public override void UseItem()
        {
            LeagueSharp.Common.Items.UseItem((int)Id, TargetSelector.GetTarget(550, DamageType.Physical));
        }

        #endregion
    }
}
