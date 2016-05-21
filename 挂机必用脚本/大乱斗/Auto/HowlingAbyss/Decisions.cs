using System.Linq;
using AutoSharp.Utils;
using EloBuddy;
using EloBuddy.SDK;

namespace AutoSharp.Auto.HowlingAbyss
{
    internal static class Decisions
    {
        internal static bool HealUp()
        {
            if (Heroes.Player.IsDead)
            {
                Orbwalker.ActiveModesFlags = Orbwalker.ActiveModes.None;
                return true;
            }

            if (Heroes.Player.HealthPercent >= 60) return false;

            var closestEnemyBuff = HealingBuffs.EnemyBuffs.FirstOrDefault(eb => eb.IsVisible && eb.IsValid && eb.Position.Distance(Heroes.Player.Position) < 800 && (eb.Position.CountEnemiesInRange(600) == 0 || eb.Position.CountEnemiesInRange(600) < eb.Position.CountAlliesInRange(600)));
            var closestAllyBuff = HealingBuffs.AllyBuffs.FirstOrDefault(ab => ab.IsVisible && ab.IsValid);


            //BUFF EXISTANCE CHECKS;
            if ((closestAllyBuff == null && closestEnemyBuff == null)) return false;

            //BECAUSE WE CHECKED THAT BUFFS CAN'T BE BOTH NULL; IF ONE OF THEM IS NULL IT MEANS THE OTHER ISN'T.
            // ReSharper disable once PossibleNullReferenceException
            var buffPos = closestEnemyBuff != null ? closestEnemyBuff.Position.Randomize(0, 15) : closestAllyBuff.Position.Randomize(0,15);

            if (Heroes.Player.Position.Distance(buffPos) <= 800 && (Heroes.Player.CountEnemiesInRange(800) == 0 || Heroes.Player.CountEnemiesInRange(800) < Heroes.Player.Position.CountAlliesInRange(800)))
            {
                DecisionMaker.Goto(buffPos);
                return true;
            }

            //stay in fight if you can't instantly gratify yourself and u don't really need the buff
            if (Heroes.Player.HealthPercent >= 30 && Heroes.Player.CountEnemiesInRange(900) <= Player.Instance.Position.CountAlliesInRange(900) && Heroes.Player.Distance(buffPos) > 1000) return false;

            //IF BUFFPOS IS VECTOR ZERO OR NOT VALID SOMETHING MUST HAVE GONE WRONG
            if (!buffPos.IsValid()) return false;

            //MOVE TO BUFFPOS
            DecisionMaker.Goto(buffPos);

            //STOP EVERYTHING ELSE TO DO THIS
            return true;
        }

        internal static bool Farm()
        {
            var minion = Wizard.GetFarthestMinion();
            var minionPos = minion != null ? minion.Position.Extend(HeadQuarters.AllyHQ.Position, 250).RandomizePosition() : Wizard.GetFarthestAllyTurret().RandomizePosition();
            //IF THERE ARE ALLIES AROUND US STOP ORBWALKING AROUND THE TURRET LIKE A RETARD
            if (Heroes.Player.Distance(Wizard.GetFarthestAllyTurret().Position) < 500 && Heroes.Player.Position.CountAlliesInRange(1000) != 0 && Minions.AllyMinions.Count < 3) return false;
            //IF THERE ARE ENEMIES AROUND US OR THE MINION WE WONT FOLLOW HIM, WE WILL FIGHT!
            if ((minionPos.CountEnemiesInRange(1000) != 0 || Heroes.Player.CountEnemiesInRange(1000) != 0) && minionPos.CountAlliesInRange(1000) != 0) return false;
            //IF THE FARTHEST ALLY IS IN DANGER, WE SHALL FIGHT WITH HIM
            if (Heroes.AllyHeroes.OrderByDescending(h => h.Distance(HeadQuarters.AllyHQ)).FirstOrDefault().CountEnemiesInRange(1400) != 0) return false;
            //IF WERE FUGGD WE WILL FIGHT SKIP FARMING CUZ WE CANT FARM WHILE FUGGING XDD
            if (Heroes.Player.CountEnemiesInRange(1000) > Heroes.Player.Position.CountAlliesInRange(1000)) return false;
            //FOLLOW MINION
            DecisionMaker.Goto(minionPos.RandomizePosition());
            Orbwalker.ActiveModesFlags = Orbwalker.ActiveModes.LaneClear;
            //IF I JUST FARM A BIT GUYS WE MIGHT WIN...
            return true;
        }

        internal static void Fight()
        {
            Orbwalker.ActiveModesFlags = Heroes.Player.CountEnemiesInRange(Heroes.Player.AttackRange) == 0 ? Orbwalker.ActiveModes.LaneClear : Orbwalker.ActiveModes.Combo;
            DecisionMaker.Goto(Positioning.RandomlyChosenMove);
        }

        internal static bool ImSoLonely()
        {
            if (Heroes.AllyHeroes.All(h => h.IsDead) || Heroes.AllyHeroes.All(h=>h.InFountain()) || (Heroes.AllyHeroes.All(h => h.Distance(HeadQuarters.AllyHQ) < Heroes.Player.Distance(h))))
            {
                //Chat.Print("cuck");
                DecisionMaker.Goto(Wizard.GetFarthestAllyTurret().Position.RandomizePosition());
                Orbwalker.ActiveModesFlags = Heroes.Player.Distance(Wizard.GetFarthestAllyTurret().Position) < 750
                    ? Orbwalker.ActiveModes.LaneClear
                    : Orbwalker.ActiveModes.LaneClear;
                return true;
            }
            return false;
        }
    }
}
