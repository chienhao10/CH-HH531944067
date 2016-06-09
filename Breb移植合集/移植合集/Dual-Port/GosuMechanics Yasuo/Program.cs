using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace GosuMechanicsYasuo
{
    class Program
    {

        public static LeagueSharp.Common.Spell Q, Q3;
        public static LeagueSharp.Common.Spell W = new LeagueSharp.Common.Spell(SpellSlot.W, 400);
        public static LeagueSharp.Common.Spell E = new LeagueSharp.Common.Spell(SpellSlot.E, 475);
        public static LeagueSharp.Common.Spell R = new LeagueSharp.Common.Spell(SpellSlot.R, 1200);
        public static LeagueSharp.Common.Spell Ignite;
        public static LeagueSharp.Common.Spell Flash;
        public static Menu Config;
        public static bool wallCasted;
        public static List<Skillshot> DetectedSkillShots = new List<Skillshot>();
        public static List<Skillshot> EvadeDetectedSkillshots = new List<Skillshot>();
        public static Menu skillShotMenu;
        public static bool isDashing;
        public static YasWall wall = new YasWall();
        public static AIHeroClient myHero { get { return ObjectManager.Player; } }
        public static float HealthPercent { get { return myHero.Health / myHero.MaxHealth * 100; } }

        public struct IsSafeResult
        {
            public bool IsSafe;
            public List<Skillshot> SkillshotList;
            public List<Obj_AI_Base> casters;
        }
        internal class YasWall
        {
            public MissileClient pointL;
            public MissileClient pointR;
            public float endtime = 0;
            public YasWall()
            {

            }

            public YasWall(MissileClient L, MissileClient R)
            {
                pointL = L;
                pointR = R;
                endtime = Game.Time + 4;
            }

            public void setR(MissileClient R)
            {
                pointR = R;
                endtime = Game.Time + 4;
            }

            public void setL(MissileClient L)
            {
                pointL = L;
                endtime = Game.Time + 4;
            }

            public bool isValid(int time = 0)
            {
                return pointL != null && pointR != null && endtime - (time / 1000) > Game.Time;
            }
        }

        public static Menu wwMenu, comboMenu, harassMenu, ultMenu, lastHitMenu, laneClearMenu, jungleClearMenu, fleeMenu, smartWMenu, miscMenu, drawMenu;

        private static float GetQDelay { get { return 1 - Math.Min((myHero.AttackSpeedMod - 1) * 0.0058552631578947f, 0.6675f); } }
        private static float GetQ1Delay { get { return 0.4f * GetQDelay; } }
        private static float GetQ2Delay { get { return 0.5f * GetQDelay; } }

        public static void Game_OnGameLoad()
        {
            if (myHero.ChampionName != "Yasuo")
                return;

            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 505);
            Q3 = new LeagueSharp.Common.Spell(SpellSlot.Q, 1100);
            Q.SetSkillshot(GetQ1Delay, 20, float.MaxValue, false, SkillshotType.SkillshotLine);
            Q3.SetSkillshot(GetQ2Delay, 90, 1200, false, SkillshotType.SkillshotLine);

            var slot = ObjectManager.Player.GetSpellSlot("summonerdot");
            if (slot != SpellSlot.Unknown)
            {
                Ignite = new LeagueSharp.Common.Spell(slot, 600, DamageType.True);
            }

            var Fslot = ObjectManager.Player.GetSpellSlot("SummonerFlash");
            if (Fslot != SpellSlot.Unknown)
            {
                Flash = new LeagueSharp.Common.Spell(slot, 425);
            }
         
            Config = MainMenu.AddMenu("GosuMechanics Yasuo", "Yasuo");

            // WW Combo White List
            wwMenu = Config.AddSubMenu("Windwall on Combo Whitelist Settings", "ww");
            foreach (var hero in HeroManager.Enemies.Where(x => x.IsEnemy))
            {
                wwMenu.Add(hero.ChampionName, new CheckBox("Use Put WallBehind if Enemy is " + hero.ChampionName));
            }

            // Combo
            comboMenu = Config.AddSubMenu("Combo Settings", "combo");
            comboMenu.AddGroupLabel("Q Settings : ");
            comboMenu.Add("QC", new CheckBox("Use Q"));
            comboMenu.AddGroupLabel("E Settings : ");
            comboMenu.Add("EC", new CheckBox("Use E"));
            comboMenu.Add("E1", new Slider("when enemy range >=", 375, 1, 475));
            comboMenu.Add("E2", new Slider("Use E-GapCloser when enemy range >=", 230, 1, 1300));
            comboMenu.Add("E3", new CheckBox("Mode: On = ToTarget / OFF = ToMouse"));
            comboMenu.AddGroupLabel("R Settings : ");
            comboMenu.Add("R", new CheckBox("Use Smart R"));
            comboMenu.Add("R1", new Slider("When enemy HP <=", 50, 1, 101));
            comboMenu.Add("R2", new Slider("Or when enemy knockedup >=", 2, 1, 5));
            comboMenu.Add("R3", new CheckBox("Use R instantly when an ally is in range"));
            comboMenu.AddGroupLabel("Auto R Settings : ");
            comboMenu.Add("R4", new CheckBox("Use Auto R"));
            comboMenu.Add("R5", new Slider("when knockedUp enemy is >=", 3, 1, 5));
            comboMenu.Add("R6", new Slider("when <= enemy in range", 2, 1, 5));
            comboMenu.Add("R7", new Slider("when myHero HP is >=", 50, 1, 101));
            comboMenu.AddGroupLabel("Item Settings : ");
            comboMenu.Add("Ignite", new CheckBox("Use Ignite"));
            comboMenu.Add("comboItems", new CheckBox("Use Items"));
            comboMenu.Add("myHP", new Slider("Use BOTRK if my hp <=", 70, 1, 101));

            //R whitelist
            ultMenu = Config.AddSubMenu("Ult Whitelist Settings", "ult");
            foreach (var hero in HeroManager.Enemies.Where(x => x.IsEnemy))
            {
                ultMenu.Add(hero.ChampionName, new CheckBox("Use Ulti if Target is " + hero.ChampionName));
            }

            //Harass / AutoQ
            harassMenu = Config.AddSubMenu("Harass Settings", "Harass");
            harassMenu.Add("AutoQHarass", new KeyBind("Harass Toggle", false, KeyBind.BindTypes.PressToggle, 'L'));
            harassMenu.Add("HarassQ", new CheckBox("Use Q12"));
            harassMenu.Add("HarassQ3", new CheckBox("Use Q3"));
            harassMenu.Add("HarassTower", new CheckBox("Harass UnderTower"));

            //LastHit
            lastHitMenu = Config.AddSubMenu("LastHit Settings", "LastHit");
            lastHitMenu.Add("LastHitQ1", new CheckBox("Use Q12"));
            lastHitMenu.Add("LastHitQ3", new CheckBox("Use Q3"));
            lastHitMenu.Add("LastHitE", new CheckBox("Use E"));

            //LaneClear
            laneClearMenu = Config.AddSubMenu("LaneClear Settings", "Clear");
            laneClearMenu.Add("LaneClearQ1", new CheckBox("Use Q12"));
            laneClearMenu.Add("LaneClearQ3", new CheckBox("Use Q3"));
            laneClearMenu.Add("LaneClearQ3count", new Slider("when Q3 will hit minions >= ", 2, 1, 5));
            laneClearMenu.Add("LaneClearE", new CheckBox("Use E"));
            laneClearMenu.Add("LaneClearItems", new CheckBox("Use Items"));

            //JungleClear
            jungleClearMenu = Config.AddSubMenu("JungleClear Settings", "JungleClear");
            jungleClearMenu.Add("JungleClearQ12", new CheckBox("Use Q12"));
            jungleClearMenu.Add("JungleClearQ3", new CheckBox("Use Q3"));
            jungleClearMenu.Add("JungleClearE", new CheckBox("Use E"));

            //Flee away
            fleeMenu = Config.AddSubMenu("Escape Settings", "Escape");
            fleeMenu.Add("flee", new KeyBind("Escape", false, KeyBind.BindTypes.HoldActive, 'Z'));
            fleeMenu.Add("wall", new KeyBind("WallJump Escape", false, KeyBind.BindTypes.HoldActive, 'V'));
            fleeMenu.Add("AutoQ1", new CheckBox("Use Q Stack while Dashing"));
            fleeMenu.Add("AutoQToggle", new KeyBind("Auto Q Minion Toggle (Normal)", true, KeyBind.BindTypes.PressToggle, 'K'));

            //SmartW
            smartWMenu = Config.AddSubMenu("WindWall Settings", "aShots");
            smartWMenu.Add("smartW", new CheckBox("Use Auto WindWall"));
            smartWMenu.Add("smartWDanger", new Slider("if Spell DangerLevel >=", 3, 1, 5));
            smartWMenu.Add("smartWDelay", new Slider("WindWall Humanizer (500 = Lowest Reaction Time)", 3000, 500, 3000));
            smartWMenu.Add("smartEDogue", new CheckBox("Use E-Vade"));
            smartWMenu.Add("smartEDogueDanger", new Slider("if Spell DangerLevel >=", 1, 1, 5));
            smartWMenu.Add("wwDanger", new CheckBox("Block only dangerous", false));

            skillShotMenu = getSkilshotMenu();

            //Misc
            miscMenu = Config.AddSubMenu("Misc Settings", "misc");
            miscMenu.Add("ETower", new CheckBox("Dont Jump turrets"));
            miscMenu.Add("KS", new CheckBox("KillSteal"));
            miscMenu.Add("IntAnt", new CheckBox("Use Q3 - AntiGapcloser/Interrupter"));

            //draw
            drawMenu = Config.AddSubMenu("Draw Settings", "Draw");
            drawMenu.Add("Disable", new CheckBox("Disable all draws", false));
            drawMenu.Add("DrawQ", new CheckBox("Draw Q12 Range"));
            drawMenu.Add("DrawQ3", new CheckBox("Draw Q3 Range"));
            drawMenu.Add("DrawW", new CheckBox("Draw W Range"));
            drawMenu.Add("DrawE", new CheckBox("Draw E Range"));
            drawMenu.Add("DrawR", new CheckBox("Draw R Range"));
            drawMenu.Add("DrawSpots", new CheckBox("Draw WallJump Spots"));

            SkillshotDetector.OnDetectSkillshot += OnDetectSkillshot;
            SkillshotDetector.OnDeleteMissile += OnDeleteMissile;
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnCreate += Obj_AI_Base_OnCreate;
            Obj_AI_Base.OnDelete += Obj_AI_Base_OnDelete;
            Obj_AI_Base.OnPlayAnimation += Obj_AI_Base_OnPlayAnimation;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        private static void Obj_AI_Base_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender is MissileClient)
            {
                MissileClient missle = (MissileClient)sender;
                if (missle.SData.Name == "yasuowmovingwallmisl")
                {
                    wall.setL(missle);
                }
                if (missle.SData.Name == "yasuowmovingwallmisl")
                {
                    wallCasted = false;
                }
                if (missle.SData.Name == "yasuowmovingwallmisr")
                {
                    wall.setR(missle);
                }
            }
        }

        private static void Obj_AI_Base_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender is MissileClient)
            {
                MissileClient missle = (MissileClient)sender;
                if (missle.SData.Name == "yasuowmovingwallmisl")
                {
                    wall.setL(missle);
                }
                if (missle.SData.Name == "yasuowmovingwallmisl")
                {
                    wallCasted = true;
                }
                if (missle.SData.Name == "yasuowmovingwallmisr")
                {
                    wall.setR(missle);
                }
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var target = gapcloser.Sender;

            if (!target.LSIsValidTarget(Q3.Range))
            {
                return;
            }

            if (Q3.IsReady() && Q3READY() && getCheckBoxItem(miscMenu, "IntAnt"))
            {
                Q3.Cast(target);
            }
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender != null && Q3.IsReady() && Q3READY() && sender.LSIsValidTarget(Q3.Range) && getCheckBoxItem(miscMenu, "IntAnt"))
            {
                Q3.Cast(sender);
            }
        }

        private static void Obj_AI_Base_OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (!sender.IsMe || args.Animation != "Spell3")
            {
                return;
            }
            isDashing = true;
            LeagueSharp.Common.Utility.DelayAction.Add(300, () => { if (myHero.LSIsDashing()) { isDashing = false; } });

            LeagueSharp.Common.Utility.DelayAction.Add(450, () => isDashing = false);
        }
        private static void Game_OnUpdate(EventArgs args)
        {
            if (myHero.IsDead || myHero.IsRecalling())
            {
                return;
            }

            EvadeDetectedSkillshots.RemoveAll(skillshot => !skillshot.IsActive());

            foreach (var mis in EvadeDetectedSkillshots)
            {
                if (getCheckBoxItem(smartWMenu, "smartW"))
                    useWSmart(mis);

                if (getCheckBoxItem(smartWMenu, "smartEDogue") && !W.IsReady() && !isSafePoint(ObjectManager.Player.Position.LSTo2D(), true).IsSafe)
                    useEtoSafe(mis);
            }
            if (getKeyBindItem(fleeMenu, "flee"))
            {
                Flee();
                AutoQFlee();
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }

            if (getKeyBindItem(fleeMenu, "wall"))
            {
                Yasuo.WallJump();
                Yasuo.WallDash();
            }

            if (getKeyBindItem(fleeMenu, "AutoQToggle") && (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee)) && !getKeyBindItem(fleeMenu, "flee") && !getKeyBindItem(fleeMenu, "wall"))
            {
                AutoQ();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                LaneClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                LastHit();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
                HarassLastHit();
            }

            AutoR();
            KillSteal();

            if (!IsDashing && getKeyBindItem(harassMenu, "AutoQHarass") && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                var TsTarget = TargetSelector.GetTarget(1000, DamageType.Physical);

                if (TsTarget != null && TsTarget.CharData.BaseSkinName != "gangplankbarrel" && getCheckBoxItem(harassMenu, "HarassTower") && Q3.IsReady() && getCheckBoxItem(harassMenu, "HarassQ3") && !IsDashing && Q3READY() && Q3.IsInRange(TsTarget))
                {
                    CastQ3(TsTarget);
                }
                else if (TsTarget != null && !getCheckBoxItem(harassMenu, "HarassTower") && TsTarget.CharData.BaseSkinName != "gangplankbarrel" && !UnderTower(myHero.ServerPosition.LSTo2D()) && Q3.IsReady() && getCheckBoxItem(harassMenu, "HarassQ3") && !IsDashing && Q3READY() && Q3.IsInRange(TsTarget))
                {
                    CastQ3(TsTarget);
                }
            }

            if (!IsDashing && getKeyBindItem(harassMenu, "AutoQHarass") && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                var TsTarget = TargetSelector.GetTarget(475, DamageType.Physical);

                if (TsTarget != null && TsTarget.CharData.BaseSkinName != "gangplankbarrel" && getCheckBoxItem(harassMenu, "HarassTower") && !Q3READY() && Q.IsReady() && getCheckBoxItem(harassMenu, "HarassQ") && !IsDashing && Q.IsInRange(TsTarget))
                {
                    CastQ12(TsTarget);
                }
                else if (TsTarget != null && TsTarget.CharData.BaseSkinName != "gangplankbarrel" && !getCheckBoxItem(harassMenu, "HarassTower") && !Q3READY() && Q.IsReady() && getCheckBoxItem(harassMenu, "HarassQ") && !IsDashing && !UnderTower(myHero.ServerPosition.LSTo2D()) && Q.IsInRange(TsTarget))
                {
                    CastQ12(TsTarget);
                }
            }
        }
        public static void HarassLastHit()
        {
            foreach (Obj_AI_Base minion in MinionManager.GetMinions(myHero.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Enemy).OrderByDescending(m => m.Health))
            {
                if (minion == null)
                {
                    return;
                }

                if (!minion.IsDead && minion != null && getCheckBoxItem(lastHitMenu, "LastHitQ1") && Q.IsReady() && minion.LSIsValidTarget(500) && !Q3READY() && Q.IsInRange(minion))
                {
                    var predHealth = HealthPrediction.GetHealthPrediction(minion, (int)(Program.myHero.LSDistance(minion.Position) * 1000 / 2000));
                    if (predHealth <= GetQDmg(minion))
                    {
                        CastQ12(minion);
                    }
                }
            }
        }
        public static void Harass()
        {
            var TsTarget = TargetSelector.GetTarget(1300, DamageType.Physical);

            if (TsTarget == null || TsTarget.CharData.BaseSkinName == "gangplankbarrel")
            {
                return;
            }
            if (TsTarget != null && getCheckBoxItem(harassMenu, "HarassTower"))
            {

                if (Q3.IsReady() && getCheckBoxItem(harassMenu, "HarassQ3") && !IsDashing && Q3READY() && Q3.IsInRange(TsTarget))
                {
                    CastQ3(TsTarget);
                }
                else if (!Q3READY() && Q.IsReady() && getCheckBoxItem(harassMenu, "HarassQ") && !IsDashing && Q.IsInRange(TsTarget))
                {
                    CastQ12(TsTarget);
                }
            }
            else if (TsTarget != null && !getCheckBoxItem(harassMenu, "HarassTower"))
            {
                if (!UnderTower(myHero.ServerPosition.LSTo2D()) && Q3.IsReady() && getCheckBoxItem(harassMenu, "HarassQ3") && !IsDashing && Q3READY() && Q3.IsInRange(TsTarget))
                {
                    CastQ3(TsTarget);
                }
                if (!Q3READY() && Q.IsReady() && getCheckBoxItem(harassMenu, "HarassQ") && !IsDashing && !UnderTower(myHero.ServerPosition.LSTo2D()) && Q.IsInRange(TsTarget))
                {
                    CastQ12(TsTarget);
                }
            }
        }
        public static void Flee()
        {
            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range);
            if (E.IsReady())
            {
                var bestMinion =
                   ObjectManager.Get<Obj_AI_Base>()
                       .Where(x => x.LSIsValidTarget(E.Range))
                       .Where(x => x.LSDistance(Game.CursorPos) < ObjectManager.Player.LSDistance(Game.CursorPos))
                       .OrderByDescending(x => x.LSDistance(ObjectManager.Player))
                       .FirstOrDefault();

                if (bestMinion != null && ObjectManager.Player.LSIsFacing(bestMinion) && CanCastE(bestMinion) && E.IsReady())
                {
                    E.CastOnUnit(bestMinion, true);
                }
            }
        }
        public static void AutoQFlee()
        {
            List<Obj_AI_Base> Qminion = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 1000, MinionTypes.All, MinionTeam.NotAlly);
            foreach (var minion in Qminion.Where(minion => minion.LSIsValidTarget(Q.Range) && !minion.IsDead))
            {
                if (minion == null)
                {
                    return;
                }
                if (!Q3READY() && getCheckBoxItem(fleeMenu, "AutoQ1") && minion.LSIsValidTarget(Q.Range) && IsDashing)
                {
                    CastQ12(minion);
                }
            }
        }
        public static void AutoQ()
        {
            List<Obj_AI_Base> Qminion = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 1000, MinionTypes.All, MinionTeam.NotAlly);
            foreach (var minion in Qminion.Where(minion => minion.LSIsValidTarget(Q.Range) && !minion.IsDead))
            {
                if (minion == null && minion.CharData.BaseSkinName == "gangplankbarrel")
                {
                    return;
                }
                if (!Q3READY() && getCheckBoxItem(fleeMenu, "AutoQ1") && minion.LSIsValidTarget(Q.Range) && !IsDashing)
                {
                    CastQ12(minion);
                }
            }
        }
        public static void LastHit()
        {
            foreach (Obj_AI_Base minion in MinionManager.GetMinions(myHero.ServerPosition, Q3.Range, MinionTypes.All, MinionTeam.Enemy).OrderByDescending(m => m.Health))
            {
                if (minion == null)
                {
                    return;
                }

                if (!minion.IsDead && minion != null && getCheckBoxItem(lastHitMenu, "LastHitQ1") && Q.IsReady() && minion.LSIsValidTarget(500) && !Q3READY() && Q.IsInRange(minion) && !IsDashing)
                {
                    var predHealth = HealthPrediction.GetHealthPrediction(minion, (int)(Program.myHero.LSDistance(minion.Position) * 1000 / 2000));
                    if (predHealth <= GetQDmg(minion))
                    {
                        CastQ12(minion);
                    }
                }
                if (!minion.IsDead && minion != null && getCheckBoxItem(lastHitMenu, "LastHitQ3") && Q.IsReady() && minion.LSIsValidTarget(1100) && Q3READY() && Q3.IsInRange(minion) && !IsDashing)
                {
                    var predHealth = HealthPrediction.GetHealthPrediction(minion, (int)(Program.myHero.LSDistance(minion.Position) * 1000 / 2000));
                    if (predHealth <= GetQDmg(minion))
                    {
                        CastQ3(minion);
                    }
                }
                if (getCheckBoxItem(lastHitMenu, "LastHitE") && E.IsReady() && minion.LSIsValidTarget(475))
                {
                    if (!UnderTower(PosAfterE(minion)) && CanCastE(minion))
                    {
                        var predHealth = HealthPrediction.GetHealthPrediction(minion, (int)(Program.myHero.LSDistance(minion.Position) * 1000 / 2000));
                        if (predHealth <= GetEDmg(minion) && !isDangerous(minion, 600))
                        {
                            E.CastOnUnit(minion, true);
                        }
                    }
                }
            }
        }
        public static void LaneClear()
        {
            List<Obj_AI_Base> Qminion = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 1000, MinionTypes.All, MinionTeam.Enemy);
            foreach (var minion in Qminion.Where(minion => minion.LSIsValidTarget(Q3.Range)))
            {
                if (minion == null)
                {
                    return;
                }
                if (minion != null && getCheckBoxItem(laneClearMenu, "LaneClearQ1") && Q.IsReady() && minion.LSIsValidTarget(500) && !Q3READY() && Q.IsInRange(minion))
                {
                    var predHealth = HealthPrediction.LaneClearHealthPrediction(minion, (int)(Program.myHero.LSDistance(minion.Position) * 1000 / 2000));
                    if (predHealth <= GetQDmg(minion) && !IsDashing)
                    {
                        CastQ12(minion);
                    }
                    else if (!Q3READY() && Q.IsInRange(minion) && !IsDashing)
                    {
                        List<Vector2> minionPs = GetCastMinionsPredictedPositions(Qminion, .025f, 50f, float.MaxValue, myHero.ServerPosition, 475f, false, SkillshotType.SkillshotLine);
                        MinionManager.FarmLocation farm = Q.GetLineFarmLocation(minionPs);
                        if (farm.MinionsHit >= 1)
                        {
                            CastQ12(minion);
                        }
                        else if (Q.IsReady() && IsDashing && Q.IsInRange(minion))
                        {
                            if (farm.MinionsHit >= 2)
                            {
                                CastQ12(minion);
                            }
                        }
                    }
                }
                if (!minion.IsDead && minion != null && getCheckBoxItem(laneClearMenu, "LaneClearQ3") && Q3.IsReady() && minion.LSIsValidTarget(1100) && Q3READY() && Q3.IsInRange(minion))
                {
                    var predHealth = HealthPrediction.LaneClearHealthPrediction(minion, (int)(Program.myHero.LSDistance(minion.Position) * 1000 / 2000));
                    if (predHealth <= GetQDmg(minion) && !IsDashing)
                    {
                        CastQ3(minion);
                    }
                    else if (Q3READY() && Q3.IsInRange(minion) && !IsDashing)
                    {
                        List<Vector2> minionPs = GetCastMinionsPredictedPositions(Qminion, .025f, 50f, float.MaxValue, myHero.ServerPosition, 1000f, false, SkillshotType.SkillshotLine);
                        MinionManager.FarmLocation farm = Q3.GetLineFarmLocation(minionPs);
                        if (farm.MinionsHit >= getSliderItem(laneClearMenu, "LaneClearQ3count"))
                        {
                            CastQ3(minion);
                        }
                        else if (Q3.IsReady() && IsDashing && Q.IsInRange(minion) && Q3READY())
                        {
                            if (farm.MinionsHit >= 2)
                            {
                                CastQ3(minion);
                            }
                        }
                    }
                }
            }
            var allMinionsE = MinionManager.GetMinions(myHero.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Enemy);
            foreach (var minion in allMinionsE.Where(x => x.LSIsValidTarget(E.Range) && CanCastE(x)))
            {
                if (minion == null)
                {
                    return;
                }

                if (getCheckBoxItem(laneClearMenu, "LaneClearE") && E.IsReady() && minion.LSIsValidTarget(E.Range) && CanCastE(minion))
                {
                    if (!UnderTower(PosAfterE(minion)))
                    {

                        var predHealth = HealthPrediction.LaneClearHealthPrediction(minion, (int)(Program.myHero.LSDistance(minion.Position) * 1000 / 2000));
                        if (predHealth <= GetEDmg(minion) && !isDangerous(minion, 600))
                        {
                            E.CastOnUnit(minion, true);
                        }
                    }
                }
                if (getCheckBoxItem(laneClearMenu, "LaneClearItems"))
                {
                    UseItems(minion);
                }
            }
            var jminions = MinionManager.GetMinions(myHero.ServerPosition, 1000, MinionTypes.All, MinionTeam.Neutral);
            foreach (var jungleMobs in jminions.Where(x => x.LSIsValidTarget(Q3.Range)))
            {
                if (jungleMobs == null)
                {
                    return;
                }
                if (getCheckBoxItem(jungleClearMenu, "JungleClearE") && E.IsReady() && jungleMobs != null && jungleMobs.LSIsValidTarget(E.Range) && CanCastE(jungleMobs))
                {
                    E.CastOnUnit(jungleMobs);
                }
                if (jungleMobs != null && getCheckBoxItem(jungleClearMenu, "JungleClearQ3") && Q3.IsReady() && jungleMobs.LSIsValidTarget(1000) && Q3READY() && Q3.IsInRange(jungleMobs))
                {
                    CastQ3(jungleMobs);
                }
                if (jungleMobs != null && getCheckBoxItem(jungleClearMenu, "JungleClearQ12") && Q.IsReady() && jungleMobs.LSIsValidTarget(500) && !Q3READY() && Q.IsInRange(jungleMobs))
                {
                    CastQ12(jungleMobs);
                }
            }
        }
        public static void UseItems(Obj_AI_Base unit)
        {
            if (Items.HasItem((int)ItemId.Blade_of_the_Ruined_King, myHero) && Items.CanUseItem((int)ItemId.Blade_of_the_Ruined_King)
               && getCheckBoxItem(comboMenu, "comboItems") && HealthPercent <= getSliderItem(comboMenu, "myHP"))
            {
                Items.UseItem((int)ItemId.Blade_of_the_Ruined_King, unit);
            }
            if (Items.HasItem((int)ItemId.Bilgewater_Cutlass, myHero) && Items.CanUseItem((int)ItemId.Bilgewater_Cutlass)
               && unit.LSIsValidTarget(Q.Range))
            {
                Items.UseItem((int)ItemId.Bilgewater_Cutlass, unit);
            }
            if (Items.HasItem((int)ItemId.Youmuus_Ghostblade, myHero) && Items.CanUseItem((int)ItemId.Youmuus_Ghostblade)
               && myHero.LSDistance(unit.Position) <= Q.Range)
            {
                Items.UseItem((int)ItemId.Youmuus_Ghostblade);
            }
            if (Items.HasItem((int)ItemId.Ravenous_Hydra_Melee_Only, myHero) && Items.CanUseItem((int)ItemId.Ravenous_Hydra_Melee_Only)
               && myHero.LSDistance(unit.Position) <= 400)
            {
                Items.UseItem((int)ItemId.Ravenous_Hydra_Melee_Only);
            }
            if (Items.HasItem((int)ItemId.Tiamat_Melee_Only, myHero) && Items.CanUseItem((int)ItemId.Tiamat_Melee_Only)
               && myHero.LSDistance(unit.Position) <= 400)
            {
                Items.UseItem((int)ItemId.Tiamat_Melee_Only);
            }
            if (Items.HasItem((int)ItemId.Randuins_Omen, myHero) && Items.CanUseItem((int)ItemId.Randuins_Omen)
               && myHero.LSDistance(unit.Position) <= 400)
            {
                Items.UseItem((int)ItemId.Randuins_Omen);
            }
        }
        public static void AutoR()
        {
            if (!Program.R.IsReady())
            {
                return;
            }

            var useR = getCheckBoxItem(comboMenu, "R4");
            var autoREnemies = getSliderItem(comboMenu, "R5");
            var MyHP = getSliderItem(comboMenu, "R7");
            var enemyInRange = getSliderItem(comboMenu, "R6");
            //var useRDown = SubMenu["Combo"]["AutoR3"].Cast<Slider>().CurrentValue;

            if (!useR)
            {
                return;
            }

            var enemiesKnockedUp =
                ObjectManager.Get<AIHeroClient>()
                    .Where(x => x.LSIsValidTarget(Program.R.Range))
                    .Where(x => x.HasBuffOfType(BuffType.Knockup) || x.HasBuffOfType(BuffType.Knockback) && x.IsEnemy);

            var enemies = enemiesKnockedUp as IList<AIHeroClient> ?? enemiesKnockedUp.ToList();

            if (enemies.Count() >= autoREnemies && Program.myHero.Health >= MyHP && myHero.CountEnemiesInRange(1500) <= enemyInRange)
            {
                Program.R.Cast();
            }
        }
        public static void Combo()
        {
            var TsTarget = TargetSelector.GetTarget(1300, DamageType.Physical);

            if (TsTarget == null || TsTarget.CharData.BaseSkinName == "gangplankbarrel")
            {
                return;
            }

            if (TsTarget != null && getCheckBoxItem(comboMenu, "QC"))
            {
                if (Q3READY() && Q3.IsReady() && Q3.IsInRange(TsTarget) && !IsDashing)
                {
                    PredictionOutput Q3Pred = Q3.GetPrediction(TsTarget);
                    if (Q3.IsInRange(TsTarget) && Q3Pred.Hitchance >= HitChance.VeryHigh) 
                    {
                        Q3.Cast(TsTarget);
                    }
                }
                if (!Q3READY() && Q.IsReady() && Q.IsInRange(TsTarget))
                {
                    PredictionOutput QPred = Q.GetPrediction(TsTarget);
                    if (Q.IsInRange(TsTarget) && QPred.Hitchance >= HitChance.High)
                    {
                        Q.Cast(TsTarget);
                    }
                } 
            }
            if (getCheckBoxItem(smartWMenu, "smartW"))
            {
                putWallBehind(TsTarget);
            }
            if (getCheckBoxItem(smartWMenu, "smartW") && wallCasted && myHero.LSDistance(TsTarget.Position) < 300)
            {
                eBehindWall(TsTarget);
            }
            if (getCheckBoxItem(comboMenu, "EC") && TsTarget != null)
            {
                var dmg = ((float)myHero.GetSpellDamage(TsTarget, SpellSlot.Q) + (float)myHero.GetSpellDamage(TsTarget, SpellSlot.E) + (float)myHero.GetSpellDamage(TsTarget, SpellSlot.R));
                if (E.IsReady() && TsTarget.LSDistance(myHero) >= (getSliderItem(comboMenu, "E1")) && dmg >= TsTarget.Health && UnderTower(PosAfterE(TsTarget)) && CanCastE(TsTarget) && myHero.LSIsFacing(TsTarget))
                {
                    E.CastOnUnit(TsTarget);
                }
                else if(TsTarget.LSDistance(myHero) >= (getSliderItem(comboMenu, "E1")) && dmg <= TsTarget.Health && CanCastE(TsTarget) && myHero.LSIsFacing(TsTarget))
                {
                    useENormal(TsTarget);
                }
                else if (Q.IsReady() && IsDashing && myHero.LSDistance(TsTarget) <= 275 * 275)
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(200, () => { CastQ12(TsTarget); } );
                }
                else if (Q3.IsReady() && myHero.LSDistance(TsTarget) <= E.Range && Q3READY() && TsTarget != null && E.IsReady() && CanCastE(TsTarget))
                {
                    E.CastOnUnit(TsTarget, true);
                }
                else if (Q3.IsReady() && IsDashing && myHero.LSDistance(TsTarget) <= 275 * 275 && Q3READY())
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(200, () => { CastQ3(TsTarget); });
                }           

                if (getCheckBoxItem(comboMenu, "E3") && E.IsReady())
                {
                    var bestMinion =
                    EntityManager.MinionsAndMonsters.CombinedAttackable
                    .Where(x => x.LSIsValidTarget(E.Range))
                    .Where(x => x.LSDistance(TsTarget) < myHero.LSDistance(TsTarget))
                    .OrderByDescending(x => x.LSDistance(myHero))
                    .FirstOrDefault();
                    var dmg2 = ((float)myHero.GetSpellDamage(TsTarget, SpellSlot.Q) + (float)myHero.GetSpellDamage(TsTarget, SpellSlot.E) + (float)myHero.GetSpellDamage(TsTarget, SpellSlot.R));
                    if (bestMinion != null && TsTarget != null && dmg2 >= TsTarget.Health && UnderTower(PosAfterE(bestMinion)) && myHero.LSIsFacing(bestMinion) && TsTarget.LSDistance(myHero) >= (getSliderItem(comboMenu, "E2")) && CanCastE(bestMinion) && myHero.LSIsFacing(bestMinion))
                    {
                        E.CastOnUnit(bestMinion, true);
                    }
                    else if (bestMinion != null && TsTarget != null && dmg2 <= TsTarget.Health && myHero.LSIsFacing(bestMinion) && TsTarget.LSDistance(myHero) >= (getSliderItem(comboMenu, "E2")) && CanCastE(bestMinion) && myHero.IsFacing(bestMinion))
                    {
                        useENormal(bestMinion);
                    }
                }
                if (!getCheckBoxItem(comboMenu, "E3") && E.IsReady())
                {
                       var bestMinion =
                       EntityManager.MinionsAndMonsters.CombinedAttackable
                      .Where(x => x.LSIsValidTarget(E.Range))
                      .Where(x => x.LSDistance(Game.CursorPos) < ObjectManager.Player.LSDistance(Game.CursorPos))
                      .OrderByDescending(x => x.LSDistance(myHero))
                      .FirstOrDefault();

                    var dmg3 = ((float)myHero.GetSpellDamage(TsTarget, SpellSlot.Q) + (float)myHero.GetSpellDamage(TsTarget, SpellSlot.E) + (float)myHero.GetSpellDamage(TsTarget, SpellSlot.R));
                    if (bestMinion != null && TsTarget != null && dmg3 >= TsTarget.Health && UnderTower(PosAfterE(bestMinion)) && myHero.LSIsFacing(bestMinion) && TsTarget.LSDistance(myHero) >= (getSliderItem(comboMenu, "E2")) && CanCastE(bestMinion) && myHero.LSIsFacing(bestMinion))
                    {
                        E.CastOnUnit(bestMinion, true);
                    }
                    else if (bestMinion != null && TsTarget != null && dmg3 <= TsTarget.Health && myHero.IsFacing(bestMinion) && TsTarget.LSDistance(myHero) >= (getSliderItem(comboMenu, "E2")) && CanCastE(bestMinion) && myHero.LSIsFacing(bestMinion))
                    {
                        useENormal(bestMinion);
                    }
                }
            }
            /*if (Config.Item("flash") && E.IsReady() && R.IsReady() && Flash.IsReady())
            {
                if (TsTarget == null)
                {
                    return;
                }
                var flashQ3range = ((Flash.Range + E.Range) - 25);
                if (Flash != null && TsTarget != null && myHero.LSDistance(TsTarget) <= flashQ3range && TsTarget.CountEnemiesInRange(400) >= Config.Item("flash2") 
                    && myHero.CountAlliesInRange(1000) >= Config.Item("flash3"))
                {
                    myHero.Spellbook.CastSpell(Flash.Slot, TsTarget.ServerPosition);
                    Utility.DelayAction.Add(10, () => { E.CastOnUnit(TsTarget, true); });
                    Utility.DelayAction.Add(200, () => { CastQ3(TsTarget); });
                }
            }*/

            if (Program.R.IsReady() && getCheckBoxItem(comboMenu, "R"))
            {
                List<AIHeroClient> enemies = HeroManager.Enemies;
                foreach (AIHeroClient enemy in enemies)
                {
                    if (ObjectManager.Player.LSDistance(enemy) <= 1200)
                    {
                        var enemiesKnockedUp =
                            ObjectManager.Get<AIHeroClient>()
                            .Where(x => x.LSIsValidTarget(Program.R.Range))
                            .Where(x => x.HasBuffOfType(BuffType.Knockup));

                        var enemiesKnocked = enemiesKnockedUp as IList<AIHeroClient> ?? enemiesKnockedUp.ToList();
                        if (enemy.LSIsValidTarget(Program.R.Range) && Program.CanCastDelayR(enemy) && enemiesKnocked.Count() >= (getSliderItem(comboMenu, "R2")))
                        {
                            Program.R.Cast();
                        }
                    }
                    if (enemy.LSIsValidTarget(Program.R.Range))
                    {
                        
                        if (Program.IsKnockedUp(enemy) && Program.CanCastDelayR(enemy) && enemy.Health <= ((getSliderItem(comboMenu, "R1") / 100 * enemy.MaxHealth) * 1.5f) && getCheckBoxItem(ultMenu, enemy.ChampionName))
                        {
                            Program.R.Cast();
                        }
                        else if (Program.IsKnockedUp(enemy) && Program.CanCastDelayR(enemy) && enemy.Health >= ((getSliderItem(comboMenu, "R1") / 100 * enemy.MaxHealth) * 1.5f) && (getCheckBoxItem(comboMenu, "R3")))
                        {
                            if (Program.AlliesNearTarget(enemy, 600))
                            {
                                Program.R.Cast();
                            }
                        }
                    }
                }
                if (getCheckBoxItem(comboMenu, "Ignite") && TsTarget.Health <= myHero.GetSummonerSpellDamage(TsTarget, LeagueSharp.Common.Damage.SummonerSpell.Ignite))
                {
                    Ignite.Cast(TsTarget);
                }
                
                if (getCheckBoxItem(comboMenu, "comboItems") && TsTarget != null && TsTarget.LSIsValidTarget())
                {
                    UseItems(TsTarget);
                }
            }
        }
        public static void KillSteal()
        {
            foreach (AIHeroClient enemy in HeroManager.Enemies)
            {
                if (enemy.LSIsValidTarget(Q.Range) && getCheckBoxItem(miscMenu, "KS"))
                {
                    if (Q.IsReady() && !Q3READY())
                    {
                        
                        if (enemy.Health <= GetQDmg(enemy))
                        {
                            CastQ12(enemy);
                        }
                    }
                    if (Q3.IsReady() && Q3READY())
                    {

                        if (enemy.Health <= GetQDmg(enemy))
                        {
                            CastQ3(enemy);
                        }
                    }
                    if (!Q.IsReady() && E.IsReady() && CanCastE(enemy))
                    {
                        
                        if (enemy.Health <= GetEDmg(enemy))
                        {
                            E.CastOnUnit(enemy, true);
                        }
                    }
                    if (Ignite != null && Ignite.IsReady())
                    {
                        
                        if (enemy.Health <= Program.myHero.GetSummonerSpellDamage(enemy, LeagueSharp.Common.Damage.SummonerSpell.Ignite))
                        {
                            Program.Ignite.Cast(enemy);
                        }
                    }
                }
            }
        }
        public static void CastQ12(Obj_AI_Base target)
        {
            if (target == null)
            {
                return;
            }
            PredictionOutput QPred = Q.GetPrediction(target, true);
            if (QPred.Hitchance >= HitChance.Medium && Q.IsInRange(target))
            {
                Q.Cast(QPred.CastPosition, true);
            }
        }
        public static void CastQ3(Obj_AI_Base target)
        {
            if (target == null)
            {
                return;
            }
            PredictionOutput Q3Pred = Q3.GetPrediction(target, true);
            if (Q3Pred.Hitchance >= HitChance.Medium && Q3.IsInRange(target))
            {
                Q.Cast(Q3Pred.CastPosition, true);
            }
        }
        public static void CastQ3AoE()
        {
            foreach (AIHeroClient target in HeroManager.Enemies.Where(x => x.LSIsValidTarget(1100)))
            {
                PredictionOutput Q3Pred = Q3.GetPrediction(target, true);
                if (Q3Pred.Hitchance >= HitChance.Medium && Q3.IsInRange(target) && Q3Pred.AoeTargetsHitCount >= 2)
                {
                    Q3.Cast(Q3Pred.CastPosition, true);
                }
            }          
        }
        public static bool IsKnockedUp(AIHeroClient target)
        {
            return target.HasBuffOfType(BuffType.Knockup) || target.HasBuffOfType(BuffType.Knockback);
        }

        public static bool AlliesNearTarget(Obj_AI_Base target, float range)
        {
            return HeroManager.Allies.Where(tar => tar.LSDistance(target) < range).Any(tar => tar != null);
        }
        public static bool isDangerous(Obj_AI_Base target, float range)
        {
            return HeroManager.Enemies.Where(tar => tar.LSDistance(PosAfterE(target)) < range).Any(tar => tar != null);
        }

        //copy pasta from valvesharp
        private static bool CanCastDelayR(AIHeroClient target)
        {
            var buff = target.Buffs.FirstOrDefault(i => i.Type == BuffType.Knockback || i.Type == BuffType.Knockup);
            return buff != null && buff.EndTime - Game.Time <= (buff.EndTime - buff.StartTime) / (buff.EndTime - buff.StartTime <= 0.5 ? 1.5 : 3);
        }
        public static Vector2 PosAfterE(Obj_AI_Base target)
        {
            return myHero.ServerPosition.LSExtend(target.ServerPosition, myHero.LSDistance(target) < 410 ? E.Range : myHero.LSDistance(target) + 65).LSTo2D();
        }
        public static bool UnderTower(Vector2 pos)
        {
            return ObjectManager.Get<Obj_AI_Turret>().Any(i => i.Health > 0 && i.LSDistance(pos) <= 950 && i.IsEnemy);
        }
        public static bool IsDashing
        {
            get
            {
                return isDashing || myHero.LSIsDashing();
            }
        }
        public static bool Q3READY()
        {
            return ObjectManager.Player.HasBuff("YasuoQ3W");
        }

        public static bool CanCastE(Obj_AI_Base target)
        {
            return !target.HasBuff("YasuoDashWrapper");
        }
        public static double GetEDmg(Obj_AI_Base target)
        {
            var stacksPassive = myHero.Buffs.Find(b => b.DisplayName.Equals("YasuoDashScalar"));
            var Estacks = (stacksPassive != null) ? stacksPassive.Count : 0 ;
            var damage = ((E.Level * 20) + 50) * (1 + 0.25 * Estacks) + (myHero.FlatMagicDamageMod * 0.6);
            return myHero.CalcDamage(target, DamageType.Magical, damage);
        }
        public static Vector2 getNextPos(AIHeroClient target)
        {
            Vector2 dashPos = target.Position.LSTo2D();
            if (target.IsMoving && target.Path.Count() != 0)
            {
                Vector2 tpos = target.Position.LSTo2D();
                Vector2 path = target.Path[0].LSTo2D() - tpos;
                path.Normalize();
                dashPos = tpos + (path * 100);
            }
            return dashPos;
        }
        public static void putWallBehind(AIHeroClient target)
        {
            if (!W.IsReady() || !E.IsReady() || target.IsMelee())
                return;
            Vector2 dashPos = getNextPos(target);
            PredictionOutput po = LeagueSharp.Common.Prediction.GetPrediction(target, 0.5f);

            float dist = myHero.LSDistance(po.UnitPosition);
            if (!target.IsMoving || myHero.LSDistance(dashPos) <= dist + 40)
                if (dist < 330 && dist > 100 && W.IsReady() && getCheckBoxItem(wwMenu, target.ChampionName))
                {
                    W.Cast(po.UnitPosition, true);
                }
        }

        public static void eBehindWall(AIHeroClient target)
        {
            if (!E.IsReady() || !enemyIsJumpable(target) || target.IsMelee())
                return;
            float dist = myHero.LSDistance(target);
            var pPos = myHero.Position.LSTo2D();
            Vector2 dashPos = target.Position.LSTo2D();
            if (!target.IsMoving || myHero.LSDistance(dashPos) <= dist)
            {
                foreach (Obj_AI_Base enemy in ObjectManager.Get<Obj_AI_Base>().Where(enemy => enemyIsJumpable(enemy)))
                {
                    Vector2 posAfterE = pPos + (Vector2.Normalize(enemy.Position.LSTo2D() - pPos) * E.Range);
                    if ((target.LSDistance(posAfterE) < dist
                        || target.LSDistance(posAfterE) < Orbwalking.GetRealAutoAttackRange(target) + 100)
                        && goesThroughWall(target.Position, posAfterE.To3D()))
                    {
                        if (useENormal(target))
                            return;
                    }
                }
            }
        }


        public static IsSafeResult isSafePoint(Vector2 point, bool igonre = false)
        {
            var result = new IsSafeResult();
            result.SkillshotList = new List<Skillshot>();
            result.casters = new List<Obj_AI_Base>();


            bool safe = Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) || point.To3D().GetEnemiesInRange(500).Count > myHero.HealthPercent % 65;
            if (!safe)
            {
                result.IsSafe = false;
                return result;
            }
            foreach (var skillshot in EvadeDetectedSkillshots)
            {
                if (skillshot.IsDanger(point) && skillshot.IsAboutToHit(500, myHero))
                {
                    result.SkillshotList.Add(skillshot);
                    result.casters.Add(skillshot.Unit);
                }
            }

            result.IsSafe = (result.SkillshotList.Count == 0);
            return result;
        }

        public static Obj_AI_Minion GetCandidates(AIHeroClient player, List<Skillshot> skillshots)
        {
            float currentDashSpeed = 700 + player.MoveSpeed;//At least has to be like this
            IEnumerable<Obj_AI_Minion> minions = ObjectManager.Get<Obj_AI_Minion>();
            Obj_AI_Minion candidate = new Obj_AI_Minion();
            double closest = 10000000000000;
            foreach (Obj_AI_Minion minion in minions)
            {
                if (Vector2.Distance(player.Position.LSTo2D(), minion.Position.LSTo2D()) < 475 && minion.IsEnemy && enemyIsJumpable(minion) && closest > Vector3.DistanceSquared(Game.CursorPos, minion.Position))
                {
                    foreach (Skillshot skillshot in skillshots)
                    {
                        //Get intersection point
                        //  Vector2 intersectionPoint = LineIntersectionPoint(startPos, player.Position.LSTo2D(), endPos, V2E(player.Position, minion.Position, 475));
                        //Time when yasuo will be in intersection point
                        //  float arrivingTime = Vector2.LSDistance(player.Position.LSTo2D(), intersectionPoint) / currentDashSpeed;
                        //Estimated skillshot position
                        //  Vector2 skillshotPosition = V2E(startPos.To3D(), intersectionPoint.To3D(), speed * arrivingTime);
                        if (skillshot.IsDanger(V2E(player.Position, minion.Position, 475)))
                        {
                            candidate = minion;
                            closest = Vector3.DistanceSquared(Game.CursorPos, minion.Position);
                        }
                    }
                }
            }
            return candidate;
        }

        private static Vector2 V2E(Vector3 from, Vector3 direction, float distance)
        {
            return (from + distance * Vector3.Normalize(direction - from)).LSTo2D();
        }
        public static Vector2 LineIntersectionPoint(Vector2 ps1, Vector2 pe1, Vector2 ps2,
                Vector2 pe2)
        {
            // Get A,B,C of first line - points : ps1 to pe1
            float A1 = pe1.Y - ps1.Y;
            float B1 = ps1.X - pe1.X;
            float C1 = A1 * ps1.X + B1 * ps1.Y;

            // Get A,B,C of second line - points : ps2 to pe2
            float A2 = pe2.Y - ps2.Y;
            float B2 = ps2.X - pe2.X;
            float C2 = A2 * ps2.X + B2 * ps2.Y;

            // Get delta and check if the lines are parallel
            float delta = A1 * B2 - A2 * B1;
            if (delta == 0)
                return new Vector2(-1, -1);

            // now return the Vector2 intersection point
            return new Vector2(
                (B2 * C1 - B1 * C2) / delta,
                (A1 * C2 - A2 * C1) / delta
            );
        }
        public static bool willColide(Skillshot ss, Vector2 from, float speed, Vector2 direction, float radius)
        {
            Vector2 ssVel = ss.Direction.Normalized() * ss.SpellData.MissileSpeed;
            Vector2 dashVel = direction * speed;
            Vector2 a = ssVel - dashVel;//true direction + speed
            Vector2 realFrom = from.LSExtend(direction, ss.SpellData.Delay + speed);
            if (!ss.IsAboutToHit((int)((dashVel.Length() / 475) * 1000) + Game.Ping + 100, ObjectManager.Player))
                return false;
            if (ss.IsAboutToHit(1000, ObjectManager.Player) && interCir(ss.MissilePosition, ss.MissilePosition.LSExtend(ss.MissilePosition + a, ss.SpellData.Range + 50), from,
                radius))
                return true;
            return false;
        }
        public static bool interCir(Vector2 E, Vector2 L, Vector2 C, float r)
        {
            Vector2 d = L - E;
            Vector2 f = E - C;

            float a = Vector2.Dot(d, d);
            float b = 2 * Vector2.Dot(f, d);
            float c = Vector2.Dot(f, f) - r * r;

            float discriminant = b * b - 4 * a * c;
            if (discriminant < 0)
            {
                // no intersection
            }
            else
            {
                // ray didn't totally miss sphere,
                // so there is a solution to
                // the equation.

                discriminant = (float)Math.Sqrt(discriminant);

                // either solution may be on or off the ray so need to test both
                // t1 is always the smaller value, because BOTH discriminant and
                // a are nonnegative.
                float t1 = (-b - discriminant) / (2 * a);
                float t2 = (-b + discriminant) / (2 * a);

                // 3x HIT cases:
                //          -o->             --|-->  |            |  --|->
                // Impale(t1 hit,t2 hit), Poke(t1 hit,t2>1), ExitWound(t1<0, t2 hit), 

                // 3x MISS cases:
                //       ->  o                     o ->              | -> |
                // FallShort (t1>1,t2>1), Past (t1<0,t2<0), CompletelyInside(t1<0, t2>1)

                if (t1 >= 0 && t1 <= 1)
                {
                    // t1 is the intersection, and it's closer than t2
                    // (since t1 uses -b - discriminant)
                    // Impale, Poke
                    return true;
                }

                // here t1 didn't intersect so we are either started
                // inside the sphere or completely past it
                if (t2 >= 0 && t2 <= 1)
                {
                    // ExitWound
                    return true;
                }

                // no intn: FallShort, Past, CompletelyInside
                return false;
            }
            return false;
        }
        public static bool wontHitOnDash(Skillshot ss, Obj_AI_Base jumpOn, Skillshot skillShot, Vector2 dashDir)
        {
            float currentDashSpeed = 700 + myHero.MoveSpeed;//At least has to be like this
            //Get intersection point
            Vector2 intersectionPoint = LineIntersectionPoint(myHero.Position.LSTo2D(), V2E(myHero.Position, jumpOn.Position, 475), ss.Start, ss.End);
            //Time when yasuo will be in intersection point
            float arrivingTime = Vector2.Distance(myHero.Position.LSTo2D(), intersectionPoint) / currentDashSpeed;
            //Estimated skillshot position
            Vector2 skillshotPosition = ss.GetMissilePosition((int)(arrivingTime * 1000));
            if (Vector2.DistanceSquared(skillshotPosition, intersectionPoint) <
                (ss.SpellData.Radius + myHero.BoundingRadius) && !willColide(skillShot, myHero.Position.LSTo2D(), 700f + myHero.MoveSpeed, dashDir, myHero.BoundingRadius + skillShot.SpellData.Radius))
                return false;
            return true;
        }

        public static void useEtoSafe(Skillshot skillShot)
        {
            if (!E.IsReady())
                return;
            float closest = float.MaxValue;
            Obj_AI_Base closestTarg = null;
            float currentDashSpeed = 700 + myHero.MoveSpeed;
            foreach (Obj_AI_Base enemy in ObjectManager.Get<Obj_AI_Base>().Where(ob => ob.NetworkId != skillShot.Unit.NetworkId && enemyIsJumpable(ob) && ob.LSDistance(myHero) < E.Range).OrderBy(ene => ene.LSDistance(Game.CursorPos, true)))
            {
                var pPos = myHero.Position.LSTo2D();
                Vector2 posAfterE = V2E(myHero.Position, enemy.Position, 475);
                Vector2 dashDir = (posAfterE - myHero.Position.LSTo2D()).Normalized();

                if (isSafePoint(posAfterE).IsSafe && wontHitOnDash(skillShot, enemy, skillShot, dashDir) /*&& skillShot.IsSafePath(new List<Vector2>() { posAfterE }, 0, (int)currentDashSpeed, 0).IsSafe*/)
                {
                    float curDist = Vector2.DistanceSquared(Game.CursorPos.LSTo2D(), posAfterE);
                    if (curDist < closest)
                    {
                        closestTarg = enemy;
                        closest = curDist;
                    }
                }
            }
            if (closestTarg != null && closestTarg.CountEnemiesInRange(600) <= 2 && skillShotMenu["DangerLevel" + skillShot.SpellData.MenuItemName] != null && skillShotMenu["DangerLevel" + skillShot.SpellData.MenuItemName].Cast<Slider>().CurrentValue >= getSliderItem(smartWMenu, "smartEDogueDanger"))
                useENormal(closestTarg);
        }

        public static void useWSmart(Skillshot skillShot)
        {
            //try douge with E if cant windWall
            var WDelay = getSliderItem(smartWMenu, "smartWDelay");

            if (!W.IsReady() || skillShot.SpellData.Type == SkillShotType.SkillshotCircle || skillShot.SpellData.Type == SkillShotType.SkillshotRing)
                return;
            if (skillShot.IsAboutToHit(WDelay, myHero))
            {
                var sd = SpellDatabase.GetByMissileName(skillShot.SpellData.MissileSpellName);
                if (sd == null)
                    return;

                //If enabled
                if (!EvadeSpellEnabled(sd.MenuItemName))
                    return;

                //if only dangerous
                if (getCheckBoxItem(smartWMenu, "wwDanger") && skillShotIsDangerous(sd.MenuItemName))
                {
                    myHero.Spellbook.CastSpell(SpellSlot.W, skillShot.Start.To3D(), skillShot.Start.To3D());
                }
                if (!getCheckBoxItem(smartWMenu, "wwDanger") && skillShotMenu["DangerLevel" + sd.MenuItemName] != null && skillShotMenu["DangerLevel" + sd.MenuItemName].Cast<Slider>().CurrentValue >= getSliderItem(smartWMenu, "smartWDanger"))
                {
                    myHero.Spellbook.CastSpell(SpellSlot.W, skillShot.Start.To3D(), skillShot.Start.To3D());
                }
            }
        }

        public static bool enemyIsJumpable(Obj_AI_Base enemy, List<AIHeroClient> ignore = null)
        {
            if (enemy.IsValid && enemy.IsEnemy && !enemy.IsInvulnerable && !enemy.MagicImmune && !enemy.IsDead && !(enemy is FollowerObject))
            {
                if (ignore != null)
                    foreach (AIHeroClient ign in ignore)
                    {
                        if (ign.NetworkId == enemy.NetworkId)
                            return false;
                    }
                foreach (BuffInstance buff in enemy.Buffs)
                {
                    if (buff.Name == "YasuoDashWrapper")
                        return false;
                }
                return true;
            }
            return false;
        }
        public static void setUpWall()
        {
            if (wall == null)
                return;

        }     

        public static bool useENormal(Obj_AI_Base target)
        {
            if (!E.IsReady() || target.LSDistance(myHero) > 470)
                return false;
            Vector2 posAfter = V2E(myHero.Position, target.Position, 475);
            if (!getCheckBoxItem(miscMenu, "ETower"))
            {
                if (isSafePoint(posAfter).IsSafe)
                {
                    E.Cast(target, true, false);
                }
                return true;
            }
            else
            {
                Vector2 pPos = myHero.ServerPosition.LSTo2D();
                Vector2 posAfterE = pPos + (Vector2.Normalize(target.Position.LSTo2D() - pPos) * E.Range);
                if (!(posAfterE.To3D().UnderTurret(true)))
                {
                    Console.WriteLine("use gap?");
                    if (isSafePoint(posAfter, true).IsSafe)
                    {
                        E.Cast(target, true, false);
                    }
                    return true;
                }
            }
            return false;

        }
        public static bool goesThroughWall(Vector3 vec1, Vector3 vec2)
        {
            if (wall.endtime < Game.Time || wall.pointL == null || wall.pointL == null)
                return false;
            Vector2 inter = LineIntersectionPoint(vec1.LSTo2D(), vec2.LSTo2D(), wall.pointL.Position.LSTo2D(), wall.pointR.Position.LSTo2D());
            float wallW = (300 + 50 * W.Level);
            if (wall.pointL.Position.LSTo2D().LSDistance(inter) > wallW ||
                wall.pointR.Position.LSTo2D().LSDistance(inter) > wallW)
                return false;
            var dist = vec1.LSDistance(vec2);
            if (vec1.LSTo2D().LSDistance(inter) + vec2.LSTo2D().LSDistance(inter) - 30 > dist)
                return false;

            return true;
        }
        public static Menu getSkilshotMenu()
        {
            //Create the skillshots submenus.
            var skillShots = Config.AddSubMenu("Enemy Skillshots", "aShotsSkills");

            foreach (var hero in ObjectManager.Get<AIHeroClient>())
            {
                if (hero.Team != ObjectManager.Player.Team)
                {
                    foreach (var spell in SpellDatabase.Spells)
                    {
                        if (spell.ChampionName == hero.ChampionName)
                        {
                            skillShots.AddGroupLabel(spell.MenuItemName);
                            skillShots.Add("DangerLevel" + spell.MenuItemName, new Slider("Danger level", spell.DangerValue, 1, 5));
                            skillShots.Add("IsDangerous" + spell.MenuItemName, new CheckBox("Is Dangerous", spell.IsDangerous));
                            skillShots.Add("Enabled" + spell.MenuItemName, new CheckBox("Enabled"));
                        }
                    }
                }
            }
            return skillShots;
        }

        public static bool skillShotIsDangerous(string Name)
        {
            if (skillShotMenu["IsDangerous" + Name] != null)
            {
                return skillShotMenu["IsDangerous" + Name].Cast<CheckBox>().CurrentValue;
            }
            return true;
        }

        public static bool EvadeSpellEnabled(string Name)
        {
            if (skillShotMenu["Enabled" + Name] != null)
            {
                return skillShotMenu["Enabled" + Name].Cast<CheckBox>().CurrentValue;
            }
            return true;
        }

        public static void updateSkillshots()
        {
            foreach (var ss in EvadeDetectedSkillshots)
            {
                ss.Game_OnGameUpdate();
            }
        }

        private static void OnDeleteMissile(Skillshot skillshot, MissileClient missile)
        {
            if (skillshot.SpellData.SpellName == "VelkozQ")
            {
                var spellData = SpellDatabase.GetByName("VelkozQSplit");
                var direction = skillshot.Direction.Perpendicular();
                if (EvadeDetectedSkillshots.Count(s => s.SpellData.SpellName == "VelkozQSplit") == 0)
                {
                    for (var i = -1; i <= 1; i = i + 2)
                    {
                        var skillshotToAdd = new Skillshot(
                            DetectionType.ProcessSpell, spellData, Environment.TickCount, missile.Position.LSTo2D(),
                            missile.Position.LSTo2D() + i * direction * spellData.Range, skillshot.Unit);
                        EvadeDetectedSkillshots.Add(skillshotToAdd);
                    }
                }
            }
        }
        private static double GetQDmg(Obj_AI_Base target)
        {
            var dmgItem = 0d;
            if (Items.HasItem(3057) && (Items.CanUseItem(3057) || myHero.HasBuff("Sheen")))
            {
                dmgItem = myHero.BaseAttackDamage;
            }
            if (Items.HasItem(3078) && (Items.CanUseItem(3078) || myHero.HasBuff("Sheen")))
            {
                dmgItem = myHero.BaseAttackDamage * 2;
            }
            var damageModifier = 1d;
            var reduction = 0d;
            var result = dmgItem
                         + myHero.TotalAttackDamage * (myHero.Crit >= 0.85f ? (Items.HasItem(3031) ? 1.875 : 1.5) : 1);
            if (Items.HasItem(3153))
            {
                var dmgBotrk = Math.Max(0.08 * target.Health, 10);
                result += target is Obj_AI_Minion ? Math.Min(dmgBotrk, 60) : dmgBotrk;
            }
            var targetHero = target as AIHeroClient;
            if (targetHero != null)
            {
                if (Items.HasItem(3047, targetHero))
                {
                    damageModifier *= 0.9d;
                }
                if (targetHero.ChampionName == "Fizz")
                {
                    reduction += 4 + (targetHero.Level - 1 / 3) * 2;
                }
                var mastery = targetHero.Masteries.FirstOrDefault(i => i.Page == MasteryPage.Defense && i.Id == 68);
                if (mastery != null && mastery.Points >= 1)
                {
                    reduction += 1 * mastery.Points;
                }
            }
            return myHero.CalcDamage(
                target,
                DamageType.Physical,
                20 * Q.Level + (result - reduction) * damageModifier)
                   + (HaveStatik
                          ? myHero.CalcDamage(
                              target,
                              DamageType.Magical,
                              100 * (myHero.Crit >= 0.85f ? (Items.HasItem(3031) ? 2.25 : 1.8) : 1))
                          : 0);
        }
        private static bool HaveStatik
        {
            get
            {
                return myHero.GetBuffCount("ItemStatikShankCharge") == 100;
            }
        }
        public static List<Vector2> GetCastMinionsPredictedPositions(List<Obj_AI_Base> minions,
            float delay,
            float width,
            float speed,
            Vector3 from,
            float range,
            bool collision,
            SkillshotType stype,
            Vector3 rangeCheckFrom = new Vector3())
        {
            var result = new List<Vector2>();
            from = from.LSTo2D().IsValid() ? from : ObjectManager.Player.ServerPosition;
            foreach (var minion in minions)
            {
                var pos = LeagueSharp.Common.Prediction.GetPrediction(new PredictionInput
                {
                    Unit = minion,
                    Delay = delay,
                    Radius = width,
                    Speed = speed,
                    From = from,
                    Range = range,
                    Collision = collision,
                    Type = stype,
                    RangeCheckFrom = rangeCheckFrom
                });

                if (pos.Hitchance >= HitChance.High)
                {
                    result.Add(pos.CastPosition.LSTo2D());
                }
            }

            return result;
        }
        private static void OnDetectSkillshot(Skillshot skillshot)
        {
            var alreadyAdded = false;

            foreach (var item in EvadeDetectedSkillshots)
            {
                if (item.SpellData.SpellName == skillshot.SpellData.SpellName &&
                    (item.Unit.NetworkId == skillshot.Unit.NetworkId &&
                     (skillshot.Direction).AngleBetween(item.Direction) < 5 &&
                     (skillshot.Start.LSDistance(item.Start) < 100 || skillshot.SpellData.FromObjects.Length == 0)))
                {
                    alreadyAdded = true;
                }
            }

            //Check if the skillshot is from an ally.
            if (skillshot.Unit.Team == ObjectManager.Player.Team)
            {
                return;
            }

            //Check if the skillshot is too far away.
            if (skillshot.Start.LSDistance(ObjectManager.Player.ServerPosition.LSTo2D()) >
                (skillshot.SpellData.Range + skillshot.SpellData.Radius + 1000) * 1.5)
            {
                return;
            }

            //Add the skillshot to the detected skillshot list.
            if (!alreadyAdded)
            {
                //Multiple skillshots like twisted fate Q.
                if (skillshot.DetectionType == DetectionType.ProcessSpell)
                {
                    if (skillshot.SpellData.MultipleNumber != -1)
                    {
                        var originalDirection = skillshot.Direction;

                        for (var i = -(skillshot.SpellData.MultipleNumber - 1) / 2;
                            i <= (skillshot.SpellData.MultipleNumber - 1) / 2;
                            i++)
                        {
                            var end = skillshot.Start +
                                      skillshot.SpellData.Range *
                                      originalDirection.Rotated(skillshot.SpellData.MultipleAngle * i);
                            var skillshotToAdd = new Skillshot(
                                skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, skillshot.Start, end,
                                skillshot.Unit);

                            EvadeDetectedSkillshots.Add(skillshotToAdd);
                        }
                        return;
                    }

                    if (skillshot.SpellData.SpellName == "UFSlash")
                    {
                        skillshot.SpellData.MissileSpeed = 1600 + (int)skillshot.Unit.MoveSpeed;
                    }

                    if (skillshot.SpellData.Invert)
                    {
                        var newDirection = -(skillshot.End - skillshot.Start).Normalized();
                        var end = skillshot.Start + newDirection * skillshot.Start.LSDistance(skillshot.End);
                        var skillshotToAdd = new Skillshot(
                            skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, skillshot.Start, end,
                            skillshot.Unit);
                        EvadeDetectedSkillshots.Add(skillshotToAdd);
                        return;
                    }

                    if (skillshot.SpellData.Centered)
                    {
                        var start = skillshot.Start - skillshot.Direction * skillshot.SpellData.Range;
                        var end = skillshot.Start + skillshot.Direction * skillshot.SpellData.Range;
                        var skillshotToAdd = new Skillshot(
                            skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, start, end,
                            skillshot.Unit);
                        EvadeDetectedSkillshots.Add(skillshotToAdd);
                        return;
                    }

                    if (skillshot.SpellData.SpellName == "SyndraE" || skillshot.SpellData.SpellName == "syndrae5")
                    {
                        var angle = 60;
                        var edge1 =
                            (skillshot.End - skillshot.Unit.ServerPosition.LSTo2D()).Rotated(
                                -angle / 2 * (float)Math.PI / 180);
                        var edge2 = edge1.Rotated(angle * (float)Math.PI / 180);

                        foreach (var minion in ObjectManager.Get<Obj_AI_Minion>())
                        {
                            var v = minion.ServerPosition.LSTo2D() - skillshot.Unit.ServerPosition.LSTo2D();
                            if (minion.Name == "Seed" && edge1.CrossProduct(v) > 0 && v.CrossProduct(edge2) > 0 &&
                                minion.LSDistance(skillshot.Unit) < 800 &&
                                (minion.Team != ObjectManager.Player.Team))
                            {
                                var start = minion.ServerPosition.LSTo2D();
                                var end = skillshot.Unit.ServerPosition.LSTo2D()
                                    .LSExtend(
                                        minion.ServerPosition.LSTo2D(),
                                        skillshot.Unit.LSDistance(minion) > 200 ? 1300 : 1000);

                                var skillshotToAdd = new Skillshot(
                                    skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, start, end,
                                    skillshot.Unit);
                                EvadeDetectedSkillshots.Add(skillshotToAdd);
                            }
                        }
                        return;
                    }

                    if (skillshot.SpellData.SpellName == "AlZaharCalloftheVoid")
                    {
                        var start = skillshot.End - skillshot.Direction.Perpendicular() * 400;
                        var end = skillshot.End + skillshot.Direction.Perpendicular() * 400;
                        var skillshotToAdd = new Skillshot(
                            skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, start, end,
                            skillshot.Unit);
                        EvadeDetectedSkillshots.Add(skillshotToAdd);
                        return;
                    }

                    if (skillshot.SpellData.SpellName == "ZiggsQ")
                    {
                        var d1 = skillshot.Start.LSDistance(skillshot.End);
                        var d2 = d1 * 0.4f;
                        var d3 = d2 * 0.69f;


                        var bounce1SpellData = SpellDatabase.GetByName("ZiggsQBounce1");
                        var bounce2SpellData = SpellDatabase.GetByName("ZiggsQBounce2");

                        var bounce1Pos = skillshot.End + skillshot.Direction * d2;
                        var bounce2Pos = bounce1Pos + skillshot.Direction * d3;

                        bounce1SpellData.Delay =
                            (int)(skillshot.SpellData.Delay + d1 * 1000f / skillshot.SpellData.MissileSpeed + 500);
                        bounce2SpellData.Delay =
                            (int)(bounce1SpellData.Delay + d2 * 1000f / bounce1SpellData.MissileSpeed + 500);

                        var bounce1 = new Skillshot(
                            skillshot.DetectionType, bounce1SpellData, skillshot.StartTick, skillshot.End, bounce1Pos,
                            skillshot.Unit);
                        var bounce2 = new Skillshot(
                            skillshot.DetectionType, bounce2SpellData, skillshot.StartTick, bounce1Pos, bounce2Pos,
                            skillshot.Unit);

                        EvadeDetectedSkillshots.Add(bounce1);
                        EvadeDetectedSkillshots.Add(bounce2);
                    }

                    if (skillshot.SpellData.SpellName == "ZiggsR")
                    {
                        skillshot.SpellData.Delay =
                            (int)(1500 + 1500 * skillshot.End.LSDistance(skillshot.Start) / skillshot.SpellData.Range);
                    }

                    if (skillshot.SpellData.SpellName == "JarvanIVDragonStrike")
                    {
                        var endPos = new Vector2();

                        foreach (var s in EvadeDetectedSkillshots)
                        {
                            if (s.Unit.NetworkId == skillshot.Unit.NetworkId && s.SpellData.Slot == SpellSlot.E)
                            {
                                endPos = s.End;
                            }
                        }

                        foreach (var m in ObjectManager.Get<Obj_AI_Minion>())
                        {
                            if (m.CharData.BaseSkinName == "jarvanivstandard" && m.Team == skillshot.Unit.Team &&
                                skillshot.IsDanger(m.Position.LSTo2D()))
                            {
                                endPos = m.Position.LSTo2D();
                            }
                        }

                        if (!endPos.IsValid())
                        {
                            return;
                        }

                        skillshot.End = endPos + 200 * (endPos - skillshot.Start).Normalized();
                        skillshot.Direction = (skillshot.End - skillshot.Start).Normalized();
                    }
                }

                if (skillshot.SpellData.SpellName == "OriannasQ")
                {
                    var endCSpellData = SpellDatabase.GetByName("OriannaQend");

                    var skillshotToAdd = new Skillshot(
                        skillshot.DetectionType, endCSpellData, skillshot.StartTick, skillshot.Start, skillshot.End,
                        skillshot.Unit);

                    EvadeDetectedSkillshots.Add(skillshotToAdd);
                }


                //Dont allow fow detection.
                if (skillshot.SpellData.DisableFowDetection && skillshot.DetectionType == DetectionType.RecvPacket)
                {
                    return;
                }
#if DEBUG
                    Console.WriteLine(Environment.TickCount + "Adding new skillshot: " + skillshot.SpellData.SpellName);
#endif

                EvadeDetectedSkillshots.Add(skillshot);
            }
        }

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

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(drawMenu, "Disable"))
            {
                return;
            }
            if (getCheckBoxItem(drawMenu, "DrawQ") && Q.IsReady())
            {
                Render.Circle.DrawCircle(myHero.Position, Q.Range, Color.LightGreen);
            }
            if (getCheckBoxItem(drawMenu, "DrawQ3") && Q3.IsReady())
            {
                Render.Circle.DrawCircle(myHero.Position, Q3.Range, Color.LightGreen);
            }
            if (getCheckBoxItem(drawMenu, "DrawW") && W.IsReady())
            {
                Render.Circle.DrawCircle(myHero.Position, W.Range, Color.LightGreen);
            }
            if (getCheckBoxItem(drawMenu, "DrawE") && E.IsReady())
            {
                Render.Circle.DrawCircle(myHero.Position, E.Range, Color.LightGreen);
            }
            if (getCheckBoxItem(drawMenu, "DrawR") && R.IsReady())
            {
                Render.Circle.DrawCircle(myHero.Position, R.Range, Color.LightGreen);
            }
            if (getCheckBoxItem(drawMenu, "DrawSpots"))
            {
                Render.Circle.DrawCircle(Yasuo.spot1.To3D(), 150, Color.Red, 2);
                Render.Circle.DrawCircle(Yasuo.spot2.To3D(), 150, Color.Red, 2);
                Render.Circle.DrawCircle(Yasuo.spot3.To3D(), 150, Color.Red, 2);
                Render.Circle.DrawCircle(Yasuo.spot4.To3D(), 150, Color.Red, 2);
                Render.Circle.DrawCircle(Yasuo.spot5.To3D(), 150, Color.Red, 2);
                Render.Circle.DrawCircle(Yasuo.spot6.To3D(), 150, Color.Red, 2);
                Render.Circle.DrawCircle(Yasuo.spot7.To3D(), 150, Color.Red, 2);
                Render.Circle.DrawCircle(Yasuo.spot8.To3D(), 150, Color.Red, 2);
                Render.Circle.DrawCircle(Yasuo.spot9.To3D(), 150, Color.Red, 2);
                Render.Circle.DrawCircle(Yasuo.spot10.To3D(), 150, Color.Red, 2);
                Render.Circle.DrawCircle(Yasuo.spot11.To3D(), 150, Color.Red, 2);
                Render.Circle.DrawCircle(Yasuo.spot12.To3D(), 150, Color.Red, 2);
                Render.Circle.DrawCircle(Yasuo.spot13.To3D(), 150, Color.Red, 2);
                Render.Circle.DrawCircle(Yasuo.spot14.To3D(), 150, Color.Red, 2);
                Render.Circle.DrawCircle(Yasuo.spot15.To3D(), 150, Color.Red, 2);
                Render.Circle.DrawCircle(Yasuo.spot16.To3D(), 150, Color.Red, 2);

                Render.Circle.DrawCircle(Yasuo.spotA.To3D(), 400, Color.Green, 1);
                Render.Circle.DrawCircle(Yasuo.spotB.To3D(), 400, Color.Green, 1);
                Render.Circle.DrawCircle(Yasuo.spotC.To3D(), 400, Color.Green, 1);
                Render.Circle.DrawCircle(Yasuo.spotD.To3D(), 400, Color.Green, 1);
                Render.Circle.DrawCircle(Yasuo.spotE.To3D(), 400, Color.Green, 1);
                Render.Circle.DrawCircle(Yasuo.spotF.To3D(), 400, Color.Green, 1);
                Render.Circle.DrawCircle(Yasuo.spotG.To3D(), 400, Color.Green, 1);
                Render.Circle.DrawCircle(Yasuo.spotH.To3D(), 400, Color.Green, 1);
                Render.Circle.DrawCircle(Yasuo.spotI.To3D(), 120, Color.Green, 1);
                Render.Circle.DrawCircle(Yasuo.spotJ.To3D(), 120, Color.Green, 1);
                Render.Circle.DrawCircle(Yasuo.spotL.To3D(), 400, Color.Green, 1);
                Render.Circle.DrawCircle(Yasuo.spotM.To3D(), 200, Color.Green, 1);
                Render.Circle.DrawCircle(Yasuo.spotN.To3D(), 400, Color.Green, 1);
                Render.Circle.DrawCircle(Yasuo.spotO.To3D(), 200, Color.Green, 1);
            }
        }
    }
}