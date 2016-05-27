namespace ElUtilitySuite.Trackers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EloBuddy;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using SharpDX;
    using LeagueSharp.Common;
    using SharpDX.Direct3D9;

    /// <summary>
    ///     Tracks jungle camps.
    /// </summary>
    internal class JungleTracker : IPlugin
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes the <see cref="JungleTracker" /> class.
        /// </summary>
        static JungleTracker()
        {
            #region Jungle Camp Data

            JungleCamps = new List<JungleCamp>
                              {
                                  new JungleCamp(
                                      75000,
                                      new Vector3(6078.15f, 6094.45f, -98.63f),
                                      new[] { "TT_NWolf3.1.1", "TT_NWolf23.1.2", "TT_NWolf23.1.3" },
                                      LeagueSharp.Common.Utility.Map.MapType.TwistedTreeline,
                                      GameObjectTeam.Order),
                                  new JungleCamp(
                                      100000,
                                      new Vector3(6943.41f, 5422.61f, 52.62f),
                                      new[] { "SRU_Razorbeak3.1.1", "SRU_RazorbeakMini3.1.2", "SRU_RazorbeakMini3.1.3" },
                                      LeagueSharp.Common.Utility.Map.MapType.SummonersRift,
                                      GameObjectTeam.Order),
                                  new JungleCamp(
                                      100000,
                                      new Vector3(2164.34f, 8383.02f, 51.78f),
                                      new[] { "SRU_Gromp13.1.1" },
                                      LeagueSharp.Common.Utility.Map.MapType.SummonersRift,
                                      GameObjectTeam.Order),
                                  new JungleCamp(
                                      100000,
                                      new Vector3(8370.58f, 2718.15f, 51.09f),
                                      new[] { "SRU_Krug5.1.2", "SRU_KrugMini5.1.1" },
                                      LeagueSharp.Common.Utility.Map.MapType.SummonersRift,
                                      GameObjectTeam.Order),
                                  new JungleCamp(
                                      180000,
                                      new Vector3(4285.04f, 9597.52f, -67.6f),
                                      new[] { "SRU_Crab16.1.1" },
                                      LeagueSharp.Common.Utility.Map.MapType.SummonersRift,
                                      GameObjectTeam.Neutral),
                                  new JungleCamp(
                                      100000,
                                      new Vector3(6476.17f, 12142.51f, 56.48f),
                                      new[] { "SRU_Krug11.1.2", "SRU_KrugMini11.1.1" },
                                      LeagueSharp.Common.Utility.Map.MapType.SummonersRift,
                                      GameObjectTeam.Chaos),
                                  new JungleCamp(
                                      75000,
                                      new Vector3(11025.95f, 5805.61f, -107.19f),
                                      new[] { "TT_NWraith4.1.1", "TT_NWraith24.1.2", "TT_NWraith24.1.3" },
                                      LeagueSharp.Common.Utility.Map.MapType.TwistedTreeline,
                                      GameObjectTeam.Chaos),
                                  new JungleCamp(
                                      100000,
                                      new Vector3(10983.83f, 8328.73f, 62.22f),
                                      new[] { "SRU_Murkwolf8.1.1", "SRU_MurkwolfMini8.1.2", "SRU_MurkwolfMini8.1.3" },
                                      LeagueSharp.Common.Utility.Map.MapType.SummonersRift,
                                      GameObjectTeam.Chaos),
                                  new JungleCamp(
                                      100000,
                                      new Vector3(12671.83f, 6306.6f, 51.71f),
                                      new[] { "SRU_Gromp14.1.1" },
                                      LeagueSharp.Common.Utility.Map.MapType.SummonersRift,
                                      GameObjectTeam.Chaos),
                                  new JungleCamp(
                                      360000,
                                      new Vector3(7738.3f, 10079.78f, -61.6f),
                                      new[] { "TT_Spiderboss8.1.1" },
                                      LeagueSharp.Common.Utility.Map.MapType.SummonersRift,
                                      GameObjectTeam.Neutral),
                                  new JungleCamp(
                                      300000,
                                      new Vector3(3800.99f, 7883.53f, 52.18f),
                                      new[] { "SRU_Blue1.1.1", "SRU_BlueMini1.1.2", "SRU_BlueMini21.1.3" },
                                      LeagueSharp.Common.Utility.Map.MapType.SummonersRift,
                                      GameObjectTeam.Order),
                                  new JungleCamp(
                                      75000,
                                      new Vector3(4373.14f, 5842.84f, -107.14f),
                                      new[] { "TT_NWraith1.1.1", "TT_NWraith21.1.2", "TT_NWraith21.1.3" },
                                      LeagueSharp.Common.Utility.Map.MapType.TwistedTreeline,
                                      GameObjectTeam.Order),
                                  new JungleCamp(
                                      300000,
                                      new Vector3(4993.14f, 10491.92f, -71.24f),
                                      new[] { "SRU_RiftHerald" },
                                      LeagueSharp.Common.Utility.Map.MapType.SummonersRift,
                                      GameObjectTeam.Neutral),
                                  new JungleCamp(
                                      75000,
                                      new Vector3(5106.94f, 7985.9f, -108.38f),
                                      new[] { "TT_NGolem2.1.1", "TT_NGolem22.1.2" },
                                      LeagueSharp.Common.Utility.Map.MapType.TwistedTreeline,
                                      GameObjectTeam.Order),
                                  new JungleCamp(
                                      100000,
                                      new Vector3(7852.38f, 9562.62f, 52.3f),
                                      new[] { "SRU_Razorbeak9.1.1", "SRU_RazorbeakMini9.1.2", "SRU_RazorbeakMini9.1.3" },
                                      LeagueSharp.Common.Utility.Map.MapType.SummonersRift,
                                      GameObjectTeam.Chaos),
                                  new JungleCamp(
                                      300000,
                                      new Vector3(10984.11f, 6960.31f, 51.72f),
                                      new[] { "SRU_Blue7.1.1", "SRU_BlueMini7.1.2", "SRU_BlueMini27.1.3" },
                                      LeagueSharp.Common.Utility.Map.MapType.SummonersRift,
                                      GameObjectTeam.Chaos),
                                  new JungleCamp(
                                      180000,
                                      new Vector3(10647.7f, 5144.68f, -62.81f),
                                      new[] { "SRU_Crab15.1.1" },
                                      LeagueSharp.Common.Utility.Map.MapType.SummonersRift,
                                      GameObjectTeam.Neutral),
                                  new JungleCamp(
                                      75000,
                                      new Vector3(9294.02f, 6085.41f, -96.7f),
                                      new[] { "TT_NWolf6.1.1", "TT_NWolf26.1.2", "TT_NWolf26.1.3" },
                                      LeagueSharp.Common.Utility.Map.MapType.TwistedTreeline,
                                      GameObjectTeam.Chaos),
                                  new JungleCamp(
                                      420000,
                                      new Vector3(4993.14f, 10491.92f, -71.24f),
                                      new[] { "SRU_Baron12.1.1" },
                                      LeagueSharp.Common.Utility.Map.MapType.SummonersRift,
                                      GameObjectTeam.Neutral),
                                  new JungleCamp(
                                      100000,
                                      new Vector3(3849.95f, 6504.36f, 52.46f),
                                      new[] { "SRU_Murkwolf2.1.1", "SRU_MurkwolfMini2.1.2", "SRU_MurkwolfMini2.1.3" },
                                      LeagueSharp.Common.Utility.Map.MapType.SummonersRift,
                                      GameObjectTeam.Order),
                                  new JungleCamp(
                                      300000,
                                      new Vector3(7813.07f, 4051.33f, 53.81f),
                                      new[] { "SRU_Red4.1.1", "SRU_RedMini4.1.2", "SRU_RedMini4.1.3" },
                                      LeagueSharp.Common.Utility.Map.MapType.SummonersRift,
                                      GameObjectTeam.Order),
                                  new JungleCamp(
                                      360000,
                                      new Vector3(9813.83f, 4360.19f, -71.24f),
                                      new[] { "SRU_Dragon_Elder6.5.1", "SRU_Dragon_Air6.1.1", "SRU_Dragon_Fire6.2.1", "SRU_Dragon_Water6.3.1", "SRU_Dragon_Elder6.5.1" },
                                      LeagueSharp.Common.Utility.Map.MapType.SummonersRift,
                                      GameObjectTeam.Neutral),
                                  new JungleCamp(
                                      300000,
                                      new Vector3(7139.29f, 10779.34f, 56.38f),
                                      new[] { "SRU_Red10.1.1", "SRU_RedMini10.1.2", "SRU_RedMini10.1.3" },
                                      LeagueSharp.Common.Utility.Map.MapType.SummonersRift,
                                      GameObjectTeam.Chaos),
                                  new JungleCamp(
                                      75000,
                                      new Vector3(10276.81f, 8037.54f, -108.92f),
                                      new[] { "TT_NGolem5.1.1", "TT_NGolem25.1.2" },
                                      LeagueSharp.Common.Utility.Map.MapType.TwistedTreeline,
                                      GameObjectTeam.Chaos)
                              };

            #endregion

            // Gatta save that .5ms
            JungleCamps = JungleCamps.Where(x => x.MapType == LeagueSharp.Common.Utility.Map.GetMap().Type).ToList();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the live camps.
        /// </summary>
        /// <value>
        ///     The live camps.
        /// </value>
        public static IEnumerable<JungleCamp> DeadCamps
        {
            get
            {
                return JungleCamps.Where(x => x.Dead);
            }
        }

        /// <summary>
        ///     Gets or sets the jungle camps.
        /// </summary>
        /// <value>
        ///     The jungle camps.
        /// </value>
        public static List<JungleCamp> JungleCamps { get; set; }

        /// <summary>
        /// Gets or sets the menu.
        /// </summary>
        /// <value>
        /// The menu.
        /// </value>
        public Menu Menu { get; set; }

        #endregion

        public static bool getCheckBoxItem(string item)
        {
            return jngTimerMenu[item].Cast<CheckBox>().CurrentValue;
        }

        #region Properties

        /// <summary>
        ///     Gets or sets the font.
        /// </summary>
        /// <value>
        ///     The font.
        /// </value>
        private static Font Font { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the Menu.
        /// </summary>
        /// <param name="rootMenu">The root Menu.</param>
        /// <returns></returns>
        public static Menu rootMenu = ElUtilitySuite.Entry.menu;
        public static Menu jngTimerMenu;
        public void CreateMenu(Menu rootMenu)
        {
            jngTimerMenu = rootMenu.AddSubMenu("Jungle Timer", "JngTimer");
            jngTimerMenu.Add("DrawTimers", new CheckBox("Draw Jungle Timers"));
        }

        public void Load()
        {
            Font = new Font(
                Drawing.Direct3DDevice,
                new FontDescription
                    {
                        FaceName = "Tahoma", Height = 13, OutputPrecision = FontPrecision.Default,
                        Quality = FontQuality.Default
                    });

            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;

            Drawing.OnEndScene += this.Drawing_OnEndScene;
            Drawing.OnPreReset += args => { Font.OnLostDevice(); };
            Drawing.OnPostReset += args => { Font.OnResetDevice(); };
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Games the object_ on create.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Type != GameObjectType.obj_AI_Minion)
            {
                return;
            }

            var camp =
                JungleCamps.FirstOrDefault(
                    x => x.MobNames.Select(y => y.ToLower()).Any(z => z.Equals(sender.Name.ToLower())));

            if (camp == null)
            {
                return;
            }

            camp.ObjectsAlive.Add(sender.Name);
            camp.ObjectsDead.Remove(sender.Name);

            if (camp.ObjectsAlive.Count != camp.MobNames.Length)
            {
                return;
            }

            camp.Dead = false;
            camp.NextRespawnTime = 0;
        }

        /// <summary>
        ///     Games the object_ on delete.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender.Type != GameObjectType.obj_AI_Minion)
            {
                return;
            }

            var camp =
                JungleCamps.FirstOrDefault(
                    x => x.MobNames.Select(y => y.ToLower()).Any(z => z.Equals(sender.Name.ToLower())));

            if (camp == null)
            {
                return;
            }

            camp.ObjectsDead.Add(sender.Name);
            camp.ObjectsAlive.Remove(sender.Name);

            if (camp.ObjectsDead.Count != camp.MobNames.Length)
            {
                return;
            }

            camp.Dead = true;
            camp.NextRespawnTime = Environment.TickCount + camp.RespawnTime - 3000;
        }

        /// <summary>
        ///     Fired when the scene is completely rendered.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void Drawing_OnEndScene(EventArgs args)
        {
            if (!getCheckBoxItem("DrawTimers"))
            {
                return;
            }

            foreach (var camp in DeadCamps.Where(x => x.NextRespawnTime - Environment.TickCount > 0))
            {
                var timeSpan = TimeSpan.FromMilliseconds(camp.NextRespawnTime - Environment.TickCount);
                var text = timeSpan.ToString(@"m\:ss");
                var size = Font.MeasureText(text);

                Font.DrawText(
                    null,
                    text,
                    (int)camp.MinimapPosition.X - size.Width / 2,
                    (int)camp.MinimapPosition.Y - size.Height / 2,
                    new ColorBGRA(255, 255, 255, 255));
            }
        }

        #endregion

        /// <summary>
        ///     Represents a jungle camp.
        /// </summary>
        public class JungleCamp
        {
            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="JungleCamp" /> class.
            /// </summary>
            /// <param name="respawnTime">The respawn time.</param>
            /// <param name="position">The position.</param>
            /// <param name="mobNames">The mob names.</param>
            /// <param name="mapType">Type of the map.</param>
            /// <param name="team">The team.</param>
            public JungleCamp(
                int respawnTime,
                Vector3 position,
                string[] mobNames,
                LeagueSharp.Common.Utility.Map.MapType mapType,
                GameObjectTeam team)
            {
                this.RespawnTime = respawnTime;
                this.Position = position;
                this.MobNames = mobNames;
                this.MapType = mapType;
                this.Team = team;

                this.ObjectsDead = new List<string>();
                this.ObjectsAlive = new List<string>();
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets or sets a value indicating whether this <see cref="JungleCamp" /> is dead.
            /// </summary>
            /// <value>
            ///     <c>true</c> if dead; otherwise, <c>false</c>.
            /// </value>
            public bool Dead { get; set; }

            /// <summary>
            ///     Gets or sets the type of the map.
            /// </summary>
            /// <value>
            ///     The type of the map.
            /// </value>
            public LeagueSharp.Common.Utility.Map.MapType MapType { get; set; }

            /// <summary>
            ///     Gets the minimap position.
            /// </summary>
            /// <value>
            ///     The minimap position.
            /// </value>
            public Vector2 MinimapPosition
            {
                get
                {
                    return Drawing.WorldToMinimap(this.Position);
                }
            }

            /// <summary>
            ///     Gets or sets the mob names.
            /// </summary>
            /// <value>
            ///     The mob names.
            /// </value>
            public string[] MobNames { get; set; }

            /// <summary>
            ///     Gets or sets the next respawn time.
            /// </summary>
            /// <value>
            ///     The next respawn time.
            /// </value>
            public int NextRespawnTime { get; set; }

            /// <summary>
            ///     Gets or sets the objects alive.
            /// </summary>
            /// <value>
            ///     The objects alive.
            /// </value>
            public List<string> ObjectsAlive { get; set; }

            /// <summary>
            ///     Gets or sets the objects dead.
            /// </summary>
            /// <value>
            ///     The objects dead.
            /// </value>
            public List<string> ObjectsDead { get; set; }

            /// <summary>
            ///     Gets the position.
            /// </summary>
            /// <value>
            ///     The position.
            /// </value>
            public Vector3 Position { get; set; }

            /// <summary>
            ///     Gets the respawn time.
            /// </summary>
            /// <value>
            ///     The respawn time.
            /// </value>
            public int RespawnTime { get; set; }

            /// <summary>
            ///     Gets or sets the team.
            /// </summary>
            /// <value>
            ///     The team.
            /// </value>
            public GameObjectTeam Team { get; set; }

            #endregion
        }
    }
}