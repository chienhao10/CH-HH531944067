namespace AntiStealth
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ElUtilitySuite.Vendor.SFX;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ItemData = LeagueSharp.Common.Data.ItemData;
    using EloBuddy;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    public class AntiStealth
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

        public AntiStealth()
        {
            // add akali health here
            Spells = new List<AntiStealthSpell>
                         {
                             new AntiStealthSpell { ChampionName = "Akali", SDataName = "akalismokebomb" },
                             new AntiStealthSpell { ChampionName = "Twitch", SDataName = "hideinshadows" },
                             new AntiStealthSpell { ChampionName = "Shaco", SDataName = "deceive" },
                             new AntiStealthSpell { ChampionName = "Monkeyking", SDataName = "monkeykingdecoy" },
                             new AntiStealthSpell { ChampionName = "Khazix", SDataName = "khazixrlong" },
                             new AntiStealthSpell { ChampionName = "Khazix", SDataName = "khazixr" }
                         };

            Load();
        }

        #endregion

        #region Delegates

        /// <summary>
        ///     A delegate that returns a <see cref="SpellSlot" />
        /// </summary>
        /// <returns>
        ///     <see cref="SpellSlot" />
        /// </returns>
        public delegate SpellSlot GetSlotDelegate();

        /// <summary>
        ///     Gets an anti stealth item.
        /// </summary>
        /// <returns></returns>
        private delegate Items.Item GetAntiStealthItemDelegate();

        #endregion

        #region Public Properties

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
        public void CreateMenu()
        {
            protectMenu = MainMenu.AddMenu("反隐形单位", "AntiStealth");
            {
                protectMenu.Add("AntiStealthActive", new CheckBox("最隐身单位放置真眼"));
            }

            this.Menu = protectMenu;
            random = new Random(Environment.TickCount);
        }

        public Menu protectMenu;

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
                                                 var slots =
                                                     ItemData.Greater_Vision_Totem_Trinket.GetItem()
                                                         .Slots;
                                                 return slots.Count == 0
                                                            ? SpellSlot.Unknown
                                                            : slots[0];
                                             },
                                         Priority = 1
                                     }
                             };

            this.Items = this.Items.OrderBy(x => x.Priority).ToList();

            this.rengar =
                GameObjects.EnemyHeroes.FirstOrDefault(
                    e => e.ChampionName.Equals("Rengar", StringComparison.OrdinalIgnoreCase));

            this.vayne =
                GameObjects.EnemyHeroes.FirstOrDefault(
                    e => e.ChampionName.Equals("Vayne", StringComparison.OrdinalIgnoreCase));

            CreateMenu();
            GameObject.OnCreate += this.GameObject_OnCreate;
            Obj_AI_Base.OnProcessSpellCast += this.OnProcessSpellCast;
            Game.OnUpdate += this.OnUpdate;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Fired when a game object is created
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            try
            {
                if (!sender.IsEnemy || !protectMenu["AntiStealthActive"].Cast<CheckBox>().CurrentValue)
                {
                    return;
                }

                if (this.rengar != null)
                {
                    if (sender.Name.Contains("Rengar_Base_R_Alert"))
                    {
                        if (this.Player.HasBuff("rengarralertsound") && !this.rengar.IsVisible && !this.rengar.IsDead)
                        {
                            var item = this.GetBestWardItem();
                            if (item != null)
                            {
                                LeagueSharp.Common.Utility.DelayAction.Add(
                                    random.Next(100, 1000),
                                    () => this.Player.Spellbook.CastSpell(item.Slot, this.Player.Position));
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
        ///     Fired when the game processes a spell cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GameObjectProcessSpellCastEventArgs" /> instance containing the event data.</param>
        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            try
            {
                var hero = sender as AIHeroClient;
                if (!sender.IsEnemy || hero == null || !protectMenu["AntiStealthActive"].Cast<CheckBox>().CurrentValue)
                {
                    return;
                }

                if (this.Player.LSDistance(sender.Position) > 800)
                {
                    return;
                }

                if (this.vayne != null)
                {
                    var buff =
                        this.vayne.Buffs.FirstOrDefault(
                            b => b.Name.Equals("VayneInquisition", StringComparison.OrdinalIgnoreCase));

                    if (buff != null)
                    {
                        var item = this.GetBestWardItem();
                        if (item != null)
                        {
                            var spellCastPosition = this.Player.LSDistance(args.End) > 600
                                                        ? this.Player.Position
                                                        : args.End;

                            LeagueSharp.Common.Utility.DelayAction.Add(
                                random.Next(100, 1000),
                                () => this.Player.Spellbook.CastSpell(item.Slot, spellCastPosition));
                        }
                    }
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

        /// <summary>
        ///     Fired when the game is updated.
        /// </summary>
        /// <param name="args">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void OnUpdate(EventArgs args)
        {
            try
            {
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
            ///     Gets or sets the slot delegate.
            /// </summary>
            /// <value>
            ///     The slot delegate.
            /// </value>
            public GetSlotDelegate Slot { get; set; }

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

            #endregion
        }
    }
}