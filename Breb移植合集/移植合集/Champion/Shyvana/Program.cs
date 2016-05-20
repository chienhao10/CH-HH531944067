#region

using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

#endregion

namespace D_Shyvana
{
    internal class Program
    {
        private const string ChampionName = "Shyvana";

        private static Spell _q, _w, _e, _r;

        private static Menu _config;

        private static AIHeroClient _player;

        public static Menu comboMenu, harassMenu, clearMenu, forestMenu, miscMenu, drawMenu;

        public static void Game_OnGameLoad()
        {
            _player = ObjectManager.Player;
            if (ObjectManager.Player.ChampionName != ChampionName) return;

            _q = new Spell(SpellSlot.Q, 0);
            _w = new Spell(SpellSlot.W, 350f);
            _e = new Spell(SpellSlot.E, 925f);
            _r = new Spell(SpellSlot.R, 1000f);

            _e.SetSkillshot(0.25f, 60f, 1700, false, SkillshotType.SkillshotLine);
            _r.SetSkillshot(0.25f, 150f, 1500, false, SkillshotType.SkillshotLine);

            //D Shyvana
            _config = MainMenu.AddMenu("D-Shyvana", "D-Shyvana");

            //Combo
            comboMenu = _config.AddSubMenu("Combo", "Combo");
            comboMenu.Add("UseQC", new CheckBox("Use Q"));
            comboMenu.Add("UseWC", new CheckBox("Use W"));
            comboMenu.Add("UseEC", new CheckBox("Use E"));
            comboMenu.Add("UseRC", new CheckBox("Use R"));
            comboMenu.Add("UseRE", new CheckBox("AutoR Min Targ"));
            comboMenu.Add("MinTargets", new Slider("Ult when>=min enemy(COMBO)", 2, 1, 5));

            //Harass
            harassMenu = _config.AddSubMenu("Harass", "Harass");
            harassMenu.Add("UseQH", new CheckBox("Use Q"));
            harassMenu.Add("UseWH", new CheckBox("Use W"));
            harassMenu.Add("UseEH", new CheckBox("Use E"));
            harassMenu.Add("harasstoggle", new KeyBind("AutoHarass (toggle)", false, KeyBind.BindTypes.PressToggle, 'G'));

            //LaneClear
            clearMenu = _config.AddSubMenu("Farm", "Farm");
            clearMenu.AddGroupLabel("LastHit");
            clearMenu.Add("UseQLH", new CheckBox("Q LastHit"));
            clearMenu.Add("UseWLH", new CheckBox("W LastHit"));
            clearMenu.Add("UseELH", new CheckBox("E LastHit"));

            clearMenu.AddGroupLabel("LaneClear");
            clearMenu.Add("UseQL", new CheckBox("Q LaneClear"));
            clearMenu.Add("UseWL", new CheckBox("W LaneClear"));
            clearMenu.Add("UseEL", new CheckBox("E LaneClear"));

            clearMenu.AddGroupLabel("JungleClear");
            clearMenu.Add("UseQJ", new CheckBox("Q Jungle"));
            clearMenu.Add("UseWJ", new CheckBox("W Jungle"));
            clearMenu.Add("UseEJ", new CheckBox("E Jungle"));

            //Forest
            forestMenu = _config.AddSubMenu("Forest Gump", "Forest Gump");
            forestMenu.Add("UseWF", new CheckBox("Use W "));
            forestMenu.Add("UseEF", new CheckBox("Use E "));
            forestMenu.Add("UseRF", new CheckBox("Use R "));
            forestMenu.Add("Forest", new KeyBind("Active Forest Gump!", false, KeyBind.BindTypes.HoldActive, 'Z'));

            //Misc
            miscMenu = _config.AddSubMenu("Misc", "Misc");
            miscMenu.Add("UseEM", new CheckBox("Use E KillSteal"));
            miscMenu.Add("UseRM", new CheckBox("Use R KillSteal"));
            miscMenu.Add("Gap_E", new CheckBox("R GapClosers"));
            miscMenu.Add("UseRInt", new CheckBox("R to Interrupt"));
            miscMenu.Add("Echange", new ComboBox("E Hit", 3, "Low", "Medium", "High", "Very High"));
            miscMenu.Add("Rchange", new ComboBox("R Hit", 3, "Low", "Medium", "High", "Very High"));

            //Drawings
            drawMenu = _config.AddSubMenu("Drawings", "Drawings");
            drawMenu.Add("DrawQ", new CheckBox("Draw Q", false));
            drawMenu.Add("DrawW", new CheckBox("Draw W", false));
            drawMenu.Add("DrawE", new CheckBox("Draw E", false));
            drawMenu.Add("DrawR", new CheckBox("Draw R", false));
            drawMenu.Add("Drawharass", new CheckBox("Draw AutoHarass"));

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter.OnPossibleToInterrupt += Interrupter_OnPosibleToInterrupt;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) ||
                 getKeyBindItem(harassMenu, "harasstoggle")))
            {
                Harass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Laneclear();
                JungleClear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                LastHit();
            }

            if (getKeyBindItem(forestMenu, "Forest"))
            {
                Forest();
            }

            _player = ObjectManager.Player;
            Orbwalker.DisableAttacking = false;
            KillSteal();
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

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }
            var spell = args.SData;
            if (spell.Name.ToLower().Contains("shyvanadoubleattack"))
            {
                Utility.DelayAction.Add(450, Orbwalker.ResetAutoAttack);
            }
        }

        private static float ComboDamage(Obj_AI_Base enemy)
        {
            var damage = 0d;
            if (_q.IsReady())
                damage += _player.GetSpellDamage(enemy, SpellSlot.Q)*1.2;
            if (_q.IsReady())
                damage += _player.GetSpellDamage(enemy, SpellSlot.W)*3;
            if (_e.IsReady())
                damage += _player.GetSpellDamage(enemy, SpellSlot.E);
            if (_r.IsReady())
                damage += _player.GetSpellDamage(enemy, SpellSlot.R);
            damage += _player.GetAutoAttackDamage(enemy, true)*2;
            return (float) damage;
        }

        private static void Combo()
        {
            var useQ = getCheckBoxItem(comboMenu, "UseQC");
            var useW = getCheckBoxItem(comboMenu, "UseWC");
            var useE = getCheckBoxItem(comboMenu, "UseEC");
            var useR = getCheckBoxItem(comboMenu, "UseRC");
            var autoR = getCheckBoxItem(comboMenu, "UseRE");

            var t = TargetSelector.GetTarget(_r.Range, DamageType.Magical);

            if (useR && _r.IsReady())
            {
                if (t != null && _r.GetPrediction(t).Hitchance >= Rchange())
                    if (!t.HasBuff("JudicatorIntervention") && !t.HasBuff("Undying Rage") &&
                        ComboDamage(t) > t.Health)
                        _r.CastIfHitchanceEquals(t, HitChance.Medium);
            }
            if (useW && _w.IsReady())
            {
                if (t != null && _player.LSDistance(t) < _e.Range)
                    _w.Cast();
            }

            if (useE && _e.IsReady())
            {
                if (t != null && _player.LSDistance(t) < _e.Range &&
                    _e.GetPrediction(t).Hitchance >= Echange())
                    _e.Cast(t);
            }

            if (useQ && _q.IsReady())
            {
                if (t != null && _player.LSDistance(t) < _w.Range)
                    _q.Cast();
            }

            if (_r.IsReady() && autoR)
            {
                if (ObjectManager.Get<AIHeroClient>().Count(hero => hero.IsValidTarget(_r.Range)) >=
                    getSliderItem(comboMenu, "MinTargets")
                    && _r.GetPrediction(t).Hitchance >= Rchange())
                    _r.Cast(t);
            }
        }

        private static void Harass()
        {
            var useQ = getCheckBoxItem(harassMenu, "UseQH");
            var useW = getCheckBoxItem(harassMenu, "UseWH");
            var useE = getCheckBoxItem(harassMenu, "UseEH");

            if (useQ && _q.IsReady())
            {
                var t = TargetSelector.GetTarget(_w.Range, DamageType.Magical);
                if (t != null && t.LSDistance(_player.Position) < _w.Range)
                    _q.Cast();
            }
            if (useW && _w.IsReady())
            {
                var t = TargetSelector.GetTarget(_w.Range, DamageType.Magical);
                if (t != null && _player.LSDistance(t) < _w.Range)
                    _w.Cast();
            }
            if (useE && _e.IsReady())
            {
                var t = TargetSelector.GetTarget(_e.Range, DamageType.Magical);
                if (t != null && _player.LSDistance(t) < _e.Range && _e.GetPrediction(t).Hitchance >= Echange())
                    _e.Cast(t);
            }
        }

        private static void Laneclear()
        {
            var allMinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _w.Range);
            var rangedMinionsE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _e.Range + _e.Width,
                MinionTypes.Ranged);
            var allMinionsE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _e.Range + _e.Width);
            var useQl = getCheckBoxItem(clearMenu, "UseQL");
            var useWl = getCheckBoxItem(clearMenu, "UseWL");
            var useEl = getCheckBoxItem(clearMenu, "UseEL");
            if (_q.IsReady() && useQl && allMinionsW.Count > 0)
            {
                _q.Cast();
            }

            if (_w.IsReady() && useWl)
            {
                if (allMinionsW.Count >= 2)
                {
                    _w.Cast();
                }
                else
                    foreach (var minion in allMinionsW)
                        if (!Orbwalking.InAutoAttackRange(minion) &&
                            minion.Health < 0.75*_player.GetSpellDamage(minion, SpellSlot.W))
                            _w.Cast();
            }
            if (_e.IsReady() && useEl)
            {
                var fl1 = _e.GetLineFarmLocation(rangedMinionsE, _e.Width);
                var fl2 = _e.GetLineFarmLocation(allMinionsE, _e.Width);

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
                        if (!Orbwalking.InAutoAttackRange(minion) &&
                            minion.Health < 0.75*_player.GetSpellDamage(minion, SpellSlot.E))
                            _e.Cast(minion);
            }
        }

        private static void LastHit()
        {
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, _e.Range);
            var useQ = getCheckBoxItem(clearMenu, "UseQLH");
            var useW = getCheckBoxItem(clearMenu, "UseWLH");
            var useE = getCheckBoxItem(clearMenu, "UseELH");
            foreach (var minion in allMinions)
            {
                if (useQ && _q.IsReady() && _player.LSDistance(minion) < 200 &&
                    minion.Health < 0.75*_player.GetSpellDamage(minion, SpellSlot.Q))
                {
                    _q.Cast();
                }

                if (_w.IsReady() && useW && _player.LSDistance(minion) < _w.Range &&
                    minion.Health < 0.75*_player.GetSpellDamage(minion, SpellSlot.W))
                {
                    _w.Cast();
                }
                if (_e.IsReady() && useE && _player.LSDistance(minion) < _e.Range &&
                    minion.Health < 0.75*_player.GetSpellDamage(minion, SpellSlot.E))
                {
                    _e.Cast(minion);
                }
            }
        }

        private static void JungleClear()
        {
            var mobs = MinionManager.GetMinions(_player.ServerPosition, _w.Range,
                MinionTypes.All,
                MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            var useQ = getCheckBoxItem(clearMenu, "UseQJ");
            var useW = getCheckBoxItem(clearMenu, "UseWJ");
            var useE = getCheckBoxItem(clearMenu, "UseEJ");
            if (mobs.Count > 0)
            {
                var mob = mobs[0];
                if (useQ && _q.IsReady() && !mob.Name.Contains("Mini"))
                {
                    _q.Cast();
                }
                if (_w.IsReady() && useW)
                {
                    _w.Cast();
                }
                if (_e.IsReady() && useE)
                {
                    _e.Cast(mob);
                }
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

        private static HitChance Rchange()
        {
            switch (getBoxItem(miscMenu, "Rchange"))
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

        private static void KillSteal()
        {
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy))
            {
                if (_e.IsReady() && getCheckBoxItem(miscMenu, "UseEM"))
                {
                    if (_e.GetDamage(hero) > hero.Health && hero.IsValidTarget(_e.Range))
                    {
                        _e.CastIfHitchanceEquals(hero, Echange());
                    }
                }
                if (_r.IsReady() && getCheckBoxItem(miscMenu, "UseRM"))
                {
                    var t = TargetSelector.GetTarget(_r.Range, DamageType.Magical);
                    if (t != null)
                        if (!t.HasBuff("JudicatorIntervention") && !t.HasBuff("Undying Rage") &&
                            _r.GetDamage(t) > t.Health && _r.GetPrediction(t).Hitchance >= Rchange())
                            _r.Cast(t);
                }
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (_r.IsReady() && gapcloser.Sender.IsValidTarget(_r.Range) && getCheckBoxItem(miscMenu, "Gap_E"))
                if (!gapcloser.End.UnderTurret(true) && gapcloser.End.CountEnemiesInRange(700) < 1)
                {
                    _r.Cast(gapcloser.End);
                }
        }

        private static void Interrupter_OnPosibleToInterrupt(Obj_AI_Base target, InterruptableSpell spell)
        {
            if (!getCheckBoxItem(miscMenu, "UseRInt")) return;
            if (target.IsValidTarget(_r.Range) && spell.DangerLevel == InterruptableDangerLevel.High)
            {
                _r.Cast(target);
            }
        }

        private static void Forest()
        {
            var target = TargetSelector.GetTarget(_r.Range, DamageType.Magical);
            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            if (getCheckBoxItem(forestMenu, "UseRF") && _r.IsReady() && target != null)
            {
                _r.Cast(Game.CursorPos);
            }
            if (getCheckBoxItem(forestMenu, "UseWF") && _w.IsReady() && target != null)
            {
                _w.Cast();
            }
            if (getCheckBoxItem(forestMenu, "UseEF") && _e.IsReady() && target != null)
            {
                _e.Cast(target);
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            var harass = getKeyBindItem(harassMenu, "harasstoggle");

            if (getCheckBoxItem(drawMenu, "Drawharass"))
            {
                if (harass)
                {
                    Drawing.DrawText(Drawing.Width*0.02f, Drawing.Height*0.92f, Color.GreenYellow, "Auto harass Enabled");
                }
                else
                {
                    Drawing.DrawText(Drawing.Width*0.02f, Drawing.Height*0.92f, Color.OrangeRed, "Auto harass Disabled");
                }
            }

            if (getCheckBoxItem(drawMenu, "DrawQ"))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _q.Range, Color.GreenYellow);
            }
            if (getCheckBoxItem(drawMenu, "DrawW"))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _w.Range, Color.GreenYellow);
            }
            if (getCheckBoxItem(drawMenu, "DrawE"))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _e.Range, Color.GreenYellow);
            }

            if (getCheckBoxItem(drawMenu, "DrawR"))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, _r.Range, Color.GreenYellow);
            }
        }
    }
}