using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SebbyLib;
using SharpDX;
using Spell = LeagueSharp.Common.Spell;

namespace OneKeyToWin_AIO_Sebby.Core
{
    internal class OKTWdash
    {
        private static readonly Menu Config = Program.Config;
        private static Menu Sub;
        private static Spell DashSpell;

        public OKTWdash(Spell qwer)
        {
            DashSpell = qwer;

            Sub = Config.AddSubMenu(qwer.Slot + " 冲刺设置");
            Sub.Add("DashMode", new Slider("冲刺 模式 (0 : 鼠标 | 1 : 边上 | 2 : 安全位置)", 2, 0, 2));
            Sub.Add("EnemyCheck", new Slider("X 敌人时防止冲进", 3, 0, 5));
            Sub.Add("WallCheck", new CheckBox("防止撞墙"));
            Sub.Add("TurretCheck", new CheckBox("防止进塔"));
            Sub.Add("AAcheck", new CheckBox("只在普攻范围内冲刺"));
            Sub.AddSeparator();
            Sub.AddGroupLabel("接近/防突进");
            Sub.Add("GapcloserMode", new Slider("接近/防突进 模式 (0 : 鼠标 | 1 : 远离 - 安全位置 | 2 : 关闭)", 1, 0, 2));

            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
            {
                Sub.Add("EGCchampion" + enemy.NetworkId, new CheckBox("使用在 " + enemy.ChampionName));
            }

            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public static bool getCheckBoxItem(string item)
        {
            return Sub[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(string item)
        {
            return Sub[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(string item)
        {
            return Sub[item].Cast<KeyBind>().CurrentValue;
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (DashSpell.IsReady())
            {
                if (Sub["EGCchampion" + gapcloser.Sender.NetworkId] == null) { return; }
                if (getCheckBoxItem("EGCchampion" + gapcloser.Sender.NetworkId))
                {
                    var GapcloserMode = getSliderItem("GapcloserMode");
                    if (GapcloserMode == 0)
                    {
                        var bestpoint = Player.Position.LSExtend(Game.CursorPos, DashSpell.Range);
                        if (IsGoodPosition(bestpoint))
                            DashSpell.Cast(bestpoint);
                    }
                    else if (GapcloserMode == 1)
                    {
                        var points = OktwCommon.CirclePoints(10, DashSpell.Range, Player.Position);
                        var bestpoint = Player.Position.LSExtend(gapcloser.Sender.Position, -DashSpell.Range);
                        var enemies = bestpoint.CountEnemiesInRange(DashSpell.Range);
                        foreach (var point in points)
                        {
                            var count = point.CountEnemiesInRange(DashSpell.Range);
                            if (count < enemies)
                            {
                                enemies = count;
                                bestpoint = point;
                            }
                            else if (count == enemies && Game.CursorPos.LSDistance(point) < Game.CursorPos.LSDistance(bestpoint))
                            {
                                enemies = count;
                                bestpoint = point;
                            }
                        }
                        if (IsGoodPosition(bestpoint))
                            DashSpell.Cast(bestpoint);
                    }
                }
            }
        }

        public Vector3 CastDash(bool asap = false)
        {
            var DashMode = getSliderItem("DashMode");

            var bestpoint = Vector3.Zero;
            if (DashMode == 0)
            {
                bestpoint = Player.Position.LSExtend(Game.CursorPos, DashSpell.Range);
            }
            else if (DashMode == 1)
            {
                var orbT = Orbwalker.LastTarget;
                if (orbT is AIHeroClient)
                {
                    var start = Player.Position.To2D();
                    var end = orbT.Position.To2D();
                    var dir = (end - start).Normalized();
                    var pDir = dir.Perpendicular();

                    var rightEndPos = end + pDir * Player.LSDistance(orbT);
                    var leftEndPos = end - pDir * Player.LSDistance(orbT);

                    var rEndPos = new Vector3(rightEndPos.X, rightEndPos.Y, Player.Position.Z);
                    var lEndPos = new Vector3(leftEndPos.X, leftEndPos.Y, Player.Position.Z);

                    bestpoint = Game.CursorPos.LSDistance(rEndPos) < Game.CursorPos.LSDistance(lEndPos) ? Player.Position.LSExtend(rEndPos, DashSpell.Range) : Player.Position.LSExtend(lEndPos, DashSpell.Range);
                }
            }
            else if (DashMode == 2)
            {
                var points = OktwCommon.CirclePoints(15, DashSpell.Range, Player.Position);
                bestpoint = Player.Position.LSExtend(Game.CursorPos, DashSpell.Range);
                var enemies = bestpoint.CountEnemiesInRange(350);
                foreach (var point in points)
                {
                    int count = point.CountEnemiesInRange(350);
                    if (!InAARange(point))
                        continue;
                    if (point.UnderAllyTurret())
                    {
                        bestpoint = point;
                        enemies = count - 1;
                    }
                    else if (count < enemies)
                    {
                        enemies = count;
                        bestpoint = point;
                    }
                    else if (count == enemies && Game.CursorPos.LSDistance(point) < Game.CursorPos.LSDistance(bestpoint))
                    {
                        enemies = count;
                        bestpoint = point;
                    }
                }
            }

            if (bestpoint.IsZero)
                return Vector3.Zero;

            var isGoodPos = IsGoodPosition(bestpoint);

            if (asap && isGoodPos)
            {
                return bestpoint;
            }
            if (isGoodPos && InAARange(bestpoint))
            {
                return bestpoint;
            }
            return Vector3.Zero;
        }

        public bool InAARange(Vector3 point)
        {
            if (!getCheckBoxItem("AAcheck"))
                return true;
            if (Orbwalker.LastTarget != null && Orbwalker.LastTarget.Type == GameObjectType.AIHeroClient)
            {
                return point.LSDistance(Orbwalker.LastTarget.Position) < Player.AttackRange;
            }
            return point.CountEnemiesInRange(Player.AttackRange) > 0;
        }

        public bool IsGoodPosition(Vector3 dashPos)
        {
            if (getCheckBoxItem("WallCheck"))
            {
                var segment = DashSpell.Range / 5;
                for (var i = 1; i <= 5; i++)
                {
                    if (Player.Position.LSExtend(dashPos, i * segment).LSIsWall())
                        return false;
                }
            }

            if (getCheckBoxItem("TurretCheck"))
            {
                if (dashPos.UnderTurret(true))
                    return false;
            }

            var enemyCheck = getSliderItem("EnemyCheck");
            var enemyCountDashPos = dashPos.CountEnemiesInRange(600);

            if (enemyCheck > enemyCountDashPos)
                return true;

            var enemyCountPlayer = Player.CountEnemiesInRange(400);

            if (enemyCountDashPos <= enemyCountPlayer)
                return true;

            return false;
        }
    }
}
