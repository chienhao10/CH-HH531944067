using System;
using System.Linq;
using System.Security.Cryptography;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using Settings = Tristana.Config.MiscMenu;
using SettingsPrediction = Tristana.Config.PredictionMenu;
using SettingsMana = Tristana.Config.ManaManagerMenu;

namespace Tristana.Modes
{
    public sealed class PermaActive : ModeBase
    {
        static Item HealthPotion;
        static Item CorruptingPotion;
        static Item RefillablePotion;
        static Item HuntersPotion;
        static Item TotalBiscuit;
        static int lastKSTime; // Prevents spamming abillities in KS mode

        static PermaActive()
        {
            HealthPotion = new Item(2003, 0);
            TotalBiscuit = new Item(2010, 0);
            CorruptingPotion = new Item(2033, 0);
            RefillablePotion = new Item(2031, 0);
            HuntersPotion = new Item(2032, 0);
            lastKSTime = Environment.TickCount;
        }

        public override bool ShouldBeExecuted()
        {
            return !Player.Instance.IsDead;
        }

        public override void Execute()
        {
            // KillSteal
            if ((Settings.KsR || Settings.KsE || Settings.KsW || (Settings.KsIgnite && HasIgnite)) && Environment.TickCount-lastKSTime > 1000)
            {
                var enemies = EntityManager.Heroes.Enemies.Where(e => e.IsValidTarget(600));
                if (Settings.KsR && R.IsReady() && PlayerMana >= SettingsMana.MinRMana)
                {
                    var target = enemies.FirstOrDefault(e => R.IsInRange(e) && e.TotalShieldHealth() < Damages.RDamage(e) && !e.HasBuffOfType(BuffType.SpellImmunity) && !e.HasBuffOfType(BuffType.SpellShield));
                    if (target != null)
                    {
                        lastKSTime = Environment.TickCount;
                        R.Cast(target);
                        Debug.Write("Casting R in KS on {0} who has {1}HP.", target.ChampionName, target.Health.ToString());
                        return;
                    }
                }
                if (Settings.KsE && E.IsReady() && PlayerMana >= SettingsMana.MinEMana)
                {
                    var target = enemies.FirstOrDefault(e => E.IsInRange(e) && e.Health < Damages.EDamage(e) && !e.HasBuffOfType(BuffType.SpellImmunity) && !e.HasBuffOfType(BuffType.SpellShield));
                    if (target != null)
                    {
                        lastKSTime = Environment.TickCount;
                        E.Cast(target);
                        Debug.Write("Casting E in KS on {0} who has {1}HP.", target.ChampionName, target.Health.ToString());
                        return;
                    }
                }
                if (Settings.KsW && W.IsReady() && PlayerMana >= SettingsMana.MinWMana)
                {
                    var target = enemies.FirstOrDefault(e => W.IsInRange(e) && e.Health < Damages.WDamage(e) && !e.HasBuffOfType(BuffType.SpellImmunity) && !e.HasBuffOfType(BuffType.SpellShield));
                    if (target != null)
                    {
                        var pred = W.GetPrediction(target);
                        if (pred.HitChance >= SettingsPrediction.MinWHCKillSteal)
                        {
                            lastKSTime = Environment.TickCount;
                            W.Cast(pred.CastPosition);
                            Debug.Write("Casting W in KS on {0} who has {1}HP.", target.ChampionName, target.Health.ToString());
                            return;
                        }
                    }
                }
                if (Settings.KsIgnite && HasIgnite && Ignite.IsReady())
                {
                    var target = enemies.FirstOrDefault(e => W.IsInRange(e) && e.Health < Damages.IgniteDmg(e));
                    if (target != null)
                    {
                            lastKSTime = Environment.TickCount;
                            Ignite.Cast(target);
                            Debug.Write("Casting Ignite in KS on {0} who has {1}HP.", target.ChampionName, target.Health.ToString());
                            return;
                    }
                }
            }
            // Dash To Cursor
            if (Settings.WToCursor)
            {
                var cursorPos = Game.CursorPos;
                if (W.IsReady())
                {
                    if (cursorPos.Distance(_PlayerPos) < 200)
                    {
                        return;
                    }
                    if (cursorPos.Distance(_PlayerPos) <= W.Range)
                    {
                        W.Cast(cursorPos);
                    }
                    else
                    {
                        Orbwalker.MoveTo(cursorPos);
                    }
                }
                else
                {
                    Orbwalker.MoveTo(cursorPos);
                }
            }

            // Potion manager
            if (Settings.Potion && !Player.Instance.IsInShopRange() && Player.Instance.HealthPercent <= Settings.potionMinHP && !(Player.Instance.HasBuff("RegenerationPotion") || Player.Instance.HasBuff("ItemCrystalFlaskJungle") || Player.Instance.HasBuff("ItemMiniRegenPotion") || Player.Instance.HasBuff("ItemCrystalFlask") || Player.Instance.HasBuff("ItemDarkCrystalFlask")))
            {
                if (Item.HasItem(HealthPotion.Id) && Item.CanUseItem(HealthPotion.Id))
                {
                    Debug.WriteChat("Using HealthPotion because below {0}% HP - have {1}% HP", String.Format("{0}", Settings.potionMinHP), String.Format("{0:##.##}", Player.Instance.HealthPercent));
                    HealthPotion.Cast();
                    return;
                }
                if (Item.HasItem(HuntersPotion.Id) && Item.CanUseItem(HuntersPotion.Id))
                {
                    Debug.WriteChat("Using HuntersPotion because below {0}% HP - have {1}% HP", String.Format("{0}", Settings.potionMinHP), String.Format("{0:##.##}", Player.Instance.HealthPercent));
                    HealthPotion.Cast();
                    return;
                }
                if (Item.HasItem(TotalBiscuit.Id) && Item.CanUseItem(TotalBiscuit.Id))
                {
                    Debug.WriteChat("Using TotalBiscuitOfRejuvenation because below {0}% HP - have {1}% HP", String.Format("{0}", Settings.potionMinHP), String.Format("{0:##.##}", Player.Instance.HealthPercent));
                    TotalBiscuit.Cast();
                    return;
                }
                if (Item.HasItem(RefillablePotion.Id) && Item.CanUseItem(RefillablePotion.Id))
                {
                    Debug.WriteChat("Using RefillablePotion because below {0}% HP - have {1}% HP", String.Format("{0}", Settings.potionMinHP), String.Format("{0:##.##}", Player.Instance.HealthPercent));
                    RefillablePotion.Cast();
                    return;
                }
                if (Item.HasItem(CorruptingPotion.Id) && Item.CanUseItem(CorruptingPotion.Id))
                {
                    Debug.WriteChat("Using CorruptingPotion because below {0}% HP - have {1}% HP", String.Format("{0}", Settings.potionMinHP), String.Format("{0:##.##}", Player.Instance.HealthPercent));
                    CorruptingPotion.Cast();
                    return;
                }
            }
            if (Player.Instance.ManaPercent <= Settings.potionMinMP && !(Player.Instance.HasBuff("RegenerationPotion") || Player.Instance.HasBuff("ItemMiniRegenPotion") || Player.Instance.HasBuff("ItemCrystalFlask") || Player.Instance.HasBuff("ItemDarkCrystalFlask")))
            {
                if (Item.HasItem(CorruptingPotion.Id) && Item.CanUseItem(CorruptingPotion.Id))
                {
                    Debug.WriteChat("Using HealthPotion because below {0}% MP - have {1}% MP", String.Format("{0}", Settings.potionMinMP), String.Format("{0:##.##}", Player.Instance.ManaPercent));
                    CorruptingPotion.Cast();
                }
            }
        }
    }
}
