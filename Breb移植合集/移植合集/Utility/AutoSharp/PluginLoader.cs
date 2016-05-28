using AutoSharp.Plugins;
using EloBuddy;
using LeagueSharp;

namespace AutoSharp
{
    public class PluginLoader
    {
        private static bool _loaded;
        public PluginLoader()
        {
            if (!_loaded)
            {
                switch (ObjectManager.Player.ChampionName.ToLower())
                {
                    case "aatrox":
                        new Aatrox();
                        _loaded = true;
                        break;
                    case "ahri":
                        new Ahri();
                        _loaded = true;
                        break;
                    case "akali":
                        new Akali();
                        _loaded = true;
                        break;
                    case "alistar":
                        new Alistar();
                        _loaded = true;
                        break;
                    case "amumu":
                        new Amumu();
                        _loaded = true;
                        break;
                    case "anivia":
                        new Anivia();
                        _loaded = true;
                        break;
                    case "annie":
                        new Annie();
                        _loaded = true;
                        break;
                    case "ashe":
                        new Ashe();
                        _loaded = true;
                        break;
                    case "bard":
                        new Bard();
                        _loaded = true;
                        break;
                    case "blitzcrank":
                        new Blitzcrank();
                        _loaded = true;
                        break;
                    case "brand":
                        new Brand();
                        _loaded = true;
                        break;
                    case "braum":
                        new Braum();
                        _loaded = true;
                        break;
                    case "caitlyn":
                        new Caitlyn();
                        _loaded = true;
                        break;
                    case "cassiopeia":
                        new Cassiopeia();
                        _loaded = true;
                        break;
                    case "chogath":
                        new Chogath();
                        _loaded = true;
                        break;
                    case "corki":
                        new Corki();
                        _loaded = true;
                        break;
                    case "darius":
                        new Darius();
                        _loaded = true;
                        break;
                    case "diana":
                        new Diana();
                        _loaded = true;
                        break;
                    case "draven":
                        new Draven();
                        _loaded = true;
                        break;
                    case "evelynn":
                        new AutoSharp.Plugins.Evelynn();
                        _loaded = true;
                        break;
                    case "ezreal":
                        new Ezreal();
                        _loaded = true;
                        break;
                    case "fiddlesticks":
                        new FiddleSticks();
                        _loaded = true;
                        break;
                    case "fiora":
                        new Fiora();
                        _loaded = true;
                        break;
                    case "fizz":
                        new Fizz();
                        _loaded = true;
                        break;
                    case "galio":
                        new Galio();
                        _loaded = true;
                        break;
                    case "gangplank":
                        new Gangplank();
                        _loaded = true;
                        break;
                    case "garen":
                        new Garen();
                        _loaded = true;
                        break;
                    case "gragas":
                        new Gragas();
                        _loaded = true;
                        break;
                    case "graves":
                        new Graves();
                        _loaded = true;
                        break;
                    case "heimerdinger":
                        new Heimerdinger();
                        _loaded = true;
                        break;
                    case "irelia":
                        new Irelia();
                        _loaded = true;
                        break;
                    case "kalista":
                        new Kalista();
                        _loaded = true;
                        break;
                    case "karma":
                        new AutoSharp.Plugins.Karma();
                        _loaded = true;
                        break;
                    case "karthus":
                        new Karthus();
                        _loaded = true;
                        break;
                    case "katarina":
                        new Katarina();
                        _loaded = true;
                        break;
                    case "kayle":
                        new Kayle();
                        _loaded = true;
                        break;
                    case "kogmaw":
                        new Kayle();
                        _loaded = true;
                        break;
                    case "leblanc":
                        new AutoSharp.Plugins.Leblanc();
                        _loaded = true;
                        break;
                    case "leona":
                        new Leona();
                        _loaded = true;
                        break;
                    case "lucian":
                        new Lucian();
                        _loaded = true;
                        break;
                    case "lulu":
                        new Lulu();
                        _loaded = true;
                        break;
                    case "lux":
                        new Lux();
                        _loaded = true;
                        break;
                    case "malzahar":
                        new Malzahar();
                        _loaded = true;
                        break;
                    case "masteryi":
                        new Masteryi();
                        _loaded = true;
                        break;
                    case "morgana":
                        new Morgana();
                        _loaded = true;
                        break;
                    case "nami":
                        new Nami();
                        _loaded = true;
                        break;
                    case "nunu":
                        new Nunu();
                        _loaded = true;
                        break;
                    case "poppy":
                        new Poppy();
                        _loaded = true;
                        break;
                    case "riven":
                        new Riven();
                        _loaded = true;
                        break;
                    case "shaco":
                        new Shaco();
                        _loaded = true;
                        break;
                    case "sivir":
                        new Sivir();
                        _loaded = true;
                        break;
                    case "skarner":
                        new Skarner();
                        _loaded = true;
                        break;
                    case "sona":
                        new Sona();
                        _loaded = true;
                        break;
                    case "soraka":
                        new Soraka();
                        _loaded = true;
                        break;
                    case "taric":
                        new Taric();
                        _loaded = true;
                        break;
                    case "teemo":
                        new Teemo();
                        _loaded = true;
                        break;
                    case "thresh":
                        new Thresh();
                        _loaded = true;
                        break;
                    case "tristana":
                        new Tristana();
                        _loaded = true;
                        break;
                    case "vayne":
                        new AutoSharp.Plugins.Vayne();
                        _loaded = true;
                        break;
                    case "veigar":
                        new Veigar();
                        _loaded = true;
                        break;
                    case "vladimir":
                        new Vladimir();
                        _loaded = true;
                        break;
                    case "warwick":
                        new AutoSharp.Plugins.Warwick();
                        _loaded = true;
                        break;
                    case "xerath":
                        new Xerath();
                        _loaded = true;
                        break;
                    case "zilean":
                        new Zilean();
                        _loaded = true;
                        break;
                    case "zyra":
                        new Zyra();
                        _loaded = true;
                        break;
                    default:
                        new Default();
                        _loaded = true;
                        break;
                }
            }
        }

    }
}
