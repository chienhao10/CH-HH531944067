namespace ElUtilitySuite.Summoners
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;    // ReSharper disable once ClassNeverInstantiated.Global
    using EloBuddy;
    public class Ignite : IPlugin
    {
        #region Public Properties

        public static Menu Menu { get; set; }

        #endregion

        #region Public Methods and Operators

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

        /// <summary>
        ///     Gets or sets the slot.
        /// </summary>
        /// <value>
        ///     The Smitespell
        /// </value>
        public Spell IgniteSpell { get; set; }

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public void CreateMenu(Menu rootMenu)
        {
            if (this.Player.GetSpellSlot("summonerdot") == SpellSlot.Unknown)
            {
                return;
            }

            var igniteMenu = rootMenu.AddSubMenu("点燃", "Ignite");
            {
                igniteMenu.Add("Ignite.Activated", new CheckBox("开启点燃"));
            }

            Menu = igniteMenu;
        }

        /// <summary>
        /// Loads this instance
        /// </summary>
        public void Load()
        {
            try
            {
                var igniteSlot = this.Player.GetSpellSlot("summonerdot");

                if (igniteSlot == SpellSlot.Unknown)
                {
                    return;
                }

                this.IgniteSpell = new Spell(igniteSlot);

                Game.OnUpdate += this.OnUpdate;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion

        #region Methods

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

        private void IgniteKs()
        {
            try
            {
                if (!getCheckBoxItem(Menu, "Ignite.Activated"))
                {
                    return;
                }

                var kSableEnemy =
                    HeroManager.Enemies.FirstOrDefault(
                        hero =>
                        hero.LSIsValidTarget(600) && !hero.IsZombie
                        && this.Player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite) > hero.Health);

                if (kSableEnemy != null)
                {
                    this.Player.Spellbook.CastSpell(this.IgniteSpell.Slot, kSableEnemy);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        /// <summary>
        ///     Fired when the game is updated.
        /// </summary>
        /// <param name="args">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void OnUpdate(EventArgs args)
        {
            try
            {
                if (this.Player.IsDead)
                {
                    return;
                }

                this.IgniteKs();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion
    }
}
