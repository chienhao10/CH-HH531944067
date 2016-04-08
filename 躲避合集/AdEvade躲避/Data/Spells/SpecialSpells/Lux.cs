using System;
using System.Linq;
using AdEvade.Draw;
using AdEvade.Utils;
using EloBuddy;
using EloBuddy.SDK;

namespace AdEvade.Data.Spells.SpecialSpells
{
    class Lux : IChampionPlugin
    {
        
        static Lux()
        {

        }

        public const string ChampionName = "Lux";
        public string GetChampionName()
        {
            return ChampionName;
        }

        public void LoadSpecialSpell(SpellData spellData)
        {
            Debug.DrawTopLeft("Loading Special Spell For: " + ChampionName);
            if (spellData.SpellName == "LuxMaliceCannon")
            {
                var hero = EntityManager.Heroes.Enemies.FirstOrDefault(h => h.ChampionName == "Lux");
                if (hero != null)
                {
                    GameObject.OnCreate += (obj, args) => OnCreateObj_LuxMaliceCannon(obj, args, hero, spellData);
                }
            }
        }

        private static void OnCreateObj_LuxMaliceCannon(GameObject obj, EventArgs args, AIHeroClient hero, SpellData spellData)
        {
            if (obj.IsEnemy && !hero.IsVisible &&
                obj.Name.Contains("Lux") && obj.Name.Contains("R_mis_beam_middle"))
            {
                var objList = ObjectTracker.ObjTracker.Values.Where(o => o.Name == "hiu");
                if (objList.Count() > 3)
                {
                    var dir = ObjectTracker.GetLastHiuOrientation();
                    var pos1 = obj.Position.To2D() - dir * 1750;
                    var pos2 = obj.Position.To2D() + dir * 1750;

                    SpellDetector.CreateSpellData(hero, pos1.To3D(), pos2.To3D(), spellData, null, 0);

                    foreach (ObjectTrackerInfo gameObj in objList)
                    {
                        DelayAction.Add(1, () => ObjectTracker.ObjTracker.Remove(gameObj.Obj.NetworkId));
                    }
                }
            }
        }
    }
}
