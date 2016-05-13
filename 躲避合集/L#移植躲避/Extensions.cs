using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

using Color = System.Drawing.Color;

namespace Evade
{
    static class Extensions
    {
        public static List<Vector2> GetWaypoints(this Obj_AI_Base unit)
        {
            var result = new List<Vector2>();

            if (unit.IsVisible)
            {
                result.Add(unit.ServerPosition.To2D());
                var path = unit.Path;
                if (path.Length > 0)
                {
                    var first = path[0].To2D();
                    if (first.Distance(result[0], true) > 40)
                    {
                        result.Add(first);
                    }

                    for (int i = 1; i < path.Length; i++)
                    {
                        result.Add(path[i].To2D());
                    }
                }
            }
            else if (WaypointTracker.StoredPaths.ContainsKey(unit.NetworkId))
            {
                var path = WaypointTracker.StoredPaths[unit.NetworkId];
                var timePassed = (Utils.TickCount - WaypointTracker.StoredTick[unit.NetworkId]) / 1000f;
                if (path.PathLength() >= unit.MoveSpeed * timePassed)
                {
                    result = CutPath(path, (int)(unit.MoveSpeed * timePassed));
                }
            }

            return result;
        }

        public static List<Vector2> CutPath(this List<Vector2> path, float distance)
        {
            var result = new List<Vector2>();
            var Distance = distance;
            if (distance < 0)
            {
                path[0] = path[0] + distance * (path[1] - path[0]).Normalized();
                return path;
            }

            for (var i = 0; i < path.Count - 1; i++)
            {
                var dist = path[i].Distance(path[i + 1]);
                if (dist > Distance)
                {
                    result.Add(path[i] + Distance * (path[i + 1] - path[i]).Normalized());
                    for (var j = i + 1; j < path.Count; j++)
                    {
                        result.Add(path[j]);
                    }

                    break;
                }
                Distance -= dist;
            }
            return result.Count > 0 ? result : new List<Vector2> { path.Last() };
        }
    }
    internal static class WaypointTracker
    {
        public static readonly Dictionary<int, List<Vector2>> StoredPaths = new Dictionary<int, List<Vector2>>();
        public static readonly Dictionary<int, int> StoredTick = new Dictionary<int, int>();
    }

    internal static class SGeometry
    {
        public static float PathLength(this List<Vector2> path)
        {
            var distance = 0f;
            for (var i = 0; i < path.Count - 1; i++)
            {
                distance += path[i].Distance(path[i + 1]);
            }
            return distance;
        }
    }

    public class SharpGeometry
    {
        public static Vector2[] CircleCircleIntersection(Vector2 center1, Vector2 center2, float radius1, float radius2)
        {
            var D = center1.Distance(center2);
            //The Circles dont intersect:
            if (D > radius1 + radius2 || (D <= Math.Abs(radius1 - radius2)))
            {
                return new Vector2[] {};
            }

            var A = (radius1*radius1 - radius2*radius2 + D*D)/(2*D);
            var H = (float) Math.Sqrt(radius1*radius1 - A*A);
            var Direction = (center2 - center1).Normalized();
            var PA = center1 + A*Direction;
            var S1 = PA + H*Direction.Perpendicular();
            var S2 = PA - H*Direction.Perpendicular();
            return new[] {S1, S2};
        }
    }

    static class EnumerableExtensions
    {
        public static T MinOrDefault<T, R>(this IEnumerable<T> container, Func<T, R> valuingFoo) where R : IComparable
        {
            var enumerator = container.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                return default(T);
            }

            var minElem = enumerator.Current;
            var minVal = valuingFoo(minElem);

            while (enumerator.MoveNext())
            {
                var currVal = valuingFoo(enumerator.Current);

                if (currVal.CompareTo(minVal) < 0)
                {
                    minVal = currVal;
                    minElem = enumerator.Current;
                }
            }

            return minElem;
        }
    }

    static class LastCastedSpell
    {
        /// <summary>
        /// The casted spells
        /// </summary>
        internal static readonly Dictionary<int, LastCastedSpellEntry> CastedSpells =
            new Dictionary<int, LastCastedSpellEntry>();

        /// <summary>
        /// The last cast packet sent
        /// </summary>
        public static LastCastPacketSentEntry LastCastPacketSent;

        /// <summary>
        /// Initializes static members of the <see cref="LastCastedSpell"/> class. 
        /// </summary>
        static LastCastedSpell()
        {
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Hero_OnProcessSpellCast;
            Spellbook.OnCastSpell += SpellbookOnCastSpell;
        }

        /// <summary>
        /// Fired then a spell is casted.
        /// </summary>
        /// <param name="spellbook">The spellbook.</param>
        /// <param name="args">The <see cref="SpellbookCastSpellEventArgs"/> instance containing the event data.</param>
        static void SpellbookOnCastSpell(Spellbook spellbook, SpellbookCastSpellEventArgs args)
        {
            if (spellbook.Owner.IsMe)
            {
                LastCastPacketSent = new LastCastPacketSentEntry(
                        args.Slot, Utils.TickCount, (args.Target is Obj_AI_Base) ? args.Target.NetworkId : 0);
            }
        }

        /// <summary>
        /// Fired when the game processes the spell cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GameObjectProcessSpellCastEventArgs"/> instance containing the event data.</param>
        private static void Obj_AI_Hero_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender is AIHeroClient)
            {
                var entry = new LastCastedSpellEntry(args.SData.Name, Utils.TickCount, ObjectManager.Player);
                if (CastedSpells.ContainsKey(sender.NetworkId))
                {
                    CastedSpells[sender.NetworkId] = entry;
                }
                else
                {
                    CastedSpells.Add(sender.NetworkId, entry);
                }
            }
        }

        /// <summary>
        /// Gets the last casted spell tick.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <returns></returns>
        public static int LastCastedSpellT(this AIHeroClient unit)
        {
            return CastedSpells.ContainsKey(unit.NetworkId) ? CastedSpells[unit.NetworkId].Tick : (Utils.TickCount > 0 ? 0 : int.MinValue);
        }

        /// <summary>
        /// Gets the last casted spell name.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <returns></returns>
        public static string LastCastedSpellName(this AIHeroClient unit)
        {
            return CastedSpells.ContainsKey(unit.NetworkId) ? CastedSpells[unit.NetworkId].Name : string.Empty;
        }

        /// <summary>
        /// Gets the last casted spell's target.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <returns></returns>
        public static Obj_AI_Base LastCastedSpellTarget(this AIHeroClient unit)
        {
            return CastedSpells.ContainsKey(unit.NetworkId) ? CastedSpells[unit.NetworkId].Target : null;
        }

        /// <summary>
        /// Gets the last casted spell.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <returns></returns>
        public static LastCastedSpellEntry LastCastedspell(this AIHeroClient unit)
        {
            return CastedSpells.ContainsKey(unit.NetworkId) ? CastedSpells[unit.NetworkId] : null;
        }
    }

    public class LastCastedSpellEntry
    {
        /// <summary>
        /// The name
        /// </summary>
        public string Name;

        /// <summary>
        /// The target
        /// </summary>
        public Obj_AI_Base Target;

        /// <summary>
        /// The tick
        /// </summary>
        public int Tick;

        /// <summary>
        /// Initializes a new instance of the <see cref="LastCastedSpellEntry"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="tick">The tick.</param>
        /// <param name="target">The target.</param>
        public LastCastedSpellEntry(string name, int tick, Obj_AI_Base target)
        {
            Name = name;
            Tick = tick;
            Target = target;
        }
    }

    public class LastCastPacketSentEntry
    {
        /// <summary>
        /// The slot
        /// </summary>
        public SpellSlot Slot;

        /// <summary>
        /// The target network identifier
        /// </summary>
        public int TargetNetworkId;

        /// <summary>
        /// The tick
        /// </summary>
        public int Tick;

        /// <summary>
        /// Initializes a new instance of the <see cref="LastCastPacketSentEntry"/> class.
        /// </summary>
        /// <param name="slot">The slot.</param>
        /// <param name="tick">The tick.</param>
        /// <param name="targetNetworkId">The target network identifier.</param>
        public LastCastPacketSentEntry(SpellSlot slot, int tick, int targetNetworkId)
        {
            Slot = slot;
            Tick = tick;
            TargetNetworkId = targetNetworkId;
        }
    }

    static class MenuExtensions
    {
        public static void AddStringList(this Menu m, string uniqueId, string displayName, string[] values, int defaultValue)
        {
            var mode = m.Add(uniqueId, new Slider(displayName, defaultValue, 0, values.Length - 1));
            mode.DisplayName = displayName + ": " + values[mode.CurrentValue];
            mode.OnValueChange += delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
            {
                sender.DisplayName = displayName + ": " + values[args.NewValue];
            };
        }

        public static Color AddColor(this Menu m, string[] uniqueIds, string displayName, Color defaultValue)
        {
            m.AddGroupLabel(displayName);

            string[] dp = new string[4];

            dp[0] = "A:";
            dp[1] = "R:";
            dp[2] = "G:";
            dp[3] = "B:";

            int[] df = new int[4];

            df[0] = defaultValue.A;
            df[1] = defaultValue.R;
            df[2] = defaultValue.G;
            df[3] = defaultValue.B;

            for (int x = -1; x <= 3; x++)
            {
                m.Add(uniqueIds[x], new Slider(dp[x], df[x], 1, 255));
            }
            var a = m[uniqueIds[0]].Cast<Slider>().CurrentValue;
            var r = m[uniqueIds[1]].Cast<Slider>().CurrentValue;
            var g = m[uniqueIds[2]].Cast<Slider>().CurrentValue;
            var b = m[uniqueIds[3]].Cast<Slider>().CurrentValue;

            Color color = Color.FromArgb(a, r, g, b);
            return color;
        }
    }

    static class Utility
    {
        public static bool IsWall(this Vector3 position)
        {
            return NavMesh.GetCollisionFlags(position).HasFlag(CollisionFlags.Wall);
        }

        public static bool IsWall(this Vector2 position)
        {
            return position.To3D().IsWall();
        }

        public static bool PlayerWindingUp;

        public static void OnPreAttack(AIHeroClient sender, Orbwalker.PreAttackArgs args)
        {
            if (sender.IsMe)
            {
                PlayerWindingUp = true;
            }
        }
    }

    static class Items
    {
        public static InventorySlot GetWardSlot()
        {
            var wardIds = new[] { 3340, 3350, 3361, 3154, 2045, 2049, 2050, 2044 };
            return (from wardId in wardIds
                    where Item.CanUseItem(wardId)
                    select ObjectManager.Player.InventoryItems.FirstOrDefault(slot => slot.Id == (ItemId)wardId))
                .FirstOrDefault();
        }
    }
}
