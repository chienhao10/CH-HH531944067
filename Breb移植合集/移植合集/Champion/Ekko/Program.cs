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
using Prediction = LeagueSharp.Common.Prediction;
using Spell = LeagueSharp.Common.Spell;

namespace PortAIO.Champion.Ekko
{
    internal class Program
    {
        private static readonly Menu Config = SebbyLib.Program.Config;
        private static Menu drawMenu, wMenu, rMenu, harassMenu, farmMenu;
        private static Spell E, Q, Q1, R, W;
        private static float QMANA, WMANA, EMANA, RMANA, Wtime, Wtime2;
        private static GameObject RMissile, WMissile2, WMissile;
        public static MissileReturn missileManager;

        private static AIHeroClient Player
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

        public static void LoadOKTW()
        {
            Q = new Spell(SpellSlot.Q, 750);
            Q1 = new Spell(SpellSlot.Q, 1000);
            W = new Spell(SpellSlot.W, 1620);
            E = new Spell(SpellSlot.E, 330f);
            R = new Spell(SpellSlot.R, 280f);

            Q.SetSkillshot(0.25f, 60f, 1650f, false, SkillshotType.SkillshotLine);
            Q1.SetSkillshot(0.5f, 150f, 1000f, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(2.5f, 200f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.4f, 280f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            missileManager = new MissileReturn("ekkoqmis", "ekkoqreturn", Q);

            drawMenu = Config.AddSubMenu("Draw");
            drawMenu.Add("qRange", new CheckBox("Q range"));
            drawMenu.Add("wRange", new CheckBox("W range"));
            drawMenu.Add("eRange", new CheckBox("E range"));
            drawMenu.Add("rRange", new CheckBox("R range"));
            drawMenu.Add("Qhelp", new CheckBox("Show Q,W helper"));
            drawMenu.Add("onlyRdy", new CheckBox("Draw only ready spells"));

            wMenu = Config.AddSubMenu("W Option");
            wMenu.Add("autoW", new CheckBox("Auto W"));
            wMenu.Add("Waoe", new CheckBox("Cast if 2 targets"));

            rMenu = Config.AddSubMenu("R Option");
            rMenu.Add("autoR", new CheckBox("Auto R"));
            rMenu.Add("rCount", new Slider("Auto R if enemies in range", 3, 0, 5));

            harassMenu = Config.AddSubMenu("Harass");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != Player.Team))
            {
                harassMenu.Add("haras" + enemy.ChampionName, new CheckBox(enemy.ChampionName));
            }

            farmMenu = Config.AddSubMenu("Farm");
            farmMenu.Add("farmQ", new CheckBox("Lane clear Q"));
            farmMenu.Add("farmW", new CheckBox("Farm W"));
            farmMenu.Add("jungleQ", new CheckBox("Jungle clear Q"));
            farmMenu.Add("jungleW", new CheckBox("Jungle clear W"));
            farmMenu.Add("LCminions", new Slider("LaneClear minimum minions", 2, 0, 10));
            farmMenu.Add("Mana", new Slider("LaneClear Mana", 80, 30));

            Game.OnUpdate += Game_OnGameUpdate;
            GameObject.OnCreate += Obj_AI_Base_OnCreate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (SebbyLib.Program.LagFree(0))
            {
                SetMana();
                Jungle();
            }

            if (SebbyLib.Program.LagFree(1) && Q.IsReady())
                LogicQ();
            if (SebbyLib.Program.LagFree(2) && W.IsReady() && getCheckBoxItem(wMenu, "autoW") &&
                Player.Mana > RMANA + WMANA + EMANA + QMANA)
                LogicW();
            if (SebbyLib.Program.LagFree(3) && E.IsReady())
                LogicE();
            if (R.IsReady())
                LogicR();
        }

        private static void LogicR()
        {
            if (getCheckBoxItem(rMenu, "autoR"))
            {
                if (SebbyLib.Program.LagFree(4) && SebbyLib.Program.Combo && RMissile != null && RMissile.IsValid)
                {
                    if (RMissile.Position.CountEnemiesInRange(R.Range) >= getSliderItem(rMenu, "rCount") &&
                        getSliderItem(rMenu, "rCount") > 0)
                        R.Cast();

                    foreach (
                        var t in
                            SebbyLib.Program.Enemies.Where(
                                t =>
                                    t.IsValidTarget() &&
                                    RMissile.Position.Distance(Prediction.GetPrediction(t, R.Delay).CastPosition) <
                                    R.Range && RMissile.Position.Distance(t.ServerPosition) < R.Range))
                    {
                        var comboDMG = OktwCommon.GetKsDamage(t, R);

                        if (Q.IsReady())
                            comboDMG += Q.GetDamage(t);

                        if (E.IsReady())
                            comboDMG += E.GetDamage(t);

                        if (W.IsReady())
                            comboDMG += W.GetDamage(t);

                        if (t.Health < comboDMG && OktwCommon.ValidUlt(t))
                            R.Cast();

                        SebbyLib.Program.debug("ks");
                    }
                }

                var dmg = OktwCommon.GetIncomingDamage(Player, 1);
                var enemys = Player.CountEnemiesInRange(800);

                if (dmg > 0 || enemys > 0)
                {
                    if (dmg > Player.Level*50)
                    {
                        R.Cast();
                    }
                    else if (Player.Health - dmg < enemys*Player.Level*20)
                    {
                        R.Cast();
                    }
                    else if (Player.Health - dmg < Player.Level*10)
                    {
                        R.Cast();
                    }
                }
            }
        }

        private static void LogicE()
        {
            if (SebbyLib.Program.Combo && WMissile != null && WMissile.IsValid)
            {
                if (WMissile.Position.CountEnemiesInRange(200) > 0 &&
                    WMissile.Position.Distance(Player.ServerPosition) < 100)
                {
                    E.Cast(Player.Position.Extend(WMissile.Position, E.Range), true);
                }
            }

            var t = TargetSelector.GetTarget(800, DamageType.Magical);

            if (E.IsReady() && Player.Mana > RMANA + EMANA
                && Player.CountEnemiesInRange(260) > 0
                && Player.Position.Extend(Game.CursorPos, E.Range).CountEnemiesInRange(500) < 3
                && t.Position.Distance(Game.CursorPos) > t.Position.Distance(Player.Position))
            {
                E.Cast(Player.Position.Extend(Game.CursorPos, E.Range), true);
            }
            else if (SebbyLib.Program.Combo && Player.Health > Player.MaxHealth*0.4
                     && Player.Mana > RMANA + EMANA
                     && !Player.UnderTurret(true)
                     && Player.Position.Extend(Game.CursorPos, E.Range).CountEnemiesInRange(700) < 3)
            {
                if (t.IsValidTarget() && Player.Mana > QMANA + EMANA + WMANA &&
                    t.Position.Distance(Game.CursorPos) + 300 < t.Position.Distance(Player.Position))
                {
                    E.Cast(Player.Position.Extend(Game.CursorPos, E.Range), true);
                }
            }
            else if (t.IsValidTarget() && SebbyLib.Program.Combo && E.GetDamage(t) + W.GetDamage(t) > t.Health)
            {
                E.Cast(Player.Position.Extend(t.Position, E.Range), true);
            }
        }

        private static void Jungle()
        {
            if (SebbyLib.Program.LaneClear && Player.Mana > QMANA + RMANA)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, 500, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    if (W.IsReady() && getCheckBoxItem(farmMenu, "jungleW"))
                    {
                        W.Cast(mob.Position);
                        return;
                    }
                    if (Q.IsReady() && getCheckBoxItem(farmMenu, "jungleQ"))
                    {
                        Q.Cast(mob.Position);
                    }
                }
            }
        }

        private static void Obj_AI_Base_OnCreate(GameObject obj, EventArgs args)
        {
            if (obj.IsValid)
            {
                if (obj.Name == "Ekko" && obj.IsAlly)
                    RMissile = obj;
                if (obj.Name == "Ekko_Base_W_Indicator.troy")
                {
                    WMissile = obj;
                    Wtime = Game.Time;
                }
                if (obj.Name == "Ekko_Base_W_Cas.troy")
                {
                    WMissile2 = obj;
                    Wtime2 = Game.Time;
                }
            }
        }

        private static void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            var t1 = TargetSelector.GetTarget(Q1.Range, DamageType.Magical);
            if (t.IsValidTarget())
            {
                missileManager.Target = t;
                if (SebbyLib.Program.Combo && Player.Mana > RMANA + QMANA)
                    SebbyLib.Program.CastSpell(Q, t);
                else if (SebbyLib.Program.Farm && getCheckBoxItem(harassMenu, "haras" + t.ChampionName) &&
                         Player.Mana > RMANA + WMANA + QMANA + QMANA && OktwCommon.CanHarras())
                    SebbyLib.Program.CastSpell(Q, t);
                else if (OktwCommon.GetKsDamage(t, Q)*2 > t.Health)
                    SebbyLib.Program.CastSpell(Q, t);
                if (Player.Mana > RMANA + QMANA + WMANA)
                {
                    foreach (
                        var enemy in
                            SebbyLib.Program.Enemies.Where(
                                enemy => enemy.IsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                        Q.Cast(enemy, true, true);
                }
            }
            else if (t1.IsValidTarget())
            {
                missileManager.Target = t1;
                if (SebbyLib.Program.Combo && Player.Mana > RMANA + QMANA)
                    SebbyLib.Program.CastSpell(Q1, t1);
                else if (SebbyLib.Program.Farm && getCheckBoxItem(harassMenu, "haras" + t1.ChampionName) &&
                         Player.Mana > RMANA + WMANA + QMANA + QMANA && OktwCommon.CanHarras())
                    SebbyLib.Program.CastSpell(Q1, t1);
                else if (OktwCommon.GetKsDamage(t1, Q1)*2 > t1.Health)
                    SebbyLib.Program.CastSpell(Q1, t1);
                if (Player.Mana > RMANA + QMANA + WMANA)
                {
                    foreach (
                        var enemy in
                            SebbyLib.Program.Enemies.Where(
                                enemy => enemy.IsValidTarget(Q1.Range) && !OktwCommon.CanMove(enemy)))
                        Q1.Cast(enemy, true, true);
                }
            }
            else if (SebbyLib.Program.LaneClear && Player.ManaPercent > getSliderItem(farmMenu, "Mana") &&
                     getCheckBoxItem(farmMenu, "farmQ") && Player.Mana > RMANA + QMANA + WMANA)
            {
                var allMinionsQ = Cache.GetMinions(Player.ServerPosition, Q1.Range);
                var Qfarm = Q.GetLineFarmLocation(allMinionsQ, 100);
                if (Qfarm.MinionsHit >= getSliderItem(farmMenu, "LCminions"))
                    Q.Cast(Qfarm.Position);
            }
        }

        private static void LogicW()
        {
            var t = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            if (t.IsValidTarget())
            {
                if (getCheckBoxItem(wMenu, "Waoe"))
                {
                    W.CastIfWillHit(t, 2, true);
                    if (t.CountEnemiesInRange(250) > 1)
                    {
                        SebbyLib.Program.CastSpell(W, t);
                    }
                }

                if (SebbyLib.Program.Combo && W.GetPrediction(t).CastPosition.Distance(t.Position) > 200)
                    SebbyLib.Program.CastSpell(W, t);
            }
            if (!SebbyLib.Program.None)
            {
                foreach (
                    var enemy in
                        SebbyLib.Program.Enemies.Where(
                            enemy => enemy.IsValidTarget(W.Range) && !OktwCommon.CanMove(enemy)))
                    W.Cast(enemy, true, true);
            }
        }

        private static void SetMana()
        {
            if ((SebbyLib.Program.getCheckBoxItem("manaDisable") && SebbyLib.Program.Combo) || Player.HealthPercent < 20)
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

        public static void drawText2(string msg, Vector3 Hero, Color color)
        {
            var wts = Drawing.WorldToScreen(Hero);
            Drawing.DrawText(wts[0] - msg.Length*5, wts[1] - 200, color, msg);
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(drawMenu, "Qhelp"))
            {
                if (WMissile != null && WMissile.IsValid)
                {
                    LeagueSharp.Common.Utility.DrawCircle(WMissile.Position, 300, Color.Yellow, 1, 1);
                    drawText2("W:  " + string.Format("{0:0.0}", Wtime + 3 - Game.Time), WMissile.Position, Color.White);
                }
                if (WMissile2 != null && WMissile2.IsValid)
                {
                    LeagueSharp.Common.Utility.DrawCircle(WMissile2.Position, 300, Color.Red, 1, 1);
                    drawText2("W:  " + string.Format("{0:0.0}", Wtime2 + 1 - Game.Time), WMissile2.Position, Color.Red);
                }
            }

            if (getCheckBoxItem(drawMenu, "qRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (Q.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Cyan, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Cyan, 1, 1);
            }
            if (getCheckBoxItem(drawMenu, "wRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (W.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, W.Range, Color.Orange, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, W.Range, Color.Orange, 1, 1);
            }
            if (getCheckBoxItem(drawMenu, "eRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (E.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, 800, Color.Yellow, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, 800, Color.Yellow, 1, 1);
            }
            if (getCheckBoxItem(drawMenu, "rRange"))
            {
                if (RMissile != null && RMissile.IsValid)
                {
                    if (getCheckBoxItem(drawMenu, "rRange"))
                    {
                        if (getCheckBoxItem(drawMenu, "onlyRdy"))
                        {
                            if (R.IsReady())
                                LeagueSharp.Common.Utility.DrawCircle(RMissile.Position, R.Width, Color.YellowGreen, 1,
                                    1);
                        }
                        else
                            LeagueSharp.Common.Utility.DrawCircle(RMissile.Position, R.Width, Color.YellowGreen, 1, 1);

                        drawLine(RMissile.Position, Player.Position, 10, Color.YellowGreen);
                    }
                }
            }
        }
    }
}