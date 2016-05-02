namespace KappaKindred.Modes
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;

    internal class Clear
    {
        public static void Start()
        {
            var useQ = Menu.LaneMenu["Q"].Cast<CheckBox>().CurrentValue && Spells.Q.IsReady();
            var useW = Menu.LaneMenu["W"].Cast<CheckBox>().CurrentValue && Spells.W.IsReady();
            var useE = Menu.LaneMenu["E"].Cast<CheckBox>().CurrentValue && Spells.E.IsReady();

            if (useW)
            {
                var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(
                    EntityManager.UnitTeam.Enemy,
                    Player.Instance.Position,
                    Spells.W.Range + 50,
                    false);

                if (minions.Count() >= 3)
                {
                    Spells.W.Cast();
                }
            }

            if (useE)
            {
                var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(
                    EntityManager.UnitTeam.Enemy,
                    Player.Instance.Position,
                    Spells.E.Range,
                    false);
                foreach (var minion in minions)
                {
                    if (minion != null)
                    {
                        if (minion.BoundingRadius == 65)
                        {
                            Spells.E.Cast(minion);
                        }
                    }
                }
            }

            if (useQ)
            {
                var minions1 = EntityManager.MinionsAndMonsters.GetLaneMinions(
                    EntityManager.UnitTeam.Enemy,
                    Player.Instance.Position,
                    Spells.Q.Range,
                    false);

                var location =
                    Prediction.Position.PredictCircularMissileAoe(
                        minions1.Cast<Obj_AI_Base>().ToArray(),
                        Spells.Q.Range - 25,
                        Spells.Q.Radius - 25,
                        Spells.Q.CastDelay,
                        Spells.Q.Speed).OrderByDescending(r => r.GetCollisionObjects<Obj_AI_Minion>().Length).FirstOrDefault();
                if (location != null && location.CollisionObjects.Length >= 2)
                {
                    Spells.Q.Cast(Game.CursorPos);
                }
            }
        }
    }
}