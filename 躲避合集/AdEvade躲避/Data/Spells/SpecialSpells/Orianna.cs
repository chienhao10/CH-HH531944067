using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;

namespace AdEvade.Data.Spells.SpecialSpells
{
    class Orianna : IChampionPlugin
    {
        static Orianna()
        {

        }
        public const string ChampionName = "Orianna";
        public string GetChampionName()
        {
            return ChampionName;
        }
        public void LoadSpecialSpell(SpellData spellData)
        {
            if (spellData.SpellName == "OrianaIzunaCommand")
            {
                AIHeroClient hero = EntityManager.Heroes.Enemies.FirstOrDefault(h => h.ChampionName == "Orianna");
                if (hero == null)
                {
                    return;
                }

                ObjectTrackerInfo info = new ObjectTrackerInfo(hero);
                info.Name = "TheDoomBall";
                info.OwnerNetworkId = hero.NetworkId;

                ObjectTracker.ObjTracker.Add(hero.NetworkId, info);

                GameObject.OnCreate += (obj, args) => OnCreateObj_OrianaIzunaCommand(obj, args, hero);
                //Obj_AI_Minion.OnDelete += (obj, args) => OnDeleteObj_OrianaIzunaCommand(obj, args, hero);
                Obj_AI_Base.OnProcessSpellCast += ProcessSpell_OrianaRedactCommand;
                SpellDetector.OnProcessSpecialSpell += ProcessSpell_OrianaIzunaCommand;
            }
        }

        private static void OnCreateObj_OrianaIzunaCommand(GameObject obj, EventArgs args, AIHeroClient hero)
        {                        
            if (obj.Name.Contains("Orianna") && obj.Name.Contains("Ball_Flash_Reverse") && obj.IsEnemy)
            {
                foreach (KeyValuePair<int, ObjectTrackerInfo> entry in ObjectTracker.ObjTracker)
                {
                    var info = entry.Value;

                    if (entry.Value.Name == "TheDoomBall")
                    {
                        info.UsePosition = false;
                        info.Obj = hero;
                    }
                }
            }
        }

        private static void OnDeleteObj_OrianaIzunaCommand(GameObject obj, EventArgs args, AIHeroClient hero)
        {
            if (obj.Name.Contains("Orianna") && obj.Name.Contains("ball_glow_red") && obj.IsEnemy)
            {
                foreach (KeyValuePair<int, ObjectTrackerInfo> entry in ObjectTracker.ObjTracker)
                {
                    var info = entry.Value;

                    if (entry.Value.Name == "TheDoomBall")
                    {
                        info.UsePosition = false;
                        info.Obj = hero;
                    }
                }
            }
        }

        private static void ProcessSpell_OrianaRedactCommand(Obj_AI_Base hero, GameObjectProcessSpellCastEventArgs args)
        {
            if (hero.GetType() != typeof(AIHeroClient) || !((AIHeroClient) hero).IsValid())
                return;

            var champ = (AIHeroClient)hero;

            if (champ.ChampionName == "Orianna" && champ.IsEnemy)
            {
                if (args.SData.Name == "OrianaRedactCommand")
                {
                    foreach (KeyValuePair<int, ObjectTrackerInfo> entry in ObjectTracker.ObjTracker)
                    {
                        var info = entry.Value;

                        if (entry.Value.Name == "TheDoomBall")
                        {
                            info.UsePosition = false;
                            info.Obj = args.Target;
                        }
                    }
                }
            }
        }

        private static void ProcessSpell_OrianaIzunaCommand(Obj_AI_Base hero, GameObjectProcessSpellCastEventArgs args, SpellData spellData,
            SpecialSpellEventArgs specialSpellArgs)
        {
            if (spellData.SpellName == "OrianaIzunaCommand")
            {
                foreach (KeyValuePair<int, ObjectTrackerInfo> entry in ObjectTracker.ObjTracker)
                {
                    var info = entry.Value;

                    if (entry.Value.Name == "TheDoomBall")
                    {
                        if (info.UsePosition)
                        {
                            SpellDetector.CreateSpellData(hero, info.Position, args.End, spellData, null, 0, false);
                            SpellDetector.CreateSpellData(hero, info.Position, args.End,
                spellData, null, 150, true, SpellType.Circular, false, spellData.SecondaryRadius);

                        }
                        else
                        {
                            if (info.Obj == null)
                                return;

                            SpellDetector.CreateSpellData(hero, info.Obj.Position, args.End, spellData, null, 0, false);
                            SpellDetector.CreateSpellData(hero, info.Obj.Position, args.End,
                spellData, null, 150, true, SpellType.Circular, false, spellData.SecondaryRadius);

                        }

                        info.Position = args.End;
                        info.UsePosition = true;
                    }
                }

                specialSpellArgs.NoProcess = true;
            }

            if (spellData.SpellName == "OrianaDetonateCommand" || spellData.SpellName == "OrianaDissonanceCommand")
            {
                foreach (KeyValuePair<int, ObjectTrackerInfo> entry in ObjectTracker.ObjTracker)
                {
                    var info = entry.Value;

                    if (entry.Value.Name == "TheDoomBall")
                    {
                        if (info.UsePosition)
                        {
                            Vector3 endPos2 = info.Position;
                            SpellDetector.CreateSpellData(hero, endPos2, endPos2, spellData, null, 0);
                        }
                        else
                        {
                            if (info.Obj == null)
                                return;

                            Vector3 endPos2 = info.Obj.Position;
                            SpellDetector.CreateSpellData(hero, endPos2, endPos2, spellData, null, 0);
                        }
                    }
                }

                specialSpellArgs.NoProcess = true;
            }
        }
    }
}
