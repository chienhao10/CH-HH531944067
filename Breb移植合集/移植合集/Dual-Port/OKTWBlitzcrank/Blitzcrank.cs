using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;
using EloBuddy.SDK;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Blitzcrank
    {
        private static Menu Config = Program.Config;

        private static LeagueSharp.Common.Spell E, Q, R, W;

        private static int grab = 0, grabS = 0;

        private static float grabW = 0;

        public static AIHeroClient Player { get { return ObjectManager.Player; } }

        public static Menu weMenu, qMenu, rMenu, drawMenu;

        public static void LoadOKTW()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 920);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 200);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 475);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 600);

            Q.SetSkillshot(0.25f, 80f, 1800f, true, SkillshotType.SkillshotLine);

            weMenu = Config.AddSubMenu("W/E Config");
            weMenu.Add("autoW", new CheckBox("Auto W", true));
            weMenu.Add("autoE", new CheckBox("Auto E", true));
            weMenu.Add("showgrab", new CheckBox("Show statistics", true));

            qMenu = Config.AddSubMenu("Q Config");
            qMenu.Add("ts", new CheckBox("Use common TargetSelector", true));
            qMenu.AddLabel("ON - only one target");
            qMenu.AddLabel("OFF - all grab-able targets");
            qMenu.AddSeparator();
            qMenu.Add("qTur", new CheckBox("Auto Q under turret", true));
            qMenu.Add("qCC", new CheckBox("Auto Q cc & dash enemy", true));
            qMenu.Add("minGrab", new Slider("Min range grab", 250, 125, (int)Q.Range));
            qMenu.Add("maxGrab", new Slider("Max range grab", (int)Q.Range, 125, (int)Q.Range));
            qMenu.AddSeparator();
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != Player.Team))
                qMenu.Add("grab" + enemy.ChampionName, new CheckBox("Grab : " + enemy.ChampionName));

            rMenu = Config.AddSubMenu("R Config");
            rMenu.Add("rCount", new Slider("Auto R if enemies in range", 3, 0, 5));
            rMenu.Add("afterGrab", new CheckBox("Auto R after grab", true));
            rMenu.Add("afterAA", new CheckBox("Auto R befor AA", true));
            rMenu.Add("rKs", new CheckBox("R ks", false));
            rMenu.Add("inter", new CheckBox("OnPossibleToInterrupt", true));
            rMenu.Add("Gap", new CheckBox("OnEnemyGapcloser", true));

            drawMenu = Config.AddSubMenu("Drawings");
            drawMenu.Add("qRange", new CheckBox("Q range", false));
            drawMenu.Add("rRange", new CheckBox("R range", false));
            drawMenu.Add("onlyRdy", new CheckBox("Draw when skill rdy", true));

            Game.OnUpdate += Game_OnGameUpdate;
            Orbwalker.OnPreAttack += BeforeAttack;
            Orbwalker.OnPostAttack += afterAttack;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
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

        private static void afterAttack(AttackableUnit target, EventArgs args)
        {
            if (getCheckBoxItem(rMenu, "afterAA") && R.IsReady() && target is AIHeroClient)
            {
                R.Cast();
            }
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (R.IsReady() && getCheckBoxItem(rMenu, "inter") && sender.IsValidTarget(R.Range))
                R.Cast();
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "RocketGrabMissile")
            {
                LeagueSharp.Common.Utility.DelayAction.Add(500, SebbyLib.Orbwalking.ResetAutoAttackTimer);
                grab++;
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(weMenu, "showgrab"))
            {
                var percent = 0f;
                if (grab > 0)
                    percent = ((float)grabS / (float)grab) * 100f;
                Drawing.DrawText(Drawing.Width * 0f, Drawing.Height * 0.4f, System.Drawing.Color.YellowGreen, " grab: " + grab + " grab successful: " + grabS + " grab successful % : " + percent + "%");
            }
            if (getCheckBoxItem(drawMenu, "qRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (Q.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(Player.Position, (float)getSliderItem(qMenu, "maxGrab"), System.Drawing.Color.Cyan, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(Player.Position, (float)getSliderItem(qMenu, "maxGrab"), System.Drawing.Color.Cyan, 1, 1);
            }
            if (getCheckBoxItem(drawMenu, "rRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (R.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Gray, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Gray, 1, 1);
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (R.IsReady() && getCheckBoxItem(rMenu, "Gap") && gapcloser.Sender.IsValidTarget(R.Range))
                R.Cast();
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Program.LagFree(1) && Q.IsReady())
                LogicQ();
            if (Program.LagFree(2) && R.IsReady())
                LogicR();
            if (Program.LagFree(3) && W.IsReady() && getCheckBoxItem(weMenu, "autoW"))
                LogicW();

            if (!Q.IsReady() && Game.Time - grabW > 2)
            {
                foreach (var t in Program.Enemies.Where(t => t.HasBuff("rocketgrab2")))
                {
                    grabS++;
                    grabW = Game.Time;
                    Program.debug("GRAB!!!!");
                }
            }
        }

        private static void BeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (E.IsReady() && args.Target.IsValid<AIHeroClient>() && getCheckBoxItem(weMenu, "autoE"))
                E.Cast();
        }

        private static void LogicQ()
        {
            float maxGrab = getSliderItem(qMenu, "maxGrab");
            float minGrab = getSliderItem(qMenu, "minGrab");
            var ts = getCheckBoxItem(qMenu, "ts");
            var qTur = Player.UnderAllyTurret() && getCheckBoxItem(qMenu, "qTur");
            var qCC = getCheckBoxItem(qMenu, "qCC");

            if (Program.Combo && ts)
            {
                var t = TargetSelector.GetTarget(maxGrab, DamageType.Physical);

                if (t.IsValidTarget(maxGrab) && !t.HasBuffOfType(BuffType.SpellImmunity) && !t.HasBuffOfType(BuffType.SpellShield) && getCheckBoxItem(qMenu, "grab" + t.ChampionName) && Player.Distance(t.ServerPosition) > minGrab)
                    Program.CastSpell(Q, t);
            }

            foreach (var t in Program.Enemies.Where(t => t.IsValidTarget(maxGrab) && getCheckBoxItem(qMenu, "grab" + t.ChampionName)))
            {
                if (!t.HasBuffOfType(BuffType.SpellImmunity) && !t.HasBuffOfType(BuffType.SpellShield) && Player.Distance(t.ServerPosition) > minGrab)
                {
                    if (Program.Combo && !ts)
                        Program.CastSpell(Q, t);
                    else if (qTur)
                        Program.CastSpell(Q, t);

                    if (qCC)
                    {
                        if (!OktwCommon.CanMove(t))
                            Q.Cast(t, true);
                        Q.CastIfHitchanceEquals(t, HitChance.Dashing);
                        Q.CastIfHitchanceEquals(t, HitChance.Immobile);
                    }
                }
            }
        }

        private static void LogicR()
        {
            bool rKs = getCheckBoxItem(rMenu, "rKs");
            bool afterGrab = getCheckBoxItem(rMenu, "afterGrab");
            foreach (var target in Program.Enemies.Where(target => target.IsValidTarget(R.Range)))
            {
                if (rKs && R.GetDamage(target) > target.Health)
                    R.Cast();
                if (afterGrab && target.IsValidTarget(400) && target.HasBuff("rocketgrab2"))
                    R.Cast();
            }
            if (Player.CountEnemiesInRange(R.Range) >= getSliderItem(rMenu, "rCount") && getSliderItem(rMenu, "rCount") > 0)
                R.Cast();
        }
        private static void LogicW()
        {
            foreach (var target in Program.Enemies.Where(target => target.IsValidTarget(R.Range) && target.HasBuff("rocketgrab2")))
                W.Cast();
        }
    }
}