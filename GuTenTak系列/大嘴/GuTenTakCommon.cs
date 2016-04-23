using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using System.Collections.Generic;
using System.Linq;
using System;

namespace GuTenTak.KogMaw
{
    internal class Common : Program
    {
        //public static object HeroManager { get; private set; }
        private static bool cz = false;
        private static float czx = 0, czy = 0, czx2 = 0, czy2 = 0;

        //private static bool IsZombie = PlayerInstance.HasBuff("kogmawicathiansurprise");
        //private static bool wActive = PlayerInstance.HasBuff("kogmawbioarcanebarrage");

        public static Obj_AI_Base GetFindObj(Vector3 Pos, string name, float range)
        {
            var CusPos = Pos;
            {
                var GetObj = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(f => f.IsAlly && !f.IsMe && f.Position.Distance(ObjectManager.Player.Position) < range && f.Distance(CusPos) < 150);
                if (GetObj != null)
                    return GetObj;
                return null;
            }
        }

        public static void MovingPlayer(Vector3 Pos)
        {
            Player.IssueOrder(GameObjectOrder.MoveTo, Pos);
        }
        public static Vector2 ToScreen(Vector3 Target)
        {
            var target = Drawing.WorldToScreen(Target);
            return target;
        }

        public static void Combo()
        {
            var RTarget = TargetSelector.GetTarget(R.Range, DamageType.Mixed);
            if (RTarget == null) return;
            {
                var useR = ModesMenu1["ComboR"].Cast<CheckBox>().CurrentValue;
                var Rp = R.GetPrediction(RTarget);
                if (!RTarget.IsValid()) return;
                if (R.IsInRange(RTarget) && R.IsReady() && useR && Rp.HitChance >= HitChance.High && !RTarget.IsInvulnerable)
                {
                    if (Player.Instance.GetBuffCount("kogmawlivingartillerycost") < ModesMenu1["CRStack"].Cast<Slider>().CurrentValue && ObjectManager.Player.ManaPercent >= Program.ModesMenu1["ManaCR"].Cast<Slider>().CurrentValue)
                    {
                        if (ModesMenu1["LogicRn"].Cast<ComboBox>().CurrentValue == 0)
                        {
                            R.Cast(Rp.CastPosition);
                        }
                        if (RTarget.HealthPercent <= 55 && ModesMenu1["LogicRn"].Cast<ComboBox>().CurrentValue == 1)
                        {
                            R.Cast(Rp.CastPosition);
                        }
                        if (RTarget.HealthPercent <= 30 && ModesMenu1["LogicRn"].Cast<ComboBox>().CurrentValue == 2)
                        {
                            R.Cast(Rp.CastPosition);
                        }

                    }
                }

                var useW = ModesMenu1["ComboW"].Cast<CheckBox>().CurrentValue;
                if (useW && W.IsReady())
                {
                    if (EntityManager.Heroes.Enemies.Any(x => x.IsValidTarget(W.Range)))
                    {
                        W.Cast();
                    }

                }

                var QTarget = TargetSelector.GetTarget(Q.Range, DamageType.Mixed);
                if (QTarget == null) return;
                {
                    var useQ = ModesMenu1["ComboQ"].Cast<CheckBox>().CurrentValue;
                    var Qp = Q.GetPrediction(QTarget);
                    if (!QTarget.IsValid()) return;
                    if (Q.IsInRange(QTarget) && Q.IsReady() && useQ && Qp.HitChance >= HitChance.High && !QTarget.IsInvulnerable)
                    {
                       if (ObjectManager.Player.Mana >= 80)
                        {
                          Q.Cast(Qp.CastPosition);
                        }
                    }
                }
                var ETarget = TargetSelector.GetTarget(E.Range, DamageType.Mixed);
                if (ETarget == null) return;
                {
                    var useE = ModesMenu1["ComboE"].Cast<CheckBox>().CurrentValue;
                    var Ep = E.GetPrediction(ETarget);
                    if (E.IsInRange(ETarget) && E.IsReady() && useE && ObjectManager.Player.ManaPercent >= Program.ModesMenu1["ManaCE"].Cast<Slider>().CurrentValue && ObjectManager.Player.IsInAutoAttackRange(ETarget))
                    {
                        E.Cast(Ep.CastPosition);
                    }
                }
            }
        }

        internal static void zigzag(EventArgs args)
        {
            var zigzagTarget = TargetSelector.GetTarget(R.Range, DamageType.Mixed);
            if (zigzagTarget == null)
            {
                czx = 0;
                czx2 = 0;
                czy = 0;
                czy2 = 0;
                return;
            }

            if (czx < czx2)
            {
                if (czx2 >= zigzagTarget.Position.X)
                    cz = true;
                else
                    cz = false;
            }
            else if (czx == czx2)
            {
                cz = false;
                czx = czx2;
                czx2 = zigzagTarget.Position.X;
                return;
            }
            else
            {
                if (czx2 <= zigzagTarget.Position.X)
                    cz = true;
                else
                    cz = false;
            }
            czx = czx2;
            czx2 = zigzagTarget.Position.X;

            if (czy < czy2)
            {
                if (czy2 >= zigzagTarget.Position.Y)
                    cz = true;
                else
                    cz = false;
            }
            else if (czy == czy2)
                cz = false;
            else
            {
                if (czy2 <= zigzagTarget.Position.Y)
                    cz = true;
                else
                    cz = false;
            }
            czy = czy2;
            czy2 = zigzagTarget.Position.Y;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && ModesMenu1["ComboR"].Cast<CheckBox>().CurrentValue)
            {
                var zigzag = R.GetPrediction(zigzagTarget);
                if (zigzag.HitChance >= HitChance.High && !zigzagTarget.IsInvulnerable && Player.Instance.GetBuffCount("kogmawlivingartillerycost") < ModesMenu1["CRStack"].Cast<Slider>().CurrentValue 
                    && ObjectManager.Player.ManaPercent >= Program.ModesMenu1["ManaCR"].Cast<Slider>().CurrentValue)
                {
                        if (ModesMenu1["LogicRn"].Cast<ComboBox>().CurrentValue == 0)
                        {
                            R.Cast(zigzag.CastPosition);
                        }
                        if (zigzagTarget.HealthPercent <= 50 && ModesMenu1["LogicRn"].Cast<ComboBox>().CurrentValue == 1)
                        {
                            R.Cast(zigzag.CastPosition);
                        }
                        if (zigzagTarget.HealthPercent <= 25 && ModesMenu1["LogicRn"].Cast<ComboBox>().CurrentValue == 2)
                        {
                            R.Cast(zigzag.CastPosition);
                        }

                }
            }

        }

        public static void Harass()
        {
            //Harass
            var RTarget = TargetSelector.GetTarget(R.Range, DamageType.Mixed);
            if (RTarget == null) return;
            {
                var useR = ModesMenu1["HarassR"].Cast<CheckBox>().CurrentValue;
                var Rp = R.GetPrediction(RTarget);
                if (!RTarget.IsValid()) return;
                if (R.IsInRange(RTarget) && R.IsReady() && useR && !RTarget.IsInvulnerable && Rp.HitChance >= HitChance.High && ObjectManager.Player.ManaPercent >= Program.ModesMenu1["ManaHR"].Cast<Slider>().CurrentValue)
                {
                    if (Player.Instance.GetBuffCount("kogmawlivingartillerycost") < ModesMenu1["HRStack"].Cast<Slider>().CurrentValue)
                    {
                        R.Cast(Rp.CastPosition);
                    }
                }
            }

            var Target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (Target == null) return;
            {
                var useQ = ModesMenu1["HarassQ"].Cast<CheckBox>().CurrentValue;
                var Qp = Q.GetPrediction(Target);
                if (!Target.IsValid() && Target == null) return;
                if (Q.IsInRange(Target) && Q.IsReady() && useQ && Qp.HitChance >= HitChance.High && ObjectManager.Player.ManaPercent >= Program.ModesMenu1["ManaHQ"].Cast<Slider>().CurrentValue)
                {
                    Q.Cast(Qp.CastPosition);
                }
            }
        }
        public static void LastHit()
        {

        }
        public static void LaneClear()
        {
            if (Q.IsReady() && ModesMenu2["FarmQ"].Cast<CheckBox>().CurrentValue && ObjectManager.Player.ManaPercent >= Program.ModesMenu2["ManaL"].Cast<Slider>().CurrentValue)
            {
                var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t.IsEnemy && !t.IsDead && t.IsValid && !t.IsInvulnerable && t.IsInRange(Player.Instance.Position, Q.Range));
                foreach (var m in minions)
                {
                    if (Q.GetPrediction(m).CollisionObjects.Where(t => t.IsEnemy && !t.IsDead && t.IsValid && !t.IsInvulnerable).Count() >= 0)
                    {
                        Q.Cast(m);
                        break;
                    }
                }
            }
            if (R.IsReady() && ModesMenu2["FarmR"].Cast<CheckBox>().CurrentValue && ObjectManager.Player.ManaPercent >= Program.ModesMenu2["ManaLR"].Cast<Slider>().CurrentValue)
            {
                var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t.IsEnemy && !t.IsDead && t.IsValid && !t.IsInvulnerable && t.IsInRange(Player.Instance.Position, Q.Range)).OrderBy(t => t.Health);
                if (minions.Count() > 2)
                {
                    if (Player.Instance.GetBuffCount("kogmawlivingartillerycost") < ModesMenu2["FRStack"].Cast<Slider>().CurrentValue)
                    {
                        R.Cast(minions.First());
                    }
                   
                }
            }
        }
        public static void JungleClear()
        {
            if (Q.IsReady() && ModesMenu2["JungleQ"].Cast<CheckBox>().CurrentValue && ObjectManager.Player.ManaPercent >= Program.ModesMenu2["ManaJ"].Cast<Slider>().CurrentValue)
            {
                var minions = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, Q.Range).Where(t => !t.IsDead && t.IsValid && !t.IsInvulnerable);
                foreach (var m in minions)
                {
                        if (Q.GetPrediction(m).CollisionObjects.Where(t => t.IsEnemy && !t.IsDead && t.IsValid && !t.IsInvulnerable).Count() >= minions.Count() - 1)
                        {
                            Q.Cast(m);
                            break;
                        }
                }
            }
            if (R.IsReady() && ModesMenu2["JungleR"].Cast<CheckBox>().CurrentValue && ObjectManager.Player.ManaPercent >= Program.ModesMenu2["ManaJR"].Cast<Slider>().CurrentValue)
            {
                var minions = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, Q.Range).Where(t => !t.IsDead && t.IsValid && !t.IsInvulnerable);
                if (minions.Count() > 0)
                {
                    if (Player.Instance.GetBuffCount("kogmawlivingartillerycost") < ModesMenu2["JRStack"].Cast<Slider>().CurrentValue)
                    {
                        R.Cast(minions.First());
                    }
                }
            }
        }

        public static void Flee()
        {
            if (ModesMenu3["FleeE"].Cast<CheckBox>().CurrentValue)
            {
                var ETarget = TargetSelector.GetTarget(E.Range, DamageType.Mixed);
                if (ETarget == null) return;
                {
                    var Ep = E.GetPrediction(ETarget);
                    if (E.IsInRange(ETarget) && E.IsReady() && ObjectManager.Player.ManaPercent >= Program.ModesMenu3["ManaFlR"].Cast<Slider>().CurrentValue)
                    {
                        E.Cast(Ep.CastPosition);
                    }
                }
            }
            if (ModesMenu3["FleeR"].Cast<CheckBox>().CurrentValue && ObjectManager.Player.ManaPercent <= Program.ModesMenu3["ManaFlR"].Cast<Slider>().CurrentValue)
            {
                if (ObjectManager.Player.CountEnemiesInRange(400) == 0)
                {
                    var Target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                    if (Target == null) return;
                    var Rp = R.GetPrediction(Target);
                    if (Player.Instance.GetBuffCount("kogmawlivingartillerycost") < ModesMenu3["FlRStack"].Cast<Slider>().CurrentValue)
                    {
                        R.Cast(Rp.CastPosition);
                    }
                }

            }
        }


        internal static void OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (!sender.IsMe || ModesMenu3["Qssmode"].Cast<ComboBox>().CurrentValue == 1 && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) return;
            var type = args.Buff.Type;
            //var duration = args.Buff.EndTime - Game.Time;
            var Name = args.Buff.Name.ToLower();

            /*if (ModesMenu3["Qssmode"].Cast<ComboBox>().CurrentValue == 0)
            {*/
                if (type == BuffType.Taunt && ModesMenu3["Taunt"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Stun && ModesMenu3["Stun"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Snare && ModesMenu3["Snare"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Polymorph && ModesMenu3["Polymorph"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Blind && ModesMenu3["Blind"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Flee && ModesMenu3["Fear"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Charm && ModesMenu3["Charm"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Suppression && ModesMenu3["Suppression"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Silence && ModesMenu3["Silence"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (Name == "zedrdeathmark" && ModesMenu3["ZedUlt"].Cast<CheckBox>().CurrentValue)
                {
                    UltQSS();
                }
                if (Name == "vladimirhemoplague" && ModesMenu3["VladUlt"].Cast<CheckBox>().CurrentValue)
                {
                    UltQSS();
                }
                if (Name == "fizzmarinerdoom" && ModesMenu3["FizzUlt"].Cast<CheckBox>().CurrentValue)
                {
                    UltQSS();
                }
                if (Name == "mordekaiserchildrenofthegrave" && ModesMenu3["MordUlt"].Cast<CheckBox>().CurrentValue)
                {
                    UltQSS();
                }
                if (Name == "poppydiplomaticimmunity" && ModesMenu3["PoppyUlt"].Cast<CheckBox>().CurrentValue)
                {
                    UltQSS();
                }/*
            }
            if (ModesMenu3["Qssmode"].Cast<ComboBox>().CurrentValue == 1 && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (type == BuffType.Taunt && ModesMenu3["Taunt"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Stun && ModesMenu3["Stun"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Snare && ModesMenu3["Snare"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Polymorph && ModesMenu3["Polymorph"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Blind && ModesMenu3["Blind"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Flee && ModesMenu3["Fear"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Charm && ModesMenu3["Charm"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Suppression && ModesMenu3["Suppression"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Silence && ModesMenu3["Silence"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (Name == "zedrdeathmark" && ModesMenu3["ZedUlt"].Cast<CheckBox>().CurrentValue)
                {
                    UltQSS();
                }
                if (Name == "vladimirhemoplague" && ModesMenu3["VladUlt"].Cast<CheckBox>().CurrentValue)
                {
                    UltQSS();
                }
                if (Name == "fizzmarinerdoom" && ModesMenu3["FizzUlt"].Cast<CheckBox>().CurrentValue)
                {
                    UltQSS();
                }
                if (Name == "mordekaiserchildrenofthegrave" && ModesMenu3["MordUlt"].Cast<CheckBox>().CurrentValue)
                {
                    UltQSS();
                }
                if (Name == "poppydiplomaticimmunity" && ModesMenu3["PoppyUlt"].Cast<CheckBox>().CurrentValue)
                {
                    UltQSS();
                }
            }*/
        }


        internal static void ItemUsage()
        {
            var target = TargetSelector.GetTarget(550, DamageType.Physical); // 550 = Botrk.Range
            var hextech = TargetSelector.GetTarget(700, DamageType.Magical); // 700 = hextech.Range

            if (ModesMenu3["useYoumuu"].Cast<CheckBox>().CurrentValue && Program.Youmuu.IsOwned() && Program.Youmuu.IsReady())
            {
                Program.Youmuu.Cast();
            }
            if (hextech != null)
            {
                if (ModesMenu3["usehextech"].Cast<CheckBox>().CurrentValue && Item.HasItem(Program.Cutlass.Id) && Item.CanUseItem(Program.Cutlass.Id))
                {
                    Item.UseItem(Program.hextech.Id, hextech);
                }
            }
            if (target != null)
            {
                if (ModesMenu3["useBotrk"].Cast<CheckBox>().CurrentValue && Item.HasItem(Program.Cutlass.Id) && Item.CanUseItem(Program.Cutlass.Id) &&
                    Player.Instance.HealthPercent < ModesMenu3["minHPBotrk"].Cast<Slider>().CurrentValue &&
                    target.HealthPercent < ModesMenu3["enemyMinHPBotrk"].Cast<Slider>().CurrentValue)
                {
                    Item.UseItem(Program.Cutlass.Id, target);
                }
                if (ModesMenu3["useBotrk"].Cast<CheckBox>().CurrentValue && Item.HasItem(Program.Botrk.Id) && Item.CanUseItem(Program.Botrk.Id) &&
                    Player.Instance.HealthPercent < ModesMenu3["minHPBotrk"].Cast<Slider>().CurrentValue &&
                    target.HealthPercent < ModesMenu3["enemyMinHPBotrk"].Cast<Slider>().CurrentValue)
                {
                    Program.Botrk.Cast(target);
                }
            }
        }

        internal static void DoQSS()
        {
            if (ModesMenu3["useQss"].Cast<CheckBox>().CurrentValue && Qss.IsOwned() && Qss.IsReady() && ObjectManager.Player.CountEnemiesInRange(1800) > 0)
            {
                Core.DelayAction(() => Qss.Cast(), ModesMenu3["QssDelay"].Cast<Slider>().CurrentValue);
            }
            if (Simitar.IsOwned() && Simitar.IsReady() && ObjectManager.Player.CountEnemiesInRange(1800) > 0)
            {
                Core.DelayAction(() => Simitar.Cast(), ModesMenu3["QssDelay"].Cast<Slider>().CurrentValue);
            }
        }

        private static void UltQSS()
        {
            if (ModesMenu3["useQss"].Cast<CheckBox>().CurrentValue && Qss.IsOwned() && Qss.IsReady())
            {
                Core.DelayAction(() => Qss.Cast(), ModesMenu3["QssUltDelay"].Cast<Slider>().CurrentValue);
            }
            if (Simitar.IsOwned() && Simitar.IsReady())
            {
                Core.DelayAction(() => Simitar.Cast(), ModesMenu3["QssUltDelay"].Cast<Slider>().CurrentValue);
            }
        }

        public static void Skinhack()
        {
            if (ModesMenu3["skinhack"].Cast<CheckBox>().CurrentValue)
            {
                Player.SetSkinId((int)ModesMenu3["skinId"].Cast<ComboBox>().CurrentValue);
            }
        }
        internal static void Gapcloser_OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs gapcloser)
        {
        }

        public static void KillSteal()
        {
            if (Program.ModesMenu1["KS"].Cast<CheckBox>().CurrentValue)
            {
                foreach (var enemy in EntityManager.Heroes.Enemies.Where(a => !a.IsDead && !a.IsZombie && a.Health > 0))
                {
                    if (enemy == null) return;


                    if (enemy.IsValidTarget(R.Range) && enemy.HealthPercent <= 50)
                    {

                        if (DamageLib.QCalc(enemy) >= enemy.Health)
                        {
                            var Qp = Q.GetPrediction(enemy);
                            if (Q.IsReady() && Q.IsInRange(enemy) && Program.ModesMenu1["KQ"].Cast<CheckBox>().CurrentValue && Qp.HitChance >= HitChance.High && !enemy.IsInvulnerable)
                            {
                                Q.Cast(Qp.CastPosition);
                            }
                        }

                        if (enemy.HealthPercent <= 20 && DamageLib.R3Calc(enemy) >= enemy.Health)
                        {
                            var Rp = R.GetPrediction(enemy);
                            if (R.IsReady() && R.IsInRange(enemy) && Program.ModesMenu1["KR"].Cast<CheckBox>().CurrentValue && Rp.HitChance >= HitChance.High && !enemy.IsInvulnerable)
                            {
                                R.Cast(Rp.CastPosition);
                            }
                        }

                        if (enemy.HealthPercent <= 30 && DamageLib.R2Calc(enemy) >= enemy.Health)
                        {
                            var Rp = R.GetPrediction(enemy);
                            if (R.IsReady() && R.IsInRange(enemy) && Program.ModesMenu1["KR"].Cast<CheckBox>().CurrentValue && Rp.HitChance >= HitChance.High && !enemy.IsInvulnerable)
                            {
                                R.Cast(Rp.CastPosition);
                            }
                        }

                        if (DamageLib.R1Calc(enemy) >= enemy.Health)
                            {
                                var Rp = R.GetPrediction(enemy);
                                if (R.IsReady() && R.IsInRange(enemy) && Program.ModesMenu1["KR"].Cast<CheckBox>().CurrentValue && Rp.HitChance >= HitChance.High && !enemy.IsInvulnerable)
                                {
                                    R.Cast(Rp.CastPosition);
                                }
                            }
                    }
                }
            }
        }

        public static void AutoR()
        {
            {
                if (Program.ModesMenu1["AutoHarass"].Cast<CheckBox>().CurrentValue)
                {
                    var Target = TargetSelector.GetTarget(R.Range, DamageType.Mixed);
                    if (Target == null) return;
                    var Rpr = R.GetPrediction(Target);
                    if (R.IsInRange(Target) && R.IsReady() && !Target.IsInvulnerable)
                    {
                        if (Player.Instance.GetBuffCount("kogmawlivingartillerycost") < ModesMenu1["ARStack"].Cast<Slider>().CurrentValue)
                        {
                            R.Cast(Rpr.CastPosition);
                        }
                    }
                }
            }
        }
    }
}