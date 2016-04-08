namespace AdEvade.Data.Spells
{
    public interface IChampionPlugin
    {
        string GetChampionName();
        void LoadSpecialSpell(SpellData spellData);
    }
}