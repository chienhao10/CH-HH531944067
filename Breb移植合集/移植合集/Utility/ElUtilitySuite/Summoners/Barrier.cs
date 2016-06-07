namespace ElUtilitySuite.Summoners
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy.SDK.Menu;
    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;
    public class Barrier : IPlugin
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the barrier spell.
        /// </summary>
        /// <value>
        ///     The barrier spell.
        /// </value>
        public Spell BarrierSpell { get; set; }

        /// <summary>
        /// Gets or sets the menu.
        /// </summary>
        /// <value>
        /// The menu.
        /// </value>
        public Menu Menu { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        private AIHeroClient Player
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
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public void CreateMenu(Menu rootMenu)
        {
            if (this.Player.GetSpellSlot("summonerbarrier") == SpellSlot.Unknown)
            {
                return;
            }

            var barrierMenu = rootMenu.AddSubMenu("护盾", "Barrier");
            {
                barrierMenu.Add("Barrier.Activated", new CheckBox("启用护盾"));
                barrierMenu.Add("Barrier.HP", new Slider("使用百分比", 20, 1));
                barrierMenu.Add("Barrier.Damage", new Slider("受到伤害 % 启用护盾用", 20, 1));
            }

            this.Menu = barrierMenu;
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            var barrierSlot = this.Player.GetSpellSlot("summonerbarrier");

            if (barrierSlot == SpellSlot.Unknown)
            {
                return;
            }

            this.BarrierSpell = new Spell(barrierSlot, 550);

            Game.OnUpdate += Game_OnUpdate;
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

        #region Methods

        private void Game_OnUpdate(EventArgs args)
        {
            var barrierSlot = this.Player.GetSpellSlot("summonerbarrier");
            if (!getCheckBoxItem(this.Menu, "Barrier.Activated"))
            {
                return;
            }
            if (this.BarrierSpell.IsReady())
            {
                if (ObjectManager.Player.LSCountEnemiesInRange(700f) > 0 && HealthPrediction.GetHealthPrediction(ObjectManager.Player, (int)(1000 + Game.Ping / 2f)) <= ObjectManager.Player.MaxHealth / 6)
                {
                    ObjectManager.Player.Spellbook.CastSpell(barrierSlot);
                    return;
                }
            }
        }

        #endregion
    }
}
