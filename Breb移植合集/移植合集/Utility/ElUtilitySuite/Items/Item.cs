namespace ElUtilitySuite.Items
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK;/// <summary>
                       ///     Represents an item.
                       /// </summary>
    internal abstract class Item
    {
        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether the combo mode is active.
        /// </summary>
        /// <value>
        ///     <c>true</c> if combo mode is active; otherwise, <c>false</c>.
        /// </value>
        public bool ComboModeActive
        {
            get
            {
                return Entry.getCombo() || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
            }
        }

        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        public virtual ItemId Id
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        public Menu Menu { get; set; }

        /// <summary>
        ///     Gets or sets the name of the item.
        /// </summary>
        /// <value>
        ///     The name of the item.
        /// </value>
        public virtual string Name
        {
            get
            {
                return "Unknown Item";
            }
        }

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        public AIHeroClient Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        public virtual void CreateMenu()
        {
            this.Menu.AddGroupLabel(Name);
            this.Menu.Add(this.Name + "combo", new CheckBox("Use in Combo"));
            this.Menu.AddSeparator();
        }

        /// <summary>
        ///     Shoulds the use item.
        /// </summary>
        /// <returns></returns>
        public virtual bool ShouldUseItem()
        {
            return false;
        }

        /// <summary>
        ///     Uses the item.
        /// </summary>
        public virtual void UseItem()
        {
            Items.UseItem((int)this.Id);
        }

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        #endregion
    }
}
