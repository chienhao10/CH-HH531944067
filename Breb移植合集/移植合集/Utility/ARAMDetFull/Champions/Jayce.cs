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
    class JayceA : Champion
    {

        public SummonerItems sumItems = new SummonerItems(player);

        public static Spellbook sBook = player.Spellbook;

        public SpellDataInst Qdata = sBook.GetSpell(SpellSlot.Q);
        public SpellDataInst Wdata = sBook.GetSpell(SpellSlot.W);
        public SpellDataInst Edata = sBook.GetSpell(SpellSlot.E);
        public SpellDataInst Rdata = sBook.GetSpell(SpellSlot.R);
        public Spell Q1 = new Spell(SpellSlot.Q, 1050);//Emp 1470
        public Spell QEmp1 = new Spell(SpellSlot.Q, 1500);//Emp 1470
        public Spell W1 = new Spell(SpellSlot.W, 0);
        public Spell E1 = new Spell(SpellSlot.E, 650);
        public Spell R1 = new Spell(SpellSlot.R, 0);

        public Spell Q2 = new Spell(SpellSlot.Q, 600);
        public Spell W2 = new Spell(SpellSlot.W, 285);
        public Spell E2 = new Spell(SpellSlot.E, 240);
        public Spell R2 = new Spell(SpellSlot.R, 0);

        public GameObjectProcessSpellCastEventArgs castEonQ = null;

        public MissileClient myCastedQ = null;

        public AIHeroClient lockedTarg = null;

        public Vector3 castQon = new Vector3(0, 0, 0);

        /* COOLDOWN STUFF */
        public float[] rangTrueQcd = { 8, 8, 8, 8, 8,8 };
        public float[] rangTrueWcd = { 14, 12, 10, 8, 6,6 };
        public float[] rangTrueEcd = { 16, 16, 16, 16, 16,16 };

        public float[] hamTrueQcd = { 16, 14, 12, 10, 8,8 };
        public float[] hamTrueWcd = { 10, 10, 10, 10, 10,10 };
        public float[] hamTrueEcd = { 14, 12, 12, 11, 10,10 };

        public float rangQCD = 0, rangWCD = 0, rangECD = 0;
        public float hamQCD = 0, hamWCD = 0, hamECD = 0;

        public float rangQCDRem = 0, rangWCDRem = 0, rangECDRem = 0;
        public float hamQCDRem = 0, hamWCDRem = 0, hamECDRem = 0;


        /* COOLDOWN STUFF END */
        public bool isHammer = false;


        public JayceA()
        {
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += OnPosibleToInterrupt;

            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                        {
                            new ConditionalItem(ItemId.Infinity_Edge),
                            new ConditionalItem(ItemId.Ionian_Boots_of_Lucidity),
                            new ConditionalItem(ItemId.The_Bloodthirster),
                            new ConditionalItem(ItemId.Last_Whisper),
                            new ConditionalItem(ItemId.Youmuus_Ghostblade),
                            new ConditionalItem(ItemId.Banshees_Veil),
                        },
                startingItems = new List<ItemId>
                        {
                            ItemId.Pickaxe,ItemId.Boots_of_Speed
                        }
            };
        }

        private void OnPosibleToInterrupt(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if(isHammer || args.DangerLevel == Interrupter2.DangerLevel.High)
             knockAway(sender);
        }


        private void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if(isHammer)
            knockAway(gapcloser.Sender);
        }

        public void OnProcessSpell(Obj_AI_Base obj, GameObjectProcessSpellCastEventArgs arg)
        {
            if (obj.IsMe)
            {
                getCDs(arg);
                checkForm();

                if (arg.SData.Name == "jayceshockblast")
                {
                      //castEonQ = arg;
                }
                else if (arg.SData.Name == "jayceaccelerationgate")
                {
                    castEonQ = null;
                    // Console.WriteLine("Cast dat E on: " + arg.SData.Name);
                }
            }
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

        public override void useSpells()
        {
            checkForm();
            if (isHammer && R1.IsReady())
                R1.Cast();

            if (!E1.IsReady() && !isHammer)
                castQon = new Vector3(0, 0, 0);
            else if (castQon.X != 0 && !isHammer)
                shootQE(castQon);
            //Must fix
           // if (castEonQ != null && castEonQ.)
           //     castEonQ = null;

            var tar = ARAMTargetSelector.getBestTarget(getBestRange());

            if (tar != null)
            {
                var dmgOn = getJayceFullComoDmg(tar);
                if ( (!Sector.inTowerRange(tar.Position.LSTo2D()) && (MapControl.balanceAroundPoint(tar.Position.LSTo2D(), 700) >= -1 || (MapControl.fightIsOn() != null && MapControl.fightIsOn().NetworkId == tar.NetworkId)))
           )
                    doFullDmg(tar);
                else
                    doCombo(tar);
            }
        }

        public override void setUpSpells()
        {
            //Create the spells
            Q1.SetSkillshot(0.3f, 70f, 1500, true, SkillshotType.SkillshotLine);
            QEmp1.SetSkillshot(0.3f, 70f, 2180, true, SkillshotType.SkillshotLine);
        }


        public void doCombo(AIHeroClient target)
        {
            castOmen(target);
            if (!isHammer)
            {
                if (castEonQ != null)
                    castEonSpell(target);

                //DO QE combo first
                if (E1.IsReady() && Q1.IsReady() && gotManaFor(true, false, true))
                {
                    castQEPred(target);
                }
                else if (Q1.IsReady() && gotManaFor(true))
                {
                    castQPred(target);
                }
                else if (W1.IsReady() && gotManaFor(false, true) && targetInRange(getClosestEnem(), 650f))
                {
                    W1.Cast();
                    sumItems.cast(SummonerItems.ItemIds.Ghostblade);
                }//and wont die wih 1 AA
                else if (!Q1.IsReady() && !W1.IsReady() && R1.IsReady() && hammerWillKill(target) && hamQCDRem == 0 && hamECDRem == 0)// will need to add check if other form skills ready
                {
                    R1.Cast();
                }
            }
            else
            {
                if ((!Q2.IsReady() && R2.IsReady() && player.LSDistance(getClosestEnem()) > 350) || player.LSDistance(getClosestEnem()) > 700)
                {
                    sumItems.cast(SummonerItems.ItemIds.Ghostblade);
                    R2.Cast();
                }
                if (Q2.IsReady() && gotManaFor(true) && targetInRange(target, Q2.Range) && player.LSDistance(target) > 300)
                {
                    sumItems.cast(SummonerItems.ItemIds.Ghostblade);
                    Q2.Cast(target);
                }
                if (E2.IsReady() && gotManaFor(false, false, true) && targetInRange(target, E2.Range) && shouldIKnockDatMadaFaka(target))
                {
                    E2.Cast(target);
                }
                if (W2.IsReady() && gotManaFor(false, true) && targetInRange(target, W2.Range))
                {
                    W2.Cast();
                }

            }
        }

        public void doFullDmg(AIHeroClient target)
        {
            castIgnite(target);


            if (!isHammer)
            {
                if (castEonQ != null)
                {
                    castEonSpell(target);
                }
                //DO QE combo first
                if (E1.IsReady() && Q1.IsReady() && gotManaFor(true, false, true))
                {
                    castQEPred(target);
                }
                else if (Q1.IsReady() && gotManaFor(true))
                {
                    castQPred(target);
                }
                else if (W1.IsReady() && gotManaFor(false, true) && targetInRange(getClosestEnem(), 1000f))
                {

                    sumItems.cast(SummonerItems.ItemIds.Ghostblade);
                    W1.Cast();
                }
                else if (!Q1.IsReady() && !W1.IsReady() && R1.IsReady() && hamQCDRem == 0 && hamECDRem == 0)// will need to add check if other form skills ready
                {
                    R1.Cast();
                }
            }
            else
            {
                if (!Q2.IsReady() && R2.IsReady() && player.LSDistance(getClosestEnem()) > 350)
                {

                    sumItems.cast(SummonerItems.ItemIds.Ghostblade);
                    R2.Cast();
                }
                if (Q2.IsReady() && gotManaFor(true) && targetInRange(target, Q2.Range))
                {
                    Q2.Cast(target);
                }
                if (E2.IsReady() && gotManaFor(false, false, true) && targetInRange(target, E2.Range) && (!gotSpeedBuff()) || (getJayceEHamDmg(target) > target.Health))
                {
                    E2.Cast(target);
                }
                if (W2.IsReady() && gotManaFor(false, true) && targetInRange(target, W2.Range))
                {
                    W2.Cast();
                }

            }
        }

        public void doKillSteal()
        {
            try
            {
                if (rangQCDRem == 0 && rangECDRem == 0 && gotManaFor(true, false, true))
                {
                    List<AIHeroClient> deadEnes = ObjectManager.Get<AIHeroClient>().Where(ene => getJayceEQDmg(ene) > ene.Health && ene.IsEnemy && ene.IsValid && ene.LSDistance(player.ServerPosition) < 1800).ToList();
                    foreach (var enem in deadEnes)
                    {
                        if (player.LSDistance(enem) < 300)
                            continue;
                        if (isHammer && R2.IsReady())
                        {
                            R2.Cast();
                        }
                        castQEPred(enem);
                    }
                }
                else if (rangQCDRem == 0 && gotManaFor(true))
                {
                    List<AIHeroClient> deadEnes = ObjectManager.Get<AIHeroClient>().Where(ene => getJayceQDmg(ene) > ene.Health && ene.IsEnemy && ene.IsValid && ene.LSDistance(player.ServerPosition) < 1200).ToList();
                    foreach (var enem in deadEnes)
                    {
                        if (isHammer && R2.IsReady())
                        {
                            R2.Cast();
                        }
                        castQPred(enem);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }

        public void castQEPred(AIHeroClient target)
        {
            if (isHammer)
                return;
            PredictionOutput po = QEmp1.GetPrediction(target);
            if (po.Hitchance >= HitChance.Low && player.LSDistance(po.UnitPosition) < (QEmp1.Range + target.BoundingRadius))
            {
                castQon = po.CastPosition;
            }
            else if (po.Hitchance == HitChance.Collision)
            {
                Obj_AI_Base fistCol = po.CollisionObjects.OrderBy(unit => unit.LSDistance(player.ServerPosition)).First();
                if (fistCol.LSDistance(po.UnitPosition) < (180 - fistCol.BoundingRadius / 2) && fistCol.LSDistance(target.ServerPosition) < (180 - fistCol.BoundingRadius / 2))
                {
                    castQon = po.CastPosition;
                }
            }
        }

        public void castQPred(AIHeroClient target)
        {
            if (isHammer)
                return;
            PredictionOutput po = Q1.GetPrediction(target);
            if (po.Hitchance >= HitChance.High && player.LSDistance(po.UnitPosition) < (Q1.Range + target.BoundingRadius))
            {
                Q1.Cast(po.CastPosition);
            }
            else if (po.Hitchance == HitChance.Collision)
            {
                Obj_AI_Base fistCol = po.CollisionObjects.OrderBy(unit => unit.LSDistance(player.ServerPosition)).First();
                if (fistCol.LSDistance(po.UnitPosition) < (180 - fistCol.BoundingRadius / 2) && fistCol.LSDistance(target.ServerPosition) < (100 - fistCol.BoundingRadius / 2))
                {
                    Q1.Cast(po.CastPosition);
                }

            }
        }

        public  Vector3 getBestPosToHammer(Vector3 target)
        {
            Obj_AI_Base tower = ObjectManager.Get<Obj_AI_Turret>().Where(tur => tur.IsAlly && tur.Health > 0).OrderBy(tur => player.LSDistance(tur)).First();
            return target + Vector3.Normalize(tower.ServerPosition - target) * (-120);
        }

        public  Vector3 posAfterHammer(Obj_AI_Base target)
        {
            return getBestPosToHammer(target.ServerPosition) + Vector3.Normalize(getBestPosToHammer(target.ServerPosition) - player.ServerPosition) * 600;
        }

        public  AIHeroClient getClosestEnem()
        {
            return ObjectManager.Get<AIHeroClient>().Where(ene => ene.IsEnemy && ene.LSIsValidTarget()).OrderBy(ene => player.LSDistance(ene)).First();
        }

        public float getBestRange()
        {
            float range;
            if (!isHammer)
            {
                if (Q1.IsReady() && E1.IsReady() && gotManaFor(true, false, true))
                {
                    range = 1750;
                }
                else if (Q1.IsReady() && gotManaFor(true))
                {
                    range = 1150;
                }
                else
                {
                    range = 500;
                }
            }
            else
            {
                if (Q1.IsReady() && gotManaFor(true))
                {
                    range = 600;
                }
                else
                {
                    range = 300;
                }
            }
            return range + 50;
        }


        public bool shootQE(Vector3 pos)
        {
            try
            {
                if (isHammer && R2.IsReady())
                    R2.Cast();
                if (!E1.IsReady() || !Q1.IsReady() || isHammer)
                    return false;

                Vector3 bPos = player.ServerPosition - Vector3.Normalize(pos - player.ServerPosition) * 50;

                Player.IssueOrder(GameObjectOrder.MoveTo, bPos);
                Q1.Cast(pos);

                E1.Cast(getParalelVec(pos));

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return true;
        }

        public bool shouldIKnockDatMadaFaka(AIHeroClient target)
        {
            //if (useSmartKnock(target) && R2.IsReady() && target.CombatType == GameObjectCombatType.Melee)
            // {
            //  return true;
            // }
            float damageOn = getJayceEHamDmg(target);

            if (damageOn > target.Health * 0.9f)
            {
                return true;
            }
            if (((player.Health / player.MaxHealth) < 0.15f) /*&& target.CombatType == GameObjectCombatType.Melee*/)
            {
                return true;
            }
            Vector3 posAfter = target.ServerPosition + Vector3.Normalize(target.ServerPosition - player.ServerPosition) * 450;
            if (inMyTowerRange(posAfter))
            {
                return true;
            }

            return false;
        }

        public bool useSmartKnock(AIHeroClient target)
        {
            float trueAARange = player.BoundingRadius + target.AttackRange;
            float trueERange = target.BoundingRadius + E2.Range;

            float dist = player.LSDistance(target);
            Vector2 movePos = new Vector2();
            if (target.IsMoving)
            {
                Vector2 tpos = target.Position.LSTo2D();
                Vector2 path = target.Path[0].LSTo2D() - tpos;
                path.Normalize();
                movePos = tpos + (path * 100);
            }
            float targ_ms = (target.IsMoving && player.LSDistance(movePos) < dist) ? target.MoveSpeed : 0;
            float msDif = (player.MoveSpeed * 0.7f - targ_ms) == 0 ? 0.0001f : (targ_ms - player.MoveSpeed * 0.7f);
            float timeToReach = (dist - trueAARange) / msDif;
            if (dist > trueAARange && dist < trueERange && target.IsMoving)
            {
                if (timeToReach > 1.7f || timeToReach < 0.0f)
                {
                    return true;
                }
            }
            return false;
        }

        public bool inMyTowerRange(Vector3 pos)
        {
            return ObjectManager.Get<Obj_AI_Turret>().Where(tur => tur.IsAlly && tur.Health > 0).Any(tur => pos.LSDistance(tur.Position) < (850 + player.BoundingRadius));
        }

        public void castEonSpell(AIHeroClient mis)
        {
            if (isHammer || !E1.IsReady())
                return;
            if (player.LSDistance(myCastedQ.Position) < 250)
            {
                E1.Cast(getParalelVec(mis.Position));
            }

        }


        public bool targetInRange(Obj_AI_Base target, float range)
        {
            float dist2 = Vector2.DistanceSquared(target.ServerPosition.LSTo2D(), player.ServerPosition.LSTo2D());
            float range2 = range * range + target.BoundingRadius * target.BoundingRadius;
            return dist2 < range2;
        }

        public void checkForm()
        {
            isHammer = !Qdata.SData.Name.Contains("jayceshockblast");
        }


        public bool gotSpeedBuff()//jaycehypercharge
        {
            return player.Buffs.Any(bi => bi.Name.Contains("jaycehypercharge"));
        }

        public Vector2 getParalelVec(Vector3 pos)
        {
            Random rnd = new Random();
            int par = rnd.Next(0, 1);
            if (par==1)
            {
                int neg = rnd.Next(0, 1);
                int away = 42;
                away = (neg == 1) ? away : -away;
                var v2 = Vector3.Normalize(pos - player.ServerPosition) * away;
                var bom = new Vector2(v2.Y, -v2.X);
                return player.ServerPosition.LSTo2D() + bom;
            }
            else
            {
                var v2 = Vector3.Normalize(pos - player.ServerPosition) * 300;
                var bom = new Vector2(v2.X, v2.Y);
                return player.ServerPosition.LSTo2D() + bom;
            }
        }

        //Need to fix!!
        public bool gotManaFor(bool q = false, bool w = false, bool e = false)
        {
            float manaNeeded = 0;
            if (q)
                manaNeeded += Qdata.SData.Mana;
            if (w)
                manaNeeded += Wdata.SData.Mana;
            if (e)
                manaNeeded += Edata.SData.Mana;
            // Console.WriteLine("Mana: " + manaNeeded);
            return manaNeeded <= player.Mana;
        }

        public float calcRealCD(float time)
        {
            return time + (time * player.PercentCooldownMod);
        }

        public void processCDs()
        {
            hamQCDRem = ((hamQCD - Game.Time) > 0) ? (hamQCD - Game.Time) : 0;
            hamWCDRem = ((hamWCD - Game.Time) > 0) ? (hamWCD - Game.Time) : 0;
            hamECDRem = ((hamECD - Game.Time) > 0) ? (hamECD - Game.Time) : 0;

            rangQCDRem = ((rangQCD - Game.Time) > 0) ? (rangQCD - Game.Time) : 0;
            rangWCDRem = ((rangWCD - Game.Time) > 0) ? (rangWCD - Game.Time) : 0;
            rangECDRem = ((rangECD - Game.Time) > 0) ? (rangECD - Game.Time) : 0;
        }

        public void getCDs(GameObjectProcessSpellCastEventArgs spell)
        {
            try
            {
                //Console.WriteLine(spell.SData.Name + ": " + Q2.Level);

                if (spell.SData.Name == "JayceToTheSkies")
                    hamQCD = Game.Time + calcRealCD(hamTrueQcd[Q2.Level - 1]);
                if (spell.SData.Name == "JayceStaticField")
                    hamWCD = Game.Time + calcRealCD(hamTrueWcd[W2.Level - 1]);
                if (spell.SData.Name == "JayceThunderingBlow")
                    hamECD = Game.Time + calcRealCD(hamTrueEcd[E2.Level - 1]);

                if (spell.SData.Name == "jayceshockblast")
                    rangQCD = Game.Time + calcRealCD(rangTrueQcd[Q1.Level - 1]);
                if (spell.SData.Name == "jaycehypercharge")
                    rangWCD = Game.Time + calcRealCD(rangTrueWcd[W1.Level - 1]);
                if (spell.SData.Name == "jayceaccelerationgate")
                    rangECD = Game.Time + calcRealCD(rangTrueEcd[E1.Level - 1]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void knockAway(Obj_AI_Base target)
        {
            if (!targetInRange(target, 270) || hamECDRem != 0 || E1.Level == 0)
                return;

            if (!isHammer && R2.IsReady())
                R1.Cast();
            if (isHammer && E2.IsReady() && targetInRange(target, 260))
                E2.Cast(target);

        }

        public bool hammerWillKill(Obj_AI_Base target)
        {
            if (!safeToJumpOn(target))
                return false;
            float damage = (float)player.LSGetAutoAttackDamage(target) + 50;
            damage += getJayceEHamDmg(target);
            damage += getJayceQHamDmg(target);

            return (target.Health < damage);
        }


        public float getJayceFullComoDmg(Obj_AI_Base target)
        {
            float dmg = 0;
            //Ranged
            if (!isHammer || R1.IsReady())
            {
                if (rangECDRem == 0 && rangQCDRem == 0 && Q1.Level != 0 && E1.Level != 0)
                {
                    dmg += getJayceEQDmg(target);
                }
                else if (rangQCDRem == 0 && Q1.Level != 0)
                {
                    dmg += getJayceQDmg(target);
                }
                float hyperMulti = W1.Level * 0.15f + 0.7f;
                if (rangWCDRem == 0 && W1.Level != 0)
                {
                    dmg += getJayceAADmg(target) * 3 * hyperMulti;
                }
            }
            //Hamer
            if (isHammer || R1.IsReady())
            {
                if (hamECDRem == 0 && E2.Level != 0)
                {
                    dmg += getJayceEHamDmg(target);
                }
                if (hamQCDRem == 0 && Q2.Level != 0)
                {
                    dmg += getJayceQHamDmg(target);
                }
            }
            return dmg;
        }

        public float getJayceAADmg(Obj_AI_Base target)
        {
            return (float)player.LSGetAutoAttackDamage(target);

        }

        public float getJayceEQDmg(Obj_AI_Base target)
        {
            return
                (float)
                    player.CalcDamage(target, DamageType.Physical,
                        (7 + (player.Spellbook.GetSpell(SpellSlot.Q).Level * 77)) +
                        (1.68 * player.FlatPhysicalDamageMod));


        }

        public float getJayceQDmg(Obj_AI_Base target)
        {
            return (float)player.CalcDamage(target, DamageType.Physical,
                                    (5 + (player.Spellbook.GetSpell(SpellSlot.Q).Level * 55)) +
                                    (1.2 * player.FlatPhysicalDamageMod));
        }

        public float getJayceEHamDmg(Obj_AI_Base target)
        {
            double percentage = 5 + (3 * player.Spellbook.GetSpell(SpellSlot.E).Level);
            return (float)player.CalcDamage(target, DamageType.Magical,
                    ((target.MaxHealth / 100) * percentage) + (player.FlatPhysicalDamageMod));
        }

        public float getJayceQHamDmg(Obj_AI_Base target)
        {
            return (float)player.CalcDamage(target, DamageType.Physical,
                                (-25 + (player.Spellbook.GetSpell(SpellSlot.Q).Level * 45)) +
                                (1.0 * player.FlatPhysicalDamageMod));
        }

        public void castIgnite(AIHeroClient target)
        {
            if (targetInRange(target, 600) && (target.Health / target.MaxHealth) * 100 < 25)
                sumItems.castIgnite(target);
        }

        public void castOmen(AIHeroClient target)
        {
            if (player.LSDistance(target) < 430)
                sumItems.cast(SummonerItems.ItemIds.Omen);
        }

        public void activateMura()
        {
            if (player.Buffs.Count(buf => buf.Name == "Muramana") == 0)
                sumItems.cast(SummonerItems.ItemIds.Muramana);
        }

        public void deActivateMura()
        {
            if (player.Buffs.Count(buf => buf.Name == "Muramana") != 0)
                sumItems.cast(SummonerItems.ItemIds.Muramana);
        }

        public bool safeToJumpOn(Obj_AI_Base target)
        {
            return (!Sector.inTowerRange(target.Position.LSTo2D()) &&
                    ((MapControl.fightIsOn() != null && MapControl.fightIsOn().NetworkId == target.NetworkId)));
        }

        public void drawCD()
        {
            var pScreen = Drawing.WorldToScreen(player.Position);

            // Drawing.DrawText(Drawing.WorldToScreen(Player.Position)[0], Drawing.WorldToScreen(Player.Position)[1], System.Drawing.Color.Green, "Q: wdeawd ");
            pScreen[0] -= 20;

            if (isHammer)
            {
                if (rangQCDRem == 0)
                    Drawing.DrawText(pScreen.X - 60, pScreen.Y, System.Drawing.Color.Green, "Q: Rdy");
                else
                    Drawing.DrawText(pScreen.X - 60, pScreen.Y, System.Drawing.Color.Red, "Q: " + rangQCDRem.ToString("0.0"));

                if (rangWCDRem == 0)
                    Drawing.DrawText(pScreen.X, pScreen.Y, System.Drawing.Color.Green, "W: Rdy");
                else
                    Drawing.DrawText(pScreen.X, pScreen.Y, System.Drawing.Color.Red, "W: " + rangWCDRem.ToString("0.0"));

                if (rangECDRem == 0)
                    Drawing.DrawText(pScreen.X + 60, pScreen.Y, System.Drawing.Color.Green, "E: Rdy");
                else
                    Drawing.DrawText(pScreen.X + 60, pScreen.Y, System.Drawing.Color.Red, "E: " + rangECDRem.ToString("0.0"));
            }
            else
            {
                // pScreen.Y += 30;
                if (hamQCDRem == 0)
                    Drawing.DrawText(pScreen.X - 60, pScreen.Y, System.Drawing.Color.Green, "Q: Rdy");
                else
                    Drawing.DrawText(pScreen.X - 60, pScreen.Y, System.Drawing.Color.Red, "Q: " + hamQCDRem.ToString("0.0"));

                if (hamWCDRem == 0)
                    Drawing.DrawText(pScreen.X, pScreen.Y, System.Drawing.Color.Green, "W: Rdy");
                else
                    Drawing.DrawText(pScreen.X, pScreen.Y, System.Drawing.Color.Red, "W: " + hamWCDRem.ToString("0.0"));

                if (hamECDRem == 0)
                    Drawing.DrawText(pScreen.X + 60, pScreen.Y, System.Drawing.Color.Green, "E: Rdy");
                else
                    Drawing.DrawText(pScreen.X + 60, pScreen.Y, System.Drawing.Color.Red, "E: " + hamECDRem.ToString("0.0"));
            }
        }

    }
}
