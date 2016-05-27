#region License

/*
 Copyright 2014 - 2015 Nikita Bernthaler
 IncomingDamageManager.cs is part of SFXSivir.

 SFXSivir is free software: you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation, either version 3 of the License, or
 (at your option) any later version.

 SFXSivir is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with SFXSivir. If not, see <http://www.gnu.org/licenses/>.
*/

#endregion License

#region



#endregion

namespace ElUtilitySuite.Vendor.SFX
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EloBuddy;
    using LeagueSharp.Common;
    using EloBuddy.SDK;
    public static class IncomingDamageManager
    {
        private static readonly Dictionary<int, float> IncomingDamages = new Dictionary<int, float>();
        private static int _removeDelay = 300;

        static IncomingDamageManager()
        {
            Obj_AI_Base.OnProcessSpellCast += OnObjAiBaseProcessSpellCast;
        }

        public static bool Skillshots { get; set; }
        public static bool Enabled { get; set; }

        public static int RemoveDelay
        {
            get { return _removeDelay; }
            set { _removeDelay = value; }
        }

        private static void OnObjAiBaseProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            try
            {
                if (!Enabled)
                {
                    return;
                }
                var enemy = sender as AIHeroClient;
                var turret = sender as Obj_AI_Turret;
                foreach (
                    var hero in EntityManager.Heroes.AllHeroes.Where(h => h.IsValid && IncomingDamages.ContainsKey(h.NetworkId)))
                {
                    if (ShouldReset(hero))
                    {
                        IncomingDamages[hero.NetworkId] = 0;
                        continue;
                    }

                    if (enemy != null && enemy.IsValid && enemy.IsEnemy && enemy.LSDistance(hero) <= 2000f)
                    {
                        if (args.Target != null && args.Target.NetworkId.Equals(hero.NetworkId))
                        {
                            if (args.SData.IsAutoAttack())
                            {
                                AddDamage(
                                    hero, (int)(GetTime(sender, hero, args.SData) * 0.3f),
                                    (float)sender.LSGetAutoAttackDamage(hero, true));
                            }
                            else if (args.SData.TargettingType == SpellDataTargetType.Unit ||
                                     args.SData.TargettingType == SpellDataTargetType.SelfAndUnit)
                            {
                                AddDamage(
                                    hero, (int)(GetTime(sender, hero, args.SData) * 0.3f),
                                    (float)sender.LSGetSpellDamage(hero, args.SData.Name));
                            }
                        }

                        if (args.Target == null && Skillshots)
                        {
                            var slot = enemy.GetSpellSlot(args.SData.Name);
                            if (slot != SpellSlot.Unknown &&
                                (slot == SpellSlot.Q || slot == SpellSlot.E || slot == SpellSlot.W ||
                                 slot == SpellSlot.R))
                            {
                                var width = Math.Min(
                                    750f,
                                    (args.SData.TargettingType == SpellDataTargetType.Cone || args.SData.LineWidth > 0) &&
                                    args.SData.TargettingType != SpellDataTargetType.Cone
                                        ? args.SData.LineWidth
                                        : (args.SData.CastRadius <= 0
                                            ? args.SData.CastRadiusSecondary
                                            : args.SData.CastRadius));
                                if (args.End.LSDistance(hero.ServerPosition) <= Math.Pow(width, 2))
                                {
                                    AddDamage(
                                        hero, (int)(GetTime(sender, hero, args.SData) * 0.6f),
                                        (float)sender.LSGetSpellDamage(hero, args.SData.Name));
                                }
                            }
                        }
                    }

                    if (turret != null && turret.IsValid && turret.IsEnemy && turret.LSDistance(hero) <= 1500f)
                    {
                        if (args.Target != null && args.Target.NetworkId.Equals(hero.NetworkId))
                        {
                            AddDamage(
                                hero, (int)(GetTime(sender, hero, args.SData) * 0.3f),
                                (float)
                                    sender.CalcDamage(
                                        hero, DamageType.Physical,
                                        sender.BaseAttackDamage + sender.FlatPhysicalDamageMod));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static bool ShouldReset(AIHeroClient hero)
        {
            try
            {
                return !hero.LSIsValidTarget(float.MaxValue, false) || hero.IsZombie ||
                       hero.HasBuffOfType(BuffType.Invulnerability) || hero.IsInvulnerable;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return false;
        }

        private static float GetTime(Obj_AI_Base sender, AIHeroClient hero, SpellData sData)
        {
            try
            {
                return (Math.Max(2, sData.CastFrame / 30f) - 100 + Game.Ping / 2000f +
                        sender.LSDistance(hero.ServerPosition) / Math.Max(500, Math.Min(5000, sData.MissileSpeed))) *
                       1000f;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return 0;
        }

        private static void AddDamage(AIHeroClient hero, int delay, float damage)
        {
            try
            {
                if (delay >= 5000 || damage <= 0)
                {
                    return;
                }
                if (delay < 0)
                {
                    delay = 0;
                }
                LeagueSharp.Common.Utility.DelayAction.Add(
                    delay, () =>
                    {
                        IncomingDamages[hero.NetworkId] += damage;
                        LeagueSharp.Common.Utility.DelayAction.Add(
                            _removeDelay,
                            () =>
                            {
                                IncomingDamages[hero.NetworkId] = IncomingDamages[hero.NetworkId] - damage < 0
                                    ? 0
                                    : IncomingDamages[hero.NetworkId] - damage;
                            });
                    });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void AddChampion(AIHeroClient hero)
        {
            try
            {
                if (IncomingDamages.ContainsKey(hero.NetworkId))
                {
                    throw new ArgumentException(
                        string.Format("IncomingDamageManager: NetworkId \"{0}\" already exist.", hero.NetworkId));
                }

                IncomingDamages[hero.NetworkId] = 0;

                Enabled = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static float GetDamage(AIHeroClient hero)
        {
            try
            {
                float damage;
                if (IncomingDamages.TryGetValue(hero.NetworkId, out damage))
                {
                    return damage;
                }
                throw new KeyNotFoundException(
                    string.Format("IncomingDamageManager: NetworkId \"{0}\" not found.", hero.NetworkId));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return 0;
        }
    }
}