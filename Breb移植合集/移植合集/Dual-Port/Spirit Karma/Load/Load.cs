#region

using EloBuddy;
using LeagueSharp.SDK;
using Spirit_Karma.Core;
using Spirit_Karma.Draw;
using Spirit_Karma.Event;
using Spirit_Karma.Menus;

#endregion

namespace Spirit_Karma.Load
{
    internal class Load
    {
        public static void LoadAssembly()
        {
            Spells.Load();
            MenuConfig.Load();

            Drawing.OnDraw += DrawMantra.SelectedMantra;
            Drawing.OnEndScene += DrawDmg.OnDrawEnemy;

            Game.OnUpdate += Mode.OnUpdate;
            Game.OnUpdate += Trinkets.Update;

         
            Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Spirit Karma</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> Loaded Sucessfully</font></b>");
        }
    }
}
