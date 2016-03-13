namespace CowAwareness.Features
{
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    public abstract class Feature
    {
        #region Public Properties

        public Menu Menu { get; private set; }

        public abstract string Name { get; }

        #endregion

        #region Public Indexers

        public ValueBase this[string menuItem]
        {
            get
            {
                return this.Menu[menuItem];
            }
        }

        #endregion

        #region Public Methods and Operators

        public virtual void Load(Addon owner)
        {
            this.Menu = owner.Menu.AddSubMenu(this.Name, this.Name);
            this.Menu.AddGroupLabel("設定");

            var toggleFeature = this as IToggleFeature;

            if (toggleFeature != null)
            {
                this.ToggleFeatureLoad(toggleFeature);
            }

            this.Initialize();
        }

        #endregion

        #region Methods

        protected abstract void Initialize();

        private void ToggleFeatureLoad(IToggleFeature toggleFeature)
        {
            this.Menu.Add(this.Name + "enabled", new CheckBox("啟用")).OnValueChange += (sender, args) =>
                {
                    if (args.NewValue)
                    {
                        toggleFeature.Enable();
                    }
                    else
                    {
                        toggleFeature.Disable();
                    }
                };

            if (this[this.Name + "enabled"].Cast<CheckBox>().CurrentValue)
            {
                toggleFeature.Enable();
            }
        }

        #endregion
    }
}