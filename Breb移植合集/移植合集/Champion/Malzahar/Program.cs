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
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    internal class Malzahar
    {
        private static readonly Menu Config = Program.Config;
        private static Spell Q, Qr, W, E, R;
        private static float QMANA, WMANA, EMANA, RMANA;
        private static float Rtime;

        public static Menu drawMenu, qMenu, wMenu, eMenu, rMenu, farmMenu;

        public static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public static void LoadOKTW()
        {
            Q = new Spell(SpellSlot.Q, 900);
            Qr = new Spell(SpellSlot.Q, 900);
            W = new Spell(SpellSlot.W, 650);
            E = new Spell(SpellSlot.E, 650);
            R = new Spell(SpellSlot.R, 700);

            Qr.SetSkillshot(0.25f, 100, float.MaxValue, false, SkillshotType.SkillshotCircle);
            Q.SetSkillshot(0.75f, 80, float.MaxValue, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(1.2f, 230, float.MaxValue, false, SkillshotType.SkillshotCircle);

            drawMenu = Config.AddSubMenu("线圈");
            drawMenu.Add("noti", new CheckBox("显示提示"));
            drawMenu.Add("onlyRdy", new CheckBox("只显示无冷却技能"));
            drawMenu.Add("qRange", new CheckBox("Q 范围", false));
            drawMenu.Add("wRange", new CheckBox("W 范围", false));
            drawMenu.Add("eRange", new CheckBox("E 范围", false));
            drawMenu.Add("rRange", new CheckBox("R 范围", false));

            qMenu = Config.AddSubMenu("Q 设置");
            qMenu.Add("autoQ", new CheckBox("自动 Q"));
            qMenu.Add("harrasQ", new CheckBox("骚扰 Q"));
            qMenu.Add("intQ", new CheckBox("Q 技能打断"));
            qMenu.Add("gapQ", new CheckBox("防突进 Q"));

            wMenu = Config.AddSubMenu("W 设置");
            wMenu.Add("autoW", new CheckBox("自动 W"));
            wMenu.Add("harrasW", new CheckBox("骚扰 W"));

            eMenu = Config.AddSubMenu("E 设置");
            eMenu.Add("autoE", new CheckBox("自动 E"));
            eMenu.Add("harrasE", new CheckBox("骚扰 E"));
            eMenu.Add("harrasEminion", new CheckBox("尝试E 小兵进行骚扰"));

            rMenu = Config.AddSubMenu("R 设置");
            rMenu.Add("autoR", new CheckBox("自动 R"));
            rMenu.Add("useR", new KeyBind("快速连招(R)按键", false, KeyBind.BindTypes.HoldActive, 'T')); //32 == space
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                rMenu.Add("gapcloser" + enemy.ChampionName, new CheckBox("防突进 : " + enemy.ChampionName, false));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                rMenu.Add("Ron" + enemy.ChampionName, new CheckBox("快速连招 : " + enemy.ChampionName));
            rMenu.Add("Rturrent", new CheckBox("塔下不 R"));

            farmMenu = Config.AddSubMenu("农兵");
            farmMenu.Add("farmQ", new CheckBox("清线 Q"));
            farmMenu.Add("farmW", new CheckBox("清线 W"));
            farmMenu.Add("farmE", new CheckBox("清线 E"));
            farmMenu.Add("Mana", new Slider("清线蓝量", 80));
            farmMenu.Add("LCminions", new Slider("清线最低小兵数", 2, 0, 10));
            farmMenu.Add("jungleE", new CheckBox("清野 E"));
            farmMenu.Add("jungleQ", new CheckBox("清野 Q"));
            farmMenu.Add("jungleW", new CheckBox("清野 W"));

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
        }

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if ((Player.IsChannelingImportantSpell() || Game.Time - Rtime < 0.5) && Game.Time - Rtime < 2.5)
            {
                args.Process = false;
                return;
            }

            if (args.Slot == SpellSlot.R)
            {
                var t = TargetSelector.GetTarget(R.Range - 20, DamageType.Magical);

                if (E.IsReady() && t.IsValidTarget(E.Range) && Player.Mana > RMANA + EMANA)
                {
                    E.CastOnUnit(t);
                    args.Process = false;
                    return;
                }

                if (W.IsReady() && t.IsValidTarget(W.Range) && Player.Mana > RMANA + WMANA)
                {
                    W.Cast(t);
                    args.Process = false;
                    return;
                }

                if (Q.IsReady() && t.IsValidTarget(Q.Range) && Player.Mana > RMANA + QMANA)
                {
                    Qr.Cast(t);
                    args.Process = false;
                    return;
                }

                if (R.IsReady() && t.IsValidTarget())
                    Rtime = Game.Time;
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

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var t = gapcloser.Sender;

            if (Q.IsReady() && getCheckBoxItem(qMenu, "gapQ") && t.IsValidTarget(Q.Range))
            {
                Q.Cast(gapcloser.End);
            }
            else if (R.IsReady() && getCheckBoxItem(rMenu, "gapcloser" + gapcloser.Sender.ChampionName) &&
                     t.IsValidTarget(R.Range))
            {
                R.CastOnUnit(t);
            }
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient t,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!getCheckBoxItem(qMenu, "intQ") || !Q.IsReady())
                return;

            if (t.IsValidTarget(Q.Range))
            {
                Q.Cast(t);
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if ((Player.IsChannelingImportantSpell() || Game.Time - Rtime < 0.5) && Game.Time - Rtime < 2.5)
            {
                Orbwalker.DisableAttacking = true;
                Orbwalker.DisableMovement = true;
                Program.debug("cast R");
                return;
            }
            Orbwalker.DisableAttacking = false;
            Orbwalker.DisableMovement = false;

            if (R.IsReady() && getKeyBindItem(rMenu, "useR"))
            {
                var t = TargetSelector.GetTarget(R.Range, DamageType.Magical);
                if (t.IsValidTarget(R.Range) && getCheckBoxItem(rMenu, "Ron" + t.ChampionName))
                {
                    R.CastOnUnit(t);
                    return;
                }
            }

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
            var t = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (t.IsValidTarget())
            {
                var qDmg = OktwCommon.GetKsDamage(t, Q) + BonusDmg(t);

                if (qDmg > t.Health)
                    Program.CastSpell(Q, t);

                if (R.IsReady() && t.IsValidTarget(R.Range))
                {
                    return;
                }
                if (Program.Combo && Player.Mana > RMANA + QMANA)
                    Program.CastSpell(Q, t);
                else if (Program.Farm && getCheckBoxItem(qMenu, "harrasQ") && Player.Mana > RMANA + EMANA + WMANA + EMANA)
                    Program.CastSpell(Q, t);

                if (Player.Mana > RMANA + QMANA)
                {
                    foreach (
                        var enemy in
                            Program.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                        Q.Cast(enemy);
                }
            }
            else if (Program.LaneClear && Player.ManaPercent > getSliderItem(farmMenu, "Mana") &&
                     getCheckBoxItem(farmMenu, "farmQ"))
            {
                var allMinions = Cache.GetMinions(Player.ServerPosition, Q.Range);
                var farmPos = Q.GetCircularFarmLocation(allMinions, 150);
                if (farmPos.MinionsHit > getSliderItem(farmMenu, "LCminions"))
                    Q.Cast(farmPos.Position);
            }
        }

        private static void LogicW()
        {
            var t = TargetSelector.GetTarget(W.Range, DamageType.Magical);
            if (t.IsValidTarget())
            {
                var qDmg = Q.GetDamage(t);
                var wDmg = OktwCommon.GetKsDamage(t, W) + BonusDmg(t);
                if (wDmg > t.Health)
                {
                    W.Cast(Player.Position.LSExtend(t.Position, 450));
                }
                else if (wDmg + qDmg > t.Health && Player.Mana > QMANA + EMANA)
                    W.Cast(Player.Position.LSExtend(t.Position, 450));
                else if (Program.Combo && Player.Mana > RMANA + WMANA)
                    W.Cast(Player.Position.LSExtend(t.Position, 450));
                else if (Program.Farm && getCheckBoxItem(wMenu, "harrasW") && !Player.UnderTurret(true) &&
                         (Player.Mana > Player.MaxMana*0.8 || W.Level > Q.Level) &&
                         Player.Mana > RMANA + WMANA + EMANA + QMANA + WMANA && OktwCommon.CanHarras())
                    W.Cast(Player.Position.LSExtend(t.Position, 450));
            }
            else if (Program.LaneClear && Player.ManaPercent > getSliderItem(farmMenu, "Mana") && getCheckBoxItem(farmMenu, "farmW"))
            {
                var allMinions = Cache.GetMinions(Player.ServerPosition, W.Range);
                var farmPos = W.GetCircularFarmLocation(allMinions, W.Width);
                if (farmPos.MinionsHit >= getSliderItem(farmMenu, "LCminions"))
                    W.Cast(farmPos.Position);
            }
        }

        private static void LogicE()
        {
            var t = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            if (t.IsValidTarget())
            {
                var eDmg = OktwCommon.GetKsDamage(t, E) + BonusDmg(t);
                var wDmg = W.GetDamage(t);

                if (eDmg > t.Health)
                    E.CastOnUnit(t);
                else if (W.IsReady() && wDmg + eDmg > t.Health && Player.Mana > WMANA + EMANA)
                    E.CastOnUnit(t);
                else if (R.IsReady() && W.IsReady() && wDmg + eDmg + R.GetDamage(t) > t.Health &&
                         Player.Mana > WMANA + EMANA + RMANA)
                    E.CastOnUnit(t);
                if (Program.Combo && Player.Mana > RMANA + EMANA)
                    E.CastOnUnit(t);
                else if (Program.Farm && getCheckBoxItem(eMenu, "harrasE") && Player.Mana > RMANA + EMANA + WMANA + EMANA)
                    E.CastOnUnit(t);
            }
            else if (Program.LaneClear && Player.ManaPercent > getSliderItem(farmMenu, "Mana") &&
                     getCheckBoxItem(farmMenu, "farmE"))
            {
                var allMinions = Cache.GetMinions(Player.ServerPosition, E.Range);
                if (allMinions.Count >= getSliderItem(farmMenu, "LCminions"))
                {
                    foreach (
                        var minion in
                            allMinions.Where(
                                minion =>
                                    minion.IsValidTarget(E.Range) && minion.Health < E.GetDamage(minion) &&
                                    !minion.HasBuff("AlZaharMaleficVisions")))
                    {
                        E.CastOnUnit(minion);
                    }
                }
            }
            else if (Program.Farm && Player.Mana > RMANA + EMANA + WMANA + EMANA &&
                     getCheckBoxItem(eMenu, "harrasEminion"))
            {
                var te = TargetSelector.GetTarget(E.Range + 400, DamageType.Magical);
                if (te.IsValidTarget())
                {
                    var allMinions = Cache.GetMinions(Player.ServerPosition, E.Range);
                    foreach (
                        var minion in
                            allMinions.Where(
                                minion =>
                                    minion.IsValidTarget(E.Range) && minion.Health < E.GetDamage(minion) &&
                                    te.Distance(minion.Position) < 500 &&
                                    !minion.HasBuff("AlZaharMaleficVisions")))
                    {
                        E.CastOnUnit(minion);
                    }
                }
            }
        }

        private static void LogicR()
        {
            if (Player.UnderTurret(true) && getCheckBoxItem(rMenu, "Rturrent"))
                return;
            var t = TargetSelector.GetTarget(R.Range, DamageType.Magical);
            if (Player.CountEnemiesInRange(900) < 3 && t.IsValidTarget())
            {
                var totalComboDamage = OktwCommon.GetKsDamage(t, R);
                // E calculation

                totalComboDamage += E.GetDamage(t);

                if (W.IsReady() && Player.Mana > RMANA + WMANA)
                {
                    totalComboDamage += W.GetDamage(t)*5;
                }

                if (Player.Mana > RMANA + QMANA)
                    totalComboDamage += Q.GetDamage(t);

                if (totalComboDamage > t.Health - OktwCommon.GetIncomingDamage(t) && OktwCommon.ValidUlt(t))
                {
                    R.CastOnUnit(t);
                }
            }
        }

        private static void Jungle()
        {
            if (Program.LaneClear && Player.Mana > RMANA + EMANA)
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
                    }
                }
            }
        }

        private static float BonusDmg(AIHeroClient target)
        {
            return (float) Player.CalcDamage(target, DamageType.Magical, target.MaxHealth*0.08 - target.HPRegenRate*5);
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
                RMANA = WMANA - Player.PARRegenRate*W.Instance.Cooldown;
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

            if (getCheckBoxItem(drawMenu, "noti") && R.IsReady())
            {
            }
        }
    }
}