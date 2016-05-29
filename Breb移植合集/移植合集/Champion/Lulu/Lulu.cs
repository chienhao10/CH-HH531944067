using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;
using EloBuddy;
using LeagueSharp.Common;
using System.Collections.Generic;
using System.Linq;
using System;
using TreeLib.Core;
using TreeLib.Core.Extensions;
using TreeLib.Managers;

namespace LuluLicious
{
    internal class Lulu : TreeLib.Objects.Champion
    {
        private const int RRadius = 350;
        private static int LastWCast;
        private static Obj_AI_Base QAfterETarget;

        private static readonly Dictionary<SpellSlot, int[]> ManaCostDictionary = new Dictionary<SpellSlot, int[]>
        {
            { SpellSlot.Q, new[] { 0, 60, 65, 70, 75, 80 } },
            { SpellSlot.W, new[] { 0, 65, 65, 65, 65, 65 } },
            { SpellSlot.E, new[] { 0, 60, 70, 80, 90, 100 } },
            { SpellSlot.R, new[] { 0, 0, 0, 0, 0, 0 } }
        };

        public static Menu pixMenu, qMenu, wMenu, eMenu, rMenu, ksMenu, fleeMenu, drawMenu, miscMenu, superMMenu;

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

        public Lulu()
        {
            Q = SpellManager.Q;
            W = SpellManager.W;
            E = SpellManager.E;
            R = SpellManager.R;

            Menu = MainMenu.AddMenu("Lulu", "Lulu");

            pixMenu = Menu.AddSubMenu("Pix", "Pix");
            pixMenu.AddGroupLabel("Q");
            pixMenu.Add("QPixCombo", new CheckBox("Use in Combo", false));
            pixMenu.Add("QPixHarass", new CheckBox("Use in Harass", false));
            pixMenu.AddGroupLabel("EQ");
            pixMenu.Add("EQPixCombo", new CheckBox("Use in Combo"));
            pixMenu.Add("EQPixHarass", new CheckBox("Use in Harass"));

            qMenu = Menu.AddSubMenu("Q", "Q");
            qMenu.Add("QCombo", new CheckBox("Use in Combo"));
            qMenu.Add("QHarass", new CheckBox("Use in Harass"));
            qMenu.Add("QGapcloser", new CheckBox("Use Q on Gapcloser"));
            qMenu.Add("QImpaired", new CheckBox("Auto Q Movement Impaired", false));
            qMenu.Add("QFarm", new KeyBind("Use Q to Farm", true, KeyBind.BindTypes.PressToggle, 'K'));
            qMenu.Add("QLC", new CheckBox("Use in LaneClear"));
            qMenu.Add("QLH", new CheckBox("Use in LastHit", false));

            wMenu = Menu.AddSubMenu("W", "W");
            wMenu.AddGroupLabel("Enemy Priority");
            foreach (var enemy in Enemies)
            {
                wMenu.Add(enemy.NetworkId + "WPriority", new Slider(enemy.ChampionName, 1, 0, 5));
            }
            wMenu.Add("WPriority", new CheckBox("Priority Enabled", false));
            wMenu.AddSeparator();
            wMenu.Add("WCombo", new CheckBox("Use on Enemy in Combo"));
            wMenu.Add("WHarass", new CheckBox("Use on Enemy in Harass"));
            wMenu.Add("WGapcloser", new CheckBox("Use W on Gapcloser"));
            wMenu.Add("WInterrupter", new CheckBox("Use W to Interrupt"));

            eMenu = Menu.AddSubMenu("E", "E");
            eMenu.AddGroupLabel("Ally Shielding");
            foreach (var ally in Allies)
            {
                eMenu.Add(ally.NetworkId + "EPriority", new Slider(ally.ChampionName + " Min Health", 20));
            }
            eMenu.Add("EAuto", new CheckBox("Use E on Allies"));
            eMenu.Add("ECombo", new CheckBox("Use on Enemy in Combo"));
            eMenu.Add("EHarass", new CheckBox("Use on Enemy in Harass"));

            rMenu = Menu.AddSubMenu("R", "R");
            rMenu.AddGroupLabel("Saver");
            foreach (var ally in Allies)
            {
                rMenu.Add(ally.NetworkId + "RPriority", new Slider(ally.ChampionName + " Min Health", 15));
            }
            rMenu.Add("RAuto", new CheckBox("Use R on Allies"));
            rMenu.AddSeparator();
            rMenu.Add("RForce", new KeyBind("Force Ult Ally", false, KeyBind.BindTypes.HoldActive, 'K'));
            rMenu.Add("RInterrupter", new CheckBox("Use R on Interrupt"));
            rMenu.Add("RKnockup", new CheckBox("Auto R to Knockup"));
            rMenu.Add("RKnockupEnemies", new Slider("Min Enemes to Knockup", 2, 1, 5));

            ksMenu = Menu.AddSubMenu("Killsteal", "Killsteal");
            ksMenu.Add("KSEnabled", new CheckBox("Enabled"));
            ksMenu.Add("KSQ", new CheckBox("Use Q"));
            ksMenu.Add("KSE", new CheckBox("Use E"));
            ksMenu.Add("KSEQ", new CheckBox("Use E->Q"));

            ManaManager.Initialize(Menu);
            Q.SetManaCondition(ManaManager.ManaMode.Combo, 5);
            Q.SetManaCondition(ManaManager.ManaMode.Harass, 5);
            Q.SetManaCondition(ManaManager.ManaMode.Farm, 30);

            W.SetManaCondition(ManaManager.ManaMode.Combo, 15);
            W.SetManaCondition(ManaManager.ManaMode.Harass, 15);

            E.SetManaCondition(ManaManager.ManaMode.Combo, 10);
            E.SetManaCondition(ManaManager.ManaMode.Harass, 10);

            fleeMenu = Menu.AddSubMenu("Flee", "Flee");
            fleeMenu.Add("Flee", new KeyBind("Flee", false, KeyBind.BindTypes.HoldActive, 'T'));
            fleeMenu.Add("FleeW", new CheckBox("Use W"));
            fleeMenu.Add("FleeMove", new CheckBox("Move to Cursor Position"));

            drawMenu = Menu.AddSubMenu("Drawings", "Drawings");
            drawMenu.Add("DrawQ", new CheckBox("Draw Q"));//, System.Drawing.Color.Purple, Q.Range));
            drawMenu.Add("DrawW", new CheckBox("Draw W/E"));//, System.Drawing.Color.Purple, W.Range));
            drawMenu.Add("DrawR", new CheckBox("Draw R"));//, System.Drawing.Color.Purple, R.Range));
            drawMenu.Add("DrawPix", new CheckBox("Draw Pix"));

            miscMenu = Menu.AddSubMenu("Misc", "Misc");
            miscMenu.Add("Support", new CheckBox("Support Mode", false));
            CustomAntiGapcloser.Initialize(miscMenu);
            CustomInterrupter.Initialize(miscMenu);

            superMMenu = Menu.AddSubMenu("Superman", "Speedy Up!");
            foreach (var ally in Allies.Where(a => a.Team == ObjectManager.Player.Team))
            {
                superMMenu.Add(ally.NetworkId + "WEPriority", new Slider(ally.ChampionName + " Priority", 1, 0, 5));
            }
            superMMenu.Add("Superman", new KeyBind("Use Speedy Up!", false, KeyBind.BindTypes.HoldActive, 'A'));


            ManaBarIndicator.Initialize(drawMenu, ManaCostDictionary);
            Pix.Initialize(drawMenu["DrawPix"].Cast<CheckBox>().CurrentValue);
            SpellManager.Initialize();

            CustomAntiGapcloser.OnEnemyGapcloser += CustomAntiGapcloser_OnEnemyGapcloser;
            CustomInterrupter.OnInterruptableTarget += CustomInterrupter_OnInterruptableTarget;
        }

        public override void OnCombo()
        {
            if (W.IsActive() && !W.HasManaCondition() && W.IsReady() && getCheckBoxItem(wMenu, "WPriority"))
            {
                var wTarg = Utility.GetBestWTarget();
                if (wTarg != null && W.CanCast(wTarg) && W.CastOnUnit(wTarg))
                {
                    Console.WriteLine("[AUTO] Cast W");
                    return;
                }
            }

            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical) ?? Pix.GetTarget();

            if (!target.LSIsValidTarget() || !SpellManager.Q.IsInRange(target))
            {
                PixCombo();
                return;
            }

            if (PixCombo())
            {
                return;
            }

            if (E.IsActive() && !E.HasManaCondition() && E.CanCast(target) && E.CastOnUnit(target))
            {
                Console.WriteLine("[Combo] Cast E");
                return;
            }

            if (Q.IsReady() && W.IsActive() && !W.HasManaCondition() && W.CanCast(target) && W.CastOnUnit(target))
            {
                Console.WriteLine("[Combo] Cast W");
                return;
            }

            if (!Q.IsActive() || !Q.IsReady() || Q.HasManaCondition())
            {
                return;
            }

            if (Q.Cast(target).IsCasted())
            {
                Console.WriteLine("[Combo] Cast Q");
            }
        }

        private static bool PixCombo(AIHeroClient target, bool useQ, bool useE, bool killSteal = false)
        {
            if (!target.LSIsValidTarget() || !Pix.IsValid())
            {
                return false;
            }

            useQ &= Q.IsReady() && (killSteal || !Q.HasManaCondition());
            useE &= useQ && E.IsReady() && (killSteal || !E.HasManaCondition()) &&
                    Player.Mana > ManaCostDictionary[Q.Slot][Q.Level] + ManaCostDictionary[E.Slot][E.Level];

            if (useQ && SpellManager.PixQ.IsInRange(target) && SpellManager.PixQ.Cast(target).IsCasted())
            {
                Console.WriteLine("[Pix] Cast Q");
                return true;
            }

            if (!useE)
            {
                return false;
            }

            var eqTarget = Pix.GetETarget(target);
            if (eqTarget == null || !E.CastOnUnit(eqTarget))
            {
                return false;
            }

            Console.WriteLine("[Pix] Cast E");
            return true;
        }

        private static bool PixCombo()
        {
            string s = "";
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                s = "Combo";
            }
            else
            {
                s = "Harass";
            }
            var target = Pix.GetTarget(Q.Range + E.Range);
            return PixCombo(target, getCheckBoxItem(pixMenu, "QPix" + s), getCheckBoxItem(pixMenu, "EQPix" + s));
        }

        public override void OnFarm()
        {
            if (!getKeyBindItem(qMenu, "QFarm") || !Q.IsReady() || Q.HasManaCondition())
            {
                return;
            }

            var condition = Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ? getCheckBoxItem(qMenu, "QLC") : getCheckBoxItem(qMenu, "QLH");

            if (qMenu["QLC"] == null || qMenu["QLH"] == null || !condition)
            {
                return;
            }

            var qMinions = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.NotAlly);
            var killable = qMinions.FirstOrDefault(o => o.Health < Q.GetDamage(o));

            if (killable != null && !killable.CanAAKill() && Q.Cast(killable).IsCasted())
            {
                return;
            }

            var pixMinions = Pix.GetMinions();
            killable = pixMinions.FirstOrDefault(o => o.Health < Q.GetDamage(o));

            if (Pix.IsValid() && killable != null && !killable.CanAAKill() &&
                SpellManager.PixQ.Cast(killable).IsCasted())
            {
                return;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                return;
            }

            var pos = Q.GetLineFarmLocation(qMinions);
            var spell = Q;

            var pixPos = Pix.GetFarmLocation();

            if (Pix.IsValid() && pixPos.MinionsHit > pos.MinionsHit)
            {
                pos = pixPos;
                spell = SpellManager.PixQ;
            }

            if (pos.MinionsHit > 2 && spell.Cast(pos.Position)) {}
        }

        public override void OnUpdate()
        {
            if (Player.IsDead)
            {
                return;
            }

            if (Player.LSIsRecalling())
            {
                return;
            }

            if (Saver())
            {
                return;
            }

            if (Flee() || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                return;
            }

            if (AutoQ())
            {
                return;
            }

            if (Superman())
            {
                return;
            }

            if (AutoR())
            {
                return;
            }

            if (Killsteal()) {}
        }

        private static bool Killsteal()
        {
            if (!getCheckBoxItem(ksMenu, "KSEnabled"))
            {
                return false;
            }

            var mana = Player.Mana;
            var useQ = getCheckBoxItem(ksMenu, "KSQ") && Q.IsReady();
            var useE = getCheckBoxItem(ksMenu, "KSE") && E.IsReady();
            var useEQ = getCheckBoxItem(ksMenu, "KSEQ") && Player.Mana > Q.ManaCost + E.ManaCost;

            if (!useQ && !useE)
            {
                return false;
            }

            foreach (var enemy in
                Enemies.Where(e => e.LSIsValidTarget(E.Range + Q.Range) && !e.IsZombie).OrderBy(e => e.Health))
            {
                var qDmg = Q.GetDamage(enemy);
                var eDmg = E.GetDamage(enemy);

                if (useE && E.IsInRange(enemy))
                {
                    if (eDmg > enemy.Health && E.CastOnUnit(enemy))
                    {
                        return true;
                    }

                    if (useQ && qDmg + eDmg > enemy.Health && useEQ && E.CastOnUnit(enemy))
                    {
                        QAfterETarget = enemy;
                        return true;
                    }
                }


                if (useQ && qDmg > enemy.Health && Q.IsInRange(enemy) && Q.Cast(enemy).IsCasted())
                {
                    return true;
                }

                if (useQ && useE && useEQ && qDmg > enemy.Health && PixCombo(enemy, true, true, true))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool Saver()
        {
            if (Player.InFountain())
            {
                return false;
            }

            var useE = getCheckBoxItem(eMenu, "EAuto") && E.IsReady();
            var useR = getCheckBoxItem(rMenu, "RAuto") && R.IsReady();

            if (!useE && !useR)
            {
                return false;
            }

            foreach (var ally in Allies.Where(h => h.LSIsValidTarget(R.Range, false) && h.LSCountEnemiesInRange(300) > 0))
            {
                var hp = ally.GetPredictedHealthPercent();

                if (useE && E.IsInRange(ally) &&
                    hp <= getSliderItem(eMenu, ally.NetworkId + "EPriority"))
                {
                    E.CastOnUnit(ally);
                }

                if (useR && hp <= getSliderItem(rMenu, ally.NetworkId + "RPriority") && R.CastOnUnit(ally))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool AutoQ()
        {
            return getCheckBoxItem(qMenu, "QImpaired") && Q.IsReady() &&
                   Enemies.Any(e => e.LSIsValidTarget(Q.Range) && e.IsMovementImpaired() && Q.Cast(e).IsCasted());
        }

        private static bool Superman()
        {
            if (!getKeyBindItem(superMMenu, "Superman") || !(W.IsReady() || E.IsReady()))
            {
                return false;
            }

            var target = Utility.GetBestWETarget();

            if (target == null)
            {
                Console.WriteLine("TARG");
                return false;
            }

            if (W.IsReady() && W.IsInRange(target) && W.CastOnUnit(target)) {}

            return E.IsReady() && E.IsInRange(target) && E.CastOnUnit(target);
        }

        private static bool AutoR()
        {
            if (!R.IsReady() || Player.InFountain())
            {
                return false;
            }

            if (getKeyBindItem(rMenu, "RForce") &&
                Allies.Where(h => h.LSIsValidTarget(R.Range, false)).OrderBy(o => o.Health).Any(o => R.CastOnUnit(o)))
            {
                return true;
            }


            if (!getCheckBoxItem(rMenu, "RKnockup"))
            {
                return false;
            }

            var count = 0;
            var bestAlly = Player;
            foreach (var ally in Allies.Where(a => a.LSIsValidTarget(R.Range, false)))
            {
                var c = ally.LSCountEnemiesInRange(RRadius);

                if (c <= count)
                {
                    continue;
                }

                count = c;
                bestAlly = ally;
            }

            return count >= getSliderItem(rMenu, "RKnockupEnemies") && R.CastOnUnit(bestAlly);
        }

        private static bool Flee()
        {
            if (!getKeyBindItem(fleeMenu, "Flee") || !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                return false;
            }

            if (Player.IsDashing())
            {
                return true;
            }

            if (getCheckBoxItem(fleeMenu, "FleeW") && W.IsReady() && W.CastOnUnit(Player))
            {
                return true;
            }

            if (!getCheckBoxItem(fleeMenu, "FleeMove") || Player.GetWaypoints().Last().LSDistance(Game.CursorPos) < 100)
            {
                return true;
            }

            return EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
        }

        public override void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.Slot == SpellSlot.W)
                {
                    LastWCast = Utils.TickCount;
                }

                if (args.Slot == SpellSlot.E && QAfterETarget != null)
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(
                        100, () =>
                        {
                            SpellManager.PixQ.Cast(QAfterETarget);
                            QAfterETarget = null;
                        });
                }

                return;
            }

            if (!(getCheckBoxItem(eMenu, "EAuto") && E.IsReady()) || !(getCheckBoxItem(rMenu, "RAuto") && R.IsReady()))
            {
                return;
            }

            var caster = sender as AIHeroClient;
            var target = args.Target as AIHeroClient;

            if (caster == null || !caster.IsValid || !caster.IsEnemy || target == null || !target.IsValid ||
                !target.IsAlly)
            {
                return;
            }

            var damage = 0d;
            try
            {
                damage = caster.LSGetSpellDamage(target, args.SData.Name);
            }
            catch {}

            var hp = (target.Health - damage) / target.MaxHealth * 100;

            if (E.CanCast(target) && hp <= getSliderItem(eMenu, target.NetworkId + "EPriority"))
            {
                E.CastOnUnit(target);
            }

            if (R.CanCast(target) && hp <= getSliderItem(rMenu, target.NetworkId + "RPriority"))
            {
                R.CastOnUnit(target);
            }
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            foreach (var spell in new[] { "Q", "W", "R" })
            {
                var circle = getCheckBoxItem(drawMenu, "Draw" + spell);

                var r = 0f;

                if (spell == "Q")
                {
                    r = Q.Range;
                }

                if (spell == "W")
                {
                    r = W.Range;
                }

                if (spell == "R")
                {
                    r = R.Range;
                }

                if (circle)
                {
                    Render.Circle.DrawCircle(Player.Position, r, System.Drawing.Color.Purple);
                }
            }
        }

        public override void Orbwalking_BeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (!getCheckBoxItem(miscMenu, "Support") ||
                !HeroManager.Allies.Any(x => x.LSIsValidTarget(1000, false) && !x.IsMe))
            {
                return;
            }

            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) &&
                !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                return;
            }

            var minion = args.Target as Obj_AI_Base;
            if (minion != null && minion.IsMinion && minion.LSIsValidTarget())
            {
                args.Process = false;
            }
        }

        public void CustomInterrupter_OnInterruptableTarget(AIHeroClient sender,
            CustomInterrupter.InterruptableTargetEventArgs args)
        {
            if (sender == null || !sender.LSIsValidTarget())
            {
                return;
            }

            if (Utils.TickCount - LastWCast < 2000)
            {
                return;
            }

            if (getCheckBoxItem(wMenu, "WInterrupter") && W.CanCast(sender) && W.CastOnUnit(sender))
            {
                return;
            }

            if (!getCheckBoxItem(rMenu, "RInterrupter") || !R.IsReady())
            {
                return;
            }

            if (
                Allies.OrderBy(h => h.LSDistance(sender))
                    .Any(h => h.LSIsValidTarget(R.Range, false) && h.LSDistance(sender) < RRadius && R.CastOnUnit(h))) {}
        }

        private static void CustomAntiGapcloser_OnEnemyGapcloser(TreeLib.Core.ActiveGapcloser gapcloser)
        {
            if (!gapcloser.Sender.LSIsValidTarget())
            {
                return;
            }

            if (getCheckBoxItem(qMenu, "QGapcloser") && Q.CanCast(gapcloser.Sender))
            {
                Q.Cast(gapcloser.Sender);
            }

            if (getCheckBoxItem(wMenu, "WGapcloser") && W.CanCast(gapcloser.Sender) && W.CastOnUnit(gapcloser.Sender)) {}
        }
    }
}