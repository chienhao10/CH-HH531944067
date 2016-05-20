using System;
using System.Linq;
using EloBuddy;
using LeagueSharp.Common;
using EloBuddy.SDK;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using Color = System.Drawing.Color;

namespace D_Elise
{
    class Program
    {
        private const string ChampionName = "Elise";

        private static bool _human;

        private static bool _spider;

        private static Spell _humanQ, _humanW, _humanE, _r, _spiderQ, _spiderW, _spiderE;

        private static Menu Menu { get; set; }

        public static Menu comboMenu, harassMenu, itemMenu, clearMenu, miscMenu, jungleMenu, drawMenu, ksMenu, smiteMenu;

        private static SpellSlot _igniteSlot;

        private static AIHeroClient _player;

        private static readonly float[] HumanQcd = { 6, 6, 6, 6, 6 };

        private static readonly float[] HumanWcd = { 12, 12, 12, 12, 12 };

        private static readonly float[] HumanEcd = { 14, 13, 12, 11, 10 };

        private static readonly float[] SpiderQcd = { 6, 6, 6, 6, 6 };

        private static readonly float[] SpiderWcd = { 12, 12, 12, 12, 12 };

        private static readonly float[] SpiderEcd = { 26, 23, 20, 17, 14 };

        private static float _humQcd = 0, _humWcd = 0, _humEcd = 0;

        private static float _spidQcd = 0, _spidWcd = 0, _spidEcd = 0;

        private static float _humaQcd = 0, _humaWcd = 0, _humaEcd = 0;

        private static float _spideQcd = 0, _spideWcd = 0, _spideEcd = 0;

        private static Items.Item _tiamat, _hydra, _blade, _bilge, _rand, _lotis, _zhonya;

        private static SpellSlot _smiteSlot;

        private static Spell _smite;

        public static void Game_OnGameLoad()
        {

            _player = ObjectManager.Player;
            if (_player.ChampionName != ChampionName) return;

            _humanQ = new Spell(SpellSlot.Q, 625f);
            _humanW = new Spell(SpellSlot.W, 950f);
            _humanE = new Spell(SpellSlot.E, 1075f);
            _spiderQ = new Spell(SpellSlot.Q, 475f);
            _spiderW = new Spell(SpellSlot.W, 0);
            _spiderE = new Spell(SpellSlot.E, 750f);
            _r = new Spell(SpellSlot.R, 0);

            _humanW.SetSkillshot(0.75f, 100f, 5000, true, SkillshotType.SkillshotLine);
            _humanE.SetSkillshot(0.5f, 55f, 1450, true, SkillshotType.SkillshotLine);

            _bilge = new Items.Item(3144, 475f);
            _blade = new Items.Item(3153, 425f);
            _hydra = new Items.Item(3074, 250f);
            _tiamat = new Items.Item(3077, 250f);
            _rand = new Items.Item(3143, 490f);
            _lotis = new Items.Item(3190, 590f);
            _zhonya = new Items.Item(3157, 10);

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

            _igniteSlot = _player.GetSpellSlot("SummonerDot");

            Menu = MainMenu.AddMenu("D-Elise", "D-Elise");

            comboMenu = Menu.AddSubMenu("Combo", "Combo");
            comboMenu.Add("UseHumanQ", new CheckBox("Human Q"));
            comboMenu.Add("UseHumanW", new CheckBox("Human W"));
            comboMenu.Add("UseHumanE", new CheckBox("Human E"));
            comboMenu.Add("UseRCombo", new CheckBox("Auto use R"));
            comboMenu.Add("UseSpiderQ", new CheckBox("Spider Q"));
            comboMenu.Add("UseSpiderW", new CheckBox("Spider W"));
            comboMenu.Add("UseSpiderE", new CheckBox("Spider E"));


            harassMenu = Menu.AddSubMenu("Harass", "Harass");
            harassMenu.Add("UseQHarass", new CheckBox("Human Q"));
            harassMenu.Add("UseWHarass", new CheckBox("Human W"));
            harassMenu.Add("Harrasmana", new Slider("Minimum Mana", 60, 1, 100));



            itemMenu = Menu.AddSubMenu("Items", "items");
            itemMenu.Add("Youmuu", new CheckBox("Use Youmuu's"));
            itemMenu.AddSeparator();
            itemMenu.Add("Bilge", new CheckBox("Use Bilge"));
            itemMenu.Add("BilgeEnemyhp", new Slider("If Enemy Hp <", 85, 1, 100));
            itemMenu.Add("Bilgemyhp", new Slider("Or Your Hp <", 85, 1, 100));
            itemMenu.Add("Blade", new CheckBox("Use Bork"));
            itemMenu.Add("BladeEnemyhp", new Slider("If Enemy Hp <", 85, 1, 100));
            itemMenu.Add("Blademyhp", new Slider("Or Your Hp <", 85, 1, 100));
            itemMenu.Add("Hextech", new CheckBox("Hextech Gunblade"));
            itemMenu.Add("HextechEnemyhp", new Slider("If Enemy Hp <", 85, 1, 100));
            itemMenu.Add("Hextechmyhp", new Slider("Or Your Hp <", 85, 1, 100));
            itemMenu.AddLabel("Deffensive Items");
            itemMenu.AddSeparator();
            itemMenu.Add("Omen", new CheckBox("Use Randuin Omen"));
            itemMenu.Add("Omenenemys", new Slider("Randuin if enemys>", 2, 1, 5));
            itemMenu.Add("Zhonyas", new CheckBox("Use Zhonya's"));
            itemMenu.Add("Zhonyashp", new Slider("Use Zhonya's if HP%<", 20, 1, 100));
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


            clearMenu = Menu.AddSubMenu("Farm", "Farm");
            clearMenu.Add("HumanQFarm", new CheckBox("Human Q"));
            clearMenu.Add("HumanWFarm", new CheckBox("Human W"));
            clearMenu.Add("SpiderQFarm", new CheckBox("Spider Q", false));
            clearMenu.Add("SpiderWFarm", new CheckBox("Spider W"));
            clearMenu.Add("Farm_R", new KeyBind("Auto Switch (toggle)", false, KeyBind.BindTypes.PressToggle, 'L'));
            clearMenu.Add("Lanemana", new Slider("Minimum Mana", 60, 1, 100));

            jungleMenu = Menu.AddSubMenu("Jungle", "Jungle");
            jungleMenu.Add("HumanQFarmJ", new CheckBox("Human Q"));
            jungleMenu.Add("HumanWFarmJ", new CheckBox("Human W"));
            jungleMenu.Add("SpiderQFarmJ", new CheckBox("Spider Q"));
            jungleMenu.Add("SpiderWFarmJ", new CheckBox("Spider W"));
            jungleMenu.Add("Junglemana", new Slider("Minimum Mana", 60, 1, 100));

            smiteMenu = Menu.AddSubMenu("Smite", "Smite");
            smiteMenu.Add("Usesmite", new KeyBind("Use Smite(toggle)", false, KeyBind.BindTypes.PressToggle, 'H'));
            smiteMenu.Add("Useblue", new CheckBox("Smite Blue Early"));
            smiteMenu.Add("manaJ", new Slider("Smite Blue Early if MP% <", 35, 1, 100));
            smiteMenu.Add("Usered", new CheckBox("Smite Red Early"));
            smiteMenu.Add("healthJ", new Slider("Smite Red Early if HP% <", 35, 1, 100));
            smiteMenu.Add("smitecombo", new CheckBox("Use Smite in target"));
            smiteMenu.Add("Smiteeee", new CheckBox("Smite Minion in HumanE path", false));


            miscMenu = Menu.AddSubMenu("Misc", "Misc");
            miscMenu.Add("Spidergapcloser", new CheckBox("SpiderE to GapCloser"));       
            miscMenu.Add("Humangapcloser", new CheckBox("HumanE to GapCloser"));
            miscMenu.Add("UseEInt", new CheckBox("HumanE to Interrupt"));
            miscMenu.Add("autoE", new KeyBind("HUmanE with VeryHigh Chance", false, KeyBind.BindTypes.HoldActive, 'T'));
            miscMenu.Add("Echange", new ComboBox("E Hit Combo", 3, "Low", "Medium", "High", "Very High"));

            ksMenu = Menu.AddSubMenu("KillSteal", "Ks");
            ksMenu.Add("ActiveKs", new CheckBox("Use KillSteal"));
            ksMenu.Add("HumanQKs", new CheckBox("Human Q"));
            ksMenu.Add("HumanWKs", new CheckBox("Human W"));
            ksMenu.Add("SpiderQKs", new CheckBox("Spider Q"));
            ksMenu.Add("UseIgnite", new CheckBox("Use Ignite"));


            drawMenu = Menu.AddSubMenu("Drawings", "Drawings");
            drawMenu.Add("DrawQ", new CheckBox("Human Q", false));
            drawMenu.Add("DrawW", new CheckBox("Human W", false));
            drawMenu.Add("DrawE", new CheckBox("Human E", false));
            drawMenu.Add("SpiderDrawQ", new CheckBox("Spider Q", false));
            drawMenu.Add("SpiderDrawE", new CheckBox("Spider E", false));
            drawMenu.Add("Drawsmite", new CheckBox("Draw Smite", true));
            drawMenu.Add("drawmode", new CheckBox("Draw Smite Mode", true));
            drawMenu.Add("DrawCooldown", new CheckBox("Draw Cooldown", false));
            drawMenu.Add("Drawharass", new CheckBox("Draw AutoHarass", true));


            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Chat.Print("<font color='#881df2'>D-Elise by Diabaths</font> Loaded.");
            Chat.Print(
                "<font color='#f2f21d'>If You like my work and want to support me,  plz donate via paypal in </font> <font color='#00e6ff'>ssssssssssmith@hotmail.com</font> (10) S");

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
            Cooldowns();

            _player = ObjectManager.Player;

            Orbwalker.DisableAttacking = false;

            CheckSpells();
            if (getKeyBindItem(smiteMenu, "Usesmite"))
            {
                Smiteuse();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit)
                || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                FarmLane();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                JungleFarm();
            }
            Usepotion();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();

            }
            if (getCheckBoxItem(ksMenu, "ActiveKs"))
            {
                KillSteal();
            }
            if (getKeyBindItem(miscMenu, "autoE"))
            {
                AutoE();
            }
        }

        private static void Smiteontarget()
        {
            if (_smite == null) return;
            var hero = HeroManager.Enemies.FirstOrDefault(x => x.IsValidTarget(570));
            var smiteDmg = _player.GetSummonerSpellDamage(hero, LeagueSharp.Common.Damage.SummonerSpell.Smite);
            var usesmite = getCheckBoxItem(smiteMenu, "smitecombo");
            if (_player.GetSpell(_smiteSlot).Name.ToLower() == "s5_summonersmiteplayerganker" && usesmite
                && ObjectManager.Player.Spellbook.CanUseSpell(_smiteSlot) == SpellState.Ready)
            {
                if (!hero.HasBuffOfType(BuffType.Stun) || !hero.HasBuffOfType(BuffType.Slow))
                {
                    ObjectManager.Player.Spellbook.CastSpell(_smiteSlot, hero);
                }
                else if (smiteDmg >= hero.Health)
                {
                    ObjectManager.Player.Spellbook.CastSpell(_smiteSlot, hero);
                }
            }
            if (_player.GetSpell(_smiteSlot).Name.ToLower() == "s5_summonersmiteduel" && usesmite
                && ObjectManager.Player.Spellbook.CanUseSpell(_smiteSlot) == SpellState.Ready && hero.IsValidTarget(570))
            {
                ObjectManager.Player.Spellbook.CastSpell(_smiteSlot, hero);
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!getCheckBoxItem(drawMenu, "DrawCooldown")) return;
            if (sender.IsMe)
                //Game.PrintChat("Spell name: " + args.SData.Name.ToString());
                GetCDs(args);
        }

        private static void Usepotion()
        {
            var mobs = MinionManager.GetMinions(
                _player.ServerPosition,
                600,
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

            if (ObjectManager.Player.CountEnemiesInRange(800) > 0
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
                var iHextech = getCheckBoxItem(itemMenu, "Hextech");
                var iHextechEnemyhp = hero.Health <=
                                      (hero.MaxHealth * (getSliderItem(itemMenu, "HextechEnemyhp")) / 100);
                var iHextechmyhp = _player.Health <=
                                   (_player.MaxHealth * (getSliderItem(itemMenu, "Hextechmyhp")) / 100);
                var iOmen = getCheckBoxItem(itemMenu, "Omen");
                var iOmenenemys = hero.CountEnemiesInRange(450) >= getSliderItem(itemMenu, "Omenenemys");
                var iZhonyas = getCheckBoxItem(itemMenu, "Zhonyas");
                var iZhonyashp = _player.Health
                                 <= (_player.MaxHealth * (getSliderItem(itemMenu, "Zhonyashp")) / 100);
                if (hero.IsValidTarget(450) && iBilge && (iBilgeEnemyhp || iBilgemyhp) && _bilge.IsReady())
                {
                    _bilge.Cast(hero);

                }
                if (hero.IsValidTarget(450) && iBlade && (iBladeEnemyhp || iBlademyhp) && _blade.IsReady())
                {
                    _blade.Cast(hero);

                }
                if (iOmenenemys && iOmen && _rand.IsReady() && hero.IsValidTarget(450))
                {
                    _rand.Cast();
                }
                if (iZhonyas && iZhonyashp && ObjectManager.Player.CountEnemiesInRange(1000) >= 1)
                {
                    _zhonya.Cast(_player);

                }
            }
        }

        private static void Combo()
        {
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy))
            {
                var target = TargetSelector.GetTarget(_humanW.Range, DamageType.Magical);
                var sReady = (_smiteSlot != SpellSlot.Unknown
                              && ObjectManager.Player.Spellbook.CanUseSpell(_smiteSlot) == SpellState.Ready);
                var qdmg = _player.GetSpellDamage(hero, SpellSlot.Q);
                var wdmg = _player.GetSpellDamage(hero, SpellSlot.W);
                //if (target == null) return; //buffelisecocoon
                Smiteontarget();
                if (_human)
                {
                    if (target.IsValidTarget(_humanE.Range) && getCheckBoxItem(comboMenu, "UseHumanE")
                        && _humanE.IsReady())
                    {
                        if (sReady && getCheckBoxItem(smiteMenu, "Smiteeee")
                            && _humanE.GetPrediction(target).CollisionObjects.Count == 1)
                        {
                            CheckingCollision(target);
                            _humanE.Cast(hero);
                        }
                        else if (_humanE.GetPrediction(target).Hitchance >= Echange())
                        {
                            _humanE.Cast(target);
                        }
                    }

                    if (target.IsValidTarget(_humanQ.Range) && getCheckBoxItem(comboMenu, "UseHumanQ")
                        && _humanQ.IsReady())
                    {
                        _humanQ.Cast(target);
                    }
                    if (target.IsValidTarget(_humanW.Range) && getCheckBoxItem(comboMenu, "UseHumanW")
                        && _humanW.IsReady())
                    {
                        _humanW.Cast(target);
                    }
                    if (!_humanQ.IsReady() && !_humanW.IsReady() && !_humanE.IsReady()
                        && getCheckBoxItem(comboMenu, "UseRCombo") && _r.IsReady())
                    {
                        _r.Cast();
                    }
                    if (!_humanQ.IsReady() && !_humanW.IsReady() && hero.IsValidTarget(_spiderQ.Range)
                        && getCheckBoxItem(comboMenu, "UseRCombo") && _r.IsReady())
                    {
                        _r.Cast();
                    }
                }
                if (!_spider) return;
                if (hero.IsValidTarget(_spiderQ.Range) && getCheckBoxItem(comboMenu, "UseSpiderQ")
                    && _spiderQ.IsReady())
                {
                    _spiderQ.Cast(hero);
                }
                if (hero.IsValidTarget(200) && getCheckBoxItem(comboMenu, "UseSpiderW") && _spiderW.IsReady())
                {
                    _spiderW.Cast();
                }
                if (hero.IsValidTarget(_spiderE.Range) && _player.Distance(target) > _spiderQ.Range
                    && getCheckBoxItem(comboMenu, "UseSpiderE") && _spiderE.IsReady() && !_spiderQ.IsReady())
                {
                    _spiderE.Cast(hero);
                }
                if (!hero.IsValidTarget(_spiderQ.Range) && !_spiderE.IsReady() && _r.IsReady() && !_spiderQ.IsReady()
                    && getCheckBoxItem(comboMenu, "UseRCombo"))
                {
                    _r.Cast();
                }
                if (_humanQ.IsReady() && _humanW.IsReady() && _r.IsReady() && getCheckBoxItem(comboMenu, "UseRCombo"))
                {
                    _r.Cast();
                }
                if (_humanQ.IsReady() && _humanW.IsReady() && _r.IsReady() && getCheckBoxItem(comboMenu, "UseRCombo"))
                {
                    _r.Cast();
                }
                if ((_humanQ.IsReady() && qdmg >= hero.Health || _humanW.IsReady() && wdmg >= hero.Health)
                    && getCheckBoxItem(comboMenu, "UseRCombo"))
                {
                    _r.Cast();
                }

                UseItemes();
            }
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(_humanQ.Range, DamageType.Magical);

            if (_human && target.IsValidTarget(_humanQ.Range) && getCheckBoxItem(harassMenu, "UseQHarass")
                && _humanQ.IsReady())
            {
                _humanQ.Cast(target);
            }

            if (_human && target.IsValidTarget(_humanW.Range) && getCheckBoxItem(harassMenu, "UseWHarass")
                && _humanW.IsReady())
            {
                _humanW.Cast(target, false, true);
            }
        }

        private static void JungleFarm()
        {
            var jungleQ = (getCheckBoxItem(jungleMenu, "HumanQFarmJ")
                           && (100 * (_player.Mana / _player.MaxMana))
                           > getSliderItem(jungleMenu, "Junglemana"));
            var jungleW = (getCheckBoxItem(jungleMenu, "HumanQFarmJ")
                           && (100 * (_player.Mana / _player.MaxMana))
                           > getSliderItem(jungleMenu, "Junglemana"));
            var spiderjungleQ = getCheckBoxItem(jungleMenu, "SpiderQFarmJ");
            var spiderjungleW = getCheckBoxItem(jungleMenu, "SpiderWFarmJ");
            var switchR = (100 * (_player.Mana / _player.MaxMana)) < getSliderItem(jungleMenu, "Junglemana");
            var mobs = MinionManager.GetMinions(
                _player.ServerPosition,
                _humanQ.Range,
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);
            if (mobs.Count > 0)
            {
                foreach (var minion in mobs)
                    if (_human)
                    {
                        if (jungleQ && _humanQ.IsReady() && minion.IsValidTarget()
                            && _player.Distance(minion) <= _humanQ.Range)
                        {
                            _humanQ.Cast(minion);
                        }
                        if (jungleW && _humanW.IsReady() && !_humanQ.IsReady() && minion.IsValidTarget()
                            && _player.Distance(minion) <= _humanW.Range)
                        {
                            _humanW.Cast(minion);
                        }
                        if ((!_humanQ.IsReady() && !_humanW.IsReady()) || switchR)
                        {
                            _r.Cast();
                        }
                    }
                foreach (var minion in mobs)
                {
                    if (_spider)
                    {
                        if (spiderjungleQ && _spiderQ.IsReady() && minion.IsValidTarget()
                            && _player.Distance(minion) <= _spiderQ.Range)
                        {
                            _spiderQ.Cast(minion);
                        }
                        if (spiderjungleW && _spiderW.IsReady() && minion.IsValidTarget()
                            && _player.Distance(minion) <= 150)
                        {
                            Orbwalker.DisableAttacking = false;
                            _spiderW.Cast();
                        }
                        if (_r.IsReady() && _humanQ.IsReady() && _spider && !switchR)
                        {
                            _r.Cast();
                        }
                    }
                }
            }
        }

        private static void FarmLane()
        {
            var ManaUse = (100 * (_player.Mana / _player.MaxMana)) > getSliderItem(clearMenu, "Lanemana");
            var useR = getKeyBindItem(clearMenu, "Farm_R");
            var useHumQ = (getCheckBoxItem(clearMenu, "HumanQFarm")
                           && (100 * (_player.Mana / _player.MaxMana))
                           > getSliderItem(clearMenu, "Lanemana"));
            var useHumW = (getCheckBoxItem(clearMenu, "HumanWFarm")
                           && (100 * (_player.Mana / _player.MaxMana))
                           > getSliderItem(clearMenu, "Lanemana"));
            var useSpiQFarm = (_spiderQ.IsReady() && getCheckBoxItem(clearMenu, "SpiderQFarm"));
            var useSpiWFarm = (_spiderW.IsReady() && getCheckBoxItem(clearMenu, "SpiderWFarm"));
            var allminions = MinionManager.GetMinions(
                _player.ServerPosition,
                _humanQ.Range,
                MinionTypes.All,
                MinionTeam.Enemy,
                MinionOrderTypes.Health);
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                {
                    foreach (var minion in allminions)
                        if (_human)
                        {
                            if (useHumQ && _humanQ.IsReady() && minion.IsValidTarget()
                                && _player.Distance(minion) <= _humanQ.Range)
                            {
                                _humanQ.Cast(minion);
                            }
                            if (useHumW && _humanW.IsReady() && minion.IsValidTarget()
                                && _player.Distance(minion) <= _humanW.Range)
                            {
                                _humanW.Cast(minion);
                            }
                            if (useR && _r.IsReady())
                            {
                                _r.Cast();
                            }
                        }
                    foreach (var minion in allminions)
                        if (_spider)
                        {
                            if (useSpiQFarm && _spiderQ.IsReady() && minion.IsValidTarget()
                                && _player.Distance(minion) <= _spiderQ.Range)
                            {
                                _spiderQ.Cast(minion);
                            }
                            if (useSpiWFarm && _spiderW.IsReady() && minion.IsValidTarget()
                                && _player.Distance(minion) <= 125)
                            {
                                _spiderW.Cast();
                            }
                        }
                }
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
                {
                    foreach (var minion in allminions)
                        if (_human)
                        {
                            if (useHumQ && _player.GetSpellDamage(minion, SpellSlot.Q) > minion.Health
                                && _humanQ.IsReady() && minion.IsValidTarget()
                                && _player.Distance(minion) <= _humanQ.Range)
                            {
                                _humanQ.Cast(minion);
                            }
                            if (useHumW && _player.GetSpellDamage(minion, SpellSlot.W) > minion.Health
                                && _humanW.IsReady() && minion.IsValidTarget()
                                && _player.Distance(minion) <= _humanW.Range)
                            {
                                _humanW.Cast(minion);
                            }
                            if (useR && _r.IsReady())
                            {
                                _r.Cast();
                            }
                        }
                    foreach (var minion in allminions)
                        if (_spider)
                        {
                            if (useSpiQFarm && _spiderQ.IsReady()
                                && _player.GetSpellDamage(minion, SpellSlot.Q) > minion.Health && _spiderQ.IsReady()
                                && minion.IsValidTarget() && _player.Distance(minion) <= _spiderQ.Range)
                            {
                                _spiderQ.Cast(minion);
                            }
                            if (useSpiQFarm && _spiderW.IsReady() && minion.IsValidTarget()
                                && _player.Distance(minion) <= 125)
                            {
                                _spiderW.Cast();
                            }
                        }
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
            var useblue = getCheckBoxItem(smiteMenu, "Useblue");
            var usered = getCheckBoxItem(smiteMenu, "Usered");
            var health = (100 * (_player.Health / _player.MaxHealth)) < getSliderItem(smiteMenu, "healthJ");
            var mana = (100 * (_player.Mana / _player.MaxMana)) < getSliderItem(smiteMenu, "manaJ");
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
                    else if (jungle && useblue && mana && minion.Health >= smiteDmg
                             && jungleMinions.Any(name => minion.Name.StartsWith("SRU_Blue"))
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

        private static void AutoE()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            var target = TargetSelector.GetTarget(_humanE.Range, DamageType.Magical);

            if (_human && target.IsValidTarget(_humanE.Range) && _humanE.IsReady()
                && _humanE.GetPrediction(target).Hitchance >= HitChance.VeryHigh)
            {
                _humanE.Cast(target);
            }
        }

        /*private static bool Packets()
        {
            return _config.Item("usePackets").GetValue<bool>();
        }*/

        private static void Interrupter2_OnInterruptableTarget(
            AIHeroClient unit,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!getCheckBoxItem(miscMenu, "UseEInt")) return;
            if (unit.IsValidTarget(_humanE.Range) && _humanE.GetPrediction(unit).Hitchance >= HitChance.Low)
            {
                _humanE.Cast(unit);
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (_spiderE.IsReady() && _spider && gapcloser.Sender.IsValidTarget(_spiderE.Range)
                && getCheckBoxItem(miscMenu, "Spidergapcloser"))
            {
                _spiderE.Cast(gapcloser.Sender);
            }
            if (_humanE.IsReady() && _human && gapcloser.Sender.IsValidTarget(_humanE.Range)
                && getCheckBoxItem(miscMenu, "Humangapcloser"))
            {
                _humanE.Cast(gapcloser.Sender);
            }
        }

        private static float CalculateCd(float time)
        {
            return time + (time * _player.PercentCooldownMod);
        }

        private static void Cooldowns()
        {
            _humaQcd = ((_humQcd - Game.Time) > 0) ? (_humQcd - Game.Time) : 0;
            _humaWcd = ((_humWcd - Game.Time) > 0) ? (_humWcd - Game.Time) : 0;
            _humaEcd = ((_humEcd - Game.Time) > 0) ? (_humEcd - Game.Time) : 0;
            _spideQcd = ((_spidQcd - Game.Time) > 0) ? (_spidQcd - Game.Time) : 0;
            _spideWcd = ((_spidWcd - Game.Time) > 0) ? (_spidWcd - Game.Time) : 0;
            _spideEcd = ((_spidEcd - Game.Time) > 0) ? (_spidEcd - Game.Time) : 0;
        }

        private static void GetCDs(GameObjectProcessSpellCastEventArgs spell)
        {
            if (_human)
            {
                if (spell.SData.Name == "EliseHumanQ") _humQcd = Game.Time + CalculateCd(HumanQcd[_humanQ.Level]);
                if (spell.SData.Name == "EliseHumanW") _humWcd = Game.Time + CalculateCd(HumanWcd[_humanW.Level]);
                if (spell.SData.Name == "EliseHumanE") _humEcd = Game.Time + CalculateCd(HumanEcd[_humanE.Level]);
            }
            else
            {
                if (spell.SData.Name == "EliseSpiderQCast") _spidQcd = Game.Time + CalculateCd(SpiderQcd[_spiderQ.Level]);
                if (spell.SData.Name == "EliseSpiderW") _spidWcd = Game.Time + CalculateCd(SpiderWcd[_spiderW.Level]);
                if (spell.SData.Name == "EliseSpiderEInitial") _spidEcd = Game.Time + CalculateCd(SpiderEcd[_spiderE.Level]);
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
                    return HitChance.Medium;
            }
        }

        // Credits to Brain0305
        private static bool CheckingCollision(AIHeroClient target)
        {
            foreach (var col in MinionManager.GetMinions(_player.Position, 1500, MinionTypes.All, MinionTeam.NotAlly))
            {
                var segment = LeagueSharp.Common.Geometry.LSProjectOn(
                    col.ServerPosition.LSTo2D(),
                    _player.ServerPosition.LSTo2D(),
                    col.Position.LSTo2D());
                if (segment.IsOnSegment
                    && target.ServerPosition.LSTo2D().Distance(segment.SegmentPoint) <= GetHitBox(col) + 40)
                {
                    if (col.Distance(_player.Position) < _smite.Range
                        && col.Health < _player.GetSummonerSpellDamage(col, LeagueSharp.Common.Damage.SummonerSpell.Smite))
                    {
                        _player.Spellbook.CastSpell(_smiteSlot, col);
                        return true;
                    }
                }
            }
            return false;
        }

        // Credits to Brain0305
        static float GetHitBox(Obj_AI_Base minion)
        {
            var nameMinion = minion.Name.ToLower();
            if (nameMinion.Contains("mech")) return 65;
            if (nameMinion.Contains("wizard") || nameMinion.Contains("basic")) return 48;
            if (nameMinion.Contains("wolf") || nameMinion.Contains("wraith")) return 50;
            if (nameMinion.Contains("golem") || nameMinion.Contains("lizard")) return 80;
            if (nameMinion.Contains("dragon") || nameMinion.Contains("worm")) return 100;
            return 50;
        }

        private static void KillSteal()
        {
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy))
            {
                var igniteDmg = _player.GetSummonerSpellDamage(hero, LeagueSharp.Common.Damage.SummonerSpell.Ignite);
                var qhDmg = _player.GetSpellDamage(hero, SpellSlot.Q);
                var wDmg = _player.GetSpellDamage(hero, SpellSlot.W);

                if (hero.IsValidTarget(600) && getCheckBoxItem(ksMenu, "UseIgnite")
                    && _igniteSlot != SpellSlot.Unknown
                    && _player.Spellbook.CanUseSpell(_igniteSlot) == SpellState.Ready)
                {
                    if (igniteDmg > hero.Health)
                    {
                        _player.Spellbook.CastSpell(_igniteSlot, hero);
                    }
                }
                if (_human)
                {
                    if (_humanQ.IsReady() && hero.IsValidTarget(_humanQ.Range)
                        && getCheckBoxItem(ksMenu, "HumanQKs"))
                    {
                        if (hero.Health <= qhDmg)
                        {
                            _humanQ.Cast(hero);
                        }
                    }
                    if (_humanW.IsReady() && hero.IsValidTarget(_humanW.Range)
                        && getCheckBoxItem(ksMenu, "HumanWKs"))
                    {
                        if (hero.Health <= wDmg)
                        {
                            _humanW.Cast(hero);
                        }
                    }
                }
                if (_spider && _spiderQ.IsReady() && hero.IsValidTarget(_spiderQ.Range)
                    && getCheckBoxItem(ksMenu, "SpiderQKs"))
                {
                    if (hero.Health <= qhDmg)
                    {
                        _spiderQ.Cast(hero);
                    }
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            var elise = Drawing.WorldToScreen(_player.Position);

            if (getCheckBoxItem(drawMenu, "drawmode") && _smite != null)
            {
                if (getCheckBoxItem(smiteMenu, "smitecombo")
                    && (_player.GetSpell(_smiteSlot).Name.ToLower() == "s5_summonersmiteplayerganker"
                        || _player.GetSpell(_smiteSlot).Name.ToLower() == "s5_summonersmiteduel"))
                {
                    Drawing.DrawText(
                        Drawing.Width * 0.02f,
                        Drawing.Height * 0.90f,
                        System.Drawing.Color.GreenYellow,
                        "Smite Tagret");
                }
                else
                    Drawing.DrawText(
                        Drawing.Width * 0.02f,
                        Drawing.Height * 0.90f,
                        System.Drawing.Color.GreenYellow,
                        "Smite minion in Human E Path");
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
                        System.Drawing.Color.DarkRed,
                        "Smite Jungle On");
            }
            if (_human && getCheckBoxItem(drawMenu, "DrawQ"))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _humanQ.Range, System.Drawing.Color.GreenYellow);
            }
            if (_human && getCheckBoxItem(drawMenu, "DrawW"))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _humanW.Range, System.Drawing.Color.GreenYellow);
            }
            if (_human && getCheckBoxItem(drawMenu, "DrawE"))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _humanE.Range, System.Drawing.Color.GreenYellow);
            }
            if (_spider && getCheckBoxItem(drawMenu, "SpiderDrawQ"))
            {
                Render.Circle.DrawCircle(
                    ObjectManager.Player.Position,
                    _spiderQ.Range,
                    System.Drawing.Color.GreenYellow);
            }
            if (_spider && getCheckBoxItem(drawMenu, "SpiderDrawE")) 
            {
                Render.Circle.DrawCircle(
                    ObjectManager.Player.Position,
                    _spiderE.Range,
                    System.Drawing.Color.GreenYellow);
            }

            if (!getCheckBoxItem(drawMenu, "DrawCooldown")) return;
            if (!_spider)
            {
                if (_spideQcd == 0) Drawing.DrawText(elise[0] - 60, elise[1], Color.White, "SQ Rdy");
                else Drawing.DrawText(elise[0] - 60, elise[1], Color.Orange, "SQ: " + _spideQcd.ToString("0.0"));
                if (_spideWcd == 0) Drawing.DrawText(elise[0] - 20, elise[1] + 30, Color.White, "SW Rdy");
                else Drawing.DrawText(elise[0] - 20, elise[1] + 30, Color.Orange, "SW: " + _spideWcd.ToString("0.0"));
                if (_spideEcd == 0) Drawing.DrawText(elise[0], elise[1], Color.White, "SE Rdy");
                else Drawing.DrawText(elise[0], elise[1], Color.Orange, "SE: " + _spideEcd.ToString("0.0"));
            }
            else
            {
                if (_humaQcd == 0) Drawing.DrawText(elise[0] - 60, elise[1], Color.White, "HQ Rdy");
                else Drawing.DrawText(elise[0] - 60, elise[1], Color.Orange, "HQ: " + _humaQcd.ToString("0.0"));
                if (_humaWcd == 0) Drawing.DrawText(elise[0] - 20, elise[1] + 30, Color.White, "HW Rdy");
                else Drawing.DrawText(elise[0] - 20, elise[1] + 30, Color.Orange, "HW: " + _humaWcd.ToString("0.0"));
                if (_humaEcd == 0) Drawing.DrawText(elise[0], elise[1], Color.White, "HE Rdy");
                else Drawing.DrawText(elise[0], elise[1], Color.Orange, "HE: " + _humaEcd.ToString("0.0"));
            }
        }

        private static void CheckSpells()
        {
            if (_player.Spellbook.GetSpell(SpellSlot.Q).Name == "EliseHumanQ"
                || _player.Spellbook.GetSpell(SpellSlot.W).Name == "EliseHumanW"
                || _player.Spellbook.GetSpell(SpellSlot.E).Name == "EliseHumanE")
            {
                _human = true;
                _spider = false;
            }

            if (_player.Spellbook.GetSpell(SpellSlot.Q).Name == "EliseSpiderQCast"
                || _player.Spellbook.GetSpell(SpellSlot.W).Name == "EliseSpiderW"
                || _player.Spellbook.GetSpell(SpellSlot.E).Name == "EliseSpiderEInitial")
            {
                _human = false;
                _spider = true;
            }
        }
    }
}

