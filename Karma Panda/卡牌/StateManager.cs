namespace TwistedBuddy
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;

    internal class StateManager
    {
        /// <summary>
        /// Selects a Card
        /// </summary>
        /// <param name="t">The Target</param>
        /// <param name="selectedCard">The Card that is selected.</param>
        public static void SelectCard(Obj_AI_Base t, Cards selectedCard)
        {
            if (t == null)
            {
                return;
            }

            if (selectedCard == Cards.Red)
            {
                CardSelector.StartSelecting(Cards.Red);
            }
            else if (selectedCard == Cards.Yellow)
            {
                CardSelector.StartSelecting(Cards.Yellow);
            }
            else if (selectedCard == Cards.Blue)
            {
                CardSelector.StartSelecting(Cards.Blue);
            }
        }

        /// <summary>
        /// Does LaneClear
        /// </summary>
        public static void LaneClear()
        {
            var useQ = Essentials.LaneClearMenu["useQ"].Cast<CheckBox>().CurrentValue;

            if (useQ)
            {
                var qMinion =
                    EntityManager.MinionsAndMonsters.GetLaneMinions(
                        EntityManager.UnitTeam.Enemy,
                        Player.Instance.ServerPosition,
                        Program.Q.Range).OrderBy(t => t.Health);
                var qPred = Essentials.LaneClearMenu["qPred"].Cast<Slider>().CurrentValue;
                var manaManagerQ = Essentials.LaneClearMenu["manaManagerQ"].Cast<Slider>().CurrentValue;

                if (Program.Q.IsReady() && Player.Instance.ManaPercent >= manaManagerQ)
                {
                    var minionPrediction = EntityManager.MinionsAndMonsters.GetLineFarmLocation(
                        qMinion,
                        Program.Q.Width,
                        (int) Program.Q.Range);

                    if (minionPrediction.HitNumber >= qPred)
                    {
                        Program.Q.Cast(minionPrediction.CastPosition);
                    }
                }
            }

            var useCard = Essentials.LaneClearMenu["useCard"].Cast<CheckBox>().CurrentValue;

            if (useCard)
            {
                var chooser = Essentials.LaneClearMenu["chooser"].Cast<ComboBox>().SelectedText;
                var minion =
                    EntityManager.MinionsAndMonsters.GetLaneMinions(
                        EntityManager.UnitTeam.Enemy,
                        Player.Instance.ServerPosition,
                        Player.Instance.AttackRange + 100).ToArray();

                if (!minion.Any()) return;

                switch (chooser)
                {
                    case "Smart":
                        var redCardKillableMinions = minion.Count(
                            target => target.Distance(minion.FirstOrDefault()) <= 200 &&
                                      target.Health <=
                                      DamageLibrary.PredictWDamage(target, Cards.Red));
                        var selectedCard = Cards.None;

                        if (selectedCard == Cards.None && Player.Instance.ManaPercent < Essentials.LaneClearMenu["manaW"].Cast<Slider>().CurrentValue)
                        {
                            selectedCard = Cards.Blue;
                        }

                        if (selectedCard == Cards.None && redCardKillableMinions >= Essentials.LaneClearMenu["enemyW"].Cast<Slider>().CurrentValue)
                        {
                            selectedCard = Cards.Red;
                        }

                        if (selectedCard != Cards.None)
                        {
                            SelectCard(minion.FirstOrDefault(), selectedCard);
                        }
                        break;
                    case "Yellow":
                        SelectCard(minion.FirstOrDefault(), Cards.Yellow);
                        break;
                    case "Red":
                        SelectCard(minion.FirstOrDefault(), Cards.Red);
                        break;
                    case "Blue":
                        SelectCard(minion.FirstOrDefault(), Cards.Blue);
                        break;
                }
            }
        }

        /// <summary>
        /// Does JungleSteal
        /// </summary>
        public static void JungleClear()
        {
            var useQ = Essentials.JungleClearMenu["useQ"].Cast<CheckBox>().CurrentValue;

            if (useQ)
            {
                var qMinion =
                    EntityManager.MinionsAndMonsters.GetJungleMonsters(
                        Player.Instance.ServerPosition,
                        Program.Q.Range).OrderBy(t => t.Health).FirstOrDefault();
                if (qMinion == null) return;

                var qPred = Essentials.JungleClearMenu["qPred"].Cast<Slider>().CurrentValue;
                var manaManagerQ = Essentials.JungleClearMenu["manaManagerQ"].Cast<Slider>().CurrentValue;

                if (Program.Q.IsReady() && Player.Instance.ManaPercent >= manaManagerQ)
                {
                    var minionPrediction = Program.Q.GetPrediction(qMinion);

                    if (minionPrediction.HitChancePercent >= qPred)
                    {
                        Program.Q.Cast(minionPrediction.CastPosition);
                    }
                }
            }

            var useCard = Essentials.JungleClearMenu["useCard"].Cast<CheckBox>().CurrentValue;

            if (useCard)
            {
                var minion = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.ServerPosition,
                    Player.Instance.AttackRange + 100).ToArray();
                var chooser = Essentials.JungleClearMenu["chooser"].Cast<ComboBox>().SelectedText;

                if (!minion.Any()) return;

                switch (chooser)
                {
                    case "Smart":
                        var redCardKillableMinions = minion.Count(
                            target => target.Distance(minion.FirstOrDefault()) <= 200 &&
                                      target.Health <=
                                      DamageLibrary.PredictWDamage(target, Cards.Red));
                        var selectedCard = Cards.None;

                        if (selectedCard == Cards.None && Player.Instance.ManaPercent < Essentials.JungleClearMenu["manaW"].Cast<Slider>().CurrentValue)
                        {
                            selectedCard = Cards.Blue;
                        }

                        if (selectedCard == Cards.None && redCardKillableMinions >= Essentials.JungleClearMenu["enemyW"].Cast<Slider>().CurrentValue)
                        {
                            selectedCard = Cards.Red;
                        }

                        if (selectedCard == Cards.None && redCardKillableMinions < Essentials.JungleClearMenu["enemyW"].Cast<Slider>().CurrentValue)
                        {
                            selectedCard = Cards.Yellow;
                        }

                        if (selectedCard != Cards.None)
                        {
                            SelectCard(minion.FirstOrDefault(), selectedCard);
                        }
                        break;
                    case "Yellow":
                        SelectCard(minion.FirstOrDefault(), Cards.Yellow);
                        break;
                    case "Red":
                        SelectCard(minion.FirstOrDefault(), Cards.Red);
                        break;
                    case "Blue":
                        SelectCard(minion.FirstOrDefault(), Cards.Blue);
                        break;
                }
            }
        }

        /// <summary>
        /// Does Harass
        /// </summary>
        public static void Harass()
        {
            var useCard = Essentials.HarassMenu["useCard"].Cast<CheckBox>().CurrentValue;

            if (useCard)
            {
                var wSlider = Essentials.HarassMenu["wSlider"].Cast<Slider>().CurrentValue;
                var t = TargetSelector.GetTarget(
                    Player.Instance.AttackRange + wSlider,
                    DamageType.Mixed);

                if (t == null) return;

                var chooser = Essentials.HarassMenu["chooser"].Cast<ComboBox>().SelectedText;

                if (chooser == "Smart")
                {
                    var selectedCard = Essentials.HeroCardSelection(t, Essentials.HarassMenu);
                    SelectCard(t, selectedCard);
                }
                else if (chooser == "Yellow")
                {
                    SelectCard(t, Cards.Yellow);
                }
                else if (chooser == "Red")
                {
                    SelectCard(t, Cards.Red);
                }
                else if (chooser == "Blue")
                {
                    SelectCard(t, Cards.Blue);
                }
            }

            var useQ = Essentials.HarassMenu["useQ"].Cast<CheckBox>().CurrentValue;

            if (useQ)
            {
                var qTarget = TargetSelector.GetTarget(Program.Q.Range, DamageType.Magical);
                if (qTarget == null)
                {
                    return;
                }
                var manaManagerQ = Essentials.HarassMenu["manaManagerQ"].Cast<Slider>().CurrentValue;
                if (!Program.Q.IsInRange(qTarget) || !Program.Q.IsReady()
                    || !(Player.Instance.ManaPercent >= manaManagerQ))
                {
                    return;
                }
                var qPred = Essentials.HarassMenu["qPred"].Cast<Slider>().CurrentValue;
                var pred = Program.Q.GetPrediction(qTarget);
                if (pred.HitChancePercent >= qPred)
                {
                    Program.Q.Cast(pred.CastPosition);
                }
            }
        }

        /// <summary>
        /// Does Combo
        /// </summary>
        public static void Combo()
        {
            var useCard = Essentials.ComboMenu["useCard"].Cast<CheckBox>().CurrentValue;

            if (useCard)
            {
                var wSlider = Essentials.ComboMenu["wSlider"].Cast<Slider>().CurrentValue;
                var wTarget = TargetSelector.GetTarget(
                    Player.Instance.AttackRange + wSlider,
                    DamageType.Magical);

                if (wTarget != null)
                {
                    var chooser = Essentials.ComboMenu["chooser"].Cast<ComboBox>().SelectedText;

                    switch (chooser)
                    {
                        case "Smart":
                            var selectedCard = Essentials.HeroCardSelection(wTarget, Essentials.ComboMenu);
                            if (selectedCard != Cards.None)
                            {
                                SelectCard(wTarget, selectedCard);
                            }
                            break;
                        case "Yellow":
                            SelectCard(wTarget, Cards.Yellow);
                            break;
                        case "Red":
                            SelectCard(wTarget, Cards.Red);
                            break;
                        case "Blue":
                            SelectCard(wTarget, Cards.Blue);
                            break;
                    }
                }
            }
            var useQ = Essentials.ComboMenu["useQ"].Cast<CheckBox>().CurrentValue;

            if (useQ)
            {
                var qTarget = TargetSelector.GetTarget(
                    Program.Q.Range,
                    DamageType.Magical);
                if (qTarget == null) return;

                var useQStun = Essentials.ComboMenu["useQStun"].Cast<CheckBox>().CurrentValue;
                var qPred = Essentials.ComboMenu["qPred"].Cast<Slider>().CurrentValue;
                var manaManagerQ = Essentials.ComboMenu["manaManagerQ"].Cast<Slider>().CurrentValue;

                if (useQStun) return;
                if (!Program.Q.IsInRange(qTarget) || !Program.Q.IsReady() ||
                    !(Player.Instance.ManaPercent >= manaManagerQ)) return;
                var pred = Program.Q.GetPrediction(qTarget);

                if (pred.HitChancePercent >= qPred)
                {
                    Program.Q.Cast(pred.CastPosition);
                }
            }
        }

        /// <summary>
        /// Does KillSteal
        /// </summary>
        public static void KillSteal()
        {
            var useQ = Essentials.KillStealMenu["useQ"].Cast<CheckBox>().CurrentValue;
            var qPred = Essentials.KillStealMenu["qPred"].Cast<Slider>().CurrentValue;
            var manaManagerQ = Essentials.KillStealMenu["manaManagerQ"].Cast<Slider>().CurrentValue;

            if (useQ)
            {
                var target =
                    EntityManager.Heroes.Enemies.Where(
                        t =>
                            Program.Q.IsInRange(t) && t.IsValidTarget() &&
                            t.Health <= DamageLibrary.CalculateDamage(t, true, false, false, false))
                        .OrderByDescending(t => t.Health)
                        .FirstOrDefault();

                if (target != null && Program.Q.IsReady() && Player.Instance.ManaPercent >= manaManagerQ)
                {
                    var pred = Program.Q.GetPrediction(target);

                    if (pred != null && pred.HitChancePercent >= qPred)
                    {
                        Program.Q.Cast(pred.CastPosition);
                    }
                }
            }
        }

        /// <summary>
        /// Does Auto Q
        /// </summary>
        public static void AutoQ()
        {
            if (Essentials.UseStunQ &&
                (Essentials.MiscMenu["autoQ"].Cast<CheckBox>().CurrentValue ||
                 Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                 Essentials.ComboMenu["useQStun"].Cast<CheckBox>().CurrentValue))
            {
                var target = Essentials.StunnedTarget;

                if (target != null)
                {
                    var qPrediction = Program.Q.GetPrediction(target);

                    if (qPrediction != null &&
                        qPrediction.HitChancePercent >= Essentials.MiscMenu["qPred"].Cast<Slider>().CurrentValue)
                    {
                        Program.Q.Cast(qPrediction.CastPosition);
                        Essentials.UseStunQ = false;
                        Essentials.StunnedTarget = null;
                    }
                }
            }

            if (!Essentials.MiscMenu["autoQ"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            var enemies = EntityManager.Heroes.Enemies.Where(t => t.IsValidTarget(Program.Q.Range) && t.HasBuffOfType(BuffType.Stun));

            foreach (
                var pred in
                    enemies.Select(target => Program.Q.GetPrediction(target))
                        .Where(pred => pred.HitChancePercent >= Essentials.MiscMenu["qPred"].Cast<Slider>().CurrentValue
                        ))
            {
                Program.Q.Cast(pred.CastPosition);
            }
        }
    }
}