using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;

namespace Mordekaiser.Events
{
    internal class LaneClear
    {
        private static bool AttackingToSiegeMinion;
        private static bool AttackingToMinion;
        private static int TargetMinionNetworkId;

        public LaneClear()
        {
            Game.OnUpdate += Game_OnUpdate;
            Orbwalker.OnPreAttack += Orbwalker_OnPreAttack;
        }

        private void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                var minion = args.Target as Obj_AI_Minion;
                AttackingToMinion = minion != null;

                if (minion != null)
                {
                    TargetMinionNetworkId = minion.NetworkId;
                    AttackingToSiegeMinion = minion.BaseSkinName == "SRU_ChaosMinionSiege" ||
                                             minion.BaseSkinName == "SRU_ChaosMinionSuper";
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                return;

            ExecuteQ();
            ExecuteW();
            ExecuteE();

            UseItems();
        }

        private static void ExecuteQ()
        {
            if (Utils.Player.Self.HealthPercent <= Menu.getSliderItem(Menu.MenuQ, "UseQ.Lane.MinHeal"))
                return;

            if (!Spells.Q.IsReady())
                return;

            var lMode = Menu.getBoxItem(Menu.MenuQ, "UseQ.Lane");
            if (lMode == 0)
                return;

            var lMinions = Utils.MinionManager.GetOneMinionObject(Utils.Player.AutoAttackRange);
            if (lMinions == null)
                return;

            switch (lMode)
            {
                case 1:
                {
                    if (AttackingToMinion)
                        CastQ(lMinions);
                    break;
                }
                case 2:
                {
                    lMinions = Utils.MinionManager.GetOneMinionObject(Utils.Player.AutoAttackRange,
                        Utils.MinionManager.MinionTypes.Siege);
                    if (AttackingToSiegeMinion)
                        CastQ(lMinions);
                    break;
                }
            }
        }

        private static void ExecuteW()
        {
            var laneClearStatus = Menu.getSliderItem(Menu.MenuW, "UseW.Lane");
            if (laneClearStatus == 0)
                return;

            if (Utils.Player.Self.Spellbook.GetSpell(SpellSlot.W).Name == "mordekaisercreepingdeath2")
                return;

            var minionsW = MinionManager.GetMinions(Utils.Player.Self.Position, Spells.WDamageRadius);
            if (laneClearStatus != 7)
            {
                if (minionsW.Count >= laneClearStatus)
                {
                    Spells.W.CastOnUnit(Utils.Player.Self);
                }
            }
            else
            {
                if (minionsW.Count >= 2) // WIP
                {
                    Spells.W.CastOnUnit(Utils.Player.Self);
                }
            }
        }

        private static void ExecuteE()
        {
            if (Utils.Player.Self.HealthPercent <= Menu.getSliderItem(Menu.MenuE, "UseE.Lane.MinHeal"))
                return;

            if (!Spells.E.IsReady())
                return;

            if (!Menu.getCheckBoxItem(Menu.MenuE, "UseE.Lane"))
                return;

            var minionE = MinionManager.GetMinions(Utils.Player.Self.Position, Spells.E.Range);
            if (minionE == null)
                return;

            var minionsE = Spells.E.GetCircularFarmLocation(minionE, Spells.E.Range);
            var minionOutOfAutoAttackRange = minionE.FirstOrDefault(m => m.Health <= Spells.E.GetDamage(m));

            if (minionOutOfAutoAttackRange != null)
            {
                if (Utils.Player.Self.LSDistance(minionOutOfAutoAttackRange) > Utils.Player.AutoAttackRange)
                {
                    Spells.E.Cast(minionOutOfAutoAttackRange);
                    return;
                }
                if (minionOutOfAutoAttackRange.NetworkId != TargetMinionNetworkId)
                {
                    Spells.E.Cast(minionOutOfAutoAttackRange);
                    return;
                }
            }

            if (minionsE.MinionsHit > 1 && Spells.E.IsInRange(minionsE.Position.To3D()))
            {
                Spells.E.Cast(minionsE.Position);
            }
        }

        public static void UseItems()
        {
            if (!Menu.getCheckBoxItem(Menu.MenuItems, "Items.Lane"))
                return;

            foreach (var item in from item in Items.ItemDb
                where
                    item.Value.ItemType == Items.EnumItemType.AoE &&
                    item.Value.TargetingType == Items.EnumItemTargettingType.EnemyObjects
                let iMinions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, item.Value.Item.Range)
                where
                    iMinions.Count >= 2 && item.Value.Item.IsReady() &&
                    iMinions[0].LSDistance(Utils.Player.Self.Position) < item.Value.Item.Range
                select item)
            {
                item.Value.Item.Cast();
            }
        }

        public static void CastQ(Obj_AI_Base t)
        {
            if (!t.IsValidTarget(Utils.Player.AutoAttackRange))
                return;

            Spells.Q.Cast();
        }
    }
}