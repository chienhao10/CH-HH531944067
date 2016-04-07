using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using System.Collections.Generic;
using System.Linq;
using System;

namespace GuTenTak.Sivir
{
    internal class Common : Program
    {
        public static object HeroManager { get; private set; }
        private static HashSet<string> DB { get; set; }

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

        public static float GetComboDamage(AIHeroClient target)
        {
            var damage = 0f;
            if (Q.IsReady())
            {
                damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q);
            }

            return damage;
        }


        public static void Combo()
        {
            var Target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (Target == null) return;
            var useQ = ModesMenu1["ComboQ"].Cast<CheckBox>().CurrentValue;
            var Qp = Q.GetPrediction(Target);
            if (!Target.IsValid()) return;
            if (Q.IsInRange(Target) && Q.IsReady() && useQ && Qp.HitChance >= HitChance.High && !ObjectManager.Player.IsInAutoAttackRange(Target) && !Target.IsInvulnerable)
            {
               Q.Cast(Qp.CastPosition);
            }

        }

        internal static void WLogic(AttackableUnit target, EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (target == null || !(target is AIHeroClient) || target.IsDead || target.IsInvulnerable || !target.IsEnemy || target.IsPhysicalImmune || target.IsZombie)
                    return;
                var ATarget = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                var useAQ = ModesMenu1["ComboQ"].Cast<CheckBox>().CurrentValue;
                var useAW = ModesMenu1["ComboW"].Cast<CheckBox>().CurrentValue;
                var AQp = Q.GetPrediction(ATarget);
                var enemy = target as AIHeroClient;
                if (enemy == null)
                    return;

                if (PlayerInstance.IsInAutoAttackRange(target) && W.IsReady() && useAW)
                {
                    W.Cast();
                }

                if (!PlayerInstance.IsAttackingPlayer && Q.IsInRange(ATarget) && Q.IsReady() && useAQ && AQp.HitChance >= HitChance.High)
                {
                    Q.Cast(AQp.CastPosition);
                }
            }
            
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                if (target == null || !(target is AIHeroClient) || target.IsDead || target.IsInvulnerable || !target.IsEnemy || target.IsPhysicalImmune || target.IsZombie)
                    return;
                var useHW = ModesMenu1["HarassW"].Cast<CheckBox>().CurrentValue;
                var enemy = target as AIHeroClient;
                if (enemy == null)
                    return;

                if (PlayerInstance.IsInAutoAttackRange(target) && W.IsReady() && useHW && PlayerInstance.ManaPercent >= Program.ModesMenu1["ManaHW"].Cast<Slider>().CurrentValue)
                {
                    W.Cast();
                }

            }
      }
      

         public static void Harass()
        {
            var Target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (Target == null) return;
            var useQ = ModesMenu1["HarassQ"].Cast<CheckBox>().CurrentValue;
            var Qp = Q.GetPrediction(Target);
            if (!Target.IsValid()) return;

            if (Q.IsInRange(Target) && Q.IsReady() && useQ && Qp.HitChance >= HitChance.High && !Target.IsInvulnerable && PlayerInstance.ManaPercent >= Program.ModesMenu1["HarassMana"].Cast<Slider>().CurrentValue)
            {
                    Q.Cast(Qp.CastPosition);
            }
        }
        

        public static void LJClear(AttackableUnit target, EventArgs args)
        {
          if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                var qMinions = EntityManager.MinionsAndMonsters.GetLineFarmLocation(EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, _Player.Position, Q.Range, true), Q.Width, (int)Q.Range);
                if (Q.IsReady() && qMinions.HitNumber >= ModesMenu2["MinionLC"].Cast<Slider>().CurrentValue &&
                    PlayerInstance.ManaPercent >= Program.ModesMenu2["ManaLQ"].Cast<Slider>().CurrentValue && ModesMenu2["FarmQ"].Cast<CheckBox>().CurrentValue)
                {
                    Q.Cast(qMinions.CastPosition);
                }
                if (W.IsReady() && ModesMenu2["FarmW"].Cast<CheckBox>().CurrentValue && PlayerInstance.ManaPercent >= Program.ModesMenu2["ManaLW"].Cast<Slider>().CurrentValue && EntityManager.MinionsAndMonsters.GetLaneMinions().Count(a => a.Distance(_Player.Position) <= _Player.GetAutoAttackRange()) >= ModesMenu2["MinionLC"].Cast<Slider>().CurrentValue)
                {
                    W.Cast();
                }  
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                var minions = EntityManager.MinionsAndMonsters.GetJungleMonsters(_Player.Position).ToList();
                if (!minions.Any()) return;
                var qFarm = EntityManager.MinionsAndMonsters.GetLineFarmLocation(minions, Q.Width, (int)Q.Range);
                if (qFarm.HitNumber >= 1 && Q.IsReady() &&
                    ModesMenu2["JungleQ"].Cast<CheckBox>().CurrentValue &&
                    PlayerInstance.ManaPercent >= Program.ModesMenu2["ManaJQ"].Cast<Slider>().CurrentValue)
                {
                    Q.Cast(qFarm.CastPosition);
                }
                if (W.IsReady() && minions.Count >= 1 &&
                    ModesMenu2["JungleW"].Cast<CheckBox>().CurrentValue &&
                   PlayerInstance.ManaPercent >= Program.ModesMenu2["ManaJW"].Cast<Slider>().CurrentValue)
                {
                    W.Cast();
                }

            }


        }


        public static void Flee()
        {

        }

        internal static void OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (W.IsReady() && !PlayerInstance.HasBuff("Spell Shield"))
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
        }
        
        internal static void ItemUsage()
        {
            var target = TargetSelector.GetTarget(550, DamageType.Physical); // 550 = Botrk.Range
            var hextech = TargetSelector.GetTarget(700, DamageType.Magical); // 700 = hextech.Range

            if (ModesMenu3["useYoumuu"].Cast<CheckBox>().CurrentValue && Program.Youmuu.IsOwned() && Program.Youmuu.IsReady())
            {
                if (ObjectManager.Player.CountEnemiesInRange(600) == 1)
                {
                    Program.Youmuu.Cast();
                }
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

        internal static void LastHit()
        {
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

        public static void KillSteal()
        {
            if (Program.ModesMenu1["KS"].Cast<CheckBox>().CurrentValue)
            {
                foreach (var enemy in EntityManager.Heroes.Enemies.Where(a => !a.IsDead && !a.IsZombie && a.Health > 0))
                {
                    if (enemy == null) return;
                    if (enemy.IsValidTarget(Q.Range) && enemy.HealthPercent <= 50)
                    {
                        var Qp = Q.GetPrediction(enemy);
                        if (DamageLib.QCalc(enemy) >= enemy.Health && Q.IsReady() && Q.IsInRange(enemy) && Program.ModesMenu1["KQ"].Cast<CheckBox>().CurrentValue && Qp.HitChance >= HitChance.High && !enemy.IsInvulnerable)
                        {

                            Q.Cast(Qp.CastPosition);
                        }
                    }
                }
            }
        }

    }
}