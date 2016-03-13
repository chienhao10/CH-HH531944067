namespace CowAwareness.Detectors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CowAwareness.Features;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Rendering;

    using SharpDX;

    public class Clone : Feature, IToggleFeature
    {
        #region Fields

        private readonly List<string> cloneHeroes = new List<string> { "shaco", "leblanc", "monkeyking", "yorick" };

        private readonly List<Obj_AI_Base> heroes = new List<Obj_AI_Base>();

        private ColorBGRA color;

        #endregion

        #region Public Properties

        public override string Name
        {
            get
            {
                return "Clone Revealer";
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Disable()
        {
            Drawing.OnEndScene -= this.Drawing_OnEndScene;
        }

        public void Enable()
        {
            Drawing.OnEndScene += this.Drawing_OnEndScene;
        }

        #endregion

        #region Methods

        protected override void Initialize()
        {
            this.Menu.AddLabel("偵測分身(在真身下面有一個圓圈), 啟用在:");
            this.Menu.AddLabel("- 小丑");
            this.Menu.AddLabel("- 勒布朗");
            this.Menu.AddLabel("- 悟空");
            this.Menu.AddLabel("- 約瑞科");

            this.color = Color.Magenta;
            this.heroes.AddRange(
                EntityManager.Heroes.Enemies.Where(e => this.cloneHeroes.Contains(e.ChampionName.ToLower())));

            if (!this.heroes.Any())
            {
                this.Disable();
            }
        }

        private void Drawing_OnEndScene(EventArgs args)
        {
            foreach (var hero in this.heroes.Where(hero => !hero.IsDead && hero.IsVisible && hero.Position.IsOnScreen())
                )
            {
                Circle.Draw(this.color, hero.BoundingRadius, 2f, hero.Position);
            }
        }

        #endregion
    }
}