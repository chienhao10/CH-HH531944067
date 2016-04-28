using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using OneKeyToWin_AIO_Sebby.Core;
using SebbyLib;
using Orbwalking = SebbyLib.Orbwalking;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

namespace OneKeyToWin_AIO_Sebby
{
    internal class Graves
    {
        private static readonly Menu Config = Program.Config;
        private static Spell E, Q, R, W, R1;
        private static float QMANA, WMANA, EMANA, RMANA;

        public static bool Esmart = false;
        public static float OverKill;
        public static OKTWdash Dash;

        public static Menu drawMenu, qMenu, wMenu, eMenu, rMenu, farmMenu, miscMenu;

        public static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public static void LoadOKTW()
        {
            Q = new Spell(SpellSlot.Q, 900);
            W = new Spell(SpellSlot.W, 950f);
            E = new Spell(SpellSlot.E, 450f);
            R = new Spell(SpellSlot.R, 1000f);
            R1 = new Spell(SpellSlot.R, 1700f);

            Q.SetSkillshot(0.25f, 100f, 2100f, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.25f, 120f, 1500f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.25f, 100f, 2100f, false, SkillshotType.SkillshotLine);
            R1.SetSkillshot(0.25f, 100f, 2100f, false, SkillshotType.SkillshotLine);

            LoadMenuOKTW();

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
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

        private static void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            Jungle();
            if (E.IsReady() && getCheckBoxItem(eMenu, "autoE"))
                LogicE();
        }

        private static void LoadMenuOKTW()
        {
            drawMenu = Config.AddSubMenu("Draw", "Draw");
            drawMenu.Add("qRange", new CheckBox("Q range", false));
            drawMenu.Add("wRange", new CheckBox("W range", false));
            drawMenu.Add("eRange", new CheckBox("E range", false));
            drawMenu.Add("rRange", new CheckBox("R range", false));
            drawMenu.Add("onlyRdy", new CheckBox("Draw only ready spells"));

            qMenu = Config.AddSubMenu("Q Config", "Q Config");
            qMenu.Add("autoQ", new CheckBox("Auto Q"));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != Player.Team))
                qMenu.Add("haras" + enemy.ChampionName, new CheckBox(enemy.ChampionName));

            wMenu = Config.AddSubMenu("W Config", "W Config");
            wMenu.Add("autoW", new CheckBox("Auto W"));
            wMenu.Add("AGCW", new CheckBox("AntiGapcloser W"));

            eMenu = Config.AddSubMenu("E Config", "E Config");
            eMenu.Add("autoE", new CheckBox("Auto E"));

            Dash = new OKTWdash(E);

            rMenu = Config.AddSubMenu("R Config", "R Config");
            rMenu.Add("autoR", new CheckBox("Auto R"));
            rMenu.Add("fastR", new CheckBox("Fast R ks Combo"));
            rMenu.Add("overkillR", new CheckBox("Overkill protection", false));
            rMenu.Add("useR", new KeyBind("Semi-manual cast R key", false, KeyBind.BindTypes.HoldActive, 'T'));
                //32 == space

            farmMenu = Config.AddSubMenu("Farm", "farm");
            farmMenu.Add("farmQ", new CheckBox("Lane clear Q"));
            farmMenu.Add("Mana", new Slider("LaneClear Mana", 80));
            farmMenu.Add("jungleQ", new CheckBox("Jungle clear Q"));
            farmMenu.Add("jungleW", new CheckBox("Jungle clear W"));

            miscMenu = Config.AddSubMenu("Misc", "Misc");
            miscMenu.Add("QWlogic", new CheckBox("Use Q and W only if don't have ammo"));
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Player.Mana > RMANA + EMANA)
            {
                var t = gapcloser.Sender;
                if (t.IsValidTarget(E.Range))
                {
                    if (W.IsReady() && getCheckBoxItem(wMenu, "AGCW"))
                    {
                        W.Cast(gapcloser.End);
                        Program.debug("W AGC");
                    }
                }
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (getKeyBindItem(rMenu, "useR") && R.IsReady())
            {
                var t = TargetSelector.GetTarget(1800, DamageType.Physical);
                if (t.IsValidTarget())
                    R1.Cast(t, true);
            }

            if (Program.LagFree(0))
            {
                SetMana();
            }

            if (!getCheckBoxItem(miscMenu, "QWlogic") || !Player.HasBuff("gravesbasicattackammo1"))
            {
                if (Program.LagFree(2) && Q.IsReady() && !Orbwalker.IsAutoAttacking && getCheckBoxItem(qMenu, "autoQ"))
                    //BERB
                    LogicQ();
                if (Program.LagFree(3) && W.IsReady() && !Orbwalker.IsAutoAttacking && getCheckBoxItem(wMenu, "autoW"))
                    LogicW();
            }
            if (Program.LagFree(4) && R.IsReady() && getCheckBoxItem(rMenu, "autoR"))
                LogicR();
        }

        private static void Jungle()
        {
            if (Program.LaneClear)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, 600, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    if (Q.IsReady() && getCheckBoxItem(farmMenu, "jungleQ"))
                    {
                        Q.Cast(mob.Position);
                        return;
                    }
                    if (W.IsReady() && getCheckBoxItem(farmMenu, "jungleW"))
                    {
                        W.Cast(mob.Position);
                    }
                }
            }
        }

        private static void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (t.IsValidTarget())
            {
                var step = t.Distance(Player)/20;
                for (var i = 0; i < 20; i++)
                {
                    var p = Player.Position.Extend(t.Position, step*i);
                    if (p.IsWall())
                    {
                        return;
                    }
                }

                if (Program.Combo && Player.Mana > RMANA + QMANA)
                    Program.CastSpell(Q, t);
                else if (Program.Farm && getCheckBoxItem(qMenu, "haras" + t.ChampionName) &&
                         Player.Mana > RMANA + EMANA + WMANA + QMANA + QMANA)
                    Program.CastSpell(Q, t);
                else
                {
                    var qDmg = OktwCommon.GetKsDamage(t, Q);
                    var rDmg = R.GetDamage(t);
                    if (qDmg > t.Health)
                    {
                        Q.Cast(t, true);
                        OverKill = Game.Time;
                        Program.debug("Q ks");
                    }
                    else if (qDmg + rDmg > t.Health && R.IsReady() && Player.Mana > RMANA + QMANA)
                    {
                        Program.CastSpell(Q, t);
                        if (getCheckBoxItem(rMenu, "fastR") && rDmg < t.Health)
                            Program.CastSpell(R, t);
                        Program.debug("Q + R ks");
                    }
                }

                if (!Program.None && Player.Mana > RMANA + QMANA + EMANA)
                {
                    foreach (
                        var enemy in
                            Program.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                        Q.Cast(enemy);
                }
            }
            else if (Program.LaneClear && Player.ManaPercent > getSliderItem(farmMenu, "Mana") &&
                     getCheckBoxItem(farmMenu, "farmQ") && Player.Mana > RMANA + QMANA)
            {
                var allMinionsQ = Cache.GetMinions(Player.ServerPosition, Q.Range);
                var Qfarm = Q.GetLineFarmLocation(allMinionsQ, Q.Width);
                if (Qfarm.MinionsHit > 2)
                    Q.Cast(Qfarm.Position);
            }
        }

        private static void LogicW()
        {
            var t = TargetSelector.GetTarget(W.Range, DamageType.Magical);
            if (t.IsValidTarget())
            {
                var wDmg = OktwCommon.GetKsDamage(t, W);
                if (wDmg > t.Health)
                {
                    W.Cast(t, true, true);
                }
                else if (wDmg + Q.GetDamage(t) > t.Health && Player.Mana > QMANA + WMANA + RMANA)
                {
                    W.Cast(t, true, true);
                }
                else if (Program.Combo && Player.Mana > RMANA + WMANA + QMANA)
                {
                    if (!Orbwalking.InAutoAttackRange(t) || Player.CountEnemiesInRange(300) > 0 ||
                        t.CountEnemiesInRange(250) > 1 || Player.HealthPercent < 50)
                        W.Cast(t, true, true);
                    else if (Player.Mana > RMANA + WMANA + QMANA + EMANA)
                    {
                        foreach (
                            var enemy in
                                Program.Enemies.Where(
                                    enemy => enemy.IsValidTarget(W.Range) && !OktwCommon.CanMove(enemy)))
                            W.Cast(enemy, true, true);
                    }
                }
            }
        }

        private static void LogicE()
        {
            if (Program.Enemies.Any(target => target.IsValidTarget(270) && target.IsMelee))
            {
                var dashPos = Dash.CastDash(true);
                if (!dashPos.IsZero)
                {
                    E.Cast(dashPos);
                }
            }
            if (Program.Combo && Player.Mana > RMANA + EMANA && !Player.HasBuff("gravesbasicattackammo2"))
            {
                var dashPos = Dash.CastDash();
                if (!dashPos.IsZero)
                {
                    E.Cast(dashPos);
                }
            }
        }

        private static void LogicR()
        {
            foreach (
                var target in
                    Program.Enemies.Where(target => target.IsValidTarget(R1.Range) && OktwCommon.ValidUlt(target)))
            {
                double rDmg = OktwCommon.GetKsDamage(target, R);

                if (rDmg < target.Health)
                    continue;

                if (getCheckBoxItem(rMenu, "overkillR") && target.Health < Player.Health)
                {
                    if (Orbwalking.InAutoAttackRange(target))
                        continue;
                    if (target.CountAlliesInRange(400) > 0)
                        continue;
                }

                var rDmg2 = rDmg*0.8;

                if (target.IsValidTarget(R.Range) && !OktwCommon.IsSpellHeroCollision(target, R) && rDmg > target.Health)
                {
                    Program.CastSpell(R, target);
                    Program.debug("Rdmg");
                }
                else if (rDmg2 > target.Health)
                {
                    if (!OktwCommon.IsSpellHeroCollision(target, R1))
                    {
                        Program.CastSpell(R1, target);
                        Program.debug("Rdmg2");
                    }
                    else if (target.IsValidTarget(1200))
                    {
                        Program.CastSpell(R1, target);
                        Program.debug("Rdmg2 collision");
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
                RMANA = QMANA - Player.PARRegenRate*Q.Instance.Cooldown;
            else
                RMANA = R.Instance.SData.Mana;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(drawMenu, "qRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (Q.IsReady())
                        Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Cyan, 1, 1);
                }
                else
                    Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Cyan, 1, 1);
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
                        Utility.DrawCircle(ObjectManager.Player.Position, E.Range, Color.Yellow, 1, 1);
                }
                else
                    Utility.DrawCircle(ObjectManager.Player.Position, E.Range, Color.Yellow, 1, 1);
            }
            if (getCheckBoxItem(drawMenu, "rRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (R.IsReady())
                        Utility.DrawCircle(ObjectManager.Player.Position, R.Range, Color.Gray, 1, 1);
                }
                else
                    Utility.DrawCircle(ObjectManager.Player.Position, R.Range, Color.Gray, 1, 1);
            }
        }
    }
}