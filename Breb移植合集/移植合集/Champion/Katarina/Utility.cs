using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using SharpDX;
using Damage = LeagueSharp.Common.Damage;
using Spell = LeagueSharp.Common.Spell;

//using TreeLib.Extensions;

namespace Staberina
{
    internal static class Utility
    {
        private static readonly ItemId[] WardIds =
        {
            ItemId.Warding_Totem_Trinket, ItemId.Greater_Stealth_Totem_Trinket,
            ItemId.Greater_Vision_Totem_Trinket, ItemId.Sightstone, ItemId.Ruby_Sightstone, ItemId.Vision_Ward,
            (ItemId) 3711, (ItemId) 1411, (ItemId) 1410, (ItemId) 1408, (ItemId) 1409
        };

        public static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public static bool MoveRandomly()
        {
            var pos = ObjectManager.Player.ServerPosition.Randomize(10, 20);
            return EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, pos, false);
        }

        public static Obj_AI_Base GetClosestETarget(Obj_AI_Base unit)
        {
            return
                GetETargets(unit.ServerPosition)
                    .Where(o => o.NetworkId != unit.NetworkId && unit.LSDistance(o) < SpellManager.Q.Range)
                    .MinOrDefault(o => o.LSDistance(unit));
        }

        public static float DistanceToPlayer(this GameObject obj)
        {
            var unit = obj as Obj_AI_Base;
            if (unit == null || !unit.IsValid)
            {
                return obj.Position.LSDistance(Player.ServerPosition);
            }

            return unit.ServerPosition.LSDistance(Player.ServerPosition);
        }

        public static float DistanceToPlayer(this Vector3 position)
        {
            return position.LSDistance(Player.ServerPosition);
        }

        public static float DistanceToPlayer(this Vector2 position)
        {
            return position.LSDistance(Player.ServerPosition);
        }

        public static Obj_AI_Base GetClosestETarget(Vector3 position)
        {
            return GetETargets().OrderBy(t => t.LSDistance(position)).ThenByDescending(t => t.DistanceToPlayer()).FirstOrDefault();
        }

        public static IEnumerable<Obj_AI_Base> GetETargets(Vector3 position = new Vector3())
        {
            return ObjectManager.Get<Obj_AI_Base>().Where(o => o.LSIsValidTarget(SpellManager.E.Range, false, position) && !o.IsMe && SpellManager.E.IsInRange(o));
        }

        public static bool IsRReady()
        {
            var slot = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R);
            return SpellManager.R.IsReady() || (slot.Level > 0 && slot.State.Equals(SpellState.Surpressed));
        }

        public static SpellSlot GetReadyWard()
        {
            var ward =
                ObjectManager.Player.InventoryItems.Where(s => WardIds.Contains(s.Id) && s.SpellSlot.IsReady())
                    .Select(s => s.SpellSlot)
                    .ToList();

            if (ward.Count == 0)
            {
                return SpellSlot.Unknown;
            }

            return ward.Contains(SpellSlot.Trinket) ? SpellSlot.Trinket : ward.FirstOrDefault();
        }

        public static float GetGapcloseDamage(this Obj_AI_Base target, Obj_AI_Base gapclose)
        {
            var q = SpellManager.Q.IsReady() && gapclose.LSDistance(target) < SpellManager.Q.Range &&
                    SpellManager.Q.IsActive(true);
            var w = SpellManager.W.IsReady() && gapclose.LSDistance(target) < SpellManager.W.Range &&
                    SpellManager.W.IsActive(true);
            var r = IsRReady() && gapclose.LSDistance(target) < SpellManager.R.Range && SpellManager.R.IsActive(true);
            return GetComboDamage(target, q, w, false, r, true);
        }

        public static float GetGapcloseDamage(this Obj_AI_Base target, Vector3 position)
        {
            var q = SpellManager.Q.IsReady() && target.LSDistance(position) + 15 <= SpellManager.Q.Range &&
                    SpellManager.Q.IsActive(true);
            var w = SpellManager.W.IsReady() && target.LSDistance(position) + 15 <= SpellManager.W.Range &&
                    SpellManager.W.IsActive(true);
            var r = IsRReady() && target.LSDistance(position) + 15 <= SpellManager.R.Range &&
                    SpellManager.R.IsActive(true);
            return GetComboDamage(target, q, w, false, r, true);
        }

        public static float GetKSDamage(this Obj_AI_Base target, bool gapcloseE = false)
        {
            var q = SpellManager.Q.IsReady() && SpellManager.Q.IsActive(true);
            var w = SpellManager.W.IsReady() && SpellManager.W.IsActive(true);
            var e = !gapcloseE && SpellManager.E.IsReady() && SpellManager.E.IsActive(true);
            var r = SpellManager.R.IsReady() && SpellManager.R.IsActive(true);
            return target.GetComboDamage(q, w, e, r, true);
        }

        public static float GetComboDamage(this Obj_AI_Base target, params Spell[] spells)
        {
            return spells.Sum(spell => spell.GetDamage(target)) +
                   (spells.Contains(SpellManager.Q) && spells.Length > 1 ? SpellManager.Q.GetDamage(target, 1) : 0);
        }

        public static float GetComboDamage(this Obj_AI_Base unit,
            bool q,
            bool w,
            bool e,
            bool r,
            bool calculateUltTick = false,
            bool damageIndicator = false)
        {
            if (!unit.IsValidTarget())
            {
                return 0;
            }

            var d = 0f;

            if (q)
            {
                d += SpellManager.Q.GetDamage(unit);

                if (unit.HasBuff("katarinaqmark"))
                {
                    d += SpellManager.Q.GetDamage(unit, 1);
                }
            }

            if (w)
            {
                d += SpellManager.W.GetDamage(unit);
            }

            if (e)
            {
                d += SpellManager.E.GetDamage(unit);
            }

            if (r)
            {
                if (!calculateUltTick)
                {
                    d += SpellManager.R.GetDamage(unit, 1);
                }
                else
                {
                    d += unit.GetCalculatedRDamage(Katarina.UltTicks);
                }
            }

            d += ObjectManager.Player.GetAutoAttackDamage(unit, true);

            var dl = ObjectManager.Player.GetMastery(MasteryData.Ferocity.DoubleEdgedSword);
            if (dl != null && dl.IsActive())
            {
                d *= 1.03f;
            }

            var assasin = ObjectManager.Player.GetMastery((MasteryData.Cunning) 83);
            if (assasin != null && assasin.IsActive() && ObjectManager.Player.CountAlliesInRange(800) == 0)
            {
                d *= 1.015f;
            }

            var igniteSlot = ObjectManager.Player.GetSpellSlot("summonerdot");

            if (!igniteSlot.Equals(SpellSlot.Unknown))
            {
                if (igniteSlot.IsReady())
                {
                    d += (float) ObjectManager.Player.GetSummonerSpellDamage(unit, Damage.SummonerSpell.Ignite);
                }
            }

            if (damageIndicator)
            {
                var tl = ObjectManager.Player.GetMastery(MasteryData.Cunning.ThunderlordsDecree);
                if (tl != null && tl.IsActive() && !ObjectManager.Player.HasBuff("masterylordsdecreecooldown"))
                {
                    d += 10*ObjectManager.Player.Level + .3f*ObjectManager.Player.FlatPhysicalDamageMod +
                         .1f*ObjectManager.Player.TotalMagicalDamage;
                }
            }

            if (ItemManager.LudensEcho != null)
            {
                var b = ObjectManager.Player.GetBuff("itemmagicshankcharge");
                if (b != null && b.IsActive && b.Count >= (damageIndicator ? 70 : 100))
                {
                    d += 100 + ObjectManager.Player.TotalMagicalDamage*.1f;
                }
            }

            return d;
        }

        public static float GetComboDamage(Obj_AI_Base unit)
        {
            return unit.GetComboDamage(
                SpellManager.Q.IsReady(), SpellManager.W.IsReady(), SpellManager.E.IsReady(), IsRReady(), true, true);
        }

        public static float GetTimeToUnit(Obj_AI_Base unit)
        {
            var path = ObjectManager.Player.GetPath(unit.ServerPosition);

            if (path.Length == 0)
            {
                return 0;
            }

            var d = 0f;
            var lastPoint = path.FirstOrDefault();
            foreach (var point in path)
            {
                d += lastPoint.LSDistance(point);
                lastPoint = point;
            }

            return d/ObjectManager.Player.MoveSpeed;
        }

        public static float GetCalculatedRDamage(this Obj_AI_Base target, int ticks)
        {
            var dmg = ObjectManager.Player.GetDamageSpell(target, SpellSlot.R).CalculatedDamage;
            return (float) (dmg*ticks);
        }
    }
}