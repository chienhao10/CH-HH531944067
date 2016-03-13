namespace CowAwareness.Trackers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    using CowAwareness.Features;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;

    public class Teleport : Feature, IToggleFeature
    {
        #region Fields

        private readonly HashSet<TeleportInfo> teleports = new HashSet<TeleportInfo>();

        private Text text;

        #endregion

        #region Public Properties

        public override string Name
        {
            get
            {
                return "Recall Tracker";
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Disable()
        {
            Obj_AI_Base.OnTeleport += this.OnTeleport;
        }

        public void Enable()
        {
            Obj_AI_Base.OnTeleport += this.OnTeleport;
            Drawing.OnEndScene += this.Drawing_OnEndScene;
        }

        #endregion

        #region Methods

        protected override void Initialize()
        {
            this.Menu.AddLabel("偵測回程和傳送");

            this.Menu.AddSeparator();

            this.Menu.AddLabel("位置");

            this.Menu.Add("x", new Slider("X", Drawing.Width - 250, 0, Drawing.Width));
            this.Menu.Add("y", new Slider("Y", Drawing.Height - 400, 0, Drawing.Height));

            this.Menu.Add("allies", new CheckBox("偵測隊友", false));
            this.Menu.Add("self", new CheckBox("偵測自己", false));
            this.Menu.Add("enemies", new CheckBox("偵測敵人s", false));

            this.text = new Text(string.Empty, new Font(FontFamily.GenericSansSerif, 10f, FontStyle.Bold));
        }

        private static int GetRecallTime(AIHeroClient hero)
        {
            var buffHerald = hero.GetBuff("exaltedwithminibaronnashor");
            var buffNashor = hero.GetBuff("exaltedwithbaronnashor");

            if (buffHerald != null || buffNashor != null)
            {
                return 4000;
            }

            return 8000;
        }

        private void Drawing_OnEndScene(EventArgs args)
        {
            var x = this["x"].Cast<Slider>().CurrentValue;
            var y = this["y"].Cast<Slider>().CurrentValue;

            foreach (var tp in this.teleports)
            {
                var remaining = (tp.EndTime - Environment.TickCount) / 1000;

                if (tp.Finished)
                {
                    this.text.Draw(string.Format("{0} 已回程", tp.Hero.ChampionName), Color.Lime, x, y);
                }
                else if (tp.Aborted)
                {
                    this.text.Draw(string.Format("{0} 已停止回程", tp.Hero.ChampionName), Color.Red, x, y);
                }
                else
                {
                    this.text.Draw(
                        string.Format(
                            "{0} 回程中{1:0.00} ({2}%)",
                            tp.Hero.ChampionName,
                            remaining,
                            (int)tp.Hero.HealthPercent),
                        Color.White,
                        x,
                        y);
                }

                y += 20;
            }
        }

        private void OnTeleport(Obj_AI_Base sender, GameObjectTeleportEventArgs args)
        {
            var hero = sender as AIHeroClient;

            if (hero == null)
            {
                return;
            }

            if (hero.IsMe && !this["self"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            if (hero.IsEnemy && !this["enemies"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            if (hero.IsAlly && !hero.IsMe && !this["allies"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            if (args.RecallName != string.Empty)
            {
                this.teleports.Add(new TeleportInfo(hero, Environment.TickCount + GetRecallTime(hero)));
                return;
            }

            foreach (var tp in this.teleports)
            {
                if (Environment.TickCount >= tp.EndTime)
                {
                    tp.Finished = true;
                }
                else
                {
                    tp.Aborted = true;
                }

                Core.DelayAction(() => { this.teleports.Remove(tp); }, 3000);
            }
        }

        #endregion

        private class TeleportInfo
        {
            #region Constructors and Destructors

            public TeleportInfo(AIHeroClient hero, float endTime)
            {
                this.Hero = hero;
                this.EndTime = endTime;
                this.Aborted = false;
                this.Finished = false;
            }

            #endregion

            #region Public Properties

            public bool Aborted { get; set; }

            public float EndTime { get; private set; }

            public bool Finished { get; set; }

            public AIHeroClient Hero { get; private set; }

            #endregion
        }
    }
}