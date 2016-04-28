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
using HealthPrediction = SebbyLib.HealthPrediction;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

namespace OneKeyToWin_AIO_Sebby.Champions
{
    internal class Karthus
    {
        private static readonly Menu Config = Program.Config;
        public static Menu drawMenu, qMenu, wMenu, eMenu, rMenu, harassMenu, farmMenu, miscMenu;
        private static Spell E, Q, R, W;
        private static float QMANA, WMANA, EMANA, RMANA;

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
            Q = new Spell(SpellSlot.Q, 890);
            W = new Spell(SpellSlot.W, 1000);
            E = new Spell(SpellSlot.E, 520);
            R = new Spell(SpellSlot.R, 20000);

            Q.SetSkillshot(1.02f, 150f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(0.5f, 50f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            R.DamageType = DamageType.Magical;

            drawMenu = Config.AddSubMenu("Draw");
            drawMenu.Add("noti", new CheckBox("Show R notification"));
            drawMenu.Add("qRange", new CheckBox("Q range", false));
            drawMenu.Add("wRange", new CheckBox("W range", false));
            drawMenu.Add("eRange", new CheckBox("E range", false));
            drawMenu.Add("rRange", new CheckBox("R range", false));
            drawMenu.Add("onlyRdy", new CheckBox("Draw when skill rdy"));

            qMenu = Config.AddSubMenu("Q Config");
            qMenu.Add("autoQ", new CheckBox("Auto Q"));
            qMenu.Add("harrasQ", new CheckBox("Harass Q"));
            qMenu.Add("QHarassMana", new Slider("Harass Mana", 30));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                qMenu.Add("Qon" + enemy.ChampionName, new CheckBox(enemy.ChampionName));

            wMenu = Config.AddSubMenu("W Config");
            wMenu.Add("autoW", new CheckBox("Auto W"));
            wMenu.Add("harrasW", new CheckBox("Harass W", false));
            wMenu.Add("WmodeCombo", new ComboBox("W combo mode", 1, "always", "run - cheese"));
            wMenu.Add("WmodeGC", new ComboBox("Gap Closer position mode", 0, "Dash end position", "My hero position"));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                wMenu.Add("WGCchampion" + enemy.ChampionName, new CheckBox("W : " + enemy.ChampionName));

            eMenu = Config.AddSubMenu("E Config");
            eMenu.Add("autoE", new CheckBox("Auto E if enemy in range"));
            eMenu.Add("Emana", new Slider("E % minimum mana", 20));

            rMenu = Config.AddSubMenu("R Config");
            rMenu.Add("autoR", new CheckBox("Auto R"));
            rMenu.Add("autoRzombie", new CheckBox("Auto R upon dying if can help team"));
            rMenu.Add("Renemy", new Slider("Don't R if enemy in x range", 1500, 0, 2000));
            rMenu.Add("RenemyA", new Slider("Don't R if ally in x range near target", 800, 0, 2000));
            rMenu.Add("Rturrent", new CheckBox("Don't R under turret"));

            harassMenu = Config.AddSubMenu("Harass Config");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                harassMenu.Add("harras" + enemy.ChampionName, new CheckBox(enemy.ChampionName));

            farmMenu = Config.AddSubMenu("Farm");
            farmMenu.Add("farmQout", new CheckBox("Last hit Q minion out range AA"));
            farmMenu.Add("farmQ", new CheckBox("Lane clear Q"));
            farmMenu.Add("farmE", new CheckBox("Lane clear E"));
            farmMenu.Add("Mana", new Slider("LaneClear Mana", 80));
            farmMenu.Add("QLCminions", new Slider(" QLaneClear minimum minions", 2, 0, 10));
            farmMenu.Add("ELCminions", new Slider(" ELaneClear minimum minions", 5, 0, 10));
            farmMenu.Add("jungleQ", new CheckBox("Jungle clear Q"));
            farmMenu.Add("jungleE", new CheckBox("Jungle clear E"));

            miscMenu = Config.AddSubMenu("Misc");
            miscMenu.Add("autoZombie", new CheckBox("Auto zombie mode COMBO / LANECLEAR"));

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsRecalling())
                return;

            if (Player.IsZombie)
            {
                if (getCheckBoxItem(miscMenu, "autoZombie"))
                {
                    Orbwalker.ActiveModesFlags = Player.CountEnemiesInRange(Q.Range) > 0 ? Orbwalker.ActiveModes.Combo : Orbwalker.ActiveModes.LaneClear;
                }
                if (R.IsReady() && getCheckBoxItem(miscMenu, "autoRzombie"))
                {
                    float timeDeadh = 8;
                    timeDeadh = OktwCommon.GetPassiveTime(Player, "KarthusDeathDefiedBuff");
                    Program.debug("Time " + timeDeadh);
                    if (timeDeadh < 4)
                    {
                        foreach (
                            var target in
                                Program.Enemies.Where(target => target.IsValidTarget() && OktwCommon.ValidUlt(target)))
                        {
                            var rDamage = R.GetDamage(target);
                            if (target.Health < 3*rDamage && target.CountAlliesInRange(800) > 0)
                                R.Cast();
                            if (target.Health < rDamage*1.5 && target.Distance(Player.Position) < 900)
                                R.Cast();
                            if (target.Health + target.HPRegenRate*5 < rDamage)
                                R.Cast();
                        }
                    }
                }
            }
            else
            {
                Orbwalker.ActiveModesFlags = Orbwalker.ActiveModes.None;
            }

            if (Program.LagFree(0))
            {
                SetMana();
                Jungle();
            }
            if (Program.LagFree(1) && Q.IsReady() && getCheckBoxItem(qMenu, "autoQ"))
                LogicQ();
            if (Program.LagFree(2) && E.IsReady() && getCheckBoxItem(eMenu, "autoE"))
                LogicE();
            if (Program.LagFree(3) && R.IsReady())
                LogicR();
            if (Program.LagFree(4) && W.IsReady() && getCheckBoxItem(eMenu, "autoW"))
                LogicW();
        }

        private static void LogicR()
        {
            if (getCheckBoxItem(rMenu, "autoR") && Player.CountEnemiesInRange(getSliderItem(rMenu, "Renemy")) == 0)
            {
                if (Player.UnderTurret(true) && getCheckBoxItem(rMenu, "Rturrent"))
                    return;

                foreach (var target in Program.Enemies.Where(target => target.IsValid && !target.IsDead))
                {
                    if (target.IsValidTarget() && target.CountAlliesInRange(getSliderItem(rMenu, "RenemyA")) == 0)
                    {
                        var predictedHealth = target.Health + target.HPRegenRate*4;
                        var Rdmg = OktwCommon.GetKsDamage(target, R);

                        if (target.HealthPercent > 30)
                        {
                            if (Items.HasItem(3155, target))
                            {
                                Rdmg = Rdmg - 250;
                            }

                            if (Items.HasItem(3156, target))
                            {
                                Rdmg = Rdmg - 400;
                            }
                        }

                        if (Rdmg > predictedHealth && OktwCommon.ValidUlt(target))
                        {
                            R.Cast();
                            Program.debug("R normal");
                        }
                    }
                    else if (!target.IsVisible)
                    {
                        var ChampionInfoOne = OKTWtracker.ChampionInfoList.Find(x => x.NetworkId == target.NetworkId);
                        if (ChampionInfoOne != null)
                        {
                            var timeInvisible = Game.Time - ChampionInfoOne.LastVisableTime;
                            if (timeInvisible > 3 && timeInvisible < 10)
                            {
                                var predictedHealth = target.Health + target.HPRegenRate*(4 + timeInvisible);
                                if (R.GetDamage(target) > predictedHealth)
                                    R.Cast();
                            }
                        }
                    }
                }
            }
        }

        private static float GetQDamage(Obj_AI_Base t)
        {
            var minions = Cache.GetMinions(t.Position, Q.Width + 20);

            if (minions.Count > 1)
                return Q.GetDamage(t, 1);
            return Q.GetDamage(t);
        }

        public static float GetRealAutoAttackRange(AttackableUnit target)
        {
            var result = ObjectManager.Player.AttackRange + ObjectManager.Player.BoundingRadius;
            if (target.IsValidTarget())
            {
                var aiBase = target as Obj_AI_Base;
                if (aiBase != null && ObjectManager.Player.ChampionName == "Caitlyn")
                {
                    if (aiBase.HasBuff("caitlynyordletrapinternal"))
                    {
                        result += 650;
                    }
                }

                return result + target.BoundingRadius;
            }

            return result;
        }

        public static bool InAutoAttackRange(AttackableUnit target)
        {
            if (!target.IsValidTarget())
            {
                return false;
            }
            var myRange = GetRealAutoAttackRange(target);
            return
                Vector2.DistanceSquared(
                    target is Obj_AI_Base ? ((Obj_AI_Base) target).ServerPosition.To2D() : target.Position.To2D(),
                    ObjectManager.Player.ServerPosition.To2D()) <= myRange*myRange;
        }

        private static void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (t.IsValidTarget() && getCheckBoxItem(qMenu, "Qon" + t.ChampionName))
            {
                if (Program.Combo && Player.Mana > RMANA + QMANA + WMANA)
                    Program.CastSpell(Q, t);
                else if (Program.Farm && OktwCommon.CanHarras() && getCheckBoxItem(qMenu, "harrasQ")
                         && getCheckBoxItem(harassMenu, "harras" + t.ChampionName) &&
                         Player.ManaPercent > getSliderItem(qMenu, "QHarassMana"))
                    Program.CastSpell(Q, t);
                else if (OktwCommon.GetKsDamage(t, Q) > t.Health)
                    Program.CastSpell(Q, t);

                foreach (
                    var enemy in
                        Program.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                    Program.CastSpell(Q, t);
            }
            if (!OktwCommon.CanHarras())
                return;

            if (!Program.None && !Program.Combo && Player.Mana > RMANA + QMANA*2)
            {
                var allMinions = Cache.GetMinions(Player.ServerPosition, Q.Range);

                if (getCheckBoxItem(farmMenu, "farmQout"))
                {
                    foreach (
                        var minion in
                            allMinions.Where(
                                minion =>
                                    minion.IsValidTarget(Q.Range) &&
                                    (!InAutoAttackRange(minion) || (!minion.UnderTurret(true) && minion.UnderTurret())))
                        )
                    {
                        var hpPred = HealthPrediction.GetHealthPrediction(minion, 1100);
                        if (hpPred < GetQDamage(minion)*0.9 && hpPred > minion.Health - hpPred*2)
                        {
                            Q.Cast(minion);
                            return;
                        }
                    }
                }

                if (Program.LaneClear && getCheckBoxItem(farmMenu, "farmQ") &&
                    Player.ManaPercent > getSliderItem(farmMenu, "Mana"))
                {
                    foreach (
                        var minion in
                            allMinions.Where(minion => minion.IsValidTarget(Q.Range) && InAutoAttackRange(minion)))
                    {
                        var hpPred = HealthPrediction.GetHealthPrediction(minion, 1100);
                        if (hpPred < GetQDamage(minion)*0.9 && hpPred > minion.Health - hpPred*2)
                        {
                            Q.Cast(minion);
                            return;
                        }
                    }
                }

                if (Program.LaneClear && Player.ManaPercent > getSliderItem(farmMenu, "Mana") &&
                    getCheckBoxItem(farmMenu, "farmQ"))
                {
                    var farmPos = Q.GetCircularFarmLocation(allMinions, Q.Width);
                    if (farmPos.MinionsHit >= getSliderItem(farmMenu, "QLCminions"))
                        Q.Cast(farmPos.Position);
                }
            }
        }

        private static void LogicW()
        {
            if ((Program.Combo || (Program.Farm && getCheckBoxItem(wMenu, "harrasW"))) && Player.Mana > RMANA + WMANA)
            {
                if (getBoxItem(wMenu, "WmodeCombo") == 1)
                {
                    var t = TargetSelector.GetTarget(W.Range, DamageType.Magical);
                    if (t.IsValidTarget(W.Range) && W.GetPrediction(t).CastPosition.Distance(t.Position) > 100)
                    {
                        if (Player.Position.Distance(t.ServerPosition) > Player.Position.Distance(t.Position))
                        {
                            if (t.Position.Distance(Player.ServerPosition) < t.Position.Distance(Player.Position))
                                Program.CastSpell(W, t);
                        }
                        else
                        {
                            if (t.Position.Distance(Player.ServerPosition) > t.Position.Distance(Player.Position))
                                Program.CastSpell(W, t);
                        }
                    }
                }
                else
                {
                    var t = TargetSelector.GetTarget(W.Range, DamageType.Magical);
                    if (t.IsValidTarget())
                    {
                        Program.CastSpell(W, t);
                    }
                }
            }
        }

        private static void LogicE()
        {
            if (Program.None)
                return;

            if (Player.HasBuff("KarthusDefile"))
            {
                if (Program.LaneClear)
                {
                    if (OktwCommon.CountEnemyMinions(Player, E.Range) < getSliderItem(farmMenu, "ELCminions") ||
                        Player.ManaPercent < getSliderItem(farmMenu, "Mana"))
                        E.Cast();
                }
                else if (getCheckBoxItem(eMenu, "autoE"))
                {
                    if (Player.ManaPercent < getSliderItem(eMenu, "Emana") || Player.CountEnemiesInRange(E.Range) == 0)
                        E.Cast();
                }
            }
            else
            {
                if (Program.LaneClear &&
                    OktwCommon.CountEnemyMinions(Player, E.Range) >= getSliderItem(farmMenu, "ELCminions") &&
                    Player.ManaPercent > getSliderItem(farmMenu, "Mana"))
                    E.Cast();
                else if (getCheckBoxItem(eMenu, "autoE") && Player.ManaPercent > getSliderItem(eMenu, "Emana") &&
                         Player.CountEnemiesInRange(E.Range) > 0)
                {
                    E.Cast();
                }
            }
        }

        private static void Jungle()
        {
            if (Program.LaneClear && Player.Mana > RMANA + QMANA)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, Q.Range, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    if (Q.IsReady() && getCheckBoxItem(farmMenu, "jungleQ"))
                    {
                        Q.Cast(mob.ServerPosition);
                        return;
                    }
                    if (E.IsReady() && getCheckBoxItem(farmMenu, "jungleE") && mob.IsValidTarget(E.Range))
                    {
                        E.Cast(mob.ServerPosition);
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
            if (R.IsReady() && getCheckBoxItem(drawMenu, "noti"))
            {
                var t = TargetSelector.GetTarget(R.Range, DamageType.Magical);

                if (t.IsValidTarget() && OktwCommon.GetKsDamage(t, R) > t.Health)
                {
                    Drawing.DrawText(Drawing.Width*0.1f, Drawing.Height*0.5f, Color.Red,
                        "Ult can kill: " + t.ChampionName + " Heal - damage =  " +
                        (t.Health - OktwCommon.GetKsDamage(t, R)) + " hp");
                }
            }
        }
    }
}