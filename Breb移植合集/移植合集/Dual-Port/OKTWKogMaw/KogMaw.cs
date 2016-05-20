using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SebbyLib;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace OneKeyToWin_AIO_Sebby
{
    class KogMaw
    {
        private static Menu Config = Program.Config;
        public static LeagueSharp.Common.Spell Q, W, E, R;
        public static float QMANA = 0, WMANA = 0, EMANA = 0, RMANA = 0;

        public static bool attackNow = true;

        public static AIHeroClient Player { get { return ObjectManager.Player; } }

        public static Menu qMenu, eMenu, wMenu, rMenu, drawMenu, miscMenu;

        public static void LoadOKTW()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 980);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 1000);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 1200);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 1800);

            Q.SetSkillshot(0.25f, 50f, 2000f, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 120f, 1400f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(1.2f, 120f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            qMenu = Config.AddSubMenu("Q Config");
            qMenu.Add("autoQ", new CheckBox("Auto Q", true));
            qMenu.Add("harrasQ", new CheckBox("Harass Q", true));

            eMenu = Config.AddSubMenu("E Config");
            eMenu.Add("autoE", new CheckBox("Auto E", true));
            eMenu.Add("HarrasE", new CheckBox("Harass E", true));
            eMenu.Add("AGC", new CheckBox("AntiGapcloserE", true));

            wMenu = Config.AddSubMenu("W Config");
            wMenu.Add("autoW", new CheckBox("Auto W", true));
            wMenu.Add("harasW", new CheckBox("Harass W on max range", true));

            rMenu = Config.AddSubMenu("R Config");
            rMenu.Add("autoR", new CheckBox("Auto R", true));
            rMenu.Add("RmaxHp", new Slider("Target max % HP", 50, 0, 100));
            rMenu.Add("comboStack", new Slider("Max combo stack R", 2, 0, 10));
            rMenu.Add("harasStack", new Slider("Max haras stack R", 1, 0, 10));
            rMenu.Add("Rcc", new CheckBox("R cc", true));
            rMenu.Add("Rslow", new CheckBox("R slow", true));
            rMenu.Add("Raoe", new CheckBox("R aoe", true));
            rMenu.Add("Raa", new CheckBox("R only out off AA range", false));

            drawMenu = Config.AddSubMenu("Drawings");
            drawMenu.Add("ComboInfo", new CheckBox("R killable info", true));
            drawMenu.Add("qRange", new CheckBox("Q range", false));
            drawMenu.Add("wRange", new CheckBox("W range", false));
            drawMenu.Add("eRange", new CheckBox("E range", false));
            drawMenu.Add("rRange", new CheckBox("R range", false));
            drawMenu.Add("onlyRdy", new CheckBox("Draw only ready spells", true));

            miscMenu = Config.AddSubMenu("Misc Config");
            miscMenu.Add("sheen", new CheckBox("Sheen logic", true));
            miscMenu.Add("AApriority", new CheckBox("AA priority over spell", true));

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalker.OnPreAttack += BeforeAttack;
            Orbwalker.OnPostAttack += afterAttack;
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
            if (getCheckBoxItem(wMenu, "AGC") && E.IsReady() && Player.Mana > RMANA + EMANA)
            {
                var Target = (AIHeroClient)gapcloser.Sender;
                if (Target.IsValidTarget(E.Range))
                {
                    E.Cast(Target, true);
                    Program.debug("E AGC");
                }
            }
            return;
        }

        private static void afterAttack(AttackableUnit target, EventArgs args)
        {
            attackNow = true;
        }

        private static void BeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            attackNow = false;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Program.LagFree(0))
            {
                R.Range = 800 + 300 * Player.Spellbook.GetSpell(SpellSlot.R).Level;
                W.Range = 650 + 30 * Player.Spellbook.GetSpell(SpellSlot.W).Level;
                SetMana();

            }
            if (Program.LagFree(1) && E.IsReady() && !Player.Spellbook.IsAutoAttacking && getCheckBoxItem(eMenu, "autoE"))
                LogicE();

            if (Program.LagFree(2) && Q.IsReady() && !Player.Spellbook.IsAutoAttacking && getCheckBoxItem(qMenu, "autoQ"))
                LogicQ();

            if (Program.LagFree(3) && W.IsReady() && getCheckBoxItem(wMenu, "autoW"))
                LogicW();

            if (Program.LagFree(4) && R.IsReady() && !Player.Spellbook.IsAutoAttacking)
                LogicR();

        }

        private static void LogicR()
        {
            if (getCheckBoxItem(rMenu, "autoR") && Sheen())
            {
                var target = TargetSelector.GetTarget(R.Range, DamageType.Magical);

                if (target.IsValidTarget(R.Range) && target.HealthPercent < getSliderItem(rMenu, "RmaxHp") && OktwCommon.ValidUlt(target))
                {


                    if (getCheckBoxItem(rMenu, "Raa") && SebbyLib.Orbwalking.InAutoAttackRange(target))
                        return;

                    var harasStack = getSliderItem(rMenu, "harasStack");
                    var comboStack = getSliderItem(rMenu, "comboStack");
                    var countR = GetRStacks();

                    var Rdmg = R.GetDamage(target);
                    Rdmg = Rdmg + target.CountAlliesInRange(500) * Rdmg;

                    if (R.GetDamage(target) > target.Health - OktwCommon.GetIncomingDamage(target))
                        Program.CastSpell(R, target);
                    else if (Program.Combo && Rdmg * 2 > target.Health && Player.Mana > RMANA * 3)
                        Program.CastSpell(R, target);
                    else if (countR < comboStack + 2 && Player.Mana > RMANA * 3)
                    {
                        foreach (var enemy in Program.Enemies.Where(enemy => enemy.IsValidTarget(R.Range) && !OktwCommon.CanMove(enemy)))
                        {
                            R.Cast(enemy, true);
                        }
                    }

                    if (target.HasBuffOfType(BuffType.Slow) && getCheckBoxItem(rMenu, "Rslow") && countR < comboStack + 1 && Player.Mana > RMANA + WMANA + EMANA + QMANA)
                        Program.CastSpell(R, target);
                    else if (Program.Combo && countR < comboStack && Player.Mana > RMANA + WMANA + EMANA + QMANA)
                        Program.CastSpell(R, target);
                    else if (Program.Farm && countR < harasStack && Player.Mana > RMANA + WMANA + EMANA + QMANA)
                        Program.CastSpell(R, target);
                }
            }
        }

        private static void LogicW()
        {
            if (Player.CountEnemiesInRange(W.Range) > 0 && Sheen())
            {
                if (Program.Combo)
                    W.Cast();
                else if (Program.Farm && getCheckBoxItem(wMenu, "harasW") && Player.CountEnemiesInRange(Player.AttackRange) > 0)
                    W.Cast();
            }
        }

        private static void LogicQ()
        {
            if (Sheen())
            {
                var t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                if (t.IsValidTarget())
                {
                    var qDmg = OktwCommon.GetKsDamage(t, Q);
                    var eDmg = E.GetDamage(t);
                    if (t.IsValidTarget(W.Range) && qDmg + eDmg > t.Health)
                        Program.CastSpell(Q, t);
                    else if (Program.Combo && Player.Mana > RMANA + QMANA * 2 + EMANA)
                        Program.CastSpell(Q, t);
                    else if ((Program.Farm && Player.Mana > RMANA + EMANA + QMANA * 2 + WMANA) && getCheckBoxItem(qMenu, "harrasQ") && !Player.UnderTurret(true))
                        Program.CastSpell(Q, t);
                    else if ((Program.Combo || Program.Farm) && Player.Mana > RMANA + QMANA + EMANA)
                    {
                        foreach (var enemy in Program.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                            Q.Cast(enemy, true);

                    }
                }
            }
        }

        private static void LogicE()
        {
            if (Sheen())
            {
                var t = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                if (t.IsValidTarget())
                {
                    var qDmg = Q.GetDamage(t);
                    var eDmg = OktwCommon.GetKsDamage(t, E);
                    if (eDmg > t.Health)
                        Program.CastSpell(E, t);
                    else if (eDmg + qDmg > t.Health && Q.IsReady())
                        Program.CastSpell(E, t);
                    else if (Program.Combo && ObjectManager.Player.Mana > RMANA + WMANA + EMANA + QMANA)
                        Program.CastSpell(E, t);
                    else if (Program.Farm && getCheckBoxItem(eMenu, "HarrasE") && Player.Mana > RMANA + WMANA + EMANA + QMANA + EMANA)
                        Program.CastSpell(E, t);
                    else if ((Program.Combo || Program.Farm) && ObjectManager.Player.Mana > RMANA + WMANA + EMANA)
                    {
                        foreach (var enemy in Program.Enemies.Where(enemy => enemy.IsValidTarget(E.Range) && !OktwCommon.CanMove(enemy)))
                            E.Cast(enemy, true);
                    }
                }
            }
        }

        private static bool Sheen()
        {
            var target = Orbwalker.LastTarget;
            if (!(target is AIHeroClient))
                attackNow = true;
            if (target.IsValidTarget() && Player.HasBuff("sheen") && getCheckBoxItem(miscMenu, "sheen") && target is AIHeroClient)
            {
                Program.debug("shen true");
                return false;
            }
            else if (target.IsValidTarget() && getCheckBoxItem(miscMenu, "AApriority") && target is AIHeroClient && !attackNow)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private static int GetRStacks()
        {
            foreach (var buff in ObjectManager.Player.Buffs)
            {
                if (buff.Name == "kogmawlivingartillerycost")
                    return buff.Count;
            }
            return 0;
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

        private static void drawText(string msg, AIHeroClient Hero, System.Drawing.Color color)
        {
            var wts = Drawing.WorldToScreen(Hero.Position);
            Drawing.DrawText(wts[0] - (msg.Length) * 5, wts[1], color, msg);
        }

        private static void Drawing_OnDraw(EventArgs args)
        {

            if (getCheckBoxItem(drawMenu, "ComboInfo"))
            {
                var combo = "haras";
                foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsValidTarget()))
                {
                    if (R.GetDamage(enemy) > enemy.Health)
                    {
                        combo = "KILL R";
                        drawText(combo, enemy, System.Drawing.Color.GreenYellow);
                    }
                    else
                    {
                        combo = (int)(enemy.Health / R.GetDamage(enemy)) + " R";
                        drawText(combo, enemy, System.Drawing.Color.Red);
                    }
                }
            }
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
            if (getCheckBoxItem(drawMenu, "rRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (R.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.Gray, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.Gray, 1, 1);
            }
        }
    }
}