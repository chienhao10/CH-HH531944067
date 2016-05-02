namespace Genesis.Library
{
    using System.Collections.Generic;
    using System.Linq;

    using Genesis.Library.Spells;

    using EloBuddy;
    using EloBuddy.SDK;

    public static class SpellManager
    {
        static SpellManager()
        {
            CurrentSpells = SpellLibrary.GetSpells(Player.Instance.Hero);
            SpellsDictionary = new Dictionary<AIHeroClient, SpellBase>();
        }

        public static SpellBase CurrentSpells { get; set; }

        public static Dictionary<AIHeroClient, SpellBase> SpellsDictionary { get; set; }

        public static float GetRange(SpellSlot slot, AIHeroClient sender)
        {
            switch (slot)
            {
                case SpellSlot.Q:
                    return SpellsDictionary.FirstOrDefault(x => x.Key == sender).Value.Q.Range;
                case SpellSlot.W:
                    return SpellsDictionary.FirstOrDefault(x => x.Key == sender).Value.W.Range;
                case SpellSlot.E:
                    return SpellsDictionary.FirstOrDefault(x => x.Key == sender).Value.E.Range;
                case SpellSlot.R:
                    return SpellsDictionary.FirstOrDefault(x => x.Key == sender).Value.R.Range;
                default:
                    return 0;
            }
        }

        public static void Initialize()
        {
            EntityManager.Heroes.AllHeroes.ForEach(PrepareSpells);
        }

        public static void PrepareSpells(AIHeroClient hero)
        {
            SpellBase spells = SpellLibrary.GetSpells(hero.Hero);
            //This only needs to be called once per champion, anymore is a memory leak.
            if (spells != null)
            {
                SpellsDictionary.Add(hero, spells);
            }
        }
    }
}