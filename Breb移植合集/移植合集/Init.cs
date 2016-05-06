#region

using System;
using System.Collections.Generic;
using EloBuddy;
using EloBuddy.SDK.Events;
using ExorAIO.Core;
using PortAIO.Utility;
using LeagueSharp.Common;
using SharpDX;
using PortAIO.Properties;
// ReSharper disable ObjectCreationAsStatement

#endregion

namespace PortAIO
{
    internal static class Init
    {
        private static void Main()
        {
            Loading.OnLoadingComplete += Initialize;
        }

        private static Render.Sprite Intro;
        private static float IntroTimer = Game.Time;
        public static SCommon.PluginBase.Champion Champion;
        public static List<string> RandomUltChampsList = new List<string>(new[] { "Ezreal", "Jinx", "Ashe", "Draven", "Gangplank", "Ziggs", "Lux", "Xerath" });
        public static List<string> BaseUltList = new List<string>(new[] { "Jinx", "Ashe", "Draven", "Ezreal", "Karthus" });

        private static System.Drawing.Bitmap LoadImg(string imgName)
        {
            var bitmap = Resources.ResourceManager.GetObject(imgName) as System.Drawing.Bitmap;
            if (bitmap == null)
            {
                Console.WriteLine(imgName + ".png not found.");
            }
            return bitmap;
        }

        private static void Initialize(EventArgs args)
        {
            Loader.Menu();

            if (Loader.intro)
            {
                Intro = new Render.Sprite(LoadImg("PortLogo"), new Vector2((Drawing.Width / 2) - 175, (Drawing.Height / 2) - 300));
                Intro.Add(0);
                Intro.OnDraw();
                LeagueSharp.Common.Utility.DelayAction.Add(5000, () => Intro.Remove());
            }

            if (!Loader.champOnly)
            {
                if (Loader.useActivator)
                {
                    ElUtilitySuite.Entry.OnLoad();
                }

                if (Loader.useRecall)
                {
                    UniversalRecallTracker.Program.Main();
                }

                if (Loader.useSkin)
                {
                    SkinsSharp.Program.GameLoad();
                }

                if (Loader.useTracker)
                {
                    NabbTracker.Program.Game_OnGameLoad();
                }

                if (Loader.godTracker)
                {
                    GodJungleTracker.Program.OnGameLoad();
                    Chat.Print("Berb : Depending on whether packets are updated or not will this work.");
                }

                if (Loader.ping)
                {
                    new UniversalPings.Program();
                }

                if (Loader.human)
                {
                    Humanizer.Program.Game_OnGameLoad();
                }

                if (Loader.gank)
                {
                    UniversalGankAlerter.Program.Main();
                }

                /*
                if (Loader.evade)
                {
                    new ezEvade.Evade();
                }

                if (Loader.stream)
                {
                    StreamBuddy.Program.Main();
                }

                if (RandomUltChampsList.Contains(ObjectManager.Player.ChampionName))
                {
                    if (Loader.randomUlt)
                    {
                        RandomUlt.Program.Game_OnGameLoad();
                    }
                }

                if (BaseUltList.Contains(ObjectManager.Player.ChampionName))
                {
                    if (Loader.baseUlt)
                    {
                        new BaseUlt3.BaseUlt();
                    }
                }
                */
            }

            if (!Loader.utilOnly)
            {
                switch (ObjectManager.Player.ChampionName.ToLower())
                {
                    case "aatrox": // BrianSharp's Aatrox
                        PortAIO.Champion.Aatrox.Program.Main();
                        break;
                    case "akali": // Akali by xQx
                        PortAIO.Champion.Akali.Program.Main();
                        break;
                    case "alistar": // El Alistar
                        PortAIO.Champion.Alistar.Program.OnGameLoad();
                        break;
                    case "amumu": // Shine#
                        PortAIO.Champion.Amumu.Program.OnLoad();
                        break;
                    case "caitlyn":
                        switch (Loader.cait)
                        {
                            case 0:
                                SebbyLib.Program.GameOnOnGameLoad();
                                break;
                            case 1:
                                Bootstrap.BuildMenu();
                                Bootstrap.LoadChampion();
                                break;
                            default:
                                SebbyLib.Program.GameOnOnGameLoad();
                                break;
                        }
                        break;
                    case "anivia": // OKTW - Sebby - All Seeby champs go down here
                    case "thresh":
                    case "annie":
                    case "ashe": // Or (Challenger Series Ashe)
                    case "braum":
                    case "ekko":
                    case "ezreal":
                    case "graves":
                    case "jayce":
                    case "jinx":
                    case "karthus":
                    case "missfortune":
                    case "malzahar":
                    case "orianna":
                    case "sivir":
                    case "twitch":
                    case "syndra":
                    case "velkoz":
                    case "xerath":
                    case "swain":
                    case "urgot":
                    case "ahri":
                        SebbyLib.Program.GameOnOnGameLoad();
                        break;
                    case "azir": // Synx Auto Carry
                        //Champion = new SAutoCarry.Champions.Azir();
                        HeavenStrikeAzir.Program.Game_OnGameLoad();
                        break;
                    case "bard": // Dreamless Wanderer
                        PortAIO.Champion.Bard.Program.OnLoad();
                        break;
                    case "blitzcrank": // Fresh Booster
                        PortAIO.Champion.Blitzcrank.Program.OnLoad();
                        break;
                    case "brand": // TheBrand (or OKTWBrand)
                        PortAIO.Champion.Brand.Program.Load();
                        break;
                    case "cassiopeia": // Synx Auto Carry
                        Champion = new SAutoCarry.Champions.Cassiopeia();
                        break;
                    case "chogath": // Underrated Cho'Gath
                        UnderratedAIO.Champions.Chogath.Load();
                        break;
                    case "corki": // ElCorki
                        ElCorki.Corki.Game_OnGameLoad();
                        break;
                    case "darius": // Exory
                    case "nautilus":
                    case "nunu":
                    case "olaf":
                    case "pantheon":
                    case "renekton":
                    case "tryndamere":
                    case "ryze":
                        Bootstrap.BuildMenu();
                        Bootstrap.LoadChampion();
                        break;
                    case "diana":
                        switch (Loader.diana)
                        {
                            case 0:
                                ElDiana.Diana.OnLoad();
                                break;
                            case 1:
                                Nechrito_Diana.Program.Game_OnGameLoad();
                                break;
                            default:
                                ElDiana.Diana.OnLoad();
                                break;
                        }
                        break;
                    case "drmundo": // Hestia's Mundo
                        Mundo.Mundo.OnLoad();
                        break;
                    case "draven": // UltimaDraven
                        RevampedDraven.Program.OnLoad();
                        break;
                    case "elise": // G-FUEL Elise
                        GFUELElise.Elise.OnGameLoad();
                        break;
                    case "evelynn": // Evelynn#
                        Evelynn.Program.Game_OnGameLoad();
                        break;
                    case "fiddlesticks": // Feedlesticks
                        Feedlesticks.Program.Game_OnGameLoad();
                        break;
                    case "fiora": // Underrated AIO
                        FioraProject.Program.Game_OnGameLoad();
                        break;
                    case "fizz": // Math Fizz
                        MathFizz.Program.Game_OnGameLoad();
                        break;
                    case "galio": // Underrated AIO
                        UnderratedAIO.Champions.Galio.OnLoad();
                        break;
                    case "gangplank": // Underrated AIO
                        UnderratedAIO.Champions.Gangplank.OnLoad();
                        break;
                    case "garen": // Underrated AIO
                        UnderratedAIO.Champions.Garen.OnLoad();
                        break;
                    case "gnar": // Slutty Gnar
                        Slutty_Gnar_Reworked.Gnar.OnLoad();
                        break;
                    case "gragas": // Gragas - Drunk Carry
                        GragasTheDrunkCarry.Gragas.Game_OnGameLoad();
                        break;
                    case "hecarim": // JustyHecarim
                        JustHecarim.Program.OnLoad();
                        break;
                    case "heimerdinger": // 2 Girls 1 Dong
                        Two_Girls_One_Donger.Program.Game_OnGameLoad();
                        break;
                    case "illaoi": // Tentacle Kitty
                        Illaoi___Tentacle_Kitty.Program.Game_OnGameLoad();
                        break;
                    case "irelia": // Challenger Series Irelia
                        Challenger_Series.Irelia.OnLoad();
                        break;
                    case "janna": // LCS Janna
                        LCS_Janna.Program.OnGameLoad();
                        break;
                    case "jarvaniv": // BrianSharp
                        BrianSharp.Plugin.JarvanIV.OnLoad();
                        break;
                    case "jax": // xqx
                        JaxQx.Program.Game_OnGameLoad();
                        break;
                    case "jhin": // Jhin The Virtuoso
                        Jhin___The_Virtuoso.Jhin.JhinOnLoad();
                        break;
                    case "kalista": // iKalista
                        switch (Loader.kalista)
                        {
                            case 0:
                                new IKalista.Kalista();
                                break;
                            case 1:
                                new iKalistaReborn.Kalista();
                                break;
                            case 2:
                                Challenger_Series.Program.Main();
                                break;
                            default:
                                new iKalistaReborn.Kalista();
                                break;
                        }
                        break;
                    case "karma": // Karma by Eskor
                        Karma.Program.Game_OnGameLoad();
                        break;
                    case "kassadin": // Kassawin
                        Kassawin.Kassadin.OnLoad();
                        break;
                    case "katarina": // Staberina
                        new Staberina.Katarina();
                        break;
                    case "kayle": // SephKayle
                        SephKayle.Program.OnGameLoad();
                        break;
                    case "aurelionsol": // El Aurelion Sol
                        ElAurelion_Sol.AurelionSol.OnGameLoad();
                        break;
                    case "kennen": // Underrated AIO
                        new UnderratedAIO.Champions.Kennen();
                        break;
                    case "khazix": // SephKhaZix
                        new SephKhazix.Khazix();
                        break;
                    case "kindred": // Yin Yang Kindred
                        Kindred___YinYang.Program.Game_OnGameLoad();
                        break;
                    case "kogmaw":
                        switch (Loader.kogmaw)
                        {
                            case 0:
                                KogMaw.Program.OnLoad();
                                break;
                            case 1:
                                Challenger_Series.Program.Main();
                                break;
                            default:
                                Challenger_Series.Program.Main();
                                break;
                        }
                        break;
                    case "leblanc": // PopBlanc
                        PopBlanc.Program.OnLoad();
                        break;
                    case "leesin": // El Lee Sin
                        switch (Loader.leesin)
                        {
                            case 0:
                                Valvrave_Sharp.Program.MainA();
                                break;
                            case 1:
                                ElLeeSin.Program.Game_OnGameLoad();
                                break;
                            default:
                                Valvrave_Sharp.Program.MainA();
                                break;
                        }
                        break;
                    case "leona": // El Easy
                        new ElEasy.Plugins.Leona();
                        break;
                    case "lissandra": // SephLissandra
                        SephLissandra.Lissandra.OnLoad();
                        break;
                    case "lucian": // LCS Lucian
                        LCS_Lucian.Program.OnLoad();
                        break;
                    case "lulu": // LuluLicious
                        new LuluLicious.Lulu();
                        break;
                    case "lux": // MoonLux
                        MoonLux.Program.GameOnOnGameLoad();
                        break;
                    case "malphite": // eleasy
                        new ElEasy.Plugins.Malphite();
                        break;
                    case "vayne": // ChallengerVayne
                        Vayne.Program.OnLoad();
                        break;
                    case "quinn": // GFuel Quinn
                        GFUELQuinn.Quinn.OnGameLoad();
                        break;
                    case "tristana": // ElTristana
                        ElTristana.Tristana.OnLoad();
                        break;
                    case "riven": // Nechrito Riven
                        NechritoRiven.Program.OnGameLoad();
                        break;
                    case "talon": // GFuel Talon
                        GFUELTalon.Talon.OnGameLoad();
                        break;
                    case "zed": // iZed
                        //iDZed.Zed.OnLoad();
                        Valvrave_Sharp.Program.MainA();
                        break;
                    case "udyr": // D_Udyr
                        D_Udyr.Program.Game_OnGameLoad();
                        break;
                    case "maokai": // Underrated AIO
                        new UnderratedAIO.Champions.Maokai();
                        break;
                    case "masteryi": // MasterSharp
                        MasterSharp.MasterSharp.OnLoad();
                        break;
                    case "mordekaiser": // How to Train your dragon
                        Mordekaiser.Program.Game_OnGameLoad();
                        break;
                    case "morgana": // Kurisi Morg
                        new KurisuMorgana.KurisuMorgana();
                        break;
                    case "nami": // vSupport Series
                        new vSupport_Series.Champions.Nami();
                        break;
                    case "nasus": // Underrated AIO
                        new UnderratedAIO.Champions.Nasus();
                        break;
                    case "nidalee": // Kurisu Nidalee
                        KurisuNidalee.KurisuNidalee.Game_OnGameLoad();
                        break;
                    case "yasuo": // YasuPro
                        //new YasuoPro.Yasuo();
                        Valvrave_Sharp.Program.MainA();
                        break;
                    case "nocturne": // Underrated AIO
                        new UnderratedAIO.Champions.Nocturne();
                        break;
                    case "poppy": // Underrated AIO
                        new UnderratedAIO.Champions.Poppy();
                        break;
                    case "rammus": // BrianSharp
                        new BrianSharp.Plugin.Rammus();
                        break;
                    case "rengar": // ElRengar
                        ElRengarRevamped.Rengar.OnLoad();
                        break;
                    case "soraka": // Sophie's Soraka
                        switch (Loader.soraka)
                        {
                            case 0:
                                Sophies_Soraka.SophiesSoraka.OnGameLoad();
                                break;
                            case 1:
                                Challenger_Series.Program.Main();
                                break;
                            default:
                                Challenger_Series.Program.Main();
                                break;
                        }
                        break;
                    case "twistedfate": // Twisted Fate by Kortatu
                        TwistedFate.Program.Game_OnGameLoad();
                        break;
                    case "varus": // ElVarus
                        Elvarus.Varus.Game_OnGameLoad();
                        break;
                    case "veigar": // FreshBooster
                        new FreshBooster.Champion.Veigar();
                        break;
                    case "reksai": // D-Reksai
                        D_RekSai.Program.Game_OnGameLoad();
                        break;
                    case "rumble": // Underrated AIO
                        new UnderratedAIO.Champions.Rumble();
                        break;
                    case "sejuani": // ElSejuani
                        ElSejuani.Sejuani.OnLoad();
                        break;
                    case "shaco": // Underrated AIO
                        new UnderratedAIO.Champions.Shaco();
                        break;
                    case "shen": // Underrated AIO
                        new UnderratedAIO.Champions.Shen();
                        break;
                    case "skarner": // Underrated AIO
                        new UnderratedAIO.Champions.Skarner();
                        break;
                    case "sona": // vSeries Support
                        new vSupport_Series.Champions.Sona();
                        break;
                    case "teemo": // Sharpshooter
                        new SharpShooter.Plugins.Teemo();
                        break;
                    case "viktor": // Trus In my Viktor
                        Viktor.Program.Game_OnGameLoad();
                        break;
                    case "vladimir": // ElVlad
                        ElVladimirReborn.Vladimir.OnLoad();
                        break;
                    case "warwick": // Warwick - Mirin
                        Warwick.Program.Game_OnGameLoad();
                        break;
                    case "monkeyking": // Wukong - xQx
                        Wukong.Program.Game_OnGameLoad();
                        break;
                    case "xinzhao": // Xin xQx
                        XinZhao.Program.Game_OnGameLoad();
                        break;
                    case "ziggs": // Ziggs#
                        Ziggs.Program.Game_OnGameLoad();
                        break;
                    case "yorick": // UnderratedAIO
                        new UnderratedAIO.Champions.Yorick();
                        break;
                    case "zyra": // D-Zyra
                        D_Zyra.Program.Game_OnGameLoad();
                        break;
                    case "zilean": // ElZilean
                        ElZilean.Zilean.Game_OnGameLoad();
                        break;
                    case "shyvana": // D-Shyvana
                        D_Shyvana.Program.Game_OnGameLoad();
                        break;
                    case "singed": // ElSinged
                        ElSinged.Singed.Game_OnGameLoad();
                        break;
                    case "zac": // Underrated AIO
                        new UnderratedAIO.Champions.Zac();
                        break;
                    case "volibear": // Underrated AIO
                        new UnderratedAIO.Champions.Volibear();
                        break;
                    case "tahmkench": // Underrated AIO
                        new UnderratedAIO.Champions.TahmKench();
                        break;
                    case "sion": // Underrated AIO
                        new UnderratedAIO.Champions.Sion();
                        break;
                    case "vi": //ElVi
                        ElVi.Vi.OnLoad();
                        break;
                    case "trundle": // ElTrundle
                        ElTrundle.Trundle.OnLoad();
                        break;
                    case "taric": // SkyLv_Taric
                        new SkyLv_Taric.SkyLv_Taric();
                        break;
                    default:
                        Chat.Print("This champion is not supported yet but the utilities will still load! - Berb");
                        break;
                }
            }
        }
    }
}