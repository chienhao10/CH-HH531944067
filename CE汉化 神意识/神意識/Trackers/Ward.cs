namespace CowAwareness.Trackers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using CowAwareness.Features;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;

    using SharpDX;

    using Color = System.Drawing.Color;

    public class Ward : Feature, IToggleFeature
    { 
        #region Static Fields

        private static List<WardInfo> wards = new List<WardInfo>();

        #endregion

        #region Fields

        private Text text;

        #endregion

        #region Public Properties

        public override string Name
        {
            get
            {
                return "Ward Tracker";
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Disable()
        {
            //Drawing.OnDraw -= this.Drawing_OnDraw;
            Drawing.OnEndScene -= this.DrawWards;
            GameObject.OnCreate -= this.GameObject_OnCreate;
            GameObject.OnDelete -= this.GameObject_OnDelete;
        }

        public void Enable()
        {
            Drawing.OnEndScene += this.DrawWards;
            GameObject.OnCreate += this.GameObject_OnCreate;
            GameObject.OnDelete += this.GameObject_OnDelete;
        }

        #endregion

        #region Methods

        protected override void Initialize()
        {
            this.Menu.Add("timer", new CheckBox("繪畫剩餘時間"));
            this.Menu.Add("range", new KeyBind("繪畫眼睛視野範圍", false, KeyBind.BindTypes.HoldActive, 'Z'));
            this.text = new Text(string.Empty, new Font(FontFamily.GenericSansSerif, 11f, FontStyle.Bold));
        }

        private static void DrawWardMinimap(Vector3 center, Color color)
        {
            var centerMap = center.WorldToMinimap();
            var a = new Vector2(centerMap.X - 5, centerMap.Y);
            var b = new Vector2(centerMap.X + 5, centerMap.Y);
            var c = new Vector2(centerMap.X, centerMap.Y + 10);

            Drawing.DrawLine(a, b, 2f, color);
            Drawing.DrawLine(b, c, 2f, color);
            Drawing.DrawLine(c, a, 2f, color);
        }

        private void DrawWards(EventArgs args)
        {
            var removeList = new List<WardInfo>();

            foreach (var wardInfo in wards)
            {
                if (!wardInfo.Available)
                {
                    removeList.Add(wardInfo);
                    continue;
                }

                var buff = wardInfo.Ward.Buffs.FirstOrDefault();

                if (buff == null)
                {
                    removeList.Add(wardInfo);
                    continue;
                }

                var remaining = buff.EndTime - Game.Time;

                if (remaining > 0 || wardInfo.IsPink)
                {
                    var radius = this["range"].Cast<KeyBind>().CurrentValue ? 1100 : 60;

                    new Circle { Color = wardInfo.Color, Radius = radius, BorderWidth = 1f }.Draw(wardInfo.Position);
                    DrawWardMinimap(wardInfo.Position, wardInfo.Color);
                    //this.text.Draw(".", wardInfo.Color, (int)mapPos.X, (int)mapPos.Y);

                    if (this["timer"].Cast<CheckBox>().CurrentValue)
                    {
                        var location = Drawing.WorldToScreen(wardInfo.Position);
                        
                        if (!wardInfo.IsPink)
                        {
                            this.text.Draw(
                                string.Format("{0:0}", remaining),
                                Color.White,
                                (int)location.X,
                                (int)location.Y);
                        }
                    }
                }
                else
                {
                    removeList.Add(wardInfo);
                }
            }

            if (removeList.Any())
            {
                wards = wards.Except(removeList).ToList();
            }
        }

        private void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            var ward = sender as Obj_AI_Minion;

            if (ward == null || !sender.Name.ToLower().Contains("ward"))
            {
                return;
            }

            if (ward.IsAlly)
            {
                return;
            }

            switch (ward.BaseSkinName)
            {
                case "VisionWard":
                    wards.Add(new WardInfo(ward, true));
                    break;
                default:
                    wards.Add(new WardInfo(ward, false));
                    break;
            }
        }

        private void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            var ward = sender as Obj_AI_Minion;

            var wardInfo = wards.FirstOrDefault(w => w.Ward == ward);

            if (wardInfo != null)
            {
                wardInfo.Available = false;
            }
        }

        #endregion

        internal class WardInfo
        {
            #region Constructors and Destructors

            public WardInfo(Obj_AI_Minion ward, bool isPink)
            {
                this.Ward = ward;
                this.Color = isPink ? Color.Magenta : Color.Lime;
                this.Available = true;
                this.Position = ward.Position;
                this.IsPink = isPink;
            }

            #endregion

            #region Public Properties

            public bool Available { get; set; }

            public Color Color { get; set; }

            public bool IsPink { get; set; }

            public Vector3 Position { get; set; }

            public Obj_AI_Minion Ward { get; set; }

            #endregion
        }
    }
}