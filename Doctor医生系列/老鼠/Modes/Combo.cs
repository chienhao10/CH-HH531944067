using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using Settings = Twitch.Config.ModesMenu.Combo;
using SettingsPrediction = Twitch.Config.PredictionMenu;
using SettingsMana = Twitch.Config.ManaManagerMenu;

namespace Twitch.Modes
{
    public sealed class Combo : ModeBase
    {
        static Item Cutlass;
        static Item BOTRK;
        static Item Youmuu;

        static Combo()
        {
            Cutlass = new Item(ItemId.Bilgewater_Cutlass, 450);
            BOTRK = new Item(ItemId.Blade_of_the_Ruined_King, 450);
            Youmuu = new Item(ItemId.Youmuus_Ghostblade);
        }

        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
        }

        public override void Execute()
        {
            if (Settings.UseQ && Q.IsReady() && !QActive && PlayerMana >= SettingsMana.MinQMana)
            {
                var enemiesAround = EntityManager.Heroes.Enemies.Any(e => e.IsValidTarget(2500.0f));
                if (enemiesAround)
                {
                    Q.Cast();
                    Debug.WriteChat("Casting Q in Combo, because enemies in 2500 range.");
                }
            }
            if (Settings.UseE && E.IsReady() && PlayerMana >= SettingsMana.MinEMana)
            {
                var enemy =
                    EntityManager.Heroes.Enemies
                        .FirstOrDefault(e => e.IsValidTarget(E.Range) && EStacks(e) >= Settings.MinEStacks);
                if (enemy != null)
                {
                    E.Cast();
                    Debug.WriteChat("Casting E in Combo, Target: {0}", enemy.ChampionName);
                }
            }
            if (Settings.UseR && R.IsReady() && PlayerMana >= SettingsMana.MinRMana)
            {
                var enemiesAround =
                    EntityManager.Heroes.Enemies
                        .Count(e => e.IsValidTarget(1000.0f));
                if (enemiesAround >= Settings.MinREnemies)
                {
                    R.Cast();
                    Debug.WriteChat("Casting R in Combo, Enemies in 1000 range: {0}", "" + enemiesAround);
                }
            }
            if (Settings.UseW && W.IsReady() && PlayerMana >= SettingsMana.MinWMana)
            {
                var enemy = TargetSelector.GetTarget(W.Range, DamageType.True);
                if (enemy != null && enemy.IsValidTarget(W.Range))
                {
                    var pred = W.GetPrediction(enemy);
                    if (pred.HitChance >= SettingsPrediction.MinWHCCombo)
                    {
                        W.Cast(pred.CastPosition);
                        Debug.WriteChat("Casting W in Combo, Target: {0}", enemy.ChampionName);
                    }
                }
            }
            // Items
            if (Settings.UseItems)
            {
                if (CanUseItem(ItemId.Youmuus_Ghostblade))
                {
                    Youmuu.Cast();
                }
                var enemy = TargetSelector.GetTarget(BOTRK.Range, DamageType.Physical);
                if (enemy != null)
                {
                    if (CanUseItem(ItemId.Bilgewater_Cutlass))
                    {
                        Cutlass.Cast(enemy);
                        Debug.WriteChat("Using Bilgewater Cutlass on {0}", enemy.ChampionName);
                    } else if (CanUseItem(ItemId.Blade_of_the_Ruined_King) &&
                               enemy.HealthPercent <= Settings.MaxBOTRKHPEnemy && PlayerHealth <= Settings.MaxBOTRKHPPlayer)
                    {
                        BOTRK.Cast(enemy);
                        Debug.WriteChat("Using BOTRK on {0}", enemy.ChampionName);
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
