using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace iKalistaReborn.Utils
{
    public static class SentinelManager
    {
        private enum SentinelLocations
        {
            Baron,
            Dragon,
            Mid,
            Blue,
            Red
        }

        private const float MaxRandomRadius = 15;
        private static readonly Random Random = new Random(DateTime.Now.Millisecond);
        private static readonly Dictionary<GameObjectTeam, Dictionary<SentinelLocations, Vector2>> Locations = new Dictionary<GameObjectTeam, Dictionary<SentinelLocations, Vector2>>
        {
            {
                GameObjectTeam.Order, new Dictionary<SentinelLocations, Vector2>
                {
                    { SentinelLocations.Mid, new Vector2(8428, 6465) },
                    { SentinelLocations.Blue, new Vector2(3871.489f, 7901.054f) },
                    { SentinelLocations.Red, new Vector2(7862.244f, 4111.187f) }
                }
            },
            {
                GameObjectTeam.Chaos, new Dictionary<SentinelLocations, Vector2>
                {
                    { SentinelLocations.Mid, new Vector2(6545, 8361) },
                    { SentinelLocations.Blue, new Vector2(10931.73f, 6990.844f) },
                    { SentinelLocations.Red, new Vector2(7016.869f, 10775.55f) }
                }
            },
            {
                GameObjectTeam.Neutral, new Dictionary<SentinelLocations, Vector2>
                {
                    { SentinelLocations.Baron, new Vector2(5007.124f, 10471.45f) },
                    { SentinelLocations.Dragon, new Vector2(9866.148f, 4414.014f) }
                }
            }
        };

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        private static readonly Dictionary<SentinelLocations, Func<bool>> EnabledLocations = new Dictionary<SentinelLocations, Func<bool>>
        {
            { SentinelLocations.Baron, () => getCheckBoxItem(Kalista.miscMenu, "baron") },
            { SentinelLocations.Blue, () => getCheckBoxItem(Kalista.miscMenu, "blue") },
            { SentinelLocations.Dragon, () => getCheckBoxItem(Kalista.miscMenu, "dragon") },
            { SentinelLocations.Mid, () => getCheckBoxItem(Kalista.miscMenu, "mid") },
            { SentinelLocations.Red, () => getCheckBoxItem(Kalista.miscMenu, "red") },
        };

        private static readonly List<Tuple<GameObjectTeam, SentinelLocations>> OpenLocations = new List<Tuple<GameObjectTeam, SentinelLocations>>();
        private static readonly Dictionary<GameObjectTeam, Dictionary<SentinelLocations, Obj_AI_Base>> ActiveSentinels = new Dictionary<GameObjectTeam, Dictionary<SentinelLocations, Obj_AI_Base>>();
        private static Tuple<GameObjectTeam, SentinelLocations> SentLocation { get; set; }

        static SentinelManager()
        {
            if (Game.MapId == GameMapId.SummonersRift)
            {
                // Listen to required events
                Game.OnTick += OnTick;
                GameObject.OnCreate += OnCreate;

                // Recalculate open sentinel locations
                RecalculateOpenLocations();
            }
        }

        public static void Initialize()
        {
        }

        public static LeagueSharp.Common.Spell W = new LeagueSharp.Common.Spell(SpellSlot.W, 5000);

        private static void OnTick(EventArgs args)
        {
            // Validate all sentinels
            foreach (var entry in ActiveSentinels.ToArray())
            {
                if (getCheckBoxItem(Kalista.miscMenu, "alert") && entry.Value.Any(o => o.Value.Health == 1))
                {
                    var activeSentinel = entry.Value.First(o => o.Value.Health == 1);
                    Chat.Print("[Kalista] Sentinel at {0} taking damage! (local ping)", string.Concat((entry.Key == GameObjectTeam.Order ? "Blue-Jungle" : entry.Key == GameObjectTeam.Chaos ? "Red-Jungle" : "Lake"), " (", activeSentinel.Key, ")"));
                    TacticalMap.ShowPing(PingCategory.Fallback, activeSentinel.Value.Position, true);
                }

                var invalid = entry.Value.Where(o => !o.Value.IsValid || o.Value.Health < 2 || o.Value.GetBuffCount("kalistaw") == 0).ToArray();
                if (invalid.Length > 0)
                {
                    foreach (var location in invalid)
                    {
                        ActiveSentinels[entry.Key].Remove(location.Key);
                    }
                    RecalculateOpenLocations();
                }
            }

            // Auto sentinel management
            if (getCheckBoxItem(Kalista.miscMenu, "enabled") && W.IsReady() && Player.Instance.ManaPercent >= getSliderItem(Kalista.miscMenu, "mana") && !Player.Instance.IsRecalling())
            {
                if (!getCheckBoxItem(Kalista.miscMenu, "noMode") || Orbwalker.ActiveModesFlags == Orbwalker.ActiveModes.None)
                {
                    if (OpenLocations.Count > 0 && SentLocation == null)
                    {
                        var closestLocation = OpenLocations.Where(o => Locations[o.Item1][o.Item2].IsInRange(Player.Instance, W.Range - MaxRandomRadius / 2))
                            .OrderByDescending(o => Locations[o.Item1][o.Item2].Distance(Player.Instance, true))
                            .FirstOrDefault();
                        if (closestLocation != null)
                        {
                            var position = Locations[closestLocation.Item1][closestLocation.Item2];
                            var randomized = (new Vector2(position.X - MaxRandomRadius / 2 + Random.NextFloat(0, MaxRandomRadius),
                                position.Y - MaxRandomRadius / 2 + Random.NextFloat(0, MaxRandomRadius))).To3DWorld();
                            SentLocation = closestLocation;
                            W.Cast(randomized);
                            Core.DelayAction(() => SentLocation = null, 2000);
                        }
                    }
                }
            }
        }

        public static void RecalculateOpenLocations()
        {
            OpenLocations.Clear();
            foreach (var location in Locations)
            {
                if (!ActiveSentinels.ContainsKey(location.Key))
                {
                    OpenLocations.AddRange(location.Value.Where(o => EnabledLocations[o.Key]()).Select(loc => new Tuple<GameObjectTeam, SentinelLocations>(location.Key, loc.Key)));
                }
                else
                {
                    OpenLocations.AddRange(from loc in location.Value where EnabledLocations[loc.Key]() && !ActiveSentinels[location.Key].ContainsKey(loc.Key) select new Tuple<GameObjectTeam, SentinelLocations>(location.Key, loc.Key));
                }
            }
        }

        private static void OnCreate(GameObject sender, EventArgs args)
        {
            if (SentLocation == null)
            {
                return;
            }

            var sentinel = sender as Obj_AI_Minion;
            if (sentinel != null && sentinel.IsAlly && sentinel.MaxHealth == 2 && sentinel.Name == "RobotBuddy")
            {
                Core.DelayAction(() => ValidateSentinel(sentinel), 1000);
            }
        }

        private static void ValidateSentinel(Obj_AI_Base sentinel)
        {
            if (sentinel.Health == 2 && sentinel.GetBuffCount("kalistaw") == 1)
            {
                if (!ActiveSentinels.ContainsKey(SentLocation.Item1))
                {
                    ActiveSentinels.Add(SentLocation.Item1, new Dictionary<SentinelLocations, Obj_AI_Base>());
                }
                ActiveSentinels[SentLocation.Item1].Remove(SentLocation.Item2);
                ActiveSentinels[SentLocation.Item1].Add(SentLocation.Item2, sentinel);

                SentLocation = null;
                RecalculateOpenLocations();
            }
        }
    }
}