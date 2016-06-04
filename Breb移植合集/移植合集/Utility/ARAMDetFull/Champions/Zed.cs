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
    class ZedA : Champion
    {

        public int lastW = 0;

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady())
                return;
            Q.Cast(target);
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady() || shadowW != null || lastW+1000>LXOrbwalker.now)
                return;
            lastW = LXOrbwalker.now;
            W.Cast(target.Position);
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || W.IsReady())
                return;
            E.Cast();
        }

        public override void useR(Obj_AI_Base target)
        {
            return;
        }

        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 900f);
            W = new Spell(SpellSlot.W, 550f);
            E = new Spell(SpellSlot.E, 270f);
            R = new Spell(SpellSlot.R, 650f);

            Q.SetSkillshot(0.25f, 50f, 1700f, false, SkillshotType.SkillshotLine);
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(600);
            if (tar == null)
                return;

            if (!Sector.inTowerRange(tar.Position.LSTo2D()) && (getFullComboDmg(tar) > tar.Health || player.HealthPercent < 25))
            {
                if (tFocus == null) tFocus = tar;
                doLineCombo(tFocus);
            }
            else
            {
                tFocus = null;
                tar = ARAMTargetSelector.getBestTarget(Q.Range);
                if (tar != null) useQ(tar);
                tar = ARAMTargetSelector.getBestTarget(W.Range);
                if (tar != null) useW(tar);
                tar = ARAMTargetSelector.getBestTarget(E.Range);
                if (tar != null) useE(tar);
            }
        }


        public static SummonerItems sumItems;

        public static AIHeroClient tFocus = null;

        public static Obj_AI_Minion shadowW;
        public static bool getRshad;
        public static bool getWshad;
        public static Obj_AI_Minion shadowR;

        public static bool wIsCasted = false;
        public static bool serverTookWCast = false;
        public static float recast = 0;

        public ZedA()
        {
            sumItems = new SummonerItems(ObjectManager.Player);
            GameObject.OnCreate += OnCreateObject;
            GameObject.OnDelete += OnDeleteObject;

            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                {
                    new ConditionalItem(ItemId.Blade_of_the_Ruined_King),
                    new ConditionalItem(ItemId.Mercurys_Treads),
                    new ConditionalItem(ItemId.Last_Whisper),
                    new ConditionalItem(ItemId.Ravenous_Hydra_Melee_Only),
                    new ConditionalItem(ItemId.Maw_of_Malmortius, ItemId.The_Bloodthirster, ItemCondition.ENEMY_AP),
                    new ConditionalItem(ItemId.Banshees_Veil),
                },
                startingItems = new List<ItemId>
                {
                    ItemId.Vampiric_Scepter,ItemId.Boots_of_Speed
                }
            };
        }

        private static void OnDeleteObject(GameObject sender, EventArgs args) {
            if (ZedA.shadowR != null && sender.NetworkId == ZedA.shadowR.NetworkId) {
                ZedA.shadowR = null;
                ZedA.getRshad = false;
            }

            if (ZedA.shadowW != null && sender.NetworkId == ZedA.shadowW.NetworkId) {
                ZedA.shadowW = null;
                ZedA.getWshad = false;
            }
        }

        private static void OnCreateObject(GameObject sender, EventArgs args)
        {
            /* if (sender.Name.Equals("Zed_ShadowIndicatorNEARBloop.troy") && ZedA.isSafeSwap(ZedA.shadowR) &&
                menu.Item("SwapRKill").GetValue<bool>()) {
                if (ZedA.canGoToShadow("R")) {
                    ZedA.R.Cast(); TODO fixerino
                }
            }*/
            if (sender is Obj_AI_Minion)
            {
                var min = sender as Obj_AI_Minion;
                if (min.IsAlly && min.BaseSkinName == "ZedShadow")
                {
                    if (ZedA.getRshad)
                    {
                        // Chat.Print("R Create");
                        ZedA.shadowR = min;
                        ZedA.getRshad = false;
                    }
                    if (ZedA.getWshad)
                    {
                        //Chat.Print("W Created");
                        ZedA.shadowW = min;
                        ZedA.getWshad = false;
                    }
                }
            }
        }

        public float getFullComboDmg(AIHeroClient target)
        {
            if (target == null)
                return 0;
            float dmg = 0;
            PredictionOutput po = Prediction.GetPrediction(target, 0.5f);
            float dist = player.LSDistance(po.UnitPosition);
            float gapDist = ((W.IsReady()) ? W.Range : 0);
            float distAfterGap = dist - gapDist;

            if (distAfterGap < player.AttackRange)
                dmg += (float)player.LSGetAutoAttackDamage(target);
            if (Q.IsReady() && distAfterGap < Q.Range)
                dmg += Q.GetDamage(target);
            if (Q.IsReady() && W.IsReady() && distAfterGap < Q.Range && dist < Q.Range)
                dmg += Q.GetDamage(target) / 2;
            if (distAfterGap < E.Range)
                dmg += E.GetDamage(target);
            if (R.IsReady() && distAfterGap < R.Range)
            {
                dmg += R.GetDamage(target);
                dmg += (float)player.CalcDamage(target, DamageType.Physical, (dmg * (5 + 15 * R.Level) / 100));
            }
            if (LeagueSharp.Common.Items.HasItem(3153)) // botrk
                dmg += (float)player.GetItemDamage(target, Damage.DamageItems.Botrk);
            if (LeagueSharp.Common.Items.HasItem(3074))
                dmg += (float)player.GetItemDamage(target, Damage.DamageItems.Hydra);
            if (LeagueSharp.Common.Items.HasItem(3077))
                dmg += (float)player.GetItemDamage(target, Damage.DamageItems.Tiamat);
            if (LeagueSharp.Common.Items.HasItem(3144))
                dmg += (float)player.GetItemDamage(target, Damage.DamageItems.Bilgewater);

            return dmg;
        }

        private bool canDoCombo(IEnumerable<SpellSlot> sp)
        {
            float delay = sp.Sum(sp1 => player.Spellbook.GetSpell(sp1).SData.SpellCastTime); //Hope it is correct
            float totalCost = sp.Sum(sp1 => player.Spellbook.GetSpell(sp1).SData.Mana);
            return player.Mana + delay * 5 >= totalCost;
        }

        public void doLineCombo(Obj_AI_Base target)
        {
            if (target == null)
                return;

            try
            {
                //Tried to Add shadow Coax
                float dist = player.LSDistance(target);
                if (R.IsReady() && shadowR == null && dist < R.Range &&
                    canDoCombo(new[] { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R }))
                {
                    R.Cast(target);
                }
                //eather casts 2 times or 0 get it to cast 1 time TODO
                // Chat.Print("W2 "+ZedSharp.W2);
                /*foreach (
                AIHeroClient newtarget in
                ObjectManager.Get<AIHeroClient>().Where(hero => hero.LSIsValidTarget(Q.Range)).Where(
                enemy => enemy.HasBuff("zedulttargetmark") && enemy.IsEnemy && !enemy.IsMinion)) {
                target = newtarget;
                }*/
                //PredictionOutput p1o = Prediction.GetPrediction(target, 0.350f);
                Vector3 shadowPos = target.Position + Vector3.Normalize(target.Position - shadowR.Position) * E.Range;
                if (W.IsReady() && shadowW == null &&
                    ((!getWshad && recast < LXOrbwalker.now && !serverTookWCast)))
                {
                    //V2E(shadowR.Position, po.UnitPosition, E.Range)
                    Console.WriteLine("cast WWW");
                    W.Cast(shadowPos);
                    serverTookWCast = false;
                    wIsCasted = true;
                    recast = LXOrbwalker.now + 300;
                }
                if (E.IsReady() && shadowW != null || shadowR != null)
                {
                    E.Cast();
                }
                if (Q.IsReady() && shadowW != null && shadowR != null)
                {
                    float midDist = dist;
                    midDist += target.LSDistance(shadowR);
                    midDist += target.LSDistance(shadowW);
                    float delay = midDist / (Q.Speed * 3);
                    PredictionOutput po = Prediction.GetPrediction(target, delay * 1.1f);
                    if (po.Hitchance > HitChance.Low)
                    {
                        // Console.WriteLine("Cast QQQQ");
                        Q.Cast(po.UnitPosition);
                    }
                }
                if (shadowR != null)
                {
                    castItemsFull(target);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void castItemsFull(Obj_AI_Base target)
        {
            if (target.LSDistance(player) < 500)
            {
                sumItems.cast(SummonerItems.ItemIds.Ghostblade);
                sumItems.castIgnite((AIHeroClient)target);
            }
            if (target.LSDistance(player) < 500)
            {
                sumItems.cast(SummonerItems.ItemIds.BotRK, target);
                sumItems.cast(SummonerItems.ItemIds.Cutlass, target);
            }
            if (target.LSDistance(player.ServerPosition) < (400 + target.BoundingRadius - 20))
            {
                sumItems.cast(SummonerItems.ItemIds.Tiamat);
                sumItems.cast(SummonerItems.ItemIds.Hydra);
            }
        }
    }
}
