using System.Text;
using EloBuddy;

namespace AdEvade.Data.Spells
{
    public enum SpellType
    {
        Line,
        Circular,
        Cone,
        Arc,
        None,
    }

    public enum CollisionObjectType
    {
        EnemyChampions,
        EnemyMinions,
    }

    public class SpellData
    {
        public string CharName;
        public SpellSlot SpellKey = SpellSlot.Q;
        public SpellDangerLevel Dangerlevel = SpellDangerLevel.Low;
        public string SpellName;
        public string Name;
        public float Range;
        public float Radius;
        public float SecondaryRadius;
        public float ProjectileSpeed = float.MaxValue;
        public string MissileName = "";
        public SpellType SpellType;
        public float SpellDelay = 250;
        public bool FixedRange = false;
        public bool UseEndPosition = false;
        public float Angle;
        public float SideRadius;
        public int Splits;
        public bool UsePackets = false;
        public float ExtraDelay = 0;
        public float ExtraDistance = 0;
        public bool IsThreeWay = false;
        public bool DefaultOff = false;
        public bool NoProcess = false;
        public bool IsWall = false;
        public float ExtraEndTime = 0;
        public bool HasEndExplosion = false;
        public bool IsSpecial = false;
        public float ExtraDrawHeight = 0;
        public string[] ExtraSpellNames;
        public string[] ExtraMissileNames;
        public CollisionObjectType[] CollisionObjects = { };

        public SpellData()
        {

        }

        public SpellData(
            string charName,
            string spellName,
            string name,
            int range,
            int radius,
            SpellDangerLevel dangerlevel,
            SpellType spellType
            )
        {
            CharName = charName;
            SpellName = spellName;
            Name = name;
            Range = range;
            Radius = radius;
            Dangerlevel = dangerlevel;
            SpellType = spellType;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Name: " + SpellName + " : " + CharName + " : " + MissileName);
            sb.AppendLine("Danger Level: " + Dangerlevel);
            sb.AppendLine("Raduis: " + Radius + "   Range: " + Range + "Is Fixed Ranged: " + FixedRange);
            return sb.ToString();
        }
    }
}