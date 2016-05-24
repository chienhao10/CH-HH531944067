using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using LeagueSharp.Common;

namespace ezEvade
{
    class SpecialEvadeSpell
    {
        private static AIHeroClient myHero { get { return ObjectManager.Player; } }

        public static void LoadSpecialSpell(EvadeSpellData spellData)
        {
            if (spellData.spellName == "EkkoEAttack")
            {
                spellData.useSpellFunc = UseEkkoE2;
            }

            if (spellData.spellName == "EkkoR")
            {
                spellData.useSpellFunc = UseEkkoR;
            }
        }

        public static bool UseEkkoE2(EvadeSpellData evadeSpell, bool process = true)
        {
            if (myHero.HasBuff("ekkoeattackbuff"))
            {
                var posInfo = EvadeHelper.GetBestPositionTargetedDash(evadeSpell);
                if (posInfo != null && posInfo.target != null)
                {
                    EvadeSpell.CastEvadeSpell(() => EvadeCommand.Attack(evadeSpell, posInfo.target), process);
                    //DelayAction.Add(50, () => myHero.IssueOrder(GameObjectOrder.MoveTo, posInfo.position.To3D()));
                    return true;
                }
            }

            return false;
        }

        public static bool UseEkkoR(EvadeSpellData evadeSpell, bool process = true)
        {
            if ((from obj in ObjectManager.Get<Obj_AI_Minion>() where obj != null && obj.IsValid && !obj.IsDead && obj.Name == "Ekko" && obj.IsAlly select obj.ServerPosition.LSTo2D()).Any(blinkPos => !blinkPos.CheckDangerousPos(10)))
            {
                EvadeSpell.CastEvadeSpell(() => EvadeCommand.CastSpell(evadeSpell), process);
                //DelayAction.Add(50, () => myHero.IssueOrder(GameObjectOrder.MoveTo, posInfo.position.To3D()));
                return true;
            }

            return false;
        }
    }
}
