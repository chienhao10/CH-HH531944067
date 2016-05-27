namespace KappAzir.Modes
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;

    using SharpDX;

    using Utility;

    internal class Insec
    {
        public static bool NormalInsec;

        public static bool NewInsec;

        public static float qtime;

        public static void Normal(Obj_AI_Base target, bool Combo = false)
        {
            Orbwalker.OrbwalkTo(Game.CursorPos);
            if (target == null || insectpos(target) == null || Common.Mana() > Player.Instance.Mana || !Azir.R.IsReady())
            {
                return;
            }

            var insecpos = target.ServerPosition.Extend(insectpos(target), -200).To3D();
            var rpos = Player.Instance.ServerPosition.Extend(insectpos(target), Azir.R.Range).To3D();

            if (target.IsValidTarget(Azir.R.Range))
            {
                if (Menus.JumperMenu.checkbox("flash") && Azir.Flash != null)
                {
                    var flashrange = Azir.Flash.Range + 250;
                    var enemies = EntityManager.Heroes.Enemies.Where(e => e.IsValidTarget(flashrange) && e.IsKillable());
                    var pred = Prediction.Position.PredictCircularMissileAoe(
                        enemies.Cast<Obj_AI_Base>().ToArray(),
                        flashrange,
                        Azir.R.Width + 25,
                        Azir.R.CastDelay,
                        Azir.R.Speed);
                    var castpos =
                        pred.OrderByDescending(p => p.GetCollisionObjects<AIHeroClient>().Length)
                            .FirstOrDefault(p => p.CollisionObjects.Contains(target));
                    if (castpos?.GetCollisionObjects<AIHeroClient>().Length > Player.Instance.CountEnemeis(Azir.R.Range))
                    {
                        Azir.Flash.Cast(castpos.CastPosition);
                    }
                }
                Azir.R.Cast(rpos);
            }
            else
            {
                if (Azir.Q.IsInRange(insecpos))
                {
                    Jumper.Jump(insecpos);
                }
            }

            Orbwalker.OrbwalkTo(insecpos);
        }

        public static void New()
        {
            Orbwalker.OrbwalkTo(Game.CursorPos);
            var target = TargetSelector.SelectedTarget;
            if (target == null || insectpos(target) == null || !Azir.R.IsReady() || Player.Instance.Mana < Common.Mana() || !target.IsValidTarget()
                || NormalInsec)
            {
                return;
            }

            var insecpos = target.ServerPosition.Extend(insectpos(target), -200).To3D();
            var qpos = Player.Instance.ServerPosition.Extend(insectpos(target), Azir.Q.Range - 200).To3D();
            var soldier = Orbwalker.AzirSoldiers.FirstOrDefault(s => s.IsAlly && s.IsInRange(target, 200));
            var ready = Azir.E.IsReady() && Azir.Q.IsReady() && Player.Instance.Mana > Azir.Q.Mana() + Azir.E.Mana();

            if (ready && soldier != null)
            {
                Core.DelayAction(
                    () =>
                        {
                            if (Azir.E.Cast(target))
                            {
                                Core.DelayAction(() => Azir.Q.Cast(qpos), Jumper.delay);
                                qtime = Game.Time;
                            }
                        },
                    100);
            }

            Orbwalker.OrbwalkTo(insecpos);
        }

        public static Vector3 insectpos(Obj_AI_Base target)
        {
            Common.tower = EntityManager.Turrets.Allies.FirstOrDefault(t => !t.IsDead && t.IsInRange(target, 1250));
            Common.ally =
                EntityManager.Heroes.Allies.OrderByDescending(a => a.CountAllies(Azir.R.Range))
                    .FirstOrDefault(a => !a.IsMe && a.IsKillable() && a.IsInRange(target, 1000));
            if (Common.tower != null)
            {
                return Common.tower.ServerPosition;
            }

            return Common.ally != null ? Common.ally.ServerPosition : Player.Instance.ServerPosition;
        }

        public static Vector3 insectpos()
        {
            Common.tower = EntityManager.Turrets.Allies.FirstOrDefault(t => t.IsValidTarget(1250));
            Common.ally =
                EntityManager.Heroes.Allies.OrderByDescending(a => a.CountEnemeis(Azir.R.Range)).FirstOrDefault(a => !a.IsMe && a.IsValidTarget(1000));
            if (Common.tower != null)
            {
                return Common.tower.ServerPosition;
            }

            return Common.ally != null ? Common.ally.ServerPosition : Player.Instance.ServerPosition;
        }
    }
}