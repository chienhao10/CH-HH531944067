namespace ElUtilitySuite.Others
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;
    using EloBuddy.SDK.Menu;
    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;
    internal class TurnAround : IPlugin
    {
        // Copyright 2014 - 2015 Nikita Bernthaler

        #region Fields

        private readonly List<SpellInfo> _spellInfos = new List<SpellInfo>
        {
            new SpellInfo("Cassiopeia", "CassiopeiaPetrifyingGaze", 1000f, false, true, 0.85f),
            new SpellInfo("Tryndamere", "MockingShout", 900f, false, false, 0.65f)
        };

        /// <summary>
        ///     Block movement time
        /// </summary>
        private float blockMovementTime;

        /// <summary>
        ///     Last move
        /// </summary>
        private Vector3 lastMove;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the menu
        /// </summary>
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
        public void CreateMenu(Menu rootMenu)
        {
            var turnAroundMenu = rootMenu.AddSubMenu("自动转向", "TurnAround");
            {
                turnAroundMenu.Add("TurnAround", new CheckBox("开启防蛮王W/蛇女R"));
            }

            this.Menu = turnAroundMenu;
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
            Obj_AI_Base.OnProcessSpellCast += this.OnProcessSpellCast;
            EloBuddy.Player.OnIssueOrder += this.OnObjAiBaseIssueOrder;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Fired when a spell has been casted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnObjAiBaseIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            try
            {
                if (!getCheckBoxItem(this.Menu, "TurnAround"))
                {
                    return;
                }

                if (sender.IsMe)
                {
                    if (args.Order == GameObjectOrder.MoveTo)
                    {
                        this.lastMove = args.TargetPosition;
                    }
                    if (this.blockMovementTime > Game.Time)
                    {
                        args.Process = false;
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
                if (!getCheckBoxItem(this.Menu, "TurnAround"))
                {
                    return;
                }

                if (sender == null || !sender.IsValid || sender.Team == this.Player.Team
                    || this.Player.IsDead || !this.Player.IsTargetable)
                {
                    return;
                }

                var spellInfo =
                    this._spellInfos.FirstOrDefault(
                        i => args.SData.Name.Equals(i.Name, StringComparison.InvariantCultureIgnoreCase));

                if (spellInfo == null)
                {
                    return;
                }

                if ((spellInfo.Target && args.Target == this.Player)
                    || this.Player.LSDistance(sender.ServerPosition) + this.Player.BoundingRadius <= spellInfo.Range)
                {
                    var moveTo = this.lastMove;

                    EloBuddy.Player.IssueOrder(
                        GameObjectOrder.MoveTo,
                        sender.ServerPosition.LSExtend(
                            this.Player.ServerPosition,
                            this.Player.ServerPosition.LSDistance(sender.ServerPosition)
                            + (spellInfo.TurnOpposite ? 100 : -100)));

                    this.blockMovementTime = Game.Time + spellInfo.CastTime;

                    LeagueSharp.Common.Utility.DelayAction.Add(
                        (int)((spellInfo.CastTime + 0.1) * 1000),
                        () => EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, moveTo));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion

        private class SpellInfo
        {
            #region Constructors and Destructors

            /// <summary>
            ///     
            /// </summary>
            /// <param name="owner"></param>
            /// <param name="name"></param>
            /// <param name="range"></param>
            /// <param name="target"></param>
            /// <param name="turnOpposite"></param>
            /// <param name="castTime"></param>
            public SpellInfo(string owner, string name, float range, bool target, bool turnOpposite, float castTime)
            {
                this.Owner = owner;
                this.Name = name;
                this.Range = range;
                this.Target = target;
                this.TurnOpposite = turnOpposite;
                this.CastTime = castTime;
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     The spell cast time
            /// </summary>
            public float CastTime { get; private set; }

            /// <summary>
            ///     The spell name
            /// </summary>
            public string Name { get; private set; }

            /// <summary>
            ///     The spell casters champion name
            /// </summary>
            public string Owner { get; private set; }

            /// <summary>
            ///     The spells range
            /// </summary>
            public float Range { get; private set; }

            /// <summary>
            ///     The target
            /// </summary>
            public bool Target { get; private set; }

            /// <summary>
            ///     Turn
            /// </summary>
            public bool TurnOpposite { get; private set; }

            #endregion
        }
    }
}