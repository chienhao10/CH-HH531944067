namespace KappAzir.Modes
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    using SharpDX;

    using Utility;

    internal static class Combo
    {
        public static void Execute()
        {
            var menu = Menus.ComboMenu;
            var QS = Orbwalker.ValidAzirSoldiers.Count(s => s.IsAlly) >= menu.slider("QS");
            var Q = QS && menu.checkbox("Q") && Azir.Q.IsReady();
            var Qaoe = menu.checkbox("Qaoe");
            var W = menu.checkbox("W") && Azir.W.IsReady();
            var E = menu.checkbox("E") && Azir.E.IsReady();
            var R = menu.checkbox("R") && Azir.R.IsReady();
            var Wsave = menu.checkbox("Wsave") && Azir.W.Handle.Ammo < 2;
            var Wlimit = menu.slider("WS") >= Orbwalker.ValidAzirSoldiers.Count(s => s.IsAlly);
            var target = TargetSelector.GetTarget(Azir.Q.Range + 25, DamageType.Magical);

            if (target == null || !target.IsKillable())
            {
                return;
            }

            if (W && Wlimit)
            {
                if (Wsave)
                {
                    return;
                }

                if (target.IsValidTarget(Azir.W.Range))
                {
                    var pred = Azir.W.GetPrediction(target);
                    Azir.W.Cast(pred.CastPosition);
                }
                if (menu.checkbox("Q") && !target.IsValidTarget(Azir.W.Range) && Azir.Q.IsReady()
                    && Player.Instance.Mana > Azir.Q.Mana() + Azir.W.Mana() && target.IsValidTarget(Azir.Q.Range - 25) && menu.checkbox("WQ"))
                {
                    var p = Player.Instance.Position.Extend(target.Position, Azir.W.Range);
                    Azir.W.Cast(p.To3D());
                }
            }

            if (Orbwalker.AzirSoldiers.Count(s => s.IsAlly) == 0)
            {
                return;
            }

            if (Q)
            {
                var predQ = Azir.Q.GetPrediction(target);
                if (predQ.HitChance >= HitChance.High || target.IsCC())
                {
                    Azir.Q.Cast(predQ.CastPosition);
                }

                if (Q && E && Player.Instance.Mana > Azir.Q.Mana() + Azir.E.Mana() && target.Ehit(predQ.CastPosition)
                    && predQ.HitChance >= HitChance.Medium)
                {
                    if ((target.CountEnemeis(750) >= menu.slider("Esafe")) || (menu.slider("EHP") >= Player.Instance.HealthPercent)
                        || (!menu.checkbox("Edive") && target.IsUnderHisturret()))
                    {
                        return;
                    }
                    if (Azir.E.Cast(predQ.CastPosition))
                    {
                        Azir.Q.Cast(predQ.CastPosition);
                    }
                }

                if (Qaoe)
                {
                    var enemies = EntityManager.Heroes.Enemies.Where(e => e.IsValidTarget(Azir.Q.Range) && e.IsKillable());
                    var pred = Prediction.Position.PredictCircularMissileAoe(
                        enemies.Cast<Obj_AI_Base>().ToArray(),
                        Azir.Q.Range,
                        (int)Orbwalker.AzirSoldierAutoAttackRange,
                        Azir.Q.CastDelay,
                        Azir.Q.Speed);
                    var castpos =
                        pred.OrderByDescending(p => p.GetCollisionObjects<AIHeroClient>().Length)
                            .FirstOrDefault(p => p.CollisionObjects.Contains(target));
                    if (castpos?.GetCollisionObjects<AIHeroClient>().Length > 1)
                    {
                        Azir.Q.Cast(castpos.CastPosition);
                    }
                }
            }

            if (E && target.Ehit())
            {
                if ((target.CountEnemeis(750) >= menu.slider("Esafe")) || (menu.slider("EHP") >= Player.Instance.HealthPercent)
                    || (menu.checkbox("Edive") && target.IsUnderEnemyturret() && target.IsUnderHisturret()))
                {
                    return;
                }

                var time = Player.Instance.Distance(target) / Azir.E.Speed;
                var killable = target.Damage() >= Prediction.Health.GetPrediction(target, (int)time);
                if (menu.checkbox("Ekill") && killable && Player.Instance.Mana >= Common.Mana())
                {
                    Azir.E.Cast(target);
                }
                else
                {
                    Azir.E.Cast(target);
                }
            }
            if (R)
            {
                var Raoe = Player.Instance.CountEnemeis(Azir.R.Range) >= menu.slider("Raoe")
                           || Player.Instance.CountEnemeis(Azir.R.Width) >= menu.slider("Raoe");

                if (target.IsValidTarget(Azir.R.Range - 25))
                {
                    if ((menu.checkbox("Rkill") && Azir.R.GetDamage(target) >= Prediction.Health.GetPrediction(target, Azir.R.CastDelay))
                        || (menu.checkbox("Rsave") && menu.slider("RHP") >= Player.Instance.HealthPercent) || (Raoe))
                    {
                        Azir.R.Cast(target.Rpos());
                    }
                }
            }

            if (menu.checkbox("insec") && target.IsKillable() && !target.IsValidTarget(Azir.R.Range))
            {
                if (target.CountEnemeis(750) >= menu.slider("Esafe") || menu.slider("EHP") >= Player.Instance.HealthPercent)
                {
                    return;
                }

                var time = Azir.R.CastDelay + Azir.Q.CastDelay + Azir.E.CastDelay;
                var kill = target.Damage() + 100 > Prediction.Health.GetPrediction(target, time);
                if (kill)
                {
                    Insec.Normal(target);
                }
            }
        }

        public static Vector3 Rpos(this Obj_AI_Base target)
        {
            Common.ally =
                EntityManager.Heroes.Allies.OrderByDescending(a => a.CountAllies(Azir.R.Range))
                    .FirstOrDefault(a => a.IsKillable() && a.IsValidTarget(1250) && !a.IsMe);
            Common.tower = EntityManager.Turrets.Allies.FirstOrDefault(s => s.IsValidTarget(1250));
            if (Common.tower != null)
            {
                return Player.Instance.ServerPosition.Extend(Common.tower.ServerPosition, Azir.R.Range).To3D();
            }
            return Common.ally != null
                       ? Player.Instance.ServerPosition.Extend(Common.ally.ServerPosition, Azir.R.Range).To3D()
                       : Azir.R.GetPrediction(target).CastPosition;
        }
    }
}