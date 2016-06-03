using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;

namespace AutoJungle.Data
{
    internal class ItemHandler
    {
        public static List<BuyingOrder> ItemList = new List<BuyingOrder>();

        public ItemHandler(BuildType type)
        {
            switch (type)
            {
                case BuildType.AD:
                    SetListAD();
                    break;
                case BuildType.AS:
                    SetListAS();
                    break;
                case BuildType.ASMANA:
                    SetListASMANA();
                    break;
                case BuildType.NOC:
                    SetListNOC();
                    break;
                case BuildType.EVE:
                    SetListEVE();
                    break;
                case BuildType.APADT:
                	SetListAPADT();
                	break;
                case BuildType.VB:
                    SetlistVB();
                    break;
            }
        }

        private void SetListASMANA()
        {
            ItemList.Add(new BuyingOrder(1, 1041, "Hunter's Machete", 350));
            ItemList.Add(new BuyingOrder(2, 2031, "Refillable Potion", 150));
            ItemList.Add(new BuyingOrder(3, 3340, "Trinket", 0));
            ItemList.Add(new BuyingOrder(4, 1039, "Hunter's Talisman", 350));
            ItemList.Add(new BuyingOrder(5, 3715, "Skirmisher's Sabre", 700));
            ItemList.Add(new BuyingOrder(6, 1042, "Dagger", 350));
            ItemList.Add(new BuyingOrder(7, 1419, "Bloodrazor", 1350));
            ItemList.Add(new BuyingOrder(8, (int) ItemId.Boots_of_Speed, "Boots of Speed", 475));
            ItemList.Add(new BuyingOrder(9, (int) ItemId.Tear_of_the_Goddess, "Tear of the Goddess", 750));
            ItemList.Add(new BuyingOrder(10, (int) ItemId.Boots_of_Swiftness, "Boots of Swoftness", 500));
            ItemList.Add(new BuyingOrder(11, (int) ItemId.Bilgewater_Cutlass, "Bilgewater Cutlass", 1650));
            ItemList.Add(new BuyingOrder(12, (int) ItemId.Blade_of_the_Ruined_King, "BOTRK", 1750));
            ItemList.Add(new BuyingOrder(13, (int) ItemId.Tiamat_Melee_Only, "Tiamat", 1250));
            ItemList.Add(new BuyingOrder(14, (int) ItemId.Vampiric_Scepter, "Vampiric_Screpter", 900));
            ItemList.Add(new BuyingOrder(15, (int) ItemId.Ravenous_Hydra_Melee_Only, "Ravenous Hydra Melee Only", 1450));
            ItemList.Add(new BuyingOrder(16, (int) ItemId.Manamune, "Ravenous Hydra Melee Only", 1650));
            ItemList.Add(new BuyingOrder(17, (int) ItemId.Zeal, "Zeal", 1200));
            ItemList.Add(new BuyingOrder(18, (int) ItemId.Phantom_Dancer, "Phantom Dancer", 1500));
        }

        private void SetListAD()
        {
            //ItemList.Add(new BuyingOrder());
        }

        private void SetListAS()
        {
            ItemList.Add(new BuyingOrder(1, 1041, "Hunter's Machete", 350));
            ItemList.Add(new BuyingOrder(2, 2031, "Refillable Potion", 150));
            ItemList.Add(new BuyingOrder(3, 3340, "Trinket", 0));
            ItemList.Add(new BuyingOrder(4, 1039, "Hunter's Talisman", 350));
            ItemList.Add(new BuyingOrder(5, 3715, "Skirmisher's Sabre", 700));
            ItemList.Add(new BuyingOrder(6, 1042, "Dagger", 350));
            ItemList.Add(new BuyingOrder(7, 1419, "Bloodrazor", 1350));
            ItemList.Add(new BuyingOrder(8, (int) ItemId.Boots_of_Speed, "Boots of Speed", 475));
            ItemList.Add(new BuyingOrder(9, (int) ItemId.Boots_of_Swiftness, "Boots of Swoftness", 500));
            ItemList.Add(new BuyingOrder(10, (int) ItemId.Bilgewater_Cutlass, "Bilgewater Cutlass", 1650));
            ItemList.Add(new BuyingOrder(11, (int) ItemId.Blade_of_the_Ruined_King, "BOTRK", 1750));
            ItemList.Add(new BuyingOrder(12, (int) ItemId.Recurve_Bow, "Recurve Bow", 1000));
            ItemList.Add(new BuyingOrder(13, (int) ItemId.Wits_End, "Wits End", 1800));
            ItemList.Add(new BuyingOrder(14, (int) ItemId.Tiamat_Melee_Only, "Tiamat", 1250));
            ItemList.Add(new BuyingOrder(15, (int) ItemId.Vampiric_Scepter, "Vampiric_Screpter", 900));
            ItemList.Add(new BuyingOrder(16, (int) ItemId.Ravenous_Hydra_Melee_Only, "Ravenous Hydra Melee Only", 1450));
            ItemList.Add(new BuyingOrder(17, (int) ItemId.Zeal, "Zeal", 1200));
            ItemList.Add(new BuyingOrder(18, (int) ItemId.Phantom_Dancer, "Phantom Dancer", 1500));
        }

        private void SetListNOC()
        {
            ItemList.Add(new BuyingOrder(1, 1041, "Hunter's Machete", 350));
            ItemList.Add(new BuyingOrder(2, 2031, "Refillable Potion", 150));
            ItemList.Add(new BuyingOrder(3, 3340, "Trinket", 0));
            ItemList.Add(new BuyingOrder(4, 1039, "Hunter's Talisman", 350));
            ItemList.Add(new BuyingOrder(5, 3715, "Skirmisher's Sabre", 700));
            ItemList.Add(new BuyingOrder(6, 1042, "Dagger", 350));
            ItemList.Add(new BuyingOrder(7, 1419, "Bloodrazor", 1350));
            ItemList.Add(new BuyingOrder(8, (int) ItemId.Boots_of_Speed, "Boots of Speed", 475));
            ItemList.Add(new BuyingOrder(9, (int) ItemId.Boots_of_Swiftness, "Boots of Swoftness", 500));
            ItemList.Add(new BuyingOrder(10, (int) ItemId.Bilgewater_Cutlass, "Bilgewater Cutlass", 1650));
            ItemList.Add(new BuyingOrder(11, (int) ItemId.Blade_of_the_Ruined_King, "BOTRK", 1750));
            ItemList.Add(new BuyingOrder(12, (int) ItemId.Giants_Belt, "Giants Belt", 1000));
            ItemList.Add(new BuyingOrder(13, (int) ItemId.Chain_Vest, "Chain Vest", 800));
            ItemList.Add(new BuyingOrder(14, 3742, "Dead Mans Plate", 1800));
            ItemList.Add(new BuyingOrder(15, (int) ItemId.Tiamat_Melee_Only, "Tiamat", 1250));
            ItemList.Add(new BuyingOrder(16, (int) ItemId.Vampiric_Scepter, "Vampiric_Screpter", 900));
            ItemList.Add(new BuyingOrder(17, (int) ItemId.Ravenous_Hydra_Melee_Only, "Ravenous Hydra Melee Only", 1450));
            ItemList.Add(new BuyingOrder(18, (int) ItemId.Zeal, "Zeal", 1200));
            ItemList.Add(new BuyingOrder(19, (int) ItemId.Phantom_Dancer, "Phantom Dancer", 1350));
            ItemList.Add(new BuyingOrder(14, 2410, "Elixir of Wrath", 500));
        }

        private void SetListAPADT()
        {
            ItemList.Add(new BuyingOrder(1, 1041, "Hunter's Machete", 350));
            ItemList.Add(new BuyingOrder(2, 2031, "Refillable Potion", 150));
            ItemList.Add(new BuyingOrder(3, 3340, "Trinket", 0));
            ItemList.Add(new BuyingOrder(4, 1039, "Hunter's Talisman", 350));
            ItemList.Add(new BuyingOrder(5, 3706, "Stalker's Blade", 700));
            ItemList.Add(new BuyingOrder(6, 3113, "Aether Wisp", 850));
            ItemList.Add(new BuyingOrder(7, 1402, "Runic Echoes", 1350));
            ItemList.Add(new BuyingOrder(8, 1001, "Boots of Speed", 750));
            ItemList.Add(new BuyingOrder(9, 3111, "Mercurys Treads", 400));
            ItemList.Add(new BuyingOrder(10, 3052, "Jaurims Fist", 1300));
            ItemList.Add(new BuyingOrder(11, 3077, "Tiamat", 1300));
            ItemList.Add(new BuyingOrder(12, 3748, "Titanic Hydra", 1100));
            ItemList.Add(new BuyingOrder(13, (int) ItemId.Giants_Belt, "Giants Belt", 1000));
            ItemList.Add(new BuyingOrder(14, (int) ItemId.Chain_Vest, "Chain Vest", 800));
            ItemList.Add(new BuyingOrder(15, 3742, "Dead Mans Plate", 1800));
            ItemList.Add(new BuyingOrder(16, 3211, "Spectres Cow", 1200));
            ItemList.Add(new BuyingOrder(17, 3102, "Banshees Veil", 1250));
            ItemList.Add(new BuyingOrder(18, (int) ItemId.Bilgewater_Cutlass, "Bilgewater Cutlass", 1650));
            ItemList.Add(new BuyingOrder(19, (int) ItemId.Blade_of_the_Ruined_King, "BOTRK", 1750));
            ItemList.Add(new BuyingOrder(20, 2410, "Elixir of Wrath", 500));
//add heal spell useage and keep buy Elixir of Wrath after full item
        }

        private void SetListEVE()
        {
            ItemList.Add(new BuyingOrder(1, 1041, "Hunter's Machete", 350));
            ItemList.Add(new BuyingOrder(2, 2031, "Refillable Potion", 150));
            ItemList.Add(new BuyingOrder(3, 3340, "Trinket", 0));
            ItemList.Add(new BuyingOrder(4, 1039, "Hunter's Talisman", 350));
            ItemList.Add(new BuyingOrder(5, 3706, "Stalker's Blade", 700));
            ItemList.Add(new BuyingOrder(6, 3113, "Aether Wisp", 850));
            ItemList.Add(new BuyingOrder(7, 1402, "Runic Echoes", 1350));
            ItemList.Add(new BuyingOrder(8, 1001, "Boots of Speed", 400));
            ItemList.Add(new BuyingOrder(9, 3158, "Ionian Boots of Lucidity", 600));
            ItemList.Add(new BuyingOrder(10, (int) ItemId.Giants_Belt, "Giants Belt", 1000));
            ItemList.Add(new BuyingOrder(11, 1058, "Needlessly Large Rod", 1250));
            ItemList.Add(new BuyingOrder(12, 3116, "Rylais Crystal Scepter", 1000));
            ItemList.Add(new BuyingOrder(13, 1058, "Needlessly Large Rod", 1250));
            ItemList.Add(new BuyingOrder(14, 3113, "Aether Wisp", 875));
            ItemList.Add(new BuyingOrder(15, 3285, "Ludens Echo", 1100));
            ItemList.Add(new BuyingOrder(16, 1026, "Blasting Wand", 850));
            ItemList.Add(new BuyingOrder(17, 3135, "Void Staf", 1800));
            ItemList.Add(new BuyingOrder(18, 1058, "Needlessly Large Rod", 1250));
            ItemList.Add(new BuyingOrder(19, 3089, "Rabadons Deathcap", 2500));
            ItemList.Add(new BuyingOrder(20, 2139, "Elixir of Sorcery", 500));
        }

        private void SetlistVB()
        {
            ItemList.Add(new BuyingOrder(1, 1041, "Hunter's Machete", 350));
            ItemList.Add(new BuyingOrder(2, 2031, "Refillable Potion", 150));
            ItemList.Add(new BuyingOrder(3, 3340, "Trinket", 0));
            ItemList.Add(new BuyingOrder(4, 1039, "Hunter's Talisman", 350));
            ItemList.Add(new BuyingOrder(5, 3706, "Stalker's Blade", 700));
            ItemList.Add(new BuyingOrder(6, 1028, "Ruby Crystal", 400));
            ItemList.Add(new BuyingOrder(7, 3751, "Bami's Cinder", 700));
            ItemList.Add(new BuyingOrder(8, 1401, "Cinderhulk", 625));
            ItemList.Add(new BuyingOrder(9, 1001, "Boots of Speed", 600));
            ItemList.Add(new BuyingOrder(10, 1028, "Ruby Crystal", 400));
            ItemList.Add(new BuyingOrder(11, 1011, "Giant's Belt", 600));
            ItemList.Add(new BuyingOrder(12, 1031, "Chain Vest", 800));
            ItemList.Add(new BuyingOrder(13, 3742, "Dead Man's Plate", 1800));
            ItemList.Add(new BuyingOrder(14, 3009, "Boots of Swiftness", 600));
            ItemList.Add(new BuyingOrder(15, 3211, "Spectres Cow", 1200));
            ItemList.Add(new BuyingOrder(16, 3065, "Spirit Visage", 1600));
            ItemList.Add(new BuyingOrder(17, 3052, "Jaurims Fist", 1300));
            ItemList.Add(new BuyingOrder(18, 3077, "Tiamat", 1300));
            ItemList.Add(new BuyingOrder(19, 3748, "Titanic Hydra", 1100));
            ItemList.Add(new BuyingOrder(20, (int) ItemId.Giants_Belt, "Giants Belt", 1000));
            ItemList.Add(new BuyingOrder(21, 3143, "Randuin's Omen", 1900));
            ItemList.Add(new BuyingOrder(22, 2138, "Elixir of Iron", 500));
        }

        public static void UseItemsJungle()
        {
            if (Items.HasItem((int) ItemId.Tiamat_Melee_Only) && Items.CanUseItem((int) ItemId.Tiamat_Melee_Only) &&
                Helpers.getMobs(Program.player.Position, 400).Count > 2)
            {
                Items.UseItem((int) ItemId.Tiamat_Melee_Only);
            }
            if (Items.HasItem((int) ItemId.Ravenous_Hydra_Melee_Only) &&
                Items.CanUseItem((int) ItemId.Ravenous_Hydra_Melee_Only) &&
                Helpers.getMobs(Program.player.Position, 400).Count > 2)
            {
                Items.UseItem((int) ItemId.Ravenous_Hydra_Melee_Only);
            }
            var muramana = LeagueSharp.Common.Data.ItemData.Muramana.GetItem().Id;
            if (Items.HasItem(muramana) && Items.CanUseItem(muramana) && Program.player.HasBuff("Muramana"))
            {
                Items.UseItem(muramana);
            }
        }

        public static void UseItemsCombo(Obj_AI_Base target, bool use)
        {
            if (Items.HasItem((int) ItemId.Tiamat_Melee_Only) && Items.CanUseItem((int) ItemId.Tiamat_Melee_Only) &&
                target.LSDistance(Program.player) < 400)
            {
                Items.UseItem((int) ItemId.Tiamat_Melee_Only);
            }
            if (Items.HasItem((int) ItemId.Ravenous_Hydra_Melee_Only) &&
                Items.CanUseItem((int) ItemId.Ravenous_Hydra_Melee_Only) && target.LSDistance(Program.player) < 400)
            {
                Items.UseItem((int) ItemId.Ravenous_Hydra_Melee_Only);
            }
            if (!use)
            {
                return;
            }
            if (Items.HasItem((int) ItemId.Bilgewater_Cutlass) && Items.CanUseItem((int) ItemId.Bilgewater_Cutlass) &&
                (target.LSDistance(Program.player) > Orbwalking.GetRealAutoAttackRange(target) ||
                 (target.HealthPercent < 35 && Program.player.HealthPercent < 35)))
            {
                Items.UseItem((int) ItemId.Bilgewater_Cutlass, target);
            }
            if (Items.HasItem((int) ItemId.Blade_of_the_Ruined_King) &&
                Items.CanUseItem((int) ItemId.Blade_of_the_Ruined_King) &&
                (target.LSDistance(Program.player) > Orbwalking.GetRealAutoAttackRange(target) ||
                 (target.HealthPercent < 35 && Program.player.HealthPercent < 35)))
            {
                Items.UseItem((int) ItemId.Blade_of_the_Ruined_King, target);
            }
            var muramana = LeagueSharp.Common.Data.ItemData.Muramana.GetItem().Id;
            if (Items.HasItem(muramana) && Items.CanUseItem(muramana))
            {
                if (!Program.player.HasBuff("Muramana") && Program.player.Mana > 250)
                {
                    Items.UseItem(muramana);
                }
                else if (Program.player.HasBuff("Muramana") && Program.player.Mana < 260)
                {
                    Items.UseItem(muramana);
                }
            }
        }
    }

    internal class BuyingOrder
    {
        public int Index;
        public int ItemId;
        public string Name;
        public int Price;

        public BuyingOrder(int idx, int itemid, string name, int price)
        {
            Index = idx;
            ItemId = itemid;
            Name = name;
            Price = price;
        }
    }

    internal enum BuildType
    {
        AS,
        AD,
        ASMANA,
        NOC,
        EVE,
        APADT,
        VB
    }
}