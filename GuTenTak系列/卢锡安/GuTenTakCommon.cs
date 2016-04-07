using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using System.Collections.Generic;
using System.Linq;
using System;

namespace GuTenTak.Lucian
{
    internal class Common : Program
    {
        public static object HeroManager { get; private set; }

        // CastQ QWE Credits Lazy Lucian
        public static void CastQ()
        {
            var target = TargetSelector.SelectedTarget != null &&
                         TargetSelector.SelectedTarget.Distance(PlayerInstance) < 2000
                ? TargetSelector.SelectedTarget
                : TargetSelector.GetTarget(Q.Range, DamageType.Physical);

            if (!target.IsValidTarget(Q.Range))
                return;
            {
                Q.Cast(target);
            }
        }

        public static void CastExtendedQ()
        {
            var target = TargetSelector.SelectedTarget != null &&
                         TargetSelector.SelectedTarget.Distance(PlayerInstance) < 2000
                ? TargetSelector.SelectedTarget
                : TargetSelector.GetTarget(Q1.Range, DamageType.Physical);

            if (!target.IsValidTarget(Q1.Range))
                return;

            var predPos = Q1.GetPrediction(target);
            var minions =
                EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m.Distance(PlayerInstance) <= Q.Range);
            var champs = EntityManager.Heroes.Enemies.Where(m => m.Distance(PlayerInstance) <= Q.Range);
            var monsters =
                EntityManager.MinionsAndMonsters.Monsters.Where(m => m.Distance(PlayerInstance) <= Q.Range);
            {
                foreach (var minion in from minion in minions
                                       let polygon = new Geometry.Polygon.Rectangle(
                                           (Vector2)PlayerInstance.ServerPosition,
                                           PlayerInstance.ServerPosition.Extend(minion.ServerPosition, Q1.Range), 65f)
                                       where polygon.IsInside(predPos.CastPosition)
                                       select minion)
                {
                    Q.Cast(minion);
                }

                foreach (var champ in from champ in champs
                                      let polygon = new Geometry.Polygon.Rectangle(
                                          (Vector2)PlayerInstance.ServerPosition,
                                          PlayerInstance.ServerPosition.Extend(champ.ServerPosition, Q1.Range), 65f)
                                      where polygon.IsInside(predPos.CastPosition)
                                      select champ)
                {
                    Q.Cast(champ);
                }

                foreach (var monster in from monster in monsters
                                        let polygon = new Geometry.Polygon.Rectangle(
                                            (Vector2)PlayerInstance.ServerPosition,
                                            PlayerInstance.ServerPosition.Extend(monster.ServerPosition, Q1.Range), 65f)
                                        where polygon.IsInside(predPos.CastPosition)
                                        select monster)
                {
                    Q.Cast(monster);
                }
            }
        }
        // CastQ

        // CastW
        public static void CastWinRange()
        {
            var target = TargetSelector.GetTarget(500, DamageType.Magical);

            if (!target.IsValidTarget(500) ||
                (W1.GetPrediction(target).HitChance == HitChance.Collision) ||
                (W1.GetPrediction(target).HitChance < HitChance.Medium))
                return;
            {
                W.Cast(target);
            }
        }

        public static void CastWcombo()
        {
            var target = TargetSelector.GetTarget(W.Range, DamageType.Magical);

            if (!target.IsValidTarget(W.Range) ||
                (W.GetPrediction(target).HitChance == HitChance.Collision) ||
                (W.GetPrediction(target).HitChance < HitChance.Medium) ||
                Q.IsReady())
                return;
            {
                W.Cast(target);
            }
        }
        // Cast W

        // Cast E
        public static void CastEcombo()
        {
            var target = TargetSelector.GetTarget(E.Range + Q1.Range, DamageType.Physical);
            var direction1 = (PlayerInstance.ServerPosition - target.ServerPosition).To2D().Normalized();
            var direction2 = (target.ServerPosition - PlayerInstance.ServerPosition).To2D().Normalized();
            const int maxDistance = 475;
            const int stepSize = 20;

            if (target.HealthPercent <= PlayerInstance.HealthPercent &&
                (PlayerInstance.HealthPercent > 30 ||
                 GetComboDamage(target) >= target.Health) &&
                target.Distance(PlayerInstance) <= 975)
            {
                for (var step = 0f; step < 360; step += stepSize)
                {
                    var currentAngle = step * (float)Math.PI / 120;
                    var currentCheckPoint = target.ServerPosition.To2D() +
                                            maxDistance * direction2.Rotated(currentAngle);

                    if (!IsSafePosition((Vector3)currentCheckPoint) ||
                        NavMesh.GetCollisionFlags(currentCheckPoint).HasFlag(CollisionFlags.Wall) ||
                        NavMesh.GetCollisionFlags(currentCheckPoint).HasFlag(CollisionFlags.Building))
                        continue;
                    {
                        E.Cast((Vector3)currentCheckPoint);
                    }
                }
            }

            else if (target.HealthPercent > PlayerInstance.HealthPercent &&
                     target.Distance(PlayerInstance) <= 400)
            {
                for (var step = 0f; step < 360; step += stepSize)
                {
                    var currentAngle = step * (float)Math.PI / 120;
                    var currentCheckPoint = target.ServerPosition.To2D() +
                                            maxDistance * direction1.Rotated(currentAngle);

                    if (!IsSafePosition((Vector3)currentCheckPoint) ||
                        NavMesh.GetCollisionFlags(currentCheckPoint).HasFlag(CollisionFlags.Wall) ||
                        NavMesh.GetCollisionFlags(currentCheckPoint).HasFlag(CollisionFlags.Building))
                        continue;
                    {
                        E.Cast((Vector3)currentCheckPoint);
                    }
                }
            }
        }

        public static void CastEmouse()
        {
            var target = TargetSelector.GetTarget(E.Range + Q1.Range, DamageType.Physical);
            var direction = (Game.CursorPos - PlayerInstance.ServerPosition).To2D().Normalized();
            const int maxDistance = 475;
            const int stepSize = 20;

            if (!(target.Distance(PlayerInstance) <= 975)) return;
            for (var step = 0f; step < 360; step += stepSize)
            {
                var currentAngle = step * (float)Math.PI / 120;
                var currentCheckPoint = target.ServerPosition.To2D() +
                                        maxDistance * direction.Rotated(currentAngle);

                if (!IsSafePosition((Vector3)currentCheckPoint) ||
                    NavMesh.GetCollisionFlags(currentCheckPoint).HasFlag(CollisionFlags.Wall) ||
                    NavMesh.GetCollisionFlags(currentCheckPoint).HasFlag(CollisionFlags.Building))
                    continue;
                {
                    E.Cast((Vector3)currentCheckPoint);
                }
            }
        }

        private void chaseE(AIHeroClient target)
        {
            if (Player.Instance.CountEnemiesInRange(600) == 0 && Player.Instance.CountEnemiesInRange(600 + E.Range) > 0)
            {
                E.Cast(target);
            }
        }


        //CastE

        public static List<AIHeroClient> GetLowaiAiHeroClients(Vector3 position, float range)
        {
            return
                EntityManager.Heroes.Enemies.Where(
                    hero => hero.IsValidTarget() && (hero.Distance(position) <= 1000) && hero.HealthPercent <= 15)
                    .ToList();
        }

        public static bool IsSafePosition(Vector3 position)
        {
            var underTurret =
                EntityManager.Turrets.Enemies.FirstOrDefault(
                    turret => !turret.IsDead && turret.Distance(position) <= 950);
            var allies = EntityManager.Heroes.Allies.Count(
                allied => !allied.IsDead && allied.Distance(position) <= 800);
            var enemies = position.CountEnemiesInRange(1000);
            var lhEnemies = GetLowaiAiHeroClients(position, 800).Count();
            if (underTurret != null) return false;

            if (enemies == 1)
            {
                return true;
            }
            return allies > enemies - lhEnemies;
        }

        public static float GetComboDamage(AIHeroClient target)
        {
            var damage = 0f;
            if (Q.IsReady())
            {
                damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q);
            }
            if (W.IsReady())
            {
                damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.W);
            }
            if (E.IsReady())
            {
                damage += ObjectManager.Player.GetAutoAttackDamage(target) * 1.4f;
            }

            return damage;
        }

        public static bool HasPassiveBuff()
        {
            return ObjectManager.Player.HasBuff("lucianpassivebuff");
        }


        public static void Combo()
        {
            if (ModesMenu1["LogicAA"].Cast<ComboBox>().CurrentValue == 0)
            {
                var target = TargetSelector.SelectedTarget != null &&
                           TargetSelector.SelectedTarget.Distance(ObjectManager.Player) < 2000
                  ? TargetSelector.SelectedTarget
                  : TargetSelector.GetTarget(Q1.Range + E.Range, DamageType.Physical);

                if (target == null ||

                    (ModesMenu1["CWeaving"].Cast<CheckBox>().CurrentValue &&
                    (Program.PassiveUp || HasPassiveBuff())) ||
                    Orbwalker.IsAutoAttacking ||
                    ObjectManager.Player.IsDashing() ||
                    target.IsZombie || target.IsInvulnerable ||
                    target.HasBuffOfType(BuffType.Invulnerability))
                    return;

                if (Q.IsReady() && ModesMenu1["ComboQ"].Cast<CheckBox>().CurrentValue)
                {
                        CastQ();
                        CastExtendedQ();
                }

                if (E.IsReady() && ModesMenu1["ComboE"].Cast<CheckBox>().CurrentValue)
                {
                    if (ModesMenu1["LogicE"].Cast<ComboBox>().CurrentValue == 1)
                    {
                        CastEcombo();
                    }
                    if (ModesMenu1["LogicE"].Cast<ComboBox>().CurrentValue == 0)
                    {
                        CastEmouse();
                    }
                    if (ModesMenu1["LogicE"].Cast<ComboBox>().CurrentValue == 2)
                    {
                        Player.CastSpell(SpellSlot.E, Game.CursorPos);
                    }
                }

                if (W.IsReady() && ModesMenu1["ComboW"].Cast<CheckBox>().CurrentValue && ObjectManager.Player.ManaPercent > ModesMenu1["ManaCW"].Cast<Slider>().CurrentValue)
                {
                    if (ModesMenu1["WColision"].Cast<ComboBox>().CurrentValue == 0)
                    {
                        if (ModesMenu1["LogicW"].Cast<ComboBox>().CurrentValue == 0)
                        {
                            CastWinRange();
                        }
                        if (ModesMenu1["LogicW"].Cast<ComboBox>().CurrentValue == 1)
                        {
                            CastWcombo();
                        }
                    }
                    else
                    {
                        W.Cast(target);
                    }
                }

            }


            }

        public static void Harass()
        {
            //Harass
            var target = TargetSelector.SelectedTarget != null &&
               TargetSelector.SelectedTarget.Distance(ObjectManager.Player) < 2000
      ? TargetSelector.SelectedTarget
      : TargetSelector.GetTarget(W.Range, DamageType.Physical);

            if (target == null ||
                (ModesMenu1["HWeaving"].Cast<CheckBox>().CurrentValue && PassiveUp) ||
                Orbwalker.IsAutoAttacking ||
                target.IsZombie ||
                ObjectManager.Player.IsDashing())
                return;


            if (Q.IsReady() &&
                ObjectManager.Player.ManaPercent > ModesMenu1["HarassMana"].Cast<Slider>().CurrentValue)
            {
                if (ModesMenu1["HarassQ"].Cast<CheckBox>().CurrentValue)
                {
                    CastQ();
                }
                if (ModesMenu1["HarassQext"].Cast<CheckBox>().CurrentValue)
                {
                    CastExtendedQ();
                }
            }

            if (!W.IsReady() ||
                ObjectManager.Player.ManaPercent < ModesMenu1["ManaHW"].Cast<Slider>().CurrentValue)
                return;
            {
                if (ModesMenu1["HarassW"].Cast<CheckBox>().CurrentValue)
                {
                    if (ModesMenu1["WColision"].Cast<ComboBox>().CurrentValue == 0)
                    {
                        CastWinRange();
                    }
                    if (ModesMenu1["WColision"].Cast<ComboBox>().CurrentValue == 1)
                    {
                        CastWcombo();
                    }
                }
            }
        }
        public static void LJClear(AttackableUnit target, EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            { 

                if (Q.IsReady() && ModesMenu2["FarmQ"].Cast<CheckBox>().CurrentValue || PlayerInstance.HasBuff("lucianpassivebuff") && Program._Player.ManaPercent >= Program.ModesMenu2["ManaLQ"].Cast<Slider>().CurrentValue)
                {
                    var minions =
                        EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy,
                            PlayerInstance.Position, Q.Range);
                    var aiMinions = minions as Obj_AI_Minion[] ?? minions.ToArray();

                    foreach (var m in from m in aiMinions
                                      let p = new Geometry.Polygon.Rectangle((Vector2)PlayerInstance.ServerPosition,
    PlayerInstance.ServerPosition.Extend(m.ServerPosition, Q1.Range), 65)
                                      where aiMinions.Count(x =>
    p.IsInside(x.ServerPosition)) >= 2
                                      select m)
                    {
                        Q.Cast(m);
                        break;
                    }
                }
                if (W.IsReady() && ModesMenu2["FarmW"].Cast<CheckBox>().CurrentValue || PlayerInstance.HasBuff("lucianpassivebuff") && Program._Player.ManaPercent >= Program.ModesMenu2["ManaLW"].Cast<Slider>().CurrentValue)
                {
                    var minions =
        EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy,
            PlayerInstance.Position, 500)
            .FirstOrDefault(x => x.IsValidTarget(500));
                    if (minions != null)
                        W.Cast(minions);
                }
            }

            // LaneClear
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                if (Q.IsReady() && ModesMenu2["JungleQ"].Cast<CheckBox>().CurrentValue || PlayerInstance.HasBuff("lucianpassivebuffmstl") && Program._Player.ManaPercent >= Program.ModesMenu2["ManaJQ"].Cast<Slider>().CurrentValue)
                {
                    var monster =
                        EntityManager.MinionsAndMonsters.GetJungleMonsters(PlayerInstance.ServerPosition, Q.Range)
                            .FirstOrDefault(x => x.IsValidTarget(Q.Range));
                    if (monster != null)
                        Q.Cast(monster);
                }

                if (W.IsReady() && ModesMenu2["JungleW"].Cast<CheckBox>().CurrentValue || PlayerInstance.HasBuff("lucianpassivebuff") && Program._Player.ManaPercent >= Program.ModesMenu2["ManaJW"].Cast<Slider>().CurrentValue)
                {
                    var monster =
                        EntityManager.MinionsAndMonsters.GetJungleMonsters(PlayerInstance.ServerPosition, 600)
                            .FirstOrDefault(x => x.IsValidTarget());
                    if (monster != null && !PassiveUp)
                        W.Cast(monster.ServerPosition);

                }
            }
        }
      
        /*
        public static void LaneClear()
        {

            if (Q.IsReady() && ModesMenu2["FarmQ"].Cast<CheckBox>().CurrentValue || PlayerInstance.HasBuff("lucianpassivebuff") && Program._Player.ManaPercent >= Program.ModesMenu2["ManaLQ"].Cast<Slider>().CurrentValue)
            {
                var minions =
                    EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy,
                        PlayerInstance.Position, Q.Range);
                var aiMinions = minions as Obj_AI_Minion[] ?? minions.ToArray();

                foreach (var m in from m in aiMinions
                                  let p = new Geometry.Polygon.Rectangle((Vector2)PlayerInstance.ServerPosition,
PlayerInstance.ServerPosition.Extend(m.ServerPosition, Q1.Range), 65)
                                  where aiMinions.Count(x =>
p.IsInside(x.ServerPosition)) >= 2
                                  select m)
                {
                    Q.Cast(m);
                    break;
                }
            }
            if (PlayerInstance.IsAttackingPlayer && W.IsReady() && ModesMenu2["FarmW"].Cast<CheckBox>().CurrentValue || PlayerInstance.HasBuff("lucianpassivebuff") && Program._Player.ManaPercent >= Program.ModesMenu2["ManaLW"].Cast<Slider>().CurrentValue)
            {
                var minions =
    EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy,
        PlayerInstance.Position, 500)
        .FirstOrDefault(x => x.IsValidTarget(500));
                if (minions != null)
                    W.Cast(minions);
            }
        }
        public static void JungleClear()
        {
            if (Q.IsReady() && ModesMenu2["JungleQ"].Cast<CheckBox>().CurrentValue || PlayerInstance.HasBuff("lucianpassivebuffmstl") && Program._Player.ManaPercent >= Program.ModesMenu2["ManaJQ"].Cast<Slider>().CurrentValue)
            {
                var monster =
                    EntityManager.MinionsAndMonsters.GetJungleMonsters(PlayerInstance.ServerPosition, Q.Range)
                        .FirstOrDefault(x => x.IsValidTarget(Q.Range));
                if (monster != null)
                    Q.Cast(monster);
            }

            if (PlayerInstance.IsAttackingPlayer && W.IsReady() && ModesMenu2["JungleW"].Cast<CheckBox>().CurrentValue || PlayerInstance.HasBuff("lucianpassivebuff") && Program._Player.ManaPercent >= Program.ModesMenu2["ManaJW"].Cast<Slider>().CurrentValue)
            {
                var monster =
                    EntityManager.MinionsAndMonsters.GetJungleMonsters(PlayerInstance.ServerPosition, 600)
                        .FirstOrDefault(x => x.IsValidTarget());
                if (monster != null && !PassiveUp)
                    W.Cast(monster.ServerPosition);

            }
        }*/

        internal static void aaCombo(AttackableUnit target, EventArgs args)
        {
            if (ModesMenu1["LogicAA"].Cast<ComboBox>().CurrentValue == 1 && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (target == null || !(target is AIHeroClient) || target.IsDead || target.IsInvulnerable || !target.IsEnemy || target.IsPhysicalImmune || target.IsZombie)
                    return;

                var enemy = target as AIHeroClient;
                if (enemy == null)
                    return;

                if (ModesMenu1["ComboE"].Cast<CheckBox>().CurrentValue && E.IsReady())
                {
                    if (ModesMenu1["LogicE"].Cast<ComboBox>().CurrentValue == 1)
                    {
                        CastEcombo();
                    }
                    if (ModesMenu1["LogicE"].Cast<ComboBox>().CurrentValue == 0)
                    {
                        CastEmouse();
                    }
                    if (ModesMenu1["LogicE"].Cast<ComboBox>().CurrentValue == 2)
                    {
                        Player.CastSpell(SpellSlot.E, Game.CursorPos);
                    }
                }
                else if (ModesMenu1["ComboQ"].Cast<CheckBox>().CurrentValue && Q.IsReady())
                {
                    Q.Cast(enemy);
                    castQ1(enemy);
                }
                else if (ModesMenu1["ComboW"].Cast<CheckBox>().CurrentValue && W.IsReady())
                {
                    if (ModesMenu1["WColision"].Cast<ComboBox>().CurrentValue == 0)
                    {
                        W.Cast(enemy);
                    }
                    if (ModesMenu1["WColision"].Cast<ComboBox>().CurrentValue == 1)
                    {
                        W.Cast(W.GetPrediction(enemy).CastPosition);
                    }

                }
            }
        }
        

        public static void Flee()
        {
            if (ModesMenu3["FleeE"].Cast<CheckBox>().CurrentValue)
            {
                Player.CastSpell(SpellSlot.E, Game.CursorPos);
            }
            if (ModesMenu3["FleeR"].Cast<CheckBox>().CurrentValue && Program._Player.ManaPercent <= Program.ModesMenu3["ManaFlR"].Cast<Slider>().CurrentValue)
            {

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
                if (ObjectManager.Player.CountEnemiesInRange(1500) == 1)
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
                    var diffGapCloser = gapcloser.End - gapcloser.Start;
                    E.Cast(ObjectManager.Player.ServerPosition + diffGapCloser);
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
                            if (Q.IsReady() && Q.IsInRange(enemy) && Program.ModesMenu1["KQ"].Cast<CheckBox>().CurrentValue && !enemy.IsInvulnerable)
                            {
                                Q.Cast(enemy);
                                castQ1(enemy);
                            }
                        }

                        if (DamageLib.WCalc(enemy) >= enemy.Health)
                        {
                            if (W.IsReady() && W.IsInRange(enemy) && Program.ModesMenu1["KW"].Cast<CheckBox>().CurrentValue && !enemy.IsInvulnerable)
                            {
                                W.Cast(enemy);
                            }
                        }

                            if (DamageLib.RCalc(enemy) * 8 >= enemy.Health)
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

        /*
        public static new void AutoQ()
        {
            var target = TargetSelector.GetTarget(Q1.Range, DamageType.Mixed);
            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                return;
            if (target == null || !(target is AIHeroClient) || target.IsDead || target.IsInvulnerable || !target.IsEnemy || target.IsPhysicalImmune || target.IsZombie)
                return;

            var enemy = target as AIHeroClient;
            if (enemy == null)
                return;
            {
                if (Program.ModesMenu1["AutoHarass"].Cast<CheckBox>().CurrentValue && Program._Player.ManaPercent >= Program.ModesMenu1["ManaAuto"].Cast<Slider>().CurrentValue)
                {
                    Q.Cast(enemy);
                    castQ1(enemy);
                }
            }
        }
        */

        public static bool dashToQ1(AIHeroClient enemy)
        {
            int delay = (int)(E.Range / (700 * Player.Instance.MoveSpeed));
            List<Vector2> castTo = new List<Vector2>();
            foreach (var e in EntityManager.Heroes.Enemies.Where(t => t.IsInRange(enemy, Q1.Range - Q.Range) && t.HealthPercent > 0 && !t.IsInvulnerable))
            {
                for (int i = 0; i < Q.Range; i += 25)
                {
                    var targetPos = Prediction.Position.PredictUnitPosition(e, delay).Extend(Prediction.Position.PredictUnitPosition(enemy, delay), -1 * i);
                    if (Math.Abs(targetPos.Distance(Player.Instance) - E.Range) < 50)
                    {
                        castTo.Add(targetPos);
                    }
                }
            }
            foreach (var e in EntityManager.MinionsAndMonsters.CombinedAttackable.Where(t => t.IsInRange(Player.Instance, Q.Range) && t.HealthPercent > 0 && !t.IsInvulnerable))
            {
                for (int i = 0; i < Q.Range; i += 25)
                {
                    var targetPos = Prediction.Position.PredictUnitPosition(e, delay).Extend(Prediction.Position.PredictUnitPosition(enemy, delay), -1 * i);
                    if (Math.Abs(targetPos.Distance(Player.Instance) - E.Range) < 50)
                    {
                        castTo.Add(targetPos);
                    }
                }
            }
            if (castTo.Count == 0)
                return false;
            return dashTo(castTo);
        }

        public static bool dashTo(List<Vector2> castTo)
        {
            List<Vector2> castTo2 = new List<Vector2>();
            castTo2.AddRange(castTo);
            foreach (var pos in castTo)
            {
                if (pos.ToNavMeshCell().CollFlags.HasFlag(CollisionFlags.Building) || pos.ToNavMeshCell().CollFlags.HasFlag(CollisionFlags.Wall))
                {
                    castTo2.Remove(pos);
                    continue;
                }
                foreach (var t in EntityManager.Turrets.Enemies)
                {
                    if (pos.IsInRange(t.Position, t.GetAutoAttackRange() + 10))
                        castTo2.Remove(pos);
                }
            }
            if (castTo2.Count == 0)
                return false;
            E.Cast(castTo2.First().To3D());
            return true;
        }

        public static void castR()
        {
            var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
            if (Program.RTarget != null && Program.RTarget.Health > 0 && Program.RTarget.Distance(Player.Instance) < R.Range && !Program.RTarget.IsZombie && !Program.RTarget.IsInvulnerable)
            {
                target = Program.RTarget;
            }
            else
            {
                Program.RCastToPosition = target.Position;
                Program.RTarget = target;
            }
            if (target == null)
                return;

            Vector2 TRCast = Program.RCastToPosition.To2D();
            Vector2 MyRCast = Program.MyRCastPosition.To2D();
            Vector2 TPos = target.Position.To2D();
            Vector2 x = new Vector2(TPos.X - TRCast.X, TPos.Y - TRCast.Y);
            Vector2 Y = new Vector2(MyRCast.X + x.X, MyRCast.Y + x.Y);
            Vector2 best = new Vector2(Y.X, Y.Y);
            for (int i = -100; i <= 1000; i += 25)
            {
                Vector2 next = TPos.Extend(Y, i);
                if (next.Distance(TPos) < R.Range * 0.75)
                {
                    if (next.Distance(TPos) > best.Distance(TPos))
                    {
                        best = next;
                    }
                }
                if (best.X == 0 || best.Y == 0 || best.Distance(TPos) > R.Range * 0.75)
                {
                    best = next;
                    continue;
                }
            }
            if (best.X == 0 || best.Y == 0)
            {
                return;
            }
            Player.IssueOrder(GameObjectOrder.MoveTo, best.To3D());
        }

        public static bool Q1hits(AIHeroClient target, Obj_AI_Base via)
        {
            for (int i = 0; i < Q.Range; i += 25)
            {
                var targetPos = Prediction.Position.PredictUnitPosition(via, Q1.CastDelay).Extend(Prediction.Position.PredictUnitPosition(target, Q1.CastDelay), -1 * i);
                if (Math.Abs(targetPos.Distance(Player.Instance) - E.Range) < 50)
                {
                    return true;
                }
            }
            return false;
        }

        public static void castQ1(AIHeroClient target)
        {

            foreach (var e in EntityManager.Heroes.Enemies.Where(t => t.IsInRange(Player.Instance, Q.Range) && t.HealthPercent > 0 && !t.IsInvulnerable))
            {
                if (Q.IsReady() && Q1hits(target, e))
                {
                    Q.Cast(e);
                }
            }
            foreach (var e in EntityManager.MinionsAndMonsters.CombinedAttackable.Where(t => t.IsInRange(Player.Instance, Q.Range) && t.HealthPercent > 0 && !t.IsInvulnerable))
            {
                if (Q.IsReady() && Q1hits(target, e))
                {
                    Q.Cast(e);
                }
            }
        }

        public static void castE()
        {
            if (ModesMenu1["LogicE"].Cast<ComboBox>().CurrentValue == 0)
            {
                List<Vector2> castTo = new List<Vector2>();
                int Qacc = 40;
                for (int i = 0; i < Qacc; i++)
                {
                    double rad = i / Qacc * 2 * Math.PI;
                    Vector2 onCircle = new Vector2(Player.Instance.Position.X + 300 * (float)Math.Cos(rad), Player.Instance.Position.Y + 300 * (float)Math.Sin(rad));
                    castTo.Add(onCircle);
                }
                foreach (var pos in castTo)
                {
                    foreach (var t in EntityManager.Turrets.Enemies)
                    {
                        if (pos.IsInRange(t.Position, t.GetAutoAttackRange() + 10))
                            castTo.Remove(pos);
                    }
                }
                List<Vector2> goodOnes = new List<Vector2>();
                foreach (var pos in castTo)
                {
                    bool good = true;
                    foreach (var e in EntityManager.Heroes.Enemies.Where(t => t.IsInRange(Player.Instance, 850) && t.HealthPercent > 0))
                    {
                        if (!(Math.Abs(e.Distance(pos) - 550) < 150) && pos.CountEnemiesInRange(550) - (pos.CountAlliesInRange(450) + 1) <= 1)
                        {
                            good = false;
                        }
                    }
                    if (good)
                    {
                        goodOnes.Add(pos);
                    }
                }
                goodOnes.OrderBy(t => t.CountEnemiesInRange(550));
                foreach (var pos in goodOnes)
                {
                    if (pos.ToNavMeshCell().CollFlags.HasFlag(CollisionFlags.Grass))
                    {
                        Player.CastSpell(SpellSlot.E, pos.To3D());
                        return;
                    }
                }

                if (goodOnes.Count > 0)
                    Player.CastSpell(SpellSlot.E, goodOnes.OrderBy(t => t.Distance(Game.CursorPos2D)).FirstOrDefault().To3D());
                else if (castTo.Count > 0)
                    Player.CastSpell(SpellSlot.E, castTo.OrderBy(t => t.Distance(Game.CursorPos2D)).FirstOrDefault().To3D());
                else
                    Player.CastSpell(SpellSlot.E, Game.CursorPos);
            }
            if (ModesMenu1["LogicE"].Cast<ComboBox>().CurrentValue == 2)
            {
                Player.CastSpell(SpellSlot.E, Game.CursorPos);
            }
        }

    }
}