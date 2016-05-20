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
    class Brand
    {
        private static Menu Config = Program.Config;
        private static LeagueSharp.Common.Spell Q, W, E, R;
        private static float QMANA = 0, WMANA = 0, EMANA = 0, RMANA = 0;
        public static AIHeroClient Player { get { return ObjectManager.Player; } }

        public static Menu drawMenu, qMenu, wMenu, eMenu, rMenu, harassMenu, farmMenu;

        public static void LoadOKTW()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 1000);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 940);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 625);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 750);

            Q.SetSkillshot(0.25f, 60f, 1600f, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(1.15f, 230f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            R.SetTargetted(0.25f, 2000f);

            drawMenu = Config.AddSubMenu("Drawings");
            drawMenu.Add("noti", new CheckBox("Show notification & line", true));
            drawMenu.Add("onlyRdy", new CheckBox("Draw only ready spells", true));
            drawMenu.Add("qRange", new CheckBox("Q range", false));
            drawMenu.Add("wRange", new CheckBox("W range", false));
            drawMenu.Add("eRange", new CheckBox("E range", false));
            drawMenu.Add("rRange", new CheckBox("R range", false));

            qMenu = Config.AddSubMenu("Q Config");
            qMenu.Add("autoQ", new CheckBox("Auto Q", true));
            qMenu.Add("QAblazed", new CheckBox("Q only if ablazed", true));
            qMenu.Add("harrasQ", new CheckBox("Harass Q", true));
            qMenu.Add("gapQ", new CheckBox("Gapcloser E + Q", true));
            qMenu.Add("intQ", new CheckBox("Interrupt spells E + Q", true));

            wMenu = Config.AddSubMenu("W Config");
            wMenu.Add("autoW", new CheckBox("Auto W", true));
            wMenu.Add("harrasW", new CheckBox("Harass W", true));

            eMenu = Config.AddSubMenu("E Config");
            eMenu.Add("autoE", new CheckBox("Auto E", true));
            eMenu.Add("harrasE", new CheckBox("Harass E", true));
            eMenu.Add("minionE", new CheckBox("use E on ablazed minion", true));

            rMenu = Config.AddSubMenu("R Config");
            rMenu.Add("autoR", new CheckBox("Auto R", true));
            rMenu.Add("rCount", new Slider("Auto R if can hit x enemies", 3, 0, 5));

            harassMenu = Config.AddSubMenu("Harass");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                harassMenu.Add("harras" + enemy.ChampionName, new CheckBox(enemy.ChampionName));

            farmMenu = Config.AddSubMenu("Farm");
            farmMenu.Add("farmE", new CheckBox("Lane clear E", true));
            farmMenu.Add("farmW", new CheckBox("Lane clear W", true));
            farmMenu.Add("Mana", new Slider("LaneClear Mana", 80, 0, 100));
            farmMenu.Add("LCminions", new Slider("LaneClear minimum minions", 2, 0, 10));
            farmMenu.Add("jungleE", new CheckBox("Jungle clear E", true));
            farmMenu.Add("jungleQ", new CheckBox("Jungle clear Q", true));
            farmMenu.Add("jungleW", new CheckBox("Jungle clear W", true));

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
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
            if (!getCheckBoxItem(qMenu, "gapQ") || Player.Mana < QMANA + EMANA)
                return;

            var t = gapcloser.Sender;

            if (t.IsValidTarget(E.Range) && (t.HasBuff("brandablaze") || E.IsReady()))
            {

                E.CastOnUnit(t);
                if (Q.IsReady())
                    Q.Cast(t);
            }
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient t, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!getCheckBoxItem(qMenu, "intQ") || Player.Mana < QMANA + EMANA)
                return;

            if (t.IsValidTarget(E.Range) && (t.HasBuff("brandablaze") || E.IsReady()))
            {
                E.CastOnUnit(t);
                if (Q.IsReady())
                    Q.Cast(t);
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Program.Combo)
            {
                if (!E.IsReady())
                    Orbwalker.DisableAttacking = false;
                else
                    Orbwalker.DisableAttacking = true;
            }
            else
                Orbwalker.DisableAttacking = false;

            if (Program.LagFree(0))
            {
                SetMana();
                Jungle();
            }

            if (Program.LagFree(1) && E.IsReady() && getCheckBoxItem(eMenu, "autoE"))
                LogicE();
            if (Program.LagFree(2) && Q.IsReady() && getCheckBoxItem(qMenu, "autoQ"))
                LogicQ();
            if (Program.LagFree(3) && W.IsReady() && getCheckBoxItem(wMenu, "autoW"))
                LogicW();
            if (Program.LagFree(4) && R.IsReady() && getCheckBoxItem(rMenu, "autoR"))
                LogicR();
        }

        private static void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (t.IsValidTarget())
            {
                if (OktwCommon.GetKsDamage(t, Q) + BonusDmg(t) + OktwCommon.GetEchoLudenDamage(t) > t.Health)
                    Program.CastSpell(Q, t);

                if (!t.HasBuff("brandablaze") && getCheckBoxItem(qMenu, "QAblazed"))
                {
                    var otherEnemy = t;

                    foreach (var enemy in Program.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range) && enemy.HasBuff("brandablaze")))
                        t = enemy;

                    if (otherEnemy == t && !LogicQuse(t))
                        return;
                }

                if (Program.Combo && Player.Mana > RMANA + QMANA)
                    Program.CastSpell(Q, t);
                else if (Program.Farm && getCheckBoxItem(qMenu, "harrasQ") && Player.Mana > RMANA + EMANA + WMANA + EMANA)
                    Program.CastSpell(Q, t);

                if (Player.Mana > RMANA + QMANA)
                {
                    foreach (var enemy in Program.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                        Q.Cast(enemy);
                }
            }
        }

        private static bool LogicQuse(Obj_AI_Base t)
        {
            if (t.HasBuff("brandablaze"))
                return true;
            else if (E.Instance.CooldownExpires - Game.Time + 2 >= Q.Instance.Cooldown && W.Instance.CooldownExpires - Game.Time + 2 >= Q.Instance.Cooldown)
                return true;
            else
                return false;
        }

        private static void LogicW()
        {
            var t = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            if (t.IsValidTarget())
            {
                if (Program.Combo && Player.Mana > RMANA + WMANA)
                    Program.CastSpell(W, t);
                else if (Program.Farm && getCheckBoxItem(wMenu, "harrasW") && getCheckBoxItem(harassMenu, "harras" + t.ChampionName) && Player.Mana > RMANA + WMANA + EMANA + QMANA + WMANA && OktwCommon.CanHarras())
                    Program.CastSpell(W, t);
                else
                {
                    var qDmg = Q.GetDamage(t);
                    var wDmg = OktwCommon.GetKsDamage(t, W) + BonusDmg(t);
                    if (wDmg > t.Health)
                    {
                        Program.CastSpell(W, t);
                    }
                    else if (wDmg + qDmg > t.Health && Player.Mana > QMANA + EMANA)
                        Program.CastSpell(W, t);
                }

                if (Player.Mana > RMANA + WMANA)
                {
                    foreach (var enemy in Program.Enemies.Where(enemy => enemy.IsValidTarget(W.Range) && !OktwCommon.CanMove(enemy)))
                        W.Cast(enemy, true);
                }
            }
            else if (Program.LaneClear && Player.ManaPercent > getSliderItem(farmMenu, "Mana") && getCheckBoxItem(farmMenu, "farmW") && Player.Mana > RMANA + QMANA + WMANA)
            {
                var allMinions = Cache.GetMinions(Player.ServerPosition, W.Range);
                var farmPos = W.GetCircularFarmLocation(allMinions, W.Width);
                if (farmPos.MinionsHit >= getSliderItem(farmMenu, "LCminions"))
                    W.Cast(farmPos.Position);
            }
        }

        private static void LogicE()
        {
            var t = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            if (t.IsValidTarget())
            {
                if (Program.Combo && Player.Mana > RMANA + EMANA)
                    E.CastOnUnit(t);
                else if (Program.Farm && getCheckBoxItem(eMenu, "harrasE") && Player.Mana > RMANA + EMANA + WMANA + EMANA)
                    E.CastOnUnit(t);
                else
                {
                    var eDmg = OktwCommon.GetKsDamage(t, E) + BonusDmg(t) + OktwCommon.GetEchoLudenDamage(t);
                    var wDmg = W.GetDamage(t);
                    if (eDmg > t.Health)
                        E.CastOnUnit(t);
                    else if (wDmg + eDmg > t.Health && Player.Mana > WMANA + EMANA)
                        E.CastOnUnit(t);
                }
            }
            else
            {
                if (getCheckBoxItem(eMenu, "minionE"))
                {
                    if ((Program.Combo && Player.Mana > RMANA + EMANA) || (Program.Farm && getCheckBoxItem(eMenu, "harrasE") && Player.Mana > RMANA + EMANA))
                    {
                        foreach (var minion in Cache.GetMinions(Player.Position, E.Range).Where(minion => minion.IsValidTarget(E.Range) && minion.CountEnemiesInRange(300) > 0 && minion.HasBuff("brandablaze")))
                        {
                            E.CastOnUnit(minion);
                        }
                    }
                    if (Program.LaneClear && Player.ManaPercent > getSliderItem(farmMenu, "Mana") && getCheckBoxItem(farmMenu, "farmE") && Player.Mana > RMANA + EMANA)
                    {
                        foreach (var minion in Cache.GetMinions(Player.Position, E.Range).Where(minion => minion.IsValidTarget(E.Range) && minion.HasBuff("brandablaze") && CountMinionsInRange(400, minion.Position) >= getSliderItem(farmMenu, "LCminions")))
                        {
                            E.CastOnUnit(minion);
                        }
                    }
                }
            }
        }

        private static void LogicR()
        {
            var bounceRange = 430;
            var t2 = TargetSelector.GetTarget(R.Range + bounceRange, DamageType.Magical);

            if (t2.IsValidTarget(R.Range) && t2.CountEnemiesInRange(bounceRange) >= getSliderItem(rMenu, "rCount") && getSliderItem(rMenu, "rCount") > 0)
                R.Cast(t2);

            if (t2.IsValidTarget() && OktwCommon.ValidUlt(t2))
            {
                if (t2.CountAlliesInRange(550) == 0 || Player.HealthPercent < 50 || t2.CountEnemiesInRange(bounceRange) > 1)
                {
                    var prepos = R.GetPrediction(t2).CastPosition;
                    var dmgR = R.GetDamage(t2);

                    if (t2.Health < dmgR * 3)
                    {
                        var totalDmg = dmgR;
                        var minionCount = CountMinionsInRange(bounceRange, prepos);

                        if (t2.IsValidTarget(R.Range))
                        {
                            if (prepos.CountEnemiesInRange(bounceRange) > 1)
                            {
                                if (minionCount > 2)
                                    totalDmg = dmgR * 2;
                                else
                                    totalDmg = dmgR * 3;
                            }
                            else if (minionCount > 0)
                            {
                                totalDmg = dmgR * 2;
                            }

                            if (W.IsReady())
                            {
                                totalDmg += W.GetDamage(t2);
                            }

                            if (E.IsReady())
                            {
                                totalDmg += E.GetDamage(t2);
                            }

                            if (Q.IsReady())
                            {
                                totalDmg += Q.GetDamage(t2);
                            }

                            totalDmg += BonusDmg(t2);
                            totalDmg += OktwCommon.GetEchoLudenDamage(t2);

                            if (Items.HasItem(3155, t2))
                            {
                                totalDmg = totalDmg - 250;
                            }

                            if (Items.HasItem(3156, t2))
                            {
                                totalDmg = totalDmg - 400;
                            }

                            if (totalDmg > t2.Health - OktwCommon.GetIncomingDamage(t2) && Player.GetAutoAttackDamage(t2) * 2 < t2.Health)
                            {

                                R.CastOnUnit(t2);
                            }

                        }
                        else if (t2.Health - OktwCommon.GetIncomingDamage(t2) < dmgR * 2 + BonusDmg(t2))
                        {
                            if (Player.CountEnemiesInRange(R.Range) > 0)
                            {
                                foreach (var t in Program.Enemies.Where(enemy => enemy.IsValidTarget(R.Range) && enemy.Distance(prepos) < bounceRange))
                                {
                                    R.CastOnUnit(t);
                                }
                            }
                            else
                            {
                                var minions = Cache.GetMinions(Player.Position, R.Range);
                                foreach (var minion in minions.Where(minion => minion.IsValidTarget(R.Range) && minion.Distance(prepos) < bounceRange))
                                {
                                    R.CastOnUnit(minion);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void Jungle()
        {
            if (Program.LaneClear && Player.Mana > RMANA + WMANA + RMANA + WMANA)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, 600, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    if (W.IsReady() && getCheckBoxItem(farmMenu, "jungleW"))
                    {
                        W.Cast(mob.ServerPosition);
                        return;
                    }

                    if (Q.IsReady() && getCheckBoxItem(farmMenu, "jungleQ"))
                    {
                        Q.Cast(mob.ServerPosition);
                        return;
                    }

                    if (E.IsReady() && getCheckBoxItem(farmMenu, "jungleE") && mob.HasBuff("brandablaze"))
                    {
                        E.Cast(mob);
                        return;
                    }
                }
            }
        }

        private static int CountMinionsInRange(float range, Vector3 pos)
        {
            var minions = Cache.GetMinions(pos, range);
            int count = 0;
            foreach (var minion in minions)
            {
                count++;
            }
            return count;
        }

        private static float BonusDmg(AIHeroClient target)
        {
            return (float)Player.CalcDamage(target, DamageType.Magical, (target.MaxHealth * 0.08) - (target.HPRegenRate * 5));
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
                RMANA = QMANA - Player.PARRegenRate * Q.Instance.Cooldown;
            else
                RMANA = R.Instance.SData.Mana;
        }

        public static void drawLine(Vector3 pos1, Vector3 pos2, int bold, System.Drawing.Color color)
        {
            var wts1 = Drawing.WorldToScreen(pos1);
            var wts2 = Drawing.WorldToScreen(pos2);

            Drawing.DrawLine(wts1[0], wts1[1], wts2[0], wts2[1], bold, color);
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(drawMenu, "qRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (Q.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
            }

            if (getCheckBoxItem(drawMenu, "wRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (W.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(Player.Position, W.Range, System.Drawing.Color.Orange, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(Player.Position, W.Range, System.Drawing.Color.Orange, 1, 1);
            }

            if (getCheckBoxItem(drawMenu, "eRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (E.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(Player.Position, E.Range, System.Drawing.Color.Yellow, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(Player.Position, E.Range, System.Drawing.Color.Yellow, 1, 1);
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

            if (getCheckBoxItem(drawMenu, "noti") && R.IsReady())
            {
                var t = TargetSelector.GetTarget(1000, DamageType.Physical);

                if (t.IsValidTarget())
                {
                    var rDamage = R.GetDamage(t);
                    if (rDamage * 3 > t.Health)
                    {
                        Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.5f, System.Drawing.Color.Red, "3 x Ult can kill: " + t.ChampionName + " have: " + t.Health + "hp");
                        drawLine(t.Position, Player.Position, 10, System.Drawing.Color.Yellow);
                    }
                    else if (rDamage * 2 > t.Health)
                    {
                        Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.5f, System.Drawing.Color.Red, "2 x Ult can kill: " + t.ChampionName + " have: " + t.Health + "hp");
                        drawLine(t.Position, Player.Position, 10, System.Drawing.Color.Yellow);
                    }
                    else if (rDamage > t.Health)
                    {
                        Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.5f, System.Drawing.Color.Red, "1 x Ult can kill: " + t.ChampionName + " have: " + t.Health + "hp");
                        drawLine(t.Position, Player.Position, 10, System.Drawing.Color.Yellow);
                    }
                }
            }
        }
    }
}