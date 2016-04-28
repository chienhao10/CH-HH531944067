using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace KhaZix
{
    public static class Program
    {
        internal const float Wangle = 22*(float) Math.PI/180;

        private static Spell.Skillshot W, E, WE;
        private static Spell.Active R;
        private static Spell.Targeted Q;

        private static Menu Menu;
        internal static bool EvolvedQ, EvolvedW, EvolvedE;
        private static Spell.Targeted ignite;
        internal static List<AIHeroClient> HeroList;
        internal static List<Vector3> EnemyTurretPositions = new List<Vector3>();
        internal static Vector3 NexusPosition;
        internal static Vector3 Jumppoint1, Jumppoint2;
        internal static bool Jumping;

        private static AIHeroClient myHero
        {
            get { return Player.Instance; }
        }

        internal static bool IsInvisible
        {
            get { return myHero.HasBuff("khazixrstealth"); }
        }

        internal static bool Override
        {
            get { return SafetyOverride; }
        }

        internal static bool IsHealthy
        {
            get { return myHero.HealthPercent >= SafetyMinHealth; }
        }

        public static void OnLoad()
        {
            if (myHero.Hero != Champion.Khazix)
            {
                return;
            }

            Menu = MainMenu.AddMenu("Seph KhaZix", "khazix");
            Menu.AddLabel("Ported from Seph's KhaZix - Berb");
            Menu.AddSeparator();

            Menu.AddGroupLabel("Combo");
            Menu.Add("UseQCombo", new CheckBox("Use Q"));
            Menu.Add("UseWCombo", new CheckBox("Use W"));
            Menu.Add("UseECombo", new CheckBox("Use E"));
            Menu.Add("UseRCombo", new CheckBox("Use R"));
            Menu.Add("UseItems", new CheckBox("Use Items"));
            Menu.AddSeparator();
            Menu.Add("UseEGapclose", new CheckBox("Use E To Gapclose for Q"));
            Menu.Add("UseEGapcloseW", new CheckBox("Use E To Gapclose For W"));
            Menu.Add("UseRGapcloseW", new CheckBox("Use R after long gapcloses"));
            Menu.AddSeparator();

            Menu.AddGroupLabel("Harass");
            Menu.Add("UseQHarass", new CheckBox("Use Q"));
            Menu.Add("UseWHarass", new CheckBox("Use W"));
            Menu.Add("Harass.AutoWI", new CheckBox("Auto-W immobile"));
            Menu.Add("Harass.AutoWD", new CheckBox("Auto W"));
            Menu.AddSeparator();

            Menu.AddGroupLabel("Lane Clear");
            Menu.Add("UseQFarm", new CheckBox("Use Q"));
            Menu.Add("UseEFarm", new CheckBox("Use E"));
            Menu.Add("UseWFarm", new CheckBox("Use W"));
            Menu.Add("Farm.WHealth", new Slider("Health % to use W", 80));
            Menu.Add("UseItemsFarm", new CheckBox("Use Items"));
            Menu.AddSeparator();

            Menu.AddGroupLabel("Double Jump");
            Menu.Add("djumpenabled", new CheckBox("Enabled"));
            Menu.Add("delayQ", new Slider("Delay on Q", 1, 1, 5));
            Menu.Add("JEDelay", new Slider("Delay on jumps", 250, 250, 500));
            Menu.Add("jumpmode",
                new Slider("1 : Default (jumps towards your nexus) | 2 : Custom - Settings below", 1, 1, 2));
            Menu.AddSeparator();
            Menu.Add("save", new CheckBox("Save Double Jump Abilities"));
            Menu.Add("noauto", new CheckBox("Wait for Q instead of autos"));
            Menu.Add("jcursor", new CheckBox("Jump to Cursor (true) or false for script logic"));
            Menu.Add("jcursor2", new CheckBox("Second Jump to Cursor (true) or false for script logic"));
            Menu.AddSeparator();

            Menu.AddGroupLabel("Safety");
            Menu.Add("Safety.Enabled", new CheckBox("Enable Safety Checks"));
            Menu.Add("Safety.CountCheck", new CheckBox("Min Ally ratio to Enemies to jump"));
            Menu.Add("Safety.TowerJump", new CheckBox("Avoid Tower Diving"));
            Menu.Add("Safety.Override", new KeyBind("Safety Override Key", false, KeyBind.BindTypes.HoldActive, 'T'));
            Menu.Add("Safety.MinHealth", new Slider("Healthy %", 15));
            Menu.Add("Safety.Ratio", new Slider("Ally:Enemy Ratio (/5)", 1, 0, 5));
            Menu.Add("Safety.noaainult", new CheckBox("No Autos while Stealth", false));
            Menu.AddSeparator();

            Menu.AddGroupLabel("Kill Steal Settings");
            Menu.Add("Kson", new CheckBox("Use KillSteal"));
            Menu.Add("UseQKs", new CheckBox("Use Q"));
            Menu.Add("UseWKs", new CheckBox("Use W"));
            Menu.Add("UseEKs", new CheckBox("Use E"));
            Menu.Add("Ksbypass", new CheckBox("Bypass safety checks for E KS", false));
            Menu.Add("UseEQKs", new CheckBox("Use EQ in KS"));
            Menu.Add("UseEWKs", new CheckBox("Use EW in KS"));
            Menu.Add("UseTiamatKs", new CheckBox("Use items"));
            Menu.Add("Edelay", new Slider("E Delay (ms)", 0, 0, 300));
            Menu.Add("UseIgnite", new CheckBox("Use Ignite"));
            Menu.AddSeparator();

            var ign = Player.Spells.FirstOrDefault(o => o.SData.Name == "summonerdot");
            if (ign != null)
            {
                var igslot = myHero.GetSpellSlotFromName("summonerdot");
                ignite = new Spell.Targeted(igslot, 600);
            }

            Q = new Spell.Targeted(SpellSlot.Q, 325);
            W = new Spell.Skillshot(SpellSlot.W, 1000, SkillShotType.Linear, 225, 828, 80);
            WE = new Spell.Skillshot(SpellSlot.W, 1000, SkillShotType.Linear, 225, 828, 100);
            E = new Spell.Skillshot(SpellSlot.E, 700, SkillShotType.Circular, 250, 1000, 100);
            R = new Spell.Active(SpellSlot.R, 0);

            foreach (var t in ObjectManager.Get<Obj_AI_Turret>().Where(t => t.IsEnemy))
            {
                EnemyTurretPositions.Add(t.ServerPosition);
            }

            var shop = ObjectManager.Get<Obj_Shop>().FirstOrDefault(o => o.IsAlly);
            if (shop != null)
            {
                NexusPosition = shop.Position;
            }

            HeroList = EntityManager.Heroes.AllHeroes;

            Game.OnTick += OnTick;
            Game.OnTick += DoubleJump;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Orbwalker.OnPreAttack += Orbwalker_OnPreAttack;
        }

        internal static bool PointUnderEnemyTurret(this Vector3 Point)
        {
            var EnemyTurrets =
                ObjectManager.Get<Obj_AI_Turret>()
                    .Where(t => t.IsEnemy && Vector3.Distance(t.Position, Point) < 900f + myHero.BoundingRadius);
            return EnemyTurrets.Any();
        }

        internal static bool ShouldJump(Vector3 position)
        {
            if (!SafetyEnabled || Override)
            {
                return true;
            }
            if (SafetyTowerJump && position.PointUnderEnemyTurret())
            {
                return false;
            }
            if (SafetyEnabled)
            {
                if (myHero.HealthPercent < SafetyMinHealth)
                {
                    return false;
                }

                if (SafetyCountCheck)
                {
                    var enemies = position.CountEnemiesInRange(400);
                    var allies = position.CountAlliesInRange(400);

                    var ec = enemies;
                    var ac = allies;
                    float setratio = SafetyRatio/5;


                    if (ec != 0 && !(ac/ec >= setratio))
                    {
                        return false;
                    }
                }
                return true;
            }
            return true;
        }

        private static Vector3 GetJumpPoint(AIHeroClient Qtarget, bool firstjump = true)
        {
            if (myHero.ServerPosition.PointUnderEnemyTurret())
            {
                return (Vector3) myHero.ServerPosition.Extend(NexusPosition, E.Range);
            }

            if (jumpmode == 1)
            {
                return (Vector3) myHero.ServerPosition.Extend(NexusPosition, E.Range);
            }

            if (firstjump && jcursor)
            {
                return Game.CursorPos;
            }

            if (!firstjump && jcursor2)
            {
                return Game.CursorPos;
            }

            var Position = new Vector3();
            var jumptarget = IsHealthy
                ? HeroList.FirstOrDefault(
                    x =>
                        x.IsValidTarget() && !x.IsZombie && x != Qtarget &&
                        Vector3.Distance(myHero.ServerPosition, x.ServerPosition) < E.Range)
                : HeroList.FirstOrDefault(
                    x =>
                        x.IsAlly && !x.IsZombie && !x.IsDead && !x.IsMe &&
                        Vector3.Distance(myHero.ServerPosition, x.ServerPosition) < E.Range);
            if (jumptarget != null)
            {
                Position = jumptarget.ServerPosition;
            }
            if (jumptarget == null)
            {
                return (Vector3) myHero.ServerPosition.Extend(NexusPosition, E.Range);
            }
            return Position;
        }

        private static void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (args.Target.Type == GameObjectType.obj_AI_Base && djumpenabled && noauto)
            {
                if (args.Target.Health < GetQDamage((AIHeroClient) args.Target) && myHero.ManaPercent > 15)
                {
                    args.Process = false;
                }
            }

            if (args.Target.Type == GameObjectType.obj_AI_Base)
            {
                if (Safetynoaainult && IsInvisible)
                {
                    args.Process = false;
                    return;
                }
                if (djumpenabled && noauto)
                {
                    if (args.Target.Health < GetQDamage((AIHeroClient) args.Target) && myHero.ManaPercent > 15)
                    {
                        args.Process = false;
                    }
                }
            }
        }

        internal static void EvolutionCheck()
        {
            if (!EvolvedQ && myHero.HasBuff("khazixqevo"))
            {
                Q = new Spell.Targeted(SpellSlot.Q, 325);
                EvolvedQ = true;
            }
            if (!EvolvedW && myHero.HasBuff("khazixwevo"))
            {
                EvolvedW = true;
                W = new Spell.Skillshot(SpellSlot.W, 1000, SkillShotType.Linear, 225, 828, 100);
            }

            if (!EvolvedE && myHero.HasBuff("khazixeevo"))
            {
                E = new Spell.Skillshot(SpellSlot.E, 1000, SkillShotType.Circular, 250, 1000, 100);
                EvolvedE = true;
            }
        }

        internal static List<AIHeroClient> GetIsolatedTargets()
        {
            var validtargets = HeroList.Where(h => h.IsValidTarget(E.Range) && h.IsIsolated()).ToList();
            return validtargets;
        }

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (!EvolvedE || !save)
            {
                return;
            }

            if (args.Slot.Equals(SpellSlot.Q) && args.Target is AIHeroClient && djumpenabled)
            {
                var target = args.Target as AIHeroClient;
                var qdmg = GetQDamage(target);
                var dmg = myHero.GetAutoAttackDamage(target)*2 + qdmg;
                if (target.Health < dmg && target.Health > qdmg)
                {
                    args.Process = false;
                }
            }
        }

        internal static bool IsIsolated(this Obj_AI_Base target)
        {
            return
                !ObjectManager.Get<Obj_AI_Base>()
                    .Any(
                        x =>
                            x.NetworkId != target.NetworkId && x.Team == target.Team && x.Distance(target) <= 500 &&
                            (x.Type == GameObjectType.obj_AI_Base || x.Type == GameObjectType.obj_AI_Minion ||
                             x.Type == GameObjectType.obj_AI_Turret));
        }

        internal static double GetQDamage(Obj_AI_Base target)
        {
            if (Q.Range < 326)
            {
                return 0.984*
                       myHero.GetSpellDamage(target, SpellSlot.Q,
                           target.IsIsolated() ? DamageLibrary.SpellStages.Empowered : DamageLibrary.SpellStages.Default);
            }
            if (Q.Range > 325)
            {
                var isolated = target.IsIsolated();
                if (isolated)
                {
                    return 0.984*myHero.GetSpellDamage(target, SpellSlot.Q, DamageLibrary.SpellStages.Empowered);
                }
                return myHero.GetSpellDamage(target, SpellSlot.Q);
            }
            return 0;
        }

        private static void DoubleJump(EventArgs args)
        {
            if (!E.IsReady() || !EvolvedE || !djumpenabled || myHero.IsDead || myHero.IsRecalling())
            {
                return;
            }

            if (Q.IsReady() && E.IsReady())
            {
                var Targets = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                if (Targets == null)
                {
                    return;
                }

                var CheckQKillable = Vector3.Distance(myHero.ServerPosition, Targets.ServerPosition) < Q.Range - 25 &&
                                     myHero.GetSpellDamage(Targets, SpellSlot.Q) > Targets.Health;

                if (CheckQKillable)
                {
                    Jumping = true;
                    Jumppoint1 = GetJumpPoint(Targets);
                    Core.DelayAction(delegate { E.Cast(Jumppoint1); }, 0);
                    Core.DelayAction(delegate { Q.Cast(Targets); }, delayQ);
                    Core.DelayAction(delegate
                    {
                        if (E.IsReady())
                        {
                            Jumppoint2 = GetJumpPoint(Targets, false);
                            E.Cast(Jumppoint2);
                        }
                        Jumping = false;
                    }, JEDelay + Game.Ping);
                }
            }
        }

        private static int CountHits(Vector2 position, List<Vector2> points, List<int> hitBoxes)
        {
            var result = 0;

            var startPoint = myHero.ServerPosition.To2D();
            var originalDirection = W.Range*(position - startPoint).Normalized();
            var originalEndPoint = startPoint + originalDirection;

            for (var i = 0; i < points.Count; i++)
            {
                var point = points[i];

                for (var k = 0; k < 3; k++)
                {
                    var endPoint = new Vector2();
                    if (k == 0)
                        endPoint = originalEndPoint;
                    if (k == 1)
                        endPoint = startPoint + originalDirection.Rotated(Wangle);
                    if (k == 2)
                        endPoint = startPoint + originalDirection.Rotated(-Wangle);

                    if (point.Distance(startPoint, endPoint, true, true) <
                        (W.Width + hitBoxes[i])*(W.Width + hitBoxes[i]))
                    {
                        result++;
                        break;
                    }
                }
            }
            return result;
        }

        private static void Combo()
        {
            //var target = TargetSelector.GetTarget(W.Range, DamageType.Physical);

            AIHeroClient target = null;

            if (E.IsReady() && Q.IsReady())
            {
                target = TargetSelector.GetTarget((E.Range + Q.Range) * 0.95f, DamageType.Physical);
            }

            if (target == null)
            {
                target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            }


            if (target != null)
            {
                var dist = myHero.Distance(target);

                // Normal abilities
                if (Q.IsReady() && !Jumping && UseQCombo)
                {
                    if (dist <= Q.Range)
                    {
                        Q.Cast(target);
                    }
                }

                if (W.IsReady() && !EvolvedW && dist <= W.Range && UseWCombo)
                {
                    var pred = W.GetPrediction(target);
                    if (pred.HitChance >= HitChance.High)
                    {
                        W.Cast(pred.CastPosition);
                    }
                }

                if (E.IsReady() && !Jumping && dist <= E.Range && UseECombo && dist > Q.Range + 0.7*myHero.MoveSpeed)
                {
                    var pred = E.GetPrediction(target);
                    if (target.IsValid && !target.IsDead && ShouldJump(pred.CastPosition))
                    {
                        E.Cast(pred.CastPosition);
                    }
                }

                // Use EQ AND EW Synergy
                if ((dist <= E.Range + Q.Range + 0.7*myHero.MoveSpeed && dist > Q.Range && E.IsReady() && UseEGapclose) ||
                    (dist <= E.Range + W.Range && dist > Q.Range && E.IsReady() && W.IsReady() && UseEGapcloseW))
                {
                    var pred = E.GetPrediction(target);
                    if (target.IsValid && !target.IsDead && ShouldJump(pred.CastPosition))
                    {
                        E.Cast(pred.CastPosition);
                    }
                    if (UseRGapcloseW && R.IsReady())
                    {
                        R.Cast();
                    }
                }


                // Ult Usage
                if (R.IsReady() && !Q.IsReady() && !W.IsReady() && !E.IsReady() && UseRCombo)
                {
                    R.Cast();
                }
                // Evolved

                if (W.IsReady() && EvolvedW && dist <= WE.Range && UseWCombo)
                {
                    var pred = WE.GetPrediction(target);
                    if (pred.HitChance >= HitChance.High)
                    {
                        CastWE(target, pred.UnitPosition.To2D(), 0, HitChance.High);
                    }
                    if (pred.HitChance >= HitChance.Collision)
                    {
                        var PCollision = pred.CollisionObjects.ToList();
                        var x = PCollision.FirstOrDefault(PredCollisionChar => PredCollisionChar.Distance(target) <= 30);
                        if (x != null)
                        {
                            W.Cast(x.Position);
                        }
                    }
                }

                if (dist <= E.Range + 0.7*myHero.MoveSpeed && dist > Q.Range && UseECombo && E.IsReady())
                {
                    var pred = E.GetPrediction(target);
                    if (target.IsValid && !target.IsDead && ShouldJump(pred.CastPosition))
                    {
                        E.Cast(pred.CastPosition);
                    }
                }

                if (UseItems_)
                {
                    UseItems(target);
                }
            }
        }

        internal static void CastWE(Obj_AI_Base unit, Vector2 unitPosition, int minTargets = 0,
            HitChance hc = HitChance.Medium)
        {
            var points = new List<Vector2>();
            var hitBoxes = new List<int>();

            var startPoint = myHero.ServerPosition.To2D();
            var originalDirection = W.Range*(unitPosition - startPoint).Normalized();

            foreach (var enemy in EntityManager.Heroes.Enemies)
            {
                if (enemy.IsValidTarget() && enemy.NetworkId != unit.NetworkId)
                {
                    var pos = W.GetPrediction(enemy);
                    if (pos.HitChance >= HitChance.Medium)
                    {
                        points.Add(pos.UnitPosition.To2D());
                        hitBoxes.Add((int) enemy.BoundingRadius + 275);
                    }
                }
            }

            var posiblePositions = new List<Vector2>();

            for (var i = 0; i < 3; i++)
            {
                if (i == 0)
                    posiblePositions.Add(unitPosition + originalDirection.Rotated(0));
                if (i == 1)
                    posiblePositions.Add(startPoint + originalDirection.Rotated(Wangle));
                if (i == 2)
                    posiblePositions.Add(startPoint + originalDirection.Rotated(-Wangle));
            }


            if (startPoint.Distance(unitPosition) < 900)
            {
                for (var i = 0; i < 3; i++)
                {
                    var pos = posiblePositions[i];
                    var direction = (pos - startPoint).Normalized().Perpendicular();
                    var k = 2/3*(unit.BoundingRadius + W.Width);
                    posiblePositions.Add(startPoint - k*direction);
                    posiblePositions.Add(startPoint + k*direction);
                }
            }

            var bestPosition = new Vector2();
            var bestHit = -1;

            foreach (var position in posiblePositions)
            {
                var hits = CountHits(position, points, hitBoxes);
                if (hits > bestHit)
                {
                    bestPosition = position;
                    bestHit = hits;
                }
            }

            if (bestHit <= minTargets)
                return;

            W.Cast(bestPosition.To3D());
        }

        private static void OnTick(EventArgs args)
        {
            if (myHero.IsDead || myHero.IsRecalling())
            {
                return;
            }

            EvolutionCheck();

            if (Kson)
            {
                KillSteal();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }
            else if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
            else if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                     Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                WaveClear();
            }
        }

        private static void WaveClear()
        {
            List<Obj_AI_Minion> allMinions = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsValidTarget(W.Range) && !LeagueSharp.Common.MinionManager.IsWard(x)).ToList();

            if (UseQFarm && Q.IsReady())
            {
                var minion =
                    EntityManager.MinionsAndMonsters.GetLaneMinions().OrderBy(x => x.MaxHealth).FirstOrDefault();
                if (minion != null &&
                    Prediction.Health.GetPrediction(minion, (int) (myHero.Distance(minion)*1000/1400)) >
                    0.35f*myHero.GetSpellDamage(minion, SpellSlot.Q) && myHero.Distance(minion) <= Q.Range)
                {
                    Q.Cast(minion);
                }
                else if (minion == null || !minion.IsValid)
                {
                    foreach (var min in allMinions.Where(x => x.IsValidTarget(Q.Range)))
                    {
                        if (Prediction.Health.GetPrediction(min, (int) (myHero.Distance(min)*1000/1400)) >
                            3*myHero.GetSpellDamage(min, SpellSlot.Q) && myHero.Distance(min) <= Q.Range)
                        {
                            Q.Cast(min);
                            break;
                        }
                    }
                }
            }

            if (UseWFarm && W.IsReady() && myHero.HealthPercent <= FarmWHealth)
            {
                var wmins = EvolvedW
                    ? allMinions.Where(x => x.IsValidTarget(WE.Range))
                    : allMinions.Where(x => x.IsValidTarget(W.Range));
                var farmLocation = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(wmins.ToList(),
                    EvolvedW ? WE.Width : W.Width, EvolvedW ? (int) WE.Range : (int) W.Range);
                var distcheck = EvolvedW
                    ? myHero.Distance(farmLocation.CastPosition) <= WE.Range
                    : myHero.Distance(farmLocation.CastPosition) <= W.Range;
                if (distcheck)
                {
                    W.Cast(farmLocation.CastPosition);
                }
            }

            if (UseEFarm && E.IsReady())
            {
                var farmLocation =
                    EntityManager.MinionsAndMonsters.GetCircularFarmLocation(
                        ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsValidTarget(E.Range)).ToList(), E.Width,
                        (int) E.Range);
                if (myHero.Distance(farmLocation.CastPosition) <= E.Range)
                {
                    E.Cast(farmLocation.CastPosition);
                }
            }


            if (UseItemsFarm)
            {
                var farmLocation =
                    EntityManager.MinionsAndMonsters.GetCircularFarmLocation(
                        ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsValidTarget(400)).ToList(), 400, 400);

                if (Item.CanUseItem(ItemId.Ravenous_Hydra_Melee_Only) &&
                    myHero.Distance(farmLocation.CastPosition) <= 400 && farmLocation.HitNumber >= 2)
                {
                    Item.UseItem(ItemId.Ravenous_Hydra_Melee_Only);
                }
                if (Item.CanUseItem(ItemId.Tiamat_Melee_Only) && myHero.Distance(farmLocation.CastPosition) <= 400 &&
                    farmLocation.HitNumber >= 2)
                {
                    Item.UseItem(ItemId.Tiamat_Melee_Only);
                }
                if (Item.CanUseItem(ItemId.Titanic_Hydra) && myHero.Distance(farmLocation.CastPosition) <= 400 &&
                    farmLocation.HitNumber >= 2)
                {
                    Item.UseItem(ItemId.Titanic_Hydra);
                }
            }
        }

        private static void Harass()
        {
            if (UseQHarass && Q.IsReady())
            {
                var enemy = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                if (enemy.IsValidTarget())
                {
                    Q.Cast(enemy);
                }
            }

            if (UseWHarass && W.IsReady())
            {
                var target = TargetSelector.GetTarget(950, DamageType.Physical);
                var autoWI = HarassAutoWI;
                var autoWD = HarassAutoWD;
                var hitchance = HitChance.High;
                if (target != null && W.IsReady())
                {
                    if (!EvolvedW && myHero.Distance(target) <= W.Range)
                    {
                        var predw = W.GetPrediction(target);
                        if (predw.HitChance == hitchance)
                        {
                            W.Cast(predw.CastPosition);
                        }
                    }
                    else if (EvolvedW && target.IsValidTarget(W.Range + 200))
                    {
                        var pred = W.GetPrediction(target);
                        if ((pred.HitChance == HitChance.Immobile && autoWI) ||
                            (pred.HitChance == HitChance.Dashing && autoWD) || pred.HitChance >= hitchance)
                        {
                            CastWE(target, pred.UnitPosition.To2D());
                        }
                    }
                }
            }
        }

        private static void KillSteal()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);

            if (target != null)
            {
                if (UseIgnite && ignite != null && ignite.IsReady() && ignite.IsInRange(target))
                {
                    double igniteDmg = myHero.GetSummonerSpellDamage(target, DamageLibrary.SummonerSpells.Ignite);
                    if (igniteDmg > target.Health)
                    {
                        ignite.Cast(target);
                        return;
                    }
                }

                if (UseQKs && Q.IsReady() && Vector3.Distance(myHero.ServerPosition, target.ServerPosition) <= Q.Range)
                {
                    var QDmg = GetQDamage(target);
                    if (!Jumping && target.Health <= QDmg)
                    {
                        Q.Cast(target);
                        return;
                    }
                }

                if (UseEKs && E.IsReady() && !Jumping && Vector3.Distance(myHero.ServerPosition, target.ServerPosition) <= E.Range &&
                    Vector3.Distance(myHero.ServerPosition, target.ServerPosition) > Q.Range)
                {
                    double EDmg = myHero.GetSpellDamage(target, SpellSlot.E);
                    if (!Jumping && target.Health < EDmg)
                    {
                        Core.DelayAction(
                            delegate
                            {
                                var pred = E.GetPrediction(target);
                                if (target.IsValid && !target.IsDead)
                                {
                                    if (Ksbypass || ShouldJump(pred.CastPosition))
                                    {
                                        E.Cast(pred.CastPosition);
                                    }
                                }
                            },
                            Game.Ping + Edelay);
                    }
                }

                if (W.IsReady() && !EvolvedW &&
                    Vector3.Distance(myHero.ServerPosition, target.ServerPosition) <= W.Range && UseWKs)
                {
                    double WDmg = myHero.GetSpellDamage(target, SpellSlot.W);
                    if (target.Health <= WDmg)
                    {
                        var pred = W.GetPrediction(target);
                        if (pred.HitChance >= HitChance.Medium)
                        {
                            W.Cast(pred.CastPosition);
                            return;
                        }
                    }
                }

                if (W.IsReady() && EvolvedW && Vector3.Distance(myHero.ServerPosition, target.ServerPosition) <= W.Range &&
                    UseWKs)
                {
                    double WDmg = myHero.GetSpellDamage(target, SpellSlot.W);
                    var pred = W.GetPrediction(target);
                    if (target.Health <= WDmg && pred.HitChance > HitChance.Medium)
                    {
                        CastWE(target, pred.UnitPosition.To2D());
                        return;
                    }

                    if (pred.HitChance >= HitChance.Collision)
                    {
                        var PCollision = pred.CollisionObjects.ToList();
                        var x =
                            PCollision.FirstOrDefault(
                                PredCollisionChar =>
                                    Vector3.Distance(PredCollisionChar.ServerPosition, target.ServerPosition) <= 30);
                        if (x != null)
                        {
                            W.Cast(x.Position);
                            return;
                        }
                    }
                }


                // Mixed's EQ KS
                if (Q.IsReady() && E.IsReady() &&
                    Vector3.Distance(myHero.ServerPosition, target.ServerPosition) <= E.Range + Q.Range && UseEQKs)
                {
                    var QDmg = GetQDamage(target);
                    double EDmg = myHero.GetSpellDamage(target, SpellSlot.E);
                    if (target.Health <= QDmg + EDmg)
                    {
                        Core.DelayAction(delegate
                        {
                            var pred = E.GetPrediction(target);
                            if (target.IsValidTarget() && !target.IsZombie && ShouldJump(pred.CastPosition))
                            {
                                if (Ksbypass || ShouldJump(pred.CastPosition))
                                {
                                    E.Cast(pred.CastPosition);
                                }
                            }
                        }, Edelay);
                    }
                }

                // MIXED EW KS
                if (W.IsReady() && E.IsReady() && !EvolvedW &&
                    Vector3.Distance(myHero.ServerPosition, target.ServerPosition) <= W.Range + E.Range && UseEWKs)
                {
                    double WDmg = myHero.GetSpellDamage(target, SpellSlot.W);
                    if (target.Health <= WDmg)
                    {
                        Core.DelayAction(delegate
                        {
                            var pred = E.GetPrediction(target);
                            if (target.IsValid && !target.IsDead && ShouldJump(pred.CastPosition))
                            {
                                if (Ksbypass || ShouldJump(pred.CastPosition))
                                {
                                    E.Cast(pred.CastPosition);
                                }
                            }
                        }, Edelay);
                    }
                }

                if (Item.CanUseItem(ItemId.Tiamat_Melee_Only) &&
                    Vector2.Distance(myHero.ServerPosition.To2D(), target.ServerPosition.To2D()) <= 400 && UseTiamatKs)
                {
                    double Tiamatdmg = myHero.GetItemDamage(target, ItemId.Tiamat_Melee_Only);
                    if (target.Health <= Tiamatdmg)
                    {
                        Item.UseItem(ItemId.Tiamat_Melee_Only);
                        return;
                    }
                }
                if (Item.CanUseItem(ItemId.Ravenous_Hydra_Melee_Only) &&
                    Vector2.Distance(myHero.ServerPosition.To2D(), target.ServerPosition.To2D()) <= 400 && UseTiamatKs)
                {
                    double hydradmg = myHero.GetItemDamage(target, ItemId.Ravenous_Hydra_Melee_Only);
                    if (target.Health <= hydradmg)
                    {
                        Item.UseItem(ItemId.Ravenous_Hydra_Melee_Only);
                    }
                }
            }
        }

        internal static void UseItems(Obj_AI_Base target)
        {

            if (Item.CanUseItem(ItemId.Ravenous_Hydra_Melee_Only) && 400 > myHero.Distance(target))
            {
                Item.UseItem(ItemId.Ravenous_Hydra_Melee_Only);
            }
            if (Item.CanUseItem(ItemId.Tiamat_Melee_Only) && 400 > myHero.Distance(target))
            {
                Item.UseItem(ItemId.Tiamat_Melee_Only);
            }
            if (Item.CanUseItem(ItemId.Titanic_Hydra) && 400 > myHero.Distance(target))
            {
                Item.UseItem(ItemId.Titanic_Hydra);
            }
            if (Item.CanUseItem(ItemId.Blade_of_the_Ruined_King) && 550 > myHero.Distance(target))
            {
                Item.UseItem(ItemId.Blade_of_the_Ruined_King);
            }
            if (Item.CanUseItem(ItemId.Youmuus_Ghostblade) && myHero.GetAutoAttackRange() > myHero.Distance(target))
            {
                Item.UseItem(ItemId.Youmuus_Ghostblade);
            }
            if (Item.CanUseItem(ItemId.Bilgewater_Cutlass) && 550 > myHero.Distance(target))
            {
                Item.UseItem(ItemId.Bilgewater_Cutlass);
            }
        }

        #region Menu Items

        public static bool UseQCombo
        {
            get { return Menu["UseQCombo"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool UseWCombo
        {
            get { return Menu["UseWCombo"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool UseECombo
        {
            get { return Menu["UseECombo"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool UseRCombo
        {
            get { return Menu["UseRCombo"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool UseEGapclose
        {
            get { return Menu["UseEGapclose"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool UseEGapcloseW
        {
            get { return Menu["UseEGapcloseW"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool UseRGapcloseW
        {
            get { return Menu["UseRGapcloseW"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool djumpenabled
        {
            get { return Menu["djumpenabled"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool save
        {
            get { return Menu["save"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool noauto
        {
            get { return Menu["noauto"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool jcursor
        {
            get { return Menu["jcursor"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool jcursor2
        {
            get { return Menu["jcursor2"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool SafetyEnabled
        {
            get { return Menu["Safety.Enabled"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool SafetyCountCheck
        {
            get { return Menu["Safety.CountCheck"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool SafetyTowerJump
        {
            get { return Menu["Safety.TowerJump"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool UseItems_
        {
            get { return Menu["UseItems"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool Kson
        {
            get { return Menu["Kson"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool UseQKs
        {
            get { return Menu["UseQKs"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool UseWKs
        {
            get { return Menu["UseWKs"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool UseEKs
        {
            get { return Menu["UseEKs"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool Ksbypass
        {
            get { return Menu["Ksbypass"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool UseEQKs
        {
            get { return Menu["UseEQKs"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool UseEWKs
        {
            get { return Menu["UseEWKs"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool UseTiamatKs
        {
            get { return Menu["UseTiamatKs"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool UseIgnite
        {
            get { return Menu["UseIgnite"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool UseQHarass
        {
            get { return Menu["UseQHarass"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool UseWHarass
        {
            get { return Menu["UseWHarass"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool HarassAutoWI
        {
            get { return Menu["Harass.AutoWI"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool HarassAutoWD
        {
            get { return Menu["Harass.AutoWD"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool UseQFarm
        {
            get { return Menu["UseQFarm"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool UseEFarm
        {
            get { return Menu["UseEFarm"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool UseWFarm
        {
            get { return Menu["UseWFarm"].Cast<CheckBox>().CurrentValue; }
        }

        public static bool UseItemsFarm
        {
            get { return Menu["UseItemsFarm"].Cast<CheckBox>().CurrentValue; }
        }

        public static int JEDelay
        {
            get { return Menu["JEDelay"].Cast<Slider>().CurrentValue; }
        }

        public static int delayQ
        {
            get { return Menu["delayQ"].Cast<Slider>().CurrentValue; }
        }

        public static int jumpmode
        {
            get { return Menu["jumpmode"].Cast<Slider>().CurrentValue; }
        }

        public static int SafetyMinHealth
        {
            get { return Menu["Safety.MinHealth"].Cast<Slider>().CurrentValue; }
        }

        public static int SafetyRatio
        {
            get { return Menu["Safety.Ratio"].Cast<Slider>().CurrentValue; }
        }

        public static int Edelay
        {
            get { return Menu["Edelay"].Cast<Slider>().CurrentValue; }
        }

        public static int FarmWHealth
        {
            get { return Menu["Farm.WHealth"].Cast<Slider>().CurrentValue; }
        }

        public static bool SafetyOverride
        {
            get { return Menu["Safety.Override"].Cast<KeyBind>().CurrentValue; }
        }

        public static bool Safetynoaainult
        {
            get { return Menu["Safety.noaainult"].Cast<CheckBox>().CurrentValue; }
        }

        //Safety.noaainult

        #endregion
    }
}