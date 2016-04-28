using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK.Menu;
using LeagueSharp.Common;

namespace JaxQx
{
    internal class PlayerSpells
    {
        public static SpellSlot SmiteSlot = SpellSlot.Unknown;
        public static SpellSlot IgniteSlot = SpellSlot.Unknown;
        private static readonly int[] SmitePurple = {3713, 3726, 3725, 3726, 3723};
        private static readonly int[] SmiteGrey = {3711, 3722, 3721, 3720, 3719};
        private static readonly int[] SmiteRed = {3715, 3718, 3717, 3716, 3714};
        private static readonly int[] SmiteBlue = {3706, 3710, 3709, 3708, 3707};

        private static string Smitetype
        {
            get
            {
                if (SmiteBlue.Any(i => LeagueSharp.Common.Items.HasItem(i)))
                    return "s5_summonersmiteplayerganker";

                if (SmiteRed.Any(i => LeagueSharp.Common.Items.HasItem(i)))
                    return "s5_summonersmiteduel";

                if (SmiteGrey.Any(i => LeagueSharp.Common.Items.HasItem(i)))
                    return "s5_summonersmitequick";

                if (SmitePurple.Any(i => LeagueSharp.Common.Items.HasItem(i)))
                    return "itemsmiteaoe";

                return "summonersmite";
            }
        }

        public static void Initialize()
        {
            SetSmiteSlot();
            SetIgniteSlot();
        }

        private static void SetSmiteSlot()
        {
            foreach (
                var spell in
                    Program.Player.Spellbook.Spells.Where(
                        spell => string.Equals(spell.Name, Smitetype, StringComparison.CurrentCultureIgnoreCase)))
            {
                SmiteSlot = spell.Slot;
            }
        }

        private static void SetIgniteSlot()
        {
            IgniteSlot = Program.Player.GetSpellSlot("SummonerDot");
        }
    }
}