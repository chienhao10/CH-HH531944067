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

using Settings = Kitelyn.Config.Misc;

namespace Kitelyn.Modes
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
            trapped();
            castR();
            autoBuyStartingItems();
            skinChanger();
            castW();
            castQSS();
        }

        private void trapped()
        {
            foreach (var e in EntityManager.Heroes.Enemies.Where(e => e.Distance(Player.Instance) < Player.Instance.GetAutoAttackRange(e)))
                foreach (var b in e.Buffs)
                {
                    if (e.HasBuff("caitlynyordletrapsight"))
                    {
                        Orbwalker.ForcedTarget = e;
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

        private void castW()
        {
            if (Settings.useWOnTP)
            {
                foreach(var e in EntityManager.Heroes.Enemies.Where(e => e.IsInRange(Player.Instance, W.Range)))
                {
                    if (e.HasBuff("summonerteleport"))
                    {
                        W.Cast(e);
                    }
                }
            }
            else if (Settings.useWOnZhonyas)
            {
                foreach (var e in EntityManager.Heroes.Enemies.Where(e => e.IsInRange(Player.Instance, W.Range)))
                {
                    if (e.HasBuff("zhonyasringshield"))
                    {
                        W.Cast(e);
                    }
                }
            }
            else if (Config.Modes.Combo.UseW)
            {
                foreach (var e in EntityManager.Heroes.Enemies.Where(e => e.IsInRange(Player.Instance, W.Range) && (e.HasBuffOfType(BuffType.Stun) || e.HasBuffOfType(BuffType.Suppression) || e.HasBuffOfType(BuffType.Snare) || e.HasBuffOfType(BuffType.Knockup)) && e.GetMovementBlockedDebuffDuration() > 1 && !e.IsDead).OrderBy(t => t.MaxHealth))
                {
                    if (e != null)
                    {
                        W.Cast(e);
                    }
                }
            }
        }

        private void castR()
        {
            if (Settings.UseR && R.IsReady())
            {
                var target = TargetSelector.GetTarget(SpellManager.RRange, DamageType.Physical);
                if (target != null && ((target.Distance(Player.Instance) > 700 && Player.Instance.CountEnemiesInRange(650) == 0) || Settings.UseRAlways) && DamageLibrary.GetSpellDamage(Player.Instance, target, SpellSlot.R)>target.Health + 2/5 * target.HPRegenRate)
                {
                    R.Cast(target);
                }
                else if (target != null && target.Distance(Player.Instance) > 700 && !Settings.UseRAlways && DamageLibrary.GetSpellDamage(Player.Instance, target, SpellSlot.R) > target.Health + 2 / 5 * target.HPRegenRate)
                {
                    /*if (Settings.useScryingOrbMarker)
                    {
                        InventorySlot[] inv = Player.Instance.InventoryItems;
                        foreach (var item in inv)
                        {
                            if (item.Id == ItemId.Farsight_Alteration)
                            {
                                item.Cast(target);
                            }
                        }
                    }*/
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
            if (args.Level == 6 || args.Level == 11 || args.Level == 16)
            {
                SpellManager.RRange += 500;
            }
            if (Settings.autolevelskills)
            {
                if (!sender.IsMe || args.Level > 17)
                {
                    return;
                }
                int[] leveler = new int[] { 2, 1, 3, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
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
