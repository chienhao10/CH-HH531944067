namespace CowAwareness.Features
{
    using System;
    using System.Collections.Generic;

    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;

    public class Addon
    {
        #region Fields

        private readonly string addonName;

        private readonly HashSet<Feature> features = new HashSet<Feature>();

        #endregion

        #region Constructors and Destructors

        public Addon(string name)
        {
            this.addonName = name;
            Loading.OnLoadingComplete += this.Loading_OnLoadingComplete;
        }

        #endregion

        #region Delegates

        public delegate void MenuInitializedEventHandler(Menu menu);

        #endregion

        #region Public Events

        public event MenuInitializedEventHandler MenuInitialized;

        #endregion

        #region Public Properties

        public Menu Menu { get; private set; }

        #endregion

        #region Public Methods and Operators

        public Addon Add(Feature feat)
        {
            this.features.Add(feat);
            return this;
        }

        #endregion

        #region Methods

        private void Loading_OnLoadingComplete(EventArgs args)
        {
            this.Menu = MainMenu.AddMenu(this.addonName, this.addonName);
            this.OnMenuInitialized(this.Menu);

            foreach (var feat in this.features)
            {
                feat.Load(this);
            }
        }

        private void OnMenuInitialized(Menu menu)
        {
            if (this.MenuInitialized != null)
            {
                this.MenuInitialized(menu);
            }
        }

        #endregion
    }
}