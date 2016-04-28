using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SebbyLib;
using SharpDX;
using Color = System.Drawing.Color;
using Orbwalking = SebbyLib.Orbwalking;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

namespace OneKeyToWin_AIO_Sebby
{
    internal class MissFortune
    {
        private static readonly Menu Config = Program.Config;
        private static Spell E, Q, Q1, R, W;
        private static float QMANA, WMANA, EMANA, RMANA;
        private static int LastAttackId;
        private static float RCastTime;

        public static Menu drawMenu, qMenu, wMenu, eMenu, rMenu, farmMenu, miscMenu;

        public static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public static void LoadOKTW()
        {
            Q = new Spell(SpellSlot.Q, 655f);
            Q1 = new Spell(SpellSlot.Q, 1300f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 1000f);
            R = new Spell(SpellSlot.R, 1350f);

            Q1.SetSkillshot(0.25f, 70f, 1500f, true, SkillshotType.SkillshotLine);
            Q.SetTargetted(0.25f, 1400f);
            E.SetSkillshot(0.5f, 200f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.25f, 100f, 2000f, false, SkillshotType.SkillshotCircle);

            drawMenu = Config.AddSubMenu("Draw");
            drawMenu.Add("onlyRdy", new CheckBox("Draw only ready spells"));
            drawMenu.Add("QRange", new CheckBox("Q range", false));
            drawMenu.Add("ERange", new CheckBox("E range", false));
            drawMenu.Add("RRange", new CheckBox("R range", false));
            drawMenu.Add("noti", new CheckBox("Show notification & line"));

            qMenu = Config.AddSubMenu("Q Config");
            qMenu.Add("autoQ", new CheckBox("Auto Q"));
            qMenu.AddGroupLabel("Minion Config");
            qMenu.Add("harasQ", new CheckBox("Use Q on minion"));
            qMenu.Add("killQ", new CheckBox("Use Q only if can kill minion", false));
            qMenu.Add("qMinionMove", new CheckBox("Don't use if minions moving"));
            qMenu.Add("qMinionWidth", new Slider("Collision width calculation", 70, 0, 200));

            wMenu = Config.AddSubMenu("W Config");
            wMenu.Add("autoW", new CheckBox("Auto W"));
            wMenu.Add("harasW", new CheckBox("Harass W"));

            eMenu = Config.AddSubMenu("E Config");
            eMenu.Add("autoE", new CheckBox("Auto E"));
            eMenu.Add("AGC", new CheckBox("AntiGapcloserE"));

            rMenu = Config.AddSubMenu("R Config");
            rMenu.Add("autoR", new CheckBox("Auto R"));
            rMenu.Add("forceBlockMove", new CheckBox("Force block player"));
            rMenu.Add("useR", new KeyBind("Semi-manual cast R key", false, KeyBind.BindTypes.HoldActive, 'T'));
                //32 == space
            rMenu.Add("disableBlock", new KeyBind("Disable R key", false, KeyBind.BindTypes.HoldActive, 'R'));
                //32 == space
            rMenu.Add("Rturrent", new CheckBox("Don't R under turret"));

            farmMenu = Config.AddSubMenu("Farm");
            farmMenu.Add("jungleE", new CheckBox("Jungle clear E"));
            farmMenu.Add("jungleQ", new CheckBox("Jungle Q ks"));
            farmMenu.Add("jungleW", new CheckBox("Jungle clear W"));

            miscMenu = Config.AddSubMenu("Misc");
            miscMenu.Add("newTarget", new CheckBox("Try change focus after attack "));


            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalker.OnPostAttack += afterAttack;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            //new SebbyLib.OktwCommon(); // not sure
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

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (E.IsReady() && getCheckBoxItem(eMenu, "AGC") && Player.Mana > RMANA + EMANA)
            {
                var Target = gapcloser.Sender;
                if (Target.IsValidTarget(E.Range))
                {
                    E.Cast(gapcloser.End);
                }
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "MissFortuneBulletTime")
            {
                RCastTime = Game.Time;
                Program.debug(args.SData.Name);
                Orbwalker.DisableAttacking = true;
                Orbwalker.DisableMovement = true;
                if (getCheckBoxItem(rMenu, "forceBlockMove"))
                {
                    OktwCommon.blockMove = true;
                    OktwCommon.blockAttack = true;
                    OktwCommon.blockSpells = true;
                }
            }
        }

        private static void afterAttack(AttackableUnit target, EventArgs args)
        {
            LastAttackId = target.NetworkId;

            if (!(target is AIHeroClient))
                return;
            var t = target as AIHeroClient;

            if (Q.IsReady() && t.IsValidTarget(Q.Range))
            {
                if (Q.GetDamage(t) + Player.GetAutoAttackDamage(t)*3 > t.Health)
                    Q.Cast(t);
                else if (Program.Combo && Player.Mana > RMANA + QMANA + WMANA)
                    Q.Cast(t);
                else if (Program.Farm && Player.Mana > RMANA + QMANA + EMANA + WMANA)
                    Q.Cast(t);
            }
            if (W.IsReady())
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && Player.Mana > RMANA + WMANA &&
                    getCheckBoxItem(wMenu, "autoW"))
                    W.Cast();
                else if (Player.Mana > RMANA + WMANA + QMANA && getCheckBoxItem(wMenu, "harasW"))
                    W.Cast();
            }
        }

        private static void Jungle()
        {
            if (Program.LaneClear && Player.Mana > RMANA + WMANA + QMANA)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, 600, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    if (Q.IsReady() && getCheckBoxItem(farmMenu, "jungleQ") && Q.GetDamage(mob) > mob.Health)
                    {
                        Q.Cast(mob);
                        return;
                    }
                    if (W.IsReady() && getCheckBoxItem(farmMenu, "jungleW"))
                    {
                        W.Cast();
                        return;
                    }
                    if (E.IsReady() && getCheckBoxItem(farmMenu, "jungleE"))
                    {
                        E.Cast(mob.ServerPosition);
                    }
                }
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (getKeyBindItem(rMenu, "disableBlock"))
            {
                Orbwalker.DisableAttacking = false;
                Orbwalker.DisableMovement = false;

                OktwCommon.blockSpells = false;
                OktwCommon.blockAttack = false;
                OktwCommon.blockMove = false;
                return;
            }
            if (Player.IsChannelingImportantSpell() || Game.Time - RCastTime < 0.3)
            {
                if (getCheckBoxItem(rMenu, "forceBlockMove"))
                {
                    OktwCommon.blockMove = true;
                    OktwCommon.blockAttack = true;
                    OktwCommon.blockSpells = true;
                }

                Orbwalker.DisableAttacking = true;
                Orbwalker.DisableMovement = true;

                Program.debug("cast R");
                return;
            }
            Orbwalker.DisableAttacking = false;
            Orbwalker.DisableMovement = false;
            if (getCheckBoxItem(rMenu, "forceBlockMove"))
            {
                OktwCommon.blockAttack = false;
                OktwCommon.blockMove = false;
                OktwCommon.blockSpells = false;
            }
            if (R.IsReady() && getKeyBindItem(rMenu, "useR"))
            {
                var t = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                if (t.IsValidTarget(R.Range))
                {
                    R.Cast(t, true, true);
                    RCastTime = Game.Time;
                    return;
                }
            }

            if (getCheckBoxItem(miscMenu, "newTarget"))
            {
                var orbT = Orbwalker.LastTarget;

                AIHeroClient t2 = null;

                if (orbT is AIHeroClient)
                    t2 = (AIHeroClient) orbT;

                if (t2.IsValidTarget() && t2.NetworkId == LastAttackId)
                {
                    var ta = ObjectManager.Get<AIHeroClient>()
                        .FirstOrDefault(enemy => enemy.IsValidTarget() && Orbwalking.InAutoAttackRange(enemy)
                        && (enemy.NetworkId != LastAttackId || enemy.Health < Player.GetAutoAttackDamage(enemy)*2));

                    if (ta != null)
                        Orbwalker.ForcedTarget = ta;
                }
            }

            if (Program.LagFree(1))
            {
                SetMana();
                Jungle();
            }

            if (Program.LagFree(2) && !Orbwalker.IsAutoAttacking && Q.IsReady() && getCheckBoxItem(qMenu, "autoQ"))
                LogicQ();

            if (Program.LagFree(3) && !Orbwalker.IsAutoAttacking && E.IsReady() && getCheckBoxItem(eMenu, "autoE"))
                LogicE();

            if (Program.LagFree(4) && !Orbwalker.IsAutoAttacking && R.IsReady() && getCheckBoxItem(rMenu, "autoR"))
                LogicR();
        }

        private static void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            var t1 = TargetSelector.GetTarget(Q1.Range, DamageType.Physical);
            if (t.IsValidTarget(Q.Range) && Player.Distance(t.ServerPosition) > 500)
            {
                var qDmg = OktwCommon.GetKsDamage(t, Q);
                if (qDmg + Player.GetAutoAttackDamage(t) > t.Health)
                    Q.Cast(t);
                else if (qDmg + Player.GetAutoAttackDamage(t)*3 > t.Health)
                    Q.Cast(t);
                else if (Program.Combo && Player.Mana > RMANA + QMANA + WMANA)
                    Q.Cast(t);
                else if (Program.Farm && Player.Mana > RMANA + QMANA + EMANA + WMANA)
                    Q.Cast(t);
            }
            else if (t1.IsValidTarget(Q1.Range) && getCheckBoxItem(qMenu, "harasQ") &&
                     Player.Distance(t1.ServerPosition) > Q.Range + 50)
            {
                if (getCheckBoxItem(qMenu, "qMinionMove"))
                {
                    var minions = Cache.GetMinions(Player.ServerPosition, Q1.Range);

                    if (minions.Exists(x => x.IsMoving))
                        return;
                }

                Q1.Width = getSliderItem(qMenu, "qMinionWidth");

                var poutput = Q1.GetPrediction(t1);
                var col = poutput.CollisionObjects;
                if (!col.Any())
                    return;

                var minionQ = col.Last();
                if (minionQ.IsValidTarget(Q.Range))
                {
                    if (getCheckBoxItem(qMenu, "killQ") && Q.GetDamage(minionQ) < minionQ.Health)
                        return;
                    var minionToT = minionQ.Distance(t1.Position);
                    var minionToP = minionQ.Distance(poutput.CastPosition);
                    if (minionToP < 400 && minionToT < 420 && minionToT > 150 && minionToP > 200)
                    {
                        if (Q.GetDamage(t1) + Player.GetAutoAttackDamage(t1) > t1.Health)
                            Q.Cast(col.Last());
                        else if (Program.Combo && Player.Mana > RMANA + QMANA + WMANA)
                            Q.Cast(col.Last());
                        else if (Program.Farm && Player.Mana > RMANA + QMANA + EMANA + WMANA + QMANA)
                            Q.Cast(col.Last());
                    }
                }
            }
        }

        private static void LogicE()
        {
            var t = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            if (t.IsValidTarget())
            {
                var eDmg = OktwCommon.GetKsDamage(t, E);
                if (eDmg > t.Health)
                    Program.CastSpell(E, t);
                else if (eDmg + Q.GetDamage(t) > t.Health && Player.Mana > QMANA + EMANA + RMANA)
                    Program.CastSpell(E, t);
                else if (Program.Combo && Player.Mana > RMANA + WMANA + QMANA + EMANA)
                {
                    if (!Orbwalking.InAutoAttackRange(t) || Player.CountEnemiesInRange(300) > 0 ||
                        t.CountEnemiesInRange(250) > 1)
                        Program.CastSpell(E, t);
                    else
                    {
                        foreach (
                            var enemy in
                                Program.Enemies.Where(
                                    enemy => enemy.IsValidTarget(E.Range) && !OktwCommon.CanMove(enemy)))
                            E.Cast(enemy, true, true);
                    }
                }
            }
        }

        private static void LogicR()
        {
            if (Player.UnderTurret(true) && getCheckBoxItem(rMenu, "Rturrent"))
                return;

            var t = TargetSelector.GetTarget(R.Range, DamageType.Physical);

            if (t.IsValidTarget(R.Range) && OktwCommon.ValidUlt(t))
            {
                var rDmg = R.GetDamage(t)*new[] {0.5, 0.75, 1}[R.Level];

                if (Player.CountEnemiesInRange(700) == 0 && t.CountAlliesInRange(400) == 0)
                {
                    var tDis = Player.Distance(t.ServerPosition);
                    if (rDmg*7 > t.Health && tDis < 800)
                    {
                        R.Cast(t, true, true);
                        RCastTime = Game.Time;
                    }
                    else if (rDmg*6 > t.Health && tDis < 900)
                    {
                        R.Cast(t, true, true);
                        RCastTime = Game.Time;
                    }
                    else if (rDmg*5 > t.Health && tDis < 1000)
                    {
                        R.Cast(t, true, true);
                        RCastTime = Game.Time;
                    }
                    else if (rDmg*4 > t.Health && tDis < 1100)
                    {
                        R.Cast(t, true, true);
                        RCastTime = Game.Time;
                    }
                    else if (rDmg*3 > t.Health && tDis < 1200)
                    {
                        R.Cast(t, true, true);
                        RCastTime = Game.Time;
                    }
                    else if (rDmg > t.Health && tDis < 1300)
                    {
                        R.Cast(t, true, true);
                        RCastTime = Game.Time;
                    }
                    return;
                }
                if (rDmg*8 > t.Health - OktwCommon.GetIncomingDamage(t) && rDmg*2 < t.Health &&
                    Player.CountEnemiesInRange(300) == 0 && !OktwCommon.CanMove(t))
                {
                    R.Cast(t, true, true);
                    RCastTime = Game.Time;
                }
            }
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

        public static void drawLine(Vector3 pos1, Vector3 pos2, int bold, Color color)
        {
            var wts1 = Drawing.WorldToScreen(pos1);
            var wts2 = Drawing.WorldToScreen(pos2);

            Drawing.DrawLine(wts1[0], wts1[1], wts2[0], wts2[1], bold, color);
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(drawMenu, "noti") && R.IsReady())
            {
                var t = TargetSelector.GetTarget(R.Range, DamageType.Physical);

                if (t.IsValidTarget())
                {
                    var rDamage = R.GetDamage(t) + W.GetDamage(t)*10;
                    if (rDamage*8 > t.Health)
                    {
                        Drawing.DrawText(Drawing.Width*0.1f, Drawing.Height*0.5f, Color.GreenYellow,
                            "8 x R wave can kill: " + t.ChampionName + " have: " + t.Health + "hp");
                        drawLine(t.Position, Player.Position, 10, Color.GreenYellow);
                    }
                    else if (rDamage*5 > t.Health)
                    {
                        Drawing.DrawText(Drawing.Width*0.1f, Drawing.Height*0.5f, Color.Orange,
                            "5 x R wave can kill: " + t.ChampionName + " have: " + t.Health + "hp");
                        drawLine(t.Position, Player.Position, 10, Color.Orange);
                    }
                    else if (rDamage*3 > t.Health)
                    {
                        Drawing.DrawText(Drawing.Width*0.1f, Drawing.Height*0.5f, Color.Yellow,
                            "3 x R wave can kill: " + t.ChampionName + " have: " + t.Health + "hp");
                        drawLine(t.Position, Player.Position, 10, Color.Yellow);
                    }
                    else if (rDamage > t.Health)
                    {
                        Drawing.DrawText(Drawing.Width*0.1f, Drawing.Height*0.5f, Color.Red,
                            "1 x R wave can kill: " + t.ChampionName + " have: " + t.Health + "hp");
                        drawLine(t.Position, Player.Position, 10, Color.Red);
                    }
                }
            }

            if (getCheckBoxItem(drawMenu, "QRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (W.IsReady())
                        Utility.DrawCircle(Player.Position, Q.Range, Color.Cyan, 1, 1);
                }
                else
                    Utility.DrawCircle(Player.Position, Q.Range, Color.Cyan, 1, 1);
            }
            if (getCheckBoxItem(drawMenu, "ERange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (E.IsReady())
                        Utility.DrawCircle(Player.Position, E.Range, Color.Orange, 1, 1);
                }
                else
                    Utility.DrawCircle(Player.Position, E.Range, Color.Orange, 1, 1);
            }
            if (getCheckBoxItem(drawMenu, "RRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (R.IsReady())
                        Utility.DrawCircle(Player.Position, R.Range, Color.Gray, 1, 1);
                }
                else
                    Utility.DrawCircle(Player.Position, R.Range, Color.Gray, 1, 1);
            }
        }

        public static void drawText(string msg, Obj_AI_Base Hero, Color color)
        {
            var wts = Drawing.WorldToScreen(Hero.Position);
            Drawing.DrawText(wts[0] - msg.Length*5, wts[1], color, msg);
        }
    }
}