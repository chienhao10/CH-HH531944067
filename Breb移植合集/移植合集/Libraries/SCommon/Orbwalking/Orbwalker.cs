using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SCommon;
using SCommon.Database;
using SCommon.Packet;
//typedefs
using TargetSelector = SCommon.TS.TargetSelector;
using EloBuddy;
using EloBuddy.SDK.Menu;

namespace SCommon.Orbwalking
{
    public class Orbwalker
    {
        /// <summary>
        /// The Orbwalker mode enum.
        /// </summary>
        public enum Mode
        {
            None,
            Combo,
            Mixed,
            LaneClear,
            LastHit,
        }

        private Random m_rnd;
        private int m_lastAATick;
        private int m_lastWindUpEndTick;
        private int m_lastWindUpTime;
        private int m_lastAttackCooldown;
        private int m_lastAttackCompletesAt;
        private int m_lastMoveTick;
        private int m_lastAttackTick;
        private float m_baseAttackSpeed;
        private float m_baseWindUp;
        private bool m_attackInProgress;
        private bool m_rengarAttack;
        private bool m_Attack;
        private bool m_Move;
        private bool m_IslastCastedAA;
        private Vector2 m_lastAttackPos;
        private Vector3 m_orbwalkingPoint;
        private ConfigMenu m_Configuration;
        private bool m_orbwalkEnabled;
        private AttackableUnit m_forcedTarget;
        private bool m_attackReset;
        private AttackableUnit m_lastTarget;
        private Obj_AI_Base m_towerTarget;
        private Obj_AI_Base m_sourceTower;
        private int m_towerAttackTick;
        private bool m_channelingWait;
        private Func<bool> m_fnCanAttack;
        private Func<bool> m_fnCanMove;
        private Func<AttackableUnit, bool> m_fnCanOrbwalkTarget;
        private Func<bool> m_fnShouldWait;

        /// <summary>
        ///     Spells that reset the attack timer.
        /// </summary>
        private static readonly string[] AttackResets =
        {
            "dariusnoxiantacticsonh", "fioraflurry", "garenq",
            "gravesmove", "hecarimrapidslash", "jaxempowertwo", "jaycehypercharge", "leonashieldofdaybreak", "luciane",
            "monkeykingdoubleattack", "mordekaisermaceofspades", "nasusq", "nautiluspiercinggaze", "netherblade",
            "gangplankqwrapper", "powerfist", "renektonpreexecute", "rengarq",
            "shyvanadoubleattack", "sivirw", "takedown", "talonnoxiandiplomacy", "trundletrollsmash", "vaynetumble",
            "vie", "volibearq", "xenzhaocombotarget", "yorickspectral", "reksaiq", "itemtitanichydracleave", "masochism",
            "illaoiw", "elisespiderw", "fiorae", "meditate", "sejuaninorthernwinds", "asheq"
        };

        /// <summary>
        /// The orbwalker constructor
        /// </summary>
        /// <param name="menuToAttach">The menu to attach.</param>
        public Orbwalker()
        {
            m_rnd = new Random();
            m_lastAATick = 0;
            m_lastWindUpEndTick = 0;
            m_lastMoveTick = 0;
            m_Attack = true;
            m_Move = true;
            m_baseWindUp = 1f / (ObjectManager.Player.AttackCastDelay * ObjectManager.Player.GetAttackSpeed());
            m_baseAttackSpeed = 1f / (ObjectManager.Player.AttackDelay * ObjectManager.Player.GetAttackSpeed());
            m_orbwalkingPoint = Vector3.Zero;
            m_Configuration = new ConfigMenu();
            m_orbwalkEnabled = true;
            m_forcedTarget = null;
            m_lastTarget = null;
            m_fnCanAttack = null;
            m_fnCanMove = null;
            m_fnCanOrbwalkTarget = null;
            m_fnShouldWait = null;

            Game.OnUpdate += Game_OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Obj_AI_Base.OnSpellCast += Obj_AI_Base_OnDoCast;
            Obj_AI_Base.OnBuffGain += Obj_AI_Base_OnBuffAdd;
            Obj_AI_Base.OnBuffLose += Obj_AI_Base_OnBuffRemove;
            Obj_AI_Base.OnNewPath += Obj_AI_Base_OnNewPath;
            Obj_AI_Base.OnPlayAnimation += Obj_AI_Base_OnPlayAnimation;
            Spellbook.OnStopCast += Spellbook_OnStopCast;
            Obj_AI_Base.OnDamage += Obj_AI_Base_OnDamage;
            PacketHandler.Register(0x31, PacketHandler_AfterAttack);
            PacketHandler.Register(0x155, PacketHandler_CancelWindup);
            new Drawings(this);
        }

        /// <summary>
        /// Gets Orbwalker's active mode
        /// </summary>
        public Mode ActiveMode
        {
            get
            {
                if (m_Configuration.Combo)
                    return Mode.Combo;

                if (m_Configuration.Harass)
                    return Mode.Mixed;

                if (m_Configuration.LaneClear)
                    return Mode.LaneClear;

                if (m_Configuration.LastHit)
                    return Mode.LastHit;

                return Mode.None;
            }
        }

        /// <summary>
        /// Gets Last Auto Attack Tick
        /// </summary>
        public int LastAATick
        {
            get { return m_lastAATick; }
        }

        /// <summary>
        /// Gets Last WindUp tick
        /// </summary>
        public int LastWindUpEndTick
        {
            get { return m_lastWindUpEndTick; }
        }

        /// <summary>
        /// Gets Last Movement tick
        /// </summary>
        public int LastMoveTick
        {
            get { return m_lastMoveTick; }
        }

        /// <summary>
        /// Gets Configuration menu;
        /// </summary>
        public ConfigMenu Configuration
        {
            get { return m_Configuration; }
        }

        /// <summary>
        /// Gets or sets orbwalking point
        /// </summary>
        public Vector3 OrbwalkingPoint
        {
            get { return m_orbwalkingPoint == Vector3.Zero ? Game.CursorPos : m_orbwalkingPoint; }
            set { m_orbwalkingPoint = value; }
        }

        /// <summary>
        /// Gets or sets orbwalking is enabled
        /// </summary>
        public bool Enabled
        {
            get { return m_orbwalkEnabled; }
            set { m_orbwalkEnabled = value; }
        }

        /// <summary>
        /// Gets or sets forced orbwalk target
        /// </summary>
        public AttackableUnit ForcedTarget
        {
            get { return m_forcedTarget; }
            set { m_forcedTarget = value; }
        }

        /// <summary>
        /// Gets base attack speed value
        /// </summary>
        public float BaseAttackSpeed
        {
            get { return m_baseAttackSpeed; }
        }

        /// <summary>
        /// Gets base windup value
        /// </summary>
        public float BaseWindup
        {
            get { return m_baseWindUp; }
        }

        /// <summary>
        /// Resets auto attack timer
        /// </summary>
        public void ResetAATimer()
        {
            if (m_baseAttackSpeed != 0.5f)
            {
                m_lastAATick = Utils.TickCount - Game.Ping / 2 - m_lastAttackCooldown;
                m_lastAttackTick = 0;
                m_attackReset = true;
                m_attackInProgress = false;
            }
        }

        /// <summary>
        /// Resets orbwalk values
        /// </summary>
        public void ResetOrbwalkValues()
        {
            m_baseAttackSpeed = 0.5f;
        }

        /// <summary>
        /// Sets orbwalk values
        /// </summary>
        public void SetOrbwalkValues()
        {
            m_baseWindUp = 1f / (ObjectManager.Player.AttackCastDelay * ObjectManager.Player.GetAttackSpeed());
            m_baseAttackSpeed = 1f / (ObjectManager.Player.AttackDelay * ObjectManager.Player.GetAttackSpeed());
        }

        /// <summary>
        /// Sets attack value
        /// </summary>
        public void SetAttack(bool set)
        {
            m_Attack = set;
        }

        /// <summary>
        /// Sets move value
        /// </summary>
        public void SetMove(bool set)
        {
            m_Move = set;
        }

        /// <summary>
        /// Sets can orbwalk while channeling spell
        /// </summary>
        /// <param name="set">The orbwalker will orbwalk if the set is <c>false</c></param>
        public void SetChannelingWait(bool set)
        {
            m_channelingWait = set;
        }

        /// <summary>
        /// Checks if player can attack
        /// </summary>
        /// <returns>true if can attack</returns>
        public bool CanAttack(int t = 0)
        {
            if (!m_Attack)
                return false;

            if (m_attackReset)
                return true;

            if (m_fnCanAttack != null)
                return m_fnCanAttack();

            if (ObjectManager.Player.CharData.BaseSkinName == "Graves" && !ObjectManager.Player.HasBuff("GravesBasicAttackAmmo1") && !ObjectManager.Player.HasBuff("GravesBasicAttackAmmo2"))
                return false;

            if (ObjectManager.Player.CharData.BaseSkinName == "Jhin" && ObjectManager.Player.HasBuff("JhinPassiveReload"))
                return false;

            return Utils.TickCount + t + Game.Ping - m_lastAATick - m_Configuration.ExtraWindup - (m_Configuration.LegitMode && !ObjectManager.Player.IsMelee ? Math.Max(100, ObjectManager.Player.AttackDelay * 1000) : 0) * m_Configuration.LegitPercent / 100f >= 1000 / (ObjectManager.Player.GetAttackSpeed() * m_baseAttackSpeed);
        }

        /// <summary>
        /// Checks if player can move
        /// </summary>
        /// <returns>true if can move</returns>
        public bool CanMove(int t = 0)
        {
            if (!m_Move)
                return false;

            if (Utils.TickCount - m_lastWindUpEndTick < (ObjectManager.Player.AttackDelay - ObjectManager.Player.AttackCastDelay) * 1000f + (Game.Ping <= 30 ? 30 : 0))
                return true;

            if (m_fnCanMove != null)
                return m_fnCanMove();

            if (Utility.IsNonCancelChamp(ObjectManager.Player.CharData.BaseSkinName))
                return Utils.TickCount - m_lastMoveTick >= 70 + m_rnd.Next(0, Game.Ping);

            return Utils.TickCount + t - 20 - m_lastAATick - m_Configuration.ExtraWindup - m_Configuration.MovementDelay >= 1000 / (ObjectManager.Player.GetAttackSpeed() * m_baseWindUp);
        }

        /// <summary>
        /// Checks if player can orbwalk given target
        /// </summary>
        /// <param name="target">Target</param>
        /// <returns>true if can orbwalk target</returns>
        public bool CanOrbwalkTarget(AttackableUnit target)
        {
            if (target == null)
                return false;

            if (m_fnCanOrbwalkTarget != null)
                return m_fnCanOrbwalkTarget(target);

            if (target.LSIsValidTarget())
            {
                if (target.Type == GameObjectType.AIHeroClient)
                {
                    AIHeroClient hero = target as AIHeroClient;
                    return ObjectManager.Player.LSDistance(hero.ServerPosition) - hero.BoundingRadius - hero.GetScalingRange() + 20 < Utility.GetAARange();
                }
                else
                    return (target.Type != GameObjectType.obj_AI_Turret || m_Configuration.AttackStructures) && ObjectManager.Player.LSDistance(target.Position) - target.BoundingRadius + 20 < Utility.GetAARange();
            }
            return false;
        }

        /// <summary>
        /// Checks if player can orbwalk given target in custom range
        /// </summary>
        /// <param name="target">Target</param>
        /// <param name="range">Custom range</param>
        /// <returns>true if can orbwalk target</returns>
        public bool CanOrbwalkTarget(AttackableUnit target, float range)
        {
            if (target.LSIsValidTarget())
            {
                if (target.Type == GameObjectType.AIHeroClient)
                {
                    AIHeroClient hero = target as AIHeroClient;
                    return ObjectManager.Player.LSDistance(hero.ServerPosition) - hero.BoundingRadius - hero.GetScalingRange() + 10 < range + ObjectManager.Player.BoundingRadius + ObjectManager.Player.GetScalingRange();
                }
                else
                    return ObjectManager.Player.LSDistance(target.Position) - target.BoundingRadius + 20 < range + ObjectManager.Player.BoundingRadius + ObjectManager.Player.GetScalingRange();
            }
            return false;
        }

        /// <summary>
        /// Checks if player can orbwalk given target from custom position
        /// </summary>
        /// <param name="target">Target</param>
        /// <param name="position">Custom position</param>
        /// <returns>true if can orbwalk target</returns>
        public bool CanOrbwalkTarget(AttackableUnit target, Vector3 position)
        {
            if (target.LSIsValidTarget())
            {
                if (target.Type == GameObjectType.AIHeroClient)
                {
                    AIHeroClient hero = target as AIHeroClient;
                    return position.LSDistance(hero.ServerPosition) - hero.BoundingRadius - hero.GetScalingRange() < Utility.GetAARange();
                }
                else
                    return position.LSDistance(target.Position) - target.BoundingRadius < Utility.GetAARange();
            }
            return false;
        }

        /// <summary>
        /// Orbwalk itself
        /// </summary>
        /// <param name="target">Target</param>
        public void Orbwalk(AttackableUnit target)
        {
            Orbwalk(target, OrbwalkingPoint);
        }

        /// <summary>
        /// Orbwalk itself
        /// </summary>
        /// <param name="target">Target</param>
        /// <param name="point">Orbwalk point</param>
        public void Orbwalk(AttackableUnit target, Vector3 point)
        {
            if (!m_attackInProgress)
            {
                if (CanOrbwalkTarget(target))
                {
                    if (CanAttack())
                    {
                        BeforeAttackArgs args = Events.FireBeforeAttack(this, target);
                        if (args.Process)
                        {
                            if (!m_Configuration.DisableAA || target.Type != GameObjectType.AIHeroClient)
                                Attack(target);
                        }
                        else
                        {
                            if (CanMove())
                            {
                                if (m_Configuration.DontMoveInRange && target.Type == GameObjectType.AIHeroClient)
                                    return;

                                if ((m_Configuration.LegitMode && !ObjectManager.Player.IsMelee) || !m_Configuration.LegitMode)
                                    Move(point);
                            }
                        }
                    }
                    else if (CanMove())
                    {
                        if (m_Configuration.DontMoveInRange && target.Type == GameObjectType.AIHeroClient)
                            return;

                        if ((m_Configuration.LegitMode && !ObjectManager.Player.IsMelee) || !m_Configuration.LegitMode)
                            Move(point);
                    }
                }
                else
                    Move(point);
            }
            Move(point);
        }

        /// <summary>
        /// Gets AA Animation Time
        /// </summary>
        /// <returns></returns>
        private float GetAnimationTime()
        {
            return 1f / (ObjectManager.Player.GetAttackSpeed() * m_baseAttackSpeed);
        }

        /// <summary>
        /// Gets AA Windup Time
        /// </summary>
        /// <returns></returns>
        private float GetWindupTime()
        {
            return 1f / (ObjectManager.Player.GetAttackSpeed() * m_baseWindUp) + m_Configuration.ExtraWindup;
        }

        /// <summary>
        /// Orders move hero to given position
        /// </summary>
        /// <param name="pos"></param>
        public void Move(Vector3 pos)
        {
            if (!m_attackInProgress && CanMove() && (!CanAttack(60) || CanAttack()))
            {
                Vector3 playerPos = ObjectManager.Player.ServerPosition;

                bool holdzone = m_Configuration.DontMoveMouseOver || m_Configuration.HoldAreaRadius != 0;
                var holdzoneRadiusSqr = Math.Max(m_Configuration.HoldAreaRadius * m_Configuration.HoldAreaRadius, ObjectManager.Player.BoundingRadius * ObjectManager.Player.BoundingRadius * 4);
                if (holdzone && playerPos.LSDistance(pos, true) < holdzoneRadiusSqr)
                {
                    if ((Utils.TickCount + Game.Ping / 2 - m_lastAATick) * 0.6f >= 1000f / (ObjectManager.Player.GetAttackSpeed() * m_baseWindUp))
                        EloBuddy.Player.IssueOrder(GameObjectOrder.Stop, playerPos);
                    m_lastMoveTick = Utils.TickCount + m_rnd.Next(1, 20);
                    return;
                }

                if (ObjectManager.Player.LSDistance(pos, true) < 22500)
                    pos = playerPos.LSExtend(pos, (m_rnd.NextFloat(0.6f, 1.01f) + 0.2f) * 400);


                if (m_lastMoveTick + 50 + Math.Min(60, Game.Ping) < Utils.TickCount)
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, pos);
                    m_lastMoveTick = Utils.TickCount + m_rnd.Next(1, 20);
                }
            }
        }

        /// <summary>
        /// Orders attack hero to given target
        /// </summary>
        /// <param name="target"></param>
        private void Attack(AttackableUnit target)
        {
            if (m_lastAttackTick < Utils.TickCount && !m_attackInProgress)
            {
                m_lastWindUpEndTick = 0;
                m_lastAttackTick = Utils.TickCount + (int)Math.Floor(ObjectManager.Player.AttackDelay * 1000);
                m_lastAATick = Utils.TickCount + Game.Ping;
                m_attackInProgress = true;
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
            }
        }

        /// <summary>
        /// Magnets the hero to given target
        /// </summary>
        /// <param name="target">The target.</param>
        private void Magnet(AttackableUnit target)
        {
            if (!m_attackInProgress && !CanOrbwalkTarget(target))
            {
                if (ObjectManager.Player.AttackRange <= m_Configuration.StickRange)
                {
                    if (target.LSIsValidTarget(m_Configuration.StickRange))
                    {
                        /*expermential*/
                        OrbwalkingPoint = target.Position.LSExtend(ObjectManager.Player.ServerPosition, -(m_rnd.NextFloat(0.6f, 1.01f) + 0.2f) * 400);
                        /*expermential*/
                    }
                    else
                        OrbwalkingPoint = Vector3.Zero;
                }
                else
                    OrbwalkingPoint = Vector3.Zero;
            }
            else
                OrbwalkingPoint = Vector3.Zero;
        }

        /// <summary>
        /// The event which called after an attack
        /// </summary>
        /// <param name="target">The target.</param>
        private void AfterAttack(AttackableUnit target)
        {
            m_lastWindUpEndTick = Utils.TickCount;
            m_attackReset = false;
            m_attackInProgress = false;
            m_lastMoveTick = 0;
            Events.FireAfterAttack(this, target);
        }

        private Obj_AI_Minion _prevMinion;

        /// <summary>
        /// Gets laneclear target
        /// </summary>
        /// <returns></returns>
        private AttackableUnit GetLaneClearTarget()
        {
            AttackableUnit result = null;

            if (_prevMinion.LSIsValidTarget() && InAutoAttackRange(_prevMinion))
            {
                var predHealth = HealthPrediction.LaneClearHealthPrediction(
                    _prevMinion, (int)(ObjectManager.Player.AttackDelay * 1000 * 2f), ConfigMenu.getSliderItem("Orbwalking.Root.iExtraWindup"));
                if (predHealth >= 2 * ObjectManager.Player.LSGetAutoAttackDamage(_prevMinion) ||
                    Math.Abs(predHealth - _prevMinion.Health) < float.Epsilon)
                {
                    return _prevMinion;
                }
            }

            result = (from minion in
                ObjectManager.Get<Obj_AI_Minion>()
                    .Where(
                        minion =>
                            minion.LSIsValidTarget() && InAutoAttackRange(minion))
                      let predHealth =
                          HealthPrediction.LaneClearHealthPrediction(
                              minion, (int)(ObjectManager.Player.AttackDelay * 1000 * 2f), ConfigMenu.getSliderItem("Orbwalking.Root.iExtraWindup"))
                      where
                          predHealth >= 2 * ObjectManager.Player.LSGetAutoAttackDamage(minion) ||
                          Math.Abs(predHealth - minion.Health) < float.Epsilon
                      select minion).MaxOrDefault(
                    m => !MinionManager.IsMinion(m, true) ? float.MaxValue : m.Health);

            if (result != null)
            {
                _prevMinion = (Obj_AI_Minion)result;
            }
            return result;
        }

        /// <summary>
        /// Gets jungleclear target
        /// </summary>
        /// <returns></returns>
        private Obj_AI_Base GetJungleClearTarget()
        {
            Obj_AI_Base mob = null;
            if (Game.MapId == GameMapId.SummonersRift || Game.MapId == GameMapId.TwistedTreeline)
            {
                int mobPrio = 0;
                foreach (var minion in MinionManager.GetMinions(2000, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth))
                {
                    if (CanOrbwalkTarget(minion))
                    {
                        int prio = minion.GetJunglePriority();
                        if (minion.Health < ObjectManager.Player.LSGetAutoAttackDamage(minion))
                            return minion;
                        else
                        {
                            if (mob == null)
                            {
                                mob = minion;
                                mobPrio = prio;
                            }
                            else if (prio < mobPrio)
                            {
                                mob = minion;
                                mobPrio = prio;
                            }
                        }
                    }
                }
            }
            return mob;
        }

        /// <summary>
        /// Finds the last hit minion
        /// </summary>
        /// <returns></returns>
        private Obj_AI_Base FindKillableMinion()
        {
            if (m_Configuration.SupportMode)
                return null;

            foreach (var minion in MinionManager.GetMinions(ObjectManager.Player.AttackRange + 100f).OrderBy(p => ObjectManager.Player.LSGetAutoAttackDamage(p)))
            {
                if (CanOrbwalkTarget(minion) && Damage.Prediction.IsLastHitable(minion))
                    return minion;
            }
            return null;
        }

        /// <summary>
        /// Checks if the orbwalker should wait to lasthit
        /// </summary>
        /// <returns><c>true</c> if should wait</returns>
        public bool ShouldWait()
        {
            return
                ObjectManager.Get<Obj_AI_Minion>()
                    .Any(
                        minion =>
                            minion.LSIsValidTarget() && minion.Team != GameObjectTeam.Neutral &&
                            InAutoAttackRange(minion) && MinionManager.IsMinion(minion, false) &&
                            HealthPrediction.LaneClearHealthPrediction(
                                minion, (int)(ObjectManager.Player.AttackDelay * 1000 * 2f), ConfigMenu.getSliderItem("Orbwalking.Root.iExtraWindup")) <=
                            ObjectManager.Player.LSGetAutoAttackDamage(minion));
        }


        /// <summary>
        ///     Returns true if the target is in auto-attack range.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool InAutoAttackRange(AttackableUnit target)
        {
            if (!target.LSIsValidTarget())
            {
                return false;
            }
            var myRange = GetRealAutoAttackRange(target);
            return
                Vector2.DistanceSquared(
                    target is Obj_AI_Base ? ((Obj_AI_Base)target).ServerPosition.LSTo2D() : target.Position.LSTo2D(),
                    ObjectManager.Player.ServerPosition.LSTo2D()) <= myRange * myRange;
        }

        /// <summary>
        ///     Returns the auto-attack range of local player with respect to the target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>System.Single.</returns>
        public static float GetRealAutoAttackRange(AttackableUnit target)
        {
            var result = ObjectManager.Player.AttackRange + ObjectManager.Player.BoundingRadius;
            if (target.LSIsValidTarget())
            {
                var aiBase = target as Obj_AI_Base;
                if (aiBase != null && ObjectManager.Player.ChampionName == "Caitlyn")
                {
                    if (aiBase.HasBuff("caitlynyordletrapinternal"))
                    {
                        result += 650;
                    }
                }

                return result + target.BoundingRadius;
            }

            return result;
        }

        /// <summary>
        ///     Returns true if the unit is melee
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <returns><c>true</c> if the specified unit is melee; otherwise, <c>false</c>.</returns>
        public static bool IsMelee(Obj_AI_Base unit)
        {
            return unit.CombatType == GameObjectCombatType.Melee;
        }


        /// <summary>
        ///     Returns player auto-attack missile speed.
        /// </summary>
        /// <returns>System.Single.</returns>
        public static float GetMyProjectileSpeed()
        {
            return IsMelee(ObjectManager.Player) || ObjectManager.Player.ChampionName == "Azir" || ObjectManager.Player.ChampionName == "Velkoz" ||
                   ObjectManager.Player.ChampionName == "Viktor" && Player.HasBuff("ViktorPowerTransferReturn")
                ? float.MaxValue
                : ObjectManager.Player.BasicAttack.MissileSpeed;
        }

        /// <summary>
        /// Gets orbwalker target
        /// </summary>
        /// <returns></returns>
        public AttackableUnit GetTarget()
        {
            #region farm

            if (!EloBuddy.SDK.Orbwalker.ShouldWait)
            {
                if (ActiveMode == Mode.LaneClear)
                {
                    var minion = EloBuddy.SDK.Orbwalker.LaneClearMinion;
                    if (minion != null)
                        return minion;
                }
            }

            if (!EloBuddy.SDK.Orbwalker.ShouldWait)
            {
                if (ActiveMode == Mode.LaneClear)
                {
                    var jminions = ObjectManager.Get<Obj_AI_Minion>().Where(mobA => mobA.LSIsValidTarget() && mobA.Team == GameObjectTeam.Neutral && InAutoAttackRange(mobA) && mobA.CharData.BaseSkinName != "gangplankbarrel" && mobA.Name != "WardCorpse" && mobA.IsMonster);
                    var mob = jminions.MaxOrDefault(mobB => mobB.MaxHealth);
                    if (mob != null)
                        return mob;
                }
            }

            if (ActiveMode == Mode.LaneClear || ActiveMode == Mode.Mixed || ActiveMode == Mode.LastHit)
            {
                var MinionList = ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.LSIsValidTarget() && InAutoAttackRange(minion)).OrderByDescending(minion => minion.CharData.BaseSkinName.Contains("Siege")).ThenBy(minion => minion.CharData.BaseSkinName.Contains("Super")).ThenBy(minion => minion.Health).ThenByDescending(minion => minion.MaxHealth);
                foreach (var minion in MinionList)
                {
                    var t = (int)(ObjectManager.Player.AttackCastDelay * 1000) - 100 + Game.Ping / 2 + 1000 * (int)Math.Max(0, ObjectManager.Player.LSDistance(minion) - ObjectManager.Player.BoundingRadius) / (int)GetMyProjectileSpeed();
                    var predHealth = HealthPrediction.GetHealthPrediction(minion, t, 40);
                    if (minion.Team != GameObjectTeam.Neutral)
                    {
                        var damage = ObjectManager.Player.LSGetAutoAttackDamage(minion, true);
                        var killable = predHealth <= damage;
                        if (predHealth <= 0)
                        {
                            return FindKillableMinion();
                        }
                        if (killable)
                        {
                            return minion;
                        }
                    }
                }
            }

            if (ActiveMode == Mode.LaneClear || ActiveMode == Mode.LastHit || ActiveMode == Mode.Mixed)
            {
                #region turret farming
                var closestTower =
                    ObjectManager.Get<Obj_AI_Turret>()
                        .MinOrDefault(t => t.IsAlly && !t.IsDead ? ObjectManager.Player.LSDistance(t, true) : float.MaxValue);

                if (closestTower != null && ObjectManager.Player.LSDistance(closestTower, true) < 1500 * 1500)
                {
                    Obj_AI_Minion farmUnderTurretMinion = null;
                    Obj_AI_Minion noneKillableMinion = null;
                    // return all the minions underturret in auto attack range
                    var minions =
                        MinionManager.GetMinions(ObjectManager.Player.Position, ObjectManager.Player.AttackRange + 200)
                            .Where(
                                minion =>
                                    InAutoAttackRange(minion) && closestTower.LSDistance(minion, true) < 900 * 900)
                            .OrderByDescending(minion => minion.CharData.BaseSkinName.Contains("Siege"))
                            .ThenBy(minion => minion.CharData.BaseSkinName.Contains("Super"))
                            .ThenByDescending(minion => minion.MaxHealth)
                            .ThenByDescending(minion => minion.Health);
                    if (minions.Any())
                    {
                        // get the turret aggro minion
                        var turretMinion =
                            minions.FirstOrDefault(
                                minion =>
                                    minion is Obj_AI_Minion &&
                                    HealthPrediction.HasTurretAggro(minion as Obj_AI_Minion));

                        if (turretMinion != null)
                        {
                            var hpLeftBeforeDie = 0;
                            var hpLeft = 0;
                            var turretAttackCount = 0;
                            var turretStarTick = HealthPrediction.TurretAggroStartTick(
                                turretMinion as Obj_AI_Minion);
                            // from healthprediction (don't blame me :S)
                            var turretLandTick = turretStarTick + (int)(closestTower.AttackCastDelay * 1000) +
                                                 1000 *
                                                 Math.Max(
                                                     0,
                                                     (int)
                                                         (turretMinion.LSDistance(closestTower) -
                                                          closestTower.BoundingRadius)) /
                                                 (int)(closestTower.BasicAttack.MissileSpeed + 70);
                            // calculate the HP before try to balance it
                            for (float i = turretLandTick + 50;
                                i < turretLandTick + 10 * closestTower.AttackDelay * 1000 + 50;
                                i = i + closestTower.AttackDelay * 1000)
                            {
                                var time = (int)i - Utils.GameTimeTickCount + Game.Ping / 2;
                                var predHP =
                                    (int)
                                        HealthPrediction.LaneClearHealthPrediction(
                                            turretMinion, time > 0 ? time : 0);
                                if (predHP > 0)
                                {
                                    hpLeft = predHP;
                                    turretAttackCount += 1;
                                    continue;
                                }
                                hpLeftBeforeDie = hpLeft;
                                hpLeft = 0;
                                break;
                            }
                            // calculate the hits is needed and possibilty to balance
                            if (hpLeft == 0 && turretAttackCount != 0 && hpLeftBeforeDie != 0)
                            {
                                var damage = (int)ObjectManager.Player.LSGetAutoAttackDamage(turretMinion, true);
                                var hits = hpLeftBeforeDie / damage;
                                var timeBeforeDie = turretLandTick +
                                                    (turretAttackCount + 1) *
                                                    (int)(closestTower.AttackDelay * 1000) -
                                                    Utils.GameTimeTickCount;
                                var timeUntilAttackReady = LastAATick + (int)(ObjectManager.Player.AttackDelay * 1000) >
                                                           Utils.GameTimeTickCount + Game.Ping / 2 + 25
                                    ? LastAATick + (int)(ObjectManager.Player.AttackDelay * 1000) -
                                      (Utils.GameTimeTickCount + Game.Ping / 2 + 25)
                                    : 0;
                                var timeToLandAttack = ObjectManager.Player.IsMelee
                                    ? ObjectManager.Player.AttackCastDelay * 1000
                                    : ObjectManager.Player.AttackCastDelay * 1000 +
                                      1000 * Math.Max(0, turretMinion.LSDistance(ObjectManager.Player) - ObjectManager.Player.BoundingRadius) /
                                      ObjectManager.Player.BasicAttack.MissileSpeed;
                                if (hits >= 1 &&
                                    hits * ObjectManager.Player.AttackDelay * 1000 + timeUntilAttackReady + timeToLandAttack <
                                    timeBeforeDie)
                                {
                                    farmUnderTurretMinion = turretMinion as Obj_AI_Minion;
                                }
                                else if (hits >= 1 &&
                                         hits * ObjectManager.Player.AttackDelay * 1000 + timeUntilAttackReady + timeToLandAttack >
                                         timeBeforeDie)
                                {
                                    noneKillableMinion = turretMinion as Obj_AI_Minion;
                                }
                            }
                            else if (hpLeft == 0 && turretAttackCount == 0 && hpLeftBeforeDie == 0)
                            {
                                noneKillableMinion = turretMinion as Obj_AI_Minion;
                            }
                            // should wait before attacking a minion.
                            if (ShouldWaitUnderTurret(noneKillableMinion))
                            {
                                return null;
                            }
                            if (farmUnderTurretMinion != null)
                            {
                                return farmUnderTurretMinion;
                            }
                            // balance other minions
                            foreach (var minion in
                                minions.Where(
                                    x =>
                                        x.NetworkId != turretMinion.NetworkId && x is Obj_AI_Minion &&
                                        !HealthPrediction.HasMinionAggro(x as Obj_AI_Minion)))
                            {
                                var playerDamage = (int)ObjectManager.Player.LSGetAutoAttackDamage(minion);
                                var turretDamage = (int)closestTower.LSGetAutoAttackDamage(minion, true);
                                var leftHP = (int)minion.Health % turretDamage;
                                if (leftHP > playerDamage)
                                {
                                    return minion;
                                }
                            }
                            // late game
                            var lastminion =
                                minions.LastOrDefault(x => x.NetworkId != turretMinion.NetworkId && x is Obj_AI_Minion &&
                                        !HealthPrediction.HasMinionAggro(x as Obj_AI_Minion));
                            if (lastminion != null && minions.Count() >= 2)
                            {
                                if (1f / ObjectManager.Player.AttackDelay >= 1f &&
                                    (int)(turretAttackCount * closestTower.AttackDelay / ObjectManager.Player.AttackDelay) *
                                    ObjectManager.Player.LSGetAutoAttackDamage(lastminion) > lastminion.Health)
                                {
                                    return lastminion;
                                }
                                if (minions.Count() >= 5 && 1f / ObjectManager.Player.AttackDelay >= 1.2)
                                {
                                    return lastminion;
                                }
                            }
                        }
                        else
                        {
                            if (ShouldWaitUnderTurret(noneKillableMinion))
                            {
                                return null;
                            }
                            // balance other minions
                            foreach (var minion in
                                minions.Where(
                                    x => x is Obj_AI_Minion && !HealthPrediction.HasMinionAggro(x as Obj_AI_Minion))
                                )
                            {
                                if (closestTower != null)
                                {
                                    var playerDamage = (int)ObjectManager.Player.LSGetAutoAttackDamage(minion);
                                    var turretDamage = (int)closestTower.LSGetAutoAttackDamage(minion, true);
                                    var leftHP = (int)minion.Health % turretDamage;
                                    if (leftHP > playerDamage)
                                    {
                                        return minion;
                                    }
                                }
                            }
                            //late game
                            var lastminion =
                                minions
                                    .LastOrDefault(x => x is Obj_AI_Minion && !HealthPrediction.HasMinionAggro(x as Obj_AI_Minion));
                            if (lastminion != null && minions.Count() >= 2)
                            {
                                if (minions.Count() >= 5 && 1f / ObjectManager.Player.AttackDelay >= 1.2)
                                {
                                    return lastminion;
                                }
                            }
                        }
                        return null;
                    }
                }
                #endregion
            }
            #endregion

            if (m_forcedTarget != null && m_forcedTarget.LSIsValidTarget() && Utility.InAARange(m_forcedTarget))
                return m_forcedTarget;

            //buildings
            if (ActiveMode == Mode.LaneClear && m_Configuration.AttackStructures && !EloBuddy.SDK.Orbwalker.ShouldWait)
            {
                /* turrets */
                foreach (var turret in
                    ObjectManager.Get<Obj_AI_Turret>().Where(t => t.LSIsValidTarget() && Utility.InAARange(t)))
                {
                    return turret;
                }

                /* inhibitor */
                foreach (var turret in
                    ObjectManager.Get<Obj_BarracksDampener>().Where(t => t.LSIsValidTarget() && Utility.InAARange(t)))
                {
                    return turret;
                }

                /* nexus */
                foreach (var nexus in
                    ObjectManager.Get<Obj_HQ>().Where(t => t.LSIsValidTarget() && Utility.InAARange(t)))
                {
                    return nexus;
                }
            }

            //champions
            if (ActiveMode != Mode.LastHit)
            {
                if (ActiveMode == Mode.LaneClear && EloBuddy.SDK.Orbwalker.ShouldWait)
                    return null;

                if ((ActiveMode == Mode.LaneClear && !m_Configuration.DontAttackChampWhileLaneClear) || ActiveMode == Mode.Combo || ActiveMode == Mode.Mixed)
                {
                    float range = -1;
                    range = (ObjectManager.Player.IsMelee && m_Configuration.MagnetMelee && m_Configuration.StickRange > ObjectManager.Player.AttackRange) ? m_Configuration.StickRange : -1;
                    if (ObjectManager.Player.CharData.BaseSkinName == "Azir")
                        range = 1000f;
                    var target = TargetSelector.GetTarget(range, DamageType.Physical);
                    if (target.LSIsValidTarget() && (Utility.InAARange(target) || (ActiveMode != Mode.LaneClear && ObjectManager.Player.IsMelee && m_Configuration.MagnetMelee && target.LSIsValidTarget(m_Configuration.StickRange))))
                        return target;
                }
            }

            return null;
        }

        private bool ShouldWaitUnderTurret(Obj_AI_Minion noneKillableMinion)
        {
            return
                ObjectManager.Get<Obj_AI_Minion>()
                    .Any(
                        minion =>
                            (noneKillableMinion != null ? noneKillableMinion.NetworkId != minion.NetworkId : true) &&
                            minion.LSIsValidTarget() && minion.Team != GameObjectTeam.Neutral &&
                            InAutoAttackRange(minion) && MinionManager.IsMinion(minion, false) &&
                            HealthPrediction.LaneClearHealthPrediction(
                                minion,
                                (int)
                                    (ObjectManager.Player.AttackDelay * 1000 +
                                     (ObjectManager.Player.IsMelee
                                         ? ObjectManager.Player.AttackCastDelay * 1000
                                         : ObjectManager.Player.AttackCastDelay * 1000 +
                                           1000 * (ObjectManager.Player.AttackRange + 2 * ObjectManager.Player.BoundingRadius) /
                                           ObjectManager.Player.BasicAttack.MissileSpeed)), ConfigMenu.getSliderItem("Orbwalking.Root.iExtraWindup")) <=
                            ObjectManager.Player.LSGetAutoAttackDamage(minion));
        }


        /// <summary>
        /// Registers the CanAttack function
        /// </summary>
        /// <param name="fn">The function.</param>
        public void RegisterCanAttack(Func<bool> fn)
        {
            m_fnCanAttack = fn;
        }

        /// <summary>
        /// Registers the CanMove function
        /// </summary>
        /// <param name="fn">The function.</param>
        public void RegisterCanMove(Func<bool> fn)
        {
            m_fnCanMove = fn;
        }

        /// <summary>
        /// Registers the CanOrbwalkTarget function
        /// </summary>
        /// <param name="fn">The function.</param>
        public void RegisterCanOrbwalkTarget(Func<AttackableUnit, bool> fn)
        {
            m_fnCanOrbwalkTarget = fn;
        }

        /// <summary>
        /// Registers the ShouldWait function
        /// </summary>
        /// <param name="fn">The function</param>
        public void RegisterShouldWait(Func<bool> fn)
        {
            m_fnShouldWait = fn;
        }

        /// <summary>
        /// Unregisters the CanAttack function
        /// </summary>
        public void UnRegisterCanAttack()
        {
            m_fnCanAttack = null;
        }

        /// <summary>
        /// Unregisters the CanMove function
        /// </summary>
        public void UnRegisterCanMove()
        {
            m_fnCanMove = null;
        }

        /// <summary>
        /// Unregisters the CanOrbwalkTarget function
        /// </summary>
        public void UnRegisterCanOrbwalkTarget()
        {
            m_fnCanOrbwalkTarget = null;
        }

        /// <summary>
        /// Unregisters the ShouldWait function
        /// </summary>
        public void UnRegisterShouldWait()
        {
            m_fnShouldWait = null;
        }

        /// <summary>
        /// Game.OnUpdate event
        /// </summary>
        /// <param name="args">The args.</param>
        private void Game_OnUpdate(EventArgs args)
        {
            if (ActiveMode == Mode.None || (ObjectManager.Player.IsCastingInterruptableSpell(true) && m_channelingWait) || ObjectManager.Player.IsDead)
                return;

            if (CanMove() && m_attackInProgress)
                m_attackInProgress = false;

            var t = GetTarget();
            if (t != null)
                m_lastTarget = t;

            if (ObjectManager.Player.IsMelee && m_Configuration.MagnetMelee && t is AIHeroClient)
                Magnet(t);
            else
                OrbwalkingPoint = Vector3.Zero;

            Orbwalk(t);

            EloBuddy.SDK.Orbwalker.DisableAttacking = true;
            EloBuddy.SDK.Orbwalker.DisableMovement = true;
        }

        /// <summary>
        /// OnProcessSpellCast event for detect the auto attack and auto attack resets
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (Utility.IsAutoAttack(args.SData.Name))
                {
                    m_IslastCastedAA = true;
                    OnAttackArgs onAttackArgs = Events.FireOnAttack(this, args.Target as AttackableUnit);
                    if (!onAttackArgs.Cancel)
                    {
                        m_lastAATick = Utils.TickCount - Game.Ping / 2;
                        m_lastWindUpTime = (int)Math.Round(sender.AttackCastDelay * 1000);
                        m_lastAttackCooldown = (int)Math.Round(sender.AttackDelay * 1000);
                        m_lastAttackCompletesAt = m_lastAATick + m_lastWindUpTime;
                        m_lastAttackPos = ObjectManager.Player.ServerPosition.LSTo2D();
                        m_attackInProgress = true;
                    }
                    if (m_baseAttackSpeed == 0.5f)
                        SetOrbwalkValues();
                }
                else
                {
                    m_IslastCastedAA = false;
                    if (Utility.IsAutoAttackReset(args.SData.Name))
                    {
                        ResetAATimer();
                    }
                    else if (!Utility.IsAutoAttackReset(args.SData.Name))
                    {
                        if (m_attackInProgress)
                            ResetAATimer();
                    }
                    else if (args.SData.Name == "AspectOfTheCougar")
                    {
                        ResetOrbwalkValues();
                    }
                }
            }
            else
            {
                if (sender.Type == GameObjectType.obj_AI_Turret && args.Target.Type == GameObjectType.obj_AI_Minion && sender.Team == ObjectManager.Player.Team && args.Target.Position.LSDistance(ObjectManager.Player.ServerPosition) <= 2000)
                {
                    m_towerTarget = args.Target as Obj_AI_Base;
                    m_sourceTower = sender;
                    m_towerAttackTick = Utils.TickCount - Game.Ping / 2;
                }
            }
        }

        /// <summary>
        /// OnNewPath event for the detect rengar leap
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Obj_AI_Base_OnNewPath(Obj_AI_Base sender, GameObjectNewPathEventArgs args)
        {
            if (sender.IsMe && args.IsDash && sender.CharData.BaseSkinName == "Rengar")
            {
                Events.FireOnAttack(this, m_lastTarget);
                m_lastAATick = Utils.TickCount - Game.Ping / 2;
                m_lastWindUpTime = (int)(sender.AttackCastDelay * 1000);
                m_lastAttackCooldown = (int)(sender.AttackDelay * 1000);
                m_lastAttackCompletesAt = m_lastAATick + m_lastWindUpTime;
                m_lastAttackPos = ObjectManager.Player.ServerPosition.LSTo2D();
                m_attackInProgress = true;
                m_rengarAttack = true;
                if (m_baseAttackSpeed == 0.5f)
                    SetOrbwalkValues();
            }
        }

        /// <summary>
        /// OnDamage event for detect rengar leap's end
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Obj_AI_Base_OnDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            if (args.Source.NetworkId == ObjectManager.Player.NetworkId && ObjectManager.Player.CharData.BaseSkinName == "Rengar" && m_rengarAttack)
            {
                AfterAttack(m_lastTarget);
                m_rengarAttack = false;
            }
        }

        /// <summary>
        /// AfterAttack event for detect after attack for heroes which has projectile
        /// </summary>
        /// <param name="data"></param>
        private void PacketHandler_AfterAttack(byte[] data)
        {
            if (BitConverter.ToInt32(data, 2) == ObjectManager.Player.NetworkId && m_IslastCastedAA && m_attackInProgress)
            {
                m_lastAATick = Utils.TickCount - (int)Math.Ceiling(GetWindupTime()) - Game.Ping;
                AfterAttack(m_lastTarget);
            }
        }

        private void PacketHandler_CancelWindup(byte[] data)
        {
            if (BitConverter.ToInt32(data, 2) == ObjectManager.Player.NetworkId)
                ResetAATimer();
        }

        /// <summary>
        /// OnDoCast event for detect after attack for heroes which hasnt projectile
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Obj_AI_Base_OnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (Utility.IsAutoAttack(args.SData.Name) && (m_attackInProgress || !Utility.HasProjectile()))
                    AfterAttack(args.Target as AttackableUnit);
            }
        }

        /// <summary>
        /// OnBuffRemoveEvent for detect orbwalk value changes
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        private void Obj_AI_Base_OnBuffRemove(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (sender.IsMe)
            {
                string buffname = args.Buff.Name.ToLower();
                if (buffname == "swainmetamorphism" || buffname == "gnartransform")
                    ResetOrbwalkValues();

                if (Data.IsImmobilizeBuff(args.Buff.Type))
                    ResetAATimer();
            }
        }

        /// <summary>
        /// OnBuffAdd for detect orbwalk value changes
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        private void Obj_AI_Base_OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (sender.IsMe)
            {
                string buffname = args.Buff.Name.ToLower();
                if (buffname == "jaycestancegun" || buffname == "jaycestancehammer" || buffname == "swainmetamorphism" || buffname == "gnartransform")
                    ResetOrbwalkValues();
            }
        }

        /// <summary>
        /// OnPlayAnimation Event
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        private void Obj_AI_Base_OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            //if (sender.IsMe && m_attackInProgress && (args.Animation == "Run" || args.Animation == "Idle"))
            //{
            //    Game.PrintChat("{0} ({1})", args.Animation, Utils.TickCount);
            //    ResetAATimer();
            //}
        }

        public AttackableUnit LastTarget
        {
            get
            {
                return m_lastTarget;
            }
        }

        /// <summary>
        /// OnStopCast Event
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        private void Spellbook_OnStopCast(Obj_AI_Base sender, SpellbookStopCastEventArgs args)
        {
            if (sender.IsValid && sender.IsMe && args.DestroyMissile && args.StopAnimation)
                ResetAATimer();
        }
    }
}