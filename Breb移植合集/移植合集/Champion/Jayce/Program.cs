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

namespace OneKeyToWin_AIO_Sebby.Champions
{
    internal class Jayce
    {
        private static readonly Menu Config = Program.Config;
        public static Menu drawMenu, qMenu, wMenu, eMenu, rMenu, harassMenu, farmMenu, miscMenu;
        private static Spell Q, Q2, Qext, QextCol, W, W2, E, E2, R;
        private static float QMANA, WMANA, EMANA, QMANA2, WMANA2, EMANA2;
        private static readonly float RMANA = 0;
        private static float Qcd, Wcd, Ecd, Q2cd, W2cd, E2cd;
        private static float Qcdt, Wcdt, Ecdt = 0, Q2cdt, W2cdt, E2cdt;
        private static Vector3 EcastPos;
        private static int Etick;
        public static int Muramana = 3042;
        public static int Tear = 3070;
        public static int Manamune = 3004;

        public static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        private static bool Range
        {
            get { return Q.Instance.Name.ToLower() == "jayceshockblast"; }
        }


        public static void LoadOKTW()
        {
            #region SET SKILLS

            Q = new Spell(SpellSlot.Q, 1030);
            Qext = new Spell(SpellSlot.Q, 1650);
            QextCol = new Spell(SpellSlot.Q, 1650);
            Q2 = new Spell(SpellSlot.Q, 600);
            W = new Spell(SpellSlot.W);
            W2 = new Spell(SpellSlot.W, 350);
            E = new Spell(SpellSlot.E, 650);
            E2 = new Spell(SpellSlot.E, 240);
            R = new Spell(SpellSlot.R);

            Q.SetSkillshot(0.25f, 70, 1450, true, SkillshotType.SkillshotLine);
            Qext.SetSkillshot(0.30f, 80, 2000, false, SkillshotType.SkillshotLine);
            QextCol.SetSkillshot(0.30f, 100, 1600, true, SkillshotType.SkillshotLine);
            Q2.SetTargetted(0.25f, float.MaxValue);
            E.SetSkillshot(0.1f, 120, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E2.SetTargetted(0.25f, float.MaxValue);

            #endregion

            drawMenu = Config.AddSubMenu("线圈");
            drawMenu.Add("showcd", new CheckBox("显示冷却"));
            drawMenu.Add("noti", new CheckBox("显示提示"));
            drawMenu.Add("onlyRdy", new CheckBox("只显示无冷却技能"));
            drawMenu.Add("qRange", new CheckBox("Q 范围", false));

            qMenu = Config.AddSubMenu("Q 设置");
            qMenu.Add("autoQ", new CheckBox("自动 Q 远程"));
            qMenu.Add("autoQm", new CheckBox("战斗 Q 近程"));
            qMenu.Add("QEforce", new CheckBox("强制 E + Q"));
            qMenu.Add("QEsplash", new CheckBox("Q + E 范围伤害"));
            qMenu.Add("QEsplashAdjust", new Slider("Q + E 范围半径", 150, 250, 50));

            wMenu = Config.AddSubMenu("W 设置");
            wMenu.Add("autoW", new CheckBox("自动 W 远程"));
            wMenu.Add("autoWm", new CheckBox("战斗 W 近程"));
            wMenu.Add("autoWmove", new CheckBox("屏蔽移动如果开启远程 W"));

            eMenu = Config.AddSubMenu("E 设置");
            eMenu.Add("autoE", new CheckBox("自动 E 远程 (Q + E)"));
            eMenu.Add("autoEm", new CheckBox("自动 E 近程"));
            eMenu.Add("autoEks", new CheckBox("只用 近程E 抢头"));
            eMenu.Add("gapE", new CheckBox("防突进 R + E"));
            eMenu.Add("intE", new CheckBox("技能打断 R + Q + E"));

            rMenu = Config.AddSubMenu("R 设置 ");
            rMenu.Add("autoR", new CheckBox("自动 R 远程"));
            rMenu.Add("autoRm", new CheckBox("自动 R 近程"));

            harassMenu = Config.AddSubMenu("骚扰");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != Player.Team))
                harassMenu.Add("haras" + enemy.ChampionName, new CheckBox(enemy.ChampionName));
            harassMenu.Add("harassMana", new Slider("骚扰蓝量", 80));

            miscMenu = Config.AddSubMenu("杂项");
            miscMenu.Add("flee", new KeyBind("逃跑模式", false, KeyBind.BindTypes.HoldActive, 'T'));
            miscMenu.Add("stack", new CheckBox("满蓝时叠加女神"));

            farmMenu = Config.AddSubMenu("农兵");
            farmMenu.Add("farmQ", new CheckBox("清线 Q + E 远程"));
            farmMenu.Add("farmW", new CheckBox("清线 W 远程 && 近程"));
            farmMenu.Add("Mana", new Slider("清线蓝量", 80));
            farmMenu.Add("LCminions", new Slider("清线最少命中小兵数", 2, 0, 10));

            farmMenu.Add("jungleQ", new CheckBox("清野 Q"));
            farmMenu.Add("jungleW", new CheckBox("清野 W"));
            farmMenu.Add("jungleE", new CheckBox("清野 E"));
            farmMenu.Add("jungleR", new CheckBox("清野 R"));

            farmMenu.Add("jungleQm", new CheckBox("清野 Q 近程"));
            farmMenu.Add("jungleWm", new CheckBox("清野 W 近程"));
            farmMenu.Add("jungleEm", new CheckBox("清野 E 近程"));

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += OnUpdate;
            Orbwalker.OnPreAttack += Orbwalker_OnPreAttack;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
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
            if (!getCheckBoxItem(eMenu, "gapE") || E2cd > 0.1)
                return;

            if (Range && !R.IsReady())
                return;

            var t = gapcloser.Sender;

            if (t.IsValidTarget(400))
            {
                if (Range)
                {
                    R.Cast();
                }
                else
                    E.Cast(t);
            }
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient t,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!getCheckBoxItem(eMenu, "intE") || E2cd > 0.1)
                return;

            if (Range && !R.IsReady())
                return;

            if (t.IsValidTarget(300))
            {
                if (Range)
                {
                    R.Cast();
                }
                else
                    E.Cast(t);
            }
            else if (Q2cd < 0.2 && t.IsValidTarget(Q2.Range))
            {
                if (Range)
                {
                    R.Cast();
                }
                else
                {
                    Q.Cast(t);
                    if (t.IsValidTarget(E2.Range))
                        E.Cast(t);
                }
            }
        }

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.Q)
            {
                if (W.IsReady() && !Range && Player.Mana > 80)
                    W.Cast();
                if (E.IsReady() && Range && getCheckBoxItem(qMenu, "QEforce"))
                    E.Cast(Player.ServerPosition.Extend(args.EndPosition, 120));
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name.ToLower() == "jayceshockblast")
            {
                if (Range && E.IsReady() && getCheckBoxItem(eMenu, "autoE"))
                {
                    EcastPos = Player.ServerPosition.LSExtend(args.End, 130 + Game.Ping/2);
                    Etick = Utils.TickCount;
                    E.Cast(EcastPos);
                }
            }
        }

        private static void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (W.IsReady() && getCheckBoxItem(wMenu, "autoW") && Range && args.Target is AIHeroClient)
            {
                if (Program.Combo)
                    W.Cast();
                else if (args.Target.Position.Distance(Player.Position) < 500)
                    W.Cast();
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Range && E.IsReady() && Utils.TickCount - Etick < 250 + Game.Ping)
            {
                E.Cast(EcastPos);
            }

            if (getKeyBindItem(miscMenu, "flee"))
            {
                FleeMode();
            }

            if (Range)
            {
                if (getCheckBoxItem(wMenu, "autoWmove") && Orbwalker.LastTarget != null &&
                    Player.HasBuff("jaycehyperchargevfx"))
                    Orbwalker.DisableMovement = true;
                else
                    Orbwalker.DisableMovement = false;

                if (Program.LagFree(1) && Q.IsReady() && getCheckBoxItem(qMenu, "autoQ"))
                    LogicQ();

                if (Program.LagFree(2) && W.IsReady() && getCheckBoxItem(wMenu, "autoW"))
                    LogicW();
            }
            else
            {
                if (Program.LagFree(1) && E2.IsReady() && getCheckBoxItem(eMenu, "autoEm"))
                    LogicE2();

                if (Program.LagFree(2) && Q2.IsReady() && getCheckBoxItem(qMenu, "autoQm"))
                    LogicQ2();
                if (Program.LagFree(3) && W2.IsReady() && getCheckBoxItem(wMenu, "autoWm"))
                    LogicW2();
            }

            if (Program.LagFree(4))
            {
                if (Program.None && getCheckBoxItem(miscMenu, "stack") && !Player.HasBuff("Recall") &&
                    Player.Mana > Player.MaxMana*0.90 && (Items.HasItem(Tear) || Items.HasItem(Manamune)))
                {
                    if (Utils.TickCount - Q.LastCastAttemptT > 4200 && Utils.TickCount - W.LastCastAttemptT > 4200 &&
                        Utils.TickCount - E.LastCastAttemptT > 4200)
                    {
                        if (Range)
                        {
                            if (W.IsReady())
                                W.Cast();
                            else if (E.IsReady() && (Player.InFountain() || Player.IsMoving))
                                E.Cast(Player.ServerPosition);
                            else if (Q.IsReady() && !E.IsReady())
                                Q.Cast(Player.Position.Extend(Game.CursorPos, 500));
                            else if (R.IsReady() && Player.InFountain())
                                R.Cast();
                        }
                        else
                        {
                            if (W.IsReady())
                                W.Cast();
                            else if (R.IsReady() && Player.InFountain())
                                R.Cast();
                        }
                    }
                }

                SetValue();
                if (R.IsReady())
                    LogicR();
            }

            Jungle();
            LaneClearLogic();
        }

        private static void FleeMode()
        {
            if (Range)
            {
                if (E.IsReady())
                    E.Cast(Player.Position.Extend(Game.CursorPos, 150));
                else if (R.IsReady())
                    R.Cast();
            }
            else
            {
                if (Q2.IsReady())
                {
                    var mobs = Cache.GetMinions(Player.ServerPosition, Q2.Range);


                    if (mobs.Count > 0)
                    {
                        var best = mobs[0];

                        foreach (var mob in mobs.Where(mob => mob.IsValidTarget(Q2.Range)))
                        {
                            if (mob.Distance(Game.CursorPos) < best.Distance(Game.CursorPos))
                                best = mob;
                        }
                        if (best.Distance(Game.CursorPos) + 200 < Player.Distance(Game.CursorPos))
                            Q2.Cast(best);
                    }
                    else if (R.IsReady())
                        R.Cast();
                }
                else if (R.IsReady())
                    R.Cast();
            }
            //Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
        }

        private static void LogicQ()
        {
            var qtype = Q;
            if (CanUseQE())
                qtype = Qext;

            var t = TargetSelector.GetTarget(qtype.Range, DamageType.Physical);

            if (t.IsValidTarget())
            {
                var qDmg = OktwCommon.GetKsDamage(t, qtype);

                if (CanUseQE())
                {
                    qDmg = qDmg*1.4f;
                }

                if (qDmg > t.Health)
                    CastQ(t);
                else if (Program.Combo && Player.Mana > EMANA + QMANA)
                    CastQ(t);
                else if (Program.Farm && Player.ManaPercent > getSliderItem(harassMenu, "harassMana") &&
                         OktwCommon.CanHarras())
                {
                    foreach (var enemy in Program.Enemies.Where(enemy => enemy.IsValidTarget(qtype.Range) && getCheckBoxItem(harassMenu, "haras" + enemy.ChampionName)))
                    {
                        CastQ(enemy);
                    }
                }
                else if ((Program.Combo || Program.Farm) && Player.Mana > RMANA + QMANA + EMANA)
                {
                    foreach (
                        var enemy in
                            Program.Enemies.Where(
                                enemy => enemy.IsValidTarget(qtype.Range) && !OktwCommon.CanMove(enemy)))
                        CastQ(t);
                }
            }
        }

        private static void LogicW()
        {
            if (Program.Combo && R.IsReady() && Range && Orbwalker.LastTarget.IsValidTarget() &&
                Orbwalker.LastTarget is AIHeroClient)
            {
                W.Cast();
            }
        }

        private static void LogicQ2()
        {
            var t = TargetSelector.GetTarget(Q2.Range, DamageType.Physical);

            if (t.IsValidTarget())
            {
                if (OktwCommon.GetKsDamage(t, Q2) > t.Health)
                    Q2.Cast(t);
                else if (Program.Combo && Player.Mana > RMANA + QMANA)
                    Q2.Cast(t);
            }
        }

        private static void LogicW2()
        {
            if (Player.CountEnemiesInRange(300) > 0 && Player.Mana > 80)
                W.Cast();
        }

        private static void LogicE2()
        {
            var t = TargetSelector.GetTarget(E2.Range, DamageType.Physical);
            if (t.IsValidTarget())
            {
                if (OktwCommon.GetKsDamage(t, E2) > t.Health)
                    E2.Cast(t);
                else if (Program.Combo && !getCheckBoxItem(eMenu, "autoEks") && !Player.HasBuff("jaycehyperchargevfx"))
                    E2.Cast(t);
            }
        }

        private static void LogicR()
        {
            if (Range && getCheckBoxItem(rMenu, "autoRm"))
            {
                var t = TargetSelector.GetTarget(Q2.Range + 200, DamageType.Physical);
                if (Program.Combo && Qcd > 0.5 && t.IsValidTarget() &&
                    ((!W.IsReady() && !t.IsMelee) ||
                     (!W.IsReady() && !Player.HasBuff("jaycehyperchargevfx") && t.IsMelee)))
                {
                    if (Q2cd < 0.5 && t.CountEnemiesInRange(800) < 3)
                        R.Cast();
                    else if (Player.CountEnemiesInRange(300) > 0 && E2cd < 0.5)
                        R.Cast();
                }
            }
            else if (Program.Combo && getCheckBoxItem(rMenu, "autoR"))
            {
                var t = TargetSelector.GetTarget(1400, DamageType.Physical);
                if (t.IsValidTarget() && !t.IsValidTarget(Q2.Range + 200) && Q.GetDamage(t)*1.4 > t.Health && Qcd < 0.5 &&
                    Ecd < 0.5)
                {
                    R.Cast();
                }

                if (!Q.IsReady() && (!E.IsReady() || getCheckBoxItem(eMenu, "autoEks")))
                {
                    R.Cast();
                }
            }
        }

        private static void LaneClearLogic()
        {
            if (!Program.LaneClear)
                return;

            if (Range && Q.IsReady() && E.IsReady() && Player.ManaPercent > getSliderItem(farmMenu, "Mana") &&
                getCheckBoxItem(farmMenu, "farmQ") && Player.Mana > RMANA + WMANA)
            {
                var minionList = Cache.GetMinions(Player.ServerPosition, Q.Range);
                var farmPosition = QextCol.GetCircularFarmLocation(minionList, 150);

                if (farmPosition.MinionsHit > getSliderItem(farmMenu, "LCminions"))
                    Q.Cast(farmPosition.Position);
            }

            if (W.IsReady() && Player.ManaPercent > getSliderItem(farmMenu, "Mana") &&
                getCheckBoxItem(farmMenu, "farmW"))
            {
                if (Range)
                {
                    Program.debug("csa");
                    var mobs = Cache.GetMinions(Player.ServerPosition, 550);
                    if (mobs.Count >= getSliderItem(farmMenu, "LCminions"))
                    {
                        W.Cast();
                    }
                }
                else
                {
                    var mobs = Cache.GetMinions(Player.ServerPosition, 300);
                    if (mobs.Count >= getSliderItem(farmMenu, "LCminions"))
                    {
                        W.Cast();
                    }
                }
            }
        }

        private static void Jungle()
        {
            if (Program.LaneClear && Player.Mana > RMANA + WMANA + WMANA)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, 700, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    if (Range)
                    {
                        if (Q.IsReady() && getCheckBoxItem(farmMenu, "jungleQ"))
                        {
                            Q.Cast(mob.ServerPosition);
                            return;
                        }
                        if (W.IsReady() && getCheckBoxItem(farmMenu, "jungleE"))
                        {
                            if (Orbwalking.InAutoAttackRange(mob))
                                W.Cast();
                            return;
                        }
                        if (getCheckBoxItem(farmMenu, "jungleR"))
                            R.Cast();
                    }
                    else
                    {
                        if (Q2.IsReady() && getCheckBoxItem(farmMenu, "jungleQm") && mob.IsValidTarget(Q2.Range))
                        {
                            Q2.Cast(mob);
                            return;
                        }

                        if (W2.IsReady() && getCheckBoxItem(farmMenu, "jungleWm"))
                        {
                            if (mob.IsValidTarget(300))
                                W.Cast();
                            return;
                        }
                        if (E2.IsReady() && getCheckBoxItem(farmMenu, "jungleEm") && mob.IsValidTarget(E2.Range))
                        {
                            if (mob.IsValidTarget(E2.Range))
                                E2.Cast(mob);
                            return;
                        }
                        if (getCheckBoxItem(farmMenu, "jungleR"))
                            R.Cast();
                    }
                }
            }
        }

        private static void CastQ(Obj_AI_Base t)
        {
            if (!CanUseQE())
            {
                Program.CastSpell(Q, t);
                return;
            }

            var cast = true;

            if (getCheckBoxItem(qMenu, "QEsplash"))
            {
                var poutput = QextCol.GetPrediction(t);

                if (poutput.CollisionObjects.Any(minion => minion.IsEnemy && minion.Distance(poutput.CastPosition) > getSliderItem(qMenu, "QEsplashAdjust")))
                {
                    cast = false;
                }
            }
            else
                cast = false;

            Program.CastSpell(cast ? Qext : QextCol, t);
        }

        private static float GetComboDMG(Obj_AI_Base t)
        {
            float comboDMG = 0;

            if (Qcd < 1 && Ecd < 1)
                comboDMG = Q.GetDamage(t)*1.4f;
            else if (Qcd < 1)
                comboDMG = Q.GetDamage(t);

            if (Q2cd < 1)
                comboDMG = Q.GetDamage(t, 1);

            if (Wcd < 1)
                comboDMG += Player.GetAutoAttackDamage(t)*3;

            if (W2cd < 1)
                comboDMG += W.GetDamage(t)*2;

            if (E2cd < 1)
                comboDMG += E.GetDamage(t)*3;
            return comboDMG;
        }

        private static bool CanUseQE()
        {
            if (E.IsReady() && Player.Mana > QMANA + EMANA && getCheckBoxItem(eMenu, "autoE"))
                return true;
            return false;
        }

        private static float SetPlus(float valus)
        {
            if (valus < 0)
                return 0;
            return valus;
        }

        private static void SetValue()
        {
            if (Range)
            {
                Qcdt = Q.Instance.CooldownExpires;
                Wcdt = W.Instance.CooldownExpires;
                Ecd = E.Instance.CooldownExpires;

                QMANA = Q.Instance.SData.Mana;
                WMANA = W.Instance.SData.Mana;
                EMANA = E.Instance.SData.Mana;
            }
            else
            {
                Q2cdt = Q.Instance.CooldownExpires;
                W2cdt = W.Instance.CooldownExpires;
                E2cdt = E.Instance.CooldownExpires;

                QMANA2 = Q.Instance.SData.Mana;
                WMANA2 = W.Instance.SData.Mana;
                EMANA2 = E.Instance.SData.Mana;
            }

            Qcd = SetPlus(Qcdt - Game.Time);
            Wcd = SetPlus(Wcdt - Game.Time);
            Ecd = SetPlus(Ecdt - Game.Time);
            Q2cd = SetPlus(Q2cdt - Game.Time);
            W2cd = SetPlus(W2cdt - Game.Time);
            E2cd = SetPlus(E2cdt - Game.Time);
        }

        public static void drawLine(Vector3 pos1, Vector3 pos2, int bold, Color color)
        {
            var wts1 = Drawing.WorldToScreen(pos1);
            var wts2 = Drawing.WorldToScreen(pos2);

            Drawing.DrawLine(wts1[0], wts1[1], wts2[0], wts2[1], bold, color);
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(drawMenu, "showcd"))
            {
                var msg = " ";

                if (Range)
                {
                    msg = "Q " + (int) Q2cd + "   W " + (int) W2cd + "   E " + (int) E2cd;
                    Drawing.DrawText(Drawing.Width*0.5f - 50, Drawing.Height*0.3f, Color.Orange, msg);
                }
                else
                {
                    msg = "Q " + (int) Qcd + "   W " + (int) Wcd + "   E " + (int) Ecd;
                    Drawing.DrawText(Drawing.Width*0.5f - 50, Drawing.Height*0.3f, Color.Aqua, msg);
                }
            }


            if (getCheckBoxItem(drawMenu, "qRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (Q.IsReady())
                    {
                        if (Range)
                        {
                            Utility.DrawCircle(Player.Position, CanUseQE() ? Qext.Range : Q.Range, Color.Cyan, 1, 1);
                        }
                        else
                            Utility.DrawCircle(Player.Position, Q2.Range, Color.Orange, 1, 1);
                    }
                }
                else
                {
                    if (Range)
                    {
                        Utility.DrawCircle(Player.Position, CanUseQE() ? Qext.Range : Q.Range, Color.Cyan, 1, 1);
                    }
                    else
                        Utility.DrawCircle(Player.Position, Q2.Range, Color.Orange, 1, 1);
                }
            }

            if (getCheckBoxItem(drawMenu, "noti"))
            {
                var t = TargetSelector.GetTarget(1600, DamageType.Physical);

                if (t.IsValidTarget())
                {
                    var damageCombo = GetComboDMG(t);
                    if (damageCombo > t.Health)
                    {
                        Drawing.DrawText(Drawing.Width*0.1f, Drawing.Height*0.5f, Color.Red,
                            "Combo deal  " + damageCombo + " to " + t.ChampionName);
                        drawLine(t.Position, Player.Position, 10, Color.Yellow);
                    }
                }
            }
        }
    }
}