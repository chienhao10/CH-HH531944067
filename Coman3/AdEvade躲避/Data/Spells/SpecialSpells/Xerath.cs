using System;
using EloBuddy;
using EloBuddy.SDK;

namespace AdEvade.Data.Spells.SpecialSpells
{
    class Xerath : IChampionPlugin
    {
        static Xerath()
        {

        }
        public const string ChampionName = "Xerath";
        public string GetChampionName()
        {
            return ChampionName;
        }
        public void LoadSpecialSpell(SpellData spellData)
        {
            if (spellData.SpellName == "xeratharcanopulse2")
            {
                SpellDetector.OnProcessSpecialSpell += ProcessSpell_XerathArcanopulse2;
            }
        }

        private static void ProcessSpell_XerathArcanopulse2(Obj_AI_Base hero, GameObjectProcessSpellCastEventArgs args, SpellData spellData, SpecialSpellEventArgs specialSpellArgs)
        {
            if (args.SData.Name == "XerathArcanopulseChargeUp")// || spellData.spellName == "xeratharcanopulse2")
            {
                var castTime = -1 * (hero.Spellbook.CastTime - Game.Time) * 1000;

                if (castTime > 0)
                {
                    var dir = (args.End.To2D() - args.Start.To2D()).Normalized();
                    var endPos = args.Start.To2D() + dir * Math.Min(spellData.Range, 750 + castTime / 2);
                    SpellDetector.CreateSpellData(hero, args.Start, endPos.To3D(), spellData);
                }

                specialSpellArgs.NoProcess = true;
            }
        }
    }
}
