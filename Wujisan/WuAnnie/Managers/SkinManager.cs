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
            menu = MenuManager.AddSubMenu("换肤功能");
            {
                menu.NewCheckbox("enable", "开启换肤");
                menu.NewSlider("skinid", "皮肤ID", 0, 0, skins, true);
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
