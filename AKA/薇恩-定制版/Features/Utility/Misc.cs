using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using EloBuddy.SDK.Events;


namespace Auto_Carry_Vayne.Features.Utility
{
    class Misc
    {
        #region Skinhack
        public static void Skinhack()
        {
            if (Manager.MenuManager.Skinhack)
            {
                Player.SetSkinId((int)Manager.MenuManager.SkinId);
            }
        }
        #endregion Skinhack
        #region AutoBuyTrinkets
        public static void AutobuyTrinkets()
        {
            if (Game.MapId == GameMapId.SummonersRift && Manager.MenuManager.AutobuyTrinkets)
            {
                if (Variables._Player.IsInShopRange() &&
                    Variables._Player.Level > 9 && Item.HasItem((int)ItemId.Warding_Totem_Trinket))
                {
                    Shop.BuyItem(ItemId.Farsight_Alteration);
                }
                if (Variables._Player.IsInShopRange() &&
                    !Item.HasItem((int)ItemId.Sweeping_Lens_Trinket, Variables._Player) && Variables._Player.Level > 6 &&
                    EntityManager.Heroes.Enemies.Any(
                        h =>
                            h.BaseSkinName == "Rengar" || h.BaseSkinName == "Talon" ||
                            h.BaseSkinName == "Vayne"))
                {
                    Shop.BuyItem(ItemId.Sweeping_Lens_Trinket);
                }
            }
        }
        #endregion AutoBuyTrinkets
        #region AutoLevelUp
        public static void AutoLevelUp()
        {
            switch (Variables.Misc ? Manager.MenuManager.AutolvlSlider : 0)
            {
                case 0:
                    Variables.AbilitySequence = new[] { 1, 3, 2, 2, 2, 4, 2, 1, 2, 1, 4, 1, 1, 3, 3, 4, 3, 3 };
                    break;
                case 1:
                    Variables.AbilitySequence = new[] { 1, 3, 2, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
                    break;
            }

            if (!Manager.MenuManager.Autolvl) return;

            var qL = Variables._Player.Spellbook.GetSpell(SpellSlot.Q).Level + Variables.QOff;
            var wL = Variables._Player.Spellbook.GetSpell(SpellSlot.W).Level + Variables.WOff;
            var eL = Variables._Player.Spellbook.GetSpell(SpellSlot.E).Level + Variables.EOff;
            var rL = Variables._Player.Spellbook.GetSpell(SpellSlot.R).Level + Variables.ROff;
            if (qL + wL + eL + rL >= Variables._Player.Level) return;
            int[] level = { 0, 0, 0, 0 };
            for (var i = 0; i < Variables._Player.Level; i++)
            {
                level[Variables.AbilitySequence[i] - 1] = level[Variables.AbilitySequence[i] - 1] + 1;
            }
            if (qL < level[0]) Variables._Player.Spellbook.LevelSpell(SpellSlot.Q);
            if (wL < level[1]) Variables._Player.Spellbook.LevelSpell(SpellSlot.W);
            if (eL < level[2]) Variables._Player.Spellbook.LevelSpell(SpellSlot.E);
            if (rL < level[3]) Variables._Player.Spellbook.LevelSpell(SpellSlot.R);
        }
        #endregion AutoLevelUp
        #region AutoBuyStarters
        public static void AutobuyStartes()
        {
            if (!Manager.MenuManager.AutobuyStarters) return;

            if (Variables.bought || Variables.ticks / Game.TicksPerSecond < 3)
            {
                Variables.ticks++;
                return;
            }

            Variables.bought = true;
            if (Game.MapId == GameMapId.SummonersRift && Variables._Player.Level == 1)
            {
                Shop.BuyItem(ItemId.Dorans_Blade);
                Shop.BuyItem(ItemId.Health_Potion);
                Shop.BuyItem(ItemId.Warding_Totem_Trinket);

            }
        }
        #endregion AutoBuyStarters
        #region LowLifeE
        public static void LowlifeE()
        {
            var meleeEnemies = EntityManager.Heroes.Enemies.FindAll(m => m.IsMelee);

            if (meleeEnemies.Any() && Manager.SpellManager.E.IsReady() && Manager.MenuManager.LowLifeE && Variables._Player.HealthPercent > Manager.MenuManager.LowLifeESlider)
            {
                var mostDangerous =
                    meleeEnemies.OrderByDescending(m => m.GetAutoAttackDamage(Variables._Player)).First();
                Manager.SpellManager.E.Cast(mostDangerous);
            }
        }
        #endregion LowLifeE
        #region QKS
        public static void QKs()
        {
            var currentTarget = TargetSelector.GetTarget((int)Variables._Player.GetAutoAttackRange(),
    DamageType.Physical);

            if (!currentTarget.IsValidTarget() || currentTarget.IsZombie || currentTarget.IsInvulnerable || currentTarget.IsDead)
            {
                return;
            }

            if (currentTarget.ServerPosition.Distance(Variables._Player.ServerPosition) <=
                Variables._Player.GetAutoAttackRange())
            {
                return;
            }

            if (currentTarget.Health <
                Variables._Player.GetAutoAttackDamage(currentTarget) +
                Variables._Player.GetSpellDamage(currentTarget, SpellSlot.Q)
                && currentTarget.Health > 0)
            {
                var extendedPosition = (Vector3)Variables._Player.ServerPosition.Extend(
                    currentTarget.ServerPosition, 300f);
                //    if (extendedPosition.IsSafe())
                {
                    Player.CastSpell(SpellSlot.Q, extendedPosition);
                }
            }
        }
        #endregion QKS
        #region GapcloseQandE

        public static void Gapclose(AIHeroClient sender, EloBuddy.SDK.Events.Gapcloser.GapcloserEventArgs e)
        {
            if (sender == null || sender.IsAlly) return;

            if ((e.End.Distance(Variables._Player) <= 70) && Manager.MenuManager.GapcloseE)
            {
                Manager.SpellManager.E.Cast(sender);
            }

            if ((e.End.Distance(Variables._Player) <= 70) && Manager.MenuManager.GapcloseQ)
            {
                var QPos = e.End.Extend(Variables._Player.Position, Manager.SpellManager.Q.Range);
                Player.CastSpell(SpellSlot.Q, QPos.To3D());
            }
        }
        #endregion GapcloseQandE
        #region InterruptE
        public static void Interrupt(Obj_AI_Base sender,
    Interrupter.InterruptableSpellEventArgs e)
        {
            if (Extensions.IsValidTarget(e.Sender) && e.DangerLevel == EloBuddy.SDK.Enumerations.DangerLevel.High)
            {
                Manager.SpellManager.E.Cast(e.Sender);
            }
        }
        #endregion InterruptE
        #region FocusW
        public static void FocusW()
        {
            if (Manager.MenuManager.FocusW)
            {
                if (FocusWTarget == null &&
                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) ||
                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                {
                    return;
                }
                if (FocusWTarget.IsValidTarget(Variables._Player.GetAutoAttackRange()) &&
                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) ||
                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                {
                    TargetSelector.GetPriority(FocusWTarget);
                }
                else
                {
                    TargetSelector.GetPriority(
                        TargetSelector.GetTarget(Variables._Player.AttackRange, DamageType.Physical));
                }
            }
        }

        private static AIHeroClient FocusWTarget
        {
            get
            {
                return ObjectManager.Get<AIHeroClient>()
                    .Where(
                        enemy =>
                            !enemy.IsDead &&
                            enemy.IsValidTarget((Manager.SpellManager.Q.IsReady() ? Manager.SpellManager.Q.Range : 0) + Variables._Player.AttackRange))
                    .FirstOrDefault(
                        enemy => enemy.Buffs.Any(buff => buff.Name == "vaynesilvereddebuff" && buff.Count > 0));
            }
        }
        #endregion FocusW
        #region AutoE
        public static void AutoE()
        {
            var ctarget = Logic.Condemn.GetTarget(ObjectManager.Player.Position);
            if (ctarget == null) return;
            if (Manager.MenuManager.AutoE && Manager.MenuManager.UseE && Manager.SpellManager.E.IsReady())
            {
                Manager.SpellManager.E.Cast(ctarget);
            }
        }
    }
    #endregion AutoE
}

