// This file is part of LeagueSharp.Common.
// 
// LeagueSharp.Common is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// LeagueSharp.Common is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with LeagueSharp.Common.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using LeagueSharp.Common;
using SharpDX;
using Utility = LeagueSharp.Common.Utility;

// ReSharper disable MergeConditionalExpression
// ReSharper disable FunctionRecursiveOnAllPaths

namespace iDZed.Utils
{
    internal static class ShadowManager
    {
        private const string ZedWMissileName = "ZedShadowDashMissile";
        private const string ZedRMissileName = "ZedUltMissile";
        private const string ZedShadowName = "zedshadow";
        // ReSharper disable once InconsistentNaming
        public static readonly List<Shadow> _shadowsList = new List<Shadow>
        {
            new Shadow {State = ShadowState.NotActive, Type = ShadowType.Normal},
            new Shadow {State = ShadowState.NotActive, Type = ShadowType.Ult}
        };

        /// <summary>
        ///     Find the WShadow from the shadows list
        /// </summary>
        public static Shadow WShadow
        {
            get { return _shadowsList.Find(x => x.Type == ShadowType.Normal); }
        }

        /// <summary>
        ///     Find the RShadow from the shadows list
        /// </summary>
        public static Shadow RShadow
        {
            get { return _shadowsList.Find(x => x.Type == ShadowType.Ult); }
        }

        /// <summary>
        ///     Subscribe to the events needed.
        /// </summary>
        public static void OnLoad()
        {
            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
        }

        /// <summary>
        ///     OnCreate Event :)
        /// </summary>
        /// <param name="sender"> the sender of the gameobject </param>
        /// <param name="args">the event arguments kappa</param>
        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (!(sender is Obj_AI_Minion) && !(sender is MissileClient))
            {
                return;
            }
            switch (sender.Type)
            {
                case GameObjectType.obj_AI_Minion:
                    var minion = sender as Obj_AI_Minion;
                    if (minion != null && minion.BaseSkinName.Equals(ZedShadowName))
                    {
                        var myShadow = _shadowsList.FirstOrDefault(shadow => shadow.State == ShadowState.Travelling);
                        if (myShadow != null)
                        {
                            myShadow.State = ShadowState.Created;
                            myShadow.ShadowObject = minion;
                            myShadow.Position = minion.ServerPosition;
                            // TODO, since you added this m8 the position is offset a little bit, and when you switch with shadows the new shadow position isn't correct. :S
                            //Hacky workaround, TODO: Find a better way
                            Utility.DelayAction.Add(
                                4200, () =>
                                {
                                    myShadow.State = ShadowState.NotActive;
                                    myShadow.ShadowObject = null;
                                    myShadow.Position = Vector3.Zero;
                                });
                        }
                    }
                    break;
                default:
                    var spell = (MissileClient) sender;
                    var caster = spell.SpellCaster;
                    var spellName = spell.SData.Name;
                    if (caster.IsMe)
                    {
                        switch (spellName)
                        {
                            case ZedRMissileName:
                                var rShadow = _shadowsList.FirstOrDefault(shadow => shadow.Type == ShadowType.Ult);
                                if (rShadow != null)
                                {
                                    rShadow.State = ShadowState.Travelling;
                                    rShadow.Position = spell.EndPosition;
                                }
                                break;
                            case ZedWMissileName:
                                var wShadow = _shadowsList.FirstOrDefault(shadow => shadow.Type == ShadowType.Normal);
                                if (wShadow != null)
                                {
                                    wShadow.State = ShadowState.Travelling;
                                    wShadow.Position = spell.EndPosition;
                                }
                                break;
                        }
                    }
                    break;
            }
        }

        /// <summary>
        ///     OnDelete Event
        /// </summary>
        /// <param name="sender">the sender of the game object</param>
        /// <param name="args">the event arguments</param>
        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender != null && sender.IsAlly)
            {
                var myShadow =
                    _shadowsList.Find(
                        shadow => shadow.ShadowObject != null && shadow.ShadowObject.NetworkId.Equals(sender.NetworkId));
                if (myShadow != null)
                {
                    myShadow.State = ShadowState.NotActive;
                    myShadow.ShadowObject = null;
                }
            }
        }

        /// <summary>
        ///     Checks if the player can go to the selected Shadow.
        /// </summary>
        /// <param name="shadow">The shadow</param>
        /// <returns></returns>
        public static bool CanGoToShadow(Shadow shadow) //TODO safety Checks lel
        {
            if (Zed.getCheckBoxItem(Zed.miscMenu, "safetyChecks") &&
                (!Zed.getKeyBindItem(Zed.fleeMenu, "fleeActive") ||
                 !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee)))
            {
                if (shadow.State == ShadowState.Created)
                {
                    if (ObjectManager.Player.HealthPercent < 35 || shadow.Position.UnderTurret(true) ||
                        (shadow.ShadowObject.CountEnemiesInRange(1200f) > 1 &&
                         shadow.ShadowObject.CountEnemiesInRange(1200f) < 2))
                    {
                        return false;
                    }
                }
            }

            return shadow.State == ShadowState.Created;
        }
    }

    internal class Shadow
    {
        private Vector3 shPos = Vector3.Zero;
        public Obj_AI_Minion ShadowObject { get; set; }
        public ShadowState State { get; set; }
        public ShadowType Type { get; set; }

        public Vector3 Position
        {
            get { return ShadowObject != null ? ShadowObject.Position : shPos; }
            set { shPos = value; }
        }

        public bool IsUsable
        {
            get { return ShadowObject == null && State == ShadowState.NotActive; }
        }

        public bool Exists
        {
            get { return ShadowObject != null && State != ShadowState.NotActive; }
        }
    }

    internal enum ShadowType
    {
        Normal,
        Ult
    }

    internal enum ShadowState
    {
        NotActive,
        Travelling,
        Created,
        Used
    }
}