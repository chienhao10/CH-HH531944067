namespace Kalista_FlyHack
{
    using System;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    internal class Program
    {
        private static Menu flymenu;

        public static int LastAATick;

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.ChampionName != "Kalista")
            {
                return;
            }

            flymenu = MainMenu.AddMenu("滑板鞋漏洞", "KalistaFlyHack");
            flymenu.AddLabel("CH汉化卡莉斯塔漏洞脚本.");
            flymenu.AddLabel("只会在连招模式使用，详细设置请看汉化贴，否则无效.");
            flymenu.Add("Fly", new CheckBox("使用漏洞", false));
            flymenu.Add("Flyspeed", new Slider("飞行速度 (修改测试至有效)", 250, 0, 500));
            flymenu.AddSeparator();
            flymenu.AddGroupLabel("必读!");
            flymenu.AddLabel("使用这个可能会造成封号.");
            flymenu.AddLabel("后果自负.");

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (flymenu["Fly"].Cast<CheckBox>().CurrentValue && Player.Instance.AttackSpeedMod >= 2.5)
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    var target = TargetSelector.GetTarget(
                        ObjectManager.Player.GetAutoAttackRange(),
                        DamageType.Physical);
                    if (target.IsValidTarget(ObjectManager.Player.GetAutoAttackRange()))
                    {
                        if (Game.Time * (1000 - flymenu["Flyspeed"].Cast<Slider>().CurrentValue) - Game.Ping
                            >= LastAATick + 1)
                        {
                            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                        }
                        if (Game.Time * (1000 - flymenu["Flyspeed"].Cast<Slider>().CurrentValue) - Game.Ping
                            > LastAATick + ObjectManager.Player.AttackDelay * 1000 - 250)
                        {
                            Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                        }
                    }
                    else
                    {
                        Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    }
                }
            }
        }
    }
}