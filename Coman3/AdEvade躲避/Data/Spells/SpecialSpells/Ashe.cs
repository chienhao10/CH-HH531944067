using AdEvade.Utils;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;

namespace AdEvade.Data.Spells.SpecialSpells
{
    class Ashe : IChampionPlugin
    {
        static Ashe()
        {

        }
        public const string ChampionName = "Ashe";
        public string GetChampionName()
        {
            return ChampionName;
        }
        public void LoadSpecialSpell(SpellData spellData)
        {
            if (spellData.SpellName == "Volley")
            {
                SpellDetector.OnProcessSpecialSpell += ProcessSpell_AsheVolley;
            }
        }

        private static void ProcessSpell_AsheVolley(Obj_AI_Base hero, GameObjectProcessSpellCastEventArgs args, SpellData spellData, SpecialSpellEventArgs specialSpellArgs)
        {
            if (spellData.SpellName == "Volley")
            {
                for (int i = -4; i < 5; i++)
                {
                    Vector3 endPos2 = MathUtils.RotateVector(args.Start.To2D(), args.End.To2D(), i * spellData.Angle).To3D();
                    if (i != 0)
                    {
                        SpellDetector.CreateSpellData(hero, args.Start, endPos2, spellData, null, 0, false);
                    }
                }
            }
        }
    }
}
