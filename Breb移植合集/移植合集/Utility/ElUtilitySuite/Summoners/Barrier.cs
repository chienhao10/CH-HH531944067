namespace ElUtilitySuite.Summoners
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using LeagueSharp.Common;
    using System;
    public class Barrier : IPlugin
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the barrier spell.
        /// </summary>
        /// <value>
        ///     The barrier spell.
        /// </value>
        public LeagueSharp.Common.Spell BarrierSpell { get; set; }

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
        public static Menu rootMenu = ElUtilitySuite.Entry.menu;
        public static Menu barrierMenu;

        public static bool getCheckBoxItem(string item)
        {
            return barrierMenu[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(string item)
        {
            return barrierMenu[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(string item)
        {
            return barrierMenu[item].Cast<KeyBind>().CurrentValue;
        }

        public void CreateMenu(Menu rootMenu)
        {
            if (this.Player.GetSpellSlot("summonerbarrier") == SpellSlot.Unknown)
            {
                return;
            }

            barrierMenu = rootMenu.AddSubMenu("Barrier", "Barrier");
            barrierMenu.Add("Barrier.Activated", new CheckBox("Barrier activated"));
            //barrierMenu.Add("Barrier.HP", new Slider("Barrier percentage", 20, 1));
            //barrierMenu.Add("Barrier.Damage", new Slider("Barrier on damage dealt %", 20, 1));
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

            this.BarrierSpell = new LeagueSharp.Common.Spell(barrierSlot, 550);

            //AttackableUnit.OnDamage += this.AttackableUnit_OnDamage;
            Game.OnUpdate += Game_OnUpdate;
        }

        private void Game_OnUpdate(EventArgs args)
        {
            var barrierSlot = this.Player.GetSpellSlot("summonerbarrier");
            if (!getCheckBoxItem("Barrier.Activated"))
            {
                return;
            }
            if (this.BarrierSpell.IsReady())
            {
                if (ObjectManager.Player.CountEnemiesInRange(700f) > 0 && HealthPrediction.GetHealthPrediction(ObjectManager.Player, (int)(1000 + Game.Ping / 2f)) <= ObjectManager.Player.MaxHealth / 6)
                {
                    ObjectManager.Player.Spellbook.CastSpell(barrierSlot);
                    return;
                }
            }
        }

        #endregion

        #region Methods
        /*
        private void AttackableUnit_OnDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            if (!getCheckBoxItem("Barrier.Activated"))
            {
                return;
            }

            var obj = ObjectManager.GetUnitByNetworkId<Obj_AI_Base>((uint)args.Source.NetworkId);
            var source = ObjectManager.GetUnitByNetworkId<GameObject>((uint)args.Source.NetworkId);

            if (obj.Type != GameObjectType.AIHeroClient || source.Type != GameObjectType.AIHeroClient)
            {
                return;
            }

            var hero = (AIHeroClient)obj;

            if (!hero.IsMe)
            {
                return;
            }

            if (((int)(args.Damage / this.Player.MaxHealth * 100) > getSliderItem("Barrier.Damage") || this.Player.HealthPercent < getSliderItem("Barrier.HP")) && this.Player.CountEnemiesInRange(1000) >= 1)
            {
                this.BarrierSpell.Cast();
            }
        }
        */
        #endregion
    }
}
