using EloBuddy;
using EloBuddy.SDK.Menu;
using OneForWeek.Draw.Notifications;
using OneForWeek.Model.Notification;
using SharpDX;

namespace OneForWeek.Plugin
{
    abstract class PluginModel
    {
        #region Global Variables

        /*
         Config
         */

        public static readonly string GVersion = "1.4.1";
        public static readonly string GCharname = _Player.ChampionName;

        /*
         Menus
         */

        public static Menu Menu,
            ComboMenu,
            LaneClearMenu,
            JungleClearMenu,
            LastHitMenu,
            HarassMenu,
            MiscMenu,
            DrawMenu;


        /*
         Misc
         */

        public static AIHeroClient Target;

        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }

        #endregion
    }
}
