namespace ElUtilitySuite.Summoners
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using SharpDX;
    using LeagueSharp.Common;
    using LeagueSharp.Common.Data;

    using Color = SharpDX.Color;
    using ItemData = LeagueSharp.Common.Data.ItemData;

    internal class Cleanse : IPlugin
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes the <see cref="Cleanse" /> class.
        /// </summary>
        static Cleanse()
        {
            #region Spell Data

            Spells = new List<CleanseSpell>
                         {
                             /*new CleanseSpell
                                 {
                                     Name = "summonerdot", MenuName = "Summoner Ignite", Evade = false, DoT = true,
                                     EvadeTimer = 0, Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.Unknown,
                                     Interval = 1.0
                                 },*/
                             /*new CleanseSpell
                                 {
                                     Name = "summonerexhaustdebuff", MenuName = "Summoner Exhaust", Evade = false,
                                     DoT = true, EvadeTimer = 0, Cleanse = false, CleanseTimer = 0,
                                     Slot = SpellSlot.Unknown, Interval = 1.0
                                 },*/
                             /*new CleanseSpell
                                 {
                                     Name = "itemdusknightfall", MenuName = "Duskblade", Evade = false, DoT = true,
                                     EvadeTimer = 0, Cleanse = false, CleanseTimer = 1650, Slot = SpellSlot.Unknown,
                                     Interval = 1.0
                                 },*/
                             new CleanseSpell
                                 {
                                     Champion = "Taric", Name = "stun", MenuName = "Taric Stun", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.E,
                                     Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Lulu", Name = "polymorph", MenuName = "Lulu polymorph", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.W,
                                     Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Nocturne", Name = "fear", MenuName = "Nocturne Fear", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.E,
                                     Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Pantheon", Name = "stun", MenuName = "Pantheon Stun", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.W,
                                     Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Leona", Name = "stun", MenuName = "Leona Stun", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.Q,
                                     Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Quinn", Name = "QuinnQSightReduction", MenuName = "Quinn Blind",
                                     Evade = false, DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.Q, Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Teemo", Name = "blind", MenuName = "Teemo Blind", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.Q,
                                     Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Ashe", Name = "stun", MenuName = "Ashe (R)", Evade = false, DoT = false,
                                     EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.R
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "AurelionSol", Name = "stun", MenuName = "Aurelion Sol stun", Evade = false, DoT = false,
                                     EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.Q
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Brand", Name = "stun", MenuName = "Brand stun", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.Unknown, Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Ekko", Name = "stun", MenuName = "Ekko stun", Evade = false, DoT = false,
                                     EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.Unknown,
                                     Interval = 1.0
                                 },
                             /*new CleanseSpell
                                 {
                                     Champion = "Vi", Name = "virknockup", MenuName = "Vi R Knockup", Evade = true,
                                     DoT = false, EvadeTimer = 0, Cleanse = false, CleanseTimer = 0,
                                     Slot = SpellSlot.Unknown, Interval = 1.0
                                 },*/

                             /*new CleanseSpell
                                 {
                                     Champion = "Gangplank", Name = "gangplankpassiveattackdot",
                                     MenuName = "Gangplank Passive Burn", Evade = false, DoT = true, EvadeTimer = 0,
                                     Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.Unknown, Interval = .8
                                 },*/
                             new CleanseSpell
                                 {
                                     Champion = "Teemo", Name = "bantamtraptarget", MenuName = "Teemo Shroom",
                                     Evade = false, DoT = true, EvadeTimer = 0, Cleanse = false, CleanseTimer = 0,
                                     Slot = SpellSlot.Unknown, Interval = 1.0
                                 },
                             /*new CleanseSpell
                                 {
                                     Champion = "Teemo", Name = "toxicshotparticle", MenuName = "Teemo Toxic Shot",
                                     Evade = false, DoT = true, EvadeTimer = 0, Cleanse = false, CleanseTimer = 0,
                                     Slot = SpellSlot.E, Interval = 1.0
                                 },*/
                             new CleanseSpell
                                 {
                                     Champion = "Ahri", Name = "ahriseduce", MenuName = "Ahri Charm", Evade = false,
                                     DoT = true, EvadeTimer = 0, Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.E,
                                     Interval = 1.0
                                 },
                             /*new CleanseSpell
                                 {
                                     Champion = "Talon", Name = "talonbleeddebuf", MenuName = "Talon Bleed", Evade = false,
                                     DoT = true, EvadeTimer = 0, Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.Q,
                                     Interval = .8
                                 },*/
                             new CleanseSpell
                                 {
                                     Champion = "Malzahar", Name = "MalzaharR",
                                     MenuName = "Malzahar Nether Grasp (R)", Evade = false, DoT = true, EvadeTimer = 0,
                                     Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.R, Interval = .8
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Malzahar", Name = "malzaharerecent",
                                     MenuName = "Malzahar Ficvisions (E)", Evade = false, DoT = true, EvadeTimer = 0,
                                     Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.E, Interval = .8
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Soraka", Name = "SorakaESnare", MenuName = "Soraka E snare",
                                     Evade = false, DoT = true, EvadeTimer = 0, Cleanse = false, CleanseTimer = 0,
                                     Slot = SpellSlot.E, Interval = 1.0
                                 },
                             /*new CleanseSpell
                                 {
                                     Champion = "FiddleSticks", Name = "Drain", MenuName = "Fiddle Drain", Evade = false,
                                     DoT = true, EvadeTimer = 0, Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.W,
                                     Interval = 1.0
                                 },*/
                             new CleanseSpell
                                 {
                                     Champion = "FiddleSticks", Name = "fleeslow", MenuName = "Fiddle Fear", Evade = false,
                                     DoT = true, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.Q,
                                     Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "FiddleSticks", Name = "Silence", MenuName = "Fiddle Silence",
                                     Evade = false, DoT = true, EvadeTimer = 0, Cleanse = false, CleanseTimer = 0,
                                     Slot = SpellSlot.E, Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Chogath", Name = "Silence", MenuName = "Cho Silence", Evade = false,
                                     DoT = true, EvadeTimer = 0, Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.W,
                                     Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Galio", Name = "galioidolofdurand", MenuName = "Galio Idol of Durand",
                                     Evade = false, DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.R, Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Nasus", Name = "nasusw", MenuName = "Nasus Wither", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.W,
                                     Interval = 1.0
                                 },
                             /*new CleanseSpell
                                 {
                                     Champion = "Hecarim", Name = "hecarimdefilelifeleech",
                                     MenuName = "Hecarim Defile Leech", Evade = false, DoT = true, EvadeTimer = 0,
                                     Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.W, Interval = 1.0
                                 },*/
                             /*new CleanseSpell
                                 {
                                     Champion = "Swain", Name = "swaintorment", MenuName = "Swain Torment", Evade = false,
                                     DoT = true, EvadeTimer = 0, Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.E,
                                     Interval = 1.0
                                 },*/
                             /*new CleanseSpell
                                 {
                                     Champion = "Brand", Name = "brandablaze", MenuName = "Brand Burn Passive",
                                     Evade = false, DoT = true, EvadeTimer = 0, Cleanse = false, CleanseTimer = 0,
                                     Slot = SpellSlot.Unknown, Interval = 0.5
                                 },*/
                             /*new CleanseSpell
                                 {
                                     Champion = "Fizz", Name = "fizzseastonetrident", MenuName = "Fizz Burn Passive",
                                     Evade = false, DoT = true, EvadeTimer = 0, Cleanse = false, CleanseTimer = 0,
                                     Slot = SpellSlot.Unknown, Interval = .8
                                 },*/
                             /*new CleanseSpell
                                 {
                                     Champion = "Tristana", Name = "tristanaechargesound",
                                     MenuName = "Tristana Explosive Charge", Evade = false, DoT = true, EvadeTimer = 0,
                                     Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.E, Interval = .8
                                 },*/
                             /*new CleanseSpell
                                 {
                                     Champion = "Darius", Name = "DariusNoxianTacticsONH", MenuName = "Darius (W)",
                                     Evade = false, DoT = true, EvadeTimer = 0, Cleanse = false, CleanseTimer = 0,
                                     Slot = SpellSlot.W, Interval = 1.0
                                 },*/
                             /*new CleanseSpell
                                 {
                                     Champion = "Nidalee", Name = "bushwackdamage", MenuName = "Nidalee Bushwhack",
                                     Evade = false, DoT = true, EvadeTimer = 0, Cleanse = false, CleanseTimer = 0,
                                     Slot = SpellSlot.W, Interval = .8
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Nidalee", Name = "nidaleepassivehunted",
                                     MenuName = "Nidalee Passive Mark", Evade = false, DoT = true, EvadeTimer = 0,
                                     Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.Unknown, Interval = .8
                                 },*/
                             new CleanseSpell
                                 {
                                     Name = "missfortunescattershotslow", Evade = false, DoT = true, EvadeTimer = 0,
                                     Champion = "MissFortune", Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.E,
                                     Interval = 0.5
                                 },
                             new CleanseSpell
                                 {
                                     Name = "missfortunepassivestack", Evade = false, DoT = true, EvadeTimer = 0,
                                     Champion = "MissFortune", Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.R,
                                     Interval = 0.5
                                 },
                             /*new CleanseSpell
                                 {
                                     Champion = "Wukong", Name = "monkeykingspintowin", Evade = false, DoT = true,
                                     EvadeTimer = 0, Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.R, Interval = 1.0
                                 },*/
                             /*new CleanseSpell
                                 {
                                     Champion = "Cassiopeia", Name = "cassiopeianoxiousblastpoison",
                                     MenuName = "Cassio Noxious Blast", Evade = false, Cleanse = false, DoT = true,
                                     EvadeTimer = 0, CleanseTimer = 0, Slot = SpellSlot.Q, Interval = 0.4
                                 },*/
                             new CleanseSpell
                                 {
                                     Champion = "Cassiopeia", Name = "cassiopeiamiasmapoison", MenuName = "Cassio Miasma",
                                     Evade = false, Cleanse = false, DoT = true, EvadeTimer = 0, CleanseTimer = 0,
                                     Slot = SpellSlot.Q, Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Cassiopeia", Name = "cassiopeiapetrifyinggazestun",
                                     MenuName = "Cassio Petrifying Gaze", Evade = false, DoT = false, EvadeTimer = 0,
                                     Cleanse = true, CleanseTimer = 100, Slot = SpellSlot.R, Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Sejuani", Name = "sejuaniglacialprison",
                                     MenuName = "Sejuani Glacial Prison", Evade = false, DoT = false, EvadeTimer = 0,
                                     Cleanse = true, CleanseTimer = 100, Slot = SpellSlot.R, Interval = 1.0
                                 },
                             /*new CleanseSpell
                                 {
                                     Champion = "Fiora", Name = "fiorarmark", MenuName = "Fiora Grand Challenge",
                                     Evade = false, DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 100,
                                     Slot = SpellSlot.R, Interval = 1.0
                                 },*/
                             /*new CleanseSpell
                                 {
                                     Champion = "Twitch", Name = "twitchdeadlyvenon", MenuName = "Twitch Deadly Venom",
                                     Evade = false, DoT = true, EvadeTimer = 0, Cleanse = false, CleanseTimer = 0,
                                     Slot = SpellSlot.E, Interval = 0.6
                                 },*/
                             /*new CleanseSpell
                                 {
                                     Champion = "Urgot", Name = "urgotcorrosivedebuff",
                                     MenuName = "Urgot Corrosive Charge", Evade = false, DoT = true, EvadeTimer = 0,
                                     Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.E, Interval = 1.0
                                 },*/
                             /*new CleanseSpell
                                 {
                                     Champion = "Zac", Name = "zacr", Evade = true, DoT = true, EvadeTimer = 150,
                                     Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.R, Interval = 1.5
                                 },*/
                             /*new CleanseSpell
                                 {
                                     Champion = "Mordekaiser", Name = "mordekaiserchildrenofthegrave",
                                     MenuName = "Mordekaiser Children of the Grave", Evade = false, DoT = true,
                                     EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.Unknown,
                                     Interval = 1.5
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Zed", Name = "zedrtargetmark", MenuName = "Zed Mark", Evade = true,
                                     DoT = false, EvadeTimer = 2600, Cleanse = true, CleanseTimer = 1000,
                                     Slot = SpellSlot.R, Interval = 1.0
                                 }, */
                             /*new CleanseSpell
                                 {
                                     Champion = "Karthus", Name = "fallenonetarget", Evade = true, DoT = false,
                                     EvadeTimer = 2600, Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.R
                                 },*/
                             /*new CleanseSpell
                                 {
                                     Champion = "Karthus", Name = "karthusfallenonetarget", Evade = true, DoT = false,
                                     EvadeTimer = 2600, Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.R
                                 },*/
                              new CleanseSpell
                                 {
                                     Champion = "Karthus", Name = "KarthusWallOfPain ",  MenuName = "Karthus W", Evade = true, DoT = false,
                                     EvadeTimer = 1600, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.W
                                 },
                             /*new CleanseSpell
                                 {
                                     Champion = "Fizz", Name = "fizzmarinerdoombomb", MenuName = "Fizz Shark Bait",
                                     Evade = false, DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.R
                                 },*/
                             new CleanseSpell
                                 {
                                     Champion = "Morgana", Name = "SoulShackless", MenuName = "Morgana Soul Shackles",
                                     Evade = true, DoT = false, EvadeTimer = 2600, Cleanse = true, CleanseTimer = 1100,
                                     Slot = SpellSlot.R, Interval = 3.9
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Morgana", Name = "DarkBindingMissile", MenuName = "Morgana Q",
                                     Evade = true, DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.Q
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Varus", Name = "varusrsecondary", MenuName = "Varus Chains of Corruption",
                                     Evade = false, DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.R
                                 },
                             /*new CleanseSpell
                                 {
                                     Champion = "Caitlyn", Name = "caitlynaceinthehole",
                                     MenuName = "Caitlyn Ace in the Hole", Evade = true, DoT = false, EvadeTimer = 900,
                                     Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.R
                                 },*/
                             /*new CleanseSpell
                                 {
                                     Champion = "Vladimir", Name = "vladimirhemoplague", MenuName = "Vladimir Hemoplague",
                                     Evade = true, DoT = false, EvadeTimer = 4500, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.R
                                 },*/
                             /*new CleanseSpell
                                 {
                                     Champion = "Diana", Name = "dianamoonlight", MenuName = "Diana Moonlight",
                                     Evade = false, DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.Q
                                 },*/
                             new CleanseSpell
                                 {
                                     Champion = "Urgot", Name = "urgotswap2", MenuName = "Urgot Swap", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.R
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Diana", Name = "DianaArc", MenuName = "Diana Q", Evade = true,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.R
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Skarner", Name = "skarnerimpale", MenuName = "Skarner Impale",
                                     Evade = false, DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 500,
                                     Slot = SpellSlot.R
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Poppy", Name = "Stun", MenuName = "Poppy wall stun", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.E
                                 },
                             /*new CleanseSpell
                                 {
                                     Champion = "Poppy", Name = "poppyulttargetmark",
                                     MenuName = "Poppy Diplomatic Immunity", Evade = false, DoT = false, EvadeTimer = 0,
                                     Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.R
                                 },*/
                             /*new CleanseSpell
                                 {
                                     Champion = "LeeSin", Name = "blindmonkqone", MenuName = "Lee Sin Q", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.Q
                                 },*/
                             new CleanseSpell
                                 {
                                     Champion = "LeeSin", Name = "knockback", MenuName = "Lee Sin R", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.R
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Leblanc", Name = "leblancsoulshackle", MenuName = "Leblanc Shackle",
                                     Evade = false, DoT = false, EvadeTimer = 2000, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.E
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Leblanc", Name = "leblancsoulshacklem", MenuName = "Leblanc Shackle (R)",
                                     Evade = true, DoT = false, EvadeTimer = 2000, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.E
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "twistedfate", Name = "stun", MenuName = "Twisted Fate Gold (W)",
                                     Evade = false, DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.Unknown
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "amumu", Name = "bandagetoss", MenuName = "bandagetoss (Q)", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.Q
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "amumu", Name = "curseofthesadmummy", MenuName = "curseofthesadmummy (R)",
                                     Evade = false, DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.R
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Rammus", Name = "taunt", MenuName = "Rammus Taunt", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.E
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "shen", Name = "taunt", MenuName = "Shen Taunt", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.E
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "jax", Name = "stun", MenuName = "Jax (E)", Evade = false, DoT = false,
                                     EvadeTimer = 0, Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.E
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "kennen", Name = "stun", MenuName = "Kennen stun", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.Unknown
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "braum", Name = "braumstundebuff", MenuName = "Braum Passive",
                                     Evade = false, DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.Unknown
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "nunu", Name = "IceBlast", MenuName = "Nunu E", Evade = false, DoT = false,
                                     EvadeTimer = 0, Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.E
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "riven", Name = "stun", MenuName = "Riven (W)", Evade = false, DoT = false,
                                     EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.W
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "annie", Name = "Stun", MenuName = "Annie Stun", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.Unknown
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Warwick", Name = "suppression", MenuName = "Warwick (R)", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = false, CleanseTimer = 0, Slot = SpellSlot.R
                                 },
                              new CleanseSpell
                                 {
                                     Champion = "maokai", Name = "MaokaiUnstableGrowthRoot", MenuName = "Maokai (W)", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.W
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "anivia", Name = "stun", MenuName = "Anivia Stun", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.Q
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "heimerdinger", Name = "stun", MenuName = "Heimerdinger Stun",
                                     Evade = false, DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.W
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "bard", Name = "bardqshackledebuff", MenuName = "Bard Stun", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.Unknown
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "vayne", Name = "stun", MenuName = "Vayne Condemn", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.E
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "elise", Name = "buffelisecocoon", MenuName = "Elise E", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.E
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "udyr", Name = "stun", MenuName = "Udyr Stun", Evade = false, DoT = false,
                                     EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.E
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "irelia", Name = "stun", MenuName = "Irelia Stun", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.E
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "veigar", Name = "stun", MenuName = "Veigar Stun", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.E
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "gnar", Name = "stun", MenuName = "Gnar stun", Evade = false, DoT = false,
                                     EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.R
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "syndra", Name = "stun", MenuName = "Syndra stun", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.Unknown
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "sona", Name = "SonaR", MenuName = "Sona (R)", Evade = true, DoT = false,
                                     EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.R
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "xerath", Name = "stun", MenuName = "Xerath (E)", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.E
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "thresh", Name = "threshq", MenuName = "Thresh (Q)", Evade = true,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.Q
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "Lissandra", Name = "lissandrarenemy2", MenuName = "Lissandra (R)",
                                     Evade = false, DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 100,
                                     Slot = SpellSlot.R, Interval = 1.0
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "lissandra", Name = "lissandraw", MenuName = "Lissandra (W)",
                                     Evade = false, DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.W
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "lux", Name = "luxlightbinding", MenuName = "Lux (Q)", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.Q
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "renekton", Name = "stun", MenuName = "Renekton stun", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.W
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "tahmkench", Name = "tahmkenchqstun", MenuName = "Tahm Q stun",
                                     Evade = false, DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.Q
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "tahmkench", Name = "tahmkenchwhasdevouredtarget", MenuName = "Tahm stun",
                                     Evade = false, DoT = false, EvadeTimer = 0, Cleanse = false, QssIgnore = true,
                                     CleanseTimer = 0, Slot = SpellSlot.Unknown
                                 },
                             new CleanseSpell
                                 {
                                     Champion = "nautilus", Name = "nautilusanchordragroot", MenuName = "Nautilus Q",
                                     Evade = false, DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0,
                                     Slot = SpellSlot.Q //The Q isn't the root here, is this just a weird entry that still happens to be tracking the passive?//
                                 },
                             /*new CleanseSpell
                                 {
                                     Champion = "nautilus", Name = "stun", MenuName = "Nautilus R", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.R
                                 },*/ //Looking at the wiki it seems like nautilus has a stun on impact along with the knockup, displacement can't be cleansed, but I don't know if the stun lasts after the hit and when you're back on the ground//
                             new CleanseSpell
                                 {
                                     Champion = "zilean", Name = "stun", MenuName = "Zilean double Q", Evade = false,
                                     DoT = false, EvadeTimer = 0, Cleanse = true, CleanseTimer = 0, Slot = SpellSlot.Q
                                 }
                         };

            Spells =
                Spells.Where(x => !x.QssIgnore)
                    .Where(
                        x =>
                        string.IsNullOrEmpty(x.Champion)
                        || HeroManager.Enemies.Any(
                            y => y.ChampionName.Equals(x.Champion, StringComparison.InvariantCultureIgnoreCase)))
                    .ToList();

            #endregion

            #region Item Data

            Items = new List<CleanseItem>
                        {
                            new CleanseItem
                                {
                                    Slot =
                                        () =>
                                        Player.GetSpellSlot("summonerboost") == SpellSlot.Unknown
                                            ? SpellSlot.Unknown
                                            : Player.GetSpellSlot("summonerboost"),
                                    WorksOn =
                                        new[]
                                            {
                                                BuffType.Blind, BuffType.Charm, BuffType.Flee, BuffType.Slow,
                                                BuffType.Polymorph, BuffType.Silence, BuffType.Snare, BuffType.Stun,
                                                BuffType.Taunt, BuffType.Damage
                                            },
                                    Priority = 2
                                },
                            new CleanseItem
                                {
                                    Slot = () =>
                                        {
                                            var slots = ItemData.Quicksilver_Sash.GetItem().Slots;
                                            return slots.Count == 0 ? SpellSlot.Unknown : slots[0];
                                        },
                                    WorksOn =
                                        new[]
                                            {
                                                BuffType.Blind, BuffType.Charm, BuffType.Flee, BuffType.Slow,
                                                BuffType.Polymorph, BuffType.Silence, BuffType.Snare,
                                                BuffType.Stun, BuffType.Taunt, BuffType.Damage,
                                                BuffType.CombatEnchancer
                                            },
                                    Priority = 0
                                },
                            new CleanseItem
                                {
                                    Slot = () =>
                                        {
                                            var slots = ItemData.Dervish_Blade.GetItem().Slots;
                                            return slots.Count == 0 ? SpellSlot.Unknown : slots[0];
                                        },
                                    WorksOn =
                                        new[]
                                            {
                                                BuffType.Blind, BuffType.Charm, BuffType.Flee, BuffType.Slow,
                                                BuffType.Polymorph, BuffType.Silence, BuffType.Snare,
                                                BuffType.Stun, BuffType.Taunt, BuffType.Damage,
                                                BuffType.CombatEnchancer
                                            },
                                    Priority = 0
                                },
                            new CleanseItem
                                {
                                    Slot = () =>
                                        {
                                            var slots = ItemData.Mercurial_Scimitar.GetItem().Slots;
                                            return slots.Count == 0 ? SpellSlot.Unknown : slots[0];
                                        },
                                    WorksOn =
                                        new[]
                                            {
                                                BuffType.Blind, BuffType.Charm, BuffType.Flee, BuffType.Slow,
                                                BuffType.Polymorph, BuffType.Silence, BuffType.Snare,
                                                BuffType.Stun, BuffType.Taunt, BuffType.Damage,
                                                BuffType.CombatEnchancer
                                            },
                                    Priority = 0
                                },
                            new CleanseItem
                                {
                                    Slot = () =>
                                        {
                                            var slots = ItemData.Mikaels_Crucible.GetItem().Slots;
                                            return slots.Count == 0 ? SpellSlot.Unknown : slots[0];
                                        },
                                    WorksOn =
                                        new[]
                                            {
                                                BuffType.Stun, BuffType.Snare, BuffType.Taunt, BuffType.Silence,
                                                BuffType.Slow, BuffType.CombatEnchancer
                                            },
                                    WorksOnAllies = true,
                                    Priority = 1
                                }
                        };

            #endregion

            Items = Items.OrderBy(x => x.Priority).ToList();

            Random = new Random(Environment.TickCount);
        }

        #endregion

        #region Delegates

        /// <summary>
        ///     A delegate that returns a <see cref="SpellSlot" />
        /// </summary>
        /// <returns>
        ///     <see cref="SpellSlot" />
        /// </returns>
        public delegate SpellSlot GetSlotDelegate();

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the items.
        /// </summary>
        /// <value>
        ///     The items.
        /// </value>
        public static List<CleanseItem> Items { get; set; }

        /// <summary>
        ///     Gets or sets the spells.
        /// </summary>
        /// <value>
        ///     The spells.
        /// </value>
        public static List<CleanseSpell> Spells { get; set; }

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
        ///     Gets or sets the random.
        /// </summary>
        /// <value>
        ///     The random.
        /// </value>
        private static Random Random { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        public static Menu rootMenu = ElUtilitySuite.Entry.menu;
        public static Menu cleanseMenu;

        public static bool getCheckBoxItem(string item)
        {
            return cleanseMenu[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(string item)
        {
            return cleanseMenu[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(string item)
        {
            return cleanseMenu[item].Cast<KeyBind>().CurrentValue;
        }

        public void CreateMenu(Menu rootMenu)
        {
            cleanseMenu = rootMenu.AddSubMenu("Cleanse/QSS", "CleanseV3");

            cleanseMenu.Add("CleanseActivated", new CheckBox("Use Cleanse"));
            cleanseMenu.AddSeparator();

            cleanseMenu.AddGroupLabel("Spells to QSS");
            foreach (var spell in Spells)
            {
                cleanseMenu.Add(spell.MenuName != null ? spell.MenuName.Replace(" ", string.Empty) : spell.Name, new CheckBox(string.IsNullOrEmpty(spell.MenuName) ? spell.Name : spell.MenuName));
            }

            cleanseMenu.AddSeparator();

            cleanseMenu.AddGroupLabel("Humanizer Delay");
            cleanseMenu.Add("CleanseMaxDelay", new Slider("Delay (MS)", 800, 0, 1500));
            cleanseMenu.AddSeparator();

        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            Game.OnUpdate += this.GameOnUpdate;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets the best cleanse item.
        /// </summary>
        /// <param name="ally">The ally.</param>
        /// <param name="buff">The buff.</param>
        /// <returns></returns>
        private static LeagueSharp.Common.Spell GetBestCleanseItem(GameObject ally, BuffInstance buff)
        {
            return (from item in Items.OrderBy(x => x.Priority) where item.WorksOn.Any(x => buff.Type.HasFlag(x)) where ally.IsMe || item.WorksOnAllies where item.Spell.IsInRange(ally) && item.Spell.Slot != SpellSlot.Unknown select item.Spell).FirstOrDefault();
        }

        /// <summary>
        ///     Fired when the game is updated.
        /// </summary>
        /// <param name="args">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private void GameOnUpdate(EventArgs args)
        {
            if (Player.IsDead || Player.IsInvulnerable || Player.HasBuffOfType(BuffType.SpellImmunity) || Player.HasBuffOfType(BuffType.Invulnerability))
            {
                return;
            }

            if (!getCheckBoxItem("CleanseActivated"))
            {
                return;
            }

            foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(x => x.IsAlly && x.IsValidTarget(800f)))
            {
                var ally1 = ally;
                foreach (var spell in Spells.Where(x => ally1.HasBuff(x.Name)))
                {
                    if (!getCheckBoxItem(spell.MenuName != null ? spell.MenuName.Replace(" ", string.Empty) : spell.Name))
                    {
                        continue;
                    }

                    var buff = ally.GetBuff(spell.Name);

                    if (
                        !((AIHeroClient)buff.Caster).ChampionName.Equals(
                            spell.Champion,
                            StringComparison.InvariantCultureIgnoreCase) && !string.IsNullOrEmpty(spell.Champion))
                    {
                        return;
                    }

                    var item = GetBestCleanseItem(ally, buff);

                    if (item == null)
                    {
                        continue;
                    }

                    var ally2 = ally;
                    LeagueSharp.Common.Utility.DelayAction.Add(
                        spell.CleanseTimer
                        + Random.Next(getSliderItem("CleanseMaxDelay")),
                        () =>
                        {
                            if (!ally2.HasBuff(buff.Name) || ally2.IsInvulnerable)
                            {
                                return;
                            }

                            if (item.Slot != SpellSlot.Unknown)
                            {
                                Player.Spellbook.CastSpell(item.Slot, ally2);
                            }
                        });
                }
            }
        }

        #endregion

        /// <summary>
        ///     An item/spell that can be used to cleanse a spell.
        /// </summary>
        internal class CleanseItem
        {
            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="CleanseSpell" /> class.
            /// </summary>
            public CleanseItem()
            {
                this.Range = float.MaxValue;
                this.WorksOnAllies = false;
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets or sets the priority.
            /// </summary>
            /// <value>
            ///     The priority.
            /// </value>
            public int Priority { get; set; }

            /// <summary>
            ///     Gets or sets the range.
            /// </summary>
            /// <value>
            ///     The range.
            /// </value>
            public float Range { get; set; }

            /// <summary>
            ///     Gets or sets the slot delegate.
            /// </summary>
            /// <value>
            ///     The slot delegate.
            /// </value>
            public GetSlotDelegate Slot { get; set; }

            /// <summary>
            ///     Gets or sets the spell.
            /// </summary>
            /// <value>
            ///     The spell.
            /// </value>
            public LeagueSharp.Common.Spell Spell
            {
                get
                {
                    return new LeagueSharp.Common.Spell(this.Slot(), this.Range);
                }
            }

            /// <summary>
            ///     Gets or sets what the spell works on.
            /// </summary>
            /// <value>
            ///     The buff types the spell works on.
            /// </value>
            public BuffType[] WorksOn { get; set; }

            /// <summary>
            ///     Gets or sets a value indicating whether the spell works on allies.
            /// </summary>
            /// <value>
            ///     <c>true</c> if the spell works on allies; otherwise, <c>false</c>.
            /// </value>
            public bool WorksOnAllies { get; set; }

            #endregion
        }

        /// <summary>
        ///     Represents a spell that cleanse can be used on.
        /// </summary>
        internal class CleanseSpell
        {
            #region Public Properties

            /// <summary>
            ///     Gets or sets the champion.
            /// </summary>
            /// <value>
            ///     The champion.
            /// </value>
            public string Champion { get; set; }

            /// <summary>
            ///     Gets or sets a value indicating whether this <see cref="CleanseSpell" /> is cleanse.
            /// </summary>
            /// <value>
            ///     <c>true</c> if cleanse; otherwise, <c>false</c>.
            /// </value>
            public bool Cleanse { get; set; }

            /// <summary>
            ///     Gets or sets the cleanse timer.
            /// </summary>
            /// <value>
            ///     The cleanse timer.
            /// </value>
            public int CleanseTimer { get; set; }

            /// <summary>
            ///     Gets or sets a value indicating whether the spell does damage over time.
            /// </summary>
            /// <value>
            ///     <c>true</c> if the spell does damage over time; otherwise, <c>false</c>.
            /// </value>
            public bool DoT { get; set; }

            /// <summary>
            ///     Gets or sets a value indicating whether this <see cref="CleanseSpell" /> can be evaded.
            /// </summary>
            /// <value>
            ///     <c>true</c> if the spell can be evaded; otherwise, <c>false</c>.
            /// </value>
            public bool Evade { get; set; }

            /// <summary>
            ///     Gets or sets the evade timer.
            /// </summary>
            /// <value>
            ///     The evade timer.
            /// </value>
            public int EvadeTimer { get; set; }

            /// <summary>
            ///     Gets or sets the incoming damage.
            /// </summary>
            /// <value>
            ///     The incoming damage.
            /// </value>
            public int IncomeDamage { get; set; }

            /// <summary>
            ///     Gets or sets the interval.
            /// </summary>
            /// <value>
            ///     The interval.
            /// </value>
            public double Interval { get; set; }

            /// <summary>
            ///     Gets or sets the name of the menu.
            /// </summary>
            /// <value>
            ///     The name of the menu.
            /// </value>
            public string MenuName { get; set; }

            /// <summary>
            ///     Gets or sets the name.
            /// </summary>
            /// <value>
            ///     The name.
            /// </value>
            public string Name { get; set; }

            /// <summary>
            ///     Gets or sets a value indicating whether QSS can be used on this spell.
            /// </summary>
            /// <value>
            ///     <c>true</c> if QSS can be used on this spell; otherwise, <c>false</c>.
            /// </value>
            public bool QssIgnore { get; set; }

            /// <summary>
            ///     Gets or sets the slot.
            /// </summary>
            /// <value>
            ///     The slot.
            /// </value>
            public SpellSlot Slot { get; set; }

            /// <summary>
            ///     Gets or sets the tick limiter.
            /// </summary>
            /// <value>
            ///     The tick limiter.
            /// </value>
            public int TickLimiter { get; set; }

            #endregion
        }
    }
}
