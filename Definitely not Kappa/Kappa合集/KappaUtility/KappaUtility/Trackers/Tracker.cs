namespace KappaUtility.Trackers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;

    using SharpDX;

    using Color = System.Drawing.Color;

    internal class Tracker
    {
        public static TeleportStatus teleport;

        private static Vector2 PingLocation;

        public static readonly List<Recall> Recalls = new List<Recall>();

        private static int LastPingT;

        public static Menu TrackMenu { get; private set; }

        internal static void OnLoad()
        {
            TrackMenu = Load.UtliMenu.AddSubMenu("记录器");
            TrackMenu.AddGroupLabel("记录器设置");
            TrackMenu.Add("Track", new CheckBox("记录敌方状态", false));
            TrackMenu.Add("trackrecalls", new CheckBox("计时敌方回城", false));
            TrackMenu.Add("Tracktraps", new CheckBox("计时敌方陷阱 [测试]", false));
            TrackMenu.Add("Trackping", new CheckBox("对可击杀敌人点出信号 (本地)", false));
            TrackMenu.Add("Trackway", new CheckBox("显示敌人行走路径", false));
            TrackMenu.AddSeparator();
            TrackMenu.AddGroupLabel("投降记录");
            TrackMenu.Add("Trackally", new CheckBox("记录友军头投降", false));
            TrackMenu.Add("Trackenemy", new CheckBox("记录敌军头投降", false));
            TrackMenu.Add("Distance", new Slider("行走路径探测范围", 1000, 0, 5000));
            TrackMenu.AddSeparator();
            TrackMenu.AddGroupLabel("记录器位置设置");
            TrackMenu.Add("trackx", new Slider("记录器位置 X", 0, 0, 100));
            TrackMenu.Add("tracky", new Slider("记录器位置 Y", 0, 0, 100));
            TrackMenu.AddSeparator();
            TrackMenu.AddGroupLabel("不记录:");
            foreach (var enemy in ObjectManager.Get<AIHeroClient>())
            {
                var cb = new CheckBox(enemy.BaseSkinName) { CurrentValue = false };
                if (enemy.Team != Player.Instance.Team)
                {
                    TrackMenu.Add("DontTrack" + enemy.BaseSkinName, cb);
                }
            }

            foreach (var hero in ObjectManager.Get<AIHeroClient>())
            {
                Recalls.Add(new Recall(hero, RecallStatus.Inactive));
            }

            Teleport.OnTeleport += OnTeleport;
        }

        public static void track()
        {
            foreach (var enemy in
                ObjectManager.Get<AIHeroClient>()
                    .Where(ene => ene != null && !ene.IsDead && ene.IsEnemy && ene.IsValid)
                    .Where(
                        enemy =>
                        CalcDamage(enemy) >= enemy.TotalShieldHealth()
                        && !TrackMenu["DontTrack" + enemy.BaseSkinName].Cast<CheckBox>().CurrentValue)
                    .Where(
                        enemy =>
                        TrackMenu.Get<CheckBox>("Trackping").CurrentValue && enemy.IsVisible && enemy.IsHPBarRendered))
            {
                Ping(enemy.Position.To2D());
            }
        }

        private static void Ping(Vector2 position)
        {
            if (Environment.TickCount - LastPingT < 30 * 1000)
            {
                return;
            }

            LastPingT = Environment.TickCount;
            PingLocation = position;
            SimplePing();
            Core.DelayAction(SimplePing, 150);
            Core.DelayAction(SimplePing, 450);
            Core.DelayAction(SimplePing, 750);
        }

        private static void SimplePing()
        {
            TacticalMap.ShowPing(PingCategory.Danger, PingLocation, true);
        }

        public static int CalcDamage(Obj_AI_Base target)
        {
            var aa = Player.Instance.GetAutoAttackDamage(target, true);
            var damage = aa;

            if (Player.Instance.Spellbook.GetSpell(SpellSlot.Q).IsReady)
            {
                // Q damage
                damage += Player.Instance.GetSpellDamage(target, SpellSlot.Q);
            }

            if (Player.Instance.Spellbook.GetSpell(SpellSlot.W).IsReady)
            {
                // W damage
                damage += Player.Instance.GetSpellDamage(target, SpellSlot.W);
            }

            if (Player.Instance.Spellbook.GetSpell(SpellSlot.E).IsReady)
            {
                // E damage
                damage += Player.Instance.GetSpellDamage(target, SpellSlot.E);
            }

            if (Player.Instance.Spellbook.GetSpell(SpellSlot.R).IsReady)
            {
                // R damage
                damage += Player.Instance.GetSpellDamage(target, SpellSlot.R);
            }

            return (int)damage;
        }

        private static void OnTeleport(Obj_AI_Base sender, Teleport.TeleportEventArgs args)
        {
            if (sender.IsAlly || sender.IsMe)
            {
                return;
            }

            teleport = args.Status;
            if (!(sender is AIHeroClient))
            {
                return;
            }

            var unit = Recalls.Find(h => h.Unit.NetworkId == sender.NetworkId);

            if (unit == null || args.Type != TeleportType.Recall)
            {
                return;
            }

            switch (teleport)
            {
                case TeleportStatus.Start:
                    {
                        unit.Status = RecallStatus.Active;
                        unit.Started = Game.Time;
                        unit.Duration = (float)args.Duration / 1000;
                        break;
                    }

                case TeleportStatus.Abort:
                    {
                        unit.Status = RecallStatus.Abort;
                        unit.Ended = Game.Time;

                        if (Game.Time == unit.Ended)
                        {
                            Core.DelayAction(() => unit.Status = RecallStatus.Inactive, 2000);
                        }
                        break;
                    }

                case TeleportStatus.Finish:
                    {
                        unit.Status = RecallStatus.Finished;
                        unit.Ended = Game.Time;

                        if (Game.Time == unit.Ended)
                        {
                            Core.DelayAction(() => unit.Status = RecallStatus.Inactive, 2000);
                        }
                        break;
                    }
            }
        }

        internal static void HPtrack()
        {
            var trackx = TrackMenu["trackx"].Cast<Slider>().CurrentValue;
            var tracky = TrackMenu["tracky"].Cast<Slider>().CurrentValue;
            float timer = 0;
            float i = 0;

            foreach (var champ in
                Recalls.Where(
                    hero =>
                    hero != null && hero.Unit.IsEnemy
                    && !TrackMenu["DontTrack" + hero.Unit.BaseSkinName].Cast<CheckBox>().CurrentValue))
            {
                var hero = champ.Unit;
                if (TrackMenu["Track"].Cast<CheckBox>().CurrentValue)
                {
                    var champion = hero.ChampionName;
                    if (champion.Length > 12)
                    {
                        champion = champion.Remove(7) + "..";
                    }

                    var percent = (int)hero.HealthPercent;
                    var color = Color.FromArgb(194, 194, 194);

                    if (percent > 0)
                    {
                        color = Color.Red;
                    }

                    if (percent > 25)
                    {
                        color = Color.Orange;
                    }

                    if (percent > 50)
                    {
                        color = Color.Yellow;
                    }

                    if (percent > 75)
                    {
                        color = Color.LimeGreen;
                    }

                    Drawing.DrawText(
                        (Drawing.Width * 0.01f) + (trackx * 20),
                        (Drawing.Height * 0.1f) + (tracky * 10) + i,
                        color,
                        champion);
                    Drawing.DrawText(
                        (Drawing.Width * 0.06f) + (trackx * 20),
                        (Drawing.Height * 0.1f) + (tracky * 10) + i,
                        color,
                        (" ( " + (int)hero.TotalShieldHealth()) + " / " + (int)hero.MaxHealth + " | " + percent + "% ) ");

                    if (hero.IsVisible && hero.IsHPBarRendered && !hero.IsDead)
                    {
                        Drawing.DrawText(
                            (Drawing.Width * 0.13f) + (trackx * 20),
                            (Drawing.Height * 0.1f) + (tracky * 10) + i,
                            color,
                            "     可视 ");
                    }
                    else
                    {
                        if (!hero.IsDead)
                        {
                            Drawing.DrawText(
                                (Drawing.Width * 0.13f) + (trackx * 20),
                                (Drawing.Height * 0.1f) + (tracky * 10) + i,
                                color,
                                "     不可视 ");
                        }
                    }
                    if (hero.IsDead)
                    {
                        Drawing.DrawText(
                            (Drawing.Width * 0.13f) + (trackx * 20),
                            (Drawing.Height * 0.1f) + (tracky * 10) + i,
                            color,
                            "     死亡 ");
                    }

                    if (hero.Health < CalcDamage(hero) && !hero.IsDead)
                    {
                        Drawing.DrawText(
                            (Drawing.Width * 0.18f) + (trackx * 20),
                            (Drawing.Height * 0.1f) + (tracky * 10) + i,
                            color,
                            "可击杀 ");
                    }

                    if (TrackMenu["trackrecalls"].Cast<CheckBox>().CurrentValue)
                    {
                        var recallpercent = GetRecallPercent(champ);

                        if (champ.Status != RecallStatus.Inactive)
                        {
                            if (champ.Status == RecallStatus.Active && (int)(recallpercent * 100) != 100)
                            {
                                Drawing.DrawText(
                                    (Drawing.Width * 0.22f) + (trackx * 20),
                                    (Drawing.Height * 0.1f) + (tracky * 10) + i,
                                    color,
                                    "回城中 " + (int)(recallpercent * 100) + "%");
                            }

                            if (champ.Status == RecallStatus.Finished)
                            {
                                Drawing.DrawText(
                                    (Drawing.Width * 0.22f) + (trackx * 20),
                                    (Drawing.Height * 0.1f) + (tracky * 10) + i,
                                    color,
                                    "回城完毕 ");
                            }

                            if (champ.Status == RecallStatus.Abort && (int)(recallpercent * 100) != 100)
                            {
                                Drawing.DrawText(
                                    (Drawing.Width * 0.22f) + (trackx * 20),
                                    (Drawing.Height * 0.1f) + (tracky * 10) + i,
                                    color,
                                    "回城终止 ");
                            }
                        }
                    }

                    i += 20f;
                }

                if (!TrackMenu.Get<CheckBox>("Trackway").CurrentValue
                    || (!(hero.Path.LastOrDefault().Distance(Player.Instance)
                          <= TrackMenu["Distance"].Cast<Slider>().CurrentValue)
                        || hero.IsInRange(hero.Path.LastOrDefault(), 50)))
                {
                    continue;
                }
                timer += hero.Position.Distance(hero.Path.LastOrDefault()) / hero.MoveSpeed;
                if ((int)timer > 500 || hero.IsDead)
                {
                    return;
                }

                Drawing.DrawLine(
                    hero.Position.WorldToScreen(),
                    hero.Path.LastOrDefault().WorldToScreen(),
                    2,
                    Color.White);
                Circle.Draw(SharpDX.Color.White, 50, hero.Path.LastOrDefault());
                Drawing.DrawText(
                    Drawing.WorldToScreen(hero.Path.LastOrDefault()) - new Vector2(25, -20),
                    Color.White,
                    hero.ChampionName + " " + timer.ToString("F"),
                    12);
            }
        }

        private static double GetRecallPercent(Recall recall)
        {
            var recallDuration = recall.Duration;
            var cd = recall.Started + recallDuration - Game.Time;
            var percent = (cd > 0 && Math.Abs(recallDuration) > float.Epsilon) ? 1f - (cd / recallDuration) : 1f;
            return percent;
        }

        public class Recall
        {
            public Recall(AIHeroClient unit, RecallStatus status)
            {
                this.Unit = unit;
                this.Status = status;
            }

            public AIHeroClient Unit { get; set; }

            public RecallStatus Status { get; set; }

            public float Started { get; set; }

            public float Ended { get; set; }

            public float Duration { get; set; }
        }

        public enum RecallStatus
        {
            Active,

            Inactive,

            Finished,

            Abort
        }
    }
}