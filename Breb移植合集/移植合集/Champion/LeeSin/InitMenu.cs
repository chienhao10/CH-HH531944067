using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace ElLeeSin
{
    public class InitMenu
    {
        #region Static Fields

        public static Menu Menu,
            comboMenu,
            harassMenu,
            waveclearMenu,
            kickMenu,
            insecMenu,
            wardjumpMenu,
            miscMenu,
            drawMenu;

        #endregion

        #region Public Methods and Operators

        public static void Initialize()
        {
            Menu = MainMenu.AddMenu("El李星", "LeeSin");

            comboMenu = Menu.AddSubMenu("连招", "Combo");
            comboMenu.Add("ElLeeSin.Combo.Q", new CheckBox("使用 Q"));
            comboMenu.Add("ElLeeSin.Combo.Q2", new CheckBox("使用 Q2"));
            comboMenu.Add("ElLeeSin.Combo.W2", new CheckBox("使用 W"));
            comboMenu.Add("ElLeeSin.Combo.E", new CheckBox("使用 E"));
            comboMenu.Add("ElLeeSin.Combo.R", new CheckBox("使用 R"));
            comboMenu.Add("ElLeeSin.Combo.PassiveStacks", new Slider("最少被动叠加", 1, 1, 2));
            comboMenu.Add("ElLeeSin.Combo.W", new CheckBox("连招跳眼", false));
            comboMenu.Add("ElLeeSin.Combo.Mode.WW", new CheckBox("超出普攻距离", false));
            comboMenu.Add("ElLeeSin.Combo.KS.R", new CheckBox("抢头 R"));
            comboMenu.Add("starCombo", new KeyBind("全明星连招", false, KeyBind.BindTypes.HoldActive, 'T'));
            comboMenu.Add("ElLeeSin.Combo.AAStacks", new CheckBox("等待被动", false));

            harassMenu = Menu.AddSubMenu("骚扰", "Harass");
            harassMenu.Add("ElLeeSin.Harass.Q1", new CheckBox("使用 Q"));
            harassMenu.Add("ElLeeSin.Harass.Wardjump", new CheckBox("使用 W"));
            harassMenu.Add("ElLeeSin.Harass.E1", new CheckBox("使用 E", false));
            harassMenu.Add("ElLeeSin.Harass.PassiveStacks", new Slider("最少被动叠加", 1, 1, 2));

            kickMenu = Menu.AddSubMenu("踢 (R)", "Kick");
            kickMenu.Add("ElLeeSin.Combo.New", new CheckBox("踢复数目标:"));
            kickMenu.Add("ElLeeSin.Combo.R.Count", new Slider("R 踢中目标数量", 3, 2, 4));

            waveclearMenu = Menu.AddSubMenu("推线", "Clear");
            waveclearMenu.AddGroupLabel("清线");
            waveclearMenu.Add("ElLeeSin.Lane.Q", new CheckBox("使用 Q"));
            waveclearMenu.Add("ElLeeSin.Lane.E", new CheckBox("使用 E"));
            waveclearMenu.AddSeparator();
            waveclearMenu.AddGroupLabel("清野");

            waveclearMenu.Add("ElLeeSin.Jungle.Q", new CheckBox("使用 Q"));
            waveclearMenu.Add("ElLeeSin.Jungle.W", new CheckBox("使用 W"));
            waveclearMenu.Add("ElLeeSin.Jungle.E", new CheckBox("使用 E"));

            insecMenu = Menu.AddSubMenu("回旋踢", "Insec");
            insecMenu.Add("InsecEnabled", new KeyBind("回旋踢按键:", false, KeyBind.BindTypes.HoldActive, 'Y'));
            insecMenu.Add("insecMode", new CheckBox("左键点击选择回旋踢目标"));
            insecMenu.Add("insecOrbwalk", new CheckBox("走砍"));
            insecMenu.Add("ElLeeSin.Flash.Insec", new CheckBox("无眼时使用闪现", false));
            insecMenu.Add("waitForQBuff", new CheckBox("等待 Q", false));
            insecMenu.Add("checkOthers1", new CheckBox("检查回旋踢其他可用物体"));
            insecMenu.Add("clickInsec", new CheckBox("点击回旋踢"));
            insecMenu.Add("bonusRangeA", new Slider("友军额外距离", 0, 0, 1000));
            insecMenu.Add("bonusRangeT", new Slider("T防御塔额外距离", 0, 0, 1000));
            insecMenu.AddGroupLabel("回旋踢模式:");
            insecMenu.Add("ElLeeSin.Insec.Ally", new CheckBox("踢至友军"));
            insecMenu.Add("ElLeeSin.Insec.Tower", new CheckBox("踢至塔", false));
            insecMenu.Add("ElLeeSin.Insec.Original.Pos", new CheckBox("踢至原来的位置"));
            insecMenu.AddSeparator();
            insecMenu.Add("ElLeeSin.Insec.UseInstaFlash",
                new KeyBind("闪现 + R", false, KeyBind.BindTypes.HoldActive, 'G'));

            wardjumpMenu = Menu.AddSubMenu("跳眼/逃跑", "Wardjump");
            wardjumpMenu.Add("ElLeeSin.Escape", new KeyBind("逃跑按键", false, KeyBind.BindTypes.HoldActive, 'A'));
            wardjumpMenu.Add("escapeMode", new CheckBox("开启野区逃跑"));
            wardjumpMenu.Add("ElLeeSin.Wardjump", new KeyBind("跳眼按键", false, KeyBind.BindTypes.HoldActive, 'G'));
            wardjumpMenu.Add("ElLeeSin.Wardjump.MaxRange", new CheckBox("跳至最远眼位", false));
            wardjumpMenu.Add("ElLeeSin.Wardjump.Mouse", new CheckBox("跳至鼠标位置"));
            wardjumpMenu.Add("ElLeeSin.Wardjump.Minions", new CheckBox("跳至小兵"));
            wardjumpMenu.Add("ElLeeSin.Wardjump.Champions", new CheckBox("跳至其他英雄"));

            drawMenu = Menu.AddSubMenu("线圈", "Drawings");
            drawMenu.Add("DrawEnabled", new CheckBox("开启线圈"));
            drawMenu.Add("Draw.Insec.Lines", new CheckBox("显示回旋踢线位", false));
            drawMenu.Add("ElLeeSin.Draw.Insec.Text", new CheckBox("显示回旋踢文字", false));
            drawMenu.Add("drawOutLineST", new CheckBox("显示整体线", false));
            drawMenu.Add("ElLeeSin.Draw.Insec", new CheckBox("显示回旋踢", false));
            drawMenu.Add("ElLeeSin.Draw.WJDraw", new CheckBox("显示跳眼", false));
            drawMenu.Add("ElLeeSin.Draw.Q", new CheckBox("显示 Q", false));
            drawMenu.Add("ElLeeSin.Draw.W", new CheckBox("显示 W", false));
            drawMenu.Add("ElLeeSin.Draw.E", new CheckBox("显示 E", false));
            drawMenu.Add("ElLeeSin.Draw.R", new CheckBox("显示 R", false));
            drawMenu.Add("ElLeeSin.Draw.Escape", new CheckBox("显示可逃脱位置", false));
            drawMenu.Add("ElLeeSin.Draw.Q.Width", new CheckBox("显示逃跑 Q 宽度", false));

            miscMenu = Menu.AddSubMenu("杂项", "Misc");
            miscMenu.Add("ElLeeSin.Ignite.KS", new CheckBox("使用点燃"));
            miscMenu.Add("ElLeeSin.Smite.KS", new CheckBox("使用惩戒"));
            miscMenu.Add("ElLeeSin.Smite.Q", new CheckBox("惩戒 Q!", false)); //qSmite
        }

        #endregion
    }
}