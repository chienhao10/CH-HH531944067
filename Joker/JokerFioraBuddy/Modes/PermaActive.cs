using System.Linq;
using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Rendering;
using SharpDX;

using Settings = JokerFioraBuddy.Config.Modes.Perma;
using ComboSettings = JokerFioraBuddy.Config.Modes.Combo;



namespace JokerFioraBuddy.Modes
{
    public sealed class PermaActive : ModeBase
    {

        public override bool ShouldBeExecuted()
        {
            
            return true;
        }
        
        public override void Execute()
        {
            if(Config.Drawings.ShowKillable)
                DamageIndicator.DamageToUnit = GetComboDamage;

            if (Player.Instance.GetSpellSlotFromName("summonerdot") == SpellSlot.Summoner1 ||
                Player.Instance.GetSpellSlotFromName("summonerdot") == SpellSlot.Summoner2)
            {
                if (ObjectManager.Player.IsDead || !IG.IsReady() || !Settings.UseIgnite) return;
                if (ObjectManager.Get<AIHeroClient>().Where(
                    h =>
                        h.IsValidTarget(IG.Range) &&
                        h.Health <
                        ObjectManager.Player.GetSummonerSpellDamage(h, DamageLibrary.SummonerSpells.Ignite)).Count() <=
                    0) return;

                var target = ObjectManager.Get<AIHeroClient>()
                    .Where(
                        h =>
                            h.IsValidTarget(IG.Range) &&
                            h.Health <
                            ObjectManager.Player.GetSummonerSpellDamage(h, DamageLibrary.SummonerSpells.Ignite));
                if (Config.Modes.Perma.igniteMode.Equals("0"))
                    IG.Cast(target.First());
                else
                {
                    if (target.First().Distance(Player.Instance) > 450 || (Player.Instance.HealthPercent < 25))
                    {
                      
                        IG.Cast(target.First());
                    }
                }
            }

            if (Settings.UseW)
            {
                var target = TargetSelector.GetTarget(W.Range, DamageType.Magical);

                if (target != null && target.IsEnemy && target.Distance(Player.Instance.Position) > Player.Instance.AttackRange && (DamageLibrary.GetSpellDamage(Player.Instance,target,SpellSlot.W)) > target.Health)
                {
                    W.Cast(target.Position);
                }
            }
        }

        private static float GetComboDamage(AIHeroClient unit)
        {
            return GetComboDamage(unit, 0);
        }

        private static float GetComboDamage(AIHeroClient unit, int maxStacks)
        {
            var d = 2 * Player.Instance.GetAutoAttackDamage(unit);

            if (ItemManager.BOTRK.IsReady() && ItemManager.BOTRK.IsOwned())
                d += Player.Instance.GetItemDamage(unit, ItemId.Blade_of_the_Ruined_King);

            if (ItemManager.Cutl.IsReady() && ItemManager.Cutl.IsOwned())
                d += Player.Instance.GetItemDamage(unit, ItemId.Bilgewater_Cutlass);

            if (ItemManager.RavenousHydra.IsReady() && ItemManager.RavenousHydra.IsOwned())
                d += Player.Instance.GetItemDamage(unit, ItemId.Ravenous_Hydra_Melee_Only);

            if (ItemManager.TitanicHydra.IsReady() && ItemManager.TitanicHydra.IsOwned())
                d += Player.Instance.GetItemDamage(unit, ItemId.Titanic_Hydra);

            if (ItemManager.Tiamat.IsReady() && ItemManager.Tiamat.IsOwned())
                d += Player.Instance.GetItemDamage(unit, ItemId.Ravenous_Hydra_Melee_Only);

            if (ItemManager.Sheen.IsReady() && ItemManager.Sheen.IsOwned())
                d += Player.Instance.GetAutoAttackDamage(unit) + Player.Instance.BaseAttackDamage * 2;

            if (ItemManager.TriForce.IsReady() && ItemManager.TriForce.IsOwned())
                d += Player.Instance.GetAutoAttackDamage(unit) + Player.Instance.BaseAttackDamage * 3;

            if ((Player.Instance.GetSpellSlotFromName("summonerdot") == SpellSlot.Summoner1 ||
                Player.Instance.GetSpellSlotFromName("summonerdot") == SpellSlot.Summoner2) && Settings.UseIgnite && SpellManager.IG.IsReady())
                d += Player.Instance.GetSummonerSpellDamage(unit, DamageLibrary.SummonerSpells.Ignite);

            if (ComboSettings.UseQ && SpellManager.Q.IsReady())
                d += Player.Instance.GetSpellDamage(unit, SpellSlot.Q);

            if (ComboSettings.UseE && SpellManager.E.IsReady())
                d += 2 * Player.Instance.GetAutoAttackDamage(unit);

            if (maxStacks == 0)
            {
                if (SpellManager.R.IsReady())
                    d += (float)PassiveManager.GetPassiveDamage(unit, 4);
                else
                    d += (float)PassiveManager.GetPassiveDamage(unit, PassiveManager.GetPassiveCount(unit));
            }
            else
                d += (float)PassiveManager.GetPassiveDamage(unit, maxStacks);
            if (SpellManager.R.IsReady())
                d += Player.Instance.GetSpellDamage(unit, SpellSlot.R);

            return (float)d;

        }
    }
}
