using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SharpDX;
using TreeLib.Extensions;
using Color = System.Drawing.Color;
using Prediction = LeagueSharp.Common.Prediction;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

namespace PopBlanc
{
    internal static class Program
    {
        private const int ERange = 950;
        private static AIHeroClient KSTarget;
        public static Spell Q, W, E, R;
        private static readonly Random Random = new Random(Utils.TickCount);

        public static Menu menu, qMenu, wMenu, eMenu, rMenu, comboMenu, ksMenu, fleeMenu, drawMenu;

        public static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        private static int Delay
        {
            get { return (int) (200 + Game.Ping/2f + Random.Next(100)); }
        }

        private static int WCastTime
        {
            get { return (int) (1000*W.Delay + Game.Ping/2f); }
        }

        private static float WRadius
        {
            get { return W.Instance.SData.CastRadius; }
        }

        private static bool EFirst
        {
            get { return getCheckBoxItem(eMenu, "ComboEFirst"); }
        }

        private static bool AdditionalComboActive
        {
            get
            {
                return getKeyBindItem(comboMenu, "2Key") || getKeyBindItem(comboMenu, "AOECombo") ||
                       getKeyBindItem(fleeMenu, "flee");
            }
        }

        public static List<AIHeroClient> Enemies
        {
            get { return HeroManager.Enemies; }
        }

        public static void OnLoad()
        {
            Q = SpellManager.Q;
            W = SpellManager.W;
            E = SpellManager.E;
            R = SpellManager.R;

            menu = MainMenu.AddMenu("PopBlanc", "PopBlanc");

            qMenu = menu.AddSubMenu("Q");
            qMenu.Add("ComboQ", new CheckBox("Use in Combo"));
            qMenu.Add("HarassQ", new CheckBox("Use in Harass"));
            qMenu.Add("LastHitQ", new CheckBox("Use in Last Hit"));
            qMenu.Add("LaneClearQ", new CheckBox("Use in Lane Clear"));
            qMenu.Add("FarmQMana", new Slider("Farm Minimum Mana", 40));

            wMenu = menu.AddSubMenu("W");
            wMenu.Add("ComboW", new CheckBox("Use in Combo"));
            wMenu.Add("HarassW", new CheckBox("Use in Harass"));
            wMenu.Add("LaneClearW", new CheckBox("Use in Lane Clear"));
            wMenu.Add("FarmWMinions", new Slider("Farm Minimum Minions", 3, 1, 5));
            wMenu.Add("WBackHarass", new CheckBox("Harass W Back"));
            wMenu.Add("WBackFarm", new CheckBox("Farm W Back"));
            wMenu.Add("WBackClick", new CheckBox("Left Click W Back", false));

            eMenu = menu.AddSubMenu("E");
            eMenu.Add("ComboE", new CheckBox("Use in Combo"));
            eMenu.Add("HarassE", new CheckBox("Use in Harass"));
            eMenu.Add("ERangeDecrease", new Slider("Decrease Range"));
            E.Range = ERange - getSliderItem(eMenu, "ERangeDecrease");
            eMenu.Add("ComboEFirst", new CheckBox("Combo E First", false));
            eMenu.Add("AntiGapcloser", new CheckBox("AntiGapCloser with E"));
            eMenu.Add("AutoEImmobile", new CheckBox("Auto E Immobile Targets"));

            rMenu = menu.AddSubMenu("R");
            rMenu.Add("ComboR", new CheckBox("Use in Combo"));
            rMenu.Add("HarassR", new CheckBox("Use in Harass"));
            rMenu.Add("LaneClearR", new CheckBox("Use in LaneClear", false));
            rMenu.Add("AntiGapcloserR", new CheckBox("AntiGapCloser with R(E)", false));
            rMenu.Add("RBackClick", new CheckBox("Left Click R(W) Back", false));

            comboMenu = menu.AddSubMenu("Combo", "Other Combos");
            comboMenu.AddGroupLabel("2 Chainz (E > R(E))");
            comboMenu.Add("2Key", new KeyBind("Combo Key", false, KeyBind.BindTypes.HoldActive, 'H'));
            comboMenu.Add("2Selected", new CheckBox("Selected Target Only", false));
            comboMenu.Add("2W", new CheckBox("Use W if out of range"));
            comboMenu.AddSeparator();

            comboMenu.AddGroupLabel("AOECombo (W > R(W))");
            comboMenu.Add("AOECombo", new KeyBind("Combo Key", false, KeyBind.BindTypes.HoldActive, 'N'));
            comboMenu.Add("AOEW", new CheckBox("Use W"));
            comboMenu.Add("GapcloseW", new CheckBox("Use W to Gapclose"));
            comboMenu.Add("AOER", new CheckBox("Use R(W)"));
            comboMenu.Add("AOEEnemies", new Slider("Minimum Enemies", 2, 1, 5));
            comboMenu.AddSeparator();
            comboMenu.Add("ComboOrbwalk", new CheckBox("Orbwalk when Comboing"));

            ksMenu = menu.AddSubMenu("Killsteal");
            ksMenu.Add("SmartKS", new CheckBox("Smart Killsteal"));
            ksMenu.Add("KSMana", new Slider("Minimum Mana", 30));
            ksMenu.Add("KSHealth", new Slider("Minimum Health to W", 40));
            ksMenu.Add("KSGapclose", new CheckBox("Use W to Gapclose", false));
            ksMenu.Add("KSEnemies", new Slider("Maximum Enemies to W", 3, 1, 4));

            fleeMenu = menu.AddSubMenu("Flee", "Flee");
            fleeMenu.Add("Flee", new KeyBind("Flee", false, KeyBind.BindTypes.HoldActive, 'T'));
            fleeMenu.Add("FleeW", new CheckBox("Use W"));
            fleeMenu.Add("FleeRW", new CheckBox("Use R(W)"));
            fleeMenu.Add("FleeMove", new CheckBox("Move to Cursor Position"));

            drawMenu = menu.AddSubMenu("Drawings");
            drawMenu.Add("Draw0", new CheckBox("Draw Q Range"));
            drawMenu.Add("Draw1", new CheckBox("Draw W Range"));
            drawMenu.Add("Draw2", new CheckBox("Draw E Range"));
            drawMenu.Add("DrawCD", new CheckBox("Draw on CD"));
            drawMenu.Add("DrawWBack", new CheckBox("Draw W Back Position"));
            drawMenu.AddSeparator();

            SpellManager.Initialize(menu);
            WBackPosition.Initialize();

            Drawing.OnDraw += Drawing_OnDraw;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Game.OnWndProc += Game_OnWndProc;
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            OnUpdate();

            if (Player.IsDead)
            {
                return;
            }

            if (Player.IsDashing() || Player.IsChannelingImportantSpell())
            {
                return;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                OnCombo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                OnFarm();
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

        public static void OnUpdate()
        {
            SpellManager.UpdateUltimate();

            if (Player.IsDead)
            {
                return;
            }

            if (getCheckBoxItem(eMenu, "AutoEImmobile") && E.IsReady())
            {
                var target = Enemies.FirstOrDefault(e => e.IsValidTarget(E.Range) && e.IsMovementImpaired());
                if (target.IsValidTarget() && target != null)
                {
                    E.Cast(target);
                    return;
                }
            }

            if (getKeyBindItem(comboMenu, "AOECombo"))
            {
                ManualOrbwalk();
                if (AOECombo())
                {
                    return;
                }
            }

            if (getKeyBindItem(comboMenu, "2Key"))
            {
                ManualOrbwalk();
                if (_2Chainz())
                {
                    return;
                }
            }

            if (getKeyBindItem(fleeMenu, "Flee") ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee) && Flee())
            {
                return;
            }

            if (getCheckBoxItem(ksMenu, "SmartKS") && Player.ManaPercent >= getSliderItem(ksMenu, "KSMana") &&
                AutoKill())
            {
            }
        }

        private static void ManualOrbwalk()
        {
            if (getCheckBoxItem(comboMenu, "ComboOrbwalk"))
            {
                //Orbwalking.Orbwalk(Orbwalker.GetTarget(), Game.CursorPos);
                Orbwalker.OrbwalkTo(Game.CursorPos);
            }
        }

        public static void OnCombo()
        {
            if (AdditionalComboActive)
            {
                return;
            }

            Combo();
        }

        private static void Combo(AIHeroClient targ = null, bool force = false)
        {
            if (Q.LastCastedDelay(Delay) || R.LastCastedDelay(Delay))
            {
                return;
            }

            if (!force && CastSecondW())
            {
                return;
            }

            var target = targ ??
                         TargetSelector.GetTarget(
                             EFirst && E.IsReady() ? E.Range : W.Range + WRadius - 10, DamageType.Magical);

            if (!target.IsValidTarget())
            {
                //Console.WriteLine("BAD TARG");
                return;
            }

            if (CastEFirst(target))
            {
                Console.WriteLine("Combo: Cast E FIRST");
                return;
            }

            if (Q.CanCast(target) && Q.IsActive(force) && Q.CastOnUnit(target))
            {
                Console.WriteLine("Combo: Cast Q");
                return;
            }

            SpellManager.UpdateUltimate();
            if (R.CanCast(target) && R.IsActive(force) && R.GetSpellSlot() == SpellSlot.Q && R.CastOnUnit(target))
            {
                Console.WriteLine("Combo: Cast R(Q)");
                return;
            }

            if (W.IsReady() && target.IsValidTarget(W.Range + WRadius - 10) && W.IsActive(force) && W.IsFirstW())
            {
                if (!force ||
                    (target.CountEnemiesInRange(300) <= getSliderItem(ksMenu, "KSEnemies") &&
                     Player.HealthPercent >= getSliderItem(ksMenu, "KSHealth")))
                {
                    var pos = Prediction.GetPrediction(target, W.Delay, W.Range + WRadius, W.Speed);
                    if (pos.CastPosition.Distance(target.ServerPosition) < WRadius && W.Cast(pos.CastPosition))
                    {
                        Console.WriteLine("Combo: Cast W");
                        return;
                    }
                }
            }

            if (E.CanCast(target) && E.IsActive(force) && target.DistanceToPlayer() > 50 && E.Cast(target).IsCasted())
            {
                Console.WriteLine("Combo: Cast E");
            }
        }

        private static bool CastSecondW()
        {
            return getCheckBoxItem(wMenu, "WBackHarass") &&
                   Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) &&
                   W.IsReady() && !W.IsFirstW() && !Q.IsReady() && W.Cast();
        }

        public static float DistanceToPlayer(this GameObject obj)
        {
            var unit = obj as Obj_AI_Base;
            if (unit == null || !unit.IsValid)
            {
                return obj.Position.Distance(Player.ServerPosition);
            }

            return unit.ServerPosition.Distance(Player.ServerPosition);
        }

        public static float DistanceToPlayer(this Vector3 position)
        {
            return position.Distance(Player.ServerPosition);
        }

        public static float DistanceToPlayer(this Vector2 position)
        {
            return position.Distance(Player.ServerPosition);
        }

        private static bool CastEFirst(Obj_AI_Base target)
        {
            return EFirst && E.CanCast(target) && E.IsActive() && target.DistanceToPlayer() > 50 &&
                   E.Cast(target).IsCasted();
        }

        private static bool AOECombo()
        {
            if (Player.IsDashing())
            {
                return true;
            }

            var target = TargetSelector.GetTarget(W.Range + WRadius, DamageType.Magical);

            if (target != null &&
                target.CountEnemiesInRange(WRadius) >= getSliderItem(comboMenu, "AOEEnemies"))
            {
                if (W.IsReady() && getCheckBoxItem(comboMenu, "AOEW") && W.IsFirstW() &&
                    W.Cast(target, false, true).IsCasted())
                {
                    Console.WriteLine("AOE: Cast W");
                    return true;
                }

                if (R.IsReady() && getCheckBoxItem(comboMenu, "AOER") && R.GetSpellSlot() == SpellSlot.W && R.IsFirstW() &&
                    R.Cast(target, false, true).IsCasted())
                {
                    Console.WriteLine("AOE: Cast R(W)");
                    return true;
                }
            }

            if (!W.IsReady() || !W.IsFirstW() || !R.IsReady(WCastTime) || !getCheckBoxItem(comboMenu, "GapcloseW"))
            {
                return false;
            }

            target = TargetSelector.GetTarget(W.Range*2, DamageType.Magical);

            if (target == null || target.CountEnemiesInRange(WRadius) < getSliderItem(comboMenu, "AOEEnemies"))
            {
                return false;
            }

            var pos = Player.ServerPosition.LSExtend(target.ServerPosition, W.Range + 10);

            if (pos.IsValidWPoint() && W.Cast(pos))
            {
                Console.WriteLine("AOE: Cast Gapclose W");
                return true;
            }

            return false;
        }

        private static bool _2Chainz()
        {
            var chainable = TargetSelector.SelectedTarget ??
                            TargetSelector.GetTarget(E.Range, DamageType.Magical);

            if (chainable != null)
            {
                if (E.CanCast(chainable) && E.Cast(chainable).IsCasted())
                {
                    Console.WriteLine("2Chainz: Cast E");
                    return true;
                }

                if (R.CanCast(chainable) && R.IsReady() && R.GetSpellSlot() == SpellSlot.E &&
                    R.Cast(chainable).IsCasted())
                {
                    Console.WriteLine("2Chainz: Cast R(E)");
                    return true;
                }

                return false;
            }

            if (!getCheckBoxItem(comboMenu, "2W") || !W.IsReady() || !W.IsFirstW() || !E.IsReady())
            {
                return false;
            }

            chainable = TargetSelector.SelectedTarget ??
                        TargetSelector.GetTarget(W.Range + E.Range, DamageType.Magical);

            if (!chainable.IsValidTarget(W.Range + E.Range) || chainable.HasEBuff())
            {
                return false;
            }

            var pos = Player.ServerPosition.Extend(chainable.ServerPosition, W.Range + 10);
            Console.WriteLine("2Chainz: Cast Gapclose W");
            return W.Cast(pos);
        }

        public static void OnFarm()
        {
            if (Q.IsReady() && Q.IsActive() && Player.ManaPercent >= getSliderItem(qMenu, "FarmQMana"))
            {
                var killable =
                    MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.NotAlly)
                        .FirstOrDefault(m => Q.IsKillable(m));

                if (killable.IsValidTarget() && killable.Health > Player.GetAutoAttackDamage(killable, true) &&
                    Q.CastOnUnit(killable))
                {
                    return;
                }
            }

            if (!Q.IsReady() && W.IsReady() && !W.IsFirstW() && getCheckBoxItem(wMenu, "WBackFarm") && W.Cast())
            {
                return;
            }

            var wReady = W.IsReady() && W.IsActive() && W.IsFirstW();
            var rReady = R.IsReady() && R.IsActive() && R.GetSpellSlot() == SpellSlot.W && R.IsFirstW();

            if (!wReady && !rReady)
            {
                return;
            }

            var min = getSliderItem(wMenu, "FarmWMinions");
            var minions = MinionManager.GetMinions(W.Range, MinionTypes.All, MinionTeam.NotAlly);

            if (minions.Count < min)
            {
                return;
            }

            var pos = W.GetCircularFarmLocation(minions);

            if (pos.MinionsHit < min)
            {
                return;
            }

            if (wReady && W.Cast(pos.Position))
            {
                return;
            }

            if (rReady && R.Cast(pos.Position))
            {
            }
        }

        private static bool Flee()
        {
            if (Player.IsDashing())
            {
                return true;
            }

            Orbwalker.ActiveModesFlags = Orbwalker.ActiveModes.None;

            if (getCheckBoxItem(fleeMenu, "FleeW") && W.IsReady() && W.IsFirstW())
            {
                var pos = Player.ServerPosition.Extend(Game.CursorPos, W.Range);
                if (W.Cast(pos))
                {
                    return true;
                }
            }

            SpellManager.UpdateUltimate();
            if (getCheckBoxItem(fleeMenu, "FleeRW") && R.IsReady() && R.GetSpellSlot() == SpellSlot.W && R.IsFirstW())
            {
                var pos = Player.ServerPosition.Extend(Game.CursorPos, W.Range);
                if (R.Cast(pos))
                {
                    return true;
                }
            }

            if (!getCheckBoxItem(fleeMenu, "FleeMove") || Player.GetWaypoints().Last().Distance(Game.CursorPos) < 100)
            {
                return true;
            }

            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            return true;
        }

        private static bool AutoKill()
        {
            var wRange = W.IsReady() && W.IsFirstW() && getCheckBoxItem(ksMenu, "KSGapclose") ? W.Range : 0;
            var enemies =
                HeroManager.Enemies.Where(
                    enemy =>
                        enemy.IsValidTarget(wRange + E.Range) && !enemy.IsZombie &&
                        enemy.Health < GetComboDamage(enemy, SpellSlot.Q, WCastTime)).ToList();

            if (!enemies.Any())
            {
                return false;
            }

            if (!KSTarget.IsValidTarget(wRange + E.Range))
            {
                KSTarget = null;
            }

            KSTarget = enemies.MinOrDefault(e => e.Health);

            if (KSTarget == null || !KSTarget.IsValid)
            {
                return false;
            }

            var pos = Player.ServerPosition.LSExtend(KSTarget.ServerPosition, W.Range);

            if (!E.IsInRange(KSTarget) && KSTarget.IsValidTarget(E.Range + W.Range) &&
                KSTarget.Health < GetKSDamage(KSTarget, false, pos))
            {
                if (!pos.IsValidWPoint() || !W.IsFirstW() || !W.Cast(pos))
                {
                    return false;
                }

                Console.WriteLine("KS: Gapclose W");
                return true;
            }

            if (KSTarget.Health < GetKSDamage(KSTarget))
            {
                Combo(KSTarget, true);
                return true;
            }

            return false;
        }

        public static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!E.IsReady() || !getCheckBoxItem(eMenu, "AntiGapcloser") || gapcloser.Sender.IsMe)
            {
                return;
            }

            Utility.DelayAction.Add(
                150, () =>
                {
                    if (gapcloser.Sender.IsValidTarget(E.Range) && E.Cast(gapcloser.Sender).IsCasted() &&
                        getCheckBoxItem(rMenu, "AntiGapcloserR"))
                    {
                        Utility.DelayAction.Add(
                            (int) (200 + 1000*E.Delay + Game.Ping/2f), () =>
                            {
                                SpellManager.UpdateUltimate();
                                if (R.GetSpellSlot() == SpellSlot.E)
                                {
                                    R.Cast(gapcloser.Sender);
                                }
                            });
                    }
                });
        }

        public static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            foreach (var spell in new[] {"0", "1", "2"})
            {
                var radi = 0.0f;
                var c = Color.AliceBlue;
                if (spell == "0")
                {
                    radi = Q.Range;
                    c = Color.Red;
                }
                if (spell == "1")
                {
                    radi = W.Range;
                    c = Color.Red;
                }
                if (spell == "2")
                {
                    radi = E.Range;
                    c = Color.Purple;
                }

                var circle = getCheckBoxItem(drawMenu, "Draw" + spell);
                var slot = (SpellSlot) Convert.ToInt32(spell);

                if (circle && Player.Spellbook.GetSpell(slot).IsReady())
                {
                    Render.Circle.DrawCircle(Player.Position, radi, c);
                }
            }
        }

        public static void Game_OnWndProc(WndEventArgs args)
        {
            if ((!getCheckBoxItem(wMenu, "WBackClick") && !getCheckBoxItem(rMenu, "RBackClick")) || Player.IsDead ||
                args.Msg != 0x202)
            {
                return;
            }

            var wPos =
                WBackPosition.Positions.Where(p => p.Obj.Position.Distance(Game.CursorPos) < 2000)
                    .OrderBy(p => p.Obj.Position.Distance(Game.CursorPos));
            if (
                wPos.Select(w => w.IsR ? R : W)
                    .Any(
                        spell =>
                            getCheckBoxItem(spell.Slot == SpellSlot.W ? wMenu : rMenu, spell.Slot + "BackClick") &&
                            spell.IsReady() && !spell.IsFirstW() &&
                            spell.Cast()))
            {
            }
        }

        private static float GetKSDamage(Obj_AI_Base enemy, bool includeW = true, Vector3 position = default(Vector3))
        {
            var damage = 0d;

            if (Q.IsReady() && enemy.IsValidTarget(Q.Range, true, position))
            {
                var q = Q.GetDamage(enemy);
                damage += q;

                if (enemy.HasQBuff() || enemy.HasQRBuff())
                {
                    damage += q;
                }
            }

            if (includeW && W.IsReady() && W.IsFirstW() &&
                enemy.IsValidTarget(W.Range, true, position))
            {
                damage += W.GetDamage(enemy);
            }

            if (E.IsReady() && enemy.IsValidTarget(E.Range, true, position))
            {
                SpellManager.EPrediction.UpdateSourcePosition(position, position);
                var pred = SpellManager.EPrediction.GetPrediction(enemy);
                if (pred.Hitchance > HitChance.Medium)
                {
                    damage += E.GetDamage(enemy);
                }
            }

            if (R.IsReady() && enemy.IsValidTarget(R.Range, true, position))
            {
                var d = GetUltimateDamage(enemy, SpellSlot.Unknown);
                if (enemy.HasQBuff() || enemy.HasQRBuff())
                {
                    d += Q.GetDamage(enemy);
                }

                damage += d;
            }

            return (float) damage;
        }

        private static float GetComboDamage(Obj_AI_Base enemy, SpellSlot slot = SpellSlot.Unknown, int t = 0)
        {
            var damage = 0d;

            if (Q.IsReady(t))
            {
                // 2 for q mark
                var d = Q.GetDamage(enemy);
                if (enemy.HasQBuff() || enemy.HasQRBuff())
                {
                    d += Q.GetDamage(enemy);
                }
                damage += d;
            }

            if (W.IsReady(t) && W.IsFirstW())
            {
                damage += W.GetDamage(enemy);
            }

            if (E.IsReady(t))
            {
                damage += E.GetDamage(enemy);
            }

            if (R.IsReady(t))
            {
                var d = GetUltimateDamage(enemy, slot);
                if (enemy.HasQBuff() || enemy.HasQRBuff())
                {
                    d += Q.GetDamage(enemy);
                }

                damage += d;
            }

            damage += 2*Player.GetAutoAttackDamage(enemy, true);

            return (float) damage;
        }

        private static double GetUltimateDamage(Obj_AI_Base enemy, SpellSlot slot)
        {
            var d = 0d;
            var s = Player.Spellbook.GetSpell(SpellSlot.R);
            var level = s.Level;

            if (level < 1 || s.State == SpellState.NotLearned)
            {
                return 0d;
            }

            var maxDamage = new double[] {200, 400, 600}[level - 1] + 1.3f*Player.FlatMagicDamageMod;
            var spell = slot.Equals(SpellSlot.Unknown) ? R.GetSpellSlot() : slot;

            switch (spell)
            {
                case SpellSlot.Q:
                    var qDmg = Player.CalcDamage(
                        enemy, DamageType.Magical,
                        new double[] {100, 200, 300}[level - 1] + .65f*Player.FlatMagicDamageMod);
                    d = qDmg > maxDamage ? maxDamage : qDmg;
                    break;
                case SpellSlot.W:
                    d = Player.CalcDamage(
                        enemy, DamageType.Magical,
                        new double[] {150, 300, 450}[level - 1] + .975f*Player.FlatMagicDamageMod);
                    break;
                case SpellSlot.E:
                    var eDmg = Player.CalcDamage(
                        enemy, DamageType.Magical,
                        new double[] {100, 200, 300}[level - 1] + .65f*Player.FlatMagicDamageMod);
                    d = eDmg > maxDamage ? maxDamage : eDmg;
                    break;
            }
            return d;
        }
    }
}