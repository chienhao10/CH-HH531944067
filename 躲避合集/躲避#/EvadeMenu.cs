using System.Collections.Generic;
using System.Linq;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace EvadePlus
{
    internal class EvadeMenu
    {
        public static Menu MainMenu { get; private set; }
        public static Menu SkillshotMenu { get; private set; }
        public static Menu SpellMenu { get; private set; }
        public static Menu DrawMenu { get; private set; }
        public static Menu HotkeysMenu { get; private set; }

        public static readonly Dictionary<string, EvadeSkillshot> MenuSkillshots = new Dictionary<string, EvadeSkillshot>();

        public static void CreateMenu()
        {
            if (MainMenu != null)
            {
                return;
            }

            MainMenu = EloBuddy.SDK.Menu.MainMenu.AddMenu("躲避#", "EvadePlus");

            // Set up main menu
            MainMenu.AddGroupLabel("一般设置");
            MainMenu.Add("fowDetection", new CheckBox("开启战争迷雾探测"));
            MainMenu.AddLabel("开启: 会躲避来自战争迷雾的技能, 关闭: 人性化一点，毕竟战争迷雾是黑的人反应没那么快");
            MainMenu.AddSeparator(3);

            MainMenu.Add("processSpellDetection", new CheckBox("开启技能生成探测"));
            MainMenu.AddLabel("探测技能施放前就计算弹道 推荐开启");
            MainMenu.AddSeparator(3);

            MainMenu.Add("limitDetectionRange", new CheckBox("限制技能探测距离"));
            MainMenu.AddLabel("只探测你附近的技能, 推荐开启");
            MainMenu.AddSeparator(3);

            MainMenu.Add("recalculatePosition", new CheckBox("允许重新计算躲避位置", false));
            MainMenu.AddLabel("允许躲到一半找到新的躲避位置, 不推荐打开");
            MainMenu.AddSeparator(3);

            MainMenu.Add("moveToInitialPosition", new CheckBox("躲避后移动到之前希望到达的位置", false));
            MainMenu.AddLabel("走到一半开始躲避，躲避后会走到刚才想去到的地方");
            MainMenu.AddSeparator(3);

            MainMenu.Add("serverTimeBuffer", new Slider("服务器时间缓冲", 0, 0, 200));
            MainMenu.AddLabel("额外时间加入到躲避计算中");
            MainMenu.AddSeparator();

            MainMenu.AddGroupLabel("人性化");
            MainMenu.Add("skillshotActivationDelay", new Slider("躲避延迟", 0, 0, 400));
            MainMenu.AddSeparator(10);

            MainMenu.Add("extraEvadeRange", new Slider("额外躲避距离", 0, 0, 300));
            MainMenu.Add("randomizeExtraEvadeRange", new CheckBox("随机额外躲避距离", false));

            // Set up skillshot menu
            var heroes = Program.DeveloperMode ? EntityManager.Heroes.AllHeroes : EntityManager.Heroes.Enemies;
            var heroNames = heroes.Select(obj => obj.ChampionName).ToArray();
            var skillshots =
                SkillshotDatabase.Database.Where(s => heroNames.Contains(s.SpellData.ChampionName)).ToList();
            skillshots.AddRange(
                SkillshotDatabase.Database.Where(
                    s =>
                        s.SpellData.ChampionName == "AllChampions" &&
                        heroes.Any(obj => obj.Spellbook.Spells.Select(c => c.Name).Contains(s.SpellData.SpellName))));

            SkillshotMenu = MainMenu.AddSubMenu("指向性技能");
            SkillshotMenu.AddLabel(string.Format("Skillshots Loaded {0}", skillshots.Count));
            SkillshotMenu.AddSeparator();

            foreach (var c in skillshots)
            {
                var skillshotString = c.ToString().ToLower();

                if (MenuSkillshots.ContainsKey(skillshotString))
                    continue;

                MenuSkillshots.Add(skillshotString, c);

                SkillshotMenu.AddGroupLabel(c.DisplayText);
                SkillshotMenu.Add(skillshotString + "/enable", new CheckBox("躲避"));
                SkillshotMenu.Add(skillshotString + "/draw", new CheckBox("显示"));

                var dangerous = new CheckBox("危险", c.SpellData.IsDangerous);
                dangerous.OnValueChange += delegate(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
                {
                    GetSkillshot(sender.SerializationId).SpellData.IsDangerous = args.NewValue;
                };
                SkillshotMenu.Add(skillshotString + "/dangerous", dangerous);

                var dangerValue = new Slider("危险数值", c.SpellData.DangerValue, 1, 5);
                dangerValue.OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                {
                    GetSkillshot(sender.SerializationId).SpellData.DangerValue = args.NewValue;
                };
                SkillshotMenu.Add(skillshotString + "/dangervalue", dangerValue);

                SkillshotMenu.AddSeparator();
            }

            // Set up spell menu
            SpellMenu = MainMenu.AddSubMenu("敌方技能");
            SpellMenu.AddGroupLabel("闪现");
            SpellMenu.Add("flash", new Slider("危险等级", 5, 0, 5));

            // Set up draw menu
            DrawMenu = MainMenu.AddSubMenu("线圈");
            DrawMenu.AddGroupLabel("躲避线圈");
            DrawMenu.Add("disableAllDrawings", new CheckBox("屏蔽所有线圈", false));
            DrawMenu.Add("drawEvadePoint", new CheckBox("显示躲避位置"));
            DrawMenu.Add("drawEvadeStatus", new CheckBox("显示躲避状态"));
            DrawMenu.Add("drawDangerPolygon", new CheckBox("显示危险技能多边形线圈", false));
            DrawMenu.AddSeparator();
            DrawMenu.Add("drawPath", new CheckBox("显示自动躲避路径"));

            // Set up controls menu
            HotkeysMenu = MainMenu.AddSubMenu("快捷键");
            HotkeysMenu.AddGroupLabel("开关");
            HotkeysMenu.Add("enableEvade", new KeyBind("开启躲避", true, KeyBind.BindTypes.PressToggle, 'M'));
            HotkeysMenu.Add("dodgeOnlyDangerous", new KeyBind("只躲避危险的", false, KeyBind.BindTypes.HoldActive));
        }

        private static EvadeSkillshot GetSkillshot(string s)
        {
            return MenuSkillshots[s.ToLower().Split('/')[0]];
        }

        public static bool IsSkillshotEnabled(EvadeSkillshot skillshot)
        {
            var valueBase = SkillshotMenu[skillshot + "/enable"];
            return valueBase != null && valueBase.Cast<CheckBox>().CurrentValue;
        }

        public static bool IsSkillshotDrawingEnabled(EvadeSkillshot skillshot)
        {
            var valueBase = SkillshotMenu[skillshot + "/draw"];
            return valueBase != null && valueBase.Cast<CheckBox>().CurrentValue;
        }
    }
}