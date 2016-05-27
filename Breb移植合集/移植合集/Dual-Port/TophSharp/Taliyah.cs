using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using Spell = LeagueSharp.Common.Spell;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;

namespace TophSharp
{
    internal class Taliyah 
    {
        private static Spell _q;
        private static Spell _w;
        private static Spell _e;
        public static SpellSlot Ignite;
        public static AIHeroClient Player { get { return ObjectManager.Player; } }
        private static readonly DateTime AssemblyLoadTime = DateTime.Now;

        public static void OnLoad()
        {
            if (Player.ChampionName != "Taliyah") return;

            Ignite = Player.GetSpellSlot("SummonerDot");
            // thanks to Shine for spell values!

            _q = new Spell(SpellSlot.Q, 900f);
            _q.SetSkillshot(0.5f, 60f, _q.Instance.SData.MissileSpeed, true, SkillshotType.SkillshotLine);

            _w = new Spell(SpellSlot.W, 800f);
            _w.SetSkillshot(0.8f, 50f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            _e = new Spell(SpellSlot.E, 700f);
            _e.SetSkillshot(0.25f, 150f, 2000f, false, SkillshotType.SkillshotLine);

            Game.OnUpdate += OnGameUpdate;
            CustomEvents.Unit.OnDash += OnDash;
            AntiGapcloser.OnEnemyGapcloser += OnGapClose;
            Interrupter2.OnInterruptableTarget += OnInterrupt;
            Drawing.OnDraw += OnDraw;
            Spellbook.OnCastSpell += OnCastSpell;
            MenuConfig.MenuLoaded();
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

        private static void OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            var usee = getCheckBoxItem(MenuConfig.comboMenu, "usee");
            var target = TargetSelector.GetTarget(_q.Range, DamageType.Magical);
            if (!target.IsValidTarget())
                return;

            if (CanUse(_e, target) && (CanUse(_w, target) || SpellUpSoon(SpellSlot.W) < 0.5f) && usee)
            {

                if (args.Slot == SpellSlot.E)
                {
                    EJustUsed = Environment.TickCount;
                }

            }
        }

        private static void OnDraw(EventArgs args)
        {
            var drawq = getCheckBoxItem(MenuConfig.drawMenu, "drawq");
            var draww = getCheckBoxItem(MenuConfig.drawMenu, "draww");
            var drawe = getCheckBoxItem(MenuConfig.drawMenu, "drawe");

            if (_q.Level > 0 && drawq)
            {
                var color = _q.IsReady() ? Color.CadetBlue : Color.Red;
                Render.Circle.DrawCircle(Player.Position, _q.Range, color, 3);
            }

            if (_w.Level > 0 && draww)
            {
                var color = _w.IsReady() ? Color.Green : Color.Red;
                Render.Circle.DrawCircle(Player.Position, _w.Range, color, 3);
            }

            if (_e.Level > 0 && drawe)
            {
                var color = _e.IsReady() ? Color.DarkOrchid : Color.Red;
                Render.Circle.DrawCircle(Player.Position, _e.Range, color, 3);
            }
        }

        private static void OnInterrupt(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
                if (sender.IsValid && CanUse(_w, sender))
                {

                    _w.Cast(sender);
                }
        }

        private static void OnGapClose(ActiveGapcloser gapcloser)
        {
            if (gapcloser.End.Distance(Player.Position) <= _w.Range && CanUse(_w, gapcloser.Sender))
            {
                _w.Cast(gapcloser.End);
            }
        }

        private static void OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            if (Player.Distance(args.EndPos) <= _w.Range && CanUse(_w, args.Unit))
            {
                _w.Cast(args.Unit);
            }
        }

        private static void OnGameUpdate(EventArgs args)
        {

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Mixed();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                LaneClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                LastHit();
            }          

            if (getKeyBindItem(MenuConfig.ahMenu, "onofftoggle"))
            {
                Mixed();
            }
        }

        private static void LastHit()
        {
            var minions = MinionManager.GetMinions(Player.Position, _q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth);

            var mana = getSliderItem(MenuConfig.lasthitMenu, "minmanal");
            var useqlasthit = getCheckBoxItem(MenuConfig.lasthitMenu, "qlasthit");
            var usewlasthit = getCheckBoxItem(MenuConfig.lasthitMenu, "wlasthit");

            if (Player.ManaPercent < mana)
                return;

            foreach (var minion in minions)
            {
                if (minion.Health <= _q.GetDamage(minion) && useqlasthit && _q.IsReady())
                {
                    _q.Cast(minion);
                }
                if (minion.Health <= _w.GetDamage(minion) && usewlasthit && _w.IsReady() && minion.Distance(Player) <= _w.Range)
                {
                    _w.Cast(minion);
                }
            }
        }

        private static void LaneClear()
        {
            var minions = MinionManager.GetMinions(Player.Position, _q.Range, MinionTypes.All, MinionTeam.Enemy,
       MinionOrderTypes.MaxHealth);

            var mana = getSliderItem(MenuConfig.clearMenu, "minmana");
            var useqlasthit = getCheckBoxItem(MenuConfig.clearMenu, "qlasthitlane");
            var usewlasthit = getCheckBoxItem(MenuConfig.clearMenu, "wlasthitlane");
            var qlaneclear = getCheckBoxItem(MenuConfig.clearMenu, "qlaneclear");
            var wlaneclear = getCheckBoxItem(MenuConfig.clearMenu, "wlaneclear");
            var minminionsw = getSliderItem(MenuConfig.clearMenu, "wlaneclearmin");

            if (Player.ManaPercent < mana)
                return;

            foreach (var minion in minions)
            {
                if (minion.Health <= _q.GetDamage(minion) && useqlasthit && _q.IsReady())
                {
                    _q.Cast(minion);
                }
                if (minion.Health <= _w.GetDamage(minion) && usewlasthit && _w.IsReady() && minion.Distance(Player) <= _w.Range)
                {
                    _w.Cast(minion);
                }

                if (qlaneclear && _q.IsReady())
                {
                    _q.Cast(minion);
                }
            }



            var circularposition = _w.GetCircularFarmLocation(minions);
            if (qlaneclear && circularposition.MinionsHit >= minminionsw && _w.IsReady())
            {
                _w.Cast(circularposition.Position);
            }
        }

        private static float IgniteDamage(Obj_AI_Base target)
        {
            if (Ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(Ignite) != SpellState.Ready)
                return 0f;
            return (float)Player.GetSummonerSpellDamage(target, LeagueSharp.Common.Damage.SummonerSpell.Ignite);
        }

        private static bool CanUse(Spell spell, AttackableUnit target)
        {
            return spell.IsReady() && Player.Mana >= spell.ManaCost && target.IsValidTarget(spell.Range);
        }

        public static float SpellUpSoon(SpellSlot slot)
        {
            var expires = (Player.Spellbook.GetSpell(slot).CooldownExpires);
            var cd =
                (float)
                    (expires -
                     (Game.Time - 1));

            return cd;
        }

        private static void Mixed()
        {
            var useq = getCheckBoxItem(MenuConfig.harassMenu, "useqh");
            var usew = getCheckBoxItem(MenuConfig.harassMenu, "usewh");

            var target = TargetSelector.GetTarget(_q.Range, DamageType.Magical);
            if (!target.IsValidTarget())
                return;

            var wpred = _w.GetPrediction(target);

            if (CanUse(_q, target) && useq)
            {
                _q.Cast(target);
            }

            if (CanUse(_w, target) && usew && wpred.Hitchance >= HitChance.High)
            {
                _w.Cast(wpred.CastPosition);
            }

        }

        private static void Combo()
        {
            var useq = getCheckBoxItem(MenuConfig.comboMenu, "useq");
            var usew = getCheckBoxItem(MenuConfig.comboMenu, "usew");
            var usee = getCheckBoxItem(MenuConfig.comboMenu, "usee");


            var target = TargetSelector.GetTarget(_q.Range, DamageType.Magical);
            if (!target.IsValidTarget()) return;

            if (getCheckBoxItem(MenuConfig.comboMenu, "useignite"))
            {
                if (_q.IsReady() && Ignite.IsReady() && (target.Health <= _q.GetDamage(target) + IgniteDamage(target)))
                {
                    Player.Spellbook.CastSpell(Ignite, target);
                }

                if (Ignite.IsReady() && (target.Health <= IgniteDamage(target) - 30))
                {
                    Player.Spellbook.CastSpell(Ignite, target);
                }
            }

            var wpred = _w.GetPrediction(target);
            var qpred = _q.GetPrediction(target);

            if (CanUse(_q, target) && useq && qpred.Hitchance > HitChance.High)
            {
                _q.Cast(qpred.CastPosition);
            }

            if ((CanUse(_w, target) || SpellUpSoon(SpellSlot.W) < 0.5f) && usee && _e.IsReady() && target.IsValidTarget(_q.Range))
            {         
                      
                _e.Cast(target);              
            }

            if (Environment.TickCount - EJustUsed < 2500 && Environment.TickCount - EJustUsed > 500 &&
                CanUse(_w, target) && !CanUse(_e, target) && usew && wpred.Hitchance >= HitChance.VeryHigh) 
            {
                _w.Cast(wpred.CastPosition);
            }

            if (!CanUse(_e, target) && CanUse(_w, target) && SpellUpSoon(SpellSlot.E) > 1f && usew &&
                wpred.Hitchance >= HitChance.VeryHigh)
            {
                if (CanUse(_e, target) && (CanUse(_w, target) || SpellUpSoon(SpellSlot.W) < 0.5f) && usee) return;
                _w.Cast(wpred.CastPosition);
            }

            if (SpellUpSoon(SpellSlot.W) < 0.9f && CanUse(_e, target) && usee)
            {
                
                _e.Cast(target);
            }

            if (SpellUpSoon(SpellSlot.W) > 2f && CanUse(_e, target) && usee)
            {
                _e.Cast(target);
            }
        }

        public static int EJustUsed
        { get; set; }
    }
}
