namespace SkyLv_Taric
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    internal class KillSteal
    {
        #region #GET
        private static AIHeroClient Player
        {
            get
            {
                return SkyLv_Taric.Player;
            }
        }

        private static LeagueSharp.Common.Spell E
        {
            get
            {
                return SkyLv_Taric.E;
            }
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
        #endregion


        static KillSteal()
        {
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            KS();
        }

        public static void KS()
        {
            var PacketCast = getCheckBoxItem(SkyLv_Taric.Misc, "Taric.UsePacketCast");
            var UseIgniteKS = getCheckBoxItem(SkyLv_Taric.Combo, "Taric.UseIgniteKS");
            var UseAAKS = getCheckBoxItem(SkyLv_Taric.Combo, "Taric.UseAAKS");
            var UseEKS = getCheckBoxItem(SkyLv_Taric.Combo, "Taric.UseEKS");

            foreach (var target in ObjectManager.Get<AIHeroClient>().Where(target => !target.IsMe && !target.IsDead && target.Team != ObjectManager.Player.Team && !target.IsZombie && (SkyLv_Taric.Ignite.Slot != SpellSlot.Unknown || !target.HasBuff("summonerdot"))))
            {
                if (UseAAKS && Orbwalker.CanAutoAttack && Player.GetAutoAttackDamage(target) > target.Health && target.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Player)))
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                }

                if (UseEKS && E.GetDamage(target) > target.Health && Player.LSDistance(target) <= E.Range && Player.Mana >= E.ManaCost)
                {
                    E.CastIfHitchanceEquals(target, HitChance.VeryHigh, PacketCast);
                }

                if (UseIgniteKS && SkyLv_Taric.Ignite.Slot != SpellSlot.Unknown && target.Health < Player.GetSummonerSpellDamage(target, LeagueSharp.Common.Damage.SummonerSpell.Ignite) && Player.LSDistance(target) <= SkyLv_Taric.Ignite.Range)
                {
                    SkyLv_Taric.Ignite.Cast(target, PacketCast);
                }
            }
        }

    }
}
