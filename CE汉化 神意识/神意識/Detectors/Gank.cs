namespace CowAwareness.Detectors
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using CowAwareness.Features;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;

    public class Gank : Feature, IToggleFeature
    {
        #region Static Fields

        private static readonly HashSet<GankObject> GankObjects = new HashSet<GankObject>();

        #endregion

        #region Fields

        private float lastCheck;

        private float lastPing;

        #endregion

        #region Public Properties

        public override string Name
        {
            get
            {
                return "Gank";
            }
        }

        #endregion

        #region Properties

        private int Cooldown
        {
            get
            {
                return this["cooldown"].Cast<Slider>().CurrentValue;
            }
        }

        private int Duration
        {
            get
            {
                return this["duration"].Cast<Slider>().CurrentValue;
            }
        }

        private bool Ping
        {
            get
            {
                return this["ping"].Cast<CheckBox>().CurrentValue;
            }
        }

        private int Range
        {
            get
            {
                return this["range"].Cast<Slider>().CurrentValue;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Disable()
        {
            Game.OnUpdate -= this.OnGameUpdate;
            Drawing.OnDraw -= this.OnDrawingEndScene;
        }

        public void Enable()
        {
            Game.OnUpdate += this.OnGameUpdate;
            Drawing.OnDraw += this.OnDrawingEndScene;
        }

        #endregion

        #region Methods

        protected override void Initialize()
        {
            this.Menu.AddLabel("偵測敵方或隊友的Gank");

            this.Menu.Add("range", new Slider("範圍", 3000, 1100, 5000));
            this.Menu.Add("cooldown", new Slider("CD", 10, 0, 30));
            this.Menu.Add("duration", new Slider("持續時間", 10, 0, 30));
            this.Menu.Add("ping", new CheckBox("亮燈(本地)"));

            foreach (var hero in EntityManager.Heroes.AllHeroes.Where(h => !h.IsMe))
            {
                GankObjects.Add(new GankObject(hero));
            }
        }

        private void OnDrawingEndScene(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || Drawing.Direct3DDevice == null || Drawing.Direct3DDevice.IsDisposed)
            {
                return;
            }

            foreach (
                var obj in
                    GankObjects.Where(
                        c =>
                        !c.Hero.IsDead && c.Hero.IsValidTarget(this.Range) && c.LastTrigger + this.Duration > Game.Time)
                )
            {
                Drawing.DrawLine(
                    ObjectManager.Player.Position.WorldToScreen(),
                    obj.Hero.Position.WorldToScreen(),
                    8f,
                    Color.FromArgb(80, obj.Color));
                if (this.Ping && obj.Hero.IsEnemy && this.lastPing + this.Cooldown * 1000 < Environment.TickCount)
                {
                    TacticalMap.ShowPing(PingCategory.Danger, ObjectManager.Player.Position, true);
                    this.lastPing = Environment.TickCount;
                }
            }
        }

        private void OnGameUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || this.lastCheck + 500f > Environment.TickCount)
            {
                return;
            }

            this.lastCheck = Environment.TickCount;

            foreach (var obj in GankObjects.Where(c => !c.Hero.IsDead))
            {
                var distance = obj.Hero.Distance(ObjectManager.Player);

                if (obj.Distance > this.Range && distance <= this.Range && Game.Time > obj.LastTrigger + this.Cooldown)
                {
                    obj.LastTrigger = Game.Time;
                }

                obj.Distance = distance;
            }
        }

        #endregion

        internal class GankObject
        {
            #region Constructors and Destructors

            public GankObject(AIHeroClient hero)
            {
                var hasSmite = hero.Spellbook.Spells.Any(spell => spell.Name.ToLower().Contains("smite"));
                this.Hero = hero;
                this.Color = hero.IsEnemy
                                 ? (hasSmite ? Color.Purple : Color.Red)
                                 : (hasSmite ? Color.Green : Color.Cyan);
            }

            #endregion

            #region Public Properties

            public Color Color { get; set; }

            public float Distance { get; set; }

            public AIHeroClient Hero { get; private set; }

            public float LastTrigger { get; set; }

            #endregion
        }
    }
}