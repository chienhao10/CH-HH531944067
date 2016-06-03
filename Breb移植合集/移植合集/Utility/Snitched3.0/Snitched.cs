namespace Snitched
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Spell = LeagueSharp.Common.Spell;
    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK;
    internal class Snitched
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the spells.
        /// </summary>
        /// <value>
        ///     The spells.
        /// </value>
        private List<Spell> Spells { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Loads Snitched.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public void Load(EventArgs args)
        {
            Config.Instance.CreateMenu();

            HealthPrediction.Load();

            ObjectTracker.OnObjectiveCreated += (sender, type) => HealthPrediction.TrackObject((Obj_AI_Base)sender);
            ObjectTracker.OnObjectiveDead += (sender, type) => HealthPrediction.UntrackObject((Obj_AI_Base)sender);
            ObjectTracker.Load();

            this.Spells = SpellLoader.GetUsableSpells();

            Game.OnUpdate += this.Game_OnUpdate;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Fired when the game is updated.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void Game_OnUpdate(EventArgs args)
        {
            if (ObjectTracker.Baron != null)
            {
                this.HandleObjective(ObjectTracker.Baron, ObjectiveType.Baron);
            }

            if (ObjectTracker.Dragon != null)
            {
                this.HandleObjective(ObjectTracker.Dragon, ObjectiveType.Dragon);
            }

            if (ObjectTracker.BlueBuffs.Any())
            {
                ObjectTracker.BlueBuffs.ForEach(x => this.HandleBuff(x, ObjectiveType.Blue));
            }

            if (ObjectTracker.RedBuffs.Any())
            {
                ObjectTracker.RedBuffs.ForEach(x => this.HandleBuff(x, ObjectiveType.Red));
            }

            this.StealKills();
        }

        /// <summary>
        ///     Handles the buff.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="type">The type.</param>
        private void HandleBuff(Obj_AI_Base unit, ObjectiveType type)
        {
            if (!Config.Instance.buffMenu["Steal" + type + "Buff"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            var alliesinRange = unit.CountAlliesInRange(1000);

            if (!Config.Instance.buffMenu["StealAllyBuffs"].Cast<CheckBox>().CurrentValue && alliesinRange > 0)
            {
                return;
            }

            this.StealObject(unit, StealType.BuffSteal);
        }

        /// <summary>
        ///     Handles the objective.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="type">The type.</param>
        private void HandleObjective(Obj_AI_Base unit, ObjectiveType type)
        {
            if ((Config.Instance.objectiveMenu["SmartObjectiveSteal"].Cast<CheckBox>().CurrentValue
                 && !Config.Instance.objectiveMenu["StealObjectiveKeyBind"].Cast<CheckBox>().CurrentValue && unit.CountAlliesInRange(500) != 0)
                || !Config.Instance.objectiveMenu["StealObjectiveKeyBind"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            if (!Config.Instance.objectiveMenu["Steal" + type].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            this.StealObject(unit, StealType.ObjectiveSteal);
        }

        /// <summary>
        ///     Steals the kills.
        /// </summary>
        private void StealKills()
        {
            if (Config.Instance.ksMenu["DontStealOnCombo"].Cast<CheckBox>().CurrentValue
                && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                return;
            }

            foreach (var enemy in
                HeroManager.Enemies.Where(
                    x =>
                    x.LSIsValidTarget(Config.Instance.miscMenu["DistanceLimit"].Cast<Slider>().CurrentValue)
                    && Config.Instance.ksMenu["KS" + x.ChampionName].Cast<CheckBox>().CurrentValue))
            {
                var spell =
                    this.Spells.Where(
                        x =>
                        ObjectManager.Player.LSGetSpellDamage(enemy, x.Slot) > enemy.Health
                        && x.IsInRange(enemy) && Config.Instance.ksMenu["KillSteal" + x.Slot].Cast<CheckBox>().CurrentValue
                        && x.GetMissileArrivalTime(enemy)
                        < Config.Instance.miscMenu["ETALimit"].Cast<Slider>().CurrentValue / 1000f).MinOrDefault(x => x.GetDamage(enemy));

                spell?.Cast(enemy);
            }
        }

        /// <summary>
        ///     Steals the object.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="type">The type.</param>
        private void StealObject(Obj_AI_Base unit, StealType type)
        {
            if (ObjectManager.Player.LSDistance(unit)
                > Config.Instance.miscMenu["DistanceLimit"].Cast<Slider>().CurrentValue)
            {
                return;
            }

            if (!unit.IsVisible && !Config.Instance.miscMenu["StealFOW"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            //type.ToString() + x.Slot

            var spell =
                this.Spells.Where(
                    x =>
                    x.IsReady() && x.IsInRange(unit) && (type == StealType.BuffSteal ? Config.Instance.buffMenu[type.ToString() + x.Slot].Cast<CheckBox>().CurrentValue : Config.Instance.objectiveMenu[type.ToString() + x.Slot].Cast<CheckBox>().CurrentValue)
                    && x.GetMissileArrivalTime(unit) < Config.Instance.miscMenu["ETALimit"].Cast<Slider>().CurrentValue / 1000f).MaxOrDefault(x => ObjectManager.Player.LSGetSpellDamage(unit, x.Slot));

            if (spell == null)
            {
                return;
            }

            var healthPred = HealthPrediction.GetPredictedHealth(unit, spell.GetMissileArrivalTime(unit));

            if (spell.GetDamage(unit) >= healthPred)
            {
                spell.Cast(unit);
            }
        }

        #endregion
    }

    /// <summary>
    ///     The type of stealing. Used to check menu values.
    /// </summary>
    internal enum StealType
    {
        /// <summary>
        ///     The objective steal
        /// </summary>
        ObjectiveSteal,

        /// <summary>
        ///     The kill steal
        /// </summary>
        KillSteal,

        /// <summary>
        ///     The buff steal
        /// </summary>
        BuffSteal
    }
}