using System;
using EloBuddy;
using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using System.Linq;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;

namespace AutoSharp.Plugins
{
	public class Poppy : PluginBase
	{
		public static string[] Supports =
		{

            "Alistar", "Blitzcrank", "Braum", "Janna", "Karma", "Leona", "Lulu",
            "Morgana", "Nunu", "Nami", "Soraka", "Sona", "Taric", "Thresh", "Zyra"
		};

		public Poppy()
		{
			//spelldata from Mechanics-StackOverflow Galio //wat? XD
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q);
			W = new LeagueSharp.Common.Spell(SpellSlot.W);
			E = new LeagueSharp.Common.Spell(SpellSlot.E, 525);
			R = new LeagueSharp.Common.Spell(SpellSlot.R, 900);
		}

		public override void OnAfterAttack(AttackableUnit target, EventArgs args)
		{
			var t = target as AIHeroClient;
			if (t != null)
			{
				if (Q.IsReady() && t.LSIsValidTarget(Q.Range))
				{
					Q.Cast();
					Orbwalker.ResetAutoAttack();
				}
			}
		}

		public override void OnUpdate(EventArgs args)
		{
			if (Q.IsReady()&&Player.LSCountEnemiesInRange(900) >= 1)
			{
				Q.Cast();
			}	var tarpop = TargetSelector.GetTarget(900, DamageType.Physical);

                DoCombo(tarpop);
           
        }

        private AIHeroClient FindTank()
        {
            AIHeroClient getTank = null;
            var tempmaxhp = 0.0f;
            foreach (
                var target in
                    ObjectManager.Get<AIHeroClient>().Where(x => Player.LSDistance(x) <= R.Range && x.IsEnemy && !x.IsDead)
                )
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (target != null)
                {
                    if (target.MaxHealth > tempmaxhp)
                    {
                        tempmaxhp = target.MaxHealth;
                        getTank = target;
                    }
                }
            }

            return getTank;
        }

        private bool IsSupport(AIHeroClient hero)
        {
            return Supports.Any(support => hero.CharData.BaseSkinName.ToLower() == support.ToLower());
        }

        private void DoCombo(AIHeroClient target)
        {
            if (target == null)
            {
                return;
            }


            if (Player.LSCountEnemiesInRange(500) >= 2)
            {
                foreach (
                    var hero in
                        from hero in
                            ObjectManager.Get<AIHeroClient>()
                                .Where(
                                    hero =>
                                        hero.LSIsValidTarget(R.Range) && hero.IsEnemy && !hero.IsDead && IsSupport(hero))
                        select hero)
                {
                    if (hero != null)
                    {
                        R.Cast(hero);
                    }
                }

                R.Cast(FindTank());
            }

            if (W.IsReady() && W.Range >= Player.LSDistance(target))
            {
                W.Cast();
            }

            if (E.IsReady())
            {
                //from vayne markmans
                foreach (
                    var hero in from hero in ObjectManager.Get<AIHeroClient>().Where(hero => hero.LSIsValidTarget(525f))
                        let prediction = E.GetPrediction(hero)
                        where
                            NavMesh.GetCollisionFlags(
                                prediction.UnitPosition.LSTo2D()
                                    .LSExtend(ObjectManager.Player.ServerPosition.LSTo2D(), -300)
                                    .To3D()).HasFlag(CollisionFlags.Wall) ||
                            NavMesh.GetCollisionFlags(
                                prediction.UnitPosition.LSTo2D()
                                    .LSExtend(ObjectManager.Player.ServerPosition.LSTo2D(), -(300 / 2))
                                    .To3D()).HasFlag(CollisionFlags.Wall)
                        select hero)
                {
                    E.Cast(hero);
                }
            }
        }

        public override void ComboMenu(Menu config)
        {
            config.AddBool("ComboQ", "Use Q", true);
            config.AddBool("ComboW", "Use W", true);
            config.AddBool("ComboE", "Use E", true);
            config.AddBool("ComboR", "Use R", true);
        }
    }
}