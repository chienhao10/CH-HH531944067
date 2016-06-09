using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leblanc.Common;
using LeagueSharp;
using LeagueSharp.Common;
using Color = SharpDX.Color;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Leblanc.Modes
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
        public static void Init()
        {
            MenuConfig = MainMenu.AddMenu(":: Leblanc II ::", "Leblanc");
            Modes.ModeSettings.Init(MenuConfig);
            Common.CommonGeometry.Init();
            Common.CommonAutoLevel.Init(MenuConfig);
            Common.CommonAutoBush.Init(MenuConfig);
            Common.CommonHelper.Init();
            MenuKeys = MenuConfig.AddSubMenu("Keys", "Keys");
            {
                MenuKeys.Add("Key.ChangeCombo", new KeyBind("Change Combo!", false, KeyBind.BindTypes.HoldActive, 'J'));
                MenuKeys.Add("Key.Harass1", new KeyBind("Harass Toggle!", false, KeyBind.BindTypes.PressToggle, 'T'));
                MenuKeys.Add("Key.DoubleChain", new KeyBind("Double Chain!", false, KeyBind.BindTypes.HoldActive, 'G'));
            }
            Modes.ModeCombo.Init();
            Modes.ModeChain.Init();
            Modes.ModeHarass.Init();
            MenuFarm = MenuConfig.AddSubMenu("Farm", "Farm");
            {
                Modes.ModeLane.Init(MenuConfig);
                Modes.ModeJungle.Init(MenuConfig);
                MenuFarm.Add("Farm.Enable", new KeyBind(":: Lane / Jungle Clear Active!", false, KeyBind.BindTypes.PressToggle, 'J'));
                MenuFarm.Add("Farm.MinMana.Enable", new KeyBind("Min. Mana Control!", false, KeyBind.BindTypes.PressToggle, 'M'));
            }
            Modes.ModeFlee.Init(MenuConfig);
            new ModeDraw().Init();
            Modes.ModePerma.Init();
            Champion.PlayerObjects.Init();
        }
    }
}
