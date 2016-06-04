using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy;
using EloBuddy.SDK;

namespace ARAMDetFull
{
    public class LXOrbwalker
    {

        public static int now
        {
            get { return (int)DateTime.Now.TimeOfDay.TotalMilliseconds; }
        }

        private static readonly string[] AttackResets = { "mordekaisermaceofspades", "dariusnoxiantacticsonh", "fioraflurry", "garenq", "hecarimrapidslash", "jaxempowertwo", "jaycehypercharge", "leonashieldofdaybreak", "luciane", "lucianq", "monkeykingdoubleattack", "mordekaisermaceofspades", "nasusq", "nautiluspiercinggaze", "netherblade", "parley", "poppydevastatingblow", "powerfist", "renektonpreexecute", "rengarq", "shyvanadoubleattack", "sivirw", "takedown", "talonnoxiandiplomacy", "trundletrollsmash", "vaynetumble", "vie", "volibearq", "xenzhaocombotarget", "yorickspectral" };
        private static readonly string[] NoAttacks = { "jarvanivcataclysmattack", "monkeykingdoubleattack", "shyvanadoubleattack", "shyvanadoubleattackdragon", "zyragraspingplantattack", "zyragraspingplantattack2", "zyragraspingplantattackfire", "zyragraspingplantattack2fire" };
        private static readonly string[] Attacks = { "caitlynheadshotmissile", "frostarrow", "garenslash2", "kennenmegaproc", "lucianpassiveattack", "masteryidoublestrike", "quinnwenhanced", "renektonexecute", "renektonsuperexecute", "rengarnewpassivebuffdash", "trundleq", "xenzhaothrust", "viktorqbuff", "xenzhaothrust2", "xenzhaothrust3" };


        public static AIHeroClient MyHero = ObjectManager.Player;
        public static Obj_AI_Base ForcedTarget = null;
        public static IEnumerable<AIHeroClient> AllEnemys = ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy);
        public static IEnumerable<AIHeroClient> AllAllys = ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsAlly);

        public static List<Obj_AI_Turret> EnemyTowers = new List<Obj_AI_Turret>();

        public static List<Obj_BarracksDampener> EnemyBarracs = new List<Obj_BarracksDampener>();


        private static List<Obj_AI_Base> enemiesMinionsAround = new List<Obj_AI_Base>();

        public static List<Obj_HQ> EnemyHQ = new List<Obj_HQ>();

        public static bool CustomOrbwalkMode = false;

        public delegate void BeforeAttackEvenH(BeforeAttackEventArgs args);
        public delegate void OnTargetChangeH(Obj_AI_Base oldTarget, Obj_AI_Base newTarget);
        public delegate void AfterAttackEvenH(Obj_AI_Base unit, Obj_AI_Base target);
        public delegate void OnAttackEvenH(Obj_AI_Base unit, Obj_AI_Base target);

        public static event BeforeAttackEvenH BeforeAttack;
        public static event OnTargetChangeH OnTargetChange;
        public static event AfterAttackEvenH AfterAttack;
        public static event OnAttackEvenH OnAttack;

        public enum Mode
        {
            Combo,
            Harass,
            LaneClear,
            LaneFreeze,
            Lasthit,
            Flee,
            None,
        }

        private static bool _attack = true;
        private static bool _movement = true;
        private static bool _disableNextAttack;
        private const float LaneClearWaitTimeMod = 2f;
        private static int _lastAATick;
        private static Obj_AI_Base _lastTarget;
        private static LeagueSharp.Common.Spell _movementPrediction;
        private static int _lastMovement;
        private static int _delayAttackTill = 0;
        public static bool inDanger = false;


        public static float farmRange = 900;

        public static void setpOrbwalker()
        {
            _movementPrediction = new LeagueSharp.Common.Spell(SpellSlot.Unknown, GetAutoAttackRange());
            _movementPrediction.SetTargetted(MyHero.BasicAttack.SpellCastTime, MyHero.BasicAttack.MissileSpeed);
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
            GameObject.OnCreate += MissileClient_OnCreate;

            AllEnemys = ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy).ToList();
            AllAllys = ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsAlly).ToList();

            EnemyTowers = ObjectManager.Get<Obj_AI_Turret>().Where(tow => tow.IsEnemy).ToList();

            EnemyBarracs = ObjectManager.Get<Obj_BarracksDampener>().Where(tow => tow.IsEnemy).ToList();

            EnemyHQ = ObjectManager.Get<Obj_HQ>().Where(tow => tow.IsEnemy).ToList();
        }

        private static void MissileClient_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.IsMe)
            {
                var obj = (AIHeroClient)sender;
                if (obj.IsMelee())
                    return;
            }
            if (!(sender is MissileClient) || !sender.IsValid)
                return;
            var missile = (MissileClient)sender;
            if (missile.SpellCaster is AIHeroClient && missile.SpellCaster.IsValid && IsAutoAttack(missile.SData.Name))
                FireAfterAttack(missile.SpellCaster, _lastTarget);
        }

        public static void OrbwalkTo(Vector3 goalPosition, bool useDelay = true, bool onlyChamps = false)
        {
            CheckAutoWindUp();
            if (MyHero.IsChannelingImportantSpell() || CustomOrbwalkMode)
                return;
            var target = GetPossibleTarget(onlyChamps || Aggresivity.getIgnoreMinions());
            Orbwalk(goalPosition, target);
        }

        public static void getTheFukaway()
        {
            _delayAttackTill = LXOrbwalker.now + Game.Ping / 2 + 500;
        }

        public static void Orbwalk(Vector3 goalPosition, AttackableUnit target, bool useDelay = true)
        {
            try
            {
                if (target != null && target.IsValidTarget() && CanAttack() && IsAllowedToAttack())
                {
                    if (EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target))
                        _lastAATick = LXOrbwalker.now + Game.Ping / 2;
                }
                if (!CanMove() || !IsAllowedToMove())
                    return;
                MoveTo(goalPosition, -1, useDelay);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static int moveDelay = 600;

        private static void MoveTo(Vector3 position, float holdAreaRadius = -1, bool useDelay = true)
        {
            Orbwalker.MoveTo(position);
        }

        private static bool IsAllowedToMove()
        {
            if (!_movement)
                return false;

            return true;
        }

        private static bool IsAllowedToAttack()
        {
            if (!_attack || (MyHero.HealthPercent < 25 && ARAMSimulator.balance < -150))
                return false;

            return true;

        }

        private static void OnDraw(EventArgs args)
        {

        }

        private static void OnProcessSpell(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs spell)
        {
            if (IsAutoAttackReset(spell.SData.Name) && unit.IsMe)
                LeagueSharp.Common.Utility.DelayAction.Add(250, ResetAutoAttackTimer);

            if (!IsAutoAttack(spell.SData.Name))
                return;
            if (unit.IsMe)
            {
                _lastAATick = LXOrbwalker.now;
                // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
                if (spell.Target is Obj_AI_Base)
                {
                    FireOnTargetSwitch((Obj_AI_Base)spell.Target);
                    _lastTarget = (Obj_AI_Base)spell.Target;
                }
                if (unit.IsMelee())
                    LeagueSharp.Common.Utility.DelayAction.Add(
                        (int)(unit.AttackCastDelay * 1000 + Game.Ping * 0.5) + 50, () => FireAfterAttack(unit, _lastTarget));
            }
            FireOnAttack(unit, _lastTarget);
        }


        public static AttackableUnit GetPossibleTarget(bool onlyChamps = false)
        {
            if (ForcedTarget != null)
            {
                if (InAutoAttackRange(ForcedTarget))
                    return ForcedTarget;
                ForcedTarget = null;
            }

            if (onlyChamps)
            {
                return GetBestHeroTarget();
            }

            var camp = GetBestHeroTarget();
            if (camp != null)
                return camp;

            Obj_AI_Base tempTarget = null;
            /*turrets*/
            if (CurrentMode == Mode.LaneClear)
            {
                foreach (var turret in
                   EnemyTowers.Where(t => t.LSIsValidTarget() && InAutoAttackRange(t)))
                {
                    return turret;
                }
            }
            if ((CurrentMode == Mode.Harass || CurrentMode == Mode.LaneClear))
            {
                tempTarget = GetBestHeroTarget();
                if (tempTarget != null)
                    return tempTarget;
            }



            /*inhibitor*/
            if (CurrentMode == Mode.LaneClear)
            {
                foreach (var turret in
                    EnemyBarracs
                        .Where(t => t.LSIsValidTarget() && !t.IsInvulnerable && InAutoAttackRange(t)))
                {
                    return turret;
                }
            }

            /*nexus*/
            if (CurrentMode == Mode.LaneClear)
            {
                foreach (var nexus in
                    EnemyHQ
                        .Where(t => t.LSIsValidTarget() && InAutoAttackRange(t)))
                {
                    return nexus;
                }
            }
            if (ARAMSimulator.towerAttackedMe)
                return null;
            /* dont aa if enemy close */
            var closestenemy =
                HeroManager.Enemies.Where(ene => !ene.IsDead)
                    .OrderBy(ene => ene.LSDistance(MyHero, true))
                    .FirstOrDefault();
            var aaRangeext = GetAutoAttackRange(MyHero, closestenemy) + 120;
            if (closestenemy != null && closestenemy.LSDistance(MyHero, true) < aaRangeext * aaRangeext)
                return null;
            enemiesMinionsAround = ObjectManager.Get<Obj_AI_Base>()
                   .Where(targ => targ.LSIsValidTarget(farmRange) && !targ.IsDead && targ.IsTargetable && targ.IsEnemy).ToList();

            if (CurrentMode == Mode.Harass || CurrentMode == Mode.Lasthit || CurrentMode == Mode.LaneClear || CurrentMode == Mode.LaneFreeze)
            {
                foreach (
                    var minion in
                        from minion in
                            enemiesMinionsAround.Where(minion => minion.LSIsValidTarget() && InAutoAttackRange(minion))
                        let t = (int)(MyHero.AttackCastDelay * 1000) - 100 + Game.Ping / 2 +
                                1000 * (int)MyHero.LSDistance(minion) / (int)MyProjectileSpeed()
                        let predHealth = HealthPrediction.GetHealthPrediction(minion, t, FarmDelay())
                        where minion.Team != GameObjectTeam.Neutral && predHealth > 0 && minion.BaseSkinName != "GangplankBarrel" &&
                              predHealth <= MyHero.LSGetAutoAttackDamage(minion, true)
                        select minion)
                    return minion;
            }

            if (CurrentMode == Mode.Harass || CurrentMode == Mode.LaneClear || CurrentMode == Mode.LaneFreeze)
            {
                foreach (
                    var turret in
                        EnemyTowers.Where(turret => turret.LSIsValidTarget(GetAutoAttackRange(MyHero, turret))))
                    return turret;
            }

            if (CurrentMode != Mode.Lasthit)
            {
                tempTarget = GetBestHeroTarget();
                if (tempTarget != null)
                    return tempTarget;
            }

            float[] maxhealth;
            if (CurrentMode == Mode.LaneClear || CurrentMode == Mode.Harass || CurrentMode == Mode.LaneFreeze)
            {
                maxhealth = new float[] { 0 };
                var maxhealth1 = maxhealth;
                foreach (var minion in enemiesMinionsAround.Where(minion => minion.LSIsValidTarget(GetAutoAttackRange(MyHero, minion)) && minion.BaseSkinName != "GangplankBarrel" && minion.Team == GameObjectTeam.Neutral).Where(minion => minion.MaxHealth >= maxhealth1[0] || Math.Abs(maxhealth1[0] - float.MaxValue) < float.Epsilon))
                {
                    tempTarget = minion;
                    maxhealth[0] = minion.MaxHealth;
                }
                if (tempTarget != null)
                    return tempTarget;
            }

            if (CurrentMode != Mode.LaneClear || ShouldWait())
                return null;
            maxhealth = new float[] { 0 };
            foreach (var minion in from minion in enemiesMinionsAround
                .Where(minion => minion.LSIsValidTarget(GetAutoAttackRange(MyHero, minion)) && minion.BaseSkinName != "GangplankBarrel")
                                   let predHealth = HealthPrediction.LaneClearHealthPrediction(minion, (int)((MyHero.AttackDelay * 1000) * LaneClearWaitTimeMod), FarmDelay())
                                   where predHealth >=
                                         2 * MyHero.LSGetAutoAttackDamage(minion, true) ||
                                         Math.Abs(predHealth - minion.Health) < float.Epsilon
                                   where minion.Health >= maxhealth[0] || Math.Abs(maxhealth[0] - float.MaxValue) < float.Epsilon
                                   select minion)
            {
                tempTarget = minion;
                maxhealth[0] = minion.MaxHealth;
            }
            return tempTarget;
        }

        private static bool ShouldWait()
        {
            return
            enemiesMinionsAround
            .Any(
            minion =>
            minion.LSIsValidTarget() && minion.Team != GameObjectTeam.Neutral &&
            InAutoAttackRange(minion) &&
            HealthPrediction.LaneClearHealthPrediction(
            minion, (int)((MyHero.AttackDelay * 1000) * LaneClearWaitTimeMod), FarmDelay()) <= MyHero.LSGetAutoAttackDamage(minion));
        }

        public static bool IsAutoAttack(string name)
        {
            return (name.ToLower().Contains("attack") && !NoAttacks.Contains(name.ToLower())) ||
            Attacks.Contains(name.ToLower());
        }

        public static void ResetAutoAttackTimer()
        {
            _lastAATick = 0;
        }

        public static bool IsAutoAttackReset(string name)
        {
            return AttackResets.Contains(name.ToLower());
        }

        public static bool CanAttack()
        {
            if (_lastAATick <= LXOrbwalker.now && _delayAttackTill <= LXOrbwalker.now)
            {
                float danger = (inDanger && MyHero.FlatMagicDamageMod > MyHero.FlatPhysicalDamageMod * 1.4f) ? 500 : 0;
                return LXOrbwalker.now + Game.Ping / 2 + 25 >= _lastAATick + MyHero.AttackDelay * 1000 + danger && _attack;
            }
            return false;
        }

        public static bool CanMove()
        {
            var extraWindup = (CustomOrbwalkMode) ? 100 : 320;
            if (_lastAATick <= LXOrbwalker.now)
                return LXOrbwalker.now >= _lastAATick + MyHero.AttackCastDelay * 1000 + extraWindup && _movement || MyHero.ChampionName == "Kalista";
            return false;
        }

        private static float MyProjectileSpeed()
        {
            return (MyHero.CombatType == GameObjectCombatType.Melee) ? float.MaxValue : MyHero.BasicAttack.MissileSpeed;
        }

        private static int FarmDelay()
        {
            var ret = 100;
            if (MyHero.ChampionName == "Azir")
                ret += 125;
            return 50 + ret;
        }

        private static Obj_AI_Base GetBestHeroTarget()
        {
            AIHeroClient killableEnemy = null;
            var hitsToKill = double.MaxValue;
            foreach (var enemy in AllEnemys.Where(hero => !hero.IsDead && hero.LSIsValidTarget() && !hero.IsZombie && InAutoAttackRange(hero)))
            {
                var killHits = CountKillhits(enemy);
                if (killableEnemy != null && !(killHits < hitsToKill))
                    continue;
                killableEnemy = enemy;
                hitsToKill = killHits;
            }
            return hitsToKill < 4 ? killableEnemy : ARAMTargetSelector.getBestTarget(GetAutoAttackRange() + 100);
        }

        private static double CountKillhits(AIHeroClient enemy)
        {
            return enemy.Health / MyHero.LSGetAutoAttackDamage(enemy);
        }


        private static void CheckAutoWindUp()
        {

        }

        public static int GetCurrentWindupTime()
        {
            return 100;
        }

        public static float GetAutoAttackRange(Obj_AI_Base source = null, AttackableUnit target = null)
        {
            try
            {

                if (source == null)
                    source = MyHero;
                var ret = source.AttackRange + MyHero.BoundingRadius;
                if (target != null)
                    ret += target.BoundingRadius;
                return ret;
            }
            catch (Exception)
            {
                return MyHero.AttackRange + 50;
            }
        }

        public static bool InAutoAttackRange(AttackableUnit target)
        {
            if (target == null)
                return false;
            var myRange = GetAutoAttackRange(MyHero, target);
            return Vector2.DistanceSquared(target.Position.LSTo2D(), MyHero.ServerPosition.LSTo2D()) <= myRange * myRange;
        }

        public static Mode CurrentMode
        {
            get
            {
                return Mode.LaneClear;
            }
        }

        public static void SetAttack(bool value)
        {
            _attack = value;
        }

        public static void SetMovement(bool value)
        {
            _movement = value;
        }

        public static bool GetAttack()
        {
            return _attack;
        }

        public static bool GetMovement()
        {
            return _movement;
        }

        public class BeforeAttackEventArgs
        {
            public Obj_AI_Base Target;
            public Obj_AI_Base Unit = ObjectManager.Player;
            private bool _process = true;
            public bool Process
            {
                get
                {
                    return _process;
                }
                set
                {
                    _disableNextAttack = !value;
                    _process = value;
                }
            }
        }
        private static void FireBeforeAttack(Obj_AI_Base target)
        {
            if (BeforeAttack != null)
            {
                BeforeAttack(new BeforeAttackEventArgs
                {
                    Target = target
                });
            }
            else
            {
                _disableNextAttack = false;
            }
        }

        private static void FireOnTargetSwitch(Obj_AI_Base newTarget)
        {
            if (OnTargetChange != null && (_lastTarget == null || _lastTarget.NetworkId != newTarget.NetworkId))
            {
                OnTargetChange(_lastTarget, newTarget);
            }
        }

        private static void FireAfterAttack(Obj_AI_Base unit, Obj_AI_Base target)
        {
            _lastMovement = 0;
            if (AfterAttack != null)
            {
                AfterAttack(unit, target);
            }
        }

        private static void FireOnAttack(Obj_AI_Base unit, Obj_AI_Base target)
        {
            if (OnAttack != null)
            {
                OnAttack(unit, target);
            }
        }
    }
}