using System;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Channels;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace IreliaGod
{
    internal class Program
    {
        private static readonly AIHeroClient Player = ObjectManager.Player;
        private static int lastsheenproc;
        private static int rcount;

        public static Menu
            comboMenu = IreliaMenu.comboMenu,
            targetSelectorMenu = IreliaMenu.targetSelectorMenu,
            harassMenu = IreliaMenu.harassMenu,
            laneclearMenu = IreliaMenu.laneclearMenu,
            drawingsMenu = IreliaMenu.drawingsMenu,
            miscMenu = IreliaMenu.miscMenu,
            fleeMenu = IreliaMenu.fleeMenu;

        public static void OnGameLoad()
        {
            // Only load on Irelia, silly
            if (Player.CharData.BaseSkinName != "Irelia") return;

            // Initialize our menu
            IreliaMenu.Initialize();

            // Initialize our spells
            Spells.Initialize();

            // Subscribe to our events
            Game.OnUpdate += OnUpdate;
            Orbwalker.OnPreAttack += BeforeAttack;
            Drawing.OnDraw += OnDraw;
            Obj_AI_Base.OnBuffLose += OnBuffRemove; // Sheen buff workaround
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += OnInterruptableTarget;

            //EloBuddy.Player.OnBasicAttack += AIHeroClient_OnAggro;
            Obj_AI_Turret.OnBasicAttack += Obj_AI_Turret_OnBasicAttack;

            Obj_AI_Base.OnProcessSpellCast += (sender, eventArgs) =>
            {
                if (sender.IsMe && eventArgs.SData.Name == Spells.E.Instance.SData.Name)
                    LeagueSharp.Common.Utility.DelayAction.Add(260, Orbwalker.ResetAutoAttack);

                if (sender.IsMe && eventArgs.SData.Name == Spells.Q.Instance.SData.Name)
                    LeagueSharp.Common.Utility.DelayAction.Add(260, Orbwalker.ResetAutoAttack);
            };

            Orbwalker.OnPostAttack += (unit, target) =>
            {
                if (getCheckBoxItem(comboMenu, "combo.items") && unit.IsMe && target != null && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    if (Spells.Tiamat.IsReady())
                        Spells.Tiamat.Cast();

                    if (Spells.Hydra.IsReady())
                        Spells.Hydra.Cast();
                }
            };


            comboMenu = IreliaMenu.comboMenu;
            targetSelectorMenu = IreliaMenu.targetSelectorMenu;
            harassMenu = IreliaMenu.harassMenu;
            laneclearMenu = IreliaMenu.laneclearMenu;
            drawingsMenu = IreliaMenu.drawingsMenu;
            miscMenu = IreliaMenu.miscMenu;
            fleeMenu = IreliaMenu.fleeMenu;
        }

        private static void Obj_AI_Turret_OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!getCheckBoxItem(miscMenu, "misc.stunundertower")) return;
            if (!Spells.E.IsReady()) return;
            if (!sender.Name.Contains("Turret")) return;

            foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.NetworkId == args.Target.NetworkId && Player.HealthPercent <= enemy.HealthPercent))
            {
                if (Player.LSDistance(enemy) <= Spells.E.Range)
                    Spells.E.CastOnUnit(enemy);

                else if (Player.LSDistance(enemy) <= Spells.Q.Range && Spells.Q.IsReady())
                {
                    var qminion = MinionManager.GetMinions(Spells.Q.Range + 350, MinionTypes.All, MinionTeam.NotAlly).Where(m => m.LSDistance(Player) <= Spells.Q.Range && m.Health <= QDamage(m) + ExtraWDamage(m) + SheenDamage(m) - 30 && m.LSIsValidTarget()).OrderBy(m => m.LSDistance(enemy.Position) <= Spells.Q.Range + 350).FirstOrDefault();
                    if (qminion != null && qminion.LSDistance(enemy) <= Spells.E.Range)
                    {
                        var qtraveltime = Player.LSDistance(qminion) / Spells.Q.Speed + Spells.Q.Delay;
                        var enemy1 = enemy;
                        Spells.Q.CastOnUnit(qminion);
                        LeagueSharp.Common.Utility.DelayAction.Add((int)qtraveltime, () => Spells.E.CastOnUnit(enemy1));
                    }
                }
            }
        }

        private static bool Selected()
        {
            if (!getCheckBoxItem(targetSelectorMenu, "force.target")) return false;
            var target = TargetSelector.SelectedTarget;
            float range = getSliderItem(targetSelectorMenu, "force.target.range");
            if (target == null || target.IsDead || target.IsZombie) return false;

            return !(Player.LSDistance(target.Position) > range);
        }

        private static Obj_AI_Base GetTarget(float range)
        {
            return Selected() ? TargetSelector.SelectedTarget : TargetSelector.GetTarget(range, DamageType.Physical);
        }

        private static void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender == null) return;
            if (getCheckBoxItem(miscMenu, "misc.interrupt") && sender.LSIsValidTarget(Spells.E.Range) &&
                Spells.E.IsReady() && Player.HealthPercent <= sender.HealthPercent)
                Spells.E.CastOnUnit(sender);
        }

        private static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender == null) return;
            if (getCheckBoxItem(miscMenu, "misc.age") && Spells.E.IsReady() &&
                gapcloser.Sender.LSIsValidTarget())
            {
                Spells.E.Cast(gapcloser.Sender);
            }
        }

        public static float ComboDamage(AIHeroClient hero) // Thanks honda
        {
            var result = 0d;

            if (Spells.Q.IsReady())
            {
                result += QDamage(hero) + ExtraWDamage(hero) + SheenDamage(hero);
            }
            if (Spells.W.IsReady() || Player.HasBuff("ireliahitenstylecharged"))
            {
                result += (ExtraWDamage(hero) +
                           Player.CalcDamage(hero, DamageType.Physical, Player.TotalAttackDamage)) * 3; // 3 autos
            }
            if (Spells.E.IsReady())
            {
                result += Spells.E.GetDamage(hero);
            }
            if (Spells.R.IsReady())
            {
                result += Spells.R.GetDamage(hero) * rcount;
            }

            return (float)result;
        }

        private static void OnBuffRemove(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (sender.IsMe && args.Buff.Name == "sheen")
                lastsheenproc = Utils.TickCount;
        }

        private static void RCount()
        {
            if (rcount == 0 && Spells.R.IsReady())
                rcount = 4;

            if (!Spells.R.IsReady() & rcount != 0)
                rcount = 0;

            foreach (
                var buff in
                    Player.Buffs.Where(b => b.Name == "ireliatranscendentbladesspell" && b.IsValid))
            {
                rcount = buff.Count;
            }
        }

        private static bool UnderTheirTower(Obj_AI_Base target)
        {
            var tower =
                ObjectManager
                    .Get<Obj_AI_Turret>()
                    .FirstOrDefault(turret => turret != null && turret.LSDistance(target) <= 775 && turret.IsValid && turret.Health > 0 && !turret.IsAlly);

            return tower != null;
        }

        private static void OnUpdate(EventArgs args)
        {
            Killsteal();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Harass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Laneclear();
                Jungleclear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                if (Spells.E.IsReady() && getCheckBoxItem(fleeMenu, "flee.e"))
                {
                    var etarget =
                        HeroManager.Enemies
                            .FindAll(
                                enemy =>
                                    enemy.LSIsValidTarget() && Player.LSDistance(enemy.Position) <= Spells.E.Range)
                            .OrderBy(e => e.LSDistance(Player));

                    if (etarget.FirstOrDefault() != null)
                        Spells.E.CastOnUnit(etarget.FirstOrDefault());
                }

                if (Spells.R.IsReady() && getCheckBoxItem(fleeMenu, "flee.r"))
                {
                    var rtarget =
                        HeroManager.Enemies
                            .FindAll(
                                enemy =>
                                    enemy.LSIsValidTarget() && Player.LSDistance(enemy.Position) <= Spells.R.Range)
                            .OrderBy(e => e.LSDistance(Player));
                    if (rtarget.FirstOrDefault() == null) goto WALK;
                    var rprediction = LeagueSharp.Common.Prediction.GetPrediction(rtarget.FirstOrDefault(), Spells.R.Delay,
                        Spells.R.Width, Spells.R.Speed);

                    Spells.R.Cast(rprediction.CastPosition);
                }

                if (Spells.Q.IsReady() && getCheckBoxItem(fleeMenu, "flee.q"))
                {
                    var target =
                        HeroManager.Enemies
                            .FindAll(
                                enemy =>
                                    enemy.LSIsValidTarget() && Player.LSDistance(enemy.Position) <= Spells.R.Range)
                            .MinOrDefault(e => e.LSDistance(Player) <= Spells.R.Range);

                    if (target == null) goto WALK;

                    var qminion =
                        MinionManager
                            .GetMinions(Spells.Q.Range, MinionTypes.All, MinionTeam.NotAlly)
                            .Where(
                                m =>
                                    m.LSIsValidTarget(Spells.Q.Range) &&
                                    m.LSDistance(target) > Player.LSDistance(target))
                            .MaxOrDefault(m => m.LSDistance(target));

                    if (qminion != null)
                        Spells.Q.CastOnUnit(qminion);
                }

                WALK:
                Orbwalker.MoveTo(Game.CursorPos);
            }


            RCount();
        }

        private static void Combo()
        {
            var gctarget = GetTarget(Spells.Q.Range * 2.5f);
            var target = GetTarget(Spells.Q.Range);
            if (gctarget == null) return;

            var qminion =
                MinionManager
                    .GetMinions(Spells.Q.Range + 350, MinionTypes.All, MinionTeam.NotAlly) //added 350 range, bad?
                    .Where(
                        m =>
                            m.LSDistance(Player) <= Spells.Q.Range &&
                            m.Health <= QDamage(m) + ExtraWDamage(m) + SheenDamage(m) - 30 && m.LSIsValidTarget())
                    .OrderBy(m => m.LSDistance(gctarget.Position) <= Spells.Q.Range + 350)
                    .FirstOrDefault();


            if (Spells.Q.IsReady())
            {
                if (getCheckBoxItem(comboMenu, "combo.q.gc") &&
                    gctarget.LSDistance(Player.Position) >= Orbwalking.GetRealAutoAttackRange(gctarget) && qminion != null &&
                    qminion.LSDistance(gctarget.Position) <= Player.LSDistance(gctarget.Position) &&
                    qminion.LSDistance(Player.Position) <= Spells.Q.Range)
                {
                    Spells.Q.CastOnUnit(qminion);
                }

                if (getCheckBoxItem(comboMenu, "combo.q") && target != null &&
                    target.LSDistance(Player.Position) <= Spells.Q.Range &&
                    target.LSDistance(Player.Position) >=
                    getSliderItem(comboMenu, "combo.q.minrange"))
                {
                    if (UnderTheirTower(target))
                        if (target.HealthPercent >=
                            getSliderItem(comboMenu, "combo.q.undertower")) return;

                    if (getCheckBoxItem(comboMenu, "combo.w"))
                        Spells.W.Cast();

                    Spells.Q.CastOnUnit(target);
                }

                if (getCheckBoxItem(comboMenu, "combo.q") &&
                    getCheckBoxItem(comboMenu, "combo.q.lastsecond") && target != null)
                {
                    var buff = Player.Buffs.FirstOrDefault(b => b.Name == "ireliahitenstylecharged" && b.IsValid);
                    if (buff != null && buff.EndTime - Game.Time <= (Player.LSDistance(target) / Spells.Q.Speed + Spells.Q.Delay + .500 + Player.AttackCastDelay) && !Player.Spellbook.IsAutoAttacking)
                    {
                        if (UnderTheirTower(target))
                            if (target.HealthPercent >=
                                getSliderItem(comboMenu, "combo.q.undertower")) return;

                        Spells.Q.Cast(target);
                    }
                }
            }

            if (Spells.E.IsReady() && getCheckBoxItem(comboMenu, "combo.e") && target != null)
            {
                if (getCheckBoxItem(comboMenu, "combo.e.logic") &&
                    target.LSDistance(Player.Position) <= Spells.E.Range)
                {
                    if (target.HealthPercent >= Player.HealthPercent)
                    {
                        Spells.E.CastOnUnit(target);
                    }
                    else if (!target.LSIsFacing(Player) && target.LSDistance(Player.Position) >= Spells.E.Range / 2)
                    {
                        Spells.E.CastOnUnit(target);
                    }
                }
                else if (target.LSDistance(Player.Position) <= Spells.E.Range)
                {
                    Spells.E.CastOnUnit(target);
                }
            }

            if (Spells.R.IsReady() && getCheckBoxItem(comboMenu, "combo.r") && !getCheckBoxItem(comboMenu, "combo.r.selfactivated"))
            {
                if (getCheckBoxItem(comboMenu, "combo.r.weave"))
                {
                    if (target != null && !Player.HasBuff("sheen") &&
                        target.LSDistance(Player.Position) <= Spells.R.Range &&
                        Utils.TickCount - lastsheenproc >= 1500)
                    {
                        Spells.R.Cast(target, false, true);
                    }
                }
                else
                {
                    Spells.R.Cast(target, false, true);
                    // Set to Q range because we are already going to combo them at this point most likely, no stupid long range R initiations
                }
            }
            else if (Spells.R.IsReady() && getCheckBoxItem(comboMenu, "combo.r") && getCheckBoxItem(comboMenu, "combo.r.selfactivated") && rcount <= 3)
            {
                if (getCheckBoxItem(comboMenu, "combo.r.weave"))
                {
                    if (target != null && !Player.HasBuff("sheen") &&
                        target.LSDistance(Player.Position) <= Spells.R.Range &&
                        Utils.TickCount - lastsheenproc >= 1500)
                    {
                        Spells.R.Cast(target, false, true);
                    }
                }
                else
                {
                    Spells.R.Cast(target, false, true);
                    // Set to Q range because we are already going to combo them at this point most likely, no stupid long range R initiations
                }
            }

            if (getCheckBoxItem(comboMenu, "combo.items") && target != null)
            {
                if (Player.LSDistance(target.ServerPosition) <= 600 && ComboDamage((AIHeroClient)target) >= target.Health &&
                    getCheckBoxItem(comboMenu, "combo.ignite"))
                {
                    Player.Spellbook.CastSpell(Spells.Ignite, target);
                }

                if (Spells.Youmuu.IsReady() && target.LSIsValidTarget(Spells.Q.Range))
                {
                    Spells.Youmuu.Cast();
                }

                if (Player.LSDistance(target.ServerPosition) <= 450 && Spells.Cutlass.IsReady())
                {
                    Spells.Cutlass.Cast(target);
                }

                if (Player.LSDistance(target.ServerPosition) <= 450 && Spells.Blade.IsReady())
                {
                    Spells.Blade.Cast(target);
                }
            }
        }

        private static void Harass()
        {
            var gctarget = TargetSelector.GetTarget(Spells.Q.Range * 2.5f, DamageType.Physical);
            var target = TargetSelector.GetTarget(Spells.Q.Range, DamageType.Physical);
            if (gctarget == null) return;
            if (Player.ManaPercent <= getSliderItem(harassMenu, "harass.mana") && Player.HasBuff("ireliatranscendentbladesspell") && rcount >= 1) goto castr;
            if (Player.ManaPercent <= getSliderItem(harassMenu, "harass.mana")) return;

            var qminion =
                MinionManager
                    .GetMinions(Spells.Q.Range, MinionTypes.All, MinionTeam.NotAlly)
                    .Where(
                        m =>
                            m.LSDistance(Player) <= Spells.Q.Range && m.Health <= Spells.Q.GetDamage(m) &&
                            m.LSIsValidTarget())
                    .MinOrDefault(m => m.LSDistance(target) <= Spells.Q.Range);

            if (Spells.Q.IsReady())
            {
                if (getCheckBoxItem(harassMenu, "harass.q.gc") && qminion != null &&
                    qminion.LSDistance(target) <= Player.LSDistance(target))
                {
                    Spells.Q.CastOnUnit(qminion);
                }

                if (getCheckBoxItem(harassMenu, "harass.q") && target != null &&
                    target.LSDistance(Player.Position) <= Spells.Q.Range &&
                    target.LSDistance(Player.Position) >=
                    getSliderItem(harassMenu, "harass.q.minrange"))
                {
                    if (UnderTheirTower(target))
                        if (target.HealthPercent >=
                            getSliderItem(harassMenu, "harass.q.undertower")) return;

                    Spells.Q.CastOnUnit(target);
                }

                if (getCheckBoxItem(harassMenu, "harass.q") &&
                    getCheckBoxItem(harassMenu, "harass.q.lastsecond") && target != null)
                {
                    var buff = Player.Buffs.FirstOrDefault(b => b.Name == "ireliahitenstylecharged" && b.IsValid);
                    if (buff != null && buff.EndTime - Game.Time <= (Player.LSDistance(target) / Spells.Q.Speed + Spells.Q.Delay + .500 + Player.AttackCastDelay) && !Player.Spellbook.IsAutoAttacking)
                    {
                        if (UnderTheirTower(target))
                            if (target.HealthPercent >=
                                getSliderItem(harassMenu, "harass.q.undertower")) return;

                        Spells.Q.Cast(target);
                    }
                }
            }

            if (Spells.E.IsReady() && getCheckBoxItem(harassMenu, "harass.e") && target != null)
            {
                if (getCheckBoxItem(harassMenu, "harass.e.logic") &&
                    target.LSDistance(Player.Position) <= Spells.E.Range)
                {
                    if (target.HealthPercent >= Player.HealthPercent)
                    {
                        Spells.E.CastOnUnit(target);
                    }
                    else if (!target.LSIsFacing(Player) && target.LSDistance(Player.Position) >= Spells.E.Range / 2)
                    {
                        Spells.E.CastOnUnit(target);
                    }
                }
                else if (target.LSDistance(Player.Position) <= Spells.E.Range)
                {
                    Spells.E.CastOnUnit(target);
                }
            }

            castr:
            if (Spells.R.IsReady() && getCheckBoxItem(harassMenu, "harass.r"))
            {
                if (getCheckBoxItem(harassMenu, "harass.r.weave"))
                {
                    if (getCheckBoxItem(harassMenu, "harass.r.weave"))
                    {
                        if (target != null && !Player.HasBuff("sheen") &&
                            target.LSDistance(Player.Position) <= Spells.R.Range &&
                            Utils.TickCount - lastsheenproc >= 1500)
                        {
                            Spells.R.Cast(target, false, true);
                        }
                    }
                }
                else
                {
                    Spells.R.Cast(target, false, true);
                    // Set to Q range because we are already going to combo them at this point most likely, no stupid long range R initiations
                }
            }
        }

        private static void Killsteal()
        {
            foreach (
                var enemy in
                    HeroManager.Enemies.Where(e => e.LSDistance(Player.Position) <= Spells.R.Range && e.LSIsValidTarget()))
            {
                if (enemy == null) return;

                if (Spells.Q.IsReady() && getCheckBoxItem(miscMenu, "misc.ks.q") &&
                    Spells.E.IsReady() && getCheckBoxItem(miscMenu, "misc.ks.e") &&
                    Spells.E.GetDamage(enemy) + QDamage(enemy) + ExtraWDamage(enemy) + SheenDamage(enemy) >=
                            enemy.Health)
                {
                    if (enemy.LSDistance(Player.Position) <= Spells.Q.Range && enemy.LSDistance(Player.Position) > Spells.E.Range)
                    {
                        Spells.Q.Cast(enemy);
                        var enemy1 = enemy;
                        LeagueSharp.Common.Utility.DelayAction.Add((int)(1000 * Player.LSDistance(enemy) / Spells.Q.Speed + Spells.Q.Delay), () => Spells.E.Cast(enemy1));
                    }
                    else if (enemy.LSDistance(Player.Position) <= Spells.Q.Range)
                    {
                        Spells.E.Cast(enemy);
                        var enemy1 = enemy;
                        LeagueSharp.Common.Utility.DelayAction.Add(250, () => Spells.Q.Cast(enemy1));
                    }

                }

                if (getCheckBoxItem(miscMenu, "misc.ks.q") && Spells.Q.IsReady() &&
                    QDamage(enemy) + ExtraWDamage(enemy) + SheenDamage(enemy) >= enemy.Health &&
                    enemy.LSDistance(Player.Position) <= Spells.Q.Range)
                {
                    Spells.Q.CastOnUnit(enemy);
                    return;
                }

                if (getCheckBoxItem(miscMenu, "misc.ks.e") && Spells.E.IsReady() &&
                    Spells.E.GetDamage(enemy) >= enemy.Health && enemy.LSDistance(Player.Position) <= Spells.E.Range)
                {
                    Spells.E.CastOnUnit(enemy);
                    return;
                }

                if (getCheckBoxItem(miscMenu, "misc.ks.r") && Spells.R.IsReady() &&
                    Spells.R.GetDamage(enemy) * rcount >= enemy.Health)
                {
                    Spells.R.Cast(enemy, false, true);
                }

            }
        }

        private static void Laneclear()
        {
            {
                var farmMode = getBoxItem(laneclearMenu, "useqfarm");
                switch (farmMode)
                {
                    case 0:
                        {
                            var unkillableMinion =
                                ObjectManager.Get<Obj_AI_Minion>()
                                    .FirstOrDefault(
                                        m =>
                                            m.IsEnemy && m.Position.LSDistance(ObjectManager.Player.ServerPosition) < 650 &&
                                            m.Position.LSDistance(ObjectManager.Player.Position) >
                                            ObjectManager.Player.AttackRange && m.LSIsValidTarget() &&
                                            m.Health < 25);
                            if (unkillableMinion != null)
                            {
                                Spells.Q.Cast(unkillableMinion);
                            }
                            break;
                        }
                    case 1:
                        {
                            var killableMinion =
                                ObjectManager.Get<Obj_AI_Minion>()
                                    .FirstOrDefault(
                                        m =>
                                            m.IsEnemy && m.Position.LSDistance(ObjectManager.Player.ServerPosition) < 650 &&
                                            m.LSIsValidTarget() && m.Health < Spells.Q.GetDamage(m));
                            if (killableMinion != null)
                            {
                                Spells.Q.Cast(killableMinion);
                            }
                            break;
                        }
                    case 2:
                        {
                            break;
                        }
                }
 }

            if (Player.ManaPercent <= getSliderItem(laneclearMenu, "laneclear.mana")) return;

            var qminion =
                MinionManager
                    .GetMinions(
                        Spells.Q.Range, MinionTypes.All, MinionTeam.NotAlly)
                    .FirstOrDefault(
                        m =>
                            m.LSDistance(Player) <= Spells.Q.Range &&
                            m.Health <= QDamage(m) + ExtraWDamage(m) + SheenDamage(m) - 10 &&
                            m.LSIsValidTarget());



            var rminions = MinionManager.GetMinions(Player.Position, Spells.R.Range);
            if (Spells.R.IsReady() && getCheckBoxItem(laneclearMenu, "laneclear.r") && rminions.Count != 0)
            {
                var location = Spells.R.GetLineFarmLocation(rminions);

                if (location.MinionsHit >=
                    getSliderItem(laneclearMenu, "laneclear.r.minimum"))
                    Spells.R.Cast(location.Position);
            }
        }

        private static void Jungleclear()
        {
        }

        private static void BeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (getCheckBoxItem(comboMenu, "combo.w") &&
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                args.Target != null &&
                args.Target.Type == GameObjectType.AIHeroClient &&
                args.Target.LSIsValidTarget() ||
                getCheckBoxItem(harassMenu, "harass.w") &&
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) &&
                args.Target != null &&
                args.Target.Type == GameObjectType.AIHeroClient &&
                args.Target.LSIsValidTarget())
                Spells.W.Cast();
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

        private static void OnDraw(EventArgs args)
        {
            if (Player.IsDead) return;
            if (getCheckBoxItem(drawingsMenu, "drawings.q"))
            {
                if (Spells.Q.IsReady())
                    Drawing.DrawCircle(Player.Position, Spells.Q.Range, Color.LightCoral);
                else
                    Drawing.DrawCircle(Player.Position, Spells.Q.Range, Color.Maroon);
            }
            if (getCheckBoxItem(drawingsMenu, "drawings.e"))
            {
                if (Spells.E.IsReady())
                    Drawing.DrawCircle(Player.Position, Spells.E.Range, Color.LightCoral);
                else
                    Drawing.DrawCircle(Player.Position, Spells.E.Range, Color.Maroon);
            }
            if (getCheckBoxItem(drawingsMenu, "drawings.r"))
            {
                if (Spells.R.IsReady())
                    Drawing.DrawCircle(Player.Position, Spells.R.Range, Color.LightCoral);
                else
                    Drawing.DrawCircle(Player.Position, Spells.R.Range, Color.Maroon);
            }
        }

        private static double SheenDamage(Obj_AI_Base target) // Thanks princer007 for the basic idea
        {
            var result = 0d;
            foreach (var item in Player.InventoryItems)
                switch ((int)item.Id)
                {
                    case 3057: //Sheen
                        if (Utils.TickCount - lastsheenproc >= 1750 + Game.Ping)
                            result += Player.CalcDamage(target, DamageType.Physical, Player.BaseAttackDamage);
                        break;
                    case 3078: //Triforce
                        if (Utils.TickCount - lastsheenproc >= 1750 + Game.Ping)
                            result += Player.CalcDamage(target, DamageType.Physical, Player.BaseAttackDamage * 2);
                        break;
                }
            return result;
        }

        private static double ExtraWDamage(Obj_AI_Base target)
        {
            // tried some stuff with if buff == null but the damage will be enough then cast W and it worked.. but meh, idk will look at later

            var extra = 0d;
            var buff = Player.Buffs.FirstOrDefault(b => b.Name == "ireliahitenstylecharged" && b.IsValid);
            if (buff != null && buff.EndTime < (1000 * Player.LSDistance(target) / Spells.Q.Speed + Spells.Q.Delay))
                extra += new double[] { 15, 30, 45, 60, 75 }[Spells.W.Level - 1];

            return extra;
        }

        private static double QDamage(Obj_AI_Base target)
        {
            return Spells.Q.IsReady()
                ? Player.CalcDamage(
                    target,
                    DamageType.Physical,
                    new double[] { 20, 50, 80, 110, 140 }[Spells.Q.Level - 1]
                    + Player.TotalAttackDamage)
                //- 25) Safety net, for some reason the damage is never exact ): why?
                : 0d;
        }
    }
}