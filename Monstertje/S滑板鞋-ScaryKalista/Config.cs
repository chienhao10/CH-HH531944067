using System.Linq;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace ScaryKalista
{
    public class Config
    {
        public static Menu Menu { get; private set; }
        public static Menu ComboMenu { get; private set; }
        public static Menu HarassMenu { get; private set; }
        public static Menu LaneMenu { get; private set; }
        public static Menu JungleMenu { get; private set; }
        public static Menu FleeMenu { get; private set; }
        public static Menu SentinelMenu { get; private set; }
        public static Menu MiscMenu { get; private set; }
        public static Menu DrawMenu { get; private set; }
        public static Menu BalistaMenu { get; private set; }
        public static Menu ItemMenu { get; private set; }

        public static void Initialize()
        {
            var blitzcrank = EntityManager.Heroes.Allies.Any(x => x.ChampionName == "Blitzcrank");

            //Initialize the menu
            Menu = MainMenu.AddMenu("S复仇之矛", "ScaryKalista");
            Menu.AddGroupLabel("欢迎使用Scary Kalista 滑板鞋，由CH汉化!");

            //Combo
            ComboMenu = Menu.AddSubMenu("连招");
            {
                ComboMenu.Add("combo.useQ", new CheckBox("使用 Q"));
                ComboMenu.Add("combo.minManaQ", new Slider("最低 {0}% 蓝量使用Q", 40));

                ComboMenu.Add("combo.sep1", new Separator());
                ComboMenu.Add("combo.useE", new CheckBox("抢头 E"));
                ComboMenu.Add("combo.gapClose", new CheckBox("使用小兵/野怪造成间距"));

                ComboMenu.Add("combo.sep2", new Separator());
                ComboMenu.Add("combo.harassEnemyE", new CheckBox("E骚扰敌人英雄当能同时杀死小兵", false));
            }

            //Harass
            HarassMenu = Menu.AddSubMenu("骚扰");
            {
                HarassMenu.Add("harass.useQ", new CheckBox("使用 Q"));
                HarassMenu.Add("harass.minManaQ", new Slider("最低 {0}% 蓝量使用Q", 60));

                HarassMenu.Add("harass.sep1", new Separator());
                HarassMenu.Add("harass.harassEnemyE", new CheckBox("E骚扰敌人英雄当能同时杀死小兵"));
            }

            //LaneClear
            LaneMenu = Menu.AddSubMenu("LaneClear");
            {
                LaneMenu.Add("laneclear.useQ", new CheckBox("使用 Q"));
                LaneMenu.Add("laneclear.minQ", new Slider("最低 {0}% 小兵数量使用Q", 3, 2, 10));
                LaneMenu.Add("laneclear.minManaQ", new Slider("最低 {0}% 蓝量使用Q", 30));

                LaneMenu.Add("laneclear.sep1", new Separator());
                LaneMenu.Add("laneclear.useE", new CheckBox("使用 E"));
                LaneMenu.Add("laneclear.minE", new Slider("最低 {0}% 小兵数量使用E", 3, 2, 10));
                LaneMenu.Add("laneclear.minManaE", new Slider("最低 {0}% 蓝量使用E", 30));

                LaneMenu.Add("laneclear.sep2", new Separator());
                LaneMenu.Add("laneclear.harassEnemyE", new CheckBox("E骚扰敌人英雄当能同时杀死小兵"));
            }

            //JungleClear
            JungleMenu = Menu.AddSubMenu("清野");
            {
                JungleMenu.Add("jungleclear.useE", new CheckBox("使用E杀死野怪"));
                JungleMenu.Add("jungleclear.miniE", new CheckBox("使用E杀死小野怪", false));
            }
            
            //Flee
            FleeMenu = Menu.AddSubMenu("逃跑");
            {
                FleeMenu.Add("flee.attack", new CheckBox("攻击 英雄/小兵/野怪"));
                FleeMenu.Add("flee.useJump", new CheckBox("在可跳墙点时使用Q跳墙"));
            }

            //Sentinel
            SentinelMenu = Menu.AddSubMenu("守卫 (W)");
            {
                SentinelMenu.Add("sentinel.castDragon", new KeyBind("往小龙送守卫", false, KeyBind.BindTypes.HoldActive, 'U'));
                SentinelMenu.Add("sentinel.castBaron", new KeyBind("往先锋/男爵送守卫", false, KeyBind.BindTypes.HoldActive, 'I'));

                SentinelMenu.Add("sentinel.sep1", new Separator());
                SentinelMenu.Add("sentinel.enable", new CheckBox("自动发送守卫", false));
                SentinelMenu.Add("sentinel.noMode", new CheckBox("无模式下才使用"));
                SentinelMenu.Add("sentinel.alert", new CheckBox("警告（本地）当守卫受到攻击"));
                SentinelMenu.Add("sentinel.mana", new Slider("最低 {0}% 蓝量使用W", 40));

                SentinelMenu.Add("sentinel.sep2", new Separator());
                SentinelMenu.Add("sentinel.locationLabel", new Label("向以下地点发送守卫:"));
                (SentinelMenu.Add("sentinel.baron", new CheckBox("男爵 / 峡谷先锋"))).OnValueChange += SentinelLocationsChanged;
                (SentinelMenu.Add("sentinel.dragon", new CheckBox("小龙"))).OnValueChange += SentinelLocationsChanged;
                (SentinelMenu.Add("sentinel.mid", new CheckBox("中路草"))).OnValueChange += SentinelLocationsChanged;
                (SentinelMenu.Add("sentinel.blue", new CheckBox("蓝"))).OnValueChange += SentinelLocationsChanged;
                (SentinelMenu.Add("sentinel.red", new CheckBox("红"))).OnValueChange += SentinelLocationsChanged;
                Sentinel.RecalculateOpenLocations();
            }

            //Misc
            MiscMenu = Menu.AddSubMenu("杂项");
            {
                MiscMenu.Add("misc.labelSteal", new Label("偷窃: 不需要按任何按钮"));
                MiscMenu.Add("misc.killstealE", new CheckBox("E抢头"));
                MiscMenu.Add("misc.junglestealE", new CheckBox("E抢野怪"));

                MiscMenu.Add("misc.sep1", new Separator());
                MiscMenu.Add("misc.autoE", new CheckBox("自动使用E"));
                MiscMenu.Add("misc.autoEHealth", new Slider("生命低于 {0}% 自动使用E", 10, 5, 25));

                MiscMenu.Add("misc.sep2", new Separator());
                MiscMenu.Add("misc.unkillableE", new CheckBox("对无法杀死的小兵使用E"));

                MiscMenu.Add("misc.sep3", new Separator());
                MiscMenu.Add("misc.useR", new CheckBox("使用R拯救友军"));
                MiscMenu.Add("misc.healthR", new Slider("友军生命低于{0}% 使用R", 15, 5, 25));
            }

            //Items
            ItemMenu = Menu.AddSubMenu("物品");
            {
                var cutlass = Items.BilgewaterCutlass;
                ItemMenu.Add("item." + cutlass.ItemInfo.Name, new CheckBox("使用 " + cutlass.ItemInfo.Name));
                ItemMenu.Add("item." + cutlass.ItemInfo.Name + "MyHp", new Slider("我的生命低于 {0}%", 80));
                ItemMenu.Add("item." + cutlass.ItemInfo.Name + "EnemyHp", new Slider("敌方生命低于 {0}%", 80));
                ItemMenu.Add("item.sep", new Separator());

                var bork = Items.BladeOfTheRuinedKing;
                ItemMenu.Add("item." + bork.ItemInfo.Name, new CheckBox("使用 "+ bork.ItemInfo.Name));
                ItemMenu.Add("item." + bork.ItemInfo.Name + "MyHp", new Slider("我的生命低于 {0}%", 80));
                ItemMenu.Add("item." + bork.ItemInfo.Name + "EnemyHp", new Slider("敌方生命低于 {0}%", 80));
            }

            //Balista
            if (blitzcrank)
            {
                BalistaMenu = Menu.AddSubMenu("机器人与滑板鞋的合体");
                {
                    BalistaMenu.Add("balista.comboOnly", new CheckBox("只在连招时使用合体技"));
                    BalistaMenu.Add("balista.distance", new Slider("我与机器人间最短距离使用合体: {0}", 400, 0, 1200));
                    BalistaMenu.Add("balista.sep", new Separator());
                    BalistaMenu.Add("balista.label", new Label("给以下敌人秀合体:"));
                    foreach (var enemy in EntityManager.Heroes.Enemies)
                    {
                        BalistaMenu.Add("balista." + enemy.ChampionName, new CheckBox(enemy.ChampionName));
                    }
                }
            }

            //Drawings
            DrawMenu = Menu.AddSubMenu("线圈");
            {
                DrawMenu.Add("draw.Q", new CheckBox("显示 Q 范围"));
                DrawMenu.Add("draw.W", new CheckBox("显示 W 范围", false));
                DrawMenu.Add("draw.E", new CheckBox("显示 E 范围"));
                DrawMenu.Add("draw.R", new CheckBox("显示 R 范围"));
                DrawMenu.Add("draw.enemyE", new CheckBox("敌人血条显示E伤害"));
                DrawMenu.Add("draw.percentage", new CheckBox("敌人血条显示E伤害百分比"));
                DrawMenu.Add("draw.jungleE", new CheckBox("野怪血条显示E伤害"));
                DrawMenu.Add("draw.killableMinions", new CheckBox("显示E可击杀的小兵"));
                DrawMenu.Add("draw.stacks", new CheckBox("显示敌人E叠加数量", false));
                DrawMenu.Add("draw.jumpSpots", new CheckBox("显示可跳墙点"));
                if (blitzcrank) DrawMenu.Add("draw.balista", new CheckBox("显示合体技距离"));
            }
        }

        private static void SentinelLocationsChanged(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
        {
            Sentinel.RecalculateOpenLocations();
        }
    }
}
