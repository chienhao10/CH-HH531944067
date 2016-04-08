namespace AdEvade.Data.Spells.SpecialSpells
{
    class Syndra : IChampionPlugin
    {
        static Syndra()
        {

        }
        public const string ChampionName = "Syndra";
        public string GetChampionName()
        {
            return ChampionName;
        }
        public void LoadSpecialSpell(SpellData spellData)
        {

        }
    }
}
