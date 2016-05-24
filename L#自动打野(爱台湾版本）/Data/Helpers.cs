using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;

namespace AutoJungle.Data
{
    internal class Helpers
    {
        public static List<Vector3> mod = new List<Vector3>();
        public static List<Vector3> orig = new List<Vector3>();
        //public static List<Vector3> mod = new List<Vector3>();

        public static List<Geometry.Polygon> Lanes = new List<Geometry.Polygon>()
        {
            SummonersRift.MidLane.Contest_Zone,
            SummonersRift.BottomLane.Contest_Zone,
            SummonersRift.TopLane.Contest_Zone
        };

        public static List<Vector3> GankPos = new List<Vector3>()
        {
            new Vector3(2918f, 11142f, -71.2406f),
            //TopRiverBush
            new Vector3(2247.295f, 9706.15f, 56.8484f), //TopTriBush
            new Vector3(4462f, 11764f, 51.9751f), //TopTriBushD
            new Vector3(6538f, 8312f, -71.2406f), //MidTopBush
            new Vector3(8502f, 6548f, -71.2406f), //MidBottomBush
            new Vector3(11900f, 3898f, -67.15347f), //BotRiverBush
            new Vector3(10418f, 3050f, 50.23584f), //BotTriBush
            new Vector3(9227.038f, 2201.226f, 54.70776f), //BotSideBushDown
            new Vector3(5739.739f, 12759.03f, 52.83813f), //TopBushSide
            new Vector3(12483.09f, 5221.66f, 51.72937f) //BotupperTriBush
        };

        public static List<Vector3> WardPos = new List<Vector3>()
        {
            //LoL, this is easier...
            SummonersRift.Bushes.TopRedJungle_TriBush.CenterOfPolygone().To3D(),
            SummonersRift.Bushes.TopRiver_MiddleBush.CenterOfPolygone().To3D(),
            SummonersRift.Bushes.BottomRiver_MiddleBush.CenterOfPolygone().To3D(),
            SummonersRift.Bushes.BottomBlueJungle_TriBush.CenterOfPolygone().To3D(),
            SummonersRift.Bushes.TopRiver_TopBush.CenterOfPolygone().To3D(),
            SummonersRift.River.Dragon_Contest_Zone.CenterOfPolygone()
                .To3D()
                .Extend(SummonersRift.River.Dragon_Pit.CenterOfPolygone().To3D(), -300),
        };

        public static List<Obj_AI_Base> getMobs(Vector3 pos, float range)
        {
            return
                MinionManager.GetMinions(pos, range, MinionTypes.All, MinionTeam.NotAlly)
                    .Where(
                        m =>
                            m.IsValidTarget() && !Jungle.bosses.Any(n => m.Name.Contains(n)) && m.IsValidTarget() &&
                            !m.Name.Contains("barrel"))
                    .OrderBy(m => m.Distance(Program.player))
                    .Take(25)
                    .ToList();
        }

        public static List<Obj_AI_Base> getAllyMobs(Vector3 pos, float range)
        {
            return
                MinionManager.GetMinions(pos, range, MinionTypes.All, MinionTeam.Ally)
                    .OrderBy(m => m.Distance(Program.player))
                    .Take(25)
                    .ToList();
        }

        internal static bool CheckPath(Vector3[] vectors, bool withoutChamps = false)
        {
            var list = vectors.ToList();
            for (var i = 0; i < list.Count; i++)
            {
                if (i < list.Count - 1 && list[i].Distance(list[i + 1]) > 800)
                {
                    if ((i > 1 && list[i - 1].Distance(list[i]) > 150) &&
                        ((list[i].CountEnemiesInRange(GameInfo.ChampionRange) > 0 || !withoutChamps) &&
                         (list[i].UnderTurret(true) || AvoidLane(list[i]))))
                    {
                        return false;
                    }
                    list.Insert(i + 1, list[i].Extend(list[i + 1], list[i].Distance(list[i + 1]) / 2));
                    i--;
                }
            }
            mod = list;
            return true;
        }


        private static bool AvoidLane(Vector3 point)
        {
            if (Program._GameInfo.GameState != State.Positioning)
            {
                return false;
            }
            Lanes.Add(
                Program.player.Team == GameObjectTeam.Chaos
                    ? SummonersRift.MidLane.Blue_Zone
                    : SummonersRift.MidLane.Red_Zone);
            return Lanes.Any(l => !l.IsInside(Program.player.Position) && l.IsInside(point));
        }

        public static Obj_AI_Hero GetTargetEnemy()
        {
            return
                HeroManager.Enemies.Where(
                    e =>
                        e.IsValidTarget() &&
                        (!e.UnderTurret(true) ||
                         (e.Health < e.GetAutoAttackDamage(e, true) * 2 &&
                          e.Distance(Program.player) < Orbwalking.GetRealAutoAttackRange(e))) &&
                        e.Distance(Program.player) < GameInfo.ChampionRange)
                    .OrderByDescending(e => GetComboDMG(Program.player, e) > e.Health)
                    .ThenByDescending(e => e.Distance(Program.player) < 500)
                    .ThenBy(e => e.Health)
                    .FirstOrDefault();
        }

        public static Obj_AI_Base GetNearest(Vector3 pos, float dist = 700f)
        {
            var minions =
                ObjectManager.Get<Obj_AI_Minion>()
                    .Where(
                        minion =>
                            minion.IsValidTarget() && minion.IsEnemy && !minion.IsDead && !minion.Name.Contains("Mini") &&
                            Camps.BigMobs.Any(
                                name => minion.Name.StartsWith(name) && pos.Distance(minion.Position) <= dist))
                    .OrderByDescending(m => m.MaxHealth);
            return minions.FirstOrDefault();
        }

        public static float GetComboDMG(Obj_AI_Hero source, Obj_AI_Hero target)
        {
            double result = 0;
            double basicDmg = 0;
            int attacks = (int) Math.Floor(source.AttackSpeedMod * 5);
            for (int i = 0; i < attacks; i++)
            {
                if (source.Crit > 0)
                {
                    basicDmg += source.GetAutoAttackDamage(target, true) * (1f + source.Crit / attacks);
                }
                else
                {
                    basicDmg += source.GetAutoAttackDamage(target, true);
                }
            }
            result += basicDmg;
            var spells = source.Spellbook.Spells;
            foreach (var spell in spells)
            {
                var t = spell.CooldownExpires - Game.Time;
                if (t < 0.5)
                {
                    switch (source.SkinName)
                    {
                        case "Ahri":
                            if (spell.Slot == SpellSlot.Q)
                            {
                                result += (Damage.GetSpellDamage(source, target, spell.Slot));
                                result += (Damage.GetSpellDamage(source, target, spell.Slot, 1));
                            }
                            else
                            {
                                result += Damage.GetSpellDamage(source, target, spell.Slot);
                            }
                            break;
                        case "Akali":
                            if (spell.Slot == SpellSlot.R)
                            {
                                result += (Damage.GetSpellDamage(source, target, spell.Slot) * spell.Ammo);
                            }
                            else if (spell.Slot == SpellSlot.Q)
                            {
                                result += (Damage.GetSpellDamage(source, target, spell.Slot));
                                result += (Damage.GetSpellDamage(source, target, spell.Slot, 1));
                            }
                            else
                            {
                                result += Damage.GetSpellDamage(source, target, spell.Slot);
                            }
                            break;
                        case "Amumu":
                            if (spell.Slot == SpellSlot.W)
                            {
                                result += (Damage.GetSpellDamage(source, target, spell.Slot) * 5);
                            }
                            else
                            {
                                result += Damage.GetSpellDamage(source, target, spell.Slot);
                            }
                            break;
                        case "Cassiopeia":
                            if (spell.Slot == SpellSlot.Q || spell.Slot == SpellSlot.E || spell.Slot == SpellSlot.W)
                            {
                                result += (Damage.GetSpellDamage(source, target, spell.Slot) * 2);
                            }
                            else
                            {
                                result += Damage.GetSpellDamage(source, target, spell.Slot);
                            }
                            break;
                        case "Fiddlesticks":
                            if (spell.Slot == SpellSlot.W || spell.Slot == SpellSlot.E)
                            {
                                result += (Damage.GetSpellDamage(source, target, spell.Slot) * 5);
                            }
                            else
                            {
                                result += Damage.GetSpellDamage(source, target, spell.Slot);
                            }
                            break;
                        case "Garen":
                            if (spell.Slot == SpellSlot.E)
                            {
                                result += (Damage.GetSpellDamage(source, target, spell.Slot) * 3);
                            }
                            else
                            {
                                result += Damage.GetSpellDamage(source, target, spell.Slot);
                            }
                            break;
                        case "Irelia":
                            if (spell.Slot == SpellSlot.W)
                            {
                                result += (Damage.GetSpellDamage(source, target, spell.Slot) * attacks);
                            }
                            else
                            {
                                result += Damage.GetSpellDamage(source, target, spell.Slot);
                            }
                            break;
                        case "Karthus":
                            if (spell.Slot == SpellSlot.Q)
                            {
                                result += (Damage.GetSpellDamage(source, target, spell.Slot) * 4);
                            }
                            else
                            {
                                result += Damage.GetSpellDamage(source, target, spell.Slot);
                            }
                            break;
                        case "KogMaw":
                            if (spell.Slot == SpellSlot.W)
                            {
                                result += (Damage.GetSpellDamage(source, target, spell.Slot) * attacks);
                            }
                            else
                            {
                                result += Damage.GetSpellDamage(source, target, spell.Slot);
                            }
                            break;
                        case "LeeSin":
                            if (spell.Slot == SpellSlot.Q)
                            {
                                result += Damage.GetSpellDamage(source, target, spell.Slot);
                                result += Damage.GetSpellDamage(source, target, spell.Slot, 1);
                            }
                            else
                            {
                                result += Damage.GetSpellDamage(source, target, spell.Slot);
                            }
                            break;
                        case "Lucian":
                            if (spell.Slot == SpellSlot.R)
                            {
                                result += Damage.GetSpellDamage(source, target, spell.Slot) * 4;
                            }
                            else
                            {
                                result += Damage.GetSpellDamage(source, target, spell.Slot);
                            }
                            break;
                        case "Nunu":
                            if (spell.Slot != SpellSlot.R && spell.Slot != SpellSlot.Q)
                            {
                                result += Damage.GetSpellDamage(source, target, spell.Slot);
                            }
                            else
                            {
                                result += Damage.GetSpellDamage(source, target, spell.Slot);
                            }
                            break;
                        case "MasterYi":
                            if (spell.Slot == SpellSlot.E)
                            {
                                result += Damage.GetSpellDamage(source, target, spell.Slot) * attacks;
                            }
                            else if (spell.Slot == SpellSlot.R)
                            {
                                result += basicDmg * 0.6f;
                            }
                            else
                            {
                                result += Damage.GetSpellDamage(source, target, spell.Slot);
                            }
                            break;
                        case "MonkeyKing":
                            if (spell.Slot == SpellSlot.R)
                            {
                                result += Damage.GetSpellDamage(source, target, spell.Slot) * 4;
                            }
                            else
                            {
                                result += Damage.GetSpellDamage(source, target, spell.Slot);
                            }
                            break;
                        case "Pantheon":
                            if (spell.Slot == SpellSlot.E)
                            {
                                result += Damage.GetSpellDamage(source, target, spell.Slot) * 3;
                            }
                            else if (spell.Slot == SpellSlot.R)
                            {
                                result += 0;
                            }
                            else
                            {
                                result += Damage.GetSpellDamage(source, target, spell.Slot);
                            }

                            break;
                        case "Rammus":
                            if (spell.Slot == SpellSlot.R)
                            {
                                result += Damage.GetSpellDamage(source, target, spell.Slot) * 6;
                            }
                            else
                            {
                                result += Damage.GetSpellDamage(source, target, spell.Slot);
                            }
                            break;
                        case "Riven":
                            if (spell.Slot == SpellSlot.Q)
                            {
                                result += RivenDamageQ(spell, source, target);
                            }
                            else
                            {
                                result += Damage.GetSpellDamage(source, target, spell.Slot);
                            }
                            break;
                        case "Shyvana":
                            if (spell.Slot == SpellSlot.W)
                            {
                                result += Damage.GetSpellDamage(source, target, spell.Slot) * 4;
                            }
                            else
                            {
                                result += Damage.GetSpellDamage(source, target, spell.Slot);
                            }
                            break;
                        case "Viktor":
                            if (spell.Slot == SpellSlot.R)
                            {
                                result += Damage.GetSpellDamage(source, target, spell.Slot);
                                result += Damage.GetSpellDamage(source, target, spell.Slot, 1) * 5;
                            }
                            else
                            {
                                result += Damage.GetSpellDamage(source, target, spell.Slot);
                            }
                            break;
                        case "Vladimir":
                            if (spell.Slot == SpellSlot.E)
                            {
                                result += Damage.GetSpellDamage(source, target, spell.Slot) * 2;
                            }
                            else
                            {
                                result += Damage.GetSpellDamage(source, target, spell.Slot);
                            }
                            break;
                        default:
                            result += Damage.GetSpellDamage(source, target, spell.Slot);
                            break;
                    }
                }
            }
            if (source.Spellbook.CanUseSpell(target.GetSpellSlot("summonerdot")) == SpellState.Ready)
            {
                result += source.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            }
            return (float) result;
        }

        private static float RivenDamageQ(SpellDataInst spell, Obj_AI_Hero src, Obj_AI_Hero dsc)
        {
            double dmg = 0;
            if (spell.IsReady())
            {
                dmg += src.CalcDamage(
                    dsc, Damage.DamageType.Physical,
                    (-10 + (spell.Level * 20) +
                     (0.35 + (spell.Level * 0.05)) * (src.FlatPhysicalDamageMod + src.BaseAttackDamage)) * 3);
            }
            return (float) dmg;
        }

        internal static AttackableUnit CheckStructure()
        {
            var turret =
                ObjectManager.Get<Obj_AI_Turret>()
                    .FirstOrDefault(
                        t =>
                            t.IsValidTarget() && Program.player.Distance(t) < GameInfo.ChampionRange &&
                            getAllyMobs(t.Position, 1000).Count > 1);
            if (turret != null)
            {
                return turret;
            }
            var inhib =
                ObjectManager.Get<Obj_BarracksDampener>()
                    .FirstOrDefault(i => i.IsValidTarget() && Program.player.Distance(i) < 1100);
            if (inhib != null)
            {
                return inhib;
            }

            var nexus =
                ObjectManager.Get<Obj_HQ>().FirstOrDefault(n => n.IsValidTarget() && Program.player.Distance(n) < 1100);
            if (nexus != null)
            {
                return nexus;
            }
            return null;
        }

        internal static Vector3 GetClosestWard()
        {
            return
                WardPos.Where(
                    w =>
                        w.Distance(Program.player.Position) < 750 &&
                        ObjectManager.Get<Obj_AI_Base>()
                            .FirstOrDefault(
                                o =>
                                    o.Distance(w) < GameInfo.ChampionRange && o.Health > 0 &&
                                    o.Name.ToLower().Contains("ward") && !o.Name.ToLower().Contains("corpse")) == null)
                    .OrderBy(w => w.Distance(Program.player.Position))
                    .FirstOrDefault();
        }

        internal static float GetHealth(bool ally, Vector3 pos)
        {
            return
                HeroManager.AllHeroes.Where(
                    h => !h.IsDead && h.IsAlly == ally && pos.Distance(h.Position) < GameInfo.ChampionRange)
                    .Sum(h => h.Health);
        }

        internal static int AlliesThere(Vector3 pos, float range = GameInfo.ChampionRange)
        {
            return HeroManager.Allies.Count(h => !h.IsDead && !h.IsMe && pos.Distance(h.Position) < range);
        }

        internal static float GetRealDistance(Obj_AI_Hero hero, Vector3 b)
        {
            var path = hero.GetPath(b);
            var lastPoint = path[0];
            var distance = 0f;
            foreach (var point in path.Where(point => !point.Equals(lastPoint)))
            {
                distance += lastPoint.Distance(point);
                lastPoint = point;
            }
            return distance;
        }
    }
}