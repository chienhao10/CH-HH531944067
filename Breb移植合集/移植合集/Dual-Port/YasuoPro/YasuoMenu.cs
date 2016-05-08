using System.Reflection;
using System.Linq;
using LeagueSharp.Common;
using EloBuddy.SDK.Menu;

namespace YasuoPro
{
    internal static class YasuoMenu
    {
        internal static Menu Config, ComboM, HarassM, EvadeM, KillstealM, FarmingM, WaveclearM, MiscM, DrawingsM;
        internal static Yasuo Yas;

        public static void Init(Yasuo yas)
        {
            Yas = yas;

            Config = MainMenu.AddMenu("亚索Pro", "YasuoPro");

            ComboM = Config.AddSubMenu("连招");
            YasuoMenu.Combo.Attach(ComboM);

            HarassM = Config.AddSubMenu("骚扰");
            YasuoMenu.Harass.Attach(HarassM);

            EvadeM = Config.AddSubMenu("躲避");
            YasuoMenu.Evade.Attach(EvadeM);

            KillstealM = Config.AddSubMenu("抢头");
            YasuoMenu.Killsteal.Attach(KillstealM);

            FarmingM = Config.AddSubMenu("尾兵");
            YasuoMenu.Farm.Attach(FarmingM);

            WaveclearM = Config.AddSubMenu("清线");
            YasuoMenu.Waveclear.Attach(WaveclearM);

            MiscM = Config.AddSubMenu("杂项");
            YasuoMenu.Misc.Attach(MiscM);

            DrawingsM = Config.AddSubMenu("线圈");
            YasuoMenu.Drawings.Attach(DrawingsM);
        }

        struct Combo
        {
            internal static void Attach(Menu menu)
            {
                menu.AddGroupLabel("物品");
                menu.AddBool("Items.Enabled", "使用 物品");
                menu.AddBool("Items.UseTIA", "使用 提亚马特");
                menu.AddBool("Items.UseHDR", "使用 九头蛇");
                menu.AddBool("Items.UseBRK", "使用 破败");
                menu.AddBool("Items.UseBLG", "使用 弯刀");
                menu.AddBool("Items.UseYMU", "使用 幽梦");
                menu.AddSeparator();
                menu.AddGroupLabel("连招 :");
                menu.AddBool("Combo.UseQ", "使用 Q");
                menu.AddBool("Combo.UseQ2", "使用 Q2");
                menu.AddBool("Combo.StackQ", "叠加 Q 如果不在范围内");
                menu.AddBool("Combo.UseW", "使用 W");
                menu.AddBool("Combo.UseE", "使用 E");
                menu.AddBool("Combo.UseEQ", "使用 EQ");
                menu.AddBool("Combo.ETower", "使用 E 塔下", false);
                menu.AddBool("Combo.EAdvanced", "预判 E 路线");
                menu.AddBool("Combo.NoQ2Dash", "冲刺时不 Q2", false);
                menu.AddBool("Combo.UseIgnite", "使用 点燃");
                menu.AddSeparator();
                menu.AddGroupLabel("大招 设置");
                foreach (var hero in HeroManager.Enemies)
                {
                    menu.AddBool("ult" + hero.ChampionName, "大招 " + hero.ChampionName);
                }
                menu.AddSeparator();
                menu.AddSList("Combo.UltMode", "大招 优先顺序", new string[] { "最低血量", "目标选择器", "命中最多" }, 0);
                menu.AddSlider("Combo.knockupremainingpct", "击飞目标剩下血量 % 使用大招", 95, 40, 100);
                menu.AddBool("Combo.UseR", "使用 R");
                menu.AddBool("Combo.UltTower", "塔下大招r", false);
                menu.AddBool("Combo.UltOnlyKillable", "R 只用在可击杀的目标", false);
                menu.AddBool("Combo.RPriority", "优先使用在击飞5个英雄时", true);
                menu.AddSeparator();
                menu.AddSlider("Combo.RMinHit", "最少英雄数使用 R", 1, 1, 5);
                menu.AddBool("Combo.OnlyifMin", "只大招最低敌人数量", false);
                menu.AddSeparator();
                menu.AddSlider("Combo.MinHealthUlt", "最低血量 % 使用R", 0, 0, 100);
            }
        }

        struct Harass
        {
            internal static void Attach(Menu menu)
            {
                menu.AddKeyBind("Harass.KB", "骚扰按键", KeyCode("H"), EloBuddy.SDK.Menu.Values.KeyBind.BindTypes.PressToggle);
                menu.AddSeparator();
                menu.AddBool("Harass.InMixed", "混合模式使用骚扰", false);
                menu.AddBool("Harass.UseQ", "使用 Q");
                menu.AddBool("Harass.UseQ2", "使用 Q2");
                menu.AddBool("Harass.UseE", "使用 E", false);
                menu.AddBool("Harass.UseEMinion", "使用 E 小兵", false);
            }
        }

        struct Farm
        {
            internal static void Attach(Menu menu)
            {
                menu.AddBool("Farm.UseQ", "使用 Q");
                menu.AddBool("Farm.UseQ2", "使用 Q - 龙卷风");
                menu.AddSlider("Farm.Qcount", "对小兵使用 Q (龙卷风)", 1, 1, 10);
                menu.AddBool("Farm.UseE", "使用 E");
            }
        }


        struct Waveclear
        {
            internal static void Attach(Menu menu)
            {
                menu.AddGroupLabel("清线物品");
                menu.AddBool("Waveclear.UseItems", "使用 物品");
                menu.AddSlider("Waveclear.MinCountHDR", "最低小兵数量使用九头", 2, 1, 10);
                menu.AddSlider("Waveclear.MinCountYOU", "最低小兵数量使用幽梦", 2, 1, 10);
                menu.AddBool("Waveclear.UseTIA", "使用 提亚马特");
                menu.AddBool("Waveclear.UseHDR", "使用 九头");
                menu.AddBool("Waveclear.UseYMU", "使用 幽梦", false);
                menu.AddSeparator();
                menu.AddGroupLabel("清线 :");
                menu.AddBool("Waveclear.UseQ", "使用 Q");
                menu.AddBool("Waveclear.UseQ2", "使用 Q - 龙卷风");
                menu.AddSlider("Waveclear.Qcount", "对小兵使用 Q (龙卷风)", 1, 1, 10);
                menu.AddSeparator();
                menu.AddBool("Waveclear.UseE", "使用 E");
                menu.AddSlider("Waveclear.Edelay", "E 延迟", 0, 0, 400);
                menu.AddBool("Waveclear.ETower", "使用 E 塔下", false);
                menu.AddBool("Waveclear.UseENK", "使用 E (就算不可击杀)");
                menu.AddSeparator();
                menu.AddBool("Waveclear.Smart", "智能清线");
            }
        }

        struct Killsteal
        {
            internal static void Attach(Menu menu)
            {
                menu.AddBool("Killsteal.Enabled", "抢头");
                menu.AddBool("Killsteal.UseQ", "使用 Q");
                menu.AddBool("Killsteal.UseE", "使用 E");
                menu.AddBool("Killsteal.UseR", "使用 R");
                menu.AddBool("Killsteal.UseIgnite", "使用 点燃");
                menu.AddBool("Killsteal.UseItems", "使用 物品");
            }
        }


        struct Evade
        {
            internal static void Attach(Menu menu)
            {
                menu.AddGroupLabel("目标技能");

                foreach (var spell in TargettedDanger.spellList.Where(x => HeroManager.Enemies.Any(e => e.CharData.BaseSkinName == x.championName)))
                {
                    menu.AddBool("enabled." + spell.spellName, spell.spellName + " (" + spell.championName + ")", true);
                    menu.AddSlider("enabled." + spell.spellName + ".delay", spell.spellName + " 延迟", 0, 0, 1000);
                    menu.AddSeparator();
                }

                menu.AddGroupLabel("测试技能");
                foreach (var spell in TargettedDanger.spellList.Where(x => HeroManager.Enemies.Any(e => x.championName == "Baron")))
                {
                    menu.AddBool("enabled." + spell.spellName, spell.spellName + " (" + spell.championName + ")", true);
                }
                menu.AddSeparator();
                menu.AddBool("Evade.Enabled", "躲避已开启");
                menu.AddBool("Evade.OnlyDangerous", "只躲避危险的", false);
                menu.AddBool("Evade.FOW", "躲避来自战争迷雾的技能");
                menu.AddBool("Evade.WTS", "对目标技能使用风墙");
                menu.AddBool("Evade.WSS", "对技能使用风墙");
                menu.AddBool("Evade.UseW", "使用风墙躲避");
                menu.AddBool("Evade.UseE", "使用 E 躲避");
                menu.AddSeparator();
                menu.AddSlider("Evade.MinDangerLevelWW", "最低危险等级使用风墙", 1, 1, 5);
                menu.AddSlider("Evade.MinDangerLevelE", "最低危险等级使用 E", 1, 1, 5);
                menu.AddSlider("Evade.Delay", "风墙延迟", 0, 0, 1000);
            }
        }


        struct Misc
        {
            internal static void Attach(Menu menu)
            {
                menu.AddGroupLabel("逃跑");
                menu.AddSList("Flee.Mode", "Flee Mode", new[] { "至泉水", "至友军", "至鼠标" }, 2);
                menu.AddBool("Flee.Smart", "智能逃跑", true);
                menu.AddBool("Flee.StackQ", "逃跑时叠加Q");
                menu.AddBool("Flee.UseQ2", "使用龙卷风", false);
                menu.AddSeparator();
                menu.AddGroupLabel("杂项 :");
                menu.AddBool("Misc.SafeE", "E 安全检查");
                menu.AddBool("Misc.AutoStackQ", "自动叠加 Q", false);
                menu.AddBool("Misc.AutoR", "自动大招");
                menu.AddSlider("Misc.RMinHit", "最低敌人数量 R", 1, 1, 5);
                menu.AddSeparator();
                menu.AddKeyBind("Misc.TowerDive", "越塔按键", KeyCode("T"), EloBuddy.SDK.Menu.Values.KeyBind.BindTypes.HoldActive);
                menu.AddSeparator();
                menu.AddSList("Hitchance.Q", "Q 命中率", new[] { HitChance.Low.ToString(), HitChance.Medium.ToString(), HitChance.High.ToString(), HitChance.VeryHigh.ToString() }, 2);
                menu.AddSlider("Misc.Healthy", "X 血量为安全", 5, 0, 100);
                menu.AddSeparator();
                menu.AddBool("Misc.AG", "使用 Q (龙卷风) 防突进");
                menu.AddBool("Misc.Interrupter", "使用 Q (龙卷风) 技能打断");
                menu.AddBool("Misc.Walljump", "使用 跳墙", false);
                menu.AddBool("Misc.Debug", "调试", false);
            }
        }

        struct Drawings
        {
            internal static void Attach(Menu menu)
            {
                menu.AddBool("Drawing.Disable", "屏蔽线圈", true);
                menu.AddCircle("Drawing.DrawQ", "显示 Q");//, Yas.Qrange, System.Drawing.Color.Red);
                menu.AddCircle("Drawing.DrawE", "显示 E");//, Yas.Erange, System.Drawing.Color.CornflowerBlue);
                menu.AddCircle("Drawing.DrawR", "显示 R");//, Yas.Rrange, System.Drawing.Color.DarkOrange);
                menu.AddBool("Drawing.SS", "显示技能释放位置", false);
            }
        }

        internal static uint KeyCode(string s)
        {
            return s.ToCharArray()[0];
        }
    }
}
