using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using Color = System.Drawing.Color;
using LS = LeagueSharp.Common;
using SV = SoloVayne.Skills.Tumble;
using SOLOVayne.Utility.General;
using SAutoCarry.Champions.Helpers;

namespace Vayne
{
    //github.com/dakotasblack
    class AutoLevel
    {

        public static int QOff = 0, WOff = 0, EOff = 0, ROff;

        public static int[] Level = { 0, 0, 0, 0 };

        public static int[] AbilitySequence;

        public static AIHeroClient myHero
        {
            get { return EloBuddy.Player.Instance; }
        }

        public static void _AutoSpell()
        {
            AbilitySequence = new[] { 1, 2, 3, 2, 2, 4, 1, 1, 1, 1, 4, 2, 2, 3, 3, 4, 3, 3 };
            var qL = Vayne.Program.Q.Level + QOff;
            var wL = Vayne.Program.W.Level + WOff;
            var eL = Vayne.Program.E.Level + EOff;
            var rL = Vayne.Program.R.Level + ROff;

            Level = new[] { 0, 0, 0, 0 };

            for (var i = 1; i <= ObjectManager.Player.Level; i++)
            {
                switch (AbilitySequence[i - 1])
                {
                    case 1:
                        Level[0] += 1;
                        break;
                    case 2:
                        Level[1] += 1;
                        break;
                    case 3:
                        Level[2] += 1;
                        break;
                    case 4:
                        Level[3] += 1;
                        break;
                }
            }

            if (qL < Level[0])
            {
                LevelUp(SpellSlot.Q);
            }

            if (wL < Level[1])
            {
                LevelUp(SpellSlot.W);
            }

            if (eL < Level[2])
            {
                LevelUp(SpellSlot.E);
            }

            if (rL < Level[3])
            {
                LevelUp(SpellSlot.R);
            }
        }

        public static void LevelUp(SpellSlot slot)
        {
            Core.DelayAction(() => { myHero.Spellbook.LevelSpell(slot); }, 150);
        }
    }
}
