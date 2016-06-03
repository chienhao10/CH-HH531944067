namespace Snitched
{
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using LeagueSharp.Data;
    using LeagueSharp.Data.DataTypes;
    using LeagueSharp.Data.Enumerations;

    using CollisionableObjects = LeagueSharp.Data.Enumerations.CollisionableObjects;
    using SkillshotType = LeagueSharp.Common.SkillshotType;
    using Spell = LeagueSharp.Common.Spell;
    using EloBuddy;
    /// <summary>
    ///     Gets spells that can be used for Snitched.
    /// </summary>
    internal static class SpellLoader
    {
        #region Static Fields

        /// <summary>
        ///     The disallowed spelltags
        /// </summary>
        private static readonly SpellTags[] DisallowedSpelltags = { SpellTags.Blink, SpellTags.Dash };

        /// <summary>
        ///     The whitelisted spells
        /// </summary>
        private static readonly string[] WhitelistedSpells = { "EzrealTrueshotBarrage" };

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the missile arrival time in seconds.
        /// </summary>
        /// <param name="spell">The spell.</param>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public static float GetMissileArrivalTime(this Spell spell, Obj_AI_Base target)
        {
            return target.LSDistance(ObjectManager.Player) / spell.Speed + spell.Delay + Game.Ping / 2f / 1000;
        }

        /// <summary>
        ///     Gets the usable spells.
        /// </summary>
        /// <returns></returns>
        public static List<Spell> GetUsableSpells()
        {
            var slots = new[] { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R };
            var usableSpells = new List<Spell>();

            foreach (var spellEntry in
                slots.Select(x => Data.Get<SpellDatabase>().GetByName(ObjectManager.Player.GetSpell(x).SData.Name.ToLower()))
                    .Where(x => x != null)
                    .Where(
                        x =>
                        x.SpellTags != null && x.CastType != null && x.CollisionObjects != null
                        && (x.SpellTags.Any(y => y == SpellTags.Damage)
                            && !x.SpellTags.Any(y => DisallowedSpelltags.Any(z => y == z))
                            && x.CastType.Any(y => y == CastType.Position))
                        || WhitelistedSpells.Any(y => y.Equals(x.MissileSpellName))))
            {
                var spell = new Spell(spellEntry.Slot, spellEntry.Range);

                switch (spellEntry.SpellType)
                {
                    case SpellType.SkillshotMissileCircle:
                    case SpellType.SkillshotRing:
                    case SpellType.SkillshotCircle:
                    case SpellType.SkillshotArc:
                    case SpellType.SkillshotMissileArc:
                        spell.SetSkillshot(
                            spellEntry.Delay / 1000f,
                            spellEntry.Radius,
                            spellEntry.MissileSpeed,
                            spellEntry.CollisionObjects.Any(x => x != CollisionableObjects.YasuoWall),
                            SkillshotType.SkillshotCircle);
                        break;
                    case SpellType.SkillshotMissileLine:
                    case SpellType.SkillshotLine:
                        spell.SetSkillshot(
                            spellEntry.Delay / 1000f,
                            spellEntry.Width,
                            spellEntry.MissileSpeed,
                            spellEntry.CollisionObjects.Any(x => x != CollisionableObjects.YasuoWall),
                            SkillshotType.SkillshotLine);
                        break;
                    case SpellType.SkillshotCone:
                    case SpellType.SkillshotMissileCone:
                        spell.SetSkillshot(
                            spellEntry.Delay / 1000f,
                            spellEntry.Radius,
                            spellEntry.MissileSpeed,
                            spellEntry.CollisionObjects.Any(x => x != CollisionableObjects.YasuoWall),
                            SkillshotType.SkillshotLine);
                        break;
                    case SpellType.Targeted:
                    case SpellType.TargetedMissile:
                        spell.SetTargetted(spellEntry.Delay / 1000f, spellEntry.MissileSpeed);
                        break;
                    case SpellType.Position:
                        spell.SetSkillshot(
                            spellEntry.Delay / 1000f,
                            spellEntry.Width,
                            spellEntry.MissileSpeed,
                            spellEntry.CollisionObjects.Any(x => x != CollisionableObjects.YasuoWall),
                            SkillshotType.SkillshotLine);
                        break;
                    default:
                        continue;
                }

                usableSpells.Add(spell);
            }

            return usableSpells;
        }

        #endregion
    }
}