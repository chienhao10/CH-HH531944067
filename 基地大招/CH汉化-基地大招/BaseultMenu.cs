using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;

namespace Baseult
{
    class BaseultMenu
    {
        public static Menu Principal, Settings;

        public static void Load()
        {
            Principal = MainMenu.AddMenu("基地大招", "BaseUlt");
            Principal.AddLabel("感谢 iRaxe");

            Settings = Principal.AddSubMenu("设置", "Settings");
            Settings.Add("UseBaseUlt", new CheckBox("使用基地大招 ?"));
            Settings.AddSeparator(2);
            Settings.Add("ShowEnemies", new CheckBox("显示敌人回城计时"));
            Settings.Add("ShowAllies", new CheckBox("显示友军回城计时"));
            Settings.AddSeparator(1);
            Settings.AddSeparator(2);
            Settings.AddLabel("对谁使用基地大招:");
            foreach(var x in EntityManager.Heroes.Enemies)
            {
                Settings.Add("对/" + x.ChampionName, new CheckBox(x.ChampionName));
            }
        }

        public static bool CheckBox(Menu m, string s)
        {
            return m[s].Cast<CheckBox>().CurrentValue;
        }

        public static int Slider(Menu m, string s)
        {
            return m[s].Cast<Slider>().CurrentValue;
        }

    }
}
