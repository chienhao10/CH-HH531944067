using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;
using EloBuddy.SDK.Menu;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class Thresh
    {
        private static Menu Config = Program.Config;
        private static LeagueSharp.Common.Spell E, Epush, Q, R, W;
        private static Obj_AI_Base Marked;
        public static AIHeroClient Player { get { return ObjectManager.Player; } }

        public static Menu qMenu, wMenu, eMenu, rMenu, drawMenu, miscMenu;

        public static void LoadOKTW()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 1075);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 950);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 460);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 420);
            Epush = new LeagueSharp.Common.Spell(SpellSlot.E, 450);

            Q.SetSkillshot(0.5f, 70, 1900f, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.2f, 10, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.25f, 50, 2000, false, SkillshotType.SkillshotLine);
            Epush.SetSkillshot(0f, 50, float.MaxValue, false, SkillshotType.SkillshotLine);

            qMenu = Config.AddSubMenu("Q Option");
            qMenu.Add("ts", new CheckBox("Use common TargetSelector", true));
            qMenu.AddGroupLabel("ON - only one target");
            qMenu.AddGroupLabel("OFF - all grab-able targets");
            qMenu.Add("qCC", new CheckBox("Auto Q cc", true));
            qMenu.Add("qDash", new CheckBox("Auto Q dash", true));
            qMenu.Add("minGrab", new Slider("Min range grab", 250, 125, (int)Q.Range));
            qMenu.Add("maxGrab", new Slider("Max range grab", (int)Q.Range, 125, (int)Q.Range));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != Player.Team))
                qMenu.Add("grab" + enemy.NetworkId, new CheckBox("Pull :" + enemy.ChampionName));
            qMenu.Add("GapQ", new CheckBox("OnEnemyGapcloser Q", true));

            wMenu = Config.AddSubMenu("W Option");
            wMenu.Add("ThrowLantern", new KeyBind("Throw Lantern to Ally", false, KeyBind.BindTypes.HoldActive, 'T'));
            wMenu.Add("autoW", new CheckBox("Auto W", true));
            wMenu.Add("Wdmg", new Slider("W dmg % hp", 10, 0, 100));
            wMenu.Add("autoW3", new CheckBox("Auto W shield big dmg", true));
            wMenu.Add("autoW2", new CheckBox("Auto W if Q succesfull", true));
            wMenu.Add("autoW4", new CheckBox("Auto W vs Blitz Hook", true));
            wMenu.Add("autoW5", new CheckBox("Auto W if jungler pings", true));
            wMenu.Add("autoW6", new CheckBox("Auto W on gapCloser", true));
            wMenu.Add("autoW7", new CheckBox("Auto W on Slows/Stuns", true));
            wMenu.Add("wCount", new Slider("Auto W if x enemies near ally", 3, 0, 5));

            eMenu = Config.AddSubMenu("E Option");
            eMenu.Add("autoE", new CheckBox("Auto E", true));
            eMenu.Add("pushE", new CheckBox("Auto push", true));
            eMenu.Add("inter", new CheckBox("OnPossibleToInterrupt", true));
            eMenu.Add("Gap", new CheckBox("OnEnemyGapcloser", true));
            eMenu.Add("Emin", new Slider("Min pull range E", 200, 0, (int)E.Range));

            rMenu = Config.AddSubMenu("R Option");
            rMenu.Add("rCount", new Slider("Auto R if x enemies in range", 2, 0, 5));
            rMenu.Add("rKs", new CheckBox("R ks", false));
            rMenu.Add("comboR", new CheckBox("always R in combo", false));

            drawMenu = Config.AddSubMenu("Drawings");
            drawMenu.Add("qRange", new CheckBox("Q range", false));
            drawMenu.Add("wRange", new CheckBox("W range", false));
            drawMenu.Add("eRange", new CheckBox("E range", false));
            drawMenu.Add("rRange", new CheckBox("R range", false));
            drawMenu.Add("onlyRdy", new CheckBox("Draw when skill rdy", true));

            miscMenu = Config.AddSubMenu("Misc");
            miscMenu.Add("AACombo", new CheckBox("Disable AA if can use E", true));

            Game.OnUpdate += Game_OnGameUpdate;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnBuffGain += Obj_AI_Base_OnBuffAdd;
            Obj_AI_Base.OnBuffLose += Obj_AI_Base_OnBuffRemove;

            EloBuddy.TacticalMap.OnPing += Game_OnPing;
        }

        private static void ThrowLantern()
        {
            if (W.IsReady())
            {
                var NearAllies = Player.GetAlliesInRange(W.Range).Where(x => !x.IsMe).Where(x => !x.IsDead).Where(x => x.LSDistance(Player.Position) <= W.Range + 250).FirstOrDefault();
                if (NearAllies == null) return;
                W.Cast(NearAllies.Position);
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

        static void Game_OnPing(TacticalMapPingEventArgs args)
        {
            if (!getCheckBoxItem(wMenu, "autoW5")) return;
            var jungler = args.Source as AIHeroClient;
            if (jungler != null && jungler.LSDistance(Player.Position) <= W.Range + 500)
            {
                if (jungler.Spellbook.GetSpell(SpellSlot.Summoner1).Name.ToLower().Contains("smite") ||
                    jungler.Spellbook.GetSpell(SpellSlot.Summoner2).Name.ToLower().Contains("smite"))
                {
                    if (args.PingType == PingCategory.OnMyWay)
                    {
                        int random = new Random().Next(350, 750); // so it wont happen too fast
                        LeagueSharp.Common.Utility.DelayAction.Add(random, () => CastW(args.Position.To3D()));
                    }

                }
            }
        }


        private static void Obj_AI_Base_OnBuffRemove(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (sender.IsEnemy && args.Buff.Name == "ThreshQ")
            {
                Marked = null;
            }
        }

        private static void Obj_AI_Base_OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (sender.IsEnemy && args.Buff.Name == "ThreshQ")
            {
                Marked = sender;
            }
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (E.IsReady() && getCheckBoxItem(eMenu, "inter") && sender.LSIsValidTarget(E.Range))
            {
                E.Cast(sender.ServerPosition);
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsAlly) return;

            if (getCheckBoxItem(wMenu, "autoW6"))
            {
                var allyHero =
                    HeroManager.Allies.Where(ally => ally.LSDistance(Player) <= W.Range + 550 && !ally.IsMe)
                        .OrderBy(ally => ally.LSDistance(gapcloser.End))
                        .FirstOrDefault();
                if (allyHero != null)
                {
                    CastW(allyHero.Position);
                }
            }
            if (E.IsReady() && getCheckBoxItem(eMenu, "Gap") && gapcloser.Sender.LSIsValidTarget(E.Range))
            {
                E.Cast(gapcloser.Sender);
            }
            else if (Q.IsReady() && getCheckBoxItem(qMenu, "GapQ") && gapcloser.Sender.LSIsValidTarget(Q.Range))
            {
                Q.Cast(gapcloser.Sender);
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {

            if (Program.Combo && getCheckBoxItem(miscMenu, "AACombo"))
            {
                if (!E.IsReady())
                    Orbwalker.DisableAttacking = false;

                else
                    Orbwalker.DisableAttacking = true;
            }
            else
                Orbwalker.DisableAttacking = false;

            if (getKeyBindItem(wMenu, "ThrowLantern"))
            {
                ThrowLantern();
            }

            if (Marked.LSIsValidTarget())
            {
                if (Program.Combo)
                {
                    if (OktwCommon.GetPassiveTime(Marked, "ThreshQ") < 0.3)
                        Q.Cast();

                    if (W.IsReady() && getCheckBoxItem(wMenu, "autoW2"))
                    {
                        var allyW = Player;
                        foreach (var ally in Program.Allies.Where(ally => ally.IsValid && !ally.IsDead && Player.LSDistance(ally.ServerPosition) < W.Range + 500))
                        {
                            if (Marked.LSDistance(ally.ServerPosition) > 800 && Player.LSDistance(ally.ServerPosition) > 600)
                            {
                                CastW(LeagueSharp.Common.Prediction.GetPrediction(ally, 1f).CastPosition);
                            }
                        }
                    }
                }
            }
            else
            {
                if (Program.LagFree(1) && Q.IsReady())
                    LogicQ();

                if (Program.LagFree(2) && E.IsReady() && getCheckBoxItem(eMenu, "autoE"))
                    LogicE();
            }

            if (Program.LagFree(3) && W.IsReady())
                LogicW();
            if (Program.LagFree(4) && R.IsReady())
                LogicR();
        }

        private static void LogicE()
        {
            var t = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            if (t.LSIsValidTarget() && OktwCommon.CanMove(t))
            {

                if (Program.Combo)
                {
                    if (Player.LSDistance(t) > getSliderItem(eMenu, "Emin"))
                        CastE(false, t);
                }
                else if (getCheckBoxItem(eMenu, "pushE"))
                {
                    CastE(true, t);
                }
            }
        }

        private static void CastE(bool push, Obj_AI_Base target)
        {
            if (push)
            {
                var eCastPosition = E.GetPrediction(target).CastPosition;
                E.Cast(eCastPosition);
            }
            else
            {
                var eCastPosition = Epush.GetPrediction(target).CastPosition;
                var distance = Player.LSDistance(eCastPosition);
                var ext = Player.Position.LSExtend(eCastPosition, -distance);
                E.Cast(ext);
            }
        }

        private static void LogicQ()
        {
            float maxGrab = getSliderItem(qMenu, "maxGrab");
            float minGrab = getSliderItem(qMenu, "minGrab");

            if (Program.Combo && getCheckBoxItem(qMenu, "ts"))
            {
                var t = TargetSelector.GetTarget(maxGrab, DamageType.Physical);

                if (t.LSIsValidTarget(maxGrab) && !t.HasBuffOfType(BuffType.SpellImmunity) && !t.HasBuffOfType(BuffType.SpellShield) && getCheckBoxItem(qMenu, "grab" + t.NetworkId) && Player.LSDistance(t.ServerPosition) > minGrab)
                    Program.CastSpell(Q, t);
            }

            foreach (var t in Program.Enemies.Where(t => t.LSIsValidTarget(maxGrab) && getCheckBoxItem(qMenu, "grab" + t.NetworkId) && Player.LSDistance(t.ServerPosition) > minGrab))
            {
                if (!t.HasBuffOfType(BuffType.SpellImmunity) && !t.HasBuffOfType(BuffType.SpellShield))
                {
                    if (Program.Combo && !getCheckBoxItem(qMenu, "ts"))
                        Program.CastSpell(Q, t);

                    if (getCheckBoxItem(qMenu, "qCC"))
                    {
                        if (!OktwCommon.CanMove(t))
                            Q.Cast(t);

                        Q.CastIfHitchanceEquals(t, HitChance.Immobile);
                    }
                    if (getCheckBoxItem(qMenu, "qDash"))
                    {
                        Q.CastIfHitchanceEquals(t, HitChance.Dashing);
                    }
                }
            }
        }

        private static void LogicR()
        {
            bool rKs = getCheckBoxItem(rMenu, "rKs");
            foreach (var target in Program.Enemies.Where(target => target.LSIsValidTarget(R.Range) && target.HasBuff("rocketgrab2")))
            {
                if (rKs && R.GetDamage(target) > target.Health && R.IsInRange(target))
                    R.Cast();
            }
            if (Player.LSCountEnemiesInRange(R.Range) >= getSliderItem(rMenu, "rCount") && getSliderItem(rMenu, "rCount") > 0)
                R.Cast();
            if (getCheckBoxItem(rMenu, "comboR"))
            {
                var t = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                if (t.LSIsValidTarget() && ((Player.UnderTurret(false) && !Player.UnderTurret(true)) || Program.Combo) && R.IsInRange(t))
                {
                    if (Player.LSDistance(t.ServerPosition) > Player.LSDistance(t.Position))
                        R.Cast();
                }
            }
        }

        private static void LogicW()
        {
            if (getCheckBoxItem(wMenu, "autoW4"))
            {
                var saveAlly = Program.Allies.FirstOrDefault(ally => ally.HasBuff("rocketgrab2") && !ally.IsMe);
                if (saveAlly != null)
                {
                    var blitz = saveAlly.GetBuff("rocketgrab2").Caster;
                    if (Player.LSDistance(blitz.Position) <= W.Range + 550 && W.IsReady())
                    {
                        CastW(blitz.Position);
                    }
                }
            }

            foreach (var ally in Program.Allies.Where(ally => ally.IsValid && !ally.IsDead && Player.LSDistance(ally) < W.Range + 400))
            {
                if (getCheckBoxItem(wMenu, "autoW7") && !ally.IsMe)
                {
                    if (ally.LSDistance(Player) <= W.Range)
                    {
                        if (ally.IsStunned || ally.IsRooted)
                        {
                            //W.Cast(ally.Position);
                            LeagueSharp.Common.Utility.DelayAction.Add(250, () => { W.Cast(ally.Position); });
                        }
                    }
                }

                int nearEnemys = ally.LSCountEnemiesInRange(900);

                if (nearEnemys >= getSliderItem(wMenu, "wCount") && getSliderItem(wMenu, "wCount") > 0)
                    CastW(W.GetPrediction(ally).UnitPosition);

                if (getCheckBoxItem(wMenu, "autoW") && Player.LSDistance(ally) < W.Range + 100)
                {
                    double dmg = OktwCommon.GetIncomingDamage(ally);
                    if (dmg == 0)
                        continue;

                    int sensitivity = 20;

                    double HpPercentage = (dmg * 100) / ally.Health;
                    double shieldValue = 20 + (Player.Level * 20) + (0.4 * Player.FlatMagicDamageMod);

                    nearEnemys = (nearEnemys == 0) ? 1 : nearEnemys;

                    if (dmg > shieldValue && getCheckBoxItem(wMenu, "autoW3"))
                        LeagueSharp.Common.Utility.DelayAction.Add(500, () => { W.Cast(ally); });
                    else if (dmg > 100 + Player.Level * sensitivity)
                        LeagueSharp.Common.Utility.DelayAction.Add(500, () => { W.Cast(ally); });
                    else if (ally.Health - dmg < nearEnemys * ally.Level * sensitivity)
                        LeagueSharp.Common.Utility.DelayAction.Add(500, () => { W.Cast(ally); });
                    else if (HpPercentage >= getSliderItem(wMenu, "Wdmg"))
                        LeagueSharp.Common.Utility.DelayAction.Add(500, () => { W.Cast(ally); });
                }
            }
        }

        private static void CastW(Vector3 pos)
        {
            if (Player.LSDistance(pos) < W.Range)
                LeagueSharp.Common.Utility.DelayAction.Add(250, () => { W.Cast(pos); });
            else
                LeagueSharp.Common.Utility.DelayAction.Add(250, () => { Player.Position.LSExtend(pos, W.Range); });
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
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

            if (getCheckBoxItem(drawMenu, "wRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (E.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(Player.Position, W.Range, System.Drawing.Color.Cyan, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(Player.Position, W.Range, System.Drawing.Color.Cyan, 1, 1);
            }

            if (getCheckBoxItem(drawMenu, "eRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (E.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(Player.Position, E.Range, System.Drawing.Color.Orange, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(Player.Position, E.Range, System.Drawing.Color.Orange, 1, 1);
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
    }
}