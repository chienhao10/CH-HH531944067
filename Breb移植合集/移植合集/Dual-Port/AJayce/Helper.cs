using System;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;

namespace Jayce
{
    internal class Helper
    {
        public static AIHeroClient Player { get { return ObjectManager.Player; } }
        private static readonly DateTime AssemblyLoadTime = DateTime.Now;
        public const string Menuname = "Slutty Jayce";

        public static float TickCount
        {
            get { return (int)DateTime.Now.Subtract(AssemblyLoadTime).TotalMilliseconds; }
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
                && !Player.LSIsRecalling() && !Player.InFountain()
                && Player.LSCountEnemiesInRange(700) >= 1)
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
