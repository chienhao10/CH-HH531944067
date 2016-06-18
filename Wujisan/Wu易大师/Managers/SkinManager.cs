using System;
using EloBuddy;
using EloBuddy.SDK.Menu;

namespace WuAIO.Managers
{
    class SkinManager
    {
        public Menu menu { get; set; }

        readonly AIHeroClient Player = EloBuddy.Player.Instance;

        public SkinManager(int skins)
        {
            //Creating menu
            menu = MenuManager.AddSubMenu("Skin Manager");
            {
                menu.NewCheckbox("enable", "Enable");
                menu.NewSlider("skinid", "SkinID", 0, 0, skins, true);
            }

            Game.OnTick += Game_OnTick;
        }

        private void Game_OnTick(EventArgs args)
        {
            if (Player.SkinId != menu.Value("skinid") && menu.IsActive("enable"))
            {
                Player.SetSkinId(menu.Value("skinid"));
            }

            return;
        }
    }
}
