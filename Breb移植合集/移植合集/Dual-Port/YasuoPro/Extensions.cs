using System.Linq;
using System.Reflection;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;

namespace YasuoPro
{
    static class Extensions
    {
        internal static AIHeroClient Player = Helper.Yasuo;

        internal static bool IsDashable(this Obj_AI_Base unit, float range = 475)
        {
            if (unit == null || unit.Team == Player.Team || unit.LSDistance(Player) > range || !unit.IsValid || unit.IsDead || !unit.IsVisible || !unit.IsTargetable)
            {
                return false;
            }

            if (Helper.GetBool("Misc.SafeE", YasuoMenu.MiscM))
            {
                var point = Helper.GetDashPos(unit);
                if (!Valvrave_Sharp.Evade.Evade.IsSafePoint(point).IsSafe)
                {
                    return false;
                }
            }

            var minion = unit as Obj_AI_Minion;
            return !unit.HasBuff("YasuoDashWrapper") && (unit is AIHeroClient || minion.IsValidMinion());
        }

      
        internal static bool IsValidMinion(this Obj_AI_Minion minion, float range = 50000)
        {
            if (minion == null)
            {
                return false;
            }

            var name = minion.CharData.BaseSkinName.ToLower();
            return (Player.LSDistance(minion) <= range && minion.IsValid && minion.IsVisible && minion.Team != Player.Team && minion.IsHPBarRendered && !MinionManager.IsWard(minion) && !name.Contains("gangplankbarrel"));
        }

        internal static bool IsValidAlly(this Obj_AI_Base unit, float range = 50000)
        {
            if (unit == null || unit.LSDistance(Player) > range || unit.Team != Player.Team || !unit.IsValid || unit.IsDead || !unit.IsVisible || unit.IsTargetable)
            {
                return false;
            }
            return true;
        }

        internal static bool IsValidEnemy(this Obj_AI_Base unit, float range = 50000)
        {
            if (unit == null || !unit.IsHPBarRendered || unit.IsZombie || unit.LSDistance(Player) > range || unit.Team == Player.Team || !unit.IsValid || unit.IsDead || !unit.IsVisible || !unit.IsTargetable)
            {
                return false;
            }
            return true;
        }

        internal static bool IsInRange(this Obj_AI_Base unit, float range)
        {
            if (unit != null)
            {
                return Vector2.Distance(unit.ServerPosition.LSTo2D(), Helper.Yasuo.ServerPosition.LSTo2D()) <= range;
            }
            return false;
        }

        internal static bool PointUnderEnemyTurret(this Vector2 Point)
        {
            var EnemyTurrets =
                ObjectManager.Get<Obj_AI_Turret>().Find(t => t.IsEnemy && Vector2.Distance(Point, t.Position.LSTo2D()) < 910f + Helper.Yasuo.BoundingRadius);
            return EnemyTurrets != null;
        }

        internal static bool PointUnderEnemyTurret(this Vector3 Point)
        {
            var EnemyTurrets =
                ObjectManager.Get<Obj_AI_Turret>().Where(t => t.IsEnemy && Vector3.Distance(t.Position, Point) < 910f + Helper.Yasuo.BoundingRadius);
            return EnemyTurrets.Any();
        }

        internal static bool CanKill(this Obj_AI_Base @base, SpellSlot slot)
        {
            if (slot == SpellSlot.E)
            {
                return Helper.GetProperEDamage(@base) >= @base.Health;
            }
            return Player.LSGetSpellDamage(@base, slot) >= @base.Health;
        }

        internal static bool IsCloserWP(this Vector2 point, Obj_AI_Base target)
        {
            var wp = target.GetWaypoints();
            var lastwp = wp.LastOrDefault();
            var wpc = wp.Count();
            var midwpnum = wpc / 2;
            var midwp = wp[midwpnum];
            var plength = wp[0].LSDistance(lastwp);
            return (point.LSDistance(target.ServerPosition) <= Player.LSDistance(target.ServerPosition) - Helper.Yasuo.BoundingRadius) || ((plength <= Player.LSDistance(target.ServerPosition) * 1.2f && point.LSDistance(lastwp.To3D()) < Player.LSDistance(lastwp.To3D()) || point.LSDistance(midwp.To3D()) < Player.LSDistance(midwp)));
        }

        internal static bool IsCloser(this Vector2 point, Obj_AI_Base target)
        {
            if (Helper.GetBool("Combo.EAdvanced", YasuoMenu.ComboM))
            {
                return IsCloserWP(point, target);
            }
            return (point.LSDistance(target.ServerPosition) <= Player.LSDistance(target.ServerPosition) - Helper.Yasuo.BoundingRadius);
        }

        internal static bool IsCloser(this Obj_AI_Base @base, Obj_AI_Base target)
        {
            return Helper.GetDashPos(@base).LSDistance(target.ServerPosition) < Player.LSDistance(target.ServerPosition);
        }

        internal static Vector3 WTS(this Vector3 vect)
        {
            return Drawing.WorldToScreen(vect).To3D();
        }


        //Menu Extensions
       
        public static void AddBool(this Menu menu, string name, string displayname, bool @defaultvalue = true)
        {
            menu.Add(name, new CheckBox(displayname, @defaultvalue));
        }

        public static void AddKeyBind(this Menu menu, string name, string displayname, uint key, KeyBind.BindTypes type)
        {
            menu.Add(name, new KeyBind(displayname, false, type, key));
        }

        public static void AddCircle(this Menu menu, string name, string displayname, bool @defaultvalue = true)
        {
            menu.Add(name, new CheckBox(displayname, @defaultvalue));
        }

        public static void AddSlider(this Menu menu, string name, string displayname, int initial = 0, int min = 0, int max = 100)
        {
            menu.Add(name, new Slider(displayname, initial, min, max));
        }

        public static void AddSList(this Menu menu, string name, string displayname, string[] stringlist, int @default = 0)
        {
            menu.Add(name, new ComboBox(displayname, @default, stringlist));
        }

        internal static bool IsTargetValid(this AttackableUnit unit,
        float range = float.MaxValue,
        bool checkTeam = true,
        Vector3 from = new Vector3())
        {
            if (unit == null || !unit.IsValid || unit.IsDead || !unit.IsVisible || !unit.IsTargetable ||
                unit.IsInvulnerable)
            {
                return false;
            }

            var @base = unit as Obj_AI_Base;
            if (@base != null)
            {
                if (@base.HasBuff("kindredrnodeathbuff") && @base.HealthPercent <= 10)
                {
                    return false;
                }
            }

            if (checkTeam && unit.Team == ObjectManager.Player.Team)
            {
                return false;
            }

            var unitPosition = @base != null ? @base.ServerPosition : unit.Position;

            return !(range < float.MaxValue) ||
                   !(Vector2.DistanceSquared(
                       (@from.LSTo2D().IsValid() ? @from : ObjectManager.Player.ServerPosition).LSTo2D(),
                       unitPosition.LSTo2D()) > range * range);
        }

        internal static bool QCanKill(this Obj_AI_Base minion, bool isQ2 = false)
        {
            //var hpred =
            //  HealthPrediction.GetHealthPrediction(minion, 0, 500 + Game.Ping / 2);
            // return hpred < 0.95 * Player.GetSpellDamage(minion, SpellSlot.Q) && hpred > 0;
            var qspell = isQ2 ? Helper.Spells[Helper.Q2] : Helper.Spells[Helper.Q];
            var dmg = Player.LSGetSpellDamage(minion, SpellSlot.Q) / 1.3
                            >= HealthPrediction.GetHealthPrediction(
                                minion,
                                (int)(Player.LSDistance(minion) / qspell.Speed) * 1000,
                                (int)qspell.Delay * 1000);
            return dmg;
        }

        internal static bool ECanKill(this Obj_AI_Base minion)
        {
            var espell = Helper.Spells[Helper.E];
            return Helper.GetProperEDamage(minion) / 1.2
                            >= HealthPrediction.GetHealthPrediction(
                                minion,
                                (int)(Player.LSDistance(minion) / espell.Speed) * 1000,
                                (int)espell.Delay * 1000);
        }

        internal static bool isBlackListed(this AIHeroClient unit)
        {
            return !Helper.GetBool("ult" + unit.ChampionName, YasuoMenu.ComboM);
        }

        internal static int MinionsInRange(this Obj_AI_Base unit, float range)
        {
            var minions = ObjectManager.Get<Obj_AI_Minion>().Count(x => x.LSDistance(unit) <= range && x.NetworkId != unit.NetworkId && x.Team == unit.Team);
            return minions;
        }

        internal static int MinionsInRange(this Vector2 pos, float range)
        {
            var minions = ObjectManager.Get<Obj_AI_Minion>().Count(x => x.LSDistance(pos) <= range && (x.IsEnemy || x.Team == GameObjectTeam.Neutral));
            return minions;
        }

        internal static int MinionsInRange(this Vector3 pos, float range)
        {
            var minions = ObjectManager.Get<Obj_AI_Minion>().Count(x => x.LSDistance(pos) <= range && (x.IsEnemy || x.Team == GameObjectTeam.Neutral));
            return minions;
        }
    }
}
