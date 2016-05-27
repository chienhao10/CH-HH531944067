namespace ElUtilitySuite.Summoners
{
    using System;
    using System.Linq;
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using LeagueSharp.Common;

    public static class IgniteExtensions
    {
        #region Public Methods and Operators

        public static bool IgniteCheck(this Obj_AI_Base hero)
        {
            if (Ignite.getCheckBoxItem("Ignite.shieldCheck"))
            {
                return !hero.HasBuff("summonerdot") || !hero.HasBuff("summonerbarrier") || !hero.HasBuff("BlackShield") || !hero.HasBuff("SivirShield") || !hero.HasBuff("BansheesVeil") || !hero.HasBuff("ShroudofDarkness");
            }

            return true;
        }

        #endregion
    }

    // ReSharper disable once ClassNeverInstantiated.Global
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
        public LeagueSharp.Common.Spell IgniteSpell { get; set; }

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        /// 

        public static bool getCheckBoxItem(string item)
        {
            return igniteMenu[item].Cast<CheckBox>().CurrentValue;
        }

        public static Menu rootMenu = ElUtilitySuite.Entry.menu;
        public static Menu igniteMenu;
        public void CreateMenu(Menu rootMenu)
        {
            if (this.Player.GetSpellSlot("summonerdot") == SpellSlot.Unknown)
            {
                return;
            }

            igniteMenu = rootMenu.AddSubMenu("Ignite", "Ignite");
            igniteMenu.Add("Ignite.Activated", new CheckBox("Ignite"));
            igniteMenu.Add("Ignite.shieldCheck", new CheckBox("Check for shields"));
        }

        public void Load()
        {
            try
            {
                var igniteSlot = this.Player.GetSpellSlot("summonerdot");

                if (igniteSlot == SpellSlot.Unknown)
                {
                    return;
                }

                this.IgniteSpell = new LeagueSharp.Common.Spell(igniteSlot);

                Game.OnUpdate += this.OnUpdate;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion

        #region Methods

        private void IgniteKs()
        {
            try
            {
                if (!getCheckBoxItem("Ignite.Activated"))
                {
                    return;
                }

                var kSableEnemy =
                    HeroManager.Enemies.FirstOrDefault(
                        hero =>
                        hero.IsValidTarget(550) && hero.IgniteCheck() && !hero.IsZombie
                        && this.Player.GetSummonerSpellDamage(hero, LeagueSharp.Common.Damage.SummonerSpell.Ignite) >= hero.Health);

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
