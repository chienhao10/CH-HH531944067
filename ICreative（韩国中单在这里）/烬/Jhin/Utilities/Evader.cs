using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using Jhin;
using Jhin.Champions;
using Jhin.Managers;
using Jhin.Model;
using SharpDX;

namespace Jhin.Utilities
{
    public class EvaderArgs : EventArgs
    {
        public MissileClient Missile;
        public SpellData SData;
        public GameObjectProcessSpellCastEventArgs Spell;
        public AIHeroClient Sender { get; set; }
        public int StartTick { get; set; }

        public Vector3 EndPosition
        {
            get { return Spell.End; }
        }

        public bool ContainsMissile
        {
            get { return SData.MissileEffectName.Contains("_mis.troy") || SData.MissileEffectName.Contains("_mis_"); }
        }
    }

    public static class Evader
    {
        public delegate void EvaderHandler(EvaderArgs e);

        public static HashSet<string> SpellsAddedToMenu = new HashSet<string>();
        public static List<EvaderArgs> EvaderList = new List<EvaderArgs>();

        public static HashSet<SpellSlot> AllowedSlots = new HashSet<SpellSlot>
        {
            SpellSlot.Q,
            SpellSlot.W,
            SpellSlot.E,
            SpellSlot.R
        };

        public static Dictionary<Champion, SpellSlot> DangerousSpellSlots = new Dictionary<Champion, SpellSlot>
        {
            {Champion.Ezreal, SpellSlot.R},
            {Champion.Graves, SpellSlot.R},
            {Champion.Jinx, SpellSlot.R},
            {Champion.LeeSin, SpellSlot.Q},
            {Champion.Lux, SpellSlot.R},
            {Champion.Nidalee, SpellSlot.Q},
            {Champion.Riven, SpellSlot.R},
            {Champion.Syndra, SpellSlot.R},
            {Champion.Xerath, SpellSlot.R},
            {Champion.Yasuo, SpellSlot.Q},
            {Champion.Zed, SpellSlot.R}
        };

        public static Dictionary<Champion, string> DangerousSpellNames = new Dictionary<Champion, string>
        {
            {Champion.LeeSin, "BlindMonkQOne"},
            {Champion.TwistedFate, "GoldCardAttack"},
            {Champion.Yasuo, "yasuoq3w"},
            {Champion.Zed, "ZedR"}
        };

        public static Dictionary<Champion, HashSet<SpellSlot>> CrowdControlSpellSlots = new Dictionary
            <Champion, HashSet<SpellSlot>>
        {
            {Champion.Ahri, new HashSet<SpellSlot> {SpellSlot.E}},
            {Champion.Alistar, new HashSet<SpellSlot> {SpellSlot.Q, SpellSlot.W}},
            {Champion.Amumu, new HashSet<SpellSlot> {SpellSlot.Q, SpellSlot.R}},
            {Champion.Anivia, new HashSet<SpellSlot> {SpellSlot.Q}},
            {Champion.Annie, new HashSet<SpellSlot> {SpellSlot.R}},
            {Champion.Ashe, new HashSet<SpellSlot> {SpellSlot.R}},
            {Champion.Bard, new HashSet<SpellSlot> {SpellSlot.Q}},
            {Champion.Blitzcrank, new HashSet<SpellSlot> {SpellSlot.Q}},
            {Champion.Brand, new HashSet<SpellSlot> {SpellSlot.Q}},
            {Champion.Braum, new HashSet<SpellSlot> {SpellSlot.Q}},
            {Champion.Cassiopeia, new HashSet<SpellSlot> {SpellSlot.R}},
            {Champion.Darius, new HashSet<SpellSlot> {SpellSlot.E}},
            {Champion.Draven, new HashSet<SpellSlot> {SpellSlot.E}},
            {Champion.DrMundo, new HashSet<SpellSlot> {SpellSlot.Q}},
            {Champion.Ekko, new HashSet<SpellSlot> {SpellSlot.W}},
            {Champion.Elise, new HashSet<SpellSlot> {SpellSlot.E}},
            {Champion.Evelynn, new HashSet<SpellSlot> {SpellSlot.R}},
            {Champion.Fizz, new HashSet<SpellSlot> {SpellSlot.R}},
            {Champion.Galio, new HashSet<SpellSlot> {SpellSlot.R}},
            {Champion.Gnar, new HashSet<SpellSlot> {SpellSlot.R}},
            {Champion.Gragas, new HashSet<SpellSlot> {SpellSlot.R}},
            {Champion.Heimerdinger, new HashSet<SpellSlot> {SpellSlot.E}},
            {Champion.Irelia, new HashSet<SpellSlot> {SpellSlot.E}},
            {Champion.Jax, new HashSet<SpellSlot> {SpellSlot.E}},
            {Champion.Jinx, new HashSet<SpellSlot> {SpellSlot.W}},
            {Champion.Khazix, new HashSet<SpellSlot> {SpellSlot.W}},
            {Champion.Leblanc, new HashSet<SpellSlot> {SpellSlot.E}},
            {Champion.Leona, new HashSet<SpellSlot> {SpellSlot.E, SpellSlot.R}},
            {Champion.Lissandra, new HashSet<SpellSlot> {SpellSlot.W, SpellSlot.R}},
            {Champion.Lux, new HashSet<SpellSlot> {SpellSlot.Q}},
            {Champion.Malphite, new HashSet<SpellSlot> {SpellSlot.R}},
            {Champion.Morgana, new HashSet<SpellSlot> {SpellSlot.Q}},
            {Champion.Nami, new HashSet<SpellSlot> {SpellSlot.Q}},
            {Champion.Nautilus, new HashSet<SpellSlot> {SpellSlot.Q}},
            {Champion.Orianna, new HashSet<SpellSlot> {SpellSlot.R}},
            {Champion.Pantheon, new HashSet<SpellSlot> {SpellSlot.W}},
            {Champion.Quinn, new HashSet<SpellSlot> {SpellSlot.E}},
            {Champion.Renekton, new HashSet<SpellSlot> {SpellSlot.W}},
            {Champion.Rengar, new HashSet<SpellSlot> {SpellSlot.E}},
            {Champion.Riven, new HashSet<SpellSlot> {SpellSlot.W}},
            {Champion.Rumble, new HashSet<SpellSlot> {SpellSlot.E}},
            {Champion.Sejuani, new HashSet<SpellSlot> {SpellSlot.R}},
            {Champion.Syndra, new HashSet<SpellSlot> {SpellSlot.E}},
            {Champion.Sion, new HashSet<SpellSlot> {SpellSlot.E}},
            {Champion.Shen, new HashSet<SpellSlot> {SpellSlot.E}},
            {Champion.Sona, new HashSet<SpellSlot> {SpellSlot.R}},
            {Champion.Swain, new HashSet<SpellSlot> {SpellSlot.W}},
            {Champion.Taric, new HashSet<SpellSlot> {SpellSlot.W}},
            {Champion.Thresh, new HashSet<SpellSlot> {SpellSlot.Q}},
            {Champion.TwistedFate, new HashSet<SpellSlot> {SpellSlot.W}},
            {Champion.Varus, new HashSet<SpellSlot> {SpellSlot.R}},
            {Champion.Vayne, new HashSet<SpellSlot> {SpellSlot.E}},
            {Champion.Veigar, new HashSet<SpellSlot> {SpellSlot.E}},
            {Champion.Vi, new HashSet<SpellSlot> {SpellSlot.Q}},
            {Champion.Xerath, new HashSet<SpellSlot> {SpellSlot.E}},
            {Champion.Zyra, new HashSet<SpellSlot> {SpellSlot.E}}
        };

        public static Menu Menu
        {
            get { return MenuManager.GetSubMenu("Evader"); }
        }

        public static event EvaderHandler OnEvader;

        public static void Initialize()
        {
            if (EntityManager.Heroes.Enemies.Count > 0)
            {
                if (Menu == null)
                {
                    MenuManager.AddSubMenu("Evader");
                }
                if (Menu != null)
                {
                    foreach (var enemy in EntityManager.Heroes.Enemies)
                    {
                        Menu.AddLabel(enemy.Hero.ToString());
                        foreach (var slot in AllowedSlots)
                        {
                            if (!SpellsAddedToMenu.Contains(enemy.Hero.ToString() + slot))
                            {
                                Menu.AddValue(enemy.Hero.ToString() + slot, new CheckBox(slot.ToString(), false));
                                SpellsAddedToMenu.Add(enemy.Hero.ToString() + slot);
                            }
                        }
                    }
                }
            }
            Obj_AI_Base.OnProcessSpellCast += delegate (Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
            {
                var hero = sender as AIHeroClient;
                if (hero != null && sender.IsEnemy && AllowedSlots.Contains(args.Slot) && hero.IsValidTarget(1500f) &&
                    SpellsAddedToMenu.Contains(hero.Hero.ToString() + args.Slot) &&
                    Menu.CheckBox(hero.Hero.ToString() + args.Slot))
                {
                    if (DangerousSpellSlots.ContainsKey(hero.Hero) && DangerousSpellSlots[hero.Hero] == args.Slot)
                    {
                        if (DangerousSpellNames.ContainsKey(hero.Hero) &&
                            DangerousSpellNames[hero.Hero] != args.SData.Name)
                        {
                            return;
                        }
                    }
                    var evadeArgs = new EvaderArgs
                    {
                        Sender = hero,
                        StartTick = Core.GameTickCount,
                        Spell = args,
                        SData = args.SData
                    };
                    EvaderList.Add(evadeArgs);
                    if (OnEvader != null && evadeArgs.WillHitMyHero())
                    {
                        OnEvader(evadeArgs);
                    }
                }
            };
            GameObject.OnCreate += delegate (GameObject sender, EventArgs args)
            {
                var missile = sender as MissileClient;
                if (missile != null)
                {
                    var hero = missile.SpellCaster as AIHeroClient;
                    if (hero != null && hero.IsEnemy && hero.IsValidTarget(1500f))
                    {
                        var evaderArgs =
                            EvaderList.FirstOrDefault(
                                args1 =>
                                    args1.Sender.IdEquals(hero) &&
                                    args1.SData.AlternateName == missile.SData.AlternateName);
                        if (evaderArgs != null)
                        {
                            evaderArgs.SData = missile.SData;
                            evaderArgs.Missile = missile;
                        }
                        else if (EvaderList.Count(args1 => args1.Sender.IdEquals(hero)) == 1)
                        {
                            evaderArgs = EvaderList.FirstOrDefault(args1 => args1.Sender.IdEquals(hero));
                            if (evaderArgs != null)
                            {
                                evaderArgs.SData = missile.SData;
                                evaderArgs.Missile = missile;
                            }
                        }
                    }
                }
            };
            GameObject.OnDelete += delegate (GameObject sender, EventArgs args)
            {
                var missile = sender as MissileClient;
                if (missile != null)
                {
                    var hero = missile.SpellCaster as AIHeroClient;
                    if (hero != null)
                    {
                        foreach (var evaderArgs in EvaderList.Where(evaderArgs => evaderArgs.Missile != null))
                        {
                            evaderArgs.Missile = null;
                        }
                    }
                }
            };
            Game.OnTick += delegate
            {
                EvaderList.RemoveAll(args => Core.GameTickCount - args.StartTick > 1800 || !args.Sender.IsValidTarget());
                if (EvaderList.Count == 0 || OnEvader == null)
                {
                    return;
                }
                foreach (var args in EvaderList.Where(args => args.WillHitMyHero()))
                {
                    OnEvader(args);
                }
            };
        }

        public static void AddCrowdControlSpells()
        {
            if (Menu != null && EntityManager.Heroes.Enemies.Count > 0)
            {
                foreach (
                    var enemy in EntityManager.Heroes.Enemies.Where(h => CrowdControlSpellSlots.ContainsKey(h.Hero)))
                {
                    foreach (var slot in CrowdControlSpellSlots[enemy.Hero])
                    {
                        Menu[enemy.Hero.ToString() + slot].Cast<CheckBox>().CurrentValue = true;
                    }
                }
            }
        }

        public static void AddDangerousSpells()
        {
            if (Menu != null && EntityManager.Heroes.Enemies.Count > 0)
            {
                foreach (var enemy in EntityManager.Heroes.Enemies.Where(h => DangerousSpellSlots.ContainsKey(h.Hero)))
                {
                    Menu[enemy.Hero.ToString() + DangerousSpellSlots[enemy.Hero]].Cast<CheckBox>().CurrentValue = true;
                }
            }
        }

        public static bool WillHitMyHero(this EvaderArgs args, Vector3? vector = null)
        {
            SpellType spellType;
            switch (args.Spell.SData.TargettingType)
            {
                case SpellDataTargetType.Self:
                    spellType = SpellType.Self;
                    break;
                case SpellDataTargetType.Unit:
                    if (args.Spell.Target != null ||
                        (args.Missile != null && args.Missile.IsValidMissile() && args.Missile.Target != null))
                    {
                        spellType = SpellType.Targeted;
                    }
                    else
                    {
                        spellType = SpellType.Circular;
                    }
                    break;
                case SpellDataTargetType.LocationAoe:
                    spellType = SpellType.Circular;
                    break;
                case SpellDataTargetType.Cone:
                    spellType = SpellType.Cone;
                    break;
                case SpellDataTargetType.SelfAoe:
                    spellType = SpellType.Self;
                    break;
                case SpellDataTargetType.Location:
                    spellType = SpellType.Linear;
                    break;
                case SpellDataTargetType.SelfAndUnit:
                    spellType = SpellType.Self;
                    break;
                case SpellDataTargetType.Location2:
                    spellType = SpellType.Linear;
                    break;
                case SpellDataTargetType.LocationVector:
                    spellType = SpellType.Linear;
                    break;
                case SpellDataTargetType.LocationTunnel:
                    spellType = SpellType.Linear;
                    break;
                case SpellDataTargetType.LocationSummon:
                    spellType = SpellType.Linear;
                    break;
                case SpellDataTargetType.Location3:
                    spellType = SpellType.Linear;
                    break;
                default:
                    spellType = SpellType.Self;
                    break;
            }
            var currentPosition = vector ?? Player.Instance.Position;
            var missileIsValid = args.Missile != null && args.Missile.IsValidMissile();
            var range = Math.Min(args.Spell.SData.CastRangeDisplayOverride > 0
                ? args.Spell.SData.CastRangeDisplayOverride
                : args.Spell.SData.CastRange, args.SData.CastRangeDisplayOverride > 0
                    ? args.SData.CastRangeDisplayOverride
                    : args.SData.CastRange);
            var startVector = missileIsValid ? args.Missile.Position : args.Spell.Start;
            var realEnd = args.Spell.Start + (args.Spell.End - args.Spell.Start).Normalized() * range;
            var endVector = missileIsValid ? args.Missile.EndPosition : realEnd;
            var width = AIO.MyHero.BoundingRadius * 1.5f;
            var willHit = false;
            var containsMissile = args.ContainsMissile;
            switch (spellType)
            {
                case SpellType.Self:
                    break;
                case SpellType.Targeted:
                    break;
                case SpellType.Linear:
                    width += args.SData.LineWidth;
                    break;
                case SpellType.Circular:
                    width += Math.Max(args.SData.CastRadius, args.SData.CastRadiusSecondary) / 2f;
                    //Math.Max(args.SData.CastRadius, args.SData.CastRadiusSecondary);
                    break;
                case SpellType.Cone:
                    width += args.SData.LineWidth;
                    break;
            }
            if (missileIsValid)
            {
                if (spellType == SpellType.Targeted)
                {
                    willHit = args.Spell.Target != null && args.Spell.Target.IsMe &&
                              args.Missile.IsInRange(currentPosition, 200f);
                }
                else if (spellType == SpellType.Linear || spellType == SpellType.Circular || spellType == SpellType.Cone)
                {
                    var distance = spellType == SpellType.Circular
                        ? currentPosition.Distance(endVector)
                        : currentPosition.To2D().Distance(startVector.To2D(), endVector.To2D(), true, false);
                    if (distance <= width && distance > 0 && distance < float.MaxValue)
                    {
                        var timeToEvade = (width - distance) / AIO.MyHero.MoveSpeed + Game.Ping / 2000f;
                        var timeToArrive = args.SData.MissileFixedTravelTime > 0
                            ? args.SData.MissileFixedTravelTime
                            : currentPosition.Distance(args.Missile) / args.Missile.SData.MissileSpeed;
                        willHit = timeToEvade >= timeToArrive;
                    }
                }
            }
            else if (!containsMissile)
            {
                switch (spellType)
                {
                    case SpellType.Self:
                        if (args.Sender.Hero == Champion.Orianna)
                        {
                            var ballObject =
                                ObjectManager.Get<Obj_GeneralParticleEmitter>()
                                    .FirstOrDefault(o => o.IsValid && !o.IsDead && o.Name == "Orianna_Base_Q_yomu_ring_red.troy") ??
                                (args.Sender.HasBuff("orianaghostself") ? args.Sender : null) ??
                                UnitManager.ValidHeroes.FirstOrDefault(h => h.HasBuff("orianaghost")) as GameObject;
                            if (ballObject != null)
                            {
                                if (currentPosition.IsInRange(ballObject, range + width))
                                {
                                    willHit = true;
                                }
                            }
                        }
                        else
                        {
                            if (currentPosition.IsInRange(startVector, range + width))
                            {
                                willHit = true;
                            }
                        }
                        break;
                    case SpellType.Targeted:
                        willHit = args.Spell.Target != null && args.Spell.Target.IsMe;
                        break;
                    case SpellType.Linear:
                        willHit = currentPosition.To2D().Distance(startVector.To2D(), endVector.To2D(), true, true) <=
                                  width.Pow();
                        break;
                    case SpellType.Circular:
                        if (currentPosition.IsInRange(endVector, width))
                        {
                            willHit = true;
                        }
                        break;
                    case SpellType.Cone:
                        willHit = currentPosition.To2D().Distance(startVector.To2D(), endVector.To2D(), true, true) <=
                                  width.Pow();
                        break;
                }
            }
            return willHit;
        }
    }
}