using System;
using System.Linq;
using AutoJungle.Data;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using UnderratedAIO.Helpers;
using EloBuddy;

namespace AutoJungle
{
    internal class Champdata
    {
        public AIHeroClient Hero = null;
        public BuildType Type;

        public Func<bool> JungleClear;
        public Func<bool> Combo;
        public Spell R;
        public static Spell Q;
        public Spell W;
        public static Spell E;
        public AutoLeveler Autolvl;

        public Champdata()
        {
            switch (ObjectManager.Player.ChampionName)
            {
                case "MasterYi":
                    Hero = ObjectManager.Player;
                    Type = BuildType.NOC;

                    Q = new Spell(SpellSlot.Q, 600);
                    Q.SetTargetted(0.5f, float.MaxValue);
                    W = new Spell(SpellSlot.W);
                    E = new Spell(SpellSlot.E);
                    R = new Spell(SpellSlot.R);

                    Autolvl = new AutoLeveler(new int[] { 0, 2, 1, 0, 0, 3, 0, 2, 0, 2, 3, 2, 2, 1, 1, 3, 1, 1 });

                    JungleClear = MasteryiJungleClear;
                    Combo = MasteryiCombo;
                    Console.WriteLine("Masteryi loaded");
                    break;

                case "Warwick":
                    Hero = ObjectManager.Player;
                    Type = BuildType.AS;

                    Q = new Spell(SpellSlot.Q, 400, DamageType.Magical);
                    Q.SetTargetted(0.5f, float.MaxValue);
                    W = new Spell(SpellSlot.W, 1250);
                    E = new Spell(SpellSlot.E);
                    R = new Spell(SpellSlot.R, 700, DamageType.Magical);
                    R.SetTargetted(0.5f, float.MaxValue);

                    Autolvl = new AutoLeveler(new int[] { 0, 1, 2, 0, 0, 3, 0, 1, 0, 1, 3, 1, 1, 2, 2, 3, 2, 2 });

                    JungleClear = WarwickJungleClear;
                    Combo = WarwickCombo;

                    Console.WriteLine("Warwick loaded");
                    break;

                case "Shyvana":
                    Hero = ObjectManager.Player;
                    Type = BuildType.AS;

                    Q = new Spell(SpellSlot.Q);
                    W = new Spell(SpellSlot.W, 350f);
                    E = new Spell(SpellSlot.E, 925f);
                    E.SetSkillshot(0.25f, 60f, 1500, false, SkillshotType.SkillshotLine);
                    R = new Spell(SpellSlot.R, 1000f);
                    R.SetSkillshot(0.25f, 150f, 1500, false, SkillshotType.SkillshotLine);

                    Autolvl = new AutoLeveler(new int[] { 1, 2, 0, 1, 1, 3, 1, 0, 1, 0, 3, 0, 0, 2, 2, 3, 2, 2 });

                    JungleClear = ShyvanaJungleClear;
                    Combo = ShyvanaCombo;

                    Console.WriteLine("Shyvana loaded");
                    break;

                case "SkarnerNOTWORKINGYET":
                    Hero = ObjectManager.Player;
                    Type = BuildType.AS;

                    Q = new Spell(SpellSlot.Q, 325);
                    W = new Spell(SpellSlot.W);
                    E = new Spell(SpellSlot.E, 985);
                    E.SetSkillshot(0.5f, 60, 1200, false, SkillshotType.SkillshotLine);
                    R = new Spell(SpellSlot.R, 325);

                    Autolvl = new AutoLeveler(new int[] { 0, 1, 2, 0, 0, 3, 0, 2, 0, 2, 3, 2, 2, 1, 1, 3, 1, 1 });

                    JungleClear = SkarnerJungleClear;
                    Combo = SkarnerCombo;

                    Console.WriteLine("Skarner loaded");
                    break;
                case "Jax":
                    Hero = ObjectManager.Player;
                    Type = BuildType.ASMANA;

                    Q = new Spell(SpellSlot.Q, 680f);
                    Q.SetTargetted(0.50f, 75f);
                    W = new Spell(SpellSlot.W);
                    E = new Spell(SpellSlot.E);
                    R = new Spell(SpellSlot.R);

                    Autolvl = new AutoLeveler(new int[] { 2, 1, 0, 0, 0, 3, 0, 1, 0, 1, 3, 1, 1, 2, 2, 3, 2, 2 });
                    JungleClear = JaxJungleClear;
                    Combo = JaxCombo;

                    Console.WriteLine("Jax loaded");
                    break;
                case "XinZhao":
                    Hero = ObjectManager.Player;
                    Type = BuildType.AS;

                    Q = new Spell(SpellSlot.Q);
                    W = new Spell(SpellSlot.W);
                    E = new Spell(SpellSlot.E, 600);
                    R = new Spell(SpellSlot.R, 450f);

                    Autolvl = new AutoLeveler(new int[] { 0, 1, 2, 0, 0, 3, 0, 2, 0, 2, 3, 2, 2, 1, 1, 3, 1, 1 });

                    JungleClear = XinJungleClear;
                    Combo = XinCombo;
                    Console.WriteLine("Xin Zhao loaded");
                    break;

                    case "Nocturne":
                    Hero = ObjectManager.Player;
                    Type = BuildType.NOC;

                    Q = new Spell(SpellSlot.Q, 1150);
                    Q.SetSkillshot(0.25f, 60f, 1350, false, SkillshotType.SkillshotLine);
                    W = new Spell(SpellSlot.W);
                    E = new Spell(SpellSlot.E, 400, DamageType.Magical);
                    E.SetTargetted(0.50f, 75f);
                    R = new Spell(SpellSlot.R, 4000);
                    R.SetTargetted(0.75f, 4000f);

                    Autolvl = new AutoLeveler(new int[] { 0, 2, 1, 0, 0, 3, 0, 2, 0, 2, 3, 2, 2, 1, 1, 3, 1, 1 });

                    JungleClear = NocturneJungleClear;
                    Combo = NocturneCombo;
                    Console.WriteLine("Nocturne loaded");
                    break;

                    case "Evelynn":
                    Hero = ObjectManager.Player;
                    Type = BuildType.EVE;

                    Q = new Spell(SpellSlot.Q, 500f);
                    W = new Spell(SpellSlot.W);
                    E = new Spell(SpellSlot.E, 225);
                    R = new Spell(SpellSlot.R, 650);
                    R.SetSkillshot(R.Instance.SData.SpellCastTime, R.Instance.SData.LineWidth, R.Speed, false, SkillshotType.SkillshotCone);

                    Autolvl = new AutoLeveler(new int[] { 0, 2, 1, 0, 0, 3, 0, 2, 0, 2, 3, 2, 2, 1, 1, 3, 1, 1 });

                    JungleClear = EveJungleClear;
                    Combo = EveCombo;
                    Console.WriteLine("Evelynn loaded");
                    break;

                    case "Volibear":
                    Hero = ObjectManager.Player;
                    Type = BuildType.NOC;

                    Q = new Spell(SpellSlot.Q);
                    W = new Spell(SpellSlot.W, 400);
                    E = new Spell(SpellSlot.E, 425);
                    R = new Spell(SpellSlot.R);

                    Autolvl = new AutoLeveler(new int[] { 2, 1, 0, 1, 1, 3, 1, 0, 1, 0, 3, 0, 0, 2, 2, 3, 2, 2 });

                    JungleClear = VbJungleClear;
                    Combo = VoliCombo;
                    Console.WriteLine("Volibear loaded");
                    break;
                	default:
                    Console.WriteLine(ObjectManager.Player.ChampionName + " not supported");
                    break;
//nidale w buff?(优先）)nunu R check | sej，结束skr，amumu？ graves！
            }
        }

        private bool VoliCombo()
        {
            var targetHero = Program._GameInfo.Target;
            if (Hero.Spellbook.IsChanneling)
            {
                return false;
            }
            if (Program.getCheckBoxItem("ComboSmite"))
            {
                Data.Jungle.CastSmiteHero((AIHeroClient) targetHero);
            }
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            if (Q.IsReady() && targetHero.LSIsValidTarget(550))
            {
                Q.Cast();
            }
            if (E.IsReady() && targetHero.LSIsValidTarget(425))
            {
                E.Cast();
            }
            if (R.IsReady() && Hero.LSDistance(targetHero) < 400 && Hero.Mana > 100)
            {
                R.Cast();
            }
            if (W.IsReady() && targetHero.LSIsValidTarget(400))
            {
                W.CastOnUnit(targetHero);
            }
            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, targetHero);
            return false;
        }

        private bool VbJungleClear()
        {
        	var targetMob = Program._GameInfo.Target;
            var structure = Helpers.CheckStructure();
            if (structure != null)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, structure);
                return false;
            }
            if (targetMob == null)
            {
                return false;
            }
            ItemHandler.UseItemsJungle();
            if (E.IsReady() && targetMob.LSIsValidTarget(425) && (Hero.ManaPercent > 60 || Hero.HealthPercent < 50))
            {
                E.Cast();
            }
            if (Q.IsReady() && targetMob.LSIsValidTarget(550))
            {
                Q.Cast();
            }
            if (W.IsReady() && targetMob.LSIsValidTarget(400))
            {
                W.CastOnUnit(targetMob);
            }
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, targetMob);
            return false;
        }

        private bool EveCombo()
        {
            var targetHero = Program._GameInfo.Target;
            if (Hero.Spellbook.IsChanneling)
            {
                return false;
            }
            if (Program.getCheckBoxItem("ComboSmite"))
            {
                Data.Jungle.CastSmiteHero((AIHeroClient) targetHero);
            }
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            if (Q.IsReady() && Q.CanCast(targetHero))
            {
                Q.CastOnUnit(targetHero);
            }
            if (W.IsReady() && targetHero.LSIsValidTarget(750))
            {
                W.Cast();
            }
            if (R.IsReady() && Hero.LSDistance(targetHero) < 650 && Hero.Mana > 100)
            {
                R.Cast(targetHero);
            }
            if (E.IsReady() && E.CanCast(targetHero))
            {
                E.CastOnUnit(targetHero);
            }
            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, targetHero);
            return false;
        }

        private bool EveJungleClear()
        {
            var targetMob = Program._GameInfo.Target;
            var structure = Helpers.CheckStructure();
            if (structure != null)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, structure);
                return false;
            }
            if (targetMob == null)
            {
                return false;
            }
            ItemHandler.UseItemsJungle();
            if (Q.IsReady() && Hero.LSDistance(targetMob) < Q.Range &&
            (Helpers.getMobs(Hero.Position, Q.Range).Count >= 2 || targetMob.MaxHealth>700))
            {
                Q.Cast(targetMob);
            }
            if (E.IsReady() && E.CanCast(targetMob) && (Hero.ManaPercent > 60 || targetMob.MaxHealth>700))
            {
                E.CastOnUnit(targetMob);
            }
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, targetMob);
            return false;
        }

        private bool JaxCombo()
        {
            var targetHero = Program._GameInfo.Target;
            if (targetHero == null)
            {
                return false;
            }
            if (R.IsReady() && Hero.LSDistance(targetHero) < 300 && Hero.Mana > 250)
            {
                R.Cast();
            }
            if (W.IsReady() && targetHero.LSIsValidTarget(300))
            {
                W.Cast();
            }
            ItemHandler.UseItemsCombo(targetHero, !Q.IsReady());
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            if (Q.IsReady() && Q.CanCast(targetHero) &&
                (targetHero.LSDistance(Hero) > Orbwalking.GetRealAutoAttackRange(targetHero) || Hero.HealthPercent < 40))
            {
                Q.CastOnUnit(targetHero);
            }
            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, targetHero);
            return false;
        }

        private bool XinJungleClear()
        {
            var targetMob = Program._GameInfo.Target;
            var structure = Helpers.CheckStructure();
            if (structure != null)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, structure);
                return false;
            }
            if (targetMob == null)
            {
                return false;
            }
            if (W.IsReady() && targetMob.LSIsValidTarget(300) && (Hero.ManaPercent > 60 || Hero.HealthPercent < 50))
            {
                W.Cast();
            }
            ItemHandler.UseItemsJungle();
            if (Q.IsReady() && targetMob.LSIsValidTarget(300))
            {
                Q.Cast();
            }
            if (E.IsReady() && E.CanCast(targetMob) && (Hero.ManaPercent > 60 || Hero.HealthPercent < 50))
            {
                E.CastOnUnit(targetMob);
            }
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, targetMob);
            return false;
        }

        private bool XinCombo()
        {
            var targetHero = Program._GameInfo.Target;
            if (targetHero == null)
            {
                return false;
            }
            if (R.IsReady() && Hero.LSDistance(targetHero) < R.Range && targetHero.HasBuff("xenzhaointimidate") &&
                targetHero.Health > R.GetDamage(targetHero) + Hero.LSGetAutoAttackDamage(targetHero, true) * 4)
            {
                R.Cast();
            }
            if (W.IsReady() && targetHero.LSIsValidTarget(300))
            {
                W.Cast();
            }
            ItemHandler.UseItemsCombo(targetHero, !E.IsReady());
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            if (Q.IsReady() && targetHero.LSDistance(Hero) < Orbwalking.GetRealAutoAttackRange(targetHero) + 50)
            {
                Q.Cast();
            }
            if (E.IsReady() && E.CanCast(targetHero) &&
                (Hero.HealthPercent < 40 || targetHero.LSDistance(Hero) > Orbwalking.GetRealAutoAttackRange(targetHero) ||
                 Prediction.GetPrediction(targetHero, 1f).UnitPosition.UnderTurret(true)))
            {
                E.CastOnUnit(targetHero);
            }
            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, targetHero);
            return false;
        }

        private bool JaxJungleClear()
        {
            var targetMob = Program._GameInfo.Target;
            var structure = Helpers.CheckStructure();
            if (structure != null)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, structure);
                return false;
            }
            if (targetMob == null)
            {
                return false;
            }
            if (W.IsReady() && targetMob.LSIsValidTarget(300))
            {
                W.Cast();
            }
            ItemHandler.UseItemsJungle();
            if (Q.IsReady() && Q.CanCast(targetMob) && (Hero.ManaPercent > 60 || Hero.HealthPercent < 50))
            {
                Q.CastOnUnit(targetMob);
            }
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, targetMob);
            return false;
        }

        private bool SkarnerCombo()
        {
            var targetHero = Program._GameInfo.Target;
            var rActive = Hero.HasBuff("skarnerimpalevo");
            if (W.IsReady() && targetHero != null && Hero.LSDistance(targetHero) < 700)
            {
                W.Cast();
            }
            ItemHandler.UseItemsCombo(targetHero, !E.IsReady());
            if (Q.IsReady() && ((targetHero != null && Q.CanCast(targetHero)) || rActive))
            {
                Q.Cast();
            }
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            if (E.IsReady() && !rActive && targetHero != null && E.CanCast(targetHero) &&
                Hero.LSDistance(targetHero) < 700)
            {
                E.CastIfHitchanceEquals(targetHero, HitChance.High);
            }
            if (R.IsReady() && targetHero != null && R.CanCast(targetHero) && !targetHero.HasBuff("SkarnerImpale"))
            {
                R.CastOnUnit(targetHero);
            }
            if (rActive)
            {
                var allyTower =
                    Program._GameInfo.AllyStructures.OrderBy(a => a.LSDistance(Hero.Position)).FirstOrDefault();
                if (allyTower != null && allyTower.LSDistance(Hero.Position) < 2000 &&
                    allyTower.LSDistance(Hero.Position) > 300)
                {
                    Console.WriteLine(2);
                    Console.WriteLine(allyTower.LSDistance(Hero.Position));
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, allyTower.LSExtend(Program._GameInfo.SpawnPoint, 300));
                    Program.pos = allyTower.LSExtend(Program._GameInfo.SpawnPoint, 300);
                    return false;
                }
                var ally =
                    HeroManager.Allies.Where(a => a.LSDistance(Hero.Position) < 1500)
                        .OrderBy(a => a.LSDistance(Hero))
                        .FirstOrDefault();
                if (ally != null && ally.LSDistance(Hero) > 300)
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, ally.Position);
                    Console.WriteLine(1);
                    Program.pos = ally.Position;
                    return false;
                }
                var enemyTower =
                    Program._GameInfo.EnemyStructures.OrderBy(a => a.LSDistance(Hero.Position)).FirstOrDefault();
                if (enemyTower != null && enemyTower.LSDistance(Hero.Position) < 2000 &&
                    enemyTower.LSDistance(Hero.Position) > 300)
                {
                    Console.WriteLine(3);
                    Program.pos = targetHero.Position.LSExtend(enemyTower, 2500);
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Hero.Position.LSExtend(enemyTower, 2500));
                    return false;
                }
            }
            else if (targetHero != null)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, targetHero);
            }
            return false;
        }

        private bool SkarnerJungleClear()
        {
            var targetMob = Program._GameInfo.Target;
            var structure = Helpers.CheckStructure();
            if (structure != null)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, structure);
                return false;
            }
            if (targetMob == null)
            {
                return false;
            }
            if (W.IsReady() && Hero.LSDistance(targetMob) < Q.Range &&
                (Helpers.getMobs(Hero.Position, W.Range).Count >= 2 ||
                 targetMob.Health > Hero.LSGetAutoAttackDamage(targetMob, true) * 5))
            {
                W.Cast();
            }
            ItemHandler.UseItemsJungle();
            if (Q.IsReady() && Q.CanCast(targetMob))
            {
                Q.Cast();
            }
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            if (E.IsReady() && E.CanCast(targetMob))
            {
                var pred = E.GetLineFarmLocation(Helpers.getMobs(Hero.Position, E.Range));
                if (pred.MinionsHit >= 2 || targetMob.Health > Hero.LSGetAutoAttackDamage(targetMob, true) * 5)
                {
                    E.CastIfHitchanceEquals(targetMob, HitChance.VeryHigh);
                }
            }
            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, targetMob);
            return false;
        }

        private bool ShyvanaCombo()
        {
            var targetHero = Program._GameInfo.Target;
            if (Hero.Spellbook.IsChanneling)
            {
                return false;
            }
            if (W.IsReady() && Hero.LSDistance(targetHero) < W.Range + 100)
            {
                W.Cast();
            }
            ItemHandler.UseItemsCombo(targetHero, true);
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            if (Q.IsReady() && Orbwalking.GetRealAutoAttackRange(targetHero) > Hero.LSDistance(targetHero))
            {
                Q.Cast();
            }
            if (E.IsReady() && E.CanCast(targetHero))
            {
                E.Cast(targetHero);
            }
            if (R.IsReady() && Hero.Mana == 100 &&
                targetHero.LSCountEnemiesInRange(GameInfo.ChampionRange) <=
                targetHero.CountAlliesInRange(GameInfo.ChampionRange) &&
                !Hero.Position.LSExtend(targetHero.Position, GameInfo.ChampionRange).UnderTurret(true))
            {
                R.CastIfHitchanceEquals(targetHero, HitChance.VeryHigh);
            }

            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, targetHero);
            return false;
        }

        private bool ShyvanaJungleClear()
        {
            var targetMob = Program._GameInfo.Target;
            if (Hero.Spellbook.IsChanneling)
            {
                return false;
            }
            var structure = Helpers.CheckStructure();
            if (structure != null)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, structure);
                return false;
            }
            if (targetMob == null)
            {
                return false;
            }
            if (W.IsReady() && Hero.LSDistance(targetMob) < W.Range &&
                (Helpers.getMobs(Hero.Position, W.Range).Count >= 2 ||
                 targetMob.Health > W.GetDamage(targetMob) * 7 + Hero.LSGetAutoAttackDamage(targetMob, true) * 2))
            {
                W.Cast();
            }
            ItemHandler.UseItemsJungle();
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            if (Q.IsReady())
            {
                Q.Cast();
                EloBuddy.Player.IssueOrder(GameObjectOrder.AutoAttack, targetMob);
            }
            if (E.IsReady() && E.CanCast(targetMob))
            {
                E.Cast(targetMob);
            }
            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, targetMob);
            return false;
        }

        private bool WarwickCombo()
        {
            var targetHero = Program._GameInfo.Target;
            if (Hero.Spellbook.IsChanneling)
            {
                return false;
            }
            if (Program.getCheckBoxItem("ComboSmite"))
            {
                Data.Jungle.CastSmiteHero((AIHeroClient) targetHero);
            }
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            if (Q.IsReady() && Q.CanCast(targetHero))
            {
                Q.CastOnUnit(targetHero);
            }
            if (W.IsReady() && Hero.LSDistance(targetHero) < 300)
            {
                if (Hero.Mana > Q.ManaCost + W.ManaCost || Hero.HealthPercent > 70)
                {
                    W.Cast();
                }
            }
            if (R.IsReady() && R.CanCast(targetHero) && !targetHero.MagicImmune)
            {
                R.CastOnUnit(targetHero);
            }
            if (E.IsReady() && Hero.Spellbook.GetSpell(SpellSlot.E).ToggleState == 1 && Hero.LSDistance(targetHero) < 1000)
            {
                E.Cast();
            }
            ItemHandler.UseItemsCombo(targetHero, !R.IsReady());
            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, targetHero);
            return false;
        }

        private bool WarwickJungleClear()
        {
            var targetMob = Program._GameInfo.Target;
            if (Hero.Spellbook.IsChanneling)
            {
                return false;
            }
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            var structure = Helpers.CheckStructure();
            if (structure != null)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, structure);
                return false;
            }
            if (targetMob == null)
            {
                return false;
            }
            if (Q.IsReady() && Q.CanCast(targetMob) &&
                (Hero.ManaPercent > 50 || Hero.MaxHealth - Hero.Health > Q.GetDamage(targetMob) * 0.8f))
            {
                Q.CastOnUnit(targetMob);
            }
            if (W.IsReady() && Hero.LSDistance(targetMob) < 300 && (Program._GameInfo.SmiteableMob != null) ||
                Program._GameInfo.MinionsAround > 3)
            {
                if (Hero.Mana > Q.ManaCost + W.ManaCost || Hero.HealthPercent > 70)
                {
                    W.Cast();
                }
            }
            if (E.IsReady() && Hero.Spellbook.GetSpell(SpellSlot.E).ToggleState != 1 && Hero.LSDistance(targetMob) < 500)
            {
                E.Cast();
            }
            ItemHandler.UseItemsJungle();
            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, targetMob);
            return false;
        }

        private bool MasteryiJungleClear()
        {
            var targetMob = Program._GameInfo.Target;
            if (Hero.Spellbook.IsChanneling)
            {
                return false;
            }
            if (E.IsReady() && Hero.Spellbook.IsAutoAttacking)
            {
                E.Cast();
            }
            var structure = Helpers.CheckStructure();
            if (structure != null)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, structure);
                return false;
            }
            if (targetMob == null)
            {
                return false;
            }
            if (R.IsReady() && Hero.Position.LSDistance(Hero.Position) < 300 &&
                Data.Jungle.bosses.Any(n => targetMob.Name.Contains(n)))
            {
                R.Cast();
            }
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            if (Q.IsReady() && Q.CanCast(targetMob) && targetMob.Health < targetMob.MaxHealth)
            {
                Q.CastOnUnit(targetMob);
            }
            if (W.IsReady() && Hero.HealthPercent < 50)
            {
                W.Cast();
            }
            ItemHandler.UseItemsJungle();
            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, targetMob);
            return false;
        }

        private bool MasteryiCombo()
        {
            var targetHero = Program._GameInfo.Target;
            if (Hero.Spellbook.IsChanneling &&
                targetHero.Health > Program.player.LSGetAutoAttackDamage(targetHero, true) * 2)
            {
                return false;
            }
            if (E.IsReady() && Hero.Spellbook.IsAutoAttacking)
            {
                E.Cast();
            }
            if (R.IsReady() && Hero.LSDistance(targetHero) < 600)
            {
                R.Cast();
            }
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            if (Q.IsReady())
            {
                Q.CastOnUnit(targetHero);
            }
            if (W.IsReady() && Hero.HealthPercent < 25 || Program._GameInfo.DamageTaken >= Hero.Health / 3)
            {
                W.Cast();
            }
            ItemHandler.UseItemsCombo(targetHero, !Q.IsReady());
            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, targetHero);
            return false;
        }

        private bool NocturneCombo()
        {
            var targetHero = Program._GameInfo.Target;
            if (Hero.Spellbook.IsChanneling)
            {
                return false;
            }
            if (Program.getCheckBoxItem("ComboSmite"))
            {
                Data.Jungle.CastSmiteHero((AIHeroClient) targetHero);
            }
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            /* check under tower? r active 1sec delay move to target
                        if (R.IsReady() && Hero.LSDistance(targetHero) < 1300 &&
                            (targetHero.LSDistance(Hero) > Orbwalking.GetRealAutoAttackRange(targetHero) &&
                            targetHero.UnderTurret(true))
            {
                R.CastOnUnit(targetHero);
            }
            */
            if (R.IsReady() && Hero.LSDistance(targetHero) < 900)
            {
                R.CastOnUnit(targetHero);
            }
            if (Q.IsReady() && Q.CanCast(targetHero))
            {
                Q.Cast(targetHero);
            }
            if (W.IsReady() && targetHero.LSIsValidTarget(300))
            {
                W.Cast();
            }
            if (E.IsReady() && E.CanCast(targetHero))
            {
                E.CastOnUnit(targetHero);
            }
            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, targetHero);
            return false;        }

        private bool NocturneJungleClear()
        {
            var targetMob = Program._GameInfo.Target;
            var structure = Helpers.CheckStructure();
            if (structure != null)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, structure);
                return false;
            }
            if (targetMob == null)
            {
                return false;
            }
            ItemHandler.UseItemsJungle();
            if (Q.IsReady() && targetMob.LSIsValidTarget(400))
            {
                Q.Cast(targetMob);
            }
            if (E.IsReady() && E.CanCast(targetMob) && (Hero.ManaPercent > 60 || Hero.HealthPercent < 50))
            {
                E.CastOnUnit(targetMob);
            }
            if (Hero.Spellbook.IsAutoAttacking)
            {
                return false;
            }
            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, targetMob);
            return false;
        }
    }
}