using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;

namespace ScaryKalista
{
    class Items
    {
        public static Item BilgewaterCutlass { get; private set; }
        public static Item BladeOfTheRuinedKing { get; private set; }

        public static void InitItems()
        {
            BilgewaterCutlass = new Item(ItemId.Bilgewater_Cutlass);
            BladeOfTheRuinedKing = new Item(ItemId.Blade_of_the_Ruined_King);

            Game.OnTick += OnTick;
        }

        private static void OnTick(EventArgs args)
        {
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) return;

            var target = Orbwalker.LastTarget as AIHeroClient;
            if (target == null) return;

            if (BladeOfTheRuinedKing.IsOwned())
            {
                TryUseItem(BladeOfTheRuinedKing, target);
            }

            else if (BilgewaterCutlass.IsOwned())
            {
                TryUseItem(BilgewaterCutlass, target);
            }
        }

        private static void TryUseItem(Item item, AIHeroClient target)
        {
            if (!target.IsValidTarget(550)
                || !Config.ItemMenu.IsChecked("item." + item.ItemInfo.Name)
                || Config.ItemMenu.GetValue("item." + item.ItemInfo.Name + "MyHp") < Player.Instance.HealthPercent
                || Config.ItemMenu.GetValue("item." + item.ItemInfo.Name + "EnemyHP") < target.HealthPercent)
            {
                return;
            }

            var slot = Player.Instance.InventoryItems.FirstOrDefault(x => x.Id == item.Id);
            if (slot != null && Player.GetSpell(slot.SpellSlot).IsReady)
            {
                Player.CastSpell(slot.SpellSlot, target);
            }
        }
    }
}
