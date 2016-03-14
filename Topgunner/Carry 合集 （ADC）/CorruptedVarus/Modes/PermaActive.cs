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

using Settings = CorruptedVarus.Config.Misc;

namespace CorruptedVarus.Modes
{
    public sealed class PermaActive : ModeBase
    {
        int currentSkin = 0;
        bool bought = false;
        int ticks = 0;
        bool following = false;
        public override bool ShouldBeExecuted()
        {
            // Since this is permaactive mode, always execute the loop
            return true;
        }

        public override void Execute()
        {
            castR();
            letQGo();
            walkToMouse();
            autoBuyStartingItems();
            skinChanger();
            castQSS();
            ks();
        }

        private void castR()
        {
            if (R.IsReady() && Config.Modes.Combo.RPressed)
            {
                var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                if (target != null && R.GetPrediction(target).HitChance >= HitChance.Medium)
                {
                    R.Cast(R.GetPrediction(target).CastPosition);
                }
            }
        }

        private void letQGo()
        {
            if (Q.IsOnCooldown)
            {
                SpellManager.isCharging = false;
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)
                || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)
                || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)
                || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear)
                )
            {
                return;
            }
            if (Q.IsFullyCharged && Q.IsCharging && SpellManager.isCharging)
            {
                var target = TargetSelector.GetTarget(Q.MaximumRange, DamageType.Physical);
                if (target != null)
                {
                    Q.Cast(Q.GetPrediction(target).CastPosition);
                    SpellManager.isCharging = false;
                }
                else
                {
                    Obj_AI_Base m0 = null;
                    foreach (var m in EntityManager.MinionsAndMonsters.CombinedAttackable.Where(t => t.IsEnemy && !t.IsDead && t.IsValid && !t.IsInvulnerable && t.IsInRange(Player.Instance.Position, Q.MaximumRange)))
                    {
                        if (m0 == null)
                        {
                            m0 = m;
                            continue;
                        }
                        if (Q.GetPrediction(m).CollisionObjects.Where(t => t.IsEnemy && !t.IsDead && t.IsValid && !t.IsInvulnerable).Count() >= Q.GetPrediction(m0).CollisionObjects.Where(t => t.IsEnemy && !t.IsDead && t.IsValid && !t.IsInvulnerable).Count())
                        {
                            m0 = m;
                        }
                    }
                    if (m0 != null)
                    {
                        Q.Cast(m0);
                        SpellManager.isCharging = false;
                    }
                }
            }
        }

        private void walkToMouse()
        {
            if (Program.disableMove || !Q.IsCharging || !SpellManager.isCharging || Q.IsOnCooldown)
            {
                if (following)
                {
                    following = false;
                    Orbwalker.ResetAutoAttack();
                }
                return;
            }
            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos); 
        }
        public static void checkQDelayedCast(AIHeroClient enemy)
        {
            if (enemy.HealthPercent == 0 || !enemy.IsInRange(Player.Instance, SpellManager.Q.MaximumRange))
            {
                SpellManager.isCharging = true;
                return;
            }
            SpellManager.Q.Cast(SpellManager.Q.GetPrediction(enemy).CastPosition);
        }
        private void ks()
        {
            foreach (var enemy in EntityManager.Heroes.Enemies.Where(target => target.HealthPercent > 0 && !target.IsInvulnerable && target.IsEnemy && !target.IsPhysicalImmune && !target.IsZombie))
            {
                if (enemy.IsInRange(Player.Instance, Q.MinimumRange) && Settings.ksQ && Q.IsReady())
                {
                    if (enemy.Health < DamageLibrary.GetSpellDamage(Player.Instance, enemy, SpellSlot.Q))
                    {
                        SpellManager.Q.StartCharging();
                        Core.DelayAction(() => checkQDelayedCast(enemy), 650);
                        break;
                    }
                } 
                if (enemy.IsInRange(Player.Instance, E.Range) && Settings.ksE && E.IsReady())
                {
                    if (enemy.Health < DamageLibrary.GetSpellDamage(Player.Instance, enemy, SpellSlot.E))
                    {
                        E.Cast(E.GetPrediction(enemy).CastPosition);
                        break;
                    }
                }
                if (enemy.IsInRange(Player.Instance, R.Range) && Settings.ksR && R.IsReady())
                {
                    if (enemy.Health < DamageLibrary.GetSpellDamage(Player.Instance, enemy, SpellSlot.R) && R.GetPrediction(enemy).CollisionObjects.Count() > 0 && R.GetPrediction(enemy).CollisionObjects[0].NetworkId == enemy.NetworkId)
                    {
                        R.Cast(R.GetPrediction(enemy).CastPosition);
                        break;
                    }
                }
                if (enemy.IsInRange(Player.Instance, Q.MinimumRange) && enemy.IsInRange(Player.Instance, E.Range) && Settings.ksQ && Settings.ksE && Q.IsReady() && E.IsReady())
                {
                    if (enemy.Health < DamageLibrary.GetSpellDamage(Player.Instance, enemy, SpellSlot.Q) + DamageLibrary.GetSpellDamage(Player.Instance, enemy, SpellSlot.E))
                    {
                        SpellManager.Q.StartCharging();
                        Core.DelayAction(() => checkQDelayedCast(enemy), 650);
                        E.Cast(E.GetPrediction(enemy).CastPosition);
                        break;
                    }
                }
                if (enemy.IsInRange(Player.Instance, Q.MinimumRange) && enemy.IsInRange(Player.Instance, R.Range) && Settings.ksQ && Settings.ksR && Q.IsReady() && R.IsReady())
                {
                    if (enemy.Health < DamageLibrary.GetSpellDamage(Player.Instance, enemy, SpellSlot.Q) + DamageLibrary.GetSpellDamage(Player.Instance, enemy, SpellSlot.R) && R.GetPrediction(enemy).CollisionObjects.Count() > 0 && R.GetPrediction(enemy).CollisionObjects[0].NetworkId == enemy.NetworkId)
                    {
                        SpellManager.Q.StartCharging();
                        Core.DelayAction(() => SpellManager.Q.Cast(SpellManager.Q.GetPrediction(enemy).CastPosition), 650);
                        R.Cast(R.GetPrediction(enemy).CastPosition);
                        break;
                    }
                }
                if (enemy.IsInRange(Player.Instance, E.Range) && enemy.IsInRange(Player.Instance, R.Range) && Settings.ksE && Settings.ksR && E.IsReady() && R.IsReady())
                {
                    if (enemy.Health < DamageLibrary.GetSpellDamage(Player.Instance, enemy, SpellSlot.E) + DamageLibrary.GetSpellDamage(Player.Instance, enemy, SpellSlot.R) && R.GetPrediction(enemy).CollisionObjects.Count() > 0 && R.GetPrediction(enemy).CollisionObjects[0].NetworkId == enemy.NetworkId)
                    {
                        E.Cast(E.GetPrediction(enemy).CastPosition);
                        R.Cast(R.GetPrediction(enemy).CastPosition);
                        break;
                    }
                }
                if (enemy.IsInRange(Player.Instance, Q.MinimumRange) && enemy.IsInRange(Player.Instance, E.Range) && enemy.IsInRange(Player.Instance, R.Range) && Settings.ksQ && Settings.ksE && Settings.ksR && Q.IsReady() && E.IsReady() && R.IsReady())
                {
                    if (enemy.Health < DamageLibrary.GetSpellDamage(Player.Instance, enemy, SpellSlot.Q) + DamageLibrary.GetSpellDamage(Player.Instance, enemy, SpellSlot.E) + DamageLibrary.GetSpellDamage(Player.Instance, enemy, SpellSlot.R) && R.GetPrediction(enemy).CollisionObjects.Count() > 0 && R.GetPrediction(enemy).CollisionObjects[0].NetworkId == enemy.NetworkId)
                    {
                        SpellManager.Q.StartCharging();
                        Core.DelayAction(() => checkQDelayedCast(enemy), 650);
                        E.Cast(E.GetPrediction(enemy).CastPosition);
                        R.Cast(R.GetPrediction(enemy).CastPosition);
                        break;
                    }
                } 
            }
        }


        private bool castQSS()
        {
            if (!Settings.useQSS)
                return false;
            if (Player.Instance.IsDead || Player.Instance.IsInvulnerable || !Player.Instance.IsTargetable || Player.Instance.IsZombie || Player.Instance.IsInShopRange())
                return false;
            InventorySlot[] inv = Player.Instance.InventoryItems;
            foreach (var item in inv)
            {
                if ((item.Id == ItemId.Quicksilver_Sash || item.Id == ItemId.Mercurial_Scimitar) && item.CanUseItem() && Player.Instance.CountEnemiesInRange(700) > 0)
                {
                    if(Player.HasBuff("PoppyDiplomaticImmunity") || Player.HasBuff("MordekaiserChildrenOfTheGrave") || Player.HasBuff("FizzMarinerDoom") || Player.HasBuff("VladimirHemoplague") || 
                        Player.HasBuff("zedulttargetmark") || Player.HasBuffOfType(BuffType.Suppression) || Player.HasBuffOfType(BuffType.Charm) || Player.HasBuffOfType(BuffType.Flee) || Player.HasBuffOfType(BuffType.Blind) || 
                        Player.HasBuffOfType(BuffType.Polymorph) || Player.HasBuffOfType(BuffType.Snare) || Player.HasBuffOfType(BuffType.Stun) || Player.HasBuffOfType(BuffType.Taunt))
                    {
                        Core.DelayAction(() => item.Cast(), 110);
                        return true;
                    }
                }
            }
            return false;
        }

        private void autoBuyStartingItems()
        {
            if (bought || ticks / Game.TicksPerSecond < 3)
            {
                ticks++;
                return;
            }
            bought = true;
            if (Settings.autoBuyStartingItems)
            {
                if (Game.MapId == GameMapId.SummonersRift)
                {
                    Shop.BuyItem(ItemId.Dorans_Blade);
                    Shop.BuyItem(ItemId.Health_Potion);
                    Shop.BuyItem(ItemId.Warding_Totem_Trinket);
                }
            }
        }

        private void skinChanger()
        {
            if (!Settings.UseSkinHack)
                return;
            if (Settings.skinId != currentSkin)
            {
                Player.Instance.SetSkinId(Settings.skinId);
                this.currentSkin = Settings.skinId;
            }
        }

        internal static void autoLevelSkills(Obj_AI_Base sender, Obj_AI_BaseLevelUpEventArgs args)
        {
            if (Settings.autolevelskills)
            {
                if (!sender.IsMe || args.Level > 17)
                {
                    return;
                }
                int[] leveler = new int[] { 1, 2, 3, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3};
                int skill = leveler[Player.Instance.Level];

                if (skill == 1)
                    Player.Instance.Spellbook.LevelSpell(SpellSlot.Q);
                else if (skill == 2)
                    Player.Instance.Spellbook.LevelSpell(SpellSlot.W);
                else if (skill == 3)
                    Player.Instance.Spellbook.LevelSpell(SpellSlot.E);
                else if (skill == 4)
                    Player.Instance.Spellbook.LevelSpell(SpellSlot.R);
                else
                    return;
            }
        }
    }
}
