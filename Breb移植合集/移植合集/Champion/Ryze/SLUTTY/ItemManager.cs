using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using ItemData = LeagueSharp.Common.Data.ItemData;

namespace Slutty_ryze
{
    internal class ItemManager
    {
        public static Menu
            _config = MenuManager._config,
            humanizerMenu = MenuManager.humanizerMenu,
            combo1Menu = MenuManager.combo1Menu,
            mixedMenu = MenuManager.mixedMenu,
            laneMenu = MenuManager.laneMenu,
            jungleMenu = MenuManager.jungleMenu,
            lastMenu = MenuManager.lastMenu,
            passiveMenu = MenuManager.passiveMenu,
            itemMenu = MenuManager.itemMenu,
            eventMenu = MenuManager.eventMenu,
            ksMenu = MenuManager.ksMenu,
            chase = MenuManager.chase;

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        #region Variable Declaration

        private static readonly Items.Item _tearoftheGoddess = new Items.Item(3070);
        private static readonly Items.Item _tearoftheGoddessCrystalScar = new Items.Item(3073);
        private static readonly Items.Item _archangelsStaff = new Items.Item(3003);
        private static readonly Items.Item _archangelsStaffCrystalScar = new Items.Item(3007);
        private static readonly Items.Item _manamune = new Items.Item(3004);
        private static readonly Items.Item _manamuneCrystalScar = new Items.Item(3008);

        #endregion

        #region Public Properties

        #endregion

        #region Public Functions

        public static void Item()
        {
            var staff = getCheckBoxItem(itemMenu, "staff");
            var staffhp = getSliderItem(itemMenu, "staffhp");

            if (!staff || !Items.HasItem(ItemData.Seraphs_Embrace.Id) ||
                !(GlobalManager.GetHero.HealthPercent <= staffhp)) return;

            Items.UseItem(ItemData.Seraphs_Embrace.Id);
        }

        public static void TearStack()
        {
            var minions = MinionManager.GetMinions(
                GlobalManager.GetHero.ServerPosition, Champion.Q.Range, MinionTypes.All, MinionTeam.Enemy,
                MinionOrderTypes.MaxHealth);

            if (getCheckBoxItem(itemMenu, "tearoptions")
                && !GlobalManager.GetHero.InFountain())
                return;

            if (GlobalManager.GetHero.IsRecalling()
                || minions.Count >= 1)
                return;

            var mtears = getSliderItem(itemMenu, "tearSM");

            if (GlobalManager.GetPassiveBuff == 4)
                return;


            if (!Champion.Q.IsReady() ||
                (!_tearoftheGoddess.IsOwned(GlobalManager.GetHero) &&
                 !_tearoftheGoddessCrystalScar.IsOwned(GlobalManager.GetHero) &&
                 !_archangelsStaff.IsOwned(GlobalManager.GetHero) &&
                 !_archangelsStaffCrystalScar.IsOwned(GlobalManager.GetHero) &&
                 !_manamune.IsOwned(GlobalManager.GetHero) && !_manamuneCrystalScar.IsOwned(GlobalManager.GetHero)) ||
                !(GlobalManager.GetHero.ManaPercent >= mtears))
                return;

            if (!Game.CursorPos.IsZero)
                Champion.Q.Cast(Game.CursorPos);
            else
                Champion.Q.Cast();
        }

        #endregion
    }
}