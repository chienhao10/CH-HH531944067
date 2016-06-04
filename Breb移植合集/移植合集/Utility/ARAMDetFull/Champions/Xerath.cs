using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;
using SharpDX;
using EloBuddy.SDK;

namespace ARAMDetFull.Champions
{
    class Xerath : Champion
    {
        
        private bool AttacksEnabled
        {
            get
            {
                if (IsCastingR)
                    return false;

                if (Q.IsCharging)
                    return false;

                return IsPassiveUp || (!Q.IsReady() && !W.IsReady() && !E.IsReady());
            }
        }

        public bool IsPassiveUp
        {
            get { return ObjectManager.Player.HasBuff("xerathascended2onhit"); }
        }

        public bool IsCastingR
        {
            get
            {
                return ObjectManager.Player.HasBuff("XerathLocusOfPower2") ||
                       (ObjectManager.Player.LastCastedSpellName() == "XerathLocusOfPower2" &&
                        Utils.TickCount - ObjectManager.Player.LastCastedSpellT() < 500);
            }
        }

        public class RCharge
        {
            public static int CastT;
            public static int Index;
            public static Vector3 Position;
            public static bool TapKeyPressed;
        }

        public Xerath()
        {
            ARAMSimulator.defBuyThings = new List<ARAMSimulator.ItemToShop>
                {
                    new ARAMSimulator.ItemToShop()
                    {
                        itemsMustHave = new List<int>{},
                        itemIds = new List<int>{1001,3028},//boots of speed, chalice
                        sellItems = new List<int>{}
                    },
                    new ARAMSimulator.ItemToShop()
                    {
                        itemsMustHave = new List<int>{1001,3028},
                        itemIds = new List<int>{1052}// amplifying tome
                    },
                    new ARAMSimulator.ItemToShop()
                    {
                        itemsMustHave = new List<int>{3028,1001,1052},
                        itemIds = new List<int>{3108}//codex
                    },
                    new ARAMSimulator.ItemToShop()
                    {
                        itemsMustHave = new List<int>{3028,3108,1001},
                        itemIds = new List<int>{3174}//unholy grail
                    },
                    new ARAMSimulator.ItemToShop()
                    {
                        itemsMustHave = new List<int>{3174,1001},
                        itemIds = new List<int>{3020}//Sorcerer's shoes
                    },
                    new ARAMSimulator.ItemToShop()
                    {
                        itemsMustHave = new List<int>{3020,3174},
                        itemIds = new List<int>{1058}// needlesly large rod
                    },
                    new ARAMSimulator.ItemToShop()
                    {
                        itemsMustHave = new List<int>{1058,3020,3174},
                        itemIds = new List<int>{1026}//blasting wand
                    },
                    new ARAMSimulator.ItemToShop()
                    {
                        itemsMustHave = new List<int>{1026,1058,3020,3174},
                        itemIds = new List<int>{3089}//rabadons deathcap
                    },
                    new ARAMSimulator.ItemToShop()
                    {
                        itemsMustHave = new List<int>{3089,3020,3174},
                        itemIds = new List<int>{1028,1052}//ruby crystal, amplifying tome
                    },
                    new ARAMSimulator.ItemToShop()
                    {
                        itemsMustHave = new List<int>{1028,1052,3089,3020,3174},
                        itemIds = new List<int>{3136}//haunting guize
                    },
                    new ARAMSimulator.ItemToShop()
                    {
                        itemsMustHave = new List<int>{3136,3089,3020,3174},
                        itemIds = new List<int>{3151}//liandry's troment
                    },
                    new ARAMSimulator.ItemToShop()
                    {
                        itemsMustHave = new List<int>{3151,3089,3020,3174},
                        itemIds = new List<int>{1026,1052}//blasting wand, amplifying tome
                    },
                    new ARAMSimulator.ItemToShop()
                    {
                        itemsMustHave = new List<int>{1026,1052,3151,3089,3020,3174},
                        itemIds = new List<int>{3135}//void staff
                    },
                    new ARAMSimulator.ItemToShop()
                    {
                        itemsMustHave = new List<int>{3135,3151,3089,3020,3174},
                        itemIds = new List<int>{3191}//seekers armgaurd
                    },
                                        new ARAMSimulator.ItemToShop()
                    {
                        itemsMustHave = new List<int>{3191,3135,3151,3089,3020,3174},
                        itemIds = new List<int>{3157}//zhonyas
                    },
                };

            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            AIHeroClient.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
            LXOrbwalker.BeforeAttack += OrbwalkingOnBeforeAttack;
            Player.OnIssueOrder += AIHeroClient_OnIssueOrder;
        }

        private void OrbwalkingOnBeforeAttack(LXOrbwalker.BeforeAttackEventArgs args)
        {
            args.Process = AttacksEnabled;
        }

        private void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name == "XerathLocusOfPower2")
                {
                    RCharge.CastT = 0;
                    RCharge.Index = 0;
                    RCharge.Position = new Vector3();
                    RCharge.TapKeyPressed = false;
                }
                else if (args.SData.Name == "xerathlocuspulse")
                {
                    RCharge.CastT = Utils.TickCount;
                    RCharge.Index++;
                    RCharge.Position = args.End;
                    RCharge.TapKeyPressed = false;
                }
            }
        }

        void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {

            if (player.LSDistance(sender) < E.Range)
            {
                E.Cast(sender);
            }
        }

        void AIHeroClient_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (IsCastingR )
            {
                args.Process = false;
            }
        }


        void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (player.LSDistance(gapcloser.Sender) < E.Range)
            {
                E.Cast(gapcloser.Sender);
            }
        }

        public override void useQ(Obj_AI_Base target)
        {
        }

        public override void useW(Obj_AI_Base target)
        {
            //  if (!W.IsReady())
            //      return;
            //  W.Cast(target);
        }

        public override void useE(Obj_AI_Base target)
        {

        }

        public override void useR(Obj_AI_Base target)
        {
        }

        public override void setUpSpells()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 1550);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 1000);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 1150);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 675);

            Q.SetSkillshot(0.6f, 100f, float.MaxValue, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.7f, 125f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.25f, 60f, 1400f, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.7f, 120f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            Q.SetCharged("XerathArcanopulseChargeUp", "XerathArcanopulseChargeUp", 750, 1550, 1.5f);
        }

        public override void useSpells()
        {
            try
            {
                if (IsCastingR)
                {
                    LXOrbwalker.SetMovement(false);
                    WhileCastingR();
                    return;
                }
                else if (!LXOrbwalker.GetMovement())
                {
                    LXOrbwalker.SetMovement(true);
                }

                if (R.IsReady())
                {
                    foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(h => h.LSIsValidTarget() && R.IsInRange(h) && (float)player.LSGetSpellDamage(h, SpellSlot.R) * 3 > h.Health))
                    {
                        R.Cast();
                    }
                }

                var tar = ARAMTargetSelector.getBestTarget(Q.Range);
                if (tar != null)
                    Combo();
                else
                    Farm(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            

            
        }


        private void Combo()
        {

            UseSpells(true, true, true);
        }

        private void Harass()
        {
            UseSpells(true, true,
                false);
        }

        private void UseSpells(bool useQ, bool useW, bool useE)
        {
            
            var qTarget = TargetSelector.GetTarget(Q.ChargedMaxRange-250, DamageType.Magical);
            var wTarget = TargetSelector.GetTarget(W.Range + W.Width * 0.5f, DamageType.Magical);
            var eTarget = TargetSelector.GetTarget(E.Range, DamageType.Magical);

            if (eTarget != null && useE && E.IsReady())
            {
                if (player.LSDistance(eTarget) < E.Range * 0.4f)
                    E.Cast(eTarget);
                else if ((!useW || !W.IsReady()))
                    E.Cast(eTarget);
            }

            if (useQ && Q.IsReady() && qTarget != null)
            {
                if (Q.IsCharging)
                {
                    Q.Cast(qTarget, false, false);
                }
                else if (!useW || !W.IsReady() || player.LSDistance(qTarget) > W.Range)
                {
                    Q.StartCharging();
                }
            }

            if (wTarget != null && useW && W.IsReady())
                W.Cast(wTarget, false, true);
        }


        private void WhileCastingR()
        {
            var rTarget = ARAMTargetSelector.getBestTarget(R.Range);

            if (rTarget != null)
            {
                //Wait at least 0.6f if the target is going to die or if the target is to far away
                if (rTarget.Health - R.GetDamage(rTarget) < 0)
                    if (Utils.TickCount - RCharge.CastT <= 700) return;

                if ((RCharge.Index != 0 && rTarget.LSDistance(RCharge.Position) > 1000))
                    if (Utils.TickCount - RCharge.CastT <= Math.Min(2500, rTarget.LSDistance(RCharge.Position) - 1000)) return;

                R.Cast(rTarget, true);
            }
        }

        private void Farm(bool laneClear)
        {
            if (player.ManaPercent < 55) return;

            var allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.ChargedMaxRange,
                MinionTypes.All);
            var rangedMinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range + W.Width + 30,
                MinionTypes.Ranged);

            var useQi = 2;
            var useWi = 2;
            var useQ = (laneClear && (useQi == 1 || useQi == 2)) || (!laneClear && (useQi == 0 || useQi == 2));
            var useW = (laneClear && (useWi == 1 || useWi == 2)) || (!laneClear && (useWi == 0 || useWi == 2));

            if (useW && W.IsReady())
            {
                var locW = W.GetCircularFarmLocation(rangedMinionsW, W.Width * 0.75f);
                if (locW.MinionsHit >= 3 && W.IsInRange(locW.Position.To3D()))
                {
                    W.Cast(locW.Position);
                    return;
                }
                else
                {
                    var locW2 = W.GetCircularFarmLocation(allMinionsQ, W.Width * 0.75f);
                    if (locW2.MinionsHit >= 1 && W.IsInRange(locW.Position.To3D()))
                    {
                        W.Cast(locW.Position);
                        return;
                    }

                }
            }

            if (useQ && Q.IsReady())
            {
                if (Q.IsCharging)
                {
                    var locQ = Q.GetLineFarmLocation(allMinionsQ);
                    if (allMinionsQ.Count == allMinionsQ.Count(m => player.LSDistance(m) < Q.Range) && locQ.MinionsHit > 0 && locQ.Position.IsValid())
                        Q.Cast(locQ.Position);
                }
                else if (allMinionsQ.Count > 0)
                    Q.StartCharging();
            }
        }

    }
}
