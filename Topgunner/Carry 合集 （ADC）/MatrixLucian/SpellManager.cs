using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;
using Settings = MatrixLucian.Config.Misc;

namespace MatrixLucian
{
    public static class SpellManager
    {
        // You will need to edit the types of spells you have for each champ as they
        // don't have the same type for each champ, for example Xerath Q is chargeable,
        // right now it's  set to Active.
        public static Spell.Targeted Q;
        public static Spell.Skillshot Q1;
        public static Spell.Skillshot W;
        public static Spell.Skillshot W1;
        public static Spell.Skillshot E;
        public static Spell.Skillshot R;
        public static Spell.Active heal { get; private set; }
        public static Spell.Targeted ignite { get; private set; }

        static SpellManager()
        {
            // Initialize spells
            Q = new Spell.Targeted(SpellSlot.Q, 675);
            Q1 = new Spell.Skillshot(SpellSlot.Q, 1140, SkillShotType.Linear, 350, int.MaxValue, 75);
            W = new Spell.Skillshot(SpellSlot.W, 1000, SkillShotType.Linear, 250, 1600, 100);
            W1 = new Spell.Skillshot(SpellSlot.W, 500, SkillShotType.Linear, 250, 1600, 100);
            E = new Spell.Skillshot(SpellSlot.E, 475, SkillShotType.Linear);
            R = new Spell.Skillshot(SpellSlot.R, 1400, SkillShotType.Linear, 500, 2800, 110);

            var slot = ObjectManager.Player.GetSpellSlotFromName("summonerheal");
            if (slot != SpellSlot.Unknown)
            {
                heal = new Spell.Active(slot, 850);
            }
            var slot2 = ObjectManager.Player.GetSpellSlotFromName("summonerdot");
            if (slot2 != SpellSlot.Unknown)
            {
                ignite = new Spell.Targeted(slot2, 600);
            }

        }
        public static void Initialize()
        {
        }

        public static bool dashToQ1(AIHeroClient enemy)
        {
            int delay = (int)(E.Range / (700 * Player.Instance.MoveSpeed));
            List<Vector2> castTo = new List<Vector2>();
            foreach (var e in EntityManager.Heroes.Enemies.Where(t => t.IsInRange(enemy, Q1.Range - Q.Range) && t.HealthPercent > 0 && !t.IsInvulnerable))
            {
                for (int i = 0; i < Q.Range; i += 25)
                {
                    var targetPos = Prediction.Position.PredictUnitPosition(e, delay).Extend(Prediction.Position.PredictUnitPosition(enemy, delay), -1 * i);
                    if (Math.Abs(targetPos.Distance(Player.Instance) - E.Range) < 50)
                    {
                        castTo.Add(targetPos);
                    }
                }
            }
            foreach (var e in EntityManager.MinionsAndMonsters.CombinedAttackable.Where(t => t.IsInRange(Player.Instance, Q.Range) && t.HealthPercent > 0 && !t.IsInvulnerable))
            {
                for (int i = 0; i < Q.Range; i += 25)
                {
                    var targetPos = Prediction.Position.PredictUnitPosition(e, delay).Extend(Prediction.Position.PredictUnitPosition(enemy, delay), -1 * i);
                    if (Math.Abs(targetPos.Distance(Player.Instance) - E.Range) < 50)
                    {
                        castTo.Add(targetPos);
                    }
                }
            }
            if (castTo.Count == 0)
                return false;
            return dashTo(castTo);
        }

        public static bool dashTo(List<Vector2> castTo)
        {
            List<Vector2> castTo2 = new List<Vector2>();
            castTo2.AddRange(castTo);
            foreach (var pos in castTo)
            {
                if (pos.ToNavMeshCell().CollFlags.HasFlag(CollisionFlags.Building) || pos.ToNavMeshCell().CollFlags.HasFlag(CollisionFlags.Wall))
                {
                    castTo2.Remove(pos);
                    continue;
                }
                foreach (var skillshot in Evade.Evade.DetectedSkillshots)
                {
                    if (!skillshot.IsSafe(pos))
                    {
                        castTo2.Remove(pos);
                        break;
                    }
                }
                foreach (var t in EntityManager.Turrets.Enemies)
                {
                    if (pos.IsInRange(t.Position, t.GetAutoAttackRange() + 10))
                        castTo2.Remove(pos);
                }
            }
            if (castTo2.Count == 0)
                return false;
            E.Cast(castTo2.First().To3D());
            return true;
        }

        public static void castR()
        {
            var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
            if (Program.RTarget != null && Program.RTarget.Health > 0 && Program.RTarget.Distance(Player.Instance) < R.Range && !Program.RTarget.IsZombie && !Program.RTarget.IsInvulnerable)
            {
                target = Program.RTarget;
            }
            else
            {
                Program.RCastToPosition = target.Position;
                Program.RTarget = target;
            }
            if (target == null)
                return;

            Vector2 TRCast = Program.RCastToPosition.To2D();
            Vector2 MyRCast = Program.MyRCastPosition.To2D();
            Vector2 TPos = target.Position.To2D();
            Vector2 x = new Vector2(TPos.X - TRCast.X, TPos.Y - TRCast.Y);
            Vector2 Y = new Vector2(MyRCast.X + x.X,MyRCast.Y + x.Y);
            Vector2 best = new Vector2(Y.X, Y.Y);
            for (int i = -100; i <= 1000; i += 25)
            {
                Vector2 next = TPos.Extend(Y, i);
                if (next.Distance(TPos) < R.Range*0.75)
                {
                    if (next.Distance(TPos) > best.Distance(TPos))
                    {
                        best = next;
                    }
                }
                if (best.X == 0 || best.Y == 0 || best.Distance(TPos) > R.Range * 0.75)
                {
                    best = next;
                    continue;
                }
            }
            if (best.X == 0 || best.Y == 0)
            {
                return;
            }
            Player.IssueOrder(GameObjectOrder.MoveTo, best.To3D());
        }

        public static bool Q1hits(AIHeroClient target, Obj_AI_Base via)
        {
            for (int i = 0; i < Q.Range; i += 25)
            {
                var targetPos = Prediction.Position.PredictUnitPosition(via, Q1.CastDelay).Extend(Prediction.Position.PredictUnitPosition(target, Q1.CastDelay), -1 * i);
                if (Math.Abs(targetPos.Distance(Player.Instance) - E.Range) < 50)
                {
                    return true;
                }
            }
            return false;
        }

        public static void castQ1(AIHeroClient target)
        {

            foreach (var e in EntityManager.Heroes.Enemies.Where(t => t.IsInRange(Player.Instance, Q.Range) && t.HealthPercent > 0 && !t.IsInvulnerable))
            {
                if (Q.IsReady() && Q1hits(target, e))
                {
                    Q.Cast(e);
                }
            }
            foreach (var e in EntityManager.MinionsAndMonsters.CombinedAttackable.Where(t => t.IsInRange(Player.Instance, Q.Range) && t.HealthPercent > 0 && !t.IsInvulnerable))
            {
                if (Q.IsReady() && Q1hits(target, e))
                {
                    Q.Cast(e);
                }
            }
        }

        public static void castE()
        {
            if (Config.Misc.UseEToMouse)
            {
                Player.CastSpell(SpellSlot.E, Game.CursorPos);
                return;
            }
            List<Vector2> castTo = new List<Vector2>();
            int Qacc = 40;
            for (int i = 0; i < Qacc; i++)
            {
                double rad = i / Qacc * 2 * Math.PI;
                Vector2 onCircle = new Vector2(Player.Instance.Position.X + 300 * (float)Math.Cos(rad), Player.Instance.Position.Y + 300 * (float)Math.Sin(rad));
                castTo.Add(onCircle);
            }
            foreach (var pos in castTo)
            {
                foreach (var skillshot in Evade.Evade.DetectedSkillshots)
                {
                    if (!skillshot.IsSafe(pos))
                    {
                        castTo.Remove(pos);
                        break;
                    }
                }
                foreach (var t in EntityManager.Turrets.Enemies)
                {
                    if (pos.IsInRange(t.Position, t.GetAutoAttackRange() + 10))
                        castTo.Remove(pos);
                }
            }
            List<Vector2> goodOnes = new List<Vector2>();
            foreach (var pos in castTo)
            {
                bool good = true;
                foreach (var e in EntityManager.Heroes.Enemies.Where(t => t.IsInRange(Player.Instance, 850) && t.HealthPercent > 0))
                {
                    if (!(Math.Abs(e.Distance(pos) - 550) < 150) && pos.CountEnemiesInRange(550) - (pos.CountAlliesInRange(450) + 1) <= 1)
                    {
                        good = false;
                    }
                }
                if (good)
                {
                    goodOnes.Add(pos);
                }
            }
            goodOnes.OrderBy(t => t.CountEnemiesInRange(550));
            foreach (var pos in goodOnes)
            {
                if (pos.ToNavMeshCell().CollFlags.HasFlag(CollisionFlags.Grass))
                {
                    Player.CastSpell(SpellSlot.E, pos.To3D());
                    return;
                }
            }

            if (goodOnes.Count > 0)
                Player.CastSpell(SpellSlot.E, goodOnes.OrderBy(t => t.Distance(Game.CursorPos2D)).FirstOrDefault().To3D());
            else if (castTo.Count > 0)
                Player.CastSpell(SpellSlot.E, castTo.OrderBy(t => t.Distance(Game.CursorPos2D)).FirstOrDefault().To3D());
            else
                Player.CastSpell(SpellSlot.E, Game.CursorPos);
        }

    }
}
