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
    class SionA : Champion
    {
        public Vector2 QCastPos = new Vector2();
        private Spell E1;

        public SionA()
        {
            Chat.Print("Sion on");
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.Spirit_Visage,ItemId.Sunfire_Cape,ItemCondition.ENEMY_AP),
                            new ConditionalItem(ItemId.Ravenous_Hydra_Melee_Only),
                            new ConditionalItem(ItemId.Banshees_Veil,ItemId.Randuins_Omen,ItemCondition.ENEMY_AP),
                            new ConditionalItem(ItemId.Frozen_Mallet),
                            new ConditionalItem(ItemId.Warmogs_Armor),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Mercurys_Treads
                        }
            };
            LXOrbwalker.AfterAttack += afterAttack;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
            Player.OnIssueOrder += onIssueOrder;
        }

        private void onIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (sender.IsMe && RTarg != null && args.Order == GameObjectOrder.MoveTo && player.HasBuff("SionR"))
            {
                if (args.TargetPosition.LSDistance(Game.CursorPos, true) < 10000)
                {
                    args.Process = false;
                    if (RTarg != null)
                        Player.IssueOrder(GameObjectOrder.MoveTo, RTarg.Position, false);
                }
            }
        }

        private void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "SionQ")
            {
                QCastPos = args.End.LSTo2D();
            }


        }

        private void afterAttack(AttackableUnit unit, AttackableUnit target)
        {

        }


        public override void useQ(Obj_AI_Base target)
        {
            if (Q.IsReady() && !player.IsChannelingImportantSpell() && safeGap(player))
            {
                Q.StartCharging(target.ServerPosition);
            }
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady() || target == null)
                return;
            //if (!Q.IsReady(4500) && player.Mana > 200)
            W.Cast();
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || target == null)
                return;
            E.Cast(target);
        }

        public void useE1(Obj_AI_Base target)
        {
            if (!E1.IsReady() || target == null)
                return;
            var pred = E1.GetPrediction(target);
            if (pred.Hitchance>= HitChance.High &&
                pred.CollisionObjects.Any(
                    obj => !obj.IsDead && obj is Obj_AI_Minion && obj.LSDistance(player, true) < E.Range*E.Range))
                E1.Cast(target);
        }

        private Obj_AI_Base RTarg;

        public override void useR(Obj_AI_Base target)
        {
            if (player.HasBuff("SionR"))
            {
                if (target.LSDistance(player) < 150)
                    R.Cast();
            }

            if (target == null || !R.IsReady() || !safeGap(target))
                return;
            RTarg = target;
            R.Cast(target.Position);
        }

        public override void useSpells()
        {

            if (Q.IsCharging)
            {
                var start = ObjectManager.Player.ServerPosition.LSTo2D();
                var end = start.LSExtend(QCastPos, Q.Range);
                var direction = (end - start).LSNormalized();
                var normal = direction.LSPerpendicular();
                var points = new List<Vector2>();
                points.Add(start + normal * (Q.Width));
                points.Add(start - normal * (Q.Width));
                points.Add(end - normal * (Q.Width));
                points.Add(end + normal * (Q.Width));
                
                Polygon pol = new Polygon(points);
                var enesInside = HeroManager.Enemies.Where(ene => !ene.IsDead && pol.pointInside(ene.Position.LSTo2D())).ToList();
                if(enesInside.Count == 0)
                    Q.Cast();
                /*
                for (var i = 0; i <= points.Count - 1; i++)
                {
                    var A = points[i];
                    var B = points[i == points.Count - 1 ? 0 : i + 1];

                    if (enesInside.Any(targ => targ.ServerPosition.LSTo2D().LSDistance(A, B, true, true) < 55 * 55))
                    {
                        Q.Cast();
                    }
                }
                return;*/
            }
            var tar = ARAMTargetSelector.getBestTarget(R.Range);
            if (tar != null) useR(tar);
            tar = ARAMTargetSelector.getBestTarget(580);
            if (tar != null) useQ(tar);
            if (player.IsChannelingImportantSpell())
                return;

            
            tar = ARAMTargetSelector.getBestTarget(W.Range);
            if (tar != null) useW(tar);
            tar = ARAMTargetSelector.getBestTarget(E1.Range);
            if (tar != null) useE1(tar);
            tar = ARAMTargetSelector.getBestTarget(E.Range);
            if (tar != null) useE(tar);
            
        }


        private bool sionZombie()
        {
            return player.Spellbook.GetSpell(SpellSlot.Q).Name == player.Spellbook.GetSpell(SpellSlot.E).Name ||
                   player.Spellbook.GetSpell(SpellSlot.W).Name == player.Spellbook.GetSpell(SpellSlot.R).Name;
        }

        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q, 920);
            Q.SetSkillshot(0.6f, 100f, float.MaxValue, false, SkillshotType.SkillshotLine);
            Q.SetCharged("SionQ", "SionQ", 600, 920, 2.0f);
            W = new Spell(SpellSlot.W, 450);
            E = new Spell(SpellSlot.E, 800);
            E.SetSkillshot(0.25f, 80f, 1800, false, SkillshotType.SkillshotLine);
            E1 = new Spell(SpellSlot.E, 1500);
            E1.SetSkillshot(0.25f, 80f, 1800, false, SkillshotType.SkillshotLine);
            R = new Spell(SpellSlot.R, 1900);

        }

        public override void alwaysCheck()
        {
            if (sionZombie() && Q.IsReady() && player.LSCountEnemiesInRange(350) != 0)
            {
                Console.WriteLine("Zombie Q cast");
                Q.Cast();
            }
        }

        public override void farm()
        {
            if (player.ManaPercent < 55)
                return;

            var AllMinions = MinionManager.GetMinions(player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health);
            foreach (var minion in AllMinions)
            {
                if (E.IsReady() && E.GetDamage(minion) > minion.Health)
                {
                    E.Cast(minion);
                }
            }
        }
    }
}
