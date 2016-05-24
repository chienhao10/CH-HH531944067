using System;
using System.Linq;
using AutoJungle.Data;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using UnderratedAIO.Helpers;

namespace AutoJungle
{
    internal class Champdata
    {
        public Obj_AI_Hero Hero = null;
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
                    Type = BuildType.AS;

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

                    Q = new Spell(SpellSlot.Q, 400, TargetSelector.DamageType.Magical);
                    Q.SetTargetted(0.5f, float.MaxValue);
                    W = new Spell(SpellSlot.W, 1250);
                    E = new Spell(SpellSlot.E);
                    R = new Spell(SpellSlot.R, 700, TargetSelector.DamageType.Magical);
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
                    E = new Spell(SpellSlot.E, 400, TargetSelector.DamageType.Magical);
                    E.SetTargetted(0.50f, 75f);
                    R = new Spell(SpellSlot.R, 4000);
                    R.SetTargetted(0.75f, 4000f);

                    Autolvl = new AutoLeveler(new int[] { 0, 2, 1, 0, 0, 3, 0, 2, 0, 2, 3, 2, 2, 1, 1, 3, 1, 1 });

                    JungleClear = NocturneJungleClear;
                    Combo = NocturneCombo;
                    Console.WriteLine("Nocturne loaded");
                    break;
                default:
                    Console.WriteLine(ObjectManager.Player.ChampionName + " not supported");
                    break;
            }
        }

        private bool JaxCombo()
        {
            var targetHero = Program._GameInfo.Target;
            if (targetHero == null)
            {
                return false;
            }
            if (R.IsReady() && Hero.Distance(targetHero) < 300 && Hero.Mana > 250)
            {
                R.Cast();
            }
            if (W.IsReady() && targetHero.IsValidTarget(300))
            {
                W.Cast();
            }
            ItemHandler.UseItemsCombo(targetHero, !Q.IsReady());
            if (Hero.IsWindingUp)
            {
                return false;
            }
            if (Q.IsReady() && Q.CanCast(targetHero) &&
                (targetHero.Distance(Hero) > Orbwalking.GetRealAutoAttackRange(targetHero) || Hero.HealthPercent < 40))
            {
                Q.CastOnUnit(targetHero);
            }
            Hero.IssueOrder(GameObjectOrder.AttackUnit, targetHero);
            return false;
        }

        private bool XinJungleClear()
        {
            var targetMob = Program._GameInfo.Target;
            var structure = Helpers.CheckStructure();
            if (structure != null)
            {
                Hero.IssueOrder(GameObjectOrder.AttackUnit, structure);
                return false;
            }
            if (targetMob == null)
            {
                return false;
            }
            if (W.IsReady() && targetMob.IsValidTarget(300) && (Hero.ManaPercent > 60 || Hero.HealthPercent < 50))
            {
                W.Cast();
            }
            ItemHandler.UseItemsJungle();
            if (Q.IsReady() && targetMob.IsValidTarget(300))
            {
                Q.Cast();
            }
            if (E.IsReady() && E.CanCast(targetMob) && (Hero.ManaPercent > 60 || Hero.HealthPercent < 50))
            {
                E.CastOnUnit(targetMob);
            }
            if (Hero.IsWindingUp)
            {
                return false;
            }
            Hero.IssueOrder(GameObjectOrder.AttackUnit, targetMob);
            return false;
        }

        private bool XinCombo()
        {
            var targetHero = Program._GameInfo.Target;
            if (targetHero == null)
            {
                return false;
            }
            if (R.IsReady() && Hero.Distance(targetHero) < R.Range && targetHero.HasBuff("xenzhaointimidate") &&
                targetHero.Health > R.GetDamage(targetHero) + Hero.GetAutoAttackDamage(targetHero, true) * 4)
            {
                R.Cast();
            }
            if (W.IsReady() && targetHero.IsValidTarget(300))
            {
                W.Cast();
            }
            ItemHandler.UseItemsCombo(targetHero, !E.IsReady());
            if (Hero.IsWindingUp)
            {
                return false;
            }
            if (Q.IsReady() && targetHero.Distance(Hero) < Orbwalking.GetRealAutoAttackRange(targetHero) + 50)
            {
                Q.Cast();
            }
            if (E.IsReady() && E.CanCast(targetHero) &&
                (Hero.HealthPercent < 40 || targetHero.Distance(Hero) > Orbwalking.GetRealAutoAttackRange(targetHero) ||
                 Prediction.GetPrediction(targetHero, 1f).UnitPosition.UnderTurret(true)))
            {
                E.CastOnUnit(targetHero);
            }
            Hero.IssueOrder(GameObjectOrder.AttackUnit, targetHero);
            return false;
        }

        private bool JaxJungleClear()
        {
            var targetMob = Program._GameInfo.Target;
            var structure = Helpers.CheckStructure();
            if (structure != null)
            {
                Hero.IssueOrder(GameObjectOrder.AttackUnit, structure);
                return false;
            }
            if (targetMob == null)
            {
                return false;
            }
            if (W.IsReady() && targetMob.IsValidTarget(300))
            {
                W.Cast();
            }
            ItemHandler.UseItemsJungle();
            if (Q.IsReady() && Q.CanCast(targetMob) && (Hero.ManaPercent > 60 || Hero.HealthPercent < 50))
            {
                Q.CastOnUnit(targetMob);
            }
            if (Hero.IsWindingUp)
            {
                return false;
            }
            Hero.IssueOrder(GameObjectOrder.AttackUnit, targetMob);
            return false;
        }

        private bool SkarnerCombo()
        {
            var targetHero = Program._GameInfo.Target;
            var rActive = Hero.HasBuff("skarnerimpalevo");
            if (W.IsReady() && targetHero != null && Hero.Distance(targetHero) < 700)
            {
                W.Cast();
            }
            ItemHandler.UseItemsCombo(targetHero, !E.IsReady());
            if (Q.IsReady() && ((targetHero != null && Q.CanCast(targetHero)) || rActive))
            {
                Q.Cast();
            }
            if (Hero.IsWindingUp)
            {
                return false;
            }
            if (E.IsReady() && !rActive && targetHero != null && E.CanCast(targetHero) &&
                Hero.Distance(targetHero) < 700)
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
                    Program._GameInfo.AllyStructures.OrderBy(a => a.Distance(Hero.Position)).FirstOrDefault();
                if (allyTower != null && allyTower.Distance(Hero.Position) < 2000 &&
                    allyTower.Distance(Hero.Position) > 300)
                {
                    Console.WriteLine(2);
                    Console.WriteLine(allyTower.Distance(Hero.Position));
                    Hero.IssueOrder(GameObjectOrder.MoveTo, allyTower.Extend(Program._GameInfo.SpawnPoint, 300));
                    Program.pos = allyTower.Extend(Program._GameInfo.SpawnPoint, 300);
                    return false;
                }
                var ally =
                    HeroManager.Allies.Where(a => a.Distance(Hero.Position) < 1500)
                        .OrderBy(a => a.Distance(Hero))
                        .FirstOrDefault();
                if (ally != null && ally.Distance(Hero) > 300)
                {
                    Hero.IssueOrder(GameObjectOrder.MoveTo, ally.Position);
                    Console.WriteLine(1);
                    Program.pos = ally.Position;
                    return false;
                }
                var enemyTower =
                    Program._GameInfo.EnemyStructures.OrderBy(a => a.Distance(Hero.Position)).FirstOrDefault();
                if (enemyTower != null && enemyTower.Distance(Hero.Position) < 2000 &&
                    enemyTower.Distance(Hero.Position) > 300)
                {
                    Console.WriteLine(3);
                    Program.pos = targetHero.Position.Extend(enemyTower, 2500);
                    Hero.IssueOrder(GameObjectOrder.MoveTo, Hero.Position.Extend(enemyTower, 2500));
                    return false;
                }
            }
            else if (targetHero != null)
            {
                Hero.IssueOrder(GameObjectOrder.AttackUnit, targetHero);
            }
            return false;
        }

        private bool SkarnerJungleClear()
        {
            var targetMob = Program._GameInfo.Target;
            var structure = Helpers.CheckStructure();
            if (structure != null)
            {
                Hero.IssueOrder(GameObjectOrder.AttackUnit, structure);
                return false;
            }
            if (targetMob == null)
            {
                return false;
            }
            if (W.IsReady() && Hero.Distance(targetMob) < Q.Range &&
                (Helpers.getMobs(Hero.Position, W.Range).Count >= 2 ||
                 targetMob.Health > Hero.GetAutoAttackDamage(targetMob, true) * 5))
            {
                W.Cast();
            }
            ItemHandler.UseItemsJungle();
            if (Q.IsReady() && Q.CanCast(targetMob))
            {
                Q.Cast();
            }
            if (Hero.IsWindingUp)
            {
                return false;
            }
            if (E.IsReady() && E.CanCast(targetMob))
            {
                var pred = E.GetLineFarmLocation(Helpers.getMobs(Hero.Position, E.Range));
                if (pred.MinionsHit >= 2 || targetMob.Health > Hero.GetAutoAttackDamage(targetMob, true) * 5)
                {
                    E.CastIfHitchanceEquals(targetMob, HitChance.VeryHigh);
                }
            }
            Hero.IssueOrder(GameObjectOrder.AttackUnit, targetMob);
            return false;
        }

        private bool ShyvanaCombo()
        {
            var targetHero = Program._GameInfo.Target;
            if (Hero.Spellbook.IsChanneling)
            {
                return false;
            }
            if (W.IsReady() && Hero.Distance(targetHero) < W.Range + 100)
            {
                W.Cast();
            }
            ItemHandler.UseItemsCombo(targetHero, true);
            if (Hero.IsWindingUp)
            {
                return false;
            }
            if (Q.IsReady() && Orbwalking.GetRealAutoAttackRange(targetHero) > Hero.Distance(targetHero))
            {
                Q.Cast();
            }
            if (E.IsReady() && E.CanCast(targetHero))
            {
                E.Cast(targetHero);
            }
            if (R.IsReady() && Hero.Mana == 100 &&
                targetHero.CountEnemiesInRange(GameInfo.ChampionRange) <=
                targetHero.CountAlliesInRange(GameInfo.ChampionRange) &&
                !Hero.Position.Extend(targetHero.Position, GameInfo.ChampionRange).UnderTurret(true))
            {
                R.CastIfHitchanceEquals(targetHero, HitChance.VeryHigh);
            }

            Hero.IssueOrder(GameObjectOrder.AttackUnit, targetHero);
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
                Hero.IssueOrder(GameObjectOrder.AttackUnit, structure);
                return false;
            }
            if (targetMob == null)
            {
                return false;
            }
            if (W.IsReady() && Hero.Distance(targetMob) < W.Range &&
                (Helpers.getMobs(Hero.Position, W.Range).Count >= 2 ||
                 targetMob.Health > W.GetDamage(targetMob) * 7 + Hero.GetAutoAttackDamage(targetMob, true) * 2))
            {
                W.Cast();
            }
            ItemHandler.UseItemsJungle();
            if (Hero.IsWindingUp)
            {
                return false;
            }
            if (Q.IsReady())
            {
                Q.Cast();
                Hero.IssueOrder(GameObjectOrder.AutoAttack, targetMob);
            }
            if (E.IsReady() && E.CanCast(targetMob))
            {
                E.Cast(targetMob);
            }
            Hero.IssueOrder(GameObjectOrder.AttackUnit, targetMob);
            return false;
        }

        private bool WarwickCombo()
        {
            var targetHero = Program._GameInfo.Target;
            if (Hero.Spellbook.IsChanneling)
            {
                return false;
            }
            if (Program.menu.Item("ComboSmite").GetValue<Boolean>())
            {
                Jungle.CastSmiteHero((Obj_AI_Hero) targetHero);
            }
            if (Hero.IsWindingUp)
            {
                return false;
            }
            if (Q.IsReady() && Q.CanCast(targetHero))
            {
                Q.CastOnUnit(targetHero);
            }
            if (W.IsReady() && Hero.Distance(targetHero) < 300)
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
            if (E.IsReady() && Hero.Spellbook.GetSpell(SpellSlot.E).ToggleState == 1 && Hero.Distance(targetHero) < 1000)
            {
                E.Cast();
            }
            ItemHandler.UseItemsCombo(targetHero, !R.IsReady());
            Hero.IssueOrder(GameObjectOrder.AttackUnit, targetHero);
            return false;
        }

        private bool WarwickJungleClear()
        {
            var targetMob = Program._GameInfo.Target;
            if (Hero.Spellbook.IsChanneling)
            {
                return false;
            }
            if (Hero.IsWindingUp)
            {
                return false;
            }
            var structure = Helpers.CheckStructure();
            if (structure != null)
            {
                Hero.IssueOrder(GameObjectOrder.AttackUnit, structure);
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
            if (W.IsReady() && Hero.Distance(targetMob) < 300 && (Program._GameInfo.SmiteableMob != null) ||
                Program._GameInfo.MinionsAround > 3)
            {
                if (Hero.Mana > Q.ManaCost + W.ManaCost || Hero.HealthPercent > 70)
                {
                    W.Cast();
                }
            }
            if (E.IsReady() && Hero.Spellbook.GetSpell(SpellSlot.E).ToggleState != 1 && Hero.Distance(targetMob) < 500)
            {
                E.Cast();
            }
            ItemHandler.UseItemsJungle();
            Hero.IssueOrder(GameObjectOrder.AttackUnit, targetMob);
            return false;
        }

        private bool MasteryiJungleClear()
        {
            var targetMob = Program._GameInfo.Target;
            if (Hero.Spellbook.IsChanneling)
            {
                return false;
            }
            if (E.IsReady() && Hero.IsWindingUp)
            {
                E.Cast();
            }
            var structure = Helpers.CheckStructure();
            if (structure != null)
            {
                Hero.IssueOrder(GameObjectOrder.AttackUnit, structure);
                return false;
            }
            if (targetMob == null)
            {
                return false;
            }
            if (R.IsReady() && Hero.Position.Distance(Hero.Position) < 300 &&
                Jungle.bosses.Any(n => targetMob.Name.Contains(n)))
            {
                R.Cast();
            }
            if (Hero.IsWindingUp)
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
            Hero.IssueOrder(GameObjectOrder.AttackUnit, targetMob);
            return false;
        }

        private bool MasteryiCombo()
        {
            var targetHero = Program._GameInfo.Target;
            if (Hero.Spellbook.IsChanneling &&
                targetHero.Health > Program.player.GetAutoAttackDamage(targetHero, true) * 2)
            {
                return false;
            }
            if (E.IsReady() && Hero.IsWindingUp)
            {
                E.Cast();
            }
            if (R.IsReady() && Hero.Distance(targetHero) < 600)
            {
                R.Cast();
            }
            if (Hero.IsWindingUp)
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
            Hero.IssueOrder(GameObjectOrder.AttackUnit, targetHero);
            return false;
        }

        private bool NocturneCombo()
        {
            var targetHero = Program._GameInfo.Target;
            if (Hero.Spellbook.IsChanneling)
            {
                return false;
            }
            if (Program.menu.Item("ComboSmite").GetValue<Boolean>())
            {
                Jungle.CastSmiteHero((Obj_AI_Hero) targetHero);
            }
            if (Hero.IsWindingUp)
            {
                return false;
            }
            /* check under tower? r active 1sec delay move to target
                        if (R.IsReady() && Hero.Distance(targetHero) < 1300 &&
                            (targetHero.Distance(Hero) > Orbwalking.GetRealAutoAttackRange(targetHero) &&
                            targetHero.UnderTurret(true))
            {
                R.CastOnUnit(targetHero);
            }
            */
            if (R.IsReady() && Hero.Distance(targetHero) < 900)
            {
                R.CastOnUnit(targetHero);
            }
            if (Q.IsReady() && Q.CanCast(targetHero))
            {
                Q.Cast(targetHero);
            }
            if (W.IsReady() && targetHero.IsValidTarget(300))
            {
                W.Cast();
            }
            if (E.IsReady() && E.CanCast(targetHero))
            {
                E.CastOnUnit(targetHero);
            }
            Hero.IssueOrder(GameObjectOrder.AttackUnit, targetHero);
            return false;        }

        private bool NocturneJungleClear()
        {
            var targetMob = Program._GameInfo.Target;
            var structure = Helpers.CheckStructure();
            if (structure != null)
            {
                Hero.IssueOrder(GameObjectOrder.AttackUnit, structure);
                return false;
            }
            if (targetMob == null)
            {
                return false;
            }
            ItemHandler.UseItemsJungle();
            if (Q.IsReady() && targetMob.IsValidTarget(400))
            {
                Q.Cast(targetMob);
            }
            if (E.IsReady() && E.CanCast(targetMob) && (Hero.ManaPercent > 60 || Hero.HealthPercent < 50))
            {
                E.CastOnUnit(targetMob);
            }
            if (Hero.IsWindingUp)
            {
                return false;
            }
            Hero.IssueOrder(GameObjectOrder.AttackUnit, targetMob);
            return false;
        }
    }
}