using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SebbyLib;
using HealthPrediction = SebbyLib.HealthPrediction;
using Orbwalking = SebbyLib.Orbwalking;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

namespace OneKeyToWin_AIO_Sebby
{
    internal class Urgot
    {
        private static readonly Menu Config = Program.Config;
        private static Spell Q, Q2, W, E, R;
        private static float QMANA, WMANA, EMANA, RMANA;
        private static int FarmId;

        private static readonly int Muramana = 3042;
        private static readonly int Tear = 3070;
        private static readonly int Manamune = 3004;

        public static Menu draw, w, e, r, harass, farm, item;

        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public static void LoadOKTW()
        {
            Q = new Spell(SpellSlot.Q, 980);
            Q2 = new Spell(SpellSlot.Q, 1200);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 900);
            R = new Spell(SpellSlot.R, 850);

            Q.SetSkillshot(0.25f, 60f, 1600f, true, SkillshotType.SkillshotLine);
            Q2.SetSkillshot(0.25f, 60f, 1600f, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 120f, 1500f, false, SkillshotType.SkillshotCircle);
            LoadMenuOKTW();

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Orbwalker.OnPreAttack += BeforeAttack;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += OnInterruptableSpell;
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

        private static void LoadMenuOKTW()
        {
            draw = Config.AddSubMenu("Drawing");
            draw.Add("onlyRdy", new CheckBox("Draw only ready spells"));
            draw.Add("qRange", new CheckBox("Q range", false));
            draw.Add("eRange", new CheckBox("E range", false));
            draw.Add("rRange", new CheckBox("R range", false));

            item = Config.AddSubMenu("Item");
            item.Add("mura", new CheckBox("Auto Muramana"));
            item.Add("stack", new CheckBox("Stack Tear if full mana"));

            w = Config.AddSubMenu("W Config");
            w.Add("autoW", new CheckBox("Auto W"));
            w.Add("Waa", new CheckBox("Auto W befor AA"));
            w.Add("AGC", new CheckBox("AntiGapcloserW"));
            w.Add("Wdmg", new Slider("W dmg % hp", 10));

            e = Config.AddSubMenu("E Config");
            e.Add("autoE", new CheckBox("Auto E haras"));

            r = Config.AddSubMenu("R Config");
            r.Add("autoR", new CheckBox("Auto R under turrent"));
            r.Add("inter", new CheckBox("OnPossibleToInterrupt R"));
            r.Add("Rhp", new Slider("dont R if under % hp", 50));
            r.Add("useR", new KeyBind("Semi-manual cast R key", false, KeyBind.BindTypes.HoldActive, 'T'));
                //32 == space
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != Player.Team))
                r.Add("GapCloser" + enemy.NetworkId, new CheckBox("GapClose : " + enemy.ChampionName));

            harass = Config.AddSubMenu("Harass");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                harass.Add("harras" + enemy.NetworkId, new CheckBox(enemy.ChampionName));

            farm = Config.AddSubMenu("Farm");
            farm.Add("farmQ", new CheckBox("Farm Q"));
            farm.Add("LC", new CheckBox("LaneClear"));
            farm.Add("Mana", new Slider("LaneClear Mana", 60));
            farm.Add("LCP", new CheckBox("FAST LaneClear"));
        }

        private static void OnInterruptableSpell(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (getCheckBoxItem(r, "inter") && R.IsReady() && sender.IsValidTarget(R.Range))
                R.Cast(sender);
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (R.IsReady())
            {
                var t = gapcloser.Sender;
                if (getCheckBoxItem(r, "GapCloser" + t.NetworkId) && t.IsValidTarget(R.Range))
                {
                    R.Cast(t);
                }
            }
            if (getCheckBoxItem(w, "AGC") && W.IsReady() && Player.Mana > RMANA + WMANA)
            {
                var Target = gapcloser.Sender;
                if (Target.IsValidTarget(E.Range))
                    W.Cast();
            }
        }

        private static void BeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (FarmId != args.Target.NetworkId)
                FarmId = args.Target.NetworkId;
            if (W.IsReady() && getCheckBoxItem(w, "Waa") && args.Target.IsValid<AIHeroClient>() &&
                Player.Mana > WMANA + QMANA*4)
                W.Cast();

            if (getCheckBoxItem(item, "mura"))
            {
                var Mur = Items.HasItem(Muramana) ? 3042 : 3043;
                if (!Player.HasBuff("Muramana") && args.Target.IsEnemy && args.Target.IsValid<AIHeroClient>() &&
                    Items.HasItem(Mur) && Items.CanUseItem(Mur) && Player.Mana > RMANA + EMANA + QMANA + WMANA)
                    Items.UseItem(Mur);
                else if (Player.HasBuff("Muramana") && Items.HasItem(Mur) && Items.CanUseItem(Mur))
                    Items.UseItem(Mur);
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (getKeyBindItem(r, "useR"))
            {
                var tr = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                if (tr.IsValidTarget())
                    R.Cast(tr);
            }

            if (Program.LagFree(0))
            {
                SetMana();
            }

            if (Program.LagFree(1) && !Orbwalker.IsAutoAttacking && E.IsReady())
                LogicE();

            if (Program.LagFree(2) && W.IsReady() && getCheckBoxItem(w, "autoW"))
                LogicW();

            if (Program.LagFree(3) && !Orbwalker.IsAutoAttacking && Q.IsReady())
            {
                LogicQ();
                LogicQ2();
            }

            if (Program.LagFree(4) && !Orbwalker.IsAutoAttacking && R.IsReady())
                LogicR();
        }

        private static void LogicW()
        {
            if (Player.Mana > RMANA + WMANA && !Player.LSIsRecalling())
            {
                var sensitivity = 20;
                var dmg = OktwCommon.GetIncomingDamage(Player);
                var nearEnemys = Player.CountEnemiesInRange(900);
                var shieldValue = 20 + W.Level*40 + 0.08*Player.MaxMana + 0.8*Player.FlatMagicDamageMod;
                var HpPercentage = dmg*100/Player.Health;

                nearEnemys = nearEnemys == 0 ? 1 : nearEnemys;

                if (dmg > shieldValue)
                    W.Cast();
                else if (HpPercentage >= getSliderItem(w, "Wdmg"))
                    W.Cast();
                else if (Player.Health - dmg < nearEnemys*Player.Level*sensitivity)
                    W.Cast();
            }
        }

        private static void LogicQ2()
        {
            if (Program.Farm && getCheckBoxItem(farm, "farmQ"))
                farmQ();
            else if (getCheckBoxItem(item, "stack") && Utils.TickCount - Q.LastCastAttemptT > 4000 &&
                     !Player.HasBuff("Recall") && Player.Mana > Player.MaxMana*0.95 &&
                     Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None) &&
                     (Items.HasItem(Tear) || Items.HasItem(Manamune)))
                Q.Cast(Player.ServerPosition);
        }

        private static void LogicQ()
        {
            if (Program.Combo || Program.Farm)
            {
                foreach (
                    var enemy in
                        Program.Enemies.Where(
                            enemy => enemy.IsValidTarget(Q2.Range) && enemy.HasBuff("urgotcorrosivedebuff")))
                {
                    if (W.IsReady() && (Player.Mana > WMANA + QMANA*4 || Q.GetDamage(enemy)*3 > enemy.Health) &&
                        getCheckBoxItem(w, "autoW"))
                    {
                        W.Cast();
                        Program.debug("W");
                    }
                    Program.debug("E");
                    Q2.Cast(enemy.ServerPosition);
                    return;
                }
            }

            var t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);

            if (Player.CountEnemiesInRange(Q.Range - 200) > 0)
                t = TargetSelector.GetTarget(Q.Range - 200, DamageType.Physical);

            if (t.IsValidTarget())
            {
                if (Program.Combo && Player.Mana > RMANA + QMANA)
                    Program.CastSpell(Q, t);
                else if (Program.Farm && Player.Mana > RMANA + EMANA + QMANA + WMANA && OktwCommon.CanHarras())
                {
                    foreach (
                        var enemy in
                            Program.Enemies.Where(
                                enemy =>
                                    enemy.IsValidTarget(Q.Range) && getCheckBoxItem(harass, "harras" + t.NetworkId)))
                        Program.CastSpell(Q, enemy);
                }
                else if (OktwCommon.GetKsDamage(t, Q)*2 > t.Health)
                {
                    Program.CastSpell(Q, t);
                }
                if (!Program.None && Player.Mana > RMANA + QMANA)
                {
                    foreach (
                        var enemy in
                            Program.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                        Q.Cast(enemy, true);
                }
            }
        }

        private static void LogicR()
        {
            R.Range = 400 + 150*R.Level;
            if (Player.UnderTurret(false) && !ObjectManager.Player.UnderTurret(true) &&
                Player.HealthPercent >= getSliderItem(r, "Rhp") && getCheckBoxItem(r, "autoR"))
            {
                foreach (
                    var target in
                        Program.Enemies.Where(target => target.IsValidTarget(R.Range) && OktwCommon.ValidUlt(target)))
                {
                    if (target.CountEnemiesInRange(700) < 2 + Player.CountAlliesInRange(700))
                    {
                        R.Cast(target);
                    }
                }
            }
        }

        private static void LogicE()
        {
            var qCd = Q.Instance.CooldownExpires - Game.Time;

            var t = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            if (t.IsValidTarget())
            {
                var qDmg = Q.GetDamage(t);
                var eDmg = E.GetDamage(t);
                if (eDmg > t.Health)
                    E.Cast(t);
                else if (eDmg + qDmg > t.Health && Player.Mana > EMANA + QMANA)
                    Program.CastSpell(E, t);
                else if (eDmg + 3*qDmg > t.Health && Player.Mana > EMANA + QMANA*3)
                    Program.CastSpell(E, t);
                else if (Program.Combo && Player.Mana > EMANA + QMANA*2 && qCd < 0.5f)
                    Program.CastSpell(E, t);
                else if (Program.Farm && Player.Mana > RMANA + EMANA + QMANA*5 && getCheckBoxItem(e, "autoE") &&
                         getCheckBoxItem(harass, "harras" + t.NetworkId))
                    Program.CastSpell(E, t);
                else if ((Program.Combo || Program.Farm) && Player.Mana > RMANA + WMANA + EMANA)
                {
                    foreach (
                        var enemy in
                            Program.Enemies.Where(
                                enemy => enemy.IsValidTarget(E.Range) && !OktwCommon.CanMove(enemy)))
                        E.Cast(enemy, true, true);
                }
            }
        }

        public static void farmQ()
        {
            if (Program.LaneClear)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, 800, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    Q.Cast(mob, true);
                    return;
                }
            }

            if (!getCheckBoxItem(farm, "farmQ"))
                return;

            var minions = Cache.GetMinions(Player.ServerPosition, Q.Range);

            var orbTarget = 0;
            if (Orbwalker.LastTarget != null)
                orbTarget = Orbwalker.LastTarget.NetworkId;

            if (minions.Where(
                minion =>
                    orbTarget != minion.NetworkId && !ObjectManager.Player.IsInAutoAttackRange(minion) &&
                    minion.Health < Q.GetDamage(minion)).Any(minion => Q.Cast(minion) == Spell.CastStates.SuccessfullyCasted))
            {
                return;
            }

            if (getCheckBoxItem(farm, "LC") && Program.LaneClear && !Orbwalking.CanAttack() &&
                Player.ManaPercent > getSliderItem(farm, "Mana"))
            {
                var LCP = getCheckBoxItem(farm, "LCP");

                foreach (
                    var minion in
                        minions.Where(
                            minion => ObjectManager.Player.IsInAutoAttackRange(minion) && orbTarget != minion.NetworkId)
                    )
                {
                    var hpPred = HealthPrediction.GetHealthPrediction(minion, 300);
                    var dmgMinion = minion.GetAutoAttackDamage(minion);
                    var qDmg = Q.GetDamage(minion);
                    if (hpPred < qDmg)
                    {
                        if (hpPred > dmgMinion)
                        {
                            if (Q.Cast(minion) == Spell.CastStates.SuccessfullyCasted)
                                return;
                        }
                    }
                    else if (LCP)
                    {
                        if (hpPred > dmgMinion + qDmg)
                        {
                            if (Q.Cast(minion) == Spell.CastStates.SuccessfullyCasted)
                                return;
                        }
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
            if (getCheckBoxItem(draw, "qRange"))
            {
                if (getCheckBoxItem(draw, "onlyRdy"))
                {
                    if (Q.IsReady())
                        Utility.DrawCircle(Player.Position, Q.Range, Color.Cyan, 1, 1);
                }
                else
                    Utility.DrawCircle(Player.Position, Q.Range, Color.Cyan, 1, 1);
            }
            if (getCheckBoxItem(draw, "eRange"))
            {
                if (getCheckBoxItem(draw, "onlyRdy"))
                {
                    if (E.IsReady())
                        Utility.DrawCircle(Player.Position, E.Range, Color.Yellow, 1, 1);
                }
                else
                    Utility.DrawCircle(Player.Position, E.Range, Color.Yellow, 1, 1);
            }
            if (getCheckBoxItem(draw, "rRange"))
            {
                if (getCheckBoxItem(draw, "onlyRdy"))
                {
                    if (R.IsReady())
                        Utility.DrawCircle(Player.Position, R.Range, Color.Gray, 1, 1);
                }
                else
                    Utility.DrawCircle(Player.Position, R.Range, Color.Gray, 1, 1);
            }
        }
    }
}