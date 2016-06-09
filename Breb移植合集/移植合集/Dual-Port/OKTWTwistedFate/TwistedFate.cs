using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;
using EloBuddy.SDK;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    class TwistedFate
    {
        private static Menu Config = Program.Config;
        private static LeagueSharp.Common.Spell Q, W, E, R;
        private static float QMANA = 0, WMANA = 0, EMANA = 0, RMANA = 0;
        private static string temp = null;
        private static bool cardok = true;
        private static int FindCard = 0;
        public static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public static Menu drawMenu, qMenu, wMenu, rMenu, farmMenu;

        public static void LoadOKTW()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 1400);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 700);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 1200);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 5500);

            Q.SetSkillshot(0.25f, 40f, 1000, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(1f, 40f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetTargetted(0.25f, 2000f);

            drawMenu = Config.AddSubMenu("Drawings");
            drawMenu.Add("onlyRdy", new CheckBox("Draw only ready spells", true));
            drawMenu.Add("qRange", new CheckBox("Q range", false));
            drawMenu.Add("rRangeMini", new CheckBox("R range minimap", true));
            drawMenu.Add("cardInfo", new CheckBox("Show card info", true));
            drawMenu.Add("notR", new CheckBox("R info helper", true));

            qMenu = Config.AddSubMenu("Q Config");
            qMenu.Add("autoQ", new CheckBox("Auto Q", true));
            qMenu.Add("harrasQ", new CheckBox("Harass Q", true));

            wMenu = Config.AddSubMenu("W Config");
            wMenu.Add("Wmode", new ComboBox("W mode", 0, "Auto", "Manual"));
            wMenu.Add("Wgold", new KeyBind("Gold key", false, KeyBind.BindTypes.HoldActive, 'Y'));
            wMenu.Add("Wblue", new KeyBind("Blue key", false, KeyBind.BindTypes.HoldActive, 'U'));
            wMenu.Add("Wred", new KeyBind("RED key", false, KeyBind.BindTypes.HoldActive, 'I'));
            wMenu.Add("WblockAA", new CheckBox("Block AA if seeking GOLD card", true));
            wMenu.Add("harasW", new CheckBox("Harass GOLD low range", true));
            wMenu.Add("ignoreW", new CheckBox("Ignore first card", true));

            rMenu = Config.AddSubMenu("R Config");
            rMenu.Add("useR", new KeyBind("Semi-manual cast R key", false, KeyBind.BindTypes.HoldActive, 'T'));
            rMenu.Add("autoR", new CheckBox("Auto R", true));
            rMenu.Add("Renemy", new Slider("Don't R if enemy in x range", 1000, 0, 2000));
            rMenu.Add("RenemyA", new Slider("Don't R if ally in x range near target", 800, 0, 2000));
            rMenu.Add("turetR", new CheckBox("Don't R under turret ", true));

            farmMenu = Config.AddSubMenu("Farm");
            farmMenu.Add("WredFarm", new Slider("LaneClear red card above % mana", 80, 0, 100));
            farmMenu.Add("farmQ", new CheckBox("Lane clear Q", true));
            farmMenu.Add("farmW", new CheckBox("Lane clear W Blue / Red card", false));
            farmMenu.Add("jungleQ", new CheckBox("Jungle clear Q", true));
            farmMenu.Add("Mana", new Slider("LaneClear Mana", 80, 0, 100));
            farmMenu.Add("LCminions", new Slider("LaneClear minimum minions", 2, 0, 10));

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnEndScene += Drawing_OnEndScene;
            Drawing.OnDraw += Drawing_OnDraw;
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
            if (Program.Combo && W.IsReady() && FindCard == 1 && W.Instance.Name != "PickACard" && getCheckBoxItem(wMenu, "WblockAA"))
            {
                args.Process = false;
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {

            if (!getCheckBoxItem(wMenu, "ignoreW"))
                cardok = true;

            if (W.IsReady())
            {
                if (getBoxItem(wMenu, "Wmode") == 0)
                    LogicW();
                else
                    LogicWmaunal();
            }
            else if (W.Instance.Name == "PickACard")
            {
                temp = null;
                cardok = false;
            }

            if (Program.LagFree(2) && Q.IsReady() && getCheckBoxItem(qMenu, "autoQ"))
                LogicQ();

            if (Program.LagFree(4) && Q.IsReady())
                Jungle();

            if (R.IsReady())
            {
                if (Program.LagFree(3) && W.IsReady() && getCheckBoxItem(rMenu, "autoR"))
                    LogicR();

                if (getKeyBindItem(rMenu, "useR"))
                {
                    if (Player.HasBuff("destiny_marker"))
                    {
                        var t = TargetSelector.GetTarget(R.Range, DamageType.Magical);
                        if (t.IsValidTarget())
                        {
                            R.Cast(t);
                        }
                    }
                    else
                    {
                        R.Cast();
                    }
                }
            }
            //Program.debug("" + (W.Instance.CooldownExpires - Game.Time));
        }

        private static void LogicWmaunal()
        {
            var wName = W.Instance.Name;
            if (wName == "PickACard" && Utils.TickCount - W.LastCastAttemptT > 150)
            {
                if (R.IsReady() && (Player.HasBuff("destiny_marker") || Player.HasBuff("gate")))
                {
                    FindCard = 1;
                    W.Cast();
                }
                else if (getKeyBindItem(wMenu, "Wgold"))
                {
                    FindCard = 1;
                    W.Cast();
                }
                else if (getKeyBindItem(wMenu, "Wblue"))
                {
                    FindCard = 2;
                    W.Cast();
                }
                else if (getKeyBindItem(wMenu, "Wred"))
                {
                    FindCard = 3;
                    W.Cast();
                }
            }
            else if (Player.HasBuff("pickacard_tracker"))
            {
                if (temp == null)
                    temp = wName;
                else if (temp != wName)
                    cardok = true;

                if (cardok)
                {
                    if (R.IsReady() && (Player.HasBuff("destiny_marker") || Player.HasBuff("gate")))
                    {
                        FindCard = 1;
                        if (wName == "GoldCardLock")
                            W.Cast();
                    }
                    else if (FindCard == 1)
                    {
                        if (wName == "GoldCardLock")
                            W.Cast();
                    }
                    else if (FindCard == 2)
                    {
                        if (wName == "BlueCardLock")
                            W.Cast();
                    }
                    else if (FindCard == 3)
                    {
                        if (wName == "RedCardLock")
                            W.Cast();
                    }
                }
            }
        }

        private static void LogicW()
        {
            var wName = W.Instance.Name;
            var t = TargetSelector.GetTarget(1100, DamageType.Magical);
            if (wName == "PickACard" && Utils.TickCount - W.LastCastAttemptT > 150)
            {
                if (R.IsReady() && (Player.HasBuff("destiny_marker") || Player.HasBuff("gate")))
                    W.Cast();
                else if (t.LSIsValidTarget() && Program.Combo)
                    W.Cast();
                else if (Orbwalker.LastTarget != null)
                {
                    if (Program.Farm && Orbwalker.LastTarget.Type == GameObjectType.AIHeroClient && getCheckBoxItem(wMenu, "harasW"))
                        W.Cast();
                    else if (Program.LaneClear && (Orbwalker.LastTarget.Type == GameObjectType.obj_AI_Minion || Orbwalker.LastTarget.Type == GameObjectType.obj_AI_Turret) && getCheckBoxItem(farmMenu, "farmW"))
                        W.Cast();
                }
            }
            else if (Player.HasBuff("pickacard_tracker"))
            {
                if (temp == null)
                    temp = wName;
                else if (temp != wName)
                    cardok = true;

                if (cardok)
                {
                    AIHeroClient orbTarget = null;

                    var getTarget = Orbwalker.LastTarget;
                    if (getTarget != null && getTarget.Type == GameObjectType.AIHeroClient)
                    {
                        orbTarget = (AIHeroClient)getTarget;
                    }

                    if (R.IsReady() && (Player.HasBuff("destiny_marker") || Player.HasBuff("gate")))
                    {
                        FindCard = 1;
                        if (wName == "GoldCardLock")
                            W.Cast();
                    }
                    else if (Program.Combo && orbTarget.LSIsValidTarget() && W.GetDamage(orbTarget) + Player.GetAutoAttackDamage(orbTarget) > orbTarget.Health)
                    {
                        W.Cast();
                        Program.debug("1" + wName);
                    }
                    else if (Player.Mana < RMANA + QMANA + WMANA)
                    {
                        FindCard = 2;
                        if (wName == "BlueCardLock")
                            W.Cast();
                    }
                    else if (Program.Farm && orbTarget.LSIsValidTarget())
                    {
                        FindCard = 1;
                        if (wName == "GoldCardLock")
                            W.Cast();
                    }
                    else if (Player.ManaPercent > getSliderItem(farmMenu, "WredFarm") && Program.LaneClear && getCheckBoxItem(farmMenu, "farmW"))
                    {
                        FindCard = 3;
                        if (wName == "RedCardLock")
                            W.Cast();
                    }
                    else if ((Program.LaneClear || Player.Mana < RMANA + QMANA) && getCheckBoxItem(farmMenu, "farmW"))
                    {
                        FindCard = 2;
                        if (wName == "BlueCardLock")
                            W.Cast();
                    }
                    else if (Program.Combo)
                    {
                        FindCard = 1;
                        if (wName == "GoldCardLock")
                            W.Cast();
                    }
                }
            }
        }

        private static void Jungle()
        {
            if (Program.LaneClear)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, 700, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];

                    if (Q.IsReady() && getCheckBoxItem(qMenu, "jungleQ"))
                    {
                        Q.Cast(mob);
                        return;
                    }
                }
            }
        }

        private static void LogicR()
        {
            if (Player.CountEnemiesInRange(getSliderItem(rMenu, "Renemy")) == 0)
            {
                var t = TargetSelector.GetTarget(R.Range, DamageType.Magical);
                if (t.IsValidTarget() && t.Distance(Player.Position) > Q.Range && t.CountAlliesInRange(getSliderItem(rMenu, "RenemyA")) == 0)
                {
                    if (Q.GetDamage(t) + W.GetDamage(t) + Player.GetAutoAttackDamage(t) * 3 > t.Health && t.CountEnemiesInRange(1000) < 3)
                    {
                        var rPos = R.GetPrediction(t).CastPosition;
                        if (getCheckBoxItem(rMenu, "turetR"))
                        {
                            if (!rPos.UnderTurret(true))
                                R.Cast(rPos);
                        }
                        else
                        {
                            R.Cast(rPos);
                        }
                    }
                }
            }
        }

        private static void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (t.IsValidTarget())
            {
                if (OktwCommon.GetKsDamage(t, Q) > t.Health && !SebbyLib.Orbwalking.InAutoAttackRange(t))
                    Program.CastSpell(Q, t);

                if (W.Instance.CooldownExpires - Game.Time < W.Instance.Cooldown - 1.3 && W.Instance.Name == "PickACard" && (W.Instance.CooldownExpires - Game.Time > 3 || Player.CountEnemiesInRange(950) == 0))
                {
                    if (Program.Combo && Player.Mana > RMANA + QMANA)
                        Program.CastSpell(Q, t);
                    if (Program.Farm && Player.Mana > RMANA + QMANA + WMANA + EMANA && getCheckBoxItem(qMenu, "harrasQ") && OktwCommon.CanHarras())
                        Program.CastSpell(Q, t);
                }

                foreach (var enemy in Program.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                    Q.Cast(enemy, true, true);

            }
            else if (Program.LaneClear && Player.ManaPercent > getSliderItem(farmMenu, "Mana") && getCheckBoxItem(farmMenu, "farmQ") && Player.Mana > RMANA + QMANA)
            {
                var minionList = Cache.GetMinions(Player.ServerPosition, Q.Range);
                var farmPosition = Q.GetLineFarmLocation(minionList, Q.Width);
                if (farmPosition.MinionsHit > getSliderItem(farmMenu, "LCminions"))
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

        private static void Drawing_OnEndScene(EventArgs args)
        {
            if (R.IsReady() && getCheckBoxItem(drawMenu, "rRangeMini"))
            {
                LeagueSharp.Common.Utility.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Aqua, 1, 20, true);
            }
        }

        public static void drawText(string msg, Vector3 Hero, System.Drawing.Color color, int weight = 0)
        {
            var wts = Drawing.WorldToScreen(Hero);
            Drawing.DrawText(wts[0] - (msg.Length) * 5, wts[1] + weight, color, msg);
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

            if (getCheckBoxItem(drawMenu, "cardInfo") && W.Instance.Name != "PickACard")
            {
                if (FindCard == 1)
                    drawText("SEEK YELLOW", Player.Position, System.Drawing.Color.Yellow, -70);
                if (FindCard == 2)
                    drawText("SEEK BLUE ", Player.Position, System.Drawing.Color.Aqua, -70);
                if (FindCard == 3)
                    drawText("SEEK RED ", Player.Position, System.Drawing.Color.OrangeRed, -70);

            }


            if (R.IsReady() && getCheckBoxItem(drawMenu, "notR"))
            {
                var t = TargetSelector.GetTarget(R.Range, DamageType.Magical);
                if (t.IsValidTarget())
                {
                    var comboDMG = Q.GetDamage(t) + W.GetDamage(t) + Player.GetAutoAttackDamage(t) * 3;
                    if (Player.HasBuff("destiny_marker"))
                        Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.5f, System.Drawing.Color.Yellow, "AUTO R TARGET: " + t.ChampionName + " Heal " + t.Health + " My damage: " + comboDMG);
                    else if (comboDMG > t.Health)
                        Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.5f, System.Drawing.Color.Red, "You can kill: " + t.ChampionName + " Heal " + t.Health + " My damage: " + comboDMG + " PRESS semi-manual cast");
                }
            }
        }
    }
}