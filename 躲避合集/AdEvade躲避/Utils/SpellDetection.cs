using EloBuddy;
using EloBuddy.SDK;
using AdEvade.Config;
using AdEvade.Data;
using SpellData = AdEvade.Data.Spells.SpellData;

namespace AdEvade.Utils
{
    public static class SpellDetection
    {
        private static readonly AIHeroClient MyHero = ObjectManager.Player;
        public static bool IsMissileClient(this GameObject gameObject, out MissileClient missileClient)
        {
            missileClient = null;
            if (gameObject.GetType() != typeof(MissileClient) || !((MissileClient)gameObject).IsValidMissile())
                return false;
            missileClient = (MissileClient) gameObject;
            return true;
        }

        public static bool ShouldEvade(this EloBuddy.SpellData eloData, Obj_AI_Base hero, out SpellData spellData)
        {
            spellData = null;
            return (hero.Team != MyHero.Team) && SpellDetector.OnProcessSpells.TryGetValue(eloData.Name, out spellData);
        }
        public static bool ShouldEvade(this MissileClient missile, out SpellData spellData)
        {
            spellData = null;
            return (missile.SpellCaster.Team != MyHero.Team) && SpellDetector.OnMissileSpells.TryGetValue(missile.SData.Name, out spellData);

        }

        public static bool IsInRange(this MissileClient missile, SpellData spellData)
        {
            return missile.StartPosition.Distance(MyHero.Position) <
                spellData.Range + ConfigValue.ExtraDetectionRange.GetInt();
        }
        public static bool IsValidEvadeSpell(this MissileClient missile, out SpellData spellData)
        {
            spellData = null;
            if (missile == null) return false;
            //Check if spell is valid to continue
            if (missile.SpellCaster != null && missile.SData != null && missile.SData.Name != null)
            {
                return missile.ShouldEvade(out spellData);
            }
            return false;
        }
    }
}