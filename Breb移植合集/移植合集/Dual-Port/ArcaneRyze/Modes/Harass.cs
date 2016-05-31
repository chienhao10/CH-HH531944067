#region

using LeagueSharp.SDK;
using static Arcane_Ryze.Core;

#endregion

namespace Arcane_Ryze.Modes
{
    internal class Harass
    {
        public static void HarassLogic()
        {
            if(Target.LSIsValidTarget() && Target != null)
            {
                if(PassiveStack < 4)
                {
                    if(Spells.E.IsReady())
                    {
                        Spells.E.Cast(Target);
                    }
                    else if(Spells.W.IsReady())
                    {
                        Spells.W.Cast(Target);
                    }
                }
            }
        }
    }
}
