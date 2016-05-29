using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irelia.Common;
using LeagueSharp;
using LeagueSharp.Common;
using Irelia.Evade;
using Color = SharpDX.Color;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Irelia.Modes
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
            MenuConfig = MainMenu.AddMenu(":: Irelia is Back", "Irelia");

            //MenuTools = new Menu("Tools", "Tools");

            Modes.ModeSettings.Init(MenuConfig);
            Common.CommonAutoLevel.Init(MenuConfig);
            Common.CommonAutoBush.Init(MenuConfig);

            //EvadeMain.Init();
            Common.CommonHelper.Init();

            Modes.ModeCombo.Init();

            MenuFarm = MenuConfig.AddSubMenu("Farm", "Farm");
            {
                Modes.ModeLane.Init(MenuConfig);
                Modes.ModeJungle.Init(MenuConfig);

                MenuFarm.Add("Farm.Enable", new KeyBind(":: Lane / Jungle Clear Active!", true, KeyBind.BindTypes.PressToggle, 'J'));
                MenuFarm.Add("Farm.MinMana.Enable", new KeyBind("Min. Mana Control Active!", true, KeyBind.BindTypes.PressToggle, 'M'));
            }

            Modes.ModeFlee.Init(MenuConfig);

            new ModeDraw().Init();
            Modes.ModePerma.Init();
        }
    }
}
