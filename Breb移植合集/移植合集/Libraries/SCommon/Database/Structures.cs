namespace SCommon.Database
{
    /// <summary>
    ///     The ward data structure
    /// </summary>
    public struct WardData
    {
        public string Name;
        public string DisplayName;
        public string ObjectName;
        public ObjectType Type;
        public int CastRange;
        public int Duration;
    }

    /// <summary>
    ///     The champion data structure
    /// </summary>
    public struct ChampionData
    {
        public int ID;
        public string Name;
        public ChampionRole Role;
    }
}