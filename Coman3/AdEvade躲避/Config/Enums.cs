using System;

namespace AdEvade.Config
{
    public enum EvadeMode
    {
        Fast,
        Smooth,
        VerySmooth,
        LowBan

    }

    public enum ConfigValue : short
    {
        //TODO: Add Config Values

        //Values < ConfigPlugin.ValueSeperator are Int32 values

        #region Int32's

        ExtraLineWidth,
        ExtraDetectionRange,
        ExtraEvadeDistance,
        ExtraSpellRadius,
        ExtraPingBuffer,
        ExtraCpaDistance,
        RejectMinDistance,
        MinimumComfortZone,
        EvadeMode,
        FastEvadeActivationTime,
        SpellActivationTime,
        TickLimiter,
        SpellDetectionTime,
        ReactionTime,
        DodgeInterval,
        LowDangerDrawWidth,
        NormalDangerDrawWidth,
        HighDangerDrawWidth,
        ExtremeDangerDrawWidth,
        AutoSetPingPercentile,
        RandomizerPercentage,
        RandomizerMaxDangerLevel,

        #endregion

        #region Seperator

        //Seperator for logic speration
        Seperator = Constants.ValueSeperator,

        #endregion

        //Values > ConfigPlugin.ValueSeperator are booleans

        #region Booleans

        DodgeSkillShots,
        ActivateEvadeSpells,
        DrawSkillShots,
        PreventDodgingNearEnemy,
        HighPrecision,
        CalculateWindupDelay,
        PreventDodgingUnderTower,
        ClickOnlyOnce,
        OnlyDodgeDangerous,
        DodgeDangerousKeysEnabled,
        DodgeFowSpells,
        DodgeCircularSpells,
        DodgeDangerousKey1,
        DodgeDangerousKey2, //
        RecalculatePath,
        ContinueMovement,
        CheckSpellCollision,
        AdvancedSpellDetection,
        FastMovementBlock,
        EnableEvadeDistance,
        ShowDebugInfo,
        EnableSpellTester,
        DrawEvadeStatus,
        DrawSpellPosition,
        DrawEvadePosition,
        AutoSetPing,
        EnableRandomizer,
        DrawBlockedRandomizerSpells





        #endregion
    }
    public static class ConfigValueHelpers
    {
        public static bool IsBool(this ConfigValue value)
        {
            return (short) value > Constants.ValueSeperator;
        }

        public static bool IsInt(this ConfigValue value)
        {
            return (short) value < Constants.ValueSeperator;
        }

        public static string Name(this ConfigValue value)
        {
            return Enum.GetName(typeof (ConfigValue), value);
        }

        public static short Value(this ConfigValue value)
        {
            return (short) value;
        }

        public static int GetInt(this ConfigValue value)
        {
            return Properties.GetInt(value);
        }

        public static bool GetBool(this ConfigValue value)
        {
            return Properties.GetBool(value);
        }

        public static void SetInt(this ConfigValue key, int value)
        {
            Properties.SetValue(key, value);
        }

        public static void SetBool(this ConfigValue key, bool value)
        {
            Properties.SetValue(key, value);
        }

        public static int SetTo(this int value, ConfigValue key)
        {
            SetInt(key, value);
            return value;
        }

        public static int SetTo(this float value, ConfigValue key)
        {
            SetInt(key, (int) value);
            return (int) value;
        }

        public static bool SetTo(this bool value, ConfigValue key)
        {
            SetBool(key, value);
            return value;
        }
    }


}