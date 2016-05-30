using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using LeagueSharp.SDK;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;

namespace hJhin.Extensions
{
    class Qss
    {
        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static void ExecuteQss()
        {
            if (getCheckBoxItem(Config.itemMenu, "qss.charm") && ObjectManager.Player.HasBuffOfType(BuffType.Charm))
            {
                if (Items.CanUseItem((int)ItemId.Quicksilver_Sash) && Items.HasItem((int)ItemId.Quicksilver_Sash))
                {
                    Items.UseItem((int)ItemId.Quicksilver_Sash);
                }
                if (Items.CanUseItem((int)ItemId.Mercurial_Scimitar) && Items.HasItem((int)ItemId.Mercurial_Scimitar))
                {
                    Items.UseItem((int)ItemId.Mercurial_Scimitar);
                }
            }
            if (getCheckBoxItem(Config.itemMenu, "qss.snare") && ObjectManager.Player.HasBuffOfType(BuffType.Snare))
            {
                if (Items.CanUseItem((int)ItemId.Quicksilver_Sash) && Items.HasItem((int)ItemId.Quicksilver_Sash))
                {
                    Items.UseItem((int)ItemId.Quicksilver_Sash);
                }
                if (Items.CanUseItem((int)ItemId.Mercurial_Scimitar) && Items.HasItem((int)ItemId.Mercurial_Scimitar))
                {
                    Items.UseItem((int)ItemId.Mercurial_Scimitar);
                }
            }
            if (getCheckBoxItem(Config.itemMenu, "qss.polymorph") && ObjectManager.Player.HasBuffOfType(BuffType.Polymorph))
            {
                if (Items.CanUseItem((int)ItemId.Quicksilver_Sash) && Items.HasItem((int)ItemId.Quicksilver_Sash))
                {
                    Items.UseItem((int)ItemId.Quicksilver_Sash);
                }
                if (Items.CanUseItem((int)ItemId.Mercurial_Scimitar) && Items.HasItem((int)ItemId.Mercurial_Scimitar))
                {
                    Items.UseItem((int)ItemId.Mercurial_Scimitar);
                }
            }
            if (getCheckBoxItem(Config.itemMenu, "qss.stun") && ObjectManager.Player.HasBuffOfType(BuffType.Stun))
            {
                if (Items.CanUseItem((int)ItemId.Quicksilver_Sash) && Items.HasItem((int)ItemId.Quicksilver_Sash))
                {
                    Items.UseItem((int)ItemId.Quicksilver_Sash);
                }
                if (Items.CanUseItem((int)ItemId.Mercurial_Scimitar) && Items.HasItem((int)ItemId.Mercurial_Scimitar))
                {
                    Items.UseItem((int)ItemId.Mercurial_Scimitar);
                }
            }
            if (getCheckBoxItem(Config.itemMenu, "qss.suppression") && ObjectManager.Player.HasBuffOfType(BuffType.Suppression))
            {
                if (Items.CanUseItem((int)ItemId.Quicksilver_Sash) && Items.HasItem((int)ItemId.Quicksilver_Sash))
                {
                    Items.UseItem((int)ItemId.Quicksilver_Sash);
                }
                if (Items.CanUseItem((int)ItemId.Mercurial_Scimitar) && Items.HasItem((int)ItemId.Mercurial_Scimitar))
                {
                    Items.UseItem((int)ItemId.Mercurial_Scimitar);
                }
            }
            if (getCheckBoxItem(Config.itemMenu, "qss.taunt") && ObjectManager.Player.HasBuffOfType(BuffType.Taunt))
            {
                if (Items.CanUseItem((int)ItemId.Quicksilver_Sash) && Items.HasItem((int)ItemId.Quicksilver_Sash))
                {
                    Items.UseItem((int)ItemId.Quicksilver_Sash);
                }
                if (Items.CanUseItem((int)ItemId.Mercurial_Scimitar) && Items.HasItem((int)ItemId.Mercurial_Scimitar))
                {
                    Items.UseItem((int)ItemId.Mercurial_Scimitar);
                }
            }

        }
    }
}
