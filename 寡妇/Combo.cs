using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace Evelynn
{
    internal static class Combo
    {
        private static Obj_AI_Base GetEnemy(float range, GameObjectType t)
        {
            switch (t)
            {
                case GameObjectType.AIHeroClient:
                    return EntityManager.Heroes.Enemies.OrderBy(a => a.Health).FirstOrDefault(
                        a => a.Distance(Player.Instance) < range && !a.IsDead && !a.IsInvulnerable);
                default:
                    return EntityManager.MinionsAndMonsters.EnemyMinions.OrderBy(a => a.Health).FirstOrDefault(
                        a => a.Distance(Player.Instance) < range && !a.IsDead && !a.IsInvulnerable);
            }
        }

        public static void EveCombo()
        {
            var qcheck = Program.ComboMenu["usecomboq"].Cast<CheckBox>().CurrentValue;
            var echeck = Program.ComboMenu["usecomboe"].Cast<CheckBox>().CurrentValue;
            var wcheck = Program.ComboMenu["usecombow"].Cast<CheckBox>().CurrentValue;
            var qready = Program.Q.IsReady();
            var wready = Program.W.IsReady();
            var eready = Program.E.IsReady();


            if (qcheck && qready)
            {
                var enemy = GetEnemy(Program.Q.Range, GameObjectType.AIHeroClient);

                if (enemy != null)
                    Program.Q.Cast();
            }

            if (echeck && eready) 
            {
                var enemy = GetEnemy(Program.E.Range, GameObjectType.AIHeroClient);

                if (enemy != null)
                    Program.E.Cast(enemy);
            }
            if (wcheck && wready) 
            {
                Program.W.Cast();
            }
        }
    }
}