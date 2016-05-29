using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK;

//gg

namespace D_Jarvan
{
    internal class Program
    {
        private const string ChampionName = "JarvanIV";

        private static LeagueSharp.Common.Spell _q, _w, _e, _r;

        private static SpellSlot _igniteSlot;

        private static Items.Item _tiamat, _hydra, _blade, _bilge, _rand, _lotis, _youmuu;

        private static Menu _config, comboMenu, harassMenu, clearMenu, lastMenu, jungleMenu, miscMenu, drawMenu, forestMenu;

        private static AIHeroClient _player;

        private static bool _haveulti;

        private static SpellSlot _smiteSlot;

        private static LeagueSharp.Common.Spell _smite;

        private static SpellSlot _flashSlot;

        public static void Game_OnGameLoad()
        {
            _player = ObjectManager.Player;
            if (_player.ChampionName != ChampionName) return;

            _q = new LeagueSharp.Common.Spell(SpellSlot.Q, 770f);
            _w = new LeagueSharp.Common.Spell(SpellSlot.W, 300f);
            _e = new LeagueSharp.Common.Spell(SpellSlot.E, 830f);
            _r = new LeagueSharp.Common.Spell(SpellSlot.R, 650f);

            _q.SetSkillshot(0.5f, 70f, float.MaxValue, false, SkillshotType.SkillshotLine);
            _e.SetSkillshot(0.5f, 70f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            _igniteSlot = _player.GetSpellSlot("SummonerDot");
            _flashSlot = _player.GetSpellSlot("SummonerFlash");

            if (_player.GetSpell(SpellSlot.Summoner1).Name.ToLower().Contains("smite"))
            {
                _smite = new LeagueSharp.Common.Spell(SpellSlot.Summoner1, 570f);
                _smiteSlot = SpellSlot.Summoner1;
            }
            else if (_player.GetSpell(SpellSlot.Summoner2).Name.ToLower().Contains("smite"))
            {
                _smite = new LeagueSharp.Common.Spell(SpellSlot.Summoner2, 570f);
                _smiteSlot = SpellSlot.Summoner2;
            }

            _youmuu = new Items.Item(3142, 10);
            _bilge = new Items.Item(3144, 450f);
            _blade = new Items.Item(3153, 450f);
            _hydra = new Items.Item(3074, 250f);
            _tiamat = new Items.Item(3077, 250f);
            _rand = new Items.Item(3143, 490f);
            _lotis = new Items.Item(3190, 590f);

            //D Jarvan
            _config = MainMenu.AddMenu("D-Jarvan", "D-Jarvan");

            //Combo
            comboMenu = _config.AddSubMenu("Combo", "Combo");
            comboMenu.Add("UseIgnite", new CheckBox("Use Ignite"));
            comboMenu.Add("smitecombo", new CheckBox("Use Smite in target"));
            comboMenu.Add("UseQC", new CheckBox("Use Q"));
            comboMenu.Add("UseWC", new CheckBox("Use W"));
            comboMenu.Add("UseEC", new CheckBox("Use E"));
            comboMenu.Add("UseRC", new CheckBox("Use R(killable)"));
            comboMenu.Add("UseRE", new CheckBox("AutoR Min Targ"));
            comboMenu.Add("MinTargets", new Slider("Ult when>=min enemy(COMBO)", 2, 1, 5));
            comboMenu.Add("ActiveComboEQR", new KeyBind("ComboEQ-R!", false, KeyBind.BindTypes.HoldActive, 'T'));
            comboMenu.Add("ComboeqFlash", new KeyBind("ComboEQ- Flash!", false, KeyBind.BindTypes.HoldActive, 'H'));
            comboMenu.Add("FlashDista", new Slider("Flash Distance", 700, 700, 1000));

            //Harass
            harassMenu = _config.AddSubMenu("Harass", "Harass");
            harassMenu.Add("UseQH", new CheckBox("Use Q"));
            harassMenu.Add("UseEH", new CheckBox("Use E"));
            harassMenu.Add("UseEQH", new CheckBox("Use EQ Combo"));
            harassMenu.Add("UseEQHHP", new Slider("EQ If Your Hp > ", 85, 1, 100));
            harassMenu.Add("UseItemsharass", new CheckBox("Use Tiamat/Hydra"));
            harassMenu.Add("harassmana", new Slider("Minimum Mana% >", 35, 1, 100));
            harassMenu.Add("harasstoggle", new KeyBind("AutoHarass (toggle)", false, KeyBind.BindTypes.PressToggle, 'G'));

            //LaneClear
            clearMenu = _config.AddSubMenu("LaneFarm", "LaneFarm");
            clearMenu.Add("UseItemslane", new CheckBox("Use Items in LaneClear"));
            clearMenu.Add("UseQL", new CheckBox("Q LaneClear"));
            clearMenu.Add("UseEL", new CheckBox("E LaneClear"));
            clearMenu.Add("UseWL", new CheckBox("W LaneClear"));
            clearMenu.Add("UseWLHP", new Slider("use W if Hp% <", 35, 1, 100));
            clearMenu.Add("lanemana", new Slider("Minimum Mana% >", 35, 1, 100));

            // Last Hit
            lastMenu = _config.AddSubMenu("LastHit", "LastHit");
            lastMenu.Add("UseQLH", new CheckBox("Q LastHit"));
            lastMenu.Add("UseELH", new CheckBox("E LastHit"));
            lastMenu.Add("UseWLH", new CheckBox("W LaneClear"));
            lastMenu.Add("UseWLHHP", new Slider("use W if Hp% <", 35, 1, 100));
            lastMenu.Add("lastmana", new Slider("Minimum Mana% >", 35, 1, 100));

            // Jungle
            jungleMenu = _config.AddSubMenu("Jungle", "Jungle");
            jungleMenu.Add("UseItemsjungle", new CheckBox("Use Items in jungle"));
            jungleMenu.Add("UseQJ", new CheckBox("Q Jungle"));
            jungleMenu.Add("UseEJ", new CheckBox("E Jungle"));
            jungleMenu.Add("UseWJ", new CheckBox("W Jungle"));
            jungleMenu.Add(" UseEQJ", new CheckBox("EQ In Jungle"));
            jungleMenu.Add("UseWJHP", new Slider("use W if Hp% <", 35, 1, 100));
            jungleMenu.Add("junglemana", new Slider("Minimum Mana% >", 35, 1, 100));

            //Forest
            forestMenu = _config.AddSubMenu("Forest Gump", "Forest Gump");
            forestMenu.Add("UseEQF", new CheckBox("Use EQ in Mouse "));
            forestMenu.Add("UseWF", new CheckBox("Use W "));
            forestMenu.Add("Forest", new KeyBind("Active Forest Gump!", false, KeyBind.BindTypes.HoldActive, 'Z'));

            //Misc
            miscMenu = _config.AddSubMenu("Misc", "Misc");
            miscMenu.Add("UseIgnitekill", new CheckBox("Use Ignite KillSteal"));
            miscMenu.Add("UseQM", new CheckBox("Use Q KillSteal"));
            miscMenu.Add("UseRM", new CheckBox("Use R KillSteal"));
            miscMenu.Add("Gap_W", new CheckBox("W GapClosers"));
            miscMenu.Add("UseEQInt", new CheckBox("EQ to Interrupt"));

            //Drawings
            drawMenu = _config.AddSubMenu("Drawings", "Drawings");
            drawMenu.Add("DrawQ", new CheckBox("Draw Q", false));
            drawMenu.Add("DrawW", new CheckBox("Draw W", false));
            drawMenu.Add("DrawE", new CheckBox("Draw E", false));
            drawMenu.Add("DrawR", new CheckBox("Draw R", false));
            drawMenu.Add("DrawQR", new CheckBox("Draw EQ-R", false));
            drawMenu.Add("DrawEQF", new CheckBox("Draw EQ-Flash", false));
            drawMenu.Add("Drawsmite", new CheckBox("Draw smite"));
            drawMenu.Add("Drawharass", new CheckBox("Draw Auto Harass"));

            Chat.Print("<font color='#881df2'>D-Jarvan by Diabaths</font> Loaded.");
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            GameObject.OnCreate += OnCreateObj;
            GameObject.OnDelete += OnDeleteObj;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
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
            if (getKeyBindItem(forestMenu, "Forest"))
            {
                Forest();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (getKeyBindItem(comboMenu, "ActiveComboEQR"))
            {
                ComboEqr();
            }

            if ((Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)
                 || getKeyBindItem(harassMenu, "harasstoggle"))
                && (100 * (_player.Mana / _player.MaxMana)) > getSliderItem(harassMenu, "harassmana")
                && !(Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)
                     || getKeyBindItem(comboMenu, "ActiveComboEQR")
                     || getKeyBindItem(comboMenu, "ComboeqFlash")))
            {
                Harass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                if ((100 * (_player.Mana / _player.MaxMana)) > getSliderItem(clearMenu, "lanemana"))
                {
                    Laneclear();
                }

                if ((100 * (_player.Mana / _player.MaxMana)) > getSliderItem(jungleMenu, "junglemana"))
                {
                    JungleClear();
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit)
                && (100 * (_player.Mana / _player.MaxMana)) > getSliderItem(lastMenu, "lastmana"))
            {
                LastHit();
            }

            if (getKeyBindItem(comboMenu, "ComboeqFlash"))
            {
                ComboeqFlash();
            }

            _player = ObjectManager.Player;

            Orbwalker.DisableAttacking = false;

            KillSteal();
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (_w.IsReady() && gapcloser.Sender.LSIsValidTarget(_w.Range) && getCheckBoxItem(miscMenu, "Gap_W"))
            {
                _w.Cast(gapcloser.Sender);
            }
        }

        private static void Interrupter2_OnInterruptableTarget(
            AIHeroClient unit,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            var manacheck = _player.Mana
                            > _player.Spellbook.GetSpell(SpellSlot.Q).SData.Mana
                            + _player.Spellbook.GetSpell(SpellSlot.E).SData.Mana;
            if (unit.LSIsValidTarget(_e.Range) && getCheckBoxItem(miscMenu, "UseEQInt") && manacheck)
            {
                var vector = unit.ServerPosition - ObjectManager.Player.Position;
                var Behind = _e.GetPrediction(unit).CastPosition + Vector3.Normalize(vector) * 100;
                _e.Cast(Behind);
                _q.Cast(Behind);
            }
        }

        private static float ComboDamage(Obj_AI_Base enemy)
        {
            var damage = 0d;
            if (_igniteSlot != SpellSlot.Unknown && _player.Spellbook.CanUseSpell(_igniteSlot) == SpellState.Ready) damage += ObjectManager.Player.GetSummonerSpellDamage(enemy, LeagueSharp.Common.Damage.SummonerSpell.Ignite);
            if (Items.HasItem(3077) && Items.CanUseItem(3077)) damage += _player.GetItemDamage(enemy, LeagueSharp.Common.Damage.DamageItems.Tiamat);
            if (Items.HasItem(3074) && Items.CanUseItem(3074)) damage += _player.GetItemDamage(enemy, LeagueSharp.Common.Damage.DamageItems.Hydra);
            if (Items.HasItem(3153) && Items.CanUseItem(3153)) damage += _player.GetItemDamage(enemy, LeagueSharp.Common.Damage.DamageItems.Botrk);
            if (Items.HasItem(3144) && Items.CanUseItem(3144)) damage += _player.GetItemDamage(enemy, LeagueSharp.Common.Damage.DamageItems.Bilgewater);
            if (_q.IsReady()) damage += _player.LSGetSpellDamage(enemy, SpellSlot.Q) * 2 * 1.2;
            if (_e.IsReady()) damage += _player.LSGetSpellDamage(enemy, SpellSlot.E);
            if (_r.IsReady()) damage += _player.LSGetSpellDamage(enemy, SpellSlot.R);

            damage += _player.LSGetAutoAttackDamage(enemy, true) * 1.1;
            damage += _player.LSGetAutoAttackDamage(enemy, true);
            return (float)damage;
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

        private static void Combo()
        {
            var useQ = getCheckBoxItem(comboMenu, "UseQC");
            var useW = getCheckBoxItem(comboMenu, "UseWC");
            var useE = getCheckBoxItem(comboMenu, "UseEC");
            var useR = getCheckBoxItem(comboMenu, "UseRC");
            var autoR = getCheckBoxItem(comboMenu, "UseRE");
            var cooldown = _player.GetSpell(SpellSlot.E).CooldownExpires;
            var CD = (int)(cooldown - (Game.Time - 1));
            var t = TargetSelector.GetTarget(_e.Range, DamageType.Magical);
            Smiteontarget();
            if (t.LSIsValidTarget(600) && getCheckBoxItem(comboMenu, "UseIgnite") && _igniteSlot != SpellSlot.Unknown
                && _player.Spellbook.CanUseSpell(_igniteSlot) == SpellState.Ready)
            {
                if (ComboDamage(t) > t.Health)
                {
                    _player.Spellbook.CastSpell(_igniteSlot, t);
                }
            }

            if (useR && _r.IsReady())
            {
                if (t.LSIsValidTarget(_r.Range) && !_haveulti) if (!t.HasBuff("JudicatorIntervention") && !t.HasBuff("Undying Rage") && ComboDamage(t) > t.Health) _r.CastIfHitchanceEquals(t, HitChance.Medium);
            }

            if (useE && _e.IsReady() && t.LSIsValidTarget(_e.Range) && _q.IsReady())
            {
                var vector = t.ServerPosition - ObjectManager.Player.Position;
                var Behind = _e.GetPrediction(t).CastPosition + Vector3.Normalize(vector) * 100;
                _e.Cast(Behind);
                _q.Cast(Behind);
            }

            if (useW && _w.IsReady())
            {
                if (t.LSIsValidTarget(_w.Range)) LeagueSharp.Common.Utility.DelayAction.Add(1000, () => _w.Cast());
            }

            if (useQ && _q.IsReady() && !_e.IsReady() && CD >= 3)
            {
                if (t.LSIsValidTarget(_q.Range) && _q.GetPrediction(t).Hitchance >= HitChance.High) _q.Cast(t);
            }

            if (_r.IsReady() && autoR && !_haveulti)
            {
                var target = TargetSelector.GetTarget(_e.Range, DamageType.Magical);
                if (ObjectManager.Get<AIHeroClient>().Count(hero => hero.LSIsValidTarget(_r.Range))
                    >= getSliderItem(comboMenu, "MinTargets")
                    && _r.GetPrediction(target).Hitchance >= HitChance.High) _r.Cast(target);
            }

            if (_haveulti)
            {
                Orbwalker.DisableAttacking = false;
            }
        }

        private static void ComboEqr()
        {
            var manacheck = _player.Mana
                            > _player.Spellbook.GetSpell(SpellSlot.Q).SData.Mana
                            + _player.Spellbook.GetSpell(SpellSlot.E).SData.Mana
                            + _player.Spellbook.GetSpell(SpellSlot.R).SData.Mana;
            var t = TargetSelector.GetTarget(_q.Range + _r.Range, DamageType.Magical);
            if (t == null)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }
            else
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, t);
            }

            Smiteontarget();
            if (_e.IsReady() && _q.IsReady() && manacheck && t.LSIsValidTarget(_q.Range))
            {
                if (t != null)
                {
                    _e.Cast(t.ServerPosition);
                    _q.Cast(t.ServerPosition);
                }
            }

            if (t.LSIsValidTarget(600) && getCheckBoxItem(comboMenu, "UseIgnite") && _igniteSlot != SpellSlot.Unknown
                && _player.Spellbook.CanUseSpell(_igniteSlot) == SpellState.Ready)
            {
                if (t != null && ComboDamage(t) > t.Health)
                {
                    _player.Spellbook.CastSpell(_igniteSlot, t);
                }
            }

            if (_r.IsReady() && !_haveulti && t.LSIsValidTarget(_r.Range))
            {
                _r.CastIfHitchanceEquals(t, HitChance.Immobile);
            }

            if (_haveulti)
            {
                Orbwalker.DisableAttacking = false;
            }

            if (_w.IsReady())
            {
                if (t.LSIsValidTarget(_w.Range)) LeagueSharp.Common.Utility.DelayAction.Add(1000, () => _w.Cast());
            }
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(_e.Range, DamageType.Magical);
            var useQ = getCheckBoxItem(harassMenu, "UseQH");
            var useE = getCheckBoxItem(harassMenu, "UseEH");
            var useEq = getCheckBoxItem(harassMenu, "UseEQH");
            var useEqhp = (100 * (_player.Health / _player.MaxHealth))
                          > getSliderItem(harassMenu, "UseEQHHP");
            var useItemsH = getCheckBoxItem(harassMenu, "UseItemsharass");
            if (useEqhp && useEq && _q.IsReady() && _e.IsReady() && target.LSIsValidTarget(_e.Range))
            {
                var vector = target.ServerPosition - ObjectManager.Player.Position;
                var Behind = _e.GetPrediction(target).CastPosition + Vector3.Normalize(vector) * 100;
                _e.Cast(Behind);
                _q.Cast(Behind);
            }

            if (useQ && _q.IsReady())
            {
                var t = TargetSelector.GetTarget(_e.Range, DamageType.Magical);
                if (t.LSIsValidTarget(_q.Range)) _q.Cast(t);
            }

            if (useE && _e.IsReady())
            {
                var t = TargetSelector.GetTarget(_e.Range, DamageType.Magical);
                if (t.LSIsValidTarget(_e.Range)) _e.Cast(t);
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

        private static void ComboeqFlash()
        {
            var flashDista = getSliderItem(comboMenu, "FlashDista");
            var manacheck = _player.Mana
                            > _player.Spellbook.GetSpell(SpellSlot.Q).SData.Mana
                            + _player.Spellbook.GetSpell(SpellSlot.E).SData.Mana;
            var t = TargetSelector.GetTarget(_q.Range + 800, DamageType.Magical);
            if (t == null)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }
            else
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, t);
            }

            Smiteontarget();
            if (_flashSlot != SpellSlot.Unknown && _player.Spellbook.CanUseSpell(_flashSlot) == SpellState.Ready)
            {
                if (_e.IsReady() && _q.IsReady() && manacheck && !t.LSIsValidTarget(_e.Range))
                {
                    if (t != null)
                    {
                        var vector = t.ServerPosition - ObjectManager.Player.Position;
                        var Behind = _e.GetPrediction(t).CastPosition + Vector3.Normalize(vector) * 100;
                        _e.Cast(Behind);
                        _q.Cast(Behind);
                    }
                }

                if (t.LSIsValidTarget(flashDista) && !_q.IsReady())
                {
                    if (t != null) _player.Spellbook.CastSpell(_flashSlot, t.ServerPosition);
                }
            }

            if (_haveulti)
            {
                Orbwalker.DisableAttacking = false;
            }
        }

        private static void Laneclear()
        {
            var allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _q.Range, MinionTypes.All);
            var rangedMinionsQ = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition,
                _q.Range + _q.Width,
                MinionTypes.Ranged);
            var rangedMinionsE = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition,
                _e.Range + _e.Width,
                MinionTypes.Ranged);
            var allMinionsE = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition,
                _e.Range + _e.Width,
                MinionTypes.All);
            var useItemsl = getCheckBoxItem(clearMenu, "UseItemslane");
            var useQl = getCheckBoxItem(clearMenu, "UseQL");
            var useEl = getCheckBoxItem(clearMenu, "UseEL");
            var useWl = getCheckBoxItem(clearMenu, "UseWL");
            var usewhp = (100 * (_player.Health / _player.MaxHealth)) < getSliderItem(clearMenu, "UseWLHP");

            if (_q.IsReady() && useQl)
            {
                var fl1 = _q.GetLineFarmLocation(rangedMinionsQ, _q.Width);
                var fl2 = _q.GetLineFarmLocation(allMinionsQ, _q.Width);

                if (fl1.MinionsHit >= 3)
                {
                    _q.Cast(fl1.Position);
                }
                else if (fl2.MinionsHit >= 2 || allMinionsQ.Count == 1)
                {
                    _q.Cast(fl2.Position);
                }
                else
                    foreach (var minion in allMinionsQ)
                        if (!Orbwalking.InAutoAttackRange(minion)
                            && minion.Health < 0.75 * _player.LSGetSpellDamage(minion, SpellSlot.Q)) _q.Cast(minion);
            }

            if (_e.IsReady() && useEl)
            {
                var fl1 = _e.GetCircularFarmLocation(rangedMinionsE, _e.Width);
                var fl2 = _e.GetCircularFarmLocation(allMinionsE, _e.Width);

                if (fl1.MinionsHit >= 3)
                {
                    _e.Cast(fl1.Position);
                }
                else if (fl2.MinionsHit >= 2 || allMinionsE.Count == 1)
                {
                    _e.Cast(fl2.Position);
                }
                else
                    foreach (var minion in allMinionsE)
                        if (!Orbwalking.InAutoAttackRange(minion)
                            && minion.Health < 0.75 * _player.LSGetSpellDamage(minion, SpellSlot.E)) _e.Cast(minion);
            }

            if (usewhp && useWl && _w.IsReady() && allMinionsQ.Count > 0)
            {
                _w.Cast();

            }

            foreach (var minion in allMinionsQ)
            {
                if (useItemsl && _tiamat.IsReady() && minion.LSIsValidTarget(_tiamat.Range))
                {
                    _tiamat.Cast();
                }
                if (useItemsl && _hydra.IsReady() && minion.LSIsValidTarget(_tiamat.Range))
                {
                    _hydra.Cast();
                }
            }
        }

        private static void LastHit()
        {
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _q.Range, MinionTypes.All);
            var useQ = getCheckBoxItem(lastMenu, "UseQLH");
            var useW = getCheckBoxItem(lastMenu, "UseWLH");
            var useE = getCheckBoxItem(lastMenu, "UseELH");
            var usewhp = (100 * (_player.Health / _player.MaxHealth))
                         < getSliderItem(lastMenu, "UseWLHHP");
            foreach (var minion in allMinions)
            {
                if (useQ && _q.IsReady() && _player.LSDistance(minion) < _q.Range
                    && minion.Health < 0.95 * _player.LSGetSpellDamage(minion, SpellSlot.Q))
                {
                    _q.Cast(minion);
                }

                if (_e.IsReady() && useE && _player.LSDistance(minion) < _e.Range
                    && minion.Health < 0.95 * _player.LSGetSpellDamage(minion, SpellSlot.E))
                {
                    _e.Cast(minion);
                }

                if (usewhp && useW && _w.IsReady() && allMinions.Count > 0)
                {
                    _w.Cast();
                }
            }
        }

        private static void JungleClear()
        {
            var mobs = MinionManager.GetMinions(
                _player.ServerPosition,
                _q.Range,
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);
            var useItemsJ = getCheckBoxItem(jungleMenu, "UseItemsjungle");
            var useQ = getCheckBoxItem(jungleMenu, "UseQJ");
            var useW = getCheckBoxItem(jungleMenu, "UseWJ");
            var useE = getCheckBoxItem(jungleMenu, "UseEJ");
            var useEq = getCheckBoxItem(jungleMenu, " UseEQJ");
            var usewhp = (100 * (_player.Health / _player.MaxHealth)) < getSliderItem(jungleMenu, "UseWJHP");

            if (mobs.Count > 0)
            {
                var mob = mobs[0];
                if (useEq && !mob.Name.Contains("Mini"))
                {
                    if (_e.IsReady() && useE && _player.LSDistance(mob) < _q.Range)
                    {
                        _e.Cast(mob);
                    }

                    if (useQ && _q.IsReady() && _player.LSDistance(mob) < _q.Range)
                    {
                        _q.Cast(mob);
                    }
                }
                else if (!mob.Name.Contains("Mini"))
                {
                    if (useQ && _q.IsReady() && _player.LSDistance(mob) < _q.Range)
                    {
                        _q.Cast(mob);
                    }

                    if (_e.IsReady() && useE && _player.LSDistance(mob) < _q.Range)
                    {
                        _e.Cast(mob);
                    }
                }

                if (_w.IsReady() && useW && usewhp && _player.LSDistance(mob) < _w.Range)
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(500, () => _w.Cast());
                }

                if (useItemsJ && _tiamat.IsReady() && mob.LSIsValidTarget(_tiamat.Range))
                {
                    _tiamat.Cast();
                }

                if (useItemsJ && _hydra.IsReady() && mob.LSIsValidTarget(_tiamat.Range))
                {
                    _hydra.Cast();
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


        private static void KillSteal()
        {
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy))
            {
                var igniteDmg = _player.GetSummonerSpellDamage(hero, LeagueSharp.Common.Damage.SummonerSpell.Ignite);
                if (hero.LSIsValidTarget(600) && getCheckBoxItem(miscMenu, "UseIgnitekill")
                    && _igniteSlot != SpellSlot.Unknown
                    && _player.Spellbook.CanUseSpell(_igniteSlot) == SpellState.Ready)
                {
                    if (igniteDmg > hero.Health)
                    {
                        _player.Spellbook.CastSpell(_igniteSlot, hero);
                    }
                }

                if (_q.IsReady() && getCheckBoxItem(miscMenu, "UseQM"))
                {
                    if (hero.LSIsValidTarget(_q.Range) && _q.GetPrediction(hero).Hitchance >= HitChance.High
                        && _player.LSGetSpellDamage(hero, SpellSlot.Q) > hero.Health)
                    {
                        _q.Cast(hero);
                    }
                }

                if (_r.IsReady() && getCheckBoxItem(miscMenu, "UseRM"))
                {
                    if (hero.LSIsValidTarget(_r.Range))
                        if (!hero.HasBuff("JudicatorIntervention") && !hero.HasBuff("Undying Rage")
                            && _r.GetDamage(hero) > hero.Health) _r.Cast(hero, false, true);
                }
            }
        }

        private static void Forest()
        {
            var manacheck = _player.Mana
                            > _player.Spellbook.GetSpell(SpellSlot.Q).SData.Mana
                            + _player.Spellbook.GetSpell(SpellSlot.E).SData.Mana;
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            var target = TargetSelector.GetTarget(_e.Range, DamageType.Magical);

            if (getCheckBoxItem(forestMenu, "UseEQF") && _q.IsReady() && _e.IsReady() && manacheck)
            {
                _e.Cast(Game.CursorPos);
                _q.Cast(Game.CursorPos);
            }

            if (getCheckBoxItem(forestMenu, "UseWF") && _w.IsReady() && target != null
                && _player.LSDistance(target) < _w.Range)
            {
                _w.Cast();
            }
        }

        private static void OnCreateObj(GameObject sender, EventArgs args)
        {
            if (!(sender is Obj_GeneralParticleEmitter)) return;
            var obj = (Obj_GeneralParticleEmitter)sender;

            if (obj.IsMe && obj.Name == "JarvanCataclysm_tar")
            {
                _haveulti = true;
            }
        }

        private static void OnDeleteObj(GameObject sender, EventArgs args)
        {
            if (!(sender is Obj_GeneralParticleEmitter)) return;

            var obj = (Obj_GeneralParticleEmitter)sender;
            if (obj.IsMe && obj.Name == "JarvanCataclysm_tar")
            {
                _haveulti = false;
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
            if (getCheckBoxItem(drawMenu, "Drawsmite") && _smite != null)
            {
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
            if (getCheckBoxItem(comboMenu, "DrawQ"))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _q.Range, System.Drawing.Color.GreenYellow);
            }
            if (getCheckBoxItem(comboMenu, "DrawW"))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _w.Range, System.Drawing.Color.GreenYellow);
            }
            if (getCheckBoxItem(comboMenu, "DrawE"))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _e.Range, System.Drawing.Color.GreenYellow);
            }

            if (getCheckBoxItem(comboMenu, "DrawR"))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _r.Range, System.Drawing.Color.GreenYellow);
            }
            if (getCheckBoxItem(comboMenu, "DrawQR"))
            {
                Render.Circle.DrawCircle(
                    ObjectManager.Player.Position,
                    _q.Range + _r.Range,
                    System.Drawing.Color.GreenYellow);
            }
            if (getCheckBoxItem(comboMenu, "DrawEQF"))
            {
                Render.Circle.DrawCircle(
                    ObjectManager.Player.Position,
                    _q.Range + getSliderItem(comboMenu, "FlashDista"),
                    System.Drawing.Color.GreenYellow);
            }
        }
    }
}






