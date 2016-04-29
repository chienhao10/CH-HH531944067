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

            Sub = Config.AddSubMenu(qwer.Slot + " Dash Config");
            Sub.Add("DashMode", new Slider("Dash MODE (0 : Cursor | 1 : Side | 2 : Safe Pos)", 2, 0, 2));
            Sub.Add("EnemyCheck", new Slider("Block dash in x enemies", 3, 0, 5));
            Sub.Add("WallCheck", new CheckBox("Block dash in wall"));
            Sub.Add("TurretCheck", new CheckBox("Block dash under turret"));
            Sub.Add("AAcheck", new CheckBox("Dash only in AA range"));
            Sub.AddSeparator();
            Sub.AddGroupLabel("Gapcloser");
            Sub.Add("GapcloserMode",
                new Slider("Gapcloser MODE (0 : Cursor | 1 : Away - Safe Pos | 2 : Disable)", 1, 0, 2));

            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
            {
                Sub.Add("EGCchampion" + enemy.ChampionName, new CheckBox("Gapclose " + enemy.ChampionName));
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
            if (DashSpell.IsReady() && getCheckBoxItem("EGCchampion" + gapcloser.Sender.ChampionName))
            {
                var GapcloserMode = getSliderItem("GapcloserMode");
                if (GapcloserMode == 0)
                {
                    var bestpoint = Player.Position.Extend(Game.CursorPos, DashSpell.Range).To3D();
                    if (IsGoodPosition(bestpoint))
                        DashSpell.Cast(bestpoint);
                }
                else if (GapcloserMode == 1)
                {
                    var points = OktwCommon.CirclePoints(10, DashSpell.Range, Player.Position);
                    var bestpoint = Player.Position.Extend(gapcloser.Sender.Position, -DashSpell.Range).To3D();
                    var enemies = bestpoint.CountEnemiesInRange(DashSpell.Range);
                    foreach (var point in points)
                    {
                        var count = point.CountEnemiesInRange(DashSpell.Range);
                        if (count < enemies)
                        {
                            enemies = count;
                            bestpoint = point;
                        }
                        else if (count == enemies && Game.CursorPos.Distance(point) < Game.CursorPos.Distance(bestpoint))
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

        public Vector3 CastDash(bool asap = false)
        {
            var DashMode = getSliderItem("DashMode");

            var bestpoint = Vector3.Zero;
            if (DashMode == 0)
            {
                bestpoint = Player.Position.Extend(Game.CursorPos, DashSpell.Range).To3D();
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

                    var rightEndPos = end + pDir*Player.Distance(orbT);
                    var leftEndPos = end - pDir*Player.Distance(orbT);

                    var rEndPos = new Vector3(rightEndPos.X, rightEndPos.Y, Player.Position.Z);
                    var lEndPos = new Vector3(leftEndPos.X, leftEndPos.Y, Player.Position.Z);

                    bestpoint = Game.CursorPos.Distance(rEndPos) < Game.CursorPos.Distance(lEndPos) ? Player.Position.Extend(rEndPos, DashSpell.Range).To3D() : Player.Position.Extend(lEndPos, DashSpell.Range).To3D();
                }
            }
            else if (DashMode == 2)
            {
                var points = OktwCommon.CirclePoints(15, DashSpell.Range, Player.Position);
                bestpoint = Player.Position.Extend(Game.CursorPos, DashSpell.Range).To3D();
                var enemies = bestpoint.CountEnemiesInRange(350);
                foreach (var point in points)
                {
                    var count = point.CountEnemiesInRange(350);
                    if (!InAARange(point))
                        continue;
                    if (count < enemies)
                    {
                        enemies = count;
                        bestpoint = point;
                    }
                    else if (count == enemies && Game.CursorPos.Distance(point) < Game.CursorPos.Distance(bestpoint))
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
                return point.Distance(Orbwalker.LastTarget.Position) < Player.AttackRange;
            }
            return point.CountEnemiesInRange(Player.AttackRange) > 0;
        }

        public bool IsGoodPosition(Vector3 dashPos)
        {
            if (getCheckBoxItem("WallCheck"))
            {
                var segment = DashSpell.Range/5;
                for (var i = 1; i <= 5; i++)
                {
                    if (Player.Position.Extend(dashPos, i*segment).LSIsWall())
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