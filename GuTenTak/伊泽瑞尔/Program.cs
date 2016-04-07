using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using GuTenTak.Ezreal;
using SharpDX;

namespace GuTenTak.Ezreal
{
    internal class Program
    {
        public const string ChampionName = "Ezreal";
        public static Menu Menu, ModesMenu1, ModesMenu2, ModesMenu3, DrawMenu;
        public static int SkinBase;
        public static Item Youmuu = new Item(ItemId.Youmuus_Ghostblade);
        public static Item Botrk = new Item(ItemId.Blade_of_the_Ruined_King);
        public static Item Cutlass = new Item(ItemId.Bilgewater_Cutlass);
        public static Item Tear = new Item(ItemId.Tear_of_the_Goddess);
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
        public static Spell.Skillshot W;
        public static Spell.Skillshot E;
        public static Spell.Skillshot R;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Game_OnStart;
        }


        static void Game_OnStart(EventArgs args)
        {
            if (ChampionName != PlayerInstance.BaseSkinName)
            {
                return;
            }
                
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Game_OnDraw;
            Obj_AI_Base.OnBuffGain += Common.OnBuffGain;
            Gapcloser.OnGapcloser += Common.Gapcloser_OnGapCloser;
            Game.OnTick += OnTick;
            // Item
            SkinBase = Player.Instance.SkinId;
            try
            {
                Q = new Spell.Skillshot(SpellSlot.Q, 1150, SkillShotType.Linear, 250, 2000, 60);
                Q.AllowedCollisionCount = 0;
                W = new Spell.Skillshot(SpellSlot.W, 1000, SkillShotType.Linear, 250, 1600, 80);
                W.AllowedCollisionCount = int.MaxValue;
                E = new Spell.Skillshot(SpellSlot.E, 475, SkillShotType.Linear);
                E.AllowedCollisionCount = int.MaxValue;
                R = new Spell.Skillshot(SpellSlot.R, 3000, SkillShotType.Linear, 1000, 2000, 160);
                R.AllowedCollisionCount = int.MaxValue;



                Bootstrap.Init(null);
                Chat.Print("GuTenTak Addon Loading Success", Color.Green);


                Menu = MainMenu.AddMenu("GuTenTak EZ", "Ezreal");
                Menu.AddSeparator();
                Menu.AddLabel("CH汉化-GuTenTak 伊泽瑞尔脚本");

                var Enemies = EntityManager.Heroes.Enemies.Where(a => !a.IsMe).OrderBy(a => a.BaseSkinName);
                ModesMenu1 = Menu.AddSubMenu("菜单", "Modes1Ezreal");
                ModesMenu1.AddSeparator();
                ModesMenu1.AddLabel("连招设置");
                ModesMenu1.Add("ComboQ", new CheckBox("连招Q", true));
                ModesMenu1.Add("ComboA", new CheckBox("优先普攻（后Q）", false));
                ModesMenu1.Add("ComboW", new CheckBox("连招W", true));
                ModesMenu1.Add("ComboR", new CheckBox("连招R", true));
                ModesMenu1.Add("ManaCW", new Slider("W蓝量使用 %", 30));
                ModesMenu1.Add("RCount", new Slider("使用R如果能命中数量 >=", 3, 2, 5));
                ModesMenu1.AddSeparator();
                ModesMenu1.AddSeparator();
                ModesMenu1.AddLabel("自动骚扰设置");
                ModesMenu1.Add("AutoHarass", new CheckBox("Q自动骚扰", false));

                ModesMenu1.Add("ManaAuto", new Slider("Q蓝量使用 %", 80));
                ModesMenu1.AddLabel("骚扰设置");
                ModesMenu1.Add("HarassQ", new CheckBox("骚扰Q", true));
                ModesMenu1.Add("ManaHQ", new Slider("Q蓝量使用 %", 40));
                ModesMenu1.Add("HarassW", new CheckBox("骚扰W", true));
                ModesMenu1.Add("ManaHW", new Slider("W蓝量使用 %", 60));
                ModesMenu1.AddSeparator();
                ModesMenu1.AddLabel("抢头设置");
                ModesMenu1.Add("KS", new CheckBox("开启抢头", true));
                ModesMenu1.Add("KQ", new CheckBox("Q抢头", true));
                ModesMenu1.Add("KW", new CheckBox("W抢头", true));
                ModesMenu1.Add("KR", new CheckBox("R抢头", true));

                ModesMenu2 = Menu.AddSubMenu("尾兵", "Modes2Ezreal");
                ModesMenu2.AddLabel("尾兵设置");
                ModesMenu2.Add("ManaF", new Slider("Q蓝量使用 %", 60));
                ModesMenu2.Add("LastQ", new CheckBox("Q尾兵", true));
                ModesMenu2.AddLabel("清线设置");
                ModesMenu2.Add("ManaL", new Slider("Q蓝量使用 %", 40));
                ModesMenu2.Add("FarmQ", new CheckBox("清线Q", true));
                ModesMenu2.AddLabel("清野设置");
                ModesMenu2.Add("ManaJ", new Slider("Q蓝量使用 %", 40));
                ModesMenu2.Add("JungleQ", new CheckBox("清野Q", true));

                ModesMenu3 = Menu.AddSubMenu("杂项", "Modes3Ezreal");
                ModesMenu3.AddLabel("杂项设置");
                ModesMenu3.Add("AntiGap", new CheckBox("E 防突进", true));
                ModesMenu3.Add("StackTear", new CheckBox("泉水自动叠加女神", true));
                ModesMenu3.AddLabel("逃跑设置");
                ModesMenu3.Add("FleeQ", new CheckBox("逃跑使用Q", true));
                ModesMenu3.Add("FleeE", new CheckBox("逃跑使用E", true));
                ModesMenu3.Add("ManaFlQ", new Slider("Q蓝量使用 %", 35));
                
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
                ModesMenu3.Add("skinId", new ComboBox("模式", 0, "预设", "1", "2", "3", "4", "5", "6", "7", "8"));

                DrawMenu = Menu.AddSubMenu("线圈", "DrawEzreal");
                DrawMenu.Add("drawQ", new CheckBox(" 显示 Q", true));
                DrawMenu.Add("drawW", new CheckBox(" 显示 W", true));
                DrawMenu.Add("drawR", new CheckBox(" 显示 R", false));
                DrawMenu.Add("drawXR", new CheckBox(" 显示 不使用R", true));
                DrawMenu.Add("drawXFleeQ", new CheckBox(" 显示 逃跑不使用Q", false));
                
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
                if (DrawMenu["drawW"].Cast<CheckBox>().CurrentValue)
                {
                    if (W.IsReady() && W.IsLearned)
                    {
                        Circle.Draw(Color.White, W.Range, Player.Instance.Position);
                    }
                }
                if (DrawMenu["drawR"].Cast<CheckBox>().CurrentValue)
                {
                    if (R.IsReady() && R.IsLearned)
                    {
                        Circle.Draw(Color.White, R.Range, Player.Instance.Position);
                    }
                }
                if (DrawMenu["drawXR"].Cast<CheckBox>().CurrentValue)
                {
                    if (R.IsReady() && R.IsLearned)
                    {
                        Circle.Draw(Color.Red, 700, Player.Instance.Position);
                    }
                }
                if (DrawMenu["drawXFleeQ"].Cast<CheckBox>().CurrentValue)
                {
                    if (Q.IsReady() && Q.IsLearned)
                    {
                        Circle.Draw(Color.Red, 400, Player.Instance.Position);
                    }
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
                var AutoHarass = ModesMenu1["AutoHarass"].Cast<CheckBox>().CurrentValue;
                var ManaAuto = ModesMenu1["ManaAuto"].Cast<Slider>().CurrentValue;

                Common.Skinhack();


                if (AutoHarass && ManaAuto <= _Player.ManaPercent)
                {
                    Common.AutoQ();
                }

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
            if (ModesMenu1["ComboA"].Cast<CheckBox>().CurrentValue && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Orbwalker.OnPostAttack += Common.Orbwalker_OnPostAttack;
            }
            else
            {
                Orbwalker.OnPostAttack -= Common.Orbwalker_OnPostAttack;
            }
            Common.KillSteal();
            Common.StackTear();
        }
    }
}
