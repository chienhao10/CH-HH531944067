using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace NidaleeBuddyEvolution
{
    internal class Events
    {
        /// <summary>
        /// Updates the spell timers on update.
        /// </summary>
        /// <param name="args">The Args</param>
        public static void SpellsOnUpdate(EventArgs args)
        {
            Essentials.SpellTimer["Takedown"] = ((Essentials.TimeStamp["Takedown"] - Game.Time) > 0)
                ? (Essentials.TimeStamp["Takedown"] - Game.Time)
                : 0;

            Essentials.SpellTimer["Pounce"] = ((Essentials.TimeStamp["Pounce"] - Game.Time) > 0)
                ? (Essentials.TimeStamp["Pounce"] - Game.Time)
                : 0;

            Essentials.SpellTimer["Swipe"] = ((Essentials.TimeStamp["Swipe"] - Game.Time) > 0)
                ? (Essentials.TimeStamp["Swipe"] - Game.Time)
                : 0;

            Essentials.SpellTimer["Javelin"] = ((Essentials.TimeStamp["Javelin"] - Game.Time) > 0)
                ? (Essentials.TimeStamp["Javelin"] - Game.Time)
                : 0;

            Essentials.SpellTimer["Bushwhack"] = ((Essentials.TimeStamp["Bushwhack"] - Game.Time) > 0)
                ? (Essentials.TimeStamp["Bushwhack"] - Game.Time)
                : 0;

            Essentials.SpellTimer["Primalsurge"] = ((Essentials.TimeStamp["Primalsurge"] - Game.Time) > 0)
                ? (Essentials.TimeStamp["Primalsurge"] - Game.Time)
                : 0;
        }

        /// <summary>
        /// Steals Kills if possible
        /// </summary>
        /// <param name="args"></param>
        public static void KillSteal(EventArgs args)
        {
            var useQ = NidaleeMenu.KillStealMenu["useQH"].Cast<CheckBox>().CurrentValue;
            var predQ = NidaleeMenu.KillStealMenu["predQH"].Cast<Slider>().CurrentValue;
            var useIgnite = NidaleeMenu.KillStealMenu["useIgnite"].Cast<CheckBox>().CurrentValue;

            if (useIgnite && Program.Ignite != null)
            {
                var targetI =
                    EntityManager.Heroes.Enemies.FirstOrDefault(
                        t =>
                            t.IsValidTarget(Program.Ignite.Range) &&
                            t.Health <= Player.Instance.GetSpellDamage(t, Program.Ignite.Slot));

                if (targetI != null)
                {
                    Program.Ignite.Cast(targetI);
                }
            }

            if (Program.QHuman.IsReady() && useQ)
            {
                var target = TargetSelector.GetTarget(
                    EntityManager.Heroes.Enemies.Where(
                        t =>
                            t.IsValidTarget() && Program.QHuman.IsInRange(t) &&
                            t.Health <= Essentials.DamageLibrary.CalculateDamage(t, true, false, false, false)),
                    DamageType.Magical);

                var pred = Program.QHuman.GetPrediction(target);

                if (pred.HitChancePercent >= predQ)
                {
                    Program.QHuman.Cast(pred.CastPosition);
                }
            }
        }

        /// <summary>
        /// Steals Jungle Mobs
        /// </summary>
        /// <param name="args"></param>
        public static void JungleSteal(EventArgs args)
        {
            if (Essentials.HasSpell("smite"))
            {
                Essentials.SetSmiteSlot();
            }

            if (Game.MapId == GameMapId.SummonersRift)
            {
                var mob =
                    EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.ServerPosition,
                        Program.QHuman.Range)
                        .Where(t => Essentials.JungleMobsList.Contains(t.BaseSkinName))
                        .OrderByDescending(t => t.MaxHealth)
                        .FirstOrDefault();

                if (mob != null)
                {
                    StealJungle(mob);
                }
            }

            if (Game.MapId == GameMapId.TwistedTreeline)
            {
                var mob = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.ServerPosition,
                    Program.QHuman.Range)
                    .Where(t => Essentials.JungleMobsListTwistedTreeline.Contains(t.BaseSkinName))
                    .OrderByDescending(t => t.MaxHealth)
                    .FirstOrDefault();

                if (mob != null)
                {
                    StealJungle(mob);
                }
            }
        }

        /// <summary>
        /// Executes the Spells to steal jungle
        /// </summary>
        /// <param name="mob"></param>
        private static void StealJungle(Obj_AI_Base mob)
        {
            if (mob == null)
            {
                return;
            }

            if (!NidaleeMenu.JungleStealMenu[mob.BaseSkinName].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            var useSmite = NidaleeMenu.JungleStealMenu["useSmite"].Cast<CheckBox>().CurrentValue;
            var toggleK = NidaleeMenu.JungleStealMenu["toggleK"].Cast<KeyBind>().CurrentValue;

            if (useSmite && toggleK)
            {
                if (Program.Smite != null)
                {
                    if (mob.IsValidTarget(Program.Smite.Range)
                        && mob.Health <= Essentials.GetSmiteDamage(mob))
                    {
                        Program.Smite.Cast(mob);
                    }
                }
            }

            var useQ = NidaleeMenu.JungleStealMenu["useQH"].Cast<CheckBox>().CurrentValue;

            if (useQ && !Essentials.CatForm())
            {
                if (mob.IsValidTarget()
                    && Essentials.IsReady(Essentials.SpellTimer["Javelin"])
                    && mob.Health <= Essentials.DamageLibrary.CalculateDamage(mob, true, false, false, false))
                {
                    var pred = Program.QHuman.GetPrediction(mob);

                    if (pred != null &&
                        pred.HitChancePercent >=
                        NidaleeMenu.JungleStealMenu["predQH"].Cast<Slider>().CurrentValue)
                    {
                        Program.QHuman.Cast(pred.CastPosition);
                    }
                }
            }
        }

        /// <summary>
        /// Automatically Heals Player and Allies
        /// </summary>
        /// <param name="args"></param>
        public static void AutoE(EventArgs args)
        {
            if (Essentials.CatForm())
            {
                return;
            }

            if (!Program.EHuman.IsReady() || Player.Instance.IsRecalling())
            {
                return;
            }

            var lowestHealthAlly =
                EntityManager.Heroes.Allies.Where(a => Program.EHuman.IsInRange(a) && !a.IsMe)
                    .OrderBy(a => a.Health)
                    .FirstOrDefault();

            if (Player.Instance.HealthPercent <= NidaleeMenu.MiscMenu["autoHealPercent"].Cast<Slider>().CurrentValue &&
                NidaleeMenu.MiscMenu["autoHeal_" + Player.Instance.BaseSkinName].Cast<CheckBox>().CurrentValue)
            {
                Program.EHuman.Cast(Player.Instance);
            }

            if (lowestHealthAlly == null)
            {
                return;
            }

            if (!(lowestHealthAlly.Health <= NidaleeMenu.MiscMenu["autoHealPercent"].Cast<Slider>().CurrentValue))
            {
                return;
            }
            if (NidaleeMenu.MiscMenu["autoHeal_" + lowestHealthAlly.BaseSkinName].Cast<CheckBox>().CurrentValue)
            {
                Program.EHuman.Cast(lowestHealthAlly);
            }
        }
    }
}