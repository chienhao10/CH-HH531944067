#region License
/* Copyright (c) LeagueSharp 2016
 * No reproduction is allowed in any way unless given written consent
 * from the LeagueSharp staff.
 * 
 * Author: LeagueSharp.Common Developers
 * Date: 2/21/2016
 * File: Misc.cs
 */
#endregion License

using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.SDK;
using SharpDX;
using SharpDX.Text;
using Color = System.Drawing.Color;
using EloBuddy;

namespace Challenger_Series.Utils
{
    public static class Misc
    {
        private static Random _rand = new Random();

        /// <summary>
        ///     Returns true if the unit is under tower range.
        /// </summary>
        public static bool UnderTurret(this Obj_AI_Base unit)
        {
            return UnderTurret(unit.Position, true);
        }

        /// <summary>
        ///     Returns if the spell is ready to use.
        /// </summary>
        public static bool IsReady(this SpellDataInst spell, int t = 0)
        {
            return spell != null && spell.Slot != SpellSlot.Unknown && t == 0
                       ? spell.State == SpellState.Ready
                       : (spell.State == SpellState.Ready
                          || (spell.State == SpellState.Cooldown && (spell.CooldownExpires - Game.Time) <= t / 1000f));
        }

        public static bool IsReady(this SpellSlot slot, int t = 0)
        {
            var s = ObjectManager.Player.Spellbook.GetSpell(slot);
            return s != null && IsReady(s, t);
        }

        public static bool IsReady(this LeagueSharp.Common.Spell spell, int t = 0)
        {
            return IsReady(spell.Instance, t);
        }

        /// <summary>
        ///     Returns true if the unit is under turret range.
        /// </summary>
        public static bool UnderTurret(this Obj_AI_Base unit, bool enemyTurretsOnly)
        {
            return UnderTurret(unit.Position, enemyTurretsOnly);
        }

        public static Vector3 RandomizeToVector3(this Vector2 position, int min, int max)
        {
            return new Vector2(position.X + _rand.Next(min, max), position.Y + _rand.Next(min, max)).ToVector3();
        }

        public static Vector3 Randomize(this Vector3 position, int min, int max)
        {
            return new Vector2(position.X + _rand.Next(min, max), position.Y + _rand.Next(min, max)).ToVector3();
        }

        public static int GiveRandomInt(int min, int max)
        {
            return _rand.Next(min, max);
        }

        public static bool UnderTurret(this Vector3 position, bool enemyTurretsOnly)
        {
            return
                ObjectManager.Get<Obj_AI_Turret>().Any(turret => turret.LSIsValidTarget(950, enemyTurretsOnly, position));
        }
        public static bool UnderAllyTurret(this Obj_AI_Base unit)
        {
            return UnderAllyTurret(unit.ServerPosition);
        }

        public static bool UnderAllyTurret(this Vector3 position)
        {
            return
                GameObjects.Get<Obj_AI_Turret>()
                    .Any(turret => turret.Position.Distance(position) < 950 && turret.IsAlly && turret.Health > 1);
        }
    }
}
