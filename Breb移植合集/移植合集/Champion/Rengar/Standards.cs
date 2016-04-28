using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using LeagueSharp.Common;

namespace ElRengarRevamped
{
    public class Standards
    {
        #region Public Methods and Operators

        public static float IgniteDamage(AIHeroClient target)
        {
            if (Ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(Ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float) Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        #endregion

        #region Methods

        protected static void SmiteCombo()
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

        #endregion

        #region Static Fields

        private static readonly int[] BlueSmite = {3706, 1400, 1401, 1402, 1403};

        private static readonly int[] RedSmite = {3715, 1415, 1414, 1413, 1412};

        protected static readonly Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>
        {
            {
                Spells.Q,
                new Spell(
                    SpellSlot.Q,
                    Orbwalking.GetRealAutoAttackRange(
                        Player) + 100) // + 150
            },
            {
                Spells.W,
                new Spell(
                    SpellSlot.W,
                    500 + Player.BoundingRadius)
            },
            {
                Spells.E,
                new Spell(
                    SpellSlot.E,
                    1000 + Player.BoundingRadius)
            },
            {Spells.R, new Spell(SpellSlot.R, 2000)}
        };

        public static int LastSwitch;

        protected static SpellSlot Ignite;

        protected static SpellSlot Smite;

        protected static Items.Item Youmuu;

        #endregion

        #region Public Properties

        public static int Ferocity
        {
            get { return (int) ObjectManager.Player.Mana; }
        }

        public static bool HasPassive
        {
            get { return Player.Buffs.Any(x => x.Name.ToLower().Contains("rengarpassivebuff")); }
        }

        public static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public static bool RengarR
        {
            get { return Player.Buffs.Any(x => x.Name.ToLower().Contains("RengarR")); }
        }

        public static string ScriptVersion
        {
            get { return typeof (Rengar).Assembly.GetName().Version.ToString(); }
        }

        #endregion
    }
}