#region

using System;
using System.Linq;
using EloBuddy;
using LeagueSharp.Common;
using EloBuddy.SDK;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

#endregion

namespace D_Graves
{
    using SharpDX;

    internal class Program
    {
        private const string ChampionName = "Graves";


        private static Spell _q, _w, _e, _r;

        private static SpellSlot _smiteSlot;

        private static Spell _smite;

        private static Menu Menu { get; set; }

        public static Menu comboMenu, harassMenu, itemMenu, clearMenu, miscMenu, lasthitMenu, jungleMenu, drawMenu, smiteMenu;

        private static AIHeroClient _player;

        private static Items.Item _youmuu, _blade, _bilge;

 
        public static void Game_OnGameLoad()
        {
            _player = ObjectManager.Player;

            if (_player.ChampionName != ChampionName) return;

            _q = new Spell(SpellSlot.Q, 950F);
            _w = new Spell(SpellSlot.W, 950f);
            _e = new Spell(SpellSlot.E, 450f);
            _r = new Spell(SpellSlot.R, 1500f);

            _q.SetSkillshot(0.25f, 60f, 2000f, false, SkillshotType.SkillshotLine);
            _w.SetSkillshot(0.35f, 150f, 1650f, false, SkillshotType.SkillshotCircle);
            _r.SetSkillshot(0.25f, 100f, 2100f, false, SkillshotType.SkillshotLine);

            _youmuu = new Items.Item(3142, 10);
            _bilge = new Items.Item(3144, 450f);
            _blade = new Items.Item(3153, 450f);

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

            Menu = MainMenu.AddMenu("D-Graves", "D-Graves");

            comboMenu = Menu.AddSubMenu("Combo", "Combo");
            comboMenu.Add("smitecombo", new CheckBox("Use Smite in target"));
            comboMenu.Add("UseQC", new CheckBox("Use Q"));
            comboMenu.Add("UseWC", new CheckBox("Use W"));
            comboMenu.Add("UseEC", new CheckBox("Use E"));
            comboMenu.Add("UseRC", new CheckBox("Use R"));
            comboMenu.Add("UseEreload", new CheckBox("Use E to Reload"));
            comboMenu.Add("UseRE", new CheckBox("Use R if hits X enemies"));
            comboMenu.Add("MinTargets", new Slider("Use R if Hit Enemys >=", 2, 1, 5));
            comboMenu.Add("useRaim", new KeyBind("Manual R", false, KeyBind.BindTypes.HoldActive, 'T'));


            itemMenu = Menu.AddSubMenu("Items", "items");
            itemMenu.Add("Youmuu", new CheckBox("Use Youmuu's"));
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
            harassMenu.Add("UseQH", new CheckBox("Use Q"));
            harassMenu.Add("UseWH", new CheckBox("Use W"));
            harassMenu.Add("Harrasmana", new Slider("Minimum Mana", 60, 1, 100));
            harassMenu.Add("harasstoggle", new KeyBind("AutoHarass (toggle)", false, KeyBind.BindTypes.HoldActive, 'L'));


            lasthitMenu = Menu.AddSubMenu("LastHit", "LastHit");
            lasthitMenu.Add("UseQLH", new CheckBox("Q LastHit"));
            lasthitMenu.Add("UseWLH", new CheckBox("W LastHit"));
            lasthitMenu.Add("Lastmana", new Slider("Minimum Mana", 60, 1, 100));

            clearMenu = Menu.AddSubMenu("LaneClear", "LaneClear");
            clearMenu.Add("UseQL", new CheckBox("Q LaneClear"));
            clearMenu.Add("minminions", new Slider("Minimum minions to use Q", 3, 1, 6));
            clearMenu.Add("UseWL", new CheckBox("W LaneClear", false));
            clearMenu.Add("minminionsw", new Slider("Minimum minions to use W", 3, 1, 5));
            clearMenu.Add("Lanemana", new Slider("Minimum Mana", 60, 1, 100));

            jungleMenu = Menu.AddSubMenu("JungleClear", "JungleClear");
            jungleMenu.Add("UseQJ", new CheckBox("Q Jungle"));
            jungleMenu.Add("UseWJ", new CheckBox("W Jungle"));
            jungleMenu.Add("Junglemana", new Slider("Minimum Mana", 60, 1, 100));

            smiteMenu = Menu.AddSubMenu("Smite", "Smite");
            smiteMenu.Add("Usesmite", new KeyBind("Use Smite(toggle)", false, KeyBind.BindTypes.PressToggle, 'N'));
            smiteMenu.Add("Useblue", new CheckBox("Smite Blue Early"));
            smiteMenu.Add("manaJ", new Slider("Smite Blue Early if MP% <", 30, 1, 100));
            smiteMenu.Add("Usered", new CheckBox("Smite Red Early"));
            smiteMenu.Add("healthJ", new Slider("Smite Red Early if HP% <", 30, 1, 100));

            miscMenu = Menu.AddSubMenu("Misc", "Misc");
            miscMenu.Add("UseQM", new CheckBox("Use Q KillSteal"));
            miscMenu.Add("UseWM", new CheckBox("Use W KillSteal"));
            miscMenu.Add("UseRM", new CheckBox("Use R KillSteal"));
            miscMenu.Add("Gap_W", new CheckBox("GapClosers W"));
            miscMenu.Add("Gap_E", new CheckBox("GapClosers E"));


            drawMenu = Menu.AddSubMenu("Drawings", "Drawings");
            drawMenu.Add("DrawQ", new CheckBox("Draw Q", false));
            drawMenu.Add("DrawW", new CheckBox("Draw W", false));
            drawMenu.Add("DrawE", new CheckBox("Draw E", false));
            drawMenu.Add("DrawR", new CheckBox("Draw R", false));
            drawMenu.Add("Drawsmite", new CheckBox("Draw smite", true));
            drawMenu.Add("Drawharass", new CheckBox("Draw Auto Harass", true));

            Chat.Print("<font color='#881df2'>D-Graves by Diabaths</font> Loaded.");
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Orbwalker.OnPostAttack += Orbwalking_AfterAttack;
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
            if (getKeyBindItem(comboMenu, "useRaim") && _r.IsReady())
            {
                var t = TargetSelector.GetTarget(_r.Range + 300, DamageType.Physical);
                if (t.IsValidTarget()) _r.Cast(t, true, true);
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)
                && (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)
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

            if (getKeyBindItem(smiteMenu, "Usesmite"))
            {
                Smiteuse();
            }

            _player = ObjectManager.Player;

            Orbwalker.DisableAttacking = false;
            Usecleanse();
            KillSteal();
            Usepotion();
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

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (_w.IsReady() && gapcloser.Sender.IsValidTarget(_w.Range) && getCheckBoxItem(miscMenu, "Gap_W")) _w.Cast(gapcloser.Sender.IsMelee() ? _player : gapcloser.Sender);

            if (_e.IsReady() && gapcloser.Sender.Distance(_player.ServerPosition) <= 200
                && getCheckBoxItem(miscMenu, "Gap_E"))
            {
                _e.Cast(ObjectManager.Player.Position.Extend(gapcloser.Sender.Position, -_e.Range));
            }
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

        private static void Smiteontarget()
        {
            if (_smite == null) return;
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy))
            {
                var smiteDmg = _player.GetSummonerSpellDamage(hero, LeagueSharp.Common.Damage.SummonerSpell.Smite);
                var usesmite = getCheckBoxItem(comboMenu, "smitecombo");
                if (_player.GetSpell(_smiteSlot).Name.ToLower() == "s5_summonersmiteplayerganker" && usesmite
                    && ObjectManager.Player.Spellbook.CanUseSpell(_smiteSlot) == SpellState.Ready
                    && hero.IsValidTarget(570))
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
                    && ObjectManager.Player.Spellbook.CanUseSpell(_smiteSlot) == SpellState.Ready
                    && hero.IsValidTarget(570))
                {
                    ObjectManager.Player.Spellbook.CastSpell(_smiteSlot, hero);
                }
            }
        }

        private static void Combo()
        {
            var useQ = getCheckBoxItem(comboMenu, "UseQC");
            var useW = getCheckBoxItem(comboMenu, "UseWC");
            var useR = getCheckBoxItem(comboMenu, "UseRC");
            var autoR = getCheckBoxItem(comboMenu, "UseRE");

            if (getCheckBoxItem(comboMenu, "smitecombo"))
            {
                Smiteontarget();
            }

            if (useQ && _q.IsReady())
            {
                var t = TargetSelector.GetTarget(_q.Range, DamageType.Physical);
                if (t.IsValidTarget(_q.Range - 70)) _q.CastIfHitchanceEquals(t, HitChance.High, true);
            }

            if (useW && _w.IsReady())
            {
                var t = TargetSelector.GetTarget(_w.Range, DamageType.Magical);
                if (t.IsValidTarget(_w.Range)) _w.CastIfHitchanceEquals(t, HitChance.High, true);
            }

            if (_r.IsReady() && useR)
            {
                var t = TargetSelector.GetTarget(_r.Range, DamageType.Physical);
                if (t.IsInvulnerable) return;
                if (_q.IsReady() && t.IsValidTarget(_q.Range)
                    && (_q.GetDamage(t) > t.Health || _player.GetAutoAttackDamage(t, true) > t.Health))
                    return;
                if (_r.GetDamage(t) - 80 > t.Health && t.IsValidTarget(_r.Range))
                {
                    _r.CastIfHitchanceEquals(t, HitChance.High, true);
                }

                if (autoR)
                {
                    var fuckr = _r.GetPrediction(t, true);
                    if (fuckr.AoeTargetsHitCount >= getSliderItem(comboMenu, "MinTargets")
                        && t.IsValidTarget(_r.Range))
                        _r.CastIfHitchanceEquals(t, HitChance.High, true);
                }
            }

            UseItemes();
        }

        private static void Orbwalking_AfterAttack(AttackableUnit target, EventArgs args)
        {
            var mana = _player.ManaPercent > getSliderItem(harassMenu, "Harrasmana");
            if (target.IsMe)
                if (target.Type == GameObjectType.AIHeroClient)
                {
                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)
                        || (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) && mana))
                    {
                        var useQ = getCheckBoxItem(comboMenu, "UseQC") || getCheckBoxItem(harassMenu, "UseQH");
                        var useW = getCheckBoxItem(comboMenu, "UseWC") || getCheckBoxItem(harassMenu, "UseWH");
                        if (useQ && _q.IsReady())
                        {
                            var t = TargetSelector.GetTarget(_q.Range, DamageType.Physical);
                            if (t.IsValidTarget(_q.Range - 70)) _q.CastIfHitchanceEquals(t, HitChance.High, true);
                        }

                        if (useW && _w.IsReady())
                        {
                            var t = TargetSelector.GetTarget(_w.Range, DamageType.Magical);
                            if (t.IsValidTarget(_w.Range)) _w.CastIfHitchanceEquals(t, HitChance.High, true);
                        }
                    }

                    var useE = getCheckBoxItem(comboMenu, "UseEC");
                    var ta = TargetSelector.GetTarget(700, DamageType.Magical);
                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && _e.IsReady())
                    {
                        if (ObjectManager.Player.Position.Extend(Game.CursorPos, 700).CountEnemiesInRange(700) <= 1
                            && useE)
                        {
                            if (!ta.UnderTurret()) _e.Cast(ObjectManager.Player.Position.Extend(Game.CursorPos, 450));
                            else if (ta.UnderTurret() && _e.IsReady() && ta.IsValidTarget()
                                     && _q.ManaCost + _e.ManaCost < _player.Mana)
                                if (ta.Health < _q.GetDamage(ta) && ta.IsValidTarget())
                                {
                                    _e.Cast(ObjectManager.Player.Position.Extend(Game.CursorPos, 450));
                                    _q.CastIfHitchanceEquals(ta, HitChance.High, true);
                                }
                        }

                        var useEreload = getCheckBoxItem(comboMenu, "UseEreload");;
                        if (!_player.HasBuff("GravesBasicAttackAmmo2") && _e.IsReady() && useEreload)
                        {
                            var direction = (Game.CursorPos - _player.ServerPosition).To2D().Normalized();
                            for (var step = 0f; step < 360; step += 30)
                            {
                                for (var a = 450; a > 0; a -= 50)
                                {
                                    var currentAngle = step * (float)Math.PI / 90;
                                    var currentCheckPoint = _player.ServerPosition.To2D()
                                                            + a * direction.Rotated(currentAngle);
                                    if (currentCheckPoint.To3D().UnderTurret(true) || currentCheckPoint.To3D().IsWall())
                                    {
                                        return;
                                    }

                                    _e.Cast((Vector3)currentCheckPoint);
                                }
                            }
                        }
                    }
                }
        }

        private static void Harass()
        {
            var useQ = getCheckBoxItem(harassMenu, "UseQH");
            var useW = getCheckBoxItem(harassMenu, "UseWH");

            if (useQ && _q.IsReady())
            {
                var t = TargetSelector.GetTarget(_q.Range, DamageType.Physical);
                if (t.IsValidTarget(_q.Range - 70)) _q.CastIfHitchanceEquals(t, HitChance.High, true);
            }

            if (useW && _w.IsReady())
            {
                var t = TargetSelector.GetTarget(_w.Range, DamageType.Magical);
                if (t.IsValidTarget(_w.Range)) _w.CastIfHitchanceEquals(t, HitChance.High, true);
            }
        }

        private static void Laneclear()
        {

            var allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _q.Range, MinionTypes.All);
            var allMinionsW = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition,
                _w.Range + _w.Width / 2,
                MinionTypes.All);
            var rangedMinionsW = MinionManager.GetMinions(
                ObjectManager.Player.ServerPosition,
                _w.Range + _w.Width + 50,
                MinionTypes.Ranged);
            var minionhitq = getSliderItem(clearMenu, "minminions");
            var minionhitw = getSliderItem(clearMenu, "minminionsw");
            var useQl = getCheckBoxItem(clearMenu, "UseQL");
            var useWl = getCheckBoxItem(clearMenu, "UseWL");


            if (_q.IsReady() && useQl)
            {
                var fl2 = _q.GetCircularFarmLocation(allMinionsQ, 200);

                if (fl2.MinionsHit >= minionhitq)
                {
                    _q.Cast(fl2.Position);
                }
                else
                {
                    foreach (var minion in allMinionsQ)
                        if (Orbwalking.InAutoAttackRange(minion)
                            && minion.Health < 0.75 * _player.GetSpellDamage(minion, SpellSlot.Q))
                        {
                            _q.Cast(minion);
                        }
                        else if (!Orbwalking.InAutoAttackRange(minion)
                                 && minion.Health < 0.75 * _player.GetSpellDamage(minion, SpellSlot.Q))
                        {
                            _q.Cast(minion);
                        }
                }
            }

            if (_w.IsReady() && useWl)
            {
                var fl1 = _w.GetCircularFarmLocation(rangedMinionsW, _w.Width);

                if (fl1.MinionsHit >= minionhitw)
                {
                    _w.Cast(fl1.Position);
                }
                else
                    foreach (var minion in allMinionsW)
                        if (!Orbwalking.InAutoAttackRange(minion)
                            && minion.Health < 0.75 * _player.GetSpellDamage(minion, SpellSlot.W))
                            _w.Cast(minion);
            }
        }


        private static void LastHit()
        {
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _q.Range, MinionTypes.All);
            var useQ = getCheckBoxItem(lasthitMenu, "UseQLH");
            var useW = getCheckBoxItem(lasthitMenu, "UseWLH");
            if (allMinions.Count < 3) return;
            foreach (var minion in allMinions)
            {
                if (useQ && _q.IsReady() && minion.Health < 0.75 * _player.GetSpellDamage(minion, SpellSlot.Q))
                {
                    _q.Cast(minion);
                }

                if (_w.IsReady() && useW && minion.Health < 0.75 * _player.GetSpellDamage(minion, SpellSlot.W))
                {
                    _w.Cast(minion);
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
            var useQ = getCheckBoxItem(jungleMenu, "UseQJ");
            var useW = getCheckBoxItem(jungleMenu, "UseWJ");
            if (mobs.Count > 0)
            {
                var mob = mobs[0];
                if (useQ && _q.IsReady())
                {
                    _q.Cast(mob);
                }
                if (_w.IsReady() && useW)
                {
                    _w.Cast(mob);
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

        private static void KillSteal()
        {
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy))
            {
                if (_q.IsReady() && getCheckBoxItem(miscMenu, "UseQM"))
                {
                    if (_q.GetDamage(hero) > hero.Health && hero.IsValidTarget(_q.Range - 30))
                    {
                        _q.CastIfHitchanceEquals(hero, HitChance.High, true);
                    }
                }
                if (_w.IsReady() && getCheckBoxItem(miscMenu, "UseWM"))
                {
                    if (_w.GetDamage(hero) > hero.Health && hero.IsValidTarget(_w.Range))
                    {
                        _w.CastIfHitchanceEquals(hero, HitChance.High, true);
                    }
                }
                if (_r.IsReady() && getCheckBoxItem(miscMenu, "UseRM") && hero.IsValidTarget(_r.Range))
                {
                    if (_q.IsReady() && _q.GetDamage(hero) > hero.Health && hero.IsValidTarget(_q.Range)) return;
                    if (!hero.IsInvulnerable && _r.GetDamage(hero) - 80 > hero.Health)
                    {
                        _r.CastIfHitchanceEquals(hero, HitChance.High, true);
                    }
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            var harass = (getKeyBindItem(harassMenu, "harasstoggle"));
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
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _q.Range, System.Drawing.Color.GreenYellow);
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