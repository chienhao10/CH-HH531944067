namespace ElUtilitySuite.Trackers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Security.Permissions;

    using ElUtilitySuite.Vendor.SFX;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;
    using SharpDX.Direct3D9;

    using Font = SharpDX.Direct3D9.Font;
    using Rectangle = SharpDX.Rectangle;
    using EloBuddy;
    using EloBuddy.SDK.Menu;
    using PortAIO.Properties;
    using EloBuddy.SDK.Menu.Values;
    internal class SpellTracker : IPlugin
    {
        #region Constants

        private const float TeleportCd = 300f;

        #endregion

        #region Fields

        private readonly Dictionary<int, List<SpellDataInst>> _spellDatas = new Dictionary<int, List<SpellDataInst>>();

        private readonly SpellSlot[] _spellSlots = { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R };

        private readonly Dictionary<int, List<SpellDataInst>> _summonerDatas =
            new Dictionary<int, List<SpellDataInst>>();

        private readonly SpellSlot[] _summonerSlots = { SpellSlot.Summoner1, SpellSlot.Summoner2 };

        private readonly Dictionary<string, Texture> _summonerTextures = new Dictionary<string, Texture>();

        private readonly Dictionary<int, float> _teleports = new Dictionary<int, float>();

        private List<AIHeroClient> _heroes = new List<AIHeroClient>();

        private Texture _hudSelfTexture;

        private Texture _hudTexture;

        private Line _line;

        private Sprite _sprite;

        private Font _text;

        #endregion

        internal class ManualSpell
        {
            #region Constructors and Destructors

            public ManualSpell(string champ, string spell, SpellSlot slot, float[] cooldowns, float additional = 0)
            {
                this.Champ = champ;
                this.Spell = spell;
                this.Slot = slot;
                this.Cooldowns = cooldowns;
                this.Additional = additional;
            }

            #endregion

            #region Public Properties

            public float Additional { get; set; }

            public string Champ { get; private set; }

            public float Cooldown { get; set; }

            public float CooldownExpires { get; set; }

            public float[] Cooldowns { get; set; }

            public SpellSlot Slot { get; private set; }

            public string Spell { get; private set; }

            #endregion
        } // ReSharper disable StringLiteralTypo

        private readonly List<ManualSpell> _manualAllySpells = new List<ManualSpell>
                                                                   {
                                                                       new ManualSpell(
                                                                           "Lux",
                                                                           "LuxLightStrikeKugel",
                                                                           SpellSlot.E,
                                                                           new[] { 10f, 10f, 10f, 10f, 10f }),
                                                                       new ManualSpell(
                                                                           "Gragas",
                                                                           "GragasQ",
                                                                           SpellSlot.Q,
                                                                           new[] { 11f, 10f, 9f, 8f, 7f }),
                                                                       new ManualSpell(
                                                                           "Riven",
                                                                           "RivenFengShuiEngine",
                                                                           SpellSlot.R,
                                                                           new[] { 110f, 80f, 50f },
                                                                           15),
                                                                       new ManualSpell(
                                                                           "TwistedFate",
                                                                           "PickACard",
                                                                           SpellSlot.W,
                                                                           new[] { 6f, 6f, 6f, 6f, 6f }),
                                                                       new ManualSpell(
                                                                           "Velkoz",
                                                                           "VelkozQ",
                                                                           SpellSlot.Q,
                                                                           new[] { 7f, 7f, 7f, 7f, 7f },
                                                                           0.75f),
                                                                       new ManualSpell(
                                                                           "Xerath",
                                                                           "xeratharcanopulse2",
                                                                           SpellSlot.Q,
                                                                           new[] { 9f, 8f, 7f, 6f, 5f }),
                                                                       new ManualSpell(
                                                                           "Ziggs",
                                                                           "ZiggsW",
                                                                           SpellSlot.W,
                                                                           new[] { 26f, 24f, 22f, 20f, 18f }),
                                                                       new ManualSpell(
                                                                           "Rumble",
                                                                           "RumbleGrenade",
                                                                           SpellSlot.E,
                                                                           new[] { 10f, 10f, 10f, 10f, 10f }),
                                                                       new ManualSpell(
                                                                           "Riven",
                                                                           "RivenTriCleave",
                                                                           SpellSlot.Q,
                                                                           new[] { 13f, 13f, 13f, 13f, 13f }),
                                                                       new ManualSpell(
                                                                           "Fizz",
                                                                           "FizzJump",
                                                                           SpellSlot.E,
                                                                           new[] { 16f, 14f, 12f, 10f, 8f },
                                                                           0.75f)
                                                                   };

        private readonly List<ManualSpell> _manualEnemySpells = new List<ManualSpell>
                                                                    {
                                                                        new ManualSpell(
                                                                            "Lux",
                                                                            "LuxLightStrikeKugel",
                                                                            SpellSlot.E,
                                                                            new[] { 10f, 10f, 10f, 10f, 10f }),
                                                                        new ManualSpell(
                                                                            "Gragas",
                                                                            "GragasQ",
                                                                            SpellSlot.Q,
                                                                            new[] { 11f, 10f, 9f, 8f, 7f }),
                                                                        new ManualSpell(
                                                                            "Riven",
                                                                            "RivenFengShuiEngine",
                                                                            SpellSlot.R,
                                                                            new[] { 110f, 80f, 50f },
                                                                            15),
                                                                        new ManualSpell(
                                                                            "TwistedFate",
                                                                            "PickACard",
                                                                            SpellSlot.W,
                                                                            new[] { 6f, 6f, 6f, 6f, 6f }),
                                                                        new ManualSpell(
                                                                            "Velkoz",
                                                                            "VelkozQ",
                                                                            SpellSlot.Q,
                                                                            new[] { 7f, 7f, 7f, 7f, 7f },
                                                                            0.75f),
                                                                        new ManualSpell(
                                                                            "Xerath",
                                                                            "xeratharcanopulse2",
                                                                            SpellSlot.Q,
                                                                            new[] { 9f, 8f, 7f, 6f, 5f }),
                                                                        new ManualSpell(
                                                                            "Ziggs",
                                                                            "ZiggsW",
                                                                            SpellSlot.W,
                                                                            new[] { 26f, 24f, 22f, 20f, 18f }),
                                                                        new ManualSpell(
                                                                            "Rumble",
                                                                            "RumbleGrenade",
                                                                            SpellSlot.E,
                                                                            new[] { 10f, 10f, 10f, 10f, 10f }),
                                                                        new ManualSpell(
                                                                            "Riven",
                                                                            "RivenTriCleave",
                                                                            SpellSlot.Q,
                                                                            new[] { 13f, 13f, 13f, 13f, 13f }),
                                                                        new ManualSpell(
                                                                            "Fizz",
                                                                            "FizzJump",
                                                                            SpellSlot.E,
                                                                            new[] { 16f, 14f, 12f, 10f, 8f },
                                                                            0.75f)
                                                                    };

        public Menu Menu { get; set; }

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
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public void CreateMenu(Menu rootMenu)
        {
            var cooldownMenu = rootMenu.AddSubMenu("冷却计时", "cdddddtracker");
            {
                cooldownMenu.Add("cooldown-tracker-TimeFormat", new ComboBox("时间格式", 0, "mm:ss", "ss"));
                cooldownMenu.Add("cooldown-tracker-FontSize", new Slider("字体大小", 13, 3, 30));
                cooldownMenu.Add("cooldown-tracker-Enemy", new CheckBox("敌方"));
                cooldownMenu.Add("cooldown-tracker-Ally", new CheckBox("友军"));
                cooldownMenu.Add("cooldown-tracker-Self", new CheckBox("自己"));
                cooldownMenu.Add("cooldown-tracker-Enabled", new CheckBox("开启"));
            }

            this.Menu = cooldownMenu;

            this.Menu["cooldown-tracker-Enemy"].Cast<CheckBox>().OnValueChange += delegate (ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
            {
                if (_heroes == null)
                {
                    return;
                }
                var ally = getCheckBoxItem(this.Menu, "cooldown-tracker-Ally");
                var enemy = args.NewValue;
                _heroes = ally && enemy ? HeroManager.AllHeroes.ToList() : (ally ? HeroManager.Allies : (enemy ? HeroManager.Enemies : new List<AIHeroClient>())) .ToList();
                if (getCheckBoxItem(this.Menu, "cooldown-tracker-Self"))
                {
                    if (_heroes.All(h => h.NetworkId != ObjectManager.Player.NetworkId))
                    {
                        _heroes.Add(ObjectManager.Player);
                    }
                }
                else
                {
                    _heroes.RemoveAll(h => h.NetworkId == ObjectManager.Player.NetworkId);
                }
            };

            this.Menu["cooldown-tracker-Ally"].Cast<CheckBox>().OnValueChange += delegate (ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
            {
                if (_heroes == null)
                {
                    return;
                }
                var ally = args.NewValue;
                var enemy = getCheckBoxItem(this.Menu, "cooldown-tracker-Enemy");
                _heroes = ally && enemy
                              ? HeroManager.AllHeroes.ToList()
                              : (ally
                                     ? HeroManager.Allies
                                     : (enemy ? HeroManager.Enemies : new List<AIHeroClient>())).ToList();
                if (getCheckBoxItem(this.Menu, "cooldown-tracker-Self")
                    && _heroes.All(h => h.NetworkId != ObjectManager.Player.NetworkId))
                {
                    _heroes.Add(ObjectManager.Player);
                }
                if (getCheckBoxItem(this.Menu, "cooldown-tracker-Self"))
                {
                    if (_heroes.All(h => h.NetworkId != ObjectManager.Player.NetworkId))
                    {
                        _heroes.Add(ObjectManager.Player);
                    }
                }
                else
                {
                    _heroes.RemoveAll(h => h.NetworkId == ObjectManager.Player.NetworkId);
                }
            };

            this.Menu["cooldown-tracker-Self"].Cast<CheckBox>().OnValueChange += delegate (ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
            {
                if (_heroes == null)
                {
                    return;
                }
                var ally = getCheckBoxItem(this.Menu, "cooldown-tracker-Ally");
                var enemy = getCheckBoxItem(this.Menu, "cooldown-tracker-Enemy");
                _heroes = ally && enemy
                              ? HeroManager.AllHeroes.ToList()
                              : (ally ? HeroManager.Allies : (enemy ? HeroManager.Enemies : new List<AIHeroClient>()))
                                    .ToList();
                if (args.NewValue)
                {
                    if (_heroes.All(h => h.NetworkId != ObjectManager.Player.NetworkId))
                    {
                        _heroes.Add(ObjectManager.Player);
                    }
                }
                else
                {
                    _heroes.RemoveAll(h => h.NetworkId == ObjectManager.Player.NetworkId);
                }
            };
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            try
            {
                this._hudTexture = Resources.CD_Hud.ToTexture();
                this._hudSelfTexture = Resources.CD_HudSelf.ToTexture();

                foreach (var enemy in HeroManager.AllHeroes)
                {
                    var lEnemy = enemy;
                    this._spellDatas.Add(
                        enemy.NetworkId,
                        this._spellSlots.Select(slot => lEnemy.GetSpell(slot)).ToList());
                    this._summonerDatas.Add(
                        enemy.NetworkId,
                        this._summonerSlots.Select(slot => lEnemy.GetSpell(slot)).ToList());
                }

                foreach (var sName in
                    HeroManager.AllHeroes.SelectMany(
                        h =>
                        this._summonerSlots.Select(summoner => h.Spellbook.GetSpell(summoner).Name.ToLower())
                            .Where(sName => !this._summonerTextures.ContainsKey(FixName(sName)))))
                {
                    this._summonerTextures[FixName(sName)] =
                        ((Bitmap)Resources.ResourceManager.GetObject(string.Format("CD_{0}", FixName(sName)))
                         ?? Resources.CD_SummonerBarrier).ToTexture();
                }

                this._heroes = getCheckBoxItem(this.Menu, "cooldown-tracker-Ally")
                               && getCheckBoxItem(this.Menu, "cooldown-tracker-Enemy")
                                   ? HeroManager.AllHeroes.ToList()
                                   : (getCheckBoxItem(this.Menu, "cooldown-tracker-Ally")
                                          ? HeroManager.Allies
                                          : (getCheckBoxItem(this.Menu, "cooldown-tracker-Enemy")
                                                 ? HeroManager.Enemies
                                                 : new List<AIHeroClient>())).ToList();

                if (!getCheckBoxItem(this.Menu, "cooldown-tracker-Self"))
                {
                    this._heroes.RemoveAll(h => h.NetworkId == ObjectManager.Player.NetworkId);
                }

                this._sprite = MDrawing.GetSprite();
                this._line = MDrawing.GetLine(4);
                this._text = MDrawing.GetFont(getSliderItem(this.Menu, "cooldown-tracker-FontSize"));

                Drawing.OnEndScene += this.OnDrawingEndScene;
                Obj_AI_Base.OnProcessSpellCast += this.OnObjAiBaseProcessSpellCast;
                Obj_AI_Base.OnTeleport += this.OnObjAiBaseTeleport;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void OnObjAiBaseTeleport(Obj_AI_Base sender, GameObjectTeleportEventArgs args)
        {
            try
            {
                if (!getCheckBoxItem(this.Menu, "cooldown-tracker-Enabled"))
                {
                    return;
                }

                var packet = Packet.S2C.Teleport.Decoded(sender, args);
                if (packet.Type == Packet.S2C.Teleport.Type.Teleport
                    && (packet.Status == Packet.S2C.Teleport.Status.Finish
                        || packet.Status == Packet.S2C.Teleport.Status.Abort))
                {
                    var time = Game.Time;
                    LeagueSharp.Common.Utility.DelayAction.Add(
                        250,
                        delegate
                        {
                            var cd = packet.Status == Packet.S2C.Teleport.Status.Finish ? 300 : 200;
                            _teleports[packet.UnitNetworkId] = time + cd;
                        });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static string FixName(string name)
        {
            try
            {
                return name.ToLower().Contains("smite")
                           ? "summonersmite"
                           : (name.ToLower().Contains("teleport") ? "summonerteleport" : name.ToLower());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return name;
        }

        public static string[] a = { "Riven" };

        private void OnDrawingEndScene(EventArgs args)
        {
            try
            {

                if (!getCheckBoxItem(this.Menu, "cooldown-tracker-Enabled"))
                {
                    return;
                }

                if (Drawing.Direct3DDevice == null || Drawing.Direct3DDevice.IsDisposed)
                {
                    return;
                }
                var totalSeconds = getBoxItem(this.Menu, "cooldown-tracker-TimeFormat")
                                   == 1;
                foreach (var hero in
                    this._heroes.Where(
                        hero => hero != null && hero.IsValid && hero.IsHPBarRendered && hero.Position.LSIsOnScreen()))
                {
                    try
                    {
                        var lHero = hero;
                        if (!hero.Position.IsValid() || !hero.HPBarPosition.IsValid())
                        {
                            return;
                        }

                        var x = (int)hero.HPBarPosition.X;
                        var y = (int)hero.HPBarPosition.Y;

                        if (!a.Contains(hero.ChampionName) && hero.IsEnemy)
                        {
                            x += -19;
                            y += 2;
                        }

                        if (!a.Contains(hero.ChampionName) && hero.IsMe)
                        {
                            x += 3;
                            y += -3;
                        }

                        if (hero.ChampionName.Equals("Riven") && hero.IsMe)
                        {
                            x += 5;
                            y += -9;
                        }

                        this._sprite.Begin(SpriteFlags.AlphaBlend);
                        var summonerData = this._summonerDatas[hero.NetworkId];
                        for (var i = 0; i < summonerData.Count; i++)
                        {
                            var spell = summonerData[i];
                            if (spell != null)
                            {
                                var teleportCd = 0f;
                                if (spell.Name.Contains("Teleport") && this._teleports.ContainsKey(hero.NetworkId))
                                {
                                    this._teleports.TryGetValue(hero.NetworkId, out teleportCd);
                                }
                                var t = teleportCd > 0.1f
                                            ? teleportCd - Game.Time
                                            : (spell.IsReady() ? 0 : spell.CooldownExpires - Game.Time);
                                var sCd = teleportCd > 0.1f ? TeleportCd : spell.Cooldown;
                                var percent = Math.Abs(sCd) > float.Epsilon ? t / sCd : 1f;
                                var n = t > 0 ? (int)(19 * (1f - percent)) : 19;
                                if (t > 0)
                                {
                                    this._text.DrawTextCentered(
                                        t.FormatTime(totalSeconds),
                                        x - (hero.IsMe ? -160 : 13),
                                        y + 7 + 13 * i,
                                        new ColorBGRA(255, 255, 255, 255));
                                }
                                if (this._summonerTextures.ContainsKey(FixName(spell.Name)))
                                {
                                    this._sprite.Draw(
                                        this._summonerTextures[FixName(spell.Name)],
                                        new ColorBGRA(255, 255, 255, 255),
                                        new Rectangle(0, 12 * n, 12, 12),
                                        new Vector3(-x - (hero.IsMe ? 132 : 3), -y - 1 - 13 * i, 0));
                                }
                            }
                        }

                        this._sprite.Draw(
                            hero.IsMe ? this._hudSelfTexture : this._hudTexture,
                            new ColorBGRA(255, 255, 255, 255),
                            null,
                            new Vector3(-x, -y, 0));

                        this._sprite.End();

                        var x2 = x + (hero.IsMe ? 24 : 19);
                        var y2 = y + 21;

                        this._line.Begin();
                        var spellData = this._spellDatas[hero.NetworkId];
                        foreach (var spell in spellData)
                        {
                            var lSpell = spell;
                            if (spell != null)
                            {
                                var spell1 = spell;
                                var manual = hero.IsAlly
                                                 ? this._manualAllySpells.FirstOrDefault(
                                                     m =>
                                                     m.Slot.Equals(lSpell.Slot)
                                                     && m.Champ.Equals(
                                                         lHero.ChampionName,
                                                         StringComparison.OrdinalIgnoreCase))
                                                 : this._manualEnemySpells.FirstOrDefault(
                                                     m =>
                                                     m.Slot.Equals(spell1.Slot)
                                                     && m.Champ.Equals(
                                                         lHero.ChampionName,
                                                         StringComparison.OrdinalIgnoreCase));
                                var t = (manual != null ? manual.CooldownExpires : spell.CooldownExpires) - Game.Time;
                                var spellCooldown = manual != null ? manual.Cooldown : spell.Cooldown;
                                var percent = t > 0 && Math.Abs(spellCooldown) > float.Epsilon
                                                  ? 1f - t / spellCooldown
                                                  : 1f;
                                if (t > 0 && t < 100)
                                {
                                    this._text.DrawTextCentered(
                                        t.FormatTime(totalSeconds),
                                        x2 + 27 / 2,
                                        y2 + 13,
                                        new ColorBGRA(255, 255, 255, 255));
                                }

                                if (spell.Level > 0)
                                {
                                    this._line.Draw(
                                        new[] { new Vector2(x2, y2), new Vector2(x2 + percent * 23, y2) },
                                        t > 0 ? new ColorBGRA(235, 137, 0, 255) : new ColorBGRA(0, 168, 25, 255));
                                }
                                x2 = x2 + 27;
                            }
                        }
                        this._line.End();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("An error occurred: '{0}'", e);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        private void OnObjAiBaseProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            try
            {
                if (!getCheckBoxItem(this.Menu, "cooldown-tracker-Enabled"))
                {
                    return;
                }

                var hero = sender as AIHeroClient;
                if (hero != null)
                {
                    var data = hero.IsAlly
                                   ? this._manualAllySpells.FirstOrDefault(
                                       m => m.Spell.Equals(args.SData.Name, StringComparison.OrdinalIgnoreCase))
                                   : this._manualEnemySpells.FirstOrDefault(
                                       m => m.Spell.Equals(args.SData.Name, StringComparison.OrdinalIgnoreCase));
                    if (data != null && data.CooldownExpires - Game.Time < 0.5)
                    {
                        var spell = hero.GetSpell(data.Slot);
                        if (spell != null)
                        {
                            var cooldown = data.Cooldowns[spell.Level - 1];
                            var cdr = hero.PercentCooldownMod * -1 * 100;
                            data.Cooldown = cooldown - cooldown / 100 * (cdr > 40 ? 40 : cdr) + data.Additional;
                            data.CooldownExpires = Game.Time + data.Cooldown;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}