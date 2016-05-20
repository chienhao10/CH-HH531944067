using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace SephKhazix
{
    class Khazix : Helper
    {
        internal Menu menu, harass, combo, farm, ks, safety, djump, draw, debug;

        public Khazix()
        {
            if (ObjectManager.Player.ChampionName != "Khazix")
            {
                return;
            }

            Init();
            GenerateMenu();
            Game.OnUpdate += OnUpdate;
            Game.OnUpdate += DoubleJump;
            Drawing.OnDraw += OnDraw;
            Spellbook.OnCastSpell += SpellCast;
            Orbwalker.OnPreAttack += BeforeAttack;

            menu = KhazixMenu.menu;
            harass = KhazixMenu.harass;
            combo = KhazixMenu.combo;
            farm = KhazixMenu.farm;
            ks = KhazixMenu.ks;
            safety = KhazixMenu.safety;
            djump = KhazixMenu.djump;
            draw = KhazixMenu.draw;
            debug = KhazixMenu.debug;
        }

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        void Init()
        {
            InitSkills();
            Khazix = ObjectManager.Player;

            foreach (var t in ObjectManager.Get<Obj_AI_Turret>().Where(t => t.IsEnemy))
            {
                EnemyTurretPositions.Add(t.ServerPosition);
            }

            var shop = ObjectManager.Get<Obj_Shop>().FirstOrDefault(o => o.IsAlly);
            if (shop != null)
            {
                NexusPosition = shop.Position;
            }

            HeroList = HeroManager.AllHeroes;
        }


        void OnUpdate(EventArgs args)
        {
            if (Khazix.IsDead || Khazix.IsRecalling())
            {
                return;
            }

            EvolutionCheck();

            if (getCheckBoxItem(ks, "Kson"))
            {
                KillSteal();
            }

            if (getKeyBindItem(harass, "Harass.Key"))
            {
                Harass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Mixed();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Waveclear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                LH();
            }
        }


        void Mixed()
        {
            if (getCheckBoxItem(harass, "Harass.InMixed"))
            {
                Harass();
            }
            LH();
        }

        void Harass()
        {
            if (getCheckBoxItem(harass, "UseQHarass") && Q.IsReady())
            {
                var enemy = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                if (enemy.IsValidEnemy())
                {
                    Q.Cast(enemy);
                }
            }
            if (getCheckBoxItem(harass, "UseWHarass") && W.IsReady())
            {
                AIHeroClient target = TargetSelector.GetTarget(950, DamageType.Physical);
                var autoWI = getCheckBoxItem(harass, "Harass.AutoWI");
                var autoWD = getCheckBoxItem(harass, "Harass.AutoWD");
                var hitchance = HarassHitChance(Config);
                if (target != null && W.IsReady())
                {
                    if (!EvolvedW && Khazix.LSDistance(target) <= W.Range)
                    {
                        PredictionOutput predw = W.GetPrediction(target);
                        if (predw.Hitchance == hitchance)
                        {
                            W.Cast(predw.CastPosition);
                        }
                    }
                    else if (EvolvedW && target.IsValidTarget(W.Range))
                    {
                        PredictionOutput pred = WE.GetPrediction(target);
                        if ((pred.Hitchance == HitChance.Immobile && autoWI) || (pred.Hitchance == HitChance.Dashing && autoWD) || pred.Hitchance >= hitchance)
                        {
                            CastWE(target, pred.UnitPosition.To2D(), 0, hitchance);
                        }
                    }
                }
            }
        }


        void LH()
        {
            List<Obj_AI_Base> allMinions = MinionManager.GetMinions(Khazix.ServerPosition, Q.Range);
            if (getCheckBoxItem(farm, "UseQFarm") && Q.IsReady())
            {
                foreach (Obj_AI_Base minion in
                    allMinions.Where(
                        minion =>
                            minion.IsValidTarget() &&
                            HealthPrediction.GetHealthPrediction(
                                minion, (int)(Khazix.LSDistance(minion) * 1000 / 1400)) <
                            0.75 * Khazix.GetSpellDamage(minion, SpellSlot.Q)))
                {
                    if (Vector3.Distance(minion.ServerPosition, Khazix.ServerPosition) >
                        Orbwalking.GetRealAutoAttackRange(Khazix) && Khazix.LSDistance(minion) <= Q.Range)
                    {
                        Q.CastOnUnit(minion);
                        return;
                    }
                }

            }
            if (getCheckBoxItem(farm, "UseWFarm") && W.IsReady())
            {
                MinionManager.FarmLocation farmLocation = MinionManager.GetBestCircularFarmLocation(
                  MinionManager.GetMinions(Khazix.ServerPosition, W.Range).Where(minion => HealthPrediction.GetHealthPrediction(
                                minion, (int)(Khazix.LSDistance(minion) * 1000 / 1400)) <
                            0.75 * Khazix.GetSpellDamage(minion, SpellSlot.W))
                      .Select(minion => minion.ServerPosition.To2D())
                      .ToList(), W.Width, W.Range);
                if (farmLocation.MinionsHit >= 1)
                {
                    if (!EvolvedW)
                    {
                        if (Khazix.LSDistance(farmLocation.Position) <= W.Range)
                        {
                            W.Cast(farmLocation.Position);
                        }
                    }

                    if (EvolvedW)
                    {
                        if (Khazix.LSDistance(farmLocation.Position) <= W.Range)
                        {
                            W.Cast(farmLocation.Position);
                        }
                    }
                }
            }

            if (getCheckBoxItem(farm, "UseEFarm") && E.IsReady())
            {

                MinionManager.FarmLocation farmLocation =
                    MinionManager.GetBestCircularFarmLocation(
                        MinionManager.GetMinions(Khazix.ServerPosition, E.Range).Where(minion => HealthPrediction.GetHealthPrediction(
                                minion, (int)(Khazix.LSDistance(minion) * 1000 / 1400)) <
                            0.75 * Khazix.GetSpellDamage(minion, SpellSlot.W))
                            .Select(minion => minion.ServerPosition.To2D())
                            .ToList(), E.Width, E.Range);

                if (farmLocation.MinionsHit >= 1)
                {
                    if (Khazix.LSDistance(farmLocation.Position) <= E.Range)
                        E.Cast(farmLocation.Position);
                }
            }


            if (getCheckBoxItem(farm, "UseItemsFarm"))
            {
                MinionManager.FarmLocation farmLocation =
                    MinionManager.GetBestCircularFarmLocation(
                        MinionManager.GetMinions(Khazix.ServerPosition, Hydra.Range)
                            .Select(minion => minion.ServerPosition.To2D())
                            .ToList(), Hydra.Range, Hydra.Range);

                if (Hydra.IsReady() && Khazix.LSDistance(farmLocation.Position) <= Hydra.Range && farmLocation.MinionsHit >= 2)
                {
                    Items.UseItem(3074, Khazix);
                }
                if (Tiamat.IsReady() && Khazix.LSDistance(farmLocation.Position) <= Tiamat.Range && farmLocation.MinionsHit >= 2)
                {
                    Items.UseItem(3077, Khazix);
                }
            }
        }

        void Waveclear()
        {
            List<Obj_AI_Minion> allMinions = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsValidTarget(W.Range) && !MinionManager.IsWard(x)).ToList();

            if (getCheckBoxItem(farm, "UseQFarm") && Q.IsReady())
            {
                var minion = Orbwalker.LastTarget as Obj_AI_Minion;
                if (minion != null && HealthPrediction.GetHealthPrediction(
                                minion, (int)(Khazix.LSDistance(minion) * 1000 / 1400)) >
                            0.35f * Khazix.GetSpellDamage(minion, SpellSlot.Q) && Khazix.LSDistance(minion) <= Q.Range)
                {
                    Q.Cast(minion);
                }
                else if (minion == null || !minion.IsValid)
                {
                    foreach (var min in allMinions.Where(x => x.IsValidTarget(Q.Range)))
                    {
                        if (HealthPrediction.GetHealthPrediction(
                                min, (int)(Khazix.LSDistance(min) * 1000 / 1400)) >
                            3 * Khazix.GetSpellDamage(min, SpellSlot.Q) && Khazix.LSDistance(min) <= Q.Range)
                        {
                            Q.Cast(min);
                            break;
                        }
                    }
                }
            }

            if (getCheckBoxItem(farm, "UseWFarm") && W.IsReady() && Khazix.HealthPercent <= getSliderItem(farm, "Farm.WHealth"))
            {
                var wmins = EvolvedW ? allMinions.Where(x => x.IsValidTarget(WE.Range)) : allMinions.Where(x => x.IsValidTarget(W.Range));
                MinionManager.FarmLocation farmLocation = MinionManager.GetBestCircularFarmLocation(wmins
                      .Select(minion => minion.ServerPosition.To2D())
                      .ToList(), EvolvedW ? WE.Width : W.Width, EvolvedW ? WE.Range : W.Range);
                var distcheck = EvolvedW ? Khazix.LSDistance(farmLocation.Position) <= WE.Range : Khazix.LSDistance(farmLocation.Position) <= W.Range;
                if (distcheck)
                {
                    W.Cast(farmLocation.Position);
                }
            }

            if (getCheckBoxItem(farm, "UseEFarm") && E.IsReady())
            {
                MinionManager.FarmLocation farmLocation =
                    MinionManager.GetBestCircularFarmLocation(
                        MinionManager.GetMinions(Khazix.ServerPosition, E.Range)
                            .Select(minion => minion.ServerPosition.To2D())
                            .ToList(), E.Width, E.Range);
                if (Khazix.LSDistance(farmLocation.Position) <= E.Range)
                {
                    E.Cast(farmLocation.Position);
                }
            }


            if (getCheckBoxItem(farm, "UseItemsFarm"))
            {
                MinionManager.FarmLocation farmLocation =
                    MinionManager.GetBestCircularFarmLocation(
                        MinionManager.GetMinions(Khazix.ServerPosition, Hydra.Range)
                            .Select(minion => minion.ServerPosition.To2D())
                            .ToList(), Hydra.Range, Hydra.Range);

                if (Hydra.IsReady() && Khazix.LSDistance(farmLocation.Position) <= Hydra.Range && farmLocation.MinionsHit >= 2)
                {
                    Items.UseItem(3074, Khazix);
                }
                if (Tiamat.IsReady() && Khazix.LSDistance(farmLocation.Position) <= Tiamat.Range && farmLocation.MinionsHit >= 2)
                {
                    Items.UseItem(3077, Khazix);
                }
                if (Titanic.IsReady() && Khazix.LSDistance(farmLocation.Position) <= Titanic.Range && farmLocation.MinionsHit >= 2)
                {
                    Items.UseItem(3748, Khazix);
                }
            }
        }


        void Combo()
        {
            AIHeroClient target = null;

            if (SpellSlot.E.IsReady() && SpellSlot.Q.IsReady())
            {
                target = TargetSelector.GetTarget((E.Range + Q.Range) * 0.95f, DamageType.Physical);
            }

            if (target == null)
            {
                target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            }

            if ((target != null))
            {
                var dist = Khazix.LSDistance(target);

                // Normal abilities

                if (Q.IsReady() && !Jumping && getCheckBoxItem(combo, "UseQCombo"))
                {
                    if (dist <= Q.Range)
                    {
                        Q.Cast(target);
                    }
                }

                if (W.IsReady() && !EvolvedW && dist <= W.Range && getCheckBoxItem(combo, "UseWCombo"))
                {
                    var pred = W.GetPrediction(target);
                    if (pred.Hitchance >= Config.GetHitChance("WHitchance"))
                    {
                        W.Cast(pred.CastPosition);
                    }
                }

                if (E.IsReady() && !Jumping && dist <= E.Range && getCheckBoxItem(combo, "UseECombo") && dist > Q.Range + (0.7 * Khazix.MoveSpeed))
                {
                    PredictionOutput pred = E.GetPrediction(target);
                    if (target.IsValid && !target.IsDead && ShouldJump(pred.CastPosition))
                    {
                        E.Cast(pred.CastPosition);
                    }
                }

                // Use EQ AND EW Synergy
                if ((dist <= E.Range + Q.Range + (0.7 * Khazix.MoveSpeed) && dist > Q.Range && E.IsReady() &&
                    getCheckBoxItem(combo, "UseEGapclose")) || (dist <= E.Range + W.Range && dist > Q.Range && E.IsReady() && W.IsReady() &&
                    getCheckBoxItem(combo, "UseEGapcloseW")))
                {
                    PredictionOutput pred = E.GetPrediction(target);
                    if (target.IsValid && !target.IsDead && ShouldJump(pred.CastPosition))
                    {
                        E.Cast(pred.CastPosition);
                    }
                    if (getCheckBoxItem(combo, "UseRGapcloseW") && R.IsReady())
                    {
                        R.CastOnUnit(Khazix);
                    }
                }


                // Ult Usage
                if (R.IsReady() && !Q.IsReady() && !W.IsReady() && !E.IsReady() &&
                    getCheckBoxItem(combo, "UseRCombo"))
                {
                    R.Cast();
                }
                // Evolved

                if (W.IsReady() && EvolvedW && dist <= WE.Range && getCheckBoxItem(combo, "UseWCombo"))
                {
                    PredictionOutput pred = WE.GetPrediction(target);
                    if (pred.Hitchance >= Config.GetHitChance("WHitchance"))
                    {
                        CastWE(target, pred.UnitPosition.To2D(), 0, Config.GetHitChance("WHitchance"));
                    }
                    if (pred.Hitchance >= HitChance.Collision)
                    {
                        List<Obj_AI_Base> PCollision = pred.CollisionObjects;
                        var x = PCollision.Where(PredCollisionChar => PredCollisionChar.LSDistance(target) <= 30).FirstOrDefault();
                        if (x != null)
                        {
                            W.Cast(x.Position);
                        }
                    }
                }

                if (dist <= E.Range + (0.7 * Khazix.MoveSpeed) && dist > Q.Range &&
                    getCheckBoxItem(combo, "UseECombo") && E.IsReady())
                {
                    PredictionOutput pred = E.GetPrediction(target);
                    if (target.IsValid && !target.IsDead && ShouldJump(pred.CastPosition))
                    {
                        E.Cast(pred.CastPosition);
                    }
                }

                if (getCheckBoxItem(combo, "UseItems"))
                {
                    UseItems(target);
                }
            }
        }


        void KillSteal()
        {
            AIHeroClient target = HeroList
                .Where(x => x.IsValidTarget() && x.LSDistance(Khazix.Position) < 1375f && !x.IsZombie)
                .MinOrDefault(x => x.Health);

            if (target != null && target.IsInRange(600))
            {

                if (getCheckBoxItem(safety, "Safety.autoescape") && !IsHealthy)
                {
                    var ally =
                        HeroList.FirstOrDefault(h => h.HealthPercent > 40 && h.CountEnemiesInRange(400) == 0 && !h.ServerPosition.PointUnderEnemyTurret());
                    if (ally != null && ally.IsValid)
                    {
                        E.Cast(ally.ServerPosition);
                        return;
                    }
                    var objAiturret = EnemyTurretPositions.Where(x => Vector3.Distance(Khazix.ServerPosition, x) <= 900f);
                    if (objAiturret.Any() || Khazix.CountEnemiesInRange(500) >= 1)
                    {
                        var bestposition = Khazix.ServerPosition.LSExtend(NexusPosition, E.Range);
                        E.Cast(bestposition);
                        return;
                    }
                }

                if (getCheckBoxItem(ks, "UseQKs") && Q.IsReady() &&
                    Vector3.Distance(Khazix.ServerPosition, target.ServerPosition) <= Q.Range)
                {
                    double QDmg = GetQDamage(target);
                    if (!Jumping && target.Health <= QDmg)
                    {
                        Q.Cast(target);
                        return;
                    }
                }

                if (getCheckBoxItem(ks, "UseEKs") && E.IsReady() && !Jumping &&
                    Vector3.Distance(Khazix.ServerPosition, target.ServerPosition) <= E.Range && Vector3.Distance(Khazix.ServerPosition, target.ServerPosition) > Q.Range)
                {
                    double EDmg = Khazix.GetSpellDamage(target, SpellSlot.E);
                    if (!Jumping && target.Health < EDmg)
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(
                            Game.Ping + getSliderItem(ks, "Edelay"), delegate
                            {
                                PredictionOutput pred = E.GetPrediction(target);
                                if (target.IsValid && !target.IsDead)
                                {
                                    if (getCheckBoxItem(ks, "Ksbypass") || ShouldJump(pred.CastPosition))
                                    {
                                        E.Cast(pred.CastPosition);
                                    }
                                }
                            });
                    }
                }

                if (W.IsReady() && !EvolvedW && Vector3.Distance(Khazix.ServerPosition, target.ServerPosition) <= W.Range &&
                    getCheckBoxItem(ks, "UseWKs"))
                {
                    double WDmg = Khazix.GetSpellDamage(target, SpellSlot.W);
                    if (target.Health <= WDmg)
                    {
                        var pred = W.GetPrediction(target);
                        if (pred.Hitchance >= HitChance.Medium)
                        {
                            W.Cast(pred.CastPosition);
                            return;
                        }
                    }
                }

                if (W.IsReady() && EvolvedW &&
                        Vector3.Distance(Khazix.ServerPosition, target.ServerPosition) <= W.Range &&
                        getCheckBoxItem(ks, "UseWKs"))
                {
                    double WDmg = Khazix.GetSpellDamage(target, SpellSlot.W);
                    PredictionOutput pred = WE.GetPrediction(target);
                    if (target.Health <= WDmg && pred.Hitchance >= HitChance.Medium)
                    {
                        CastWE(target, pred.UnitPosition.To2D(), 0, Config.GetHitChance("WHitchance"));
                        return;
                    }

                    if (pred.Hitchance >= HitChance.Collision)
                    {
                        List<Obj_AI_Base> PCollision = pred.CollisionObjects;
                        var x =
                            PCollision
                                .FirstOrDefault(PredCollisionChar => Vector3.Distance(PredCollisionChar.ServerPosition, target.ServerPosition) <= 30);
                        if (x != null)
                        {
                            W.Cast(x.Position);
                            return;
                        }
                    }
                }


                // Mixed's EQ KS
                if (Q.IsReady() && E.IsReady() &&
                    Vector3.Distance(Khazix.ServerPosition, target.ServerPosition) <= E.Range + Q.Range
                    && getCheckBoxItem(ks, "UseEQKs"))
                {
                    double QDmg = GetQDamage(target);
                    double EDmg = Khazix.GetSpellDamage(target, SpellSlot.E);
                    if ((target.Health <= QDmg + EDmg))
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(getSliderItem(ks, "Edelay"), delegate
                        {
                            PredictionOutput pred = E.GetPrediction(target);
                            if (target.IsValidTarget() && !target.IsZombie && ShouldJump(pred.CastPosition))
                            {
                                if (getCheckBoxItem(ks, "Ksbypass") || ShouldJump(pred.CastPosition))
                                {
                                    E.Cast(pred.CastPosition);
                                }
                            }
                        });
                    }
                }

                // MIXED EW KS
                if (W.IsReady() && E.IsReady() && !EvolvedW &&
                    Vector3.Distance(Khazix.ServerPosition, target.ServerPosition) <= W.Range + E.Range
                    && getCheckBoxItem(ks, "UseEWKs"))
                {
                    double WDmg = Khazix.GetSpellDamage(target, SpellSlot.W);
                    if (target.Health <= WDmg)
                    {

                        LeagueSharp.Common.Utility.DelayAction.Add(getSliderItem(ks, "Edelay"), delegate
                        {
                            PredictionOutput pred = E.GetPrediction(target);
                            if (target.IsValid && !target.IsDead && ShouldJump(pred.CastPosition))
                            {
                                if (getCheckBoxItem(ks, "Ksbypass") || ShouldJump(pred.CastPosition))
                                {
                                    E.Cast(pred.CastPosition);
                                }
                            }
                        });
                    }
                }

                if (Tiamat.IsReady() &&
                    Vector2.Distance(Khazix.ServerPosition.To2D(), target.ServerPosition.To2D()) <= Tiamat.Range &&
                    getCheckBoxItem(ks, "UseTiamatKs"))
                {
                    double Tiamatdmg = Khazix.GetItemDamage(target, LeagueSharp.Common.Damage.DamageItems.Tiamat);
                    if (target.Health <= Tiamatdmg)
                    {
                        Tiamat.Cast();
                        return;
                    }
                }
                if (Hydra.IsReady() &&
                    Vector2.Distance(Khazix.ServerPosition.To2D(), target.ServerPosition.To2D()) <= Hydra.Range &&
                    getCheckBoxItem(ks, "UseTiamatKs"))
                {
                    double hydradmg = Khazix.GetItemDamage(target, LeagueSharp.Common.Damage.DamageItems.Hydra);
                    if (target.Health <= hydradmg)
                    {
                        Hydra.Cast();
                    }
                }
            }
        }

        internal bool ShouldJump(Vector3 position)
        {
            if (!getCheckBoxItem(safety, "Safety.Enabled") || Override)
            {
                return true;
            }
            if (getCheckBoxItem(safety, "Safety.TowerJump") && position.PointUnderEnemyTurret())
            {
                return false;
            }
            else if (getCheckBoxItem(safety, "Safety.Enabled"))
            {
                if (Khazix.HealthPercent < getSliderItem(safety, "Safety.MinHealth"))
                {
                    return false;
                }

                if (getCheckBoxItem(safety, "Safety.CountCheck"))
                {
                    var enemies = position.GetEnemiesInRange(400);
                    var allies = position.GetAlliesInRange(400);

                    var ec = enemies.Count;
                    var ac = allies.Count;
                    float setratio = getSliderItem(safety, "Safety.Ratio") / 5;


                    if (ec != 0 && !(ac / ec >= setratio))
                    {
                        return false;
                    }
                }
                return true;
            }
            return true;
        }



        internal void CastWE(Obj_AI_Base unit, Vector2 unitPosition, int minTargets = 0, HitChance hc = HitChance.Medium)
        {
            var points = new List<Vector2>();
            var hitBoxes = new List<int>();

            Vector2 startPoint = Khazix.ServerPosition.To2D();
            Vector2 originalDirection = W.Range * (unitPosition - startPoint).Normalized();

            foreach (AIHeroClient enemy in HeroManager.Enemies)
            {
                if (enemy.IsValidTarget() && enemy.NetworkId != unit.NetworkId)
                {
                    PredictionOutput pos = WE.GetPrediction(enemy);
                    if (pos.Hitchance >= hc)
                    {
                        points.Add(pos.UnitPosition.To2D());
                        hitBoxes.Add((int)enemy.BoundingRadius + 275);
                    }
                }
            }

            var posiblePositions = new List<Vector2>();

            for (int i = 0; i < 3; i++)
            {
                if (i == 0)
                    posiblePositions.Add(unitPosition + originalDirection.Rotated(0));
                if (i == 1)
                    posiblePositions.Add(startPoint + originalDirection.Rotated(Wangle));
                if (i == 2)
                    posiblePositions.Add(startPoint + originalDirection.Rotated(-Wangle));
            }


            if (startPoint.LSDistance(unitPosition) < 900)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 pos = posiblePositions[i];
                    Vector2 direction = (pos - startPoint).Normalized().Perpendicular();
                    float k = (2 / 3 * (unit.BoundingRadius + W.Width));
                    posiblePositions.Add(startPoint - k * direction);
                    posiblePositions.Add(startPoint + k * direction);
                }
            }

            var bestPosition = new Vector2();
            int bestHit = -1;

            foreach (Vector2 position in posiblePositions)
            {
                int hits = CountHits(position, points, hitBoxes);
                if (hits > bestHit)
                {
                    bestPosition = position;
                    bestHit = hits;
                }
            }

            if (bestHit + 1 <= minTargets)
                return;

            W.Cast(bestPosition.To3D(), false);
        }

        int CountHits(Vector2 position, List<Vector2> points, List<int> hitBoxes)
        {
            int result = 0;

            Vector2 startPoint = Khazix.ServerPosition.To2D();
            Vector2 originalDirection = W.Range * (position - startPoint).Normalized();
            Vector2 originalEndPoint = startPoint + originalDirection;

            for (int i = 0; i < points.Count; i++)
            {
                Vector2 point = points[i];

                for (int k = 0; k < 3; k++)
                {
                    var endPoint = new Vector2();
                    if (k == 0)
                        endPoint = originalEndPoint;
                    if (k == 1)
                        endPoint = startPoint + originalDirection.Rotated(Wangle);
                    if (k == 2)
                        endPoint = startPoint + originalDirection.Rotated(-Wangle);

                    if (point.LSDistance(startPoint, endPoint, true, true) <
                        (W.Width + hitBoxes[i]) * (W.Width + hitBoxes[i]))
                    {
                        result++;
                        break;
                    }
                }
            }
            return result;
        }


        void DoubleJump(EventArgs args)
        {
            if (!E.IsReady() || !EvolvedE || !getCheckBoxItem(djump, "djumpenabled") || Khazix.IsDead || Khazix.IsRecalling())
            {
                return;
            }

            var Targets = HeroList.Where(x => x.IsValidTarget() && !x.IsInvulnerable && !x.IsZombie);

            if (Q.IsReady() && E.IsReady())
            {
                var CheckQKillable = Targets.FirstOrDefault(x => Vector3.Distance(Khazix.ServerPosition, x.ServerPosition) < Q.Range - 25 && GetQDamage(x) > x.Health);

                if (CheckQKillable != null)
                {
                    Jumping = true;
                    Jumppoint1 = GetJumpPoint(CheckQKillable);
                    E.Cast(Jumppoint1);
                    Q.Cast(CheckQKillable);
                    var oldpos = Khazix.ServerPosition;
                    LeagueSharp.Common.Utility.DelayAction.Add(getSliderItem(djump, "JEDelay") + Game.Ping, () =>
                    {
                        if (E.IsReady())
                        {
                            Jumppoint2 = GetJumpPoint(CheckQKillable, false);
                            E.Cast(Jumppoint2);
                        }
                        Jumping = false;
                    });
                }
            }
        }


        Vector3 GetJumpPoint(AIHeroClient Qtarget, bool firstjump = true)
        {
            if (Khazix.ServerPosition.PointUnderEnemyTurret())
            {
                return Khazix.ServerPosition.LSExtend(NexusPosition, E.Range);
            }

            if (getBoxItem(djump, "jumpmode") == 0)
            {
                return Khazix.ServerPosition.LSExtend(NexusPosition, E.Range);
            }

            if (firstjump && getCheckBoxItem(djump, "jcursor"))
            {
                return Game.CursorPos;
            }

            if (!firstjump && getCheckBoxItem(djump, "jcursor2"))
            {
                return Game.CursorPos;
            }

            Vector3 Position = new Vector3();
            var jumptarget = IsHealthy
                  ? HeroList
                      .FirstOrDefault(x => x.IsValidTarget() && !x.IsZombie && x != Qtarget &&
                              Vector3.Distance(Khazix.ServerPosition, x.ServerPosition) < E.Range)
                  :
              HeroList
                  .FirstOrDefault(x => x.IsAlly && !x.IsZombie && !x.IsDead && !x.IsMe &&
                          Vector3.Distance(Khazix.ServerPosition, x.ServerPosition) < E.Range);

            if (jumptarget != null)
            {
                Position = jumptarget.ServerPosition;
            }
            if (jumptarget == null)
            {
                return Khazix.ServerPosition.LSExtend(NexusPosition, E.Range);
            }
            return Position;
        }

        void SpellCast(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (!EvolvedE || !getCheckBoxItem(djump, "save"))
            {
                return;
            }

            if (args.Slot.Equals(SpellSlot.Q) && args.Target is AIHeroClient && getCheckBoxItem(djump, "djumpenabled"))
            {
                var target = args.Target as AIHeroClient;
                var qdmg = GetQDamage(target);
                var dmg = (Khazix.GetAutoAttackDamage(target) * 2) + qdmg;
                if (target.Health < dmg && target.Health > qdmg)
                { //save some unnecessary q's if target is killable with 2 autos + Q instead of Q as Q is important for double jumping
                    args.Process = false;
                }
            }
        }

        void BeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (args.Target.Type == GameObjectType.AIHeroClient)
            {
                if (getCheckBoxItem(safety, "Safety.noaainult") && IsInvisible)
                {
                    args.Process = false;
                    return;
                }
                if (getCheckBoxItem(djump, "djumpenabled") && getCheckBoxItem(djump, "noauto"))
                {
                    if (args.Target.Health < GetQDamage((AIHeroClient)args.Target) &&
                        Khazix.ManaPercent > 15)
                    {
                        args.Process = false;
                    }
                }
            }
        }

        void OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(draw, "Drawings.Disable") || Khazix.IsDead || Khazix.IsRecalling())
            {
                return;
            }
            if (getCheckBoxItem(debug, "Debugon"))
            {
                var isolatedtargs = GetIsolatedTargets();
                foreach (var x in isolatedtargs)
                {
                    var heroposwts = Drawing.WorldToScreen(x.Position);
                    Drawing.DrawText(heroposwts.X, heroposwts.Y, System.Drawing.Color.White, "Isolated");
                }
            }

            if (getCheckBoxItem(djump, "jumpdrawings") && Jumping)
            {
                var PlayerPosition = Drawing.WorldToScreen(Khazix.Position);
                var Jump1 = Drawing.WorldToScreen(Jumppoint1).To3D();
                var Jump2 = Drawing.WorldToScreen(Jumppoint2).To3D();
                Render.Circle.DrawCircle(Jump1, 250, System.Drawing.Color.White);
                Render.Circle.DrawCircle(Jump2, 250, System.Drawing.Color.White);
                Drawing.DrawLine(PlayerPosition.X, PlayerPosition.Y, Jump1.X, Jump1.Y, 10, System.Drawing.Color.DarkCyan);
                Drawing.DrawLine(Jump1.X, Jump1.Y, Jump2.X, Jump2.Y, 10, System.Drawing.Color.DarkCyan);
            }

            var drawq = getCheckBoxItem(draw, "DrawQ");
            var draww = getCheckBoxItem(draw, "DrawW");
            var drawe = getCheckBoxItem(draw, "DrawE");

            if (drawq)
            {
                Render.Circle.DrawCircle(Khazix.Position, Q.Range, System.Drawing.Color.White);
            }
            if (draww)
            {
                Render.Circle.DrawCircle(Khazix.Position, W.Range, System.Drawing.Color.Red);
            }

            if (drawe)
            {
                Render.Circle.DrawCircle(Khazix.Position, E.Range, System.Drawing.Color.Green);
            }
        }
    }
}

