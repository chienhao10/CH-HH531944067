using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using Color = System.Drawing.Color;

namespace LeeSin
{
    public static class Program
    {

        private enum InsecComboStepSelect
        {
            None,

            Qgapclose,

            Wgapclose,

            Pressr
        };

        public static Spell.Skillshot Q;
        public static Spell.Targeted W, R;
        public static Spell.Active E, Q2, W2;

        public static bool EState
        {
            get
            {
                return E.Name == "BlindMonkEOne";
            }
        }

        private enum WCastStage
        {
            First,

            Second,

            Cooldown
        }

        public static bool QState
        {
            get
            {
                return Q.Name == "BlindMonkQOne";
            }
        }

        public static bool CheckQ = true;

        public static Vector3 InsecClickPos;

        public static Vector2 InsecLinePos;

        public static Vector2 JumpPos;

        public static int LastQ, LastQ2, LastW, LastW2, LastE, LastE2, LastR, LastWard, LastSpell, PassiveStacks;

        private static readonly bool castWardAgain = true;

        private static readonly int[] SmiteBlue = { 3706, 1403, 1402, 1401, 1400 };

        private static readonly int[] SmiteRed = { 3715, 1415, 1414, 1413, 1412 };

        private static readonly string[] SpellNames =
            {
                "BlindMonkQOne", "BlindMonkWOne", "BlindMonkEOne",
                "blindmonkwtwo", "blindmonkqtwo", "blindmonketwo",
                "BlindMonkRKick"
            };

        private static bool castQAgain;

        private static int clickCount;

        private static float doubleClickReset;

        private static Spell.Skillshot flashSlot;

        private static Spell.Targeted ignite, smiteSlot;

        private static InsecComboStepSelect insecComboStep;

        private static Vector3 insecPos;

        private static bool isNullInsecPos = true;

        private static bool lastClickBool;

        private static Vector3 lastClickPos;

        private static float lastPlaced;

        private static Vector3 lastWardPos;

        private static Vector3 mouse = Game.CursorPos;

        private static float passiveTimer;

        private static float q2Timer;

        private static bool reCheckWard = true;

        //private static SpellSlot smiteSlot;

        private static bool waitingForQ2;

        private static float wcasttime;

        private static Menu Menu;

        private static AIHeroClient myHero
        {
            get { return Player.Instance; }
        }


        public static void Main()
        {
            Loading.OnLoadingComplete += OnLoad;
        }

        #region Menu Items
        public static bool ElLeeSinComboQ { get { return Menu["ElLeeSin.Combo.Q"].Cast<CheckBox>().CurrentValue; } }
        public static bool ElLeeSinComboQ2 { get { return Menu["ElLeeSin.Combo.Q2"].Cast<CheckBox>().CurrentValue; } }
        public static bool ElLeeSinComboW2 { get { return Menu["ElLeeSin.Combo.W2"].Cast<CheckBox>().CurrentValue; } }
        public static bool ElLeeSinComboE { get { return Menu["ElLeeSin.Combo.E"].Cast<CheckBox>().CurrentValue; } }
        public static bool ElLeeSinComboR { get { return Menu["ElLeeSin.Combo.R"].Cast<CheckBox>().CurrentValue; } }
        public static bool ElLeeSinComboKSR { get { return Menu["ElLeeSin.Combo.KS.R"].Cast<CheckBox>().CurrentValue; } }
        public static bool ElLeeSinComboNew { get { return Menu["ElLeeSin.Combo.New"].Cast<CheckBox>().CurrentValue; } }
        public static int ElLeeSinComboRCount { get { return Menu["ElLeeSin.Combo.R.Count"].Cast<Slider>().CurrentValue; } }
        public static int ElLeeSinComboPassiveStacks { get { return Menu["ElLeeSin.Combo.PassiveStacks"].Cast<Slider>().CurrentValue; } }
        public static bool ElLeeSinComboAAStacks { get { return Menu["ElLeeSin.Combo.AAStacks"].Cast<CheckBox>().CurrentValue; } }
        public static bool starCombo { get { return Menu["starCombo"].Cast<KeyBind>().CurrentValue; } }
        public static bool InsecEnabled { get { return Menu["InsecEnabled"].Cast<KeyBind>().CurrentValue; } }
        public static bool ElLeeSinInsecUseInstaFlash { get { return Menu["ElLeeSin.Insec.UseInstaFlash"].Cast<KeyBind>().CurrentValue; } }
        public static bool insecOrbwalk { get { return Menu["insecOrbwalk"].Cast<CheckBox>().CurrentValue; } }
        public static bool ElLeeSinInsecAlly { get { return Menu["ElLeeSin.Insec.Ally"].Cast<CheckBox>().CurrentValue; } }
        public static bool ElLeeSinInsecOriginalPos { get { return Menu["ElLeeSin.Insec.Original.Pos"].Cast<CheckBox>().CurrentValue; } }
        public static bool insecmouse { get { return Menu["insecmouse"].Cast<CheckBox>().CurrentValue; } }
        public static bool checkOthers1 { get { return Menu["checkOthers1"].Cast<CheckBox>().CurrentValue; } }
        public static bool ElLeeSinFlashInsec { get { return Menu["ElLeeSin.Flash.Insec"].Cast<CheckBox>().CurrentValue; } }
        public static bool ElLeeSinHarassQ1 { get { return Menu["ElLeeSin.Harass.Q1"].Cast<CheckBox>().CurrentValue; } }
        public static bool ElLeeSinHarassWardjump { get { return Menu["ElLeeSin.Harass.Wardjump"].Cast<CheckBox>().CurrentValue; } }
        public static bool ElLeeSinHarassE1 { get { return Menu["ElLeeSin.Harass.E1"].Cast<CheckBox>().CurrentValue; } }
        public static int ElLeeSinHarassPassiveStacks { get { return Menu["ElLeeSin.Harass.PassiveStacks"].Cast<Slider>().CurrentValue; } }
        public static bool ElLeeSinWardjump { get { return Menu["ElLeeSin.Wardjump"].Cast<KeyBind>().CurrentValue; } }
        public static bool ElLeeSinWardjumpMaxRange { get { return Menu["ElLeeSin.Wardjump.MaxRange"].Cast<CheckBox>().CurrentValue; } }
        public static bool ElLeeSinWardjumpMouse { get { return Menu["ElLeeSin.Wardjump.Mouse"].Cast<CheckBox>().CurrentValue; } }
        public static bool ElLeeSinWardjumpMinions { get { return Menu["ElLeeSin.Wardjump.Minions"].Cast<CheckBox>().CurrentValue; } }
        public static bool ElLeeSinWardjumpChampions { get { return Menu["ElLeeSin.Wardjump.Champions"].Cast<CheckBox>().CurrentValue; } }
        public static bool ElLeeSinComboW { get { return Menu["ElLeeSin.Combo.W"].Cast<CheckBox>().CurrentValue; } }
        public static bool ElLeeSinComboModeWW { get { return Menu["ElLeeSin.Combo.Mode.WW"].Cast<CheckBox>().CurrentValue; } }
        public static bool ElLeeSinIgniteKS { get { return Menu["ElLeeSin.Ignite.KS"].Cast<CheckBox>().CurrentValue; } }
        public static bool ElLeeSinSmiteQ { get { return Menu["ElLeeSin.Smite.Q"].Cast<CheckBox>().CurrentValue; } }
        public static int bonusRangeA { get { return Menu["bonusRangeA"].Cast<Slider>().CurrentValue; } }
        public static bool ElLeeSinLaneQ { get { return Menu["ElLeeSin.Lane.Q"].Cast<CheckBox>().CurrentValue; } }
        public static bool ElLeeSinLaneE { get { return Menu["ElLeeSin.Lane.E"].Cast<CheckBox>().CurrentValue; } }
        public static bool ElLeeSinJungleQ { get { return Menu["ElLeeSin.Jungle.Q"].Cast<CheckBox>().CurrentValue; } }
        public static bool ElLeeSinJungleW { get { return Menu["ElLeeSin.Jungle.W"].Cast<CheckBox>().CurrentValue; } }
        public static bool ElLeeSinJungleE { get { return Menu["ElLeeSin.Jungle.E"].Cast<CheckBox>().CurrentValue; } }
        public static bool junglePassive { get { return Menu["junglePassive"].Cast<CheckBox>().CurrentValue; } }

        #endregion

        private static void OnLoad(EventArgs args)
        {
            if (myHero.Hero != Champion.LeeSin)
            {
                return;
            }

            Menu = MainMenu.AddMenu("国足Lee Sin", "blindboy");
            Menu.AddLabel("Ported from El Lee Sin - Berb");
            Menu.AddLabel("原版为L#的EL李星，由CH汉化");

            Menu.AddSeparator();

            Menu.AddGroupLabel("连招");
            Menu.Add("ElLeeSin.Combo.Q", new CheckBox("使用 Q"));
            Menu.Add("ElLeeSin.Combo.Q2", new CheckBox("使用 Q2"));
            Menu.Add("ElLeeSin.Combo.W2", new CheckBox("使用 W"));
            Menu.Add("ElLeeSin.Combo.E", new CheckBox("使用 E"));
            Menu.Add("ElLeeSin.Combo.R", new CheckBox("使用 R"));
            Menu.Add("ElLeeSin.Combo.KS.R", new CheckBox("抢头 R"));

            Menu.Add("ElLeeSin.Combo.New", new CheckBox("踢中多数敌人:"));
            Menu.Add("ElLeeSin.Combo.R.Count", new Slider("R 命中敌人数量", 2, 2, 4));
            Menu.Add("ElLeeSin.Combo.PassiveStacks", new Slider("连招最低被动叠加层数", 1, 1, 2));
            Menu.Add("ElLeeSin.Combo.AAStacks", new CheckBox("等待被动", false));
            Menu.Add("starCombo", new KeyBind("使用明星连招", false, KeyBind.BindTypes.HoldActive, 'T'));
            Menu.AddSeparator();

            Menu.AddGroupLabel("花式国足特技");
            Menu.Add("InsecEnabled", new KeyBind("光速回旋踢 :", false, KeyBind.BindTypes.HoldActive, 'A'));
            Menu.Add("ElLeeSin.Insec.UseInstaFlash", new KeyBind("闪现 + R :", false, KeyBind.BindTypes.HoldActive, 'U'));
            Menu.Add("insecOrbwalk", new CheckBox("光速回旋踢 时走砍"));
            Menu.Add("ElLeeSin.Insec.Ally", new CheckBox("光速回旋踢 至同盟"));
            Menu.Add("ElLeeSin.Insec.Original.Pos", new CheckBox("光速回旋踢 至原本位置"));
            Menu.Add("insecmouse", new CheckBox("光速回旋踢 至鼠标", false));
            Menu.Add("ElLeeSin.Flash.Insec", new CheckBox("当无假眼时使用闪现光速回旋踢", false));
            Menu.Add("checkOthers1", new CheckBox("检测光速回旋踢目标", false));
            Menu.AddSeparator();

            Menu.AddGroupLabel("骚扰");
            Menu.Add("ElLeeSin.Harass.Q1", new CheckBox("使用Q"));
            Menu.Add("ElLeeSin.Harass.Wardjump", new CheckBox("使用W"));
            Menu.Add("ElLeeSin.Harass.E1", new CheckBox("使用E", false));
            Menu.Add("ElLeeSin.Harass.PassiveStacks", new Slider("最低被动叠加层数", 1, 1, 2));
            Menu.AddSeparator();

            Menu.AddGroupLabel("清线");
            Menu.Add("ElLeeSin.Lane.Q", new CheckBox("使用Q"));
            Menu.Add("ElLeeSin.Lane.E", new CheckBox("使用E"));

            Menu.AddSeparator();

            Menu.AddGroupLabel("清野");
            Menu.Add("ElLeeSin.Jungle.Q", new CheckBox("使用Q"));
            Menu.Add("ElLeeSin.Jungle.W", new CheckBox("使用W"));
            Menu.Add("ElLeeSin.Jungle.E", new CheckBox("使用E"));
            Menu.Add("junglePassive", new CheckBox("智能被动控制", false));

            Menu.AddSeparator();

            Menu.AddGroupLabel("跳眼");
            Menu.Add("ElLeeSin.Wardjump", new KeyBind("跳眼 :", false, KeyBind.BindTypes.HoldActive, 'G'));
            Menu.Add("ElLeeSin.Wardjump.MaxRange", new CheckBox("跳眼至最远距离", false));
            Menu.Add("ElLeeSin.Wardjump.Mouse", new CheckBox("跳至鼠标"));
            Menu.Add("ElLeeSin.Wardjump.Minions", new CheckBox("跳至小兵"));
            Menu.Add("ElLeeSin.Wardjump.Champions", new CheckBox("跳至英雄"));
            Menu.Add("ElLeeSin.Combo.W", new CheckBox("连招时跳眼", false));
            Menu.Add("ElLeeSin.Combo.Mode.WW", new CheckBox("平A不到时 - 跳眼", false));

            Menu.AddSeparator();

            Menu.AddGroupLabel("杂项");
            Menu.Add("ElLeeSin.Ignite.KS", new CheckBox("使用点燃"));
            Menu.Add("ElLeeSin.Smite.Q", new CheckBox("Q惩戒!", false));
            Menu.Add("bonusRangeA", new Slider("友军额外距离", 0, 0, 1000));
            Menu.AddSeparator();

            Q = new Spell.Skillshot(SpellSlot.Q, 1075, SkillShotType.Linear, 250, 1800, 60);
            Q2 = new Spell.Active(SpellSlot.Q, 1300);
            W = new Spell.Targeted(SpellSlot.W, 700);
            W2 = new Spell.Active(SpellSlot.W);
            E = new Spell.Active(SpellSlot.E, 330);
            R = new Spell.Targeted(SpellSlot.R, 375);


            var flash = Player.Spells.FirstOrDefault(o => o.SData.Name == "summonerflash");
            var ign = Player.Spells.FirstOrDefault(o => o.SData.Name == "summonerdot");
            var smite = Player.Instance.Spellbook.Spells.FirstOrDefault(spell => spell.Name.Contains("smite"));

            if (flash != null)
            {
                SpellSlot flSlot = EloBuddy.SDK.Extensions.GetSpellSlotFromName(myHero, "summonerflash");

                flashSlot = new Spell.Skillshot(flSlot, 425, SkillShotType.Linear);
            }

            if (ign != null)
            {
                SpellSlot igslot = EloBuddy.SDK.Extensions.GetSpellSlotFromName(myHero, "summonerdot");

                ignite = new Spell.Targeted(igslot, 600);
            }

            if (smite != null)
            {
                smiteSlot = new Spell.Targeted(smite.Slot, 500);
            }

            Game.OnTick += OnTick;

            Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;

            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;

            Game.OnWndProc += Game_OnWndProc;

            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
        }

        public static bool WState
        {
            get
            {
                return W.Name == "BlindMonkWOne";
            }
        }

        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg != (uint)WindowMessages.LeftButtonDown)
            {
                return;
            }

            var asec = EntityManager.Heroes.Enemies.Where(a => a.IsEnemy && a.Distance(Game.CursorPos) < 200 && a.IsValid && !a.IsDead);

            if (asec.Any())
            {
                return;
            }

            if (!lastClickBool || clickCount == 0)
            {
                clickCount++;
                lastClickPos = Game.CursorPos;
                lastClickBool = true;
                doubleClickReset = Environment.TickCount + 600;
                return;
            }

            if (lastClickBool && lastClickPos.Distance(Game.CursorPos) < 200)
            {
                clickCount++;
                lastClickBool = false;
            }
        }

        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (!(sender is Obj_GeneralParticleEmitter))
            {
                return;
            }
            if (sender.Name.Contains("blindMonk_Q_resonatingStrike") && waitingForQ2)
            {
                waitingForQ2 = false;
                q2Timer = Environment.TickCount + 800;
            }
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (Environment.TickCount < lastPlaced + 300)
            {
                var ward = (Obj_AI_Base)sender;
                if (ward.Name.ToLower().Contains("ward") && ward.Distance(lastWardPos) < 500 && E.IsReady())
                {
                    W.Cast(ward);
                }
            }
        }

        public static bool castR = false;

        private static void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            if (target.IsMe && PassiveStacks > 0)
            {
                PassiveStacks--;
            }
        }
        private static void OnTick(EventArgs args)
        {

            if (doubleClickReset <= Environment.TickCount && clickCount != 0)
            {
                doubleClickReset = float.MaxValue;
                clickCount = 0;
            }

            if (passiveTimer <= Environment.TickCount)
            {
                PassiveStacks = 0;
            }

            if (myHero.IsDead || MenuGUI.IsChatOpen || myHero.IsRecalling())
            {
                return;
            }

            if (TargetSelector.GetTarget(Q.Range, DamageType.Physical) == null)
            {
                insecComboStep = InsecComboStepSelect.None;
            }

            if (starCombo)
            {
                WardCombo();
            }

            if (ElLeeSinIgniteKS)
            {
                var newTarget = TargetSelector.GetTarget(600, DamageType.True);

                if (newTarget != null && ignite != null && ignite.IsReady() && ObjectManager.Player.GetSummonerSpellDamage(newTarget, DamageLibrary.SummonerSpells.Ignite) > newTarget.Health)
                {
                    ignite.Cast(newTarget);
                }
            }

            if (InsecEnabled)
            {
                if (insecOrbwalk)
                {
                    Orbwalk(Game.CursorPos);
                }

                var newTarget = TargetSelector.GetTarget(Q.Range, DamageType.Physical);

                if (newTarget != null && R.IsReady())
                {
                    InsecCombo(newTarget);
                }
            }
            else
            {
                isNullInsecPos = true;
            }

            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                insecComboStep = InsecComboStepSelect.None;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }
            else if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                AllClear();
                JungleClear();
            }
            else if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }

            if (ElLeeSinWardjump)
            {
                Orbwalk(Game.CursorPos);
                WardjumpToMouse();
            }

            if (ElLeeSinInsecUseInstaFlash)
            {
                Orbwalk(Game.CursorPos);

                var target = TargetSelector.GetTarget(flashSlot.Range - 25, DamageType.Physical);

                if (target == null)
                {
                    return;
                }

                if (!R.IsReady() && !flashSlot.IsReady())
                {
                    castR = false;
                    return;
                }

                if (GetInsecPos(target).Distance(myHero.Position) < 425 && ElLeeSinInsecUseInstaFlash && flashSlot.IsReady() && R.IsReady())
                {
                    flashSlot.Cast(GetInsecPos(target));
                    castR = true;
                    return;
                }

                if (R.IsReady() && !target.IsZombie && R.IsInRange(target) && castR)
                {
                    R.Cast(target);
                    castR = false;
                }
            }

            if (ElLeeSinComboNew)
            {
                float leeSinRKickDistance = 700;
                float leeSinRKickWidth = 100;
                var minREnemies = ElLeeSinComboRCount;
                foreach (var enemy in EntityManager.Heroes.Enemies)
                {
                    var startPos = enemy.ServerPosition;
                    var endPos = myHero.ServerPosition.Extend(startPos, myHero.Distance(enemy) + leeSinRKickDistance);
                    var rectangle = new Geometry.Polygon.Rectangle(startPos, endPos, leeSinRKickWidth);

                    if (EntityManager.Heroes.Enemies.Count(x => rectangle.IsInside(x)) >= minREnemies)
                    {
                        R.Cast(enemy);
                    }
                }
            }
        }

        private static void JungleClear()
        {
            var minion = EntityManager.MinionsAndMonsters.GetJungleMonsters(null, Q.Range).FirstOrDefault();

            if (minion == null)
            {
                return;
            }

            if (junglePassive)
            {
                if (PassiveStacks > 0 || LastSpell + 400 > Environment.TickCount)
                {
                    return;
                }
            }

            if (E.IsReady() && ElLeeSinJungleE)
            {
                if (EState && E.IsInRange(minion))
                {
                    E.Cast();
                    LastSpell = Environment.TickCount;
                }

                if (!EState && E.IsInRange(minion) && LastE + 400 < Environment.TickCount)
                {
                    E.Cast();
                    LastSpell = Environment.TickCount;
                }
            }

            if (Q.IsReady() && ElLeeSinJungleQ)
            {
                Q.Cast(minion);
                LastSpell = Environment.TickCount;

                foreach (var jungleMobs in ObjectManager.Get<Obj_AI_Minion>().Where(o => o.IsValidTarget(Program.Q.Range) && o.Team == GameObjectTeam.Neutral && o.IsVisible && !o.IsDead))
                {

                    if (EntityManager.MinionsAndMonsters.GetJungleMonsters(null, Q.Range).Any())
                    {
                        if (jungleMobs.HasQBuff())
                        {
                            Q2.Cast();
                            LastSpell = Environment.TickCount;
                        }
                    }
                }
            }

            if (W.IsReady() && ElLeeSinJungleW)
            {
                if (WState)
                {
                    W.Cast(myHero);
                    LastSpell = Environment.TickCount;
                }

                if (WState)
                {
                    return;
                }

                W2.Cast();
                LastSpell = Environment.TickCount;
                return;
            }
        }

        private static void AllClear()
        {
            var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, null, Q.Range).FirstOrDefault();

            if (minions == null)
            {
                return;
            }

            UseItems(minions);

            if (ElLeeSinLaneQ && !QState && Q.IsReady() && minions.HasQBuff() && (LastQ + 2700 < Environment.TickCount || myHero.GetSpellDamage(minions, SpellSlot.Q, DamageLibrary.SpellStages.SecondCast) > minions.Health || minions.Distance(myHero) > myHero.GetAutoAttackRange() + 50))
            {
                Q2.Cast();
            }

            if (Q.IsReady() && ElLeeSinLaneQ && LastQ + 200 < Environment.TickCount)
            {
                if (QState && minions.Distance(myHero) < Q.Range)
                {
                    Q.Cast(minions);
                }
            }

            if (ElLeeSinComboAAStacks && PassiveStacks > ElLeeSinComboPassiveStacks && myHero.GetAutoAttackRange() > myHero.Distance(minions))
            {
                return;
            }

            if (E.IsReady() && ElLeeSinLaneE)
            {
                if (EState && E.IsInRange(minions))
                {
                    LastE = Environment.TickCount;
                    E.Cast();
                    return;
                }

                if (!EState && E.IsInRange(minions) && LastE + 400 < Environment.TickCount)
                {
                    E.Cast();
                }
            }
        }

        private static void WardCombo()
        {
            var target = TargetSelector.GetTarget(1500, DamageType.Physical);

            Orbwalker.OrbwalkTo(Game.CursorPos);

            if (target == null)
            {
                return;
            }

            UseItems(target);

            if (target.HasQBuff())
            {
                if (castQAgain || target.HasBuffOfType(BuffType.Knockback) && !myHero.IsValidTarget(300) && !R.IsReady() || !target.IsValidTarget(myHero.GetAutoAttackRange()) && !R.IsReady())
                {
                    Q2.Cast();
                }
            }

            if (target.Distance(myHero) > R.Range && target.Distance(myHero) < R.Range + 580 && target.HasQBuff())
            {
                WardJump(target.Position, false);
            }

            if (E.IsReady() && EState && target.IsValidTarget(E.Range - 25))
            {
                E.Cast();
            }

            if (E.IsReady() && QState)
            {
                CastQ(target);
            }

            if (R.IsReady() && Q.IsReady() && target.HasQBuff())
            {
                R.Cast(target);
            }
        }

        private static void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {

            if (!sender.IsMe)
            {
                return;
            }

            if (SpellNames.Contains(args.SData.Name))
            {
                PassiveStacks = 2;
                passiveTimer = Environment.TickCount + 3000;
            }

            if (args.SData.Name == "BlindMonkQOne")
            {
                castQAgain = false;
                Core.DelayAction(delegate { castQAgain = true; }, 2900);
            }

            if (R.IsReady() && flashSlot.IsReady())
            {
                var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);

                if (target == null)
                {
                    return;
                }
            }

            if (args.SData.Name == "summonerflash" && insecComboStep != InsecComboStepSelect.None)
            {
                var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);

                insecComboStep = InsecComboStepSelect.Pressr;

                Core.DelayAction(delegate { R.Cast(target); }, 80);
            }

            if (args.SData.Name == "blindmonkqtwo")
            {
                waitingForQ2 = true;
                Core.DelayAction(delegate { waitingForQ2 = false; }, 3000);
            }
            if (args.SData.Name == "BlindMonkRKick")
            {
                insecComboStep = InsecComboStepSelect.None;
            }

            switch (args.SData.Name)
            {
                case "BlindMonkQOne":
                    LastQ = Environment.TickCount;
                    LastSpell = Environment.TickCount;
                    PassiveStacks = 2;
                    break;
                case "BlindMonkWOne":
                    LastW = Environment.TickCount;
                    LastSpell = Environment.TickCount;
                    PassiveStacks = 2;
                    break;
                case "BlindMonkEOne":
                    LastE = Environment.TickCount;
                    LastSpell = Environment.TickCount;
                    PassiveStacks = 2;
                    break;
                case "blindmonkqtwo":
                    LastQ2 = Environment.TickCount;
                    LastSpell = Environment.TickCount;
                    PassiveStacks = 2;
                    CheckQ = false;
                    break;
                case "blindmonkwtwo":
                    LastW2 = Environment.TickCount;
                    LastSpell = Environment.TickCount;
                    PassiveStacks = 2;
                    break;
                case "blindmonketwo":
                    LastQ = Environment.TickCount;
                    LastSpell = Environment.TickCount;
                    PassiveStacks = 2;
                    break;
                case "BlindMonkRKick":
                    LastR = Environment.TickCount;
                    LastSpell = Environment.TickCount;
                    PassiveStacks = 2;
                    break;
            }
        }

        private static Vector3 InterceptionPoint(List<AIHeroClient> heroes)
        {
            var result = new Vector3();
            foreach (var hero in heroes)
            {
                result += hero.Position;
            }
            result.X /= heroes.Count;
            result.Y /= heroes.Count;
            return result;
        }

        private static List<AIHeroClient> GetAllyInsec(List<AIHeroClient> heroes)
        {
            byte alliesAround = 0;
            var tempObject = new AIHeroClient();
            foreach (var hero in heroes)
            {
                var localTemp =
                    GetAllyHeroes(hero, 500 + bonusRangeA).Count;
                if (localTemp > alliesAround)
                {
                    tempObject = hero;
                    alliesAround = (byte)localTemp;
                }
            }
            return GetAllyHeroes(tempObject, 500 + bonusRangeA);
        }

        private static List<AIHeroClient> GetAllyHeroes(AIHeroClient position, int range)
        {
            return EntityManager.Heroes.Allies.Where(hero => hero.IsAlly && !hero.IsMe && !hero.IsDead && hero.Distance(position) < range).ToList();
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (target == null)
            {
                return;
            }

            if (!QState && LastQ + 200 < Environment.TickCount && ElLeeSinHarassQ1 && !QState && Q.IsReady() && target.HasQBuff() && (LastQ + 2700 < Environment.TickCount || myHero.GetSpellDamage(target, SpellSlot.Q, DamageLibrary.SpellStages.SecondCast) > target.Health || target.Distance(myHero) > myHero.GetAutoAttackRange() + 50))
            {
                Q2.Cast();
            }

            if (ElLeeSinComboAAStacks && PassiveStacks > ElLeeSinHarassPassiveStacks && myHero.GetAutoAttackRange() > myHero.Distance(target))
            {
                return;
            }

            if (Q.IsReady() && ElLeeSinHarassQ1 && LastQ + 200 < Environment.TickCount)
            {
                if (QState && target.Distance(myHero) < Q.Range)
                {
                    CastQ(target, ElLeeSinSmiteQ);
                }
            }

            if (E.IsReady() && ElLeeSinHarassE1 && LastE + 200 < Environment.TickCount)
            {
                if (EState && target.Distance(myHero) < E.Range)
                {
                    E.Cast();
                    return;
                }

                if (!EState && target.Distance(myHero) > myHero.GetAutoAttackRange() + 50)
                {
                    E.Cast();
                }
            }

            if (ElLeeSinHarassWardjump && myHero.Distance(target) < 50 && !(target.HasQBuff()) && (EState || !E.IsReady() && ElLeeSinHarassE1) && (QState || !Q.IsReady() && ElLeeSinHarassQ1))
            {
                var min = ObjectManager.Get<Obj_AI_Minion>().Where(a => a.IsAlly && a.Distance(myHero) <= W.Range).OrderByDescending(a => a.Distance(target)).FirstOrDefault();
                W.Cast(min);
            }
        }

        private static InventorySlot FindBestWardItem()
        {
            var slot = GetWardSlot();

            if (slot == default(InventorySlot))
            {
                return null;
            }

            var sdi = GetItemSpell(slot);
            if (sdi != default(SpellDataInst) && sdi.State == SpellState.Ready)
            {
                return slot;
            }
            return slot;
        }

        public static InventorySlot GetWardSlot()
        {
            var wardIds = new[] { 2045, 2049, 2050, 2301, 2302, 2303, 3340, 3361, 3362, 3711, 1408, 1409, 1410, 1411, 2043 };
            return (from wardId in wardIds where Item.CanUseItem(wardId) select ObjectManager.Player.InventoryItems.FirstOrDefault(slot => slot.Id == (ItemId)wardId)).FirstOrDefault();
        }

        private static SpellDataInst GetItemSpell(InventorySlot invSlot)
        {
            return myHero.Spellbook.Spells.FirstOrDefault(spell => (int)spell.Slot == invSlot.Slot + 4);
        }

        private static Obj_AI_Base ReturnQBuff()
        {
            return ObjectManager.Get<Obj_AI_Base>().Where(a => a.IsValidTarget(1300)).FirstOrDefault(unit => unit.HasQBuff());
        }

        public static void Orbwalk(Vector3 pos, AIHeroClient target = null)
        {
            Player.IssueOrder(GameObjectOrder.MoveTo, pos);
        }

        private static string SmiteSpellName()
        {
            if (SmiteBlue.Any(a => Item.HasItem(a)))
            {
                return "s5_summonersmiteplayerganker";
            }

            if (SmiteRed.Any(a => Item.HasItem(a)))
            {
                return "s5_summonersmiteduel";
            }

            return "summonersmite";
        }

        public static bool HasQBuff(this Obj_AI_Base unit)
        {
            return (unit.HasBuff("BlindMonkQOne") || unit.HasBuff("blindmonkqonechaos"));
        }

        public static Vector3 GetInsecPos(AIHeroClient target)
        {
            if (isNullInsecPos)
            {
                isNullInsecPos = false;
                insecPos = myHero.Position;
            }

            if (ElLeeSinInsecUseInstaFlash)
            {
                if (GetAllyHeroes(target, 2000 + bonusRangeA).Count > 0 && ElLeeSinInsecAlly)
                {
                    var insecPosition = InterceptionPoint(GetAllyInsec(GetAllyHeroes(target, 2000 + bonusRangeA)));

                    InsecLinePos = Drawing.WorldToScreen(insecPosition);
                    return V2E(insecPosition, target.Position, target.Distance(insecPosition) + 165).To3D();
                }

                if (ElLeeSinInsecOriginalPos)
                {
                    InsecLinePos = Drawing.WorldToScreen(insecPos);
                    return V2E(insecPos, target.Position, target.Distance(insecPos) + 165).To3D();
                }

                if (insecmouse)
                {
                    InsecLinePos = Drawing.WorldToScreen(Game.CursorPos);

                    return V2E(Game.CursorPos, target.Position, target.Distance(insecPos) + 165).To3D();
                }
            }
            else
            {
                if (GetAllyHeroes(target, 2000 + bonusRangeA).Count > 0 && ElLeeSinInsecAlly)
                {
                    var insecPosition = InterceptionPoint(GetAllyInsec(GetAllyHeroes(target, 2000 + bonusRangeA)));

                    InsecLinePos = Drawing.WorldToScreen(insecPosition);
                    return V2E(insecPosition, target.Position, target.Distance(insecPosition) + 225).To3D();
                }

                if (ElLeeSinInsecOriginalPos)
                {
                    InsecLinePos = Drawing.WorldToScreen(insecPos);
                    return V2E(insecPos, target.Position, target.Distance(insecPos) + 225).To3D();
                }

                if (insecmouse)
                {
                    InsecLinePos = Drawing.WorldToScreen(Game.CursorPos);

                    return V2E(Game.CursorPos, target.Position, target.Distance(insecPos) + 225).To3D();
                }
            }

            return new Vector3();
        }

        private static Vector2 V2E(Vector3 from, Vector3 direction, float distance)
        {
            return from.To2D() + distance * Vector3.Normalize(direction - from).To2D();
        }

        private static void CastW(Obj_AI_Base obj)
        {
            if (500 >= Environment.TickCount - wcasttime || WStage != WCastStage.First)
            {
                return;
            }

            W.Cast(obj);
            wcasttime = Environment.TickCount;
        }

        private static WCastStage WStage
        {
            get
            {
                if (!W.IsReady())
                {
                    return WCastStage.Cooldown;
                }

                return (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "blindmonkwtwo" ? WCastStage.Second : WCastStage.First);
            }
        }

        private static void WardJump(
            Vector3 pos,
            bool m2M = true,
            bool maxRange = false,
            bool reqinMaxRange = false,
            bool minions = true,
            bool champions = true)
        {
            if (WStage != WCastStage.First)
            {
                return;
            }

            var basePos = myHero.Position.To2D();
            var newPos = (pos.To2D() - myHero.Position.To2D());

            if (JumpPos == new Vector2())
            {
                if (reqinMaxRange)
                {
                    JumpPos = pos.To2D();
                }
                else if (maxRange || myHero.Distance(pos) > 590)
                {
                    JumpPos = basePos + (newPos.Normalized() * (590));
                }
                else
                {
                    JumpPos = basePos + (newPos.Normalized() * (myHero.Distance(pos)));
                }
            }
            if (JumpPos != new Vector2() && reCheckWard)
            {
                reCheckWard = false;
                Core.DelayAction(delegate
                {
                    if (JumpPos != new Vector2())
                    {
                        JumpPos = new Vector2();
                        reCheckWard = true;
                    }
                }, 20);
            }
            if (m2M)
            {
                Orbwalk(pos);
            }
            if (!W.IsReady() || W.Name == "blindmonkwtwo" || reqinMaxRange && myHero.Distance(pos) > W.Range)
            {
                return;
            }

            if (minions || champions)
            {
                if (champions)
                {
                    var champs = (from champ in ObjectManager.Get<AIHeroClient>() where champ.IsAlly && champ.Distance(myHero) < W.Range && champ.Distance(pos) < 200 && !champ.IsMe select champ).ToList();
                    if (champs.Count > 0 && WStage == WCastStage.First)
                    {
                        if (500 >= Environment.TickCount - wcasttime || WStage != WCastStage.First)
                        {
                            return;
                        }

                        CastW(champs[0]);
                        return;
                    }
                }
                if (minions)
                {
                    var minion2 = (from minion in EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Ally) where minion.IsAlly && minion.Distance(myHero) < W.Range && minion.Distance(pos) < 200 && !minion.Name.ToLower().Contains("ward") select minion).ToList();
                    if (minion2.Count > 0 && WStage == WCastStage.First)
                    {
                        CastW(minion2[0]);
                        return;
                    }
                }
            }

            var isWard = false;
            foreach (var ward in ObjectManager.Get<Obj_AI_Base>())
            {
                if (ward.IsAlly && ward.Name.ToLower().Contains("ward") && ward.Distance(JumpPos) < 200)
                {
                    isWard = true;
                    if (500 >= Environment.TickCount - wcasttime || WStage != WCastStage.First) //credits to JackisBack
                    {
                        return;
                    }

                    CastW(ward);
                    wcasttime = Environment.TickCount;
                }
            }

            if (!isWard && castWardAgain)
            {
                var ward = FindBestWardItem();
                if (ward == null || WStage != WCastStage.First)
                {
                    return;
                }

                myHero.Spellbook.CastSpell(ward.SpellSlot, JumpPos.To3D());

                lastWardPos = JumpPos.To3D();
                LastWard = Environment.TickCount;
            }
        }
        private static void WardjumpToMouse()
        {
            WardJump(Game.CursorPos, ElLeeSinWardjumpMouse, ElLeeSinWardjumpMaxRange, false, ElLeeSinWardjumpMinions, ElLeeSinWardjumpChampions);
        }
        private static void CastQ(AIHeroClient target, bool smiteQ = false)
        {
            if (!Q.IsReady() || !target.IsValidTarget(Q.Range))
            {
                return;
            }

            var prediction = Q.GetPrediction(target);

            if (prediction.HitChance >= HitChance.High)
            {
                Q.Cast(target);
            }
            else if (ElLeeSinSmiteQ && Q.IsReady() && target.IsValidTarget(Q.Range) && prediction.CollisionObjects.Count(a => a.NetworkId != target.NetworkId && a.IsMinion) == 1 && smiteSlot.IsReady())
            {
                smiteSlot.Cast(prediction.CollisionObjects.Where(a => a.NetworkId != target.NetworkId && a.IsMinion).ToList()[0]);
                Q.Cast(prediction.CastPosition);
            }
        }

        public static float Q2Damage(Obj_AI_Base target, float subHP = 0, bool monster = false)
        {
            var damage = (50 + (Q.Level * 30)) + (0.09 * myHero.FlatPhysicalDamageMod) + ((target.MaxHealth - (target.Health - subHP)) * 0.08);

            if (monster && damage > 400)
            {
                return (float)Damage.CalculateDamageOnUnit(myHero, target, DamageType.Physical, 400);
            }

            return Damage.CalculateDamageOnUnit(myHero, target, DamageType.Physical, (float)damage);
        }

        private static void UseItems(Obj_AI_Base target)
        {
            if (target == null) { return; }
            if (Item.CanUseItem(ItemId.Ravenous_Hydra_Melee_Only) && 400 > myHero.Distance(target))
            {
                Item.UseItem(ItemId.Ravenous_Hydra_Melee_Only);
            }

            if (Item.CanUseItem(ItemId.Tiamat_Melee_Only) && 400 > myHero.Distance(target))
            {
                Item.UseItem(ItemId.Tiamat_Melee_Only);
            }

            if (Item.CanUseItem(ItemId.Blade_of_the_Ruined_King) && 550 > myHero.Distance(target))
            {
                Item.UseItem(ItemId.Blade_of_the_Ruined_King);
            }

            if (Item.CanUseItem(ItemId.Youmuus_Ghostblade) && myHero.GetAutoAttackRange() > myHero.Distance(target))
            {
                Item.UseItem(ItemId.Youmuus_Ghostblade);
            }
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (target == null) { return; }

            UseItems(target);

            if (Q.IsReady() && Q.Name == "BlindMonkQOne" && ElLeeSinComboQ)
            {
                CastQ(target, ElLeeSinSmiteQ);
            }

            if (!target.IsZombie && R.IsReady() && ElLeeSinComboR && ElLeeSinComboQ && target.IsValidTarget(R.Range) && (myHero.GetSpellDamage(target, SpellSlot.R) >= target.Health || target.HasQBuff() && target.Health < myHero.GetSpellDamage(target, SpellSlot.R) + Q2Damage(target, myHero.GetSpellDamage(target, SpellSlot.R))))
            {
                R.Cast(target);
            }

            if (target.HasQBuff() && ElLeeSinComboQ2)
            {
                if (!target.IsZombie && target.HasQBuff() && target.Health < myHero.GetSpellDamage(target, SpellSlot.R) + Q2Damage(target, myHero.GetSpellDamage(target, SpellSlot.R)))
                {
                    if (ElLeeSinComboKSR)
                    {
                        R.Cast(target);
                    }
                }

                var prediction = Q.GetPrediction(target);

                if (castQAgain || target.HasBuffOfType(BuffType.Knockback) && !myHero.IsValidTarget(300) && !R.IsReady() && prediction.HitChance >= HitChance.High || !target.IsValidTarget(myHero.GetAutoAttackRange()) || myHero.GetSpellDamage(target, SpellSlot.R, DamageLibrary.SpellStages.SecondCast) > target.Health || ReturnQBuff().Distance(target) < myHero.Distance(target) && !target.IsValidTarget(myHero.GetAutoAttackRange()))
                {
                    Q.Cast(target);
                }
            }

            if (!target.IsZombie && myHero.GetSpellDamage(target, SpellSlot.R) >= target.Health && ElLeeSinComboKSR && target.IsValidTarget())
            {
                R.Cast(target);
            }

            if (ElLeeSinComboAAStacks && PassiveStacks > ElLeeSinComboPassiveStacks && myHero.GetAutoAttackRange() > myHero.Distance(target))
            {
                return;
            }

            if (ElLeeSinComboW)
            {
                if (ElLeeSinComboModeWW && target.Distance(myHero) > myHero.GetAutoAttackRange())
                {
                    WardJump(target.Position, false, true);
                }

                if (!ElLeeSinComboModeWW && target.Distance(myHero) > Q.Range)
                {
                    WardJump(target.Position, false, true);
                }
            }

            if (E.IsReady() && ElLeeSinComboE)
            {
                if (EState && E.IsInRange(target))
                {
                    E.Cast();
                    LastSpell = Environment.TickCount;
                    return;
                }

                if (!EState && E.IsInRange(target) && LastE + 400 < Environment.TickCount)
                {
                    E.Cast();
                    LastSpell = Environment.TickCount;
                }
            }

            if (W.IsReady() && ElLeeSinComboW2)
            {
                if (WState && myHero.Distance(target) <= myHero.GetAutoAttackRange())
                {
                    W.Cast(myHero);
                    LastW = Environment.TickCount;
                    return;
                }

                if (!WState && target.Distance(myHero) <= myHero.GetAutoAttackRange()
                    && LastW + 200 < Environment.TickCount)
                {
                    W.Cast(myHero);
                }
            }
        }

        private static void InsecCombo(AIHeroClient target)
        {
            if (target != null && target.IsVisible)
            {
                if (myHero.Distance(GetInsecPos(target)) < 200)
                {
                    insecComboStep = InsecComboStepSelect.Pressr;
                }
                else if (insecComboStep == InsecComboStepSelect.None && GetInsecPos(target).Distance(myHero.Position) < 600)
                {
                    insecComboStep = InsecComboStepSelect.Wgapclose;
                }
                else if (insecComboStep == InsecComboStepSelect.None && target.Distance(myHero) < Q.Range)
                {
                    insecComboStep = InsecComboStepSelect.Qgapclose;
                }

                switch (insecComboStep)
                {
                    case InsecComboStepSelect.Qgapclose:

                        var prediction = Q.GetPrediction(target);
                        if (prediction.CollisionObjects.Count(h => h.IsEnemy && !h.IsDead && h is Obj_AI_Minion) >= 1 && checkOthers1 && Q.IsReady())
                        {
                            foreach (var unit in ObjectManager.Get<Obj_AI_Base>().Where(obj => (((obj is Obj_AI_Minion) && myHero.GetSpellDamage(target, SpellSlot.Q) < obj.Health + 10) || (obj is AIHeroClient)) && obj.IsValidTarget(Q.Range) && obj.Distance(GetInsecPos(target)) < 500))
                            {
                                var pred = Q.GetPrediction(unit);
                                if (pred.HitChance >= HitChance.High)
                                {
                                    Q.Cast(pred.CastPosition);
                                }
                                break;
                            }
                        }

                        if (!(target.HasQBuff()) && QState)
                        {
                            CastQ(target, ElLeeSinSmiteQ);
                        }
                        else if (target.HasQBuff())
                        {
                            Q2.Cast();
                            insecComboStep = InsecComboStepSelect.Wgapclose;
                        }
                        else
                        {
                            if (Q.Name == "blindmonkqtwo" && ReturnQBuff().Distance(target) <= 600 && target.HasQBuff())
                            {
                                Q2.Cast();
                            }
                        }
                        break;

                    case InsecComboStepSelect.Wgapclose:
                        if (myHero.Distance(target) < 600)
                        {
                            Console.WriteLine("Warded in 600");
                            WardJump(GetInsecPos(target) - 15, false, true, true);
                            if (FindBestWardItem() == null && GetInsecPos(target).Distance(myHero.Position) < 400 && LastWard + 500 < Environment.TickCount || FindBestWardItem() != null && GetInsecPos(target).Distance(myHero.Position) < 400 && LastWard + 500 < Environment.TickCount || !W.IsReady() && GetInsecPos(target).Distance(myHero.Position) < 400 && LastWard + 500 < Environment.TickCount)
                            {
                                if (R.IsReady() && flashSlot.IsReady() && ElLeeSinFlashInsec && LastWard + 1000 < Environment.TickCount)
                                {
                                    flashSlot.Cast(GetInsecPos(target));
                                    return;
                                }
                            }
                        }

                        if (R.IsReady() && myHero.Distance(target) < 700 && LastWard + 700 < Environment.TickCount)
                        {
                            WardJump(target.Position - 15, false, true, true);

                            if (GetInsecPos(target).Distance(myHero.Position) < 425 && ElLeeSinFlashInsec && LastWard + 500 < Environment.TickCount && FindBestWardItem() == null && flashSlot.IsReady())
                            {
                                flashSlot.Cast(GetInsecPos(target));
                                return;
                            }
                        }
                        break;

                    case InsecComboStepSelect.Pressr:
                        R.Cast(target);
                        break;
                }
            }
        }
    }
}
