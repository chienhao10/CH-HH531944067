using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy;

namespace BadaoKingdom.BadaoChampion.BadaoPoppy
{
    public static class BadaoPoppy
    {
        public static void BadaoActivate()
        {
            BadaoPoppyConfig.BadaoActivate();
            BadaoPoppyCombo.BadaoActiavate();
            BadaoPoppyHarass.BadaoActiavate();
            BadaoPoppyJungleClear.BadaoActivate();
            BadaoPoppyAuto.BadaoActivate();
            BadaoPoppyAssasinate.BadaoActivate();
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
        }
    }
}
