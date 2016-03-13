using System;
using AdEvade.Config;
using AdEvade.Data;
using AdEvade.Utils;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace AdEvade.Helpers
{
    public static class Situation
    {
        private static AIHeroClient MyHero { get { return ObjectManager.Player; } }
        
        static Situation()
        {

        }

        public static bool IsNearEnemy(this Vector2 pos, float distance, bool alreadyNear = true)
        {
            if (ConfigValue.PreventDodgingNearEnemy.GetBool())
            {
                var curDistToEnemies = GameData.HeroInfo.ServerPos2D.GetDistanceToChampions();
                var posDistToEnemies = pos.GetDistanceToChampions();
                
                if (curDistToEnemies < distance)
                {
                    if (curDistToEnemies > posDistToEnemies)
                    {
                        return true;
                    }
                }
                else
                {
                    if (posDistToEnemies < distance)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
                
        public static bool IsUnderTurret(this Vector2 pos, bool checkEnemy = true)
        {
            if (!ConfigValue.PreventDodgingUnderTower.GetBool())
            {
                return false;
            }

            var turretRange = 875 + GameData.HeroInfo.BoundingRadius;

            foreach (var entry in GameData.Turrets)
            {
                var turret = entry.Value;
                if (turret == null || !turret.IsValid || turret.IsDead)
                {
                    Core.DelayAction(() => GameData.Turrets.Remove(entry.Key), 1000);
                    continue;
                }

                if (checkEnemy && turret.IsAlly)
                {
                    continue;
                }

                var distToTurret = pos.Distance(turret.Position.To2D());
                if (distToTurret <= turretRange)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool ShouldDodge()
        {
            if (!ConfigValue.DodgeSkillShots.GetBool() || CommonChecks())
            {
                //TODO: ADD CHECKS FOR BELOW Into Champion Addons
                //has spellshield - sivir, noc, morgana
                //vlad pool
                //tryndamere r?
                //kayle ult buff?
                //hourglass
                //invulnerable
                //rooted
                //sion ult -> tenacity = 100?
                //stunned
                //elise e
                //zilean ulted
                //isdashing

                return false;
            }



            return true;
        }

        public static bool ShouldUseEvadeSpell()
        {
            return ConfigValue.ActivateEvadeSpells.GetBool() && !CommonChecks() && !(AdEvade.LastWindupTime - EvadeUtils.TickCount > 0);
        }

        public static bool CommonChecks()
        {
            return

                AdEvade.IsChanneling
                || MyHero.IsDead
                || MyHero.IsInvulnerable
                || !MyHero.IsTargetable
                //|| HasSpellShield(MyHero)
                //|| ChampionSpecificChecks()
                || Player.Instance.IsDashing()
                || AdEvade.HasGameEnded;
        }

        public static bool ChampionSpecificChecks()
        {
            return MyHero.ChampionName == "Sion" && MyHero.HasBuff("SionR");
            //TODO
            //Untargetable
            //|| (myHero.CharName == "KogMaw" && myHero.HasBuff("kogmawicathiansurprise"))
            //|| (myHero.CharName == "Karthus" && myHero.HasBuff("KarthusDeathDefiedBuff"))

            //Invulnerable
            //|| myHero.HasBuff("kalistarallyspelllock"); 
        }

        public static bool HasSpellShield(AIHeroClient unit)
        {
            if (ObjectManager.Player.HasBuffOfType(BuffType.SpellShield))
            {
                return true;
            }

            if (ObjectManager.Player.HasBuffOfType(BuffType.SpellImmunity))
            {
                return true;
            }


            //TODO:
            ////Sivir E
            //if (unit.LastCastedSpellName() == "SivirE" && (EvadeUtils.TickCount - Evade.lastSpellCastTime) < 300)
            //{
            //    return true;
            //}

            ////Morganas E
            //if (unit.LastCastedSpellName() == "BlackShield" && (EvadeUtils.TickCount - Evade.lastSpellCastTime) < 300)
            //{
            //    return true;
            //}

            ////Nocturnes E
            //if (unit.LastCastedSpellName() == "NocturneShit" && (EvadeUtils.TickCount - Evade.lastSpellCastTime) < 300)
            //{
            //    return true;
            //}
            return false;
        }

    }
}
