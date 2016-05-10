namespace Jinx
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;

    /// <summary>
    /// Class that executes the modes.
    /// </summary>
    internal class StateManager
    {
        /// <summary>
        /// Does Combo Method
        /// </summary>
        public static void Combo()
        {
            #region Q Logic

            var useQ = Config.ComboMenu["useQ"].Cast<CheckBox>().CurrentValue;
            var manaQ = Config.ComboMenu["manaQ"].Cast<Slider>().CurrentValue;

            // If the player has a minigun
            if (useQ && Player.Instance.ManaPercent >= manaQ && Program.Q.IsReady() && !Essentials.FishBones())
            {
                var target = TargetSelector.GetTarget(Essentials.FishBonesRange(), DamageType.Physical);

                if (target != null && target.IsValidTarget())
                {
                    if (!Player.Instance.IsInAutoAttackRange(target) &&
                        Player.Instance.Distance(target) <= Essentials.FishBonesRange())
                    {
                        Program.Q.Cast();
                        Orbwalker.ForcedTarget = target;
                    }

                    if (Player.Instance.IsInAutoAttackRange(target) &&
                        target.CountEnemiesInRange(100) >= Config.ComboMenu["qCountC"].Cast<Slider>().CurrentValue)
                    {
                        Program.Q.Cast();
                        Orbwalker.ForcedTarget = target;
                    }
                }
            }

            #endregion

            #region Spell Logic

            var useW = Config.ComboMenu["useW"].Cast<CheckBox>().CurrentValue;
            var useE = Config.ComboMenu["useE"].Cast<CheckBox>().CurrentValue;
            var useR = Config.ComboMenu["useR"].Cast<CheckBox>().CurrentValue;
            var wRange = Config.MiscMenu["wRange"].Cast<CheckBox>().CurrentValue;
            var wRange2 = Config.ComboMenu["wRange2"].Cast<Slider>().CurrentValue;
            var eRange = Config.ComboMenu["eRange"].Cast<Slider>().CurrentValue;
            var manaW = Config.ComboMenu["manaW"].Cast<Slider>().CurrentValue;
            var manaE = Config.ComboMenu["manaE"].Cast<Slider>().CurrentValue;
            var manaR = Config.ComboMenu["manaR"].Cast<Slider>().CurrentValue;
            var wSlider = Config.ComboMenu["wSlider"].Cast<Slider>().CurrentValue;
            var eSlider = Config.ComboMenu["eSlider"].Cast<Slider>().CurrentValue;
            var rSlider = Config.ComboMenu["rSlider"].Cast<Slider>().CurrentValue;
            var rCountC = Config.ComboMenu["rCountC"].Cast<Slider>().CurrentValue;

            if (useW && Player.Instance.ManaPercent >= manaW && Program.W.IsReady())
            {
                var target = TargetSelector.GetTarget(Program.W.Range, DamageType.Physical);

                if (target != null && target.IsValidTarget())
                {
                    if (Player.Instance.Distance(target) >= wRange2)
                    {
                        var wPrediction = Program.W.GetPrediction(target);

                        if (wPrediction != null && !wPrediction.Collision && wPrediction.HitChancePercent >= wSlider)
                        {
                            if (wRange && Player.Instance.IsInAutoAttackRange(target))
                            {
                                Program.W.Cast(wPrediction.CastPosition);
                            }
                            else if (!wRange)
                            {
                                Program.W.Cast(wPrediction.CastPosition);
                            }
                        }
                    }
                }
            }

            if (useE && Player.Instance.ManaPercent >= manaE && Program.E.IsReady())
            {
                var target = TargetSelector.GetTarget(Config.ComboMenu["eRange2"].Cast<Slider>().CurrentValue,
                    DamageType.Physical);

                if (target != null)
                {
                    if (Player.Instance.Distance(target) >= eRange)
                    {
                        var ePrediction = Program.E.GetPrediction(target);

                        if (ePrediction != null && ePrediction.HitChancePercent >= eSlider)
                        {
                            Program.E.Cast(ePrediction.CastPosition);
                        }
                    }
                }
            }

            if (useR && Player.Instance.ManaPercent >= manaR && Program.R.IsReady())
            {
                var rRange = Config.MiscMenu["rRange"].Cast<Slider>().CurrentValue;
                var rRange2 = Config.ComboMenu["rRange2"].Cast<Slider>().CurrentValue;
                var target = TargetSelector.GetTarget(rRange2, DamageType.Physical);

                if (target != null)
                {
                    if (Player.Instance.Distance(target) >= rRange)
                    {
                        var rPrediction = Program.R.GetPrediction(target);

                        if (rPrediction != null && rPrediction.HitChancePercent >= rSlider &&
                            EntityManager.Heroes.Enemies.Count(
                                t => t.IsValidTarget() && t.Distance(rPrediction.CastPosition) <= 200) >= rCountC)
                        {
                            Program.R.Cast(rPrediction.CastPosition);
                        }
                    }
                }
            }

            #endregion
        }

        /// <summary>
        /// Does LastHit Method
        /// </summary>
        public static void LastHit()
        {
            var useQ = Config.LastHitMenu["useQ"].Cast<CheckBox>().CurrentValue;
            var manaQ = Config.LastHitMenu["manaQ"].Cast<Slider>().CurrentValue;
            var qCountM = Config.LastHitMenu["qCountM"].Cast<Slider>().CurrentValue;

            // In Range
            if (useQ && Player.Instance.ManaPercent >= manaQ && !Essentials.FishBones())
            {
                var minionInRange =
                    EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(
                        m =>
                            m.Health <= (Player.Instance.GetAutoAttackDamage(m)*1.1f) &&
                            m.Distance(Player.Instance) <= Essentials.MinigunRange);

                if (minionInRange != null)
                {
                    var minion = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy,
                        Player.Instance.ServerPosition,
                        Essentials.FishBonesRange())
                        .Where(
                            t =>
                                t.Distance(minionInRange
                                    ) <=
                                100 && t.Health <= (Player.Instance.GetAutoAttackDamage(t) * 1.1f)).ToArray();

                    if (minion.Count() >= qCountM)
                    {
                        foreach (var m in minion)
                        {
                            Program.Q.Cast();
                            Orbwalker.ForcedTarget = m;
                        }
                    }
                }
            }

            // Out of Range
            if (useQ && Player.Instance.ManaPercent >= manaQ && !Essentials.FishBones())
            {
                var minion = EntityManager.MinionsAndMonsters.GetLaneMinions(
                    EntityManager.UnitTeam.Enemy,
                    Player.Instance.ServerPosition,
                    Essentials.FishBonesRange())
                    .Where(
                        t =>
                            t.Health <= Player.Instance.GetAutoAttackDamage(t)*1.1f &&
                            t.Distance(Player.Instance) > Essentials.MinigunRange)
                    .OrderByDescending(t => t.Health);

                foreach (var m in from minionOutOfRange in minion where minionOutOfRange != null select EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy,
                    Player.Instance.ServerPosition,
                    Essentials.FishBonesRange())
                    .Where(
                        t =>
                            t.Distance(minionOutOfRange
                                ) <=
                            100 && t.Health <= (Player.Instance.GetAutoAttackDamage(t) * 1.1f)).ToArray() into minion2 where minion2.Count() >= qCountM from m in minion2 select m)
                {
                    Program.Q.Cast();
                    Orbwalker.ForcedTarget = m;
                }
            }
        }

        /// <summary>
        /// Does Harass Method
        /// </summary>
        public static void Harass()
        {
            #region Variables

            var useQ = Config.HarassMenu["useQ"].Cast<CheckBox>().CurrentValue;
            var useW = Config.HarassMenu["useW"].Cast<CheckBox>().CurrentValue;
            var wRange = Config.MiscMenu["wRange"].Cast<CheckBox>().CurrentValue;
            var wRange2 = Config.HarassMenu["wRange2"].Cast<Slider>().CurrentValue;
            var manaQ = Config.HarassMenu["manaQ"].Cast<Slider>().CurrentValue;
            var manaW = Config.HarassMenu["manaW"].Cast<Slider>().CurrentValue;
            var qCountM = Config.HarassMenu["qCountM"].Cast<Slider>().CurrentValue;
            var wSlider = Config.HarassMenu["wSlider"].Cast<Slider>().CurrentValue;

            #endregion

            #region Last Hitting Section

            // Force Minigun if there is a lasthittable minion in minigun range and there is no targets more than the setting amount.
            var kM = Orbwalker.LastHitMinionsList.Where(
                t => t.IsEnemy &&
                     t.Health <= (Player.Instance.GetAutoAttackDamage(t)*0.9090909F) && t.IsValidTarget() &&
                     t.Distance(Player.Instance) <= Essentials.MinigunRange);
            if (useQ && Essentials.FishBones() && kM.Count() < qCountM)
            {
                Program.Q.Cast();
            }
            
            // Out of Range
            if (useQ && Player.Instance.ManaPercent >= manaQ && !Essentials.FishBones())
            {
                var minion = EntityManager.MinionsAndMonsters.GetLaneMinions(
                    EntityManager.UnitTeam.Enemy,
                    Player.Instance.ServerPosition,
                    Essentials.FishBonesRange())
                    .Where(
                        t =>
                            t.Health <= Player.Instance.GetAutoAttackDamage(t)*1.1f &&
                            t.Distance(Player.Instance) > Essentials.MinigunRange)
                    .OrderByDescending(t => t.Health);

                foreach (var m in from minionOutOfRange in minion
                    where minionOutOfRange != null
                    select EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy,
                        Player.Instance.ServerPosition,
                        Essentials.FishBonesRange())
                        .Where(
                            t =>
                                t.Distance(minionOutOfRange
                                    ) <=
                                100 && t.Health <= (Player.Instance.GetAutoAttackDamage(t)*1.1f)).ToArray()
                    into minion2
                    where minion2.Count() >= qCountM
                    from m in minion2
                    select m)
                {
                    Program.Q.Cast();
                    Orbwalker.ForcedTarget = m;
                }
            }

            // In Range
            if (useQ && Player.Instance.ManaPercent >= manaQ && !Essentials.FishBones())
            {
                var minionInRange = Orbwalker.LastHitMinionsList.FirstOrDefault(
                    m => m.IsValidTarget() && m.Distance(Player.Instance) <= Essentials.MinigunRange);

                if (minionInRange != null)
                {
                    var minion = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy,
                        Player.Instance.ServerPosition,
                        Essentials.FishBonesRange())
                        .Where(
                            t =>
                                t.Distance(minionInRange
                                    ) <=
                                100 && t.Health <= (Player.Instance.GetAutoAttackDamage(t)*1.1f)).ToArray();

                    if (minion.Count() >= qCountM)
                    {
                        foreach (var m in minion)
                        {
                            Program.Q.Cast();
                            Orbwalker.ForcedTarget = m;
                        }
                    }
                }
            }

            #endregion

            #region Harassing Section

            // If the player has a minigun
            if (useQ && Player.Instance.ManaPercent >= manaQ && Program.Q.IsReady() && !Essentials.FishBones())
            {
                var target = TargetSelector.GetTarget(Essentials.FishBonesRange(), DamageType.Physical);

                if (target != null && target.IsValidTarget())
                {
                    if (!Player.Instance.IsInAutoAttackRange(target) &&
                        Player.Instance.Distance(target) <= Essentials.FishBonesRange())
                    {
                        Program.Q.Cast();
                        Orbwalker.ForcedTarget = target;
                    }

                    if (Player.Instance.IsInAutoAttackRange(target) &&
                        target.CountEnemiesInRange(100) >= Config.HarassMenu["qCountC"].Cast<Slider>().CurrentValue)
                    {
                        Program.Q.Cast();
                        Orbwalker.ForcedTarget = target;
                    }
                }
            }

            // If the player has the rocket
            if (useQ && Program.Q.IsReady() && Essentials.FishBones())
            {
                var target = TargetSelector.GetTarget(Essentials.FishBonesRange(), DamageType.Physical);

                if (target != null && target.IsValidTarget())
                {
                    if (Player.Instance.Distance(target) <= Essentials.MinigunRange &&
                        target.CountEnemiesInRange(100) < Config.HarassMenu["qCountC"].Cast<Slider>().CurrentValue)
                    {
                        Program.Q.Cast();
                        Orbwalker.ForcedTarget = target;
                    }
                }
            }

            if (useW && Player.Instance.ManaPercent >= manaW && Program.W.IsReady())
            {
                var target = TargetSelector.GetTarget(Program.W.Range, DamageType.Physical);

                if (target != null && target.IsValidTarget())
                {
                    if (Player.Instance.Distance(target) >= wRange2)
                    {
                        var wPrediction = Program.W.GetPrediction(target);

                        if (wPrediction.HitChancePercent >= wSlider && !wPrediction.Collision)
                        {
                            if (wRange && Player.Instance.IsInAutoAttackRange(target))
                            {
                                Program.W.Cast(wPrediction.CastPosition);
                            }
                            else if (!wRange)
                            {
                                Program.W.Cast(wPrediction.CastPosition);
                            }
                        }
                    }
                }
            }

            #endregion
        }

        /// <summary>
        /// Does LaneClear Method
        /// </summary>
        public static void LaneClear()
        {
            var useQ = Config.LaneClearMenu["useQ"].Cast<CheckBox>().CurrentValue;
            var manaQ = Config.LaneClearMenu["manaQ"].Cast<Slider>().CurrentValue;
            var lastHit = Config.LaneClearMenu["lastHit"].Cast<CheckBox>().CurrentValue;

            if (useQ && Program.Q.IsReady())
            {
                var minion = EntityManager.MinionsAndMonsters.GetLaneMinions(
                    EntityManager.UnitTeam.Enemy,
                    Player.Instance.ServerPosition,
                    Essentials.FishBonesRange())
                    .Where(
                        t =>
                            t.Health <= Player.Instance.GetAutoAttackDamage(t)*1.1f &&
                            t.Distance(Player.Instance) > Essentials.MinigunRange)
                    .OrderByDescending(t => t.Health);
                //Orbwalker.LasthittableMinions.Where(t => t.IsValidTarget() && t.Distance(Player.Instance) <= Essentials.FishBonesRange());

                if (Essentials.FishBones())
                {
                    if (!minion.Any())
                    {
                        Program.Q.Cast();
                        return;
                    }
                }

                if (!Essentials.FishBones() && Player.Instance.ManaPercent >= manaQ)
                {
                    var m = minion.FirstOrDefault();

                    if (m == null)
                    {
                        return;
                    }

                    var minionsAoe =
                        EntityManager.MinionsAndMonsters.EnemyMinions.Count(
                            t =>
                                t.IsValidTarget() && t.Distance(m) <= 100 &&
                                t.Health <= (Player.Instance.GetAutoAttackDamage(m)*1.1f));

                    if (m.Distance(Player.Instance) <= Essentials.FishBonesRange() && m.IsValidTarget() &&
                        minionsAoe >= Config.LaneClearMenu["qCountM"].Cast<Slider>().CurrentValue)
                    {
                        Program.Q.Cast();
                        Orbwalker.ForcedTarget = m;
                    }
                    else if (m.Distance(Player.Instance) >= Player.Instance.GetAutoAttackRange() &&
                             Orbwalker.LastHitMinionsList.Contains(m) && lastHit)
                    {
                        Program.Q.Cast();
                        Orbwalker.ForcedTarget = m;
                    }
                }
            }
        }

        /// <summary>
        /// Does JungleClear Method
        /// </summary>
        public static void JungleClear()
        {
            var useQ = Config.JungleClearMenu["useQ"].Cast<CheckBox>().CurrentValue;
            var useW = Config.JungleClearMenu["useW"].Cast<CheckBox>().CurrentValue;
            var wRange = Config.MiscMenu["wRange"].Cast<CheckBox>().CurrentValue;
            var manaQ = Config.JungleClearMenu["manaQ"].Cast<Slider>().CurrentValue;
            var manaW = Config.JungleClearMenu["manaW"].Cast<Slider>().CurrentValue;
            var wSlider = Config.JungleClearMenu["wSlider"].Cast<Slider>().CurrentValue;

            if (useQ && Player.Instance.ManaPercent >= manaQ && Program.Q.IsReady())
            {
                if (Essentials.FishBones())
                {
                    var mobs = EntityManager.MinionsAndMonsters.GetJungleMonsters(
                        Player.Instance.ServerPosition,
                        Essentials.FishBonesRange());

                    foreach (
                        var mob in
                            mobs.Where(mob => mob != null && Player.Instance.Distance(mob) <= Essentials.MinigunRange))
                    {
                        Program.Q.Cast();
                        Orbwalker.ForcedTarget = mob;
                    }
                }
                else if (!Essentials.FishBones())
                {
                    var mobs = EntityManager.MinionsAndMonsters.GetJungleMonsters(
                        Player.Instance.ServerPosition,
                        Essentials.FishBonesRange());

                    foreach (
                        var mob in
                            mobs.Where(mob => mob != null)
                                .Where(
                                    mob =>
                                        !Player.Instance.IsInAutoAttackRange(mob) &&
                                        Player.Instance.Distance(mob) <= Essentials.FishBonesRange()))
                    {
                        Program.Q.Cast();
                        Orbwalker.ForcedTarget = mob;
                    }
                }
            }

            if (useW && Player.Instance.ManaPercent >= manaW && Program.W.IsReady())
            {
                var target =
                    EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.ServerPosition, Program.W.Range)
                        .OrderByDescending(t => t.Health)
                        .FirstOrDefault();
                if (target != null)
                {
                    var wPrediction = Program.W.GetPrediction(target);

                    if (!(wPrediction.HitChancePercent >= wSlider))
                    {
                        return;
                    }
                    if (wRange && Player.Instance.IsInAutoAttackRange(target))
                    {
                        Program.W.Cast(wPrediction.CastPosition);
                    }
                    else if (!wRange)
                    {
                        Program.W.Cast(wPrediction.CastPosition);
                    }
                }
            }
        }


        /// <summary>
        /// Does Flee Method
        /// </summary>
        public static void Flee()
        {
            var useW = Config.FleeMenu["useW"].Cast<CheckBox>().CurrentValue;
            var useE = Config.FleeMenu["useE"].Cast<CheckBox>().CurrentValue;
            var wSlider = Config.FleeMenu["wSlider"].Cast<Slider>().CurrentValue;
            var eSlider = Config.FleeMenu["eSlider"].Cast<Slider>().CurrentValue;

            if (useW && Program.W.IsReady())
            {
                var enemy =
                    EntityManager.Heroes.Enemies.FirstOrDefault(
                        t => Program.W.IsInRange(t) && t.IsValidTarget() && !Player.Instance.IsInAutoAttackRange(t));

                if (enemy != null)
                {
                    var prediction = Program.W.GetPrediction(enemy);

                    if (prediction.HitChancePercent >= wSlider)
                    {
                        Program.W.Cast(prediction.CastPosition);
                    }
                }
            }

            if (useE && Program.E.IsReady())
            {
                var enemy =
                    EntityManager.Heroes.Enemies.FirstOrDefault(
                        t =>
                            Program.E.IsInRange(t) && t.IsValidTarget() && t.IsMelee &&
                            t.IsInAutoAttackRange(Player.Instance));

                if (enemy != null)
                {
                    var prediction = Program.E.GetPrediction(enemy);

                    if (prediction.HitChancePercent >= eSlider)
                    {
                        Program.E.Cast(prediction.CastPosition);
                    }
                }
            }
        }
    }
}