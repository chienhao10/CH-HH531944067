//from AlrikSharp

using System;
using EloBuddy;
using EloBuddy.SDK.Menu; using EloBuddy.SDK; using EloBuddy.SDK.Menu.Values;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace AutoSharp.Plugins
{
	public class Gragas : PluginBase
	{
		public GameObject Bomb;
		public AIHeroClient CurrentQTarget;
		public Vector3 UltPos;

		public Gragas()
		{
			Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 775);
			W = new LeagueSharp.Common.Spell(SpellSlot.W, 0);
			E = new LeagueSharp.Common.Spell(SpellSlot.E, 600);
			R = new LeagueSharp.Common.Spell(SpellSlot.R, 1050);
			Q.SetSkillshot(0.3f, 110f, 1000f, false, SkillshotType.SkillshotCircle);
			E.SetSkillshot(0.3f, 50, 1000, true, SkillshotType.SkillshotLine);
			R.SetSkillshot(0.3f, 700, 1000, false, SkillshotType.SkillshotCircle);

			GameObject.OnCreate += OnCreateObject;
			GameObject.OnDelete += GameObject_OnDelete;
		}

		public double BombMaxDamageTime { get; set; }
		public double BombCreateTime { get; set; }
		public bool BarrelIsCast { get; set; }
		public bool Exploded { get; set; }

		private void OnCreateObject(GameObject sender, EventArgs args)
		{
			if (sender.Name == "Gragas_Base_Q_Ally.troy")
			{
				Bomb = sender;
				BombCreateTime = Game.Time;
				BombMaxDamageTime = BombCreateTime + 2;
				BarrelIsCast = true;
			}

			if (sender.Name == "Gragas_Base_R_End.troy")
			{
				Exploded = true;
				UltPos = sender.Position;
                LeagueSharp.Common.Utility.DelayAction.Add(3000, () => { Exploded = false; });
			}
		}

		private void GameObject_OnDelete(GameObject sender, EventArgs args)
		{
			if (sender.Name == "Gragas_Base_Q_Ally.troy")
			{
				Bomb = null;
			}
		}

		public override void OnUpdate(EventArgs args)
		{
			if (ComboMode)
			{
				var targetgrag = TargetSelector.GetTarget(900, DamageType.Magical);

                Combo(targetgrag);
            }
        }

        private void ThrowBarrel(AIHeroClient tar)
        {
            if (BarrelIsCast)
            {
                return;
            }
            if (Q.Cast(tar) == LeagueSharp.Common.Spell.CastStates.SuccessfullyCasted)
            {
                BarrelIsCast = true;
                CurrentQTarget = tar;
            }
        }

        private bool SecondQReady()
        {
            return Q.IsReady() && Bomb != null;
        }

        public bool TargetCloseToQEdge(AIHeroClient t)
        {
            var qPos = Bomb.Position;
            var qRadius = Bomb.BoundingRadius;
            var disTtoQ = t.LSDistance(qPos);
            var difference = qRadius - disTtoQ;
            if (disTtoQ > qRadius)
            {
                return false;
            }
            return difference > 5 && difference < 40;
        }

        private void ExplodeBarrel()
        {
            if (!BarrelIsCast)
            {
                return;
            }
            Q.Cast();
            BarrelIsCast = false;
            CurrentQTarget = null;
        }

        public bool TargetIsInQ(AIHeroClient t)
        {
            var qPos = Bomb.Position;
            var qRadius = Bomb.BoundingRadius;
            var disTtoQ = t.LSDistance(qPos);

            if (disTtoQ > qRadius)
            {
                return false;
            }
            return true;
        }

        private bool RKillStealIsTargetInQ(AIHeroClient target)
        {
            return Bomb != null && TargetIsInQ(target);
        }

        private void Combo(AIHeroClient t)
        {
            if (W.IsReady() && t.LSIsValidTarget(Q.Range))
            {
                W.Cast();
            }
            if (Q.IsReady())
            {
                if (FirstQReady() && t.LSIsValidTarget(Q.Range))
                {
                    ThrowBarrel(t);
                }
                if (SecondQReady() && CurrentQTarget != null)
                {
                    if (TargetCloseToQEdge(CurrentQTarget))
                    {
                        ExplodeBarrel();
                    }
                    if (CurrentQTarget.IsMoving && TargetIsInQ(CurrentQTarget))
                    {
                        ExplodeBarrel();
                    }
                }
            }


            if (E.IsReady())
            {
                if (t.LSIsValidTarget(E.Range))
                {
                    if (E.WillHit(t, E.GetPrediction(t).CastPosition))
                    {
                        if (E.Cast(t) == LeagueSharp.Common.Spell.CastStates.SuccessfullyCasted)
                        {
                            if (ObjectManager.Player.HasBuff("gragaswself"))
                            {
                                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackTo, t);
                            }
                        }
                    }
                }
            }


            if (R.IsReady())
            {
                if (t.LSIsValidTarget(R.Range))
                {
                    if (R.IsKillable(t))
                    {
                        if (RKillStealIsTargetInQ(t))
                        {
                            if (Q.IsKillable(t))
                            {
                                ExplodeBarrel();
                            }
                        }
                        else
                        {
                            var pred = LeagueSharp.Common.Prediction.GetPrediction(t, R.Delay, R.Width / 2, R.Speed);
                            R.Cast(pred.CastPosition);
                        }
                    }
                }
            }
        }

        private bool FirstQReady()
        {
            if (Q.IsReady() && Bomb == null)
            {
                BarrelIsCast = false;
                return true;
            }
            return false;
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