using System.Linq.Expressions;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Ex1L_Riven.Base
{
    internal class MenuDesigner
    {
        public const string MenuName = "CH汉化-瑞文";

        public static Menu RivenUi;
        public static Menu ComboUi;
        public static Menu EmoteUi;
        public static Menu ClearUi;
        public static Menu MiscUi;
        public static Menu LevelUi;
        public static Menu DrawUi;

        public static void Initialize()
        {
            RivenUi = MainMenu.AddMenu(MenuName, MenuName.ToLower());
            RivenUi.AddGroupLabel("欢迎使用 Ex1L 瑞文");
            RivenUi.AddLabel("由CH汉化，Enelx制作");

            ComboUi = RivenUi.AddSubMenu("连招");
            ComboUi.Add("UseR", new KeyBind("连招使用R", true, KeyBind.BindTypes.PressToggle, 'K'));
            ComboUi.Add("UseFlash", new CheckBox("爆发模式使用闪现"));
            ComboUi.AddSeparator();
            ComboUi.AddGroupLabel("连招 - 普通 | 爆发");
            ComboUi.AddLabel("R2 线圈显示 = [绿] - 普通 | [红] - 爆发");
            ComboUi.Add("switchCombo", new KeyBind("切换连招模式", false, KeyBind.BindTypes.HoldActive, 'T'))
                .OnValueChange += ModeController.ModeSwitch;

            EmoteUi = RivenUi.AddSubMenu("动作");
            EmoteUi.Add("UseEmote", new CheckBox("使用动作取消后摇"));
            EmoteUi.AddSeparator();
            EmoteUi.Add("EmoteSelect", new ComboBox("选择动作（推荐跳舞）", new[] {"笑", "跳舞", "搞笑", "嘲讽"}));
            EmoteUi.AddSeparator();
            EmoteUi.AddGroupLabel("高级设置，老司机懂的再改不懂别乱改");
            EmoteUi.Add("Q1Q2delay", new Slider("选择 Q1 / Q2 延迟", 293, 1, 400));
            EmoteUi.Add("Q3delay", new Slider("选择 Q3 延迟", 393, 1, 500));

            ClearUi = RivenUi.AddSubMenu("清理");
            ClearUi.AddGroupLabel("清野");
            ClearUi.Add("JungleE", new CheckBox("使用 E"));
            ClearUi.Add("JungleW", new CheckBox("使用W"));
            ClearUi.AddSeparator();
            ClearUi.AddGroupLabel("清线");
            ClearUi.Add("LaneE", new CheckBox("使用E"));
            ClearUi.Add("LaneW", new CheckBox("使用W"));
            ClearUi.AddSeparator();
            ClearUi.Add("Wminions", new Slider("选择最少可用W击杀的小兵", 2, 1, 6));

            MiscUi = RivenUi.AddSubMenu("杂项");
            MiscUi.Add("AutoW", new CheckBox("X 名敌人时自动W"));
            MiscUi.AddSeparator();
            MiscUi.Add("Wenemies", new Slider("选择最少敌人数量", 2, 1, 5));
            MiscUi.AddSeparator();
            MiscUi.Add("InterW", new CheckBox("使用W 打断技能"));
            MiscUi.Add("GapWE", new CheckBox("使用W - E 防突进"));
            MiscUi.Add("UseItems", new CheckBox("物品"));

            LevelUi = RivenUi.AddSubMenu("自动加点");
            LevelUi.Add("UseLevel", new CheckBox("使用自动加点"));
            LevelUi.AddSeparator();
            LevelUi.Add("SequenceSelect", new ComboBox("选择顺序", new[] {"Q > E > W", "Q > W > E", "E > Q > W"}));
            LevelUi.AddSeparator();
            LevelUi.Add("LevelHumanizer", new Slider("人性化 （毫秒）", 700, 1, 1000));

            DrawUi = RivenUi.AddSubMenu("线圈");
            DrawUi.Add("Rstatus", new CheckBox("显示 R1 状态"));
            DrawUi.Add("Rrange", new CheckBox("显示 R2 范围"));
        }
    }
}