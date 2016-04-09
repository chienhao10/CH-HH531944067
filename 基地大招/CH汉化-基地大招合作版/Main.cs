using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace HumanziedBaseUlt
{
    class Main : Events
    {
        private readonly AIHeroClient me = ObjectManager.Player;
        // ReSharper disable once FieldCanBeMadeReadOnly.Local       

        public Main()
        {
            Listing.config = MainMenu.AddMenu("CH汉化-基地大招", "humanizedBaseUlts");
            Listing.config.Add("on", new CheckBox("开启"));
            Listing.config.Add("min20", new CheckBox("经过了20分钟"));
            Listing.config.Add("minDelay", new Slider("最低大招延迟", 1000, 0, 2500));
            Listing.config.AddLabel("延迟时间等于敌人在基地回血的时间");

            Listing.config.AddSeparator(20);
            Listing.config.Add("fountainReg", new Slider("泉水回血速度", 89, 85, 92));
            Listing.config.Add("fountainRegMin20", new Slider("20分钟后泉水回血速度", 364, 350, 370));


            Listing.potionMenu = Listing.config.AddSubMenu("药水");
            Listing.potionMenu.AddLabel("[回血速度 HP/秒.]");
            Listing.potionMenu.Add("healPotionRegVal", new Slider("药水 / 饼干", 10, 5, 20));
            Listing.potionMenu.Add("crystalFlaskRegVal", new Slider("可充药剂", 10, 5, 20));
            Listing.potionMenu.Add("crystalFlaskJungleRegVal", new Slider("猎人药水", 9, 5, 20));
            Listing.potionMenu.Add("darkCrystalFlaskVal", new Slider("腐蚀药水", 16, 5, 20));


            Listing.snipeMenu = Listing.config.AddSubMenu("敌人回城时狙击");
            Listing.snipeMenu.AddLabel("[联合技能已添加]");

            Listing.snipeMenu.Add("snipeEnabled", new CheckBox("开启"));
            AddStringList(Listing.snipeMenu, "minSnipeHitChance", "最低狙击命中率", 
                new []{ "非常低", "低", "中高", "非常高"}, 2);
            Listing.snipeMenu.Add("snipeDraw", new CheckBox("显示狙击路线"));
            Listing.snipeMenu.Add("snipeCinemaMode", new CheckBox("影院观看模式 ™"));
            Listing.snipeMenu.Add("snipeAccuracy", new Slider("准确度", 10));
            Listing.snipeMenu.AddLabel("降低可防止掉FPS");

            Listing.allyconfig = Listing.config.AddSubMenu("队友");
            foreach (var ally in EntityManager.Heroes.Allies)
            {
                if (Listing.UltSpellDataList.Any(x => x.Key == ally.ChampionName))
                    Listing.allyconfig.Add(ally.ChampionName + "/联合", new CheckBox(ally.ChampionName, ally.IsMe));
            }


            Listing.MiscMenu = Listing.config.AddSubMenu("杂项");
            Listing.MiscMenu.AddLabel("[德莱文]");
            Listing.MiscMenu.Add("dravenCastBackBool", new CheckBox("开启 '德莱文R2'"));
            Listing.MiscMenu.Add("dravenCastBackDelay", new Slider("提早开启R2 X 毫秒", 400, 0, 500));

            Listing.MiscMenu.AddSeparator();
            Listing.MiscMenu.Add("allyMessaging", new CheckBox("给队友发消息"));
            Listing.MiscMenu.AddLabel("如果只有1名玩家使用这个脚本，其他玩家会通过游戏私聊收到消息");

            AddStringList(Listing.MiscMenu, "damageCalcMethod", "伤害计算逻辑", 
                new [] {"Elobudddy", "Leaguesharp"}, 1);
            Listing.MiscMenu.AddLabel("EB暂时不支持计算天赋");

            Game.OnUpdate += GameOnOnUpdate;
            Teleport.OnTeleport += TeleportOnOnTeleport;
            OnEnemyInvisible += OnOnEnemyInvisible;
            OnEnemyVisible += OnOnEnemyVisible;
        }

        private void AddStringList(Menu m, string uniqueId, string displayName, string[] values, int defaultValue)
        {
            var mode = m.Add(uniqueId, new Slider(displayName, defaultValue, 0, values.Length - 1));
            mode.DisplayName = displayName + ": " + values[mode.CurrentValue];
            mode.OnValueChange += delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
            {
                sender.DisplayName = displayName + ": " + values[args.NewValue];
            };
        }

        private void TeleportOnOnTeleport(Obj_AI_Base sender, Teleport.TeleportEventArgs args)
        {
            var invisEnemiesEntry = Listing.invisEnemiesList.FirstOrDefault(x => x.sender == sender);

            switch (args.Status)
            {
                case TeleportStatus.Start:
                    if (invisEnemiesEntry == null)
                        return;
                    if (Listing.teleportingEnemies.All(x => x.Sender != sender))
                    {
                        Listing.teleportingEnemies.Add(new Listing.PortingEnemy
                        {
                            Sender = (AIHeroClient) sender,
                            Duration = args.Duration,
                            StartTick = args.Start,
                        });
                    }
                    break;
                case TeleportStatus.Abort:
                    var teleportingEnemiesEntry = Listing.teleportingEnemies.First(x => x.Sender.Equals(sender));
                    Listing.teleportingEnemies.Remove(teleportingEnemiesEntry);
                    break;

                case TeleportStatus.Finish:
                    var teleportingEnemiesEntry2 = Listing.teleportingEnemies.First(x => x.Sender.Equals(sender));
                    Core.DelayAction(() => Listing.teleportingEnemies.Remove(teleportingEnemiesEntry2), 10000);
                    break;
            }
        }
              
        /*enemy appear*/
        private void OnOnEnemyVisible(AIHeroClient sender)
        {
            Listing.visibleEnemies.Add(sender);
            var invisEntry = Listing.invisEnemiesList.First(x => x.sender.Equals(sender));
            Listing.invisEnemiesList.Remove(invisEntry);

            var sinpeEntry = Listing.Pathing.enemySnipeProcs.FirstOrDefault(x => x.target == sender);
            sinpeEntry.CancelProcess();
            Listing.Pathing.enemySnipeProcs.Remove(sinpeEntry);
        }
        
        /*enemy disappear*/
        private void OnOnEnemyInvisible(InvisibleEventArgs args)
        {
            Listing.visibleEnemies.Remove(args.sender);
            Listing.invisEnemiesList.Add(args);

            if (Listing.snipeMenu["snipeEnabled"].Cast<CheckBox>().CurrentValue && 
                me.Spellbook.GetSpell(SpellSlot.R).IsReady && me.Mana >= 100)
                Listing.Pathing.enemySnipeProcs.Add(new SnipePrediction(args));
        }

        private void GameOnOnUpdate(EventArgs args)
        {
            Listing.config.Get<CheckBox>("min20").CurrentValue = Game.Time > 1455;

            UpdateEnemyVisibility();
            Listing.Pathing.UpdateEnemyPaths();
            CheckRecallingEnemies();
        }

        Vector3 enemySpawn {
            get { return ObjectManager.Get<Obj_SpawnPoint>().First(x => x.IsEnemy).Position; }
        }

        private void CheckRecallingEnemies()
        {
            if (!Listing.config.Get<CheckBox>("on").CurrentValue)
                return;

            foreach (Listing.PortingEnemy portingEnemy in Listing.teleportingEnemies.OrderBy(x => x.Sender.Health))
            {
                var enemy = portingEnemy.Sender;
                InvisibleEventArgs invisEntry = Listing.invisEnemiesList.First(x => x.sender.Equals(enemy));

                int recallEndTime = portingEnemy.StartTick + portingEnemy.Duration;
                float timeLeft = recallEndTime - Core.GameTickCount;
                float travelTime = Algorithm.GetUltTravelTime(me, enemySpawn);

                float regedHealthRecallFinished = Algorithm.SimulateHealthRegen(enemy, invisEntry.StartTime, recallEndTime);
                float totalEnemyHOnRecallEnd = enemy.Health + regedHealthRecallFinished;

                float aioDmg = Damage.GetAioDmg(enemy, timeLeft, enemySpawn);

                float regenerationDelayTime = Algorithm.SimulateRealDelayTime(enemy, recallEndTime, aioDmg);

                if (aioDmg > totalEnemyHOnRecallEnd)
                {
                    if (regenerationDelayTime < Listing.config.Get<Slider>("minDelay").CurrentValue)
                    {
                        Messaging.ProcessInfo(enemy.ChampionName, Messaging.MessagingType.DelayTooSmall, regenerationDelayTime);
                        continue;
                    }

                    CheckUltCast(enemy, timeLeft, travelTime, aioDmg, regenerationDelayTime);
                }
                else if (aioDmg > 0) /*not enough damage at all (maybe not enough time?)*/
                {
                    //dmg there but not enough
                    Messaging.ProcessInfo(enemy.ChampionName, Messaging.MessagingType.NotEnougDamage, aioDmg);
                }
            }
        }

        private void CheckUltCast(AIHeroClient enemy, float timeLeft, float travelTime, float aioDmg, float regenerationDelayTime)
        {
            Messaging.ProcessInfo(enemy.ChampionName, Messaging.MessagingType.DelayInfo, regenerationDelayTime);

            if (travelTime < timeLeft + regenerationDelayTime)
            {
                Vector3 enemyBaseVec =
                    ObjectManager.Get<Obj_SpawnPoint>().First(x => x.IsEnemy).Position;
                float delay = timeLeft + regenerationDelayTime - travelTime;

                Core.DelayAction(() =>
                {
                    if (Listing.teleportingEnemies.All(x => x.Sender != enemy))
                        return;

                    Player.CastSpell(SpellSlot.R, enemyBaseVec);

                    /*Draven*/
                    if (Listing.config.Get<CheckBox>("dravenCastBackBool").CurrentValue)
                    {
                        int castBackReduction = Listing.config.Get<Slider>("dravenCastBackDelay").CurrentValue;
                        if (me.ChampionName == "Draven")
                            Core.DelayAction(() =>
                            {
                                Player.CastSpell(SpellSlot.R);
                            }, (int)(travelTime - castBackReduction));
                    }
                    /*Draven*/
                },
                (int)delay);
                Debug.Init(enemy, Algorithm.GetLastEstimatedEnemyReg(), aioDmg);
            }


            //Cleaning
            AllyMessaging.SendBaseUltInfoToAllies(timeLeft, regenerationDelayTime);
            Listing.teleportingEnemies.RemoveAll(x => x.Sender.ChampionName != enemy.ChampionName);
        }

        private void UpdateEnemyVisibility()
        {
            foreach (var enemy in EntityManager.Heroes.Enemies)
            {
                if (enemy.IsHPBarRendered && !Listing.visibleEnemies.Contains(enemy))
                {
                    FireOnEnemyVisible(enemy);
                }
                else if (!enemy.IsHPBarRendered && Listing.visibleEnemies.Contains(enemy))
                {
                    FireOnEnemyInvisible(new InvisibleEventArgs
                    {
                        StartTime = Core.GameTickCount,
                        sender = enemy,
                        LastRealPath = Listing.Pathing.GetLastEnemyPath(enemy)
                    });
                }
            }
        }
    }
}