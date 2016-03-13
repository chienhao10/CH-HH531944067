using System;
using EloBuddy;
using EloBuddy.SDK;

namespace AdEvade.Data.Spells.SpecialSpells
{
    class Viktor : IChampionPlugin
    {
        static Viktor()
        {

        }
        public const string ChampionName = "Viktor";
        public string GetChampionName()
        {
            return ChampionName;
        }
        public void LoadSpecialSpell(SpellData spellData)
        {
            if (spellData.SpellName == "ViktorDeathRay3")
            {
                GameObject.OnCreate += OnCreateObj_ViktorDeathRay3;
            }
        }

        private static void OnCreateObj_ViktorDeathRay3(GameObject obj, EventArgs args)
        {
            if (obj.GetType() != typeof(MissileClient) || !((MissileClient) obj).IsValidMissile())
                return;

            MissileClient missile = (MissileClient)obj;

            SpellData spellData;

            if (missile.SpellCaster != null && missile.SpellCaster.Team != ObjectManager.Player.Team &&
                missile.SData.Name != null && missile.SData.Name == "viktoreaugmissile"
                && SpellDetector.OnMissileSpells.TryGetValue("ViktorDeathRay3", out spellData)
                && missile.StartPosition != null && missile.EndPosition != null)
            {
                var missileDist = missile.EndPosition.To2D().Distance(missile.StartPosition.To2D());
                var delay = missileDist / 1.5f + 600;

                spellData.SpellDelay = delay;

                SpellDetector.CreateSpellData(missile.SpellCaster, missile.StartPosition, missile.EndPosition, spellData);
            }
        }
    }
}
