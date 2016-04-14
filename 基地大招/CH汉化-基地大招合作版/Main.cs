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
            Listing.config.Add("humanizedDelayOff", new CheckBox("完全关闭人性化延迟", false));

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

            Listing.allyconfig = Listing.config.AddSubMenu("Premades");
            foreach (var ally in EntityManager.Heroes.Allies)
            {
                bool isKarthus = ally.ChampionName.ToLower().Contains("karthus");
                if (Listing.UltSpellDataList.Any(x => x.Key == ally.ChampionName))
                    Listing.allyconfig.Add(ally.ChampionName + "/Premade", new CheckBox(!isKarthus ? ally.ChampionName :
                       ally.ChampionName + " (Only for premade damage)", ally.IsMe));
            }
            

            Listing.MiscMenu = Listing.config.AddSubMenu("杂项");
            Listing.MiscMenu.AddLabel("[德莱文]");
            Listing.MiscMenu.Add("dravenCastBackBool", new CheckBox("开启 '德莱文R2'"));
            Listing.MiscMenu.Add("dravenCastBackDelay", new Slider("提早开启R2 X 毫秒", 400, 0, 500));

            Listing.MiscMenu.AddSeparator();
            Listing.MiscMenu.Add("allyMessaging", new CheckBox("给队友发送消息"));
            Listing.MiscMenu.AddLabel("如果只有一名开黑队友使用这个脚本，将会发私信给其他路人队友");

            RecallTracker.RecallTracker.Init();

            Game.OnUpdate += GameOnOnUpdate;
            Teleport.OnTeleport += TeleportOnOnTeleport;
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
                    var teleportingEnemiesEntry = Listing.teleportingEnemies.FirstOrDefault(x => x.Sender.Equals(sender));
                    if (teleportingEnemiesEntry != null)
                        Listing.teleportingEnemies.Remove(teleportingEnemiesEntry);
                    break;

                case TeleportStatus.Finish:
                    var teleportingEnemiesEntry2 = Listing.teleportingEnemies.FirstOrDefault(x => x.Sender.Equals(sender));
                    if (teleportingEnemiesEntry2 != null)
                        Core.DelayAction(() => Listing.teleportingEnemies.Remove(teleportingEnemiesEntry2), 10000);
                    break;
            }
        }
              

        private void OnOnEnemyVisible(AIHeroClient sender)
        {
            Listing.visibleEnemies.Add(sender);
            var invisEntry = Listing.invisEnemiesList.FirstOrDefault(x => x.sender.Equals(sender));

            if (invisEntry != null)
                Listing.invisEnemiesList.Remove(invisEntry);

            var snipeEntry = Listing.Pathing.enemySnipeProcs.FirstOrDefault(x => x.target == sender);

            if (snipeEntry != null)
            {
                snipeEntry.CancelProcess();
                Listing.Pathing.enemySnipeProcs.Remove(snipeEntry);
            }
        }
        

        private void OnOnEnemyInvisible(InvisibleEventArgs args)
        {
            Listing.visibleEnemies.Remove(args.sender);
            Listing.invisEnemiesList.Add(args);

            if (Listing.snipeMenu["snipeEnabled"].Cast<CheckBox>().CurrentValue)
            {
                var spell = me.Spellbook.GetSpell(SpellSlot.R);
                var cooldown = spell.CooldownExpires - Game.Time;

                if (cooldown <= 0 && me.Level >= 6)
                    Listing.Pathing.enemySnipeProcs.Add(new SnipePrediction(args));
            }
        }

        private void GameOnOnUpdate(EventArgs args)
        {
            Listing.config.Get<CheckBox>("min20").CurrentValue = Game.Time > 1221;

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
                InvisibleEventArgs invisEntry = Listing.invisEnemiesList.FirstOrDefault(x => x.sender.Equals(enemy));

                if (invisEntry == null) //enemy visible
                    return;

                int recallEndTime = portingEnemy.StartTick + portingEnemy.Duration;
                float timeLeft = recallEndTime - Core.GameTickCount;
                float regedHealthRecallFinished = Algorithm.SimulateHealthRegen(enemy, invisEntry.StartTime, recallEndTime);
                float totalEnemyHOnRecallEnd = enemy.Health + regedHealthRecallFinished;
                float aioDmg = Damage.GetAioDmg(enemy, timeLeft, enemySpawn, totalEnemyHOnRecallEnd);
                float regenerationDelayTime = Algorithm.SimulateRealDelayTime(enemy, recallEndTime, aioDmg);

                bool force0Delay = Listing.config.Get<CheckBox>("humanizedDelayOff").CurrentValue;
                if (force0Delay)
                    regenerationDelayTime = 0;

                if (!Damage.isRegenDelaySet)
                {
                    Damage.SetRegenerationDelay(regenerationDelayTime, timeLeft);
                    //new check but now with estimated reg delay
                    float totalEnemyHpAtArrival = totalEnemyHOnRecallEnd +
                                                  (regenerationDelayTime/1000)*Algorithm.GetFountainReg(enemy);
                    aioDmg = Damage.GetAioDmg(enemy, timeLeft, enemySpawn, totalEnemyHpAtArrival);
                }

                if (aioDmg > totalEnemyHOnRecallEnd)
                {
                    if (regenerationDelayTime < Listing.config.Get<Slider>("minDelay").CurrentValue &&
                        !Listing.config.Get<CheckBox>("humanizedDelayOff").CurrentValue)
                    {
                        Messaging.ProcessInfo(enemy.ChampionName, Messaging.MessagingType.DelayTooSmall, regenerationDelayTime);
                        continue;
                    }

                    CheckUltCast(enemy, timeLeft, aioDmg, regenerationDelayTime);
                    return; //clear every porting enemy expect the current target and return => wait for next loop run
                }

                if (aioDmg > 0)  //dmg there but not enough
                {
                    Messaging.ProcessInfo(enemy.ChampionName, Messaging.MessagingType.NotEnougDamage, aioDmg);
                }
            }
        }

        private void CheckUltCast(AIHeroClient enemy, float timeLeft, float aioDmg, float regenerationDelayTime)
        {
            Messaging.ProcessInfo(enemy.ChampionName, Messaging.MessagingType.OwnDelayInfo, regenerationDelayTime);
            float travelTime = Algorithm.GetUltTravelTime(me, enemySpawn);

            float delay = timeLeft + regenerationDelayTime - travelTime;

            #region ownCheck
            if (RecallTracker.RecallTracker.Recalls.Any(x => x.Unit.ChampionName == enemy.ChampionName))
            {
                
                var recall = RecallTracker.RecallTracker.Recalls.First(x => x.Unit.ChampionName == enemy.ChampionName);
                recall.SetBaseUltShootTime(Environment.TickCount + delay);
            }

            if (travelTime > timeLeft + regenerationDelayTime && Damage.DidOwnChampWait)
            {
                CastUltInBase(enemy);
                Debug.Init(enemy, Algorithm.GetLastEstimatedEnemyReg(), aioDmg);
            }
            #endregion ownCheck


            //Cleaning
            Listing.teleportingEnemies.RemoveAll(x => x.Sender.ChampionName != enemy.ChampionName);
            AllyMessaging.SendBaseUltInfoToAllies(timeLeft, regenerationDelayTime);
        }

        private void CastUltInBase(AIHeroClient enemy)
        {
            float travelTime = Algorithm.GetUltTravelTime(me, enemySpawn);

            if (Listing.teleportingEnemies.All(x => x.Sender != enemy))
                return;

            Player.CastSpell(SpellSlot.R, enemySpawn);

            /*Draven*/
            if (Listing.MiscMenu.Get<CheckBox>("dravenCastBackBool").CurrentValue && me.ChampionName == "Draven")
            {
                int castBackReduction = Listing.MiscMenu.Get<Slider>("dravenCastBackDelay").CurrentValue;
                Core.DelayAction(() =>
                {
                    Player.CastSpell(SpellSlot.R);
                }, (int)(travelTime - castBackReduction));
            }
            /*Draven*/
        }

        private void UpdateEnemyVisibility()
        {
            foreach (var enemy in EntityManager.Heroes.Enemies)
            {
                if (enemy.IsHPBarRendered && !Listing.visibleEnemies.Contains(enemy))
                {
                    OnOnEnemyVisible(enemy);
                }
                else if (!enemy.IsHPBarRendered && Listing.visibleEnemies.Contains(enemy))
                {
                    OnOnEnemyInvisible(new InvisibleEventArgs
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
