#region

using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using EloBuddy;

#endregion

namespace ARAMDetFull
{

    #region Items

    public class Items
    {
        public static Item GetItemByName(string name)
        {
            return ItemsList.FirstOrDefault(i => i.GetName().Equals(name));
        }

        public static Item GetItem(int itemId)
        {
            //1036
            var itm = ItemsList.FirstOrDefault(i => i.GetId() == itemId);
            return itm ?? ItemsList.FirstOrDefault(i => i.GetId() == 1036);
        }

        public static Item GetItem(ItemId id)
        {
            return ItemsList.FirstOrDefault(i => i.GetItemId() == id);
        }

        #region PreItems

        private static readonly Item[] PreItems =
        {
            new Item(
                3001, "Abyssal Scepter", 0, 580, true, ItemTier.Advanced,
                (ItemCategory.SpellBlock & ItemCategory.SpellDamage), 0f, 0f, 0f, 0f, 70f, 0f, 0f, 50f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1026), (1033), (1033)}),
            new Item(
                3105, "Aegis of the Legion", 0, 820, true, ItemTier.Advanced,
                (ItemCategory.HealthRegen & ItemCategory.Health & ItemCategory.SpellBlock), 0f, 0f, 0f, 0f, 0f, 0f, 200f,
                20f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(1028), (1033), (1006)}),
            new Item(
                3113, "Aether Wisp", 0, 515, true, ItemTier.Advanced, (ItemCategory.SpellDamage), 0f, 0f, 0f, 0f, 30f,
                0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1052)}),
            new Item(
                1052, "Amplifying Tome", 0, 435, false, ItemTier.Basic, (ItemCategory.SpellDamage), 0f, 0f, 0f, 0f, 20f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3301, "Ancient Coin", 0, 365, false, ItemTier.Basic, (ItemCategory.ManaRegen), 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3007, "Archangel's Staff (Crystal Scar)", 0, 1120, true, ItemTier.Legendary,
                (ItemCategory.ManaRegen & ItemCategory.SpellDamage & ItemCategory.Mana), 0f, 0f, 0f, 0f, 60f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(3073), (1026)}),
            new Item(
                3003, "Archangel's Staff", 0, 1120, true, ItemTier.Legendary,
                (ItemCategory.ManaRegen & ItemCategory.SpellDamage & ItemCategory.Mana), 0f, 0f, 0f, 0f, 60f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(3070), (1026)}),
            new Item(
                3504, "Ardent Censer", 0, 550, true, ItemTier.Legendary,
                (ItemCategory.ManaRegen & ItemCategory.SpellDamage),
                0f, 0f, 0f, 0f, 40f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3114), (3113)}),
            new Item(
                3174, "Athene's Unholy Grail", 0, 880, true, ItemTier.Legendary,
                (ItemCategory.ManaRegen & ItemCategory.SpellBlock & ItemCategory.SpellDamage), 0f, 0f, 0f, 0f, 60f, 0f,
                0f, 25f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(3108), (3028)}),
            new Item(
                3005, "Atma's Impaler", 0, 700, true, ItemTier.Legendary,
                (ItemCategory.CriticalStrike & ItemCategory.Damage & ItemCategory.Armor), 0f, 0f, 0f, 0.15f, 0f, 0f, 0f,
                0f, 45f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1031), (3093)}),
            new Item(
                3093, "Avarice Blade", 0, 400, true, ItemTier.None, (ItemCategory.CriticalStrike), 0f, 0f, 0f, 0.1f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1051)}),
            new Item(
                1038, "B. F. Sword", 0, 1550, false, ItemTier.None, (ItemCategory.Damage), 0f, 50f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3060, "Banner of Command", 0, 280, true, ItemTier.None,
                (ItemCategory.HealthRegen & ItemCategory.Health & ItemCategory.SpellBlock & ItemCategory.SpellDamage),
                0f, 0f, 0f, 0f, 60f, 0f, 200f, 20f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3105), (3108)}),
            new Item(
                3102, "Banshee's Veil", 0, 1150, true, ItemTier.None,
                (ItemCategory.HealthRegen & ItemCategory.Health & ItemCategory.SpellBlock), 0f, 0f, 0f, 0f, 0f, 0f, 450f,
                55f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(3211), (1028)}),
            new Item(
                3254, "Berserker's Greaves - Enchantment: Alacrity", 0, 475, true, ItemTier.Enchantment,
                (ItemCategory.None), 0f, 0f, 45f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3006)}),
            new Item(
                3251, "Berserker's Greaves - Enchantment: Captain", 0, 600, true, ItemTier.Enchantment,
                (ItemCategory.None), 0f, 0f, 45f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3006)}),
            new Item(
                3253, "Berserker's Greaves - Enchantment: Distortion", 0, 475, true, ItemTier.Enchantment,
                (ItemCategory.None), 0f, 0f, 45f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3006)}),
            new Item(
                3252, "Berserker's Greaves - Enchantment: Furor", 0, 475, true, ItemTier.Enchantment,
                (ItemCategory.None), 0f, 0f, 45f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3006)}),
            new Item(
                3250, "Berserker's Greaves - Enchantment: Homeguard", 0, 475, true, ItemTier.Enchantment,
                (ItemCategory.None), 0f, 0f, 45f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3006)}),
            new Item(
                3006, "Berserker's Greaves", 0, 225, true, ItemTier.None, (ItemCategory.AttackSpeed), 0f, 0f, 45f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1001), (1042)}),
            new Item(
                3144, "Bilgewater Cutlass", 0, 240, true, ItemTier.None, (ItemCategory.Damage & ItemCategory.LifeSteal),
                0f, 25f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(1036), (1053)}),
            new Item(
                3188, "Blackfire Torch", 0, 970, true, ItemTier.None, (ItemCategory.SpellDamage), 0f, 0f, 0f, 0f, 80f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1026), (3108)}),
            new Item(
                3153, "Blade of the Ruined King", 0, 900, true, ItemTier.None,
                (ItemCategory.Damage & ItemCategory.AttackSpeed & ItemCategory.LifeSteal), 0f, 25f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(1042), (3144), (1042)}),
            new Item(
                1026, "Blasting Wand", 0, 860, false, ItemTier.None, (ItemCategory.SpellDamage), 0f, 0f, 0f, 0f, 40f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                1057, "negatron cloak", 0, 800, false, ItemTier.None, (ItemCategory.SpellBlock), 0f, 0f, 0f, 0f, 40f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3166, "Bonetooth Necklace", 0, 0, false, ItemTier.RengarsTrinket, (ItemCategory.None), 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3167, "Bonetooth Necklace", 0, 0, false, ItemTier.RengarsTrinket, (ItemCategory.None), 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3168, "Bonetooth Necklace", 0, 0, false, ItemTier.RengarsTrinket, (ItemCategory.None), 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3169, "Bonetooth Necklace", 0, 0, false, ItemTier.RengarsTrinket, (ItemCategory.None), 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3171, "Bonetooth Necklace", 0, 0, false, ItemTier.RengarsTrinket, (ItemCategory.None), 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3405, "Bonetooth Necklace", 0, 0, false, ItemTier.RengarsTrinket, (ItemCategory.None), 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3406, "Bonetooth Necklace", 0, 0, false, ItemTier.RengarsTrinket, (ItemCategory.None), 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3407, "Bonetooth Necklace", 0, 0, false, ItemTier.RengarsTrinket, (ItemCategory.None), 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3408, "Bonetooth Necklace", 0, 0, false, ItemTier.RengarsTrinket, (ItemCategory.None), 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3409, "Bonetooth Necklace", 0, 0, false, ItemTier.RengarsTrinket, (ItemCategory.None), 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3411, "Bonetooth Necklace", 0, 0, false, ItemTier.RengarsTrinket, (ItemCategory.None), 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3412, "Bonetooth Necklace", 0, 0, false, ItemTier.RengarsTrinket, (ItemCategory.None), 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3413, "Bonetooth Necklace", 0, 0, false, ItemTier.RengarsTrinket, (ItemCategory.None), 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3414, "Bonetooth Necklace", 0, 0, false, ItemTier.RengarsTrinket, (ItemCategory.None), 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3415, "Bonetooth Necklace", 0, 0, false, ItemTier.RengarsTrinket, (ItemCategory.None), 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3417, "Bonetooth Necklace", 0, 0, false, ItemTier.RengarsTrinket, (ItemCategory.None), 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3418, "Bonetooth Necklace", 0, 0, false, ItemTier.RengarsTrinket, (ItemCategory.None), 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3419, "Bonetooth Necklace", 0, 0, false, ItemTier.RengarsTrinket, (ItemCategory.None), 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3420, "Bonetooth Necklace", 0, 0, false, ItemTier.RengarsTrinket, (ItemCategory.None), 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3421, "Bonetooth Necklace", 0, 0, false, ItemTier.RengarsTrinket, (ItemCategory.None), 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3450, "Bonetooth Necklace", 0, 0, false, ItemTier.RengarsTrinket, (ItemCategory.None), 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3451, "Bonetooth Necklace", 0, 0, false, ItemTier.RengarsTrinket, (ItemCategory.None), 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3452, "Bonetooth Necklace", 0, 0, false, ItemTier.RengarsTrinket, (ItemCategory.None), 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3453, "Bonetooth Necklace", 0, 0, false, ItemTier.RengarsTrinket, (ItemCategory.None), 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3454, "Bonetooth Necklace", 0, 0, false, ItemTier.RengarsTrinket, (ItemCategory.None), 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3274, "Boots of Mobility - Enchantment: Alacrity", 0, 475, true, ItemTier.Enchantment,
                (ItemCategory.None), 0f, 0f, 105f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3117)}),
            new Item(
                3271, "Boots of Mobility - Enchantment: Captain", 0, 600, true, ItemTier.Enchantment,
                (ItemCategory.None), 0f, 0f, 105f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3117)}),
            new Item(
                3273, "Boots of Mobility - Enchantment: Distortion", 0, 475, true, ItemTier.Enchantment,
                (ItemCategory.None), 0f, 0f, 105f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3117)}),
            new Item(
                3272, "Boots of Mobility - Enchantment: Furor", 0, 475, true, ItemTier.Enchantment, (ItemCategory.None),
                0f, 0f, 105f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3117)}),
            new Item(
                3270, "Boots of Mobility - Enchantment: Homeguard", 0, 475, true, ItemTier.Enchantment,
                (ItemCategory.None), 0f, 0f, 105f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3117)}),
            new Item(
                3117, "Boots of Mobility", 0, 475, true, ItemTier.None, (ItemCategory.None), 0f, 0f, 105f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1001)}),
            new Item(
                1001, "Boots of Speed", 0, 325, false, ItemTier.None, (ItemCategory.None), 0f, 0f, 25f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3284, "Boots of Swiftness - Enchantment: Alacrity", 0, 475, true, ItemTier.Enchantment,
                (ItemCategory.None), 0f, 0f, 60f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3009)}),
            new Item(
                3281, "Boots of Swiftness - Enchantment: Captain", 0, 600, true, ItemTier.Enchantment,
                (ItemCategory.None), 0f, 0f, 60f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3009)}),
            new Item(
                3283, "Boots of Swiftness - Enchantment: Distortion", 0, 475, true, ItemTier.Enchantment,
                (ItemCategory.None), 0f, 0f, 60f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3009)}),
            new Item(
                3282, "Boots of Swiftness - Enchantment: Furor", 0, 475, true, ItemTier.Enchantment,
                (ItemCategory.None), 0f, 0f, 60f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3009)}),
            new Item(
                3280, "Boots of Swiftness - Enchantment: Homeguard", 0, 475, true, ItemTier.Enchantment,
                (ItemCategory.None), 0f, 0f, 60f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3009)}),
            new Item(
                3009, "Boots of Swiftness", 0, 675, true, ItemTier.None, (ItemCategory.None), 0f, 0f, 60f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1001)}),
            new Item(
                1051, "Brawler's Gloves", 0, 400, false, ItemTier.None, (ItemCategory.CriticalStrike), 0f, 0f, 0f,
                0.08f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3010, "Catalyst the Protector", 0, 400, true, ItemTier.None,
                (ItemCategory.HealthRegen & ItemCategory.Health & ItemCategory.ManaRegen & ItemCategory.Mana), 0f, 0f,
                0f, 0f, 0f, 0f, 200f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(1028), (1027)}),
            new Item(
                1031, "Chain Vest", 0, 450, true, ItemTier.None, (ItemCategory.Armor), 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                40f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1029)}),
            new Item(
                3028, "Chalice of Harmony", 0, 140, true, ItemTier.None,
                (ItemCategory.ManaRegen & ItemCategory.SpellBlock), 0f, 0f, 0f, 0f, 0f, 0f, 0f, 25f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, new[] {(1004), (1033), (1004)}),
            new Item(
                1018, "Cloak of Agility", 0, 730, false, ItemTier.None, (ItemCategory.CriticalStrike), 0f, 0f, 0f,
                0.15f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                1029, "Cloth Armor", 0, 300, false, ItemTier.None, (ItemCategory.Armor), 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                15f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3801, "Crystalline Bracer", 0, 20, true, ItemTier.None,
                (ItemCategory.HealthRegen & ItemCategory.Health), 0f, 0f, 0f, 0f, 0f, 0f, 200f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, new[] {(1028), (1006)}),
            new Item(
                2041, "Crystalline Flask", 0, 345, false, ItemTier.None,
                (ItemCategory.HealthRegen & ItemCategory.Consumable & ItemCategory.ManaRegen), 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                1042, "Dagger", 0, 450, false, ItemTier.None, (ItemCategory.AttackSpeed), 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3128, "Deathfire Grasp", 0, 680, true, ItemTier.None, (ItemCategory.SpellDamage), 0f, 0f, 0f, 0f, 120f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1058), (3108)}),
            new Item(
                3137, "Dervish Blade", 0, 200, true, ItemTier.None,
                (ItemCategory.SpellBlock & ItemCategory.AttackSpeed), 0f, 0f, 0f, 0f, 0f, 0f, 0f, 45f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(3140), (3101)}),
            new Item(
                1075, "Doran's Blade (Showdown)", 0, 440, false, ItemTier.None,
                (ItemCategory.Health & ItemCategory.Damage & ItemCategory.LifeSteal), 0f, 7f, 0f, 0f, 0f, 0f, 70f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                1055, "Doran's Blade", 0, 440, false, ItemTier.None,
                (ItemCategory.Health & ItemCategory.Damage & ItemCategory.LifeSteal), 0f, 7f, 0f, 0f, 0f, 0f, 70f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                1076, "Doran's Ring (Showdown)", 0, 400, false, ItemTier.None,
                (ItemCategory.Health & ItemCategory.ManaRegen & ItemCategory.SpellDamage), 0f, 0f, 0f, 0f, 15f, 0f, 60f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                1056, "Doran's Ring", 0, 400, false, ItemTier.None,
                (ItemCategory.Health & ItemCategory.ManaRegen & ItemCategory.SpellDamage), 0f, 0f, 0f, 0f, 15f, 0f, 60f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                1074, "Doran's Shield (Showdown)", 0, 440, false, ItemTier.None,
                (ItemCategory.HealthRegen & ItemCategory.Health), 0f, 0f, 0f, 0f, 0f, 2f, 100f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                1054, "Doran's Shield", 0, 440, false, ItemTier.None, (ItemCategory.HealthRegen & ItemCategory.Health),
                0f, 0f, 0f, 0f, 0f, 1.2f, 80f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                2039, "Elixir of Brilliance", 3, 250, false, ItemTier.None,
                (ItemCategory.Consumable & ItemCategory.SpellDamage), 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                2037, "Elixir of Fortitude", 3, 350, false, ItemTier.None,
                (ItemCategory.Consumable & ItemCategory.Health & ItemCategory.Damage), 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                2138, "Elixir of Iron", 0, 400, false, ItemTier.None, (ItemCategory.Consumable), 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                2137, "Elixir of Ruin", 0, 400, false, ItemTier.None, (ItemCategory.Consumable & ItemCategory.Health),
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                2139, "Elixir of Sorcery", 0, 400, false, ItemTier.None,
                (ItemCategory.Consumable & ItemCategory.ManaRegen & ItemCategory.SpellDamage), 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                2140, "Elixir of Wrath", 0, 400, false, ItemTier.None,
                (ItemCategory.Consumable & ItemCategory.Damage & ItemCategory.LifeSteal), 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3184, "Entropy", 0, 500, true, ItemTier.None, (ItemCategory.Health & ItemCategory.Damage), 0f, 55f, 0f,
                0f, 0f, 0f, 275f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3044), (1037)}),
            new Item(
                3508, "Essence Reaver", 0, 850, true, ItemTier.None,
                (ItemCategory.Damage & ItemCategory.ManaRegen & ItemCategory.LifeSteal), 0f, 80f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1053), (1038)}),
            new Item(
                3123, "Executioner's Calling", 0, 740, true, ItemTier.None,
                (ItemCategory.CriticalStrike & ItemCategory.Damage), 0f, 25f, 0f, 0.2f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(3093), (1036)}),
            new Item(
                2050, "Explorer's Ward", 0, 0, false, ItemTier.None, (ItemCategory.Consumable), 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3401, "Face of the Mountain", 0, 485, true, ItemTier.None,
                (ItemCategory.HealthRegen & ItemCategory.Health), 0f, 0f, 0f, 0f, 0f, 0f, 500f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, new[] {(3097), (3067)}),
            new Item(
                1004, "Faerie Charm", 0, 180, false, ItemTier.None, (ItemCategory.ManaRegen), 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3363, "Farsight Orb (Trinket)", 0, 475, true, ItemTier.AdvancedTrinket, (ItemCategory.None), 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(3342)}),
            new Item(
                3160, "Feral Flare", 0, 1800, true, ItemTier.None, (ItemCategory.Damage & ItemCategory.AttackSpeed), 0f,
                12f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3108, "Fiendish Codex", 0, 385, true, ItemTier.None, (ItemCategory.SpellDamage), 0f, 0f, 0f, 0f, 30f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1052)}),
            new Item(
                3114, "Forbidden Idol", 0, 240, true, ItemTier.None, (ItemCategory.ManaRegen), 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1004), (1004)}),
            new Item(
                3092, "Forst Queen's Claim", 0, 515, true, ItemTier.None,
                (ItemCategory.ManaRegen & ItemCategory.SpellDamage), 0f, 0f, 0f, 0f, 50f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, new[] {(3098), (3108)}),
            new Item(
                3098, "Forstfang", 0, 500, true, ItemTier.None, (ItemCategory.ManaRegen & ItemCategory.SpellDamage), 0f,
                0f, 0f, 0f, 10f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(3303)}),
            new Item(
                3110, "Frozen Heart", 0, 450, true, ItemTier.None, (ItemCategory.Mana & ItemCategory.Armor), 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 100f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3082), (3024)}),
            new Item(
                3022, "Frozen Mallet", 0, 1025, true, ItemTier.None, (ItemCategory.Health & ItemCategory.Damage), 0f,
                30f, 0f, 0f, 0f, 0f, 700f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(1028), (1011), (1037)}),
            new Item(
                1011, "Giant's Belt", 0, 1000, false, ItemTier.None, (ItemCategory.Health), 0f, 0f, 0f, 0f, 0f, 0f,
                380f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3024, "Glacial Shroud", 0, 250, true, ItemTier.None, (ItemCategory.Mana & ItemCategory.Armor), 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 20f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(1027), (1029)}),
            new Item(
                3460, "Golden Transcendence", 0, 0, false, ItemTier.None, (ItemCategory.None), 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3361, "Greater Stealth Totem (Trinket)", 0, 475, true, ItemTier.AdvancedTrinket, (ItemCategory.None),
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(3340)}),
            new Item(
                3362, "Greater Vision Totem (Trinket)", 0, 475, true, ItemTier.AdvancedTrinket, (ItemCategory.None), 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(3340)}),
            new Item(
                3159, "Grez's Spectral Lantern", 0, 180, true, ItemTier.None,
                (ItemCategory.Damage & ItemCategory.AttackSpeed), 0f, 15f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, new[] {(3106), (1036), (1042)}),
            new Item(
                3026, "Guardian Angel", 0, 1500, true, ItemTier.None, (ItemCategory.SpellBlock & ItemCategory.Armor),
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 40f, 50f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(1033), (1031)}),
            new Item(
                2051, "Guardian's Horn", 0, 445, true, ItemTier.None,
                (ItemCategory.HealthRegen & ItemCategory.Health & ItemCategory.SpellBlock & ItemCategory.Armor), 0f, 0f,
                0f, 0f, 0f, 0f, 180f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(1006), (1028)}),
            new Item(
                3124, "Guinsoo's Rageblade", 0, 865, true, ItemTier.None,
                (ItemCategory.Damage & ItemCategory.AttackSpeed & ItemCategory.SpellDamage & ItemCategory.LifeSteal), 0f,
                30f, 0f, 0f, 40f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(1026), (1037)}),
            new Item(
                3136, "Haunting Guise", 0, 650, true, ItemTier.None, (ItemCategory.Health & ItemCategory.SpellDamage),
                0f, 0f, 0f, 0f, 25f, 0f, 200f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(1028), (1052)}),
            new Item(
                3175, "Head of Kha'Zix", 0, 0, false, ItemTier.None, (ItemCategory.None), 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3410, "Head of Kha'Zix", 0, 0, false, ItemTier.None, (ItemCategory.None), 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3416, "Head of Kha'Zix", 0, 0, false, ItemTier.None, (ItemCategory.None), 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3422, "Head of Kha'Zix", 0, 0, false, ItemTier.None, (ItemCategory.None), 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3455, "Head of Kha'Zix", 0, 0, false, ItemTier.None, (ItemCategory.None), 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                2003, "Health Potion", 5, 35, false, ItemTier.None, (ItemCategory.Consumable), 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3155, "Hexdrinker", 0, 590, true, ItemTier.None, (ItemCategory.Damage & ItemCategory.SpellBlock), 0f,
                25f, 0f, 0f, 0f, 0f, 0f, 30f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(1036), (1033)}),
            new Item(
                3146, "Hextech Gunblade", 0, 800, true, ItemTier.None,
                (ItemCategory.Damage & ItemCategory.SpellDamage & ItemCategory.LifeSteal), 0f, 45f, 0f, 0f, 65f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(3144), (3145)}),
            new Item(
                3145, "Hextech Revolver", 0, 330, true, ItemTier.None, (ItemCategory.SpellDamage), 0f, 0f, 0f, 0f, 40f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1052), (1052)}),
            new Item(
                3187, "Hextech Sweeper", 0, 330, true, ItemTier.None,
                (ItemCategory.Health & ItemCategory.Mana & ItemCategory.Armor), 0f, 0f, 0f, 0f, 0f, 0f, 225f, 0f, 25f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(3024), (3067)}),
            new Item(
                1039, "Hunter's Machete", 0, 400, false, ItemTier.None,
                (ItemCategory.HealthRegen & ItemCategory.Damage & ItemCategory.ManaRegen), 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3025, "Iceborn Gauntlet", 0, 750, true, ItemTier.None,
                (ItemCategory.SpellDamage & ItemCategory.Mana & ItemCategory.Armor), 0f, 0f, 0f, 0f, 30f, 0f, 0f, 0f,
                60f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(3057), (3024)}),
            new Item(
                2048, "Ichor of Illumination", 3, 500, false, ItemTier.None,
                (ItemCategory.Consumable & ItemCategory.ManaRegen & ItemCategory.SpellDamage), 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                2040, "Ichor of Rage", 3, 500, false, ItemTier.None,
                (ItemCategory.Consumable & ItemCategory.Damage & ItemCategory.AttackSpeed), 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3031, "Infinity Edge", 0, 645, true, ItemTier.None, (ItemCategory.CriticalStrike & ItemCategory.Damage),
                0f, 80f, 0f, 0.25f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(1038), (1037), (1018)}),
            new Item(
                3279, "Ionian Boots of Lucidity - Enchantment: Alacrity", 0, 475, true, ItemTier.Enchantment,
                (ItemCategory.None), 0f, 0f, 45f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3158)}),
            new Item(
                3276, "Ionian Boots of Lucidity - Enchantment: Captain", 0, 600, true, ItemTier.Enchantment,
                (ItemCategory.None), 0f, 0f, 45f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3158)}),
            new Item(
                3278, "Ionian Boots of Lucidity - Enchantment: Distortion", 0, 475, true, ItemTier.Enchantment,
                (ItemCategory.None), 0f, 0f, 45f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3158)}),
            new Item(
                3277, "Ionian Boots of Lucidity - Enchantment: Furor", 0, 475, true, ItemTier.Enchantment,
                (ItemCategory.None), 0f, 0f, 45f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3158)}),
            new Item(
                3275, "Ionian Boots of Lucidity - Enchantment: Homeguard", 0, 475, true, ItemTier.Enchantment,
                (ItemCategory.None), 0f, 0f, 45f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3158)}),
            new Item(
                3158, "Ionian Boots of Lucidity", 0, 675, true, ItemTier.None, (ItemCategory.None), 0f, 0f, 45f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1001)}),
            new Item(
                3067, "Kindlegem", 0, 450, true, ItemTier.None, (ItemCategory.Health), 0f, 0f, 0f, 0f, 0f, 0f, 200f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1028)}),
            new Item(
                3035, "Last Whisper", 0, 1065, true, ItemTier.None, (ItemCategory.Damage), 0f, 40f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1037), (1036)}),
            new Item(
                3151, "Liandry's Torment", 0, 980, true, ItemTier.None,
                (ItemCategory.Health & ItemCategory.SpellDamage), 0f, 0f, 0f, 0f, 50f, 0f, 300f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, new[] {(3136), (1052)}),
            new Item(
                3100, "Lich Bane", 0, 850, true, ItemTier.None, (ItemCategory.SpellDamage & ItemCategory.Mana), 0f, 0f,
                0f, 0f, 80f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0.05f, 0f,
                new[] {(3057), (3113)}),
            new Item(
                3190, "Locket of the Iron Solari", 0, 50, true, ItemTier.None,
                (ItemCategory.HealthRegen & ItemCategory.Health & ItemCategory.SpellBlock), 0f, 0f, 0f, 0f, 0f, 0f, 400f,
                20f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(3105), (3067)}),
            new Item(
                1036, "Long Sword", 0, 360, false, ItemTier.None, (ItemCategory.Damage), 0f, 10f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3104, "Lord Van Damm's Pillager", 0, 995, true, ItemTier.None,
                (ItemCategory.CriticalStrike & ItemCategory.Damage), 0f, 80f, 0f, 0.25f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(3122), (1037), (1018)}),
            new Item(
                3106, "Madred's Razors", 0, 0, true, ItemTier.None, (ItemCategory.AttackSpeed), 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1042)}),
            new Item(
                2004, "Mana Potion", 5, 35, false, ItemTier.None, (ItemCategory.Consumable), 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3008, "Manamune (Crystal Scar)", 0, 605, true, ItemTier.None,
                (ItemCategory.Damage & ItemCategory.ManaRegen & ItemCategory.Mana), 0f, 25f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(3073), (1037)}),
            new Item(
                3004, "Manamune", 0, 605, true, ItemTier.None,
                (ItemCategory.Damage & ItemCategory.ManaRegen & ItemCategory.Mana), 0f, 25f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(3070), (1037)}),
            new Item(
                3156, "Maw of Malmortius", 0, 875, true, ItemTier.None, (ItemCategory.Damage & ItemCategory.SpellBlock),
                0f, 60f, 0f, 0f, 0f, 0f, 0f, 40f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3155), (1037)}),
            new Item(
                3041, "Mejai's Soulstealer", 0, 965, true, ItemTier.None, (ItemCategory.SpellDamage), 0f, 0f, 0f, 0f,
                20f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1052)}),
            new Item(
                3139, "Mercurial Scimitar", 0, 900, true, ItemTier.None,
                (ItemCategory.Damage & ItemCategory.SpellBlock), 0f, 80f, 0f, 0f, 0f, 0f, 0f, 35f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, new[] {(1038), (3140)}),
            new Item(
                3269, "Mercury's Treads - Enchantment: Alacrity", 0, 475, true, ItemTier.Enchantment,
                (ItemCategory.None), 0f, 0f, 45f, 0f, 0f, 0f, 0f, 25f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3111)}),
            new Item(
                3266, "Mercury's Treads - Enchantment: Captain", 0, 600, true, ItemTier.Enchantment,
                (ItemCategory.None), 0f, 0f, 45f, 0f, 0f, 0f, 0f, 25f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3111)}),
            new Item(
                3268, "Mercury's Treads - Enchantment: Distortion", 0, 475, true, ItemTier.Enchantment,
                (ItemCategory.None), 0f, 0f, 45f, 0f, 0f, 0f, 0f, 25f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3111)}),
            new Item(
                3267, "Mercury's Treads - Enchantment: Furor", 0, 475, true, ItemTier.Enchantment, (ItemCategory.None),
                0f, 0f, 45f, 0f, 0f, 0f, 0f, 25f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3111)}),
            new Item(
                3265, "Mercury's Treads - Enchantment: Homeguard", 0, 475, true, ItemTier.Enchantment,
                (ItemCategory.None), 0f, 0f, 45f, 0f, 0f, 0f, 0f, 25f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3111)}),
            new Item(
                3111, "Mercury's Treads", 0, 375, true, ItemTier.None, (ItemCategory.SpellBlock), 0f, 0f, 45f, 0f, 0f,
                0f, 0f, 25f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1001), (1033)}),
            new Item(
                3222, "Mikael's Crucible", 0, 850, true, ItemTier.None,
                (ItemCategory.ManaRegen & ItemCategory.SpellBlock), 0f, 0f, 0f, 0f, 0f, 0f, 0f, 40f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, new[] {(3028), (3114)}),
            new Item(
                3170, "Moonflair Spellblade", 0, 920, true, ItemTier.None,
                (ItemCategory.SpellBlock & ItemCategory.SpellDamage & ItemCategory.Armor), 0f, 0f, 0f, 0f, 50f, 0f, 0f,
                50f, 50f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(3191), (1033)}),
            new Item(
                3165, "Morellonomicon", 0, 680, true, ItemTier.None,
                (ItemCategory.ManaRegen & ItemCategory.SpellDamage), 0f, 0f, 0f, 0f, 80f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, new[] {(3108), (3114)}),
            new Item(
                3042, "Muramana", 0, 2200, false, ItemTier.None, (ItemCategory.None), 0f, 25f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3043, "Muramana", 0, 2200, false, ItemTier.None, (ItemCategory.None), 0f, 25f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3115, "Nashor's Tooth", 0, 850, true, ItemTier.None,
                (ItemCategory.AttackSpeed & ItemCategory.SpellDamage), 0f, 0f, 0f, 0f, 60f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(3101), (3108)}),
            new Item(
                1058, "Needlessly Large Rod", 0, 1250, false, ItemTier.None, (ItemCategory.SpellDamage), 0f, 0f, 0f, 0f,
                80f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3264, "Ninja Tabi - Enchantment: Alacrity", 0, 475, true, ItemTier.Enchantment, (ItemCategory.None), 0f,
                0f, 45f, 0f, 0f, 0f, 0f, 0f, 25f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(3047)}),
            new Item(
                3261, "Ninja Tabi - Enchantment: Captain", 0, 600, true, ItemTier.Enchantment, (ItemCategory.None), 0f,
                0f, 45f, 0f, 0f, 0f, 0f, 0f, 25f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(3047)}),
            new Item(
                3263, "Ninja Tabi - Enchantment: Distortion", 0, 475, true, ItemTier.Enchantment, (ItemCategory.None),
                0f, 0f, 45f, 0f, 0f, 0f, 0f, 0f, 25f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3047)}),
            new Item(
                3262, "Ninja Tabi - Enchantment: Furor", 0, 475, true, ItemTier.Enchantment, (ItemCategory.None), 0f,
                0f, 45f, 0f, 0f, 0f, 0f, 0f, 25f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(3047)}),
            new Item(
                3260, "Ninja Tabi - Enchantment: Homeguard", 0, 475, true, ItemTier.Enchantment, (ItemCategory.None),
                0f, 0f, 45f, 0f, 0f, 0f, 0f, 0f, 25f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3047)}),
            new Item(
                3047, "Ninja Tabi", 0, 375, true, ItemTier.None, (ItemCategory.Armor), 0f, 0f, 45f, 0f, 0f, 0f, 0f, 0f,
                25f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1001), (1029)}),
            new Item(
                3096, "Nomad's Medallion", 0, 500, true, ItemTier.None,
                (ItemCategory.HealthRegen & ItemCategory.ManaRegen), 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, new[] {(3301)}),
            new Item(
                1033, "Null-Magic Mantle", 0, 500, false, ItemTier.None, (ItemCategory.SpellBlock), 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 25f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3180, "Odyn's Veil", 0, 800, true, ItemTier.None,
                (ItemCategory.Health & ItemCategory.SpellBlock & ItemCategory.Mana), 0f, 0f, 0f, 0f, 0f, 0f, 350f, 50f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1033), (3010)}),
            new Item(
                3056, "Ohmwrecker", 0, 750, true, ItemTier.None,
                (ItemCategory.HealthRegen & ItemCategory.Health & ItemCategory.Armor), 0f, 0f, 0f, 0f, 0f, 0f, 300f, 0f,
                50f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(2053), (3067)}),
            new Item(
                2047, "Orcale's Extract", 0, 250, false, ItemTier.None, (ItemCategory.Consumable), 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3364, "Oracle's Lens (Trinket)", 0, 475, true, ItemTier.AdvancedTrinket, (ItemCategory.None), 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(3341)}),
            new Item(
                3112, "Orb of Winter", 0, 850, true, ItemTier.None,
                (ItemCategory.HealthRegen & ItemCategory.SpellBlock), 0f, 0f, 0f, 0f, 0f, 0f, 0f, 70f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1006), (1006), (1033), (1033)}),
            new Item(
                3084, "Overlord's Bloodmail", 0, 1055, true, ItemTier.None,
                (ItemCategory.HealthRegen & ItemCategory.Health), 0f, 0f, 0f, 0f, 0f, 0f, 850f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, new[] {(1011), (1028)}),
            new Item(
                3198, "Perfect Hex Core", 0, 1000, true, ItemTier.None, (ItemCategory.SpellDamage & ItemCategory.Mana),
                0f, 0f, 0f, 0f, 60f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3197)}),
            new Item(
                3044, "Phage", 0, 565, true, ItemTier.None, (ItemCategory.Health & ItemCategory.Damage), 0f, 20f, 0f,
                0f, 0f, 0f, 200f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(1028), (1036)}),
            new Item(
                3046, "Phantom Dancer", 0, 520, true, ItemTier.None,
                (ItemCategory.CriticalStrike & ItemCategory.AttackSpeed), 0f, 0f, 0f, 0.3f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0.05f, 0f, new[] {(1018), (3086), (1042)}),
            new Item(
                1037, "Pickaxe", 0, 875, false, ItemTier.None, (ItemCategory.Damage), 0f, 25f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3722, "Poacher's Knife - Enchantment: Devourer", 0, 600, true, ItemTier.Enchantment,
                (ItemCategory.HealthRegen & ItemCategory.Damage & ItemCategory.ManaRegen & ItemCategory.AttackSpeed), 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3711), (1042), (1042)}),
            new Item(
                3721, "Poacher's Knife - Enchantment: Juggernaut", 0, 250, true, ItemTier.Enchantment,
                (ItemCategory.HealthRegen & ItemCategory.Health & ItemCategory.Damage & ItemCategory.ManaRegen), 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3711), (3067), (1028)}),
            new Item(
                3720, "Poacher's Knife - Enchantment: Magus", 0, 680, true, ItemTier.Enchantment,
                (ItemCategory.HealthRegen & ItemCategory.Damage & ItemCategory.ManaRegen & ItemCategory.SpellDamage), 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3711), (3108)}),
            new Item(
                3719, "Poacher's Knife - Enchantment: Warrior", 0, 163, true, ItemTier.Enchantment,
                (ItemCategory.HealthRegen & ItemCategory.Damage & ItemCategory.ManaRegen), 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(3711), (3134)}),
            new Item(
                3711, "Poacher's Knife", 0, 350, true, ItemTier.None,
                (ItemCategory.HealthRegen & ItemCategory.Damage & ItemCategory.ManaRegen), 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1039)}),
            new Item(
                2052, "Poro-Snax", 0, 0, false, ItemTier.None, (ItemCategory.None), 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                1062, "Prospector's Blade", 0, 950, false, ItemTier.None,
                (ItemCategory.Health & ItemCategory.Damage & ItemCategory.AttackSpeed), 0f, 16f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                1063, "Prospector's Ring", 0, 950, false, ItemTier.None,
                (ItemCategory.Health & ItemCategory.ManaRegen & ItemCategory.SpellDamage), 0f, 0f, 0f, 0f, 35f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3200, "Prototype Hex Core", 0, 0, true, ItemTier.None, (ItemCategory.None), 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3140, "Quicksilver Sash", 0, 750, true, ItemTier.None, (ItemCategory.SpellBlock), 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 30f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1033)}),
            new Item(
                3204, "Quill Coat", 0, 75, true, ItemTier.None,
                (ItemCategory.HealthRegen & ItemCategory.ManaRegen & ItemCategory.Armor), 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                20f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1039), (1029)}),
            new Item(
                3205, "Quill Coat", 0, 75, true, ItemTier.None,
                (ItemCategory.HealthRegen & ItemCategory.ManaRegen & ItemCategory.Armor), 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                20f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1039), (1029)}),
            new Item(
                3089, "Rabadon's Deathcap", 0, 840, true, ItemTier.None, (ItemCategory.SpellDamage), 0f, 0f, 0f, 0f,
                120f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(1026), (1058)}),
            new Item(
                3143, "Randuin's Omen", 0, 800, true, ItemTier.None, (ItemCategory.Health & ItemCategory.Armor), 0f, 0f,
                0f, 0f, 0f, 0f, 500f, 0f, 70f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3082), (1011)}),
            new Item(
                3726, "Ranger's Trailblazer - Enchantment: Devourer", 0, 600, true, ItemTier.Enchantment,
                (ItemCategory.HealthRegen & ItemCategory.Damage & ItemCategory.ManaRegen & ItemCategory.AttackSpeed), 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3713), (1042), (1042)}),
            new Item(
                3725, "Ranger's Trailblazer - Enchantment: Juggernaut", 0, 250, true, ItemTier.Enchantment,
                (ItemCategory.HealthRegen & ItemCategory.Health & ItemCategory.Damage & ItemCategory.ManaRegen), 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3713), (3067), (1028)}),
            new Item(
                3724, "Ranger's Trailblazer - Enchantment: Magus", 0, 680, true, ItemTier.Enchantment,
                (ItemCategory.HealthRegen & ItemCategory.Damage & ItemCategory.ManaRegen & ItemCategory.SpellDamage), 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3713), (3108)}),
            new Item(
                3723, "Ranger's Trailblazer - Enchantment: Warrior", 0, 163, true, ItemTier.Enchantment,
                (ItemCategory.HealthRegen & ItemCategory.Damage & ItemCategory.ManaRegen), 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(3713), (3134)}),
            new Item(
                3713, "Ranger's Trailblazer", 0, 350, true, ItemTier.None,
                (ItemCategory.HealthRegen & ItemCategory.Damage & ItemCategory.ManaRegen), 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1039)}),
            new Item(
                2053, "Raptor Cloak", 0, 520, true, ItemTier.None, (ItemCategory.HealthRegen & ItemCategory.Armor), 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 30f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(1006), (1029)}),
            new Item(
                3074, "Ravenous Hydra (Melee Only>", 0, 600, true, ItemTier.None,
                (ItemCategory.HealthRegen & ItemCategory.Damage & ItemCategory.LifeSteal), 0f, 75f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(3077), (1053)}),
            new Item(
                1043, "Recurve Bow", 0, 900, false, ItemTier.None, (ItemCategory.AttackSpeed), 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                1006, "Rejuvenation Bead", 0, 180, false, ItemTier.None, (ItemCategory.HealthRegen), 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3302, "Relic Shield", 0, 365, true, ItemTier.None, (ItemCategory.Health), 0f, 0f, 0f, 0f, 0f, 0f, 75f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3800, "Righteous Glory", 0, 700, true, ItemTier.None,
                (ItemCategory.HealthRegen & ItemCategory.Health & ItemCategory.Mana), 0f, 0f, 0f, 0f, 0f, 0f, 500f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(3010), (3801)}),
            new Item(
                3029, "Rod of Ages (Crystal Scar)", 0, 740, true, ItemTier.None,
                (ItemCategory.HealthRegen & ItemCategory.Health & ItemCategory.ManaRegen & ItemCategory.SpellDamage &
                 ItemCategory.Mana), 0f, 0f, 0f, 0f, 60f, 0f, 450f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3010), (1026)}),
            new Item(
                3027, "Rod of Ages", 0, 740, true, ItemTier.None,
                (ItemCategory.HealthRegen & ItemCategory.Health & ItemCategory.ManaRegen & ItemCategory.SpellDamage &
                 ItemCategory.Mana), 0f, 0f, 0f, 0f, 60f, 0f, 450f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3010), (1026)}),
            new Item(
                1028, "Ruby Crystal", 0, 400, false, ItemTier.None, (ItemCategory.Health), 0f, 0f, 0f, 0f, 0f, 0f, 150f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                2045, "Ruby Sightstone", 0, 400, true, ItemTier.None, (ItemCategory.Health), 0f, 0f, 0f, 0f, 0f, 0f,
                400f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(2049), (1028)}),
            new Item(
                3085, "Runaan's Hurricane (Ranged Only)", 0, 600, true, ItemTier.None, (ItemCategory.AttackSpeed), 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(1042), (1043), (1042)}),
            new Item(
                3116, "Rylai's Crystal Scepter", 0, 605, true, ItemTier.None,
                (ItemCategory.Health & ItemCategory.SpellDamage), 0f, 0f, 0f, 0f, 100f, 0f, 400f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, new[] {(1026), (1052), (1011)}),
            new Item(
                3181, "Sanguine Blade", 0, 600, true, ItemTier.None, (ItemCategory.Damage & ItemCategory.LifeSteal), 0f,
                45f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(1037), (1053)}),
            new Item(
                1027, "Sapphire Crystal", 0, 400, false, ItemTier.None, (ItemCategory.Mana), 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3342, "Scrying Orb (Trinket)", 0, 0, false, ItemTier.BasicTrinket, (ItemCategory.Consumable), 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3191, "Seeker's Armguard", 0, 465, true, ItemTier.None, (ItemCategory.SpellDamage & ItemCategory.Armor),
                0f, 0f, 0f, 0f, 25f, 0f, 0f, 0f, 30f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(1029), (1052)}),
            new Item(
                3040, "Seraph's Embrace", 0, 2700, false, ItemTier.None, (ItemCategory.None), 0f, 0f, 0f, 0f, 60f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3048, "Seraph's Embrace", 0, 2700, false, ItemTier.None, (ItemCategory.None), 0f, 0f, 0f, 0f, 60f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3057, "Sheen", 0, 365, true, ItemTier.None, (ItemCategory.SpellDamage & ItemCategory.Mana), 0f, 0f, 0f,
                0f, 25f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(1027), (1052)}),
            new Item(
                2049, "Sightstone", 0, 400, true, ItemTier.None, (ItemCategory.Health), 0f, 0f, 0f, 0f, 0f, 0f, 150f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1028)}),
            new Item(
                3718, "Skirmisher's Sabre - Enchantment: Devourer", 0, 600, true, ItemTier.Enchantment,
                (ItemCategory.HealthRegen & ItemCategory.Damage & ItemCategory.ManaRegen & ItemCategory.AttackSpeed), 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3715), (1042), (1042)}),
            new Item(
                3717, "Skirmisher's Sabre - Enchantment: Juggernaut", 0, 250, true, ItemTier.Enchantment,
                (ItemCategory.HealthRegen & ItemCategory.Health & ItemCategory.Damage & ItemCategory.ManaRegen), 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3715), (3067), (1028)}),
            new Item(
                3176, "Skirmisher's Sabre - Enchantment: Magus", 0, 500, true, ItemTier.Enchantment,
                (ItemCategory.Health & ItemCategory.Mana), 0f, 0f, 0f, 0f, 0f, 0f, 400f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, new[] {(3010), (1028)}),
            new Item(
                3174, "Skirmisher's Sabre - Enchantment: Warrior", 0, 880, true, ItemTier.Enchantment,
                (ItemCategory.ManaRegen & ItemCategory.SpellBlock & ItemCategory.SpellDamage), 0f, 0f, 0f, 0f, 60f, 0f,
                0f, 25f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(3108), (3028)}),
            new Item(
                3715, "Skirmisher's Sabre", 0, 350, true, ItemTier.None,
                (ItemCategory.HealthRegen & ItemCategory.Damage & ItemCategory.ManaRegen), 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1039)}),
            new Item(
                3259, "Sorcerer's Shoes - Enchantment: Alacrity", 0, 475, true, ItemTier.Enchantment,
                (ItemCategory.None), 0f, 0f, 45f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3020)}),
            new Item(
                3256, "Sorcerer's Shoes - Enchantment: Captain", 0, 600, true, ItemTier.Enchantment,
                (ItemCategory.None), 0f, 0f, 45f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3020)}),
            new Item(
                3258, "Sorcerer's Shoes - Enchantment: Distortion", 0, 475, true, ItemTier.Enchantment,
                (ItemCategory.None), 0f, 0f, 45f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3020)}),
            new Item(
                3257, "Sorcerer's Shoes - Enchantment: Furor", 0, 475, true, ItemTier.Enchantment, (ItemCategory.None),
                0f, 0f, 45f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3020)}),
            new Item(
                3255, "Sorcerer's Shoes - Enchantment: Homeguard", 0, 475, true, ItemTier.Enchantment,
                (ItemCategory.None), 0f, 0f, 45f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3020)}),
            new Item(
                3020, "Sorcerer's Shoes", 0, 775, true, ItemTier.None, (ItemCategory.None), 0f, 0f, 45f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1001)}),
            new Item(
                3345, "Soul Anchor (Trinket)", 0, 0, false, ItemTier.BasicTrinket, (ItemCategory.None), 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3211, "Spectre's Cowl", 0, 300, true, ItemTier.None,
                (ItemCategory.HealthRegen & ItemCategory.Health & ItemCategory.SpellBlock), 0f, 0f, 0f, 0f, 0f, 0f, 200f,
                35f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1028), (1033)}),
            new Item(
                3303, "Spellthief's Edge", 0, 365, false, ItemTier.None,
                (ItemCategory.ManaRegen & ItemCategory.SpellDamage), 0f, 0f, 0f, 0f, 5f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                1080, "Spirit Stone", 0, 15, true, ItemTier.None, (ItemCategory.None), 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1039), (1004), (1006)}),
            new Item(
                3065, "Spirit Visage", 0, 700, true, ItemTier.None,
                (ItemCategory.HealthRegen & ItemCategory.Health & ItemCategory.SpellBlock), 0f, 0f, 0f, 0f, 0f, 0f, 400f,
                55f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(3211), (3067)}),
            new Item(
                3207, "Spirit of the Ancient Golem", 0, 450, true, ItemTier.None,
                (ItemCategory.HealthRegen & ItemCategory.Health & ItemCategory.ManaRegen & ItemCategory.Armor), 0f, 0f,
                0f, 0f, 0f, 0f, 200f, 0f, 20f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3205), (3067)}),
            new Item(
                3208, "Spirit of the Ancient Golem", 0, 450, true, ItemTier.None,
                (ItemCategory.HealthRegen & ItemCategory.Health & ItemCategory.ManaRegen & ItemCategory.Armor), 0f, 0f,
                0f, 0f, 0f, 0f, 200f, 0f, 20f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3204), (3067)}),
            new Item(
                3209, "Spirit of the Elder Lizard", 0, 580, true, ItemTier.None, (ItemCategory.Damage), 0f, 30f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(1080), (1036), (1036)}),
            new Item(
                3206, "Spirit of the Spectral Wraith", 0, 480, true, ItemTier.None, (ItemCategory.SpellDamage), 0f, 0f,
                0f, 0f, 50f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(1080), (3108)}),
            new Item(
                3710, "Stalker's Blade - Enchantment: Devourer", 0, 600, true, ItemTier.Enchantment,
                (ItemCategory.HealthRegen & ItemCategory.Damage & ItemCategory.ManaRegen & ItemCategory.AttackSpeed), 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3706), (1042), (1042)}),
            new Item(
                3709, "Stalker's Blade - Enchantment: Juggernaut", 0, 250, true, ItemTier.Enchantment,
                (ItemCategory.HealthRegen & ItemCategory.Health & ItemCategory.Damage & ItemCategory.ManaRegen), 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3706), (3067), (1028)}),
            new Item(
                3708, "Stalker's Blade - Enchantment: Magus", 0, 680, true, ItemTier.Enchantment,
                (ItemCategory.HealthRegen & ItemCategory.Damage & ItemCategory.ManaRegen & ItemCategory.SpellDamage), 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3706), (3108)}),
            new Item(
                3707, "Stalker's Blade - Enchantment: Warrior", 0, 163, true, ItemTier.Enchantment,
                (ItemCategory.HealthRegen & ItemCategory.Damage & ItemCategory.ManaRegen), 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(3706), (3134)}),
            new Item(
                3706, "Stalker's Blade", 0, 350, true, ItemTier.None,
                (ItemCategory.HealthRegen & ItemCategory.Damage & ItemCategory.ManaRegen), 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1039)}),
            new Item(
                3087, "Statikk Shiv", 0, 600, true, ItemTier.None,
                (ItemCategory.CriticalStrike & ItemCategory.AttackSpeed), 0f, 0f, 0f, 0.2f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0.06f, 0f, new[] {(3086), (3093)}),
            new Item(
                2044, "Stealth Ward", 3, 75, false, ItemTier.None, (ItemCategory.Consumable), 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3101, "Stinger", 0, 350, true, ItemTier.None, (ItemCategory.AttackSpeed), 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1042), (1042)}),
            new Item(
                3068, "Sunfire Cape", 0, 850, true, ItemTier.None, (ItemCategory.Health & ItemCategory.Armor), 0f, 0f,
                0f, 0f, 0f, 0f, 450f, 0f, 45f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(1031), (1011)}),
            new Item(
                3341, "Sweeping Lens (Trinket)", 0, 0, false, ItemTier.BasicTrinket, (ItemCategory.Consumable), 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3131, "Sword of the Divine", 0, 800, true, ItemTier.None,
                (ItemCategory.CriticalStrike & ItemCategory.AttackSpeed), 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1043), (1042)}),
            new Item(
                3141, "Sword of the Occult", 0, 1040, true, ItemTier.None, (ItemCategory.Damage), 0f, 10f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1036)}),
            new Item(
                3069, "Talisman of Ascension", 0, 635, true, ItemTier.None,
                (ItemCategory.HealthRegen & ItemCategory.ManaRegen), 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, new[] {(3096), (3114)}),
            new Item(
                3097, "Tragon's Brace", 0, 500, true, ItemTier.None, (ItemCategory.HealthRegen & ItemCategory.Health),
                0f, 0f, 0f, 0f, 0f, 0f, 175f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3302)}),
            new Item(
                3073, "Tear of the Goddess (Crystal Scar)", 0, 140, true, ItemTier.None,
                (ItemCategory.ManaRegen & ItemCategory.Mana), 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, new[] {(1027), (1004)}),
            new Item(
                3070, "Tear of the Goddess", 0, 140, true, ItemTier.None, (ItemCategory.ManaRegen & ItemCategory.Mana),
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(1027), (1004)}),
            new Item(
                3071, "The Black Cleaver", 0, 1263, true, ItemTier.None, (ItemCategory.Health & ItemCategory.Damage),
                0f, 50f, 0f, 0f, 0f, 0f, 200f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3134), (1028)}),
            new Item(
                3599, "The Black Spear", 0, 0, false, ItemTier.None, (ItemCategory.None), 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3072, "The Bloodthirster", 0, 1150, true, ItemTier.None, (ItemCategory.Damage & ItemCategory.LifeSteal),
                0f, 80f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(1053), (1038)}),
            new Item(
                3134, "The Brutalizer", 0, 617, true, ItemTier.None, (ItemCategory.Damage), 0f, 25f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1036), (1036)}),
            new Item(
                3196, "The Hex Core mk-1", 0, 1000, true, ItemTier.None, (ItemCategory.SpellDamage & ItemCategory.Mana),
                0f, 0f, 0f, 0f, 20f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3200)}),
            new Item(
                3197, "The Hex Core mk-2", 0, 1000, true, ItemTier.None, (ItemCategory.SpellDamage & ItemCategory.Mana),
                0f, 0f, 0f, 0f, 40f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3196)}),
            new Item(
                3185, "The Lightbringer", 0, 350, true, ItemTier.None,
                (ItemCategory.CriticalStrike & ItemCategory.Damage), 0f, 30f, 0f, 0.3f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(3122), (1018)}),
            new Item(
                3075, "Thornmail", 0, 1050, true, ItemTier.None, (ItemCategory.Armor), 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                100f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1029), (1031)}),
            new Item(
                3077, "Tiamat (Melee Only)", 0, 305, true, ItemTier.None,
                (ItemCategory.HealthRegen & ItemCategory.Damage), 0f, 40f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, new[] {(1037), (1036), (1006), (1006)}),
            new Item(
                2009, "Total Biscuit of Rejuvenation", 0, 0, false, ItemTier.None, (ItemCategory.None), 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                2010, "Total Biscuit of Rejuvenation", 5, 35, false, ItemTier.None, (ItemCategory.None), 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3078, "Trinity Force", 0, 78, true, ItemTier.None,
                (ItemCategory.CriticalStrike & ItemCategory.Health & ItemCategory.Damage & ItemCategory.AttackSpeed &
                 ItemCategory.SpellDamage & ItemCategory.Mana), 0f, 30f, 0f, 0.1f, 30f, 0f, 250f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0.08f, 0f, new[] {(3086), (3057), (3044)}),
            new Item(
                3023, "Twin Shadows", 0, 630, true, ItemTier.None, (ItemCategory.SpellDamage), 0f, 0f, 0f, 0f, 80f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0.06f, 0f, new[] {(3108), (3113)}),
            new Item(
                3290, "Twin Shadows", 0, 630, true, ItemTier.None, (ItemCategory.SpellDamage), 0f, 0f, 0f, 0f, 80f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0.06f, 0f, new[] {(3108), (3113)}),
            new Item(
                1053, "Vampiric Scepter", 0, 440, true, ItemTier.None, (ItemCategory.Damage & ItemCategory.LifeSteal),
                0f, 10f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(1036)}),
            new Item(
                2043, "Vision Ward", 2, 100, false, ItemTier.None, (ItemCategory.Consumable), 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3135, "Void Staff", 0, 1000, true, ItemTier.None, (ItemCategory.SpellDamage), 0f, 0f, 0f, 0f, 70f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1026), (1052)}),
            new Item(
                3082, "Warden's Mail", 0, 450, true, ItemTier.None, (ItemCategory.Armor), 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 45f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1029), (1029)}),
            new Item(
                3340, "Warding Totem (Trinket)", 0, 0, false, ItemTier.BasicTrinket, (ItemCategory.None), 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Item(
                3083, "Warmog's Armor", 0, 300, true, ItemTier.None, (ItemCategory.HealthRegen & ItemCategory.Health),
                0f, 0f, 0f, 0f, 0f, 0f, 800f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(3801), (1011), (3801)}),
            new Item(
                3122, "Wicked Hatchet", 0, 440, true, ItemTier.None,
                (ItemCategory.CriticalStrike & ItemCategory.Damage), 0f, 20f, 0f, 0.1f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(1051), (1036)}),
            new Item(
                3152, "Will of the Ancients", 0, 480, true, ItemTier.None, (ItemCategory.SpellDamage), 0f, 0f, 0f, 0f,
                80f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(3145), (3108)}),
            new Item(
                3091, "Wit's End", 0, 750, true, ItemTier.None, (ItemCategory.SpellBlock & ItemCategory.AttackSpeed),
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 30f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                new[] {(1043), (1033), (1042)}),
            new Item(
                3090, "Wooglet's Witchcap", 0, 1045, true, ItemTier.None,
                (ItemCategory.SpellDamage & ItemCategory.Armor), 0f, 0f, 0f, 0f, 100f, 0f, 0f, 0f, 45f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, new[] {(3191), (1026), (1052)}),
            new Item(
                3154, "Wriggle's Lantern", 0, 215, true, ItemTier.None,
                (ItemCategory.Damage & ItemCategory.AttackSpeed), 0f, 12f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, new[] {(3106), (1036), (1042)}),
            new Item(
                3142, "Youmuu's Ghostblade", 0, 563, true, ItemTier.None,
                (ItemCategory.CriticalStrike & ItemCategory.Damage & ItemCategory.AttackSpeed), 0f, 30f, 0f, 0.15f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(3093), (3134)}),
            new Item(
                3086, "Zeal", 0, 250, true, ItemTier.None, (ItemCategory.CriticalStrike & ItemCategory.AttackSpeed), 0f,
                0f, 0f, 0.1f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0.05f, 0f,
                new[] {(1051), (1042)}),
            new Item(
                3050, "Zeke's Herald", 0, 800, true, ItemTier.None,
                (ItemCategory.Health & ItemCategory.Damage & ItemCategory.LifeSteal), 0f, 0f, 0f, 0f, 0f, 0f, 250f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, new[] {(3067), (1053)}),
            new Item(
                3172, "Zephyr", 0, 725, true, ItemTier.None, (ItemCategory.Damage & ItemCategory.AttackSpeed), 0f, 25f,
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0.1f, 0f,
                new[] {(3101), (1037)}),
            new Item(
                3157, "Zhonya's Hourglass", 0, 500, true, ItemTier.None,
                (ItemCategory.SpellDamage & ItemCategory.Armor), 0f, 0f, 0f, 0f, 120f, 0f, 0f, 0f, 50f, 0f, 0f, 0f, 0f,
                0f, 0f, 0f, 0f, 0f, 0f, new[] {(3191), (1058)})
        };

        #endregion

        #region StaticItems

        // Example? ==> Items.ABYSSAL_SCEPTER?.func?(par?); (C#6)
        //public static Item ABYSSAL_SCEPTER = GetItem(3001);

        #endregion

        private static readonly List<Item> ItemsList = new List<Item>(PreItems);
    }

    #endregion

    #region Item

    public class Item
    {
        #region Constructor

        public Item(int itemId,
            string itemName,
            int maxStacks,
            int price,
            bool isRecipe,
            ItemTier itemTier,
            ItemCategory itemCategory,
            float flatCritDamageMod,
            float flatPhysicalDamageMod,
            float flatMovementSpeedMod,
            float flatCritChanceMod,
            float flatMagicDamageMod,
            float flatHpRegenMod,
            float flatHpPoolMod,
            float flatSpellBlockMod,
            float flatArmorMod,
            float precentAttackSpeedMod,
            float percentSpellBlockMod,
            float percentHpPoolMod,
            float percentCritDamageMod,
            float percentArmorMod,
            float percentExpBonus,
            float percentHpRegenMod,
            float percentMagicDamageMod,
            float percentMovementSpeedMod,
            float percentPhysicalDamageMod,
            int[] recipeItems = null)
        {
            // BASE STATS
            _itemId = itemId;
            _itemName = itemName;
            _maxStacks = maxStacks;
            _price = price;
            _sellValue = IsReducedSellItem() ? ((price * 30) / 100) : ((price * 70) / 100);
            _isRecipe = isRecipe;
            _itemTier = itemTier;

            // RECIPE
            _recipeItems = recipeItems;

            // CATEGORY
            _itemCategory = itemCategory;

            // Data
            FlatCritDamageMod = flatCritDamageMod;
            FlatPhysicalDamageMod = flatPhysicalDamageMod;
            FlatMovementSpeedMod = flatMovementSpeedMod;
            FlatCritChanceMod = flatCritChanceMod;
            FlatMagicDamageMod = flatMagicDamageMod;
            FlatHpRegenMod = flatHpRegenMod;
            FlatHpPoolMod = flatHpPoolMod;
            FlatSpellBlockMod = flatSpellBlockMod;
            FlatArmorMod = flatArmorMod;
            PrecentAttackSpeedMod = precentAttackSpeedMod;
            PercentSpellBlockMod = percentSpellBlockMod;
            PercentHpPoolMod = percentHpPoolMod;
            PercentCritDamageMod = percentCritDamageMod;
            PercentArmorMod = percentArmorMod;
            PercentExpBonus = percentExpBonus;
            PercentHpRegenMod = percentHpRegenMod;
            PercentMagicDamageMod = percentMagicDamageMod;
            PercentMovementSpeedMod = percentMovementSpeedMod;
            PercentPhysicalDamageMod = percentPhysicalDamageMod;
        }

        #endregion

        #region Other Functions

        private bool IsReducedSellItem()
        {
            switch (_itemId)
            {
                case 3069:
                case 3092:
                case 1055:
                case 1054:
                case 1039:
                case 1062:
                case 1063:
                    return true;
                default:
                    return false;
            }
        }

        #endregion

        #region Item Variables

        public readonly float FlatArmorMod;
        public readonly float FlatCritChanceMod;

        public readonly float FlatCritDamageMod;
        public readonly float FlatHpPoolMod;
        public readonly float FlatHpRegenMod;
        public readonly float FlatMagicDamageMod;
        public readonly float FlatMovementSpeedMod;
        public readonly float FlatPhysicalDamageMod;
        public readonly float FlatSpellBlockMod;

        private readonly bool _isRecipe;

        private readonly ItemCategory _itemCategory;
        private readonly ItemTier _itemTier;

        private readonly int _itemId;
        private readonly string _itemName;
        private readonly int _maxStacks;

        public readonly float PercentArmorMod;
        public readonly float PercentCritDamageMod;
        public readonly float PercentExpBonus;
        public readonly float PercentHpPoolMod;
        public readonly float PercentHpRegenMod;
        public readonly float PercentMagicDamageMod;
        public readonly float PercentMovementSpeedMod;
        public readonly float PercentPhysicalDamageMod;
        public readonly float PercentSpellBlockMod;
        public readonly float PrecentAttackSpeedMod;

        private readonly int _price;
        private readonly int[] _recipeItems;
        private readonly int _sellValue;

        #endregion

        #region Get Functions

        public int GetId()
        {
            return _itemId;
        }

        public string GetName()
        {
            return _itemName;
        }

        public int GetMaxStacks()
        {
            return _maxStacks;
        }

        public int GetPrice()
        {
            return _price;
        }

        public int GetTotalPrice()
        {
            return GetRecipePrice() + _price;
        }

        public int GetRecipePrice()
        {
            return _recipeItems == null ? 0 : _recipeItems.Sum(i => Items.GetItem(i).GetTotalPrice());
        }

        public int GetSellPrice()
        {
            return _sellValue;
        }

        public bool IsRecipeComponent()
        {
            return _isRecipe;
        }

        public ItemTier GetTier()
        {
            return _itemTier;
        }

        public int[] GetComponents()
        {
            return _recipeItems;
        }

        public int GetPriceToBuy()
        {
            var pri = _price;
            if (GetComponents() != null)
                foreach (var comps in GetComponents())
                {
                    if (LeagueSharp.Common.Items.HasItem(comps))
                        pri -= Items.GetItem(comps).GetPrice();
                }
            return pri;
        }

        public List<Item> GetCopmponentList()
        {
            var list = new List<Item>();
            var components = GetComponents();
            if (components != null && components.Count() > 0)
            {
                list.AddRange(components.Select(Items.GetItem));
            }
            return list;
        }

        public ItemId GetItemId()
        {
            return (ItemId)_itemId;
        }

        public ItemCategory GetItemCategory()
        {
            return _itemCategory;
        }

        #endregion
    }

    #endregion

    #region ItemTier

    public enum ItemTier
    {
        None,
        Basic,
        Advanced,
        Legendary,
        Mythical,
        Enchantment,
        Consumable,
        RengarsTrinket,
        BasicTrinket,
        AdvancedTrinket
    }

    #endregion

    #region ItemCategory

    [Flags]
    public enum ItemCategory
    {
        None = 0,
        CriticalStrike = 1 << 0,
        HealthRegen = 1 << 1,
        Consumable = 1 << 2,
        Health = 1 << 3,
        Damage = 1 << 4,
        ManaRegen = 1 << 5,
        SpellBlock = 1 << 6,
        AttackSpeed = 1 << 7,
        LifeSteal = 1 << 8,
        SpellDamage = 1 << 9,
        Mana = 1 << 10,
        Armor = 1 << 11
    }

    #endregion
}