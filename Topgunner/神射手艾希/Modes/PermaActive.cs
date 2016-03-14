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

using Settings = AshesToAshes.Config.Misc;

namespace AshesToAshes.Modes
{
    public sealed class PermaActive : ModeBase
    {
        List<AIHeroClient> tracker = new List<AIHeroClient>();
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
            ksWithW();
            useAutoW();
            autoBuyStartingItems();
            skinChanger();
            castE();
            castQSS();
            trackEnemies();
        }

        private void trackEnemies()
        {
            if (!Settings.useE)
                return;
            foreach (var e in tracker)
            {
                if (!e.IsVisible)
                {
                    E.Cast(E.GetPrediction(e).CastPosition);
                }
            }
            tracker = new List<AIHeroClient>();
            foreach(var e in EntityManager.Heroes.Enemies.Where(t => t.Distance(Player.Instance) < 700 && !t.IsDead && !t.IsInvulnerable && t.IsTargetable && !t.IsZombie))
            {
                if (e.IsVisible)
                    tracker.Add(e);
            }
        }

        private void useAutoW()
        {
            if (Settings.useAutoW && W.IsReady() && Player.Instance.ManaPercent >= Settings.autoWMana)
            {
                var target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
                if (target != null && target.IsValidTarget() && !target.IsInvulnerable && W.GetPrediction(target).HitChance >= HitChance.High)
                {
                    W.Cast(W.GetPrediction(target).CastPosition);
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

        private void castE()
        {
            //TODO
        }

        private void ksWithW()
        {
            if (Settings.ksW && W.IsReady())
            {
                foreach (var e in EntityManager.Heroes.Enemies.Where(e => e.IsInRange(Player.Instance, W.Range) && e.Health > 0 && !e.IsInvulnerable && e.IsTargetable && !e.IsZombie && e.Health < DamageLibrary.GetSpellDamage(Player.Instance, e, SpellSlot.W)))
                {
                    if (W.GetPrediction(e).HitChance >= HitChance.Medium)
                    {
                        W.Cast(W.GetPrediction(e).CastPosition);
                        return;
                    }
                }
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
                int[] leveler = new int[] { 2, 1, 2, 3, 2, 4, 2, 1, 2, 1, 4, 1, 1, 3, 3, 4, 3, 3 };
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
