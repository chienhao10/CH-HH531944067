using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ARAMDetFull.Champions;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using UnderratedAIO.Helpers;
using System.Diagnostics;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;
using EloBuddy;

namespace ARAMDetFull
{
    class ARAMSimulator
    {
        public enum ARAMPlayState
        {
            Shopping,
            Defending,
            Pushing
        }

        public enum ChampType
        {
            Mage,
            Tank,
            Support,
            Carry,
            Bruiser,
            MageTank,
            MageNoMana,
            TankAS,
            MageAS,
            DpsAS
        }

        internal class ItemToShop
        {
            public List<int> itemIds;
            public List<int> itemsMustHave;
            public List<int> sellItems;
            public bool last = false;

            public int goldReach
            {
                get
                {

                    return (last) ? 99999999 : itemIds.Sum(ids => Items.GetItem(ids).GetPriceToBuy());
                }
            }
        }

        public static string[] mages =
        {
            "Annie", "Ahri", "Anivia", "Annie", "Brand", "Cassiopeia", "Diana",  "FiddleSticks",
             "Gragas", "Heimerdinger", "Karthus",
            "Kassadin",  "Leblanc", "Lissandra", "Lux", "Malzahar",
            "Morgana", "Orianna",   "Swain", "Syndra",  "TwistedFate", "Veigar", "Viktor",
            "Xerath", "Ziggs", "Zyra", "Vel'Koz", "Chogath", "Malphite", "Maokai","Shaco"
        };

        public static string[] supports =
        {
            "Alistar", "Blitzcrank", "Janna", "Karma", "Nami", "Sona", "Soraka", "Taric",
            "Thresh", "Zilean","Lulu",
        };

        public static string[] tanks =
        {
            "Amumu", "DrMundo","Sion", "Galio", "Hecarim", "Rammus", "Sejuani",
            "Shen", "Singed", "Skarner", "Volibear", "Leona",
             "Yorick", "Zac", "Udyr", "Nasus", "Trundle", "Irelia","Braum", "Vi"
        };

        public static string[] ad_carries =
        {
            "Ashe", "Caitlyn", "Corki", "Draven", "Ezreal", "Graves",  "KogMaw", "MissFortune", 
              "Sivir", "Jinx","Jayce", "Gangplank",
            "Talon", "Tristana", "Twitch", "Urgot", "Varus",  "Zed", "Lucian","Yasuo","MasterYi","Quinn","Kalista","Vayne"

        };

        public static string[] bruisers =
        {
            "Aatrox", "Darius",  "Fiora", "Garen", "JarvanIV", "Jax", "Khazix", "LeeSin",
            "Nautilus", "Nocturne", "Olaf", "Poppy",
            "Renekton", "Rengar", "Riven", "Shyvana", "Tryndamere", "MonkeyKing", "XinZhao","Pantheon",
            "Rek'Sai","Gnar","Wukong","RekSai"
        };

        public static string[] mageNoMana =
        {
            "Akali", "Katarina", "Vladimir", "Rumble","Mordekaiser", "Kennen"
        };

        public static string[] mageTank =
        {
            "Gragas","Galio","Bard","Singed","Nunu","Evelynn","Elise","Ryze","Ekko","Fizz", "Nidalee"
        };

        public static string[] mageAS =
        {
            "Azir","Kayle","Teemo",
        };

        public static string[] tankAS =
        {
            "Warwick",
        };

        public static string[] dpsAS =
        {
            
        };
        public static AIHeroClient player = ObjectManager.Player;

        public static GameObject fromNex = null;

        public static GameObject toNex = null;


        public static List<Sector> sectors = new List<Sector>();

        public static List<ItemToShop> defBuyThings;


        public static Build champBuild;

        public static ItemToShop nextItem;
        public static int lastBuy = 0;
        public static SummonerSpells sSpells ;
        public static int tankBal = -20;

        public static ChampType getType()
        {
            var cName = player.ChampionName;
            if(mages.Contains(cName))
                return ChampType.Mage;
            if (supports.Contains(cName))
                return ChampType.Support;
            if (tanks.Contains(cName))
                return ChampType.Tank;
            if (ad_carries.Contains(cName))
                return ChampType.Carry;
            if (bruisers.Contains(cName))
                return ChampType.Bruiser;
            if (mageNoMana.Contains(cName))
                return ChampType.MageNoMana;
            if (mageTank.Contains(cName))
                return ChampType.MageTank;
            if (mageAS.Contains(cName))
                return ChampType.MageAS;
            if (tankAS.Contains(cName))
                return ChampType.TankAS;
            if (dpsAS.Contains(cName))
                return ChampType.DpsAS;
            return ChampType.Tank;
        }

        public static void setUpItems()
        {

            /*champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                {
                    new ConditionalItem(ItemId.Boots_of_Swiftness),
                    new ConditionalItem(ItemId.Banshees_Veil,ItemId.Sunfire_Cape,ItemCondition.ENEMY_AP),
                    new ConditionalItem(ItemId.Last_Whisper),
                    new ConditionalItem(ItemId.Statikk_Shiv),
                    new ConditionalItem(ItemId.Maw_of_Malmortius),
                    new ConditionalItem(ItemId.Frozen_Mallet),
                },
                startingItems = new List<ItemId>
                {
                    ItemId.Boots_of_Speed,ItemId.Brawlers_Gloves
                }
            };*/
            if (getType() == ChampType.Support || getType() == ChampType.Mage)
            {
                tankBal = 15;

                champBuild = new Build
                {
                    coreItems = new List<ConditionalItem>
                    {
                        new ConditionalItem(ItemId.Athenes_Unholy_Grail),
                        new ConditionalItem(ItemId.Sorcerers_Shoes),
                        new ConditionalItem(ItemId.Rabadons_Deathcap),
                        new ConditionalItem(ItemId.Abyssal_Scepter,ItemId.Zhonyas_Hourglass,ItemCondition.ENEMY_AP),
                        new ConditionalItem(ItemId.Void_Staff,ItemId.Liandrys_Torment, ItemCondition.ENEMY_MR),
                        new ConditionalItem(ItemId.Rylais_Crystal_Scepter),
                    },
                        startingItems = new List<ItemId>
                    {
                        ItemId.Needlessly_Large_Rod
                    }
                };

                #region itemsToBuyList
            }
            else if (getType() == ChampType.TankAS)
            {
                champBuild = new Build
                {
                    coreItems = new List<ConditionalItem>
                    {
                        new ConditionalItem(ItemId.Mercurys_Treads),
                        new ConditionalItem(ItemId.Frozen_Mallet),
                        new ConditionalItem(ItemId.Wits_End),
                        new ConditionalItem(ItemId.Blade_of_the_Ruined_King),
                        new ConditionalItem(ItemId.Banshees_Veil,ItemId.Sunfire_Cape, ItemCondition.ENEMY_MR),
                        new ConditionalItem(ItemId.Trinity_Force),
                    },
                    startingItems = new List<ItemId>
                    {
                        ItemId.Boots_of_Speed,(ItemId)3751
                    }
                };
            }
            else if (getType() == ChampType.DpsAS)
            {
                champBuild = new Build
                {
                    coreItems = new List<ConditionalItem>
                    {
                        new ConditionalItem(ItemId.Boots_of_Speed),
                        new ConditionalItem(ItemId.Statikk_Shiv),
                        new ConditionalItem(ItemId.The_Bloodthirster),
                        new ConditionalItem(ItemId.Phantom_Dancer),
                        new ConditionalItem(ItemId.Infinity_Edge),
                        new ConditionalItem(ItemId.Banshees_Veil,ItemId.Thornmail, ItemCondition.ENEMY_MR),
                    },
                    startingItems = new List<ItemId>
                    {
                        ItemId.Zeal,
                    }
                };
            }
            else if (getType() == ChampType.MageAS)
            {
                champBuild = new Build
                {
                    coreItems = new List<ConditionalItem>
                    {
                        new ConditionalItem(ItemId.Sorcerers_Shoes),
                        new ConditionalItem(ItemId.Nashors_Tooth),
                        new ConditionalItem(ItemId.Rabadons_Deathcap),
                        new ConditionalItem(ItemId.Ludens_Echo),
                        new ConditionalItem(ItemId.Guinsoos_Rageblade),
                        new ConditionalItem(ItemId.Hextech_Gunblade),
                    },
                    startingItems = new List<ItemId>
                    {
                        ItemId.Stinger,
                    }
                };
                
            }
            else if (getType() == ChampType.MageNoMana)
            {
                tankBal = 0;

                champBuild = new Build
                {
                    coreItems = new List<ConditionalItem>
                    {
                        new ConditionalItem(ItemId.Ludens_Echo),
                        new ConditionalItem(ItemId.Sorcerers_Shoes),
                        new ConditionalItem(ItemId.Abyssal_Scepter,ItemId.Zhonyas_Hourglass,ItemCondition.ENEMY_AP),
                        new ConditionalItem(ItemId.Rabadons_Deathcap),
                        new ConditionalItem(ItemId.Void_Staff,ItemId.Liandrys_Torment, ItemCondition.ENEMY_MR),
                        new ConditionalItem(ItemId.Rylais_Crystal_Scepter),
                    },
                    startingItems = new List<ItemId>
                    {
                        ItemId.Needlessly_Large_Rod
                    }
                };
            }
            else if (getType() == ChampType.MageTank)
            {
                tankBal = -20;
                champBuild = new Build
                {
                    coreItems = new List<ConditionalItem>
                    {
                        new ConditionalItem(ItemId.Rod_of_Ages),
                        new ConditionalItem(ItemId.Sorcerers_Shoes),
                        new ConditionalItem(ItemId.Abyssal_Scepter,ItemId.Zhonyas_Hourglass,ItemCondition.ENEMY_MR),
                        new ConditionalItem(ItemId.Rylais_Crystal_Scepter),
                        new ConditionalItem(ItemId.Rabadons_Deathcap),
                        new ConditionalItem(ItemId.Liandrys_Torment),
                    },
                    startingItems = new List<ItemId>
                    {
                        ItemId.Catalyst_the_Protector,
                    }
                };
            }
            else if (getType() == ChampType.Tank)
            {
                tankBal = -40;

                champBuild = new Build
                {
                    coreItems = new List<ConditionalItem>
                    {
                        new ConditionalItem(ItemId.Mercurys_Treads),
                        new ConditionalItem(ItemId.Sunfire_Cape),
                        new ConditionalItem(ItemId.Banshees_Veil,ItemId.Thornmail,ItemCondition.ENEMY_MR),
                        new ConditionalItem(ItemId.Frozen_Mallet),
                        new ConditionalItem(ItemId.Maw_of_Malmortius,ItemId.Iceborn_Gauntlet,ItemCondition.ENEMY_MR),
                        new ConditionalItem(ItemId.Warmogs_Armor),
                    },
                    startingItems = new List<ItemId>
                    {
                        ItemId.Phage,
                    }
                };
            }
                #endregion

            else if (getType() == ChampType.Bruiser) //Bruiser
            {
                tankBal = -25;

                champBuild = new Build
                {
                    coreItems = new List<ConditionalItem>
                    {
                        new ConditionalItem(ItemId.Mercurys_Treads),
                        new ConditionalItem(ItemId.Spirit_Visage,ItemId.Sunfire_Cape,ItemCondition.ENEMY_AP),
                        new ConditionalItem(ItemId.Trinity_Force),
                        new ConditionalItem(ItemId.Banshees_Veil,ItemId.Thornmail,ItemCondition.ENEMY_AP),
                        new ConditionalItem(ItemId.Blade_of_the_Ruined_King),
                        new ConditionalItem(ItemId.Maw_of_Malmortius,ItemId.Randuins_Omen,ItemCondition.ENEMY_AP),
                    },
                    startingItems = new List<ItemId>
                    {
                        ItemId.Boots_of_Speed,ItemId.Ruby_Crystal,ItemId.Long_Sword,
                    }
                };
                
            }
            else //adc
            {
                tankBal = 15;

                champBuild = new Build
                {
                    coreItems = new List<ConditionalItem>
                    {
                        new ConditionalItem(ItemId.The_Bloodthirster),
                        new ConditionalItem(ItemId.Berserkers_Greaves),
                        new ConditionalItem(ItemId.Infinity_Edge),
                        new ConditionalItem(ItemId.Blade_of_the_Ruined_King),
                        new ConditionalItem(ItemId.Last_Whisper),
                        new ConditionalItem((ItemId)3046),
                       // new ConditionalItem(ItemId.Banshees_Veil,ItemId.Thornmail,ItemCondition.ENEMY_MR),
                    },
                    startingItems = new List<ItemId>
                    {
                        ItemId.Boots_of_Speed,ItemId.Vampiric_Scepter,
                    }
                };
                
            }
        }

        public static Champions.Champion champ = null;


        public static void setChamp()
        {
           // Chat.Print("Support DeTuKs on his new adventures on LeagueSmurfs.com");
            Chat.Print(player.ChampionName);
            switch (player.ChampionName)
            {
                case "Cassiopeia":
                    champ = new Cassiopeia();
                    break;
                case "Pantheon":
                    champ = new Pantheon();
                    break;
                case "Udyr":
                    champ = new Udyr();
                    break;
                case "Quinn":
                    champ = new Quinn();
                    break;
                case "Nami":
                    champ = new Nami();
                    break;
                case "Malphite":
                    champ = new Malphite();
                    break;
                case "Urgot":
                    champ = new Urgot();
                    break;
                case "Zyra":
                    champ = new Zyra();
                    break;
                case "Blitzcrank":
                    champ = new Blitzcrank();
                    break;
                case "Thresh":
                    champ = new Thresh();
                    break;
                case "Karthus":
                    champ = new Karthus();
                    break;
                case "Ezreal":
                    champ = new Ezreal();
                    break;
                case "Vayne":
                    champ = new Vayne();
                    break;
                case "Kassadin":
                    champ = new Kassadin();
                    break;
                case "Zed":
                    champ = new ZedA();
                    break;
                case "Maokai":
                    champ = new Maokai();
                    break;
                case "Sejuani":
                    champ = new Sejuani();
                    break;
                case "Darius":
                    champ = new Darius();
                    break;
                case "RekSai":
                   champ = new RekSai2();
                    break;
                case "Varus":
                    champ = new Varus();
                    break;
                case "Draven":
                    champ = new Draven();
                    break;
                case "Katarina":
                    champ = new Katarina();
                    break;
                case "KogMaw":
                    champ = new Kogmaw();
                    break;
                case "Azir":
                    champ = new Azir();
                    break;
                case "Shaco":
                    champ = new Shaco();
                    break;
                case "Singed":
                    champ = new Singed();
                    break;
                case "Twitch":
                    champ = new Twitch();
                    break;
                case "FiddleSticks":
                    champ = new Fiddlestick();
                    break;
                case "Viktor":
                    champ = new ViktorA();
                    break;
                case "Leona":
                    champ = new Leona();
                    break;
                case "Taric":
                    champ = new Taric();
                    break;
                case "Kalista":
                    champ = new Kalista();
                    break;
                case "Brand":
                    champ = new Brand();
                    break;
                case "Jinx":
                    champ = new Jinx();
                    break;
                case "Nautilus":
                    champ = new Nautilus();
                    break;
                case "Gragas":
                    champ = new Gragas();
                    break;
                case "Aatrox":
                    champ = new Aatrox();
                    break;
                case "Wukong":
                    champ = new WukongA();
                    break;
                case "Syndra":
                    champ = new Syndra();
                    break;
                case "Jayce":
                    champ = new JayceA();
                    break;
                case "Ziggs":
                    champ = new ZiggsA();
                    break;
                case "Zilean":
                    champ = new Zilean();
                    break;
                case "XinZhao":
                    champ = new XinZhaoA();
                    break;
                case "Talon":
                    champ = new Talon();
                    break;
                case "Xerath":
                    champ = new Xerath();
                    break;
                case "Ahri":
                    champ = new Ahri();
                    break;
                case "Bard":
                    champ = new Brad();
                    break;
                case "Lucian":
                    champ = new Lucian();
                    break;
                case "Vi":
                    champ = new Vi();
                    break;
                case "Hecarim":
                    champ = new Hecarim();
                    break;
                case "Mordekaiser":
                    champ = new MordekaiserA();
                    break;
                case "Garen":
                    champ = new Garen();
                    break;
                case "Anivia":
                    champ = new Anivia();
                    break;
                case "Nunu":
                    champ = new Nunu();
                    break;
                case "Evelynn":
                    champ = new EvelynnA();
                    break;
                case "Malzahar":
                    champ = new Malzahar();
                    break;
                case "Khazix":
                    champ = new Khazix();
                    break;
                case "Veigar":
                    champ = new Veigar();
                    break;
                case "Poppy":
                    champ = new Poppy();
                    break;
                case "Tristana":
                    champ = new Tristana();
                    break;
                case "Lulu":
                    champ = new Lulu();
                    break;
                case "Orianna":
                    champ = new Orianna();
                    break;
                case "Elise":
                    champ = new Elise();
                    break;
                case "Kayle":
                    champ = new Kayle();
                    break;
                case "Yasuo":
                    champ = new Yasuo();
                    break;
                case "Warwick":
                    champ = new WarwickA();
                    break;
                case "Karma":
                    champ = new KarmaA();
                    break;
                case "Soraka":
                    champ = new Soraka();
                    break;
                case "Ryze":
                    champ = new Ryze();
                    break;
                case "MasterYi":
                    champ = new MasterYi();
                    break;
                case "Annie":
                    champ = new Annie();
                    break;
                case "Ekko":
                    champ = new Ekko();
                    break;
                case "Olaf":
                    champ = new Olaf();
                    break;
                case "Diana":
                    champ = new Diana();
                    break;
                case "TahmKench":
                    champ = new Tahmkench();
                    break;
                case "TwistedFate":
                    champ = new TwistedFateA();
                    break;
                case "Heimerdinger":
                    champ = new Herimerdinger();
                    break;
                case "MissFortune":
                    champ = new MissFortune();
                    break;
                case "Galio":
                    champ = new Galio();
                    break;
                case "Sion":
                    champ = new SionA();
                    break;
                case "Shen":
                    champ = new Shen();
                    break;
                case "Gangplank":
                    champ = new Gangplank();
                    break;
                case "Fizz":
                    champ = new Fizz();
                    break;
                case "Volibear":
                    champ = new Volibear();
                    break;
                case "Graves":
                    champ = new Graves();
                    break;
                case "JarvanIV":
                    champ = new Jarvan();
                    break;
                case "LeeSin":
                    champ = new LeeSin();
                    break;
                case "Nocturne":
                    champ = new Nocturne();
                    break;
                case "Vladimir":
                    champ = new Vladimir();
                    break;
                //Vladimir
            }
        }

        public static void checkItems()
        {

            for (int i = defBuyThings.Count - 1; i >= 0; i--)
            {
                if (hasAllItems(defBuyThings[i]))
                {
                    nextItem = defBuyThings[i];

                    return;
                }
            }
        }

        public static bool hasAllItems(ItemToShop its)
        {
            try
            {
                bool[] usedItems = new bool[15];
                int itemsMatch = 0;
                for (int j = 0; j < its.itemsMustHave.Count; j++)
                {
                    for (int i = 0; i < player.InventoryItems.Count(); i++)
                    {
                        if (usedItems[i])
                            continue;
                        if (its.itemsMustHave[j] == (int)player.InventoryItems[i].Id)
                        {
                            usedItems[i] = true;
                            itemsMatch++;
                            break;
                        }
                    }
                }
                return itemsMatch == its.itemsMustHave.Count;
            }
            catch (Exception)
            {
                return false;
            }
            
        }

        public static void setupARMASimulator()
        {
            GameObject.OnCreate += TowerAttackOnCreate;
            GameObject.OnDelete += onDelete;

            foreach (var tur in ObjectManager.Get<Obj_HQ>())
            {
                if (tur.Team == GameObjectTeam.Chaos && player.Team == GameObjectTeam.Chaos)
                    fromNex = tur;
                if (tur.Team == GameObjectTeam.Chaos && player.Team == GameObjectTeam.Order)
                    toNex = tur;

                if (tur.Team == GameObjectTeam.Order && player.Team == GameObjectTeam.Order)
                    fromNex = tur;

                if (tur.Team == GameObjectTeam.Order && player.Team == GameObjectTeam.Chaos)
                    toNex = tur;
            }

            if (fromNex == null)
                return;
            float sep = fromNex.Position.LSDistance(toNex.Position)/40;

            Vector2 lastPos = fromNex.Position.LSTo2D();
            //Setup sectors
            for (int i = 0; i < 40; i++)
            {
                Vector2 end = lastPos.LSExtend(toNex.Position.LSTo2D(), sep);
                sectors.Add(new Sector(lastPos,end,750));
                lastPos = end;
            }
            MapControl.setupMapControl();
            AutoLevelChamp.setAutoLevel();
            LXOrbwalker.setpOrbwalker();
            setUpItems();
            setChamp();
            //checkItems();
            sSpells = new SummonerSpells();
            if (champ != null)
            {
                champ.setUpSpells();
            }
        }

        private static void onDelete(GameObject sender, EventArgs args)
        {
            if (MapControl.usedRelics.Contains(sender.NetworkId))
                MapControl.usedRelics.Remove(sender.NetworkId);
        }

        private static void TowerAttackOnCreate(GameObject sender, EventArgs args)
        {
            if (sender.IsValid<MissileClient>())
            {
                var missile = (MissileClient)sender;
                // Ally Turret -> Enemy Hero
                if (missile.SpellCaster.IsValid<Obj_AI_Turret>() && missile.SpellCaster.IsEnemy &&
                    missile.Target.IsValid<AIHeroClient>() && missile.Target.IsAlly)
                {
                    var turret = (Obj_AI_Turret)missile.SpellCaster;
                    if (missile.Target.IsMe)
                    {
                        towerAttackedMe = true;
                        towerAttackedAlly = false;
                    }
                    else if (((AIHeroClient) missile.Target).LSDistance(turret) < 700)
                    {
                        towerAttackedAlly = true;
                        towerAttackedMe = false;
                    }
                }
            }
        }

        public static void setRambo()
        {
            ramboMode = false;
            if (player.IsZombie && (player.ChampionName == "Sion" || player.ChampionName == "KogMaw") || ARAMTargetSelector.IsInvulnerable(player))
                ramboMode = true;
        }

        public static bool towerAttackedMe = false;
        public static bool towerAttackedAlly = false;

        public static bool haveSeenMinion = false;

        public static bool ramboMode = false;

        public static int balance = 0;
        public static int agrobalance = 0;
        public static Vector2 awayTo;

        public static AIHeroClient deepestAlly = player;

        const UInt32 WM_KEYDOWN = 0x0100;
        const UInt32 WM_KEYUP = 0x0101;
        const int VK_F5 = 0x74;

        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);

        public static void updateArmaPlay()
        {

            if (!haveSeenMinion)
            {
                haveSeenMinion =
                    ObjectManager.Get<Obj_AI_Minion>().Any(min => min.IsTargetable && min.IsAlly && min.Health > 50) ||
                    ARAMDetFull.gameStart + 54*1000 < ARAMDetFull.now;
            }
            if (!haveSeenMinion)
                return;
            try
            {
                AutoLevelChamp.LevelUpOff();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            if (champ != null)
            {
                champ.alwaysCheck();
            }

            if(player.IsDead)
                return;
            setRambo();

            var closestEnemy = HeroManager.Enemies.Where(ene => !ene.IsDead && ene.IsTargetable && !ARAMTargetSelector.IsInvulnerable(ene)).OrderBy(ene =>  player.Position.LSDistance(ene.Position, true)).FirstOrDefault();
            if (closestEnemy != null && ramboMode)
            {
                LXOrbwalker.OrbwalkTo(closestEnemy.Position,false,true);
                return;
            }
            agrobalance = Aggresivity.getAgroBalance();

            balance = (ARAMTargetSelector.IsInvulnerable(player) || player.IsZombie) ? 250 : MapControl.balanceAroundPointAdvanced(player.Position.LSTo2D(), 1800 - agrobalance * 5) - tankBal + agrobalance;
            LXOrbwalker.inDanger = balance < 0;

            if (champ != null)
            {
                try
                {
                    if (player.GetEnemiesInRange(LXOrbwalker.farmRange).Count(ene => !ene.IsDead && !ene.IsZombie) != 0)
                        champ.killSteal();
                    else
                        champ.farm();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

           

            if (!Sector.inTowerRange(player.Position.LSTo2D()) || towerAttackedAlly || player.HealthPercent < 25)
            {
                try
                {
                    ItemHandler.useItems();
                    sSpells.useSumoners();
                    if (champ != null)
                    {
                        champ.useSpells();
                    }
                    else
                    {
                        MapControl.myControler.useSkillshots();
                        MapControl.myControler.useNonSkillshots();

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            deepestAlly = HeroManager.Allies.OrderBy(al => toNex.Position.LSDistance(al.Position, true)).FirstOrDefault();
            var lookRange = player.AttackRange + ((player.IsMelee) ? 160 : 35);
            var easyKill =
               HeroManager.Enemies.FirstOrDefault(ene => !ene.IsDead && ene.LSDistance(player, true) < lookRange * lookRange &&
                                                         !ARAMTargetSelector.IsInvulnerable(ene) && ene.Health / 2.5 < player.LSGetAutoAttackDamage(ene));

            if (easyKill != null)
            {
                //Console.WriteLine("go get easy");
                LXOrbwalker.OrbwalkTo(easyKill.Position.LSTo2D().LSExtend(fromNex.Position.LSTo2D(), player.AttackRange*0.4f).To3D(),true);
            }

            if (balance < 0)
                LXOrbwalker.OrbwalkTo(player.Position.LSTo2D().LSExtend(fromNex.Position.LSTo2D(), 600).To3D(),false,true);
            if (moveToRelicIfForHeal())
            {
                return;
            }

            if (!player.UnderTurret(true))
            {
                towerAttackedMe = false;
                towerAttackedAlly = false;
            }

            if (towerAttackedMe)
            {
                LXOrbwalker.CustomOrbwalkMode = false;
               // Chat.Print("ouch tower!");
                LXOrbwalker.OrbwalkTo(player.Position.LSTo2D().LSExtend(fromNex.Position.LSTo2D(), 600).To3D(),true);
                return;
            }

            

            awayTo = eAwayFromTo();
            if (awayTo.IsValid() && awayTo.X != 0 )
            {

                LXOrbwalker.CustomOrbwalkMode = false;
                if(champ != null)
                    champ.kiteBack(awayTo);
                LXOrbwalker.OrbwalkTo(awayTo.To3D(),true);
                return;
                
            }
            else
            {
                var fightOn = MapControl.fightIsOn();

                if (fightOn != null && MapControl.balanceAroundPointAdvanced(fightOn.Position.LSTo2D(),800) > (-180) && fightOn.LSDistance(player, true) < 2500 * 2500 && (!player.IsMelee() || !Sector.inTowerRange(fightOn.Position.LSTo2D())))
                {
                    if (!Aggresivity.getIgnoreMinions())
                        Aggresivity.addAgresiveMove(new AgresiveMove(40,1500,true));
                    if (player.Position.LSDistance(fightOn.Position)>player.AttackRange*0.8f)
                        LXOrbwalker.OrbwalkTo(fightOn.Position.LSExtend(player.Position, player.AttackRange * ((LXOrbwalker.inDanger)?0.75f:0.3f)));
                    return;
                }
                else
                {/*
                    if (player.HealthPercent < 69 && moveToRelicIfForHeal())
                    {
                        return;
                    }*/

                    if (!LXOrbwalker.inDanger)
                    {

                        Sector orbSector = null;
                        Sector prevSector = null;
                        foreach (var sector in sectors)
                        {
                            sector.update();
                            if (sector.containsEnemyChamp && sector.enemyChampIn.LSDistance(player,true)<1350*1350)
                            {
                                orbSector = sector;
                                break;
                            }
                            if (sector.dangerPolig)
                                break;
                          //  if (!player.IsMelee)
                           // {
                                if (sector.containsEnemy && !sector.containsAlly)
                                    break;
                           // }
                           // else
                          //  {
                           //     if (prevSector != null && sector.containsEnemy && !prevSector.containsAlly  && !sector.containsAlly)
                           //         break;
                          //  }
                            orbSector = sector;
                            if (sector.containsEnemy && sector.containsAlly)
                                break;
                            prevSector = sector;
                        }
                        if (orbSector == null)
                            return;
                        LXOrbwalker.OrbwalkTo(orbSector.getRandomPointIn().To3D());
                    }
                    else
                    {
                        LXOrbwalker.OrbwalkTo(player.Position.LSTo2D().LSExtend(fromNex.Position.LSTo2D(),600).To3D());
                    }
                }
            }

           
            

            /*foreach (var ally in MapControl.ally_champions)
            {
                if (ally.hero.LSDistance(player) < 800 && MapControl.myControler != null)
                    MapControl.myControler.useNonSkillshots(ally.hero);
            }*/
        }

        public static bool moveToRelicIfForHeal()
        {
            var relicHeal = MapControl.ClosestRelic();
            if (relicHeal != null)
            {
                var dist = relicHeal.LSDistance(player);
                var bonus = ((50 - dist/20) > 0) ? (50 - dist/20) : 0;
                bool needHeal = player.HealthPercent + (float)agrobalance / 5 - (tankBal / 2.5) < 39 + bonus;
                if (dist < 100)
                    MapControl.usedRelics.Add(relicHeal.NetworkId);
                if (needHeal || (!relicHeal.Name.Contains("Health") && dist < 1600) )
                {
                    LXOrbwalker.OrbwalkTo(relicHeal.Position);
                    return true;
                }
            }
            return false;
        }

        public static bool enemIsOnMe(Obj_AI_Base target)
        {
            if (target.IsAlly || target.IsDead || !target.LSIsValidTarget())
                return false;

            float distTo = target.LSDistance(player, true);
            bool dangerousAround = (balance < -player.HealthPercent);
            float targetReack = (!dangerousAround)?player.AttackRange + 150:MapControl.getByObj(target).getReach();
            if (distTo > targetReack * targetReack)
               return false;

            var per = target.Direction.LSTo2D().LSPerpendicular();
            var dir = new Vector3(per, 0);
            var enemDir = target.Position + dir * 40;
            if (target.LSDistance(fromNex.Position, true) < enemDir.LSDistance(fromNex.Position, true))
                return false;

            return true;
        }

        public static Vector2 eAwayFromTo()
        {
            if(player.IsMelee())
                return new Vector2(0, 0);

            Vector2 backTo = player.Position.LSTo2D();
            int count = 0;

            backTo -= (toNex.Position - player.Position).LSTo2D();
            foreach (var enem in ObjectManager.Get<AIHeroClient>().Where(enemIsOnMe))
            {
                count++;
                backTo -= (enem.Position - player.Position).LSTo2D();
            }


            if (count > 0)
            {
                var awayTo = player.Position.LSTo2D().LSExtend(backTo, 700);
                if (!Sector.inTowerRange(awayTo))
                    return backTo;
            }
            return new Vector2(0,0);
        }

    }
}
