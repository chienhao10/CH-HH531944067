namespace KappAzir.Modes
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;

    using Mario_s_Lib;
    using static Menus;
    using static SpellsManager;

    /// <summary>
    /// This mode will run when the key of the orbwalker is pressed
    /// </summary>
    internal class LaneClear
    {
        /// <summary>
        /// Put in here what you want to do when the mode is running
        /// </summary>
        public static void Execute()
        {
            var minion = EntityManager.MinionsAndMonsters.GetLaneMinions().FirstOrDefault(m => m.IsValidTarget(W.Range) && m != null);
            var maxminion = EntityManager.MinionsAndMonsters.GetLaneMinions().OrderByDescending(m => m.MaxHealth).FirstOrDefault(m => m != null && m.IsValidTarget(W.Range));
            var minions = EntityManager.MinionsAndMonsters.EnemyMinions;
            var turret = EntityManager.Turrets.Enemies.FirstOrDefault(t => t.IsValidTarget(W.Range));

            if (minion == null || minions == null || !minions.Any())
            {
                return;
            }
            if (LaneClearMenu.GetCheckBoxValue("qUse") && Q.IsReady() && Orbwalker.AzirSoldiers.Count(s => s.IsAlly) >= 1)
            {
                Q.SourcePosition = Orbwalker.AzirSoldiers.FirstOrDefault()?.ServerPosition;
                Q.RangeCheckSource = Player.Instance.ServerPosition;
                if (minion.GetDamage(SpellSlot.Q) >= minion.TotalShieldHealth())
                {
                    Q.Cast(Q.GetPrediction(minion).CastPosition);
                }

                var location =
                    Prediction.Position.PredictCircularMissileAoe(minions.Cast<Obj_AI_Base>().ToArray(), Q.Range, Q.Width, Q.CastDelay, Q.Speed)
                        .OrderByDescending(r => r.GetCollisionObjects<Obj_AI_Minion>().Length)
                        .FirstOrDefault(r => r.GetCollisionObjects<Obj_AI_Minion>().Contains(maxminion));

                if (location != null && location.CollisionObjects.Length >= 2)
                {
                    Q.Cast(location.CastPosition);
                }
            }

            if (LaneClearMenu.GetCheckBoxValue("wUse") && W.IsReady())
            {
                if (W.Handle.Ammo == 1 && LaneClearMenu.GetCheckBoxValue("wSave"))
                {
                    return;
                }

                var location =
                    Prediction.Position.PredictCircularMissileAoe(
                        minions.Cast<Obj_AI_Base>().ToArray(),
                        W.Range,
                        (int)Orbwalker.AzirSoldierAutoAttackRange,
                        W.CastDelay,
                        W.Speed).OrderByDescending(r => r.GetCollisionObjects<Obj_AI_Minion>().Length).FirstOrDefault(r => r.GetCollisionObjects<Obj_AI_Minion>().Contains(maxminion));

                if (location != null && location.CollisionObjects.Length >= 2)
                {
                    W.Cast(location.CastPosition);
                }
                if (LaneClearMenu.GetCheckBoxValue("wTurret"))
                {
                    if (turret != null)
                    {
                        W.Cast(turret);
                    }
                }
            }
        }
    }
}