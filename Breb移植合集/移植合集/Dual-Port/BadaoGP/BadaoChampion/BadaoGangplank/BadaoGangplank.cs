using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace BadaoKingdom.BadaoChampion.BadaoGangplank
{
    public static class BadaoGangplank
    {
        public static void BadaoActivate()
        {
            BadaoGangplankConfig.BadaoActivate();
            BadaoGangplankBarrels.BadaoActivate();
            BadaoGangplankCombo.BadaoActivate();
            BadaoGangplankHarass.BadaoActivate();
            BadaoGangplankLaneClear.BadaoActivate();
            BadaoGangplankJungleClear.BadaoActivate();
            BadaoGangplankAuto.BadaoActivate();
        }
    }
}
