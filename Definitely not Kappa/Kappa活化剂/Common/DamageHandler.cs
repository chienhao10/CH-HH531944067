namespace KappaUtility.Common
{
    using System;
    using System.Linq;
    using System.Threading;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;

    using Items;

    using Summoners;

    using SharpDX;

    internal class DamageHandler
    {
        internal static void OnLoad()
        {
            try
            {
                Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
                Obj_AI_Base.OnBasicAttack += Obj_AI_Base_OnBasicAttack;
            }
            catch (Exception e)
            {
                Helpers.Log(e.ToString());
            }
        }

        public static void Obj_AI_Base_OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(args.Target is AIHeroClient))
            {
                return;
            }

            var caster = sender;
            var enemy = sender as AIHeroClient;
            var target = (AIHeroClient)args.Target;

            if (!(caster is AIHeroClient || caster is Obj_AI_Turret) || !caster.IsEnemy || target == null || caster == null || !target.IsAlly)
            {
                return;
            }

            var aaprecent = (caster.GetAutoAttackDamage(target, true) / target.TotalShieldHealth()) * 100;
            var death = caster.GetAutoAttackDamage(target, true) >= target.TotalShieldHealth() || aaprecent >= target.HealthPercent;

            if (target.IsAlly && !target.IsMe && target.IsValidTarget())
            {
                Defensive.defcast(caster, target, enemy, aaprecent);
                Spells.summcast(caster, target, enemy, aaprecent);
            }

            if (target.IsMe)
            {
                Defensive.defcast(caster, target, enemy, aaprecent);
                Spells.summcast(caster, target, enemy, aaprecent);
            }
        }

        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(args.Target is AIHeroClient))
            {
                return;
            }

            var caster = sender;
            var enemy = sender as AIHeroClient;
            var target = (AIHeroClient)args.Target;
            var ally = EntityManager.Heroes.Allies.FirstOrDefault(a => a.IsInRange(args.End, 100) && a.IsValidTarget() && !a.IsMe);
            var hitally = ally != null && args.End != Vector3.Zero && args.End.Distance(ally) < 100;
            var hitme = args.End != Vector3.Zero && args.End.Distance(Player.Instance) < 100;

            if (!(caster is AIHeroClient || caster is Obj_AI_Turret) || !caster.IsEnemy || enemy == null || caster == null)
            {
                return;
            }

            if (((target.IsAlly && !target.IsMe) || hitally) && ally != null && ally.IsValidTarget())
            {
                var spelldamageally = enemy.GetSpellDamage(ally, args.Slot);
                var damagepercentally = (spelldamageally / ally.TotalShieldHealth()) * 100;
                var deathally = damagepercentally >= ally.HealthPercent || spelldamageally >= ally.TotalShieldHealth()
                                || caster.GetAutoAttackDamage(ally, true) >= ally.TotalShieldHealth()
                                || enemy.GetAutoAttackDamage(ally, true) >= ally.TotalShieldHealth();

                Defensive.defcast(caster, ally, enemy, spelldamageally);
                Spells.summcast(caster, ally, enemy, spelldamageally);
            }

            if (target.IsMe || hitme)
            {
                var spelldamageme = enemy.GetSpellDamage(Player.Instance, args.Slot);
                var damagepercentme = (spelldamageme / Player.Instance.TotalShieldHealth()) * 100;
                var deathme = damagepercentme >= Player.Instance.HealthPercent || spelldamageme >= Player.Instance.TotalShieldHealth()
                              || caster.GetAutoAttackDamage(Player.Instance, true) >= Player.Instance.TotalShieldHealth()
                              || enemy.GetAutoAttackDamage(Player.Instance, true) >= Player.Instance.TotalShieldHealth();

                Defensive.defcast(caster, Player.Instance, enemy, spelldamageme);
                Spells.summcast(caster, Player.Instance, enemy, spelldamageme);
            }
        }
    }
}