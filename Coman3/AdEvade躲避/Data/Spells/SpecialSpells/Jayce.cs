using System;
using System.Linq;
using AdEvade.Utils;
using EloBuddy;
using EloBuddy.SDK;

namespace AdEvade.Data.Spells.SpecialSpells
{
    class Jayce : IChampionPlugin
    {
        static Jayce()
        {

        }
        public const string ChampionName = "Jayce";
        public string GetChampionName()
        {
            return ChampionName;
        }
        public void LoadSpecialSpell(SpellData spellData)
        {
            if (spellData.SpellName == "JayceShockBlastWall")
            {
                AIHeroClient hero = EntityManager.Heroes.Enemies.FirstOrDefault(h => h.ChampionName == "Jayce");
                if (hero == null)
                {
                    return;
                }

                Obj_AI_Minion.OnCreate += (obj, args) => OnCreateObj_jayceshockblast(obj, args, hero, spellData);
                //AIHeroClient.OnProcessSpellCast += OnProcessSpell_jayceshockblast;
                //SpellDetector.OnProcessSpecialSpell += ProcessSpell_jayceshockblast;
            }
        }

        private static void OnCreateObj_jayceshockblast(GameObject obj, EventArgs args, AIHeroClient hero, SpellData spellData)
        {

            if (obj.IsEnemy && obj.Type == GameObjectType.obj_GeneralParticleEmitter
                && obj.Name.Contains("Jayce") && obj.Name.Contains("accel_gate_start"))
            {
                var dir = ObjectTracker.GetLastHiuOrientation();
                var pos1 = obj.Position.To2D() - dir * 470;
                var pos2 = obj.Position.To2D() + dir * 470;

                var gateTracker = new ObjectTrackerInfo(obj, "AccelGate");
                gateTracker.Direction = dir.To3D();

                ObjectTracker.ObjTracker.Add(obj.NetworkId, gateTracker);

                foreach (var entry in SpellDetector.Spells)
                {
                    var spell = entry.Value;

                    if (spell.Info.SpellName == "JayceShockBlast")
                    {
                        var tHero = spell.HeroId;

                        var intersection = spell.StartPos.Intersection(spell.EndPos, pos1, pos2);
                        var projection = intersection.Point.ProjectOn(spell.StartPos, spell.EndPos);

                        if (intersection.Intersects && projection.IsOnSegment)
                        {
                            SpellDetector.CreateSpellData(hero, intersection.Point.To3D(), spell.EndPos.To3D(), spellData, spell.SpellObject);

                            DelayAction.Add(1, () => SpellDetector.DeleteSpell(entry.Key));
                        }
                    }
                }

                SpellDetector.CreateSpellData(hero, pos1.To3D(), pos2.To3D(), spellData, null, 0);                               
            }
        }

        private static void OnProcessSpell_jayceshockblast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsEnemy && args.SData.Name == "jayceaccelerationgate")
            {
                //ObjectTracker.objTracker.Add(obj.NetworkId, new ObjectTrackerInfo(obj, "RobotBuddy"));

                //AddObjectTracker.objTrackerPosition
            }
        }

        private static void ProcessSpell_jayceshockblast(Obj_AI_Base hero, GameObjectProcessSpellCastEventArgs args, SpellData spellData, SpecialSpellEventArgs specialSpellArgs)
        {
            if (spellData.SpellName == "jayceshockblast")
            {
            }
        }
    }
}
