using EloBuddy;
using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nechrito_Nidalee.Extras
{ 
    // ITEMS | Smite
    class Item : Core
    {
        public static SpellSlot Smite;
        private static readonly int[] BlueSmite = { 3706, 1400, 1401, 1402, 1403 };
        private static readonly int[] RedSmite = { 3715, 1415, 1414, 1413, 1412 };

        public static void SmiteCombo()
        {
            if (BlueSmite.Any(id => Items.HasItem(id)))
            {
                Smite = Player.GetSpellSlot("s5_summonersmiteplayerganker");
                return;
            }
            if (RedSmite.Any(id => Items.HasItem(id)))
            {
                Smite = Player.GetSpellSlot("s5_summonersmiteduel");
                return;
            }
            Smite = Player.GetSpellSlot("summonersmite");
        }
        public static void SmiteJungle()
        {
            foreach (var minion in MinionManager.GetMinions(900f, MinionTypes.All, MinionTeam.Neutral))
            {
                var StealDmg = Player.Spellbook.GetSpell(Smite).State == SpellState.Ready
                    ? (float)Player.GetSummonerSpellDamage(minion, Damage.SummonerSpell.Smite)
                    : 0;
                if (minion.LSDistance(Player.ServerPosition) <= 550)
                {
                    if ((minion.CharData.BaseSkinName.Contains("Dragon") || minion.CharData.BaseSkinName.Contains("Baron")))
                    {
                        if (StealDmg >= minion.Health)
                            Player.Spellbook.CastSpell(Smite, minion);
                    }
                }
            }
        }
    }
}
