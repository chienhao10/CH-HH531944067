using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using GuTenTak.TwistedFate;
using SharpDX;
using EloBuddy.SDK.Constants;

namespace GuTenTak.TwistedFate
{
    internal class Program
    {
        public const string ChampionName = "TwistedFate";
        public static Menu Menu, ModesMenu1, ModesMenu2, ModesMenu3, DrawMenu;
        public static int SkinBase;
        public static Item Youmuu = new Item(ItemId.Youmuus_Ghostblade);
        public static Item Botrk = new Item(ItemId.Blade_of_the_Ruined_King);
        public static Item Cutlass = new Item(ItemId.Bilgewater_Cutlass);
        public static Item Qss = new Item(ItemId.Quicksilver_Sash);
        public static Item Simitar = new Item(ItemId.Mercurial_Scimitar);
        public static Item hextech = new Item(ItemId.Hextech_Gunblade, 700);


        public static AIHeroClient PlayerInstance
        {
            get { return Player.Instance; }
        }
        private static float HealthPercent()
        {
            return (PlayerInstance.Health / PlayerInstance.MaxHealth) * 100;
        }

        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }

        public static bool AutoQ { get; protected set; }
        public static float Manaah { get; protected set; }
        public static object GameEvent { get; private set; }

        public static Spell.Skillshot Q;
        public static Spell.Active W;
        public static Spell.Active R;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Game_OnStart;
        }


        static void Game_OnStart(EventArgs args)
        {
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Game_OnDraw;
            Obj_AI_Base.OnBuffGain += Common.OnBuffGain;
            Game.OnTick += OnTick;
            Orbwalker.OnPreAttack += Common.QAA;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            SkinBase = Player.Instance.SkinId;
            // Item
            try
            {
                if (ChampionName != PlayerInstance.BaseSkinName)
                {
                    return;
                }

                Q = new Spell.Skillshot(SpellSlot.Q, 1450, SkillShotType.Linear, 0, 1000, 40)
                {
                    AllowedCollisionCount = int.MaxValue
                };
                W = new Spell.Active(SpellSlot.W);
                R = new Spell.Active(SpellSlot.R, 5500);





                Bootstrap.Init(null);
                Chat.Print("GuTenTak Addon Loading Success", Color.Green);


                Menu = MainMenu.AddMenu("GuTenTak 卡牌", "TwistedFate");
                Menu.AddSeparator();
                Menu.AddLabel("CH汉化-GuTenTak卡牌脚本");

                var Enemies = EntityManager.Heroes.Enemies.Where(a => !a.IsMe).OrderBy(a => a.BaseSkinName);
                ModesMenu1 = Menu.AddSubMenu("菜单", "Modes1TwistedFate");
                ModesMenu1.AddSeparator();
                ModesMenu1.AddLabel("连招设置");
                ModesMenu1.Add("ComboQ", new CheckBox("使用Q", true));
                ModesMenu1.Add("ComboYellowCard", new CheckBox("连招定黄牌", true));
                ModesMenu1.Add("RYellow", new CheckBox("开R自动黄牌", true));
                ModesMenu1.Add("ComboWRed", new KeyBind("切红牌", false, KeyBind.BindTypes.HoldActive, 'T'));
                ModesMenu1.Add("ComboWBlue", new KeyBind("切蓝牌", false, KeyBind.BindTypes.HoldActive, 'E'));
                ModesMenu1.Add("ComboWYellow", new KeyBind("切黄牌", false, KeyBind.BindTypes.HoldActive, 'S'));
                ModesMenu1.Add("WHumanizer", new CheckBox("选牌-人性化", true));
                ModesMenu1.Add("WHumanizerms", new Slider("选牌-人性化延迟 (毫秒)", 250, 0, 250));
                ModesMenu1.Add("WHumanizerrandom", new Slider("选牌-人性化延迟 (随机最小值)", 125, 0, 125));
                ModesMenu1.Add("WHumanizerrandom2", new Slider("选牌-人性化延迟 (随机最大值)", 250, 0, 250));

                ModesMenu1.AddSeparator();
                ModesMenu1.AddLabel("自动骚扰");
                ModesMenu1.Add("AutoHarass", new CheckBox("自动Q无法移动的敌人", true));
                ModesMenu1.Add("ManaAuto", new Slider("自动骚扰蓝量百分比", 40));
                ModesMenu1.AddSeparator();
                ModesMenu1.AddLabel("骚扰设置");
                ModesMenu1.Add("HarassQ", new CheckBox("使用Q", true));
                ModesMenu1.Add("ManaHQ", new Slider("Q 骚扰蓝量百分比", 60));
                ModesMenu1.Add("HarassW", new CheckBox("使用W", true));
                ModesMenu1.Add("ManaHW", new Slider("W 骚扰蓝量百分比", 60));
                ModesMenu1.Add("HarassPick", new ComboBox("骚扰选牌", 0, "蓝", "红", "黄"));
                ModesMenu1.AddSeparator();
                ModesMenu1.AddLabel("抢头设置");
                ModesMenu1.Add("KS", new CheckBox("开启抢头", true));
                ModesMenu1.Add("KQ", new CheckBox("Q 抢头", true));

                ModesMenu2 = Menu.AddSubMenu("农兵", "Modes2TwistedFate");
                ModesMenu2.AddLabel("尾兵设置");
                ModesMenu2.AddSeparator();
                ModesMenu2.Add("LastBlue", new CheckBox("选择蓝牌", true));
                ModesMenu2.Add("ManaLast", new Slider("蓝量低于百分比时", 40));
                ModesMenu2.AddSeparator();
                ModesMenu2.AddLabel("清线设置");
                ModesMenu2.AddSeparator();
                ModesMenu2.Add("FarmQ", new CheckBox("使用Q", true));
                ModesMenu2.Add("ManaLQ", new Slider("Q 清线蓝量百分比", 40));
                ModesMenu2.Add("MinionLC", new Slider("Q最低小兵命中数", 3, 1, 5));
                ModesMenu2.Add("FarmW", new CheckBox("清线使用W", true));
                ModesMenu2.Add("ClearPick", new ComboBox("清线选牌", 1, "红", "蓝"));
                ModesMenu2.Add("ManaLW", new Slider("W 清线蓝量百分比", 40));
                ModesMenu2.AddSeparator();
                ModesMenu2.AddLabel("清野设置");
                ModesMenu2.AddSeparator();
                ModesMenu2.Add("JungleQ", new CheckBox("使用Q", true));
                ModesMenu2.Add("ManaJQ", new Slider("Q 清野蓝量百分比", 40));
                ModesMenu2.Add("JungleW", new CheckBox("使用W", true));
                ModesMenu2.Add("JungleClearPick", new ComboBox("清野选牌", 1, "红", "蓝", "黄"));
                ModesMenu2.Add("ManaJW", new Slider("W 清野蓝量百分比", 40));

                ModesMenu3 = Menu.AddSubMenu("杂项", "Modes3TwistedFate");
                //ModesMenu3.Add("AntiGap", new CheckBox("AntiGap - Pick Golden Card", true));

                ModesMenu3.AddLabel("物品使用（连招）");
                ModesMenu3.Add("useYoumuu", new CheckBox("使用幽梦", true));
                ModesMenu3.Add("usehextech", new CheckBox("使用科技枪", true));
                ModesMenu3.Add("useBotrk", new CheckBox("使用破败&弯刀", true));
                ModesMenu3.Add("useQss", new CheckBox("使用水银饰带", true));
                ModesMenu3.Add("minHPBotrk", new Slider("最低血量 % 使用破败", 80));
                ModesMenu3.Add("enemyMinHPBotrk", new Slider("敌人最低血量 % 使用破败", 80));

                ModesMenu3.AddLabel("水银设置");
                ModesMenu3.Add("Qssmode", new ComboBox(" ", 0, "自动", "连招"));
                ModesMenu3.Add("Stun", new CheckBox("晕眩", true));
                ModesMenu3.Add("Blind", new CheckBox("致盲", true));
                ModesMenu3.Add("Charm", new CheckBox("魅惑", true));
                ModesMenu3.Add("Suppression", new CheckBox("压制", true));
                ModesMenu3.Add("Polymorph", new CheckBox("变形", true));
                ModesMenu3.Add("Fear", new CheckBox("恐惧", true));
                ModesMenu3.Add("Taunt", new CheckBox("嘲讽", true));
                ModesMenu3.Add("Silence", new CheckBox("沉默", false));
                ModesMenu3.Add("QssDelay", new Slider("使用水银延迟(毫秒)", 250, 0, 1000));

                ModesMenu3.AddLabel("解大招水银设置");
                ModesMenu3.Add("ZedUlt", new CheckBox("劫 R", true));
                ModesMenu3.Add("VladUlt", new CheckBox("吸血鬼 R", true));
                ModesMenu3.Add("FizzUlt", new CheckBox("小鱼人 R", true));
                ModesMenu3.Add("MordUlt", new CheckBox("金属大师 R", true));
                ModesMenu3.Add("PoppyUlt", new CheckBox("波比 R", true));
                ModesMenu3.Add("QssUltDelay", new Slider("使用水银解大招延迟(毫秒)", 250, 0, 1000));

                ModesMenu3.AddLabel("换肤");
                ModesMenu3.Add("skinhack", new CheckBox("开启换肤", false));
                ModesMenu3.Add("skinId", new ComboBox("模式", 0, "预设", "1", "2", "3", "4", "5", "6", "7", "8", "9"));

                DrawMenu = Menu.AddSubMenu("线圈", "DrawTwistedFate");
                DrawMenu.Add("drawA", new CheckBox(" 显示真实普攻距离", true));
                DrawMenu.Add("drawQ", new CheckBox(" 显示 Q", true));
                DrawMenu.Add("drawR", new CheckBox(" 显示 R", false));

            }

            catch (Exception e)
            {

            }

        }
        private static void Game_OnDraw(EventArgs args)
        {

            try
            {
                if (DrawMenu["drawQ"].Cast<CheckBox>().CurrentValue)
                {
                    if (Q.IsReady() && Q.IsLearned)
                    {
                        Circle.Draw(Color.White, Q.Range, Player.Instance.Position);
                    }
                }
                if (DrawMenu["drawR"].Cast<CheckBox>().CurrentValue)
                {
                    if (R.IsReady() && R.IsLearned)
                    {
                        Circle.Draw(Color.White, R.Range, Player.Instance.Position);
                    }
                }
                if (DrawMenu["drawA"].Cast<CheckBox>().CurrentValue)
                {
                    Circle.Draw(Color.LightGreen, 590, Player.Instance.Position);
                }
            }
            catch (Exception e)
            {

            }
        }
        static void Game_OnUpdate(EventArgs args)
        {
            try
            {
                //var AutoHarass = ModesMenu1["AutoHarass"].Cast<CheckBox>().CurrentValue;
                //var ManaAuto = ModesMenu1["ManaAuto"].Cast<Slider>().CurrentValue;
                var RedCard = ModesMenu1["ComboWRed"].Cast<KeyBind>().CurrentValue;
                var YellowCard = ModesMenu1["ComboWYellow"].Cast<KeyBind>().CurrentValue;
                var BlueCard = ModesMenu1["ComboWBlue"].Cast<KeyBind>().CurrentValue;

                if (YellowCard)
                {
                    Common.CardSelector.StartSelecting(Common.Cards.Yellow);
                }
                if (RedCard)
                {
                    Common.CardSelector.StartSelecting(Common.Cards.Red);
                }
                if (BlueCard)
                {
                    Common.CardSelector.StartSelecting(Common.Cards.Blue);
                }

                /*
                if (AutoHarass && ManaAuto <= _Player.ManaPercent)
                    {
                        Common.AutoQ();
                    }*/
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    Common.Combo();
                    Common.ItemUsage();
                }
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                {
                    Common.Harass();
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                {

                    Common.LaneClear();

                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                {

                    Common.JungleClear();
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
                {
                    Common.LastHit();

                }
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
                {
                    Common.Flee();

                }
            }
            catch (Exception e)
            {

            }
        }

        public static void OnTick(EventArgs args)
        {
            Common.KillSteal();
            Common.AutoQ();
            Common.Skinhack();
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (args.SData.Name.ToLower() == "gate" && ModesMenu1["RYellow"].Cast<CheckBox>().CurrentValue)
            {
                Common.CardSelector.StartSelecting(Common.Cards.Yellow);
            }
        }

    }
}
