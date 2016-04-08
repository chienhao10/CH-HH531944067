using System.Linq;
using EloBuddy;
using EloBuddy.SDK;

namespace AdEvade.Data.Spells.SpecialSpells
{
    class Yasuo : IChampionPlugin
    {
        static Yasuo()
        {

        }
        public const string ChampionName = "Yasuo";
        public string GetChampionName()
        {
            return ChampionName;
        }
        public void LoadSpecialSpell(SpellData spellData)
        {
            if (spellData.SpellName == "YasuoQW")
            {
                AIHeroClient hero = EntityManager.Heroes.Enemies.FirstOrDefault(h => h.ChampionName == "Yasuo");
                if (hero == null)
                {
                    return;
                }

                Obj_AI_Base.OnProcessSpellCast += (sender, args) => ProcessSpell_YasuoQW(sender, args, spellData);
            }

            if (spellData.SpellName == "yasuoq3w")
            {
                AIHeroClient hero = EntityManager.Heroes.Enemies.FirstOrDefault(h => h.ChampionName == "Yasuo");
                if (hero == null)
                {
                    return;
                }

                Obj_AI_Base.OnProcessSpellCast += (sender, args) => ProcessSpell_YasuoQW(sender, args, spellData);
            } 
        }

        private static void ProcessSpell_YasuoQW(Obj_AI_Base hero, GameObjectProcessSpellCastEventArgs args, SpellData spellData)
        {
            if (hero.IsEnemy && args.SData.Name == "yasuoq")
            {
                var castTime = (hero.Spellbook.CastTime - Game.Time) * 1000;

                if (castTime > 0)
                {
                    spellData.SpellDelay = castTime;
                }
            }
        }
    }
}
