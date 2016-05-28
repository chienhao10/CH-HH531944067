using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;

namespace AutoSharp.Auto.SummonersRift.SRShopAI
{
    internal class Main
    {
        private static HowlingAbyss.ARAMShopAI.Item _lastItem;
        private static int _priceAddup, _lastShop;
        private static readonly List<HowlingAbyss.ARAMShopAI.Item> ItemList = new List<HowlingAbyss.ARAMShopAI.Item>();

        public static void ItemSequence(HowlingAbyss.ARAMShopAI.Item item, Queue<HowlingAbyss.ARAMShopAI.Item> shopListQueue)
        {
            if (item.From == null)
                shopListQueue.Enqueue(item);
            else
            {
                foreach (int itemDescendant in item.From)
                    ItemSequence(GetItemById(itemDescendant), shopListQueue);
                shopListQueue.Enqueue(item);
            }
        }

        public static HowlingAbyss.ARAMShopAI.Item GetItemById(int id)
        {
            return ItemList.Single(x => x.Id.Equals(id));
        }

        public static HowlingAbyss.ARAMShopAI.Item GetItemByName(string name)
        {
            return ItemList.FirstOrDefault(x => x.Name.Equals(name));
        }

        public static string Request(string url)
        {
            WebRequest request = WebRequest.Create(url);
            request.Credentials = CredentialCache.DefaultCredentials;
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            Debug.Assert(dataStream != null, "dataStream != null");
            StreamReader reader = new StreamReader(stream: dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            response.Close();
            return responseFromServer;
        }

        public static string[] List = new[] { "", "", "", "", "", "" };

        public static string[] Aatrox =
        {
            "The Bloodthirster", "Statikk Shiv",
            "Berserker's Greaves", "Spirit Visage", "Frozen Mallet", "Randuin's Omen"
        };

        public static string[] Ahri =
        {
            "Athene's Unholy Grail", "Morellonomicon",
            "Sorcerer's Shoes", "Void Staff", "Rabadon's Deathcap", "Liandry's Torment"
        };

        public static string[] Akali =
        {
            "Lich Bane", "Void Staff", "Rabadon's Deathcap",
            "Sorcerer's Shoes", "Zhonya's Hourglass", "Luden's Echo"
        };

        public static string[] Alistar =
        {
            "Face of the Mountain", "Frozen Heart",
            "Mercury's Treads", "Thornmail", "Locket of the Iron Solari", "Banshee's Veil"
        };

        public static string[] Amumu =
        {
            "Liandry's Torment", "Abyssal Scepter",
            "Boots of Mobility", "Sunfire Cape", "Rylai's Crystal Scepter", "Warmog's Armor"
        };

        public static string[] Anivia =
        {
            "Athene's Unholy Grail", "Morellonomicon",
            "Sorcerer's Shoes", "Void Staff", "Rabadon's Deathcap", "Liandry's Torment"
        };

        public static string[] Annie =
        {
            "Athene's Unholy Grail", "Morellonomicon",
            "Sorcerer's Shoes", "Void Staff", "Rabadon's Deathcap", "Luden's Echo"
        };

        public static string[] Ashe =
        {
            "Blade of the Ruined King", "The Bloodthirster",
            "Berserker's Greaves", "Infinity Edge", "Statikk Shiv", "Statikk Shiv"
        };

        public static string[] Azir =
        {
            "Athene's Unholy Grail", "Morellonomicon",
            "Sorcerer's Shoes", "Rabadon's Deathcap", "Rylai's Crystal Scepter", "Luden's Echo"
        };

        public static string[] Bard =
        {
            "Talisman of Ascension", "Frozen Heart",
            "Boots of Mobility", "Locket of the Iron Solari", "Mikael's Crucible", "Ardent Censer"
        };

        public static string[] Blitzcrank =
        {
            "Face of the Mountain", "Frozen Heart",
            "Mercury's Treads", "Thornmail", "Locket of the Iron Solari", "Banshee's Veil"
        };

        public static string[] Brand =
        {
            "Athene's Unholy Grail", "Morellonomicon",
            "Sorcerer's Shoes", "Void Staff", "Rabadon's Deathcap", "Liandry's Torment"
        };

        public static string[] Braum =
        {
            "Face of the Mountain", "Frozen Heart",
            "Mercury's Treads", "Thornmail", "Locket of the Iron Solari", "Banshee's Veil"
        };

        public static string[] Caitlyn =
        {
            "Blade of the Ruined King", "The Bloodthirster",
            "Berserker's Greaves", "Infinity Edge", "Statikk Shiv", "Statikk Shiv"
        };

        public static string[] Cassiopeia =
        {
            "Archangel's Staff", "Liandry's Torment",
            "Sorcerer's Shoes", "Rabadon's Deathcap", "Zhonya's Hourglass", "Luden's Echo"
        };

        public static string[] Chogath =
        {
            "Randuin's Omen", "Abyssal Scepter",
            "Ninja Tabi", "Spirit Visage", "Wit's End", "Locket of the Iron Solari"
        };

        public static string[] Corki =
        {
            "Trinity Force", "Statikk Shiv", "Berserker's Greaves",
            "The Bloodthirster", "Infinity Edge", "Banshee's Veil"
        };

        public static string[] Darius =
        {
            "The Bloodthirster", "Ravenous Hydra",
            "Mercury's Treads", "Randuin's Omen", "Banshee's Veil", "Trinity Force"
        };

        public static string[] Diana =
        {
            "Lich Bane", "Void Staff", "Rabadon's Deathcap",
            "Sorcerer's Shoes", "Zhonya's Hourglass", "Luden's Echo"
        };

        public static string[] DrMundo =
        {
            "Warmog's Armor", "Liandry's Torment",
            "Mercury's Treads", "Abyssal Scepter", "Thornmail", "Warmog's Armor"
        };

        public static string[] Draven =
        {
            "Blade of the Ruined King", "The Bloodthirster",
            "Berserker's Greaves", "Infinity Edge", "Statikk Shiv", "Statikk Shiv"
        };

        public static string[] Ekko =
        {
            "Athene's Unholy Grail", "Morellonomicon",
            "Sorcerer's Shoes", "Rabadon's Deathcap", "Void Staff", "Lich Bane"
        };

        public static string[] Elise =
        {
            "Liandry's Torment", "Athene's Unholy Grail",
            "Sorcerer's Shoes", "Rylai's Crystal Scepter", "Rabadon's Deathcap", "Void Staff",
        };

        public static string[] Evelynn =
        {
            "Rylai's Crystal Scepter", "Liandry's Torment",
            "Sorcerer's Shoes", "Abyssal Scepter", "Zhonya's Hourglass", "Rabadon's Deathcap"
        };

        public static string[] Ezreal =
        {
            "Trinity Force", "Statikk Shiv", "Berserker's Greaves",
            "The Bloodthirster", "Infinity Edge", "Banshee's Veil"
        };

        public static string[] FiddleSticks =
        {
            "Athene's Unholy Grail", "Rod of Ages",
            "Sorcerer's Shoes", "Void Staff", "Rabadon's Deathcap", "Liandry's Torment"
        };

        public static string[] Fiora =
        {
            "Ravenous Hydra", "Statikk Shiv",
            "Ninja Tabi", "Blade of the Ruined King", "Spirit Visage", "Randuin's Omen"
        };

        public static string[] Fizz =
        {
            "Lich Bane", "Void Staff", "Rabadon's Deathcap",
            "Sorcerer's Shoes", "Zhonya's Hourglass", "Luden's Echo"
        };

        public static string[] Galio =
        {
            "Athene's Unholy Grail", "Abyssal Scepter",
            "Mercury's Treads", "Zhonya's Hourglass", "Spirit Visage", "Thornmail"
        };

        public static string[] Gangplank =
        {
            "Blade of the Ruined King", "The Bloodthirster",
            "Berserker's Greaves", "Infinity Edge", "Statikk Shiv", "Statikk Shiv"
        };

        public static string[] Garen =
        {
            "Warmog's Armor", "Spirit Visage",
            "Ninja Tabi", "Randuin's Omen", "Banshee's Veil", "Frozen Mallet"
        };

        public static string[] Gnar =
        {
            "The Black Cleaver", "Statikk Shiv",
            "Mercury's Treads", "Sunfire Cape", "Blade of the Ruined King", "Frozen Mallet"
        };

        public static string[] Gragas =
        {
            "Rod of Ages", "Ionian Boots of Lucidity",
            "Rabadon's Deathcap", "Abyssal Scepter", "Void Staff", "Zhonya's Hourglass"
        };

        public static string[] Graves =
        {
            "Blade of the Ruined King", "The Bloodthirster",
            "Berserker's Greaves", "Infinity Edge", "Statikk Shiv", "Statikk Shiv"
        };

        public static string[] Hecarim =
        {
            "Ravenous Hydra", "Trinity Force",
            "Ninja Tabi", "Frozen Mallet", "Blade of the Ruined King", "Statikk Shiv"
        };

        public static string[] Heimerdinger =
        {
            "Athene's Unholy Grail", "Morellonomicon",
            "Sorcerer's Shoes", "Void Staff", "Rabadon's Deathcap", "Liandry's Torment"
        };

        public static string[] Irelia =
        {
            "Randuin's Omen", "Spirit Visage",
            "Mercury's Treads", "Frozen Mallet", "Trinity Force", "Frozen Heart"
        };

        public static string[] Janna =
        {
            "Talisman of Ascension", "Frozen Heart",
            "Boots of Mobility", "Locket of the Iron Solari", "Mikael's Crucible", "Ardent Censer"
        };

        public static string[] JarvanIV =
        {
            "The Black Cleaver", "Ravenous Hydra",
            "Mercury's Treads", "Statikk Shiv", "Warmog's Armor", "Randuin's Omen"
        };

        public static string[] Jax =
        {
            "Trinity Force", "Blade of the Ruined King",
            "Mercury's Treads", "Randuin's Omen", "Banshee's Veil", "Warmog's Armor"
        };

        public static string[] Jayce =
        {
            "Manamune", "Statikk Shiv",
            "Ionian Boots of Lucidity", "Youmuu's Ghostblade", "The Black Cleaver", "Maw of Malmortius"
        };

        public static string[] Jinx =
        {
            "Blade of the Ruined King", "The Bloodthirster",
            "Berserker's Greaves", "Infinity Edge", "Statikk Shiv", "Statikk Shiv"
        };

        public static string[] Kalista =
        {
            "Blade of the Ruined King", "The Bloodthirster", "Runaan's Hurricane",
            "Berserker's Greaves", "Infinity Edge", "Statikk Shiv"
        };

        public static string[] Karma =
        {
            "Athene's Unholy Grail", "Morellonomicon",
            "Sorcerer's Shoes", "Rabadon's Deathcap", "Rylai's Crystal Scepter", "Luden's Echo"
        };

        public static string[] Karthus =
        {
            "Athene's Unholy Grail", "Morellonomicon",
            "Sorcerer's Shoes", "Void Staff", "Rabadon's Deathcap", "Zhonya's Hourglass"
        };

        public static string[] Kassadin =
        {
            "Athene's Unholy Grail", "Morellonomicon",
            "Sorcerer's Shoes", "Rod of Ages", "Void Staff", "Rabadon's Deathcap"
        };

        public static string[] Katarina =
        {
            "Liandry's Torment", "Void Staff", "Rabadon's Deathcap",
            "Sorcerer's Shoes", "Hextech Gunblade", "Luden's Echo"
        };

        public static string[] Kayle =
        {
            "Nashor's Tooth", "Runaan's Hurricane",
            "Sorcerer's Shoes", "Rabadon's Deathcap", "Liandry's Torment", "Void Staff"
        };

        public static string[] Kennen =
        {
            "Rabadon's Deathcap", "Void Staff",
            "Sorcerer's Shoes", "Zhonya's Hourglass", "Liandry's Torment", "Rylai's Crystal Scepter"
        };

        public static string[] Khazix =
        {
            "The Bloodthirster", "Ravenous Hydra",
            "Ninja Tabi", "Maw of Malmortius", "Statikk Shiv", "The Black Cleaver"
        };

        public static string[] KogMaw =
        {
            "Athene's Unholy Grail", "Rabadon's Deathcap",
            "Sorcerer's Shoes", "Rod of Ages", "Nashor's Tooth", "Lich Bane",
        };

        public static string[] Leblanc =
        {
            "Athene's Unholy Grail", "Morellonomicon",
            "Sorcerer's Shoes", "Rabadon's Deathcap", "Void Staff", "Luden's Echo"
        };

        public static string[] LeeSin =
        {
            "Blade of the Ruined King", "Ravenous Hydra",
            "Mercury's Treads", "Sunfire Cape", "The Bloodthirster", "Statikk Shiv"
        };

        public static string[] Leona =
        {
            "Face of the Mountain", "Frozen Heart",
            "Mercury's Treads", "Thornmail", "Locket of the Iron Solari", "Banshee's Veil"
        };

        public static string[] Lissandra =
        {
            "Athene's Unholy Grail", "Morellonomicon",
            "Sorcerer's Shoes", "Rod of Ages", "Rabadon's Deathcap", "Liandry's Torment"
        };

        public static string[] Lucian =
        {
            "Infinity Edge", "Statikk Shiv", "The Bloodthirster",
            "Berserker's Greaves", "Statikk Shiv", "Blade of the Ruined King"
        };

        public static string[] Lulu =
        {
            "Athene's Unholy Grail", "Morellonomicon",
            "Sorcerer's Shoes", "Void Staff", "Rabadon's Deathcap", "Liandry's Torment"
        };

        public static string[] Lux =
        {
            "Athene's Unholy Grail", "Morellonomicon",
            "Sorcerer's Shoes", "Void Staff", "Rabadon's Deathcap", "Zhonya's Hourglass"
        };

        public static string[] Malphite =
        {
            "Sunfire Cape", "Banshee's Veil",
            "Ninja Tabi", "Spirit Visage", "Randuin's Omen", "Warmog's Armor"
        };

        public static string[] Malzahar =
        {
            "Athene's Unholy Grail", "Morellonomicon",
            "Sorcerer's Shoes", "Void Staff", "Rabadon's Deathcap", "Liandry's Torment"
        };

        public static string[] Maokai =
        {
            "Sunfire Cape", "Spirit Visage",
            "Boots of Mobility", "Rod of Ages", "Randuin's Omen", "Banshee's Veil"
        };

        public static string[] MasterYi =
        {
            "Infinity Edge", "Statikk Shiv",
            "The Bloodthirster", "Statikk Shiv", "Statikk Shiv", "Frozen Mallet"
        };

        public static string[] MissFortune =
        {
            "Infinity Edge", "Statikk Shiv", "The Bloodthirster",
            "Berserker's Greaves", "Infinity Edge", "Statikk Shiv", "Frozen Mallet"
        };

        public static string[] Mordekaiser =
        {
            "Rod of Ages", "Rabadon's Deathcap",
            "Sorcerer's Shoes", "Spirit Visage", "Zhonya's Hourglass", "Liandry's Torment"
        };

        public static string[] Morgana =
        {
            "Athene's Unholy Grail", "Morellonomicon",
            "Sorcerer's Shoes", "Void Staff", "Rabadon's Deathcap", "Abyssal Scepter",
        };

        public static string[] Nami =
        {
            "Talisman of Ascension", "Frozen Heart",
            "Boots of Mobility", "Locket of the Iron Solari", "Mikael's Crucible", "Ardent Censer"
        };

        public static string[] Nasus =
        {
            "Trinity Force", "Spirit Visage",
            "Mercury's Treads", "Randuin's Omen", "Banshee's Veil", "Frozen Heart"
        };

        public static string[] Nautilus =
        {
            "Randuin's Omen", "Banshee's Veil",
            "Boots of Mobility", "Frozen Heart", "Spirit Visage", "Locket of the Iron Solari"
        };

        public static string[] Nidalee =
        {
            "Athene's Unholy Grail", "Morellonomicon",
            "Sorcerer's Shoes", "Rabadon's Deathcap", "Void Staff", "Luden's Echo"
        };

        public static string[] Nocturne =
        {
            "Ravenous Hydra", "Blade of the Ruined King",
            "Berserker's Greaves", "Youmuu's Ghostblade", "Statikk Shiv", "Frozen Mallet"
        };

        public static string[] Nunu =
        {
            "Athene's Unholy Grail", "Morellonomicon",
            "Ninja Tabi", "Rod of Ages", "Abyssal Scepter", "Rylai's Crystal Scepter"
        };

        public static string[] Olaf =
        {
            "Ravenous Hydra", "Blade of the Ruined King",
            "Mercury's Treads", "Frozen Heart", "Frozen Mallet", "Warmog's Armor"
        };

        public static string[] Orianna =
        {
            "Athene's Unholy Grail", "Morellonomicon",
            "Sorcerer's Shoes", "Void Staff", "Rabadon's Deathcap", "Liandry's Torment"
        };

        public static string[] Pantheon =
        {
            "Youmuu's Ghostblade", "The Black Cleaver",
            "Mercury's Treads", "Sunfire Cape", "Warmog's Armor", "The Bloodthirster"
        };

        public static string[] Poppy =
        {
            "Trinity Force", "Banshee's Veil",
            "Ionian Boots of Lucidity", "Frozen Heart", "Hextech Gunblade", "Void Staff",
        };

        public static string[] Quinn =
        {
            "Blade of the Ruined King", "The Bloodthirster",
            "Berserker's Greaves", "Infinity Edge", "Statikk Shiv", "Runaan's Hurricane"
        };

        public static string[] Rammus =
        {
            "Ohmwrecker", "Randuin's Omen",
            "Mercury's Treads", "Frozen Heart", "Banshee's Veil", "Warmog's Armor"
        };

        public static string[] RekSai =
        {
            "Frozen Mallet", "Ravenous Hydra",
            "Mercury's Treads", "Thornmail", "Spirit Visage", "Randuin's Omen"
        };

        public static string[] Renekton =
        {
            "Youmuu's Ghostblade", "Spirit Visage",
            "Mercury's Treads", "Randuin's Omen", "Banshee's Veil", "Warmog's Armor"
        };

        public static string[] Rengar =
        {
            "Youmuu's Ghostblade", "Ravenous Hydra",
            "Ninja Tabi", "Statikk Shiv", "The Black Cleaver", "Frozen Mallet"
        };

        public static string[] Riven =
        {
            "Youmuu's Ghostblade", "Ravenous Hydra",
            "Ionian Boots of Lucidity", "The Black Cleaver", "Statikk Shiv", "Frozen Mallet"
        };

        public static string[] Rumble =
        {
            "Rod of Ages", "Rabadon's Deathcap",
            "Sorcerer's Shoes", "Spirit Visage", "Zhonya's Hourglass", "Liandry's Torment"
        };

        public static string[] Ryze =
        {
            "Archangel's Staff", "Rod of Ages", "Frozen Heart", "Sorcerer's Shoes",
            "Void Staff", "Rod of Ages"
        };

        public static string[] Sejuani =
        {
            "Abyssal Scepter", "Sunfire Cape",
            "Boots of Mobility", "Warmog's Armor", "Righteous Glory", "Locket of the Iron Solari"
        };

        public static string[] Shaco =
        {
            "Ravenous Hydra", "Berserker's Greaves",
            "Infinity Edge", "Statikk Shiv", "Trinity Force", "Youmuu's Ghostblade"
        };

        public static string[] Shen =
        {
            "Sunfire Cape", "Spirit Visage",
            "Mercury's Treads", "Randuin's Omen", "Wit's End", "Warmog's Armor"
        };

        public static string[] Shyvana =
        {
            "Blade of the Ruined King", "Randuin's Omen",
            "Mercury's Treads", "Spirit Visage", "Ravenous Hydra", "Frozen Mallet"
        };

        public static string[] Singed =
        {
            "Rod of Ages", "Rylai's Crystal Scepter",
            "Mercury's Treads", "Liandry's Torment", "Thornmail", "Banshee's Veil"
        };

        public static string[] Sion =
        {
            "Frozen Heart", "Spirit Visage",
            "Mercury's Treads", "Warmog's Armor", "Banshee's Veil", "Righteous Glory"
        };

        public static string[] Sivir =
        {
            "Infinity Edge", "The Bloodthirster",
            "Berserker's Greaves", "Statikk Shiv", "Statikk Shiv", "Blade of the Ruined King"
        };

        public static string[] Skarner =
        {
            "Trinity Force", "Blade of the Ruined King",
            "Mercury's Treads", "Randuin's Omen", "Banshee's Veil", "Sunfire Cape"
        };

        public static string[] Sona =
        {
            "Athene's Unholy Grail", "Morellonomicon",
            "Sorcerer's Shoes", "Rabadon's Deathcap", "Void Staff", "Luden's Echo"
        };

        public static string[] Soraka =
        {
            "Athene's Unholy Grail", "Morellonomicon",
            "Sorcerer's Shoes", "Rabadon's Deathcap", "Void Staff", "Luden's Echo"
        };

        public static string[] Swain =
        {
            "Athene's Unholy Grail", "Rod of Ages",
            "Sorcerer's Shoes", "Rod of Ages", "Rabadon's Deathcap", "Liandry's Torment"
        };

        public static string[] Syndra =
        {
            "Athene's Unholy Grail", "Morellonomicon",
            "Sorcerer's Shoes", "Rod of Ages", "Rabadon's Deathcap", "Void Staff"
        };

        public static string[] TahmKench =
        {
            "Sunfire Cape", "Spirit Visage",
            "Mercury's Treads", "Warmog's Armor", "Randuin's Omen", "Liandry's Torment"
        };

        public static string[] Talon =
        {
            "Ravenous Hydra", "Youmuu's Ghostblade",
            "Ninja Tabi", "Statikk Shiv", "Maw of Malmortius", "Frozen Mallet"
        };

        public static string[] Taric =
        {
            "Face of the Mountain", "Frozen Heart",
            "Mercury's Treads", "Thornmail", "Locket of the Iron Solari", "Banshee's Veil"
        };

        public static string[] Teemo =
        {
            "Nashor's Tooth", "Runaan's Hurricane",
            "Sorcerer's Shoes", "Rabadon's Deathcap", "Liandry's Torment", "Void Staff"
        };

        public static string[] Thresh =
        {
            "Face of the Mountain", "Frozen Heart",
            "Mercury's Treads", "Thornmail", "Locket of the Iron Solari", "Banshee's Veil"
        };

        public static string[] Tristana =
        {
            "Infinity Edge", "The Bloodthirster",
            "Berserker's Greaves", "Statikk Shiv", "Statikk Shiv", "Blade of the Ruined King"
        };

        public static string[] Trundle =
        {
            "Ravenous Hydra", "Spirit Visage",
            "Mercury's Treads", "Randuin's Omen", "Banshee's Veil", "Thornmail"
        };

        public static string[] Tryndamere =
        {
            "Statikk Shiv", "Ravenous Hydra",
            "Berserker's Greaves", "The Black Cleaver", "Statikk Shiv", "Mercurial Scimitar"
        };

        public static string[] TwistedFate =
        {
            "Athene's Unholy Grail", "Morellonomicon",
            "Sorcerer's Shoes", "Rabadon's Deathcap", "Void Staff", "Luden's Echo"
        };

        public static string[] Twitch =
        {
            "Blade of the Ruined King", "The Bloodthirster",
            "Berserker's Greaves", "Infinity Edge", "Statikk Shiv", "Statikk Shiv"
        };

        public static string[] Udyr =
        {
            "Trinity Force", "Spirit Visage",
            "Mercury's Treads", "Frozen Heart", "Blade of the Ruined King", "Frozen Mallet"
        };

        public static string[] Urgot =
        {
            "Manamune", "The Black Cleaver", "Mercury's Treads",
            "Frozen Heart", "Statikk Shiv", "Maw of Malmortius"
        };

        public static string[] Varus =
        {
            "Blade of the Ruined King", "The Bloodthirster",
            "Berserker's Greaves", "Infinity Edge", "Statikk Shiv", "Statikk Shiv"
        };

        public static string[] Vayne =
        {
            "Blade of the Ruined King", "The Bloodthirster",
            "Berserker's Greaves", "Infinity Edge", "Statikk Shiv", "Statikk Shiv"
        };

        public static string[] Veigar =
        {
            "Athene's Unholy Grail", "Morellonomicon",
            "Sorcerer's Shoes", "Void Staff", "Rabadon's Deathcap", "Zhonya's Hourglass"
        };

        public static string[] Velkoz =
        {
            "Athene's Unholy Grail", "Morellonomicon",
            "Sorcerer's Shoes", "Rabadon's Deathcap", "Rylai's Crystal Scepter", "Luden's Echo"
        };

        public static string[] Vi =
        {
            "The Black Cleaver", "Blade of the Ruined King",
            "Mercury's Treads", "Randuin's Omen", "Banshee's Veil", "Warmog's Armor"
        };

        public static string[] Viktor =
        {
            "Athene's Unholy Grail", "Morellonomicon",
            "Sorcerer's Shoes", "Perfect Hex Core", "Rabadon's Deathcap", "Void Staff"
        };

        public static string[] Vladimir =
        {
            "Rod of Ages", "Rabadon's Deathcap",
            "Sorcerer's Shoes", "Spirit Visage", "Zhonya's Hourglass", "Liandry's Torment"
        };

        public static string[] Volibear =
        {
            "Spirit Visage", "Sunfire Cape",
            "Mercury's Treads", "Randuin's Omen", "Warmog's Armor", "Righteous Glory"
        };

        public static string[] Warwick =
        {
            "Spirit Visage", "Sunfire Cape",
            "Mercury's Treads", "Frozen Heart", "Frozen Mallet", "Blade of the Ruined King"
        };

        public static string[] MonkeyKing =
        {
            "Youmuu's Ghostblade", "The Black Cleaver",
            "Ninja Tabi", "Maw of Malmortius", "Statikk Shiv", "Ravenous Hydra"
        };

        public static string[] Xerath =
        {
            "Athene's Unholy Grail", "Morellonomicon",
            "Sorcerer's Shoes", "Rabadon's Deathcap", "Void Staff", "Luden's Echo"
        };

        public static string[] XinZhao =
        {
            "Blade of the Ruined King", "Youmuu's Ghostblade",
            "Mercury's Treads", "Infinity Edge", "Ravenous Hydra", "Randuin's Omen"
        };

        public static string[] Yasuo =
        {
            "Statikk Shiv", "The Bloodthirster",
            "Berserker's Greaves", "Infinity Edge", "Statikk Shiv", "Blade of the Ruined King"
        };

        public static string[] Yorick =
        {
            "Manamune", "Spirit Visage",
            "Mercury's Treads", "Frozen Heart", "Trinity Force", "Frozen Mallet"
        };

        public static string[] Zac =
        {
            "Spirit Visage", "Randuin's Omen",
            "Mercury's Treads", "Warmog's Armor", "Thornmail", "Frozen Mallet"
        };

        public static string[] Zed =
        {
            "Youmuu's Ghostblade", "Blade of the Ruined King",
            "Ionian Boots of Lucidity", "The Black Cleaver", "Statikk Shiv", "Frozen Mallet"
        };

        public static string[] Ziggs =
        {
            "Athene's Unholy Grail", "Morellonomicon",
            "Sorcerer's Shoes", "Void Staff", "Rabadon's Deathcap", "Liandry's Torment"
        };

        public static string[] Zilean =
        {
            "Athene's Unholy Grail", "Morellonomicon",
            "Sorcerer's Shoes", "Rabadon's Deathcap", "Void Staff", "Luden's Echo"
        };

        public static string[] Zyra =
        {
            "Athene's Unholy Grail", "Morellonomicon",
            "Sorcerer's Shoes", "Rabadon's Deathcap", "Liandry's Torment", "Rylai's Crystal Scepter"
        };

        public static Queue<HowlingAbyss.ARAMShopAI.Item> Queue = new Queue<HowlingAbyss.ARAMShopAI.Item>();
        public static bool CanBuy = true;

        public static void Init()
        {
            string itemJson = "https://raw.githubusercontent.com/myo/Experimental/master/item.json";
            string itemsData = Request(itemJson);
            string itemArray = itemsData.Split(new[] { "data" }, StringSplitOptions.None)[1];
            MatchCollection itemIdArray = Regex.Matches(itemArray, "[\"]\\d*[\"][:][{].*?(?=},\"\\d)");
            foreach (HowlingAbyss.ARAMShopAI.Item item in from object iItem in itemIdArray select new HowlingAbyss.ARAMShopAI.Item(iItem.ToString()))
                ItemList.Add(item);
            LeagueSharp.Common.CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            var name = ObjectManager.Player.ChampionName;
            switch (name)
            {

                case "Aatrox":
                    List = Aatrox;
                    break;
                case "Ahri":
                    List = Ahri;
                    break;
                case "Akali":
                    List = Akali;
                    break;
                case "Alistar":
                    List = Alistar;
                    break;
                case "Amumu":
                    List = Amumu;
                    break;
                case "Anivia":
                    List = Anivia;
                    break;
                case "Annie":
                    List = Annie;
                    break;
                case "Ashe":
                    List = Ashe;
                    break;
                case "Azir":
                    List = Azir;
                    break;
                case "Bard":
                    List = Bard;
                    break;
                case "Blitzcrank":
                    List = Blitzcrank;
                    break;
                case "Brand":
                    List = Brand;
                    break;
                case "Braum":
                    List = Braum;
                    break;
                case "Caitlyn":
                    List = Caitlyn;
                    break;
                case "Cassiopeia":
                    List = Cassiopeia;
                    break;
                case "Chogath":
                    List = Chogath;
                    break;
                case "Corki":
                    List = Corki;
                    break;
                case "Darius":
                    List = Darius;
                    break;
                case "Diana":
                    List = Diana;
                    break;
                case "DrMundo":
                    List = DrMundo;
                    break;
                case "Draven":
                    List = Draven;
                    break;
                case "Ekko":
                    List = Ekko;
                    break;
                case "Elise":
                    List = Elise;
                    break;
                case "Evelynn":
                    List = Evelynn;
                    break;
                case "Ezreal":
                    List = Ezreal;
                    break;
                case "FiddleSticks":
                    List = FiddleSticks;
                    break;
                case "Fiora":
                    List = Fiora;
                    break;
                case "Fizz":
                    List = Fizz;
                    break;
                case "Galio":
                    List = Galio;
                    break;
                case "Gangplank":
                    List = Gangplank;
                    break;
                case "Garen":
                    List = Garen;
                    break;
                case "Gnar":
                    List = Gnar;
                    break;
                case "Gragas":
                    List = Gragas;
                    break;
                case "Graves":
                    List = Graves;
                    break;
                case "Hecarim":
                    List = Hecarim;
                    break;
                case "Heimerdinger":
                    List = Heimerdinger;
                    break;
                case "Irelia":
                    List = Irelia;
                    break;
                case "Janna":
                    List = Janna;
                    break;
                case "JarvanIV":
                    List = JarvanIV;
                    break;
                case "Jax":
                    List = Jax;
                    break;
                case "Jayce":
                    List = Jayce;
                    break;
                case "Jinx":
                    List = Jinx;
                    break;
                case "Kalista":
                    List = Kalista;
                    break;
                case "Karma":
                    List = Karma;
                    break;
                case "Karthus":
                    List = Karthus;
                    break;
                case "Kassadin":
                    List = Kassadin;
                    break;
                case "Katarina":
                    List = Katarina;
                    break;
                case "Kayle":
                    List = Kayle;
                    break;
                case "Kennen":
                    List = Kennen;
                    break;
                case "Khazix":
                    List = Khazix;
                    break;
                case "KogMaw":
                    List = KogMaw;
                    break;
                case "Leblanc":
                    List = Leblanc;
                    break;
                case "LeeSin":
                    List = LeeSin;
                    break;
                case "Leona":
                    List = Leona;
                    break;
                case "Lissandra":
                    List = Lissandra;
                    break;
                case "Lucian":
                    List = Lucian;
                    break;
                case "Lulu":
                    List = Lulu;
                    break;
                case "Lux":
                    List = Lux;
                    break;
                case "Malphite":
                    List = Malphite;
                    break;
                case "Malzahar":
                    List = Malzahar;
                    break;
                case "Maokai":
                    List = Maokai;
                    break;
                case "MasterYi":
                    List = MasterYi;
                    break;
                case "MissFortune":
                    List = MissFortune;
                    break;
                case "Mordekaiser":
                    List = Mordekaiser;
                    break;
                case "Morgana":
                    List = Morgana;
                    break;
                case "Nami":
                    List = Nami;
                    break;
                case "Nasus":
                    List = Nasus;
                    break;
                case "Nautilus":
                    List = Nautilus;
                    break;
                case "Nidalee":
                    List = Nidalee;
                    break;
                case "Nocturne":
                    List = Nocturne;
                    break;
                case "Nunu":
                    List = Nunu;
                    break;
                case "Olaf":
                    List = Olaf;
                    break;
                case "Orianna":
                    List = Orianna;
                    break;
                case "Pantheon":
                    List = Pantheon;
                    break;
                case "Poppy":
                    List = Poppy;
                    break;
                case "Quinn":
                    List = Quinn;
                    break;
                case "Rammus":
                    List = Rammus;
                    break;
                case "RekSai":
                    List = RekSai;
                    break;
                case "Renekton":
                    List = Renekton;
                    break;
                case "Rengar":
                    List = Rengar;
                    break;
                case "Riven":
                    List = Riven;
                    break;
                case "Rumble":
                    List = Rumble;
                    break;
                case "Ryze":
                    List = Ryze;
                    break;
                case "Sejuani":
                    List = Sejuani;
                    break;
                case "Shaco":
                    List = Shaco;
                    break;
                case "Shen":
                    List = Shen;
                    break;
                case "Shyvana":
                    List = Shyvana;
                    break;
                case "Singed":
                    List = Singed;
                    break;
                case "Sion":
                    List = Sion;
                    break;
                case "Sivir":
                    List = Sivir;
                    break;
                case "Skarner":
                    List = Skarner;
                    break;
                case "Sona":
                    List = Sona;
                    break;
                case "Soraka":
                    List = Soraka;
                    break;
                case "Swain":
                    List = Swain;
                    break;
                case "Syndra":
                    List = Syndra;
                    break;
                case "TahmKench":
                    List = TahmKench;
                    break;
                case "Talon":
                    List = Talon;
                    break;
                case "Taric":
                    List = Taric;
                    break;
                case "Teemo":
                    List = Teemo;
                    break;
                case "Thresh":
                    List = Thresh;
                    break;
                case "Tristana":
                    List = Tristana;
                    break;
                case "Trundle":
                    List = Trundle;
                    break;
                case "Tryndamere":
                    List = Tryndamere;
                    break;
                case "TwistedFate":
                    List = TwistedFate;
                    break;
                case "Twitch":
                    List = Twitch;
                    break;
                case "Udyr":
                    List = Udyr;
                    break;
                case "Urgot":
                    List = Urgot;
                    break;
                case "Varus":
                    List = Varus;
                    break;
                case "Vayne":
                    List = Vayne;
                    break;
                case "Veigar":
                    List = Veigar;
                    break;
                case "Velkoz":
                    List = Velkoz;
                    break;
                case "Vi":
                    List = Vi;
                    break;
                case "Viktor":
                    List = Viktor;
                    break;
                case "Vladimir":
                    List = Vladimir;
                    break;
                case "Volibear":
                    List = Volibear;
                    break;
                case "Warwick":
                    List = Warwick;
                    break;
                case "MonkeyKing":
                    List = MonkeyKing;
                    break;
                case "Xerath":
                    List = Xerath;
                    break;
                case "XinZhao":
                    List = XinZhao;
                    break;
                case "Yasuo":
                    List = Yasuo;
                    break;
                case "Yorick":
                    List = Yorick;
                    break;
                case "Zac":
                    List = Zac;
                    break;
                case "Zed":
                    List = Zed;
                    break;
                case "Ziggs":
                    List = Ziggs;
                    break;
                case "Zilean":
                    List = Zilean;
                    break;
                case "Zyra":
                    List = Zyra;
                    break;
                default:
                    List = Vayne;
                    break;
            }
            Queue = ShoppingQueue();
            AlterInventory();
            Game.OnUpdate += BuyItems;
        }

        public static Queue<HowlingAbyss.ARAMShopAI.Item> ShoppingQueue()
        {
            var shoppingItems = new Queue<HowlingAbyss.ARAMShopAI.Item>();
            foreach (string indexItem in List)
            {
                var macroItems = new Queue<HowlingAbyss.ARAMShopAI.Item>();
                ItemSequence(GetItemByName(indexItem), macroItems);
                foreach (HowlingAbyss.ARAMShopAI.Item secondIndexItem in macroItems)
                    shoppingItems.Enqueue(secondIndexItem);
            }
            return shoppingItems;
        }

        public static void BuyItems(EventArgs args)
        {
            if ((ObjectManager.Player.InFountain() || ObjectManager.Player.IsDead) && Environment.TickCount - _lastShop < new Random().Next(350, 450)) return;
            if (!InventoryFull() && !Items.HasItem(2003) && ObjectManager.Player.Gold > 400)
            {
                Shop.BuyItem(ItemId.Health_Potion);
            }
            if ((Queue.Peek() != null && InventoryFull()) &&
                   (Queue.Peek().From == null ||
                    (Queue.Peek().From != null && !Queue.Peek().From.Contains(_lastItem.Id))))
            {
                var y = Queue.Dequeue();
                _priceAddup += y.Goldbase;
            }
            if (Queue.Peek().Goldbase <= ObjectManager.Player.Gold - _priceAddup && Queue.Count > 0 &&
                   ObjectManager.Player.InShop())
            {
                var y = Queue.Dequeue();
                Shop.BuyItem((ItemId)y.Id);
                _lastItem = y;
                _priceAddup = 0;
            }
            _lastShop = Environment.TickCount;
        }

        public static int FreeSlots()
        {
            return -1 + ObjectManager.Player.InventoryItems.Count(y => !y.DisplayName.Contains("Poro"));
        }

        public static bool InventoryFull()
        {
            return FreeSlots() == 6;
        }

        public static void AlterInventory()
        {
            var y = 0;
            var z = ObjectManager.Player.InventoryItems.ToList().OrderBy(i => i.Slot).Select(item => item.Id).ToList();
            for (int i = 0; i < z.Count - 2; i++)
            {
                var x = GetItemById((int)z[i]);
                Queue<HowlingAbyss.ARAMShopAI.Item> temp = new Queue<HowlingAbyss.ARAMShopAI.Item>();
                ItemSequence(x, temp);
                y += temp.Count;
            }
            for (int i = 0; i < y; i++)
                Console.WriteLine(Queue.Dequeue());
        }
    }
}
