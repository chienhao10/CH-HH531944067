using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;
using EloBuddy;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;

namespace OneKeyToWin_AIO_Sebby
{
    class Caitlyn
    {
        private Menu Config = Program.Config;
        private LeagueSharp.Common.Spell E, Q, Qc, R, W;
        private float QMANA = 0, WMANA = 0, EMANA = 0, RMANA = 0;

        private float QCastTime = 0;

        public AIHeroClient Player { get { return ObjectManager.Player; } }
        public AIHeroClient LastW = ObjectManager.Player;

        private static string[] Spells =
        {
            "katarinar","drain","consume","absolutezero", "staticfield","reapthewhirlwind","jinxw","jinxr","shenstandunited","threshe","threshrpenta","threshq","meditate","caitlynpiltoverpeacemaker", "volibearqattack",
            "cassiopeiapetrifyinggaze","ezrealtrueshotbarrage","galioidolofdurand","luxmalicecannon", "missfortunebullettime","infiniteduress","alzaharnethergrasp","lucianq","velkozr","rocketgrabmissile"
        };

        public static bool getBushW()
        {
            return getCheckBoxItem(wMenu, "bushW");
        }

        public void LoadOKTW()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 1250f);
            Qc = new LeagueSharp.Common.Spell(SpellSlot.Q, 1250f);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 800f);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 750f);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 3000f);


            Q.SetSkillshot(0.65f, 60f, 2200f, false, SkillshotType.SkillshotLine);
            Qc.SetSkillshot(0.65f, 60f, 2200f, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(1.5f, 20f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.25f, 70f, 1600f, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.7f, 200f, 1500f, false, SkillshotType.SkillshotCircle);

            LoadMenuOKTW();

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            //SebbyLib.Orbwalking.BeforeAttack += BeforeAttack;
            //SebbyLib.Orbwalking.AfterAttack += afterAttack;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
        }

        private void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.W)
            {
                if (ObjectManager.Get<Obj_GeneralParticleEmitter>().Any(obj => obj.IsValid && obj.Position.LSDistance(args.EndPosition) < 300 && obj.Name.ToLower().Contains("yordleTrap_idle_green.troy".ToLower())))
                    args.Process = false;
            }
        }

        public static Menu drawMenu, qMenu, wMenu, eMenu, rMenu, farmMenu;

        private void LoadMenuOKTW()
        {
            drawMenu = Config.AddSubMenu("线圈");
            drawMenu.Add("noti", new CheckBox("显示提示 & 线"));
            drawMenu.Add("qRange", new CheckBox("Q 范围"));
            drawMenu.Add("wRange", new CheckBox("W 范围"));
            drawMenu.Add("eRange", new CheckBox("E 范围"));
            drawMenu.Add("rRange", new CheckBox("R 范围"));
            drawMenu.Add("onlyRdy", new CheckBox("只显示无冷却技能"));

            qMenu = Config.AddSubMenu("Q 设置");
            qMenu.Add("autoQ2", new CheckBox("自动 Q"));
            qMenu.Add("autoQ", new CheckBox("减少Q使用次数"));

            wMenu = Config.AddSubMenu("W 设置");
            wMenu.Add("autoW", new CheckBox("对强控自动放W"));
            wMenu.Add("telE", new CheckBox("对传送自动W"));
            wMenu.Add("bushW", new CheckBox("草丛自动W"));
            wMenu.Add("Wspell", new CheckBox("对特殊技能使用 W"));
            wMenu.AddSeparator();
            wMenu.AddGroupLabel("防突进 : ");
            wMenu.Add("WmodeGC", new ComboBox("防突进位置模式", 0, "冲刺结束位置", "我英雄位置"));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                wMenu.Add("WGCchampion" + enemy.ChampionName, new CheckBox("对敌方使用 : " + enemy.ChampionName, true));

            eMenu = Config.AddSubMenu("E 设置");
            eMenu.Add("autoE", new CheckBox("自动 E"));
            eMenu.Add("harrasEQ", new CheckBox("骚扰 E + Q"));
            eMenu.Add("EQks", new CheckBox("抢头 E + Q + AA"));
            eMenu.Add("useE", new KeyBind("智能冲刺 E 按键", false, KeyBind.BindTypes.HoldActive, 'T'));
            eMenu.AddSeparator();
            eMenu.AddGroupLabel("防突进 : ");
            eMenu.Add("EmodeGC", new ComboBox("防突进位置模式", 2, "冲刺结束位置", "鼠标位置", "敌方位置"));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                eMenu.Add("EGCchampion" + enemy.ChampionName, new CheckBox("对敌方使用 : " + enemy.ChampionName, true));

            rMenu = Config.AddSubMenu("R 设置");
            rMenu.Add("autoR", new CheckBox("自动R抢头"));
            rMenu.Add("Rcol", new Slider("R 体积碰撞宽度 [400]", 400, 1, 1000));
            rMenu.Add("Rrange", new Slider("R 最低范围 [1000]", 1000, 1, 1500));
            rMenu.Add("useR", new KeyBind("半自动 R按键", false, KeyBind.BindTypes.HoldActive, 'T'));
            rMenu.Add("Rturrent", new CheckBox("塔下不R"));

            farmMenu = Config.AddSubMenu("农兵");
            farmMenu.Add("farmQ", new CheckBox("清线 Q"));
            farmMenu.Add("Mana", new Slider("清线蓝量", 80, 30));
            farmMenu.Add("LCminions", new Slider("清线最低小兵数量", 2, 0, 10));
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

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && (args.SData.Name == "CaitlynPiltoverPeacemaker" || args.SData.Name == "CaitlynEntrapment"))
            {
                QCastTime = Game.Time;
            }

            if (!W.IsReady() || sender.IsMinion || !sender.IsEnemy || !getCheckBoxItem(wMenu, "Wspell") || !sender.IsValid<AIHeroClient>() || !sender.LSIsValidTarget(W.Range))
                return;

            var foundSpell = Spells.Find(x => args.SData.Name.ToLower() == x);
            if (foundSpell != null)
            {
                W.Cast(sender.Position);
            }
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Player.Mana > RMANA + WMANA)
            {
                var t = gapcloser.Sender;
                if (E.IsReady() && t.LSIsValidTarget(E.Range) && getCheckBoxItem(eMenu, "EGCchampion" + t.ChampionName))
                {
                    if (getBoxItem(eMenu, "EmodeGC") == 0)
                        E.Cast(gapcloser.End);
                    else if (getBoxItem(eMenu, "EmodeGC") == 1)
                        E.Cast(Game.CursorPos);
                    else
                        E.Cast(t.ServerPosition);
                }
                else if (W.IsReady() && t.LSIsValidTarget(W.Range) && getCheckBoxItem(wMenu, "WGCchampion" + t.ChampionName))
                {
                    if (getBoxItem(eMenu, "WmodeGC") == 0)
                        W.Cast(gapcloser.End);
                    else
                        W.Cast(Player.ServerPosition);
                }
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (getKeyBindItem(rMenu, "useR") && R.IsReady())
            {
                var t = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                if (t.LSIsValidTarget())
                    R.CastOnUnit(t);
            }



            if (Program.LagFree(0))
            {
                SetMana();
                R.Range = (500 * R.Level) + 1500;
                //debug("" + ObjectManager.Player.AttackRange);
            }

            if (Program.LagFree(1) && E.IsReady() && !Player.Spellbook.IsAutoAttacking)
                LogicE();
            if (Program.LagFree(2) && W.IsReady() && !Player.Spellbook.IsAutoAttacking)
                LogicW();
            if (Program.LagFree(3) && Q.IsReady() && !Player.Spellbook.IsAutoAttacking && getCheckBoxItem(qMenu, "autoQ2"))
                LogicQ();
            if (Program.LagFree(4) && R.IsReady() && getCheckBoxItem(rMenu, "autoR") && !ObjectManager.Player.UnderTurret(true) && Game.Time - QCastTime > 1)
                LogicR();
            return;
        }

        private void LogicR()
        {
            bool cast = false;

            if (Player.UnderTurret(true) && getCheckBoxItem(rMenu, "Rturrent"))
                return;


            foreach (var target in Program.Enemies.Where(target => target.LSIsValidTarget(R.Range) && Player.LSDistance(target.Position) > getSliderItem(rMenu, "Rrange") && target.LSCountEnemiesInRange(getSliderItem(rMenu, "Rcol")) == 1 && target.CountAlliesInRange(500) == 0 && OktwCommon.ValidUlt(target)))
            {
                if (R.GetDamage(target) > target.Health)
                {
                    cast = true;
                    PredictionOutput output = R.GetPrediction(target);
                    Vector2 direction = output.CastPosition.LSTo2D() - Player.Position.LSTo2D();
                    direction.Normalize();
                    List<AIHeroClient> enemies = Program.Enemies.Where(x => x.LSIsValidTarget()).ToList();
                    foreach (var enemy in enemies)
                    {
                        if (enemy.BaseSkinName == target.BaseSkinName || !cast)
                            continue;
                        PredictionOutput prediction = R.GetPrediction(enemy);
                        Vector3 predictedPosition = prediction.CastPosition;
                        Vector3 v = output.CastPosition - Player.ServerPosition;
                        Vector3 w = predictedPosition - Player.ServerPosition;
                        double c1 = Vector3.Dot(w, v);
                        double c2 = Vector3.Dot(v, v);
                        double b = c1 / c2;
                        Vector3 pb = Player.ServerPosition + ((float)b * v);
                        float length = Vector3.Distance(predictedPosition, pb);
                        if (length < (getSliderItem(rMenu, "Rcol") + enemy.BoundingRadius) && Player.LSDistance(predictedPosition) < Player.LSDistance(target.ServerPosition))
                            cast = false;
                    }
                    if (cast)
                        R.CastOnUnit(target);
                }
            }
        }

        private void LogicW()
        {
            if (Player.Mana > RMANA + WMANA)
            {
                if (Program.Combo && Player.Spellbook.IsAutoAttacking)
                    return;
                if (getCheckBoxItem(wMenu, "autoW"))
                {
                    foreach (var enemy in Program.Enemies.Where(enemy => enemy.LSIsValidTarget(W.Range) && !OktwCommon.CanMove(enemy) && !enemy.HasBuff("caitlynyordletrapinternal")))
                    {
                        if (Utils.TickCount - W.LastCastAttemptT > 1000)
                        {
                            W.Cast(enemy.Position, true);
                            LastW = enemy;
                        }
                        else if (LastW.NetworkId != enemy.NetworkId)
                        {
                            W.Cast(enemy.Position, true);
                            LastW = enemy;
                        }
                    }
                }

                if (getCheckBoxItem(wMenu, "telE"))
                {
                    var trapPos = OktwCommon.GetTrapPos(W.Range);
                    if (!trapPos.IsZero)
                        W.Cast(trapPos);
                }

            }
        }

        private void LogicQ()
        {
            if (Program.Combo && Player.Spellbook.IsAutoAttacking)
                return;
            var t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (t.LSIsValidTarget(Q.Range))
            {
                if (GetRealDistance(t) > bonusRange() + 250 && !SebbyLib.Orbwalking.InAutoAttackRange(t) && OktwCommon.GetKsDamage(t, Q) > t.Health && Player.LSCountEnemiesInRange(400) == 0)
                {
                    Program.CastSpell(Q, t);
                    Program.debug("Q KS");
                }
                else if (Program.Combo && Player.Mana > RMANA + QMANA + EMANA + 10 && Player.LSCountEnemiesInRange(bonusRange() + 100 + t.BoundingRadius) == 0 && !getCheckBoxItem(qMenu, "autoQ"))
                    Program.CastSpell(Q, t);
                if ((Program.Combo || Program.Farm) && Player.Mana > RMANA + QMANA && Player.LSCountEnemiesInRange(400) == 0)
                {
                    foreach (var enemy in Program.Enemies.Where(enemy => enemy.LSIsValidTarget(Q.Range) && (!OktwCommon.CanMove(enemy) || enemy.HasBuff("caitlynyordletrapinternal"))))
                        Q.Cast(enemy, true);
                    if (Player.LSCountEnemiesInRange(bonusRange()) == 0 && OktwCommon.CanHarras())
                    {
                        if (t.HasBuffOfType(BuffType.Slow))
                            Q.Cast(t);

                        Q.CastIfWillHit(t, 2, true);
                    }
                }
            }
            else if (Program.LaneClear && Player.ManaPercent > getSliderItem(farmMenu, "Mana") && getCheckBoxItem(farmMenu, "farmQ") && Player.Mana > RMANA + QMANA)
            {
                var minionList = Cache.GetMinions(Player.ServerPosition, Q.Range);
                var farmPosition = Q.GetLineFarmLocation(minionList, Q.Width);
                if (farmPosition.MinionsHit > getSliderItem(farmMenu, "LCminions"))
                    Q.Cast(farmPosition.Position);
            }
        }

        private void LogicE()
        {
            if (Program.Combo && Player.Spellbook.IsAutoAttacking)
                return;
            if (getCheckBoxItem(eMenu, "autoE"))
            {
                var t = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                if (t.LSIsValidTarget())
                {
                    var positionT = Player.ServerPosition - (t.Position - Player.ServerPosition);

                    if (Q.IsReady() && Player.Position.LSExtend(positionT, 400).LSCountEnemiesInRange(700) < 2)
                    {
                        var eDmg = E.GetDamage(t);
                        var qDmg = Q.GetDamage(t);
                        if (getCheckBoxItem(eMenu, "EQks") && qDmg + eDmg + Player.LSGetAutoAttackDamage(t) > t.Health && Player.Mana > EMANA + QMANA)
                        {
                            Program.CastSpell(E, t);
                            Program.debug("E + Q FINISH");
                        }
                        else if ((Program.Farm || Program.Combo) && getCheckBoxItem(eMenu, "harrasEQ") && Player.Mana > EMANA + QMANA + RMANA)
                        {
                            Program.CastSpell(E, t);
                            Program.debug("E + Q Harras");
                        }
                    }

                    if (Player.Mana > RMANA + EMANA && Player.Health < Player.MaxHealth * 0.3)
                    {
                        if (GetRealDistance(t) < 500)
                            E.Cast(t, true);
                        if (Player.LSCountEnemiesInRange(250) > 0)
                            E.Cast(t, true);
                    }

                }
            }
            if (getKeyBindItem(eMenu, "useE"))
            {
                var position = Player.ServerPosition - (Game.CursorPos - Player.ServerPosition);
                E.Cast(position, true);
            }
        }

        private float GetRealRange(GameObject target)
        {
            return 680f + Player.BoundingRadius + target.BoundingRadius;
        }

        private float GetRealDistance(GameObject target)
        {
            return Player.ServerPosition.LSDistance(target.Position) + ObjectManager.Player.BoundingRadius + target.BoundingRadius;
        }
        public float bonusRange()
        {
            return 720f + Player.BoundingRadius;
        }
        private void SetMana()
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

        private void Drawing_OnDraw(EventArgs args)
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
            if (getCheckBoxItem(drawMenu, "noti"))
            {
                var t = TargetSelector.GetTarget(R.Range, DamageType.Physical);

                if (t.LSIsValidTarget() && R.IsReady())
                {
                    var rDamage = R.GetDamage(t);
                    if (rDamage > t.Health)
                    {
                        Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.5f, System.Drawing.Color.Red, "R 可击杀: " + t.ChampionName + " 剩下: " + t.Health + "hp");
                        drawLine(t.Position, Player.Position, 10, System.Drawing.Color.Yellow);
                    }
                }

                var tw = TargetSelector.GetTarget(W.Range, DamageType.Physical);
                if (tw.LSIsValidTarget())
                {
                    if (Q.GetDamage(tw) > tw.Health)
                        Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.4f, System.Drawing.Color.Red, "Q 可击杀: " + t.ChampionName + " 剩下: " + t.Health + "hp");
                }
            }
        }
    }
}