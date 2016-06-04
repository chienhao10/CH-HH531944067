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
    class LeeSin : Champion
    {
        public LeeSin()
        {
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                {
                    new ConditionalItem(ItemId.Ravenous_Hydra_Melee_Only),
                    new ConditionalItem(ItemId.Mercurys_Treads, ItemId.Ninja_Tabi, ItemCondition.ENEMY_AP),
                    new ConditionalItem(ItemId.Last_Whisper),
                    new ConditionalItem(ItemId.Spirit_Visage, ItemId.Randuins_Omen, ItemCondition.ENEMY_AP),
                    new ConditionalItem(ItemId.Maw_of_Malmortius),
                    new ConditionalItem(ItemId.Banshees_Veil),
                },
                startingItems = new List<ItemId>
                {
                    ItemId.Vampiric_Scepter,ItemId.Boots_of_Speed
                }
            };
            LXOrbwalker.AfterAttack += afterAttack;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
        }

        private void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!W.IsReady() || sender == null || args.Target == null || sender.IsAlly || !(sender is AIHeroClient) || !(args.Target is AIHeroClient) || !args.Target.IsAlly || safeGap((AIHeroClient)args.Target))
                return;
            if (W.IsInRange(args.Target))
                W.CastOnUnit((AIHeroClient)args.Target);
        }

        private void afterAttack(AttackableUnit unit, AttackableUnit target)
        {

        }

        private AIHeroClient buffQhero()
        {
            return HeroManager.Enemies.FirstOrDefault(ene => ene.IsDead && ene.HasBuff("BlindMonkQOne"));
        }

        private bool passive
        {
            get {return player.HasBuff("blindmonkpassive_cosmetic");}
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady() || target == null)
                return;
            if(Q.Instance.Name == "BlindMonkQOne")
                Q.Cast(target);
            else if (safeGap(target) || target.LSDistance(player,true)<300*300)
                Q.Cast();
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady() || target == null)
                return;
            if (W.Instance.Name == "blindmonkwtwo")
            {
                if (!passive || player.HealthPercent < 65)
                    W.Cast();
            }
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || target == null)
                return;
            if (E.Instance.Name == "BlindMonkEOne")
                E.Cast();
            else if (!passive)
                E.Cast();
        }

        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady() || target == null)
                return;
            if (R.IsKillable(target) || player.HealthPercent<25)
                R.Cast(target);
            else
            {
                castRMultiple(2);
            }
        }

        private void castRMultiple(int min)
        {
            if (!R.IsReady())
                return;

            foreach (var enemy in from enemy in HeroManager.Enemies
                                  let input = new PredictionInput()
                                  {
                                      Aoe = false,
                                      Collision = true,
                                      CollisionObjects = new[] { CollisionableObjects.Heroes },
                                      Delay = 0.1f,
                                      Radius = 100f,
                                      Range = R.Range,
                                      Speed = 1500f,
                                      From = player.ServerPosition,
                                  }
                                  let output = Prediction.GetPrediction(input)
                                  where output.Hitchance >= HitChance.Medium && player.LSDistance(output.CastPosition) < R.Range
                                  let endPos = (player.ServerPosition + output.CastPosition - player.ServerPosition).LSNormalized() * 1000
                                  let colObjs = output.CollisionObjects
                                  where player.LSDistance(endPos) < 1200 && colObjs.Any()
                                  where colObjs.Count >= min
                                  select enemy)
            {
                R.CastOnUnit(enemy);
            }
        }

        public override void useSpells()
        {
           
            var tar = ARAMTargetSelector.getBestTarget(Q.Range);
            if (tar != null) useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(W.Range);
            if (tar != null) useW(tar);
            tar = ARAMTargetSelector.getBestTarget(E.Range);
            if (tar != null) useE(tar);
            tar = ARAMTargetSelector.getBestTarget(R.Range);
            if (tar != null) useR(tar);
        }


        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 1100f);
            W = new Spell(SpellSlot.W, 700f);
            E = new Spell(SpellSlot.E, 330f);
            R = new Spell(SpellSlot.R, 375f);

            Q.SetSkillshot(0.25f, 65f, 1800f, true, SkillshotType.SkillshotLine);

        }


        public override void farm()
        {

            var AllMinions = MinionManager.GetMinions(player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health);
            foreach (var minion in AllMinions)
            {
                if (Q.IsReady() && Q.GetDamage(minion) > minion.Health)
                {
                    Q.Cast(minion);
                    return;
                }
                if (E.IsReady() && E.GetDamage(minion) > minion.Health)
                {
                    E.Cast(minion);
                }
            }
        }
    }
}
