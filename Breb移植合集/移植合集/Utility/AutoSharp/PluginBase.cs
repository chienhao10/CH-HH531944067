using System;
using System.Drawing;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace AutoSharp
{

    #region

    #endregion

    /// <summary>
    ///     PluginBase class
    /// </summary>
    public abstract class PluginBase
    {
        /// <summary>
        ///     Init BaseClass
        /// </summary>
        protected PluginBase()
        {
            InitConfig();
            InitPluginEvents();
        }

        /// <summary>
        ///     Champion Name
        /// </summary>
        public string ChampionName { get; set; }

        /// <summary>
        ///     ComboMode
        /// </summary>
        public bool ComboMode
        {
            get { return true; }
        }

        /// <summary>
        ///     HarassMode
        /// </summary>
        public bool HarassMode
        {
            get { return false; }
        }

        /// <summary>
        ///     HarassMana
        /// </summary>
        public bool HarassMana
        {
            get { return Player.ManaPercent > ManaConfig["HarassMana"].Cast<Slider>().CurrentValue; }
        }

        /// <summary>
        ///     UsePackets
        /// </summary>
        public bool UsePackets
        {
            get { return false; /* 4.21 ConfigValue<bool>("UsePackets"); */ }
        }

        /// <summary>
        ///     Player Object
        /// </summary>
        public AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        /// <summary>
        ///     AttackRange
        /// </summary>
        public float AttackRange
        {
            get { return Orbwalking.GetRealAutoAttackRange(Target); }
        }

        /// <summary>
        ///     Target
        /// </summary>
        public AIHeroClient Target
        {
            get { return TargetSelector.GetTarget(1200, DamageType.Magical); }
        }

        /// <summary>
        ///     OrbwalkerTarget
        /// </summary>
        public AttackableUnit OrbwalkerTarget
        {
            get { return Orbwalker.LastTarget; }
        }

        /// <summary>
        ///     Q
        /// </summary>
        public LeagueSharp.Common.Spell Q { get; set; }

        /// <summary>
        ///     W
        /// </summary>
        public LeagueSharp.Common.Spell W { get; set; }

        /// <summary>
        ///     E
        /// </summary>
        public LeagueSharp.Common.Spell E { get; set; }

        /// <summary>
        ///     R
        /// </summary>
        public LeagueSharp.Common.Spell R { get; set; }

        /// <summary>
        ///     Config
        /// </summary>
        public static Menu Config { get; set; }

        /// <summary>
        ///     ComboConfig
        /// </summary>
        public Menu ComboConfig { get; set; }

        /// <summary>
        ///     HarassConfig
        /// </summary>
        public Menu HarassConfig { get; set; }

        /// <summary>
        ///     MiscConfig
        /// </summary>
        public Menu MiscConfig { get; set; }

        /// <summary>
        ///     ManaConfig
        /// </summary>
        public Menu ManaConfig { get; set; }

        /// <summary>
        ///     DrawingConfig
        /// </summary>
        public Menu DrawingConfig { get; set; }

        /// <summary>
        ///     InterruptConfig
        /// </summary>
        public Menu InterruptConfig { get; set; }

        /// <summary>
        ///     OnProcessPacket
        /// </summary>
        /// <remarks>
        ///     override to Implement OnProcessPacket logic
        /// </remarks>
        /// <param name="args"></param>
        public virtual void OnProcessPacket(GamePacketEventArgs args) { }

        /// <summary>
        ///     OnSendPacket
        /// </summary>
        /// <remarks>
        ///     override to Implement OnSendPacket logic
        /// </remarks>
        /// <param name="args"></param>
        public virtual void OnSendPacket(GamePacketEventArgs args) { }

        /// <summary>
        ///     OnPossibleToInterrupt
        /// </summary>
        /// <remarks>
        ///     override to Implement SpellsInterrupt logic
        /// </remarks>
        /// <param name="unit">Obj_AI_Base</param>
        /// <param name="spell">InterruptableSpell</param>
        public virtual void OnPossibleToInterrupt(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs spell) { }

        /// <summary>
        ///     OnEnemyGapcloser
        /// </summary>
        /// <remarks>
        ///     override to Implement AntiGapcloser logic
        /// </remarks>
        /// <param name="gapcloser">ActiveGapcloser</param>
        public virtual void OnEnemyGapcloser(ActiveGapcloser gapcloser) { }

        /// <summary>
        ///     OnUpdate
        /// </summary>
        /// <remarks>
        ///     override to Implement Update logic
        /// </remarks>
        /// <param name="args">EventArgs</param>
        public virtual void OnUpdate(EventArgs args) { }

        /// <summary>
        ///     OnBeforeAttack
        /// </summary>
        /// <remarks>
        ///     override to Implement OnBeforeAttack logic
        /// </remarks>
        /// <param name="args">Orbwalking.BeforeAttackEventArgs</param>
        public virtual void OnBeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args) { }

        /// <summary>
        ///     OnAfterAttack
        /// </summary>
        /// <remarks>
        ///     override to Implement OnAfterAttack logic
        /// </remarks>
        /// <param name="unit">unit</param>
        /// <param name="target">target</param>
        public virtual void OnAfterAttack(AttackableUnit target, EventArgs args) { }

        /// <summary>
        ///     OnLoad
        /// </summary>
        /// <remarks>
        ///     override to Implement class Initialization
        /// </remarks>
        /// <param name="args">EventArgs</param>
        public virtual void OnLoad(EventArgs args) { }

        /// <summary>
        ///     OnDraw
        /// </summary>
        /// <remarks>
        ///     override to Implement Drawing
        /// </remarks>
        /// <param name="args">EventArgs</param>
        public virtual void OnDraw(EventArgs args) { }

        /// <summary>
        ///     ComboMenu
        /// </summary>
        /// <remarks>
        ///     override to Implement ComboMenu Config
        /// </remarks>
        /// <param name="config">Menu</param>
        public virtual void ComboMenu(Menu config) { }

        /// <summary>
        ///     HarassMenu
        /// </summary>
        /// <remarks>
        ///     override to Implement HarassMenu Config
        /// </remarks>
        /// <param name="config">Menu</param>
        public virtual void HarassMenu(Menu config) { }

        /// <summary>
        ///     ManaMenu
        /// </summary>
        /// <remarks>
        ///     override to Implement ManaMenu Config
        /// </remarks>
        /// <param name="config">Menu</param>
        public virtual void ManaMenu(Menu config) { }

        /// <summary>
        ///     MiscMenu
        /// </summary>
        /// <remarks>
        ///     override to Implement MiscMenu Config
        /// </remarks>
        /// <param name="config">Menu</param>
        public virtual void MiscMenu(Menu config) { }

        /// <summary>
        ///     MiscMenu
        /// </summary>
        /// <remarks>
        ///     override to Implement Interrupt Config
        /// </remarks>
        /// <param name="config">Menu</param>
        public virtual void InterruptMenu(Menu config) { }

        /// <summary>
        ///     DrawingMenu
        /// </summary>
        /// <remarks>
        ///     override to Implement DrawingMenu Config
        /// </remarks>
        /// <param name="config">Menu</param>
        public virtual void DrawingMenu(Menu config) { }

        #region Private Stuff

        /// <summary>
        ///     PluginEvents Initialization
        /// </summary>
        private void InitPluginEvents()
        {
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Orbwalker.OnPreAttack += OnBeforeAttack;
            Orbwalker.OnPostAttack += OnAfterAttack;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += OnPossibleToInterrupt;
            //Game.OnGameSendPacket += OnSendPacket;
            //Game.OnGameProcessPacket += OnProcessPacket;
            OnLoad(new EventArgs());
        }

        /// <summary>
        ///     Config Initialization
        /// </summary>
        private void InitConfig()
        {
            ChampionName = Player.ChampionName;

            Config = Program.Config;

            ComboConfig = Config.AddSubMenu("Combo", "Combo");
            HarassConfig = Config.AddSubMenu("Harass", "Harass");
            ManaConfig = Config.AddSubMenu("Mana Limiter", "Mana Limiter");
            MiscConfig = Config.AddSubMenu("Misc", "Misc");
            InterruptConfig = Config.AddSubMenu("Interrupt", "Interrupt");
            DrawingConfig = Config.AddSubMenu("Drawings", "Drawings");

            // mana
            ManaConfig.AddSlider("HarassMana", "Harass Mana %", 1, 1, 100);

            // misc
            MiscConfig.AddList("AttackMinions", "Attack Minions?", new[] { "Smart", "Never", "Always" });
            MiscConfig.AddBool("AttackChampions", "Attack Champions?", true);

            // drawing
            DrawingConfig.Add("Target" + ChampionName, new CheckBox("Target"));//.SetValue(new Circle(true, Color.DodgerBlue)));
            DrawingConfig.Add("QRange" + ChampionName, new CheckBox("Q Range", false));//.SetValue(new Circle(false, Color.FromArgb(150, Color.DodgerBlue))));
            DrawingConfig.Add("WRange" + ChampionName, new CheckBox("W Range", false));//.SetValue(new Circle(false, Color.FromArgb(150, Color.DodgerBlue))));
            DrawingConfig.Add("ERange" + ChampionName, new CheckBox("E Range", false));//.SetValue(new Circle(false, Color.FromArgb(150, Color.DodgerBlue))));
            DrawingConfig.Add("RRange" + ChampionName, new CheckBox("R Range", false));//.SetValue(new Circle(false, Color.FromArgb(150, Color.DodgerBlue))));

            // plugins
            ComboMenu(ComboConfig);
            HarassMenu(HarassConfig);
            ManaMenu(ManaConfig);
            MiscMenu(MiscConfig);
            InterruptMenu(InterruptConfig);
            DrawingMenu(DrawingConfig);
        }

        #endregion
    }
}