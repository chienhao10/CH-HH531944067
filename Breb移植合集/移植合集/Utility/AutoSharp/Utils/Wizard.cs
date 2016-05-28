using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Path = System.Collections.Generic.List<ClipperLib.IntPoint>;
using Paths = System.Collections.Generic.List<System.Collections.Generic.List<ClipperLib.IntPoint>>;
using GamePath = System.Collections.Generic.List<SharpDX.Vector2>;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace AutoSharp.Utils
{
    internal static class Extensions
    {
        public static Vector3 GetPRADAPos(this AIHeroClient target)
        {
            if (target == null) return Vector3.Zero;
            var aRC = new Geometry.Circle(Heroes.Player.ServerPosition.LSTo2D(), 300).ToPolygon().ToClipperPath();
            var tP = target.ServerPosition;
            var pList = new List<Vector3>();
            foreach (var p in aRC)
            {
                var v3 = new Vector2(p.X, p.Y).To3D();


                if (!v3.UnderTurret(true) && v3.LSDistance(tP) > 325 && v3.LSDistance(tP) < Heroes.Player.AttackRange &&
                    (v3.LSCountEnemiesInRange(425) <= v3.CountAlliesInRange(325))) pList.Add(v3);
            }
            return pList.Count > 1 ? pList.OrderByDescending(el => el.LSDistance(tP)).FirstOrDefault() : Vector3.Zero;
        }

        public static Obj_AI_Turret GetClosestEnemyTurret(this Vector3 point)
        {
            return Turrets.EnemyTurrets.OrderBy(t => t.LSDistance(point)).FirstOrDefault();
        }

        public static Obj_AI_Minion GetFarthestMinionOnLane(Vector3 lanepos)
        {
            return Minions.AllyMinions.OrderByDescending(m => m.LSDistance(lanepos)).FirstOrDefault();
        }

        public static int CountNearbyAllyMinions(this Obj_AI_Base x, int distance)
        {
            if (x == null) return 0;
            return Minions.AllyMinions.Count(minion => minion.LSDistance(x.Position) < distance);
        }

        public static int CountNearbyAllyMinions(this Vector3 x, int distance)
        {
            return Minions.AllyMinions.Count(minion => minion.LSDistance(x) < distance);
        }

        public static int CountNearbyAllies(this Obj_AI_Base x, int distance)
        {
            if (x == null) return 0;
            return Heroes.AllyHeroes.Count(hero => !hero.IsDead && !hero.IsMe && hero.LSDistance(x.Position) < distance);
        }

        public static int CountNearbyAllies(this Vector3 x, int distance)
        {
            return Heroes.AllyHeroes.Count(hero => !hero.IsDead && !hero.IsMe && hero.LSDistance(x) < distance);
        }

        public static int CountNearbyEnemies(this Vector3 x, int distance)
        {
            return Heroes.EnemyHeroes.Count(hero => !hero.IsDead && !hero.IsMe && hero.LSDistance(x) < distance);
        }

        public static bool IsLowHealth(this Obj_AI_Base x)
        {
            if (x == null) return false;
            return x.HealthPercent < 30f;
        }

        public static int Randomize(this int i, int max = 200)
        {
            var r = new Random(Environment.TickCount);
            return i + r.Next(0, max);
        }

        public static Vector3 RandomizePosition(this GameObject o)
        {
            if (o == null) return Vector3.Zero;
            var r = new Random(Environment.TickCount);
            var minRandBy = Program.getMinRand;
            var maxRandBy = Program.getMaxRand;
            if (Program.getDefen && Heroes.Player.LSCountEnemiesInRange(800) >= Heroes.Player.CountAlliesInRange(800))
            {
                minRandBy *= Wizard.GetDefensiveMultiplier();
            }
            else
            {
                minRandBy *= r.Next(-1, 1);
            }
            return new Vector2(o.Position.X + r.Next(minRandBy, maxRandBy), o.Position.Y + r.Next(minRandBy, maxRandBy)).To3D();
        }

        public static Vector3 RandomizePosition(this Vector3 v)
        {
            var r = new Random(Environment.TickCount);
            var minRandBy = Program.getMinRand;
            var maxRandBy = Program.getMaxRand;
            if (Program.getDefen && Heroes.Player.LSCountEnemiesInRange(800) >= Heroes.Player.CountAlliesInRange(800))
            {
                minRandBy *= Wizard.GetDefensiveMultiplier();
            }
            else
            {
                minRandBy *= r.Next(-1, 1);
            }
            return new Vector2(v.X + r.Next(minRandBy, maxRandBy), v.Y + r.Next(minRandBy, maxRandBy)).To3D();
        }

        public static Vector3 RandomizePosition(this Vector2 v)
        {
            var r = new Random(Environment.TickCount);
            var minRandBy = Program.getMinRand;
            var maxRandBy = Program.getMaxRand;
            if (Program.getDefen && Heroes.Player.LSCountEnemiesInRange(800) >= Heroes.Player.CountAlliesInRange(800))
            {
                minRandBy *= Wizard.GetDefensiveMultiplier();
            }
            else
            {
                minRandBy *= r.Next(-1, 1);
            }
            return new Vector2(v.X + r.Next(minRandBy, maxRandBy), v.Y + r.Next(minRandBy, maxRandBy)).To3D();
        }

        public static bool HasItem(this AIHeroClient hero, ItemId item)
        {
            if (hero == null) return false;
            return hero.InventoryItems.Any(slot => slot.Id == item);
        }

        public static bool HasSupportItems(this AIHeroClient hero)
        {
            if (hero == null) return false;
            return hero.HasItem(ItemId.Ancient_Coin) || hero.HasItem(ItemId.Relic_Shield) ||
                   hero.HasItem(ItemId.Spellthiefs_Edge);
        }

        public static bool IsValidAlly(this Obj_AI_Base unit, float range = float.MaxValue)
        {
            if (unit == null) return false;
            return unit.LSDistance(ObjectManager.Player) < range && unit.IsValid<AIHeroClient>() && unit.IsAlly &&
                   !unit.IsDead && unit.IsTargetable;
        }

        public static double HealthBuffer(this Obj_AI_Base hero, int buffer)
        {
            if (hero == null) return 0;
            return hero.Health - (hero.MaxHealth * buffer / 100);
        }

        public static bool CastCheck(this Items.Item item, Obj_AI_Base target)
        {
            if (target == null) return false;
            return item.IsReady() && target.LSIsValidTarget(item.Range);
        }

        public static bool CastCheck(this Spell spell,
            Obj_AI_Base target,
            string menu,
            bool range = true,
            bool team = true)
        {
            return spell.IsReady() && target.LSIsValidTarget() && !ObjectManager.Player.UnderTurret(true);
        }

        public static bool CastCheck(this Spell spell, Obj_AI_Base target, bool range = true, bool team = true)
        {
            return spell.IsReady() && target.LSIsValidTarget() && !ObjectManager.Player.UnderTurret(true);
        }

        public static bool IsInRange(this Spell spell, Obj_AI_Base target)
        {
            return ObjectManager.Player.LSDistance(target) < spell.Range;
        }

        public static bool IsInRange(this Items.Item item, Obj_AI_Base target)
        {
            return ObjectManager.Player.LSDistance(target) < item.Range;
        }

        public static bool WillKill(this Obj_AI_Base caster, Obj_AI_Base target, string spell, int buffer = 10)
        {
            if (target == null) return false;
            return caster.LSGetSpellDamage(target, spell) >= target.HealthBuffer(buffer);
        }

        public static bool WillKill(this Obj_AI_Base caster, Obj_AI_Base target, SpellData spell, int buffer = 10)
        {
            if (target == null) return false;
            return caster.LSGetSpellDamage(target, spell.Name) >= target.HealthBuffer(buffer);
        }

        public static void AddList(this Menu menu, string name, string displayName, string[] list)
        {
            menu.Add("autosharp." + ObjectManager.Player.ChampionName + "." + name, new ComboBox(displayName, 0, list));
        }

        public static void AddBool(this Menu menu, string name, string displayName, bool value)
        {
            menu.Add("autosharp." + ObjectManager.Player.ChampionName + "." + name, new CheckBox(displayName, value));
        }

        public static void AddSlider(this Menu menu, string name, string displayName, int value, int min, int max)
        {
            menu.Add("autosharp." + ObjectManager.Player.ChampionName + "." + name, new Slider(displayName, value, min, max));
        }

        public static void AddObject(this Menu menu, string name, string displayName, object value)
        {
            //menu.Add("autosharp." + ObjectManager.Player.ChampionName + "." + name, new CheckBox(displayName, value));
        }
    }

    internal static class Wizard
    {
        private static int TimeSinceLastTakenDecision = 0;

        public static void AntiAfk()
        {
            if (Environment.TickCount - TimeSinceLastTakenDecision > 120000)
            {
                Player.IssueOrder(GameObjectOrder.MoveTo,
                    Heroes.Player.Position.RandomizePosition());
                TimeSinceLastTakenDecision = Environment.TickCount;
            }
        }

        public static Vector3 GetBestRecallPosition()
        {
            var t = GetClosestAllyTurret().Position;
            var i = 250.Randomize(50)*GetDefensiveMultiplier(); 
            return new Vector2(t.X + i, t.Y + i).To3D();
        }

        public static GameObject GetFarthestAllyTurret()
        {
            var turret =
                Turrets.AllyTurrets.OrderByDescending(t => t.LSDistance(HeadQuarters.AllyHQ.Position)).FirstOrDefault();
            if (turret != null)
            {
                return turret;
            }
            return ObjectManager.Get<Obj_BarracksDampener>().FirstOrDefault(b => b.IsAlly);
        }

        public static GameObject GetClosestAllyTurret()
        {
            var turret = Turrets.AllyTurrets.OrderBy(t => t.LSDistance(Heroes.Player)).FirstOrDefault();
            if (turret != null)
            {
                return turret;
            }
            return ObjectManager.Get<Obj_BarracksDampener>().FirstOrDefault(b => b.IsAlly);
        }

        public static Obj_AI_Turret GetClosestEnemyTurret()
        {
            return Turrets.EnemyTurrets.OrderBy(t => t.LSDistance(Heroes.Player)).FirstOrDefault();
        }

        public static Obj_AI_Minion GetFarthestMinion()
        {
            return Minions.AllyMinions.OrderByDescending(m => m.LSDistance(HeadQuarters.AllyHQ.Position)).FirstOrDefault();
        }

        public static Obj_AI_Minion GetClosestAllyMinion()
        {
            return Minions.AllyMinions.OrderBy(m => m.LSDistance(Heroes.Player.ServerPosition)).FirstOrDefault();
        }

        public static Obj_AI_Minion GetClosestEnemyMinion()
        {
            return Minions.EnemyMinions.OrderBy(m => m.LSDistance(Heroes.Player.ServerPosition)).FirstOrDefault();
        }

        public static int GetAggressiveMultiplier()
        {
            if (Heroes.Player.Team == GameObjectTeam.Order)
            {
                return 1;
            }
            return -1;
        }

        public static int GetDefensiveMultiplier()
        {
            if (Heroes.Player.Team == GameObjectTeam.Order)
            {
                return -1;
            }
            return 1;
        }

        public static void MoveToClosestAllyMinion()
        {
            Player.IssueOrder(GameObjectOrder.MoveTo, GetFarthestMinion().RandomizePosition());
        }

        public static void MoveBehindClosestAllyMinion()
        {
            var m = GetClosestAllyMinion().Position;
            var i = 100.Randomize(50)*GetDefensiveMultiplier();
            Player.IssueOrder(GameObjectOrder.MoveTo, new Vector2(m.X + i, m.Y + i).To3D());
        }

        public static void MoveToRandomAllyMinion()
        {
            var randomAllyMinion = Minions.AllyMinions.OrderBy(m => new Random().Next()).FirstOrDefault();
            if (randomAllyMinion == null || !randomAllyMinion.IsValid) { return; }
            Player.IssueOrder(GameObjectOrder.MoveTo, randomAllyMinion.Position.RandomizePosition());
        }
    }
}