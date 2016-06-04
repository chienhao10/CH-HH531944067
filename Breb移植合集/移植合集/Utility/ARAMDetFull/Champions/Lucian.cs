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
    class Lucian : Champion
    {
        internal enum QhitChance
        {
            himself = 0,
            easy = 1,
            medium = 2,
            hard = 3,
            wontHit = 4
        }


        public static SummonerItems sumItems = new SummonerItems(player);

        public Lucian()
        {
            LXOrbwalker.AfterAttack += AfterAttack;

            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.Infinity_Edge),
                            new ConditionalItem(ItemId.Berserkers_Greaves),
                            new ConditionalItem(ItemId.Statikk_Shiv),
                            new ConditionalItem(ItemId.Blade_of_the_Ruined_King),
                            new ConditionalItem(ItemId.Last_Whisper),
                            new ConditionalItem(ItemId.Banshees_Veil),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Pickaxe
                        }
            };
        }

        private void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            var hero = target as AIHeroClient;
            if (hero == null || LXOrbwalker.CurrentMode != LXOrbwalker.Mode.Combo) return;

            if (Q.IsReady())
            {
                useQonTarg((AIHeroClient)target, QhitChance.medium);
            }

            if (W.IsReady() && !Q.IsReady() && player.Mana >= 120 && !tooEasyKill(hero))
            {
                W.Cast(hero.Position);
            }

            if (!useQonTarg(hero, QhitChance.hard))
                eAwayFrom();
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady() || target == null)
                return;
            useQonTarg((AIHeroClient)target, QhitChance.medium);
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady())
                return;
            if (player.HealthPercent < 35)
                W.Cast(player.Position);
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || target == null)
                return;
            E.Cast();
        }


        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady() || target == null)
                return;
            if (safeGap(target))
                R.Cast(target);
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(670);
            if (tar != null) useQ(tar);
        }

        public override void setUpSpells()
        {
            //Create the spells
            Q = new Spell(SpellSlot.Q, 600);
            W = new Spell(SpellSlot.W, 700);
            E = new Spell(SpellSlot.E, 325);
            R = new Spell(SpellSlot.R, 800);
        }

        public float fullComboOn(Obj_AI_Base targ)
        {
            float dmg = (float)player.LSGetAutoAttackDamage(targ) * 3;
            if (Q.IsReady())
                dmg += Q.GetDamage(targ);
            if (W.IsReady())
                dmg += W.GetDamage(targ);
            return dmg;
        }

        public bool tooEasyKill(Obj_AI_Base target)
        {
            return target.Health < player.LSGetAutoAttackDamage(target) * 1.5f;
        }

        public bool enemIsOnMe(Obj_AI_Base target)
        {
            if (!target.IsMelee() || target.IsAlly || target.IsDead)
                return false;

            float distTo = target.LSDistance(player, true);
            float targetReack = target.AttackRange + target.BoundingRadius + player.BoundingRadius + 100;
            if (distTo > targetReack * targetReack)
                return false;

            var per = target.Direction.LSTo2D().LSPerpendicular();
            var dir = new Vector3(per, 0);
            var enemDir = target.Position + dir * 40;
            if (distTo < enemDir.LSDistance(player.Position, true))
                return false;

            return true;
        }

        public void eAwayFrom()
        {
            if (!E.IsReady())
                return;
            Vector2 backTo = player.Position.LSTo2D();
            AIHeroClient targ = null;
            int count = 0;
            foreach (var enem in ObjectManager.Get<AIHeroClient>().Where(enemIsOnMe))
            {
                targ = enem;
                count++;
                backTo -= (enem.Position - player.Position).LSTo2D();
            }

            if (count == 1 && targ.Health > fullComboOn(targ))
            { }

            if (count > 1 || (count == 1 && targ.Health > fullComboOn(targ)))
            {
                var awayTo = player.Position.LSTo2D().LSExtend(backTo, 425);
                if (!inTowerRange(awayTo))
                    E.Cast(awayTo);
            }
        }
        public bool inTowerRange(Vector2 pos)
        {
            foreach (Obj_AI_Turret tur in ObjectManager.Get<Obj_AI_Turret>().Where(tur => tur.IsEnemy && tur.Health > 0))
            {
                if (pos.LSDistance(tur.Position.LSTo2D()) < (850 + player.BoundingRadius))
                    return true;
            }
            return false;
        }

        public void useItems(AIHeroClient target)
        {
            if (target.LSDistance(player) < 500)
            {
                sumItems.cast(SummonerItems.ItemIds.Ghostblade);
            }
            if (target.LSDistance(player) < 300)
            {
                sumItems.cast(SummonerItems.ItemIds.Hydra);
            }
            if (target.LSDistance(player) < 300)
            {
                sumItems.cast(SummonerItems.ItemIds.Tiamat);
            }
            if (target.LSDistance(player) < 300)
            {
                sumItems.cast(SummonerItems.ItemIds.Cutlass, target);
            }
            if (target.LSDistance(player) < 500 && (player.Health / player.MaxHealth) * 100 < 85)
            {
                sumItems.cast(SummonerItems.ItemIds.BotRK, target);
            }
        }

        public bool useQonTarg(AIHeroClient target, QhitChance hitChance)
        {
            if (!Q.IsReady())
                return false;

            if (targValidForQ(target))
            {
                Q.CastOnUnit(target);
                return true;
            }

            var bestQon =
                ObjectManager.Get<Obj_AI_Base>()
                    .Where(targValidForQ)
                    .OrderBy(hit => hitChOnTarg(target, hit))
                    .FirstOrDefault();
            if (bestQon != null && hitChOnTarg(target, bestQon) <= hitChance)
            {
                Q.CastOnUnit(bestQon);
                return true;
            }
            return false;
        }

        public bool targValidForQ(Obj_AI_Base targ)
        {
            if (targ.MagicImmune || targ.IsDead || !targ.IsTargetable)
                return false;
            if (targ.IsAlly)
                return false;
            var dist = targ.Position.LSTo2D().LSDistance(player.Position.LSTo2D(), true);
            var realQRange = Q.Range + targ.BoundingRadius;
            if (dist > realQRange * realQRange)
                return false;
            return true;
        }

        public QhitChance hitChOnTarg(AIHeroClient target, Obj_AI_Base onTarg)
        {
            if (target.NetworkId == onTarg.NetworkId)
                return QhitChance.himself;

            var poly = getPolygonOn(onTarg, target.BoundingRadius * 0.6f);
            var predTarPos = Prediction.GetPrediction(target, 0.35f).UnitPosition.LSTo2D();
            var nowPos = target.Position.LSTo2D();

            bool nowInside = poly.pointInside(nowPos);
            bool predInsode = poly.pointInside(predTarPos);

            if (nowInside && predInsode)
                return QhitChance.easy;
            if (predInsode)
                return QhitChance.medium;
            if (nowInside)
                return QhitChance.hard;

            return QhitChance.wontHit;
        }

        public Polygon getPolygonOn(Obj_AI_Base target, float bonusW = 0)
        {
            List<Vector2> points = new List<Vector2>();
            Vector2 rTpos = Prediction.GetPrediction(target, 0.10f).UnitPosition.LSTo2D();
            Vector2 startP = player.ServerPosition.LSTo2D();
            Vector2 endP = startP.LSExtend(rTpos, 1100 + bonusW);

            Vector2 p = (rTpos - startP);
            var per = p.LSPerpendicular().LSNormalized() * (Q.Width / 2 + bonusW);
            points.Add(startP + per);
            points.Add(startP - per);
            points.Add(endP - per);
            points.Add(endP + per);

            return new Polygon(points);
        }

    }
}
