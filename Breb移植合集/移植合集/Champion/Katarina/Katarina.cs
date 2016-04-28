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

namespace Staberina
{
    internal class Katarina
    {
        private const int RRange = 550;
        private static bool CastWAfterE;
        private static int LastWardPlacement;
        private static bool WardJumping;
        private static readonly Random Random = new Random(Utils.TickCount);
        private static AIHeroClient KSTarget;
        private static bool CancellingUlt;
        private static int LastStealthedUlt;
        public static LeagueSharp.Common.Spell Q, W, E, R;

        public static Menu menu, qMenu, wMenu, eMenu, rMenu, ksMenu, farmMenu, fleeMenu, drawMenu, miscMenu;

        public Katarina()
        {
            Q = SpellManager.Q;
            W = SpellManager.W;
            E = SpellManager.E;
            R = SpellManager.R;

            menu = MainMenu.AddMenu("Staberina", "Staberina");

            qMenu = menu.AddSubMenu("Q", "Q");
            qMenu.Add("QCombo", new CheckBox("Use in Combo"));
            qMenu.Add("QHarass", new CheckBox("Use in Harass"));

            wMenu = menu.AddSubMenu("W", "W");
            wMenu.Add("WCombo", new CheckBox("Use in Combo"));
            wMenu.Add("WHarass", new CheckBox("Use in Harass"));
            wMenu.Add("WAuto", new CheckBox("Auto W", false));

            eMenu = menu.AddSubMenu("E", "E");
            eMenu.Add("ECombo", new CheckBox("Use in Combo"));
            eMenu.Add("EHarass", new CheckBox("Use in Harass"));
            eMenu.Add("ETurret", new CheckBox("Block E Under Turret", false));
            eMenu.Add("EEnemies", new Slider("Max Enemies to E", 5, 1, 5));

            rMenu = menu.AddSubMenu("R", "R");
            rMenu.Add("RInCombo", new CheckBox("Always R in Combo", false));
            rMenu.Add("RCombo", new CheckBox("Smart R"));
            rMenu.Add("RUltTicks", new Slider("Smart R Ticks", 7, 1, 10));
            rMenu.Add("RRangeDecrease", new Slider("Decrease Spell Range", 30, 0, 30));
            R.Range = RRange - rMenu["RRangeDecrease"].Cast<Slider>().CurrentValue;
            rMenu.Add("RMovement", new CheckBox("Disable Movement while casting R"));
            rMenu.Add("RCancelNoEnemies", new CheckBox("Cancel R if no enemies", false));
            rMenu.Add("RCancelUlt", new KeyBind("Cancel R Key", false, KeyBind.BindTypes.HoldActive, 'J'));
            rMenu.Add("RStealth", new CheckBox("R Stealthed Units", false));

            ksMenu = menu.AddSubMenu("Killsteal", "Killsteal");
            ksMenu.Add("KSEnabled", new CheckBox("Use Smart KS"));
            ksMenu.Add("KSQ", new CheckBox("Use Q"));
            ksMenu.Add("KSW", new CheckBox("Use W"));
            ksMenu.Add("KSE", new CheckBox("Use E"));
            ksMenu.Add("KSR", new CheckBox("Use Smart R"));
            ksMenu.Add("KSRCancel", new CheckBox("Cancel R to KS"));
            ksMenu.Add("KSEnemies", new Slider("Max Enemies", 5, 1, 5));
            ksMenu.Add("KSHealth", new Slider("Min Health", 10));
            ksMenu.Add("KSGapclose", new CheckBox("Gapclose with E", false));
            ksMenu.Add("KSWardJump", new CheckBox("Ward Jump", false));
            ksMenu.Add("KSTurret", new CheckBox("Block E Under Turret"));

            farmMenu = menu.AddSubMenu("Farm", "Farm");
            farmMenu.AddGroupLabel("Q");
            farmMenu.Add("QFarm", new CheckBox("Use in Farm"));
            farmMenu.Add("QLastHit", new CheckBox("Only Last Hit (Only Killable)"));
            farmMenu.AddSeparator();
            farmMenu.AddGroupLabel("W");
            farmMenu.Add("WFarm", new CheckBox("Use in Farm"));
            farmMenu.Add("WMinionsHit", new Slider("Min Minions Killed", 2, 1, 4));
            farmMenu.AddSeparator();
            farmMenu.AddGroupLabel("E");
            farmMenu.Add("EFarm", new CheckBox("Use E->W in Farm", false));
            farmMenu.Add("EMinionsHit", new Slider("Min Minions Killed", 3, 1, 4));
            farmMenu.AddSeparator();
            farmMenu.Add("FarmEnabled", new KeyBind("Farm Enabled", true, KeyBind.BindTypes.PressToggle, 'J'));

            fleeMenu = menu.AddSubMenu("Flee", "Flee");
            fleeMenu.Add("FleeEnabled", new KeyBind("Flee Enabled", false, KeyBind.BindTypes.HoldActive, 'T'));
            fleeMenu.Add("FleeE", new CheckBox("Use E"));
            fleeMenu.Add("FleeWard", new CheckBox("Use Wards to Flee"));

            drawMenu = menu.AddSubMenu("Drawing", "Drawing");
            drawMenu.Add("0Draw", new CheckBox("Draw Q"));
            drawMenu.Add("1Draw", new CheckBox("Draw W"));
            drawMenu.Add("2Draw", new CheckBox("Draw E"));
            drawMenu.Add("3Draw", new CheckBox("Draw R"));

            drawMenu.Add("DmgEnabled", new CheckBox("Draw Damage Indicator"));
            drawMenu.Add("HPColor", new CheckBox("Predicted Health Color"));
            drawMenu.Add("FillColor", new CheckBox("Damage Color"));
            drawMenu.Add("Killable", new CheckBox("Killable Text"));

            miscMenu = menu.AddSubMenu("Misc");
            miscMenu.Add("ComboMode", new ComboBox("Combo Mode", 0, "E->Q->W", "Q->E->W"));
            miscMenu.Add("ComboKillable", new KeyBind("Only Combo Killable", false, KeyBind.BindTypes.PressToggle, 'K'));
            SpellManager.Initialize(menu);

            EloBuddy.Player.OnIssueOrder += Obj_AI_Base_OnIssueOrder;
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        public static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public static int UltTicks
        {
            get { return getSliderItem(rMenu, "RUltTicks"); }
        }

        public static List<AIHeroClient> Enemies
        {
            get { return HeroManager.Enemies; }
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

        private void Obj_AI_Base_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (getCheckBoxItem(rMenu, "RMovement") && sender.IsMe && ObjectManager.Player.IsChannelingImportantSpell())
            {
                args.Process = false;
            }
        }

        private void Game_OnUpdate(EventArgs args)
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

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                OnFarm();
            }
        }

        public static void OnCombo()
        {
            Combo();
        }

        private void OnUpdate()
        {
            if (Player.IsDead)
            {
                return;
            }

            if (getCheckBoxItem(rMenu, "RStealth") && R.IsReady() && Player.CountEnemiesInRange(RRange) == 0 && R.Cast())
            {
                LastStealthedUlt = Utils.TickCount;
                return;
            }

            var c = Player.IsChannelingImportantSpell();

            if (c)
            {
                if (getKeyBindItem(rMenu, "RCancelUlt") && Utility.MoveRandomly())
                {
                    return;
                }

                if (getCheckBoxItem(rMenu, "RCancelNoEnemies") && Player.CountEnemiesInRange(RRange) == 0 &&
                    !CancellingUlt && Utils.TickCount - LastStealthedUlt > 2500)
                {
                    CancellingUlt = true;
                    LeagueSharp.Common.Utility.DelayAction.Add(
                        300, () =>
                        {
                            CancellingUlt = false;
                            if (Player.CountEnemiesInRange(RRange) == 0 && Utility.MoveRandomly())
                            {
                            }
                        });
                }
            }

            if (WardJumping)
            {
                if (Utils.TickCount - LastWardPlacement < Game.Ping + 100 || E.LastCastedDelay(200))
                {
                    return;
                }

                if (!E.IsReady())
                {
                    WardJumping = false;
                    return;
                }

                var ward =
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(o => E.IsInRange(o) && MinionManager.IsWard(o) && o.Buffs.Any(b => b.Caster.IsMe))
                        .OrderBy(o => o.Distance(Game.CursorPos))
                        .ThenByDescending(o => o.DistanceToPlayer())
                        .FirstOrDefault();

                if (ward == null)
                {
                    WardJumping = false;
                    return;
                }

                // stop movement to prevent turning around after e
                if (EloBuddy.Player.IssueOrder(GameObjectOrder.Stop, Player.ServerPosition))
                {
                    E.CastOnUnit(ward);
                    Console.WriteLine("WARD JUMP");
                    return;
                }
            }

            if (Flee())
            {
                return;
            }

            if (AutoKill())
            {
                return;
            }

            if (c)
            {
                return;
            }

            if (getCheckBoxItem(wMenu, "WAuto") && W.IsReady() && Enemies.Any(h => h.IsValidTarget(W.Range)) && W.Cast())
            {
                Console.WriteLine("AUTO W");
            }
        }

        private static bool Combo(AIHeroClient forcedTarget = null)
        {
            //var mode = Orbwalker.ActiveModesFlags;
            var comboMode = getBoxItem(miscMenu, "ComboMode");
            var d = comboMode == 0 ? E.Range : Q.Range;
            var forceTarget = forcedTarget.IsValidTarget();
            var target = forceTarget ? forcedTarget : TargetSelector.GetTarget(d, DamageType.Magical);

            if (!target.IsValidTarget())
            {
                return false;
            }

            var q = Q.CanCast(target) && Q.IsActive(forceTarget);
            var w = W.CanCast(target) && W.IsActive(forceTarget);
            var e = E.CanCast(target) && E.IsActive(forceTarget) &&
                    target.CountEnemiesInRange(200) <= getSliderItem(eMenu, "EEnemies");

            if (!forceTarget && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                getKeyBindItem(miscMenu, "ComboKillable"))
            {
                var damage = target.GetComboDamage(q, w, e, Utility.IsRReady(), true);
                if (target.Health > damage)
                {
                    return false;
                }
            }

            var delay = (int) (100 + Game.Ping/2f + Random.Next(150));
            if (Q.LastCastedDelay(delay) || E.LastCastedDelay(delay) || R.LastCastedDelay(delay))
            {
                return false;
            }

            if (comboMode == 0 && q && e && CastE(target, forceTarget))
            {
                return true;
            }

            if (q && Q.CastOnUnit(target))
            {
                return true;
            }

            if (e && CastE(target, forceTarget))
            {
                return true;
            }

            if (w && W.Cast())
            {
                return true;
            }

            if (Utility.IsRReady() && (forceTarget || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)))
            {
                if (!forceTarget && getCheckBoxItem(rMenu, "RInCombo") && target.IsValidTarget(R.Range) && R.Cast())
                {
                    return true;
                }

                if (!forceTarget && !getCheckBoxItem(rMenu, "RCombo"))
                {
                    return false;
                }

                var enemy =
                    Enemies.FirstOrDefault(h => h.IsValidTarget(R.Range) && h.GetCalculatedRDamage(UltTicks) > h.Health);
                if (enemy != null && R.Cast())
                {
                    return true;
                }
            }
            return false;
        }

        private static bool CastE(Obj_AI_Base target, bool skipCheck = false)
        {
            return (skipCheck || !getCheckBoxItem(eMenu, "ETurret") || !target.UnderTurret(true)) &&
                   E.CastOnUnit(target);
        }

        private static bool AutoKill()
        {
            if (!getCheckBoxItem(ksMenu, "KSEnabled"))
            {
                return false;
            }

            var minHealth = getSliderItem(ksMenu, "KSHealth");
            var channeling = Player.IsChannelingImportantSpell();

            if (channeling && !getCheckBoxItem(ksMenu, "KSRCancel"))
            {
                return false;
            }

            var delay = (int) (150 + Game.Ping/2f + Random.Next(150));
            if (Q.LastCastedDelay(delay) || E.LastCastedDelay(delay) || R.LastCastedDelay(delay))
            {
                return false;
            }

            if (KSTarget != null && !KSTarget.IsValidTarget(E.Range))
            {
                KSTarget = null;
            }

            foreach (
                var enemy in
                    Enemies.Where(
                        h =>
                            h.IsValidTarget(E.Range + Q.Range) && !h.IsZombie &&
                            (KSTarget == null || KSTarget.NetworkId == h.NetworkId)).OrderBy(h => h.Health))
            {
                if (E.IsInRange(enemy))
                {
                    if (W.IsCastable(enemy, true) && W.Cast())
                    {
                        KSTarget = enemy;
                        return true;
                    }

                    if (Q.IsCastable(enemy, true) && Q.CastOnUnit(enemy))
                    {
                        KSTarget = enemy;
                        return true;
                    }

                    if (Q.IsCastable(enemy, true, false) && W.IsCastable(enemy, true, false) &&
                        enemy.GetComboDamage(Q, W) > enemy.Health && Q.CastOnUnit(enemy))
                    {
                        KSTarget = enemy;
                        return true;
                    }

                    if (Player.HealthPercent < minHealth)
                    {
                        continue;
                    }

                    if (E.IsCastable(enemy, true) && E.CastOnUnit(enemy))
                    {
                        KSTarget = enemy;
                        return true;
                    }

                    if (enemy.GetKSDamage() > enemy.Health && Combo(enemy))
                    {
                        KSTarget = enemy;
                        return true;
                    }

                    continue;
                }

                // doing some gapclosers and hops here
                if (!E.IsActive(true) || !E.IsReady())
                {
                    continue;
                }

                var closestTarget = Utility.GetClosestETarget(enemy);
                if (getCheckBoxItem(ksMenu, "KSGapclose") && closestTarget != null)
                {
                    var gapcloseDmg = enemy.GetGapcloseDamage(closestTarget);
                    if (enemy.Health < gapcloseDmg &&
                        enemy.CountEnemiesInRange(300) <= getSliderItem(ksMenu, "KSEnemies") &&
                        (!getCheckBoxItem(ksMenu, "KSTurret") || !closestTarget.UnderTurret(true)) &&
                        E.CastOnUnit(closestTarget))
                    {
                        return true;
                    }
                }
                if (!getCheckBoxItem(ksMenu, "KSWardJump"))
                {
                    continue;
                }

                var wardSlot = Utility.GetReadyWard();

                if (wardSlot.Equals(SpellSlot.Unknown) || !LastWardPlacement.HasTimePassed(2000))
                {
                    continue;
                }

                var range = Player.Spellbook.GetSpell(wardSlot).SData.CastRange;

                if (!enemy.IsValidTarget(Q.Range + range))
                {
                    continue;
                }

                var pos = Player.ServerPosition.LSExtend(enemy.ServerPosition, range);

                if (getCheckBoxItem(ksMenu, "KSTurret") && pos.UnderTurret(true))
                {
                    continue;
                }

                if (pos.CountEnemiesInRange(300) - 1 > 2)
                {
                    continue;
                }

                if (enemy.Health < enemy.GetGapcloseDamage(pos) && Player.Spellbook.CastSpell(wardSlot, pos))
                {
                    LastWardPlacement = Utils.TickCount;
                    WardJumping = true;
                    return true;
                }
            }
            return false;
        }

        private static bool Flee()
        {
            if (!getKeyBindItem(fleeMenu, "FleeEnabled"))
            {
                return false;
            }

            Orbwalker.ActiveModesFlags = Orbwalker.ActiveModes.None;

            if (getCheckBoxItem(fleeMenu, "FleeE") && E.IsReady())
            {
                var closestTarget = Utility.GetClosestETarget(Game.CursorPos);

                if (closestTarget != null && closestTarget.Distance(Game.CursorPos) < 200)
                {
                    return E.CastOnUnit(closestTarget);
                }

                var ward = Utility.GetReadyWard();
                if (getCheckBoxItem(fleeMenu, "FleeWard") && ward != SpellSlot.Unknown &&
                    LastWardPlacement.HasTimePassed(2000))
                {
                    var range = Player.Spellbook.GetSpell(ward).SData.CastRange;
                    if (Player.Spellbook.CastSpell(ward, Player.ServerPosition.LSExtend(Game.CursorPos, range)))
                    {
                        Console.WriteLine("PLACE WARD");
                        LastWardPlacement = Utils.TickCount;
                        WardJumping = true;
                        return true;
                    }
                }
            }

            if (!Player.IsDashing() && Player.GetWaypoints().Last().Distance(Game.CursorPos) > 100)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Player.ServerPosition.LSExtend(Game.CursorPos, 250),
                    false);
            }

            return false;
        }


        public static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            foreach (var spell in new[] {"0", "1", "2", "3"})
            {
                var c = Color.Black;
                var radi = 0.0f;
                if (spell == "0")
                {
                    c = Color.Purple;
                    radi = Q.Range;
                }
                if (spell == "1")
                {
                    c = Color.DeepPink;
                    radi = W.Range;
                }
                if (spell == "2")
                {
                    c = Color.DeepPink;
                    radi = E.Range;
                }
                if (spell == "3")
                {
                    c = Color.White;
                    radi = R.Range;
                }
                var circle = getCheckBoxItem(drawMenu, spell + "Draw");
                if (circle)
                {
                    Render.Circle.DrawCircle(Player.Position, radi, c);
                }
            }
        }

        public static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender == null || !sender.IsValid || !sender.IsMe)
            {
                return;
            }

            if (args.Slot.Equals(SpellSlot.E) && CastWAfterE && W.IsReady())
            {
                CastWAfterE = false;
                W.Cast();
            }
        }

        public static void OnFarm()
        {
            if (!getKeyBindItem(farmMenu, "FarmEnabled"))
            {
                return;
            }

            var minions = MinionManager.GetMinions(E.Range);

            if (getCheckBoxItem(farmMenu, "WFarm") && W.IsReady())
            {
                var wKillableMinions = minions.Count(m => m.IsValidTarget(W.Range) && W.IsKillable(m));
                if (wKillableMinions < getSliderItem(farmMenu, "WMinionsHit"))
                {
                    if (getCheckBoxItem(farmMenu, "EFarm") && E.IsReady()) // e->w
                    {
                        foreach (var target in from target in Utility.GetETargets()
                            let killableMinions =
                                MinionManager.GetMinions(target.ServerPosition, W.Range + E.Range)
                                    .Count(m => W.IsKillable(m))
                            where killableMinions >= getSliderItem(farmMenu, "EMinionsHit")
                            select target)
                        {
                            CastWAfterE = true;
                            if (E.CastOnUnit(target))
                            {
                                return;
                            }
                        }
                    }
                }
                else if (W.Cast())
                {
                    return;
                }
            }

            if (!getCheckBoxItem(farmMenu, "QFarm") || !Q.IsReady())
            {
                return;
            }

            var qKillableMinion = minions.FirstOrDefault(m => m.IsValidTarget(Q.Range) && Q.IsKillable(m));
            var qMinion = minions.Where(m => m.IsValidTarget(Q.Range)).MinOrDefault(m => m.Health);

            if (qKillableMinion == null)
            {
                if (getCheckBoxItem(farmMenu, "QLastHit") || qMinion == null)
                {
                    return;
                }

                Q.CastOnUnit(qMinion);
                return;
            }

            Q.CastOnUnit(qKillableMinion);
        }
    }
}