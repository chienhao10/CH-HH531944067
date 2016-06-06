namespace ElUtilitySuite.Trackers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Security.Permissions;

    using ElUtilitySuite.Vendor.SFX;

    using LeagueSharp;
    using LeagueSharp.Common;
    using LeagueSharp.Data;
    using LeagueSharp.Data.DataTypes;

    using SharpDX;
    using SharpDX.Direct3D9;

    using Color = System.Drawing.Color;
    using EloBuddy;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    internal class SpellCountdownTracker //: IPlugin
    {
        #region Constants

        private const int BoxHeight = 105;

        private const int BoxSpacing = 25;

        private const int BoxWidth = 235;

        private const int ColorIndicatorWidth = 10;

        private const int Countdown = 10;

        #endregion

        #region Fields

        private readonly int MoveRightSpeed = 300;

        private int StartX { get; set; } = Drawing.Width - BoxWidth;

        private int StartY { get; set; } = Drawing.Height - BoxHeight * 4;

        #endregion

        #region Properties

        private List<Card> Cards { get; } = new List<Card>();

        private Font CountdownFont { get; } = new Font(Drawing.Direct3DDevice, new System.Drawing.Font("Arial", 25));

        private Vector2 Padding { get; } = new Vector2(10, 5);

        private Font SpellNameFont { get; } = new Font(Drawing.Direct3DDevice, new System.Drawing.Font("Arial", 15));

        private Dictionary<string, Texture> Icons { get; set; } = new Dictionary<string, Texture>();

        private Sprite Sprite { get; set; } = new Sprite(Drawing.Direct3DDevice);

        /// <summary>
        ///     Gets or sets the spells.
        /// </summary>
        /// <value>
        ///     The spells.
        /// </value>
        private List<SpellDatabaseEntry> Spells { get; set; } = new List<SpellDatabaseEntry>();


        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public void CreateMenu(Menu rootMenu)
        {
            var menu = rootMenu.AddSubMenu("技能倒数", "SPELLCS");

            menu.Add("XPos", new Slider("X 位置", this.StartX, 0, Drawing.Width));
            menu.Add("YPos", new Slider("Y 位置", this.StartY, 0, Drawing.Height));
            menu.Add("DrawCards", new CheckBox("显示卡片"));

            menu["XPos"].Cast<Slider>().OnValueChange += (sender, args) => this.StartX = args.NewValue;
            menu["YPos"].Cast<Slider>().OnValueChange += (sender, args) => this.StartY = args.NewValue;

            this.Menu = menu;
        }

        private Menu Menu { get; set; }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        public void Load()
        {
            Game.OnUpdate += this.GameOnUpdate;
            Obj_AI_Base.OnProcessSpellCast += this.ObjAiBaseOnProcessSpellCast;
            Drawing.OnDraw += this.Drawing_OnDraw;
            JungleTracker.CampDied += this.JungleTrackerCampDied;
            Drawing.OnPreReset += args =>
            {
                this.SpellNameFont.OnLostDevice();
                this.CountdownFont.OnLostDevice();
                this.Sprite.OnLostDevice();
            };

            Drawing.OnPostReset += args =>
            {
                this.SpellNameFont.OnResetDevice();
                this.CountdownFont.OnResetDevice();
                this.Sprite.OnResetDevice();
            };


            try
            {
                var names =
                Assembly.GetExecutingAssembly().GetManifestResourceNames().Skip(1).ToList();
                Console.WriteLine(string.Join(" ", names));
                Console.WriteLine();

                foreach (var name in names)
                {
                    var spellName = name.Split('.')[3];
                    Console.WriteLine(spellName);
                    if (spellName != "Dragon" && spellName != "Baron")
                    {
                        this.Spells.Add(Data.Get<SpellDatabase>().Spells.First(x => x.SpellName.Equals(spellName)));
                    }
                    this.Icons.Add(
                    spellName,
                    Texture.FromStream(
                        Drawing.Direct3DDevice,
                        Assembly.GetExecutingAssembly().GetManifestResourceStream(name)));
                }


            }
            catch (Exception e)
            {

                Console.WriteLine(e);
            }

        }

        private void JungleTrackerCampDied(object sender, JungleTracker.JungleCamp e)
        {
            Chat.Print("Camp died!");
            if (!e.MobNames.Any(x => x.ToLower().Contains("baron") || x.ToLower().Contains("dragon")))
            {
                Chat.Print("wasnt baron or dragon!");
                return;
            }

            var card = new Card
            {
                EndTime = e.NextRespawnTime,
                StartTime = Game.Time,
                EndMessage = "Respawn",
                FriendlyName = e.MobNames.Any(x => x.ToLower().Contains("dragon")) ? "Dragon" : "Baron"
            };

            card.Name = card.FriendlyName;
            this.Cards.Add(card);
            Chat.Print("added baron or dragon card!");
        }

        #endregion

        #region Methods

        private static void DrawBox(
            Vector2 position,
            int width,
            int height,
            Color color,
            int borderwidth,
            Color borderColor)
        {
            Drawing.DrawLine(position.X, position.Y, position.X + width, position.Y, height, color);

            if (borderwidth <= 0)
            {
                return;
            }

            Drawing.DrawLine(position.X, position.Y, position.X + width, position.Y, borderwidth, borderColor);
            Drawing.DrawLine(
                position.X,
                position.Y + height,
                position.X + width,
                position.Y + height,
                borderwidth,
                borderColor);

            Drawing.DrawLine(position.X, position.Y + 1, position.X, position.Y + height + 1, borderwidth, borderColor);
            Drawing.DrawLine(
                position.X + width,
                position.Y + 1,
                position.X + width,
                position.Y + height + 1,
                borderwidth,
                borderColor);
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (Drawing.Direct3DDevice == null || Drawing.Direct3DDevice.IsDisposed)
            {
                return;
            }



            foreach (var card in this.Cards.Where(x => x.EndTime - Game.Time <= Countdown))
            {
                // draw spell
                var remainingTime = card.EndTime - Game.Time;
                var spellReady = remainingTime <= 0;

                var remainingTimePretty = remainingTime > 0 ? remainingTime.ToString("N1") : card.EndMessage;
                var i = this.Cards.IndexOf(card);

                var indicatorColor = spellReady ? Color.LawnGreen : Color.Yellow;

                // We only need to calculate the y axis since the boxes stack vertically
                var boxY = this.StartY - i * BoxSpacing - (i * BoxHeight);
                var boxX = this.StartX;

                if (remainingTime <= -5)
                {
                    boxX += (int)((-remainingTime - 5) * this.MoveRightSpeed);
                }

                var lineStart = new Vector2(boxX, boxY);

                DrawBox(lineStart, ColorIndicatorWidth, BoxHeight, indicatorColor, 0, new Color());

                // Draw the black rectangle
                var boxStart = new Vector2(boxX + ColorIndicatorWidth, boxY);
                DrawBox(boxStart, BoxWidth - ColorIndicatorWidth, BoxHeight, Color.Black, 0, new Color());


                // Draw spell name
                var spellNameStart = boxStart + this.Padding;
                this.SpellNameFont.DrawText(
                    null,
                    card.FriendlyName,
                    (int)spellNameStart.X,
                    (int)spellNameStart.Y,
                    new ColorBGRA(255, 255, 255, 255));

                // draw icon
                var textSize = this.SpellNameFont.MeasureText(null, card.FriendlyName);
                var iconStart = spellNameStart + new Vector2(0, textSize.Height + 5);

                var texture = this.Icons[card.Name];
                this.Sprite.Begin();
                this.Sprite.Draw(
                texture, new ColorBGRA(255, 255, 255, 255), null,
                new Vector3(-1 * iconStart, 0));
                this.Sprite.End();

                // draw countdown, add [icon size + padding]
                var countdownStart = iconStart + new Vector2(51 + 22, -7);
                this.CountdownFont.DrawText(
                    null,
                    remainingTimePretty,
                    (int)countdownStart.X,
                    (int)countdownStart.Y,
                    new ColorBGRA(255, 255, 255, 255));

                // Draw progress bar :(
                var countdownSize = this.CountdownFont.MeasureText(null, remainingTimePretty);
                var progressBarStart = countdownStart + new Vector2(0, countdownSize.Height + 9);
                var progressBarFullSize = 125;
                var cooldown = card.EndTime - card.StartTime;
                var progressBarActualSize = (cooldown - remainingTime) / cooldown * progressBarFullSize;

                if (progressBarActualSize > progressBarFullSize)
                {
                    progressBarActualSize = progressBarFullSize;
                }

                // MAGICERINO
                DrawBox(progressBarStart, progressBarFullSize, 15, Color.Black, 1, Color.LawnGreen);
                DrawBox(
                    progressBarStart + new Vector2(3, 3),
                    (int)(progressBarActualSize - 6),
                    15 - 5,
                    Color.LawnGreen,
                    0,
                    new Color());
            }
        }

        private void GameOnUpdate(EventArgs args)
        {
            this.Cards.RemoveAll(
                x =>
                x.EndTime - Game.Time <= -5
                && this.StartX + (int)((-(x.EndTime - Game.Time) - 5) * this.MoveRightSpeed)
                >= Drawing.Width + this.Cards.Count * this.MoveRightSpeed);
        }

        private void ObjAiBaseOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var data = Data.Get<SpellDatabase>().GetByName(args.SData.Name);
            if (!sender.IsEnemy
                || !this.Spells.Any(
                    x => x.SpellName.Equals(args.SData.Name, StringComparison.InvariantCultureIgnoreCase)) || Cards.Any(x => x.Name == data.SpellName))
            {
                return;
            }


            this.Cards.Add(
                new Card
                {
                    StartTime = Game.Time,
                    EndTime = Game.Time + args.SData.CooldownTime,
                    FriendlyName = $"{data.ChampionName} {data.Slot}",
                    Name = data.SpellName,
                    EndMessage = "Ready"
                });
        }

        #endregion

        private class Card
        {
            #region Public Properties

            public float EndTime { get; set; }

            public string FriendlyName { get; set; }

            public string Name { get; set; }

            public float StartTime { get; set; }

            public string EndMessage { get; set; }

            #endregion
        }
    }
}