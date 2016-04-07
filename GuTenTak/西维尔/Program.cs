using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using GuTenTak.Sivir;
using SharpDX;
using EloBuddy.SDK.Constants;
using System.Collections.Generic;

namespace GuTenTak.Sivir
{
    internal class Program
    {
        public const string ChampionName = "Sivir";
        public static Menu Menu, ModesMenu1, ModesMenu2, ModesMenu3, ModesMenu4, DrawMenu;
        public static int SkinBase;
        private static HashSet<string> DB { get; set; }
        public static Item Youmuu = new Item(ItemId.Youmuus_Ghostblade);
        public static Item Botrk = new Item(ItemId.Blade_of_the_Ruined_King);
        public static Item Cutlass = new Item(ItemId.Bilgewater_Cutlass);
        public static Item Qss = new Item(ItemId.Quicksilver_Sash);
        public static Item Simitar = new Item(ItemId.Mercurial_Scimitar);
        public static Item hextech = new Item(ItemId.Hextech_Gunblade, 700);

        private static readonly Dictionary<float, float>[] IncDamage = new Dictionary<float, float>[]
{
            new Dictionary<float, float>(), new Dictionary<float, float>(), new Dictionary<float, float>(), new Dictionary<float, float>(), new Dictionary<float, float>()
};
        private static readonly Dictionary<float, float>[] InstDamage = new Dictionary<float, float>[]
        {
            new Dictionary<float, float>(), new Dictionary<float, float>(), new Dictionary<float, float>(), new Dictionary<float, float>(), new Dictionary<float, float>()
        };
        public static List<MissileClient> blockThese = new List<MissileClient>();
        public static int me = int.MaxValue;
        public static bool castOnMe = false;

        public static float getIncomingDamageForI(int i)
        {
            return IncDamage[i].Sum(e => e.Value) + InstDamage[i].Sum(e => e.Value);
        }


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

        public static Spell.Skillshot Q;
        public static Spell.Active W;
        public static Spell.Active E;
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
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Game.OnTick += OnTick;
            Orbwalker.OnPostAttack += Common.WLogic;
            Orbwalker.OnPostAttack += Common.LJClear;
            SkinBase = Player.Instance.SkinId;
            try
            {
                if (ChampionName != PlayerInstance.BaseSkinName)
                {
                    return;
                }

                Q = new Spell.Skillshot(SpellSlot.Q, 1250, SkillShotType.Linear, (int)0.25, 1030, 90)
                {
                    AllowedCollisionCount = int.MaxValue
                };
                W = new Spell.Active(SpellSlot.W);
                E = new Spell.Active(SpellSlot.E);



                Bootstrap.Init(null);
                Chat.Print("GuTenTak Addon Loading Success", Color.Green);


                Menu = MainMenu.AddMenu("GuTenTak 西维尔", "Sivir");
                Menu.AddSeparator();
                Menu.AddLabel("CH汉化-GuTenTak 战争女神脚本");

                var Enemies = EntityManager.Heroes.Enemies.Where(a => !a.IsMe).OrderBy(a => a.BaseSkinName);
                ModesMenu1 = Menu.AddSubMenu("菜单", "Modes1Sivir");
                ModesMenu1.AddSeparator();
                ModesMenu1.AddLabel("连招设置");
                ModesMenu1.Add("ComboQ", new CheckBox("连招Q", true));
                ModesMenu1.Add("ComboW", new CheckBox("连招W", true));
                ModesMenu1.AddSeparator();
                ModesMenu1.AddLabel("骚扰设置");
                ModesMenu1.Add("HarassQ", new CheckBox("骚扰Q", true));
                ModesMenu1.Add("HarassMana", new Slider("Q蓝量使用 %", 60));
                ModesMenu1.Add("HarassW", new CheckBox("骚扰W", true));
                ModesMenu1.Add("ManaHW", new Slider("W蓝量使用 %", 60));
                ModesMenu1.AddSeparator();
                ModesMenu1.AddLabel("抢头设置");
                ModesMenu1.Add("KS", new CheckBox("开启抢头", true));
                ModesMenu1.Add("KQ", new CheckBox("Q抢头", true));

                ModesMenu2 = Menu.AddSubMenu("尾兵", "Modes2Sivir");
                ModesMenu2.AddLabel("清线设置");
                ModesMenu2.Add("MinionLC", new Slider("清线最低小兵数量", 3, 1, 5));
                ModesMenu2.AddSeparator();
                ModesMenu2.Add("FarmQ", new CheckBox("清线Q", true));
                ModesMenu2.Add("ManaLQ", new Slider("Q蓝量使用 %", 40));
                ModesMenu2.Add("FarmW", new CheckBox("清线W", true));
                ModesMenu2.Add("ManaLW", new Slider("W蓝量使用 %", 40));
                ModesMenu2.AddSeparator();
                ModesMenu2.AddLabel("清野设置");
                ModesMenu2.Add("JungleQ", new CheckBox("清野Q", true));
                ModesMenu2.Add("ManaJQ", new Slider("Q蓝量使用 %", 40));
                ModesMenu2.Add("JungleW", new CheckBox("清野W", true));
                ModesMenu2.Add("ManaJW", new Slider("W蓝量使用 %", 40));

                ModesMenu3 = Menu.AddSubMenu("杂项", "Modes3Sivir");
                ModesMenu3.Add("SpellShield", new CheckBox("自动E", true));
                ModesMenu3.Add("GoldCard", new CheckBox("卡牌" + " 黄牌 ", true));
                ModesMenu3.Add("RedCard", new CheckBox("卡牌" + " 红牌 ", false));

                foreach (var enemy in EntityManager.Heroes.Enemies.Where(a => a.Team != Player.Instance.Team))
                {
                    foreach (
                        var spell in
                            enemy.Spellbook.Spells.Where(
                                a =>
                                    a.Slot == SpellSlot.Q || a.Slot == SpellSlot.W || a.Slot == SpellSlot.E ||
                                    a.Slot == SpellSlot.R))
                    {
                        if (spell.SData.TargettingType != SpellDataTargetType.Self && spell.SData.TargettingType != SpellDataTargetType.SelfAndUnit)
                        {
                            if (spell.Slot == SpellSlot.Q)
                            {
                                if (spell.SData.TargettingType == SpellDataTargetType.Unit)
                                    ModesMenu3.Add(spell.SData.Name,
                                    new CheckBox(enemy.ChampionName + " Q ", false));
                                //new CheckBox(enemy.ChampionName + " Q " + spell.Name, false));
                            }
                            else if (spell.Slot == SpellSlot.W)
                            {
                                if (spell.SData.TargettingType == SpellDataTargetType.Unit)
                                    ModesMenu3.Add(spell.SData.Name,
                                    new CheckBox(enemy.ChampionName + " W ", false));
                            }
                            else if (spell.Slot == SpellSlot.E)
                            {
                                if (spell.SData.TargettingType == SpellDataTargetType.Unit)
                                    ModesMenu3.Add(spell.SData.Name,
                                    new CheckBox(enemy.ChampionName + " E ", false));
                                else if (spell.Slot == SpellSlot.R)
                                {
                                    if (spell.SData.TargettingType == SpellDataTargetType.Unit)
                                        ModesMenu3.Add(spell.SData.Name,
                                        new CheckBox(enemy.ChampionName + " R ", false)); ;
                                }
                            }
                        }
                    }
                }

                /*
                foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        var spell = enemy.Spellbook.Spells[i];
                        if (spell.SData.TargettingType != SpellDataTargetType.Self && spell.SData.TargettingType != SpellDataTargetType.SelfAndUnit)
                        {
                            if (spell.SData.TargettingType == SpellDataTargetType.Unit)
                                ModesMenu3.Add(spell.SData.Name,
                                new CheckBox(enemy.ChampionName + " - Target - " + spell.Name, false));
                        }
                    }
                }
                */
                ModesMenu3.AddSeparator();
                ModesMenu3.AddLabel("Item Usage on Combo");
                ModesMenu3.Add("useYoumuu", new CheckBox("Use Youmuu", true));
                ModesMenu3.Add("usehextech", new CheckBox("Use Hextech", true));
                ModesMenu3.Add("useBotrk", new CheckBox("Use Botrk & Cutlass", true));
                ModesMenu3.Add("useQss", new CheckBox("Use QuickSilver", true));
                ModesMenu3.Add("minHPBotrk", new Slider("Min health to use Botrk %", 80));
                ModesMenu3.Add("enemyMinHPBotrk", new Slider("Min enemy health to use Botrk %", 80));

                ModesMenu3.AddLabel("QSS Configs");
                ModesMenu3.Add("Qssmode", new ComboBox(" ", 0, "Auto", "Combo"));
                ModesMenu3.Add("Stun", new CheckBox("Stun", true));
                ModesMenu3.Add("Blind", new CheckBox("Blind", true));
                ModesMenu3.Add("Charm", new CheckBox("Charm", true));
                ModesMenu3.Add("Suppression", new CheckBox("Suppression", true));
                ModesMenu3.Add("Polymorph", new CheckBox("Polymorph", true));
                ModesMenu3.Add("Fear", new CheckBox("Fear", true));
                ModesMenu3.Add("Taunt", new CheckBox("Taunt", true));
                ModesMenu3.Add("Silence", new CheckBox("Silence", false));
                ModesMenu3.Add("QssDelay", new Slider("Use QSS Delay(ms)", 250, 0, 1000));

                ModesMenu3.AddLabel("QSS Ult Configs");
                ModesMenu3.Add("ZedUlt", new CheckBox("Zed R", true));
                ModesMenu3.Add("VladUlt", new CheckBox("Vladimir R", true));
                ModesMenu3.Add("FizzUlt", new CheckBox("Fizz R", true));
                ModesMenu3.Add("MordUlt", new CheckBox("Mordekaiser R", true));
                ModesMenu3.Add("PoppyUlt", new CheckBox("Poppy R", true));
                ModesMenu3.Add("QssUltDelay", new Slider("Use QSS Delay(ms) for Ult", 250, 0, 1000));

                ModesMenu3.AddLabel("Skin Hack");
                ModesMenu3.Add("skinhack", new CheckBox("Activate Skin hack", false));
                ModesMenu3.Add("skinId", new ComboBox("Skin Mode", 0, "Default", "1", "2", "3", "4", "5", "6", "7", "8"));

                DrawMenu = Menu.AddSubMenu("Draws", "DrawSivir");
                DrawMenu.Add("drawA", new CheckBox(" Draw Real AA", true));
                DrawMenu.Add("drawQ", new CheckBox(" Draw Q", true));
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
            Common.KillSteal();
            Common.Skinhack();
        }

        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe || sender.IsAlly)
                return;
            var SpellShield = ModesMenu3["SpellShield"].Cast<CheckBox>().CurrentValue;
            if (SpellShield)
            {
                var Gold = ModesMenu3["GoldCard"].Cast<CheckBox>().CurrentValue;
                var Red = ModesMenu3["RedCard"].Cast<CheckBox>().CurrentValue;
                if (Gold && sender.HasBuff("Pick A Card Gold") && E.IsReady() && (args.Target != null && args.Target.IsMe))
                {
                    E.Cast();
                }
                if (Red && sender.HasBuff("Pick A Card Red") && E.IsReady() && (args.Target != null && args.Target.IsMe))
                {
                    E.Cast();
                }

                if (!(args.Target != null))
                    return;
                if ((args.Slot == SpellSlot.Q || args.Slot == SpellSlot.W || args.Slot == SpellSlot.E ||
               args.Slot == SpellSlot.R) && sender.IsEnemy && E.IsReady())
                {
                    if (args.SData.TargettingType == SpellDataTargetType.Unit ||
                        args.SData.TargettingType == SpellDataTargetType.SelfAndUnit ||
                        args.SData.TargettingType == SpellDataTargetType.Self)
                    {
                        if ((args.Target.NetworkId == Player.Instance.NetworkId && args.Time < 1.5 ||
                             args.End.Distance(Player.Instance.ServerPosition) <= Player.Instance.BoundingRadius * 3) &&
                            ModesMenu3[args.SData.Name].Cast<CheckBox>().CurrentValue)
                        {
                            E.Cast();
                        }
                    }
                    else if (args.SData.TargettingType == SpellDataTargetType.LocationAoe)
                    {
                        var castvector =
                            new Geometry.Polygon.Circle(args.End, args.SData.CastRadius).IsInside(
                                Player.Instance.ServerPosition);

                        if (castvector && ModesMenu3[args.SData.Name].Cast<CheckBox>().CurrentValue)
                        {
                            E.Cast();
                        }
                    }

                    else if (args.SData.TargettingType == SpellDataTargetType.Cone)
                    {
                        var castvector =
                            new Geometry.Polygon.Arc(args.Start, args.End, args.SData.CastConeAngle, args.SData.CastRange)
                                .IsInside(Player.Instance.ServerPosition);

                        if (castvector && ModesMenu3[args.SData.Name].Cast<CheckBox>().CurrentValue)
                        {
                            E.Cast();
                        }
                    }

                    else if (args.SData.TargettingType == SpellDataTargetType.SelfAoe)
                    {
                        var castvector =
                            new Geometry.Polygon.Circle(sender.ServerPosition, args.SData.CastRadius).IsInside(
                                Player.Instance.ServerPosition);

                        if (castvector && ModesMenu3[args.SData.Name].Cast<CheckBox>().CurrentValue)
                        {
                            E.Cast();
                        }
                    }
                    else
                    {
                        var castvector =
                            new Geometry.Polygon.Rectangle(args.Start, args.End, args.SData.LineWidth).IsInside(
                                Player.Instance.ServerPosition);

                        if (castvector && ModesMenu3[args.SData.Name].Cast<CheckBox>().CurrentValue)
                        {
                            E.Cast();
                        }
                    }
                }
            }
        }
    }
}
