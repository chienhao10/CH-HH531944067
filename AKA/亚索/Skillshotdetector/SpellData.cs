using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;

namespace AkaYasuo
{
    public class SpellData
    {
        #region Fields

        public bool AddHitbox = false;

        public bool CanBeRemoved = false;

        public bool Centered = false;

        public CollisionObjectTypes[] CollisionObjects = { };

        public string ChampionName;

        public int DangerValue;

        public int Delay;

        public bool DisabledByDefault = false;

        public bool DisableFowDetection = false;

        public bool DontAddExtraDuration = false;

        public bool DontCross = false;

        public bool DontCheckForDuplicates = false;

        public bool DontRemove = false;

        public int ExtraDuration;

        public string[] ExtraMissileNames = { };

        public int ExtraRange = -1;

        public string[] ExtraSpellNames = { };

        public bool FixedRange = false;

        public bool FollowCaster = false;

        public bool ForceRemove = false;

        public string FromObject = "";

        public string[] FromObjects = { };

        public int Id = -1;

        public bool Invert = false;

        public bool IsDangerous = false;

        public int MissileAccel = 0;

        public bool MissileDelayed = false;

        public bool MissileFollowsUnit = false;

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

        public string MenuItemName
        {
            get
            {
                return this.ChampionName + " - " + this.SpellName;
            }
        }

        public int Radius
        {
            get
            {
                return Configs.SkillShotsExtraRadius + this.RawRadius
                       + (!this.AddHitbox ? 0 : (int)ObjectManager.Player.BoundingRadius);
            }
            set
            {
                this.RawRadius = value;
            }
        }

        public int Range
        {
            get
            {
                return this.RawRange
                       + (this.Type == SkillShotType.SkillshotLine || this.Type == SkillShotType.SkillshotMissileLine
                              ? Configs.SkillShotsExtraRange
                              : 0);
            }
            set
            {
                this.RawRange = value;
            }
        }

        public int RawRadius { get; private set; }

        public int RawRange { get; private set; }

        #endregion
    }
}
