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
using HealthPrediction = SebbyLib.HealthPrediction;
using Orbwalking = SebbyLib.Orbwalking;
using Prediction = LeagueSharp.Common.Prediction;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

namespace OneKeyToWin_AIO_Sebby
{
    internal class Jinx
    {
        private static readonly Menu Config = Program.Config;
        public static Spell Q, W, E, R;
        public static float QMANA, WMANA, EMANA, RMANA;

        public static double lag = 0, WCastTime, QCastTime = 0, DragonTime, grabTime;
        public static float DragonDmg;

        public static Menu drawMenu, wMenu, qMenu, eMenu, rMenu, farmMenu;

        public static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        private static bool FishBoneActive
        {
            get { return Player.HasBuff("JinxQ"); }
        }

        public static void LoadOKTW()
        {
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 1500f);
            E = new Spell(SpellSlot.E, 920f);
            R = new Spell(SpellSlot.R, 3000f);

            W.SetSkillshot(0.6f, 60f, 3300f, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(1.2f, 100f, 1750f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.7f, 140f, 1500f, false, SkillshotType.SkillshotLine);

            LoadMenuOKTW();
            Game.OnUpdate += Game_OnUpdate;
            Orbwalker.OnPreAttack += Orbwalker_OnPreAttack;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Drawing.OnDraw += Drawing_OnDraw;
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

        private static void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (!Q.IsReady() || !getCheckBoxItem(qMenu, "autoQ") || !FishBoneActive)
                return;

            var t = args.Target as AIHeroClient;

            if (t != null)
            {
                var realDistance = GetRealDistance(t) - 50;
                if (Program.Combo &&
                    (realDistance < GetRealPowPowRange(t) ||
                     (Player.Mana < RMANA + 20 && Player.GetAutoAttackDamage(t)*3 < t.Health)))
                    Q.Cast();
                else if (Program.Farm && getCheckBoxItem(qMenu, "Qharras") &&
                         (realDistance > bonusRange() || realDistance < GetRealPowPowRange(t) ||
                          Player.Mana < RMANA + EMANA + WMANA + WMANA))
                    Q.Cast();
            }

            var minion = args.Target as Obj_AI_Minion;
            if (Program.Farm && minion != null)
            {
                var realDistance = GetRealDistance(minion);

                if (realDistance < GetRealPowPowRange(minion) || Player.ManaPercent < getSliderItem(farmMenu, "Mana"))
                {
                    Q.Cast();
                }
            }
        }

        private static void LoadMenuOKTW()
        {
            drawMenu = Config.AddSubMenu("Draw");
            drawMenu.Add("noti", new CheckBox("Show notification", false));
            drawMenu.Add("semi", new CheckBox("Semi-manual R target", false));
            drawMenu.Add("qRange", new CheckBox("Q range", false));
            drawMenu.Add("wRange", new CheckBox("W range", false));
            drawMenu.Add("eRange", new CheckBox("E range", false));
            drawMenu.Add("rRange", new CheckBox("R range", false));
            drawMenu.Add("onlyRdy", new CheckBox("Draw only ready spells"));

            wMenu = Config.AddSubMenu("W Config");
            wMenu.Add("autoW", new CheckBox("Auto W"));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                wMenu.Add("haras" + enemy.ChampionName, new CheckBox(enemy.ChampionName));

            qMenu = Config.AddSubMenu("Q Config");
            qMenu.Add("autoQ", new CheckBox("Auto Q"));
            qMenu.Add("Qharras", new CheckBox("Harass Q"));

            eMenu = Config.AddSubMenu("E Config");
            eMenu.Add("autoE", new CheckBox("Auto E on CC"));
            eMenu.Add("comboE", new CheckBox("Auto E in Combo BETA"));
            eMenu.Add("AGC", new CheckBox("AntiGapcloserE"));
            eMenu.Add("opsE", new CheckBox("OnProcessSpellCastE"));
            eMenu.Add("telE", new CheckBox("Auto E teleport"));

            rMenu = Config.AddSubMenu("R Config");
            rMenu.Add("autoR", new CheckBox("Auto R"));
            rMenu.Add("Rjungle", new CheckBox("R Jungle stealer"));
            rMenu.Add("Rdragon", new CheckBox("Dragon"));
            rMenu.Add("Rbaron", new CheckBox("Baron"));
            rMenu.Add("hitchanceR", new Slider("Hit Chance R", 2, 0, 3));
            rMenu.Add("useR", new KeyBind("OneKeyToCast R", false, KeyBind.BindTypes.HoldActive, 'T')); //32 == space
            rMenu.Add("Rturrent", new CheckBox("Don't R under turret"));

            farmMenu = Config.AddSubMenu("Farm");
            farmMenu.Add("farmQout", new CheckBox("Q farm out range AA"));
            farmMenu.Add("farmQ", new CheckBox("Q LaneClear Q"));
            farmMenu.Add("Mana", new Slider("LaneClear Q Mana", 80, 30));
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (getCheckBoxItem(eMenu, "AGC") && E.IsReady() && Player.Mana > RMANA + EMANA)
            {
                var Target = gapcloser.Sender;
                if (Target.IsValidTarget(E.Range))
                {
                    E.Cast(gapcloser.End);
                }
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs args)
        {
            if (unit.IsMinion)
                return;

            if (unit.IsMe)
            {
                if (args.SData.Name == "JinxWMissile")
                    WCastTime = Game.Time;
            }
            if (E.IsReady())
            {
                if (unit.IsEnemy && getCheckBoxItem(eMenu, "opsE") && unit.IsValidTarget(E.Range) &&
                    ShouldUseE(args.SData.Name))
                {
                    E.Cast(unit.ServerPosition, true);
                }
                if (unit.IsAlly && args.SData.Name == "RocketGrab" && Player.Distance(unit.Position) < E.Range)
                {
                    grabTime = Game.Time;
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (R.IsReady())
            {
                if (getKeyBindItem(rMenu, "useR"))
                {
                    var t = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                    if (t.IsValidTarget())
                        R.Cast(t, true, true);
                }
                if (getCheckBoxItem(rMenu, "Rjungle"))
                {
                    KsJungle();
                }
            }

            if (Program.LagFree(0))
            {
                SetMana();
            }

            if (E.IsReady())
                LogicE();

            if (Program.LagFree(2) && Q.IsReady() && getCheckBoxItem(qMenu, "autoQ"))
                LogicQ();

            if (Program.LagFree(3) && W.IsReady() && !Orbwalker.IsAutoAttacking && getCheckBoxItem(wMenu, "autoW"))
                LogicW();

            if (Program.LagFree(4) && R.IsReady())
                LogicR();
        }

        private static void LogicQ()
        {
            if (Program.Farm && !FishBoneActive && !Orbwalker.IsAutoAttacking && Orbwalker.LastTarget == null &&
                Orbwalking.CanAttack() && getCheckBoxItem(farmMenu, "farmQout") &&
                Player.Mana > RMANA + WMANA + EMANA + 10)
            {
                foreach (var minion in Cache.GetMinions(Player.Position, bonusRange() + 30).Where(
                    minion =>
                        !Orbwalking.InAutoAttackRange(minion) && GetRealPowPowRange(minion) < GetRealDistance(minion) &&
                        bonusRange() < GetRealDistance(minion)))
                {
                    var hpPred = HealthPrediction.GetHealthPrediction(minion, 200);
                    if (hpPred < Player.GetAutoAttackDamage(minion)*1.1 && hpPred > 5)
                    {
                        Orbwalker.ForcedTarget = minion;
                        Q.Cast();
                        return;
                    }
                }
            }

            var t = TargetSelector.GetTarget(bonusRange() + 60, DamageType.Physical);
            if (t.IsValidTarget())
            {
                if (!FishBoneActive && (!Orbwalking.InAutoAttackRange(t) || t.CountEnemiesInRange(250) > 2) &&
                    Orbwalker.LastTarget == null)
                {
                    var distance = GetRealDistance(t);
                    if (Program.Combo &&
                        (Player.Mana > RMANA + WMANA + 10 || Player.GetAutoAttackDamage(t)*3 > t.Health))
                        Q.Cast();
                    else if (Program.Farm && !Orbwalker.IsAutoAttacking && Orbwalking.CanAttack() &&
                             getCheckBoxItem(qMenu, "Qharras") && !ObjectManager.Player.UnderTurret(true) &&
                             Player.Mana > RMANA + WMANA + EMANA + 20 &&
                             distance < bonusRange() + t.BoundingRadius + Player.BoundingRadius)
                        Q.Cast();
                }
            }
            else if (!FishBoneActive && Program.Combo && Player.Mana > RMANA + WMANA + 20 &&
                     Player.CountEnemiesInRange(2000) > 0)
                Q.Cast();
            else if (FishBoneActive && Program.Combo && Player.Mana < RMANA + WMANA + 20)
                Q.Cast();
            else if (FishBoneActive && Program.Combo && Player.CountEnemiesInRange(2000) == 0)
                Q.Cast();
            else if (FishBoneActive &&
                     (Program.Farm || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit)))
            {
                Q.Cast();
            }
        }

        private static void LogicW()
        {
            var t = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            if (t.IsValidTarget())
            {
                foreach (
                    var enemy in
                        Program.Enemies.Where(
                            enemy => enemy.IsValidTarget(W.Range) && enemy.Distance(Player) > bonusRange()))
                {
                    var comboDmg = OktwCommon.GetKsDamage(enemy, W);
                    if (R.IsReady() && Player.Mana > RMANA + WMANA + 20)
                    {
                        comboDmg += R.GetDamage(enemy, 1);
                    }
                    if (comboDmg > enemy.Health && OktwCommon.ValidUlt(enemy))
                    {
                        Program.CastSpell(W, enemy);
                        return;
                    }
                }


                if (Player.CountEnemiesInRange(bonusRange()) == 0)
                {
                    if (Program.Combo && Player.Mana > RMANA + WMANA + 10)
                    {
                        foreach (
                            var enemy in
                                Program.Enemies.Where(
                                    enemy => enemy.IsValidTarget(W.Range) && GetRealDistance(enemy) > bonusRange())
                                    .OrderBy(enemy => enemy.Health))
                            Program.CastSpell(W, enemy);
                    }
                    else if (Program.Farm && Player.Mana > RMANA + EMANA + WMANA + WMANA + 40 && OktwCommon.CanHarras())
                    {
                        foreach (
                            var enemy in
                                Program.Enemies.Where(
                                    enemy =>
                                        enemy.IsValidTarget(W.Range) &&
                                        getCheckBoxItem(wMenu, "haras" + enemy.ChampionName)))
                            Program.CastSpell(W, enemy);
                    }
                }

                if (!Program.None && Player.Mana > RMANA + WMANA &&
                    Player.CountEnemiesInRange(GetRealPowPowRange(t)) == 0)
                {
                    foreach (
                        var enemy in
                            Program.Enemies.Where(enemy => enemy.IsValidTarget(W.Range) && !OktwCommon.CanMove(enemy)))
                        W.Cast(enemy, true);
                }
            }
        }

        private static void LogicE()
        {
            if (Player.Mana > RMANA + EMANA && getCheckBoxItem(eMenu, "autoE") && Game.Time - grabTime > 1)
            {
                foreach (
                    var enemy in
                        Program.Enemies.Where(enemy => enemy.IsValidTarget(E.Range) && !OktwCommon.CanMove(enemy)))
                {
                    E.Cast(enemy.Position);
                    return;
                }
                if (!Program.LagFree(1))
                    return;

                if (getCheckBoxItem(eMenu, "telE"))
                {
                    foreach (
                        var Object in
                            ObjectManager.Get<Obj_AI_Base>()
                                .Where(
                                    Obj =>
                                        Obj.IsEnemy && Obj.Distance(Player.ServerPosition) < E.Range &&
                                        (Obj.HasBuff("teleport_target") || Obj.HasBuff("Pantheon_GrandSkyfall_Jump"))))
                    {
                        E.Cast(Object.Position);
                    }
                }

                if (Program.Combo && Player.IsMoving && getCheckBoxItem(eMenu, "comboE") &&
                    Player.Mana > RMANA + EMANA + WMANA)
                {
                    var t = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                    if (t.IsValidTarget(E.Range) && E.GetPrediction(t).CastPosition.Distance(t.Position) > 200 &&
                        (int) E.GetPrediction(t).Hitchance == 5)
                    {
                        E.CastIfWillHit(t, 2);
                        if (t.HasBuffOfType(BuffType.Slow))
                        {
                            Program.CastSpell(E, t);
                        }
                        else
                        {
                            if (E.GetPrediction(t).CastPosition.Distance(t.Position) > 200)
                            {
                                if (Player.Position.Distance(t.ServerPosition) > Player.Position.Distance(t.Position))
                                {
                                    if (t.Position.Distance(Player.ServerPosition) <
                                        t.Position.Distance(Player.Position))
                                        Program.CastSpell(E, t);
                                }
                                else
                                {
                                    if (t.Position.Distance(Player.ServerPosition) >
                                        t.Position.Distance(Player.Position))
                                        Program.CastSpell(E, t);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void LogicR()
        {
            if (Player.UnderTurret(true) && getCheckBoxItem(rMenu, "Rturrent"))
                return;
            if (Game.Time - WCastTime > 0.9 && getCheckBoxItem(rMenu, "autoR"))
            {
                foreach (
                    var target in
                        Program.Enemies.Where(target => target.IsValidTarget(R.Range) && OktwCommon.ValidUlt(target)))
                {
                    var predictedHealth = target.Health - OktwCommon.GetIncomingDamage(target);
                    var Rdmg = R.GetDamage(target, 1);

                    if (Rdmg > predictedHealth && !OktwCommon.IsSpellHeroCollision(target, R) &&
                        GetRealDistance(target) > bonusRange() + 200)
                    {
                        if (GetRealDistance(target) > bonusRange() + 300 + target.BoundingRadius &&
                            target.CountAlliesInRange(600) == 0 && Player.CountEnemiesInRange(400) == 0)
                        {
                            castR(target);
                        }
                        else if (target.CountEnemiesInRange(200) > 2)
                        {
                            R.Cast(target, true, true);
                        }
                    }
                }
            }
        }

        private static void castR(AIHeroClient target)
        {
            var inx = getSliderItem(rMenu, "hitchanceR");
            if (inx == 0)
            {
                R.Cast(R.GetPrediction(target).CastPosition);
            }
            else if (inx == 1)
            {
                R.Cast(target);
            }
            else if (inx == 2)
            {
                Program.CastSpell(R, target);
            }
            else if (inx == 3)
            {
                var waypoints = target.GetWaypoints();
                if (Player.Distance(waypoints.Last().To3D()) - Player.Distance(target.Position) > 400)
                {
                    Program.CastSpell(R, target);
                }
            }
        }

        private static float bonusRange()
        {
            return 670f + Player.BoundingRadius + 25*Player.Spellbook.GetSpell(SpellSlot.Q).Level;
        }

        private static float GetRealPowPowRange(GameObject target)
        {
            return 650f + Player.BoundingRadius + target.BoundingRadius;
        }

        private static float GetRealDistance(Obj_AI_Base target)
        {
            return Player.ServerPosition.Distance(Prediction.GetPrediction(target, 0.05f).CastPosition) +
                   Player.BoundingRadius + target.BoundingRadius;
        }

        public static bool ShouldUseE(string SpellName)
        {
            switch (SpellName)
            {
                case "ThreshQ":
                    return true;
                case "KatarinaR":
                    return true;
                case "AlZaharNetherGrasp":
                    return true;
                case "GalioIdolOfDurand":
                    return true;
                case "LuxMaliceCannon":
                    return true;
                case "MissFortuneBulletTime":
                    return true;
                case "RocketGrabMissile":
                    return true;
                case "CaitlynPiltoverPeacemaker":
                    return true;
                case "EzrealTrueshotBarrage":
                    return true;
                case "InfiniteDuress":
                    return true;
                case "VelkozR":
                    return true;
            }
            return false;
        }

        private static void KsJungle()
        {
            var mobs = Cache.GetMinions(Player.ServerPosition, float.MaxValue, MinionTeam.Neutral);
            foreach (var mob in mobs)
            {
                //debug(mob.SkinName);
                if (mob.Health < mob.MaxHealth &&
                    ((mob.BaseSkinName == "SRU_Dragon" && getCheckBoxItem(rMenu, "Rdragon"))
                     || (mob.BaseSkinName == "SRU_Baron" && getCheckBoxItem(rMenu, "Rbaron")))
                    && mob.CountAlliesInRange(1000) == 0
                    && mob.Distance(Player.Position) > 1000)
                {
                    if (DragonDmg == 0)
                        DragonDmg = mob.Health;

                    if (Game.Time - DragonTime > 4)
                    {
                        if (DragonDmg - mob.Health > 0)
                        {
                            DragonDmg = mob.Health;
                        }
                        DragonTime = Game.Time;
                    }

                    else
                    {
                        var DmgSec = (DragonDmg - mob.Health)*(Math.Abs(DragonTime - Game.Time)/4);
                        //debug("DS  " + DmgSec);
                        if (DragonDmg - mob.Health > 0)
                        {
                            var timeTravel = GetUltTravelTime(Player, R.Speed, R.Delay, mob.Position);
                            var timeR = (mob.Health -
                                         Player.CalcDamage(mob, DamageType.Physical,
                                             250 + 100*R.Level + Player.FlatPhysicalDamageMod + 300))/(DmgSec/4);
                            if (timeTravel > timeR)
                                R.Cast(mob.Position);
                        }
                        else
                        {
                            DragonDmg = mob.Health;
                        }
                        //debug("" + GetUltTravelTime(Player, R.Speed, R.Delay, mob.Position));
                    }
                }
            }
        }

        private static float GetUltTravelTime(AIHeroClient source, float speed, float delay, Vector3 targetpos)
        {
            var distance = Vector3.Distance(source.ServerPosition, targetpos);
            var missilespeed = speed;
            if (source.ChampionName == "Jinx" && distance > 1350)
            {
                const float accelerationrate = 0.3f; //= (1500f - 1350f) / (2200 - speed), 1 unit = 0.3units/second
                var acceldifference = distance - 1350f;
                if (acceldifference > 150f) //it only accelerates 150 units
                    acceldifference = 150f;
                var difference = distance - 1500f;
                missilespeed = (1350f*speed + acceldifference*(speed + accelerationrate*acceldifference) +
                                difference*2200f)/distance;
            }
            return distance/missilespeed + delay;
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

            QMANA = 10;
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
            if (getCheckBoxItem(drawMenu, "onlyRdy"))
            {
                if (FishBoneActive)
                    Utility.DrawCircle(Player.Position, 590f + Player.BoundingRadius, Color.DeepPink, 1, 1);
                else
                    Utility.DrawCircle(Player.Position, bonusRange() - 40, Color.DeepPink, 1, 1);
            }
            if (getCheckBoxItem(drawMenu, "wRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (W.IsReady())
                        Utility.DrawCircle(Player.Position, W.Range, Color.Cyan, 1, 1);
                }
                else
                    Utility.DrawCircle(Player.Position, W.Range, Color.Cyan, 1, 1);
            }
            if (getCheckBoxItem(drawMenu, "eRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (E.IsReady())
                        Utility.DrawCircle(Player.Position, E.Range, Color.Gray, 1, 1);
                }
                else
                    Utility.DrawCircle(Player.Position, E.Range, Color.Gray, 1, 1);
            }
            if (getCheckBoxItem(drawMenu, "noti"))
            {
                var t = TargetSelector.GetTarget(R.Range, DamageType.Physical);

                if (R.IsReady() && t.IsValidTarget() && R.GetDamage(t, 1) > t.Health)
                {
                    Drawing.DrawText(Drawing.Width*0.1f, Drawing.Height*0.5f, Color.Red,
                        "Ult can kill: " + t.ChampionName + " have: " + t.Health + "hp");
                    drawLine(t.Position, Player.Position, 5, Color.Red);
                }
                else if (t.IsValidTarget(2000) && W.GetDamage(t) > t.Health)
                {
                    Drawing.DrawText(Drawing.Width*0.1f, Drawing.Height*0.5f, Color.Red,
                        "W can kill: " + t.ChampionName + " have: " + t.Health + "hp");
                    drawLine(t.Position, Player.Position, 3, Color.Yellow);
                }
            }
        }
    }
}