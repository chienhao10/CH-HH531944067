using System;
using System.Linq;
using AutoSharp.Utils;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Geometry = AutoSharp.Utils.Geometry;

// ReSharper disable RedundantDefaultMemberInitializer

namespace AutoSharp.Auto.SummonersRift.Role
{
    public static class Support
    {
        private static Obj_AI_Hero _carry;
        private static Obj_AI_Hero _tempCarry;
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private static Obj_AI_Hero _bot = Heroes.Player;
        private static int _timeSinceLastCarrySwitch = 0;
        private static Vector3 _followPos;
        private static int _timeSinceLastFollowPosUpdate = 0;

        #region RoleManager
        //if true will pause role, not unload it!
        // ReSharper disable once RedundantDefaultMemberInitializer
        private static bool _paused = false;
        public static Enums.BehaviorStates State = Enums.BehaviorStates.Unknown;

        /// <summary>
        /// Loads role and sets it's state to running.
        /// </summary>
        public static void Load()
        {
            State = Enums.BehaviorStates.Running;
            Game.OnUpdate += OnUpdate;
        }

        /// <summary>
        /// Unloads role and sets it's state back to stopped.
        /// </summary>
        public static void Unload()
        {
            State = Enums.BehaviorStates.Stopped;
            Game.OnUpdate -= OnUpdate;
        }

        /// <summary>
        /// Pauses a role and sets it's state to paused.
        /// </summary>
        public static void Pause()
        {
            State = Enums.BehaviorStates.Paused;
            _paused = true;
        }

        /// <summary>
        /// Resumes a role, and sets it's state to running.
        /// </summary>
        public static void Resume()
        {
            State = Enums.BehaviorStates.Running;
            _paused = false;
        }

        /// <summary>
        /// returns wether the role is paused or not
        /// </summary>
        public static bool IsPaused { get { return _paused; } }

        #endregion

        public static void OnUpdate(EventArgs args)
        {
            //stop orbwalker if dead
            if (_bot.IsDead)
            {
                Program.Orbwalker.ActiveMode = MyOrbwalker.OrbwalkingMode.None;
                return;
            }

            if (_paused) return;

            if (_bot.Level < 7) // support the adc
            {
                _carry = MyTeam.ADC;
                if (_carry == null)
                {
                    FindCarry();
                    return;
                }
            }
            else //if >lvl 7, support the most fed teammate
            {
                SmartCarrySwitch();
            }
            UpdateFollowPos();

            if (_carry.InFountain() || _carry.IsDead)
            {
                Roam();
                return;
            }

            SupportCarry();
        }

        /// <summary>
        /// Switching carries based on kda from highest to lowest
        /// </summary>
        public static void SmartCarrySwitch()
        {
            if (Environment.TickCount - _timeSinceLastCarrySwitch < 120000) return;

            if (_carry == null)
            {
                _carry = Heroes.AllyHeroes.OrderByDescending(
                    hero => (hero.ChampionsKilled/((hero.Deaths != 0) ? hero.Deaths : 1))).FirstOrDefault();
                return;
            }

            if (_carry.Level < _bot.Level || _bot.Level >= 7)
            {
                _carry = Heroes.AllyHeroes.OrderByDescending(
                    hero => (hero.ChampionsKilled / ((hero.Deaths != 0) ? hero.Deaths : 1))).FirstOrDefault();
            }
            _timeSinceLastCarrySwitch = Environment.TickCount;
        }

        /// <summary>
        /// This behavior will follow carry around in combo mode :D
        /// </summary>
        public static void SupportCarry()
        {
            Program.Orbwalker.ActiveMode = MyOrbwalker.OrbwalkingMode.Combo;
            Program.Orbwalker.SetOrbwalkingPoint(_followPos.IsValid() ? _followPos : _carry.ServerPosition.RandomizePosition());
        }

        /// <summary>
        /// This behavior will make the bot follow the tempcarry, the closest ally in range, as long as he's not the jungler.
        /// </summary>
        public static void Roam()
        {
            _tempCarry = Heroes.AllyHeroes.OrderBy(h => h.Distance(_bot)).FirstOrDefault(h => h != MyTeam.Jungler && h != _carry);
            if (_tempCarry == null)
            {
                //should probably go base or switch role here
                return;
            }
            Program.Orbwalker.ActiveMode = MyOrbwalker.OrbwalkingMode.Combo;
            Program.Orbwalker.SetOrbwalkingPoint(_followPos.IsValid() ? _followPos : _tempCarry.ServerPosition.RandomizePosition());
        }

        /// <summary>
        /// This behavior will make the bot go to bottom lane in and wait safely at the outer turret for a carry to support.
        /// </summary>
        public static void FindCarry()
        {
            _carry = MyTeam.ADC;
            var botLaneTurretPos = _bot.Team == GameObjectTeam.Order ? Map.BottomLane.Blue_Outer_Turret.RandomizePosition() : Map.BottomLane.Red_Outer_Turret.RandomizePosition();
                if (_bot.InFountain())
                {
                    Program.Orbwalker.ActiveMode = MyOrbwalker.OrbwalkingMode.Combo;
                    Program.Orbwalker.SetOrbwalkingPoint(botLaneTurretPos);
                }
        }

        /// <summary>
        /// this method will update the random follow position ever 400 ticks xd
        /// </summary>
        public static void UpdateFollowPos()
        {
            if (Environment.TickCount - _timeSinceLastFollowPosUpdate > 400)
            {
                _timeSinceLastFollowPosUpdate = Environment.TickCount;
                if ((_carry.IsDead || _carry.InFountain()) && _tempCarry != null)
                {
                    var point =
                        new Geometry.Circle(_tempCarry.Position.To2D(), 300).ToPolygon()
                            .ToClipperPath()
                            .OrderBy(p => new Random().Next())
                            .FirstOrDefault();

                    _followPos = new Vector2(point.X, point.Y).To3D();
                }
                else
                {
                    var point =
                        new Geometry.Circle(_carry.Position.To2D(), 300).ToPolygon()
                            .ToClipperPath()
                            .OrderBy(p => new Random().Next())
                            .FirstOrDefault();

                    _followPos = new Vector2(point.X, point.Y).To3D();
                }
            }
        }
    }
}
