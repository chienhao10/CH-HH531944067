using System.Linq;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;

namespace Mordekaiser
{
    internal class Menu
    {
        public static EloBuddy.SDK.Menu.Menu Config = Program.Config;

        public static EloBuddy.SDK.Menu.Menu MenuQ;

        public static EloBuddy.SDK.Menu.Menu MenuW;

        public static EloBuddy.SDK.Menu.Menu MenuE;

        public static EloBuddy.SDK.Menu.Menu MenuR;

        public static EloBuddy.SDK.Menu.Menu MenuGhost;

        public static EloBuddy.SDK.Menu.Menu MenuItems;

        public static EloBuddy.SDK.Menu.Menu MenuDrawings;

        public Menu()
        {
            // Q
            MenuQ = Config.AddSubMenu("Q", "Q");
            MenuQ.Add("UseQ.Combo", new CheckBox("连招"));
            MenuQ.Add("UseQ.Lane", new ComboBox("清线", 1, "关闭", "开启", "只炮兵/超级兵"));
            MenuQ.Add("UseQ.Jungle", new ComboBox("清野", 1, "关闭", "开启", "只大型野怪"));
            MenuQ.AddSeparator();
            MenuQ.AddGroupLabel("最低治疗设置:");
            MenuQ.Add("UseQ.Lane.MinHeal", new Slider("清线:", 30));
            MenuQ.Add("UseQ.Jungle.MinHeal", new Slider("清野:", 30));

            // W
            MenuW = Config.AddSubMenu("W", "W");
            MenuW.Add("UseW.DamageRadius", new Slider("W 伤害半径 (预设 = 350):", 350, 250, 400));
            MenuW.AddSeparator();
            MenuW.Add("Allies.Active", new CheckBox("对友军使用"));
            MenuW.Add("Selected" + Utils.Player.Self.ChampionName, new ComboBox(Utils.Player.Self.ChampionName + " (自己)", Utils.TargetSelector.Ally.GetPriority(Utils.Player.Self.ChampionName), "不使用", "连招", "一直"));
            MenuW.Add("SelectedGhost", new ComboBox("龙 / 敌方幽灵", Utils.TargetSelector.Ally.GetPriority("龙"), "不使用", "连招", "一直"));
            foreach (var ally in HeroManager.Allies.Where(a => !a.IsMe))
            {
            MenuW.Add("Selected" + ally.NetworkId, new ComboBox(ally.CharData.BaseSkinName, Utils.TargetSelector.Ally.GetPriority(ally.ChampionName), "不使用", "连招", "一直"));
            }
            MenuW.AddSeparator();
            MenuW.AddGroupLabel("清线 / 清野设置:");
            MenuW.Add("UseW.Lane",
                new Slider("权限 : (0 : 关闭 | 1-6 : # 个小兵 | 7 : 自动 (推荐))", 7, 0, 7));
            MenuW.Add("UseW.Jungle", new CheckBox("清野"));
            MenuW.AddSeparator();
            MenuW.AddGroupLabel("线圈");
            MenuW.Add("DrawW.Search", new CheckBox("W 范围")); //.SetValue(new Circle(true, Color.Aqua)));
            MenuW.Add("DrawW.DamageRadius", new CheckBox("W 伤害半径"));

            // E
            MenuE = Config.AddSubMenu("E", "E");
            MenuE.Add("UseE.Combo", new CheckBox("连招"));
            MenuE.Add("UseE.Harass", new CheckBox("骚扰"));
            MenuE.Add("UseE.Lane", new CheckBox("清线"));
            MenuE.Add("UseE.Jungle", new CheckBox("清野"));
            MenuE.AddSeparator();
            MenuE.AddGroupLabel("开关设置:");
            MenuE.Add("UseE.Toggle", new KeyBind("E 开关:", false, KeyBind.BindTypes.PressToggle, 'T'));
            MenuE.AddSeparator();
            MenuE.AddGroupLabel("最低治疗设置:");
            MenuE.Add("UseE.Harass.MinHeal", new Slider("骚扰:", 30));
            MenuE.Add("UseE.Lane.MinHeal", new Slider("清线:", 30));
            MenuE.Add("UseE.Jungle.MinHeal", new Slider("清野:", 30));
            MenuE.AddSeparator();
            MenuE.AddGroupLabel("线圈");
            MenuE.Add("DrawE.Search", new CheckBox("E 范围")); //.SetValue(new Circle(true, Color.Aqua)));

            // R
            MenuR = Config.AddSubMenu("R", "R");
            MenuR.Add("UseR.Active", new CheckBox("使用 R"));
            foreach (var enemy in HeroManager.Enemies)
            {
            MenuR.Add("Selected" + enemy.NetworkId, new ComboBox(enemy.ChampionName, Utils.TargetSelector.Enemy.GetPriority(enemy.ChampionName), "不使用", "低", "中", "高"));
            }

            MenuR.AddSeparator();
            MenuR.AddGroupLabel("线圈");
            MenuR.Add("DrawR.Search", new CheckBox("R 范围")); //.SetValue(new Circle(true, Color.GreenYellow)));
            MenuR.Add("DrawR.Status.Show", new ComboBox("目标提示:", 0, "关闭", "开启", "高，的目标"));

            //ghost
            MenuGhost = Config.AddSubMenu("幽灵");
            MenuGhost.AddGroupLabel("给予幽灵什么命令?");
            MenuGhost.Add("Ghost.Use", new ComboBox("命令:", 1, "什么都不做", "和我一起作战", "攻击重要目标"));
            MenuGhost.AddSeparator();
            MenuGhost.AddGroupLabel("线圈");
            MenuGhost.Add("Ghost.Draw.Position", new CheckBox("幽灵位置"));
            MenuGhost.Add("Ghost.Draw.AARange", new CheckBox("幽灵普攻范围"));
            MenuGhost.Add("Ghost.Draw.ControlRange", new CheckBox("幽灵控制范围"));

            //items
            MenuItems = Config.AddSubMenu("物品");
            MenuItems.AddGroupLabel("以下模式使用物品:");
            MenuItems.Add("Items.Combo", new CheckBox("连招"));
            MenuItems.Add("Items.Lane", new CheckBox("清线"));
            MenuItems.Add("Items.Jungle", new CheckBox("清野"));

            //draws
            MenuDrawings = Config.AddSubMenu("其他线圈", "Drawings");
            /* [ Damage After Combo ] */
            MenuDrawings.Add("Draw.Calc.Q", new CheckBox("Q 伤害"));
            MenuDrawings.Add("Draw.Calc.W", new CheckBox("W 伤害"));
            MenuDrawings.Add("Draw.Calc.E", new CheckBox("E 伤害"));
            MenuDrawings.Add("Draw.Calc.R", new CheckBox("R 伤害"));
            MenuDrawings.Add("Draw.Calc.I", new CheckBox("点燃 伤害"));
                //.SetFontStyle(FontStyle.Regular, SharpDX.Color.Aqua));
            MenuDrawings.Add("Draw.Calc.T", new CheckBox("物品伤害"));
                //.SetFontStyle(FontStyle.Regular, SharpDX.Color.Aqua));
            if (PlayerSpells.SmiteSlot != SpellSlot.Unknown)
            {
                MenuDrawings.Add("Calc.S", new CheckBox("惩戒伤害"));
                    //.SetFontStyle(FontStyle.Regular, SharpDX.Color.Aqua));
            }
        }

        public static bool getCheckBoxItem(EloBuddy.SDK.Menu.Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(EloBuddy.SDK.Menu.Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(EloBuddy.SDK.Menu.Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(EloBuddy.SDK.Menu.Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }
    }
}