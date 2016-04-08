using System.Text;
using AdEvade.Data.Spells;
using EloBuddy;

namespace AdEvade.Data.EvadeSpells
{
    public delegate bool UseSpellFunc(EvadeSpellData evadeSpell, bool process = true);

    public enum CastType
    {
        Position,
        Target,
        Self,
    }

    public enum SpellTargets
    {
        AllyMinions,
        EnemyMinions,

        AllyChampions,
        EnemyChampions,

        Targetables,
    }

    public enum EvadeType
    {
        Blink,
        Dash,
        SpellShield,
        Shield,
        WindWall,
        MovementSpeedBuff
    }

    public class EvadeSpellData
    {
        public string CharName;
        public SpellSlot SpellKey = SpellSlot.Q;
        public string SpellName;
        public string Name;
        public bool CheckSpellName = false;
        public float SpellDelay = 250;
        public float Range;
        public float Speed = 0;
        public SpellDangerLevel Dangerlevel = SpellDangerLevel.Normal;
        public bool FixedRange = false;
        public EvadeType EvadeType;
        public bool IsReversed = false;
        public bool BehindTarget = false;
        public bool InfrontTarget = false;
        public bool IsSummonerSpell = false;
        public bool IsItem = false;
        public ItemId ItemId = 0;
        public CastType CastType = CastType.Position;
        public SpellTargets[] SpellTargets = { };
        public UseSpellFunc UseSpellFunc = null;
        public bool IsSpecial = false;

        public EvadeSpellData()
        {

        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Name " + SpellName);
            sb.Append(" DangerLevel: " + Dangerlevel);
            sb.Append(" EvadeType: " + EvadeType);
            sb.Append("Range: " + Range);
            return sb.ToString();
        }
    }
}
