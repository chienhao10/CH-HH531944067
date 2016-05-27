using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SharpDX;

namespace ElUtilitySuite.Items
{
    /// <summary>
    ///     Casts Zhonya on dangerous spells.
    /// </summary>
    public class Zhonya : IPlugin
    {
        #region Static Fields

        /// <summary>
        ///     The zhyonya item
        /// </summary>
        private static LeagueSharp.Common.Items.Item zhonyaItem;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes the <see cref="Zhonya" /> class.
        /// </summary>
        static Zhonya()
        {
            #region Spells Init

            Spells = new List<ZhonyaSpell>
                         {
                             new ZhonyaSpell
                                 {
                                     ChampionName = "aatrox", SDataName = "aatroxq", MissileName = "", Delay = 250,
                                     MissileSpeed = 2000, CastRange = 650f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "ahri", SDataName = "ahriseduce", MissileName = "ahriseducemissile",
                                     Delay = 250, MissileSpeed = 1550, CastRange = 975f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "amumu", SDataName = "bandagetoss",
                                     MissileName = "sadmummybandagetoss", Delay = 250, MissileSpeed = 2000,
                                     CastRange = 1100f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "amumu", SDataName = "curseofthesadmummy", MissileName = "",
                                     Delay = 250, MissileSpeed = int.MaxValue, CastRange = 560f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "annie", SDataName = "infernalguardian", MissileName = "", Delay = 0,
                                     MissileSpeed = int.MaxValue, CastRange = 900f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "ashe", SDataName = "enchantedcrystalarrow",
                                     MissileName = "enchantedcrystalarrow", Delay = 250, MissileSpeed = 1600,
                                     CastRange = 20000f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "azir", SDataName = "azirr", MissileName = "", Delay = 250,
                                     MissileSpeed = int.MaxValue, CastRange = 475f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "blitzcrank", SDataName = "rocketgrabmissile", MissileName = "",
                                     Delay = 250, MissileSpeed = 1800, CastRange = 1050f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "ziggs", SDataName = "ziggsq", MissileName = "ziggsqspell",
                                     Delay = 250, MissileSpeed = 1750, CastRange = 850f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "ziggs", SDataName = "ziggsr", MissileName = "ziggsr", Delay = 1800,
                                     MissileSpeed = 1750, CastRange = 2250f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "zed", SDataName = "zedr", MissileName = "", Delay = 900,
                                     MissileSpeed = int.MaxValue, CastRange = 850f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "syndra", SDataName = "syndrar", MissileName = "", Delay = 450,
                                     MissileSpeed = 1250, CastRange = 675f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "syndra", SDataName = "syndraq", MissileName = "syndraq", Delay = 250,
                                     MissileSpeed = 1750, CastRange = 800f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "braum", SDataName = "braumq", MissileName = "braumqmissile",
                                     Delay = 250, MissileSpeed = 1200, CastRange = 1100f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "braum", SDataName = "braumqmissle", MissileName = "", Delay = 250,
                                     MissileSpeed = 1200, CastRange = 1100f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "braum", SDataName = "braumrwrapper", MissileName = "braumrmissile",
                                     Delay = 250, MissileSpeed = 1200, CastRange = 1250f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "cassiopeia", SDataName = "cassiopeiapetrifyinggaze",
                                     MissileName = "cassiopeiapetrifyinggaze", Delay = 350, MissileSpeed = int.MaxValue,
                                     CastRange = 875f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "chogath", SDataName = "rupture", MissileName = "rupture",
                                     Delay = 1000, MissileSpeed = int.MaxValue, CastRange = 950f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "darius", SDataName = "dariusaxegrabcone",
                                     MissileName = "dariusaxegrabcone", Delay = 150, MissileSpeed = int.MaxValue,
                                     CastRange = 555f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "diana", SDataName = "dianavortex", MissileName = "", Delay = 250,
                                     MissileSpeed = int.MaxValue, CastRange = 350f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "elise", SDataName = "elisehumane", MissileName = "elisehumane",
                                     Delay = 250, MissileSpeed = 1600, CastRange = 1075f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "evelynn", SDataName = "evelynnr", MissileName = "evelynnr",
                                     Delay = 250, MissileSpeed = int.MaxValue, CastRange = 900f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "fiddlesticks", SDataName = "terrify", MissileName = "", Delay = 250,
                                     MissileSpeed = int.MaxValue, CastRange = 575f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "fiddlesticks", SDataName = "crowstorm", MissileName = "", Delay = 250,
                                     MissileSpeed = int.MaxValue, CastRange = 800f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "galio", SDataName = "galioidolofdurand", MissileName = "", Delay = 0,
                                     MissileSpeed = int.MaxValue, CastRange = 600f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "garen", SDataName = "garenqattack", MissileName = "", Delay = 0,
                                     MissileSpeed = int.MaxValue, CastRange = 350f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "gnar", SDataName = "gnarult", MissileName = "", Delay = 250,
                                     MissileSpeed = int.MaxValue, CastRange = 600f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "gragas", SDataName = "gragase", MissileName = "gragase", Delay = 200,
                                     MissileSpeed = 1200, CastRange = 600f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "gragas", SDataName = "gragasr", MissileName = "gragasrboom",
                                     Delay = 250, MissileSpeed = 1750, CastRange = 1150f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "hecarim", SDataName = "hecarimult", MissileName = "", Delay = 50,
                                     MissileSpeed = 1200, CastRange = 1350f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "irelia", SDataName = "ireliaequilibriumstrike", MissileName = "",
                                     Delay = 250, MissileSpeed = int.MaxValue, CastRange = 450f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "janna", SDataName = "reapthewhirlwind", MissileName = "", Delay = 250,
                                     MissileSpeed = int.MaxValue, CastRange = 725f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "jarvaniv", SDataName = "jarvanivdragonstrike", MissileName = "",
                                     Delay = 250, MissileSpeed = 2000, CastRange = 700f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "jayce", SDataName = "jaycetotheskies", MissileName = "", Delay = 450,
                                     MissileSpeed = int.MaxValue, CastRange = 600f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "jayce", SDataName = "jayceshockblast",
                                     MissileName = "jayceshockblastmis", Delay = 250, MissileSpeed = 2350,
                                     CastRange = 1570f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "karma", SDataName = "karmaq", MissileName = "karmaqmissile",
                                     Delay = 250, MissileSpeed = 1800, CastRange = 1050f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "kassadin", SDataName = "nulllance", MissileName = "", Delay = 250,
                                     MissileSpeed = 1900, CastRange = 650f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "kassadin", SDataName = "forcepulse", MissileName = "", Delay = 250,
                                     MissileSpeed = int.MaxValue, CastRange = 700f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "leesin", SDataName = "blindmonkrkick", MissileName = "", Delay = 500,
                                     MissileSpeed = int.MaxValue, CastRange = 375f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "leona", SDataName = "leonashieldofdaybreak", MissileName = "",
                                     Delay = 250, MissileSpeed = int.MaxValue, CastRange = 215f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "leona", SDataName = "leonasolarflare",
                                     MissileName = "leonasolarflare", Delay = 1200, MissileSpeed = int.MaxValue,
                                     CastRange = 1200f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "lissandra", SDataName = "lissandraw", MissileName = "", Delay = 250,
                                     MissileSpeed = int.MaxValue, CastRange = 450f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "lissandra", SDataName = "lissandrar", MissileName = "", Delay = 250,
                                     MissileSpeed = int.MaxValue, CastRange = 550f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "lux", SDataName = "luxlightbinding",
                                     MissileName = "luxlightbindingmis", Delay = 250, MissileSpeed = 1200,
                                     CastRange = 1300f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "lux", SDataName = "luxmalicecannon",
                                     MissileName = "luxmalicecannonmis", Delay = 1750, MissileSpeed = int.MaxValue,
                                     CastRange = 3340f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "malphite", SDataName = "ufslash", MissileName = "ufslash",
                                     Delay = 250, MissileSpeed = 2200, CastRange = 1000f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "malzahar", SDataName = "alzaharnethergrasp", MissileName = "",
                                     Delay = 250, MissileSpeed = int.MaxValue, CastRange = 700f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "maokai", SDataName = "maokaiunstablegrowth", MissileName = "",
                                     Delay = 250, MissileSpeed = int.MaxValue, CastRange = 650f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "monkeyking", SDataName = "monkeykingspintowin", MissileName = "",
                                     Delay = 250, MissileSpeed = int.MaxValue, CastRange = 450f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "morgana", SDataName = "darkbindingmissile",
                                     MissileName = "darkbindingmissile", Delay = 250, MissileSpeed = 1200,
                                     CastRange = 1175f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "nami", SDataName = "namiq", MissileName = "namiqmissile", Delay = 250,
                                     MissileSpeed = 1750, CastRange = 875f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "nami", SDataName = "namir", MissileName = "namirmissile", Delay = 250,
                                     MissileSpeed = 1200, CastRange = 2550f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "nautilus", SDataName = "nautilusanchordrag",
                                     MissileName = "nautilusanchordragmissile", Delay = 250, MissileSpeed = 2000,
                                     CastRange = 1080f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "nocturne", SDataName = "nocturneunspeakablehorror", MissileName = "",
                                     Delay = 250, MissileSpeed = int.MaxValue, CastRange = 350f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "orianna", SDataName = "orianadetonatecommand",
                                     MissileName = "orianadetonatecommand", Delay = 500, MissileSpeed = int.MaxValue,
                                     CastRange = 425f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "riven", SDataName = "rivenmartyr", MissileName = "", Delay = 0,
                                     MissileSpeed = int.MaxValue, CastRange = 260f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "riven", SDataName = "rivenizunablade",
                                     MissileName = "rivenlightsabermissile", Delay = 250, MissileSpeed = 1600,
                                     CastRange = 1075
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "sejuani", SDataName = "sejuaniglacialprisoncast",
                                     MissileName = "sejuaniglacialprison", Delay = 250, MissileSpeed = 1600,
                                     CastRange = 1200f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "skarner", SDataName = "skarnerimpale", MissileName = "", Delay = 350,
                                     MissileSpeed = int.MaxValue, CastRange = 350f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "sona", SDataName = "sonar", MissileName = "sonar", Delay = 250,
                                     MissileSpeed = 2400, CastRange = 1000f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "thresh", SDataName = "threshq", MissileName = "threshqmissile",
                                     Delay = 500, MissileSpeed = 1900, CastRange = 1175f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "twistedfate", SDataName = "goldcardpreattack", MissileName = "",
                                     Delay = 250, MissileSpeed = int.MaxValue, CastRange = 600f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "varus", SDataName = "varusr", MissileName = "varusrmissile",
                                     Delay = 250, MissileSpeed = 1950, CastRange = 1300f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "vayne", SDataName = "vaynecondemnmissile", MissileName = "",
                                     Delay = 500, MissileSpeed = int.MaxValue, CastRange = 450f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "velkoz", SDataName = "velkozqplitactive", MissileName = "", Delay = 0,
                                     MissileSpeed = 1200, CastRange = 1050f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "velkoz", SDataName = "velkozr", MissileName = "", Delay = 0,
                                     MissileSpeed = 1500, CastRange = 1575f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "vi", SDataName = "viq", MissileName = "", Delay = 250,
                                     MissileSpeed = 1500, CastRange = 800f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "vi", SDataName = "vir", MissileName = "", Delay = 250,
                                     MissileSpeed = 1400, CastRange = 800f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "viktor", SDataName = "viktorchaosstorm", MissileName = "",
                                     Delay = 350, MissileSpeed = int.MaxValue, CastRange = 700f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "warwick", SDataName = "infiniteduress", MissileName = "", Delay = 250,
                                     MissileSpeed = int.MaxValue, CastRange = 700f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "xerath", SDataName = "xerathmagespear",
                                     MissileName = "xerathmagespearmissile", Delay = 250, MissileSpeed = 1600,
                                     CastRange = 1050f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "xinzhao", SDataName = "xenzhaosweep", MissileName = "", Delay = 250,
                                     MissileSpeed = 1750, CastRange = 600f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "xinzhao", SDataName = "xenzhaoparry", MissileName = "", Delay = 250,
                                     MissileSpeed = 1750, CastRange = 375f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "zac", SDataName = "zacr", MissileName = "", Delay = 250,
                                     MissileSpeed = int.MaxValue, CastRange = 850f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "zyra", SDataName = "zyrabramblezone", MissileName = "", Delay = 500,
                                     MissileSpeed = int.MaxValue, CastRange = 700f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "bard", SDataName = "bardr", MissileName = "bardr", Delay = 450,
                                     MissileSpeed = 210, CastRange = 3400f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "fizz", SDataName = "fizzmarinerdoom",
                                     MissileName = "fizzmarinerdoommissile", Delay = 250, MissileSpeed = 1300,
                                     CastRange = 1275f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "vladimir", SDataName = "vladimirhemoplague", MissileName = "",
                                     Delay = 250, MissileSpeed = int.MaxValue, CastRange = 875f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "mordekaiser", SDataName = "mordekaiserchildrenofthegrave",
                                     MissileName = "", Delay = 250, MissileSpeed = int.MaxValue, CastRange = 850f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "veigar", SDataName = "veigareventhorizon", MissileName = "",
                                     Delay = 250, MissileSpeed = 1400, CastRange = 850f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "veigar", SDataName = "veigardarkmatter", MissileName = "",
                                     Delay = 1200, MissileSpeed = int.MaxValue, CastRange = 900f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "shyvana", SDataName = "shyvanatransformcast",
                                     MissileName = "shyvanatransformcast", Delay = 100, MissileSpeed = 1100,
                                     CastRange = 1000f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "vayne", SDataName = "vaynecondemnmissile", MissileName = "",
                                     Delay = 500, MissileSpeed = int.MaxValue, CastRange = 450f
                                 },
                             new ZhonyaSpell
                                 {
                                     ChampionName = "pantheon", SDataName = "pantheonw", MissileName = "", Delay = 250,
                                     MissileSpeed = int.MaxValue, CastRange = 600f
                                 }
                         };

            #endregion
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the spells.
        /// </summary>
        /// <value>
        ///     The spells.
        /// </value>
        public static List<ZhonyaSpell> Spells { get; set; }

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        public Menu Menu { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        private static AIHeroClient Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        /// <summary>
        ///     Gets the zhonya below hp menu value.
        /// </summary>
        /// <value>
        ///     The zhonya below hp menu value.
        /// </value>
        private int ZhonyaBelowHp
        {
            get
            {
                return getSliderItem("ZhonyaHPSlider");
            }
        }

        /// <summary>
        ///     Gets a value indicating whether to zhonya at low hp.
        /// </summary>
        /// <value>
        ///     <c>true</c> if zhonya at low hp; otherwise, <c>false</c>.
        /// </value>
        private bool ZhonyaLowHp
        {
            get
            {
                return getCheckBoxItem("ZhonyaHP");
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        /// 
        public static Menu rootMenu = Entry.menu;
        public static Menu zhonyaMenu;

        public static bool getCheckBoxItem(string item)
        {
            return zhonyaMenu[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(string item)
        {
            return zhonyaMenu[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(string item)
        {
            return zhonyaMenu[item].Cast<KeyBind>().CurrentValue;
        }

        public void CreateMenu(Menu rootMenu)
        {
            zhonyaMenu = rootMenu.AddSubMenu("Zhonya's Hourglass", "zhonya");
            zhonyaMenu.AddGroupLabel("Spells");

            foreach (var spell in Spells.Where(x => ObjectManager.Get<AIHeroClient>().Where(y => y.IsEnemy).Any(y => y.ChampionName.ToLower() == x.ChampionName)))
            {
                var objAiHero = ObjectManager.Get<AIHeroClient>().FirstOrDefault(x => x.ChampionName.ToLower() == spell.ChampionName);
                if (objAiHero == null) { continue; }
                var firstOrDefault = objAiHero.Spellbook.Spells.FirstOrDefault(x => x.SData.Name.ToLower() == spell.SDataName);
                if (firstOrDefault != null)
                {
                    zhonyaMenu.Add(string.Format("Zhonya{0}", spell.SDataName), new CheckBox(string.Format("{0} ({1}) - {2}", char.ToUpper(spell.ChampionName[0]) + spell.ChampionName.Substring(1), firstOrDefault.Slot, spell.SDataName)));
                }
            }

            zhonyaMenu.Add("ZhonyaDangerous", new CheckBox("Use Zhonya"));
            zhonyaMenu.Add("ZhonyaHP", new CheckBox("Use Zhonya on low HP"));
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        public void Load()
        {
            zhonyaItem = new LeagueSharp.Common.Items.Item(Game.MapId == GameMapId.SummonersRift ? 3157 : 3090);

            Game.OnUpdate += Game_OnUpdate;
            GameObject.OnCreate += GameObjectOnCreate;
            Obj_AI_Base.OnProcessSpellCast += ObjAiBaseOnProcessSpellCast;
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if (!getCheckBoxItem("ZhonyaHP"))
            {
                return;
            }
            if (EloBuddy.SDK.Item.CanUseItem(ItemId.Zhonyas_Hourglass))
            {
                if (HealthPrediction.GetHealthPrediction(ObjectManager.Player, (int)(250 + Game.Ping / 2f)) <= ObjectManager.Player.MaxHealth / 4)
                {
                    EloBuddy.SDK.Item.UseItem(ItemId.Zhonyas_Hourglass);
                    return;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Determines whether the hero can evade the specified missile.
        /// </summary>
        /// <param name="missile">The missile.</param>
        /// <param name="hero">The hero.</param>
        /// <returns></returns>
        private bool CanEvadeMissile(MissileClient missile, Obj_AI_Base hero)
        {
            var heroPos = hero.ServerPosition.To2D();
            float evadeTime = 0;
            float spellHitTime = 0;

            if (missile.SData.TargettingType.ToString().Contains("Location")
                && !missile.SData.TargettingType.ToString().Contains("Aoe"))
            {
                var projection =
                    heroPos.ProjectOn(missile.StartPosition.To2D(), missile.EndPosition.To2D()).SegmentPoint;
                evadeTime = 1000 * (missile.SData.LineWidth - heroPos.LSDistance(projection) + hero.BoundingRadius)
                            / hero.MoveSpeed;
                spellHitTime = GetSpellHitTime(missile, projection);
            }
            else if (missile.SData.TargettingType == SpellDataTargetType.LocationAoe)
            {
                evadeTime = 1000 * (missile.SData.CastRadius - heroPos.LSDistance(missile.EndPosition)) / hero.MoveSpeed;
                spellHitTime = GetSpellHitTime(missile, heroPos);
            }

            return spellHitTime > evadeTime;
        }

        /// <summary>
        ///     Fired when a game object is created.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void GameObjectOnCreate(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid<MissileClient>() || sender.IsAlly || !zhonyaItem.IsReady() || !getCheckBoxItem("ZhonyaDangerous"))
            {
                return;
            }

            var missile = (MissileClient)sender;
            var sdata = Spells.FirstOrDefault(x => missile.SData.Name.ToLower().Equals(x.MissileName) || missile.SData.Name.ToLower().Equals(x.SDataName));

            // Not in database
            if (sdata == null)
            {
                return;
            }

            if (!getCheckBoxItem(string.Format("Zhonya{0}", sdata.SDataName)) || !getCheckBoxItem("ZhonyaDangerous"))
            {
                return;
            }

            // Correct the end position
            var endPosition = missile.EndPosition;

            if (missile.StartPosition.LSDistance(endPosition) > sdata.CastRange)
            {
                endPosition = missile.StartPosition + Vector3.Normalize(endPosition - missile.StartPosition) * sdata.CastRange;
            }

            if (missile.SData.LineWidth + Player.BoundingRadius > Player.ServerPosition.To2D().LSDistance(Player.ServerPosition.To2D().ProjectOn(missile.StartPosition.To2D(), endPosition.To2D()).SegmentPoint))
            {
                zhonyaItem.Cast();
            }
        }

        /// <summary>
        ///     Gets the spell hit time.
        /// </summary>
        /// <param name="missile">The missile.</param>
        /// <param name="pos">The position.</param>
        /// <returns></returns>
        private float GetSpellHitTime(MissileClient missile, Vector2 pos)
        {
            if (!missile.SData.TargettingType.ToString().Contains("Location")
                || missile.SData.TargettingType.ToString().Contains("Aoe"))
            {
                return missile.SData.TargettingType == SpellDataTargetType.LocationAoe
                           ? Math.Max(
                               0,
                               missile.SData.CastFrame / 30 * 1000
                               + missile.StartPosition.LSDistance(missile.EndPosition) / missile.SData.MissileSpeed * 1000
                               - Environment.TickCount - Game.Ping)
                           : float.MaxValue;
            }

            if (
                Spells.Find(x => x.MissileName.Equals(missile.Name, StringComparison.InvariantCultureIgnoreCase))
                    .MissileSpeed == int.MaxValue)
            {
                return Math.Max(
                    0,
                    missile.SData.CastFrame / 30 * 1000
                    + missile.StartPosition.LSDistance(missile.EndPosition) / missile.SData.MissileSpeed * 1000
                    - Environment.TickCount - Game.Ping);
            }

            var spellPos = missile.Position.To2D();
            return 1000 * spellPos.LSDistance(pos) / missile.SData.MissileAccel;
        }

        /// <summary>
        ///     Fired when the game processes a spell cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GameObjectProcessSpellCastEventArgs" /> instance containing the event data.</param>
        private void ObjAiBaseOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsAlly || !zhonyaItem.IsReady() || !getCheckBoxItem("ZhonyaDangerous"))
            {
                return;
            }

            var spellData =
                Spells.FirstOrDefault(
                    x => x.SDataName == args.SData.Name.ToLower() || x.MissileName == args.SData.Name.ToLower());

            if (spellData == null)
            {
                return;
            }

            if (!getCheckBoxItem(string.Format("Zhonya{0}", spellData.SDataName)) || !getCheckBoxItem("ZhonyaDangerous"))
            {
                return;
            }

            if (Player.LSDistance(args.Start) > spellData.CastRange)
            {
                return;
            }

            // Targetted spells
            if (args.SData.TargettingType == SpellDataTargetType.Unit && args.Target.IsMe || args.SData.TargettingType == SpellDataTargetType.SelfAndUnit && args.Target.IsMe || args.SData.TargettingType == SpellDataTargetType.Self || args.SData.TargettingType == SpellDataTargetType.SelfAoe && Player.LSDistance(sender) < spellData.CastRange)
            {
                LeagueSharp.Common.Utility.DelayAction.Add((int)spellData.Delay, () => zhonyaItem.Cast());
                return;
            }

            // Anything besides a skillshot return
            if (!args.SData.TargettingType.ToString().Contains("Location")
                && args.SData.TargettingType != SpellDataTargetType.Cone)
            {
                return;
            }

            // Correct the end position
            var endPosition = args.End;

            if (args.Start.LSDistance(endPosition) > spellData.CastRange)
            {
                endPosition = args.Start + Vector3.Normalize(endPosition - args.Start) * spellData.CastRange;
            }

            // credits to kurisu
            var isLinear = args.SData.TargettingType == SpellDataTargetType.Cone || args.SData.LineWidth > 0;
            var width = isLinear && args.SData.TargettingType != SpellDataTargetType.Cone
                            ? args.SData.LineWidth
                            : (args.SData.CastRadius < 1 ? args.SData.CastRadiusSecondary : args.SData.CastRadius);

            if ((isLinear && width + Player.BoundingRadius > Player.ServerPosition.To2D().LSDistance(Player.ServerPosition.To2D().ProjectOn(args.Start.To2D(), endPosition.To2D()).SegmentPoint)) || (!isLinear && Player.LSDistance(endPosition) <= width + Player.BoundingRadius))
            {

                zhonyaItem.Cast();
            }
        }

        #endregion

        /// <summary>
        ///     Represents a spell that zhonya should be casted on.
        /// </summary>
        public class ZhonyaSpell
        {
            #region Public Properties

            /// <summary>
            ///     Gets or sets the cast range.
            /// </summary>
            /// <value>
            ///     The cast range.
            /// </value>
            public float CastRange { get; set; }

            /// <summary>
            ///     Gets or sets the name of the champion.
            /// </summary>
            /// <value>
            ///     The name of the champion.
            /// </value>
            public string ChampionName { get; set; }

            /// <summary>
            ///     Gets or sets the delay.
            /// </summary>
            /// <value>
            ///     The delay.
            /// </value>
            public float Delay { get; set; }

            /// <summary>
            ///     Gets or sets the name of the missile.
            /// </summary>
            /// <value>
            ///     The name of the missile.
            /// </value>
            public string MissileName { get; set; }

            /// <summary>
            ///     Gets or sets the missile speed.
            /// </summary>
            /// <value>
            ///     The missile speed.
            /// </value>
            public int MissileSpeed { get; set; }

            /// <summary>
            ///     Gets or sets the name of the s data.
            /// </summary>
            /// <value>
            ///     The name of the s data.
            /// </value>
            public string SDataName { get; set; }

            #endregion
        }
    }
}