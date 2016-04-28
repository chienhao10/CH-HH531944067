namespace ElUtilitySuite.Summoners
{
    using System.Linq;

    using System;
    using EloBuddy;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using LeagueSharp.Common;

    public class Heal : IPlugin
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the heal spell.
        /// </summary>
        /// <value>
        ///     The heal spell.
        /// </value>
        public LeagueSharp.Common.Spell HealSpell { get; set; }

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

        public static bool getCheckBoxItem(string item)
        {
            return healMenu[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(string item)
        {
            return healMenu[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(string item)
        {
            return healMenu[item].Cast<KeyBind>().CurrentValue;
        }

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        /// 
        public static Menu rootMenu = ElUtilitySuite.Entry.menu;
        public static Menu healMenu;
        public void CreateMenu(Menu rootMenu)
        {
            if (this.Player.GetSpellSlot("summonerheal") == SpellSlot.Unknown)
            {
                return;
            }

            healMenu = rootMenu.AddSubMenu("Heal", "Heal");
            healMenu.Add("Heal.Activated", new CheckBox("Heal"));
            /*
            healMenu.Add("Heal.HP", new Slider("Health percentage", 20, 1));
            healMenu.Add("Heal.Damage", new Slider("Heal on % incoming damage", 20, 1));
            healMenu.AddSeparator();

            foreach (var x in ObjectManager.Get<AIHeroClient>().Where(x => x.IsAlly))
            {
                healMenu.Add("healon" + x.ChampionName, new CheckBox("Use for " + x.ChampionName));
            }
            */
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            try
            {
                var healSlot = this.Player.GetSpellSlot("summonerheal");

                if (healSlot == SpellSlot.Unknown)
                {
                    return;
                }

                this.HealSpell = new LeagueSharp.Common.Spell(healSlot, 550);

                //AttackableUnit.OnDamage += this.AttackableUnit_OnDamage;
                Game.OnUpdate += Game_OnUpdate;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        private void Game_OnUpdate(EventArgs args)
        {
            var healSlot = this.Player.GetSpellSlot("summonerheal");
            if (!getCheckBoxItem("Heal.Activated"))
            {
                return;
            }
            if (this.HealSpell.IsReady())
            {
                foreach (AIHeroClient ally in HeroManager.Allies.Where(a => a.LSIsValidTarget(850f, false) && a.LSCountEnemiesInRange(700f) > 0 && HealthPrediction.GetHealthPrediction(a, (int)(1000 + Game.Ping / 2f)) <= a.MaxHealth / 6))
                {
                    ObjectManager.Player.Spellbook.CastSpell(healSlot, ally);
                }
            }
        }

        #endregion

        #region Methods
        /*
        private void AttackableUnit_OnDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            try
            {
                if (!getCheckBoxItem("Heal.Activated"))
                {
                    return;
                }

                var obj = ObjectManager.GetUnitByNetworkId<Obj_AI_Base>((uint)args.Target.NetworkId);
                var source = ObjectManager.GetUnitByNetworkId<GameObject>((uint)args.Source.NetworkId);

                if (obj.Type != GameObjectType.AIHeroClient || source.Type != GameObjectType.AIHeroClient)
                {
                    return;
                }

                var hero = (AIHeroClient)obj;

                if (hero.IsEnemy || (!hero.IsMe && !this.HealSpell.IsInRange(obj)) || !getCheckBoxItem(string.Format("healon{0}", hero.ChampionName)))
                {
                    return;
                }

                if (((int)(args.Damage / hero.Health) > getSliderItem("Heal.Damage")) || (hero.HealthPercent < getSliderItem("Heal.HP")))
                {
                    this.Player.Spellbook.CastSpell(this.HealSpell.Slot);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
            }
            */
        #endregion
    }
}
