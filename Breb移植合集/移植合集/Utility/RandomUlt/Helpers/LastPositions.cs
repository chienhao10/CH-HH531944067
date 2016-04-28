using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using LeagueSharp.Common;
using SharpDX;
using Collision = LeagueSharp.Common.Collision;
using Color = System.Drawing.Color;
using Extensions = EloBuddy.SDK.Extensions;
using Utility = LeagueSharp.Common.Utility;

namespace RandomUlt.Helpers
{
    internal class LastPositions
    {
        public static Menu menu;
        public static List<Positions> Enemies;
        public static Spell R;
        public static readonly AIHeroClient player = ObjectManager.Player;

        public static List<string> SupportedHeroes =
            new List<string>(new[] {"Ezreal", "Jinx", "Ashe", "Draven", "Gangplank", "Ziggs", "Lux", "Xerath"});

        public static List<string> BaseUltHeroes = new List<string>(new[] {"Ezreal", "Jinx", "Ashe", "Draven"});

        public static int[] xerathUltRange = {3200, 4400, 5600};
        public bool enabled = true;

        public List<Vector3> ShielderTurretsChaos =
            new List<Vector3>(new[] {new Vector3(7943.15f, 13411.8f, 38), new Vector3(13327.4f, 8226.28f, 38)});

        public List<Vector3> ShielderTurretsOrder =
            new List<Vector3>(new[]
            {new Vector3(6919.155f, 1483.599f, 43.32f), new Vector3(1512.892f, 6699.57f, 42.06392f)});

        public Vector3 SpawnPos;
        public bool xerathUltActivated;
        public AIHeroClient xerathUltTarget;

        public LastPositions(Menu config)
        {
            menu = config;
            R = new Spell(SpellSlot.R);
            if (player.ChampionName == "Ezreal")
            {
                R.SetSkillshot(1.2f, 160f, 2000f, false, SkillshotType.SkillshotLine);
            }
            if (player.ChampionName == "Jinx")
            {
                R.SetSkillshot(0.6f, 140f, 1700f, true, SkillshotType.SkillshotLine);
            }
            if (player.ChampionName == "Ashe")
            {
                R.SetSkillshot(0.25f, 130f, 1600f, true, SkillshotType.SkillshotLine);
            }
            if (player.ChampionName == "Draven")
            {
                R.SetSkillshot(0.4f, 160f, 2000f, true, SkillshotType.SkillshotLine);
            }
            if (player.ChampionName == "Lux")
            {
                R.SetSkillshot(0.5f, 190f, float.MaxValue, false, SkillshotType.SkillshotLine);
            }
            if (player.ChampionName == "Ziggs")
            {
                R.SetSkillshot(0.1f, 525f, 1750f, false, SkillshotType.SkillshotCircle);
            }
            if (player.ChampionName == "Gangplank")
            {
                R.SetSkillshot(0.1f, 600f, R.Speed, false, SkillshotType.SkillshotCircle);
            }
            if (player.ChampionName == "Xerath")
            {
                R.SetSkillshot(0.7f, 120f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            }
            var objSpawnPoint = ObjectManager.Get<Obj_SpawnPoint>().FirstOrDefault(x => x.IsEnemy);
            if (objSpawnPoint != null)
                SpawnPos = objSpawnPoint.Position;
            if (SupportedChamps())
            {
                config.Add("UseR", new CheckBox("Use R"));
                if (player.ChampionName == "Gangplank")
                {
                    config.Add("gpWaves", new Slider("GP ult waves to damage", 2, 1, 7));
                }
                if (player.ChampionName == "Xerath")
                {
                    config.Add("XerathUlts", new Slider("Xerath ults to damage", 2, 1, 3));
                }
                if (player.ChampionName == "Draven")
                {
                    config.Add("Backdamage", new CheckBox("Count second hit"));
                    config.Add("CallBack", new CheckBox("Reduce time between hits"));
                }
                config.Add("Hitchance", new Slider("Hitchance", 3, 1, 5));
                config.AddSeparator();
                config.AddGroupLabel("Don't Ult :");
                foreach (var e in HeroManager.Enemies)
                {
                    config.Add(e.ChampionName + "DontUltRandomUlt", new CheckBox(e.ChampionName, false));
                }
                config.AddSeparator();
                config.Add("Alliesrange", new Slider("Allies min range from the target", 1500, 500, 2000));
                config.Add("EnemiesAroundYou", new Slider("Block if enemies around you", 600, 0, 2000));
                config.Add("waitBeforeUlt", new Slider("Wait time before ults(ms)", 600, 0, 3000));
                config.Add("BaseUltFirst", new CheckBox("BaseUlt has higher priority", false));
                config.Add("Collision", new CheckBox("Calc damage reduction"));
                config.Add("drawNotification", new CheckBox("Draw notification"));
            }
            config.Add("RandomUltDrawings", new CheckBox("Draw possible place", false));
            config.AddSeparator();
            config.AddGroupLabel("Block Keys : ");
            config.Add("OrbBlock1", new KeyBind("Disabled by keypress", false, KeyBind.BindTypes.HoldActive, 65));
            config.Add("OrbBlock2", new KeyBind("Disabled by keypress", false, KeyBind.BindTypes.HoldActive, 88));
            config.Add("OrbBlock3", new KeyBind("Disabled by keypress", false, KeyBind.BindTypes.HoldActive, 67));
            config.Add("ComboBlock", new KeyBind("Disabled by Combo", false, KeyBind.BindTypes.HoldActive, 32));
            config.Add("OnlyCombo", new CheckBox("Only Combo key"));

            Enemies = HeroManager.Enemies.Select(x => new Positions(x)).ToList();
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnTeleport += Obj_AI_Base_OnTeleport;
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

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }


        private void Obj_AI_Base_OnTeleport(Obj_AI_Base sender, GameObjectTeleportEventArgs args)
        {
            var unit = sender as AIHeroClient;

            if (unit == null || !unit.IsValid || unit.IsAlly)
            {
                return;
            }

            var recall = Packet.S2C.Teleport.Decoded(unit, args);
            Enemies.Find(x => x.Player.NetworkId == recall.UnitNetworkId).RecallData.Update(recall);
        }

        private bool SupportedChamps()
        {
            return SupportedHeroes.Any(h => h.Contains(player.ChampionName));
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (!getCheckBoxItem(menu, "RandomUltDrawings") || !enabled || getKeyBindItem(menu, "ComboBlock"))
            {
                return;
            }
            foreach (var enemy in
                Enemies.Where(
                    x =>
                        x.Player.IsValid<AIHeroClient>() && !x.Player.IsDead &&
                        x.RecallData.Recall.Status == Packet.S2C.Teleport.Status.Start &&
                        x.RecallData.Recall.Type == Packet.S2C.Teleport.Type.Recall)
                    .OrderBy(x => x.RecallData.GetRecallTime()))
            {
                var trueDist = Math.Abs(enemy.LastSeen - enemy.RecallData.RecallStartTime)/1000*
                               enemy.Player.MoveSpeed;
                var dist = Math.Abs(enemy.LastSeen - enemy.RecallData.RecallStartTime)/1000*enemy.Player.MoveSpeed -
                           enemy.Player.MoveSpeed/3;
                if (dist > 1500)
                {
                    continue;
                }

                if (dist < 50)
                {
                    dist = 50;
                }
                var line = getpos(enemy, trueDist);
                Vector3 pos;
                if (enemy.Player.IsVisible)
                {
                    pos = enemy.Player.Position;
                }
                else if (Extensions.Distance(line, enemy.Player.Position) < dist &&
                         (enemy.predictedpos.UnderTurret(true) ||
                          NavMesh.GetCollisionFlags(enemy.predictedpos).HasFlag(CollisionFlags.Grass)))
                {
                    pos = enemy.predictedpos;
                }
                else
                {
                    pos =
                        PointsAroundTheTarget(enemy.Player.Position, trueDist)
                            .Where(
                                p =>
                                    !Extensions.IsWall(p) && Extensions.Distance(line, p) < dist/1.5f &&
                                    GetPath(enemy.Player, p) < trueDist)
                            .OrderByDescending(p => NavMesh.GetCollisionFlags(p).HasFlag(CollisionFlags.Grass))
                            .ThenBy(p => Extensions.Distance(line, p))
                            .FirstOrDefault();
                }
                if (pos != null)
                {
                    Render.Circle.DrawCircle(pos, 50, Color.Red, 8);
                }
                if (!enemy.Player.IsVisible)
                {
                    if (pos != null)
                    {
                        Drawing.DrawCircle(line, dist/1.5f, Color.LawnGreen);
                    }
                }
            }
            if (SupportedChamps() && getCheckBoxItem(menu, "drawNotification") && R.IsReady() &&
                !player.IsDead)
            {
                var possibleTargets =
                    Enemies.Where(
                        x =>
                            !x.Player.IsDead && checkdmg(x.Player, x.Player.Position) &&
                            (Environment.TickCount - x.LastSeen < 4000) && x.Player.CountAlliesInRange(1000) < 1 &&
                            UltTime(x.Player.Position) <
                            9500 - getSliderItem(menu, "waitBeforeUlt"));
                if (possibleTargets.Any() && player.IsHPBarRendered)
                {
                    Drawing.DrawText(
                        player.HPBarPosition.X + 8, player.HPBarPosition.Y - 30, Color.Red, "Possible Randomult");
                }
            }
        }

        private Vector3 getpos(Positions enemy, float dist)
        {
            var time = (enemy.LastSeen - enemy.RecallData.RecallStartTime)/1000;
            var line = enemy.Player.Position.LSExtend(enemy.predictedpos, dist);
            if (Extensions.Distance(enemy.Player.Position, enemy.predictedpos) < dist && (time < 2 || Extensions.Distance(enemy.Player.Position, enemy.predictedpos) > Extensions.Distance(enemy.Player.Position, line)*0.70f))
            {
                line = enemy.predictedpos;
            }
            return line;
        }

        private void Game_OnUpdate(EventArgs args)
        {
            float time = Environment.TickCount;
            foreach (var enemyInfo in
                Enemies.Where(x => x.Player.IsVisible && !x.Player.IsDead && Extensions.IsValidTarget(x.Player)))
            {
                enemyInfo.LastSeen = time;
                var prediction = Prediction.GetPrediction(enemyInfo.Player, 10);
                if (prediction != null)
                {
                    enemyInfo.predictedpos = prediction.UnitPosition;
                }
            }
            if (xerathUltActivated && R.IsReady() && !getKeyBindItem(menu, "ComboBlock") &&
                player.HasBuff("xerathrshots"))
            {
                var enemy = Enemies.FirstOrDefault(x => x.Player.NetworkId == xerathUltTarget.NetworkId && !x.Player.IsDead);
                if (enemy != null)
                {
                    R.Cast(enemy.Player.Position);
                }
                else
                {
                    var target =
                        HeroManager.Enemies.Where(
                            h => Extensions.Distance(player, h) < xerathUltRange[R.Level - 1] && h.IsVisible)
                            .OrderBy(h => h.Health)
                            .FirstOrDefault();
                    if (target != null)
                    {
                        R.Cast(target);
                    }
                }
            }
            if (!SupportedChamps() || !getCheckBoxItem(menu, "UseR") || !R.IsReady() || !enabled ||
                getKeyBindItem(menu, "ComboBlock"))
            {
                return;
            }
            if (Extensions.CountEnemiesInRange(player, getSliderItem(menu, "EnemiesAroundYou")) >=
                1)
            {
                return;
            }
            if (!getCheckBoxItem(menu, "OnlyCombo") &&
                (getKeyBindItem(menu, "OrbBlock1") ||
                 getKeyBindItem(menu, "OrbBlock2") ||
                 getKeyBindItem(menu, "OrbBlock3")))
            {
                return;
            }
            if (player.ChampionName == "Draven" && player.Spellbook.GetSpell(SpellSlot.R).Name != "DravenRCast")
            {
                return;
            }
            var HitChance = getSliderItem(menu, "Hitchance");
            foreach (var enemy in
                Enemies.Where(
                    x =>
                        x.Player.IsValid<AIHeroClient>() && !x.Player.IsDead &&
                        !getCheckBoxItem(menu, x.Player.ChampionName + "DontUltRandomUlt") &&
                        x.RecallData.Recall.Status == Packet.S2C.Teleport.Status.Start &&
                        x.RecallData.Recall.Type == Packet.S2C.Teleport.Type.Recall)
                    .OrderBy(x => x.RecallData.GetRecallTime()))
            {
                if (CheckBuffs(enemy.Player) || CheckBaseUlt(enemy.RecallData.GetRecallCountdown()) ||
                    !(Environment.TickCount - enemy.RecallData.RecallStartTime >
                      getSliderItem(menu, "waitBeforeUlt")))
                {
                    continue;
                }
                var dist = Math.Abs(enemy.LastSeen - enemy.RecallData.RecallStartTime)/1000*enemy.Player.MoveSpeed -
                           enemy.Player.MoveSpeed/3;
                var trueDist = Math.Abs(enemy.LastSeen - enemy.RecallData.RecallStartTime)/1000*
                               enemy.Player.MoveSpeed;
                var line = getpos(enemy, dist);
                switch (HitChance)
                {
                    case 1:
                        break;
                    case 2:
                        if (trueDist > 1000 && !enemy.Player.IsVisible)
                        {
                            continue;
                        }
                        break;
                    case 3:
                        if (trueDist > 850 && !enemy.Player.IsVisible)
                        {
                            continue;
                        }
                        break;
                    case 4:
                        if (trueDist > 700 && !enemy.Player.IsVisible)
                        {
                            continue;
                        }
                        break;
                    case 5:
                        if (trueDist > 500 && !enemy.Player.IsVisible)
                        {
                            continue;
                        }
                        break;
                }
                Vector3 pos;
                if (enemy.Player.IsVisible)
                {
                    pos = enemy.Player.Position;
                }
                else if (Extensions.Distance(line, enemy.Player.Position) < dist &&
                         (enemy.predictedpos.UnderTurret(true) ||
                          NavMesh.GetCollisionFlags(enemy.predictedpos).HasFlag(CollisionFlags.Grass)))
                {
                    pos = enemy.predictedpos;
                }
                {
                    if (dist > 1500)
                    {
                        continue;
                    }
                    pos =
                        PointsAroundTheTarget(enemy.Player.Position, trueDist)
                            .Where(
                                p =>
                                    !Extensions.IsWall(p) && Extensions.Distance(line, p) < dist/1.2f &&
                                    GetPath(enemy.Player, p) < trueDist)
                            .OrderByDescending(p => NavMesh.GetCollisionFlags(p).HasFlag(CollisionFlags.Grass))
                            .ThenBy(p => Extensions.Distance(line, p))
                            .FirstOrDefault();
                }
                if (pos != null)
                {
                    if (player.ChampionName == "Ziggs" && Extensions.Distance(player, pos) > 5000f)
                    {
                        continue;
                    }
                    if (player.ChampionName == "Lux" && Extensions.Distance(player, pos) > 3000f)
                    {
                        continue;
                    }
                    if (player.ChampionName == "Xerath" &&
                        Extensions.Distance(player, pos) > xerathUltRange[R.Level - 1] - 500)
                    {
                        continue;
                    }
                    kill(enemy, new Vector3(pos.X, pos.Y, 0));
                }
            }
        }

        private bool CheckBaseUlt(float recallCooldown)
        {
            if (getCheckBoxItem(menu, "BaseUltFirst") &&
                BaseUltHeroes.Any(h => h.Contains(player.ChampionName)) && recallCooldown > UltTime(SpawnPos))
            {
                return true;
            }
            return false;
        }

        private bool CheckBuffs(AIHeroClient enemy)
        {
            if (enemy.ChampionName == "Anivia")
            {
                if (enemy.HasBuff("rebirthcooldown"))
                {
                    return false;
                }
                return true;
            }
            if (enemy.ChampionName == "Aatrox")
            {
                if (enemy.HasBuff("aatroxpassiveready"))
                {
                    return true;
                }
            }
            return false;
        }

        public static float GetPath(AIHeroClient hero, Vector3 b)
        {
            var path = hero.GetPath(b);
            Vector3[] lastPoint = {path[0]};
            var distance = 0f;
            foreach (var point in path.Where(point => !point.Equals(lastPoint[0])))
            {
                distance += Extensions.Distance(lastPoint[0], point);
                lastPoint[0] = point;
            }
            return distance;
        }

        public static Vector3 GetPointAfterTimeFromPath(AIHeroClient hero, Vector3 b, float timeInSec)
        {
            var path = hero.GetPath(b);
            Vector3[] lastPoint = {path[0]};
            var distance = 0f;
            var maxDist = hero.MoveSpeed*timeInSec;
            foreach (var point in path.Where(point => !point.Equals(lastPoint[0])))
            {
                if (distance > maxDist)
                {
                    break;
                }
                distance += Extensions.Distance(lastPoint[0], point);
                lastPoint[0] = point;
            }
            return lastPoint[0];
        }

        private bool CheckShieldTower(Vector3 pos)
        {
            if (Game.MapId != GameMapId.SummonersRift)
            {
                return false;
            }
            if (player.Team == GameObjectTeam.Chaos)
            {
                return ShielderTurretsOrder.Any(s => Extensions.Distance(s, pos) < 1150f);
            }
            if (player.Team == GameObjectTeam.Order)
            {
                return ShielderTurretsChaos.Any(s => Extensions.Distance(s, pos) < 1150f);
            }
            return false;
        }

        private void kill(Positions positions, Vector3 pos)
        {
            if (R.IsReady() && Extensions.Distance(pos, positions.Player.Position) < 1200 &&
                ObjectManager.Get<AIHeroClient>()
                    .Count(
                        o =>
                            o.IsAlly &&
                            Extensions.Distance(o, pos) < getSliderItem(menu, "Alliesrange")) <
                1)
            {
                if (checkdmg(positions.Player, pos) && UltTime(pos) < positions.RecallData.GetRecallTime() &&
                    !isColliding(pos))
                {
                    if (player.ChampionName == "Xerath")
                    {
                        xerathUlt(positions, pos);
                    }
                    R.Cast(pos);
                    if (player.ChampionName == "Draven" && getCheckBoxItem(menu, "CallBack"))
                    {
                        Utility.DelayAction.Add((int) (UltTime(pos) - 300), () => R.Cast());
                    }
                }
            }
        }

        private void xerathUlt(Positions positions, Vector3 pos = default(Vector3))
        {
            if (pos != Vector3.Zero)
            {
                xerathUltActivated = true;
                xerathUltTarget = positions.Player;
                Utility.DelayAction.Add(5000, () => xerathUltActivated = false);
                R.Cast(pos);
            }
            else
            {
                if (positions.Player.IsVisible)
                {
                    xerathUltActivated = true;
                    xerathUltTarget = positions.Player;
                    Utility.DelayAction.Add(5000, () => xerathUltActivated = false);
                    R.Cast(positions.Player);
                }
                else
                {
                    xerathUltActivated = true;
                    xerathUltTarget = positions.Player;
                    Utility.DelayAction.Add(5000, () => xerathUltActivated = false);
                    R.Cast(
                        Extensions.Extend(positions.Player.Position, positions.predictedpos,
                            (float) (positions.Player.MoveSpeed*0.3)));
                }
            }
        }

        private bool isColliding(Vector3 pos)
        {
            if (player.ChampionName == "Draven" && player.ChampionName == "Ashe" && player.ChampionName == "Jinx")
            {
                var input = new PredictionInput {Radius = R.Width, Unit = player};

                input.CollisionObjects[0] = CollisionableObjects.Heroes;

                return Collision.GetCollision(new List<Vector3> {pos}, input).Any();
            }
            return false;
        }

        private float UltTime(Vector3 pos)
        {
            var dist = Extensions.Distance(player.ServerPosition, pos);
            if (player.ChampionName == "Ezreal")
            {
                return dist/2000*1000 + 1000;
            }
            //Beaving's calculations
            if (player.ChampionName == "Jinx" && dist > 1350)
            {
                const float accelerationrate = 0.3f;

                var acceldifference = dist - 1350f;

                if (acceldifference > 150f)
                {
                    acceldifference = 150f;
                }

                var difference = dist - 1500f;
                return dist/
                       ((1350f*1700f + acceldifference*(1700f + accelerationrate*acceldifference) +
                         difference*1700f)/dist)*1000 + 250;
            }
            if (player.ChampionName == "Ashe")
            {
                return dist/1600*1000 + 250;
            }
            if (player.ChampionName == "Draven")
            {
                return dist/2000*1000 + 400;
            }
            if (player.ChampionName == "Ziggs")
            {
                return dist/1750f*1000 + 1000;
            }
            if (player.ChampionName == "Lux")
            {
                return 500f;
            }
            if (player.ChampionName == "Xerath")
            {
                return 500f;
            }
            return 0;
        }

        public static List<Vector3> PointsAroundTheTarget(Vector3 pos, float dist, float prec = 15, float prec2 = 5)
        {
            if (!pos.IsValid())
            {
                return new List<Vector3>();
            }
            var list = new List<Vector3>();
            if (dist > 500)
            {
                prec = 20;
                prec2 = 6;
            }
            if (dist > 805)
            {
                prec = 35;
                prec2 = 8;
            }
            var angle = 360/prec*Math.PI/180.0f;
            var step = dist/prec2;
            for (var i = 0; i < prec; i++)
            {
                for (var j = 0; j < prec2; j++)
                {
                    list.Add(
                        new Vector3(
                            pos.X + (float) (Math.Cos(angle*i)*(j*step)),
                            pos.Y + (float) (Math.Sin(angle*i)*(j*step)), pos.Z));
                }
            }

            return list;
        }

        private bool checkdmg(AIHeroClient target, Vector3 pos)
        {
            var dmg = R.GetDamage(target);
            var bonuShieldNearTowers = 0f;
            var collision = getCheckBoxItem(menu, "Collision");
            if (CheckShieldTower(pos))
            {
                bonuShieldNearTowers = 300f;
            }
            if (player.ChampionName == "Ezreal")
            {
                if (dmg*(collision ? 0.7f : 1f) - 10 - bonuShieldNearTowers > target.Health)
                {
                    return true;
                }
            }
            if (player.ChampionName == "Draven")
            {
                if (getCheckBoxItem(menu, "Backdamage"))
                {
                    dmg = dmg*2;
                }
                if (dmg*(collision ? 0.8f : 1f) - 10 - bonuShieldNearTowers > target.Health)
                {
                    return true;
                }
            }
            if (player.ChampionName == "Jinx")
            {
                if (R.GetDamage(target, 1) - 10 - bonuShieldNearTowers > target.Health)
                {
                    return true;
                }
            }
            if (player.ChampionName == "Gangplank")
            {
                if (getSliderItem(menu, "gpWaves") * dmg - bonuShieldNearTowers > target.Health)
                {
                    return true;
                }
            }
            if (player.ChampionName == "Xerath")
            {
                if (getSliderItem(menu, "XerathUlts") * dmg - bonuShieldNearTowers > target.Health)
                {
                    return true;
                }
            }
            if (player.ChampionName == "Ashe" || player.ChampionName == "Lux" || player.ChampionName == "Ziggs")
            {
                if (dmg - 10 - bonuShieldNearTowers > target.Health)
                {
                    return true;
                }
            }
            return false;
        }
    }

    internal class Positions
    {
        public float LastSeen;
        public AIHeroClient Player;
        public Vector3 predictedpos;

        public RecallData RecallData;

        public Positions(AIHeroClient player)
        {
            Player = player;
            RecallData = new RecallData(this);
        }
    }

    internal class RecallData
    {
        public Packet.S2C.Teleport.Struct Aborted;
        public float AbortTime;
        public int FADEOUT_TIME = 3000;
        public Positions Positions;
        public Packet.S2C.Teleport.Struct Recall;
        public float RecallStartTime;
        public bool started;

        public RecallData(Positions positions)
        {
            Positions = positions;
            Recall = new Packet.S2C.Teleport.Struct(
                Positions.Player.NetworkId, Packet.S2C.Teleport.Status.Unknown, Packet.S2C.Teleport.Type.Unknown, 0);
        }

        public float GetRecallTime()
        {
            float time = Environment.TickCount;
            float countdown = 0;

            if (time - AbortTime < 2000)
            {
                countdown = Aborted.Duration - (AbortTime - Aborted.Start);
            }
            else if (AbortTime > 0)
            {
                countdown = 0;
            }
            else
            {
                countdown = Recall.Start + Recall.Duration - time;
            }

            return countdown < 0 ? 0 : countdown;
        }

        public float GetRecallCountdown()
        {
            float time = Environment.TickCount;
            float countdown = 0;

            if (time - AbortTime < FADEOUT_TIME)
            {
                countdown = Aborted.Duration - (AbortTime - Aborted.Start);
            }
            else if (AbortTime > 0)
            {
                countdown = 0;
            }
            else
            {
                countdown = Recall.Start + Recall.Duration - time;
            }

            return countdown < 0 ? 0 : countdown;
        }

        public Positions Update(Packet.S2C.Teleport.Struct newData)
        {
            if (newData.Type == Packet.S2C.Teleport.Type.Recall && newData.Status == Packet.S2C.Teleport.Status.Abort)
            {
                Aborted = Recall;
                AbortTime = Environment.TickCount;
                started = false;
            }
            else
            {
                AbortTime = 0;
            }
            if (newData.Type == Packet.S2C.Teleport.Type.Recall && newData.Status == Packet.S2C.Teleport.Status.Finish)
            {
                started = false;
            }
            if (newData.Type == Packet.S2C.Teleport.Type.Recall && newData.Status == Packet.S2C.Teleport.Status.Start)
            {
                if (!started)
                {
                    RecallStartTime = Environment.TickCount;
                }
                started = true;
            }
            Recall = newData;
            return Positions;
        }
    }
}