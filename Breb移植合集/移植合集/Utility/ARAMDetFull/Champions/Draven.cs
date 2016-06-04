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
    class Draven : Champion
    {

        private List<PossibleReticle> Axes = new List<PossibleReticle>(); 
        public Draven()
        {//have to fix done
            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
            Interrupter.OnPossibleToInterrupt += Interrupter_OnPossibleToInterrupt;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            LXOrbwalker.AfterAttack += LXOrbwalker_AfterAttack;

            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.The_Bloodthirster),
                            new ConditionalItem(ItemId.Berserkers_Greaves),
                            new ConditionalItem(ItemId.Infinity_Edge),
                            new ConditionalItem(ItemId.Phantom_Dancer),
                            new ConditionalItem(ItemId.Last_Whisper),
                            new ConditionalItem(ItemId.Guardian_Angel),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Vampiric_Scepter,ItemId.Boots_of_Speed
                        }
            };
        }

        void LXOrbwalker_AfterAttack(Obj_AI_Base unit, Obj_AI_Base target)
        {
            //Chat.Print("Registered");
            if (!(target is AIHeroClient)) return;
            if (unit.IsMe && target.LSIsValidTarget())
            {
                bool useW;
                var axe = getClosestAxe(out useW);

                if(axe!=null)
                    Player.IssueOrder(GameObjectOrder.MoveTo, axe.Position);
                else if (player.LSCountEnemiesInRange(550)==0)
                    Player.IssueOrder(GameObjectOrder.MoveTo, target.Position);
                else
                    Player.IssueOrder(GameObjectOrder.MoveTo,player.Position.LSExtend(ARAMSimulator.fromNex.Position,450));

                CastW(target);
                //castItems((AIHeroClient)target);
            }
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady())
                return;
            Q.Cast(target);
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady())
                return;
            W.Cast(target);
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady())
                return;
            if (EnemyInRange(1, 300))
                E.Cast(player.Position.LSTo2D().LSExtend(ARAMSimulator.fromNex.Position.LSTo2D(), 400));

        }

        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady())
                return;
            R.CastIfWillHit(target,2);
        }

        public override void setUpSpells()
        {
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 1100);
            R = new Spell(SpellSlot.R, 4000);
            E.SetSkillshot(250f, 130f, 1400f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(400f, 160f, 2000f, false, SkillshotType.SkillshotLine);
        }

        public override void alwaysCheck()
        {
            if (Axes.Count == 0)
            {
                LXOrbwalker.CustomOrbwalkMode = false;
            }
        }

        public override void useSpells()
        {
            var target = ARAMTargetSelector.getBestTarget(LXOrbwalker.GetAutoAttackRange());
            var Etarget = ARAMTargetSelector.getBestTarget(E.Range);
            var RTarget = ARAMTargetSelector.getBestTarget(2000f);
            CatchAxes();

            if (target.LSIsValidTarget()) CastQ();
            if (Etarget.LSIsValidTarget()) CastE(Etarget);
            if (RTarget.LSIsValidTarget()) CastRExecute(RTarget);
            if (RTarget.LSIsValidTarget()) { RExecute(RTarget); }
            
        }

        public bool EnemyInRange(int numOfEnemy, float range)
        {
            return LeagueSharp.Common.Utility.LSCountEnemiesInRange(player, (int)range) >= numOfEnemy;
        }

        void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var GPSender = (AIHeroClient)gapcloser.Sender;
            if (!E.IsReady() || !GPSender.LSIsValidTarget()) return;
            CastEHitchance(GPSender);

        }

        void Interrupter_OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            var Sender = (AIHeroClient)unit;
            if ( !E.IsReady() || !Sender.LSIsValidTarget()) return;
            CastEHitchance(Sender);
        }

        void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (!sender.Name.Contains("Q_reticle_self")) return;
            Axes.RemoveAll(ax => ax.networkID == sender.NetworkId);
           
        }

        void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (!sender.Name.Contains("Q_reticle_self")) return;
            Axes.Add(new PossibleReticle(sender));
        }

        public PossibleReticle getClosestAxe(out bool useW)
        {

            if (Axes.Count <= 0)
            {
                useW = false;
                return null;
            }
            var CatchRange = 560;
            var UseW = false;

            bool ShouldUseW;

            var Axe = Axes.Where(
                    axe =>
                        axe.AxeGameObject.IsValid && axe.Position.LSDistance(player.Position) <= CatchRange).OrderBy(axe => axe.Distance())
                        .FirstOrDefault();
            if (Axe == null)
            {
                useW = false;
                return null;
            }
            if (Axe.canCatch(UseW, out ShouldUseW))
            {
                useW = ShouldUseW;
                return Axe;
            }
            useW = false;
            return null;
        }

        public void CatchAxes()
        {
            bool shouldUseWForIt;
            //Chat.Print("I'm Combo");
            if (Axes.Count == 0)
            {
                LXOrbwalker.CustomOrbwalkMode = false;
                return;
            }
            if (LXOrbwalker.inDanger)
                return;
            var Axe = getClosestAxe(out shouldUseWForIt);

            if (Axe == null)
            {
                LXOrbwalker.CustomOrbwalkMode = false;
                return;
            }
            //  if (shouldUseWForIt) { LXOrbwalker.SetAttack(false); } else { LXOrbwalker.SetAttack(true);}
            Catch(shouldUseWForIt, Axe);
                  
        }

        public void CastEHitchance(AIHeroClient target)
        {
            var Pred = E.GetPrediction(target);
            if (Pred.Hitchance >= HitChance.Medium)
            {
                E.Cast(target);
            }
        }
        public void Catch(bool shouldUseWForIt, PossibleReticle Axe)
        {
            if (shouldUseWForIt && W.IsReady() && !Axe.isCatchingNow()) W.Cast();
            LXOrbwalker.CustomOrbwalkMode = true;
            LXOrbwalker.Orbwalk(Axe.Position.LSExtend(player.Position,player.BoundingRadius+10), LXOrbwalker.GetPossibleTarget());
        }
        public void CastQ()
        {
            var ManaQCombo = 0;
            var QMax = 2;
            if (getPerValue(true) >= ManaQCombo && GetQStacks() + 1 <= QMax) Q.Cast();
                 
        }
        private void CastW(Obj_AI_Base target)
        {
            if (hasWBuff() || !W.IsReady()) return;

            var MWC = 0;
            if (getPerValue(true) >= MWC)
            {
                Aggresivity.addAgresiveMove(new AgresiveMove(50,3000,true));
                W.Cast();
            }

        }

        private void CastE(AIHeroClient target)
        {
            if (!E.IsReady() || !target.LSIsValidTarget()) return;
                    CastEHitchance(target);
        }

        private void CastRExecute(AIHeroClient RTarget)
        {
            var Pred = R.GetPrediction(RTarget);
            if (!RTarget.LSIsValidTarget() || Pred.Hitchance < HitChance.Medium || !R.IsReady()) return;
            var ManaR = 0;
            if (getUnitsInPath(player, RTarget, R) && getPerValue(true) >= ManaR && !player.HasBuff("dravenrdoublecast"))
            {
                R.Cast(RTarget);
            }
            
        }

        private void RExecute(AIHeroClient RTarget)
        {
            var Pred = R.GetPrediction(RTarget);
            if (!RTarget.LSIsValidTarget() || Pred.Hitchance < HitChance.Medium || !R.IsReady()) return;
            if (getUnitsInPath(player, RTarget, R) && !player.HasBuff("dravenrdoublecast"))
            {
                R.Cast(RTarget);
            }
        }
        void castItems(AIHeroClient tar)
        {
            if (tar == null)
                return;
            UseItem(3153, tar);
            UseItem(3153, tar);
            UseItem(3142);
            UseItem(3142);
            UseItem(3144, tar);
            UseItem(3144, tar);
        }
        private bool hasWBuff()
        {
            //dravenfurybuff
            //DravenFury
            return player.HasBuff("DravenFury") || player.HasBuff("dravenfurybuff");
        }
        public Vector3 PosAfterRange(Vector3 p1, Vector3 finalp2, float range)
        {
            var Pos2 = Vector3.Normalize(finalp2 - p1);
            return p1 + (Pos2 * range);
        }
        public int GetQStacks()
        {
            var buff = player.Buffs.FirstOrDefault(buff1 => buff1.Name.Equals("dravenspinningattack"));
            return buff != null ? buff.Count : 0;
        }

        float getPerValue(bool mana)
        {
            if (mana) return (player.Mana / player.MaxMana) * 100;
            return (player.Health / player.MaxHealth) * 100;
        }
        float getPerValueTarget(AIHeroClient target, bool mana)
        {
            if (mana) return (target.Mana / target.MaxMana) * 100;
            return (target.Health / target.MaxHealth) * 100;
        }
        public void UseItem(int id, AIHeroClient target = null)
        {
            if (LeagueSharp.Common.Items.HasItem(id) && LeagueSharp.Common.Items.CanUseItem(id))
            {
                LeagueSharp.Common.Items.UseItem(id, target);
            }
        }
        public static bool isUnderEnTurret(Vector3 Position)
        {
            foreach (var tur in ObjectManager.Get<Obj_AI_Turret>().Where(turr => turr.IsEnemy && (turr.Health != 0)))
            {
                if (tur.LSDistance(Position) <= 975f) return true;
            }
            return false;
        }
        private  bool getUnitsInPath(AIHeroClient player, AIHeroClient target, Spell spell)
        {
            float distance = player.LSDistance(target);
            List<Obj_AI_Base> minionList = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, spell.Range,
                MinionTypes.All, MinionTeam.NotAlly);
            int numberOfMinions = (from Obj_AI_Minion minion in minionList
                                   let skillshotPosition =
                                       V2E(player.Position,
                                           V2E(player.Position, target.Position,
                                               Vector3.Distance(player.Position, target.Position) - spell.Width + 1).To3D(),
                                           Vector3.Distance(player.Position, minion.Position))
                                   where skillshotPosition.LSDistance(minion) < spell.Width
                                   select minion).Count();
            int numberOfChamps = (from minion in ObjectManager.Get<AIHeroClient>()
                                  let skillshotPosition =
                                      V2E(player.Position,
                                          V2E(player.Position, target.Position,
                                              Vector3.Distance(player.Position, target.Position) - spell.Width + 1).To3D(),
                                          Vector3.Distance(player.Position, minion.Position))
                                  where skillshotPosition.LSDistance(minion) < spell.Width && minion.IsEnemy
                                  select minion).Count();
            int totalUnits = numberOfChamps + numberOfMinions - 1;
            // total number of champions and minions the projectile will pass through.
            if (totalUnits == -1) return false;
            double damageReduction = 0;
            damageReduction = ((totalUnits > 7)) ? 0.4 : (totalUnits == 0) ? 1.0 : (1 - ((totalUnits) / 12.5));
            // the damage reduction calculations minus percentage for each unit it passes through!
            return spell.GetDamage(target) * damageReduction >= (target.Health + (distance / 2000) * target.HPRegenRate);
            // - 15 is a safeguard for certain kill.
        }
        private Vector2 V2E(Vector3 from, Vector3 direction, float distance)
        {
            return from.LSTo2D() + distance * Vector3.Normalize(direction - from).LSTo2D();
        }

        internal class PossibleReticle
        {
            public GameObject AxeGameObject;
            public int networkID;
            public Vector3 Position;
            public int CreationTime;
            public int EndTime;

            public PossibleReticle(GameObject Axe)
            {
                AxeGameObject = Axe;
                networkID = Axe.NetworkId;
                Position = Axe.Position;
                CreationTime = LXOrbwalker.now;
                EndTime = LXOrbwalker.now + 2000;
            }

            public bool canCatch(bool UseW,out bool ShouldUseW)
            {
                var EnemyHeroesCount =
                    ObjectManager.Get<AIHeroClient>()
                        .Where(h => h.IsEnemy && h.LSIsValidTarget() && h.LSDistance(Position) <= 350).ToList();
                if ((isUnderEnTurret(Position) && !isUnderEnTurret(player.Position)) || EnemyHeroesCount.Count > 1)
                {
                    ShouldUseW = false;
                    return false;
                }
                Spell W = new Spell(SpellSlot.W);
                var distance = player.GetPath(Position).ToList().LSTo2D().PathLength()-50;
                var catchNormal = (distance * 1000) / player.MoveSpeed + LXOrbwalker.now < EndTime; // Not buffed with W, Normal
                var AdditionalSpeed = (5*W.Level + 35)*0.01*player.MoveSpeed;
                var catchBuff = distance / (player.MoveSpeed + AdditionalSpeed) + LXOrbwalker.now < EndTime; //Buffed with W
                if (catchNormal)
                {
                    ShouldUseW = false;
                    return catchNormal;
                }
                if (UseW && !catchNormal && catchBuff)
                {
                    ShouldUseW = true;
                    return catchBuff;
                }
                ShouldUseW = false;
                return false;
            }
            public float Distance()
            {
                return Vector3.Distance(Position, player.Position);
            }

            public bool isCatchingNow()
            {
                return Distance() < player.BoundingRadius; //Taken from PUC Draven
            }
        }

    }
}
