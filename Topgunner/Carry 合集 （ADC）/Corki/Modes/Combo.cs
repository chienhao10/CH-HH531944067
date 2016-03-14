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
using Settings = Corki.Config.Modes.Combo;

namespace Corki.Modes
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
            if (Settings.UseR && Player.Instance.CountEnemiesInRange(550) < 1 && Player.Instance.CountEnemiesInRange(R.Range) > 0)
            {
                var target = TargetSelector.GetTarget(R.Range, DamageType.Magical);
                if (R.GetPrediction(target).HitChance >= HitChance.Medium)
                {
                    R.Cast(R.GetPrediction(target).CastPosition);
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
            w2();
        }

        private void w2()
        {
            if (Settings.useW2 && W.IsReady())
            {
                var target = TargetSelector.GetTarget(W.Range, DamageType.Magical);
                if (target == null)
                    return;
                int counter = 0;
                foreach (var e in EntityManager.Heroes.Enemies.Where(t => t.IsInRange(Player.Instance, W.Range) && t.IsVisible && t.HealthPercent > 0 && !t.IsZombie && !t.IsInvulnerable && !t.IsInShopRange()))
                {
                    if(Prediction.Position.Collision.LinearMissileCollision(e, Player.Instance.Position.To2D(), Player.Instance.Position.Extend(target, W.Range), W.Speed, W.Width, 0))
                    {
                        counter++;
                    }

                }
                if (counter >= Settings.W2Enemies && Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name == "CarpetBombMega")
                {
                    W.Cast(target);
                }
            }
        }

        private void getVision()
        {
            if (Player.Instance.IsDead || Player.Instance.IsInvulnerable || !Player.Instance.IsTargetable || Player.Instance.IsZombie || Player.Instance.IsInShopRange())
                return;
            if (Settings.useQVision && Q.IsReady() && Program.lastTarget != null && Program.lastTarget.Position.Distance(Player.Instance) < Q.Range && Game.Time - Program.lastSeen > 2)
            {
                Q.Cast(Program.predictedPos);
            }
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
            if (Settings.UseQ && SpellManager.Q.IsReady())
            {
                SpellManager.Q.Cast(SpellManager.Q.GetPrediction((AIHeroClient)target).CastPosition);
            }
            else if (Settings.UseE && SpellManager.E.IsReady())
            {
                SpellManager.E.Cast();
            }
            else if (Settings.UseR && SpellManager.R.IsReady())
            {
                SpellManager.R.Cast(SpellManager.R.GetPrediction((AIHeroClient)target).CastPosition);
            }
        }
    }
}
