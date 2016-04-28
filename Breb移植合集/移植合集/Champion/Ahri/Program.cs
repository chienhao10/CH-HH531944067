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
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    internal class Ahri
    {
        private static readonly Menu Config = Program.Config;
        private static Spell Q, W, E, R;
        private static float QMANA, WMANA, EMANA, RMANA;
        private static GameObject QMissile, EMissile;
        public static AIHeroClient Qtarget = null;
        public static MissileReturn missileManager;

        private static Menu drawMenu, QMenu, WMenu, EMenu, RMenu, FarmMenu, harassMenu;

        public static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
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

        public static void LoadOKTW()
        {
            Q = new Spell(SpellSlot.Q, 870);
            W = new Spell(SpellSlot.W, 580);
            E = new Spell(SpellSlot.E, 920);
            R = new Spell(SpellSlot.R, 600);

            Q.SetSkillshot(0.25f, 90, 1550, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 70, 1550, true, SkillshotType.SkillshotLine);

            missileManager = new MissileReturn("AhriOrbMissile", "AhriOrbReturn", Q);

            drawMenu = Config.AddSubMenu("线圈");
            drawMenu.Add("noti", new CheckBox("显示提示 & 线"));
            drawMenu.Add("onlyRdy", new CheckBox("只显示无冷却技能"));
            drawMenu.Add("qRange", new CheckBox("Q 范围", false));
            drawMenu.Add("wRange", new CheckBox("W 范围", false));
            drawMenu.Add("eRange", new CheckBox("E 范围", false));
            drawMenu.Add("rRange", new CheckBox("R 范围", false));
            drawMenu.Add("Qhelp", new CheckBox("显示 Q助手"));

            QMenu = Config.AddSubMenu("Q 设置");
            QMenu.Add("autoQ", new CheckBox("自动 Q"));
            QMenu.Add("harrasQ", new CheckBox("骚扰 Q"));
            QMenu.Add("aimQ", new CheckBox("自动校准 Q 物体"));

            WMenu = Config.AddSubMenu("W 设置");
            WMenu.Add("autoW", new CheckBox("自动 W"));
            WMenu.Add("harrasW", new CheckBox("骚扰 W"));

            EMenu = Config.AddSubMenu("E 设置");
            EMenu.Add("autoE", new CheckBox("自动 E"));
            EMenu.Add("harrasE", new CheckBox("骚扰 E"));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                EMenu.Add("Eon" + enemy.ChampionName, new CheckBox("E : " + enemy.ChampionName));
            EMenu.AddSeparator();
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                EMenu.Add("Egapcloser" + enemy.ChampionName, new CheckBox("Gapclose : " + enemy.ChampionName));

            RMenu = Config.AddSubMenu("R 设置");
            RMenu.Add("autoR", new CheckBox("R 抢头 "));
            RMenu.Add("autoR2", new CheckBox("自动 团战R逻辑 + Q 校准"));

            harassMenu = Config.AddSubMenu("骚扰");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                harassMenu.Add("harras" + enemy.ChampionName, new CheckBox(enemy.ChampionName));

            FarmMenu = Config.AddSubMenu("农兵");
            FarmMenu.Add("farmQ", new CheckBox("清线 Q"));
            FarmMenu.Add("farmW", new CheckBox("清线 W", false));
            FarmMenu.Add("Mana", new Slider("清线蓝量", 80));
            FarmMenu.Add("LCminions", new Slider("清线最低小兵数量", 2, 0, 10));
            FarmMenu.Add("jungleQ", new CheckBox("清野 Q"));
            FarmMenu.Add("jungleW", new CheckBox("清野 W"));

            Game.OnUpdate += Game_OnGameUpdate;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Drawing.OnDraw += Drawing_OnDraw;
            GameObject.OnCreate += SpellMissile_OnCreateOld;
            GameObject.OnDelete += Obj_SpellMissile_OnDelete;
        }

        private static void Obj_SpellMissile_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender.IsEnemy || sender.Type != GameObjectType.MissileClient || !sender.IsValid<MissileClient>())
                return;

            var missile = (MissileClient)sender;

            if (missile.SData.Name != null)
            {
                if (missile.SData.Name == "AhriOrbReturn")
                    QMissile = null;
                if (missile.SData.Name == "AhriSeduceMissile")
                    EMissile = null;
            }
        }

        private static void SpellMissile_OnCreateOld(GameObject sender, EventArgs args)
        {
            if (sender.IsEnemy || sender.Type != GameObjectType.MissileClient || !sender.IsValid<MissileClient>())
                return;

            var missile = (MissileClient)sender;

            if (missile.SData.Name != null)
            {
                if (missile.SData.Name == "AhriOrbMissile" || missile.SData.Name == "AhriOrbReturn")
                {
                    QMissile = sender;
                }
                if (missile.SData.Name == "AhriSeduceMissile")
                {
                    EMissile = sender;
                }
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (E.IsReady() && gapcloser.Sender.IsValidTarget(E.Range) &&
                getCheckBoxItem(EMenu, "Egapcloser" + gapcloser.Sender.ChampionName))
            {
                E.Cast(gapcloser.Sender);
            }
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (E.IsReady() && Player.Distance(sender.ServerPosition) < E.Range)
            {
                E.Cast(sender);
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Program.LagFree(0))
            {
                SetMana();
                Jungle();
            }

            if (E.IsReady() && getCheckBoxItem(EMenu, "autoE"))
                LogicE();
            if (Program.LagFree(2) && W.IsReady() && getCheckBoxItem(WMenu, "autoW"))
                LogicW();
            if (Program.LagFree(3) && Q.IsReady() && getCheckBoxItem(QMenu, "autoQ"))
                LogicQ();
            if (Program.LagFree(4) && R.IsReady() && Program.Combo)
                LogicR();
        }

        private static void LogicR()
        {
            var dashPosition = Player.Position.LSExtend(Game.CursorPos, 450);

            if (Player.Distance(Game.CursorPos) < 450)
                dashPosition = Game.CursorPos;

            if (dashPosition.CountEnemiesInRange(800) > 2)
                return;

            if (getCheckBoxItem(RMenu, "autoR2"))
            {
                if (Player.HasBuff("AhriTumble"))
                {
                    var BuffTime = OktwCommon.GetPassiveTime(Player, "AhriTumble");
                    if (BuffTime < 3)
                    {
                        R.Cast(dashPosition);
                    }

                    var posPred = missileManager.CalculateReturnPos();
                    if (posPred != Vector3.Zero)
                    {
                        if (missileManager.Missile.SData.Name == "AhriOrbReturn" && Player.Distance(posPred) > 200)
                        {
                            R.Cast(posPred);
                            Program.debug("AIMMMM");
                        }
                    }
                }
            }

            if (getCheckBoxItem(RMenu, "autoR"))
            {
                var t = TargetSelector.GetTarget(450 + R.Range, DamageType.Magical);
                if (t.IsValidTarget())
                {
                    var comboDmg = R.GetDamage(t) * 3;
                    if (Q.IsReady())
                    {
                        comboDmg += Q.GetDamage(t) * 2;
                    }
                    if (W.IsReady())
                    {
                        comboDmg += W.GetDamage(t) + W.GetDamage(t, 1);
                    }
                    if (t.CountAlliesInRange(600) < 2 && comboDmg > t.Health && t.Position.Distance(Game.CursorPos) < t.Position.Distance(Player.Position) && dashPosition.Distance(t.ServerPosition) < 500)
                    {
                        R.Cast(dashPosition);
                    }
                    foreach (var target in Program.Enemies.Where(target => target.IsMelee && target.IsValidTarget(300)))
                    {
                        R.Cast(dashPosition);
                    }
                }
            }
        }

        private static void LogicW()
        {
            var t = TargetSelector.GetTarget(W.Range, DamageType.Magical);
            if (t.IsValidTarget())
            {
                if (Program.Combo && Player.Mana > RMANA + WMANA)
                    W.Cast();
                else if (Program.Farm && Player.Mana > RMANA + QMANA + WMANA && getCheckBoxItem(WMenu, "harrasW") &&
                         getCheckBoxItem(harassMenu, "harras" + t.ChampionName))
                    W.Cast();
                else if (W.GetDamage(t) + W.GetDamage(t, 1) + Q.GetDamage(t) * 2 >
                         t.Health - OktwCommon.GetIncomingDamage(t))
                    W.Cast();
            }
            else if (Program.LaneClear && QMissile == null && Player.ManaPercent > getSliderItem(FarmMenu, "Mana") &&
                     getCheckBoxItem(FarmMenu, "farmW") && Player.Mana > RMANA + WMANA)
            {
                var minionList = Cache.GetMinions(Player.ServerPosition, W.Range);
                foreach (var minion in minionList.Where(minion => minion.Health < W.GetDamage(minion)))
                {
                    W.Cast();
                }
            }
        }

        private static void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (t.IsValidTarget())
            {
                missileManager.Target = t;
                if (EMissile == null || !EMissile.IsValid)
                {
                    if (Program.Combo && Player.Mana > RMANA + QMANA)
                        Program.CastSpell(Q, t);
                    else if (Program.Farm && Player.Mana > RMANA + WMANA + QMANA + QMANA &&
                             getCheckBoxItem(QMenu, "harrasQ") && getCheckBoxItem(harassMenu, "harras" + t.ChampionName) &&
                             OktwCommon.CanHarras())
                        Program.CastSpell(Q, t);
                    else if (Q.GetDamage(t) * 2 + OktwCommon.GetEchoLudenDamage(t) >
                             t.Health - OktwCommon.GetIncomingDamage(t))
                        Q.Cast(t, true);
                }
                if (!Program.None && Player.Mana > RMANA + WMANA)
                {
                    foreach (
                        var enemy in
                            Program.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                        Q.Cast(enemy, true);
                }
            }
            else if (Program.LaneClear && Player.ManaPercent > getSliderItem(FarmMenu, "Mana") &&
                     getCheckBoxItem(FarmMenu, "farmQ") && Player.Mana > RMANA + QMANA)
            {
                var minionList = Cache.GetMinions(Player.ServerPosition, Q.Range);
                var farmPosition = Q.GetLineFarmLocation(minionList, Q.Width);
                if (farmPosition.MinionsHit > getSliderItem(FarmMenu, "LCminions"))
                    Q.Cast(farmPosition.Position);
            }
        }

        private static void LogicE()
        {
            foreach (
                var enemy in
                    Program.Enemies.Where(
                        enemy =>
                            enemy.IsValidTarget(E.Range) &&
                            E.GetDamage(enemy) + Q.GetDamage(enemy) + W.GetDamage(enemy) +
                            OktwCommon.GetEchoLudenDamage(enemy) > enemy.Health))
            {
                Program.CastSpell(E, enemy);
            }
            var t = Orbwalker.LastTarget as AIHeroClient;
            if (!t.IsValidTarget())
                t = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            if (t.IsValidTarget())
            {
                if (Program.Combo && Player.Mana > RMANA + EMANA && getCheckBoxItem(EMenu, "Eon" + t.ChampionName))
                    Program.CastSpell(E, t);
                else if (Program.Farm && getCheckBoxItem(EMenu, "harrasE") &&
                         getCheckBoxItem(harassMenu, "harras" + t.ChampionName) &&
                         Player.Mana > RMANA + EMANA + WMANA + EMANA)
                    Program.CastSpell(E, t);
                else if (OktwCommon.GetKsDamage(t, E) > t.Health)
                    Program.CastSpell(E, t);
                if (Player.Mana > RMANA + EMANA)
                {
                    foreach (
                        var enemy in
                            Program.Enemies.Where(
                                enemy =>
                                    enemy.IsValidTarget(E.Range) && !OktwCommon.CanMove(enemy) &&
                                    getCheckBoxItem(EMenu, "Eon" + enemy.ChampionName)))
                        E.Cast(enemy);
                }
            }
        }

        private static void Jungle()
        {
            if (Program.LaneClear && Player.Mana > QMANA + RMANA)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, 600, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    if (W.IsReady() && getCheckBoxItem(FarmMenu, "jungleW"))
                    {
                        W.Cast();
                        return;
                    }
                    if (Q.IsReady() && getCheckBoxItem(FarmMenu, "jungleQ"))
                    {
                        Q.Cast(mob.Position);
                    }
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
                RMANA = QMANA - Player.PARRegenRate * Q.Instance.Cooldown;
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
                        Utility.DrawCircle(Player.Position, W.Range, Color.Orange, 1, 1);
                }
                else
                    Utility.DrawCircle(Player.Position, W.Range, Color.Orange, 1, 1);
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

            if (getCheckBoxItem(drawMenu, "noti"))
            {
                var t = TargetSelector.GetTarget(1500, DamageType.Physical);

                if (t.IsValidTarget())
                {
                    var comboDmg = 0f;
                    if (R.IsReady())
                    {
                        comboDmg += R.GetDamage(t) * 3;
                    }
                    if (Q.IsReady())
                    {
                        comboDmg += Q.GetDamage(t) * 2;
                    }
                    if (W.IsReady())
                    {
                        comboDmg += W.GetDamage(t) + W.GetDamage(t, 1);
                    }
                    if (comboDmg > t.Health)
                    {
                        Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.5f, Color.Red,
                            "COMBO KILL " + t.ChampionName + " have: " + t.Health + "hp");
                        drawLine(t.Position, Player.Position, 10, Color.Yellow);
                    }
                }
            }
        }
    }
}