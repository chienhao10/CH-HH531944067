using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using ExorAIO.Utilities;
using LeagueSharp.Common;

namespace ExorAIO.Champions.Nunu
{
    /// <summary>
    ///     The logics class.
    /// </summary>
    internal partial class Logics
    {
        /// <summary>
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void Automatic(EventArgs args)
        {
            if (ObjectManager.Player.IsRecalling())
            {
                return;
            }

            /// <summary>
            ///     The JungleClear Q Logic.
            /// </summary>
            if (Variables.Q.IsReady() &&
                Variables.getCheckBoxItem(Variables.QMenu, "qspell.jgc"))
            {
                if (Targets.JungleMinions.Any())
                {
                    foreach (Obj_AI_Minion minion in
                        Targets.JungleMinions.Where(
                            m =>
                                m.IsValidTarget(Variables.Q.Range) &&
                                m.Health < Variables.Q.GetDamage(m) &&
                                !m.CharData.BaseSkinName.Contains("Mini")))
                    {
                        Variables.Q.CastOnUnit(minion);
                    }
                }
            }

            /// <summary>
            ///     The Automatic Q Logic.
            /// </summary>
            if (Variables.Q.IsReady() &&
                Targets.Minions.Any() &&
                Variables.getCheckBoxItem(Variables.QMenu, "qspell.auto"))
            {
                if (ObjectManager.Player.Health +
                    30 + 45*ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level +
                    ObjectManager.Player.TotalMagicalDamage*0.75 < ObjectManager.Player.MaxHealth)
                {
                    foreach (Obj_AI_Minion minion in Targets.Minions.Where(m => m.IsValidTarget(Variables.Q.Range)))
                    {
                        Variables.Q.CastOnUnit(minion);
                    }
                }
            }

            /// <summary>
            ///     The Automatic W Logic.
            /// </summary>
            if (Variables.W.IsReady() &&
                (ObjectManager.Player.ManaPercent > ManaManager.NeededWMana ||
                 ObjectManager.Player.Buffs.Any(b => b.Name.Equals("visionary"))) &&
                Variables.getCheckBoxItem(Variables.WMenu, "wspell.auto"))
            {
                if (HeroManager.Allies.Any(h => !h.IsMe))
                {
                    foreach (var ally in HeroManager.Allies
                        .Where(
                            a =>
                                a.IsValidTarget(Variables.W.Range) &&
                                (HeroManager.Enemies.Any(t => t.IsValidTarget(1200f)) || Targets.Minions.Any()) &&
                                Variables.getCheckBoxItem(Variables.WhiteListMenu,
                                    "wspell.whitelist." + a.NetworkId))
                        .OrderBy(
                            o =>
                                ObjectManager.Player.TotalAttackDamage))
                    {
                        Variables.W.CastOnUnit(ally);
                    }
                }
                else if (Targets.Minions.Any() &&
                         GameObjects.AllyMinions.Any() &&
                         !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    foreach (
                        var minion in
                            GameObjects.AllyMinions.Where(
                                m =>
                                    m.IsValidTarget(Variables.W.Range) &&
                                    m.CharData.BaseSkinName.ToLower().Contains("super") ||
                                    m.CharData.BaseSkinName.ToLower().Contains("siege")))
                    {
                        Variables.W.CastOnUnit(minion);
                    }
                }
                else
                {
                    Variables.W.CastOnUnit(ObjectManager.Player);
                }
            }

            /// <summary>
            ///     The Semi-Automatic R Management.
            ///     Testare Semi-Auto R.
            /// </summary>
            if (Variables.R.IsReady() && Variables.getCheckBoxItem(Variables.RMenu, "rspell.boolrsa"))
            {
                if (ObjectManager.Player.CountEnemiesInRange(Variables.R.Range) > 0 &&
                    Variables.getKeyBindItem(Variables.RMenu, "rspell.keyrsa") &&
                    !ObjectManager.Player.HasBuff("AbsoluteZero"))
                {
                    Variables.R.Cast();
                }
                else if (!Variables.getKeyBindItem(Variables.RMenu, "rspell.keyrsa") &&
                         ObjectManager.Player.HasBuff("AbsoluteZero"))
                {
                    Player.IssueOrder(GameObjectOrder.MoveTo, ObjectManager.Player.Position);
                    Variables.R.Cast();
                }
            }
        }
    }
}