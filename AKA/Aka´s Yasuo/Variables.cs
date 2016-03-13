
using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace AkaYasuo
{
    class Variables
    {
        public static YasWall wall = new YasWall();

        public const int QRange = 550, Q2Range = 1150, QCirWidth = 275, QCirWidthMin = 250, RWidth = 400;

        public static Vector3 castFrom;

        public static bool IsDashing = false;

        public static bool wallCasted;

        public static BuffType[] buffs;

        public static string[] interrupt;

        public static AIHeroClient _Player { get { return ObjectManager.Player; } }

        public static int[] abilitySequence;

        public static int QOff = 0, WOff = 0, EOff = 0, ROff = 0;

        public static List<Skillshot> DetectedSkillShots = new List<Skillshot>();

        public static List<Skillshot> EvadeDetectedSkillshots = new List<Skillshot>();

        public static float HealthPercent { get { return _Player.Health / _Player.MaxHealth * 100; } }

        internal class YasWall
        {
            public MissileClient pointL;
            public MissileClient pointR;
            public float endtime = 0;
            public YasWall()
            {

            }

            public YasWall(MissileClient L, MissileClient R)
            {
                pointL = L;
                pointR = R;
                endtime = Game.Time + 4;
            }

            public void setR(MissileClient R)
            {
                pointR = R;
                endtime = Game.Time + 4;
            }

            public void setL(MissileClient L)
            {
                pointL = L;
                endtime = Game.Time + 4;
            }

            public bool isValid(int time = 0)
            {
                return pointL != null && pointR != null && endtime - (time / 1000) > Game.Time;
            }
        }

        public static bool skillShotIsDangerous(string Name)
        {
            if (MenuManager.SubMenu["SMIN"]["IsDangerous" + Name] != null)
            {
                return MenuManager.SubMenu["SMIN"]["IsDangerous" + Name].Cast<CheckBox>().CurrentValue;
            }
            return true;
        }

        public static bool EvadeSpellEnabled(string Name)
        {
            if (MenuManager.SubMenu["SMIN"]["Enabled" + Name] != null)
            {
                return MenuManager.SubMenu["SMIN"]["Enabled" + Name].Cast<CheckBox>().CurrentValue;
            }
            return true;
        }

        public static float GetQ2Delay
        {
            get
            {
                return 0.5f * (1 - Math.Min((_Player.AttackSpeedMod - 1) * 0.58f, 0.66f));
            }
        }

        public static float GetQDelay
        {
            get
            {
                return 0.4f * (1 - Math.Min((_Player.AttackSpeedMod - 1) * 0.58f, 0.66f));
            }
        }

        public static bool HaveQ3
        {
            get
            {
                return Player.HasBuff("YasuoQ3W");
            }
        }

        public static AIHeroClient QCirTarget
        {
            get
            {
                var pos = _Player.GetDashInfo().EndPos.To2D();
                var target = TargetSelector.GetTarget(QCirWidth, DamageType.Physical);
                return target != null && _Player.Distance(pos) < 150 ? target : null;
            }
        }

        public static bool CanCastE(Obj_AI_Base target)
        {
            return !target.HasBuff("YasuoDashWrapper");
        }

        public static bool CanCastR(AIHeroClient target)
        {
            return target.HasBuffOfType(BuffType.Knockup) || target.HasBuffOfType(BuffType.Knockback);
        }

        public static bool CastQCir(Obj_AI_Base target)
        {
            return target.IsValidTarget(QCirWidthMin - target.BoundingRadius) && Program.Q.Cast(Game.CursorPos);
        }

        public static Vector3 PosAfterE(Obj_AI_Base target)
        {
            return (Vector3)_Player.ServerPosition.Extend(
                target.ServerPosition,
                _Player.Distance(target) < 410 ? Program.E.Range : _Player.Distance(target) + 65);
        }

        public static bool CanCastDelayR(AIHeroClient target)
        {
            var buff = target.Buffs.FirstOrDefault(i => i.Type == BuffType.Knockback || i.Type == BuffType.Knockup);
            return buff != null && buff.EndTime - Game.Time <= (buff.EndTime - buff.StartTime) / 3;
        }

        public static bool AlliesNearTarget(Obj_AI_Base target, float range)
        {
            return EntityManager.Heroes.Allies.Where(tar => tar.Distance(target) < range).Any(tar => tar != null);
        }

        public static Obj_AI_Base GetNearObj(Obj_AI_Base target = null, bool inQCir = false)
        {
            var pos = target != null
                          ? Program.E2.GetPrediction(target).UnitPosition
                          : Game.CursorPos;
            var obj = new List<Obj_AI_Base>();
            obj.AddRange(EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, _Player.ServerPosition, Program.E.Range));
            obj.AddRange(EntityManager.Heroes.Enemies.Where(i => i.IsValidTarget(Program.E.Range)));
            return
                obj.Where(
                    i =>
                    CanCastE(i) && pos.Distance(PosAfterE(i)) < (inQCir ? QCirWidthMin : _Player.Distance(pos))
                     && EvadeManager.EvadeSkillshot.IsSafePoint(PosAfterE(i).To2D()).IsSafe)
                    .MinOrDefault(i => pos.Distance(PosAfterE(i)));
        }

        public static bool enemyIsJumpable(Obj_AI_Base enemy, List<AIHeroClient> ignore = null)
        {
            if (enemy.IsValid && enemy.IsEnemy && !enemy.IsInvulnerable && !enemy.MagicImmune && !enemy.IsDead &&
                !(enemy is FollowerObject))
            {
                if (ignore != null)
                    foreach (AIHeroClient ign in ignore)
                    {
                        if (ign.NetworkId == enemy.NetworkId)
                            return false;
                    }
                foreach (BuffInstance buff in enemy.Buffs)
                {
                    if (buff.Name == "YasuoDashWrapper")
                        return false;
                }
                return true;
            }
            return false;
        }

        public static bool isDashing
        {
            get
            {
                return _Player.IsDashing();
            }
        }
    }
}
