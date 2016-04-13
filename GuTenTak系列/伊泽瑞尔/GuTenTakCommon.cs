using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using System.Collections.Generic;
using System.Linq;
using System;

namespace GuTenTak.Ezreal
{
    internal class Common : Program
    {
        public static object HeroManager { get; private set; }
        // public static Geometry.Polygon.Circle DashCircle { get; private set; }

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
            if (ModesMenu1["ComboA"].Cast<CheckBox>().CurrentValue && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                if (target == null || !(target is AIHeroClient) || target.IsDead || target.IsInvulnerable || !target.IsEnemy || target.IsPhysicalImmune || target.IsZombie)
                    return;
                var useQ = ModesMenu1["ComboQ"].Cast<CheckBox>().CurrentValue;
                var enemy = target as AIHeroClient;
                if (enemy == null)
                    return;

                if (useQ)
                {
                    var Target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                    var Qp = Q.GetPrediction(Target);
                    if (Q.IsInRange(Target) && Q.IsReady() && Qp.HitChance >= HitChance.High)
                    {
                        Q.Cast(Qp.CastPosition);
                    }
                }
            }
        }

        public static void Combo()
        {

            var Target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (Target == null) return;
            var useQ = ModesMenu1["ComboQ"].Cast<CheckBox>().CurrentValue;
            var useW = ModesMenu1["ComboW"].Cast<CheckBox>().CurrentValue;
            var useR = ModesMenu1["ComboR"].Cast<CheckBox>().CurrentValue;
            var Qp = Q.GetPrediction(Target);
            var Wp = W.GetPrediction(Target);
            var Rp = R.GetPrediction(Target);
            if (!Target.IsValid()) return;
            //if (ModesMenu1["useItem"].Cast<CheckBox>().CurrentValue)
            // {
            //Itens.useItemtens();
            //}

            if (Q.IsInRange(Target) && Q.IsReady() && useQ && Qp.HitChance >= HitChance.High)
            {
                if (ModesMenu1["ComboA"].Cast<CheckBox>().CurrentValue && !Player.Instance.IsInAutoAttackRange(Target) && !Target.IsInvulnerable)
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

            if (W.IsInRange(Target) && W.IsReady() && useW && _Player.ManaPercent >= Program.ModesMenu1["ManaCW"].Cast<Slider>().CurrentValue && Wp.HitChance >= HitChance.High && !Target.IsInvulnerable)
            {
                W.Cast(Wp.CastPosition);
            }
            if (R.IsInRange(Target) && R.IsReady() && useR && !Target.IsInvulnerable)
            {
                if (Player.Instance.CountEnemiesInRange(700) == 0)
                {//Thanks to Hi I'm Ezreal
                    foreach (var hero in EntityManager.Heroes.Enemies.Where(hero => hero.IsValidTarget(3000)))
                    {
                        if (R.IsReady())
                        {
                            var collision = new List<AIHeroClient>();
                            var startPos = Player.Instance.Position.To2D();
                            var endPos = hero.Position.To2D();
                            collision.Clear();
                            foreach (
                                var colliHero in
                                    EntityManager.Heroes.Enemies.Where(
                                        colliHero =>
                                            !colliHero.IsDead && colliHero.IsVisible &&
                                            colliHero.IsInRange(hero, 3000) && colliHero.IsValidTarget(3000)))
                            {
                                if (Prediction.Position.Collision.LinearMissileCollision(colliHero, startPos, endPos,
                                    R.Speed, R.Width, R.CastDelay))
                                {
                                    collision.Add(colliHero);
                                }

                                var RTargets = Program.ModesMenu1["RCount"].Cast<Slider>().CurrentValue;
                                if (collision.Count >= RTargets)
                                {
                                    R.Cast(hero);
                                }
                                //
                            }
                        }
                    }
                }
            }
        }

        internal static void OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (!sender.IsMe || ModesMenu3["Qssmode"].Cast<ComboBox>().CurrentValue == 1 && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) ) return;
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

        public static void Harass()
        {
            //Harass

            var Target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (Target == null && !Target.IsValid()) return;
            //var TargetR = TargetSelector.GetTarget(R.Range, DamageType.Physical);
            var useQ = ModesMenu1["HarassQ"].Cast<CheckBox>().CurrentValue;
            var useW = ModesMenu1["HarassW"].Cast<CheckBox>().CurrentValue;
            var Qp = Q.GetPrediction(Target);
            var Wp = W.GetPrediction(Target);
            //if (!Target.IsValid() && Target == null) return;


            if (Q.IsInRange(Target) && Q.IsReady() && useQ && Qp.HitChance >= HitChance.High && Program._Player.ManaPercent >= Program.ModesMenu1["ManaHQ"].Cast<Slider>().CurrentValue)
            {
                Q.Cast(Qp.CastPosition);
            }
            if (W.IsInRange(Target) && W.IsReady() && useW && Wp.HitChance >= HitChance.High && Program._Player.ManaPercent >= Program.ModesMenu1["ManaHW"].Cast<Slider>().CurrentValue)
            {
                W.Cast(Wp.CastPosition);

            }
        }
        public static void LaneClear()
        {
            if (Q.IsReady() && ModesMenu2["FarmQ"].Cast<CheckBox>().CurrentValue && Program._Player.ManaPercent >= Program.ModesMenu2["ManaL"].Cast<Slider>().CurrentValue)
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
        }

      //    var useQ = ModesMenu2["FarmQ"].Cast<CheckBox>().CurrentValue;
      //      var minions = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(m => m.IsValidTarget(Q.Range));
      //      var minion = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t.IsInRange(Player.Instance.Position, W.Range) && !t.IsDead && t.IsValid && !t.IsInvulnerable).Count();
      //      if (minions == null) return;
      //      if ((_Player.ManaPercent <= Program.ModesMenu2["ManaF"].Cast<Slider>().CurrentValue))
      //      {
      //          return;
      //      }
      //
      //      if (useQ && Q.IsReady() && Q.IsInRange(minions))
      //      {
      //          Q.Cast(minions);
      //      }
      //
      // }
        public static void JungleClear()
        {

            var useQ = ModesMenu2["JungleQ"].Cast<CheckBox>().CurrentValue;
            var jungleMonsters = EntityManager.MinionsAndMonsters.GetJungleMonsters().OrderByDescending(j => j.Health).FirstOrDefault(j => j.IsValidTarget(Program.Q.Range));
            var minioon = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t.IsInRange(Player.Instance.Position, Program.Q.Range) && !t.IsDead && t.IsValid && !t.IsInvulnerable).Count();
            if (jungleMonsters == null) return;
            if ((Program._Player.ManaPercent <= Program.ModesMenu2["ManaJ"].Cast<Slider>().CurrentValue))
            {
                return;
            }
            var Qp = Q.GetPrediction(jungleMonsters);
            if (jungleMonsters == null) return;
            if (useQ && Q.IsReady() && Q.IsInRange(jungleMonsters))
            {
                Q.Cast(Qp.CastPosition);
            }

        }

        public static void LastHit()
        {
            var source =
                EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault
                    (m =>
                        m.IsValidTarget(Q.Range) &&
                        (Player.Instance.GetSpellDamage(m, SpellSlot.Q) > m.TotalShieldHealth() && m.IsEnemy && !m.IsDead && m.IsValid && !m.IsInvulnerable));

            if (source == null) return;

            if (Program._Player.ManaPercent >= Program.ModesMenu2["ManaF"].Cast<Slider>().CurrentValue)
            {
                if (ModesMenu2["LastQ"].Cast<CheckBox>().CurrentValue && Q.IsReady())
                {
                    Q.Cast(source);
                }
            }
        }

        /*
if (Q.IsReady() && ModesMenu2["FarmQ"].Cast<CheckBox>().CurrentValue && Program._Player.ManaPercent >= Program.ModesMenu2["ManaL"].Cast<Slider>().CurrentValue)
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
*/

        public static void Flee()
        {
            if (ModesMenu3["FleeQ"].Cast<CheckBox>().CurrentValue && Program._Player.ManaPercent <= Program.ModesMenu3["ManaFlQ"].Cast<Slider>().CurrentValue)
            {
                if ( Player.Instance.CountEnemiesInRange(400) == 0 /*|| E.IsReady(1500) && !E.IsReady()*/ )
                {
                    var Target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                    if (Target != null && Target.IsValid)
                    {
                        var Qp = Q.GetPrediction(Target);
                        Q.Cast(Qp.CastPosition);
                    }
                }

            }
            if (ModesMenu3["FleeE"].Cast<CheckBox>().CurrentValue)
            {
                var tempPos = Game.CursorPos;
                if ( tempPos.IsInRange(Player.Instance.Position, E.Range))
                {
                    //if (ModesMenu3["BlockE"].Cast<CheckBox>().CurrentValue && !enemyTurret.FirstOrDefault(tur => tur.Distance(tempPos) < 850).IsValid) return;
                    E.Cast(tempPos);
                }
                else
                {
                    tempPos = Player.Instance.Position.Extend(tempPos, 450).To3DWorld();
                    //if (ModesMenu3["BlockE"].Cast<CheckBox>().CurrentValue && enemyTurret.FirstOrDefault(tur => tur.Distance(tempPos) < 850).IsValid) return;
                    E.Cast(tempPos);
                    //Drawing.OnDraw+=(args)=>Drawing.DrawCircle(Player.Instance.Position.Extend(tempPos, 450).To3DWorld(),30, System.Drawing.Color.Red);
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
                    var diffGapCloser = gapcloser.End - gapcloser.Start + Player.Instance.ServerPosition;
                    //if (ModesMenu3["BlockE"].Cast<CheckBox>().CurrentValue && !enemyTurret.FirstOrDefault(tur => tur.Distance(diffGapCloser) < 850).IsValid)
                    //    return;
                    E.Cast(diffGapCloser);
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
                        var Qp = Q.GetPrediction(enemy);
                        var Wp = W.GetPrediction(enemy);
                        var Rp = R.GetPrediction(enemy);
                        if (DamageLib.QCalc(enemy) >= enemy.Health && Q.IsReady() && Q.IsInRange(enemy) && Program.ModesMenu1["KQ"].Cast<CheckBox>().CurrentValue && Qp.HitChance >= HitChance.High && !enemy.IsInvulnerable)
                            {
                           
                            Q.Cast(Qp.CastPosition);
                            }
                            if (DamageLib.WCalc(enemy) >= enemy.Health && W.IsReady() && W.IsInRange(enemy) && Program.ModesMenu1["KW"].Cast<CheckBox>().CurrentValue && Wp.HitChance >= HitChance.High && !enemy.IsInvulnerable)
                            {
                            
                            W.Cast(Wp.CastPosition);
                            }
                            if (DamageLib.RCalc(enemy) * 0.7f >= enemy.Health && R.IsReady() && R.IsInRange(enemy) && Program.ModesMenu1["KR"].Cast<CheckBox>().CurrentValue && Rp.HitChance >= HitChance.High && !enemy.IsInvulnerable)
                            {
                           
                            if (Player.Instance.CountEnemiesInRange(700) == 0)
                                {
                                    R.Cast(Rp.CastPosition);
                                }
                            }
                        }
                    }
                }
            }
        public static new void AutoQ()
        {
            {
                var Target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                if (Target == null) return;
                var Qpr = Q.GetPrediction(Target);
                if (!Target.IsValid()) return;
                if (Q.IsInRange(Target) && Q.IsReady() && Qpr.HitChance >= HitChance.High)
                {
                    Q.Cast(Qpr.CastPosition);
                }
            }
        }
        public static void StackTear()
        {
            if (Program.ModesMenu3["StackTear"].Cast<CheckBox>().CurrentValue)
                {
                if (Player.Instance.IsInShopRange())
                {
                    if (Tear.IsOwned() || Manamune.IsOwned())
                    {
                        Q.Cast(Game.CursorPos);
                        W.Cast(Game.CursorPos);
                    }
                }
            }
        }

    }
}