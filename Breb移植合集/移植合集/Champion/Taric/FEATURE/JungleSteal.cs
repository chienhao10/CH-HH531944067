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
    internal class JungleSteal
    {
        #region #GET
        private static AIHeroClient Player
        {
            get
            {
                return SkyLv_Taric.Player;
            }
        }

        private static LeagueSharp.Common.Spell Q
        {
            get
            {
                return SkyLv_Taric.Q;
            }
        }

        private static LeagueSharp.Common.Spell W
        {
            get
            {
                return SkyLv_Taric.W;
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

        static JungleSteal()
        {
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            JungleKS();
        }

        public static void JungleKS()
        {
            if (getCheckBoxItem(SkyLv_Taric.JungleClear, "Taric.JungleKS"))
            {
                var UseAAJungleKS = getCheckBoxItem(SkyLv_Taric.JungleClear, "Taric.UseAAJungleKS");
                var UseEJungleKS = getCheckBoxItem(SkyLv_Taric.JungleClear, "Taric.UseEJungleKS");
                var PacketCast = getCheckBoxItem(SkyLv_Taric.Misc, "Taric.UsePacketCast");

                if (Player.IsRecalling()) return;

                foreach (var target in ObjectManager.Get<Obj_AI_Base>().Where(target => SkyLv_Taric.Monsters.Contains(target.BaseSkinName) && !target.IsDead))
                {
                    if (UseAAJungleKS && Orbwalker.CanAutoAttack && Player.GetAutoAttackDamage(target) > target.Health && target.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Player)))
                    {
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                    }

                    if (UseEJungleKS && E.GetDamage(target) > target.Health && Player.LSDistance(target) <= E.Range && Player.Mana >= E.ManaCost)
                    {
                        E.CastIfHitchanceEquals(target, HitChance.VeryHigh, PacketCast);
                    }
                }
            }
        }
    }
}
