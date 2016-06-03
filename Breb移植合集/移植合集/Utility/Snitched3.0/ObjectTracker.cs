namespace Snitched
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using EloBuddy;
    /// <summary>
    ///     Tracks objects.
    /// </summary>
    internal class ObjectTracker
    {
        #region Public Events

        /// <summary>
        ///     Occurs when an objective spawns.
        /// </summary>
        public static event EventHandler<ObjectiveType> OnObjectiveCreated;

        /// <summary>
        ///     Occurs when an objective spawns.
        /// </summary>
        public static event EventHandler<ObjectiveType> OnObjectiveDead;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the baron.
        /// </summary>
        /// <value>
        ///     The baron.
        /// </value>
        public static Obj_AI_Base Baron { get; set; }

        /// <summary>
        ///     Gets or sets the blue.
        /// </summary>
        /// <value>
        ///     The blue.
        /// </value>
        public static List<Obj_AI_Base> BlueBuffs { get; set; }

        /// <summary>
        ///     Gets or sets the dragon.
        /// </summary>
        /// <value>
        ///     The dragon.
        /// </value>
        public static Obj_AI_Base Dragon { get; set; }

        /// <summary>
        ///     Gets or sets the red.
        /// </summary>
        /// <value>
        ///     The red.
        /// </value>
        public static List<Obj_AI_Base> RedBuffs { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public static void Load()
        {
            BlueBuffs = new List<Obj_AI_Base>();
            RedBuffs = new List<Obj_AI_Base>();

            ScanObjects();

            GameObject.OnCreate += GameObject_OnCreate;
            Game.OnUpdate += GameOnOnUpdate;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Fires the object created event.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="type">The type.</param>
        private static void FireObjectCreatedEvent(Obj_AI_Base unit, ObjectiveType type)
        {
            OnObjectiveCreated?.Invoke(unit, type);
        }

        /// <summary>
        ///     Fired when a <see cref="GameObject" /> is created.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            var obj = sender as Obj_AI_Base;

            if (obj == null)
            {
                return;
            }

            if (obj.CharData.BaseSkinName == "SRU_Baron")
            {
                Baron = obj;

                FireObjectCreatedEvent(obj, ObjectiveType.Baron);
            }

            if (obj.CharData.BaseSkinName.Contains("SRU_Dragon"))
            {
                Dragon = obj;

                FireObjectCreatedEvent(obj, ObjectiveType.Dragon);
            }

            if (obj.CharData.BaseSkinName == "SRU_Blue")
            {
                BlueBuffs.Add(obj);

                FireObjectCreatedEvent(obj, ObjectiveType.Blue);
            }

            if (obj.CharData.BaseSkinName == "SRU_Red")
            {
                RedBuffs.Add(obj);

                FireObjectCreatedEvent(obj, ObjectiveType.Red);
            }
        }

        /// <summary>
        ///     Fired when the game is updated.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void GameOnOnUpdate(EventArgs args)
        {
            if (Baron != null && (Baron.IsDead || !Baron.IsValid))
            {
                Baron = null;

                OnObjectiveDead?.Invoke(Baron, ObjectiveType.Baron);
            }

            if (Dragon != null && (Dragon.IsDead || !Dragon.IsValid))
            {
                Dragon = null;

                OnObjectiveDead?.Invoke(Dragon, ObjectiveType.Dragon);
            }

            foreach (var blue in BlueBuffs.ToArray().Where(x => x.IsDead || !x.IsValid))
            {
                BlueBuffs.RemoveAll(x => x.NetworkId == blue.NetworkId);

                OnObjectiveDead?.Invoke(blue, ObjectiveType.Blue);
            }

            foreach (var red in RedBuffs.ToArray().Where(x => x.IsDead || !x.IsValid))
            {
                RedBuffs.RemoveAll(x => x.NetworkId == red.NetworkId);

                OnObjectiveDead?.Invoke(red, ObjectiveType.Red);
            }
        }

        private static void ScanObjects()
        {
            Baron = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(x => x.CharData.BaseSkinName.Equals("SRU_Baron"));

            if (Baron != null)
            {
                FireObjectCreatedEvent(Baron, ObjectiveType.Baron);
            }

            Dragon = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(x => x.CharData.BaseSkinName.Equals("SRU_Dragon"));

            if (Dragon != null)
            {
                FireObjectCreatedEvent(Dragon, ObjectiveType.Dragon);
            }

            BlueBuffs = ObjectManager.Get<Obj_AI_Base>().Where(x => x.CharData.BaseSkinName.Equals("SRU_Blue")).ToList();
            BlueBuffs.ForEach(x => FireObjectCreatedEvent(x, ObjectiveType.Blue));

            RedBuffs = ObjectManager.Get<Obj_AI_Base>().Where(x => x.CharData.BaseSkinName.Equals("SRU_Red")).ToList();
            RedBuffs.ForEach(x => FireObjectCreatedEvent(x, ObjectiveType.Red));
        }

        #endregion
    }

    /// <summary>
    ///     The type of objective.
    /// </summary>
    internal enum ObjectiveType
    {
        /// <summary>
        ///     The baron
        /// </summary>
        Baron,

        /// <summary>
        ///     The dragon
        /// </summary>
        Dragon,

        /// <summary>
        ///     The blue
        /// </summary>
        Blue,

        /// <summary>
        ///     The red
        /// </summary>
        Red,
    }
}