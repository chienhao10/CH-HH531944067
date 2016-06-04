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
    class Brand : Champion
    {

        public readonly List<Spell> spellList = new List<Spell>();

        private const int BOUNCE_RADIUS = 450;

        public Brand()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                {
                    new ConditionalItem(ItemId.Morellonomicon),
                    new ConditionalItem(ItemId.Sorcerers_Shoes),
                    new ConditionalItem(ItemId.Rylais_Crystal_Scepter),
                    new ConditionalItem(ItemId.Liandrys_Torment),
                    new ConditionalItem(ItemId.Rabadons_Deathcap),
                    new ConditionalItem(ItemId.Void_Staff),
                },
                startingItems = new List<ItemId>
                {
                    ItemId.Boots_of_Speed,ItemId.Fiendish_Codex
                }
            };
        }

        public override void useQ(Obj_AI_Base target)
        {
        }

        public override void useW(Obj_AI_Base target)
        {
        }

        public override void useE(Obj_AI_Base target)
        {
        }

        public override void useR(Obj_AI_Base target)
        {
        }

        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 1100);
            W = new Spell(SpellSlot.W, 900);
            E = new Spell(SpellSlot.E, 625);
            R = new Spell(SpellSlot.R, 750);


            Q.SetSkillshot(0.25f, 60, 1600, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(1, 240, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetTargetted(0.25f, float.MaxValue);
            R.SetTargetted(0.25f, 1000);
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(W.Range + W.Width);
            if(tar == null)
                OnWaveClear();
            else
                OnCombo();
        }


        private void OnWaveClear()
        {
            if (player.ManaPercent < 43)
                return;

            // Minions around
            var minions = MinionManager.GetMinions(player.Position, W.Range + W.Width / 2);

            // Spell usage
            bool useQ = Q.IsReady();
            bool useW = W.IsReady();
            bool useE = E.IsReady();

            if (useQ)
            {
                // Loop through all minions to find a target, preferred a killable one
                Obj_AI_Base target = null;
                foreach (var minion in minions)
                {
                    var prediction = Q.GetPrediction(minion);
                    if (prediction.Hitchance == HitChance.VeryHigh)
                    {
                        // Set target
                        target = minion;

                        // Break if killlable
                        if (minion.Health > player.LSGetAutoAttackDamage(minion) && IsSpellKillable(minion,SpellSlot.Q))
                            break;
                    }
                }

                // Cast if target found
                if (target != null)
                    Q.Cast(target);
            }

            if (useW)
            {
                // Get farm location
                var farmLocation = MinionManager.GetBestCircularFarmLocation(minions.Select(minion => minion.ServerPosition.LSTo2D()).ToList(), W.Width, W.Range);

                // Check required hitnumber and cast
                if (farmLocation.MinionsHit >= 2)
                    W.Cast(farmLocation.Position);
            }

            if (useE)
            {
                // Loop through all minions to find a target
                foreach (var minion in minions)
                {
                    // Distance check
                    if (minion.ServerPosition.LSDistance(player.Position, true) < E.Range * E.Range)
                    {
                        // E only on targets that are ablaze or killable
                        if (IsAblazed(minion) || minion.Health > player.LSGetAutoAttackDamage(minion) && IsSpellKillable(minion,SpellSlot.E))
                        {
                            E.CastOnUnit(minion);
                            break;
                        }
                    }
                }
            }
        }

        private void OnCombo()
        {
            // Target aquireing
            var target = ARAMTargetSelector.getBestTarget(W.Range + W.Width);

            // Target validation
            if (target == null)
                return;

            // Spell usage
            bool useQ = true;
            bool useW = true;
            bool useE = true;
            bool useR = true;
            // Add to spell list
            spellList.AddRange(new[] { Q, W, E, R });

            // Killable status
            bool mainComboKillable = IsMainComboKillable(target);
            bool bounceComboKillable = IsBounceComboKillable(target);
            bool inMinimumRange = target.ServerPosition.LSDistance(player.Position, true) < E.Range * E.Range;

            // Ignite auto cast if killable, bitch please

          
                // Continue if spell not ready

                // Q
                if (Q.IsReady() && useQ)
                {
                    if ((mainComboKillable && inMinimumRange) || // Main combo killable
                        (!useW && !useE) || // Casting when not using W and E
                        (IsAblazed(target)) || // Ablazed
                        (player.LSGetSpellDamage(target, SpellSlot.Q) > target.Health) || // Killable
                        (useW && !useE && !W.IsReady() && W.IsReady((int)(player.Spellbook.GetSpell(SpellSlot.Q).Cooldown * 1000))) || // Cooldown substraction W ready
                        ((useE && !useW || useW && useE) && !E.IsReady() && E.IsReady((int)(player.Spellbook.GetSpell(SpellSlot.Q).Cooldown * 1000)))) // Cooldown substraction E ready
                    {
                        // Cast Q on high hitchance
                        Q.CastIfHitchanceEquals(target, HitChance.VeryHigh);
                    }
                }
                // W
                if (W.IsReady() && useW)
                {
                    if ((mainComboKillable && inMinimumRange) || // Main combo killable
                        (!useE) || // Casting when not using E
                        (IsAblazed(target)) || // Ablazed
                        (player.LSGetSpellDamage(target, SpellSlot.W) > target.Health) || // Killable
                        (target.ServerPosition.LSDistance(player.Position, true) > E.Range * E.Range) ||
                        (!E.IsReady() && E.IsReady((int)(player.Spellbook.GetSpell(SpellSlot.W).Cooldown * 1000)))) // Cooldown substraction E ready
                    {
                        // Cast W on high hitchance
                        W.CastIfHitchanceEquals(target, HitChance.VeryHigh);
                    }
                }
                // E
               if (E.IsReady() && useE)
                {
                    // Distance check
                    if (Vector2.DistanceSquared(target.ServerPosition.LSTo2D(), player.Position.LSTo2D()) < E.Range * E.Range)
                    {
                        if ((mainComboKillable) || // Main combo killable
                            (!useQ && !useW) || // Casting when not using Q and W
                            (E.Level >= 4) || // E level high, damage output higher
                            (useQ && (Q.IsReady() || player.Spellbook.GetSpell(SpellSlot.Q).Cooldown < 5)) || // Q ready
                            (useW && W.IsReady())) // W ready
                        {
                            // Cast E on target
                            E.CastOnUnit(target);
                        }
                    }
                }
                // R
                if (R.IsReady() && useR)
                {
                    // Distance check
                    if (target.ServerPosition.LSDistance(player.Position, true) < R.Range * R.Range)
                    {
                        // Logic prechecks
                        if ((useQ && Q.IsReady() && Q.GetPrediction(target).Hitchance == HitChance.VeryHigh || useW && W.IsReady()) && player.Health / player.MaxHealth > 0.4f)
                            return;

                        // Single hit
                        if (mainComboKillable && inMinimumRange || player.LSGetSpellDamage(target, SpellSlot.R) > target.Health)
                            R.CastOnUnit(target);
                        // Double bounce combo
                        else if (bounceComboKillable && inMinimumRange || player.LSGetSpellDamage(target, SpellSlot.R) * 2 > target.Health)
                        {
                            if (ObjectManager.Get<Obj_AI_Base>().Count(enemy => (enemy.Type == GameObjectType.obj_AI_Minion || enemy.NetworkId != target.NetworkId && enemy.Type == GameObjectType.AIHeroClient) && enemy.LSIsValidTarget() && enemy.ServerPosition.LSDistance(target.ServerPosition, true) < BOUNCE_RADIUS * BOUNCE_RADIUS) > 0)
                                R.CastOnUnit(target);
                        }
                    }
                }
            
        }


        // TODO: DFG handling and so on :P
        public double GetMainComboDamage(Obj_AI_Base target)
        {
            double damage = player.LSGetAutoAttackDamage(target);

            if (Q.IsReady())
                damage += player.LSGetSpellDamage(target, SpellSlot.Q);

            if (W.IsReady())
                damage += player.LSGetSpellDamage(target, SpellSlot.W) * (IsAblazed(target) ? 2 : 1);

            if (E.IsReady())
                damage += player.LSGetSpellDamage(target, SpellSlot.E);

            if (R.IsReady())
                damage += player.LSGetSpellDamage(target, SpellSlot.R);


            return damage;
        }

        public bool IsMainComboKillable( Obj_AI_Base target)
        {
            return GetMainComboDamage(target) > target.Health;
        }

        public double GetBounceComboDamage(Obj_AI_Base target)
        {
            double damage = GetMainComboDamage(target);

            if (R.IsReady())
                damage += player.LSGetSpellDamage(target, SpellSlot.R);

            return damage;
        }

        public bool IsBounceComboKillable(Obj_AI_Base target)
        {
            return GetBounceComboDamage(target) > target.Health;
        }

        public static bool IsAblazed(Obj_AI_Base target)
        {
            return target.HasBuff("brandablaze");
        }

        public static bool IsSpellKillable(Obj_AI_Base target, SpellSlot spellSlot)
        {
            return player.LSGetSpellDamage(target, spellSlot) > target.Health;
        }

        public static bool HasIgnite(AIHeroClient target)
        {
            if (target.IsMe)
            {
                var ignite = player.Spellbook.GetSpell(player.GetSpellSlot("SummonerDot"));
                return ignite != null && ignite.Slot != SpellSlot.Unknown;
            }
            return false;
        }

    }
}
