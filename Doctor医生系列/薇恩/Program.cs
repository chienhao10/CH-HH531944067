using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;
using DrVayne.Common;


namespace DrVayne
{
    class Program
    {
        public static Spell2 E;
        public static Spell2 Q;
        public static Spell2 W;
        public static Spell2 R;
        public static Spell.Active Heal;
        public static string Author = "Doctor";
        public static string AddonName = "CH汉化-Dr薇恩";
        public static Menu menu;
        public static Orbwalking.Orbwalker orbwalker;
        private static BuffType[] buffs;
        private static AIHeroClient rengarLeap;
        public static Dictionary<string, Menu> SubMenu = new Dictionary<string, Menu>() { };
        public static AIHeroClient myHero { get { return ObjectManager.Player; } }
        public static float ManaPercent { get { return myHero.Mana / myHero.MaxMana * 100; } }
        public static float HealthPercent { get { return myHero.Health / myHero.MaxHealth * 100; } }
        private static List<GameObject> _traps;
        private static List<string> _trapNames = new List<string> { "teemo", "shroom", "trap", "mine", "ziggse_red" };
        public static List<GameObject> EnemyTraps
        {
            get { return _traps.FindAll(t => t.IsValid && t.IsEnemy); }
        }

        static void Main(string[] args)
        {
            if (Hacks.RenderWatermark)
            {
                Hacks.RenderWatermark = false;
            }

            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        public static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (myHero.Hero != Champion.Vayne) { return; }
            Chat.Print("<font color=\"#F20000\"><b>Doctor Vayne:</b></font> Loaded!");
            Chat.Print("Good Luck!");

            Q = new Spell2(SpellSlot.Q);
            W = new Spell2(SpellSlot.W);
            E = new Spell2(SpellSlot.E, 590f);
            E.SetTargetted(0.25f, 2200f);
            R = new Spell2(SpellSlot.R);

            var slot = myHero.GetSpellSlotFromName("summonerheal");
            if (slot != SpellSlot.Unknown)
            {
                Heal = new Spell.Active(slot, 600);
            }

            menu = MainMenu.AddMenu(AddonName, AddonName + " by " + Author + " v1.0");
            menu.AddLabel(AddonName + " Doctor Vayne " + Author);
            menu.AddLabel("Good Luck!");

            SubMenu["Combo"] = menu.AddSubMenu("连招", "Combo");
            SubMenu["Combo"].AddGroupLabel("Q 设置");
            SubMenu["Combo"].Add("Q", new CheckBox("使用 Q", true));
            SubMenu["Combo"].Add("Qult", new CheckBox("大招智能 Q", true));
            var Q1 = SubMenu["Combo"].Add("Qmode", new Slider("Q 模式", 0, 0, 1));
            var Q2 = new[] { "智能", "鼠标位置" };
            Q1.DisplayName = Q2[Q1.CurrentValue];

            Q1.OnValueChange +=
                delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs changeArgs)
                {
                    sender.DisplayName = Q2[changeArgs.NewValue];
                };
            SubMenu["Combo"].AddSeparator(10);
            SubMenu["Combo"].Add("E", new CheckBox("使用 E", true));
            SubMenu["Combo"].Add("ELast", new CheckBox("使用 E 击杀敌人", false));
            SubMenu["Combo"].Add("PushDistance", new Slider("E 推距离", 380, 300, 425));
            SubMenu["Combo"].Add("R", new CheckBox("使用 R", true));
            SubMenu["Combo"].Add("R2", new Slider("范围内敌人数量 X 时", 2, 0, 5));
            SubMenu["Combo"].Add("noaastealth", new CheckBox("隐身时不普攻", true));
            SubMenu["Combo"].Add("noaastealth2", new Slider("范围内敌人数量 X 时", 2, 0, 5));
            SubMenu["Combo"].AddSeparator(10);
            SubMenu["Combo"].AddGroupLabel("物品设置");
            SubMenu["Combo"].Add("useItems", new CheckBox("使用物品", true));
            SubMenu["Combo"].Add("myhp", new Slider("玩家生命低于X %时使用破败", 55, 0, 100));

            SubMenu["Harass"] = menu.AddSubMenu("骚扰", "Harass");
            SubMenu["Harass"].Add("Q", new CheckBox("使用 Q", true));
            SubMenu["Harass"].Add("E", new CheckBox("使用 E", true));

            SubMenu["LastHit"] = menu.AddSubMenu("尾兵", "LastHit");
            SubMenu["LastHit"].Add("Q", new CheckBox("使用 Q", true));
            SubMenu["LastHit"].Add("LastHitMana", new Slider("蓝量大于", 80, 0, 100));

            SubMenu["LaneClear"] = menu.AddSubMenu("清线", "LaneClear");
            SubMenu["LaneClear"].Add("Q", new CheckBox("使用 Q", true));
            SubMenu["LaneClear"].Add("LaneClearMana", new Slider("蓝量大于", 85, 0, 100));

            SubMenu["JungleClear"] = menu.AddSubMenu("清野", "JungleClear");
            SubMenu["JungleClear"].Add("E", new CheckBox("使用 E", true));
            SubMenu["JungleClear"].Add("JungleClearMana", new Slider("蓝量大于", 50, 0, 100));


            SubMenu["Misc"] = menu.AddSubMenu("杂项", "Misc");
            SubMenu["Misc"].AddGroupLabel("防突进/技能打断设置");
            SubMenu["Misc"].Add("UseEInterrupt", new CheckBox("使用E防突进/技能打断", true));
            SubMenu["Misc"].Add("AntiGapQ", new CheckBox("使用Q造成间距", true));
            SubMenu["Misc"].AddSeparator(10);
            foreach (var hero in EntityManager.Heroes.Enemies.Where(x => x.IsEnemy))
            {
                SubMenu["Misc"].Add(hero.ChampionName, new CheckBox("对英雄防突进/技能打断 " + hero.ChampionName, true));
            }
            SubMenu["Misc"].AddSeparator(10);
            foreach (var enemy in EntityManager.Heroes.Enemies.Where(x => x.IsEnemy))
            {
                SubMenu["Misc"].Add(enemy.ChampionName + "E", new CheckBox("对英雄使用E " + enemy.ChampionName, true));
            }
            SubMenu["Misc"].AddSeparator(10);
            SubMenu["Misc"].AddGroupLabel("优先攻击W目标");
            SubMenu["Misc"].Add("focusW", new CheckBox("优先攻击W目标触发3W", true));

            SubMenu["Misc"].Add("waypoint", new CheckBox("显示目标行径", true));

            SubMenu["Misc"].AddSeparator(10);
            SubMenu["Misc"].AddGroupLabel("治疗设置");
            SubMenu["Misc"].Add("heal", new CheckBox("使用治疗", true));
            SubMenu["Misc"].Add("hp", new Slider("生命低于", 25, 0, 100));

            SubMenu["QSS"] = menu.AddSubMenu("自动解控", "QSS");
            SubMenu["QSS"].AddGroupLabel("自动水银/米凯尔");
            SubMenu["QSS"].Add("use", new KeyBind("使用水银/米凯尔", false, KeyBind.BindTypes.PressToggle, 'K'));
            SubMenu["QSS"].Add("delay", new Slider("延迟施放设置", 1000, 0, 2000));

            buffs = new[]
            {
                BuffType.Blind, BuffType.Charm, BuffType.CombatDehancer, BuffType.Fear, BuffType.Flee, BuffType.Knockback,
                BuffType.Knockup, BuffType.Polymorph, BuffType.Silence, BuffType.Sleep, BuffType.Snare, BuffType.Stun,
                BuffType.Suppression, BuffType.Taunt, BuffType.Poison
            };

            for (int i = 0; i < buffs.Length; i++)
            {
                SubMenu["QSS"].Add(buffs[i].ToString(), new CheckBox(buffs[i].ToString(), true));
            }

            orbwalker = new Orbwalking.Orbwalker(menu);
            TargetSelector2.Initialize();
            Spell2.Initialize();
            Prediction2.Initialize();

            _traps = new List<GameObject>();

            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Game.OnUpdate += Game_OnUpdate;
            Obj_AI_Base.OnSpellCast += Obj_AI_Base_OnSpellCast;
            Obj_AI_Base.OnCreate += Obj_AI_Base_OnCreate;
            Obj_AI_Base.OnDelete += Obj_AI_Base_OnDelete;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Drawing.OnDraw += Drawing_OnDraw;
            Player.OnIssueOrder += Player_OnIssueOrder;
        }
        private static void Player_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (sender.IsMe
                 && (args.Order == GameObjectOrder.AttackUnit || args.Order == GameObjectOrder.AttackTo)
                 && (SubMenu["Combo"]["noaastealth"].Cast<CheckBox>().CurrentValue && ObjectManager.Player.CountEnemiesInRange(1000f) > SubMenu["Combo"]["noaastealth2"].Cast<Slider>().CurrentValue)
                 && UltActive() || myHero.HasBuffOfType(BuffType.Invisibility)
                 && orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                args.Process = false;
            }
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            if(SubMenu["Misc"]["waypoint"].Cast<CheckBox>().CurrentValue)
            {
                foreach (var e in EntityManager.Heroes.Enemies.Where(en => en.IsVisible && !en.IsDead && en.Distance(myHero) < 2500))
                {
                    var ip = Drawing.WorldToScreen(e.Position); //start pos

                    var wp = Utility2.GetWaypoints(e);
                    var c = wp.Count - 1;
                    if (wp.Count() <= 1) break;

                    var w = Drawing.WorldToScreen(wp[c].To3D()); //endpos

                    Drawing.DrawLine(ip.X, ip.Y, w.X, w.Y, 2, System.Drawing.Color.Red);
                }
            }
        }

        private static void Obj_AI_Base_OnDelete(GameObject sender, EventArgs args)
        {
            _traps.RemoveAll(trap => trap.NetworkId == sender.NetworkId);
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var target = gapcloser.Sender;

            if (!target.IsValidTarget(E.Range))
            {
                return;
            }

            if (E.IsReady() && SubMenu["Misc"]["UseEInterrupt"].Cast<CheckBox>().CurrentValue && SubMenu["Misc"][target.ChampionName].Cast<CheckBox>().CurrentValue)
            {
                E.CastOnUnit(target);
            }
            if (SubMenu["Misc"]["AntiGapQ"].Cast<CheckBox>().CurrentValue && Q.IsReady())
            {
                if (myHero.Distance4(gapcloser.End) < 425)
                {
                    Tumble.Cast(myHero.Position.Extend2(gapcloser.End, -300));
                }
            }

        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (E.IsReady() && sender.IsValidTarget(E.Range) && SubMenu["Misc"]["UseEInterrupt"].Cast<CheckBox>().CurrentValue && SubMenu["Misc"][sender.ChampionName].Cast<CheckBox>().CurrentValue)
            {
                E.CastOnUnit(sender);
            }
        }

        private static void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!Q.IsReady()) return;
            if (unit.IsMe && target.IsValid<AIHeroClient>())
            {
                var tg = target as AIHeroClient;
                if (tg == null) return;
                var mode = SubMenu["Combo"]["Qmode"].Cast<Slider>().DisplayName;
                var tumblePosition = Game.CursorPos;
                switch (mode)
                {
                    case "Smart":
                        tumblePosition = tg.GetTumblePos();
                        break;
                    default:
                        tumblePosition = Game.CursorPos;
                        break;
                }
                Tumble.Cast(tumblePosition);
            }
            switch (orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Clear:
                    JungleClear();
                    LaneClear();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit();
                    break;
            }
        }     
        private static void Obj_AI_Base_OnCreate(GameObject sender, EventArgs args)
        {

            if (sender.Name == "Rengar_LeapSound.troy" && sender.IsEnemy)
            {
                foreach (AIHeroClient enemy in
                    ObjectManager.Get<AIHeroClient>()
                        .Where(hero => hero.IsValidTarget(1500) && hero.ChampionName == "Rengar"))
                {
                    rengarLeap = enemy;
                }
            }
            if (rengarLeap != null && SubMenu["Misc"]["UseEInterrupt"].Cast<CheckBox>().CurrentValue && myHero.Distance2(rengarLeap, true) < 1000 * 1000)
            {
                CastCondemn();
            }

            foreach (var trapName in _trapNames)
            {
                if (sender.Name.ToLower().Contains(trapName)) _traps.Add(sender);
            }
        }

        private static void Obj_AI_Base_OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;

            var enemy = TargetSelector2.GetTarget(E.Range, TargetSelector2.DamageType.Physical);

            if (enemy == null)
            {
                return;
            }

            if (enemy != null)
            {
                var mousePos = myHero.Position.Extend2(Game.CursorPos, Q.Range);
                if (SubMenu["Harass"]["Q"].Cast<CheckBox>().CurrentValue && orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed &&
                     enemy.IsValidTarget(myHero.GetAutoAttackRange()) && Q.IsReady())
                {
                    myHero.Spellbook.CastSpell(SpellSlot.Q, mousePos, true);
                    orbwalker.ForceTarget(enemy);
                }

                if (SubMenu["Harass"]["E"].Cast<CheckBox>().CurrentValue && E.IsReady() && !Q.IsReady()
                && orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed &&
                enemy.IsValidTarget(myHero.GetAutoAttackRange()))
                {
                    E.Cast(enemy);
                    orbwalker.ForceTarget(enemy);
                }
            }

            var LastHitE = myHero;

            foreach (var Etarget in EntityManager.Heroes.Enemies.Where(Etarget => Etarget.IsValidTarget(E.Range) && Etarget.Path.Count() < 2))
            {
                if (SubMenu["Combo"]["ELast"].Cast<CheckBox>().CurrentValue && E.IsReady() && myHero.CountEnemiesInRange2(600) <= 1)
                {
                    var dmgE = myHero.GetSpellDamage2(Etarget, SpellSlot.E);
                    if (dmgE > Etarget.Health || (WTarget(Etarget) == 2 && dmgE + Wdmg(Etarget) > Etarget.Health))
                    {
                        LastHitE = Etarget;

                    }
                }

                if (LastHitE != myHero)
                {
                    E.CastOnUnit(LastHitE);
                }
            }

            if (sender.Spellbook.Owner.IsMe)
            {
                if (args.Slot == SpellSlot.Q)
                {
                    if (Tumble.TumbleOrderPos != Vector3.Zero)
                    {
                        if (Tumble.TumbleOrderPos.IsDangerousPosition())
                        {
                            Tumble.TumbleOrderPos = Vector3.Zero;
                            args.Process = false;
                        }
                        else
                        {
                            Tumble.TumbleOrderPos = Vector3.Zero;
                        }
                    }
                }
            }
        }

        public static void Game_OnUpdate(EventArgs args)
        {
            switch (orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Clear:
                    JungleClear2();
                    break;
            }

            if (SubMenu["QSS"]["use"].Cast<KeyBind>().CurrentValue)
            {
                for (int i = 0; i < buffs.Length; i++)
                {
                    if (myHero.HasBuffOfType(buffs[i]) && SubMenu["QSS"][buffs[i].ToString()].Cast<CheckBox>().CurrentValue && myHero.CountEnemiesInRange(800) > 0)
                    {
                        var delay = SubMenu["QSS"]["delay"].Cast<Slider>().CurrentValue;
                        if (Item.CanUseItem(3140))
                        {
                            Core.DelayAction(() => { Item.UseItem(3140); }, delay);
                        }
                        else if (Item.CanUseItem(3139))
                        {
                            Core.DelayAction(() => { Item.UseItem(3139); }, delay);
                        }
                    }
                }
            }


            if (SubMenu["Misc"]["focusW"].Cast<CheckBox>().CurrentValue)
            {
                if (FocusWTarget == null && orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo ||
                    orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Mixed)
                {
                    return;
                }
                if (FocusWTarget.IsValidTarget(myHero.GetAutoAttackRange()) && !FocusWTarget.IsDead && orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo ||
                    orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                {
                    TargetSelector2.GetPriority(FocusWTarget);
                    Console.WriteLine("Focus W");
                }
                else
                {
                    TargetSelector2.GetPriority(
                        TargetSelector2.GetTarget(myHero.AttackRange, TargetSelector2.DamageType.Physical));
                }
            }

            AIHeroClient bestEnemy = null;
            foreach (var target in EntityManager.Heroes.Enemies.Where(target => target.IsValidTarget(E.Range)))
            {
                if (target == null)
                {
                    return;
                }

                if (bestEnemy == null)
                    bestEnemy = target;

                else if (myHero.Distance4(target.Position) < myHero.Distance4(bestEnemy.Position))
                    bestEnemy = target;

                if (SubMenu["Combo"]["castE"].Cast<KeyBind>().CurrentValue && bestEnemy != null)
                {
                    E.CastOnUnit(bestEnemy);
                }
            }
        }

        public static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;
            if (args.SData.Name.ToLower().Contains("vaynetumble"))
            {
                Core.DelayAction(Orbwalking.ResetAutoAttackTimer, 250);
            }

            if (!sender.IsMe) return;
            var mousePos = myHero.Position.Extend2(Game.CursorPos, Q.Range);
            if (args.SData.Name.ToLower().Contains("attack"))
            {
                Core.DelayAction(Orbwalking.ResetAutoAttackTimer, 250);
            }

            if (sender is AIHeroClient)
            {
                var pant = (AIHeroClient)sender;
                if (pant.IsValidTarget(myHero.GetAutoAttackRange()) && pant.ChampionName == "Pantheon" && pant.GetSpellSlotFromName(args.SData.Name) == SpellSlot.W)
                {
                    if (SubMenu["Misc"]["UseEInterrupt"].Cast<CheckBox>().CurrentValue && args.Target.IsMe)
                    {
                        if (pant.IsValidTarget(E.Range))
                        {
                            E.CastOnUnit(pant);
                        }
                    }
                }
            }
            if (args.SData.Name.ToLower() == "zedult" && args.Target.IsMe)
            {
                if (Item.CanUseItem(3140))
                {
                    Core.DelayAction(() => { Item.UseItem(3140); }, 1000);
                }
                else if (Item.CanUseItem(3139))
                {
                    Core.DelayAction(() => { Item.UseItem(3139); }, 1000);
                }
            }
        }

        private static void Combo()
        {
            if (Heal != null && SubMenu["Misc"]["heal"].Cast<CheckBox>().CurrentValue && Heal.IsReady() && HealthPercent <= SubMenu["Misc"]["hp"].Cast<Slider>().CurrentValue
                 && myHero.CountEnemiesInRange2(600) > 0 && Heal.IsReady())
            {
                Heal.Cast();
                Console.WriteLine("heal ");
            }

            var target = TargetSelector2.GetTarget(E.Range, TargetSelector2.DamageType.Physical);
            orbwalker.ForceTarget(target);

            if (!target.IsValidTarget(E.Range))
            {
                return;
            }
            if (SubMenu["Combo"]["E"].Cast<CheckBox>().CurrentValue && target != null && target.IsValidTarget(E.Range) && E.IsReady() &&
                SubMenu["Misc"][target.ChampionName + "E"].Cast<CheckBox>().CurrentValue)
            {
                Condemn();
                Console.WriteLine(" E");
            }
            if (SubMenu["Combo"]["R"].Cast<CheckBox>().CurrentValue && myHero.CountEnemiesInRange2(600f) >= (SubMenu["Combo"]["R2"].Cast<Slider>().CurrentValue) && R.IsReady())
            {
                R.Cast();
                Console.WriteLine("R");
            }
            var mousePos = myHero.Position.Extend2(Game.CursorPos, Q.Range);

            if ((SubMenu["Combo"]["Qult"].Cast<CheckBox>().CurrentValue && Q.IsReady() && myHero.HasBuff("vayneinquisition") && myHero.CountEnemiesInRange2(1500) > 0 && myHero.CountEnemiesInRange2(670) != 1))
            {
                myHero.Spellbook.CastSpell(SpellSlot.Q, mousePos, true);
                Console.WriteLine(" RQ");
            }
            if (SubMenu["Combo"]["useItems"].Cast<CheckBox>().CurrentValue)
            {
                if (Item.HasItem((int)ItemId.Blade_of_the_Ruined_King, myHero) && Item.CanUseItem((int)ItemId.Blade_of_the_Ruined_King)
                    && HealthPercent <= SubMenu["Combo"]["myhp"].Cast<Slider>().CurrentValue)
                {
                    Item.UseItem((int)ItemId.Blade_of_the_Ruined_King, target);
                }
                if (Item.HasItem((int)ItemId.Bilgewater_Cutlass, myHero) && Item.CanUseItem((int)ItemId.Bilgewater_Cutlass)
                   && target.IsValidTarget(myHero.GetAutoAttackRange()))
                {
                    Item.UseItem((int)ItemId.Bilgewater_Cutlass, target);
                }
                if (Item.HasItem((int)ItemId.Youmuus_Ghostblade, myHero) && Item.CanUseItem((int)ItemId.Youmuus_Ghostblade)
                   && myHero.Distance4(target.Position) <= myHero.GetAutoAttackRange())
                {
                    Item.UseItem((int)ItemId.Youmuus_Ghostblade);
                }
            }
        }

        private static void LastHit()
        {
            if (Q.IsReady() && SubMenu["LastHit"]["Q"].Cast<CheckBox>().CurrentValue && ManaPercent >= SubMenu["LastHit"]["LastHitMana"].Cast<Slider>().CurrentValue)
            {
                var Minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, myHero.Position, myHero.GetAutoAttackRange(), true);
                foreach (var minions in
                    Minions.Where(
                        minions => minions.Health < ObjectManager.Player.GetSpellDamage(minions, SpellSlot.Q)))
                {
                    if (minions != null && minions.IsValidTarget(E.Range))
                    {
                        Q.Cast(myHero.GetTumblePos());
                        orbwalker.ForceTarget(minions);
                        Console.WriteLine("lasthit Q");
                    }
                }
            }
        }

        private static void LaneClear()
        {
            if (Q.IsReady() && SubMenu["LaneClear"]["Q"].Cast<CheckBox>().CurrentValue && ManaPercent >= SubMenu["LaneClear"]["LaneClearMana"].Cast<Slider>().CurrentValue)
            {
                var Minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, myHero.Position, Q.Range, true);
                foreach (var minions in
                    Minions.Where(
                        minions => minions.Health < ObjectManager.Player.GetSpellDamage(minions, SpellSlot.Q)))
                {
                    if (minions != null && minions.IsValidTarget(E.Range) && minions.IsVisible)
                    {
                        Q.Cast(myHero.GetTumblePos());
                        orbwalker.ForceTarget(minions);
                        Console.WriteLine("laneclear Q");
                    }
                }
            }
        }

        private static void JungleClear()
        {
            Obj_AI_Base jungleMobs = EntityManager.MinionsAndMonsters.GetJungleMonsters(myHero.Position, Q.Range, true).FirstOrDefault();
            {
                if (SubMenu["JungleClear"]["Q"].Cast<CheckBox>().CurrentValue && Q.IsReady() && jungleMobs != null && jungleMobs.IsValidTarget(Q.Range) && ManaPercent >= SubMenu["JungleClear"]["JungleClearMana"].Cast<Slider>().CurrentValue)
                {
                    Q.Cast(myHero.GetTumblePos());
                    Console.WriteLine("jungle Q");
                }
            }
        }
        private static void JungleClear2()
        {
            Obj_AI_Base jungleMobs = EntityManager.MinionsAndMonsters.GetJungleMonsters(myHero.Position, Q.Range, true).FirstOrDefault();
            {
                if (SubMenu["JungleClear"]["E"].Cast<CheckBox>().CurrentValue && E.IsReady() && jungleMobs != null && jungleMobs.IsValidTarget(E.Range) && ManaPercent >= SubMenu["JungleClear"]["JungleClearMana"].Cast<Slider>().CurrentValue)
                {
                    if (jungleMobs.BaseSkinName == "SRU_Razorbeak" || jungleMobs.BaseSkinName == "SRU_Red" ||
                    jungleMobs.BaseSkinName == "SRU_Blue" ||
                    jungleMobs.BaseSkinName == "SRU_Krug" || jungleMobs.BaseSkinName == "SRU_Gromp" ||
                    jungleMobs.BaseSkinName == "Sru_Crab")
                    {
                        var pushDistance = 425;
                        var targetPosition = E.GetPrediction(jungleMobs, false, -1, null).UnitPosition;
                        var pushDirection = (targetPosition - ObjectManager.Player.ServerPosition).Normalized2();
                        float checkDistance = pushDistance / 40f;
                        for (int i = 0; i < 40; i++)
                        {
                            Vector3 finalPosition = targetPosition + (pushDirection * checkDistance * i);
                            var collFlags = NavMesh.GetCollisionFlags(finalPosition);
                            if (collFlags.HasFlag(CollisionFlags.Wall) || collFlags.HasFlag(CollisionFlags.Building))
                            {
                                E.Cast(jungleMobs);
                                orbwalker.ForceTarget(jungleMobs);
                                Console.WriteLine("jungle E");
                            }
                        }
                    }
                }
            }
        }

        public static bool UltActive()
        {
            return (myHero.HasBuff("vaynetumblefade") && !UnderTower(myHero.Position));
        }
        public static bool TumbleActive()
        {
            return myHero.Buffs.Any(b => b.Name.ToLower().Contains("vaynetumblebonus"));
        }

        public static bool UnderTower(Vector3 pos)
        {
            return
                ObjectManager.Get<Obj_AI_Turret>()
                    .Any(i => i.IsEnemy && !i.IsDead && i.Distance4(pos) < 850 + ObjectManager.Player.BoundingRadius);
        }

        public static AIHeroClient FocusWTarget
        {
            get
            {
                return ObjectManager.Get<AIHeroClient>().Where(enemy => !enemy.IsDead && enemy.IsValidTarget((Q.IsReady() ? Q.Range : 0) + myHero.AttackRange + 300))
                       .FirstOrDefault(
                           enemy => enemy.Buffs.Any(buff => buff.Name == "vaynesilvereddebuff" && buff.Count > 0));
            }
        }

        public static float Wdmg(Obj_AI_Base target)
        {
            var dmg = (W.Level * 10 + 10) + ((0.03 + (W.Level * 0.01)) * target.MaxHealth);
            return (float)dmg;

        }

        public static int WTarget(Obj_AI_Base target)
        {
            foreach (var buff in target.Buffs)
            {
                if (buff.Name == "vaynesilvereddebuff")
                    return buff.Count;
            }
            return -1;
        }

        private static void CastCondemn()
        {
            if (rengarLeap.ChampionName == "Rengar")
            {
                if (rengarLeap.IsValidTarget(E.Range) && E.IsReady() &&
                    rengarLeap.Distance2(myHero) <= E.Range)
                {
                    E.CastOnUnit(rengarLeap);
                    Console.WriteLine("E Rengar");
                }
            }
        }
        public static void Condemn()
        {
            if (!E.IsReady()) return;
            if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && SubMenu["Combo"]["E"].Cast<CheckBox>().CurrentValue)
                foreach (var hero in from hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsValidTarget(425f))
                                     let prediction = E.GetPrediction(hero)
                                     where NavMesh.GetCollisionFlags(
                                         prediction.UnitPosition.To2D2()
                                             .Extend2(ObjectManager.Player.ServerPosition.To2D2(),
                                                 -SubMenu["Combo"]["PushDistance"].Cast<Slider>().CurrentValue)
                                             .To3D())
                                         .HasFlag(CollisionFlags.Wall) || NavMesh.GetCollisionFlags(
                                             prediction.UnitPosition.To2D2()
                                                 .Extend2(ObjectManager.Player.ServerPosition.To2D2(),
                                                     -(SubMenu["Combo"]["PushDistance"].Cast<Slider>().CurrentValue / 2))
                                                 .To3D())
                                             .HasFlag(CollisionFlags.Wall)
                                     select hero)
                {
                    E.CastOnUnit(hero);
                }
        }
    }
}