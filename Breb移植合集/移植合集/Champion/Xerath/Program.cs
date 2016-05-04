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
    internal class Xerath
    {
        private static readonly Menu Config = Program.Config;
        private static Spell Q, W, E, R;
        private static float QMANA, WMANA, EMANA, RMANA;
        private static Vector3 Rtarget;
        private static float lastR;

        private static readonly Items.Item FarsightOrb = new Items.Item(3342, 4000f);
        private static readonly Items.Item ScryingOrb = new Items.Item(3363, 3500f);

        public static Menu drawMenu, qMenu, wMenu, eMenu, rMenu, harassMenu, farmMenu, miscMenu;

        public static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        private static bool IsCastingR
        {
            get
            {
                return Player.HasBuff("XerathLocusOfPower2") ||
                       (ObjectManager.Player.LastCastedSpellName() == "XerathLocusOfPower2" &&
                        Utils.TickCount - ObjectManager.Player.LastCastedSpellT() < 500);
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

        public static void LoadOKTW()
        {
            Q = new Spell(SpellSlot.Q, 1550);
            W = new Spell(SpellSlot.W, 1100);
            E = new Spell(SpellSlot.E, 1040);
            R = new Spell(SpellSlot.R, 675);

            Q.SetSkillshot(0.6f, 100f, float.MaxValue, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.7f, 125f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.25f, 60f, 1400f, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.7f, 120f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            Q.SetCharged("XerathArcanopulseChargeUp", "XerathArcanopulseChargeUp", 700, 1550, 1.5f);

            drawMenu = Config.AddSubMenu("Drawings");
            drawMenu.Add("noti", new CheckBox("Show notification & line"));
            drawMenu.Add("onlyRdy", new CheckBox("Draw only ready spells"));
            drawMenu.Add("qRange", new CheckBox("Q range", false));
            drawMenu.Add("wRange", new CheckBox("W range", false));
            drawMenu.Add("eRange", new CheckBox("E range", false));
            drawMenu.Add("rRange", new CheckBox("R range", false));
            drawMenu.Add("rRangeMini", new CheckBox("R range minimap"));

            qMenu = Config.AddSubMenu("Q Config");
            qMenu.Add("autoQ", new CheckBox("Auto Q"));

            wMenu = Config.AddSubMenu("W Config");
            wMenu.Add("autoW", new CheckBox("Auto W"));
            wMenu.Add("harrasW", new CheckBox("Harras W"));

            eMenu = Config.AddSubMenu("E Config");
            eMenu.Add("autoE", new CheckBox("Auto E"));
            eMenu.Add("harrasE", new CheckBox("Harras E"));

            rMenu = Config.AddSubMenu("R Config");
            rMenu.Add("autoR", new CheckBox("Auto R 2 x dmg R"));
            rMenu.Add("autoRlast", new CheckBox("Cast last position if no target"));
            rMenu.Add("useR", new KeyBind("Semi-manual cast R key", false, KeyBind.BindTypes.HoldActive, 'T'));
                //32 == space
            rMenu.Add("trinkiet", new CheckBox("Auto blue trinkiet"));
            rMenu.Add("delayR", new Slider("custome R delay ms (1000ms = 1 sec)", 0, 0, 3000));
            rMenu.Add("MaxRangeR", new Slider("Max R adjustment (R range - slider)", 0, 0, 5000));

            harassMenu = Config.AddSubMenu("Harass");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                harassMenu.Add("harras" + enemy.ChampionName, new CheckBox(enemy.ChampionName));

            farmMenu = Config.AddSubMenu("Farm");
            farmMenu.Add("separate", new CheckBox("Separate laneclear from harras", false));
            farmMenu.Add("farmQ", new CheckBox("Lane clear Q"));
            farmMenu.Add("farmW", new CheckBox("Lane clear W"));
            farmMenu.Add("Mana", new Slider("LaneClear Mana", 80));
            farmMenu.Add("LCminions", new Slider("LaneClear minimum minions", 2, 0, 10));
            farmMenu.Add("jungleE", new CheckBox("Jungle clear E"));
            farmMenu.Add("jungleQ", new CheckBox("Jungle clear Q"));
            farmMenu.Add("jungleW", new CheckBox("Jungle clear W"));

            miscMenu = Config.AddSubMenu("Misc");
            miscMenu.Add("force", new CheckBox("Force passive use in combo on minion"));

            Game.OnUpdate += Game_OnGameUpdate;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += Drawing_OnEndScene;

            Orbwalker.OnPreAttack += Orbwalking_BeforeAttack;
            Orbwalker.OnPostAttack += Orbwalking_AfterAttack;

            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            EloBuddy.Player.OnIssueOrder += Obj_AI_Base_OnIssueOrder;
        }

        private static void Obj_AI_Base_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (args.Order == GameObjectOrder.AttackUnit && Q.IsCharging)
            {
                Program.debug("BADDDD");
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                args.Process = false;
            }
        }

        private static void Orbwalking_AfterAttack(AttackableUnit target, EventArgs args)
        {
            Orbwalker.ForcedTarget = null; // BERB - not even sure what this is even suppose to do lmfao
        }

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.R)
            {
                if (getCheckBoxItem(rMenu, "trinkiet") && !IsCastingR)
                {
                    ScryingOrb.Range = Player.Level < 9 ? 2500 : 3500;

                    if (ScryingOrb.IsReady())
                        ScryingOrb.Cast(Rtarget);
                    if (FarsightOrb.IsReady())
                        FarsightOrb.Cast(Rtarget);
                }
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name == "xerathlocuspulse")
                {
                    lastR = Game.Time;
                }
            }
        }

        private static void Orbwalking_BeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (args.Target.IsValid<Obj_AI_Minion>() && !Player.HasBuff("xerathascended2onhit") && Program.Combo)
            {
                args.Process = false;
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Player.Distance(gapcloser.Sender.ServerPosition) < E.Range)
            {
                E.Cast(gapcloser.Sender);
            }
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Player.Distance(sender.ServerPosition) < E.Range)
            {
                E.Cast(sender);
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Q.IsCharging && (int)(Game.Time * 10) % 2 == 0)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }

            //Program.debug(""+OktwCommon.GetPassiveTime(Player, "XerathArcanopulseChargeUp"));
            if (IsCastingR)
            {
                Orbwalker.DisableAttacking = true;
                Orbwalker.DisableMovement = true;
            }
            else
            {
                Orbwalker.DisableAttacking = false;
                Orbwalker.DisableMovement = false;
            }

            if (Program.LagFree(1))
            {
                SetMana();
                Jungle();
                int[] mana = {0, 30, 33, 36, 42, 48, 54, 63, 72, 81, 90, 102, 114, 126, 138, 150, 165, 180, 195};
                if (!Player.HasBuff("xerathascended2onhit") || Player.Mana + mana[Player.Level] > Player.MaxMana)
                    Orbwalker.ForcedTarget = null;
                else if ((Program.Combo || Program.Farm) && getCheckBoxItem(miscMenu, "force") &&
                         Orbwalker.LastTarget == null)
                {
                    var minion =
                        Cache.GetMinions(Player.ServerPosition, Player.AttackRange + Player.BoundingRadius*2)
                            .OrderByDescending(x => x.Health)
                            .FirstOrDefault();

                    if (minion != null)
                        Orbwalker.ForcedTarget = minion;
                }
            }

            if (E.IsReady() && getCheckBoxItem(eMenu, "autoE"))
                LogicE();
            if (Program.LagFree(2) && W.IsReady() && !Orbwalker.IsAutoAttacking && getCheckBoxItem(wMenu, "autoW"))
                LogicW();
            if (Program.LagFree(3) && R.IsReady() && getCheckBoxItem(rMenu, "autoR"))
                LogicR();
            if (Program.LagFree(4) && Q.IsReady() && !Orbwalker.IsAutoAttacking && getCheckBoxItem(qMenu, "autoQ"))
                LogicQ();
        }

        private static void LogicR()
        {
            R.Range = 2000 + R.Level*1200;
            if (!IsCastingR)
                R.Range = R.Range - getSliderItem(rMenu, "MaxRangeR");

            var t = TargetSelector.GetTarget(R.Range, DamageType.Magical);
            if (t.LSIsValidTarget())
            {
                if (getKeyBindItem(rMenu, "useR") && !IsCastingR)
                {
                    R.Cast();
                }
                if (!t.IsValidTarget(W.Range) && !IsCastingR && t.CountAlliesInRange(500) == 0 && Player.CountEnemiesInRange(1100) == 0)
                {
                    if (OktwCommon.GetKsDamage(t, R) + R.GetDamage(t) > t.Health)
                    {
                        R.Cast();
                    }
                }
                if (Game.Time - lastR > 0.001*getSliderItem(rMenu, "delayR") && IsCastingR)
                {
                    Program.CastSpell(R, t);
                }
                Rtarget = R.GetPrediction(t).CastPosition;
            }
            else if (getCheckBoxItem(rMenu, "autoRlast") && Game.Time - lastR > 0.001*getSliderItem(rMenu, "delayR") &&
                     IsCastingR)
            {
                R.Cast(Rtarget);
            }
        }

        private static void LogicW()
        {
            var t = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            if (t.IsValidTarget())
            {
                var qDmg = Q.GetDamage(t);
                var wDmg = OktwCommon.GetKsDamage(t, W);

                if (wDmg > t.Health)
                {
                    Program.CastSpell(W, t);
                }
                else if (wDmg + qDmg > t.Health && Player.Mana > WMANA + QMANA)
                    Program.CastSpell(W, t);
                else if (Program.Combo && Player.Mana > RMANA + WMANA)
                    Program.CastSpell(W, t);
                else if (Program.Farm && OktwCommon.CanHarras() && getCheckBoxItem(wMenu, "harrasW") &&
                         getCheckBoxItem(harassMenu, "harras" + t.ChampionName) &&
                         Player.Mana > RMANA + WMANA + EMANA + QMANA + WMANA)
                    Program.CastSpell(W, t);
                else if (Program.Combo || Program.Farm)
                {
                    foreach (
                        var enemy in
                            Program.Enemies.Where(
                                enemy => enemy.IsValidTarget(W.Range) && !OktwCommon.CanMove(enemy)))
                        W.Cast(enemy, true);
                }
            }
            else if (Program.LaneClear && Player.ManaPercent > getSliderItem(farmMenu, "Mana") &&
                     getCheckBoxItem(farmMenu, "farmW") && Player.Mana > RMANA + WMANA)
            {
                var minionList = Cache.GetMinions(Player.ServerPosition, W.Range);
                var farmPosition = W.GetCircularFarmLocation(minionList, W.Width);

                if (farmPosition.MinionsHit > getSliderItem(farmMenu, "LCminions"))
                    W.Cast(farmPosition.Position);
            }
        }

        private static void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            var t2 = TargetSelector.GetTarget(1500, DamageType.Magical);

            if (t.IsValidTarget() && t2.IsValidTarget() && t == t2 &&
                !(getCheckBoxItem(farmMenu, "separate") && Program.LaneClear))
            {
                if (Q.IsCharging)
                {
                    Program.CastSpell(Q, t);
                    if (OktwCommon.GetPassiveTime(Player, "XerathArcanopulseChargeUp") < 1 || (Player.CountEnemiesInRange(800) > 0) || Player.Distance(t) > 1450)
                        Q.Cast(t);
                    else if (OktwCommon.GetPassiveTime(Player, "XerathArcanopulseChargeUp") < 2 || (Player.CountEnemiesInRange(1000) > 0))
                        Q.CastIfHitchanceEquals(t, HitChance.VeryHigh);
                }
                else if (t.IsValidTarget(Q.Range - 300))
                {
                    if (t.Health < OktwCommon.GetKsDamage(t, Q))
                        Q.StartCharging();
                    else if (Program.Combo && Player.Mana > EMANA + QMANA)
                    {
                        Q.StartCharging();
                    }
                    else if (t.IsValidTarget(1200) && Program.Farm && Player.Mana > RMANA + EMANA + QMANA + QMANA &&
                             getCheckBoxItem(harassMenu, "harras" + t.ChampionName) && !Player.UnderTurret(true) &&
                             OktwCommon.CanHarras())
                    {
                        Q.StartCharging();
                    }
                    else if ((Program.Combo || Program.Farm) && Player.Mana > RMANA + WMANA)
                    {
                        foreach (
                            var enemy in
                                Program.Enemies.Where(
                                    enemy => enemy.IsValidTarget(Q.Range) && !OktwCommon.CanMove(enemy)))
                            Q.StartCharging();
                    }
                }
            }
            else if (Program.LaneClear && Q.Range > 1000 && Player.CountEnemiesInRange(1450) == 0 &&
                     (Q.IsCharging ||
                      (Player.ManaPercent > getSliderItem(farmMenu, "Mana") && getCheckBoxItem(farmMenu, "farmQ") &&
                       Player.Mana > RMANA + QMANA + WMANA)))
            {
                var allMinionsQ = Cache.GetMinions(Player.ServerPosition, Q.Range);
                var Qfarm = Q.GetLineFarmLocation(allMinionsQ, Q.Width);
                if (Qfarm.MinionsHit > getSliderItem(farmMenu, "LCminions") ||
                    (Q.IsCharging && Qfarm.MinionsHit > 0))
                    Q.Cast(Qfarm.Position);
            }
        }

        private static void LogicE()
        {
            foreach (
                var enemy in
                    Program.Enemies.Where(
                        enemy =>
                            enemy.IsValidTarget(E.Range) &&
                            E.GetDamage(enemy) + OktwCommon.GetKsDamage(enemy, Q) + W.GetDamage(enemy) +
                            OktwCommon.GetEchoLudenDamage(enemy) > enemy.Health))
            {
                Program.CastSpell(E, enemy);
            }
            var t = Orbwalker.LastTarget as AIHeroClient;
            if (!t.IsValidTarget())
                t = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            if (t.IsValidTarget())
            {
                if (Program.Combo && Player.Mana > RMANA + EMANA)
                    Program.CastSpell(E, t);
                if (Program.Farm && OktwCommon.CanHarras() && getCheckBoxItem(eMenu, "harrasE") &&
                    Player.Mana > RMANA + EMANA + WMANA + EMANA)
                    Program.CastSpell(E, t);
                foreach (
                    var enemy in
                        Program.Enemies.Where(enemy => enemy.IsValidTarget(E.Range) && !OktwCommon.CanMove(enemy)))
                    E.Cast(enemy);
            }
        }

        private static void Jungle()
        {
            if (Program.LaneClear && Player.Mana > RMANA + WMANA + RMANA + WMANA)
            {
                var mobs = Cache.GetMinions(Player.ServerPosition, 600, MinionTeam.Neutral);
                if (mobs.Count > 0)
                {
                    var mob = mobs[0];
                    if (E.IsReady() && getCheckBoxItem(farmMenu, "jungleE"))
                    {
                        E.Cast(mob.ServerPosition);
                        return;
                    }
                    if (W.IsReady() && getCheckBoxItem(farmMenu, "jungleW"))
                    {
                        W.Cast(mob.ServerPosition);
                        return;
                    }
                    if (Q.IsReady() && getCheckBoxItem(farmMenu, "jungleQ"))
                    {
                        Q.Cast(mob.ServerPosition);
                    }
                }
            }
        }

        private static void SetMana()
        {
            if ((Program.getCheckBoxItem("manaDisable") && Program.Combo) || Player.HealthPercent < 20 || Q.IsCharging)
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
                RMANA = RMANA - (30 + Player.Level*3 + Player.Level);
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            if (getCheckBoxItem(drawMenu, "rRangeMini"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (R.IsReady())
                        Utility.DrawCircle(Player.Position, R.Range, Color.Aqua, 1, 20, true);
                }
                else
                    Utility.DrawCircle(Player.Position, R.Range, Color.Aqua, 1, 20, true);
            }
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
                var t = TargetSelector.GetTarget(R.Range, DamageType.Physical);

                if (t.IsValidTarget())
                {
                    var rDamage = R.GetDamage(t);
                    if (rDamage*3 > t.Health)
                    {
                        Drawing.DrawText(Drawing.Width*0.1f, Drawing.Height*0.5f, Color.Red,
                            "3 x Ult can kill: " + t.ChampionName + " have: " + t.Health + "hp");
                        drawLine(t.Position, Player.Position, 10, Color.Yellow);
                    }
                    else if (rDamage*2 > t.Health)
                    {
                        Drawing.DrawText(Drawing.Width*0.1f, Drawing.Height*0.5f, Color.Red,
                            "2 x Ult can kill: " + t.ChampionName + " have: " + t.Health + "hp");
                        drawLine(t.Position, Player.Position, 10, Color.Yellow);
                    }
                    else if (rDamage > t.Health)
                    {
                        Drawing.DrawText(Drawing.Width*0.1f, Drawing.Height*0.5f, Color.Red,
                            "1 x Ult can kill: " + t.ChampionName + " have: " + t.Health + "hp");
                        drawLine(t.Position, Player.Position, 10, Color.Yellow);
                    }
                }
            }
        }
    }
}