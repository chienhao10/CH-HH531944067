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
using EloBuddy.SDK.Constants;

// Using the config like this makes your life easier, trust me
using Settings = SivirDamage.Config.Modes.Combo;

namespace SivirDamage.Modes
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
            castR();
            if (Settings.UseQ && Q.IsReady())
            {
                var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                if (target != null && Q.GetPrediction(target).HitChance >= HitChance.Medium && target.Distance(Player.Instance) > 550)
                {
                    int count = 0;
                    foreach (var e in Q.GetPrediction(target).CollisionObjects)
                    {
                        if (e.NetworkId != target.NetworkId)
                        {
                            count++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    int dmgMod = 100 - count * 15;
                    if (dmgMod < 40)
                        dmgMod = 40;
                    if (dmgMod >= Settings.damageMin)
                    {
                        Q.Cast(Q.GetPrediction(target).CastPosition);
                    }
                }
            }/*
            if (Settings.UseW && W.IsReady())
            {
                var target = TargetSelector.GetTarget(500, DamageType.Physical);
                if (target != null)
                {
                    W.Cast();
                }
            }*/
            if (Settings.useBOTRK)
            {
                if (!castBOTRK())
                    castBilgewater();
            }
            if (Settings.useYOUMOUS)
            {
                castYoumous();
            }
        }

        private void castR()
        {
            if (Settings.UseR && R.IsReady() && (Player.Instance.CountEnemiesInRange(600) >= Settings.useRCount || (Player.Instance.HealthPercent < 10 && Player.Instance.CountEnemiesInRange(500) > 0)))
            {
                R.Cast();
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
                    if (target != null && Player.Instance.Health <= DamageLibrary.GetItemDamage(Player.Instance, target, ItemId.Blade_of_the_Ruined_King))
                        return item.Cast(target);
                }
            }
            return false;
        }

        internal static void PostAttack(AttackableUnit target, EventArgs args)
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                return;
            }
            if (Settings.UseW && SpellManager.W.IsReady())
            {
                var t = TargetSelector.GetTarget(600, DamageType.Physical);
                if (t != null)
                {
                    Core.DelayAction(() => SpellManager.W.Cast(), 130);
                    return;
                }
            }
            if (Settings.UseQ && SpellManager.Q.IsReady())
            {
                var t = TargetSelector.GetTarget(SpellManager.Q.Range, DamageType.Physical);
                if (t != null)
                {
                    int count = 0;
                    foreach (var e in SpellManager.Q.GetPrediction(t).CollisionObjects)
                    {
                        if (e.NetworkId != target.NetworkId)
                        {
                            count++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    int dmgMod = 100 - count * 15;
                    if (dmgMod < 40)
                        dmgMod = 40;
                    if (dmgMod >= Settings.damageMin)
                    {
                        Core.DelayAction(() => SpellManager.Q.Cast(SpellManager.Q.GetPrediction(t).CastPosition), 130);
                    }
                }
            }
        }
    }
}
