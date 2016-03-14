using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Constants;
using Settings = SivirDamage.Config.Misc;
using EloBuddy.SDK.Enumerations;

namespace SivirDamage
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

        public static List<MissileClient> blockThese = new List<MissileClient>();

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
            if (Settings.useE && SpellManager.E.IsReady())
            {
                foreach (var skillshot in Evade.Evade.GetSkillshotsAboutToHit(Player.Instance, 200))
                {
                    AIHeroClient caster = null;
                    foreach (var e in EntityManager.Heroes.Enemies.Where(t => t.ChampionName == skillshot.SpellData.ChampionName))
                    {
                        caster = e;
                    }
                    if (caster != null && (skillshot.IsGlobal || caster.Distance(Player.Instance) <= skillshot.SpellData.Range + 50))
                    {
                        if (DamageLibrary.GetSpellDamage(caster, Player.Instance, skillshot.SpellData.Slot) > Settings.minDamage / 100 * Player.Instance.MaxHealth)
                        {
                            SpellManager.E.Cast();
                            break;
                        }

                        if (skillshot.SpellData.IsDangerous)
                        {
                            SpellManager.E.Cast();
                            break;
                        }
                    }
                }
            }
            if (!Settings.useHeal || SpellManager.heal == null)
                return;
            for (int i = 0; i < EntityManager.Heroes.Allies.Count; i++)
            {
                if (SpellManager.heal.IsReady() && Settings.useHealOnI(i) && EntityManager.Heroes.Allies[i].IsInRange(Player.Instance, SpellManager.heal.Range))
                {
                    if ((Player.Instance.HealthPercent < 5) || (getIncomingDamageForI(i) >= EntityManager.Heroes.Allies[i].Health))
                    {
                        if (!Settings.useHeal)
                            return;
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
                        if (i == me && sender is AIHeroClient)
                        {
                            var attacker = sender as AIHeroClient;
                            if (attacker != null)
                            {
                                if (dangerousAA(attacker) && SpellManager.E.IsReady())
                                {
                                    SpellManager.E.Cast();
                                }
                            }

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

                                        if (i != me)
                                            continue;
                                        if (Settings.useE && (attacker.GetSpellDamage(EntityManager.Heroes.Allies[me], slot) >= Settings.minDamage / 100 * Player.Instance.MaxHealth || dangerousSpell(slot, attacker)))
                                        {
                                            //dangerous targeted spell, not covered by Evade
                                            if (args.Target != null)
                                            {
                                                SpellManager.E.Cast();
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


        private static bool dangerousAA(AIHeroClient attacker)
        {
            if (attacker.ChampionName == "Leona" && attacker.HasBuff("LeonaShieldOfDaybreak"))
            {
                return true;
            }
            if (attacker.ChampionName == "Udyr" && attacker.HasBuff("UdyrBearStance"))
            {
                return true;
            }
            if (attacker.ChampionName == "Nautilus" && attacker.HasBuff("NautilusStaggeringBlow"))
            {
                return true;
            }
            if (attacker.ChampionName == "Renekton" && attacker.HasBuff("RenektonRuthlessPredator"))
            {
                return true;
            }
            return false;
        }

        private static bool dangerousSpell(SpellSlot slot, AIHeroClient sender)
        {
            if (sender.ChampionName == "Ahri" && slot == SpellSlot.E)
            {
                return true;
            }
            if (sender.ChampionName == "Aatrox" && slot == SpellSlot.Q)
            {
                return true;
            }
            if (sender.ChampionName == "Alistar" && slot == SpellSlot.Q)
            {
                return true;
            }
            if (sender.ChampionName == "Annie" && slot == SpellSlot.R)
            {
                return true;
            }
            if (sender.ChampionName == "Ashe" && slot == SpellSlot.R)
            {
                return true;
            }
            if (sender.ChampionName == "Blitzcrank" && slot == SpellSlot.Q)
            {
                return true;
            }
            if (sender.ChampionName == "Braum" && slot == SpellSlot.R)
            {
                return true;
            }
            if (sender.ChampionName == "Bard" && slot == SpellSlot.Q)
            {
                return true;
            }
            if (sender.ChampionName == "Amumu" && slot == SpellSlot.R)
            {
                return true;
            }
            if (sender.ChampionName == "Amumu" && slot == SpellSlot.Q)
            {
                return true;
            }
            if (sender.ChampionName == "Caitlyn" && slot == SpellSlot.R)
            {
                return true;
            }
            if (sender.ChampionName == "Cassiopeia" && slot == SpellSlot.R)
            {
                return true;
            }
            if (sender.ChampionName == "Chogath" && slot == SpellSlot.Q)
            {
                return true;
            }
            if (sender.ChampionName == "Chogath" && slot == SpellSlot.R)
            {
                return true;
            }
            if (sender.ChampionName == "Darius" && slot == SpellSlot.R)
            {
                return true;
            }
            if (sender.ChampionName == "Diana" && slot == SpellSlot.R)
            {
                return true;
            }
            if (sender.ChampionName == "Draven" && slot == SpellSlot.R)
            {
                return true;
            }
            if (sender.ChampionName == "Ekko" && slot == SpellSlot.R)
            {
                return true;
            }
            if (sender.ChampionName == "Elise" && slot == SpellSlot.E)
            {
                return true;
            }
            if (sender.ChampionName == "Ezreal" && slot == SpellSlot.R)
            {
                return true;
            }
            if (sender.ChampionName == "Fiora" && slot == SpellSlot.R)
            {
                return true;
            }
            if (sender.ChampionName == "Fizz" && slot == SpellSlot.R)
            {
                return true;
            }
            if (sender.ChampionName == "Galio" && slot == SpellSlot.R)
            {
                return true;
            }
            if (sender.ChampionName == "Gnar" && slot == SpellSlot.R)
            {
                return true;
            }
            if (sender.ChampionName == "Gragas" && slot == SpellSlot.R)
            {
                return true;
            }
            if (sender.ChampionName == "Graves" && slot == SpellSlot.R)
            {
                return true;
            }
            if (sender.ChampionName == "Hecarim" && slot == SpellSlot.R)
            {
                return true;
            }
            if (sender.ChampionName == "Jinx" && slot == SpellSlot.R)
            {
                return true;
            }
            if (sender.ChampionName == "Karthus" && slot == SpellSlot.R)
            {
                return true;
            }
            if (sender.ChampionName == "Kennen" && slot == SpellSlot.R)
            {
                return true;
            }
            if (sender.ChampionName == "Leona" && slot == SpellSlot.R)
            {
                return true;
            }
            if (sender.ChampionName == "Leona" && slot == SpellSlot.Q)
            {
                return true;
            }
            if (sender.ChampionName == "Leona" && slot == SpellSlot.E)
            {
                return true;
            }
            if (sender.ChampionName == "Malphite" && slot == SpellSlot.R)
            {
                return true;
            }
            if (sender.ChampionName == "Morgana" && slot == SpellSlot.Q)
            {
                return true;
            }
            if (sender.ChampionName == "Nami" && slot == SpellSlot.Q)
            {
                return true;
            }
            if (sender.ChampionName == "Nami" && slot == SpellSlot.R)
            {
                return true;
            }
            if (sender.ChampionName == "Nautilus" && slot == SpellSlot.R)
            {
                return true;
            }
            if (sender.ChampionName == "Nautilus" && slot == SpellSlot.Q)
            {
                return true;
            }
            if (sender.ChampionName == "Orianna" && slot == SpellSlot.R)
            {
                return true;
            }
            if (sender.ChampionName == "Sejuani" && slot == SpellSlot.R)
            {
                return true;
            }
            if (sender.ChampionName == "Shen" && slot == SpellSlot.E)
            {
                return true;
            }
            if (sender.ChampionName == "Shyvana" && slot == SpellSlot.R)
            {
                return true;
            }
            if (sender.ChampionName == "Skarner" && slot == SpellSlot.R)
            {
                return true;
            }
            if (sender.ChampionName == "Sona" && slot == SpellSlot.R)
            {
                return true;
            }
            if (sender.ChampionName == "Syndra" && slot == SpellSlot.R)
            {
                return true;
            }
            if (sender.ChampionName == "Thresh" && slot == SpellSlot.Q)
            {
                return true;
            }
            if (sender.ChampionName == "Taric" && slot == SpellSlot.E)
            {
                return true;
            }
            if (sender.ChampionName == "Veigar" && slot == SpellSlot.R)
            {
                return true;
            }
            if (sender.ChampionName == "Vi" && slot == SpellSlot.R)
            {
                return true;
            }
            if (sender.ChampionName == "Zed" && slot == SpellSlot.R)
            {
                return true;
            }
            if (sender.ChampionName == "Ziggs" && slot == SpellSlot.R)
            {
                return true;
            }
            if (sender.ChampionName == "Zyra" && slot == SpellSlot.R)
            {
                return true;
            }
            return false;
        }


    }
}