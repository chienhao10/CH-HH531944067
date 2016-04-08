using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy;
using EloBuddy.SDK;
using SharpDX;

namespace ezEvade.SpecialSpells
{
    class Ekko : ChampionPlugin
    {
        static Ekko()
        {

        }

        public void LoadSpecialSpell(SpellData spellData)
        {
            if (spellData.spellName == "EkkoR")
            {
                SpellDetector.OnProcessSpecialSpell += ProcessSpell_EkkoR;
            }   
        }

        private static void ProcessSpell_EkkoR(Obj_AI_Base hero, GameObjectProcessSpellCastEventArgs args, SpellData spellData,
            SpecialSpellEventArgs specialSpellArgs)
        {
            if (spellData.spellName == "EkkoR")
            {
                foreach (var obj in ObjectManager.Get<Obj_AI_Minion>())
                {
                    if (obj != null && obj.IsValid && !obj.IsDead && obj.Name == "Ekko" && obj.IsEnemy)
                    {
                        var blinkPos = obj.ServerPosition.To2D();

                        SpellDetector.CreateSpellData(hero, args.Start, blinkPos.To3D(), spellData);
                    }
                }

                specialSpellArgs.noProcess = true;
            }
        }
    }
}
