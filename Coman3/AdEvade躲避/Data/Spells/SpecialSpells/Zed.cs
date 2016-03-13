using System;
using System.Collections.Generic;
using AdEvade.Utils;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;

namespace AdEvade.Data.Spells.SpecialSpells
{
    class Zed : IChampionPlugin
    {
        static Zed()
        {

        }
        public const string ChampionName = "Zed";
        public string GetChampionName()
        {
            return ChampionName;
        }
        public void LoadSpecialSpell(SpellData spellData)
        {
            if (spellData.SpellName == "ZedQ")
            {
                SpellDetector.OnProcessSpecialSpell += ProcessSpell_ZedShuriken;
                GameObject.OnCreate += SpellMissile_ZedShadowDash;
                GameObject.OnCreate += OnCreateObj_ZedShuriken;
                GameObject.OnDelete += OnDeleteObj_ZedShuriken;
            }
        }

        private static void OnCreateObj_ZedShuriken(GameObject obj, EventArgs args)
        {
            if (obj.Name == "Shadow" && obj.IsEnemy)
            {
                if (!ObjectTracker.ObjTracker.ContainsKey(obj.NetworkId))
                {
                    ObjectTracker.ObjTracker.Add(obj.NetworkId, new ObjectTrackerInfo(obj));

                    foreach (KeyValuePair<int, ObjectTrackerInfo> entry in ObjectTracker.ObjTracker)
                    {
                        var info = entry.Value;

                        if (info.Name == "Shadow" && info.UsePosition && info.Position.Distance(obj.Position) < 5)
                        {
                            info.UsePosition = false;
                            info.Obj = obj;
                        }
                    }
                }
            }
        }

        private static void OnDeleteObj_ZedShuriken(GameObject obj, EventArgs args)
        {
            if (obj != null && obj.IsValid && obj.Name == "Shadow" && obj.IsEnemy)
            {
                ObjectTracker.ObjTracker.Remove(obj.NetworkId);
            }
        }

        private static void ProcessSpell_ZedShuriken(Obj_AI_Base hero, GameObjectProcessSpellCastEventArgs args, SpellData spellData,
            SpecialSpellEventArgs specialSpellArgs)
        {
            if (spellData.SpellName == "ZedQ")
            {
                foreach (KeyValuePair<int, ObjectTrackerInfo> entry in ObjectTracker.ObjTracker)
                {
                    var info = entry.Value;

                    if (info.Obj.Name == "Shadow" || info.Name == "Shadow")
                    {
                        if (info.UsePosition == false && (info.Obj == null || !info.Obj.IsValid || info.Obj.IsDead))
                        {
                            DelayAction.Add(1, () => ObjectTracker.ObjTracker.Remove(info.Obj.NetworkId));
                            continue;
                        }
                        else
                        {
                            Vector3 endPos2;
                            if (info.UsePosition == false)
                            {
                                endPos2 = info.Obj.Position.Extend(args.End, spellData.Range).To3DWorld();
                                SpellDetector.CreateSpellData(hero, info.Obj.Position, endPos2, spellData, null, 0, false);
                            }
                            else
                            {
                                endPos2 = info.Position.Extend(args.End, spellData.Range).To3DWorld();
                                SpellDetector.CreateSpellData(hero, info.Position, endPos2, spellData, null, 0, false);
                            }

                        }
                    }
                }
            }
        }

        private static void SpellMissile_ZedShadowDash(GameObject obj, EventArgs args)
        {
            if (obj.GetType() != typeof(MissileClient) || !((MissileClient) obj).IsValidMissile())
                return;

            MissileClient missile = (MissileClient)obj;

            if (missile.SpellCaster.IsEnemy && missile.SData.Name == "ZedWMissile")
            {
                if (!ObjectTracker.ObjTracker.ContainsKey(obj.NetworkId))
                {
                    ObjectTrackerInfo info = new ObjectTrackerInfo(obj);
                    info.Name = "Shadow";
                    info.OwnerNetworkId = missile.SpellCaster.NetworkId;
                    info.UsePosition = true;
                    info.Position = missile.EndPosition;

                    ObjectTracker.ObjTracker.Add(obj.NetworkId, info);

                    DelayAction.Add(1000, () => ObjectTracker.ObjTracker.Remove(obj.NetworkId));
                }
            }
        }
    }
}
