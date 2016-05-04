using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using Damage = LeagueSharp.Common.Damage;

namespace YasuoPro
{
    //Credits to Kortatu/Esk0r for his work on Evade which this assembly relies on heavily!

    internal class Yasuo : Helper
    {
        public AIHeroClient CurrentTarget;
        public bool Fleeing;

        public Yasuo()
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }

        private void OnLoad(EventArgs args)
        {
            Yasuo = ObjectManager.Player;

            if (Yasuo.CharData.BaseSkinName != "Yasuo")
            {
                return;
            }

            InitItems();
            InitSpells();
            YasuoMenu.Init(this);
            //Program.Init();
            if (YasuoMenu.getCheckBoxItem(YasuoMenu.MiscA, "Misc.Walljump") && Game.MapId == GameMapId.SummonersRift)
            {
                WallJump.Initialize();
            }
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            AntiGapcloser.OnEnemyGapcloser += OnGapClose;
            Interrupter2.OnInterruptableTarget += OnInterruptable;
            //Obj_AI_Base.OnProcessSpellCast += TargettedDanger.SpellCast;
        }


        private void OnUpdate(EventArgs args)
        {
            if (Yasuo.IsDead || Yasuo.IsRecalling())
            {
                return;
            }

            CastUlt();

            //if (YasuoMenu.getCheckBoxItem(YasuoMenu.EvadeA, "Evade.WTS"))
            //{
            //TargettedDanger.OnUpdate();
            //}

            if (YasuoMenu.getCheckBoxItem(YasuoMenu.MiscA, "Misc.AutoStackQ") && !TornadoReady &&
                !CurrentTarget.IsValidEnemy(Spells[Q].Range))
            {
                var closest =
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(x => x.IsValidMinion(Spells[Q].Range) && MinionManager.IsMinion(x))
                        .MinOrDefault(x => x.Distance(Yasuo));

                var pred = Spells[Q].GetPrediction(closest);
                if (pred.Hitchance >= HitChance.Low)
                {
                    Spells[Q].Cast(closest.ServerPosition);
                }
            }

            if (YasuoMenu.getCheckBoxItem(YasuoMenu.MiscA, "Misc.Walljump") && Game.MapId == GameMapId.SummonersRift)
            {
                WallJump.OnUpdate();
            }

            Fleeing = Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee);

            if (YasuoMenu.getCheckBoxItem(YasuoMenu.KillstealA, "Killsteal.Enabled") && !Fleeing)
            {
                Killsteal();
            }

            if (YasuoMenu.getKeyBindItem(YasuoMenu.HarassA, "Harass.KB") && !Fleeing)
            {
                Harass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Orbwalker.DisableAttacking = false;
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Orbwalker.DisableAttacking = false;
                Mixed();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                Orbwalker.DisableAttacking = false;
                LHSkills();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                Orbwalker.DisableAttacking = false;
                Waveclear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                Flee();
            }
        }

        private void CastUlt()
        {
            if (!SpellSlot.R.IsReady())
            {
                return;
            }
            if (YasuoMenu.getCheckBoxItem(YasuoMenu.ComboA, "Combo.UseR") &&
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                CastR(YasuoMenu.getSliderItem(YasuoMenu.ComboA, "Combo.RMinHit"));
            }

            if (YasuoMenu.getCheckBoxItem(YasuoMenu.MiscA, "Misc.AutoR") && !Fleeing)
            {
                CastR(YasuoMenu.getSliderItem(YasuoMenu.MiscA, "Misc.RMinHit"));
            }
        }

        private void OnDraw(EventArgs args)
        {
            if (Debug)
            {
                Drawing.DrawCircle(DashPosition.To3D(), Yasuo.BoundingRadius, Color.Chartreuse);
            }


            if (Yasuo.IsDead || YasuoMenu.getCheckBoxItem(YasuoMenu.DrawingsA, "Drawing.Disable"))
            {
                return;
            }

            //TargettedDanger.OnDraw(args);

            if (YasuoMenu.getCheckBoxItem(YasuoMenu.MiscA, "Misc.Walljump") && Game.MapId == GameMapId.SummonersRift)
            {
                WallJump.OnDraw();
            }

            var pos = Yasuo.Position.WTS();

            Drawing.DrawText(pos.X - 25, pos.Y - 25, isHealthy ? Color.Green : Color.Red, "Healthy: " + isHealthy);

            var drawq = YasuoMenu.getCheckBoxItem(YasuoMenu.DrawingsA, "Drawing.DrawQ");
            var drawe = YasuoMenu.getCheckBoxItem(YasuoMenu.DrawingsA, "Drawing.DrawE");
            var drawr = YasuoMenu.getCheckBoxItem(YasuoMenu.DrawingsA, "Drawing.DrawR");

            if (drawq)
            {
                Render.Circle.DrawCircle(Yasuo.Position, Qrange, Color.Red);
            }
            if (drawe)
            {
                Render.Circle.DrawCircle(Yasuo.Position, Spells[E].Range, Color.CornflowerBlue);
            }
            if (drawr)
            {
                Render.Circle.DrawCircle(Yasuo.Position, Spells[R].Range, Color.DarkOrange);
            }
        }


        private void Combo()
        {
            CurrentTarget = TargetSelector.GetTarget(Spells[R].Range, DamageType.Physical);

            CastQ(CurrentTarget);

            if (YasuoMenu.getCheckBoxItem(YasuoMenu.ComboA, "Combo.UseE"))
            {
                CastE(CurrentTarget);
            }

            if (YasuoMenu.getCheckBoxItem(YasuoMenu.ComboA, "Items.Enabled"))
            {
                if (YasuoMenu.getCheckBoxItem(YasuoMenu.ComboA, "Items.UseTIA"))
                {
                    Tiamat.Cast(null);
                }
                if (YasuoMenu.getCheckBoxItem(YasuoMenu.ComboA, "Items.UseHDR"))
                {
                    Hydra.Cast(null);
                }
                if (YasuoMenu.getCheckBoxItem(YasuoMenu.ComboA, "Items.UseBRK") && CurrentTarget != null)
                {
                    Blade.Cast(CurrentTarget);
                }
                if (YasuoMenu.getCheckBoxItem(YasuoMenu.ComboA, "Items.UseBLG") && CurrentTarget != null)
                {
                    Bilgewater.Cast(CurrentTarget);
                }
                if (YasuoMenu.getCheckBoxItem(YasuoMenu.ComboA, "Items.UseYMU"))
                {
                    Youmu.Cast(null);
                }
            }
        }

        private void CastQ(AIHeroClient target)
        {
            if (Spells[Q].IsReady() && target.IsValidEnemy(Qrange))
            {
                UseQ(target, GetHitChance("Hitchance.Q"), YasuoMenu.getCheckBoxItem(YasuoMenu.ComboA, "Combo.UseQ"),
                    YasuoMenu.getCheckBoxItem(YasuoMenu.ComboA, "Combo.UseQ2"));
                return;
            }

            if (YasuoMenu.getCheckBoxItem(YasuoMenu.ComboA, "Combo.StackQ") && !target.IsValidEnemy(Qrange) &&
                !TornadoReady)
            {
                var bestmin =
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(x => x.IsValidMinion(Qrange) && MinionManager.IsMinion(x))
                        .MinOrDefault(x => x.Distance(Yasuo));
                if (bestmin != null)
                {
                    var pred = Spells[Q].GetPrediction(bestmin);

                    if (pred.Hitchance >= HitChance.Medium)
                    {
                        Spells[Q].Cast(bestmin.ServerPosition);
                    }
                }
            }
        }

        private void CastE(AIHeroClient target)
        {
            if (target != null)
            {
                if (SpellSlot.E.IsReady() && isHealthy && target.Distance(Yasuo) >= 0.30*Yasuo.AttackRange)
                {
                    if (DashCount >= 1 && GetDashPos(target).IsCloser(target) && target.IsDashable() &&
                        (YasuoMenu.getCheckBoxItem(YasuoMenu.ComboA, "Combo.ETower") ||
                         YasuoMenu.getKeyBindItem(YasuoMenu.MiscA, "Misc.TowerDive") ||
                         !GetDashPos(target).PointUnderEnemyTurret()))
                    {
                        ETarget = target;
                        Spells[E].CastOnUnit(target);
                        return;
                    }

                    if (DashCount == 0)
                    {
                        var bestminion =
                            ObjectManager.Get<Obj_AI_Base>()
                                .Where(
                                    x =>
                                        x.IsDashable()
                                        && GetDashPos(x).IsCloser(target) &&
                                        (YasuoMenu.getCheckBoxItem(YasuoMenu.ComboA, "Combo.ETower") ||
                                         YasuoMenu.getKeyBindItem(YasuoMenu.MiscA, "Misc.TowerDive") ||
                                         !GetDashPos(x).PointUnderEnemyTurret()))
                                .OrderBy(x => Vector2.Distance(GetDashPos(x), target.ServerPosition.To2D()))
                                .FirstOrDefault();
                        if (bestminion != null)
                        {
                            ETarget = bestminion;
                            Spells[E].CastOnUnit(bestminion);
                        }

                        else if (target.IsDashable() && GetDashPos(target).IsCloser(target) &&
                                 (YasuoMenu.getCheckBoxItem(YasuoMenu.ComboA, "Combo.ETower") ||
                                  YasuoMenu.getKeyBindItem(YasuoMenu.MiscA, "Misc.TowerDive") ||
                                  !GetDashPos(target).PointUnderEnemyTurret()))
                        {
                            ETarget = target;
                            Spells[E].CastOnUnit(target);
                        }
                    }


                    else
                    {
                        var minion =
                            ObjectManager.Get<Obj_AI_Base>()
                                .Where(
                                    x =>
                                        x.IsDashable() && GetDashPos(x).IsCloser(target) &&
                                        (YasuoMenu.getCheckBoxItem(YasuoMenu.ComboA, "Combo.ETower") ||
                                         YasuoMenu.getKeyBindItem(YasuoMenu.MiscA, "Misc.TowerDive") ||
                                         !GetDashPos(x).PointUnderEnemyTurret()))
                                .OrderBy(x => GetDashPos(x).Distance(target.ServerPosition)).FirstOrDefault();

                        if (minion != null && GetDashPos(minion).IsCloser(target))
                        {
                            ETarget = minion;
                            Spells[E].CastOnUnit(minion);
                        }
                    }
                }
            }
        }

        private void CastR(float minhit = 1)
        {
            var ultmode = GetUltMode();

            IOrderedEnumerable<AIHeroClient> ordered = null;


            if (ultmode == UltMode.Health)
            {
                ordered =
                    KnockedUp.OrderBy(x => x.Health)
                        .ThenByDescending(TargetSelector.GetPriority)
                        .ThenByDescending(x => x.CountEnemiesInRange(350));
            }

            if (ultmode == UltMode.Priority)
            {
                ordered =
                    KnockedUp.OrderByDescending(TargetSelector.GetPriority)
                        .ThenBy(x => x.Health)
                        .ThenByDescending(x => x.CountEnemiesInRange(350));
            }

            if (ultmode == UltMode.EnemiesHit)
            {
                ordered =
                    KnockedUp.OrderByDescending(x => x.CountEnemiesInRange(350))
                        .ThenByDescending(TargetSelector.GetPriority)
                        .ThenBy(x => x.Health);
            }

            if ((YasuoMenu.getCheckBoxItem(YasuoMenu.ComboA, "Combo.OnlyifMin") && ordered.Count() < minhit) ||
                (ordered.Count() == 1 &&
                 ordered.FirstOrDefault().HealthPercent <
                 YasuoMenu.getSliderItem(YasuoMenu.ComboA, "Combo.MinHealthUlt")))
            {
                return;
            }

            if (YasuoMenu.getCheckBoxItem(YasuoMenu.ComboA, "Combo.RPriority"))
            {
                var best =
                    ordered.Find(
                        x =>
                            !x.isBlackListed() && TargetSelector.GetPriority(x) == 5 &&
                            (YasuoMenu.getCheckBoxItem(YasuoMenu.ComboA, "Combo.UltTower") ||
                             YasuoMenu.getKeyBindItem(YasuoMenu.MiscA, "Misc.TowerDive") ||
                             !x.Position.To2D().PointUnderEnemyTurret()));
                if (best != null && Yasuo.HealthPercent/best.HealthPercent <= 1)
                {
                    Spells[R].CastOnUnit(best);
                    return;
                }
            }

            if (YasuoMenu.getCheckBoxItem(YasuoMenu.ComboA, "Combo.UltOnlyKillable"))
            {
                var killable =
                    ordered.FirstOrDefault(
                        x =>
                            !x.isBlackListed() && x.Health <= Yasuo.GetSpellDamage(x, SpellSlot.R) &&
                            x.HealthPercent >= YasuoMenu.getSliderItem(YasuoMenu.ComboA, "Combo.MinHealthUlt") &&
                            (YasuoMenu.getCheckBoxItem(YasuoMenu.ComboA, "Combo.UltTower") ||
                             YasuoMenu.getKeyBindItem(YasuoMenu.MiscA, "Misc.TowerDive") ||
                             !x.Position.To2D().PointUnderEnemyTurret()));
                if (killable != null && !killable.IsInRange(Spells[Q].Range))
                {
                    Spells[R].CastOnUnit(killable);
                    return;
                }
            }

            if (ordered.Count() >= minhit)
            {
                var best2 =
                    ordered.FirstOrDefault(
                        x =>
                            !x.isBlackListed() &&
                            (YasuoMenu.getCheckBoxItem(YasuoMenu.ComboA, "Combo.UltTower") ||
                             YasuoMenu.getKeyBindItem(YasuoMenu.MiscA, "Misc.TowerDive") ||
                             !x.Position.To2D().PointUnderEnemyTurret()));
                if (best2 != null)
                {
                    Spells[R].CastOnUnit(best2);
                }
            }
        }


        private void Flee()
        {
            //Orbwalker.SetAttack(false);
            Orbwalker.DisableAttacking = true;
            if (YasuoMenu.getCheckBoxItem(YasuoMenu.Flee, "Flee.UseQ2") && !Yasuo.IsDashing() && SpellSlot.Q.IsReady() &&
                TornadoReady)
            {
                var qtarg = TargetSelector.GetTarget(Spells[Q2].Range, DamageType.Physical);
                if (qtarg != null)
                {
                    Spells[Q2].Cast(qtarg.ServerPosition);
                }
            }

            if (FleeMode == FleeType.ToCursor)
            {
                Orbwalker.OrbwalkTo(Game.CursorPos);

                if (Spells[E].IsReady())
                {
                    var dashtarg =
                        ObjectManager.Get<Obj_AI_Base>()
                            .Where(x => x.IsDashable())
                            .MinOrDefault(x => GetDashPos(x).Distance(Game.CursorPos));

                    if (dashtarg != null &&
                        GetDashPos(dashtarg).Distance(Game.CursorPos) < Yasuo.Distance(Game.CursorPos))
                    {
                        Spells[E].CastOnUnit(dashtarg);

                        if (YasuoMenu.getCheckBoxItem(YasuoMenu.Flee, "Flee.StackQ") && SpellSlot.Q.IsReady() &&
                            !TornadoReady)
                        {
                            Spells[Q].Cast(dashtarg.ServerPosition);
                        }
                    }
                }
            }

            if (FleeMode == FleeType.ToNexus)
            {
                var nexus = ObjectManager.Get<Obj_Shop>().FirstOrDefault(x => x.IsAlly);
                if (nexus != null)
                {
                    Orbwalker.OrbwalkTo(nexus.Position);
                    var bestminion =
                        ObjectManager.Get<Obj_AI_Base>()
                            .Where(x => x.IsDashable())
                            .MinOrDefault(x => GetDashPos(x).Distance(nexus.Position));
                    if (bestminion != null &&
                        GetDashPos(bestminion).Distance(nexus.Position) < Yasuo.Distance(nexus.Position))
                    {
                        Spells[E].CastOnUnit(bestminion);
                        if (YasuoMenu.getCheckBoxItem(YasuoMenu.Flee, "Flee.StackQ") && SpellSlot.Q.IsReady() &&
                            !TornadoReady)
                        {
                            Spells[Q].Cast(bestminion.ServerPosition);
                        }
                    }
                }
            }

            if (FleeMode == FleeType.ToAllies)
            {
                Obj_AI_Base bestally =
                    HeroManager.Allies.Where(x => !x.IsMe && x.CountEnemiesInRange(300) == 0)
                        .MinOrDefault(x => x.Distance(Yasuo)) ?? (Obj_AI_Base) ObjectManager.Get<Obj_AI_Minion>()
                            .Where(x => x.IsValidAlly(3000))
                            .MinOrDefault(x => x.Distance(Yasuo));

                if (bestally != null)
                {
                    Orbwalker.OrbwalkTo(bestally.ServerPosition);
                    if (Spells[E].IsReady())
                    {
                        var besttarget =
                            ObjectManager.Get<Obj_AI_Base>()
                                .Where(x => x.IsDashable())
                                .MinOrDefault(x => GetDashPos(x).Distance(bestally.ServerPosition));
                        if (besttarget != null)
                        {
                            Spells[E].CastOnUnit(besttarget);
                            if (YasuoMenu.getCheckBoxItem(YasuoMenu.Flee, "Flee.StackQ") && SpellSlot.Q.IsReady() &&
                                !TornadoReady)
                            {
                                Spells[Q].Cast(besttarget.ServerPosition);
                            }
                        }
                    }
                }

                else
                {
                    var nexus = ObjectManager.Get<Obj_Shop>().FirstOrDefault(x => x.IsAlly);
                    if (nexus != null)
                    {
                        Orbwalker.OrbwalkTo(nexus.Position);
                        var bestminion =
                            ObjectManager.Get<Obj_AI_Base>()
                                .Where(x => x.IsDashable())
                                .MinOrDefault(x => GetDashPos(x).Distance(nexus.Position));
                        if (bestminion != null &&
                            GetDashPos(bestminion).Distance(nexus.Position) < Yasuo.Distance(nexus.Position))
                        {
                            Spells[E].CastOnUnit(bestminion);
                        }
                    }
                }
            }
        }


        private void Waveclear()
        {
            if (SpellSlot.Q.IsReady() && !Yasuo.IsDashing())
            {
                if (!TornadoReady && YasuoMenu.getCheckBoxItem(YasuoMenu.WaveclearA, "Waveclear.UseQ"))
                {
                    var minion =
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(
                                x =>
                                    x.IsValidMinion(Spells[Q].Range) &&
                                    ((x.IsDashable() &&
                                      (x.Health - Yasuo.GetSpellDamage(x, SpellSlot.Q) >= GetProperEDamage(x))) ||
                                     x.Health - Yasuo.GetSpellDamage(x, SpellSlot.Q) >= 0.15*x.MaxHealth || x.QCanKill()))
                            .MaxOrDefault(x => x.MaxHealth);
                    if (minion != null)
                    {
                        Spells[Q].Cast(minion.ServerPosition);
                    }
                }

                else if (TornadoReady && YasuoMenu.getCheckBoxItem(YasuoMenu.WaveclearA, "Waveclear.UseQ2"))
                {
                    var minions =
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(
                                x =>
                                    x.Distance(Yasuo) > Yasuo.AttackRange && x.IsValidMinion(Spells[Q2].Range) &&
                                    ((x.IsDashable() &&
                                      x.Health - Yasuo.GetSpellDamage(x, SpellSlot.Q) >= 0.85*GetProperEDamage(x)) ||
                                     (x.Health - Yasuo.GetSpellDamage(x, SpellSlot.Q) >= 0.10*x.MaxHealth) ||
                                     x.CanKill(SpellSlot.Q)));
                    var pred =
                        MinionManager.GetBestLineFarmLocation(minions.Select(m => m.ServerPosition.To2D()).ToList(),
                            Spells[Q2].Width, Spells[Q2].Range);
                    if (pred.MinionsHit >= YasuoMenu.getSliderItem(YasuoMenu.WaveclearA, "Waveclear.Qcount"))
                    {
                        Spells[Q2].Cast(pred.Position);
                    }
                }
            }

            if (SpellSlot.E.IsReady() && YasuoMenu.getCheckBoxItem(YasuoMenu.WaveclearA, "Waveclear.UseE") &&
                (!YasuoMenu.getCheckBoxItem(YasuoMenu.WaveclearA, "Waveclear.Smart") || isHealthy))
            {
                var minions =
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(
                            x =>
                                x.IsDashable() &&
                                ((YasuoMenu.getCheckBoxItem(YasuoMenu.WaveclearA, "Waveclear.UseENK") &&
                                  (!YasuoMenu.getCheckBoxItem(YasuoMenu.WaveclearA, "Waveclear.Smart") ||
                                   x.Health - GetProperEDamage(x) > GetProperEDamage(x)*3)) || x.ECanKill()) &&
                                (YasuoMenu.getCheckBoxItem(YasuoMenu.WaveclearA, "Waveclear.ETower") ||
                                 YasuoMenu.getKeyBindItem(YasuoMenu.MiscA, "Misc.TowerDive") ||
                                 !GetDashPos(x).PointUnderEnemyTurret()));
                Obj_AI_Minion minion = null;
                minion = minions.MaxOrDefault(x => GetDashPos(x).MinionsInRange(200));
                if (minion != null)
                {
                    Spells[E].Cast(minion);
                }
            }

            if (YasuoMenu.getCheckBoxItem(YasuoMenu.WaveclearA, "Waveclear.UseItems"))
            {
                if (YasuoMenu.getCheckBoxItem(YasuoMenu.WaveclearA, "Waveclear.UseTIA"))
                {
                    Tiamat.minioncount = YasuoMenu.getSliderItem(YasuoMenu.WaveclearA, "Waveclear.MinCountHDR");
                    Tiamat.Cast(null, true);
                }
                if (YasuoMenu.getCheckBoxItem(YasuoMenu.WaveclearA, "Waveclear.UseHDR"))
                {
                    Hydra.minioncount = YasuoMenu.getSliderItem(YasuoMenu.WaveclearA, "Waveclear.MinCountHDR");
                    Hydra.Cast(null, true);
                }
                if (YasuoMenu.getCheckBoxItem(YasuoMenu.WaveclearA, "Waveclear.UseYMU"))
                {
                    Youmu.minioncount = YasuoMenu.getSliderItem(YasuoMenu.WaveclearA, "Waveclear.MinCountYOU");
                    Youmu.Cast(null, true);
                }
            }
        }


        private void Killsteal()
        {
            if (SpellSlot.Q.IsReady() && YasuoMenu.getCheckBoxItem(YasuoMenu.KillstealA, "Killsteal.UseQ"))
            {
                var targ = HeroManager.Enemies.Find(x => x.CanKill(SpellSlot.Q) && x.IsInRange(Qrange));
                if (targ != null)
                {
                    UseQ(targ, GetHitChance("Hitchance.Q"));
                    return;
                }
            }

            if (SpellSlot.E.IsReady() && YasuoMenu.getCheckBoxItem(YasuoMenu.KillstealA, "Killsteal.UseE"))
            {
                var targ = HeroManager.Enemies.Find(x => x.CanKill(SpellSlot.E) && x.IsInRange(Spells[E].Range));
                if (targ != null)
                {
                    Spells[E].Cast(targ);
                    return;
                }
            }

            if (SpellSlot.R.IsReady() && YasuoMenu.getCheckBoxItem(YasuoMenu.KillstealA, "Killsteal.UseR"))
            {
                var targ =
                    KnockedUp.Find(x => x.CanKill(SpellSlot.R) && x.IsValidEnemy(Spells[R].Range) && !x.isBlackListed());
                if (targ != null)
                {
                    Spells[R].Cast(targ);
                    return;
                }
            }

            if (YasuoMenu.getCheckBoxItem(YasuoMenu.KillstealA, "Killsteal.UseItems"))
            {
                if (Tiamat.item.IsReady())
                {
                    var targ =
                        HeroManager.Enemies.Find(
                            x =>
                                x.IsValidEnemy(Tiamat.item.Range) &&
                                x.Health <= Yasuo.GetItemDamage(x, Damage.DamageItems.Tiamat));
                    if (targ != null)
                    {
                        Tiamat.Cast(null);
                    }
                }
                if (Hydra.item.IsReady())
                {
                    var targ =
                        HeroManager.Enemies.Find(
                            x =>
                                x.IsValidEnemy(Hydra.item.Range) &&
                                x.Health <= Yasuo.GetItemDamage(x, Damage.DamageItems.Tiamat));
                    if (targ != null)
                    {
                        Hydra.Cast(null);
                    }
                }
                if (Blade.item.IsReady())
                {
                    var targ = HeroManager.Enemies.Find(
                        x =>
                            x.IsValidEnemy(Blade.item.Range) &&
                            x.Health <= Yasuo.GetItemDamage(x, Damage.DamageItems.Botrk));
                    if (targ != null)
                    {
                        Blade.Cast(targ);
                    }
                }
                if (Bilgewater.item.IsReady())
                {
                    var targ = HeroManager.Enemies.Find(
                        x =>
                            x.IsValidEnemy(Bilgewater.item.Range) &&
                            x.Health <= Yasuo.GetItemDamage(x, Damage.DamageItems.Bilgewater));
                    if (targ != null)
                    {
                        Bilgewater.Cast(targ);
                    }
                }
            }
        }

        private void Harass()
        {
            //No harass under enemy turret to avoid aggro
            if (Yasuo.ServerPosition.PointUnderEnemyTurret())
            {
                return;
            }

            var target = TargetSelector.GetTarget(Spells[Q2].Range, DamageType.Physical);
            if (SpellSlot.Q.IsReady() && target != null && target.IsInRange(Qrange))
            {
                UseQ(target, GetHitChance("Hitchance.Q"), YasuoMenu.getCheckBoxItem(YasuoMenu.HarassA, "Harass.UseQ"),
                    YasuoMenu.getCheckBoxItem(YasuoMenu.HarassA, "Harass.UseQ2"));
            }

            if (target != null && isHealthy && YasuoMenu.getCheckBoxItem(YasuoMenu.HarassA, "Harass.UseE") &&
                Spells[E].IsReady() && target.IsInRange(Spells[E].Range*3) &&
                !target.Position.To2D().PointUnderEnemyTurret())
            {
                if (target.IsInRange(Spells[E].Range))
                {
                    ETarget = target;
                    Spells[E].CastOnUnit(target);
                    return;
                }

                var minion =
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(x => x.IsDashable() && !x.ServerPosition.To2D().PointUnderEnemyTurret())
                        .OrderBy(x => GetDashPos(x).Distance(target.ServerPosition))
                        .FirstOrDefault();

                if (minion != null && YasuoMenu.getCheckBoxItem(YasuoMenu.HarassA, "Harass.UseEMinion") &&
                    GetDashPos(minion).IsCloser(target))
                {
                    ETarget = minion;
                    Spells[E].Cast(minion);
                }
            }
        }

        private void Mixed()
        {
            if (YasuoMenu.getCheckBoxItem(YasuoMenu.HarassA, "Harass.InMixed"))
            {
                Harass();
            }
            LHSkills();
        }

        private void LHSkills()
        {
            if (SpellSlot.Q.IsReady() && !Yasuo.IsDashing())
            {
                if (!TornadoReady && YasuoMenu.getCheckBoxItem(YasuoMenu.FarmingA, "Farm.UseQ"))
                {
                    var minion =
                        ObjectManager.Get<Obj_AI_Minion>()
                            .FirstOrDefault(x => x.IsValidMinion(Spells[Q].Range) && x.QCanKill());
                    if (minion != null)
                    {
                        Spells[Q].Cast(minion.ServerPosition);
                    }
                }

                else if (TornadoReady && YasuoMenu.getCheckBoxItem(YasuoMenu.FarmingA, "Farm.UseQ2"))
                {
                    var minions =
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(
                                x =>
                                    x.Distance(Yasuo) > Yasuo.AttackRange && x.IsValidMinion(Spells[Q2].Range) &&
                                    x.QCanKill());
                    var pred =
                        MinionManager.GetBestLineFarmLocation(minions.Select(m => m.ServerPosition.To2D()).ToList(),
                            Spells[Q2].Width, Spells[Q2].Range);
                    if (pred.MinionsHit >= YasuoMenu.getSliderItem(YasuoMenu.FarmingA, "Farm.Qcount"))
                    {
                        Spells[Q2].Cast(pred.Position);
                    }
                }
            }

            if (Spells[E].IsReady() && YasuoMenu.getCheckBoxItem(YasuoMenu.FarmingA, "Farm.UseE"))
            {
                var minion =
                    ObjectManager.Get<Obj_AI_Minion>()
                        .FirstOrDefault(
                            x =>
                                x.IsDashable() && x.ECanKill() &&
                                (YasuoMenu.getCheckBoxItem(YasuoMenu.WaveclearA, "Waveclear.ETower") ||
                                 YasuoMenu.getKeyBindItem(YasuoMenu.MiscA, "Misc.TowerDive") ||
                                 !GetDashPos(x).PointUnderEnemyTurret()));
                if (minion != null)
                {
                    Spells[E].Cast(minion);
                }
            }
        }


        private void OnGapClose(ActiveGapcloser args)
        {
            if (Yasuo.ServerPosition.PointUnderEnemyTurret())
            {
                return;
            }
            if (YasuoMenu.getCheckBoxItem(YasuoMenu.MiscA, "Misc.AG") && TornadoReady && Yasuo.Distance(args.End) <= 500)
            {
                var pred = Spells[Q2].GetPrediction(args.Sender);
                if (pred.Hitchance >= GetHitChance("Hitchance.Q"))
                {
                    Spells[Q2].Cast(pred.CastPosition);
                }
            }
        }

        private void OnInterruptable(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Yasuo.ServerPosition.PointUnderEnemyTurret())
            {
                return;
            }
            if (YasuoMenu.getCheckBoxItem(YasuoMenu.MiscA, "Misc.Interrupter") && TornadoReady &&
                Yasuo.Distance(sender.ServerPosition) <= 500)
            {
                if (args.EndTime >= Spells[Q2].Delay)
                {
                    Spells[Q2].Cast(sender.ServerPosition);
                }
            }
        }
    }
}