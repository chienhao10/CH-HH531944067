using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using JokerFioraBuddy.Evade;

using Settings = JokerFioraBuddy.Config.ShieldBlock;

namespace JokerFioraBuddy
{
    public static class SpellBlock
    {
        public static readonly Dictionary<string, List<BlockedSpell>> BlockedSpells =
            new Dictionary<string, List<BlockedSpell>>();


        static SpellBlock()
        {
            const SpellSlot N48 = (SpellSlot)48;

            var q = new BlockedSpell(SpellSlot.Q);
            var w = new BlockedSpell(SpellSlot.W);
            var e = new BlockedSpell(SpellSlot.E);
            var r = new BlockedSpell(SpellSlot.R);

            BlockedSpells.Add(
                 "Aatrox",
                 new List<BlockedSpell>
                {
                    new BlockedSpell("aatroxwonhattacklife", "Blood Thirst", true),
                    new BlockedSpell("aatroxwonhattackpower", "Blood Price", true),
                    r
                });
            BlockedSpells.Add("Akali", new List<BlockedSpell> { q, e, r }); //
            BlockedSpells.Add("Alistar", new List<BlockedSpell> { q, w });
            BlockedSpells.Add("Anivia", new List<BlockedSpell> { e }); //
            BlockedSpells.Add("Annie", new List<BlockedSpell> { q, r }); //
            BlockedSpells.Add("Azir", new List<BlockedSpell> { r });
            BlockedSpells.Add("Bard", new List<BlockedSpell> { r });
            BlockedSpells.Add(
                "Blitzcrank", new List<BlockedSpell> { new BlockedSpell("PowerFistAttack", "Power Fist", true) });
            BlockedSpells.Add("Brand", new List<BlockedSpell> { e, r });
            BlockedSpells.Add(
                "Braum", new List<BlockedSpell> { new BlockedSpell("BraumBasicAttackPassiveOverride", "Stun", true) });
            BlockedSpells.Add(
                "Caitlyn", new List<BlockedSpell> { new BlockedSpell("CaitlynHeadshotMissile", "Headshot", true), r });
            BlockedSpells.Add("Chogath", new List<BlockedSpell> { r });
            BlockedSpells.Add(
                "Darius",
                new List<BlockedSpell>
                {
                    q,
                    new BlockedSpell("DariusNoxianTacticsONHAttack", "Empowered W", true),
                    e,
                    r
                });
            BlockedSpells.Add(
                "Diana",
                new List<BlockedSpell> { new BlockedSpell("DianaBasicAttack3", "Moonsilver Blade", true), e, r });
            BlockedSpells.Add(
                "DrMundo", new List<BlockedSpell> { new BlockedSpell("MasochismAttack", "Empowered E", true) });
            BlockedSpells.Add(
                "Ekko",
                new List<BlockedSpell>
                {
                    new BlockedSpell("EkkoEAttack", "Empowered E", true),
                    new BlockedSpell("ekkobasicattackp3", "Third Proc Passive", true)
                });
            BlockedSpells.Add("Elise", new List<BlockedSpell> { q });
            BlockedSpells.Add("Evelynn", new List<BlockedSpell> { e });
            BlockedSpells.Add("FiddleSticks", new List<BlockedSpell> { q, w, e });
            BlockedSpells.Add(
                "Fiora", new List<BlockedSpell> { new BlockedSpell("FioraEAttack", "Empowered First E", true) });
            BlockedSpells.Add("Fizz", new List<BlockedSpell> { q, new BlockedSpell("fizzjumptwo", "Second E") });
            BlockedSpells.Add(
                "Gangplank", new List<BlockedSpell> { q, new BlockedSpell((SpellSlot)45) { Name = "Barrel Q" } });
            BlockedSpells.Add(
                "Garen", new List<BlockedSpell> { new BlockedSpell("GarenQAttack", "Empowered Q", true), r });
            BlockedSpells.Add(
                "Gnar",
                new List<BlockedSpell>
                {
                    new BlockedSpell("GnarBasicAttack", "Empowered W", true)
                    {
                        BuffName = "gnarwproc",
                        IsPlayerBuff = true
                    }
                });
            BlockedSpells.Add(
                "Gragas", new List<BlockedSpell> { new BlockedSpell("DrunkenRage", "Drunken Rage", true) });
            BlockedSpells.Add(
                "Hecarim", new List<BlockedSpell> { new BlockedSpell("hecarimrampattack", "Empowered E", true), r });
            BlockedSpells.Add(
                "Illaoi", new List<BlockedSpell> { new BlockedSpell("illaoiwattack", "Empowered W", true) });
            BlockedSpells.Add("Irelia", new List<BlockedSpell> { q, e });
            BlockedSpells.Add("Janna", new List<BlockedSpell> { w });
            BlockedSpells.Add(
                "JarvanIV",
                new List<BlockedSpell> { new BlockedSpell("JarvanIVMartialCadenceAttack", "Martial Cadence", true), r });
            BlockedSpells.Add(
                "Jax",
                new List<BlockedSpell>
                {
                    new BlockedSpell("JaxBasicAttack", "Empowered", true)
                    {
                        BuffName = "JaxEmpowerTwo",
                        IsSelfBuff = true
                    },
                    q
                    //new BlockedSpell(SpellSlot.E) { BuffName = "JaxCounterStrike", IsSelfBuff = true }
                });
            BlockedSpells.Add(
                "Jayce",
                new List<BlockedSpell>
                {
                    new BlockedSpell("JayceToTheSkies", "Hammer Q"),
                    new BlockedSpell("JayceThunderingBlow", "Hammer E")
                });
            BlockedSpells.Add("Jhin", new List<BlockedSpell> { q, new BlockedSpell("JhinPassiveAttack", "4th", true) });
            BlockedSpells.Add(
                "Kassadin", new List<BlockedSpell> { q, new BlockedSpell("KassadinBasicAttack3", "Empowered W", true) });
            BlockedSpells.Add("Katarina", new List<BlockedSpell> { e });
            BlockedSpells.Add("Kayle", new List<BlockedSpell> { q });
            BlockedSpells.Add(
                "Kennen", new List<BlockedSpell> { new BlockedSpell("KennenMegaProc", "Empowered", true), w });
            BlockedSpells.Add("Khazix", new List<BlockedSpell> { q });
            BlockedSpells.Add("Kindred", new List<BlockedSpell> { e });
            //new BlockedSpell((SpellSlot) 48) { SpellName = "kindredbasicattackoverridelightbombfinal", Name = "Empowered E" } });
            BlockedSpells.Add("Leblanc", new List<BlockedSpell> { q, new BlockedSpell("LeblancChaosOrbM", "Block RQ") });
            BlockedSpells.Add(
                "LeeSin",
                new List<BlockedSpell>
                {
                    new BlockedSpell("blindmonkqtwo", "Second Q")
                    {
                        BuffName = "blindmonkqonechaos",
                        IsPlayerBuff = true
                    },
                    new BlockedSpell("BlindMonkEOne", "First E"),
                    r
                });
            BlockedSpells.Add(
                "Leona", new List<BlockedSpell> { new BlockedSpell("LeonaShieldOfDaybreakAttack", "Stun Q", true) });
            BlockedSpells.Add("Lissandra", new List<BlockedSpell> { new BlockedSpell(N48) { Name = "R" } });
            BlockedSpells.Add("Lulu", new List<BlockedSpell> { w });
            BlockedSpells.Add("Malphite", new List<BlockedSpell> { q, e });
            BlockedSpells.Add("Malzahar", new List<BlockedSpell> { e, r });
            BlockedSpells.Add("Maokai", new List<BlockedSpell> { w });
            BlockedSpells.Add(
                "MasterYi", new List<BlockedSpell> { q, new BlockedSpell("MasterYiDoubleStrike", "Empowered", true) });
            BlockedSpells.Add("MissFortune", new List<BlockedSpell> { q });
            BlockedSpells.Add(
                "MonkeyKing", new List<BlockedSpell> { new BlockedSpell("MonkeyKingQAttack", "Empowered Q", true), e });
            BlockedSpells.Add(
                "Mordekaiser",
                new List<BlockedSpell> { new BlockedSpell("mordekaiserqattack2", "Empowered Q", true), r });
            BlockedSpells.Add("Nami", new List<BlockedSpell> { w });
            BlockedSpells.Add(
                "Nasus", new List<BlockedSpell> { new BlockedSpell("NasusQAttack", "Empowered Q", true), q });
            BlockedSpells.Add(
                "Nautilus",
                new List<BlockedSpell> { new BlockedSpell("NautilusRavageStrikeAttack", "Empowered", true), e, r });
            BlockedSpells.Add(
                "Nidalee",
                new List<BlockedSpell>
                {
                    new BlockedSpell("NidaleeTakedownAttack", "Cougar Q", true) { ModelName = "nidalee_cougar" },
                    new BlockedSpell(SpellSlot.W)
                    {
                        ModelName = "nidalee_cougar",
                        BuffName = "nidaleepassivehunted",
                        IsPlayerBuff = true,
                        Name = "Cougar W"
                    }
                });
            BlockedSpells.Add("Nocturne", new List<BlockedSpell> { r });
            BlockedSpells.Add("Nunu", new List<BlockedSpell> { e });
            BlockedSpells.Add("Olaf", new List<BlockedSpell> { e });
            BlockedSpells.Add("Pantheon", new List<BlockedSpell> { q, w });
            BlockedSpells.Add(
                "Poppy",
                new List<BlockedSpell>
                {
                    new BlockedSpell("PoppyPassiveAttack", "Passive Attack", true)
                    {
                        BuffName = "poppypassivebuff",
                        IsSelfBuff = true
                    },
                    e
                });
            BlockedSpells.Add(
                "Quinn", new List<BlockedSpell> { new BlockedSpell("QuinnWEnhanced", "Empowered", true), e });
            BlockedSpells.Add("Rammus", new List<BlockedSpell> { e });
            BlockedSpells.Add(
                "RekSai",
                new List<BlockedSpell>
                {
                    new BlockedSpell("reksaiwburrowed", "W"),
                    new BlockedSpell("reksaie", "E") { UseContains = false }
                });
            BlockedSpells.Add(
                "Renekton",
                new List<BlockedSpell>
                {
                    q,
                    new BlockedSpell("RenektonExecute", "Empowered W", true),
                    new BlockedSpell("RenektonSuperExecute", "Fury Empowered W", true)
                });
            BlockedSpells.Add(
                "Rengar", new List<BlockedSpell> { new BlockedSpell("RengarBasicAttack", "Empowered Q", true) });
            BlockedSpells.Add(
                "Riven",
                new List<BlockedSpell>
                {
                    new BlockedSpell(SpellSlot.Q) { Name = "Third Q", BuffName = "RivenTriCleave", IsSelfBuff = true },
                    w
                });
            BlockedSpells.Add("Ryze", new List<BlockedSpell> { w, e });
            BlockedSpells.Add("Shaco", new List<BlockedSpell> { q, e });
            BlockedSpells.Add(
                "Shen",
                new List<BlockedSpell>
                {
                    new BlockedSpell("ShenQAttack", "Empowered", true) { BuffName = "shenqbuff", IsSelfBuff = true }
                });
            BlockedSpells.Add(
                "Shyvana", new List<BlockedSpell> { new BlockedSpell("ShyvanaDoubleAttackHit", "Empowered Q", true) });
            BlockedSpells.Add("Singed", new List<BlockedSpell> { e });
            BlockedSpells.Add("Sion", new List<BlockedSpell> { q, r });
            BlockedSpells.Add("Skarner", new List<BlockedSpell> { r });
            BlockedSpells.Add("Syndra", new List<BlockedSpell> { r });
            BlockedSpells.Add("Swain", new List<BlockedSpell> { q, e });
            BlockedSpells.Add(
                "Talon",
                new List<BlockedSpell>
                {
                    new BlockedSpell("TalonNoxianDiplomacyAttack", "Empowered Q") { IsAutoAttack = true },
                    e
                });
            BlockedSpells.Add("Taric", new List<BlockedSpell> { e });
            BlockedSpells.Add("Teemo", new List<BlockedSpell> { q });
            BlockedSpells.Add("Tristana", new List<BlockedSpell> { e, r });
            BlockedSpells.Add(
                "Trundle", new List<BlockedSpell> { new BlockedSpell("TrundleQ", "Empowered Q", true), r });
            BlockedSpells.Add(
                "TwistedFate", new List<BlockedSpell> { new BlockedSpell("goldcardpreattack", "Gold Card", true) });
            BlockedSpells.Add("Udyr", new List<BlockedSpell> { new BlockedSpell("UdyrBearAttack", "Bear", true) });
            BlockedSpells.Add("Urgot", new List<BlockedSpell> { r });
            BlockedSpells.Add(
                "Vayne",
                new List<BlockedSpell>
                {
                    new BlockedSpell("VayneBasicAttack", "Silver Bolts")
                    {
                        IsAutoAttack = true,
                        IsPlayerBuff = true,
                        BuffName = "vaynesilvereddebuff",
                        BuffCount = 2
                    },
                    e
                });
            BlockedSpells.Add("Veigar", new List<BlockedSpell> { r });
            BlockedSpells.Add(
                "Vi",
                new List<BlockedSpell>
                {
                    new BlockedSpell("ViBasicAttack", "Empowered W")
                    {
                        IsAutoAttack = true,
                        BuffName = "viwproc",
                        IsPlayerBuff = true,
                        BuffCount = 2
                    },
                    new BlockedSpell("ViEAttack", "Empowered E") { IsAutoAttack = true },
                    r
                });
            BlockedSpells.Add(
                "Viktor",
                new List<BlockedSpell> { q, new BlockedSpell("viktorqbuff", "Empowered Q") { IsAutoAttack = true } });
            BlockedSpells.Add("Vladimir", new List<BlockedSpell> { q });
            BlockedSpells.Add(
                "Volibear", new List<BlockedSpell> { new BlockedSpell("VolibearQAttack", "Empowered Q", true), w });
            BlockedSpells.Add("Warwick", new List<BlockedSpell> { q });
            BlockedSpells.Add(
                "XinZhao", new List<BlockedSpell> { new BlockedSpell("XenZhaoThrust3", "Empowered Q", true), e, r });
            BlockedSpells.Add("Yasuo", new List<BlockedSpell> { new BlockedSpell("yasuoq3", "Whirlwind Q"), e });
            BlockedSpells.Add(
                "Yorick",
                new List<BlockedSpell>
                {
                    new BlockedSpell("yorickbasicattack", "Empowered Q")
                    {
                        IsAutoAttack = true,
                        BuffName = "YorickSpectral",
                        IsSelfBuff = true
                    },
                    e
                });
            BlockedSpells.Add("Zac", new List<BlockedSpell> { w, r });
            BlockedSpells.Add("Zilean", new List<BlockedSpell> { e });

            var enemies = EntityManager.Heroes.Enemies;

            if (enemies.Any(o => o.ChampionName.Equals("Kalista")))
                Config.ShieldBlock.Menu.Add("Oathsworn", new CheckBox("Block Oathsworn Knockup (Kalista R)", true));

            foreach (var unit in enemies)
            {
                if (!SpellBlock.BlockedSpells.ContainsKey(unit.ChampionName))
                    continue;

                var name = unit.ChampionName.Equals("MonkeyKing") ? "Wukong" : unit.ChampionName;

                foreach (var spell in SpellBlock.BlockedSpells[unit.ChampionName])
                {
                    var slot = spell.Slot.Equals(48) ? SpellSlot.R : spell.Slot;
                    Config.ShieldBlock.Menu.Add(unit.ChampionName + spell.MenuName, new CheckBox(unit.ChampionName + " - " + spell.DisplayName, true));
                }
            }

            Game.OnUpdate += OnUpdate;
        }

        static void OnUpdate(EventArgs args)
        {
            if (!Settings.BlockSpells || !SpellManager.W.IsReady())
                return;

            foreach (var skillshot in Evade.Evade.GetSkillshotsAboutToHit(Player.Instance, (int)(SpellManager.W.CastDelay + Game.Ping / 2f)))
            {
                if (!SpellManager.W.IsReady())
                    return;

                var enemy = skillshot.Unit as AIHeroClient;

                if (enemy == null)
                    continue;

                var spells = new List<BlockedSpell>();

                BlockedSpells.TryGetValue(enemy.ChampionName, out spells);


                if (spells == null || spells.Count == 0)
                    continue;

                foreach (var spell in spells)
                {
                    var item = Config.ShieldBlock.Menu[enemy.ChampionName + spell.MenuName];

                    if (item == null || !item.Cast<CheckBox>().CurrentValue)
                        continue;
                   
                    if (!spell.PassesSlotCondition(skillshot.SpellData.Slot))
                        continue;
                   
                    if (!spell.PassesBuffCondition(enemy) || !spell.PassesModelCondition(enemy))
                        continue;
                    if (!spell.PassesSpellCondition(skillshot.SpellData.SpellName))
                        continue;

                    if (Config.ShieldBlock.EvadeIntegration)
                    Program.CastW(skillshot.Unit);

                }
            }
        }
    
        public static bool Contains(AIHeroClient unit, GameObjectProcessSpellCastEventArgs args)
        {
            var name = unit.ChampionName;
            var slot = args.Slot.Equals(48) ? SpellSlot.R : args.Slot;

            if (args.SData.Name.Equals("KalistaRAllyDash") && Config.ShieldBlock.Menu["Oathsworn"].Cast<CheckBox>().CurrentValue)
            {
                return true;
            }
            var spells = new List<BlockedSpell>();
            BlockedSpells.TryGetValue(name, out spells);

            if (spells == null || spells.Count == 0)
            {
                return false;
            }

            foreach (var spell in
                spells)
            {
                var item = Config.ShieldBlock.Menu[name + spell.MenuName];
                if (item == null || !item.Cast<CheckBox>().CurrentValue)
                {
                    continue;
                }

                if (!spell.PassesModelCondition(unit))
                {
                    continue;
                }

                if (!spell.PassesSpellCondition(args.SData.Name))
                {
                    continue;
                }

                if (!spell.PassesBuffCondition(unit))
                {
                    continue;
                }

                if (!spell.PassesSlotCondition(args.Slot))
                {
                    continue;
                }

                if (spell.IsAutoAttack)
                {
                    if (spell.SpellName.Equals("-1"))
                    {
                        Console.WriteLine(args.SData.Name);
                    }

                    var condition = true;

                    if (unit.ChampionName.Equals("Gnar"))
                    {
                        var buff = ObjectManager.Player.Buffs.FirstOrDefault(b => b.Name.Equals("gnarwproc"));
                        condition = buff != null && buff.Count == 2;
                    }
                    else if (unit.ChampionName.Equals("Rengar"))
                    {
                        condition = unit.Mana.Equals(5);
                    }

                    if (condition)
                    {
                        return true;
                    }

                    continue;
                }

                if (name.Equals("Riven") && args.SData.Name == "RivenTriCleave")
                {
                    return unit.GetBuffCount("RivenTriCleave").Equals(2);
                }

                return true;
            }

            return false;
        }
    

        public static void Initialize()
        {
 
        }
    }

    public class BlockedSpell
    {
        public int BuffCount;
        public string BuffName;
        public string DisplayName;
        public bool Enabled;
        public bool IsAutoAttack;
        public bool IsPlayerBuff;
        public bool IsSelfBuff;
        public string MenuName;
        public string ModelName;
        public string Name;
        public SpellSlot Slot = SpellSlot.Unknown;
        public string SpellName;
        public bool UseContains = true;

        public BlockedSpell(string spellName, string displayName, bool isAutoAttack = false, bool enabled = true)
        {
            SpellName = spellName.ToLower();
            Name = displayName;
            IsAutoAttack = isAutoAttack;
            SetMenuData();
        }

        public BlockedSpell(SpellSlot slot)
        {
            Slot = slot;
            SetMenuData();
        }

        public bool HasBuffCondition
        {
            get { return !string.IsNullOrWhiteSpace(BuffName); }
        }

        public bool HasModelCondition
        {
            get { return !string.IsNullOrWhiteSpace(ModelName); }
        }

        public bool HasSpellCondition
        {
            get { return !string.IsNullOrWhiteSpace(SpellName); }
        }

        public bool HasSlotCondition
        {
            get { return !Slot.Equals(SpellSlot.Unknown); }
        }

        private void SetMenuData()
        {
            var menuName = "";
            var display = "Block ";

            if (HasSpellCondition)
            {
                menuName += SpellName;
            }
            else if (!Slot.Equals(SpellSlot.Unknown))
            {
                menuName += Slot;
            }

            if (!string.IsNullOrWhiteSpace(Name))
            {
                display += " " + Name;
            }
            else if (!Slot.Equals(SpellSlot.Unknown))
            {
                display += Slot;
            }

            if (IsAutoAttack)
            {
                display += " AA";
            }

            MenuName = menuName;
            DisplayName = display;
        }

        public bool PassesModelCondition(AIHeroClient hero)
        {
            return !HasModelCondition || hero.BaseSkinName.Equals(ModelName);
        }

        public bool PassesBuffCondition(AIHeroClient hero)
        {
            if (!HasBuffCondition)
            {
                return true;
            }

            var unit = IsSelfBuff ? hero : ObjectManager.Player;

            if (BuffName.Equals("-1"))
            {
                foreach (var buff in unit.Buffs)
                {
                    Console.WriteLine(buff.Name + " " + buff.Count);
                }
            }
            return BuffCount == 0 ? unit.HasBuff(BuffName) : unit.GetBuffCount(BuffName).Equals(BuffCount);
        }

        public bool PassesSlotCondition(SpellSlot slot)
        {
            return !HasSlotCondition || Slot.Equals(slot);
        }

        public bool PassesSpellCondition(string spell)
        {
            spell = spell.ToLower();
            if (UseContains)
            {
                return !HasSpellCondition || spell.ToLower().Contains(SpellName);
            }
            return !HasSpellCondition || spell.ToLower().Equals(SpellName);
        }
    }
 }
