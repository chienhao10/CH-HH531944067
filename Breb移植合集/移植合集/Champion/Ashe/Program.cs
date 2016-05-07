using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SebbyLib;
using HitChance = SebbyLib.Prediction.HitChance;
using Prediction = SebbyLib.Prediction.Prediction;
using PredictionInput = SebbyLib.Prediction.PredictionInput;
using Spell = LeagueSharp.Common.Spell;

namespace PortAIO.Champion.Ashe
{
    internal class Program
    {
        private static readonly Menu Config = SebbyLib.Program.Config;
        public static Spell Q, W, E, R;
        public static float QMANA, WMANA, EMANA, RMANA;
        private static Menu drawMenu, QMenu, EMenu, RMenu, FarmMenu, harassMenu;

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

        public static bool getAutoE()
        {
            return EMenu["autoE"].Cast<CheckBox>().CurrentValue;
        }

        private static void LoadMenuOKTW()
        {
            drawMenu = Config.AddSubMenu("线圈");
            drawMenu.Add("onlyRdy", new CheckBox("只显示无冷却技能"));
            drawMenu.Add("wRange", new CheckBox("W 范围"));

            QMenu = Config.AddSubMenu("Q 设置");
            QMenu.Add("harasQ", new CheckBox("骚扰 Q"));

            EMenu = Config.AddSubMenu("E 设置");
            EMenu.Add("autoE", new CheckBox("自动 E"));

            RMenu = Config.AddSubMenu("R 设置");
            RMenu.Add("autoR", new CheckBox("自动 R"));
            RMenu.Add("Rkscombo", new CheckBox("R 连招抢头 R + W + AA"));
            RMenu.Add("autoRaoe", new CheckBox("自动R （多敌人模式）"));
            RMenu.Add("autoRinter", new CheckBox("自动R （可尝试技能打断目标"));
            RMenu.Add("useR", new KeyBind("半自动 R 按键", false, KeyBind.BindTypes.HoldActive, 'T'));

            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != Player.Team))
            {
                RMenu.Add("GapCloser" + enemy.NetworkId, new CheckBox("防突进 R : " + enemy.ChampionName, false));
            }

            harassMenu = Config.AddSubMenu("骚扰");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != Player.Team))
            {
                harassMenu.Add("haras" + enemy.NetworkId, new CheckBox(enemy.ChampionName));
            }

            FarmMenu = Config.AddSubMenu("农兵");
            FarmMenu.Add("farmQ", new CheckBox("清线 Q"));
            FarmMenu.Add("farmW", new CheckBox("清线 W"));
            FarmMenu.Add("Mana", new Slider("清线蓝量", 80, 30));
            FarmMenu.Add("LCminions", new Slider("最低小兵命中数", 3, 0, 10));
            FarmMenu.Add("jungleQ", new CheckBox("清野 Q"));
            FarmMenu.Add("jungleW", new CheckBox("清野 W"));
        }

        public static void LoadOKTW()
        {
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 1260);
            E = new Spell(SpellSlot.E, 2500);
            R = new Spell(SpellSlot.R, 3000f);

            W.SetSkillshot(0.25f, 20f, 1500f, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 299f, 1400f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.25f, 130f, 1600f, false, SkillshotType.SkillshotLine);
            LoadMenuOKTW();

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalker.OnPreAttack += BeforeAttack;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (getCheckBoxItem(RMenu, "autoRinter") && R.IsReady() && sender.IsValidTarget(R.Range))
                R.Cast(sender);
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (R.IsReady())
            {
                var Target = gapcloser.Sender;
                if (Target.IsValidTarget(800) && getCheckBoxItem(RMenu, "GapCloser" + Target.NetworkId))
                {
                    R.Cast(Target.ServerPosition, true);
                    SebbyLib.Program.debug("AGC " + Target.ChampionName);
                }
            }
        }

        private static void BeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            LogicQ();
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (R.IsReady())
            {
                if (getKeyBindItem(RMenu, "useR"))
                {
                    var t = TargetSelector.GetTarget(1500, DamageType.Physical);
                    if (t.IsValidTarget())
                        R.Cast(t, true, true);
                }
            }

            if (SebbyLib.Program.LagFree(1))
            {
                SetMana();
                Jungle();
            }

            if (SebbyLib.Program.LagFree(3) && W.IsReady())
                LogicW();

            if (SebbyLib.Program.LagFree(4) && R.IsReady())
                LogicR();
        }

        private static void Jungle()
        {
            if (SebbyLib.Program.LaneClear)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, 600, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];

                    if (W.IsReady() && getCheckBoxItem(FarmMenu, "jungleW"))
                    {
                        W.Cast(mob.ServerPosition);
                        return;
                    }
                    if (Q.IsReady() && getCheckBoxItem(FarmMenu, "jungleQ"))
                    {
                        Q.Cast();
                    }
                }
            }
        }

        private static void LogicR()
        {
            if (getCheckBoxItem(RMenu, "autoR"))
            {
                foreach (
                    var target in
                        SebbyLib.Program.Enemies.Where(
                            target => target.IsValidTarget(R.Range) && OktwCommon.ValidUlt(target)))
                {
                    var rDmg = OktwCommon.GetKsDamage(target, R);
                    if (SebbyLib.Program.Combo && target.CountEnemiesInRange(250) > 2 &&
                        getCheckBoxItem(RMenu, "autoRaoe"))
                        SebbyLib.Program.CastSpell(R, target);
                    if (SebbyLib.Program.Combo && target.IsValidTarget(W.Range) && getCheckBoxItem(RMenu, "Rkscombo") &&
                        Player.GetAutoAttackDamage(target)*5 + rDmg + W.GetDamage(target) > target.Health &&
                        target.HasBuffOfType(BuffType.Slow) && !OktwCommon.IsSpellHeroCollision(target, R))
                        SebbyLib.Program.CastSpell(R, target);
                    if (rDmg > target.Health && target.CountAlliesInRange(600) == 0 &&
                        target.Distance(Player.Position) > 1000)
                    {
                        if (!OktwCommon.IsSpellHeroCollision(target, R))
                            SebbyLib.Program.CastSpell(R, target);
                    }
                }
            }

            if (Player.HealthPercent < 50)
            {
                foreach (
                    var enemy in
                        SebbyLib.Program.Enemies.Where(
                            enemy =>
                                enemy.IsValidTarget(300) && enemy.IsMelee &&
                                getCheckBoxItem(RMenu, "GapCloser" + enemy.NetworkId) && !OktwCommon.ValidUlt(enemy))
                    )
                {
                    R.Cast(enemy);
                    SebbyLib.Program.debug("R Meele");
                }
            }
        }

        private static void LogicQ()
        {
            var t = Orbwalker.LastTarget as AIHeroClient;
            if (t != null && t.IsValidTarget())
            {
                if (SebbyLib.Program.Combo &&
                    (Player.Mana > RMANA + QMANA || t.Health < 5*Player.GetAutoAttackDamage(Player)))
                    Q.Cast();
                else if (SebbyLib.Program.Farm && Player.Mana > RMANA + QMANA + WMANA && getCheckBoxItem(QMenu, "harasQ") &&
                         getCheckBoxItem(harassMenu, "haras" + t.NetworkId))
                    Q.Cast();
            }
            else if (SebbyLib.Program.LaneClear)
            {
                var minion = Orbwalker.LastTarget as Obj_AI_Minion;
                if (minion != null && Player.ManaPercent > getSliderItem(FarmMenu, "Mana") &&
                    getCheckBoxItem(FarmMenu, "farmQ") && Player.Mana > RMANA + QMANA)
                {
                    if (Cache.GetMinions(Player.ServerPosition, 600).Count >= getSliderItem(FarmMenu, "LCminions"))
                        Q.Cast();
                }
            }
        }

        private static void LogicW()
        {
            var t = Orbwalker.LastTarget as AIHeroClient ?? TargetSelector.GetTarget(W.Range, DamageType.Physical);

            if (t.IsValidTarget())
            {
                if (SebbyLib.Program.Combo && Player.Mana > RMANA + WMANA)
                    CastW(t);
                else if (SebbyLib.Program.Farm && Player.Mana > RMANA + WMANA + QMANA + WMANA && OktwCommon.CanHarras())
                {
                    foreach (
                        var enemy in
                            SebbyLib.Program.Enemies.Where(
                                enemy =>
                                    enemy.IsValidTarget(W.Range) &&
                                    getCheckBoxItem(harassMenu, "haras" + t.NetworkId)))
                        CastW(t);
                }
                else if (OktwCommon.GetKsDamage(t, W) > t.Health)
                {
                    CastW(t);
                }

                if (!SebbyLib.Program.None && Player.Mana > RMANA + WMANA)
                {
                    foreach (
                        var enemy in
                            SebbyLib.Program.Enemies.Where(
                                enemy => enemy.IsValidTarget(W.Range) && !OktwCommon.CanMove(enemy)))
                        W.Cast(t);
                }
            }
            else if (SebbyLib.Program.LaneClear && Player.ManaPercent > getSliderItem(FarmMenu, "Mana") &&
                     getCheckBoxItem(FarmMenu, "farmW") && Player.Mana > RMANA + WMANA)
            {
                var minionList = Cache.GetMinions(Player.ServerPosition, W.Range);
                var farmPosition = W.GetCircularFarmLocation(minionList, 300);

                if (farmPosition.MinionsHit >= getSliderItem(FarmMenu, "LCminions"))
                    W.Cast(farmPosition.Position);
            }
        }

        private static void CastW(Obj_AI_Base t)
        {
            var CoreType2 = SebbyLib.Prediction.SkillshotType.SkillshotLine;

            var predInput2 = new PredictionInput
            {
                Aoe = false,
                Collision = W.Collision,
                Speed = W.Speed,
                Delay = W.Delay,
                Range = W.Range,
                From = Player.ServerPosition,
                Radius = W.Width,
                Unit = t,
                Type = CoreType2
            };

            var poutput2 = Prediction.GetPrediction(predInput2);

            if (poutput2.Hitchance >= HitChance.High)
            {
                W.Cast(poutput2.CastPosition);
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
                RMANA = WMANA - Player.PARRegenRate*W.Instance.Cooldown;
            else
                RMANA = R.Instance.SData.Mana;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
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
        }
    }
}