namespace ElUtilitySuite.Items.OffensiveItems
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy;
    class HextechGLP_800 : Item
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
                return (ItemId)3030;
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
                return "Hextech GLP 800";
            }
        }

        #endregion

        #region Public Methods and Operators

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

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        public override void CreateMenu()
        {
            Menu.AddGroupLabel("HextechGLP - 800");
            Menu.Add("UseHextech800Combo", new CheckBox("Use on Combo"));
            Menu.Add("Hextech800EnemyHp", new Slider("Use on Enemy Hp %", 70));
        }

        /// <summary>
        ///     Shoulds the use item.
        /// </summary>
        /// <returns></returns>
        public override bool ShouldUseItem()
        {
            return getCheckBoxItem("UseHextech800Combo") && this.ComboModeActive && HeroManager.Enemies.Any(x => x.HealthPercent < getSliderItem("Hextech800EnemyHp") && x.LSDistance(this.Player) < 700 && !x.IsDead && !x.IsZombie);
        }

        /// <summary>
        ///     Uses the item.
        /// </summary>
        public override void UseItem()
        {
            var objAiHero = HeroManager.Enemies.FirstOrDefault(
                x =>
                x.HealthPercent < getSliderItem("Hextech800EnemyHp") && x.LSDistance(this.Player) < 500 && !x.IsDead && !x.IsZombie);

            if (objAiHero != null)
            {
                Items.UseItem((int)this.Id, objAiHero.ServerPosition);
            }
        }

        #endregion
    }
}