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
    using EloBuddy.SDK;
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

        #endregion

        #region Constructors and Destructors

        static AntiStealth()
        {
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
        ///     Gets or sets the spells.
        /// </summary>
        /// <value>
        ///     The spells.
        /// </value>
        public static List<AntiStealthSpell> Spells { get; set; }

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
            var protectMenu = rootMenu.AddSubMenu("Anti-Stealth", "AntiStealth");
            {
                protectMenu.Add("AntiStealthActive", new CheckBox("Place Pink Ward on Unit Stealth"));
            }

            this.Menu = protectMenu;
            random = new Random(Environment.TickCount);
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            this.Items = new List<AntiStealthRevealItem>
                             {
                                 new AntiStealthRevealItem { GetItem = () => ItemData.Vision_Ward.GetItem() },
                                 new AntiStealthRevealItem
                                     { GetItem = () => ItemData.Greater_Vision_Totem_Trinket.GetItem() }
                             };

            this.rengar = HeroManager.Enemies.Find(x => x.ChampionName.ToLower() == "rengar");

            GameObject.OnCreate += this.GameObject_OnCreate;
            Obj_AI_Base.OnProcessSpellCast += this.OnProcessSpellCast;
            Game.OnUpdate += this.OnUpdate;
        }

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

                foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy &&
                    x.ChampionName.ToLower().Contains("vayne") &&
                    x.Buffs.Any(y => y.Name == "VayneInquisition")))
                {
                    this.VayneBuffEndTime = hero.Buffs.First(x => x.Name == "VayneInquisition").EndTime;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            try
            {
                if (!sender.IsEnemy || !this.Menu["AntiStealthActive"].Cast<CheckBox>().CurrentValue || ObjectManager.Player.InFountain(LeagueSharp.Common.Utility.FountainType.OwnFountain))
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

                            var item =
                                this.Items.Select(x => x.Item).FirstOrDefault(x => x.IsInRange(hero) && x.IsReady());
                            if (item != null)
                            {
                                LeagueSharp.Common.Utility.DelayAction.Add(random.Next(100, 1000), () => item.Cast(this.Player.Position));
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

        public static Item Pink = new Item(ItemId.Vision_Ward, 550f);
        public static Item Sweep = new Item(ItemId.Sweeping_Lens_Trinket, getLevelSWEEP());
        public static Item Oracle = new Item(ItemId.Oracle_Alteration, 550f);

        public static float getLevelSWEEP()
        {
            var f = 0.0f;
            if (ObjectManager.Player.Level >= 1 && ObjectManager.Player.Level < 4)
            {
                f = 500f;
            }
            if (ObjectManager.Player.Level >= 4 && ObjectManager.Player.Level < 7)
            {
                f = 800f;
            }
            if (ObjectManager.Player.Level >= 7 && ObjectManager.Player.Level < 10)
            {
                f = 1100f;
            }
            if (ObjectManager.Player.Level >= 10 && ObjectManager.Player.Level < 13)
            {
                f = 1400f;
            }
            if (ObjectManager.Player.Level >= 13 && ObjectManager.Player.Level < 16)
            {
                f = 1700f;
            }
            if (ObjectManager.Player.Level >= 16)
            {
                f = 2000f;
            }
            return f;
        }

        /// <summary>
        ///     Vayne
        /// </summary>
        private AIHeroClient vayne;
        public float VayneBuffEndTime = 0;

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            try
            {
                var hero = sender as AIHeroClient;
                if (!sender.IsEnemy || hero == null || !this.Menu["AntiStealthActive"].Cast<CheckBox>().CurrentValue || ObjectManager.Player.InFountain(LeagueSharp.Common.Utility.FountainType.OwnFountain))
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

                var stealthChampion = Spells.FirstOrDefault(x => x.SDataName.Equals(args.SData.Name, StringComparison.OrdinalIgnoreCase));

                if (stealthChampion != null)
                {
                    var spellCastPosition = this.Player.LSDistance(args.End) > 600 ? this.Player.Position : args.End;
                    if (Pink.IsReady() && Pink.IsOwned())
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(random.Next(100, 1000), () => Pink.Cast(spellCastPosition));
                    }
                    if (Sweep.IsReady() && Sweep.IsOwned())
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(random.Next(100, 1000), () => Sweep.Cast(spellCastPosition));
                    }
                    if (Oracle.IsReady() && Oracle.IsOwned())
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(random.Next(100, 1000), () => Oracle.Cast());
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
            ///     Gets the item.
            /// </summary>
            /// <value>
            ///     The item.
            /// </value>
            public Items.Item Item
            {
                get
                {
                    return this.GetItem();
                }
            }

            #endregion
        }
    }
}