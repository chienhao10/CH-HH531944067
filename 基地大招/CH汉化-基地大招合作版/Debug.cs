using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace HumanziedBaseUlt
{
    static class Debug
    {
        private static AIHeroClient currentTarget;
        private static float estimatedDamage;
        private static float estimatedReg;

        private static float lastEnemyHealth;

        private static bool finished = false;

        public static void Init(AIHeroClient t, float _estimatedReg, float _estimatedDamage)
        {
            finished = false;

            lastEnemyHealth = t.Health;
            
            currentTarget = t;
            estimatedDamage = _estimatedDamage;
            estimatedReg = _estimatedReg;
            AIHeroClient.OnDamage += AiHeroClientOnOnDamage;
        }

        private static void AiHeroClientOnOnDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            if (!(args.Target is AIHeroClient) || !sender.IsMe)
                return;

            var enemySpawnPos = ObjectManager.Get<Obj_SpawnPoint>().First(x => x.IsEnemy).Position;
            if (args.Target.Distance(enemySpawnPos) <= 1000)
            {
                OnTargetHitInBase(args.Target as AIHeroClient, args.Damage);
            }
        }

        private static void OnTargetHitInBase(AIHeroClient _target, float damage)
        {
            if (finished)
                return;

            finished = true;
            Chat.Print("[Debug] Target found as " + _target.ChampionName);

            //Chat.Print("Esitmated Health: " + estimatedHealth + "//Real Health: " + currentTarget.Health);
            //==> Regeneration is responsible for health deviation
            Chat.Print("[Debug] Esitmated Reg: " + estimatedReg + "//Real Reg: " + (currentTarget.Health - lastEnemyHealth));
            Chat.Print("[Debug] Esitmated Damage: " + estimatedDamage + "//Real Damage: " + damage);
            //==> Dmg calc is mostly ok
            AIHeroClient.OnDamage -= AiHeroClientOnOnDamage;
        }
    }
}
