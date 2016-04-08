using EloBuddy;
using EloBuddy.SDK;

namespace AdEvade.Data.Spells.SpecialSpells
{
    class Malzahar : IChampionPlugin
    {
        static Malzahar()
        {

        }
        public const string ChampionName = "Malzahar";
        public string GetChampionName()
        {
            return ChampionName;
        }
        public void LoadSpecialSpell(SpellData spellData)
        {
            if (spellData.SpellName == "AlZaharCalloftheVoid")
            {
                SpellDetector.OnProcessSpecialSpell += ProcessSpell_AlZaharCalloftheVoid;
            }
        }

        private static void ProcessSpell_AlZaharCalloftheVoid(Obj_AI_Base hero, GameObjectProcessSpellCastEventArgs args, SpellData spellData,
            SpecialSpellEventArgs specialSpellArgs)
        {
            if (spellData.SpellName == "AlZaharCalloftheVoid")
            {
                var direction = (args.End.To2D() - args.Start.To2D()).Normalized();
                var pDirection = direction.Perpendicular();
                var targetPoint = args.End.To2D();

                var pos1 = targetPoint - pDirection * spellData.SideRadius;
                var pos2 = targetPoint + pDirection * spellData.SideRadius;

                SpellDetector.CreateSpellData(hero, pos1.To3D(), pos2.To3D(), spellData, null, 0, false);
                SpellDetector.CreateSpellData(hero, pos2.To3D(), pos1.To3D(), spellData, null, 0);

                specialSpellArgs.NoProcess = true;
            }
        }
    }
}
