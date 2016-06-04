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
    class Syndra : Champion
    {

        private Spell EQ;

        private static int QEComboT;
        private static int WEComboT;

        public Syndra()
        {
            Obj_AI_Base.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;

            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.Morellonomicon),
                            new ConditionalItem(ItemId.Sorcerers_Shoes),
                            new ConditionalItem(ItemId.Rabadons_Deathcap),
                            new ConditionalItem(ItemId.Zhonyas_Hourglass),
                            new ConditionalItem(ItemId.Void_Staff ),
                            new ConditionalItem(ItemId.Banshees_Veil),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Fiendish_Codex,ItemId.Boots_of_Speed
                        }
            };
        }

        void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {

            if (player.LSDistance(sender) < E.Range && E.IsReady())
            {
                Q.Cast(sender.ServerPosition);
                E.Cast(sender.ServerPosition);
            }
            else if (player.LSDistance(sender) < EQ.Range && E.IsReady() && Q.IsReady())
            {
                UseQE(sender);
            }
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady() || target == null)
                return;
            Q.Cast();
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady())
                return;
            if (player.HealthPercent < 30)
                W.Cast();
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || target == null)
                return;
            if (safeGap(target))
                E.CastOnUnit(target);
        }


        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady() || target == null)
                return;
            if (player.LSCountEnemiesInRange(250) > 1)
            {
                R.Cast();
            }
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(Q.Range+250);
            if(tar != null)
                useSyndraSpells(true, true, true, true, true, true, false);
            else if(player.ManaPercent > 40)
                Farm(false);

        }

        public override void setUpSpells()
        {
            //Create the spells
            Q = new Spell(SpellSlot.Q, 790);
            W = new Spell(SpellSlot.W, 925);
            E = new Spell(SpellSlot.E, 700);
            R = new Spell(SpellSlot.R, 675);
            EQ = new Spell(SpellSlot.Q, Q.Range + 500);

            Q.SetSkillshot(0.6f, 125f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(0.25f, 140f, 1600f, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.25f, (float)(45 * 0.5), 2500f, false, SkillshotType.SkillshotCircle);
            EQ.SetSkillshot(float.MaxValue, 55f, 2000f, false, SkillshotType.SkillshotCircle);
        }






        private void UseE(Obj_AI_Base enemy)
        {
            foreach (var orb in OrbManager.GetOrbs(true))
                if (player.LSDistance(orb) < E.Range + 100)
                {
                    var startPoint = orb.LSTo2D().LSExtend(player.ServerPosition.LSTo2D(), 100);
                    var endPoint = player.ServerPosition.LSTo2D()
                        .LSExtend(orb.LSTo2D(), player.LSDistance(orb) > 200 ? 1300 : 1000);
                    EQ.Delay = E.Delay + player.LSDistance(orb) / E.Speed;
                    EQ.From = orb;
                    var enemyPred = EQ.GetPrediction(enemy);
                    if (enemyPred.Hitchance >= HitChance.High &&
                        enemyPred.UnitPosition.LSTo2D().LSDistance(startPoint, endPoint, false) <
                        EQ.Width + enemy.BoundingRadius)
                    {
                        E.Cast(orb, true);
                        W.LastCastAttemptT = LXOrbwalker.now;
                        return;
                    }
                }
        }

        private void UseQE(Obj_AI_Base enemy)
        {
            EQ.Delay = E.Delay + Q.Range / E.Speed;
            EQ.From = player.ServerPosition.LSTo2D().LSExtend(enemy.ServerPosition.LSTo2D(), Q.Range).To3D();

            var prediction = EQ.GetPrediction(enemy);
            if (prediction.Hitchance >= HitChance.High)
            {
                Q.Cast(player.ServerPosition.LSTo2D().LSExtend(prediction.CastPosition.LSTo2D(), Q.Range - 100));
                QEComboT = LXOrbwalker.now;
                W.LastCastAttemptT = LXOrbwalker.now;
            }
        }

        private Vector3 GetGrabableObjectPos(bool onlyOrbs)
        {
            if (!onlyOrbs)
                foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.LSIsValidTarget(W.Range))
                    )
                    return minion.ServerPosition;

            return OrbManager.GetOrbToGrab((int)W.Range);
        }

        private float GetComboDamage(Obj_AI_Base enemy)
        {
            var damage = 0d;

            if (Q.IsReady(420))
                damage += player.LSGetSpellDamage(enemy, SpellSlot.Q);

            if (W.IsReady())
                damage += player.LSGetSpellDamage(enemy, SpellSlot.W);

            if (E.IsReady())
                damage += player.LSGetSpellDamage(enemy, SpellSlot.E);


            if (R.IsReady())
                damage += Math.Min(7, player.Spellbook.GetSpell(SpellSlot.R).Ammo) * player.LSGetSpellDamage(enemy, SpellSlot.R, 1);

            return (float)damage;
        }

        private void useSyndraSpells(bool useQ, bool useW, bool useE, bool useR, bool useQE, bool useIgnite, bool isHarass)
        {
            var qTarget = ARAMTargetSelector.getBestTarget(Q.Range + (isHarass ? Q.Width / 3 : Q.Width));
            var wTarget = ARAMTargetSelector.getBestTarget(W.Range + W.Width);
            var rTarget = ARAMTargetSelector.getBestTarget(R.Range);
            var qeTarget = ARAMTargetSelector.getBestTarget(EQ.Range);
            var comboDamage = rTarget != null ? GetComboDamage(rTarget) : 0;

            //Q
            if (qTarget != null && useQ)
                Q.Cast(qTarget, false, true);

            //E
            if (LXOrbwalker.now - W.LastCastAttemptT > Game.Ping + 150 && E.IsReady() && useE)
                foreach (var enemy in ObjectManager.Get<AIHeroClient>())
                {
                    if (enemy.LSIsValidTarget(EQ.Range))
                        UseE(enemy);
                }

            //W
            if (useW)
            {
                if (wTarget != null &&W.IsReady() && player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 1)
                {
                    W.Cast(wTarget);
                }
                if (player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 1 && W.IsReady() && qeTarget != null)
                {
                    //WObject
                    var gObjectPos = GetGrabableObjectPos(wTarget == null);

                    if (gObjectPos.LSTo2D().IsValid() && Utils.TickCount - W.LastCastAttemptT > Game.Ping + 300 &&
                        Utils.TickCount - E.LastCastAttemptT > Game.Ping + 300)
                    {
                        W.Cast(gObjectPos);
                        W.LastCastAttemptT = Utils.TickCount;
                    }
                }
                else if (wTarget != null && player.Spellbook.GetSpell(SpellSlot.W).ToggleState != 1 && W.IsReady() &&
                         Utils.TickCount - W.LastCastAttemptT > Game.Ping + 100)
                {
                    if (OrbManager.WObject(false) != null)
                    {
                        W.From = OrbManager.WObject(false).ServerPosition;
                        W.Cast(wTarget, false, true);
                    }
                }
            }

            if (rTarget != null)
                useR = true;//Config.Item("DontUlt" + rTarget.BaseSkinName) != null &&

            if (rTarget != null && useR && comboDamage > rTarget.Health)
            {
                if (R.IsReady())
                {
                    R.Cast(rTarget);
                }
            }

            //R
            if (rTarget != null && useR && R.IsReady() && !Q.IsReady())
            {
                if (comboDamage > rTarget.Health)
                {
                    R.Cast(rTarget);
                }
            }


            //QE
            if ( qeTarget != null && Q.IsReady() && E.IsReady() && useQE)
                UseQE(qeTarget);

            //WE
            if (qeTarget != null && E.IsReady() && useE && OrbManager.WObject(true) != null)
            {
                EQ.Delay = E.Delay + Q.Range / W.Speed;
                EQ.From = player.ServerPosition.LSTo2D().LSExtend(qeTarget.ServerPosition.LSTo2D(), Q.Range).To3D();
                var prediction = EQ.GetPrediction(qeTarget);
                if (prediction.Hitchance >= HitChance.High)
                {
                    W.Cast(player.ServerPosition.LSTo2D().LSExtend(prediction.CastPosition.LSTo2D(), Q.Range - 100));
                    WEComboT = LXOrbwalker.now;
                }
            }
        }

        private void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && LXOrbwalker.now - QEComboT < 500 &&
                (args.SData.Name == "SyndraQ"))
            {
                W.LastCastAttemptT = LXOrbwalker.now + 400;
                E.Cast(args.End, true);
            }

            if (sender.IsMe && LXOrbwalker.now - WEComboT < 500 &&
                (args.SData.Name == "SyndraW" || args.SData.Name == "syndrawcast"))
            {
                W.LastCastAttemptT = LXOrbwalker.now + 400;
                E.Cast(args.End, true);
            }
        }

        private void Farm(bool laneClear)
        {
            if (!LXOrbwalker.CanMove()) return;

            var rangedMinionsQ = MinionManager.GetMinions(player.ServerPosition, Q.Range + Q.Width + 30,
                MinionTypes.Ranged);
            var allMinionsQ = MinionManager.GetMinions(player.ServerPosition, Q.Range + Q.Width + 30,
                MinionTypes.All);
            var rangedMinionsW = MinionManager.GetMinions(player.ServerPosition, W.Range + W.Width + 30,
                MinionTypes.Ranged);
            var allMinionsW = MinionManager.GetMinions(player.ServerPosition, W.Range + W.Width + 30,
                MinionTypes.All);


            if (Q.IsReady())
                if (laneClear)
                {
                    var fl1 = Q.GetCircularFarmLocation(rangedMinionsQ, Q.Width);
                    var fl2 = Q.GetCircularFarmLocation(allMinionsQ, Q.Width);

                    if (fl1.MinionsHit >= 3)
                    {
                        Q.Cast(fl1.Position);
                    }

                    else if (fl2.MinionsHit >= 2 || allMinionsQ.Count == 1)
                    {
                        Q.Cast(fl2.Position);
                    }
                }
                else
                    foreach (var minion in allMinionsQ)
                        if (!Orbwalking.InAutoAttackRange(minion) &&
                            minion.Health < 0.75 * player.LSGetSpellDamage(minion, SpellSlot.Q))
                            Q.Cast(minion);

            if (W.IsReady() && allMinionsW.Count > 3)
            {
                if (laneClear)
                {
                    if (player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 1)
                    {
                        //WObject
                        var gObjectPos = GetGrabableObjectPos(false);

                        if (gObjectPos.LSTo2D().IsValid() && LXOrbwalker.now - W.LastCastAttemptT > Game.Ping + 150)
                        {
                            W.Cast(gObjectPos);
                        }
                    }
                    else if (player.Spellbook.GetSpell(SpellSlot.W).ToggleState != 1)
                    {
                        var fl1 = Q.GetCircularFarmLocation(rangedMinionsW, W.Width);
                        var fl2 = Q.GetCircularFarmLocation(allMinionsW, W.Width);

                        if (fl1.MinionsHit >= 3 && W.IsInRange(fl1.Position.To3D()))
                        {
                            W.Cast(fl1.Position);
                        }

                        else if (fl2.MinionsHit >= 1 && W.IsInRange(fl2.Position.To3D()) && fl1.MinionsHit <= 2)
                        {
                            W.Cast(fl2.Position);
                        }
                    }
                }
            }
        }

    }
    public static class OrbManager
    {
        private static int _wobjectnetworkid = -1;

        public static int WObjectNetworkId
        {
            get
            {
                if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 1)
                    return -1;

                return _wobjectnetworkid;
            }
            set
            {
                _wobjectnetworkid = value;
            }
        }

        public static int tmpQOrbT;
        public static Vector3 tmpQOrbPos = new Vector3();

        public static int tmpWOrbT;
        public static Vector3 tmpWOrbPos = new Vector3();

        static OrbManager()
        {
            //Obj_AI_Base.on += Obj_AI_Base_OnPauseAnimation;
            Obj_AI_Base.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
        }

        //static void Obj_AI_Base_OnPauseAnimation(Obj_AI_Base sender, Obj_AI_BasePauseAnimationEventArgs args)
        //{
            //if (sender is Obj_AI_Minion && sender.IsAlly)
            //{
                //WObjectNetworkId = sender.NetworkId;
            //}
        //}

        private static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "SyndraQ")
            {
                tmpQOrbT = Utils.TickCount;
                tmpQOrbPos = args.End;
            }

            if (sender.IsMe && WObject(true) != null && (args.SData.Name == "SyndraW" || args.SData.Name == "syndraw2"))
            {
                tmpWOrbT = Utils.TickCount + 250;
                tmpWOrbPos = args.End;
            }
        }

        public static Obj_AI_Minion WObject(bool onlyOrb)
        {
            if (WObjectNetworkId == -1) return null;
            var obj = ObjectManager.GetUnitByNetworkId<Obj_AI_Base>((uint)WObjectNetworkId);
            if (obj != null && obj.IsValid<Obj_AI_Minion>() && (obj.Name == "Seed" && onlyOrb || !onlyOrb)) return (Obj_AI_Minion)obj;
            return null;
        }

        public static List<Vector3> GetOrbs(bool toGrab = false)
        {
            var result = new List<Vector3>();
            foreach (
                var obj in
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(obj => obj.IsValid && obj.Team == ObjectManager.Player.Team && obj.Name == "Seed"))
            {

                var valid = false;
                if (obj.NetworkId != WObjectNetworkId)
                    if (
                        ObjectManager.Get<GameObject>()
                            .Any(
                                b =>
                                    b.IsValid && b.Name.Contains("_Q_") && b.Name.Contains("Syndra_") &&
                                    b.Name.Contains("idle") && obj.Position.LSDistance(b.Position) < 50))
                        valid = true;

                if (valid && (!toGrab || !obj.IsMoving))
                    result.Add(obj.ServerPosition);
            }

            if (Utils.TickCount - tmpQOrbT < 400)
            {
                result.Add(tmpQOrbPos);
            }

            if (Utils.TickCount - tmpWOrbT < 400 && Utils.TickCount - tmpWOrbT > 0)
            {
                result.Add(tmpWOrbPos);
            }

            return result;
        }

        public static Vector3 GetOrbToGrab(int range)
        {
            var list = GetOrbs(true).Where(orb => ObjectManager.Player.LSDistance(orb) < range).ToList();
            return list.Count > 0 ? list[0] : new Vector3();
        }
    }

}
