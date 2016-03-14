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
using Settings = PurifierVayne.Config.Misc;

namespace PurifierVayne
{
    public static class SpellManager
    {
        // You will need to edit the types of spells you have for each champ as they
        // don't have the same type for each champ, for example Xerath Q is chargeable,
        // right now it's  set to Active.
        public static Spell.Active Q { get; private set; }
        public static Spell.Targeted E { get; private set; }
        public static Spell.Skillshot E2 { get; private set; }
        public static Spell.Active R { get; private set; }
        public static Spell.Active heal { get; private set; }
        public static Spell.Targeted ignite { get; private set; }

        static SpellManager()
        {
            // Initialize spells
            Q = new Spell.Active(SpellSlot.Q, 300);
            E = new Spell.Targeted(SpellSlot.E, 550);
            E2 = new Spell.Skillshot(SpellSlot.E, 600, SkillShotType.Linear, 250, 1250);
            R = new Spell.Active(SpellSlot.R);
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



        internal static void CastQ(bool onChamps)
        {
            if (!Q.IsReady())
                return;
            if (Config.QSettings.useQToMouse
                || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit) && Config.QSettings.lastHitQToMouse
                || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) && Config.QSettings.laneClearQToMouse
                || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) && Config.QSettings.laneClearQToMouse
                || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && Config.QSettings.comboQToMouse
                || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) && Config.QSettings.harassQToMouse
                )
            {
                Player.CastSpell(SpellSlot.Q, Game.CursorPos);
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
            foreach(var pos in castTo)
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
                    if(pos.IsInRange(t.Position, t.GetAutoAttackRange() + 10))
                        castTo.Remove(pos);
                }
            }
            List<Vector2> goodOnes = new List<Vector2>();
            if (onChamps)
            {
                foreach (var pos in castTo)
                {
                    if (!defQ())
                    {
                        bool good = true;
                        foreach (var e in EntityManager.Heroes.Enemies.Where(t => t.IsInRange(Player.Instance, 850) && t.HealthPercent > 0))
                        {
                            if (!(Math.Abs(e.Distance(pos) - 550) < 150) && pos.CountEnemiesInRange(550) - (pos.CountAlliesInRange(450) + 1) <= Config.QSettings.defQ)
                            {
                                good = false;
                            }
                        }
                        if (good)
                        {
                            goodOnes.Add(pos);
                        }
                    }
                    else
                    {
                        bool good = true;
                        foreach (var e in EntityManager.Heroes.Enemies.Where(t => t.IsInRange(Player.Instance, 850) && t.HealthPercent > 0))
                        {
                            if (!(e.Distance(pos) - 550 > 0 && e.Distance(pos) - 550 < 50 && pos.CountEnemiesInRange(550) == 0))
                            {
                                good = false;
                            }
                        }
                        if (good)
                        {
                            goodOnes.Add(pos);
                        }

                    }
                }
            }
            else
            {
                foreach (var pos in castTo)
                {
                    var e = EntityManager.MinionsAndMonsters.CombinedAttackable.Where(t => t.Health > 0 && t.Distance(Player.Instance) < 850).OrderBy(t => t.Distance(Player.Instance)).FirstOrDefault();
                    if(e != null && Math.Abs(e.Distance(pos) - 550) < 100)
                        goodOnes.Add(pos);
                }
            }
            goodOnes.OrderBy(t => t.CountEnemiesInRange(550));
            if (E.IsReady())
            {
                foreach (var pos in goodOnes)
                {
                    AIHeroClient t = condemnable(pos.To3D());
                    if (t != null)
                    {
                        Player.CastSpell(SpellSlot.Q, pos.To3D());
                    }
                }
            }
            foreach (var pos in goodOnes)
            {
                if (pos.ToNavMeshCell().CollFlags.HasFlag(CollisionFlags.Grass))
                {
                    Player.CastSpell(SpellSlot.Q, pos.To3D());
                    return;
                }
            }

            if (goodOnes.Count > 0)
                Player.CastSpell(SpellSlot.Q, goodOnes.OrderBy(t => t.Distance(Game.CursorPos2D)).FirstOrDefault().To3D());
            else if (castTo.Count > 0)
                Player.CastSpell(SpellSlot.Q, castTo.OrderBy(t => t.Distance(Game.CursorPos2D)).FirstOrDefault().To3D());
            else
                Player.CastSpell(SpellSlot.Q, Game.CursorPos);
        }

        private static float damage(AIHeroClient t)
        {
            float dmg = 0;
            if (E.IsReady())
            {
                dmg += DamageLibrary.GetSpellDamage(Player.Instance, t, SpellSlot.E);
            }
            dmg += DamageLibrary.GetSpellDamage(Player.Instance, t, SpellSlot.Q);
            dmg += Player.Instance.GetAutoAttackDamage(t);
            return dmg;
        }

        private static bool defQ()
        {
            if (Player.Instance.HealthPercent < Config.QSettings.lowHPQ)
                return true;

            if (Player.Instance.CountEnemiesInRange(550) - (Player.Instance.CountAlliesInRange(450) + 1) >= Config.QSettings.defQ)
            {
                return true;
            }
            return false;
        }

        private static AIHeroClient condemnable(Vector3 myPosition)
        {
            List<AIHeroClient> condemnables = new List<AIHeroClient>();
            foreach (var e in EntityManager.Heroes.Enemies.Where(t => t.IsInRange(Player.Instance, E.Range) && t.HealthPercent > 0).OrderBy(t => t.Health))
            {
                if (e.HasBuffOfType(BuffType.SpellImmunity) || e.HasBuffOfType(BuffType.SpellShield))
                    continue;

                Vector2 castTo = Prediction.Position.PredictUnitPosition(e, 600);
                Vector3 pos = e.ServerPosition;
                for (float i = e.BoundingRadius / 2; i < 410; i += e.BoundingRadius)
                {
                    var coll = Player.Instance.ServerPosition.Extend(castTo, Player.Instance.Distance(castTo) + i).To3D();
                    var collOrigin = Player.Instance.ServerPosition.Extend(pos, Player.Instance.Distance(pos) + i).To3D();
                    if ((coll.ToNavMeshCell().CollFlags.HasFlag(CollisionFlags.Wall) || coll.ToNavMeshCell().CollFlags.HasFlag(CollisionFlags.Building)) ||
                        (collOrigin.ToNavMeshCell().CollFlags.HasFlag(CollisionFlags.Wall) || collOrigin.ToNavMeshCell().CollFlags.HasFlag(CollisionFlags.Building)))
                    {
                        condemnables.Add(e);
                    }
                }
            }
            foreach(var e in condemnables)
            {
                var pred = E2.GetPrediction(e);
                if (pred.HitChance < HitChance.High)
                    continue;
                for (float i = e.BoundingRadius/2; i < 410; i += e.BoundingRadius)
                {
                    var coll = Player.Instance.Position.To2D().Extend(pred.UnitPosition.To2D(), i + Player.Instance.Distance(pred.UnitPosition));
                    var collOrigin = Player.Instance.Position.To2D().Extend(pred.CastPosition.To2D(), i + Player.Instance.Distance(pred.CastPosition));
                    if ((coll.ToNavMeshCell().CollFlags.HasFlag(CollisionFlags.Wall) || coll.ToNavMeshCell().CollFlags.HasFlag(CollisionFlags.Building))||
                        (collOrigin.ToNavMeshCell().CollFlags.HasFlag(CollisionFlags.Wall) || collOrigin.ToNavMeshCell().CollFlags.HasFlag(CollisionFlags.Building)))
                    {
                        return e;
                    }
                }
            }
            return null;
        }

        internal static void CastE()
        {
            //vaynesilvereddebuff
            if (!E.IsReady())
                return;
            
            if (!Player.Instance.IsDashing() && (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && Config.ESettings.comboPinToWall) || (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) && Config.ESettings.harassPinToWall))
            {
                AIHeroClient t = condemnable(Player.Instance.Position);
                if(t != null)
                    E.Cast(t);
            }
            if ((Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && Config.ESettings.comboEProcW) || (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) && Config.ESettings.harassEProcW))
            {
                foreach (var e in EntityManager.Heroes.Enemies.Where(t => t.IsInRange(Player.Instance, E.Range) && t.HealthPercent > 0))
                {
                    if (e.GetBuffCount("vaynesilvereddebuff") == 2)
                    {
                        E.Cast(e);
                        return;
                    }
                }
            }
        }

        internal static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
        }
    }
}
