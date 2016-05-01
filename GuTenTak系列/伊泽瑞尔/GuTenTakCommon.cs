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
        public static Obj_AI_Base GetFindObj(string name, float range, Vector3 Pos)
        {
            var CusPos = Pos;
            if (ObjectManager.Player.Distance(CusPos) > range) CusPos = (Vector3)Player.Instance.Position.Extend(Game.CursorPos, range);
            var GetObj = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(f => f.IsAlly && !f.IsMe && f.Position.Distance(ObjectManager.Player.Position) < range && f.Distance(CusPos) < 150);
            if (GetObj != null)
                return GetObj;
            return null;
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
            if (ModesMenu1["ComboQ"].Cast<CheckBox>().CurrentValue && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                /*if (target == null || !(target is AIHeroClient) || target.IsDead || target.IsInvulnerable || !target.IsEnemy || target.IsPhysicalImmune || target.IsZombie)
                    return;
                var enemy = target as AIHeroClient;*/

                var Target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                if (Target == null || !Target.IsValid) return;
                
                if (Q.IsReady() && Q.IsInRange(Target))
                {
                    var Qp = Q.GetPrediction(Target);
                    if (Qp.HitChance >= HitChance.High)
                        Q.Cast(Qp.CastPosition);
                }
            }
        }

        public static void Combo()
        {

            var Target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (Target == null || !Target.IsValid) return;
            var useQ = ModesMenu1["ComboQ"].Cast<CheckBox>().CurrentValue;
            var useW = ModesMenu1["ComboW"].Cast<CheckBox>().CurrentValue;
            var useR = ModesMenu1["ComboR"].Cast<CheckBox>().CurrentValue;
            
            /*var Rp = R.GetPrediction(Target);
            if (!Target.IsValidTarget()) return;
            if (ModesMenu1["useItem"].Cast<CheckBox>().CurrentValue)
            {
            Itens.useItemtens();
            }*/

            if (useQ && Q.IsReady() && Q.IsInRange(Target))
            {
                var Qp = Q.GetPrediction(Target);
                if (ModesMenu1["ComboA"].Cast<CheckBox>().CurrentValue && !Player.Instance.IsInAutoAttackRange(Target) && !Target.IsInvulnerable)
                {
                    if (Qp.HitChancePercent >= 77)
                        Q.Cast(Qp.CastPosition);
                }
                else if(!ModesMenu1["ComboA"].Cast<CheckBox>().CurrentValue)
                {
                    if (Qp.HitChancePercent >= 77)
                        Q.Cast(Qp.CastPosition);
                }
            }

            if (useW && W.IsReady() && Player.Instance.ManaPercent >= ModesMenu1["ManaCW"].Cast<Slider>().CurrentValue && W.IsInRange(Target) && !Target.IsInvulnerable)
            {
                var Wp = W.GetPrediction(Target);
                if (Wp.HitChance >= HitChance.High)
                    W.Cast(Wp.CastPosition);
            }
            if (useR && R.IsReady() && R.IsInRange(Target) && !Target.IsInvulnerable)
            {
                if (Player.Instance.CountEnemiesInRange(700) == 0)
                {//Thanks to Hi I'm Ezreal
                    foreach (var hero in EntityManager.Heroes.Enemies.Where(hero => hero.IsValidTarget(3000)))
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

                            var RTargets = ModesMenu1["RCount"].Cast<Slider>().CurrentValue;
                            if (collision.Count >= RTargets)
                            {
                                R.Cast(hero);
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
            if (Target == null || !Target.IsValid()) return;
            //var TargetR = TargetSelector.GetTarget(R.Range, DamageType.Physical);
            var useQ = ModesMenu1["HarassQ"].Cast<CheckBox>().CurrentValue;
            var useW = ModesMenu1["HarassW"].Cast<CheckBox>().CurrentValue;

            if (useQ && Q.IsReady() && Player.Instance.ManaPercent >= ModesMenu1["ManaHQ"].Cast<Slider>().CurrentValue && Q.IsInRange(Target))
            {
                var Qp = Q.GetPrediction(Target);
                if (Qp.HitChance >= HitChance.High)
                    Q.Cast(Qp.CastPosition);
            }
            if (useW && W.IsReady() && Player.Instance.ManaPercent >= ModesMenu1["ManaHW"].Cast<Slider>().CurrentValue && W.IsInRange(Target))
            {
                var Wp = W.GetPrediction(Target);
                if (Wp.HitChance >= HitChance.High)
                    W.Cast(Wp.CastPosition);
            }
        }
        public static void LaneClear()
        {
            if (ModesMenu2["FarmQ"].Cast<CheckBox>().CurrentValue && Player.Instance.ManaPercent >= ModesMenu2["ManaL"].Cast<Slider>().CurrentValue && Q.IsReady())
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
            if (ModesMenu2["JungleQ"].Cast<CheckBox>().CurrentValue && Player.Instance.ManaPercent >= ModesMenu2["ManaJ"].Cast<Slider>().CurrentValue && Q.IsReady())
            {
                var jungleMonsters = EntityManager.MinionsAndMonsters.GetJungleMonsters().OrderByDescending(j => j.Health).FirstOrDefault(j => j.IsValidTarget(Q.Range));
                //var minioon = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t.IsInRange(Player.Instance.Position, Q.Range) && !t.IsDead && t.IsValid && !t.IsInvulnerable).Count();
                if (jungleMonsters == null) return;

                if (Q.IsInRange(jungleMonsters))
                {
                    var Qp = Q.GetPrediction(jungleMonsters);
                    Q.Cast(Qp.CastPosition);
                }
            }
        }

        public static void LastHit()
        {
            var source =
                EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault
                    (m =>
                        m.IsValidTarget(Q.Range) &&
                        (Player.Instance.GetSpellDamage(m, SpellSlot.Q) > m.TotalShieldHealth() && m.IsEnemy && !m.IsDead && m.IsValid && !m.IsInvulnerable));

            if (source == null || !source.IsValid) return;

            if (Player.Instance.ManaPercent >= ModesMenu2["ManaF"].Cast<Slider>().CurrentValue)
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
            if (ModesMenu3["FleeQ"].Cast<CheckBox>().CurrentValue && Player.Instance.ManaPercent >= ModesMenu3["ManaFlQ"].Cast<Slider>().CurrentValue)
            {
                if ( Player.Instance.CountEnemiesInRange(400) == 0 /*|| E.IsReady(2500)*/)
                {
                    Obj_AI_Base Target = TargetSelector.GetTarget(Q.Range, DamageType.Physical) as AIHeroClient;
                    if (!ModesMenu3["FleeOQ"].Cast<CheckBox>().CurrentValue && (Target == null || !Target.IsValid))
                        Target = EntityManager.MinionsAndMonsters.CombinedAttackable.FirstOrDefault(e=>Player.Instance.IsInRange(e,Q.Range)) as Obj_AI_Minion;
                    //Drawing.OnDraw += (args) => Drawing.DrawCircle(Target.Position, Target.BoundingRadius * 2, System.Drawing.Color.Green);

                    if (Target != null && Target.IsValid)
                    {
                        if (Target is AIHeroClient)
                        {
                            var Qp = Q.GetPrediction(Target);
                            if (Qp.HitChance > HitChance.High)
                                Q.Cast(Qp.CastPosition);
                        }
                        else if (Target is Obj_AI_Minion)
                            Q.Cast(Target);
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

        internal static void ItemUsage(EventArgs args)
        {
            if (ModesMenu3["useYoumuu"].Cast<CheckBox>().CurrentValue && Youmuu.IsReady())
            {
                Youmuu.Cast();
            }
            if (ModesMenu3["usehextech"].Cast<CheckBox>().CurrentValue && Item.CanUseItem(hextech.Id))
            {
                var htarget = TargetSelector.GetTarget(700, DamageType.Magical); // 700 = hextech.Range
                if (htarget != null && htarget.IsValid)
                {
                    Item.UseItem(hextech.Id, htarget);
                }
            }
            if (ModesMenu3["useBotrk"].Cast<CheckBox>().CurrentValue)
            {
                var ReadyCutlass = Item.CanUseItem(Cutlass.Id);
                var ReadyBotrk = Item.CanUseItem(Botrk.Id);
                if (!ReadyBotrk && !ReadyCutlass || Player.Instance.HealthPercent > ModesMenu3["minHPBotrk"].Cast<Slider>().CurrentValue)
                    return;
                var btarget = TargetSelector.GetTarget(550, DamageType.Physical); // 550 = Botrk.Range
                if (btarget != null && btarget.IsValid &&
                    btarget.HealthPercent < ModesMenu3["enemyMinHPBotrk"].Cast<Slider>().CurrentValue)
                {
                    if (ReadyCutlass)
                        Item.UseItem(Cutlass.Id, btarget);
                    if (ReadyBotrk)
                        Botrk.Cast(btarget);
                }
            }
        }

        internal static void DoQSS()
        {
            if (Qss.IsReady() && Player.Instance.CountEnemiesInRange(1800) > 0)
            {
                Core.DelayAction(() => Qss.Cast(), ModesMenu3["QssDelay"].Cast<Slider>().CurrentValue);
            }
            if (Simitar.IsReady() && Player.Instance.CountEnemiesInRange(1800) > 0)
            {
                Core.DelayAction(() => Simitar.Cast(), ModesMenu3["QssDelay"].Cast<Slider>().CurrentValue);
            }
        }

        private static void UltQSS()
        {
            if (Qss.IsReady())
            {
                Core.DelayAction(() => Qss.Cast(), ModesMenu3["QssUltDelay"].Cast<Slider>().CurrentValue);
            }
            if (Simitar.IsReady())
            {
                Core.DelayAction(() => Simitar.Cast(), ModesMenu3["QssUltDelay"].Cast<Slider>().CurrentValue);
            }
        }

        /*public static void Skinhack(EventArgs args)
        {
            Player.SetSkinId(ModesMenu3["skinId"].Cast<ComboBox>().CurrentValue);
        }
        */
        internal static void Gapcloser_OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs gapcloser)
        {
            if (sender.IsEnemy && sender.GetAutoAttackRange() >= Player.Instance.Distance(gapcloser.End) && !herogapcloser.Any(sender.ChampionName.Contains))
            {
                var diffGapCloser = gapcloser.End - gapcloser.Start + Player.Instance.ServerPosition;
                //if (ModesMenu3["BlockE"].Cast<CheckBox>().CurrentValue && !enemyTurret.FirstOrDefault(tur => tur.Distance(diffGapCloser) < 850).IsValid)
                //    return;
                E.Cast(diffGapCloser);
            }
        }

        public static void KillSteal(EventArgs args)
        {
            foreach (var enemy in EntityManager.Heroes.Enemies.Where(a => !a.IsDead && !a.IsZombie && a.Health > 0))
            {
                if (enemy == null) return;
                if (enemy.HealthPercent <= 40 && enemy.IsValidTarget(R.Range))
                {
                    if (ModesMenu1["KQ"].Cast<CheckBox>().CurrentValue && Q.IsReady() && Q.IsInRange(enemy) && GuTenTak_Ezreal.KillSteal.IsKillable(enemy, Q.Slot))//!enemy.IsInvulnerable && DamageLib.QCalc(enemy) >= enemy.Health)
                    {
                        var Qp = Q.GetPrediction(enemy);
                        if (Qp.HitChance >= HitChance.High)
                            Q.Cast(Qp.CastPosition);
                    }
                    if (ModesMenu1["KW"].Cast<CheckBox>().CurrentValue && W.IsReady() && W.IsInRange(enemy) && GuTenTak_Ezreal.KillSteal.IsKillable(enemy, W.Slot))//!enemy.IsInvulnerable && DamageLib.WCalc(enemy) >= enemy.Health)
                    {
                        var Wp = W.GetPrediction(enemy);
                        if (Wp.HitChance >= HitChance.High)
                            W.Cast(Wp.CastPosition);
                    }
                    if (ModesMenu1["KR"].Cast<CheckBox>().CurrentValue && R.IsReady() && R.IsInRange(enemy) && Player.Instance.CountEnemiesInRange(700) == 0 && GuTenTak_Ezreal.KillSteal.IsKillable(enemy, R.Slot))//!enemy.IsInvulnerable && DamageLib.RCalc(enemy) * 0.7f >= enemy.Health)
                    {
                        var Rp = R.GetPrediction(enemy);
                        if (Rp.HitChance >= HitChance.High)
                            R.Cast(Rp.CastPosition);
                    }
                }
            }
        }
        public static void AutoQ(EventArgs args)
        {
            if (ModesMenu1["ManaAuto"].Cast<Slider>().CurrentValue <= Player.Instance.ManaPercent)
            {
                var Target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                if (Target == null || !Target.IsValid()) return;
                if (Q.IsReady() && Q.IsInRange(Target))
                {
                    var Qpr = Q.GetPrediction(Target);
                    if (Qpr.HitChance >= HitChance.High)
                        Q.Cast(Qpr.CastPosition);
                }
            }
        }
        public static void StackTear(EventArgs args)
        {
            if (Player.Instance.IsInShopRange() && (Tear.IsOwned() || Manamune.IsOwned() || Archangel.IsOwned()))
            {
                Q.Cast(Game.CursorPos);
                W.Cast(Game.CursorPos);
            }
        }

    }
}
