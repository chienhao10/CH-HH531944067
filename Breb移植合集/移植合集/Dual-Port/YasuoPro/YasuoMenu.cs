using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;

namespace YasuoPro
{
    internal static class YasuoMenu
    {
        internal static Menu Config;
        internal static Yasuo Yas;

        public static Menu ComboA, HarassA, KillstealA, FarmingA, WaveclearA, MiscA, DrawingsA, Flee;

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        public static void Init(Yasuo yas)
        {
            Yas = yas;

            Config = MainMenu.AddMenu("亚索Pro", "YasuoPro");

            ComboA = Config.AddSubMenu("连招");
            Combo.Attach(ComboA);

            HarassA = Config.AddSubMenu("骚扰");
            Harass.Attach(HarassA);

            KillstealA = Config.AddSubMenu("抢头");
            Killsteal.Attach(KillstealA);

            FarmingA = Config.AddSubMenu("尾兵");
            Farm.Attach(FarmingA);

            WaveclearA = Config.AddSubMenu("清线");
            Waveclear.Attach(WaveclearA);

            MiscA = Config.AddSubMenu("杂项");
            Misc.Attach(MiscA);

            DrawingsA = Config.AddSubMenu("线圈");
            Drawings.Attach(DrawingsA);

            Flee = Config.AddSubMenu("逃跑设置", "Flee");
            Flee.Add("Flee.Mode", new ComboBox("逃跑模式", 0, "至泉水", "至友军", "至鼠标"));
            Flee.Add("Flee.StackQ", new CheckBox("逃跑时叠加Q"));
            Flee.Add("Flee.UseQ2", new CheckBox("使用龙卷风", false));
        }

        internal static uint KeyCode(string s)
        {
            return s.ToCharArray()[0];
        }

        private struct Combo
        {
            internal static void Attach(Menu menu)
            {
                menu.AddGroupLabel("物品");
                menu.Add("Items.Enabled", new CheckBox("使用 物品"));
                menu.Add("Items.UseTIA", new CheckBox("使用 提亚马特"));
                menu.Add("Items.UseHDR", new CheckBox("使用 九头蛇"));
                menu.Add("Items.UseBRK", new CheckBox("使用 破败"));
                menu.Add("Items.UseBLG", new CheckBox("使用 弯刀"));
                menu.Add("Items.UseYMU", new CheckBox("使用 幽梦"));
                menu.AddSeparator();

                menu.AddGroupLabel("连招");
                menu.Add("Combo.UseQ", new CheckBox("使用 Q"));
                menu.Add("Combo.UseQ2", new CheckBox("使用 Q2"));
                menu.Add("Combo.StackQ", new CheckBox("叠加 Q 如果不在范围内"));
                menu.Add("Combo.UseW", new CheckBox("使用 W"));
                menu.Add("Combo.UseE", new CheckBox("使用 E"));
                menu.Add("Combo.ETower", new CheckBox("使用 E 塔下", false));
                menu.Add("Combo.EAdvanced", new CheckBox("预判 E 路线"));
                menu.Add("Combo.NoQ2Dash", new CheckBox("冲刺时不 Q2", false));
                menu.AddSeparator();

                menu.AddGroupLabel("大招 设置");
                foreach (var hero in HeroManager.Enemies)
                {
                    menu.Add("ult" + hero.ChampionName, new CheckBox("大招 " + hero.ChampionName));
                }
                menu.Add("Combo.UltMode",
                    new ComboBox("大招 优先顺序", 0, "最低血量", "目标选择器", "命中最多"));
                menu.Add("Combo.knockupremainingpct", new Slider("剩下 % 使用大招", 95, 40));
                menu.Add("Combo.UseR", new CheckBox("使用 R"));
                menu.Add("Combo.UltTower", new CheckBox("塔下大招", false));
                menu.Add("Combo.UltOnlyKillable", new CheckBox("R 只用在可击杀的目标", false));
                menu.Add("Combo.RPriority", new CheckBox("优先使用在击飞5个英雄时"));
                menu.Add("Combo.RMinHit", new Slider("最少英雄数使用R", 1, 1, 5));
                menu.Add("Combo.OnlyifMin", new CheckBox("只大招最低敌人数量", false));
                menu.Add("Combo.MinHealthUlt", new Slider("最低血量 % 使用R"));
                menu.AddSeparator();

                menu.Add("Combo.UseIgnite", new CheckBox("使用 点燃"));
            }
        }

        private struct Harass
        {
            internal static void Attach(Menu menu)
            {
                menu.Add("Harass.KB", new KeyBind("骚扰按键", false, KeyBind.BindTypes.PressToggle, 'H'));
                menu.Add("Harass.InMixed", new CheckBox("混合模式使用骚扰", false));
                menu.Add("Harass.UseQ", new CheckBox("使用 Q"));
                menu.Add("Harass.UseQ2", new CheckBox("使用 Q2"));
                menu.Add("Harass.UseE", new CheckBox("使用 E", false));
                menu.Add("Harass.UseEMinion", new CheckBox("使用 E 小兵", false));
            }
        }

        private struct Farm
        {
            internal static void Attach(Menu menu)
            {
                menu.Add("Farm.UseQ", new CheckBox("使用 Q"));
                menu.Add("Farm.UseQ2", new CheckBox("使用 Q - 龙卷风"));
                menu.Add("Farm.Qcount", new Slider("对小兵使用 Q （龙卷风）", 1, 1, 10));
                menu.Add("Farm.UseE", new CheckBox("使用 E"));
            }
        }


        private struct Waveclear
        {
            internal static void Attach(Menu menu)
            {
                menu.AddGroupLabel("物品");
                menu.Add("Waveclear.UseItems", new CheckBox("使用 物品"));
                menu.Add("Waveclear.MinCountHDR", new Slider("最低小兵数量使用九头", 2, 1, 10));
                menu.Add("Waveclear.MinCountYOU", new Slider("最低小兵数量使用幽梦", 2, 1, 10));
                menu.Add("Waveclear.UseTIA", new CheckBox("使用 提亚马特"));
                menu.Add("Waveclear.UseHDR", new CheckBox("使用 九头"));
                menu.Add("Waveclear.UseYMU", new CheckBox("使用 幽梦", false));
                menu.AddSeparator();

                menu.AddGroupLabel("清线");
                menu.Add("Waveclear.UseQ", new CheckBox("使用 Q"));
                menu.Add("Waveclear.UseQ2", new CheckBox("使用 Q - 龙卷风"));
                menu.Add("Waveclear.Qcount", new Slider("对小兵使用 Q （龙卷风）", 1, 1, 10));
                menu.Add("Waveclear.UseE", new CheckBox("使用 E"));
                menu.Add("Waveclear.ETower", new CheckBox("使用 E 塔下", false));
                menu.Add("Waveclear.UseENK", new CheckBox("使用 E （就算不可击杀)"));
                menu.Add("Waveclear.Smart", new CheckBox("智能清线"));
            }
        }

        private struct Killsteal
        {
            internal static void Attach(Menu menu)
            {
                menu.Add("Killsteal.Enabled", new CheckBox("抢头"));
                menu.Add("Killsteal.UseQ", new CheckBox("使用 Q"));
                menu.Add("Killsteal.UseE", new CheckBox("使用 E"));
                menu.Add("Killsteal.UseR", new CheckBox("使用 R"));
                menu.Add("Killsteal.UseIgnite", new CheckBox("使用 点燃"));
                menu.Add("Killsteal.UseItems", new CheckBox("使用 物品"));
            }
        }

        private struct Misc
        {
            internal static void Attach(Menu menu)
            {
                menu.Add("Misc.SafeE", new CheckBox("E 安全检查"));
                menu.Add("Misc.AutoStackQ", new CheckBox("制度叠加 Q", false));
                menu.Add("Misc.AutoR", new CheckBox("自动大招"));
                menu.Add("Misc.RMinHit", new Slider("最低敌人数量 R", 1, 1, 5));
                menu.Add("Misc.TowerDive", new KeyBind("越塔按键", false, KeyBind.BindTypes.HoldActive, 'T'));
                menu.Add("Hitchance.Q",
                    new ComboBox("Q 命中率", 2, HitChance.Low.ToString(), HitChance.Medium.ToString(),
                        HitChance.High.ToString(), HitChance.VeryHigh.ToString()));
                menu.Add("Misc.Healthy", new Slider("正常血量", 5));
                menu.Add("Misc.AG", new CheckBox("使用 Q (龙卷风) 防突进"));
                menu.Add("Misc.Interrupter", new CheckBox("使用 Q (龙卷风) 技能打断"));
                menu.Add("Misc.Walljump", new CheckBox("使用 跳墙", false));
                menu.Add("Misc.Debug", new CheckBox("调试", false));
            }
        }

        private struct Drawings
        {
            internal static void Attach(Menu menu)
            {
                menu.Add("Drawing.Disable", new CheckBox("屏蔽线圈"));
                menu.Add("Drawing.DrawQ", new CheckBox("显示 Q")); // Yas.Qrange, System.Drawing.Color.Red);
                menu.Add("Drawing.DrawE", new CheckBox("显示 E")); // Yas.Erange, System.Drawing.Color.CornflowerBlue);
                menu.Add("Drawing.DrawR", new CheckBox("显示 R")); // Yas.Rrange, System.Drawing.Color.DarkOrange);
                menu.Add("Drawing.SS", new CheckBox("显示技能释放位置", false));
            }
        }
    }
}