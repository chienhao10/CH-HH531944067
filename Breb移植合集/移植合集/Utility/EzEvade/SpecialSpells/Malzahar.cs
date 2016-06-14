using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;

namespace ezEvade.SpecialSpells
{
    class Malzahar : ChampionPlugin
    {
        static Malzahar()
        {

        }

        public void LoadSpecialSpell(SpellData spellData)
        {
            if (spellData.spellName == "MalzaharQ")
            {
                SpellDetector.OnProcessSpecialSpell += ProcessSpell_AlZaharCalloftheVoid;
            }
        }

        private static void ProcessSpell_AlZaharCalloftheVoid(Obj_AI_Base hero, GameObjectProcessSpellCastEventArgs args, SpellData spellData,
            SpecialSpellEventArgs specialSpellArgs)
        {
            if (spellData.spellName == "MalzaharQ")
            {
                var direction = (args.End.LSTo2D() - args.Start.LSTo2D()).LSNormalized();
                var pDirection = direction.LSPerpendicular();
                var targetPoint = args.End.LSTo2D();

                var pos1 = targetPoint - pDirection * spellData.sideRadius;
                var pos2 = targetPoint + pDirection * spellData.sideRadius;

                SpellDetector.CreateSpellData(hero, pos1.To3D(), pos2.To3D(), spellData, null, 0, false);
                SpellDetector.CreateSpellData(hero, pos2.To3D(), pos1.To3D(), spellData, null, 0);

                specialSpellArgs.noProcess = true;
            }
        }
    }
}
