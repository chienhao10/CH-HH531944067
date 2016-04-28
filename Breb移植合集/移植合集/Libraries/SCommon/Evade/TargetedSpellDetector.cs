using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using SCommon.Database;
using SpellData = SCommon.Database.SpellData;

namespace SCommon.Evade
{
    public static class TargetedSpellDetector
    {
        /// <summary>
        ///     OnDeceted Event delegate
        /// </summary>
        /// <param name="args">The args.</param>
        public delegate void dOnDetected(DetectedTargetedSpellArgs args);

        /// <summary>
        ///     Initializes TargetedSpellDetector class
        /// </summary>
        static TargetedSpellDetector()
        {
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            if (SpellDatabase.TargetedSpells == null)
                SpellDatabase.InitalizeSpellDatabase();
        }

        /// <summary>
        ///     The event which fired when targeted spell is detected
        /// </summary>
        public static event dOnDetected OnDetected;

        /// <summary>
        ///     OnProcessSpellCast Event which detects targeted spells to me
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (OnDetected != null && sender.IsChampion() && !sender.IsMe)
            {
                var spells =
                    SpellDatabase.TargetedSpells.Where(p => p.ChampionName == (sender as AIHeroClient).ChampionName);
                if (spells != null && spells.Count() > 0)
                {
                    var spell = spells.Where(p => p.SpellName == args.SData.Name).FirstOrDefault();
                    if (spell != null)
                    {
                        if ((spell.IsTargeted && args.Target != null && args.Target.IsMe) ||
                            (!spell.IsTargeted && sender.Distance(ObjectManager.Player.ServerPosition) <= spell.Radius))
                            OnDetected(new DetectedTargetedSpellArgs
                            {
                                Caster = sender,
                                SpellData = spell,
                                SpellCastArgs = args
                            });
                    }
                }
            }
        }
    }

    /// <summary>
    ///     DetectedTargetedSpellArgs class
    /// </summary>
    public class DetectedTargetedSpellArgs : EventArgs
    {
        public Obj_AI_Base Caster;
        public GameObjectProcessSpellCastEventArgs SpellCastArgs;
        public SpellData SpellData;
    }
}