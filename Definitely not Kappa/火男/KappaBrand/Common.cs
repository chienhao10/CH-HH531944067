namespace KappaBrand
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Menu;

    public static class Common
    {
        public static bool Qmana = ObjectManager.Player.Mana > Brand.Q.Handle.SData.Mana;

        public static bool Wmana = ObjectManager.Player.Mana > Brand.W.Handle.SData.Mana;

        public static bool Emana = ObjectManager.Player.Mana > Brand.E.Handle.SData.Mana;

        public static bool Rmana = ObjectManager.Player.Mana > Brand.R.Handle.SData.Mana;

        public static DangerLevel danger()
        {
            switch (Brand.Auto.combobox("Danger"))
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

        public static int CountEnemeis(float range, Obj_AI_Base target)
        {
            return EntityManager.Heroes.Enemies.Count(e => e.IsValidTarget(range, true, target.ServerPosition) && e.IsKillable());
        }

        public static bool orbmode(Orbwalker.ActiveModes mode)
        {
            return Orbwalker.ActiveModesFlags.HasFlag(mode);
        }

        public static bool Mana(this Spell.SpellBase spell, Menu m)
        {
            return ObjectManager.Player.ManaPercent > m.slider(spell.Slot + "Mana");
        }

        public static bool brandpassive(this Obj_AI_Base target)
        {
            return target.HasBuff("BrandAblaze");
        }

        public static int countpassive(this Obj_AI_Base target)
        {
            return target.GetBuffCount("BrandAblaze");
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