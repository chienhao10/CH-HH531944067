using System.Collections.Generic;
using AdEvade.Utils;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;

namespace AdEvade.Data.Spells.SpecialSpells
{
    class Lulu : IChampionPlugin
    {
        static Lulu()
        {

        }
        public const string ChampionName = "Lulu";
        public string GetChampionName()
        {
            return ChampionName;
        }
        public void LoadSpecialSpell(SpellData spellData)
        {
            if (spellData.SpellName == "LuluQ")
            {
                SpellDetector.OnProcessSpecialSpell += ProcessSpell_LuluQ;
                GetLuluPix();
            }
        }

        private static void GetLuluPix()
        {
            bool gotObj = false;

            foreach (var obj in ObjectManager.Get<Obj_AI_Minion>())
            {
                if (obj != null && obj.IsValid && obj.BaseSkinName == "lulufaerie" && obj.IsEnemy)
                {
                    gotObj = true;

                    if (!ObjectTracker.ObjTracker.ContainsKey(obj.NetworkId))
                    {
                        ObjectTracker.ObjTracker.Add(obj.NetworkId, new ObjectTrackerInfo(obj, "RobotBuddy"));
                    }
                }
            }

            if (gotObj == false)
            {
                DelayAction.Add(5000, () => GetLuluPix());
            }
        }

        private static void ProcessSpell_LuluQ(Obj_AI_Base hero, GameObjectProcessSpellCastEventArgs args, SpellData spellData,
            SpecialSpellEventArgs specialSpellArgs)
        {
            if (spellData.SpellName == "LuluQ")
            {
                foreach (KeyValuePair<int, ObjectTrackerInfo> entry in ObjectTracker.ObjTracker)
                {
                    var info = entry.Value;

                    if (entry.Value.Name == "RobotBuddy")
                    {
                        if (info.Obj == null || !info.Obj.IsValid || info.Obj.IsDead || info.Obj.IsVisible)
                        {
                            continue;
                        }
                        else
                        {
                            Vector3 endPos2 = info.Obj.Position.Extend(args.End, spellData.Range).To3DWorld();
                            SpellDetector.CreateSpellData(hero, info.Obj.Position, endPos2, spellData, null, 0, false);
                        }
                    }
                }
            }
        }
    }
}
