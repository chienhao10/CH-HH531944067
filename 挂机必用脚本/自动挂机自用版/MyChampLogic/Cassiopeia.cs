using System;
using System.IO;
using System.Linq;
using AutoBuddy.Humanizers;
using AutoBuddy.MainLogics;
using AutoBuddy.Utilities;
using AutoBuddy.Utilities.AutoShop;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;
using Color = System.Drawing.Color;

namespace AutoBuddy.MyChampLogic
{
    internal class Cassiopeia : IChampLogic
    {
        public float MaxDistanceForAA { get { return 500; } }
        public float OptimalMaxComboDistance { get { return 500; } }
        public float HarassDistance { get { return 500; } }
        private readonly Spell.Skillshot Q, W, R;
        private readonly Spell.Targeted E;
        private int minManaHarass = 35;
        private int tick;
        private bool isTearOwned;
        private bool qblock;
        private string dmg;
        private StreamWriter file500;
        private StreamWriter file600;
        private StreamWriter file700;
        private readonly bool log = false;
        public Cassiopeia()
        {
            if (BrutalExtensions.GetGameType().Equals("bot_intermediate"))
            {
                log = true;
                AutoWalker.EndGame += end;
                file500 =
                    new StreamWriter(Path.Combine(Environment.GetFolderPath(
                        Environment.SpecialFolder.ApplicationData), "EloBuddy\\AutoBuddyTreeline\\qPred500"), true);
                file600 =
                    new StreamWriter(Path.Combine(Environment.GetFolderPath(
                        Environment.SpecialFolder.ApplicationData), "EloBuddy\\AutoBuddyTreeline\\qPred600"), true);
                file700 =
                    new StreamWriter(Path.Combine(Environment.GetFolderPath(
                        Environment.SpecialFolder.ApplicationData), "EloBuddy\\AutoBuddyTreeline\\qPred700"), true);
                Core.DelayAction(fl, 10000);
            }
            ShopSequence =
                "3340:Buy,2003:StartHpPot,1056:Buy,1027:Buy,3070:Buy,1001:Buy,1058:Buy,3003:Buy,3020:Buy,1028:Buy,1011:Buy,1058:Buy,2003:StopHpPot,3116:Buy,1004:Buy,1004:Buy,3114:Buy,1052:Buy,3108:Buy,3165:Buy,1056:Sell,1058:Buy,3089:Buy,1028:Buy,3136:Buy,3151:Buy";
            Q = new Spell.Skillshot(SpellSlot.Q, 850, SkillShotType.Circular, 600, int.MaxValue, 35);
            W = new Spell.Skillshot(SpellSlot.W, 850, SkillShotType.Circular, 500, 2500, 90);
            R = new Spell.Skillshot(SpellSlot.R, 500, SkillShotType.Cone, 650, int.MaxValue, 75);
            E = new Spell.Targeted(SpellSlot.E, 700);
            updateTearStatus();

            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private void fl()
        {
            file500.Flush();
            file600.Flush();
            file700.Flush();
            Core.DelayAction(fl, 10000);
        }

        private void end(object sender, EventArgs e)
        {
            file500.Close();
            file600.Close();
            file700.Close();
        }

        private void QCast(AIHeroClient target, Vector3 pos)
        {
            if (qblock) return;
            qblock = true;
            Core.DelayAction(qUnblock, 500);
            if (target == null) return;
            Vector3[] vPath = target.Path.Copy();
            Vector3 vPos = pos.Copy();
            Vector3 vTargetPos = target.Position.Copy();
            float ms = target.MoveSpeed;
            float myhp = AutoWalker.myHero.HealthPercent;
            float enemyhp = target.HealthPercent;
            Vector3 vmypos = AutoWalker.myHero.Position.Copy();

            if (log)
            {
                Core.DelayAction(() => qResult(vPath, vPos, vTargetPos, ms, target, myhp, enemyhp, vmypos, file500), 500);
                Core.DelayAction(() => qResult(vPath, vPos, vTargetPos, ms, target, myhp, enemyhp, vmypos, file600), 600);
                Core.DelayAction(() => qResult(vPath, vPos, vTargetPos, ms, target, myhp, enemyhp, vmypos, file700), 700);
            }

        }

        private void qUnblock()
        {
            qblock = false;
        }
        private void qResult(Vector3[] path, Vector3 castpos, Vector3 targetPos, float ms, AIHeroClient tg, float myhp, float targhp, Vector3 castheropos, StreamWriter fi)
        {
            //Console.WriteLine(castpos.Distance(tg));
            if (castpos.Distance(tg) > 600 || !tg.IsVisible()) return;
            //endpos.x endpos.y movespeed startpos.x startpos.y startpath[1].x startpath[1].y startpath[2].x startpath[2].y myhpPerc targHpPer distance
            string res = tg.Position.X + " " + tg.Position.Y + " " + ms + " " + targetPos.X + " " + targetPos.Y;
            for (int i = 1; i < 3; i++)
            {
                if (path.Length <= i)
                    res += " " + path[path.Length - 1].X + " " + path[path.Length - 1].Y;
                else
                    res += " " + path[i].X + " " + path[i].Y;
            }
            res += " " + myhp + " " + targhp + " " + castheropos.Distance(targetPos);
            fi.WriteLine(res);
        }
        private void updateTearStatus()
        {
            isTearOwned = BrutalItemInfo.GetItemSlot(3070) != -1 || BrutalItemInfo.GetItemSlot(3003) != -1;
            Core.DelayAction(updateTearStatus, 5000);
        }

        void Drawing_OnDraw(EventArgs args)
        {
            if (!MainMenu.GetMenu("AB").Get<CheckBox>("debuginfo").CurrentValue)
                return;

            foreach (var vector3 in AutoWalker.myHero.Path)
            {
                Circle.Draw(new ColorBGRA(100, 100, 100, 255), 10, vector3);
            }
            Drawing.DrawText(900, 10, Color.Chocolate, dmg, 70);
        }

        private void Game_OnTick(EventArgs args)
        {
            //Chat.Print(AutoWalker.Recalling());
            AIHeroClient t = EntityManager.Heroes.Enemies.Where(en => en.IsVisible() && en.Distance(Game.CursorPos) < 630).OrderBy(en => en.Health).FirstOrDefault();
            if (t != null)
            {
                float ti = TimeForAttack(t, 630);
                float dm = 0;
                if (EstDmg(t, ti) > 0)
                {
                    dm = EstDmg(t, ti);
                }
                if (AutoWalker.Ignite != null && AutoWalker.Ignite.IsReady() && t.Health > dm && t.Health < dm + (50 + 20 * AutoWalker.myHero.Level))
                    AutoWalker.UseIgnite(t);
                dmg = dm + ", " + (t.Health - dm);
            }

            if (isTearOwned && Q.IsReady() && AutoWalker.myHero.ManaPercent > 95 && !AutoWalker.Recalling() && !EntityManager.Heroes.Enemies.Any(en => en.Distance(AutoWalker.myHero) < 2000) && !EntityManager.MinionsAndMonsters.EnemyMinions.Any(min => min.Distance(AutoWalker.myHero) < 1000))
            {

                QCast(null, new Vector3());
                Q.Cast((Prediction.Position.PredictUnitPosition(AutoWalker.myHero, 2000) +
                       new Vector2(RandGen.r.NextFloat(-200, 200), RandGen.r.NextFloat(-200, 200))).To3D());
            }

            if (Orbwalker.ActiveModesFlags == Orbwalker.ActiveModes.Harass || Orbwalker.ActiveModesFlags == Orbwalker.ActiveModes.LaneClear)
            {
                if (!EntityManager.Heroes.Enemies.Any(en => en.Distance(AutoWalker.myHero) < 650 + en.BoundingRadius))
                {
                    if (Q.IsReady() && AutoWalker.myHero.MaxMana > 750 && AutoWalker.myHero.ManaPercent > 65)
                    {
                        tick++;
                        if (tick % 5 != 0) return;
                        EntityManager.MinionsAndMonsters.FarmLocation f =
                            EntityManager.MinionsAndMonsters.GetCircularFarmLocation(EntityManager.MinionsAndMonsters.GetLaneMinions(radius: 850), 250, 700);
                        if (f.HitNumber >= 4 || (f.HitNumber == 3 && AutoWalker.myHero.ManaPercent > 80))
                        {
                            QCast(null, new Vector3());
                            Q.Cast(f.CastPosition);
                        }


                    }

                    if (E.IsReady())
                    {
                        Obj_AI_Minion minionToE = EntityManager.MinionsAndMonsters.GetLaneMinions(radius: 850).FirstOrDefault(min => min.HasBuffOfType(BuffType.Poison) && min.Distance(AutoWalker.myHero) < min.BoundingRadius + E.Range && Prediction.Health.GetPrediction(min, 100) < AutoWalker.myHero.GetSpellDamage(min, SpellSlot.E) && Prediction.Health.GetPrediction(min, 100) > 0);
                        if (minionToE != null)
                            E.Cast(minionToE);
                        else if (!EntityManager.Heroes.Enemies.Any(en => en.IsVisible() && en.Distance(AutoWalker.myHero) < 1200))
                        {
                            minionToE = EntityManager.MinionsAndMonsters.GetLaneMinions(radius: 850).FirstOrDefault(min => min.Distance(AutoWalker.myHero) < min.BoundingRadius + E.Range && Prediction.Health.GetPrediction(min, 200) < AutoWalker.myHero.GetSpellDamage(min, SpellSlot.E) && Prediction.Health.GetPrediction(min, 200) > 0);
                            if (minionToE != null)
                                E.Cast(minionToE);
                        }
                    }
                }


                if (AutoWalker.myHero.ManaPercent < 15) return;
                AIHeroClient poorVictim = TargetSelector.GetTarget(850, DamageType.Magical, addBoundingRadius: true);
                if (poorVictim != null && minManaHarass < AutoWalker.myHero.HealthPercent)
                {
                    if (Q.IsReady())
                    {
                        PredictionResult pr = Q.GetPrediction(poorVictim);
                        if (pr.HitChancePercent > 35)
                        {
                            QCast(poorVictim, pr.CastPosition);
                            Q.Cast(pr.CastPosition);

                        }
                    }
                    if (E.IsReady())
                    {
                        AIHeroClient candidateForE = EntityManager.Heroes.Enemies.Where(
                            en =>
                                en.HasBuffOfType(BuffType.Poison) && en.IsTargetable &&
                                !en.HasBuffOfType(BuffType.SpellImmunity) && !en.HasBuffOfType(BuffType.Invulnerability) &&
                                en.Distance(AutoWalker.myHero) < en.BoundingRadius + E.Range && !en.IsDead())
                            .OrderBy(en => en.Health / AutoWalker.myHero.GetSpellDamage(en, SpellSlot.E))
                            .FirstOrDefault();
                        if (candidateForE != null)
                            E.Cast(candidateForE);

                    }

                }
            }
            else if (Orbwalker.ActiveModesFlags == Orbwalker.ActiveModes.Combo || Orbwalker.ActiveModesFlags == Orbwalker.ActiveModes.Flee)
            {
                AIHeroClient poorVictim = TargetSelector.GetTarget(700, DamageType.Magical, addBoundingRadius: true) ??
                                          TargetSelector.GetTarget(850, DamageType.Magical, addBoundingRadius: true);

                if (poorVictim != null)
                {
                    if (Q.IsReady())
                    {
                        PredictionResult pr = Q.GetPrediction(poorVictim);
                        if (pr.HitChancePercent > 60)
                        {
                            QCast(poorVictim, pr.CastPosition);
                            Q.Cast(pr.CastPosition);
                        }

                    }
                    if (E.IsReady() && (poorVictim.HasBuffOfType(BuffType.Poison) || AutoWalker.myHero.GetSpellDamage(poorVictim, SpellSlot.E) > poorVictim.Health))
                        E.Cast(poorVictim);
                    else if (E.IsReady())
                    {
                        AIHeroClient an = EntityManager.Heroes.Enemies.Where(en => en.HasBuffOfType(BuffType.Poison) && AutoWalker.myHero.Distance(en) < E.Range + en.BoundingRadius).OrderBy(en => en.Health / AutoWalker.myHero.GetSpellDamage(en, SpellSlot.E))
                            .FirstOrDefault();
                        if (an != null)
                            E.Cast(an);
                    }
                    if (!poorVictim.HasBuffOfType(BuffType.Poison) && W.IsReady() || poorVictim.Distance(AutoWalker.myHero) > 650)
                    {
                        PredictionResult pr = W.GetPrediction(poorVictim);
                        if (pr.HitChance >= HitChance.Medium)
                        {
                            W.Cast(pr.CastPosition);
                        }
                    }
                    if (R.IsReady() && poorVictim.HasBuffOfType(BuffType.Poison) && AutoWalker.myHero.ManaPercent > 35 && poorVictim.Distance(AutoWalker.myHero) > 200 && poorVictim.Distance(AutoWalker.myHero) < 600 + poorVictim.BoundingRadius && poorVictim.IsFacing(AutoWalker.myHero) && poorVictim.HealthPercent > 30 && poorVictim.HealthPercent < 60)
                        R.Cast(Prediction.Position.PredictUnitPosition(poorVictim, 300).To3D());
                    if (R.IsReady() && poorVictim.Distance(AutoWalker.myHero) < 600 && EntityManager.Heroes.Enemies.Count(en => en.IsVisible() && !en.IsDead() && en.Distance(AutoWalker.myHero) < 600) >= 2)
                        R.Cast(Prediction.Position.PredictUnitPosition(poorVictim, 400).To3D());
                    if (Orbwalker.ActiveModesFlags == Orbwalker.ActiveModes.Flee)
                    {
                        if (R.IsReady() && Logic.surviLogic.dangerValue > 10000 && AutoWalker.myHero.HealthPercent < 20)
                        {
                            AIHeroClient champToUlt =
                                EntityManager.Heroes.Enemies.FirstOrDefault(
                                    en =>
                                        en.HealthPercent > 5 && en.Distance(AutoWalker.myHero) < 600 &&
                                        en.Distance(AutoWalker.myHero) > 100);
                            if (champToUlt != null)
                            {
                                R.Cast(Prediction.Position.PredictUnitPosition(champToUlt, 500).To3D());
                            }
                        }
                    }

                }

            }



            if (R.IsReady() && AutoWalker.myHero.HealthPercent < 15)
            {
                AIHeroClient champToUlt =
EntityManager.Heroes.Enemies.FirstOrDefault(
    en => en.Distance(AutoWalker.myHero) < 700);
                if (champToUlt != null)
                {
                    R.Cast(champToUlt);
                }

            }

        }

        public int[] skillSequence { get; private set; }
        public LogicSelector Logic { get; set; }


        public string ShopSequence { get; private set; }

        public void Harass(AIHeroClient target)
        {
        }

        public void Survi()
        {

        }

        public void Combo(AIHeroClient target)
        {

        }

        private static float TimeForAttack(Obj_AI_Base o, float range)
        {
            float time = (range - AutoWalker.myHero.Distance(o)) / (o.MoveSpeed + 100 - AutoWalker.myHero.MoveSpeed);
            float time2 = (AutoWalker.myHero.Distance(o.GetNearestTurret()) - 950) / (o.MoveSpeed + 100 - AutoWalker.myHero.MoveSpeed);
            return time < time2 ? time : time2;
        }
        private float EstDmg(Obj_AI_Base o, float time)
        {

            float eCD = E.Handle.CooldownExpires - Game.Time < 0 ? 0 : E.Handle.CooldownExpires - Game.Time;
            float qCD = Q.Handle.CooldownExpires - Game.Time < 0 ? 0 : Q.Handle.CooldownExpires - Game.Time;
            float eTimes = (float)Math.Floor((time - eCD) / .5f);
            float damage = AutoWalker.myHero.GetSpellDamage(o, SpellSlot.E) * eTimes;
            damage += AutoWalker.myHero.GetSpellDamage(o, SpellSlot.Q) * (float)Math.Floor((time - qCD) / Q.Handle.Cooldown);
            float neededMana = E.Handle.SData.Mana * eTimes + Q.Handle.SData.Mana;
            if (AutoWalker.myHero.Mana < neededMana)
                return damage * AutoWalker.myHero.Mana / neededMana;
            return damage;
        }
    }
}