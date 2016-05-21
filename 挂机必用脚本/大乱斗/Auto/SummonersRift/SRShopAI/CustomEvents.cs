using System;
using System.Collections.Generic;
using EloBuddy;

namespace AutoSharp.Auto.SummonersRift.SRShopAI
{
    public class CustomEvents
    {
        public delegate void OnSpawned(AIHeroClient sender, EventArgs args);
        private static readonly Dictionary<int, bool> Heroes = new Dictionary<int, bool>();

        static CustomEvents()
        {
            Game.OnUpdate += OnGameUpdate;
        }

        public static event OnSpawned OnSpawn;

        private static void OnGameUpdate(EventArgs args)
        {
            var spawnHandler = OnSpawn;
            if (spawnHandler == null)
                return;

            foreach (var hero in ObjectManager.Get<AIHeroClient>())
            {
                if (Heroes.ContainsKey(hero.NetworkId))
                {
                    bool death;
                    if (Heroes.TryGetValue(hero.NetworkId, out death))
                    {
                        if (death && !hero.IsDead)
                            spawnHandler(hero, new EventArgs());

                        Heroes[hero.NetworkId] = hero.IsDead;
                    }
                }
                else
                    Heroes.Add(hero.NetworkId, hero.IsDead);
            }
        }
    }
}
