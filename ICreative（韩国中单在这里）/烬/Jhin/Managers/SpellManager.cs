using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using Jhin;
using Jhin.Model;

namespace Jhin.Managers
{
    public static class SpellManager
    {
        public static SpellBase Ignite;
        public static SpellBase Heal;
        public static SpellBase Smite;
        public static SpellBase Flash;

        public static HashSet<SpellSlot> SummonerSlots = new HashSet<SpellSlot>
        {
            SpellSlot.Summoner1,
            SpellSlot.Summoner2
        };

        public static HashSet<SpellSlot> ItemSlots = new HashSet<SpellSlot>
        {
            SpellSlot.Item1,
            SpellSlot.Item2,
            SpellSlot.Item3,
            SpellSlot.Item4,
            SpellSlot.Item5,
            SpellSlot.Item6,
            SpellSlot.Trinket
        };

        public static void Initialize()
        {
            Smite = new SpellBase(AIO.MyHero.GetSummonerSpellSlot("smite"), SpellType.Targeted, 500);
            Ignite = new SpellBase(AIO.MyHero.GetSummonerSpellSlot("summonerdot"), SpellType.Targeted, 600);
            Flash = new SpellBase(AIO.MyHero.GetSummonerSpellSlot("flash"), SpellType.Circular, 400) {Width = 100};
            Heal = new SpellBase(AIO.MyHero.GetSummonerSpellSlot("heal"), SpellType.Self);
        }

        public static SpellSlot GetSummonerSpellSlot(this AIHeroClient hero, string name)
        {
            foreach (
                var s in
                    hero.Spellbook.Spells.Where(
                        s => SummonerSlots.Contains(s.Slot) && s.Name.ToLower().Contains(name.ToLower())))
            {
                return s.Slot;
            }
            return SpellSlot.Unknown;
        }

        public static SpellSlot GetItemSpellSlot(this AIHeroClient hero, string name)
        {
            foreach (var s in hero.Spellbook.Spells.Where(s => ItemSlots.Contains(s.Slot)))
            {
                Chat.Print(s.Name.ToLower() + " " + name.ToLower() + " " + s.Name.ToLower().Contains(name.ToLower()));
                if (s.Name.ToLower().Contains(name.ToLower()))
                {
                    return s.Slot;
                }
            }
            return SpellSlot.Unknown;
        }

        public static SpellSlot GetSpellSlotFromSpellName(this AIHeroClient hero, string name)
        {
            foreach (var s in hero.Spellbook.Spells.Where(s => s.Name.ToLower().Contains(name.ToLower())))
            {
                return s.Slot;
            }
            return SpellSlot.Unknown;
        }

        public static SpellDataInst GetSpellDataInst(this SpellSlot slot)
        {
            return AIO.MyHero.Spellbook.GetSpell(slot);
        }
    }
}