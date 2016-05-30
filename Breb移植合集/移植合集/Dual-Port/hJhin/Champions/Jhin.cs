using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hJhin.Extensions;
using hJhin.Modes;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace hJhin.Champions
{
    public class Jhin
    {
        public Jhin()
        {
            JhinOnLoad();
        }

        public static void JhinOnLoad()
        {
            Spells.Initialize();
            Config.ExecuteMenu();

            Game.OnUpdate += OnUpdate;
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

        private static void OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo.Execute();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                Clear.Execute();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Jungle.Execute();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass.Execute();
            }

           
            if (ObjectManager.Player.IsActive(Spells.R))
            {
                Orbwalker.DisableAttacking = true;
                Orbwalker.DisableMovement = true;
            }
            else if (!ObjectManager.Player.IsActive(Spells.R))
            {
                Orbwalker.DisableAttacking = false;
                Orbwalker.DisableMovement = false;
            }

            if (getKeyBindItem(Config.SemiManualUlt, "semi.manual.ult") && !ObjectManager.Player.IsActive(Spells.R))
            {
                Orbwalker.OrbwalkTo(Game.CursorPos);
            }

            if (Spells.R.IsReady() && getKeyBindItem(Config.SemiManualUlt, "semi.manual.ult"))
            {
                Ultimate.Execute();
            }
           

            if (getCheckBoxItem(Config.itemMenu, "use.qss") && (Items.HasItem((int)ItemId.Quicksilver_Sash) && Items.CanUseItem((int)ItemId.Quicksilver_Sash) ||
                Items.CanUseItem(3139) && Items.HasItem(3137)))
            {
                Qss.ExecuteQss();
            }

            if (getCheckBoxItem(Config.miscMenu, "auto.orb.buy") && ObjectManager.Player.Level >= getSliderItem(Config.miscMenu, "orb.level")
                && !Items.HasItem((int)ItemId.Farsight_Alteration))
            {
                Shop.BuyItem(ItemId.Farsight_Alteration);
            }
        }
    }
}
