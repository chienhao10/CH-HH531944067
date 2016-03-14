using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Constants;
using Settings = RoamQueenQuinn.Config.Misc;

namespace RoamQueenQuinn
{
    public class SaveMePls
    {
        private static readonly Dictionary<float, float>[] IncDamage = new Dictionary<float, float>[]
        {
            new Dictionary<float, float>(), new Dictionary<float, float>(), new Dictionary<float, float>(), new Dictionary<float, float>(), new Dictionary<float, float>()
        };
        private static readonly Dictionary<float, float>[] InstDamage = new Dictionary<float, float>[]
        {
            new Dictionary<float, float>(), new Dictionary<float, float>(), new Dictionary<float, float>(), new Dictionary<float, float>(), new Dictionary<float, float>()
        };

        public static int me = int.MaxValue;
        public static bool castOnMe = false;

        public static float getIncomingDamageForI(int i)
        {
            return IncDamage[i].Sum(e => e.Value) + InstDamage[i].Sum(e => e.Value);
        }
        public static void Initialize()
        {
            // Listen to related events
            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            for (int i = 0; i < EntityManager.Heroes.Allies.Count; i++)
            {
                if (EntityManager.Heroes.Allies[i].NetworkId == Player.Instance.NetworkId)
                {
                    me = i;
                }
            }
            if (me == int.MaxValue)
            {
                me = EntityManager.Heroes.Allies[0].NetworkId;
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (!Settings.useHeal || SpellManager.heal == null)
                return;
            for (int i = 0; i < EntityManager.Heroes.Allies.Count; i++)
            {
                // Check spell arrival
                foreach (var entry in IncDamage[i].Where(entry => entry.Key < Game.Time).ToArray())
                {
                    IncDamage[i].Remove(entry.Key);
                }

                // Instant damage removal
                foreach (var entry in InstDamage[i].Where(entry => entry.Key < Game.Time).ToArray())
                {
                    InstDamage[i].Remove(entry.Key);
                }
            }
            for (int i = 0; i < EntityManager.Heroes.Allies.Count; i++)
            {
                if (SpellManager.heal.IsReady() && Settings.useHealOnI(i) && EntityManager.Heroes.Allies[i].IsInRange(Player.Instance, SpellManager.heal.Range))
                {
                    if ((Player.Instance.HealthPercent < 5) || (getIncomingDamageForI(i) >= EntityManager.Heroes.Allies[i].Health))
                    {
                        SpellManager.heal.Cast();
                        if (i == me)
                        {
                            castOnMe = true;
                        }
                        break;
                    }
                }
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!Settings.useHeal)
                return;
            if (sender.IsEnemy || sender.IsMonster)
            {
                if ((sender.IsMinion || sender.IsMonster || args.SData.IsAutoAttack()) && args.Target != null)
                {
                    for (int i = 0; i < EntityManager.Heroes.Allies.Count; i++)
                    {
                        if (args.Target.NetworkId == EntityManager.Heroes.Allies[i].NetworkId)
                        {
                            IncDamage[i][EntityManager.Heroes.Allies[i].ServerPosition.Distance(sender.ServerPosition) / args.SData.MissileSpeed + Game.Time] = sender.GetAutoAttackDamage(EntityManager.Heroes.Allies[i]);
                        }
                    }
                }
                else if (!(sender is AIHeroClient))
                {
                    return;
                }
                else
                {
                    var attacker = sender as AIHeroClient;
                    if (attacker != null)
                    {
                        var slot = attacker.GetSpellSlotFromName(args.SData.Name);

                        if (slot != SpellSlot.Unknown)
                        {
                            if (slot == attacker.GetSpellSlotFromName("SummonerDot") && args.Target != null)
                            {
                                for (int i = 0; i < EntityManager.Heroes.Allies.Count; i++)
                                {
                                    if (args.Target.NetworkId == EntityManager.Heroes.Allies[i].NetworkId)
                                    {
                                        InstDamage[i][Game.Time + 2] = attacker.GetSummonerSpellDamage(EntityManager.Heroes.Allies[i], DamageLibrary.SummonerSpells.Ignite);
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < EntityManager.Heroes.Allies.Count; i++)
                                {
                                    if ((args.Target != null && args.Target.NetworkId == EntityManager.Heroes.Allies[i].NetworkId) || args.End.Distance(EntityManager.Heroes.Allies[i].ServerPosition) < Math.Pow(args.SData.LineWidth, 2))
                                    {
                                        InstDamage[i][Game.Time + 2] = attacker.GetSpellDamage(EntityManager.Heroes.Allies[i], slot);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}