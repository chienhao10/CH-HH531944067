namespace Jinx
{
    using System;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;

    /// <summary>
    /// Class that executes Kill Steal and Jungle Steal
    /// </summary>
    internal class ActiveStates
    {
        /// <summary>
        /// Called every time the Game Ticks
        /// </summary>
        /// <param name="args">The Args</param>
        public static void Game_OnUpdate(EventArgs args)
        {
            if (Player.Instance.IsDead || Player.Instance.HasBuff("Recall")
                || Player.Instance.IsStunned || Player.Instance.IsRooted || Player.Instance.IsCharmed ||
                Orbwalker.IsAutoAttacking)
            {
                return;
            }

            var toggleK = Config.KillStealMenu["toggle"].Cast<CheckBox>().CurrentValue;
            var toggleJ = Config.JungleStealMenu["toggle"].Cast<CheckBox>().CurrentValue;
            var toggleaW = Config.MiscMenu["autoW"].Cast<CheckBox>().CurrentValue;
            var toggleaE = Config.MiscMenu["autoE"].Cast<CheckBox>().CurrentValue;

            if (toggleK)
            {
                WR_KillSteal();
                StandaloneKillSteal();
            }

            if (toggleJ)
            {
                JungleSteal();
            }

            if (toggleaW)
            {
                AutoW();
            }

            if (toggleaE)
            {
                AutoE();
            }
        }

        /// <summary>
        /// Executes the Auto W Method
        /// </summary>
        private static void AutoW()
        {
            if (!Program.W.IsLearned || !Program.W.IsReady() || Orbwalker.IsAutoAttacking ||
                EntityManager.Turrets.Enemies.Count(t => t.IsValidTarget() && t.IsAttackingPlayer) > 0)
            {
                return;
            }

            var stunW = Config.MiscMenu["stunW"].Cast<CheckBox>().CurrentValue;
            var charmW = Config.MiscMenu["charmW"].Cast<CheckBox>().CurrentValue;
            //var tauntW = Config.MiscMenu["tauntW"].Cast<CheckBox>().CurrentValue;
            var fearW = Config.MiscMenu["fearW"].Cast<CheckBox>().CurrentValue;
            var snareW = Config.MiscMenu["snareW"].Cast<CheckBox>().CurrentValue;
            var wRange = Config.MiscMenu["wRange"].Cast<CheckBox>().CurrentValue;
            var wSlider = Config.MiscMenu["wSlider"].Cast<Slider>().CurrentValue;
            var wRange2 = Config.MiscMenu["wRange2"].Cast<Slider>().CurrentValue;
            var enemy =
                EntityManager.Heroes.Enemies.Where(
                    t => t.IsValidTarget() && Program.W.IsInRange(t) && t.Distance(Player.Instance) >= wRange2)
                    .OrderByDescending(t => t.Distance(Player.Instance));

            foreach (var target in enemy)
            {
                if (wRange && Player.Instance.Distance(target) <= Player.Instance.GetAutoAttackRange())
                {
                    return;
                }

                if (stunW && target.IsStunned)
                {
                    var prediction = Program.W.GetPrediction(target);

                    if (prediction.HitChancePercent >= wSlider && !prediction.Collision)
                    {
                        Program.W.Cast(prediction.CastPosition);
                    }
                }

                else if (charmW && target.IsCharmed)
                {
                    var prediction = Program.W.GetPrediction(target);

                    if (prediction.HitChancePercent >= wSlider && !prediction.Collision)
                    {
                        Program.W.Cast(prediction.CastPosition);
                    }
                }

                /*else if (tauntW && target.IsTaunted)
                {
                    var prediction = Program.W.GetPrediction(target);

                    if (prediction.HitChancePercent >= wSlider && !prediction.Collision)
                    {
                        Program.W.Cast(prediction.CastPosition);
                    }
                }*/

                else if (fearW && target.IsFeared)
                {
                    var prediction = Program.W.GetPrediction(target);

                    if (prediction.HitChancePercent >= wSlider && !prediction.Collision)
                    {
                        Program.W.Cast(prediction.CastPosition);
                    }
                }

                else if (snareW && target.IsRooted)
                {
                    var prediction = Program.W.GetPrediction(target);

                    if (prediction.HitChancePercent >= wSlider && !prediction.Collision)
                    {
                        Program.W.Cast(prediction.CastPosition);
                    }
                }
            }
        }

        /// <summary>
        /// Executes the Auto E Method
        /// </summary>
        private static void AutoE()
        {
            if (!Program.E.IsLearned || !Program.E.IsReady())
            {
                return;
            }

            var enemies =
                EntityManager.Heroes.Enemies.Where(
                    t =>
                        t.IsValidTarget(Program.E.Range) &&
                        (t.IsCharmed || t.IsStunned || t.IsRecalling() || t.IsRooted || t.IsFeared || t.HasBuffOfType(BuffType.Slow)));
            var target = TargetSelector.GetTarget(enemies, DamageType.Physical);
            if (target == null) return;

            var prediction = Program.E.GetPrediction(target);

            if (prediction != null && prediction.HitChancePercent >= 75)
            {
                Program.E.Cast(prediction.CastPosition);
            }

            /* OKTW Logic ; Credits to Sebby */
            foreach (var pred in EntityManager.Heroes.Enemies.Where(
                enemy => enemy != null &&
                         Program.E.IsInRange(enemy) && enemy.IsValidTarget() && !enemy.CanMove &&
                         Game.Time - Essentials.GrabTime > 1)
                .Select(enemy => Program.E.GetPrediction(enemy))
                .Where(pred => pred != null && pred.HitChancePercent >= 75))
            {
                Program.E.Cast(pred.CastPosition);
            }
        }

        /// <summary>
        /// Executes the Kill Steal Method
        /// </summary>
        private static void WR_KillSteal()
        {
            var useW = Config.KillStealMenu["useW"].Cast<CheckBox>().CurrentValue;
            var useR = Config.KillStealMenu["useR"].Cast<CheckBox>().CurrentValue;
            var manaW = Config.KillStealMenu["manaW"].Cast<Slider>().CurrentValue;
            var manaR = Config.KillStealMenu["manaR"].Cast<Slider>().CurrentValue;
            var wSlider = Config.KillStealMenu["wSlider"].Cast<Slider>().CurrentValue;
            var rSlider = Config.KillStealMenu["rSlider"].Cast<Slider>().CurrentValue;
            var wRange = Config.KillStealMenu["wRange"].Cast<Slider>().CurrentValue;

            if (useW && useR && Player.Instance.ManaPercent >= manaW && Player.Instance.ManaPercent >= manaR
                && Program.W.IsReady() && Program.R.IsReady())
            {
                var selection =
                    EntityManager.Heroes.Enemies.Where(
                        t =>
                            t.IsValidTarget() && !Essentials.HasUndyingBuff(t) && Program.W.IsInRange(t) &&
                            Player.Instance.Distance(t) >= wRange &&
                            Player.Instance.Distance(t) <=
                            Config.KillStealMenu["rRange"].Cast<Slider>().CurrentValue &&
                            Player.Instance.Distance(t) >= Config.MiscMenu["rRange"].Cast<Slider>().CurrentValue
                            && Essentials.DamageLibrary.CalculateDamage(t, false, true, false, true) >= t.Health);

                foreach (var enemy in selection)
                {
                    var pred = Program.W.GetPrediction(enemy);

                    if (pred != null && pred.HitChancePercent >= wSlider && !pred.Collision)
                    {
                        Program.W.Cast(pred.CastPosition);
                        var target = enemy;

                        Core.DelayAction(() =>
                        {
                            var predR = Program.R.GetPrediction(target);
                            var checkDmg = target.Health <=
                                           Essentials.DamageLibrary.CalculateDamage(target, false, false, false, true);

                            if (predR != null && predR.HitChancePercent >= rSlider && checkDmg)
                            {
                                Program.R.Cast(predR.CastPosition);
                            }
                        }, Program.W.CastDelay);
                    }
                }
            }
        }

        /// <summary>
        /// Executes the Kill Steal Method
        /// </summary>
        private static void StandaloneKillSteal()
        {
            var useW = Config.KillStealMenu["useW"].Cast<CheckBox>().CurrentValue;
            var useR = Config.KillStealMenu["useR"].Cast<CheckBox>().CurrentValue;
            var manaW = Config.KillStealMenu["manaW"].Cast<Slider>().CurrentValue;
            var manaR = Config.KillStealMenu["manaR"].Cast<Slider>().CurrentValue;
            var wSlider = Config.KillStealMenu["wSlider"].Cast<Slider>().CurrentValue;
            var rSlider = Config.KillStealMenu["rSlider"].Cast<Slider>().CurrentValue;
            var wRange = Config.KillStealMenu["wRange"].Cast<Slider>().CurrentValue;

            if (useW && Player.Instance.ManaPercent >= manaW && Program.W.IsReady())
            {
                var selection =
                    EntityManager.Heroes.Enemies.Where(
                        t =>
                            t.IsValidTarget() && !Essentials.HasUndyingBuff(t) && Program.W.IsInRange(t) &&
                            t.Distance(Player.Instance) >= wRange
                            && Essentials.DamageLibrary.CalculateDamage(t, false, true, false, false) >= t.Health);

                foreach (
                    var pred in
                        selection.Select(enemy => Program.W.GetPrediction(enemy))
                            .Where(pred => pred != null && pred.HitChancePercent >= wSlider && !pred.Collision))
                {
                    Program.W.Cast(pred.CastPosition);
                }
            }

            if (useR && Player.Instance.ManaPercent >= manaR && Program.R.IsReady())
            {
                var selection =
                    EntityManager.Heroes.Enemies.Where(
                        t =>
                            t.IsValidTarget() && !Essentials.HasUndyingBuff(t)
                            &&
                            Player.Instance.Distance(t) <=
                            Config.KillStealMenu["rRange"].Cast<Slider>().CurrentValue &&
                            Player.Instance.Distance(t) >= Config.MiscMenu["rRange"].Cast<Slider>().CurrentValue
                            && Essentials.DamageLibrary.CalculateDamage(t, false, false, false, true) >= t.Health);

                foreach (
                    var pred in
                        selection.Select(enemy => Program.R.GetPrediction(enemy))
                            .Where(pred => pred != null && pred.HitChancePercent >= rSlider))
                {
                    Program.R.Cast(pred.CastPosition);
                }
            }
        }

        /// <summary>
        /// Executes the Jungle Steal Method
        /// </summary>
        private static void JungleSteal()
        {
            var manaR = Config.JungleStealMenu["manaR"].Cast<Slider>().CurrentValue;
            var rRange = Config.JungleStealMenu["rRange"].Cast<Slider>().CurrentValue;

            if (Player.Instance.ManaPercent >= manaR)
            {
                if (Game.MapId == GameMapId.SummonersRift)
                {
                    var jungleMob =
                        EntityManager.MinionsAndMonsters.Monsters.FirstOrDefault(
                            u =>
                                u.IsVisible && Essentials.JungleMobsList.Contains(u.BaseSkinName)
                                && Essentials.DamageLibrary.CalculateDamage(u, false, false, false, true) >= u.Health);

                    if (jungleMob == null)
                    {
                        return;
                    }

                    if (!Config.JungleStealMenu[jungleMob.BaseSkinName].Cast<CheckBox>().CurrentValue)
                    {
                        return;
                    }

                    var enemy =
                        EntityManager.Heroes.Enemies.Where(t => t.Distance(jungleMob) <= 100)
                            .OrderByDescending(t => t.Distance(jungleMob));

                    if (enemy.Any())
                    {
                        foreach (var target in enemy.Where(target => Player.Instance.Distance(target) < rRange))
                        {
                            if (target.Distance(jungleMob) <= 100)
                            {
                                Program.R.Cast(jungleMob.ServerPosition);
                            }
                        }
                    }
                }
                if (Game.MapId == GameMapId.TwistedTreeline)
                {
                    var jungleMob =
                        EntityManager.MinionsAndMonsters.Monsters.FirstOrDefault(
                            u =>
                                u.IsVisible && Essentials.JungleMobsListTwistedTreeline.Contains(u.BaseSkinName)
                                && Essentials.DamageLibrary.CalculateDamage(u, false, false, false, true) >= u.Health);

                    if (jungleMob == null)
                    {
                        return;
                    }

                    if (!Config.JungleStealMenu[jungleMob.BaseSkinName].Cast<CheckBox>().CurrentValue)
                    {
                        return;
                    }

                    var enemy =
                        EntityManager.Heroes.Enemies.Where(t => t.Distance(jungleMob) <= 100)
                            .OrderByDescending(t => t.Distance(jungleMob));

                    if (enemy.Any())
                    {
                        foreach (var target in enemy.Where(target => Player.Instance.Distance(target) < rRange))
                        {
                            if (target.Distance(jungleMob) <= 100)
                            {
                                Program.R.Cast(jungleMob.ServerPosition);
                            }
                        }
                    }
                }
            }
        }
    }
}