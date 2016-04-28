using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SharpDX;
using UnderratedAIO.Helpers;
using Color = System.Drawing.Color;
using Damage = LeagueSharp.Common.Damage;
using Environment = System.Environment;
using Prediction = LeagueSharp.Common.Prediction;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

namespace UnderratedAIO.Champions
{
    internal class Gangplank
    {
        public const int BarrelExplosionRange = 325;
        public const int BarrelConnectionRange = 660;
        public static Menu config, drawMenu, comboMenu, harassMenu, laneClearMenu, miscMenu;
        public static Spell Q, W, E, R;
        public static readonly AIHeroClient player = ObjectManager.Player;
        public static bool justQ, justE;
        public static Vector3 ePos;
        public static List<Barrel> savedBarrels = new List<Barrel>();
        public static IncomingDamage IncDamages = new IncomingDamage();
        public static double[] Rwave = {50, 70, 90};
        public static double[] EDamage = {60, 90, 120, 150, 180};

        public static void OnLoad()
        {
            InitGangPlank();
            InitMenu();
            Drawing.OnDraw += Game_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += Game_ProcessSpell;
            GameObject.OnCreate += GameObjectOnOnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
        }


        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            for (var i = 0; i < savedBarrels.Count; i++)
            {
                if (savedBarrels[i].barrel.NetworkId == sender.NetworkId || savedBarrels[i].barrel.IsDead)
                {
                    savedBarrels.RemoveAt(i);
                    return;
                }
            }
        }

        private static void GameObjectOnOnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Barrel")
            {
                savedBarrels.Add(new Barrel(sender as Obj_AI_Minion, Environment.TickCount));
            }
        }

        private static IEnumerable<Obj_AI_Minion> GetBarrels()
        {
            return savedBarrels.Select(b => b.barrel).Where(b => b.IsValid);
        }

        private static bool KillableBarrel(Obj_AI_Base targetB,
            bool melee = false,
            AIHeroClient sender = null,
            float missileTravelTime = -1)
        {
            if (targetB.Health < 2)
            {
                return true;
            }
            if (sender == null)
            {
                sender = player;
            }
            if (missileTravelTime == -1)
            {
                missileTravelTime = GetQTime(targetB);
            }
            var barrel = savedBarrels.FirstOrDefault(b => b.barrel.NetworkId == targetB.NetworkId);
            if (barrel != null)
            {
                var time = targetB.Health*getEActivationDelay()*1000;
                if (Environment.TickCount - barrel.time + (melee ? sender.AttackDelay : missileTravelTime)*1000 >
                    time)
                {
                    return true;
                }
            }
            return false;
        }

        private static float GetQTime(Obj_AI_Base targetB)
        {
            return player.Distance(targetB)/2800f + Q.Delay;
        }

        private static void InitGangPlank()
        {
            Q = new Spell(SpellSlot.Q, 590f); //2600f
            Q.SetTargetted(0.25f, 2200f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 950);
            E.SetSkillshot(0.8f, 50, float.MaxValue, false, SkillshotType.SkillshotCircle);
            R = new Spell(SpellSlot.R);
            R.SetSkillshot(1f, 100, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            Orbwalker.DisableMovement = false;
            Orbwalker.DisableAttacking = false;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Clear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                if (getCheckBoxItem(harassMenu, "useqLHH") && !justE)
                {
                    Lasthit();
                }
            }

            if (getCheckBoxItem(miscMenu, "AutoR") && R.IsReady())
            {
                foreach (
                    var enemy in
                        HeroManager.Enemies.Where(
                            e =>
                                ((e.UnderTurret(true) &&
                                  e.MaxHealth/100*getSliderItem(miscMenu, "Rhealt")*0.75f >
                                  e.Health - IncDamages.GetEnemyData(e.NetworkId).DamageTaken) ||
                                 (!e.UnderTurret(true) &&
                                  e.MaxHealth/100*getSliderItem(miscMenu, "Rhealt") >
                                  e.Health - IncDamages.GetEnemyData(e.NetworkId).DamageTaken)) &&
                                e.HealthPercent > getSliderItem(miscMenu, "RhealtMin") && e.IsValidTarget() &&
                                e.Distance(player) > 1500))
                {
                    var pred = IncDamages.GetEnemyData(enemy.NetworkId);
                    if (pred != null && pred.DamageTaken < enemy.Health)
                    {
                        var ally =
                            HeroManager.Allies.OrderBy(a => a.Health).FirstOrDefault(a => enemy.Distance(a) < 1000);
                        if (ally != null)
                        {
                            var pos = Prediction.GetPrediction(enemy, 0.75f);
                            if (pos.CastPosition.Distance(enemy.Position) < 450 && pos.Hitchance >= HitChance.VeryHigh)
                            {
                                if (enemy.IsMoving)
                                {
                                    R.Cast(enemy.Position.Extend(pos.CastPosition, 450));
                                }
                                else
                                {
                                    R.Cast(enemy.ServerPosition);
                                }
                            }
                        }
                    }
                }
            }
            if (getKeyBindItem(comboMenu, "EQtoCursor") && E.IsReady() && Q.IsReady())
            {
                Orbwalker.DisableMovement = true;
                var barrel =
                    GetBarrels()
                        .Where(
                            o =>
                                o.IsValid && !o.IsDead && o.Distance(player) < Q.Range &&
                                o.BaseSkinName == "GangplankBarrel" && o.GetBuff("gangplankebarrellife").Caster.IsMe &&
                                KillableBarrel(o))
                        .OrderBy(o => o.Distance(Game.CursorPos))
                        .FirstOrDefault();
                if (barrel != null)
                {
                    var cp = Game.CursorPos;
                    var cursorPos = barrel.Distance(cp) > BarrelConnectionRange
                        ? barrel.Position.LSExtend(cp, BarrelConnectionRange)
                        : cp;
                    var points =
                        CombatHelper.PointsAroundTheTarget(player.Position, E.Range - 200)
                            .Where(p => p.Distance(player.Position) < E.Range);
                    var middle = GetMiddleBarrel(barrel, points, cursorPos);
                    var threeBarrel = cursorPos.Distance(cp) > BarrelExplosionRange && E.Instance.Ammo >= 2 &&
                                      Game.CursorPos.Distance(player.Position) < E.Range && middle.IsValid();
                    var firsDelay = threeBarrel ? 500 : 1;
                    if (cursorPos.IsValid() && cursorPos.Distance(player.Position) < E.Range)
                    {
                        E.Cast(threeBarrel ? middle : cursorPos);
                        Utility.DelayAction.Add(firsDelay, () => Q.CastOnUnit(barrel));
                        if (threeBarrel)
                        {
                            if (player.IsMoving)
                            {
                                Player.IssueOrder(GameObjectOrder.Stop, player.Position);
                            }
                            Utility.DelayAction.Add(801, () => E.Cast(middle.Extend(cp, BarrelConnectionRange)));
                        }
                        else
                        {
                            Orbwalker.DisableMovement = false;
                            Orbwalker.OrbwalkTo(Game.CursorPos);
                        }
                    }
                    else
                    {
                        Orbwalker.DisableMovement = false;
                        Orbwalker.OrbwalkTo(Game.CursorPos);
                    }
                }
            }
            else if (getKeyBindItem(comboMenu, "EQtoCursor"))
            {
                Orbwalker.DisableMovement = false;
                Orbwalker.OrbwalkTo(Game.CursorPos);
            }
            if (getKeyBindItem(comboMenu, "QbarrelCursor") && Q.IsReady())
            {
                var meleeRangeBarrel =
                    GetBarrels()
                        .OrderBy(o => o.Distance(Game.CursorPos))
                        .FirstOrDefault(
                            o =>
                                o.Health > 1 && o.Distance(player) < Orbwalking.GetRealAutoAttackRange(o) &&
                                !KillableBarrel(o, true));
                if (meleeRangeBarrel != null && Orbwalker.CanAutoAttack)
                {
                    Orbwalker.DisableAttacking = true;
                    Player.IssueOrder(GameObjectOrder.AttackUnit, meleeRangeBarrel);
                    return;
                }
                var barrel =
                    GetBarrels()
                        .Where(
                            o =>
                                o.IsValid && !o.IsDead && o.Distance(player) < Q.Range &&
                                o.BaseSkinName == "GangplankBarrel" && o.GetBuff("gangplankebarrellife").Caster.IsMe &&
                                KillableBarrel(o))
                        .OrderBy(o => o.Distance(Game.CursorPos))
                        .FirstOrDefault();
                if (barrel != null)
                {
                    Q.CastOnUnit(barrel);
                }
            }

            if (getCheckBoxItem(miscMenu, "AutoQBarrel") && Q.IsReady())
            {
                var barrel =
                    GetBarrels()
                        .FirstOrDefault(
                            o =>
                                o.IsValid && !o.IsDead && o.Distance(player) < Q.Range &&
                                o.BaseSkinName == "GangplankBarrel" && o.GetBuff("gangplankebarrellife").Caster.IsMe &&
                                KillableBarrel(o) &&
                                (o.CountEnemiesInRange(BarrelExplosionRange - 50) > 0 ||
                                 HeroManager.Enemies.Count(
                                     e =>
                                         e.IsValidTarget() &&
                                         Prediction.GetPrediction(e, 0.1f).UnitPosition.Distance(o.Position) <
                                         BarrelExplosionRange - 20) > 0));

                if (barrel != null)
                {
                    Q.Cast(barrel);
                }
            }
        }

        private static Vector3 GetMiddleBarrel(Obj_AI_Minion barrel, IEnumerable<Vector3> points, Vector3 cursorPos)
        {
            var middle =
                points.Where(
                    p =>
                        !p.IsWall() && p.Distance(barrel.Position) < BarrelConnectionRange &&
                        p.Distance(barrel.Position) > BarrelExplosionRange &&
                        p.Distance(cursorPos) < BarrelConnectionRange && p.Distance(cursorPos) > BarrelExplosionRange &&
                        p.Distance(barrel.Position) + p.Distance(cursorPos) > BarrelExplosionRange*2 - 100)
                    .OrderByDescending(p => p.CountEnemiesInRange(BarrelExplosionRange))
                    .ThenByDescending(p => p.Distance(barrel.Position))
                    .FirstOrDefault();
            return middle;
        }

        private static void Lasthit()
        {
            if (Q.IsReady())
            {
                var mini =
                    MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.NotAlly)
                        .Where(m => m.Health < Q.GetDamage(m) && m.BaseSkinName != "GangplankBarrel")
                        .OrderByDescending(m => m.MaxHealth)
                        .ThenByDescending(m => m.Distance(player))
                        .FirstOrDefault();

                if (mini != null && !justE)
                {
                    Q.CastOnUnit(mini, getCheckBoxItem(config, "packets"));
                }
            }
        }


        private static void Harass()
        {
            var perc = getSliderItem(harassMenu, "minmanaH")/100f;
            if (player.Mana < player.MaxMana*perc)
            {
                return;
            }
            var target = TargetSelector.GetTarget(
                Q.Range + BarrelExplosionRange, DamageType.Physical);
            var barrel =
                GetBarrels()
                    .FirstOrDefault(
                        o =>
                            target != null && o.IsValid && !o.IsDead && o.Distance(player) < Q.Range &&
                            o.BaseSkinName == "GangplankBarrel" && o.GetBuff("gangplankebarrellife").Caster.IsMe &&
                            KillableBarrel(o) && o.Distance(target) < BarrelExplosionRange);

            if (barrel != null)
            {
                Q.CastOnUnit(barrel, getCheckBoxItem(config, "packets"));
                return;
            }
            if (getCheckBoxItem(harassMenu, "useqLHH"))
            {
                var mini =
                    MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.NotAlly)
                        .Where(m => m.Health < Q.GetDamage(m) && m.BaseSkinName != "GangplankBarrel")
                        .OrderByDescending(m => m.MaxHealth)
                        .ThenByDescending(m => m.Distance(player))
                        .FirstOrDefault();

                if (mini != null)
                {
                    Q.CastOnUnit(mini, getCheckBoxItem(config, "packets"));
                    return;
                }
            }

            if (target == null)
            {
                return;
            }
            if (getCheckBoxItem(harassMenu, "useqH") && Q.CanCast(target) && !justE)
            {
                var barrels =
                    GetBarrels()
                        .Where(
                            o =>
                                o.IsValid && !o.IsDead && o.Distance(player) < 1600 &&
                                o.BaseSkinName == "GangplankBarrel" &&
                                o.GetBuff("gangplankebarrellife").Caster.IsMe)
                        .ToList();
                CastQonHero(target, barrels);
            }
            if (getCheckBoxItem(harassMenu, "useeH") && Q.CanCast(target) &&
                getSliderItem(harassMenu, "eStacksH") < E.Instance.Ammo)
            {
                CastEtarget(target);
            }
        }

        private static void Clear()
        {
            var perc = getSliderItem(laneClearMenu, "minmana")/100f;
            if (player.Mana < player.MaxMana*perc)
            {
                return;
            }
            if (Q.IsReady() && Q.IsReady() && getCheckBoxItem(laneClearMenu, "useqLC"))
            {
                var barrel =
                    GetBarrels()
                        .FirstOrDefault(
                            o =>
                                o.IsValid && !o.IsDead && o.Distance(player) < Q.Range &&
                                o.BaseSkinName == "GangplankBarrel" && o.GetBuff("gangplankebarrellife").Caster.IsMe &&
                                KillableBarrel(o) &&
                                Helpers.Environment.Minion.countMinionsInrange(o.Position, BarrelExplosionRange) >=
                                getSliderItem(laneClearMenu, "eMinHit"));
                if (barrel != null)
                {
                    Q.CastOnUnit(barrel, getCheckBoxItem(config, "packets"));
                    return;
                }
            }
            if (getCheckBoxItem(laneClearMenu, "useqLC") && !justE)
            {
                Lasthit();
            }
            if (getCheckBoxItem(laneClearMenu, "useeLC") && E.IsReady() &&
                getSliderItem(laneClearMenu, "eStacksLC") < E.Instance.Ammo)
            {
                var bestPositionE =
                    E.GetCircularFarmLocation(
                        MinionManager.GetMinions(
                            ObjectManager.Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.NotAlly),
                        BarrelExplosionRange);

                if (bestPositionE.MinionsHit >= getSliderItem(laneClearMenu, "eMinHit") &&
                    bestPositionE.Position.Distance(ePos) > 400)
                {
                    E.Cast(bestPositionE.Position, getCheckBoxItem(config, "packets"));
                }
            }
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            if (target == null)
            {
                return;
            }
            var ignitedmg = (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            var hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            if (getCheckBoxItem(comboMenu, "useIgnite") &&
                ignitedmg > target.Health - IncDamages.GetEnemyData(target.NetworkId).DamageTaken && hasIgnite &&
                !CombatHelper.CheckCriticalBuffs(target) && !Q.IsReady() && !justQ)
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }
            if (getSliderItem(comboMenu, "usew") > player.HealthPercent &&
                player.CountEnemiesInRange(500) > 0)
            {
                W.Cast();
            }
            if (R.IsReady() && getCheckBoxItem(comboMenu, "user"))
            {
                var Rtarget =
                    HeroManager.Enemies.FirstOrDefault(e => e.HealthPercent < 50 && e.CountAlliesInRange(660) > 0);
                if (Rtarget != null)
                {
                    R.CastIfWillHit(Rtarget, getSliderItem(comboMenu, "Rmin"));
                }
            }
            var dontQ = false;
            var barrels =
                GetBarrels()
                    .Where(
                        o =>
                            o.IsValid && !o.IsDead && o.Distance(player) < 1600 && o.BaseSkinName == "GangplankBarrel" &&
                            o.GetBuff("gangplankebarrellife").Caster.IsMe)
                    .ToList();

            if (getCheckBoxItem(comboMenu, "useq") && Q.IsReady() && getCheckBoxItem(comboMenu, "usee") &&
                E.IsReady() && !justE)
            {
                var pred = Prediction.GetPrediction(target, 0.5f);
                if (pred.Hitchance >= HitChance.High)
                {
                    var Qbarrels = GetBarrels().Where(o => o.Distance(player) < Q.Range && KillableBarrel(o));
                    foreach (var Qbarrel in Qbarrels.OrderByDescending(b => b.Distance(target) < BarrelExplosionRange))
                    {
                        var targPred = Prediction.GetPrediction(target, GetQTime(Qbarrel));
                        if (Qbarrel.Distance(targPred.UnitPosition) < BarrelExplosionRange)
                        {
                            if (getCheckBoxItem(comboMenu, "useeAOE") && barrels.Count < 2)
                            {
                                var enemies =
                                    HeroManager.Enemies.Where(
                                        e => e.Distance(player) < 1600 && e.Distance(Qbarrel) > BarrelExplosionRange)
                                        .Select(e => Prediction.GetPrediction(e, 05f));
                                var pos =
                                    GetBarrelPoints(Qbarrel.Position)
                                        .Where(p => p.Distance(Qbarrel.Position) < BarrelConnectionRange)
                                        .OrderByDescending(
                                            p => enemies.Count(e => e.UnitPosition.Distance(p) < BarrelExplosionRange))
                                        .ThenBy(p => p.Distance(target.Position))
                                        .FirstOrDefault();
                                if (pos.IsValid() && pos.CountEnemiesInRange(BarrelExplosionRange) > 0 &&
                                    enemies.Count(e => e.UnitPosition.Distance(pos) < BarrelExplosionRange) > 0)
                                {
                                    dontQ = true;
                                    E.Cast(pos);
                                }
                            }
                            break;
                        }
                        var point =
                            GetBarrelPoints(Qbarrel.Position)
                                .Where(
                                    p =>
                                        p.IsValid() && !p.IsWall() && p.Distance(player.Position) < E.Range &&
                                        target.Distance(p) < BarrelExplosionRange &&
                                        p.Distance(targPred.UnitPosition) < BarrelExplosionRange &&
                                        Qbarrel.Distance(p) < BarrelConnectionRange &&
                                        savedBarrels.Count(b => b.barrel.Position.Distance(p) < BarrelExplosionRange) <
                                        1)
                                .OrderBy(p => p.Distance(pred.UnitPosition))
                                .FirstOrDefault();
                        if (point.IsValid())
                        {
                            dontQ = true;
                            E.Cast(point);
                            Utility.DelayAction.Add(1, () => Q.CastOnUnit(Qbarrel));
                            return;
                        }
                    }
                }
            }
            var meleeRangeBarrel =
                barrels.FirstOrDefault(
                    b =>
                        (b.Health < 2 || (b.Health == 2 && !KillableBarrel(b, true) && Q.IsReady() && !justQ)) &&
                        KillableBarrel(b, true) && b.Distance(player) < Orbwalking.GetRealAutoAttackRange(b));
            var secondb =
                barrels.Where(
                    b =>
                        b.Distance(meleeRangeBarrel) < BarrelConnectionRange &&
                        HeroManager.Enemies.Count(
                            o =>
                                o.IsValidTarget() && o.Distance(b) < BarrelExplosionRange &&
                                b.Distance(Prediction.GetPrediction(o, 500).UnitPosition) < BarrelExplosionRange) > 0);
            if (meleeRangeBarrel != null &&
                ((HeroManager.Enemies.Count(
                    o =>
                        o.IsValidTarget() && o.Distance(meleeRangeBarrel) < BarrelExplosionRange &&
                        meleeRangeBarrel.Distance(Prediction.GetPrediction(o, 500).UnitPosition) < BarrelExplosionRange) >
                  0) || secondb != null) && !Q.IsReady() && !justQ && Orbwalker.CanAutoAttack)
            {
                Orbwalker.DisableAttacking = true;
                Player.IssueOrder(GameObjectOrder.AttackUnit, meleeRangeBarrel);
            }
            if (Q.IsReady())
            {
                if (barrels.Any())
                {
                    var detoneateTargetBarrels = barrels.Where(b => b.Distance(player) < Q.Range);
                    if (getCheckBoxItem(comboMenu, "detoneateTarget"))
                    {
                        if (detoneateTargetBarrels.Any())
                        {
                            foreach (var detoneateTargetBarrel in detoneateTargetBarrels)
                            {
                                if (!KillableBarrel(detoneateTargetBarrel))
                                {
                                    continue;
                                }
                                if (
                                    detoneateTargetBarrel.Distance(
                                        Prediction.GetPrediction(target, GetQTime(detoneateTargetBarrel)).UnitPosition) <
                                    BarrelExplosionRange &&
                                    target.Distance(detoneateTargetBarrel.Position) < BarrelExplosionRange)
                                {
                                    dontQ = true;
                                    Q.CastOnUnit(detoneateTargetBarrel, getCheckBoxItem(config, "packets"));
                                    return;
                                }
                                var detoneateTargetBarrelSeconds =
                                    barrels.Where(b => b.Distance(detoneateTargetBarrel) < BarrelConnectionRange);
                                if (detoneateTargetBarrelSeconds.Any())
                                {
                                    if (detoneateTargetBarrelSeconds.Any(detoneateTargetBarrelSecond => detoneateTargetBarrelSecond.Distance(
                                        Prediction.GetPrediction(
                                            target, GetQTime(detoneateTargetBarrel) + 0.15f).UnitPosition) <
                                                                                                        BarrelExplosionRange &&
                                                                                                        target.Distance(detoneateTargetBarrelSecond.Position) < BarrelExplosionRange))
                                    {
                                        dontQ = true;
                                        Q.CastOnUnit(detoneateTargetBarrel, getCheckBoxItem(config, "packets"));
                                        return;
                                    }
                                }
                            }
                        }

                        if (getSliderItem(comboMenu, "detoneateTargets") > 1)
                        {
                            var enemies =
                                HeroManager.Enemies.Where(e => e.IsValidTarget() && e.Distance(player) < 600)
                                    .Select(e => Prediction.GetPrediction(e, 0.25f));
                            var enemies2 =
                                HeroManager.Enemies.Where(e => e.IsValidTarget() && e.Distance(player) < 600)
                                    .Select(e => Prediction.GetPrediction(e, 0.35f));
                            if (detoneateTargetBarrels.Any())
                            {
                                foreach (var detoneateTargetBarrel in detoneateTargetBarrels)
                                {
                                    if (!KillableBarrel(detoneateTargetBarrel))
                                    {
                                        continue;
                                    }
                                    var enemyCount =
                                        enemies.Count(
                                            e =>
                                                e.UnitPosition.Distance(detoneateTargetBarrel.Position) <
                                                BarrelExplosionRange);
                                    if (enemyCount >= getSliderItem(comboMenu, "detoneateTargets") &&
                                        detoneateTargetBarrel.CountEnemiesInRange(BarrelExplosionRange) >=
                                        getSliderItem(comboMenu, "detoneateTargets"))
                                    {
                                        dontQ = true;
                                        Q.CastOnUnit(detoneateTargetBarrel, getCheckBoxItem(config, "packets"));
                                        return;
                                    }
                                    var detoneateTargetBarrelSeconds =
                                        barrels.Where(b => b.Distance(detoneateTargetBarrel) < BarrelConnectionRange);
                                    if (detoneateTargetBarrelSeconds.Any())
                                    {
                                        if (detoneateTargetBarrelSeconds.Any(detoneateTargetBarrelSecond => enemyCount +
                                                                                                            enemies2.Count(
                                                                                                                e =>
                                                                                                                    e.UnitPosition.Distance(detoneateTargetBarrelSecond.Position) <
                                                                                                                    BarrelExplosionRange) >=
                                                                                                            getSliderItem(comboMenu, "detoneateTargets") &&
                                                                                                            detoneateTargetBarrelSecond.CountEnemiesInRange(BarrelExplosionRange) >=
                                                                                                            getSliderItem(comboMenu, "detoneateTargets")))
                                        {
                                            dontQ = true;
                                            Q.CastOnUnit(
                                                detoneateTargetBarrel, getCheckBoxItem(config, "packets"));
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (getCheckBoxItem(comboMenu, "useeAlways") && E.IsReady() && player.Distance(target) < E.Range &&
                    !justE && target.Health > Q.GetDamage(target) + player.GetAutoAttackDamage(target) &&
                    getSliderItem(comboMenu, "eStacksC") < E.Instance.Ammo)
                {
                    CastE(target, barrels);
                }
                var Qbarrels = GetBarrels().FirstOrDefault(o => o.Distance(player) < Q.Range);
                if (Qbarrels != null && E.Instance.Ammo > 0 && Q.IsReady() && getCheckBoxItem(comboMenu, "usee") &&
                    target.Health > Q.GetDamage(target))
                {
                    dontQ = true;
                }
                if (getCheckBoxItem(comboMenu, "useq") && Q.CanCast(target) && !justE &&
                    !dontQ)
                {
                    CastQonHero(target, barrels);
                }
            }
        }

        private static void CastQonHero(AIHeroClient target, List<Obj_AI_Minion> barrels)
        {
            if (barrels.FirstOrDefault(b => target.Distance(b.Position) < BarrelExplosionRange) != null &&
                target.Health > Q.GetDamage(target))
            {
                return;
            }
            Q.CastOnUnit(target, getCheckBoxItem(config, "packets"));
        }

        private static void CastE(AIHeroClient target, List<Obj_AI_Minion> barrels)
        {
            if (barrels.Count(b => b.CountEnemiesInRange(BarrelConnectionRange) > 0) < 1)
            {
                if (getCheckBoxItem(comboMenu, "useeAlways"))
                {
                    CastEtarget(target);
                }
                return;
            }
            var enemies =
                HeroManager.Enemies.Where(e => e.IsValidTarget() && e.Distance(player) < E.Range)
                    .Select(e => Prediction.GetPrediction(e, 0.35f));
            var points = new List<Vector3>();
            foreach (var barrel in
                barrels.Where(b => b.Distance(player) < Q.Range && KillableBarrel(b)))
            {
                if (barrel != null)
                {
                    var newP = GetBarrelPoints(barrel.Position).Where(p => !p.IsWall());
                    if (newP.Any())
                    {
                        points.AddRange(newP.Where(p => p.Distance(player.Position) < E.Range));
                    }
                }
            }
            var bestPoint =
                points.Where(b => enemies.Count(e => e.UnitPosition.Distance(b) < BarrelExplosionRange) > 0)
                    .OrderByDescending(b => enemies.Count(e => e.UnitPosition.Distance(b) < BarrelExplosionRange))
                    .FirstOrDefault();
            if (bestPoint.IsValid() &&
                !savedBarrels.Any(b => b.barrel.Position.Distance(bestPoint) < BarrelConnectionRange))
            {
                E.Cast(bestPoint, getCheckBoxItem(config, "packets"));
            }
        }

        private static void CastEtarget(AIHeroClient target)
        {
            var ePred = Prediction.GetPrediction(target, 1);
            var pos = target.Position.Extend(ePred.CastPosition, BarrelExplosionRange);
            if (pos.Distance(ePos) > 400 && !justE)
            {
                E.Cast(pos, getCheckBoxItem(config, "packets"));
            }
        }

        private static void Game_OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(drawMenu, "drawqq"))
            {
                Render.Circle.DrawCircle(player.Position, Q.Range, Color.FromArgb(180, 100, 146, 166));
            }

            if (getCheckBoxItem(drawMenu, "drawee"))
            {
                Render.Circle.DrawCircle(player.Position, E.Range, Color.FromArgb(180, 100, 146, 166));
            }

            if (getCheckBoxItem(drawMenu, "drawW"))
            {
                if (W.IsReady() && player.HealthPercent < 100)
                {
                    var Heal = new[] {50, 75, 100, 125, 150}[W.Level - 1] +
                               (player.MaxHealth - player.Health)*0.15f + player.FlatMagicDamageMod*0.9f;
                    var mod = Math.Max(100f, player.Health + Heal)/player.MaxHealth;
                    var xPos = (float) ((double) player.HPBarPosition.X + 36 + 103.0*mod);
                    Drawing.DrawLine(
                        xPos, player.HPBarPosition.Y + 8, xPos, (float) ((double) player.HPBarPosition.Y + 17), 2f,
                        Color.Coral);
                }
            }
            if (getBoxItem(drawMenu, "drawKillableSL") != 0 && R.IsReady())
            {
                var text = (from enemy in HeroManager.Enemies.Where(e => e.IsValidTarget()) where getRDamage(enemy) > enemy.Health select enemy.ChampionName + "(" + Math.Ceiling(enemy.Health/Rwave[R.Level - 1]) + " wave)").ToList();
                if (text.Count > 0)
                {
                    var result = string.Join(", ", text);
                    switch (getBoxItem(drawMenu, "drawKillableSL"))
                    {
                        case 2:
                            drawText(2, result);
                            break;
                        case 1:
                            drawText(1, result);
                            break;
                        default:
                            return;
                    }
                }
            }

            if (Q.IsReady() && getCheckBoxItem(drawMenu, "drawEQ"))
            {
                var points =
                    CombatHelper.PointsAroundTheTarget(player.Position, E.Range - 200)
                        .Where(p => p.Distance(player.Position) < E.Range);


                var barrel =
                    GetBarrels()
                        .Where(
                            o =>
                                o.IsValid && !o.IsDead && o.Distance(player) < Q.Range &&
                                o.BaseSkinName == "GangplankBarrel" && o.GetBuff("gangplankebarrellife").Caster.IsMe &&
                                KillableBarrel(o))
                        .OrderBy(o => o.Distance(Game.CursorPos))
                        .FirstOrDefault();
                if (barrel != null)
                {
                    var cp = Game.CursorPos;
                    var cursorPos = barrel.Distance(cp) > BarrelConnectionRange
                        ? barrel.Position.LSExtend(cp, BarrelConnectionRange)
                        : cp;
                    var cursorPos2 = cursorPos.Distance(cp) > BarrelConnectionRange
                        ? cursorPos.LSExtend(cp, BarrelConnectionRange)
                        : cp;
                    var middle = GetMiddleBarrel(barrel, points, cursorPos);
                    var threeBarrel = cursorPos.Distance(cp) > BarrelExplosionRange && E.Instance.Ammo >= 2 &&
                                      cursorPos2.Distance(player.Position) < E.Range && middle.IsValid();
                    if (threeBarrel)
                    {
                        Render.Circle.DrawCircle(
                            middle.LSExtend(cp, BarrelConnectionRange), BarrelExplosionRange, Color.DarkOrange, 6);
                        Render.Circle.DrawCircle(middle, BarrelExplosionRange, Color.DarkOrange, 6);
                        Drawing.DrawLine(
                            Drawing.WorldToScreen(barrel.Position),
                            Drawing.WorldToScreen(middle.LSExtend(barrel.Position, BarrelExplosionRange)), 2,
                            Color.DarkOrange);
                    }
                    else if (E.Instance.Ammo >= 1)
                    {
                        Drawing.DrawLine(
                            Drawing.WorldToScreen(barrel.Position),
                            Drawing.WorldToScreen(cursorPos.LSExtend(barrel.Position, BarrelExplosionRange)), 2,
                            Color.DarkOrange);
                        Render.Circle.DrawCircle(cursorPos, BarrelExplosionRange, Color.DarkOrange, 6);
                    }
                }
            }
            if (getCheckBoxItem(drawMenu, "drawWcd"))
            {
                foreach (var barrelData in savedBarrels)
                {
                    var time =
                        Math.Min(
                            Environment.TickCount - barrelData.time -
                            barrelData.barrel.Health*getEActivationDelay()*1000f, 0)/1000f;
                    if (time < 0)
                    {
                        Drawing.DrawText(
                            barrelData.barrel.HPBarPosition.X - time.ToString().Length*5 + 40,
                            barrelData.barrel.HPBarPosition.Y - 20, Color.DarkOrange,
                            string.Format("{0:0.00}", time).Replace("-", ""));
                    }
                }
            }

            if (getCheckBoxItem(drawMenu, "drawEmini"))
            {
                var barrels =
                    GetBarrels()
                        .Where(
                            o =>
                                o.IsValid && !o.IsDead && o.Distance(player) < E.Range &&
                                o.BaseSkinName == "GangplankBarrel" && o.GetBuff("gangplankebarrellife").Caster.IsMe);
                foreach (var b in barrels)
                {
                    var minis = MinionManager.GetMinions(
                        b.Position, BarrelExplosionRange, MinionTypes.All, MinionTeam.NotAlly);
                    foreach (var m in
                        minis.Where(e => Q.GetDamage(e) >= e.Health && e.Health > 3))
                    {
                        Render.Circle.DrawCircle(m.Position, 45, Color.Yellow, 7);
                    }
                }
            }
        }

        public static void drawText(int mode, string result)
        {
            const string baseText = "Killable with R: ";
            if (mode == 1)
            {
                Drawing.DrawText(
                    Drawing.Width/2 - (baseText + result).Length*5, Drawing.Height*0.75f, Color.Red,
                    baseText + result);
            }
            else
            {
                Drawing.DrawText(
                    player.HPBarPosition.X - (baseText + result).Length*5 + 110, player.HPBarPosition.Y + 250,
                    Color.Red, baseText + result);
            }
        }

        private static float getRDamage(AIHeroClient enemy)
        {
            return
                (float)
                    player.CalcDamage(enemy, DamageType.Magical,
                        (Rwave[R.Level - 1] + 0.1*player.FlatMagicDamageMod)*waveLength());
        }

        public static int waveLength()
        {
            if (player.HasBuff("GangplankRUpgrade1"))
            {
                return 18;
            }
            return 12;
        }

        private static void Game_ProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name == "GangplankQWrapper")
                {
                    if (!justQ)
                    {
                        justQ = true;
                        Utility.DelayAction.Add(200, () => justQ = false);
                    }
                }
                if (args.SData.Name == "GangplankE")
                {
                    ePos = args.End;
                    if (!justE)
                    {
                        justE = true;
                        Utility.DelayAction.Add(500, () => justE = false);
                    }
                }
            }
            if (sender.IsEnemy && args.Target != null && sender is AIHeroClient && sender.Distance(player) < E.Range)
            {
                var targetBarrels =
                    savedBarrels.Where(
                        b =>
                            b.barrel.NetworkId == args.Target.NetworkId &&
                            KillableBarrel(
                                b.barrel, sender.IsMelee, (AIHeroClient) sender,
                                sender.Distance(b.barrel)/args.SData.MissileSpeed));
                foreach (var barrelData in targetBarrels)
                {
                    savedBarrels.Remove(barrelData);
                    Console.WriteLine("Barrel removed");
                }
            }
        }

        private static IEnumerable<Vector3> GetBarrelPoints(Vector3 point)
        {
            return
                CombatHelper.PointsAroundTheTarget(point, BarrelConnectionRange, 20f)
                    .Where(p => p.Distance(point) > BarrelExplosionRange);
        }

        private static float getEActivationDelay()
        {
            if (player.Level >= 13)
            {
                return 0.5f;
            }
            if (player.Level >= 7)
            {
                return 1f;
            }
            return 2f;
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

        private static void InitMenu()
        {
            config = MainMenu.AddMenu("Gangplank ", "Gangplank");

            // Draw settings
            drawMenu = config.AddSubMenu("Drawings ", "dsettings");
            drawMenu.Add("drawqq", new CheckBox("Draw Q range"));
            drawMenu.Add("drawW", new CheckBox("Draw W"));
            drawMenu.Add("drawee", new CheckBox("Draw E range"));
            drawMenu.Add("drawWcd", new CheckBox("Draw E countdown"));
            drawMenu.Add("drawEmini", new CheckBox("Draw killable minions around E"));
            drawMenu.Add("drawEQ", new CheckBox("Draw EQ to cursor"));
            drawMenu.Add("drawKillableSL",
                new ComboBox("Show killable targets with R", 1, "OFF", "Above HUD", "Under GP"));

            // Combo Settings
            comboMenu = config.AddSubMenu("Combo ", "csettings");
            comboMenu.Add("useq", new CheckBox("Use Q"));
            comboMenu.Add("detoneateTarget", new CheckBox("Blow up target with E"));
            comboMenu.Add("detoneateTargets", new Slider("Blow up enemies with E", 2, 1, 5));
            comboMenu.Add("usew", new Slider("Use W under health", 20));
            comboMenu.Add("useeAlways", new CheckBox("Use E always", false));
            comboMenu.Add("eStacksC", new Slider("E : Keep stacks", 0, 0, 5));
            comboMenu.Add("usee", new CheckBox("Use E to extend range"));
            comboMenu.Add("useeAOE", new CheckBox("Put bonus barrel to AOE", false));
            comboMenu.Add("EQtoCursor", new KeyBind("EQ to cursor", false, KeyBind.BindTypes.HoldActive, 'T'));
            comboMenu.Add("QbarrelCursor", new KeyBind("Q barrel at cursor", false, KeyBind.BindTypes.HoldActive, 'H'));
            comboMenu.Add("user", new CheckBox("Use R"));
            comboMenu.Add("Rmin", new Slider("R min", 2, 1, 5));
            comboMenu.Add("useIgnite", new CheckBox("Use Ignite"));

            // Harass Settings
            harassMenu = config.AddSubMenu("Harass ", "Hsettings");
            harassMenu.Add("useqH", new CheckBox("Use Q harass"));
            harassMenu.Add("useqLHH", new CheckBox("Use Q lasthit"));
            harassMenu.Add("useeH", new CheckBox("Use E"));
            harassMenu.Add("eStacksH", new Slider("Keep stacks", 0, 0, 5));
            harassMenu.Add("minmanaH", new Slider("Keep X% mana", 1, 1));

            // LaneClear Settings
            laneClearMenu = config.AddSubMenu("LaneClear ", "Lcsettings");
            laneClearMenu.Add("useqLC", new CheckBox("Use Q"));
            laneClearMenu.Add("useeLC", new CheckBox("Use E"));
            laneClearMenu.Add("eMinHit", new Slider("Q : Min hit", 3, 1, 6));
            laneClearMenu.Add("eStacksLC", new Slider("Q : Keep stacks", 0, 0, 5));
            laneClearMenu.Add("minmana", new Slider("Keep X% mana", 1, 1));

            // Misc Settings
            miscMenu = config.AddSubMenu("Misc ", "Msettings");
            miscMenu.Add("AutoR", new CheckBox("Cast R to get assists", false));
            miscMenu.Add("Rhealt", new Slider("R : Enemy health %", 35));
            miscMenu.Add("RhealtMin", new Slider(" R : Enemy min health %", 10));
            miscMenu.Add("AutoW", new CheckBox("W with QSS options"));
            miscMenu.Add("AutoQBarrel", new CheckBox("AutoQ barrel near enemies", false));

            config.Add("packets", new CheckBox("Use Packets", false));
        }
    }

    internal class Barrel
    {
        public Obj_AI_Minion barrel;
        public float time;

        public Barrel(Obj_AI_Minion objAiBase, int tickCount)
        {
            barrel = objAiBase;
            time = tickCount;
        }
    }
}