using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SebbyLib;
using Orbwalking = SebbyLib.Orbwalking;
using Spell = LeagueSharp.Common.Spell;

namespace PortAIO.Champion.Anivia
{
    internal class Program
    {
        private static readonly Menu Config = SebbyLib.Program.Config;
        private static Spell E, Q, R, W;
        private static float QMANA, WMANA, EMANA, RMANA;
        private static GameObject QMissile, RMissile;

        public static Menu drawMenu, QMenu, WMenu, EMenu, RMenu, FarmMenu, AniviaMenu;

        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public static void LoadOKTW()
        {
            Q = new Spell(SpellSlot.Q, 1000);
            W = new Spell(SpellSlot.W, 950);
            E = new Spell(SpellSlot.E, 650);
            R = new Spell(SpellSlot.R, 685);

            Q.SetSkillshot(0.25f, 110f, 870f, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.6f, 1f, float.MaxValue, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(2f, 400f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            Chat.Print("Berb : Using Common Pred w/ Medium Hitchance is the best for Anivia!");

            LoadMenuOKTW();

            Game.OnUpdate += Game_OnGameUpdate;
            GameObject.OnDelete += Obj_AI_Base_OnDelete;
            GameObject.OnCreate += Obj_AI_Base_OnCreate;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (getCheckBoxItem(WMenu, "inter") && W.IsReady() && sender.IsValidTarget(W.Range))
                W.Cast(sender);
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var Target = gapcloser.Sender;
            if (Q.IsReady() && getCheckBoxItem(QMenu, "AGCQ"))
            {
                if (Target.IsValidTarget(300))
                {
                    Q.Cast(Target);
                    SebbyLib.Program.debug("AGC Q");
                }
            }
            else if (W.IsReady() && getCheckBoxItem(WMenu, "AGCW"))
            {
                if (Target.IsValidTarget(W.Range))
                {
                    W.Cast(ObjectManager.Player.Position.Extend(Target.Position, 50), true);
                }
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

        private static void LoadMenuOKTW()
        {
            drawMenu = Config.AddSubMenu("线圈");
            drawMenu.Add("qRange", new CheckBox("Q 范围"));
            drawMenu.Add("wRange", new CheckBox("W 范围"));
            drawMenu.Add("eRange", new CheckBox("E 范围"));
            drawMenu.Add("rRange", new CheckBox("R 范围"));
            drawMenu.Add("onlyRdy", new CheckBox("只显示无冷却技能"));

            QMenu = Config.AddSubMenu("Q 设置");
            QMenu.Add("autoQ", new CheckBox("自动 Q"));
            QMenu.Add("AGCQ", new CheckBox("Q 防突进"));
            QMenu.Add("harrasQ", new CheckBox("骚扰 Q"));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != Player.Team))
            {
                QMenu.Add("haras" + enemy.ChampionName, new CheckBox("骚扰 :" + enemy.ChampionName));
            }

            WMenu = Config.AddSubMenu("W 设置");
            WMenu.Add("autoW", new CheckBox("自动 W"));
            WMenu.Add("AGCW", new CheckBox("防突进 W"));
            WMenu.Add("inter", new CheckBox("可打断技能的目标 W"));

            EMenu = Config.AddSubMenu("E 设置");
            EMenu.Add("autoE", new CheckBox("自动 E"));

            RMenu = Config.AddSubMenu("R 设置");
            RMenu.Add("autoR", new CheckBox("自动 R"));

            FarmMenu = Config.AddSubMenu("农兵");
            FarmMenu.Add("farmE", new CheckBox("清线 E"));
            FarmMenu.Add("farmR", new CheckBox("清线 R"));
            FarmMenu.Add("Mana", new Slider("清线蓝量", 80));
            FarmMenu.Add("LCminions", new Slider("清线最低小兵数量", 2, 0, 10));
            FarmMenu.Add("jungleQ", new CheckBox("清野 Q"));
            FarmMenu.Add("jungleW", new CheckBox("清野 W"));
            FarmMenu.Add("jungleE", new CheckBox("清野 E"));
            FarmMenu.Add("jungleR", new CheckBox("清野 R"));

            AniviaMenu = Config.AddSubMenu(Player.ChampionName);
            AniviaMenu.Add("AACombo", new CheckBox("E无冷却时，屏蔽普攻"));
        }

        private static void Obj_AI_Base_OnCreate(GameObject obj, EventArgs args)
        {
            if (obj.IsValid)
            {
                if (obj.Name == "cryo_FlashFrost_Player_mis.troy")
                {
                    QMissile = obj;
                }
                if (obj.Name.Contains("cryo_storm"))
                {
                    RMissile = obj;
                }
            }
        }

        private static void Obj_AI_Base_OnDelete(GameObject obj, EventArgs args)
        {
            if (obj.IsValid)
            {
                if (obj.Name == "cryo_FlashFrost_Player_mis.troy")
                    QMissile = null;
                if (obj.Name.Contains("cryo_storm"))
                    RMissile = null;
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (SebbyLib.Program.Combo && getCheckBoxItem(AniviaMenu, "AACombo"))
            {
                Orbwalker.DisableAttacking = E.IsReady();
            }
            else
                Orbwalker.DisableAttacking = false;

            if (Q.IsReady() && QMissile != null && QMissile.Position.CountEnemiesInRange(230) > 0)
                Q.Cast();


            if (SebbyLib.Program.LagFree(0))
            {
                SetMana();
            }

            if (SebbyLib.Program.LagFree(1) && R.IsReady() && getCheckBoxItem(RMenu, "autoR"))
                LogicR();

            if (SebbyLib.Program.LagFree(2) && W.IsReady() && getCheckBoxItem(WMenu, "autoW"))
                LogicW();

            if (SebbyLib.Program.LagFree(3) && Q.IsReady() && QMissile == null && getCheckBoxItem(QMenu, "autoQ"))
                LogicQ();

            if (SebbyLib.Program.LagFree(4))
            {
                if (E.IsReady() && getCheckBoxItem(EMenu, "autoE"))
                    LogicE();

                Jungle();
            }
        }

        private static void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (t.IsValidTarget())
            {
                if (SebbyLib.Program.Combo && Player.Mana > EMANA + QMANA - 10)
                    SebbyLib.Program.CastSpell(Q, t);
                else if (SebbyLib.Program.Farm && getCheckBoxItem(QMenu, "harrasQ") && getCheckBoxItem(QMenu, "haras" + t.ChampionName) && Player.Mana > RMANA + EMANA + QMANA + WMANA && OktwCommon.CanHarras())
                {
                    SebbyLib.Program.CastSpell(Q, t);
                }
                else
                {
                    var qDmg = OktwCommon.GetKsDamage(t, Q);
                    var eDmg = E.GetDamage(t);
                    if (qDmg > t.Health)
                        SebbyLib.Program.CastSpell(Q, t);
                    else if (qDmg + eDmg > t.Health && Player.Mana > QMANA + WMANA)
                        SebbyLib.Program.CastSpell(Q, t);
                }
                if (!SebbyLib.Program.None && Player.Mana > RMANA + EMANA)
                {
                    foreach (
                        var enemy in
                            SebbyLib.Program.Enemies.Where(
                                enemy => enemy.IsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                        Q.Cast(enemy, true);
                }
            }
        }

        private static void LogicW()
        {
            if (SebbyLib.Program.Combo && Player.Mana > RMANA + EMANA + WMANA)
            {
                var t = TargetSelector.GetTarget(W.Range, DamageType.Magical);
                if (t != null && W.IsReady())
                {
                    if (Player.Position.Distance(t.ServerPosition) > Player.Position.Distance(t.Position))
                    {
                        if (t.Position.Distance(Player.ServerPosition) < t.Position.Distance(Player.Position))
                            SebbyLib.Program.CastSpell(W, t);
                    }
                    else
                    {
                        if (t.Position.Distance(Player.ServerPosition) > t.Position.Distance(Player.Position) &&
                            t.Distance(Player) < R.Range)
                            SebbyLib.Program.CastSpell(W, t);
                    }
                }
            }
        }

        private static void LogicE()
        {
            var t = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            if (t.IsValidTarget())
            {
                var qCd = Q.Instance.CooldownExpires - Game.Time;
                var rCd = R.Instance.CooldownExpires - Game.Time;
                if (Player.Level < 7)
                    rCd = 10;
                //debug("Q " + qCd + "R " + rCd + "E now " + E.Instance.Cooldown);
                var eDmg = OktwCommon.GetKsDamage(t, E);

                if (eDmg > t.Health)
                    E.Cast(t, true);

                if (t.HasBuff("chilled") || qCd > E.Instance.Cooldown - 1 && rCd > E.Instance.Cooldown - 1)
                {
                    if (eDmg*3 > t.Health)
                        E.Cast(t, true);
                    else if (SebbyLib.Program.Combo && (t.HasBuff("chilled") || Player.Mana > RMANA + EMANA))
                    {
                        E.Cast(t, true);
                    }
                    else if (SebbyLib.Program.Farm && Player.Mana > RMANA + EMANA + QMANA + WMANA &&
                             !Player.UnderTurret(true) && QMissile == null)
                    {
                        E.Cast(t, true);
                    }
                }
                else if (SebbyLib.Program.Combo && R.IsReady() && Player.Mana > RMANA + EMANA && QMissile == null)
                {
                    R.Cast(t, true, true);
                }
            }
            farmE();
        }

        private static void farmE()
        {
            if (SebbyLib.Program.LaneClear && getCheckBoxItem(FarmMenu, "farmE") && Player.Mana > QMANA + EMANA + WMANA &&
                !Orbwalking.CanAttack() && Player.ManaPercent > getSliderItem(FarmMenu, "Mana"))
            {
                var minions = Cache.GetMinions(Player.ServerPosition, E.Range);
                foreach (var minion in minions.Where(minion => minion.Health > Player.GetAutoAttackDamage(minion)))
                {
                    var eDmg = E.GetDamage(minion)*2;
                    if (minion.Health < eDmg && minion.HasBuff("chilled"))
                        E.Cast(minion);
                }
            }
        }

        private static void LogicR()
        {
            if (RMissile == null)
            {
                var t = TargetSelector.GetTarget(R.Range + 400, DamageType.Magical);
                if (t.IsValidTarget())
                {
                    if (R.GetDamage(t) > t.Health)
                    {
                        R.Cast(t, false, true);
                    }
                    else if (Player.Mana > RMANA + EMANA && E.GetDamage(t)*2 + R.GetDamage(t) > t.Health)
                    {
                        R.Cast(t, false, true);
                    }
                    if (Player.Mana > RMANA + EMANA + QMANA + WMANA && SebbyLib.Program.Combo)
                    {
                        R.Cast(t, false, true);
                    }
                }
                if (SebbyLib.Program.LaneClear && Player.ManaPercent > getSliderItem(FarmMenu, "Mana") &&
                    getCheckBoxItem(FarmMenu, "farmR"))
                {
                    var allMinions = Cache.GetMinions(Player.ServerPosition, R.Range);
                    var farmPos = R.GetCircularFarmLocation(allMinions, R.Width);
                    if (farmPos.MinionsHit >= getSliderItem(FarmMenu, "LCminions"))
                        R.Cast(farmPos.Position);
                }
            }
            else
            {
                if (SebbyLib.Program.LaneClear && getCheckBoxItem(FarmMenu, "farmR"))
                {
                    var allMinions = Cache.GetMinions(RMissile.Position, R.Width);
                    var mobs = Cache.GetMinions(RMissile.Position, R.Width, MinionTeam.Neutral);
                    if (mobs.Count > 0)
                    {
                        if (!getCheckBoxItem(FarmMenu, "jungleR"))
                        {
                            R.Cast();
                        }
                    }
                    else if (allMinions.Count > 0)
                    {
                        if (allMinions.Count < 2 || Player.ManaPercent < getSliderItem(FarmMenu, "Mana"))
                            R.Cast();
                        else if (Player.ManaPercent < getSliderItem(FarmMenu, "Mana"))
                            R.Cast();
                    }
                    else
                        R.Cast();
                }
                else if ((RMissile.Position.CountEnemiesInRange(470) == 0 || Player.Mana < EMANA + QMANA))
                {
                    R.Cast();
                }
            }
        }

        private static void Jungle()
        {
            if (SebbyLib.Program.LaneClear)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, E.Range, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    if (Q.IsReady() && getCheckBoxItem(FarmMenu, "jungleQ"))
                    {
                        if (QMissile != null)
                        {
                            if (QMissile.Position.Distance(mob.ServerPosition) < 230)
                                Q.Cast();
                        }
                        else
                        {
                            Q.Cast(mob.ServerPosition);
                        }

                        return;
                    }
                    if (R.IsReady() && getCheckBoxItem(FarmMenu, "jungleR") && RMissile == null)
                    {
                        R.Cast(mob.ServerPosition);
                        return;
                    }
                    if (E.IsReady() && getCheckBoxItem(FarmMenu, "jungleE") && mob.HasBuff("chilled"))
                    {
                        E.Cast(mob);
                        return;
                    }
                    if (W.IsReady() && getCheckBoxItem(FarmMenu, "jungleW"))
                    {
                        W.Cast(mob.Position.Extend(Player.Position, 100));
                    }
                }
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

        private static void Drawing_OnDraw(EventArgs args)
        {
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
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, E.Range, Color.Yellow, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, E.Range, Color.Yellow, 1, 1);
            }
            if (getCheckBoxItem(drawMenu, "rRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (R.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, R.Range, Color.Gray, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, R.Range, Color.Gray, 1, 1);
            }
        }
    }
}