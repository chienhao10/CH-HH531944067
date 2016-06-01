using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SharpDX;
using Prediction = LeagueSharp.Common.Prediction;
using Spell = LeagueSharp.Common.Spell;

namespace PortAIO.Champion.Bard
{
    internal class Program
    {
        public static Menu BardMenu, comboMenu, harassMenu, miscMenu;

        public static Dictionary<SpellSlot, Spell> spells = new Dictionary<SpellSlot, Spell>
        {
            {SpellSlot.Q, new Spell(SpellSlot.Q, 950f)},
            {SpellSlot.W, new Spell(SpellSlot.W, 945f)},
            {SpellSlot.E, new Spell(SpellSlot.E)}
        };

        public static float LastMoveC;
        public static int TunnelNetworkID;
        public static Vector3 TunnelEntrance = Vector3.Zero;
        public static Vector3 TunnelExit = Vector3.Zero;


        internal static void OnLoad()
        {
            LoadEvents();
            LoadSpells();
            LoadMenu();
        }

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        private static void LoadMenu()
        {
            BardMenu = MainMenu.AddMenu("巴德", "Bard");

            comboMenu = BardMenu.AddSubMenu("连招", "dz191.bard.combo");
            comboMenu.Add("dz191.bard.combo.useq", new CheckBox("使用 Q"));
            comboMenu.Add("dz191.bard.combo.usew", new CheckBox("使用 W"));
            comboMenu.Add("dz191.bard.combo.qks", new CheckBox("使用 Q 抢头"));

            harassMenu = BardMenu.AddSubMenu("骚扰", "dz191.bard.mixed");
            harassMenu.AddGroupLabel("Q 目标 (只骚扰)");
            foreach (var hero in HeroManager.Enemies)
            {
                harassMenu.Add(string.Format("dz191.bard.qtarget.{0}", hero.NetworkId), new CheckBox("Harass : " + hero.ChampionName));
            }
            harassMenu.AddSeparator();
            harassMenu.Add("dz191.bard.mixed.useq", new CheckBox("使用 Q"));

            miscMenu = BardMenu.AddSubMenu("Misc", "dz191.bard.misc");
            miscMenu.AddGroupLabel("W 设置");
            foreach (var hero in HeroManager.Allies)
            {
                miscMenu.Add(string.Format("dz191.bard.wtarget.{0}", hero.NetworkId), new CheckBox("Heal " + hero.ChampionName));
            }
            miscMenu.Add("dz191.bard.wtarget.healthpercent", new Slider("使用 W 治疗当生命", 25, 1));
            miscMenu.AddGroupLabel("Q - 捆绑");
            miscMenu.Add("dz191.bard.misc.distance", new Slider("计算距离", 250, 100, 450));
            miscMenu.Add("dz191.bard.misc.accuracy", new Slider("命中率", 20, 1, 50));
            miscMenu.AddSeparator();
            miscMenu.Add("dz191.bard.misc.attackMinions", new CheckBox("辅助模式"));
            miscMenu.Add("dz191.bard.misc.attackMinionsRange",
                new Slider("友军在 X 范围内不普攻小兵", 1200, 700, 2000));
        }

        private static void LoadSpells()
        {
            spells[SpellSlot.Q].SetSkillshot(0.25f, 65f, 1600f, true, SkillshotType.SkillshotLine);
        }

        private static void LoadEvents()
        {
            Game.OnUpdate += Game_OnUpdate;
            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDelete;
            Orbwalker.OnPreAttack += OnBeforeAttack;
        }

        private static void OnBeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (args.Target.Type == GameObjectType.obj_AI_Minion &&
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) &&
                getCheckBoxItem(miscMenu, "dz191.bard.misc.attackMinions"))
            {
                if (
                    ObjectManager.Player.CountAlliesInRange(getSliderItem(miscMenu, "dz191.bard.misc.attackMinionsRange")) >
                    0)
                {
                    args.Process = false;
                }
            }
        }

        private static void OnDelete(GameObject sender, EventArgs args)
        {
            if (sender.Name.Contains("BardDoor_EntranceMinion") && sender.NetworkId == TunnelNetworkID)
            {
                TunnelNetworkID = -1;
                TunnelEntrance = Vector3.Zero;
                TunnelExit = Vector3.Zero;
            }
        }

        private static void OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name.Contains("BardDoor_EntranceMinion"))
            {
                TunnelNetworkID = sender.NetworkId;
                TunnelEntrance = sender.Position;
            }

            if (sender.Name.Contains("BardDoor_ExitMinion"))
            {
                TunnelExit = sender.Position;
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            var ComboTarget = TargetSelector.GetTarget(spells[SpellSlot.Q].Range / 1.3f, DamageType.Magical);

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (spells[SpellSlot.Q].IsReady() && getCheckBoxItem(comboMenu, "dz191.bard.combo.useq") &&
                    ComboTarget.LSIsValidTarget())
                {
                    HandleQ(ComboTarget);
                }

                if (getCheckBoxItem(comboMenu, "dz191.bard.combo.usew"))
                {
                    HandleW();
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                if (spells[SpellSlot.Q].IsReady() && getCheckBoxItem(harassMenu, "dz191.bard.mixed.useq") &&
                    ComboTarget.LSIsValidTarget() &&
                    getCheckBoxItem(harassMenu,
                        string.Format("dz191.bard.qtarget.{0}", ComboTarget.NetworkId)))
                {
                    HandleQ(ComboTarget);
                }
            }
        }

        private static void HandleQ(AIHeroClient comboTarget)
        {
            var QPrediction = spells[SpellSlot.Q].GetPrediction(comboTarget);

            if (QPrediction.Hitchance >= HitChance.High)
            {
                if (spells[SpellSlot.Q].GetDamage(comboTarget) > comboTarget.Health + 15 &&
                    getCheckBoxItem(comboMenu, "dz191.bard.combo.qks"))
                {
                    spells[SpellSlot.Q].Cast(QPrediction.CastPosition);
                    return;
                }

                var QPushDistance = getSliderItem(miscMenu, "dz191.bard.misc.distance");
                var QAccuracy = getSliderItem(miscMenu, "dz191.bard.misc.accuracy");
                var PlayerPosition = ObjectManager.Player.ServerPosition;

                var BeamStartPositions = new List<Vector3>
                {
                    QPrediction.CastPosition,
                    QPrediction.UnitPosition,
                    comboTarget.ServerPosition,
                    comboTarget.Position
                };

                if (comboTarget.IsDashing())
                {
                    BeamStartPositions.Add(comboTarget.GetDashInfo().EndPos);
                }

                var PositionsList = new List<Vector3>();
                var CollisionPositions = new List<Vector3>();

                foreach (var position in BeamStartPositions)
                {
                    var collisionableObjects = spells[SpellSlot.Q].GetCollision(position.LSTo2D(),
                        new List<Vector2> { position.LSExtend(PlayerPosition, -QPushDistance).LSTo2D() });

                    if (collisionableObjects.Any())
                    {
                        if (collisionableObjects.Any(h => h is AIHeroClient) &&
                            collisionableObjects.All(h => h.LSIsValidTarget()))
                        {
                            spells[SpellSlot.Q].Cast(QPrediction.CastPosition);
                            break;
                        }

                        for (var i = 0; i < QPushDistance; i += (int)comboTarget.BoundingRadius)
                        {
                            CollisionPositions.Add(position.LSExtend(PlayerPosition, -i));
                        }
                    }

                    for (var i = 0; i < QPushDistance; i += (int)comboTarget.BoundingRadius)
                    {
                        PositionsList.Add(position.LSExtend(PlayerPosition, -i));
                    }
                }

                if (PositionsList.Any())
                {
                    //We don't want to divide by 0 Kappa
                    var WallNumber = PositionsList.Count(p => p.LSIsWall()) * 1.3f;
                    var CollisionPositionCount = CollisionPositions.Count;
                    var Percent = (WallNumber + CollisionPositionCount) / PositionsList.Count;
                    var AccuracyEx = QAccuracy / 100f;
                    if (Percent >= AccuracyEx)
                    {
                        spells[SpellSlot.Q].Cast(QPrediction.CastPosition);
                    }
                }
            }
            else if (QPrediction.Hitchance == HitChance.Collision)
            {
                var QCollision = QPrediction.CollisionObjects;
                if (QCollision.Count == 1)
                {
                    spells[SpellSlot.Q].Cast(QPrediction.CastPosition);
                }
            }
        }


        private static void HandleW()
        {
            if (ObjectManager.Player.LSIsRecalling() || ObjectManager.Player.InShop() || !spells[SpellSlot.W].IsReady())
            {
                return;
            }

            if (ObjectManager.Player.HealthPercent <= getSliderItem(miscMenu, "dz191.bard.wtarget.healthpercent"))
            {
                var castPosition = ObjectManager.Player.ServerPosition.LSExtend(Game.CursorPos, 65);
                spells[SpellSlot.W].Cast(castPosition);
                return;
            }

            var LowHealthAlly =
                HeroManager.Allies.Where(
                    ally =>
                        ally.LSIsValidTarget(spells[SpellSlot.W].Range) &&
                        ally.HealthPercent <= getSliderItem(miscMenu, "dz191.bard.wtarget.healthpercent") &&
                        getCheckBoxItem(miscMenu, string.Format("dz191.bard.wtarget.{0}", ally.NetworkId)))
                    .OrderBy(ally => ally.Health)
                    .FirstOrDefault();

            if (LowHealthAlly != null)
            {
                var movementPrediction = Prediction.GetPrediction(LowHealthAlly, 0.25f);
                spells[SpellSlot.W].Cast(movementPrediction.UnitPosition);
            }
        }

        private static bool IsOverWall(Vector3 start, Vector3 end)
        {
            double distance = Vector3.Distance(start, end);
            for (uint i = 0; i < distance; i += 10)
            {
                var tempPosition = start.LSExtend(end, i);
                if (tempPosition.LSIsWall())
                {
                    return true;
                }
            }

            return false;
        }

        private static Vector3 GetFirstWallPoint(Vector3 start, Vector3 end)
        {
            double distance = Vector3.Distance(start, end);
            for (uint i = 0; i < distance; i += 10)
            {
                var tempPosition = start.LSExtend(end, i);
                if (tempPosition.LSIsWall())
                {
                    return tempPosition.LSExtend(start, -35);
                }
            }

            return Vector3.Zero;
        }

        private static float GetWallLength(Vector3 start, Vector3 end)
        {
            double distance = Vector3.Distance(start, end);
            var firstPosition = Vector3.Zero;
            var lastPosition = Vector3.Zero;

            for (uint i = 0; i < distance; i += 10)
            {
                var tempPosition = start.LSExtend(end, i);
                if (tempPosition.LSIsWall() && firstPosition == Vector3.Zero)
                {
                    firstPosition = tempPosition;
                }
                lastPosition = tempPosition;
                if (!lastPosition.LSIsWall() && firstPosition != Vector3.Zero)
                {
                    break;
                }
            }

            return Vector3.Distance(firstPosition, lastPosition);
        }

        public static void MoveToLimited(Vector3 where)
        {
            if (Environment.TickCount - LastMoveC < 80)
            {
                return;
            }

            LastMoveC = Environment.TickCount;

            Player.IssueOrder(GameObjectOrder.MoveTo, where);
        }
    }
}