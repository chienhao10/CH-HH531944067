namespace Valvrave_Sharp.Evade
{
    #region

    using LeagueSharp;

    #endregion
    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Menu;
    public enum SpellValidTargets
    {
        AllyMinions,

        EnemyMinions,

        AllyWards,

        EnemyWards,

        AllyChampions,

        EnemyChampions
    }

    internal class EvadeSpellData
    {
        #region Fields

        public string CheckBuffName = "";

        public string CheckSpellName = "";

        public int Delay;

        public bool ExtraDelay;

        public bool FixedRange;

        public bool IsBlink;

        public bool IsDash;

        public bool IsInvulnerability;

        public bool IsMovementSpeedBuff;

        public bool IsShield;

        public bool IsSpellShield;

        public MoveSpeedAmount MoveSpeedTotalAmount;

        public string Name;

        public float Range;

        public bool SelfCast;

        public SpellSlot Slot;

        public int Speed;

        public bool UnderTower;

        public SpellValidTargets[] ValidTargets;

        private int dangerLevel;

        public static bool getCheckBoxItem(string item)
        {
            return Config.evadeMenu[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(string item)
        {
            return Config.evadeMenu[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(string item)
        {
            return Config.evadeMenu[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(string item)
        {
            return Config.evadeMenu[item].Cast<ComboBox>().CurrentValue;
        }

        #endregion

        #region Delegates

        public delegate float MoveSpeedAmount();

        #endregion

        #region Public Properties

        public int DangerLevel
        {
            get
            {
                if (Config.evadeMenu[this.Name + "DangerLevel"] == null)
                {
                    return this.dangerLevel;
                }
                 return getSliderItem(this.Name + "DangerLevel");
            }
            set
            {
                this.dangerLevel = value;
            }
        }

        public bool Enable => getCheckBoxItem(this.Name + "Enabled");

        public bool IsReady
            =>
                (this.CheckSpellName == ""
                 || Program.Player.Spellbook.GetSpell(this.Slot).SData.Name.ToLower() == this.CheckSpellName)
                && Program.Player.Spellbook.CanUseSpell(this.Slot) == SpellState.Ready;

        public bool IsTargetted => this.ValidTargets != null;

        #endregion
    }

    internal class DashData : EvadeSpellData
    {
        #region Constructors and Destructors

        public DashData(
            string name,
            SpellSlot slot,
            float range,
            bool fixedRange,
            int delay,
            int speed,
            int dangerLevel)
        {
            this.Name = name;
            this.Range = range;
            this.Slot = slot;
            this.FixedRange = fixedRange;
            this.Delay = delay;
            this.Speed = speed;
            this.DangerLevel = dangerLevel;
            this.IsDash = true;
        }

        #endregion
    }

    internal class BlinkData : EvadeSpellData
    {
        #region Constructors and Destructors

        public BlinkData(string name, SpellSlot slot, float range, int delay, int dangerLevel)
        {
            this.Name = name;
            this.Range = range;
            this.Slot = slot;
            this.Delay = delay;
            this.DangerLevel = dangerLevel;
            this.IsBlink = true;
        }

        #endregion
    }

    internal class InvulnerabilityData : EvadeSpellData
    {
        #region Constructors and Destructors

        public InvulnerabilityData(string name, SpellSlot slot, int delay, int dangerLevel)
        {
            this.Name = name;
            this.Slot = slot;
            this.Delay = delay;
            this.DangerLevel = dangerLevel;
            this.IsInvulnerability = true;
        }

        #endregion
    }

    internal class ShieldData : EvadeSpellData
    {
        #region Constructors and Destructors

        public ShieldData(string name, SpellSlot slot, int delay, int dangerLevel, bool isSpellShield = false)
        {
            this.Name = name;
            this.Slot = slot;
            this.Delay = delay;
            this.DangerLevel = dangerLevel;
            this.IsSpellShield = isSpellShield;
            this.IsShield = !this.IsSpellShield;
        }

        #endregion
    }

    internal class MoveBuffData : EvadeSpellData
    {
        #region Constructors and Destructors

        public MoveBuffData(string name, SpellSlot slot, int delay, int dangerLevel, MoveSpeedAmount amount)
        {
            this.Name = name;
            this.Slot = slot;
            this.Delay = delay;
            this.DangerLevel = dangerLevel;
            this.MoveSpeedTotalAmount = amount;
            this.IsMovementSpeedBuff = true;
        }

        #endregion
    }
}