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
    class Lucian : ChampionPlugin
    {
        static Lucian()
        {

        }

        public void LoadSpecialSpell(SpellData spellData)
        {
            if (spellData.spellName == "LucianQ")
            {
                SpellDetector.OnProcessSpecialSpell += ProcessSpell_LucianQ;
            }
        }

        private static void ProcessSpell_LucianQ(Obj_AI_Base hero, GameObjectProcessSpellCastEventArgs args, SpellData spellData, SpecialSpellEventArgs specialSpellArgs)
        {
            if (spellData.spellName == "LucianQ")
            {

                if (args.Target.GetType() == typeof(Obj_AI_Base) && ((Obj_AI_Base)args.Target).IsValid())
                {
                    var target = args.Target as Obj_AI_Base;

                    float spellDelay = ((float)(350 - ObjectCache.gamePing)) / 1000;
                    var heroWalkDir = (target.ServerPosition - target.Position).Normalized();
                    var predictedHeroPos = target.Position + heroWalkDir * target.MoveSpeed * (spellDelay);


                    SpellDetector.CreateSpellData(hero, args.Start, predictedHeroPos, spellData, null, 0);

                    specialSpellArgs.noProcess = true;
                }
            }
        }
    }
}
