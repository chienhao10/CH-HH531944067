using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using GuTenTak.Lucian;
using SharpDX;
using EloBuddy.SDK.Constants;

namespace GuTenTak.Lucian
{
    internal class Program
    {
        public const string ChampionName = "Lucian";
        public static Menu Menu, ModesMenu1, ModesMenu2, ModesMenu3, DrawMenu;
        public static int SkinBase;
        public static Item Youmuu = new Item(ItemId.Youmuus_Ghostblade);
        public static Item Botrk = new Item(ItemId.Blade_of_the_Ruined_King);
        public static Item Cutlass = new Item(ItemId.Bilgewater_Cutlass);
        public static Item Tear = new Item(ItemId.Tear_of_the_Goddess);
        public static Item Qss = new Item(ItemId.Quicksilver_Sash);
        public static Item Simitar = new Item(ItemId.Mercurial_Scimitar);
        public static Item hextech = new Item(ItemId.Hextech_Gunblade, 700);
        public static AIHeroClient lastTarget;
        public static float lastSeen = Game.Time;
        public static float RCast = 0;
        public static Vector3 predictedPos;
        public static AIHeroClient RTarget = null;
        public static Vector3 RCastToPosition = new Vector3();
        public static Vector3 MyRCastPosition = new Vector3();
        public static bool disableMovement = false;
        public static bool PassiveUp;


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

        public static Spell.Targeted Q;
        public static Spell.Skillshot Q1;
        public static Spell.Skillshot W;
        public static Spell.Skillshot W1;
        public static Spell.Skillshot E;
        public static Spell.Skillshot R;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Game_OnStart;
        }


        static void Game_OnStart(EventArgs args)
        {
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Game_OnDraw;
            Gapcloser.OnGapcloser += Common.Gapcloser_OnGapCloser;
            Obj_AI_Base.OnBuffGain += Common.OnBuffGain;
            Game.OnTick += OnTick;
            Orbwalker.OnPostAttack += Common.aaCombo;
            Orbwalker.OnPostAttack += Common.LJClear;
            Player.OnBasicAttack += Player_OnBasicAttack;
            SkinBase = Player.Instance.SkinId;
            // Item
            try
            {
                if (ChampionName != PlayerInstance.BaseSkinName)
                {
                    return;
                }

                Q = new Spell.Targeted(SpellSlot.Q, 675);
                Q1 = new Spell.Skillshot(SpellSlot.Q, 1140, SkillShotType.Linear, 350, int.MaxValue, 75);
                W = new Spell.Skillshot(SpellSlot.W, 1000, SkillShotType.Linear, 250, 1600, 100);
                W1 = new Spell.Skillshot(SpellSlot.W, 500, SkillShotType.Linear, 250, 1600, 100);
                E = new Spell.Skillshot(SpellSlot.E, 475, SkillShotType.Linear);
                R = new Spell.Skillshot(SpellSlot.R, 1400, SkillShotType.Linear, 500, 2800, 110);



                Bootstrap.Init(null);
                Chat.Print("GuTenTak Addon Loading Success", Color.Green);


                Menu = MainMenu.AddMenu("GuTenTak 卢锡安", "Lucian");
                Menu.AddSeparator();
                Menu.AddLabel("CH汉化-GuTenTak 卢锡安脚本");

                var Enemies = EntityManager.Heroes.Enemies.Where(a => !a.IsMe).OrderBy(a => a.BaseSkinName);
                ModesMenu1 = Menu.AddSubMenu("菜单", "Modes1Lucian");
                ModesMenu1.AddSeparator();
                ModesMenu1.AddLabel("连招设置");
                ModesMenu1.Add("CWeaving", new CheckBox("连招使用被动", true));
                ModesMenu1.Add("ComboQ", new CheckBox("连招Q", true));
                ModesMenu1.Add("ComboW", new CheckBox("连招W", true));
                ModesMenu1.Add("ComboE", new CheckBox("连招E", true));
                ModesMenu1.Add("ManaCW", new Slider("W蓝量使用 %", 30));
                ModesMenu1.AddSeparator();
                ModesMenu1.AddLabel("逻辑设置");
                ModesMenu1.Add("LogicAA", new ComboBox(" 连招逻辑 ", 1, "神速", "普攻距离最大伤害"));
                ModesMenu1.Add("LogicW", new ComboBox(" 普通W逻辑 ", 1, "普攻距离", "总是"));
                ModesMenu1.Add("WColision", new ComboBox(" W 体积碰撞 ", 1, "检查碰撞", "不检查碰撞"));
                ModesMenu1.Add("LogicE", new ComboBox(" E 逻辑 ", 0, "E 至鼠标(安全位置)", "E 至靠边", "E 至鼠标"));

                ModesMenu1.AddSeparator();
                //ModesMenu1.AddLabel("AutoHarass Configs");
                //ModesMenu1.Add("AutoHarass", new CheckBox("Use Q on AutoHarass", false));
               // ModesMenu1.Add("ManaAuto", new Slider("Mana %", 80));

                ModesMenu1.AddLabel("骚扰设置");
                ModesMenu1.Add("HWeaving", new CheckBox("骚扰使用被动", true));
                ModesMenu1.Add("HarassMana", new Slider("Q蓝量使用 %", 60));
                ModesMenu1.Add("HarassQ", new CheckBox("骚扰Q", true));
                ModesMenu1.Add("HarassQext", new CheckBox("骚扰使用超远Q", true));
                ModesMenu1.Add("HarassW", new CheckBox("骚扰W", true));
                ModesMenu1.Add("ManaHW", new Slider("W蓝量使用 %", 60));
                ModesMenu1.AddSeparator();
                ModesMenu1.AddLabel("抢头设置");
                ModesMenu1.Add("KS", new CheckBox("开启抢头", true));
                ModesMenu1.Add("KQ", new CheckBox("Q抢头", true));
                ModesMenu1.Add("KW", new CheckBox("W抢头", true));
                ModesMenu1.Add("KR", new CheckBox("R抢头", false));

                ModesMenu2 = Menu.AddSubMenu("尾兵", "Modes2Lucian");
                ModesMenu2.AddLabel("清线设置");
                ModesMenu1.AddSeparator();
                ModesMenu2.Add("FarmQ", new CheckBox("清线Q", true));
                ModesMenu2.Add("ManaLQ", new Slider("Q蓝量使用 %", 40));
                ModesMenu2.Add("FarmW", new CheckBox("清线W", true));
                ModesMenu2.Add("ManaLW", new Slider("W蓝量使用 %", 40));
                ModesMenu2.AddLabel("清野设置");
                ModesMenu2.Add("JungleQ", new CheckBox("清野Q", true));
                ModesMenu2.Add("ManaJQ", new Slider("Q蓝量使用 %", 40));
                ModesMenu2.Add("JungleW", new CheckBox("清野QW", true));
                ModesMenu2.Add("ManaJW", new Slider("W蓝量使用 %", 40));

                ModesMenu3 = Menu.AddSubMenu("杂项", "Modes3Lucian");
                ModesMenu3.Add("AntiGap", new CheckBox("E 防突进", true));
                ModesMenu3.AddLabel("逃跑设置");
                ModesMenu3.Add("FleeE", new CheckBox("逃跑使用E", true));

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

                DrawMenu = Menu.AddSubMenu("线圈", "DrawLucian");
                DrawMenu.Add("drawA", new CheckBox(" 显示真实普攻距离", true));
                DrawMenu.Add("drawQ", new CheckBox(" 显示 Q", true));
                DrawMenu.Add("drawQext", new CheckBox(" 显示 额外Q ", true));
                DrawMenu.Add("drawW", new CheckBox(" 显示 W", true));
                DrawMenu.Add("drawE", new CheckBox(" 显示 E", true));
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
                if (DrawMenu["drawQext"].Cast<CheckBox>().CurrentValue)
                {
                    if (Q.IsReady() && Q.IsLearned)
                    {
                        Circle.Draw(Color.White, Q1.Range, Player.Instance.Position);
                    }
                }
                if (DrawMenu["drawW"].Cast<CheckBox>().CurrentValue)
                {
                    if (W.IsReady() && W.IsLearned)
                    {
                        Circle.Draw(Color.White, W.Range, Player.Instance.Position);
                    }
                }
                if (DrawMenu["drawE"].Cast<CheckBox>().CurrentValue)
                {
                    if (E.IsReady() && E.IsLearned)
                    {
                        Circle.Draw(Color.White, E.Range, Player.Instance.Position);
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
                    Circle.Draw(Color.LightGreen, 560, Player.Instance.Position);
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
                Common.KillSteal();

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

                    //Common.LaneClear();

                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                {

                    //Common.JungleClear();
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
                {
                    //Common.LastHit();

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
            Common.Skinhack();
            if (lastTarget != null)
            {
                if (lastTarget.IsVisible)
                {
                    predictedPos = Prediction.Position.PredictUnitPosition(lastTarget, 300).To3D();
                    lastSeen = Game.Time;
                }
                if (lastTarget.Distance(Player.Instance) > 700)
                {
                    lastTarget = null;
                }
            }
        }

        private static void Player_OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender != Player.Instance)
                return;
            if (args.Target is AIHeroClient)
                lastTarget = (AIHeroClient)args.Target;
            else
                lastTarget = null;
        }

        public static void OnCastSpell(GameObject sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsDead || !sender.IsMe) return;
            if (args.SData.IsAutoAttack())
            {
                PassiveUp = false;
            }
            switch (args.Slot)
            {
                case SpellSlot.Q:
                case SpellSlot.W:
                    Orbwalker.ResetAutoAttack();
                    break;
            }
        }

        public static void OnProcessSpellCast(GameObject sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsDead || !sender.IsMe) return;
            {
                switch (args.Slot)
                {
                    case SpellSlot.Q:
                    case SpellSlot.W:
                    case SpellSlot.R:
                        PassiveUp = true;
                        break;
                    case SpellSlot.E:
                        PassiveUp = true;
                        Orbwalker.ResetAutoAttack();
                        break;
                }
            }
        }

    }
}
