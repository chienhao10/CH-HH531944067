using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using System.Collections.Generic;
using System.Linq;
using System;

namespace GuTenTak.Tristana
{
    internal class Common : Program
    {
        public static float GetComboDamage(AIHeroClient target)
        {
            var damage = 0f;
            if (E.IsReady())
            {
                damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.E);
            }

            return damage;
        }

        //c
        public static void Combo()
        {
            var Target = TargetSelector.GetTarget(_Player.AttackRange, DamageType.Physical);
            if (Target == null) return;
            var useE = ModesMenu1["ComboE"].Cast<CheckBox>().CurrentValue;
            if (!Target.IsValid()) return;
            foreach (var ETarget in EntityManager.Heroes.Enemies)
            {
                if (E.IsReady() && useE && !Target.IsInvulnerable && ModesMenu1[ETarget.ChampionName].Cast<CheckBox>().CurrentValue)
                {
                    E.Cast(Target);
                }
            }
                    
            if (Target == null) return;
            var useQ = ModesMenu1["ComboQ"].Cast<CheckBox>().CurrentValue;
            if (!Target.IsValid()) return;
            if (Q.IsReady() && useQ && !Target.IsInvulnerable)
            {
                Q.Cast();
            }
        }
      

         public static void Harass()
        {
            var Target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (Target == null) return;
            var useQ = ModesMenu1["HarassQ"].Cast<CheckBox>().CurrentValue;
            var useE = ModesMenu1["HarassE"].Cast<CheckBox>().CurrentValue;
            if (!Target.IsValid()) return;


            if (ModesMenu1["HarassEF"].Cast<CheckBox>().CurrentValue)
            {
                var forcedtarget = CloseEnemies(Q.Range).Find (a => a.HasBuff("TristanaECharge"));
                if (forcedtarget != null)
                {
                    Orbwalker.ForcedTarget = forcedtarget;
                }
                else
                {
                    Orbwalker.ForcedTarget = null;
                }
            }

            if (Q.IsInRange(Target) && Q.IsReady() && useQ && !Target.IsInvulnerable)
            {
                Q.Cast();
            }

            if (E.IsReady() && useE && !Target.IsInvulnerable && PlayerInstance.ManaPercent >= Program.ModesMenu1["ManaHE"].Cast<Slider>().CurrentValue)
            {
                E.Cast(Target);
            }
        }

        public static void LaneClear()
        {
            var LaneCount =
            EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy,
                Player.Instance.ServerPosition,
                Player.Instance.AttackRange, false).Count();
            var LaneClear =
                EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy,
                    Player.Instance.ServerPosition,
                    Player.Instance.AttackRange).OrderByDescending(a => a.MaxHealth).FirstOrDefault();
            var LaneClearE =
                EntityManager.MinionsAndMonsters.GetLaneMinions()
                    .FirstOrDefault(m => m.IsValidTarget(Player.Instance.AttackRange) && m.HasBuff("TristanaECharge"));

            if (ModesMenu2["FarmEF"].Cast<CheckBox>().CurrentValue)
            {
                if (LaneClearE != null)
                {
                    Orbwalker.ForcedTarget = LaneClearE;
                }
                else
                {
                    Orbwalker.ForcedTarget = null;
                }
            }

            if (LaneCount == 0) return;

            if (ModesMenu2["FarmE"].Cast<CheckBox>().CurrentValue && PlayerInstance.ManaPercent >= Program.ModesMenu2["ManaLE"].Cast<Slider>().CurrentValue && E.IsReady())
            {
                    E.Cast(LaneClear);
            }

            if (ModesMenu2["FarmQ"].Cast<CheckBox>().CurrentValue && Q.IsReady())
            {
                    Q.Cast();
            }

        }

        public static void JungleClear()
        {
            var monsters =
    EntityManager.MinionsAndMonsters.GetJungleMonsters(ObjectManager.Player.ServerPosition,
        Q.Range)
        .FirstOrDefault(x => x.IsValidTarget(Q.Range));
            var JungleE =
     EntityManager.MinionsAndMonsters.GetJungleMonsters()
         .FirstOrDefault(m => m.IsValidTarget(Player.Instance.AttackRange) && m.HasBuff("TristanaECharge"));

            if (monsters == null) return;

            if (ModesMenu2["JungleEF"].Cast<CheckBox>().CurrentValue)
            {
                if (JungleE != null)
                {
                    Orbwalker.ForcedTarget = JungleE;
                }
                else
                {
                    Orbwalker.ForcedTarget = null;
                }
            }

            if (ModesMenu2["JungleQ"].Cast<CheckBox>().CurrentValue && Q.IsReady())
                {
                        Q.Cast();
                }

                if (ModesMenu2["JungleE"].Cast<CheckBox>().CurrentValue && PlayerInstance.ManaPercent >= Program.ModesMenu2["ManaJE"].Cast<Slider>().CurrentValue && E.IsReady())
                {
                        E.Cast(monsters);
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
                if (ObjectManager.Player.CountEnemiesInRange(PlayerInstance.AttackRange) == 1)
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

        internal static void Gapcloser_OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs gapcloser)
        {
            if (Program.ModesMenu3["AntiGapW"].Cast<CheckBox>().CurrentValue)
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
            if (Program.ModesMenu3["AntiGapR"].Cast<CheckBox>().CurrentValue)
            {
                if (gapcloser.End.Distance(ObjectManager.Player.Position) <= 200 && gapcloser.Sender.IsValidTarget(R.Range))
                        R.Cast(gapcloser.Sender);
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
            if (ModesMenu1["KS"].Cast<CheckBox>().CurrentValue)
            {
                foreach (var enemy in EntityManager.Heroes.Enemies.Where(a => !a.IsDead && !a.IsZombie && a.Health > 0))
                {
                    if (enemy == null) return;
                    if (enemy.IsValidTarget(R.Range) && enemy.HealthPercent <= 60)
                    {
                        var RCast = ModesMenu1["KR"].Cast<CheckBox>().CurrentValue;
                        if (DamageLib.RCalc(enemy) * 0.99 >= enemy.Health && R.IsInRange(enemy) && RCast && R.IsReady() && !enemy.IsInvulnerable)
                        {
                            R.Cast(enemy);
                        }
                    }
                }
            }

                var RECast = ModesMenu1["KER"].Cast<CheckBox>().CurrentValue;
            foreach (var enemy in from enemy in EntityManager.Heroes.Enemies.Where(e => e.IsInAutoAttackRange(e))
                                  where (DamageLib.RCalc(enemy) * 0.99) + (DamageLib.ECharge(enemy) * 0.99) >= enemy.Health
                                  select enemy)
            {
                if (enemy == null) return;
                if (RECast && R.IsReady() && enemy.HasBuff("TristanaECharge") && !enemy.IsInvulnerable)
                {
                    R.Cast(enemy);
                    return;
                }
            }
           }
        }
    }