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

namespace TreeLib.SpellData
{
    public class SpellData
    {
        #region Fields

        public bool AddHitbox;

        public bool CanBeRemoved = false;

        public bool Centered;

        public CollisionObjectTypes[] CollisionObjects = { };

        public string ChampionName;

        public int DangerValue;

        public int Delay;

        public bool DisabledByDefault = false;

        public bool DisableFowDetection = false;

        public bool DontAddExtraDuration;

        public bool DontCross = false;

        public bool DontCheckForDuplicates = false;

        public bool DontRemove = false;

        public int ExtraDuration;

        public string[] ExtraMissileNames = { };

        public int ExtraRange = -1;

        public string[] ExtraSpellNames = { };

        public bool FixedRange;

        public bool FollowCaster = false;

        public bool ForceRemove = false;

        public string FromObject = "";

        public string[] FromObjects = { };

        public int Id = -1;

        public bool Invert;

        public bool IsDangerous = false;

        public int MissileAccel = 0;

        public bool MissileDelayed;

        public bool MissileFollowsUnit;

        public int MissileMaxSpeed;

        public int MissileMinSpeed;

        public int MissileSpeed;

        public string MissileSpellName = "";

        public float MultipleAngle;

        public int MultipleNumber = -1;

        public int RingRadius;

        public SpellSlot Slot;

        public string SpellName;

        public bool TakeClosestPath = false;

        public string ToggleParticleName = "";

        public SkillShotType Type;

        #endregion

        #region Public Properties

        public int Radius
        {
            get
            {
                return !AddHitbox
                    ? RawRadius + Config.SkillShotsExtraRadius
                    : Config.SkillShotsExtraRadius + RawRadius + (int) ObjectManager.Player.BoundingRadius;
            }
            set { RawRadius = value; }
        }

        public int Range
        {
            get
            {
                return RawRange +
                       (Type == SkillShotType.SkillshotLine || Type == SkillShotType.SkillshotMissileLine
                           ? Config.SkillShotsExtraRange
                           : 0);
            }
            set { RawRange = value; }
        }

        public int RawRadius { get; private set; }

        public int RawRange { get; private set; }

        #endregion
    }
}