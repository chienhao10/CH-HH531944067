using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace OneKeyToWin_AIO_Sebby
{
    class Quinn
    {
        private static Menu Config = Program.Config;
        private static LeagueSharp.Common.Spell Q, W, E, R;
        private static float QMANA = 0, WMANA = 0, EMANA = 0, RMANA = 0;

        public static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public static Menu drawMenu, qMenu, eMenu, miscMenu, harassMenu, farmMenu;

        public static bool getAutoW()
        {
            return getCheckBoxItem(miscMenu, "autoW");
        }

        public static void LoadOKTW()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 1000);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 700);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 2100);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 550);

            Q.SetSkillshot(0.25f, 90f, 1550, true, SkillshotType.SkillshotLine);
            E.SetTargetted(0.25f, 2000f);

            drawMenu = Config.AddSubMenu("Drawings");
            drawMenu.Add("onlyRdy", new CheckBox("Draw only ready spells", true));
            drawMenu.Add("qRange", new CheckBox("Q range", false));
            drawMenu.Add("wRange", new CheckBox("W range", false));
            drawMenu.Add("eRange", new CheckBox("E range", false));

            qMenu = Config.AddSubMenu("Q Config");
            qMenu.Add("autoQ", new CheckBox("Auto Q", true));
            qMenu.Add("harrasQ", new CheckBox("Harass Q", true));

            eMenu = Config.AddSubMenu("E Config");
            eMenu.Add("autoE", new CheckBox("Auto E", true));
            eMenu.Add("harrasE", new CheckBox("Harass E", true));
            eMenu.Add("AGC", new CheckBox("AntiGapcloser E", true));
            eMenu.Add("Int", new CheckBox("Interrupter E", true));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != Player.Team))
                eMenu.Add("gap" + enemy.ChampionName, new CheckBox("GapClose : " + enemy.ChampionName, true));

            miscMenu = Config.AddSubMenu("Misc");
            miscMenu.Add("autoW", new CheckBox("Auto W", true));
            miscMenu.Add("autoR", new CheckBox("Auto R in shop", true));
            miscMenu.Add("focusP", new CheckBox("Focus marked enemy", true));

            harassMenu = Config.AddSubMenu("Harass");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                harassMenu.Add("harras" + enemy.ChampionName, new CheckBox(enemy.ChampionName));

            farmMenu = Config.AddSubMenu("Farm");
            farmMenu.Add("farmP", new CheckBox("Attack marked minion first", true));
            farmMenu.Add("farmQ", new CheckBox("Farm Q", true));
            farmMenu.Add("jungleE", new CheckBox("Jungle clear E", true));
            farmMenu.Add("jungleQ", new CheckBox("Jungle clear Q", true));
            farmMenu.Add("Mana", new Slider("LaneClear Mana", 80, 0, 100));
            farmMenu.Add("LCminions", new Slider("LaneClear minimum minions", 2, 0, 10));

            Game.OnUpdate += Game_OnGameUpdate;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalker.OnPostAttack += afterAttack;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Orbwalker.OnPreAttack += Orbwalking_BeforeAttack;
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

        private static void Orbwalking_BeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (args.Target.Type == GameObjectType.AIHeroClient && getCheckBoxItem(miscMenu, "focusP") && args.Target.HealthPercent > 40)
            {
                var orbTarget = args.Target as AIHeroClient;
                if (!orbTarget.HasBuff("quinnw"))
                {
                    var best = Program.Enemies.FirstOrDefault(enemy => enemy.IsValidTarget() && SebbyLib.Orbwalking.InAutoAttackRange(enemy) && enemy.HasBuff("quinnw"));
                    if (best != null)
                        Orbwalker.ForcedTarget = best;
                    else
                        Orbwalker.ForcedTarget = null;
                }
            }
            else if (Program.LaneClear && args.Target.Type == GameObjectType.obj_AI_Minion && getCheckBoxItem(farmMenu, "farmP"))
            {
                var bestMinion = Cache.GetMinions(Player.Position, Player.AttackRange).FirstOrDefault(minion => minion.IsValidTarget() && SebbyLib.Orbwalking.InAutoAttackRange(minion) && minion.HasBuff("quinnw"));

                if (bestMinion != null)
                    Orbwalker.ForcedTarget = bestMinion;
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Program.LagFree(1))
                SetMana();
            if (Program.LagFree(2) && Q.IsReady() && getCheckBoxItem(qMenu, "autoQ"))
                LogicQ();

            if (Program.LagFree(4) && R.IsReady() && getCheckBoxItem(miscMenu, "autoR"))
                LogicR();
        }
        private static void Jungle()
        {
            if (Program.LaneClear && Player.Mana > RMANA + WMANA + RMANA + WMANA)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, 700, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    if (mob.HasBuff("QuinnW"))
                        return;

                    if (Q.IsReady() && getCheckBoxItem(farmMenu, "jungleQ"))
                    {
                        Q.Cast(mob.ServerPosition);
                        return;
                    }

                    if (E.IsReady() && getCheckBoxItem(farmMenu, "jungleE"))
                    {
                        E.CastOnUnit(mob);
                        return;
                    }
                }
            }
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (E.IsReady() && getCheckBoxItem(eMenu, "Int") && sender.IsValidTarget(E.Range))
                E.CastOnUnit(sender);
        }

        private static void afterAttack(AttackableUnit target, EventArgs args)
        {
            if (target.Type == GameObjectType.AIHeroClient)
            {
                var t = target as AIHeroClient;
                if (E.IsReady() && getCheckBoxItem(eMenu, "autoE") && t.IsValidTarget(E.Range) && t.CountEnemiesInRange(800) < 3)
                {
                    if (Program.Combo && Player.Mana > RMANA + EMANA)
                        E.Cast(t);
                    else if (Program.Farm && Player.Mana > RMANA + EMANA + QMANA + WMANA && getCheckBoxItem(eMenu, "harrasE") && getCheckBoxItem(harassMenu, "harras" + t.ChampionName) && OktwCommon.CanHarras())
                    {
                        E.Cast(t);
                    }
                    else if (OktwCommon.GetKsDamage(t, E) > t.Health)
                        E.Cast(t);
                }
                if (Q.IsReady() && t.IsValidTarget(Q.Range))
                {
                    if (Program.Combo && Player.Mana > RMANA + QMANA)
                        Program.CastSpell(Q, t);
                    else if (Program.Farm && Player.Mana > RMANA + EMANA + QMANA + WMANA && getCheckBoxItem(qMenu, "harrasQ") && getCheckBoxItem(harassMenu, "harras" + t.ChampionName) && OktwCommon.CanHarras())
                    {
                        Program.CastSpell(Q, t);
                    }
                    else if (OktwCommon.GetKsDamage(t, Q) > t.Health)
                        Program.CastSpell(Q, t);

                    if (!Program.None && Player.Mana > RMANA + QMANA + EMANA)
                    {
                        foreach (var enemy in Program.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                            Q.Cast(enemy);
                    }
                }
            }
            Jungle();
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (E.IsReady() && getCheckBoxItem(eMenu, "AGC") && getCheckBoxItem(eMenu, "gap" + gapcloser.Sender.ChampionName))
            {
                var t = gapcloser.Sender;
                if (t.IsValidTarget(E.Range))
                {
                    E.Cast(t);
                }
            }
        }

        private static void LogicR()
        {
            if (Player.InFountain() && R.Instance.Name == "QuinnR")
            {
                R.Cast();
            }
        }

        private static void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (t.IsValidTarget())
            {
                if (SebbyLib.Orbwalking.InAutoAttackRange(t) && t.HasBuff("quinnw"))
                    return;
                if (Program.Combo && Player.Mana > RMANA + QMANA)
                    Program.CastSpell(Q, t);
                else if (Program.Farm && Player.Mana > RMANA + EMANA + QMANA + WMANA && getCheckBoxItem(qMenu, "harrasQ") && getCheckBoxItem(harassMenu, "harras" + t.ChampionName) && OktwCommon.CanHarras())
                {
                    Program.CastSpell(Q, t);
                }
                else if (OktwCommon.GetKsDamage(t, Q) > t.Health)
                    Program.CastSpell(Q, t);

                if (!Program.None && Player.Mana > RMANA + QMANA + EMANA)
                {
                    foreach (var enemy in Program.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                        Q.Cast(enemy);
                }
            }
            else if (Program.LaneClear && Player.ManaPercent > getSliderItem(farmMenu, "Mana") && getCheckBoxItem(farmMenu, "farmQ") && Player.Mana > RMANA + QMANA)
            {
                var minionList = Cache.GetMinions(Player.ServerPosition, Q.Range - 150);
                var farmPosition = Q.GetCircularFarmLocation(minionList, 150);
                if (farmPosition.MinionsHit >= getSliderItem(farmMenu, "LCminions"))
                    Q.Cast(farmPosition.Position);
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
                RMANA = WMANA - Player.PARRegenRate * W.Instance.Cooldown;
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
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
            }
            if (getCheckBoxItem(drawMenu, "wRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (W.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Orange, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Orange, 1, 1);
            }
            if (getCheckBoxItem(drawMenu, "eRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (E.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Yellow, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Yellow, 1, 1);
            }
        }
    }
}