namespace Valvrave_Sharp.Core
{
    #region

    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Data.Enumerations;
    using LeagueSharp.SDK;

    using SharpDX;

    using Collision = LeagueSharp.SDK.Collision;
    using EloBuddy;
    using LeagueSharp.SDK.Core.Utils;
    #endregion

    internal static class Common
    {
        #region Properties

        private static int GetSmiteDmg
            =>
                new[] { 390, 410, 430, 450, 480, 510, 540, 570, 600, 640, 680, 720, 760, 800, 850, 900, 950, 1000 }[
                    Program.Player.Level - 1];

        #endregion

        #region Methods

        internal static bool CanHitCircle(this Spell spell, Obj_AI_Base unit)
        {
            return spell.IsInRange(spell.GetPredPosition(unit));
        }

        internal static bool CanLastHit(this Spell spell, Obj_AI_Base unit, double dmg, double subDmg = 0)
        {
            var hpPred = spell.GetHealthPrediction(unit);
            return hpPred > 0 && hpPred - subDmg < dmg;
        }

        internal static CastStates Casting(
            this Spell spell,
            Obj_AI_Base unit,
            bool aoe = false,
            LeagueSharp.SDK.CollisionableObjects collisionable = LeagueSharp.SDK.CollisionableObjects.Minions | LeagueSharp.SDK.CollisionableObjects.YasuoWall)
        {
            if (!unit.LSIsValidTarget())
            {
                return CastStates.InvalidTarget;
            }
            if (!spell.IsReady())
            {
                return CastStates.NotReady;
            }
            if (spell.CastCondition != null && !spell.CastCondition())
            {
                return CastStates.FailedCondition;
            }
            var pred = spell.GetPrediction(unit, aoe, -1, collisionable);
            if (pred.CollisionObjects.Count > 0)
            {
                return CastStates.Collision;
            }
            if (spell.RangeCheckFrom.DistanceSquared(pred.CastPosition) > spell.RangeSqr)
            {
                return CastStates.OutOfRange;
            }
            if (pred.Hitchance < spell.MinHitChance
                && (!pred.Input.AoE || pred.Hitchance < HitChance.High || pred.AoeTargetsHitCount < 2))
            {
                return CastStates.LowHitChance;
            }
            if (!Program.Player.Spellbook.CastSpell(spell.Slot, pred.CastPosition))
            {
                return CastStates.NotCasted;
            }
            spell.LastCastAttemptT = Variables.TickCount;
            return CastStates.SuccessfullyCasted;
        }

        internal static CastStates CastingBestTarget(
            this Spell spell,
            bool aoe = false,
            LeagueSharp.SDK.CollisionableObjects collisionable = LeagueSharp.SDK.CollisionableObjects.Minions | LeagueSharp.SDK.CollisionableObjects.YasuoWall)
        {
            return spell.Casting(spell.GetTarget(spell.Width / 2), aoe, collisionable);
        }

        internal static bool CastSmiteKillCollision(List<Obj_AI_Base> col)
        {
            if (col.Count > 1 || !Program.Smite.IsReady())
            {
                return false;
            }
            var obj = col.First();
            return obj.Health <= GetSmiteDmg && obj.DistanceToPlayer() < Program.SmiteRange
                   && Program.Player.Spellbook.CastSpell(Program.Smite, obj);
        }

        internal static List<Obj_AI_Base> GetCollision(
            this Spell spell,
            Obj_AI_Base target,
            List<Vector3> to,
            LeagueSharp.SDK.CollisionableObjects collisionable = LeagueSharp.SDK.CollisionableObjects.Minions)
        {
            var col = Collision.GetCollision(
                to,
                new PredictionInput
                {
                    Delay = spell.Delay,
                    Radius = spell.Width,
                    Speed = spell.Speed,
                    From = spell.From,
                    Range = spell.Range,
                    Type = spell.Type,
                    CollisionObjects = collisionable
                });
            col.RemoveAll(i => i.Compare(target));
            return col;
        }

        internal static Vector3 GetPredPosition(this Spell spell, Obj_AI_Base unit, bool useRange = false)
        {
            var pos = Movement.GetPrediction(unit, spell.Delay, 1, spell.Speed).UnitPosition;
            return useRange && !spell.IsInRange(pos) ? unit.ServerPosition : pos;
        }

        internal static bool IsCasted(this CastStates state)
        {
            return state == CastStates.SuccessfullyCasted;
        }

        internal static bool IsWard(this Obj_AI_Minion minion)
        {
            return minion.GetMinionType().HasFlag(MinionTypes.Ward) && minion.CharData.BaseSkinName != "BlueTrinket";
        }

        internal static List<Obj_AI_Base> ListEnemies(bool includeClones = false)
        {
            var list = new List<Obj_AI_Base>();
            list.AddRange(GameObjects.EnemyHeroes);
            list.AddRange(ListMinions(includeClones));
            return list;
        }

        internal static List<Obj_AI_Minion> ListMinions(bool includeClones = false)
        {
            var list = new List<Obj_AI_Minion>();
            list.AddRange(GameObjects.Jungle);
            list.AddRange(GameObjects.EnemyMinions.Where(i => i.IsMinion() || i.IsPet(includeClones)));
            return list;
        }

        #endregion
    }
}