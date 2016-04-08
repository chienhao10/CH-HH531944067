using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using Settings = Tristana.Config.ModesMenu.Combo;
using SettingsMana = Tristana.Config.ManaManagerMenu;
using SettingsPrediction = Tristana.Config.PredictionMenu;

namespace Tristana.Modes
{
    public sealed class Combo : ModeBase
    {
        static Item Cutlass;
        static Item BOTRK;

        static Combo()
        {
            Cutlass = new Item(ItemId.Bilgewater_Cutlass, 450);
            BOTRK = new Item(ItemId.Blade_of_the_Ruined_King, 450);
        }

        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
        }

        public override void Execute()
        {
            // Items
            if (Settings.UseItems)
            {
                var enemy = TargetSelector.GetTarget(BOTRK.Range, DamageType.Physical);
                if (enemy != null)
                {
                    if (CanUseItem(ItemId.Bilgewater_Cutlass))
                    {
                        Cutlass.Cast(enemy);
                        Debug.WriteChat("Using Bilgewater Cutlass on {0}", enemy.ChampionName);
                    }
                    else if (CanUseItem(ItemId.Blade_of_the_Ruined_King) &&
                             enemy.HealthPercent <= Settings.MaxBOTRKHPEnemy && PlayerHealth <= Settings.MaxBOTRKHPPlayer)
                    {
                        BOTRK.Cast(enemy);
                        Debug.WriteChat("Using BOTRK on {0}", enemy.ChampionName);
                    }
                }
            }
            // Skills
            if (Settings.UseR && R.IsReady() && PlayerMana >= SettingsMana.MinRMana)
            {
                var target = TargetSelector.GetTarget(R.Range, DamageType.Magical);
                if (target != null)
                {
                    if (!target.HasBuffOfType(BuffType.SpellImmunity) && !target.HasBuffOfType(BuffType.SpellShield))
                    {
                        var targetHealth = target.TotalShieldHealth();
                        if (target.HasBuff("TristanaEChargeSound"))
                        {
                            targetHealth -= Damages.EDamage(target);
                        }
                        if (targetHealth < Damages.RDamage(target))
                        {
                            R.Cast(target);
                            Debug.WriteChat("Casting R in Combo to finish {0}, calculated HP to cast R: {1}, current HP: {2}.", target.ChampionName, targetHealth.ToString(), target.Health.ToString());
                            return;
                        }

                    }
                }
            }
            if (Settings.UseW && W.IsReady() && PlayerMana >= SettingsMana.MinWMana)
            {
                var target = TargetSelector.GetTarget(W.Range, DamageType.Magical);
                if (target != null)
                {
                    if (!target.HasBuffOfType(BuffType.SpellImmunity) && !target.HasBuffOfType(BuffType.SpellShield))
                    {
                        var pred = W.GetPrediction(target);
                        if (pred.HitChance >= SettingsPrediction.MinWHCCombo)
                        {
                            W.Cast(pred.CastPosition);
                            Debug.WriteChat("Casting W in Combo on {0}", target.ChampionName);
                        }
                    }
                }
            }
        }

        private bool CanUseItem(ItemId id)
        {
            return Item.HasItem(id) && Item.CanUseItem(id);
        }
    }
}
