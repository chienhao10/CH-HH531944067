using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace NidaleeBuddyEvolution
{
    internal class NidaleeMenu
    {
        /// <summary>
        /// Stores Menus
        /// </summary>
        public static Menu DefaultMenu,
            ComboMenu,
            LastHitMenu,
            HarassMenu,
            LaneClearMenu,
            JungleClearMenu,
            KillStealMenu,
            JungleStealMenu,
            DrawingMenu,
            MiscMenu;

        /// <summary>
        /// Creates the Menu.
        /// </summary>
        public static void Create()
        {
            DefaultMenu = MainMenu.AddMenu("狂野女猎手", "NidaleeBuddy");
            DefaultMenu.AddGroupLabel("This addon is made by KarmaPanda and should not be redistributed in any way.");
            DefaultMenu.AddGroupLabel(
                "Any unauthorized redistribution without credits will result in severe consequences.");
            DefaultMenu.AddGroupLabel("Thank you for using this addon and have a fun time!");

            #region Combo

            ComboMenu = DefaultMenu.AddSubMenu("连招", "Combo");
            ComboMenu.AddGroupLabel("连招设置");
            ComboMenu.Add("useQH", new CheckBox("人类状态使用Q"));
            ComboMenu.Add("useWH", new CheckBox("人类状态使用W"));
            ComboMenu.Add("useQC", new CheckBox("豹子状态使用Q"));
            ComboMenu.Add("useWC", new CheckBox("豹子状态使用W"));
            ComboMenu.Add("useEC", new CheckBox("豹子状态使用E"));
            ComboMenu.Add("useR", new CheckBox("连招时使用R"));
            ComboMenu.AddLabel("预判设置 - 人类状态");
            ComboMenu.Add("predQH", new Slider("命中率为 x %时使用Q", 75));
            ComboMenu.Add("predWH", new Slider("命中率为 x %时使用W", 75));
            ComboMenu.AddLabel("预判设置 - 豹子状态");
            ComboMenu.Add("predWC", new Slider("命中率为 x %时使用W", 75));
            ComboMenu.Add("predEC", new Slider("命中率为 x %时使用E", 75));

            #endregion

            #region Last Hit

            LastHitMenu = DefaultMenu.AddSubMenu("尾兵", "Last Hit");
            LastHitMenu.AddGroupLabel("尾兵设置");
            LastHitMenu.Add("useQC", new CheckBox("豹子模式使用Q当无法杀死小兵时"));
            LastHitMenu.Add("useEC", new CheckBox("豹子模式使用E当无法杀死小兵时", false));
            LastHitMenu.Add("useR", new CheckBox("超出范围时使用R"));

            #endregion

            #region Harass

            HarassMenu = DefaultMenu.AddSubMenu("骚扰", "Harass");
            HarassMenu.AddGroupLabel("骚扰设置");
            HarassMenu.Add("useQH", new CheckBox("人类状态使用Q"));
            HarassMenu.Add("useR", new CheckBox("强制人类状态"));
            HarassMenu.AddLabel("预判设置 - 人类状态");
            HarassMenu.Add("predQH", new Slider("命中率为 x %时使用Q", 75));

            #endregion

            #region Kill Steal

            KillStealMenu = DefaultMenu.AddSubMenu("抢人头", "Kill Steal");
            KillStealMenu.AddGroupLabel("抢头设置");
            KillStealMenu.Add("useQH", new CheckBox("Q抢头"));
            KillStealMenu.Add("predQH", new Slider("命中率为 x %时使用Q", 75));
            KillStealMenu.Add("useIgnite", new CheckBox("使用点燃", false));

            #endregion

            #region Lane Clear

            LaneClearMenu = DefaultMenu.AddSubMenu("清线", "Lane Clear");
            LaneClearMenu.AddGroupLabel("清线设置");
            LaneClearMenu.Add("useQC", new CheckBox("豹子模式使用Q"));
            LaneClearMenu.Add("useWC", new CheckBox("豹子模式使用W"));
            LaneClearMenu.Add("useEC", new CheckBox("豹子模式使用E"));
            LaneClearMenu.Add("useR", new CheckBox("清线时使用R", false));
            LaneClearMenu.AddLabel("农兵设置 - 豹子模式");
            LaneClearMenu.Add("predWC", new Slider("如果能命中 X 小兵使用W", 1, 1, 7));
            LaneClearMenu.Add("predEC", new Slider("如果能命中 X 小兵使用E", 1, 1, 7));

            #endregion

            #region Jungle Clear

            JungleClearMenu = DefaultMenu.AddSubMenu("清野", "Jungle Clear");
            JungleClearMenu.AddGroupLabel("清野设置");
            JungleClearMenu.Add("useQH", new CheckBox("人类模式使用Q"));
            JungleClearMenu.Add("useQC", new CheckBox("豹子模式使用Q"));
            JungleClearMenu.Add("useWC", new CheckBox("豹子模式使用W"));
            JungleClearMenu.Add("useEC", new CheckBox("豹子模式使用E"));
            JungleClearMenu.Add("useR", new CheckBox("清野时使用R"));
            JungleClearMenu.AddLabel("预判设置");
            JungleClearMenu.Add("predQH", new Slider("人类状态命中率为 x %时使用Q", 75));
            JungleClearMenu.Add("predWC", new Slider("豹子状态命中率为 x %时使用W", 75));
            JungleClearMenu.Add("predEC", new Slider("豹子状态命中数量 X 使用E", 1, 1, 3));

            #endregion

            #region Jungle Steal

            JungleStealMenu = DefaultMenu.AddSubMenu("偷野", "Jungle Steal");
            JungleStealMenu.AddGroupLabel("偷野设置");
            JungleStealMenu.Add("useQH", new CheckBox("使用Q偷野"));
            JungleStealMenu.Add("predQH", new Slider("命中率为 x %时使用Q", 75));
            JungleStealMenu.Add("useSmite", new CheckBox("使用惩戒偷野"));
            JungleStealMenu.Add("toggleK", new KeyBind("惩戒开关", true, KeyBind.BindTypes.PressToggle, 'M'));
            JungleStealMenu.AddGroupLabel("野怪开关");
            switch (Game.MapId)
            {
                case GameMapId.SummonersRift:
                    JungleStealMenu.AddLabel("5V5史诗");
                    JungleStealMenu.Add("SRU_Baron", new CheckBox("男爵"));
                    JungleStealMenu.Add("SRU_Dragon", new CheckBox("小龙"));
                    JungleStealMenu.AddLabel("增益");
                    JungleStealMenu.Add("SRU_Blue", new CheckBox("蓝"));
                    JungleStealMenu.Add("SRU_Red", new CheckBox("红"));
                    JungleStealMenu.AddLabel("小怪");
                    JungleStealMenu.Add("SRU_Gromp", new CheckBox("青蛙", false));
                    JungleStealMenu.Add("SRU_Murkwolf", new CheckBox("狼", false));
                    JungleStealMenu.Add("SRU_Krug", new CheckBox("石头人", false));
                    JungleStealMenu.Add("SRU_Razorbeak", new CheckBox("鸟怪", false));
                    JungleStealMenu.Add("Sru_Crab", new CheckBox("河蟹", false));
                    break;
                case GameMapId.TwistedTreeline:
                    JungleStealMenu.AddLabel("3V3史诗");
                    JungleStealMenu.Add("TT_Spiderboss8.1", new CheckBox("蜘蛛怪"));
                    JungleStealMenu.AddLabel("Camps");
                    JungleStealMenu.Add("TT_NWraith1.1", new CheckBox("幽鬼"));
                    JungleStealMenu.Add("TT_NWraith4.1", new CheckBox("幽鬼"));
                    JungleStealMenu.Add("TT_NGolem2.1", new CheckBox("石头人"));
                    JungleStealMenu.Add("TT_NGolem5.1", new CheckBox("石头人"));
                    JungleStealMenu.Add("TT_NWolf3.1", new CheckBox("狼"));
                    JungleStealMenu.Add("TT_NWolf6.1", new CheckBox("狼"));
                    break;
            }

            #endregion

            #region Drawing

            DrawingMenu = DefaultMenu.AddSubMenu("线圈", "Drawing");
            DrawingMenu.AddGroupLabel("线圈设置");
            DrawingMenu.Add("drawQH", new CheckBox("显示Q范围"));
            DrawingMenu.Add("drawPred", new CheckBox("显示Q命中率"));
            DrawingMenu.AddLabel("伤害显示");
            DrawingMenu.Add("draw.Damage", new CheckBox("显示伤害"));
            DrawingMenu.Add("draw.Q", new CheckBox("计算Q伤害"));
            DrawingMenu.Add("draw.W", new CheckBox("计算W伤害"));
            DrawingMenu.Add("draw.E", new CheckBox("计算E伤害"));
            DrawingMenu.Add("draw.R", new CheckBox("计算R伤害", false));
            DrawingMenu.AddLabel("伤害计算显示颜色");
            DrawingMenu.Add("draw_Alpha", new Slider("Alpha: ", 255, 0, 255));
            DrawingMenu.Add("draw_Red", new Slider("Red: ", 255, 0, 255));
            DrawingMenu.Add("draw_Green", new Slider("Green: ", 0, 0, 255));
            DrawingMenu.Add("draw_Blue", new Slider("Blue: ", 0, 0, 255));

            #endregion

            #region Misc

            MiscMenu = DefaultMenu.AddSubMenu("杂项", "Misc Menu");
            MiscMenu.AddGroupLabel("自动治疗设置");
            MiscMenu.Add("autoHeal", new CheckBox("治疗友军与我"));
            MiscMenu.Add("autoHealPercent", new Slider("自动治疗百分比", 50));

            foreach (var a in EntityManager.Heroes.Allies.OrderBy(a => a.BaseSkinName))
            {
                MiscMenu.Add("autoHeal_" + a.BaseSkinName, new CheckBox("自动治疗 " + a.BaseSkinName));
            }

            MiscMenu.AddGroupLabel("技能设置");
            MiscMenu.AddLabel("请在以下只选择一个.");
            MiscMenu.Add("useQC_AfterAttack", new CheckBox("豹子状态平A后使用Q"));
            MiscMenu.Add("useQC_BeforeAttack", new CheckBox("豹子状态平A前使用Q", false));
            MiscMenu.Add("useQC_OnUpdate", new CheckBox("豹子状态后立刻使用Q", false));
            MiscMenu.AddGroupLabel("蓝量控制器");
            MiscMenu.Add("manaQ", new Slider("人类状态当蓝量百分比 >= x时使用Q", 25));
            MiscMenu.Add("manaW", new Slider("人类状态当蓝量百分比 >= x时使用W", 25));
            MiscMenu.Add("manaE", new Slider("人类状态当蓝量百分比 >= x时使用E", 25));
            MiscMenu.Add("disableMM", new CheckBox("连招模式下停止使用蓝量控制器"));

            #endregion
        }
    }
}