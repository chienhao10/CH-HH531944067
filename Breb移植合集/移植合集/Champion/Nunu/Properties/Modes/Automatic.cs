using System;
using System.Linq;
using ExorSDK.Utilities;
using LeagueSharp;
using LeagueSharp.SDK;
using EloBuddy;
using LeagueSharp.SDK.Core.Utils;
using EloBuddy.SDK;

namespace ExorSDK.Champions.Nunu
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
            if (GameObjects.Player.LSIsRecalling())
            {
                return;
            }

            /// <summary>
            ///     The Semi-Automatic R Management.
            /// </summary>
            if (Vars.R.IsReady() &&
                Vars.getCheckBoxItem(Vars.RMenu, "bool"))
            {
                if (!GameObjects.Player.HasBuff("AbsoluteZero") &&
                    GameObjects.Player.CountEnemyHeroesInRange(Vars.R.Range) > 0 &&
                    Vars.getKeyBindItem(Vars.RMenu, "key"))
                {
                    Vars.R.Cast();
                }

                if (GameObjects.Player.HasBuff("AbsoluteZero") &&
                    !Vars.getKeyBindItem(Vars.RMenu, "key"))
                {
                    Orbwalker.MoveTo(Game.CursorPos);
                    Vars.R.Cast();
                }
            }

            /// <summary>
            ///     The JungleClear Q Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                Vars.getCheckBoxItem(Vars.QMenu, "jungleclear"))
            {
                if (Targets.JungleMinions.Any())
                {
                    foreach (var minion in Targets.JungleMinions.Where(
                        m =>
                            m.LSIsValidTarget(Vars.Q.Range) &&
                            Vars.GetRealHealth(m) <
                                (float)GameObjects.Player.LSGetSpellDamage(m, SpellSlot.Q)))
                    {
                        Vars.Q.CastOnUnit(minion);
                    }
                }
            }

            /// <summary>
            ///     The Automatic Q Logic.
            /// </summary>
            if (Vars.Q.IsReady() &&
                Targets.Minions.Any() &&
                Vars.getCheckBoxItem(Vars.QMenu, "logical"))
            {
                if (GameObjects.Player.MaxHealth >
                        GameObjects.Player.Health +
                        (30 + 45 * GameObjects.Player.Spellbook.GetSpell(SpellSlot.Q).Level) +
                        GameObjects.Player.TotalMagicalDamage * 0.75)
                {
                    foreach (var minion in Targets.Minions.Where(m => m.LSIsValidTarget(Vars.Q.Range)))
                    {
                        Vars.Q.CastOnUnit(minion);
                    }
                }
            }

            /// <summary>
            ///     The Automatic W Logic.
            /// </summary>
            if (Vars.W.IsReady() &&
                Vars.getSliderItem(Vars.WMenu, "logical") != 101)
            {
                if (GameObjects.Player.ManaPercent <
                        ManaManager.GetNeededMana(Vars.W.Slot, Vars.getSliderItem(Vars.WMenu, "logical")) &&
                    !GameObjects.Player.Buffs.Any(b => b.Name.Equals("visionary")))
                {
                    return;
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    /// <summary>
                    ///     The Ally W Combo Logic.
                    /// </summary>
                    if (GameObjects.AllyHeroes.Any(a => !a.IsMe && a.LSIsValidTarget(Vars.W.Range, false) && Vars.getCheckBoxItem(Vars.WhiteListMenu, a.ChampionName.ToLower())))
                    {
                        Vars.W.CastOnUnit(GameObjects.AllyHeroes.Where(a => !a.IsMe && a.LSIsValidTarget(Vars.W.Range, false) && Vars.getCheckBoxItem(Vars.WhiteListMenu, a.ChampionName.ToLower())).OrderBy(o => o.TotalAttackDamage).First());
                    }

                    /// <summary>
                    ///     The Normal W Combo Logic.
                    /// </summary>
                    else
                    {
                        if (Targets.Target.LSIsValidTarget())
                        {
                            Vars.W.CastOnUnit(GameObjects.Player);
                        }
                    }
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                {
                    /// <summary>
                    ///     Use if There are Enemy Minions in range.
                    /// </summary>
                    if (Targets.Minions.Any() ||
                        Targets.JungleMinions.Any())
                    {
                        Vars.W.CastOnUnit(GameObjects.Player);
                    }
                }

                /// <summary>
                ///     The W Pushing Logic.
                /// </summary>
                if (Targets.Minions.Any() &&
                    GameObjects.AllyMinions.Any())
                {
                    /// <summary>
                    ///     Use if there are Super or Siege minions in W Range.
                    /// </summary>
                    foreach (var minion in GameObjects.AllyMinions.Where(m => m.LSIsValidTarget(Vars.W.Range, false)))
                    {
                        if (minion.GetMinionType() == MinionTypes.Super ||
                            minion.GetMinionType() == MinionTypes.Siege)
                        {
                            Vars.W.CastOnUnit(minion);
                        }
                    }
                }
            }
        }
    }
}