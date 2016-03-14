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
using Settings = MissFortune.Config.Misc;

namespace MissFortune
{
    public static class SpellManager
    {
        // You will need to edit the types of spells you have for each champ as they
        // don't have the same type for each champ, for example Xerath Q is chargeable,
        // right now it's  set to Active.
        public static Spell.Targeted Q { get; private set; }
        public static Spell.Active W { get; private set; }
        public static Spell.Skillshot E { get; private set; }
        public static Spell.Skillshot R { get; private set; }
        public static Spell.Active heal { get; private set; }
        public static Spell.Targeted ignite { get; private set; }

        static SpellManager()
        {
            // Initialize spells
            Q = new Spell.Targeted(SpellSlot.Q, 650);
            W = new Spell.Active(SpellSlot.W);
            E = new Spell.Skillshot(SpellSlot.E, 1000, SkillShotType.Circular, 500, int.MaxValue, 200);
            R = new Spell.Skillshot(SpellSlot.R, 1400, SkillShotType.Cone, 0, int.MaxValue);
            R.ConeAngleDegrees = 30;
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

        public static void castQ(bool NotkillOnly, bool killMinionOnly)
        {
            castQ(NotkillOnly, killMinionOnly, false);
        }

        public static void castQ(bool NotkillOnly, bool killMinionOnly, bool onChampsOnly)
        {
            foreach (var killable in EntityManager.Heroes.Enemies.Where(e => e.IsInRange(Player.Instance, 1000) && !e.IsDead && !e.IsInvulnerable && e.IsTargetable && !e.IsZombie && (e.Health < DamageLibrary.GetSpellDamage(Player.Instance, e, SpellSlot.Q) || NotkillOnly)))
            {
                var killablePosition = Prediction.Position.PredictUnitPosition(killable, 250).To3D();
                int i = -1;
                for (int j = 0; j < EntityManager.Heroes.Enemies.Count; j++)
                {
                    if (killable.NetworkId == EntityManager.Heroes.Enemies[j].NetworkId)
                        i = j;
                }
                if (i == -1 || !Config.Misc.UseQOnI(i))
                    continue;

                bool buff = false;
                foreach (var b in killable.Buffs)
                {
                    if (b.Name == "missfortunepassivestack")
                        buff = true;
                }
                foreach (var t in EntityManager.Heroes.Enemies.Where(e => e.IsInRange(Player.Instance, Q.Range) && !e.IsDead && !e.IsInvulnerable && e.IsTargetable && Prediction.Position.PredictUnitPosition(e, 250).To3D().Distance(killable) < 500))
                {
                    Vector3 meToTarget = Prediction.Position.PredictUnitPosition(t, 250).To3D() - Prediction.Position.PredictUnitPosition(Player.Instance, 250).To3D();
                    if (meToTarget.AngleBetween(killablePosition) < 0.6981 && buff && meToTarget.AngleBetween(killablePosition) > 0)
                    {
                        Q.Cast(t);
                        return;
                    }
                    else if (meToTarget.AngleBetween(killablePosition) < 0.349066 && meToTarget.AngleBetween(killablePosition) > 0)
                    {
                        int m = EntityManager.MinionsAndMonsters.CombinedAttackable.Where(e => e.IsInRange(t, 500) && !e.IsDead && !e.IsInvulnerable && e.IsTargetable && !e.IsZombie && Prediction.Position.PredictUnitPosition(t, 250).To3D().AngleBetween(Prediction.Position.PredictUnitPosition(e, 250).To3D()) < 0.349066 && Prediction.Position.PredictUnitPosition(t, 250).To3D().AngleBetween(killablePosition) > 0).Count();
                        if (m == 0)
                        {
                            Q.Cast(t);
                            return;
                        }
                    }
                    else if (meToTarget.AngleBetween(killablePosition) < 0.6981 && meToTarget.AngleBetween(killablePosition) > 0)
                    {
                        int m = EntityManager.MinionsAndMonsters.CombinedAttackable.Where(e => e.IsInRange(t, 500) && !e.IsDead && !e.IsInvulnerable && e.IsTargetable && !e.IsZombie && Prediction.Position.PredictUnitPosition(t, 250).To3D().AngleBetween(Prediction.Position.PredictUnitPosition(e, 250).To3D()) < 0.6981 && Prediction.Position.PredictUnitPosition(t, 250).To3D().AngleBetween(killablePosition) > 0).Count();
                        if (m == 0)
                        {
                            Q.Cast(t);
                            return;
                        }
                    }
                    else if (meToTarget.AngleBetween(killablePosition) < 1.9 && meToTarget.AngleBetween(killablePosition) > 0)
                    {
                        int m = EntityManager.MinionsAndMonsters.CombinedAttackable.Where(e => e.IsInRange(t, 500) && !e.IsDead && !e.IsInvulnerable && e.IsTargetable && !e.IsZombie && Prediction.Position.PredictUnitPosition(t, 250).To3D().AngleBetween(Prediction.Position.PredictUnitPosition(e, 250).To3D()) < 1.9 && Prediction.Position.PredictUnitPosition(t, 250).To3D().AngleBetween(killablePosition) > 0).Count();
                        if (m == 0)
                        {
                            Q.Cast(t);
                            return;
                        }
                    }
                }
                if (onChampsOnly)
                    return;
                foreach (var t in EntityManager.MinionsAndMonsters.CombinedAttackable.Where(e => e.IsInRange(Player.Instance, Q.Range) && Prediction.Position.PredictUnitPosition(e, 250).To3D().Distance(killable) < 500 && !e.IsDead && !e.IsInvulnerable && e.IsTargetable && (!killMinionOnly || e.Health < Player.Instance.GetSpellDamage(e, SpellSlot.Q))).OrderBy(t => t.Health))
                {
                    Vector3 meToTarget = Prediction.Position.PredictUnitPosition(t, 250).To3D() - Prediction.Position.PredictUnitPosition(Player.Instance, 250).To3D();
                    if (meToTarget.AngleBetween(killablePosition) < 0.6981 && buff && meToTarget.AngleBetween(killablePosition) > 0)
                    {
                        Q.Cast(t);
                        return;
                    }
                    else if (meToTarget.AngleBetween(killablePosition) < 0.349066 && meToTarget.AngleBetween(killablePosition) > 0)
                    {
                        int m = EntityManager.MinionsAndMonsters.CombinedAttackable.Where(e => e.IsInRange(t, 500) && !e.IsDead && !e.IsInvulnerable && e.IsTargetable && !e.IsZombie && Prediction.Position.PredictUnitPosition(t, 250).To3D().AngleBetween(Prediction.Position.PredictUnitPosition(e, 250).To3D()) < 0.349066 && Prediction.Position.PredictUnitPosition(t, 250).To3D().AngleBetween(killablePosition) > 0).Count();
                        if (m == 0)
                        {
                            Q.Cast(t);
                            return;
                        }
                    }
                    else if (meToTarget.AngleBetween(killablePosition) < 0.6981 && meToTarget.AngleBetween(killablePosition) > 0)
                    {
                        int m = EntityManager.MinionsAndMonsters.CombinedAttackable.Where(e => e.IsInRange(t, 500) && !e.IsDead && !e.IsInvulnerable && e.IsTargetable && !e.IsZombie && Prediction.Position.PredictUnitPosition(t, 250).To3D().AngleBetween(Prediction.Position.PredictUnitPosition(e, 250).To3D()) < 0.6981 && Prediction.Position.PredictUnitPosition(t, 250).To3D().AngleBetween(killablePosition) > 0).Count();
                        if (m == 0)
                        {
                            Q.Cast(t);
                            return;
                        }
                    }
                    else if (meToTarget.AngleBetween(killablePosition) < 1.9 && meToTarget.AngleBetween(killablePosition) > 0)
                    {
                        int m = EntityManager.MinionsAndMonsters.CombinedAttackable.Where(e => e.IsInRange(t, 500) && !e.IsDead && !e.IsInvulnerable && e.IsTargetable && !e.IsZombie && Prediction.Position.PredictUnitPosition(t, 250).To3D().AngleBetween(Prediction.Position.PredictUnitPosition(e, 250).To3D()) < 1.9 && Prediction.Position.PredictUnitPosition(t, 250).To3D().AngleBetween(killablePosition) > 0).Count();
                        if (m == 0)
                        {
                            Q.Cast(t);
                            return;
                        }
                    }
                }
            }
        }
    }
}
