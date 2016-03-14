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
using Settings = MissFortune.Config.Modes.Combo;

namespace MissFortune.Modes
{
    public sealed class Combo : ModeBase
    {
        public static bool Rchanneling = false;
        public static bool RcameOut = false;
        public override bool ShouldBeExecuted()
        {
            // Only execute this mode when the orbwalker is on combo mode
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
        }

        public override void Execute()
        {
            if (Rchanneling)
                return;
            if (castR())
                return;
            if (Settings.UseE && E.IsReady())
            {
                var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                if (target != null)
                {
                    E.Cast(target);
                }
            }
            if (Settings.UseW && W.IsReady())
            {
                if (Player.Instance.CountEnemiesInRange(550) > 0)
                {
                    W.Cast();
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
            castQ();

        }

        private bool castR()
        {
            if (Settings.UseR && R.IsReady())
            {
                var enemies = EntityManager.Heroes.Enemies.Where(e => e.IsInRange(Player.Instance, 1300) && !e.IsDead && !e.IsInvulnerable && e.IsTargetable && !e.IsZombie);
                AIHeroClient castOn = null;
                foreach (var target in enemies)
                {
                    bool lower = false;
                    if (target != null)
                    {
                        int collCount = 1;
                        foreach (var e in enemies)
                        {
                            if (e.HealthPercent < Settings.REnemiesMaxHP)
                            {
                                lower = true;
                            }
                            if (e == target)
                                continue;
                            if (Math.Abs(e.Position.AngleBetween(Player.Instance.Position) - target.Position.AngleBetween(Player.Instance.Position)) < 0.3)
                            {
                                collCount++;
                            }
                        }
                        if (!lower)
                        {
                            return false;
                        }
                        if (Settings.saveRforStunned && (target.HasBuffOfType(BuffType.Suppression) || target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Flee) || target.HasBuffOfType(BuffType.Blind) ||
                        target.HasBuffOfType(BuffType.Polymorph) || target.HasBuffOfType(BuffType.Snare) || target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Taunt)))
                        {
                            if (collCount > Settings.ROnEnemies)
                            {
                                castOn = target;
                            }
                        }
                        else if (Settings.alwaysROnStunned && (target.HasBuffOfType(BuffType.Suppression) || target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Flee) || target.HasBuffOfType(BuffType.Blind) ||
                        target.HasBuffOfType(BuffType.Polymorph) || target.HasBuffOfType(BuffType.Snare) || target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Taunt)))
                        {
                            castOn = target;
                        }
                        else if (!Settings.saveRforStunned && collCount >= Settings.ROnEnemies)
                        {
                            castOn = target;
                        }
                    }
                }
                if (castOn != null)
                {
                    Orbwalker.DisableAttacking = true;
                    Orbwalker.DisableMovement = true;
                    Rchanneling = true;
                    R.Cast(castOn);
                    return true;
                }
            }
            return false;
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

        public void castQ()
        {
            if (Settings.UseQ && Q.IsReady())
            {
                var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                if (target == null)
                    return;
                int i = -1;
                for (int j = 0; j < EntityManager.Heroes.Enemies.Count; j++)
                {
                    if (target.NetworkId == EntityManager.Heroes.Enemies[j].NetworkId)
                        i = j;
                }
                if (i == -1)
                    return;
                if (target != null && Config.Misc.UseQOnI(i))
                {
                    Q.Cast(target);
                    return;
                }
            }
            if (Settings.useQChampsOnly)
                SpellManager.castQ(true, false, true);
            else
                SpellManager.castQ(true, false, false);
        }

        internal static void Update(EventArgs args)
        {
            if (!Settings.UseRHotkey)
            {
                return;
            }
            if (Settings.RPressed)
            {
                Orbwalker.DisableAttacking = true;
                Orbwalker.DisableMovement = true;
                Rchanneling = true;
                Settings.resetRKey();
            }
            else
            {
                if (!Rchanneling)
                {
                    Orbwalker.DisableAttacking = false;
                    Orbwalker.DisableMovement = false;
                }
            }
        }
    }
}
