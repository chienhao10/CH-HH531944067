#region

using System;
using System.Linq;
using EloBuddy;
using LeagueSharp.SDK;
using SharpDX;
using Spirit_Karma.Core;
using Spirit_Karma.Menus;

#endregion

namespace Spirit_Karma.Draw 
{
    internal class DrawDmg : Core.Core
    {
        private static readonly HpBarDraw DrawHpBar = new HpBarDraw();

        public static void OnDrawEnemy(EventArgs args)
        {
            if (Player.IsDead || !MenuConfig.getCheckBoxItem(MenuConfig.drawMenu, "UseDrawings") || !MenuConfig.getCheckBoxItem(MenuConfig.drawMenu, "Dind"))
            {
                return;
            }
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(x => x.LSIsValidTarget() && !x.IsZombie))
            {
                var EasyKill = Spells.Q.IsReady() && Dmg.IsLethal(enemy)
                      ? new ColorBGRA(0, 255, 0, 120)
                      : new ColorBGRA(255, 255, 0, 120);
                DrawHpBar.unit = enemy;
                DrawHpBar.drawDmg(Dmg.ComboDmg(enemy), EasyKill);
            }
        }
    }
}
