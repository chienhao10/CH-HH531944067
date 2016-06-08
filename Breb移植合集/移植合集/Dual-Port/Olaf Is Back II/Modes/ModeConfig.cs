using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using OlafxQx.Common;
using OlafxQx.Evade;
using Color = SharpDX.Color;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace OlafxQx.Modes
{
    internal class ModeConfig
    {
        public static Menu MenuConfig { get; private set; }
        public static Menu MenuKeys { get; private set; }
        public static Menu MenuHarass { get; private set; }
        public static Menu MenuFarm { get; private set; }
        public static Menu MenuFlee { get; private set; }
        public static Menu MenuMisc { get; private set; }
        public static Menu MenuTools { get; private set; }
        // to-do: add ganker mode combo mode + use Q with E Combo
        public static void Init()
        {
            MenuConfig = MainMenu.AddMenu(":: Olaf is Back", "Olaf");

            Modes.ModeSettings.Init(MenuConfig);
            Common.CommonAutoLevel.Init(MenuConfig);
            Common.CommonAutoBush.Init(MenuConfig);
            Common.CommonManaManager.Init(MenuConfig);
            ModeUlti.Init(MenuConfig);
            Common.CommonHelper.Init();
            MenuKeys = MenuConfig.AddSubMenu("Keys", "Keys");
            {
                MenuKeys.Add("Key.HarassToggle", new KeyBind("Harass (Toggle)", false, KeyBind.BindTypes.PressToggle, 'T'));
            }
            Modes.ModeCombo.Init();
            Modes.ModeHarass.Init();
            MenuFarm = MenuConfig.AddSubMenu("Farm", "Farm");
            {
                MenuFarm.Add("Farm.Enable", new KeyBind(":: Lane / Jungle Clear Active!", false, KeyBind.BindTypes.PressToggle, 'J'));
                MenuFarm.Add("Farm.MinMana.Enable", new KeyBind("Min. Mana Control!", false, KeyBind.BindTypes.PressToggle, 'M'));
                Modes.ModeLane.Init(MenuConfig);
                Modes.ModeJungle.Init(MenuConfig);
            }
            Modes.ModeDraw.Init();
            Modes.ModePerma.Init();
        }
    }
}
