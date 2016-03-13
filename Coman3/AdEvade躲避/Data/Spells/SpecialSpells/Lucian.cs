using EloBuddy;
using EloBuddy.SDK;

namespace AdEvade.Data.Spells.SpecialSpells
{
    class Lucian : IChampionPlugin
    {
        static Lucian()
        {

        }
        public const string ChampionName = "Lucian";
        public string GetChampionName()
        {
            return ChampionName;
        }
        public void LoadSpecialSpell(SpellData spellData)
        {
            if (spellData.SpellName == "LucianQ")
            {
                SpellDetector.OnProcessSpecialSpell += ProcessSpell_LucianQ;
            }
        }

        private static void ProcessSpell_LucianQ(Obj_AI_Base hero, GameObjectProcessSpellCastEventArgs args, SpellData spellData, SpecialSpellEventArgs specialSpellArgs)
        {
            if (spellData.SpellName == "LucianQ")
            {

                if (args.Target.GetType() == typeof(Obj_AI_Base) && ((Obj_AI_Base)args.Target).IsValid())
                {
                    var target = args.Target as Obj_AI_Base;

                    float spellDelay = ((float)(350 - Game.Ping)) / 1000;
                    var heroWalkDir = (target.ServerPosition - target.Position).Normalized();
                    var predictedHeroPos = target.Position + heroWalkDir * target.MoveSpeed * (spellDelay);


                    SpellDetector.CreateSpellData(hero, args.Start, predictedHeroPos, spellData, null, 0);

                    specialSpellArgs.NoProcess = true;
                }
            }
        }
    }
}
