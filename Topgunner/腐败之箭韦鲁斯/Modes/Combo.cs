using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;

// Using the config like this makes your life easier, trust me
using Settings = CorruptedVarus.Config.Modes.Combo;

namespace CorruptedVarus.Modes
{
    public sealed class Combo : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            // Only execute this mode when the orbwalker is on combo mode
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
        }

        public override void Execute()
        {
            //rest in Spellblade()
            if (Settings.UseQ && Q.IsReady() && Player.Instance.CountEnemiesInRange(700) < 1 && Player.Instance.CountEnemiesInRange(1400) > 0)
            {
                var target = TargetSelector.GetTarget(1400, DamageType.Physical);
                if (target != null)
                {
                    if (!Q.IsCharging || !SpellManager.isCharging)
                    {
                        Q.StartCharging();
                        SpellManager.isCharging = true;
                    }
                }
            }
            if (Settings.UseQ && Q.IsFullyCharged && Q.IsCharging && SpellManager.isCharging)
            {
                var target = TargetSelector.GetTarget(1400, DamageType.Physical);
                if (target != null)
                {
                    Q.Cast(Q.GetPrediction(target).CastPosition);
                    SpellManager.isCharging = false;
                }
            }
            if (Settings.UseQ && Q.IsReady() && Q.IsFullyCharged && Q.IsCharging && SpellManager.isCharging)
            {
                var target = TargetSelector.GetTarget(1400, DamageType.Physical);
                if (target != null)
                {
                    Q.Cast(Q.GetPrediction(target).CastPosition);
                    SpellManager.isCharging = false;
                }
            }
            if (Settings.UseQ && Q.IsReady() && Settings.UseQInstant)
            {
                var target = TargetSelector.GetTarget(Q.MinimumRange, DamageType.Physical);
                if (target != null && Q.GetPrediction(target).HitChance >= HitChance.Medium)
                {
                    SpellManager.Q.StartCharging();
                    Core.DelayAction(() => PermaActive.checkQDelayedCast(target), 650);
                }
            }
            if (Settings.UseR && R.IsReady() && Settings.UseRInstant && (!Q.IsCharging || !SpellManager.isCharging))
            {
                var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                if (target != null && R.GetPrediction(target).HitChance >= HitChance.Medium)
                {
                    R.Cast(R.GetPrediction(target).CastPosition);
                }
            }
            if (Settings.UseE && E.IsReady() && Settings.UseEInstant && (!Q.IsCharging || !SpellManager.isCharging))
            {
                var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                if (target != null)
                {
                    E.Cast(E.GetPrediction(target).CastPosition);
                }
            }
            if (Settings.useBOTRK)
            {
                if (!castBOTRK())
                    castBilgewater();
            }
            if (Settings.useYOUMOUS)
            {
                castYoumous();
            }
            getVision();
        }

        private void getVision()
        {
            if (Player.Instance.IsDead || Player.Instance.IsInvulnerable || !Player.Instance.IsTargetable || Player.Instance.IsZombie || Player.Instance.IsInShopRange())
                return;
            else if (Settings.useTrinketVision && Program.lastTarget != null && Program.lastTarget.Position.Distance(Player.Instance) < 600 && Game.Time - Program.lastSeen > 2)
            {
                InventorySlot[] inv = Player.Instance.InventoryItems;
                foreach (var item in inv)
                {
                    if (item.Id == ItemId.Greater_Stealth_Totem_Trinket || item.Id == ItemId.Greater_Vision_Totem_Trinket || item.Id == ItemId.Warding_Totem_Trinket)
                    {
                        item.Cast(Program.predictedPos);
                    }
                }
            }
            else if (Settings.useWardVision && Program.lastTarget != null && Program.lastTarget.Position.Distance(Player.Instance) < R.Range && Game.Time - Program.lastSeen > 2)
            {
                InventorySlot[] inv = Player.Instance.InventoryItems;
                foreach (var item in inv)
                {
                    if (item.Id == ItemId.Vision_Ward)
                    {
                        item.Cast(Program.predictedPos);
                    }
                }
            }
        }

        private bool castYoumous()
        {
            if (Player.Instance.IsDead || !Player.Instance.CanCast || Player.Instance.IsInvulnerable || !Player.Instance.IsTargetable || Player.Instance.IsZombie || Player.Instance.IsInShopRange())
                return false;
            InventorySlot[] inv = Player.Instance.InventoryItems;
            foreach (var item in inv)
            {
                if ((item.Id == ItemId.Youmuus_Ghostblade) && item.CanUseItem() && Player.Instance.CountEnemiesInRange(700) > 0)
                {
                    return item.Cast();
                }
            }
            return false;
        }

        private bool castBilgewater()
        {
            if (Player.Instance.IsDead || !Player.Instance.CanCast || Player.Instance.IsInvulnerable || !Player.Instance.IsTargetable || Player.Instance.IsZombie || Player.Instance.IsInShopRange())
                return false;
            InventorySlot[] inv = Player.Instance.InventoryItems;
            foreach (var item in inv)
            {
                if ((item.Id == ItemId.Bilgewater_Cutlass) && item.CanUseItem())
                {
                    var target = TargetSelector.GetTarget(550, DamageType.Magical);
                    if (target != null)
                        return item.Cast(target);
                }
            }
            return false;
        }

        private bool castBOTRK()
        {
            if (Player.Instance.IsDead || !Player.Instance.CanCast || Player.Instance.IsInvulnerable || !Player.Instance.IsTargetable || Player.Instance.IsZombie || Player.Instance.IsInShopRange())
                return false;
            InventorySlot[] inv = Player.Instance.InventoryItems;
            foreach (var item in inv)
            {
                if ((item.Id == ItemId.Blade_of_the_Ruined_King) && item.CanUseItem())
                {
                    var target = TargetSelector.GetTarget(550, DamageType.Physical);
                    if(target != null && Player.Instance.Health <= DamageLibrary.GetItemDamage(Player.Instance, target, ItemId.Blade_of_the_Ruined_King))
                        return item.Cast(target);
                }
            }
            return false;
        }

        internal static void Spellblade(AttackableUnit target, EventArgs args)
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                return;
            if (target == null || !(target is AIHeroClient) || target.IsDead || target.IsInvulnerable || !target.IsEnemy || target.IsPhysicalImmune || target.IsZombie)
                return;
            var enemy = target as AIHeroClient;
            //varuswdebuff
            if (Settings.UseR && SpellManager.R.IsReady() && enemy.GetBuffCount("varuswdebuff") == 2)
            {
                SpellManager.R.Cast(SpellManager.R.GetPrediction(enemy).CastPosition);
            }
            else if (Settings.UseE && SpellManager.E.IsReady() && enemy.GetBuffCount("varuswdebuff") == 2)
            {
                SpellManager.E.Cast(SpellManager.E.GetPrediction(enemy).CastPosition);
            }
            else if (Settings.UseQ && SpellManager.Q.IsReady() && enemy.GetBuffCount("varuswdebuff") == 2)
            {
                SpellManager.Q.StartCharging();
                Core.DelayAction(() => PermaActive.checkQDelayedCast(enemy), 650);
            }
        }
    }
}
