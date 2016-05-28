namespace ElUtilitySuite.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ItemData = LeagueSharp.Common.Data.ItemData;
    using EloBuddy;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    internal class AntiStealth : IPlugin
    {
        #region Static Fields

        /// <summary>
        ///     The random
        /// </summary>
        private static Random random;

        #endregion

        #region Fields

        /// <summary>
        ///     Rengar
        /// </summary>
        private AIHeroClient rengar;


        /// <summary>
        ///     Vayne
        /// </summary>
        private AIHeroClient vayne;

        #endregion

        #region Constructors and Destructors

        static AntiStealth()
        {
            // add akali health here
            Spells = new List<AntiStealthSpell>
                         {
                             new AntiStealthSpell { ChampionName = "Akali", SDataName = "akalismokebomb" },
                             new AntiStealthSpell { ChampionName = "Vayne", SDataName = "vayneinquisition" },
                             new AntiStealthSpell { ChampionName = "Twitch", SDataName = "hideinshadows" },
                             new AntiStealthSpell { ChampionName = "Shaco", SDataName = "deceive" },
                             new AntiStealthSpell { ChampionName = "Monkeyking", SDataName = "monkeykingdecoy" },
                             new AntiStealthSpell { ChampionName = "Khazix", SDataName = "khazixrlong" },
                             new AntiStealthSpell { ChampionName = "Khazix", SDataName = "khazixr" }
                         };
        }

        #endregion

        #region Delegates

        /// <summary>
        ///     Gets an anti stealth item.
        /// </summary>
        /// <returns></returns>
        private delegate Items.Item GetAntiStealthItemDelegate();

        #endregion

        #region Public Properties

        /// <summary>
        ///     A delegate that returns a <see cref="SpellSlot" />
        /// </summary>
        /// <returns>
        ///     <see cref="SpellSlot" />
        /// </returns>
        public delegate SpellSlot GetSlotDelegate();

        /// <summary>
        ///     The Vayne buff stealth end time
        /// </summary>
        public float VayneBuffEndTime = 0;

        /// <summary>
        ///     Gets or sets the spells.
        /// </summary>
        /// <value>
        ///     The spells.
        /// </value>
        public static List<AntiStealthSpell> Spells { get; set; }

        /// <summary>
        ///     Gets or sets the menu
        /// </summary>
        public Menu Menu { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the items.
        /// </summary>
        /// <value>
        ///     The items.
        /// </value>
        private List<AntiStealthRevealItem> Items { get; set; }

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
            var protectMenu = rootMenu.AddSubMenu("反隐形单位", "AntiStealth");
            {
                protectMenu.Add("AntiStealthActive", new CheckBox("插真眼显示隐形单位"));
            }

            this.Menu = protectMenu;
            random = new Random(Environment.TickCount);
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

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            if (Game.MapId != GameMapId.SummonersRift)
            {
                return;
            }

            this.Items = new List<AntiStealthRevealItem>
                             {
                                 new AntiStealthRevealItem
                                     {
                                     Slot = () =>
                                        {
                                            var slots = ItemData.Vision_Ward.GetItem().Slots;
                                            return slots.Count == 0 ? SpellSlot.Unknown : slots[0];
                                        },
                                         Priority = 0
                                     },
                                 new AntiStealthRevealItem
                                     {
                                     Slot = () =>
                                        {
                                            var slots = ItemData.Greater_Vision_Totem_Trinket.GetItem().Slots;
                                            return slots.Count == 0 ? SpellSlot.Unknown : slots[0];
                                        },
                                         Priority = 1
                                     }
                             };

            this.Items = this.Items.OrderBy(x => x.Priority).ToList();

            this.rengar = HeroManager.Enemies.Find(x => x.ChampionName.ToLower() == "rengar");

            GameObject.OnCreate += this.GameObject_OnCreate;
            Obj_AI_Base.OnProcessSpellCast += this.OnProcessSpellCast;
            Game.OnUpdate += this.OnUpdate;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Fired when the game is updated.
        /// </summary>
        /// <param name="args">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void OnUpdate(EventArgs args)
        {
            try
            {
                this.vayne = HeroManager.Enemies.Find(x => x.ChampionName.ToLower() == "vayne");
                if (this.vayne == null)
                {
                    return;
                }

                foreach (var hero in HeroManager.Enemies.Where(x => x.ChampionName.ToLower().Contains("vayne") && x.Buffs.Any(y => y.Name.ToLower().Contains("vayneinquisition"))))
                {
                    this.VayneBuffEndTime = hero.Buffs.First(x => x.Name.ToLower().Contains("vayneinquisition")).EndTime;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        /// <summary>
        ///     Gets the best ward item.
        /// </summary>
        /// <returns></returns>
        private Spell GetBestWardItem()
        {
            foreach (var item in this.Items.OrderBy(x => x.Priority))
            {
                if (!item.Spell.IsReady() || item.Spell.Slot == SpellSlot.Unknown)
                {
                    continue;
                }

                return item.Spell;
            }

            return null;
        }

        /// <summary>
        ///     Fired when a game object is created
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            try
            {
                if (!sender.IsEnemy || !getCheckBoxItem(this.Menu, "AntiStealthActive"))
                {
                    return;
                }

                if (this.rengar != null)
                {
                    if (sender.Name.Contains("Rengar_Base_R_Alert"))
                    {
                        if (this.Player.HasBuff("rengarralertsound") && !this.rengar.IsVisible && !this.rengar.IsDead && 
                            this.Player.LSDistance(sender.Position) < 1700)
                        {
                            var hero = (AIHeroClient)sender;

                            if (hero.IsAlly)
                            {
                                return;
                            }

                            var item = this.GetBestWardItem();
                            if (item != null)
                            {
                                LeagueSharp.Common.Utility.DelayAction.Add(
                                   random.Next(100, 1000),
                                   () =>
                                   this.Player.Spellbook.CastSpell(item.Slot, this.Player.Position));
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        /// <summary>
        ///     Fired when the game processes a spell cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GameObjectProcessSpellCastEventArgs" /> instance containing the event data.</param>
        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            try
            {
                var hero = sender as AIHeroClient;
                if (!sender.IsEnemy || hero == null || !getCheckBoxItem(this.Menu, "AntiStealthActive"))
                {
                    return;
                }

                if (this.Player.LSDistance(sender.Position) > 800)
                {
                    return;
                }

                if (args.SData.Name.ToLower().Contains("vaynetumble") && Game.Time > this.VayneBuffEndTime)
                {
                    return;
                }
                   
                var stealthChampion =
                Spells.FirstOrDefault(x => x.SDataName.Equals(args.SData.Name, StringComparison.OrdinalIgnoreCase));

                if (stealthChampion != null)
                { 
                    var item = this.GetBestWardItem();
                    if (item != null)
                    {
                        var spellCastPosition = this.Player.LSDistance(args.End) > 600 ? this.Player.Position : args.End;
                        LeagueSharp.Common.Utility.DelayAction.Add(
                            random.Next(100, 1000),
                            () => this.Player.Spellbook.CastSpell(item.Slot, spellCastPosition));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion

        /// <summary>
        ///     Represents a spell that an item should be casted on.
        /// </summary>
        public class AntiStealthSpell
        {
            #region Public Properties

            /// <summary>
            ///     Gets or sets the name of the champion.
            /// </summary>
            /// <value>
            ///     The name of the champion.
            /// </value>
            public string ChampionName { get; set; }

            /// <summary>
            ///     Gets or sets the name of the s data.
            /// </summary>
            /// <value>
            ///     The name of the s data.
            /// </value>
            public string SDataName { get; set; }

            #endregion
        }

        /// <summary>
        ///     Represents an item that can reveal stealthed units.
        /// </summary>
        private class AntiStealthRevealItem
        {
            #region Public Properties

            /// <summary>
            ///     Gets or sets the get item.
            /// </summary>
            /// <value>
            ///     The get item.
            /// </value>
            public GetAntiStealthItemDelegate GetItem { get; set; }

            /// <summary>
            ///     Gets or sets the priority.
            /// </summary>
            /// <value>
            ///     The priority.
            /// </value>
            public int Priority { get; set; }

            /// <summary>
            ///     Gets or sets the spell.
            /// </summary>
            /// <value>
            ///     The spell.
            /// </value>
            public Spell Spell
            {
                get
                {
                    return new Spell(this.Slot());
                }
            }

            /// <summary>
            ///     Gets or sets the slot delegate.
            /// </summary>
            /// <value>
            ///     The slot delegate.
            /// </value>
            public GetSlotDelegate Slot { get; set; }


            #endregion
        }
    }
}
