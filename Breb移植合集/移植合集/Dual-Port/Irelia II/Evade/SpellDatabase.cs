// Copyright 2014 - 2014 Esk0r
// SpellDatabase.cs is part of Evade.
// 
// Evade is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Evade is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Evade. If not, see <http://www.gnu.org/licenses/>.

#region
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using EloBuddy;
#endregion

namespace Irelia.Evade
{
    public static class SpellDatabase
    {
        public static List<SpellData> Spells = new List<SpellData>();

        static SpellDatabase()
        {
            //Add spells to the database 
            //LeonaSunLight

            #region TargetedSkills

            #region Kayle

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Kayle",
                    SpellName = "KayleQ",
                    Slot = SpellSlot.Q,
                    Type = SpellData.SkillShotType.SkillshotTargeted,
                    DangerValue = 5,
                    IsDangerous = true,
                });

            #endregion Kayle

            #region Tristana

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Tristana",
                    SpellName = "TristanaR",
                    Slot = SpellSlot.R,
                    Type = SpellData.SkillShotType.SkillshotTargeted,
                    DangerValue = 5,
                    IsDangerous = true,
                });

            #endregion Tristana

            #region LeeSin

            Spells.Add(
                new SpellData
                {
                    ChampionName = "LeeSin",
                    SpellName = "LeeSinR",
                    Slot = SpellSlot.R,
                    Type = SpellData.SkillShotType.SkillshotTargeted,
                    DangerValue = 5,
                    IsDangerous = true,
                });

            #endregion LeeSin

            #region Nasus

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Nasus",
                    SpellName = "NasusW",
                    Slot = SpellSlot.W,
                    Type = SpellData.SkillShotType.SkillshotTargeted,
                    DangerValue = 5,
                    IsDangerous = true,
                });

            #endregion Nasus

            #region Alistar

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Alistar",
                    SpellName = "AlistarW",
                    Slot = SpellSlot.W,
                    Type = SpellData.SkillShotType.SkillshotTargeted,
                    DangerValue = 5,
                    IsDangerous = true,
                });

            #endregion Alistar

            #region FiddleStick

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Fiddlesticks",
                    SpellName = "FiddlesticksQ",
                    Slot = SpellSlot.Q,
                    Type = SpellData.SkillShotType.SkillshotTargeted,
                    DangerValue = 5,
                    IsDangerous = true,
                });

            #endregion FiddleStick

            #region Nautilius

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Nautilius",
                    SpellName = "NautiliusR",
                    Slot = SpellSlot.R,
                    Type = SpellData.SkillShotType.SkillshotTargeted,
                    DangerValue = 5,
                    IsDangerous = true,
                });

            #endregion Nautilius

            #region Skarner

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Skarner",
                    SpellName = "SkarnerR",
                    Slot = SpellSlot.R,
                    Type = SpellData.SkillShotType.SkillshotTargeted,
                    DangerValue = 5,
                    IsDangerous = true,
                });

            #endregion Skarner

            #region Warwick

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Warwick",
                    SpellName = "WarwickR",
                    Slot = SpellSlot.R,
                    Type = SpellData.SkillShotType.SkillshotTargeted,
                    DangerValue = 5,
                    IsDangerous = true,
                });

            #endregion Malzahar

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Malzahar",
                    SpellName = "MalzaharR",
                    Slot = SpellSlot.R,
                    Type = SpellData.SkillShotType.SkillshotTargeted,
                    DangerValue = 5,
                    IsDangerous = true,
                });

            #endregion Warwick

            #region Vayne

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Vayne",
                    SpellName = "VayneE",
                    Slot = SpellSlot.E,
                    Type = SpellData.SkillShotType.SkillshotTargeted,
                    DangerValue = 5,
                    IsDangerous = true,
                });

            #endregion Vayne

            #region Poppy

            Spells.Add(
                new SpellData
                {
                    ChampionName = "Poppy",
                    SpellName = "PoppyE",
                    Slot = SpellSlot.E,
                    Type = SpellData.SkillShotType.SkillshotTargeted,
                    DangerValue = 5,
                    IsDangerous = true,
                });

            #endregion Poppy
        }

        public static SpellData GetBySourceObjectName(string objectName)
        {
            objectName = objectName.ToLowerInvariant();
            return
                Spells.Where(spellData => spellData.SourceObjectName.Length != 0)
                    .FirstOrDefault(spellData => objectName.Contains(spellData.SourceObjectName));
        }

        public static SpellData GetByName(string spellName)
        {
            spellName = spellName.ToLower();
            return
                Spells.FirstOrDefault(
                    spellData =>
                        spellData.SpellName.ToLower() == spellName || spellData.ExtraSpellNames.Contains(spellName));
        }

        public static SpellData GetByMissileName(string missileSpellName)
        {
            missileSpellName = missileSpellName.ToLower();
            return
                Spells.FirstOrDefault(
                    spellData =>
                        spellData.MissileSpellName != null && spellData.MissileSpellName.ToLower() == missileSpellName ||
                        spellData.ExtraMissileNames.Contains(missileSpellName));
        }

        public static SpellData GetBySpeed(string ChampionName, int speed, int id = -1)
        {
            return
                Spells.FirstOrDefault(
                    spellData =>
                        spellData.ChampionName == ChampionName && spellData.MissileSpeed == speed &&
                        (spellData.Id == -1 || id == spellData.Id));
        }
    }
}
