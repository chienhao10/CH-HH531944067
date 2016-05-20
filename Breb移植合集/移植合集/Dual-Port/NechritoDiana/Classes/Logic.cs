using ClipperLib;
using Color = System.Drawing.Color;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK;
using EloBuddy;
using Font = SharpDX.Direct3D9.Font;
using LeagueSharp.Common.Data;
using LeagueSharp.Common;
using SharpDX.Direct3D9;
using SharpDX;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Security.AccessControl;
using System;
using System.Speech.Synthesis;

namespace Nechrito_Diana
{
    class Logic
    {
        public static AIHeroClient Player => ObjectManager.Player;
        private static readonly int[] BlueSmite = { 3706, 1400, 1401, 1402, 1403 };

        private static readonly int[] RedSmite = { 3715, 1415, 1414, 1413, 1412 };
        public static SpellSlot Smite;
        public static void CastHydra()
        {
            if (LeagueSharp.Common.Data.ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady())
                LeagueSharp.Common.Data.ItemData.Ravenous_Hydra_Melee_Only.GetItem().Cast();
            else if (LeagueSharp.Common.Data.ItemData.Tiamat_Melee_Only.GetItem().IsReady())
                LeagueSharp.Common.Data.ItemData.Tiamat_Melee_Only.GetItem().Cast();
        }
        public static void CastYoumoo()
        {
            if (LeagueSharp.Common.Data.ItemData.Youmuus_Ghostblade.GetItem().IsReady()) LeagueSharp.Common.Data.ItemData.Youmuus_Ghostblade.GetItem().Cast();
        }
        public static bool HasItem() => LeagueSharp.Common.Data.ItemData.Tiamat_Melee_Only.GetItem().IsReady() || LeagueSharp.Common.Data.ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady();

        // Thanks jQuery for letting me use this! Great guy.
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
                    ? (float)Player.GetSummonerSpellDamage(minion, LeagueSharp.Common.Damage.SummonerSpell.Smite)
                    : 0;
                if (minion.LSDistance(Player.ServerPosition) <= 550)
                {
                    if ((minion.CharData.BaseSkinName.Contains("Dragon") || minion.CharData.BaseSkinName.Contains("Baron")))
                    {
                        if (StealDmg >= minion.Health)
                            Player.Spellbook.CastSpell(Smite, minion);
                    }
                }
                else if (minion.LSDistance(Player.ServerPosition) <= 850)
                    {
                        if ((minion.CharData.BaseSkinName.Contains("Dragon") || minion.CharData.BaseSkinName.Contains("Baron")))
                        {
                        var QRDmg = Spells._q.IsReady() && Spells._r.IsReady()
                   ? Spells._q.GetDamage(minion) + Spells._r.GetDamage(minion)
                   : 0;
                        if(QRDmg >= minion.Health)
                        {
                            Spells._q.Cast(minion);
                            Spells._r.Cast(minion);
                        }
                        if (QRDmg + StealDmg >= minion.Health)
                        {
                            Spells._q.Cast(minion);
                            Spells._r.Cast(minion);
                            Player.Spellbook.CastSpell(Smite, minion);
                        }       
                      }
               }
            }
        }
    }
}
