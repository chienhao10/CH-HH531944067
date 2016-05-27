namespace ElUtilitySuite.Summoners
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy.SDK.Menu;
    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;
    public class Exhaust
    {
        /// <summary>
        ///     Gets or sets the exhaust spell.
        /// </summary>
        /// <value>
        ///     The exhaust spell.
        /// </value>
        public Spell ExhaustSpell { get; set; }

        public Menu Menu { get; set; }

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
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public void CreateMenu(Menu rootMenu)
        {
            if (this.Player.GetSpellSlot("summonerexhaust") == SpellSlot.Unknown)
            {
                return;
            }

            var exhaustMenu = rootMenu.AddSubMenu("虚弱", "Exhaust");
            {
                exhaustMenu.Add("Exhaust.Activated", new CheckBox("使用虚弱"));
                foreach (var x in ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy))
                {
                    exhaustMenu.Add("exhauston" + x.ChampionName, new CheckBox("为以下使用 " + x.ChampionName));
                }
            }

            this.Menu = exhaustMenu;
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            var exhaustSlot = this.Player.GetSpellSlot("summonerexhaust");

            if (exhaustSlot == SpellSlot.Unknown)
            {
                return;
            }

            this.ExhaustSpell = new Spell(exhaustSlot, 550);
        }
    }
}
