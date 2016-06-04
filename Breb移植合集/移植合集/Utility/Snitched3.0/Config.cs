namespace Snitched
{
    using System.Reflection;

    using LeagueSharp.Common;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;    /// <summary>
                                       ///     Handles configuration settings.
                                       /// </summary>
    internal class Config
    {
        #region Static Fields

        /// <summary>
        ///     The instance
        /// </summary>
        private static Config instance;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Prevents a default instance of the <see cref="Config" /> class from being created.
        /// </summary>
        private Config()
        {
            this.Menu = MainMenu.AddMenu("Snitched 3.0", "Snitched3");
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the instance.
        /// </summary>
        /// <value>
        ///     The instance.
        /// </value>
        public static Config Instance => instance ?? (instance = new Config());

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        public Menu Menu { get; set; }

        public Menu buffMenu, objectiveMenu, ksMenu, miscMenu;

        #endregion

        #region Public Indexers

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        public void CreateMenu()
        {
            buffMenu = this.Menu.AddSubMenu("偷Buff设置", "BuffStealSettings");
            AddSpellsToMenu(buffMenu, "偷Buff");
            buffMenu.Add("StealBlueBuff", new CheckBox("抢蓝"));
            buffMenu.Add("StealRedBuff", new CheckBox("抢红"));
            buffMenu.Add("StealAllyBuffs", new CheckBox("抢友军Buff", false));

            objectiveMenu = this.Menu.AddSubMenu("偷目标野怪", "ObjectiveStealSettings");
            AddSpellsToMenu(objectiveMenu, "目标");
            objectiveMenu.Add("StealBaron", new CheckBox("偷 男爵"));
            objectiveMenu.Add("StealDragon", new CheckBox("偷 龙"));
            objectiveMenu.Add("SmartObjectiveSteal", new CheckBox("智能偷"));
            objectiveMenu.Add("StealObjectiveKeyBind", new KeyBind("开启偷目标野怪", false, KeyBind.BindTypes.HoldActive, 90));

            ksMenu = this.Menu.AddSubMenu("抢头", "KillStealingSettings");
            ksMenu.AddGroupLabel("抢头目标 : ");
            HeroManager.Enemies.ForEach(x => ksMenu.Add("KS" + x.ChampionName, new CheckBox("KS : " + x.ChampionName)));
            AddSpellsToMenu(ksMenu, "KS");
            ksMenu.Add("DontStealOnCombo", new CheckBox("连招时不抢头"));

            miscMenu = this.Menu.AddSubMenu("杂项", "MiscSettings");
            miscMenu.Add("ETALimit", new Slider("弹道到达时间 (毫秒)", 3000, 0, 15000));
            miscMenu.Add("DistanceLimit", new Slider("距离限制", 5000, 0, 15000));
            miscMenu.Add("StealFOW", new CheckBox("在战争迷雾中偷怪", false));
            miscMenu.Add("DrawADPS", new CheckBox("显示平均 FPS"));
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Adds the spells to menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <param name="name">The name.</param>
        private static void AddSpellsToMenu(Menu rootMenu, string name)
        {
            SpellLoader.GetUsableSpells().ForEach(x => rootMenu.Add(name + x.Slot, new CheckBox("Use " + x.Slot)));
        }

        #endregion
    }
}