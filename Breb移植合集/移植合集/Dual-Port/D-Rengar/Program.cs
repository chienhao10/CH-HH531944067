using System;
using System.Linq;
using EloBuddy;
using LeagueSharp.Common;
using EloBuddy.SDK;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;
using Damage = LeagueSharp.Common.Damage;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System.Drawing;

namespace D_Rengar
{
    internal class Program
    {
        private static AIHeroClient _player;

        private static Spell _q, _w, _e, _r;



        private static SpellSlot _igniteSlot;

        private static Items.Item _youmuu, _tiamat, _hydra, _blade, _bilge, _rand, _lotis;

        private static SpellSlot _smiteSlot;

        private static Menu Menu { get; set; }

        public static Menu comboMenu, harassMenu, itemMenu, clearMenu, miscMenu, lasthitMenu, jungleMenu, drawMenu, smiteMenu;

        private static Spell _smite;

        private static int _lastTick;

        public static void Game_OnGameLoad()
        {
            _player = ObjectManager.Player;
            if (_player.ChampionName != "Rengar") return;
            _q = new Spell(SpellSlot.Q, 250f);
            _w = new Spell(SpellSlot.W, 400);
            _e = new Spell(SpellSlot.E, 980f);
            _r = new Spell(SpellSlot.R, 2000f);

            _e.SetSkillshot(0.125f, 70f, 1500f, true, SkillshotType.SkillshotLine);

            _bilge = new Items.Item(3144, 450f);
            _blade = new Items.Item(3153, 450f);
            _hydra = new Items.Item(3074, 250f);
            _tiamat = new Items.Item(3077, 250f);
            _rand = new Items.Item(3143, 490f);
            _lotis = new Items.Item(3190, 590f);
            _youmuu = new Items.Item(3142, 10);
            _igniteSlot = _player.GetSpellSlot("SummonerDot");

            if (_player.GetSpell(SpellSlot.Summoner1).Name.ToLower().Contains("smite"))
            {
                _smite = new Spell(SpellSlot.Summoner1, 570f);
                _smiteSlot = SpellSlot.Summoner1;
            }
            else if (_player.GetSpell(SpellSlot.Summoner2).Name.ToLower().Contains("smite"))
            {
                _smite = new Spell(SpellSlot.Summoner2, 570f);
                _smiteSlot = SpellSlot.Summoner2;
            }

            Menu = MainMenu.AddMenu("D-Rengar", "D-Rengar");

            comboMenu = Menu.AddSubMenu("Combo", "Combo");
            comboMenu.Add("Switch", new KeyBind("Switch Empowered Priority", false, KeyBind.BindTypes.HoldActive, 'T'));
            comboMenu.Add("ComboPrio", new ComboBox("Empowered Priority", 0, "Q", "W", "E"));
            comboMenu.Add("smitecombo", new CheckBox("Use Smite in target"));
            comboMenu.Add("UseQC", new CheckBox("Use Q"));
            comboMenu.Add("UseWC", new CheckBox("Use W"));
            comboMenu.Add("UseEC", new CheckBox("Use E"));
            comboMenu.Add("UseEEC", new CheckBox("Use Empower E when Q(range) < target(range)"));

            itemMenu = Menu.AddSubMenu("Items", "items");
            itemMenu.Add("Youmuu", new CheckBox("Use Youmuu's"));
            itemMenu.Add("Tiamat", new CheckBox("Use Tiamat"));
            itemMenu.Add("Hydra", new CheckBox("Use Hydra"));
            itemMenu.AddSeparator();
            itemMenu.Add("Bilge", new CheckBox("Use Bilge"));
            itemMenu.Add("BilgeEnemyhp", new Slider("If Enemy Hp <", 85, 1, 100));
            itemMenu.Add("Bilgemyhp", new Slider("Or Your Hp <", 85, 1, 100));
            itemMenu.Add("Blade", new CheckBox("Use Bork"));
            itemMenu.Add("BladeEnemyhp", new Slider("If Enemy Hp <", 85, 1, 100));
            itemMenu.Add("Blademyhp", new Slider("Or Your Hp <", 85, 1, 100));
            itemMenu.AddLabel("Deffensive Items");
            itemMenu.AddSeparator();
            itemMenu.Add("useqss", new CheckBox("Use QSS/Mercurial Scimitar/Dervish Blade"));
            itemMenu.Add("blind", new CheckBox("Blind"));
            itemMenu.Add("charm", new CheckBox("Charm"));
            itemMenu.Add("fear", new CheckBox("Fear"));
            itemMenu.Add("flee", new CheckBox("Flee"));
            itemMenu.Add("taunt", new CheckBox("Taunt"));
            itemMenu.Add("snare", new CheckBox("Snare"));
            itemMenu.Add("suppression", new CheckBox("Suppression"));
            itemMenu.Add("stun", new CheckBox("Stun"));
            itemMenu.Add("polymorph", new CheckBox("Polymorph"));
            itemMenu.Add("silence", new CheckBox("Silence"));
            itemMenu.Add("Cleansemode", new ComboBox("Use Cleanse", 1, "Always", "In Combo"));
            itemMenu.AddLabel("Potions");
            itemMenu.AddSeparator();
            itemMenu.Add("usehppotions", new CheckBox("Use Healt potion/Refillable/Hunters/Corrupting/Biscuit"));
            itemMenu.Add("usepotionhp", new Slider("If Health % <", 35, 1, 100));
            itemMenu.Add("usemppotions", new CheckBox("Use Hunters/Corrupting/Biscuit"));
            itemMenu.Add("usepotionmp", new Slider("If Mana % <", 35, 1, 100));


            harassMenu = Menu.AddSubMenu("Harass", "Harass");
            harassMenu.Add("HarrPrio", new ComboBox("Empowered Priority", 2, "Q", "W", "E"));
            harassMenu.Add("UseQH", new CheckBox("Use Q"));
            harassMenu.Add("UseWH", new CheckBox("Use W"));
            harassMenu.Add("UseEH", new CheckBox("Use E"));
            harassMenu.Add("harasstoggle", new KeyBind("AutoHarass (toggle)", false, KeyBind.BindTypes.HoldActive, 'L'));


            lasthitMenu = Menu.AddSubMenu("LastHit", "LastHit");
            lasthitMenu.Add("LastPrio", new ComboBox("Empowered Priority", 0, "Q", "W", "E"));
            lasthitMenu.Add("LastSave", new CheckBox("Save Ferocity"));
            lasthitMenu.Add("UseQLH", new CheckBox("Q LastHit"));
            lasthitMenu.Add("UseWLH", new CheckBox("W LastHit"));
            lasthitMenu.Add("UseELH", new CheckBox("E LastHit"));

            clearMenu = Menu.AddSubMenu("LaneClear", "LaneClear");
            clearMenu.Add("LanePrio", new ComboBox("Empowered Priority", 0, "Q", "W", "E"));
            clearMenu.Add("LaneSave", new CheckBox("Save Ferocity"));
            clearMenu.Add("UseItemslane", new CheckBox("Use Items"));
            clearMenu.Add("UseQL", new CheckBox("Q LaneClear"));
            clearMenu.Add("UseWL", new CheckBox("W LaneClear"));
            clearMenu.Add("UseEL", new CheckBox("E LaneClear"));

            jungleMenu = Menu.AddSubMenu("JungleClear", "JungleClear");
            jungleMenu.Add("JunglePrio", new ComboBox("Empowered Priority", 0, "Q", "W", "E"));
            jungleMenu.Add("JungleSave", new CheckBox("Save Ferocity"));
            jungleMenu.Add("UseItemsjungle", new CheckBox("Use Items"));
            jungleMenu.Add("UseQJ", new CheckBox("Q JungleClear"));
            jungleMenu.Add("UseWJ", new CheckBox("W JungleClear"));
            jungleMenu.Add("UseEJ", new CheckBox("E JungleClear"));

            smiteMenu = Menu.AddSubMenu("Smite", "Smite");
            smiteMenu.Add("Usesmite", new KeyBind("Use Smite(toggle)", false, KeyBind.BindTypes.PressToggle, 'N'));
            smiteMenu.Add("Usered", new CheckBox("Smite Red Early"));
            smiteMenu.Add("healthJ", new Slider("Smite Red Early if HP% <", 35, 1, 100));

            miscMenu = Menu.AddSubMenu("Misc", "Misc");
            miscMenu.Add("UseIgnite", new CheckBox("Use Ignite KillSteal"));
            miscMenu.Add("UseQM", new CheckBox("Use Q KillSteal"));
            miscMenu.Add("UseWM", new CheckBox("Use W KillSteal"));
            miscMenu.Add("UseRM", new CheckBox("Use R KillSteal"));
            miscMenu.Add("UseEInt", new CheckBox("E to Interrupt"));
            miscMenu.Add("AutoW", new CheckBox("use W to Heal"));
            miscMenu.Add("AutoWHP", new Slider("If Health % <", 35, 1, 100));
            miscMenu.Add("Echange", new ComboBox("E Hit", 3, "Low", "Medium", "High", "Very High"));


            drawMenu = Menu.AddSubMenu("Drawings", "Drawings");
            drawMenu.Add("DrawQ", new CheckBox("Draw Q", false));
            drawMenu.Add("DrawW", new CheckBox("Draw W", false));
            drawMenu.Add("DrawE", new CheckBox("Draw E", false));
            drawMenu.Add("Drawsmite", new CheckBox("Draw smite", true));
            drawMenu.Add("Drawharass", new CheckBox("Draw AutoHarass", true));
            drawMenu.Add("combomode", new CheckBox("Draw Combo Mode", true));
            drawMenu.Add("DamageAfterCombo", new CheckBox("Draw damage after combo", true));

            LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = true;
            LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnit = ComboDamage;


            Chat.Print("<font color='#881df2'>D-Rengar by Diabaths</font> Loaded.");
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Orbwalker.OnPreAttack += OnBeforeAttack;
            Orbwalker.OnPostAttack += OnAfterAttack;
            CustomEvents.Unit.OnDash += Dash;
            Chat.Print(
                "<font color='#f2f21d'>Do you like it???  </font> <font color='#ff1900'>Drop 1 Upvote in Database </font>");
            Chat.Print(
                "<font color='#f2f21d'>Buy me cigars </font> <font color='#ff1900'>ssssssssssmith@hotmail.com</font> (10) S");
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

        private static void Game_OnGameUpdate(EventArgs args)
        {
            LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = getCheckBoxItem(drawMenu, "DamageAfterCombo");
            if (_player.IsDead) return;
            if (getCheckBoxItem(miscMenu, "AutoW") && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                AutoHeal();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)
                && (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)
                    || getKeyBindItem(harassMenu, "harasstoggle")))
            {
                Harass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                Laneclear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                JungleClear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                LastHit();
            }

            Usepotion();
            if (getKeyBindItem(smiteMenu, "Usesmite"))
            {
                Smiteuse();
            }

            _player = ObjectManager.Player;

            Orbwalker.DisableAttacking = false;

            KillSteal();
            ChangeComboMode();
        }

        public static void Dash(Obj_AI_Base sender, Dash.DashItem args)
        {
            var useQ = getCheckBoxItem(comboMenu, "UseQC");
            var useW = getCheckBoxItem(comboMenu, "UseWC");
            var useE = getCheckBoxItem(comboMenu, "UseEC");
            var useEE = getCheckBoxItem(comboMenu, "UseEEC");
            var iYoumuu = getCheckBoxItem(itemMenu, "Youmuu");
            var iTiamat = getCheckBoxItem(itemMenu, "Tiamat");
            var iHydra = getCheckBoxItem(itemMenu, "Hydra");
            if (!sender.IsMe) return;
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (_player.Mana <= 4)
                {
                    if (useQ)
                    {
                        var tq = TargetSelector.GetTarget(_q.Range, DamageType.Physical);
                        if (tq.LSIsValidTarget(_q.Range) && _q.IsReady()) _q.Cast();
                    }

                    var th = TargetSelector.GetTarget(_e.Range, DamageType.Magical);

                    if (iTiamat && _tiamat.IsReady() && th.LSIsValidTarget(_tiamat.Range))
                    {
                        _tiamat.Cast();
                    }

                    if (iHydra && _hydra.IsReady() && th.LSIsValidTarget(_hydra.Range))
                    {
                        _hydra.Cast();
                    }


                    if (useE)
                    {
                        var te = TargetSelector.GetTarget(_e.Range, DamageType.Physical);
                        var predE = _e.GetPrediction(te);
                        if (te.LSIsValidTarget(_e.Range) && _e.IsReady()
                            && predE.Hitchance >= Echange() && predE.CollisionObjects.Count == 0)
                            _e.Cast(te);
                    }
                }

                if (_player.Mana == 5)
                {
                    var tq = TargetSelector.GetTarget(_q.Range, DamageType.Physical);
                    if (useQ
                        && (getBoxItem(comboMenu, "ComboPrio") == 1
                            || (getBoxItem(comboMenu, "ComboPrio") == 2
                              && Orbwalking.InAutoAttackRange(tq))))
                        if (tq.LSIsValidTarget(_q.Range) && _q.IsReady()) _q.Cast();
                    var th = TargetSelector.GetTarget(_e.Range, DamageType.Magical);

                    if (iTiamat && _tiamat.IsReady() && th.LSIsValidTarget(_tiamat.Range))
                    {
                        _tiamat.Cast();
                    }

                    if (iHydra && _hydra.IsReady() && th.IsValidTarget(_hydra.Range))
                    {
                        _hydra.Cast();
                    }

                    if (useE && getBoxItem(comboMenu, "ComboPrio") == 2)
                    {
                        var te = TargetSelector.GetTarget(_e.Range, DamageType.Physical);
                        var predE = _e.GetPrediction(te);
                        if (te.LSIsValidTarget(_e.Range) && _e.IsReady() && predE.Hitchance >= Echange()
                            && predE.CollisionObjects.Count == 0)
                            _e.Cast(te);
                    }
                }
            }
        }

        private static void OnAfterAttack(AttackableUnit target, EventArgs args)
        {
            var combo = Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
            var Q = getCheckBoxItem(comboMenu, "UseQC");
            if (!target.IsMe) return;
            if (combo && _q.IsReady() && Q && target.LSIsValidTarget(_q.Range))
            {
                _q.Cast();
            }
        }

        private static void OnBeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            var combo = Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
            var harass = Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass);
            var QC = getCheckBoxItem(comboMenu, "UseQC");
            var QH = getCheckBoxItem(harassMenu, "UseQH");
            var mode = getBoxItem(comboMenu, "ComboPrio") == 0
                       || getBoxItem(comboMenu, "ComboPrio") == 2;
            if (!(args.Target is AIHeroClient))
            {
                return;
            }

            if (_player.HasBuff("rengarpassivebuff") || _player.HasBuff("RengarR"))
            {
                return;
            }

            if (_player.Mana <= 4)
            {
                if (combo && QC && _q.IsReady() && Orbwalking.InAutoAttackRange(args.Target)
                    && args.Target.LSIsValidTarget(_q.Range))
                {
                    _q.Cast();
                }

                if (harass && QH && _q.IsReady() && Orbwalking.InAutoAttackRange(args.Target)
                    && args.Target.LSIsValidTarget(_q.Range))
                {
                    _q.Cast();
                }
            }

            if (_player.Mana == 5)
            {
                if (combo && QC && _q.IsReady() && Orbwalking.InAutoAttackRange(args.Target) && mode
                    && args.Target.LSIsValidTarget(_q.Range))
                {
                    _q.Cast();
                }


                if (harass && QH && _q.IsReady() && Orbwalking.InAutoAttackRange(args.Target) && mode
                    && args.Target.LSIsValidTarget(_q.Range))
                {
                    _q.Cast();
                }
            }
        }

        private static void ChangeComboMode()
        {
            var changetime = Environment.TickCount - _lastTick;


            if (getKeyBindItem(comboMenu, "Switch"))
            {
                if (getBoxItem(comboMenu, "ComboPrio") == 0 && _lastTick + 400 < Environment.TickCount)
                {
                    _lastTick = Environment.TickCount;
                    comboMenu["ComboPrio"].Cast<ComboBox>().CurrentValue = 1;
                }

                if (getBoxItem(comboMenu, "ComboPrio") == 1 && _lastTick + 400 < Environment.TickCount)
                {
                    _lastTick = Environment.TickCount;
                    comboMenu["ComboPrio"].Cast<ComboBox>().CurrentValue = 2;
                }
                if (getBoxItem(comboMenu, "ComboPrio") == 2 && _lastTick + 400 < Environment.TickCount)
                {
                    _lastTick = Environment.TickCount;
                    comboMenu["ComboPrio"].Cast<ComboBox>().CurrentValue = 0;
                }
            }

        }

        private static void Interrupter2_OnInterruptableTarget(
            AIHeroClient unit,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (_player.Mana < 5) return;
            if (_e.IsReady() && unit.LSIsValidTarget(_e.Range) && getCheckBoxItem(miscMenu, "UseEInt"))
            {
                var predE = _e.GetPrediction(unit);
                if (predE.Hitchance >= Echange() && predE.CollisionObjects.Count == 0) _e.Cast(unit);
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var spell = args.SData;
            if (!sender.IsMe)
            {
                return;
            }

            if (spell.Name.ToLower().Contains("rengarq") || spell.Name.ToLower().Contains("rengare"))
            {
                Orbwalker.ResetAutoAttack();
            }
        }

        private static void Smiteontarget()
        {
            if (_smite == null) return;
            var hero = HeroManager.Enemies.FirstOrDefault(x => x.LSIsValidTarget(570));
            var smiteDmg = _player.GetSummonerSpellDamage(hero, LeagueSharp.Common.Damage.SummonerSpell.Smite);
            var usesmite = getCheckBoxItem(comboMenu, "smitecombo");
            if (_player.GetSpell(_smiteSlot).Name.ToLower() == "s5_summonersmiteplayerganker" && usesmite
                && ObjectManager.Player.Spellbook.CanUseSpell(_smiteSlot) == SpellState.Ready)
            {
                if (hero != null && (!hero.HasBuffOfType(BuffType.Stun) || !hero.HasBuffOfType(BuffType.Slow)))
                {
                    ObjectManager.Player.Spellbook.CastSpell(_smiteSlot, hero);
                }
                else if (hero != null && smiteDmg >= hero.Health)
                {
                    ObjectManager.Player.Spellbook.CastSpell(_smiteSlot, hero);
                }
            }
            if (_player.GetSpell(_smiteSlot).Name.ToLower() == "s5_summonersmiteduel" && usesmite
                && ObjectManager.Player.Spellbook.CanUseSpell(_smiteSlot) == SpellState.Ready && hero.LSIsValidTarget(570))
            {
                ObjectManager.Player.Spellbook.CastSpell(_smiteSlot, hero);
            }
        }

        private static void Usepotion()
        {
            var mobs = MinionManager.GetMinions(
                _player.ServerPosition,
                _q.Range,
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);
            var iusehppotion = getCheckBoxItem(itemMenu, "usehppotions");
            var iusepotionhp = _player.Health
                               <= (_player.MaxHealth * (getSliderItem(itemMenu, "usepotionhp")) / 100);
            var iusemppotion = getCheckBoxItem(itemMenu, "usemppotions");
            var iusepotionmp = _player.Mana
                               <= (_player.MaxMana * (getSliderItem(itemMenu, "usepotionmp")) / 100);
            if (_player.InFountain() || ObjectManager.Player.HasBuff("Recall")) return;

            if (ObjectManager.Player.LSCountEnemiesInRange(800) > 0
                || (mobs.Count > 0 && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) && _smite != null))
            {
                if (iusepotionhp && iusehppotion
                    && !(ObjectManager.Player.HasBuff("RegenerationPotion")
                         || ObjectManager.Player.HasBuff("ItemMiniRegenPotion")
                         || ObjectManager.Player.HasBuff("ItemCrystalFlask")
                         || ObjectManager.Player.HasBuff("ItemCrystalFlaskJungle")
                         || ObjectManager.Player.HasBuff("ItemDarkCrystalFlask")))
                {
                    if (Items.HasItem(2010) && Items.CanUseItem(2010))
                    {
                        Items.UseItem(2010);
                    }

                    if (Items.HasItem(2003) && Items.CanUseItem(2003))
                    {
                        Items.UseItem(2003);
                    }

                    if (Items.HasItem(2031) && Items.CanUseItem(2031))
                    {
                        Items.UseItem(2031);
                    }

                    if (Items.HasItem(2032) && Items.CanUseItem(2032))
                    {
                        Items.UseItem(2032);
                    }

                    if (Items.HasItem(2033) && Items.CanUseItem(2033))
                    {
                        Items.UseItem(2033);
                    }
                }

                if (iusepotionmp && iusemppotion
                    && !(ObjectManager.Player.HasBuff("ItemDarkCrystalFlask")
                         || ObjectManager.Player.HasBuff("ItemMiniRegenPotion")
                         || ObjectManager.Player.HasBuff("ItemCrystalFlaskJungle")
                         || ObjectManager.Player.HasBuff("ItemCrystalFlask")))
                {
                    if (Items.HasItem(2041) && Items.CanUseItem(2041))
                    {
                        Items.UseItem(2041);
                    }

                    if (Items.HasItem(2010) && Items.CanUseItem(2010))
                    {
                        Items.UseItem(2010);
                    }

                    if (Items.HasItem(2032) && Items.CanUseItem(2032))
                    {
                        Items.UseItem(2032);
                    }

                    if (Items.HasItem(2033) && Items.CanUseItem(2033))
                    {
                        Items.UseItem(2033);
                    }
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            var harass = getKeyBindItem(harassMenu, "harasstoggle");
            var Rengar = Drawing.WorldToScreen(_player.Position);
            if (getCheckBoxItem(drawMenu, "combomode"))
            {
                if (getBoxItem(comboMenu, "ComboPrio") == 0) Drawing.DrawText(Rengar[0] - 45, Rengar[1] + 20, Color.PaleTurquoise, "Empower:Q");
                else if (getBoxItem(comboMenu, "ComboPrio") == 1) Drawing.DrawText(Rengar[0] - 45, Rengar[1] + 20, Color.PaleTurquoise, "Empower:W");
                else if (getBoxItem(comboMenu, "ComboPrio") == 2) Drawing.DrawText(Rengar[0] - 45, Rengar[1] + 20, Color.PaleTurquoise, "Empower:E");
            }
            if (getCheckBoxItem(drawMenu, "Drawharass"))
            {
                if (harass)
                {
                    Drawing.DrawText(
                        Drawing.Width * 0.02f,
                        Drawing.Height * 0.92f,
                        System.Drawing.Color.GreenYellow,
                        "Auto harass Enabled");
                }
                else
                    Drawing.DrawText(
                        Drawing.Width * 0.02f,
                        Drawing.Height * 0.92f,
                        System.Drawing.Color.OrangeRed,
                        "Auto harass Disabled");
            }
            if (getCheckBoxItem(drawMenu, "Drawsmite") && _smite != null)
            {

                if (getKeyBindItem(smiteMenu, "Usesmite"))
                {
                    Drawing.DrawText(
                        Drawing.Width * 0.02f,
                        Drawing.Height * 0.88f,
                        System.Drawing.Color.GreenYellow,
                        "Smite Jungle On");
                }
                else
                    Drawing.DrawText(
                        Drawing.Width * 0.02f,
                        Drawing.Height * 0.88f,
                        System.Drawing.Color.OrangeRed,
                        "Smite Jungle Off");

                if (_player.GetSpell(_smiteSlot).Name.ToLower() == "s5_summonersmiteplayerganker"
                    || _player.GetSpell(_smiteSlot).Name.ToLower() == "s5_summonersmiteduel")
                {
                    if (getCheckBoxItem(comboMenu, "smitecombo"))
                    {
                        Drawing.DrawText(
                            Drawing.Width * 0.02f,
                            Drawing.Height * 0.90f,
                            System.Drawing.Color.GreenYellow,
                            "Smite Target On");
                    }
                    else
                        Drawing.DrawText(
                            Drawing.Width * 0.02f,
                            Drawing.Height * 0.90f,
                            System.Drawing.Color.OrangeRed,
                            "Smite Target Off");
                }
            }

            if (getCheckBoxItem(drawMenu, "DrawQ") && _q.Level > 0)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _q.Range, System.Drawing.Color.GreenYellow);
            }
            if (getCheckBoxItem(drawMenu, "DrawW") && _w.Level > 0)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _w.Range, System.Drawing.Color.GreenYellow);
            }
            if (getCheckBoxItem(drawMenu, "DrawE") && _e.Level > 0)
            {
                Render.Circle.DrawCircle(
                    ObjectManager.Player.Position,
                    _e.Range,
                    _e.IsReady() ? System.Drawing.Color.GreenYellow : System.Drawing.Color.OrangeRed);
            }
        }

        private static float ComboDamage(Obj_AI_Base enemy)
        {
            var damage = 0d;
            if (_igniteSlot != SpellSlot.Unknown && _player.Spellbook.CanUseSpell(_igniteSlot) == SpellState.Ready) damage += ObjectManager.Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
            if (Items.HasItem(3077) && Items.CanUseItem(3077)) damage += _player.GetItemDamage(enemy, Damage.DamageItems.Tiamat);
            if (Items.HasItem(3074) && Items.CanUseItem(3074)) damage += _player.GetItemDamage(enemy, Damage.DamageItems.Hydra);
            if (Items.HasItem(3153) && Items.CanUseItem(3153)) damage += _player.GetItemDamage(enemy, Damage.DamageItems.Botrk);
            if (Items.HasItem(3144) && Items.CanUseItem(3144)) damage += _player.GetItemDamage(enemy, Damage.DamageItems.Bilgewater);
            if (_q.IsReady()) damage += _player.LSGetSpellDamage(enemy, SpellSlot.Q) * 2;
            if (_q.IsReady()) damage += _player.LSGetSpellDamage(enemy, SpellSlot.W);
            if (_e.IsReady()) damage += _player.LSGetSpellDamage(enemy, SpellSlot.E);

            damage += _player.GetAutoAttackDamage(enemy, true) * 3;
            return (float)damage;
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(2000, DamageType.Physical);
            var useQ = getCheckBoxItem(comboMenu, "UseQC");
            var useW = getCheckBoxItem(comboMenu, "UseWC");
            var useE = getCheckBoxItem(comboMenu, "UseEC");
            var useEE = getCheckBoxItem(comboMenu, "UseEEC");
            var iYoumuu = getCheckBoxItem(itemMenu, "Youmuu");
            var iTiamat = getCheckBoxItem(itemMenu, "Tiamat");
            var iHydra = getCheckBoxItem(itemMenu, "Hydra");
            var usesmite = getCheckBoxItem(comboMenu, "smitecombo");
            if (usesmite && target.LSIsValidTarget(570) && (_smite != null)
                && _player.Spellbook.CanUseSpell(_smiteSlot) == SpellState.Ready)
            {
                Smiteontarget();
            }

            if (target.LSIsValidTarget(600) && getCheckBoxItem(miscMenu, "UseIgnite")
                && _igniteSlot != SpellSlot.Unknown && _player.Spellbook.CanUseSpell(_igniteSlot) == SpellState.Ready)
            {
                if (ComboDamage(target) > target.Health)
                {
                    _player.Spellbook.CastSpell(_igniteSlot, target);
                }
            }

            if (iYoumuu && _youmuu.IsReady())
            {
                if (_player.HasBuff("RengarR"))
                {
                    Utility.DelayAction.Add(300, () => _youmuu.Cast());
                }
                else if (target.LSIsValidTarget(_e.Range))
                {
                    _youmuu.Cast();
                }
            }

            if (_player.Mana <= 4)
            {
                if (useQ)
                {
                    var tq = TargetSelector.GetTarget(_q.Range, DamageType.Physical);
                    if (tq.LSIsValidTarget(_q.Range) && _q.IsReady()) _q.Cast();
                }

                if (useW)
                {
                    var tw = TargetSelector.GetTarget(_w.Range, DamageType.Magical);
                    if (tw.LSIsValidTarget(_w.Range) && _w.IsReady() && !_player.HasBuff("rengarpassivebuff")) _w.Cast();
                }

                var th = TargetSelector.GetTarget(_w.Range, DamageType.Magical);

                if (iTiamat && _tiamat.IsReady() && th.LSIsValidTarget(_tiamat.Range))
                {
                    _tiamat.Cast();
                }

                if (iHydra && _hydra.IsReady() && th.LSIsValidTarget(_hydra.Range))
                {
                    _hydra.Cast();
                }


                if (useE)
                {
                    var te = TargetSelector.GetTarget(_e.Range, DamageType.Physical);
                    var predE = _e.GetPrediction(te);
                    if (!_player.HasBuff("rengarpassivebuff") && te.LSIsValidTarget(_e.Range) && _e.IsReady()
                        && predE.Hitchance >= Echange() && predE.CollisionObjects.Count == 0)
                        _e.Cast(te);
                }
            }

            if (_player.Mana == 5)
            {
                var tq = TargetSelector.GetTarget(_q.Range, DamageType.Physical);
                if (useQ
                    && (getBoxItem(comboMenu, "ComboPrio") == 0
                        || (getBoxItem(comboMenu, "ComboPrio") == 2
                            && Orbwalking.InAutoAttackRange(tq))))
                    if (tq.LSIsValidTarget(_q.Range) && _q.IsReady()) _q.Cast();

                if (useW && getBoxItem(comboMenu, "ComboPrio") == 1)
                {
                    var tw = TargetSelector.GetTarget(_w.Range, DamageType.Magical);
                    if (tw.LSIsValidTarget(_w.Range) && _w.IsReady() && !_player.HasBuff("rengarpassivebuff")) _w.Cast();
                }

                var th = TargetSelector.GetTarget(_w.Range, DamageType.Magical);

                if (iTiamat && _tiamat.IsReady() && th.LSIsValidTarget(_tiamat.Range))
                {
                    _tiamat.Cast();
                }

                if (iHydra && _hydra.IsReady() && th.IsValidTarget(_hydra.Range))
                {
                    _hydra.Cast();
                }

                if (useE && getBoxItem(comboMenu, "ComboPrio") == 2)
                {
                    var te = TargetSelector.GetTarget(_e.Range, DamageType.Physical);
                    var predE = _e.GetPrediction(te);
                    if (te.LSIsValidTarget(_e.Range) && _e.IsReady() && predE.Hitchance >= Echange()
                        && predE.CollisionObjects.Count == 0 && !_player.HasBuff("rengarpassivebuff"))
                        _e.Cast(te);
                }

                if (useEE && !_player.HasBuff("RengarR")
                    && (getBoxItem(comboMenu, "ComboPrio") == 2
                        || getBoxItem(comboMenu, "ComboPrio") == 0))
                {
                    var te = TargetSelector.GetTarget(_e.Range, DamageType.Physical);

                    if (_player.LSDistance(te) > _q.Range + 100f)
                    {
                        var predE = _e.GetPrediction(te);
                        if (te.LSIsValidTarget(_e.Range) && _e.IsReady() && predE.Hitchance >= Echange()
                            && predE.CollisionObjects.Count == 0)
                            _e.Cast(te);
                    }
                }
            }

            UseItemes();
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(_e.Range, DamageType.Physical);
            var useQ = getCheckBoxItem(harassMenu, "UseQH");
            var useW = getCheckBoxItem(harassMenu, "UseWH");
            var useE = getCheckBoxItem(harassMenu, "UseEH");
            var useItemsH = getCheckBoxItem(harassMenu, "UseItemsharass");
            if (_player.Mana <= 4)
            {
                if (useQ)
                {
                    var tq = TargetSelector.GetTarget(_q.Range, DamageType.Physical);
                    if (tq.LSIsValidTarget(_q.Range) && _q.IsReady()) _q.Cast();
                }

                if (useW)
                {
                    var tw = TargetSelector.GetTarget(_w.Range, DamageType.Magical);
                    if (tw.LSIsValidTarget(_w.Range) && _w.IsReady()) _w.Cast();
                }

                if (useE)
                {
                    var te = TargetSelector.GetTarget(_e.Range, DamageType.Physical);
                    var predE = _e.GetPrediction(te);
                    if (te.LSIsValidTarget(_e.Range) && _e.IsReady() && predE.Hitchance >= Echange()
                        && predE.CollisionObjects.Count == 0)
                        _e.Cast(te);
                }
            }

            if (_player.Mana == 5)
            {
                if (useQ && getBoxItem(harassMenu, "HarrPrio") == 0)
                {
                    var tq = TargetSelector.GetTarget(_q.Range, DamageType.Physical);
                    if (tq.LSIsValidTarget(_q.Range) && _q.IsReady()) _q.Cast();
                }

                if (useW && getBoxItem(harassMenu, "HarrPrio") == 1)
                {
                    var tw = TargetSelector.GetTarget(_w.Range, DamageType.Magical);
                    if (tw.LSIsValidTarget(_w.Range) && _w.IsReady() && !_player.HasBuff("rengarpassivebuff")) _w.Cast();
                }

                if (useE && getBoxItem(harassMenu, "HarrPrio") == 2)
                {
                    var te = TargetSelector.GetTarget(_e.Range, DamageType.Physical);
                    var predE = _e.GetPrediction(te);
                    if (te.LSIsValidTarget(_e.Range) && _e.IsReady() && predE.Hitchance >= Echange()
                        && predE.CollisionObjects.Count == 0)
                        _e.Cast(te);
                }
            }

            if (useItemsH && _tiamat.IsReady() && target.LSIsValidTarget(_tiamat.Range))
            {
                _tiamat.Cast();
            }

            if (useItemsH && _hydra.IsReady() && target.LSIsValidTarget(_hydra.Range))
            {
                _hydra.Cast();
            }
        }

        private static HitChance Echange()
        {
            switch (getBoxItem(miscMenu, "Echange"))
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
                case 3:
                    return HitChance.VeryHigh;
                default:
                    return HitChance.High;
            }
        }

        private static void KillSteal()
        {
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy))
            {
                var igniteDmg = _player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
                var qDmg = _player.LSGetSpellDamage(hero, SpellSlot.Q);
                var wDmg = _player.LSGetSpellDamage(hero, SpellSlot.W);
                var eDmg = _player.LSGetSpellDamage(hero, SpellSlot.E);

                if (hero.LSIsValidTarget(600) && getCheckBoxItem(miscMenu, "UseIgnite")
                    && _igniteSlot != SpellSlot.Unknown
                    && _player.Spellbook.CanUseSpell(_igniteSlot) == SpellState.Ready)
                {
                    if (igniteDmg > hero.Health)
                    {
                        _player.Spellbook.CastSpell(_igniteSlot, hero);
                    }
                }

                if (_q.IsReady() && getCheckBoxItem(miscMenu, "UseQM") && _player.LSDistance(hero) <= _q.Range)
                {
                    var t = TargetSelector.GetTarget(_q.Range, DamageType.Physical);
                    if (t != null) if (!t.HasBuff("JudicatorIntervention") && !t.HasBuff("Undying Rage") && qDmg > t.Health) _q.Cast(t);
                }

                if (_w.IsReady() && getCheckBoxItem(miscMenu, "UseWM") && _player.LSDistance(hero) <= _w.Range)
                {
                    var t = TargetSelector.GetTarget(_w.Range, DamageType.Physical);
                    if (t != null) if (!t.HasBuff("JudicatorIntervention") && !t.HasBuff("Undying Rage") && wDmg > t.Health) _w.Cast(t);
                }

                if (_q.IsReady() && getCheckBoxItem(miscMenu, "UseRM") && _player.LSDistance(hero) <= _e.Range)
                {
                    var t = TargetSelector.GetTarget(_q.Range, DamageType.Physical);
                    if (t != null)
                        if (!t.HasBuff("JudicatorIntervention") && !t.HasBuff("Undying Rage") && eDmg > t.Health
                            && _e.GetPrediction(t).Hitchance >= HitChance.High)
                            _e.Cast(t);
                }
            }
        }

        public static readonly string[] Smitetype =
            {
                "s5_summonersmiteplayerganker", "s5_summonersmiteduel",
                "s5_summonersmitequick", "itemsmiteaoe", "summonersmite"
            };


        private static int GetSmiteDmg()
        {
            int level = _player.Level;
            int index = _player.Level / 5;
            float[] dmgs = { 370 + 20 * level, 330 + 30 * level, 240 + 40 * level, 100 + 50 * level };
            return (int)dmgs[index];
        }

        //New map Monsters Name By SKO
        private static void Smiteuse()
        {
            var jungle = Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear);
            if (ObjectManager.Player.Spellbook.CanUseSpell(_smiteSlot) != SpellState.Ready) return;
            var usered = getCheckBoxItem(smiteMenu, "Usered");
            var health = (100 * (_player.Health / _player.MaxHealth)) < getSliderItem(smiteMenu, "healthJ");
            string[] jungleMinions;
            if (Utility.Map.GetMap().Type == Utility.Map.MapType.TwistedTreeline)
            {
                jungleMinions = new string[] { "TT_Spiderboss", "TT_NWraith", "TT_NGolem", "TT_NWolf" };
            }
            else
            {
                jungleMinions = new string[]
                                    {
                                        "SRU_Blue", "SRU_Gromp", "SRU_Murkwolf", "SRU_Razorbeak", "SRU_RiftHerald",
                                        "SRU_Red", "SRU_Krug", "SRU_Dragon_Air", "SRU_Dragon_Water", "SRU_Dragon_Fire",
                                        "SRU_Dragon_Elder", "SRU_Baron"
                                    };
            }

            var minions = MinionManager.GetMinions(_player.Position, 1000, MinionTypes.All, MinionTeam.Neutral);
            if (minions.Count() > 0)
            {
                int smiteDmg = GetSmiteDmg();

                foreach (Obj_AI_Base minion in minions)
                {
                    if (Utility.Map.GetMap().Type == Utility.Map.MapType.TwistedTreeline && minion.Health <= smiteDmg
                        && jungleMinions.Any(name => minion.Name.Substring(0, minion.Name.Length - 5).Equals(name)))
                    {
                        ObjectManager.Player.Spellbook.CastSpell(_smiteSlot, minion);
                    }
                    if (minion.Health <= smiteDmg && jungleMinions.Any(name => minion.Name.StartsWith(name))
                        && !jungleMinions.Any(name => minion.Name.Contains("Mini")))
                    {
                        ObjectManager.Player.Spellbook.CastSpell(_smiteSlot, minion);
                    }
                    else if (jungle && usered && health && minion.Health >= smiteDmg
                             && jungleMinions.Any(name => minion.Name.StartsWith("SRU_Red"))
                             && !jungleMinions.Any(name => minion.Name.Contains("Mini")))
                    {
                        ObjectManager.Player.Spellbook.CastSpell(_smiteSlot, minion);
                    }
                }
            }
        }

        private static void Laneclear()
        {
            var minions = MinionManager.GetMinions(_player.ServerPosition, _e.Range).FirstOrDefault();
            var useItemsl = getCheckBoxItem(clearMenu, "UseItemslane");
            var useQl = getCheckBoxItem(clearMenu, "UseQL");
            var useWl = getCheckBoxItem(clearMenu, "UseWL");
            var useEl = getCheckBoxItem(clearMenu, "UseEL");
            var save = getCheckBoxItem(clearMenu, "LaneSave");
            if (minions == null) return;
            if (_player.Mana <= 4)
            {
                if (_q.IsReady() && useQl && minions.LSIsValidTarget(_q.Range))
                {
                    _q.Cast();
                }

                if (_w.IsReady() && useWl && minions.LSIsValidTarget(_w.Range))
                {
                    _w.Cast();
                }

                if (_e.IsReady() && useEl && minions.LSIsValidTarget(_e.Range))
                {
                    _e.Cast(minions);
                }
            }

            if (_player.Mana == 5)
            {
                if (save) return;
                if (_q.IsReady() && getBoxItem(clearMenu, "LanePrio") == 0 && useQl
                    && minions.LSIsValidTarget(_q.Range))
                {
                    _q.Cast();
                }

                if (_w.IsReady() && getBoxItem(clearMenu, "LanePrio") == 1 && useWl
                    && minions.LSIsValidTarget(_w.Range))
                {
                    _w.Cast();
                }

                if (_e.IsReady() && getBoxItem(clearMenu, "LanePrio") == 2 && useEl
                    && minions.IsValidTarget(_e.Range))
                {
                    _e.Cast(minions);
                }
            }

            if (useItemsl && _tiamat.IsReady() && minions.LSIsValidTarget(_tiamat.Range))
            {
                _tiamat.Cast();
            }

            if (useItemsl && _hydra.IsReady() && minions.LSIsValidTarget(_hydra.Range))
            {
                _hydra.Cast();
            }
        }

        private static void JungleClear()
        {
            var mob =
                MinionManager.GetMinions(
                    _player.ServerPosition,
                    _e.Range,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth).FirstOrDefault();
            var useItemsJ = getCheckBoxItem(jungleMenu, "UseItemsjungle");
            var useQ = getCheckBoxItem(jungleMenu, "UseQJ");
            var useW = getCheckBoxItem(jungleMenu, "UseWJ");
            var useE = getCheckBoxItem(jungleMenu, "UseEJ");
            var save = getCheckBoxItem(jungleMenu, "JungleSave");
            if (mob == null)
            {
                return;
            }

            if (_player.Mana <= 4)
            {
                if (useQ && _q.IsReady() && mob.LSIsValidTarget(_q.Range))
                {
                    _q.Cast();
                }

                if (_w.IsReady() && useW && mob.LSIsValidTarget(_w.Range - 100) && !_player.HasBuff("rengarpassivebuff"))
                {
                    _w.Cast();
                }

                if (useItemsJ && _tiamat.IsReady() && mob.LSIsValidTarget(_tiamat.Range))
                {
                    _tiamat.Cast();
                }

                if (useItemsJ && _hydra.IsReady() && mob.IsValidTarget(_hydra.Range))
                {
                    _hydra.Cast();
                }

                if (_e.IsReady() && useE && mob.IsValidTarget(_e.Range))
                {
                    _e.Cast(mob);
                }
            }

            if (_player.Mana != 5 || save)
            {
                return;
            }

            if (mob.LSIsValidTarget(_q.Range) && _q.IsReady()
                && getBoxItem(jungleMenu, "JunglePrio") == 0 && useQ)
            {
                _q.Cast();
            }

            if (mob.LSIsValidTarget(_w.Range) && _w.IsReady()
                && getBoxItem(jungleMenu, "JunglePrio") == 1 && useW
                && !_player.HasBuff("rengarpassivebuff"))
            {
                _w.Cast();
            }

            if (useItemsJ && _tiamat.IsReady() && mob.LSIsValidTarget(_tiamat.Range))
            {
                _tiamat.Cast();
            }

            if (useItemsJ && _hydra.IsReady() && mob.LSIsValidTarget(_hydra.Range))
            {
                _hydra.Cast();
            }

            if (mob.LSIsValidTarget(_e.Range) && _e.IsReady()
                && getBoxItem(jungleMenu, "JunglePrio") == 2 && useE)
            {
                _e.Cast(mob.ServerPosition);
            }
        }

        private static void LastHit()
        {
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _e.Range, MinionTypes.All);
            var useQ = getCheckBoxItem(lasthitMenu, "UseQLH");
            var useW = getCheckBoxItem(lasthitMenu, "UseWLH");
            var useE = getCheckBoxItem(lasthitMenu, "UseELH");
            var save = getCheckBoxItem(lasthitMenu, "LastSave");
            foreach (var minion in allMinions)
            {
                if (_player.Mana <= 4)
                {
                    if (useQ && _q.IsReady() && _player.LSDistance(minion) < _q.Range
                        && minion.Health < 0.75 * _player.LSGetSpellDamage(minion, SpellSlot.Q))
                    {
                        _q.Cast();
                    }

                    if (_w.IsReady() && useW && _player.LSDistance(minion) < _w.Range
                        && minion.Health < 0.75 * _player.LSGetSpellDamage(minion, SpellSlot.W))
                    {
                        _w.Cast();
                    }

                    if (_e.IsReady() && useE && _player.LSDistance(minion) < _e.Range
                        && minion.Health < 0.75 * _player.LSGetSpellDamage(minion, SpellSlot.E))
                    {
                        _e.Cast(minion);
                    }
                }

                if (_player.Mana != 5 || save)
                {
                    return;
                }

                if (useQ && _q.IsReady() && _player.LSDistance(minion) < _q.Range
                    && minion.Health < 0.75 * _player.LSGetSpellDamage(minion, SpellSlot.Q)
                    && getBoxItem(lasthitMenu, "LastPrio") == 0)
                {
                    _q.Cast();
                }

                if (_w.IsReady() && useW && _player.LSDistance(minion) < _w.Range
                    && minion.Health < 0.75 * _player.LSGetSpellDamage(minion, SpellSlot.W)
                    && getBoxItem(lasthitMenu, "LastPrio") == 1)
                {
                    _w.Cast();
                }

                if (_e.IsReady() && useE && _player.LSDistance(minion) < _e.Range
                    && minion.Health < 0.75 * _player.LSGetSpellDamage(minion, SpellSlot.E)
                    && getBoxItem(lasthitMenu, "LastPrio") == 2)
                {
                    _e.Cast(minion);
                }
            }
        }

        private static void AutoHeal()
        {
            var health = (100 * (_player.Health / _player.MaxHealth)) < getSliderItem(miscMenu, "AutoWHP");

            if (_player.HasBuff("Recall") || _player.Mana <= 4) return;


            if (_w.IsReady() && health)
            {
                _w.Cast();
            }
        }

        private static void UseItemes()
        {
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy))
            {
                var iBilge = getCheckBoxItem(itemMenu, "Bilge");
                var iBilgeEnemyhp = hero.Health
                                    <= (hero.MaxHealth * (getSliderItem(itemMenu, "BilgeEnemyhp")) / 100);
                var iBilgemyhp = _player.Health
                                 <= (_player.MaxHealth * (getSliderItem(itemMenu, "Bilgemyhp")) / 100);
                var iBlade = getCheckBoxItem(itemMenu, "Blade");
                var iBladeEnemyhp = hero.Health
                                    <= (hero.MaxHealth * (getSliderItem(itemMenu, "BladeEnemyhp")) / 100);
                var iBlademyhp = _player.Health
                                 <= (_player.MaxHealth * (getSliderItem(itemMenu, "Blademyhp")) / 100);
                var iYoumuu = getCheckBoxItem(itemMenu, "Youmuu");
                // var iTiamat = _config.Item("Tiamat").GetValue<bool>();
                // var iHydra = _config.Item("Hydra").GetValue<bool>();
                if (hero.LSIsValidTarget(450) && iBilge && (iBilgeEnemyhp || iBilgemyhp) && _bilge.IsReady())
                {
                    _bilge.Cast(hero);
                }

                if (hero.LSIsValidTarget(450) && iBlade && (iBladeEnemyhp || iBlademyhp) && _blade.IsReady())
                {
                    _blade.Cast(hero);
                }

                /* if (iTiamat && _tiamat.IsReady() && hero.IsValidTarget(_tiamat.Range))
                {
                    _tiamat.Cast();

                }
                if (iHydra && _hydra.IsReady() && hero.IsValidTarget(_hydra.Range))
                {
                    _hydra.Cast();

                }*/

            }

            }
        }
    }
