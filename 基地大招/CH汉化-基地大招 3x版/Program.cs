using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace BaseUltPlusPlus
{
    public class Program
    {
        public static Menu BaseUltMenu { get; set; }

        public static void Main(string[] args)
        {
            // Wait till the name has fully loaded
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            //Menu
            BaseUltMenu = MainMenu.AddMenu("CH汉化基地大招", "BUP");
            BaseUltMenu.Add("baseult", new CheckBox("使用基地大招"));
            BaseUltMenu.Add("showrecalls", new CheckBox("显示回城"));
            BaseUltMenu.Add("checkcollision", new CheckBox("检查体积碰撞"));
            BaseUltMenu.AddSeparator();
            BaseUltMenu.Add("timeLimit", new Slider("战争迷雾时间限制 (秒)", 0, 0, 120));
            BaseUltMenu.AddSeparator();
            BaseUltMenu.Add("nobaseult", new KeyBind("按下时不使用基地大招", false, KeyBind.BindTypes.HoldActive, 32));
            BaseUltMenu.AddSeparator();
            BaseUltMenu.AddGroupLabel("基地大招目标");
            foreach (var unit in EntityManager.Heroes.Enemies)
            {
                BaseUltMenu.Add("目标" + unit.ChampionName,
                    new CheckBox(string.Format("{0} ({1})", unit.ChampionName, unit.Name)));
            }

            BaseUltMenu.AddGroupLabel("BaseUlt-3x - By BestAkaliAfrica");
            BaseUltMenu.AddLabel("Based on Roach_ version of LunarBlue Addon");
            BaseUltMenu.AddLabel("FinnDev, MrOwl, Beaving, DanThePman, Gabe2901");
            BaseUltMenu.AddLabel("CH汉化");

            Chat.Print("<font color = \"#6B9FE3\">BaseUlt-3x</font><font color = \"#E3AF6B\"> by BestAkaliAfrica</font>. You like ? Buy me a coffee :p");
            // Initialize the Addon
            OfficialAddon.Initialize();

            // Listen to the two main events for the Addon
            Game.OnUpdate += args1 => OfficialAddon.Game_OnUpdate();
            Drawing.OnPreReset += args1 => OfficialAddon.Drawing_OnPreReset(args1);
            Drawing.OnPostReset += args1 => OfficialAddon.Drawing_OnPostReset(args1);
            Drawing.OnDraw += args1 => OfficialAddon.Drawing_OnDraw(args1);
            Teleport.OnTeleport += OfficialAddon.Teleport_OnTeleport;
        }
    }
}