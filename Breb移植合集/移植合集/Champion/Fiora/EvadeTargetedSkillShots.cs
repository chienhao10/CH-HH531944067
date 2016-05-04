using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using LeagueSharp.Common.Data;
using ItemData = LeagueSharp.Common.Data.ItemData;
using FioraProject.Evade;

namespace FioraProject
{
    using static Program;
    using static GetTargets;
    using EloBuddy;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    #region Targeted Skillshot
    internal class EvadeTarget
    {
        #region Static Fields

        private static readonly List<Targets> DetectedTargets = new List<Targets>();

        private static readonly List<SpellData> Spells = new List<SpellData>();

        #endregion

        #region Methods

        public static Menu evadeMenu;

        internal static void Init()
        {
            LoadSpellData();

            Spells.RemoveAll(i => !HeroManager.Enemies.Any(a => string.Equals(a.ChampionName, i.ChampionName, StringComparison.InvariantCultureIgnoreCase)));

            evadeMenu = Program.Menu.AddSubMenu("Evade Targeted SkillShot", "EvadeTarget");
            evadeMenu.Add("W", new CheckBox("Use W"));
            evadeMenu.AddGroupLabel("Auto Attack");
            evadeMenu.Add("B", new CheckBox("Basic Attack", false));
            evadeMenu.Add("BHpU", new Slider("-> If Hp < (%)", 35));
            evadeMenu.Add("C", new CheckBox("Crit Attack", false));
            evadeMenu.Add("CHpU", new Slider("-> If Hp < (%)", 40));

            foreach (var spell in Spells.Where(i => HeroManager.Enemies.Any(a => string.Equals(a.ChampionName, i.ChampionName, StringComparison.InvariantCultureIgnoreCase))))
            {
                evadeMenu.Add(spell.MissileName, new CheckBox(spell.MissileName + " (" + spell.Slot + ")", false));
            }

            Game.OnUpdate += OnUpdateTarget;
            GameObject.OnCreate += ObjSpellMissileOnCreate;
            GameObject.OnDelete += ObjSpellMissileOnDelete;
        }

        private static void LoadSpellData()
        {
            Spells.Add(
                new SpellData { ChampionName = "Ahri", SpellNames = new[] { "ahrifoxfiremissiletwo" }, Slot = SpellSlot.W });
            Spells.Add(
                new SpellData { ChampionName = "Ahri", SpellNames = new[] { "ahritumblemissile" }, Slot = SpellSlot.R });
            Spells.Add(
                new SpellData { ChampionName = "Akali", SpellNames = new[] { "akalimota" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData { ChampionName = "Anivia", SpellNames = new[] { "frostbite" }, Slot = SpellSlot.E });
            Spells.Add(
                new SpellData { ChampionName = "Annie", SpellNames = new[] { "disintegrate" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Brand",
                    SpellNames = new[] { "brandconflagrationmissile" },
                    Slot = SpellSlot.E
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Brand",
                    SpellNames = new[] { "brandwildfire", "brandwildfiremissile" },
                    Slot = SpellSlot.R
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Caitlyn",
                    SpellNames = new[] { "caitlynaceintheholemissile" },
                    Slot = SpellSlot.R
                });
            Spells.Add(
                new SpellData { ChampionName = "Cassiopeia", SpellNames = new[] { "cassiopeiatwinfang" }, Slot = SpellSlot.E });
            Spells.Add(
                new SpellData { ChampionName = "Elise", SpellNames = new[] { "elisehumanq" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ezreal",
                    SpellNames = new[] { "ezrealarcaneshiftmissile" },
                    Slot = SpellSlot.E
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "FiddleSticks",
                    SpellNames = new[] { "fiddlesticksdarkwind", "fiddlesticksdarkwindmissile" },
                    Slot = SpellSlot.E
                });
            Spells.Add(
                new SpellData { ChampionName = "Gangplank", SpellNames = new[] { "parley" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData { ChampionName = "Janna", SpellNames = new[] { "sowthewind" }, Slot = SpellSlot.W });
            Spells.Add(
                new SpellData { ChampionName = "Kassadin", SpellNames = new[] { "nulllance" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Katarina",
                    SpellNames = new[] { "katarinaq", "katarinaqmis" },
                    Slot = SpellSlot.Q
                });
            Spells.Add(
                new SpellData { ChampionName = "Kayle", SpellNames = new[] { "judicatorreckoning" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Leblanc",
                    SpellNames = new[] { "leblancchaosorb", "leblancchaosorbm" },
                    Slot = SpellSlot.Q
                });
            Spells.Add(new SpellData { ChampionName = "Lulu", SpellNames = new[] { "luluw" }, Slot = SpellSlot.W });
            Spells.Add(
                new SpellData { ChampionName = "Malphite", SpellNames = new[] { "seismicshard" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "MissFortune",
                    SpellNames = new[] { "missfortunericochetshot", "missFortunershotextra" },
                    Slot = SpellSlot.Q
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Nami",
                    SpellNames = new[] { "namiwenemy", "namiwmissileenemy" },
                    Slot = SpellSlot.W
                });
            Spells.Add(
                new SpellData { ChampionName = "Nunu", SpellNames = new[] { "iceblast" }, Slot = SpellSlot.E });
            Spells.Add(
                new SpellData { ChampionName = "Pantheon", SpellNames = new[] { "pantheonq" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ryze",
                    SpellNames = new[] { "spellflux", "spellfluxmissile" },
                    Slot = SpellSlot.E
                });
            Spells.Add(
                new SpellData { ChampionName = "Shaco", SpellNames = new[] { "twoshivpoison" }, Slot = SpellSlot.E });
            Spells.Add(
                new SpellData { ChampionName = "Shen", SpellNames = new[] { "shenvorpalstar" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData { ChampionName = "Sona", SpellNames = new[] { "sonaqmissile" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData { ChampionName = "Swain", SpellNames = new[] { "swaintorment" }, Slot = SpellSlot.E });
            Spells.Add(
                new SpellData { ChampionName = "Syndra", SpellNames = new[] { "syndrar" }, Slot = SpellSlot.R });
            Spells.Add(
                new SpellData { ChampionName = "Taric", SpellNames = new[] { "dazzle" }, Slot = SpellSlot.E });
            Spells.Add(
                new SpellData { ChampionName = "Teemo", SpellNames = new[] { "blindingdart" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData { ChampionName = "Tristana", SpellNames = new[] { "detonatingshot" }, Slot = SpellSlot.E });
            Spells.Add(
                new SpellData { ChampionName = "Tristana", SpellNames = new[] { "tristanar" }, Slot = SpellSlot.R });
            Spells.Add(
                new SpellData { ChampionName = "TwistedFate", SpellNames = new[] { "bluecardattack" }, Slot = SpellSlot.W });
            Spells.Add(
                new SpellData { ChampionName = "TwistedFate", SpellNames = new[] { "goldcardattack" }, Slot = SpellSlot.W });
            Spells.Add(
                new SpellData { ChampionName = "TwistedFate", SpellNames = new[] { "redcardattack" }, Slot = SpellSlot.W });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Urgot",
                    SpellNames = new[] { "urgotheatseekinghomemissile" },
                    Slot = SpellSlot.Q
                });
            Spells.Add(
                new SpellData { ChampionName = "Vayne", SpellNames = new[] { "vaynecondemnmissile" }, Slot = SpellSlot.E });
            Spells.Add(
                new SpellData { ChampionName = "Veigar", SpellNames = new[] { "veigarprimordialburst" }, Slot = SpellSlot.R });
            Spells.Add(
                new SpellData { ChampionName = "Viktor", SpellNames = new[] { "viktorpowertransfer" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Vladimir",
                    SpellNames = new[] { "vladimirtidesofbloodnuke" },
                    Slot = SpellSlot.E
                });
        }

        private static void ObjSpellMissileOnCreate(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;
            if (missile == null || !missile.IsValid)
            {
                return;
            }
            var caster = missile.SpellCaster as AIHeroClient;
            if (caster == null || !caster.IsValid || caster.Team == ObjectManager.Player.Team || !(missile.Target != null && missile.Target.IsMe))
            {
                return;
            }

            var spellData = Spells.FirstOrDefault(i => i.SpellNames.Contains(missile.SData.Name.ToLower()) && evadeMenu[i.MissileName] != null ? getCheckBoxItem(evadeMenu, i.MissileName) : false);
            if (spellData == null && Orbwalking.IsAutoAttack(missile.SData.Name) && (!missile.SData.Name.ToLower().Contains("crit")
                        ? getCheckBoxItem(evadeMenu, "B")
                          && ObjectManager.Player.HealthPercent < getSliderItem(evadeMenu, "BHpU")
                        : getCheckBoxItem(evadeMenu, "C")
                          && ObjectManager.Player.HealthPercent < getSliderItem(evadeMenu, "CHpU")))
            {
                spellData = new SpellData { ChampionName = caster.ChampionName, SpellNames = new[] { missile.SData.Name } };
            }
            if (spellData == null)
            {
                return;
            }
            DetectedTargets.Add(new Targets { Start = caster.ServerPosition, Obj = missile });
        }

        private static void ObjSpellMissileOnDelete(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;
            if (missile == null || !missile.IsValid)
            {
                return;
            }
            var caster = missile.SpellCaster as AIHeroClient;
            if (caster == null || !caster.IsValid || caster.Team == ObjectManager.Player.Team)
            {
                return;
            }
            DetectedTargets.RemoveAll(i => i.Obj.NetworkId == missile.NetworkId);
        }

        private static void OnUpdateTarget(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }
            if (ObjectManager.Player.HasBuffOfType(BuffType.SpellShield) || ObjectManager.Player.HasBuffOfType(BuffType.SpellImmunity))
            {
                return;
            }
            if (!getCheckBoxItem(evadeMenu, "W") || !W.IsReady())
            {
                return;
            }
            foreach (var target in
                DetectedTargets.Where(i => W.IsInRange(i.Obj, 150 + Game.Ping * i.Obj.SData.MissileSpeed / 1000)).OrderBy(i => i.Obj.Position.LSDistance(ObjectManager.Player.Position)))
            {
                var tar = GetTarget(W.Range);
                if (tar.LSIsValidTarget(W.Range))
                    ObjectManager.Player.Spellbook.CastSpell(SpellSlot.W, tar.Position);
                else
                {
                    var hero = HeroManager.Enemies.FirstOrDefault(x => x.LSIsValidTarget(W.Range));
                    if (hero != null)
                        ObjectManager.Player.Spellbook.CastSpell(SpellSlot.W, hero.Position);
                    else
                        ObjectManager.Player.Spellbook.CastSpell(SpellSlot.W, ObjectManager.Player.ServerPosition.LSExtend(target.Start, 100));
                }
            }
        }

        #endregion

        private class SpellData
        {
            #region Fields

            public string ChampionName;

            public SpellSlot Slot;

            public string[] SpellNames = { };

            #endregion

            #region Public Properties

            public string MissileName
            {
                get
                {
                    return this.SpellNames.First();
                }
            }

            #endregion
        }

        private class Targets
        {
            #region Fields

            public MissileClient Obj;

            public Vector3 Start;

            #endregion
        }
    }
    #endregion Targeted Skillshot

}
