#region

using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using Color = System.Drawing.Color;

#endregion

namespace Velkoz
{
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    internal class Program
    {
        public const string ChampionName = "Velkoz";

        //Spells
        public static List<Spell.SpellBase> SpellList = new List<Spell.SpellBase>();

        public int LastCastAttemptT { get; set; }
        public static List<Vector2> Perpendiculars { get; set; }
        public static Vector2 Direction { get; set; }
        public static Spell.Skillshot QSplit;
        public static Spell.Skillshot QDummy;
        public static Spell.Skillshot Q { get; set; }
        public static Spell.Skillshot W { get; set; }
        public static Spell.Skillshot E { get; set; }
        public static Spell.Skillshot R { get; set; }

        public static Menu ComboMenu { get; private set; }

        public static Menu HarassMenu { get; private set; }

        public static Menu LaneMenu { get; private set; }

        public static Menu MiscMenu { get; private set; }

        public static Menu DrawMenu { get; private set; }

        private static Menu menuIni;

        public static MissileClient Qmiss = null;

        public const int MissileSpeed = 2100;
        public const int CastDelay = 250;
        public const int SpellWidth = 45;
        public const int SpellRange = 1100;

        public static SpellSlot IgniteSlot;

        public static MissileClient QMissile;

        //Menu
        public static Menu Config;

        private static AIHeroClient Player;
        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            Player = ObjectManager.Player;

            if (Player.BaseSkinName != ChampionName) return;

            //Create the spells

            QDummy = new Spell.Skillshot(SpellSlot.Q, 1100, SkillShotType.Linear, 250, int.MaxValue, 50)
            {
                AllowedCollisionCount = 0
            };
            QSplit = new Spell.Skillshot(SpellSlot.Q, 1100, SkillShotType.Linear, 250, 2100, 50)
            {
                MinimumHitChance = HitChance.High,
                AllowedCollisionCount = 0
            };
            Q = new Spell.Skillshot(SpellSlot.Q, 1100, SkillShotType.Linear, 250, 1300, 50)
            {
                MinimumHitChance = HitChance.High,
                AllowedCollisionCount = 0
            };
            W = new Spell.Skillshot(SpellSlot.W, 1050, SkillShotType.Linear, 250, 1700, 80)
            {
                MinimumHitChance = HitChance.High,
                AllowedCollisionCount = int.MaxValue
            };
            E = new Spell.Skillshot(SpellSlot.E, 850, SkillShotType.Circular, 500, 1500, 120)
            {
                MinimumHitChance = HitChance.High,
                AllowedCollisionCount = int.MaxValue
            };
            R = new Spell.Skillshot(SpellSlot.R, 1550, SkillShotType.Linear)
            {
                AllowedCollisionCount = int.MaxValue
            };

            IgniteSlot = Player.GetSpellSlotFromName("SummonerDot");
            

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            menuIni = MainMenu.AddMenu(ChampionName, ChampionName);
            menuIni.AddGroupLabel("Welcome to the Worst VelKoz addon!");
            menuIni.AddGroupLabel("Global Settings");
            menuIni.Add("Combo", new CheckBox("Use Combo?"));
            menuIni.Add("Harass", new CheckBox("Use Harass?"));
            menuIni.Add("Clear", new CheckBox("Use Lane Clear?"));
            menuIni.Add("Drawings", new CheckBox("Use Drawings?"));

            ComboMenu = menuIni.AddSubMenu("Combo");
            ComboMenu.AddGroupLabel("Combo Settings");
            ComboMenu.Add("Q", new CheckBox("Use Q"));
            ComboMenu.Add("W", new CheckBox("Use W"));
            ComboMenu.Add("E", new CheckBox("Use E"));
            ComboMenu.Add("R", new CheckBox("Use R"));
            ComboMenu.Add("Ignite", new CheckBox("Ignite"));
            ComboMenu.Add("Rhit", new Slider("Use R Hit", 2, 1, 5));

            HarassMenu = menuIni.AddSubMenu("Harass");
            HarassMenu.AddGroupLabel("Harass Settings");
            HarassMenu.Add("Q", new CheckBox("Use Q"));
            HarassMenu.Add("W", new CheckBox("Use W"));
            HarassMenu.Add("E", new CheckBox("Use E"));
            HarassMenu.Add("Mana", new Slider("Save Mana %", 30, 0, 100));

            LaneMenu = menuIni.AddSubMenu("Farm");
            LaneMenu.AddGroupLabel("LaneClear Settings");
            LaneMenu.Add("Q", new CheckBox("Use Q"));
            LaneMenu.Add("W", new CheckBox("Use W"));
            LaneMenu.Add("E", new CheckBox("Use E"));
            LaneMenu.Add("Mana", new Slider("Save Mana %", 30, 0, 100));

            MiscMenu = menuIni.AddSubMenu("Misc");
            MiscMenu.AddGroupLabel("Misc Settings");
            MiscMenu.Add("gapcloser", new CheckBox("Anti-GapCloser"));
            MiscMenu.Add("Interrupt", new CheckBox("Interrupt"));
            MiscMenu.Add("gapclosermana", new Slider("Anti-GapCloser Mana", 25, 0, 100));

            DrawMenu = menuIni.AddSubMenu("Drawings");
            DrawMenu.AddGroupLabel("Drawing Settings");
            DrawMenu.Add("Q", new CheckBox("Draw Q"));
            DrawMenu.Add("W", new CheckBox("Draw W"));
            DrawMenu.Add("E", new CheckBox("Draw E"));
            DrawMenu.Add("R", new CheckBox("Draw R"));


            Game.OnUpdate += Game_OnGameUpdate;
            Interrupter.OnInterruptableSpell += Interrupter2_OnInterruptableTarget;
            GameObject.OnCreate += Obj_SpellMissile_OnCreate;
            Spellbook.OnUpdateChargeableSpell += Spellbook_OnUpdateChargedSpell;
        }

        static void Interrupter2_OnInterruptableTarget(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {
            if (!MiscMenu["Interrupt"].Cast<CheckBox>().CurrentValue && !sender.IsEnemy) return;
            E.Cast(sender);
        }

        private static void Obj_SpellMissile_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.IsMe && sender.IsAlly)
            {
                MissileClient missile = (MissileClient)sender;
                if (missile.SData.Name != null && missile.SData.Name == "VelkozQMissile")
                    QMissile = missile;
                Direction = (missile.EndPosition.To2D() - missile.StartPosition.To2D()).Normalized();
                Perpendiculars.Add(Direction.Perpendicular());
                Perpendiculars.Add(Direction.Perpendicular2());
            }
        }
        

        static void Spellbook_OnUpdateChargedSpell(Spellbook sender, SpellbookUpdateChargeableSpellEventArgs args)
        {
            var flags = Orbwalker.ActiveModesFlags;
                if (sender.Owner.IsMe)
            {
                args.Process =
                        !((flags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                          ComboMenu["R"].Cast<CheckBox>().CurrentValue));
            }
        }

        private static void Combo()
        {
            UseSpells(ComboMenu["Q"].Cast<CheckBox>().CurrentValue, ComboMenu["W"].Cast<CheckBox>().CurrentValue,
                ComboMenu["E"].Cast<CheckBox>().CurrentValue, ComboMenu["R"].Cast<CheckBox>().CurrentValue,
                ComboMenu["Ignite"].Cast<CheckBox>().CurrentValue);
        }

        private static void Harass()
        {
            UseSpells(HarassMenu["Q"].Cast<CheckBox>().CurrentValue, HarassMenu["W"].Cast<CheckBox>().CurrentValue,
                HarassMenu["E"].Cast<CheckBox>().CurrentValue, false, false);
        }

        private static float GetComboDamage(Obj_AI_Base enemy)
        {
            var damage = 0d;

            if (Q.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.Q);

            if (W.IsReady())
            {
                damage += W.Handle.Ammo * Player.GetSpellDamage(enemy, SpellSlot.W);
            }

            if (E.IsReady())
            {
                damage += Player.GetSpellDamage(enemy, SpellSlot.E);
            }

            if (IgniteSlot != SpellSlot.Unknown && Player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
            {
                damage += Player.GetSummonerSpellDamage(enemy, DamageLibrary.SummonerSpells.Ignite);
            }

            if (R.IsReady())
            {
                damage += 7 * Player.GetSpellDamage(enemy, SpellSlot.R) / 10;
            }

            return (float)damage;
        }

        private static void UseSpells(bool useQ, bool useW, bool useE, bool useR, bool useIgnite)
        {
            var qTarget = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            var qDummyTarget = TargetSelector.GetTarget(QDummy.Range, DamageType.Magical);
            var wTarget = TargetSelector.GetTarget(W.Range, DamageType.Magical);
            var eTarget = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            var rTarget = TargetSelector.GetTarget(R.Range, DamageType.Magical);


            if (useW && wTarget != null && W.IsReady())
            {
                W.Cast(wTarget.Position);
                return;
            }

            if (useE && eTarget != null && E.IsReady())
            {
                E.Cast(eTarget.Position);
                return;
            }
            
            if (useQ && qTarget != null && Q.IsReady() && Q.Name == "VelkozQ")
            {
                var predq = Q.GetPrediction(qTarget);
                if (predq.HitChance >= HitChance.High)
                {
                    Q.Cast(predq.CastPosition);
                }
            }

            if (qDummyTarget != null && useQ && Q.IsReady() && Q.Name == "VelkozQ")
            {
                if (qTarget != null) qDummyTarget = qTarget;
                QDummy.CastDelay = (int)(Q.CastDelay + Q.Range / Q.Speed * 1000 + QSplit.Range / QSplit.Speed * 1000);

                var predictedPos = QDummy.GetPrediction(qDummyTarget);
                if (predictedPos.HitChance >= HitChance.High)
                {
                    for (var i = -1; i < 1;)
                    {
                        const float Alpha = 28 * (float)Math.PI / 180;
                        Vector2 cp = ObjectManager.Player.ServerPosition.To2D() +
                                     (predictedPos.CastPosition.To2D() - ObjectManager.Player.ServerPosition.To2D()).Rotated
                                         (i * Alpha);
                        {
                            Q.Cast((Vector3)cp);
                            return;
                        }
                    }
                }
            }

            if (qTarget != null && useIgnite && IgniteSlot != SpellSlot.Unknown &&
                Player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
            {
                if (Player.Distance(qTarget) < 650 && GetComboDamage(qTarget) > qTarget.Health)
                {
                    Player.Spellbook.CastSpell(IgniteSlot, qTarget);
                }
            }

            if (useR && rTarget != null && R.IsReady() &&
                Player.GetSpellDamage(rTarget, SpellSlot.R) / 10 * (Player.Distance(rTarget) < (R.Range - 500) ? 10 : 6) > rTarget.Health)
            {
                if (!Q.IsReady() && !W.IsReady() && !E.IsReady())
                {
                    R.Cast(rTarget.Position);
                }
            }
        }
        

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead) return;
            if (Player.Spellbook.IsChanneling)
            {
                var endPoint = new Vector2();
                foreach (var obj in ObjectManager.Get<GameObject>())
                {
                    if (obj != null && obj.IsValid && obj.Name.Contains("Velkoz_") &&
                        obj.Name.Contains("_R_Beam_End"))
                    {
                        endPoint = Player.ServerPosition.To2D() +
                                   R.Range * (obj.Position - Player.ServerPosition).To2D().Normalized();
                        break;
                    }
                }

                if (endPoint.IsValid())
                {
                    var targets = new List<Obj_AI_Base>();

                    foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(h => h.IsValidTarget(R.Range)))
                    {
                        if (enemy.ServerPosition.To2D().Distance(Player.ServerPosition.To2D(), endPoint, true) < 400)
                        {
                            targets.Add(enemy);
                        }
                    }
                    if (targets.Count > 0)
                    {
                        var target = targets.OrderBy(t => t.Health / Player.GetSpellDamage(t, SpellSlot.Q)).ToList()[0];
                        ObjectManager.Player.Spellbook.UpdateChargeableSpell(SpellSlot.R, target.ServerPosition, false, false);
                    }
                    else
                    {
                        ObjectManager.Player.Spellbook.UpdateChargeableSpell(SpellSlot.R, Game.CursorPos, false, false);
                    }
                }

                return;
            }
            
            if (QMissile != null && Q.IsReady() && Q.Name == "VelkozQSplitActivate")
            {
                foreach (var perpendicular in Perpendiculars)
                {
                    if (QMissile != null)
                    {
                        var startPos = QMissile.Position.To2D();
                        var endPos = QMissile.Position.To2D() + SpellRange * perpendicular;

                        var collisionObjects = ObjectManager.Get<Obj_AI_Base>()
                            .Where(o => o.IsEnemy && !o.IsDead && !o.IsStructure() && !o.IsWard() && !o.IsInvulnerable
                                    && o.Distance(Player, true) < (SpellRange + 200).Pow()
                                    && o.ServerPosition.To2D().Distance(startPos, endPos, true, true) <= (SpellWidth * 2 + o.BoundingRadius).Pow());

                        var colliding = collisionObjects
                            .Where(o => o.Type == GameObjectType.AIHeroClient && o.IsValidTarget()
                                    && Prediction.Position.Collision.LinearMissileCollision(o, startPos, endPos, MissileSpeed, SpellWidth, CastDelay, (int)o.BoundingRadius))
                                .OrderBy(o => o.Distance(Player, true)).FirstOrDefault();

                        if (colliding != null)
                        {
                            EloBuddy.Player.CastSpell(SpellSlot.Q);
                            QMissile = null;
                        }
                    }
                }
            }

            if (QMissile != null && QMissile.IsValid && Q.Name == "VelkozQSplitActivate")
            {
                var qMissilePosition = QMissile.Position.To2D();
                var perpendicular = (QMissile.EndPosition - QMissile.StartPosition).To2D().Normalized().Perpendicular();

                var lineSegment1End = qMissilePosition + perpendicular * QSplit.Range;
                var lineSegment2End = qMissilePosition - perpendicular * QSplit.Range;

                var potentialTargets = new List<Obj_AI_Base>();
                foreach (
                    var enemy in
                        ObjectManager.Get<AIHeroClient>()
                            .Where(
                                h =>
                                    h.IsValidTarget() &&
                                    h.ServerPosition.To2D()
                                        .Distance(qMissilePosition, QMissile.EndPosition.To2D(), true) < 700))
                {
                    potentialTargets.Add(enemy);
                }

                QSplit.RangeCheckSource = qMissilePosition.To3D();
                QSplit.SourcePosition = qMissilePosition.To3D();
                
                foreach (
                    var enemy in
                        ObjectManager.Get<AIHeroClient>()
                            .Where(
                                h =>
                                    h.IsValidTarget() &&
                                    (potentialTargets.Count == 0 ||
                                     h.NetworkId == potentialTargets.OrderBy(t => t.Health / Player.GetSpellDamage(t, SpellSlot.Q)).ToList()[0].NetworkId) &&
                                    (h.ServerPosition.To2D().Distance(qMissilePosition, QMissile.EndPosition.To2D(), true) > Q.Width + h.BoundingRadius)))
                {
                    var prediction = QSplit.GetPrediction(enemy);
                    var d1 = prediction.UnitPosition.To2D().Distance(qMissilePosition, lineSegment1End, true);
                    var d2 = prediction.UnitPosition.To2D().Distance(qMissilePosition, lineSegment2End, true);
                    if (prediction.HitChance >= HitChance.High &&
                        (d1 < QSplit.Width + enemy.BoundingRadius || d2 < QSplit.Width + enemy.BoundingRadius))
                    {
                        Q.Cast();
                    }
                }
            }

            var flags = Orbwalker.ActiveModesFlags;
            if (flags.HasFlag(Orbwalker.ActiveModes.Combo))// && menuIni.Get<CheckBox>("Combo").CurrentValue)
            {
                Combo();
            }

            if (flags.HasFlag(Orbwalker.ActiveModes.Harass))//&& menuIni.Get<CheckBox>("Harass").CurrentValue)
            {
                Harass();
            }
            
        }
    }
}