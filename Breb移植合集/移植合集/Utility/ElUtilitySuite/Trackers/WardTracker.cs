namespace ElUtilitySuite.Trackers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Permissions;

    using ElUtilitySuite.Vendor.SFX;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;
    using SharpDX.Direct3D9;

    using Color = System.Drawing.Color;
    using ItemData = LeagueSharp.Common.Data.ItemData;
    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;
    using PortAIO.Properties;
    using EloBuddy.SDK.Menu;
    internal class WardTracker : IPlugin
    {
        #region Constants

        private const float CheckInterval = 300f;

        #endregion

        #region Fields

        private readonly List<HeroWard> _heroNoWards = new List<HeroWard>();

        private readonly List<WardObject> _wardObjects = new List<WardObject>();

        private readonly List<WardStruct> _wardStructs = new List<WardStruct>
                                                             {
                                                                 new WardStruct(
                                                                     60 * 1,
                                                                     1100,
                                                                     "YellowTrinket",
                                                                     "TrinketTotemLvl1",
                                                                     WardType.Green),
                                                                 new WardStruct(
                                                                     60 * 1,
                                                                     1100,
                                                                     "BlueTrinket",
                                                                     "TrinketOrbLvl3",
                                                                     WardType.Green),
                                                                 new WardStruct(
                                                                     60 * 2,
                                                                     1100,
                                                                     "YellowTrinketUpgrade",
                                                                     "TrinketTotemLvl2",
                                                                     WardType.Green),
                                                                 new WardStruct(
                                                                     60 * 3,
                                                                     1100,
                                                                     "SightWard",
                                                                     "ItemGhostWard",
                                                                     WardType.Green),
                                                                 new WardStruct(
                                                                     60 * 3,
                                                                     1100,
                                                                     "SightWard",
                                                                     "SightWard",
                                                                     WardType.Green),
                                                                 new WardStruct(
                                                                     60 * 3,
                                                                     1100,
                                                                     "MissileWard",
                                                                     "MissileWard",
                                                                     WardType.Green),
                                                                 new WardStruct(
                                                                     int.MaxValue,
                                                                     1100,
                                                                     "VisionWard",
                                                                     "VisionWard",
                                                                     WardType.Pink),
                                                                 new WardStruct(
                                                                     60 * 4,
                                                                     212,
                                                                     "CaitlynTrap",
                                                                     "CaitlynYordleTrap",
                                                                     WardType.Trap),
                                                                 new WardStruct(
                                                                     60 * 10,
                                                                     212,
                                                                     "TeemoMushroom",
                                                                     "BantamTrap",
                                                                     WardType.Trap),
                                                                 new WardStruct(
                                                                     60 * 1,
                                                                     212,
                                                                     "ShacoBox",
                                                                     "JackInTheBox",
                                                                     WardType.Trap),
                                                                 new WardStruct(
                                                                     60 * 2,
                                                                     212,
                                                                     "Nidalee_Spear",
                                                                     "Bushwhack",
                                                                     WardType.Trap),
                                                                 new WardStruct(
                                                                     60 * 10,
                                                                     212,
                                                                     "Noxious_Trap",
                                                                     "BantamTrap",
                                                                     WardType.Trap)
                                                             };

        private Texture _greenWardTexture;

        private float _lastCheck = Environment.TickCount;

        private Line _line;

        private Texture _pinkWardTexture;

        private Sprite _sprite;

        private Font _text;

        #endregion

        #region Enums

        private enum WardType
        {
            Green,

            Pink,

            Trap
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the menu
        /// </summary>
        public Menu Menu { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public void CreateMenu(Menu rootMenu)
        {
            var wardtrackerMenu = rootMenu.AddSubMenu("眼位计时", "wardtracker");
            {
                wardtrackerMenu.Add("wardtracker.TimeFormat", new ComboBox("时间格式", 0, "mm:ss", "ss"));
                wardtrackerMenu.Add("wardtracker.FontSize", new Slider("字体大小", 13, 3, 30));
                wardtrackerMenu.Add("wardtracker.CircleRadius", new Slider("圆圈半径", 150, 25, 300));
                wardtrackerMenu.Add("wardtracker.CircleThickness", new Slider("圆圈粗细", 2, 1, 10));
                wardtrackerMenu.Add("wardtracker.GreenCircle", new CheckBox("绿色圈"));
                //wardtrackerMenu.Add("wardtracker.GreenColor", "Green Color").SetValue(Color.Lime));
                //wardtrackerMenu.Add("wardtracker.PinkColor", "Pink Color").SetValue(Color.Magenta));
                //wardtrackerMenu.Add("wardtracker.TrapColor", "Trap Color").SetValue(Color.Red));
                wardtrackerMenu.Add("wardtracker.VisionRange", new CheckBox("视野范围"));
                wardtrackerMenu.Add("wardtracker.Minimap", new CheckBox("小地图"));
                wardtrackerMenu.Add("wardtracker.FilterWards", new Slider("过滤眼", 250, 0, 600));
                wardtrackerMenu.Add("wardtracker.Hotkey", new KeyBind("按键", false, KeyBind.BindTypes.HoldActive, 16));
                wardtrackerMenu.Add("wardtracker.PermaShow", new CheckBox("菜单显示", false));
                wardtrackerMenu.Add("wardtracker.Enabled", new CheckBox("开启"));
            }

            this.Menu = wardtrackerMenu;
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
            this._greenWardTexture = Resources.WT_Green.ToTexture();
            this._pinkWardTexture = Resources.WT_Pink.ToTexture();

            this._sprite = MDrawing.GetSprite();
            this._text = MDrawing.GetFont(getSliderItem(this.Menu, "wardtracker.FontSize"));
            this._line = MDrawing.GetLine(getSliderItem(this.Menu, "wardtracker.CircleThickness"));

            Game.OnUpdate += this.OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += this.OnObjAiBaseProcessSpellCast;
            GameObject.OnCreate += this.OnGameObjectCreate;
            GameObject.OnDelete += this.OnGameObjectDelete;
            Drawing.OnEndScene += this.OnDrawingEndScene;
            Game.OnWndProc += this.OnGameWndProc;
            AttackableUnit.OnCreate += this.OnAttackableUnitEnterVisiblityClient;
        }

        #endregion

        #region Methods

        private void CheckDuplicateWards(WardObject wObj)
        {
            try
            {
                var range = getSliderItem(this.Menu, "wardtracker.FilterWards");
                if (wObj.Data.Duration != int.MaxValue)
                {
                    foreach (var obj in this._wardObjects.Where(w => w.Data.Duration != int.MaxValue).ToList())
                    {
                        if (wObj.Position.LSDistance(obj.Position) < range)
                        {
                            this._wardObjects.Remove(obj);
                            return;
                        }
                        if (obj.IsFromMissile && !obj.Corrected)
                        {
                            var newPoint = obj.StartPosition.LSExtend(obj.EndPosition, -(range * 1.5f));
                            if (wObj.Position.LSDistance(newPoint) < range)
                            {
                                this._wardObjects.Remove(obj);
                                return;
                            }
                        }
                    }
                }
                else
                {
                    foreach (var obj in
                        this._wardObjects.Where(
                            w =>
                            w.Data.Duration != int.MaxValue && w.IsFromMissile
                            && w.Position.LSDistance(wObj.Position) < 100).ToList())
                    {
                        this._wardObjects.Remove(obj);
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private WardStruct GetWardStructForInvisible(Vector3 start, Vector3 end)
        {
            return
                HeroManager.Enemies.Where(hero => this._heroNoWards.All(h => h.Hero.NetworkId != hero.NetworkId))
                    .Any(hero => hero.LSDistance(start.LSExtend(end, start.LSDistance(end) / 2f)) <= 1500f)
                && HeroManager.Enemies.Any(e => e.Level > 3)
                    ? this._wardStructs[3]
                    : this._wardStructs[0];
        }

        private void OnAttackableUnitEnterVisiblityClient(GameObject sender, EventArgs args)
        {
            try
            {
                if (!getCheckBoxItem(this.Menu, "wardtracker.Enabled"))
                {
                    return;
                }

                if (!sender.IsValid || sender.IsDead || !sender.IsEnemy)
                {
                    return;
                }
                var hero = sender as AIHeroClient;
                if (hero != null)
                {
                    if (ItemData.Sightstone.GetItem().IsOwned(hero) || ItemData.Ruby_Sightstone.GetItem().IsOwned(hero)
                        || ItemData.Vision_Ward.GetItem().IsOwned(hero))
                    {
                        this._heroNoWards.RemoveAll(h => h.Hero.NetworkId == hero.NetworkId);
                    }
                    else
                    {
                        this._heroNoWards.Add(new HeroWard(hero));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void OnDrawingEndScene(EventArgs args)
        {
            try
            {
                if (Drawing.Direct3DDevice == null || Drawing.Direct3DDevice.IsDisposed)
                {
                    return;
                }

                if (!getCheckBoxItem(this.Menu, "wardtracker.Enabled"))
                {
                    return;
                }

                var totalSeconds = getBoxItem(this.Menu, "wardtracker.TimeFormat") == 1;
                var circleRadius = getSliderItem(this.Menu, "wardtracker.CircleRadius");
                var circleThickness = getSliderItem(this.Menu, "wardtracker.CircleThickness");
                var visionRange = getCheckBoxItem(this.Menu, "wardtracker.VisionRange");
                var minimap = getCheckBoxItem(this.Menu, "wardtracker.Minimap");
                var greenCircle = getCheckBoxItem(this.Menu, "wardtracker.GreenCircle");
                var hotkey = getKeyBindItem(this.Menu, "wardtracker.Hotkey");
                var permaShow = getCheckBoxItem(this.Menu, "wardtracker.PermaShow");

                this._sprite.Begin(SpriteFlags.AlphaBlend);
                foreach (var ward in this._wardObjects)
                {
                    var color = System.Drawing.Color.Red;

                    if (ward.Data.Type == WardType.Green)
                    {
                        color = Color.Lime;
                    }

                    if (ward.Data.Type == WardType.Pink)
                    {
                        color = Color.Magenta;
                    }

                    if (ward.Position.LSIsOnScreen())
                    {
                        if (greenCircle || ward.Data.Type != WardType.Green)
                        {
                            if (ward.Object == null || !ward.Object.IsValid
                                || (ward.Object != null && ward.Object.IsValid && !ward.Object.IsVisible))
                            {
                                Render.Circle.DrawCircle(ward.Position, circleRadius, color, circleThickness);
                            }
                        }
                        if (ward.Data.Type == WardType.Green)
                        {
                            this._text.DrawTextCentered(
                                string.Format(
                                    "{0} {1} {0}",
                                    ward.IsFromMissile ? (ward.Corrected ? "?" : "??") : string.Empty,
                                    (ward.EndTime - Game.Time).FormatTime(totalSeconds)),
                                Drawing.WorldToScreen(ward.Position),
                                new SharpDX.Color(color.R, color.G, color.B, color.A));
                        }
                    }
                    if (minimap && ward.Data.Type != WardType.Trap)
                    {
                        this._sprite.DrawCentered(
                            ward.Data.Type == WardType.Green ? this._greenWardTexture : this._pinkWardTexture,
                            ward.MinimapPosition.LSTo2D());
                    }
                    if (hotkey || permaShow)
                    {
                        if (visionRange)
                        {
                            Render.Circle.DrawCircle(
                                ward.Position,
                                ward.Data.Range,
                                Color.FromArgb(30, color),
                                circleThickness);
                        }
                        if (ward.IsFromMissile)
                        {
                            this._line.Begin();
                            this._line.Draw(
                                new[]
                                    {
                                        Drawing.WorldToScreen(ward.StartPosition), Drawing.WorldToScreen(ward.EndPosition)
                                    },
                                SharpDX.Color.White);
                            this._line.End();
                        }
                    }
                }
                this._sprite.End();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void OnGameObjectCreate(GameObject sender, EventArgs args)
        {
            try
            {
                if (!getCheckBoxItem(this.Menu, "wardtracker.Enabled"))
                {
                    return;
                }

                var missile = sender as MissileClient;
                if (missile != null && missile.IsValid)
                {
                    if (missile.SpellCaster != null && !missile.SpellCaster.IsAlly && missile.SData != null)
                    {
                        if (missile.SData.Name.Equals("itemplacementmissile", StringComparison.OrdinalIgnoreCase)
                            && !missile.SpellCaster.IsVisible)
                        {
                            var sPos = missile.StartPosition;
                            var ePos = missile.EndPosition;

                            LeagueSharp.Common.Utility.DelayAction.Add(
                                1000,
                                delegate
                                {
                                    if (
                                        !_wardObjects.Any(
                                            w =>
                                            w.Position.LSTo2D().LSDistance(sPos.LSTo2D(), ePos.LSTo2D(), false) < 300
                                            && ((int)Game.Time - w.StartT < 2)))
                                    {
                                        var wObj = new WardObject(
                                            GetWardStructForInvisible(sPos, ePos),
                                            new Vector3(
                                                ePos.X,
                                                ePos.Y,
                                                NavMesh.GetHeightForPosition(ePos.X, ePos.Y)),
                                            (int)Game.Time,
                                            null,
                                            true,
                                            new Vector3(
                                                sPos.X,
                                                sPos.Y,
                                                NavMesh.GetHeightForPosition(sPos.X, sPos.Y)));
                                        CheckDuplicateWards(wObj);
                                        _wardObjects.Add(wObj);
                                    }
                                });
                        }
                    }
                }
                else
                {
                    var wardObject = sender as Obj_AI_Base;
                    if (wardObject != null && wardObject.IsValid && !wardObject.IsAlly)
                    {
                        foreach (var ward in this._wardStructs)
                        {
                            if (wardObject.CharData.BaseSkinName.Equals(
                                ward.ObjectBaseSkinName,
                                StringComparison.OrdinalIgnoreCase))
                            {
                                this._wardObjects.RemoveAll(
                                    w =>
                                    w.Position.LSDistance(wardObject.Position) < 300 && ((int)Game.Time - w.StartT < 0.5));
                                var wObj = new WardObject(
                                    ward,
                                    new Vector3(wardObject.Position.X, wardObject.Position.Y, wardObject.Position.Z),
                                    (int)(Game.Time - (int)(wardObject.MaxMana - wardObject.Mana)),
                                    wardObject);
                                this.CheckDuplicateWards(wObj);
                                this._wardObjects.Add(wObj);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void OnGameObjectDelete(GameObject sender, EventArgs args)
        {
            try
            {
                var ward = sender as Obj_AI_Base;
                if (ward != null && sender.Name.ToLower().Contains("Ward"))
                {
                    this._wardObjects.RemoveAll(w => w.Object != null && w.Object.NetworkId == sender.NetworkId);
                    this._wardObjects.RemoveAll(
                        w =>
                        (Math.Abs(w.Position.X - ward.Position.X) <= (w.IsFromMissile ? 25 : 10))
                        && (Math.Abs(w.Position.Y - ward.Position.Y) <= (w.IsFromMissile ? 25 : 10)));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void OnGameUpdate(EventArgs args)
        {
            try
            {
                if (this._lastCheck + CheckInterval > Environment.TickCount)
                {
                    return;
                }

                if (!getCheckBoxItem(this.Menu, "wardtracker.Enabled"))
                {
                    return;
                }

                this._lastCheck = Environment.TickCount;

                this._wardObjects.RemoveAll(
                    w =>
                    (w.EndTime <= Game.Time && w.Data.Duration != int.MaxValue)
                    || (w.Object != null && !w.Object.IsValid));
                foreach (var hw in this._heroNoWards.ToArray())
                {
                    if (hw.Hero.IsVisible)
                    {
                        hw.LastVisible = Game.Time;
                    }
                    else
                    {
                        if (Game.Time - hw.LastVisible >= 15)
                        {
                            this._heroNoWards.Remove(hw);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void OnGameWndProc(WndEventArgs args)
        {
            try
            {
                if (!getCheckBoxItem(this.Menu, "wardtracker.Enabled"))
                {
                    return;
                }

                if (args.Msg == (ulong)WindowsMessages.WM_LBUTTONDBLCLK
                    && getKeyBindItem(this.Menu, "wardtracker.Hotkey"))
                {
                    var ward = this._wardObjects.OrderBy(w => Game.CursorPos.LSDistance(w.Position)).FirstOrDefault();
                    if (ward != null && Game.CursorPos.LSDistance(ward.Position) <= 300)
                    {
                        this._wardObjects.Remove(ward);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void OnObjAiBaseProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            try
            {
                if (!getCheckBoxItem(this.Menu, "wardtracker.Enabled"))
                {
                    return;
                }

                if (sender.IsAlly)
                {
                    return;
                }

                foreach (var ward in this._wardStructs)
                {
                    if (args.SData.Name.Equals(ward.SpellName, StringComparison.OrdinalIgnoreCase))
                    {
                        var wObj = new WardObject(
                            ward,
                            ObjectManager.Player.GetPath(args.End).LastOrDefault(),
                            (int)Game.Time);
                        this.CheckDuplicateWards(wObj);
                        this._wardObjects.Add(wObj);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        private struct WardStruct
        {
            #region Fields

            public readonly int Duration;

            public readonly string ObjectBaseSkinName;

            public readonly int Range;

            public readonly string SpellName;

            public readonly WardType Type;

            #endregion

            #region Constructors and Destructors

            public WardStruct(int duration, int range, string objectBaseSkinName, string spellName, WardType type)
            {
                this.Duration = duration;
                this.Range = range;
                this.ObjectBaseSkinName = objectBaseSkinName;
                this.SpellName = spellName;
                this.Type = type;
            }

            #endregion
        }

        private class HeroWard
        {
            #region Constructors and Destructors

            public HeroWard(AIHeroClient hero)
            {
                this.Hero = hero;
                this.LastVisible = Game.Time;
            }

            #endregion

            #region Public Properties

            public AIHeroClient Hero { get; private set; }

            public float LastVisible { get; set; }

            #endregion
        }

        private class WardObject
        {
            #region Fields

            public readonly bool Corrected;

            public readonly Vector3 EndPosition;

            public readonly Vector3 MinimapPosition;

            public readonly Obj_AI_Base Object;

            public readonly Vector3 StartPosition;

            public readonly int StartT;

            private Vector3 _position;

            #endregion

            #region Constructors and Destructors

            public WardObject(
                WardStruct data,
                Vector3 position,
                int startT,
                Obj_AI_Base wardObject = null,
                bool isFromMissile = false,
                Vector3 startPosition = default(Vector3))
            {
                try
                {
                    var pos = position;
                    if (isFromMissile)
                    {
                        var newPos = this.GuessPosition(startPosition, position);
                        if (!position.X.Equals(newPos.X) || !position.Y.Equals(newPos.Y))
                        {
                            pos = newPos;
                            this.Corrected = true;
                        }
                        if (!this.Corrected)
                        {
                            pos = startPosition;
                        }
                    }
                    this.IsFromMissile = isFromMissile;
                    this.Data = data;
                    this.Position = this.RealPosition(pos);
                    this.EndPosition = this.Position.Equals(position) || this.Corrected
                                           ? position
                                           : this.RealPosition(position);
                    this.MinimapPosition = Drawing.WorldToMinimap(this.Position).To3D();
                    this.StartT = startT;
                    this.StartPosition = startPosition.Equals(default(Vector3)) || this.Corrected
                                             ? startPosition
                                             : this.RealPosition(startPosition);
                    this.Object = wardObject;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            #endregion

            #region Public Properties

            public WardStruct Data { get; private set; }

            public int EndTime
            {
                get
                {
                    return this.StartT + this.Data.Duration;
                }
            }

            public bool IsFromMissile { get; private set; }

            public Vector3 Position
            {
                get
                {
                    if (this.Object != null && this.Object.IsValid && this.Object.IsVisible)
                    {
                        this._position = this.Object.Position;
                    }
                    return this._position;
                }
                private set
                {
                    this._position = value;
                }
            }

            #endregion

            #region Methods

            private Vector3 GuessPosition(Vector3 start, Vector3 end)
            {
                try
                {
                    var grass = new List<Vector3>();
                    var distance = start.LSDistance(end);
                    for (var i = 0; i < distance; i++)
                    {
                        var pos = start.LSExtend(end, i);
                        if (NavMesh.IsWallOfGrass(pos, 1))
                        {
                            grass.Add(pos);
                        }
                    }
                    return grass.Count > 0 ? grass[(int)(grass.Count / 2d + 0.5d * Math.Sign(grass.Count / 2d))] : end;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                return end;
            }

            private Vector3 RealPosition(Vector3 end)
            {
                try
                {
                    if (end.LSIsWall())
                    {
                        for (var i = 0; i < 500; i = i + 5)
                        {
                            var c = new Geometry.Polygon.Circle(end, i, 15).Points;
                            foreach (var item in c.OrderBy(p => p.LSDistance(end)).Where(item => !item.LSIsWall()))
                            {
                                return new Vector3(item.X, item.Y, NavMesh.GetHeightForPosition(item.X, item.Y));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                return end;
            }

            #endregion
        }
    }
}