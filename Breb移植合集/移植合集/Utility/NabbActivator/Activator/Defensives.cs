using System;
using System.Linq;
using EloBuddy;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core.Utils;

namespace NabbActivator
{
    /// <summary>
    ///     The activator class.
    /// </summary>
    internal partial class Activator
    {
        /// <summary>
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void Defensives(EventArgs args)
        {
            if (!Vars.getCheckBoxItem(Vars.TypesMenu, "defensives"))
            {
                return;
            }

            /// <summary>
            ///     The Zeke's Herald Logic.
            /// </summary>
            if (Items.CanUseItem(3153))
            {
                if (GameObjects.AllyHeroes.Any(
                    a =>
                        a.HasBuff("itemstarksbindingbufferproc") ||
                        (!a.IsDead && a.HasBuff("rallyingbanneraurafriend"))))
                {
                    return;
                }

                if (GameObjects.AllyHeroes.OrderBy(t => t.FlatCritChanceMod).First().LSIsValidTarget(800f, false))
                {
                    Items.UseItem(3153, GameObjects.AllyHeroes.OrderBy(t => t.FlatCritChanceMod).First());
                }
            }

            /// <summary>
            ///     The Banner of Command Logic.
            /// </summary>
            if (Items.CanUseItem(3060))
            {
                if (GameObjects.AllyMinions.Any(m => m.GetMinionType() == MinionTypes.Super))
                {
                    foreach (var super in GameObjects.AllyMinions.Where(
                        m =>
                            m.LSIsValidTarget(1200f, false) &&
                            m.GetMinionType() == MinionTypes.Super))
                    {
                        Items.UseItem(3060, super);
                    }
                }
                else if (GameObjects.AllyMinions.Any(m => m.GetMinionType() == MinionTypes.Siege))
                {
                    foreach (var siege in GameObjects.AllyMinions.Where(
                        m =>
                            m.LSIsValidTarget(1200f, false) &&
                            m.GetMinionType() == MinionTypes.Siege))
                    {
                        Items.UseItem(3060, siege);
                    }
                }
            }

            /// <summary>
            ///     The Face of the Mountain Logic.
            /// </summary>
            if (Items.CanUseItem(3401))
            {
                foreach (var ally in GameObjects.AllyHeroes.Where(
                    a =>
                        a.LSIsValidTarget(500f, false) &&
                        Health.GetPrediction(a, (int)(250 + Game.Ping/2f)) <= a.MaxHealth/4))
                {
                    Items.UseItem(3401, ally);
                    return;
                }
            }

            /// <summary>
            ///     The Locket of the Iron Solari Logic.
            /// </summary>
            if (Items.CanUseItem(3190) &&
                !Items.CanUseItem(3401))
            {
                if (GameObjects.AllyHeroes.Count(
                    a =>
                        a.LSIsValidTarget(600f, false) &&
                        Health.GetPrediction(a, (int)(250 + Game.Ping/2f)) <= a.MaxHealth/1.5) >= 3)
                {
                    Items.UseItem(3190);
                    return;
                }
            }

            /// <summary>
            ///     The Zhonya's Hourglass Logic.
            /// </summary>
            if (Items.CanUseItem(3157))
            {
                if (Health.GetPrediction(ObjectManager.Player, (int)(250 + Game.Ping/2f)) <= ObjectManager.Player.MaxHealth/4)
                {
                    Items.UseItem(3157);
                    return;
                }
            }

            /// <summary>
            ///     The Wooglet's Witchcap Logic.
            /// </summary>
            if (Items.CanUseItem(3090))
            {
                if (Health.GetPrediction(ObjectManager.Player, (int)(250 + Game.Ping/2f)) <= ObjectManager.Player.MaxHealth/4)
                {
                    Items.UseItem(3090);
                    return;
                }
            }

            /// <summary>
            ///     The Seraph's Embrace Logic.
            /// </summary>
            if (Items.CanUseItem(3040))
            {
                if (Health.GetPrediction(ObjectManager.Player, (int)(250 + Game.Ping/2f)) <= ObjectManager.Player.MaxHealth/4)
                {
                    Items.UseItem(3040);
                    return;
                }
            }

            /// <summary>
            ///     The Guardian's Horn Logic.
            /// </summary>
            if (Items.CanUseItem(2051))
            {
                if (GameObjects.EnemyHeroes.Count(t => t.LSIsValidTarget(1000f)) >= 3)
                {
                    Items.UseItem(2051);
                    return;
                }
            }

            /// <summary>
            ///     The Talisman of Ascension Logic.
            /// </summary>
            if (Items.CanUseItem(3059))
            {
                if (GameObjects.EnemyHeroes.Count(
                    t =>
                        t.LSIsValidTarget(2000f) &&
                        t.CountEnemyHeroesInRange(1500f) <=
                            ObjectManager.Player.CountAllyHeroesInRange(1500f) + t.CountAllyHeroesInRange(1500f) - 1) > 1)
                {
                    Items.UseItem(3059);
                    return;
                }
            }

            /// <summary>
            ///     The Righteous Glory Logic.
            /// </summary>
            if (Items.CanUseItem(3800))
            {
                if (!ObjectManager.Player.HasBuff("ItemRighteousGlory"))
                {
                    if (GameObjects.EnemyHeroes.Count(
                        t =>
                            t.LSIsValidTarget(2000f) &&
                            t.CountEnemyHeroesInRange(1500f) <=
                                ObjectManager.Player.CountAllyHeroesInRange(1500f) + t.CountAllyHeroesInRange(1500f) - 1) > 1)
                    {
                        Items.UseItem(3800);
                        return;
                    }
                }
                else
                {
                    if (ObjectManager.Player.CountEnemyHeroesInRange(450f) >= 2)
                    {
                        Items.UseItem(3800);
                    }
                }
                return;
            }

            /// <summary>
            ///     The Randuin's Omen Logic.
            /// </summary>
            if (Items.CanUseItem(3143))
            {
                if (ObjectManager.Player.CountEnemyHeroesInRange(500f) >= 2)
                {
                    Items.UseItem(3143);
                }
            }
        }
    }
}