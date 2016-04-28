namespace Kindred___YinYang.Spell_Database
{
    public enum SpellType
    {
        Line,
        Circular,
        Cone
    }

    public class SpellData
    {
        public float angle;
        public string charName;
        public int dangerlevel = 1;
        public bool defaultOff = false;
        public float extraDelay = 0;
        public float extraDistance = 0;
        public float extraEndTime = 0;
        public string[] extraMissileNames;
        public string[] extraSpellNames;
        public bool fixedRange = false;
        public bool hasEndExplosion = false;
        public bool isSpecial = false;
        public bool isThreeWay = false;
        public bool isWall = false;
        public string missileName = "";
        public string name;
        public bool noProcess = false;
        public float projectileSpeed = float.MaxValue;
        public float radius;
        public float range;
        public float sideRadius;
        public float spellDelay = 250;
        public string spellKey;
        public string spellName;
        public SpellType spellType;
        public int splits;
        public bool usePackets = false;

        public SpellData()
        {
        }

        public SpellData(
            string charName,
            string spellName,
            string name,
            int range,
            int radius,
            int dangerlevel,
            SpellType spellType
            )
        {
            this.charName = charName;
            this.spellName = spellName;
            this.name = name;
            this.range = range;
            this.radius = radius;
            this.dangerlevel = dangerlevel;
            this.spellType = spellType;
        }
    }
}