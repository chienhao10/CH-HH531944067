using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using OneKeyToWin_AIO_Sebby.Core;
using SebbyLib;
using SharpDX;
using Color = System.Drawing.Color;
using HealthPrediction = SebbyLib.HealthPrediction;
using Orbwalking = SebbyLib.Orbwalking;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

namespace OneKeyToWin_AIO_Sebby
{
    internal class Ezreal
    {
        private static readonly Menu Config = Program.Config;
        public static Menu drawMenu, wMenu, eMenu, rMenu, harassMenu, farmMenu, miscMenu;
        public static Spell Q, W, E, R;
        public static float QMANA, WMANA, EMANA, RMANA;

        private static Vector3 CursorPosition = Vector3.Zero;
        public static double lag;
        public static double WCastTime = 0;
        public static double QCastTime = 0;
        public static float DragonDmg;
        public static double DragonTime;
        public static bool Esmart;
        public static double OverKill;
        public static double OverFarm = 0;
        public static double diag = 0;
        public static double diagF = 0;
        public static int Muramana = 3042;
        public static int Tear = 3070;
        public static int Manamune = 3004;
        public static double NotTime = 0;

        public static OKTWdash Dash;

        public static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        private static bool Farm
        {
            get
            {
                return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                       Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) ||
                       Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit);
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

        public static void LoadOKTW()
        {
            Q = new Spell(SpellSlot.Q, 1150);
            W = new Spell(SpellSlot.W, 950);
            E = new Spell(SpellSlot.E, 475);
            R = new Spell(SpellSlot.R, 3000f);

            Q.SetSkillshot(0.25f, 60f, 2000f, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.25f, 80f, 1600f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(1.1f, 160f, 2000f, false, SkillshotType.SkillshotLine);

            drawMenu = Config.AddSubMenu("Draw");
            drawMenu.Add("noti", new CheckBox("Show notification"));
            drawMenu.Add("onlyRdy", new CheckBox("Draw only ready spells"));
            drawMenu.Add("qRange", new CheckBox("Q range"));
            drawMenu.Add("wRange", new CheckBox("W range"));
            drawMenu.Add("eRange", new CheckBox("E range"));
            drawMenu.Add("rRange", new CheckBox("R range"));

            wMenu = Config.AddSubMenu("W Option");
            wMenu.Add("autoW", new CheckBox("Auto W"));
            wMenu.Add("wPush", new CheckBox("W ally (push tower)"));
            wMenu.Add("harrasW", new CheckBox("Harass W"));

            eMenu = Config.AddSubMenu("E Option");
            eMenu.Add("smartE", new KeyBind("SmartCast E key", false, KeyBind.BindTypes.HoldActive, 'T'));
            eMenu.Add("smartEW", new KeyBind("SmartCast E + W key", false, KeyBind.BindTypes.HoldActive, 'T'));
            eMenu.Add("autoE", new CheckBox("Auto E"));
            eMenu.Add("autoEgrab", new CheckBox("Auto E anti grab"));

            Dash = new OKTWdash(E);

            rMenu = Config.AddSubMenu("R Option");
            rMenu.AddGroupLabel("R Jungle Stealer");
            rMenu.Add("Rjungle", new CheckBox("R Jungle stealer"));
            rMenu.Add("Rdragon", new CheckBox("Dragon"));
            rMenu.Add("Rbaron", new CheckBox("Baron"));
            rMenu.Add("Rred", new CheckBox("Red"));
            rMenu.Add("Rblue", new CheckBox("Blue"));
            rMenu.Add("Rally", new CheckBox("Ally stealer", false));
            rMenu.AddSeparator();
            rMenu.Add("autoR", new CheckBox("Auto R"));
            rMenu.Add("Rcc", new CheckBox("R cc"));
            rMenu.Add("Raoe", new Slider("R AOE", 3, 0, 5));
            rMenu.Add("useR", new KeyBind("Semi-manual cast R key", false, KeyBind.BindTypes.HoldActive, 'J'));
            rMenu.Add("Rturrent", new CheckBox("Don't R under turret"));
            rMenu.Add("MaxRangeR", new Slider("Max R range", 3000, 0, 5000));
            rMenu.Add("MinRangeR", new Slider("Min R range", 900, 0, 5000));

            farmMenu = Config.AddSubMenu("Farm");
            farmMenu.Add("farmQ", new CheckBox("LaneClear Q"));
            farmMenu.Add("FQ", new CheckBox("Farm Q out range"));
            farmMenu.Add("Mana", new Slider("LaneClear Mana", 50));
            farmMenu.Add("LCP", new CheckBox("FAST LaneClear"));

            harassMenu = Config.AddSubMenu("Harass");
            harassMenu.Add("HarassMana", new Slider("Harass Mana", 30));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != Player.Team))
            {
                harassMenu.Add("haras" + enemy.ChampionName, new CheckBox(enemy.ChampionName));
            }

            miscMenu = Config.AddSubMenu("Misc");
            miscMenu.Add("debug", new CheckBox("Debug", false));
            miscMenu.Add("apEz", new CheckBox("AP Ezreal", false));
            miscMenu.Add("stack", new CheckBox("Stack Tear if full mana"));

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalker.OnPostAttack += afterAttack;
            Obj_AI_Base.OnSpellCast += Obj_AI_Base_OnBuffAdd;
        }

        private static void Obj_AI_Base_OnBuffAdd(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && getCheckBoxItem(eMenu, "autoEgrab") && E.IsReady())
            {
                if (args.SData.Name == "ThreshQ" || args.SData.Name == "rocketgrab2")
                {
                    var dashPos = Dash.CastDash(true);
                    E.Cast(!dashPos.IsZero ? dashPos : Game.CursorPos);
                }
            }
        }

        private static void afterAttack(AttackableUnit target, EventArgs args)
        {
            if (W.IsReady() && getCheckBoxItem(wMenu, "wPush") && target.IsValid<Obj_AI_Turret>() && Player.Mana > RMANA + EMANA + QMANA + WMANA + WMANA + RMANA)
            {
                foreach (var ally in Program.Allies)
                {
                    if (!ally.IsMe && ally.IsAlly && ally.Distance(Player.Position) < 600)
                        W.Cast(ally);
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Program.LagFree(0))
            {
                SetMana();
            }

            if (R.IsReady() && getCheckBoxItem(rMenu, "Rjungle"))
            {
                KsJungle();
            }
            else
                DragonTime = 0;

            if (E.IsReady())
            {
                if (Program.LagFree(0) && getCheckBoxItem(eMenu, "autoE") && Program.Combo)
                    LogicE();

                if (getKeyBindItem(eMenu, "smartE"))
                    Esmart = true;
                if (getKeyBindItem(eMenu, "smartEW") && W.IsReady())
                {
                    CursorPosition = Game.CursorPos;
                    W.Cast(CursorPosition);
                }
                if (Esmart && Player.Position.Extend(Game.CursorPos, E.Range).CountEnemiesInRange(500) < 4)
                    E.Cast(Player.Position.Extend(Game.CursorPos, E.Range), true);

                if (!CursorPosition.IsZero)
                    E.Cast(Player.Position.Extend(CursorPosition, E.Range), true);
            }
            else
            {
                CursorPosition = Vector3.Zero;
                Esmart = false;
            }

            if (Q.IsReady())
                LogicQ();

            if (Program.LagFree(3) && W.IsReady() && getCheckBoxItem(wMenu, "autoW"))
                LogicW();

            if (R.IsReady())
            {
                if (getKeyBindItem(rMenu, "useR"))
                {
                    var t = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                    if (t.IsValidTarget())
                        R.Cast(t, true, true);
                }

                if (Program.LagFree(4))
                    LogicR();
            }
        }

        private static void LogicQ()
        {
            if (Program.LagFree(1))
            {
                var cc = !Program.None && Player.Mana > RMANA + QMANA + EMANA;
                var harass = Program.Farm && Player.ManaPercent > getSliderItem(harassMenu, "HarassMana") &&
                             OktwCommon.CanHarras();
                var combo = Program.Combo && Player.Mana > RMANA + QMANA;
                foreach (var t in Program.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range)).OrderBy(t => t.Health))
                {
                    var qDmg = OktwCommon.GetKsDamage(t, Q);
                    var wDmg = W.GetDamage(t);
                    if (qDmg + wDmg > t.Health)
                    {
                        Program.CastSpell(Q, t);
                        OverKill = Game.Time;
                        return;
                    }

                    if (cc && !OktwCommon.CanMove(t))
                        Q.Cast(t);

                    if (combo)
                        Program.CastSpell(Q, t);
                    else if (harass && getCheckBoxItem(harassMenu, "haras" + t.ChampionName))
                        Program.CastSpell(Q, t);
                }
            }
            else if (Program.LagFree(2))
            {
                if (Farm && Player.Mana > RMANA + EMANA + WMANA + QMANA*3)
                {
                    farmQ();
                    lag = Game.Time;
                }
                else if (getCheckBoxItem(miscMenu, "stack") && Utils.TickCount - Q.LastCastAttemptT > 4000 &&
                         !Player.HasBuff("Recall") && Player.Mana > Player.MaxMana*0.95 && Program.None &&
                         (Items.HasItem(Tear) || Items.HasItem(Manamune)))
                {
                    Q.Cast(Player.Position.Extend(Game.CursorPos, 500));
                }
            }
        }

        private static void LogicW()
        {
            var t = TargetSelector.GetTarget(W.Range, DamageType.Magical);
            if (t.IsValidTarget())
            {
                if (Program.Combo && Player.Mana > RMANA + WMANA + EMANA)
                    Program.CastSpell(W, t);
                else if (Program.Farm && getCheckBoxItem(wMenu, "harrasW") &&
                         getCheckBoxItem(harassMenu, "haras" + t.ChampionName) &&
                         (Player.Mana > Player.MaxMana*0.8 || getCheckBoxItem(miscMenu, "apEz")) &&
                         Player.ManaPercent > getSliderItem(harassMenu, "HarassMana") && OktwCommon.CanHarras())
                    Program.CastSpell(W, t);
                else
                {
                    var qDmg = Q.GetDamage(t);
                    var wDmg = OktwCommon.GetKsDamage(t, W);
                    if (wDmg > t.Health)
                    {
                        Program.CastSpell(W, t);
                        OverKill = Game.Time;
                    }
                    else if (wDmg + qDmg > t.Health && Q.IsReady())
                    {
                        Program.CastSpell(W, t);
                    }
                }

                if (!Program.None && Player.Mana > RMANA + WMANA + EMANA)
                {
                    foreach (
                        var enemy in
                            Program.Enemies.Where(enemy => enemy.IsValidTarget(W.Range) && !OktwCommon.CanMove(enemy)))
                        W.Cast(enemy, true);
                }
            }
        }

        private static void LogicE()
        {
            var t = TargetSelector.GetTarget(1300, DamageType.Physical);
            var dashPosition = Player.Position.Extend(Game.CursorPos, E.Range);

            if (Program.Enemies.Any(target => target.IsValidTarget(270) && target.IsMelee))
            {
                var dashPos = Dash.CastDash(true);
                if (!dashPos.IsZero)
                {
                    E.Cast(dashPos);
                }
            }

            if (t.IsValidTarget() && Player.HealthPercent > 40 && !Player.UnderTurret(true) &&
                (Game.Time - OverKill > 0.3) && dashPosition.CountEnemiesInRange(900) < 3)
            {
                if (t.Distance(Game.CursorPos) + 300 < t.Position.Distance(Player.Position) &&
                    !Orbwalking.InAutoAttackRange(t))
                {
                    var dmgCombo = 0f;

                    if (t.IsValidTarget(950))
                    {
                        dmgCombo = Player.GetAutoAttackDamage(t) + E.GetDamage(t);
                    }

                    if (Q.IsReady() && Player.Mana > QMANA + EMANA &&
                        Q.WillHit(dashPosition.To3D(), Q.GetPrediction(t).UnitPosition))
                        dmgCombo = Q.GetDamage(t);

                    if (W.IsReady() && Player.Mana > QMANA + EMANA + WMANA)
                    {
                        dmgCombo += W.GetDamage(t);
                    }

                    if (dmgCombo > t.Health && OktwCommon.ValidUlt(t))
                    {
                        E.Cast(dashPosition);
                        OverKill = Game.Time;
                        Program.debug("E ks combo");
                    }
                }
            }
        }

        private static void LogicR()
        {
            if (Player.UnderTurret(true) && getCheckBoxItem(rMenu, "Rturrent"))
                return;

            if (getSliderItem(rMenu, "MinRangeR") > getSliderItem(rMenu, "MaxRangeR"))
            {
                Chat.Print("R Logic can not be used because MinRange for R is greater than Max Range.");
                return;
            }

            if (getCheckBoxItem(rMenu, "autoR") && Player.CountEnemiesInRange(800) == 0 && Game.Time - OverKill > 0.6)
            {
                R.Range = getSliderItem(rMenu, "MaxRangeR");
                foreach (
                    var target in
                        Program.Enemies.Where(target => target.IsValidTarget(R.Range) && OktwCommon.ValidUlt(target)))
                {
                    var predictedHealth = target.Health - OktwCommon.GetIncomingDamage(target);

                    if (getCheckBoxItem(rMenu, "Rcc") && target.IsValidTarget(Q.Range + E.Range) &&
                        target.Health < Player.MaxHealth && !OktwCommon.CanMove(target))
                    {
                        R.Cast(target, true, true);
                    }

                    double Rdmg = R.GetDamage(target);

                    if (Rdmg > predictedHealth)
                        Rdmg = getRdmg(target);

                    if (Rdmg > predictedHealth && target.CountAlliesInRange(500) == 0 &&
                        Player.Distance(target) > getSliderItem(rMenu, "MinRangeR"))
                    {
                        Program.CastSpell(R, target);
                        Program.debug("R normal");
                    }
                    if (Program.Combo && Player.CountEnemiesInRange(1200) == 0)
                    {
                        R.CastIfWillHit(target, getSliderItem(rMenu, "Raoe"), true);
                    }
                }
            }
        }

        private static double getRdmg(Obj_AI_Base target)
        {
            var rDmg = R.GetDamage(target);
            var output = R.GetPrediction(target);
            var direction = output.CastPosition.To2D() - Player.Position.To2D();
            direction.Normalize();
            var enemies = ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy && x.IsValidTarget()).ToList();
            var dmg = (from enemy in enemies let prediction = R.GetPrediction(enemy) let predictedPosition = prediction.CastPosition let v = output.CastPosition - Player.ServerPosition let w = predictedPosition - Player.ServerPosition let c1 = Vector3.Dot(w, v) let c2 = Vector3.Dot(v, v) let b = c1/(double) c2 let pb = Player.ServerPosition + (float) b*v let length = Vector3.Distance(predictedPosition, pb) where length < R.Width + 100 + enemy.BoundingRadius/2 && Player.Distance(predictedPosition) < Player.Distance(target.ServerPosition) select enemy).Count();
            var allMinionsR = Cache.GetMinions(ObjectManager.Player.ServerPosition, R.Range);
            dmg += (from minion in allMinionsR let prediction = R.GetPrediction(minion) let predictedPosition = prediction.CastPosition let v = output.CastPosition - Player.ServerPosition let w = predictedPosition - Player.ServerPosition let c1 = Vector3.Dot(w, v) let c2 = Vector3.Dot(v, v) let b = c1/(double) c2 let pb = Player.ServerPosition + (float) b*v let length = Vector3.Distance(predictedPosition, pb) where length < R.Width + 100 + minion.BoundingRadius/2 && Player.Distance(predictedPosition) < Player.Distance(target.ServerPosition) select minion).Count();
            if (dmg == 0)
                return rDmg;
            if (dmg > 7)
                return rDmg*0.7;
            return rDmg - rDmg*0.1*dmg;
        }

        private static float GetPassiveTime()
        {
            return
                ObjectManager.Player.Buffs.OrderByDescending(buff => buff.EndTime - Game.Time)
                    .Where(buff => buff.Name == "ezrealrisingspellforce")
                    .Select(buff => buff.EndTime)
                    .FirstOrDefault();
        }

        public static void farmQ()
        {
            if (Program.LaneClear)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, 800, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    Q.Cast(mob.Position);
                }
            }

            var minions = Cache.GetMinions(Player.ServerPosition, Q.Range);
            var orbTarget = 0;

            if (Orbwalker.LastTarget != null)
                orbTarget = Orbwalker.LastTarget.NetworkId;

            if (getCheckBoxItem(farmMenu, "FQ"))
            {
                if (minions.Where(minion => minion.IsValidTarget() && orbTarget != minion.NetworkId && minion.HealthPercent < 70 && !LeagueSharp.Common.Orbwalking.InAutoAttackRange(minion) && minion.Health < Q.GetDamage(minion)).Any(minion => Q.Cast(minion) == Spell.CastStates.SuccessfullyCasted))
                {
                    return;
                }
            }

            if (getCheckBoxItem(farmMenu, "farmQ") && Program.LaneClear && !Orbwalking.CanAttack() &&
                Player.ManaPercent > getSliderItem(farmMenu, "Mana"))
            {
                var LCP = getCheckBoxItem(farmMenu, "LCP");
                var PT = Game.Time - GetPassiveTime() > -1.5 || !E.IsReady();

                foreach (var minion in minions.Where(LeagueSharp.Common.Orbwalking.InAutoAttackRange))
                {
                    var hpPred = HealthPrediction.GetHealthPrediction(minion, 300);
                    if (hpPred < 20)
                        continue;

                    var qDmg = Q.GetDamage(minion);
                    if (hpPred < qDmg && orbTarget != minion.NetworkId)
                    {
                        if (Q.Cast(minion) == Spell.CastStates.SuccessfullyCasted)
                            return;
                    }
                    else if (PT || LCP)
                    {
                        if (minion.HealthPercent > 80)
                        {
                            if (Q.Cast(minion) == Spell.CastStates.SuccessfullyCasted)
                                return;
                        }
                    }
                }
            }
        }

        private static void KsJungle()
        {
            var mobs = Cache.GetMinions(Player.ServerPosition, float.MaxValue, MinionTeam.Neutral);
            foreach (var mob in mobs)
            {
                if (mob.Health == mob.MaxHealth)
                    continue;
                if (((mob.BaseSkinName == "SRU_Dragon" && getCheckBoxItem(rMenu, "Rdragon"))
                     || (mob.BaseSkinName == "SRU_Baron" && getCheckBoxItem(rMenu, "Rbaron"))
                     || (mob.BaseSkinName == "SRU_Red" && getCheckBoxItem(rMenu, "Rred"))
                     || (mob.BaseSkinName == "SRU_Blue" && getCheckBoxItem(rMenu, "Rblue")))
                    && (mob.CountAlliesInRange(1000) == 0 || getCheckBoxItem(rMenu, "Rally"))
                    && mob.Distance(Player.Position) > 1000
                    )
                {
                    if (DragonDmg == 0)
                        DragonDmg = mob.Health;

                    if (Game.Time - DragonTime > 3)
                    {
                        if (DragonDmg - mob.Health > 0)
                        {
                            DragonDmg = mob.Health;
                        }
                        DragonTime = Game.Time;
                    }
                    else
                    {
                        var DmgSec = (DragonDmg - mob.Health)*(Math.Abs(DragonTime - Game.Time)/3);
                        //SebbyLib.Program.debug("DS  " + DmgSec);
                        if (DragonDmg - mob.Health > 0)
                        {
                            var timeTravel = GetUltTravelTime(Player, R.Speed, R.Delay, mob.Position);
                            var timeR = (mob.Health - R.GetDamage(mob))/(DmgSec/3);
                            //SebbyLib.Program.debug("timeTravel " + timeTravel + "timeR " + timeR + "d " + R.GetDamage(mob));
                            if (timeTravel > timeR)
                                R.Cast(mob.Position);
                        }
                        else
                            DragonDmg = mob.Health;
                        //SebbyLib.Program.debug("" + GetUltTravelTime(ObjectManager.Player, R.Speed, R.Delay, mob.Position));
                    }
                }
            }
        }

        private static float GetUltTravelTime(AIHeroClient source, float speed, float delay, Vector3 targetpos)
        {
            var distance = Vector3.Distance(source.ServerPosition, targetpos);
            var missilespeed = speed;

            return distance/missilespeed + delay;
        }

        private static void SetMana()
        {
            if ((Program.getCheckBoxItem("manaDisable") && Program.Combo) || Player.HealthPercent < 20)
            {
                QMANA = 0;
                WMANA = 0;
                EMANA = 0;
                RMANA = 0;
                return;
            }

            QMANA = Q.Instance.SData.Mana;
            WMANA = W.Instance.SData.Mana;
            EMANA = E.Instance.SData.Mana;

            if (!R.IsReady())
                RMANA = QMANA - Player.PARRegenRate*Q.Instance.Cooldown;
            else
                RMANA = R.Instance.SData.Mana;
        }

        public static void drawText(string msg, AIHeroClient Hero, Color color)
        {
            var wts = Drawing.WorldToScreen(Hero.Position);
            Drawing.DrawText(wts[0] - msg.Length*5, wts[1], color, msg);
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(drawMenu, "qRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (Q.IsReady())
                        Utility.DrawCircle(Player.Position, Q.Range, Color.Cyan, 1, 1);
                }
                else
                    Utility.DrawCircle(Player.Position, Q.Range, Color.Cyan, 1, 1);
            }
            if (getCheckBoxItem(drawMenu, "wRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (W.IsReady())
                        Utility.DrawCircle(ObjectManager.Player.Position, W.Range, Color.Orange, 1, 1);
                }
                else
                    Utility.DrawCircle(ObjectManager.Player.Position, W.Range, Color.Orange, 1, 1);
            }
            if (getCheckBoxItem(drawMenu, "eRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (E.IsReady())
                        Utility.DrawCircle(Player.Position, E.Range, Color.Yellow, 1, 1);
                }
                else
                    Utility.DrawCircle(Player.Position, E.Range, Color.Yellow, 1, 1);
            }
            if (getCheckBoxItem(drawMenu, "rRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (R.IsReady())
                        Utility.DrawCircle(Player.Position, R.Range, Color.Gray, 1, 1);
                }
                else
                    Utility.DrawCircle(Player.Position, R.Range, Color.Gray, 1, 1);
            }


            if (getCheckBoxItem(drawMenu, "noti"))
            {
                var target = TargetSelector.GetTarget(1500, DamageType.Physical);
                if (target.IsValidTarget())
                {
                    var poutput = Q.GetPrediction(target);
                    if ((int) poutput.Hitchance == 5)
                        Render.Circle.DrawCircle(poutput.CastPosition, 50, Color.YellowGreen);
                    if (Q.GetDamage(target) > target.Health)
                    {
                        Render.Circle.DrawCircle(target.ServerPosition, 200, Color.Red);
                        Drawing.DrawText(Drawing.Width*0.1f, Drawing.Height*0.4f, Color.Red,
                            "Q kill: " + target.ChampionName + " have: " + target.Health + "hp");
                    }
                    else if (Q.GetDamage(target) + W.GetDamage(target) > target.Health)
                    {
                        Render.Circle.DrawCircle(target.ServerPosition, 200, Color.Red);
                        Drawing.DrawText(Drawing.Width*0.1f, Drawing.Height*0.4f, Color.Red,
                            "Q + W kill: " + target.ChampionName + " have: " + target.Health + "hp");
                    }
                    else if (Q.GetDamage(target) + W.GetDamage(target) + E.GetDamage(target) > target.Health)
                    {
                        Render.Circle.DrawCircle(target.ServerPosition, 200, Color.Red);
                        Drawing.DrawText(Drawing.Width*0.1f, Drawing.Height*0.4f, Color.Red,
                            "Q + W + E kill: " + target.ChampionName + " have: " + target.Health + "hp");
                    }
                }
            }
        }
    }
}