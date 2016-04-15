namespace Khappa_Zix.Modes
{
    using System;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;

    using Load;

    using SharpDX;

    internal class JumpsHandler : Load
    {
        public static bool Jumping;

        public static bool blockE = menu.Jump["block"].Cast<CheckBox>().CurrentValue;

        public static Obj_Shop bases;

        private static Vector3 Jumppoint1, Jumppoint2;

        public static int Edelay = menu.Jump["delay"].Cast<Slider>().CurrentValue;

        internal static void Obj_AI_Base_OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender is Obj_AI_Turret && args.Target.IsMe)
            {
                bases = ObjectManager.Get<Obj_Shop>().FirstOrDefault(o => o.IsAlly && o != null);
                if (player.HealthPercent <= menu.Jump["saveh"].Cast<Slider>().CurrentValue
                    && menu.Jump["save"].Cast<CheckBox>().CurrentValue && E.IsReady() && player.IsUnderEnemyturret()
                    && !player.IsDead)
                {
                    var pos = player.ServerPosition.Extend(bases.Position, E.Range).To3D();

                    if (!pos.IsUnderTurret())
                    {
                        E.Cast(pos);
                    }
                }
            }
        }

        public static Vector3 GetJumpPoint(AIHeroClient Qtarget, bool firstjump = true)
        {
            var target =
                EntityManager.Heroes.Enemies.OrderByDescending(
                    x =>
                    x.IsValidTarget(E.Range) && !x.IsValidTarget(Q.Range)
                    && x.Health <= GetQDamage(x) + player.GetAutoAttackDamage(x)).FirstOrDefault(x => x != null);
            var ally =
                EntityManager.Heroes.Allies.OrderByDescending(x => x.Health)
                    .FirstOrDefault(x => x.IsValidTarget(E.Range) && x != null);
            var finalPosition = player.Position.Extend(bases.Position, E.Range);
            var collFlags = NavMesh.GetCollisionFlags(finalPosition);

            if (player.IsUnderEnemyturret() && collFlags != CollisionFlags.Wall)
            {
                player.ServerPosition.Extend(bases.Position, E.Range).To3D();
            }

            if (firstjump)
            {
                switch (menu.Jump["1jump"].Cast<ComboBox>().CurrentValue)
                {
                    case 0:
                        {
                            if (bases != null)
                            {
                                player.ServerPosition.Extend(bases.Position, E.Range).To3D();
                            }
                        }
                        break;
                    case 1:
                        {
                            if (ally != null)
                            {
                                player.ServerPosition.Extend(ally.Position, E.Range).To3D();
                            }
                        }
                        break;
                    case 2:
                        {
                            player.ServerPosition.Extend(Game.CursorPos, E.Range).To3D();
                        }
                        break;
                    case 3:
                        {
                            if (target != null)
                            {
                                player.ServerPosition.Extend(target.Position, E.Range).To3D();
                            }
                        }
                        break;
                }
            }

            if (!firstjump)
            {
                switch (menu.Jump["2jump"].Cast<ComboBox>().CurrentValue)
                {
                    case 0:
                        {
                            if (bases != null)
                            {
                                player.ServerPosition.Extend(bases.Position, E.Range).To3D();
                            }
                        }
                        break;
                    case 1:
                        {
                            if (ally != null)
                            {
                                player.ServerPosition.Extend(ally.Position, E.Range).To3D();
                            }
                        }
                        break;
                    case 2:
                        {
                            player.ServerPosition.Extend(Game.CursorPos, E.Range).To3D();
                        }
                        break;
                    case 3:
                        {
                            if (target != null)
                            {
                                player.ServerPosition.Extend(target.Position, E.Range).To3D();
                            }
                        }
                        break;
                }
            }

            return player.ServerPosition.Extend(bases.Position, E.Range).To3D();
        }

        internal static void JumpLogic(EventArgs args)
        {
            if (!E.IsReady() || !EvolvedE || player.IsDead || player.IsRecalling() || !Combo.doubleJump)
            {
                return;
            }

            var targets = EntityManager.Heroes.Enemies.Where(x => x.IsValidTarget() && !x.IsInvulnerable && !x.IsZombie);
            var checkQKillable =
                targets.FirstOrDefault(
                    x =>
                    Vector3.Distance(player.ServerPosition, x.ServerPosition) < Q.Range - 25 && GetQDamage(x) > x.Health);

            if (checkQKillable != null && checkQKillable.IsValidTarget(Q.Range))
            {
                if (Q.IsReady() && E.IsReady())
                {
                    Jumppoint1 = GetJumpPoint(checkQKillable);
                    if (NavMesh.GetCollisionFlags(Jumppoint1) == CollisionFlags.Wall && blockE)
                    {
                        return;
                    }
                    E.Cast(Jumppoint1);
                    Q.Cast(checkQKillable);
                    Core.DelayAction(
                        () =>
                            {
                                Jumppoint2 = GetJumpPoint(checkQKillable, false);
                                if (NavMesh.GetCollisionFlags(Jumppoint2) == CollisionFlags.Wall && blockE)
                                {
                                    return;
                                }
                                E.Cast(Jumppoint2);
                            },
                        Edelay + Game.Ping);
                }
            }
        }

        internal static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (!EvolvedE || !Combo.doubleJump || !E.IsReady() || !(args.Target is AIHeroClient))
            {
                return;
            }

            if (args.Slot.Equals(SpellSlot.Q) && args.Target is AIHeroClient)
            {
                var target = (AIHeroClient)args.Target;
                var qdmg = GetQDamage(target);
                var dmg = (player.GetAutoAttackDamage(target) * 2) + qdmg;
                if (target.Health < dmg && target.Health > qdmg)
                {
                    args.Process = false;
                }
            }
        }

        internal static void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (!EvolvedE || !Combo.doubleJump || !E.IsReady() || !(args.Target is AIHeroClient)
                || !(target is AIHeroClient))
            {
                return;
            }

            if (args.Target.Health < GetQDamage((AIHeroClient)args.Target) && player.ManaPercent > 15)
            {
                args.Process = false;
            }
        }
    }
}