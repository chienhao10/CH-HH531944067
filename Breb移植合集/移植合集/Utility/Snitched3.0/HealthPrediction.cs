namespace Snitched
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;

    using LeagueSharp;
    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;    /// <summary>
                                       ///     Predicts health by getting the average damage per tick. To be used on Objectives ONLY.
                                       /// </summary>
    internal class HealthPrediction
    {
        #region Constants

        /// <summary>
        ///     The multiplier
        /// </summary>
        private const float Multiplier = 1;

        #endregion

        #region Static Fields

        /// <summary>
        ///     The tracked objects
        /// </summary>
        private static readonly Dictionary<Obj_AI_Base, HealthValues> TrackedObjects =
            new Dictionary<Obj_AI_Base, HealthValues>();

        /// <summary>
        ///     The last clock update
        /// </summary>
        private static float lastClockUpdate;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the predicted health.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="seconds">The ticks.</param>
        /// <returns></returns>
        public static float GetPredictedHealth(Obj_AI_Base unit, float seconds = 1)
        {
            HealthValues healthValues;

            if (TrackedObjects.TryGetValue(unit, out healthValues))
            {
                return unit.Health - healthValues.AverageDamage * Multiplier * seconds;
            }

            TrackObject(unit);
            return unit.Health;
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public static void Load()
        {
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        /// <summary>
        ///     Tracks the object.
        /// </summary>
        /// <param name="unit">The unit.</param>
        public static void TrackObject(Obj_AI_Base unit)
        {
            TrackedObjects[unit] = new HealthValues(unit.MaxHealth);
        }

        /// <summary>
        ///     Untracks the object.
        /// </summary>
        /// <param name="unit">The unit.</param>
        public static void UntrackObject(Obj_AI_Base unit)
        {
            TrackedObjects.Remove(unit);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Fired when the game is drawn.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!Config.Instance.miscMenu["DrawADPS"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            foreach (var trackedobj in TrackedObjects)
            {
                var wts = Drawing.WorldToScreen(trackedobj.Key.Position);
                var text = "Avg DPS: "
                           + (trackedobj.Value.AverageDamage * Multiplier).ToString(CultureInfo.InvariantCulture);
                //var size = Drawing.GetTextExtent(text);

                Drawing.DrawText(wts.X - 70 / 2f, wts.Y, Color.BlueViolet, text);
            }
        }

        /// <summary>
        ///     Fired when the game is updated.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void Game_OnUpdate(EventArgs args)
        {
            if (Game.Time - lastClockUpdate < 1f / Multiplier)
            {
                return;
            }

            lastClockUpdate = Game.Time;

            foreach (var obj in TrackedObjects.ToDictionary(x => x.Key, y => y.Value))
            {
                if (obj.Key.IsDead || !obj.Key.IsValid)
                {
                    TrackedObjects.Remove(obj.Key);
                    continue;
                }

                // If the object regen'd erase all previous values (Gives us negative values)
                if (obj.Key.Health > obj.Value.LastHealth || Math.Abs(obj.Key.Health - obj.Key.MaxHealth) < float.Epsilon)
                {
                    obj.Value.Health.Clear();
                    obj.Value.LastHealth = obj.Key.Health;

                    continue;
                }

                if (!obj.Key.IsVisible)
                {
                    continue;
                }

                obj.Value.Health.Add(obj.Value.LastHealth - obj.Key.Health);
                obj.Value.LastHealth = obj.Key.Health;
            }
        }

        #endregion
    }

    internal class HealthValues
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="HealthValues" /> class.
        /// </summary>
        public HealthValues(float health)
        {
            this.Health = new List<float>();
            this.LastHealth = health;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the average damage per tick.
        /// </summary>
        /// <value>
        ///     The average damage per tick.
        /// </value>
        public float AverageDamage
        {
            get
            {
                if (!this.Health.Any())
                {
                    return 0;
                }

                return this.Health.Where(x => Math.Abs(x) > float.Epsilon).Sum() / this.Health.Count;
            }
        }

        /// <summary>
        ///     Gets or sets the health.
        /// </summary>
        /// <value>
        ///     The health.
        /// </value>
        public List<float> Health { get; set; }

        /// <summary>
        ///     Gets or sets the last health.
        /// </summary>
        /// <value>
        ///     The last health.
        /// </value>
        public float LastHealth { get; set; }

        #endregion
    }
}