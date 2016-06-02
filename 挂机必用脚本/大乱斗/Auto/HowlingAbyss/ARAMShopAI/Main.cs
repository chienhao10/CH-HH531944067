using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using AutoSharp.Utils;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;

namespace AutoSharp.Auto.HowlingAbyss.ARAMShopAI
{
    internal class Main
    {
        private static AramItem _lastItem;
        private static int _priceAddup, _lastShop;
        private static readonly List<AramItem> ItemList = new List<AramItem>();

        public static void ItemSequence(AramItem item, Queue<AramItem> shopListQueue)
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

        public static AramItem GetItemById(int id)
        {
            return ItemList.Single(x => x.Id.Equals(id));
        }

        public static AramItem GetItemByName(string name)
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

        public static string[] List = new[] {"", "", "", "", "", ""};

        public static string[] Aatrox =
        {
            "The Bloodthirster", "Statikk Shiv",
            "Berserker's Greaves", "Spirit Visage", "Frozen Mallet", "Randuin's Omen"
        };

        public static string[] Ahri =
        {
            "Liandry's Torment", "Morellonomicon",
            "Sorcerer's Shoes", "Void Staff", "Zhonya's Hourglass", "Rabadon's Deathcap"
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
            "Luden's Echo", "Morellonomicon",
            "Sorcerer's Shoes", "Void Staff", "Rabadon's Deathcap", "Liandry's Torment"
        };

        public static string[] Annie =
        {
            "Morellonomicon", "Rylai's Crystal Scepter",
            "Sorcerer's Shoes", "Void Staff", "Rabadon's Deathcap", "Luden's Echo"
        };

        public static string[] Ashe =
        {
            "Infinity Edge", "Runaan's Hurricane",
            "Berserker's Greaves", "Mercurial Scimitar", "Lord Dominik's Regards", "Banshee's Veil"
        };

        public static string[] Azir =
        {
            "Nashor's Tooth", "Morellonomicon",
            "Sorcerer's Shoes", "Rabadon's Deathcap", "Rylai's Crystal Scepter", "Luden's Echo"
        };

        public static string[] Bard =
        {
            "Talisman of Ascension", "Sunfire Cape",
            "Mercury's Treads", "Luden's Echo", "Mikael's Crucible", "Ardent Censer"
        };

        public static string[] Blitzcrank =
        {
            "Face of the Mountain", "Frozen Heart",
            "Mercury's Treads", "Thornmail", "Locket of the Iron Solari", "Banshee's Veil"
        };

        public static string[] Brand =
        {
            "Luden's Echo", "Morellonomicon",
            "Sorcerer's Shoes", "Void Staff", "Rabadon's Deathcap", "Liandry's Torment"
        };

        public static string[] Braum =
        {
            "Face of the Mountain", "Frozen Heart",
            "Mercury's Treads", "Thornmail", "Locket of the Iron Solari", "Banshee's Veil"
        };

        public static string[] Caitlyn =
        {
            "Infinity Edge", "Berserker's Greaves",
            "Runaan's Hurricane", "Mercurial Scimitar", "Rapid Firecannon", "The Bloodthirster"
        };

        public static string[] Cassiopeia =
        {
            "Archangel's Staff", "Liandry's Torment",
            "Morellonomicon", "Rabadon's Deathcap", "Zhonya's Hourglass", "Luden's Echo"
        };

        public static string[] Chogath =
        {
            "Randuin's Omen", "Abyssal Scepter",
            "Ninja Tabi", "Spirit Visage", "Wit's End", "Locket of the Iron Solari"
        };

        public static string[] Corki =
        {
            "Trinity Force", "Mercury's Treads", "Maw of Malmortius",
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
            "Mercury's Treads", "Abyssal Scepter", "Thornmail", "Banshee's Veil"
        };

        public static string[] Draven =
        {
            "Youmuu's Ghostblade", "Boots of Swiftness",
            "Statikk Shiv", "Mercurial Scimitar", "Infinity Edge", "Mortal Reminder"
        };

        public static string[] Ekko =
        {
            "Lich Bane", "Morellonomicon",
            "Sorcerer's Shoes", "Rabadon's Deathcap", "Abyssal Scepter", "Frozen Mallet"
        };

        public static string[] Elise =
        {
            "Liandry's Torment", "Zhonya's Hourglass",
            "Sorcerer's Shoes", "Rylai's Crystal Scepter", "Rabadon's Deathcap", "Void Staff",
        };

        public static string[] Evelynn =
        {
            "Morellonomicon", "Liandry's Torment",
            "Sorcerer's Shoes", "Zhonya's Hourglass", "Rylai's Crystal Scepter", "Rabadon's Deathcap"
        };

        public static string[] Ezreal =
        {
            "Manamune", "Ionian Boots of Lucidity", "Iceborn Gauntlet",
            "Mercurial Scimitar", "Lord Dominik's Regards", "Maw of Malmortius"
        };

        public static string[] FiddleSticks =
        {
            "Morellonomicon", "Spirit Visage",
            "Sorcerer's Shoes", "Void Staff", "Rabadon's Deathcap", "Liandry's Torment"
        };

        public static string[] Fiora =
        {
            "Ravenous Hydra", "Statikk Shiv",
            "Ninja Tabi", "Blade of the Ruined King", "Spirit Visage", "Randuin's Omen"
        };

        public static string[] Fizz =
        {
            "Lich Bane", "Liandry's Torment", "Rabadon's Deathcap",
            "Sorcerer's Shoes", "Zhonya's Hourglass", "Luden's Echo"
        };

        public static string[] Galio =
        {
            "Rod of Ages", "Abyssal Scepter",
            "Mercury's Treads", "Zhonya's Hourglass", "Spirit Visage", "Thornmail"
        };

        public static string[] Gangplank =
        {
            "Blade of the Ruined King", "The Bloodthirster",
            "Berserker's Greaves", "Infinity Edge", "Statikk Shiv", "Frozen Mallet"
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
            "Death's Dance", "Boots of Swiftness",
            "Phantom Dancer", "Maw of Malmortius", "Sterak's Gage", "Frozen Mallet"
        };

        public static string[] Hecarim =
        {
            "Ravenous Hydra", "Trinity Force",
            "Ninja Tabi", "Frozen Mallet", "Blade of the Ruined King", "Statikk Shiv"
        };

        public static string[] Heimerdinger =
        {
            "Rod of Ages", "Morellonomicon",
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
        
        public static string[] Jhin =
        {
            "Essence Reaver", "Rapid Firecannon",
            "Boots of Swiftness", "Infinity Edge", "Mercurial Scimitar", "Lord Dominik's Regards"
        };

        public static string[] Jinx =
        {
            "Infinity Edge", "Statikk Shiv",
            "Berserker's Greaves", "Essence Reaver", "Mercurial Scimitar", "Lord Dominik's Regards"
        };

        public static string[] Kalista =
        {
            "The Black Spear", "Runaan's Hurricane", "The Bloodthirster", "Blade of the Ruined King",
            "Berserker's Greaves", "Infinity Edge"
        };

        public static string[] Karma =
        {
            "Morellonomicon", "Zhonya's Hourglass",
            "Sorcerer's Shoes", "Rabadon's Deathcap", "Rylai's Crystal Scepter", "Luden's Echo"
        };

        public static string[] Karthus =
        {
            "Rod of Ages", "Morellonomicon",
            "Sorcerer's Shoes", "Void Staff", "Rabadon's Deathcap", "Zhonya's Hourglass"
        };

        public static string[] Kassadin =
        {
            "Morellonomicon", "Zhonya's Hourglass",
            "Sorcerer's Shoes", "Rod of Ages", "Abyssal Scepter", "Rabadon's Deathcap"
        };

        public static string[] Katarina =
        {
            "Liandry's Torment", "Void Staff", "Rabadon's Deathcap",
            "Sorcerer's Shoes", "Abyssal Scepter", "Luden's Echo"
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
            "Guinsoo's Rageblade", "Boots of Swiftness",
            "Runaan's Hurricane", "Mercurial Scimitar", "Blade of the Ruined King", "The Bloodthirster"
        };

        public static string[] Leblanc =
        {
            "Morellonomicon", "Abyssal Scepter",
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
            "Rod of Ages", "Morellonomicon",
            "Sorcerer's Shoes", "Void Staff", "Rabadon's Deathcap", "Liandry's Torment"
        };

        public static string[] Lucian =
        {
            "Infinity Edge", "Statikk Shiv", "Berserker's Greaves",
            "Mercurial Scimitar", "Essence Reaver", "Lord Dominik's Regards"
        };

        public static string[] Lulu =
        {
            "Athene's Unholy Grail", "Morellonomicon",
            "Sorcerer's Shoes", "Void Staff", "Rabadon's Deathcap", "Liandry's Torment"
        };

        public static string[] Lux =
        {
            "Luden's Echo", "Morellonomicon",
            "Sorcerer's Shoes", "Void Staff", "Rabadon's Deathcap", "Zhonya's Hourglass"
        };

        public static string[] Malphite =
        {
            "Sunfire Cape", "Banshee's Veil",
            "Ninja Tabi", "Spirit Visage", "Randuin's Omen", "Warmog's Armor"
        };

        public static string[] Malzahar =
        {
            "Morellonomicon", "Banshee's Veil",
            "Sorcerer's Shoes", "Void Staff", "Rabadon's Deathcap", "Liandry's Torment"
        };

        public static string[] Maokai =
        {
            "Sunfire Cape", "Spirit Visage",
            "Boots of Mobility", "Rod of Ages", "Randuin's Omen", "Banshee's Veil"
        };

        public static string[] MasterYi =
        {
            "Infinity Edge", "Sunfire Cape",
            "The Bloodthirster", "The Black Cleaver", "Statikk Shiv", "Frozen Mallet"
        };

        public static string[] MissFortune =
        {
            "Infinity Edge", "Statikk Shiv", "Berserker's Greaves",
            "Essence Reaver", "Mercurial Scimitar", "Lord Dominik's Regards"
        };

        public static string[] Mordekaiser =
        {
            "Abyssal Scepter", "Rabadon's Deathcap",
            "Sorcerer's Shoes", "Spirit Visage", "Zhonya's Hourglass", "Liandry's Torment"
        };

        public static string[] Morgana =
        {
            "Athene's Unholy Grail", "Abyssal Scepter",
            "Sorcerer's Shoes", "Void Staff", "Liandry's Torment", "Rabadon's Deathcap",
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
            "Luden's Echo", "Morellonomicon",
            "Sorcerer's Shoes", "Rabadon's Deathcap", "Void Staff", "Rylai's Crystal Scepter"
        };

        public static string[] Nocturne =
        {
            "Ravenous Hydra", "Blade of the Ruined King",
            "Berserker's Greaves", "Youmuu's Ghostblade", "Statikk Shiv", "Frozen Mallet"
        };

        public static string[] Nunu =
        {
            "Ravenous Hydra", "Sunfire Cape",
            "Ninja Tabi", "Rod of Ages", "Abyssal Scepter", "Rylai's Crystal Scepter"
        };

        public static string[] Olaf =
        {
            "Ravenous Hydra", "Blade of the Ruined King",
            "Mercury's Treads", "Frozen Heart", "Frozen Mallet", "Warmog's Armor"
        };

        public static string[] Orianna =
        {
            "Rod of Ages", "Liandry's Torment",
            "Mercury's Treads", "Void Staff", "Rylai's Crystal Scepter", "Rabadon's Deathcap"
        };

        public static string[] Pantheon =
        {
            "Youmuu's Ghostblade", "The Black Cleaver",
            "Mercury's Treads", "Sunfire Cape", "Warmog's Armor", "The Bloodthirster"
        };

        public static string[] Poppy =
        {
            "Trinity Force", "Banshee's Veil",
            "Ionian Boots of Lucidity", "Frozen Heart", "Sunfire Cape", "Void Staff",
        };

        public static string[] Quinn =
        {
            "Youmuu's Ghostblade", "Berserker's Greaves",
            "Maw of Malmortius", "The Bloodthirster", "Statikk Shiv", "Sterak's Gage"
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
            "Void Staff", "Zhonya's Hourglass"
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
            "Infinity Edge", "Statikk Shiv",
            "Berserker's Greaves", "Essence Reaver", "Mercurial Scimitar", "Lord Dominik's Regards"
        };

        public static string[] Skarner =
        {
            "Trinity Force", "Blade of the Ruined King",
            "Mercury's Treads", "Randuin's Omen", "Banshee's Veil", "Sunfire Cape"
        };

        public static string[] Sona =
        {
            "Athene's Unholy Grail", "Ardent Censer",
            "Sorcerer's Shoes", "Rabadon's Deathcap", "Void Staff", "Luden's Echo"
        };

        public static string[] Soraka =
        {
            "Athene's Unholy Grail", "Ardent Censer",
            "Mercury's Treads", "Rabadon's Deathcap", "Void Staff", "Luden's Echo"
        };

        public static string[] Swain =
        {
            "Rod of Ages", "Zhonya's Hourglass",
            "Sorcerer's Shoes", "Morellonomicon", "Rabadon's Deathcap", "Liandry's Torment"
        };

        public static string[] Syndra =
        {
            "Morellonomicon", "Rod of Ages",
            "Sorcerer's Shoes", "Luden's Echo", "Rabadon's Deathcap", "Void Staff"
        };

        public static string[] TahmKench =
        {
            "Sunfire Cape", "Spirit Visage",
            "Mercury's Treads", "Warmog's Armor", "Randuin's Omen", "Liandry's Torment"
        };

        public static string[] Taliyah =
        {
            "Rod of Ages", "Rylai's Crystal Scepter",
            "Sorcerer's Shoes", "Zhonya's Hourglass", "Void Staff", "Rabadon's Deathcap"
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
            "Infinity Edge", "Berserker's Greaves",
            "Runaan's Hurricane", "Mercurial Scimitar", "Rapid Firecannon", "Lord Dominik's Regards"
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
            "Rod of Ages", "Morellonomicon",
            "Sorcerer's Shoes", "Rabadon's Deathcap", "Void Staff", "Luden's Echo"
        };

        public static string[] Twitch =
        {
            "Infinity Edge", "Berserker's Greaves",
            "Runaan's Hurricane", "Mercurial Scimitar", "Phantom Dancer", "Lord Dominik's Regards"
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
            "Manamune", "Youmuu's Ghostblade",
            "Ionian Boots of Lucidity", "Mercurial Scimitar", "Lord Dominik's Regards", "The Bloodthirster"
        };

        public static string[] Vayne =
        {
            "Blade of the Ruined King", "Phantom Dancer",
            "Berserker's Greaves", "Infinity Edge", "Maw of Malmortius", "The Bloodthirster"
        };

        public static string[] Veigar =
        {
            "Morellonomicon", "Luden's Echo",
            "Sorcerer's Shoes", "Void Staff", "Rabadon's Deathcap", "Zhonya's Hourglass"
        };

        public static string[] Velkoz =
        {
            "Morellonomicon", "Zhonya's Hourglass",
            "Sorcerer's Shoes", "Rabadon's Deathcap", "Rylai's Crystal Scepter", "Luden's Echo"
        };

        public static string[] Vi =
        {
            "The Black Cleaver", "Blade of the Ruined King",
            "Mercury's Treads", "Randuin's Omen", "Banshee's Veil", "Warmog's Armor"
        };

        public static string[] Viktor =
        {
            "Morellonomicon", "Zhonya's Hourglass",
            "Sorcerer's Shoes", "Perfect Hex Core", "Rabadon's Deathcap", "Void Staff"
        };

        public static string[] Vladimir =
        {
            "Warmog's Armor", "Rabadon's Deathcap",
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
            "Morellonomicon", "Liandry's Torment",
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
            "Luden's Echo", "Morellonomicon",
            "Sorcerer's Shoes", "Void Staff", "Rabadon's Deathcap", "Liandry's Torment"
        };

        public static string[] Zilean =
        {
            "Morellonomicon", "Mikael's Crucible",
            "Sorcerer's Shoes", "Rabadon's Deathcap", "Void Staff", "Luden's Echo"
        };

        public static string[] Zyra =
        {
            "Luden's Echo", "Morellonomicon",
            "Sorcerer's Shoes", "Rabadon's Deathcap", "Liandry's Torment", "Rylai's Crystal Scepter"
        };

        public static Queue<AramItem> Queue = new Queue<AramItem>();
        public static bool CanBuy = true;

        public static void Init()
        {
            string itemJson = "https://raw.githubusercontent.com/voidsurfer/hotmilkshakes/master/item.json";
            string itemsData = Request(itemJson);
            string itemArray = itemsData.Split(new[] {"data"}, StringSplitOptions.None)[1];
            MatchCollection itemIdArray = Regex.Matches(itemArray, "[\"]\\d*[\"][:][{].*?(?=},\"\\d)");
            foreach (AramItem item in from object iItem in itemIdArray select new AramItem(iItem.ToString()))
                ItemList.Add(item);
            Console.WriteLine("Auto Buy Activated");
            Loading.OnLoadingComplete += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            var name = ObjectManager.Player.ChampionName;
            if (name.Equals("Aatrox"))
                List = Aatrox;
            if (name.Equals("Ahri"))
                List = Ahri;
            if (name.Equals("Akali"))
                List = Akali;
            if (name.Equals("Alistar"))
                List = Alistar;
            if (name.Equals("Amumu"))
                List = Amumu;
            if (name.Equals("Anivia"))
                List = Anivia;
            if (name.Equals("Annie"))
                List = Annie;
            if (name.Equals("Ashe"))
                List = Ashe;
            if (name.Equals("Azir"))
                List = Azir;
            if (name.Equals("Bard"))
                List = Bard;
            if (name.Equals("Blitzcrank"))
                List = Blitzcrank;
            if (name.Equals("Brand"))
                List = Brand;
            if (name.Equals("Braum"))
                List = Braum;
            if (name.Equals("Caitlyn"))
                List = Caitlyn;
            if (name.Equals("Cassiopeia"))
                List = Cassiopeia;
            if (name.Equals("Chogath"))
                List = Chogath;
            if (name.Equals("Corki"))
                List = Corki;
            if (name.Equals("Darius"))
                List = Darius;
            if (name.Equals("Diana"))
                List = Diana;
            if (name.Equals("DrMundo"))
                List = DrMundo;
            if (name.Equals("Draven"))
                List = Draven;
            if (name.Equals("Ekko"))
                List = Ekko;
            if (name.Equals("Elise"))
                List = Elise;
            if (name.Equals("Evelynn"))
                List = Evelynn;
            if (name.Equals("Ezreal"))
                List = Ezreal;
            if (name.Equals("FiddleSticks"))
                List = FiddleSticks;
            if (name.Equals("Fiora"))
                List = Fiora;
            if (name.Equals("Fizz"))
                List = Fizz;
            if (name.Equals("Galio"))
                List = Galio;
            if (name.Equals("Gangplank"))
                List = Gangplank;
            if (name.Equals("Garen"))
                List = Garen;
            if (name.Equals("Gnar"))
                List = Gnar;
            if (name.Equals("Gragas"))
                List = Gragas;
            if (name.Equals("Graves"))
                List = Graves;
            if (name.Equals("Hecarim"))
                List = Hecarim;
            if (name.Equals("Heimerdinger"))
                List = Heimerdinger;
            if (name.Equals("Irelia"))
                List = Irelia;
            if (name.Equals("Janna"))
                List = Janna;
            if (name.Equals("JarvanIV"))
                List = JarvanIV;
            if (name.Equals("Jax"))
                List = Jax;
            if (name.Equals("Jayce"))
                List = Jayce;
            if (name.Equals("Jhin"))
                List = Jhin;
            if (name.Equals("Jinx"))
                List = Jinx;
            if (name.Equals("Kalista"))
                List = Kalista;
            if (name.Equals("Karma"))
                List = Karma;
            if (name.Equals("Karthus"))
                List = Karthus;
            if (name.Equals("Kassadin"))
                List = Kassadin;
            if (name.Equals("Katarina"))
                List = Katarina;
            if (name.Equals("Kayle"))
                List = Kayle;
            if (name.Equals("Kennen"))
                List = Kennen;
            if (name.Equals("Khazix"))
                List = Khazix;
            if (name.Equals("KogMaw"))
                List = KogMaw;
            if (name.Equals("Leblanc"))
                List = Leblanc;
            if (name.Equals("LeeSin"))
                List = LeeSin;
            if (name.Equals("Leona"))
                List = Leona;
            if (name.Equals("Lissandra"))
                List = Lissandra;
            if (name.Equals("Lucian"))
                List = Lucian;
            if (name.Equals("Lulu"))
                List = Lulu;
            if (name.Equals("Lux"))
                List = Lux;
            if (name.Equals("Malphite"))
                List = Malphite;
            if (name.Equals("Malzahar"))
                List = Malzahar;
            if (name.Equals("Maokai"))
                List = Maokai;
            if (name.Equals("MasterYi"))
                List = MasterYi;
            if (name.Equals("MissFortune"))
                List = MissFortune;
            if (name.Equals("Mordekaiser"))
                List = Mordekaiser;
            if (name.Equals("Morgana"))
                List = Morgana;
            if (name.Equals("Nami"))
                List = Nami;
            if (name.Equals("Nasus"))
                List = Nasus;
            if (name.Equals("Nautilus"))
                List = Nautilus;
            if (name.Equals("Nidalee"))
                List = Nidalee;
            if (name.Equals("Nocturne"))
                List = Nocturne;
            if (name.Equals("Nunu"))
                List = Nunu;
            if (name.Equals("Olaf"))
                List = Olaf;
            if (name.Equals("Orianna"))
                List = Orianna;
            if (name.Equals("Pantheon"))
                List = Pantheon;
            if (name.Equals("Poppy"))
                List = Poppy;
            if (name.Equals("Quinn"))
                List = Quinn;
            if (name.Equals("Rammus"))
                List = Rammus;
            if (name.Equals("RekSai"))
                List = RekSai;
            if (name.Equals("Renekton"))
                List = Renekton;
            if (name.Equals("Rengar"))
                List = Rengar;
            if (name.Equals("Riven"))
                List = Riven;
            if (name.Equals("Rumble"))
                List = Rumble;
            if (name.Equals("Ryze"))
                List = Ryze;
            if (name.Equals("Sejuani"))
                List = Sejuani;
            if (name.Equals("Shaco"))
                List = Shaco;
            if (name.Equals("Shen"))
                List = Shen;
            if (name.Equals("Shyvana"))
                List = Shyvana;
            if (name.Equals("Singed"))
                List = Singed;
            if (name.Equals("Sion"))
                List = Sion;
            if (name.Equals("Sivir"))
                List = Sivir;
            if (name.Equals("Skarner"))
                List = Skarner;
            if (name.Equals("Sona"))
                List = Sona;
            if (name.Equals("Soraka"))
                List = Soraka;
            if (name.Equals("Swain"))
                List = Swain;
            if (name.Equals("Syndra"))
                List = Syndra;
            if (name.Equals("TahmKench"))
                List = TahmKench;
            if (name.Equals("Taliyah"))
                List = Taliyah;
            if (name.Equals("Talon"))
                List = Talon;
            if (name.Equals("Taric"))
                List = Taric;
            if (name.Equals("Teemo"))
                List = Teemo;
            if (name.Equals("Thresh"))
                List = Thresh;
            if (name.Equals("Tristana"))
                List = Tristana;
            if (name.Equals("Trundle"))
                List = Trundle;
            if (name.Equals("Tryndamere"))
                List = Tryndamere;
            if (name.Equals("TwistedFate"))
                List = TwistedFate;
            if (name.Equals("Twitch"))
                List = Twitch;
            if (name.Equals("Udyr"))
                List = Udyr;
            if (name.Equals("Urgot"))
                List = Urgot;
            if (name.Equals("Varus"))
                List = Varus;
            if (name.Equals("Vayne"))
                List = Vayne;
            if (name.Equals("Veigar"))
                List = Veigar;
            if (name.Equals("Velkoz"))
                List = Velkoz;
            if (name.Equals("Vi"))
                List = Vi;
            if (name.Equals("Viktor"))
                List = Viktor;
            if (name.Equals("Vladimir"))
                List = Vladimir;
            if (name.Equals("Volibear"))
                List = Volibear;
            if (name.Equals("Warwick"))
                List = Warwick;
            if (name.Equals("MonkeyKing"))
                List = MonkeyKing;
            if (name.Equals("Xerath"))
                List = Xerath;
            if (name.Equals("XinZhao"))
                List = XinZhao;
            if (name.Equals("Yasuo"))
                List = Yasuo;
            if (name.Equals("Yorick"))
                List = Yorick;
            if (name.Equals("Zac"))
                List = Zac;
            if (name.Equals("Zed"))
                List = Zed;
            if (name.Equals("Ziggs"))
                List = Ziggs;
            if (name.Equals("Zilean"))
                List = Zilean;
            if (name.Equals("Zyra"))
                List = Zyra;
            //Cuz while this doesn't have all assemblies will just go for ad
            //List = Sivir; 
            Queue = ShoppingQueue();
            AlterInventory();
            /*
            if (Program.Config.Item("autosharp.shop").GetValue<bool>())
            {
                Game.PrintChat("[{0}] Autobuy Loaded", ObjectManager.Player.ChampionName);
            }
            else
            {
                Game.PrintChat("Autobuy has been disabled in the menu.");
            }
             */
            Game.OnUpdate += BuyItems;
        }

        public static Queue<AramItem> ShoppingQueue()
        {
            var shoppingItems = new Queue<AramItem>();
            foreach (string indexItem in List)
            {
                var macroItems = new Queue<AramItem>();
                ItemSequence(GetItemByName(indexItem), macroItems);
                foreach (AramItem secondIndexItem in macroItems)
                    shoppingItems.Enqueue(secondIndexItem);
            }
            return shoppingItems;
        }

        public static void BuyItems(EventArgs args)
        {
            //if (!Program.Config.Item("autosharp.shop").GetValue<bool>()) return;
            if ((ObjectManager.Player.InFountain() || ObjectManager.Player.IsDead) && Environment.TickCount - _lastShop < 300) return;
            if ((Queue.Peek() != null && InventoryFull()) &&
                   (Queue.Peek().From == null ||
                    (Queue.Peek().From != null && !Queue.Peek().From.Contains(_lastItem.Id))))
            {
                var y = Queue.Dequeue();
                _priceAddup += y.Goldbase;
            }
            if (Queue.Peek().Goldbase <= ObjectManager.Player.Gold - _priceAddup && Queue.Count > 0 &&
                   ObjectManager.Player.IsInShopRange())
            {
                var y = Queue.Dequeue();
                //y.Id
                (new Item((ItemId) y.Id)).Buy();
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
                var x = GetItemById((int) z[i]);
                Queue<AramItem> temp = new Queue<AramItem>();
                ItemSequence(x, temp);
                y += temp.Count;
            }
            for (int i = 0; i < y; i++)
                Console.WriteLine(Queue.Dequeue());
        }
    }
}
