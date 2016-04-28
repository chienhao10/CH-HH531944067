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
using Prediction = LeagueSharp.Common.Prediction;
using PredictionInput = SebbyLib.Prediction.PredictionInput;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;

namespace OneKeyToWin_AIO_Sebby
{
    internal class Orianna
    {
        private static readonly Menu Config = Program.Config;
        private static Spell E, Q, R, W, QR;
        private static float QMANA, WMANA, EMANA, RMANA;

        private static Vector3 BallPos;
        private static bool Rsmart;
        private static AIHeroClient best;

        public static Menu drawMenu, eMenu, farmMenu, rMenu, wMenu;

        private static AIHeroClient Player
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
            Q = new Spell(SpellSlot.Q, 800);
            W = new Spell(SpellSlot.W, 210);
            E = new Spell(SpellSlot.E, 1095);
            R = new Spell(SpellSlot.R, 360);
            QR = new Spell(SpellSlot.Q, 825);

            Q.SetSkillshot(0.05f, 70f, 1150f, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(0.25f, 210f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.25f, 100f, 1700f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.5f, 360f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            QR.SetSkillshot(0.5f, 400f, 100f, false, SkillshotType.SkillshotCircle);

            drawMenu = Config.AddSubMenu("Draw");
            drawMenu.Add("qRange", new CheckBox("Q range", false));
            drawMenu.Add("wRange", new CheckBox("W range", false));
            drawMenu.Add("eRange", new CheckBox("E range", false));
            drawMenu.Add("rRange", new CheckBox("R range", false));
            drawMenu.Add("onlyRdy", new CheckBox("Draw only ready spells"));

            wMenu = Config.AddSubMenu("W Config");
            wMenu.Add("W", new CheckBox("Auto W SpeedUp logic", false));

            eMenu = Config.AddSubMenu("E Shield Config");
            eMenu.Add("autoW", new CheckBox("Auto E"));
            eMenu.Add("hadrCC", new CheckBox("Auto E hard CC"));
            eMenu.Add("poison", new CheckBox("Auto E poison"));
            eMenu.Add("Wdmg", new Slider("E dmg % hp", 10));
            eMenu.Add("AGC", new CheckBox("AntiGapcloserE"));

            farmMenu = Config.AddSubMenu("Farm");
            farmMenu.Add("farmQout", new CheckBox("Farm Q out range aa minion"));
            farmMenu.Add("Mana", new Slider("LaneClear Mana", 60));
            farmMenu.Add("LCminions", new Slider("LaneClear minimum minions", 2, 0, 10));
            farmMenu.Add("farmQ", new CheckBox("LaneClear Q"));
            farmMenu.Add("farmW", new CheckBox("LaneClear W"));
            farmMenu.Add("farmE", new CheckBox("LaneClear E", false));

            rMenu = Config.AddSubMenu("R Config");
            rMenu.Add("rCount", new Slider("Auto R x enemies", 3, 0, 5));
            rMenu.Add("smartR", new KeyBind("Semi-manual cast R key", false, KeyBind.BindTypes.HoldActive, 'T'));
            rMenu.Add("OPTI", new CheckBox("OnPossibleToInterrupt R"));
            rMenu.Add("Rturrent", new CheckBox("auto R under turrent"));
            rMenu.Add("Rks", new CheckBox("R ks"));
            rMenu.Add("Rlifesaver", new CheckBox("auto R life saver"));
            rMenu.Add("Rblock", new CheckBox("Block R if 0 hit "));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                rMenu.Add("Ralways" + enemy.ChampionName, new CheckBox("Always R : " + enemy.ChampionName, false));

            Game.OnUpdate += Game_OnGameUpdate;
            GameObject.OnCreate += Obj_AI_Base_OnCreate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!getCheckBoxItem(rMenu, "OPTI"))
                return;
            if (R.IsReady() && sender.Distance(BallPos) < R.Range)
            {
                R.Cast();
                Program.debug("interupt");
            }
            else if (Q.IsReady() && Player.Mana > RMANA + QMANA && sender.IsValidTarget(Q.Range))
                Q.Cast(sender.ServerPosition);
        }


        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.R && getCheckBoxItem(rMenu, "Rblock") &&
                CountEnemiesInRangeDeley(BallPos, R.Width, R.Delay) == 0)
                args.Process = false;
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var Target = gapcloser.Sender;
            if (getCheckBoxItem(eMenu, "AGC") && E.IsReady() && Target.IsValidTarget(800) && Player.Mana > RMANA + EMANA)
                E.CastOnUnit(Player);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            //Program.debug(""+BallPos.Distance(Player.Position));
            if (Player.HasBuff("Recall") || Player.IsDead)
                return;

            if (R.IsReady())
                LogicR();

            bool hadrCC = true;
            if (Program.LagFree(0))
            {
                SetMana();
                hadrCC = getCheckBoxItem(eMenu, "hadrCC");
            }

            best = Player;

            foreach (var ally in Program.Allies.Where(ally => ally.IsValid && !ally.IsDead))
            {
                if (ally.HasBuff("orianaghostself") || ally.HasBuff("orianaghost"))
                    BallPos = ally.ServerPosition;

                if (Program.LagFree(3))
                {
                    if (E.IsReady() && Player.Mana > RMANA + EMANA && ally.Distance(Player.Position) < E.Range)
                    {
                        var countEnemy = ally.CountEnemiesInRange(800);
                        if (ally.Health < countEnemy*ally.Level*25)
                        {
                            E.CastOnUnit(ally);
                        }
                        else if (HardCC(ally) && hadrCC && countEnemy > 0)
                        {
                            E.CastOnUnit(ally);
                        }
                        else if (ally.HasBuffOfType(BuffType.Poison))
                        {
                            E.CastOnUnit(ally);
                        }
                    }
                    if (W.IsReady() && Player.Mana > RMANA + WMANA && BallPos.Distance(ally.ServerPosition) < 240 &&
                        ally.Health < ally.CountEnemiesInRange(600)*ally.Level*20)
                        W.Cast();

                    if ((ally.Health < best.Health || ally.CountEnemiesInRange(300) > 0) &&
                        ally.Distance(Player.Position) < E.Range && ally.CountEnemiesInRange(700) > 0)
                        best = ally;
                }
                if (Program.LagFree(1) && E.IsReady() && Player.Mana > RMANA + EMANA &&
                    ally.Distance(Player.Position) < E.Range &&
                    ally.CountEnemiesInRange(R.Width) >= getSliderItem(rMenu, "rCount") &&
                    0 != getSliderItem(rMenu, "rCount"))
                {
                    E.CastOnUnit(ally);
                }
            }
            /*
            foreach (var ally in HeroManager.Allies.Where(ally => ally.IsValid && ally.Distance(Player.Position) < 1000))
            {
                foreach (var buff in ally.Buffs)
                {
                        Program.debug(buff.Name);
                }

            }
            */
            if ((getKeyBindItem(rMenu, "smartR") || Rsmart) && R.IsReady())
            {
                Rsmart = true;
                var target = TargetSelector.GetTarget(Q.Range + 100, DamageType.Magical);
                if (target.IsValidTarget())
                {
                    if (CountEnemiesInRangeDeley(BallPos, R.Width, R.Delay) > 1)
                        R.Cast();
                    else if (Q.IsReady())
                        QR.Cast(target, true, true);
                    else if (CountEnemiesInRangeDeley(BallPos, R.Width, R.Delay) > 0)
                        R.Cast();
                }
                else
                    Rsmart = false;
            }
            else
                Rsmart = false;

            if (Program.LagFree(1))
            {
                LogicQ();
                LogicFarm();
            }

            if (Program.LagFree(2) && W.IsReady())
                LogicW();

            if (Program.LagFree(4) && E.IsReady())
                LogicE(best);
        }

        private static void LogicE(AIHeroClient best)
        {
            var ta = TargetSelector.GetTarget(1300, DamageType.Magical);

            if (Program.Combo && ta.IsValidTarget() && !W.IsReady() && Player.Mana > RMANA + EMANA)
            {
                if (CountEnemiesInRangeDeley(BallPos, 100, 0.1f) > 0)
                    E.CastOnUnit(best);
                var castArea = ta.Distance(best.ServerPosition)*(best.ServerPosition - ta.ServerPosition).Normalized() +
                               ta.ServerPosition;
                if (castArea.Distance(ta.ServerPosition) < ta.BoundingRadius/2)
                    E.CastOnUnit(best);
            }
        }

        private static void LogicR()
        {
            foreach (
                var t in
                    Program.Enemies.Where(
                        t =>
                            t.IsValidTarget() &&
                            BallPos.Distance(Prediction.GetPrediction(t, R.Delay).CastPosition) < R.Width &&
                            BallPos.Distance(t.ServerPosition) < R.Width))
            {
                if (Program.Combo && getCheckBoxItem(rMenu, "Ralways" + t.ChampionName))
                {
                    R.Cast();
                }

                if (getCheckBoxItem(rMenu, "Rks"))
                {
                    var comboDmg = OktwCommon.GetKsDamage(t, R);

                    if (t.IsValidTarget(Q.Range))
                        comboDmg += Q.GetDamage(t);
                    if (W.IsReady())
                        comboDmg += W.GetDamage(t);
                    if (Player.IsInAutoAttackRange(t))
                        comboDmg += Player.GetAutoAttackDamage(t)*2;
                    if (t.Health < comboDmg)
                        R.Cast();
                    Program.debug("ks");
                }
                if (getCheckBoxItem(rMenu, "Rturrent") && BallPos.UnderTurret(false) && !BallPos.UnderTurret(true))
                {
                    R.Cast();
                    Program.debug("Rturrent");
                }
                if (getCheckBoxItem(rMenu, "Rlifesaver") &&
                    Player.Health < Player.CountEnemiesInRange(800)*Player.Level*20 &&
                    Player.Distance(BallPos) > t.Distance(Player.Position))
                {
                    R.Cast();
                    Program.debug("ls");
                }
            }

            var countEnemies = CountEnemiesInRangeDeley(BallPos, R.Width, R.Delay);

            if (countEnemies >= getSliderItem(rMenu, "rCount") && BallPos.CountEnemiesInRange(R.Width) == countEnemies)
                R.Cast();
        }

        private static void LogicW()
        {
            if (Program.Enemies.Any(t => t.IsValidTarget() && BallPos.Distance(t.ServerPosition) < 250 && t.Health < W.GetDamage(t)))
            {
                W.Cast();
                return;
            }
            if (CountEnemiesInRangeDeley(BallPos, W.Width, 0f) > 0 && Player.Mana > RMANA + WMANA)
            {
                W.Cast();
                return;
            }
            if (getCheckBoxItem(wMenu, "W") && !Program.Farm && !Program.Combo &&
                ObjectManager.Player.Mana > Player.MaxMana*0.95 && Player.HasBuff("orianaghostself"))
                W.Cast();
        }

        private static void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (t.IsValidTarget() && Q.IsReady())
            {
                if (Q.GetDamage(t) + W.GetDamage(t) > t.Health)
                    CastQ(t);
                else if (Program.Combo && Player.Mana > RMANA + QMANA - 10)
                    CastQ(t);
                else if (Program.Farm && Player.Mana > RMANA + QMANA + WMANA + EMANA)
                    CastQ(t);
            }
            if (getCheckBoxItem(wMenu, "W") && !t.IsValidTarget() && Program.Combo &&
                Player.Mana > RMANA + 3*QMANA + WMANA + EMANA + WMANA)
            {
                if (W.IsReady() && Player.HasBuff("orianaghostself"))
                {
                    W.Cast();
                }
                else if (E.IsReady() && !Player.HasBuff("orianaghostself"))
                {
                    E.CastOnUnit(Player);
                }
            }
        }

        private static void LogicFarm()
        {
            if (!Program.Farm)
                return;

            var allMinions = Cache.GetMinions(Player.ServerPosition, Q.Range);
            if (getCheckBoxItem(farmMenu, "farmQout") && Player.Mana > RMANA + QMANA + WMANA + EMANA)
            {
                foreach (
                    var minion in
                        allMinions.Where(
                            minion =>
                                minion.IsValidTarget(Q.Range) && !Player.IsInAutoAttackRange(minion) &&
                                minion.Health < Q.GetDamage(minion) && minion.Health > minion.FlatPhysicalDamageMod))
                {
                    Q.Cast(minion);
                }
            }

            if (!Program.LaneClear || Player.Mana < RMANA + QMANA)
                return;

            var mobs = Cache.GetMinions(Player.ServerPosition, 800, MinionTeam.Neutral);
            if (mobs.Count > 0)
            {
                var mob = mobs[0];
                if (Q.IsReady())
                    Q.Cast(mob.Position);
                if (W.IsReady() && BallPos.Distance(mob.Position) < W.Width)
                    W.Cast();
                else if (E.IsReady())
                    E.CastOnUnit(best);
                return;
            }


            if (Player.ManaPercent > getSliderItem(farmMenu, "Mana") ||
                (Player.UnderTurret(false) && !Player.UnderTurret(true)))
            {
                var Qfarm = Q.GetCircularFarmLocation(allMinions, 100);
                var QWfarm = Q.GetCircularFarmLocation(allMinions, W.Width);

                if (Qfarm.MinionsHit + QWfarm.MinionsHit == 0)
                    return;
                if (getCheckBoxItem(farmMenu, "farmQ"))
                {
                    if (Qfarm.MinionsHit > getSliderItem(farmMenu, "LCminions") && !W.IsReady() && Q.IsReady())
                    {
                        Q.Cast(Qfarm.Position);
                    }
                    else if (QWfarm.MinionsHit > 2 && Q.IsReady())
                        Q.Cast(QWfarm.Position);
                }

                foreach (var minion in allMinions)
                {
                    if (W.IsReady() && minion.Distance(BallPos) < W.Range && minion.Health < W.GetDamage(minion) &&
                        getCheckBoxItem(farmMenu, "farmW"))
                        W.Cast();
                    if (!W.IsReady() && E.IsReady() && minion.Distance(BallPos) < E.Width &&
                        getCheckBoxItem(farmMenu, "farmE"))
                        E.CastOnUnit(Player);
                }
            }
        }

        private static void CastQ(AIHeroClient target)
        {
            var distance = Vector3.Distance(BallPos, target.ServerPosition);


            if (E.IsReady() && Player.Mana > RMANA + QMANA + WMANA + EMANA &&
                distance > Player.Distance(target.ServerPosition) + 300)
            {
                E.CastOnUnit(Player);
                return;
            }

            if (Program.getSliderItem("PredictionMODE") == 1)
            {
                var predInput2 = new PredictionInput
                {
                    Aoe = true,
                    Collision = Q.Collision,
                    Speed = Q.Speed,
                    Delay = Q.Delay,
                    Range = float.MaxValue,
                    From = BallPos,
                    Radius = Q.Width,
                    Unit = target,
                    Type = SebbyLib.Prediction.SkillshotType.SkillshotCircle
                };
                var prepos5 = SebbyLib.Prediction.Prediction.GetPrediction(predInput2);

                if ((int) prepos5.Hitchance > 5 - Program.getSliderItem("HitChance"))
                {
                    if (prepos5.CastPosition.Distance(prepos5.CastPosition) < Q.Range)
                    {
                        Q.Cast(prepos5.CastPosition);
                    }
                }
            }
            else
            {
                var delay = distance/Q.Speed + Q.Delay;
                var prepos = Prediction.GetPrediction(target, delay, Q.Width);

                if ((int) prepos.Hitchance > 5 - Program.getSliderItem("HitChance"))
                {
                    if (prepos.CastPosition.Distance(prepos.CastPosition) < Q.Range)
                    {
                        Q.Cast(prepos.CastPosition);
                    }
                }
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "OrianaIzunaCommand")
                BallPos = args.End;

            if (!E.IsReady() || !sender.IsEnemy || !getCheckBoxItem(eMenu, "autoW") || Player.Mana < EMANA + RMANA ||
                sender.Distance(Player.Position) > 1600)
                return;

            foreach (
                var ally in
                    Program.Allies.Where(
                        ally => ally.IsValid && !ally.IsDead && Player.Distance(ally.ServerPosition) < E.Range))
            {
                double dmg = 0;
                if (args.Target != null && args.Target.NetworkId == ally.NetworkId)
                {
                    dmg = dmg + sender.LSGetSpellDamage(ally, args.SData.Name);
                }
                else
                {
                    var castArea = ally.Distance(args.End)*(args.End - ally.ServerPosition).Normalized() +
                                   ally.ServerPosition;
                    if (castArea.Distance(ally.ServerPosition) < ally.BoundingRadius/2)
                        dmg = dmg + sender.LSGetSpellDamage(ally, args.SData.Name);
                    else
                        continue;
                }

                var HpPercentage = dmg*100/ally.Health;
                var shieldValue = 60 + E.Level*40 + 0.4*Player.FlatMagicDamageMod;

                if (HpPercentage >= getSliderItem(eMenu, "Wdmg") || dmg > shieldValue)
                    E.CastOnUnit(ally);
            }
        }

        private static int CountEnemiesInRangeDeley(Vector3 position, float range, float delay)
        {
            return Program.Enemies.Where(t => t.IsValidTarget()).Select(t => Prediction.GetPrediction(t, delay).CastPosition).Count(prepos => position.Distance(prepos) < range);
        }

        private static void Obj_AI_Base_OnCreate(GameObject obj, EventArgs args)
        {
            if (obj.IsValid && obj.IsAlly && obj.Name == "TheDoomBall")
            {
                BallPos = obj.Position;
            }
        }

        private static bool HardCC(AIHeroClient target)
        {
            if (target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare) ||
                target.HasBuffOfType(BuffType.Knockup) ||
                target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Fear) ||
                target.HasBuffOfType(BuffType.Knockback) ||
                target.HasBuffOfType(BuffType.Taunt) || target.HasBuffOfType(BuffType.Suppression) ||
                target.IsStunned)
            {
                return true;
            }
            return false;
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
            if (BallPos.IsValid())
            {
                if (getCheckBoxItem(drawMenu, "wRange"))
                {
                    if (getCheckBoxItem(drawMenu, "onlyRdy"))
                    {
                        if (W.IsReady())
                            Utility.DrawCircle(BallPos, W.Range, Color.Orange, 1, 1);
                    }
                    else
                        Utility.DrawCircle(BallPos, W.Range, Color.Orange, 1, 1);
                }

                if (getCheckBoxItem(drawMenu, "rRange"))
                {
                    if (getCheckBoxItem(drawMenu, "onlyRdy"))
                    {
                        if (R.IsReady())
                            Utility.DrawCircle(BallPos, R.Range, Color.Gray, 1, 1);
                    }
                    else
                        Utility.DrawCircle(BallPos, R.Range, Color.Gray, 1, 1);
                }
            }

            if (getCheckBoxItem(drawMenu, "qRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (Q.IsReady())
                        Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Cyan, 1, 1);
                }
                else
                    Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Cyan, 1, 1);
            }

            if (getCheckBoxItem(drawMenu, "eRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (E.IsReady())
                        Utility.DrawCircle(ObjectManager.Player.Position, E.Range, Color.Yellow, 1, 1);
                }
                else
                    Utility.DrawCircle(ObjectManager.Player.Position, E.Range, Color.Yellow, 1, 1);
            }
        }
    }
}