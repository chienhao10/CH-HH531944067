using System;
using System.Linq;
using EloBuddy;
using LeagueSharp.Common;
using EloBuddy.SDK;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace D_Corki
{
    internal class Program
    {
        private const string ChampionName = "Corki";

        private static Spell _q, _w, _e, _r, _r1, _r2;

        private static Menu Menu { get; set; }

        public static Menu comboMenu, harassMenu, itemMenu, clearMenu, miscMenu, lasthitMenu, jungleMenu, drawMenu;

        private static AIHeroClient _player;

        private static int Qcast, Rcast;

        private static Items.Item _youmuu, _blade, _bilge, _hextech;


        public static void Game_OnGameLoad()
        {
            _player = ObjectManager.Player;
            if (_player.ChampionName != ChampionName) return;

            _q = new Spell(SpellSlot.Q, 825f);
            _w = new Spell(SpellSlot.W, 800f);
            _e = new Spell(SpellSlot.E, 600f);
            _r = new Spell(SpellSlot.R);
            _r1 = new Spell(SpellSlot.R, 1300f);
            _r2 = new Spell(SpellSlot.R, 1500f);

            _q.SetSkillshot(0.35f, 250f, 1000f, false, SkillshotType.SkillshotCircle);
            _e.SetSkillshot(0f, (float)(45 * Math.PI / 180), 1500, false, SkillshotType.SkillshotCone);
            _r.SetSkillshot(0.20f, 40f, 2000f, true, SkillshotType.SkillshotLine);

            _youmuu = new Items.Item(3142, 10);
            _bilge = new Items.Item(3144, 450f);
            _blade = new Items.Item(3153, 450f);
            _hextech = new Items.Item(3146, 700);


            Menu = MainMenu.AddMenu("D-Corki", "D-Corki");

            comboMenu = Menu.AddSubMenu("Combo", "Combo");
            comboMenu.Add("UseQC", new CheckBox("Use Q"));
            comboMenu.Add("UseEC", new CheckBox("Use E"));
            comboMenu.Add("UseRC", new CheckBox("Use R"));

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
            harassMenu.Add("UseQH", new CheckBox("Use Q"));
            harassMenu.Add("UseEH", new CheckBox("Use E"));
            harassMenu.Add("UseRH", new CheckBox("Use R"));
            harassMenu.Add("RlimH", new Slider("R Amount >", 3, 1, 7));
            harassMenu.Add("harasstoggle", new KeyBind("AutoHarass (toggle)", false, KeyBind.BindTypes.PressToggle, 'L'));
            harassMenu.Add("Harrasmana", new Slider("Minimum Mana", 60, 1, 100));

            lasthitMenu = Menu.AddSubMenu("LastHit", "LastHit");
            lasthitMenu.Add("UseQLH", new CheckBox("Q LastHit"));
            lasthitMenu.Add("UseELH", new CheckBox("E LastHit"));
            lasthitMenu.Add("Lastmana", new Slider("Minimum Mana", 60, 1, 100));

            clearMenu = Menu.AddSubMenu("LaneClear", "LaneClear");
            clearMenu.Add("UseQL", new CheckBox("Q LaneClear"));
            clearMenu.Add("UseEL", new CheckBox("E LaneClear"));
            clearMenu.Add("UseRL", new CheckBox("R LaneClear"));
            clearMenu.Add("Lanemana", new Slider("Minimum Mana", 60, 1, 100));
            clearMenu.Add("RlimL", new Slider("R Amount >", 60, 1, 100));

            jungleMenu = Menu.AddSubMenu("JungleClear", "JungleClear");
            jungleMenu.Add("UseQJ", new CheckBox("Q Jungle"));
            jungleMenu.Add("UseEJ", new CheckBox("E Jungle"));
            jungleMenu.Add("UseRJ", new CheckBox("R Jungle"));
            jungleMenu.Add("Junglemana", new Slider("Minimum Mana", 60, 1, 100));
            jungleMenu.Add("RlimJ", new Slider("R Amount >", 60, 1, 100));

            miscMenu = Menu.AddSubMenu("Misc", "Misc");
            miscMenu.Add("UseQM", new CheckBox("Use Q KillSteal"));
            miscMenu.Add("UseEM", new CheckBox("Use E KillSteal"));
            miscMenu.Add("UseRM", new CheckBox("Use R KillSteal"));
            miscMenu.Add("delaycombo", new Slider("Delay between Q-R Use", 200, 0, 1500));

            drawMenu = Menu.AddSubMenu("Drawings", "Drawings");
            drawMenu.Add("DrawQ", new CheckBox("Draw Q", false));
            drawMenu.Add("DrawW", new CheckBox("Draw W", false));
            drawMenu.Add("DrawE", new CheckBox("Draw E", false));
            drawMenu.Add("DrawR", new CheckBox("Draw R", false));
            drawMenu.Add("Drawharass", new CheckBox("Draw AutoHarass", true));

            Chat.Print("<font color='#881df2'>D-Corki by Diabaths</font> Loaded.");
            Chat.Print(
                "<font color='#f2f21d'>If You like my work and want to support me,  plz donate via paypal in </font> <font color='#00e6ff'>ssssssssssmith@hotmail.com</font> (10) S");
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalker.OnPostAttack += Orbwalking_AfterAttack;
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
            _r.Range = _player.HasBuff("CorkiMissileBarrageCounterBig") ? _r2.Range : _r1.Range;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if ((Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)
                 || getKeyBindItem(harassMenu, "harasstoggle"))
                && (100 * (_player.Mana / _player.MaxMana)) > getSliderItem(harassMenu, "Harrasmana"))
            {
                Harass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear)
                && (100 * (_player.Mana / _player.MaxMana)) > getSliderItem(clearMenu, "Lanemana"))
            {
                Laneclear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)
                && (100 * (_player.Mana / _player.MaxMana)) > getSliderItem(jungleMenu, "Junglemana"))
            {
                JungleClear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit)
                && (100 * (_player.Mana / _player.MaxMana)) > getSliderItem(lasthitMenu, "Lastmana"))
            {
                LastHit();
            }

            _player = ObjectManager.Player;

            Orbwalker.DisableAttacking = false;

            Usecleanse();
            KillSteal();
            Usepotion();
        }

        private static void Usecleanse()
        {
            if (_player.IsDead
                || (getBoxItem(itemMenu, "Cleansemode") == 1)
                    && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                return;
            if (Cleanse(_player) && getCheckBoxItem(itemMenu, "useqss"))
            {
                if (_player.HasBuff("zedulttargetmark"))
                {
                    if (Items.HasItem(3140) && Items.CanUseItem(3140)) Utility.DelayAction.Add(1000, () => Items.UseItem(3140));
                    else if (Items.HasItem(3139) && Items.CanUseItem(3139)) Utility.DelayAction.Add(1000, () => Items.UseItem(3139));
                    else if (Items.HasItem(3137) && Items.CanUseItem(3137)) Utility.DelayAction.Add(1000, () => Items.UseItem(3137));
                }
                else
                {
                    if (Items.HasItem(3140) && Items.CanUseItem(3140)) Items.UseItem(3140);
                    else if (Items.HasItem(3139) && Items.CanUseItem(3139)) Items.UseItem(3139);
                    else if (Items.HasItem(3137) && Items.CanUseItem(3137)) Items.UseItem(3137);
                }
            }
        }

        private static bool Cleanse(AIHeroClient hero)
        {
            bool cc = false;
            if (getCheckBoxItem(itemMenu, "blind"))
            {
                if (hero.HasBuffOfType(BuffType.Blind))
                {
                    cc = true;
                }
            }

            if (getCheckBoxItem(itemMenu, "charm"))
            {
                if (hero.HasBuffOfType(BuffType.Charm))
                {
                    cc = true;
                }
            }

            if (getCheckBoxItem(itemMenu, "fear"))
            {
                if (hero.HasBuffOfType(BuffType.Fear))
                {
                    cc = true;
                }
            }

            if (getCheckBoxItem(itemMenu, "flee"))
            {
                if (hero.HasBuffOfType(BuffType.Flee))
                {
                    cc = true;
                }
            }

            if (getCheckBoxItem(itemMenu, "snare"))
            {
                if (hero.HasBuffOfType(BuffType.Snare))
                {
                    cc = true;
                }
            }

            if (getCheckBoxItem(itemMenu, "taunt"))
            {
                if (hero.HasBuffOfType(BuffType.Taunt))
                {
                    cc = true;
                }
            }

            if (getCheckBoxItem(itemMenu, "suppression"))
            {
                if (hero.HasBuffOfType(BuffType.Suppression))
                {
                    cc = true;
                }
            }

            if (getCheckBoxItem(itemMenu, "stun"))
            {
                if (hero.HasBuffOfType(BuffType.Stun))
                {
                    cc = true;
                }
            }

            if (getCheckBoxItem(itemMenu, "polymorph"))
            {
                if (hero.HasBuffOfType(BuffType.Polymorph))
                {
                    cc = true;
                }
            }

            if (getCheckBoxItem(itemMenu, "silence"))
            {
                if (hero.HasBuffOfType(BuffType.Silence))
                {
                    cc = true;
                }
            }

            return cc;
        }

        private static int UltiStucks()
        {
            return _player.Spellbook.GetSpell(SpellSlot.R).Ammo;
        }

        private static void Combo()
        {
            var useQ = getCheckBoxItem(comboMenu, "UseQC");
            var useE = getCheckBoxItem(comboMenu, "UseEC");
            var useR = getCheckBoxItem(comboMenu, "UseRC");
            var Qdelay = Environment.TickCount - Qcast;
            var Rdelay = Environment.TickCount - Rcast;

            if (useQ && _q.IsReady() && Rdelay >= getSliderItem(miscMenu, "delaycombo"))
            {
                var t = TargetSelector.GetTarget(_q.Range, DamageType.Magical);
                if (t.IsValidTarget(_q.Range) && _q.GetPrediction(t).Hitchance >= HitChance.High) _q.Cast(t, false, true);
                Qcast = Environment.TickCount;
            }

            if (useE && _e.IsReady())
            {
                var t = TargetSelector.GetTarget(_e.Range, DamageType.Magical);
                if (t.IsValidTarget(_e.Range) && _e.GetPrediction(t).Hitchance >= HitChance.High) _e.Cast(t, false, true);
            }

            if (useR && _r.IsReady() && Qdelay >= getSliderItem(miscMenu, "delaycombo"))
            {
                var t = TargetSelector.GetTarget(_r.Range, DamageType.Magical);
                if (t.IsValidTarget(_r.Range) && _r.GetPrediction(t).Hitchance >= HitChance.High) _r.Cast(t, false, true);
                Rcast = Environment.TickCount;
            }

            UseItemes();
        }

        private static void Harass()
        {
            var useQ = getCheckBoxItem(harassMenu, "UseQH");
            var useE = getCheckBoxItem(harassMenu, "UseEH");
            var useR = getCheckBoxItem(harassMenu, "UseRH");
            var rlimH = getSliderItem(harassMenu, "RlimH");       
            var Qdelay = Environment.TickCount - Qcast;
            var Rdelay = Environment.TickCount - Rcast;
            if (useQ && _q.IsReady() && Rdelay >= getSliderItem(miscMenu, "delaycombo"))
            {
                var t = TargetSelector.GetTarget(_q.Range, DamageType.Magical);
                if (t.IsValidTarget(_q.Range) && _q.GetPrediction(t).Hitchance >= HitChance.High) _q.Cast(t, false, true);
                Qcast = Environment.TickCount;
            }

            if (useE && _e.IsReady())
            {
                var t = TargetSelector.GetTarget(_e.Range, DamageType.Magical);
                if (t.IsValidTarget(_e.Range) && _e.GetPrediction(t).Hitchance >= HitChance.High) _e.Cast(t, false, true);
            }

            if (useR && _r.IsReady() && rlimH < UltiStucks() && Qdelay >= getSliderItem(miscMenu, "delaycombo"))
            {
                var t = TargetSelector.GetTarget(_r.Range, DamageType.Magical);
                if (t.IsValidTarget(_r.Range) && _r.GetPrediction(t).Hitchance >= HitChance.High) _r.Cast(t, false, true);
                Rcast = Environment.TickCount;
            }
        }

        private static void Orbwalking_AfterAttack(AttackableUnit target, EventArgs args)
        {
            var combo = Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
            var useQ = getCheckBoxItem(comboMenu, "UseQC"); 
            var useE = getCheckBoxItem(comboMenu, "UseEC");
            var useR = getCheckBoxItem(comboMenu, "UseRC");
            var Qdelay = Environment.TickCount - Qcast;
            var Rdelay = Environment.TickCount - Rcast;
            if (combo && target.IsMe && (target is AIHeroClient))
            {
                {
                    if (useQ && _q.IsReady() && Rdelay >= getSliderItem(miscMenu, "delaycombo"))
                    {
                        var t = TargetSelector.GetTarget(_q.Range, DamageType.Magical);
                        if (t.IsValidTarget(_q.Range) && _q.GetPrediction(t).Hitchance >= HitChance.High) _q.Cast(t, false, true);
                        Qcast = Environment.TickCount;
                    }

                    if (useE && _e.IsReady())
                    {
                        var t = TargetSelector.GetTarget(_e.Range, DamageType.Magical);
                        if (t.IsValidTarget(_e.Range) && _e.GetPrediction(t).Hitchance >= HitChance.High) _e.Cast(t, false, true);
                    }

                    if (useR && _r.IsReady() && Qdelay >= getSliderItem(miscMenu, "delaycombo"))
                    {
                        var t = TargetSelector.GetTarget(_r.Range, DamageType.Magical);
                        if (t.IsValidTarget(_r.Range) && _r.GetPrediction(t).Hitchance >= HitChance.High) _r.Cast(t, false, true);
                        Rcast = Environment.TickCount;
                    }
                }
            }
        }

        private static void Laneclear()
        {
            var Qdelay = Environment.TickCount - Qcast;
            var Rdelay = Environment.TickCount - Rcast;
            var rangedMinionsQ = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition,
                _q.Range + _q.Width + 30,
                MinionTypes.Ranged);
            var allMinionsQ = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition,
                _q.Range + _q.Width + 30,
                MinionTypes.All);
            var allMinionsE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _e.Range, MinionTypes.All);
            var allMinionsR = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _r.Range, MinionTypes.All);
            var rangedMinionsR = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition,
                _r.Range + _r.Width,
                MinionTypes.Ranged);
            var useQl = getCheckBoxItem(clearMenu, "UseQL");
            var useEl = getCheckBoxItem(clearMenu, "UseEL");
            var useRl = getCheckBoxItem(clearMenu, "UseRL");
            var rlimL = getSliderItem(clearMenu, "RlimL");
            if (_q.IsReady() && useQl && Rdelay >= getSliderItem(miscMenu, "delaycombo"))
            {
                var fl1 = _q.GetCircularFarmLocation(rangedMinionsQ, _q.Width);
                var fl2 = _q.GetCircularFarmLocation(allMinionsQ, _q.Width);

                if (fl1.MinionsHit >= 3)
                {
                    _q.Cast(fl1.Position);
                    Qcast = Environment.TickCount;
                }
                else if (fl2.MinionsHit >= 2 || allMinionsQ.Count == 1)
                {
                    _q.Cast(fl2.Position);
                    Qcast = Environment.TickCount;
                }
                else
                    foreach (var minion in allMinionsQ)
                        if (!Orbwalking.InAutoAttackRange(minion)
                            && minion.Health < 0.75 * _player.GetSpellDamage(minion, SpellSlot.Q))
                            _q.Cast(minion);
                Qcast = Environment.TickCount;
            }

            if (_e.IsReady() && useEl)
            {
                var fl2 = _w.GetLineFarmLocation(allMinionsE, _e.Width);

                if (fl2.MinionsHit >= 2 || allMinionsE.Count == 1)
                {
                    _e.Cast(fl2.Position);
                }
                else
                    foreach (var minion in allMinionsE)
                        if (!Orbwalking.InAutoAttackRange(minion)
                            && minion.Health < 0.75 * _player.GetSpellDamage(minion, SpellSlot.E))
                            _e.Cast(minion);
            }

            if (_r.IsReady() && useRl && rlimL < UltiStucks() && allMinionsR.Count > 3 && Qdelay >= getSliderItem(miscMenu, "delaycombo"))
            {
                var fl1 = _w.GetLineFarmLocation(rangedMinionsR, _r.Width);
                var fl2 = _w.GetLineFarmLocation(allMinionsR, _r.Width);

                if (fl1.MinionsHit >= 3)
                {
                    _r.Cast(fl1.Position);
                    Rcast = Environment.TickCount;
                }
                else if (fl2.MinionsHit >= 2 || allMinionsR.Count == 1)
                {
                    _r.Cast(fl2.Position);
                    Rcast = Environment.TickCount;
                }
                else
                    foreach (var minion in allMinionsR)
                        if (!Orbwalking.InAutoAttackRange(minion)
                            && minion.Health < 0.75 * _player.GetSpellDamage(minion, SpellSlot.R))
                            _r.Cast(minion);
                Rcast = Environment.TickCount;
            }
        }

        private static void LastHit()
        {
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _q.Range, MinionTypes.All);
            var useQ = getCheckBoxItem(lasthitMenu, "UseQLH");
            var useE = getCheckBoxItem(lasthitMenu, "UseELH");
            if (allMinions.Count < 3) return;
            foreach (var minion in allMinions)
            {
                if (useQ && _q.IsReady() && minion.Health < 0.75 * _player.GetSpellDamage(minion, SpellSlot.Q))
                {
                    _q.Cast(minion);
                }

                if (_w.IsReady() && useE && minion.Health < 0.75 * _player.GetSpellDamage(minion, SpellSlot.E))
                {
                    _e.Cast(minion);
                }
            }
        }

        private static void JungleClear()
        {
            var Qdelay = Environment.TickCount - Qcast;
            var Rdelay = Environment.TickCount - Rcast;
            var mob =
                MinionManager.GetMinions(
                    _player.ServerPosition,
                    _e.Range,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth).FirstOrDefault();
            var useQ = getCheckBoxItem(jungleMenu, "UseQJ");
            var useE = getCheckBoxItem(jungleMenu, "UseEJ");
            var useR = getCheckBoxItem(jungleMenu, "UseRJ");
            var rlimJ = getSliderItem(jungleMenu, "RlimJ");
            if (mob == null)
            {
                return;
            }

            if (useQ && _q.IsReady() && mob.IsValidTarget(_q.Range) && Rdelay >= getSliderItem(miscMenu, "delaycombo"))
            {
                _q.Cast(mob);
                Qcast = Environment.TickCount;
            }

            if (_e.IsReady() && useE && mob.IsValidTarget(_e.Range))
            {
                _e.Cast(mob);
            }

            if (_r.IsReady() && useR && rlimJ < UltiStucks() && mob.IsValidTarget(_q.Range) && Qdelay >= getSliderItem(miscMenu, "delaycombo"))
            {
                _r.Cast(mob);
                Rcast = Environment.TickCount;
            }
        }

        private static bool HasBigRocket()
        {
            return ObjectManager.Player.Buffs.Any(buff => buff.DisplayName.ToLower() == "corkimissilebarragecounterbig");
        }

        private static void KillSteal()
        {
            var Qdelay = Environment.TickCount - Qcast;
            var Rdelay = Environment.TickCount - Rcast;
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy))
            {
                if (_q.IsReady() && getCheckBoxItem(miscMenu, "UseQM") && Rdelay >= getSliderItem(miscMenu, "delaycombo"))
                {
                    if (_q.GetDamage(hero) > hero.Health && hero.IsValidTarget(_q.Range)
                        && _q.GetPrediction(hero).Hitchance >= HitChance.High)
                        _q.Cast(hero, false, true);
                    Qcast = Environment.TickCount;
                }

                if (_e.IsReady() && getCheckBoxItem(miscMenu, "UseEM"))
                {
                    if (_e.GetDamage(hero) > hero.Health && hero.IsValidTarget(_e.Range)
                        && _e.GetPrediction(hero).Hitchance >= HitChance.High)
                        _e.Cast(hero, false, true);
                }

                if (_r.IsReady() && getCheckBoxItem(miscMenu, "UseRM") && Qdelay >= getSliderItem(miscMenu, "delaycombo"))
                {
                    var t = TargetSelector.GetTarget(_r.Range, DamageType.Magical);
                    var bigRocket = HasBigRocket();
                    if (hero.IsValidTarget(bigRocket ? _r2.Range : _r1.Range)
                        && _r1.GetDamage(hero) * (bigRocket ? 1.5f : 1f) > hero.Health)
                        if (_r.GetPrediction(t).Hitchance >= HitChance.High) _r.Cast(t, false, true);
                    Rcast = Environment.TickCount;
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
                if (hero.IsValidTarget(450) && iBilge && (iBilgeEnemyhp || iBilgemyhp) && _bilge.IsReady())
                {
                    _bilge.Cast(hero);
                }

                if (hero.IsValidTarget(450) && iBlade && (iBladeEnemyhp || iBlademyhp) && _blade.IsReady())
                {
                    _blade.Cast(hero);
                }

                if (hero.IsValidTarget(450) && iYoumuu && _youmuu.IsReady())
                {
                    _youmuu.Cast();
                }

                if (hero.IsValidTarget(700) && iHextech && (iHextechEnemyhp || iHextechmyhp) && _hextech.IsReady())
                {
                    _hextech.Cast(hero);
                }
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

            if (ObjectManager.Player.CountEnemiesInRange(800) > 0
                || (mobs.Count > 0 && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear)))
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
            var harass = (getKeyBindItem(harassMenu, "harasstoggle"));

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

            if (getCheckBoxItem(drawMenu, "DrawQ") && _q.Level > 0)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _q.Range, _q.IsReady() ? System.Drawing.Color.GreenYellow : System.Drawing.Color.OrangeRed);
            }

            if (getCheckBoxItem(drawMenu, "DrawW") && _w.Level > 0)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _w.Range, System.Drawing.Color.GreenYellow);
            }

            if (getCheckBoxItem(drawMenu, "DrawE") && _e.Level > 0)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _e.Range, System.Drawing.Color.GreenYellow);
            }

            if (getCheckBoxItem(drawMenu, "DrawR") && _r.Level > 0)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _r.Range, System.Drawing.Color.GreenYellow);
            }
        }
    }
}