namespace KappaBaseUlt
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;

    using SharpDX;

    using Color = SharpDX.Color;

    internal static class Program
    {
        private static Menu baseMenu;

        private static readonly List<EnemyInfo> RecallsList = new List<EnemyInfo>();

        private static Spell.Skillshot R { get; set; }

        private static float Damage;

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            var me = Database.Champions.FirstOrDefault(hero => hero.Champion == Player.Instance.Hero);
            if (me.Champion != Player.Instance.Hero)
            {
                return;
            }

            R = new Spell.Skillshot(me.Slot, me.Range, me.Type, me.CastDelay, me.Speed, me.Width);

            baseMenu = MainMenu.AddMenu("Kapa基地大招", "KappaBaseUlt");
            baseMenu.AddGroupLabel(Player.Instance.Hero + " BaseUlt");
            baseMenu.AddSeparator(0);
            baseMenu.AddGroupLabel("按键设置:");
            baseMenu.Add("enable", new KeyBind("开启基地大招", true, KeyBind.BindTypes.PressToggle, 'K'));
            baseMenu.Add("disable", new KeyBind("屏蔽按键", false, KeyBind.BindTypes.HoldActive, 32));
            baseMenu.AddSeparator(0);
            baseMenu.AddGroupLabel("设置:");
            baseMenu.Add("col", new CheckBox("检查体积碰撞"));
            baseMenu.Add("limit", new Slider("战争迷雾时间限制 [{0}]", 120, 0, 180));
            baseMenu.AddLabel("0 = 总是限制");
            baseMenu.AddSeparator(0);
            baseMenu.AddGroupLabel("线圈:");
            baseMenu.Add("draw", new CheckBox("显示调试信息"));
            baseMenu.AddGroupLabel("对以下使用:");
            RecallsList.Clear();
            foreach (var enemy in EntityManager.Heroes.Enemies)
            {
                baseMenu.Add(enemy.NetworkId.ToString(), new CheckBox("使用 " + enemy.BaseSkinName + " - (" + enemy.Name + ")"));
                RecallsList.Add(new EnemyInfo(enemy));
            }

            Game.OnTick += Game_OnTick;
            Teleport.OnTeleport += Teleport_OnTeleport;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Game_OnTick(EventArgs args)
        {
            foreach (var enemy in RecallsList.Where(e => e.Enemy.IsHPBarRendered && !e.Enemy.IsDead))
            {
                enemy.lastseen = Game.Time;
            }

            foreach (var enemy in RecallsList.Where(d => baseMenu[d.Enemy.NetworkId.ToString()].Cast<CheckBox>().CurrentValue && d.Duration > 0))
            {
                DoBaseUlt(enemy);
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!baseMenu["draw"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            var X = Player.Instance.ServerPosition.WorldToScreen().X;
            var Y = Player.Instance.ServerPosition.WorldToScreen().Y;

            foreach (var player in RecallsList.Where(e => baseMenu[e.Enemy.NetworkId.ToString()].Cast<CheckBox>().CurrentValue && e.Duration > 0))
            {
                Drawing.DrawText(
                    X,
                    Y,
                    System.Drawing.Color.White,
                    player.Enemy.BaseSkinName + " | CountDown: " + (player.CountDown()) + " | TravelTime: " + player.Enemy.traveltime()
                    + " | LastSeen: " + (Game.Time - player.lastseen) + " | Damage: " + (player.Enemy.GetDamage()) + " | Health: "
                    + player.Enemy.TotalShieldHealth(),
                    5);
            }
            /*
            var order = ObjectManager.Get<GameObject>().FirstOrDefault(name => name.Name == "__Spawn_T1");
            var choes = ObjectManager.Get<GameObject>().FirstOrDefault(name => name.Name == "__Spawn_T2");
            Circle.Draw(Color.White, 100, order);
            Circle.Draw(Color.White, 100, choes);
            */
        }

        private static void Teleport_OnTeleport(Obj_AI_Base sender, Teleport.TeleportEventArgs args)
        {
            if (!sender.IsEnemy || args.Type != TeleportType.Recall)
            {
                return;
            }

            if (args.Status == TeleportStatus.Start)
            {
                if (RecallsList.Exists(s => s.Enemy.NetworkId.Equals(sender.NetworkId)))
                {
                    RecallsList.Add(
                        new EnemyInfo(sender) { Duration = args.Duration, Started = args.Start, RecallDuration = args.Duration + Core.GameTickCount });
                }
                else
                {
                    RecallsList.Add(
                        new EnemyInfo(sender) { Duration = args.Duration, Started = args.Start, RecallDuration = args.Duration + Core.GameTickCount });
                }
            }
            else
            {
                var remove = RecallsList.FirstOrDefault(r => r.Enemy.NetworkId.Equals(sender.NetworkId));
                if (remove == null)
                {
                    return;
                }
                RecallsList.Remove(remove);
                removeFromList(remove.Enemy);
            }
        }

        private static bool Killable(this Obj_AI_Base target)
        {
            return target.GetDamage() >= target.TotalShieldHealth();
        }

        private static float GetDamage(this Obj_AI_Base target)
        {
            if (!R.IsLearned || !R.IsReady())
            {
                return 0;
            }

            var missinghealth = target.MaxHealth - target.Health;
            var level = R.Level - 1;
            var hero = Player.Instance.Hero;
            var AD = Player.Instance.TotalAttackDamage;
            var AP = Player.Instance.TotalMagicalDamage;

            var champion = Database.Damages.FirstOrDefault(h => h.Champion == hero);

            if (champion.Champion != hero)
            {
                return 0;
            }

            if (champion.DamageType == DamageType.Magical)
            {
                Damage = champion.Floats[level] + champion.Float * AP;
            }

            if (champion.DamageType == DamageType.Physical)
            {
                Damage = champion.Floats[level] + champion.Float * AD;
            }

            if (champion.Champion == Champion.Ezreal)
            {
                Damage = champion.Floats[level] + (1f * AD + (0.9f * AP));
            }

            if (champion.Champion == Champion.Gangplank)
            {
                Damage = (champion.Floats[level] + champion.Float * AP) * 3;
            }

            if (champion.Champion == Champion.Jinx)
            {
                Damage = champion.Floats[level] + new[] { 0.25f * missinghealth, 0.30f * missinghealth, 0.35f * missinghealth }[level] + (0.1f * AD);
            }

            return Player.Instance.CalculateDamageOnUnit(target, champion.DamageType, Damage);
        }

        private static void removeFromList(Obj_AI_Base sender)
        {
            var recall = RecallsList.FirstOrDefault(x => x.Enemy.NetworkId.Equals(sender.NetworkId));

            if (recall == null)
            {
                return;
            }

            RecallsList.Remove(recall);
        }

        private static float traveltime(this Obj_AI_Base target)
        {
            var hero = Player.Instance.Hero;
            var pos = target.Fountain();
            var distance = Player.Instance.Distance(pos);
            var speed = R.Speed;

            if (hero == Champion.Lux || hero == Champion.Karthus || hero == Champion.Pantheon || hero == Champion.Gangplank)
            {
                return R.CastDelay;
            }

            return ((distance / speed) * 1000f) + R.CastDelay;
        }

        private static void DoBaseUlt(EnemyInfo target)
        {
            var disable = baseMenu["disable"].Cast<KeyBind>().CurrentValue;
            var enable = baseMenu["enable"].Cast<KeyBind>().CurrentValue;
            float CountDown = target.CountDown();
            float Traveltime = traveltime(target.Enemy);

            if (enable && !disable)
            {
                if (lastseen(target) && CountDown >= Traveltime && target.Enemy.Killable())
                {
                    if (CountDown - Traveltime < 60 && !target.Enemy.Collison())
                    {
                        Player.CastSpell(R.Slot, target.Enemy.Fountain());
                    }
                }
            }
        }

        private static bool lastseen(EnemyInfo target)
        {
            float timelimit = baseMenu["limit"].Cast<Slider>().CurrentValue;
            if (timelimit.Equals(0))
            {
                return true;
            }
            return Game.Time - target.lastseen < timelimit;
        }

        private static float CountDown(this EnemyInfo target)
        {
            return target.Started + target.Duration - Core.GameTickCount;
        }

        private static bool Collison(this Obj_AI_Base target)
        {
            var col = baseMenu["col"].Cast<CheckBox>().CurrentValue;
            var Rectangle = new Geometry.Polygon.Rectangle(Player.Instance.ServerPosition, target.Fountain(), R.Width);

            return col && EntityManager.Heroes.Enemies.Count(e => Rectangle.IsInside(e) && e.IsValidTarget()) > R.AllowedCollisionCount;
        }

        private static Vector3 Fountain(this Obj_AI_Base target)
        {
            var objSpawnPoint = ObjectManager.Get<Obj_SpawnPoint>().FirstOrDefault(x => x.Team == target.Team);
            return objSpawnPoint?.Position ?? Vector3.Zero;
        }

        public class EnemyInfo
        {
            public Obj_AI_Base Enemy;

            public float lastseen;

            public float RecallDuration;

            public float Duration;

            public float Started;

            public EnemyInfo(Obj_AI_Base enemy)
            {
                this.Enemy = enemy;
                this.lastseen = 0f;
                this.Duration = 0f;
            }
        }
    }
}