namespace KappaEkko.Logics
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;

    internal class Rlogic
    {
        public static void Aoe()
        {
            var RAoe = Menu.UltMenu["RAoeh"].Cast<Slider>().CurrentValue;
            var Enemies = Spells.EkkoREmitter.Position.CountEnemiesInRange(400);

            if (Enemies >= RAoe && Spells.EkkoREmitter != null)
            {
                Spells.R.Cast();
            }
        }

        public static void Combo()
        {
            var Rhit = Menu.ComboMenu["Rhit"].Cast<Slider>().CurrentValue;
            var Enemies = Spells.EkkoREmitter.Position.CountEnemiesInRange(400);

            if (Enemies >= Rhit)
            {
                Spells.R.Cast();
            }
        }

        public static void Rk()
        {
            var Rks =
                ObjectManager.Get<AIHeroClient>()
                    .FirstOrDefault(
                        enemy =>
                        enemy != null && !enemy.IsZombie && enemy.IsEnemy && enemy.IsInRange(Spells.EkkoREmitter.Position, Spells.R.Range)
                        && !enemy.IsDead && !enemy.HasBuff("kindredrnodeathbuff") && !enemy.HasBuff("JudicatorIntervention")
                        && !enemy.HasBuff("ChronoShift") && !enemy.HasBuff("UndyingRage")
                        && enemy.TotalShieldHealth() < ObjectManager.Player.GetSpellDamage(enemy, SpellSlot.R));
            if (Rks != null)
            {
                Spells.R.Cast();
            }
        }

        public static void Escape()
        {
            var REscapeh = Menu.UltMenu["REscapeh"].Cast<Slider>().CurrentValue;
            var Health = ObjectManager.Player.HealthPercent;
            var Enemies = Spells.EkkoREmitter.Position.CountEnemiesInRange(450);
            var EnemiesMy = ObjectManager.Player.Position.CountEnemiesInRange(800);

            if (REscapeh >= Health && Enemies < EnemiesMy)
            {
                Spells.R.Cast();
            }
        }
    }
}