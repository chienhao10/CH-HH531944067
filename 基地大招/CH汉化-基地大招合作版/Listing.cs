using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace HumanziedBaseUlt
{
    static class Listing
    {
        public static Menu allyconfig;
        public static Menu potionMenu;
        public static Menu snipeMenu;
        public static Menu config;
        public static Menu MiscMenu;

        public static Menu recallTrackerMenu;

        public class UltSpellDataS
        {
            public string championName;
            public int SpellStage;
            public float DamageMultiplicator;
            public float Width;
            public float Delay;
            public float Speed;
            public bool Collision;
        }

        public static readonly Dictionary<string, UltSpellDataS> UltSpellDataList = new Dictionary<string, UltSpellDataS>
        {
            {"Jinx",    new UltSpellDataS { SpellStage = 1, DamageMultiplicator = 1f, Width = 140f, Delay = 700f/1000f, Speed = 1700f, Collision = true}},
            {"Ashe",    new UltSpellDataS { SpellStage = 0, DamageMultiplicator = 1.0f, Width = 130f, Delay = 0250f/1000f, Speed = 1600f, Collision = true}},
            {"Draven",  new UltSpellDataS { SpellStage = 0, DamageMultiplicator = 0.7f, Width = 160f, Delay = 0400f/1000f, Speed = 2000f, Collision = true}},
            {"Ezreal",  new UltSpellDataS { SpellStage = 0, DamageMultiplicator = 0.7f, Width = 160f, Delay = 1000f/1000f, Speed = 2000f, Collision = false}},
            {"Karthus", new UltSpellDataS { SpellStage = 0, DamageMultiplicator = 1.0f, Width = 000f, Delay = 3125f/1000f, Speed = 0000f, Collision = false}}
        };

        public class PortingEnemy
        {
            public AIHeroClient Sender { get; set; }
            /// <summary>
            /// in ms
            /// </summary>
            public int StartTick { get; set; }

            /// <summary>
            /// in ms
            /// </summary>
            public int Duration { get; set; }
        }
        public static readonly List<PortingEnemy> teleportingEnemies = new List<PortingEnemy>(5);

        public static readonly List<AIHeroClient> visibleEnemies = new List<AIHeroClient>(5);
        public static readonly List<Events.InvisibleEventArgs> invisEnemiesList = new List<Events.InvisibleEventArgs>(5);

        public static class Regeneration
        {
            public static float GetNormalRegenRate(AIHeroClient enemy)
            {
                bool hasbuff = HasPotionActive(enemy);
                BuffInstance buff = hasbuff ? GetPotionBuff(enemy) : null;

                float val = hasbuff
                    ? enemy.HPRegenRate - GetPotionRegenRate(buff)
                    : enemy.HPRegenRate;

                if (hasbuff && val < 0) //HPRegenRate not updated on potion
                    val = enemy.HPRegenRate;

                return val;
            }

            public static bool HasPotionActive(AIHeroClient hero)
            {
                string[] potionStrings = {
                    RegenerationSpellBook.HealthPotion.BuffName,
                    RegenerationSpellBook.HealthPotion.BuffNameCookie,
                    RegenerationSpellBook.RefillablePotion.BuffName,
                    RegenerationSpellBook.CorruptingPotion.BuffName,
                    RegenerationSpellBook.HuntersPotion.BuffName
                };

                return hero.Buffs.Any(x => potionStrings.Contains(x.Name));
            }

            public static BuffInstance GetPotionBuff(AIHeroClient hero)
            {
                string[] potionStrings = {
                    RegenerationSpellBook.HealthPotion.BuffName,
                    RegenerationSpellBook.HealthPotion.BuffNameCookie,
                    RegenerationSpellBook.RefillablePotion.BuffName,
                    RegenerationSpellBook.CorruptingPotion.BuffName,
                    RegenerationSpellBook.HuntersPotion.BuffName
                };

                return hero.Buffs.First(x => potionStrings.Contains(x.Name));
            }

            /// <summary>
            /// per second
            /// </summary>
            /// <param name="buff"></param>
            /// <returns></returns>
            public static float GetPotionRegenRate(BuffInstance buff)
            {
                if (buff.Name == RegenerationSpellBook.HealthPotion.BuffName ||
                    buff.Name == RegenerationSpellBook.HealthPotion.BuffNameCookie)
                {
                    return RegenerationSpellBook.HealthPotion.RegenRate;
                }
                if (buff.Name == RegenerationSpellBook.RefillablePotion.BuffName)
                {
                    return RegenerationSpellBook.RefillablePotion.RegenRate;
                }
                if (buff.Name == RegenerationSpellBook.CorruptingPotion.BuffName)
                {
                    return RegenerationSpellBook.CorruptingPotion.RegenRate;
                }
                if (buff.Name == RegenerationSpellBook.HuntersPotion.BuffName)
                {
                    return RegenerationSpellBook.HuntersPotion.RegenRate;
                }

                return float.NaN;
            }
        }

        /// <summary>
        /// Regens per second
        /// </summary>
        private class RegenerationSpellBook
        {
            public static class HealthPotion
            {
                public static string BuffName = "RegenerationPotion";
                public static string BuffNameCookie = "ItemMiniRegenPotion";
                public static float RegenRate {
                    get { return potionMenu["healPotionRegVal"].Cast<Slider>().CurrentValue; }
                }
                public static float Duration = 15000;
            }
            public static class RefillablePotion
            {
                public static string BuffName = "ItemCrystalFlask";
                public static float RegenRate
                {
                    get { return potionMenu["crystalFlaskRegVal"].Cast<Slider>().CurrentValue; }
                }
                public static float Duration = 12000;
            }
            
            public static class HuntersPotion
            {
                public static string BuffName = "ItemCrystalFlaskJungle";
                public static float RegenRate
                {
                    get { return potionMenu["crystalFlaskJungleRegVal"].Cast<Slider>().CurrentValue; }
                }
                public static float Duration = 8000;
            }
            public static class CorruptingPotion
            {
                public static string BuffName = "ItemDarkCrystalFlask";
                public static float RegenRate
                {
                    get { return potionMenu["darkCrystalFlaskVal"].Cast<Slider>().CurrentValue; }
                }
                public static float Duration = 12000;
            }
        }

        public static class Pathing
        {
            public static readonly Dictionary<AIHeroClient, Vector3[]> enemiesPaths = new Dictionary<AIHeroClient, Vector3[]>(5);
            public static readonly List<SnipePrediction> enemySnipeProcs = new List<SnipePrediction>(5);

            public static void UpdateEnemyPaths()
            {
                foreach (AIHeroClient enemy in EntityManager.Heroes.Enemies.Where(x => x.IsHPBarRendered))
                {
                    if (enemiesPaths.ContainsKey(enemy))
                        enemiesPaths[enemy] = enemy.RealPath();
                    else
                        enemiesPaths.Add(enemy, enemy.RealPath());
                }
            }

            public static Vector3[] GetLastEnemyPath(AIHeroClient enemy)
            {
                return enemiesPaths.FirstOrDefault(x => x.Key.Equals(enemy)).Value;
            }
        }
    }
}
