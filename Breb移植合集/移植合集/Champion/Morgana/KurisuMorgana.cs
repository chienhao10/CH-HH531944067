using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using Damage = LeagueSharp.Common.Damage;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

namespace KurisuMorgana
{
    internal class KurisuMorgana
    {
        private static Menu _menu;
        private static Spell _q, _w, _e, _r;
        private static readonly AIHeroClient Me = ObjectManager.Player;

        public static Menu menuQ, menuW, menuE, shieldMenu, menuR, wwmenu, miscMenu;

        private static float _mq, _mw, _mr;
        private static float _ma, _mi, _guise;

        public KurisuMorgana()
        {
            if (Me.ChampionName != "Morgana")
                return;

            // set spells
            _q = new Spell(SpellSlot.Q, 1175f);
            _q.SetSkillshot(0.25f, 72f, 1200f, true, SkillshotType.SkillshotLine);

            _w = new Spell(SpellSlot.W, 900f);
            _w.SetSkillshot(0.50f, 225f, 2200f, false, SkillshotType.SkillshotCircle);

            _e = new Spell(SpellSlot.E, 750f);
            _r = new Spell(SpellSlot.R, 600f);

            _menu = MainMenu.AddMenu("Kurisu's Morgana", "morgana");

            menuQ = _menu.AddSubMenu("Dark Binding [Q]", "asdfasdf");
            menuQ.Add("hitchanceq", new Slider("Binding Hitchance", 3, 1, 4));
            menuQ.Add("useqcombo", new CheckBox("Use in Combo"));
            menuQ.Add("useharassq", new CheckBox("Use in Harass", false));
            menuQ.Add("useqanti", new CheckBox("Use on Gapcloser"));
            menuQ.Add("useqauto", new CheckBox("Use on Immobile"));
            menuQ.Add("useqdash", new CheckBox("Use on Dashing"));
            menuQ.Add("autoqaa", new CheckBox("Use on Enemy Cast"));

            menuW = _menu.AddSubMenu("Tormented Soil [W]", "wmeasdfasdfasdfnu");
            menuW.Add("hitchancew", new Slider("Tormentsoil Hitchance ", 3, 1, 4));
            menuW.Add("calcw", new Slider("Calculated Ticks", 6, 3, 10));
            menuW.Add("usewcombo", new CheckBox("Use in Combo"));
            menuW.Add("useharassw", new CheckBox("Use in Harass", false));
            menuW.Add("usewauto", new CheckBox("Use on Immobile"));
            menuW.Add("waitfor", new CheckBox("Cast only on if Immobile"));

            menuE = _menu.AddSubMenu("BlackShield [E]", "emasdfasdfasdfenu");
            menuE.Add("shieldtg", new CheckBox("Shield Only Target Spells", false));
            menuE.Add("usemorge", new CheckBox("Enabled"));

            shieldMenu = _menu.AddSubMenu("Use Shield [Who?]", "usefor");
            foreach (var frn in ObjectManager.Get<AIHeroClient>().Where(x => x.Team == Me.Team))
            {
                shieldMenu.Add("useon" + frn.ChampionName, new CheckBox("Shield " + frn.ChampionName, !frn.IsMe));
            }
            shieldMenu.AddSeparator();
            shieldMenu.AddGroupLabel("Enemy Shield :");
            shieldMenu.AddLabel("Shield these skills.");
            shieldMenu.AddSeparator();
            foreach (var ene in ObjectManager.Get<AIHeroClient>().Where(x => x.Team != Me.Team))
            {
                shieldMenu.AddGroupLabel(ene.ChampionName);

                foreach (var lib in KurisuLib.CCList.Where(x => x.HeroName == ene.ChampionName))
                {
                    shieldMenu.AddLabel(lib.Slot + " - " + lib.SpellMenuName);
                    shieldMenu.Add(lib.SDataName + "on", new CheckBox("Enabled"));
                    shieldMenu.AddSeparator();
                }
            }

            menuR = _menu.AddSubMenu("Soul Shackles [R]", "rasdfasdfmenu");
            menuR.Add("rkill", new CheckBox("Use in combo if killable"));
            menuR.Add("rcount", new Slider("Use in combo if enemies >= ", 3, 1, 5));
            menuR.Add("useautor", new Slider("Use automatic if enemies >= ", 4, 2, 5));
            menuR.Add("usercombo", new CheckBox("Enabled"));

            wwmenu = _menu.AddSubMenu(":: Farm Settings", "wwmasdfasdfenu");
            wwmenu.Add("farmw", new CheckBox("Use W"));
            wwmenu.Add("farmcount", new Slider("-> If Min Minions >=", 3, 1, 7));

            miscMenu = _menu.AddSubMenu(":: Misc Settings", "miasdfasdfsc");
            miscMenu.Add("harassmana", new Slider("Harass mana %", 55, 0, 99));
            miscMenu.Add("support", new CheckBox(":: Support Mode", false));
            miscMenu.Add("dp", new CheckBox(":: Drawings"));

            // events
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Orbwalker.OnPreAttack += Orbwalker_OnPreAttack;

            try
            {
                Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            }

            catch (Exception e)
            {
                Console.WriteLine("Exception thrown KurisuMorgana: (BlackShield: {0})", e);
            }
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

        private void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (getCheckBoxItem(miscMenu, "support"))
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) ||
                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
                {
                    var minion = args.Target as Obj_AI_Base;
                    if (minion != null && minion.IsMinion && minion.IsValidTarget())
                    {
                        if (HeroManager.Allies.Any(x => x.IsValidTarget(1000) && !x.IsMe))
                        {
                            args.Process = false;
                        }
                    }
                }
            }
        }

        private static bool Immobile(AIHeroClient unit)
        {
            return unit.HasBuffOfType(BuffType.Charm) || unit.HasBuffOfType(BuffType.Knockup) ||
                   unit.HasBuffOfType(BuffType.Snare) ||
                   unit.HasBuffOfType(BuffType.Taunt) || unit.HasBuffOfType(BuffType.Suppression);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (!Me.IsValidTarget(300))
            {
                return;
            }

            CheckDamage(TargetSelector.GetTarget(_r.Range + 10, DamageType.Magical));

            AutoCast(getCheckBoxItem(menuQ, "useqdash"), getCheckBoxItem(menuQ, "useqauto"),
                getCheckBoxItem(menuW, "usewauto"));

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo(getCheckBoxItem(menuQ, "useqcombo"), getCheckBoxItem(menuW, "usewcombo"),
                    getCheckBoxItem(menuR, "usercombo"));
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass(getCheckBoxItem(menuQ, "useharassq"), getCheckBoxItem(menuW, "useharassw"));
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                if (getCheckBoxItem(wwmenu, "farmw") && _w.IsReady())
                {
                    var minionpositions = MinionManager.GetMinions(_w.Range).Select(x => x.Position.To2D());
                    var location = MinionManager.GetBestCircularFarmLocation(minionpositions.ToList(), _w.Width,
                        _w.Range);
                    if (location.MinionsHit >= getSliderItem(wwmenu, "farmcount"))
                    {
                        _w.Cast(location.Position);
                    }
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(miscMenu, "dp"))
            {
                Render.Circle.DrawCircle(Me.Position, _q.Range,
                    Color.FromArgb(155, Color.DeepPink), 4);
            }
        }

        private static void Combo(bool useq, bool usew, bool user)
        {
            if (useq && _q.IsReady())
            {
                var qtarget = TargetSelector.GetTarget(_q.Range, DamageType.Magical);
                if (qtarget.IsValidTarget())
                {
                    var poutput = _q.GetPrediction(qtarget);
                    if (poutput.Hitchance >= (HitChance) getSliderItem(menuQ, "hitchanceq") + 2)
                    {
                        _q.Cast(poutput.CastPosition);
                    }
                }
            }

            if (usew && _w.IsReady())
            {
                var wtarget = TargetSelector.GetTarget(_w.Range + 10, DamageType.Magical);
                if (wtarget.IsValidTarget())
                {
                    if (!getCheckBoxItem(menuW, "waitfor") || _mw*1 >= wtarget.Health)
                    {
                        var poutput = _w.GetPrediction(wtarget);
                        if (poutput.Hitchance >= (HitChance) getSliderItem(menuW, "hitchancew") + 2)
                        {
                            _w.Cast(poutput.CastPosition);
                        }
                    }
                }
            }

            if (user && _r.IsReady())
            {
                var ticks = getSliderItem(menuW, "calcw");

                var rtarget = TargetSelector.GetTarget(_r.Range, DamageType.Magical);
                if (rtarget.IsValidTarget() && getCheckBoxItem(menuR, "rkill"))
                {
                    if (_mr + _mq + _mw*ticks + _ma*3 + _mi + _guise >= rtarget.Health)
                    {
                        if (rtarget.Health > _mr + _ma*2 + _mw*2 && !rtarget.IsZombie)
                        {
                            if (_e.IsReady()) _e.CastOnUnit(Me);
                            _r.Cast();
                        }
                    }

                    if (Me.CountEnemiesInRange(_r.Range) >= getSliderItem(menuR, "rcount"))
                    {
                        if (_e.IsReady())
                            _e.CastOnUnit(Me);

                        _r.Cast();
                    }
                }
            }
        }

        private static void Harass(bool useq, bool usew)
        {
            if (useq && _q.IsReady())
            {
                var qtarget = TargetSelector.GetTarget(_q.Range, DamageType.Magical);
                if (qtarget.IsValidTarget())
                {
                    if (Me.ManaPercent >= getSliderItem(miscMenu, "harassmana"))
                    {
                        var poutput = _q.GetPrediction(qtarget);
                        if (poutput.Hitchance >= (HitChance) getSliderItem(menuQ, "hitchanceq") + 2)
                        {
                            _q.Cast(poutput.CastPosition);
                        }
                    }
                }
            }

            if (usew && _w.IsReady())
            {
                var wtarget = TargetSelector.GetTarget(_w.Range + 200, DamageType.Magical);
                if (wtarget.IsValidTarget())
                {
                    if (Me.ManaPercent >= getSliderItem(miscMenu, "harassmana"))
                    {
                        if (!getCheckBoxItem(menuW, "waitfor") || _mw*1 >= wtarget.Health)
                        {
                            var poutput = _w.GetPrediction(wtarget);
                            if (poutput.Hitchance >= (HitChance) getSliderItem(menuW, "hitchancew") + 2)
                            {
                                _w.Cast(poutput.CastPosition);
                            }
                        }
                    }
                }
            }
        }

        private static void AutoCast(bool dashing, bool immobile, bool soil)
        {
            if (_q.IsReady())
            {
                foreach (var itarget in HeroManager.Enemies.Where(h => h.IsValidTarget(_q.Range)))
                {
                    if (immobile && Immobile(itarget))
                        _q.Cast(itarget);

                    if (immobile)
                        _q.CastIfHitchanceEquals(itarget, HitChance.Immobile);

                    if (dashing && itarget.Distance(Me.ServerPosition) <= 400f)
                        _q.CastIfHitchanceEquals(itarget, HitChance.Dashing);
                }
            }

            if (_w.IsReady() && soil)
            {
                foreach (var itarget in HeroManager.Enemies.Where(h => h.IsValidTarget(_w.Range)))
                    if (immobile && Immobile(itarget))
                        _w.Cast(itarget.ServerPosition);
            }

            if (_r.IsReady())
            {
                if (Me.CountEnemiesInRange(_r.Range) >= getSliderItem(menuR, "useautor"))
                {
                    if (_e.IsReady())
                        _e.CastOnUnit(Me);

                    _r.Cast();
                }
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsValidTarget(250f))
            {
                if (getCheckBoxItem(menuQ, "useqanti"))
                    _q.Cast(gapcloser.Sender);
            }
        }

        private static void CheckDamage(Obj_AI_Base target)
        {
            if (target == null)
            {
                return;
            }

            var qready = Me.Spellbook.CanUseSpell(SpellSlot.Q) == SpellState.Ready;
            var wready = Me.Spellbook.CanUseSpell(SpellSlot.W) == SpellState.Ready;
            var rready = Me.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready;
            var iready = Me.Spellbook.CanUseSpell(Me.GetSpellSlot("summonerdot")) == SpellState.Ready;

            _ma = Me.GetAutoAttackDamage(target);
            _mq = qready ? Me.GetSpellDamage(target, SpellSlot.Q) : 0;
            _mw = wready ? Me.GetSpellDamage(target, SpellSlot.W) : 0;
            _mr = rready ? Me.GetSpellDamage(target, SpellSlot.R) : 0;
            _mi = (float) (iready ? Me.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) : 0);

            _guise = (float) (Items.HasItem(3151)
                ? Me.GetItemDamage(target, Damage.DamageItems.LiandrysTorment)
                : 0);
        }

        internal static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsEnemy && sender.Type == GameObjectType.AIHeroClient && _q.IsReady())
            {
                if (args.End.IsValid() && args.End.Distance(Me.ServerPosition) <= 200 + Me.BoundingRadius)
                {
                    var hero = sender as AIHeroClient;
                    if (!hero.IsValid<AIHeroClient>() || !hero.IsValidTarget(_q.Range - 50))
                    {
                        return;
                    }

                    if (getCheckBoxItem(menuQ, "autoqaa"))
                    {
                        _q.CastIfHitchanceEquals(hero, HitChance.VeryHigh);
                    }
                }
            }

            if (sender.Type != Me.Type || !_e.IsReady() || !sender.IsEnemy || !getCheckBoxItem(menuE, "usemorge"))
                return;

            var attacker = ObjectManager.Get<AIHeroClient>().First(x => x.NetworkId == sender.NetworkId);
            foreach (var ally in HeroManager.Allies.Where(x => x.IsValidTarget(_e.Range)))
            {
                var detectRange = ally.ServerPosition +
                                  (args.End - ally.ServerPosition).Normalized()*ally.Distance(args.End);
                if (detectRange.Distance(ally.ServerPosition) > ally.AttackRange - ally.BoundingRadius)
                    continue;

                foreach (
                    var lib in
                        KurisuLib.CCList.Where(
                            x => x.HeroName == attacker.ChampionName && x.Slot == attacker.GetSpellSlot(args.SData.Name))
                    )
                {
                    if (lib.Type == Skilltype.Unit && args.Target.NetworkId != ally.NetworkId)
                        return;

                    if (getCheckBoxItem(menuE, "shieldtg") && lib.Type != Skilltype.Unit)
                        return;

                    if (getCheckBoxItem(shieldMenu, lib.SDataName + "on") &&
                        getCheckBoxItem(shieldMenu, "useon" + ally.ChampionName))
                    {
                        Utility.DelayAction.Add(lib.Slot != SpellSlot.R ? 100 : 0, () => _e.CastOnUnit(ally));
                    }
                }
            }
        }
    }
}