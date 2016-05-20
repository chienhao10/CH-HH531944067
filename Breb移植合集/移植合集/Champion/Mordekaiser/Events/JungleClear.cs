using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;

namespace Mordekaiser.Events
{
    internal class JungleClear
    {
        public JungleClear()
        {
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Utils.Player.Self.IsDead)
            {
                return;
            }

            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                return;
            }

            ExecuteQ();
            ExecuteW();
            ExecuteE();
            UseItems();
        }

        private static void ExecuteQ()
        {
            if (Utils.Player.Self.HealthPercent <= Menu.getSliderItem(Menu.MenuQ, "UseQ.Jungle.MinHeal"))
            {
                return;
            }

            if (!Spells.Q.IsReady())
            {
                return;
            }

            var jMobs = Utils.MinionManager.GetMobs(Utils.Player.AutoAttackRange);

            if (jMobs == null)
            {
                return;
            }

            var jMode = Menu.getBoxItem(Menu.MenuQ, "UseQ.Jungle");
            switch (jMode)
            {
                case 1:
                {
                    CastQ(jMobs);
                    break;
                }
                case 2:
                {
                    jMobs = Utils.MinionManager.GetMobs(Utils.Player.AutoAttackRange,
                        Utils.MinionManager.MobTypes.BigBoys);
                    CastQ(jMobs);
                    break;
                }
            }
        }

        private static void ExecuteW()
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                return;

            if (Utils.Player.Self.Spellbook.GetSpell(SpellSlot.W).Name == "mordekaisercreepingdeath2")
                return;

            if (!Menu.getCheckBoxItem(Menu.MenuW, "UseW.Jungle"))
                return;

            var minionsW = MinionManager.GetMinions(Utils.Player.Self.Position, Spells.WDamageRadius, MinionTypes.All,
                MinionTeam.Neutral);
            if (minionsW.Count > 0)
            {
                Spells.W.CastOnUnit(Utils.Player.Self);
            }
        }

        private static void ExecuteE()
        {
            if (Utils.Player.Self.HealthPercent <= Menu.getSliderItem(Menu.MenuE, "UseE.Jungle.MinHeal"))
            {
                return;
            }

            if (!Spells.E.IsReady() || !Menu.getCheckBoxItem(Menu.MenuE, "UseE.Jungle"))
            {
                return;
            }

            var minionE = MinionManager.GetMinions(Utils.Player.Self.Position, Spells.E.Range, MinionTypes.All,
                MinionTeam.Neutral);

            if (minionE == null)
            {
                return;
            }

            var minionsE = Spells.E.GetCircularFarmLocation(minionE, Spells.E.Range);
            var minionOutOfAutoAttackRange =
                minionE.FirstOrDefault(m => m.Health > Utils.Player.Self.TotalAttackDamage);

            if (minionOutOfAutoAttackRange != null)
            {
                if (Utils.Player.Self.LSDistance(minionOutOfAutoAttackRange) > Utils.Player.AutoAttackRange)
                {
                    Spells.E.Cast(minionOutOfAutoAttackRange);
                    return;
                }
            }

            if (minionsE.MinionsHit > 0 && Spells.E.IsInRange(minionsE.Position.To3D()))
            {
                Spells.E.Cast(minionsE.Position);
            }
        }

        public static void UseItems()
        {
            if (!Menu.getCheckBoxItem(Menu.MenuItems, "Items.Jungle"))
            {
                return;
            }

            foreach (var item in from item in Items.ItemDb
                where
                    item.Value.ItemType == Items.EnumItemType.AoE &&
                    item.Value.TargetingType == Items.EnumItemTargettingType.EnemyObjects
                let iMinions =
                    MinionManager.GetMinions(ObjectManager.Player.ServerPosition, item.Value.Item.Range, MinionTypes.All,
                        MinionTeam.Neutral)
                where
                    item.Value.Item.IsReady() &&
                    iMinions[0].LSDistance(Utils.Player.Self.Position) < item.Value.Item.Range
                select item)
            {
                item.Value.Item.Cast();
            }
        }

        public static void CastQ(Obj_AI_Base t)
        {
            if (!t.IsValidTarget(Utils.Player.AutoAttackRange))
            {
                return;
            }

            Spells.Q.Cast();
        }
    }
}