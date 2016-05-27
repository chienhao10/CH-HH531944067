namespace KappAzir.Utility
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;

    using SharpDX;

    public static class Common
    {
        internal static bool Ehit(this Obj_AI_Base target)
        {
            return
                Orbwalker.AzirSoldiers.Select(
                    soldier => new Geometry.Polygon.Rectangle(Player.Instance.ServerPosition, soldier.ServerPosition, target.BoundingRadius + 35))
                    .Any(rectangle => rectangle.IsInside(target));
        }

        internal static bool Ehit(this Obj_AI_Base target, Vector3 pos)
        {
            var targetpos = Prediction.Position.PredictUnitPosition(target, Azir.E.CastDelay);
            return new Geometry.Polygon.Rectangle(Player.Instance.ServerPosition, pos, target.BoundingRadius + 35).IsInside(targetpos);
        }

        public static DangerLevel danger()
        {
            switch (Menus.Auto.combobox("danger"))
            {
                case 0:
                    {
                        return DangerLevel.High;
                    }
                case 1:
                    {
                        return DangerLevel.Medium;
                    }
                case 2:
                    {
                        return DangerLevel.Low;
                    }
            }
            return DangerLevel.Low;
        }

        public static int CountEnemeis(this Obj_AI_Base target, float range)
        {
            return EntityManager.Heroes.Enemies.Count(e => e.IsValidTarget(range, true, target.ServerPosition) && e.IsKillable());
        }

        public static int CountEnemeis(this GameObject target, float range)
        {
            return EntityManager.Heroes.Enemies.Count(e => e.IsValidTarget(range, true, target.Position) && e.IsKillable());
        }

        public static int CountAllies(this Obj_AI_Base target, float range)
        {
            return EntityManager.Heroes.Allies.Count(e => e.IsValidTarget(range, false, target.ServerPosition) && e.IsKillable());
        }

        public static AIHeroClient ally;

        public static Obj_AI_Turret tower;

        public static float Mana()
        {
            var mana = 0f;

            if (Azir.Q.IsReady())
            {
                // Q mana
                mana += Azir.Q.Handle.SData.Mana;
            }

            if (Azir.W.IsReady())
            {
                // W mana
                mana += Azir.W.Handle.SData.Mana;
            }

            if (Azir.E.IsReady())
            {
                // E mana
                mana += Azir.E.Handle.SData.Mana;
            }
            if (Azir.R.IsReady())
            {
                // R mana
                mana += Azir.R.Handle.SData.Mana;
            }

            return mana;
        }

        public static float Mana(this Spell.SpellBase spell)
        {
            return spell.Handle.SData.Mana;
        }

        public static bool IsKillable(this Obj_AI_Base target)
        {
            return !target.HasBuff("kindredrnodeathbuff") && !target.HasBuff("JudicatorIntervention") && !target.HasBuff("ChronoShift")
                   && !target.HasBuff("UndyingRage") && !target.IsInvulnerable && !target.IsZombie && !target.HasBuff("bansheesveil")
                   && !target.IsDead && !target.IsPhysicalImmune && target.Health > 0 && !target.HasBuffOfType(BuffType.Invulnerability)
                   && !target.HasBuffOfType(BuffType.PhysicalImmunity) && target.IsValidTarget();
        }

        public static bool IsCC(this Obj_AI_Base target)
        {
            return target.IsStunned || target.IsRooted || target.IsTaunted || target.IsCharmed || target.Spellbook.IsChanneling || !target.CanMove
                   || target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Knockback) || target.HasBuffOfType(BuffType.Knockup)
                   || target.HasBuffOfType(BuffType.Snare) || target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Suppression)
                   || target.HasBuffOfType(BuffType.Taunt);
        }
    }
}