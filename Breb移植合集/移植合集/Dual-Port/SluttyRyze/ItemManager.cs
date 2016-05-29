using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp;
using LeagueSharp.Common;
using ItemData = LeagueSharp.Common.Data.ItemData;

namespace Slutty_ryze
{
    class ItemManager
    {
        #region Variable Declaration
        private static Items.Item _tearoftheGoddess = new Items.Item(3070, 0);
        private static Items.Item _tearoftheGoddesss = new Items.Item(3072, 0);
        private static Items.Item _tearoftheGoddessCrystalScar = new Items.Item(3073, 0);
        private static Items.Item _archangelsStaff = new Items.Item(3003, 0);
        private static Items.Item _archangelsStaffCrystalScar = new Items.Item(3007, 0);
        private static int _pMuramana = 3042;
        private static Items.Item _healthPotion = new Items.Item(2003, 0);
        private static Items.Item _crystallineFlask = new Items.Item(2041, 0);
        private static Items.Item _manaPotion = new Items.Item(2004);
        private static Items.Item _biscuitofRejuvenation = new Items.Item(2010, 0);
        private static Items.Item _seraphsEmbrace = new Items.Item(3040, 0);
        private static Items.Item _manamune = new Items.Item(3004, 0);
        private static Items.Item _manamuneCrystalScar = new Items.Item(3008, 0);
        #endregion
        #region Public Properties
        // public static int Muramana() => pMuramana;
        public static int Muramana
        {
            get {return _pMuramana;}   
        }
        #endregion
        #region Public Functions
        public static void Item()
        {
            var staff = getCheckBoxItem(MenuManager.itemMenu, "staff");
            var staffhp = getSliderItem(MenuManager.itemMenu, "staffhp");

            if (!staff || !Items.HasItem(ItemData.Seraphs_Embrace.Id) || !(GlobalManager.GetHero.HealthPercent <= staffhp)) return;

            Items.UseItem(ItemData.Seraphs_Embrace.Id);
        }
        public static void Potion()
        {
            var autoPotion = getCheckBoxItem(MenuManager.hpMenu, "autoPO");
            var hPotion = getCheckBoxItem(MenuManager.hpMenu, "HP");
            var mPotion = getCheckBoxItem(MenuManager.hpMenu, "MANA");
            var bPotion = getCheckBoxItem(MenuManager.hpMenu, "Biscuit");
            var fPotion = getCheckBoxItem(MenuManager.hpMenu, "flask");
            var pSlider = getSliderItem(MenuManager.hpMenu, "HPSlider");
            var mSlider = getSliderItem(MenuManager.hpMenu, "MANASlider");
            var bSlider = getSliderItem(MenuManager.hpMenu, "bSlider");
            var fSlider = getSliderItem(MenuManager.hpMenu, "fSlider");

            if (GlobalManager.GetHero.LSIsRecalling() || GlobalManager.GetHero.InFountain()) return;
            if (!autoPotion) return;

            if (hPotion
                && GlobalManager.GetHero.HealthPercent <= pSlider
                && GlobalManager.GetHero.LSCountEnemiesInRange(1000) >= 0
                && _healthPotion.IsReady()
                && !GlobalManager.GetHero.HasBuff("FlaskOfCrystalWater")
                && !GlobalManager.GetHero.HasBuff("ItemCrystalFlask")
                && !GlobalManager.GetHero.HasBuff("RegenerationPotion"))
                _healthPotion.Cast();

            if (mPotion
                && GlobalManager.GetHero.ManaPercent <= mSlider
                && GlobalManager.GetHero.LSCountEnemiesInRange(1000) >= 0
                && _manaPotion.IsReady()
                && !GlobalManager.GetHero.HasBuff("RegenerationPotion")
                && !GlobalManager.GetHero.HasBuff("FlaskOfCrystalWater"))
                _manaPotion.Cast();

            if (bPotion
                && GlobalManager.GetHero.HealthPercent <= bSlider
                && GlobalManager.GetHero.LSCountEnemiesInRange(1000) >= 0
                && _biscuitofRejuvenation.IsReady()
                && !GlobalManager.GetHero.HasBuff("ItemMiniRegenPotion"))
                _biscuitofRejuvenation.Cast();

            if (fPotion
                && GlobalManager.GetHero.HealthPercent <= fSlider
                && GlobalManager.GetHero.LSCountEnemiesInRange(1000) >= 0
                && _crystallineFlask.IsReady()
                && !GlobalManager.GetHero.HasBuff("ItemMiniRegenPotion")
                && !GlobalManager.GetHero.HasBuff("ItemCrystalFlask")
                && !GlobalManager.GetHero.HasBuff("RegenerationPotion")
                && !GlobalManager.GetHero.HasBuff("FlaskOfCrystalWater"))
                _crystallineFlask.Cast();
        }

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

        public static void TearStack()
        {
            var minions = MinionManager.GetMinions(
                GlobalManager.GetHero.ServerPosition, Champion.Q.Range, MinionTypes.All, MinionTeam.Enemy,
                MinionOrderTypes.MaxHealth);

            if (getCheckBoxItem(MenuManager.itemMenu, "tearoptions")
                && !GlobalManager.GetHero.InFountain())
                return;

            if (GlobalManager.GetHero.LSIsRecalling()
                || minions.Count >= 1)
                return;

            var mtears = getSliderItem(MenuManager.itemMenu, "tearSM");

            if (GlobalManager.GetPassiveBuff == 4)
                return;


            if (!Champion.Q.IsReady() ||
                (!_tearoftheGoddess.IsOwned(GlobalManager.GetHero) && !_tearoftheGoddessCrystalScar.IsOwned(GlobalManager.GetHero) &&
                 !_archangelsStaff.IsOwned(GlobalManager.GetHero) && !_archangelsStaffCrystalScar.IsOwned(GlobalManager.GetHero) &&
                 !_manamune.IsOwned(GlobalManager.GetHero) && !_manamuneCrystalScar.IsOwned(GlobalManager.GetHero)) || !(GlobalManager.GetHero.ManaPercent >= mtears))
                return;

            if (!Game.CursorPos.IsZero)
                Champion.Q.Cast(Game.CursorPos);
            else
                Champion.Q.Cast();
        }
        #endregion
    }
}
