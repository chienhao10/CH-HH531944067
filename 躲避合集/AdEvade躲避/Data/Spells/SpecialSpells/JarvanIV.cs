using System;
using System.Collections.Generic;
using System.Linq;
using AdEvade.Utils;
using EloBuddy;
using EloBuddy.SDK;

namespace AdEvade.Data.Spells.SpecialSpells
{
    class JarvanIv : IChampionPlugin
    {
        static JarvanIv()
        {
            
        }
        public const string ChampionName = "JarvanIv";
        public string GetChampionName()
        {
            return ChampionName;
        }
        public void LoadSpecialSpell(SpellData spellData)
        {
            if (spellData.SpellName == "JarvanIVDragonStrike")
            {
                AIHeroClient hero = EntityManager.Heroes.Enemies.FirstOrDefault(h => h.ChampionName == "JarvanIV");
                if (hero == null)
                {
                    return;
                }

                Obj_AI_Base.OnProcessSpellCast += ProcessSpell_JarvanIVDemacianStandard;

                SpellDetector.OnProcessSpecialSpell += ProcessSpell_JarvanIVDragonStrike;
                GameObject.OnCreate += OnCreateObj_JarvanIVDragonStrike;
                GameObject.OnDelete += OnDeleteObj_JarvanIVDragonStrike;
            }            
        }

        private static void ProcessSpell_JarvanIVDemacianStandard(Obj_AI_Base hero, GameObjectProcessSpellCastEventArgs args)
        {
            if (hero.IsEnemy && args.SData.Name == "JarvanIVDemacianStandard")
            {
                ObjectTracker.AddObjTrackerPosition("Beacon", args.End, 1000);
            }
        }

        private static void OnDeleteObj_JarvanIVDragonStrike(GameObject obj, EventArgs args)
        {
            if (obj.Name == "Beacon")
            {
                ObjectTracker.ObjTracker.Remove(obj.NetworkId);
            }
        }

        private static void OnCreateObj_JarvanIVDragonStrike(GameObject obj, EventArgs args)
        {
            if (obj.Name == "Beacon")
            {
                ObjectTracker.ObjTracker.Add(obj.NetworkId, new ObjectTrackerInfo(obj));
            }
        }

        private static void ProcessSpell_JarvanIVDragonStrike(Obj_AI_Base hero, GameObjectProcessSpellCastEventArgs args, SpellData spellData, SpecialSpellEventArgs specialSpellArgs)
        {
            if (args.SData.Name == "JarvanIVDragonStrike")
            {
                if (SpellDetector.OnProcessSpells.TryGetValue("JarvanIVDragonStrike2", out spellData))
                {
                    foreach (KeyValuePair<int, ObjectTrackerInfo> entry in ObjectTracker.ObjTracker)
                    {
                        var info = entry.Value;

                        if (info.Name == "Beacon" || info.Obj.Name == "Beacon")
                        {
                            if (info.UsePosition == false && (info.Obj == null || !info.Obj.IsValid || info.Obj.IsDead))
                            {
                                DelayAction.Add(1, () => ObjectTracker.ObjTracker.Remove(info.Obj.NetworkId));
                                continue;
                            }

                            var objPosition = info.UsePosition ? info.Position.To2D() : info.Obj.Position.To2D();

                            if (args.End.To2D().Distance(objPosition) < 300)
                            {
                                var dir = (objPosition - args.Start.To2D()).Normalized();
                                var endPosition = objPosition + dir * 110;

                                SpellDetector.CreateSpellData(hero, args.Start, endPosition.To3D(), spellData);
                                specialSpellArgs.NoProcess = true;
                                return;
                            }
                        }
                    }


                }
            }
        }
    }
}
