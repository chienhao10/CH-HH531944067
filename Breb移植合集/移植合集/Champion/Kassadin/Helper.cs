using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;

namespace Kassawin
{
    internal class Helper
    {
        public const string Menuname = "Slutty Kassadin";
        public static Menu Config, comboMenu, harassMenu, farmMenu, ksMenu, drawMenu, miscMenu;
        private static readonly DateTime AssemblyLoadTime = DateTime.Now;

        public static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public static float TickCount
        {
            get { return (int) DateTime.Now.Subtract(AssemblyLoadTime).TotalMilliseconds; }
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

        public static bool ItemReady(int id)
        {
            return Items.CanUseItem(id);
        }

        public static bool HasItem(int id)
        {
            return Items.HasItem(id);
        }

        public static bool UseUnitItem(int item, AIHeroClient target)
        {
            return Items.UseItem(item, target);
        }

        public static bool SelfCast(int item)
        {
            return Items.UseItem(item);
        }

        public static bool PlayerBuff(string name)
        {
            return Player.HasBuff(name);
        }

        public static void PotionCast(int id, string buff)
        {
            if (ItemReady(id) && !PlayerBuff(buff)
                && !Player.IsRecalling() && !Player.InFountain()
                && Player.CountEnemiesInRange(700) >= 1)
            {
                SelfCast(id);
            }
        }

        public static void ElixerCast(int id, string buff)
        {
            if (!PlayerBuff(buff)
                && HasItem(id))
            {
                SelfCast(id);
            }
        }

        public static float SpellRange(SpellSlot spellSlot)
        {
            return Player.Spellbook.GetSpell(spellSlot).SData.CastRange;
        }
    }
}