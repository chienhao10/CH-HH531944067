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
using Settings = MatrixLucian.Config.Modes.Combo;

namespace MatrixLucian.Modes
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
            if (Game.Time - Program.RCast > 3)
            {
                Program.RTarget = null;
                Orbwalker.DisableMovement = Program.disableMovement;
            }
            else
            {
                if (Settings.trackRTarget)
                {
                    Orbwalker.DisableMovement = true;
                    SpellManager.castR();
                }
            }
            //rest in Spellblade()
            var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
            if (target == null)
                return;
            chaseE(target);
            if (Settings.UseR && SpellManager.R.IsReady() && Player.Instance.CountEnemiesInRange(500) == 0 && Game.Time - Program.RCast > 3 )
            {
                if (Settings.useYOUMOUS)
                {
                    castYoumous();
                }
                Program.RCastToPosition = SpellManager.R.GetPrediction((AIHeroClient)target).CastPosition;
                SpellManager.R.Cast(Program.RCastToPosition);
                Program.RTarget = target;
            }
            if (Settings.useBOTRK)
            {
                if (!castBOTRK())
                    castBilgewater();
            }
            getVision();
            if (Settings.UseBurstMode)
            {
                if (target == null || !(target is AIHeroClient) || target.IsDead || target.IsInvulnerable || !target.IsEnemy || target.IsPhysicalImmune || target.IsZombie)
                    return;

                var enemy = target as AIHeroClient;
                if (enemy == null)
                    return;

                if (Settings.UseE && SpellManager.E.IsReady())
                {
                    SpellManager.castE();
                }
                else if (Settings.UseQ && SpellManager.Q.IsReady())
                {
                    SpellManager.Q.Cast(enemy);
                    SpellManager.castQ1(enemy);
                }
                else if (Settings.UseW && SpellManager.W.IsReady())
                {
                    SpellManager.W.Cast(SpellManager.W.GetPrediction(enemy).CastPosition);
                }
            }
        }

        private void chaseE(AIHeroClient target)
        {
            if (Player.Instance.CountEnemiesInRange(600) == 0 && Player.Instance.CountEnemiesInRange(600 + E.Range) > 0)
            {
                E.Cast(target);
            }
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

        public static bool castYoumous()
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
            if (enemy == null)
                return;

            if (Settings.UseE && SpellManager.E.IsReady())
            {
                SpellManager.castE();
            }
            else if (Settings.UseQ && SpellManager.Q.IsReady())
            {
                SpellManager.Q.Cast(enemy);
                SpellManager.castQ1(enemy);
            }
            else if (Settings.UseW && SpellManager.W.IsReady())
            {
                SpellManager.W.Cast(SpellManager.W.GetPrediction(enemy).CastPosition);
            }
        }
    }
}
