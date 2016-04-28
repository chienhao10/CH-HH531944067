using System;
using System.Collections.Generic;
using EloBuddy;
using EloBuddy.SDK.Rendering;
using SharpDX;
using Sprite = SharpDX.Direct3D9.Sprite;

namespace WardTracker
{
    /// <summary>
    ///     The WardTracker Variables class
    /// </summary>
    internal class WardTrackerVariables
    {
        /// <summary>
        ///     The ward durations
        /// </summary>
        public static Dictionary<WardType, float> wardDurations = new Dictionary<WardType, float>
        {
            {WardType.Green, 60*3*1000},
            {WardType.Trinket, 60*1000},
            {WardType.TrinketUpgrade, 60*3*1000},
            {WardType.Pink, float.MaxValue},
            {WardType.TeemoShroom, 60*10*1000},
            {WardType.ShacoBox, 60*1*1000}
        };


        /// <summary>
        ///     The wards wrappers containing a list of wards.
        /// </summary>
        public static List<WardTypeWrapper> wrapperTypes = new List<WardTypeWrapper>
        {
            new WardTypeWrapper
            {
                ObjectName = "YellowTrinket",
                SpellName = "TrinketTotemLvl1",
                WardType = WardType.Trinket,
                WardVisionRange = 1100
            },
            new WardTypeWrapper
            {
                ObjectName = "YellowTrinketUpgrade",
                SpellName = "TrinketTotemLvl2",
                WardType = WardType.TrinketUpgrade,
                WardVisionRange = 1100
            },
            new WardTypeWrapper
            {
                ObjectName = "SightWard",
                SpellName = "TrinketTotemLvl3",
                WardType = WardType.Green,
                WardVisionRange = 1100
            },
            new WardTypeWrapper
            {
                ObjectName = "SightWard",
                SpellName = "SightWard",
                WardType = WardType.Green,
                WardVisionRange = 1100
            },
            new WardTypeWrapper
            {
                ObjectName = "SightWard",
                SpellName = "ItemGhostWard",
                WardType = WardType.Green,
                WardVisionRange = 1100
            },
            //Pink Wards
            new WardTypeWrapper
            {
                ObjectName = "VisionWard",
                SpellName = "VisionWard",
                WardType = WardType.Pink,
                WardVisionRange = 1100
            },

            //Traps
            new WardTypeWrapper
            {
                ObjectName = "TeemoMushroom",
                SpellName = "BantamTrap",
                WardType = WardType.TeemoShroom,
                WardVisionRange = 212
            },
            new WardTypeWrapper
            {
                ObjectName = "ShacoBox",
                SpellName = "JackInTheBox",
                WardType = WardType.ShacoBox,
                WardVisionRange = 212
            }
        };

        /// <summary>
        ///     The detected wards list
        /// </summary>
        public static List<Ward> detectedWards = new List<Ward>();
    }

    /// <summary>
    ///     The Ward Class
    /// </summary>
    internal class Ward
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Ward" /> class.
        /// </summary>
        /// <param name="wrapper">The wrapper.</param>
        public Ward(WardTypeWrapper wrapper)
        {
            WardTypeW = wrapper;
        }

        /// <summary>
        ///     Gets or sets the position.
        /// </summary>
        /// <value>
        ///     The position.
        /// </value>
        public Vector3 Position { get; set; }

        /// <summary>
        ///     Gets or sets the start tick.
        /// </summary>
        /// <value>
        ///     The start tick.
        /// </value>
        public float startTick { get; set; }

        /// <summary>
        ///     Gets or sets the ward type wrapper.
        /// </summary>
        /// <value>
        ///     The ward type w.
        /// </value>
        public WardTypeWrapper WardTypeW { get; set; }

        /// <summary>
        ///     Gets or sets the text render object.
        /// </summary>
        /// <value>
        ///     The text object.
        /// </value>
        public Text TextObject { get; set; }

        /// <summary>
        ///     Gets or sets the minimap sprite object.
        /// </summary>
        /// <value>
        ///     The minimap sprite object.
        /// </value>
        public Sprite MinimapSpriteObject { get; set; }

        // LOL NOT RIGHT - BERB - Elobuddy doesn't have a dispose/remove function for Sprites

        /// <summary>
        ///     Removes the render objects.
        /// </summary>
        public void RemoveRenderObjects()
        {
            TextObject.Dispose();
            MinimapSpriteObject.Dispose();
        }
    }

    /// <summary>
    ///     The Ward Type wrapper class
    /// </summary>
    internal class WardTypeWrapper
    {
        /// <summary>
        ///     Gets or sets the name of the object.
        /// </summary>
        /// <value>
        ///     The name of the object.
        /// </value>
        public string ObjectName { get; set; }

        /// <summary>
        ///     Gets or sets the name of the spell.
        /// </summary>
        /// <value>
        ///     The name of the spell.
        /// </value>
        public string SpellName { get; set; }

        /// <summary>
        ///     Gets or sets the ward vision range.
        /// </summary>
        /// <value>
        ///     The ward vision range.
        /// </value>
        public float WardVisionRange { get; set; }

        /// <summary>
        ///     Gets or sets the type of the ward.
        /// </summary>
        /// <value>
        ///     The type of the ward.
        /// </value>
        public WardType WardType { get; set; }

        /// <summary>
        ///     Gets the duration of the ward.
        /// </summary>
        /// <value>
        ///     The duration of the ward.
        /// </value>
        public float WardDuration
        {
            get
            {
                float val;
                WardTrackerVariables.wardDurations.TryGetValue(WardType, out val);
                return val;
            }
        }
    }

    /// <summary>
    ///     The Enumeration containing the ward types.
    /// </summary>
    internal enum WardType
    {
        Trinket,
        TrinketUpgrade,
        Pink,
        Green,
        TeemoShroom,
        ShacoBox
    }
}