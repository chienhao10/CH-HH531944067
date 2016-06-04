using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common; using EloBuddy;
using SharpDX;

namespace ARAMDetFull.Champions
{
    class ViktorA : Champion
    {
        public ViktorA()
        {
            ARAMSimulator.defBuyThings = new List<ARAMSimulator.ItemToShop>
                {//3198
                    new ARAMSimulator.ItemToShop()
                    {
                        itemsMustHave = new List<int>{},
                        itemIds = new List<int>{1001,3028},
                        sellItems = new List<int>{}
                    },
                    new ARAMSimulator.ItemToShop()
                    {
                        itemsMustHave = new List<int>{1001,3028},
                        itemIds = new List<int>{1058}//rod
                    },
                    new ARAMSimulator.ItemToShop()
                    {
                        itemsMustHave = new List<int>{1058},
                        itemIds = new List<int>{3020}
                    },
                    new ARAMSimulator.ItemToShop()
                    {
                        itemsMustHave = new List<int>{3020},
                        itemIds = new List<int>{3089}
                    },
                    new ARAMSimulator.ItemToShop()
                    {
                        itemsMustHave = new List<int>{3089},
                        itemIds = new List<int>{3198}
                    },
                    new ARAMSimulator.ItemToShop()
                    {
                        itemsMustHave = new List<int>{3198},
                        itemIds = new List<int>{3174}
                    },
                    new ARAMSimulator.ItemToShop()
                    {
                        itemsMustHave = new List<int>{3174},
                        itemIds = new List<int>{1058}
                    },
                    new ARAMSimulator.ItemToShop()
                    {
                        itemsMustHave = new List<int>{1058,3089},
                        itemIds = new List<int>{3157}
                    },
                    new ARAMSimulator.ItemToShop()
                    {
                        itemsMustHave = new List<int>{3157},
                        itemIds = new List<int>{3135}
                    },
                    new ARAMSimulator.ItemToShop()
                    {
                        itemsMustHave = new List<int>{3135},
                        itemIds = new List<int>{3100}
                    },
                    new ARAMSimulator.ItemToShop()
                    {
                        last = true,
                        itemsMustHave = new List<int>{3100},
                        itemIds = new List<int>{}
                    },
                };

            Interrupter.OnPossibleToInterrupt += Interrupter_OnPossibleToInterrupt;
        }

        private readonly int maxRangeE = 1175;
        private readonly int lengthE = 750;
        private readonly int speedE = 1200;
        private readonly int rangeE = 525;

        private static int evolveTimes = 0;

        public override void useQ(Obj_AI_Base target)
        {
            Console.WriteLine("cast Q");
            if (!Q.IsReady() || target == null)
                return;
            Q.Cast(target);
            Player.IssueOrder(GameObjectOrder.AttackUnit, target);
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady() || target == null)
                return;
            W.Cast(target);

        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || target == null)
                return;
            PredictCastE(target, true);
        }

        public override void useR(Obj_AI_Base target)
        {
            if (target == null)
                return;
            R.Cast(target);
        }

        public override void useSpells()
        {
            if (R.IsReady() && R.Instance.Name != "ViktorChaosStorm")
            {
                var stormT = ARAMTargetSelector.getBestTarget(1100);
                if (stormT != null)
                     R.Cast(stormT.ServerPosition);
            }

            var tar = ARAMTargetSelector.getBestTarget(Q.Range);
            if (tar != null) useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(W.Range);
            if (tar != null) useW(tar);
            tar = ARAMTargetSelector.getBestTarget(maxRangeE);
            if (tar != null) useE(tar);
            tar = ARAMTargetSelector.getBestTarget(R.Range);
            if (tar != null) useR(tar);

            if (evolveTimes < 4 && LeagueSharp.Common.Items.HasItem(3198) && player.EvolvePoints != 0)
            {
                Console.WriteLine("evolve");
                player.Spellbook.EvolveSpell(SpellSlot.Q);
                player.Spellbook.EvolveSpell(SpellSlot.W);
                player.Spellbook.EvolveSpell(SpellSlot.E);
                evolveTimes++;
            }
        }

        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 600);
            W = new Spell(SpellSlot.W, 700);
            E = new Spell(SpellSlot.E, maxRangeE);
            R = new Spell(SpellSlot.R, 700);

            // Finetune spells
            Q.SetTargetted(0.25f, 2000);
            W.SetSkillshot(0.25f, 300, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0, 90, speedE, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.25f, 450f, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }

        private void Interrupter_OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (R.IsReady() && spell.DangerLevel == InterruptableDangerLevel.High && R.IsInRange(unit.ServerPosition))
                R.Cast(unit.ServerPosition.LSTo2D(), true);
        }

        private bool PredictCastMinionE(int requiredHitNumber = -1)
        {
            int hitNum = 0;
            Vector2 startPos = new Vector2(0, 0);
            foreach (var minion in MinionManager.GetMinions(player.Position, rangeE))
            {
                var farmLocation = MinionManager.GetBestLineFarmLocation((from mnion in MinionManager.GetMinions(minion.Position, lengthE) select mnion.Position.LSTo2D()).ToList<Vector2>(), E.Width, lengthE);
                if (farmLocation.MinionsHit > hitNum)
                {
                    hitNum = farmLocation.MinionsHit;
                    startPos = minion.Position.LSTo2D();
                }
            }

            if (startPos.X != 0 && startPos.Y != 0)
                return PredictCastMinionE(startPos, requiredHitNumber);

            return false;
        }

        private bool PredictCastMinionE(Vector2 fromPosition, int requiredHitNumber = 1)
        {
            var farmLocation = MinionManager.GetBestLineFarmLocation(MinionManager.GetMinionsPredictedPositions(MinionManager.GetMinions(fromPosition.To3D(), lengthE), E.Delay, E.Width, speedE, fromPosition.To3D(), lengthE, false, SkillshotType.SkillshotLine), E.Width, lengthE);

            if (farmLocation.MinionsHit >= requiredHitNumber)
            {
                CastE(fromPosition, farmLocation.Position);
                return true;
            }

            return false;
        }

        private void PredictCastE(Obj_AI_Base target, bool longRange = false)
        {
            // Helpers
            bool inRange = Vector2.DistanceSquared(target.ServerPosition.LSTo2D(), player.Position.LSTo2D()) < E.Range * E.Range;
            PredictionOutput prediction;
            bool spellCasted = false;

            // Positions
            Vector3 pos1, pos2;

            // Champs
            var nearChamps = (from champ in ObjectManager.Get<AIHeroClient>() where champ.LSIsValidTarget(maxRangeE) && target != champ select champ).ToList();
            var innerChamps = new List<AIHeroClient>();
            var outerChamps = new List<AIHeroClient>();
            foreach (var champ in nearChamps)
            {
                if (Vector2.DistanceSquared(champ.ServerPosition.LSTo2D(), player.Position.LSTo2D()) < E.Range * E.Range)
                    innerChamps.Add(champ);
                else
                    outerChamps.Add(champ);
            }

            // Minions
            var nearMinions = MinionManager.GetMinions(player.Position, maxRangeE);
            var innerMinions = new List<Obj_AI_Base>();
            var outerMinions = new List<Obj_AI_Base>();
            foreach (var minion in nearMinions)
            {
                if (Vector2.DistanceSquared(minion.ServerPosition.LSTo2D(), player.Position.LSTo2D()) < E.Range * E.Range)
                    innerMinions.Add(minion);
                else
                    outerMinions.Add(minion);
            }

            // Main target in close range
            if (inRange)
            {
                // Get prediction reduced speed, adjusted sourcePosition
                E.Speed = speedE * 0.9f;
                E.From = target.ServerPosition + (Vector3.Normalize(player.Position - target.ServerPosition) * (lengthE * 0.1f));
                prediction = E.GetPrediction(target);
                E.From = player.Position;

                // Prediction in range, go on
                if (prediction.CastPosition.LSDistance(player.Position) < E.Range)
                    pos1 = prediction.CastPosition;
                // Prediction not in range, use exact position
                else
                {
                    pos1 = target.ServerPosition;
                    E.Speed = speedE;
                }

                // Set new sourcePosition
                E.From = pos1;
                E.RangeCheckFrom = pos1;

                // Set new range
                E.Range = lengthE;

                // Get next target
                if (nearChamps.Count > 0)
                {
                    // Get best champion around
                    var closeToPrediction = new List<AIHeroClient>();
                    foreach (var enemy in nearChamps)
                    {
                        // Get prediction
                        prediction = E.GetPrediction(enemy);
                        // Validate target
                        if (prediction.Hitchance >= HitChance.High && Vector2.DistanceSquared(pos1.LSTo2D(), prediction.CastPosition.LSTo2D()) < (E.Range * E.Range) * 0.8)
                            closeToPrediction.Add(enemy);
                    }

                    // Champ found
                    if (closeToPrediction.Count > 0)
                    {
                        // Sort table by health DEC
                        if (closeToPrediction.Count > 1)
                            closeToPrediction.Sort((enemy1, enemy2) => enemy2.Health.CompareTo(enemy1.Health));

                        // Set destination
                        prediction = E.GetPrediction(closeToPrediction[0]);
                        pos2 = prediction.CastPosition;

                        // Cast spell
                        CastE(pos1, pos2);
                        spellCasted = true;
                    }
                }

                // Spell not casted
                if (!spellCasted)
                    // Try casting on minion
                    if (!PredictCastMinionE(pos1.LSTo2D()))
                        // Cast it directly
                        CastE(pos1, E.GetPrediction(target).CastPosition);

                // Reset spell
                E.Speed = speedE;
                E.Range = rangeE;
                E.From = player.Position;
                E.RangeCheckFrom = player.Position;
            }

            // Main target in extended range
            else if (longRange)
            {
                // Radius of the start point to search enemies in
                float startPointRadius = 150;

                // Get initial start point at the border of cast radius
                Vector3 startPoint = player.Position + Vector3.Normalize(target.ServerPosition - player.Position) * rangeE;

                // Potential start from postitions
                var targets = (from champ in nearChamps where Vector2.DistanceSquared(champ.ServerPosition.LSTo2D(), startPoint.LSTo2D()) < startPointRadius * startPointRadius && Vector2.DistanceSquared(player.Position.LSTo2D(), champ.ServerPosition.LSTo2D()) < rangeE * rangeE select champ).ToList();
                if (targets.Count > 0)
                {
                    // Sort table by health DEC
                    if (targets.Count > 1)
                        targets.Sort((enemy1, enemy2) => enemy2.Health.CompareTo(enemy1.Health));

                    // Set target
                    pos1 = targets[0].ServerPosition;
                }
                else
                {
                    var minionTargets = (from minion in nearMinions where Vector2.DistanceSquared(minion.ServerPosition.LSTo2D(), startPoint.LSTo2D()) < startPointRadius * startPointRadius && Vector2.DistanceSquared(player.Position.LSTo2D(), minion.ServerPosition.LSTo2D()) < rangeE * rangeE select minion).ToList();
                    if (minionTargets.Count > 0)
                    {
                        // Sort table by health DEC
                        if (minionTargets.Count > 1)
                            minionTargets.Sort((enemy1, enemy2) => enemy2.Health.CompareTo(enemy1.Health));

                        // Set target
                        pos1 = minionTargets[0].ServerPosition;
                    }
                    else
                        // Just the regular, calculated start pos
                        pos1 = startPoint;
                }

                // Predict target position
                E.From = pos1;
                E.Range = lengthE;
                E.RangeCheckFrom = pos1;
                prediction = E.GetPrediction(target);

                // Cast the E
                if (prediction.Hitchance == HitChance.High)
                    CastE(pos1, prediction.CastPosition);

                // Reset spell
                E.Range = rangeE;
                E.From = player.Position;
                E.RangeCheckFrom = player.Position;
            }

        }

        private void CastE(Vector3 source, Vector3 destination)
        {
            E.Cast(source, destination);
        }

        private void CastE(Vector2 source, Vector2 destination)
        {
            E.Cast(source, destination);
        }
    }
}
