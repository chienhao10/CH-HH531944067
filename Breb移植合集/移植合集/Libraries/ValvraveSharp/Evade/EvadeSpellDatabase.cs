namespace Valvrave_Sharp.Evade
{
    #region

    using System.Collections.Generic;

    using LeagueSharp;

    #endregion
    using EloBuddy;
    internal class EvadeSpellDatabase
    {
        #region Static Fields

        public static List<EvadeSpellData> Spells = new List<EvadeSpellData>();

        #endregion

        #region Constructors and Destructors

        static EvadeSpellDatabase()
        {
            EvadeSpellData spell;

            #region Champion Dashes

            #region Yasuo

            if (Program.Player.ChampionName == "Yasuo")
            {
                spell = new DashData("Yasuo E", SpellSlot.E, 475, true, 10, 1000, 2)
                {
                                ValidTargets = new[] { SpellValidTargets.EnemyChampions, SpellValidTargets.EnemyMinions },
                                CheckBuffName = "YasuoDashWrapper", UnderTower = true
                            };
                Spells.Add(spell);
            }

            #endregion

            #endregion

            #region Champion Blinks

            #region Zed

            if (Program.Player.ChampionName == "Zed")
            {
                spell = new BlinkData("Zed W2", SpellSlot.W, 20000, 50, 3)
                            { CheckSpellName = "zedw2", SelfCast = true, UnderTower = true };
                Spells.Add(spell);
                spell = new BlinkData("Zed R1", SpellSlot.R, 625, 50, 4)
                            { CheckSpellName = "zedr", ValidTargets = new[] { SpellValidTargets.EnemyChampions } };
                Spells.Add(spell);
                spell = new BlinkData("Zed R2", SpellSlot.R, 20000, 50, 4)
                            { CheckSpellName = "zedr2", SelfCast = true, UnderTower = true };
                Spells.Add(spell);
            }

            #endregion

            #endregion

            #region Champion Invulnerabilities

            #region Yasuo

            if (Program.Player.ChampionName == "Yasuo")
            {
                spell = new InvulnerabilityData("Yasuo W", SpellSlot.W, 250, 3) { ExtraDelay = true };
                Spells.Add(spell);
            }

            #endregion

            #endregion
        }

        #endregion
    }
}