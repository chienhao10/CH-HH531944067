using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy;

namespace ARAMDetFull
{
    class Sector
    {
        private Vector2 center;
        private Polygon polig;

        /* Sector Info */
        public int myMinCount = 0;
        public int enemMinCount = 0;
        public bool inNonActiveEnemyTower = false;

        public int enemyReachIn = 0;
        public int enemyDangerReachIn = 0;

        public bool containsAllyMinion = false;
        public bool containsAlly = false;
        public bool containsEnemy = false;
        public bool containsEnemyChamp = false;
        public AIHeroClient enemyChampIn = null;
        public bool containsAllyChamp = false;
        public AttackableUnit enem = null;
        public bool dangerPolig = false;

        public Sector(Vector2 start, Vector2 end, float W)
        {
            center = (start + end)/2;
            List<Vector2> points = new List<Vector2>();

            Vector2 p = (end - start);
            var per = p.LSPerpendicular().LSNormalized() * (W);
            points.Add(start + per);
            points.Add(start - per);
            points.Add(end - per);
            points.Add(end + per);

            polig = new Polygon(points);

        }

        public void draw()
        {
            polig.Draw(Color.Red,2);
            LeagueSharp.Common.Utility.DrawCircle(center.To3D(), 50, Color.Violet);
        }

        public void update()
        {
            enemyChampIn = null;
            containsAlly = false;
            containsAllyMinion = false;
            containsEnemy = false;
            dangerPolig = false;
            containsAllyChamp = false;
            containsEnemyChamp = false;
            enem = null;
            int dangLVL = 0;
            foreach (var obj in ObjectManager.Get<AttackableUnit>())
            {
                if(obj.IsDead || obj.Health==0 || !obj.IsValid || !obj.IsVisible || obj.IsInvulnerable || obj.IsMe)
                    continue;

                if((!containsAlly || !containsAllyMinion) && obj.IsAlly && !obj.IsDead )
                    if (polig.pointInside(obj.Position.LSTo2D()))
                    {
                        containsAlly = true;
                        if (obj is Obj_AI_Minion)
                            containsAllyMinion = true;
                    }

                if (!containsEnemy && obj.IsEnemy && obj is Obj_AI_Minion && !MapControl.fightIsClose() && !ARAMTargetSelector.IsInvulnerable(ObjectManager.Player) && !Aggresivity.getIgnoreMinions())
                    if (!obj.IsDead && ((Obj_AI_Minion)obj).BaseSkinName != "GangplankBarrel" && obj.Health > 0 && obj.LSIsValidTarget() && polig.pointInside(obj.Position.LSTo2D()))
                    {
                        containsEnemy = true;
                        enem = obj;
                    }
                if (containsEnemy && containsAlly)
                    break;
            }

            foreach (var en_chemp in MapControl.enemy_champions)
            {
                en_chemp.getReach();
                //Console.WriteLine(en_chemp.reach);
                if (!en_chemp.hero.IsDead  && (en_chemp.hero.IsVisible || ARAMSimulator.deepestAlly.IsMe) && (sectorInside(en_chemp.hero.Position.LSTo2D(), en_chemp.hero.AttackRange + 120) ||
                    sectorInside(en_chemp.hero.Position.LSTo2D(), en_chemp.reach)))
                {
                    dangLVL++;
                    containsEnemyChamp = true;
                    if (enemyChampIn == null || (enemyChampIn.Health > en_chemp.hero.Health && !enemyChampIn.IsZombie && !ARAMTargetSelector.IsInvulnerable(enemyChampIn)))
                        enemyChampIn = en_chemp.hero;
                }
            }

            foreach (var al_chemp in MapControl.ally_champions)
            {
                if (al_chemp.hero.LSIsValidTarget() && !al_chemp.hero.IsDead && polig.pointInside(al_chemp.hero.Position.LSTo2D()))
                {
                    dangLVL--;
                    containsAllyChamp = true;
                }
            }
            if (dangLVL > 0)
                dangerPolig = true;

            foreach (var turret in ObjectManager.Get<Obj_AI_Turret>())
            {
                if (turret.IsEnemy && !turret.IsDead && turret.IsValid && sectorInside(turret.Position.LSTo2D(), 1100) && !towerContainsAlly(turret))
                {
                    dangerPolig = true;
                    break;
                }
            }

        }

        public static bool towerContainsAlly(Obj_AI_Turret tow)
        {
            int count = 0;
            foreach (var mins in ObjectManager.Get<Obj_AI_Base>().Where(ob => ob.IsAlly && !ob.IsDead && ob.IsTargetable && !ob.IsMe))
            {
                if (mins.LSDistance(tow, true) < 800*800)
                    count += (mins is AIHeroClient) ? 2 : 1;
            }
            return count>2;
        }

        public static bool inTowerRange(Vector2 pos)
        {
            //  if (!YasuoSharp.Config.Item("djTur").GetValue<bool>())
            //      return false;
            return pos.To3D().UnderTurret(true);
           
        }


        public bool sectorInside(Vector2 pos, float range)
        {
            return center.LSDistance(pos, true) <= range*range;
        }

        public Vector2 getRandomPointIn(bool rand = false)
        {
            if (enemyChampIn != null && ARAMSimulator.player.IsMelee && !enemyChampIn.Position.UnderTurret(true))
                return enemyChampIn.Position.LSTo2D().LSExtend(ARAMSimulator.player.Position.LSTo2D(),ARAMSimulator.player.AttackRange/2);

            Vector2 result = new Vector2();
            if (containsEnemy && enem != null && ARAMSimulator.player.IsMelee() && !rand)
                result = enem.Position.LSTo2D();
            else
            {
                Random r = new Random();
                int x = 200 - r.Next(400);
                int y = 200 - r.Next(400);


                result = new Vector2(center.X + x, center.Y + y);
                int count = 0;
                while (inTowerRange(result))
                {
                    count++;
                     x = 200 - r.Next(400);
                     y = 200 - r.Next(400);

                    result = new Vector2(center.X + x, center.Y + y);
                    if(count>10)
                        break;
                }
            }
            return result;
        }


    }
}
