namespace ElUtilitySuite.Summoners
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using SharpDX;
    using Color = System.Drawing.Color;
    using LeagueSharp.Common;

    // ReSharper disable once ClassNeverInstantiated.Global
    public class Smite : IPlugin
    {
        #region Static Fields

        public static Obj_AI_Minion Minion;

        private static readonly string[] BuffsThatActuallyMakeSenseToSmite =
            {
                "SRU_Red", "SRU_Blue",
                "SRU_Dragon_Water",  "SRU_Dragon_Fire", "SRU_Dragon_Earth", "SRU_Dragon_Air", "SRU_Dragon_Elder",
                "SRU_Baron", "SRU_Gromp", "SRU_Murkwolf",
                "SRU_Razorbeak", "SRU_RiftHerald",
                "SRU_Krug", "Sru_Crab", "TT_Spiderboss",
                "TT_NGolem", "TT_NWolf", "TT_NWraith"
            };

        #endregion

        #region Fields

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        ///     The spell type.
        /// </value>
        public SpellDataTargetType TargetType;

        #endregion

        #region Constructors and Destructors

        static Smite()
        {
            Spells = new List<Smite>
                         {
                             new Smite
                                 {
                                     ChampionName = "ChoGath", Range = 325f, Slot = SpellSlot.R, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Elise", Range = 475f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Fizz", Range = 550f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "LeeSin", Range = 1100f, Slot = SpellSlot.Q, Stage = 1,
                                     TargetType = SpellDataTargetType.Self
                                 },
                             new Smite
                                 {
                                     ChampionName = "MonkeyKing", Range = 375f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Nunu", Range = 300f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Olaf", Range = 325f, Slot = SpellSlot.E, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Pantheon", Range = 600f, Slot = SpellSlot.E, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Volibear", Range = 400f, Slot = SpellSlot.W, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "XinZhao", Range = 600f, Slot = SpellSlot.E, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Mundo", Range = 1050f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "KhaZix", Range = 325f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Evelynn", Range = 225f, Slot = SpellSlot.E, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Shen", Range = 520f, Slot = SpellSlot.E, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Diana", Range = 895f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Alistar", Range = 365f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Nocturne", Range = 1200f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Location
                                 },
                             new Smite
                                 {
                                     ChampionName = "Maokai", Range = 600f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Location
                                 },
                             new Smite
                                 {
                                     ChampionName = "Twitch", Range = 950f, Slot = SpellSlot.E, Stage = 0,
                                     TargetType = SpellDataTargetType.Self
                                 },
                             new Smite
                                 {
                                     ChampionName = "JarvanIV", Range = 770f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Location
                                 },
                             new Smite
                                 {
                                     ChampionName = "Rengar", Range = 325f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Aatrox", Range = 650f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Location
                                 },
                             new Smite
                                 {
                                     ChampionName = "Amumu", Range = 1100f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Gragas", Range = 600f, Slot = SpellSlot.E, Stage = 0,
                                     TargetType = SpellDataTargetType.Location
                                 },
                             new Smite
                                 {
                                     ChampionName = "Trundle", Range = 325f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Fiddlesticks", Range = 750f, Slot = SpellSlot.E, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Jax", Range = 700f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Ekko", Range = 1075f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Location
                                 },
                             new Smite
                                 {
                                     ChampionName = "Vi", Range = 325f, Slot = SpellSlot.E, Stage = 0,
                                     TargetType = SpellDataTargetType.Self
                                 },
                             new Smite
                                 {
                                     ChampionName = "Shaco", Range = 625f, Slot = SpellSlot.E, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Warwick", Range = 400f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Sejuani", Range = 625f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Location
                                 },
                             new Smite
                                 {
                                     ChampionName = "Tryndamere", Range = 660f, Slot = SpellSlot.E, Stage = 0,
                                     TargetType = SpellDataTargetType.Location
                                 },
                             new Smite
                                 {
                                     ChampionName = "Zac", Range = 550f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Location
                                 },
                             new Smite
                                 {
                                     ChampionName = "TahmKench", Range = 880f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Location
                                 },
                             new Smite
                                 {
                                     ChampionName = "Quinn", Range = 1025f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Location
                                 },
                             new Smite
                                 {
                                     ChampionName = "Poppy", Range = 525f, Slot = SpellSlot.E, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Kayle", Range = 650f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Hecarim", Range = 350f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                             new Smite
                                 {
                                     ChampionName = "Renekton", Range = 350f, Slot = SpellSlot.W, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 },
                              new Smite
                                 {
                                     ChampionName = "Irelia", Range = 750f, Slot = SpellSlot.Q, Stage = 0,
                                     TargetType = SpellDataTargetType.Unit
                                 }
                         };
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the spells.
        /// </summary>
        /// <value>
        ///     The spells.
        /// </value>
        public static List<Smite> Spells { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string ChampionName { get; set; }

        /// <summary>
        ///     Gets or sets the range.
        /// </summary>
        /// <value>
        ///     The range.
        /// </value>
        public float Range { get; set; }

        /// <summary>
        ///     Gets or sets the slot.
        /// </summary>
        /// <value>
        ///     The slot.
        /// </value>
        public SpellSlot Slot { get; set; }

        /// <summary>
        ///     Gets or sets the slot.
        /// </summary>
        /// <value>
        ///     The Smitespell
        /// </value>
        public LeagueSharp.Common.Spell SmiteSpell { get; set; }

        /// <summary>
        /// Gets or sets the slot.
        /// </summary>
        /// <value>
        ///     The stage.
        /// </value>
        public int Stage { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        private AIHeroClient Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the combo mode is active.
        /// </summary>
        /// <value>
        ///     <c>true</c> if combo mode is active; otherwise, <c>false</c>.
        /// </value>
        public bool ComboModeActive
        {
            get
            {
                return Entry.getComboMenu() || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public static Menu rootMenu = ElUtilitySuite.Entry.menu;
        public static Menu smiteMenu;

        public static bool getCheckBoxItem(string item)
        {
            return smiteMenu[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(string item)
        {
            return smiteMenu[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(string item)
        {
            return smiteMenu[item].Cast<KeyBind>().CurrentValue;
        }

        public void CreateMenu(Menu rootMenu)
        {
            var smiteSlot = this.Player.Spellbook.Spells.FirstOrDefault(x => x.Name.ToLower().Contains("smite"));

            if (smiteSlot == null)
            {
                return;
            }

            smiteMenu = rootMenu.AddSubMenu("Smite", "Smite");
            smiteMenu.Add("ElSmite.Activated", new KeyBind("Smite Activated", false, KeyBind.BindTypes.PressToggle, 'M'));
            smiteMenu.Add("Smite.Spell", new CheckBox("Use spell smite combo"));
            smiteMenu.Add("Smite.Ammo", new CheckBox("Save 1 smite charge"));

            if (Game.MapId == GameMapId.SummonersRift)
            {
                smiteMenu.AddSeparator();
                smiteMenu.AddGroupLabel("Mob Smites");
                smiteMenu.Add("SRU_Dragon_Air", new CheckBox("Air Dragon"));
                smiteMenu.Add("SRU_Dragon_Earth", new CheckBox("Earth Dragon"));
                smiteMenu.Add("SRU_Dragon_Fire", new CheckBox("Fire Dragon"));
                smiteMenu.Add("SRU_Dragon_Water", new CheckBox("Water Dragon"));
                smiteMenu.Add("SRU_Dragon_Elder", new CheckBox("Elder Dragon"));
                smiteMenu.Add("SRU_Baron", new CheckBox("Baron"));  
                smiteMenu.Add("SRU_Red", new CheckBox("Red buff"));
                smiteMenu.Add("SRU_Blue", new CheckBox("Blue buff"));
                smiteMenu.Add("SRU_RiftHerald", new CheckBox("Rift Herald"));

                smiteMenu.Add("SRU_Gromp", new CheckBox("Gromp", false));
                smiteMenu.Add("SRU_Murkwolf", new CheckBox("Wolves", false));
                smiteMenu.Add("SRU_Krug", new CheckBox("Krug", false));
                smiteMenu.Add("SRU_Razorbeak", new CheckBox("Chickens (Razorbeak)", false));
                smiteMenu.Add("Sru_Crab", new CheckBox("Crab", false));
                smiteMenu.AddSeparator();
            }

            if (Game.MapId == GameMapId.TwistedTreeline)
            {
                smiteMenu.AddSeparator();
                smiteMenu.AddGroupLabel("Mob Smites");
                smiteMenu.Add("TT_Spiderboss", new CheckBox("Vilemaw"));
                smiteMenu.Add("TT_NGolem", new CheckBox("Golem"));
                smiteMenu.Add("TT_NWolf", new CheckBox("Wolf"));
                smiteMenu.Add("TT_NWraith", new CheckBox("Wraith"));
                smiteMenu.AddSeparator();
            }

            //Champion Smite
            smiteMenu.AddGroupLabel("Champion Smite");
            smiteMenu.Add("ElSmite.KS.Activated", new CheckBox("Use smite to killsteal"));
            smiteMenu.Add("ElSmite.KS.Combo", new CheckBox("Use smite in combo"));
            smiteMenu.AddSeparator();

            //Drawings
            smiteMenu.AddGroupLabel("Drawings");
            smiteMenu.Add("ElSmite.Draw.Range", new CheckBox("Draw smite Range"));
            smiteMenu.Add("ElSmite.Draw.Text", new CheckBox("Draw smite text"));
            smiteMenu.Add("ElSmite.Draw.Damage", new CheckBox("Draw smite Damage", false));
        }

        public void Load()
        {
            try
            {
                var smiteSlot = this.Player.Spellbook.Spells.FirstOrDefault(x => x.Name.ToLower().Contains("smite"));


                if (smiteSlot != null)
                {
                    this.SmiteSpell = new LeagueSharp.Common.Spell(smiteSlot.Slot, 570f, DamageType.True);

                    Drawing.OnDraw += this.OnDraw;
                    Game.OnUpdate += this.OnUpdate;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion

        #region Methods

        private void ChampionSpellSmite(float damage, Obj_AI_Base mob)
        {
            try
            {
                foreach (var spell in
                    Spells.Where(
                        x =>
                        x.ChampionName.Equals(this.Player.ChampionName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    if (this.Player.LSGetSpellDamage(mob, spell.Slot, spell.Stage) + damage >= mob.Health)
                    {
                        if (mob.IsValidTarget(this.SmiteSpell.Range))
                        {
                            if (spell.TargetType == SpellDataTargetType.Unit)
                            {
                                this.Player.Spellbook.CastSpell(spell.Slot, mob);
                            }
                            else if (spell.TargetType == SpellDataTargetType.Self)
                            {
                                this.Player.Spellbook.CastSpell(spell.Slot);
                            }
                            else if (spell.TargetType == SpellDataTargetType.Location)
                            {
                                this.Player.Spellbook.CastSpell(spell.Slot, mob.ServerPosition);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        private void JungleSmite()
        {
            try
            {
                if (!getKeyBindItem("ElSmite.Activated"))
                {
                    return;
                }

                Minion = (Obj_AI_Minion) EntityManager.MinionsAndMonsters.Monsters.FirstOrDefault(buff => this.Player.IsInRange(buff, 570) && (buff.Name.StartsWith(buff.BaseSkinName) || BuffsThatActuallyMakeSenseToSmite.Contains(buff.BaseSkinName)) && !buff.Name.Contains("Mini") && !buff.Name.Contains("Spawn"));

                if (Minion == null)
                {
                    return;
                }

                if (getCheckBoxItem(Minion.CharData.BaseSkinName))
                {
                    if (this.SmiteSpell.IsReady())
                    {
                        if (Minion.IsValidTarget(570f))
                        {
                            if (this.Player.GetSummonerSpellDamage(Minion, LeagueSharp.Common.Damage.SummonerSpell.Smite) >= Minion.Health && this.SmiteSpell.CanCast(Minion))
                            {
                                this.SmiteSpell.Cast(Minion);
                            }
                        }

                        if (getCheckBoxItem("Smite.Spell"))
                        {
                            this.ChampionSpellSmite((float)this.Player.GetSummonerSpellDamage(Minion, LeagueSharp.Common.Damage.SummonerSpell.Smite), Minion);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        private void OnDraw(EventArgs args)
        {
            try
            {
                if (this.Player.IsDead)
                {
                    return;
                }
                var smiteActive = getKeyBindItem("ElSmite.Activated");
                var drawSmite = getCheckBoxItem("ElSmite.Draw.Range");
                var drawText = getCheckBoxItem("ElSmite.Draw.Text");
                var playerPos = Drawing.WorldToScreen(this.Player.Position);
                var drawDamage = getCheckBoxItem("ElSmite.Draw.Damage");

                if (smiteActive && this.SmiteSpell != null)
                {
                    if (drawText && this.Player.Spellbook.CanUseSpell(this.SmiteSpell.Slot) == SpellState.Ready)
                    {
                        Drawing.DrawText(playerPos.X - 70, playerPos.Y + 40, Color.GhostWhite, "Smite active");
                    }

                    if (drawText && this.Player.Spellbook.CanUseSpell(this.SmiteSpell.Slot) != SpellState.Ready)
                    {
                        Drawing.DrawText(playerPos.X - 70, playerPos.Y + 40, Color.Red, "Smite cooldown");
                    }

                    if (drawDamage && this.SmiteDamage() != 0)
                    {
                        var minions =
                            ObjectManager.Get<Obj_AI_Minion>()
                                .Where(
                                    m =>
                                    m.Team == GameObjectTeam.Neutral && m.IsValidTarget()
                                    && BuffsThatActuallyMakeSenseToSmite.Contains(m.CharData.BaseSkinName));

                        foreach (var minion in minions.Where(m => m.IsHPBarRendered))
                        {
                            var hpBarPosition = minion.HPBarPosition;
                            var maxHealth = minion.MaxHealth;
                            var sDamage = this.SmiteDamage();
                            //SmiteDamage : MaxHealth = x : 100
                            //Ratio math for this ^
                            var x = this.SmiteDamage() / maxHealth;
                            var barWidth = 0;

                            /*
                        * DON'T STEAL THE OFFSETS FOUND BY ASUNA DON'T STEAL THEM JUST GET OUT WTF MAN.
                        * EL SMITE IS THE BEST SMITE ASSEMBLY ON LEAGUESHARP AND YOU WILL NOT FIND A BETTER ONE.
                        * THE DRAWINGS ACTUALLY MAKE FUCKING SENSE AND THEY ARE FUCKING GOOD
                        * GTFO HERE SERIOUSLY OR I CALL DETUKS FOR YOU GUYS
                        * NO STEAL OR DMC FUCKING A REPORT.
                        * HELLO COPYRIGHT BY ASUNA 2015 ALL AUSTRALIAN RIGHTS RESERVED BY UNIVERSAL GTFO SERIOUSLY THO
                        * NO ALSO NO CREDITS JUST GET OUT DUDE GET OUTTTTTTTTTTTTTTTTTTTTTTT
                        */

                            switch (minion.CharData.BaseSkinName)
                            {
                                case "SRU_RiftHerald":
                                    barWidth = 145;
                                    Drawing.DrawLine(
                                        new Vector2(hpBarPosition.X + 3 + (float)(barWidth * x), hpBarPosition.Y + 17),
                                        new Vector2(hpBarPosition.X + 3 + (float)(barWidth * x), hpBarPosition.Y + 30),
                                        2f,
                                        Color.Chartreuse);
                                    Drawing.DrawText(
                                        hpBarPosition.X - 22 + (float)(barWidth * x),
                                        hpBarPosition.Y - 5,
                                        Color.Chartreuse,
                                        sDamage.ToString());
                                    break;

                                case "SRU_Dragon_Air":
                                case "SRU_Dragon_Water":
                                case "SRU_Dragon_Elder":
                                case "SRU_Dragon_Fire":
                                case "SRU_Dragon_Earth":
                                    barWidth = 145;
                                    Drawing.DrawLine(
                                        new Vector2(hpBarPosition.X + 3 + (float)(barWidth * x), hpBarPosition.Y + 22),
                                        new Vector2(hpBarPosition.X + 3 + (float)(barWidth * x), hpBarPosition.Y + 30),
                                        2f,
                                        Color.Chartreuse);
                                    Drawing.DrawText(
                                        hpBarPosition.X - 22 + (float)(barWidth * x),
                                        hpBarPosition.Y - 5,
                                        Color.Chartreuse,
                                        sDamage.ToString());
                                    break;

                                case "SRU_Red":
                                case "SRU_Blue":
                                    barWidth = 145;
                                    Drawing.DrawLine(
                                        new Vector2(hpBarPosition.X + 3 + (float)(barWidth * x), hpBarPosition.Y + 20),
                                        new Vector2(hpBarPosition.X + 3 + (float)(barWidth * x), hpBarPosition.Y + 30),
                                        2f,
                                        Color.Chartreuse);
                                    Drawing.DrawText(
                                        hpBarPosition.X - 22 + (float)(barWidth * x),
                                        hpBarPosition.Y - 5,
                                        Color.Chartreuse,
                                        sDamage.ToString());
                                    break;

                                case "SRU_Baron":
                                    barWidth = 194;
                                    Drawing.DrawLine(
                                        new Vector2(hpBarPosition.X + 18 + (float)(barWidth * x), hpBarPosition.Y + 20),
                                        new Vector2(hpBarPosition.X + 18 + (float)(barWidth * x), hpBarPosition.Y + 35),
                                        2f,
                                        Color.Chartreuse);
                                    Drawing.DrawText(
                                        hpBarPosition.X - 22 + (float)(barWidth * x),
                                        hpBarPosition.Y - 3,
                                        Color.Chartreuse,
                                        sDamage.ToString());
                                    break;

                                case "SRU_Gromp":
                                    barWidth = 87;
                                    Drawing.DrawLine(
                                        new Vector2(hpBarPosition.X + (float)(barWidth * x), hpBarPosition.Y + 11),
                                        new Vector2(hpBarPosition.X + (float)(barWidth * x), hpBarPosition.Y + 4),
                                        2f,
                                        Color.Chartreuse);
                                    Drawing.DrawText(
                                        hpBarPosition.X + (float)(barWidth * x),
                                        hpBarPosition.Y - 15,
                                        Color.Chartreuse,
                                        sDamage.ToString());
                                    break;

                                case "SRU_Murkwolf":
                                    barWidth = 75;
                                    Drawing.DrawLine(
                                        new Vector2(hpBarPosition.X + (float)(barWidth * x), hpBarPosition.Y + 11),
                                        new Vector2(hpBarPosition.X + (float)(barWidth * x), hpBarPosition.Y + 4),
                                        2f,
                                        Color.Chartreuse);
                                    Drawing.DrawText(
                                        hpBarPosition.X + (float)(barWidth * x),
                                        hpBarPosition.Y - 15,
                                        Color.Chartreuse,
                                        sDamage.ToString());
                                    break;

                                case "Sru_Crab":
                                    barWidth = 61;
                                    Drawing.DrawLine(
                                        new Vector2(hpBarPosition.X + (float)(barWidth * x), hpBarPosition.Y + 8),
                                        new Vector2(hpBarPosition.X + (float)(barWidth * x), hpBarPosition.Y + 4),
                                        2f,
                                        Color.Chartreuse);
                                    Drawing.DrawText(
                                        hpBarPosition.X + (float)(barWidth * x),
                                        hpBarPosition.Y - 15,
                                        Color.Chartreuse,
                                        sDamage.ToString());
                                    break;

                                case "SRU_Razorbeak":
                                    barWidth = 75;
                                    Drawing.DrawLine(
                                        new Vector2(hpBarPosition.X + (float)(barWidth * x), hpBarPosition.Y + 11),
                                        new Vector2(hpBarPosition.X + (float)(barWidth * x), hpBarPosition.Y + 4),
                                        2f,
                                        Color.Chartreuse);
                                    Drawing.DrawText(
                                        hpBarPosition.X + (float)(barWidth * x),
                                        hpBarPosition.Y - 15,
                                        Color.Chartreuse,
                                        sDamage.ToString());
                                    break;

                                case "SRU_Krug":
                                    barWidth = 81;
                                    Drawing.DrawLine(
                                        new Vector2(hpBarPosition.X + (float)(barWidth * x), hpBarPosition.Y + 11),
                                        new Vector2(hpBarPosition.X + (float)(barWidth * x), hpBarPosition.Y + 4),
                                        2f,
                                        Color.Chartreuse);
                                    Drawing.DrawText(
                                        hpBarPosition.X + (float)(barWidth * x),
                                        hpBarPosition.Y - 15,
                                        Color.Chartreuse,
                                        sDamage.ToString());
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    if (drawText && this.SmiteSpell != null)
                    {
                        Drawing.DrawText(playerPos.X - 70, playerPos.Y + 40, Color.Red, "Smite not active");
                    }
                }

                var smiteSpell = this.SmiteSpell;
                if (smiteSpell != null)
                {
                    if (smiteActive && drawSmite
                        && this.Player.Spellbook.CanUseSpell(smiteSpell.Slot) == SpellState.Ready)
                    {
                        Render.Circle.DrawCircle(this.Player.Position, 570, Color.Green);
                    }

                    if (drawSmite && this.Player.Spellbook.CanUseSpell(smiteSpell.Slot) != SpellState.Ready)
                    {
                        Render.Circle.DrawCircle(this.Player.Position, 570, Color.Red);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        private void OnUpdate(EventArgs args)
        {
            try
            {
                if (this.Player.IsDead)
                {
                    return;
                }

                if (this.SmiteSpell != null)
                {
                    this.JungleSmite();
                    this.SmiteKill();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        private float SmiteDamage()
        {
            try
            {
                return this.Player.Spellbook.GetSpell(this.SmiteSpell.Slot).State == SpellState.Ready
                           ? (float)this.Player.GetSummonerSpellDamage(Minion, LeagueSharp.Common.Damage.SummonerSpell.Smite)
                           : 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }

            return 0;
        }

        private void SmiteKill()
        {
            try
            {

                if (getCheckBoxItem("Smite.Ammo") && this.Player.GetSpell(this.SmiteSpell.Slot).Ammo == 1)
                {
                    return;
                }

                if (getCheckBoxItem("ElSmite.KS.Combo")
                    && this.Player.GetSpell(this.SmiteSpell.Slot).Name.ToLower() == "s5_summonersmiteduel"
                    && this.ComboModeActive)
                {
                    var smiteComboEnemy =
                        HeroManager.Enemies.FirstOrDefault(hero => !hero.IsZombie && hero.IsValidTarget(570));
                    if (smiteComboEnemy != null)
                    {
                        this.Player.Spellbook.CastSpell(this.SmiteSpell.Slot, smiteComboEnemy);
                    }
                }

                if (this.Player.GetSpell(this.SmiteSpell.Slot).Name.ToLower() != "s5_summonersmiteplayerganker")
                {
                    return;
                }

                if (getCheckBoxItem("ElSmite.KS.Activated"))
                {
                    var kSableEnemy =
                        HeroManager.Enemies.FirstOrDefault(
                            hero =>
                            !hero.IsZombie && hero.IsValidTarget(500) && 20 + 8 * this.Player.Level >= hero.Health);
                    if (kSableEnemy != null)
                    {
                        this.Player.Spellbook.CastSpell(this.SmiteSpell.Slot, kSableEnemy);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion
    }
}