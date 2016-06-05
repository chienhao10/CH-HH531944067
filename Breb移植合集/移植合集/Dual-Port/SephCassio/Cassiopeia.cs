#region imports
using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;
using Spell = LeagueSharp.Common.Spell;

#endregion;


namespace SephCassiopeia
{

    #region Initiliazation

    internal static class Cassiopeia
    {
        #region vars

        public static AIHeroClient Player;
        private static AIHeroClient target;
        public static Menu Config;
        private static SpellSlot IgniteSlot = SpellSlot.Summoner1;
        private static float edelay = 0;
        private static float laste = 0;
        private static int[] skillorder = { 1, 3, 2, 3, 3, 4, 3, 1, 3, 1, 4, 1, 1, 2, 2, 4, 2, 2 };

        #endregion

        #region OnLoad

        private static Dictionary<SpellSlot, Spell> Spells;

        private static void InitializeSpells()
        {
            Spells = new Dictionary<SpellSlot, Spell> {
            { SpellSlot.Q, new Spell(SpellSlot.Q, 850f, DamageType.Magical) },
            { SpellSlot.W, new Spell(SpellSlot.W, 850f, DamageType.Magical) },
            { SpellSlot.E, new Spell(SpellSlot.E, 700f, DamageType.Magical) },
            { SpellSlot.R, new Spell(SpellSlot.R, 825f, DamageType.Magical) },
            { IgniteSlot, new Spell(ObjectManager.Player.GetSpellSlot("summonerdot"), 550f) }
                };
            Spells[SpellSlot.Q].SetSkillshot(0.6f, 70f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            Spells[SpellSlot.W].SetSkillshot(0.5f, 275f, 2500f, false, SkillshotType.SkillshotCircle);
            Spells[SpellSlot.R].SetSkillshot(0.3f, (float)(80 * Math.PI / 180), float.MaxValue, false, SkillshotType.SkillshotCone); //Credits to InjectionDev for Width Value
        }

        public static void CassMain()
        {
            Player = ObjectManager.Player;

            if (Player.CharData.BaseSkinName != "Cassiopeia")
            {
                return;
            }


            Config = CassiopeiaMenu.CreateMenu();

            InitializeSpells();

            AntiGapcloser.OnEnemyGapcloser += OnGapClose;

            Interrupter2.OnInterruptableTarget += OnInterruptableTarget;

            Game.OnUpdate += OnUpdate;
            Game.OnUpdate += CheckKillable;
            Game.OnUpdate += AutoSpells;
            Drawing.OnDraw += OnDraw;
            Orbwalker.OnPreAttack += BeforeAuto;
        }

        #endregion

    #endregion

        #region BeforeAuto

        private static void BeforeAuto(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (!CassioUtils.getCheckBoxItem(CassiopeiaMenu.Combo, "Combo.Useauto") && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                args.Process = false;
                return;

            }
            if (CassioUtils.getCheckBoxItem(CassiopeiaMenu.Combo, "Combo.Disableautoifspellsready") && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (SpellSlot.Q.IsReady() || SpellSlot.W.IsReady() || SpellSlot.E.IsReady() || SpellSlot.R.IsReady())
                {
                    args.Process = false;
                    return;
                }
            }
            if (!CassioUtils.getCheckBoxItem(CassiopeiaMenu.Waveclear, "Waveclear.Useauto") && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                args.Process = false;
                return;
            }
            if (!CassioUtils.getCheckBoxItem(CassiopeiaMenu.Farm, "Farm.Useauto") && (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit)))
            {
                args.Process = false;
                return;
            }


        }

        #endregion

        #region AutoSpells

        static void AutoSpells(EventArgs args)
        {
            if (Player.IsDead || Player.recalling())
            {
                return;
            }
            if (SpellSlot.E.IsReady() && CassioUtils.getCheckBoxItem(CassiopeiaMenu.Combo, "Combo.UseE") && (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) || CassioUtils.getCheckBoxItem(CassiopeiaMenu.misc, "Misc.autoe")))
            {
                AIHeroClient etarg;
                etarg = target;
                if (etarg == null)
                {
                    etarg = HeroManager.Enemies.FirstOrDefault(h => h.LSIsValidTarget(Spells[SpellSlot.E].Range) && h.isPoisoned() && !h.IsInvulnerable && !h.IsZombie);
                }
                if (etarg != null && CassioUtils.getCheckBoxItem(CassiopeiaMenu.Combo, "Combo.useepoison") && etarg.isPoisoned())
                {
                    if ((Utils.GameTimeTickCount - laste) > edelay)
                    {
                        Spells[SpellSlot.E].Cast(etarg);
                        laste = Utils.GameTimeTickCount;
                    }
                }
                else if (!CassioUtils.getCheckBoxItem(CassiopeiaMenu.Combo, "Combo.useepoison"))
                {
                    if ((Utils.GameTimeTickCount - laste) > edelay)
                    {
                        Spells[SpellSlot.E].Cast(target);
                        laste = Utils.GameTimeTickCount;
                    }
                }
            }

            

            if (SpellSlot.R.IsReady() && CassioUtils.getCheckBoxItem(CassiopeiaMenu.Combo, "Combo.UseR") &&
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                var targets = HeroManager.Enemies.Where(x => x.LSIsValidTarget(Spells[SpellSlot.R].Range) && !x.IsZombie).OrderBy(x => x.Health);
                Vector3 bestpositionfacing = new Vector3(0, 0, 0);
                Vector3 bestpositionnf = new Vector3(0, 0, 0);
                int mosthitfacing = 0;
                int mosthitnf = 0;
                foreach (var targ in targets)
                {
                    var pred = Spells[SpellSlot.R].GetPrediction(targ, true);
                    if (pred.Hitchance >= CassioUtils.RChance())
                    {
                        int enemhitpred = 0;
                        int enemfacingpred = 0;
                        foreach (var hero in targets)
                        {
                            if (Spells[SpellSlot.R].WillHit(hero, pred.CastPosition, 0, CassioUtils.RChance()))
                            {
                                enemhitpred++;
                                if (hero.IsFacing(Player))
                                {
                                    enemfacingpred++;
                                }
                            }
                        }

                        if (enemfacingpred > mosthitfacing)
                        {
                            mosthitfacing = enemfacingpred;
                            bestpositionfacing = pred.CastPosition;
                        }

                        if (enemhitpred > mosthitnf)
                        {
                            mosthitnf = enemhitpred;
                            bestpositionnf = pred.CastPosition;
                        }
                    }
                }

                if (mosthitfacing >= CassioUtils.getSliderItem(CassiopeiaMenu.Combo, "Combo.Rcount"))
                {
                    Spells[SpellSlot.R].Cast(bestpositionfacing);
                    return;
                }
                if (mosthitnf >= CassioUtils.getSliderItem(CassiopeiaMenu.Combo, "Combo.Rcountnf") && CassioUtils.getCheckBoxItem(CassiopeiaMenu.Combo, "Combo.UseRNF"))
                {
                    Spells[SpellSlot.R].Cast(bestpositionnf);
                    return;
                }

                var easycheck = HeroManager.Enemies.FirstOrDefault(x =>
                     !x.IsInvulnerable && !x.IsZombie && x.LSIsValidTarget(Spells[SpellSlot.R].Range) &&
                     x.LSIsFacing(Player) && x.isImmobile());

                if (easycheck != null)
                {
                    Spells[SpellSlot.R].Cast(easycheck.ServerPosition);
                    return;
                }
            }
            

            /* © ® ™ Work on patented algorithms in the future! XD © ® ™ */
            /*
            if (SpellSlot.R.IsReady() && CassioUtils.Active("Combo.UseR") && CassiopeiaMenu.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                var easycheck =
                    HeroManager.Enemies.FirstOrDefault(
                        x =>
                            !x.IsInvulnerable && !x.IsZombie && x.IsValidTarget(Spells[SpellSlot.R].Range) &&
                            x.IsFacing(Player) && x.isImmobile() && (Player.HealthPercent <= 20 || x.HealthPercent > 30));

                if (easycheck != null)
                {
                    Spells[SpellSlot.R].Cast(easycheck.ServerPosition);
                    DontMove = true;
                    Utility.DelayAction.Add(50, () => DontMove = false);
                    return;
                }
                var targs = HeroManager.Enemies.Where(h => h.IsValidTarget(Spells[SpellSlot.R].Range));
                Dictionary<Vector3, double> Hitatpos = new Dictionary<Vector3, double>();
                Dictionary<Vector3, double> Hitatposfacing = new Dictionary<Vector3, double>();
                foreach (var t in targs)
                {
                    var pred = Spells[SpellSlot.R].GetPrediction(t, false);
                    var enemshit = pred.CastPosition.GetEnemiesInRange(Spells[SpellSlot.R].Width).Where(x=> x.Distance(Player) <= Spells[SpellSlot.R].Range);
                    var counthit = enemshit.Count();
                    var hitfacing = enemshit.Count(x => x.IsFacing(Player) && !x.IsDashing() && !x.IsZombie && !x.IsInvulnerable);
                    var anymovingtome = enemshit.Any(x => x.isMovingToMe() || x.IsFacing(Player));

                    if (pred.Hitchance >= CassioUtils.GetHitChance("Hitchance.R") && anymovingtome)
                    {
                         Hitatposfacing.Add(pred.CastPosition, hitfacing);
                    }
                    if (CassioUtils.Active("Combo.UseRNF") && pred.Hitchance >= CassioUtils.GetHitChance("Hitchance.R"))
                    {
                        Hitatpos.Add(pred.CastPosition, counthit);
                    }
                }
                if (Hitatposfacing.Any())
                {
                    var bestpos = Hitatposfacing.Find(pos => pos.Value.Equals(Hitatposfacing.Values.Max())).Key;
                    if (bestpos.IsValid() && bestpos.CountEnemiesInRange(Spells[SpellSlot.R].Width) >= CassioUtils.GetSlider("Combo.Rcount"))
                    {
                        Spells[SpellSlot.R].Cast(bestpos);
                        DontMove = true;
                        Utility.DelayAction.Add(50, () => DontMove = false);
                    }
                }
                else if (Hitatpos.Any() && CassioUtils.Active("Combo.UseRNF") &&
                         CassioUtils.GetSlider("Combo.Rcountnf") >= Hitatpos.Values.Max())
                {
                    var bestposnf = Hitatpos.Find(pos => pos.Value.Equals(Hitatpos.Values.Max())).Key;
                    if (bestposnf.IsValid() && bestposnf.CountEnemiesInRange(Spells[SpellSlot.R].Width) >= CassioUtils.GetSlider("Combo.Rcountnf"))
                    {
                        Spells[SpellSlot.R].Cast(bestposnf);
                        DontMove = true;
                        Utility.DelayAction.Add(50, () => DontMove = false);
                    }
                }
            
            }   
            */
             

        }

        #endregion

        #region Immobility Check

        private static bool isImmobile(this AIHeroClient hero)
        {
            foreach (var buff in hero.Buffs)
            {
                if (buff.Type == BuffType.Stun || buff.Type == BuffType.Taunt || buff.Type == BuffType.Charm ||
                    buff.Type == BuffType.Fear || buff.Type == BuffType.Knockup || buff.Type == BuffType.Polymorph ||
                    buff.Type == BuffType.Snare || buff.Type == BuffType.Suppression || buff.Type == BuffType.Flee ||
                    buff.Type == BuffType.Slow && target.MoveSpeed <= 0.90 * target.MoveSpeed)
                {
                    var tenacity = hero.PercentCCReduction;
                    var buffEndTime = buff.EndTime - (tenacity * (buff.EndTime - buff.StartTime));
                    var cctimeleft = buffEndTime - Game.Time;
                    if (cctimeleft > Game.Ping / 1000f + Spells[SpellSlot.R].Delay)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion

        #region OnUpdate

        private static void OnUpdate(EventArgs args) {

            if (Player.IsDead || Player.recalling())
            {
                return;
            }


            edelay = CassioUtils.getSliderItem(CassiopeiaMenu.Combo, "Combo.edelay");

            Killsteal();

            target = TargetSelector.GetTarget(Spells[SpellSlot.Q].Range, DamageType.Magical);
            if (target != null && CassioUtils.getKeyBindItem(CassiopeiaMenu.Harass, "Keys.HarassT") && Player.ManaPercent >= CassioUtils.getSliderItem(CassiopeiaMenu.Harass, "Harass.Mana") && !target.IsInvulnerable && !target.IsZombie)
            {
                Harass(target);
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (target != null && !target.IsInvulnerable && !target.IsZombie)
                {
                    Combo(target);
                }
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                MixedModeLogic(target, true);
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                WaveClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                MixedModeLogic(target, false);
            }

        }

        #endregion

        #region Recallcheck
        public static bool recalling(this AIHeroClient unit)
        {
            return unit.Buffs.Any(buff => buff.Name.ToLower().Contains("recall") && buff.Name != "MasteryImprovedRecallBuff");
        }
        #endregion Recallcheck

        #region Combo

        private static void Combo(AIHeroClient target)
        {
            if (Spells[SpellSlot.Q].IsReady() && CassioUtils.getCheckBoxItem(CassiopeiaMenu.Combo, "Combo.UseQ"))
            {
	            //Spells[SpellSlot.Q].SPredictionCast(target, CassioUtils.GetHitChance("Hitchance.Q"));
	            
                var pred = Spells[SpellSlot.Q].GetPrediction(target, true);
                if (pred.Hitchance >= CassioUtils.QChance())
                {
                    Spells[SpellSlot.Q].Cast(pred.CastPosition);
                }
				
            }
            if (Spells[SpellSlot.W].IsReady() && CassioUtils.getCheckBoxItem(CassiopeiaMenu.Combo, "Combo.UseW"))
            {
				//Spells[SpellSlot.W].SPredictionCast(target, CassioUtils.GetHitChance("Hitchance.W"));
				
                var pred = Spells[SpellSlot.W].GetPrediction(target, true);
                if (pred.Hitchance > CassioUtils.WChance())
                {
                    Spells[SpellSlot.W].Cast(pred.CastPosition);
                }
				
			}
            if (Spells[SpellSlot.E].IsReady() && CassioUtils.getCheckBoxItem(CassiopeiaMenu.Combo, "Combo.UseE"))
            {
                if (CassioUtils.getCheckBoxItem(CassiopeiaMenu.Combo, "Combo.useepoison") && target.isPoisoned())
                {
                    if ((Utils.GameTimeTickCount - laste) > edelay)
                    {
                        Spells[SpellSlot.E].Cast(target);
                        laste = Utils.GameTimeTickCount;
                    }
                }
                else if (!CassioUtils.getCheckBoxItem(CassiopeiaMenu.Combo, "Combo.useepoison"))
                {
                    if ((Utils.GameTimeTickCount - laste) > edelay)
                    {
                        Spells[SpellSlot.E].Cast(target);
                        laste = Utils.GameTimeTickCount;
                    }
                }

            }
            /*
            if (SpellSlot.R.IsReady() && CassioUtils.Active("Combo.UseR"))
            {
                    var pred = Spells[SpellSlot.R].GetPrediction(target, true);
                    var enemshit = pred.CastPosition.GetEnemiesInRange(Spells[SpellSlot.R].Width);
                    var counthit = enemshit.Count;
                    var hitfacing = enemshit.Count(x => x.IsFacing(Player));
                    var anymovingtome = enemshit.Any(x => x.isMovingToMe());
                    if (hitfacing >= CassioUtils.GetSlider("Combo.Rcount") && pred.Hitchance >= CassioUtils.GetHitChance("Hitchance.R") && anymovingtome || CassioUtils.Active("Combo.UseRNF") && counthit >= CassioUtils.GetSlider("Combo.Rcountnf") && pred.Hitchance >= CassioUtils.GetHitChance("Hitchance.R"))
                    {
                         Spells[SpellSlot.R].Cast(pred.CastPosition);
                    }
                }
             * */
        }

        #endregion

        #region ExaminetargetWP

        static bool isMovingToMe(this Obj_AI_Base target)
        {
            var x = target.GetWaypoints().Last();
            var mypos2d = Player.ServerPosition.LSTo2D();
            if (Vector2.Distance(mypos2d, x) <= Vector2.Distance(mypos2d, target.ServerPosition.LSTo2D()) && target.GetWaypoints().Count >= 2 || !target.IsMoving)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region Waveclear

        private static void WaveClear()
        {
            var Minions =
                ObjectManager.Get<Obj_AI_Minion>()
                    .Where(
                        m =>
                            (m.LSIsValidTarget()) &&
                            (Vector3.Distance(m.ServerPosition, Player.ServerPosition) <= Spells[SpellSlot.R].Range));

            if (SpellSlot.Q.IsReady() && CassioUtils.getCheckBoxItem(CassiopeiaMenu.Waveclear, "Waveclear.UseQ"))
            {
                var qminions =
                    Minions.Where(
                        m =>
                            Vector3.Distance(m.ServerPosition, Player.ServerPosition) <= Spells[SpellSlot.Q].Range);
                MinionManager.FarmLocation QLocation =
                    MinionManager.GetBestCircularFarmLocation(
                        qminions.Select(m => m.ServerPosition.LSTo2D()).ToList(), Spells[SpellSlot.Q].Width,
                        Spells[SpellSlot.Q].Range);
                if (QLocation.MinionsHit >= 1)
                {
                    Spells[SpellSlot.Q].Cast(QLocation.Position);
                }
            }


            if (SpellSlot.W.IsReady() && CassioUtils.getCheckBoxItem(CassiopeiaMenu.Waveclear, "Waveclear.UseW"))
            {
                var wminions = Minions.Where(m =>
                            Vector3.Distance(m.ServerPosition, Player.ServerPosition) <= Spells[SpellSlot.W].Range);
                MinionManager.FarmLocation WLocation =
                    MinionManager.GetBestCircularFarmLocation(
                        wminions.Select(m => m.ServerPosition.LSTo2D()).ToList(), Spells[SpellSlot.W].Width,
                        Spells[SpellSlot.W].Range);
                if (WLocation.MinionsHit >= 1)
                {
                    Spells[SpellSlot.W].Cast(WLocation.Position);
                }
            }

            if (SpellSlot.E.IsReady() && CassioUtils.getCheckBoxItem(CassiopeiaMenu.Waveclear, "Waveclear.UseE"))
            {
                Obj_AI_Minion KillableMinionE = null;
                var eminions = Minions.Where(m =>
                          Vector3.Distance(m.ServerPosition, Player.ServerPosition) <= Spells[SpellSlot.E].Range);
                if (CassioUtils.getCheckBoxItem(CassiopeiaMenu.Waveclear, "Waveclear.useekillable"))
                {
                    KillableMinionE = eminions.FirstOrDefault(m => m.Health < Player.LSGetSpellDamage(m, SpellSlot.E));
                }
                else
                {
                    KillableMinionE = eminions.OrderBy(x => x.Health).FirstOrDefault();
                }

                if (KillableMinionE != null)
                {
                    if (CassioUtils.getCheckBoxItem(CassiopeiaMenu.Waveclear, "Waveclear.useepoison"))
                    {
                        if (KillableMinionE.isPoisoned())
                            Spells[SpellSlot.E].Cast(KillableMinionE);
                    }
                    else
                    {
                        Spells[SpellSlot.E].Cast(KillableMinionE);
                    }
                }
            }

            if (SpellSlot.R.IsReady() && CassioUtils.getCheckBoxItem(CassiopeiaMenu.Waveclear, "Waveclear.UseR"))
            {
                MinionManager.FarmLocation RLocation =
                    MinionManager.GetBestLineFarmLocation(
                        Minions.Select(m => m.ServerPosition.LSTo2D()).ToList(), Spells[SpellSlot.R].Width,
                        Spells[SpellSlot.R].Range);
                if (RLocation.MinionsHit > CassioUtils.getSliderItem(CassiopeiaMenu.Waveclear, "Waveclear.Rcount"))
                {
                    Spells[SpellSlot.R].Cast(RLocation.Position);
                }
            }
        }
        #endregion Waveclear


        #region MixedModeLogic

        static void MixedModeLogic(AIHeroClient target, bool isMixed)
        {
            if (isMixed && CassioUtils.getCheckBoxItem(CassiopeiaMenu.Harass, "Harass.InMixed") && Player.ManaPercent > CassioUtils.getSliderItem(CassiopeiaMenu.Harass, "Harass.Mana"))
            {
                if (target != null && !target.IsInvulnerable && !target.IsZombie)
                {
                    Harass(target);
                }
            }

            if (!CassioUtils.getCheckBoxItem(CassiopeiaMenu.Farm, "Farm.Enable") || Player.ManaPercent < CassioUtils.getSliderItem(CassiopeiaMenu.Farm, "Farm.Mana"))
            {
                return;
            }

            var Minions =
                ObjectManager.Get<Obj_AI_Minion>()
                    .Where(
                        m =>
                            m.LSIsValidTarget() &&
                            (Vector3.Distance(m.ServerPosition, Player.ServerPosition) <= Spells[SpellSlot.Q].Range));

            if (!Minions.Any())
            {
                return;
            }
            if (SpellSlot.Q.IsReady() && CassioUtils.getCheckBoxItem(CassiopeiaMenu.Farm, "Farm.UseQ"))
            {
                var KillableMinionsQ = Minions.Where(m => m.Health < Player.LSGetSpellDamage(m, SpellSlot.Q));
                if (KillableMinionsQ.Any())
                {
                    Spells[SpellSlot.Q].Cast(KillableMinionsQ.FirstOrDefault().ServerPosition);
                }
            }
            if (SpellSlot.W.IsReady() && CassioUtils.getCheckBoxItem(CassiopeiaMenu.Farm, "Farm.UseW"))
            {
                var KillableMinionsW = Minions.Where(m => m.Health < Player.LSGetSpellDamage(m, SpellSlot.W));
                if (KillableMinionsW.Any())
                {
                    Spells[SpellSlot.W].Cast(KillableMinionsW.FirstOrDefault().ServerPosition);
                }
            }
            if (SpellSlot.E.IsReady() && CassioUtils.getCheckBoxItem(CassiopeiaMenu.Farm, "Farm.UseE"))
            {

                var KillableMinionE = Minions.FirstOrDefault(m => m.Health < Player.LSGetSpellDamage(m, SpellSlot.E));
                if (KillableMinionE != null)
                {
                    if (CassioUtils.getCheckBoxItem(CassiopeiaMenu.Farm, "Farm.useepoison"))
                    {
                        if (KillableMinionE.isPoisoned())
                            Spells[SpellSlot.E].Cast(KillableMinionE);
                    }
                    else
                    {
                        Spells[SpellSlot.E].Cast(KillableMinionE);
                    }
                }
            }
        }
        #endregion MixedModeLogic

        #region Harass

        static void Harass(AIHeroClient target)
        {
            if (Spells[SpellSlot.Q].IsReady() && CassioUtils.getCheckBoxItem(CassiopeiaMenu.Harass, "Harass.UseQ"))
            {
				//Spells[SpellSlot.Q].SPredictionCast(target, CassioUtils.GetHitChance("Hitchance.Q"));
				
                var pred = Spells[SpellSlot.Q].GetPrediction(target, true);
                if (pred.Hitchance >= CassioUtils.QChance())
                {
                    Spells[SpellSlot.Q].Cast(pred.CastPosition);
                }
				
			}
            if (Spells[SpellSlot.W].IsReady() && CassioUtils.getCheckBoxItem(CassiopeiaMenu.Harass, "Harass.UseW"))
            {
				//Spells[SpellSlot.W].SPredictionCast(target, CassioUtils.GetHitChance("Hitchance.W"));
				
                var pred = Spells[SpellSlot.W].GetPrediction(target, true);
                if (pred.Hitchance >= CassioUtils.WChance())
                {
                    Spells[SpellSlot.W].Cast(pred.CastPosition);
                }
				

			}
            if (Spells[SpellSlot.E].IsReady() && CassioUtils.getCheckBoxItem(CassiopeiaMenu.Harass, "Harass.UseE"))
            {
                if (target.isPoisoned())
                {
                    if ((Utils.GameTimeTickCount - laste) > edelay)
                    {
                        Spells[SpellSlot.E].Cast(target);
                        laste = Utils.GameTimeTickCount;
                    }
                }
            }
        }

        #endregion

        #region KillSteal

        static void Killsteal()
        {
            if (!CassioUtils.getCheckBoxItem(CassiopeiaMenu.KillSteal, "Killsteal"))
            {
                return;
            }
            var targets = HeroManager.Enemies.Where(x => x.LSIsValidTarget() && !x.IsInvulnerable & !x.IsZombie);

            if (SpellSlot.Q.IsReady() && CassioUtils.getCheckBoxItem(CassiopeiaMenu.KillSteal, "Killsteal.UseQ"))
            {
                AIHeroClient qtarget =
                    targets.Where(x => x.LSDistance(Player.Position) < Spells[SpellSlot.Q].Range)
                    .MinOrDefault(x => x.Health);
                if (qtarget != null)
                {
                    var qdmg = Player.LSGetSpellDamage(qtarget, SpellSlot.Q);
                    if (qtarget.Health < qdmg)
                    {
						//Spells[SpellSlot.Q].SPredictionCast(target,       CassioUtils.GetHitChance("Hitchance.Q"));
	                    //return;
	                    
                        var pred = Spells[SpellSlot.Q].GetPrediction(qtarget);
                        if (pred != null && pred.Hitchance >= HitChance.Medium)
                        {
                            Spells[SpellSlot.W].Cast(pred.CastPosition);
                            return;
                        }
						
                    }
                }
            }

            if (SpellSlot.W.IsReady() && CassioUtils.getCheckBoxItem(CassiopeiaMenu.KillSteal, "Killsteal.UseW"))
            {
                AIHeroClient wtarget =
                    targets.Where(x => x.LSDistance(Player.Position) < Spells[SpellSlot.W].Range)
                        .MinOrDefault(x => x.Health);
                if (wtarget != null)
                {
                    var wdmg = Player.LSGetSpellDamage(wtarget, SpellSlot.W);
                    if (wtarget.Health < wdmg)
                    {
						//Spells[SpellSlot.W].SPredictionCast(target, CassioUtils.GetHitChance("Hitchance.W"));
						
                        var pred = Spells[SpellSlot.W].GetPrediction(wtarget);
                        if (pred != null && pred.Hitchance >= HitChance.Medium)
                        {
                            Spells[SpellSlot.W].Cast(pred.CastPosition);
                            return;
                        }
						
					}
                }
            }
            if (SpellSlot.E.IsReady() && CassioUtils.getCheckBoxItem(CassiopeiaMenu.KillSteal, "Killsteal.UseE"))
            {

                AIHeroClient etarget =
                    targets.Where(x => x.LSDistance(Player.Position) < Spells[SpellSlot.E].Range)
                    .MinOrDefault(x => x.Health);
                if (etarget != null)
                {
                    var edmg = Player.LSGetSpellDamage(etarget, SpellSlot.E);
                    if (etarget.Health < edmg)
                    {
                        if ((Utils.GameTimeTickCount - laste) > edelay)
                        {
                            Spells[SpellSlot.E].Cast(etarget);
                            laste = Utils.GameTimeTickCount;
                        }
                    }
                }
            }

            if (SpellSlot.R.IsReady() && CassioUtils.getCheckBoxItem(CassiopeiaMenu.KillSteal, "Killsteal.UseR"))
            {

                var targ = HeroManager.Enemies.FirstOrDefault(x => x.LSIsValidTarget() && Vector3.Distance(Player.ServerPosition, x.ServerPosition) < Spells[SpellSlot.R].Range - 200 && x.Health < Player.LSGetSpellDamage(x, SpellSlot.R) && !x.IsZombie && !x.IsInvulnerable);

                if (targ != null)
                {
					//Spells[SpellSlot.R].SPredictionCast(target, CassioUtils.GetHitChance("Hitchance.R"));
					
                    var pred = Spells[SpellSlot.R].GetPrediction(targ);
                    if (pred.Hitchance >= CassioUtils.RChance())
                    {
                        Spells[SpellSlot.R].Cast(pred.CastPosition);
                    }
					
				}
            }

            if (Spells[IgniteSlot].IsReady() && CassioUtils.getCheckBoxItem(CassiopeiaMenu.KillSteal, "Killsteal.UseIgnite"))
            {
                var targ =
                     HeroManager.Enemies.FirstOrDefault(x => x.LSIsValidTarget() &&
                             Vector3.Distance(Player.ServerPosition, x.ServerPosition) < Spells[IgniteSlot].Range && x.Health < (Player.GetSummonerSpellDamage(x, LeagueSharp.Common.Damage.SummonerSpell.Ignite)));
                if (targ != null)
                {
                    Spells[IgniteSlot].Cast(targ);
                }

            }
        }
        #endregion KillSteal


        #region PoisonCheck

        static bool isPoisoned(this Obj_AI_Base target)
        {
            return target.HasBuffOfType(BuffType.Poison);
        }
        #endregion

        #region Killable

        public static List<AIHeroClient> Killable = new List<AIHeroClient>();
        private static void CheckKillable(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }
            Killable.Clear();
            foreach (var hero in HeroManager.Enemies)
            {
                if (hero.LSIsValidTarget(10000) && hero.canKill())
                {
                    Killable.Add(hero);
                }
            }
        }

        public static bool canKill(this AIHeroClient target)
        {
            double totaldmgavailable = 0;
            if (SpellSlot.Q.IsReady() && CassioUtils.getCheckBoxItem(CassiopeiaMenu.Combo, "Combo.UseQ"))
            {
                totaldmgavailable += Player.LSGetSpellDamage(target, SpellSlot.Q);
            }
            if (SpellSlot.E.IsReady() && CassioUtils.getCheckBoxItem(CassiopeiaMenu.Combo, "Combo.UseE"))
            {
                totaldmgavailable += Player.LSGetSpellDamage(target, SpellSlot.E);
            }
            if (SpellSlot.R.IsReady() && CassioUtils.getCheckBoxItem(CassiopeiaMenu.Combo, "Combo.UseR"))
            {
                totaldmgavailable += Player.LSGetSpellDamage(target, SpellSlot.R);
            }

            if (Spells[IgniteSlot].IsReady() && CassioUtils.getCheckBoxItem(CassiopeiaMenu.KillSteal, "Killsteal.UseIgnite"))
            {
                totaldmgavailable += Player.GetSummonerSpellDamage(target, LeagueSharp.Common.Damage.SummonerSpell.Ignite);
            }
            return totaldmgavailable > target.Health;
        }
        #endregion


        #region AntiGapcloser

        static void OnGapClose(ActiveGapcloser args)
        {
            if (Player.IsDead || Player.recalling())
            {
                return;
            }
            var sender = args.Sender;

            if (CassioUtils.getCheckBoxItem(CassiopeiaMenu.Interrupter, "Interrupter.AntiGapClose") && sender.LSIsValidTarget())
            {
                if (CassioUtils.getCheckBoxItem(CassiopeiaMenu.Interrupter, "Interrupter.AG.UseR") && Vector3.Distance(args.End, Player.ServerPosition) <= Spells[SpellSlot.R].Range && sender.LSIsFacing(Player))
                {
	                //Spells[SpellSlot.R].SPredictionCast(sender, CassioUtils.GetHitChance("Hitchance.R"));
	                
                    var pred = Spells[SpellSlot.R].GetPrediction(sender);
                    if (pred.Hitchance >= HitChance.VeryHigh && sender.LSIsFacing(Player))
                    {
                        Spells[SpellSlot.R].Cast(pred.CastPosition);
                    }
					
                }
            }
        }
        #endregion


        #region Interrupter
        static void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }
            if (sender.LSIsValidTarget())
            {
                if (CassioUtils.getCheckBoxItem(CassiopeiaMenu.Interrupter, "Interrupter.UseR") && Vector3.Distance(sender.ServerPosition, Player.ServerPosition) <= Spells[SpellSlot.R].Range && sender.LSIsFacing(Player))
                {
					//Spells[SpellSlot.R].SPredictionCast(sender, CassioUtils.GetHitChance("Hitchance.R"));
					
                    var pred = Spells[SpellSlot.R].GetPrediction(sender);
                    if (pred.Hitchance >= HitChance.VeryHigh && sender.LSIsFacing(Player))
                    {
                        Spells[SpellSlot.R].Cast(pred.CastPosition);
                    }
					
				}
            }
        }
        #endregion


        #region Drawing

        static void OnDraw(EventArgs args)
        {
            if (Player.IsDead || CassioUtils.getCheckBoxItem(CassiopeiaMenu.Drawings, "Drawing.Disable"))
            {
                return;
            }
            foreach (var x in Killable)
            {
                var pos = Drawing.WorldToScreen(x.ServerPosition);
                Drawing.DrawText(pos.X, pos.Y, System.Drawing.Color.Azure, "Killable");
            }


            if (CassioUtils.getCheckBoxItem(CassiopeiaMenu.Drawings, "Drawing.DrawQ"))
            {
                Render.Circle.DrawCircle(Player.Position, Spells[SpellSlot.Q].Range, System.Drawing.Color.White);
            }
            if (CassioUtils.getCheckBoxItem(CassiopeiaMenu.Drawings, "Drawing.DrawW"))
            {
                Render.Circle.DrawCircle(Player.Position, Spells[SpellSlot.Q].Range, System.Drawing.Color.Green);
            }
            if (CassioUtils.getCheckBoxItem(CassiopeiaMenu.Drawings, "Drawing.DrawE"))
            {
                Render.Circle.DrawCircle(Player.Position, Spells[SpellSlot.E].Range, System.Drawing.Color.RoyalBlue);
            }
            if (CassioUtils.getCheckBoxItem(CassiopeiaMenu.Drawings, "Drawing.DrawR"))
            {
                Render.Circle.DrawCircle(Player.Position, Spells[SpellSlot.R].Range, System.Drawing.Color.Red);
            }

        }
        #endregion

    }
}





