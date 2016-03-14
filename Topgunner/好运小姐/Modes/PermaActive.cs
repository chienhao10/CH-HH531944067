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

using Settings = MissFortune.Config.Misc;

namespace MissFortune.Modes
{
    public sealed class PermaActive : ModeBase
    {
        int currentSkin = 0;
        bool bought = false;
        int ticks = 0;
        int Rticks = 0;
        public override bool ShouldBeExecuted()
        {
            // Since this is permaactive mode, always execute the loop
            return true;
        }

        public override void Execute()
        {
            if (Combo.Rchanneling)
            {
                Rticks++;
                if (!Combo.RcameOut && Rticks > 10)
                {
                    Orbwalker.DisableAttacking = false;
                    Orbwalker.DisableMovement = false;
                    Combo.Rchanneling = false;
                }
                return;
            }
            else
            {
                Rticks = 0;
            }
            if (Player.Instance.IsRecalling())
            {
                return;
            }
            forcedTargetRefresh();
            ksWithQ();
            autoBuyStartingItems();
            skinChanger();
            castE();
            castQSS();
        }

        private void forcedTargetRefresh()
        {
            if (Orbwalker.ForcedTarget != null && !Orbwalker.ForcedTarget.IsInRange(Player.Instance, 550))
            {
                Orbwalker.ForcedTarget = null;
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

        private void castE()
        {
            if (Config.Modes.Combo.UseE)
            {
                foreach (var e in EntityManager.Heroes.Enemies.Where(e => e.IsInRange(Player.Instance, E.Range) && (e.HasBuffOfType(BuffType.Stun) || e.HasBuffOfType(BuffType.Suppression) || e.HasBuffOfType(BuffType.Snare) || e.HasBuffOfType(BuffType.Knockup)) && e.GetMovementBlockedDebuffDuration() > 1 && !e.IsDead).OrderBy(t => t.MaxHealth))
                {
                    if (e != null)
                    {
                        //E.Cast(e);
                    }
                }
            }
        }

        private void ksWithQ()
        {
            if (Settings.ksQ && Q.IsReady())
            {
                foreach (var e in EntityManager.Heroes.Enemies.Where(e => e.IsInRange(Player.Instance, Q.Range) && !e.IsDead && !e.IsInvulnerable && e.IsTargetable && !e.IsZombie && e.Health < DamageLibrary.GetSpellDamage(Player.Instance, e, SpellSlot.Q)))
                {
                    Q.Cast(e);
                    return;
                }
                SpellManager.castQ(false, false, true);
                SpellManager.castQ(false, false);
            }
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
                int[] leveler = new int[] { 1, 2, 3, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
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
