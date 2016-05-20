using System;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace RengarPro_Revamped
{
    internal class Menu
    {
        public static EloBuddy.SDK.Menu.Menu RengarMenu, ComboM, LaneM, JungleM, DrawM, MiscM;

        public static void Init()
        {
            try
            {
                //Main Menu
                RengarMenu = MainMenu.AddMenu("RengarPro Revamped", "RengarProMenu");
                RengarMenu.AddGroupLabel("狮子狗！Meow~");
                RengarMenu.AddLabel("Its loaded. Have Fun ! :)");
                //Combo Menu
                ComboM = RengarMenu.AddSubMenu("连招");
                ComboM.AddGroupLabel("连招菜单");
                ComboM.AddLabel("1- 秒杀 | 2- 定身 | 3- AP 狮子狗");
                ComboM.Add("combo.mode", new Slider("连招模式", 1, 1, 3));
                var switcher = ComboM.Add("switch",
                    new KeyBind("连招切换按键", false, KeyBind.BindTypes.HoldActive, 'T'));
                switcher.OnValueChange += delegate(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args1)
                {
                    if (args1.NewValue)
                    {
                        var cast = ComboM["combo.mode"].Cast<Slider>();
                        if (cast.CurrentValue == cast.MaxValue)
                        {
                            cast.CurrentValue = 0;
                        }
                        else
                        {
                            cast.CurrentValue++;
                        }
                    }
                };

                ComboM.Add("combo.useEoutofQ", new CheckBox("超出Q 范围使用 E"));
                ComboM.AddSeparator();
                ComboM.AddLabel("以下为推荐设置");
                ComboM.AddLabel("非脚本开发者请勿调整");
                ComboM.AddSeparator();
                ComboM.AddLabel("BetaQ => 激活大招前使用Q");
                ComboM.Add("betaq.active", new CheckBox("BetaQ 开启"));
                ComboM.Add("betaq.range", new Slider("Beta Q 范围", 875, 600, 1000));

                //Lane Clear Menu
                LaneM = RengarMenu.AddSubMenu("清线");
                LaneM.AddGroupLabel("清线设置");
                LaneM.Add("lane.useQ", new CheckBox("使用 Q"));
                LaneM.Add("lane.useW", new CheckBox("使用 W"));
                LaneM.Add("lane.useE", new CheckBox("使用 E"));
                LaneM.Add("lane.save", new CheckBox("保留叠加层数", false));

                //Jungle Clear Menu
                JungleM = RengarMenu.AddSubMenu("清野");
                JungleM.AddGroupLabel("清野设置");
                JungleM.Add("jungle.useQ", new CheckBox("使用 Q"));
                JungleM.Add("jungle.useW", new CheckBox("使用 W"));
                JungleM.Add("jungle.useE", new CheckBox("使用 E"));
                JungleM.Add("jungle.save", new CheckBox("保留叠加层数", false));

                //Draw  Menu
                DrawM = RengarMenu.AddSubMenu("线圈");
                DrawM.AddGroupLabel("线圈设置");
                DrawM.Add("draw.mode", new CheckBox("显示连招模式"));
                DrawM.Add("draw.selectedenemy", new CheckBox("显示所选敌人"));

                //Misc Menu
                MiscM = RengarMenu.AddSubMenu("杂项");
                MiscM.AddGroupLabel("杂项菜单");
                MiscM.Add("misc.autoyoumuu", new CheckBox("R时自动幽梦"));
                /*MiscM.Add("misc.magnet", new CheckBox("Enable Magnet"));
            MiscM.Add("magnet.range", new Slider("Magnet Range", 225, 100, 500));*/
                MiscM.Add("misc.smite", new CheckBox("连招使用惩戒"));
                MiscM.Add("misc.autohp", new CheckBox("自动激活回血"));
                MiscM.Add("misc.hp.value", new Slider("血量%", 30, 1));
                MiscM.AddLabel("1- HeadHunter 2- NightHunter 3- SSW");
                MiscM.Add("skin.active", new CheckBox("开启换肤"));
                MiscM.Add("skin.value", new Slider("皮肤", 1, 1, 3));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}