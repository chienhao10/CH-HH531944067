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

using Settings = MatrixLucian.Config.Misc;

namespace MatrixLucian.Modes
{
    public sealed class PermaActive : ModeBase
    {
        int currentSkin = 0;
        bool bought = false;
        int ticks = 0;
        public override bool ShouldBeExecuted()
        {
            // Since this is permaactive mode, always execute the loop
            return true;
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
                if (Config.Modes.Combo.RPressed)
                {
                    Orbwalker.DisableMovement = true;
                    SpellManager.castR();
                }
            }
            autoBuyStartingItems();
            skinChanger();
            castQSS();
            ks();
        }

        private void ks()
        {
            foreach (var enemy in EntityManager.Heroes.Enemies.Where(target => target.HealthPercent > 0 && !target.IsInvulnerable && target.IsEnemy && !target.IsPhysicalImmune && !target.IsZombie))
            {
                if (enemy.IsInRange(Player.Instance, Q.Range) && Settings.ksQ && Q.IsReady())
                {
                    if (enemy.Health < DamageLibrary.GetSpellDamage(Player.Instance, enemy, SpellSlot.Q))
                    {
                        Q.Cast(enemy);
                        return;
                    }
                }
                else if (enemy.IsInRange(Player.Instance, Q1.Range) && Settings.ksQ && Q.IsReady())
                {
                    if (enemy.Health < DamageLibrary.GetSpellDamage(Player.Instance, enemy, SpellSlot.Q))
                    {
                        SpellManager.castQ1(enemy);
                    }
                }
                else if (enemy.IsInRange(Player.Instance, Q1.Range + E.Range) && Settings.ksQ && Settings.ksE && E.IsReady() && Q.IsReady())
                {
                    if (enemy.Health < DamageLibrary.GetSpellDamage(Player.Instance, enemy, SpellSlot.Q))
                    {
                        SpellManager.dashToQ1(enemy);
                    }
                }
                else if (enemy.IsInRange(Player.Instance, W.Range) && Settings.ksW && W.IsReady())
                {
                    if (enemy.Health < DamageLibrary.GetSpellDamage(Player.Instance, enemy, SpellSlot.W))
                    {
                        if (W.GetPrediction(enemy).GetCollisionObjects<Obj_AI_Base>()[0].NetworkId == enemy.NetworkId)
                        {
                            W.Cast(W.GetPrediction(enemy).CastPosition);
                        }
                    }
                }
                else if (enemy.IsInRange(Player.Instance, R.Range) && Settings.ksR && R.IsReady() && Game.Time - Program.RCast > 3)
                {
                    if (enemy.Health < DamageLibrary.GetSpellDamage(Player.Instance, enemy, SpellSlot.R)*0.3 && R.GetPrediction(enemy).CollisionObjects.Count() > 0)
                    {
                        Program.RCastToPosition = SpellManager.R.GetPrediction((AIHeroClient)enemy).CastPosition;
                        SpellManager.R.Cast(Program.RCastToPosition);
                        Program.RTarget = enemy;
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
                int[] leveler = new int[] { 1, 3, 1, 2, 1, 4, 1, 3, 1, 3, 4, 3, 2, 3, 2, 4, 2, 2 };
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
