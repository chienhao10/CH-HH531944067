namespace SkyLv_Taric
{
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    internal class Interrupter
    {
        #region #GET

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

        private static AIHeroClient Player
        {
            get
            {
                return SkyLv_Taric.Player;
            }
        }

        private static Spell E
        {
            get
            {
                return SkyLv_Taric.E;
            }
        }
        #endregion

        static Interrupter()
        {
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            var PacketCast = getCheckBoxItem(SkyLv_Taric.Misc, "Taric.UsePacketCast");

            if (Player.LSIsRecalling()) return;

            if (getCheckBoxItem(SkyLv_Taric.Misc, "Taric.AutoEInterrupt") && E.IsReady() && sender.LSIsValidTarget(E.Range))
                E.Cast(sender, PacketCast);

            if (getCheckBoxItem(SkyLv_Taric.Combo, "Taric.UseEFromAlly") && E.IsReady() && Player.Mana >= E.ManaCost)
            {
                foreach (var AllyHero in ObjectManager.Get<AIHeroClient>().Where(a => !a.IsMe && !a.IsDead && a.Team == ObjectManager.Player.Team && Player.LSDistance(a) < 1600 && (a.HasBuff("TaricWAllyBuff") || a.HasBuff("TaricW"))))
                {
                    var Allytarget = ObjectManager.Get<AIHeroClient>().Where(t => !t.IsDead && t.Team != ObjectManager.Player.Team && AllyHero.LSDistance(t) < E.Range).FirstOrDefault();

                    if (getCheckBoxItem(SkyLv_Taric.Combo, AllyHero.NetworkId + "TargetInterruptEComboFromAlly") && Allytarget.NetworkId == sender.NetworkId)
                    {
                        E.Cast(sender.ServerPosition, PacketCast);
                        return;
                    }
                }
            }
        }
    }
}
