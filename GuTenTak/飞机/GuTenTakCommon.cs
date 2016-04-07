using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using System.Collections.Generic;
using System.Linq;
using System;

namespace GuTenTak.Corki
{
    internal class Common : Program
    {
        public static object HeroManager { get; private set; }
        private static bool cz = false;
        private static float czx = 0, czy = 0, czx2 = 0, czy2 = 0;

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

        internal static void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            if (ModesMenu1["ComboQ"].Cast<CheckBox>().CurrentValue)
            {
                var Target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                var Qp = Q.GetPrediction(Target);
                if (Q.IsInRange(Target) && Q.IsReady() && Qp.HitChance >= HitChance.High)
                {
                    Q.Cast(Qp.CastPosition);
                }
            }
        }

        public static void Combo()
        {
            //if (ModesMenu1["useItem"].Cast<CheckBox>().CurrentValue)
            // {
            //Itens.useItemtens();
            //}

            var RTarget = TargetSelector.GetTarget(R.Range, DamageType.Mixed);
            if (RTarget == null) return;
            {
                R.AllowedCollisionCount = 0;
                var useR = ModesMenu1["ComboR"].Cast<CheckBox>().CurrentValue;
                var Rp = R.GetPrediction(RTarget);
                if (!RTarget.IsValid()) return;
                if (R.IsInRange(RTarget) && R.IsReady() && useR && !RTarget.IsInvulnerable)
                {
                    float rSplash = 140;
                    if (Player.HasBuff("corkimissilebarragecounterbig"))
                    {
                        rSplash = 290;
                    }

                    bool cast = true;
                    var poutput = R.GetPrediction(RTarget);
                    foreach (var minion in poutput.CollisionObjects.Where(minion => minion.IsEnemy && minion.Distance(poutput.CastPosition) > rSplash))
                    {
                        cast = false;
                        break;
                    }

                    if (cast == true)
                    {
                        if (Rp.HitChance >= HitChance.Medium)
                        {
                            R.Cast(Rp.CastPosition);
                        }
                    }
                    else
                    {
                        R.Cast(RTarget);
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
                        if (ModesMenu1["ComboA"].Cast<CheckBox>().CurrentValue && !ObjectManager.Player.IsInAutoAttackRange(QTarget))
                        {
                            Q.Cast(Qp.CastPosition);
                        }
                        else
                        {
                            if (!ModesMenu1["ComboA"].Cast<CheckBox>().CurrentValue)
                            {
                                Q.Cast(Qp.CastPosition);
                            }
                        }
                    }
                }
                var ETarget = TargetSelector.GetTarget(550, DamageType.Mixed);
                if (ETarget == null) return;
                {
                    var useE = ModesMenu1["ComboE"].Cast<CheckBox>().CurrentValue;
                    if (E.IsInRange(QTarget) && E.IsReady() && useE && _Player.ManaPercent >= Program.ModesMenu1["ManaCE"].Cast<Slider>().CurrentValue && ObjectManager.Player.IsInAutoAttackRange(QTarget))
                    {
                        E.Cast();
                    }
                }
            }
        }

        internal static void zigzag(EventArgs args)
        {
            var zigzagTarget = TargetSelector.GetTarget(Q.Range, DamageType.Mixed);
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

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && ModesMenu1["ComboQ"].Cast<CheckBox>().CurrentValue)
            {
                var zigzag = Q.GetPrediction(zigzagTarget);
                if (zigzag.HitChance >= HitChance.High)
                {
                    Q.Cast(zigzagTarget.Position);
                }
            }

        }

        public static void Harass()
        {
            //Harass
            var RTarget = TargetSelector.GetTarget(R.Range, DamageType.Mixed);
            if (RTarget == null) return;
            {
                R.AllowedCollisionCount = 0;
                var useR = ModesMenu1["HarassR"].Cast<CheckBox>().CurrentValue;
                var Rp = R.GetPrediction(RTarget);
                if (!RTarget.IsValid()) return;
                if (R.IsInRange(RTarget) && R.IsReady() && useR && !RTarget.IsInvulnerable && Program._Player.ManaPercent >= Program.ModesMenu1["ManaHR"].Cast<Slider>().CurrentValue
                    && R.Handle.Ammo <= ModesMenu1["HRStack"].Cast<Slider>().CurrentValue)
                {
                    float rSplash = 140;
                    if (Player.HasBuff("corkimissilebarragecounterbig"))
                    {
                        rSplash = 290;
                    }

                    bool cast = true;
                    var poutput = R.GetPrediction(RTarget);
                    foreach (var minion in poutput.CollisionObjects.Where(minion => minion.IsEnemy && minion.Distance(poutput.CastPosition) > rSplash))
                    {
                        cast = false;
                        break;
                    }

                    if (cast == true)
                    {
                        if (Rp.HitChance >= HitChance.Medium)
                        {
                            R.Cast(Rp.CastPosition);
                        }
                    }
                    else
                    {
                        R.Cast(RTarget);
                    }
                }
            }

            var Target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (Target == null) return;
            {
                var useQ = ModesMenu1["HarassQ"].Cast<CheckBox>().CurrentValue;
                var Qp = Q.GetPrediction(Target);
                if (!Target.IsValid() && Target == null) return;
                if (Q.IsInRange(Target) && Q.IsReady() && useQ && Qp.HitChance >= HitChance.High && Program._Player.ManaPercent >= Program.ModesMenu1["ManaHQ"].Cast<Slider>().CurrentValue)
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
            if (Q.IsReady() && ModesMenu2["FarmQ"].Cast<CheckBox>().CurrentValue && Program._Player.ManaPercent >= Program.ModesMenu2["ManaL"].Cast<Slider>().CurrentValue)
            {
                var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t.IsEnemy && !t.IsDead && t.IsValid && !t.IsInvulnerable && t.IsInRange(Player.Instance.Position, Q.Range));
                foreach (var m in minions)
                {
                    if (Q.GetPrediction(m).CollisionObjects.Where(t => t.IsEnemy && !t.IsDead && t.IsValid && !t.IsInvulnerable).Count() >= 3)
                    {
                        Q.Cast(m);
                        break;
                    }
                }
            }
            if (R.IsReady() && ModesMenu2["FarmR"].Cast<CheckBox>().CurrentValue && Program._Player.ManaPercent >= Program.ModesMenu2["ManaLR"].Cast<Slider>().CurrentValue
                 && R.Handle.Ammo > ModesMenu2["LJRStack"].Cast<Slider>().CurrentValue)
            {
                var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t.IsEnemy && !t.IsDead && t.IsValid && !t.IsInvulnerable && t.IsInRange(Player.Instance.Position, R.Range)).OrderBy(t => t.Health);
                if (minions.Count() > 0)
                {
                    R.Cast(minions.First());
                }
            }
        }
        public static void JungleClear()
        {
            if (Q.IsReady() && ModesMenu2["JungleQ"].Cast<CheckBox>().CurrentValue && Program._Player.ManaPercent >= Program.ModesMenu2["ManaJ"].Cast<Slider>().CurrentValue)
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
            if (R.IsReady() && ModesMenu2["JungleR"].Cast<CheckBox>().CurrentValue && Program._Player.ManaPercent >= Program.ModesMenu2["ManaJR"].Cast<Slider>().CurrentValue
                && R.Handle.Ammo > ModesMenu2["LJRStack"].Cast<Slider>().CurrentValue)
            {
                var minions = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, R.Range).Where(t => !t.IsDead && t.IsValid && !t.IsInvulnerable);
                if (minions.Count() > 0)
                {
                    R.Cast(minions.First());
                }
            }
         }

        public static void Flee()
        {
            if (ModesMenu3["FleeW"].Cast<CheckBox>().CurrentValue)
            {
                var tempPos = Game.CursorPos;
                if (tempPos.IsInRange(Player.Instance.Position, W.Range))
                {
                    W.Cast(tempPos);
                }
                else
                {
                    W.Cast(Player.Instance.Position.Extend(tempPos, 800).To3DWorld());
                }
            }
            if (ModesMenu3["FleeR"].Cast<CheckBox>().CurrentValue && Program._Player.ManaPercent <= Program.ModesMenu3["ManaFlR"].Cast<Slider>().CurrentValue)
            {
                if (ObjectManager.Player.CountEnemiesInRange(400) == 0)
                {
                    var Target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                    if (Target == null) return;
                    var Rp = R.GetPrediction(Target);
                    R.Cast(Rp.CastPosition);
                }

            }
        }


        internal static void OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (!sender.IsMe) return;
            var type = args.Buff.Type;
            var duration = args.Buff.EndTime - Game.Time;
            var Name = args.Buff.Name.ToLower();

            if (ModesMenu3["Qssmode"].Cast<ComboBox>().CurrentValue == 0)
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
            }
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

        /*
        internal static void Dash_OnDash(Obj_AI_Base sender, Dash.DashEventArgs e)
        {
            var Target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            var Qp = Q.GetPrediction(Target);
            if (Qp.HitChance == HitChance.Dashing)
            {
                Q.Cast(Qp.CastPosition);
                Chat.Print("Dash!");
            }
        }
        */

        /*
        internal static void Dash_OnDash(Obj_AI_Base sender, Dash.DashEventArgs e)
        {
            if (ModesMenu1["Snipe"].Cast<CheckBox>().CurrentValue)
            {
                var Target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                var Qp = Q.GetPrediction(Target);
                if (Q.GetPrediction(Target).HitChance >= HitChance.Dashing)
                {
                    Q.Cast(Q.GetPrediction(Target).CastPosition);
                }
            }
            Chat.Print("Dash!");
        }
        */

        internal static void Gapcloser_OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs gapcloser)
        {
            if (Program.ModesMenu3["AntiGap"].Cast<CheckBox>().CurrentValue)
            {
                string[] herogapcloser =
                {
                "Braum", "Ekko", "Elise", "Fiora", "Kindred", "Lucian", "Yi", "Nidalee", "Quinn", "Riven", "Shaco", "Sion", "Vayne", "Yasuo", "Graves", "Azir", "Gnar", "Irelia", "Kalista"
                };
                if (sender.IsEnemy && sender.GetAutoAttackRange() >= ObjectManager.Player.Distance(gapcloser.End) && !herogapcloser.Any(sender.ChampionName.Contains))
                {
                    var diffGapCloser = gapcloser.End - gapcloser.Start;
                    W.Cast(ObjectManager.Player.ServerPosition + diffGapCloser);
                }
            }
        }

        public static void KillSteal()
        {
            if (Program.ModesMenu1["KS"].Cast<CheckBox>().CurrentValue)
            {
                foreach (var enemy in EntityManager.Heroes.Enemies.Where(a => !a.IsDead && !a.IsZombie && a.Health > 0))
                {
                    if (enemy == null) return;


                    if (enemy.IsValidTarget(R.Range) && enemy.HealthPercent <= 40)
                    {

                        if (DamageLib.QCalc(enemy) >= enemy.Health)
                        {
                            var Qp = Q.GetPrediction(enemy);
                            if (Q.IsReady() && Q.IsInRange(enemy) && Program.ModesMenu1["KQ"].Cast<CheckBox>().CurrentValue && Qp.HitChance >= HitChance.High && !enemy.IsInvulnerable)
                            {
                                Q.Cast(Qp.CastPosition);
                            }
                        }


                        if (Player.HasBuff("corkimissilebarragecounterbig"))
                        {
                            if (DamageLib.RCalc(enemy) * .5f >= enemy.Health)
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
        }
        /*
var Target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
if (ModesMenu1["Snipe"].Cast<CheckBox>().CurrentValue) //&& ModesMenu1["Snipe " + Target.ChampionName].Cast<CheckBox>().CurrentValue)
{
        var pred = Q.GetPrediction(Target);
    if (Q.IsReady() && pred.HitChance >= HitChance.Dashing)
    {
        Q.Cast(pred.CastPosition);
    }
}
*/
        /*
        public static void Snipe()
        {
            var Target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (ModesMenu1["Snipe"].Cast<CheckBox>().CurrentValue) //&& ModesMenu1["Snipe " + Target.ChampionName].Cast<CheckBox>().CurrentValue)
            {
                if (Q.IsInRange(Target) && Q.IsReady() && Q.GetPrediction(Target).HitChance == HitChance.Dashing)
                {
                    Prediction.Position.GetDashPos(Target)
                    var Qpr = Q.GetPrediction(Target);
                    Q.Cast(Target
                }
            }
        }*/

        /*
        public static new void AutoR()
=======
        public static new void AutoQ()
>>>>>>> origin/master
        {
            {
                var Target = TargetSelector.GetTarget(R.Range, DamageType.Mixed);
                if (Target == null) return;
                var Rpr = R.GetPrediction(Target);
                if (!Target.IsValid()) return;
                if (Q.IsInRange(Target) && Q.IsReady() && Rpr.HitChance >= HitChance.High)
                {
                    Q.Cast(Rpr.CastPosition);
                }
            }
        }
        */
        public static void AutoR()
        {
            {
                if (R.Handle.Ammo <= ModesMenu1["HRStack"].Cast<Slider>().CurrentValue)
                {
                    var Target = TargetSelector.GetTarget(R.Range, DamageType.Mixed);
                    if (Target == null) return;
                    var Rpr = R.GetPrediction(Target);
                    float rSplash = 140;
                    if (Player.HasBuff("corkimissilebarragecounterbig"))
                    {
                        rSplash = 290;
                    }

                    bool cast = true;
                    var poutput = R.GetPrediction(Target);
                    foreach (var minion in poutput.CollisionObjects.Where(minion => minion.IsEnemy && minion.Distance(poutput.CastPosition) > rSplash))
                    {
                        cast = false;
                        break;
                    }

                    if (cast == true)
                    {
                        if (Rpr.HitChance >= HitChance.Medium)
                        {
                            R.Cast(Rpr.CastPosition);
                        }
                    }
                    else
                    {
                        R.Cast(Target);
                    }
                }
            }
        }
    }
}