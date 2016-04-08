using System;
using EloBuddy;
using EloBuddy.SDK;

namespace AdEvade.Data.Spells.SpecialSpells
{
    class Fizz : IChampionPlugin
    {
        static Fizz()
        {

        }
        public const string ChampionName = "Fizz";
        public string GetChampionName()
        {
            return ChampionName;
        }
        public void LoadSpecialSpell(SpellData spellData)
        {
            if (spellData.SpellName == "FizzMarinerDoom")
            {
                GameObject.OnCreate += (obj, args) => OnCreateObj_FizzMarinerDoom(obj, args, spellData);
                GameObject.OnDelete += (obj, args) => OnDeleteObj_FizzMarinerDoom(obj, args, spellData);
            }

            if (spellData.SpellName == "FizzPiercingStrike")
            {
                SpellDetector.OnProcessSpecialSpell += ProcessSpell_FizzPiercingStrike;
            }
        }

        private static void ProcessSpell_FizzPiercingStrike(Obj_AI_Base hero, GameObjectProcessSpellCastEventArgs args, SpellData spellData, SpecialSpellEventArgs specialSpellArgs)
        {
            if (spellData.SpellName == "FizzPiercingStrike")
            {
                if (args.Target != null && args.Target.IsMe)
                {
                    SpellDetector.CreateSpellData(hero, args.Start, args.End, spellData, null, 0);
                }

                specialSpellArgs.NoProcess = true;
            }
        }

        private static void OnDeleteObj_FizzMarinerDoom(GameObject obj, EventArgs args, SpellData spellData)
        {
            //need to track where bait is attached to

            if (obj.GetType() != typeof(MissileClient) || !((MissileClient) obj).IsValidMissile())
                return;

            MissileClient missile = (MissileClient)obj;

            if (missile.SpellCaster != null && missile.SpellCaster.Team != ObjectManager.Player.Team &&
                missile.SData.Name == "FizzMarinerDoomMissile")
            {
                SpellDetector.CreateSpellData(missile.SpellCaster, missile.StartPosition, missile.EndPosition,
                spellData, null, 1000, true, SpellType.Circular, false, 350);
            }
        }

        private static void OnCreateObj_FizzMarinerDoom(GameObject obj, EventArgs args, SpellData spellData)
        {
            if (obj.GetType() != typeof(MissileClient) || !((MissileClient) obj).IsValidMissile())
                return;

            MissileClient missile = (MissileClient)obj;

            if (missile.SpellCaster != null && missile.SpellCaster.Team != ObjectManager.Player.Team &&
                missile.SData.Name == "FizzMarinerDoomMissile")
            {
                SpellDetector.CreateSpellData(missile.SpellCaster, missile.StartPosition, missile.EndPosition,
                spellData, null, 500, true, SpellType.Circular, false, spellData.SecondaryRadius);

                /*foreach (KeyValuePair<int, Spell> entry in SpellDetector.spells)
                {
                    var spell = entry.Value;

                    if (spell.info.spellName == "FizzMarinerDoom" &&
                        spell.spellObject != null && spell.spellObject.NetworkId == missile.NetworkId)
                    {
                        if (spell.spellType == SpellType.Circular)
                        {                            
                            spell.spellObject = null;
                        }
                    }
                }*/
            }
        }
    }
}
