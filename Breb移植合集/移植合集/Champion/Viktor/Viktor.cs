using System;
using System.Collections.Generic;
using System.Linq;
using Color = System.Drawing.Color;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;
using EloBuddy;
using LeagueSharp.Common;
using SharpDX;

namespace Viktor
{
    public class Program
    {
        public const string CHAMP_NAME = "Viktor";
        private static readonly AIHeroClient player = ObjectManager.Player;

        // Spells
        private static LeagueSharp.Common.Spell Q, W, E, R;
        private static readonly int maxRangeE = 1225;
        private static readonly int lengthE = 700;
        private static readonly int speedE = 1200;
        private static readonly int rangeE = 525;
        private static int lasttick = 0;
        private static Vector3 GapCloserPos;
        private static bool AttacksEnabled
        {
            get
            {
                if ((Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) || (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)))
                    return ((!Q.IsReady() || player.Mana < Q.Instance.SData.Mana) && (!E.IsReady() || player.Mana < E.Instance.SData.Mana));

                return true;
            }
        }

        private static void OrbwalkingOnBeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (args.Target.Type == GameObjectType.AIHeroClient)
            {
                args.Process = AttacksEnabled;
            }
            else
                args.Process = true;

        }
        public static void Game_OnGameLoad()
        {
            // Champ validation
            if (player.ChampionName != CHAMP_NAME)
                return;

            // Define spells
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 600);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 700);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, rangeE);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 700);

            // Finetune spells
            Q.SetTargetted(0.25f, 2000);
            W.SetSkillshot(0.5f, 300, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0, 80, speedE, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.25f, 450f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            // Create menu
            SetupMenu();

            // Register events
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Orbwalker.OnUnkillableMinion += Orbwalker_OnUnkillableMinion;
            Orbwalker.OnPreAttack += OrbwalkingOnBeforeAttack;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
        }
        private static void Orbwalker_OnUnkillableMinion(Obj_AI_Base target, Orbwalker.UnkillableMinionArgs args)
        {
            QLastHit((Obj_AI_Base)target);
        }
        private static void QLastHit(Obj_AI_Base minion)
        {
            bool castQ = ((getKeyBindItem(waveClear, "waveUseQLH")) || getCheckBoxItem(waveClear, "waveUseQ") && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear));
            if (castQ)
            {
                var distance = LeagueSharp.Common.Geometry.LSDistance(player, minion);
                var t = 250 + (int)distance / 2;
                var predHealth = HealthPrediction.GetHealthPrediction(minion, t, 0);
                Console.WriteLine(" Distance: " + distance + " timer : " + t + " health: " + predHealth);
                if (predHealth > 0 && Q.IsKillable(minion))
                {
                    Q.Cast(minion);
                }
            }
        }
        private static void Game_OnGameUpdate(EventArgs args)
        {
            // Combo
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                OnCombo();

            // Harass�
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                OnHarass();

            // WaveClear
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                OnWaveClear();
                OnJungleClear();
            }

            // Ultimate follow
            if (R.Instance.Name != "ViktorChaosStorm" && getCheckBoxItem(comboMenu, "AutoFollowR") && Environment.TickCount - lasttick > 0)
            {
                var stormT = TargetSelector.GetTarget(1100, DamageType.Magical);
                if (stormT != null)
                {
                    R.Cast(stormT.ServerPosition);
                    lasttick = Environment.TickCount + 500;
                }
            }
        }
        private static bool KillableWithAA(Obj_AI_Base target)
        {
            var qaaDmg = new Double[] { 20, 25, 30, 35, 40, 45, 50, 55, 60, 70, 80, 90, 110, 130, 150, 170, 190, 210 };
            if (player.HasBuff("viktorpowertransferreturn") && Orbwalker.CanAutoAttack && (player.CalcDamage(target, DamageType.Magical,
                    qaaDmg[player.Level >= 18 ? 18 - 1 : player.Level - 1] +
                    (player.TotalMagicalDamage * .5) + player.TotalAttackDamage) > target.Health))
            {
                Console.WriteLine("killable with aa");
                return true;
            }
            else
                return false;
        }
        private static void OnCombo()
        {
            bool useQ = getCheckBoxItem(comboMenu, "comboUseQ") && Q.IsReady();
            bool useW = getCheckBoxItem(comboMenu, "comboUseW") && W.IsReady();
            bool useE = getCheckBoxItem(comboMenu, "comboUseE") && E.IsReady();
            bool useR = getCheckBoxItem(comboMenu, "comboUseR") && R.IsReady();
            bool killpriority = getCheckBoxItem(comboMenu, "spPriority") && R.IsReady();
            bool rKillSteal = getCheckBoxItem(comboMenu, "rLastHit");
            var Etarget = TargetSelector.GetTarget(maxRangeE, DamageType.Magical);
            var Qtarget = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            var RTarget = TargetSelector.GetTarget(R.Range, DamageType.Magical);
            if (killpriority && Qtarget != null & Etarget != null && Etarget != Qtarget && ((Etarget.Health > TotalDmg(Etarget, false, true, false, false)) || (Etarget.Health > TotalDmg(Etarget, false, true, true, false) && Etarget == RTarget)) && Qtarget.Health < TotalDmg(Qtarget, true, true, false, false))
            {
                Etarget = Qtarget;
            }

            if (RTarget != null && rKillSteal && useR)
            {
                if (TotalDmg(RTarget, true, true, false, false) < RTarget.Health && TotalDmg(RTarget, true, true, true, true) > RTarget.Health)
                {
                    R.Cast(RTarget.ServerPosition);
                }
            }


            if (useE)
            {
                if (Etarget != null)
                    PredictCastE(Etarget);
            }
            if (useQ)
            {

                if (Qtarget != null)
                    Q.Cast(Qtarget);
            }

            if (useW)
            {
                var t = TargetSelector.GetTarget(W.Range, DamageType.Magical);

                if (t != null)
                {
                    if (t.Path.Count() < 2)
                    {
                        if (t.HasBuffOfType(BuffType.Slow))
                        {
                            if (W.GetPrediction(t).Hitchance >= LeagueSharp.Common.HitChance.Medium)
                                if (W.Cast(t) == LeagueSharp.Common.Spell.CastStates.SuccessfullyCasted)
                                    return;
                        }
                        if (t.CountEnemiesInRange(250) > 2 || W.IsInRange(t))
                        {
                            if (W.GetPrediction(t).Hitchance >= LeagueSharp.Common.HitChance.Medium)
                                if (W.Cast(t) == LeagueSharp.Common.Spell.CastStates.SuccessfullyCasted)
                                    return;
                        }
                    }
                }
            }
            if (useR && R.Instance.Name == "ViktorChaosStorm" && player.CanCast && !player.Spellbook.IsCastingSpell)
            {

                foreach (var unit in HeroManager.Enemies.Where(h => h.IsValidTarget(R.Range)))
                {
                    R.CastIfWillHit(unit, getSliderItem(comboMenu, "HitR"));

                }
            }

        }

        private static void OnHarass()
        {
            // Mana check
            if ((player.Mana / player.MaxMana) * 100 < getSliderItem(harass, "harassMana"))
                return;
            bool useE = getCheckBoxItem(harass, "harassUseE") && E.IsReady();
            bool useQ = getCheckBoxItem(harass, "harassUseQ") && Q.IsReady();
            if (useQ)
            {
                var qtarget = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
                if (qtarget != null)
                    Q.Cast(qtarget);
            }
            if (useE)
            {
                var target = TargetSelector.GetTarget(maxRangeE, DamageType.Magical);

                if (target != null)
                    PredictCastE(target);
            }
        }

        private static void OnWaveClear()
        {
            // Mana check
            if ((player.Mana / player.MaxMana) * 100 < getSliderItem(waveClear, "waveMana"))
                return;

            bool useQ = getCheckBoxItem(waveClear, "waveUseQ") && Q.IsReady();
            bool useE = getCheckBoxItem(waveClear, "waveUseE") && E.IsReady();

            if (useQ)
            {
                foreach (var minion in MinionManager.GetMinions(player.Position, player.AttackRange))
                {
                    if (Q.IsKillable(minion) && minion.BaseSkinName.Contains("Siege"))
                    {
                        QLastHit(minion);
                        break;
                    }
                }
            }

            if (useE)
                PredictCastMinionE(getSliderItem(waveClear, "waveNumE"));
        }

        private static void OnJungleClear()
        {
            // Mana check
            if ((player.Mana / player.MaxMana) * 100 < getSliderItem(waveClear, "waveMana"))
                return;

            bool useQ = getCheckBoxItem(waveClear, "waveUseQ") && Q.IsReady();
            bool useE = getCheckBoxItem(waveClear, "waveUseE") && E.IsReady();

            if (useQ)
            {
                foreach (var minion in MinionManager.GetMinions(player.Position, player.AttackRange, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth))
                {
                    Q.Cast(minion);
                }
            }

            if (useE)
                PredictCastMinionEJungle();
        }


        private static bool PredictCastMinionEJungle()
        {
            int requiredHitNumber = 1;
            int hitNum = 0;
            Vector2 startPos = new Vector2(0, 0);
            Vector2 endPos = new Vector2(0, 0);
            foreach (var minion in MinionManager.GetMinions(player.Position, rangeE, MinionTypes.All, MinionTeam.Neutral))
            {
                var farmLocation = GetBestLaserFarmLocation(minion.Position.To2D(), (from mnion in MinionManager.GetMinions(minion.Position, lengthE, MinionTypes.All, MinionTeam.Neutral) select mnion.Position.To2D()).ToList<Vector2>(), E.Width, lengthE);
                if (farmLocation.MinionsHit > hitNum)
                {
                    hitNum = farmLocation.MinionsHit;
                    startPos = minion.Position.To2D();
                    endPos = farmLocation.Position;
                }
            }

            if (startPos.X != 0 && startPos.Y != 0)
                return PredictCastMinionEJungle(startPos, requiredHitNumber);
            return false;
        }

        private static bool PredictCastMinionE(int requiredHitNumber = -1)
        {
            int hitNum = 0;
            Vector2 startPos = new Vector2(0, 0);
            Vector2 endPos = new Vector2(0, 0);
            foreach (var minion in MinionManager.GetMinions(player.Position, rangeE))
            {
                var farmLocation = GetBestLaserFarmLocation(minion.Position.To2D(), (from mnion in MinionManager.GetMinions(minion.Position, lengthE) select mnion.Position.To2D()).ToList<Vector2>(), E.Width, lengthE);
                if (farmLocation.MinionsHit > hitNum)
                {
                    hitNum = farmLocation.MinionsHit;
                    startPos = minion.Position.To2D();
                    endPos = farmLocation.Position;
                }
            }

            if (startPos.X != 0 && startPos.Y != 0)
                return PredictCastMinionE(startPos, requiredHitNumber);
            return false;
        }
        public static MinionManager.FarmLocation GetBestLaserFarmLocation(Vector2 sourcepos, List<Vector2> minionPositions, float width, float range)
        {
            var result = new Vector2();
            var minionCount = 0;
            var startPos = sourcepos;

            var max = minionPositions.Count;
            for (var i = 0; i < max; i++)
            {
                for (var j = 0; j < max; j++)
                {
                    if (minionPositions[j] != minionPositions[i])
                    {
                        minionPositions.Add((minionPositions[j] + minionPositions[i]) / 2);
                    }
                }
            }

            foreach (var pos in minionPositions)
            {
                if (pos.Distance(startPos, true) <= range * range)
                {
                    var endPos = startPos + range * (pos - startPos).Normalized();

                    var count =
                        minionPositions.Count(pos2 => pos2.Distance(startPos, endPos, true, true) <= width * width);

                    if (count >= minionCount)
                    {
                        result = endPos;
                        minionCount = count;
                    }
                }
            }

            return new MinionManager.FarmLocation(result, minionCount);
        }


        private static bool PredictCastMinionEJungle(Vector2 fromPosition, int requiredHitNumber = 1)
        {
            var farmLocation = GetBestLaserFarmLocation(fromPosition, MinionManager.GetMinionsPredictedPositions(MinionManager.GetMinions(fromPosition.To3D(), lengthE, MinionTypes.All, MinionTeam.Neutral), E.Delay, E.Width, speedE, fromPosition.To3D(), lengthE, false, SkillshotType.SkillshotLine), E.Width, lengthE);

            if (farmLocation.MinionsHit >= requiredHitNumber)
            {
                CastE(fromPosition, farmLocation.Position);
                return true;
            }

            return false;
        }
        private static bool PredictCastMinionE(Vector2 fromPosition, int requiredHitNumber = 1)
        {
            var farmLocation = GetBestLaserFarmLocation(fromPosition, MinionManager.GetMinionsPredictedPositions(MinionManager.GetMinions(fromPosition.To3D(), lengthE), E.Delay, E.Width, speedE, fromPosition.To3D(), lengthE, false, SkillshotType.SkillshotLine), E.Width, lengthE);

            if (farmLocation.MinionsHit >= requiredHitNumber)
            {
                CastE(fromPosition, farmLocation.Position);
                return true;
            }

            return false;
        }

        private static void PredictCastE(AIHeroClient target)
        {
            // Helpers
            bool inRange = Vector2.DistanceSquared(target.ServerPosition.To2D(), player.Position.To2D()) < E.Range * E.Range;
            PredictionOutput prediction;
            bool spellCasted = false;

            // Positions
            Vector3 pos1, pos2;

            // Champs
            var nearChamps = (from champ in ObjectManager.Get<AIHeroClient>() where champ.IsValidTarget(maxRangeE) && target != champ select champ).ToList();
            var innerChamps = new List<AIHeroClient>();
            var outerChamps = new List<AIHeroClient>();
            foreach (var champ in nearChamps)
            {
                if (Vector2.DistanceSquared(champ.ServerPosition.To2D(), player.Position.To2D()) < E.Range * E.Range)
                    innerChamps.Add(champ);
                else
                    outerChamps.Add(champ);
            }

            // Minions
            var nearMinions = MinionManager.GetMinions(player.Position, maxRangeE);
            var innerMinions = new List<Obj_AI_Base>();
            var outerMinions = new List<Obj_AI_Base>();
            foreach (var minion in nearMinions)
            {
                if (Vector2.DistanceSquared(minion.ServerPosition.To2D(), player.Position.To2D()) < E.Range * E.Range)
                    innerMinions.Add(minion);
                else
                    outerMinions.Add(minion);
            }

            // Main target in close range
            if (inRange)
            {
                // Get prediction reduced speed, adjusted sourcePosition
                E.Speed = speedE * 0.9f;
                E.From = target.ServerPosition + (Vector3.Normalize(player.Position - target.ServerPosition) * (lengthE * 0.1f));
                prediction = E.GetPrediction(target);
                E.From = player.Position;

                // Prediction in range, go on
                if (prediction.CastPosition.Distance(player.Position) < E.Range)
                    pos1 = prediction.CastPosition;
                // Prediction not in range, use exact position
                else
                {
                    pos1 = target.ServerPosition;
                    E.Speed = speedE;
                }

                // Set new sourcePosition
                E.From = pos1;
                E.RangeCheckFrom = pos1;

                // Set new range
                E.Range = lengthE;

                // Get next target
                if (nearChamps.Count > 0)
                {
                    // Get best champion around
                    var closeToPrediction = new List<AIHeroClient>();
                    foreach (var enemy in nearChamps)
                    {
                        // Get prediction
                        prediction = E.GetPrediction(enemy);
                        // Validate target
                        if (prediction.Hitchance >= LeagueSharp.Common.HitChance.High && Vector2.DistanceSquared(pos1.To2D(), prediction.CastPosition.To2D()) < (E.Range * E.Range) * 0.8)
                            closeToPrediction.Add(enemy);
                    }

                    // Champ found
                    if (closeToPrediction.Count > 0)
                    {
                        // Sort table by health DEC
                        if (closeToPrediction.Count > 1)
                            closeToPrediction.Sort((enemy1, enemy2) => enemy2.Health.CompareTo(enemy1.Health));

                        // Set destination
                        prediction = E.GetPrediction(closeToPrediction[0]);
                        pos2 = prediction.CastPosition;

                        // Cast spell
                        CastE(pos1, pos2);
                        spellCasted = true;
                    }
                }

                // Spell not casted
                if (!spellCasted)
                    // Try casting on minion
                    if (!PredictCastMinionE(pos1.To2D()))
                        // Cast it directly
                        CastE(pos1, E.GetPrediction(target).CastPosition);

                // Reset spell
                E.Speed = speedE;
                E.Range = rangeE;
                E.From = player.Position;
                E.RangeCheckFrom = player.Position;
            }

            // Main target in extended range
            else
            {
                // Radius of the start point to search enemies in
                float startPointRadius = 150;

                // Get initial start point at the border of cast radius
                Vector3 startPoint = player.Position + Vector3.Normalize(target.ServerPosition - player.Position) * rangeE;

                // Potential start from postitions
                var targets = (from champ in nearChamps where Vector2.DistanceSquared(champ.ServerPosition.To2D(), startPoint.To2D()) < startPointRadius * startPointRadius && Vector2.DistanceSquared(player.Position.To2D(), champ.ServerPosition.To2D()) < rangeE * rangeE select champ).ToList();
                if (targets.Count > 0)
                {
                    // Sort table by health DEC
                    if (targets.Count > 1)
                        targets.Sort((enemy1, enemy2) => enemy2.Health.CompareTo(enemy1.Health));

                    // Set target
                    pos1 = targets[0].ServerPosition;
                }
                else
                {
                    var minionTargets = (from minion in nearMinions where Vector2.DistanceSquared(minion.ServerPosition.To2D(), startPoint.To2D()) < startPointRadius * startPointRadius && Vector2.DistanceSquared(player.Position.To2D(), minion.ServerPosition.To2D()) < rangeE * rangeE select minion).ToList();
                    if (minionTargets.Count > 0)
                    {
                        // Sort table by health DEC
                        if (minionTargets.Count > 1)
                            minionTargets.Sort((enemy1, enemy2) => enemy2.Health.CompareTo(enemy1.Health));

                        // Set target
                        pos1 = minionTargets[0].ServerPosition;
                    }
                    else
                        // Just the regular, calculated start pos
                        pos1 = startPoint;
                }

                // Predict target position
                E.From = pos1;
                E.Range = lengthE;
                E.RangeCheckFrom = pos1;
                prediction = E.GetPrediction(target);

                // Cast the E
                if (prediction.Hitchance >= LeagueSharp.Common.HitChance.High)
                    CastE(pos1, prediction.CastPosition);

                // Reset spell
                E.Range = rangeE;
                E.From = player.Position;
                E.RangeCheckFrom = player.Position;
            }

        }

        private static void CastE(Vector3 source, Vector3 destination)
        {
            E.Cast(source, destination);
        }

        private static void CastE(Vector2 source, Vector2 destination)
        {
            E.Cast(source, destination);
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (args.DangerLevel >= Interrupter2.DangerLevel.High)
            {
                var useW = getCheckBoxItem(misc, "wInterrupt");
                var useR = getCheckBoxItem(misc, "rInterrupt");

                if (useW && W.IsReady() && unit.IsValidTarget(W.Range) &&
                    (Game.Time + 1.5 + W.Delay) >= args.EndTime)
                {
                    if (W.Cast(unit) == LeagueSharp.Common.Spell.CastStates.SuccessfullyCasted)
                        return;
                }
                else if (useR && unit.IsValidTarget(R.Range) && R.Instance.Name == "ViktorChaosStorm")
                {
                    R.Cast(unit);
                }
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (getCheckBoxItem(misc, "miscGapcloser") && W.IsInRange(gapcloser.End))
            {
                GapCloserPos = gapcloser.End;
                if (LeagueSharp.Common.Geometry.LSDistance(gapcloser.Start, gapcloser.End) > gapcloser.Sender.Spellbook.GetSpell(gapcloser.Slot).SData.CastRangeDisplayOverride && gapcloser.Sender.Spellbook.GetSpell(gapcloser.Slot).SData.CastRangeDisplayOverride > 100)
                {
                    GapCloserPos = LeagueSharp.Common.Geometry.LSExtend(gapcloser.Start, gapcloser.End, gapcloser.Sender.Spellbook.GetSpell(gapcloser.Slot).SData.CastRangeDisplayOverride);
                }
                W.Cast(GapCloserPos.To2D(), true);
            }
        }
        private static void AutoW()
        {
            if (!W.IsReady() || !getCheckBoxItem(misc, "autoW"))
                return;

            var tPanth = HeroManager.Enemies.Find(h => h.IsValidTarget(W.Range) && h.HasBuff("Pantheon_GrandSkyfall_Jump"));
            if (tPanth != null)
            {
                if (W.Cast(tPanth) == LeagueSharp.Common.Spell.CastStates.SuccessfullyCasted)
                    return;
            }

            foreach (var enemy in HeroManager.Enemies.Where(h => h.IsValidTarget(W.Range)))
            {
                if (enemy.HasBuff("rocketgrab2"))
                {
                    var t = HeroManager.Allies.Find(h => h.BaseSkinName.ToLower() == "blitzcrank" && h.Distance((AttackableUnit)player) < W.Range);
                    if (t != null)
                    {
                        if (W.Cast(t) == LeagueSharp.Common.Spell.CastStates.SuccessfullyCasted)
                            return;
                    }
                }
                if (enemy.HasBuffOfType(BuffType.Stun) || enemy.HasBuffOfType(BuffType.Snare) ||
                         enemy.HasBuffOfType(BuffType.Charm) || enemy.HasBuffOfType(BuffType.Fear) ||
                         enemy.HasBuffOfType(BuffType.Taunt) || enemy.HasBuffOfType(BuffType.Suppression) ||
                         enemy.IsStunned || enemy.IsRecalling())
                {
                    if (W.Cast(enemy) == LeagueSharp.Common.Spell.CastStates.SuccessfullyCasted)
                        return;
                }
                if (W.GetPrediction(enemy).Hitchance == LeagueSharp.Common.HitChance.Immobile)
                {
                    if (W.Cast(enemy) == LeagueSharp.Common.Spell.CastStates.SuccessfullyCasted)
                        return;
                }
            }
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (getCheckBoxItem(draw, "drawRangeQ"))
            {
                Render.Circle.DrawCircle(player.Position, Q.Range, Color.IndianRed);
            }

            if (getCheckBoxItem(draw, "drawRangeW"))
            {
                Render.Circle.DrawCircle(player.Position, W.Range, Color.IndianRed);
            }

            if (getCheckBoxItem(draw, "drawRangeE"))
            {
                Render.Circle.DrawCircle(player.Position, E.Range, Color.DarkRed);
            }

            if (getCheckBoxItem(draw, "drawRangeEMax"))
            {
                Render.Circle.DrawCircle(player.Position, maxRangeE, Color.OrangeRed);
            }

            if (getCheckBoxItem(draw, "drawRangeR"))
            {
                Render.Circle.DrawCircle(player.Position, R.Range, Color.Red);
            }
        }

        private static float TotalDmg(Obj_AI_Base enemy, bool useQ, bool useE, bool useR, bool qRange)
        {
            var qaaDmg = new Double[] { 20, 25, 30, 35, 40, 45, 50, 55, 60, 70, 80, 90, 110, 130, 150, 170, 190, 210 };
            var damage = 0d;
            var rTicks = getSliderItem(comboMenu, "rTicks");
            bool inQRange = ((qRange && Orbwalking.InAutoAttackRange(enemy)) || qRange == false);
            //Base Q damage
            if (useQ && Q.IsReady() && inQRange)
            {
                damage += player.GetSpellDamage(enemy, SpellSlot.Q);
                damage += player.CalcDamage(enemy, DamageType.Magical, qaaDmg[player.Level >= 18 ? 18 - 1 : player.Level - 1] + (player.TotalMagicalDamage * .5) + player.TotalAttackDamage);
            }

            // Q damage on AA
            if (useQ && !Q.IsReady() && player.HasBuff("viktorpowertransferreturn") && inQRange)
            {
                damage += player.CalcDamage(enemy, DamageType.Magical,
                    qaaDmg[player.Level >= 18 ? 18 - 1 : player.Level - 1] +
                    (player.TotalMagicalDamage * .5) + player.TotalAttackDamage);
            }

            //E damage
            if (useE && E.IsReady())
            {
                if (player.HasBuff("viktoreaug") || player.HasBuff("viktorqeaug") || player.HasBuff("viktorqweaug"))
                    damage += player.LSGetSpellDamage(enemy, SpellSlot.E, 1);
                else
                    damage += player.LSGetSpellDamage(enemy, SpellSlot.E, 0);
            }

            //R damage + 2 ticks
            if (useR && R.Level > 0 && R.IsReady() && R.Instance.Name == "ViktorChaosStorm")
            {
                damage += LeagueSharp.Common.Damage.LSGetSpellDamage(player, enemy, SpellSlot.R, 1) * rTicks;
                damage += LeagueSharp.Common.Damage.LSGetSpellDamage(player, enemy, SpellSlot.R);
            }

            // Ludens Echo damage
            if (Items.HasItem(3285))
                damage += player.CalcDamage(enemy, DamageType.Magical, 100 + player.FlatMagicDamageMod * 0.1);

            //sheen damage
            if (Items.HasItem(3057))
                damage += player.CalcDamage(enemy, DamageType.Physical, 0.5 * player.BaseAttackDamage);

            //lich bane dmg
            if (Items.HasItem(3100))
                damage += player.CalcDamage(enemy, DamageType.Magical, 0.5 * player.FlatMagicDamageMod + 0.75 * player.BaseAttackDamage);

            return (float)damage;
        }
        private static float GetComboDamage(Obj_AI_Base enemy)
        {

            return TotalDmg(enemy, true, true, true, false);
        }

        public static Menu menu, comboMenu, harass, waveClear, misc, draw;

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

        private static void SetupMenu()
        {

            menu = MainMenu.AddMenu("TRUSt in my " + CHAMP_NAME, "asd");

            // Combo
            comboMenu = menu.AddSubMenu("Combo");
            comboMenu.Add("comboUseQ", new CheckBox("Use Q"));
            comboMenu.Add("comboUseW", new CheckBox("Use W"));
            comboMenu.Add("comboUseE", new CheckBox("Use E"));
            comboMenu.Add("comboUseR", new CheckBox("Use R"));
            comboMenu.Add("HitR", new Slider("Ultimate to hit", 3, 1, 5));
            comboMenu.Add("rLastHit", new CheckBox("1 target ulti"));
            comboMenu.Add("AutoFollowR", new CheckBox("Auto Follow R"));
            comboMenu.Add("rTicks", new Slider("Ultimate ticks to count", 2, 1, 14));
            comboMenu.AddGroupLabel("Test Features");
            comboMenu.Add("spPriority", new CheckBox("Prioritize kill over dmg"));

            // Harass
            harass = menu.AddSubMenu("Harass");
            harass.Add("harassUseQ", new CheckBox("Use Q"));
            harass.Add("harassUseE", new CheckBox("Use E"));
            harass.Add("harassMana", new Slider("Mana usage in percent (%)", 30));

            // WaveClear
            waveClear = menu.AddSubMenu("WaveClear");
            waveClear.Add("waveUseQ", new CheckBox("Use Q"));
            waveClear.Add("waveUseE", new CheckBox("Use E"));
            waveClear.Add("waveNumE", new Slider("Minions to hit with E", 2, 1, 10));
            waveClear.Add("waveMana", new Slider("Mana usage in percent (%)", 30));
            waveClear.AddGroupLabel("LastHit");
            waveClear.Add("waveUseQLH", new KeyBind("Use Q", false, KeyBind.BindTypes.PressToggle, 'A'));

            // Misc
            misc = menu.AddSubMenu("Misc");
            misc.Add("rInterrupt", new CheckBox("Use R to interrupt dangerous spells"));
            misc.Add("wInterrupt", new CheckBox("Use W to interrupt dangerous spells"));
            misc.Add("autoW", new CheckBox("Use W to continue CC"));
            misc.Add("miscGapcloser", new CheckBox("Use W against gapclosers"));

            // Drawings
            draw = menu.AddSubMenu("Drawings");
            draw.Add("drawRangeQ", new CheckBox("Q range"));// true, Color.FromArgb(150, Color.IndianRed), Q.Range));
            draw.Add("drawRangeW", new CheckBox("W range"));// true, Color.FromArgb(150, Color.IndianRed), W.Range));
            draw.Add("drawRangeE", new CheckBox("E range"));// false, Color.FromArgb(150, Color.DarkRed), E.Range));
            draw.Add("drawRangeEMax", new CheckBox("E max range"));// true, Color.FromArgb(150, Color.OrangeRed), maxRangeE));
            draw.Add("drawRangeR", new CheckBox("R range"));// false, Color.FromArgb(150, Color.Red), R.Range));
        }
    }
}