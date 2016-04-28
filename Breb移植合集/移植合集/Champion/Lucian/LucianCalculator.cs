using EloBuddy;
using LeagueSharp.Common;

namespace LCS_Lucian
{
    internal class LucianCalculator
    {
        public static float LucianTotalDamage(AIHeroClient enemy)
        {
            if (LucianSpells.Q.IsReady() && Program.getCheckBoxItem(Program.comboMenu, "lucian.q.combo"))
            {
                return LucianSpells.Q.GetDamage(enemy);
            }
            if (LucianSpells.W.IsReady() && Program.getCheckBoxItem(Program.comboMenu, "lucian.w.combo"))
            {
                return LucianSpells.W.GetDamage(enemy);
            }
            if (LucianSpells.R.IsReady() && Program.getCheckBoxItem(Program.comboMenu, "lucian.r.combo"))
            {
                return LucianSpells.R.GetDamage(enemy);
            }
            return 0;
        }
    }
}