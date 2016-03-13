namespace CowAwareness.Drawings
{
    using System;
    using System.Drawing;
    using System.Linq;

    using CowAwareness.Features;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Rendering;

    public class TowerRange : Feature, IToggleFeature
    {
        #region Public Properties

        public override string Name
        {
            get
            {
                return "Tower Range";
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Disable()
        {
            Drawing.OnDraw -= this.Drawing_OnDraw;
        }

        public void Enable()
        {
            Drawing.OnDraw += this.Drawing_OnDraw;
        }

        #endregion

        #region Methods

        protected override void Initialize()
        {
            this.Menu.AddLabel("繪畫敵人防御塔的範圍");
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            foreach (
                var turret in
                    EntityManager.Turrets.Enemies.Where(a => !a.IsDead && ObjectManager.Player.Distance(a) <= 2000))
            {
                if (ObjectManager.Player.Distance(turret) <= 870)
                {
                    new Circle { Color = Color.Red, Radius = 870, BorderWidth = 2f }.Draw(turret.Position);
                }
                else if (ObjectManager.Player.Distance(turret) > 870 && ObjectManager.Player.Distance(turret) < 1650)
                {
                    new Circle { Color = Color.Yellow, Radius = 870, BorderWidth = 2f }.Draw(turret.Position);
                }
                else
                {
                    new Circle { Color = Color.White, Radius = 870, BorderWidth = 2f }.Draw(turret.Position);
                }
            }
        }

        #endregion
    }
}