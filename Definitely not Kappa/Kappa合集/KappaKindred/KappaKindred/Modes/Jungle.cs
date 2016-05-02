namespace KappaKindred.Modes
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;

    internal class Jungle
    {
        public static void Start()
        {
            var useQ = Menu.JungleMenu["Q"].Cast<CheckBox>().CurrentValue && Spells.Q.IsReady();
            var useW = Menu.JungleMenu["W"].Cast<CheckBox>().CurrentValue && Spells.W.IsReady();
            var useE = Menu.JungleMenu["E"].Cast<CheckBox>().CurrentValue && Spells.E.IsReady();

            if (useW)
            {
                var minions = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, Spells.W.Range + 50, false);

                if (minions.Any())
                {
                    Spells.W.Cast();
                }
            }

            if (useE)
            {
                var jmobs = ObjectManager.Get<Obj_AI_Minion>().OrderBy(m => m.CampNumber).Where(m => m.IsMonster && m.IsEnemy && !m.IsDead);
                foreach (var jmob in jmobs)
                {
                    if (jmob.IsValidTarget(Spells.E.Range))
                    {
                        if (jmob.BoundingRadius >= 60)
                        {
                            Spells.E.Cast(jmob);
                        }

                        if (jmob.BaseSkinName == "Sru_Crab")
                        {
                            Spells.E.Cast(jmob);
                        }
                    }
                }
            }

            if (useQ)
            {
                var minions1 = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, Spells.Q.Range, false);

                var location =
                    Prediction.Position.PredictCircularMissileAoe(
                        minions1.Cast<Obj_AI_Base>().ToArray(),
                        Spells.Q.Range - 25,
                        Spells.Q.Radius - 25,
                        Spells.Q.CastDelay,
                        Spells.Q.Speed).OrderByDescending(r => r.GetCollisionObjects<Obj_AI_Minion>().Length).FirstOrDefault();

                if (location != null)
                {
                    Spells.Q.Cast(Game.CursorPos);
                }
            }
        }
    }
}