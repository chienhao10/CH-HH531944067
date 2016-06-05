using System.Drawing;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Slutty_ryze
{
    internal class MenuManager
    {
        #region Variable Declaration
        public const string Menuname = "Slutty Ryze";

        public static Menu _config;
        public static Menu passiveMenu, itemMenu, hpMenu, eventMenu, ksMenu, chase, lastMenu, jungleMenu, laneMenu, mixedMenu, combo1Menu, drawMenu, humanizerMenu;
        #endregion
        #region Public Functions
        public static void GetMenu()
        {
            _config = MainMenu.AddMenu(Menuname, Menuname);

            HumanizerMenu();
            DrawingMenu();
            ComboMenu();
            MixedMenu();
            FarmMenu();
            MiscMenu();
            _config.Add("test", new KeyBind("Level 3-5 Oriented Combo", false, KeyBind.BindTypes.HoldActive, 'Z'));
        }
        #endregion
        #region Private Functions

        private static void HumanizerMenu()
        {
            humanizerMenu = _config.AddSubMenu("人性化", "Humanizer");

            humanizerMenu.Add("minDelay", new Slider("最高动作延迟 (ms)", 0, 0, 200));
            humanizerMenu.Add("maxDelay", new Slider("最低动作延迟 (ms)", 0, 0, 250));
            humanizerMenu.Add("minCreepHPOffset", new Slider("最低小兵血量进行尾兵伤害 >= HP+(%)", 5, 0, 25));
            humanizerMenu.Add("maxCreepHPOffset", new Slider("最高小兵血量进行尾兵伤害 >= HP+(%)", 15, 0, 25));
            humanizerMenu.Add("doHuman", new CheckBox("人性化", false));
        }

        private static void DrawingMenu()
        {
            drawMenu = _config.AddSubMenu("线圈", "Drawings");
            drawMenu.Add("drawoptions", new ComboBox("线圈模式", 0, "正常", "色盲"));
            drawMenu.Add("Draw", new CheckBox("显示线圈"));
            drawMenu.Add("qDraw", new CheckBox("显示 Q"));
            drawMenu.Add("eDraw", new CheckBox("显示 E"));
            drawMenu.Add("wDraw", new CheckBox("显示 W"));
            drawMenu.Add("stackDraw", new CheckBox("叠加层数"));
            drawMenu.Add("notdraw", new CheckBox("显示浮动文字"));
            drawMenu.Add("keyBindDisplay", new CheckBox("显示按键"));  
        }

        private static void ComboMenu()
        {
            combo1Menu = _config.AddSubMenu("连招", "combospells");
            {
                combo1Menu.Add("combooptions", new ComboBox("连招模式", 0, "高级连招", "新连招"));
                combo1Menu.Add("useQ", new CheckBox("使用 Q"));
                combo1Menu.Add("useW", new CheckBox("使用 W"));
                combo1Menu.Add("useE", new CheckBox("使用 E"));
                combo1Menu.Add("useR", new CheckBox("使用 R"));
                combo1Menu.Add("useRww", new CheckBox("只在目标定身时使用R"));
                combo1Menu.Add("AAblock", new CheckBox("连招屏蔽普攻", false));
                combo1Menu.Add("minaarange", new Slider("屏蔽普攻当距离目标 >", 550, 100, 550));
            }
        }

        private static void MixedMenu()
        {
            mixedMenu = _config.AddSubMenu("混合", "mixedsettings");
            {
                mixedMenu.Add("mMin", new Slider("最低.蓝量使用技能", 40));
                mixedMenu.Add("UseQM", new CheckBox("使用 Q"));
                mixedMenu.Add("UseQMl", new CheckBox("使用 Q 尾兵"));
                mixedMenu.Add("UseEM", new CheckBox("使用 E", false));
                mixedMenu.Add("UseWM", new CheckBox("使用 W", false));
                mixedMenu.Add("UseQauto", new CheckBox("自动使用 Q", false));
            }
        }

        private static void FarmMenu()
        {
            laneMenu = _config.AddSubMenu("清线", "lanesettings");
            {
                laneMenu.Add("disablelane", new KeyBind("清线按键", false, KeyBind.BindTypes.PressToggle, 'T'));
                laneMenu.Add("useEPL", new Slider("最低. % 清线蓝量", 50));
                laneMenu.Add("passiveproc", new CheckBox("不使用技能如果会触发被动"));
                laneMenu.Add("useQlc", new CheckBox("使用 Q 尾兵"));
                laneMenu.Add("useWlc", new CheckBox("使用 W 尾兵", false));
                laneMenu.Add("useElc", new CheckBox("使用 E 尾兵", false));
                laneMenu.Add("useQ2L", new CheckBox("使用 Q 清线"));
                laneMenu.Add("useW2L", new CheckBox("使用 W 清线", false));
                laneMenu.Add("useE2L", new CheckBox("使用 E 清线", false));
                laneMenu.Add("useRl", new CheckBox("使用 R 清线", false));
                laneMenu.Add("rMin", new Slider("最低. 小兵数量使用R", 3, 1, 20));
            }

            jungleMenu = _config.AddSubMenu("清野", "junglesettings");
            {
                jungleMenu.Add("useJM", new Slider("最低. % 清野蓝量", 50));
                jungleMenu.Add("useQj", new CheckBox("使用 Q"));
                jungleMenu.Add("useWj", new CheckBox("使用 W"));
                jungleMenu.Add("useEj", new CheckBox("使用 E"));
                jungleMenu.Add("useRj", new CheckBox("使用 R"));
            }

            lastMenu = _config.AddSubMenu("尾兵", "lastsettings");
            {
                lastMenu.Add("useQl2h", new CheckBox("使用 Q 尾兵"));
                lastMenu.Add("useWl2h", new CheckBox("使用 W 尾兵", false));
                lastMenu.Add("useEl2h", new CheckBox("使用 E 尾兵", false));
            }
        }

        private static void MiscMenu()
        {
            passiveMenu = _config.AddSubMenu("自动叠加被动", "passivesettings");
            {
                passiveMenu.Add("ManapSlider", new Slider("最低. % 蓝量", 30));
                passiveMenu.Add("autoPassive", new KeyBind("叠加被动", false, KeyBind.BindTypes.PressToggle, 'Z'));
                passiveMenu.Add("stackSlider", new Slider("保持被动层数", 3, 1, 4));
                passiveMenu.Add("autoPassiveTimer", new Slider("X秒 刷新被动 (s)", 5, 1, 10));
            }

            itemMenu = _config.AddSubMenu("物品", "itemsettings");
            {
                itemMenu.Add("tearS", new KeyBind("自动叠加女神", false, KeyBind.BindTypes.PressToggle, 'G'));
                itemMenu.Add("tearoptions", new CheckBox("只在泉水叠加", false));
                itemMenu.Add("tearSM", new Slider("最低 % 蓝量进行叠加", 95));
                itemMenu.Add("staff", new CheckBox("使用 炽天使"));
                itemMenu.Add("staffhp", new Slider("当血量低于", 30));
                itemMenu.Add("muramana", new CheckBox("使用 魔切"));
            }

            hpMenu = _config.AddSubMenu("自动喝药", "hpsettings");
            {
                hpMenu.Add("autoPO", new CheckBox("开启喝药"));
                hpMenu.Add("HP", new CheckBox("自动血药"));
                hpMenu.Add("HPSlider", new Slider("最低. % 血量", 30));
                hpMenu.Add("MANA", new CheckBox("自动蓝药"));
                hpMenu.Add("MANASlider", new Slider("最低. % 蓝量", 30));
                hpMenu.Add("Biscuit", new CheckBox("自动饼干"));
                hpMenu.Add("bSlider", new Slider("最低. % 血量", 30));
                hpMenu.Add("flask", new CheckBox("自动魔瓶"));
                hpMenu.Add("fSlider", new Slider("最低. % 血量", 30));
            }

            eventMenu = _config.AddSubMenu("事件", "eventssettings");
            {
                eventMenu.Add("useW2I", new CheckBox("技能打断 W"));
                eventMenu.Add("useQW2D", new CheckBox("W/Q 突击单位"));
                eventMenu.Add("level", new CheckBox("自动加点"));
                eventMenu.Add("autow", new CheckBox("塔下自动W"));
            }

            ksMenu = _config.AddSubMenu("抢头", "kssettings");
            {
                ksMenu.Add("KS", new CheckBox("开启抢头"));
                ksMenu.Add("useQ2KS", new CheckBox("使用 Q"));
                ksMenu.Add("useW2KS", new CheckBox("使用 W"));
                ksMenu.Add("useE2KS", new CheckBox("使用 E"));
            }

            chase = _config.AddSubMenu("追击", "Chase Target");
            {
                chase.Add("chase", new KeyBind("追击按键", false, KeyBind.BindTypes.HoldActive, 'A'));
                chase.Add("usewchase", new CheckBox("使用 W"));
                chase.Add("chaser", new CheckBox("使用 [R]", false));
            }
        }
        #endregion
    }

}
